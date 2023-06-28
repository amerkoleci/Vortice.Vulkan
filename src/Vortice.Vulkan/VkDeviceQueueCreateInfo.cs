// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created device queue
/// </summary>
public partial struct VkDeviceQueueCreateInfo
{
    /// <summary>
    ///  NULL or a pointer to a structure extending this structure.
    /// </summary>
    public unsafe void* pNext;

    /// <summary>
    /// A bitmask indicating behavior of the queues.
    /// </summary>
    public VkDeviceQueueCreateFlags Flags;

    /// <summary>
    /// An unsigned integer indicating the index of the queue family in which to create the queues on this device.
    /// This index corresponds to the index of an element of the pQueueFamilyProperties array that was returned by <see cref="Vulkan.vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice)"/>.
    /// </summary>
    public uint QueueFamilyIndex;

    /// <summary>
    /// An integer specifying the number of queues to create in the queue family indicated by queueFamilyIndex, and with the behavior specified by flags.
    /// </summary>
    public int QueueCount;

    /// <summary>
    /// An array of normalized floating point values, specifying priorities of work that
    /// will be submitted to each created queue.
    /// </summary>
    public float[] QueuePriorities;

    public VkDeviceQueueCreateInfo(uint queueFamilyIndex, int queueCount, params float[] queuePriorities)
    {
        QueueFamilyIndex = queueFamilyIndex;
        QueueCount = queueCount;
        QueuePriorities = queuePriorities;
    }

    #region Marshal
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct __Native
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceQueueCreateFlags flags;
        public uint queueFamilyIndex;
        public int queueCount;
        public float* pQueuePriorities;

        public readonly void Free()
        {
            NativeMemory.Free(pQueuePriorities);
        }
    }

    internal readonly unsafe void ToNative(out __Native native)
    {
        native.sType = VkStructureType.DeviceQueueCreateInfo;
        native.pNext = pNext;
        native.flags = Flags;
        native.queueFamilyIndex = QueueFamilyIndex;
        native.queueCount = QueueCount;
        native.pQueuePriorities = Interop.AllocToPointer(QueuePriorities);
    }
    #endregion
}
