// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Vortice;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangle
{
    public static unsafe class Program
    {
        public static void Main()
        {
            using (var testApp = new TestApp())
            {
                testApp.Run();
            }
        }

        private static bool TryGetQueueFamilyIndex(
            VkPhysicalDevice device, VkQueueFlags flag, out uint index)
        {
            index = 0;

            ReadOnlySpan<VkQueueFamilyProperties > queueFamilies = vkGetPhysicalDeviceQueueFamilyProperties(device);
            for (int i = 0; i < queueFamilies.Length; i++)
            {
                if (queueFamilies[i].queueFlags.HasFlag(flag))
                {
                    index = (uint)i;
                    return true;
                }
            }

            return false;
        }

        class TestApp : Application
        {
            private static readonly VkString s_EngineName = "Vortice";
            private VkInstance instance;
            private VkDevice device;
            private VkQueue graphicsQueue;

            protected override void Initialize()
            {
                var result = vkInitialize();
                result.CheckResult();

                var version = vkEnumerateInstanceVersion();
                var queryExtensions = vkEnumerateInstanceExtensionProperties();
                var queryLayers = vkEnumerateInstanceLayerProperties();

                VkString name = "01-ClearScreen";
                var appInfo = new VkApplicationInfo
                {
                    sType = VkStructureType.ApplicationInfo,
                    pApplicationName = name,
                    applicationVersion = new VkVersion(1, 0, 0),
                    pEngineName = s_EngineName,
                    engineVersion = new VkVersion(1, 0, 0),
                    apiVersion = VkVersion.Version_1_0,
                };

                var instanceExtensions = new List<string>
                {
                    KHRSurfaceExtensionName,
                    KHRWin32SurfaceExtensionName
                };

                using var vkInstanceExtensions = new VkStringArray(instanceExtensions);
                var instanceCreateInfo = new VkInstanceCreateInfo
                {
                    sType = VkStructureType.InstanceCreateInfo,
                    pApplicationInfo = &appInfo,
                    enabledExtensionCount = vkInstanceExtensions.Length,
                    ppEnabledExtensionNames = vkInstanceExtensions
                };

                result = vkCreateInstance(&instanceCreateInfo, null, out instance);
                vkLoadInstance(instance);

                var surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
                {
                    sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                    hinstance = HInstance,
                    hwnd = MainWindow.Handle
                };

                result = vkCreateWin32SurfaceKHR(instance, &surfaceCreateInfo, null, out var surface);

                var physicalDevices = vkEnumeratePhysicalDevices(instance);
                foreach (var physicalDevice in physicalDevices)
                {
                    vkGetPhysicalDeviceProperties(physicalDevice, out var properties);
                    vkGetPhysicalDeviceFormatProperties(physicalDevice, VkFormat.B8G8R8A8UNorm, out var formatProperties);

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


                result = vkCreateDevice(physicalDevices[0], &deviceCreateInfo, null, out device);
                if (result != VkResult.Success)
                    throw new Exception($"Failed to create Vulkan Logical Device, {result}");

                vkGetDeviceQueue(device, 0, 0, out graphicsQueue);
            }
        }
    }
}
