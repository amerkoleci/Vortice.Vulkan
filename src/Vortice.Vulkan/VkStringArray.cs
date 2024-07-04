// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

public unsafe readonly struct VkStringArray : IDisposable
{
    private readonly byte** _data;
    public readonly uint Length;

    public VkStringArray(IList<string> strings)
    {
        Length = (uint)strings.Count;
        _data = (byte**)NativeMemory.Alloc((nuint)(strings.Count * sizeof(byte*)));

        for (int i = 0; i < Length; i++)
        {
            ReadOnlySpan<char> source = strings[i];
            int maxLength = Encoding.UTF8.GetMaxByteCount(source.Length);
            Span<byte> bytes = new byte[maxLength + 1];

            int length = Encoding.UTF8.GetBytes(source, bytes);

            uint size = (uint)(bytes.Length + 1) * sizeof(byte);
            _data[i] = (byte*)NativeMemory.Alloc(size);

            fixed (byte* pBytes = bytes)
            {
                NativeMemory.Copy(pBytes, _data[i], size);
            }
        }
    }

    public VkStringArray(IList<VkUtf8String> strings)
    {
        Length = (uint)strings.Count;
        _data = (byte**)NativeMemory.Alloc((nuint)(strings.Count * sizeof(byte*)));

        for (int i = 0; i < Length; i++)
        {
            ReadOnlySpan<byte> bytes = strings[i].Span;

            uint size = (uint)(bytes.Length + 1) * sizeof(byte);
            _data[i] = (byte*)NativeMemory.Alloc(size);

            fixed (byte* pBytes = bytes)
            {
                NativeMemory.Copy(pBytes, _data[i], size);
            }
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_data[i]);

        NativeMemory.Free(_data);
    }

    public override string ToString()
    {
        StringBuilder builder = new("[");

        for (int i = 0; i < Length; i++)
        {
            builder.Append(new string((sbyte*)_data[i]));

            if (i < Length - 1)
                builder.Append(", ");
        }

        builder.Append(']');

        return builder.ToString();
    }

    public static implicit operator byte**(VkStringArray pStringArray)
        => pStringArray._data;
}
