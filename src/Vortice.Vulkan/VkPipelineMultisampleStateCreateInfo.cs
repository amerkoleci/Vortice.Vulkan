// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline multisample state
/// </summary>
public unsafe partial struct VkPipelineMultisampleStateCreateInfo
{
    public static VkPipelineMultisampleStateCreateInfo Default => new(VkSampleCountFlags.Count1);

    public VkPipelineMultisampleStateCreateInfo(
        VkSampleCountFlags rasterizationSamples = VkSampleCountFlags.Count1,
        bool sampleShadingEnable = false,
        float minSampleShading = 0.0f,
        uint* pSampleMask = default,
        bool alphaToCoverageEnable = false,
        bool alphaToOneEnable = false,
        void* pNext = default,
        VkPipelineMultisampleStateCreateFlags flags = VkPipelineMultisampleStateCreateFlags.None)
    {
        Unsafe.SkipInit(out this);

        this.sType = VkStructureType.PipelineMultisampleStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.rasterizationSamples = rasterizationSamples;
        this.sampleShadingEnable = sampleShadingEnable;
        this.minSampleShading = minSampleShading;
        this.pSampleMask = pSampleMask;
        this.alphaToCoverageEnable = alphaToCoverageEnable;
        this.alphaToOneEnable = alphaToOneEnable;
    }
}
