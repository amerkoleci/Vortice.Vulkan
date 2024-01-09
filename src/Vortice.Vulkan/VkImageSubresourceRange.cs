// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying an image subresource range.
/// </summary>
public partial struct VkImageSubresourceRange
{
    public VkImageSubresourceRange(
        VkImageAspectFlags aspectMask,
        uint baseMipLevel = 0, uint levelCount = VK_REMAINING_MIP_LEVELS,
        uint baseArrayLayer = 0, uint layerCount = VK_REMAINING_ARRAY_LAYERS)
    {
        this.aspectMask = aspectMask;
        this.baseMipLevel = baseMipLevel;
        this.levelCount = levelCount;
        this.baseArrayLayer = baseArrayLayer;
        this.layerCount = layerCount;
    }
}
