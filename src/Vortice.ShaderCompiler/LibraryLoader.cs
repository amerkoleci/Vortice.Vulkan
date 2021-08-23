// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.ShaderCompiler
{
    internal static class LibraryLoader
    {
        static LibraryLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Extension = ".dll";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Extension = ".dylib";
            else
                Extension = ".so";
        }

        public static string Extension { get; }

        public static IntPtr LoadLocalLibrary(string libraryName)
        {
#if !NET5_0_OR_GREATER
            var libraryPath = GetLibraryPath(libraryName);

            static string GetLibraryPath(string libraryName)
            {
                var arch = PlatformConfiguration.Is64Bit
                    ? PlatformConfiguration.IsArm ? "arm64" : "x64"
                    : PlatformConfiguration.IsArm ? "arm" : "x86";

                var libWithExt = libraryName;
                if (!libraryName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                    libWithExt += Extension;

                // 1. try alongside managed assembly
                var path = typeof(LibraryLoader).Assembly.Location;
                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.GetDirectoryName(path);
                    if (CheckLibraryPath(path, arch, libWithExt, out var localLib))
                        return localLib;
                }

                // 2. try current directory
                if (CheckLibraryPath(Directory.GetCurrentDirectory(), arch, libWithExt, out var lib))
                    return lib;

                // 3. try app domain
                try
                {
                    if (AppDomain.CurrentDomain is AppDomain domain)
                    {
                        // 3.1 RelativeSearchPath
                        if (CheckLibraryPath(domain.RelativeSearchPath, arch, libWithExt, out lib))
                            return lib;

                        // 3.2 BaseDirectory
                        if (CheckLibraryPath(domain.BaseDirectory, arch, libWithExt, out lib))
                            return lib;
                    }
                }
                catch
                {
                    // no-op as there may not be any domain or path
                }

                // 4. use PATH or default loading mechanism
                return libWithExt;
            }

            static bool CheckLibraryPath(string root, string arch, string libWithExt, out string? foundPath)
            {
                if (!string.IsNullOrEmpty(root))
                {
                    // a. in specific platform sub dir
                    if (!string.IsNullOrEmpty(PlatformConfiguration.LinuxFlavor))
                    {
                        var muslLib = Path.Combine(root, PlatformConfiguration.LinuxFlavor + "-" + arch, libWithExt);
                        if (File.Exists(muslLib))
                        {
                            foundPath = muslLib;
                            return true;
                        }
                    }

                    // b. in generic platform sub dir
                    var searchLib = Path.Combine(root, arch, libWithExt);
                    if (File.Exists(searchLib))
                    {
                        foundPath = searchLib;
                        return true;
                    }

                    // c. in root
                    searchLib = Path.Combine(root, libWithExt);
                    if (File.Exists(searchLib))
                    {
                        foundPath = searchLib;
                        return true;
                    }
                }

                // d. nothing
                foundPath = null;
                return false;
            }

            IntPtr handle = LoadPlatformLibrary(libraryPath);
#else
            IntPtr handle = NativeLibrary.Load(
                libraryName,
                Assembly.GetExecutingAssembly(),
                DllImportSearchPath.SafeDirectories);
#endif

            if (handle == IntPtr.Zero)
                throw new DllNotFoundException($"Unable to load library '{libraryName}'.");

            return handle;
        }

        public static T LoadFunction<T>(IntPtr library, string name)
        {
#if !NET5_0_OR_GREATER
            IntPtr symbol = GetSymbol(library, name);
#else
            IntPtr symbol = NativeLibrary.GetExport(library, name);
#endif

            if (symbol == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Unable to load symbol '{name}'.");

            return Marshal.GetDelegateForFunctionPointer<T>(symbol);
        }

#if !NET5_0_OR_GREATER
        private static IntPtr LoadPlatformLibrary(string libraryName)
        {
            if (string.IsNullOrEmpty(libraryName))
                throw new ArgumentNullException(nameof(libraryName));

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
        }

        private static IntPtr GetSymbol(IntPtr library, string symbolName)
        {
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
        }


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
            private const string SystemLibrary = "Kernel32.dll";

            [DllImport(SystemLibrary, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport(SystemLibrary, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            [DllImport(SystemLibrary, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern void FreeLibrary(IntPtr hModule);
        }
#pragma warning restore IDE1006 // Naming Styles
#endif
    }
}
