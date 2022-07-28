// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.SpirvCross;

public enum Result 
{
    Success = 0,
	ErrorInvalidSPIRV = -1,
    ErrorUnsupportedSPIRV = -2,
	ErrorOutOfMemory = -3,
	ErrorInvalidArgumetn = -4
}
