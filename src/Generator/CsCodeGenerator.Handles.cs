﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_ignoredHandles = new(StringComparer.OrdinalIgnoreCase)
    {
        //"VkInstance",
        "MTLDevice_id",
        "MTLCommandQueue_id",
        "MTLBuffer_id",
        "MTLTexture_id",
        "MTLSharedEvent_id",
        "IOSurfaceRef",
    };

    private void GenerateHandles(CppCompilation compilation)
    {
        bool hasAnyHandleType = false;
        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (typedef.Name.StartsWith("PFN_") ||
                typedef.Name == "spvc_error_callback" ||
                s_ignoredHandles.Contains(typedef.Name))
            {
                continue;
            }

            string sourceFileName = Path.GetFileNameWithoutExtension(typedef.SourceFile);
            if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                continue;

            if (typedef.ElementType is not CppPointerType)
            {
                continue;
            }

            hasAnyHandleType = true;
            break;
        }

        if (!hasAnyHandleType)
            return;

        string visibility = _options.PublicVisiblity ? "public" : "internal";

        // Generate Functions
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Handles.cs"),
            true,
            _options.Namespace,
            ["System.Diagnostics"]
            );

        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (typedef.Name.StartsWith("PFN_") ||
                typedef.Name == "spvc_error_callback" ||
                s_ignoredHandles.Contains(typedef.Name))
            {
                continue;
            }

            string sourceFileName = Path.GetFileNameWithoutExtension(typedef.SourceFile);
            if (ShouldIgnoreFile(sourceFileName, _options.IsVulkan))
                continue;

            if (typedef.ElementType is not CppPointerType)
            {
                continue;
            }

            bool isDispatchable =
                typedef.Name == "VkInstance" ||
                typedef.Name == "VkPhysicalDevice" ||
                typedef.Name == "VkDevice" ||
                typedef.Name == "VkQueue" ||
                typedef.Name == "VkCommandBuffer" ||
                typedef.Name == "VmaAllocator" ||
                typedef.Name == "VmaPool" ||
                typedef.Name == "VmaAllocation" ||
                typedef.Name == "VmaDefragmentationContext" ||
                typedef.Name == "VmaVirtualBlock";

            string csName = typedef.Name;

            if (!_options.IsVulkan)
            {
                if (csName != "VmaVirtualAllocation" && typedef.ElementType is CppPointerType)
                {
                    isDispatchable = true;
                }
            }

            if (_options.IsVulkan)
            {
                writer.WriteLine($"/// <summary>");
                writer.WriteLine($"/// A {(isDispatchable ? "dispatchable" : "non-dispatchable")} handle.");
                writer.WriteLine("/// </summary>");
            }

            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            using (writer.PushBlock($"{visibility} readonly partial struct {csName} : IEquatable<{csName}>"))
            {
                string handleType = isDispatchable ? "nint" : "ulong";
                string nullValue = "0";

                writer.WriteLine($"public {csName}({handleType} handle) {{ Handle = handle; }}");
                writer.WriteLine($"public {handleType} Handle {{ get; }}");
                writer.WriteLine($"public bool IsNull => Handle == 0;");
                writer.WriteLine($"public bool IsNotNull => Handle != 0;");

                writer.WriteLine($"public static {csName} Null => new({nullValue});");
                writer.WriteLine($"public static implicit operator {csName}({handleType} handle) => new(handle);");
                writer.WriteLine($"public static implicit operator {handleType}({csName} handle) => handle.Handle;");
                writer.WriteLine($"public static bool operator ==({csName} left, {csName} right) => left.Handle == right.Handle;");
                writer.WriteLine($"public static bool operator !=({csName} left, {csName} right) => left.Handle != right.Handle;");
                writer.WriteLine($"public static bool operator ==({csName} left, {handleType} right) => left.Handle == right;");
                writer.WriteLine($"public static bool operator !=({csName} left, {handleType} right) => left.Handle != right;");
                writer.WriteLine($"public bool Equals({csName} other) => Handle == other.Handle;");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals(object? obj) => obj is {csName} handle && Equals(handle);");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
                writer.WriteLine($"private string DebuggerDisplay => $\"{{nameof({csName})}} [0x{{Handle.ToString(\"X\")}}]\";");
            }

            writer.WriteLine();
        }
    }
}
