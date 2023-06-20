// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_instanceFunctions = new()
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

    private static readonly HashSet<string> s_outReturnFunctions = new()
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

        "vkGetDeviceGroupPeerMemoryFeaturesKHR",
        "vkGetDeviceQueue2",
        "vkCreateSamplerYcbcrConversion",
        "vkCreateDescriptorUpdateTemplate",
        "vkCreateRenderPass2",
        "vkGetPhysicalDeviceSurfaceSupportKHR",
        "vkGetPhysicalDeviceSurfaceCapabilitiesKHR",

        "vkCreateSwapchainKHR",
        "vkAcquireNextImageKHR",
        "vkGetDeviceGroupSurfacePresentModesKHR",
        "vkAcquireNextImage2KHR",

        "vkCreateDisplayModeKHR",
        "vkGetDisplayPlaneCapabilitiesKHR",
        "vkCreateDisplayPlaneSurfaceKHR",
        "vkCreateSharedSwapchainsKHR",

        "vkCreateDebugUtilsMessengerEXT"
    };

    private static string GetFunctionPointerSignature(CppFunction function, bool canUseOut, bool allowNonBlittable = true)
    {
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

            if (parameter.Name.EndsWith("Count"))
            {
                if (function.Name.StartsWith("vkEnumerate") ||
                    function.Name.StartsWith("vkGet"))
                {
                    paramCsType = "int*";
                }
                else
                {
                    paramCsType = "int";
                }
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

        return $"delegate* unmanaged<{builder}>";
    }

    private static void GenerateCommands(CppCompilation compilation, string outputPath)
    {
        // Generate Functions
        using var writer = new CodeWriter(Path.Combine(outputPath, "Commands.cs"),
            false,
            "System"
            );

        Dictionary<string, CppFunction> commands = new();
        Dictionary<string, CppFunction> instanceCommands = new();
        Dictionary<string, CppFunction> deviceCommands = new();
        foreach (CppFunction? cppFunction in compilation.Functions)
        {
            string? returnType = GetCsTypeName(cppFunction.ReturnType, false);
            bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
            string? csName = cppFunction.Name;
            string argumentsString = GetParameterSignature(cppFunction, canUseOut);

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
            foreach (KeyValuePair<string, CppFunction> command in commands)
            {
                CppFunction cppFunction = command.Value;

                if (cppFunction.Name == "vkGetInstanceProcAddr" ||
                    cppFunction.Name == "vkGetDeviceProcAddr")
                {
                    continue;
                }

                bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                string functionPointerSignature = GetFunctionPointerSignature(cppFunction, false);
                string modifier = "private";

                // Used by VulkanMemoryAllocator
                if(command.Key == "vkGetPhysicalDeviceProperties" ||
                    command.Key == "vkGetBufferMemoryRequirements2KHR" ||
                    command.Key == "vkGetBufferMemoryRequirements2" ||
                    command.Key == "vkGetImageMemoryRequirements2KHR" ||
                    command.Key == "vkGetImageMemoryRequirements2" ||
                    command.Key == "vkBindBufferMemory2KHR" ||
                    command.Key == "vkBindBufferMemory2" ||
                    command.Key == "vkBindImageMemory2KHR" ||
                    command.Key == "vkBindImageMemory2" ||
                    command.Key == "vkGetPhysicalDeviceMemoryProperties2KHR" ||
                    command.Key == "vkGetDeviceImageMemoryRequirements" ||
                    command.Key == "vkGetDeviceBufferMemoryRequirements")
                {
                    modifier = "internal";
                }
                writer.WriteLine($"{modifier} static {functionPointerSignature} {command.Key}_ptr;");
                WriteFunctionInvocation(writer, cppFunction, false);

                if (canUseOut)
                {
                    functionPointerSignature = GetFunctionPointerSignature(cppFunction, true);
                    writer.WriteLine($"private static {functionPointerSignature} {command.Key}_out_ptr;");

                    WriteFunctionInvocation(writer, cppFunction, true);
                }
            }

            WriteCommands(writer, "GenLoadInstance", instanceCommands);
            WriteCommands(writer, "GenLoadDevice", deviceCommands);
        }
    }

    private static void WriteCommands(CodeWriter writer, string name, Dictionary<string, CppFunction> commands)
    {
        using (writer.PushBlock($"private static void {name}(IntPtr context, LoadFunction load)"))
        {
            foreach (KeyValuePair<string, CppFunction> instanceCommand in commands)
            {
                string commandName = instanceCommand.Key;
                if (commandName == "vkGetInstanceProcAddr" ||
                    commandName == "vkGetDeviceProcAddr")
                {
                    continue;
                }

                bool canUseOut = s_outReturnFunctions.Contains(instanceCommand.Value.Name);
                string functionPointerSignature = GetFunctionPointerSignature(instanceCommand.Value, false);
                writer.WriteLine($"{commandName}_ptr = ({functionPointerSignature}) load(context, nameof({commandName}));");

                if (canUseOut)
                {
                    functionPointerSignature = GetFunctionPointerSignature(instanceCommand.Value, true);
                    writer.WriteLine($"{commandName}_out_ptr = ({functionPointerSignature}) load(context, nameof({commandName}));");
                }
            }
        }
    }

    private static void WriteFunctionInvocation(CodeWriter writer, CppFunction cppFunction, bool canUseOut)
    {
        string returnCsName = GetCsTypeName(cppFunction.ReturnType, false);
        string argumentsString = GetParameterSignature(cppFunction, canUseOut);

        using (writer.PushBlock($"public static {returnCsName} {cppFunction.Name}({argumentsString})"))
        {
            if (returnCsName != "void")
            {
                writer.Write("return ");
            }

            if (canUseOut)
            {
                writer.Write($"{cppFunction.Name}_out_ptr(");
            }
            else
            {
                writer.Write($"{cppFunction.Name}_ptr(");
            }

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
        writer.WriteLine($"{function.Name}_ptr({callArgumentString}){postCall};");
    }

    private static bool IsInstanceFunction(string name)
    {
        return s_instanceFunctions.Contains(name);
    }

    public static string GetParameterSignature(CppFunction cppFunction, bool canUseOut)
    {
        return GetParameterSignature(cppFunction.Parameters, canUseOut, cppFunction.Name);
    }

    private static string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut, string functionName)
    {
        var argumentBuilder = new StringBuilder();
        int index = 0;

        foreach (CppParameter cppParameter in parameters)
        {
            string direction = string.Empty;
            string paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
            string paramCsName = GetParameterName(cppParameter.Name);

            if (cppParameter.Name.EndsWith("Count"))
            {
                if (functionName.StartsWith("vkEnumerate") ||
                    functionName.StartsWith("vkGet"))
                {
                    paramCsTypeName = "int*";
                }
                else
                {
                    paramCsTypeName = "int";
                }
            }

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
            else
            {
                if (paramCsTypeName == "VkAllocationCallbacks*")
                {
                    argumentBuilder.Append(" = default");
                }
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
