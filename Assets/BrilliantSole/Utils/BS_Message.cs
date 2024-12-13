using System;
using System.Collections.Generic;

public readonly struct BS_Message<T> where T : unmanaged, Enum
{
    public readonly T Type;
    public readonly List<byte> Data;

    public BS_Message(T type, List<byte> data)
    {
        Type = type;
        Data = data;
    }

    public BS_Message(T type)
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