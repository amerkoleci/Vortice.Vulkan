// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Vortice.Vulkan.Interop;

namespace Vortice.Vulkan;

public unsafe readonly struct VkString : IDisposable
{
    public readonly sbyte* Pointer;

    /// <summary>
    /// Size of the byte array that is created from the string.
    /// This value is bigger then the length of the string because '\0' is added at the end of the string and because some UTF8 characters can be longer then 1 byte.
    /// When Size is 0, then this represents a null string or disposed VkString.
    /// </summary>
    public int Size { get; }

    public VkString(string? str)
    {
        if (str == null)
            return; // Preserve Size as 0

        ReadOnlySpan<sbyte> strSpan = str.GetUtf8Span();
        int strLength = strSpan.Length + 1;
        Pointer = (sbyte*)NativeMemory.Alloc((nuint)strLength);

        var destination = new Span<sbyte>(Pointer, strLength);
        strSpan.CopyTo(destination);
        destination[strSpan.Length] = 0x00;

        // note that empty string will have Size set to 1 because of added '\0'
        Size = strLength;
    }

    public void Dispose()
    {
        if (Size == 0)
            return;

        NativeMemory.Free(Pointer);
    }

    public static unsafe implicit operator sbyte*(VkString value) => value.Pointer;
    public static unsafe implicit operator IntPtr(VkString value) => new(value.Pointer);
    public static implicit operator VkString(string str) => new(str);
    public static implicit operator string?(VkString str) => GetUtf8Span(str).GetString();
}
