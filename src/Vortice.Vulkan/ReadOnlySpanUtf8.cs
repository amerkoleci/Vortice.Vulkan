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
/// <param name="span">The span of UTF-8 bytes.</param>
/// <remarks>This class should be used only with UTF-8 string literals.</remarks>
public readonly ref struct ReadOnlySpanUtf8(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> _span = span;

    /// <summary>
    /// Gets the number of bytes in the current <see cref="ReadOnlySpanUtf8"/>.
    /// </summary>
    public int Length => _span.Length;

    /// <summary>
    /// Gets the <see cref="ReadOnlySpanUtf8"/> as a span of bytes.
    /// </summary>
    public ReadOnlySpan<byte> Bytes => _span;

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.AddBytes(_span);
        return hash.ToHashCode();
    }

    /// <inheritdoc />
    public override string? ToString() => Unsafe.IsNullRef(ref MemoryMarshal.GetReference(_span)) ? null : Encoding.UTF8.GetString(_span);

    /// <summary>
    /// Converts a <see cref="ReadOnlySpanUtf8"/> to a <see cref="string"/>.
    /// </summary>
    public static implicit operator ReadOnlySpanUtf8(ReadOnlySpan<byte> span) => new(span);

    /// <summary>
    /// Converts a <see cref="ReadOnlySpanUtf8"/> to a <see cref="ReadOnlySpan{byte}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan<byte>(ReadOnlySpanUtf8 span) => span._span;

    /// <summary>
    /// Casts a <see cref="ReadOnlySpanUtf8"/> to a byte pointer.
    /// </summary>
    /// <param name="span">The UTF8 span.</param>
    /// <remarks>
    /// In order to safely use this operator, the Span must have been created from a string literal or a pinned array otherwise unexpected pointers will be returned.
    /// </remarks>
    public static unsafe implicit operator byte*(ReadOnlySpanUtf8 span) => (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span._span));

    public static bool operator ==(ReadOnlySpanUtf8 left, ReadOnlySpanUtf8 right) => left._span.SequenceEqual(right._span);

    public static bool operator !=(ReadOnlySpanUtf8 left, ReadOnlySpanUtf8 right) => !left._span.SequenceEqual(right._span);
}
