// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public static unsafe class Vma
{
    private const string LibName = "vma";

    static Vma()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    public static event DllImportResolver? ResolveLibrary;

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        if (libraryName.Equals(LibName) && TryResolveVMA(assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveVMA(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
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

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary != null)
        {
            var resolvers = resolveLibrary.GetInvocationList();

            foreach (DllImportResolver resolver in resolvers)
            {
                nativeLibrary = resolver(libraryName, assembly, searchPath);

                if (nativeLibrary != 0)
                {
                    return true;
                }
            }
        }

        nativeLibrary = 0;
        return false;
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAllocator")]
    public static extern VkResult vmaCreateAllocatorPrivate(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator);

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;

        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocatorPrivate(allocateInfo, out allocator);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaDestroyAllocator(VmaAllocator allocator);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetAllocatorInfo(VmaAllocator allocator, VmaAllocatorInfo* pAllocatorInfo);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetPhysicalDeviceProperties(VmaAllocator allocator, VkPhysicalDeviceProperties* ppPhysicalDeviceProperties);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetMemoryProperties(VmaAllocator allocator, VkPhysicalDeviceMemoryProperties* ppPhysicalDeviceMemoryProperties);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetMemoryTypeProperties(VmaAllocator allocator, uint memoryTypeIndex, VkMemoryPropertyFlags* pFlags);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaSetCurrentFrameIndex(VmaAllocator allocator, uint frameIndex);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaCalculateStatistics(VmaAllocator allocator, VmaTotalStatistics* pStats);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetHeapBudgets(VmaAllocator allocator, VmaBudget* pBudgets);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaFindMemoryTypeIndex(VmaAllocator allocator, uint memoryTypeBits, VmaAllocationCreateInfo* pAllocationCreateInfo, uint* pMemoryTypeIndex);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaFindMemoryTypeIndexForBufferInfo(VmaAllocator allocator, VkBufferCreateInfo* pBufferCreateInfo, VmaAllocationCreateInfo* pAllocationCreateInfo, uint* pMemoryTypeIndex);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaFindMemoryTypeIndexForImageInfo(VmaAllocator allocator, VkImageCreateInfo* pImageCreateInfo, VmaAllocationCreateInfo* pAllocationCreateInfo, uint* pMemoryTypeIndex);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreatePool(VmaAllocator allocator, VmaPoolCreateInfo* pCreateInfo, VmaPool* pPool);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaDestroyPool(VmaAllocator allocator, VmaPool pool);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetPoolStatistics(VmaAllocator allocator, VmaPool pool, VmaStatistics* pPoolStats);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaCalculatePoolStatistics(VmaAllocator allocator, VmaPool pool, VmaDetailedStatistics* pPoolStats);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCheckPoolCorruption(VmaAllocator allocator, VmaPool pool);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaSetPoolName(VmaAllocator allocator, VmaPool pool, sbyte* pName);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaAllocateMemory(VmaAllocator allocator,
        VkMemoryRequirements* pVkMemoryRequirements,
        VmaAllocationCreateInfo* pCreateInfo,
        VmaAllocation* pAllocation,
        VmaAllocationInfo* pAllocationInfo
        );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaAllocateMemoryPages(VmaAllocator allocator,
        VkMemoryRequirements* pVkMemoryRequirements,
        VmaAllocationCreateInfo* pCreateInfo,
        nuint allocationCount,
        VmaAllocation* pAllocation,
        VmaAllocationInfo* pAllocationInfo
        );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaAllocateMemoryForBuffer(VmaAllocator allocator,
        VkBuffer buffer,
        VmaAllocationCreateInfo* pCreateInfo,
        VmaAllocation* pAllocation,
        VmaAllocationInfo* pAllocationInfo
        );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaAllocateMemoryForImage(VmaAllocator allocator,
        VkImage image,
        VmaAllocationCreateInfo* pCreateInfo,
        VmaAllocation* pAllocation,
        VmaAllocationInfo* pAllocationInfo
        );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaFreeMemory(VmaAllocator allocator, VmaAllocation allocation);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaFreeMemoryPages(VmaAllocator allocator, nuint allocationCount, VmaAllocation* pAllocations);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetAllocationInfo(VmaAllocator allocator, VmaAllocation allocation, VmaAllocationInfo* pAllocationInfo);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaSetAllocationUserData(VmaAllocator allocator, VmaAllocation allocation, nint userData);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, sbyte* name);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetAllocationMemoryProperties(VmaAllocator allocator, VmaAllocation allocation, VkMemoryPropertyFlags* pFlags);

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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaMapMemory(VmaAllocator allocator, VmaAllocation allocation, void** ppData);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaUnmapMemory(VmaAllocator allocator, VmaAllocation allocation);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaFlushAllocation(VmaAllocator allocator, VmaAllocation allocation, ulong offset, ulong size);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaInvalidateAllocation(VmaAllocator allocator, VmaAllocation allocation, ulong offset, ulong size);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaFlushAllocations(VmaAllocator allocator, uint allocationCount, VmaAllocation* allocations, ulong* offsets, ulong* sizes);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaInvalidateAllocations(VmaAllocator allocator, uint allocationCount, VmaAllocation* allocations, ulong* offsets, ulong* sizes);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCheckCorruption(VmaAllocator allocator, uint memoryTypeBits);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaBindBufferMemory(VmaAllocator allocator, VmaAllocation allocation, VkBuffer buffer);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaBindBufferMemory2(VmaAllocator allocator, VmaAllocation allocation, ulong allocationLocalOffset, VkBuffer buffer, void* pNext);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaBindImageMemory(VmaAllocator allocator, VmaAllocation allocation, VkImage image);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaBindImageMemory2(VmaAllocator allocator, VmaAllocation allocation, ulong allocationLocalOffset, VkImage image, void* pNext);

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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaDestroyBuffer(VmaAllocator allocator, VkBuffer buffer, VmaAllocation allocation);

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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateAliasingImage(VmaAllocator allocator, VmaAllocation allocation, in VkImageCreateInfo imageCreateInfo, out VkImage image);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateAliasingImage2(VmaAllocator allocator, VmaAllocation allocation, ulong allocationLocalOffset, in VkImageCreateInfo imageCreateInfo, out VkImage image);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaDestroyImage(VmaAllocator allocator, VkImage image, VmaAllocation allocation);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaCreateVirtualBlock(VmaVirtualBlockCreateInfo* pCreateInfo, out VmaVirtualBlock virtualBlock);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaDestroyVirtualBlock(VmaVirtualBlock virtualBlock);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkBool32 vmaIsVirtualBlockEmpty(VmaVirtualBlock virtualBlock);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetVirtualAllocationInfo(VmaVirtualBlock virtualBlock, VmaVirtualAllocation allocation, VmaVirtualAllocationInfo* pVirtualAllocInfo);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VkResult vmaVirtualAllocate(VmaVirtualBlock virtualBlock, VmaVirtualAllocationCreateInfo* pCreateInfo, VmaVirtualAllocation* pAllocation, ulong* pOffset);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaVirtualFree(VmaVirtualBlock virtualBlock, VmaVirtualAllocation allocation);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaClearVirtualBlock(VmaVirtualBlock virtualBlock);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaSetVirtualAllocationUserData(VmaVirtualBlock virtualBlock, VmaVirtualAllocation allocation, nuint userData);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaGetVirtualBlockStatistics(VmaVirtualBlock virtualBlock, VmaStatistics* pStats);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vmaCalculateVirtualBlockStatistics(VmaVirtualBlock virtualBlock, VmaDetailedStatistics* pStats);
}
