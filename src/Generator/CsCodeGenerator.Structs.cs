// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static void GenerateStructAndUnions(CppCompilation compilation, string outputPath)
        {
            // Generate Structures
            using var writer = new CodeWriter(Path.Combine(outputPath, "Structures.cs"),
                "System",
                "System.Runtime.InteropServices",
                "Vortice.Mathematics"
                );

            // Print All classes, structs
            foreach (var cppClass in compilation.Classes)
            {
                if (cppClass.ClassKind == CppClassKind.Class ||
                    cppClass.SizeOf == 0 ||
                    cppClass.Name.EndsWith("_T"))
                {
                    continue;
                }

                var isUnion = cppClass.ClassKind == CppClassKind.Union;

                var csName = cppClass.Name;
                if (isUnion)
                {
                    writer.WriteLine("[StructLayout(LayoutKind.Explicit)]");
                }
                else
                {
                    writer.WriteLine("[StructLayout(LayoutKind.Sequential)]");
                }

                using (writer.PushBlock($"public partial struct {csName}"))
                {
                    if (cppClass.SizeOf > 0)
                    {
                        writer.WriteLine("/// <summary>");
                        writer.WriteLine($"/// The size of the <see cref=\"{csName}\"/> type, in bytes.");
                        writer.WriteLine("/// </summary>");
                        writer.WriteLine($"public static readonly int SizeInBytes = {cppClass.SizeOf};");
                        writer.WriteLine();
                    }

                    foreach (var cppField in cppClass.Fields)
                    {
                        var csFieldName = NormalizeFieldName(cppField.Name);

                        if (isUnion)
                        {
                            writer.WriteLine("[FieldOffset(0)]");
                        }

                        if (cppField.Type is CppArrayType arrayType)
                        {
                            var canUseFixed = false;
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
                            var csFieldType = GetCsTypeName(cppField.Type, false);
                            if (csFieldName.Equals("specVersion", StringComparison.OrdinalIgnoreCase) ||
                                csFieldName.Equals("applicationVersion", StringComparison.OrdinalIgnoreCase) ||
                                csFieldName.Equals("engineVersion", StringComparison.OrdinalIgnoreCase) ||
                                csFieldName.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
                            {
                                csFieldType = "VkVersion";
                            }

                            var unsafePrefix = string.Empty;
                            if (csFieldType.EndsWith('*'))
                            {
                                unsafePrefix = "unsafe ";
                            }

                            writer.WriteLine($"public {unsafePrefix}{csFieldType} {csFieldName};");
                        }
                    }
                }

                writer.WriteLine();
            }
        }
    }
}
