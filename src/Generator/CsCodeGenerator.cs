// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_keywords = new()
    {
        "object",
        "event",
    };

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

        { "buffer_handle_t", "IntPtr" },
        { "AHardwareBuffer", "IntPtr" },
        { "ANativeWindow", "IntPtr" },

        { "MirConnection", "IntPtr" },
        { "MirSurface", "IntPtr" },

        { "wl_display", "IntPtr" },
        { "wl_surface", "IntPtr" },

        { "Display", "IntPtr" },
        { "Window", "IntPtr" },
        { "VisualID", "IntPtr" },
        { "RROutput", "IntPtr" },

        { "HINSTANCE", "IntPtr" },
        { "HWND", "IntPtr" },
        { "HANDLE", "IntPtr" },
        { "SECURITY_ATTRIBUTES", "IntPtr" },
        { "LPCWSTR", "IntPtr" },
        { "HMONITOR", "IntPtr" },

        { "xcb_connection_t", "IntPtr" },
        { "xcb_window_t", "IntPtr" },
        { "xcb_visualid_t", "IntPtr" },

        { "CAMetalLayer", "IntPtr" },
        { "GgpFrameToken", "IntPtr" },
        { "GgpStreamDescriptor", "IntPtr" },

        { "MTLDevice_id", "IntPtr" },
        { "MTLCommandQueue_id", "IntPtr" },
        { "MTLBuffer_id", "IntPtr" },
        { "MTLTexture_id", "IntPtr" },
        { "MTLSharedEvent_id", "IntPtr" },
        { "IOSurfaceRef", "IntPtr" },

        { "VkAccelerationStructureTypeNV", "VkAccelerationStructureTypeKHR" },
        { "VkAccelerationStructureMemoryRequirementsTypeNV", "VkAccelerationStructureMemoryRequirementsTypeKHR" },
        { "VkAccelerationStructureNV", "VkAccelerationStructureKHR" },

        { "VkPipelineStageFlagBits2KHR", "VkPipelineStageFlags2KHR" },
        { "VkAccessFlagBits2KHR", "VkAccessFlags2KHR" },
        { "VkFormatFeatureFlagBits2KHR", "VkFormatFeatureFlags2KHR" },
        { "VkComponentTypeNV", "VkComponentTypeKHR" },
        { "VkScopeNV", "VkScopeKHR" },

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
        {
            GenerateHelperCommands(compilation);
        }

        if (specification != null)
        {
            GenerateFormatHelpers(specification);
        }
    }

    public static void AddCsMapping(string typeName, string csTypeName)
    {
        s_csNameMappings[typeName] = csTypeName;
    }

    private static void GenerateConstants(CppCompilation compilation)
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Constants.cs"), false, _options.Namespace, Array.Empty<string>());

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
                    )
                {
                    continue;
                }

                string modifier = "const";
                string csDataType = _options.ReadOnlySpanForStrings ? "ReadOnlySpan<byte>" : "string";
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
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_10" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_10" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_11")
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
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H264_ENCODE_API_VERSION_0_9_10" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_10" ||
                    cppMacro.Name == "VK_STD_VULKAN_VIDEO_CODEC_H265_ENCODE_API_VERSION_0_9_11")
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
                    if (csDataType == "ReadOnlySpan<byte>")
                    {
                        modifier = "static";
                        macroValue += "u8";
                        assignValue = "=>";
                    }
                    else if (csDataType == "VkString")
                    {
                        modifier = "static";
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
                    if (first)
                    {
                        writer.WriteLine();
                        first = false;
                    }

                    string? fieldType = GetCsTypeName(cppField.Type, false);
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
            return "IntPtr";
        }

        if (allowPretty)
        {
            if (name.StartsWith("spvc_"))
            {
                string[] parts = name["spvc_".Length..].Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                string result = PrettifyName(parts);
                AddCsMapping(name, result);
                return result;
            }
        }

        return name;
    }

    private static string GetCsTypeName(CppType? type, bool isPointer = false)
    {
        if (type is CppPrimitiveType primitiveType)
        {
            return GetCsTypeName(primitiveType, isPointer);
        }

        if (type is CppQualifiedType qualifiedType)
        {
            return GetCsTypeName(qualifiedType.ElementType, isPointer);
        }

        if (type is CppEnum enumType)
        {
            string enumCsName = GetCsCleanName(enumType.Name, false);
            if (isPointer)
                return enumCsName + "*";

            return enumCsName;
        }

        if (type is CppTypedef typedef)
        {
            if (typedef.ElementType is CppClass classElementType)
            {
                return GetCsTypeName(classElementType, isPointer);
            }

            string typeDefCsName = GetCsCleanName(typedef.Name, false);
            if (isPointer)
                return typeDefCsName + "*";

            return typeDefCsName;
        }

        if (type is CppClass @class)
        {
            var className = GetCsCleanName(@class.Name, false);
            if (isPointer)
                return className + "*";

            return className;
        }

        if (type is CppPointerType pointerType)
        {
            return GetCsTypeName(pointerType);
        }

        if (type is CppArrayType arrayType)
        {
            return GetCsTypeName(arrayType.ElementType, true);
        }

        return string.Empty;
    }

    private static string GetCsTypeName(CppPrimitiveType primitiveType, bool isPointer)
    {
        switch (primitiveType.Kind)
        {
            case CppPrimitiveKind.Void:
                return isPointer ? "void*" : "void";

            case CppPrimitiveKind.Char:
                return isPointer ? "sbyte*" : "sbyte";

            case CppPrimitiveKind.WChar:
                return isPointer ? "ushort*" : "ushort";

            case CppPrimitiveKind.Short:
                return isPointer ? "short*" : "short";

            case CppPrimitiveKind.Int:
                return isPointer ? "int*" : "int";

            case CppPrimitiveKind.LongLong:
                return isPointer ? "long*" : "long";

            case CppPrimitiveKind.UnsignedChar:
                return isPointer ? "byte*" : "byte";

            case CppPrimitiveKind.UnsignedShort:
                return isPointer ? "ushort*" : "ushort";

            case CppPrimitiveKind.UnsignedInt:
                return isPointer ? "uint*" : "uint";
            case CppPrimitiveKind.UnsignedLongLong:
                return isPointer ? "ulong*" : "ulong";

            case CppPrimitiveKind.Float:
                return isPointer ? "float*" : "float";

            case CppPrimitiveKind.Double:
                return isPointer ? "double*" : "double";

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
                return GetCsTypeName(primitiveType, true);
            }
            else if (qualifiedType.ElementType is CppClass @classType)
            {
                return GetCsTypeName(@classType, true);
            }
            else if (qualifiedType.ElementType is CppPointerType subPointerType)
            {
                return GetCsTypeName(subPointerType, true) + "*";
            }
            else if (qualifiedType.ElementType is CppTypedef typedef)
            {
                return GetCsTypeName(typedef, true);
            }
            else if (qualifiedType.ElementType is CppEnum @enum)
            {
                return GetCsTypeName(@enum, true);
            }

            return GetCsTypeName(qualifiedType.ElementType, true);
        }

        return GetCsTypeName(pointerType.ElementType, true);
    }
}
