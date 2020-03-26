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
    public partial struct VkClearColorValue
    {
        [FieldOffset(0)]
        public Color4 color;
        
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
            unsafe
            {
                float32[0] = r;
                float32[1] = g;
                float32[2] = b;
                float32[3] = a;
            }
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
            unsafe
            {
                int32[0] = r;
                int32[1] = g;
                int32[2] = b;
                int32[3] = a;
            }
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
            unsafe
            {
                uint32[0] = r;
                uint32[1] = g;
                uint32[2] = b;
                uint32[3] = a;
            }
        }
    }
}
