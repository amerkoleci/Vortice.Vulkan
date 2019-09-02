// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying layer properties.
    /// </summary>
    public struct LayerProperties
    {
        /// <summary>
        /// A unicode string specifying the name of the layer. Use this name in the <see
        /// cref="InstanceCreateInfo.EnabledLayerNames"/> array to enable this layer for an instance.
        /// </summary>
        public string LayerName;

        /// <summary>
        /// The Vulkan version the layer was written to.
        /// </summary>
        public VkVersion SpecVersion;

        /// <summary>
        /// The version of this layer. It is an integer, increasing with backward compatible changes.
        /// </summary>
        public VkVersion ImplementationVersion;

        /// <summary>
        /// A unicode string providing additional details that can be used by the application to
        /// identify the layer.
        /// </summary>
        public string Description;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{LayerName} v{SpecVersion}";

        //internal unsafe static LayerProperties FromNative(ref VkLayerProperties native)
        //{
        //    fixed (byte* layerNamePtr = native.layerName)
        //    {
        //        fixed (byte* descriptionPtr = native.description)
        //        {
        //            return new LayerProperties
        //            {
        //                LayerName = Interop.StringFromPointer(layerNamePtr),
        //                SpecVersion = new VkVersion(native.specVersion),
        //                ImplementationVersion = new VkVersion(native.implementationVersion),
        //                Description = Interop.StringFromPointer(descriptionPtr)
        //            };
        //        }
        //    }
        //}
    }
}
