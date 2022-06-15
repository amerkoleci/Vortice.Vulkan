// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

public enum OptimizationLevel
{
    /// <summary>
    /// No optimization
    /// </summary>
    Zero,
    /// <summary>
    /// Optimize towards reducing code size
    /// </summary>
    Size,
    /// <summary>
    /// Optimize towards performance.
    /// </summary>
    Performance,
}
