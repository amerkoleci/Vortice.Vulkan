// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a three-dimensional offset.
    /// </summary>
    public partial struct VkOffset3D : IEquatable<VkOffset3D>
    {
        /// <summary>
        /// An <see cref="VkOffset3D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkOffset3D Zero = default;

        public VkOffset3D(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is VkOffset3D other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(VkOffset3D other) => x == other.x && y == other.y && z == other.z;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ z;
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override readonly string ToString() => $"{{X={x},Y={y},Z={z}}}";

        /// <summary>
        /// Compares two <see cref="VkOffset3D"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="VkOffset3D"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="VkOffset3D"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(VkOffset3D left, VkOffset3D right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="VkOffset3D"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="VkOffset3D"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="VkOffset3D"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(VkOffset3D left, VkOffset3D right) => !left.Equals(right);
    }
}
