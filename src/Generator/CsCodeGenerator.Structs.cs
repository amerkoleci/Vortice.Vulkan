﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static void GenerateStructAndUnions(CppCompilation compilation)
    {
        if (compilation.Classes.Count == 0)
            return;

        List<string> usings =
        [
            "System.Runtime.InteropServices",
            "System.Runtime.CompilerServices",
            "System.Diagnostics.CodeAnalysis"
        ];
        if (_options.ExtraUsings.Count > 0)
        {
            usings.AddRange(_options.ExtraUsings);
        }

        // Generate Structures
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Structures.cs"),
            false,
            _options.Namespace,
            [.. usings],
            "#pragma warning disable CS0649"
            );

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
                || cppClass.Name == "VkBaseInStructure"
                || cppClass.Name == "VkBaseOutStructure"
                || cppClass.Name == "VkTransformMatrixKHR"
                || cppClass.Name == "VkAccelerationStructureInstanceKHR"
                || cppClass.Name == "VkAccelerationStructureSRTMotionInstanceNV"
                || cppClass.Name == "VkAccelerationStructureMatrixMotionInstanceNV"
                )
            {
                continue;
            }

            WriteStruct(writer, cppClass, cppClass.Name);

            writer.WriteLine();
        }
    }

    private static void WriteStruct(CodeWriter writer, CppClass cppClass, string structName)
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";
        bool isUnion = cppClass.ClassKind == CppClassKind.Union;
        bool hasSType = false;
        bool hasPNext = false;
        if (cppClass.Fields.FirstOrDefault(item => item.Name == "sType") != null)
        {
            hasSType = true;
        }

        if (cppClass.Fields.FirstOrDefault(item => item.Name == "pNext") != null)
        {
            hasPNext = true;
        }

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
        bool needUnsafe = NeedUnsafe(cppClass);
        if (needUnsafe)
        {
            modifier = "unsafe partial";
        }
        if (structName == "VkClearDepthStencilValue")
        {
            modifier = "readonly partial";
            isReadOnly = true;
        }

        string interfaceSubclass = string.Empty;
        if (hasSType && hasPNext)
            interfaceSubclass = " : IStructureType, IChainType";
        else if (hasSType)
            interfaceSubclass = " : IStructureType";
        else if (hasPNext)
            interfaceSubclass = " : IChainType";

        using (writer.PushBlock($"{visibility} {modifier} struct {structName}{interfaceSubclass}"))
        {
            if (_options.GenerateSizeOfStructs && cppClass.SizeOf > 0)
            {
                writer.WriteLine("/// <summary>");
                writer.WriteLine($"/// The size of the <see cref=\"{structName}\"/> type, in bytes.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine($"public static readonly int SizeInBytes = {cppClass.SizeOf};");
                writer.WriteLine();
            }

            foreach (CppField cppField in cppClass.Fields)
            {
                WriteField(writer, cppField, hasSType, isUnion, isReadOnly);
            }

            if (hasSType)
            {
                string structureTypeValue = structName;
                if (structureTypeValue.StartsWith("Vk"))
                {
                    structureTypeValue = structureTypeValue.Substring(2);
                }
                if (structureTypeValue.EndsWith("ANDROID"))
                {
                    structureTypeValue = structureTypeValue.Replace("ANDROID", "Android");
                }
                if (structureTypeValue == "ImportMemoryFdInfoKHR")
                {
                    structureTypeValue = "ImportMemoryFDInfoKHR";
                }
                else if (structureTypeValue == "MemoryFdPropertiesKHR")
                {
                    structureTypeValue = "MemoryFDPropertiesKHR";
                }
                else if (structureTypeValue == "MemoryGetFdInfoKHR")
                {
                    structureTypeValue = "MemoryGetFDInfoKHR";
                }
                else if (structureTypeValue == "ImportSemaphoreFdInfoKHR")
                {
                    structureTypeValue = "ImportSemaphoreFDInfoKHR";
                }
                else if (structureTypeValue == "SemaphoreGetFdInfoKHR")
                {
                    structureTypeValue = "SemaphoreGetFDInfoKHR";
                }
                else if (structureTypeValue == "ImportFenceFdInfoKHR")
                {
                    structureTypeValue = "ImportFenceFDInfoKHR";
                }
                else if (structureTypeValue == "FenceGetFdInfoKHR")
                {
                    structureTypeValue = "FenceGetFDInfoKHR";
                }

                writer.WriteLine();
                using (writer.PushBlock($"public {structName}()"))
                {
                    writer.WriteLine($"sType = VkStructureType.{structureTypeValue};");
                }

                writer.WriteLine();
                writer.WriteLine("/// <inheritdoc />");
                writer.WriteLine("VkStructureType IStructureType.sType => sType;");
            }

            if (hasPNext)
            {
                writer.WriteLine();
                writer.WriteLine("/// <inheritdoc />");
                using (writer.PushBlock("void* IChainType.pNext"))
                {
                    writer.WriteLine("get => pNext;");
                    writer.WriteLine("set => pNext = value;");
                }
            }
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
                string csFieldType = GetCsTypeName(arrayType.ElementType);
                writer.WriteLine($"public fixed {csFieldType} {csFieldName}[{arrayType.Size}];");
            }
            else
            {
                string csFieldType;
                if (arrayType.ElementType is CppArrayType elementArrayType)
                {
                    // vk-video madness
                    csFieldType = GetCsTypeName(elementArrayType.ElementType);
                    writer.WriteLine($"public fixed {csFieldType} {csFieldName}[{arrayType.Size} * {elementArrayType.Size}];");
                }
                else
                {
                    csFieldType = GetCsTypeName(arrayType.ElementType);

                    writer.WriteLine($"public {csFieldName}__FixedBuffer {csFieldName};");
                    writer.WriteLine();

                    writer.WriteLineUndindented("#if NET8_0_OR_GREATER");
                    writer.WriteLine($"[InlineArray({arrayType.Size})]");
                    using (writer.PushBlock($"public partial struct {csFieldName}__FixedBuffer"))
                    {
                        writer.WriteLine($"public {csFieldType} e0;");
                    }
                    writer.WriteLineUndindented("#else");
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
                                    writer.WriteLine($"return ref Unsafe.Add(ref e0, index);");
                                }
                            }
                        }
                        writer.WriteLine();

                        if (!csFieldType.EndsWith('*'))
                        {
                            writer.WriteLine("[UnscopedRef]");
                            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            writer.WriteLine($"public Span<{csFieldType}> AsSpan() => MemoryMarshal.CreateSpan(ref e0, {arrayType.Size});");
                        }
                    }
                    writer.WriteLineUndindented("#endif");
                }
            }
        }
        else if (field.Type is CppClass cppClass && (cppClass.IsAnonymous || cppClass.FullName.StartsWith($"{field.FullParentName}::")))
        {
            if (cppClass.IsAnonymous)
            {
                string fullParentName = field.FullParentName;
                if (fullParentName.EndsWith("::"))
                {
                    fullParentName = fullParentName.Substring(0, fullParentName.Length - 2);
                }
                string csFieldType = $"{fullParentName}_{csFieldName}";
                writer.WriteLine($"public {csFieldType} {csFieldName};");
                writer.WriteLine("");

                WriteStruct(writer, cppClass, csFieldType);
            }
            else
            {
                string csFieldType = cppClass.Name;
                writer.WriteLine($"public {csFieldType} {csFieldName};");
                writer.WriteLine("");

                WriteStruct(writer, cppClass, csFieldType);
            }
        }
        else if (field.Type is CppPointerType cppPointer
            && cppPointer.ElementType is CppClass cppPointerClass
            && (cppPointerClass.IsAnonymous || cppPointerClass.FullName.StartsWith($"{field.FullParentName}::")))
        {
            if (cppPointerClass.IsAnonymous)
            {
                string fullParentName = field.FullParentName;
                if (fullParentName.EndsWith("::"))
                {
                    fullParentName = fullParentName.Substring(0, fullParentName.Length - 2);
                }
                string csFieldType = $"{fullParentName}_{csFieldName}";
                writer.WriteLine($"public {csFieldType} {csFieldName};");
                writer.WriteLine("");

                WriteStruct(writer, cppPointerClass, csFieldType);
            }
            else
            {
                string csFieldType = cppPointerClass.Name;
                writer.WriteLine($"public {csFieldType}* {csFieldName};");
                writer.WriteLine("");

                WriteStruct(writer, cppPointerClass, csFieldType);
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
                    string paramCsType = GetCsTypeName(parameter.Type);
                    // Otherwise we get interop issues with non blittable types
                    if (paramCsType == "VkBool32")
                        paramCsType = "uint";
                    builder.Append(paramCsType).Append(", ");
                }

                string returnCsName = GetCsTypeName(functionType.ReturnType);
                // Otherwise we get interop issues with non blittable types
                if (returnCsName == "VkBool32")
                    returnCsName = "uint";

                builder.Append(returnCsName);

                writer.WriteLine($"public delegate* unmanaged<{builder}> {csFieldName};");
                return;
            }

            string csFieldType = GetCsTypeName(field.Type);
            if (csFieldName.Equals("specVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("applicationVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("engineVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("apiVersion", StringComparison.OrdinalIgnoreCase) ||
                csFieldName.Equals("vulkanApiVersion", StringComparison.OrdinalIgnoreCase))
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

            string modifier = "public";
            if (handleSType && csFieldName == "sType")
            {
                modifier = "internal";
            }

            writer.WriteLine($"{modifier} {fieldPrefix}{csFieldType} {csFieldName};");
        }
    }

    private static bool NeedUnsafe(CppClass @class)
    {
        if (@class.Name == "StdVideoAV1LoopRestoration")
            return true;

        foreach (CppField field in @class.Fields)
        {
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
                    return true;
                }
                else
                {
                    if (arrayType.ElementType is CppArrayType elementArrayType)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
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
                    return true;
                }

                string csFieldName = NormalizeFieldName(field.Name);
                string csFieldType = GetCsTypeName(field.Type);
                if (csFieldName.Equals("specVersion", StringComparison.OrdinalIgnoreCase) ||
                    csFieldName.Equals("applicationVersion", StringComparison.OrdinalIgnoreCase) ||
                    csFieldName.Equals("engineVersion", StringComparison.OrdinalIgnoreCase) ||
                    csFieldName.Equals("apiVersion", StringComparison.OrdinalIgnoreCase) ||
                    csFieldName.Equals("vulkanApiVersion", StringComparison.OrdinalIgnoreCase))
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

                if (csFieldType.EndsWith('*'))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
