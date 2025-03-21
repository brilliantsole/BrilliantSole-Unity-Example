using System;
using System.Collections.Generic;

public readonly struct BS_ServerMessage
{
    public readonly BS_ServerMessageType Type;
    public readonly List<byte> Data;

    public BS_ServerMessage(BS_ServerMessageType type, List<byte> data)
    {
        Type = type;
        Data = data;
    }

    public BS_ServerMessage(BS_ServerMessageType type)
    {
        Type = type;
        Data = null;
    }

    public readonly ushort Length()
    {
        return (ushort)(3 + DataLength());
    }

    public readonly ushort DataLength()
    {
        return (ushort)(Data?.Count ?? 0);
    }

    public readonly void AppendTo(List<byte> array)
    {
        array.Add(Convert.ToByte(Type));

        ushort dataLength = DataLength();

        array.AddRange(BS_ByteUtils.ToByteArray(dataLength, true));

        if (dataLength > 0 && Data != null)
        {
            array.AddRange(Data);
        }
    }
}
