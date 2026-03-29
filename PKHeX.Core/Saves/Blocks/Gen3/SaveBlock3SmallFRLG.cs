using System;
using System.ComponentModel.DataAnnotations;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record SaveBlock3SmallFRLG(Memory<byte> Raw) : ISaveBlock3SmallExpansion
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

    public uint LinkFlags { get => ReadUInt32LittleEndian(Data[0x0A8..]); set => WriteUInt32LittleEndian(Data[0x0A8..], value); }


    public bool DummyFlagTrue
    {
        get => ReadUInt16LittleEndian(Data[0x0AC..]) != 0;
        set => WriteUInt16LittleEndian(Data[0x0AC..], value ? (ushort)1 : (ushort)0);
    }

    public bool DummyFlagFalse
    {
        get => ReadUInt16LittleEndian(Data[0x0AE..]) == 0;
        set => WriteUInt16LittleEndian(Data[0x0AE..], value ? (ushort)0 : (ushort)1);
    }

    public void FixDummyFlags()
    {
        DummyFlagTrue = true;
        DummyFlagFalse = false;
    }

    // Battle Tower: 0xB0

    public Span<byte> EReaderTrainer => Data.Slice(0x4A0, 0xBC);

    // 0xAF0: Berry Crush
    public ushort GetBerryPressSpeed([Range(0,3)] int index) => ReadUInt16LittleEndian(Data[(0xAF0 + (index * 2))..]);
    public void SetBerryPressSpeed([Range(0, 3)] int index, ushort value) => WriteUInt16LittleEndian(Data[(0xAF0 + (index * 2))..], value);
    public uint BerryPowder { get => ReadUInt32LittleEndian(Data[0xAF8..]) ^ SecurityKey; set => WriteUInt32LittleEndian(Data[0xAF8..], value ^ SecurityKey); }
    public ushort JoyfulJumpInRow { get => ReadUInt16LittleEndian(Data[0xB00..]); set => WriteUInt16LittleEndian(Data[0xB00..], Math.Min((ushort)9999, value)); }
    public ushort JoyfulJump5InRow { get => ReadUInt16LittleEndian(Data[0xB04..]); set => WriteUInt16LittleEndian(Data[0xB04..], Math.Min((ushort)9999, value)); }
    public ushort JoyfulJumpGamesMaxPlayers { get => ReadUInt16LittleEndian(Data[0xB06..]); set => WriteUInt16LittleEndian(Data[0xB06..], Math.Min((ushort)9999, value)); }
    public uint JoyfulJumpScore { get => ReadUInt16LittleEndian(Data[0xB0C..]); set => WriteUInt32LittleEndian(Data[0xB0C..], Math.Min(99990, value)); }
    public uint JoyfulBerriesScore { get => ReadUInt16LittleEndian(Data[0xB10..]); set => WriteUInt32LittleEndian(Data[0xB10..], Math.Min(99990, value)); }
    public ushort JoyfulBerriesInRow { get => ReadUInt16LittleEndian(Data[0xB14..]); set => WriteUInt16LittleEndian(Data[0xB14..], Math.Min((ushort)9999, value)); }
    public ushort JoyfulBerries5InRow { get => ReadUInt16LittleEndian(Data[0xB16..]); set => WriteUInt16LittleEndian(Data[0xB16..], Math.Min((ushort)9999, value)); }

    public uint SecurityKey
    {
        get => ReadUInt32LittleEndian(Data[0xF20..]);
        set => WriteUInt32LittleEndian(Data[0xF20..], value);
    }
}
