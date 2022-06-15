// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#if !NET6_0_OR_GREATER
namespace Vortice.Vulkan;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void* PFN_vkAllocationFunction(void* pUserData, nuint size, nuint alignment, VkSystemAllocationScope allocationScope);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void PFN_vkFreeFunction(void* pUserData, void* pMemory);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void PFN_vkInternalAllocationNotification(void* pUserData, nuint size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void PFN_vkInternalFreeNotification(void* pUserData, nuint size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void* PFN_vkReallocationFunction(void* pUserData, void* pOriginal, nuint size, nuint alignment, VkSystemAllocationScope allocationScope);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate VkBool32 PFN_vkDebugReportCallbackEXT(
   VkDebugReportFlagsEXT flags,
   VkDebugReportObjectTypeEXT objectType,
   ulong @object,
   nuint location,
   int messageCode,
   byte* pLayerPrefix,
   byte* pMessage,
   void* pUserData);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate uint PFN_vkDebugUtilsMessengerCallbackEXT(
    VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
    VkDebugUtilsMessageTypeFlagsEXT messageTypes,
    VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
    void* userData);

#endif
