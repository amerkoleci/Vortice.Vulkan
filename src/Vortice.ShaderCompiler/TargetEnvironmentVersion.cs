// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

public enum TargetEnvironmentVersion : uint
{
    /// <summary>
    /// For Vulkan, use Vulkan's mapping of version numbers to integers.
    /// See vulkan.h
    /// </summary>
    Vulkan_1_0 = ((1u << 22)),

    /// <summary>
    /// For Vulkan, use Vulkan's mapping of version numbers to integers.
    /// See vulkan.h
    /// </summary>
    Vulkan_1_1 = ((1u << 22) | (1 << 12)),

    /// <summary>
    /// For Vulkan, use Vulkan's mapping of version numbers to integers.
    /// See vulkan.h
    /// </summary>
    Vulkan_1_2 = ((1u << 22) | (2 << 12)),

    /// <summary>
    /// For Vulkan, use Vulkan's mapping of version numbers to integers.
    /// See vulkan.h
    /// </summary>
    Vulkan_1_3 = ((1u << 22) | (3 << 12)),

    /// <summary>
    /// For OpenGL, use the number from #version in shaders.
    /// TODO(dneto): Currently no difference between OpenGL 4.5 and 4.6.
    /// See glslang/Standalone/Standalone.cpp
    /// TODO(dneto): Glslang doesn't accept a OpenGL client version of 460.
    /// </summary>
    OpenGL_4_5 = 450,

    /// <summary>
    /// Deprecated, WebGPU env never defined versions
    /// </summary>
    WebGpu,
}
