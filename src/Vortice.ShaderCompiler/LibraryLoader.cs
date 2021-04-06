// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

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
#if NETSTANDARD2_0
            string? libraryPath = GetLibraryPath(libraryName);

            IntPtr handle = LoadPlatformLibrary(libraryPath);
            if (handle == IntPtr.Zero)
                throw new DllNotFoundException($"Unable to load library '{libraryName}'.");

            return handle;

            static string GetLibraryPath(string libraryName)
            {
                var arch = RuntimeInformation.ProcessArchitecture;
                bool isArm = arch == Architecture.Arm || arch == Architecture.Arm64;
                string archStr = IntPtr.Size == 8
                    ? isArm ? "arm64" : "x64"
                    : isArm ? "arm" : "x86";

                string libWithExt = libraryName;
                if (!libraryName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                    libWithExt += Extension;

                // 1. try alongside managed assembly
                string? path = typeof(Native).Assembly.Location;
                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.GetDirectoryName(path);
                    // 1.1 in platform sub dir
                    var lib = Path.Combine(path, archStr, libWithExt);
                    if (File.Exists(lib))
                        return lib;
                    // 1.2 in root
                    lib = Path.Combine(path, libWithExt);
                    if (File.Exists(lib))
                        return lib;
                }

                // 2. try current directory
                path = Directory.GetCurrentDirectory();
                if (!string.IsNullOrEmpty(path))
                {
                    // 2.1 in platform sub dir
                    var lib = Path.Combine(path, archStr, libWithExt);
                    if (File.Exists(lib))
                        return lib;
                    // 2.2 in root
                    lib = Path.Combine(lib, libWithExt);
                    if (File.Exists(lib))
                        return lib;
                }

                // 3. try app domain
                try
                {
                    if (AppDomain.CurrentDomain is AppDomain domain)
                    {
                        // 3.1 RelativeSearchPath
                        path = domain.RelativeSearchPath;
                        if (!string.IsNullOrEmpty(path))
                        {
                            // 3.1.1 in platform sub dir
                            string? lib = Path.Combine(path, archStr, libWithExt);
                            if (File.Exists(lib))
                                return lib;
                            // 3.1.2 in root
                            lib = Path.Combine(lib, libWithExt);
                            if (File.Exists(lib))
                                return lib;
                        }

                        // 3.2 BaseDirectory
                        path = domain.BaseDirectory;
                        if (!string.IsNullOrEmpty(path))
                        {
                            // 3.2.1 in platform sub dir
                            var lib = Path.Combine(path, archStr, libWithExt);
                            if (File.Exists(lib))
                                return lib;
                            // 3.2.2 in root
                            lib = Path.Combine(lib, libWithExt);
                            if (File.Exists(lib))
                                return lib;
                        }
                    }
                }
                catch
                {
                    // no-op as there may not be any domain or path
                }

                // 4. use PATH or default loading mechanism
                return libWithExt;
            }
#else
            var libWithExt = libraryName;
			if (!libraryName.EndsWith (Extension, StringComparison.OrdinalIgnoreCase))
				libWithExt += Extension;

            return NativeLibrary.Load(libWithExt);
#endif
        }

        public static T LoadFunction<T>(IntPtr library, string name)
        {
#if NETSTANDARD2_0
            IntPtr symbol = GetSymbol(library, name);
#else
            IntPtr symbol = NativeLibrary.GetExport(library, name);
#endif

            if (symbol == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Unable to load symbol '{name}'.");

            return Marshal.GetDelegateForFunctionPointer<T>(symbol);
        }

#if NETSTANDARD2_0
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
