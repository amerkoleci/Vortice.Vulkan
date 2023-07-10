// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

unsafe partial class Vma
{
    private const string LibName = "vma";

    static Vma()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName.Equals(LibName) && TryResolveVMA(assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        return 0;
    }

    private static bool TryResolveVMA(Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
    {
        if (OperatingSystem.IsWindows())
        {
            if (NativeLibrary.TryLoad("vma.dll", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            if (NativeLibrary.TryLoad("libvma.so", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            if (NativeLibrary.TryLoad("libvma.dylib", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }

        if (NativeLibrary.TryLoad("libvma", assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        if (NativeLibrary.TryLoad("vma", assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        return false;
    }

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, VmaAllocator* allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;

        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocatorPrivate(allocateInfo, allocator);
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* namePtr = name)
        {
            vmaSetAllocationName(allocator, allocation, namePtr);
        }
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, string name)
    {
        ReadOnlySpan<sbyte> data = Interop.GetUtf8Span(name);
        fixed (sbyte* namePtr = data)
        {
            vmaSetAllocationName(allocator, allocation, namePtr);
        }
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
        return vmaCreateBuffer(allocator, pBufferCreateInfo, &allocationInfo, out buffer, out allocation, null);
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
        return vmaCreateBuffer(allocator, pBufferCreateInfo, &allocationInfo, out buffer, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation)
    {
        return vmaCreateBuffer(allocator, pBufferCreateInfo, pAllocationCreateInfo, out buffer, out allocation, null);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateBufferWithAlignment(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        ulong minAlignment,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateAliasingBuffer(
        VmaAllocator allocator,
        VmaAllocation allocation,
        VkBufferCreateInfo* pBufferCreateInfo,
        out VkBuffer buffer);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateAliasingBuffer2(
        VmaAllocator allocator,
        VmaAllocation allocation,
        ulong allocationLocalOffset,
        VkBufferCreateInfo* pBufferCreateInfo,
        out VkBuffer buffer);

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        out VkImage image,
        out VmaAllocation allocation)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateImage(allocator, pImageCreateInfo, &allocationInfo, out image, out allocation, null);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateImage(allocator, pImageCreateInfo, &allocationInfo, out image, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkImage image,
        out VmaAllocation allocation)
    {
        return vmaCreateImage(allocator, pImageCreateInfo, pAllocationCreateInfo, out image, out allocation, null);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo);
}
