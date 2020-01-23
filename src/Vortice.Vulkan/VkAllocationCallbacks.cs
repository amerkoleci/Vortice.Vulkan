// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    internal struct VkAllocationCallbacks
    {
        public unsafe void* pUserData;

        public IntPtr pfnAllocation;

        public IntPtr pfnReallocation;

        public IntPtr pfnFree;

        public IntPtr pfnInternalAllocation;

        public IntPtr pfnInternalFree;
    }
}
