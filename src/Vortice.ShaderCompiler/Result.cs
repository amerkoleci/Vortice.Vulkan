// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler;

public class Result : IDisposable
{
    private nint _handle;

    internal Result(nint handle)
    {
        _handle = handle;
    }

    public CompilationStatus Status => shaderc_result_get_compilation_status(_handle);

    public nuint Length => shaderc_result_get_length(_handle);
    public uint WarningsCount => (uint)shaderc_result_get_num_warnings(_handle);
    public uint ErrorsCount => (uint)shaderc_result_get_num_errors(_handle);

    /// <summary>
    /// Returns a null-terminated string that contains any error messages generated
    /// during the compilation.
    /// </summary>
    public string? ErrorMessage => Marshal.PtrToStringAnsi(shaderc_result_get_error_message(_handle));

    public unsafe byte* GetBytes() => shaderc_result_get_bytes(_handle);

    public unsafe Span<byte> GetBytecode()
    {
        return new Span<byte>(shaderc_result_get_bytes(_handle), (int)shaderc_result_get_length(_handle));
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Result" /> class.
    /// </summary>
    ~Result() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_handle == 0)
            return;

        if (disposing)
        {
            shaderc_result_release(_handle);
        }
    }
}
