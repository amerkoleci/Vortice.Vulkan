// Copyright (c) Amer Koleci and Contributors.
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

        string? headerFile;
        CppParserOptions parserOptions;
        CsCodeGeneratorOptions generateOptions;
        bool isVulkan = false;

        if (outputPath.Contains("Vortice.SPIRV.Reflect"))
        {
            headerFile = Path.Combine(AppContext.BaseDirectory, "headers", "spirv_reflect.h");

            parserOptions = new()
            {
                ParseMacros = true,
                IncludeFolders =
                {
                    Path.Combine(AppContext.BaseDirectory, "headers")
                },
                Defines =
                {
                    "_ALLOW_COMPILER_AND_STL_VERSION_MISMATCH"
                },
            };

            generateOptions = new()
            {
                OutputPath = outputPath,
                ClassName = "SPIRVReflectApi",
                Namespace = "Vortice.SPIRV.Reflect",
                PublicVisiblity = true,
                GenerateFunctionPointers = false,
                ExtraUsings =
                {
                    "Vortice.SPIRV"
                }
            };
        }
        else if (outputPath.Contains("Vortice.SPIRV"))
        {
            headerFile = Path.Combine(AppContext.BaseDirectory, "headers", "spirv.h");

            parserOptions = new()
            {
                ParseMacros = true
            };

            generateOptions = new()
            {
                OutputPath = outputPath,
                ClassName = "Spv",
                Namespace = "Vortice.SPIRV",
                PublicVisiblity = true,
            };
        }
        else if (outputPath.Contains("Vortice.VulkanMemoryAllocator"))
        {
            headerFile = Path.Combine(AppContext.BaseDirectory, "headers", "vk_mem_alloc.h");

            parserOptions = new()
            {
                ParseMacros = true,
                SystemIncludeFolders =
                {
                    Path.Combine(AppContext.BaseDirectory)
                }
            };

            generateOptions = new()
            {
                OutputPath = outputPath,
                ClassName = "Vma",
                Namespace = "Vortice.Vulkan",
                PublicVisiblity = true,
                GenerateFunctionPointers = false,
            };
        }
        else if (outputPath.Contains("Vortice.SpirvCross"))
        {
            headerFile = Path.Combine(AppContext.BaseDirectory, "headers", "spirv_cross_c.h");

            parserOptions = new()
            {
                ParseMacros = true,
                SystemIncludeFolders =
                {
                    Path.Combine(AppContext.BaseDirectory, "headers")
                }
            };

            generateOptions = new()
            {
                OutputPath = outputPath,
                ClassName = "SpirvCrossApi",
                Namespace = "Vortice.SpirvCross",
                PublicVisiblity = true,
                GenerateFunctionPointers = false,
                ExtraUsings =
                {
                    "Vortice.SPIRV"
                }
            };
        }
        else
        {
            isVulkan = true;
            headerFile = Path.Combine(AppContext.BaseDirectory, "vulkan", "vulkan_volk.h");
            parserOptions = new()
            {
                ParseMacros = true,
                AutoSquashTypedef = true,
                Defines =
                {
                    //"VK_NO_PROTOTYPES",
                    "VK_USE_PLATFORM_ANDROID_KHR",
                    //"VK_USE_PLATFORM_IOS_MVK", // Deprecated
                    //"VK_USE_PLATFORM_MACOS_MVK",  // Deprecated
                    "VK_USE_PLATFORM_METAL_EXT",
                    "VK_USE_PLATFORM_VI_NN",
                    "VK_USE_PLATFORM_WIN32_KHR",
                    "VK_USE_PLATFORM_XCB_KHR",
                    "VK_USE_PLATFORM_XLIB_KHR",
                    "VK_USE_PLATFORM_WAYLAND_KHR",
                    //"VK_USE_PLATFORM_SCREEN_QNX",
                    "VK_ENABLE_BETA_EXTENSIONS"
                }
            };

            generateOptions = new()
            {
                OutputPath = outputPath,
                ClassName = "Vulkan",
                Namespace = "Vortice.Vulkan",
                PublicVisiblity = true,
                GenerateFunctionPointers = true,
                IsVulkan = true,
                ReadOnlyMemoryUtf8ForStrings = true // TODO: Enable
            };
        }

        if (OperatingSystem.IsWindows())
        {
            //parserOptions.ConfigureForWindowsMsvc(CppTargetCpu.X86_64, CppVisualStudioVersion.VS2022);

            //@"C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0"
            parserOptions.SystemIncludeFolders.AddRange(SdkResolver.ResolveStdLib());

            // Windows Sdk candidates 10.0.22621.0, 10.0.26100.0
            List<string> sdkPaths = SdkResolver.ResolveWindowsSdk("10.0.26100.0");
            if (sdkPaths.Count > 0)
            {
                parserOptions.SystemIncludeFolders.AddRange(sdkPaths);
            }
            else
            {
                sdkPaths = SdkResolver.ResolveWindowsSdk("10.0.22621.0");
                if (sdkPaths.Count > 0)
                {
                    parserOptions.SystemIncludeFolders.AddRange(sdkPaths);
                }
            }
        }

        CppCompilation compilation = CppParser.ParseFile(headerFile, parserOptions);

        // Print diagnostic messages
        if (compilation.HasErrors)
        {
            foreach (CppDiagnosticMessage message in compilation.Diagnostics.Messages)
            {
                if (message.Type == CppLogMessageType.Error)
                {
                    ConsoleColor currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ForegroundColor = currentColor;
                }
            }

            return 0;
        }

        if (isVulkan)
        {
            using (FileStream stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "docs", "vk.xml")))
            {
                VulkanSpecification vs = new(stream);
                CsCodeGenerator generator = new(generateOptions, vs);
                generator.Generate(compilation);
            }
        }
        else
        {
            CsCodeGenerator generator = new(generateOptions);
            generator.Generate(compilation);
        }

        return 0;
    }
}
