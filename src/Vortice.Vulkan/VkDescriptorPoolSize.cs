// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying descriptor pool size.
/// </summary>
public partial struct VkDescriptorPoolSize
{
    public VkDescriptorPoolSize(VkDescriptorType type, uint descriptorCount)
    {
        this.type = type;
        this.descriptorCount = descriptorCount;
    }
}
