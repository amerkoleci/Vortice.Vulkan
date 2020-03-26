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
        private static void GenerateHandles(CppCompilation compilation, string outputPath)
        {
            // Generate Functions
            using var writer = new CodeWriter(Path.Combine(outputPath, "Handles.cs"),
                "System",
                "System.Diagnostics"
                );

            foreach (var typedef in compilation.Typedefs)
            {
                if (typedef.Name.StartsWith("PFN_"))
                {
                    continue;
                }

                if (!(typedef.ElementType is CppPointerType))
                {
                    continue;
                }

                var isDispatchable =
                    typedef.Name == "VkInstance" ||
                    typedef.Name == "VkPhysicalDevice" ||
                    typedef.Name == "VkDevice" ||
                    typedef.Name == "VkQueue" ||
                    typedef.Name == "VkCommandBuffer";

                var csName = typedef.Name;

                writer.WriteLine($"/// <summary>");
                writer.WriteLine($"/// A {(isDispatchable ? "dispatchable" : "non-dispatchable")} handle.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine("[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
                using (writer.PushBlock($"public partial struct {csName} : IEquatable<{csName}>"))
                {
                    string handleType = isDispatchable ? "IntPtr" : "ulong";
                    string nullValue = isDispatchable ? "IntPtr.Zero" : "0";

                    writer.WriteLine($"public readonly {handleType} Handle;");

                    writer.WriteLine($"public {csName}({handleType} handle) {{ Handle = handle; }}");
                    writer.WriteLine($"public static {csName} Null => new {csName}({nullValue});");
                    writer.WriteLine($"public static implicit operator {csName}({handleType} handle) => new {csName}(handle);");
                    writer.WriteLine($"public static bool operator ==({csName} left, {csName} right) => left.Handle == right.Handle;");
                    writer.WriteLine($"public static bool operator !=({csName} left, {csName} right) => left.Handle != right.Handle;");
                    writer.WriteLine($"public static bool operator ==({csName} left, {handleType} right) => left.Handle == right;");
                    writer.WriteLine($"public static bool operator !=({csName} left, {handleType} right) => left.Handle != right;");
                    writer.WriteLine($"public bool Equals(ref {csName} other) => Handle == other.Handle;");
                    writer.WriteLine($"public bool Equals({csName} other) => Handle == other.Handle;");
                    writer.WriteLine($"public override bool Equals(object obj) => obj is {csName} handle && Equals(ref handle);");
                    writer.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
                    writer.WriteLine($"private string DebuggerDisplay => string.Format(\"{csName} [0x{{0}}]\", Handle.ToString(\"X\"));");
                }

                writer.WriteLine();
            }
        }
    }
}
