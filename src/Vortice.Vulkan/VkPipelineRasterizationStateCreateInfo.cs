// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created pipeline rasterization state
/// </summary>
public unsafe partial struct VkPipelineRasterizationStateCreateInfo
{
    /// <summary>
    /// A built-in description with settings with settings for not culling any primitives.
    /// </summary>
    public static VkPipelineRasterizationStateCreateInfo CullNone => new(VkCullModeFlags.None);

    /// <summary>
    /// A built-in description with settings for culling primitives with clockwise winding order.
    /// </summary>
    public static VkPipelineRasterizationStateCreateInfo CullClockwise => new(VkCullModeFlags.Front);

    /// <summary>
    /// A built-in description with settings for culling primitives with counter-clockwise winding order.
    /// </summary>
    public static VkPipelineRasterizationStateCreateInfo CullCounterClockwise => new(VkCullModeFlags.Back);

    /// <summary>
    /// A built-in description with settings for not culling any primitives and wireframe fill mode.
    /// </summary>
    public static VkPipelineRasterizationStateCreateInfo Wireframe => new(VkCullModeFlags.Back, VkPolygonMode.Line);

    public VkPipelineRasterizationStateCreateInfo(
        VkCullModeFlags cullMode,
        VkPolygonMode polygonMode = VkPolygonMode.Fill,
        VkFrontFace frontFace = VkFrontFace.Clockwise,
        bool depthClampEnable = false,
        bool rasterizerDiscardEnable = false,
        bool depthBiasEnable = false,
        float depthBiasConstantFactor = 0.0f,
        float depthBiasClamp = 0.0f,
        float depthBiasSlopeFactor = 0.0f,
        float lineWidth = 1.0f,
        void* pNext = default,
        VkPipelineRasterizationStateCreateFlags flags = VkPipelineRasterizationStateCreateFlags.None)
    {
        this.sType = VkStructureType.PipelineRasterizationStateCreateInfo;
        this.pNext = pNext;
        this.flags = flags;
        this.depthClampEnable = depthClampEnable;
        this.rasterizerDiscardEnable = rasterizerDiscardEnable;
        this.polygonMode = polygonMode;
        this.cullMode = cullMode;
        this.frontFace = frontFace;
        this.depthBiasEnable = depthBiasEnable;
        this.depthBiasConstantFactor = depthBiasConstantFactor;
        this.depthBiasClamp = depthBiasClamp;
        this.depthBiasSlopeFactor = depthBiasSlopeFactor;
        this.lineWidth = lineWidth;
    }
}
