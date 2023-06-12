// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Information about existing #VmaAllocator object.
/// </summary>
public unsafe struct VmaAllocatorInfo
{
    /// <summary>
    /// Handle to Vulkan instance object.
    /// This is the same value as has been passed through <see cref="VmaAllocatorCreateInfo.Instance"/>.
    /// </summary>
    public VkInstance Instance;
    /// <summary>
    /// Handle to Vulkan physical device object.
    /// This is the same value as has been passed through <see cref="VmaAllocatorCreateInfo.PhysicalDevice"/>.
    /// </summary>
    public VkPhysicalDevice PhysicalDevice;
    /// <summary>
    /// Handle to Vulkan device object.
    /// This is the same value as has been passed through  <see cref="VmaAllocatorCreateInfo.Device"/>.
    /// </summary>
    public VkDevice Device;
}
