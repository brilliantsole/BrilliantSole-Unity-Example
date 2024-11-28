using System;

public static class BS_ParseUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ParseUtils", BS_Logger.LogLevel.Log);

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
                messageDataLength = BS_ByteUtils.ParseNumber<ushort>(data, byteOffset, true);
                byteOffset += 2;
            }
            else
            {
                messageDataLength = BS_ByteUtils.ParseNumber<byte>(data, byteOffset, true);
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
}
