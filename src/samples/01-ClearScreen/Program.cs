// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangle
{
    public static unsafe class Program
    {
        public static void Main()
        {
            var result = vkInitialize();
            var version = vkEnumerateInstanceVersion();
            var extensions = vkEnumerateInstanceExtensionProperties();
            var layers = vkEnumerateLayerProperties();

            foreach (var ext in extensions)
            {
                var name = ext.GetExtensionName();
            }

            foreach(var layer in layers)
            {
                var name = layer.GetName();
                var desc = layer.GetDescription();
            }

            var instanceCreateInfo = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo
            };

            VkInstance instance;
            vkCreateInstance(&instanceCreateInfo, null, &instance).CheckResult();
            vkLoadInstance(instance);
#if TODO
            var properties = Vulkan.EnumerateInstanceExtensionProperties();
            var layers = Vulkan.EnumerateLayerProperties();
            var v = Vulkan.EnumerateInstanceVersion();
            var applicationInfo = new ApplicationInfo
            {
                ApplicationName = "SharpVulkan",
                ApiVersion = new VkVersion(1, 0, 0),
                EngineName = "SharpVulkan"
            };

            //var instanceInfo = new InstanceCreateInfo
            //{
            //    ApplicationInfo = applicationInfo
            //};

            var instance = new VkInstance(new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo
            });


            //Vulkan.CreateInstance();  
#endif
        }
    }
}
