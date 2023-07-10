// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.SPIRV;
using static Vortice.SpirvCross.SpirvCrossApi;
using static Vortice.SpirvCross.Utils;

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
        fixed (sbyte* dataPtr = text.GetUtf8Span())
        {
            return spvc_compiler_add_header_line(_handle, dataPtr);
        }
    }

    public Result RequireExtension(string text)
    {
        fixed (sbyte* dataPtr = text.GetUtf8Span())
        {
            return spvc_compiler_require_extension(_handle, dataPtr);
        }
    }

    public void SetName(uint id, string text)
    {
        fixed (sbyte* dataPtr = text.GetUtf8Span())
        {
            spvc_compiler_set_name(_handle, id, dataPtr);
        }
    }

    public string? GetName(uint id)
    {
        return GetUtf8Span(spvc_compiler_get_name(_handle, id)).GetString();
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

    public Result CreateShaderResources(out Resources resources)
    {
        return spvc_compiler_create_shader_resources(_handle, out resources);
    }

    public int GetDecoration(uint id, SpvDecoration decoration)
    {
        return spvc_compiler_get_decoration(_handle, id, decoration);
    }
}
