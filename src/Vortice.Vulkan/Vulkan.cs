// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    public static unsafe partial class Vulkan
    {
        private static IntPtr s_vulkanModule = IntPtr.Zero;
        private static readonly ILibraryLoader _loader = InitializeLoader();
        private static VkInstance s_loadedInstance = VkInstance.Null;
        private static VkDevice s_loadedDevice = VkDevice.Null;

        private delegate IntPtr VoidFunction(IntPtr context, string name);



        static Vulkan()
        {
            //vkCreateInstance_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkCreateInstance));
            //vkEnumerateInstanceExtensionProperties_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkEnumerateInstanceExtensionProperties));
            //vkEnumerateInstanceLayerProperties_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkEnumerateInstanceLayerProperties));
            //vkEnumerateInstanceVersion_ptr = vkGetInstanceProcAddr(IntPtr.Zero, nameof(vkEnumerateInstanceVersion));
        }

        public static VkResult Initialize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                s_vulkanModule = _loader.LoadNativeLibrary("vulkan-1.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.dylib");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.1.dylib");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libMoltenVK.dylib");
            }
            else
            {
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so.1");
            }

            if (s_vulkanModule == IntPtr.Zero)
                return VkResult.ErrorInitializationFailed;

#if CALLI_SUPPORT
            vkGetInstanceProcAddr_ptr = GetProcAddress(nameof(vkGetInstanceProcAddr));
#else
            vkGetInstanceProcAddr_ptr = GetProcAddress<vkGetInstanceProcAddrDelegate>(nameof(vkGetInstanceProcAddr));
#endif

            GenLoadLoader(IntPtr.Zero, vkGetInstanceProcAddr);

            return VkResult.Success;
        }

        private static ILibraryLoader InitializeLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsLoader();
            }
            else
            {
                return new UnixLoader();
            }
        }

        private static IntPtr GetProcAddress(string procName)
        {
            return _loader.GetSymbol(s_vulkanModule, procName);
        }

        public static TDelegate GetProcAddress<TDelegate>(string procName) where TDelegate : class
        {
            var handle = _loader.GetSymbol(s_vulkanModule, procName);
            return handle == IntPtr.Zero
                ? null
                : Marshal.GetDelegateForFunctionPointer<TDelegate>(handle);
        }

        private static void GenLoadLoader(IntPtr context, VoidFunction load)
        {
#if CALLI_SUPPORT
            vkCreateInstance_ptr = load(context, nameof(vkCreateInstance));
#else
            //vkCreateInstance_ptr = Marshal.GetDelegateForFunctionPointer<PFN_vkCreateInstance>(load(context, nameof(vkCreateInstance)));
#endif
        }

        #region Functions
        // TODO: We have WIP Calli patch, disable for now.
#if CALLI_SUPPORT
        private static IntPtr vkGetInstanceProcAddr_ptr;
        private static IntPtr vkCreateInstance_ptr;
#else
        private delegate IntPtr vkGetInstanceProcAddrDelegate(VkInstance instance, byte* name);
        //private delegate VkResult PFN_vkCreateInstance(VkInstanceCreateInfo* createInfo, VkAllocationCallbacks* allocator, out VkInstance pInstance);

        private static vkGetInstanceProcAddrDelegate vkGetInstanceProcAddr_ptr;
        //private static PFN_vkCreateInstance vkCreateInstance_ptr;
#endif
        #endregion



        public unsafe static IntPtr vkGetInstanceProcAddr(IntPtr instance, string name)
        {
            int byteCount = Interop.GetMaxByteCount(name);
            var stringPtr = stackalloc byte[byteCount];
            Interop.StringToPointer(name, stringPtr, byteCount);
            return vkGetInstanceProcAddr(instance, stringPtr);
        }

        [Calli]
        private unsafe static IntPtr vkGetInstanceProcAddr(IntPtr instance, byte* name)
        {
#if CALLI_SUPPORT
            throw new NotImplementedException();
#else
            return vkGetInstanceProcAddr_ptr(instance, name);
#endif
        }

        /*public static VkResult vkCreateInstance(VkInstanceCreateInfo createInfo, out VkInstance instance)
        {
            return vkCreateInstance_ptr(&createInfo, null, out instance);
        }*/

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

       
        private static readonly IntPtr vkEnumerateInstanceExtensionProperties_ptr;
        private static readonly IntPtr vkEnumerateInstanceLayerProperties_ptr;
        private static readonly IntPtr vkEnumerateInstanceVersion_ptr;

        

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
            IntPtr LoadNativeLibrary(string name);

            IntPtr GetSymbol(IntPtr module, string name);
        }

        private class WindowsLoader : ILibraryLoader
        {
            public IntPtr LoadNativeLibrary(string name) => LoadLibrary(name);

            public IntPtr GetSymbol(IntPtr module, string name) => GetProcAddress(module, name);

            [DllImport("kernel32")]
            private static extern IntPtr LoadLibrary(string fileName);

            [DllImport("kernel32")]
            private static extern IntPtr GetProcAddress(IntPtr module, string procName);

            [DllImport("kernel32")]
            private static extern int FreeLibrary(IntPtr module);
        }

        private class UnixLoader : ILibraryLoader
        {
            public IntPtr LoadNativeLibrary(string name) => dlopen(name, RTLD_NOW | RTLD_LOCAL);

            public IntPtr GetSymbol(IntPtr module, string name) => dlsym(module, name);

            [DllImport("libdl", EntryPoint = "dlopen")]
            private static extern IntPtr dlopen(string fileName, int flags);

            [DllImport("libdl", EntryPoint = "dlsym")]
            private static extern IntPtr dlsym(IntPtr handle, string name);

            [DllImport("libdl", EntryPoint = "dlclose")]
            private static extern int dlclose(IntPtr handle);

            [DllImport("libdl", EntryPoint = "dlerror")]
            private static extern string dlerror();

            private const int RTLD_LOCAL = 0x0000;
            private const int RTLD_NOW = 0x0002;
        }
    }
}
