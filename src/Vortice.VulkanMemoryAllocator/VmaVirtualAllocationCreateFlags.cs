// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VmaVirtualAllocationCreateFlags : uint
{
    None = 0,
    /// <summary>
    /// Allocation will be created from upper stack in a double stack pool.
    /// This flag is only allowed for virtual blocks created with <see cref="VmaVirtualBlockCreateFlags.LinearAlgorith"/> flag.
    /// </summary>
    UpperAddress = VmaAllocationCreateFlags.UpperAddress,
    /// <summary>
    /// Allocation strategy that tries to minimize memory usage.
    /// </summary>
    StrategyMinMemory = VmaAllocationCreateFlags.StrategyMinMemory,
    /// <summary>
    /// Allocation strategy that tries to minimize allocation time.
    /// </summary>
    StrategyMinTime = VmaAllocationCreateFlags.StrategyMinTime,
    /// <summary>
    /// Allocation strategy that chooses always the lowest offset in available space.
    /// This is not the most efficient strategy but achieves highly packed data.
    /// </summary>
    StrategyMinOffset = VmaAllocationCreateFlags.StrategyMinOffset,
}
