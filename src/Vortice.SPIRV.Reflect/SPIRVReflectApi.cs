// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.SPIRV.Reflect.Utils;

namespace Vortice.SPIRV.Reflect;

unsafe partial class SPIRVReflectApi
{
    private const string LibName = "spirv-reflect";

    /// <unmanaged>SPV_REFLECT_MAX_ARRAY_DIMS</unmanaged>
    public const int MaxArrayDims = 32;

    /// <unmanaged>SPV_REFLECT_MAX_DESCRIPTOR_SETS</unmanaged>
    public const int MaxDescriptorSets = 64;

    /// <unmanaged>SPV_REFLECT_BINDING_NUMBER_DONT_CHANGE</unmanaged>
    public const uint BindingNumberDontChange = unchecked((uint)~0);

    /// <unmanaged>SPV_REFLECT_SET_NUMBER_DONT_CHANGE</unmanaged>
    public const uint SetNumberDontChange = unchecked((uint)~0);

    static SPIRVReflectApi()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName.Equals(LibName) && TryResolveSpirvCross(assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveSpirvCross(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (NativeLibrary.TryLoad("spirv-reflect.dll", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad("libspirv-reflect.so", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (NativeLibrary.TryLoad("libspirv-reflect.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
        }

        if (NativeLibrary.TryLoad("spirv-reflect", assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfFailed(SpvReflectResult result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result != SpvReflectResult.Success)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new SPIRVReflectException(result, message);
        }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DebugThrowIfFailed(SpvReflectResult result, [CallerArgumentExpression(nameof(result))] string? valueExpression = null)
    {
        if (result != SpvReflectResult.Success)
        {
            string message = string.Format("'{0}' failed with an error result of '{1}'", valueExpression ?? "Method", result);
            throw new SPIRVReflectException(result, message);
        }
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void CheckResult(this SpvReflectResult result, string message = "SPIRV-Cross error occured")
    {
        if (result != SpvReflectResult.Success)
        {
            throw new SPIRVReflectException(result, message);
        }
    }

    [Conditional("DEBUG")]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void DebugCheckResult(this SpvReflectResult result, string message = "SPIRV-Cross error occured")
    {
        if (result != SpvReflectResult.Success)
        {
            throw new SPIRVReflectException(result, message);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule(byte[] bytecode, SpvReflectShaderModule* module)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvReflectCreateShaderModule((nuint)bytecode.Length, bytecodePtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule(ReadOnlySpan<byte> bytecode, SpvReflectShaderModule* module)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvReflectCreateShaderModule((nuint)bytecode.Length, bytecodePtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule(uint[] spirv, SpvReflectShaderModule* module)
    {
        fixed (uint* spirvPtr = spirv)
        {
            return spvReflectCreateShaderModule((nuint)spirv.Length / sizeof(uint), spirvPtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule(ReadOnlySpan<uint> spirv, SpvReflectShaderModule* module)
    {
        fixed (uint* spirvPtr = spirv)
        {
            return spvReflectCreateShaderModule((nuint)spirv.Length / sizeof(uint), spirvPtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule2(SpvReflectModuleFlags flags, byte[] bytecode, SpvReflectShaderModule* module)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvReflectCreateShaderModule2(flags, (nuint)bytecode.Length, (uint*)bytecodePtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule2(SpvReflectModuleFlags flags, ReadOnlySpan<byte> bytecode, SpvReflectShaderModule* module)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            return spvReflectCreateShaderModule2(flags, (nuint)bytecode.Length, bytecodePtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule2(SpvReflectModuleFlags flags, uint[] spirv, SpvReflectShaderModule* module)
    {
        fixed (uint* spirvPtr = spirv)
        {
            return spvReflectCreateShaderModule2(flags, (nuint)spirv.Length / sizeof(uint), spirvPtr, module);
        }
    }

    public static SpvReflectResult spvReflectCreateShaderModule2(SpvReflectModuleFlags flags, ReadOnlySpan<uint> spirv, SpvReflectShaderModule* module)
    {
        fixed (uint* spirvPtr = spirv)
        {
            return spvReflectCreateShaderModule2(flags, (nuint)spirv.Length / sizeof(uint), spirvPtr, module);
        }
    }

    public static SpvReflectEntryPoint* spvReflectGetEntryPoint(SpvReflectShaderModule* module, ReadOnlySpan<byte> line)
    {
        fixed (byte* dataPtr = line)
        {
            return spvReflectGetEntryPoint(module, dataPtr);
        }
    }

    public static SpvReflectEntryPoint* spvReflectGetEntryPoint(SpvReflectShaderModule* module, string line)
    {
        fixed (byte* dataPtr = line.GetUtf8Span())
        {
            return spvReflectGetEntryPoint(module, dataPtr);
        }
    }
}
