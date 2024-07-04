// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

/// <summary>
/// Represents a null terminated UTF8 encoded text buffer.
/// </summary>
public readonly unsafe struct VkUtf8String : IEquatable<VkUtf8String>
{
    /// <summary>
    /// Initializes a new instance of <see cref="VkUtf8String"/> with a null-terminated UTF8 string.
    /// </summary>
    /// <param name="buffer">A null terminated UTF-8 string.</param>
    public VkUtf8String(byte* buffer)
        : this(buffer, buffer == null ? 0 : new ReadOnlySpan<byte>(buffer, int.MaxValue).IndexOf((byte)0))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VkUtf8String"/> with a null-terminated UTF-8 string.
    /// </summary>
    /// <param name="buffer">A null terminated UTF8 string.</param>
    /// <param name="length">The lenght of UTF8 string</param>
    public VkUtf8String(byte* buffer, int length)
    {
        Buffer = buffer;
        Length = length;
    }

    /// <summary>
    /// Gets the pointer to the buffer.
    /// </summary>
    public readonly byte* Buffer { get; }

    /// <summary>
    /// Gets the number of bytes in the current <see cref="VkUtf8String"/>.
    /// </summary>
    public readonly int Length { get; }

    /// <summary>
    /// Gets whether this string is null.
    /// </summary>
    public bool IsNull => Buffer == null;

    /// <summary>
    /// Gets the <see cref="VkUtf8String"/> as a span of bytes.
    /// </summary>
    public ReadOnlySpan<byte> Span => new(Buffer, Length);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.AddBytes(Span);
        return hash.ToHashCode();
    }

    /// <inheritdoc />
    public override string? ToString() => IsNull ? null : Encoding.UTF8.GetString(Span);

    public static implicit operator VkUtf8String(ReadOnlySpan<byte> span) => new((byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)), span.Length);

    /// <summary>
    /// Converts a <see cref="VkUtf8String"/> to a <see cref="VkUtf8ReadOnlyString"/>.
    /// </summary>
    public static implicit operator VkUtf8ReadOnlyString(VkUtf8String memory) => memory.Span;

    /// <summary>
    /// Converts a <see cref="VkUtf8String"/> to a <see cref="ReadOnlySpan{byte}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan<byte>(VkUtf8String memory) => memory.Span;

    public static implicit operator byte*(VkUtf8String memory) => memory.Buffer;

    public bool Equals(VkUtf8String other)
    {
        return Span.SequenceEqual(other.Span);
    }

    public override bool Equals(object? obj)
    {
        return obj is VkUtf8String other && Equals(other);
    }

    public static bool operator ==(VkUtf8String left, VkUtf8String right) => left.Equals(right);

    public static bool operator !=(VkUtf8String left, VkUtf8String right) => !left.Equals(right);

    public static bool operator ==(VkUtf8String left, ReadOnlySpan<byte> right) => left.Span.SequenceEqual(right);
    public static bool operator !=(VkUtf8String left, ReadOnlySpan<byte> right) => !left.Equals(right);

}
