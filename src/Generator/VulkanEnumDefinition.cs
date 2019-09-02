// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Generator
{
    public sealed class VulkanEnumDefinition
    {
        public string Name { get; }
        public bool IsBitMask { get; }

        public readonly List<VulkanEnumValue> Values;

        public VulkanEnumDefinition(string name, bool isBitMask, VulkanEnumValue[] values)
        {
            Name = name;
            IsBitMask = isBitMask;
            Values = new List<VulkanEnumValue>(values);
        }

        public override string ToString()
        {
            if (IsBitMask)
            {
                return $"Flag Enum: {Name}[{Values}]";
            }

            return $"Enum: {Name}[{Values}]";
        }
    }

    public class VulkanEnumValue
    {
        public string Name { get; }
        public int Value { get; }
        public string Comment { get; }

        public VulkanEnumValue(string name, int value, string comment)
        {
            Name = name;
            Value = value;
            Comment = comment;
        }

        public override string ToString() => $"{Name} = {Value}";
    }

}
