// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a two-dimensional offset
    /// </summary>
    public readonly partial struct VkOffset2D : IEquatable<VkOffset2D>, IFormattable
    {
        /// <summary>
        /// An <see cref="VkOffset2D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkOffset2D Zero = default;

        /// <summary>
        /// Gets a value indicating whether this <see cref="VkOffset2D"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly bool IsEmpty => this == Zero;

        public VkOffset2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns><c>true</c> if the offsets are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkOffset2D left, VkOffset2D right) => left.x == right.x && left.y == right.y;

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are not equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns>
        /// <c>true</c> if the offsets are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkOffset2D left, VkOffset2D right) => (left.x != right.x) || (left.y != right.y);

        /// <summary>
        /// Determines whether the specified <see cref="VkOffset2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkOffset2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkOffset2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkOffset2D other) => this == other;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is VkOffset2D other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            {
                hashCode.Add(x);
                hashCode.Add(y);
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
                .Append(x.ToString(format, formatProvider))
                .Append(separator)
                .Append(' ')
                .Append(y.ToString(format, formatProvider))
                .Append('>')
                .ToString();
        }
    }
}
