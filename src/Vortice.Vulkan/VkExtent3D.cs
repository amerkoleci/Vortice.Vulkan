// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a three-dimensional extent.
/// </summary>
public partial struct VkExtent3D : IEquatable<VkExtent3D>
{
    /// <summary>
    /// An <see cref="VkExtent3D"/> with all of its components set to zero.
    /// </summary>
    public static readonly VkExtent3D Zero = default;

    /// <summary>
    /// Initializes a new instance of <see cref="VkExtent3D"/> structure.
    /// </summary>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    /// <param name="depth">The depth component of the extent.</param>
    public VkExtent3D(uint width, uint height, uint depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VkExtent3D"/> structure.
    /// </summary>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    /// <param name="depth">The depth component of the extent.</param>
    public VkExtent3D(int width, int height, int depth)
    {
        this.width = (uint)width;
        this.height = (uint)height;
        this.depth = (uint)depth;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VkExtent3D other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(VkExtent3D other) => width == other.width && height == other.height && depth == other.depth;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(width, height, depth);

    /// <inheritdoc/>
    public override readonly string ToString() => $"{{Width={width},Height={height},Depth={depth}}}";

    /// <summary>
    /// Compares two <see cref="VkExtent3D"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="VkExtent3D"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="VkExtent3D"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(VkExtent3D left, VkExtent3D right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="VkExtent3D"/> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="VkExtent3D"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="VkExtent3D"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(VkExtent3D left, VkExtent3D right) => !left.Equals(right);
}
