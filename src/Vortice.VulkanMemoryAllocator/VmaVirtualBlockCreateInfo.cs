// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VkDeviceSize = System.UInt64;

namespace Vortice.Vulkan;

public unsafe struct VmaVirtualBlockCreateInfo
{
    /// <summary>
    /// Total size of the virtual block.
    ///
    /// Sizes can be expressed in bytes or any units you want as long as you are consistent in using them.
    /// For example, if you allocate from some array of structures, 1 can mean single instance of entire structure.
    /// </summary>
    public VkDeviceSize size;

    /// <summary>
    /// Use combination of <see cref="VmaVirtualBlockCreateFlags"/>.
    /// </summary>
    public VmaVirtualBlockCreateFlags flags;

    /// <summary>
    /// Custom CPU memory allocation callbacks. Optional.
    /// Optional, can be null. When specified, they will be used for all CPU-side memory allocations.
    /// </summary>
    public VkAllocationCallbacks*  pAllocationCallbacks;
}
