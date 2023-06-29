// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

public static class Program
{
    public static int Main(string[] args)
    {
        string outputPath = AppContext.BaseDirectory;
        if (args.Length > 0)
        {
            outputPath = args[0];
        }

        if (!Path.IsPathRooted(outputPath))
        {
            outputPath = Path.Combine(AppContext.BaseDirectory, outputPath);
        }

        if (!outputPath.EndsWith("Generated"))
        {
            outputPath = Path.Combine(outputPath, "Generated");
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string? headerFile = Path.Combine(AppContext.BaseDirectory, "vulkan", "vulkan.h");
        var options = new CppParserOptions
        {
            ParseMacros = true,
            Defines =
                {
                    "VK_USE_PLATFORM_ANDROID_KHR",
                    "VK_USE_PLATFORM_IOS_MVK",
                    "VK_USE_PLATFORM_MACOS_MVK",
                    "VK_USE_PLATFORM_METAL_EXT",
                    "VK_USE_PLATFORM_VI_NN",
                    //"VK_USE_PLATFORM_WAYLAND_KHR",
                    //"VK_USE_PLATFORM_WIN32_KHR",
                    //"VK_USE_PLATFORM_SCREEN_QNX",
                    "VK_ENABLE_BETA_EXTENSIONS"
                }
        };

        CppCompilation compilation = CppParser.ParseFile(headerFile, options);

        // Print diagnostic messages
        if (compilation.HasErrors)
        {
            foreach (CppDiagnosticMessage message in compilation.Diagnostics.Messages)
            {
                if (message.Type == CppLogMessageType.Error)
                {
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ForegroundColor = currentColor;
                }
            }

            return 0;
        }

        using (FileStream stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "docs", "vk.xml")))
        {
            VulkanSpecification vs = new(stream);

            CsCodeGeneratorOptions generateOptions = new()
            {
                OutputPath = outputPath,
                ClassName = "Vulkan",
                Namespace = "Vortice.Vulkan",
                PublicVisiblity = true,
                GenerateFunctionPointers = true
            };
            CsCodeGenerator.Generate(compilation, generateOptions, vs);
        }
        
        return 0;
    }
}
