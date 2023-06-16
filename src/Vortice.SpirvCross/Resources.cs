// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using static Vortice.SpirvCross.SpirvCrossApi;

namespace Vortice.SpirvCross;

/// <unmanaged>spvc_resources </unmanaged>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct Resources : IEquatable<Resources>
{
    public Resources(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static Resources Null => new(0);
    public static implicit operator Resources(nint handle) => new(handle);
    public static bool operator ==(Resources left, Resources right) => left.Handle == right.Handle;
    public static bool operator !=(Resources left, Resources right) => left.Handle != right.Handle;
    public static bool operator ==(Resources left, nint right) => left.Handle == right;
    public static bool operator !=(Resources left, nint right) => left.Handle != right;
    public bool Equals(Resources other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Resources handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(Resources)} [0x{Handle:X}]";


    public unsafe SpvReflectedResource[] GetResourceListForType(SpvResourceType type)
    {
        SpvReflectedResource.Native* resourceList;
        nuint resourceSize;
        spvc_resources_get_resource_list_for_type(Handle, type, &resourceList, &resourceSize).CheckResult();

        SpvReflectedResource[] resources = new SpvReflectedResource[(uint)resourceSize];

        for (uint i = 0; i < (uint)resourceSize; i++)
        {
            resources[i].MarshalFrom(in resourceList[i]);
        }

        return resources;
    }

    public unsafe SpvReflectedBuiltinResource[] GetBuiltinResourceListForType(SpvcBuiltinResourceType type)
    {
        SpvReflectedBuiltinResource.Native* resourceList;
        nuint resourceSize;
        spvc_resources_get_builtin_resource_list_for_type(Handle, type, &resourceList, &resourceSize).CheckResult();

        SpvReflectedBuiltinResource[] resources = new SpvReflectedBuiltinResource[(uint)resourceSize];

        for (uint i = 0; i < (uint)resourceSize; i++)
        {
            resources[i].MarshalFrom(in resourceList[i]); ;
        }

        return resources;
    }
}
