// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a mapped memory range.
    /// </summary>
    public partial struct VkMappedMemoryRange
    {
        public unsafe VkMappedMemoryRange(
            VkDeviceMemory memory,
            ulong offset = 0,
            ulong size = Vulkan.WholeSize,
            void* pNext = default)
        {
            sType = VkStructureType.MappedMemoryRange;
            this.pNext = pNext;
            this.memory = memory;
            this.offset = offset;
            this.size = size;
        }
    }
}
