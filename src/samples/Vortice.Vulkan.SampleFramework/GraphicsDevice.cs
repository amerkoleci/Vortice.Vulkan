// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Vulkan.GLFW;

namespace Vortice.Vulkan;

public unsafe sealed class GraphicsDevice : IDisposable
{
    private static readonly VkString s_EngineName = new("Vortice");

    public readonly bool DebugUtils;
    public readonly VkInstance VkInstance;

#if !NET6_0_OR_GREATER
    private readonly PFN_vkDebugUtilsMessengerCallbackEXT DebugMessagerCallbackDelegate = DebugMessengerCallback;
#endif

    private readonly VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;
    public readonly VkPhysicalDevice PhysicalDevice;
    public readonly VkDevice VkDevice;
    public readonly VkQueue GraphicsQueue;
    public readonly VkQueue PresentQueue;
    public readonly Swapchain Swapchain;
    private PerFrame[] _perFrame;
    private uint _frameIndex;

    private readonly List<VkSemaphore> _recycledSemaphores = new();

    public GraphicsDevice(string applicationName, bool enableValidation, Window window)
    {
        HashSet<string> availableInstanceLayers = new(EnumerateInstanceLayers());
        HashSet<string> availableInstanceExtensions = new(GetInstanceExtensions());

        List<string> instanceExtensions = new();
        instanceExtensions.AddRange(glfwGetRequiredInstanceExtensions());

        List<string> instanceLayers = new();

        if (enableValidation)
        {
            // Determine the optimal validation layers to enable that are necessary for useful debugging
            GetOptimalValidationLayers(availableInstanceLayers, instanceLayers);
        }

        // Check if VK_EXT_debug_utils is supported, which supersedes VK_EXT_Debug_Report
        foreach (string availableExtension in availableInstanceExtensions)
        {
            if (availableExtension == VK_EXT_DEBUG_UTILS_EXTENSION_NAME)
            {
                DebugUtils = true;
                instanceExtensions.Add(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
            }
            else if (availableExtension == VK_EXT_SWAPCHAIN_COLOR_SPACE_EXTENSION_NAME)
            {
                instanceExtensions.Add(VK_EXT_SWAPCHAIN_COLOR_SPACE_EXTENSION_NAME);
            }
        }

        VkString name = applicationName;
        var appInfo = new VkApplicationInfo
        {
            pApplicationName = name,
            applicationVersion = new VkVersion(1, 0, 0),
            pEngineName = s_EngineName,
            engineVersion = new VkVersion(1, 0, 0),
            apiVersion = VkVersion.Version_1_2
        };

        using VkStringArray vkLayerNames = new(instanceLayers);
        using VkStringArray vkInstanceExtensions = new(instanceExtensions);

        var instanceCreateInfo = new VkInstanceCreateInfo
        {
            sType = VkStructureType.InstanceCreateInfo,
            pApplicationInfo = &appInfo,
            enabledLayerCount = vkLayerNames.Length,
            ppEnabledLayerNames = vkLayerNames,
            enabledExtensionCount = vkInstanceExtensions.Length,
            ppEnabledExtensionNames = vkInstanceExtensions
        };

        var debugUtilsCreateInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT
        };

        if (instanceLayers.Count > 0)
        {
            debugUtilsCreateInfo.messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error | VkDebugUtilsMessageSeverityFlagsEXT.Warning;
            debugUtilsCreateInfo.messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance;
#if NET6_0_OR_GREATER
            debugUtilsCreateInfo.pfnUserCallback = &DebugMessengerCallback;
#else
            debugUtilsCreateInfo.pfnUserCallback = Marshal.GetFunctionPointerForDelegate(DebugMessagerCallbackDelegate);
#endif

            instanceCreateInfo.pNext = &debugUtilsCreateInfo;
        }

        VkResult result = vkCreateInstance(&instanceCreateInfo, null, out VkInstance);
        if (result != VkResult.Success)
        {
            throw new InvalidOperationException($"Failed to create vulkan instance: {result}");
        }

        vkLoadInstanceOnly(VkInstance);

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

        VkSurfaceKHR surface;
        window.CreateSurface(VkInstance, &surface).CheckResult();

        // Find physical device, setup queue's and create device.
        int physicalDevicesCount = 0;
        vkEnumeratePhysicalDevices(VkInstance, &physicalDevicesCount, null).CheckResult();

        if (physicalDevicesCount == 0)
        {
            throw new Exception("Vulkan: Failed to find GPUs with Vulkan support");
        }

        VkPhysicalDevice* physicalDevices = stackalloc VkPhysicalDevice[physicalDevicesCount];
        vkEnumeratePhysicalDevices(VkInstance, &physicalDevicesCount, physicalDevices).CheckResult();

        for (int i = 0; i < physicalDevicesCount; i++)
        {
            VkPhysicalDevice physicalDevice = physicalDevices[i];

            if (IsDeviceSuitable(physicalDevice, surface) == false)
                continue;

            vkGetPhysicalDeviceProperties(physicalDevice, out VkPhysicalDeviceProperties checkProperties);
            bool discrete = checkProperties.deviceType == VkPhysicalDeviceType.DiscreteGpu;

            if (discrete || PhysicalDevice.IsNull)
            {
                PhysicalDevice = physicalDevice;
                if (discrete)
                {
                    // If this is discrete GPU, look no further (prioritize discrete GPU)
                    break;
                }
            }
        }

        vkGetPhysicalDeviceProperties(PhysicalDevice, out VkPhysicalDeviceProperties properties);

        var queueFamilies = FindQueueFamilies(PhysicalDevice, surface);
        var availableDeviceExtensions = vkEnumerateDeviceExtensionProperties(PhysicalDevice);

        //var supportPresent = vkGetPhysicalDeviceWin32PresentationSupportKHR(PhysicalDevice, queueFamilies.graphicsFamily);

        HashSet<uint> uniqueQueueFamilies = new();
        uniqueQueueFamilies.Add(queueFamilies.graphicsFamily);
        uniqueQueueFamilies.Add(queueFamilies.presentFamily);

        float priority = 1.0f;
        uint queueCount = 0;
        VkDeviceQueueCreateInfo* queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[2];

        foreach (uint queueFamily in uniqueQueueFamilies)
        {
            queueCreateInfos[queueCount++] = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.DeviceQueueCreateInfo,
                queueFamilyIndex = queueFamily,
                queueCount = 1,
                pQueuePriorities = &priority
            };
        }

        List<string> enabledExtensions = new()
        {
            VK_KHR_SWAPCHAIN_EXTENSION_NAME
        };

        const bool useNewFeatures = false;
        VkPhysicalDeviceFeatures2 deviceFeatures2 = new()
        {
            sType = VkStructureType.PhysicalDeviceFeatures2

        };
        if (useNewFeatures)
        {
            VkPhysicalDeviceVulkan11Features features_1_1 = new()
            {
                sType = VkStructureType.PhysicalDeviceVulkan11Features
            };

            VkPhysicalDeviceVulkan12Features features_1_2 = new()
            {
                sType = VkStructureType.PhysicalDeviceVulkan12Features
            };

            deviceFeatures2.pNext = &features_1_1;
            features_1_1.pNext = &features_1_2;

            void** features_chain = &features_1_2.pNext;

            VkPhysicalDevice8BitStorageFeatures storage_8bit_features = default;
            if (properties.apiVersion <= VkVersion.Version_1_2)
            {
                if (CheckDeviceExtensionSupport(VK_KHR_8BIT_STORAGE_EXTENSION_NAME, availableDeviceExtensions))
                {
                    enabledExtensions.Add(VK_KHR_8BIT_STORAGE_EXTENSION_NAME);
                    //storage_8bit_features.sType = VkStructureType.PhysicalDevice8bitStorageFeatures;
                    *features_chain = &storage_8bit_features;
                    features_chain = &storage_8bit_features.pNext;
                }
            }

            if (CheckDeviceExtensionSupport(VK_KHR_SPIRV_1_4_EXTENSION_NAME, availableDeviceExtensions))
            {
                // Required by VK_KHR_spirv_1_4
                enabledExtensions.Add(VK_KHR_SHADER_FLOAT_CONTROLS_EXTENSION_NAME);

                // Required for VK_KHR_ray_tracing_pipeline
                enabledExtensions.Add(VK_KHR_SPIRV_1_4_EXTENSION_NAME);
            }

            if (CheckDeviceExtensionSupport(VK_KHR_BUFFER_DEVICE_ADDRESS_EXTENSION_NAME, availableDeviceExtensions))
            {
                // Required by VK_KHR_acceleration_structure
                enabledExtensions.Add(VK_KHR_BUFFER_DEVICE_ADDRESS_EXTENSION_NAME);
            }

            if (CheckDeviceExtensionSupport(VK_EXT_DESCRIPTOR_INDEXING_EXTENSION_NAME, availableDeviceExtensions))
            {
                // Required by VK_KHR_acceleration_structure
                enabledExtensions.Add(VK_EXT_DESCRIPTOR_INDEXING_EXTENSION_NAME);
            }

            VkPhysicalDeviceAccelerationStructureFeaturesKHR acceleration_structure_features = default;
            if (CheckDeviceExtensionSupport(VK_KHR_ACCELERATION_STRUCTURE_EXTENSION_NAME, availableDeviceExtensions))
            {
                // Required by VK_KHR_acceleration_structure
                enabledExtensions.Add(VK_KHR_DEFERRED_HOST_OPERATIONS_EXTENSION_NAME);

                enabledExtensions.Add(VK_KHR_ACCELERATION_STRUCTURE_EXTENSION_NAME);
                acceleration_structure_features.sType = VkStructureType.PhysicalDeviceAccelerationStructureFeaturesKHR;
                *features_chain = &acceleration_structure_features;
                features_chain = &acceleration_structure_features.pNext;
            }

            vkGetPhysicalDeviceFeatures2(PhysicalDevice, &deviceFeatures2);
        }

        using var deviceExtensionNames = new VkStringArray(enabledExtensions);

        VkDeviceCreateInfo deviceCreateInfo = new()
        {
            sType = VkStructureType.DeviceCreateInfo,
            pNext = useNewFeatures ? &deviceFeatures2 : default,
            queueCreateInfoCount = queueCount,
            pQueueCreateInfos = queueCreateInfos,
            enabledExtensionCount = deviceExtensionNames.Length,
            ppEnabledExtensionNames = deviceExtensionNames,
            pEnabledFeatures = null,
        };

        result = vkCreateDevice(PhysicalDevice, &deviceCreateInfo, null, out VkDevice);
        if (result != VkResult.Success)
            throw new Exception($"Failed to create Vulkan Logical Device, {result}");

        vkLoadDevice(VkDevice);

        vkGetDeviceQueue(VkDevice, queueFamilies.graphicsFamily, 0, out GraphicsQueue);
        vkGetDeviceQueue(VkDevice, queueFamilies.presentFamily, 0, out PresentQueue);

        // Create swap chain
        Swapchain = new Swapchain(this, surface, window);
        _perFrame = new PerFrame[Swapchain.ImageCount];
        for (int i = 0; i < _perFrame.Length; i++)
        {
            vkCreateFence(VkDevice, VkFenceCreateFlags.Signaled, out _perFrame[i].QueueSubmitFence).CheckResult();

            VkCommandPoolCreateInfo poolCreateInfo = new()
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                flags = VkCommandPoolCreateFlags.Transient,
                queueFamilyIndex = queueFamilies.graphicsFamily,
            };
            vkCreateCommandPool(VkDevice, &poolCreateInfo, null, out _perFrame[i].PrimaryCommandPool).CheckResult();

            vkAllocateCommandBuffer(VkDevice, _perFrame[i].PrimaryCommandPool, out _perFrame[i].PrimaryCommandBuffer).CheckResult();
        }
    }

    public void Dispose()
    {
        // Don't release anything until the GPU is completely idle.
        WaitIdle();

        Swapchain.Dispose();

        for (var i = 0; i < _perFrame.Length; i++)
        {
            vkDestroyFence(VkDevice, _perFrame[i].QueueSubmitFence);

            if (_perFrame[i].PrimaryCommandBuffer != IntPtr.Zero)
            {
                vkFreeCommandBuffers(VkDevice, _perFrame[i].PrimaryCommandPool, _perFrame[i].PrimaryCommandBuffer);

                _perFrame[i].PrimaryCommandBuffer = IntPtr.Zero;
            }

            vkDestroyCommandPool(VkDevice, _perFrame[i].PrimaryCommandPool);

            if (_perFrame[i].SwapchainAcquireSemaphore != VkSemaphore.Null)
            {
                vkDestroySemaphore(VkDevice, _perFrame[i].SwapchainAcquireSemaphore);
                _perFrame[i].SwapchainAcquireSemaphore = VkSemaphore.Null;
            }

            if (_perFrame[i].SwapchainReleaseSemaphore != VkSemaphore.Null)
            {
                vkDestroySemaphore(VkDevice, _perFrame[i].SwapchainReleaseSemaphore);
                _perFrame[i].SwapchainReleaseSemaphore = VkSemaphore.Null;
            }
        }

        foreach (VkSemaphore semaphore in _recycledSemaphores)
        {
            vkDestroySemaphore(VkDevice, semaphore);
        }
        _recycledSemaphores.Clear();

        if (VkDevice.IsNotNull)
        {
            //vkDestroyDevice(VkDevice);
        }

        if (_debugMessenger != VkDebugUtilsMessengerEXT.Null)
        {
            vkDestroyDebugUtilsMessengerEXT(VkInstance, _debugMessenger);
        }

        if (VkInstance != VkInstance.Null)
        {
            vkDestroyInstance(VkInstance);
        }
    }

    public void WaitIdle()
    {
        vkDeviceWaitIdle(VkDevice).CheckResult();
    }

    public void RenderFrame(
        Action<VkCommandBuffer, VkFramebuffer, VkExtent2D> draw,
        [CallerMemberName] string? frameName = null)
    {
        VkResult result = AcquireNextImage(out _frameIndex);

        // Handle outdated error in acquire.
        if (result == VkResult.SuboptimalKHR || result == VkResult.ErrorOutOfDateKHR)
        {
            //Resize(context.swapchain_dimensions.width, context.swapchain_dimensions.height);
            result = AcquireNextImage(out _frameIndex);
        }

        if (result != VkResult.Success)
        {
            vkDeviceWaitIdle(VkDevice);
            return;
        }

        // Begin command recording
        VkCommandBuffer cmd = _perFrame[_frameIndex].PrimaryCommandBuffer;

        VkCommandBufferBeginInfo beginInfo = new()
        {
            sType = VkStructureType.CommandBufferBeginInfo,
            flags = VkCommandBufferUsageFlags.OneTimeSubmit
        };
        vkBeginCommandBuffer(cmd, &beginInfo).CheckResult();

        draw(cmd, Swapchain.Framebuffers[_frameIndex], Swapchain.Extent);

        // Complete the command buffer.
        vkEndCommandBuffer(cmd).CheckResult();

        if (_perFrame[_frameIndex].SwapchainReleaseSemaphore == VkSemaphore.Null)
        {
            vkCreateSemaphore(VkDevice, out _perFrame[_frameIndex].SwapchainReleaseSemaphore).CheckResult();
        }

        VkPipelineStageFlags wait_stage = VkPipelineStageFlags.ColorAttachmentOutput;
        VkSemaphore waitSemaphore = _perFrame[_frameIndex].SwapchainAcquireSemaphore;
        VkSemaphore signalSemaphore = _perFrame[_frameIndex].SwapchainReleaseSemaphore;

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
        vkQueueSubmit(GraphicsQueue, submitInfo, _perFrame[_frameIndex].QueueSubmitFence);

        result = PresentImage(_frameIndex);

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

    public uint GetMemoryTypeIndex(uint typeBits, VkMemoryPropertyFlags properties)
    {
        vkGetPhysicalDeviceMemoryProperties(PhysicalDevice, out VkPhysicalDeviceMemoryProperties deviceMemoryProperties);

        // Iterate over all memory types available for the device used in this example
        for (int i = 0; i < deviceMemoryProperties.memoryTypeCount; i++)
        {
            if ((typeBits & 1) == 1)
            {
                if ((deviceMemoryProperties.memoryTypes[i].propertyFlags & properties) == properties)
                {
                    return (uint)i;
                }
            }
            typeBits >>= 1;
        }

        throw new Exception("Could not find a suitable memory type!");
    }

    public VkCommandBuffer GetCommandBuffer(bool begin = true)
    {
        vkAllocateCommandBuffer(VkDevice,
            _perFrame[_frameIndex].PrimaryCommandPool,
            out VkCommandBuffer commandBuffer).CheckResult();

        // If requested, also start the new command buffer
        if (begin)
        {
            VkCommandBufferBeginInfo beginInfo = new()
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.OneTimeSubmit
            };
            vkBeginCommandBuffer(commandBuffer, &beginInfo).CheckResult();
        }

        return commandBuffer;
    }

    public void FlushCommandBuffer(VkCommandBuffer commandBuffer)
    {
        vkEndCommandBuffer(commandBuffer).CheckResult();

        VkSubmitInfo submitInfo = new();
        submitInfo.sType = VkStructureType.SubmitInfo;
        submitInfo.commandBufferCount = 1;
        submitInfo.pCommandBuffers = &commandBuffer;

        // Create fence to ensure that the command buffer has finished executing
        vkCreateFence(VkDevice, out VkFence fence);

        // Submit to the queue
        vkQueueSubmit(GraphicsQueue, 1, &submitInfo, fence).CheckResult();

        // Wait for the fence to signal that command buffer has finished executing
        vkWaitForFences(VkDevice, 1, &fence, true, ulong.MaxValue).CheckResult();

        vkDestroyFence(VkDevice, fence);
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
        VkSemaphore oldSemaphore = _perFrame[imageIndex].SwapchainAcquireSemaphore;

        if (oldSemaphore != VkSemaphore.Null)
        {
            _recycledSemaphores.Add(oldSemaphore);
        }

        _perFrame[imageIndex].SwapchainAcquireSemaphore = acquireSemaphore;

        return VkResult.Success;
    }

    private VkResult PresentImage(uint imageIndex)
    {
        return vkQueuePresentKHR(PresentQueue, _perFrame[imageIndex].SwapchainReleaseSemaphore, Swapchain.Handle, imageIndex);
    }

    public static implicit operator VkDevice(GraphicsDevice device) => device.VkDevice;

    public VkResult CreateShaderModule(byte[] data, out VkShaderModule module)
    {
        return vkCreateShaderModule(VkDevice, data, null, out module);
    }

#region Private Methods


    private static bool CheckDeviceExtensionSupport(string extensionName, ReadOnlySpan<VkExtensionProperties> availableDeviceExtensions)
    {
        foreach (VkExtensionProperties property in availableDeviceExtensions)
        {
            if (string.Equals(property.GetExtensionName(), extensionName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static void GetOptimalValidationLayers(HashSet<string> availableLayers, List<string> instanceLayers)
    {
        // The preferred validation layer is "VK_LAYER_KHRONOS_validation"
        List<string> validationLayers = new()
        {
            "VK_LAYER_KHRONOS_validation"
        };

        if (ValidateLayers(validationLayers, availableLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }

        // Otherwise we fallback to using the LunarG meta layer
        validationLayers = new()
        {
            "VK_LAYER_LUNARG_standard_validation"
        };

        if (ValidateLayers(validationLayers, availableLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }

        // Otherwise we attempt to enable the individual layers that compose the LunarG meta layer since it doesn't exist
        validationLayers = new()
        {
            "VK_LAYER_GOOGLE_threading",
            "VK_LAYER_LUNARG_parameter_validation",
            "VK_LAYER_LUNARG_object_tracker",
            "VK_LAYER_LUNARG_core_validation",
            "VK_LAYER_GOOGLE_unique_objects",
        };

        if (ValidateLayers(validationLayers, availableLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }

        // Otherwise as a last resort we fallback to attempting to enable the LunarG core layer
        validationLayers = new()
        {
            "VK_LAYER_LUNARG_core_validation"
        };

        if (ValidateLayers(validationLayers, availableLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }
    }

    private static bool ValidateLayers(List<string> required, HashSet<string> availableLayers)
    {
        foreach (string layer in required)
        {
            bool found = false;
            foreach (string availableLayer in availableLayers)
            {
                if (availableLayer == layer)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                //Log.Warn("Validation Layer '{}' not found", layer);
                return false;
            }
        }

        return true;
    }

    private static bool IsDeviceSuitable(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface)
    {
        var checkQueueFamilies = FindQueueFamilies(physicalDevice, surface);
        if (checkQueueFamilies.graphicsFamily == VK_QUEUE_FAMILY_IGNORED)
            return false;

        if (checkQueueFamilies.presentFamily == VK_QUEUE_FAMILY_IGNORED)
            return false;

        SwapChainSupportDetails swapChainSupport = Utils.QuerySwapChainSupport(physicalDevice, surface);
        return !swapChainSupport.Formats.IsEmpty && !swapChainSupport.PresentModes.IsEmpty;
    }


#if NET6_0_OR_GREATER
    [UnmanagedCallersOnly]
#endif
    private static uint DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* userData)
    {
        string message = Interop.GetString(pCallbackData->pMessage);
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

        return VK_FALSE;
    }

    static (uint graphicsFamily, uint presentFamily) FindQueueFamilies(
        VkPhysicalDevice device, VkSurfaceKHR surface)
    {
        ReadOnlySpan<VkQueueFamilyProperties> queueFamilies = vkGetPhysicalDeviceQueueFamilyProperties(device);

        uint graphicsFamily = VK_QUEUE_FAMILY_IGNORED;
        uint presentFamily = VK_QUEUE_FAMILY_IGNORED;
        uint i = 0;
        foreach (VkQueueFamilyProperties queueFamily in queueFamilies)
        {
            if ((queueFamily.queueFlags & VkQueueFlags.Graphics) != VkQueueFlags.None)
            {
                graphicsFamily = i;
            }

            vkGetPhysicalDeviceSurfaceSupportKHR(device, i, surface, out VkBool32 presentSupport);
            if (presentSupport)
            {
                presentFamily = i;
            }

            if (graphicsFamily != VK_QUEUE_FAMILY_IGNORED
                && presentFamily != VK_QUEUE_FAMILY_IGNORED)
            {
                break;
            }

            i++;
        }

        return (graphicsFamily, presentFamily);
    }
#endregion

    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    public static bool IsSupported() => s_isSupported.Value;

    private static bool CheckIsSupported()
    {
        try
        {
            VkResult result = vkInitialize();
            if (result != VkResult.Success)
                return false;

            // We require Vulkan 1.1 or higher
            VkVersion version = vkEnumerateInstanceVersion();
            if (version < VkVersion.Version_1_1)
                return false;

            // TODO: Enumerate physical devices and try to create instance.

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string[] EnumerateInstanceLayers()
    {
        if (!IsSupported())
        {
            return Array.Empty<string>();
        }

        int count = 0;
        VkResult result = vkEnumerateInstanceLayerProperties(&count, null);
        if (result != VkResult.Success)
        {
            return Array.Empty<string>();
        }

        if (count == 0)
        {
            return Array.Empty<string>();
        }

        VkLayerProperties* properties = stackalloc VkLayerProperties[count];
        vkEnumerateInstanceLayerProperties(&count, properties).CheckResult();

        string[] resultExt = new string[count];
        for (int i = 0; i < count; i++)
        {
            resultExt[i] = properties[i].GetLayerName();
        }

        return resultExt;
    }

    private static string[] GetInstanceExtensions()
    {
        int count = 0;
        VkResult result = vkEnumerateInstanceExtensionProperties((byte*)null, &count, null);
        if (result != VkResult.Success)
        {
            return Array.Empty<string>();
        }

        if (count == 0)
        {
            return Array.Empty<string>();
        }

        VkExtensionProperties* props = stackalloc VkExtensionProperties[count];
        vkEnumerateInstanceExtensionProperties((byte*)null, &count, props);

        string[] extensions = new string[count];
        for (int i = 0; i < count; i++)
        {
            extensions[i] = props[i].GetExtensionName();
        }

        return extensions;
    }

    private struct PerFrame
    {
        public VkFence QueueSubmitFence;
        public VkCommandPool PrimaryCommandPool;
        public VkCommandBuffer PrimaryCommandBuffer;
        public VkSemaphore SwapchainAcquireSemaphore;
        public VkSemaphore SwapchainReleaseSemaphore;
    }
}
