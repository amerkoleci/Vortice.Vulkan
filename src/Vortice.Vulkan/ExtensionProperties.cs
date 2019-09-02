// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a extension properties.
    /// </summary>
    public struct ExtensionProperties
    {
        /// <summary>
        /// The name of the extension.
        /// </summary>
        public string ExtensionName;

        /// <summary>
        /// The version of this extension. It is an integer, incremented with backward compatible changes.
        /// </summary>
        public int SpecVersion;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{ExtensionName} v{SpecVersion}";

        //internal static unsafe ExtensionProperties FromNative(ref VkExtensionProperties native)
        //{
        //    fixed (byte* extensionNamePtr = native.extensionName)
        //    {
        //        return new ExtensionProperties
        //        {
        //            ExtensionName = Interop.StringFromPointer(extensionNamePtr),
        //            SpecVersion = (int)native.specVersion
        //        };
        //    }
        //}
    }
}
