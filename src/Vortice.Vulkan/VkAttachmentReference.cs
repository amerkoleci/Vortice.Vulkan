// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
///  SStructure specifying an attachment reference.
/// </summary>
public partial struct VkAttachmentReference
{
    public VkAttachmentReference(uint attachment, VkImageLayout layout)
    {
        this.attachment = attachment;
        this.layout = layout;
    }
}
