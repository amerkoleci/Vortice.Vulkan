// Copyright (c) Amer Koleci and Contributors
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    ///  Structure specifying an attachment description.
    /// </summary>
    public partial struct VkAttachmentDescription
    {
        public VkAttachmentDescription(
            VkFormat format,
            VkSampleCountFlags samples,
            VkAttachmentLoadOp loadOp,
            VkAttachmentStoreOp storeOp,
            VkAttachmentLoadOp stencilLoadOp,
            VkAttachmentStoreOp stencilStoreOp,
            VkImageLayout initialLayout,
            VkImageLayout finalLayout,
            VkAttachmentDescriptionFlags flags = VkAttachmentDescriptionFlags.None)
        {
            this.flags = flags;
            this.format = format;
            this.samples = samples;
            this.loadOp = loadOp;
            this.storeOp = storeOp;
            this.stencilLoadOp = stencilLoadOp;
            this.stencilStoreOp = stencilStoreOp;
            this.initialLayout = initialLayout;
            this.finalLayout = finalLayout;
        }
    }
}
