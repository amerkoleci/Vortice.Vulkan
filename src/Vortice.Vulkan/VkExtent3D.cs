// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a three-dimensional extent.
/// </summary>
partial record struct VkExtent3D : IEquatable<VkExtent3D>
{
    /// <summary>
    /// An <see cref="VkExtent3D"/> with all of its components set to zero.
    /// </summary>
    public static VkExtent3D Zero => default;

    /// <summary>
    /// Initializes a new instance of <see cref="VkExtent3D"/> structure.
    /// </summary>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    /// <param name="depth">The depth component of the extent.</param>
    public VkExtent3D(uint width, uint height, uint depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VkExtent3D"/> structure.
    /// </summary>
    /// <param name="width">The width component of the extent.</param>
    /// <param name="height">The height component of the extent.</param>
    /// <param name="depth">The depth component of the extent.</param>
    public VkExtent3D(int width, int height, int depth)
    {
        this.width = (uint)width;
        this.height = (uint)height;
        this.depth = (uint)depth;
    }
}
