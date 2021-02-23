// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a pipeline color blend attachment state
    /// </summary>
    public partial struct VkPipelineColorBlendAttachmentState
    {
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
}
