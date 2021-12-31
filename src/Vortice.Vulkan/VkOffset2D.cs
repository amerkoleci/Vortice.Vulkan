// Copyright (c) Amer Koleci and Contributors
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a two-dimensional offset.
/// </summary>
public partial struct VkOffset2D : IEquatable<VkOffset2D>
{
    /// <summary>
    /// An <see cref="VkOffset2D"/> with all of its components set to zero.
    /// </summary>
    public static readonly VkOffset2D Zero = default;

    public VkOffset2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VkOffset2D other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(VkOffset2D other) => x == other.x && y == other.y;

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            return (x * 397) ^ y;
        }
    }

    /// <inheritdoc/>
    public override readonly string ToString() => $"{{X={x},Y={y}}}";

    /// <summary>
    /// Compares two <see cref="VkOffset2D"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="VkOffset2D"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="VkOffset2D"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(VkOffset2D left, VkOffset2D right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="VkOffset2D"/> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="VkOffset2D"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="VkOffset2D"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(VkOffset2D left, VkOffset2D right) => !left.Equals(right);

    /// <summary>
    /// Performs an implicit conversion from <see cre ="VkOffset2D"/> to <see cref="Point" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Point(VkOffset2D value) => new(value.x, value.y);

    /// <summary>
    /// Performs an implicit conversion from <see cre ="Point"/> to <see cref="VkOffset2D" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator VkOffset2D(Point value) => new(value.X, value.Y);
}
