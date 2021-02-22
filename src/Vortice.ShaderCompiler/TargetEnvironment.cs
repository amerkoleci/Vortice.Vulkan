// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.ShaderCompiler
{
    public enum TargetEnvironment
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
        OpenGLCompate,
        /// <summary>
        /// Deprecated, SPIR-V under WebGPU semantics.
        /// </summary>
        WebGPU,
        Default = Vulkan
    }
}
