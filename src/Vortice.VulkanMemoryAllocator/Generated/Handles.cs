// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

#nullable enable

using System.Diagnostics;

namespace Vortice.Vulkan;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaAllocator : IEquatable<VmaAllocator>
{
	public VmaAllocator(nint handle) { Handle = handle; }
	public nint Handle { get; }
	public bool IsNull => Handle == 0;
	public bool IsNotNull => Handle != 0;
	public static VmaAllocator Null => new(0);
	public static implicit operator VmaAllocator(nint handle) => new(handle);
	public static implicit operator nint(VmaAllocator handle) => handle.Handle;
	public static bool operator ==(VmaAllocator left, VmaAllocator right) => left.Handle == right.Handle;
	public static bool operator !=(VmaAllocator left, VmaAllocator right) => left.Handle != right.Handle;
	public static bool operator ==(VmaAllocator left, nint right) => left.Handle == right;
	public static bool operator !=(VmaAllocator left, nint right) => left.Handle != right;
	public bool Equals(VmaAllocator other) => Handle == other.Handle;
	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is VmaAllocator handle && Equals(handle);
	/// <inheritdoc/>
	public override int GetHashCode() => Handle.GetHashCode();
	private string DebuggerDisplay => $"{nameof(VmaAllocator)} [0x{Handle.ToString("X")}]";
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaPool : IEquatable<VmaPool>
{
	public VmaPool(nint handle) { Handle = handle; }
	public nint Handle { get; }
	public bool IsNull => Handle == 0;
	public bool IsNotNull => Handle != 0;
	public static VmaPool Null => new(0);
	public static implicit operator VmaPool(nint handle) => new(handle);
	public static implicit operator nint(VmaPool handle) => handle.Handle;
	public static bool operator ==(VmaPool left, VmaPool right) => left.Handle == right.Handle;
	public static bool operator !=(VmaPool left, VmaPool right) => left.Handle != right.Handle;
	public static bool operator ==(VmaPool left, nint right) => left.Handle == right;
	public static bool operator !=(VmaPool left, nint right) => left.Handle != right;
	public bool Equals(VmaPool other) => Handle == other.Handle;
	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is VmaPool handle && Equals(handle);
	/// <inheritdoc/>
	public override int GetHashCode() => Handle.GetHashCode();
	private string DebuggerDisplay => $"{nameof(VmaPool)} [0x{Handle.ToString("X")}]";
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaAllocation : IEquatable<VmaAllocation>
{
	public VmaAllocation(nint handle) { Handle = handle; }
	public nint Handle { get; }
	public bool IsNull => Handle == 0;
	public bool IsNotNull => Handle != 0;
	public static VmaAllocation Null => new(0);
	public static implicit operator VmaAllocation(nint handle) => new(handle);
	public static implicit operator nint(VmaAllocation handle) => handle.Handle;
	public static bool operator ==(VmaAllocation left, VmaAllocation right) => left.Handle == right.Handle;
	public static bool operator !=(VmaAllocation left, VmaAllocation right) => left.Handle != right.Handle;
	public static bool operator ==(VmaAllocation left, nint right) => left.Handle == right;
	public static bool operator !=(VmaAllocation left, nint right) => left.Handle != right;
	public bool Equals(VmaAllocation other) => Handle == other.Handle;
	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is VmaAllocation handle && Equals(handle);
	/// <inheritdoc/>
	public override int GetHashCode() => Handle.GetHashCode();
	private string DebuggerDisplay => $"{nameof(VmaAllocation)} [0x{Handle.ToString("X")}]";
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaDefragmentationContext : IEquatable<VmaDefragmentationContext>
{
	public VmaDefragmentationContext(nint handle) { Handle = handle; }
	public nint Handle { get; }
	public bool IsNull => Handle == 0;
	public bool IsNotNull => Handle != 0;
	public static VmaDefragmentationContext Null => new(0);
	public static implicit operator VmaDefragmentationContext(nint handle) => new(handle);
	public static implicit operator nint(VmaDefragmentationContext handle) => handle.Handle;
	public static bool operator ==(VmaDefragmentationContext left, VmaDefragmentationContext right) => left.Handle == right.Handle;
	public static bool operator !=(VmaDefragmentationContext left, VmaDefragmentationContext right) => left.Handle != right.Handle;
	public static bool operator ==(VmaDefragmentationContext left, nint right) => left.Handle == right;
	public static bool operator !=(VmaDefragmentationContext left, nint right) => left.Handle != right;
	public bool Equals(VmaDefragmentationContext other) => Handle == other.Handle;
	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is VmaDefragmentationContext handle && Equals(handle);
	/// <inheritdoc/>
	public override int GetHashCode() => Handle.GetHashCode();
	private string DebuggerDisplay => $"{nameof(VmaDefragmentationContext)} [0x{Handle.ToString("X")}]";
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaVirtualAllocation : IEquatable<VmaVirtualAllocation>
{
	public VmaVirtualAllocation(ulong handle) { Handle = handle; }
	public ulong Handle { get; }
	public bool IsNull => Handle == 0;
	public bool IsNotNull => Handle != 0;
	public static VmaVirtualAllocation Null => new(0);
	public static implicit operator VmaVirtualAllocation(ulong handle) => new(handle);
	public static implicit operator ulong(VmaVirtualAllocation handle) => handle.Handle;
	public static bool operator ==(VmaVirtualAllocation left, VmaVirtualAllocation right) => left.Handle == right.Handle;
	public static bool operator !=(VmaVirtualAllocation left, VmaVirtualAllocation right) => left.Handle != right.Handle;
	public static bool operator ==(VmaVirtualAllocation left, ulong right) => left.Handle == right;
	public static bool operator !=(VmaVirtualAllocation left, ulong right) => left.Handle != right;
	public bool Equals(VmaVirtualAllocation other) => Handle == other.Handle;
	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is VmaVirtualAllocation handle && Equals(handle);
	/// <inheritdoc/>
	public override int GetHashCode() => Handle.GetHashCode();
	private string DebuggerDisplay => $"{nameof(VmaVirtualAllocation)} [0x{Handle.ToString("X")}]";
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaVirtualBlock : IEquatable<VmaVirtualBlock>
{
	public VmaVirtualBlock(nint handle) { Handle = handle; }
	public nint Handle { get; }
	public bool IsNull => Handle == 0;
	public bool IsNotNull => Handle != 0;
	public static VmaVirtualBlock Null => new(0);
	public static implicit operator VmaVirtualBlock(nint handle) => new(handle);
	public static implicit operator nint(VmaVirtualBlock handle) => handle.Handle;
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

