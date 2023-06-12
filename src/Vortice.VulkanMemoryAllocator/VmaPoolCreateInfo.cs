// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VkDeviceSize = System.UInt64;

namespace Vortice.Vulkan;

public unsafe struct VmaPoolCreateInfo
{
    /// <summary>
    /// Vulkan memory type index to allocate this pool from.
    /// </summary>
    public uint memoryTypeIndex;
    /// <summary>
    /// Use combination of #VmaPoolCreateFlagBits.
    /// </summary>
    public VmaPoolCreateFlags flags;
    /** \brief Size of a single `VkDeviceMemory` block to be allocated as part of this pool, in bytes. Optional.

    Specify nonzero to set explicit, constant size of memory blocks used by this
    pool.

    Leave 0 to use default and let the library manage block sizes automatically.
    Sizes of particular blocks may vary.
    In this case, the pool will also support dedicated allocations.
    */
    public VkDeviceSize blockSize;
    /// <summary>
    /// Minimum number of blocks to be always allocated in this pool, even if they stay empty.
    /// Set to 0 to have no preallocated blocks and allow the pool be completely empty.
    /// </summary>
    public nuint minBlockCount;
    /// <summary>
    /// Maximum number of blocks that can be allocated in this pool. Optional.
    /// Set to 0 to use default, which is `SIZE_MAX`, which means no limit.
    /// Set to same value as VmaPoolCreateInfo::minBlockCount to have fixed amount of memory allocated throughout whole lifetime of this pool.
    /// </summary>
    public nuint maxBlockCount;
    /// <summary>
    /// A floating-point value between 0 and 1, indicating the priority of the allocations in this pool relative to other memory allocations.
    ///
    /// It is used only when #VMA_ALLOCATOR_CREATE_EXT_MEMORY_PRIORITY_BIT flag was used during creation of the #VmaAllocator object.
    /// Otherwise, this variable is ignored.
    /// </summary>
    public float priority;
    /// <summary>
    /// Additional minimum alignment to be used for all allocations created from this pool. Can be 0.
    /// Leave 0 (default) not to impose any additional alignment.If not 0, it must be a power of two.
    /// It can be useful in cases where alignment returned by Vulkan by functions like `vkGetBufferMemoryRequirements` is not enough,
    /// e.g.when doing interop with OpenGL.
    /// </summary>
    public VkDeviceSize minAllocationAlignment;
    /// <summary>
    /// Additional `pNext` chain to be attached to `VkMemoryAllocateInfo` used for every allocation made by this pool. Optional.
    /// 
    /// Optional, can be null. If not null, it must point to a `pNext` chain of structures that can be attached to `VkMemoryAllocateInfo`.
    /// It can be useful for special needs such as adding `VkExportMemoryAllocateInfoKHR`.
    /// Structures pointed by this member must remain alive and unchanged for the whole lifetime of the custom pool.
    /// 
    /// Please note that some structures, e.g. `VkMemoryPriorityAllocateInfoEXT`, `VkMemoryDedicatedAllocateInfoKHR`,
    /// can be attached automatically by this library when using other, more convenient of its features.
    /// </summary>
    public void* pMemoryAllocateNext;
}
