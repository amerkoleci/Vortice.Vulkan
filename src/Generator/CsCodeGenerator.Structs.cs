// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.IO;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static void GenerateStructAndUnions(VulkanSpecs specs, string outputPath)
        {
            // Generate Structures
            using (var writer = new CodeWriter(
                Path.Combine(outputPath, "Structures.cs"),
                "System",
                "System.Runtime.InteropServices"))
            {
                foreach (var structDef in specs.StructuresAndUnions.Values)
                {
                    // Skip unsupported types.
                    if (structDef.IsUnion
                        || structDef.Name == "VkBaseOutStructure"
                        || structDef.Name == "VkBaseInStructure"
                        || structDef.Name == "VkAllocationCallbacks"
                        || structDef.Name == "VkImagePipeSurfaceCreateInfoFUCHSIA"
                        || structDef.Name == "VkViSurfaceCreateInfoNN"
                        || structDef.Name == "VkNativeBufferANDROID"
                        || structDef.Name == "VkMirSurfaceCreateInfoKHR")
                    {
                        continue;
                    }

                    if (!structDef.IsBlittable)
                        continue;

                    var csName = structDef.Name;
                    var access = structDef.IsBlittable ? "public" : "internal unsafe";
                    var nameChanged = false;
                    // Remove Vk prefix if direct marshable.
                    if (csName.StartsWith("Vk"))
                    {
                        csName = csName.Substring(2);
                        nameChanged = true;
                    }

                    if (nameChanged)
                    {
                        AddCsMapping(structDef.Name, csName);
                    }

                    //var hasFreeMembers = false;
                    //foreach (var member in structDef.Members)
                    //{
                    //    if (member.Name == "pNext"
                    //        || member.Name == "pUserData"
                    //        || !IsPointer(member.Type))
                    //    {
                    //        continue;
                    //    }

                    //    hasFreeMembers = true;
                    //    break;
                    //}

                    //writer.WriteLine("[StructLayout(LayoutKind.Sequential)]");
                    using (writer.PushBlock($"{access} partial struct {csName}"))
                    {
                        if (structDef.IsBlittable)
                        {
                            WriteStructureMembers(specs, writer, structDef, false);
                        }
                        else
                        {
                            WriteStructureMembers(specs, writer, structDef, true);
                        }

                        //var hasFixedFields = false;
                        //foreach (var member in structDef.Members)
                        //{
                        //    if (string.IsNullOrEmpty(member.ElementCountSymbolic))
                        //        continue;

                        //    if (specs.Constants.TryGetValue(
                        //        member.ElementCountSymbolic, out var constant)
                        //        && CanUseFixed(member.Type))
                        //    {
                        //        hasFixedFields = true;
                        //    }
                        //}

                        //if (!structDef.IsMarshable)
                        //{
                        //    writer.WriteLine();
                        //    writer.WriteLine("[StructLayout(LayoutKind.Sequential)]");
                        //    using (writer.PushBlock("internal unsafe struct Native"))
                        //    {
                        //        WriteStructureMembers(specs, writer, structDef, true);

                        //        var freeMembers = new List<VulkanMemberDefinition>();
                        //        foreach (var member in structDef.Members)
                        //        {
                        //            if (member.Name == "pNext"
                        //                || member.Name == "pUserData"
                        //                || !IsPointer(member.Type))
                        //            {
                        //                continue;
                        //            }

                        //            freeMembers.Add(member);
                        //        }

                        //        if (freeMembers.Count > 0)
                        //        {
                        //            writer.WriteLine();
                        //            using (writer.PushBlock("public void Free()"))
                        //            {
                        //                foreach (var member in freeMembers)
                        //                {
                        //                    if (member.IsOptional
                        //                        && specs.StructuresAndUnions.TryGetValue(member.Type.Name, out var memberStructDef)
                        //                        && !memberStructDef.IsMarshable)
                        //                    {
                        //                        if (hasFreeMembers)
                        //                        {
                        //                            using (writer.PushBlock($"if ({member.Name} != null)"))
                        //                            {
                        //                                writer.WriteLine($"{member.Name}->Free();");
                        //                                writer.WriteLine($"Interop.Free({member.Name});");
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            writer.WriteLine($"Interop.Free({member.Name});");
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        writer.WriteLine($"Interop.Free({member.Name});");
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }

                        //    if (!hasFixedFields)
                        //    {
                        //        // Create marshal methods.
                        //        writer.WriteLine();
                        //        using (writer.PushBlock("internal unsafe void ToNative(out Native native)"))
                        //        {
                        //            foreach (var member in structDef.Members)
                        //            {
                        //                var memberTypeName = member.Type.Name;
                        //                if (memberTypeName.StartsWith("Vk"))
                        //                {
                        //                    memberTypeName = memberTypeName.Substring(2);
                        //                }

                        //                VulkanMemberDefinition lengthMember = null;
                        //                if (member.IsLength)
                        //                {
                        //                    lengthMember = structDef.GetLengthMember(member);
                        //                }

                        //                if (member.Value != null)
                        //                {
                        //                    writer.WriteLine($"native.{member.Name} = {memberTypeName}.{GetDefaultValueString(member.Type, member.Value)};");
                        //                }
                        //                else
                        //                {
                        //                    var mappedTypeSpec = GetCsTypeSpecification(member.Type);
                        //                    var mappedCsTypeName = mappedTypeSpec.ToString();
                        //                    var csFieldName = GetCsFieldName(member);
                        //                    var nativeFieldName = NormalizeFieldName(member.Name);

                        //                    var marshalFunction = csFieldName;
                        //                    if (mappedCsTypeName == "void*")
                        //                    {
                        //                        marshalFunction = $"{csFieldName}";
                        //                    }
                        //                    else if (mappedCsTypeName == "byte*")
                        //                    {
                        //                        marshalFunction = $"Interop.AllocStringToPointer({csFieldName})";
                        //                    }
                        //                    else if (mappedCsTypeName == "byte**")
                        //                    {
                        //                        marshalFunction = $"Interop.AllocStringToPointers({csFieldName})";
                        //                    }
                        //                    else if (mappedCsTypeName == "float*")
                        //                    {
                        //                        marshalFunction = $"Interop.AllocStructToPointer({csFieldName})";
                        //                    }
                        //                    else if (specs.Handles.TryGetValue(member.Type.Name, out var handle))
                        //                    {
                        //                        if (member.Type.PointerIndirection == 0)
                        //                        {
                        //                            marshalFunction = $"{csFieldName}.Handle";
                        //                        }
                        //                        else if (member.Type.PointerIndirection == 1)
                        //                        {
                        //                            marshalFunction = $"Interop.AllocStructToPointer({csFieldName})";
                        //                        }
                        //                        else
                        //                        {
                        //                            throw new NotImplementedException();
                        //                        }
                        //                    }
                        //                    else if (member.Type.PointerIndirection == 1)
                        //                    {
                        //                        if (member.IsOptional)
                        //                        {
                        //                            if (specs.Handles.TryGetValue(member.Type.Name, out var shandle))
                        //                            {
                        //                            }
                        //                            else if (specs.StructuresAndUnions.TryGetValue(member.Type.Name, out var memberStructDef))
                        //                            {
                        //                                if (memberStructDef.IsMarshable)
                        //                                {
                        //                                    if (string.IsNullOrEmpty(member.LengthMemberName))
                        //                                    {
                        //                                        marshalFunction = $"Interop.AllocStructToPointer(ref {csFieldName})";
                        //                                    }
                        //                                    else
                        //                                    {
                        //                                        marshalFunction = $"Interop.AllocStructToPointer({csFieldName})";
                        //                                    }
                        //                                }
                        //                                else
                        //                                {
                        //                                    using (writer.PushBlock($"if ({csFieldName}.HasValue)"))
                        //                                    {
                        //                                        writer.WriteLine($"{csFieldName}.Value.ToNative(out var {nativeFieldName}Native);");
                        //                                        writer.WriteLine($"native.{nativeFieldName} = &{nativeFieldName}Native;");
                        //                                    }
                        //                                    using (writer.PushBlock($"else"))
                        //                                    {
                        //                                        writer.WriteLine($"native.{nativeFieldName} = null;");
                        //                                    }

                        //                                    continue;
                        //                                }
                        //                            }
                        //                            else
                        //                            {
                        //                                marshalFunction = "null";
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            marshalFunction = $"Interop.AllocStructToPointer({csFieldName})";
                        //                        }
                        //                    }

                        //                    if (lengthMember != null)
                        //                    {
                        //                        var lengthMemberCsName = GetCsFieldName(lengthMember);
                        //                        marshalFunction = $"{lengthMemberCsName} != null ? (uint){lengthMemberCsName}.Length : 0";
                        //                    }

                        //                    writer.WriteLine($"native.{nativeFieldName} = {marshalFunction};");
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    writer.WriteLine();
                }
            }
        }

    }
}
