// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying an attachment description.
/// </summary>
public unsafe partial struct VkAttachmentDescription2
{
    public VkAttachmentDescription2(
        VkFormat format,
        VkSampleCountFlags samples,
        VkAttachmentLoadOp loadOp,
        VkAttachmentStoreOp storeOp,
        VkAttachmentLoadOp stencilLoadOp,
        VkAttachmentStoreOp stencilStoreOp,
        VkImageLayout initialLayout,
        VkImageLayout finalLayout,
        VkAttachmentDescriptionFlags flags = VkAttachmentDescriptionFlags.None,
        void* pNext = default)
    {
        this.pNext = pNext;
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
