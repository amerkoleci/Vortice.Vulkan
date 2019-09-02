// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Generator
{
    public sealed class VulkanConstantDefinition
    {
        public string Name { get; }
        public string Value { get; }
        public ConstantDataType Type { get; }
        public string Comment { get; }

        public VulkanConstantDefinition(string name, string value, string comment)
        {
            Name = name;
            Value = value;
            Type = ParseDataType(value);
            Comment = comment;
        }

        private ConstantDataType ParseDataType(string value)
        {
            if (value.EndsWith("f"))
            {
                return ConstantDataType.Float;
            }
            else if (value.EndsWith("ULL)"))
            {
                return ConstantDataType.UInt64;
            }

            return ConstantDataType.UInt32;
        }

        public override string ToString() => $"{Name}, {Type} = {Value}";

        public enum ConstantDataType
        {
            UInt32,
            UInt64,
            Float
        }
    }
}
