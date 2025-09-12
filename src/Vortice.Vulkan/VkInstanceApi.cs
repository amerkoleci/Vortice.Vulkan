// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Vortice.Vulkan;

public unsafe partial class VkInstanceApi
{
    public PFN_vkVoidFunction vkGetDeviceProcAddr(VkDevice device, string name)
    {
        byte* __pName_local = default;
        scoped Utf8StringMarshaller.ManagedToUnmanagedIn __pName__marshaller = new();
        try
        {
            __pName__marshaller.FromManaged(name, stackalloc byte[Utf8StringMarshaller.ManagedToUnmanagedIn.BufferSize]);
            __pName_local = __pName__marshaller.ToUnmanaged();
            return ((delegate* unmanaged<VkDevice, byte*, PFN_vkVoidFunction>)vkGetDeviceProcAddr_ptr.Value)(device.Handle, __pName_local);
        }
        finally
        {
            __pName__marshaller.Free();
        }
    }

    public PFN_vkVoidFunction vkGetDeviceProcAddr(nint device, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return ((delegate* unmanaged<VkDevice, byte*, PFN_vkVoidFunction>)vkGetDeviceProcAddr_ptr.Value)(device, pName);
    }

    public VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, out uint propertyCount)
    {
        propertyCount = default;
        fixed (uint* propertyCountPtr = &propertyCount)
            return vkEnumerateDeviceExtensionProperties(physicalDevice, default, propertyCountPtr, default);
    }

    public VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, Span<VkExtensionProperties> properties)
    {
        uint propertiesCount = checked((uint)properties.Length);
        fixed (VkExtensionProperties* propertiesPtr = properties)
            return vkEnumerateDeviceExtensionProperties(physicalDevice, default, &propertiesCount, propertiesPtr);
    }

    public VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, ReadOnlySpan<byte> layerName, out uint propertyCount)
    {
        propertyCount = default;
        fixed (byte* layerNamePtr = layerName)
        fixed (uint* propertyCountPtr = &propertyCount)
            return vkEnumerateDeviceExtensionProperties(physicalDevice, layerNamePtr, propertyCountPtr, default);
    }

    public VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, ReadOnlySpan<byte> layerName, Span<VkExtensionProperties> properties)
    {
        uint propertiesCount = checked((uint)properties.Length);
        fixed (byte* layerNamePtr = layerName)
        fixed (VkExtensionProperties* propertiesPtr = properties)
            return vkEnumerateDeviceExtensionProperties(physicalDevice, layerNamePtr, &propertiesCount, propertiesPtr);
    }

    public VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, ReadOnlySpan<char> layerName, out uint propertyCount)
    {
        byte* __pLayerName_local = default;
        Utf8CustomMarshaller.ManagedToUnmanagedIn __pLayerName_local__marshaller = new();
        propertyCount = default;
        fixed (uint* propertyCountPtr = &propertyCount)
            try
            {
                __pLayerName_local__marshaller.FromManaged(layerName, stackalloc byte[Utf8CustomMarshaller.ManagedToUnmanagedIn.BufferSize]);
                __pLayerName_local = __pLayerName_local__marshaller.ToUnmanaged();
                return vkEnumerateDeviceExtensionProperties(physicalDevice, __pLayerName_local, propertyCountPtr, default);
            }
            finally
            {
                __pLayerName_local__marshaller.Free();
            }
    }

    public VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, ReadOnlySpan<char> layerName, Span<VkExtensionProperties> properties)
    {
        byte* __pLayerName_local = default;
        Utf8CustomMarshaller.ManagedToUnmanagedIn __pLayerName_local__marshaller = new();
        uint propertyCount = checked((uint)properties.Length);
        fixed (VkExtensionProperties* propertiesPtr = properties)
        {
            try
            {
                __pLayerName_local__marshaller.FromManaged(layerName, stackalloc byte[Utf8CustomMarshaller.ManagedToUnmanagedIn.BufferSize]);
                __pLayerName_local = __pLayerName_local__marshaller.ToUnmanaged();
                return vkEnumerateDeviceExtensionProperties(physicalDevice, __pLayerName_local, &propertyCount, propertiesPtr);
            }
            finally
            {
                __pLayerName_local__marshaller.Free();
            }
        }
    }

    public VkResult vkSetDebugUtilsObjectNameEXT(VkDevice device, VkObjectType objectType, ulong objectHandle, ReadOnlySpan<byte> label)
    {
        fixed (byte* pName = label)
        {
            VkDebugUtilsObjectNameInfoEXT info = new()
            {
                objectType = objectType,
                objectHandle = objectHandle,
                pObjectName = pName
            };
            return vkSetDebugUtilsObjectNameEXT(device, &info);
        }
    }

    public VkResult vkSetDebugUtilsObjectNameEXT(VkDevice device, VkObjectType objectType, ulong objectHandle, string? label = default)
    {
        byte* __pName_local = default;
        scoped Utf8StringMarshaller.ManagedToUnmanagedIn __label__marshaller = new();
        try
        {
            __label__marshaller.FromManaged(label, stackalloc byte[Utf8StringMarshaller.ManagedToUnmanagedIn.BufferSize]);
            __pName_local = __label__marshaller.ToUnmanaged();
            VkDebugUtilsObjectNameInfoEXT info = new()
            {
                objectType = objectType,
                objectHandle = objectHandle,
                pObjectName = __pName_local
            };
            return vkSetDebugUtilsObjectNameEXT(device, &info);
        }
        finally
        {
            __label__marshaller.Free();
        }
    }

    public VkResult vkSetDebugUtilsObjectNameEXT(VkDevice device, VkObjectType objectType, ulong objectHandle, ReadOnlySpan<char> label)
    {
        int maxLength = Encoding.UTF8.GetMaxByteCount(label.Length);
        Span<byte> bytes = stackalloc byte[maxLength + 1];

        int length = Encoding.UTF8.GetBytes(label, bytes);
        Span<byte> result = bytes.Slice(0, length);
        fixed (byte* pLabel = result)
        {
            VkDebugUtilsObjectNameInfoEXT info = new()
            {
                objectType = objectType,
                objectHandle = objectHandle,
                pObjectName = pLabel
            };
            return vkSetDebugUtilsObjectNameEXT(device, &info);
        }
    }
}
