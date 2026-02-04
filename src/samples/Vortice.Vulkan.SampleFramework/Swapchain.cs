// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public sealed unsafe class Swapchain : IDisposable
{
    public readonly GraphicsDevice Device;
    public readonly Window Window;
    private readonly VkImage[] _swapChainImages;
    private readonly VkImageView[] _swapChainImageViews;
    private readonly VkSurfaceKHR _surface;

    public VkSwapchainKHR Handle;
    public int ImageCount => _swapChainImageViews.Length;
    public VkFormat Format { get; }
    public VkExtent2D Extent { get; }
    public VkImage[] Images => _swapChainImages;
    public VkImageView[] ImageViews => _swapChainImageViews;

    public Swapchain(GraphicsDevice device, VkSurfaceKHR surface, Window window)
    {
        Device = device;
        _surface = surface;
        Window = window;

        SwapChainSupportDetails swapChainSupport = Utils.QuerySwapChainSupport(device.InstanceApi, device.PhysicalDevice, surface);

        VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
        VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.PresentModes);
        Extent = ChooseSwapExtent(swapChainSupport.Capabilities);

        uint imageCount = swapChainSupport.Capabilities.minImageCount + 1;
        if (swapChainSupport.Capabilities.maxImageCount > 0 &&
            imageCount > swapChainSupport.Capabilities.maxImageCount)
        {
            imageCount = swapChainSupport.Capabilities.maxImageCount;
        }

        VkSwapchainCreateInfoKHR createInfo = new()
        {
            surface = surface,
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

        device.DeviceApi.vkCreateSwapchainKHR(&createInfo, null, out Handle).CheckResult();

        device.DeviceApi.vkGetSwapchainImagesKHR(Handle, out uint swapChainImageCount).CheckResult();
        Span<VkImage> swapChainImages = stackalloc VkImage[(int)swapChainImageCount];
        device.DeviceApi.vkGetSwapchainImagesKHR(Handle, swapChainImages).CheckResult();
        _swapChainImages = swapChainImages.ToArray();
        _swapChainImageViews = new VkImageView[swapChainImageCount];
        Format = createInfo.imageFormat;

        for (int i = 0; i < swapChainImageCount; i++)
        {
            var viewCreateInfo = new VkImageViewCreateInfo(
                _swapChainImages[i],
                VkImageViewType.Image2D,
                surfaceFormat.format,
                VkComponentMapping.Rgba,
                new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1)
                );

            device.DeviceApi.vkCreateImageView(&viewCreateInfo, null, out _swapChainImageViews[i]).CheckResult();
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < _swapChainImageViews.Length; i++)
        {
            Device.DeviceApi.vkDestroyImageView(_swapChainImageViews[i]);
        }

        if (Handle != VkSwapchainKHR.Null)
        {
            Device.DeviceApi.vkDestroySwapchainKHR(Handle);
        }

        if (_surface != VkSurfaceKHR.Null)
        {
            Device.InstanceApi.vkDestroySurfaceKHR(_surface);
        }
    }

    private VkExtent2D ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.currentExtent.width > 0)
        {
            return capabilities.currentExtent;
        }
        else
        {
            VkExtent2D actualExtent = Window.Extent;

            actualExtent = new VkExtent2D(
                Math.Max(capabilities.minImageExtent.width, Math.Min(capabilities.maxImageExtent.width, actualExtent.width)),
                Math.Max(capabilities.minImageExtent.height, Math.Min(capabilities.maxImageExtent.height, actualExtent.height))
                );

            return actualExtent;
        }
    }

    private static VkSurfaceFormatKHR ChooseSwapSurfaceFormat(ReadOnlySpan<VkSurfaceFormatKHR> availableFormats)
    {
        // If the surface format list only includes one entry with VK_FORMAT_UNDEFINED,
        // there is no preferred format, so we assume VK_FORMAT_B8G8R8A8_UNORM
        if ((availableFormats.Length == 1) && (availableFormats[0].format == VkFormat.Undefined))
        {
            return new VkSurfaceFormatKHR(VkFormat.B8G8R8A8Unorm, availableFormats[0].colorSpace);
        }

        // iterate over the list of available surface format and
        // check for the presence of VK_FORMAT_B8G8R8A8_UNORM
        foreach (VkSurfaceFormatKHR availableFormat in availableFormats)
        {
            if (availableFormat.format == VkFormat.B8G8R8A8Unorm)
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
