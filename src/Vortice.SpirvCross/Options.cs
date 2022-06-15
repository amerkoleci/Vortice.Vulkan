// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.SpirvCross.Native;

namespace Vortice.SpirvCross;

public sealed unsafe class Options
{
    internal readonly nint Handle;

    public Options(IntPtr handle)
    {
        Handle = handle;
    }

    public void SetBool(CompilerOption option, bool value)
    {
        spvc_compiler_options_set_bool(Handle, option, value ? (byte)1 : (byte)0).CheckResult();
    }

    public void SetUInt(CompilerOption option, uint value)
    {
        spvc_compiler_options_set_uint(Handle, option, value).CheckResult();
    }
}
