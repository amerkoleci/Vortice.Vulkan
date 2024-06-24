// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a two-dimensional extent.
/// </summary>
partial record struct VkExtent2D
{
    /// <summary>
    /// An <see cref="VkExtent2D"/> with all of its components set to zero.
    /// </summary>
    public static VkExtent2D Zero => default;

    /// <summary>
    /// Initializes a new instance of <see cref="VkExtent2D"/> structure.
    /// </summary>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    public VkExtent2D(uint width, uint height)
    {
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VkExtent2D"/> structure.
    /// </summary>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    public VkExtent2D(int width, int height)
    {
        this.width = (uint)width;
        this.height = (uint)height;
    }
}
