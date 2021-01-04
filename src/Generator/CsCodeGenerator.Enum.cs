// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static readonly Dictionary<string, string> s_knownEnumValueNames = new Dictionary<string, string>
        {
            {  "VK_STENCIL_FRONT_AND_BACK", "FrontAndBack" },
            {  "VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_FLAGS_INFO", "MemoryAllocateFlagsInfo" },

            // VkSampleCountFlagBits
            {  "VK_SAMPLE_COUNT_1_BIT", "Count1" },
            {  "VK_SAMPLE_COUNT_2_BIT", "Count2" },
            {  "VK_SAMPLE_COUNT_4_BIT", "Count4" },
            {  "VK_SAMPLE_COUNT_8_BIT", "Count8" },
            {  "VK_SAMPLE_COUNT_16_BIT", "Count16" },
            {  "VK_SAMPLE_COUNT_32_BIT", "Count32" },
            {  "VK_SAMPLE_COUNT_64_BIT", "Count64" },

            // VkImageType
            { "VK_IMAGE_TYPE_1D", "Image1D" },
            { "VK_IMAGE_TYPE_2D", "Image2D" },
            { "VK_IMAGE_TYPE_3D", "Image3D" },

            // VkImageViewType
            { "VK_IMAGE_VIEW_TYPE_1D", "Image1D" },
            { "VK_IMAGE_VIEW_TYPE_2D", "Image2D" },
            { "VK_IMAGE_VIEW_TYPE_3D", "Image3D" },
            { "VK_IMAGE_VIEW_TYPE_CUBE", "ImageCube" },
            { "VK_IMAGE_VIEW_TYPE_1D_ARRAY", "Image1DArray" },
            { "VK_IMAGE_VIEW_TYPE_2D_ARRAY", "Image2DArray" },
            { "VK_IMAGE_VIEW_TYPE_CUBE_ARRAY", "ImageCubeArray" },

            // VkColorSpaceKHR
            { "VK_COLOR_SPACE_SRGB_NONLINEAR_KHR", "SrgbNonLinearKHR" },
            { "VK_COLOR_SPACE_DISPLAY_P3_NONLINEAR_EXT", "DisplayP3NonLinearEXT" },
            { "VK_COLOR_SPACE_DCI_P3_NONLINEAR_EXT", "DciP3NonLinearEXT" },
            { "VK_COLOR_SPACE_BT709_NONLINEAR_EXT", "Bt709NonLinearEXT" },
            { "VK_COLOR_SPACE_DOLBYVISION_EXT", "DolbyVisionEXT" },
            { "VK_COLOR_SPACE_ADOBERGB_LINEAR_EXT", "AdobeRgbLinearEXT" },
            { "VK_COLOR_SPACE_ADOBERGB_NONLINEAR_EXT", "AdobeRgbNonLinearEXT" },
            { "VK_COLOR_SPACE_EXTENDED_SRGB_NONLINEAR_EXT", "ExtendedSrgbNonLinearEXT" },
            { "VK_COLORSPACE_SRGB_NONLINEAR_KHR", "SrgbNonLinearKHR" },

            // VkShadingRatePaletteEntryNV
            {  "VK_SHADING_RATE_PALETTE_ENTRY_16_INVOCATIONS_PER_PIXEL_NV", "SixteenInvocationsPerPixel" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_8_INVOCATIONS_PER_PIXEL_NV", "EightInvocationsPerPixel" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_4_INVOCATIONS_PER_PIXEL_NV", "FourInvocationsPerPixel" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_2_INVOCATIONS_PER_PIXEL_NV", "TwoInvocationsPerPixel" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_PIXEL_NV", "OneInvocationPerPixel" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X1_PIXELS_NV", "OneInvocationPer2x1Pixels" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_1X2_PIXELS_NV", "OneInvocationPer1x2Pixels" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X2_PIXELS_NV", "OneInvocationPer2x2Pixels" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_4X2_PIXELS_NV", "OneInvocationPer4x2Pixels" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X4_PIXELS_NV", "OneInvocationPer2x4Pixels" },
            {  "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_4X4_PIXELS_NV", "OneInvocationPer4x4Pixels" },

            // VkDriverId
            { "VK_DRIVER_ID_GOOGLE_SWIFTSHADER", "GoogleSwiftShader" },
            { "VK_DRIVER_ID_GOOGLE_SWIFTSHADER_KHR", "GoogleSwiftShaderKHR" },
            { "VK_DRIVER_ID_MESA_LLVMPIPE", "MesaLLVMPipe" },

            // VkGeometryTypeNV
            {  "VK_GEOMETRY_TYPE_TRIANGLES_NV", "Triangles" },
            {  "VK_GEOMETRY_TYPE_AABBS_NVX", "AABBs" },

            // VkCopyAccelerationStructureModeNV
            {  "VK_COPY_ACCELERATION_STRUCTURE_MODE_CLONE_NV", "Clone" },
            {  "VK_COPY_ACCELERATION_STRUCTURE_MODE_COMPACT_NV", "Compact" },

            // VkAccelerationStructureTypeNV
            {  "VK_ACCELERATION_STRUCTURE_TYPE_TOP_LEVEL_NV", "TopLevel" },
            {  "VK_ACCELERATION_STRUCTURE_TYPE_BOTTOM_LEVEL_NV", "BottomLevel" },

            // VkAccelerationStructureMemoryRequirementsTypeNV
            {  "VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_OBJECT_NV", "Object" },
            {  "VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_BUILD_SCRATCH_NV", "BuildScratch" },
            {  "VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_UPDATE_SCRATCH_NV", "UpdateScratch" },

            // VkRayTracingShaderGroupTypeNV
            { "VK_RAY_TRACING_SHADER_GROUP_TYPE_GENERAL_NV", "General" },
            { "VK_RAY_TRACING_SHADER_GROUP_TYPE_TRIANGLES_HIT_GROUP_NV", "TrianglesHitGroup" },
            { "VK_RAY_TRACING_SHADER_GROUP_TYPE_PROCEDURAL_HIT_GROUP_NV", "ProceduralHitGroup" },

            // VkPerformanceCounterScopeKHR
            { "VK_QUERY_SCOPE_COMMAND_BUFFER_KHR", "QueryScopeCommandBufferKHR" },
            { "VK_QUERY_SCOPE_RENDER_PASS_KHR", "QueryScopeRenderPassKHR" },
            { "VK_QUERY_SCOPE_COMMAND_KHR", "QueryScopeCommandKHR" },

            // VkPerformanceConfigurationTypeINTEL
            { "VK_PERFORMANCE_CONFIGURATION_TYPE_COMMAND_QUEUE_METRICS_DISCOVERY_ACTIVATED_INTEL", "CommandQueueMetricsDiscoveryActivatedIntel" },
        };
        private static readonly Dictionary<string, string> s_knownEnumPrefixes = new Dictionary<string, string>
        {
            { "VkResult", "VK" },
            { "VkViewportCoordinateSwizzleNV", "VK_VIEWPORT_COORDINATE_SWIZZLE" },
            { "VkCoverageModulationModeNV", "VK_COVERAGE_MODULATION_MODE" },
            { "VkShadingRatePaletteEntryNV", "VK_SHADING_RATE_PALETTE_ENTRY" },
            { "VkCoarseSampleOrderTypeNV", "VK_COARSE_SAMPLE_ORDER_TYPE" },
            { "VkCopyAccelerationStructureModeNVX", "VK_COPY_ACCELERATION_STRUCTURE_MODE" },
        };

        private static readonly HashSet<string> s_ignoredParts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "flags",
            "bit",
            //"nv",
        };


        private static readonly HashSet<string> s_preserveCaps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "khr",
            "khx",
            "ext",
            "nv",
            "nvx",
            "amd",
            "intel"
        };

        public static void GenerateEnums(CppCompilation compilation, string outputPath)
        {
            using var writer = new CodeWriter(Path.Combine(outputPath, "Enumerations.cs"), "System");
            var createdEnums = new Dictionary<string, string>();

            foreach (var cppEnum in compilation.Enums)
            {
                var isBitmask =
                    cppEnum.Name.EndsWith("FlagBits") ||
                    cppEnum.Name.EndsWith("FlagBitsEXT") ||
                    cppEnum.Name.EndsWith("FlagBitsKHR") ||
                    cppEnum.Name.EndsWith("FlagBitsNV") ||
                    cppEnum.Name.EndsWith("FlagBitsAMD") ||
                    cppEnum.Name.EndsWith("FlagBitsMVK") ||
                    cppEnum.Name.EndsWith("FlagBitsNN");
                if (isBitmask)
                {
                    writer.WriteLine("[Flags]");
                }

                string csName = GetCsCleanName(cppEnum.Name);
                string enumNamePrefix = GetEnumNamePrefix(cppEnum.Name);

                // Rename FlagBits in Flags.
                if (isBitmask)
                {
                    csName = csName.Replace("FlagBits", "Flags");
                    AddCsMapping(cppEnum.Name, csName);
                }

                // Remove extension suffix from enum item values
                string extensionPrefix = "";

                if (csName.EndsWith("EXT"))
                {
                    extensionPrefix = "EXT";
                }
                else if (csName.EndsWith("NV"))
                {
                    extensionPrefix = "NV";
                }
                else if (csName.EndsWith("KHR"))
                {
                    extensionPrefix = "KHR";
                }

                createdEnums.Add(csName, cppEnum.Name);
                using (writer.PushBlock($"public enum {csName}"))
                {
                    if (isBitmask &&
                        !cppEnum.Items.Any(item => GetPrettyEnumName(item.Name, enumNamePrefix) == "None"))
                    {
                        writer.WriteLine("None = 0,");
                    }

                    foreach (var enumItem in cppEnum.Items)
                    {
                        if (enumItem.Name.EndsWith("_BEGIN_RANGE") ||
                            enumItem.Name.EndsWith("_END_RANGE") ||
                            enumItem.Name.EndsWith("_RANGE_SIZE") ||
                            enumItem.Name.EndsWith("_BEGIN_RANGE_EXT") ||
                            enumItem.Name.EndsWith("_BEGIN_RANGE_KHR") ||
                            enumItem.Name.EndsWith("_BEGIN_RANGE_NV") ||
                            enumItem.Name.EndsWith("_BEGIN_RANGE_AMD") ||
                            enumItem.Name.EndsWith("_END_RANGE_EXT") ||
                            enumItem.Name.EndsWith("_END_RANGE_KHR") ||
                            enumItem.Name.EndsWith("_END_RANGE_NV") ||
                            enumItem.Name.EndsWith("_END_RANGE_AMD") ||
                            enumItem.Name.EndsWith("_RANGE_SIZE_EXT") ||
                            enumItem.Name.EndsWith("_RANGE_SIZE_KHR") ||
                            enumItem.Name.EndsWith("_RANGE_SIZE_NV") ||
                            enumItem.Name.EndsWith("_RANGE_SIZE_AMD") ||
                            enumItem.Name.EndsWith("_MAX_ENUM") ||
                            enumItem.Name.EndsWith("_MAX_ENUM_EXT") ||
                            enumItem.Name.EndsWith("_MAX_ENUM_KHR") ||
                            enumItem.Name.EndsWith("_MAX_ENUM_NV") ||
                            enumItem.Name.EndsWith("_MAX_ENUM_AMD") ||
                            enumItem.Name.EndsWith("_MAX_ENUM_INTEL") ||
                            enumItem.Name == "VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_2_EXT" ||
                            enumItem.Name == "VK_STENCIL_FRONT_AND_BACK" ||
                            enumItem.Name == "VK_PIPELINE_CREATE_DISPATCH_BASE")
                        {
                            continue;
                        }

                        var enumItemName = GetEnumItemName(cppEnum, enumItem.Name, enumNamePrefix);

                        if (!string.IsNullOrEmpty(extensionPrefix) && enumItemName.EndsWith(extensionPrefix))
                        {
                            enumItemName = enumItemName.Remove(enumItemName.Length - extensionPrefix.Length);
                        }

                        //writer.WriteLine("/// <summary>");
                        //writer.WriteLine($"/// {enumItem.Name}");
                        //writer.WriteLine("/// </summary>");
                        if (enumItem.ValueExpression is CppRawExpression rawExpression)
                        {
                            var enumValueName = GetEnumItemName(cppEnum, rawExpression.Text, enumNamePrefix);
                            if (enumItemName == "SurfaceCapabilities2EXT")
                            {
                                continue;
                            }

                            if (!string.IsNullOrEmpty(extensionPrefix) && enumValueName.EndsWith(extensionPrefix))
                            {
                                enumValueName = enumValueName.Remove(enumValueName.Length - extensionPrefix.Length);

                                if (enumItemName == enumValueName)
                                    continue;
                            }


                            writer.WriteLine($"{enumItemName} = {enumValueName},");
                        }
                        else
                        {
                            writer.WriteLine($"{enumItemName} = {enumItem.Value},");
                        }
                    }

                    if (csName == "VkColorComponentFlags")
                    {
                        writer.WriteLine($"All = R | G | B | A");
                    }
                }

                writer.WriteLine();
            }

            // Map missing flags with typedefs to VkFlags
            foreach (var typedef in compilation.Typedefs)
            {
                if (typedef.Name.StartsWith("PFN_")
                    || typedef.Name.Equals("VkBool32", StringComparison.OrdinalIgnoreCase)
                    || typedef.Name.Equals("VkFlags", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (typedef.ElementType is CppPointerType)
                {
                    continue;
                }

                if (createdEnums.ContainsKey(typedef.Name))
                {
                    continue;
                }

                if (typedef.Name.EndsWith("Flags", StringComparison.OrdinalIgnoreCase) ||
                    typedef.Name.EndsWith("FlagsKHR", StringComparison.OrdinalIgnoreCase) ||
                    typedef.Name.EndsWith("FlagsEXT", StringComparison.OrdinalIgnoreCase) ||
                    typedef.Name.EndsWith("FlagsNV", StringComparison.OrdinalIgnoreCase) ||
                    typedef.Name.EndsWith("FlagsAMD", StringComparison.OrdinalIgnoreCase) ||
                    typedef.Name.EndsWith("FlagsMVK", StringComparison.OrdinalIgnoreCase) ||
                    typedef.Name.EndsWith("FlagsNN", StringComparison.OrdinalIgnoreCase))
                {
                    writer.WriteLine("[Flags]");
                    using (writer.PushBlock($"public enum {typedef.Name}"))
                    {
                        writer.WriteLine("None = 0,");
                    }
                    writer.WriteLine();
                }
            }
        }

        private static string GetEnumItemName(CppEnum @enum, string cppEnumItemName, string enumNamePrefix)
        {
            string enumItemName;
            if (@enum.Name == "VkFormat")
            {
                enumItemName = cppEnumItemName.Substring(enumNamePrefix.Length + 1);
                var splits = enumItemName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length <= 1)
                {
                    enumItemName = char.ToUpperInvariant(enumItemName[0]) + enumItemName.Substring(1).ToLowerInvariant();
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (var part in splits)
                    {
                        if (part.Equals("UNORM", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("UNorm");
                        }
                        else if (part.Equals("SNORM", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("SNorm");
                        }
                        else if (part.Equals("UINT", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("UInt");
                        }
                        else if (part.Equals("SINT", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("SInt");
                        }
                        else if (part.Equals("PACK8", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("Pack8");
                        }
                        else if (part.Equals("PACK16", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("Pack16");
                        }
                        else if (part.Equals("PACK32", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("Pack32");
                        }
                        else if (part.Equals("USCALED", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("UScaled");
                        }
                        else if (part.Equals("SSCALED", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("SScaled");
                        }
                        else if (part.Equals("UFLOAT", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("UFloat");
                        }
                        else if (part.Equals("SFLOAT", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("SFloat");
                        }
                        else if (part.Equals("SRGB", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("SRgb");
                        }
                        else if (part.Equals("BLOCK", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("Block");
                        }
                        else if (part.Equals("IMG", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("Img");
                        }
                        else if (part.Equals("2PACK16", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("2Pack16");
                        }
                        else if (part.Equals("3PACK16", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("3Pack16");
                        }
                        else if (part.Equals("4PACK16", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("4Pack16");
                        }
                        else if (part.Equals("2PLANE", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("2Plane");
                        }
                        else if (part.Equals("3PLANE", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("3Plane");
                        }
                        else if (part.Equals("4PLANE", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("4Plane");
                        }
                        else
                        {
                            sb.Append(part);
                        }
                    }

                    enumItemName = sb.ToString();
                }
            }
            else
            {
                enumItemName = GetPrettyEnumName(cppEnumItemName, enumNamePrefix);
            }

            return enumItemName;
        }

        private static string NormalizeEnumValue(string value)
        {
            if (value == "(~0U)")
            {
                return "~0u";
            }

            if (value == "(~0ULL)")
            {
                return "~0ul";
            }

            if (value == "(~0U-1)")
            {
                return "~0u - 1";
            }

            if (value == "(~0U-2)")
            {
                return "~0u - 2";
            }

            if (value == "(~0U-3)")
            {
                return "~0u - 3";
            }

            return value.Replace("ULL", "UL");
        }

        public static string GetEnumNamePrefix(string typeName)
        {
            if (s_knownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return knownValue;
            }

            List<string> parts = new List<string>(4);
            int chunkStart = 0;
            for (int i = 0; i < typeName.Length; i++)
            {
                if (char.IsUpper(typeName[i]))
                {
                    if (chunkStart != i)
                    {
                        parts.Add(typeName.Substring(chunkStart, i - chunkStart));
                    }

                    chunkStart = i;
                    if (i == typeName.Length - 1)
                    {
                        parts.Add(typeName.Substring(i, 1));
                    }
                }
                else if (i == typeName.Length - 1)
                {
                    parts.Add(typeName.Substring(chunkStart, typeName.Length - chunkStart));
                }
            }

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] == "Flag" ||
                    parts[i] == "Flags" ||
                    (parts[i] == "K" && (i + 2) < parts.Count && parts[i + 1] == "H" && parts[i + 2] == "R") ||
                    (parts[i] == "A" && (i + 2) < parts.Count && parts[i + 1] == "M" && parts[i + 2] == "D") ||
                    (parts[i] == "E" && (i + 2) < parts.Count && parts[i + 1] == "X" && parts[i + 2] == "T") ||
                    (parts[i] == "Type" && (i + 2) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V") ||
                    (parts[i] == "Type" && (i + 3) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" && parts[i + 3] == "X") ||
                    (parts[i] == "Scope" && (i + 2) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V") ||
                    (parts[i] == "Mode" && (i + 2) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V") ||
                    (parts[i] == "Mode" && (i + 5) < parts.Count && parts[i + 1] == "I" && parts[i + 2] == "N" && parts[i + 3] == "T" && parts[i + 4] == "E" && parts[i + 5] == "L") ||
                    (parts[i] == "Type" && (i + 5) < parts.Count && parts[i + 1] == "I" && parts[i + 2] == "N" && parts[i + 3] == "T" && parts[i + 4] == "E" && parts[i + 5] == "L")
                    )
                {
                    parts = new List<string>(parts.Take(i));
                    break;
                }
            }

            return string.Join("_", parts.Select(s => s.ToUpper()));
        }

        private static string GetPrettyEnumName(string value, string enumPrefix)
        {
            if (s_knownEnumValueNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            if (value.IndexOf(enumPrefix) != 0)
            {
                return value;
            }

            string[] parts = value[enumPrefix.Length..].Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            foreach (string part in parts)
            {
                if (s_ignoredParts.Contains(part))
                {
                    continue;
                }

                if (s_preserveCaps.Contains(part))
                {
                    sb.Append(part);
                }
                else
                {
                    sb.Append(char.ToUpper(part[0]));
                    for (int i = 1; i < part.Length; i++)
                    {
                        sb.Append(char.ToLower(part[i]));
                    }
                }
            }

            string prettyName = sb.ToString();
            return (char.IsNumber(prettyName[0])) ? "_" + prettyName : prettyName;
        }

    }
}
