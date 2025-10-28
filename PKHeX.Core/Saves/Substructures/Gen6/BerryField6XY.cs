using System;

namespace PKHeX.Core;

public sealed class BerryField6XY(SAV6XY sav, Memory<byte> raw) : SaveBlock<SAV6XY>(sav, raw)
{
    // Is this the same as the BerryField6AO class?

    public const int Size = 16; // bytes per entry
    public const int Count = 32; // 32+25 = 57 allocated?

    private const int PlotStart = 0xC;

    public Span<byte> GetPlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)Count);
        return Data.Slice(PlotStart + (index * Size), Size);
    }
}
