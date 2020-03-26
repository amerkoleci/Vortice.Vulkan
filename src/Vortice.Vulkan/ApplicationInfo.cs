// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying application info.
    /// </summary>
    public partial struct ApplicationInfo
    {
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string ApplicationName;

        /// <summary>
        /// The developer-supplied version number of the application.
        /// </summary>
        public VkVersion ApplicationVersion;

        /// <summary>
        /// The name of the engine (if any) used to create the application.
        /// </summary>
        public string EngineName;

        /// <summary>
        /// The developer-supplied version number of the engine used to create the application.
        /// </summary>
        public VkVersion EngineVersion;

        /// <summary>
        /// The highest version of Vulkan that the application is designed to use.
        /// </summary>
        public VkVersion ApiVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfo"/> structure.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="applicationVersion">The developer-supplied version number of the application. </param>
        /// <param name="engineName">The name of the engine (if any) used to create the application.</param>
        /// <param name="engineVersion">The developer-supplied version number of the engine used to create the application.</param>
        /// <param name="apiVersion">The highest version of Vulkan that the application is designed to use.</param>
        public ApplicationInfo(
            string applicationName = null,
            VkVersion applicationVersion = default,
            string engineName = null,
            VkVersion engineVersion = default,
            VkVersion apiVersion = default)
        {
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            EngineName = engineName;
            EngineVersion = engineVersion;
            ApiVersion = apiVersion;
        }

        //internal unsafe void __MarshalFrom(VkApplicationInfo* val)
        //{
        //    val->sType = VkStructureType.ApplicationInfo;
        //    val->pNext = IntPtr.Zero;
        //    val->pApplicationName = Interop.StringToPointer(ApplicationName);
        //    val->applicationVersion = ApplicationVersion;
        //    val->pEngineName = Interop.StringToPointer(EngineName);
        //    val->engineVersion = EngineVersion;
        //    val->apiVersion = ApiVersion;
        //}

        //internal unsafe void __MarshalFree(VkApplicationInfo* val)
        //{
        //    Interop.Free(val->pApplicationName);
        //    Interop.Free(val->pEngineName);
        //}
    }

    /// <summary>
    /// Structure specifying parameters for creation of <see cref="VkInstance"/>.
    /// </summary>
    public struct InstanceCreateInfo
    {
        public ApplicationInfo? ApplicationInfo;
        public string[] EnabledLayerNames;
        public string[] EnabledExtensionNames;

        //internal void __MarshalTo(out VkInstanceCreateInfo native)
        //{
        //    native = new VkInstanceCreateInfo
        //    {
        //        sType = VkStructureType.InstanceCreateInfo,
        //        pNext = IntPtr.Zero,
        //        flags = 0
        //    };
        //    //if (ApplicationInfo.HasValue)
        //    //{
        //    //    var appInfoNative = (ApplicationInfo.Native*)Interop.Alloc<ApplicationInfo.Native>();
        //    //    ApplicationInfo.Value.__MarshalFrom(appInfoNative);
        //    //    native.ApplicationInfo = appInfoNative;
        //    //}
        //    //else
        //    //{
        //    //    native.ApplicationInfo = null;
        //    //}
        //    //native.enabledLayerCount = EnabledLayerNames?.Length ?? 0;
        //    //native.enabledLayerNames = Interop.StringToPointers(EnabledLayerNames);
        //    //native.enabledExtensionCount = EnabledExtensionNames?.Length ?? 0;
        //    //native.enabledExtensionNames = Interop.StringToPointers(EnabledExtensionNames);
        //}
    }
}
