// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static readonly HashSet<string> s_outArrayReturnFunctions = new HashSet<string>
        {
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
        };

        private static void GenerateHelperCommands(CppCompilation compilation, string outputPath)
        {
            // Generate Functions
            using var writer = new CodeWriter(Path.Combine(outputPath, "VkHelpers.cs"),
                "System",
                "System.Diagnostics",
                "System.Runtime.InteropServices",
                "Vortice.Mathematics"
                );

            using (writer.PushBlock($"unsafe partial class Vulkan"))
            {
                // Generate methods with array calls
                foreach (var function in compilation.Functions)
                {
                    if (!s_outArrayReturnFunctions.Contains(function.Name))
                    {
                        continue;
                    }

                    if (function.Name == "vkGetPhysicalDeviceSurfacePresentModesKHR")
                    {

                    }

                    // Find count and array return type.
                    var countParameterName = string.Empty;
                    var returnArrayTypeName = string.Empty;
                    var returnVariableName = string.Empty;
                    var newParameters = new List<CppParameter>();
                    var hasArrayReturn = false;
                    var countArgumentArrayIndex = 0;

                    foreach (var parameter in function.Parameters)
                    {
                        if (parameter.Name.EndsWith("count", StringComparison.OrdinalIgnoreCase))
                        {
                            countParameterName = GetParameterName(parameter.Name);
                            continue;
                        }

                        if (CanBeUsedAsOutput(parameter.Type, out var cppTypeDeclaration))
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

                    var csCountParameterType = "uint";
                    if (!hasArrayReturn)
                    {
                        // Calls without return array.
                        var returnType = GetCsTypeName(function.ReturnType);

                        var argumentsSingleElementBuilder = new StringBuilder();
                        var argumentsReadOnlySpanBuilder = new StringBuilder();
                        var index = 0;

                        var invokeSingleElementParameters = new List<string>();
                        var invokeElementsParameters = new List<string>();

                        foreach (var cppParameter in newParameters)
                        {
                            var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                            var paramCsName = GetParameterName(cppParameter.Name);
                            var argumentSignature = $"{paramCsTypeName} {paramCsName}";

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
                        var argumentsString = GetParameterSignature(newParameters, false);
                        var returnType = $"ReadOnlySpan<{returnArrayTypeName}>";

                        using (writer.PushBlock($"public static {returnType} {function.Name}({argumentsString})"))
                        {
                            //var csCountParameterType = GetCsTypeName(countParameterType);
                            writer.WriteLine($"{csCountParameterType} {countParameterName} = 0;");

                            var invokeParameters = new List<string>(newParameters.Select(item => GetParameterName(item.Name)))
                            {
                                $"&{countParameterName}",
                                "null"
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
            invokeElementsParameters.Add($"({csCountParameterType}){returnVariableName}.Length");
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
}
