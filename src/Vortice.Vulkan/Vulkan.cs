// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Vortice.Vulkan;

public static unsafe partial class Vulkan
{
    private const string LibraryName = "vulkan";

    private delegate PFN_vkVoidFunction LoadFunction(nint context, ReadOnlySpan<byte> name);

    private static nint s_vulkanModule;
    private static readonly ConcurrentDictionary<VkInstance, VkInstanceApi> s_instanceTables = new();
    private static readonly ConcurrentDictionary<VkDevice, VkDeviceApi> s_deviceTables = new();

    /// <summary>
    /// The VK_LAYER_KHRONOS_validation extension name.
    /// </summary>
    public static ReadOnlySpan<byte> VK_LAYER_KHRONOS_VALIDATION_EXTENSION_NAME => "VK_LAYER_KHRONOS_validation"u8;

    public const uint VK_TRUE = 1;
    public const uint VK_FALSE = 0;

    public static VkVersion VK_API_VERSION_1_0 => new(0, 1, 0, 0);
    public static VkVersion VK_API_VERSION_1_1 => new(0, 1, 1, 0);
    public static VkVersion VK_API_VERSION_1_2 => new(0, 1, 2, 0);
    public static VkVersion VK_API_VERSION_1_3 => new(0, 1, 3, 0);
    public static VkVersion VK_API_VERSION_1_4 => new(0, 1, 4, 0);

    public static VkResult vkInitialize(string? libraryName = default)
    {
        if (OperatingSystem.IsWindows())
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                s_vulkanModule = LoadNativeLibrary(libraryName);
            }

            if (s_vulkanModule == 0)
                s_vulkanModule = LoadNativeLibrary("vulkan-1.dll");

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                s_vulkanModule = LoadNativeLibrary(libraryName);
            }

            if (s_vulkanModule == 0)
            {
                s_vulkanModule = LoadNativeLibrary("libvulkan.dylib");
                if (s_vulkanModule == 0)
                    s_vulkanModule = LoadNativeLibrary("libvulkan.1.dylib");
                if (s_vulkanModule == 0)
                    s_vulkanModule = LoadNativeLibrary("libMoltenVK.dylib");
                // Add support for using Vulkan and MoltenVK in a Framework. App store rules for iOS
                // strictly enforce no .dylib's. If they aren't found it just falls through
                if (s_vulkanModule == 0)
                    s_vulkanModule = LoadNativeLibrary("vulkan.framework/vulkan");
                if (s_vulkanModule == 0)
                    s_vulkanModule = LoadNativeLibrary("MoltenVK.framework/MoltenVK");
            }

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }
        else
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                s_vulkanModule = LoadNativeLibrary(libraryName);
            }

            if (s_vulkanModule == 0)
            {
                s_vulkanModule = LoadNativeLibrary("libvulkan.so.1");
                if (s_vulkanModule == 0)
                    s_vulkanModule = LoadNativeLibrary("libvulkan.so");
            }

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }

        vkGetInstanceProcAddr_ptr = (delegate* unmanaged<VkInstance, byte*, PFN_vkVoidFunction>)NativeLibrary.GetExport(s_vulkanModule, nameof(vkGetInstanceProcAddr));
        vkCreateInstance_ptr = vkGetGlobalProcAddr("vkCreateInstance"u8);
        vkEnumerateInstanceExtensionProperties_ptr = vkGetGlobalProcAddr("vkEnumerateInstanceExtensionProperties"u8);
        vkEnumerateInstanceLayerProperties_ptr = vkGetGlobalProcAddr("vkEnumerateInstanceLayerProperties"u8);
        vkEnumerateInstanceVersion_ptr = vkGetGlobalProcAddr("vkEnumerateInstanceVersion"u8);

        return VkResult.Success;
    }

    public static void vkShutdown()
    {
        if (s_vulkanModule != IntPtr.Zero)
        {
            NativeLibrary.Free(s_vulkanModule);
            s_vulkanModule = IntPtr.Zero;
        }
    }

    public static VkInstanceApi GetApi(VkInstance instance)
    {
        return s_instanceTables.GetOrAdd(instance, instance => new VkInstanceApi(instance));
    }

    public static VkDeviceApi GetApi(VkInstance instance, VkDevice device)
    {
        return s_deviceTables.GetOrAdd(device, device => new VkDeviceApi(GetApi(instance), device));
    }

    /// <summary>
    /// Global function pointer loader.
    /// </summary>
    public static delegate* unmanaged<VkInstance, byte*, PFN_vkVoidFunction> vkGetInstanceProcAddr_ptr;

    //public static delegate* unmanaged<VkDevice, byte*, PFN_vkVoidFunction> vkGetDeviceProcAddr_ptr;

    public static PFN_vkVoidFunction vkGetGlobalProcAddr(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return vkGetInstanceProcAddr_ptr(default, pName);
    }

    public static PFN_vkVoidFunction vkGetInstanceProcAddr(VkInstance instance, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return vkGetInstanceProcAddr_ptr(instance.Handle, pName);
    }

    public static PFN_vkVoidFunction vkGetInstanceProcAddr(nint instance, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return vkGetInstanceProcAddr_ptr(instance, pName);
    }

    public static PFN_vkVoidFunction vkGetInstanceProcAddr(VkInstance instance, string name)
    {
        byte* __pName_local = default;
        scoped Utf8StringMarshaller.ManagedToUnmanagedIn __pName__marshaller = new();
        try
        {
            __pName__marshaller.FromManaged(name, stackalloc byte[Utf8StringMarshaller.ManagedToUnmanagedIn.BufferSize]);
            __pName_local = __pName__marshaller.ToUnmanaged();
            return vkGetInstanceProcAddr_ptr(instance.Handle, __pName_local);
        }
        finally
        {
            __pName__marshaller.Free();
        }
    }

    public static VkResult vkEnumerateInstanceExtensionProperties(uint* propertyCount, VkExtensionProperties* properties)
    {
        return ((delegate* unmanaged<byte*, uint*, VkExtensionProperties*, VkResult>)vkEnumerateInstanceExtensionProperties_ptr.Value)(null, propertyCount, properties);
    }

    [SkipLocalsInit]
    public static VkResult vkEnumerateInstanceExtensionProperties(out uint propertyCount)
    {
        Unsafe.SkipInit(out propertyCount);
        fixed (uint* propertyCountPtr = &propertyCount)
        {
            return ((delegate* unmanaged<byte*, uint*, VkExtensionProperties*, VkResult>)vkEnumerateInstanceExtensionProperties_ptr.Value)(null, propertyCountPtr, default);
        }
    }

    public static VkResult vkEnumerateInstanceExtensionProperties(Span<VkExtensionProperties> properties)
    {
        uint propertiesCount = checked((uint)properties.Length);
        fixed (VkExtensionProperties* propertiesPtr = properties)
        {
            return ((delegate* unmanaged<byte*, uint*, VkExtensionProperties*, VkResult>)vkEnumerateInstanceExtensionProperties_ptr.Value)(null, &propertiesCount, propertiesPtr);
        }
    }

    [SkipLocalsInit]
    public static VkResult vkEnumerateInstanceExtensionProperties(VkUtf8ReadOnlyString layerName, out uint propertyCount)
    {
        Unsafe.SkipInit(out propertyCount);
        fixed (uint* propertyCountPtr = &propertyCount)
        {
            return ((delegate* unmanaged<byte*, uint*, VkExtensionProperties*, VkResult>)vkEnumerateInstanceExtensionProperties_ptr.Value)(layerName, propertyCountPtr, default);
        }
    }

    [SkipLocalsInit]
    public static VkResult vkEnumerateInstanceExtensionProperties(VkUtf8ReadOnlyString layerName, Span<VkExtensionProperties> properties)
    {
        uint propertiesCount = checked((uint)properties.Length);
        fixed (VkExtensionProperties* propertiesPtr = properties)
        {
            return ((delegate* unmanaged<byte*, uint*, VkExtensionProperties*, VkResult>)vkEnumerateInstanceExtensionProperties_ptr.Value)(layerName, &propertiesCount, propertiesPtr);
        }
    }

    /// <summary>
    /// Query instance-level version before instance creation.
    /// </summary>
    /// <returns>The version of Vulkan supported by instance-level functionality.</returns>
    public static VkVersion vkEnumerateInstanceVersion()
    {
        uint apiVersion;
        if (vkEnumerateInstanceVersion_ptr != null
            && vkEnumerateInstanceVersion(&apiVersion) == VkResult.Success)
        {
            return new VkVersion(apiVersion);
        }

        return VkVersion.Version_1_0;
    }

    private static nint LoadNativeLibrary(string name)
    {
        if (NativeLibrary.TryLoad(name, out nint lib))
        {
            return lib;
        }

        return 0;
    }
}
