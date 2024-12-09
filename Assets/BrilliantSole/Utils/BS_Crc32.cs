using System.Collections.Generic;

public static class BS_CRC32
{
    private static readonly uint[] Table;

    static BS_CRC32()
    {
        Table = new uint[256];
        const uint polynomial = 0xedb88320;

        for (uint i = 0; i < 256; i++)
        {
            uint crc = i;
            for (uint j = 8; j > 0; j--)
            {
                if ((crc & 1) == 1)
                    crc = (crc >> 1) ^ polynomial;
                else
                    crc >>= 1;
            }
            Table[i] = crc;
        }
    }

    public static uint Compute(List<byte> bytes)
    {
        uint crc = 0xffffffff;
        foreach (var b in bytes)
        {
            byte tableIndex = (byte)((crc ^ b) & 0xff);
            crc = (crc >> 8) ^ Table[tableIndex];
        }
        return ~crc;
    }
}