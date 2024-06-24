// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a mapped memory range.
/// </summary>
public partial struct VkMappedMemoryRange
{
    public unsafe VkMappedMemoryRange(
        VkDeviceMemory memory,
        ulong offset = 0,
        ulong size = VK_WHOLE_SIZE,
        void* pNext = default)
    {
        this.pNext = pNext;
        this.memory = memory;
        this.offset = offset;
        this.size = size;
    }
}
