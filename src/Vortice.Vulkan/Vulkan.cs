// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    public static unsafe partial class Vulkan
    {
        private delegate IntPtr LoadFunction(IntPtr context, string name);

        private static IntPtr s_vulkanModule = IntPtr.Zero;
        private static readonly ILibraryLoader _loader = InitializeLoader();
        private static VkInstance s_loadedInstance = VkInstance.Null;
        private static VkDevice s_loadedDevice = VkDevice.Null;

        public static VkResult vkInitialize()
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
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so.1");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so");
            }

            if (s_vulkanModule == IntPtr.Zero)
            {
                return VkResult.ErrorInitializationFailed;
            }

#if CALLI_SUPPORT
            vkGetInstanceProcAddr_ptr = GetProcAddress(nameof(vkGetInstanceProcAddr));
#else
            vkGetInstanceProcAddr_ptr = GetProcAddress<vkGetInstanceProcAddrDelegate>(nameof(vkGetInstanceProcAddr));
#endif

            GenLoadLoader(IntPtr.Zero, vkGetInstanceProcAddr);

            return VkResult.Success;
        }

        public static void vkLoadInstance(VkInstance instance)
        {
            s_loadedInstance = instance;
            GenLoadInstance(instance.Handle, vkGetInstanceProcAddr);
            GenLoadDevice(instance.Handle, vkGetInstanceProcAddr);

            // Manually load win32 entries.
            vkCreateWin32SurfaceKHR_ptr = LoadCallback<vkCreateWin32SurfaceKHRDelegate>(instance.Handle, vkGetInstanceProcAddr, nameof(vkCreateWin32SurfaceKHR));
            vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = LoadCallback<vkGetPhysicalDeviceWin32PresentationSupportKHRDelegate>(instance.Handle, vkGetInstanceProcAddr, nameof(vkGetPhysicalDeviceWin32PresentationSupportKHR));
        }

        private static void GenLoadLoader(IntPtr context, LoadFunction load)
        {
            vkCreateInstance_ptr = LoadCallbackThrow<vkCreateInstanceDelegate>(context, load, "vkCreateInstance");
            vkEnumerateInstanceExtensionProperties_ptr = LoadCallbackThrow<vkEnumerateInstanceExtensionPropertiesDelegate>(context, load, "vkEnumerateInstanceExtensionProperties");
            vkEnumerateInstanceLayerProperties_ptr = LoadCallbackThrow<vkEnumerateInstanceLayerPropertiesDelegate>(context, load, "vkEnumerateInstanceLayerProperties");
            vkEnumerateInstanceVersion_ptr = LoadCallback<vkEnumerateInstanceVersionDelegate>(context, load, "vkEnumerateInstanceVersion");
        }

#if !CALLI_SUPPORT
        private static T LoadCallbackThrow<T>(IntPtr context, LoadFunction load, string name)
        {
            var functionPtr = load(context, name);
            if (functionPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"No function was found with the name {name}.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }

        private static T LoadCallback<T>(IntPtr context, LoadFunction load, string name)
        {
            var functionPtr = load(context, name);
            if (functionPtr == IntPtr.Zero)
                return default;

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }
#endif

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

        private static TDelegate GetProcAddress<TDelegate>(string procName) where TDelegate : class
        {
            var handle = _loader.GetSymbol(s_vulkanModule, procName);
            return handle == IntPtr.Zero
                ? null
                : Marshal.GetDelegateForFunctionPointer<TDelegate>(handle);
        }

        public static IntPtr vkGetInstanceProcAddr(IntPtr instance, string name)
        {
            int byteCount = Interop.GetMaxByteCount(name);
            var stringPtr = stackalloc byte[byteCount];
            Interop.StringToPointer(name, stringPtr, byteCount);
            return vkGetInstanceProcAddr_ptr(instance, stringPtr);
        }

        /// <summary>
        /// Returns up to requested number of global extension properties
        /// </summary>
        /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
        /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/> </returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static unsafe ReadOnlySpan<VkExtensionProperties> vkEnumerateInstanceExtensionProperties(string layerName = null)
        {
            int dstLayerNameByteCount = Interop.GetMaxByteCount(layerName);
            var dstLayerNamePtr = stackalloc byte[dstLayerNameByteCount];
            Interop.StringToPointer(layerName, dstLayerNamePtr, dstLayerNameByteCount);

            uint count = 0;
            vkEnumerateInstanceExtensionProperties(dstLayerNamePtr, &count, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* ptr = properties)
            {
                vkEnumerateInstanceExtensionProperties(dstLayerNamePtr, &count, ptr).CheckResult();
            }

            return properties;
        }

        /// <summary>
        /// Returns properties of available physical device extensions
        /// </summary>
        /// <param name="physicalDevice">The <see cref="VkPhysicalDevice"/> that will be queried.</param>
        /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
        /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/>.</returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static ReadOnlySpan<VkExtensionProperties> vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string layerName = "")
        {
            int dstLayerNameByteCount = Interop.GetMaxByteCount(layerName);
            var dstLayerNamePtr = stackalloc byte[dstLayerNameByteCount];
            Interop.StringToPointer(layerName, dstLayerNamePtr, dstLayerNameByteCount);

            uint propertyCount = 0;
            vkEnumerateDeviceExtensionProperties(physicalDevice, dstLayerNamePtr, &propertyCount, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[propertyCount];
            fixed (VkExtensionProperties* propertiesPtr = properties)
            {
                vkEnumerateDeviceExtensionProperties(physicalDevice, dstLayerNamePtr, &propertyCount, propertiesPtr).CheckResult();
            }
            return properties;
        }

        /// <summary>
        /// Query instance-level version before instance creation.
        /// </summary>
        /// <returns>The version of Vulkan supported by instance-level functionality.</returns>
        public static unsafe VkVersion vkEnumerateInstanceVersion()
        {
            if (vkEnumerateInstanceVersion_ptr != null
                && vkEnumerateInstanceVersion_ptr(out var apiVersion) == VkResult.Success)
            {
                return new VkVersion(apiVersion);
            }

            return VkVersion.Version_1_0;
        }

        #region Nested
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
        #endregion
    }
}
