// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

public struct VmaDetailedStatistics
{
    /// <summary>
    /// Basic statistics.
    /// </summary>
    public VmaStatistics statistics;
    /// <summary>
    /// Number of free ranges of memory between allocations.
    /// </summary>
    public uint unusedRangeCount;
    /// <summary>
    /// Smallest allocation size. `VK_WHOLE_SIZE` if there are 0 allocations.
    /// </summary>
    public ulong allocationSizeMin;
    /// <summary>
    /// Largest allocation size. 0 if there are 0 allocations.
    /// </summary>
    public ulong allocationSizeMax;
    /// <summary>
    /// Smallest empty range size. `VK_WHOLE_SIZE` if there are 0 empty ranges.
    /// </summary>
    public ulong unusedRangeSizeMin;
    /// <summary>
    /// Largest empty range size. 0 if there are 0 empty ranges.
    /// </summary>
    public ulong unusedRangeSizeMax;
}
