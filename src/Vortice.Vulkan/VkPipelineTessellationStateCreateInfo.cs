// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline tessellation state
/// </summary>
public partial struct VkPipelineTessellationStateCreateInfo
{
    public unsafe VkPipelineTessellationStateCreateInfo(
        uint patchControlPoints,
        void* pNext = default,
        VkPipelineTessellationStateCreateFlags flags = VkPipelineTessellationStateCreateFlags.None)
    {
        this.pNext = pNext;
        this.flags = flags;
        this.patchControlPoints = patchControlPoints;
    }
}
