// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Generator
{
    public sealed class VulkanSpecs
    {
        public readonly Dictionary<string, VulkanConstantDefinition> Constants = new Dictionary<string, VulkanConstantDefinition>();
        public readonly Dictionary<string, VulkanTypeDef> Typedefs = new Dictionary<string, VulkanTypeDef>();
        public readonly Dictionary<string, VulkanEnumDefinition> Enums = new Dictionary<string, VulkanEnumDefinition>();

        public readonly Dictionary<string, VulkanStructDefinition> StructuresAndUnions = new Dictionary<string, VulkanStructDefinition>();
        public readonly Dictionary<string, VulkanHandleDefinition> Handles = new Dictionary<string, VulkanHandleDefinition>();

        public VulkanSpecs(string specFile)
        {
            var registry = XElement.Load(specFile);
            if (registry.Name != "registry")
                throw new InvalidDataException("Invalid vulkan specs, top element is not 'registry'");

            // Constant values first.
            LoadApiConstants(registry);
            LoadTypedefDefinitions(registry);
            LoadEnums(registry);
            LoadStructsAndUnions(registry);
            LoadHandlesAndCommands(registry);
            LoadExtensions(registry);
        }

        private void LoadApiConstants(XElement registry)
        {
            Console.WriteLine("Parse API Constants");

            foreach (var enumElement in registry.Elements("enums")
                .Where(item => item.Attribute("name").Value == "API Constants")
                .SelectMany(item => item.Elements("enum")))
            {
                string name = enumElement.Attribute("name").Value;
                var aliasAttribute = enumElement.Attribute("alias");
                if (aliasAttribute != null)
                {
                    var aliasConstant = Constants[aliasAttribute.Value];
                    Constants.Add(name, new VulkanConstantDefinition(name, aliasConstant.Value, aliasConstant.Comment));
                    continue;
                }

                string value = enumElement.Attribute("value").Value;
                string comment = enumElement.Attribute("comment")?.Value;
                var constant = new VulkanConstantDefinition(name, value, comment);
                Constants.Add(name, constant);
                // Console.WriteLine($"Constant parsed - {constant}");
            }
        }

        private void LoadTypedefDefinitions(XElement registry)
        {
            Console.WriteLine("Parse typedefs");

            foreach (var typeDefElement in registry.Elements("types")
                .Elements("type")
                .Where(item => item.Value.Contains("typedef") && item.Attribute("category")?.Value == "bitmask"))
            {
                string name = typeDefElement.Element("name").Value;
                string requires = typeDefElement.Attribute("requires")?.Value;
                string type = typeDefElement.Element("type").Value;
                var typeDef = new VulkanTypeDef(name, requires, type);
                Typedefs.Add(name, typeDef);
                //Console.WriteLine($"Typedef parsed - {typeDef}");
            }
        }

        private void LoadEnums(XElement registry)
        {
            Console.WriteLine("Parsing enums");

            // Enums
            foreach (var enumElement in registry.Elements("enums")
                .Where(item => item.Attribute("type")?.Value == "enum"
                || item.Attribute("type")?.Value == "bitmask"))
            {
                string name = enumElement.Attribute("name").Value;

                var isBitMask = false;
                var typeAttr = enumElement.Attribute("type");
                if (typeAttr != null)
                {
                    string typeString = typeAttr.Value;
                    isBitMask = typeString.Equals("bitmask");
                }

                var enumElementCommentAttr = enumElement.Attribute("comment");
                string comment = enumElementCommentAttr != null ? enumElementCommentAttr.Value : string.Empty;

                // Values
                var enumValues = new List<VulkanEnumValue>();
                foreach (var valueElement in enumElement.Elements("enum"))
                {
                    string enumValueName = valueElement.Attribute("name").Value;
                    var enumValueAlias = valueElement.Attribute("alias");
                    if (enumValueAlias != null)
                    {
                        var aliasEnum = enumValues.First(item => item.Name == enumValueAlias.Value);
                        if (enumValueName == "VK_COLORSPACE_SRGB_NONLINEAR_KHR")
                        {
                            continue;
                        }

                        enumValues.Add(new VulkanEnumValue(enumValueName, aliasEnum.Value, aliasEnum.Comment));
                        continue;
                    }

                    int enumValue;
                    string valueStr = valueElement.Attribute("value")?.Value;
                    if (valueStr != null)
                    {
                        if (valueStr.StartsWith("0x"))
                        {
                            valueStr = valueStr.Substring(2);
                            enumValue = int.Parse(valueStr, System.Globalization.NumberStyles.HexNumber);
                        }
                        else
                        {
                            enumValue = int.Parse(valueStr);
                        }
                    }
                    else
                    {
                        string bitposStr = valueElement.Attribute("bitpos").Value;
                        enumValue = 1 << int.Parse(bitposStr);
                    }

                    var valueElementCommentAttr = valueElement.Attribute("comment");
                    string enumValueComment = valueElementCommentAttr != null ? valueElementCommentAttr.Value : string.Empty;
                    enumValues.Add(new VulkanEnumValue(enumValueName, enumValue, enumValueComment));
                }

                var enumDef = new VulkanEnumDefinition(name, isBitMask, enumValues.ToArray());
                Enums.Add(name, enumDef);
                //Console.WriteLine($"Enum parsed - {enumDef}");
            }
        }

        private void LoadStructsAndUnions(XElement registry)
        {
            Console.WriteLine("Parsing structs and unions");

            // Structures
            foreach (var typeElement in registry
                .Elements("types")
                .Elements("type")
                .Where(item => item.Attribute("category")?.Value == "struct"))
            {
                var categoryAttr = typeElement.Attribute("category");
                if (categoryAttr == null
                    || (categoryAttr.Value != "struct" && categoryAttr.Value != "union"))
                {
                    continue;
                }

                var alias = string.Empty;
                var aliasAttr = typeElement.Attribute("alias");
                if (aliasAttr != null)
                {
                    alias = aliasAttr.Value;
                }

                string name = typeElement.Attribute("name").Value;
                var isUnion = categoryAttr.Value == "union";
                var isBlittable = true;
                if (isUnion)
                {

                }

                // TODO: map correctly
                if (name == "VkAttachmentSampleLocationsEXT"
                    || name == "VkSubpassSampleLocationsEXT"
                    || name == "VkGeometryDataNV")
                {
                    isBlittable = false;
                }

                var members = new List<VulkanMemberDefinition>();
                foreach (var memberElement in typeElement.Elements("member"))
                {
                    string memberName = memberElement.Element("name").Value;
                    string memberTypeName = memberElement.Element("type").Value;

                    if (memberName == "sType"
                       && memberTypeName == "VkStructureType")
                    {
                        isBlittable = false;
                    }

                    bool isOptional = memberElement.Attribute("optional")?.Value == "true";
                    int pointerLevel = memberElement.Value.Contains($"{memberTypeName}*") ? 1 : 0; // TODO: Make this better.
                    if (memberElement.Value.Contains($"{memberTypeName}* const*"))
                    {
                        pointerLevel++;
                    }

                    var type = new VulkanTypeSpecification(memberTypeName, pointerLevel);

                    bool foundConstantElementCount = false;
                    int elementCount = 1;
                    string elementCountSymbolic = null;
                    for (int i = 2; i < 10; i++)
                    {
                        if (memberElement.Value.Contains($"{name}[{i}]"))
                        {
                            elementCount = i;
                            foundConstantElementCount = true;
                            break;
                        }
                    }

                    if (!foundConstantElementCount)
                    {
                        var m = Regex.Match(memberElement.Value, @"\[(.*)\]");
                        if (m.Captures.Count > 0)
                        {
                            elementCountSymbolic = m.Groups[1].Value;
                        }
                    }

                    if (!string.IsNullOrEmpty(elementCountSymbolic))
                    {
                        isBlittable = false;
                    }

                    var nullTerminated = false;
                    var lengthMemberName = string.Empty;
                    string memberValue = memberElement.Attribute("values")?.Value;
                    var lenAttribute = memberElement.Attribute("len");
                    if (lenAttribute != null)
                    {
                        foreach (var lenSplit in lenAttribute.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (lenSplit.Equals("null-terminated", StringComparison.OrdinalIgnoreCase))
                            {
                                nullTerminated = true;
                            }
                            else
                            {
                                lengthMemberName = lenSplit;
                            }
                        }

                        isBlittable = false;
                    }

                    var comment = memberElement.Element("comment")?.Value;
                    string legalValues = memberElement.Attribute("values")?.Value;

                    members.Add(new VulkanMemberDefinition(
                        memberName,
                        type,
                        isOptional,
                        memberValue,
                        elementCount,
                        elementCountSymbolic,
                        lengthMemberName,
                        nullTerminated,
                        comment,
                        legalValues)
                        );
                }

                // Detect length members
                foreach (var member in members)
                {
                    if (string.IsNullOrEmpty(member.LengthMemberName))
                    {
                        continue;
                    }

                    var lengthMember = members.FirstOrDefault(item => item.Name.Equals(member.LengthMemberName, StringComparison.OrdinalIgnoreCase));
                    if (lengthMember != null)
                    {
                        lengthMember.IsLength = true;
                    }
                }

                var structDef = new VulkanStructDefinition(name, isUnion, isBlittable, members.ToArray(), alias);
                StructuresAndUnions.Add(name, structDef);

                //Console.WriteLine($"Struct parsed - {structDef}");
            }
        }

        private void LoadHandlesAndCommands(XElement registry)
        {
            Console.WriteLine("Parsing handles");

            // Handles
            foreach (var handleElement in registry
                .Elements("types")
                .Elements("type")
                .Where(item => item.Attribute("category")?.Value == "handle"))
            {
                string name;
                var aliasAttribute = handleElement.Attribute("alias");
                if (aliasAttribute != null)
                {
                    name = handleElement.Attribute("name").Value;
                    var aliasHandle = Handles[aliasAttribute.Value];
                    Handles.Add(name, new VulkanHandleDefinition(name, aliasHandle.Dispatchable, aliasHandle.Parent));
                    continue;
                }

                name = handleElement.Element("name").Value;
                bool dispatchable = handleElement.Element("type").Value == "VK_DEFINE_HANDLE";
                string parent = handleElement.Attribute("parent")?.Value;

                var handleDef = new VulkanHandleDefinition(name, dispatchable, parent);
                Handles.Add(name, handleDef);
                //Console.WriteLine($"Struct parsed - {handleDef}");
            }

            foreach (var commandElement in registry
                .Element("commands")
                .Elements("command"))
            {
                var aliasAttribute = commandElement.Attribute("alias");

                if (aliasAttribute != null)
                {
                    //var aliasConstant = Constants[aliasAttribute.Value];
                    //Constants.Add(name, new ConstantDefinition(name, aliasConstant.Value, aliasConstant.Comment));
                    continue;
                }

                var proto = commandElement.Element("proto");
                string name = proto.Element("name").Value;

                foreach (var param in commandElement.Elements("param"))
                {
                    string type = param.Element("type").Value;
                    string paramName = param.Element("name").Value;
                }
            }
        }

        private void LoadExtensions(XElement registry)
        {
            Console.WriteLine("Parsing extensions");

            //foreach (var extensionElement in registry
            //    .Elements("feature")
            //    .Where(item => item.Attribute("api")?.Value == "vulkan" && item.Attribute("name")?.Value == "VK_VERSION_1_1"))
            //{
            //    ParseExtension(extensionElement, true);
            //}

            foreach (var extensionElement in registry
                .Element("extensions")
                .Elements("extension"))
            {
                string numberString = extensionElement.Attribute("number").Value;
                int number = int.Parse(numberString);
                ParseExtension(extensionElement, number);
            }
        }

        private void ParseExtension(XElement extensionElement, int number)
        {
            var name = extensionElement.Attribute("name").Value;
            //var enumValues = new Dictionary<string, string>();

            foreach (var require in extensionElement.Elements("require"))
            {
                var feature = require.Attribute("feature")?.Value;
                if (feature == "VK_VERSION_1_1")
                    continue;

                // Enums
                foreach (var enumElement in require.Elements("enum"))
                {
                    string enumName = enumElement.Attribute("name").Value;
                    string enumExtends = enumElement.Attribute("extends")?.Value;
                    string enumAlias = enumElement.Attribute("alias")?.Value;

                    if (!string.IsNullOrEmpty(enumAlias))
                    {
                        // TODO: is necessary?
                        continue;
                    }

                    if (enumExtends != null)
                    {
                        string valueString;
                        string offsetString = enumElement.Attribute("offset")?.Value;
                        if (offsetString != null)
                        {
                            int offset = int.Parse(offsetString);
                            int direction = 1;
                            if (enumElement.Attribute("dir")?.Value == "-")
                            {
                                direction = -1;
                            }

                            string extNumberStr = enumElement.Attribute("extnumber")?.Value;
                            int extNumber = number;
                            if (!string.IsNullOrEmpty(extNumberStr))
                            {
                                extNumber = int.Parse(extNumberStr);
                            }

                            int value = direction * (1000000000 + (extNumber - 1) * 1000 + offset);
                            valueString = value.ToString();
                        }
                        else
                        {
                            string bitPosString = enumElement.Attribute("bitpos")?.Value;
                            if (bitPosString != null)
                            {
                                int shift = int.Parse(bitPosString);
                                valueString = (1 << shift).ToString();
                            }
                            else
                            {
                                valueString = enumElement.Attribute("value").Value;
                            }
                        }

                        var enumDefinition = Enums[enumExtends];
                        var enumValue = int.Parse(valueString);
                        //enumValues.Add(enumName, enumExtends);
                        enumDefinition.Values.Add(new VulkanEnumValue(enumName, enumValue, string.Empty));
                    }
                }
            }
        }
    }
}
