// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    /// <summary>
    /// A platform-specific type that is used to represent a size (in bytes) of an object in memory.
    /// <para>Equivalent to C/C++ size_t type.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VkPointerSize : IEquatable<VkPointerSize>
    {
        private readonly UIntPtr _value;

        /// <summary>
        /// An empty pointer size initialized to zero.
        /// </summary>
        public static readonly VkPointerSize Zero = new VkPointerSize(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="VkPointerSize"/> struct.
        /// </summary>
        /// <param name="value">The value to assign.</param>
        public VkPointerSize(UIntPtr value)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkPointerSize"/> struct.
        /// </summary>
        /// <param name="size"></param>
        private unsafe VkPointerSize(void* size)
        {
            _value = new UIntPtr(size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkPointerSize"/> struct.
        /// </summary>
        /// <param name="value">value to set</param>
        public VkPointerSize(uint value)
        {
            _value = new UIntPtr(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkPointerSize"/> struct.
        /// </summary>
        /// <param name="value">value to set</param>
        public VkPointerSize(ulong value)
        {
            _value = new UIntPtr(value);
        }

        /// <summary>
        /// Compares two <see cref="VkPointerSize"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="VkPointerSize"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="VkPointerSize"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(VkPointerSize left, VkPointerSize right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="VkPointerSize"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="VkPointerSize"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="VkPointerSize"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(VkPointerSize left, VkPointerSize right) => !left.Equals(right);

        /// <inheritdoc/>
		public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => _value.ToString();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is VkPointerSize other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(VkPointerSize other) => _value == other._value;

        /// <summary>
        ///   Performs an implicit conversion from <see cref="VkPointerSize"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator uint(VkPointerSize value) => value._value.ToUInt32();

        /// <summary>
        ///   Performs an implicit conversion from <see cref="VkPointerSize"/> to <see cref="ulong"/>.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ulong(VkPointerSize value) => value._value.ToUInt64();

        /// <summary>
        ///   Performs an implicit conversion from <see cref="uint"/> to <see cref="VkPointerSize"/>.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator VkPointerSize(uint value) => new VkPointerSize(value);

        /// <summary>
        ///   Performs an implicit conversion from <see cref="ulong"/> to <see cref="VkPointerSize"/>.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator VkPointerSize(ulong value) => new VkPointerSize(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="UIntPtr"/> to <see cref="VkPointerSize"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator VkPointerSize(UIntPtr value) => new VkPointerSize(value);

        /// <summary>
        ///   Performs an implicit conversion from <see cref = "VkPointerSize" /> to <see cref = "UIntPtr" />.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UIntPtr(VkPointerSize value) => value._value;

        /// <summary>
        ///   Performs an implicit conversion from void* to <see cref = "VkPointerSize" />.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static unsafe implicit operator VkPointerSize(void* value) => new VkPointerSize(value);

        /// <summary>
        ///   Performs an implicit conversion from <see cref = "VkPointerSize" /> to void*.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static unsafe implicit operator void*(VkPointerSize value) => (void*)value._value;
    }
}
