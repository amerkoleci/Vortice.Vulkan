// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct VkPipelineExecutableStatisticValueKHR
    {
        [FieldOffset(0)]
        public RawBool b32;

        [FieldOffset(0)]
        public long i64;

        [FieldOffset(0)]
        public ulong u64;

        [FieldOffset(0)]
        public double f64;
    }
}
