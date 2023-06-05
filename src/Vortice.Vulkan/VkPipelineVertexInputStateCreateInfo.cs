// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline vertex input state
/// </summary>
public partial struct VkPipelineVertexInputStateCreateInfo
{
    public unsafe VkPipelineVertexInputStateCreateInfo(
        uint vertexBindingDescriptionCount, VkVertexInputBindingDescription* pVertexBindingDescriptions,
        uint vertexAttributeDescriptionCount, VkVertexInputAttributeDescription* pVertexAttributeDescriptions,
        void* pNext = default,
        VkPipelineVertexInputStateCreateFlags flags = VkPipelineVertexInputStateCreateFlags.None)
    {
        Unsafe.SkipInit(out this);

        sType = VkStructureType.PipelineVertexInputStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.vertexBindingDescriptionCount = vertexBindingDescriptionCount;
        this.pVertexBindingDescriptions = pVertexBindingDescriptions;
        this.vertexAttributeDescriptionCount = vertexAttributeDescriptionCount;
        this.pVertexAttributeDescriptions = pVertexAttributeDescriptions;
    }
}
