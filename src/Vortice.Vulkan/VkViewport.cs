// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Numerics;

namespace Vortice.Vulkan
{
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
        /// <param name="bounds">A <see cref="VkRect2D"/> that defines the location and size of the viewport in a render target.</param>
        public VkViewport(VkRect2D bounds)
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
        public VkViewport(Vector4 bounds)
        {
            x = bounds.X;
            y = bounds.Y;
            width = bounds.Z;
            height = bounds.W;

            minDepth = 0.0f;
            maxDepth = 1.0f;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is VkViewport other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(VkViewport other)
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
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ width.GetHashCode();
                hashCode = (hashCode * 397) ^ height.GetHashCode();
                hashCode = (hashCode * 397) ^ minDepth.GetHashCode();
                hashCode = (hashCode * 397) ^ maxDepth.GetHashCode();
                return hashCode;
            }
        }

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
}
