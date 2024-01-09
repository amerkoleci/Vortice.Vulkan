// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline input assembly state
/// </summary>
public partial struct VkPipelineInputAssemblyStateCreateInfo
{
    public unsafe VkPipelineInputAssemblyStateCreateInfo(
        VkPrimitiveTopology topology,
        bool primitiveRestartEnable = false,
        void* pNext = default,
        VkPipelineInputAssemblyStateCreateFlags flags = VkPipelineInputAssemblyStateCreateFlags.None)
    {
        sType = VkStructureType.PipelineInputAssemblyStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.topology = topology;
        this.primitiveRestartEnable = primitiveRestartEnable;
    }
}
