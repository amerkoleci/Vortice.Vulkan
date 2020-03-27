// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    [Flags]
    public enum VkWin32SurfaceCreateFlagsKHR
    {
        None = 0,
    }

    public struct VkWin32SurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public unsafe void* pNext;
        public VkWin32SurfaceCreateFlagsKHR flags;
        public IntPtr hinstance;
        public IntPtr hwnd;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate VkResult vkCreateWin32SurfaceKHRDelegate(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate VkBool32 vkGetPhysicalDeviceWin32PresentationSupportKHRDelegate(VkPhysicalDevice physicalDevice, uint queueFamilyIndex);

    public static partial class Vulkan
    {
        /// <summary>
		/// VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface"
		/// </summary>
		public static readonly VkString KHRWin32SurfaceExtensionName = "VK_KHR_win32_surface";

        private static vkCreateWin32SurfaceKHRDelegate vkCreateWin32SurfaceKHR_ptr;
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            return vkCreateWin32SurfaceKHR_ptr(instance, pCreateInfo, pAllocator, out pSurface);
        }

        private static vkGetPhysicalDeviceWin32PresentationSupportKHRDelegate vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
        public static unsafe VkBool32 vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
        {
            return vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex);
        }
    }
}
