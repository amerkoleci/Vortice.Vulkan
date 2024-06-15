// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.


namespace Vortice.Vulkan;

public interface IChainType
{
    unsafe void* pNext { get; set; }
}
