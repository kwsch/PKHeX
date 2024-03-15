using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class EpochDateTime(Memory<byte> Data)
{
    protected Span<byte> Span => Data.Span;
    protected uint RawDate { get => ReadUInt32LittleEndian(Span); set => WriteUInt32LittleEndian(Span, value); }
    protected int RawYear { get => (int)(RawDate & 0xFFF); set => RawDate = (RawDate & 0xFFFFF000) | (uint)(value); }
    public abstract int Year { get; set; }
    protected int RawMonth { get => (int)((RawDate >> 12) & 0xF); set => RawDate = (RawDate & 0xFFFF0FFF) | (((uint)value & 0xF) << 12); }
    public abstract int Month { get; set; }
    public int Day { get => (int)((RawDate >> 16) & 0x1F); set => RawDate = (RawDate & 0xFFE0FFFF) | (((uint)value & 0x1F) << 16); }
    public int Hour { get => (int)((RawDate >> 21) & 0x1F); set => RawDate = (RawDate & 0xFC1FFFFF) | (((uint)value & 0x1F) << 21); }
    public int Minute { get => (int)((RawDate >> 26) & 0x3F); set => RawDate = (RawDate & 0x03FFFFFF) | (((uint)value & 0x3F) << 26); }
    public abstract DateTime Timestamp { get; set; }
    public abstract string DisplayValue { get; }
    public abstract ulong TotalSeconds { get; set; }
}
