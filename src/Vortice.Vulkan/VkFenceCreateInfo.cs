// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying parameters of a newly created fence.
    /// </summary>
    public partial struct VkFenceCreateInfo
    {
        public unsafe VkFenceCreateInfo(VkFenceCreateFlags flags = VkFenceCreateFlags.None)
        {
            sType = VkStructureType.FenceCreateInfo;
            pNext = null;
            this.flags = flags;
        }
    }
}
