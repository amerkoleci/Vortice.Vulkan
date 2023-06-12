// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void vmaAllocateDeviceMemoryFunction(VmaAllocator allocator, uint memoryType, VkDeviceMemory memory, ulong size, nint userData);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void vmaFreeDeviceMemoryFunction(VmaAllocator allocator, uint memoryType, VkDeviceMemory memory, ulong size, nint userData);

public unsafe struct VmaDeviceMemoryCallbacks
{
    /// <summary>
    /// <see cref="vmaAllocateDeviceMemoryFunction"/>
    /// </summary>
    public delegate* unmanaged<VmaAllocator, uint, VkDeviceMemory, ulong, nint> pfnAllocate;

    /// <summary>
    /// <see cref="vmaFreeDeviceMemoryFunction"/>
    /// </summary>
    public delegate* unmanaged<VkDevice, sbyte*, delegate* unmanaged<void>> pfnFree;

    /// <summary>
    /// Optional: user data.
    /// </summary>
    public nint userData;
}
