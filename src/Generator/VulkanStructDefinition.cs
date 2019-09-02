// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace Generator
{
    public sealed class VulkanStructDefinition
    {
        public string Name { get; }

        public bool IsUnion { get; }

        public bool IsBlittable { get; }

        public VulkanMemberDefinition[] Members { get; }

        public VulkanStructDefinition(string name, bool isUnion, bool isBlittable, VulkanMemberDefinition[] members)
        {
            Name = name;
            IsUnion = isUnion;
            IsBlittable = isBlittable;
            Members = members;
        }

        public override string ToString()
        {
            if (IsUnion)
                return $"union {Name}[{Members.Length}]";

            return $"struct {Name}[{Members.Length}]";
        }

        public VulkanMemberDefinition GetLengthMember(VulkanMemberDefinition length)
        {
            return Array.Find(Members, item => item.LengthMemberName.Equals(length.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
