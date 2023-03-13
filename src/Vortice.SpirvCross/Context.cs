// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.SpirvCross.SpirvCrossApi;

namespace Vortice.SpirvCross;

public sealed unsafe class Context : IDisposable
{
    private readonly nint _handle;

    public Context()
    {
        spvc_context_create(out _handle).CheckResult("Cannot create SPIRV-Cross context");
        spvc_context_set_error_callback(_handle, &OnErrorCallback, IntPtr.Zero);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Context" /> class.
    /// </summary>
    ~Context() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_handle == 0)
            return;

        if (disposing)
        {
            ReleaseAllocations();
            spvc_context_destroy(_handle);
        }
    }

    public void ReleaseAllocations() => spvc_context_release_allocations(_handle);

    public static void GetVersion(out uint major, out uint minor, out uint patch) => spvc_get_version(out major, out minor, out patch);

    public string GetLastErrorString()
    {
        return new string(spvc_context_get_last_error_string(_handle));
    }

    public Result ParseSpirv(byte[] bytecode, out SpvcParsedIr parsed_ir)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvc_context_parse_spirv(_handle,
                (uint*)bytecodePtr,
                (nuint)bytecode.Length / sizeof(uint),
                out parsed_ir);
        }
    }

    public Result ParseSpirv(ReadOnlySpan<byte> bytecode, out SpvcParsedIr parsed_ir)
    {
        return spvc_context_parse_spirv(_handle,
            (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(bytecode)),
            (nuint)bytecode.Length / sizeof(uint),
            out parsed_ir);
    }

    public Result ParseSpirv(uint[] spirv, out SpvcParsedIr parsed_ir)
    {
        fixed (uint* spirvPtr = spirv)
        {
            return spvc_context_parse_spirv(_handle, spirvPtr, (nuint)spirv.Length, out parsed_ir);
        }
    }

    public Result ParseSpirv(uint* spirv, nuint wordCount, out SpvcParsedIr parsed_ir)
    {
        return spvc_context_parse_spirv(_handle, spirv, wordCount, out parsed_ir);
    }

    public Compiler CreateCompiler(Backend backend, in SpvcParsedIr parsedIr, CaptureMode captureMode = CaptureMode.TakeOwnership)
    {
        spvc_context_create_compiler(_handle, backend, parsedIr, captureMode, out IntPtr compiler).CheckResult();
        return new Compiler(compiler);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void OnErrorCallback(IntPtr userData, sbyte* errorPtr)
    {
    }
}
