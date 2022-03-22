// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying an image subresource layers.
/// </summary>
public partial struct VkImageSubresourceLayers
{
    public VkImageSubresourceLayers(
        VkImageAspectFlags aspectMask,
        uint mipLevel,
        uint baseArrayLayer, uint layerCount)
    {
        this.aspectMask = aspectMask;
        this.mipLevel = mipLevel;
        this.baseArrayLayer = baseArrayLayer;
        this.layerCount = layerCount;
    }
}
