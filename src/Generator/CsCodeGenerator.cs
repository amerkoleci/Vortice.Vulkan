// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

public partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_keywords =
    [
        "object",
        "event",
        "params",
    ];

    private static readonly char[] s_spvcSeparator = ['_'];

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
        { "VisualID", "ulong" },
        { "RROutput", "nint" },

        { "HINSTANCE", "nint" },
        { "HWND", "nint" },
        { "HANDLE", "nint" },
        { "SECURITY_ATTRIBUTES", "nint" },
        { "LPCWSTR", "nint" },
        { "HMONITOR", "nint" },
        { "_SECURITY_ATTRIBUTES", "nint" },

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

        { "VkComponentTypeNV", "VkComponentTypeKHR" },
        { "VkScopeNV", "VkScopeKHR" },
        { "VkLineRasterizationModeEXT", "VkLineRasterizationMode" },
        { "VkPipelineCreateFlags2KHR", "VkPipelineCreateFlags2" },
        { "VkMemoryDecompressionMethodFlagsNV", "VkMemoryDecompressionMethodFlagsEXT" },
        

        // Spirv - Spirv-Cross
        { "SpvId", "uint" },
        { "spvc_bool", "SpvcBool" },
        { "spvc_type_id", "uint" }, // SpvId
        { "spvc_variable_id", "uint" }, // SpvId
        { "spvc_constant_id", "uint" }, // SpvId
        { "spvc_msl_vertex_format", "spvc_msl_shader_variable_format" },
        { "spvc_hlsl_binding_flags", "spvc_hlsl_binding_flag_bits" },
    };


    private static readonly HashSet<string> s_ignoredMacros =
    [
        "VK_TRUE",
        "VK_FALSE",
        "VK_MAX_GLOBAL_PRIORITY_SIZE_KHR",
        "VK_MAX_GLOBAL_PRIORITY_SIZE_EXT",
    ];

    private readonly CsCodeGeneratorOptions _options = new();
    private readonly VulkanSpecification? _vulkanSpecification = default;

    public CsCodeGenerator(CsCodeGeneratorOptions options, VulkanSpecification? specification = default)
    {
        _options = options;
        _vulkanSpecification = specification;
    }

    public void Generate(CppCompilation compilation)
    {
        GenerateEnums(compilation);
        GenerateConstants(compilation);
        GenerateHandles(compilation);
        GenerateStructAndUnions(compilation);
        GenerateCommands(compilation);

        if (_options.IsVulkan)
            GenerateHelperCommands(compilation);

        if (_vulkanSpecification != null)
            GenerateFormatHelpers();
    }

    public static void AddCsMapping(string typeName, string csTypeName)
    {
        if (typeName == csTypeName)
            return;

        s_csNameMappings[typeName] = csTypeName;
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
                string[] parts = name["spvc_".Length..].Split(s_spvcSeparator, StringSplitOptions.RemoveEmptyEntries);
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
            if (typedef.Name == "PFN_vkVoidFunction")
                return "PFN_vkVoidFunction";

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

            case CppPrimitiveKind.UnsignedLong:
                return "ulong";

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
