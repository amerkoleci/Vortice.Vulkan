// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Generator
{
    public sealed class VulkanMemberDefinition
    {
        public string Name { get; }

        public VulkanTypeSpecification Type { get; }

        public bool IsOptional { get; }

        public string Value { get; }

        public int ElementCount { get; }
        public string ElementCountSymbolic { get; }

        public string LengthMemberName { get; }

        public bool NullTerminated { get; }

        public string Comment { get; }

        public bool IsLength { get; set; }

        public string LegalValues { get; }

        public VulkanMemberDefinition(
            string name,
            VulkanTypeSpecification type,
            bool isOptional,
            string value,
            int elementCount,
            string elementCountSymbolic,
            string lengthMemberName,
            bool nullTerminated,
            string comment, string legalValues)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
            Value = value;
            ElementCount = elementCount;
            ElementCountSymbolic = elementCountSymbolic;
            LengthMemberName = lengthMemberName;
            NullTerminated = nullTerminated;
            Comment = comment;
            LegalValues = legalValues;
        }
    }
}
