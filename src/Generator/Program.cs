// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using CppAst;

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

            var headerFile = Path.Combine(AppContext.BaseDirectory, "vulkan", "vulkan.h");
            var options = new CppParserOptions
            {
                ParseMacros = true
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

            CsCodeGenerator.Generate(compilation, outputPath);
            return 0;
        }

    }
}
