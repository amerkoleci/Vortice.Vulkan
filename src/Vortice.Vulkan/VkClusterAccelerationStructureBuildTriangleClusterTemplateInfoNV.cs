// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

public partial struct VkClusterAccelerationStructureBuildTriangleClusterTemplateInfoNV
{
    public uint clusterID;
    public uint clusterFlags;
    private uint _bitfield;

    public uint triangleCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return _bitfield & 0x1FFu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~0x1FFu) | (value & 0x1FFu);
        }
    }

    public uint vertexCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 9) & 0x1FFu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0x1FFu << 9)) | ((value & 0x1FFu) << 9);
        }
    }

    public uint positionTruncateBitCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 18) & 0x3Fu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0x3Fu << 18)) | ((value & 0x3Fu) << 18);
        }
    }

    public uint indexType
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 24) & 0xFu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0xFu << 24)) | ((value & 0xFu) << 24);
        }
    }

    public uint opacityMicromapIndexType
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 28) & 0xFu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0xFu << 28)) | ((value & 0xFu) << 28);
        }
    }

    public VkClusterAccelerationStructureGeometryIndexAndGeometryFlagsNV baseGeometryIndexAndGeometryFlags;
    public ushort indexBufferStride;
    public ushort vertexBufferStride;
    public ushort geometryIndexAndFlagsBufferStride;
    public ushort opacityMicromapIndexBufferStride;
    public ulong indexBuffer;
    public ulong vertexBuffer;
    public ulong geometryIndexAndFlagsBuffer;
    public ulong opacityMicromapArray;
    public ulong opacityMicromapIndexBuffer;
    public ulong instantiationBoundingBoxLimit;
}
