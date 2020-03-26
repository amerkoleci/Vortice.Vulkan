// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    public delegate IntPtr vkAllocationFunction(IntPtr userData, VkPointerSize size, VkPointerSize alignment, VkSystemAllocationScope allocationScope);
    public delegate IntPtr vkReallocationFunction(IntPtr userData, IntPtr original, VkPointerSize size, VkPointerSize alignment, VkSystemAllocationScope allocationScope);
    public delegate void vkFreeFunction(IntPtr userData, IntPtr memory);
    public delegate void vkInternalAllocationNotification(IntPtr userData, VkPointerSize size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);
    public delegate void vkInternalFreeNotification(IntPtr userData, VkPointerSize size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

    public partial struct VkAllocationCallbacks
    {
        public unsafe VkAllocationCallbacks(
            vkAllocationFunction alloc, 
            vkReallocationFunction realloc,
            vkFreeFunction free,
            vkInternalAllocationNotification internalAlloc = null,
            vkInternalFreeNotification internalFree = null,
            void* userData = default)
        {
            pfnAllocation = Marshal.GetFunctionPointerForDelegate(alloc);
            pfnReallocation = Marshal.GetFunctionPointerForDelegate(realloc);
            pfnFree = Marshal.GetFunctionPointerForDelegate(free);
            if (internalAlloc != null)
            {
                pfnInternalAllocation = Marshal.GetFunctionPointerForDelegate(internalAlloc);
                GC.KeepAlive(internalAlloc);
            }
            else
            {
                pfnInternalAllocation = IntPtr.Zero;
            }

            if (internalFree != null)
            {
                pfnInternalFree = Marshal.GetFunctionPointerForDelegate(internalFree);
                GC.KeepAlive(internalFree);
            }
            else
            {
                pfnInternalFree = IntPtr.Zero;
            }

            pUserData = userData;
            GC.KeepAlive(alloc);
            GC.KeepAlive(realloc);
            GC.KeepAlive(free);

        }
    }
}
