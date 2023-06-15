// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.SpirvCross.SpirvCrossApi;

namespace Vortice.SpirvCross;

public sealed unsafe partial class Compiler
{
    private readonly nint _handle;

    public Compiler(IntPtr handle)
    {
        _handle = handle;

        spvc_compiler_create_compiler_options(_handle, out IntPtr optionsPtr).CheckResult();
        Options = new(optionsPtr);
    }

    public Options Options { get; }

    public uint CurrentIdBound => spvc_compiler_get_current_id_bound(_handle);

    public void Apply()
    {
        spvc_compiler_install_compiler_options(_handle, Options.Handle).CheckResult();
    }

    public string Compile()
    {
        sbyte* utf8Str = default;
        spvc_compiler_compile(_handle, (sbyte*)&utf8Str).CheckResult();
        return new string(utf8Str);
    }

    public Result AddHeaderLine(string text)
    {
        fixed (byte* dataPtr = GetUtf8(text))
        {
            return spvc_compiler_add_header_line(_handle, dataPtr);
        }
    }

    public Result RequireExtension(string text)
    {
        fixed (byte* dataPtr = GetUtf8(text))
        {
            return spvc_compiler_require_extension(_handle, dataPtr);
        }
    }

    public void SetName(uint id, string text)
    {
        fixed (byte* dataPtr = GetUtf8(text))
        {
            spvc_compiler_set_name(_handle, id, dataPtr);
        }
    }

    public string? GetName(uint id)
    {
        var dataPtr = spvc_compiler_get_name(_handle, id);
        return Marshal.PtrToStringUTF8(new IntPtr(dataPtr));
    }

    public Result FlattenBufferBlock(uint variableId)
    {
        return spvc_compiler_flatten_buffer_block(_handle, variableId);
    }

    public bool VariableIsDepthOrCompare(uint variableId)
    {
        return spvc_compiler_variable_is_depth_or_compare(_handle, variableId) == 1;
    }

    public Result MaskStageOutputByLocation(uint location, uint component)
    {
        return spvc_compiler_mask_stage_output_by_location(_handle, location, component);
    }

    public Result MaskStageOutputByBuiltIn(SpvBuiltIn builtin)
    {
        return spvc_compiler_mask_stage_output_by_builtin(_handle, builtin);
    }

    public IntPtr GetShaderResourcesPointer()
    {
        spvc_compiler_create_shader_resources(_handle, out var resourcesPtr).CheckResult();
        return resourcesPtr;
    }

    public SpvReflectedResource[] GetShaderResourceListOfType(IntPtr resourcesPtr, SpvResourceType type)
    {
        spvc_resources_get_resource_list_for_type(resourcesPtr, type, out var resourceList, out var resourceSize).CheckResult();

        var size = resourceSize.ToUInt32();
        var resources = new SpvReflectedResource[size];

        int structSize = Marshal.SizeOf<SpvReflectedResource>();

        for (int i = 0; i < size; i++)
        {
            var ptr = new IntPtr(resourceList.ToInt64() + i * structSize);
            resources[i] = Marshal.PtrToStructure<SpvReflectedResource>(ptr);
        }

        return resources;
    }

    public int GetDecoration(uint id, SpvDecoration decoration)
    {
        return spvc_compiler_get_decoration(_handle, id, decoration);
    }
}
