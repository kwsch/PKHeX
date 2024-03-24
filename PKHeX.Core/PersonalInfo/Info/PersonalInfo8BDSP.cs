using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.BDSP"/> games.
/// </summary>
public sealed class PersonalInfo8BDSP(Memory<byte> Raw)
    : PersonalInfo, IPersonalAbility12H, IPersonalInfoTM, IPersonalInfoTutorType, IPermitRecord
{
    public const int SIZE = 0x44;

    private Span<byte> Data => Raw.Span;
    public override byte[] Write() => Raw.ToArray();

    public bool IsRecordPermitted(int index) => false;
    public ReadOnlySpan<ushort> RecordPermitIndexes => PersonalInfo8SWSH.RecordedMoves;
    public int RecordCountTotal => 112;
    public int RecordCountUsed => PersonalInfo8SWSH.RecordedMoves.Length;

    public override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
    public override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
    public override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
    public override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
    public override byte Type1 { get => Data[0x06]; set => Data[0x06] = value; }
    public override byte Type2 { get => Data[0x07]; set => Data[0x07] = value; }
    public override byte CatchRate { get => Data[0x08]; set => Data[0x08] = value; }
    public override int EvoStage { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    private int EVYield { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], (ushort)value); }
    public override int EV_HP { get => (EVYield >> 0) & 0x3; set => EVYield = (EVYield & ~(0x3 << 0)) | ((value & 0x3) << 0); }
    public override int EV_ATK { get => (EVYield >> 2) & 0x3; set => EVYield = (EVYield & ~(0x3 << 2)) | ((value & 0x3) << 2); }
    public override int EV_DEF { get => (EVYield >> 4) & 0x3; set => EVYield = (EVYield & ~(0x3 << 4)) | ((value & 0x3) << 4); }
    public override int EV_SPE { get => (EVYield >> 6) & 0x3; set => EVYield = (EVYield & ~(0x3 << 6)) | ((value & 0x3) << 6); }
    public override int EV_SPA { get => (EVYield >> 8) & 0x3; set => EVYield = (EVYield & ~(0x3 << 8)) | ((value & 0x3) << 8); }
    public override int EV_SPD { get => (EVYield >> 10) & 0x3; set => EVYield = (EVYield & ~(0x3 << 10)) | ((value & 0x3) << 10); }
    public int Item1 { get => ReadInt16LittleEndian(Data[0x0C..]); set => WriteInt16LittleEndian(Data[0x0C..], (short)value); }
    public int Item2 { get => ReadInt16LittleEndian(Data[0x0E..]); set => WriteInt16LittleEndian(Data[0x0E..], (short)value); }
    public int Item3 { get => ReadInt16LittleEndian(Data[0x10..]); set => WriteInt16LittleEndian(Data[0x10..], (short)value); }
    public override byte Gender { get => Data[0x12]; set => Data[0x12] = value; }
    public override byte HatchCycles { get => Data[0x13]; set => Data[0x13] = value; }
    public override byte BaseFriendship { get => Data[0x14]; set => Data[0x14] = value; }
    public override byte EXPGrowth { get => Data[0x15]; set => Data[0x15] = value; }
    public override int EggGroup1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
    public override int EggGroup2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public int Ability1 { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], (ushort)value); }
    public int Ability2 { get => ReadUInt16LittleEndian(Data[0x1A..]); set => WriteUInt16LittleEndian(Data[0x1A..], (ushort)value); }
    public int AbilityH { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], (ushort)value); }
    public override int EscapeRate { get => 0; set { } } // moved?
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], (ushort)value); }
    public override byte FormCount { get => Data[0x20]; set => Data[0x20] = value; }
    public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
    public bool IsPresentInGame { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
    public override int BaseEXP { get => ReadUInt16LittleEndian(Data[0x22..]); set => WriteUInt16LittleEndian(Data[0x22..], (ushort)value); }
    public override int Height { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], (ushort)value); }

    //public uint TM1 { get => ReadUInt32LittleEndian(Data.Slice(0x28)); set => WriteUInt16LittleEndian(Data.Slice(0x28)); }
    //public uint TM2 { get => ReadUInt32LittleEndian(Data.Slice(0x2C)); set => WriteUInt16LittleEndian(Data.Slice(0x2C)); }
    //public uint TM3 { get => ReadUInt32LittleEndian(Data.Slice(0x30)); set => WriteUInt16LittleEndian(Data.Slice(0x30)); }
    //public uint TM4 { get => ReadUInt32LittleEndian(Data.Slice(0x34)); set => WriteUInt16LittleEndian(Data.Slice(0x34)); }
    //public uint Tutor { get => ReadUInt32LittleEndian(Data.Slice(0x38)); set => WriteUInt16LittleEndian(Data.Slice(0x38)); }

    public ushort Species { get => ReadUInt16LittleEndian(Data[0x3C..]); set => WriteUInt16LittleEndian(Data[0x3C..], value); }
    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data[0x3E..]); set => WriteUInt16LittleEndian(Data[0x3E..], value); }
    public byte HatchFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x40..]); set => WriteUInt16LittleEndian(Data[0x40..], value); }
    public ushort PokeDexIndex { get => ReadUInt16LittleEndian(Data[0x42..]); set => WriteUInt16LittleEndian(Data[0x42..], value); }

    public override int AbilityCount => 3;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        2 => AbilityH,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };

    /// <summary>
    /// Checks if the entry shows up in any of the built-in Pokédex.
    /// </summary>
    public bool IsInDex => PokeDexIndex != 0;

    private const int TMHM = 0x28;
    private const int CountTM = 100;
    private const int ByteCountTM = 112 / 8;
    private const int TypeTutors = 0x38;
    private const int TypeTutorsCount = 4;

    public bool GetIsLearnTM(int index)
    {
        if ((uint)index >= CountTM)
            return false;
        return (Data[0x28 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTM(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountTM);
        if (value)
            Data[TMHM + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TMHM + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public bool GetIsLearnTutorType(int index)
    {
        if ((uint)index >= TypeTutorsCount)
            return false;
        return (Data[TypeTutors + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutorType(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, TypeTutorsCount);
        if (value)
            Data[TypeTutors + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TypeTutors + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTM(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        var span = Data.Slice(TMHM, ByteCountTM);
        for (int index = CountTM - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public void SetAllLearnTutorType(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        var tutor = Data[TypeTutors];
        for (int index = TypeTutorsCount - 1; index >= 0; index--)
        {
            if ((tutor & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }
}
