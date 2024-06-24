// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

public unsafe readonly struct VkStringArray : IDisposable
{
    private readonly ReadOnlyMemoryUtf8[] _data;

    public VkStringArray(IReadOnlyList<string> array)
    {
        Length = (uint)array.Count;
        Pointer = NativeMemory.Alloc((nuint)(sizeof(nint) * Length));
        _data = new ReadOnlyMemoryUtf8[Length];

        for (int i = 0; i < array.Count; i++)
        {
            _data[i] = Interop.GetUtf8Span(array[i]);
            ((byte**)Pointer)[i] = _data[i].Buffer;
        }
    }

    public readonly uint Length;
    public readonly void* Pointer;

    public void Dispose()
    {
        NativeMemory.Free(Pointer);
    }

    public ReadOnlyMemoryUtf8 this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            return _data[index];
        }
        set
        {
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            _data[index] = value;
            ((byte**)Pointer)[index] = value.Buffer;
        }
    }

    public static implicit operator byte**(VkStringArray arr) => (byte**)arr.Pointer;
}
