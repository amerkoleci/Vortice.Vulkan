// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static readonly HashSet<string> s_instanceFunctions = new HashSet<string>
        {
            "vkGetDeviceProcAddr",
            "vkCmdBeginDebugUtilsLabelEXT",
            "vkCmdEndDebugUtilsLabelEXT",
            "vkCmdInsertDebugUtilsLabelEXT",
            "vkCreateDebugUtilsMessengerEXT",
            "vkDestroyDebugUtilsMessengerEXT",
            "vkQueueBeginDebugUtilsLabelEXT",
            "vkQueueEndDebugUtilsLabelEXT",
            "vkQueueInsertDebugUtilsLabelEXT",
            "vkSetDebugUtilsObjectNameEXT",
            "vkSetDebugUtilsObjectTagEXT",
            "vkSubmitDebugUtilsMessageEXT"
        };

        private static readonly HashSet<string> s_outReturnFunctions = new HashSet<string>
        {
            "vkCreateInstance",
            "vkCreateDevice",
            "vkGetPhysicalDeviceFeatures",
            "vkGetPhysicalDeviceFormatProperties",
            "vkGetPhysicalDeviceImageFormatProperties",
            "vkGetPhysicalDeviceProperties",
            "vkGetPhysicalDeviceMemoryProperties",
            "vkGetDeviceQueue",
            "vkGetDeviceMemoryCommitment",
            "vkGetBufferMemoryRequirements",
            "vkGetImageMemoryRequirements",
            "vkAllocateMemory",
            "vkCreateAndroidSurfaceKHR",
            "vkCreateWin32SurfaceKHR",
            "vkCreateXlibSurfaceKHR",
            "vkCreateWaylandSurfaceKHR",
            "vkCreateMetalSurfaceEXT",
            "vkCreateIOSSurfaceMVK",
            "vkCreateMacOSSurfaceMVK",
            "vkCreateFence",
            "vkCreateSemaphore",
            "vkCreateEvent",
            "vkCreateQueryPool",
            "vkCreateBuffer",
            "vkCreateBufferView",
            "vkCreateImage",
            "vkGetImageSubresourceLayout",
            "vkCreateImageView",
            "vkCreateShaderModule",
            "vkCreatePipelineCache",
            //"vkCreateGraphicsPipelines",
            //"vkCreateComputePipelines",
            "vkCreatePipelineLayout",
            "vkCreateSampler",
            "vkCreateDescriptorSetLayout",
            "vkCreateDescriptorPool",
            //"vkAllocateDescriptorSets",
            "vkCreateFramebuffer",
            "vkCreateRenderPass",
            "vkGetRenderAreaGranularity",
            "vkCreateCommandPool",
            //"vkAllocateCommandBuffers",

            "vkEnumerateInstanceVersion",
            "vkGetDeviceGroupPeerMemoryFeaturesKHR",
            "vkGetImageMemoryRequirements2",
            "vkGetBufferMemoryRequirements2",
            "vkGetPhysicalDeviceFeatures2",
            "vkGetPhysicalDeviceProperties2",
            "vkGetPhysicalDeviceFormatProperties2",
            "vkGetPhysicalDeviceImageFormatProperties2",
            "vkGetPhysicalDeviceMemoryProperties2",
            "vkGetDeviceQueue2",
            "vkCreateSamplerYcbcrConversion",
            "vkCreateDescriptorUpdateTemplate",
            "vkGetPhysicalDeviceExternalBufferProperties",
            "vkGetPhysicalDeviceExternalFenceProperties",
            "vkGetPhysicalDeviceExternalSemaphoreProperties",
            "vkGetDescriptorSetLayoutSupport",
            "vkCreateRenderPass2",
            "vkGetPhysicalDeviceSurfaceSupportKHR",
            "vkGetPhysicalDeviceSurfaceCapabilitiesKHR",

            "vkCreateSwapchainKHR",
            "vkAcquireNextImageKHR",
            "vkGetDeviceGroupPresentCapabilitiesKHR",
            "vkGetDeviceGroupSurfacePresentModesKHR",
            "vkAcquireNextImage2KHR",

            "vkCreateDisplayModeKHR",
            "vkGetDisplayPlaneCapabilitiesKHR",
            "vkCreateDisplayPlaneSurfaceKHR",
            "vkCreateSharedSwapchainsKHR",

            "vkGetPhysicalDeviceFeatures2KHR",
            "vkGetPhysicalDeviceProperties2KHR",
            "vkGetPhysicalDeviceFormatProperties2KHR",
            "vkGetPhysicalDeviceImageFormatProperties2KHR",
            "vkGetPhysicalDeviceMemoryProperties2KHR",
            "vkGetDeviceGroupPeerMemoryFeaturesKHR",
            "vkGetPhysicalDeviceExternalBufferPropertiesKHR",
            //"vkGetMemoryFdKHR",
            //"vkGetMemoryFdPropertiesKHR",
            "vkGetPhysicalDeviceExternalSemaphorePropertiesKHR",
            //"vkGetSemaphoreFdKHR",

            "vkCreateDebugUtilsMessengerEXT"
        };

        private static string GetFunctionPointerSignature(bool netstandard, CppFunction function, bool allowNonBlittable = true)
        {
            bool canUseOut = s_outReturnFunctions.Contains(function.Name);

            StringBuilder builder = new();
            foreach (CppParameter parameter in function.Parameters)
            {
                string paramCsType = GetCsTypeName(parameter.Type, false);

                if (canUseOut &&
                    CanBeUsedAsOutput(parameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                {
                    builder.Append("out ");
                    paramCsType = GetCsTypeName(cppTypeDeclaration, false);
                }

                builder.Append(paramCsType).Append(", ");
            }

            string returnCsName = GetCsTypeName(function.ReturnType, false);
            if (!allowNonBlittable)
            {
                // Otherwise we get interop issues with non blittable types
                if (returnCsName == "VkBool32")
                    returnCsName = "uint";
            }

            builder.Append(returnCsName);

            if (netstandard)
            {
                return $"delegate* unmanaged[Stdcall]<{builder}>";
            }

            return $"delegate* unmanaged<{builder}>";
        }

        private static void GenerateCommands(CppCompilation compilation, string outputPath, bool netStandard)
        {
            // Generate Functions
            string fileName = netStandard ? "Commands.NetStandard.cs" : "Commands.cs";
            using var writer = new CodeWriter(Path.Combine(outputPath, fileName),
                "System"
                );

            var commands = new Dictionary<string, CppFunction>();
            var instanceCommands = new Dictionary<string, CppFunction>();
            var deviceCommands = new Dictionary<string, CppFunction>();
            foreach (CppFunction? cppFunction in compilation.Functions)
            {
                string? returnType = GetCsTypeName(cppFunction.ReturnType, false);
                bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                string? csName = cppFunction.Name;
                string? argumentsString = GetParameterSignature(cppFunction, canUseOut);

                commands.Add(csName, cppFunction);

                if (cppFunction.Parameters.Count > 0)
                {
                    var firstParameter = cppFunction.Parameters[0];
                    if (firstParameter.Type is CppTypedef typedef)
                    {
                        if (typedef.Name == "VkInstance" ||
                            typedef.Name == "VkPhysicalDevice" ||
                            IsInstanceFunction(cppFunction.Name))
                        {
                            instanceCommands.Add(csName, cppFunction);
                        }
                        else
                        {
                            deviceCommands.Add(csName, cppFunction);
                        }
                    }
                }
            }

            if (netStandard)
            {
                writer.WriteLine($"#if !NET5_0_OR_GREATER");
            }
            else
            {
                writer.WriteLine($"#if NET5_0_OR_GREATER");
            }

            using (writer.PushBlock($"unsafe partial class Vulkan"))
            {
                foreach (KeyValuePair<string, CppFunction> command in commands)
                {
                    CppFunction cppFunction = command.Value;

                    if (cppFunction.Name == "vkGetInstanceProcAddr")
                    {
                        continue;
                    }

                    string functionPointerSignatureNS = GetFunctionPointerSignature(true, cppFunction);
                    string functionPointerSignature = GetFunctionPointerSignature(false, cppFunction);
                    if (netStandard)
                    {
                        writer.WriteLine($"private static {functionPointerSignatureNS} {command.Key}_ptr;");
                    }
                    else
                    {
                        writer.WriteLine($"private static {functionPointerSignature} {command.Key}_ptr;");
                    }

                    string returnCsName = GetCsTypeName(cppFunction.ReturnType, false);
                    bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                    var argumentsString = GetParameterSignature(cppFunction, canUseOut);

                    using (writer.PushBlock($"public static {returnCsName} {cppFunction.Name}({argumentsString})"))
                    {
                        if (returnCsName != "void")
                        {
                            writer.Write("return ");
                        }

                        writer.Write($"{command.Key}_ptr(");
                        int index = 0;
                        foreach (CppParameter cppParameter in cppFunction.Parameters)
                        {
                            string paramCsName = GetParameterName(cppParameter.Name);

                            if (canUseOut && CanBeUsedAsOutput(cppParameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                            {
                                writer.Write("out ");
                            }

                            writer.Write($"{paramCsName}");

                            if (index < cppFunction.Parameters.Count - 1)
                            {
                                writer.Write(", ");
                            }

                            index++;
                        }

                        writer.WriteLine(");");
                    }

                    writer.WriteLine();
                }

                WriteCommands(writer, "GenLoadInstance", instanceCommands, netStandard);
                WriteCommands(writer, "GenLoadDevice", deviceCommands, netStandard);
            }

            writer.WriteLine($"#endif");
        }

        private static void WriteCommands(CodeWriter writer, string name, Dictionary<string, CppFunction> commands, bool netStandard)
        {
            using (writer.PushBlock($"private static void {name}(IntPtr context, LoadFunction load)"))
            {
                foreach (KeyValuePair<string, CppFunction> instanceCommand in commands)
                {
                    string commandName = instanceCommand.Key;
                    if (commandName == "vkGetInstanceProcAddr")
                    {
                        continue;
                    }

                    string functionPointerSignature = GetFunctionPointerSignature(netStandard, instanceCommand.Value);
                    writer.WriteLine($"{commandName}_ptr = ({functionPointerSignature}) load(context, nameof({commandName}));");
                }
            }
        }

        private static void EmitInvoke(CodeWriter writer, CppFunction function, List<string> parameters, bool handleCheckResult = true)
        {
            var postCall = string.Empty;
            if (handleCheckResult)
            {
                var hasResultReturn = GetCsTypeName(function.ReturnType) == "VkResult";
                if (hasResultReturn)
                {
                    postCall = ".CheckResult()";
                }
            }

            int index = 0;
            var callArgumentStringBuilder = new StringBuilder();
            foreach (string? parameterName in parameters)
            {
                callArgumentStringBuilder.Append(parameterName);

                if (index < parameters.Count - 1)
                {
                    callArgumentStringBuilder.Append(", ");
                }

                index++;
            }

            string callArgumentString = callArgumentStringBuilder.ToString();
            writer.WriteLine($"{function.Name}({callArgumentString}){postCall};");
        }

        private static bool IsInstanceFunction(string name)
        {
            return s_instanceFunctions.Contains(name);
        }

        public static string GetParameterSignature(CppFunction cppFunction, bool canUseOut)
        {
            return GetParameterSignature(cppFunction.Parameters, canUseOut);
        }

        private static string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut)
        {
            var argumentBuilder = new StringBuilder();
            int index = 0;

            foreach (CppParameter cppParameter in parameters)
            {
                string direction = string.Empty;
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(cppParameter.Name);

                if (canUseOut && CanBeUsedAsOutput(cppParameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                argumentBuilder.Append(paramCsTypeName).Append(' ').Append(paramCsName);
                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        private static string GetParameterName(string name)
        {
            if (name == "event")
                return "@event";

            if (name == "object")
                return "@object";

            if (name.StartsWith('p')
                && char.IsUpper(name[1]))
            {
                name = char.ToLower(name[1]) + name.Substring(2);
                return GetParameterName(name);
            }

            return name;
        }

        private static bool CanBeUsedAsOutput(CppType type, out CppTypeDeclaration? elementTypeDeclaration)
        {
            if (type is CppPointerType pointerType)
            {
                if (pointerType.ElementType is CppTypedef typedef)
                {
                    elementTypeDeclaration = typedef;
                    return true;
                }
                else if (pointerType.ElementType is CppClass @class
                    && @class.ClassKind != CppClassKind.Class
                    && @class.SizeOf > 0)
                {
                    elementTypeDeclaration = @class;
                    return true;
                }
                else if (pointerType.ElementType is CppEnum @enum
                    && @enum.SizeOf > 0)
                {
                    elementTypeDeclaration = @enum;
                    return true;
                }
            }

            elementTypeDeclaration = null;
            return false;
        }
    }
}
