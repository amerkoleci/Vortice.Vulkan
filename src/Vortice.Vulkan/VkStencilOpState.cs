﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying stencil operation state
/// </summary>
public partial struct VkStencilOpState
{
    /// <summary>
    /// A built-in description with default values.
    /// </summary>
    public static VkStencilOpState Default => new(VkStencilOp.Keep, VkStencilOp.Keep, VkStencilOp.Keep, VkCompareOp.Always);

    public VkStencilOpState(
        VkStencilOp failOp = VkStencilOp.Keep,
        VkStencilOp passOp = VkStencilOp.Keep,
        VkStencilOp depthFailOp = VkStencilOp.Keep,
        VkCompareOp compareOp = VkCompareOp.Always,
        uint compareMask = uint.MaxValue,
        uint writeMask = uint.MaxValue,
        uint reference = 0)
    {
        this.failOp = failOp;
        this.passOp = passOp;
        this.depthFailOp = depthFailOp;
        this.compareOp = compareOp;
        this.compareMask = compareMask;
        this.writeMask = writeMask;
        this.reference = reference;
    }
}
