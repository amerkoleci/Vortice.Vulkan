// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.SpirvCross;

internal static unsafe class SpirvCrossApi
{
    private static readonly IntPtr s_NativeLibrary = LoadNativeLibrary();


    public static readonly delegate* unmanaged[Cdecl]<out uint, out uint, out uint, void> spvc_get_version;
    public static readonly delegate* unmanaged[Cdecl]<out IntPtr, Result> spvc_context_create;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, void> spvc_context_destroy;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, sbyte*> spvc_context_get_last_error_string;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, void> spvc_context_release_allocations = (delegate* unmanaged[Cdecl]<IntPtr, void>)LoadFunction(nameof(spvc_context_release_allocations));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, delegate* unmanaged[Cdecl]<IntPtr, sbyte*, void>, IntPtr, void> spvc_context_set_error_callback;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint*, nuint, out SpvcParsedIr, Result> spvc_context_parse_spirv;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, Backend, IntPtr, CaptureMode, out IntPtr, Result> spvc_context_create_compiler;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> spvc_compiler_get_current_id_bound;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, out IntPtr, Result> spvc_compiler_create_compiler_options;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, byte, Result> spvc_compiler_options_set_bool;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, uint, Result> spvc_compiler_options_set_uint;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Result> spvc_compiler_install_compiler_options;

    public static readonly delegate* unmanaged[Cdecl]<IntPtr, sbyte*, Result> spvc_compiler_compile;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, byte*, Result> spvc_compiler_add_header_line;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, byte*, Result> spvc_compiler_require_extension;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, Result> spvc_compiler_flatten_buffer_block;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, byte> spvc_compiler_variable_is_depth_or_compare;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, uint, Result> spvc_compiler_mask_stage_output_by_location;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, SpvBuiltIn, Result> spvc_compiler_mask_stage_output_by_builtin;

    static SpirvCrossApi()
    {
        spvc_get_version = (delegate* unmanaged[Cdecl]<out uint, out uint, out uint, void>)LoadFunction(nameof(spvc_get_version));
        spvc_context_create = (delegate* unmanaged[Cdecl]<out IntPtr, Result>)LoadFunction(nameof(spvc_context_create));
        spvc_context_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)LoadFunction(nameof(spvc_context_destroy));
        spvc_context_get_last_error_string = (delegate* unmanaged[Cdecl]<IntPtr, sbyte*>)LoadFunction(nameof(spvc_context_get_last_error_string));
        spvc_context_release_allocations = (delegate* unmanaged[Cdecl]<IntPtr, void>)LoadFunction(nameof(spvc_context_release_allocations));
        spvc_context_set_error_callback = (delegate* unmanaged[Cdecl]<IntPtr, delegate* unmanaged[Cdecl]<IntPtr, sbyte*, void>, IntPtr, void>)LoadFunction(nameof(spvc_context_set_error_callback));
        spvc_context_parse_spirv = (delegate* unmanaged[Cdecl]<IntPtr, uint*, nuint, out SpvcParsedIr, Result>)LoadFunction(nameof(spvc_context_parse_spirv));
        spvc_context_create_compiler = (delegate* unmanaged[Cdecl]<IntPtr, Backend, IntPtr, CaptureMode, out IntPtr, Result>)LoadFunction(nameof(spvc_context_create_compiler));
        spvc_compiler_get_current_id_bound = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LoadFunction(nameof(spvc_compiler_get_current_id_bound));
        spvc_compiler_create_compiler_options = (delegate* unmanaged[Cdecl]<IntPtr, out IntPtr, Result>)LoadFunction(nameof(spvc_compiler_create_compiler_options));
        spvc_compiler_options_set_bool = (delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, byte, Result>)LoadFunction(nameof(spvc_compiler_options_set_bool));
        spvc_compiler_options_set_uint = (delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, uint, Result>)LoadFunction(nameof(spvc_compiler_options_set_uint));
        spvc_compiler_install_compiler_options = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Result>)LoadFunction(nameof(spvc_compiler_install_compiler_options));

        spvc_compiler_compile = (delegate* unmanaged[Cdecl]<IntPtr, sbyte*, Result>)LoadFunction(nameof(spvc_compiler_compile));
        spvc_compiler_add_header_line = (delegate* unmanaged[Cdecl]<IntPtr, byte*, Result>)LoadFunction(nameof(spvc_compiler_add_header_line));
        spvc_compiler_require_extension = (delegate* unmanaged[Cdecl]<IntPtr, byte*, Result>)LoadFunction(nameof(spvc_compiler_require_extension));
        spvc_compiler_flatten_buffer_block = (delegate* unmanaged[Cdecl]<IntPtr, uint, Result>)LoadFunction(nameof(spvc_compiler_flatten_buffer_block));
        spvc_compiler_variable_is_depth_or_compare = (delegate* unmanaged[Cdecl]<IntPtr, uint, byte>)LoadFunction(nameof(spvc_compiler_variable_is_depth_or_compare));

        spvc_compiler_mask_stage_output_by_location = (delegate* unmanaged[Cdecl]<IntPtr, uint, uint, Result>)LoadFunction(nameof(spvc_compiler_mask_stage_output_by_location));
        spvc_compiler_mask_stage_output_by_builtin = (delegate* unmanaged[Cdecl]<IntPtr, SpvBuiltIn, Result>)LoadFunction(nameof(spvc_compiler_mask_stage_output_by_builtin));
    }

    public static ReadOnlySpan<byte> GetUtf8(string str)
    {
        int maxLength = Encoding.UTF8.GetByteCount(str);
        var bytes = new byte[maxLength + 1];

        var length = Encoding.UTF8.GetBytes(str, bytes);
        return bytes.AsSpan(0, length);
    }

    private static IntPtr LoadNativeLibrary()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return LibraryLoader.LoadLocalLibrary("spirv-cross-c-shared.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return LibraryLoader.LoadLocalLibrary("libshaderc_shared.so");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return LibraryLoader.LoadLocalLibrary("libspirv-cross-c-shared.dylib");
        }

        throw new PlatformNotSupportedException("SPIRV-Cross is not supported");
    }

    private static IntPtr LoadFunction(string name) => LibraryLoader.GetSymbol(s_NativeLibrary, name);
}
