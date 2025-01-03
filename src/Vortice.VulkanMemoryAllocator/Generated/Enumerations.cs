// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;

namespace Vortice.Vulkan;

[Flags]
public enum VmaAllocatorCreateFlags
{
	None = 0,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_EXTERNALLY_SYNCHRONIZED_BIT</unmanaged>
	ExternallySynchronized = 0x00000001,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_KHR_DEDICATED_ALLOCATION_BIT</unmanaged>
	KHRDedicatedAllocation = 0x00000002,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_KHR_BIND_MEMORY2_BIT</unmanaged>
	KHRBindMemory2 = 0x00000004,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_EXT_MEMORY_BUDGET_BIT</unmanaged>
	EXTMemoryBudget = 0x00000008,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_AMD_DEVICE_COHERENT_MEMORY_BIT</unmanaged>
	AMDDeviceCoherentMemory = 0x00000010,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_BUFFER_DEVICE_ADDRESS_BIT</unmanaged>
	BufferDeviceAddress = 0x00000020,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_EXT_MEMORY_PRIORITY_BIT</unmanaged>
	EXTMemoryPriority = 0x00000040,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_KHR_MAINTENANCE4_BIT</unmanaged>
	KHRMaintenance4 = 0x00000080,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_KHR_MAINTENANCE5_BIT</unmanaged>
	KHRMaintenance5 = 0x00000100,
	/// <unmanaged>VMA_ALLOCATOR_CREATE_KHR_EXTERNAL_MEMORY_WIN32_BIT</unmanaged>
	KHRExternalMemoryWin32 = 0x00000200,
}

public enum VmaMemoryUsage
{
	/// <unmanaged>VMA_MEMORY_USAGE_UNKNOWN</unmanaged>
	Unknown = 0,
	/// <unmanaged>VMA_MEMORY_USAGE_GPU_ONLY</unmanaged>
	GpuOnly = 1,
	/// <unmanaged>VMA_MEMORY_USAGE_CPU_ONLY</unmanaged>
	CpuOnly = 2,
	/// <unmanaged>VMA_MEMORY_USAGE_CPU_TO_GPU</unmanaged>
	CpuToGpu = 3,
	/// <unmanaged>VMA_MEMORY_USAGE_GPU_TO_CPU</unmanaged>
	GpuToCpu = 4,
	/// <unmanaged>VMA_MEMORY_USAGE_CPU_COPY</unmanaged>
	CpuCopy = 5,
	/// <unmanaged>VMA_MEMORY_USAGE_GPU_LAZILY_ALLOCATED</unmanaged>
	GpuLazilyAllocated = 6,
	/// <unmanaged>VMA_MEMORY_USAGE_AUTO</unmanaged>
	Auto = 7,
	/// <unmanaged>VMA_MEMORY_USAGE_AUTO_PREFER_DEVICE</unmanaged>
	AutoPreferDevice = 8,
	/// <unmanaged>VMA_MEMORY_USAGE_AUTO_PREFER_HOST</unmanaged>
	AutoPreferHost = 9,
}

[Flags]
public enum VmaAllocationCreateFlags
{
	None = 0,
	/// <unmanaged>VMA_ALLOCATION_CREATE_DEDICATED_MEMORY_BIT</unmanaged>
	DedicatedMemory = 0x00000001,
	/// <unmanaged>VMA_ALLOCATION_CREATE_NEVER_ALLOCATE_BIT</unmanaged>
	NeverAllocate = 0x00000002,
	/// <unmanaged>VMA_ALLOCATION_CREATE_MAPPED_BIT</unmanaged>
	Mapped = 0x00000004,
	/// <unmanaged>VMA_ALLOCATION_CREATE_USER_DATA_COPY_STRING_BIT</unmanaged>
	UserDataCopyString = 0x00000020,
	/// <unmanaged>VMA_ALLOCATION_CREATE_UPPER_ADDRESS_BIT</unmanaged>
	UpperAddress = 0x00000040,
	/// <unmanaged>VMA_ALLOCATION_CREATE_DONT_BIND_BIT</unmanaged>
	DontBind = 0x00000080,
	/// <unmanaged>VMA_ALLOCATION_CREATE_WITHIN_BUDGET_BIT</unmanaged>
	WithinBudget = 0x00000100,
	/// <unmanaged>VMA_ALLOCATION_CREATE_CAN_ALIAS_BIT</unmanaged>
	CanAlias = 0x00000200,
	/// <unmanaged>VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT</unmanaged>
	HostAccessSequentialWrite = 0x00000400,
	/// <unmanaged>VMA_ALLOCATION_CREATE_HOST_ACCESS_RANDOM_BIT</unmanaged>
	HostAccessRandom = 0x00000800,
	/// <unmanaged>VMA_ALLOCATION_CREATE_HOST_ACCESS_ALLOW_TRANSFER_INSTEAD_BIT</unmanaged>
	HostAccessAllowTransferInstead = 0x00001000,
	/// <unmanaged>VMA_ALLOCATION_CREATE_STRATEGY_MIN_MEMORY_BIT</unmanaged>
	StrategyMinMemory = 0x00010000,
	/// <unmanaged>VMA_ALLOCATION_CREATE_STRATEGY_MIN_TIME_BIT</unmanaged>
	StrategyMinTime = 0x00020000,
	/// <unmanaged>VMA_ALLOCATION_CREATE_STRATEGY_MIN_OFFSET_BIT</unmanaged>
	StrategyMinOffset = 0x00040000,
	/// <unmanaged>VMA_ALLOCATION_CREATE_STRATEGY_BEST_FIT_BIT</unmanaged>
	StrategyBestFit = StrategyMinMemory,
	/// <unmanaged>VMA_ALLOCATION_CREATE_STRATEGY_FIRST_FIT_BIT</unmanaged>
	StrategyFirstFit = StrategyMinTime,
	/// <unmanaged>VMA_ALLOCATION_CREATE_STRATEGY_MASK</unmanaged>
	StrategyMask = StrategyMinMemory | StrategyMinTime | StrategyMinOffset,
}

[Flags]
public enum VmaPoolCreateFlags
{
	None = 0,
	/// <unmanaged>VMA_POOL_CREATE_IGNORE_BUFFER_IMAGE_GRANULARITY_BIT</unmanaged>
	IgnoreBufferImageGranularity = 0x00000002,
	/// <unmanaged>VMA_POOL_CREATE_LINEAR_ALGORITHM_BIT</unmanaged>
	LinearAlgorithm = 0x00000004,
	/// <unmanaged>VMA_POOL_CREATE_ALGORITHM_MASK</unmanaged>
	AlgorithmMask = LinearAlgorithm,
}

[Flags]
public enum VmaDefragmentationFlags
{
	None = 0,
	/// <unmanaged>VMA_DEFRAGMENTATION_FLAG_ALGORITHM_FAST_BIT</unmanaged>
	FlagAlgorithmFast = 0x1,
	/// <unmanaged>VMA_DEFRAGMENTATION_FLAG_ALGORITHM_BALANCED_BIT</unmanaged>
	FlagAlgorithmBalanced = 0x2,
	/// <unmanaged>VMA_DEFRAGMENTATION_FLAG_ALGORITHM_FULL_BIT</unmanaged>
	FlagAlgorithmFull = 0x4,
	/// <unmanaged>VMA_DEFRAGMENTATION_FLAG_ALGORITHM_EXTENSIVE_BIT</unmanaged>
	FlagAlgorithmExtensive = 0x8,
	/// <unmanaged>VMA_DEFRAGMENTATION_FLAG_ALGORITHM_MASK</unmanaged>
	FlagAlgorithmMask = FlagAlgorithmFast | FlagAlgorithmBalanced | FlagAlgorithmFull | FlagAlgorithmExtensive,
}

public enum VmaDefragmentationMoveOperation
{
	/// <unmanaged>VMA_DEFRAGMENTATION_MOVE_OPERATION_COPY</unmanaged>
	Copy = 0,
	/// <unmanaged>VMA_DEFRAGMENTATION_MOVE_OPERATION_IGNORE</unmanaged>
	Ignore = 1,
	/// <unmanaged>VMA_DEFRAGMENTATION_MOVE_OPERATION_DESTROY</unmanaged>
	Destroy = 2,
}

[Flags]
public enum VmaVirtualBlockCreateFlags
{
	None = 0,
	/// <unmanaged>VMA_VIRTUAL_BLOCK_CREATE_LINEAR_ALGORITHM_BIT</unmanaged>
	LinearAlgorithm = 0x00000001,
	/// <unmanaged>VMA_VIRTUAL_BLOCK_CREATE_ALGORITHM_MASK</unmanaged>
	AlgorithmMask = LinearAlgorithm,
}

[Flags]
public enum VmaVirtualAllocationCreateFlags
{
	None = 0,
	/// <unmanaged>VMA_VIRTUAL_ALLOCATION_CREATE_UPPER_ADDRESS_BIT</unmanaged>
	UpperAddress = VmaAllocationCreateFlags.UpperAddress,
	/// <unmanaged>VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MIN_MEMORY_BIT</unmanaged>
	StrategyMinMemory = VmaAllocationCreateFlags.StrategyMinMemory,
	/// <unmanaged>VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MIN_TIME_BIT</unmanaged>
	StrategyMinTime = VmaAllocationCreateFlags.StrategyMinTime,
	/// <unmanaged>VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MIN_OFFSET_BIT</unmanaged>
	StrategyMinOffset = VmaAllocationCreateFlags.StrategyMinOffset,
	/// <unmanaged>VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MASK</unmanaged>
	StrategyMask = VmaAllocationCreateFlags.StrategyMask,
}

