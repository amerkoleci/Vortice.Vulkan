// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private static readonly Dictionary<string, string> s_knownEnumValueNames = new()
    {
        // VkStructureType
        { "VK_STRUCTURE_TYPE_MACOS_SURFACE_CREATE_INFO_MVK", "MacOSSurfaceCreateInfoMVK" },
        { "VK_STRUCTURE_TYPE_TEXTURE_LOD_GATHER_FORMAT_PROPERTIES_AMD", "TextureLODGatherFormatPropertiesAMD" },
        { "VK_STRUCTURE_TYPE_PRESENT_ID_KHR", "PresentIdKHR" },
        { "VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXTURE_COMPRESSION_ASTC_HDR_FEATURES", "PhysicalDeviceTextureCompressionASTCHDRFeatures" },
        { "VK_STRUCTURE_TYPE_ANDROID_SURFACE_CREATE_INFO_KHR", "AndroidSurfaceCreateInfoKHR" },
        { "VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RGBA10X6_FORMATS_FEATURES_EXT", "PhysicalDeviceRGBA10X6FormatsFeaturesEXT" },
        { "VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRESENT_ID_FEATURES_KHR", "PhysicalDevicePresentIdFeaturesKHR" },
        { "VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_8BIT_STORAGE_FEATURES", "PhysicalDevice8BitStorageFeatures" },
        { "VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_16BIT_STORAGE_FEATURES", "PhysicalDevice16BitStorageFeatures" },
        {  "VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_FLAGS_INFO", "MemoryAllocateFlagsInfo" },
        { "VK_STRUCTURE_TYPE_LATENCY_SUBMISSION_PRESENT_ID_NV", "LatencySubmissionPresentIdNV" },

        {  "VK_STENCIL_FRONT_AND_BACK", "FrontAndBack" },

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

        // VkFragmentShadingRateNV
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_PIXEL_NV", "OneInvocationPerPixel" },
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_1X2_PIXELS_NV", "OneInvocationPer1x2Pixels" },
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_2X1_PIXELS_NV", "OneInvocationPer2x1Pixels" },
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_2X2_PIXELS_NV", "OneInvocationPer2x2Pixels" },
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_2X4_PIXELS_NV", "OneInvocationPer2x4Pixels" },
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_4X2_PIXELS_NV", "OneInvocationPer4x2Pixels" },
        {  "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_4X4_PIXELS_NV", "OneInvocationPer4x4Pixels" },
        {  "VK_FRAGMENT_SHADING_RATE_2_INVOCATIONS_PER_PIXEL_NV", "TwoInvocationsPerPixel" },
        {  "VK_FRAGMENT_SHADING_RATE_4_INVOCATIONS_PER_PIXEL_NV", "FourInvocationsPerPixel" },
        {  "VK_FRAGMENT_SHADING_RATE_8_INVOCATIONS_PER_PIXEL_NV", "EightInvocationsPerPixel" },
        {  "VK_FRAGMENT_SHADING_RATE_16_INVOCATIONS_PER_PIXEL_NV", "SixteenInvocationsPerPixel" },
        {  "VK_FRAGMENT_SHADING_RATE_NO_INVOCATIONS_NV", "NoInvocations" },

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

        // VkPipelineCreateFlags
        { "VK_PIPELINE_RASTERIZATION_STATE_CREATE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR", "RasterizationStateCreateFragmentShadingRateAttachmentKHR" },
        { "VK_PIPELINE_RASTERIZATION_STATE_CREATE_FRAGMENT_DENSITY_MAP_ATTACHMENT_BIT_EXT", "RasterizationStateCreateFragmentDensityMapAttachmentKHR" },

        { "VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT", "Array2DCompatible" },
        { "VK_IMAGE_CREATE_2D_VIEW_COMPATIBLE_BIT_EXT", "View2DCompatibleEXT" },
        { "VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT_KHR", "Array2DCompatibleKHR" },
        { "VK_QUERY_RESULT_64_BIT", "Bit64" },
        { "VK_SHADER_FLOAT_CONTROLS_INDEPENDENCE_32_BIT_ONLY", "Bit32Only" },
        { "VK_SHADER_FLOAT_CONTROLS_INDEPENDENCE_32_BIT_ONLY_KHR", "Bit32OnlyKHR" },
        { "VK_OPACITY_MICROMAP_FORMAT_2_STATE_EXT", "State2" },
        { "VK_OPACITY_MICROMAP_FORMAT_4_STATE_EXT", "State4" },

        // VkOpticalFlowGridSizeFlagsNV
        { "VK_OPTICAL_FLOW_GRID_SIZE_1X1_BIT_NV", "Size1x1" },
        { "VK_OPTICAL_FLOW_GRID_SIZE_2X2_BIT_NV", "Size2x2" },
        { "VK_OPTICAL_FLOW_GRID_SIZE_4X4_BIT_NV", "Size4x4" },
        { "VK_OPTICAL_FLOW_GRID_SIZE_8X8_BIT_NV", "Size8x8" },

        // VkDirectDriverLoadingModeLUNARG
        { "VK_DIRECT_DRIVER_LOADING_MODE_EXCLUSIVE_LUNARG", "Exclusive" },
        { "VK_DIRECT_DRIVER_LOADING_MODE_INCLUSIVE_LUNARG", "Include" },

        // VkMemoryDecompressionMethodFlagBitsNV
        { "VK_MEMORY_DECOMPRESSION_METHOD_GDEFLATE_1_0_BIT_NV", "GDeflate_1_0" },

        // VkDisplacementMicromapFormatNV
        { "VK_DISPLACEMENT_MICROMAP_FORMAT_64_TRIANGLES_64_BYTES_NV", "_64Triangles64Bytes" },
        { "VK_DISPLACEMENT_MICROMAP_FORMAT_256_TRIANGLES_128_BYTES_NV", "_256Triangles128Bytes" },
        { "VK_DISPLACEMENT_MICROMAP_FORMAT_1024_TRIANGLES_128_BYTES_NV", "_1024Triangles128Bytes" },

        // Spvc
        { "SPVC_SUCCESS", "Success" },
    };

    private static readonly Dictionary<string, string> s_knownEnumPrefixes = new()
    {
        { "VkResult", "VK" },
        { "VkViewportCoordinateSwizzleNV", "VK_VIEWPORT_COORDINATE_SWIZZLE" },
        { "VkCoverageModulationModeNV", "VK_COVERAGE_MODULATION_MODE" },
        { "VkShadingRatePaletteEntryNV", "VK_SHADING_RATE_PALETTE_ENTRY" },
        { "VkCoarseSampleOrderTypeNV", "VK_COARSE_SAMPLE_ORDER_TYPE" },
        { "VkCopyAccelerationStructureModeNVX", "VK_COPY_ACCELERATION_STRUCTURE_MODE" },
        { "VkOpticalFlowPerformanceLevelNV", "VK_OPTICAL_FLOW_PERFORMANCE_LEVEL" },
        { "VkOpticalFlowSessionBindingPointNV", "VK_OPTICAL_FLOW_SESSION_BINDING_POINT" },
        { "StdVideoAV1Profile", "STD_VIDEO_AV1_PROFILE" },
        { "StdVideoAV1Level", "STD_VIDEO_AV1" },
        { "StdVideoAV1FrameType", "STD_VIDEO_AV1_FRAME_TYPE" },
        { "StdVideoAV1ReferenceName", "STD_VIDEO_AV1_REFERENCE_NAME" },
        { "StdVideoAV1InterpolationFilter", "STD_VIDEO_AV1_INTERPOLATION_FILTER" },
        { "StdVideoAV1TxMode", "STD_VIDEO_AV1_TX_MODE" },
        { "StdVideoAV1FrameRestorationType", "STD_VIDEO_AV1_FRAME_RESTORATION_TYPE" },
        { "StdVideoAV1ColorPrimaries", "STD_VIDEO_AV1_COLOR_PRIMARIES" },
        { "StdVideoAV1TransferCharacteristics", "STD_VIDEO_AV1_TRANSFER_CHARACTERISTICS" },
        { "StdVideoAV1MatrixCoefficients", "STD_VIDEO_AV1_MATRIX_COEFFICIENTS" },
        { "StdVideoAV1ChromaSamplePosition", "STD_VIDEO_AV1_CHROMA_SAMPLE_POSITION" },
        { "VkVideoEncodeAV1PredictionModeKHR", "VK_VIDEO_ENCODE_AV1_PREDICTION_MODE" },
        { "VkVideoEncodeAV1RateControlGroupKHR", "VK_VIDEO_ENCODE_AV1_RATE_CONTROL_GROUP" },
        { "VkVideoEncodeAV1CapabilityFlagBitsKHR", "VK_VIDEO_ENCODE_AV1_CAPABILITY" },
        { "VkVideoEncodeAV1StdFlagBitsKHR", "VK_VIDEO_ENCODE_AV1_STD" },
        { "VkVideoEncodeAV1SuperblockSizeFlagBitsKHR", "VK_VIDEO_ENCODE_AV1_SUPERBLOCK_SIZE" },
        { "VkVideoEncodeAV1RateControlFlagBitsKHR", "VK_VIDEO_ENCODE_AV1_RATE_CONTROL" },
        { "VkCooperativeVectorMatrixLayoutNV", "VK_COOPERATIVE_VECTOR_MATRIX_LAYOUT" },
        { "VkLatencyMarkerNV", "VK_LATENCY_MARKER" },
        { "VkBlockMatchWindowCompareModeQCOM", "VK_BLOCK_MATCH_WINDOW_COMPARE_MODE" },
        { "VkCubicFilterWeightsQCOM", "VK_CUBIC_FILTER_WEIGHTS" },
        { "VkLayeredDriverUnderlyingApiMSFT", "VK_LAYERED_DRIVER_UNDERLYING_API" },

        // spvc
        { "spvc_result", "SPVC_ERROR" },
        { "spvc_hlsl_binding_flag_bits", "SPVC_HLSL_BINDING_AUTO" },

        // Spirv-Reflect
        {  "SpvReflectResult", "SPV_REFLECT_RESULT" },
        {  "SpvReflectResourceType", "SPV_REFLECT_RESOURCE_FLAG" },
        {  "SpvReflectArrayDimType", "SPV_REFLECT_ARRAY_DIM" },
        {  "SpvReflectExecutionModeValue", "SPV_REFLECT_EXECUTION_MODE" },
    };

    private static readonly HashSet<string> s_ignoredParts = new(StringComparer.OrdinalIgnoreCase)
    {
        //"flags",
        "bit",
        //"nv",
    };

    private static readonly HashSet<string> s_preserveCaps = new(StringComparer.OrdinalIgnoreCase)
    {
        "khr",
        "khx",
        "ext",
        "nv",
        "nvx",
        "nvidia",
        "amd",
        "amdx",
        "intel",
        "arm",
        "mvk",
        "nn",
        //"android",
        "google",
        "fuchsia",
        "huawei",
        "valve",
        "qcom",
        "macos",
        "ios",
        //"id",
        "pci",
        "bit",
        "astc",
        "aabb",
        "sm",
        "rdma",
        "2d",
        "3d",
        "io",
        "sec",
        "lunarg",
        "kmt",
        "fd",
        "d3d11",
        "d3d12",

        "spirv",
        "glsl",
        "hlsl",
        "msl",
        "json",
        "msft",
        "img",
        "av1",
        "es",
        "mesa",
        "vp9",
    };

    private static readonly HashSet<string> s_toUpperCaps = new(StringComparer.OrdinalIgnoreCase)
    {
        "glsl",
        "hlsl",
        "msl",
        "mtl",
    };

    private readonly HashSet<string> s_enumConstants = [];

    public void GenerateEnums(CppCompilation compilation)
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Enumerations.cs"), false, _options.Namespace, new[] { "System" });
        Dictionary<string, string> createdEnums = [];

        foreach (CppEnum cppEnum in compilation.Enums)
        {
            if (cppEnum.IsAnonymous)
                continue;

            string sourceFileName = Path.GetFileNameWithoutExtension(cppEnum.SourceFile);
            if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                continue;

            // Skip spirv.h enums
            if (cppEnum.Name.StartsWith("Spv")
                && Path.GetFileNameWithoutExtension(cppEnum.SourceFile) == "spirv"
                && _options.ClassName != "Spv")
            {
                continue;
            }

            bool isBitmask = cppEnum.Name.EndsWith("FlagBits")
                || cppEnum.Name.EndsWith("FlagBits2")
                || cppEnum.Name.EndsWith("FlagBits3")
                || cppEnum.Name.EndsWith("FlagBitsEXT")
                || cppEnum.Name.EndsWith("FlagBitsKHR")
                || cppEnum.Name.EndsWith("FlagBitsNV")
                || cppEnum.Name.EndsWith("FlagBitsAMD")
                || cppEnum.Name.EndsWith("FlagBitsMVK")
                || cppEnum.Name.EndsWith("FlagBitsNN")
                || cppEnum.Name.EndsWith("FlagBitsARM")
                || cppEnum.Name.EndsWith("FlagBitsQCOM")
                || (cppEnum.Name.StartsWith("Spv") && cppEnum.Name.EndsWith("Mask_")) // spirv.h
                ;

            if (cppEnum.Name == "SpvReflectResourceType"
                || cppEnum.Name == "spvc_hlsl_binding_flag_bits"
                )
            {
                isBitmask = true;
            }

            // typedef enum SpvSourceLanguage_ { } SpvSourceLanguage; }
            string enumName = cppEnum.Name;

            if (!_options.IsVulkan)
            {
                if (cppEnum.Name != "spvc_msl_shader_variable_format")
                {
                    foreach (CppTypedef typedef in compilation.Typedefs)
                    {
                        if (typedef.ElementType is not CppEnum typeDefEnum)
                            continue;

                        if (typeDefEnum.Name == cppEnum.Name)
                        {
                            enumName = typedef.Name;
                            break;
                        }
                    }
                }
            }

            bool shouldGeneratePrettyPrefix = _options.IsVulkan || enumName.StartsWith("Vma") || enumName.StartsWith("spvc_") || enumName.StartsWith("SpvReflect");
            string enumCsName = GetCsCleanName(enumName, shouldGeneratePrettyPrefix);
            string enumNamePrefix = shouldGeneratePrettyPrefix ? GetEnumNamePrefix(enumName) : enumName;

            // Rename FlagBits in Flags.
            if (isBitmask)
            {
                enumCsName = enumCsName.Replace("FlagBits", "Flags");
                if (enumName != enumCsName)
                {
                    AddCsMapping(enumName, enumCsName);
                }
            }

            // Remove extension suffix from enum item values
            string extensionPrefix = "";

            if (enumCsName.EndsWith("EXT"))
            {
                extensionPrefix = "EXT";
            }
            else if (enumCsName.EndsWith("NV"))
            {
                extensionPrefix = "NV";
            }
            else if (enumCsName.EndsWith("KHR"))
            {
                extensionPrefix = "KHR";
            }
            else if (enumCsName.EndsWith("NN"))
            {
                extensionPrefix = "NN";
            }
            else if (enumCsName.EndsWith("GGP"))
            {
                extensionPrefix = "GGP";
            }
            else if (enumCsName.EndsWith("ANDROID"))
            {
                extensionPrefix = "ANDROID";
            }
            else if (enumCsName.EndsWith("ARM"))
            {
                extensionPrefix = "ARM";
            }
            else if (enumCsName.EndsWith("QCOM"))
            {
                extensionPrefix = "QCOM";
            }
            else if (enumCsName.EndsWith("MSFT"))
            {
                extensionPrefix = "MSFT";
            }

            createdEnums.Add(enumCsName, enumName);

            bool noneAdded = false;

            EnumDefinition? enumDefinition = default;
            if (_vulkanSpecification is not null)
            {
                enumDefinition = _vulkanSpecification.GetEnumDefinition(cppEnum.Name);
            }

            if (enumDefinition != null && !string.IsNullOrEmpty(enumDefinition.Comment))
            {
                writer.WriteLine("/// <summary>");
                writer.WriteLine($"/// {enumDefinition.Comment!}");
                writer.WriteLine("/// </summary>");
            }

            if (isBitmask)
            {
                writer.WriteLine("[Flags]");
            }

            string baseTypeDecl = string.Empty;
            if (enumCsName == "SpvReflectExecutionModeValue")
            {
                baseTypeDecl = " : uint";
            }

            using (writer.PushBlock($"{visibility} enum {enumCsName}{baseTypeDecl}"))
            {
                if (isBitmask &&
                    !cppEnum.Items.Any(item => GetPrettyEnumName(item.Name, enumNamePrefix) == "None"))
                {
                    writer.WriteLine("None = 0,");
                    noneAdded = true;
                }

                foreach (CppEnumItem enumItem in cppEnum.Items)
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
                        enumItem.Name.EndsWith("_RANGE_SIZE_AMD")
                        || enumItem.Name.EndsWith("_MAX_ENUM")
                        || enumItem.Name.EndsWith("_MAX_ENUM_EXT")
                        || enumItem.Name.EndsWith("_MAX_ENUM_KHR")
                        || enumItem.Name.EndsWith("_MAX_ENUM_NV")
                        || enumItem.Name.EndsWith("_MAX_ENUM_AMD")
                        || enumItem.Name.EndsWith("_MAX_ENUM_INTEL")
                        || enumItem.Name.EndsWith("_MAX_ENUM_LUNARG")
                        || enumItem.Name.EndsWith("_MAX_ENUM_ANDROID")
                        || enumItem.Name.EndsWith("_MAX_ENUM_QCOM")
                        || enumItem.Name.EndsWith("_MAX_ENUM_MSFT")
                        || enumItem.Name.EndsWith("_MAX_ENUM_MSFT")
                        //enumItem.Name == "VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_2_EXT" ||
                        || enumItem.Name == "VK_STENCIL_FRONT_AND_BACK"
                        || enumItem.Name == "VK_PIPELINE_CREATE_DISPATCH_BASE"
                        || enumItem.Name.EndsWith("_INT_MAX")
                        || enumItem.Name.EndsWith("_MAX_INT")
                        || enumItem.Name.StartsWith("SPVC_MSL_SHADER_INPUT_FORMAT_")
                        || enumItem.Name.StartsWith("SPVC_MSL_VERTEX_FORMAT_")
                        // Deprecated enums
                        || enumItem.Name == "VK_RESOLVE_MODE_EXTERNAL_FORMAT_DOWNSAMPLE_ANDROID"
                        || enumItem.Name == "VK_HOST_IMAGE_COPY_MEMCPY"
                        )
                    {
                        continue;
                    }

                    string enumItemName = GetEnumItemName(cppEnum.Name, enumItem.Name, enumNamePrefix);

                    if (!string.IsNullOrEmpty(extensionPrefix) && enumItemName.EndsWith(extensionPrefix))
                    {
                        enumItemName = enumItemName.Remove(enumItemName.Length - extensionPrefix.Length);
                    }

                    if (enumItemName == "None" && noneAdded)
                    {
                        continue;
                    }

                    EnumValue? enumItemValue = default;
                    if (enumDefinition != null)
                    {
                        enumItemValue = enumDefinition.Values.FirstOrDefault(item => item.Name == enumItem.Name);
                    }

                    if (enumItem.ValueExpression is CppRawExpression rawExpression)
                    {
                        string enumValueName = GetEnumItemName(cppEnum.Name, rawExpression.Text, enumNamePrefix);
                        if (enumItemName == "SurfaceCapabilities2EXT")
                        {
                            continue;
                        }

                        // Dont generate extensions enum values that maps to core values
                        // Example: VK_ERROR_OUT_OF_POOL_MEMORY_KHR => VK_ERROR_OUT_OF_POOL_MEMORY
                        if (_options.IsVulkan && IsExtensionsValue(enumItemName))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(extensionPrefix) && enumValueName.EndsWith(extensionPrefix))
                        {
                            enumValueName = enumValueName.Remove(enumValueName.Length - extensionPrefix.Length);

                            if (enumItemName == enumValueName)
                                continue;
                        }

                        if (enumItemValue != null && !string.IsNullOrEmpty(enumItemValue.Comment))
                        {
                            writer.WriteLine("/// <summary>");
                            writer.WriteLine($"/// {enumItemValue.Comment!}");
                            writer.WriteLine("/// </summary>");
                        }

                        if (enumValueName.StartsWith("VMA_ALLOCATION_CREATE_"))
                        {
                            enumValueName = $"VmaAllocationCreateFlags.{enumItemName}";
                        }

                        writer.WriteLine($"/// <unmanaged>{enumItem.Name}</unmanaged>");
                        writer.WriteLine($"{enumItemName} = {enumValueName},");
                        s_enumConstants.Add($"{enumCsName} {enumItem.Name} = {enumCsName}.{enumItemName}");
                    }
                    else
                    {
                        // Spv
                        if (enumItemName == "Max" && enumItem.Value == int.MaxValue)
                        {
                            continue;
                        }

                        if (enumItemValue != null && !string.IsNullOrEmpty(enumItemValue.Comment))
                        {
                            writer.WriteLine("/// <summary>");
                            writer.WriteLine($"/// {enumItemValue.Comment!}");
                            writer.WriteLine("/// </summary>");
                        }

                        writer.WriteLine($"/// <unmanaged>{enumItem.Name}</unmanaged>");
                        if (enumItem.ValueExpression is CppLiteralExpression literalExpression)
                        {
                            writer.WriteLine($"{enumItemName} = {literalExpression.Value},");
                        }
                        else if (enumItem.ValueExpression is CppBinaryExpression binaryExpression)
                        {
                            if (cppEnum.Name == "spvc_compiler_option")
                            {
                                writer.WriteLine($"{enumItemName} = {enumItem.Value},");
                            }
                            else
                            {
                                StringBuilder builder = new();
                                FormatCppBinaryExpression(binaryExpression, builder, enumNamePrefix);
                                writer.WriteLine($"{enumItemName} = {builder},");
                            }
                        }
                        else
                        {
                            writer.WriteLine($"{enumItemName} = {enumItem.Value},");
                        }
                        s_enumConstants.Add($"{enumCsName} {enumItem.Name} = {enumCsName}.{enumItemName}");
                    }
                }

                if (enumCsName == "VkColorComponentFlags")
                {
                    writer.WriteLine("All = R | G | B | A");
                }
            }

            writer.WriteLine();
        }

        // Map missing flags with typedefs to VkFlags
        if (_options.IsVulkan)
        {
            foreach (CppTypedef typedef in compilation.Typedefs)
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

                if (typedef.Name.EndsWith("Flags", StringComparison.OrdinalIgnoreCase)
                    || typedef.Name.EndsWith("FlagsKHR", StringComparison.OrdinalIgnoreCase)
                    || typedef.Name.EndsWith("FlagsEXT", StringComparison.OrdinalIgnoreCase)
                    || (typedef.Name.EndsWith("FlagsNV", StringComparison.OrdinalIgnoreCase) && typedef.Name != "VkMemoryDecompressionMethodFlagsNV")
                    || typedef.Name.EndsWith("FlagsAMD", StringComparison.OrdinalIgnoreCase)
                    || typedef.Name.EndsWith("FlagsMVK", StringComparison.OrdinalIgnoreCase)
                    || typedef.Name.EndsWith("FlagsNN", StringComparison.OrdinalIgnoreCase)
                    )
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

        // Defined with specs 1.2.170 => VK_KHR_synchronization2
        if (_options.IsVulkan)
        {
            string lastCreatedEnum = string.Empty;
            HashSet<string> lastCreatedEnumFields = [];

            foreach (CppField cppField in compilation.Fields)
            {
                string sourceFileName = Path.GetFileNameWithoutExtension(cppField.SourceFile);
                if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                    continue;

                string? fieldType = GetCsTypeName(cppField.Type);
                string createdEnumName;

                if (!createdEnums.TryGetValue(fieldType, out string? value))
                {
                    if (!string.IsNullOrEmpty(lastCreatedEnum))
                    {
                        writer.EndBlock();
                        writer.WriteLine();
                    }

                    createdEnums.Add(fieldType, fieldType);
                    lastCreatedEnum = fieldType;
                    lastCreatedEnumFields = [];

                    string baseType = "uint";
                    if (cppField.Type is CppQualifiedType qualifiedType)
                    {
                        if (qualifiedType.ElementType is CppTypedef typedef)
                        {
                            baseType = GetCsTypeName(typedef.ElementType);
                        }
                        else
                        {
                            baseType = GetCsTypeName(qualifiedType.ElementType);
                        }
                    }

                    if (fieldType.EndsWith("FlagBits2"))
                    {
                        fieldType = fieldType.Replace("FlagBits2", "Flags2");
                    }
                    else if (fieldType.EndsWith("FlagBits2KHR"))
                    {
                        fieldType = fieldType.Replace("FlagBits2KHR", "Flags2KHR");
                    }
                    else if (fieldType.EndsWith("FlagBitsARM"))
                    {
                        fieldType = fieldType.Replace("FlagBitsARM", "FlagsARM");
                    }
                    else if (fieldType.EndsWith("FlagBitsQCOM"))
                    {
                        fieldType = fieldType.Replace("FlagBitsQCOM", "FlagsQCOM");
                    }
                    else if (fieldType.EndsWith("FlagBitsNV"))
                    {
                        fieldType = fieldType.Replace("FlagBitsNV", "FlagsNV");
                    }
                    else if (fieldType.EndsWith("FlagBits3"))
                    {
                        fieldType = fieldType.Replace("FlagBits3", "Flags3");
                    }
                    else if (fieldType.EndsWith("FlagBits3KHR"))
                    {
                        fieldType = fieldType.Replace("FlagBits3KHR", "Flags3KHR");
                    }

                    writer.WriteLine("[Flags]");
                    writer.BeginBlock($"public enum {fieldType} : {baseType}");
                    createdEnumName = fieldType;
                }
                else
                {
                    createdEnumName = value;
                }

                string csFieldName = string.Empty;
                if (cppField.Name.StartsWith("VK_PIPELINE_STAGE_2"))
                {
                    csFieldName = GetPrettyEnumName(cppField.Name, "VK_PIPELINE_STAGE_2");
                }
                else if (cppField.Name.StartsWith("VK_ACCESS_2"))
                {
                    csFieldName = GetPrettyEnumName(cppField.Name, "VK_ACCESS_2");
                }
                else if (cppField.Name.StartsWith("VK_ACCESS_3"))
                {
                    csFieldName = GetPrettyEnumName(cppField.Name, "VK_ACCESS_3");
                }
                else if (cppField.Name.StartsWith("VK_FORMAT_FEATURE_2"))
                {
                    csFieldName = GetPrettyEnumName(cppField.Name, "VK_FORMAT_FEATURE_2");
                }
                else if (cppField.Name.StartsWith("VK_PIPELINE_CREATE_2"))
                {
                    csFieldName = GetPrettyEnumName(cppField.Name, "VK_PIPELINE_CREATE_2");
                }
                else if (cppField.Name.StartsWith("VK_BUFFER_USAGE_2"))
                {
                    csFieldName = GetPrettyEnumName(cppField.Name, "VK_BUFFER_USAGE_2");
                }
                else
                {
                    string enumNamePrefix = GetEnumNamePrefix(createdEnumName);
                    csFieldName = GetPrettyEnumName(cppField.Name, enumNamePrefix);
                }

                // Remove vendor suffix from enum value if enum already contains it
                if (csFieldName.EndsWith("KHR", StringComparison.Ordinal) &&
                    createdEnumName.EndsWith("KHR", StringComparison.Ordinal))
                {
                    csFieldName = csFieldName.Substring(0, csFieldName.Length - 3);
                }

                // Dont generate extensions enum values that maps to core values
                // Example: VK_PIPELINE_STAGE_2_NONE_KHR => VK_PIPELINE_STAGE_2_NONE
                if (IsExtensionsValue(csFieldName))
                {
                    string existingField = string.Empty;
                    if (csFieldName.EndsWith("KHR", StringComparison.Ordinal))
                    {
                        existingField = csFieldName.Substring(0, csFieldName.Length - 3);
                    }

                    if (!string.IsNullOrEmpty(existingField) &&
                        lastCreatedEnumFields.Contains(existingField))
                    {
                        continue;
                    }
                }

                writer.WriteLine($"/// <unmanaged>{cppField.Name}</unmanaged>");
                if (cppField.InitExpression is CppLiteralExpression literalExpression)
                {
                    string enumValue = literalExpression.Value;
                    if (enumValue.EndsWith("ULL"))
                    {
                        enumValue = enumValue.Replace("ULL", "UL");
                    }

                    writer.WriteLine($"{csFieldName} = {enumValue},");
                }
                else
                {
                    writer.WriteLine($"{csFieldName} = {cppField.InitValue},");
                }

                lastCreatedEnumFields.Add(csFieldName);
            }


            if (!string.IsNullOrEmpty(lastCreatedEnum))
            {
                writer.EndBlock();
            }
        }
    }

    private static bool IsExtensionsValue(string value)
    {
        return value.EndsWith("EXT")
            || value.EndsWith("KHR")
            || value.EndsWith("NV")
            || value.EndsWith("NN")
            || value.EndsWith("GGP")
            || value.EndsWith("ANDROID")
            || value.EndsWith("ARM")
            || value.EndsWith("QCOM")
            || value.EndsWith("MSFT")
            ;
    }

    private void FormatExpression(CppExpression expression, StringBuilder builder, string enumNamePrefix)
    {
        if (expression is CppRawExpression rawExpression)
        {
            builder.Append(GetPrettyEnumName(rawExpression.Text, enumNamePrefix));
        }
        else if (expression is CppLiteralExpression literalExpression)
        {
            builder.Append(literalExpression.Value);
        }
        else if (expression is CppBinaryExpression binaryExpression)
        {
            FormatCppBinaryExpression(binaryExpression, builder, enumNamePrefix);
        }
    }

    private void FormatCppBinaryExpression(CppBinaryExpression expression, StringBuilder builder, string enumNamePrefix)
    {
        if (expression.Arguments != null && expression.Arguments.Count > 0)
        {
            FormatExpression(expression.Arguments[0], builder, enumNamePrefix);
        }

        builder.Append(' ');
        builder.Append(expression.Operator);
        builder.Append(' ');

        if (expression.Arguments != null && expression.Arguments.Count > 1)
        {
            FormatExpression(expression.Arguments[1], builder, enumNamePrefix);
        }
    }

    private string GetEnumItemName(string enumName, string cppEnumItemName, string enumNamePrefix)
    {
        string enumItemName;
        if (enumName == "VkFormat")
        {
            enumItemName = cppEnumItemName.Substring(enumNamePrefix.Length + 1);
            string[] splits = enumItemName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length <= 1)
            {
                enumItemName = char.ToUpperInvariant(enumItemName[0]) + enumItemName.Substring(1).ToLowerInvariant();
            }
            else
            {
                StringBuilder sb = new();
                foreach (string part in splits)
                {
                    if (part.Equals("UNORM", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Unorm");
                    }
                    else if (part.Equals("SNORM", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Snorm");
                    }
                    else if (part.Equals("UINT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Uint");
                    }
                    else if (part.Equals("SINT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Sint");
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
                        sb.Append("Uscaled");
                    }
                    else if (part.Equals("SSCALED", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Sscaled");
                    }
                    else if (part.Equals("UFLOAT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Ufloat");
                    }
                    else if (part.Equals("SFLOAT", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Sfloat");
                    }
                    else if (part.Equals("SRGB", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Srgb");
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
                    else if (part.Equals("BC1", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc1");
                    }
                    else if (part.Equals("BC2", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc2");
                    }
                    else if (part.Equals("BC3", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc3");
                    }
                    else if (part.Equals("BC4", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc4");
                    }
                    else if (part.Equals("BC5", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc5");
                    }
                    else if (part.Equals("BC6H", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc6h");
                    }
                    else if (part.Equals("BC7", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Bc7");
                    }
                    else if (part.Equals("ETC2", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Etc2");
                    }
                    else if (part.Equals("EAC", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Eac");
                    }
                    else if (part.Equals("ASTC", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Astc");
                    }
                    else if (part.Equals("RGBA", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Rgba");
                    }
                    else if (part.Equals("RGB", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("Rgb");
                    }
                    else if (part.Equals("SFIXED5", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append("SFixed5");
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
        if (value == "(~0U)" || value == "(~(0u))")
        {
            return "~0u";
        }

        if (value == "(0)")
        {
            return "0";
        }

        if (value == "(~(1u))")
        {
            return "~1u";
        }
        if (value == "(~(2u))")
        {
            return "~2u";
        }
        if (value == "(~(3u))")
        {
            return "~3u";
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

        List<string> parts = new(4);
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

    private string GetPrettyEnumName(string value, string enumPrefix)
    {
        if (s_knownEnumValueNames.TryGetValue(value, out string? knownName))
        {
            return knownName;
        }

        if (!value.StartsWith(enumPrefix))
        {
            return value;
        }

        if (!_options.IsVulkan
            && !enumPrefix.StartsWith("VMA_")
            && !enumPrefix.StartsWith("SPV_")
            && !enumPrefix.StartsWith("SPVC_"))
        {
            string result = value[enumPrefix.Length..];
            if (char.IsNumber(result[0]))
            {
                if (enumPrefix.EndsWith("SpvDim"))
                {
                    return "Dim" + result;
                }
            }

            return result;
        }

        string[] parts = value[enumPrefix.Length..].Split(['_'], StringSplitOptions.RemoveEmptyEntries);
        return PrettifyName(parts, enumPrefix);
    }

    private static string PrettifyName(string[] parts, string? enumPrefix = default)
    {
        StringBuilder sb = new();
        foreach (string part in parts)
        {
            if (s_ignoredParts.Contains(part))
            {
                continue;
            }

            if (s_preserveCaps.Contains(part))
            {
                if (s_toUpperCaps.Contains(part))
                {
                    sb.Append(part.ToUpper());
                }
                else
                {
                    sb.Append(part);
                }
            }
            else if (part.Equals("OPENGL", StringComparison.InvariantCultureIgnoreCase))
            {
                return "OpenGL";
            }
            else if (part.Equals("OPENGLES", StringComparison.InvariantCultureIgnoreCase))
            {
                return "OpenGLES";
            }
            else if (part.Equals("MTLBUFFER", StringComparison.InvariantCultureIgnoreCase))
            {
                return "MTLBuffer";
            }
            else if (part.Equals("MTLTEXTURE", StringComparison.InvariantCultureIgnoreCase))
            {
                return "MTLTexture";
            }
            else if (part.Equals("MTLHEAP", StringComparison.InvariantCultureIgnoreCase))
            {
                return "MTLHeap";
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
        if (!string.IsNullOrEmpty(enumPrefix))
        {
            if (char.IsNumber(prettyName[0]))
            {
                if (enumPrefix.EndsWith("_IDC"))
                {
                    return "Idc" + prettyName;
                }

                if (enumPrefix.EndsWith("_POC_TYPE"))
                {
                    return "Type" + prettyName;
                }

                if (enumPrefix.EndsWith("_CTB_SIZE"))
                {
                    return "Size" + prettyName;
                }

                if (enumPrefix.EndsWith("_BLOCK_SIZE"))
                {
                    return "Size" + prettyName;
                }

                if (enumPrefix.EndsWith("_FIXED_RATE"))
                {
                    return "Rate" + prettyName;
                }

                if (enumPrefix.EndsWith("_SUBSAMPLING"))
                {
                    return "Subsampling" + prettyName;
                }

                if (enumPrefix.EndsWith("_BIT_DEPTH"))
                {
                    return "Depth" + prettyName;
                }

                return "_" + prettyName;
            }
        }

        return prettyName;
    }
}
