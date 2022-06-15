// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

public enum ShaderKind
{
    VertexShader,
    FragmentShader,
    ComputeShader,
    GeometryShader,
    TessControlShader,
    TessEvaluationShader,

    /// <summary>
    /// Deduce the shader kind from #pragma annotation in the source code. Compiler will emit error if #pragma annotation is not found.
    /// </summary>
    GLSLInferFromSource,
    // Default shader kinds. Compiler will fall back to compile the source code as
    // the specified kind of shader when #pragma annotation is not found in the
    // source code.
    GLSLDefaultVertexShader,
    GLSLDefaultFragmentShader,
    GLSLDefaultComputeShader,
    GLSLDefaultGeometryShader,
    GLSLDefaultTessControlShader,
    GLSLDefaultTessEvaluationShader,
    SPIRVAssembly,
    RaygenShader,
    AnyhitShader,
    ClosesthitShader,
    MissShader,
    IntersectionShader,
    CallableShader,

    GLSLDefaultRaygenShader,
    GLSLDefaultAnyhitShader,
    GLSLDefaultClosesthitShader,
    GLSLDefaultMissShader,
    GLSLDefaultIntersectionShader,
    GLSLDefaultCallableShader,

    TaskShader,
    MeshShader,
    GLSLDefaultTaskShader,
    GLSLDefaultMeshShader,
}
