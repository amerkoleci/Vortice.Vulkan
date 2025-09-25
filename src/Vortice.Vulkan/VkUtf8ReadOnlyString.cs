// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS0660
#pragma warning disable CS0661

namespace Vortice.Vulkan;

public readonly ref struct VkUtf8ReadOnlyString(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> _span = span;

    /// <inheritdoc />
    public override string? ToString() => Unsafe.IsNullRef(ref MemoryMarshal.GetReference(_span)) ? null : Encoding.UTF8.GetString(_span);

    public static implicit operator VkUtf8ReadOnlyString(ReadOnlySpan<byte> span) => new(span);
    public static implicit operator VkUtf8ReadOnlyString(byte[] data) => new(data);

    public static implicit operator ReadOnlySpan<byte>(VkUtf8ReadOnlyString span) => span._span;

    public static unsafe implicit operator byte*(VkUtf8ReadOnlyString span) => (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span._span));

    internal ref readonly byte GetPinnableReference() => ref _span.GetPinnableReference();

    public static bool operator ==(VkUtf8ReadOnlyString left, VkUtf8ReadOnlyString right) => left._span.SequenceEqual(right._span);

    public static bool operator !=(VkUtf8ReadOnlyString left, VkUtf8ReadOnlyString right) => !left._span.SequenceEqual(right._span);

}
