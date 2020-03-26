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
            "vkGetBufferMemoryRequirements",
            "vkGetImageMemoryRequirements",
            "vkCreateAndroidSurfaceKHR",
            "vkCreateWin32SurfaceKHR",
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
            "vkCreateGraphicsPipelines",
            "vkCreateComputePipelines",
            "vkCreatePipelineLayout",
            "vkCreateSampler",
            "vkCreateDescriptorSetLayout",
            "vkCreateDescriptorPool",
            //"vkAllocateDescriptorSets",
            "vkCreateFramebuffer",
            "vkCreateRenderPass",
            "vkGetRenderAreaGranularity",
            "vkCreateCommandPool",

            "vkEnumerateInstanceVersion",
        };

        private static readonly HashSet<string> s_outArrayReturnFunctions = new HashSet<string>
        {
            "vkGetPhysicalDeviceQueueFamilyProperties",
            "vkGetImageSparseMemoryRequirements",
            "vkGetPhysicalDeviceSparseImageFormatProperties",
            "vkGetQueryPoolResults"
        };

        private static bool calliFunction = false;

        private static void GenerateCommands(CppCompilation compilation, string outputPath)
        {
            // Generate Functions
            using var writer = new CodeWriter(Path.Combine(outputPath, "Commands.cs"),
                "System",
                "System.Diagnostics",
                "System.Runtime.InteropServices",
                "Vortice.Mathematics");

            var commands = new Dictionary<string, CppFunction>();
            var instanceCommands = new Dictionary<string, CppFunction>();
            var deviceCommands = new Dictionary<string, CppFunction>();
            foreach (var cppFunction in compilation.Functions)
            {
                var returnType = GetCsTypeName(cppFunction.ReturnType, false);
                bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                var csName = cppFunction.Name;
                var argumentsString = GetParameterSignature(cppFunction, canUseOut);

                writer.WriteLine("[UnmanagedFunctionPointer(CallingConvention.StdCall)]");
                writer.WriteLine($"public unsafe delegate {returnType} {csName}Delegate({argumentsString});");
                writer.WriteLine();

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

            using (writer.PushBlock($"unsafe partial class Vulkan"))
            {
                foreach (var command in commands)
                {
                    var cppFunction = command.Value;

                    if (calliFunction)
                    {
                        writer.WriteLine($"private static IntPtr {command.Key}_ptr;");
                        writer.WriteLine($"[Calli]");
                    }
                    else
                    {
                        writer.WriteLine($"private static {command.Key}Delegate {command.Key}_ptr;");
                    }

                    var returnType = GetCsTypeName(cppFunction.ReturnType, false);
                    bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                    var argumentsString = GetParameterSignature(cppFunction, canUseOut);

                    using (writer.PushBlock($"public static {returnType} {cppFunction.Name}({argumentsString})"))
                    {
                        if (returnType != "void")
                        {
                            writer.Write("return ");
                        }

                        writer.Write($"{command.Key}_ptr(");
                        var index = 0;
                        foreach (var cppParameter in cppFunction.Parameters)
                        {
                            var paramCsName = GetParameterName(cppParameter.Name);
                            if (canUseOut && CanBeUsedAsOutput(cppParameter.Type, out var cppTypeDeclaration))
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

                        writer.WriteLine($");");
                    }
                    writer.WriteLine();
                }

                WriteCommands(writer, "GenLoadInstance", instanceCommands);
                WriteCommands(writer, "GenLoadDevice", deviceCommands);
            }
        }

        private static void WriteCommands(CodeWriter writer, string name, Dictionary<string, CppFunction> commands)
        {
            using (writer.PushBlock($"private static void {name}(IntPtr context, LoadFunction load)"))
            {
                foreach (var instanceCommand in commands)
                {
                    var commandName = instanceCommand.Key;
                    if (calliFunction)
                    {

                    }
                    else
                    {
                        writer.WriteLine($"{commandName}_ptr = LoadCallback<{commandName}Delegate>(context, load, \"{commandName}\");");
                    }
                }
            }
        }

        private static bool IsInstanceFunction(string name)
        {
            return s_instanceFunctions.Contains(name);
        }

        private static string GetParameterSignature(CppFunction cppFunction, bool canUseOut)
        {
            var argumentBuilder = new StringBuilder();
            var index = 0;

            foreach (var cppParameter in cppFunction.Parameters)
            {
                var direction = string.Empty;
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(cppParameter.Name);

                if (canUseOut && CanBeUsedAsOutput(cppParameter.Type, out var cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                argumentBuilder.Append(paramCsTypeName).Append(" ").Append(paramCsName);
                if (index < cppFunction.Parameters.Count - 1)
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

        private static bool CanBeUsedAsOutput(CppType type, out CppTypeDeclaration elementTypeDeclaration)
        {
            if (type is CppPointerType pointerType)
            {
                if (pointerType.ElementType is CppTypedef typedef)
                {
                    elementTypeDeclaration = typedef;
                    return true;
                }
                else if (pointerType.ElementType is CppClass cppClass
                    && cppClass.ClassKind != CppClassKind.Class
                    && cppClass.SizeOf > 0)
                {
                    elementTypeDeclaration = cppClass;
                    return true;
                }
            }

            elementTypeDeclaration = null;
            return false;
        }
    }
}
