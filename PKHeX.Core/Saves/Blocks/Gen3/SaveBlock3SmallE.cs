using System;
using System.ComponentModel.DataAnnotations;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record SaveBlock3SmallE(Memory<byte> Raw) : ISaveBlock3SmallExpansion, ISaveBlock3SmallHoenn
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
    public byte OptionWindowFrame { get => (byte)((OptionsConfig >> 3) & 0b11111); set => OptionsConfig = (uint)((value & 0b11111) << 3) | (OptionsConfig & ~(0b11111u << 2)); }
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

    public uint LinkFlags { get => ReadUInt32LittleEndian(Data[0x0A8..]); set => WriteUInt32LittleEndian(Data[0x0A8..], value); }
    public uint SecurityKey { get => ReadUInt32LittleEndian(Data[0x0AC..]); set => WriteUInt32LittleEndian(Data[0x0AC..], value); }

    // 0xB0: Player Apprentice
    // 0xDC: Apprentices

    // 0x1EC: Berry Crush
    public ushort GetBerryPressSpeed([Range(0, 3)] int index) => ReadUInt16LittleEndian(Data[(0x1EC + (index * 2))..]);
    public void SetBerryPressSpeed([Range(0, 3)] int index, ushort value) => WriteUInt16LittleEndian(Data[(0x1EC + (index * 2))..], value);
    public uint BerryPowder { get => ReadUInt32LittleEndian(Data[0x1F4..]) ^ SecurityKey; set => WriteUInt32LittleEndian(Data[0x1F4..], value ^ SecurityKey); }
    public ushort JoyfulJumpInRow { get => ReadUInt16LittleEndian(Data[0x1FC..]); set => WriteUInt16LittleEndian(Data[0x1FC..], Math.Min((ushort)9999, value)); }
    public ushort JoyfulJump5InRow { get => ReadUInt16LittleEndian(Data[0x200..]); set => WriteUInt16LittleEndian(Data[0x200..], Math.Min((ushort)9999, value)); }
    public ushort JoyfulJumpGamesMaxPlayers { get => ReadUInt16LittleEndian(Data[0x202..]); set => WriteUInt16LittleEndian(Data[0x202..], Math.Min((ushort)9999, value)); }
    public uint JoyfulJumpScore { get => ReadUInt16LittleEndian(Data[0x208..]); set => WriteUInt32LittleEndian(Data[0x208..], Math.Min(99990, value)); }
    public uint JoyfulBerriesScore { get => ReadUInt16LittleEndian(Data[0x20C..]); set => WriteUInt32LittleEndian(Data[0x20C..], Math.Min(99990, value)); }
    public ushort JoyfulBerriesInRow { get => ReadUInt16LittleEndian(Data[0x210..]); set => WriteUInt16LittleEndian(Data[0x210..], Math.Min((ushort)9999, value)); }
    public ushort JoyfulBerries5InRow { get => ReadUInt16LittleEndian(Data[0x212..]); set => WriteUInt16LittleEndian(Data[0x212..], Math.Min((ushort)9999, value)); }

    // Battle Frontier: 0x64C
    public Span<byte> EReaderTrainer => Data.Slice(0xBEC, 0xBC);

    public BattleFrontier3 BattleFrontier => new(Data.Slice(0xCDC, BattleFrontier3.SIZE));

    public ushort BP { get => ReadUInt16LittleEndian(Data[0xEB8..]); set => WriteUInt16LittleEndian(Data[0xEB8..], Math.Min((ushort)9999, value)); }
    public ushort BPEarned { get => ReadUInt16LittleEndian(Data[0xEBA..]); set => WriteUInt16LittleEndian(Data[0xEBA..], value); }
}
