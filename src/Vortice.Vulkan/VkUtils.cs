// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

unsafe partial class Vulkan
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfFailed(VkResult result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result != VkResult.Success)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new VkException(result, message);
        }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DebugThrowIfFailed(VkResult result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result != VkResult.Success)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new VkException(result, message);
        }
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void CheckResult(this VkResult result, string message = "Vulkan error occured")
    {
        if (result != VkResult.Success)
        {
            throw new VkException(result, message);
        }
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    public static void DebugCheckResult(this VkResult result, string message = "Vulkan error occured")
    {
        if (result != VkResult.Success)
        {
            throw new VkException(result, message);
        }
    }

    public static string GetExtensionName(this VkExtensionProperties properties)
    {
        return Interop.GetString(properties.extensionName, 256)!;
    }

    public static string GetLayerName(this VkLayerProperties properties)
    {
        return Interop.GetString(properties.layerName, 256)!;
    }

    public static string GetDeviceName(this VkPhysicalDeviceProperties properties)
    {
        return Interop.GetString(properties.deviceName, 256)!;
    }

    public static string GetDescription(this VkLayerProperties properties)
    {
        return Interop.GetString(properties.description, 256)!;
    }

    public static uint IndexOf(this VkPhysicalDeviceMemoryProperties memoryProperties, int memoryTypeBits, VkMemoryPropertyFlags properties)
    {
        uint count = memoryProperties.memoryTypeCount;
        for (uint i = 0; i < count; i++)
        {
            if ((memoryTypeBits & 1) == 1 &&
                (memoryProperties.memoryTypes[(int)i].propertyFlags & properties) == properties)
            {
                return i;
            }
            memoryTypeBits >>= 1;
        }

        return uint.MaxValue;
    }
}
