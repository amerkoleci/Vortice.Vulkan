using Vortice.Vulkan;

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

        VkBufferCreateInfo bufferInfo;
        bufferInfo.sType = VkStructureType.BufferCreateInfo;
        bufferInfo.size = byteSize;
        bufferInfo.usage = usage;
        bufferInfo.sharingMode = VkSharingMode.Exclusive;

        VmaAllocationCreateInfo allocationCreateInfo;
        allocationCreateInfo.usage = VmaMemoryUsage.Auto;
        if (cpuAccessible)
        {
            allocationCreateInfo.flags = VmaAllocationCreateFlags.HostAccessSequentialWrite |
                                         VmaAllocationCreateFlags.Mapped;
        }

        if (ByteSize == 0) return;

        if (Vma.vmaCreateBuffer(allocator, &bufferInfo, &allocationCreateInfo,
                out VkBuffer, out _allocation) != VkResult.Success)
        {
            throw new Exception("Failed to create buffer!");
        }
    }

    public void Map(VmaAllocator allocator, void** data)
    {
        if (ByteSize == 0) return;

        Vma.vmaMapMemory(allocator, _allocation, data);
    }

    public void Unmap(VmaAllocator allocator)
    {
        if (ByteSize == 0) return;

        Vma.vmaUnmapMemory(allocator, _allocation);
    }

    public void Destroy(VmaAllocator allocator)
    {
        if (ByteSize == 0) return;

        Vma.vmaDestroyBuffer(allocator, VkBuffer, _allocation);
    }
}
