// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#if TODO
using System.Runtime.InteropServices;
using static Vulkan;

public struct PhysicalDeviceMemoryProperties
{
    /// <summary>
    /// Structures describing the memory types that can be used to access memory allocated from
    /// the heaps specified by <see cref="MemoryHeaps"/>.
    /// </summary>
    public VkMemoryType[] MemoryTypes;

    /// <summary>
    /// Structures describing the memory heaps from which memory can be allocated.
    /// </summary>
    public VkMemoryHeap[] MemoryHeaps;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Native
    {
        public int MemoryTypeCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxMemoryTypes)]
        public VkMemoryType[] MemoryTypes;

        public int MemoryHeapCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxMemoryHeaps)]
        public VkMemoryHeap[] MemoryHeaps;
    }

    internal static void FromNative(ref VkPhysicalDeviceMemoryProperties native, out PhysicalDeviceMemoryProperties managed)
    {
        uint memoryTypeCount = native.memoryTypeCount;
        managed.MemoryTypes = new VkMemoryType[memoryTypeCount];
        for (uint i = 0; i < memoryTypeCount; i++)
        {
            //managed.MemoryTypes[i] = native.memoryTypes[i];
        }

        uint memoryHeapCount = native.memoryHeapCount;
        managed.MemoryHeaps = new VkMemoryHeap[memoryHeapCount];
        for (uint i = 0; i < memoryHeapCount; i++)
        {
            //managed.MemoryHeaps[i] = native.MemoryHeaps[i];
        }
    }
} 
#endif
