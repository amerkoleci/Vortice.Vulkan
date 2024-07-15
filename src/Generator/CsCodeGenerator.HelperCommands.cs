// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_outArrayReturnFunctions =
    [
        "vkEnumeratePhysicalDevices",
        "vkGetPhysicalDeviceQueueFamilyProperties",
        // "vkEnumerateInstanceExtensionProperties",
        //"vkEnumerateDeviceExtensionProperties",
        "vkEnumerateInstanceLayerProperties",
        "vkEnumerateDeviceLayerProperties",

        "vkQueueSubmit",
        "vkFlushMappedMemoryRanges",
        "vkInvalidateMappedMemoryRanges",

        "vkGetImageSparseMemoryRequirements",
        "vkGetPhysicalDeviceSparseImageFormatProperties",
        "vkQueueBindSparse",
        "vkResetFences",
        "vkWaitForFences",
        //"vkGetQueryPoolResults",

        "vkMergePipelineCaches",
        "vkFreeDescriptorSets",
        //"vkUpdateDescriptorSets",

        "vkCmdSetViewport",
        "vkCmdSetScissor",
        //"vkCmdBindDescriptorSets",

        "vkBindBufferMemory2",
        "vkBindImageMemory2",
        "vkGetImageSparseMemoryRequirements2",
        "vkGetPhysicalDeviceQueueFamilyProperties2",
        "vkGetPhysicalDeviceSparseImageFormatProperties2",

        "vkGetPhysicalDeviceSurfaceFormatsKHR",
        "vkGetPhysicalDeviceSurfacePresentModesKHR",
        "vkGetSwapchainImagesKHR",
        "vkGetPhysicalDevicePresentRectanglesKHR",
        "vkGetPhysicalDeviceDisplayPropertiesKHR",
        "vkGetPhysicalDeviceDisplayPlanePropertiesKHR",
        "vkGetDisplayPlaneSupportedDisplaysKHR",
        "vkGetDisplayModePropertiesKHR",

        "vkGetPhysicalDeviceQueueFamilyProperties2KHR",
        "vkGetPhysicalDeviceSparseImageFormatProperties2KHR",
        "vkEnumeratePhysicalDeviceGroupsKHR",
    ];

    private void GenerateHelperCommands(CppCompilation compilation)
    {
        // Generate Functions
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "VkHelpers.cs"),
            false,
            _options.Namespace,
            ["System.Diagnostics", "System.Runtime.InteropServices", "System.Runtime.CompilerServices"]
            );

        using (writer.PushBlock($"unsafe partial class {_options.ClassName}"))
        {
            // Generate methods with array calls
            foreach (CppFunction function in compilation.Functions)
            {
                if (!s_outArrayReturnFunctions.Contains(function.Name))
                {
                    continue;
                }

                // Find count and array return type.
                string countParameterName = string.Empty;
                string returnArrayTypeName = string.Empty;
                string returnVariableName = string.Empty;
                List<CppParameter> newParameters = [];
                bool hasArrayReturn = false;
                int countArgumentArrayIndex = 0;

                foreach (CppParameter parameter in function.Parameters)
                {
                    if (parameter.Name.EndsWith("count", StringComparison.OrdinalIgnoreCase))
                    {
                        countParameterName = GetParameterName(parameter.Name);
                        continue;
                    }

                    if (CanBeUsedAsInOut(parameter.Type, true, out CppType? cppTypeDeclaration))
                    {
                        returnVariableName = GetParameterName(parameter.Name);
                        returnArrayTypeName = GetCsTypeName(cppTypeDeclaration);
                        hasArrayReturn = true;
                        countArgumentArrayIndex = function.Parameters.IndexOf(parameter) - 1;
                        continue;
                    }

                    if (parameter.Type is CppPointerType pointerType
                        && pointerType.ElementType is CppQualifiedType qualifiedType
                        && !string.IsNullOrEmpty(countParameterName))
                    {
                        returnVariableName = GetParameterName(parameter.Name);
                        returnArrayTypeName = GetCsTypeName(qualifiedType);
                        hasArrayReturn = false;
                        countArgumentArrayIndex = function.Parameters.IndexOf(parameter) - 1;
                        continue;
                    }

                    newParameters.Add(parameter);
                }

                string csCountParameterType = "uint";
                if (!hasArrayReturn)
                {
                    // Calls without return array.
                    string returnType = GetCsTypeName(function.ReturnType);

                    StringBuilder argumentsSingleElementBuilder = new();
                    StringBuilder argumentsReadOnlySpanBuilder = new();

                    int index = 0;
                    List<string> invokeSingleElementParameters = [];
                    List<string> invokeElementsParameters = [];

                    foreach (CppParameter cppParameter in newParameters)
                    {
                        string paramCsTypeName = GetCsTypeName(cppParameter.Type);
                        string paramCsName = GetParameterName(cppParameter.Name);
                        string argumentSignature = $"{paramCsTypeName} {paramCsName}";

                        if (index == countArgumentArrayIndex)
                        {
                            AppendCountParameter(false,
                                argumentsSingleElementBuilder, argumentsReadOnlySpanBuilder,
                                invokeSingleElementParameters, invokeElementsParameters,
                                returnArrayTypeName, returnVariableName,
                                csCountParameterType);
                        }

                        argumentsSingleElementBuilder.Append(argumentSignature);
                        argumentsReadOnlySpanBuilder.Append(argumentSignature);
                        if (index < newParameters.Count - 1)
                        {
                            argumentsSingleElementBuilder.Append(", ");
                            argumentsReadOnlySpanBuilder.Append(", ");
                        }

                        invokeSingleElementParameters.Add(paramCsName);
                        invokeElementsParameters.Add(paramCsName);
                        index++;
                    }

                    // Functions like vkFlushMappedMemoryRanges
                    if (newParameters.Count == countArgumentArrayIndex)
                    {
                        AppendCountParameter(true,
                                argumentsSingleElementBuilder, argumentsReadOnlySpanBuilder,
                                invokeSingleElementParameters, invokeElementsParameters,
                                returnArrayTypeName, returnVariableName,
                                csCountParameterType);
                    }

                    // Single element function.
                    var argumentsSingleElementString = argumentsSingleElementBuilder.ToString();
                    using (writer.PushBlock($"public static {returnType} {function.Name}({argumentsSingleElementString})"))
                    {
                        if (returnType != "void")
                        {
                            writer.Write("return ");
                        }

                        EmitInvoke(writer, function, invokeSingleElementParameters, false);
                    }

                    writer.WriteLine();

                    // ReadOnlySpan
                    var argumentsReadOnlySpanString = argumentsReadOnlySpanBuilder.ToString();
                    using (writer.PushBlock($"public static {returnType} {function.Name}({argumentsReadOnlySpanString})"))
                    {
                        using (writer.PushBlock($"fixed ({returnArrayTypeName}* {returnVariableName}Ptr = {returnVariableName})"))
                        {
                            if (returnType != "void")
                            {
                                writer.Write("return ");
                            }

                            EmitInvoke(writer, function, invokeElementsParameters, false);
                        }
                    }
                }
                else
                {
                    string argumentsString = GetParameterSignature(newParameters, false, false);
                    string extraArgs = string.Empty;
                    if (!string.IsNullOrEmpty(argumentsString))
                    {
                        extraArgs = ", ";
                    }

                    // Generate function with out only
                    // Example: public static VkResult vkEnumeratePhysicalDevices(VkInstance instance, out uint pPhysicalDeviceCount);
                    string returnType = GetCsTypeName(function.ReturnType);
                    writer.WriteLine($"[SkipLocalsInit]");
                    using (writer.PushBlock($"public static {returnType} {function.Name}({argumentsString}{extraArgs}out {csCountParameterType} {countParameterName})"))
                    {
                        writer.WriteLine($"Unsafe.SkipInit(out {countParameterName});");
                        using (writer.PushBlock($"fixed ({csCountParameterType}* {countParameterName}Ptr = &{countParameterName})"))
                        {
                            List<string> invokeParameters = new(newParameters.Select(item => GetParameterName(item.Name)))
                            {
                                $"{countParameterName}Ptr",
                                "default"
                            };
                            EmitInvoke(writer, function, invokeParameters,
                                handleCheckResult: false,
                                emitReturn: true);
                        }
                    }
                    writer.WriteLine();

                    // Generate function with Span as parameter
                    // Example: public static VkResult vkEnumeratePhysicalDevices(VkInstance instance, Span<vulkan.VkPhysicalDevice> pPhysicalDevices)
                    using (writer.PushBlock($"public static {returnType} {function.Name}({argumentsString}{extraArgs}Span<{returnArrayTypeName}> {returnVariableName})"))
                    {
                        writer.WriteLine($"{csCountParameterType} {countParameterName} = checked(({csCountParameterType}){returnVariableName}.Length);");
                        using (writer.PushBlock($"fixed ({returnArrayTypeName}* {returnVariableName}Ptr = {returnVariableName})"))
                        {
                            List<string> invokeParameters = new(newParameters.Select(item => GetParameterName(item.Name)))
                            {
                                $"&{countParameterName}",
                                $"{returnVariableName}Ptr"
                            };
                            EmitInvoke(writer, function, invokeParameters,
                                handleCheckResult: false,
                                emitReturn: true);
                        }
                    }
                    writer.WriteLine();

                    // Return ReadOnlySpan
                    returnType = $"ReadOnlySpan<{returnArrayTypeName}>";
                    using (writer.PushBlock($"public static {returnType} {function.Name}({argumentsString})"))
                    {
                        //var csCountParameterType = GetCsTypeName(countParameterType);
                        writer.WriteLine($"{csCountParameterType} {countParameterName} = 0;");

                        List<string> invokeParameters = new(newParameters.Select(item => GetParameterName(item.Name)))
                        {
                            $"&{countParameterName}",
                            "default"
                        };

                        EmitInvoke(writer, function, invokeParameters);

                        writer.WriteLine();
                        // Alloc array.
                        writer.WriteLine($"{returnType} {returnVariableName} = new {returnArrayTypeName}[{countParameterName}];");

                        // Write fixed access
                        using (writer.PushBlock($"fixed ({returnArrayTypeName}* {returnVariableName}Ptr = {returnVariableName})"))
                        {
                            invokeParameters[invokeParameters.Count - 1] = $"{returnVariableName}Ptr";
                            EmitInvoke(writer, function, invokeParameters);
                        }

                        writer.WriteLine($"return {returnVariableName};");
                    }
                }

                writer.WriteLine();
            }
        }
    }

    private static void AppendCountParameter(
        bool singleElement,
        StringBuilder argumentsSingleElementBuilder,
        StringBuilder argumentsReadOnlySpanBuilder,
        List<string> invokeSingleElementParameters,
        List<string> invokeElementsParameters,
        string returnArrayTypeName,
        string returnVariableName,
        string csCountParameterType)
    {
        var singleName = GetSingleName(returnVariableName);
        if (singleElement)
        {
            argumentsSingleElementBuilder.Append(", ");
            argumentsReadOnlySpanBuilder.Append(", ");
        }

        argumentsSingleElementBuilder
            .Append(returnArrayTypeName)
            .Append(" ")
            .Append(singleName);

        // Array invoke
        argumentsReadOnlySpanBuilder
            .Append($"ReadOnlySpan<{returnArrayTypeName}>")
            .Append(" ")
            .Append(returnVariableName);

        if (!singleElement)
        {
            argumentsSingleElementBuilder.Append(", ");
            argumentsReadOnlySpanBuilder.Append(", ");
        }

        invokeSingleElementParameters.Add("1");
        invokeSingleElementParameters.Add($"&{singleName}");
        if (csCountParameterType != "int")
        {
            invokeElementsParameters.Add($"({csCountParameterType}){returnVariableName}.Length");
        }
        else
        {
            invokeElementsParameters.Add($"{returnVariableName}.Length");
        }

        invokeElementsParameters.Add($"{returnVariableName}Ptr");
    }

    private static string GetSingleName(string name)
    {
        if (name.EndsWith("s"))
        {
            return name.Substring(0, name.Length - 1);
        }

        return name;
    }
}
