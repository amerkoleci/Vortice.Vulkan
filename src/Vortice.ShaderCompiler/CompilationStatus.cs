// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace Vortice.ShaderCompiler
{
    public enum CompilationStatus
    {
        Success = 0,
        InvalidStage = 1,  // error stage deduction
        compilationError = 2,
        InternalError = 3,  // unexpected failure
        NullResultObject = 4,
        InvalidAssembly = 5,
        ValidationError = 6,
        TransformationError = 7,
        ConfigurationError = 8,
    }
}
