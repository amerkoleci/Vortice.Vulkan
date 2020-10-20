// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a three-dimensional extent.
    /// </summary>
    public readonly partial struct VkExtent3D : IEquatable<VkExtent3D>, IFormattable
    {
        /// <summary>
        /// An <see cref="VkExtent3D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkExtent3D Zero = default;

        /// <summary>
        /// A special valued <see cref="VkExtent2D"/>.
        /// </summary>
        public static readonly VkExtent3D WholeSize = new VkExtent3D(~0, ~0, ~0);

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
        /// Initializes a new instance of <see cref="VkExtent3D"/> structure.
        /// </summary>
        /// <param name="extent2D">The <see cref="VkExtent2D"/> containing width and height.</param>
        /// <param name="depth">The depth component of the extent.</param>
        public VkExtent3D(VkExtent2D extent2D, uint depth)
        {
            width = extent2D.width;
            height = extent2D.height;
            this.depth = depth;
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns><c>true</c> if the offsets are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkExtent3D left, VkExtent3D right) => left.Equals(right);

        /// <summary>
        /// Returns a boolean indicating whether the two given offsets are not equal.
        /// </summary>
        /// <param name="left">The first offset to compare.</param>
        /// <param name="right">The second offset to compare.</param>
        /// <returns>
        /// <c>true</c> if the offsets are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkExtent3D left, VkExtent3D right) => !left.Equals(right);

        /// <summary>
        /// Determines whether the specified <see cref="VkExtent3D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkExtent3D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkExtent3D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkExtent3D other) => other.width == width && other.height == height && other.depth == depth;

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is VkExtent3D extent && Equals(extent);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            {
                hashCode.Add(width);
                hashCode.Add(height);
                hashCode.Add(depth);
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
                .Append(width.ToString(format, formatProvider)).Append(separator).Append(' ')
                .Append(height.ToString(format, formatProvider)).Append(separator).Append(' ')
                .Append(depth.ToString(format, formatProvider))
                .Append('>')
                .ToString();
        }
    }
}
