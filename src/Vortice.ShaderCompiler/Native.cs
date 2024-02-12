// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;

namespace Vortice.ShaderCompiler;

internal static unsafe partial class Native
{
    private const string LibName = "shaderc_shared";

    static Native()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName.Equals(LibName) && TryResolveShaderc(assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveShaderc(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (NativeLibrary.TryLoad("shaderc_shared.dll", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad("libshaderc_shared.so", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (NativeLibrary.TryLoad("libshaderc_shared.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
        }

        if (NativeLibrary.TryLoad("shaderc_shared", assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        return false;
    }

    [LibraryImport(LibName)]
    public static partial nint shaderc_compiler_initialize();

    [LibraryImport(LibName)]
    public static partial void shaderc_compiler_release(nint handle);

    [LibraryImport(LibName)]
    public static partial nint shaderc_compile_options_initialize();

    [LibraryImport(LibName)]
    public static partial nint shaderc_compile_options_clone(nint handle);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_release(nint handle);

    [LibraryImport(LibName)]
    public static partial nint shaderc_compile_options_add_macro_definition(nint options, byte* name, nuint name_length, byte* value, nuint value_length);

    public static void shaderc_compile_options_add_macro_definition(nint options, string name, string? value)
    {
        fixed (byte* namePtr = name.GetUtf8Span())
        fixed (byte* valuePtr = value.GetUtf8Span())
            shaderc_compile_options_add_macro_definition(options, namePtr, (nuint)name.Length, valuePtr, string.IsNullOrEmpty(value) ? 0 : (nuint)value!.Length);
    }

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_source_language(nint options, SourceLanguage lang);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_generate_debug_info(nint options);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_optimization_level(nint options, OptimizationLevel level);

    [LibraryImport(LibName)]
    public static partial nint shaderc_compile_into_spv(nint compiler, byte* source, nuint source_size, int shader_kind, byte* input_file, byte* entry_point, nint additional_options);

    public static nint shaderc_compile_into_spv(nint compiler, string source, ShaderKind shaderKind, string inputFile, string entryPoint, nint additional_options)
    {
        fixed (byte* sourcePtr = source.GetUtf8Span())
        fixed (byte* inputFilePtr = inputFile.GetUtf8Span())
        fixed (byte* entryPointPtr = entryPoint.GetUtf8Span())
            return shaderc_compile_into_spv(compiler, sourcePtr, (nuint)source.Length, (int)shaderKind, inputFilePtr, entryPointPtr, additional_options);
    }

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_forced_version_profile(nint options, int version, Profile profile);

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
        public byte* source_name;
        public nuint source_name_length;
        /// <summary>
        /// The text contents of the source file in the normal case.
        /// For a failed inclusion, this contains the error message.
        /// </summary>
        public byte* content;
        public nuint content_length;
        /// <summary>
        /// User data to be passed along with this request.
        /// </summary>
        public nint user_data;
    }

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_include_callbacks(nint options,
        delegate* unmanaged<nint, byte*, int, byte*, nuint, shaderc_include_result*> resolver,
        delegate* unmanaged<nint, shaderc_include_result*, void> result_releaser, nint user_data);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_suppress_warnings(nint options);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_target_env(nint options, TargetEnvironment target, uint version);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_target_spirv(nint options, SpirVVersion version);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_warnings_as_errors(nint options);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_limit(nint options, Limit limit, int value);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_auto_bind_uniforms(nint options, byte auto_bind);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_hlsl_io_mapping(nint options, byte hlsl_iomap);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_hlsl_offsets(nint options, byte hlsl_offsets);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_binding_base(nint options, UniformKind kind, uint _base);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_binding_base_for_stage(nint options, ShaderKind shader_kind, UniformKind kind, uint _base);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_auto_map_locations(nint options, byte auto_map);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(nint options, ShaderKind shader_kind, byte* reg, byte* set, byte* binding);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_hlsl_register_set_and_binding(nint options, byte* reg, byte* set, byte* binding);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_hlsl_functionality1(nint options, byte enable);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_invert_y(nint options, byte enable);

    [LibraryImport(LibName)]
    public static partial void shaderc_compile_options_set_nan_clamp(nint options, byte enable);

    [LibraryImport(LibName)]
    public static partial void shaderc_result_release(nint handle);

    [LibraryImport(LibName)]
    public static partial nuint shaderc_result_get_length(nint result);

    [LibraryImport(LibName)]
    public static partial nuint shaderc_result_get_num_warnings(nint result);

    [LibraryImport(LibName)]
    public static partial nuint shaderc_result_get_num_errors(nint result);

    [LibraryImport(LibName)]
    public static partial CompilationStatus shaderc_result_get_compilation_status(nint result);

    [LibraryImport(LibName)]
    public static partial byte* shaderc_result_get_bytes(nint result);

    [LibraryImport(LibName)]
    public static partial nint shaderc_result_get_error_message(nint result);

    [LibraryImport(LibName)]
    public static partial void shaderc_get_spv_version(out uint version, out uint revision);

    [LibraryImport(LibName)]
    public static partial byte shaderc_parse_version_profile(byte* str, int* version, Profile* profile);

    public static void shaderc_parse_version_profile(string str, int* version, Profile* profile)
    {
        fixed (byte* dataPtr = str.GetUtf8Span())
        {
            shaderc_parse_version_profile(dataPtr, version, profile);
        }
    }
}
