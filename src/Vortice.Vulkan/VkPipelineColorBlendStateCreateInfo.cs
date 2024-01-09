// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline color blend state
/// </summary>
public unsafe partial struct VkPipelineColorBlendStateCreateInfo
{
    public VkPipelineColorBlendStateCreateInfo(
        VkPipelineColorBlendAttachmentState attachment,
        bool logicOpEnable = false,
        VkLogicOp logicOp = VkLogicOp.Clear,
        void* pNext = default,
        VkPipelineColorBlendStateCreateFlags flags = VkPipelineColorBlendStateCreateFlags.None)
    {
        this.sType = VkStructureType.PipelineColorBlendStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.logicOpEnable = logicOpEnable;
        this.logicOp = logicOp;
        this.attachmentCount = 1;
        this.pAttachments = &attachment;
        this.blendConstants[0] = 1.0f;
        this.blendConstants[1] = 1.0f;
        this.blendConstants[2] = 1.0f;
        this.blendConstants[3] = 1.0f;
    }

    public VkPipelineColorBlendStateCreateInfo(
        uint attachmentCount,
        VkPipelineColorBlendAttachmentState* pAttachments,
        bool logicOpEnable = false,
        VkLogicOp logicOp = VkLogicOp.Clear,
        void* pNext = default,
        VkPipelineColorBlendStateCreateFlags flags = VkPipelineColorBlendStateCreateFlags.None)
    {
        this.sType = VkStructureType.PipelineColorBlendStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.logicOpEnable = logicOpEnable;
        this.logicOp = logicOp;
        this.attachmentCount = attachmentCount;
        this.pAttachments = pAttachments;
        this.blendConstants[0] = 1.0f;
        this.blendConstants[1] = 1.0f;
        this.blendConstants[2] = 1.0f;
        this.blendConstants[3] = 1.0f;
    }
}
