// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.ShaderCompiler
{
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
}
