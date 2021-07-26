// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    public unsafe partial struct SECURITY_ATTRIBUTES
    {
        public uint nLength;

        public void* lpSecurityDescriptor;

        public VkBool32 bInheritHandle;
    }

    [Flags]
    public enum VkWin32SurfaceCreateFlagsKHR
    {
        None = 0,
    }

    public unsafe struct VkWin32SurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkWin32SurfaceCreateFlagsKHR flags;
        public nint hinstance;
        public nint hwnd;
    }

    public unsafe struct VkImportMemoryWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlags handleType;
        public nint handle;
        public nint name;
    }

    public unsafe struct VkExportMemoryWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public SECURITY_ATTRIBUTES* pAttributes;
        public uint dwAccess;
        public nint name;
    }

    public unsafe struct VkMemoryWin32HandlePropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint memoryTypeBits;
    }

    public unsafe struct VkMemoryGetWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceMemory memory;
        public VkExternalMemoryHandleTypeFlags handleType;
    }

    public static unsafe partial class Vulkan
    {
        /// <summary>
        /// VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface"
        /// </summary>
        public static readonly string KHRWin32SurfaceExtensionName = "VK_KHR_win32_surface";

#if NET5_0_OR_GREATER
        private static delegate* unmanaged<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateWin32SurfaceKHR_ptr;
        private static delegate* unmanaged<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
        private static delegate* unmanaged<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult> vkGetMemoryWin32HandleKHR_ptr;
        private static delegate* unmanaged<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult> vkGetMemoryWin32HandlePropertiesKHR_ptr;
#else
        private static delegate* unmanaged[Stdcall]<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, out VkSurfaceKHR, VkResult> vkCreateWin32SurfaceKHR_ptr;
        private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
        private static delegate* unmanaged[Stdcall]<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult> vkGetMemoryWin32HandleKHR_ptr;
        private static delegate* unmanaged[Stdcall]<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult> vkGetMemoryWin32HandlePropertiesKHR_ptr;
#endif

        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            return vkCreateWin32SurfaceKHR_ptr(instance, pCreateInfo, pAllocator, out pSurface);
        }

        public static unsafe VkBool32 vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
        {
            return vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex);
        }

        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, VkMemoryGetWin32HandleInfoKHR* pGetWin32HandleInfo, void* pHandle)
        {
            return vkGetMemoryWin32HandleKHR_ptr(device, pGetWin32HandleInfo, pHandle);
        }

        public static unsafe VkResult vkGetMemoryWin32HandlePropertiesKHR(VkDevice device, VkExternalMemoryHandleTypeFlags handleType, void* handle, VkMemoryWin32HandlePropertiesKHR* pMemoryWin32HandleProperties)
        {
            return vkGetMemoryWin32HandlePropertiesKHR_ptr(device, handleType, handle, pMemoryWin32HandleProperties);
        }
    }
}
