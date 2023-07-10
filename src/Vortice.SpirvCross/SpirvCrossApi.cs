// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.SpirvCross.spvc_result;
using static Vortice.SpirvCross.Utils;

namespace Vortice.SpirvCross;

unsafe partial class SpirvCrossApi
{
    private const string LibName = "spirv-cross";

    static SpirvCrossApi()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName.Equals(LibName) && TryResolveSpirvCross(assembly, searchPath, out nint nativeLibrary))
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfFailed(spvc_result result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result != SPVC_SUCCESS)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new SpirvCrossException(result, message);
        }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DebugThrowIfFailed(spvc_result result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result != SPVC_SUCCESS)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new SpirvCrossException(result, message);
        }
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void CheckResult(this spvc_result result, string message = "SPIRV-Cross error occured")
    {
        if (result != SPVC_SUCCESS)
        {
            throw new SpirvCrossException(result, message);
        }
    }

    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void DebugCheckResult(this spvc_result result, string message = "SPIRV-Cross error occured")
    {
        if (result != SPVC_SUCCESS)
        {
            throw new SpirvCrossException(result, message);
        }
    }

    #region Context
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_get_version")]
    public static extern void spvc_get_version(out uint major, out uint minor, out uint patch);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_context_set_error_callback")]
    public static extern void spvc_context_set_error_callback(spvc_context context, delegate* unmanaged[Cdecl]<nint, sbyte*, void> callback, nint userData);

    public static spvc_result spvc_context_parse_spirv(spvc_context context, byte[] bytecode, out spvc_parsed_ir parsed_ir)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvc_context_parse_spirv(context,
                (uint*)bytecodePtr,
                (nuint)bytecode.Length / sizeof(uint),
                out parsed_ir);
        }
    }

    public static spvc_result spvc_context_parse_spirv(spvc_context context, ReadOnlySpan<byte> bytecode, out spvc_parsed_ir parsed_ir)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvc_context_parse_spirv(context, (uint*)bytecodePtr, (nuint)bytecode.Length / sizeof(uint), out parsed_ir);
        }
    }

    public static spvc_result spvc_context_parse_spirv(spvc_context context, uint[] spirv, out spvc_parsed_ir parsed_ir)
    {
        fixed (uint* spirvPtr = spirv)
        {
            return spvc_context_parse_spirv(context, spirvPtr, (nuint)spirv.Length, out parsed_ir);
        }
    }

    public static string? spvc_context_get_last_error_string(spvc_context context)
    {
        sbyte* native = spvc_context_get_last_error_stringPrivate(context);
        return Utils.GetString(Utils.GetUtf8Span(native));
    }
    #endregion

    #region Compiler
    public static spvc_result spvc_compiler_compile(spvc_compiler compiler, out string? source)
    {
        sbyte* utf8Str = default;
        spvc_result result = spvc_compiler_compile(compiler, (sbyte*)&utf8Str);
        if (result != SPVC_SUCCESS)
        {
            source = default;
            return result;
        }

        source = new string(utf8Str);
        return result;
    }

    public static void spvc_compiler_add_header_line(spvc_compiler compiler, ReadOnlySpan<sbyte> line)
    {
        fixed (sbyte* dataPtr = line)
        {
            spvc_compiler_add_header_line(compiler, dataPtr);
        }
    }

    public static void spvc_compiler_add_header_line(spvc_compiler compiler, string line)
    {
        fixed (sbyte* dataPtr = line.GetUtf8Span())
        {
            spvc_compiler_add_header_line(compiler, dataPtr);
        }
    }

    public static void spvc_compiler_require_extension(spvc_compiler compiler, ReadOnlySpan<sbyte> ext)
    {
        fixed (sbyte* dataPtr = ext)
        {
            spvc_compiler_require_extension(compiler, dataPtr);
        }
    }

    public static void spvc_compiler_require_extension(spvc_compiler compiler, string ext)
    {
        fixed (sbyte* dataPtr = ext.GetUtf8Span())
        {
            spvc_compiler_require_extension(compiler, dataPtr);
        }
    }

    public static void spvc_compiler_set_name(spvc_compiler compiler, uint id, ReadOnlySpan<sbyte> argument)
    {
        fixed (sbyte* dataPtr = argument)
        {
            spvc_compiler_set_name(compiler, id, dataPtr);
        }
    }

    public static void spvc_compiler_set_name(spvc_compiler compiler, uint id, string argument)
    {
        fixed (sbyte* dataPtr = argument.GetUtf8Span())
        {
            spvc_compiler_set_name(compiler, id, dataPtr);
        }
    }

    public static string? spvc_compiler_get_name(spvc_compiler compiler, uint id)
    {
        return GetUtf8Span(spvc_compiler_get_namePrivate(compiler, id)).GetString();
    }
    #endregion

    #region Resources
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_resources_get_resource_list_for_type")]
    public static extern spvc_result spvc_resources_get_resource_list_for_type(spvc_resources resources, spvc_resource_type type, out spvc_reflected_resource* resource_list, out nuint resource_size);
    #endregion
}
