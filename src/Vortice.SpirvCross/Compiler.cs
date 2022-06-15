// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.SpirvCross.Native;

namespace Vortice.SpirvCross;

public sealed unsafe class Compiler
{
    private readonly nint _handle;

    public Compiler(IntPtr handle)
    {
        _handle = handle;

        //spvc_compiler_create_compiler_options(_handle, out IntPtr optionsPtr).CheckResult();
        //Options = new(optionsPtr);
    }

    //public Options Options { get; }

    public uint CurrentIdBound => spvc_compiler_get_current_id_bound(_handle);

    public void Apply()
    {
        //spvc_compiler_install_compiler_options(_handle, Options.Handle).CheckResult();
    }
}
