// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Generator
{
    public sealed class VulkanHandleDefinition
    {
        public string Name { get; }
        public bool Dispatchable { get; }
        public string Parent { get; }

        public VulkanHandleDefinition(string name, bool dispatchable, string parent)
        {
            Name = name;
            Dispatchable = dispatchable;
            Parent = parent;
        }

        public override string ToString()
        {
            string handleType = Dispatchable ? "IntPtr" : "ulong";
            return $"{Name} : {handleType} -> {Parent}";
        }
    }
}
