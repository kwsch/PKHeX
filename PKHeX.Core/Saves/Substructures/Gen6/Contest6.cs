using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Contest6(SAV6AO sav, Memory<byte> raw) : SaveBlock<SAV6AO>(sav, raw)
{
    public const int CountBlock = 12;
    public const uint MaxBlock = 999;

    public uint GetBlockCount(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)CountBlock);
        return ReadUInt32LittleEndian(Data[(index * 4)..]);
    }

    public void SetBlockCount(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)CountBlock);
        if (value > MaxBlock)
            value = MaxBlock;
        WriteUInt32LittleEndian(Data[(index * 4)..], value);
    }

    // 0x48.. ???
}
