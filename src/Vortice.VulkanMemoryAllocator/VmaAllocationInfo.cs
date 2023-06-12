// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Parameters of #VmaAllocation objects, that can be retrieved using function <see cref="Vma.vmaGetAllocationInfo(VmaAllocator, VmaAllocation, Vortice.Vulkan.VmaAllocationInfo*)"/>.
/// </summary>
public unsafe readonly struct VmaAllocationInfo
{
    public readonly uint memoryType;
    public readonly VkDeviceMemory deviceMemory;
    public readonly ulong offset;
    public readonly ulong size;
    public readonly void* pMappedData;
    public readonly void* pUserData;
    public readonly sbyte* pName;
}
