﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure describing a supported swapchain format-color space pair.
/// </summary>
public partial struct VkSurfaceFormatKHR
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VkSurfaceFormatKHR"/> struct.
    /// </summary>
    /// <param name="format">Is the <see cref="VkFormat"/> that is compatible with the specified surface.</param>
    /// <param name="colorSpace">Is a presentation <see cref="VkColorSpaceKHR"/> that is compatible with the surface.</param>
    public VkSurfaceFormatKHR(VkFormat format, VkColorSpaceKHR colorSpace)
    {
        this.format = format;
        this.colorSpace = colorSpace;
    }
}
