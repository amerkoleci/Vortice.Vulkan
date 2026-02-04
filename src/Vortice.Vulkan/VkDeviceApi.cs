// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

public unsafe partial class VkDeviceApi
{
    public VkResult vkAllocateCommandBuffer(VkCommandBufferAllocateInfo* allocateInfo, out VkCommandBuffer commandBuffer)
    {
        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(allocateInfo, ptr);
        }
    }

    public VkResult vkAllocateCommandBuffer(VkCommandPool commandPool, out VkCommandBuffer commandBuffer)
    {
        VkCommandBufferAllocateInfo allocateInfo = new()
        {
            commandPool = commandPool,
            level = VkCommandBufferLevel.Primary,
            commandBufferCount = 1
        };

        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(&allocateInfo, ptr);
        }
    }

    public VkResult vkAllocateCommandBuffer(VkCommandPool commandPool, VkCommandBufferLevel level, out VkCommandBuffer commandBuffer)
    {
        VkCommandBufferAllocateInfo allocateInfo = new()
        {
            commandPool = commandPool,
            level = level,
            commandBufferCount = 1
        };

        fixed (VkCommandBuffer* ptr = &commandBuffer)
        {
            return vkAllocateCommandBuffers(&allocateInfo, ptr);
        }
    }

    public VkResult vkCreateShaderModule(nuint codeSize, byte* code, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        VkShaderModuleCreateInfo createInfo = new()
        {
            codeSize = codeSize,
            pCode = (uint*)code
        };

        return vkCreateShaderModule(&createInfo, allocator, out shaderModule);
    }

    public VkResult vkCreateShaderModule(Span<byte> bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                codeSize = (nuint)bytecode.Length,
                pCode = (uint*)bytecodePtr
            };

            return vkCreateShaderModule(&createInfo, allocator, out shaderModule);
        }
    }

    public VkResult vkCreateShaderModule(ReadOnlySpan<byte> bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
    {
        fixed (byte* bytecodePtr = bytecode)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                codeSize = (nuint)bytecode.Length,
                pCode = (uint*)bytecodePtr
            };

            return vkCreateShaderModule(&createInfo, allocator, out shaderModule);
        }
    }

    public VkResult vkCreateGraphicsPipeline(VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateGraphicsPipelines(VkPipelineCache.Null, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateGraphicsPipeline(VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateGraphicsPipelines(pipelineCache, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateGraphicsPipeline(VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, VkPipeline* pipeline)
    {
        return vkCreateGraphicsPipelines(pipelineCache, 1, &createInfo, null, pipeline);
    }

    public VkResult vkCreateComputePipeline(VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateComputePipelines(VkPipelineCache.Null, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateComputePipeline(VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
    {
        fixed (VkPipeline* pipelinePtr = &pipeline)
        {
            return vkCreateComputePipelines(pipelineCache, 1, &createInfo, null, pipelinePtr);
        }
    }

    public VkResult vkCreateComputePipelines(VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, VkPipeline* pipeline)
    {
        return vkCreateComputePipelines(pipelineCache, 1, &createInfo, null, pipeline);
    }

    public VkDescriptorPool vkCreateDescriptorPool(Span<VkDescriptorPoolSize> poolSizes, uint maxSets = 1u)
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
            vkCreateDescriptorPool(&createInfo, null, &descriptorPool).CheckResult();
            return descriptorPool;
        }
    }

    public VkResult vkCreateDescriptorPool(Span<VkDescriptorPoolSize> poolSizes, uint maxSets, out VkDescriptorPool descriptorPool)
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

                return vkCreateDescriptorPool(&createInfo, null, descriptorPoolPtr);
            }
        }
    }

    public void vkUpdateDescriptorSets(VkWriteDescriptorSet writeDescriptorSet)
    {
        vkUpdateDescriptorSets(1, &writeDescriptorSet, 0, null);
    }

    public void vkUpdateDescriptorSets(VkWriteDescriptorSet writeDescriptorSet, VkCopyDescriptorSet copyDescriptorSet)
    {
        vkUpdateDescriptorSets(1, &writeDescriptorSet, 1, &copyDescriptorSet);
    }

    public void vkUpdateDescriptorSets(Span<VkWriteDescriptorSet> writeDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            vkUpdateDescriptorSets((uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, 0, null);
        }
    }

    public void vkUpdateDescriptorSets(Span<VkWriteDescriptorSet> writeDescriptorSets, Span<VkCopyDescriptorSet> copyDescriptorSets)
    {
        fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
        {
            fixed (VkCopyDescriptorSet* copyDescriptorSetsPtr = copyDescriptorSets)
            {
                vkUpdateDescriptorSets((uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, (uint)copyDescriptorSets.Length, copyDescriptorSetsPtr);
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

    public void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, Span<VkDescriptorSet> descriptorSets)
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
        Span<VkDescriptorSet> descriptorSets,
        Span<uint> dynamicOffsets)
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

    public void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, Span<VkBuffer> buffers, Span<ulong> offsets)
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

    public void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, Span<VkCommandBuffer> secondaryCommandBuffers)
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

    public VkResult vkCreateCommandPool(uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new()
        {
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(&createInfo, null, out commandPool);
    }

    public VkResult vkCreateCommandPool(VkCommandPoolCreateFlags flags, uint queueFamilyIndex, out VkCommandPool commandPool)
    {
        VkCommandPoolCreateInfo createInfo = new()
        {
            flags = flags,
            queueFamilyIndex = queueFamilyIndex
        };
        return vkCreateCommandPool(&createInfo, null, out commandPool);
    }

    public void vkFreeCommandBuffers(VkCommandPool commandPool, VkCommandBuffer commandBuffer)
    {
        vkFreeCommandBuffers(commandPool, 1, &commandBuffer);
    }

    public VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferUsageFlags flags)
    {
        VkCommandBufferBeginInfo beginInfo = new()
        {
            flags = flags
        };

        return vkBeginCommandBuffer(commandBuffer, &beginInfo);
    }

    public VkResult vkCreateSemaphore(out VkSemaphore semaphore)
    {
        VkSemaphoreCreateInfo createInfo = new();
        return vkCreateSemaphore(&createInfo, null, out semaphore);
    }

    public VkResult vkCreateFence(out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new();
        return vkCreateFence(&createInfo, null, out fence);
    }

    public VkResult vkCreateFence(VkFenceCreateFlags flags, out VkFence fence)
    {
        VkFenceCreateInfo createInfo = new(flags);
        return vkCreateFence(&createInfo, null, out fence);
    }

    public VkResult vkCreateTypedSemaphore(VkSemaphoreType type, ulong initialValue, out VkSemaphore semaphore)
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

        return vkCreateSemaphore(&createInfo, null, out semaphore);
    }

    public VkResult vkCreateFramebuffer(
        VkRenderPass renderPass,
        Span<VkImageView> attachments,
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

            return vkCreateFramebuffer(&createInfo, null, out framebuffer);
        }
    }

    public VkResult vkCreateFramebuffer(
        VkRenderPass renderPass,
        Span<VkImageView> attachments,
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

            return vkCreateFramebuffer(&createInfo, null, out framebuffer);
        }
    }

    public VkResult vkCreatePipelineLayout(
        Span<VkDescriptorSetLayout> setLayouts,
        Span<VkPushConstantRange> pushConstantRanges,
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

                return vkCreatePipelineLayout(&createInfo, null, out pipelineLayout);
            }
        }
    }

    public void vkCmdSetViewport(VkCommandBuffer commandBuffer, float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
    {
        VkViewport viewport = new(x, y, width, height, minDepth, maxDepth);
        vkCmdSetViewport(commandBuffer, 0, 1, &viewport);
    }

    public void vkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, Span<VkViewport> viewports)
    {
        fixed (VkViewport* viewportsPtr = viewports)
            vkCmdSetViewport(commandBuffer, firstViewport, viewportCount, viewportsPtr);
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

    public void vkCmdSetViewport<TViewport>(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, Span<TViewport> viewports)
        where TViewport : unmanaged
    {
#if DEBUG
        if (sizeof(TViewport) != sizeof(VkViewport))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkViewport)}", nameof(viewports));
        }
#endif

        fixed (TViewport* viewportsPtr = viewports)
            vkCmdSetViewport(commandBuffer, firstViewport, viewportCount, (VkViewport*)viewportsPtr);
    }

    public void vkCmdSetScissor(VkCommandBuffer commandBuffer, int x, int y, uint width, uint height)
    {
        VkRect2D scissor = new(x, y, width, height);
        vkCmdSetScissor(commandBuffer, 0, 1, &scissor);
    }

    public void vkCmdSetScissor<TRect2D>(VkCommandBuffer commandBuffer, uint firstScissor, TRect2D scissor)
        where TRect2D : unmanaged
    {
#if DEBUG
        if (sizeof(TRect2D) != sizeof(VkRect2D))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkRect2D)}", nameof(scissor));
        }
#endif
        vkCmdSetScissor(commandBuffer, firstScissor, 1, (VkRect2D*)&scissor);
    }

    public void vkCmdSetScissor<TRect2D>(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, Span<TRect2D> scissorRects)
        where TRect2D : unmanaged
    {
#if DEBUG
        if (sizeof(TRect2D) != sizeof(VkRect2D))
        {
            throw new ArgumentException($"Type T must have same size and layout as {nameof(VkRect2D)}", nameof(scissorRects));
        }
#endif

        fixed (TRect2D* scissorRectsPtr = scissorRects)
            vkCmdSetScissor(commandBuffer, firstScissor, scissorCount, (VkRect2D*)scissorRectsPtr);
    }

    public void vkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, Span<VkRect2D> scissorRects)
    {
        fixed (VkRect2D* scissorRectsPtr = scissorRects)
            vkCmdSetScissor(commandBuffer, firstScissor, scissorCount, scissorRectsPtr);
    }

    public void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, float red, float green, float blue, float alpha)
    {
        float* blendConstantsArray = stackalloc float[] { red, green, blue, alpha };
        vkCmdSetBlendConstants(commandBuffer, blendConstantsArray);
    }

    public void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, Span<float> blendConstants)
    {
        fixed (float* blendConstantsPtr = blendConstants)
        {
            vkCmdSetBlendConstants(commandBuffer, blendConstantsPtr);
        }
    }

    public void vkCmdSetFragmentShadingRateKHR(VkCommandBuffer commandBuffer, Span<VkExtent2D> fragmentSize, Span<VkFragmentShadingRateCombinerOpKHR> combinerOps)
    {
        fixed (VkExtent2D* fragmentSizePtr = fragmentSize)
        {
            fixed (VkFragmentShadingRateCombinerOpKHR* combinerOpsPtr = combinerOps)
            {
                vkCmdSetFragmentShadingRateKHR(commandBuffer, fragmentSizePtr, combinerOpsPtr);
            }
        }
    }

    public void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer,
        VkBuffer srcBuffer,
        VkImage dstImage,
        VkImageLayout dstImageLayout,
        Span<VkBufferImageCopy> regions,
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

    public void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer,
        VkPipelineStageFlags srcStageMask,
        VkPipelineStageFlags dstStageMask,
        VkDependencyFlags dependencyFlags,
        Span<VkMemoryBarrier> memoryBarriers,
        Span<VkBufferMemoryBarrier> bufferMemoryBarriers,
        Span<VkImageMemoryBarrier> imageMemoryBarriers)
    {
        fixed (VkMemoryBarrier* memoryBarriersPtr = memoryBarriers)
        {
            fixed (VkBufferMemoryBarrier* bufferMemoryBarriersPtr = bufferMemoryBarriers)
            {
                fixed (VkImageMemoryBarrier* imageMemoryBarriersPtr = imageMemoryBarriers)
                {
                    vkCmdPipelineBarrier(
                        commandBuffer,
                        srcStageMask,
                        dstStageMask,
                        dependencyFlags,
                        (uint)memoryBarriers.Length, memoryBarriersPtr,
                        (uint)bufferMemoryBarriers.Length, bufferMemoryBarriersPtr,
                        (uint)imageMemoryBarriers.Length, imageMemoryBarriersPtr
                        );
                }
            }
        }
    }

    public void vkCmdPipelineBarrier2(VkCommandBuffer commandBuffer,
        VkDependencyFlags dependencyFlags,
        Span<VkMemoryBarrier2> memoryBarriers,
        Span<VkBufferMemoryBarrier2> bufferMemoryBarriers,
        Span<VkImageMemoryBarrier2> imageMemoryBarriers)
    {
        fixed (VkMemoryBarrier2* pMemoryBarriers = memoryBarriers)
        {
            fixed (VkBufferMemoryBarrier2* pBufferMemoryBarriers = bufferMemoryBarriers)
            {
                fixed (VkImageMemoryBarrier2* pImageMemoryBarriers = imageMemoryBarriers)
                {
                    VkDependencyInfo dependencyInfo = new();
                    dependencyInfo.dependencyFlags = dependencyFlags;
                    dependencyInfo.memoryBarrierCount = (uint)memoryBarriers.Length;
                    dependencyInfo.pMemoryBarriers = pMemoryBarriers;
                    dependencyInfo.bufferMemoryBarrierCount = (uint)bufferMemoryBarriers.Length;
                    dependencyInfo.pBufferMemoryBarriers = pBufferMemoryBarriers;
                    dependencyInfo.imageMemoryBarrierCount = (uint)imageMemoryBarriers.Length;
                    dependencyInfo.pImageMemoryBarriers = pImageMemoryBarriers;
                    vkCmdPipelineBarrier2(commandBuffer, &dependencyInfo);
                }
            }
        }
    }
}
