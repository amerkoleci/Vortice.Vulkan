// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a global memory barrier.
/// </summary>
public partial struct VkMemoryBarrier2
{
    public unsafe VkMemoryBarrier2(
        VkPipelineStageFlags2 srcStageMask,
        VkAccessFlags2 srcAccessMask,
        VkPipelineStageFlags2 dstStageMask,
        VkAccessFlags2 dstAccessMask,
        void* pNext = default)
    {
        sType = VkStructureType.MemoryBarrier2;
        this.pNext = pNext;
        this.srcStageMask = srcStageMask;
        this.srcAccessMask = srcAccessMask;
        this.dstStageMask = dstStageMask;
        this.dstAccessMask = dstAccessMask;
    }
}
