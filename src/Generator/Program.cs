// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;

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

            var specFile = Path.Combine(AppContext.BaseDirectory, "vk.xml");
            var spec = new VulkanSpecs(specFile);
            CsCodeGenerator.Generate(spec, outputPath);
            return 0;
        }
    }
}
