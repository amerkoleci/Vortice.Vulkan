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
        private readonly GCHandle _handle;

        public VkString(string str) 
        {
            var data = Encoding.UTF8.GetBytes(str);
            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Size = data.Length;
        }

        ~VkString()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }

        public unsafe byte* Pointer => (byte*)_handle.AddrOfPinnedObject().ToPointer();
        public readonly int Size;

        private unsafe string GetString()
        {
            return Encoding.UTF8.GetString(Pointer, Size);
        }

        public static unsafe implicit operator byte*(VkString value) => value.Pointer;
        public static unsafe implicit operator IntPtr(VkString value) => new IntPtr(value.Pointer);
        public static implicit operator VkString(string str) => new VkString(str);
        public static implicit operator string(VkString str) => str.GetString();
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
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Marshal.FreeHGlobal(Pointer);
            }
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
