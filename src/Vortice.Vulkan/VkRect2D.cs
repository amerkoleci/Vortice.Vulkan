// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a two-dimensional subregion.
/// </summary>
partial record struct VkRect2D 
{
    /// <summary>
    /// An <see cref="VkRect2D"/> with all of its components set to zero.
    /// </summary>
    public static VkRect2D Zero => new(VkOffset2D.Zero, VkExtent2D.Zero);

    /// <summary>
    /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
    /// </summary>
    /// <param name="x">The X component of the offset.</param>
    /// <param name="y">The Y component of the offset.</param>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    public VkRect2D(int x, int y, uint width, uint height)
    {
        offset = new VkOffset2D(x, y);
        extent = new VkExtent2D(width, height);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
    /// </summary>
    /// <param name="offset">The offset component of the rectangle.</param>
    /// <param name="extent">The extent component of the rectangle.</param>
    public VkRect2D(VkOffset2D offset, VkExtent2D extent)
    {
        this.offset = offset;
        this.extent = extent;
    }
}
