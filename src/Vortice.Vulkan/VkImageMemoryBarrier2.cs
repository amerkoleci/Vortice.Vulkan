// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying the parameters of an image memory barrier.
/// </summary>
public unsafe partial struct VkImageMemoryBarrier2
{
    public VkImageMemoryBarrier2(
        VkImage image,
        VkImageSubresourceRange subresourceRange,
        VkPipelineStageFlags2 srcStageMask,
        VkAccessFlags2 srcAccessMask,
        VkPipelineStageFlags2 dstStageMask,
        VkAccessFlags2 dstAccessMask,
        VkImageLayout oldLayout,
        VkImageLayout newLayout,
        uint srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
        uint dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
        void* pNext = default)
    {
        sType = VkStructureType.ImageMemoryBarrier2;
        this.pNext = pNext;
        this.srcStageMask = srcStageMask;
        this.srcAccessMask = srcAccessMask;
        this.dstStageMask = dstStageMask;
        this.dstAccessMask = dstAccessMask;
        this.oldLayout = oldLayout;
        this.newLayout = newLayout;
        this.srcQueueFamilyIndex = srcQueueFamilyIndex;
        this.dstQueueFamilyIndex = dstQueueFamilyIndex;
        this.image = image;
        this.subresourceRange = subresourceRange;
    }
}
