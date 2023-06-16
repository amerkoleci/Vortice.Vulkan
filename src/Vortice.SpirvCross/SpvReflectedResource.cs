// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.SpirvCross.Utils;

namespace Vortice.SpirvCross;

public struct SpvReflectedResource
{
    public uint Id;
    public uint BaseTypeId;
    public uint TypeId;
    public string Name;

    internal readonly unsafe struct Native
    {
        public readonly uint id;
        public readonly uint base_type_id;
        public readonly uint type_id;
        public readonly sbyte* name;
    }

    internal unsafe void MarshalFrom(in Native @ref)
    {
        Id = @ref.id;
        BaseTypeId = @ref.base_type_id;
        TypeId = @ref.type_id;
        Name = GetUtf8Span(@ref.name).GetString()!;
    }
}
