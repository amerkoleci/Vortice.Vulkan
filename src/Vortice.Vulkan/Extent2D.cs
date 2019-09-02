// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

//namespace Vortice.Vulkan
//{
//    public partial struct Extent2D : IEquatable<Extent2D>
//    {
//        /// <summary>
//        /// A special valued <see cref="Extent2D"/>.
//        /// </summary>
//        public static readonly Extent2D WholeSize = new Extent2D(~0u, ~0u);

//        /// <summary>
//        /// An <see cref="Extent2D"/> with all of its components set to zero.
//        /// </summary>
//        public static readonly Extent2D Zero = new Extent2D(0, 0);

//        /// <summary>
//        /// Initializes a new instance of <see cref="Extent2D"/> structure.
//        /// </summary>
//        /// <param name="width">The width component of the extent.</param>
//        /// <param name="height">The height component of the extent.</param>
//        public Extent2D(uint width, uint height)
//        {
//            Width = width;
//            Height = height;
//        }

//        public override string ToString() => $"{Width}x{Height}";
//        public bool Equals(ref Extent2D other) => other.Width == Width && other.Height == Height;
//        public bool Equals(Extent2D other) => Equals(ref other);
//        public override bool Equals(object obj) => obj is Extent2D && Equals((Extent2D)obj);

//        /// <summary>
//        /// Returns the hash code for this instance.
//        /// </summary>
//        /// <returns>The hash code.</returns>
//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                int hashCode = Width.GetHashCode();
//                hashCode = (hashCode * 397) ^ Height.GetHashCode();
//                return hashCode;
//            }
//        }

//        public static bool operator ==(Extent2D left, Extent2D right) => left.Equals(ref right);
//        public static bool operator !=(Extent2D left, Extent2D right) => !left.Equals(ref right);
//    }
//}
