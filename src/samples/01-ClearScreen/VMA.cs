// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Vortice;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan.Vma
{
    [Flags]
    public enum AllocatorCreateFlags : uint
    {
        ExternallySynchronized = 0x00000001,
        KHRDedicatedAllocation = 0x00000002,
        KHRBindMemory2 = 0x00000004,
        EXTMemoryBudget = 0x00000008,
        AMDDeviceCoherentMemory = 0x00000010,
        CreateBufferDeviceAddress = 0x00000020,
        CreateEXTMemoryPriority = 0x00000040,
    }

    public struct AllocatorCreateInfo
    {
        /// <summary>
        /// Flags for created allocator. 
        /// </summary>
        public AllocatorCreateFlags Flags;
        public VkPhysicalDevice PhysicalDevice;
        public VkDevice Device;
        /// <summary>
        /// Preferred optional size of a single `VkDeviceMemory` block to be allocated from large heaps > 1 GiB.
        /// </summary>
        /// <remarks>Set to 0 to use default, which is currently 256 MiB.</remarks>
        public ulong PreferredLargeHeapBlockSize;
    }

    public sealed class Allocator
    {
        /// <summary>
        /// Maximum size of a memory heap in Vulkan to consider it "small".
        /// </summary>
        private const ulong SmallHeapSize = 1024 * 1024 * 1024;

        /// <summary>
        /// Default size of a block allocated as single VkDeviceMemory from a "large" heap.
        /// </summary>
        private const ulong DefaultLargeHeapBlockSize = 256 * 1024 * 1024;

        ///Minimum value for VkPhysicalDeviceLimits::bufferImageGranularity. Set to more than 1 for debugging purposes only. Must be power of two.
        private const uint DebugMinBufferImageGranularity = 1u;

        private readonly VkPhysicalDeviceProperties _physicalDeviceProperties;
        private readonly VkPhysicalDeviceMemoryProperties _memProps;
        private readonly ulong _preferredLargeHeapBlockSize;

        // Default pools.
        private readonly BlockVector[] _blockVectors = new BlockVector[VK_MAX_MEMORY_TYPES];

        public Allocator(in AllocatorCreateInfo createInfo)
        {
            PhysicalDevice = createInfo.PhysicalDevice;

            vkGetPhysicalDeviceProperties(PhysicalDevice, out _physicalDeviceProperties);
            vkGetPhysicalDeviceMemoryProperties(PhysicalDevice, out _memProps);

            _preferredLargeHeapBlockSize = (createInfo.PreferredLargeHeapBlockSize != 0) ? createInfo.PreferredLargeHeapBlockSize : DefaultLargeHeapBlockSize;

            for (uint memTypeIndex = 0; memTypeIndex < MemoryTypeCount; ++memTypeIndex)
            {
                ulong preferredBlockSize = CalcPreferredBlockSize(memTypeIndex);

                _blockVectors[memTypeIndex] = new BlockVector(this, null, memTypeIndex, preferredBlockSize);
            }
        }

        public VkPhysicalDevice PhysicalDevice { get; }

        public ulong BufferImageGranularity => Math.Max(DebugMinBufferImageGranularity, _physicalDeviceProperties.limits.bufferImageGranularity);

        public uint MemoryHeapCount => _memProps.memoryHeapCount;
        public uint MemoryTypeCount => _memProps.memoryTypeCount;

        private ulong CalcPreferredBlockSize(uint memTypeIndex)
        {
            uint heapIndex = MemoryTypeIndexToHeapIndex(memTypeIndex);
            ulong heapSize = _memProps.GetMemoryHeap(heapIndex).size;
            bool isSmallHeap = heapSize <= SmallHeapSize;

            return Utils.AlignUp(isSmallHeap ? (heapSize / 8) : _preferredLargeHeapBlockSize, 32);
        }

        private uint MemoryTypeIndexToHeapIndex(uint memTypeIndex)
        {
            Debug.Assert(memTypeIndex < _memProps.memoryTypeCount);

            return _memProps.GetMemoryType(memTypeIndex).heapIndex;
        }
    }

    public sealed class Pool
    {
    }

    public sealed class BlockVector
    {
        public BlockVector(Allocator allocator, Pool? parentPool,
            uint memoryTypeIndex,
            ulong preferredBlockSize)
        {
            Allocator = allocator;
            ParentPool = parentPool;
            MemoryTypeIndex = memoryTypeIndex;
            PreferredBlockSize = preferredBlockSize;
        }

        public Allocator Allocator { get; }
        public Pool? ParentPool { get; }
        public uint MemoryTypeIndex { get; }
        public ulong PreferredBlockSize { get; }
    }

    internal static class Utils
    {
        public static bool IsPow2(uint x) => (x & (x - 1)) == 0;
        public static bool IsPow2(ulong x) => (x & (x - 1)) == 0;

        public static ulong AlignUp(ulong val, ulong alignment)
        {
            Debug.Assert(IsPow2(alignment));

            return (val + alignment - 1) & ~(alignment - 1);
        }
    }
}
