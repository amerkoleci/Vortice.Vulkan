// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct VkPerformanceValueDataINTEL
    {
        [FieldOffset(0)]
        public uint Value32;

        [FieldOffset(0)]
        public ulong Value64;

        [FieldOffset(0)]
        public float ValueFloat;

        [FieldOffset(0)]
        public RawBool ValueBool;

        [FieldOffset(0)]
        public IntPtr ValueString;
    }
}
