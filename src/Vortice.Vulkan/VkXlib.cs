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

	public static partial class Vulkan
	{
		/// <summary>
		/// VK_KHR_XLIB_SURFACE_EXTENSION_NAME = "VK_KHR_xlib_surface"
		/// </summary>
		public static readonly VkString KHRXlibSurfaceExtensionName = "VK_KHR_xlib_surface";

		private static IntPtr vkCreateXlibSurfaceKHR_ptr;
		[Calli]
		public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
		{
			throw new NotImplementedException();
		}

		private static IntPtr vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr;
		[Calli]
		public static unsafe uint vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display, uint visualId)
		{
			throw new NotImplementedException();
		}
	}
}
