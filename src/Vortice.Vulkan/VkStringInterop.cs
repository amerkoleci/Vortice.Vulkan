// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan;

public unsafe static class VkStringInterop
{
    /// <summary>
    /// Converts a string to an unmanaged version.
    /// </summary>
    /// <param name="managed">The managed string to convert.</param>
    /// <returns>An unmanaged string.</returns>
    public static byte* ConvertToUnmanaged(string? managed)
    {
        if (managed == null)
            return (byte*)null;
        int lengthPlus1 = Encoding.UTF8.GetByteCount(managed) + 1;
        byte* pointer = (byte*)NativeMemory.Alloc((nuint)lengthPlus1);
        var span = new Span<byte>(pointer, lengthPlus1);
        int length = Encoding.UTF8.GetBytes((ReadOnlySpan<char>)managed, span);
        span[length] = 0;
        return pointer;
    }

    /// <summary>Converts an unmanaged string to a managed version.</summary>
    /// <param name="unmanaged">The unmanaged string to convert.</param>
    /// <returns>A managed string.</returns>
    public static string? ConvertToManaged(byte* unmanaged)
    {
        if (unmanaged == null)
            return null;

        return Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(unmanaged));
    }

    /// <summary>Free the memory for a specified unmanaged string.</summary>
    /// <param name="unmanaged">
    /// The memory allocated for the unmanaged string.
    /// </param>
    public static void Free(byte* unmanaged) => NativeMemory.Free(unmanaged);
}
