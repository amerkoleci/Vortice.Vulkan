// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a two-dimensional extent.
    /// </summary>
    public partial struct VkExtent2D : IEquatable<VkExtent2D>
    {
        /// <summary>
        /// An <see cref="VkExtent2D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkExtent2D Zero = default;

        /// <summary>
        /// Initializes a new instance of <see cref="VkExtent2D"/> structure.
        /// </summary>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkExtent2D(uint width, uint height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="VkExtent2D"/> structure.
        /// </summary>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkExtent2D(int width, int height)
        {
            this.width = (uint)width;
            this.height = (uint)height;
        }

        /// <summary>
        /// Determines whether the specified <see cref="VkExtent2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkExtent2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkExtent2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref VkExtent2D other) => other.width == width && other.height == height;

        /// <summary>
        /// Determines whether the specified <see cref="VkExtent2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkExtent2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkExtent2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkExtent2D other) => Equals(ref other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is VkExtent2D extent && Equals(ref extent);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = width.GetHashCode();
                hashCode = (hashCode * 397) ^ height.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{Width={width}, Height={height}}}";

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns><c>true</c> if the offsets are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkExtent2D left, VkExtent2D right) => left.Equals(ref right);

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are not equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns>
        /// <c>true</c> if the offsets are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkExtent2D left, VkExtent2D right) => !left.Equals(ref right);
    }
}
