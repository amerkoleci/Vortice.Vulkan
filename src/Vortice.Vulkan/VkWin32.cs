// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

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

    public static partial class Vulkan
    {
        /// <summary>
		/// VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface"
		/// </summary>
		public static readonly VkString KHRWin32SurfaceExtensionName = "VK_KHR_win32_surface";

        private static IntPtr vkCreateWin32SurfaceKHR_ptr;
        [Calli]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
        [Calli]
        public static unsafe VkBool32 vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
        {
            throw new NotImplementedException();
        }
    }
}
