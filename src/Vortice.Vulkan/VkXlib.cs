// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VkXlibSurfaceCreateFlagsKHR
{
    None = 0,
}

public struct VkXlibSurfaceCreateInfoKHR
{
    public VkStructureType sType;
    public unsafe void* pNext;
    public VkXlibSurfaceCreateFlagsKHR flags;
    public nint display;
    public ulong window;

    public VkXlibSurfaceCreateInfoKHR()
    {
        sType = VkStructureType.XlibSurfaceCreateInfoKHR;
    }
}

public static unsafe partial class Vulkan
{
    public static readonly string VK_KHR_XLIB_SURFACE_EXTENSION_NAME = "VK_KHR_xlib_surface";

    /// <summary>
    /// VK_KHR_XLIB_SURFACE_EXTENSION_NAME = "VK_KHR_xlib_surface"
    /// </summary>
    public static readonly string KHRXlibSurfaceExtensionName = "VK_KHR_xlib_surface";

    private static delegate* unmanaged<VkInstance, VkXlibSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateXlibSurfaceKHR_ptr;
    private static delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr;

    private static void LoadXlib(VkInstance instance)
    {
        vkCreateXlibSurfaceKHR_ptr = (delegate* unmanaged<VkInstance, VkXlibSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateXlibSurfaceKHR));
        vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr = (delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceXlibPresentationSupportKHR));
    }

    public static VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
    {
        return vkCreateXlibSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, pSurface);
    }

    public static bool vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display, uint visualId)
    {
        return vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, display, visualId);
    }
}
