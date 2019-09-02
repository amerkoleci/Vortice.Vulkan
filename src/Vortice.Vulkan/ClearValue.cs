// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#if TODO
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a clear depth stencil value.
    /// </summary>
    public partial struct ClearDepthStencilValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearDepthStencilValue"/> structure.
        /// </summary>
        /// <param name="depth">The depth clear value.</param>
        /// <param name="stencil">The stencil clear value.</param>
        public ClearDepthStencilValue(float depth, uint stencil)
        {
            Depth = depth;
            Stencil = stencil;
        }
    }

    /// <summary>
    /// Structure specifying a clear value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct ClearValue
    {
        /// <summary>
        /// Specifies the color image clear values to use when clearing a color image or attachment.
        /// </summary>
        [FieldOffset(0)]
        public Vector4 Color;

        /// <summary>
        /// Specifies the depth and stencil clear values to use when clearing a depth/stencil image
        /// or attachment.
        /// </summary>
        [FieldOffset(0)]
        public ClearDepthStencilValue DepthStencil;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearValue"/> structure.
        /// </summary>
        /// <param name="color">Specifies the color image clear values to use when clearing a color image or attachment.</param>
        public ClearValue(Vector4 color)
        {
            Color = color;
            DepthStencil = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearValue"/> structure.
        /// </summary>
        /// <param name="depthStencil">Specifies the depth and stencil clear values to use when clearing a depth/stencil image or attachment.</param>
        public ClearValue(ClearDepthStencilValue depthStencil)
        {
            Color = default;
            DepthStencil = depthStencil;
        }

        public ClearValue(float depth, uint stencil)
        {
            Color = default;
            DepthStencil = new ClearDepthStencilValue(depth, stencil);
        }
    }
}

#endif
