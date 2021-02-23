// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
	[Flags]
	public enum VkWaylandSurfaceCreateFlagsKHR
	{
		None = 0,
	}

	public struct VkWaylandSurfaceCreateInfoKHR
	{
		public VkStructureType sType;
		public unsafe void* pNext;
		public VkWaylandSurfaceCreateFlagsKHR flags;
		public IntPtr display;
		public IntPtr surface;
	}

	public static partial class Vulkan
	{
		/// <summary>
		/// VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME = "VK_KHR_wayland_surface"
		/// </summary>
		public static readonly string KHRWaylandSurfaceExtensionName = "VK_KHR_wayland_surface";

		private static IntPtr vkCreateWaylandSurfaceKHR_ptr;
		[Calli]
		public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
		{
			throw new NotImplementedException();
		}

		private static IntPtr vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr;
		[Calli]
		public static unsafe uint vkGetPhysicalDeviceWaylandPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display)
		{
			throw new NotImplementedException();
		}
	}
}
