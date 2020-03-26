// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static readonly HashSet<string> s_keywords = new HashSet<string>
        {
            "object",
            "event",
        };

        private static readonly HashSet<string> s_ignoredParts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "flags",
            "bit"
        };

        private static readonly HashSet<string> s_preserveCaps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "khr",
            "khx",
            "ext",
            "nv",
            "nvx",
            "amd",
        };

        private static readonly HashSet<string> s_fixedCapableTypes = new HashSet<string>()
        {
            "byte", "short", "int", "long", "char", "sbyte", "ushort", "uint", "ulong", "float", "double"
        };

        private static readonly Dictionary<string, string> s_csNameMappings = new Dictionary<string, string>()
        {
            { "uint8_t", "byte" },
            { "uint16_t", "ushort" },
            { "uint32_t", "uint" },
            { "uint64_t", "ulong" },
            { "int8_t", "sbyte" },
            { "int32_t", "int" },
            { "int16_t", "short" },
            { "int64_t", "long" },
            { "int64_t*", "long*" },
            { "char", "byte" },
            { "size_t", "UIntPtr" },
            { "DWORD", "uint" },

            { "VkFlags", "uint" },
            { "VkDeviceSize", "ulong" },
            { "VkSampleMask", "uint" },

            { "buffer_handle_t", "IntPtr" },
            { "AHardwareBuffer","IntPtr" },
            { "ANativeWindow", "IntPtr" },

            { "MirConnection", "IntPtr" },
            { "MirSurface", "IntPtr" },

            { "wl_display", "IntPtr" },
            { "wl_surface", "IntPtr" },

            { "Display", "IntPtr" },
            { "Window", "IntPtr" },
            { "VisualID", "IntPtr" },
            { "RROutput", "IntPtr" },

            { "HINSTANCE", "IntPtr" },
            { "HWND", "IntPtr" },
            { "HANDLE", "IntPtr" },
            { "SECURITY_ATTRIBUTES", "IntPtr" },
            { "LPCWSTR", "IntPtr" },
            { "HMONITOR", "IntPtr" },

            { "xcb_connection_t", "IntPtr" },
            { "xcb_window_t", "IntPtr" },
            { "xcb_visualid_t", "IntPtr" },

            { "CAMetalLayer", "IntPtr" },
            { "GgpFrameToken", "IntPtr" },
            { "GgpStreamDescriptor", "IntPtr" },

            { "VkMemoryRequirements2KHR", "VkMemoryRequirements2" },

            { "VkAccelerationStructureTypeNV", "VkAccelerationStructureTypeKHR" },
            { "VkAccelerationStructureMemoryRequirementsTypeNV", "VkAccelerationStructureMemoryRequirementsTypeKHR" },
            { "VkAccelerationStructureNV", "VkAccelerationStructureKHR" },

            // TODO: Until we marshal functions
            { "VkDeviceAddress", "IntPtr" },
        };

        public static void Generate(CppCompilation compilation, string outputPath)
        {
            GenerateConstants(compilation, outputPath);
            GenerateEnums(compilation, outputPath);
            GenerateHandles(compilation, outputPath);
            GenerateStructAndUnions(compilation, outputPath);
            GenerateCommands(compilation, outputPath);
        }

        public static void AddCsMapping(string typeName, string csTypeName)
        {
            s_csNameMappings[typeName] = csTypeName;
        }

        private static void GenerateConstants(CppCompilation compilation, string outputPath)
        {
            using var writer = new CodeWriter(Path.Combine(outputPath, "Constants.cs"));
            writer.WriteLine("/// <summary>");
            writer.WriteLine("/// Provides Vulkan specific constants for special values, layer names and extension names.");
            writer.WriteLine("/// </summary>");
            using (writer.PushBlock("public static partial class Vulkan"))
            {
                foreach (var cppMacro in compilation.Macros)
                {
                    if (string.IsNullOrEmpty(cppMacro.Value)
                        || cppMacro.Name.EndsWith("_H_", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VKAPI_CALL", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VKAPI_PTR", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VULKAN_CORE_H_", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VK_TRUE", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VK_FALSE", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VK_MAKE_VERSION", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.StartsWith("VK_HEADER_VERSION", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.StartsWith("VK_VERSION_", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.StartsWith("VK_API_VERSION_", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VK_NULL_HANDLE", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VK_DEFINE_HANDLE", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.Equals("VK_DEFINE_NON_DISPATCHABLE_HANDLE", StringComparison.OrdinalIgnoreCase)
                        || cppMacro.Name.StartsWith("VK_USE_PLATFORM_", StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        continue;
                    }

                    var csName = GetPrettyEnumName(cppMacro.Name, "VK_");
                    var csDataType = "string";
                    var macroValue = NormalizeEnumValue(cppMacro.Value);
                    if (macroValue.EndsWith("F", StringComparison.OrdinalIgnoreCase))
                    {
                        csDataType = "float";
                    }
                    else if (macroValue.EndsWith("U", StringComparison.OrdinalIgnoreCase))
                    {
                        csDataType = "uint";
                    }
                    else if (macroValue.EndsWith("LL", StringComparison.OrdinalIgnoreCase))
                    {
                        csDataType = "ulong";
                    }
                    else if (uint.TryParse(macroValue, out _))
                    {
                        csDataType = "uint";
                    }

                    if (cppMacro.Name == "VK_QUEUE_FAMILY_EXTERNAL"
                        || cppMacro.Name == "VK_QUEUE_FAMILY_FOREIGN_EXT")
                    {
                        csDataType = "uint";
                    }
                    else if (cppMacro.Name == "VK_LUID_SIZE_KHR"
                        || cppMacro.Name == "VK_SHADER_UNUSED_NV"
                        || cppMacro.Name == "VK_QUEUE_FAMILY_EXTERNAL_KHR"
                        || cppMacro.Name == "VK_MAX_DRIVER_NAME_SIZE_KHR"
                        || cppMacro.Name == "VK_MAX_DRIVER_INFO_SIZE_KHR"
                        || cppMacro.Name == "VK_MAX_DEVICE_GROUP_SIZE_KHR"
                        )
                    {
                        csDataType = "uint";
                        macroValue = GetCsTypeName(cppMacro.Value);
                    }

                    AddCsMapping(cppMacro.Name, csName);

                    writer.WriteLine("/// <summary>");
                    writer.WriteLine($"/// {cppMacro.Name} = {cppMacro.Value}");
                    writer.WriteLine("/// </summary>");
                    writer.WriteLine($"public const {csDataType} {csName} = {macroValue};");
                }
            }
        }

        private static string GetCsFieldName(VulkanMemberDefinition member)
        {
            var csFieldName = NormalizeFieldName(member.Name);
            csFieldName = ToUpperCamelCase(csFieldName);

            if (IsPointer(member.Type))
            {
                while (csFieldName.StartsWith("p", StringComparison.OrdinalIgnoreCase))
                {
                    csFieldName = csFieldName.Substring(1);
                }
            }

            return csFieldName;
        }

        private static bool IsPointer(VulkanTypeSpecification typeSpecification)
        {
            var mappedTypeSpec = GetCsTypeSpecification(typeSpecification);
            var mappedCsTypeName = mappedTypeSpec.ToString();

            if (mappedCsTypeName == "void*"
                || mappedCsTypeName == "byte*"
                || mappedCsTypeName == "byte**"
                || mappedCsTypeName == "float*")
            {
                return true;
            }
            else if (typeSpecification.PointerIndirection > 0)
            {
                return true;
            }

            return false;
        }

        private static void WriteStructureMembers(
            VulkanSpecs specs,
            CodeWriter writer,
            VulkanStructDefinition structDef,
            bool unsafeWrite)
        {
            foreach (var member in structDef.Members)
            {
                if (!unsafeWrite)
                {
                    //if (member.Name == "sType"
                    //    || member.IsLength)
                    //{
                    //    continue;
                    //}

                    if (member.Comment != null)
                    {
                        writer.WriteLine("/// <summary>");
                        writer.WriteLine($"/// {member.Comment}");
                        writer.WriteLine("/// </summary>");
                    }
                }


                if (member.ElementCount > 1)
                {
                    for (int i = 0; i < member.ElementCount; i++)
                    {
                        WriteMember(specs, writer, member, unsafeWrite, "_" + i);
                    }
                }
                else if (member.ElementCountSymbolic != null)
                {
                    if (specs.Constants.TryGetValue(member.ElementCountSymbolic, out var constant))
                    {
                        WriteMemberSymbolicCount(specs, writer, member, constant, unsafeWrite);
                    }
                    else
                    {
                        WriteMember(specs, writer, member, unsafeWrite, string.Empty);
                    }
                }
                else
                {
                    WriteMember(specs, writer, member, unsafeWrite, string.Empty);
                }
            }
        }


        private static void WriteMember(
            VulkanSpecs specs,
            CodeWriter writer,
            VulkanMemberDefinition member,
            bool unsafeWrite,
            string nameSuffix)
        {
            var fieldName = NormalizeFieldName(member.Name);
            if (!unsafeWrite)
            {
                fieldName = ToUpperCamelCase(fieldName);
            }

            var mappedTypeSpec = GetCsTypeSpecification(member.Type);
            var mappedCsTypeName = mappedTypeSpec.ToString();
            if (!unsafeWrite)
            {
                var isPointer = false;
                if (mappedCsTypeName == "void*")
                {
                    mappedCsTypeName = "IntPtr";
                    isPointer = true;
                }
                else if (mappedCsTypeName == "byte*")
                {
                    mappedCsTypeName = "string";
                    isPointer = true;
                }
                else if (mappedCsTypeName == "byte**")
                {
                    mappedCsTypeName = "string[]";
                    isPointer = true;
                }
                else if (mappedCsTypeName == "float*")
                {
                    mappedCsTypeName = "float[]";
                    isPointer = true;
                }
                else if (member.Type.PointerIndirection == 1)
                {
                    if (member.IsOptional
                        && string.IsNullOrEmpty(member.LengthMemberName))
                    {
                        mappedCsTypeName = GetCsTypeName(member.Type.Name) + "?";
                    }
                    else
                    {
                        mappedCsTypeName = GetCsTypeName(member.Type.Name) + "[]";
                    }
                    isPointer = true;
                }
                else if (mappedCsTypeName == "uint"
                    && fieldName.EndsWith("Version"))
                {
                    mappedCsTypeName = "Version";
                }

            restart:
                if (isPointer
                    && fieldName.StartsWith("p", StringComparison.OrdinalIgnoreCase))
                {
                    fieldName = fieldName.Substring(1);
                    goto restart;
                }

                // Remove Vk prefix in case
                //if (mappedCsTypeName.StartsWith("Vk"))
                //{
                //    mappedCsTypeName = mappedCsTypeName.Substring(2);
                //}
            }
            else
            {
                // Map Handle to native pointer
                if (specs.Handles.TryGetValue(member.Type.Name, out var handle))
                {
                    if (member.Type.PointerIndirection == 1)
                    {
                        mappedCsTypeName = "IntPtr";
                    }
                    else if (member.Type.PointerIndirection == 2)
                    {
                        mappedCsTypeName = "IntPtr*";
                    }
                    else
                    {
                        mappedCsTypeName = handle.Dispatchable ? "IntPtr" : "ulong";
                    }
                }
                else if (specs.StructuresAndUnions.TryGetValue(member.Type.Name, out var structDef))
                {
                    if (!structDef.IsBlittable)
                    {
                        var nativeTypeSpec = new VulkanTypeSpecification(
                            //GetCsTypeName(member.Type.Name + ".Native"),
                            GetCsTypeName(member.Type.Name),
                            member.Type.PointerIndirection,
                            member.Type.ArrayDimensions);
                        mappedCsTypeName = nativeTypeSpec.ToString();
                    }
                    else
                    {
                        if (member.IsOptional || member.Type.PointerIndirection > 0)
                        {
                            mappedCsTypeName = "IntPtr";
                        }
                    }
                }
                else if (mappedCsTypeName == "void*"
                    || mappedCsTypeName == "byte*"
                    || mappedCsTypeName == "float*")
                {
                    mappedCsTypeName = "IntPtr";
                }
                else if (mappedCsTypeName == "byte**")
                {
                    mappedCsTypeName = "IntPtr*";
                }

                //if (mappedCsTypeName.StartsWith("Vk"))
                //{
                //    mappedCsTypeName = mappedCsTypeName.Substring(2);
                //}
            }

            writer.WriteLine($"public {mappedCsTypeName} {fieldName}{nameSuffix};");
        }

        private static void WriteMemberSymbolicCount(
            VulkanSpecs specs,
            CodeWriter writer,
            VulkanMemberDefinition member,
            VulkanConstantDefinition constant,
            bool unsafeWrite)
        {
            var csType = GetCsTypeSpecification(member.Type);
            if (!CanUseFixed(csType))
            {
                int count = int.Parse(constant.Value);
                for (int i = 0; i < count; i++)
                {
                    WriteMember(specs, writer, member, unsafeWrite, "_" + i);
                }
            }
            else
            {
                string mappedSymbolicName = GetPrettyEnumName(member.ElementCountSymbolic, "VK_");
                if (unsafeWrite)
                {
                    writer.WriteLine($"public fixed {csType} {NormalizeFieldName(member.Name)}[(int)Vulkan.{mappedSymbolicName}];");
                }
                else
                {
                    var csFieldName = GetCsFieldName(member);
                    if (member.Type.Name == "char")
                    {
                        writer.WriteLine($"public string {csFieldName};");
                    }
                    else
                    {
                        writer.WriteLine($"public {csType} {csFieldName};");
                    }
                }
            }
        }

        private static string ToUpperCamelCase(string value)
        {
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        private static string NormalizeFieldName(string name)
        {
            if (s_keywords.Contains(name))
                return "@" + name;

            return name;
        }

        private static string GetCsTypeName(string name)
        {
            if (s_csNameMappings.TryGetValue(name, out string mappedName))
            {
                return GetCsTypeName(mappedName);
            }
            else if (name.StartsWith("PFN"))
            {
                return "IntPtr";
            }

            return name;
        }

        private static string GetCsTypeName(CppType type, bool isPointer)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsTypeName(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsTypeName(qualifiedType.ElementType, isPointer);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsTypeName(enumType.Name);
                if (isPointer)
                    return enumCsName + "*";

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                var typeDefCsName = GetCsTypeName(typedef.Name);
                if (isPointer)
                    return typeDefCsName + "*";

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsTypeName(@class.Name);
                if (isPointer)
                    return className + "*";

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                return GetCsTypeName(pointerType);
            }

            if (type is CppArrayType arrayType)
            {
                return GetCsTypeName(arrayType.ElementType, isPointer);
            }

            return string.Empty;
        }

        private static string GetCsTypeName(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "void*" : "void";

                case CppPrimitiveKind.Char:
                    return isPointer ? "byte*" : "byte";

                case CppPrimitiveKind.Bool:
                    break;
                case CppPrimitiveKind.WChar:
                    break;
                case CppPrimitiveKind.Short:
                    break;
                case CppPrimitiveKind.Int:
                    return isPointer ? "int*" : "int";

                case CppPrimitiveKind.LongLong:
                    break;
                case CppPrimitiveKind.UnsignedChar:
                    break;
                case CppPrimitiveKind.UnsignedShort:
                    break;
                case CppPrimitiveKind.UnsignedInt:
                    break;
                case CppPrimitiveKind.UnsignedLongLong:
                    break;
                case CppPrimitiveKind.Float:
                    return isPointer ? "float*" : "float";
                case CppPrimitiveKind.Double:
                    break;
                case CppPrimitiveKind.LongDouble:
                    break;
                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        private static string GetCsTypeName(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsTypeName(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsTypeName(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsTypeName(subPointerType, true);
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsTypeName(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsTypeName(@enum, true);
                }

                return GetCsTypeName(qualifiedType.ElementType, true);
            }

            return GetCsTypeName(pointerType.ElementType, true);
        }


        private static bool CanUseFixed(VulkanTypeSpecification type)
        {
            return s_fixedCapableTypes.Contains(type.Name);
        }

        public static VulkanTypeSpecification GetCsTypeSpecification(VulkanTypeSpecification type)
        {
            return new VulkanTypeSpecification(
                GetCsTypeName(type.Name),
                type.PointerIndirection,
                type.ArrayDimensions);
        }
    }
}
