// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Xml.Linq;
using CommunityToolkit.Diagnostics;

namespace Generator;

public class VulkanSpecification
{
    public EnumDefinition[] Enums { get; }
    public ExtensionDefinition[] Extensions { get; }

    public VulkanSpecification(Stream stream)
    {
        XDocument spec = XDocument.Load(stream);
        XElement registry = spec.Element("registry")!;
        XElement commands = registry.Element("commands")!;

        Enums = registry.Elements("enums")
            .Where(enumx => enumx.GetTypeAttributeOrNull() == "enum" || enumx.GetTypeAttributeOrNull() == "bitmask")
            .Select(enumx => EnumDefinition.CreateFromXml(enumx)).ToArray();

        Extensions = registry.Element("extensions").Elements("extension")
                .Select(xe => ExtensionDefinition.CreateFromXml(xe)).ToArray();
        AddExtensionEnums();
    }

    private void AddExtensionEnums()
    {
        foreach (ExtensionDefinition exDef in Extensions)
        {
            foreach (var enumEx in exDef.EnumExtensions)
            {
                EnumDefinition enumDef = Enums.Single(ed => ed.Name == enumEx.ExtendedType);
                int value = int.Parse(enumEx.Value);
                enumDef.Values = enumDef.Values.Append(new EnumValue(enumEx.Name, value, null)).ToArray();
            }
        }
    }


    public EnumDefinition? GetEnumDefinition(string name)
    {
        return Enums.FirstOrDefault(ed => ed.Name == name);
    }
}


public class EnumDefinition
{
    public string Name { get; }
    public EnumType Type { get; }
    public string? Comment { get; }
    public EnumValue[] Values { get; set; }

    public EnumDefinition(string name, EnumType type, string? comment, EnumValue[] values)
    {
        Guard.IsNotNullOrEmpty(name);
        Guard.IsNotNull(values);

        Name = name;
        Type = type;
        Comment = comment;
        Values = values;
    }

    public static EnumDefinition CreateFromXml(XElement xe)
    {
        Guard.IsNotNull(xe);

        EnumType type;
        var typeAttr = xe.Attribute("type");
        var commentAttr = xe.Attribute("comment");
        if (typeAttr != null)
        {
            string typeString = xe.Attribute("type").Value;
            type = (EnumType)Enum.Parse(typeof(EnumType), typeString, true);
        }
        else
        {
            type = EnumType.Constants;
        }

        string name = xe.Attribute("name").Value;
        string? comment = commentAttr != null ? commentAttr.Value : null;
        EnumValue[] values = xe.Elements("enum")
            .Select(valuesx => EnumValue.CreateFromXml(valuesx))
            .Where(item => item != null)
            .ToArray();
        return new EnumDefinition(name, type, comment, values);
    }

    public override string ToString()
    {
        return $"Enum: {Name} ({Type})[{Values.Length}]";
    }
}

public enum EnumType
{
    Bitmask,
    Enum,
    Constants,
}

public class EnumValue
{
    public string Name { get; }
    public int ValueOrBitPosition { get; }
    public string Comment { get; }

    public EnumValue(string name, int value, string comment)
    {
        Name = name;
        ValueOrBitPosition = value;
        Comment = comment;

        //if (name == "VK_COLOR_COMPONENT_R_BIT")
        //{
        //    Comment = "Specifies that the R value is written to the color attachment for the appropriate sample. Otherwise, the value in memory is unmodified.";
        //}

        // Fix some broken comments
        if (!string.IsNullOrEmpty(Comment))
        {
            Comment = Comment.Replace("See <<devsandqueues-lost-device>>", string.Empty);
        }
    }

    public static EnumValue? CreateFromXml(XElement xe)
    {
        Guard.IsNotNull(xe);

        string name = xe.Attribute("name").Value;

        int value;
        string valueStr = xe.Attribute("value")?.Value;
        string deprecatedValue = xe.Attribute("deprecated")?.Value;
        if (!string.IsNullOrEmpty(deprecatedValue))
        {
            return null;
        }

        string aliasValue = xe.Attribute("alias")?.Value;
        if (!string.IsNullOrEmpty(aliasValue))
        {
            return null;
        }

        if (!string.IsNullOrEmpty(valueStr))
        {
            if (valueStr.StartsWith("0x"))
            {
                valueStr = valueStr.Substring(2);
                value = int.Parse(valueStr, System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                value = int.Parse(valueStr);
            }
        }
        else
        {
            string bitposStr = xe.Attribute("bitpos").Value;
            value = 1 << int.Parse(bitposStr);
        }

        var commentAttr = xe.Attribute("comment");
        string comment = commentAttr != null ? commentAttr.Value : string.Empty;
        return new EnumValue(name, value, comment);
    }
}

public class ExtensionDefinition
{
    public string Name { get; }
    public int Number { get; }
    public string Type { get; }
    public ExtensionConstant[] Constants { get; }
    public EnumExtensionValue[] EnumExtensions { get; }
    public string[] CommandNames { get; }

    public ExtensionDefinition(
        string name,
        int number,
        string type,
        ExtensionConstant[] constants,
        EnumExtensionValue[] enumExtensions,
        string[] commandNames)
    {
        Name = name;
        Number = number;
        Type = type;
        Constants = constants;
        EnumExtensions = enumExtensions;
        CommandNames = commandNames;
    }

    public static ExtensionDefinition CreateFromXml(XElement xe)
    {
        string name = xe.GetNameAttribute();
        string numberString = xe.Attribute("number").Value;
        int number = int.Parse(numberString);
        string type = xe.GetTypeAttributeOrNull();
        List<ExtensionConstant> extensionConstants = new List<ExtensionConstant>();
        List<EnumExtensionValue> enumExtensions = new List<EnumExtensionValue>();
        List<string> commandNames = new List<string>();

        foreach (var require in xe.Elements("require"))
        {
            foreach (var enumXE in require.Elements("enum"))
            {
                string enumName = enumXE.GetNameAttribute();
                string extends = enumXE.Attribute("extends")?.Value;
                string deprecatedValue = enumXE.Attribute("deprecated")?.Value;
                if (!string.IsNullOrEmpty(deprecatedValue))
                    continue;

                string aliasValue = enumXE.Attribute("alias")?.Value;
                if (!string.IsNullOrEmpty(aliasValue))
                {
                    continue;
                }

                if (extends != null)
                {
                    string valueString;
                    string offsetString = enumXE.Attribute("offset")?.Value;
                    if (offsetString != null)
                    {
                        int offset = int.Parse(offsetString);
                        int direction = 1;
                        if (enumXE.Attribute("dir")?.Value == "-")
                        {
                            direction = -1;
                        }

                        int value = direction * (1000000000 + (number - 1) * 1000 + offset);
                        valueString = value.ToString();
                    }
                    else
                    {
                        string bitPosString = enumXE.Attribute("bitpos")?.Value;
                        if (bitPosString != null)
                        {
                            int shift = int.Parse(bitPosString);
                            valueString = (1 << shift).ToString();
                        }
                        else
                        {
                            valueString = enumXE.Attribute("value").Value;
                        }
                    }
                    enumExtensions.Add(new EnumExtensionValue(extends, enumName, valueString));
                }
                else
                {
                    var valueAttribute = enumXE.Attribute("value");
                    if (valueAttribute == null)
                        continue;

                    extensionConstants.Add(new ExtensionConstant(name, valueAttribute.Value));
                }
            }
            foreach (var commandXE in require.Elements("command"))
            {
                commandNames.Add(commandXE.GetNameAttribute());
            }
        }
        return new ExtensionDefinition(name, number, type, extensionConstants.ToArray(), enumExtensions.ToArray(), commandNames.ToArray());
    }
}

public class ExtensionConstant
{
    public string Name { get; }
    public string Value { get; }
    public ExtensionConstant(string name, string value)
    {
        Name = name;
        Value = value;
    }
}

[DebuggerDisplay("{DebuggerDisplayString}")]
public class EnumExtensionValue
{
    public string ExtendedType { get; }
    public string Name { get; }
    public string Value { get; }

    public EnumExtensionValue(string extendedType, string name, string value)
    {
        ExtendedType = extendedType;
        Name = name;
        Value = value;
    }

    private string DebuggerDisplayString =>
        $"Ext: {ExtendedType}, {Name} = {Value}";
}

public static class XElementExtensions
{
    public static string GetNameAttribute(this XElement xe)
    {
        return xe.Attribute("name").Value;
    }

    public static string GetNameElement(this XElement xe)
    {
        return xe.Element("name").Value;
    }

    public static string GetTypeElement(this XElement xe)
    {
        return xe.Element("type").Value;
    }

    public static string GetTypeAttributeOrNull(this XElement xe)
    {
        return xe.Attribute("type")?.Value;
    }

    public static bool GetOptionalAttributeOrFalse(this XElement xe)
    {
        var attr = xe.Attribute("optional");
        return attr != null
            ? attr.Value == "true"
            : false;
    }

    public static bool HasCategoryAttribute(this XElement xe, string value)
    {
        var attr = xe.Attribute("category");
        return attr != null && attr.Value == value;
    }
}
