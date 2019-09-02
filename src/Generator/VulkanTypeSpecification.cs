// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Generator
{
    public sealed class VulkanTypeSpecification
    {
        public string Name { get; }
        public int PointerIndirection { get; }
        public int ArrayDimensions { get; }

        public VulkanTypeSpecification(string name, int pointerIndirection = 0, int arrayDimensions = 0)
        {
            Name = name;
            PointerIndirection = pointerIndirection;
            ArrayDimensions = arrayDimensions;
        }

        public override string ToString() => $"{Name}{new string('*', PointerIndirection)}{GetArrayPortion()}";

        private string GetArrayPortion()
        {
            if (ArrayDimensions == 0)
            {
                return string.Empty;
            }
            else if (ArrayDimensions == 1)
            {
                return "[]";
            }

            return $"[{new string(',', ArrayDimensions - 1)}]";
        }
    }
}
