using System;
using System.Runtime.InteropServices;

public static class BS_ParseUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ParseUtils", BS_Logger.LogLevel.Log);

    public static T ParseNumber<T>(byte[] data, int offset, bool isLittleEndian) where T : unmanaged
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

    public static void WriteNumber<T>(T value, byte[] data, int offset, bool isLittleEndian) where T : unmanaged
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

    public static byte[] GetSubarray(byte[] data, ushort offset, ushort length)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (offset < 0 || offset + length > data.Length)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset and length are out of range.");

        return data.AsSpan(offset, length).ToArray();
    }

    public static void ParseMessages(byte[] data, Action<byte, byte[]> messageCallback, bool parseMessageLengthAs2data = true)
    {
        Logger.Log($"parsing {data.Length} data");

        ushort byteOffset = 0;
        while (byteOffset < data.Length)
        {
            Logger.Log($"parsing message at {byteOffset}...");

            byte messageType = data[byteOffset++];
            // Logger.Log($"messageType: {messageType}");

            ushort messageDataLength;
            if (parseMessageLengthAs2data)
            {
                messageDataLength = ParseNumber<ushort>(data, byteOffset, true);
                byteOffset += 2;
            }
            else
            {
                messageDataLength = ParseNumber<byte>(data, byteOffset, true);
                byteOffset += 1;
            }
            // Logger.Log($"messageDataLength: {messageDataLength}");
            Logger.Log($"messageType: {messageType}, messageDataLength: {messageDataLength}");

            byte[] messageData = GetSubarray(data, byteOffset, messageDataLength);
            messageCallback?.Invoke(messageType, messageData);

            byteOffset += messageDataLength;
            Logger.Log($"new byteOffset: {byteOffset}");
        }
    }

    static BS_ParseUtils()
    {
        // FILL - set ranges on each Manager
    }
}
