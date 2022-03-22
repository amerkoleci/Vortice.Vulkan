// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created fence.
/// </summary>
public unsafe partial struct VkFenceCreateInfo
{
    public VkFenceCreateInfo(VkFenceCreateFlags flags = VkFenceCreateFlags.None)
    {
        sType = VkStructureType.FenceCreateInfo;
        pNext = null;
        this.flags = flags;
    }
}
