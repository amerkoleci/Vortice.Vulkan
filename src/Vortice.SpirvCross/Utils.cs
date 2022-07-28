// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Vortice.SpirvCross;

public static unsafe class Utils
{
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void CheckResult(this Result result, string message = "SPIRV-Cross error occured")
    {
        if (result != Result.Success)
        {
            throw new SpirvCrossException(result, message);
        }
    }
}
