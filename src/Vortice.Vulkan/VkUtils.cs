// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Vortice.Vulkan
{
    public static unsafe class VkUtils
    {
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckResult(this VkResult result)
        {
            if (result != VkResult.Success)
            {
                throw new VkException(result);
            }
        }

        public static string GetExtensionName(this VkExtensionProperties properties)
        {
            return Interop.StringFromPointer(properties.extensionName);
        }

        public static string GetName(this VkLayerProperties properties)
        {
            return Interop.StringFromPointer(properties.layerName);
        }

        public static string GetDescription(this VkLayerProperties properties)
        {
            return Interop.StringFromPointer(properties.description);
        }

        public static VkMemoryType GetMemoryType(this VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
        {
            return (&memoryProperties.memoryTypes_0)[index];
        }

        public static uint IndexOf(this VkPhysicalDeviceMemoryProperties memoryProperties, int memoryTypeBits, VkMemoryPropertyFlags properties)
        {
            uint count = memoryProperties.memoryTypeCount;
            for (uint i = 0; i < count; i++)
            {
                if ((memoryTypeBits & 1) == 1 &&
                    ((&memoryProperties.memoryTypes_0)[i].propertyFlags & properties) == properties)
                {
                    return i;
                }
                memoryTypeBits >>= 1;
            }

            return uint.MaxValue;
        }
    }
}
