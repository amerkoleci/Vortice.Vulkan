// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline viewport state
/// </summary>
public partial struct VkPipelineViewportStateCreateInfo
{
    public unsafe VkPipelineViewportStateCreateInfo(
        uint viewportCount, uint scissorCount, 
        void* pNext = default,
        VkPipelineViewportStateCreateFlags flags = VkPipelineViewportStateCreateFlags.None)
    {
        Unsafe.SkipInit(out this);

        sType = VkStructureType.PipelineViewportStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.viewportCount = viewportCount;
        this.pViewports = default;
        this.scissorCount = scissorCount;
        this.pScissors = default;
    }

    public unsafe VkPipelineViewportStateCreateInfo(
        uint viewportCount, VkViewport* pViewports,
        uint scissorCount, VkRect2D* pScissors,
        void* pNext = default,
        VkPipelineViewportStateCreateFlags flags = VkPipelineViewportStateCreateFlags.None)
    {
        Unsafe.SkipInit(out this);

        sType = VkStructureType.PipelineViewportStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.viewportCount = viewportCount;
        this.pViewports = pViewports;
        this.scissorCount = scissorCount;
        this.pScissors = pScissors;
    }
}
