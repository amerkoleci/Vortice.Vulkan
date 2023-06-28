// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

/// <summary>
/// Structure specifying parameters of a newly created instance
/// </summary>
public partial struct VkInstanceCreateInfo
{
    /// <summary>
    /// The information that helps implementations recognize behavior inherent to classes of applications.
    /// </summary>
    public VkApplicationInfo? ApplicationInfo;
    /// <summary>
    /// Array of strings containing the names of layers to enable for the created instance.
    /// </summary>
    public string[]? EnabledLayerNames;
    /// <summary>
    /// Array of strings containing the names of extensions to enable.
    /// </summary>
    public string[]? EnabledExtensionNames;

    /// <summary>
    ///  NULL or a pointer to a structure extending this structure.
    /// </summary>
    public unsafe void* pNext;

    /// <summary>
    /// A bitmask indicating behavior of the instance.
    /// </summary>
    public VkInstanceCreateFlags Flags;

    /// <summary>
    /// Initializes a new instance of the <see cref="VkInstanceCreateInfo"/> structure.
    /// </summary>
    /// <param name="appInfo">
    /// The information that helps implementations recognize behavior inherent to classes of applications.
    /// </param>
    /// <param name="enabledLayerNames">
    /// Unicode strings containing the names of layers to enable for the created instance.
    /// </param>
    /// <param name="enabledExtensionNames">
    /// Unicode strings containing the names of extensions to enable.
    /// </param>
    /// <param name="pNext">Is <see cref="null"/> or a pointer to an extension-specific structure.</param>
    /// <param name="flags">A bitmask indicating behavior of the instance.</param>
    public unsafe VkInstanceCreateInfo(
        VkApplicationInfo? appInfo = default,
        string[]? enabledLayerNames = default,
        string[]? enabledExtensionNames = default,
        void* pNext = default,
        VkInstanceCreateFlags flags = VkInstanceCreateFlags.None)
    {
        this.pNext = pNext;
        ApplicationInfo = appInfo;
        EnabledLayerNames = enabledLayerNames;
        EnabledExtensionNames = enabledExtensionNames;
        Flags = flags;
    }

    #region Marshal
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct __Native
    {
        public VkStructureType sType;
        public void* pNext;
        public VkInstanceCreateFlags flags;
        public VkApplicationInfo.__Native* pApplicationInfo;
        public int enabledLayerCount;
        public sbyte** ppEnabledLayerNames;
        public int enabledExtensionCount;
        public sbyte** ppEnabledExtensionNames;

        public readonly void Free()
        {
            if (pApplicationInfo != null)
            {
                pApplicationInfo->Free();
                NativeMemory.Free(pApplicationInfo);
            }

            NativeMemory.Free(ppEnabledLayerNames);
            NativeMemory.Free(ppEnabledExtensionNames);
        }
    }

    internal readonly unsafe void ToNative(out __Native native)
    {
        native.sType = VkStructureType.InstanceCreateInfo;
        native.pNext = pNext;
        native.flags = Flags;

        if (ApplicationInfo.HasValue)
        {
            VkApplicationInfo.__Native* appInfoNative = (VkApplicationInfo.__Native*)NativeMemory.Alloc((nuint)sizeof(VkApplicationInfo.__Native));
            ApplicationInfo.Value.ToNative(appInfoNative);
            native.pApplicationInfo = appInfoNative;
        }
        else
        {
            native.pApplicationInfo = null;
        }

        if (EnabledLayerNames?.Length > 0)
        {
            native.enabledLayerCount = EnabledLayerNames.Length;
            native.ppEnabledLayerNames = Interop.AllocToPointers(EnabledLayerNames!);
        }
        else
        {
            native.enabledLayerCount = 0;
            native.ppEnabledLayerNames = null;
        }

        if (EnabledExtensionNames?.Length > 0)
        {
            native.enabledExtensionCount = EnabledExtensionNames.Length;
            native.ppEnabledExtensionNames = Interop.AllocToPointers(EnabledExtensionNames!);
        }
        else
        {
            native.enabledExtensionCount = 0;
            native.ppEnabledExtensionNames = null;
        }
    }
    #endregion
}
