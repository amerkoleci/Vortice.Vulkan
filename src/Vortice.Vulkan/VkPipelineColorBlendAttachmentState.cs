// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a pipeline color blend attachment state
/// </summary>
public partial struct VkPipelineColorBlendAttachmentState
{
    public static VkPipelineColorBlendAttachmentState Opaque => new(false, VkBlendFactor.One, VkBlendFactor.Zero, VkBlendOp.Add, VkBlendFactor.One, VkBlendFactor.Zero, VkBlendOp.Add, VkColorComponentFlags.All);
    public static VkPipelineColorBlendAttachmentState AlphaBlend => new(true, VkBlendFactor.One, VkBlendFactor.OneMinusSrcAlpha, VkBlendOp.Add, VkBlendFactor.One, VkBlendFactor.OneMinusSrcAlpha, VkBlendOp.Add, VkColorComponentFlags.All);
    public static VkPipelineColorBlendAttachmentState Additive => new(true, VkBlendFactor.SrcAlpha, VkBlendFactor.One, VkBlendOp.Add, VkBlendFactor.SrcAlpha, VkBlendFactor.One, VkBlendOp.Add, VkColorComponentFlags.All);
    public static VkPipelineColorBlendAttachmentState NonPremultiplied => new(true, VkBlendFactor.SrcAlpha, VkBlendFactor.OneMinusSrcAlpha, VkBlendOp.Add, VkBlendFactor.SrcAlpha, VkBlendFactor.OneMinusSrcAlpha, VkBlendOp.Add, VkColorComponentFlags.All);

    public VkPipelineColorBlendAttachmentState(
        bool blendEnable = false,
        VkBlendFactor srcColorBlendFactor = VkBlendFactor.One,
        VkBlendFactor dstColorBlendFactor = VkBlendFactor.Zero,
        VkBlendOp colorBlendOp = VkBlendOp.Add,
        VkBlendFactor srcAlphaBlendFactor = VkBlendFactor.One,
        VkBlendFactor dstAlphaBlendFactor = VkBlendFactor.Zero,
        VkBlendOp alphaBlendOp = VkBlendOp.Add,
        VkColorComponentFlags colorWriteMask = VkColorComponentFlags.All)
    {
        this.blendEnable = blendEnable;
        this.srcColorBlendFactor = srcColorBlendFactor;
        this.dstColorBlendFactor = dstColorBlendFactor;
        this.colorBlendOp = colorBlendOp;
        this.srcAlphaBlendFactor = srcAlphaBlendFactor;
        this.dstAlphaBlendFactor = dstAlphaBlendFactor;
        this.alphaBlendOp = alphaBlendOp;
        this.colorWriteMask = colorWriteMask;
    }
}
