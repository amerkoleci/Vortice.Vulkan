// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a two-dimensional subregion.
    /// </summary>
    public partial struct VkRect2D : IEquatable<VkRect2D>
    {
        /// <summary>
        /// An <see cref="VkRect2D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkRect2D Empty = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="offset">The offset component of the rectangle.</param>
        /// <param name="extent">The extent component of the rectangle.</param>
        public VkRect2D(VkOffset2D offset, VkExtent2D extent)
        {
            this.offset = offset;
            this.extent = extent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="extent">The extent component of the rectangle.</param>
        public VkRect2D(VkExtent2D extent)
        {
            offset = default;
            this.extent = extent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="x">The X component of the offset.</param>
        /// <param name="y">The Y component of the offset.</param>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(int x, int y, uint width, uint height)
        {
            offset = new VkOffset2D(x, y);
            extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="x">The X component of the offset.</param>
        /// <param name="y">The Y component of the offset.</param>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(int x, int y, int width, int height)
        {
            offset = new VkOffset2D(x, y);
            extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(uint width, uint height)
        {
            offset = default;
            extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(int width, int height)
        {
            offset = default;
            extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Determines whether the specified <see cref="VkRect2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkRect2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkRect2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref VkRect2D other) => other.offset.Equals(ref offset) && other.extent.Equals(ref extent);

        /// <summary>
        /// Determines whether the specified <see cref="VkRect2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkRect2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkRect2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkRect2D other) => Equals(ref other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is VkRect2D extent && Equals(ref extent);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = offset.GetHashCode();
                hashCode = (hashCode * 397) ^ extent.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{X={offset.x}, Y={offset.y}, Width={extent.width}, Height={extent.height}}}";

        /// <summary>
        /// Returns a boolean indicating whether the two given rects are equal.
        /// </summary>
        /// <param name="left">The first rect to compare.</param>
        /// <param name="right">The second rect to compare.</param>
        /// <returns><c>true</c> if the rects are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkRect2D left, VkRect2D right) => left.Equals(ref right);

        /// <summary>
        /// Returns a boolean indicating whether the two given rects are not equal.
        /// </summary>
        /// <param name="left">The first rect to compare.</param>
        /// <param name="right">The second rect to compare.</param>
        /// <returns>
        /// <c>true</c> if the rects are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkRect2D left, VkRect2D right) => !left.Equals(ref right);
    }
}
