// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

public struct VmaBudget
{
    /// <summary>
    /// Statistics fetched from the library.
    /// </summary>
    public VmaStatistics statistics;
    /// <summary>
    /// Estimated current memory usage of the program, in bytes.
    /// 
    /// Fetched from system using VK_EXT_memory_budget extension if enabled.
    /// 
    /// It might be different than `statistics.blockBytes` (usually higher) due to additional implicit objects
    /// also occupying the memory, like swapchain, pipelines, descriptor heaps, command buffers, or
    /// `VkDeviceMemory` blocks allocated outside of this library, if any.
    /// </summary>
    public ulong usage;
    /// <summary>
    /// Estimated amount of memory available to the program, in bytes.
    /// 
    /// Fetched from system using VK_EXT_memory_budget extension if enabled.
    /// 
    /// It might be different(most probably smaller) than `VkMemoryHeap::size[heapIndex]` due to factors
    /// external to the program, decided by the operating system.
    /// Difference `budget - usage` is the amount of additional memory that can probably
    /// be allocated without problems.Exceeding the budget may result in various problems.
    /// </summary>
    public ulong budget;
}
