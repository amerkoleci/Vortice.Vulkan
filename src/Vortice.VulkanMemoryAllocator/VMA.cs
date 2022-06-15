// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public static unsafe class VMA
{
    private static readonly IntPtr s_NativeLibrary = LoadNativeLibrary();

    private static readonly delegate* unmanaged[Cdecl]<VmaAllocatorCreateInfo*, out VmaAllocator, VkResult> vmaCreateAllocator_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, void> vmaDestroyAllocator_ptr;
    private static readonly delegate* unmanaged[Cdecl]<IntPtr, VkBufferCreateInfo*, VmaAllocationCreateInfo*, out VkBuffer, out VmaAllocation, VmaAllocationInfo*, VkResult> vmaCreateBuffer_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VkBuffer, VmaAllocation, void> vmaDestroyBuffer_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, sbyte*, void> vmaSetAllocationName_ptr;

    static VMA()
    {
        vmaCreateAllocator_ptr = (delegate* unmanaged[Cdecl]<VmaAllocatorCreateInfo*, out VmaAllocator, VkResult>)LoadFunction(nameof(vmaCreateAllocator));
        vmaDestroyAllocator_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, void>)LoadFunction(nameof(vmaDestroyAllocator));
        vmaCreateBuffer_ptr = (delegate* unmanaged[Cdecl]<IntPtr, VkBufferCreateInfo*, VmaAllocationCreateInfo*, out VkBuffer, out VmaAllocation, VmaAllocationInfo*, VkResult>)LoadFunction("vmaCreateBuffer");
        vmaDestroyBuffer_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VkBuffer, VmaAllocation, void>)LoadFunction(nameof(vmaDestroyBuffer));
        vmaSetAllocationName_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, sbyte*, void>)LoadFunction(nameof(vmaSetAllocationName));
    }

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;
        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocator_ptr(allocateInfo, out allocator);
    }

    public static void vmaDestroyAllocator(VmaAllocator allocator)
    {
        vmaDestroyAllocator_ptr(allocator);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateBuffer_ptr(allocator.Handle, pBufferCreateInfo, &allocationInfo, out buffer, out allocation, null);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateBuffer_ptr(allocator.Handle, pBufferCreateInfo, &allocationInfo, out buffer, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation)
    {
        return vmaCreateBuffer_ptr(allocator.Handle, pBufferCreateInfo, pAllocationCreateInfo, out buffer, out allocation, null);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        return vmaCreateBuffer_ptr(allocator.Handle, pBufferCreateInfo, pAllocationCreateInfo, out buffer, out allocation, pAllocationInfo);
    }

    public static void vmaDestroyBuffer(VmaAllocator allocator, VkBuffer buffer, VmaAllocation allocation)
    {
        vmaDestroyBuffer_ptr(allocator, buffer, allocation);
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, string name)
    {
        var data = Interop.GetUtf8Span(name);
        fixed (sbyte* dataPtr = data)
        {
            vmaSetAllocationName_ptr(allocator, allocation, dataPtr);
        }
    }


    private static IntPtr LoadNativeLibrary()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return LibraryLoader.LoadLocalLibrary("vma.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return LibraryLoader.LoadLocalLibrary("libvma.so");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return LibraryLoader.LoadLocalLibrary("libvma.dylib");
        }

        throw new PlatformNotSupportedException("VMA is not supported");
    }

    private static IntPtr LoadFunction(string name) => LibraryLoader.GetSymbol(s_NativeLibrary, name);
}
