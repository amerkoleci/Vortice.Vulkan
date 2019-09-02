// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.IO;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static void GenerateCommands(VulkanSpecs specs, string outputPath)
        {
            // Generate Functions
            using (var writer = new CodeWriter(
                Path.Combine(outputPath, "Commands.cs"),
                "System",
                "System.Diagnostics"))
            {
                
            }
        }
    }
}
