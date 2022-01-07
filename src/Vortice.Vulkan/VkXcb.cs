// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VkXcbSurfaceCreateFlagsKHR
{
    None = 0,
}

public unsafe struct VkXcbSurfaceCreateInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkXcbSurfaceCreateFlagsKHR flags;
    public IntPtr connection;
    public uint window;
}

public static unsafe partial class Vulkan
{
    public static readonly string VK_KHR_XCB_SURFACE_EXTENSION_NAME = "VK_KHR_xcb_surface";

    /// <summary>
    /// VK_KHR_XCB_SURFACE_EXTENSION_NAME = "VK_KHR_xcb_surface"
    /// </summary>
    public static readonly string KHRXcbSurfaceExtensionName = "VK_KHR_xcb_surface";

#if NET5_0_OR_GREATER
    private static delegate* unmanaged<VkInstance, VkXcbSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateXcbSurfaceKHR_ptr;
    private static delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr;
#else
    private static delegate* unmanaged[Stdcall]<VkInstance, VkXcbSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateXcbSurfaceKHR_ptr;
    private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr;
#endif

    private static void LoadXcb(VkInstance instance)
    {
#if NET5_0_OR_GREATER
        vkCreateXcbSurfaceKHR_ptr = (delegate* unmanaged<VkInstance, VkXcbSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateXcbSurfaceKHR));
        vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr = (delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceXcbPresentationSupportKHR));
#else
        vkCreateXcbSurfaceKHR_ptr = (delegate* unmanaged[Stdcall]<VkInstance, VkXcbSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateXcbSurfaceKHR));
        vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr = (delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, uint, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceXcbPresentationSupportKHR));
#endif
    }

    public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
    {
        return vkCreateXcbSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, pSurface);
    }

    public static unsafe bool vkGetPhysicalDeviceXcbPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr connection, uint visualId)
    {
        return vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, connection, visualId);
    }
}
