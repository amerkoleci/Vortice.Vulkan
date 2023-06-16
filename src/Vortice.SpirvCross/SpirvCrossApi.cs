// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.SpirvCross;

internal static unsafe class SpirvCrossApi
{
    public static event DllImportResolver? ResolveLibrary;

    private const string LibName = "spirv-cross";

    static SpirvCrossApi()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        if (libraryName.Equals(LibName) && TryResolveSpirvCross(assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveSpirvCross(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (NativeLibrary.TryLoad("spirv-cross.dll", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad("libspirv-cross.so", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (NativeLibrary.TryLoad("libspirv-cross.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
        }

        if (NativeLibrary.TryLoad("spirv-cross", assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        if (NativeLibrary.TryLoad("libspirv-cross", assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        return false;
    }

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary != null)
        {
            var resolvers = resolveLibrary.GetInvocationList();

            foreach (DllImportResolver resolver in resolvers)
            {
                nativeLibrary = resolver(libraryName, assembly, searchPath);

                if (nativeLibrary != 0)
                {
                    return true;
                }
            }
        }

        nativeLibrary = 0;
        return false;
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void spvc_get_version(out uint major, out uint minor, out uint patch);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_context_create(out IntPtr context);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void spvc_context_destroy(IntPtr context);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern sbyte* spvc_context_get_last_error_string(IntPtr context);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void spvc_context_release_allocations(IntPtr context);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void spvc_context_set_error_callback(IntPtr context, delegate* unmanaged[Cdecl]<IntPtr, sbyte*, void> callback, IntPtr userData);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_context_parse_spirv(IntPtr context, uint* spirv, nuint word_count, out SpvcParsedIr parsed_ir);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_context_create_compiler(IntPtr context, Backend backend, SpvcParsedIr parsedIr, CaptureMode mode, out IntPtr compiler);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint spvc_compiler_get_current_id_bound(IntPtr context);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_create_compiler_options(IntPtr context, out IntPtr options);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_options_set_bool(IntPtr options, CompilerOption option, byte value);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_options_set_uint(IntPtr options, CompilerOption option, uint value);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_install_compiler_options(IntPtr compiler, IntPtr options);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_compile(IntPtr context, sbyte* source);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_add_header_line(IntPtr compiler, sbyte* source);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_require_extension(IntPtr compiler, sbyte* source);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_flatten_buffer_block(IntPtr compiler, uint id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte spvc_compiler_variable_is_depth_or_compare(IntPtr compiler, uint id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_mask_stage_output_by_location(IntPtr compiler, uint location, uint component);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_mask_stage_output_by_builtin(IntPtr compiler, SpvBuiltIn builtin);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void spvc_compiler_set_name(IntPtr compiler, uint id, sbyte* source);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern sbyte* spvc_compiler_get_name(IntPtr compiler, uint id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_get_active_interface_variables(IntPtr compiler, out nint set);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_set_enabled_interface_variables(IntPtr compiler, nint set);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_create_shader_resources(IntPtr compiler, out Resources resources);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_compiler_create_shader_resources_for_active_variables(IntPtr compiler, out Resources resources, nint active);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_resources_get_resource_list_for_type(Resources resources, SpvResourceType type, SpvReflectedResource.Native** resource_list, nuint* resource_size);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result spvc_resources_get_builtin_resource_list_for_type(Resources resources, SpvcBuiltinResourceType type, SpvReflectedBuiltinResource.Native** resource_list, nuint* resource_size);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int spvc_compiler_get_decoration(IntPtr compiler, uint id, SpvDecoration decoration);
}
