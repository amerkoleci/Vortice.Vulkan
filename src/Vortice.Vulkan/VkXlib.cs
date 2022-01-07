// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VkXlibSurfaceCreateFlagsKHR
{
    None = 0,
}

public unsafe struct VkXlibSurfaceCreateInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkXcbSurfaceCreateFlagsKHR flags;
    public IntPtr display;
    public nuint window;
}

public static unsafe partial class Vulkan
{
    public static readonly string VK_KHR_XLIB_SURFACE_EXTENSION_NAME = "VK_KHR_xlib_surface";

    /// <summary>
    /// VK_KHR_XLIB_SURFACE_EXTENSION_NAME = "VK_KHR_xlib_surface"
    /// </summary>
    public static readonly string KHRXlibSurfaceExtensionName = "VK_KHR_xlib_surface";

#if NET5_0_OR_GREATER
    private static delegate* unmanaged<VkInstance, VkXlibSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateXlibSurfaceKHR_ptr;
    private static delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr;
#else
    private static delegate* unmanaged[Stdcall]<VkInstance, VkXlibSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateXlibSurfaceKHR_ptr;
	private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr;
#endif

    public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
    {
        return vkCreateXlibSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, pSurface);
    }

    public static unsafe bool vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display, uint visualId)
    {
        return vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, display, visualId);
    }
}
