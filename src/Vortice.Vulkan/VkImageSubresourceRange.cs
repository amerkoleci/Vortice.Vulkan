// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
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
}
