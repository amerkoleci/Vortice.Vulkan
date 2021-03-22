// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
	[Flags]
	public enum VkXlibSurfaceCreateFlagsKHR
	{
		None = 0,
	}

	public struct VkXlibSurfaceCreateInfoKHR
	{
		public VkStructureType sType;
		public unsafe void* pNext;
		public VkXcbSurfaceCreateFlagsKHR flags;
		public IntPtr display;
		public IntPtr window;
	}

	public static unsafe partial class Vulkan
	{
		/// <summary>
		/// VK_KHR_XLIB_SURFACE_EXTENSION_NAME = "VK_KHR_xlib_surface"
		/// </summary>
		public static readonly string KHRXlibSurfaceExtensionName = "VK_KHR_xlib_surface";

		private static delegate* unmanaged<VkInstance, VkXlibSurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateXlibSurfaceKHR_ptr;
		private static delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, uint, VkBool32> vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr;

		public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
		{
            return vkCreateXlibSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, out pSurface);
        }

		public static unsafe VkBool32 vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display, uint visualId)
		{
            return vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, display, visualId);
        }
	}
}
