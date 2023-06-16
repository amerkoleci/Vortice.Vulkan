// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.SpirvCross;

public enum SpvResourceType
{
    Unknown = 0,
    UniformBuffer = 1,
    StorageBuffer = 2,
    StageInput = 3,
    StageOutput = 4,
    SubpassInput = 5,
    StorageImage = 6,
    SampledImage = 7,
    AtomicCounter = 8,
    PushConstant = 9,
    SeparateImage = 10,
    SeparateSamplers = 11,
    AccelerationStructure = 12,
    RayQuery = 13,
    ShaderRecordBuffer = 14,
}
