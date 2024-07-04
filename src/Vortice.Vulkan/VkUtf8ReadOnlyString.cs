// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

public readonly unsafe ref struct VkUtf8ReadOnlyString
{
    private readonly ReadOnlySpan<byte> _data;

    public VkUtf8ReadOnlyString(ReadOnlySpan<byte> data)
    {
        _data = data;
    }

    public VkUtf8ReadOnlyString(byte[] data)
    {
        _data = data.AsSpan();
    }

    /// <inheritdoc />
    public override string? ToString() => Unsafe.IsNullRef(ref MemoryMarshal.GetReference(_data)) ? null : Encoding.UTF8.GetString(_data);

    public static implicit operator VkUtf8ReadOnlyString(ReadOnlySpan<byte> span) => new(span);
    public static implicit operator VkUtf8ReadOnlyString(byte[] data) => new(data);

    public static implicit operator ReadOnlySpan<byte>(VkUtf8ReadOnlyString span) => span._data;

    public static unsafe implicit operator byte*(VkUtf8ReadOnlyString span) => (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span._data));

    internal ref readonly byte GetPinnableReference() => ref _data.GetPinnableReference();

    public static bool operator ==(VkUtf8ReadOnlyString left, VkUtf8ReadOnlyString right) => left._data.SequenceEqual(right._data);

    public static bool operator !=(VkUtf8ReadOnlyString left, VkUtf8ReadOnlyString right) => !left._data.SequenceEqual(right._data);

}
