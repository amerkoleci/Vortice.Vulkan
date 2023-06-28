// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created device
/// </summary>
public partial struct VkDeviceCreateInfo
{
    /// <summary>
    ///  NULL or a pointer to a structure extending this structure.
    /// </summary>
    public unsafe void* pNext;

    /// <summary>
    /// A bitmask indicating behavior of the device.
    /// </summary>
    public VkDeviceCreateFlags Flags;

    /// <summary>
    /// Structures describing the queues that are requested to be created along with the logical device.
    /// </summary>
    public VkDeviceQueueCreateInfo[] QueueCreateInfos;

    /// <summary>
    /// An array of strings containing the names of extensions to enable for the created device.
    /// </summary>
    public string[]? EnabledExtensionNames;

    /// <summary>
    /// Is <c>null</c> or a <see cref="VkPhysicalDeviceFeatures"/> structure that contains boolean
    /// indicators of all the features to be enabled.
    /// </summary>
    public VkPhysicalDeviceFeatures? EnabledFeatures;

    public unsafe VkDeviceCreateInfo(
        VkDeviceQueueCreateInfo[] queueCreateInfos,
        string[]? enabledExtensionNames = null,
        VkPhysicalDeviceFeatures? enabledFeatures = null,
        void* pNext = default)
    {
        this.pNext = pNext;
        QueueCreateInfos = queueCreateInfos;
        EnabledExtensionNames = enabledExtensionNames;
        EnabledFeatures = enabledFeatures;
    }


    #region Marshal
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct __Native
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceCreateFlags flags;
        public int queueCreateInfoCount;
        public VkDeviceQueueCreateInfo.__Native* pQueueCreateInfos;
        public uint enabledLayerCount;
        public sbyte** ppEnabledLayerNames;
        public int enabledExtensionCount;
        public sbyte** ppEnabledExtensionNames;
        public VkPhysicalDeviceFeatures* pEnabledFeatures;

        public readonly void Free()
        {
            for (int i = 0; i < queueCreateInfoCount; i++)
            {
                pQueueCreateInfos[i].Free();
            }
            NativeMemory.Free(pQueueCreateInfos);
            if (enabledExtensionCount > 0)
            {
                NativeMemory.Free(ppEnabledExtensionNames);
            }
            NativeMemory.Free(pEnabledFeatures);
        }
    }

    internal unsafe void ToNative(out __Native native)
    {
        native.sType = VkStructureType.DeviceCreateInfo;
        native.pNext = pNext;
        native.flags = Flags;
        if (QueueCreateInfos?.Length > 0)
        {
            native.queueCreateInfoCount = QueueCreateInfos.Length;

            VkDeviceQueueCreateInfo.__Native* pQueueCreateInfos = Interop.AllocateArray<VkDeviceQueueCreateInfo.__Native>((uint)native.queueCreateInfoCount);
            for (uint i = 0; i < native.queueCreateInfoCount; i++)
            {
                QueueCreateInfos[i].ToNative(out pQueueCreateInfos[i]);
            }

            native.pQueueCreateInfos = pQueueCreateInfos;
        }
        else
        {
            native.queueCreateInfoCount = 0;
            native.pQueueCreateInfos = null;
        }

        native.enabledLayerCount = 0u;
        native.ppEnabledLayerNames = null;
        if (EnabledExtensionNames?.Length > 0)
        {
            native.enabledExtensionCount = EnabledExtensionNames.Length;
            native.ppEnabledExtensionNames = Interop.AllocToPointers(EnabledExtensionNames!);
        }
        else
        {
            native.enabledExtensionCount = 0;
            native.ppEnabledExtensionNames = null;
        }

        native.pEnabledFeatures = Interop.AllocToPointer(ref EnabledFeatures);
    }
    #endregion
}
