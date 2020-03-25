// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    public partial struct VkExtent2D : IEquatable<VkExtent2D>
    {
        /// <summary>
        /// A special valued <see cref="VkExtent2D"/>.
        /// </summary>
        public static readonly VkExtent2D WholeSize = new VkExtent2D(~0u, ~0u);

        /// <summary>
        /// An <see cref="VkExtent2D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkExtent2D Zero = new VkExtent2D(0, 0);

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

        public override string ToString() => $"{width}x{height}";
        public bool Equals(ref VkExtent2D other) => other.width == width && other.height == height;
        public bool Equals(VkExtent2D other) => Equals(ref other);
        public override bool Equals(object obj) => obj is VkExtent2D && Equals((VkExtent2D)obj);

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

        public static bool operator ==(VkExtent2D left, VkExtent2D right) => left.Equals(ref right);
        public static bool operator !=(VkExtent2D left, VkExtent2D right) => !left.Equals(ref right);
    }
}
