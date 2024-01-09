// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Vortice.Vulkan.Interop;

namespace Vortice.Vulkan;

public unsafe readonly struct VkStringArray : IDisposable
{
    private readonly VkString[] _data;

    public VkStringArray(IReadOnlyList<string> array)
    {
        Length = (uint)array.Count;
        Pointer = NativeMemory.Alloc((nuint)(sizeof(nint) * Length));
        _data = new VkString[Length];

        for (int i = 0; i < array.Count; i++)
        {
            _data[i] = array[i];
            ((sbyte**)Pointer)[i] = _data[i].Pointer;
        }
    }

    public VkStringArray(string[] array)
    {
        Length = (uint)array.Length;
        Pointer = NativeMemory.Alloc((nuint)(sizeof(nint) * Length));
        _data = new VkString[Length];

        for (int i = 0; i < array.Length; i++)
        {
            _data[i] = array[i];
            ((sbyte**)Pointer)[i] = _data[i].Pointer;
        }
    }

    public readonly uint Length;
    public readonly void* Pointer;

    public void Dispose()
    {
        NativeMemory.Free(Pointer);
    }

    public VkString this[int index]
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
            ((sbyte**)Pointer)[index] = value;
        }
    }

    public static implicit operator sbyte**(VkStringArray arr) => (sbyte**)arr.Pointer;
}
