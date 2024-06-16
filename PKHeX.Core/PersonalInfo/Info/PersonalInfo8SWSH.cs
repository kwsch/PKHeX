using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.SWSH"/> games.
/// </summary>
public sealed class PersonalInfo8SWSH(Memory<byte> Raw) : PersonalInfo, IPersonalAbility12H, IPersonalInfoTM,
    IPersonalInfoTR, IPersonalInfoTutorType, IPermitRecord
{
    public const int SIZE = 0xB0;

    private Span<byte> Data => Raw.Span;
    public override byte[] Write() => Raw.ToArray();

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
    public int FormSprite { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], (ushort)value); } // ???
    public override byte FormCount { get => Data[0x20]; set => Data[0x20] = value; }
    public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
    public bool IsPresentInGame { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
    public bool SpriteForm { get => ((Data[0x21] >> 7) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x80) | (value ? 0x80 : 0)); }
    public override int BaseEXP { get => ReadUInt16LittleEndian(Data[0x22..]); set => WriteUInt16LittleEndian(Data[0x22..], (ushort)value); }
    public override int Height { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], (ushort)value); }

    public ushort Species { get => ReadUInt16LittleEndian(Data[0x4C..]); set => WriteUInt16LittleEndian(Data[0x4C..], value); }

    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data[0x56..]); set => WriteUInt16LittleEndian(Data[0x56..], value); }
    public byte LocalFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x58..]); set => WriteUInt16LittleEndian(Data[0x58..], value); } // local region base form
    public ushort RegionalFlags { get => ReadUInt16LittleEndian(Data[0x5A..]); set => WriteUInt16LittleEndian(Data[0x5A..], value); }
    public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => RegionalFlags = (ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0)); }
    public ushort PokeDexIndex { get => ReadUInt16LittleEndian(Data[0x5C..]); set => WriteUInt16LittleEndian(Data[0x5C..], value); }
    public byte RegionalFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x5E..]); set => WriteUInt16LittleEndian(Data[0x5E..], value); } // form index of this entry
    public ushort ArmorDexIndex { get => ReadUInt16LittleEndian(Data[0xAC..]); set => WriteUInt16LittleEndian(Data[0xAC..], value); }
    public ushort CrownDexIndex { get => ReadUInt16LittleEndian(Data[0xAE..]); set => WriteUInt16LittleEndian(Data[0xAE..], value); }

    /// <summary>
    /// Gets the Form that any offspring will hatch with, assuming it is holding an Everstone.
    /// </summary>
    public byte HatchFormIndexEverstone => IsRegionalForm ? RegionalFormIndex : LocalFormIndex;

    /// <summary>
    /// Checks if the entry shows up in any of the built-in Pokédex.
    /// </summary>
    public bool IsInDex => PokeDexIndex != 0 || ArmorDexIndex != 0 || CrownDexIndex != 0;

    public override int AbilityCount => 3;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        2 => AbilityH,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };

    private const int TM = 0x28;
    private const int TR = 0x3C;
    private const int TutorType = 0x38;
    private const int TutorArmor = 0xA8;
    private const int CountTM = 100;
    private const int CountTR = 100;
    private const int ByteCountTM = (CountTM + 7) / 8;
    private const int CountTutorType = 8;
    private const int CountTutorArmor = 18;

    public bool GetIsLearnTM(int index)
    {
        if ((uint)index >= CountTM)
            return false;
        return (Data[TM + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTM(int index, bool value)
    {
        if ((uint)index >= CountTM)
            return;
        if (value)
            Data[TM + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TM + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTM(Span<bool> result)
    {
        var moves = TM_SWSH;
        var span = Data.Slice(TM, ByteCountTM);
        for (int index = moves.Length - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public bool GetIsLearnTR(int index)
    {
        if ((uint)index >= CountTR)
            return false;
        return (Data[TR + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTR(int index, bool value)
    {
        if ((uint)index >= CountTR)
            return;
        if (value)
            Data[TR + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TR + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public bool GetIsLearnTutorType(int index)
    {
        if ((uint)index >= CountTutorType)
            return false;
        return (Data[TutorType + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutorType(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountTutorType);
        if (value)
            Data[TutorType + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TutorType + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTutorType(Span<bool> result)
    {
        var moves = TypeTutor8;
        var tutor = Data[TutorType];
        for (int index = moves.Length - 1; index >= 0; index--)
        {
            if ((tutor & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public bool GetIsLearnTutorSpecial(int index)
    {
        if ((uint)index >= CountTutorArmor)
            return false;
        return (Data[TutorArmor + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutorSpecial(int index, bool value)
    {
        if ((uint)index >= CountTutorArmor)
            return;
        if (value)
            Data[TutorArmor + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TutorArmor + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTutorSpecial(Span<bool> result)
    {
        var moves = Tutors_SWSH;
        var span = Data.Slice(TutorArmor, 3);
        for (int index = moves.Length - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public void SetAllLearnTR(Span<bool> result)
    {
        var moves = TR_SWSH;
        var span = Data.Slice(TR, ByteCountTM);
        for (int index = moves.Length - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    private static ReadOnlySpan<ushort> TM_SWSH =>
    [
        005, 025, 006, 007, 008, 009, 019, 042, 063, 416,
        345, 076, 669, 083, 086, 091, 103, 113, 115, 219,
        120, 156, 157, 168, 173, 182, 184, 196, 202, 204,
        211, 213, 201, 240, 241, 258, 250, 251, 261, 263,
        129, 270, 279, 280, 286, 291, 311, 313, 317, 328,
        331, 333, 340, 341, 350, 362, 369, 371, 372, 374,
        384, 385, 683, 409, 419, 421, 422, 423, 424, 427,
        433, 472, 478, 440, 474, 490, 496, 506, 512, 514,
        521, 523, 527, 534, 541, 555, 566, 577, 580, 581,
        604, 678, 595, 598, 206, 403, 684, 693, 707, 784,
    ];

    private static ReadOnlySpan<ushort> TR_SWSH =>
    [
        014, 034, 053, 056, 057, 058, 059, 067, 085, 087,
        089, 094, 097, 116, 118, 126, 127, 133, 141, 161,
        164, 179, 188, 191, 200, 473, 203, 214, 224, 226,
        227, 231, 242, 247, 248, 253, 257, 269, 271, 276,
        285, 299, 304, 315, 322, 330, 334, 337, 339, 347,
        348, 349, 360, 370, 390, 394, 396, 398, 399, 402,
        404, 405, 406, 408, 411, 412, 413, 414, 417, 428,
        430, 437, 438, 441, 442, 444, 446, 447, 482, 484,
        486, 492, 500, 502, 503, 526, 528, 529, 535, 542,
        583, 599, 605, 663, 667, 675, 676, 706, 710, 776,
    ];

    private static ReadOnlySpan<ushort> TypeTutor8 =>
    [
        (int)Move.GrassPledge,
        (int)Move.FirePledge,
        (int)Move.WaterPledge,
        (int)Move.FrenzyPlant,
        (int)Move.BlastBurn,
        (int)Move.HydroCannon,
        (int)Move.DracoMeteor,
        (int)Move.SteelBeam,
    ];

    private static ReadOnlySpan<ushort> Tutors_SWSH =>
    [
        805, 807, 812, 804, 803, 813, 811, 810,
        815, 814, 797, 806, 800, 809, 799, 808,
        798, 802,
    ];

    public bool IsRecordPermitted(int index) => GetIsLearnTR(index);

    public bool GetIsLearnTM(ushort move)
    {
        var index = TM_SWSH.IndexOf(move);
        return GetIsLearnTM(index);
    }

    public bool GetIsLearnTutorType(ushort move)
    {
        var index = TypeTutor8.IndexOf(move);
        return GetIsLearnTutorType(index);
    }

    public bool GetIsLearnTutorSpecial(ushort move)
    {
        var index = Tutors_SWSH.IndexOf(move);
        return GetIsLearnTutorSpecial(index);
    }

    public static ReadOnlySpan<ushort> RecordedMoves => TR_SWSH;
    public ReadOnlySpan<ushort> RecordPermitIndexes => TR_SWSH;
    public int RecordCountTotal => 112;
    public int RecordCountUsed => CountTR;
}
