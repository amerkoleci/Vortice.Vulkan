// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Mathematics;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a clear depth stencil value.
    /// </summary>
    public partial struct VkClearDepthStencilValue
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
        /// <param name="color">Specifies the color image clear values to use when clearing a color image or attachment.</param>
        public VkClearValue(Color4 color)
        {
            this.color = new VkClearColorValue(color);
            depthStencil = default;
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
