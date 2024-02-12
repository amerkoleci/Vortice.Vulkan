// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vma;

namespace DrawTriangleVma;

public unsafe struct Buffer
{
    public readonly uint ByteSize;
    public readonly VkBuffer VkBuffer;
    private readonly VmaAllocation _allocation;

    public Buffer()
    {
        ByteSize = 0;
    }

    public Buffer(VmaAllocator allocator, uint byteSize, VkBufferUsageFlags usage, bool cpuAccessible)
    {
        ByteSize = byteSize;

        VkBufferCreateInfo bufferInfo = new()
        {
            size = byteSize,
            usage = usage,
            sharingMode = VkSharingMode.Exclusive
        };

        VmaAllocationCreateInfo allocationCreateInfo;
        allocationCreateInfo.usage = VmaMemoryUsage.Auto;
        if (cpuAccessible)
        {
            allocationCreateInfo.flags = VmaAllocationCreateFlags.HostAccessSequentialWrite |
                                         VmaAllocationCreateFlags.Mapped;
        }

        if (ByteSize == 0) return;

        if (vmaCreateBuffer(allocator, &bufferInfo, &allocationCreateInfo, out VkBuffer, out _allocation) != VkResult.Success)
        {
            throw new Exception("Failed to create buffer!");
        }
    }

    public void Map(VmaAllocator allocator, void** data)
    {
        if (ByteSize == 0)
            return;

        vmaMapMemory(allocator, _allocation, data).CheckResult();
    }

    public void Unmap(VmaAllocator allocator)
    {
        if (ByteSize == 0)
            return;

        vmaUnmapMemory(allocator, _allocation);
    }

    public void Destroy(VmaAllocator allocator)
    {
        if (ByteSize == 0) return;

        vmaDestroyBuffer(allocator, VkBuffer, _allocation);
    }
}
