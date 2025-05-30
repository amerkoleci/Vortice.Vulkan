﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

unsafe partial class Vulkan
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfFailed(VkResult result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result < VK_SUCCESS)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new VkException(result, message);
        }
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void CheckResult(this VkResult result, string message = "Vulkan error occured")
    {
        if (result < VK_SUCCESS)
        {
            throw new VkException(result, message);
        }
    }
}
