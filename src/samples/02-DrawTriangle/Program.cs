// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangle;

public static unsafe class Program
{
#if DEBUG
    private static bool EnableValidationLayers = true;
#else
	private static bool EnableValidationLayers = false;
#endif

    public static void Main()
    {
        using TestApp testApp = new TestApp();
        testApp.Run();
    }

    class TestApp : Application
    {
        private GraphicsDevice _graphicsDevice;
        public override string Name => "02-DrawTriangle";

        private VkPipelineLayout _pipelineLayout;
        private VkPipeline _pipeline;
        private VkBuffer _vertexBuffer;
        private VkDeviceMemory _vertexBufferMemory;

        protected override void Initialize()
        {
            _graphicsDevice = new GraphicsDevice(Name, EnableValidationLayers, MainWindow);

            VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo = new();
            _graphicsDevice.DeviceApi.vkCreatePipelineLayout(_graphicsDevice, &pipelineLayoutCreateInfo, null, out _pipelineLayout).CheckResult();

            // Create pipeline
            {
                VkUtf8ReadOnlyString entryPoint = "main"u8;

                CreateShaderModule("triangle.vert", out VkShaderModule vertexShader);
                CreateShaderModule("triangle.frag", out VkShaderModule fragmentShader);

                VkPipelineShaderStageCreateInfo* shaderStages = stackalloc VkPipelineShaderStageCreateInfo[2];
                // Vertex shader
                shaderStages[0] = new()
                {
                    stage = VkShaderStageFlags.Vertex,
                    module = vertexShader,
                    pName = entryPoint
                };

                // Fragment shader
                shaderStages[1] = new()
                {
                    stage = VkShaderStageFlags.Fragment,
                    module = fragmentShader,
                    pName = entryPoint
                };

                // VertexInputState
                VkVertexInputBindingDescription vertexInputBinding = new(VertexPositionColor.SizeInBytes);

                // Attribute location 0: Position
                // Attribute location 1: Color
                VkVertexInputAttributeDescription* vertexInputAttributs = stackalloc VkVertexInputAttributeDescription[2]
                {
                    new(0, VkFormat.R32G32B32Sfloat, 0),
                    new(1, VkFormat.R32G32B32A32Sfloat, 12)
                };

                VkPipelineVertexInputStateCreateInfo vertexInputState = new()
                {
                    vertexBindingDescriptionCount = 1,
                    pVertexBindingDescriptions = &vertexInputBinding,
                    vertexAttributeDescriptionCount = 2,
                    pVertexAttributeDescriptions = vertexInputAttributs
                };

                VkPipelineInputAssemblyStateCreateInfo inputAssemblyState = new(VkPrimitiveTopology.TriangleList);

                VkPipelineViewportStateCreateInfo viewportState = new(1, 1);

                // Rasterization state
                VkPipelineRasterizationStateCreateInfo rasterizationState = VkPipelineRasterizationStateCreateInfo.CullCounterClockwise;

                // Multi sampling state
                VkPipelineMultisampleStateCreateInfo multisampleState = VkPipelineMultisampleStateCreateInfo.Default;

                // DepthStencil
                VkPipelineDepthStencilStateCreateInfo depthStencilState = VkPipelineDepthStencilStateCreateInfo.Default;

                // BlendStates
                VkPipelineColorBlendAttachmentState blendAttachmentState = default;
                blendAttachmentState.colorWriteMask = VkColorComponentFlags.All;
                blendAttachmentState.blendEnable = false;

                VkPipelineColorBlendStateCreateInfo colorBlendState = new(blendAttachmentState);

                // Dynamic states
                VkDynamicState* dynamicStateEnables = stackalloc VkDynamicState[2];
                dynamicStateEnables[0] = VkDynamicState.Viewport;
                dynamicStateEnables[1] = VkDynamicState.Scissor;

                VkPipelineDynamicStateCreateInfo dynamicState = new()
                {
                    dynamicStateCount = 2,
                    pDynamicStates = dynamicStateEnables
                };

                VkGraphicsPipelineCreateInfo pipelineCreateInfo = new()
                {
                    stageCount = 2,
                    pStages = shaderStages,
                    pVertexInputState = &vertexInputState,
                    pInputAssemblyState = &inputAssemblyState,
                    pTessellationState = null,
                    pViewportState = &viewportState,
                    pRasterizationState = &rasterizationState,
                    pMultisampleState = &multisampleState,
                    pDepthStencilState = &depthStencilState,
                    pColorBlendState = &colorBlendState,
                    pDynamicState = &dynamicState,
                    layout = _pipelineLayout,
                    renderPass = _graphicsDevice.Swapchain.RenderPass
                };

                // Create rendering pipeline using the specified states
                _graphicsDevice.DeviceApi.vkCreateGraphicsPipeline(_graphicsDevice, pipelineCreateInfo, out _pipeline).CheckResult();

                _graphicsDevice.DeviceApi.vkDestroyShaderModule(_graphicsDevice, vertexShader);
                _graphicsDevice.DeviceApi.vkDestroyShaderModule(_graphicsDevice, fragmentShader);
            }

            {
                // Create vertex buffer
                ReadOnlySpan<VertexPositionColor> sourceData =
                [
                    new VertexPositionColor(new Vector3(0f, 0.5f, 0.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                    new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                    new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f))
                ];

                uint vertexBufferSize = (uint)(sourceData.Length * VertexPositionColor.SizeInBytes);

                VkBufferCreateInfo vertexBufferInfo = new()
                {
                    size = vertexBufferSize,
                    // Buffer is used as the copy source
                    usage = VkBufferUsageFlags.TransferSrc
                };

                // Create a host-visible buffer to copy the vertex data to (staging buffer)
                _graphicsDevice.DeviceApi.vkCreateBuffer(_graphicsDevice, &vertexBufferInfo, null, out VkBuffer stagingBuffer).CheckResult();

                _graphicsDevice.DeviceApi.vkGetBufferMemoryRequirements(_graphicsDevice, stagingBuffer, out VkMemoryRequirements memReqs);

                VkMemoryAllocateInfo memAlloc = new()
                {
                    allocationSize = memReqs.size,
                    // Request a host visible memory type that can be used to copy our data do
                    // Also request it to be coherent, so that writes are visible to the GPU right after unmapping the buffer
                    memoryTypeIndex = _graphicsDevice.GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent)
                };
                _graphicsDevice.DeviceApi.vkAllocateMemory(_graphicsDevice, &memAlloc, null, out VkDeviceMemory stagingBufferMemory);

                // Map and copy
                void* pMappedData;
                _graphicsDevice.DeviceApi.vkMapMemory(_graphicsDevice, stagingBufferMemory, 0, memAlloc.allocationSize, 0, &pMappedData).CheckResult();
                Span<VertexPositionColor> destinationData = new(pMappedData, sourceData.Length);
                sourceData.CopyTo(destinationData);
                _graphicsDevice.DeviceApi.vkUnmapMemory(_graphicsDevice, stagingBufferMemory);
                _graphicsDevice.DeviceApi.vkBindBufferMemory(_graphicsDevice, stagingBuffer, stagingBufferMemory, 0).CheckResult();

                vertexBufferInfo.usage = VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst;
                _graphicsDevice.DeviceApi.vkCreateBuffer(_graphicsDevice, &vertexBufferInfo, null, out _vertexBuffer).CheckResult();

                _graphicsDevice.DeviceApi.vkGetBufferMemoryRequirements(_graphicsDevice, _vertexBuffer, out memReqs);
                memAlloc.allocationSize = memReqs.size;
                memAlloc.memoryTypeIndex = _graphicsDevice.GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);
                _graphicsDevice.DeviceApi.vkAllocateMemory(_graphicsDevice, &memAlloc, null, out _vertexBufferMemory).CheckResult();
                _graphicsDevice.DeviceApi.vkBindBufferMemory(_graphicsDevice, _vertexBuffer, _vertexBufferMemory, 0).CheckResult();

                VkCommandBuffer copyCmd = _graphicsDevice.GetCommandBuffer(true);

                // Put buffer region copies into command buffer
                VkBufferCopy copyRegion = default;

                // Vertex buffer
                copyRegion.size = vertexBufferSize;
                _graphicsDevice.DeviceApi.vkCmdCopyBuffer(copyCmd, stagingBuffer, _vertexBuffer, 1, &copyRegion);

                // Flushing the command buffer will also submit it to the queue and uses a fence to ensure that all commands have been executed before returning
                _graphicsDevice.FlushCommandBuffer(copyCmd);

                _graphicsDevice.DeviceApi.vkDestroyBuffer(_graphicsDevice, stagingBuffer);
                _graphicsDevice.DeviceApi.vkFreeMemory(_graphicsDevice, stagingBufferMemory);
            }
        }

        public override void Dispose()
        {
            _graphicsDevice.WaitIdle();

            _graphicsDevice.DeviceApi.vkDestroyPipelineLayout(_graphicsDevice, _pipelineLayout);
            _graphicsDevice.DeviceApi.vkDestroyPipeline(_graphicsDevice, _pipeline);
            _graphicsDevice.DeviceApi.vkDestroyBuffer(_graphicsDevice, _vertexBuffer);
            _graphicsDevice.DeviceApi.vkFreeMemory(_graphicsDevice, _vertexBufferMemory);

            _graphicsDevice.Dispose();

            base.Dispose();
        }

        protected override void OnTick()
        {
            _graphicsDevice!.RenderFrame(OnDraw);
        }

        private void OnDraw(VkCommandBuffer commandBuffer, VkFramebuffer framebuffer, VkExtent2D size)
        {
            VkClearValue clearValue = new VkClearValue(0.0f, 0.0f, 0.2f, 1.0f);

            // Begin the render pass.
            VkRenderPassBeginInfo renderPassBeginInfo = new()
            {
                renderPass = _graphicsDevice.Swapchain.RenderPass,
                framebuffer = framebuffer,
                renderArea = new VkRect2D(VkOffset2D.Zero, size),
                clearValueCount = 1,
                pClearValues = &clearValue
            };

            _graphicsDevice.DeviceApi.vkCmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);

            // Update dynamic viewport state
            // Flip coordinate to map DirectX coordinate system.
            VkViewport viewport = new()
            {
                x = 0.0f,
                y = MainWindow.Extent.height,
                width = MainWindow.Extent.width,
                height = -MainWindow.Extent.height,
                minDepth = 0.0f,
                maxDepth = 1.0f
            };
            _graphicsDevice.DeviceApi.vkCmdSetViewport(commandBuffer, viewport);

            // Update dynamic scissor state
            VkRect2D scissor = new(VkOffset2D.Zero, MainWindow.Extent);
            _graphicsDevice.DeviceApi.vkCmdSetScissor(commandBuffer, scissor);

            // Bind the rendering pipeline
            _graphicsDevice.DeviceApi.vkCmdBindPipeline(commandBuffer, VkPipelineBindPoint.Graphics, _pipeline);

            // Bind triangle vertex buffer (contains position and colors)
            _graphicsDevice.DeviceApi.vkCmdBindVertexBuffer(commandBuffer, 0, _vertexBuffer);

            // Draw non indexed
            _graphicsDevice.DeviceApi.vkCmdDraw(commandBuffer, 3, 1, 0, 0);

            _graphicsDevice.DeviceApi.vkCmdEndRenderPass(commandBuffer);
        }

        private void CreateShaderModule(string name, out VkShaderModule shaderModule)
        {
            byte[] vertexBytecode = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", $"{name}.spv"));
            _graphicsDevice.CreateShaderModule(vertexBytecode, out shaderModule).CheckResult();
        }
    }
}
