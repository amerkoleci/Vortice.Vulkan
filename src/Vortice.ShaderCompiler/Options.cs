// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler;

public class Options : IDisposable
{
    public Options()
        : this(shaderc_compile_options_initialize())
    {
    }

    private Options(nint handle)
    {
        Handle = handle;
        if (Handle == 0)
        {
            throw new Exception("Cannot initialize native handle");
        }
    }

    public nint Handle { get; }

    /// <summary>
    /// Finalizes an instance of the <see cref="Options" /> class.
    /// </summary>
    ~Options() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Handle == 0)
            return;

        if (disposing)
        {
            shaderc_compile_options_release(Handle);
        }
    }

    public Options Clone()
    {
        return new Options(shaderc_compile_options_clone(Handle));
    }

    public void AddMacroDefinition(string name, string? value = null)
    {
        shaderc_compile_options_add_macro_definition(Handle,
            name,
            (nuint)name.Length,
            value, string.IsNullOrEmpty(value) ? 0 : (nuint)value!.Length);
    }

    public void SetSourceLanguage(SourceLanguage language)
    {
        shaderc_compile_options_set_source_language(Handle, language);
    }

    public void SetGenerateDebugInfo()
    {
        shaderc_compile_options_set_generate_debug_info(Handle);
    }

    public void SetOptimizationLevel(OptimizationLevel level)
    {
        shaderc_compile_options_set_optimization_level(Handle, level);
    }

    public void SetForcedVersionPofile(int version, Profile profile)
    {
        shaderc_compile_options_set_forced_version_profile(Handle, version, profile);
    }

    public void SetSuppressWarnings()
    {
        shaderc_compile_options_set_suppress_warnings(Handle);
    }

    public void SetTargetEnv(TargetEnvironment target, uint version)
    {
        shaderc_compile_options_set_target_env(Handle, target, version);
    }

    public void SetargetSpirv(SpirVVersion version)
    {
        shaderc_compile_options_set_target_spirv(Handle, version);
    }

    public void SetWarningsAsErrors() => shaderc_compile_options_set_warnings_as_errors(Handle);

    public void SetLimit(Limit limit, int value) => shaderc_compile_options_set_limit(Handle, limit, value);

    public void SetAutoBindUniforms(bool value) => shaderc_compile_options_set_auto_bind_uniforms(Handle, value);

    public void SetHLSLIoMapping(bool value) => shaderc_compile_options_set_hlsl_io_mapping(Handle, value);

    public void SetHLSLOffsets(bool value) => shaderc_compile_options_set_hlsl_offsets(Handle, value);

    public void SetBindingBase(UniformKind kind, uint @base) => shaderc_compile_options_set_binding_base(Handle, kind, @base);

    public void SetBindingBaseForStage(ShaderKind shaderKind, UniformKind kind, uint @base)
    {
        shaderc_compile_options_set_binding_base_for_stage(Handle, shaderKind, kind, @base);
    }

    public void SetAutoMapLocations(bool value) => shaderc_compile_options_set_auto_map_locations(Handle, value);

    public void SetHLSLRegisterSetAndBindingForStage(ShaderKind shaderKind, string reg, string set, string binding)
    {
        shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(Handle, shaderKind, reg, set, binding);
    }

    public void SetHLSLRegisterSetAndBinding(string reg, string set, string binding)
    {
        shaderc_compile_options_set_hlsl_register_set_and_binding(Handle, reg, set, binding);
    }

    public void SetHLSLFunctionality1(bool enable)
    {
        shaderc_compile_options_set_hlsl_functionality1(Handle, enable);
    }

    /// <summary>
    /// Sets whether the compiler should invert position.Y output in vertex shader.
    /// </summary>
    /// <param name="enable"></param>
    public void SetInvertY(bool enable)
    {
        shaderc_compile_options_set_invert_y(Handle, enable);
    }

    public void SetNaNClamp(bool enable)
    {
        shaderc_compile_options_set_nan_clamp(Handle, enable);
    }
}
