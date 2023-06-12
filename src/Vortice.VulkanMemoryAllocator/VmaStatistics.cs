// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

public struct VmaStatistics
{
    /// <summary>
    /// Number of `<see cref="VkDeviceMemory"/>` objects - Vulkan memory blocks allocated.
    /// </summary>
    public uint blockCount;
    /// <summary>
    /// Number of <see cref="VmaAllocation"/> objects allocated.
    /// Dedicated allocations have their own blocks, so each one adds 1 to `allocationCount` as well as `blockCount`.
    /// </summary>
    public uint allocationCount;
    /// <summary>
    /// Number of bytes allocated in `VkDeviceMemory` blocks.
    /// 
    /// Note: To avoid confusion, please be aware that what Vulkan calls an "allocation" - a whole `<see cref="VkDeviceMemory"/>` object
    /// (e.g. as in `<see cref="VkPhysicalDeviceLimits.maxMemoryAllocationCount"/>`) is called a "block" in VMA, while VMA calls
    /// "allocation" a <see cref="VmaAllocation"/> object that represents a memory region sub-allocated from such block, usually for a single buffer or image.
    /// </summary>
    public ulong blockBytes;
    /// <summary>
    /// Total number of bytes occupied by all <see cref="VmaAllocation"/> objects.
    /// 
    /// Always less or equal than `<see cref="blockBytes"/>`.
    /// Difference `(<see cref="blockBytes"/> - <see cref="allocationBytes"/>)` is the amount of memory allocated from Vulkan but unused by any <see cref="VmaAllocation"/>.
    /// </summary>
    public ulong allocationBytes;
}
