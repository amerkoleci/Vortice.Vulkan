// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Vortice.Vulkan;

public readonly unsafe partial struct PFN_vkVoidFunction : IEquatable<PFN_vkVoidFunction>
{
    public PFN_vkVoidFunction(delegate* unmanaged[Stdcall]<void> value) => this.Value = value;

    public delegate* unmanaged[Stdcall]<void> Value { get; }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is PFN_vkVoidFunction other && Equals(other);

    public bool Equals(PFN_vkVoidFunction other) => Value == other.Value;

    public override int GetHashCode() => ((nint)(void*)Value).GetHashCode();

    public override string ToString() => ((nint)(void*)Value).ToString();

    public static implicit operator delegate* unmanaged[Stdcall]<void>(PFN_vkVoidFunction from) => from.Value;

    public static implicit operator PFN_vkVoidFunction(delegate* unmanaged[Stdcall]<void> from) => new(from);

    public static bool operator ==(PFN_vkVoidFunction left, PFN_vkVoidFunction right) => left.Equals(right);

    public static bool operator !=(PFN_vkVoidFunction left, PFN_vkVoidFunction right) => !left.Equals(right);
}

