// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

unsafe partial class Vma
{
    /// <summary>
    /// Raised whenever a native library is loaded by VMA. Handlers can be added to this event to customize how libraries are loaded, and they will be used first whenever a new native library is being resolved.
    /// </summary>
    public static event DllImportResolver? ResolveLibrary;

    private const string LibName = "vma";

    static Vma()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

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

        return 0;
    }

    /// <summary>Tries to resolve a native library using the handlers for the <see cref="ResolveLibrary"/> event.</summary>
    /// <param name="libraryName">The native library to resolve.</param>
    /// <param name="assembly">The assembly requesting the resolution.</param>
    /// <param name="searchPath">The <see cref="DllImportSearchPath"/> value on the P/Invoke or assembly, or <see langword="null"/>.</param>
    /// <param name="nativeLibrary">The loaded library, if one was resolved.</param>
    /// <returns>Whether or not the requested library was successfully loaded.</returns>
    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
    {
        DllImportResolver? resolveLibrary = ResolveLibrary;

        if (resolveLibrary is not null)
        {
            Delegate[] resolvers = resolveLibrary.GetInvocationList();

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
        if (allocateInfo->pVulkanFunctions == null)
        {
            VmaVulkanFunctions functions = default;
            functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
            functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;

            allocateInfo->pVulkanFunctions = &functions;
            return vmaCreateAllocatorPrivate(allocateInfo, allocator);
        }
        else
        {
            return vmaCreateAllocatorPrivate(allocateInfo, allocator);
        }
    }

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;

        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocatorPrivate(allocateInfo, out allocator);
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

    [LibraryImport(LibName)]
    public static partial VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo);

    [LibraryImport(LibName)]
    public static partial VkResult vmaCreateBufferWithAlignment(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        ulong minAlignment,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo);

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

    [LibraryImport(LibName)]
    public static partial VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo);
}
