using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record SaveBlock3SmallRS(Memory<byte> Raw) : ISaveBlock3SmallHoenn
{
    public Span<byte> Data => Raw.Span;

    public Span<byte> OriginalTrainerTrash => Data[..8];
    public byte Gender { get => Data[8]; set => Data[8] = value; }
    public uint ID32 { get => ReadUInt32LittleEndian(Data[0x0A..]); set => WriteUInt32LittleEndian(Data[0x0A..], value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data[0xA..]); set => WriteUInt16LittleEndian(Data[0xA..], value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[0xC..]); set => WriteUInt16LittleEndian(Data[0xC..], value); }
    public int PlayedHours { get => ReadUInt16LittleEndian(Data[0xE..]); set => WriteUInt16LittleEndian(Data[0xE..], (ushort)value); }
    public int PlayedMinutes { get => Data[0x10]; set => Data[0x10] = (byte)value; }
    public int PlayedSeconds { get => Data[0x11]; set => Data[0x11] = (byte)value; }
    public byte PlayedFrames { get => Data[0x12]; set => Data[0x12] = value; }

    public byte OptionsButtonMode { get => Data[0x13]; set => Data[0x13] = value; }
    private uint OptionsConfig { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], value); }
    public int TextSpeed { get => (int)(OptionsConfig & 0b111); set => OptionsConfig = (uint)((byte)value & 0b11) | (OptionsConfig & ~0b111u); }
    public byte OptionWindowFrame { get => (byte)((OptionsConfig >> 3) & 0b11111); set => OptionsConfig = (uint)((value & 0b11111) << 3) | (OptionsConfig & ~(0b11111u << 3)); }
    public bool OptionSound { get => (OptionsConfig & 0b1_00000_000) != 0; set => OptionsConfig = value ? (OptionsConfig | 0b1_00000_000) : (OptionsConfig & ~0b1_00000_000u); }
    public bool OptionBattleStyle { get => (OptionsConfig & 0b10_00000_000) != 0; set => OptionsConfig = value ? (OptionsConfig | 0b10_00000_000) : (OptionsConfig & ~0b10_00000_000u); }
    public bool OptionBattleScene { get => (OptionsConfig & 0b100_00000_000) != 0; set => OptionsConfig = value ? (OptionsConfig | 0b100_00000_000) : (OptionsConfig & ~0b100_00000_000u); }
    public bool OptionIsRegionMapZoom { get => (OptionsConfig & 0b1000_00000_000) != 0; set => OptionsConfig = value ? (OptionsConfig | 0b1000_00000_000) : (OptionsConfig & ~0b1000_00000_000u); }

    public byte PokedexSort { get => Data[0x18]; set => Data[0x18] = value; }
    public byte PokedexMode { get => Data[0x19]; set => Data[0x19] = value; }
    public byte PokedexNationalMagicRSE { get => Data[0x1A]; set => Data[0x1A] = value; }
    public byte PokedexNationalMagicFRLG { get => Data[0x1B]; set => Data[0x1B] = value; }
    public uint DexPIDUnown { get => ReadUInt32LittleEndian(Data[0x1C..]); set => WriteUInt32LittleEndian(Data[0x1C..], value); }
    public uint DexPIDSpinda { get => ReadUInt32LittleEndian(Data[0x20..]); set => WriteUInt32LittleEndian(Data[0x20..], value); }

    private Span<byte> ClockInitialSpan => Data.Slice(0x098, RTC3.Size);
    private Span<byte> ClockElapsedSpan => Data.Slice(0x0A0, RTC3.Size);
    public RTC3 ClockInitial { get => new(ClockInitialSpan.ToArray()); set => value.Data.CopyTo(ClockInitialSpan); }
    public RTC3 ClockElapsed { get => new(ClockElapsedSpan.ToArray()); set => value.Data.CopyTo(ClockElapsedSpan); }

    public uint SecurityKey => 0;

    // 0xA8: Battle Tower
    public Span<byte> EReaderTrainer => Data.Slice(0x498, 0xBC);
}
