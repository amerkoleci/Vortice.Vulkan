// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created fence.
/// </summary>
public partial struct VkFenceCreateInfo
{
    public unsafe VkFenceCreateInfo(VkFenceCreateFlags flags = VkFenceCreateFlags.None, void* pNext = default)
    {
        this.pNext = pNext;
        this.flags = flags;
    }
}
