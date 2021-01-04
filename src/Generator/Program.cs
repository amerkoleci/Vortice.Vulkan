// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using CppAst;
using Microsoft.Win32;

namespace Generator
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            string outputPath = AppContext.BaseDirectory;
            if (args.Length > 0)
            {
                outputPath = args[0];
            }

            if (File.Exists(outputPath))
            {
                Console.Error.WriteLine("The given path is a file, not a folder.");
                return 1;
            }
            else if (!Directory.Exists(outputPath))
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
                    "VK_ENABLE_BETA_EXTENSIONS"
                }
            };

            var compilation = CppParser.ParseFile(headerFile, options);

            // Print diagnostic messages
            if (compilation.HasErrors)
            {
                foreach (var message in compilation.Diagnostics.Messages)
                {
                    Console.WriteLine(message);
                }
            }

            bool generateFuncFile = false;
            if (generateFuncFile)
            {
                File.Delete("Vk.txt");
                foreach (var func in compilation.Functions)
                {
                    var signature = new System.Text.StringBuilder();
                    var argSignature = CsCodeGenerator.GetParameterSignature(func, true);
                    signature
                        .Append(func.ReturnType.GetDisplayName())
                        .Append(" ")
                        .Append(func.Name)
                        .Append("(")
                        .Append(argSignature)
                        .Append(")");
                    File.AppendAllText("Vk.txt", signature.ToString() + Environment.NewLine);
                }
            }

            CsCodeGenerator.Generate(compilation, outputPath);
            return 0;
        }

        private static IEnumerable<string> ResolveWindowsSdk(string version)
        {
            var path = @"C:\Program Files (x86)\Windows Kits\10";
            yield return $@"{path}\Include\{version}\shared";
            yield return $@"{path}\Include\{version}\um";
            yield return $@"{path}\Include\{version}\ucrt";
            yield return $@"{path}\Include\{version}\winrt";
        }
    }
}
