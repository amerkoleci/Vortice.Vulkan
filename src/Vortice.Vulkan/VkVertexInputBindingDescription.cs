// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying vertex input binding description
/// </summary>
public partial struct VkVertexInputBindingDescription
{
    public VkVertexInputBindingDescription(
        int stride,
        VkVertexInputRate inputRate = VkVertexInputRate.Vertex,
        uint binding = 0)
    {
        this.binding = binding;
        this.stride = (uint)stride;
        this.inputRate = inputRate;
    }

    public VkVertexInputBindingDescription(
        uint stride,
        VkVertexInputRate inputRate = VkVertexInputRate.Vertex,
        uint binding = 0)
    {
        this.binding = binding;
        this.stride = stride;
        this.inputRate = inputRate;
    }
}
