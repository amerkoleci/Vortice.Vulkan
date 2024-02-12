// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler;

public interface IIncluder
{
    void Activate(Options options);
    void Dispose(Options options);
}

public unsafe class Includer : IIncluder
{
    public string RootPath;

    private GCHandle _includerGCHandle = new();
    private readonly Dictionary<string, string> _sourceToPath = [];

    public Includer(string rootPath = ".")
    {
        RootPath = rootPath;
    }

    public void Activate(Options options)
    {
        if (!_includerGCHandle.IsAllocated)
            _includerGCHandle = GCHandle.Alloc(this);
        shaderc_compile_options_set_include_callbacks(options.Handle,
            &shaderc_include_resolve, &shaderc_include_result_release, GCHandle.ToIntPtr(_includerGCHandle));
    }

    public void Dispose(Options options)
    {
        shaderc_compile_options_set_include_callbacks(options.Handle, null, null, 0);
        if (_includerGCHandle.IsAllocated)
            _includerGCHandle.Free();
    }

    [UnmanagedCallersOnly]
    private static shaderc_include_result* shaderc_include_resolve(nint user_data,
        byte* requested_source,
        int type,
        byte* requesting_source,
        nuint include_depth)
    {
        GCHandle gch = GCHandle.FromIntPtr((IntPtr)user_data);
        Includer includer = (Includer)gch.Target!;

        shaderc_include_result* result = (shaderc_include_result*)NativeMemory.Alloc((nuint)sizeof(shaderc_include_result));

        string requestedSource = Utils.GetString(requested_source);
        string requestingSource = Utils.GetString(requesting_source);
        string loadPath;
        if (!Path.IsPathRooted(requestedSource))
        {
            string rootPath = includer.RootPath;
            if (includer._sourceToPath.ContainsKey(requestingSource))
            {
                rootPath = Path.GetDirectoryName(includer._sourceToPath[requestingSource])!;
            }

            loadPath = Path.Combine(rootPath, requestedSource);
        }
        else
        {
            loadPath = requestedSource;
        }

        string fileContent = File.ReadAllText(loadPath);

        result->content = Utils.ToUTF8(fileContent);
        result->content_length = (nuint)fileContent.Length;
        result->source_name = requested_source;
        result->source_name_length = (nuint)Utils.GetString(requesting_source).Length;

        includer._sourceToPath.Add(requestedSource, loadPath);
        return result;
    }

    [UnmanagedCallersOnly]
    private static void shaderc_include_result_release(nint user_data, shaderc_include_result* include_result)
    {
        Utils.FreeUTF8(include_result->content);
        NativeMemory.Free(include_result);
    }
}
