// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangleVma;

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
        public override string Name => "03-DrawTriangleVma";

        private VkPipelineLayout _pipelineLayout;
        private VkPipeline _pipeline;
        private Buffer _vertexBuffer;
        private VmaAllocator _allocator;

        protected override void Initialize()
        {
            _graphicsDevice = new GraphicsDevice(Name, EnableValidationLayers, MainWindow);


            VmaAllocatorCreateInfo allocatorCreateInfo;
            allocatorCreateInfo.VulkanApiVersion = VkVersion.Version_1_2;
            allocatorCreateInfo.PhysicalDevice = _graphicsDevice.PhysicalDevice;
            allocatorCreateInfo.Device = _graphicsDevice.VkDevice;
            allocatorCreateInfo.Instance = _graphicsDevice.VkInstance;
            Vma.vmaCreateAllocator(&allocatorCreateInfo, out _allocator);

            VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo = new()
            {
                sType = VkStructureType.PipelineLayoutCreateInfo
            };
            vkCreatePipelineLayout(_graphicsDevice, &pipelineLayoutCreateInfo, null, out _pipelineLayout).CheckResult();

            // Create pipeline
            {
                VkString entryPoint = new("main");

                CreateShaderModule("triangle.vert", out VkShaderModule vertexShader);
                CreateShaderModule("triangle.frag", out VkShaderModule fragmentShader);

                VkPipelineShaderStageCreateInfo* shaderStages = stackalloc VkPipelineShaderStageCreateInfo[2];
                // Vertex shader
                shaderStages[0].sType = VkStructureType.PipelineShaderStageCreateInfo;
                shaderStages[0].stage = VkShaderStageFlags.Vertex;
                shaderStages[0].module = vertexShader;
                shaderStages[0].pName = entryPoint;

                // Fragment shader
                shaderStages[1].sType = VkStructureType.PipelineShaderStageCreateInfo;
                shaderStages[1].stage = VkShaderStageFlags.Fragment;
                shaderStages[1].module = fragmentShader;
                shaderStages[1].pName = entryPoint;

                // VertexInputState
                VkVertexInputBindingDescription vertexInputBinding = new(VertexPositionColor.SizeInBytes);

                // Attribute location 0: Position
                // Attribute location 1: Color
                VkVertexInputAttributeDescription* vertexInputAttributs = stackalloc VkVertexInputAttributeDescription[2]
                {
                    new(0, VkFormat.R32G32B32Sfloat, 0),
                    new(1, VkFormat.R32G32B32A32Sfloat, 12)
                };

                VkPipelineVertexInputStateCreateInfo vertexInputState = new();
                vertexInputState.sType = VkStructureType.PipelineVertexInputStateCreateInfo;
                vertexInputState.vertexBindingDescriptionCount = 1;
                vertexInputState.pVertexBindingDescriptions = &vertexInputBinding;
                vertexInputState.vertexAttributeDescriptionCount = 2;
                vertexInputState.pVertexAttributeDescriptions = vertexInputAttributs;

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
                    sType = VkStructureType.PipelineDynamicStateCreateInfo,
                    dynamicStateCount = 2,
                    pDynamicStates = dynamicStateEnables
                };

                VkGraphicsPipelineCreateInfo pipelineCreateInfo = new()
                {
                    sType = VkStructureType.GraphicsPipelineCreateInfo,
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
                vkCreateGraphicsPipeline(_graphicsDevice, pipelineCreateInfo, out _pipeline).CheckResult();

                vkDestroyShaderModule(_graphicsDevice, vertexShader);
                vkDestroyShaderModule(_graphicsDevice, fragmentShader);
            }

            {
                // Create vertex buffer
                ReadOnlySpan<VertexPositionColor> sourceData = new VertexPositionColor[]
                {
                    new VertexPositionColor(new Vector3(0f, 0.5f, 0.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                    new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                    new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f))
                };

                uint vertexBufferSize = (uint)(sourceData.Length * VertexPositionColor.SizeInBytes);

                VkBufferCreateInfo vertexBufferInfo = new()
                {
                    sType = VkStructureType.BufferCreateInfo,
                    size = vertexBufferSize,
                    // Buffer is used as the copy source
                    usage = VkBufferUsageFlags.TransferSrc
                };

                // Create a host-visible buffer to copy the vertex data to (staging buffer)
                var stagingBuffer = new Buffer(_allocator, vertexBufferSize,
                    VkBufferUsageFlags.TransferSrc, true);

                // Map and copy
                void* pMappedData;
                stagingBuffer.Map(_allocator, &pMappedData);
                Span<VertexPositionColor> destinationData = new(pMappedData, sourceData.Length);
                sourceData.CopyTo(destinationData);
                stagingBuffer.Unmap(_allocator);

                _vertexBuffer = new Buffer(_allocator, vertexBufferSize,
                    VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer, false);

                VkCommandBuffer copyCmd = _graphicsDevice.GetCommandBuffer(true);

                // Put buffer region copies into command buffer
                VkBufferCopy copyRegion = default;

                // Vertex buffer
                copyRegion.size = vertexBufferSize;
                vkCmdCopyBuffer(copyCmd, stagingBuffer.VkBuffer, _vertexBuffer.VkBuffer, 1, &copyRegion);

                // Flushing the command buffer will also submit it to the queue and uses a fence to ensure that all commands have been executed before returning
                _graphicsDevice.FlushCommandBuffer(copyCmd);

                stagingBuffer.Destroy(_allocator);
            }
        }


        public override void Dispose()
        {
            _graphicsDevice.WaitIdle();

            vkDestroyPipelineLayout(_graphicsDevice, _pipelineLayout);
            vkDestroyPipeline(_graphicsDevice, _pipeline);
            _vertexBuffer.Destroy(_allocator);
            Vma.vmaDestroyAllocator(_allocator);

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
                sType = VkStructureType.RenderPassBeginInfo,
                renderPass = _graphicsDevice.Swapchain.RenderPass,
                framebuffer = framebuffer,
                renderArea = new VkRect2D(size),
                clearValueCount = 1,
                pClearValues = &clearValue
            };

            vkCmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);

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
            vkCmdSetViewport(commandBuffer, viewport);

            // Update dynamic scissor state
            VkRect2D scissor = new(MainWindow.Extent);
            vkCmdSetScissor(commandBuffer, scissor);

            // Bind the rendering pipeline
            vkCmdBindPipeline(commandBuffer, VkPipelineBindPoint.Graphics, _pipeline);

            // Bind triangle vertex buffer (contains position and colors)
            vkCmdBindVertexBuffer(commandBuffer, 0, _vertexBuffer.VkBuffer);

            // Draw non indexed
            vkCmdDraw(commandBuffer, 3, 1, 0, 0);

            vkCmdEndRenderPass(commandBuffer);
        }

        private void CreateShaderModule(string name, out VkShaderModule shaderModule)
        {
            byte[] vertexBytecode = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", $"{name}.spv"));
            _graphicsDevice.CreateShaderModule(vertexBytecode, out shaderModule).CheckResult();
        }
    }
}
