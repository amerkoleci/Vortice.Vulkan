// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

public static class LibraryLoader
{
    private static string GetOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "win";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "linux";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "osx";

        throw new ArgumentException("Unsupported OS platform.");
    }

    private static string GetArchitecture()
    {
        switch (RuntimeInformation.ProcessArchitecture)
        {
            case Architecture.X86: return "x86";
            case Architecture.X64: return "x64";
            case Architecture.Arm: return "arm";
            case Architecture.Arm64: return "arm64";
        }

        throw new ArgumentException("Unsupported architecture.");
    }

    public static IntPtr LoadLibrary(string libraryName)
    {
        string libraryPath = GetNativeAssemblyPath(libraryName);

        IntPtr handle = LoadPlatformLibrary(libraryPath);
        if (handle == IntPtr.Zero)
            throw new DllNotFoundException($"Unable to load library '{libraryName}'.");

        return handle;

        static string GetNativeAssemblyPath(string libraryName)
        {
            string osPlatform = GetOSPlatform();
            string architecture = GetArchitecture();

            string assemblyLocation = Assembly.GetExecutingAssembly() != null ? Assembly.GetExecutingAssembly().Location : typeof(LibraryLoader).Assembly.Location;
            assemblyLocation = Path.GetDirectoryName(assemblyLocation);

            string[] paths = new[]
            {
                Path.Combine(assemblyLocation, libraryName),
                Path.Combine(assemblyLocation, "runtimes", osPlatform, "native", libraryName),
                Path.Combine(assemblyLocation, "runtimes", $"{osPlatform}-{architecture}", "native", libraryName),
                Path.Combine(assemblyLocation, "native", $"{osPlatform}-{architecture}", libraryName),
            };

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return libraryName;
        }
    }

    public static T LoadFunction<T>(IntPtr library, string name)
    {
#if NET5_0_OR_GREATER
        IntPtr symbol = NativeLibrary.GetExport(library, name);
#else
        IntPtr symbol = GetSymbol(library, name);

        if (symbol == IntPtr.Zero)
            throw new EntryPointNotFoundException($"Unable to load symbol '{name}'.");
#endif

        return Marshal.GetDelegateForFunctionPointer<T>(symbol);
    }

    private static IntPtr LoadPlatformLibrary(string libraryName)
    {
#if NET5_0_OR_GREATER
        return NativeLibrary.Load(libraryName);
#else
        IntPtr handle;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            handle = Win32.LoadLibrary(libraryName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            handle = Linux.dlopen(libraryName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            handle = Mac.dlopen(libraryName);
        else
            throw new PlatformNotSupportedException($"Current platform is unknown, unable to load library '{libraryName}'.");

        return handle;
#endif
    }

    public static IntPtr GetSymbol(IntPtr library, string symbolName)
    {
#if NET5_0_OR_GREATER
        return NativeLibrary.GetExport(library, symbolName);
#else
        if (string.IsNullOrEmpty(symbolName))
            throw new ArgumentNullException(nameof(symbolName));

        IntPtr handle;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            handle = Win32.GetProcAddress(library, symbolName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            handle = Linux.dlsym(library, symbolName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            handle = Mac.dlsym(library, symbolName);
        else
            throw new PlatformNotSupportedException($"Current platform is unknown, unable to load symbol '{symbolName}' from library {library}.");

        return handle;
#endif
    }

#if !NET5_0_OR_GREATER
#pragma warning disable IDE1006 // Naming Styles
    private static class Mac
    {
        private const string SystemLibrary = "/usr/lib/libSystem.dylib";

        private const int RTLD_LAZY = 1;
        private const int RTLD_NOW = 2;

        public static IntPtr dlopen(string path, bool lazy = true) =>
            dlopen(path, lazy ? RTLD_LAZY : RTLD_NOW);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlopen(string path, int mode);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(SystemLibrary)]
        public static extern void dlclose(IntPtr handle);
    }

    private static class Linux
    {
        private const string SystemLibrary = "libdl.so";

        private const int RTLD_LAZY = 1;
        private const int RTLD_NOW = 2;

        public static IntPtr dlopen(string path, bool lazy = true) =>
            dlopen(path, lazy ? RTLD_LAZY : RTLD_NOW);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlopen(string path, int mode);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(SystemLibrary)]
        public static extern void dlclose(IntPtr handle);
    }

    private static class Win32
    {
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);
    }
#pragma warning restore IDE1006 // Naming Styles
#endif
}
