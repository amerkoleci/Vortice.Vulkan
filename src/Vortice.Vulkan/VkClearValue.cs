// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a clear depth stencil value.
    /// </summary>
    public readonly partial struct VkClearDepthStencilValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearDepthStencilValue"/> structure.
        /// </summary>
        /// <param name="depth">The depth clear value.</param>
        /// <param name="stencil">The stencil clear value.</param>
        public VkClearDepthStencilValue(float depth, uint stencil)
        {
            this.depth = depth;
            this.stencil = stencil;
        }
    }

    /// <summary>
    /// Structure specifying a clear value.
    /// </summary>
    public partial struct VkClearValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearValue"/> structure.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public VkClearValue(float r, float g, float b, float a = 1.0f) 
        {
            depthStencil = default;
            color = new VkClearColorValue(r, g, b, a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearValue"/> structure.
        /// </summary>
        /// <param name="color">The clear color value.</param>
        public VkClearValue(in VkClearColorValue color)
        {
            depthStencil = default;
            this.color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkClearValue"/> structure.
        /// </summary>
        /// <param name="depthStencil">Specifies the depth and stencil clear values to use when clearing a depth/stencil image or attachment.</param>
        public VkClearValue(VkClearDepthStencilValue depthStencil) 
        {
            color = default;
            this.depthStencil = depthStencil;
        }

        public VkClearValue(float depth, uint stencil) 
        {
            color = default;
            depthStencil = new VkClearDepthStencilValue(depth, stencil);
        }
    }
}
