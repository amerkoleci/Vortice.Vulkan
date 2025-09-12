// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public ref struct SwapChainSupportDetails
{
    public VkSurfaceCapabilitiesKHR Capabilities;
    public Span<VkSurfaceFormatKHR> Formats;
    public Span<VkPresentModeKHR> PresentModes;
}

public static class Utils
{
    public static SwapChainSupportDetails QuerySwapChainSupport(VkInstanceApi api, VkPhysicalDevice physicalDevice, VkSurfaceKHR surface)
    {
        SwapChainSupportDetails details = new();
        api.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physicalDevice, surface, out details.Capabilities).CheckResult();

        api.vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, out uint surfaceFormatCount).CheckResult();
        details.Formats = new VkSurfaceFormatKHR[surfaceFormatCount];
        api.vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, details.Formats).CheckResult();

        api.vkGetPhysicalDeviceSurfacePresentModesKHR(physicalDevice, surface, out uint presentModeCount).CheckResult();
        details.PresentModes = new VkPresentModeKHR[presentModeCount];
        api.vkGetPhysicalDeviceSurfacePresentModesKHR(physicalDevice, surface, details.PresentModes).CheckResult();
        return details;
    }
}
