// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Vortice.Vulkan;

public unsafe partial class VkDeviceApi
{
    public VkResult vkAllocateCommandBuffer(VkDevice device, VkCommandBufferAllocateInfo* allocateInfo, out VkCommandBuffer commandBuffer)
    {
        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(device, allocateInfo, ptr);
        }
    }

    public VkResult vkAllocateCommandBuffer(VkDevice device, VkCommandPool commandPool, out VkCommandBuffer commandBuffer)
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

    public VkResult vkAllocateCommandBuffer(VkDevice device, VkCommandPool commandPool, VkCommandBufferLevel level, out VkCommandBuffer commandBuffer)
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

    public VkResult vkCreateShaderModule(VkDevice device, nuint codeSize, byte* code, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        VkShaderModuleCreateInfo createInfo = new()
        {
            codeSize = codeSize,
            pCode = (uint*)code
        };

        return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
    }

    public VkResult vkCreateShaderModule(VkDevice device, Span<byte> bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                codeSize = (nuint)bytecode.Length,
                pCode = (uint*)bytecodePtr
            };

            return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
        }
    }

    public VkResult vkCreateShaderModule(VkDevice device, ReadOnlySpan<byte> bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                codeSize = (nuint)bytecode.Length,
                pCode = (uint*)bytecodePtr
            };

            return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
        }
    }

    public VkResult vkCreateGraphicsPipeline(VkDevice device, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateGraphicsPipelines(device, VkPipelineCache.Null, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateGraphicsPipeline(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateGraphicsPipelines(device, pipelineCache, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateGraphicsPipeline(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, VkPipeline* pipeline)
    {
        return vkCreateGraphicsPipelines(device, pipelineCache, 1, &createInfo, null, pipeline);
    }

    public VkResult vkCreateComputePipeline(VkDevice device, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateComputePipelines(device, VkPipelineCache.Null, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateComputePipeline(VkDevice device, VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateComputePipelines(device, pipelineCache, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, VkPipeline* pipeline)
    {
        return vkCreateComputePipelines(device, pipelineCache, 1, &createInfo, null, pipeline);
    }

    public VkDescriptorPool vkCreateDescriptorPool(VkDevice device, ReadOnlySpan<VkDescriptorPoolSize> poolSizes, uint maxSets = 1u)
    {
        fixed (VkDescriptorPoolSize* poolSizesPtr = poolSizes)
        {
            VkDescriptorPoolCreateInfo createInfo = new()
            {
                maxSets = maxSets,
                poolSizeCount = (uint)poolSizes.Length,
                pPoolSizes = poolSizesPtr
            };

            VkDescriptorPool descriptorPool;
            vkCreateDescriptorPool(device, &createInfo, null, &descriptorPool).CheckResult();
            return descriptorPool;
        }
    }

    public VkResult vkCreateDescriptorPool(VkDevice device, ReadOnlySpan<VkDescriptorPoolSize> poolSizes, uint maxSets, out VkDescriptorPool descriptorPool)
    {
        Unsafe.SkipInit(out descriptorPool);

        fixed (VkDescriptorPoolSize* poolSizesPtr = poolSizes)
        {
            fixed (VkDescriptorPool* descriptorPoolPtr = &descriptorPool)
            {
                VkDescriptorPoolCreateInfo createInfo = new()
                {
                    maxSets = maxSets,
                    poolSizeCount = (uint)poolSizes.Length,
                    pPoolSizes = poolSizesPtr
                };

                return vkCreateDescriptorPool(device, &createInfo, null, descriptorPoolPtr);
            }
        }
    }

    public void vkUpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet writeDescriptorSet)
    {
        vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 0, null);
    }

    public void vkUpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet writeDescriptorSet, VkCopyDescriptorSet copyDescriptorSet)
    {
        vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 1, &copyDescriptorSet);
    }

    public void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            vkUpdateDescriptorSets(device, (uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, 0, null);
        }
    }

    public void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets, ReadOnlySpan<VkCopyDescriptorSet> copyDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            fixed (VkCopyDescriptorSet* copyDescriptorSetsPtr = copyDescriptorSets)
            {
                vkUpdateDescriptorSets(device, (uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, (uint)copyDescriptorSets.Length, copyDescriptorSetsPtr);
            }
        }
    }

    public void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* descriptorSets)
    {
        vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, descriptorSetCount, descriptorSets, 0, null);
    }

    public void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet descriptorSet)
    {
        vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, 1, &descriptorSet, 0, null);
    }

    public void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, ReadOnlySpan<VkDescriptorSet> descriptorSets)
    {
        fixed (VkDescriptorSet* descriptorSetsPtr = descriptorSets)
        {
            vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, (uint)descriptorSets.Length, descriptorSetsPtr, 0, null);
        }
    }

    public void vkCmdBindDescriptorSets(
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

    public void vkCmdBindVertexBuffer(VkCommandBuffer commandBuffer, uint binding, VkBuffer buffer, ulong offset = 0)
    {
        vkCmdBindVertexBuffers(commandBuffer, binding, 1, &buffer, &offset);
    }

    public void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, ReadOnlySpan<VkBuffer> buffers, ReadOnlySpan<ulong> offsets)
    {
        fixed (VkBuffer* buffersPtr = buffers)
        {
            fixed (ulong* offsetPtr = offsets)
            {
                vkCmdBindVertexBuffers(commandBuffer, firstBinding, (uint)buffers.Length, buffersPtr, offsetPtr);
            }
        }
    }

    public void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, VkCommandBuffer secondaryCommandBuffer)
    {
        vkCmdExecuteCommands(commandBuffer, 1, &secondaryCommandBuffer);
    }

    public void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, ReadOnlySpan<VkCommandBuffer> secondaryCommandBuffers)
    {
        fixed (VkCommandBuffer* commandBuffersPtr = secondaryCommandBuffers)
        {
            vkCmdExecuteCommands(commandBuffer, (uint)secondaryCommandBuffers.Length, commandBuffersPtr);
        }
    }

    public VkResult vkQueuePresentKHR(VkQueue queue, VkSemaphore waitSemaphore, VkSwapchainKHR swapchain, uint imageIndex)
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

    public VkResult vkCreateCommandPool(VkDevice device, uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new()
        {
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(device, &createInfo, null, out commandPool);
    }

    public VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateFlags flags, uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new()
        {
            flags = flags,
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(device, &createInfo, null, out commandPool);
    }

    public void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, VkCommandBuffer commandBuffer)
    {
        vkFreeCommandBuffers(device, commandPool, 1, &commandBuffer);
    }

    public VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferUsageFlags flags)
    {
        VkCommandBufferBeginInfo beginInfo = new()
        {
            flags = flags
        };

        return vkBeginCommandBuffer(commandBuffer, &beginInfo);
    }

    public VkResult vkCreateSemaphore(VkDevice device, out VkSemaphore semaphore)
    {
        VkSemaphoreCreateInfo createInfo = new();
        return vkCreateSemaphore(device, &createInfo, null, out semaphore);
    }

    public VkResult vkCreateFence(VkDevice device, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new();
        return vkCreateFence(device, &createInfo, null, out fence);
    }

    public VkResult vkCreateFence(VkDevice device, VkFenceCreateFlags flags, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new(flags);
        return vkCreateFence(device, &createInfo, null, out fence);
    }

    public VkResult vkCreateTypedSemaphore(VkDevice device, VkSemaphoreType type, ulong initialValue, out VkSemaphore semaphore)
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

    public VkResult vkCreateFramebuffer(
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

    public VkResult vkCreateFramebuffer(
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

    public VkResult vkCreatePipelineLayout(
        VkDevice device,
        ReadOnlySpan<VkDescriptorSetLayout> setLayouts,
        ReadOnlySpan<VkPushConstantRange> pushConstantRanges,
        out VkPipelineLayout pipelineLayout)
    {
        fixed (VkDescriptorSetLayout* setLayoutsPtr = setLayouts)
        {
            fixed (VkPushConstantRange* pushConstantRangesPtr = pushConstantRanges)
            {
                VkPipelineLayoutCreateInfo createInfo = new()
                {
                    pNext = null,
                    setLayoutCount = (uint)setLayouts.Length,
                    pSetLayouts = setLayoutsPtr,
                    pushConstantRangeCount = (uint)pushConstantRanges.Length,
                    pPushConstantRanges = pushConstantRangesPtr,
                };

                return vkCreatePipelineLayout(device, &createInfo, null, out pipelineLayout);
            }
        }
    }

    public void vkCmdSetViewport(VkCommandBuffer commandBuffer, float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
    {
        VkViewport viewport = new(x, y, width, height, minDepth, maxDepth);
        vkCmdSetViewport(commandBuffer, 0, 1, &viewport);
    }

    public void vkCmdSetViewport(VkCommandBuffer commandBuffer, VkViewport viewport)
    {
        vkCmdSetViewport(commandBuffer, 0, 1, &viewport);
    }

    public void vkCmdSetViewport(VkCommandBuffer commandBuffer, ReadOnlySpan<VkViewport> viewports)
    {
        fixed (VkViewport* viewportsPtr = viewports)
        {
            vkCmdSetViewport(commandBuffer, 0, (uint)viewports.Length, viewportsPtr);
        }
    }

    public void vkCmdSetViewport<T>(VkCommandBuffer commandBuffer, uint firstViewport, T viewport) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkViewport))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkViewport)}", nameof(viewport));
        }
#endif
        vkCmdSetViewport(commandBuffer, firstViewport, 1, (VkViewport*)&viewport);
    }

    public void vkCmdSetViewport<T>(VkCommandBuffer commandBuffer, uint firstViewport, int viewportCount, T* viewports) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkViewport))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkViewport)}", nameof(viewports));
        }
#endif

        vkCmdSetViewport(commandBuffer, firstViewport, (uint)viewportCount, (VkViewport*)viewports);
    }

    public void vkCmdSetScissor(VkCommandBuffer commandBuffer, int x, int y, uint width, uint height)
    {
        VkRect2D scissor = new(x, y, width, height);
        vkCmdSetScissor(commandBuffer, 0, 1, &scissor);
    }

    public void vkCmdSetScissor(VkCommandBuffer commandBuffer, VkRect2D scissor)
    {
        vkCmdSetScissor(commandBuffer, 0, 1, &scissor);
    }

    public void vkCmdSetScissor<T>(VkCommandBuffer commandBuffer, uint firstScissor, T scissor) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkRect2D))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkRect2D)}", nameof(scissor));
        }
#endif
        vkCmdSetScissor(commandBuffer, firstScissor, 1, (VkRect2D*)&scissor);
    }

    public void vkCmdSetScissor<T>(VkCommandBuffer commandBuffer, uint firstScissor, int scissorCount, T* scissorRects) where T : unmanaged
    {
#if DEBUG
        if (sizeof(T) != sizeof(VkRect2D))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkRect2D)}", nameof(scissorRects));
        }
#endif

        vkCmdSetScissor(commandBuffer, firstScissor, (uint)scissorCount, (VkRect2D*)scissorRects);
    }

    public void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, float red, float green, float blue, float alpha)
    {
        float* blendConstantsArray = stackalloc float[] { red, green, blue, alpha };
        vkCmdSetBlendConstants(commandBuffer, blendConstantsArray);
    }

    public void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, ReadOnlySpan<float> blendConstants)
    {
        fixed (float* blendConstantsPtr = blendConstants)
        {
            vkCmdSetBlendConstants(commandBuffer, blendConstantsPtr);
        }
    }

    public void vkCmdSetFragmentShadingRateKHR(VkCommandBuffer commandBuffer, VkExtent2D* fragmentSize, VkFragmentShadingRateCombinerOpKHR[] combinerOps)
    {
        fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = &combinerOps[0])
        {
            vkCmdSetFragmentShadingRateKHR(commandBuffer, fragmentSize, combinerOpsPtr);
        }
    }

    public void vkCmdSetFragmentShadingRateKHR(VkCommandBuffer commandBuffer, VkExtent2D[] fragmentSize, VkFragmentShadingRateCombinerOpKHR[] combinerOps)
    {
        fixed (VkExtent2D* fragmentSizePtr = &fragmentSize[0])
        {
            fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = &combinerOps[0])
            {
                vkCmdSetFragmentShadingRateKHR(commandBuffer, fragmentSizePtr, combinerOpsPtr);
            }
        }
    }

    public void vkCmdSetFragmentShadingRateEnumNV(VkCommandBuffer commandBuffer, VkFragmentShadingRateNV shadingRate, VkFragmentShadingRateCombinerOpKHR[] combinerOps)
    {
        fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = &combinerOps[0])
        {
            vkCmdSetFragmentShadingRateEnumNV(commandBuffer, shadingRate, combinerOpsPtr);
        }
    }

    public void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer,
        VkBuffer srcBuffer,
        VkImage dstImage,
        VkImageLayout dstImageLayout,
        ReadOnlySpan<VkBufferImageCopy> regions,
        uint regionCount = 0)
    {
        if (regionCount == 0)
            regionCount = (uint)regions.Length;

        fixed (VkBufferImageCopy* pRegions = regions)
        {
            vkCmdCopyBufferToImage(commandBuffer, srcBuffer, dstImage, dstImageLayout,
                regionCount,
                pRegions
             );
        }
    }
}
