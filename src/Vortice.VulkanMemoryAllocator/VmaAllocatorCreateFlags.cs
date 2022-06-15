// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VmaAllocatorCreateFlags : uint
{
    ExternallySynchronized = 0x00000001,
    KHRDedicatedAllocation = 0x00000002,
    KHRBindMemory2 = 0x00000004,
    VMA_ALLOCATOR_CREATE_EXT_MEMORY_BUDGET_BIT = 0x00000008,
    VMA_ALLOCATOR_CREATE_AMD_DEVICE_COHERENT_MEMORY_BIT = 0x00000010,
    VMA_ALLOCATOR_CREATE_BUFFER_DEVICE_ADDRESS_BIT = 0x00000020,
    VMA_ALLOCATOR_CREATE_EXT_MEMORY_PRIORITY_BIT = 0x00000040,
}
