// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using SystemNativeLibrary = System.Runtime.InteropServices.NativeLibrary;
#endif


namespace Vortice.Vulkan;

public static unsafe partial class Vulkan
{
    private static readonly ILibraryLoader _loader = GetPlatformLoader();
    private delegate delegate* unmanaged[Stdcall]<void> LoadFunction(nint context, string name);

    private static nint s_vulkanModule = 0;
    private static VkInstance s_loadedInstance = VkInstance.Null;
    private static VkDevice s_loadedDevice = VkDevice.Null;

    public const uint VK_TRUE = 1;
    public const uint VK_FALSE = 0;

    public static VkResult vkInitialize()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            s_vulkanModule = _loader.LoadNativeLibrary("vulkan-1.dll");
            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.dylib");
            if (s_vulkanModule == 0)
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.1.dylib");
            if (s_vulkanModule == 0)
                s_vulkanModule = _loader.LoadNativeLibrary("libMoltenVK.dylib");

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }
        else
        {
            s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so.1");
            if (s_vulkanModule == 0)
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so");

            if (s_vulkanModule == 0)
                return VkResult.ErrorInitializationFailed;
        }

        vkGetInstanceProcAddr_ptr = (delegate* unmanaged[Stdcall]<VkInstance, sbyte*, delegate* unmanaged[Stdcall]<void>>)_loader.LoadFunctionPointer(s_vulkanModule, nameof(vkGetInstanceProcAddr));
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

        vkGetDeviceProcAddr_ptr = (delegate* unmanaged[Stdcall]<VkDevice, sbyte*, delegate* unmanaged[Stdcall]<void>>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetDeviceProcAddr));

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
        vkCreateInstance_ptr = (delegate* unmanaged[Stdcall]<VkInstanceCreateInfo*, VkAllocationCallbacks*, VkInstance*, VkResult>)LoadCallbackThrow(context, load, "vkCreateInstance");
        vkCreateInstance_out_ptr = (delegate* unmanaged[Stdcall]<VkInstanceCreateInfo*, VkAllocationCallbacks*, out VkInstance, VkResult>)LoadCallbackThrow(context, load, "vkCreateInstance");
        vkEnumerateInstanceExtensionProperties_ptr = (delegate* unmanaged[Stdcall]<sbyte*, int*, VkExtensionProperties*, VkResult>)LoadCallbackThrow(context, load, "vkEnumerateInstanceExtensionProperties");
        vkEnumerateInstanceLayerProperties_ptr = (delegate* unmanaged[Stdcall]<int*, VkLayerProperties*, VkResult>)LoadCallbackThrow(context, load, "vkEnumerateInstanceLayerProperties");
        vkEnumerateInstanceVersion_ptr = (delegate* unmanaged[Stdcall]<uint*, VkResult>)load(context, "vkEnumerateInstanceVersion");
    }

    private static delegate* unmanaged[Stdcall]<void> LoadCallbackThrow(nint context, LoadFunction load, string name)
    {
        delegate* unmanaged[Stdcall]<void> functionPtr = load(context, name);
        if (functionPtr == null)
        {
            throw new InvalidOperationException($"No function was found with the name {name}.");
        }

        return functionPtr;
    }

    public static delegate* unmanaged[Stdcall]<VkInstance, sbyte*, delegate* unmanaged[Stdcall]<void>> vkGetInstanceProcAddr_ptr;

    public static delegate* unmanaged[Stdcall]<void> vkGetInstanceProcAddr(VkInstance instance, sbyte* name)
    {
        return vkGetInstanceProcAddr_ptr(instance, name);
    }

    public static delegate* unmanaged[Stdcall]<void> vkGetInstanceProcAddr(VkInstance instance, ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return vkGetInstanceProcAddr_ptr(instance, pName);
        }
    }

    public static delegate* unmanaged[Stdcall]<void> vkGetInstanceProcAddr(IntPtr instance, string name)
    {
        return vkGetInstanceProcAddr(instance, name.GetUtf8Span());
    }

    public static delegate* unmanaged[Stdcall]<VkDevice, sbyte*, delegate* unmanaged[Stdcall]<void>> vkGetDeviceProcAddr_ptr;

    public static delegate* unmanaged[Stdcall]<void> vkGetDeviceProcAddr(VkDevice device, sbyte* name)
    {
        return vkGetDeviceProcAddr_ptr(device, name);
    }

    public static delegate* unmanaged[Stdcall]<void> vkGetDeviceProcAddr(VkDevice device, ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return vkGetDeviceProcAddr_ptr(device, pName);
        }
    }

    public static delegate* unmanaged[Stdcall]<void> vkGetDeviceProcAddr(IntPtr device, string name)
    {
        return vkGetDeviceProcAddr(device, name.GetUtf8Span());
    }

    public static VkResult vkEnumerateInstanceExtensionProperties(int* propertyCount, VkExtensionProperties* properties)
    {
        return vkEnumerateInstanceExtensionProperties_ptr((sbyte*)null, propertyCount, properties);
    }

    /// <summary>
    /// Returns up to requested number of global extension properties
    /// </summary>
    /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
    /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/> </returns>
    /// <exception cref="VkException">Vulkan returns an error code.</exception>
    public static ReadOnlySpan<VkExtensionProperties> vkEnumerateInstanceExtensionProperties(string? layerName = null)
    {
        if (layerName is not null)
        {
            ReadOnlySpan<sbyte> layerNameSpan = layerName.GetUtf8Span();

            fixed (sbyte* pLayerName = layerNameSpan)
            {
                int count = 0;
                vkEnumerateInstanceExtensionProperties(pLayerName, &count, null).CheckResult();

                ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
                fixed (VkExtensionProperties* ptr = properties)
                {
                    vkEnumerateInstanceExtensionProperties(pLayerName, &count, ptr).CheckResult();
                }

                return properties;
            }
        }
        else
        {
            int count = 0;
            vkEnumerateInstanceExtensionProperties(null, &count, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* ptr = properties)
            {
                vkEnumerateInstanceExtensionProperties(null, &count, ptr).CheckResult();
            }

            return properties;
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
            ReadOnlySpan<sbyte> layerNameSpan = layerName.GetUtf8Span();

            fixed (sbyte* playerName = layerNameSpan)
            {
                int propertyCount = 0;
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
            int propertyCount = 0;
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
            sType = VkStructureType.CommandBufferAllocateInfo,
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
            sType = VkStructureType.CommandBufferAllocateInfo,
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
        var createInfo = new VkShaderModuleCreateInfo
        {
            sType = VkStructureType.ShaderModuleCreateInfo,
            codeSize = codeSize,
            pCode = (uint*)code
        };

        return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
    }

    public static VkResult vkCreateShaderModule(VkDevice device, ReadOnlySpan<byte> bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        var createInfo = new VkShaderModuleCreateInfo
        {
            sType = VkStructureType.ShaderModuleCreateInfo,
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
            vkUpdateDescriptorSets(device, (int)writeDescriptorSets.Length, writeDescriptorSetsPtr, 0, null);
        }
    }

    public static void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets, ReadOnlySpan<VkCopyDescriptorSet> copyDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            fixed (VkCopyDescriptorSet* copyDescriptorSetsPtr = copyDescriptorSets)
            {
                vkUpdateDescriptorSets(device, writeDescriptorSets.Length, writeDescriptorSetsPtr, (int)copyDescriptorSets.Length, copyDescriptorSetsPtr);
            }
        }
    }

    public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, int descriptorSetCount, VkDescriptorSet* descriptorSets)
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
            vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, descriptorSets.Length, descriptorSetsPtr, 0, null);
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
                vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, (int)descriptorSets.Length, descriptorSetsPtr, dynamicOffsets.Length, dynamicOffsetsPtr);
            }
        }
    }

    public static void vkCmdBindVertexBuffer(VkCommandBuffer commandBuffer, int binding, VkBuffer buffer, ulong offset = 0)
    {
        vkCmdBindVertexBuffers(commandBuffer, (uint)binding, 1, &buffer, &offset);
    }

    public static void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, ReadOnlySpan<VkBuffer> buffers, ReadOnlySpan<ulong> offsets)
    {
        fixed (VkBuffer* buffersPtr = buffers)
        {
            fixed (ulong* offsetPtr = offsets)
            {
                vkCmdBindVertexBuffers(commandBuffer, firstBinding, buffers.Length, buffersPtr, offsetPtr);
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
            vkCmdExecuteCommands(commandBuffer, secondaryCommandBuffers.Length, commandBuffersPtr);
        }
    }

    public static VkResult vkQueuePresentKHR(VkQueue queue, VkSemaphore waitSemaphore, VkSwapchainKHR swapchain, uint imageIndex)
    {
        var presentInfo = new VkPresentInfoKHR
        {
            sType = VkStructureType.PresentInfoKHR,
            pNext = null
        };

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
        VkCommandPoolCreateInfo createInfo = new VkCommandPoolCreateInfo
        {
            sType = VkStructureType.CommandPoolCreateInfo,
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(device, &createInfo, null, out commandPool);
    }

    public static VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateFlags flags, uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new VkCommandPoolCreateInfo
        {
            sType = VkStructureType.CommandPoolCreateInfo,
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
            sType = VkStructureType.CommandBufferBeginInfo,
            flags = flags
        };

        return vkBeginCommandBuffer(commandBuffer, &beginInfo);
    }

    public static VkResult vkCreateSemaphore(VkDevice device, out VkSemaphore semaphore)
    {
        VkSemaphoreCreateInfo createInfo = VkSemaphoreCreateInfo.New();
        return vkCreateSemaphore(device, &createInfo, null, out semaphore);
    }

    public static VkResult vkCreateFence(VkDevice device, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = VkFenceCreateInfo.New();
        return vkCreateFence(device, &createInfo, null, out fence);
    }

    public static VkResult vkCreateFence(VkDevice device, VkFenceCreateFlags flags, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new()
        {
            sType = VkStructureType.FenceCreateInfo,
            pNext = null,
            flags = flags
        };

        return vkCreateFence(device, &createInfo, null, out fence);
    }

    public static VkResult vkCreateTypedSemaphore(VkDevice device, VkSemaphoreType type, ulong initialValue, out VkSemaphore semaphore)
    {
        VkSemaphoreTypeCreateInfo typeCreateiInfo = new VkSemaphoreTypeCreateInfo
        {
            sType = VkStructureType.SemaphoreTypeCreateInfo,
            pNext = null,
            semaphoreType = type,
            initialValue = initialValue
        };

        VkSemaphoreCreateInfo createInfo = new VkSemaphoreCreateInfo
        {
            sType = VkStructureType.SemaphoreCreateInfo,
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
            VkFramebufferCreateInfo createInfo = new VkFramebufferCreateInfo
            {
                sType = VkStructureType.FramebufferCreateInfo,
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
                sType = VkStructureType.FramebufferCreateInfo,
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
            sType = VkStructureType.PipelineLayoutCreateInfo,
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
            vkCmdSetViewport_ptr(commandBuffer, 0, viewports.Length, viewportsPtr);
        }
    }

    public static void vkCmdSetViewport(VkCommandBuffer commandBuffer, ReadOnlySpan<VkViewport> viewports)
    {
        fixed (VkViewport* viewportsPtr = viewports)
        {
            vkCmdSetViewport_ptr(commandBuffer, 0, viewports.Length, viewportsPtr);
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

        vkCmdSetViewport_ptr(commandBuffer, firstViewport, viewportCount, (VkViewport*)viewports);
    }

    public static void vkCmdSetScissor(VkCommandBuffer commandBuffer, int x, int y, int width, int height)
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

        vkCmdSetScissor_ptr(commandBuffer, firstScissor, scissorCount, (VkRect2D*)scissorRects);
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

    private static ILibraryLoader GetPlatformLoader()
    {
#if NET6_0_OR_GREATER
        return new SystemNativeLibraryLoader();
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Win32LibraryLoader();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.OSDescription.ToUpper().Contains("BSD"))
        {
            return new BsdLibraryLoader();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new UnixLibraryLoader();
        }

        throw new PlatformNotSupportedException("This platform cannot load native libraries.");
#endif
    }

    interface ILibraryLoader
    {
        nint LoadNativeLibrary(string name);
        void FreeNativeLibrary(nint handle);

        nint LoadFunctionPointer(nint handle, string name);
    }

#if NET6_0_OR_GREATER
    private class SystemNativeLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            if (SystemNativeLibrary.TryLoad(name, out nint lib))
            {
                return lib;
            }

            return 0;
        }

        public void FreeNativeLibrary(nint handle)
        {
            SystemNativeLibrary.Free(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            if (SystemNativeLibrary.TryGetExport(handle, name, out nint ptr))
            {
                return ptr;
            }

            return 0;
        }
    }
#else
    private class Win32LibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            return LoadLibrary(name);
        }

        public void FreeNativeLibrary(nint handle)
        {
            FreeLibrary(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            return GetProcAddress(handle, name);
        }

        [DllImport("kernel32")]
        private static extern nint LoadLibrary(string fileName);

        [DllImport("kernel32")]
        private static extern int FreeLibrary(nint module);

        [DllImport("kernel32")]
        private static extern nint GetProcAddress(nint module, string procName);
    }

    private class UnixLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            return Libdl.dlopen(name, Libdl.RTLD_NOW | Libdl.RTLD_LOCAL);
        }

        public void FreeNativeLibrary(nint handle)
        {
            Libdl.dlclose(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            return Libdl.dlsym(handle, name);
        }
    }

    private class BsdLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            return Libc.dlopen(name, Libc.RTLD_NOW | Libc.RTLD_LOCAL);
        }

        public void FreeNativeLibrary(nint handle)
        {
            Libc.dlclose(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            return Libc.dlsym(handle, name);
        }
    }

    internal static class Libdl
    {
        private const string LibName = "libdl";

        public const int RTLD_LOCAL = 0x000;
        public const int RTLD_NOW = 0x002;

        [DllImport(LibName)]
        public static extern nint dlopen(string fileName, int flags);

        [DllImport(LibName)]
        public static extern nint dlsym(nint handle, string name);

        [DllImport(LibName)]
        public static extern int dlclose(nint handle);

        [DllImport(LibName)]
        public static extern string dlerror();
    }

    internal static class Libc
    {
        private const string LibName = "libc";

        public const int RTLD_LOCAL = 0x000;
        public const int RTLD_NOW = 0x002;

        [DllImport(LibName)]
        public static extern nint dlopen(string fileName, int flags);

        [DllImport(LibName)]
        public static extern nint dlsym(nint handle, string name);

        [DllImport(LibName)]
        public static extern int dlclose(nint handle);

        [DllImport(LibName)]
        public static extern string dlerror();
    }
#endif
}
