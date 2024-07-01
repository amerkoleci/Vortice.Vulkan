// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

/// <summary>
/// Represents a null terminated read-only span of UTF-8 encoded text.
/// </summary>
/// <param name="buffer">The span of UTF-8 bytes.</param>
/// <param name="length"></param>
/// <remarks>This class should be used only with UTF-8 string literals.</remarks>
public readonly unsafe struct ReadOnlyMemoryUtf8(byte* buffer, int length) : IEquatable<ReadOnlyMemoryUtf8>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ReadOnlyMemoryUtf8"/> with a null-terminated UTF-8 string.
    /// </summary>
    /// <param name="buffer">A null terminated UTF-8 string.</param>
    public ReadOnlyMemoryUtf8(byte* buffer) : this(buffer, buffer == null ? 0 : new ReadOnlySpan<byte>(buffer, int.MaxValue).IndexOf((byte)0))
    {
    }

    public bool IsNull => Buffer == null;

    /// <summary>
    /// Gets the pointer to the buffer.
    /// </summary>
    public readonly byte* Buffer = buffer;

    /// <summary>
    /// Gets the number of bytes in the current <see cref="ReadOnlySpanUtf8"/>.
    /// </summary>
    public readonly int Length = length;

    /// <summary>
    /// Gets the <see cref="ReadOnlySpanUtf8"/> as a span of bytes.
    /// </summary>
    public ReadOnlySpan<byte> Bytes => new(Buffer, Length);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.AddBytes(Bytes);
        return hash.ToHashCode();
    }

    /// <inheritdoc />
    public override string? ToString() => IsNull ? null : Encoding.UTF8.GetString(Bytes);

    /// <summary>
    /// Converts a <see cref="ReadOnlySpan{byte}"/> to a <see cref="ReadOnlyMemoryUtf8"/>.
    /// </summary>
    public static implicit operator ReadOnlyMemoryUtf8(ReadOnlySpan<byte> span) => new((byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)), span.Length);

    /// <summary>
    /// Converts a <see cref="ReadOnlyMemoryUtf8"/> to a <see cref="ReadOnlySpanUtf8"/>.
    /// </summary>
    public static implicit operator ReadOnlySpanUtf8(ReadOnlyMemoryUtf8 memory) => memory.Bytes;

    /// <summary>
    /// Converts a <see cref="ReadOnlyMemoryUtf8"/> to a <see cref="ReadOnlySpan{byte}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan<byte>(ReadOnlyMemoryUtf8 memory) => memory.Bytes;

    /// <summary>
    /// Casts a <see cref="ReadOnlyMemoryUtf8"/> to a byte pointer.
    /// </summary>
    /// <param name="memory">The UTF8 span.</param>
    /// <remarks>
    /// In order to safely use this operator, this instance must have been created from a string literal or a pinned array otherwise unexpected pointers will be returned.
    /// </remarks>
    public static explicit operator byte*(ReadOnlyMemoryUtf8 memory) => memory.Buffer;

    public bool Equals(ReadOnlyMemoryUtf8 other)
    {
        return Bytes.SequenceEqual(other.Bytes);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ReadOnlyMemoryUtf8 other && Equals(other);
    }

    public static bool operator ==(ReadOnlyMemoryUtf8 left, ReadOnlyMemoryUtf8 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ReadOnlyMemoryUtf8 left, ReadOnlyMemoryUtf8 right)
    {
        return !left.Equals(right);
    }
}
