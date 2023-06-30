// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a color component mapping.
/// </summary>
public partial struct VkComponentMapping
{
    public static VkComponentMapping Identity => new(VkComponentSwizzle.Identity, VkComponentSwizzle.Identity, VkComponentSwizzle.Identity, VkComponentSwizzle.Identity);
    public static VkComponentMapping Rgba => new(VkComponentSwizzle.R, VkComponentSwizzle.G, VkComponentSwizzle.B, VkComponentSwizzle.A);

    public VkComponentMapping(VkComponentSwizzle r, VkComponentSwizzle g, VkComponentSwizzle b, VkComponentSwizzle a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}
