// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void vmaAllocateDeviceMemoryFunction(VmaAllocator allocator, uint memoryType, VkDeviceMemory memory, ulong size, void* userData);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void vmaFreeDeviceMemoryFunction(VmaAllocator allocator, uint memoryType, VkDeviceMemory memory, ulong size, void* userData);
