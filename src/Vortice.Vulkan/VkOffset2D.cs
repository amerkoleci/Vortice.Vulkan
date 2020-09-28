// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a two-dimensional offset
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

        /// <summary>
        /// Determines whether the specified <see cref="VkOffset2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkOffset2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkOffset2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref VkOffset2D other) => other.x == x && other.y == y;

        /// <summary>
        /// Determines whether the specified <see cref="VkOffset2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkOffset2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkOffset2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkOffset2D other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is VkOffset2D offset && Equals(ref offset);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{X={x}, Y={y}}}";

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns><c>true</c> if the offsets are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkOffset2D left, VkOffset2D right) => left.Equals(ref right);

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are not equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns>
        /// <c>true</c> if the offsets are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkOffset2D left, VkOffset2D right) => !left.Equals(ref right);
    }
}
