// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Vortice.Vulkan;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaVirtualAllocation : IEquatable<VmaVirtualAllocation>
{
    public VmaVirtualAllocation(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static VmaVirtualAllocation Null => new(0);
    public static implicit operator VmaVirtualAllocation(nint handle) => new(handle);
    public static bool operator ==(VmaVirtualAllocation left, VmaVirtualAllocation right) => left.Handle == right.Handle;
    public static bool operator !=(VmaVirtualAllocation left, VmaVirtualAllocation right) => left.Handle != right.Handle;
    public static bool operator ==(VmaVirtualAllocation left, nint right) => left.Handle == right;
    public static bool operator !=(VmaVirtualAllocation left, nint right) => left.Handle != right;
    public bool Equals(VmaVirtualAllocation other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmaVirtualAllocation handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(VmaVirtualAllocation)} [0x{Handle.ToString("X")}]";
}
