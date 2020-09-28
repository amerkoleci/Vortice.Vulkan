// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
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

        /// <summary>
        /// Determines whether the specified <see cref="VkExtent3D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkExtent3D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkExtent3D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref VkExtent3D other) => other.width == width && other.height == height && other.depth == depth;

        /// <summary>
        /// Determines whether the specified <see cref="VkExtent3D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkExtent3D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkExtent3D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkExtent3D other) => Equals(ref other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is VkExtent3D extent && Equals(ref extent);

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
                hashCode = (hashCode * 397) ^ depth.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{Width={width}, Height={height}, Depth={depth}}}";

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns><c>true</c> if the offsets are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkExtent3D left, VkExtent3D right) => left.Equals(ref right);

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are not equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns>
        /// <c>true</c> if the offsets are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkExtent3D left, VkExtent3D right) => !left.Equals(ref right);
    }
}
