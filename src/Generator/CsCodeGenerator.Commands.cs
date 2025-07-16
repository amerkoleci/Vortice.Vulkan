// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private readonly HashSet<string> _instanceFunctions =
    [
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
    ];

    private readonly HashSet<string> _outReturnFunctions =
    [
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

        "vkCreateDebugUtilsMessengerEXT",
        "vkGetMemoryWin32HandleKHR",
        "vkGetSemaphoreWin32HandleKHR",
        "vkGetFenceWin32HandleKHR",
        "vkGetMemoryWin32HandleNV",
        "vkGetMemoryAndroidHardwareBufferANDROID",

        // vma
        "vmaCreateAllocator",
        "vmaGetAllocatorInfo",
        "vmaGetMemoryTypeProperties",
        "vmaCalculateStatistics",
        "vmaGetHeapBudgets",
        "vmaCreatePool",
        "vmaGetPoolStatistics",
        "vmaCalculatePoolStatistics",
        "vmaAllocateMemory",
        "vmaAllocateMemoryForBuffer",
        "vmaAllocateMemoryForImage",
        "vmaCreateBuffer",
        "vmaCreateBufferWithAlignment",
        "vmaCreateAliasingBuffer",
        "vmaCreateAliasingBuffer2",
        "vmaCreateImage",
        "vmaCreateAliasingImage",
        "vmaCreateAliasingImage2",
        "vmaCreateVirtualBlock",
        "vmaGetVirtualAllocationInfo",
        "vmaGetVirtualBlockStatistics",
        "vmaCalculateVirtualBlockStatistics",

        // spvc
        "spvc_context_create",
        "spvc_compiler_create_compiler_options",
        "spvc_compiler_create_shader_resources",
        "spvc_context_create_compiler",
        "spvc_context_parse_spirv",

        // Spirv-Reflect
        "spvReflectCreateShaderModule",
    ];

    private string GetFunctionPointerSignature(CppFunction function, bool allowNonBlittable = true)
    {
        StringBuilder builder = new();
        foreach (CppParameter parameter in function.Parameters)
        {
            string paramCsType = GetCsTypeName(parameter.Type);
            if ((paramCsType == "IntPtr" || paramCsType == "nint")
                && _outReturnFunctions.Contains(function.Name))
            {
                paramCsType += "*";
            }

            builder.Append(paramCsType).Append(", ");
        }

        string returnCsName = GetCsTypeName(function.ReturnType);
        if (!allowNonBlittable)
        {
            // Otherwise we get interop issues with non blittable types
            if (returnCsName == "VkBool32")
                returnCsName = "uint";
        }

        builder.Append(returnCsName);

        return $"delegate* unmanaged<{builder}>";
    }

    private static bool ShouldIgnoreFile(string sourceFileName, bool vulkan)
    {
        if (sourceFileName == "vadefs"
            || sourceFileName == "vcruntime"
            || sourceFileName == "corecrt"
            || sourceFileName == "stddef")
        {
            return true;
        }

        if (!vulkan)
            return ShouldIgnoreVulkanFile(sourceFileName);

        return false;
    }

    private static bool ShouldIgnoreVulkanFile(string sourceFileName)
    {
        if (sourceFileName == "vulkan_core"
            || sourceFileName.StartsWith("vulkan_video_"))
        {
            return true;
        }

        return false;
    }

    private void GenerateCommands(CppCompilation compilation)
    {
        Dictionary<string, CppFunction> commands = [];
        Dictionary<string, CppFunction> globalCommands = [];
        Dictionary<string, CppFunction> instanceCommands = [];
        Dictionary<string, CppFunction> deviceCommands = [];
        foreach (CppFunction? cppFunction in compilation.Functions)
        {
            if (cppFunction.Name == "spvc_context_set_error_callback"
                || cppFunction.Name == "vkGetInstanceProcAddr"
                || cppFunction.Name == "vkGetDeviceProcAddr"
                // We compile VMA with #define VMA_STATS_STRING_ENABLED 0
                || cppFunction.Name == "vmaBuildVirtualBlockStatsString"
                || cppFunction.Name == "vmaFreeVirtualBlockStatsString"
                || cppFunction.Name == "vmaBuildStatsString"
                || cppFunction.Name == "vmaFreeStatsString")
            {
                continue;
            }

            string sourceFileName = Path.GetFileNameWithoutExtension(cppFunction.SourceFile);
            if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                continue;

            if (cppFunction.Attributes.Count > 0 && cppFunction.Attributes[0].Name == "deprecated")
                continue;

            string? csName = cppFunction.Name;

            commands.Add(csName, cppFunction);

            if (_options.IsVulkan)
            {
                if (cppFunction.Parameters.Count > 0)
                {
                    CppParameter firstParameter = cppFunction.Parameters[0];
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
                    else
                    {
                        globalCommands.Add(csName, cppFunction);
                    }
                }
            }
        }

        if (commands.Count == 0)
            return;

        // Generate Functions
        List<string> usings =
        [
            "System",
            "System.Runtime.InteropServices",
            "System.Runtime.CompilerServices"
        ];
        if (_options.ExtraUsings.Count > 0)
        {
            usings.AddRange(_options.ExtraUsings);
        }
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Commands.cs"),
            false,
            _options.Namespace,
            [.. usings]
            );

        bool todoNew = false;
        if (_options.IsVulkan && todoNew)
        {
            using (writer.PushBlock($"unsafe partial class VkInstance"))
            {
                foreach (KeyValuePair<string, CppFunction> command in instanceCommands)
                {
                    CppFunction cppFunction = command.Value;

                    string functionPointerSignature = GetFunctionPointerSignature(cppFunction);
                    string modifier = GetFunctionModifier(command.Key);
                    writer.WriteLine($"{modifier} readonly /*{functionPointerSignature}*/ PFN_vkVoidFunction _{command.Key};");
                }

                writer.WriteLine();

                // Generate constructors
                using (writer.PushBlock($"public VkInstance(nint handle)"))
                {
                    writer.WriteLine("Handle = handle;");
                    writer.WriteLine();

                    WriteCommandsNew(writer, instanceCommands, "vkGetInstanceProcAddr");
                }
            }
        }

        {
            using (writer.PushBlock($"unsafe partial class {_options.ClassName}"))
            {
                // Write function declarations first
                if (_options.GenerateFunctionPointers)
                {
                    writer.WriteLine("// Global functions");
                    WriteFunctionDeclarations(writer, globalCommands);

                    writer.WriteLine("// Instance functions");
                    WriteFunctionDeclarations(writer, instanceCommands);

                    writer.WriteLine("// Device functions");
                    WriteFunctionDeclarations(writer, deviceCommands);
                }

                // Write function invocation now
                foreach (KeyValuePair<string, CppFunction> command in commands)
                {
                    CppFunction cppFunction = command.Value;

                    bool canUseOut = _outReturnFunctions.Contains(cppFunction.Name);

                    WriteFunctionInvocation(writer, cppFunction, false);

                    if (command.Key.StartsWith("vkCreate")
                        && command.Key != "vkCreateDeferredOperationKHR")
                    {
                        WriteFunctionInvocation(writer, cppFunction, false, true);
                    }

                    if (canUseOut)
                    {
                        WriteFunctionInvocation(writer, cppFunction, true);

                        if (command.Key.StartsWith("vkCreate")
                            && command.Key != "vkCreateDeferredOperationKHR")
                        {
                            WriteFunctionInvocation(writer, cppFunction, true, true);
                        }
                    }
                }

                if (_options.GenerateFunctionPointers)
                {
                    if (_options.IsVulkan)
                    {
                        WriteCommands(writer, "GenLoadInstance", instanceCommands);
                        WriteCommands(writer, "GenLoadDevice", deviceCommands);
                    }
                    else
                    {
                        WriteLibraryImport(writer, commands);
                    }
                }
            }
        }
    }

    private static string GetFunctionModifier(string name)
    {
        string modifier = "private";

        // Used by VulkanMemoryAllocator
        if (name == "vkGetPhysicalDeviceProperties"
            || name == "vkGetPhysicalDeviceMemoryProperties"
            || name == "vkGetBufferMemoryRequirements2KHR"
            || name == "vkGetBufferMemoryRequirements2"
            || name == "vkGetImageMemoryRequirements2KHR"
            || name == "vkGetImageMemoryRequirements2"
            || name == "vkBindBufferMemory2KHR"
            || name == "vkBindBufferMemory2"
            || name == "vkBindImageMemory2KHR"
            || name == "vkBindImageMemory2"
            || name == "vkGetDeviceImageMemoryRequirements"
            || name == "vkGetDeviceBufferMemoryRequirements"
            || name == "vkAllocateMemory"
            || name == "vkFreeMemory"
            || name == "vkMapMemory"
            || name == "vkUnmapMemory"
            || name == "vkFlushMappedMemoryRanges"
            || name == "vkInvalidateMappedMemoryRanges"
            || name == "vkBindBufferMemory"
            || name == "vkBindImageMemory"
            || name == "vkGetBufferMemoryRequirements"
            || name == "vkGetImageMemoryRequirements"
            || name == "vkCreateBuffer"
            || name == "vkDestroyBuffer"
            || name == "vkCreateImage"
            || name == "vkDestroyImage"
            || name == "vkCmdCopyBuffer"
            || name == "vkGetPhysicalDeviceMemoryProperties2KHR"
            || name == "vkGetMemoryWin32HandleKHR"
            )
        {
            modifier = "internal";
        }

        return modifier;
    }

    private void WriteFunctionDeclarations(CodeWriter writer, Dictionary<string, CppFunction> commands)
    {
        foreach (KeyValuePair<string, CppFunction> command in commands)
        {
            CppFunction cppFunction = command.Value;

            string functionPointerSignature = _options.IsVulkan ? "PFN_vkVoidFunction" : GetFunctionPointerSignature(cppFunction);
            string modifier = GetFunctionModifier(command.Key);
            writer.WriteLine($"{modifier} static {functionPointerSignature} {command.Key}_ptr;");
        }

        if (commands.Count > 0)
            writer.WriteLine();
    }

    private void WriteCommandsNew(CodeWriter writer, Dictionary<string, CppFunction> commands, string addressFuncName)
    {
        foreach (KeyValuePair<string, CppFunction> instanceCommand in commands)
        {
            string commandName = instanceCommand.Key;
            if (commandName == "vkGetInstanceProcAddr" ||
                commandName == "vkGetDeviceProcAddr")
            {
                continue;
            }

            writer.WriteLine($"_{commandName} = Vulkan.{addressFuncName}(handle, \"{commandName}\"u8);");
        }
    }

    private void WriteCommands(CodeWriter writer, string name, Dictionary<string, CppFunction> commands)
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

                if (_options.IsVulkan)
                {
                    writer.WriteLine($"{commandName}_ptr = load(context, \"{commandName}\"u8);");
                }
                else
                {
                    string functionPointerSignature = GetFunctionPointerSignature(instanceCommand.Value);
                    writer.WriteLine($"{commandName}_ptr = ({functionPointerSignature}) load(context, \"{commandName}\"u8);");
                }
            }
        }
    }

    private void WriteLibraryImport(CodeWriter writer, Dictionary<string, CppFunction> commands)
    {
        using (writer.PushBlock($"private static void LoadEntries()"))
        {
            foreach (KeyValuePair<string, CppFunction> instanceCommand in commands)
            {
                string commandName = instanceCommand.Key;
                string functionPointerSignature = GetFunctionPointerSignature(instanceCommand.Value);
                writer.WriteLine($"{commandName}_ptr = ({functionPointerSignature}) NativeLibrary.GetExport(s_nativeLibrary, nameof({commandName}));");
            }
        }
    }

    private void WriteFunctionInvocation(CodeWriter writer, CppFunction cppFunction,
        bool canUseOut, bool inParameters = false)
    {
        string returnCsName = GetCsTypeName(cppFunction.ReturnType);
        string argumentsString = GetParameterSignature(cppFunction, canUseOut, inParameters);
        string modifier = "public static";
        string functionName = cppFunction.Name;

        if (cppFunction.Name == "vmaCreateAllocator" ||
            cppFunction.Name == "spvc_context_get_last_error_string" ||
            cppFunction.Name == "spvc_compiler_get_name")
        {
            modifier = "private static";
            functionName += "Private";
        }

        if (!_options.GenerateFunctionPointers)
        {
            modifier += " partial";
            writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
            writer.WriteLine($"{modifier} {returnCsName} {functionName}({argumentsString});");
        }
        else
        {
            using (writer.PushBlock($"{modifier} {returnCsName} {functionName}({argumentsString})"))
            {
                // Generate fixed statements
                int closeBlockCount = 0;

                if (inParameters)
                {
                    foreach (CppParameter cppParameter in cppFunction.Parameters)
                    {
                        string paramCsTypeName = GetCsTypeName(cppParameter.Type);
                        string paramCsName = GetParameterName(cppParameter.Name);

                        if (paramCsTypeName.Contains("CreateInfo")
                            && CanBeUsedAsInOut(cppParameter.Type, false, out _))
                        {
                            writer.BeginBlock($"fixed ({paramCsTypeName} createInfoPtr = &{paramCsName})");
                            closeBlockCount++;
                        }
                    }
                }

                if (canUseOut)
                {
                    foreach (CppParameter cppParameter in cppFunction.Parameters)
                    {
                        string paramCsTypeName = GetCsTypeName(cppParameter.Type);
                        string paramCsName = GetParameterName(cppParameter.Name);

                        if (CanBeUsedAsInOut(cppParameter.Type, true, out _))
                        {
                            if (paramCsTypeName == "IntPtr" || paramCsTypeName == "nint")
                            {
                                paramCsTypeName += "*";
                            }

                            writer.WriteLine($"Unsafe.SkipInit(out {paramCsName});");
                            writer.BeginBlock($"fixed ({paramCsTypeName} {paramCsName}Ptr = &{paramCsName})");
                            closeBlockCount++;
                        }
                    }
                }

                if (returnCsName != "void")
                {
                    writer.Write("return ");
                }

                string functionPointerSignature = GetFunctionPointerSignature(cppFunction);
                writer.Write($"(({functionPointerSignature}){cppFunction.Name}_ptr.Value)");
                writer.Write("(");

                int index = 0;
                foreach (CppParameter cppParameter in cppFunction.Parameters)
                {
                    string paramCsTypeName = GetCsTypeName(cppParameter.Type);
                    string paramCsName = GetParameterName(cppParameter.Name);

                    if (canUseOut && CanBeUsedAsInOut(cppParameter.Type, true, out _))
                    {
                        paramCsName = $"{paramCsName}Ptr";
                    }
                    else if (inParameters
                        && paramCsTypeName.Contains("CreateInfo")
                        && CanBeUsedAsInOut(cppParameter.Type, false, out _))
                    {
                        paramCsName = "createInfoPtr";
                    }

                    writer.Write($"{paramCsName}");

                    if (index < cppFunction.Parameters.Count - 1)
                    {
                        writer.Write(", ");
                    }

                    index++;
                }

                writer.WriteLine(");");

                for (int i = 0; i < closeBlockCount; i++)
                {
                    writer.EndBlock();
                }
            }
        }

        writer.WriteLine();
    }


    private void EmitInvoke(
        CodeWriter writer,
        CppFunction cppFunction,
        List<string> parameters,
        bool handleCheckResult = true,
        bool emitReturn = false)
    {
        string postCall = string.Empty;
        if (handleCheckResult)
        {
            var hasResultReturn = GetCsTypeName(cppFunction.ReturnType) == "VkResult";
            if (hasResultReturn)
            {
                postCall = ".CheckResult()";
            }
        }

        int index = 0;
        StringBuilder callArgumentStringBuilder = new();
        foreach (string? parameterName in parameters)
        {
            callArgumentStringBuilder.Append(parameterName);

            if (index < parameters.Count - 1)
            {
                callArgumentStringBuilder.Append(", ");
            }

            index++;
        }

        if (emitReturn)
        {
            string returnCsName = GetCsTypeName(cppFunction.ReturnType);
            if (returnCsName != "void")
            {
                writer.Write("return ");
            }
        }

        string callArgumentString = callArgumentStringBuilder.ToString();
        if (_options.IsVulkan)
        {
            string functionPointerSignature = GetFunctionPointerSignature(cppFunction);
            writer.Write($"(({functionPointerSignature}){cppFunction.Name}_ptr.Value)");
            writer.WriteLine($"({callArgumentString}){postCall};");
        }
        else
        {
            writer.WriteLine($"{cppFunction.Name}_ptr({callArgumentString}){postCall};");
        }
    }

    private bool IsInstanceFunction(string name)
    {
        return _instanceFunctions.Contains(name);
    }

    public static string GetParameterSignature(CppFunction cppFunction, bool canUseOut, bool inParameters)
    {
        return GetParameterSignature(cppFunction.Parameters, canUseOut, inParameters);
    }

    private static string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool inParameters)
    {
        StringBuilder argumentBuilder = new();
        int index = 0;

        foreach (CppParameter cppParameter in parameters)
        {
            string direction = string.Empty;
            string paramCsTypeName = GetCsTypeName(cppParameter.Type);
            string paramCsName = GetParameterName(cppParameter.Name);
            CppType? cppTypeDeclaration = default;

            if (canUseOut && CanBeUsedAsInOut(cppParameter.Type, true, out cppTypeDeclaration))
            {
                argumentBuilder.Append("out ");
                paramCsTypeName = GetCsTypeName(cppTypeDeclaration);
            }
            else if (inParameters
                && (paramCsTypeName.Contains("CreateInfo") || paramCsName == "getWin32HandleInfo")
                && CanBeUsedAsInOut(cppParameter.Type, false, out cppTypeDeclaration))
            {
                argumentBuilder.Append("in ");
                paramCsTypeName = GetCsTypeName(cppTypeDeclaration);
            }
            else if (canUseOut == false
                && paramCsTypeName == "nint"
                && cppParameter.Type.TypeKind == CppTypeKind.Pointer
                && paramCsName == "handle")
            {
                paramCsTypeName += "*";
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
        name = NormalizeFieldName(name);

        if (name.StartsWith('p')
            && char.IsUpper(name[1]))
        {
            name = char.ToLower(name[1]) + name.Substring(2);
            return GetParameterName(name);
        }

        if (name.StartsWith("pp")
            && char.IsUpper(name[2]))
        {
            name = char.ToLower(name[2]) + name.Substring(3);
            return GetParameterName(name);
        }

        return name;
    }

    private static bool CanBeUsedAsInOut(CppType type, bool onlyOutput, out CppType? elementTypeDeclaration)
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
            else if (pointerType.ElementType is CppPointerType cppElementPointerType
                && cppElementPointerType.SizeOf > 0)
            {
                elementTypeDeclaration = cppElementPointerType.ElementType;
                return true;
            }
            else if (pointerType.ElementType is CppPrimitiveType cppPrimitiveType
                && cppPrimitiveType.SizeOf > 0)
            {
                elementTypeDeclaration = cppPrimitiveType;
                return true;
            }
            else if (onlyOutput == false &&
                pointerType.ElementType is CppQualifiedType qualifiedType
                && qualifiedType.SizeOf > 0)
            {
                elementTypeDeclaration = qualifiedType.ElementType;
                return true;
            }
        }

        elementTypeDeclaration = null;
        return false;
    }
}
