using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BerryField6AO(SAV6AO sav, Memory<byte> raw) : SaveBlock<SAV6AO>(sav, raw)
{
    public const int Size = 16; // bytes per entry
    public const int Count = 90;
    public const int CountAllocated = 100; // 10 unused slots

    public void ResetAndRandomize(Random rnd, ReadOnlySpan<ushort> choices)
    {
        for (int i = 0; i < Count; i++)
        {
            var plot = GetPlot(i);
            plot.SetAsDefault();
            plot.Berry = choices[rnd.Next(choices.Length)];
        }
    }

    public BerryPlot6AO GetPlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)Count);
        return new BerryPlot6AO(Raw.Slice(index * Size, Size));
    }
}

public sealed class BerryPlot6AO(Memory<byte> raw)
{
    private Span<byte> Data => raw.Span;

    public void SetAsDefault()
    {
        Data.Clear();
        Time = 0;
        Water = 0;
        GrowthStage = 5;
        Count = 4;
        IsDefault = true;
    }

    public bool HasBerry => Berry is not (0 or NoBerry);

    public const ushort NoBerry = ushort.MaxValue;

    // Structure:
    public byte GrowthStage { get => Data[0]; set => Data[0] = value; }
    // alignment
    public ushort Time { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public ushort Water { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }
    public ushort Berry { get => ReadUInt16LittleEndian(Data[6..]); set => WriteUInt16LittleEndian(Data[6..], value); }
    public float Count { get => ReadSingleLittleEndian(Data[8..]); set => WriteSingleLittleEndian(Data[8..], value); }
    public bool IsDefault { get => Data[12] == 1; set => Data[12] = value ? (byte)1 : (byte)0; }
    // .. 3 bytes alignment
}
