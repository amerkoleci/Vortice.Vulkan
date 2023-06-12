// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Intended usage of the allocated memory.
/// </summary>
public enum VmaMemoryUsage
{
    /// <summary>
    /// No intended memory usage specified.
    /// Use other members of <see cref="VmaAllocationCreateInfo"/> to specify your requirements
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// Obsolete, preserved for backward compatibility.
    /// Prefers  <see cref="VkMemoryPropertyFlags.DeviceLocal"/>
    /// </summary>
    [Obsolete]
    GpuOnly = 1,
    [Obsolete]
    CpuOnly = 2,
    [Obsolete]
    CpuToGpuy = 3,
    [Obsolete]
    GpuToGpu = 4,
    [Obsolete]
    CpuCopy = 5,
    GpuLazilyAllocated = 6,
    Auto = 7,
    AutoPreferDevice = 8,
    AutoPreferHost = 9,
}
