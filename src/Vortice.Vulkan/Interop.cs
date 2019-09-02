// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.


using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan
{
    internal unsafe static class Interop
    {
        public static IntPtr Alloc(int byteCount)
        {
            if (byteCount == 0)
                return IntPtr.Zero;

            return Marshal.AllocHGlobal(byteCount);
        }

        public static IntPtr Alloc<T>(int count = 1) => Alloc(Unsafe.SizeOf<T>() * count);

        public static void Free(void* pointer) => Free(new IntPtr(pointer));

        public static void Free(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                return;

            //RaiseFree(pointer);
            Marshal.FreeHGlobal(pointer);
        }

        public static IntPtr AllocStructToPointer<T>(ref T value) where T : struct
        {
            var ptr = Alloc<T>();
            Unsafe.Copy(ptr.ToPointer(), ref value);
            return ptr;
        }

        public static IntPtr AllocStructToPointer<T>(ref T? value) where T : struct
        {
            if (!value.HasValue)
                return IntPtr.Zero;

            IntPtr ptr = Alloc<T>();
            Unsafe.Write(ptr.ToPointer(), value.Value);
            return ptr;
        }

        public static IntPtr AllocStructToPointer<T>(T[] values) where T : struct
        {
            if (values == null || values.Length == 0)
                return IntPtr.Zero;

            int structSize = Unsafe.SizeOf<T>();
            int totalSize = values.Length * structSize;
            var ptr = Alloc(totalSize);

            var walk = (byte*)ptr;
            for (int i = 0; i < values.Length; i++)
            {
                Unsafe.Copy(walk, ref values[i]);
                walk += structSize;
            }

            return ptr;
        }

        public static IntPtr StringToPointer(string value)
        {
            if (value == null)
                return IntPtr.Zero;

            // Get max number of bytes the string may need.
            int maxSize = GetMaxByteCount(value);
            // Allocate unmanaged memory.
            var managedPtr = Alloc(maxSize);
            var ptr = (byte*)managedPtr;
            // Encode to utf-8, null-terminate and write to unmanaged memory.
            int actualNumberOfBytesWritten;
            fixed (char* ch = value)
                actualNumberOfBytesWritten = Encoding.UTF8.GetBytes(ch, value.Length, ptr, maxSize);
            ptr[actualNumberOfBytesWritten] = 0;
            // Return pointer to the beginning of unmanaged memory.
            return managedPtr;
        }

        public static IntPtr* StringToPointers(string[] values)
        {
            if (values == null || values.Length == 0)
                return null;

            // Allocate unmanaged memory for string pointers.
            var stringHandlesPtr = (IntPtr*)Alloc<IntPtr>(values.Length);

            // Store the pointer to the string.
            for (var i = 0; i < values.Length; i++)
            {
                stringHandlesPtr[i] = StringToPointer(values[i]);
            }

            return stringHandlesPtr;
        }

        public static int GetMaxByteCount(string value)
        {
            return value == null
                ? 0
                : Encoding.UTF8.GetMaxByteCount(value.Length + 1); // +1 for null-terminator.
        }

        /// <summary>
        /// Decodes specified null-terminated UTF-8 bytes into string.
        /// </summary>
        /// <param name="pointer">Pointer to decode from.</param>
        /// <returns>
        /// A string that contains the results of decoding the specified sequence of bytes.
        /// </returns>
        public static string StringFromPointer(byte* pointer)
        {
            if (pointer == null)
                return null;

            // Read until null-terminator.
            byte* walkPtr = pointer;
            while (*walkPtr != 0)
                walkPtr++;

            // Decode UTF-8 bytes to string.
            return Encoding.UTF8.GetString(pointer, (int)(walkPtr - pointer));
        }

        public static void StringToPointer(string value, byte* dstPointer, int maxByteCount)
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
