// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Vortice.Mathematics;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a clear color value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct VkClearColorValue
    {
        [FieldOffset(0)] public Color4 Float4;
        [FieldOffset(0)] public Int4 Int4;
        [FieldOffset(0)] public unsafe fixed uint uint32[4];

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="color">The <see cref="Color4"/> color value.</param>
        public VkClearColorValue(in Color4 color) : this()
        {
            Float4 = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public VkClearColorValue(float r, float g, float b, float a = 1.0f) : this()
        {
            Float4 = new Color4(r, g, b, a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public unsafe VkClearColorValue(int r, int g, int b, int a = 255) : this()
        {
            Int4 = new Int4(r, g, b, a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public unsafe VkClearColorValue(uint r, uint g, uint b, uint a = 255) : this()
        {
            uint32[0] = r;
            uint32[1] = g;
            uint32[2] = b;
            uint32[3] = a;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Color4"/> to <see cref="VkClearColorValue"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator VkClearColorValue(Color4 value) => new VkClearColorValue(value);
    }
}
