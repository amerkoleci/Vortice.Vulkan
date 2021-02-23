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

	public static partial class Vulkan
	{
		/// <summary>
		/// VK_KHR_XCB_SURFACE_EXTENSION_NAME = "VK_KHR_xcb_surface"
		/// </summary>
		public static readonly string KHRXcbSurfaceExtensionName = "VK_KHR_xcb_surface";

		private static IntPtr vkCreateXcbSurfaceKHR_ptr;
		[Calli]
		public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
		{
			throw new NotImplementedException();
		}

		private static IntPtr vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr;
		[Calli]
		public static unsafe uint vkGetPhysicalDeviceXcbPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr connection, uint visualId)
		{
			throw new NotImplementedException();
		}
	}
}
