using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Vortice;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangle
{
    public unsafe class TestApp : Application
    {

#if DEBUG
        private static bool EnableValidationLayers = true;
#else
		private static bool EnableValidationLayers = false;
#endif

        [NotNull]
        private GraphicsDevice _graphicsDevice = default!;
        private float _green = 0.0f;

        public override string Name => "01-ClearScreen";

        protected override void Initialize()
        {
            base.Initialize();
            // Need to initialize
            vkInitialize().CheckResult();

            _graphicsDevice = new GraphicsDevice(Name, EnableValidationLayers, MainWindow);
        }

        protected override void OnRenderFrame()
        {
            _graphicsDevice.RenderFrame(OnDraw);
        }

        private void OnDraw(VkCommandBuffer commandBuffer, VkFramebuffer framebuffer, VkExtent2D size)
        {
            float g = _green + 0.01f;
            if (g > 1.0f)
                g = 0.0f;
            _green = g;

            VkClearValue clearValue = new VkClearValue(1.0f, _green, 0.0f, 1.0f);

            // Begin the render pass.
            VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderPass = _graphicsDevice.Swapchain.RenderPass,
                framebuffer = framebuffer,
                renderArea = new VkRect2D(size),
                clearValueCount = 1,
                pClearValues = &clearValue
            };
            vkCmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);
            vkCmdSetBlendConstants(commandBuffer, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            vkCmdEndRenderPass(commandBuffer);
        }
    }
}