using System;
using Vortice.Mathematics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice
{
    public unsafe sealed class Swapchain : IDisposable
    {
        public readonly GraphicsDevice Device;
        public readonly Window Window;
        private Size _extent;
        private VkImageView[] _swapChainImageViews;
        private VkFramebuffer[] _framebuffers;

        public VkSwapchainKHR Handle;
        public int ImageCount => _swapChainImageViews.Length;
        public VkRenderPass RenderPass;
        public Size Extent => _extent;
        public VkFramebuffer[] Framebuffers => _framebuffers;

        public Swapchain(GraphicsDevice device, Window window)
        {
            Device = device;
            Window = window;

            SwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(device.PhysicalDevice, device._surface);

            VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
            VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.PresentModes);
            _extent = ChooseSwapExtent(swapChainSupport.Capabilities);

            CreateRenderPass(surfaceFormat.format);

            uint imageCount = swapChainSupport.Capabilities.minImageCount + 1;
            if (swapChainSupport.Capabilities.maxImageCount > 0 &&
                imageCount > swapChainSupport.Capabilities.maxImageCount)
            {
                imageCount = swapChainSupport.Capabilities.maxImageCount;
            }

            var createInfo = new VkSwapchainCreateInfoKHR
            {
                sType = VkStructureType.SwapchainCreateInfoKHR,
                surface = device._surface,
                minImageCount = imageCount,
                imageFormat = surfaceFormat.format,
                imageColorSpace = surfaceFormat.colorSpace,
                imageExtent = _extent,
                imageArrayLayers = 1,
                imageUsage = VkImageUsageFlags.ColorAttachment,
                imageSharingMode = VkSharingMode.Exclusive,
                preTransform = swapChainSupport.Capabilities.currentTransform,
                compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
                presentMode = presentMode,
                clipped = true,
                oldSwapchain = VkSwapchainKHR.Null
            };

            vkCreateSwapchainKHR(device.VkDevice, &createInfo, null, out Handle).CheckResult();
            var swapChainImages = vkGetSwapchainImagesKHR(device.VkDevice, Handle);
            _swapChainImageViews = new VkImageView[swapChainImages.Length];
            _framebuffers = new VkFramebuffer[swapChainImages.Length];

            for (var i = 0; i < swapChainImages.Length; i++)
            {
                var viewCreateInfo = new VkImageViewCreateInfo(
                    swapChainImages[i],
                    VkImageViewType.Image2D,
                    surfaceFormat.format,
                    VkComponentMapping.Identity,
                    new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1)
                    );

                vkCreateImageView(Device.VkDevice, &viewCreateInfo, null, out _swapChainImageViews[i]).CheckResult();
                vkCreateFramebuffer(Device.VkDevice, RenderPass, new[] { _swapChainImageViews[i] }, (uint)_extent.Width, (uint)_extent.Height, 1u, out _framebuffers[i]);
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < _swapChainImageViews.Length; i++)
            {
                vkDestroyImageView(Device, _swapChainImageViews[i], null);
            }

            for (var i = 0; i < _framebuffers.Length; i++)
            {
                vkDestroyFramebuffer(Device, _framebuffers[i], null);
            }

            vkDestroyRenderPass(Device, RenderPass, null);

            if (Handle != VkSwapchainKHR.Null)
            {
                vkDestroySwapchainKHR(Device, Handle, null);
            }
        }

        private void CreateRenderPass(VkFormat colorFormat)
        {
            VkAttachmentDescription attachment = new VkAttachmentDescription(
                colorFormat,
                VkSampleCountFlags.Count1,
                VkAttachmentLoadOp.Clear, VkAttachmentStoreOp.Store,
                VkAttachmentLoadOp.DontCare, VkAttachmentStoreOp.DontCare,
                VkImageLayout.Undefined, VkImageLayout.PresentSrcKHR
            );

            VkAttachmentReference colorAttachmentRef = new VkAttachmentReference(0, VkImageLayout.ColorAttachmentOptimal);

            VkSubpassDescription subpass = new VkSubpassDescription
            {
                pipelineBindPoint = VkPipelineBindPoint.Graphics,
                colorAttachmentCount = 1,
                pColorAttachments = &colorAttachmentRef
            };

            VkSubpassDependency dependency = new VkSubpassDependency
            {
                srcSubpass = SubpassExternal,
                dstSubpass = 0,
                srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput,

                // Since we changed the image layout, we need to make the memory visible to
                // color attachment to modify.
                srcAccessMask = 0,
                dstAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite
            };

            VkRenderPassCreateInfo createInfo = new VkRenderPassCreateInfo
            {
                sType = VkStructureType.RenderPassCreateInfo,
                attachmentCount = 1,
                pAttachments = &attachment,
                subpassCount = 1,
                pSubpasses = &subpass,
                dependencyCount = 1,
                pDependencies = &dependency
            };

            vkCreateRenderPass(Device, &createInfo, null, out RenderPass).CheckResult();
        }

        private ref struct SwapChainSupportDetails
        {
            public VkSurfaceCapabilitiesKHR Capabilities;
            public ReadOnlySpan<VkSurfaceFormatKHR> Formats;
            public ReadOnlySpan<VkPresentModeKHR> PresentModes;
        };

        private Size ChooseSwapExtent(in VkSurfaceCapabilitiesKHR capabilities)
        {
            if (capabilities.currentExtent.Width > 0)
            {
                return capabilities.currentExtent;
            }
            else
            {
                Size actualExtent = new Size(Window.Width, Window.Height);

                actualExtent.Width = Math.Max(capabilities.minImageExtent.Width, Math.Min(capabilities.maxImageExtent.Width, actualExtent.Width));
                actualExtent.Height = Math.Max(capabilities.minImageExtent.Height, Math.Min(capabilities.maxImageExtent.Height, actualExtent.Height));

                return actualExtent;
            }
        }

        private static SwapChainSupportDetails QuerySwapChainSupport(VkPhysicalDevice device, VkSurfaceKHR surface)
        {
            SwapChainSupportDetails details = new SwapChainSupportDetails();
            vkGetPhysicalDeviceSurfaceCapabilitiesKHR(device, surface, out details.Capabilities).CheckResult();

            details.Formats = vkGetPhysicalDeviceSurfaceFormatsKHR(device, surface);
            details.PresentModes = vkGetPhysicalDeviceSurfacePresentModesKHR(device, surface);
            return details;
        }

        private static VkSurfaceFormatKHR ChooseSwapSurfaceFormat(ReadOnlySpan<VkSurfaceFormatKHR> availableFormats)
        {
            foreach (var availableFormat in availableFormats)
            {
                if (availableFormat.format == VkFormat.B8G8R8A8SRgb &&
                    availableFormat.colorSpace == VkColorSpaceKHR.SrgbNonlinear)
                {
                    return availableFormat;
                }
            }

            return availableFormats[0];
        }

        private static VkPresentModeKHR ChooseSwapPresentMode(ReadOnlySpan<VkPresentModeKHR> availablePresentModes)
        {
            foreach (var availablePresentMode in availablePresentModes)
            {
                if (availablePresentMode == VkPresentModeKHR.Mailbox)
                {
                    return availablePresentMode;
                }
            }

            return VkPresentModeKHR.Fifo;
        }
    }
}
