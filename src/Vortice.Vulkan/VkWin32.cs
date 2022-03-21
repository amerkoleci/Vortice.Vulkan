// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

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
    public static readonly string VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface";
    public static readonly string VK_KHR_EXTERNAL_MEMORY_WIN32_EXTENSION_NAME = "VK_KHR_external_memory_win32";
    public static readonly string VK_KHR_WIN32_KEYED_MUTEX_EXTENSION_NAME = "VK_KHR_win32_keyed_mutex";
    public static readonly string VK_KHR_EXTERNAL_SEMAPHORE_WIN32_EXTENSION_NAME = "VK_KHR_external_semaphore_win32";
    public static readonly string VK_KHR_EXTERNAL_FENCE_WIN32_EXTENSION_NAME = "VK_KHR_external_fence_win32";
    public static readonly string VK_NV_EXTERNAL_MEMORY_WIN32_EXTENSION_NAME = "VK_NV_external_memory_win32";
    public static readonly string VK_NV_WIN32_KEYED_MUTEX_EXTENSION_NAME = "VK_NV_win32_keyed_mutex";
    public static readonly string VK_EXT_FULL_SCREEN_EXCLUSIVE_EXTENSION_NAME = "VK_EXT_full_screen_exclusive";

    /// <summary>
    /// VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface"
    /// </summary>
    public static readonly string KHRWin32SurfaceExtensionName = "VK_KHR_win32_surface";

#if NET5_0_OR_GREATER
    private static delegate* unmanaged<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateWin32SurfaceKHR_ptr;
    private static delegate* unmanaged<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult> vkGetMemoryWin32HandleKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult> vkGetMemoryWin32HandlePropertiesKHR_ptr;
#else
    private static delegate* unmanaged[Stdcall]<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateWin32SurfaceKHR_ptr;
    private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
    private static delegate* unmanaged[Stdcall]<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult> vkGetMemoryWin32HandleKHR_ptr;
    private static delegate* unmanaged[Stdcall]<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult> vkGetMemoryWin32HandlePropertiesKHR_ptr;
#endif

    private static void LoadWin32(VkInstance instance)
    {
#if NET5_0_OR_GREATER
        vkCreateWin32SurfaceKHR_ptr = (delegate* unmanaged<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateWin32SurfaceKHR));
        vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = (delegate* unmanaged<VkPhysicalDevice, uint, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceWin32PresentationSupportKHR));

        vkGetMemoryWin32HandleKHR_ptr = (delegate* unmanaged<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetMemoryWin32HandleKHR));
        vkGetMemoryWin32HandlePropertiesKHR_ptr = (delegate* unmanaged<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetMemoryWin32HandlePropertiesKHR));
#else
        vkCreateWin32SurfaceKHR_ptr = (delegate* unmanaged[Stdcall]<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateWin32SurfaceKHR));
        vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = (delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceWin32PresentationSupportKHR));

        vkGetMemoryWin32HandleKHR_ptr = (delegate* unmanaged[Stdcall]<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetMemoryWin32HandleKHR));
        vkGetMemoryWin32HandlePropertiesKHR_ptr = (delegate* unmanaged[Stdcall]<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetMemoryWin32HandlePropertiesKHR));
#endif
    }

    public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
    {
        return vkCreateWin32SurfaceKHR_ptr(instance, pCreateInfo, pAllocator, pSurface);
    }

    public static unsafe bool vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
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
