// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VkDeviceSize = System.UInt64;

namespace Vortice.Vulkan;

public unsafe struct VmaVirtualAllocationInfo
{
    /// <summary>
    /// Offset of the allocation.
    /// Offset at which the allocation was made.
    /// </summary>
    public VkDeviceSize offset;
    /// <summary>
    /// Size of the allocation.
    /// Same value as passed in <see cref="VmaVirtualAllocationCreateInfo.size"/>.
    /// </summary>
    public VkDeviceSize size;
    /// <summary>
    /// Custom pointer associated with the allocation.
    /// Same value as passed in <see cref="VmaVirtualAllocationCreateInfo.userData"/>  or to <see cref="Vma.vmaSetVirtualAllocationUserData(VmaVirtualBlock, VmaVirtualAllocation, nuint)"/>
    /// </summary>
    public nint userData;
}
