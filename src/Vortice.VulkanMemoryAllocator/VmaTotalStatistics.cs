// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public struct VmaTotalStatistics
{
    public memoryTypes__FixedBuffer memoryType;

    public memoryHeaps__FixedBuffer memoryHeap;

    /// <summary>
    /// Largest empty range size. 0 if there are 0 empty ranges.
    /// </summary>
    public VmaDetailedStatistics total;

    public struct memoryTypes__FixedBuffer
    {
        public VmaDetailedStatistics e0;
        public VmaDetailedStatistics e1;
        public VmaDetailedStatistics e2;
        public VmaDetailedStatistics e3;
        public VmaDetailedStatistics e4;
        public VmaDetailedStatistics e5;
        public VmaDetailedStatistics e6;
        public VmaDetailedStatistics e7;
        public VmaDetailedStatistics e8;
        public VmaDetailedStatistics e9;
        public VmaDetailedStatistics e10;
        public VmaDetailedStatistics e11;
        public VmaDetailedStatistics e12;
        public VmaDetailedStatistics e13;
        public VmaDetailedStatistics e14;
        public VmaDetailedStatistics e15;
        public VmaDetailedStatistics e16;
        public VmaDetailedStatistics e17;
        public VmaDetailedStatistics e18;
        public VmaDetailedStatistics e19;
        public VmaDetailedStatistics e20;
        public VmaDetailedStatistics e21;
        public VmaDetailedStatistics e22;
        public VmaDetailedStatistics e23;
        public VmaDetailedStatistics e24;
        public VmaDetailedStatistics e25;
        public VmaDetailedStatistics e26;
        public VmaDetailedStatistics e27;
        public VmaDetailedStatistics e28;
        public VmaDetailedStatistics e29;
        public VmaDetailedStatistics e30;
        public VmaDetailedStatistics e31;

        [UnscopedRef]
        public ref VmaDetailedStatistics this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref AsSpan()[index];
            }
        }

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<VmaDetailedStatistics> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref e0, (int)VK_MAX_MEMORY_TYPES);
        }
    }

    public struct memoryHeaps__FixedBuffer
    {
        public VmaDetailedStatistics e0;
        public VmaDetailedStatistics e1;
        public VmaDetailedStatistics e2;
        public VmaDetailedStatistics e3;
        public VmaDetailedStatistics e4;
        public VmaDetailedStatistics e5;
        public VmaDetailedStatistics e6;
        public VmaDetailedStatistics e7;
        public VmaDetailedStatistics e8;
        public VmaDetailedStatistics e9;
        public VmaDetailedStatistics e10;
        public VmaDetailedStatistics e11;
        public VmaDetailedStatistics e12;
        public VmaDetailedStatistics e13;
        public VmaDetailedStatistics e14;
        public VmaDetailedStatistics e15;

        [UnscopedRef]
        public ref VmaDetailedStatistics this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref AsSpan()[index];
            }
        }

        [UnscopedRef]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<VmaDetailedStatistics> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref e0, (int)VK_MAX_MEMORY_HEAPS);
        }
    }
}
