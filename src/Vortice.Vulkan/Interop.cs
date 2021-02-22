// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.


using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan
{
    public unsafe static class Interop
    {
        internal static int GetMaxByteCount(string? value)
        {
            return value == null
                ? 0
                : Encoding.UTF8.GetMaxByteCount(value.Length + 1); // +1 for null-terminator.
        }

        public static string GetString(byte* ptr)
        {
            int length = 0;
            while (length < 4096 && ptr[length] != 0)
            {
                length++;
            }

            // Decode UTF-8 bytes to string.
            return Encoding.UTF8.GetString(ptr, length);
        }

        internal static void StringToPointer(string? value, byte* dstPointer, int maxByteCount)
        {
            if (value == null)
                return;

            int destBytesWritten;
            fixed (char* srcPointer = value)
            {
                destBytesWritten = Encoding.UTF8.GetBytes(srcPointer, value.Length, dstPointer, maxByteCount);
            }

            dstPointer[destBytesWritten] = 0; // Null-terminator.
        }
    }
}
