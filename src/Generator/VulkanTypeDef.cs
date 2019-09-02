// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Generator
{
    public sealed class VulkanTypeDef
    {
        public string Name { get; }
        public string Requires { get; }
        public string Type { get; }

        public VulkanTypeDef(string name, string requires, string type)
        {
            Name = name;
            Requires = requires;
            Type = type;
        }

        public override string ToString() => $"{Name}, {Requires} -> {Type}";
    }
}
