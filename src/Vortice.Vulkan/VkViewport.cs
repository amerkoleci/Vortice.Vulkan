// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a viewport.
    /// </summary>
    public readonly partial struct VkViewport : IEquatable<VkViewport>
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
        /// Determines whether the specified <see cref="VkViewport"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkViewport"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkViewport"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref VkViewport other) =>
            x == other.x
            && y == other.y
            && width == other.width
            && height == other.height
            && minDepth == other.minDepth
            && maxDepth == other.maxDepth;

        /// <summary>
        /// Determines whether the specified <see cref="VkViewport"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkViewport"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkViewport"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkViewport other) => Equals(ref other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is VkViewport viewport && Equals(ref viewport);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            {
                hashCode.Add(x);
                hashCode.Add(y);
                hashCode.Add(width);
                hashCode.Add(height);
                hashCode.Add(minDepth);
                hashCode.Add(maxDepth);
            }
            return hashCode.ToHashCode();
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{X={x}, Y={y}, Width={width}, Height={height}, MinDepth={minDepth}, MaxDepth={maxDepth}}}";

        /// <summary>
        /// Returns a boolean indicating whether the two given viewports are equal.
        /// </summary>
        /// <param name="left">The first viewport to compare.</param>
        /// <param name="right">The second viewport to compare.</param>
        /// <returns><c>true</c> if the viewports are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkViewport left, VkViewport right) => left.Equals(ref right);

        /// <summary>
        /// Returns a boolean indicating whether the two given viewports are not equal.
        /// </summary>
        /// <param name="left">The first viewport to compare.</param>
        /// <param name="right">The second viewport to compare.</param>
        /// <returns>
        /// <c>true</c> if the viewports are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkViewport left, VkViewport right) => !left.Equals(ref right);
    }
}
