// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;

namespace ClearScreen;

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
        private GraphicsDevice? _graphicsDevice;
        private float _green = 0.0f;
        public override string Name => "01-ClearScreen";

        protected override void Initialize()
        {
            _graphicsDevice = new GraphicsDevice(Name, EnableValidationLayers, MainWindow);
        }

        public override void Dispose()
        {
            _graphicsDevice!.Dispose();

            base.Dispose();
        }

        protected override void OnTick()
        {
            _graphicsDevice!.RenderFrame(OnDraw);
        }

        private void OnDraw(VkCommandBuffer commandBuffer, VkFramebuffer framebuffer, VkExtent2D size)
        {
            float g = _green + 0.001f;
            if (g > 1.0f)
                g = 0.0f;
            _green = g;

            VkClearValue clearValue = new(1.0f, _green, 0.0f, 1.0f);

            // Begin the render pass.
            VkRenderPassBeginInfo renderPassBeginInfo = new()
            {
                renderPass = _graphicsDevice!.Swapchain.RenderPass,
                framebuffer = framebuffer,
                renderArea = new VkRect2D(VkOffset2D.Zero, size),
                clearValueCount = 1,
                pClearValues = &clearValue
            };
            _graphicsDevice.DeviceApi.vkCmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);
            _graphicsDevice.DeviceApi.vkCmdSetBlendConstants(commandBuffer, 1.0f, 1.0f, 1.0f, 1.0f);
            _graphicsDevice.DeviceApi.vkCmdEndRenderPass(commandBuffer);
        }
    }
}
