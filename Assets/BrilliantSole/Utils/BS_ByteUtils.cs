using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class BS_ByteUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ByteUtils", BS_Logger.LogLevel.Log);

    public static T ParseNumber<T>(byte[] data, int offset = 0, bool isLittleEndian = false) where T : unmanaged
    {
        Logger.Log($"Parsing {data.Length} data at offset {offset} as {typeof(T).Name} (isLittleEndian: {isLittleEndian})");

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        int size = Marshal.SizeOf<T>();
        if (offset < 0 || offset + size > data.Length)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of range for the given data.");

        Span<byte> buffer = stackalloc byte[size];
        data.AsSpan(offset, size).CopyTo(buffer);

        if (isLittleEndian != BitConverter.IsLittleEndian)
        {
            buffer.Reverse();
        }

        return MemoryMarshal.Read<T>(buffer);
    }

    public static void WriteNumber<T>(T value, byte[] data, int offset = 0, bool isLittleEndian = false) where T : unmanaged
    {
        Logger.Log($"Writing {value} at offset {offset} as {typeof(T).Name} (isLittleEndian: {isLittleEndian})");

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        int size = Marshal.SizeOf<T>();
        if (offset < 0 || offset + size > data.Length)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of range for the given data.");

        Span<byte> buffer = stackalloc byte[size];
        MemoryMarshal.Write(buffer, ref value);

        if (isLittleEndian != BitConverter.IsLittleEndian)
        {
            buffer.Reverse();
        }

        buffer.CopyTo(data.AsSpan(offset, size));
    }

    public static List<byte> ToByteArray<T>(T value, bool isLittleEndian = false) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] byteArray = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(value, ptr, false);
            Marshal.Copy(ptr, byteArray, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        if (isLittleEndian != BitConverter.IsLittleEndian)
        {
            Array.Reverse(byteArray);
        }

        return new List<byte>(byteArray);
    }
}
