using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the OR &amp; AS games.
/// </summary>
public sealed class PersonalInfo6AO(Memory<byte> Raw) : PersonalInfo, IPersonalAbility12H, IPersonalInfoTM, IPersonalInfoTutorType
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
    public bool Telekenesis { get => ((EVYield >> 12) & 1) == 1; set => EVYield = (EVYield & ~(0x1 << 12)) | ((value ? 1 : 0) << 12); }
    public int Item1 { get => ReadInt16LittleEndian(Data[0x0C..]); set => WriteInt16LittleEndian(Data[0x0C..], (short)value); }
    public int Item2 { get => ReadInt16LittleEndian(Data[0x0E..]); set => WriteInt16LittleEndian(Data[0x0E..], (short)value); }
    public int Item3 { get => ReadInt16LittleEndian(Data[0x10..]); set => WriteInt16LittleEndian(Data[0x10..], (short)value); }
    public override byte Gender { get => Data[0x12]; set => Data[0x12] = value; }
    public override byte HatchCycles { get => Data[0x13]; set => Data[0x13] = value; }
    public override byte BaseFriendship { get => Data[0x14]; set => Data[0x14] = value; }
    public override byte EXPGrowth { get => Data[0x15]; set => Data[0x15] = value; }
    public override int EggGroup1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
    public override int EggGroup2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public int Ability1 { get => Data[0x18]; set => Data[0x18] = (byte)value; }
    public int Ability2 { get => Data[0x19]; set => Data[0x19] = (byte)value; }
    public int AbilityH { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }

    public override int EscapeRate { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], (ushort)value); }
    public int FormSprite { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], (ushort)value); }
    public override byte FormCount { get => Data[0x20]; set => Data[0x20] = value; }
    public override int Color { get => Data[0x21] & 0x3F; set => Data[0x21] = (byte)((Data[0x21] & 0xC0) | (value & 0x3F)); }
    public bool SpriteFlip { get => ((Data[0x21] >> 6) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x40) | (value ? 0x40 : 0)); }
    public bool SpriteForm { get => ((Data[0x21] >> 7) & 1) == 1; set => Data[0x21] = (byte)((Data[0x21] & ~0x80) | (value ? 0x80 : 0)); }

    public override int BaseEXP { get => ReadUInt16LittleEndian(Data[0x22..]); set => WriteUInt16LittleEndian(Data[0x22..], (ushort)value); }
    public override int Height { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], (ushort)value); }
    public override int Weight { get => ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], (ushort)value); }

    public override int AbilityCount => 3;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : abilityID == AbilityH ? 2 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        2 => AbilityH,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };

    private const int TMHM = 0x28;
    private const int CountTM = 100;
    private const int CountHM = 7;
    private const int CountTMHM = CountTM + CountHM;
    private const int ByteCountTM = (CountTMHM + 7) / 8;
    private const int TypeTutor = 0x38;
    private const int TypeTutorCount = 8;

    public bool GetIsLearnTM(int index)
    {
        if ((uint)index >= CountTMHM)
            return false;
        return (Data[TMHM + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTM(int index, bool value)
    {
        if ((uint)index >= CountTMHM)
            return;
        if (value)
            Data[TMHM + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TMHM + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTM(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        var span = Data.Slice(TMHM, ByteCountTM);
        for (int index = CountTMHM - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public bool GetIsLearnTutorType(int index)
    {
        if ((uint)index >= 8)
            return false;
        return (Data[TypeTutor + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutorType(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, TypeTutorCount);
        if (value)
            Data[TypeTutor + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TypeTutor + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTutorType(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        var tutor = Data[TypeTutor];
        for (int index = TypeTutorCount - 1; index >= 0; index--)
        {
            if ((tutor & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    private const int Tutor1 = 0x40;
    private const int Tutor2 = 0x44;
    private const int Tutor3 = 0x48;
    private const int Tutor4 = 0x4C;

    private const int CountTutor1 = 15;
    private const int CountTutor2 = 17;
    private const int CountTutor3 = 16;
    private const int CountTutor4 = 15;

    public bool GetIsTutor1(int index)
    {
        if ((uint)index >= CountTutor1)
            return false;
        return (Data[Tutor1 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public bool GetIsTutor1(ushort move)
    {
        var index = TutorMoves1.IndexOf(move);
        return index >= 0 && (Data[Tutor1 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutor1(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountTutor1);
        if (value)
            Data[Tutor1 + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[Tutor1 + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public bool GetIsLearnTutor2(int index)
    {
        if ((uint)index >= CountTutor2)
            return false;
        return (Data[Tutor2 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public bool GetIsLearnTutor2(ushort move)
    {
        var index = TutorMoves2.IndexOf(move);
        return index >= 0 && (Data[Tutor2 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutor2(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountTutor2);
        if (value)
            Data[Tutor2 + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[Tutor2 + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public bool GetIsLearnTutor3(int index)
    {
        if ((uint)index >= CountTutor3)
            return false;
        return (Data[Tutor3 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public bool GetIsLearnTutor3(ushort move)
    {
        var index = TutorMoves3.IndexOf(move);
        return index >= 0 && (Data[Tutor3 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutor3(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountTutor3);
        if (value)
            Data[Tutor3 + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[Tutor3 + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public bool GetIsLearnTutor4(int index)
    {
        if ((uint)index >= CountTutor4)
            return false;
        return (Data[Tutor4 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public bool GetIsLearnTutor4(ushort move)
    {
        var index = TutorMoves4.IndexOf(move);
        return index >= 0 && (Data[Tutor4 + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTutor4(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountTutor4);
        if (value)
            Data[Tutor4 + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[Tutor4 + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTutor1(Span<bool> result)
    {
        var moves = TutorMoves1;
        var span = Data.Slice(Tutor1, 4);
        for (int index = CountTutor1 - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public void SetAllLearnTutor2(Span<bool> result)
    {
        var moves = TutorMoves2;
        var span = Data.Slice(Tutor2, 4);
        for (int index = CountTutor2 - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public void SetAllLearnTutor3(Span<bool> result)
    {
        var moves = TutorMoves3;
        var span = Data.Slice(Tutor3, 4);
        for (int index = CountTutor3 - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public void SetAllLearnTutor4(Span<bool> result)
    {
        var moves = TutorMoves4;
        var span = Data.Slice(Tutor4, 4);
        for (int index = CountTutor4 - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    private static ReadOnlySpan<ushort> TutorMoves1 => [ 450, 343, 162, 530, 324, 442, 402, 529, 340, 067, 441, 253, 009, 007, 008 ];
    private static ReadOnlySpan<ushort> TutorMoves2 => [ 277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401, 399, 428, 406, 304, 231 ];
    private static ReadOnlySpan<ushort> TutorMoves3 => [ 020, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409, 355, 264, 351, 352 ]; // Last 3 added, Different from Gen5 Humilau
    private static ReadOnlySpan<ushort> TutorMoves4 => [ 380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446, 214, 285 ];

    public bool GetIsTutorSpecial(ushort move)
    {
        if (GetIsTutor1(move))
            return true;
        if (GetIsLearnTutor2(move))
            return true;
        if (GetIsLearnTutor3(move))
            return true;
        if (GetIsLearnTutor4(move))
            return true;
        return false;
    }

    public void SetAllLearnTutorSpecial(Span<bool> result)
    {
        SetAllLearnTutor1(result);
        SetAllLearnTutor2(result);
        SetAllLearnTutor3(result);
        SetAllLearnTutor4(result);
    }

    public bool GetIsLearnHM(int index)
    {
        if ((uint)index >= CountHM)
            return false;
        index += CountTM;
        return (Data[TMHM + (index >> 3)] & (1 << (index & 7))) != 0;
    }
}
