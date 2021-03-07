using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice
{
    public sealed unsafe class Swapchain : IDisposable
    {
        public readonly GraphicsDevice Device;
        public readonly Window? Window;
        private readonly VkImageView[] _swapChainImageViews;
        public VkSwapchainKHR Handle;
        public int ImageCount => _swapChainImageViews.Length;
        public VkRenderPass RenderPass;
        public VkExtent2D Extent { get; }
        public VkFramebuffer[] Framebuffers { get; }

        public Swapchain(GraphicsDevice device, Window? window)
        {
            Device = device;
            Window = window;

            SwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(device.PhysicalDevice, device._surface);

            VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
            VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.PresentModes);
            Extent = ChooseSwapExtent(swapChainSupport.Capabilities);

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
                imageExtent = Extent,
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
            ReadOnlySpan<VkImage> swapChainImages = vkGetSwapchainImagesKHR(device.VkDevice, Handle);
            _swapChainImageViews = new VkImageView[swapChainImages.Length];
            Framebuffers = new VkFramebuffer[swapChainImages.Length];

            for (int i = 0; i < swapChainImages.Length; i++)
            {
                var viewCreateInfo = new VkImageViewCreateInfo(
                    swapChainImages[i],
                    VkImageViewType.Image2D,
                    surfaceFormat.format,
                    VkComponentMapping.Rgba,
                    new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1)
                    );

                vkCreateImageView(Device.VkDevice, &viewCreateInfo, null, out _swapChainImageViews[i]).CheckResult();
                vkCreateFramebuffer(Device.VkDevice, RenderPass, new[] { _swapChainImageViews[i] }, Extent, 1u, out Framebuffers[i]);
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < _swapChainImageViews.Length; i++)
            {
                vkDestroyImageView(Device, _swapChainImageViews[i], null);
            }

            for (int i = 0; i < Framebuffers.Length; i++)
            {
                vkDestroyFramebuffer(Device, Framebuffers[i], null);
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

            VkSubpassDependency[] dependencies = new VkSubpassDependency[2];

            dependencies[0] = new VkSubpassDependency
            {
                srcSubpass = SubpassExternal,
                dstSubpass = 0,
                srcStageMask = VkPipelineStageFlags.BottomOfPipe,
                dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                srcAccessMask = VkAccessFlags.MemoryRead,
                dstAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite,
                dependencyFlags = VkDependencyFlags.ByRegion
            };

            dependencies[1] = new VkSubpassDependency
            {
                srcSubpass = 0,
                dstSubpass = SubpassExternal,
                srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
                dstStageMask = VkPipelineStageFlags.BottomOfPipe,
                srcAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite,
                dstAccessMask = VkAccessFlags.MemoryRead,
                dependencyFlags = VkDependencyFlags.ByRegion
            };

            fixed (VkSubpassDependency* dependenciesPtr = &dependencies[0])
            {
                VkRenderPassCreateInfo createInfo = new VkRenderPassCreateInfo
                {
                    sType = VkStructureType.RenderPassCreateInfo,
                    attachmentCount = 1,
                    pAttachments = &attachment,
                    subpassCount = 1,
                    pSubpasses = &subpass,
                    dependencyCount = 2,
                    pDependencies = dependenciesPtr
                };

                vkCreateRenderPass(Device, &createInfo, null, out RenderPass).CheckResult();
            }
        }

        private ref struct SwapChainSupportDetails
        {
            public VkSurfaceCapabilitiesKHR Capabilities;
            public ReadOnlySpan<VkSurfaceFormatKHR> Formats;
            public ReadOnlySpan<VkPresentModeKHR> PresentModes;
        };

        private VkExtent2D ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities)
        {
            if (capabilities.currentExtent.width > 0)
            {
                return capabilities.currentExtent;
            }
            else
            {
                VkExtent2D actualExtent = Window!.Extent;

                actualExtent = new VkExtent2D(
                    Math.Max(capabilities.minImageExtent.width, Math.Min(capabilities.maxImageExtent.width, actualExtent.width)),
                    Math.Max(capabilities.minImageExtent.height, Math.Min(capabilities.maxImageExtent.height, actualExtent.height))
                    );

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
            // If the surface format list only includes one entry with VK_FORMAT_UNDEFINED,
            // there is no preferred format, so we assume VK_FORMAT_B8G8R8A8_UNORM
            if ((availableFormats.Length == 1) && (availableFormats[0].format == VkFormat.Undefined))
            {
                return new VkSurfaceFormatKHR(VkFormat.B8G8R8A8UNorm, availableFormats[0].colorSpace);
            }

            // iterate over the list of available surface format and
            // check for the presence of VK_FORMAT_B8G8R8A8_UNORM
            foreach (VkSurfaceFormatKHR availableFormat in availableFormats)
            {
                if (availableFormat.format == VkFormat.B8G8R8A8UNorm)
                {
                    return availableFormat;
                }
            }

            return availableFormats[0];
        }

        private static VkPresentModeKHR ChooseSwapPresentMode(ReadOnlySpan<VkPresentModeKHR> availablePresentModes)
        {
            foreach (VkPresentModeKHR availablePresentMode in availablePresentModes)
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
