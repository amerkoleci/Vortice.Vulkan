// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SDL;
using static SDL.SDL3;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public unsafe sealed class GraphicsDevice : IDisposable
{
    public readonly bool DebugUtils;
    public readonly VkInstance VkInstance;
    public readonly VkInstanceApi InstanceApi;

    private readonly VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;
    public readonly VkPhysicalDevice PhysicalDevice;
    public readonly VkDevice VkDevice;
    public readonly VkDeviceApi DeviceApi;
    public readonly VkQueue GraphicsQueue;
    public readonly VkQueue PresentQueue;
    public readonly Swapchain Swapchain;
    private PerFrame[] _perFrame;
    private uint _frameIndex;

    private readonly List<VkSemaphore> _recycledSemaphores = new();

    public GraphicsDevice(string applicationName, bool enableValidation, Window window)
    {
        HashSet<VkUtf8String> availableInstanceLayers = new(EnumerateInstanceLayers());
        HashSet<VkUtf8String> availableInstanceExtensions = new(GetInstanceExtensions());

        List<VkUtf8String> instanceExtensions = [];
        uint count;
        byte** strings = SDL_Vulkan_GetInstanceExtensions(&count);
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            ReadOnlySpan<byte> sdlExtSpan = Encoding.UTF8.GetBytes(PtrToStringUTF8(strings[i])!);
            instanceExtensions.Add(sdlExtSpan);
        }

        List<VkUtf8String> instanceLayers = [];

        if (enableValidation)
        {
            // Determine the optimal validation layers to enable that are necessary for useful debugging
            GetOptimalValidationLayers(availableInstanceLayers, instanceLayers);
        }

        // Check if VK_EXT_debug_utils is supported, which supersedes VK_EXT_Debug_Report
        foreach (VkUtf8String availableExtension in availableInstanceExtensions)
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

        VkUtf8ReadOnlyString pApplicationName = Encoding.UTF8.GetBytes(applicationName);
        VkUtf8ReadOnlyString pEngineName = "Vortice"u8;

        VkApplicationInfo appInfo = new()
        {
            pApplicationName = pApplicationName,
            applicationVersion = new VkVersion(1, 0, 0),
            pEngineName = pEngineName,
            engineVersion = new VkVersion(1, 0, 0),
            apiVersion = VkVersion.Version_1_3
        };

        using VkStringArray vkLayerNames = new(instanceLayers);
        using VkStringArray vkInstanceExtensions = new(instanceExtensions);

        VkInstanceCreateInfo instanceCreateInfo = new()
        {
            pApplicationInfo = &appInfo,
            enabledLayerCount = vkLayerNames.Length,
            ppEnabledLayerNames = vkLayerNames,
            enabledExtensionCount = vkInstanceExtensions.Length,
            ppEnabledExtensionNames = vkInstanceExtensions
        };

        VkDebugUtilsMessengerCreateInfoEXT debugUtilsCreateInfo = new();

        if (instanceLayers.Count > 0)
        {
            debugUtilsCreateInfo.messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error | VkDebugUtilsMessageSeverityFlagsEXT.Warning;
            debugUtilsCreateInfo.messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance;
            debugUtilsCreateInfo.pfnUserCallback = &DebugMessengerCallback;
            instanceCreateInfo.pNext = &debugUtilsCreateInfo;
        }

        VkResult result = vkCreateInstance(&instanceCreateInfo, out VkInstance);
        if (result != VK_SUCCESS)
        {
            throw new InvalidOperationException($"Failed to create vulkan instance: {result}");
        }

        InstanceApi = GetApi(VkInstance);

        if (instanceLayers.Count > 0)
        {
            InstanceApi.vkCreateDebugUtilsMessengerEXT(&debugUtilsCreateInfo, null, out _debugMessenger).CheckResult();
        }

        Log.Info($"Created VkInstance with version: {appInfo.apiVersion.Major}.{appInfo.apiVersion.Minor}.{appInfo.apiVersion.Patch}");
        if (instanceLayers.Count > 0)
        {
            foreach (var layer in instanceLayers)
            {
                Log.Info($"Instance layer '{layer}'");
            }
        }

        foreach (VkUtf8String extension in instanceExtensions)
        {
            Log.Info($"Instance extension '{extension}'");
        }

        VkSurfaceKHR surface = window.CreateSurface(VkInstance);

        // Find physical device, setup queue's and create device.
        uint physicalDevicesCount = 0;
        InstanceApi.vkEnumeratePhysicalDevices(&physicalDevicesCount, null).CheckResult();

        if (physicalDevicesCount == 0)
        {
            throw new Exception("Vulkan: Failed to find GPUs with Vulkan support");
        }

        Span<VkPhysicalDevice> physicalDevices = stackalloc VkPhysicalDevice[(int)physicalDevicesCount];
        InstanceApi.vkEnumeratePhysicalDevices(physicalDevices).CheckResult();

        for (int i = 0; i < physicalDevicesCount; i++)
        {
            VkPhysicalDevice physicalDevice = physicalDevices[i];

            if (IsDeviceSuitable(physicalDevice, surface) == false)
                continue;

            // Query for Vulkan 1.3 features
            VkPhysicalDeviceVulkan13Features queryVulkan13Features = new();
            VkPhysicalDeviceFeatures2 queryDeviceFeatures2 = new();
            queryDeviceFeatures2.pNext = &queryVulkan13Features;
            InstanceApi.vkGetPhysicalDeviceFeatures2(physicalDevice, &queryDeviceFeatures2);

            // Check if Physical device supports Vulkan 1.3 features
            if (!queryVulkan13Features.dynamicRendering)
            {
                Debug.WriteLine("Dynamic Rendering feature is missing");
                continue;
            }

            if (!queryVulkan13Features.synchronization2)
            {
                Debug.WriteLine("Synchronization2 feature is missing");
                continue;
            }

            InstanceApi.vkGetPhysicalDeviceProperties(physicalDevice, out VkPhysicalDeviceProperties checkProperties);
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

        InstanceApi.vkGetPhysicalDeviceProperties(PhysicalDevice, out VkPhysicalDeviceProperties properties);

        var queueFamilies = FindQueueFamilies(PhysicalDevice, surface);

        InstanceApi.vkEnumerateDeviceExtensionProperties(PhysicalDevice, out uint propertyCount).CheckResult();
        Span<VkExtensionProperties> availableDeviceExtensions = stackalloc VkExtensionProperties[(int)propertyCount];
        InstanceApi.vkEnumerateDeviceExtensionProperties(PhysicalDevice, availableDeviceExtensions).CheckResult();

        //var supportPresent = vkGetPhysicalDeviceWin32PresentationSupportKHR(PhysicalDevice, queueFamilies.graphicsFamily);

        HashSet<uint> uniqueQueueFamilies = [queueFamilies.graphicsFamily, queueFamilies.presentFamily];

        float priority = 1.0f;
        uint queueCount = 0;
        VkDeviceQueueCreateInfo* queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[2];

        foreach (uint queueFamily in uniqueQueueFamilies)
        {
            queueCreateInfos[queueCount++] = new VkDeviceQueueCreateInfo
            {
                queueFamilyIndex = queueFamily,
                queueCount = 1,
                pQueuePriorities = &priority
            };
        }

        List<VkUtf8String> enabledExtensions =
        [
            VK_KHR_SWAPCHAIN_EXTENSION_NAME
        ];

        VkPhysicalDeviceVulkan13Features deviceFeatures2 = new()
        {
            synchronization2 = true,
            dynamicRendering = true
        };

        VkPhysicalDeviceFeatures2 enableDeviceFeatures2 = new();
        enableDeviceFeatures2.pNext = &deviceFeatures2;

        using var deviceExtensionNames = new VkStringArray(enabledExtensions);

        VkDeviceCreateInfo deviceCreateInfo = new()
        {
            pNext = &enableDeviceFeatures2,
            queueCreateInfoCount = queueCount,
            pQueueCreateInfos = queueCreateInfos,
            enabledExtensionCount = deviceExtensionNames.Length,
            ppEnabledExtensionNames = deviceExtensionNames,
            pEnabledFeatures = null,
        };

        result = InstanceApi.vkCreateDevice(PhysicalDevice, &deviceCreateInfo, null, out VkDevice);
        if (result != VkResult.Success)
            throw new Exception($"Failed to create Vulkan Logical Device, {result}");

        DeviceApi = GetApi(VkInstance, VkDevice);

        DeviceApi.vkGetDeviceQueue(queueFamilies.graphicsFamily, 0, out GraphicsQueue);
        DeviceApi.vkGetDeviceQueue(queueFamilies.presentFamily, 0, out PresentQueue);

        // Create swap chain
        Swapchain = new Swapchain(this, surface, window);
        _perFrame = new PerFrame[Swapchain.ImageCount];
        for (int i = 0; i < _perFrame.Length; i++)
        {
            DeviceApi.vkCreateFence(VkFenceCreateFlags.Signaled, out _perFrame[i].QueueSubmitFence).CheckResult();

            VkCommandPoolCreateInfo poolCreateInfo = new()
            {
                flags = VkCommandPoolCreateFlags.Transient,
                queueFamilyIndex = queueFamilies.graphicsFamily,
            };
            DeviceApi.vkCreateCommandPool(&poolCreateInfo, null, out _perFrame[i].PrimaryCommandPool).CheckResult();

            DeviceApi.vkAllocateCommandBuffer(_perFrame[i].PrimaryCommandPool, out _perFrame[i].PrimaryCommandBuffer).CheckResult();
        }
    }

    public void Dispose()
    {
        // Don't release anything until the GPU is completely idle.
        WaitIdle();

        Swapchain.Dispose();

        for (var i = 0; i < _perFrame.Length; i++)
        {
            DeviceApi.vkDestroyFence(_perFrame[i].QueueSubmitFence);

            if (_perFrame[i].PrimaryCommandBuffer != IntPtr.Zero)
            {
                DeviceApi.vkFreeCommandBuffers(_perFrame[i].PrimaryCommandPool, _perFrame[i].PrimaryCommandBuffer);

                _perFrame[i].PrimaryCommandBuffer = IntPtr.Zero;
            }

            DeviceApi.vkDestroyCommandPool(_perFrame[i].PrimaryCommandPool);

            if (_perFrame[i].SwapchainAcquireSemaphore != VkSemaphore.Null)
            {
                DeviceApi.vkDestroySemaphore(_perFrame[i].SwapchainAcquireSemaphore);
                _perFrame[i].SwapchainAcquireSemaphore = VkSemaphore.Null;
            }

            if (_perFrame[i].SwapchainReleaseSemaphore != VkSemaphore.Null)
            {
                DeviceApi.vkDestroySemaphore(_perFrame[i].SwapchainReleaseSemaphore);
                _perFrame[i].SwapchainReleaseSemaphore = VkSemaphore.Null;
            }
        }

        foreach (VkSemaphore semaphore in _recycledSemaphores)
        {
            DeviceApi.vkDestroySemaphore(semaphore);
        }
        _recycledSemaphores.Clear();

        if (VkDevice.IsNotNull)
        {
            DeviceApi.vkDestroyDevice();
        }

        if (_debugMessenger != VkDebugUtilsMessengerEXT.Null)
        {
            InstanceApi.vkDestroyDebugUtilsMessengerEXT(_debugMessenger);
        }

        if (VkInstance != VkInstance.Null)
        {
            InstanceApi.vkDestroyInstance();
        }
    }

    public void WaitIdle()
    {
        DeviceApi.vkDeviceWaitIdle().CheckResult();
    }

    public void RenderFrame(
        Action<VkCommandBuffer, VkRenderingAttachmentInfo, VkExtent2D> draw,
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
            DeviceApi.vkDeviceWaitIdle();
            return;
        }

        // Begin command recording
        VkCommandBuffer commandBuffer = _perFrame[_frameIndex].PrimaryCommandBuffer;

        VkCommandBufferBeginInfo beginInfo = new()
        {
            flags = VkCommandBufferUsageFlags.OneTimeSubmit
        };
        DeviceApi.vkBeginCommandBuffer(commandBuffer, &beginInfo).CheckResult();

        // Before starting rendering, transition the swapchain image to COLOR_ATTACHMENT_OPTIMAL
        TransitionImageLayout(
            commandBuffer,
            Swapchain.Images[_frameIndex],
            VK_IMAGE_LAYOUT_UNDEFINED,
            VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
            0,                                                     // srcAccessMask (no need to wait for previous operations)
            VK_ACCESS_2_COLOR_ATTACHMENT_WRITE_BIT,                // dstAccessMask
            VK_PIPELINE_STAGE_2_TOP_OF_PIPE_BIT,                   // srcStage
            VK_PIPELINE_STAGE_2_COLOR_ATTACHMENT_OUTPUT_BIT        // dstStage
        );

        VkRenderingAttachmentInfo colorAttachment = new()
        {
            imageView = Swapchain.ImageViews[_frameIndex],
            imageLayout = VkImageLayout.ColorAttachmentOptimal,
            loadOp = VkAttachmentLoadOp.Clear,
            storeOp = VkAttachmentStoreOp.Store,
            clearValue = new VkClearValue(0.0f, 0.0f, 0.0f, 1.0f)
        };

        draw(commandBuffer, colorAttachment, Swapchain.Extent);

        // After rendering, transition the swapchain image to PRESENT_SRC
        TransitionImageLayout(
            commandBuffer,
            Swapchain.Images[_frameIndex],
            VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
            VK_IMAGE_LAYOUT_PRESENT_SRC_KHR,
            VK_ACCESS_2_COLOR_ATTACHMENT_WRITE_BIT,                 // srcAccessMask
            0,                                                      // dstAccessMask
            VK_PIPELINE_STAGE_2_COLOR_ATTACHMENT_OUTPUT_BIT,        // srcStage
            VK_PIPELINE_STAGE_2_BOTTOM_OF_PIPE_BIT                  // dstStage
        );

        // Complete the command buffer.
        DeviceApi.vkEndCommandBuffer(commandBuffer).CheckResult();

        if (_perFrame[_frameIndex].SwapchainReleaseSemaphore == VkSemaphore.Null)
        {
            DeviceApi.vkCreateSemaphore(out _perFrame[_frameIndex].SwapchainReleaseSemaphore).CheckResult();
        }

        VkPipelineStageFlags wait_stage = VkPipelineStageFlags.ColorAttachmentOutput;
        VkSemaphore waitSemaphore = _perFrame[_frameIndex].SwapchainAcquireSemaphore;
        VkSemaphore signalSemaphore = _perFrame[_frameIndex].SwapchainReleaseSemaphore;

        VkSubmitInfo submitInfo = new()
        {
            commandBufferCount = 1u,
            pCommandBuffers = &commandBuffer,
            waitSemaphoreCount = 1u,
            pWaitSemaphores = &waitSemaphore,
            pWaitDstStageMask = &wait_stage,
            signalSemaphoreCount = 1u,
            pSignalSemaphores = &signalSemaphore
        };

        // Submit command buffer to graphics queue
        DeviceApi.vkQueueSubmit(GraphicsQueue, submitInfo, _perFrame[_frameIndex].QueueSubmitFence);

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
        InstanceApi.vkGetPhysicalDeviceMemoryProperties(PhysicalDevice, out VkPhysicalDeviceMemoryProperties deviceMemoryProperties);

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
        DeviceApi.vkAllocateCommandBuffer(
            _perFrame[_frameIndex].PrimaryCommandPool,
            out VkCommandBuffer commandBuffer
            ).CheckResult();

        // If requested, also start the new command buffer
        if (begin)
        {
            VkCommandBufferBeginInfo beginInfo = new()
            {
                flags = VkCommandBufferUsageFlags.OneTimeSubmit
            };
            DeviceApi.vkBeginCommandBuffer(commandBuffer, &beginInfo).CheckResult();
        }

        return commandBuffer;
    }

    public void FlushCommandBuffer(VkCommandBuffer commandBuffer)
    {
        DeviceApi.vkEndCommandBuffer(commandBuffer).CheckResult();

        VkSubmitInfo submitInfo = new()
        {
            commandBufferCount = 1,
            pCommandBuffers = &commandBuffer
        };

        // Create fence to ensure that the command buffer has finished executing
        DeviceApi.vkCreateFence(out VkFence fence);

        // Submit to the queue
        DeviceApi.vkQueueSubmit(GraphicsQueue, 1, &submitInfo, fence).CheckResult();

        // Wait for the fence to signal that command buffer has finished executing
        DeviceApi.vkWaitForFences(1, &fence, true, ulong.MaxValue).CheckResult();

        DeviceApi.vkDestroyFence(fence);
    }

    private VkResult AcquireNextImage(out uint imageIndex)
    {
        VkSemaphore acquireSemaphore;
        if (_recycledSemaphores.Count == 0)
        {
            DeviceApi.vkCreateSemaphore(out acquireSemaphore).CheckResult();
        }
        else
        {
            acquireSemaphore = _recycledSemaphores[_recycledSemaphores.Count - 1];
            _recycledSemaphores.RemoveAt(_recycledSemaphores.Count - 1);
        }

        VkResult result = DeviceApi.vkAcquireNextImageKHR(Swapchain.Handle, ulong.MaxValue, acquireSemaphore, VkFence.Null, out imageIndex);

        if (result != VkResult.Success)
        {
            _recycledSemaphores.Add(acquireSemaphore);
            return result;
        }

        if (_perFrame[imageIndex].QueueSubmitFence != VkFence.Null)
        {
            DeviceApi.vkWaitForFences(_perFrame[imageIndex].QueueSubmitFence, true, ulong.MaxValue);
            DeviceApi.vkResetFences(_perFrame[imageIndex].QueueSubmitFence);
        }

        if (_perFrame[imageIndex].PrimaryCommandPool != VkCommandPool.Null)
        {
            DeviceApi.vkResetCommandPool(_perFrame[imageIndex].PrimaryCommandPool, VkCommandPoolResetFlags.None);
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
        return DeviceApi.vkQueuePresentKHR(PresentQueue, _perFrame[imageIndex].SwapchainReleaseSemaphore, Swapchain.Handle, imageIndex);
    }

    public static implicit operator VkDevice(GraphicsDevice device) => device.VkDevice;

    public VkResult CreateShaderModule(Span<byte> data, out VkShaderModule module)
    {
        return DeviceApi.vkCreateShaderModule(data, null, out module);
    }

    public void TransitionImageLayout(
        VkCommandBuffer commandBuffer,
        VkImage image,
        VkImageLayout oldLayout,
        VkImageLayout newLayout,
        VkAccessFlags2 srcAccessMask,
        VkAccessFlags2 dstAccessMask,
        VkPipelineStageFlags2 srcStage,
        VkPipelineStageFlags2 dstStage)
    {
        // Initialize the VkImageMemoryBarrier2 structure
        VkImageMemoryBarrier2 imageBarrier = new VkImageMemoryBarrier2
        {
            // Specify the pipeline stages and access masks for the barrier
            srcStageMask = srcStage,             // Source pipeline stage mask
            srcAccessMask = srcAccessMask,        // Source access mask
            dstStageMask = dstStage,             // Destination pipeline stage mask
            dstAccessMask = dstAccessMask,        // Destination access mask

            // Specify the old and new layouts of the image
            oldLayout = oldLayout,        // Current layout of the image
            newLayout = newLayout,        // Target layout of the image

            // We are not changing the ownership between queues
            srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
            dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,

            // Specify the image to be affected by this barrier
            image = image,

            // Define the subresource range (which parts of the image are affected)
            subresourceRange = new VkImageSubresourceRange(VK_IMAGE_ASPECT_COLOR_BIT, 0, 1, 0, 1)
        };

        // Initialize the VkDependencyInfo structure
        VkDependencyInfo dependencyInfo = new()
        {
            dependencyFlags = 0,                    // No special dependency flags
            imageMemoryBarrierCount = 1,                    // Number of image memory barriers
            pImageMemoryBarriers = &imageBarrier        // Pointer to the image memory barrier(s)
        };

        // Record the pipeline barrier into the command buffer
        DeviceApi.vkCmdPipelineBarrier2(commandBuffer, &dependencyInfo);
    }

    #region Private Methods
    private static bool CheckDeviceExtensionSupport(VkUtf8ReadOnlyString extensionName, ReadOnlySpan<VkExtensionProperties> availableDeviceExtensions)
    {
        foreach (VkExtensionProperties property in availableDeviceExtensions)
        {
            if (extensionName == property.extensionName)
                return true;
        }

        return false;
    }

    private static void GetOptimalValidationLayers(
        HashSet<VkUtf8String> availableLayers,
        List<VkUtf8String> instanceLayers)
    {
        // The preferred validation layer is "VK_LAYER_KHRONOS_validation"
        List<VkUtf8String> validationLayers =
        [
            "VK_LAYER_KHRONOS_validation"u8
        ];

        if (ValidateLayers(availableLayers, validationLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }

        // Otherwise we fallback to using the LunarG meta layer
        validationLayers =
        [
            "VK_LAYER_LUNARG_standard_validation"u8
        ];

        if (ValidateLayers(availableLayers, validationLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }

        // Otherwise we attempt to enable the individual layers that compose the LunarG meta layer since it doesn't exist
        validationLayers = new()
        {
            "VK_LAYER_GOOGLE_threading"u8,
            "VK_LAYER_LUNARG_parameter_validation"u8,
            "VK_LAYER_LUNARG_object_tracker"u8,
            "VK_LAYER_LUNARG_core_validation"u8,
            "VK_LAYER_GOOGLE_unique_objects"u8,
        };

        if (ValidateLayers(availableLayers, validationLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }

        // Otherwise as a last resort we fallback to attempting to enable the LunarG core layer
        validationLayers = new()
        {
            "VK_LAYER_LUNARG_core_validation"u8
        };

        if (ValidateLayers(availableLayers, validationLayers))
        {
            instanceLayers.AddRange(validationLayers);
            return;
        }
    }

    private static bool ValidateLayers(
        HashSet<VkUtf8String> availableLayers,
        List<VkUtf8String> required
        )
    {
        foreach (VkUtf8String layer in required)
        {
            bool found = false;
            foreach (VkUtf8String availableLayer in availableLayers)
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

    private bool IsDeviceSuitable(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface)
    {
        var checkQueueFamilies = FindQueueFamilies(physicalDevice, surface);
        if (checkQueueFamilies.graphicsFamily == VK_QUEUE_FAMILY_IGNORED)
            return false;

        if (checkQueueFamilies.presentFamily == VK_QUEUE_FAMILY_IGNORED)
            return false;

        SwapChainSupportDetails swapChainSupport = Utils.QuerySwapChainSupport(InstanceApi, physicalDevice, surface);
        return !swapChainSupport.Formats.IsEmpty && !swapChainSupport.PresentModes.IsEmpty;
    }

    [UnmanagedCallersOnly]
    private static uint DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* userData)
    {
        VkUtf8String message = new VkUtf8String(pCallbackData->pMessage)!;
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

    (uint graphicsFamily, uint presentFamily) FindQueueFamilies(VkPhysicalDevice device, VkSurfaceKHR surface)
    {
        InstanceApi.vkGetPhysicalDeviceQueueFamilyProperties(device, out uint queueFamilyCount);
        Span<VkQueueFamilyProperties> queueFamilies = stackalloc VkQueueFamilyProperties[(int)queueFamilyCount];
        InstanceApi.vkGetPhysicalDeviceQueueFamilyProperties(device, queueFamilies);

        uint graphicsFamily = VK_QUEUE_FAMILY_IGNORED;
        uint presentFamily = VK_QUEUE_FAMILY_IGNORED;
        uint i = 0;
        foreach (VkQueueFamilyProperties queueFamily in queueFamilies)
        {
            if ((queueFamily.queueFlags & VkQueueFlags.Graphics) != VkQueueFlags.None)
            {
                graphicsFamily = i;
            }

            InstanceApi.vkGetPhysicalDeviceSurfaceSupportKHR(device, i, surface, out VkBool32 presentSupport);
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

            uint propCount;
            result = vkEnumerateInstanceExtensionProperties(&propCount, null);
            if (result != VkResult.Success)
            {
                return false;
            }

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

    private unsafe static VkUtf8String[] EnumerateInstanceLayers()
    {
        if (!IsSupported())
        {
            return [];
        }

        uint count = 0;
        VkResult result = vkEnumerateInstanceLayerProperties(&count, null);
        if (result != VkResult.Success || count == 0)
        {
            return [];
        }

        VkLayerProperties[] props = new VkLayerProperties[(int)count];
        vkEnumerateInstanceLayerProperties(props).CheckResult();

        VkUtf8String[] resultExt = new VkUtf8String[count];
        for (int i = 0; i < count; i++)
        {
            fixed (byte* pLayerName = props[i].layerName)
            {
                resultExt[i] = new VkUtf8String(pLayerName);
            }
        }

        return resultExt;
    }

    private static VkUtf8String[] GetInstanceExtensions()
    {
        uint count = 0;
        VkResult result = vkEnumerateInstanceExtensionProperties(&count, null);
        if (result != VkResult.Success)
        {
            return [];
        }

        if (count == 0)
        {
            return [];
        }

        VkExtensionProperties[] props = new VkExtensionProperties[(int)count];
        vkEnumerateInstanceExtensionProperties(props);

        VkUtf8String[] extensions = new VkUtf8String[count];
        for (int i = 0; i < count; i++)
        {
            fixed (byte* pExtensionName = props[i].extensionName)
            {
                extensions[i] = new VkUtf8String(pExtensionName);
            }
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
