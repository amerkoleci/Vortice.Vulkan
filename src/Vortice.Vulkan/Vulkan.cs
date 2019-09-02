// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    public static partial class Vulkan
    {
        private static readonly ILibraryLoader _loader;

        static Vulkan()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _loader = new WindowsLoader();
            }
            else
            {
                _loader = new UnixLoader();
            }

            //vkGetInstanceProcAddr_ptr = GetProcAddress(nameof(vkGetInstanceProcAddr));
            //vkCreateInstance_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkCreateInstance));
            //vkEnumerateInstanceExtensionProperties_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkEnumerateInstanceExtensionProperties));
            //vkEnumerateInstanceLayerProperties_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkEnumerateInstanceLayerProperties));
            //vkEnumerateInstanceVersion_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkEnumerateInstanceVersion));
        }

        private static IntPtr GetProcAddress(string procName)
        {
            return _loader.GetSymbol(procName);
        }

        public static TDelegate GetProcAddress<TDelegate>(string procName) where TDelegate : class
        {
            var handle = _loader.GetSymbol(procName);
            return handle == IntPtr.Zero
                ? null
                : Marshal.GetDelegateForFunctionPointer<TDelegate>(handle);
        }

#if TODO
        /// <summary>
        /// Returns global extension properties.
        /// </summary>
        /// <param name="layerName">Optional layer name.</param>
        /// <returns></returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static unsafe ExtensionProperties[] EnumerateInstanceExtensionProperties(string layerName = null)
        {
            var nativeStr = string.IsNullOrEmpty(layerName) ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(layerName);
            try
            {
                int count = 0;
                var result = vkEnumerateInstanceExtensionProperties((byte*)nativeStr, &count, null);
                VkException.ThrowForInvalidResult(result);

                var propertiesPtr = stackalloc ExtensionProperties[count];
                result = vkEnumerateInstanceExtensionProperties((byte*)nativeStr, &count, propertiesPtr);
                VkException.ThrowForInvalidResult(result);

                var properties = new ExtensionProperties[count];
                for (int i = 0; i < count; i++)
                {
                    properties[i] = ExtensionProperties.FromNative(ref propertiesPtr[i]);
                }

                return properties;
            }
            finally
            {
                Marshal.FreeHGlobal(nativeStr);
            }
        }

        /// <summary>
        /// Returns global layer properties.
        /// </summary>
        /// <returns>Properties of available layers.</returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static unsafe LayerProperties[] EnumerateLayerProperties()
        {
            int count = 0;
            var result = vkEnumerateInstanceLayerProperties(&count, null);
            VkException.ThrowForInvalidResult(result);

            var nativePropertiesPtr = stackalloc VkLayerProperties[count];
            result = vkEnumerateInstanceLayerProperties(&count, nativePropertiesPtr);
            VkException.ThrowForInvalidResult(result);

            var properties = new LayerProperties[count];
            for (int i = 0; i < count; i++)
            {
                properties[i] = LayerProperties.FromNative(ref nativePropertiesPtr[i]);
            }

            return properties;
        }

        /// <summary>
        /// Query instance-level version before instance creation.
        /// </summary>
        /// <returns>The version of Vulkan supported by instance-level functionality.</returns>
        public static unsafe VkVersion EnumerateInstanceVersion()
        {
            uint apiVersion;
            if (vkEnumerateInstanceVersion_ptr != IntPtr.Zero
                && vkEnumerateInstanceVersion(&apiVersion) == VkResult.Success)
            {
                return new VkVersion(apiVersion);
            }

            return VkVersion.Version_1_0;
        }

        public unsafe static VkInstance CreateInstance(VkInstanceCreateInfo createInfo)
        {
            return default;
            //createInfo.__MarshalTo(out var nativeCreateInfo);

            //IntPtr handle;
            //VkResult result = vkCreateInstance(&nativeCreateInfo, null, &handle);
            ////nativeCreateInfo.Free();
            //SharpVulkanException.ThrowForInvalidResult(result);

            //return new VkInstance(handle);
        }

        private static readonly IntPtr vkGetInstanceProcAddr_ptr;
        private static readonly IntPtr vkCreateInstance_ptr;
        private static readonly IntPtr vkEnumerateInstanceExtensionProperties_ptr;
        private static readonly IntPtr vkEnumerateInstanceLayerProperties_ptr;
        private static readonly IntPtr vkEnumerateInstanceVersion_ptr;

        private unsafe static IntPtr vkGetInstanceProcAddr(IntPtr instance, string name)
        {
            int byteCount = Interop.GetMaxByteCount(name);
            var stringPtr = stackalloc byte[byteCount];
            Interop.StringToPointer(name, stringPtr, byteCount);
            return vkGetInstanceProcAddr(instance, stringPtr);
        }

        [Calli]
        private unsafe static IntPtr vkGetInstanceProcAddr(IntPtr instance, byte* layerName)
        {
            throw new NotImplementedException();
        }

        [Calli]
        internal unsafe static VkResult vkCreateInstance(VkInstanceCreateInfo* createInfo, void* allocator, IntPtr* instance)
        {
            throw new NotImplementedException();
        }

        [Calli]
        private unsafe static VkResult vkEnumerateInstanceExtensionProperties(byte* layerName, int* propertyCount, VkExtensionProperties* properties)
        {
            throw new NotImplementedException();
        }


        [Calli]
        private unsafe static VkResult vkEnumerateInstanceLayerProperties(int* pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        [Calli]
        private unsafe static VkResult vkEnumerateInstanceVersion(uint* pApiVersion)
        {
            throw new NotImplementedException();
        } 
#endif

        //private delegate void vkDestroyInstanceDelegate(IntPtr instance, AllocationCallbacks.Native* allocator);
        //private static readonly vkDestroyInstanceDelegate vkDestroyInstance;

        internal interface ILibraryLoader
        {
            IntPtr GetSymbol(string name);
        }

        private class WindowsLoader : ILibraryLoader
        {
            private readonly IntPtr _module;

            public WindowsLoader()
            {
                _module = LoadLibrary("vulkan-1.dll");
                if (_module == IntPtr.Zero)
                    throw new PlatformNotSupportedException($"Vulkan is not supported this platform: {RuntimeInformation.OSDescription}");
            }

            public IntPtr GetSymbol(string name) => GetProcAddress(_module, name);

            [DllImport("kernel32")]
            private static extern IntPtr LoadLibrary(string fileName);

            [DllImport("kernel32")]
            private static extern IntPtr GetProcAddress(IntPtr module, string procName);

            [DllImport("kernel32")]
            private static extern int FreeLibrary(IntPtr module);
        }

        private class UnixLoader : ILibraryLoader
        {
            private readonly IntPtr _module;

            public UnixLoader()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _module = Open("libvulkan.dylib", RTLD_NOW | RTLD_LOCAL);
                    if (_module == IntPtr.Zero)
                        _module = Open("libvulkan.1.dylib", RTLD_NOW | RTLD_LOCAL);
                    if (_module == IntPtr.Zero)
                        _module = Open("libMoltenVK.dylib", RTLD_NOW | RTLD_LOCAL);
                }
                else
                {
                    _module = Open("libvulkan.so", RTLD_NOW | RTLD_LOCAL);
                    if (_module == IntPtr.Zero)
                        _module = Open("libvulkan.so.1", RTLD_NOW | RTLD_LOCAL);
                }

                if (_module == IntPtr.Zero)
                    throw new PlatformNotSupportedException($"Vulkan is not supported this platform: {RuntimeInformation.OSDescription}");
            }

            public IntPtr GetSymbol(string name) => GetSymbol(_module, name);

            [DllImport("libdl", EntryPoint = "dlopen")]
            private static extern IntPtr Open(string fileName, int flags);

            [DllImport("libdl", EntryPoint = "dlsym")]
            private static extern IntPtr GetSymbol(IntPtr handle, string name);

            [DllImport("libdl", EntryPoint = "dlclose")]
            private static extern int Close(IntPtr handle);

            [DllImport("libdl", EntryPoint = "dlerror")]
            public static extern string GetError();

            public const int RTLD_LOCAL = 0x0000;
            public const int RTLD_NOW = 0x0002;
        }
    }
}
