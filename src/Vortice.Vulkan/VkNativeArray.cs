// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

public unsafe struct NativeArray<T> where T : unmanaged
{
    public const int CapacityInBytes = 256;
    private static readonly int s_sizeofT = sizeof(T);

    private fixed byte _storage[CapacityInBytes];
    private uint _count;

    public uint Count => _count;
    public T* Data => (T*)Unsafe.AsPointer(ref this);

    public void Add(T item)
    {
        byte* basePtr = (byte*)Data;
        int offset = (int)(_count * s_sizeofT);
#if DEBUG
        Debug.Assert((offset + s_sizeofT) <= CapacityInBytes);
#endif
        Unsafe.Write(basePtr + offset, item);

        _count += 1;
    }

    public ref T this[uint index]
    {
        get
        {
            byte* basePtr = (byte*)Unsafe.AsPointer(ref this);
            int offset = (int)(index * s_sizeofT);
            return ref Unsafe.AsRef<T>(basePtr + offset);
        }
    }

    public ref T this[int index]
    {
        get
        {
            byte* basePtr = (byte*)Unsafe.AsPointer(ref this);
            int offset = index * s_sizeofT;
            return ref Unsafe.AsRef<T>(basePtr + offset);
        }
    }
}
