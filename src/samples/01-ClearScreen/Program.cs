// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangle
{
    public static class Program
    {
        public static void Main()
        {
            var result = Vulkan.Initialize();

            var instanceCreateInfo = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo
            };

            result = vkCreateInstance(instanceCreateInfo, out var instance);

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
