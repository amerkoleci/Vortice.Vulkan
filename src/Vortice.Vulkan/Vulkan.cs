// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    public static unsafe partial class Vulkan
    {
        private delegate IntPtr LoadFunction(IntPtr context, string name);

        private static IntPtr s_vulkanModule = IntPtr.Zero;
        private static readonly ILibraryLoader _loader = InitializeLoader();
        private static VkInstance s_loadedInstance = VkInstance.Null;
        private static VkDevice s_loadedDevice = VkDevice.Null;

        public static VkResult vkInitialize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                s_vulkanModule = _loader.LoadNativeLibrary("vulkan-1.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.dylib");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.1.dylib");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libMoltenVK.dylib");
            }
            else
            {
                s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so.1");
                if (s_vulkanModule == IntPtr.Zero)
                    s_vulkanModule = _loader.LoadNativeLibrary("libvulkan.so");
            }

            if (s_vulkanModule == IntPtr.Zero)
            {
                return VkResult.ErrorInitializationFailed;
            }

#if CALLI_SUPPORT
            vkGetInstanceProcAddr_ptr = GetProcAddress(nameof(vkGetInstanceProcAddr));
#else
            vkGetInstanceProcAddr_ptr = GetProcAddress<vkGetInstanceProcAddrDelegate>(nameof(vkGetInstanceProcAddr));
#endif

            GenLoadLoader(IntPtr.Zero, vkGetInstanceProcAddr);

            return VkResult.Success;
        }

        public static void vkLoadInstance(VkInstance instance)
        {
            s_loadedInstance = instance;
            GenLoadInstance(instance.Handle, vkGetInstanceProcAddr);
            GenLoadDevice(instance.Handle, vkGetInstanceProcAddr);

            // Manually load win32 entries.
            vkCreateWin32SurfaceKHR_ptr = LoadCallback<vkCreateWin32SurfaceKHRDelegate>(instance.Handle, vkGetInstanceProcAddr, nameof(vkCreateWin32SurfaceKHR));
            vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = LoadCallback<vkGetPhysicalDeviceWin32PresentationSupportKHRDelegate>(instance.Handle, vkGetInstanceProcAddr, nameof(vkGetPhysicalDeviceWin32PresentationSupportKHR));
        }

        private static void GenLoadLoader(IntPtr context, LoadFunction load)
        {
            vkCreateInstance_ptr = LoadCallbackThrow<vkCreateInstanceDelegate>(context, load, "vkCreateInstance");
            vkEnumerateInstanceExtensionProperties_ptr = LoadCallbackThrow<vkEnumerateInstanceExtensionPropertiesDelegate>(context, load, "vkEnumerateInstanceExtensionProperties");
            vkEnumerateInstanceLayerProperties_ptr = LoadCallbackThrow<vkEnumerateInstanceLayerPropertiesDelegate>(context, load, "vkEnumerateInstanceLayerProperties");
            vkEnumerateInstanceVersion_ptr = LoadCallback<vkEnumerateInstanceVersionDelegate>(context, load, "vkEnumerateInstanceVersion");
        }

#if !CALLI_SUPPORT
        private static T LoadCallbackThrow<T>(IntPtr context, LoadFunction load, string name)
        {
            var functionPtr = load(context, name);
            if (functionPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"No function was found with the name {name}.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }

        private static T LoadCallback<T>(IntPtr context, LoadFunction load, string name)
        {
            var functionPtr = load(context, name);
            if (functionPtr == IntPtr.Zero)
                return default;

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }
#endif

        private static ILibraryLoader InitializeLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsLoader();
            }
            else
            {
                return new UnixLoader();
            }
        }

        private static TDelegate GetProcAddress<TDelegate>(string procName) where TDelegate : class
        {
            var handle = _loader.GetSymbol(s_vulkanModule, procName);
            return handle == IntPtr.Zero
                ? null
                : Marshal.GetDelegateForFunctionPointer<TDelegate>(handle);
        }

        public static IntPtr vkGetInstanceProcAddr(IntPtr instance, string name)
        {
            int byteCount = Interop.GetMaxByteCount(name);
            var stringPtr = stackalloc byte[byteCount];
            Interop.StringToPointer(name, stringPtr, byteCount);
            return vkGetInstanceProcAddr_ptr(instance, stringPtr);
        }

        /// <summary>
        /// Returns up to requested number of global extension properties
        /// </summary>
        /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
        /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/> </returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static unsafe ReadOnlySpan<VkExtensionProperties> vkEnumerateInstanceExtensionProperties(string layerName = null)
        {
            int dstLayerNameByteCount = Interop.GetMaxByteCount(layerName);
            var dstLayerNamePtr = stackalloc byte[dstLayerNameByteCount];
            Interop.StringToPointer(layerName, dstLayerNamePtr, dstLayerNameByteCount);

            uint count = 0;
            vkEnumerateInstanceExtensionProperties(dstLayerNamePtr, &count, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* ptr = properties)
            {
                vkEnumerateInstanceExtensionProperties(dstLayerNamePtr, &count, ptr).CheckResult();
            }

            return properties;
        }

        /// <summary>
        /// Returns properties of available physical device extensions
        /// </summary>
        /// <param name="physicalDevice">The <see cref="VkPhysicalDevice"/> that will be queried.</param>
        /// <param name="layerName">Is either null/empty or a string naming the layer to retrieve extensions from.</param>
        /// <returns>A <see cref="ReadOnlySpan{VkExtensionProperties}"/>.</returns>
        /// <exception cref="VkException">Vulkan returns an error code.</exception>
        public static ReadOnlySpan<VkExtensionProperties> vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string layerName = "")
        {
            int dstLayerNameByteCount = Interop.GetMaxByteCount(layerName);
            var dstLayerNamePtr = stackalloc byte[dstLayerNameByteCount];
            Interop.StringToPointer(layerName, dstLayerNamePtr, dstLayerNameByteCount);

            uint propertyCount = 0;
            vkEnumerateDeviceExtensionProperties(physicalDevice, dstLayerNamePtr, &propertyCount, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[propertyCount];
            fixed (VkExtensionProperties* propertiesPtr = properties)
            {
                vkEnumerateDeviceExtensionProperties(physicalDevice, dstLayerNamePtr, &propertyCount, propertiesPtr).CheckResult();
            }
            return properties;
        }

        /// <summary>
        /// Query instance-level version before instance creation.
        /// </summary>
        /// <returns>The version of Vulkan supported by instance-level functionality.</returns>
        public static unsafe VkVersion vkEnumerateInstanceVersion()
        {
            if (vkEnumerateInstanceVersion_ptr != null
                && vkEnumerateInstanceVersion_ptr(out var apiVersion) == VkResult.Success)
            {
                return new VkVersion(apiVersion);
            }

            return VkVersion.Version_1_0;
        }

        public static VkResult vkCreateShaderModule(VkDevice device, byte[] bytecode, VkAllocationCallbacks* allocator, out VkShaderModule shaderModule)
        {
            fixed (byte* bytecodePtr = bytecode)
            {
                var createInfo = new VkShaderModuleCreateInfo
                {
                    sType = VkStructureType.ShaderModuleCreateInfo,
                    codeSize = new VkPointerSize((uint)bytecode.Length),
                    pCode = (uint*)bytecodePtr
                };

                return vkCreateShaderModule(device, &createInfo, allocator, out shaderModule);
            }
        }

        public static VkResult vkCreateGraphicsPipeline(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo, out VkPipeline pipeline)
        {
            VkPipeline pinPipeline;
            var result = vkCreateGraphicsPipelines(device, pipelineCache, 1, &createInfo, null, &pinPipeline);
            pipeline = pinPipeline;
            return result;
        }

        public static VkResult vkCreateGraphicsPipelines(
            VkDevice device,
            VkPipelineCache pipelineCache,
            ReadOnlySpan<VkGraphicsPipelineCreateInfo> createInfos,
            Span<VkPipeline> pipelines)
        {
            fixed (VkGraphicsPipelineCreateInfo* createInfosPtr = createInfos)
            {
                fixed (VkPipeline* pipelinesPtr = pipelines)
                {
                    return vkCreateGraphicsPipelines(device, pipelineCache, (uint)createInfos.Length, createInfosPtr, null, pipelinesPtr);
                }
            }
        }

        public static VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, VkComputePipelineCreateInfo createInfo, out VkPipeline pipeline)
        {
            VkPipeline pinPipeline;
            var result = vkCreateComputePipelines(device, pipelineCache, 1, &createInfo, null, &pinPipeline);
            pipeline = pinPipeline;
            return result;
        }

        public static VkResult vkCreateComputePipelines(
            VkDevice device,
            VkPipelineCache pipelineCache,
            ReadOnlySpan<VkComputePipelineCreateInfo> createInfos,
            Span<VkPipeline> pipelines)
        {
            fixed (VkComputePipelineCreateInfo* createInfosPtr = createInfos)
            {
                fixed (VkPipeline* pipelinesPtr = pipelines)
                {
                    return vkCreateComputePipelines(device, pipelineCache, (uint)createInfos.Length, createInfosPtr, null, pipelinesPtr);
                }
            }
        }

        public static Span<T> vkMapMemory<T>(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, ulong offset = 0, ulong size = WholeSize, VkMemoryMapFlags flags = VkMemoryMapFlags.None) where T : unmanaged
        {
            void* pData;
            vkMapMemory(device, memory, offset, size, flags, &pData).CheckResult();

            if (size == WholeSize)
            {
                vkGetBufferMemoryRequirements(device, buffer, out var memoryRequirements);
                return new Span<T>(pData, (int)memoryRequirements.size);
            }

            return new Span<T>(pData, (int)size);
        }

        public static Span<T> vkMapMemory<T>(VkDevice device, VkImage image, VkDeviceMemory memory, ulong offset = 0, ulong size = WholeSize, VkMemoryMapFlags flags = VkMemoryMapFlags.None) where T : unmanaged
        {
            void* pData;
            vkMapMemory(device, memory, offset, size, flags, &pData).CheckResult();


            if (size == WholeSize)
            {
                vkGetImageMemoryRequirements(device, image, out var memoryRequirements);
                return new Span<T>(pData, (int)memoryRequirements.size);
            }

            return new Span<T>(pData, (int)size);
        }

        public static void vkUpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet writeDescriptorSet)
        {
            vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 0, null);
        }

        public static void vkUpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet writeDescriptorSet, VkCopyDescriptorSet copyDescriptorSet)
        {
            vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 1, &copyDescriptorSet);
        }

        public static void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets)
        {
            fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
            {
                vkUpdateDescriptorSets(device, (uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, 0, null);
            }
        }

        public static void vkUpdateDescriptorSets(VkDevice device, ReadOnlySpan<VkWriteDescriptorSet> writeDescriptorSets, ReadOnlySpan<VkCopyDescriptorSet> copyDescriptorSets)
        {
            fixed (VkWriteDescriptorSet* writeDescriptorSetsPtr = writeDescriptorSets)
            {
                fixed (VkCopyDescriptorSet* copyDescriptorSetsPtr = copyDescriptorSets)
                {
                    vkUpdateDescriptorSets(device, (uint)writeDescriptorSets.Length, writeDescriptorSetsPtr, (uint)copyDescriptorSets.Length, copyDescriptorSetsPtr);
                }
            }
        }

        public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* descriptorSets)
        {
            vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, descriptorSetCount, descriptorSets, 0, null);
        }

        public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet descriptorSet)
        {
            vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, 1, &descriptorSet, 0, null);
        }

        public static void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, ReadOnlySpan<VkDescriptorSet> descriptorSets)
        {
            fixed (VkDescriptorSet* descriptorSetsPtr = descriptorSets)
            {
                vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, (uint)descriptorSets.Length, descriptorSetsPtr, 0, null);
            }
        }

        public static void vkCmdBindDescriptorSets(
            VkCommandBuffer commandBuffer,
            VkPipelineBindPoint pipelineBindPoint,
            VkPipelineLayout layout,
            uint firstSet,
            ReadOnlySpan<VkDescriptorSet> descriptorSets,
            ReadOnlySpan<uint> dynamicOffsets)
        {
            fixed (VkDescriptorSet* descriptorSetsPtr = descriptorSets)
            {
                fixed (uint* dynamicOffsetsPtr = dynamicOffsets)
                {
                    vkCmdBindDescriptorSets(commandBuffer, pipelineBindPoint, layout, firstSet, (uint)descriptorSets.Length, descriptorSetsPtr, (uint)dynamicOffsets.Length, dynamicOffsetsPtr);
                }
            }
        }

        public static void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, VkBuffer buffer, ulong offset = 0)
        {
            vkCmdBindVertexBuffers(commandBuffer, firstBinding, 1, &buffer, &offset);
        }

        public static void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, ReadOnlySpan<VkBuffer> buffers, ReadOnlySpan<ulong> offsets)
        {
            fixed (VkBuffer* buffersPtr = buffers)
            {
                fixed (ulong* offsetPtr = offsets)
                {
                    vkCmdBindVertexBuffers(commandBuffer, firstBinding, (uint)buffers.Length, buffersPtr, offsetPtr);
                }
            }
        }



        public static void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, VkCommandBuffer secondaryCommandBuffer)
        {
            vkCmdExecuteCommands(commandBuffer, 1, &secondaryCommandBuffer);
        }

        public static void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, ReadOnlySpan<VkCommandBuffer> secondaryCommandBuffers)
        {
            fixed (VkCommandBuffer* commandBuffersPtr = secondaryCommandBuffers)
            {
                vkCmdExecuteCommands(commandBuffer, (uint)secondaryCommandBuffers.Length, commandBuffersPtr);
            }
        }

        public static VkResult vkQueuePresentKHR(VkQueue queue, VkSemaphore waitSemaphore, VkSwapchainKHR swapchain, uint imageIndex)
        {
            var presentInfo = new VkPresentInfoKHR
            {
                sType = VkStructureType.PresentInfoKHR,
                pNext = null
            };

            if (waitSemaphore != VkSemaphore.Null)
            {
                presentInfo.waitSemaphoreCount = 1u;
                presentInfo.pWaitSemaphores = &waitSemaphore;
            }

            if (swapchain != VkSwapchainKHR.Null)
            {
                presentInfo.swapchainCount = 1u;
                presentInfo.pSwapchains = &swapchain;
                presentInfo.pImageIndices = &imageIndex;
            }

            return vkQueuePresentKHR(queue, &presentInfo);
        }

        #region Nested
        internal interface ILibraryLoader
        {
            IntPtr LoadNativeLibrary(string name);

            IntPtr GetSymbol(IntPtr module, string name);
        }

        private class WindowsLoader : ILibraryLoader
        {
            public IntPtr LoadNativeLibrary(string name) => LoadLibrary(name);

            public IntPtr GetSymbol(IntPtr module, string name) => GetProcAddress(module, name);

            [DllImport("kernel32")]
            private static extern IntPtr LoadLibrary(string fileName);

            [DllImport("kernel32")]
            private static extern IntPtr GetProcAddress(IntPtr module, string procName);

            [DllImport("kernel32")]
            private static extern int FreeLibrary(IntPtr module);
        }

        private class UnixLoader : ILibraryLoader
        {
            public IntPtr LoadNativeLibrary(string name) => dlopen(name, RTLD_NOW | RTLD_LOCAL);

            public IntPtr GetSymbol(IntPtr module, string name) => dlsym(module, name);

            [DllImport("libdl", EntryPoint = "dlopen")]
            private static extern IntPtr dlopen(string fileName, int flags);

            [DllImport("libdl", EntryPoint = "dlsym")]
            private static extern IntPtr dlsym(IntPtr handle, string name);

            [DllImport("libdl", EntryPoint = "dlclose")]
            private static extern int dlclose(IntPtr handle);

            [DllImport("libdl", EntryPoint = "dlerror")]
            private static extern string dlerror();

            private const int RTLD_LOCAL = 0x0000;
            private const int RTLD_NOW = 0x0002;
        }
        #endregion
    }
}
