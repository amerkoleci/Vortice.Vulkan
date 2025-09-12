// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Vortice.Vulkan;

public partial class VkDeviceTable
{
    public VkDeviceTable(VkDevice device)
    {
        Device = device;
    }

    public VkDevice Device { get; }
}
