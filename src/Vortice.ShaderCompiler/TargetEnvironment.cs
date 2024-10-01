// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

public enum TargetEnvironment : uint
{
    /// <summary>
    /// SPIR-V under Vulkan semantics.
    /// </summary>
    Vulkan,
    /// <summary>
    /// SPIR-V under OpenGL semantics.
    /// NOTE: SPIR-V code generation is not supported for shaders under OpenGL compatibility profile.
    /// </summary>
    OpenGL,
    /// <summary>
    /// SPIR-V under OpenGL semantics, including compatibility profile functions
    /// </summary>
    OpenGLCompat,
    /// <summary>
    /// Deprecated, SPIR-V under WebGPU semantics.
    /// </summary>
    WebGPU,
    Default = Vulkan
}
