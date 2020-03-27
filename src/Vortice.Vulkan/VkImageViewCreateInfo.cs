// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying parameters of a newly created image view
    /// </summary>
    public partial struct VkImageViewCreateInfo
    {
        public unsafe VkImageViewCreateInfo(
            VkImage image,
            VkImageViewType viewType,
            VkFormat format,
            VkComponentMapping components,
            VkImageSubresourceRange subresourceRange,
            VkImageViewCreateFlags flags = VkImageViewCreateFlags.None,
            void* pNext = default)
        {
            sType = VkStructureType.ImageViewCreateInfo;
            this.pNext = pNext;
            this.flags = flags;
            this.image = image;
            this.viewType = viewType;
            this.format = format;
            this.components = components;
            this.subresourceRange = subresourceRange;
        }
    }
}
