// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Vortice.Mathematics;

namespace DrawTriangle
{
    public static unsafe class Program
    {
#if DEBUG
        private static bool EnableValidationLayers = true;
#else
        private static bool EnableValidationLayers = false;
#endif
        public static void Main()
        {
            using var testApp = new TestApp();
            testApp.Run();
        }


        class TestApp : Application
        {
            private GraphicsDevice _graphicsDevice;

            public override string Name => "01-ClearScreen";

            protected override void Initialize()
            {
                // Need to initialize 
                vkInitialize().CheckResult();

                _graphicsDevice = new GraphicsDevice(Name, EnableValidationLayers, MainWindow);
            }

            public override void Dispose()
            {
                _graphicsDevice.Dispose();

                base.Dispose();
            }

            protected override void OnTick()
            {
                _graphicsDevice.RenderFrame(OnDraw);
            }

            private void OnDraw(VkCommandBuffer commandBuffer, VkFramebuffer framebuffer, Size size)
            {
                VkClearValue clearValue = new VkClearValue(new Color4(0.1f, 0.1f, 0.2f, 1.0f));

                // Begin the render pass.
                VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo
                {
                    sType = VkStructureType.RenderPassBeginInfo,
                    renderPass = _graphicsDevice.Swapchain.RenderPass,
                    framebuffer = framebuffer,
                    renderArea = new Rectangle(size.Width, size.Height),
                    clearValueCount = 1,
                    pClearValues = &clearValue
                };
                vkCmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);

                vkCmdEndRenderPass(commandBuffer);
            }
        }
    }
}
