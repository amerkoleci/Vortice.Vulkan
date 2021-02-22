// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#if NETSTANDARD2_0
using System;
namespace Vortice.Vulkan
{
    public unsafe delegate uint vkDebugUtilsMessengerCallbackEXT(
        VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* userData);
}
#endif
