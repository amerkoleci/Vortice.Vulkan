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

	public static unsafe partial class Vulkan
	{
		/// <summary>
		/// VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME = "VK_KHR_wayland_surface"
		/// </summary>
		public static readonly string KHRWaylandSurfaceExtensionName = "VK_KHR_wayland_surface";

#if NET5_0_OR_GREATER
        private static delegate* unmanaged<VkInstance, VkWaylandSurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateWaylandSurfaceKHR_ptr;
		private static delegate* unmanaged<VkPhysicalDevice, uint, IntPtr, VkBool32> vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr;
#else
        private static delegate* unmanaged[Stdcall]<VkInstance, VkWaylandSurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateWaylandSurfaceKHR_ptr;
		private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, VkBool32> vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr;
#endif

        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
		{
            return vkCreateWaylandSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, out pSurface);
        }

		public static unsafe VkBool32 vkGetPhysicalDeviceWaylandPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display)
		{
            return vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, display);
        }
	}
}
