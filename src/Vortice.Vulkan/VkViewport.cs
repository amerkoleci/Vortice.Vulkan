// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a two-dimensional subregion.
/// </summary>
public partial struct VkViewport : IEquatable<VkViewport>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VkViewport"/> struct.
    /// </summary>
    /// <param name="width">The width of the viewport in pixels.</param>
    /// <param name="height">The height of the viewport in pixels.</param>
    public VkViewport(float width, float height)
    {
        x = 0.0f;
        y = 0.0f;
        this.width = width;
        this.height = height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkViewport"/> struct.
    /// </summary>
    /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="width">The width of the viewport in pixels.</param>
    /// <param name="height">The height of the viewport in pixels.</param>
    public VkViewport(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkViewport"/> struct.
    /// </summary>
    /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="width">The width of the viewport in pixels.</param>
    /// <param name="height">The height of the viewport in pixels.</param>
    /// <param name="minDepth">The minimum depth of the clip volume.</param>
    /// <param name="maxDepth">The maximum depth of the clip volume.</param>
    public VkViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.minDepth = minDepth;
        this.maxDepth = maxDepth;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkViewport"/> struct.
    /// </summary>
    /// <param name="extent">The width and height extent of the viewport in pixels.</param>
    public VkViewport(in VkExtent2D extent)
    {
        x = 0.0f;
        y = 0.0f;
        width = extent.width;
        height = extent.height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="VkViewport"/> struct.
    /// </summary>
    /// <param name="bounds">A <see cref="VkRect2D"/> that defines the location and size of the viewport in a render target.</param>
    public VkViewport(in VkRect2D bounds)
    {
        x = bounds.offset.x;
        y = bounds.offset.y;
        width = bounds.extent.width;
        height = bounds.extent.height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkViewport"/> struct.
    /// </summary>
    /// <param name="bounds">A <see cref="Vector4"/> that defines the location and size of the viewport in a render target.</param>
    public VkViewport(in Vector4 bounds)
    {
        x = bounds.X;
        y = bounds.Y;
        width = bounds.Z;
        height = bounds.W;

        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <inheritdoc/>
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is VkViewport other && Equals(other);

    /// <inheritdoc/>
    public readonly bool Equals(VkViewport other)
    {
        return
            x == other.x &&
            y == other.y &&
            width == other.width &&
            height == other.height &&
            minDepth == other.minDepth &&
            maxDepth == other.maxDepth;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(x, y, width, height, minDepth, maxDepth);

    /// <inheritdoc/>
    public override readonly string ToString() => $"{{X={x},Y={y},Width={width},Height={height},MinDepth={minDepth},MaxDepth={maxDepth}}}";

    /// <summary>
    /// Compares two <see cref="VkViewport"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="VkViewport"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="VkViewport"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(VkViewport left, VkViewport right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="VkViewport"/> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="VkViewport"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="VkViewport"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(VkViewport left, VkViewport right) => !left.Equals(right);
}
