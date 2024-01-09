// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a buffer memory barrier.
/// </summary>
public unsafe partial struct VkBufferMemoryBarrier2
{
    public VkBufferMemoryBarrier2(
        VkBuffer buffer,
        VkPipelineStageFlags2 srcStageMask,
        VkAccessFlags2 srcAccessMask,
        VkPipelineStageFlags2 dstStageMask,
        VkAccessFlags2 dstAccessMask,
        ulong offset = 0,
        ulong size = VK_WHOLE_SIZE,
        uint srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
        uint dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
        void* pNext = default)
    {
        sType = VkStructureType.BufferMemoryBarrier2;
        this.pNext = pNext;
        this.srcStageMask = srcStageMask;
        this.srcAccessMask = srcAccessMask;
        this.dstStageMask = dstStageMask;
        this.dstAccessMask = dstAccessMask;
        this.srcQueueFamilyIndex = srcQueueFamilyIndex;
        this.dstQueueFamilyIndex = dstQueueFamilyIndex;
        this.buffer = buffer;
        this.offset = offset;
        this.size = size;
    }
}
