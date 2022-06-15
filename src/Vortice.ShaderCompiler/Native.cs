// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.ShaderCompiler;

internal static unsafe class Native
{
    private static readonly IntPtr s_NativeLibrary = LoadNativeLibrary();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_InitializeFunc();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_ReleaseFunc(nint handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_CloneFunc(nint handle);

    private static PFN_InitializeFunc shaderc_compiler_initialize_ = LoadFunction<PFN_InitializeFunc>(nameof(shaderc_compiler_initialize));
    public static nint shaderc_compiler_initialize() => shaderc_compiler_initialize_();

    private static readonly PFN_ReleaseFunc shaderc_compiler_release_ = LoadFunction<PFN_ReleaseFunc>(nameof(shaderc_compiler_release));
    public static void shaderc_compiler_release(nint handle) => shaderc_compiler_release_(handle);

    // Options
    private static PFN_InitializeFunc shaderc_compile_options_initialize_ = LoadFunction<PFN_InitializeFunc>(nameof(shaderc_compile_options_initialize));
    public static nint shaderc_compile_options_initialize() => shaderc_compile_options_initialize_();

    private static readonly PFN_CloneFunc shaderc_compile_options_clone_ = LoadFunction<PFN_CloneFunc>(nameof(shaderc_compile_options_clone));
    public static nint shaderc_compile_options_clone(nint handle) => shaderc_compile_options_clone_(handle);

    private static readonly PFN_ReleaseFunc shaderc_compile_options_release_ = LoadFunction<PFN_ReleaseFunc>(nameof(shaderc_compile_options_release));
    public static void shaderc_compile_options_release(nint handle) => shaderc_compile_options_release_(handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_add_macro_definition(nint options, string name, nuint name_length, string? value, nuint value_length);
    private static readonly PFN_shaderc_compile_options_add_macro_definition shaderc_compile_options_add_macro_definition_ = LoadFunction<PFN_shaderc_compile_options_add_macro_definition>(nameof(shaderc_compile_options_add_macro_definition));
    public static void shaderc_compile_options_add_macro_definition(nint options, string name, nuint name_length, string? value, nuint value_length)
    {
        shaderc_compile_options_add_macro_definition_(options, name, name_length, value, value_length);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_shaderc_compile_options_set_source_language(nint options, SourceLanguage lang);
    private static readonly PFN_shaderc_compile_options_set_source_language shaderc_compile_options_set_source_language_ = LoadFunction<PFN_shaderc_compile_options_set_source_language>("shaderc_compile_options_set_source_language");
    public static void shaderc_compile_options_set_source_language(nint options, SourceLanguage lang) => shaderc_compile_options_set_source_language_(options, lang);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_shaderc_compile_options_set_generate_debug_info(nint options);
    private static readonly PFN_shaderc_compile_options_set_generate_debug_info shaderc_compile_options_set_generate_debug_info_ = LoadFunction<PFN_shaderc_compile_options_set_generate_debug_info>("shaderc_compile_options_set_generate_debug_info");
    public static void shaderc_compile_options_set_generate_debug_info(nint options)
    {
        shaderc_compile_options_set_generate_debug_info_(options);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_shaderc_compile_options_set_optimization_level(nint options, OptimizationLevel level);
    private static readonly PFN_shaderc_compile_options_set_optimization_level shaderc_compile_options_set_optimization_level_ = LoadFunction<PFN_shaderc_compile_options_set_optimization_level>("shaderc_compile_options_set_optimization_level");
    public static void shaderc_compile_options_set_optimization_level(nint options, OptimizationLevel level)
    {
        shaderc_compile_options_set_optimization_level_(options, level);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private delegate nint PFN_shaderc_compile_into_spv(nint compiler, string source, nuint source_size, int shader_kind, string input_file, string entry_point, nint additional_options);

    private static readonly PFN_shaderc_compile_into_spv shaderc_compile_into_spv_ = LoadFunction<PFN_shaderc_compile_into_spv>(nameof(shaderc_compile_into_spv));
    public static nint shaderc_compile_into_spv(nint compiler, string source, nuint source_size, int shader_kind, string input_file, string entry_point, nint additional_options)
    {
        return shaderc_compile_into_spv_(compiler, source, source_size, shader_kind, input_file, entry_point, additional_options);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_forced_version_profile(nint options, int version, Profile profile);
    private static readonly PFN_shaderc_compile_options_set_forced_version_profile shaderc_compile_options_set_forced_version_profile_ = LoadFunction<PFN_shaderc_compile_options_set_forced_version_profile>("shaderc_compile_options_set_forced_version_profile");
    public static void shaderc_compile_options_set_forced_version_profile(nint options, int version, Profile profile)
    {
        shaderc_compile_options_set_forced_version_profile_(options, version, profile);
    }

    /// <summary>An include result. https://github.com/google/shaderc/blob/c42db5815fad0424f0f1311de1eec92cdd77203d/libshaderc/include/shaderc/shaderc.h#L325</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct shaderc_include_result
    {
        /// <summary>
        /// The name of the source file.  The name should be fully resolved
        /// in the sense that it should be a unique name in the context of the
        /// includer.  For example, if the includer maps source names to files in
        /// a filesystem, then this name should be the absolute path of the file.
        /// For a failed inclusion, this string is empty.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)] public string source_name;
        public UIntPtr source_name_length;
        /// <summary>
        /// The text contents of the source file in the normal case.
        /// For a failed inclusion, this contains the error message.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)] public string content;
        public UIntPtr content_length;
        /// <summary>
        /// User data to be passed along with this request.
        /// </summary>
        public void* user_data;
    }
    /// <returns>shaderc_include_result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint PFN_shaderc_include_resolve_fn(void* user_data, [MarshalAs(UnmanagedType.LPStr)] string requested_source, int type, [MarshalAs(UnmanagedType.LPStr)] string requesting_source, UIntPtr include_depth);
    /// <param name="user_data">User data</param>
    /// <param name="include_result">shaderc_include_result</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PFN_shaderc_include_result_release_fn(void* user_data, nint include_result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_include_callbacks(nint options, PFN_shaderc_include_resolve_fn resolver, PFN_shaderc_include_result_release_fn result_releaser, void* user_data);
    private static readonly PFN_shaderc_compile_options_set_include_callbacks shaderc_compile_options_set_include_callbacks_ = LoadFunction<PFN_shaderc_compile_options_set_include_callbacks>("shaderc_compile_options_set_include_callbacks");
    public static void shaderc_compile_options_set_include_callbacks(nint options, PFN_shaderc_include_resolve_fn resolver, PFN_shaderc_include_result_release_fn result_releaser, void* user_data)
    {
        shaderc_compile_options_set_include_callbacks_(options, resolver, result_releaser, user_data);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_shaderc_compile_options_set_suppress_warnings(nint options);
    private static readonly PFN_shaderc_compile_options_set_suppress_warnings shaderc_compile_options_set_suppress_warnings_ = LoadFunction<PFN_shaderc_compile_options_set_suppress_warnings>("shaderc_compile_options_set_suppress_warnings");
    public static void shaderc_compile_options_set_suppress_warnings(nint options) => shaderc_compile_options_set_suppress_warnings_(options);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_target_env(nint options, TargetEnvironment target, uint version);
    private static readonly PFN_shaderc_compile_options_set_target_env shaderc_compile_options_set_target_env_ = LoadFunction<PFN_shaderc_compile_options_set_target_env>("shaderc_compile_options_set_target_env");
    public static void shaderc_compile_options_set_target_env(nint options, TargetEnvironment target, uint version)
    {
        shaderc_compile_options_set_target_env_(options, target, version);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_target_spirv(nint options, SpirVVersion version);
    private static readonly PFN_shaderc_compile_options_set_target_spirv shaderc_compile_options_set_target_spirv_ = LoadFunction<PFN_shaderc_compile_options_set_target_spirv>("shaderc_compile_options_set_target_spirv");
    public static void shaderc_compile_options_set_target_spirv(nint options, SpirVVersion version)
    {
        shaderc_compile_options_set_target_spirv_(options, version);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_warnings_as_errors(nint option);
    private static readonly PFN_shaderc_compile_options_set_warnings_as_errors shaderc_compile_options_set_warnings_as_errors_ = LoadFunction<PFN_shaderc_compile_options_set_warnings_as_errors>("shaderc_compile_options_set_warnings_as_errors");
    public static void shaderc_compile_options_set_warnings_as_errors(nint options)
    {
        shaderc_compile_options_set_warnings_as_errors_(options);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_shaderc_compile_options_set_limit(nint options, Limit limit, int value);
    private static readonly PFN_shaderc_compile_options_set_limit shaderc_compile_options_set_limit_ = LoadFunction<PFN_shaderc_compile_options_set_limit>("shaderc_compile_options_set_limit");
    public static void shaderc_compile_options_set_limit(nint options, Limit limit, int value)
    {
        shaderc_compile_options_set_limit_(options, limit, value);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_auto_bind_uniforms(nint options, bool auto_bind);
    private static readonly PFN_shaderc_compile_options_set_auto_bind_uniforms shaderc_compile_options_set_auto_bind_uniforms_ = LoadFunction<PFN_shaderc_compile_options_set_auto_bind_uniforms>("shaderc_compile_options_set_auto_bind_uniforms");
    public static void shaderc_compile_options_set_auto_bind_uniforms(nint options, bool auto_bind)
    {
        shaderc_compile_options_set_auto_bind_uniforms_(options, auto_bind);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_hlsl_io_mapping(nint options, bool hlsl_iomap);
    private static readonly PFN_shaderc_compile_options_set_hlsl_io_mapping shaderc_compile_options_set_hlsl_io_mapping_ = LoadFunction<PFN_shaderc_compile_options_set_hlsl_io_mapping>("shaderc_compile_options_set_hlsl_io_mapping");
    public static void shaderc_compile_options_set_hlsl_io_mapping(nint options, bool hlsl_iomap)
    {
        shaderc_compile_options_set_hlsl_io_mapping_(options, hlsl_iomap);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_hlsl_offsets(nint options, bool hlsl_offsets);
    private static readonly PFN_shaderc_compile_options_set_hlsl_offsets shaderc_compile_options_set_hlsl_offsets_ = LoadFunction<PFN_shaderc_compile_options_set_hlsl_offsets>("shaderc_compile_options_set_hlsl_offsets");
    public static void shaderc_compile_options_set_hlsl_offsets(nint options, bool hlsl_offsets)
    {
        shaderc_compile_options_set_hlsl_offsets_(options, hlsl_offsets);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_binding_base(nint options, UniformKind kind, uint _base);
    private static readonly PFN_shaderc_compile_options_set_binding_base shaderc_compile_options_set_binding_base_ = LoadFunction<PFN_shaderc_compile_options_set_binding_base>("shaderc_compile_options_set_binding_base");
    public static void shaderc_compile_options_set_binding_base(nint options, UniformKind kind, uint _base)
    {
        shaderc_compile_options_set_binding_base_(options, kind, _base);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_binding_base_for_stage(nint options, ShaderKind shader_kind, UniformKind kind, uint _base);
    private static readonly PFN_shaderc_compile_options_set_binding_base_for_stage shaderc_compile_options_set_binding_base_for_stage_ = LoadFunction<PFN_shaderc_compile_options_set_binding_base_for_stage>("shaderc_compile_options_set_binding_base_for_stage");
    public static void shaderc_compile_options_set_binding_base_for_stage(nint options, ShaderKind shader_kind, UniformKind kind, uint _base)
    {
        shaderc_compile_options_set_binding_base_for_stage_(options, shader_kind, kind, _base);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_auto_map_locations(nint options, bool auto_map);
    private static readonly PFN_shaderc_compile_options_set_auto_map_locations shaderc_compile_options_set_auto_map_locations_ = LoadFunction<PFN_shaderc_compile_options_set_auto_map_locations>("shaderc_compile_options_set_auto_map_locations");
    public static void shaderc_compile_options_set_auto_map_locations(nint options, bool auto_map)
    {
        shaderc_compile_options_set_auto_map_locations_(options, auto_map);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(nint options, ShaderKind shader_kind, string reg, string set, string binding);
    private static readonly PFN_shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage_ = LoadFunction<PFN_shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage>("shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage");
    public static void shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(nint options, ShaderKind shader_kind, string reg, string set, string binding)
    {
        shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage_(options, shader_kind, reg, set, binding);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_hlsl_register_set_and_binding(nint options, string reg, string set, string binding);
    private static readonly PFN_shaderc_compile_options_set_hlsl_register_set_and_binding shaderc_compile_options_set_hlsl_register_set_and_binding_ = LoadFunction<PFN_shaderc_compile_options_set_hlsl_register_set_and_binding>("shaderc_compile_options_set_hlsl_register_set_and_binding");
    public static void shaderc_compile_options_set_hlsl_register_set_and_binding(nint options, string reg, string set, string binding)
    {
        shaderc_compile_options_set_hlsl_register_set_and_binding_(options, reg, set, binding);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_hlsl_functionality1(nint options, bool enable);
    private static readonly PFN_shaderc_compile_options_set_hlsl_functionality1 shaderc_compile_options_set_hlsl_functionality1_ = LoadFunction<PFN_shaderc_compile_options_set_hlsl_functionality1>("shaderc_compile_options_set_hlsl_functionality1");
    public static void shaderc_compile_options_set_hlsl_functionality1(nint options, bool enable)
    {
        shaderc_compile_options_set_hlsl_functionality1_(options, enable);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_invert_y(nint options, bool enable);
    private static readonly PFN_shaderc_compile_options_set_invert_y shaderc_compile_options_set_invert_y_ = LoadFunction<PFN_shaderc_compile_options_set_invert_y>("shaderc_compile_options_set_invert_y");
    public static void shaderc_compile_options_set_invert_y(nint options, bool enable)
    {
        shaderc_compile_options_set_invert_y_(options, enable);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_compile_options_set_nan_clamp(nint options, bool enable);
    private static readonly PFN_shaderc_compile_options_set_nan_clamp shaderc_compile_options_set_nan_clamp_ = LoadFunction<PFN_shaderc_compile_options_set_nan_clamp>("shaderc_compile_options_set_nan_clamp");
    public static void shaderc_compile_options_set_nan_clamp(nint options, bool enable)
    {
        shaderc_compile_options_set_nan_clamp_(options, enable);
    }

    // Result
    private static readonly PFN_ReleaseFunc shaderc_result_release_ = LoadFunction<PFN_ReleaseFunc>(nameof(shaderc_result_release));
    public static void shaderc_result_release(nint handle) => shaderc_result_release_(handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nuint PFN_shaderc_result_get_length(nint result);
    private static readonly PFN_shaderc_result_get_length shaderc_result_get_length_ = LoadFunction<PFN_shaderc_result_get_length>(nameof(shaderc_result_get_length));
    public static nuint shaderc_result_get_length(nint result) => shaderc_result_get_length_(result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nuint PFN_shaderc_result_get_num_warnings(nint result);
    private static readonly PFN_shaderc_result_get_num_warnings shaderc_result_get_num_warnings_ = LoadFunction<PFN_shaderc_result_get_num_warnings>(nameof(shaderc_result_get_num_warnings));
    public static nuint shaderc_result_get_num_warnings(nint result) => shaderc_result_get_num_warnings_(result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nuint PFN_shaderc_result_get_num_errors(nint result);
    private static readonly PFN_shaderc_result_get_num_errors shaderc_result_get_num_errors_ = LoadFunction<PFN_shaderc_result_get_num_errors>(nameof(shaderc_result_get_num_errors));
    public static nuint shaderc_result_get_num_errors(nint result) => shaderc_result_get_num_errors_(result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate CompilationStatus PFN_shaderc_result_get_compilation_status(nint result);
    private static readonly PFN_shaderc_result_get_compilation_status shaderc_result_get_compilation_status_ = LoadFunction<PFN_shaderc_result_get_compilation_status>(nameof(shaderc_result_get_compilation_status));
    public static CompilationStatus shaderc_result_get_compilation_status(nint result) => shaderc_result_get_compilation_status_(result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate byte* PFN_shaderc_result_get_bytes(nint result);
    private static readonly PFN_shaderc_result_get_bytes shaderc_result_get_bytes_ = LoadFunction<PFN_shaderc_result_get_bytes>(nameof(shaderc_result_get_bytes));
    public static byte* shaderc_result_get_bytes(nint result) => shaderc_result_get_bytes_(result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint PFN_shaderc_result_get_error_message(nint result);
    private static readonly PFN_shaderc_result_get_error_message shaderc_result_get_error_message_ = LoadFunction<PFN_shaderc_result_get_error_message>(nameof(shaderc_result_get_error_message));
    public static nint shaderc_result_get_error_message(nint result) => shaderc_result_get_error_message_(result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PFN_shaderc_get_spv_version(out uint version, out uint revision);
    private static readonly PFN_shaderc_get_spv_version shaderc_get_spv_version_ = LoadFunction<PFN_shaderc_get_spv_version>(nameof(shaderc_get_spv_version));
    public static void shaderc_get_spv_version(out uint version, out uint revision) => shaderc_get_spv_version_(out version, out revision);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint PFN_shaderc_parse_version_profile(string str, int* version, Profile* profile);
    private static readonly PFN_shaderc_parse_version_profile shaderc_parse_version_profile_ = LoadFunction<PFN_shaderc_parse_version_profile>("shaderc_parse_version_profile");
    public static bool shaderc_parse_version_profile(string str, int* version, Profile* profile) => shaderc_parse_version_profile_(str, version, profile) == 1;

    private static IntPtr LoadNativeLibrary()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return LibraryLoader.LoadLocalLibrary("shaderc_shared.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return LibraryLoader.LoadLocalLibrary("libshaderc_shared.so");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return LibraryLoader.LoadLocalLibrary("libshaderc_shared.dylib");
        }
        else
        {
            return LibraryLoader.LoadLocalLibrary("shaderc_shared");
        }
    }

    private static T LoadFunction<T>(string name) => LibraryLoader.LoadFunction<T>(s_NativeLibrary, name);
}
