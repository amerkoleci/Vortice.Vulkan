// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Vulkan
{
    public sealed class VkString : IDisposable
    {
        private GCHandle _handle;

        /// <summary>
        /// Size of the byte array that is created from the string.
        /// This value is bigger then the length of the string because '\0' is added at the end of the string and because some UTF8 characters can be longer then 1 byte.
        /// When Size is 0, then this represents a null string or disposed VkString.
        /// </summary>
        public int Size { get; private set; }


        public VkString(string? str) 
        {
            if (str == null)
                return; // Preserve Size as 0

            var data = Encoding.UTF8.GetBytes(str + '\0'); // Vulkan expects '\0' terminated UTF8 string

            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            Size = data.Length; // note that empty string will have Size set to 1 because of added '\0'
        }

        ~VkString() => this.DisposeInt();

        public void Dispose()
        {
            DisposeInt();
            GC.SuppressFinalize(this); // Remove from finalizer queue
        }

        private void DisposeInt()
        {
            if (Size == 0) // Already disposed or not allocated?
                return;

            _handle.Free();
            Size = 0; // This will also mark the VkString as disposed
        }

        public unsafe byte* Pointer
        {
            get
            {
                if (Size == 0)
                    return (byte*)0;

                return (byte*)_handle.AddrOfPinnedObject().ToPointer();
            }
        }

        private unsafe string? GetString()
        {
            if (Size == 0)
                return null;

            return Encoding.UTF8.GetString(Pointer, Size);
        }

        public static unsafe implicit operator byte*(VkString value) => value.Pointer;
        public static unsafe implicit operator IntPtr(VkString value) => new IntPtr(value.Pointer);
        public static implicit operator VkString(string str) => new VkString(str);
        public static implicit operator string?(VkString str) => str.GetString();
    }

    public sealed unsafe class VkStringArray : IDisposable
    {
        private readonly VkString[] _data;
        private bool _disposed;

        public VkStringArray(string[] array) 
            : this(array.Length)
        {
            for (int i = 0; i < array.Length; i++)
                this[i] = array[i];
        }

        public VkStringArray(List<string> array) 
            : this(array.Count)
        {
            for (int i = 0; i < array.Count; i++)
            {
                this[i] = array[i];
            }
        }

        private VkStringArray(int length)
        {
            Length = (uint)length;
            Pointer = Marshal.AllocHGlobal(sizeof(IntPtr) * length);
            _data = new VkString[length];
        }

        public readonly uint Length;
        public readonly IntPtr Pointer;

        ~VkStringArray()
        {
            DisposeInt();
        }

        public void Dispose()
        {
            DisposeInt();
            GC.SuppressFinalize(this); // Remove from finalizer queue
        }

        private void DisposeInt()
        {
            if (_disposed)
                return;

            Marshal.FreeHGlobal(Pointer);
            _disposed = true;
        }

        public VkString this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                return _data[index];
            }

            set
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                _data[index] = value;
                ((byte**)Pointer)[index] = value;
            }
        }

        public static implicit operator byte**(VkStringArray arr) => (byte**)arr.Pointer;
    }
}
