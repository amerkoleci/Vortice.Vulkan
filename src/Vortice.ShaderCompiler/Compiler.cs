// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler;

public class Compiler : IDisposable
{
    private nint _handle;

    public Compiler(Options? options = null)
    {
        _handle = shaderc_compiler_initialize();
        if (_handle == 0)
        {
            throw new Exception("Cannot initialize native handle");
        }

        Options = options ?? new Options();
    }

    public Options Options { get; }
    public IIncluder? Includer;

    /// <summary>
    /// Finalizes an instance of the <see cref="Compiler" /> class.
    /// </summary>
    ~Compiler() => Dispose(disposing: false);

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
            Includer?.Dispose(Options);
            Options.Dispose();
            shaderc_compiler_release(_handle);
        }
    }

    public Result Compile(string source, string fileName, ShaderKind shaderKind, string entryPoint = "main")
    {
        Includer?.Activate(Options);
        return new Result(shaderc_compile_into_spv(_handle, source, (nuint)source.Length, (byte)shaderKind, fileName, entryPoint, Options.Handle));
    }

    public static void GetSpvVersion(out uint version, out uint revision) => shaderc_get_spv_version(out version, out revision);
}
