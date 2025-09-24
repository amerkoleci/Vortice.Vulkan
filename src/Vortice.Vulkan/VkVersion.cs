// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Vortice.Vulkan;

public readonly struct VkVersion : IComparable, IComparable<VkVersion>, IEquatable<VkVersion>, IFormattable
{
    public readonly uint Value;

    public static VkVersion Version_1_0 => new(0, 1, 0, 0);
    public static VkVersion Version_1_1 => new(0, 1, 1, 0);
    public static VkVersion Version_1_2 => new(0, 1, 2, 0);
    public static VkVersion Version_1_3 => new(0, 1, 3, 0);
    public static VkVersion Version_1_4 => new(0, 1, 4, 0);
    public static VkVersion HeaderVersionComplete => Vulkan.VK_HEADER_VERSION_COMPLETE;

    public VkVersion(uint value)
    {
        Value = value;
    }

    public VkVersion(uint variant, uint major, uint minor, uint patch)
    {
        Value = (variant << 29) | (major << 22) | (minor << 12) | patch;
    }

    public VkVersion(uint major, uint minor, uint patch)
    {
        Value = (major << 22) | (minor << 12) | patch;
    }

    public uint Variant => Value >> 29;

    public uint Major => ((Value >> 22) & 0x7Fu);

    public uint Minor => ((Value >> 12) & 0x3FFu);

    public uint Patch => Value & 0xFFFu;

    public static bool operator ==(VkVersion left, VkVersion right) => left.Value == right.Value;

    public static bool operator !=(VkVersion left, VkVersion right) => left.Value != right.Value;

    public static bool operator <(VkVersion left, VkVersion right) => left.Value < right.Value;

    public static bool operator <=(VkVersion left, VkVersion right) => left.Value <= right.Value;

    public static bool operator >(VkVersion left, VkVersion right) => left.Value > right.Value;

    public static bool operator >=(VkVersion left, VkVersion right) => left.Value >= right.Value;


    public static implicit operator uint(VkVersion version) => version.Value;

    public int CompareTo(object? obj)
    {
        if (obj is VkVersion other)
        {
            return CompareTo(other);
        }

        throw new ArgumentException($"obj is not an instance of {nameof(VkVersion)}.");
    }

    public int CompareTo(VkVersion other) => Value.CompareTo(other.Value);

    public override bool Equals([NotNullWhen(true)] object? obj) => (obj is VkVersion other) && Equals(other);

    public bool Equals(VkVersion other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => $"{Variant}.{Major}.{Minor}.{Patch}";

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"{Variant.ToString(format, formatProvider)}.{Major.ToString(format, formatProvider)}.{Minor.ToString(format, formatProvider)}.{Patch.ToString(format, formatProvider)}";
    }
}
