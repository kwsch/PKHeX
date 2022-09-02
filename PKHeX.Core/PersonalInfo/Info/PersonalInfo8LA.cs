using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.PLA"/> games.
/// </summary>
public sealed class PersonalInfo8LA : PersonalInfo, IPersonalAbility12H
{
    public const int SIZE = 0xB0;
    private readonly byte[] Data;

    public PersonalInfo8LA(byte[] data)
    {
        Data = data;
        // TM/TR and Special Tutors are inaccessible; dummy data.

        // 0xA8-0xAF are armor type tutors, one bit for each type
        var moveShop = new bool[Legal.MoveShop8_LA.Length];
        for (int i = 0; i < moveShop.Length; i++)
            moveShop[i] = FlagUtil.GetFlag(Data, 0xA8 + (i >> 3), i);
        SpecialTutors = new[]
        {
            moveShop,
        };
    }

    public override byte[] Write()
    {
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
    public override byte FormCount { get => Data[0x20]; set => Data[0x20] = value; }
    public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
    public bool IsPresentInGame { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
    public bool SpriteForm { get => ((Data[0x21] >> 7) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x80) | (value ? 0x80 : 0)); }
    public override int BaseEXP { get => ReadUInt16LittleEndian(Data.AsSpan(0x22)); set => WriteUInt16LittleEndian(Data.AsSpan(0x22), (ushort)value); }
    public override int Height { get => ReadUInt16LittleEndian(Data.AsSpan(0x24)); set => WriteUInt16LittleEndian(Data.AsSpan(0x24), (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data.AsSpan(0x26)); set => WriteUInt16LittleEndian(Data.AsSpan(0x26), (ushort)value); }

    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data.AsSpan(0x56)); set => WriteUInt16LittleEndian(Data.AsSpan(0x56), value); }
    public int HatchFormIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x58)); set => WriteUInt16LittleEndian(Data.AsSpan(0x58), (ushort)value); } // local region base form
    public ushort RegionalFlags { get => ReadUInt16LittleEndian(Data.AsSpan(0x5A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), value); }
    public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => RegionalFlags = (ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0)); }
    public ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x5C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5C), value); }
    public byte Form { get => Data[0x5E]; set => WriteUInt16LittleEndian(Data.AsSpan(0x5E), value); }
    public ushort DexIndexHisui  { get => ReadUInt16LittleEndian(Data.AsSpan(0x60)); set => WriteUInt16LittleEndian(Data.AsSpan(0x60), value); }
    public ushort DexIndexLocal1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x62)); set => WriteUInt16LittleEndian(Data.AsSpan(0x62), value); }
    public ushort DexIndexLocal2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x64)); set => WriteUInt16LittleEndian(Data.AsSpan(0x64), value); }
    public ushort DexIndexLocal3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x66)); set => WriteUInt16LittleEndian(Data.AsSpan(0x66), value); }
    public ushort DexIndexLocal4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x68)); set => WriteUInt16LittleEndian(Data.AsSpan(0x68), value); }
    public ushort DexIndexLocal5 { get => ReadUInt16LittleEndian(Data.AsSpan(0x6A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x6A), value); }

    public override int AbilityCount => 3;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        2 => AbilityH,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };

    public int GetMoveShopCount()
    {
        // Return a count of true indexes from Tutors
        var arr = SpecialTutors[0];
        int count = 0;
        foreach (var index in arr)
        {
            if (index)
                count++;
        }
        return count;
    }

    public int GetMoveShopIndex(int randIndexFromCount)
    {
        // Return a count of true indexes from Tutors
        var arr = SpecialTutors[0];
        for (var i = 0; i < arr.Length; i++)
        {
            var index = arr[i];
            if (!index)
                continue;
            if (randIndexFromCount-- == 0)
                return i;
        }
        throw new ArgumentOutOfRangeException(nameof(randIndexFromCount));
    }
}
