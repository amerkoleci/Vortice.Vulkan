// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

public static unsafe partial class Vulkan
{
    private static readonly ILibraryLoader _loader = GetPlatformLoader();
    private delegate delegate* unmanaged<void> LoadFunction(nint context, string name);

    private static nint s_vulkanModule = 0;
    private static VkInstance s_loadedInstance = VkInstance.Null;
    private static VkDevice s_loadedDevice = VkDevice.Null;

    /// <summary>
    /// The VK_LAYER_KHRONOS_validation extension name.
    /// </summary>
    public static ReadOnlySpan<byte> VK_LAYER_KHRONOS_VALIDATION_EXTENSION_NAME => "VK_LAYER_KHRONOS_validation"u8;

    public const uint VK_TRUE = 1;
    public const uint VK_FALSE = 0;

    public static VkResult vkInitialize(string? libraryName = default)
    {
        if (OperatingSystem.IsWindows())
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                s_vulkanModule = _loader.LoadNativeLibrary(libraryName);
            }
            else
            {
                s_vulkanModule = _loader.LoadNativeLibrary("vulkan-1.dll");
            }

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                s_vulkanModule = _loader.LoadNativeLibrary(libraryName);
            }
            else
            {
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.dylib");
                if (s_vulkanModule == 0)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.1.dylib");
                if (s_vulkanModule == 0)
                    s_vulkanModule = _loader.LoadNativeLibrary("libMoltenVK.dylib");
            }

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }
        else
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                s_vulkanModule = _loader.LoadNativeLibrary(libraryName);
            }
            else
            {
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so.1");
                if (s_vulkanModule == 0)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so");
            }

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }

        vkGetInstanceProcAddr_ptr = (delegate* unmanaged<VkInstance, byte*, IntPtr>)_loader.LoadFunctionPointer(s_vulkanModule, nameof(vkGetInstanceProcAddr));
        GenLoadLoader(0, vkGetInstanceProcAddr);

        return VkResult.Success;
    }

    public static void vkLoadInstance(VkInstance instance)
    {
        vkLoadInstanceOnly(instance);
        GenLoadDevice(instance.Handle, vkGetInstanceProcAddr);
    }

    public static void vkLoadInstanceOnly(VkInstance instance)
    {
        s_loadedInstance = instance;
        GenLoadInstance(instance.Handle, vkGetInstanceProcAddr);

        vkGetDeviceProcAddr_ptr = (delegate* unmanaged<VkDevice, byte*, IntPtr>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetDeviceProcAddr));

        // Manually loaded entries.
        LoadWin32(instance);
        LoadXcb(instance);
        LoadXlib(instance);
        LoadWayland(instance);
    }

    public static void vkLoadDevice(VkDevice device)
    {
        s_loadedDevice = device;
        GenLoadDevice(device.Handle, vkGetDeviceProcAddr);

        // Manually loaded entries.
        LoadWin32(device);
        //LoadXcb(device);
    }

    private static void GenLoadLoader(nint context, LoadFunction load)
    {
        vkCreateInstance_ptr = (delegate* unmanaged<VkInstanceCreateInfo*, VkAllocationCallbacks*, VkInstance*, VkResult>)LoadCallbackThrow(context, load, "vkCreateInstance");
        vkEnumerateInstanceExtensionProperties_ptr = (delegate* unmanaged<byte*, uint*, VkExtensionProperties*, VkResult>)LoadCallbackThrow(context, load, "vkEnumerateInstanceExtensionProperties");
        vkEnumerateInstanceLayerProperties_ptr = (delegate* unmanaged<uint*, VkLayerProperties*, VkResult>)LoadCallbackThrow(context, load, "vkEnumerateInstanceLayerProperties");
        vkEnumerateInstanceVersion_ptr = (delegate* unmanaged<uint*, VkResult>)load(context, "vkEnumerateInstanceVersion");
    }

    private static delegate* unmanaged<void> LoadCallbackThrow(nint context, LoadFunction load, string name)
    {
        delegate* unmanaged<void> functionPtr = load(context, name);
        if (functionPtr == null)
        {
            throw new InvalidOperationException($"No function was found with the name {name}.");
        }

        return functionPtr;
    }

    internal static delegate* unmanaged<VkInstance, byte*, IntPtr> vkGetInstanceProcAddr_ptr;
    internal static delegate* unmanaged<VkDevice, byte*, IntPtr> vkGetDeviceProcAddr_ptr;

    public static delegate* unmanaged<void> vkGetInstanceProcAddr(VkInstance instance, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return (delegate* unmanaged<void>)vkGetInstanceProcAddr_ptr(instance, pName);
        }
    }

    public static delegate* unmanaged<void> vkGetInstanceProcAddr(IntPtr instance, string name)
    {
        return vkGetInstanceProcAddr(instance, name.GetUtf8Span());
    }

    public static delegate* unmanaged<void> vkGetDeviceProcAddr(VkDevice device, byte* name)
    {
        return (delegate* unmanaged<void>)vkGetDeviceProcAddr_ptr(device, name);
    }

    public static delegate* unmanaged<void> vkGetDeviceProcAddr(VkDevice device, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return (delegate* unmanaged<void>)vkGetDeviceProcAddr_ptr(device, pName);
        }
    }

    public static delegate* unmanaged<void> vkGetDeviceProcAddr(IntPtr device, string name)
    {
        return vkGetDeviceProcAddr(device, name.GetUtf8Span());
    }

    public static VkResult vkEnumerateInstanceExtensionProperties(uint* propertyCount, VkExtensionProperties* properties)
    {
        return vkEnumerateInstanceExtensionProperties_ptr((byte*)null, propertyCount, properties);
    }

    [SkipLocalsInit]
    public static VkResult vkEnumerateInstanceExtensionProperties(out uint propertyCount)
    {
        Unsafe.SkipInit(out propertyCount);
        fixed (uint* propertyCountPtr = &propertyCount)
        {
            return vkEnumerateInstanceExtensionProperties_ptr((byte*)null, propertyCountPtr, default);
        }
    }

    public static VkResult vkEnumerateInstanceExtensionProperties(Span<VkExtensionProperties> properties)
    {
        uint propertiesCount = checked((uint)properties.Length);
        fixed (VkExtensionProperties* propertiesPtr = properties)
        {
            return vkEnumerateInstanceExtensionProperties_ptr((byte*)null, &propertiesCount, propertiesPtr);
        }
    }

    [SkipLocalsInit]
    public static VkResult vkEnumerateInstanceExtensionProperties(VkUtf8ReadOnlyString layerName, out uint propertyCount)
    {
        Unsafe.SkipInit(out propertyCount);
        fixed (uint* propertyCountPtr = &propertyCount)
        {
            return vkEnumerateInstanceExtensionProperties_ptr(layerName, propertyCountPtr, default);
        }
    }

    [SkipLocalsInit]
    public static VkResult vkEnumerateInstanceExtensionProperties(VkUtf8ReadOnlyString layerName, Span<VkExtensionProperties> properties)
    {
        uint propertiesCount = checked((uint)properties.Length);
        fixed (VkExtensionProperties* propertiesPtr = properties)
        {
            return vkEnumerateInstanceExtensionProperties_ptr(layerName, &propertiesCount, propertiesPtr);
        }
    }

    /// <summary>
    /// Returns properties of available physical device extensions
    /// </summary>
    /// <param name="physicalDevice">The <see cref="VkPhysicalDevice"/> that will be queried.</param>
    /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
    /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/>.</returns>
    /// <exception cref="VkException">Vulkan returns an error code.</exception>
    public static ReadOnlySpan<VkExtensionProperties> vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string layerName = "")
    {
        if (layerName is not null)
        {
            ReadOnlySpan<byte> layerNameSpan = layerName.GetUtf8Span();

            fixed (byte* playerName = layerNameSpan)
            {
                uint propertyCount = 0;
                vkEnumerateDeviceExtensionProperties(physicalDevice, playerName, &propertyCount, null).CheckResult();

                ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[propertyCount];
                fixed (VkExtensionProperties* propertiesPtr = properties)
                {
                    vkEnumerateDeviceExtensionProperties(physicalDevice, playerName, &propertyCount, propertiesPtr).CheckResult();
                }

                return properties;
            }
        }
        else
        {
            uint propertyCount = 0;
            vkEnumerateDeviceExtensionProperties(physicalDevice, null, &propertyCount, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[propertyCount];
            fixed (VkExtensionProperties* propertiesPtr = properties)
            {
                vkEnumerateDeviceExtensionProperties(physicalDevice, null, &propertyCount, propertiesPtr).CheckResult();
            }
            return properties;
        }
    }

    /// <summary>
    /// Query instance-level version before instance creation.
    /// </summary>
    /// <returns>The version of Vulkan supported by instance-level functionality.</returns>
    public static VkVersion vkEnumerateInstanceVersion()
    {
        uint apiVersion;
        if (vkEnumerateInstanceVersion_ptr != null
            && vkEnumerateInstanceVersion(&apiVersion) == VkResult.Success)
        {
            return new VkVersion(apiVersion);
        }

        return VkVersion.Version_1_0;
    }

    public static VkResult vkAllocateCommandBuffer(VkDevice device, VkCommandBufferAllocateInfo* allocateInfo, out VkCommandBuffer commandBuffer)
    {
        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(device, allocateInfo, ptr);
        }
    }

    public static VkResult vkAllocateCommandBuffer(VkDevice device, VkCommandPool commandPool, out VkCommandBuffer commandBuffer)
    {
        VkCommandBufferAllocateInfo allocateInfo = new()
        {
            commandPool = commandPool,
            level = VkCommandBufferLevel.Primary,
            commandBufferCount = 1
        };

        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(device, &allocateInfo, ptr);
        }
    }

    public static VkResult vkAllocateCommandBuffer(VkDevice device, VkCommandPool commandPool, VkCommandBufferLevel level, out VkCommandBuffer commandBuffer)
    {
        VkCommandBufferAllocateInfo allocateInfo = new()
        {
            commandPool = commandPool,
            level = level,
            commandBufferCount = 1
        };

        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(device, &allocateInfo, ptr);
        }
    }

    public static VkResult vkCreateShaderModule(VkDevice device, nuint codeSize, byte* code, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        VkShaderModuleCreateInfo createInfo = new()
        {
            codeSize = codeSize,
            pCode = (uint*)code
        };

        return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
    }

    public static VkResult vkCreateShaderModule(VkDevice device, ReadOnlySpan<byte> bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        VkShaderModuleCreateInfo createInfo = new()
        {
            codeSize = (nuint)bytecode.Length,
            pCode = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(bytecode))
        };

        return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
    }

    public static VkResult vkCreateShaderModule(VkDevice device, byte[] bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        ReadOnlySpan<byte> span = bytecode.AsSpan();

        return vkCreateShaderModule(device, span, allocator, out shaderModule);
    }

    public static VkResult vkCreateGraphicsPipeline(VkDevice device, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateGraphicsPipelines(device, VkPipelineCache.Null, 1, &createInfo, null, pipelinePtr);
        }
    }

    public static VkResult vkCreateGraphicsPipeline(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateGraphicsPipelines(device, pipelineCache, 1, &createInfo, null, pipelinePtr);
        }
    }

    public static VkResult vkCreateGraphicsPipeline(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, VkPipeline* pipeline)
    {
        return vkCreateGraphicsPipelines_ptr(device, pipelineCache, 1, &createInfo, null, pipeline);
    }

    public static VkResult vkCreateComputePipeline(VkDevice device, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateComputePipelines(device, VkPipelineCache.Null, 1, &createInfo, null, pipelinePtr);
        }
    }

    public static VkResult vkCreateComputePipeline(VkDevice device, VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateComputePipelines(device, pipelineCache, 1, &createInfo, null, pipelinePtr);
        }
    }

    public static VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, VkPipeline* pipeline)
    {
        return vkCreateComputePipelines(device, pipelineCache, 1, &createInfo, null, pipeline);
    }

    public static VkDescriptorPool vkCreateDescriptorPool(VkDevice device, ReadOnlySpan<VkDescriptorPoolSize> poolSizes, uint maxSets = 1u)
    {
        VkDescriptorPoolCreateInfo createInfo = new()
        {
            maxSets = maxSets,
            poolSizeCount = (uint)poolSizes.Length,
            pPoolSizes = (VkDescriptorPoolSize*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(poolSizes))
        };

        VkDescriptorPool descriptorPool;
        vkCreateDescriptorPool_ptr(device, &createInfo, null, &descriptorPool).CheckResult();
        return descriptorPool;
    }

    public static VkResult vkCreateDescriptorPool(VkDevice device, ReadOnlySpan<VkDescriptorPoolSize> poolSizes, uint maxSets, out VkDescriptorPool descriptorPool)
    {
        Unsafe.SkipInit(out descriptorPool);

        fixed (VkDescriptorPool* descriptorPoolPtr = &descriptorPool)
        {
            VkDescriptorPoolCreateInfo createInfo = new()
            {
                maxSets = maxSets,
                poolSizeCount = (uint)poolSizes.Length,
                pPoolSizes = (VkDescriptorPoolSize*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(poolSizes))
            };

            return vkCreateDescriptorPool_ptr(device, &createInfo, null, descriptorPoolPtr);
        }
    }

    public static void vkUpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet writeDescriptorSet)
    {
        vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 0, null);
    }

    public static void vkUpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet writeDescriptorSet, VkCopyDescriptorSet copyDescriptorSet)
    {
        vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 1, &copyDescriptorSet);
    }

    public static void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            vkUpdateDescriptorSets(device, (uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, 0, null);
        }
    }

    public static void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets, ReadOnlySpan<VkCopyDescriptorSet> copyDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            fixed (VkCopyDescriptorSet* copyDescriptorSetsPtr = copyDescriptorSets)
            {
                vkUpdateDescriptorSets(device, (uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, (uint)copyDescriptorSets.Length, copyDescriptorSetsPtr);
            }
        }
    }

    public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* descriptorSets)
    {
        vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, descriptorSetCount, descriptorSets, 0, null);
    }

    public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet descriptorSet)
    {
        vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, 1, &descriptorSet, 0, null);
    }

    public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, ReadOnlySpan<VkDescriptorSet> descriptorSets)
    {
        fixed (VkDescriptorSet* descriptorSetsPtr = descriptorSets)
        {
            vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, (uint)descriptorSets.Length, descriptorSetsPtr, 0, null);
        }
    }

    public static void vkCmdBindDescriptorSets(
        VkCommandBuffer commandBuffer,
        VkPipelineBindPoint pipelineBindPoint,
        VkPipelineLayout layout,
        uint firstSet,
        ReadOnlySpan<VkDescriptorSet> descriptorSets,
        ReadOnlySpan<uint> dynamicOffsets)
    {
        fixed (VkDescriptorSet* descriptorSetsPtr = descriptorSets)
        {
            fixed (uint* dynamicOffsetsPtr = dynamicOffsets)
            {
                vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, (uint)descriptorSets.Length, descriptorSetsPtr, (uint)dynamicOffsets.Length, dynamicOffsetsPtr);
            }
        }
    }

    public static void vkCmdBindVertexBuffer(VkCommandBuffer commandBuffer, uint binding, VkBuffer buffer, ulong offset = 0)
    {
        vkCmdBindVertexBuffers(commandBuffer, binding, 1, &buffer, &offset);
    }

    public static void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, ReadOnlySpan<VkBuffer> buffers, ReadOnlySpan<ulong> offsets)
    {
        fixed (VkBuffer* buffersPtr = buffers)
        {
            fixed (ulong* offsetPtr = offsets)
            {
                vkCmdBindVertexBuffers(commandBuffer, firstBinding, (uint)buffers.Length, buffersPtr, offsetPtr);
            }
        }
    }

    public static void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, VkCommandBuffer secondaryCommandBuffer)
    {
        vkCmdExecuteCommands(commandBuffer, 1, &secondaryCommandBuffer);
    }

    public static void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, ReadOnlySpan<VkCommandBuffer> secondaryCommandBuffers)
    {
        fixed (VkCommandBuffer* commandBuffersPtr = secondaryCommandBuffers)
        {
            vkCmdExecuteCommands(commandBuffer, (uint)secondaryCommandBuffers.Length, commandBuffersPtr);
        }
    }

    public static VkResult vkQueuePresentKHR(VkQueue queue, VkSemaphore waitSemaphore, VkSwapchainKHR swapchain, uint imageIndex)
    {
        VkPresentInfoKHR presentInfo = new();

        if (waitSemaphore != VkSemaphore.Null)
        {
            presentInfo.waitSemaphoreCount = 1u;
            presentInfo.pWaitSemaphores = &waitSemaphore;
        }

        if (swapchain != VkSwapchainKHR.Null)
        {
            presentInfo.swapchainCount = 1u;
            presentInfo.pSwapchains = &swapchain;
            presentInfo.pImageIndices = &imageIndex;
        }

        return vkQueuePresentKHR(queue, &presentInfo);
    }

    public static VkResult vkCreateCommandPool(VkDevice device, uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new()
        {
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(device, &createInfo, null, out commandPool);
    }

    public static VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateFlags flags, uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new()
        {
            flags = flags,
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(device, &createInfo, null, out commandPool);
    }

    public static void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, VkCommandBuffer commandBuffer)
    {
        vkFreeCommandBuffers(device, commandPool, 1, &commandBuffer);
    }

    public static VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferUsageFlags flags)
    {
        VkCommandBufferBeginInfo beginInfo = new()
        {
            flags = flags
        };

        return vkBeginCommandBuffer(commandBuffer, &beginInfo);
    }

    public static VkResult vkCreateSemaphore(VkDevice device, out VkSemaphore semaphore)
    {
        VkSemaphoreCreateInfo createInfo = new();
        return vkCreateSemaphore(device, &createInfo, null, out semaphore);
    }

    public static VkResult vkCreateFence(VkDevice device, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new();
        return vkCreateFence(device, &createInfo, null, out fence);
    }

    public static VkResult vkCreateFence(VkDevice device, VkFenceCreateFlags flags, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new(flags);
        return vkCreateFence(device, &createInfo, null, out fence);
    }

    public static VkResult vkCreateTypedSemaphore(VkDevice device, VkSemaphoreType type, ulong initialValue, out VkSemaphore semaphore)
    {
        VkSemaphoreTypeCreateInfo typeCreateiInfo = new()
        {
            pNext = null,
            semaphoreType = type,
            initialValue = initialValue
        };

        VkSemaphoreCreateInfo createInfo = new()
        {
            pNext = &typeCreateiInfo,
            flags = VkSemaphoreCreateFlags.None
        };

        return vkCreateSemaphore(device, &createInfo, null, out semaphore);
    }

    public static VkResult vkCreateFramebuffer(
        VkDevice device,
        VkRenderPass renderPass,
        ReadOnlySpan<VkImageView> attachments,
        uint width,
        uint height,
        uint layers,
        out VkFramebuffer framebuffer)
    {
        fixed (VkImageView* attachmentsPtr = attachments)
        {
            VkFramebufferCreateInfo createInfo = new()
            {
                renderPass = renderPass,
                attachmentCount = (uint)attachments.Length,
                pAttachments = attachmentsPtr,
                width = width,
                height = height,
                layers = layers
            };

            return vkCreateFramebuffer(device, &createInfo, null, out framebuffer);
        }
    }

    public static VkResult vkCreateFramebuffer(
        VkDevice device,
        VkRenderPass renderPass,
        ReadOnlySpan<VkImageView> attachments,
        in VkExtent2D extent,
        uint layers,
        out VkFramebuffer framebuffer)
    {
        fixed (VkImageView* attachmentsPtr = attachments)
        {
            VkFramebufferCreateInfo createInfo = new VkFramebufferCreateInfo
            {
                renderPass = renderPass,
                attachmentCount = (uint)attachments.Length,
                pAttachments = attachmentsPtr,
                width = extent.width,
                height = extent.height,
                layers = layers
            };

            return vkCreateFramebuffer(device, &createInfo, null, out framebuffer);
        }
    }

    public static VkResult vkCreatePipelineLayout(
        VkDevice device,
        ReadOnlySpan<VkDescriptorSetLayout> setLayouts,
        ReadOnlySpan<VkPushConstantRange> pushConstantRanges,
        out VkPipelineLayout pipelineLayout)
    {
        VkPipelineLayoutCreateInfo createInfo = new()
        {
            pNext = null,
            setLayoutCount = (uint)setLayouts.Length,
            pSetLayouts = (VkDescriptorSetLayout*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(setLayouts)),
            pushConstantRangeCount = (uint)pushConstantRanges.Length,
            pPushConstantRanges = (VkPushConstantRange*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(pushConstantRanges)),
        };

        return vkCreatePipelineLayout(device, &createInfo, null, out pipelineLayout);
    }

    public static void vkCmdSetViewport(VkCommandBuffer commandBuffer, float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
    {
        VkViewport viewport = new(x, y, width, height, minDepth, maxDepth);
        vkCmdSetViewport_ptr(commandBuffer, 0, 1, &viewport);
    }

    public static void vkCmdSetViewport(VkCommandBuffer commandBuffer, VkViewport viewport)
    {
        vkCmdSetViewport_ptr(commandBuffer, 0, 1, &viewport);
    }

    public static void vkCmdSetViewport(VkCommandBuffer commandBuffer, VkViewport[] viewports)
    {
        fixed (VkViewport* viewportsPtr = viewports)
        {
            vkCmdSetViewport_ptr(commandBuffer, 0, (uint)viewports.Length, viewportsPtr);
        }
    }

    public static void vkCmdSetViewport(VkCommandBuffer commandBuffer, ReadOnlySpan<VkViewport> viewports)
    {
        fixed (VkViewport* viewportsPtr = viewports)
        {
            vkCmdSetViewport_ptr(commandBuffer, 0, (uint)viewports.Length, viewportsPtr);
        }
    }

    public static void vkCmdSetViewport<T>(VkCommandBuffer commandBuffer, uint firstViewport, T viewport) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkViewport))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkViewport)}", nameof(viewport));
        }
#endif
        vkCmdSetViewport_ptr(commandBuffer, firstViewport, 1, (VkViewport*)&viewport);
    }

    public static void vkCmdSetViewport<T>(VkCommandBuffer commandBuffer, uint firstViewport, int viewportCount, T* viewports) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkViewport))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkViewport)}", nameof(viewports));
        }
#endif

        vkCmdSetViewport_ptr(commandBuffer, firstViewport, (uint)viewportCount, (VkViewport*)viewports);
    }

    public static void vkCmdSetScissor(VkCommandBuffer commandBuffer, int x, int y, uint width, uint height)
    {
        VkRect2D scissor = new(x, y, width, height);
        vkCmdSetScissor_ptr(commandBuffer, 0, 1, &scissor);
    }

    public static void vkCmdSetScissor(VkCommandBuffer commandBuffer, VkRect2D scissor)
    {
        vkCmdSetScissor_ptr(commandBuffer, 0, 1, &scissor);
    }

    public static void vkCmdSetScissor<T>(VkCommandBuffer commandBuffer, uint firstScissor, T scissor) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkRect2D))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkRect2D)}", nameof(scissor));
        }
#endif
        vkCmdSetScissor_ptr(commandBuffer, firstScissor, 1, (VkRect2D*)&scissor);
    }

    public static void vkCmdSetScissor<T>(VkCommandBuffer commandBuffer, uint firstScissor, int scissorCount, T* scissorRects) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkRect2D))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkRect2D)}", nameof(scissorRects));
        }
#endif

        vkCmdSetScissor_ptr(commandBuffer, firstScissor, (uint)scissorCount, (VkRect2D*)scissorRects);
    }

    public static void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, float red, float green, float blue, float alpha)
    {
        float* blendConstantsArray = stackalloc float[] { red, green, blue, alpha };
        vkCmdSetBlendConstants(commandBuffer, blendConstantsArray);
    }

    public static void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, Vector4 blendConstants)
    {
        vkCmdSetBlendConstants(commandBuffer, &blendConstants.X);
    }

    public static void vkCmdSetFragmentShadingRateKHR(VkCommandBuffer commandBuffer, VkExtent2D* fragmentSize, VkFragmentShadingRateCombinerOpKHR[] combinerOps)
    {
        fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = &combinerOps[0])
        {
            vkCmdSetFragmentShadingRateKHR(commandBuffer, fragmentSize, combinerOpsPtr);
        }
    }

    public static void vkCmdSetFragmentShadingRateKHR(VkCommandBuffer commandBuffer, VkExtent2D[] fragmentSize, VkFragmentShadingRateCombinerOpKHR[] combinerOps)
    {
        fixed (VkExtent2D* fragmentSizePtr = &fragmentSize[0])
        {
            fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = &combinerOps[0])
            {
                vkCmdSetFragmentShadingRateKHR(commandBuffer, fragmentSizePtr, combinerOpsPtr);
            }
        }
    }

    public static void vkCmdSetFragmentShadingRateEnumNV(VkCommandBuffer commandBuffer, VkFragmentShadingRateNV shadingRate, VkFragmentShadingRateCombinerOpKHR[] combinerOps)
    {
        fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = &combinerOps[0])
        {
            vkCmdSetFragmentShadingRateEnumNV(commandBuffer, shadingRate, combinerOpsPtr);
        }
    }

    public static VkResult vkSetDebugUtilsObjectNameEXT(VkDevice device, VkObjectType objectType, ulong objectHandle, ReadOnlySpan<byte> label)
    {
        fixed (byte* pName = label)
        {
            VkDebugUtilsObjectNameInfoEXT info = new()
            {
                objectType = objectType,
                objectHandle = objectHandle,
                pObjectName = pName
            };
            return vkSetDebugUtilsObjectNameEXT(device, &info);
        }
    }

    public static VkResult vkSetDebugUtilsObjectNameEXT(VkDevice device, VkObjectType objectType, ulong objectHandle, string? label = default)
    {
        fixed (byte* pName = label.GetUtf8Span())
        {
            VkDebugUtilsObjectNameInfoEXT info = new()
            {
                objectType = objectType,
                objectHandle = objectHandle,
                pObjectName = pName
            };
            return vkSetDebugUtilsObjectNameEXT(device, &info);
        }
    }

    public static void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, ReadOnlySpan<VkBufferImageCopy> regions, uint regionCount = 0)
    {
        if (regionCount == 0)
            regionCount = (uint)regions.Length;

        fixed (VkBufferImageCopy* pRegions = regions)
        {
            vkCmdCopyBufferToImage_ptr(commandBuffer, srcBuffer, dstImage, dstImageLayout,
                regionCount,
                pRegions
             );
        }
    }

    public static void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, VkBufferImageCopy[] regions, uint regionCount = 0)
    {
        if (regionCount == 0)
            regionCount = (uint)regions.Length;

        fixed (VkBufferImageCopy* pRegions = regions)
        {
            vkCmdCopyBufferToImage_ptr(commandBuffer, srcBuffer, dstImage, dstImageLayout,
                regionCount,
                pRegions
             );
        }
    }

    public static void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, List<VkBufferImageCopy> regions, uint regionCount = 0)
    {
        if (regionCount == 0)
            regionCount = (uint)regions.Count;

        Span<VkBufferImageCopy> regionsSpan = CollectionsMarshal.AsSpan(regions);
        fixed (VkBufferImageCopy* pRegions = regionsSpan)
        {
            vkCmdCopyBufferToImage_ptr(commandBuffer, srcBuffer, dstImage, dstImageLayout,
                regionCount,
                pRegions
             );
        }
    }

    #region LibraryLoader
    private static ILibraryLoader GetPlatformLoader()
    {
        return new SystemNativeLibraryLoader();
    }

    interface ILibraryLoader
    {
        nint LoadNativeLibrary(string name);
        void FreeNativeLibrary(nint handle);

        nint LoadFunctionPointer(nint handle, string name);
    }

    private class SystemNativeLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            if (NativeLibrary.TryLoad(name, out nint lib))
            {
                return lib;
            }

            return 0;
        }

        public void FreeNativeLibrary(nint handle)
        {
            NativeLibrary.Free(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            if (NativeLibrary.TryGetExport(handle, name, out nint ptr))
            {
                return ptr;
            }

            return 0;
        }
    }
    #endregion
}
