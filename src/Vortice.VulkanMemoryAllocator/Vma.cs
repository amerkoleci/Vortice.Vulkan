// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

unsafe partial class Vma
{
    private const string LibName = "vma";
    private static nint s_nativeLibrary;

    public static VkResult vmaInitialize(string? libraryName = default)
    {
        if (!string.IsNullOrEmpty(libraryName))
        {
            if (NativeLibrary.TryLoad(libraryName, out s_nativeLibrary))
            {
                return VkResult.Success;
            }
        }

        if (OperatingSystem.IsWindows())
        {
            s_nativeLibrary = NativeLibrary.Load("vma.dll");
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            s_nativeLibrary = NativeLibrary.Load("libvma.dylib");
        }
        else if (OperatingSystem.IsLinux())
        {
            s_nativeLibrary = NativeLibrary.Load("libvma.so");
        }
        else
        {
            if (!NativeLibrary.TryLoad("libvma", out s_nativeLibrary))
            {
                if (NativeLibrary.TryLoad("vma", out s_nativeLibrary))
                {
                }
            }
        }

        if (s_nativeLibrary == 0)
            return VkResult.ErrorInitializationFailed;

        LoadEntries();

        return VkResult.Success;
    }

    [SkipLocalsInit]
    public static VkResult vmaCreateAllocator(in VmaAllocatorCreateInfo createInfo, out VmaAllocator allocator)
    {
        Unsafe.SkipInit(out allocator);

        if (createInfo.pVulkanFunctions == null)
        {
            VmaVulkanFunctions functions = default;
            functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
            functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;

            fixed (VmaAllocator* allocatorPtr = &allocator)
            {
                VmaAllocatorCreateInfo createInfoIn = createInfo;
                createInfoIn.pVulkanFunctions = &functions;
                return vmaCreateAllocatorPrivate(&createInfoIn, allocatorPtr);
            }
        }

        fixed (VmaAllocatorCreateInfo* createInfoPtr = &createInfo)
        fixed (VmaAllocator* allocatorPtr = &allocator)
            return vmaCreateAllocatorPrivate(createInfoPtr, allocatorPtr);
    }

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;

        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocatorPrivate(allocateInfo, out allocator);
    }


    public static void vmaGetPhysicalDeviceProperties(VmaAllocator allocator, out VkPhysicalDeviceProperties* physicalDeviceProperties)
    {
        fixed (VkPhysicalDeviceProperties** physicalDevicePropertiesPtr = &physicalDeviceProperties)
        {
            vmaGetPhysicalDeviceProperties_ptr(allocator, physicalDevicePropertiesPtr);
        }
    }


    public static void vmaGetMemoryProperties(VmaAllocator allocator, out VkPhysicalDeviceMemoryProperties* physicalDeviceMemoryProperties)
    {
        fixed (VkPhysicalDeviceMemoryProperties** physicalDeviceMemoryPropertiesPtr = &physicalDeviceMemoryProperties)
        {
            vmaGetMemoryProperties_ptr(allocator, physicalDeviceMemoryPropertiesPtr);
        }
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, ReadOnlySpan<byte> name)
    {
        fixed (byte* namePtr = name)
        {
            vmaSetAllocationName(allocator, allocation, namePtr);
        }
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, ReadOnlySpanUtf8 name)
    {
        byte* namePtr = name;
        vmaSetAllocationName(allocator, allocation, namePtr);
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, string name)
    {
        vmaSetAllocationName(allocator, allocation, new ReadOnlySpanUtf8(Encoding.UTF8.GetBytes(name)));
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        in VkBufferCreateInfo bufferCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateBuffer(allocator, in bufferCreateInfo, in allocationInfo, out buffer, out allocation, null);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        in VkBufferCreateInfo bufferCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateBuffer(allocator, in bufferCreateInfo, in allocationInfo, out buffer, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        in VkBufferCreateInfo bufferCreateInfo,
        in VmaAllocationCreateInfo allocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo = default)
    {
        Unsafe.SkipInit(out buffer);
        Unsafe.SkipInit(out allocation);

        fixed (VkBufferCreateInfo* bufferCreateInfoPtr = &bufferCreateInfo)
        fixed (VmaAllocationCreateInfo* allocationCreateInfoPtr = &allocationCreateInfo)
        fixed (VkBuffer* bufferPtr = &buffer)
        fixed (VmaAllocation* allocationPtr = &allocation)
            return vmaCreateBuffer_ptr(allocator, bufferCreateInfoPtr, allocationCreateInfoPtr, bufferPtr, allocationPtr, pAllocationInfo);
    }

    public static VkResult vmaCreateBufferWithAlignment(
        VmaAllocator allocator,
        in VkBufferCreateInfo bufferCreateInfo,
        in VmaAllocationCreateInfo allocationCreateInfo,
        ulong minAlignment,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo = default)
    {
        Unsafe.SkipInit(out buffer);
        Unsafe.SkipInit(out allocation);

        fixed (VkBufferCreateInfo* bufferCreateInfoPtr = &bufferCreateInfo)
        fixed (VmaAllocationCreateInfo* allocationCreateInfoPtr = &allocationCreateInfo)
        fixed (VkBuffer* bufferPtr = &buffer)
        fixed (VmaAllocation* allocationPtr = &allocation)
            return vmaCreateBufferWithAlignment_ptr(allocator, bufferCreateInfoPtr, allocationCreateInfoPtr, minAlignment, bufferPtr, allocationPtr, pAllocationInfo);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        in VkImageCreateInfo imageCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo = default)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateImage(allocator, in imageCreateInfo, in allocationInfo, out image, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        in VkImageCreateInfo imageCreateInfo,
        in VmaAllocationCreateInfo allocationCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo = default)
    {
        Unsafe.SkipInit(out image);
        Unsafe.SkipInit(out allocation);

        fixed (VkImageCreateInfo* imageCreateInfoPtr = &imageCreateInfo)
        fixed (VmaAllocationCreateInfo* allocationCreateInfoPtr = &allocationCreateInfo)
        fixed (VkImage* imagePtr = &image)
        fixed (VmaAllocation* allocationPtr = &allocation)
            return vmaCreateImage_ptr(allocator, imageCreateInfoPtr, allocationCreateInfoPtr, imagePtr, allocationPtr, pAllocationInfo);
    }
}
