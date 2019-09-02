// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static void GenerateHandles(VulkanSpecs specs, string outputPath)
        {
            // Generate Functions
            using (var writer = new CodeWriter(
                Path.Combine(outputPath, "Handles.cs"),
                "System",
                "System.Diagnostics"))
            {
                foreach (var handle in specs.Handles.Values)
                {
                    if (handle.Parent != null)
                    {
                        writer.WriteLine($"/// <summary>");
                        writer.WriteLine($"/// A {(handle.Dispatchable ? "dispatchable" : "non-dispatchable")} handle owned by a {handle.Parent}.");
                        writer.WriteLine("/// </summary>");
                    }

                    var csName = handle.Name;

                    // Remove Vk prefix.
                    var nameChanged = false;
                    if (csName.StartsWith("Vk"))
                    {
                        csName = csName.Substring(2);
                        nameChanged = true;
                    }

                    if (nameChanged)
                    {
                        AddCsMapping(handle.Name, csName);
                    }

                    writer.WriteLine("[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
                    using (writer.PushBlock($"public partial struct {csName} : IEquatable<{csName}>"))
                    {
                        string handleType = handle.Dispatchable ? "IntPtr" : "ulong";
                        string nullValue = handle.Dispatchable ? "IntPtr.Zero" : "0";

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
}
