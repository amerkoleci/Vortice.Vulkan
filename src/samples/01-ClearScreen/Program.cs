// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Vortice.Mathematics;

namespace DrawTriangle
{
    public static unsafe class Program
    {
#if DEBUG
        private static bool EnableValidationLayers = true;
        private static readonly string[] s_RequestedValidationLayers = new[] { "VK_LAYER_KHRONOS_validation" };
#else
        private static bool EnableValidationLayers = false;
        private static readonly string[] s_RequestedValidationLayers = new string[0];
#endif

        public static void Main()
        {
            using (var testApp = new TestApp())
            {
                testApp.Run();
            }
        }

        private static void FindValidationLayers(List<string> appendTo)
        {
            var availableLayers = vkEnumerateInstanceLayerProperties();

            for (int i = 0; i < s_RequestedValidationLayers.Length; i++)
            {
                var hasLayer = false;
                for (int j = 0; j < availableLayers.Length; j++)
                    if (s_RequestedValidationLayers[i] == availableLayers[j].GetName())
                    {
                        hasLayer = true;
                        break;
                    }

                if (hasLayer)
                {
                    appendTo.Add(s_RequestedValidationLayers[i]);
                }
                else
                {
                    // TODO: Warn
                }
            }
        }


        class TestApp : Application
        {
            private static readonly VkString s_EngineName = "Vortice";
            private VkInstance instance;
            private vkDebugUtilsMessengerCallbackEXT _debugMessengerCallbackFunc;
            private VkDebugUtilsMessengerEXT debugMessenger = VkDebugUtilsMessengerEXT.Null;
            private VkSurfaceKHR surface;
            private VkPhysicalDevice physicalDevice;
            private VkDevice device;
            private VkQueue graphicsQueue;
            private VkQueue presentQueue;
            private VkSwapchainKHR swapchain;
            private VkImage[] swapChainImages;

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

                var instanceLayers = new List<string>();
                if (EnableValidationLayers)
                {
                    FindValidationLayers(instanceLayers);
                }

                if (instanceLayers.Count > 0)
                {
                    instanceExtensions.Add(EXTDebugUtilsExtensionName);
                }

                using var vkInstanceExtensions = new VkStringArray(instanceExtensions);
                var instanceCreateInfo = new VkInstanceCreateInfo
                {
                    sType = VkStructureType.InstanceCreateInfo,
                    pApplicationInfo = &appInfo,
                    enabledExtensionCount = vkInstanceExtensions.Length,
                    ppEnabledExtensionNames = vkInstanceExtensions
                };

                using var vkLayerNames = new VkStringArray(instanceLayers);
                if (instanceLayers.Count > 0)
                {
                    instanceCreateInfo.enabledLayerCount = vkLayerNames.Length;
                    instanceCreateInfo.ppEnabledLayerNames = vkLayerNames;
                }

                result = vkCreateInstance(&instanceCreateInfo, null, out instance);
                vkLoadInstance(instance);

                if (instanceLayers.Count > 0)
                {
                    _debugMessengerCallbackFunc = DebugMessengerCallback;
                    var debugCreateInfo = new VkDebugUtilsMessengerCreateInfoEXT
                    {
                        sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT,
                        messageSeverity = /*VkDebugUtilsMessageSeverityFlagsEXT.VerboseEXT | */VkDebugUtilsMessageSeverityFlagsEXT.ErrorEXT | VkDebugUtilsMessageSeverityFlagsEXT.WarningEXT,
                        messageType = VkDebugUtilsMessageTypeFlagsEXT.GeneralEXT | VkDebugUtilsMessageTypeFlagsEXT.ValidationEXT | VkDebugUtilsMessageTypeFlagsEXT.PerformanceEXT,
                        pfnUserCallback = Marshal.GetFunctionPointerForDelegate(_debugMessengerCallbackFunc)
                    };

                    vkCreateDebugUtilsMessengerEXT(instance, &debugCreateInfo, null, out debugMessenger).CheckResult();
                }

                var surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
                {
                    sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                    hinstance = HInstance,
                    hwnd = MainWindow.Handle
                };

                result = vkCreateWin32SurfaceKHR(instance, &surfaceCreateInfo, null, out surface);

                var physicalDevices = vkEnumeratePhysicalDevices(instance);
                foreach (var physicalDevice in physicalDevices)
                {
                    vkGetPhysicalDeviceProperties(physicalDevice, out var properties);
                    var deviceName = properties.GetDeviceName();
                }

                physicalDevice = physicalDevices[0];

                var queueFamilies = FindQueueFamilies(physicalDevice, surface);

                var priority = 1.0f;
                var queueCreateInfo = new VkDeviceQueueCreateInfo
                {
                    sType = VkStructureType.DeviceQueueCreateInfo,
                    queueFamilyIndex = queueFamilies.graphicsFamily,
                    queueCount = 1,
                    pQueuePriorities = &priority
                };

                List<string> deviceExtensions = new List<string>
                {
                    KHRSwapchainExtensionName
                };
                var deviceCreateInfo = new VkDeviceCreateInfo
                {
                    sType = VkStructureType.DeviceCreateInfo,
                    pQueueCreateInfos = &queueCreateInfo,
                    queueCreateInfoCount = 1,
                    pEnabledFeatures = null,
                };

                using var deviceExtensionNames = new VkStringArray(deviceExtensions);
                deviceCreateInfo.enabledExtensionCount = deviceExtensionNames.Length;
                deviceCreateInfo.ppEnabledExtensionNames = deviceExtensionNames;

                result = vkCreateDevice(physicalDevice, &deviceCreateInfo, null, out device);
                if (result != VkResult.Success)
                    throw new Exception($"Failed to create Vulkan Logical Device, {result}");

                vkGetDeviceQueue(device, queueFamilies.graphicsFamily, 0, out graphicsQueue);
                vkGetDeviceQueue(device, queueFamilies.presentFamily, 0, out presentQueue);

                CreateSwapChain();
            }

            protected override void OnTick()
            {
                vkAcquireNextImageKHR(device, swapchain, ulong.MaxValue, VkSemaphore.Null, VkFence.Null, out var imageIndex);

                var lswapchain = swapchain;
                var presentInfo = new VkPresentInfoKHR
                {
                    sType = VkStructureType.PresentInfoKHR,
                    pImageIndices = &imageIndex,
                    swapchainCount = 1,
                    pSwapchains = &lswapchain
                };
                vkQueuePresentKHR(presentQueue, &presentInfo);
            }

            private void CreateSwapChain()
            {
                SwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(physicalDevice, surface);

                VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
                VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.PresentModes);
                var extent = ChooseSwapExtent(swapChainSupport.Capabilities);

                uint imageCount = swapChainSupport.Capabilities.minImageCount + 1;
                if (swapChainSupport.Capabilities.maxImageCount > 0 &&
                    imageCount > swapChainSupport.Capabilities.maxImageCount)
                {
                    imageCount = swapChainSupport.Capabilities.maxImageCount;
                }

                var createInfo = new VkSwapchainCreateInfoKHR
                {
                    sType = VkStructureType.SwapchainCreateInfoKHR,
                    surface = surface,
                    minImageCount = imageCount,
                    imageFormat = surfaceFormat.format,
                    imageColorSpace = surfaceFormat.colorSpace,
                    imageExtent = extent,
                    imageArrayLayers = 1,
                    imageUsage = VkImageUsageFlags.ColorAttachment,
                    imageSharingMode = VkSharingMode.Exclusive,
                    preTransform = swapChainSupport.Capabilities.currentTransform,
                    compositeAlpha = VkCompositeAlphaFlagsKHR.OpaqueKHR,
                    presentMode = presentMode,
                    clipped = true,
                    oldSwapchain = VkSwapchainKHR.Null
                };

                vkCreateSwapchainKHR(device, &createInfo, null, out swapchain).CheckResult();
                swapChainImages = vkGetSwapchainImagesKHR(device, swapchain).ToArray();
            }

            private Size ChooseSwapExtent(in VkSurfaceCapabilitiesKHR capabilities)
            {
                if (capabilities.currentExtent.Width > 0)
                {
                    return capabilities.currentExtent;
                }
                else
                {
                    Size actualExtent = new Size(MainWindow.Width, MainWindow.Height);

                    actualExtent.Width = Math.Max(capabilities.minImageExtent.Width, Math.Min(capabilities.maxImageExtent.Width, actualExtent.Width));
                    actualExtent.Height = Math.Max(capabilities.minImageExtent.Height, Math.Min(capabilities.maxImageExtent.Height, actualExtent.Height));

                    return actualExtent;
                }
            }
        }

        static (uint graphicsFamily, uint presentFamily) FindQueueFamilies(
            VkPhysicalDevice device, VkSurfaceKHR surface)
        {
            var queueFamilies = vkGetPhysicalDeviceQueueFamilyProperties(device);

            uint graphicsFamily = QueueFamilyIgnored;
            uint presentFamily = QueueFamilyIgnored;
            uint i = 0;
            foreach (var queueFamily in queueFamilies)
            {
                if ((queueFamily.queueFlags & VkQueueFlags.Graphics) != VkQueueFlags.None)
                {
                    graphicsFamily = i;
                }

                vkGetPhysicalDeviceSurfaceSupportKHR(device, i, surface, out var presentSupport);

                if (presentSupport)
                {
                    presentFamily = i;
                }

                if (graphicsFamily != QueueFamilyIgnored
                    && presentFamily != QueueFamilyIgnored)
                {
                    break;
                }

                i++;
            }

            return (graphicsFamily, presentFamily);
        }

        ref struct SwapChainSupportDetails
        {
            public VkSurfaceCapabilitiesKHR Capabilities;
            public ReadOnlySpan<VkSurfaceFormatKHR> Formats;
            public ReadOnlySpan<VkPresentModeKHR> PresentModes;
        };

        private static SwapChainSupportDetails QuerySwapChainSupport(VkPhysicalDevice device, VkSurfaceKHR surface)
        {
            SwapChainSupportDetails details = new SwapChainSupportDetails();
            vkGetPhysicalDeviceSurfaceCapabilitiesKHR(device, surface, out details.Capabilities).CheckResult();

            details.Formats = vkGetPhysicalDeviceSurfaceFormatsKHR(device, surface);
            details.PresentModes = vkGetPhysicalDeviceSurfacePresentModesKHR(device, surface);
            return details;
        }

        private static VkSurfaceFormatKHR ChooseSwapSurfaceFormat(ReadOnlySpan<VkSurfaceFormatKHR> availableFormats)
        {
            foreach (var availableFormat in availableFormats)
            {
                if (availableFormat.format == VkFormat.B8G8R8A8SRgb &&
                    availableFormat.colorSpace == VkColorSpaceKHR.SrgbNonlinearKHR)
                {
                    return availableFormat;
                }
            }

            return availableFormats[0];
        }

        private static VkPresentModeKHR ChooseSwapPresentMode(ReadOnlySpan<VkPresentModeKHR> availablePresentModes)
        {
            foreach (var availablePresentMode in availablePresentModes)
            {
                if (availablePresentMode == VkPresentModeKHR.MailboxKHR)
                {
                    return availablePresentMode;
                }
            }

            return VkPresentModeKHR.FifoKHR;
        }

        private static VkBool32 DebugMessengerCallback(
                    VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
                    VkDebugUtilsMessageTypeFlagsEXT messageTypes,
                    VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
                    IntPtr userData)
        {
            var message = Interop.GetString(pCallbackData->pMessage);
            if (messageTypes == VkDebugUtilsMessageTypeFlagsEXT.ValidationEXT)
            {
                Debug.WriteLine($"[Vulkan]: Validation: {messageSeverity} - {message}");
            }
            else
            {
                Debug.WriteLine($"[Vulkan]: {messageSeverity} - {message}");
            }

            return VkBool32.False;
        }
    }
}
