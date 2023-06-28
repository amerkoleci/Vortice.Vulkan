// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

public unsafe static class Interop
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Allocate(nuint count)
    {
        void* result = NativeMemory.Alloc(count);

        if (result == null)
        {
            ThrowOutOfMemoryException(count);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AllocateArray<T>(uint count)
        where T : unmanaged
    {
        T* result = (T*)NativeMemory.Alloc(count, (nuint)sizeof(T));

        if (result == null)
        {
            ThrowOutOfMemoryException(count, SizeOf<T>());
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AllocateArray<T>(nuint count)
        where T : unmanaged
    {
        T* result = (T*)NativeMemory.Alloc(count, (nuint)sizeof(T));

        if (result == null)
        {
            ThrowOutOfMemoryException(count, SizeOf<T>());
        }

        return result;
    }

    /// <summary>
    /// Allocates unmanaged memory and copies the specified structures over.
    /// <para>If the array is <c>null</c> or empty, returns <see cref="IntPtr.Zero"/>.</para>
    /// </summary>
    /// <typeparam name="T">Type of elements to copy.</typeparam>
    /// <param name="values">The values to copy.</param>
    /// <returns>
    /// A pointer to the newly allocated memory. This memory must be released using the <see
    /// cref="NativeMemory.Free(void*)"/> method.
    /// </returns>
    public static T* AllocToPointer<T>(T[] values)
        where T : unmanaged
    {
        if (values == null || values.Length == 0)
            return null;

        int structSize = sizeof(T);
        T* ptr = AllocateArray<T>((nuint)values.Length);

        byte* walk = (byte*)ptr;
        for (int i = 0; i < values.Length; i++)
        {
            Unsafe.Copy(walk, ref values[i]);
            walk += structSize;
        }

        return ptr;
    }

    /// <summary>
    /// Allocates unmanaged memory and copies the specified structure over.
    /// <para>If the value is <c>null</c>, returns <see cref="IntPtr.Zero"/>.</para>
    /// </summary>
    /// <typeparam name="T">Type of structure to copy.</typeparam>
    /// <param name="value">The value to copy.</param>
    /// <returns>
    /// A pointer to the newly allocated memory. This memory must be released using the <see
    /// cref="NativeMemory.Free(void*)"/> method.
    /// </returns>
    public static T* AllocToPointer<T>(ref T? value)
        where T : unmanaged
    {
        if (!value.HasValue)
            return null;

        void* ptr = NativeMemory.Alloc((nuint)sizeof(T));
        Unsafe.Write(ptr, value.Value);
        return (T*)ptr;
    }

    /// <summary>
    /// Encodes a string as null-terminated UTF-8 bytes and allocates to unmanaged memory.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>
    /// A pointer to the newly allocated memory. This memory must be released using the <see
    /// cref="NativeMemory.Free(void*)"/> method.
    /// </returns>
    public static sbyte* AllocToPointer(string? value)
    {
        if (value == null)
            return null;

        // Get max number of bytes the string may need.
        int maxSize = Encoding.UTF8.GetMaxByteCount(value.Length + 1); // +1 for null-terminator;
        // Allocate unmanaged memory.
        sbyte* ptr = (sbyte*)Allocate((nuint)maxSize);
        // Encode to utf-8, null-terminate and write to unmanaged memory.
        int actualNumberOfBytesWritten;
        fixed (char* ch = value)
        {
            actualNumberOfBytesWritten = Encoding.UTF8.GetBytes(ch, value.Length, (byte*)ptr, maxSize);
        }

        ptr[actualNumberOfBytesWritten] = 0;
        // Return pointer to the beginning of unmanaged memory.
        return ptr;
    }

    public static sbyte** AllocToPointers(string[] values)
    {
        if (values == null || values.Length == 0)
            return null;

        // Allocate unmanaged memory for string pointers.
        sbyte** stringHandlesPtr = (sbyte**)Allocate((nuint)(sizeof(nint) * values.Length));

        // Store the pointer to the string.
        for (int i = 0; i < values.Length; i++)
        {
            stringHandlesPtr[i] = AllocToPointer(values[i]);
        }

        return stringHandlesPtr;
    }

    public static sbyte** AllocToPointers(IList<string> values)
    {
        if (values == null || values.Count == 0)
            return null;

        // Allocate unmanaged memory for string pointers.
        sbyte** stringHandlesPtr = (sbyte**)Allocate((nuint)(sizeof(nint) * values.Count));

        // Store the pointer to the string.
        for (int i = 0; i < values.Count; i++)
        {
            stringHandlesPtr[i] = AllocToPointer(values[i]);
        }

        return stringHandlesPtr;
    }

    [DoesNotReturn]
    public static void ThrowOutOfMemoryException(ulong size)
    {
        throw new OutOfMemoryException($"The allocation of '{size}' bytes failed");
    }

    [DoesNotReturn]
    public static void ThrowOutOfMemoryException(ulong count, ulong size)
    {
        throw new OutOfMemoryException($"The allocation of '{count}x{size}' bytes failed");
    }

    /// <inheritdoc cref="Unsafe.SizeOf{T}" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SizeOf<T>() => unchecked((uint)Unsafe.SizeOf<T>());

    /// <inheritdoc cref="Unsafe.AsPointer{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(ref T source) where T : unmanaged => (T*)Unsafe.AsPointer(ref source);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TTo AsReadOnly<TFrom, TTo>(in TFrom source) => ref Unsafe.As<TFrom, TTo>(ref AsRef(in source));

    /// <summary>Reinterprets the given native integer as a reference.</summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    /// <param name="source">The native integer to reinterpret.</param>
    /// <returns>A reference to a value of type <typeparamref name="T" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(nint source) => ref Unsafe.AsRef<T>((void*)source);

    /// <summary>Reinterprets the given native unsigned integer as a reference.</summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    /// <param name="source">The native unsigned integer to reinterpret.</param>
    /// <returns>A reference to a value of type <typeparamref name="T" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(nuint source) => ref Unsafe.AsRef<T>((void*)source);

    /// <inheritdoc cref="Unsafe.AsRef{T}(in T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(in T source) => ref Unsafe.AsRef(in source);

    /// <inheritdoc cref="MemoryMarshal.CreateReadOnlySpan{T}(ref T, int)" />
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped in T reference, int length) => MemoryMarshal.CreateReadOnlySpan(ref AsRef(in reference), length);

    // <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointer<T>(this Span<T> span)
        where T : unmanaged => AsPointer(ref span.GetReference());

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointer<T>(this ReadOnlySpan<T> span)
        where T : unmanaged => AsPointer(ref AsRef(in span.GetReference()));

    /// <inheritdoc cref="MemoryMarshal.GetReference{T}(Span{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetReference<T>(this Span<T> span) => ref MemoryMarshal.GetReference(span);

    /// <inheritdoc cref="MemoryMarshal.GetReference{T}(ReadOnlySpan{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T GetReference<T>(this ReadOnlySpan<T> span) => ref MemoryMarshal.GetReference(span);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TTo As<TFrom, TTo>(ref TFrom source)
        => ref Unsafe.As<TFrom, TTo>(ref source);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TTo> As<TFrom, TTo>(this ReadOnlySpan<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        return CreateReadOnlySpan(in AsReadOnly<TFrom, TTo>(in span.GetReference()), span.Length);
    }

    /// <inheritdoc cref="Unsafe.IsNullRef{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(in T source) => Unsafe.IsNullRef(ref AsRef(in source));

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<sbyte> span)
        => span.GetPointer() != null ? Encoding.UTF8.GetString(span.As<sbyte, byte>()) : null;

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<ushort> span)
    {
        return span.GetPointer() != null ? new string(span.As<ushort, char>()) : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<sbyte> GetUtf8Span(this string? source)
    {
        ReadOnlySpan<byte> result;

        if (source is not null)
        {
            var maxLength = Encoding.UTF8.GetMaxByteCount(source.Length);
            var bytes = new byte[maxLength + 1];

            var length = Encoding.UTF8.GetBytes(source, bytes);
            result = bytes.AsSpan(0, length);
        }
        else
        {
            result = null;
        }

        return result.As<byte, sbyte>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<sbyte> GetUtf8Span(sbyte* source, int maxLength = -1)
        => (source != null) ? GetUtf8Span(in source[0], maxLength) : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<sbyte> GetUtf8Span(in sbyte source, int maxLength = -1)
    {
        ReadOnlySpan<sbyte> result;

        if (!IsNullRef(in source))
        {
            if (maxLength < 0)
            {
                maxLength = int.MaxValue;
            }

            result = CreateReadOnlySpan(in source, maxLength);
            var length = result.IndexOf((sbyte)'\0');

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
}
