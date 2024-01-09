// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

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
    public IntPtr hinstance;
    public IntPtr hwnd;

    public VkWin32SurfaceCreateInfoKHR()
    {
        sType = VkStructureType.Win32SurfaceCreateInfoKHR;
    }
}

public unsafe struct VkImportMemoryWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkExternalMemoryHandleTypeFlags handleType;
    public nint handle;
    public nint name;

    public VkImportMemoryWin32HandleInfoKHR()
    {
        sType = VkStructureType.ImportMemoryWin32HandleInfoKHR;
    }
}

public unsafe struct VkExportMemoryWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public IntPtr* pAttributes;
    public uint dwAccess;
    public ushort* name;

    public VkExportMemoryWin32HandleInfoKHR()
    {
        sType = VkStructureType.ExportMemoryWin32HandleInfoKHR;
    }
}

public unsafe struct VkMemoryWin32HandlePropertiesKHR
{
    public VkStructureType sType;
    public void* pNext;
    public uint memoryTypeBits;

    public VkMemoryWin32HandlePropertiesKHR()
    {
        sType = VkStructureType.MemoryWin32HandlePropertiesKHR;
    }
}

public unsafe struct VkMemoryGetWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkDeviceMemory memory;
    public VkExternalMemoryHandleTypeFlags handleType;

    public VkMemoryGetWin32HandleInfoKHR()
    {
        sType = VkStructureType.MemoryGetWin32HandleInfoKHR;
    }
}

public unsafe struct VkWin32KeyedMutexAcquireReleaseInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public uint acquireCount;
    public VkDeviceMemory* pAcquireSyncs;
    public ulong* pAcquireKeys;
    public uint* pAcquireTimeouts;
    public uint releaseCount;
    public VkDeviceMemory* pReleaseSyncs;
    public ulong* pReleaseKeys;

    public VkWin32KeyedMutexAcquireReleaseInfoKHR()
    {
        sType = VkStructureType.Win32KeyedMutexAcquireReleaseInfoKHR;
    }
}

public unsafe struct VkImportSemaphoreWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkSemaphore semaphore;
    public VkSemaphoreImportFlags flags;
    public VkExternalSemaphoreHandleTypeFlags handleType;
    public IntPtr handle;
    public ushort* name;

    public VkImportSemaphoreWin32HandleInfoKHR()
    {
        sType = VkStructureType.ImportSemaphoreWin32HandleInfoKHR;
    }
}

public unsafe struct VkExportSemaphoreWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public IntPtr pAttributes;
    public uint dwAccess;
    public ushort* name;

    public VkExportSemaphoreWin32HandleInfoKHR()
    {
        sType = VkStructureType.ExportSemaphoreWin32HandleInfoKHR;
    }
}

public unsafe struct VkD3D12FenceSubmitInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public uint waitSemaphoreValuesCount;
    public ulong* pWaitSemaphoreValues;
    public uint signalSemaphoreValuesCount;
    public ulong* pSignalSemaphoreValues;

    public VkD3D12FenceSubmitInfoKHR()
    {
        sType = VkStructureType.D3D12FenceSubmitInfoKHR;
    }
}

public unsafe partial struct VkSemaphoreGetWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkSemaphore semaphore;
    public VkExternalSemaphoreHandleTypeFlags handleType;

    public VkSemaphoreGetWin32HandleInfoKHR()
    {
        sType = VkStructureType.SemaphoreGetWin32HandleInfoKHR;
    }
}

public unsafe partial struct VkImportFenceWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkFence fence;
    public VkFenceImportFlags flags;
    public VkExternalFenceHandleTypeFlags handleType;
    public IntPtr handle;
    public ushort* name;

    public VkImportFenceWin32HandleInfoKHR()
    {
        sType = VkStructureType.ImportFenceWin32HandleInfoKHR;
    }
}

public unsafe partial struct VkFenceGetWin32HandleInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkFence fence;
    public VkExternalFenceHandleTypeFlags handleType;

    public VkFenceGetWin32HandleInfoKHR()
    {
        sType = VkStructureType.FenceGetWin32HandleInfoKHR;
    }
}

public unsafe partial struct VkImportMemoryWin32HandleInfoNV
{
    public VkStructureType sType;
    public void* pNext;
    public VkExternalMemoryHandleTypeFlagsNV handleType;
    public IntPtr handle;

    public VkImportMemoryWin32HandleInfoNV()
    {
        sType = VkStructureType.ImportMemoryWin32HandleInfoNV;
    }
}

public unsafe partial struct VkExportMemoryWin32HandleInfoNV
{
    public VkStructureType sType;
    public void* pNext;
    public IntPtr pAttributes;
    public uint dwAccess;

    public VkExportMemoryWin32HandleInfoNV()
    {
        sType = VkStructureType.ExportMemoryWin32HandleInfoNV;
    }
}

public unsafe partial struct VkWin32KeyedMutexAcquireReleaseInfoNV
{
    public VkStructureType sType;
    public void* pNext;
    public uint acquireCount;
    public VkDeviceMemory* pAcquireSyncs;
    public ulong* pAcquireKeys;
    public uint* pAcquireTimeoutMilliseconds;
    public uint releaseCount;
    public VkDeviceMemory* pReleaseSyncs;
    public ulong* pReleaseKeys;

    public VkWin32KeyedMutexAcquireReleaseInfoNV()
    {
        sType = VkStructureType.Win32KeyedMutexAcquireReleaseInfoNV;
    }
}

public enum VkFullScreenExclusiveEXT
{
    Default = 0,
    Allowed = 1,
    Disallowed = 2,
    ApplicationControlled = 3,
}

public partial struct VkSurfaceFullScreenExclusiveInfoEXT
{
    public VkStructureType sType;

    public unsafe void* pNext;

    public VkFullScreenExclusiveEXT fullScreenExclusive;

    public VkSurfaceFullScreenExclusiveInfoEXT()
    {
        sType = VkStructureType.SurfaceFullScreenExclusiveInfoEXT;
    }
}

public partial struct VkSurfaceCapabilitiesFullScreenExclusiveEXT
{
    public VkStructureType sType;
    public unsafe void* pNext;
    public VkBool32 fullScreenExclusiveSupported;

    public VkSurfaceCapabilitiesFullScreenExclusiveEXT()
    {
        sType = VkStructureType.SurfaceCapabilitiesFullScreenExclusiveEXT;
    }
}

public partial struct VkSurfaceFullScreenExclusiveWin32InfoEXT
{
    public VkStructureType sType;
    public unsafe void* pNext;
    public IntPtr hmonitor;

    public VkSurfaceFullScreenExclusiveWin32InfoEXT()
    {
        sType = VkStructureType.SurfaceFullScreenExclusiveWin32InfoEXT;
    }
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


    private static delegate* unmanaged<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateWin32SurfaceKHR_ptr;
    private static delegate* unmanaged<VkPhysicalDevice, uint, VkBool32> vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult> vkGetMemoryWin32HandleKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult> vkGetMemoryWin32HandlePropertiesKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkImportSemaphoreWin32HandleInfoKHR*, VkResult> vkImportSemaphoreWin32HandleKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkSemaphoreGetWin32HandleInfoKHR*, IntPtr*, VkResult> vkGetSemaphoreWin32HandleKHR_ptr;

    private static delegate* unmanaged<VkDevice, VkImportFenceWin32HandleInfoKHR*, VkResult> vkImportFenceWin32HandleKHR_ptr;
    private static delegate* unmanaged<VkDevice, VkFenceGetWin32HandleInfoKHR*, IntPtr*, VkResult> vkGetFenceWin32HandleKHR_ptr;

    private static delegate* unmanaged<VkPhysicalDevice, VkPhysicalDeviceSurfaceInfo2KHR*, uint*, VkPresentModeKHR*, VkResult> vkGetPhysicalDeviceSurfacePresentModes2EXT_ptr;

    private static delegate* unmanaged<VkDevice, VkSwapchainKHR, VkResult> vkAcquireFullScreenExclusiveModeEXT_ptr;
    private static delegate* unmanaged<VkDevice, VkSwapchainKHR, VkResult> vkReleaseFullScreenExclusiveModeEXT_ptr;
    private static delegate* unmanaged<VkDevice, VkPhysicalDeviceSurfaceInfo2KHR*, VkDeviceGroupPresentModeFlagsKHR*, VkResult> vkGetDeviceGroupSurfacePresentModes2EXT_ptr;

    private static void LoadWin32(VkInstance instance)
    {
        vkCreateWin32SurfaceKHR_ptr = (delegate* unmanaged<VkInstance, VkWin32SurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateWin32SurfaceKHR));
        vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = (delegate* unmanaged<VkPhysicalDevice, uint, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceWin32PresentationSupportKHR));
        vkGetPhysicalDeviceSurfacePresentModes2EXT_ptr = (delegate* unmanaged<VkPhysicalDevice, VkPhysicalDeviceSurfaceInfo2KHR*, uint*, VkPresentModeKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceSurfacePresentModes2EXT));
    }

    private static void LoadWin32(VkDevice device)
    {
        vkGetMemoryWin32HandleKHR_ptr = (delegate* unmanaged<VkDevice, VkMemoryGetWin32HandleInfoKHR*, void*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkGetMemoryWin32HandleKHR));
        vkGetMemoryWin32HandlePropertiesKHR_ptr = (delegate* unmanaged<VkDevice, VkExternalMemoryHandleTypeFlags, void*, VkMemoryWin32HandlePropertiesKHR*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkGetMemoryWin32HandlePropertiesKHR));

        vkImportSemaphoreWin32HandleKHR_ptr = (delegate* unmanaged<VkDevice, VkImportSemaphoreWin32HandleInfoKHR*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkImportSemaphoreWin32HandleKHR));
        vkGetSemaphoreWin32HandleKHR_ptr = (delegate* unmanaged<VkDevice, VkSemaphoreGetWin32HandleInfoKHR*, IntPtr*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkGetSemaphoreWin32HandleKHR));

        vkImportFenceWin32HandleKHR_ptr = (delegate* unmanaged<VkDevice, VkImportFenceWin32HandleInfoKHR*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkImportFenceWin32HandleKHR));
        vkGetFenceWin32HandleKHR_ptr = (delegate* unmanaged<VkDevice, VkFenceGetWin32HandleInfoKHR*, IntPtr*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkGetFenceWin32HandleKHR));

        vkAcquireFullScreenExclusiveModeEXT_ptr = (delegate* unmanaged<VkDevice, VkSwapchainKHR, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkAcquireFullScreenExclusiveModeEXT));
        vkReleaseFullScreenExclusiveModeEXT_ptr = (delegate* unmanaged<VkDevice, VkSwapchainKHR, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkReleaseFullScreenExclusiveModeEXT));
        vkGetDeviceGroupSurfacePresentModes2EXT_ptr = (delegate* unmanaged<VkDevice, VkPhysicalDeviceSurfaceInfo2KHR*, VkDeviceGroupPresentModeFlagsKHR*, VkResult>)vkGetDeviceProcAddr(device.Handle, nameof(vkGetDeviceGroupSurfacePresentModes2EXT));
    }

    public static VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
    {
        return vkCreateWin32SurfaceKHR_ptr(instance, pCreateInfo, pAllocator, pSurface);
    }

    public static bool vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
    {
        return vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex);
    }

    public static VkResult vkGetMemoryWin32HandleKHR(VkDevice device, VkMemoryGetWin32HandleInfoKHR* pGetWin32HandleInfo, void* pHandle)
    {
        return vkGetMemoryWin32HandleKHR_ptr(device, pGetWin32HandleInfo, pHandle);
    }

    public static VkResult vkGetMemoryWin32HandlePropertiesKHR(VkDevice device, VkExternalMemoryHandleTypeFlags handleType, void* handle, VkMemoryWin32HandlePropertiesKHR* pMemoryWin32HandleProperties)
    {
        return vkGetMemoryWin32HandlePropertiesKHR_ptr(device, handleType, handle, pMemoryWin32HandleProperties);
    }

    public static VkResult vkImportSemaphoreWin32HandleKHR(VkDevice device, VkImportSemaphoreWin32HandleInfoKHR* pImportSemaphoreWin32HandleInfos)
    {
        return vkImportSemaphoreWin32HandleKHR_ptr(device, pImportSemaphoreWin32HandleInfos);
    }

    public static VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, VkSemaphoreGetWin32HandleInfoKHR* pGetWin32HandleInfo, IntPtr* pHandle)
    {
        return vkGetSemaphoreWin32HandleKHR_ptr(device, pGetWin32HandleInfo, pHandle);
    }

    public static VkResult vkImportFenceWin32HandleKHR(VkDevice device, VkImportFenceWin32HandleInfoKHR* pImportFenceWin32HandleInfo)
    {
        return vkImportFenceWin32HandleKHR_ptr(device, pImportFenceWin32HandleInfo);
    }

    public static VkResult vkGetFenceWin32HandleKHR(VkDevice device, VkFenceGetWin32HandleInfoKHR* pGetWin32HandleInfo, IntPtr* pHandle)
    {
        return vkGetFenceWin32HandleKHR_ptr(device, pGetWin32HandleInfo, pHandle);
    }

    public static VkResult vkGetPhysicalDeviceSurfacePresentModes2EXT(
        VkPhysicalDevice physicalDevice,
        VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo,
        uint* pPresentModeCount,
        VkPresentModeKHR* pPresentModes)
    {
        return vkGetPhysicalDeviceSurfacePresentModes2EXT_ptr(physicalDevice, pSurfaceInfo, pPresentModeCount, pPresentModes);
    }

    public static VkResult vkAcquireFullScreenExclusiveModeEXT(VkDevice device, VkSwapchainKHR swapchain)
    {
        return vkAcquireFullScreenExclusiveModeEXT_ptr(device, swapchain);
    }

    public static VkResult vkReleaseFullScreenExclusiveModeEXT(VkDevice device, VkSwapchainKHR swapchain)
    {
        return vkReleaseFullScreenExclusiveModeEXT_ptr(device, swapchain);
    }

    public static VkResult vkGetDeviceGroupSurfacePresentModes2EXT(VkDevice device, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, VkDeviceGroupPresentModeFlagsKHR* pModes)
    {
        return vkGetDeviceGroupSurfacePresentModes2EXT_ptr(device, pSurfaceInfo, pModes);
    }
}
