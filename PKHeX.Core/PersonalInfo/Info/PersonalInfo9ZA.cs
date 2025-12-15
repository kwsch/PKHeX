using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.ZA"/> games.
/// </summary>
public sealed class PersonalInfo9ZA(Memory<byte> Raw) : PersonalInfo, IPersonalAbility12H, IPersonalInfoTM, IPermitPlus
{
    public const int SIZE = 0x50;

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
    public override byte Gender { get => Data[0x0C]; set => Data[0x0C] = value; }
    public override byte HatchCycles { get => Data[0x0D]; set => Data[0x0D] = value; }
    public override byte BaseFriendship { get => Data[0x0E]; set => Data[0x0E] = value; }
    public override byte EXPGrowth { get => Data[0x0F]; set => Data[0x0F] = value; }
    public override int EggGroup1 { get => Data[0x10]; set => Data[0x10] = (byte)value; }
    public override int EggGroup2 { get => Data[0x11]; set => Data[0x11] = (byte)value; }
    public int Ability1 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], (ushort)value); }
    public int Ability2 { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], (ushort)value); }
    public int AbilityH { get => ReadUInt16LittleEndian(Data[0x16..]); set => WriteUInt16LittleEndian(Data[0x16..], (ushort)value); }
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], (ushort)value); }
    public override byte FormCount { get => Data[0x1A]; set => Data[0x1A] = value; }
    public override int Color { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    public bool IsPresentInGame { get => Data[0x1C] != 0; set => Data[0x1C] = value ? (byte)1 : (byte)0; }
    // 0x1D unused
    public ushort DexIndex { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], value); }
    public override int Height { get => ReadUInt16LittleEndian(Data[0x20..]); set => WriteUInt16LittleEndian(Data[0x20..], (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data[0x22..]); set => WriteUInt16LittleEndian(Data[0x22..], (ushort)value); }
    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], value); } // ZA: no eggs, but we'll retain it
    public byte LocalFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], value); } // local region base form
    public ushort RegionalFlags { get => ReadUInt16LittleEndian(Data[0x28..]); set => WriteUInt16LittleEndian(Data[0x28..], value); }
    public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => RegionalFlags = (ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0)); }
    public ushort RegionalFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x2A..]); set => WriteUInt16LittleEndian(Data[0x2A..], value); }

    public override int EscapeRate { get => 0; set { } }

    /// <summary>
    /// Gets the Form that any offspring will hatch with, assuming it is holding an Everstone.
    /// </summary>
    public byte HatchFormIndexEverstone => IsRegionalForm ? (byte)RegionalFormIndex : LocalFormIndex;

    /// <summary>
    /// Checks if the entry shows up in any of the built-in Pokédex.
    /// </summary>
    public bool IsInDex => DexIndex != 0;
    public bool IsLumioseNative => DexIndex is (not 0) and <= 232;
    public bool IsHyperspaceNative => DexIndex > 232;

    public override int AbilityCount => 3;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        2 => AbilityH,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };

    private const int TM = 0x2C;
    private const int CountTM = 230;
    private const int ByteCountTM = (CountTM + 7) / 8;

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
        var moves = MachineMoves;
        var span = Data.Slice(TM, ByteCountTM);
        for (int index = CountTM - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public bool IsRecordPermitted(int index) => false; // Game never sets these flags, as TMs are infinite use.

    private const int COUNT_RECORD_BASE = 200; // Up to 200 TM flags, but not all are used.
    private const int COUNT_RECORD_DLC = 104; // 13 additional bytes allocated for DLC1/2 TM Flags
    public ReadOnlySpan<ushort> RecordPermitIndexes => MachineMoves;
    public int RecordCountTotal => COUNT_RECORD_BASE + COUNT_RECORD_DLC;
    public int RecordCountUsed => CountTM;

    /// <summary>
    /// Technical Machine moves corresponding to their index within TM bitflag permissions.
    /// </summary>
    public static ReadOnlySpan<ushort> MachineMoves =>
    [
        029, 337, 473, 249, 046, 347, 092, 086, 812, 280,
        339, 157, 058, 424, 423, 113, 182, 612, 408, 583,
        422, 332, 009, 008, 242, 412, 129, 091, 007, 014,
        115, 104, 034, 400, 203, 317, 446, 126, 435, 331,
        352, 202, 019, 063, 282, 341, 097, 120, 196, 315,
        219, 414, 188, 434, 416, 038, 261, 442, 428, 248,
        421, 053, 094, 076, 444, 521, 085, 257, 089, 250,
        304, 083, 057, 247, 406, 710, 398, 523, 542, 334,
        404, 369, 417, 430, 164, 528, 231, 191, 390, 399,
        174, 605, 200, 018, 269, 056, 377, 127, 118, 441,
        527, 411, 526, 394, 059, 087, 370,
        // ZA DLC
                                           004, 263, 886,
        047, 491, 490, 488, 885, 006, 318, 325, 466, 246,
        259, 206, 305, 706, 102, 443, 138, 402, 509, 451,
        409, 458, 299, 814, 530, 815, 480, 524, 207, 330,
        252, 660, 799, 813, 013, 130, 161, 503, 333, 410,
        080, 669, 143, 090, 329, 800, 796, 307, 308, 338,
    ];

    public override int BaseEXP { get => ReadUInt16LittleEndian(Data[0x4C..]); set => WriteUInt16LittleEndian(Data[0x4C..], (ushort)value); }
    public ushort AlphaMove { get => ReadUInt16LittleEndian(Data[0x4E..]); set => WriteUInt16LittleEndian(Data[0x4E..], value); }

    public int PlusCountTotal => (33 + 12) * 8; // 360
    public int PlusCountUsed => 340; // as of Mega Dimension DLC 2.0.0

    // Set by Seed of Mastery or Alpha Move granted
    public static ReadOnlySpan<ushort> PlusMoves =>
    [
        007, 008, 009, 014, 016, 017, 018, 019, 022, 029,
        033, 034, 036, 038, 039, 040, 042, 043, 044, 045,
        046, 048, 052, 053, 054, 055, 056, 057, 058, 059,
        060, 061, 063, 064, 071, 073, 074, 075, 076, 077,
        078, 079, 081, 083, 084, 085, 086, 087, 088, 089,
        091, 092, 093, 094, 095,      098, 100, 103, 104,
        105, 106, 108, 109, 113, 114, 115, 116, 118, 120,
        122, 126, 127, 129, 133, 137, 141, 150, 151, 153,
        157, 162, 163, 164, 172, 174, 182, 183, 188, 191,
        192, 195, 196, 197, 200, 202, 203, 204, 205, 209,
        211, 219, 223, 224, 225, 231, 232, 234, 235, 236,
        239, 242, 245, 247, 248, 249, 250, 257, 261, 268,
        269, 273, 280, 282, 297, 304, 313, 315, 317, 319,
        328, 331, 332, 334, 337, 339, 340, 341, 344, 345,
        347, 348, 350, 352, 369, 370, 377, 390, 392, 394,
        396, 398, 399, 400, 403, 404, 405, 406, 407, 408,
        411, 412, 413, 414, 416, 417, 418, 420, 421, 422,
        423, 424, 425, 427, 428, 430, 434, 435, 436, 437,
        438, 441, 442, 444, 446, 452, 453, 457, 473, 482,
        484, 521, 523, 526, 527, 528, 529, 532, 535, 538,
        540, 542, 555, 556, 560, 564, 566, 567, 570, 571,
        573, 574, 575, 576, 577, 583, 584, 585, 586, 588,
        591, 592, 593, 594, 595, 596, 598, 601, 605, 609,
        611, 612, 613, 614, 615, 616, 617, 621, 670, 679,
        687, 693, 710, 748, 784, 812, 920,
        097, // lol
             004, 006, 013, 047, 080, 090, 102, 130, 138,
        143, 147, 155, 160, 161, 176, 206, 207, 246, 252,
        259, 263, 295, 296, 299, 305, 307, 308, 318, 325,
        329, 330, 333, 338, 402, 409, 410, 443, 451, 458,
        463, 464, 466, 480, 488, 490, 491, 503, 509, 524,
        530, 533, 546, 547, 548, 618, 619, 620, 659, 660,
        665, 669, 705, 706, 708, 712, 721, 742, 753, 783,
        786, 794, 796, 799, 800, 813, 814, 815, 830, 839,
        854, 856, 858, 862, 864, 866, 874, 880, 885, 886,
        889, 890, 891, 893,
    ];

    public ReadOnlySpan<ushort> PlusMoveIndexes => PlusMoves;
}
