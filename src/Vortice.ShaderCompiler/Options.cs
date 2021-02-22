// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler
{
    public class Options : IDisposable
    {
        private IntPtr _handle;

        public Options()
            : this(shaderc_compile_options_initialize())
        {
        }

        private Options(IntPtr handle)
        {
            _handle = handle;
            if (_handle == IntPtr.Zero)
            {
                throw new Exception("Cannot initialize native handle");
            }
        }

        public IntPtr Handle => _handle;

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
            if (_handle == IntPtr.Zero)
                return;

            if (disposing)
            {
                shaderc_compile_options_release(_handle);
            }
        }

        public Options Clone()
        {
            return new Options(shaderc_compile_options_clone(_handle));
        }

        public void AddMacroDefinition(string name, string? value = null)
        {
            shaderc_compile_options_add_macro_definition(_handle, name, (nuint)name.Length, value, string.IsNullOrEmpty(value) ? 0 : (nuint)value.Length);
        }

        public void SetSourceLanguage(SourceLanguage language)
        {
            shaderc_compile_options_set_source_language(_handle, language);
        }

        public void SetGenerateDebugInfo()
        {
            shaderc_compile_options_set_generate_debug_info(_handle);
        }

        public void SetOptimizationLevel(OptimizationLevel level)
        {
            shaderc_compile_options_set_optimization_level(_handle, level);
        }

        public void SetForcedVersionPofile(int version, Profile profile)
        {
            shaderc_compile_options_set_forced_version_profile(_handle, version, profile);
        }

        public void SetSuppressWarnings()
        {
            shaderc_compile_options_set_suppress_warnings(_handle);
        }

        public void SetTargetEnv(TargetEnvironment target, uint version)
        {
            shaderc_compile_options_set_target_env(_handle, target, version);
        }

        public void SetargetSpirv(SpirVVersion version)
        {
            shaderc_compile_options_set_target_spirv(_handle, version);
        }

        public void SetWarningsAsErrors() => shaderc_compile_options_set_warnings_as_errors(_handle);

        public void SetLimit(Limit limit, int value) => shaderc_compile_options_set_limit(_handle, limit, value);

        public void SetAutoBindUniforms(bool value) => shaderc_compile_options_set_auto_bind_uniforms(_handle, value);

        public void SetHLSLIoMapping(bool value) => shaderc_compile_options_set_hlsl_io_mapping(_handle, value);

        public void SetHLSLOffsets(bool value) => shaderc_compile_options_set_hlsl_offsets(_handle, value);

        public void SetBindingBase(UniformKind kind, uint @base) => shaderc_compile_options_set_binding_base(_handle, kind, @base);

        public void SetBindingBaseForStage(ShaderKind shaderKind, UniformKind kind, uint @base)
        {
            shaderc_compile_options_set_binding_base_for_stage(_handle, shaderKind, kind, @base);
        }

        public void SetAutoMapLocations(bool value) => shaderc_compile_options_set_auto_map_locations(_handle, value);

        public void SetHLSLRegisterSetAndBindingForStage(ShaderKind shaderKind, string reg, string set, string binding)
        {
            shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(_handle, shaderKind, reg, set, binding);
        }

        public void SetHLSLRegisterSetAndBinding(string reg, string set, string binding)
        {
            shaderc_compile_options_set_hlsl_register_set_and_binding(_handle, reg, set, binding);
        }

        public void SetHLSLFunctionality1(bool enable)
        {
            shaderc_compile_options_set_hlsl_functionality1(_handle, enable);
        }

        /// <summary>
        /// Sets whether the compiler should invert position.Y output in vertex shader.
        /// </summary>
        /// <param name="enable"></param>
        public void SetInvertY(bool enable)
        {
            shaderc_compile_options_set_invert_y(_handle, enable);
        }

        public void SetNaNClamp(bool enable)
        {
            shaderc_compile_options_set_nan_clamp(_handle, enable);
        }
    }
}
