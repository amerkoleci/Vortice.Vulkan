// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a global memory barrier.
/// </summary>
public partial struct VkMemoryBarrier
{
    public unsafe VkMemoryBarrier(VkAccessFlags srcAccessMask, VkAccessFlags dstAccessMask, void* pNext = default)
    {
        this.pNext = pNext;
        this.srcAccessMask = srcAccessMask;
        this.dstAccessMask = dstAccessMask;
    }
}
