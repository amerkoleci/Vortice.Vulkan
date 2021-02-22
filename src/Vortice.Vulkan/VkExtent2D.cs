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

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is VkExtent2D other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(VkExtent2D other) => width == other.width && height == other.height;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (width.GetHashCode() * 397) ^ height.GetHashCode();
            }
        }

        /// <inheritdoc/>
        public override readonly string ToString() => $"{{Width={width},Height={height}}}";

        /// <summary>
        /// Compares two <see cref="VkExtent2D"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="VkExtent2D"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="VkExtent2D"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(VkExtent2D left, VkExtent2D right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="VkExtent2D"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="VkExtent2D"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="VkExtent2D"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(VkExtent2D left, VkExtent2D right) => !left.Equals(right);
    }
}
