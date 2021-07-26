// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    [Flags]
    public enum VkXcbSurfaceCreateFlagsKHR
    {
        None = 0,
    }

    public struct VkXcbSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public unsafe void* pNext;
        public VkXcbSurfaceCreateFlagsKHR flags;
        public IntPtr connection;
        public uint window;
    }

    public static unsafe partial class Vulkan
    {
        /// <summary>
        /// VK_KHR_XCB_SURFACE_EXTENSION_NAME = "VK_KHR_xcb_surface"
        /// </summary>
        public static readonly string KHRXcbSurfaceExtensionName = "VK_KHR_xcb_surface";

#if NET5_0_OR_GREATER
        private static delegate* unmanaged<VkInstance, VkXcbSurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateXcbSurfaceKHR_ptr;
        private static delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr;
#else
        private static delegate* unmanaged[Stdcall]<VkInstance, VkXcbSurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateXcbSurfaceKHR_ptr;
        private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr;
#endif

        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            return vkCreateXcbSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, out pSurface);
        }

        public static unsafe VkBool32 vkGetPhysicalDeviceXcbPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr connection, uint visualId)
        {
            return vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, connection, visualId);
        }
    }
}
