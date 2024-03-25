using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.SV"/> games.
/// </summary>
public sealed class PersonalInfo9SV(Memory<byte> Raw) : PersonalInfo, IPersonalAbility12H, IPersonalInfoTM, IPermitRecord
{
    public const int SIZE = 0x4C;

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
    public override byte Gender         { get => Data[0x0C]; set => Data[0x0C] = value; }
    public override byte HatchCycles    { get => Data[0x0D]; set => Data[0x0D] = value; }
    public override byte BaseFriendship { get => Data[0x0E]; set => Data[0x0E] = value; }
    public override byte EXPGrowth     { get => Data[0x0F]; set => Data[0x0F] = value; }
    public override int EggGroup1      { get => Data[0x10]; set => Data[0x10] = (byte)value; }
    public override int EggGroup2      { get => Data[0x11]; set => Data[0x11] = (byte)value; }
    public int Ability1 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], (ushort)value); }
    public int Ability2 { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], (ushort)value); }
    public int AbilityH { get => ReadUInt16LittleEndian(Data[0x16..]); set => WriteUInt16LittleEndian(Data[0x16..], (ushort)value); }
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], (ushort)value); }
    public override byte FormCount { get => Data[0x1A]; set => Data[0x1A] = value; }
    public override int Color      { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    public bool IsPresentInGame    { get => Data[0x1C] != 0; set => Data[0x1C] = value ? (byte)1 : (byte)0; }
    public byte DexGroup           { get => Data[0x1D]; set => Data[0x1D] = value; }
    public ushort DexPaldea { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], value); }
    public override int Height { get => ReadUInt16LittleEndian(Data[0x20..]); set => WriteUInt16LittleEndian(Data[0x20..], (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data[0x22..]); set => WriteUInt16LittleEndian(Data[0x22..], (ushort)value); }
    public ushort HatchSpecies { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], value); }
    public byte LocalFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], value); } // local region base form
    public ushort RegionalFlags { get => ReadUInt16LittleEndian(Data[0x28..]); set => WriteUInt16LittleEndian(Data[0x28..], value); }
    public bool IsRegionalForm { get => (RegionalFlags & 1) == 1; set => RegionalFlags = (ushort)((RegionalFlags & 0xFFFE) | (value ? 1 : 0)); }
    public ushort RegionalFormIndex { get => (byte)ReadUInt16LittleEndian(Data[0x2A..]); set => WriteUInt16LittleEndian(Data[0x2A..], value); }

    public override int EscapeRate { get => 0; set { } }
    public override int BaseEXP { get => 0; set { } }

    /// <summary>
    /// Gets the Form that any offspring will hatch with, assuming it is holding an Everstone.
    /// </summary>
    public byte HatchFormIndexEverstone => IsRegionalForm ? (byte)RegionalFormIndex : LocalFormIndex;

    /// <summary>
    /// Checks if the entry shows up in any of the built-in Pokédex.
    /// </summary>
    public bool IsInDex => DexPaldea != 0 || DexKitakami != 0 || DexBlueberry != 0;

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
        var moves = TM_SV;
        var span = Data.Slice(TM, ByteCountTM);
        for (int index = CountTM - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public bool IsRecordPermitted(int index) => GetIsLearnTM(index);

    private const int COUNT_RECORD_BASE = 200; // Up to 200 TM flags, but not all are used.
    private const int COUNT_RECORD_DLC = 104; // 13 additional bytes allocated for DLC1/2 TM Flags
    public ReadOnlySpan<ushort> RecordPermitIndexes => TM_SV;
    public int RecordCountTotal => COUNT_RECORD_BASE + COUNT_RECORD_DLC;
    public int RecordCountUsed => CountTM;

    private static ReadOnlySpan<ushort> TM_SV =>
    [
        005, 036, 204, 313, 097, 189, 184, 182, 424, 422,
        423, 352, 067, 491, 512, 522, 060, 109, 168, 574,
        885, 884, 886, 451, 083, 263, 342, 332, 523, 506,
        555, 232, 129, 345, 196, 341, 317, 577, 488, 490,
        314, 500, 101, 374, 525, 474, 419, 203, 521, 241,
        240, 201, 883, 684, 473, 091, 331, 206, 280, 428,
        369, 421, 492, 706, 339, 403, 034, 007, 009, 008,
        214, 402, 486, 409, 115, 113, 350, 127, 337, 605,
        118, 447, 086, 398, 707, 156, 157, 269, 014, 776,
        191, 390, 286, 430, 399, 141, 598, 019, 285, 442,
        349, 408, 441, 164, 334, 404, 529, 261, 242, 271,
        710, 202, 396, 366, 247, 406, 446, 304, 257, 412,
        094, 484, 227, 057, 861, 053, 085, 583, 133, 347,
        270, 676, 226, 414, 179, 058, 604, 580, 678, 581,
        417, 126, 056, 059, 519, 518, 520, 528, 188, 089,
        444, 566, 416, 307, 308, 338, 200, 315, 411, 437,
        542, 433, 405, 063, 413, 394, 087, 370, 076, 434,
        796, 851, 046, 268, 114, 092, 328, 180, 356, 479,
        360, 282, 450, 162, 410, 679, 667, 333, 503, 535,
        669, 253, 264, 311, 803, 807, 812, 814, 809, 808,
        799, 802, 220, 244, 038, 283, 572, 915, 250, 330,
        916, 527, 813, 811, 482, 815, 297, 248, 797, 806,
        800, 675, 784, 319, 174, 912, 913, 914, 917, 918,
    ];

    public byte DexKitakami { get => Data[0x4A]; set => Data[0x4A] = value; }
    public byte DexBlueberry { get => Data[0x4B]; set => Data[0x4B] = value; }
}
