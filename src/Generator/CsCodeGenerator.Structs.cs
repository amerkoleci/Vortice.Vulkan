// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static bool generateSizeOfStructs = false;

        private static void GenerateStructAndUnions(CppCompilation compilation, string outputPath)
        {
            // Generate Structures
            using var writer = new CodeWriter(Path.Combine(outputPath, "Structures.cs"),
                "System",
                "System.Runtime.InteropServices"
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
                if (cppClass.Name == "VkAllocationCallbacks")
                {

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
                        WriteField(writer, cppField, isUnion, isReadOnly);
                    }
                }

                writer.WriteLine();
            }
        }

        private static void WriteField(CodeWriter writer, CppField field, bool isUnion = false, bool isReadOnly = false)
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
                    string unsafePrefix = string.Empty;
                    string csFieldType = GetCsTypeName(arrayType.ElementType, false);
                    if (csFieldType.EndsWith('*'))
                    {
                        unsafePrefix = "unsafe ";
                    }

                    for (int i = 0; i < arrayType.Size; i++)
                    {
                        writer.WriteLine($"public {unsafePrefix}{csFieldType} {csFieldName}_{i};");
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
                    foreach(CppParameter parameter in functionType.Parameters)
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

                    writer.WriteLine("#if NET5_0_OR_GREATER");
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

                string fieldPrefix = isReadOnly ? "readonly " : string.Empty;
                if (csFieldType.EndsWith('*'))
                {
                    fieldPrefix += "unsafe ";
                }

                writer.WriteLine($"public {fieldPrefix}{csFieldType} {csFieldName};");
            }
        }
    }
}
