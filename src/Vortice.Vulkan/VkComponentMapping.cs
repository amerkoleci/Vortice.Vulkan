// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a color component mapping.
    /// </summary>
    public partial struct VkComponentMapping
    {
        public static readonly VkComponentMapping Identity = new VkComponentMapping(VkComponentSwizzle.Identity, VkComponentSwizzle.Identity, VkComponentSwizzle.Identity, VkComponentSwizzle.Identity);
        public static readonly VkComponentMapping Rgba = new VkComponentMapping(VkComponentSwizzle.R, VkComponentSwizzle.G, VkComponentSwizzle.B, VkComponentSwizzle.A);

        public VkComponentMapping(VkComponentSwizzle r, VkComponentSwizzle g, VkComponentSwizzle b, VkComponentSwizzle a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}
