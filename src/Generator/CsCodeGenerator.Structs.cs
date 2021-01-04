// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
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
                "System.Runtime.InteropServices",
                "Vortice.Mathematics"
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

                // Mapped to Vortice.Mathematics
                if (cppClass.Name == "VkOffset2D"
                    || cppClass.Name == "VkOffset3D"
                    || cppClass.Name == "VkExtent2D"
                    || cppClass.Name == "VkExtent3D"
                    || cppClass.Name == "VkRect2D"
                    || cppClass.Name == "VkViewport"
                    || cppClass.Name == "VkClearColorValue"
                    || cppClass.Name == "VkTransformMatrixKHR"
                    || cppClass.Name == "VkAccelerationStructureInstanceKHR"
                    )
                {
                    continue;
                }

                bool isUnion = cppClass.ClassKind == CppClassKind.Union;
                Console.WriteLine($"Generating struct {cppClass.Name}");

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
                        string csFieldName = NormalizeFieldName(cppField.Name);

                        if (isUnion)
                        {
                            writer.WriteLine("[FieldOffset(0)]");
                        }

                        if (cppField.Type is CppArrayType arrayType)
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
                                var csFieldType = GetCsTypeName(arrayType.ElementType, false);
                                writer.WriteLine($"public unsafe fixed {csFieldType} {csFieldName}[{arrayType.Size}];");
                            }
                            else
                            {
                                var unsafePrefix = string.Empty;
                                var csFieldType = GetCsTypeName(arrayType.ElementType, false);
                                if (csFieldType.EndsWith('*'))
                                {
                                    unsafePrefix = "unsafe ";
                                }

                                for (var i = 0; i < arrayType.Size; i++)
                                {
                                    writer.WriteLine($"public {unsafePrefix}{csFieldType} {csFieldName}_{i};");
                                }
                            }
                        }
                        else
                        {
                            string csFieldType = GetCsTypeName(cppField.Type, false);
                            if (csFieldName.Equals("specVersion", StringComparison.OrdinalIgnoreCase) ||
                                csFieldName.Equals("applicationVersion", StringComparison.OrdinalIgnoreCase) ||
                                csFieldName.Equals("engineVersion", StringComparison.OrdinalIgnoreCase) ||
                                csFieldName.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
                            {
                                csFieldType = "VkVersion";
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

                writer.WriteLine();
            }
        }
    }
}
