// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Vortice.Vulkan;

public static unsafe class VkUtils
{
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void CheckResult(this VkResult result, string message = "Vulkan error occured")
    {
        if (result != VkResult.Success)
        {
            throw new VkException(result, message);
        }
    }

    public static string GetExtensionName(this VkExtensionProperties properties)
    {
        return Interop.GetString(properties.extensionName);
    }

    public static string GetLayerName(this VkLayerProperties properties)
    {
        return Interop.GetString(properties.layerName);
    }

    public static string GetDeviceName(this VkPhysicalDeviceProperties properties)
    {
        return Interop.GetString(properties.deviceName);
    }

    public static string GetDescription(this VkLayerProperties properties)
    {
        return Interop.GetString(properties.description);
    }

    public static VkMemoryType GetMemoryType(this VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
    {
        return (&memoryProperties.memoryTypes_0)[index];
    }

    public static VkMemoryHeap GetMemoryHeap(this VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
    {
        return (&memoryProperties.memoryHeaps_0)[index];
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
