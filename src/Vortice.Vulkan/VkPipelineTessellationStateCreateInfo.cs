// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

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
        this.sType = VkStructureType.PipelineTessellationStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.patchControlPoints = patchControlPoints;
    }
}
