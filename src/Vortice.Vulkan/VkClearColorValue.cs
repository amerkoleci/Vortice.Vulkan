// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Numerics;
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
        [FieldOffset(0)]
        public Color4 color;
        [FieldOffset(0)]
        public float float32_0;
        [FieldOffset(4)]
        public float float32_1;
        [FieldOffset(8)]
        public float float32_2;
        [FieldOffset(12)]
        public float float32_3;
        [FieldOffset(0)]
        public int int32_0;
        [FieldOffset(4)]
        public int int32_1;
        [FieldOffset(8)]
        public int int32_2;
        [FieldOffset(12)]
        public int int32_3;
        [FieldOffset(0)]
        public uint uint32_0;
        [FieldOffset(4)]
        public uint uint32_1;
        [FieldOffset(8)]
        public uint uint32_2;
        [FieldOffset(12)]
        public uint uint32_3;

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="color">The <see cref="Color4"/> value.</param>
        public VkClearColorValue(in Color4 color)
            : this()
        {
            this.color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public VkClearColorValue(float r, float g, float b, float a = 1.0f)
            : this()
        {
            float32_0 = r;
            float32_1 = g;
            float32_2 = b;
            float32_3 = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public VkClearColorValue(int r, int g, int b, int a = 255)
            : this()
        {
            int32_0 = r;
            int32_1 = g;
            int32_2 = b;
            int32_3 = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public VkClearColorValue(uint r, uint g, uint b, uint a = 255)
            : this()
        {
            uint32_0 = r;
            uint32_1 = g;
            uint32_2 = b;
            uint32_3 = a;
        }
    }
}
