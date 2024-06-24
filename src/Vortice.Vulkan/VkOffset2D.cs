// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a two-dimensional offset.
/// </summary>
partial record struct VkOffset2D
{
    /// <summary>
    /// An <see cref="VkOffset2D"/> with all of its components set to zero.
    /// </summary>
    public static VkOffset2D Zero => default;

    public VkOffset2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
