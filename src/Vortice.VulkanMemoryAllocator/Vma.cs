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
    private const DllImportSearchPath DefaultDllImportSearchPath = DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UserDirectories | DllImportSearchPath.UseDllDirectoryForDependencies;

    /// <summary>
    /// Raised whenever a native library is loaded by VMA. Handlers can be added to this event to customize how libraries are loaded, and they will be used first whenever a new native library is being resolved.
    /// </summary>
    public static event DllImportResolver? VmaDllImporterResolver;

    private const string LibName = "vma";

    static Vma()
    {
        NativeLibrary.SetDllImportResolver(typeof(Vma).Assembly, OnDllImport);
    }

    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != LibName)
        {
            return IntPtr.Zero;
        }

        IntPtr nativeLibrary = IntPtr.Zero;
        DllImportResolver? resolver = VmaDllImporterResolver;
        if (resolver != null)
        {
            nativeLibrary = resolver(libraryName, assembly, searchPath);
        }

        if (nativeLibrary != IntPtr.Zero)
        {
            return nativeLibrary;
        }

        if (OperatingSystem.IsWindows())
        {
            if (NativeLibrary.TryLoad("vma.dll", assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            if (NativeLibrary.TryLoad("libvma.so", assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            if (NativeLibrary.TryLoad("libvma.dylib", assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }
        }

        if (NativeLibrary.TryLoad("libvma", assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        if (NativeLibrary.TryLoad("vma", assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        return 0;
    }

    [SkipLocalsInit]
    public static VkResult vmaCreateAllocator(in VmaAllocatorCreateInfo createInfo, out VmaAllocator allocator)
    {
        Unsafe.SkipInit(out allocator);

        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = ((delegate* unmanaged<VkDevice, byte*, PFN_vkVoidFunction>)GetApi(createInfo.instance).vkGetDeviceProcAddr_ptr.Value);

        fixed (VmaAllocator* allocatorPtr = &allocator)
        {
            VmaAllocatorCreateInfo createInfoIn = createInfo;
            createInfoIn.pVulkanFunctions = &functions;
            return vmaCreateAllocatorPrivate(&createInfoIn, allocatorPtr);
        }
    }

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = ((delegate* unmanaged<VkDevice, byte*, PFN_vkVoidFunction>)GetApi(allocateInfo->instance).vkGetDeviceProcAddr_ptr.Value);

        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocatorPrivate(allocateInfo, out allocator);
    }


    public static void vmaGetPhysicalDeviceProperties(VmaAllocator allocator, out VkPhysicalDeviceProperties* physicalDeviceProperties)
    {
        fixed (VkPhysicalDeviceProperties** physicalDevicePropertiesPtr = &physicalDeviceProperties)
        {
            vmaGetPhysicalDeviceProperties(allocator, physicalDevicePropertiesPtr);
        }
    }


    public static void vmaGetMemoryProperties(VmaAllocator allocator, out VkPhysicalDeviceMemoryProperties* physicalDeviceMemoryProperties)
    {
        fixed (VkPhysicalDeviceMemoryProperties** physicalDeviceMemoryPropertiesPtr = &physicalDeviceMemoryProperties)
        {
            vmaGetMemoryProperties(allocator, physicalDeviceMemoryPropertiesPtr);
        }
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, ReadOnlySpan<byte> name)
    {
        fixed (byte* namePtr = name)
        {
            vmaSetAllocationName(allocator, allocation, namePtr);
        }
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, VkUtf8ReadOnlyString name)
    {
        byte* namePtr = name;
        vmaSetAllocationName(allocator, allocation, namePtr);
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, string name)
    {
        vmaSetAllocationName(allocator, allocation, new VkUtf8ReadOnlyString(Encoding.UTF8.GetBytes(name)));
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
            return vmaCreateBuffer(allocator, bufferCreateInfoPtr, allocationCreateInfoPtr, bufferPtr, allocationPtr, pAllocationInfo);
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
            return vmaCreateBufferWithAlignment(allocator, bufferCreateInfoPtr, allocationCreateInfoPtr, minAlignment, bufferPtr, allocationPtr, pAllocationInfo);
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
            return vmaCreateImage(allocator, imageCreateInfoPtr, allocationCreateInfoPtr, imagePtr, allocationPtr, pAllocationInfo);
    }
}
