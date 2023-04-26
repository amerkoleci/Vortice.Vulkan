// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying vertex input attribute description
/// </summary>
public partial struct VkVertexInputAttributeDescription
{
    public VkVertexInputAttributeDescription(
        uint location,
        VkFormat format,
        uint offset,
        uint binding = 0)
    {
        this.location = location;
        this.binding = binding;
        this.format = format;
        this.offset = offset;
    }
}
