// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Vortice.Mathematics;
using static Vortice.Vulkan.Vulkan.Kernel32;
using static Vortice.Vulkan.Vulkan.Libdl;

namespace Vortice.Vulkan
{
    public static unsafe partial class Vulkan
    {
        private delegate IntPtr LoadFunction(IntPtr context, string name);

        private static IntPtr s_vulkanModule = IntPtr.Zero;
        private static VkInstance s_loadedInstance = VkInstance.Null;
        private static VkDevice s_loadedDevice = VkDevice.Null;

        public static VkResult vkInitialize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                s_vulkanModule = LoadLibrary("vulkan-1.dll");
                if (s_vulkanModule == IntPtr.Zero)
                    return VkResult.ErrorInitializationFailed;

                vkGetInstanceProcAddr_ptr = GetProcAddress(s_vulkanModule, nameof(vkGetInstanceProcAddr));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                s_vulkanModule = dlopen("libvulkan.dylib", RTLD_NOW);
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = dlopen("libvulkan.1.dylib", RTLD_NOW);
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = dlopen("libMoltenVK.dylib", RTLD_NOW);

                if (s_vulkanModule == IntPtr.Zero)
                    return VkResult.ErrorInitializationFailed;

                vkGetInstanceProcAddr_ptr = dlsym(s_vulkanModule, nameof(vkGetInstanceProcAddr));
            }
            else
            {
                s_vulkanModule = dlopen("libvulkan.so.1", RTLD_NOW);
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = dlopen("libvulkan.so", RTLD_NOW);

                if (s_vulkanModule == IntPtr.Zero)
                    return VkResult.ErrorInitializationFailed;

                vkGetInstanceProcAddr_ptr = dlsym(s_vulkanModule, nameof(vkGetInstanceProcAddr));
            }

            GenLoadLoader(IntPtr.Zero, vkGetInstanceProcAddr);

            return VkResult.Success;
        }

        public static void vkLoadInstance(VkInstance instance)
        {
            s_loadedInstance = instance;
            GenLoadInstance(instance.Handle, vkGetInstanceProcAddr);
            GenLoadDevice(instance.Handle, vkGetInstanceProcAddr);

            // Manually loaded entries.
            vkCreateWin32SurfaceKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateWin32SurfaceKHR));
            vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceWin32PresentationSupportKHR));

            vkCreateXcbSurfaceKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateXcbSurfaceKHR));
            vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceXcbPresentationSupportKHR));

            vkCreateXlibSurfaceKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateXlibSurfaceKHR));
            vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceXlibPresentationSupportKHR));

            vkCreateWaylandSurfaceKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateWaylandSurfaceKHR));
            vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr = vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceWaylandPresentationSupportKHR));
        }

        private static void GenLoadLoader(IntPtr context, LoadFunction load)
        {
            vkCreateInstance_ptr = LoadCallbackThrow(context, load, "vkCreateInstance");
            vkEnumerateInstanceExtensionProperties_ptr = LoadCallbackThrow(context, load, "vkEnumerateInstanceExtensionProperties");
            vkEnumerateInstanceLayerProperties_ptr = LoadCallbackThrow(context, load, "vkEnumerateInstanceLayerProperties");
            vkEnumerateInstanceVersion_ptr = load(context, "vkEnumerateInstanceVersion");
        }

        private static IntPtr LoadCallbackThrow(IntPtr context, LoadFunction load, string name)
        {
            IntPtr functionPtr = load(context, name);
            if (functionPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"No function was found with the name {name}.");
            }

            return functionPtr;
        }

        public static IntPtr vkGetInstanceProcAddr(IntPtr instance, string name)
        {
            int byteCount = Interop.GetMaxByteCount(name);
            byte* stringPtr = stackalloc byte[byteCount];
            Interop.StringToPointer(name, stringPtr, byteCount);
            return vkGetInstanceProcAddr(instance, stringPtr);
        }

        /// <summary>
        /// Returns up to requested number of global extension properties
        /// </summary>
        /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
        /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/> </returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static unsafe ReadOnlySpan<VkExtensionProperties> vkEnumerateInstanceExtensionProperties(string? layerName = null)
        {
            int dstLayerNameByteCount = Interop.GetMaxByteCount(layerName);
            byte* dstLayerNamePtr = stackalloc byte[dstLayerNameByteCount];
            Interop.StringToPointer(layerName, dstLayerNamePtr, dstLayerNameByteCount);

            uint count = 0;
            vkEnumerateInstanceExtensionProperties(dstLayerNamePtr, &count, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* ptr = properties)
            {
                vkEnumerateInstanceExtensionProperties(dstLayerNamePtr, &count, ptr).CheckResult();
            }

            return properties;
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
            int dstLayerNameByteCount = Interop.GetMaxByteCount(layerName);
            byte* dstLayerNamePtr = stackalloc byte[dstLayerNameByteCount];
            Interop.StringToPointer(layerName, dstLayerNamePtr, dstLayerNameByteCount);

            uint propertyCount = 0;
            vkEnumerateDeviceExtensionProperties(physicalDevice, dstLayerNamePtr, &propertyCount, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[propertyCount];
            fixed (VkExtensionProperties* propertiesPtr = properties)
            {
                vkEnumerateDeviceExtensionProperties(physicalDevice, dstLayerNamePtr, &propertyCount, propertiesPtr).CheckResult();
            }
            return properties;
        }

        /// <summary>
        /// Query instance-level version before instance creation.
        /// </summary>
        /// <returns>The version of Vulkan supported by instance-level functionality.</returns>
        public static unsafe VkVersion vkEnumerateInstanceVersion()
        {
            if (vkEnumerateInstanceVersion_ptr != IntPtr.Zero
                && vkEnumerateInstanceVersion(out uint apiVersion) == VkResult.Success)
            {
                return new VkVersion(apiVersion);
            }

            return VkVersion.Version_1_0;
        }

        public static VkResult vkCreateShaderModule(VkDevice device, byte[] bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
        {
            fixed (byte* bytecodePtr = bytecode)
            {
                var createInfo = new VkShaderModuleCreateInfo
                {
                    sType = VkStructureType.ShaderModuleCreateInfo,
                    codeSize = new VkPointerSize((uint)bytecode.Length),
                    pCode = (uint*)bytecodePtr
                };

                return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
            }
        }

        [Calli]
        public static VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* createInfos, VkAllocationCallbacks* allocator, out VkPipeline pipelines)
        {
            throw new NotImplementedException();
        }

        [Calli]
        public static VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* createInfos, VkAllocationCallbacks* allocator, out VkPipeline pipelines)
        {
            throw new NotImplementedException();
        }

        public static VkResult vkCreateGraphicsPipeline(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
        {
            return vkCreateGraphicsPipelines(device, pipelineCache, 1, &createInfo, null, out pipeline);
        }

        public static VkResult vkCreateGraphicsPipelines(
            VkDevice device,
            VkPipelineCache pipelineCache,
            ReadOnlySpan<VkGraphicsPipelineCreateInfo> createInfos,
            Span<VkPipeline> pipelines)
        {
            fixed (VkGraphicsPipelineCreateInfo* createInfosPtr = createInfos)
            {
                fixed (VkPipeline* pipelinesPtr = pipelines)
                {
                    return vkCreateGraphicsPipelines(device, pipelineCache, (uint)createInfos.Length, createInfosPtr, null, pipelinesPtr);
                }
            }
        }

        public static VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
        {
            return vkCreateComputePipelines(device, pipelineCache, 1, &createInfo, null, out pipeline);
        }

        public static VkResult vkCreateComputePipelines(
            VkDevice device,
            VkPipelineCache pipelineCache,
            ReadOnlySpan<VkComputePipelineCreateInfo> createInfos,
            Span<VkPipeline> pipelines)
        {
            fixed (VkComputePipelineCreateInfo* createInfosPtr = createInfos)
            {
                fixed (VkPipeline* pipelinesPtr = pipelines)
                {
                    return vkCreateComputePipelines(device, pipelineCache, (uint)createInfos.Length, createInfosPtr, null, pipelinesPtr);
                }
            }
        }

        public static Span<T> vkMapMemory<T>(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, ulong offset = 0, ulong size = WholeSize, VkMemoryMapFlags flags = VkMemoryMapFlags.None) where T : unmanaged
        {
            void* pData;
            vkMapMemory(device, memory, offset, size, flags, &pData).CheckResult();

            if (size == WholeSize)
            {
                vkGetBufferMemoryRequirements(device, buffer, out VkMemoryRequirements memoryRequirements);
                return new Span<T>(pData, (int)memoryRequirements.size);
            }

            return new Span<T>(pData, (int)size);
        }

        public static Span<T> vkMapMemory<T>(VkDevice device, VkImage image, VkDeviceMemory memory, ulong offset = 0, ulong size = WholeSize, VkMemoryMapFlags flags = VkMemoryMapFlags.None) where T : unmanaged
        {
            void* pData;
            vkMapMemory(device, memory, offset, size, flags, &pData).CheckResult();

            if (size == WholeSize)
            {
                vkGetImageMemoryRequirements(device, image, out VkMemoryRequirements memoryRequirements);
                return new Span<T>(pData, (int)memoryRequirements.size);
            }

            return new Span<T>(pData, (int)size);
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

        public static void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, VkBuffer buffer, ulong offset = 0)
        {
            vkCmdBindVertexBuffers(commandBuffer, firstBinding, 1, &buffer, &offset);
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

        [Calli]
        public static VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* createInfo, VkAllocationCallbacks* allocator, VkImageView* view)
        {
            throw new NotImplementedException();
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

        [Calli]
        public static VkResult vkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* allocateInfo, out VkCommandBuffer commandBuffers)
        {
            throw new NotImplementedException();
        }

        public static void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, VkCommandBuffer commandBuffer)
        {
            vkFreeCommandBuffers(device, commandPool, 1u, &commandBuffer);
        }

        public static VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferUsageFlags flags)
        {
            VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.OneTimeSubmit
            };

            return vkBeginCommandBuffer(commandBuffer, &beginInfo);
        }

        public static VkResult vkCreateSemaphore(VkDevice device, out VkSemaphore semaphore)
        {
            VkSemaphoreCreateInfo createInfo = new VkSemaphoreCreateInfo
            {
                sType = VkStructureType.SemaphoreCreateInfo,
                pNext = null,
                flags = VkSemaphoreCreateFlags.None
            };

            return vkCreateSemaphore(device, &createInfo, null, out semaphore);
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
            in Size extent,
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
                    width = (uint)extent.Width,
                    height = (uint)extent.Height,
                    layers = layers
                };

                return vkCreateFramebuffer(device, &createInfo, null, out framebuffer);
            }
        }

        [Calli]
        public static VkResult vkAllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* allocateInfo, out VkDescriptorSet descriptorSets)
        {
            throw new NotImplementedException();
        }

        #region Nested
        internal static class Kernel32
        {
            [DllImport("kernel32")]
            public static extern IntPtr LoadLibrary(string fileName);

            [DllImport("kernel32")]
            public static extern IntPtr GetProcAddress(IntPtr module, string procName);
        }

        internal static class Libdl
        {
            [DllImport("libdl")]
            public static extern IntPtr dlopen(string fileName, int flags);

            [DllImport("libdl")]
            public static extern IntPtr dlsym(IntPtr handle, string name);

            [DllImport("libdl")]
            public static extern int dlclose(IntPtr handle);

            public const int RTLD_NOW = 0x002;
        }
        #endregion
    }
}
