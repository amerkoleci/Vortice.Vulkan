// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VkDeviceSize = System.UInt64;

namespace Vortice.Vulkan;

public unsafe struct VmaVirtualAllocationCreateInfo
{
    /// <summary>
    /// Size of the allocation.
    ///
    /// Cannot be zoer.
    /// </summary>
    public VkDeviceSize size;

    /// <summary>
    /// Optional: Required alignment of the allocation.
    /// </summary>
    public VkDeviceSize alignment;

    public VmaVirtualAllocationCreateFlags flags;

    public nuint userData;
}
