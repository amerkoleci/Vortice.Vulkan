// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static bool generateSizeOfStructs = false;

    private static void GenerateStructAndUnions(CppCompilation compilation, string outputPath)
    {
        // Generate Structures
        using var writer = new CodeWriter(Path.Combine(outputPath, "Structures.cs"),
            false,
            "System.Runtime.InteropServices",
            "System.Runtime.CompilerServices",
            "System.Diagnostics.CodeAnalysis"
            );

        // Print All classes, structs
        foreach (CppClass? cppClass in compilation.Classes)
        {
            if (cppClass.ClassKind == CppClassKind.Class ||
                cppClass.SizeOf == 0 ||
                cppClass.Name.EndsWith("_T"))
            {
                continue;
            }

            // Handled manually.
            if (cppClass.Name == "VkClearColorValue"
                || cppClass.Name == "VkTransformMatrixKHR"
                || cppClass.Name == "VkAccelerationStructureInstanceKHR"
                || cppClass.Name == "VkAccelerationStructureSRTMotionInstanceNV"
                || cppClass.Name == "VkAccelerationStructureMatrixMotionInstanceNV"
                )
            {
                continue;
            }

            bool isUnion = cppClass.ClassKind == CppClassKind.Union;
            bool hasSType = false;
            if (cppClass.Fields.FirstOrDefault(item => item.Name == "sType") != null)
            {
                hasSType = true;
            }

            string csName = cppClass.Name;
            if (isUnion)
            {
                writer.WriteLine("[StructLayout(LayoutKind.Explicit)]");
            }
            else
            {
                writer.WriteLine("[StructLayout(LayoutKind.Sequential)]");
            }

            bool isReadOnly = false;
            string modifier = "partial";
            if (csName == "VkClearDepthStencilValue")
            {
                modifier = "readonly partial";
                isReadOnly = true;
            }

            bool handleSType = false;
            if (hasSType &&
                csName != "VkBaseInStructure" &&
                csName != "VkBaseOutStructure")
            {
                handleSType = true;
            }

            using (writer.PushBlock($"public {modifier} struct {csName}"))
            {
                if (generateSizeOfStructs && cppClass.SizeOf > 0)
                {
                    writer.WriteLine("/// <summary>");
                    writer.WriteLine($"/// The size of the <see cref=\"{csName}\"/> type, in bytes.");
                    writer.WriteLine("/// </summary>");
                    writer.WriteLine($"public static readonly int SizeInBytes = {cppClass.SizeOf};");
                    writer.WriteLine();
                }

                foreach (CppField cppField in cppClass.Fields)
                {
                    WriteField(writer, cppField, handleSType, isUnion, isReadOnly);
                }

                if (handleSType)
                {
                    string structureTypeValue = csName;
                    if (structureTypeValue.StartsWith("Vk"))
                    {
                        structureTypeValue = structureTypeValue.Substring(2);
                    }
                    if (structureTypeValue.EndsWith("ANDROID"))
                    {
                        structureTypeValue = structureTypeValue.Replace("ANDROID", "Android");
                    }

                    using (writer.PushBlock($"public static {csName} New()"))
                    {
                        writer.WriteLine($"Unsafe.SkipInit(out {csName} instance);");
                        writer.WriteLine($"instance.sType = VkStructureType.{structureTypeValue};");
                        writer.WriteLine($"return instance;");
                    }
                }
            }

            writer.WriteLine();
        }
    }

    private static void WriteField(CodeWriter writer, CppField field, bool handleSType, bool isUnion = false, bool isReadOnly = false)
    {
        string csFieldName = NormalizeFieldName(field.Name);

        if (isUnion)
        {
            writer.WriteLine("[FieldOffset(0)]");
        }

        if (field.Type is CppArrayType arrayType)
        {
            bool canUseFixed = false;
            if (arrayType.ElementType is CppPrimitiveType)
            {
                canUseFixed = true;
            }
            else if (arrayType.ElementType is CppTypedef typedef
                && typedef.ElementType is CppPrimitiveType)
            {
                canUseFixed = true;
            }

            if (canUseFixed)
            {
                string csFieldType = GetCsTypeName(arrayType.ElementType, false);
                writer.WriteLine($"public unsafe fixed {csFieldType} {csFieldName}[{arrayType.Size}];");
            }
            else
            {
                string csFieldType;
                if (arrayType.ElementType is CppArrayType elementArrayType)
                {
                    // vk-video madness
                    csFieldType = GetCsTypeName(elementArrayType.ElementType, false);
                    writer.WriteLine($"public unsafe fixed {csFieldType} {csFieldName}[{arrayType.Size} * {elementArrayType.Size}];");
                }
                else
                {
                    csFieldType = GetCsTypeName(arrayType.ElementType, false);

                    writer.WriteLine($"public {csFieldName}__FixedBuffer {csFieldName};");
                    writer.WriteLine();

                    using (writer.PushBlock($"public unsafe struct {csFieldName}__FixedBuffer"))
                    {
                        for (int i = 0; i < arrayType.Size; i++)
                        {
                            writer.WriteLine($"public {csFieldType} e{i};");
                        }
                        writer.WriteLine();

                        writer.WriteLine("[UnscopedRef]");
                        using (writer.PushBlock($"public ref {csFieldType} this[int index]"))
                        {
                            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            using (writer.PushBlock("get"))
                            {
                                if (csFieldType.EndsWith('*'))
                                {
                                    using (writer.PushBlock($"fixed ({csFieldType}* pThis = &e0)"))
                                    {
                                        writer.WriteLine($"return ref pThis[index];");
                                    }
                                }
                                else
                                {
                                    writer.WriteLine($"return ref AsSpan()[index];");
                                }
                            }
                        }
                        writer.WriteLine();

                        if (!csFieldType.EndsWith('*'))
                        {
                            writer.WriteLine("[UnscopedRef]");
                            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            using (writer.PushBlock($"public Span<{csFieldType}> AsSpan()"))
                            {
                                writer.WriteLine($"return MemoryMarshal.CreateSpan(ref e0, {arrayType.Size});");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // VkAllocationCallbacks members
            if (field.Type is CppTypedef typedef &&
                typedef.ElementType is CppPointerType pointerType &&
                pointerType.ElementType is CppFunctionType functionType)
            {
                StringBuilder builder = new();
                foreach (CppParameter parameter in functionType.Parameters)
                {
                    string paramCsType = GetCsTypeName(parameter.Type, false);
                    // Otherwise we get interop issues with non blittable types
                    if (paramCsType == "VkBool32")
                        paramCsType = "uint";
                    builder.Append(paramCsType).Append(", ");
                }

                string returnCsName = GetCsTypeName(functionType.ReturnType, false);
                // Otherwise we get interop issues with non blittable types
                if (returnCsName == "VkBool32")
                    returnCsName = "uint";

                builder.Append(returnCsName);

                writer.WriteLine("#if NET6_0_OR_GREATER");
                writer.WriteLine($"public unsafe delegate* unmanaged<{builder}> {csFieldName};");
                writer.WriteLine("#else");
                writer.WriteLine($"public IntPtr {csFieldName};");
                writer.WriteLine("#endif");
                return;
            }

            string csFieldType = GetCsTypeName(field.Type, false);
            if (csFieldName.Equals("specVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("applicationVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("engineVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
            {
                csFieldType = "VkVersion";
            }

            if (field.Type.ToString() == "ANativeWindow*")
            {
                csFieldType = "IntPtr";
            }
            else if (field.Type.ToString() == "CAMetalLayer*"
                || field.Type.ToString() == "const CAMetalLayer*")
            {
                csFieldType = "IntPtr";
            }
            else if (csFieldType == "VkDirectDriverLoadingFlagsLUNARG")
            {
                csFieldType = "VkDirectDriverLoadingModeLUNARG";
            }

            string fieldPrefix = isReadOnly ? "readonly " : string.Empty;
            if (csFieldType.EndsWith('*'))
            {
                fieldPrefix += "unsafe ";
            }

            writer.WriteLine($"public {fieldPrefix}{csFieldType} {csFieldName};");
        }
    }
}
