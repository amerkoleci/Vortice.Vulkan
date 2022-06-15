// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

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
