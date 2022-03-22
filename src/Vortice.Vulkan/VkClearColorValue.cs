// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying a clear color value.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe partial struct VkClearColorValue
{
    [FieldOffset(0)] public fixed float float32[4];
    [FieldOffset(0)] public fixed int int32[4];
    [FieldOffset(0)] public fixed uint uint32[4];

    /// <summary>
    /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
    /// </summary>
    /// <param name="r">The red value.</param>
    /// <param name="g">The green value.</param>
    /// <param name="b">The blue value.</param>
    /// <param name="a">The alpha value.</param>
    public VkClearColorValue(float r, float g, float b, float a = 1.0f)
        : this()
    {
        float32[0] = r;
        float32[1] = g;
        float32[2] = b;
        float32[3] = a;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
    /// </summary>
    /// <param name="r">The red value.</param>
    /// <param name="g">The green value.</param>
    /// <param name="b">The blue value.</param>
    /// <param name="a">The alpha value.</param>
    public VkClearColorValue(int r, int g, int b, int a = 255) : this()
    {
        int32[0] = r;
        int32[1] = g;
        int32[2] = b;
        int32[3] = a;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkClearColorValue"/> structure.
    /// </summary>
    /// <param name="r">The red value.</param>
    /// <param name="g">The green value.</param>
    /// <param name="b">The blue value.</param>
    /// <param name="a">The alpha value.</param>
    public VkClearColorValue(uint r, uint g, uint b, uint a = 255) : this()
    {
        uint32[0] = r;
        uint32[1] = g;
        uint32[2] = b;
        uint32[3] = a;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Vector4"/> to <see cref="VkClearColorValue"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator VkClearColorValue(Vector4 value) => new(value.X, value.Y, value.Z, value.W);
}
