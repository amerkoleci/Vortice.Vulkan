// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Numerics;
using Vortice;
using Vortice.ShaderCompiler;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

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
            string vertexShader = @"
#version 450

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inColor;

layout (binding = 0) uniform UBO 
{
	mat4 projectionMatrix;
	mat4 modelMatrix;
	mat4 viewMatrix;
} ubo;

layout (location = 0) out vec3 outColor;

out gl_PerVertex 
{
	vec4 gl_Position;   
};


void main() 
{
	outColor = inColor;
	gl_Position = ubo.projectionMatrix * ubo.viewMatrix * ubo.modelMatrix * vec4(inPos.xyz, 1.0);
}
";
            //using Compiler compiler = new Compiler();
            //using (var compilationResult = compiler.Compile(vertexShader, string.Empty, ShaderKind.VertexShader))
            //{
            //    //vkCreateShaderModule(VkDevice, compilationResult.GetBytecode(), null, out VkShaderModule module).CheckResult();
            //} 

            using var testApp = new TestApp();
			testApp.Run();
		}

		class TestApp : Application
		{
			private GraphicsDevice? _graphicsDevice;
			private float _green = 0.0f;

			public override string Name => "01-ClearScreen";

			protected override void Initialize()
			{
				// Need to initialize 
				vkInitialize().CheckResult();

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

				VkClearValue clearValue = new VkClearValue(1.0f, _green, 0.0f, 1.0f);

				// Begin the render pass.
				VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo
				{
					sType = VkStructureType.RenderPassBeginInfo,
					renderPass = _graphicsDevice!.Swapchain.RenderPass,
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
}
