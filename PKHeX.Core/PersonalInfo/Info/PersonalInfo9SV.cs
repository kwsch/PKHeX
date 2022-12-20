using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.SV"/> games.
/// </summary>
public sealed class PersonalInfo9SV : PersonalInfo, IPersonalAbility12H
{
    public const int SIZE = 0x44;
    private readonly byte[] Data;

    public const int CountTM = 172;

    public PersonalInfo9SV(byte[] data)
    {
        Data = data;
        TMHM = new bool[CountTM];

        var tm = Data.AsSpan(0x2C);
        for (var i = 0; i < CountTM; i++)
            TMHM[i] = FlagUtil.GetFlag(tm, i >> 3, i);
    }

    public override byte[] Write()
    {
        var tm = Data.AsSpan(0x2C);
        for (var i = 0; i < CountTM; i++)
            FlagUtil.SetFlag(tm, i >> 3, i, TMHM[i]);
        return Data;
    }

    public override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
    public override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
    public override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
    public override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
    public override byte Type1 { get => Data[0x06]; set => Data[0x06] = value; }
    public override byte Type2 { get => Data[0x07]; set => Data[0x07] = value; }
    public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
    public override int EvoStage { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    private int EVYield { get => ReadUInt16LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value); }
    public override int EV_HP { get => (EVYield >> 0) & 0x3; set => EVYield = (EVYield & ~(0x3 << 0)) | ((value & 0x3) << 0); }
    public override int EV_ATK { get => (EVYield >> 2) & 0x3; set => EVYield = (EVYield & ~(0x3 << 2)) | ((value & 0x3) << 2); }
    public override int EV_DEF { get => (EVYield >> 4) & 0x3; set => EVYield = (EVYield & ~(0x3 << 4)) | ((value & 0x3) << 4); }
    public override int EV_SPE { get => (EVYield >> 6) & 0x3; set => EVYield = (EVYield & ~(0x3 << 6)) | ((value & 0x3) << 6); }
    public override int EV_SPA { get => (EVYield >> 8) & 0x3; set => EVYield = (EVYield & ~(0x3 << 8)) | ((value & 0x3) << 8); }
    public override int EV_SPD { get => (EVYield >> 10) & 0x3; set => EVYield = (EVYield & ~(0x3 << 10)) | ((value & 0x3) << 10); }
    public override byte Gender         { get => Data[0x0C]; set => Data[0x0C] = value; }
    public override int HatchCycles    { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }
    public override int BaseFriendship { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
    public override byte EXPGrowth     { get => Data[0x0F]; set => Data[0x0F] = value; }
    public override int EggGroup1      { get => Data[0x10]; set => Data[0x10] = (byte)value; }
    public override int EggGroup2      { get => Data[0x11]; set => Data[0x11] = (byte)value; }
    public int Ability1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12), (ushort)value); }
    public int Ability2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14), (ushort)value); }
    public int AbilityH { get => ReadUInt16LittleEndian(Data.AsSpan(0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16), (ushort)value); }
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(0x18), (ushort)value); }
    public override byte FormCount { get => Data[0x1A]; set => Data[0x1A] = value; }
    public override int Color      { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    public bool IsPresentInGame    { get => Data[0x1C] != 0; set => Data[0x1C] = value ? (byte)1 : (byte)0; }
    public byte DexGroup           { get => Data[0x1D]; set => Data[0x1D] = value; }
    public ushort DexIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), value); }
    public override int Height { get => ReadUInt16LittleEndian(Data.AsSpan(0x20)); set => WriteUInt16LittleEndian(Data.AsSpan(0x20), (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data.AsSpan(0x22)); set => WriteUInt16LittleEndian(Data.AsSpan(0x22), (ushort)value); }
    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data.AsSpan(0x24)); set => WriteUInt16LittleEndian(Data.AsSpan(0x24), value); }
    public byte LocalFormIndex { get => (byte)ReadUInt16LittleEndian(Data.AsSpan(0x26)); set => WriteUInt16LittleEndian(Data.AsSpan(0x26), value); } // local region base form
    public ushort RegionalFlags { get => ReadUInt16LittleEndian(Data.AsSpan(0x28)); set => WriteUInt16LittleEndian(Data.AsSpan(0x28), value); }
    public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => RegionalFlags = (ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0)); }
    public ushort RegionalFormIndex { get => (byte)ReadUInt16LittleEndian(Data.AsSpan(0x2A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2A), value); }

    public override int EscapeRate { get => 0; set { } }
    public override int BaseEXP { get => 0; set { } }

    /// <summary>
    /// Gets the Form that any offspring will hatch with, assuming it is holding an Everstone.
    /// </summary>
    public byte HatchFormIndexEverstone => IsRegionalForm ? (byte)RegionalFormIndex : LocalFormIndex;

    /// <summary>
    /// Checks if the entry shows up in any of the built-in Pokédex.
    /// </summary>
    public bool IsInDex => DexIndex != 0;

    public override int AbilityCount => 3;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        2 => AbilityH,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };
}
