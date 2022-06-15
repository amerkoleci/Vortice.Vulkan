// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Vortice.SpirvCross;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SpvcParsedIr : IEquatable<SpvcParsedIr>
{
    public SpvcParsedIr(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static SpvcParsedIr Null => new(0);
    public static implicit operator SpvcParsedIr(nint handle) => new(handle);
    public static bool operator ==(SpvcParsedIr left, SpvcParsedIr right) => left.Handle == right.Handle;
    public static bool operator !=(SpvcParsedIr left, SpvcParsedIr right) => left.Handle != right.Handle;
    public static bool operator ==(SpvcParsedIr left, nint right) => left.Handle == right;
    public static bool operator !=(SpvcParsedIr left, nint right) => left.Handle != right;
    public bool Equals(SpvcParsedIr other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SpvcParsedIr handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => string.Format("SpvcParsedIr [0x{0}]", Handle.ToString("X"));
}

internal static unsafe class Native
{
    private static readonly IntPtr s_NativeLibrary = LoadNativeLibrary();

    public static readonly delegate* unmanaged[Cdecl]<out uint, out uint, out uint, void> spvc_get_version;
    public static readonly delegate* unmanaged[Cdecl]<out IntPtr, Result> spvc_context_create;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, void> spvc_context_destroy;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, sbyte*> spvc_context_get_last_error_string;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, void> spvc_context_release_allocations = (delegate* unmanaged[Cdecl]<IntPtr, void>)LoadFunction(nameof(spvc_context_release_allocations));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, spvc_error_callback, IntPtr, void> spvc_context_set_error_callback;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint*, nuint, out SpvcParsedIr, Result> spvc_context_parse_spirv;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, Backend, in SpvcParsedIr, CaptureMode, out IntPtr, Result> spvc_context_create_compiler;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> spvc_compiler_get_current_id_bound;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, out IntPtr, Result> spvc_compiler_create_compiler_options;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, byte, Result> spvc_compiler_options_set_bool;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, uint, Result> spvc_compiler_options_set_uint;
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Result> spvc_compiler_install_compiler_options;

    static Native()
    {
        spvc_get_version = (delegate* unmanaged[Cdecl]<out uint, out uint, out uint, void>)LoadFunction(nameof(spvc_get_version));
        spvc_context_create = (delegate* unmanaged[Cdecl]<out IntPtr, Result>)LoadFunction(nameof(spvc_context_create));
        spvc_context_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)LoadFunction(nameof(spvc_context_destroy));
        spvc_context_get_last_error_string = (delegate* unmanaged[Cdecl]<IntPtr, sbyte*>)LoadFunction(nameof(spvc_context_get_last_error_string));
        spvc_context_release_allocations = (delegate* unmanaged[Cdecl]<IntPtr, void>)LoadFunction(nameof(spvc_context_release_allocations));
        spvc_context_set_error_callback = (delegate* unmanaged[Cdecl]<IntPtr, spvc_error_callback, IntPtr, void>)LoadFunction(nameof(spvc_context_set_error_callback));
        spvc_context_parse_spirv = (delegate* unmanaged[Cdecl]<IntPtr, uint*, nuint, out SpvcParsedIr, Result>)LoadFunction(nameof(spvc_context_parse_spirv));
        spvc_context_create_compiler = (delegate* unmanaged[Cdecl]<IntPtr, Backend, in SpvcParsedIr, CaptureMode, out IntPtr, Result>)LoadFunction(nameof(spvc_context_create_compiler));
        spvc_compiler_get_current_id_bound = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LoadFunction(nameof(spvc_compiler_get_current_id_bound));
        spvc_compiler_create_compiler_options = (delegate* unmanaged[Cdecl]<IntPtr, out IntPtr, Result>)LoadFunction(nameof(spvc_compiler_create_compiler_options));
        spvc_compiler_options_set_bool = (delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, byte, Result>)LoadFunction(nameof(spvc_compiler_options_set_bool));
        spvc_compiler_options_set_uint = (delegate* unmanaged[Cdecl]<IntPtr, CompilerOption, uint, Result>)LoadFunction(nameof(spvc_compiler_options_set_uint));
        spvc_compiler_install_compiler_options = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, Result>)LoadFunction(nameof(spvc_compiler_install_compiler_options));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void spvc_error_callback(IntPtr userData, sbyte* error);

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
