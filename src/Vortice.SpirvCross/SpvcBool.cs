// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.SpirvCross;

public readonly partial struct SpvcBool(byte value) : IComparable, IComparable<SpvcBool>, IEquatable<SpvcBool>, IFormattable
{
    public readonly byte Value = value;

    public static SpvcBool False => new(0);
    public static SpvcBool True => new(1);

    public static bool operator ==(SpvcBool left, SpvcBool right) => left.Value == right.Value;

    public static bool operator !=(SpvcBool left, SpvcBool right) => left.Value != right.Value;

    public static bool operator <(SpvcBool left, SpvcBool right) => left.Value < right.Value;

    public static bool operator <=(SpvcBool left, SpvcBool right) => left.Value <= right.Value;

    public static bool operator >(SpvcBool left, SpvcBool right) => left.Value > right.Value;

    public static bool operator >=(SpvcBool left, SpvcBool right) => left.Value >= right.Value;

    public static implicit operator bool(SpvcBool value) => value.Value != 0;

    public static implicit operator SpvcBool(bool value) => new SpvcBool(value ? (byte)1 : (byte)0);

    public static bool operator false(SpvcBool value) => value.Value == 0;

    public static bool operator true(SpvcBool value) => value.Value != 0;

    public static implicit operator SpvcBool(byte value) => new SpvcBool(value);

    public static explicit operator byte(SpvcBool value) => (byte)(value.Value);

    public int CompareTo(object? obj)
    {
        if (obj is SpvcBool other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SpvcBool)}.");
    }

    public int CompareTo(SpvcBool other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SpvcBool other) && Equals(other);

    public bool Equals(SpvcBool other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
