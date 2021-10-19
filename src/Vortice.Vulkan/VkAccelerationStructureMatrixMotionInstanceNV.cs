// Copyright (c) Amer Koleci and Contributors
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan
{
    public partial struct VkAccelerationStructureMatrixMotionInstanceNV
    {
        public VkTransformMatrixKHR transformT0;
        public VkTransformMatrixKHR transformT1;

        private uint _bitfield1;

        public uint instanceCustomIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _bitfield1 & 0xFFFFFFu;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield1 = (_bitfield1 & ~0xFFFFFFu) | (value & 0xFFFFFFu);
            }
        }

        public uint mask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (_bitfield1 >> 24) & 0xFFu;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield1 = (_bitfield1 & ~(0xFFu << 24)) | ((value & 0xFFu) << 24);
            }
        }

        private uint _bitfield2;

        public uint instanceShaderBindingTableRecordOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _bitfield2 & 0xFFFFFFu;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield2 = (_bitfield2 & ~0xFFFFFFu) | (value & 0xFFFFFFu);
            }
        }

        public VkGeometryInstanceFlagsKHR flags
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (VkGeometryInstanceFlagsKHR)((_bitfield2 >> 24) & 0xFFu);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _bitfield2 = (_bitfield2 & ~(0xFFu << 24)) | (((uint)(value) & 0xFFu) << 24);
            }
        }

        public ulong accelerationStructureReference;
    }
}
