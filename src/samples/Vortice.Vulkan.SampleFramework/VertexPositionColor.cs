// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;

namespace Vortice.Vulkan;

public readonly struct VertexPositionColor
{
    public static unsafe readonly int SizeInBytes = sizeof(VertexPositionColor);

    public VertexPositionColor(in Vector3 position, in Vector4 color)
    {
        Position = position;
        Color = color;
    }

    public readonly Vector3 Position;
    public readonly Vector4 Color;
}
