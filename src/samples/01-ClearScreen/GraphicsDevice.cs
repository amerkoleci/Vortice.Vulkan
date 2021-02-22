using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Win32.Kernel32;

namespace Vortice
{
    public unsafe sealed class GraphicsDevice : IDisposable
    {
        private static readonly VkString s_EngineName = "Vortice";
        private static readonly string[] s_RequestedValidationLayers = new[] { "VK_LAYER_KHRONOS_validation" };

        public readonly VkInstance VkInstance;
        private vkDebugUtilsMessengerCallbackEXT? _debugMessengerCallbackFunc;
        private readonly VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;
        internal readonly VkSurfaceKHR _surface;
        public readonly VkPhysicalDevice PhysicalDevice;
        public readonly VkDevice VkDevice;
        public readonly VkQueue GraphicsQueue;
        public readonly VkQueue PresentQueue;
        public readonly Swapchain Swapchain;
        private PerFrame[] _perFrame;

        private readonly List<VkSemaphore> _recycledSemaphores = new List<VkSemaphore>();


        public GraphicsDevice(string applicationName, bool enableValidation, Window? window)
        {
            VkString name = applicationName;
            var appInfo = new VkApplicationInfo
            {
                sType = VkStructureType.ApplicationInfo,
                pApplicationName = name,
                applicationVersion = new VkVersion(1, 0, 0),
                pEngineName = s_EngineName,
                engineVersion = new VkVersion(1, 0, 0),
                apiVersion = vkEnumerateInstanceVersion()
            };

            List<string> instanceExtensions = new List<string>
            {
                KHRSurfaceExtensionName
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                instanceExtensions.Add(KHRWin32SurfaceExtensionName);
            }

            List<string> instanceLayers = new List<string>();
            if (enableValidation)
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

            var debugUtilsCreateInfo = new VkDebugUtilsMessengerCreateInfoEXT
            {
                sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT
            };

            if (instanceLayers.Count > 0)
            {
                _debugMessengerCallbackFunc = DebugMessengerCallback;
                debugUtilsCreateInfo.messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error | VkDebugUtilsMessageSeverityFlagsEXT.Warning;
                debugUtilsCreateInfo.messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance;
                debugUtilsCreateInfo.pfnUserCallback = Marshal.GetFunctionPointerForDelegate(_debugMessengerCallbackFunc);

                instanceCreateInfo.pNext = &debugUtilsCreateInfo;
            }

            VkResult result = vkCreateInstance(&instanceCreateInfo, null, out VkInstance);
            if (result != VkResult.Success)
            {
                throw new InvalidOperationException($"Failed to create vulkan instance: {result}");
            }

            vkLoadInstance(VkInstance);

            if (instanceLayers.Count > 0)
            {
                vkCreateDebugUtilsMessengerEXT(VkInstance, &debugUtilsCreateInfo, null, out _debugMessenger).CheckResult();
            }

            Log.Info($"Created VkInstance with version: {appInfo.apiVersion.Major}.{appInfo.apiVersion.Minor}.{appInfo.apiVersion.Patch}");
            if (instanceLayers.Count > 0)
            {
                foreach (var layer in instanceLayers)
                {
                    Log.Info($"Instance layer '{layer}'");
                }
            }

            foreach (string extension in instanceExtensions)
            {
                Log.Info($"Instance extension '{extension}'");
            }

            _surface = CreateSurface(window);

            // Find physical device, setup queue's and create device.
            var physicalDevices = vkEnumeratePhysicalDevices(VkInstance);
            foreach (var physicalDevice in physicalDevices)
            {
                //vkGetPhysicalDeviceProperties(physicalDevice, out var properties);
                //var deviceName = properties.GetDeviceName();
            }

            PhysicalDevice = physicalDevices[0];
            vkGetPhysicalDeviceProperties(PhysicalDevice, out VkPhysicalDeviceProperties properties);

            var queueFamilies = FindQueueFamilies(PhysicalDevice, _surface);

            var availableDeviceExtensions = vkEnumerateDeviceExtensionProperties(PhysicalDevice);

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

            if (CheckDeviceExtensionSupport(KHRSpirv14ExtensionName, availableDeviceExtensions))
            {
                deviceExtensions.Add(KHRSpirv14ExtensionName);
            }

            if (CheckDeviceExtensionSupport(KHRFragmentShadingRateExtensionName, availableDeviceExtensions))
            {
                deviceExtensions.Add(KHRFragmentShadingRateExtensionName);
            }

            if (CheckDeviceExtensionSupport(NVMeshShaderExtensionName, availableDeviceExtensions))
            {
                deviceExtensions.Add(NVMeshShaderExtensionName);
            }

            VkPhysicalDeviceVulkan11Features features_1_1 = new VkPhysicalDeviceVulkan11Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan11Features
            };

            VkPhysicalDeviceVulkan12Features features_1_2 = new VkPhysicalDeviceVulkan12Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan12Features
            };

            VkPhysicalDeviceFeatures2 deviceFeatures2 = new VkPhysicalDeviceFeatures2
            {
                sType = VkStructureType.PhysicalDeviceFeatures2
            };

            deviceFeatures2.pNext = &features_1_1;
            features_1_1.pNext = &features_1_2;

            void** features_chain = &features_1_2.pNext;

            VkPhysicalDevice8BitStorageFeatures storage_8bit_features = default;
            if (properties.apiVersion <= VkVersion.Version_1_2)
            {
                if (CheckDeviceExtensionSupport(KHR8bitStorageExtensionName, availableDeviceExtensions))
                {
                    deviceExtensions.Add(KHR8bitStorageExtensionName);
                    storage_8bit_features.sType = VkStructureType.PhysicalDevice8bitStorageFeatures;
                    *features_chain = &storage_8bit_features;
                    features_chain = &storage_8bit_features.pNext;
                }
            }

            VkPhysicalDeviceAccelerationStructureFeaturesKHR acceleration_structure_features = default;
            if (CheckDeviceExtensionSupport(KHRAccelerationStructureExtensionName, availableDeviceExtensions))
            {
                deviceExtensions.Add(KHRAccelerationStructureExtensionName);
                acceleration_structure_features.sType = VkStructureType.PhysicalDeviceAccelerationStructureFeaturesKHR;
                *features_chain = &acceleration_structure_features;
                features_chain = &acceleration_structure_features.pNext;
            }

            vkGetPhysicalDeviceFeatures2(PhysicalDevice, out deviceFeatures2);

            using var deviceExtensionNames = new VkStringArray(deviceExtensions);

            var deviceCreateInfo = new VkDeviceCreateInfo
            {
                sType = VkStructureType.DeviceCreateInfo,
                pNext = &deviceFeatures2,
                queueCreateInfoCount = 1,
                pQueueCreateInfos = &queueCreateInfo,
                enabledExtensionCount = deviceExtensionNames.Length,
                ppEnabledExtensionNames = deviceExtensionNames,
                pEnabledFeatures = null,
            };

            result = vkCreateDevice(PhysicalDevice, &deviceCreateInfo, null, out VkDevice);
            if (result != VkResult.Success)
                throw new Exception($"Failed to create Vulkan Logical Device, {result}");

            vkGetDeviceQueue(VkDevice, queueFamilies.graphicsFamily, 0, out GraphicsQueue);
            vkGetDeviceQueue(VkDevice, queueFamilies.presentFamily, 0, out PresentQueue);

            // Create swap chain
            Swapchain = new Swapchain(this, window);
            _perFrame = new PerFrame[Swapchain.ImageCount];
            for (var i = 0; i < _perFrame.Length; i++)
            {
                VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo(VkFenceCreateFlags.Signaled);
                vkCreateFence(VkDevice, &fenceCreateInfo, null, out _perFrame[i].QueueSubmitFence).CheckResult();

                VkCommandPoolCreateInfo poolCreateInfo = new VkCommandPoolCreateInfo
                {
                    sType = VkStructureType.CommandPoolCreateInfo,
                    flags = VkCommandPoolCreateFlags.Transient,
                    queueFamilyIndex = queueFamilies.graphicsFamily,
                };
                vkCreateCommandPool(VkDevice, &poolCreateInfo, null, out _perFrame[i].PrimaryCommandPool).CheckResult();

                VkCommandBufferAllocateInfo commandBufferInfo = new VkCommandBufferAllocateInfo
                {
                    sType = VkStructureType.CommandBufferAllocateInfo,
                    commandPool = _perFrame[i].PrimaryCommandPool,
                    level = VkCommandBufferLevel.Primary,
                    commandBufferCount = 1
                };
                vkAllocateCommandBuffers(VkDevice, &commandBufferInfo, out _perFrame[i].PrimaryCommandBuffer).CheckResult();
            }
        }

        private static bool CheckDeviceExtensionSupport(string extensionName, ReadOnlySpan<VkExtensionProperties> availableDeviceExtensions)
        {
            foreach (VkExtensionProperties property in availableDeviceExtensions)
            {
                if (string.Equals(property.GetExtensionName(), extensionName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public void Dispose()
        {
            // Don't release anything until the GPU is completely idle.
            vkDeviceWaitIdle(VkDevice);

            Swapchain.Dispose();

            for (var i = 0; i < _perFrame.Length; i++)
            {
                vkDestroyFence(VkDevice, _perFrame[i].QueueSubmitFence, null);

                if (_perFrame[i].PrimaryCommandBuffer != IntPtr.Zero)
                {
                    vkFreeCommandBuffers(VkDevice, _perFrame[i].PrimaryCommandPool, _perFrame[i].PrimaryCommandBuffer);

                    _perFrame[i].PrimaryCommandBuffer = IntPtr.Zero;
                }

                vkDestroyCommandPool(VkDevice, _perFrame[i].PrimaryCommandPool, null);

                if (_perFrame[i].SwapchainAcquireSemaphore != VkSemaphore.Null)
                {
                    vkDestroySemaphore(VkDevice, _perFrame[i].SwapchainAcquireSemaphore, null);
                    _perFrame[i].SwapchainAcquireSemaphore = VkSemaphore.Null;
                }

                if (_perFrame[i].SwapchainReleaseSemaphore != VkSemaphore.Null)
                {
                    vkDestroySemaphore(VkDevice, _perFrame[i].SwapchainReleaseSemaphore, null);
                    _perFrame[i].SwapchainReleaseSemaphore = VkSemaphore.Null;
                }

            }

            foreach (VkSemaphore semaphore in _recycledSemaphores)
            {
                vkDestroySemaphore(VkDevice, semaphore, null);
            }
            _recycledSemaphores.Clear();

            if (VkDevice != VkDevice.Null)
            {
                vkDestroyDevice(VkDevice, null);
            }

            if (_surface != VkSurfaceKHR.Null)
            {
                vkDestroySurfaceKHR(VkInstance, _surface, null);
            }

            if (_debugMessenger != VkDebugUtilsMessengerEXT.Null)
            {
                vkDestroyDebugUtilsMessengerEXT(VkInstance, _debugMessenger, null);
            }

            if (VkInstance != VkInstance.Null)
            {
                vkDestroyInstance(VkInstance, null);
            }
        }

        public void RenderFrame(Action<VkCommandBuffer, VkFramebuffer, VkExtent2D> draw, [CallerMemberName] string? frameName = null)
        {
            VkResult result = AcquireNextImage(out uint swapchainIndex);

            // Handle outdated error in acquire.
            if (result == VkResult.SuboptimalKHR || result == VkResult.ErrorOutOfDateKHR)
            {
                //Resize(context.swapchain_dimensions.width, context.swapchain_dimensions.height);
                result = AcquireNextImage(out swapchainIndex);
            }

            if (result != VkResult.Success)
            {
                vkDeviceWaitIdle(VkDevice);
                return;
            }

            // Begin command recording
            VkCommandBuffer cmd = _perFrame[swapchainIndex].PrimaryCommandBuffer;

            VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.OneTimeSubmit
            };
            vkBeginCommandBuffer(cmd, &beginInfo).CheckResult();

            draw(cmd, Swapchain.Framebuffers[swapchainIndex], Swapchain.Extent);

            // Complete the command buffer.
            vkEndCommandBuffer(cmd).CheckResult();

            if (_perFrame[swapchainIndex].SwapchainReleaseSemaphore == VkSemaphore.Null)
            {
                vkCreateSemaphore(VkDevice, out _perFrame[swapchainIndex].SwapchainReleaseSemaphore).CheckResult();
            }

            VkPipelineStageFlags wait_stage = VkPipelineStageFlags.ColorAttachmentOutput;
            VkSemaphore waitSemaphore = _perFrame[swapchainIndex].SwapchainAcquireSemaphore;
            VkSemaphore signalSemaphore = _perFrame[swapchainIndex].SwapchainReleaseSemaphore;

            VkSubmitInfo submitInfo = new VkSubmitInfo
            {
                sType = VkStructureType.SubmitInfo,
                commandBufferCount = 1u,
                pCommandBuffers = &cmd,
                waitSemaphoreCount = 1u,
                pWaitSemaphores = &waitSemaphore,
                pWaitDstStageMask = &wait_stage,
                signalSemaphoreCount = 1u,
                pSignalSemaphores = &signalSemaphore
            };

            // Submit command buffer to graphics queue
            vkQueueSubmit(GraphicsQueue, submitInfo, _perFrame[swapchainIndex].QueueSubmitFence);

            result = PresentImage(swapchainIndex);

            // Handle Outdated error in present.
            if (result == VkResult.SuboptimalKHR || result == VkResult.ErrorOutOfDateKHR)
            {
                //Resize(context.swapchain_dimensions.width, context.swapchain_dimensions.height);
            }
            else if (result != VkResult.Success)
            {
                Log.Error("Failed to present swapchain image.");
            }
        }

        private VkResult AcquireNextImage(out uint imageIndex)
        {
            VkSemaphore acquireSemaphore;
            if (_recycledSemaphores.Count == 0)
            {
                vkCreateSemaphore(VkDevice, out acquireSemaphore).CheckResult();
            }
            else
            {
                acquireSemaphore = _recycledSemaphores[_recycledSemaphores.Count - 1];
                _recycledSemaphores.RemoveAt(_recycledSemaphores.Count - 1);
            }

            VkResult result = vkAcquireNextImageKHR(VkDevice, Swapchain.Handle, ulong.MaxValue, acquireSemaphore, VkFence.Null, out imageIndex);

            if (result != VkResult.Success)
            {
                _recycledSemaphores.Add(acquireSemaphore);
                return result;
            }

            if (_perFrame[imageIndex].QueueSubmitFence != VkFence.Null)
            {
                vkWaitForFences(VkDevice, _perFrame[imageIndex].QueueSubmitFence, true, ulong.MaxValue);
                vkResetFences(VkDevice, _perFrame[imageIndex].QueueSubmitFence);
            }

            if (_perFrame[imageIndex].PrimaryCommandPool != VkCommandPool.Null)
            {
                vkResetCommandPool(VkDevice, _perFrame[imageIndex].PrimaryCommandPool, VkCommandPoolResetFlags.None);
            }

            // Recycle the old semaphore back into the semaphore manager.
            VkSemaphore old_semaphore = _perFrame[imageIndex].SwapchainAcquireSemaphore;

            if (old_semaphore != VkSemaphore.Null)
            {
                _recycledSemaphores.Add(old_semaphore);
            }

            _perFrame[imageIndex].SwapchainAcquireSemaphore = acquireSemaphore;

            return VkResult.Success;
        }

        private VkResult PresentImage(uint imageIndex)
        {
            return vkQueuePresentKHR(PresentQueue, _perFrame[imageIndex].SwapchainReleaseSemaphore, Swapchain.Handle, imageIndex);
        }

        public static implicit operator VkDevice(GraphicsDevice device) => device.VkDevice;

        #region Private Methods
        private VkSurfaceKHR CreateSurface(Window? window)
        {
            var surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
            {
                sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                hinstance = GetModuleHandle(null),
                hwnd = window!.Handle
            };

            vkCreateWin32SurfaceKHR(VkInstance, &surfaceCreateInfo, null, out VkSurfaceKHR surface).CheckResult();
            return surface;
        }

        private static VkBool32 DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
            VkDebugUtilsMessageTypeFlagsEXT messageTypes,
            VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
            IntPtr userData)
        {
            string? message = Interop.GetString(pCallbackData->pMessage);
            if (messageTypes == VkDebugUtilsMessageTypeFlagsEXT.Validation)
            {
                if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error)
                {
                    Log.Error($"[Vulkan]: Validation: {messageSeverity} - {message}");
                }
                else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning)
                {
                    Log.Warn($"[Vulkan]: Validation: {messageSeverity} - {message}");
                }

                Debug.WriteLine($"[Vulkan]: Validation: {messageSeverity} - {message}");
            }
            else
            {
                if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error)
                {
                    Log.Error($"[Vulkan]: {messageSeverity} - {message}");
                }
                else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning)
                {
                    Log.Warn($"[Vulkan]: {messageSeverity} - {message}");
                }

                Debug.WriteLine($"[Vulkan]: {messageSeverity} - {message}");
            }

            return VkBool32.False;
        }

        private static void FindValidationLayers(List<string> appendTo)
        {
            ReadOnlySpan<VkLayerProperties> availableLayers = vkEnumerateInstanceLayerProperties();

            for (int i = 0; i < s_RequestedValidationLayers.Length; i++)
            {
                bool hasLayer = false;
                for (int j = 0; j < availableLayers.Length; j++)
                {
                    if (s_RequestedValidationLayers[i] == availableLayers[j].GetLayerName())
                    {
                        hasLayer = true;
                        break;
                    }
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

        static (uint graphicsFamily, uint presentFamily) FindQueueFamilies(
            VkPhysicalDevice device, VkSurfaceKHR surface)
        {
            ReadOnlySpan<VkQueueFamilyProperties> queueFamilies = vkGetPhysicalDeviceQueueFamilyProperties(device);

            uint graphicsFamily = QueueFamilyIgnored;
            uint presentFamily = QueueFamilyIgnored;
            uint i = 0;
            foreach (VkQueueFamilyProperties queueFamily in queueFamilies)
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
        #endregion

        private struct PerFrame
        {
            public VkFence QueueSubmitFence;
            public VkCommandPool PrimaryCommandPool;
            public VkCommandBuffer PrimaryCommandBuffer;
            public VkSemaphore SwapchainAcquireSemaphore;
            public VkSemaphore SwapchainReleaseSemaphore;
        }
    }
}
