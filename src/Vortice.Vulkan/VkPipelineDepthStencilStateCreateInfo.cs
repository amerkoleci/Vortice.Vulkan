// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline depth stencil state
/// </summary>
public unsafe partial struct VkPipelineDepthStencilStateCreateInfo
{
    /// <summary>
    /// A built-in description with settings for not using a depth stencil buffer.
    /// </summary>
    public static VkPipelineDepthStencilStateCreateInfo None => new(false, false, VkCompareOp.LessOrEqual);

    /// <summary>
    /// A built-in description with default settings for using a depth stencil buffer.
    /// </summary>
    public static VkPipelineDepthStencilStateCreateInfo Default => new(true, true, VkCompareOp.LessOrEqual);

    /// <summary>
    /// A built-in description with settings for enabling a read-only depth stencil buffer.
    /// </summary>
    public static VkPipelineDepthStencilStateCreateInfo Read => new(true, false, VkCompareOp.LessOrEqual);

    /// <summary>
    /// A built-in description with default settings for using a reverse depth stencil buffer.
    /// </summary>
    public static VkPipelineDepthStencilStateCreateInfo ReverseZ => new(true, true, VkCompareOp.GreaterOrEqual);

    /// <summary>
    /// A built-in description with default settings for using a reverse read-only depth stencil buffer.
    /// </summary>
    public static VkPipelineDepthStencilStateCreateInfo ReadReverseZ => new(true, false, VkCompareOp.GreaterOrEqual);

    public VkPipelineDepthStencilStateCreateInfo(
        bool depthTestEnable,
        bool depthWriteEnable,
        VkCompareOp depthCompareOp,
        VkPipelineDepthStencilStateCreateFlags flags = VkPipelineDepthStencilStateCreateFlags.None,
        void* pNext = default)
    {
        this.flags = flags;
        this.pNext = pNext;
        this.depthTestEnable = depthTestEnable;
        this.depthWriteEnable = depthWriteEnable;
        this.depthCompareOp = depthCompareOp;
        this.depthBoundsTestEnable = false;
        this.stencilTestEnable = false;
        this.front = new(VkStencilOp.Keep, VkStencilOp.Keep, VkStencilOp.Keep, VkCompareOp.Always);
        this.back = new(VkStencilOp.Keep, VkStencilOp.Keep, VkStencilOp.Keep, VkCompareOp.Always);
        this.minDepthBounds = 0.0f;
        this.maxDepthBounds = 0.0f;
    }

    public VkPipelineDepthStencilStateCreateInfo(
        bool depthTestEnable,
        bool depthWriteEnable,
        VkCompareOp depthCompareOp,
        bool stencilTestEnable,
        VkStencilOpState front,
        VkStencilOpState back,
        VkPipelineDepthStencilStateCreateFlags flags = VkPipelineDepthStencilStateCreateFlags.None,
        void* pNext = default)
    {
        this.flags = flags;
        this.pNext = pNext;
        this.depthTestEnable = depthTestEnable;
        this.depthWriteEnable = depthWriteEnable;
        this.depthCompareOp = depthCompareOp;
        this.depthBoundsTestEnable = false;
        this.stencilTestEnable = stencilTestEnable;
        this.front = front;
        this.back = back;
        this.minDepthBounds = 0.0f;
        this.maxDepthBounds = 0.0f;
    }

    public VkPipelineDepthStencilStateCreateInfo(
        bool depthTestEnable,
        bool depthWriteEnable,
        VkCompareOp depthCompareOp,
        bool depthBoundsTestEnable,
        bool stencilTestEnable,
        VkStencilOpState front,
        VkStencilOpState back,
        float minDepthBounds,
        float maxDepthBounds,
        VkPipelineDepthStencilStateCreateFlags flags = VkPipelineDepthStencilStateCreateFlags.None,
        void* pNext = default)
    {
        this.flags = flags;
        this.pNext = pNext;
        this.depthTestEnable = depthTestEnable;
        this.depthWriteEnable = depthWriteEnable;
        this.depthCompareOp = depthCompareOp;
        this.depthBoundsTestEnable = depthBoundsTestEnable;
        this.stencilTestEnable = stencilTestEnable;
        this.front = front;
        this.back = back;
        this.minDepthBounds = minDepthBounds;
        this.maxDepthBounds = maxDepthBounds;
    }
}
