// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

public static unsafe class Interop
{
    /// <inheritdoc cref="Unsafe.AsPointer{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(ref T source) where T : unmanaged => (T*)Unsafe.AsPointer(ref source);

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointer<T>(this ReadOnlySpan<T> span)
        where T : unmanaged => AsPointer(ref Unsafe.AsRef(in span.GetReference()));

    /// <inheritdoc cref="MemoryMarshal.GetReference{T}(ReadOnlySpan{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T GetReference<T>(this ReadOnlySpan<T> span) => ref MemoryMarshal.GetReference(span);

    /// <summary>Converts an unmanaged string to a managed version.</summary>
    /// <param name="unmanaged">The unmanaged string to convert.</param>
    /// <param name="maxLength"></param>
    /// <returns>A managed string.</returns>
    public static string? GetString(byte* unmanaged, int maxLength = -1) => GetUtf8Span(unmanaged, maxLength).GetString();

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<byte> span)
        => span.GetPointer() != null ? UTF8EncodingRelaxed.Default.GetString(span) : null;

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<char> span)
    {
        return span.GetPointer() != null ? new string(span) : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> GetUtf8Span(this string? source)
    {
        ReadOnlySpan<byte> result;

        if (source is not null)
        {
            int maxLength = UTF8EncodingRelaxed.Default.GetMaxByteCount(source.Length);
            Span<byte> bytes = new byte[maxLength + 1];

            int length = UTF8EncodingRelaxed.Default.GetBytes(source, bytes);
            result = bytes.Slice(0, length);
        }
        else
        {
            result = null;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> GetUtf8Span(byte* source, int maxLength = -1)
        => (source != null) ? GetUtf8Span(in source[0], maxLength) : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> GetUtf8Span(in byte source, int maxLength = -1)
    {
        ReadOnlySpan<byte> result;

        if (!Unsafe.IsNullRef(in source))
        {
            if (maxLength < 0)
            {
                maxLength = int.MaxValue;
            }

            result = MemoryMarshal.CreateReadOnlySpan(in source, maxLength);
            int length = result.IndexOf((byte)'\0');

            if (length != -1)
            {
                result = result.Slice(0, length);
            }
        }
        else
        {
            result = null;
        }

        return result;
    }

    sealed class UTF8EncodingRelaxed : UTF8Encoding
    {
        public static new readonly UTF8EncodingRelaxed Default = new();

        private UTF8EncodingRelaxed() : base(false, false)
        {
        }
    }
}
