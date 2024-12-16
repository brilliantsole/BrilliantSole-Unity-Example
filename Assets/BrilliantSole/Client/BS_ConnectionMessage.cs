using System.Collections.Generic;

public readonly struct BS_ConnectionMessage
{
    public readonly byte Type;
    public readonly List<byte> Data;

    public BS_ConnectionMessage(byte type, List<byte> data)
    {
        Type = type;
        Data = data;
    }

    public BS_ConnectionMessage(byte type)
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
        array.Add(Type);

        ushort dataLength = DataLength();

        array.AddRange(BS_ByteUtils.ToByteArray(dataLength, true));

        if (dataLength > 0 && Data != null)
        {
            array.AddRange(Data);
        }
    }
}
