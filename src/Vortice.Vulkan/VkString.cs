// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Vulkan.Interop;

namespace Vortice.Vulkan;

public unsafe readonly struct VkString : IDisposable
{
    public readonly byte* Pointer;

    /// <summary>
    /// Size of the byte array that is created from the string.
    /// This value is bigger then the length of the string because '\0' is added at the end of the string and because some UTF8 characters can be longer then 1 byte.
    /// When Size is 0, then this represents a null string or disposed VkString.
    /// </summary>
    public int Size { get; }

    public VkString(string? str)
    {
        if (str == null)
            return; // Preserve Size as 0

        var strSpan = str.GetUtf8Span();
        var strLength = strSpan.Length + 1;
        Pointer = AllocateArray<byte>((uint)strLength);

        var destination = new Span<byte>(Pointer, strLength);
        strSpan.CopyTo(destination);
        destination[strSpan.Length] = 0x00;

        // note that empty string will have Size set to 1 because of added '\0'
        Size = strLength;
    }

    public void Dispose()
    {
        if (Size == 0)
            return;

        Free(Pointer);
    }

    public static unsafe implicit operator byte*(VkString value) => value.Pointer;
    public static unsafe implicit operator IntPtr(VkString value) => new IntPtr(value.Pointer);
    public static implicit operator VkString(string str) => new(str);
    public static implicit operator string?(VkString str) => GetUtf8Span(str).GetString();
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

    public VkStringArray(IList<string> array)
        : this(array.Count)
    {
        for (int i = 0; i < array.Count; i++)
        {
            this[i] = array[i];
        }
    }

    //public VkStringArray(IEnumerable<string> array)
    //    : this(array.Count())
    //{
    //    foreach (string item in array)
    //    {
    //        this[i] = array[i];
    //    }
    //}

    private VkStringArray(int length)
    {
        Length = (uint)length;
#if NET6_0_OR_GREATER
        Pointer = NativeMemory.Alloc((nuint)(sizeof(nint) * length));
#else
        Pointer = (void*)Marshal.AllocHGlobal(sizeof(nint) * length);
#endif
        _data = new VkString[length];
    }

    public readonly uint Length;
    public readonly void* Pointer;

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

#if NET6_0_OR_GREATER
        NativeMemory.Free(Pointer);
#else
        Marshal.FreeHGlobal((IntPtr)Pointer);
#endif
        _disposed = true;
    }

    //private static int MarshalNames(string[] names, out sbyte* namesBuffer)
    //{
    //    nuint sizePerEntry = SizeOf<nuint>() + VK_MAX_EXTENSION_NAME_SIZE;
    //    namesBuffer = AllocateArray<sbyte>((nuint)names.Length * sizePerEntry);

    //    var pCurrent = namesBuffer;

    //    for (var i = 0; i < names.Length; i++)
    //    {
    //        var destination = new Span<byte>(pCurrent + sizeof(nint), (int)VK_MAX_EXTENSION_NAME_SIZE);
    //        var length = Encoding.UTF8.GetBytes(names[i], destination);

    //        pCurrent[sizeof(nuint) + length] = (sbyte)'\0';

    //        *(int*)pCurrent = length;
    //        pCurrent += sizePerEntry;
    //    }

    //    return names.Length;
    //}

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
