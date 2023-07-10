// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#pragma warning disable CS0649
using Vortice.SPIRV;

namespace Vortice.SpirvCross;

public struct SpvReflectedBuiltinResource
{
    public SpvBuiltIn Builtin;
    public uint ValueTypeId;
    public SpvReflectedResource Resource;

    internal readonly unsafe struct Native
    {
        public readonly SpvBuiltIn builtin;
        public readonly uint value_type_id;
        public readonly SpvReflectedResource.Native resource;
    }

    internal unsafe void MarshalFrom(in Native @ref)
    {
        Builtin = @ref.builtin;
        ValueTypeId = @ref.value_type_id;
        Resource.MarshalFrom(in @ref.resource);
    }
}
