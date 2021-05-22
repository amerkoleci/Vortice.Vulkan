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

    [StructLayout(LayoutKind.Sequential)]
    public struct VkImportMemoryWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public unsafe void* pNext;
        public VkExternalMemoryHandleTypeFlags handleType;
        public IntPtr handle;
        public IntPtr name;
    }

    public static unsafe partial class Vulkan
    {
        /// <summary>
        /// VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface"
        /// </summary>
        public static readonly string KHRWin32SurfaceExtensionName = "VK_KHR_win32_surface";

#if NETSTANDARD2_0
        private static delegate* unmanaged[Stdcall]<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateWin32SurfaceKHR_ptr;
        private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
#else
        private static delegate* unmanaged<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateWin32SurfaceKHR_ptr;
        private static delegate* unmanaged<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
#endif

        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            return vkCreateWin32SurfaceKHR_ptr(instance, pCreateInfo, pAllocator, out pSurface);
        }

        public static unsafe VkBool32 vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
        {
            return vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex);
        }
    }
}
