using System;

public static class BS_ParseUtils
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ParseUtils", BS_Logger.LogLevel.Warn);

    public static byte[] GetSubarray(byte[] data, ushort offset, ushort length)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (offset < 0 || offset + length > data.Length)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset and length are out of range.");

        return data.AsSpan(offset, length).ToArray();
    }

    public static void ParseMessages(byte[] data, Action<byte, byte[]> messageCallback, ushort offset = 0, bool parseMessageLengthAs2Bytes = true)
    {
        Logger.Log($"parsing {data.Length} data");

        while (offset < data.Length)
        {
            Logger.Log($"parsing message at {offset}...");

            byte messageType = data[offset++];
            // Logger.Log($"messageType: {messageType}");

            ushort messageDataLength;
            if (parseMessageLengthAs2Bytes)
            {
                messageDataLength = BS_ByteUtils.ParseNumber<ushort>(data, offset, true);
                offset += 2;
            }
            else
            {
                messageDataLength = BS_ByteUtils.ParseNumber<byte>(data, offset, true);
                offset += 1;
            }
            // Logger.Log($"messageDataLength: {messageDataLength}");
            Logger.Log($"messageType: {messageType}, messageDataLength: {messageDataLength}");

            byte[] messageData = GetSubarray(data, offset, messageDataLength);
            messageCallback?.Invoke(messageType, messageData);

            offset += messageDataLength;
            Logger.Log($"new offset: {offset}");
        }
    }
}
