// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

public enum ShaderKind : uint
{
    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    VertexShader = 0,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    FragmentShader = 1,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    ComputeShader = 2,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GeometryShader = 3,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    TessControlShader = 4,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    TessEvaluationShader = 5,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GLSL_VertexShader = 0,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GLSL_FragmentShader = 1,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GLSL_ComputeShader = 2,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GLSL_GeometryShader = 3,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GLSL_TessControlShader = 4,

    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    /// source code as the specified kind of shader.
    /// </summary>
    GLSL_TessEvaluationShader = 5,

    /// <summary>
    /// Deduce the shader kind from #pragma annotation in the source code. Compiler
    /// will emit error if #pragma annotation is not found.
    /// </summary>
    GLSL_InferFromSource = 6,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultVertexShader = 7,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultFragmentShader = 8,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultComputeShader = 9,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultGeometryShader = 10,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultTessControlShader = 11,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultTessEvaluationShader = 12,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    SPIRVAssembly = 13,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    RaygenShader = 14,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    AnyHitShader = 15,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    ClosestHitShader = 16,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    MissShader = 17,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    IntersectionShader = 18,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    CallableShader = 19,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_RaygenShader = 14,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_AnyHitShader = 15,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_ClosestHitShader = 16,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_MissShader = 17,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_IntersectionShader = 18,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_CallableShader = 19,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    shaderc_glsl_default_raygen_shader = 20,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultAnyHitShader = 21,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultClosestHitShader = 22,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultMissShader = 23,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultIntersectionShader = 24,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultCallableShader = 25,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    TaskShader = 26,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    MeshShader = 27,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_TaskShader = 26,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_MeshShader = 27,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultTaskShader = 28,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GLSL_DefaultMeshShader = 29,
}
