// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.SpirvCross.Constants;

namespace Vortice.SpirvCross;

public enum CompilerOption : uint
{
    Unknown = 0,
    ForceTemporary = 1 | OptionCommonBit,
    FlattenMultidimensionalArrays = 2 | OptionCommonBit,
    FixupDepthConvention = 3 | OptionCommonBit,
    FlipVertexY = 4 | OptionCommonBit,

    GLSL_SupportNonZeroBaseInstance = 5 | OptionGLSLBit,
    GLSL_SeparateShaderObjects = 6 | OptionGLSLBit,
    GLSL_Enable420PackExtension = 7 | OptionGLSLBit,
    GLSL_Version = 8 | OptionGLSLBit,
    GLSL_ES = 9 | OptionGLSLBit,
    GLSL_VulkanSemantics = 10 | OptionGLSLBit,
    GLSL_ES_DefaultFloatPrecisionHighPrecision = 11 | OptionGLSLBit,
    GLSL_ES_DefaultIntPrecisionHighPrecision = 12 | OptionGLSLBit,

    HLSL_ShaderModel = 13 | OptionHSLBit,
    HLSL_PointSizeCompat = 14 | OptionHSLBit,
    HLSL_PointCoordCompat = 15 | OptionHSLBit,
    HLSL_SupportNonZeroBaseVertexBaseInstance = 16 | OptionHSLBit,

    MSL_Version = 17 | OptionMSLBit,
    MSL_TexelBufferTextureWidth = 18 | OptionMSLBit,

    /// <summary>
    /// Obsolete, use SwizzleBufferIndex instead.
    /// </summary>
    MSL_AixBufferIndex = 19 | OptionMSLBit,
    MSL_SwizzleBufferIndex = 19 | OptionMSLBit,

    MSL_IndirectParamsBufferIndex = 20 | OptionMSLBit,
    MSL_ShaderOutputBufferIndex = 21 | OptionMSLBit,
    MSL_ShaderPatchOutputBufferIndex = 22 | OptionMSLBit,
    MSL_ShaderTessFactorOutputBufferIndex = 23 | OptionMSLBit,
    MSL_ShaderInputWorkgroupIndex = 24 | OptionMSLBit,
    MSL_EnablePointSizeBuiltin = 25 | OptionMSLBit,
    MSL_DisableRasterization = 26 | OptionMSLBit,
    MSL_CaptureOutputoBuffer = 27 | OptionMSLBit,
    MSL_SwizzleTextureSamples = 28 | OptionMSLBit,
    MSL_PAD_FRAGMENT_OutputComponents = 29 | OptionMSLBit,
    MSL_TESS_DOMAIN_ORIGIN_LOWER_LEFT = 30 | OptionMSLBit,
    MSL_Platform = 31 | OptionMSLBit,
    MSL_ArgumentBuffers = 32 | OptionMSLBit,

    GLSL_EmitPushConstantAsUniformBuffer = 33 | OptionGLSLBit,

    MSLTextureBufferNative = 34 | OptionMSLBit,

    GLSL_EMIT_UNIFORMBuffer_AS_PLAIN_UNIFORMS = 35 | OptionGLSLBit,

    MSLBufferSizeBufferIndex = 36 | OptionMSLBit,

    EmitLineDirectives = 37 | OptionCommonBit,

    MSL_MultiView = 38 | OptionMSLBit,
    MSL_ViewMaskBufferIndex = 39 | OptionMSLBit,
    MSL_DeviceEIndex = 40 | OptionMSLBit,
    MSL_ViewIndexFromDeviceIndex = 41 | OptionMSLBit,
    MSL_DispatchBase = 42 | OptionMSLBit,
    MSL_DynamicOffsetsBufferIndex = 43 | OptionMSLBit,
    MSLTexture1DAs2D = 44 | OptionMSLBit,
    MSL_EnableBaseIndexZero = 45 | OptionMSLBit,

    /// <summary>
    /// Obsolete. Use <see cref="MSL_FramebufferFetchSubpass"/> instead.
    /// </summary>
    MSL_IOSFramebufferFetchSubpass = 46 | OptionMSLBit,
    MSL_FramebufferFetchSubpass = 46 | OptionMSLBit,

    MSL_InvariantFPMath = 47 | OptionMSLBit,
    MSL_EmulateCubemapArray = 48 | OptionMSLBit,
    MSL_EnableDecorationBinding = 49 | OptionMSLBit,
    MSL_ForceActiveArgumentBufferResources = 50 | OptionMSLBit,
    MSL_ForceNativeArrays = 51 | OptionMSLBit,

    EnableStorageImageQualifierDeduction = 52 | OptionCommonBit,

    HLSL_ForceStorageBufferAsUAV = 53 | OptionHSLBit,

    ForceZeroInitializedVariables = 54 | OptionCommonBit,

    HLSL_NonWriteableUAVTextureAsSRV = 55 | OptionHSLBit,

    MSL_EnableFragOutput_MASK = 56 | OptionMSLBit,
    MSL_EnableFragDepthBuiltin = 57 | OptionMSLBit,
    MSL_EnableFragStencilRefBuiltin = 58 | OptionMSLBit,
    MSL_EnableClipDistanceUserVarying = 59 | OptionMSLBit,

    HLSL_Enable16BitTypes = 60 | OptionHSLBit,

    MSL_MULTI_PatchWORKGROUP = 61 | OptionMSLBit,
    MSL_ShaderInputBufferIndex = 62 | OptionMSLBit,
    MSL_ShaderIndexBufferIndex = 63 | OptionMSLBit,
    MSL_VertexFOR_TESSELLATION = 64 | OptionMSLBit,
    MSL_VertexIndexType = 65 | OptionMSLBit,

    GLSL_ForceFlattenedIOBlocks = 66 | OptionGLSLBit,

    MSL_MultiviewLayeredRendering = 67 | OptionMSLBit,
    MSL_ArrayedSubpassInput = 68 | OptionMSLBit,
    MSL_R32UILinearTextureAlignment = 69 | OptionMSLBit,
    MSL_R32UIAlignmentConstantID = 70 | OptionMSLBit,

    HLSL_FlattenMatrixVertexInputSemantics = 71 | OptionHSLBit,

    MSL_IOS_UseSIMDGroupFunctions = 72 | OptionMSLBit,
    MSL_EmulateSubGroups = 73 | OptionMSLBit,
    FixedSubGroupSize = 74 | OptionMSLBit,
    MSL_ForceSampleRateShading = 75 | OptionMSLBit,
    MSL_IOS_SupportBaseVertexInstance = 76 | OptionMSLBit,

    GLSL_OVRMultiView_ViewCount = 77 | OptionGLSLBit,

    RelaxNaNChecks = 78 | OptionCommonBit,

    MSL_RAWBufferTeseInput = 79 | OptionMSLBit,
    MSL_ShaderPatchInputBufferIndex = 80 | OptionMSLBit,
    MSL_ManualHelperInvocationUpdates = 81 | OptionMSLBit,
    MSL_CheckDiscardedFragStores = 82 | OptionMSLBit,

    GLSL_EnableRowMajorLoadWorkaround = 83 | OptionGLSLBit,

    MSL_ArgumentBuffersTier = 84 | OptionMSLBit,
    MSL_SampleDRefLODArrayAsGrad = 85 | OptionMSLBit,
}
