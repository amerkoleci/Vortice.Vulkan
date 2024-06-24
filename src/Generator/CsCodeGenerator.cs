// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_keywords =
    [
        "object",
        "event",
    ];

    private static readonly char[] _spvcSeparator = ['_'];

    private static readonly Dictionary<string, string> s_csNameMappings = new()
    {
        { "uint8_t", "byte" },
        { "uint16_t", "ushort" },
        { "uint32_t", "uint" },
        { "uint64_t", "ulong" },
        { "int8_t", "sbyte" },
        { "int32_t", "int" },
        { "int16_t", "short" },
        { "int64_t", "long" },
        { "int64_t*", "long*" },
        { "char", "byte" },
        { "size_t", "nuint" },
        { "intptr_t", "nint" },
        { "uintptr_t", "nuint" },
        { "DWORD", "uint" },
        

        //{ "VkBool32", "uint" },
        { "VkDeviceAddress", "ulong" },
        { "VkDeviceSize", "ulong" },
        { "VkFlags", "uint" },
        { "VkSampleMask", "uint" },
        { "VkFlags64", "ulong" },

        { "buffer_handle_t", "nint" },
        { "AHardwareBuffer", "nint" },
        { "ANativeWindow", "nint" },

        { "MirConnection", "nint" },
        { "MirSurface", "nint" },

        { "wl_display", "nint" },
        { "wl_surface", "nint" },

        { "Display", "nint" },
        { "Window", "ulong" },
        { "VisualID", "nint" },
        { "RROutput", "nint" },

        { "HINSTANCE", "nint" },
        { "HWND", "nint" },
        { "HANDLE", "nint" },
        { "SECURITY_ATTRIBUTES", "nint" },
        { "LPCWSTR", "nint" },
        { "HMONITOR", "nint" },

        { "xcb_connection_t", "nint" },
        { "xcb_window_t", "uint" },
        { "xcb_visualid_t", "nint" },

        { "CAMetalLayer", "nint" },
        { "GgpFrameToken", "nint" },
        { "GgpStreamDescriptor", "nint" },

        { "MTLDevice_id", "nint" },
        { "MTLCommandQueue_id", "nint" },
        { "MTLBuffer_id", "nint" },
        { "MTLTexture_id", "nint" },
        { "MTLSharedEvent_id", "nint" },
        { "IOSurfaceRef", "nint" },

        { "VkAccelerationStructureTypeNV", "VkAccelerationStructureTypeKHR" },
        { "VkAccelerationStructureMemoryRequirementsTypeNV", "VkAccelerationStructureMemoryRequirementsTypeKHR" },
        { "VkAccelerationStructureNV", "VkAccelerationStructureKHR" },

        { "VkPipelineStageFlagBits2KHR", "VkPipelineStageFlags2KHR" },
        { "VkAccessFlagBits2KHR", "VkAccessFlags2KHR" },
        { "VkFormatFeatureFlagBits2KHR", "VkFormatFeatureFlags2KHR" },
        { "VkComponentTypeNV", "VkComponentTypeKHR" },
        { "VkScopeNV", "VkScopeKHR" },
        { "VkLineRasterizationModeEXT", "VkLineRasterizationModeKHR" },

        // Spirv - Spirv-Cross
        { "SpvId", "uint" },
        { "spvc_bool", "SpvcBool" },
        { "spvc_type_id", "uint" }, // SpvId
        { "spvc_variable_id", "uint" }, // SpvId
        { "spvc_constant_id", "uint" }, // SpvId
        { "spvc_msl_vertex_format", "spvc_msl_shader_variable_format" },
    };

    private static CsCodeGeneratorOptions _options = new();
    private static VulkanSpecification? s_vulkanSpecification = default;

    public static void Generate(CppCompilation compilation, CsCodeGeneratorOptions options, VulkanSpecification? specification = default)
    {
        _options = options;
        s_vulkanSpecification = specification;

        GenerateEnums(compilation);
        GenerateConstants(compilation);
        GenerateHandles(compilation);
        GenerateStructAndUnions(compilation);
        GenerateCommands(compilation);

        if (options.IsVulkan)
            GenerateHelperCommands(compilation);

        if (specification != null)
            GenerateFormatHelpers(specification);
    }

    public static void AddCsMapping(string typeName, string csTypeName)
    {
        if (typeName == csTypeName)
            return;

        s_csNameMappings[typeName] = csTypeName;
    }

    private static void GenerateConstants(CppCompilation compilation)
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
                // Skip spirv.h enums
                if ((cppMacro.Name.StartsWith("SPV_") || cppMacro.Name.StartsWith("Spv")) && _options.ClassName == "SPIRVReflectApi" && Path.GetFileNameWithoutExtension(cppMacro.SourceFile) == "spirv")
                    continue;

                if (string.IsNullOrEmpty(cppMacro.Value)
                    || cppMacro.Name.EndsWith("_H_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VKAPI_CALL", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VKAPI_PTR", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VULKAN_CORE_H_", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_TRUE", StringComparison.OrdinalIgnoreCase)
                    || cppMacro.Name.Equals("VK_FALSE", StringComparison.OrdinalIgnoreCase)
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

                string modifier = "const";
                string csDataType = _options.ReadOnlyMemoryUtf8ForStrings ? "ReadOnlyMemoryUtf8" : "string";

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
                    )
                {
                    modifier = "static readonly";
                    csDataType = "VkVersion";
                }

                writer.WriteLine($"/// <unmanaged>{cppMacro.Name}</unmanaged>");
                if (cppMacro.Name == "VK_HEADER_VERSION_COMPLETE")
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} = new VkVersion({cppMacro.Tokens[2]}, {cppMacro.Tokens[4]}, {cppMacro.Tokens[6]}, VK_HEADER_VERSION);");
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
                    )
                {
                    writer.WriteLine($"public {modifier} {csDataType} {cppMacro.Name} = new VkVersion({cppMacro.Tokens[2]}, {cppMacro.Tokens[4]}, {cppMacro.Tokens[6]});");
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
                    string assignValue = "=";
                    if (csDataType == "ReadOnlyMemoryUtf8")
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

    private static string NormalizeFieldName(string name)
    {
        if (s_keywords.Contains(name))
            return "@" + name;

        return name;
    }

    private static string GetCsCleanName(string name, bool allowPretty)
    {
        if (s_csNameMappings.TryGetValue(name, out string? mappedName))
        {
            return GetCsCleanName(mappedName, allowPretty);
        }
        else if (name.StartsWith("PFN"))
        {
            return "nint";
        }

        if (allowPretty)
        {
            if (name.StartsWith("spvc_"))
            {
                string[] parts = name["spvc_".Length..].Split(_spvcSeparator, StringSplitOptions.RemoveEmptyEntries);
                string result = PrettifyName(parts);
                AddCsMapping(name, result);
                return result;
            }
        }

        return name;
    }

    private static string GetCsTypeName(CppType? type)
    {
        if (type is CppPrimitiveType primitiveType)
        {
            return GetCsTypeName(primitiveType);
        }

        if (type is CppQualifiedType qualifiedType)
        {
            return GetCsTypeName(qualifiedType.ElementType);
        }

        if (type is CppEnum enumType)
        {
            string enumCsName = GetCsCleanName(enumType.Name, false);
            return enumCsName;
        }

        if (type is CppTypedef typedef)
        {
            if (typedef.ElementType is CppClass classElementType)
            {
                return GetCsTypeName(classElementType);
            }

            string typeDefCsName = GetCsCleanName(typedef.Name, false);
            return typeDefCsName;
        }

        if (type is CppClass @class)
        {
            string className = GetCsCleanName(@class.Name, false);
            return className;
        }

        if (type is CppPointerType pointerType)
        {
            string csPointerTypeName = GetCsTypeName(pointerType);
            if (csPointerTypeName == "IntPtr" || csPointerTypeName == "nint" /*&& s_csNameMappings.ContainsKey(pointerType.)*/)
            {
                return csPointerTypeName;
            }

            return csPointerTypeName + "*";
        }

        if (type is CppArrayType arrayType)
        {
            return GetCsTypeName(arrayType.ElementType) + "*";
        }

        return string.Empty;
    }

    private static string GetCsTypeName(CppPrimitiveType primitiveType)
    {
        switch (primitiveType.Kind)
        {
            case CppPrimitiveKind.Void:
                return "void";

            case CppPrimitiveKind.Char:
                return "byte";

            case CppPrimitiveKind.WChar:
                return "char";

            case CppPrimitiveKind.Short:
                return "short";

            case CppPrimitiveKind.Int:
                return "int";

            case CppPrimitiveKind.LongLong:
                return "long";

            case CppPrimitiveKind.UnsignedChar:
                return "byte";

            case CppPrimitiveKind.UnsignedShort:
                return "ushort";

            case CppPrimitiveKind.UnsignedInt:
                return "uint";
            case CppPrimitiveKind.UnsignedLongLong:
                return "ulong";

            case CppPrimitiveKind.Float:
                return "float";

            case CppPrimitiveKind.Double:
                return "double";

            default:
                throw new InvalidOperationException($"Unknown primitive type: {primitiveType.Kind}");
        }
    }

    private static string GetCsTypeName(CppPointerType pointerType)
    {
        if (pointerType.ElementType is CppQualifiedType qualifiedType)
        {
            if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
            {
                return GetCsTypeName(primitiveType);
            }
            else if (qualifiedType.ElementType is CppClass @classType)
            {
                return GetCsTypeName(@classType);
            }
            else if (qualifiedType.ElementType is CppPointerType subPointerType)
            {
                return GetCsTypeName(subPointerType) + "*";
            }
            else if (qualifiedType.ElementType is CppTypedef typedef)
            {
                return GetCsTypeName(typedef);
            }
            else if (qualifiedType.ElementType is CppEnum @enum)
            {
                return GetCsTypeName(@enum);
            }

            return GetCsTypeName(qualifiedType.ElementType);
        }

        return GetCsTypeName(pointerType.ElementType);
    }
}
