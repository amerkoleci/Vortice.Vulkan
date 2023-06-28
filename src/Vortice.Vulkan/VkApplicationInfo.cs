// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying application information
/// </summary>
public partial struct VkApplicationInfo
{
    /// <summary>
    ///  NULL or a pointer to a structure extending this structure.
    /// </summary>
    public unsafe void* pNext;

    /// <summary>
    /// The unicode string containing the name of the application.
    /// </summary>
    public string? ApplicationName;
    /// <summary>
    /// The unsigned integer variable containing the developer-supplied version
    /// number of the application.
    /// </summary>
    public VkVersion ApplicationVersion;
    /// <summary>
    /// The unicode string containing the name of the engine (if any) used to create the application.
    /// </summary>
    public string? EngineName;
    /// <summary>
    /// The unsigned integer variable containing the developer-supplied version
    /// number of the engine used to create the application.
    /// </summary>
    public VkVersion EngineVersion;
    /// <summary>
    /// The must be the highest version of Vulkan that the application is designed to use.
    /// </summary>
    public VkVersion ApiVersion;

    #region Marshal
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct __Native
    {
        public VkStructureType sType;
        public void* pNext;
        public sbyte* pApplicationName;
        public VkVersion applicationVersion;
        public sbyte* pEngineName;
        public VkVersion engineVersion;
        public VkVersion apiVersion;

        public readonly void Free()
        {
            NativeMemory.Free(pApplicationName);
            NativeMemory.Free(pEngineName);
        }
    }

    internal readonly unsafe void ToNative(__Native* native)
    {
        native->sType = VkStructureType.ApplicationInfo;
        native->pNext = pNext;
        native->pApplicationName = Interop.AllocToPointer(ApplicationName);
        native->applicationVersion = ApplicationVersion;
        native->pEngineName = Interop.AllocToPointer(EngineName);
        native->engineVersion = EngineVersion;
        native->apiVersion = ApiVersion;
    }
    #endregion
}
