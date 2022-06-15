// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

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
    private Dictionary<string, IntPtr> _shadercIncludeResults = new();
    private Dictionary<IntPtr, string> _ptrToName = new();
    private Dictionary<string, string> _sourceToPath = new();

    public unsafe Includer(string rootPath = ".")
    {
        RootPath = rootPath;
    }

    public unsafe void Activate(Options options)
    {
        if (!_includerGCHandle.IsAllocated)
            _includerGCHandle = GCHandle.Alloc(this);
        shaderc_compile_options_set_include_callbacks(options.Handle, shaderc_include_resolve, shaderc_include_result_release, (void*)GCHandle.ToIntPtr(_includerGCHandle));
    }

    public unsafe void Dispose(Options options)
    {
#pragma warning disable CS8600, CS8625
        shaderc_compile_options_set_include_callbacks(options.Handle, null, null, null);
#pragma warning restore CS8600, CS8625
        foreach (var includeResultPtr in _shadercIncludeResults.Values)
            Free(includeResultPtr);
        _sourceToPath = new();
        _ptrToName = new();
        _shadercIncludeResults = new();
        if (_includerGCHandle.IsAllocated)
            _includerGCHandle.Free();
    }

    private static unsafe nint shaderc_include_resolve(void* user_data, [MarshalAs(UnmanagedType.LPStr)] string requested_source, int type, [MarshalAs(UnmanagedType.LPStr)] string requesting_source, UIntPtr include_depth)
    {
        GCHandle gch = GCHandle.FromIntPtr((IntPtr)user_data);
#pragma warning disable CS8600
        Includer includer = (Includer)gch.Target;
#pragma warning restore CS8600

#pragma warning disable CS8602
        if (!includer._shadercIncludeResults.TryGetValue(requested_source, out IntPtr includeResultPtr))
#pragma warning restore CS8602
        {
            Native.shaderc_include_result includeResult = new();
            string path = requested_source;
            if (!Path.IsPathRooted(path))
            {
                string rootPath = includer.RootPath;
                if (includer._sourceToPath.ContainsKey(requesting_source))
                {
                    rootPath = Path.GetDirectoryName(includer._sourceToPath[requesting_source]);
                }
                path = Path.Combine(rootPath, path);
            }
            includeResult.content = File.ReadAllText(path);
            includeResult.content_length = (UIntPtr)includeResult.content.Length;
            includeResult.source_name = requested_source;
            includeResult.source_name_length = (UIntPtr)includeResult.source_name.Length;

            includeResultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(includeResult));
            Marshal.StructureToPtr(includeResult, includeResultPtr, false);
            includer._shadercIncludeResults.Add(requested_source, includeResultPtr);
            includer._ptrToName.Add(includeResultPtr, requested_source);
            includer._sourceToPath.Add(requested_source, path);
        }
        return includeResultPtr;
    }

    private static unsafe void shaderc_include_result_release(void* user_data, nint include_result)
    {
        GCHandle gch = GCHandle.FromIntPtr((IntPtr)user_data);
#pragma warning disable CS8600
        Includer includer = (Includer)gch.Target;
#pragma warning restore CS8600

#pragma warning disable CS8602
        if (includer._ptrToName.TryGetValue(include_result, out var path))
#pragma warning restore CS8602
        {
            includer._ptrToName.Remove(include_result);
            includer._shadercIncludeResults.Remove(path);
            includer.Free(include_result);
        }
    }

    private unsafe void Free(IntPtr includeResultPtr)
    {
        Marshal.DestroyStructure(includeResultPtr, typeof(Native.shaderc_include_result));
        Marshal.FreeHGlobal(includeResultPtr);
    }
}
