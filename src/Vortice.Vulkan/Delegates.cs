// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#if NETSTANDARD2_0
namespace Vortice.Vulkan
{
    public unsafe delegate void* PFN_vkAllocationFunction(void* pUserData, nuint size, nuint alignment, VkSystemAllocationScope allocationScope);
    public unsafe delegate void PFN_vkFreeFunction(void* pUserData, void* pMemory);
    public unsafe delegate void PFN_vkInternalAllocationNotification(void* pUserData, nuint size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);
    public unsafe delegate void PFN_vkInternalFreeNotification(void* pUserData, nuint size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);
    public unsafe delegate void* PFN_vkReallocationFunction(void* pUserData, void* pOriginal, nuint size, nuint alignment, VkSystemAllocationScope allocationScope);

    public unsafe delegate VkBool32 PFN_vkDebugReportCallbackEXT(
       VkDebugReportFlagsEXT flags,
       VkDebugReportObjectTypeEXT objectType,
       ulong @object,
       nuint location,
       int messageCode,
       byte* pLayerPrefix,
       byte* pMessage,
       void* pUserData);

    public unsafe delegate uint PFN_vkDebugUtilsMessengerCallbackEXT(
        VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* userData);
}
#endif
