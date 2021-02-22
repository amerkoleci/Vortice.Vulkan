// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.ShaderCompiler
{
    public enum UniformKind
    {
        /// <summary>
        /// Image and image buffer.
        /// </summary>
        Image,
        /// <summary>
        /// Pure sampler.
        /// </summary>
        Sampler,
        /// <summary>
        /// Sampled texture in GLSL, and Shader Resource View in HLSL.
        /// </summary>
        Texture,
        /// <summary>
        /// Uniform Buffer Object (UBO) in GLSL.  Cbuffer in HLSL.
        /// </summary>
        Buffer,
        /// <summary>
        /// Shader Storage Buffer Object (SSBO) in GLSL.
        /// </summary>
        StorageBuffer,
        /// <summary>
        /// Unordered Access View, in HLSL.  (Writable storage image or storage buffer.)
        /// </summary>
        UnorderedAccessView,
    }
}
