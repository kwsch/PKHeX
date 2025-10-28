using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MusicalShow5(Memory<byte> Raw)
{
    public const int SIZE_BW   = 0x1FC00; // footers + align = 0x20000
    public const int SIZE_B2W2 = 0x17C00; // footers + align = 0x18000
    public const string Extension = "pms";

    public Span<byte> Data => Raw.Span;
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);

    public int Length
    {
        get => ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, value);
    }

    public Memory<byte> NARC => Raw.Slice(0x10, Length);
}
