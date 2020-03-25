// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

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
                if (cppClass.ClassKind != CppClassKind.Struct)
                    continue;

                if (cppClass.SizeOf == 0)
                    continue;

                // Handles
                if (cppClass.Name.EndsWith("_T"))
                    continue;

                var csName = cppClass.Name;
                using (writer.PushBlock($"public unsafe partial struct {csName}"))
                {
                    if (cppClass.SizeOf > 0)
                    {
                        writer.WriteLine("/// <summary>");
                        writer.WriteLine($"/// The size of the <see cref=\"{csName}\"/> type, in bytes.");
                        writer.WriteLine("/// </summary>");
                        writer.WriteLine($"public static readonly int SizeInBytes = {cppClass.SizeOf};");
                        writer.WriteLine();
                    }

                    if (cppClass.Name == "VkAndroidSurfaceCreateInfoKHR")
                    {

                    }

                    foreach (var cppField in cppClass.Fields)
                    {
                        var csFieldName = NormalizeFieldName(cppField.Name);

                        if (cppField.Type is CppArrayType arrayType)
                        {
                            var canUseFixed = false;
                            if (arrayType.ElementType is CppTypedef typedef
                                && typedef.ElementType is CppPrimitiveType primitiveType)
                            {
                                canUseFixed = true;
                            }

                            if (canUseFixed)
                            {
                                var csFieldType = GetCsTypeName(arrayType.ElementType, false);
                                writer.WriteLine($"public fixed {csFieldType} {csFieldName}[{arrayType.Size}];");
                            }
                            else
                            {
                                var csFieldType = GetCsTypeName(arrayType.ElementType, false);
                                for (var i = 0; i < arrayType.Size; i++)
                                {
                                    writer.WriteLine($"public {csFieldType} {csFieldName}_{i};");
                                }
                            }
                        }
                        else
                        {
                            var csFieldType = GetCsTypeName(cppField.Type, false);
                            writer.WriteLine($"public {csFieldType} {csFieldName};");
                        }
                    }
                }

                writer.WriteLine();
            }
        }
    }
}
