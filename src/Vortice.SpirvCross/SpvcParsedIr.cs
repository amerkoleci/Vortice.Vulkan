// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Vortice.SpirvCross;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SpvcParsedIr : IEquatable<SpvcParsedIr>
{
    public SpvcParsedIr(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static SpvcParsedIr Null => new(0);
    public static implicit operator SpvcParsedIr(nint handle) => new(handle);
    public static bool operator ==(SpvcParsedIr left, SpvcParsedIr right) => left.Handle == right.Handle;
    public static bool operator !=(SpvcParsedIr left, SpvcParsedIr right) => left.Handle != right.Handle;
    public static bool operator ==(SpvcParsedIr left, nint right) => left.Handle == right;
    public static bool operator !=(SpvcParsedIr left, nint right) => left.Handle != right;
    public bool Equals(SpvcParsedIr other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SpvcParsedIr handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => string.Format("SpvcParsedIr [0x{0}]", Handle.ToString("X"));
}
