// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a three-dimensional offset.
/// </summary>
partial record struct VkOffset3D
{
    /// <summary>
    /// An <see cref="VkOffset3D"/> with all of its components set to zero.
    /// </summary>
    public static VkOffset3D Zero => default;

    public VkOffset3D(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
