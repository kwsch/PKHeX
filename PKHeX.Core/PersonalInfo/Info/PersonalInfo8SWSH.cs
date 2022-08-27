using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.SWSH"/> games.
/// </summary>
public sealed class PersonalInfo8SWSH : PersonalInfo
{
    public const int SIZE = 0xB0;
    public const int CountTM = 100;
    public const int CountTR = 100;
    private readonly byte[] Data;

    public PersonalInfo8SWSH(byte[] data)
    {
        Data = data;
        TMHM = new bool[200];
        for (var i = 0; i < CountTR; i++)
        {
            TMHM[i]           = FlagUtil.GetFlag(Data, 0x28 + (i >> 3), i);
            TMHM[i + CountTM] = FlagUtil.GetFlag(Data, 0x3C + (i >> 3), i);
        }

        // 0x38-0x3B type tutors, but only 8 bits are valid flags.
        var typeTutors = new bool[8];
        for (int i = 0; i < typeTutors.Length; i++)
            typeTutors[i] = FlagUtil.GetFlag(Data, 0x38, i);
        TypeTutors = typeTutors;

        // 0xA8-0xAF are armor type tutors, one bit for each type
        var armorTutors = new bool[18];
        for (int i = 0; i < armorTutors.Length; i++)
            armorTutors[i] = FlagUtil.GetFlag(Data, 0xA8 + (i >> 3), i);
        SpecialTutors = new[]
        {
            armorTutors,
        };
    }

    public override byte[] Write()
    {
        for (var i = 0; i < CountTR; i++)
        {
            FlagUtil.SetFlag(Data, 0x28 + (i >> 3), i, TMHM[i]);
            FlagUtil.SetFlag(Data, 0x3C + (i >> 3), i, TMHM[i + CountTM]);
        }
        for (int i = 0; i < TypeTutors.Length; i++)
            FlagUtil.SetFlag(Data, 0x38, i, TypeTutors[i]);
        for (int i = 0; i < SpecialTutors[0].Length; i++)
            FlagUtil.SetFlag(Data, 0xA8 + (i >> 3), i, SpecialTutors[0][i]);
        return Data;
    }

    public override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
    public override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
    public override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
    public override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
    public override int Type1 { get => Data[0x06]; set => Data[0x06] = (byte)value; }
    public override int Type2 { get => Data[0x07]; set => Data[0x07] = (byte)value; }
    public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
    public override int EvoStage { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    private int EVYield { get => ReadUInt16LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value); }
    public override int EV_HP { get => (EVYield >> 0) & 0x3; set => EVYield = (EVYield & ~(0x3 << 0)) | ((value & 0x3) << 0); }
    public override int EV_ATK { get => (EVYield >> 2) & 0x3; set => EVYield = (EVYield & ~(0x3 << 2)) | ((value & 0x3) << 2); }
    public override int EV_DEF { get => (EVYield >> 4) & 0x3; set => EVYield = (EVYield & ~(0x3 << 4)) | ((value & 0x3) << 4); }
    public override int EV_SPE { get => (EVYield >> 6) & 0x3; set => EVYield = (EVYield & ~(0x3 << 6)) | ((value & 0x3) << 6); }
    public override int EV_SPA { get => (EVYield >> 8) & 0x3; set => EVYield = (EVYield & ~(0x3 << 8)) | ((value & 0x3) << 8); }
    public override int EV_SPD { get => (EVYield >> 10) & 0x3; set => EVYield = (EVYield & ~(0x3 << 10)) | ((value & 0x3) << 10); }
    public int Item1 { get => ReadInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteInt16LittleEndian(Data.AsSpan(0x0C), (short)value); }
    public int Item2 { get => ReadInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteInt16LittleEndian(Data.AsSpan(0x0E), (short)value); }
    public int Item3 { get => ReadInt16LittleEndian(Data.AsSpan(0x10)); set => WriteInt16LittleEndian(Data.AsSpan(0x10), (short)value); }
    public override int Gender { get => Data[0x12]; set => Data[0x12] = (byte)value; }
    public override int HatchCycles { get => Data[0x13]; set => Data[0x13] = (byte)value; }
    public override int BaseFriendship { get => Data[0x14]; set => Data[0x14] = (byte)value; }
    public override int EXPGrowth { get => Data[0x15]; set => Data[0x15] = (byte)value; }
    public override int EggGroup1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
    public override int EggGroup2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public int Ability1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(0x18), (ushort)value); }
    public int Ability2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x1A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1A), (ushort)value); }
    public int AbilityH { get => ReadUInt16LittleEndian(Data.AsSpan(0x1C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1C), (ushort)value); }
    public override int EscapeRate { get => 0; set { } } // moved?
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), (ushort)value); }
    public int FormSprite { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), (ushort)value); } // ???
    public override byte FormCount { get => Data[0x20]; set => Data[0x20] = (byte)value; }
    public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
    public bool IsPresentInGame { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
    public bool SpriteForm { get => ((Data[0x21] >> 7) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x80) | (value ? 0x80 : 0)); }
    public override int BaseEXP { get => ReadUInt16LittleEndian(Data.AsSpan(0x22)); set => WriteUInt16LittleEndian(Data.AsSpan(0x22), (ushort)value); }
    public override int Height { get => ReadUInt16LittleEndian(Data.AsSpan(0x24)); set => WriteUInt16LittleEndian(Data.AsSpan(0x24), (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data.AsSpan(0x26)); set => WriteUInt16LittleEndian(Data.AsSpan(0x26), (ushort)value); }

    public IReadOnlyList<int> Items
    {
        get => new[] { Item1, Item2, Item3 };
        set
        {
            if (value.Count != 3) return;
            Item1 = value[0];
            Item2 = value[1];
            Item3 = value[2];
        }
    }

    public override IReadOnlyList<int> Abilities
    {
        get => new[] { Ability1, Ability2, AbilityH };
        set
        {
            if (value.Count != 3) return;
            Ability1 = value[0];
            Ability2 = value[1];
            AbilityH = value[2];
        }
    }

    public override int GetAbilityIndex(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;

    public ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x4C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x4C), value); }

    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data.AsSpan(0x56)); set => WriteUInt16LittleEndian(Data.AsSpan(0x56), value); }
    public byte LocalFormIndex { get => (byte)ReadUInt16LittleEndian(Data.AsSpan(0x58)); set => WriteUInt16LittleEndian(Data.AsSpan(0x58), value); } // local region base form
    public ushort RegionalFlags { get => ReadUInt16LittleEndian(Data.AsSpan(0x5A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), value); }
    public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => RegionalFlags = (ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0)); }
    public ushort PokeDexIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x5C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5C), value); }
    public byte RegionalFormIndex { get => (byte)ReadUInt16LittleEndian(Data.AsSpan(0x5E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5E), value); } // form index of this entry
    public ushort ArmorDexIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0xAC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xAC), value); }
    public ushort CrownDexIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0xAE)); set => WriteUInt16LittleEndian(Data.AsSpan(0xAE), value); }

    /// <summary>
    /// Gets the Form that any offspring will hatch with, assuming it is holding an Everstone.
    /// </summary>
    public byte HatchFormIndexEverstone => IsRegionalForm ? RegionalFormIndex : LocalFormIndex;

    /// <summary>
    /// Checks if the entry shows up in any of the built-in Pokédex.
    /// </summary>
    public bool IsInDex => PokeDexIndex != 0 || ArmorDexIndex != 0 || CrownDexIndex != 0;

    public bool HasHiddenAbility => AbilityH != Ability1;
}
