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
                var extName = ext.GetExtensionName();
            }

            foreach(var layer in layers)
            {
                var layerName = layer.GetName();
                var layerDesc = layer.GetDescription();
            }

            VkString name = "01-ClearScreen";
            VkString engineName = "Vortice";

            var appInfo = new VkApplicationInfo
            {
                sType = VkStructureType.ApplicationInfo,
                pApplicationName = name,
                applicationVersion = new VkVersion(1, 0, 0),
                pEngineName = engineName,
                engineVersion = new VkVersion(1, 0, 0),
                apiVersion = VkVersion.Version_1_0,
            };

            var instanceCreateInfo = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo,
                pApplicationInfo = &appInfo
            };

            VkInstance instance;
            vkCreateInstance(&instanceCreateInfo, null, &instance).CheckResult();
            vkLoadInstance(instance);

            var physicalDevices = vkEnumeratePhysicalDevices(instance);
            foreach(var physicalDevice in physicalDevices)
            {
                VkPhysicalDeviceProperties properties;
                vkGetPhysicalDeviceProperties(physicalDevice, &properties);

                var deviceName = properties.GetDeviceName();
            }

            TryGetQueueFamilyIndex(physicalDevices[0], VkQueueFlags.Graphics, out uint graphicsFamilyIndex);

            var priority = 1.0f;
            var queueCreateInfo = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.DeviceQueueCreateInfo,
                queueFamilyIndex = graphicsFamilyIndex,
                queueCount = 1,
                pQueuePriorities = &priority
            };

            var deviceCreateInfo = new VkDeviceCreateInfo
            {
                sType = VkStructureType.DeviceCreateInfo,
                pQueueCreateInfos = &queueCreateInfo,
                queueCreateInfoCount = 1,
                pEnabledFeatures = null,
            };

            VkDevice device;
            result = vkCreateDevice(physicalDevices[0], &deviceCreateInfo, null, &device);
            if (result != VkResult.Success)
                throw new Exception($"Failed to create Vulkan Logical Device, {result}");

            VkQueue graphicsQueue;
            vkGetDeviceQueue(device, 0, 0, &graphicsQueue);
        }

        private static bool TryGetQueueFamilyIndex(
            VkPhysicalDevice device, VkQueueFlags flag, out uint index)
        {
            index = 0;

            uint queueFamilyCount = 0;
            vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, null);

            VkQueueFamilyProperties* queueFamilies = stackalloc VkQueueFamilyProperties[(int)queueFamilyCount];
            vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, queueFamilies);

            for (int i = 0; i < queueFamilyCount; i++)
            {
                if (queueFamilies[i].queueFlags.HasFlag(flag))
                {
                    index = (uint)i;
                    return true;
                }
            }

            return false;
        }
    }
}
