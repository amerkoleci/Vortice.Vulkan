// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying an image subresource range.
/// </summary>
public partial struct VkImageSubresourceRange
{
    public VkImageSubresourceRange(
        VkImageAspectFlags aspectMask,
        uint baseMipLevel, uint levelCount,
        uint baseArrayLayer, uint layerCount)
    {
        this.aspectMask = aspectMask;
        this.baseMipLevel = baseMipLevel;
        this.levelCount = levelCount;
        this.baseArrayLayer = baseArrayLayer;
        this.layerCount = layerCount;
    }
}
