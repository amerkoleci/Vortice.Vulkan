// Copyright (c) Amer Koleci and Contributors
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace Vortice.Vulkan
{
    public readonly struct VkVersion : IEquatable<VkVersion>, IComparable<VkVersion>
    {
        private readonly uint _value;

        public static readonly VkVersion Version_1_0 = new VkVersion(0, 1, 0, 0);
        public static readonly VkVersion Version_1_1 = new VkVersion(0, 1, 1, 0);
        public static readonly VkVersion Version_1_2 = new VkVersion(0, 1, 2, 0);

        public VkVersion(uint value)
        {
            _value = value;
        }

        public VkVersion(uint variant, uint major, uint minor, uint patch)
        {
            _value = (variant << 29) | (major << 22) | (minor << 12) | patch;
        }

        public VkVersion(uint major, uint minor, uint patch)
        {
            _value = (major << 22) | (minor << 12) | patch;
        }

        public uint Variant => _value >> 29;

        public uint Major => ((_value >> 22) & 0x7Fu);

        public uint Minor => ((_value >> 12) & 0x3FFu);

        public uint Patch => _value & 0xfffu;

        public static implicit operator uint(VkVersion version)
        {
            return version._value;
        }

        public override string ToString() => $"{Variant}.{Major}.{Minor}.{Patch}";

        public bool Equals(VkVersion other) => _value == other._value;
        public int CompareTo(VkVersion other) => _value.CompareTo(other._value);
    }
}
