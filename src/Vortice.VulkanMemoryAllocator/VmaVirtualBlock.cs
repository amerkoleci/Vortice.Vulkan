// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Vortice.Vulkan;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaVirtualBlock : IEquatable<VmaVirtualBlock>
{
    public VmaVirtualBlock(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static VmaVirtualBlock Null => new(0);
    public static implicit operator VmaVirtualBlock(nint handle) => new(handle);
    public static bool operator ==(VmaVirtualBlock left, VmaVirtualBlock right) => left.Handle == right.Handle;
    public static bool operator !=(VmaVirtualBlock left, VmaVirtualBlock right) => left.Handle != right.Handle;
    public static bool operator ==(VmaVirtualBlock left, nint right) => left.Handle == right;
    public static bool operator !=(VmaVirtualBlock left, nint right) => left.Handle != right;
    public bool Equals(VmaVirtualBlock other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmaVirtualBlock handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(VmaVirtualBlock)} [0x{Handle.ToString("X")}]";
}
