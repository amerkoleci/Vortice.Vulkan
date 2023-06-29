// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Generator;

public static partial class CsCodeGenerator
{
    private static void GenerateFormatHelpers(VulkanSpecification specification)
    {
        // Generate Functions
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "VkFormatUtils.cs"),
            false,
            _options.Namespace,
            new string[] { }
            );

        using (writer.PushBlock($"unsafe partial class {_options.ClassName}"))
        {
            using (writer.PushBlock($"public static (int X, int Y, int Z) BlockExtent(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (format.BlockExtentX == 1 && format.BlockExtentY == 1 && format.BlockExtentZ == 1)
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return ({format.BlockExtentX}, {format.BlockExtentY}, {format.BlockExtentZ});");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return (1, 1, 1);");
                }
            }

            using (writer.PushBlock($"public static int BlockSize(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return {format.BlockSize};");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 0;");
                }
            }

            using (writer.PushBlock($"public static int TexelsPerBlock(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return {format.TexelsPerBlock};");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 0;");
                }
            }

            using (writer.PushBlock($"public static string CompatibilityClass(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return \"{format.Class}\";");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return string.Empty;");
                }
            }

            using (writer.PushBlock($"public static int ComponentCount(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return {format.Components.Length};");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 0;");
                }
            }

            using (writer.PushBlock($"public static byte ComponentBits(this VkFormat format, int component)"))
            {
                using (writer.PushBlock($"switch (format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (!string.IsNullOrEmpty(format.Compressed))
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}:");

                        writer.Indent();
                        using (writer.PushBlock($"switch (component)"))
                        {
                            int componentIndex = 0;
                            foreach(FormatComponent component in format.Components)
                            {
                                writer.WriteLine($"case {componentIndex++}: return {component.Bits};");
                            }

                            writer.WriteLine("default: return 0;");
                        }
                        writer.Dedent();
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 0;");
                }
            }

            using (writer.PushBlock($"public static bool ComponentsAreCompressed(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (string.IsNullOrEmpty(format.Compressed))
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}:");
                    }

                    writer.Indent();
                    writer.WriteLine($"return true;");
                    writer.Dedent();

                    writer.WriteLine();
                    writer.WriteLine("default:");
                    writer.Indent();
                    writer.WriteLine($"return false;");
                    writer.Dedent();
                }
            }

            using (writer.PushBlock($"public static string CompressionScheme(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (string.IsNullOrEmpty(format.Compressed))
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return \"{format.Compressed}\";");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default:");
                    writer.Indent();
                    writer.WriteLine($"return string.Empty;");
                    writer.Dedent();
                }
            }

            using (writer.PushBlock($"public static byte Packed(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (!format.Packed.HasValue)
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return {format.Packed};");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 0;");
                }
            }

            using (writer.PushBlock($"public static VkFormat PlaneCompatibleFormat(this VkFormat format, int plane)"))
            {
                using (writer.PushBlock($"switch (format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (format.Planes.Length == 0)
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}:");

                        writer.Indent();
                        using (writer.PushBlock($"switch (plane)"))
                        {
                            foreach (FormatPlane plane in format.Planes)
                            {
                                string compatibleItemName = GetEnumItemName("VkFormat", plane.Compatible, "VK_FORMAT");
                                writer.WriteLine($"case {plane.Index}: return VkFormat.{compatibleItemName};");
                            }

                            writer.WriteLine("default: return 0;");
                        }
                        writer.Dedent();
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return format;");
                }
            }

            using (writer.PushBlock($"public static byte PlaneCount(this VkFormat format)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (format.Planes.Length == 0)
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}: return {format.Planes.Length};");
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 1;");
                }
            }

            using (writer.PushBlock($"public static byte PlaneWidthDivisor(this VkFormat format, int plane)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (format.Planes.Length == 0)
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}:");

                        writer.Indent();
                        using (writer.PushBlock($"switch (plane)"))
                        {
                            foreach (FormatPlane plane in format.Planes)
                            {
                                writer.WriteLine($"case {plane.Index}: return {plane.WidthDivisor};");
                            }

                            writer.WriteLine("default: return 1;");
                        }
                        writer.Dedent();
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 1;");
                }
            }

            using (writer.PushBlock($"public static byte PlaneHeightDivisor(this VkFormat format, int plane)"))
            {
                using (writer.PushBlock($"switch(format)"))
                {
                    foreach (FormatDefinition format in specification.Formats)
                    {
                        if (format.Planes.Length == 0)
                            continue;

                        string enumItemName = GetEnumItemName("VkFormat", format.Name, "VK_FORMAT");
                        writer.WriteLine($"case VkFormat.{enumItemName}:");

                        writer.Indent();
                        using (writer.PushBlock($"switch (plane)"))
                        {
                            foreach (FormatPlane plane in format.Planes)
                            {
                                writer.WriteLine($"case {plane.Index}: return {plane.HeightDivisor};");
                            }

                            writer.WriteLine("default: return 1;");
                        }
                        writer.Dedent();
                    }

                    writer.WriteLine();
                    writer.WriteLine("default: return 1;");
                }
            }
        }
    }
}
