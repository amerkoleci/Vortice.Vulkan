// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct VkAccelerationStructureInstanceKHR
    {
        [FieldOffset(0)]
        public VkTransformMatrixKHR transform;
        [FieldOffset(48)]
        public uint instanceCustomIndex;
        [FieldOffset(51)]
        public uint mask;
        [FieldOffset(52)]
        public uint instanceShaderBindingTableRecordOffset;
        [FieldOffset(55)]
        public VkGeometryInstanceFlagsKHR flags;
        [FieldOffset(56)]
        public ulong accelerationStructureReference;
    }
}
