// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a three-dimensional offset
    /// </summary>
    public readonly partial struct VkOffset3D : IEquatable<VkOffset3D>, IFormattable
    {
        /// <summary>
        /// An <see cref="VkOffset3D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkOffset3D Zero = default;

        /// <summary>
        /// Gets a value indicating whether this <see cref="VkOffset3D"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly bool IsEmpty => this == Zero;

        public VkOffset3D(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns><c>true</c> if the offsets are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkOffset3D left, VkOffset3D right) => left.x == right.x && left.y == right.y && left.z == right.z;

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are not equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns>
        /// <c>true</c> if the offsets are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkOffset3D left, VkOffset3D right) => (left.x != right.x) || (left.y != right.y) || (left.z != right.z);

        /// <summary>
        /// Determines whether the specified <see cref="VkOffset3D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkOffset3D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkOffset3D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkOffset3D other) => this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is VkOffset3D other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            {
                hashCode.Add(x);
                hashCode.Add(y);
                hashCode.Add(z);
            }
            return hashCode.ToHashCode();
        }

        /// <inheritdoc/>
        public override string ToString() => ToString(format: null, formatProvider: null);

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            string? separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

            return new StringBuilder()
                .Append('<')
                .Append(x.ToString(format, formatProvider)).Append(separator).Append(' ')
                .Append(y.ToString(format, formatProvider)).Append(separator).Append(' ')
                .Append(z.ToString(format, formatProvider))
                .Append('>')
                .ToString();
        }
    }
}
