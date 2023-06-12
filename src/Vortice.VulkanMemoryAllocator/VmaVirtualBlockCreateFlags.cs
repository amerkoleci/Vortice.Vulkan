// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VmaVirtualBlockCreateFlags : uint
{
    None = 0,
    /// <summary>
    /// Enables alternative, linear allocation algorithm in this virtual block.
    /// Specify this flag to enable linear allocation algorithm, which always creates
    /// new allocations after last one and doesn't reuse space from allocations freed in
    /// between.It trades memory consumption for simplified algorithm and data
    /// structure, which has better performance and uses less memory for metadata.
    /// </summary>
    /// <remarks>
    /// By using this flag, you can achieve behavior of free-at-once, stack, ring buffer, and double stack.
    /// </remarks>
    LinearAlgorith = 0x00000001,
}
