// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private void GenerateConstants(CppCompilation compilation)
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";

        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Constants.cs"), false,
            _options.Namespace,
            []
            );

        if (_options.IsVulkan)
        {
            writer.WriteLine("/// <summary>");
            writer.WriteLine("/// Provides Vulkan specific constants for special values, layer names and extension names.");
            writer.WriteLine("/// </summary>");
        }

        using (writer.PushBlock($"{visibility} static partial class {_options.ClassName}"))
        {
            foreach (CppMacro cppMacro in compilation.Macros)
            {
                string sourceFileName = Path.GetFileNameWithoutExtension(cppMacro.SourceFile);
                if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                    continue;

                // Skip spirv.h enums
                if ((cppMacro.Name.StartsWith("SPV_") || cppMacro.Name.StartsWith("Spv")) && _options.ClassName == "SPIRVReflectApi" && Path.GetFileNameWithoutExtension(cppMacro.SourceFile) == "spirv")
                    continue;

                if (string.IsNullOrEmpty(cppMacro.Value)
                    || cppMacro.Name.EndsWith("_H_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VKAPI_CALL", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VKAPI_PTR", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VULKAN_CORE_H_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_MAKE_VERSION", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_MAKE_API_VERSION", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_MAKE_VIDEO_STD_VERSION", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_ENABLE_BETA_EXTENSIONS", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_VERSION_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_API_VERSION_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_NULL_HANDLE", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_DEFINE_HANDLE", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_DEFINE_NON_DISPATCHABLE_HANDLE", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_USE_PLATFORM_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_KHR_MAINTENANCE1_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_KHR_MAINTENANCE2_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_KHR_MAINTENANCE3_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_NV_VIEWPORT_ARRAY2_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_GOOGLE_HLSL_FUNCTIONALITY1_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VK_USE_64_BIT_PTR_DEFINES", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.StartsWith("VMA_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("SPVC_MAKE_MSL_VERSION", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("SPV_REFLECT_DEPRECATED", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    continue;
                }

                if (s_ignoredMacros.Contains(cppMacro.Name))
                    continue;

                string modifier = "const";
                string csDataType = _options.ReadOnlyMemoryUtf8ForStrings ? "ReadOnlySpan<byte>" : "string";
                string assignValue = "=";

                string macroValue = NormalizeEnumValue(cppMacro.Value);
                if (macroValue.EndsWith("F", StringComparison.OrdinalIgnoreCase))
                {
                    csDataType = "float";
                }
                else if (macroValue.EndsWith("UL", StringComparison.OrdinalIgnoreCase))
                {
                    csDataType = "ulong";
                }
                else if (macroValue.EndsWith("U", StringComparison.OrdinalIgnoreCase))
                {
                    csDataType = "uint";
                }
                else if (uint.TryParse(macroValue, out _) || macroValue.StartsWith("0x"))
                {
                    csDataType = "uint";
                }

                if (cppMacro.Name == "VK_QUEUE_FAMILY_EXTERNAL"
                    || cppMacro.Name == "VK_QUEUE_FAMILY_FOREIGN_EXT")
                {
                    csDataType = "uint";
                }
                else if (cppMacro.Name == "VK_LUID_SIZE_KHR"
                    || cppMacro.Name == "VK_SHADER_UNUSED_NV"
                    || cppMacro.Name == "VK_QUEUE_FAMILY_EXTERNAL_KHR"
                    || cppMacro.Name == "VK_MAX_DRIVER_NAME_SIZE_KHR"
                    || cppMacro.Name == "VK_MAX_DRIVER_INFO_SIZE_KHR"
                    || cppMacro.Name == "VK_MAX_DEVICE_GROUP_SIZE_KHR"
                    || cppMacro.Name == "VK_MAX_GLOBAL_PRIORITY_SIZE"
                    )
                {
                    csDataType = "uint";
                    macroValue = cppMacro.Value;
                }
                else if (cppMacro.Name == "SPVC_TRUE" ||
                    cppMacro.Name == "SPVC_FALSE")
                {
                    macroValue = "new (1)";
                    if (cppMacro.Name == "SPVC_FALSE")
                        macroValue = "new (0)";
                    writer.WriteLine($"public static SpvcBool {cppMacro.Name} => {macroValue};");
                    continue;
                }

                //AddCsMapping(cppMacro.Name, csName);


                if (cppMacro.Name == "VK_HEADER_VERSION_COMPLETE" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_SPEC_VERSION" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_SPEC_VERSION" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_SPEC_VERSION" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_SPEC_VERSION" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_7" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_0_9_7" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_8" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_API_VERSION_0_9_8" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_9" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_0_9_9" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_API_VERSION_1_0_0" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_1_0_0" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_9" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_11" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_10" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_12"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_SPEC_VERSION"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_SPEC_VERSION"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_AV1_DECODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_AV1_DECODE_SPEC_VERSION"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_AV1_ENCODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_AV1_ENCODE_SPEC_VERSION"
                    )
                {
                    modifier = "static";
                    csDataType = "VkVersion";
                    assignValue = "=>";
                }

                writer.WriteLine($"/// <unmanaged>{cppMacro.Name}</unmanaged>");
                if (cppMacro.Name == "VK_HEADER_VERSION_COMPLETE")
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} {assignValue} new VkVersion({cppMacro.Tokens[2]}, {cppMacro.Tokens[4]}, {cppMacro.Tokens[6]}, VK_HEADER_VERSION);");
                }
                else if (
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_0_9_6" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_7" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_0_9_7" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_8" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_API_VERSION_0_9_8" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_9" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_0_9_9" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_DECODE_API_VERSION_1_0_0" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_DECODE_API_VERSION_1_0_0" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_9" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_11" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_10" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_12"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_AV1_DECODE_API_VERSION_1_0_0"
                    || cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_AV1_ENCODE_API_VERSION_1_0_0"
                    )
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} => new VkVersion({cppMacro.Tokens[2]}, {cppMacro.Tokens[4]}, {cppMacro.Tokens[6]});");
                }
                else if (cppMacro.Name.StartsWith("STD_VIDEO_"))
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} = {macroValue};");
                }
                else if (cppMacro.Name == "VK_MAX_GLOBAL_PRIORITY_SIZE_EXT")
                {
                    csDataType = "uint";
                    macroValue = "VK_MAX_GLOBAL_PRIORITY_SIZE_KHR";
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} = {macroValue};");
                }
                else
                {

                    if (csDataType == "ReadOnlySpan<byte>")
                    {
                        modifier = "static";
                        macroValue += "u8";
                        assignValue = "=>";
                    }

                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} {assignValue} {macroValue};");
                }
            }

            if (!_options.IsVulkan)
            {
                bool first = true;

                foreach (CppField cppField in compilation.Fields)
                {
                    string sourceFileName = Path.GetFileNameWithoutExtension(cppField.SourceFile);
                    if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                        continue;

                    // Skip spirv.h enums
                    if (cppField.Name.StartsWith("Spv") && _options.ClassName == "SPIRVReflectApi" && Path.GetFileNameWithoutExtension(cppField.SourceFile) == "spirv")
                        continue;

                    if (first)
                    {
                        writer.WriteLine();
                        first = false;
                    }

                    string? fieldType = GetCsTypeName(cppField.Type);
                    string fieldName = cppField.Name;
                    if (fieldName.StartsWith(_options.ClassName))
                    {
                        fieldName = fieldName[_options.ClassName.Length..];
                    }

                    string modifier = "const";
                    writer.WriteLine($"public {modifier} {fieldType} {fieldName} = {cppField.InitExpression};");
                }
            }
            else
            {
                writer.WriteLine();

                foreach (string enumConstant in s_enumConstants)
                {
                    writer.WriteLine($"public const {enumConstant};");
                }
            }
        }
    }
}
