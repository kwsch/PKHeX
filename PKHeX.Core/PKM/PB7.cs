using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 7 <see cref="PKM"/> format used for <see cref="GameVersion.GG"/>. </summary>
public sealed class PB7 : G6PKM, IHyperTrain, IAwakened, IScaledSizeValue, ICombatPower, IFavorite, IFormArgument
{
    public static readonly ushort[] Unused =
    {
        0x2A, // Old Marking Value (PelagoEventStatus)
        0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, // Unused Ribbons
        0x58, 0x59, // Nickname Terminator
        0x73,
        0x90, 0x91, // HT Terminator
        0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F, 0xA0, 0xA1, // Old Geolocation/memories
        0xA7, 0xAA, 0xAB,
        0xAC, 0xAD, // Fatigue, no GUI editing
        0xC8, 0xC9, // OT Terminator
    };

    public override IReadOnlyList<ushort> ExtraBytes => Unused;

    public override int SIZE_PARTY => SIZE;
    public override int SIZE_STORED => SIZE;
    private const int SIZE = 260;
    public override EntityContext Context => EntityContext.Gen7b;
    public override PersonalInfo PersonalInfo => PersonalTable.GG.GetFormEntry(Species, Form);

    public PB7() : base(SIZE) { }
    public PB7(byte[] data) : base(DecryptParty(data)) { }

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted67(ref data);
        if (data.Length != SIZE)
            Array.Resize(ref data, SIZE);
        return data;
    }

    public override PKM Clone() => new PB7((byte[])Data.Clone());

    // Structure
    #region Block A
    public override uint EncryptionConstant
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x00));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value);
    }

    public override ushort Sanity
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x04));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x04), value);
    }

    public override ushort Checksum
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x06));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x06), value);
    }

    public override ushort Species
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x08));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x08), value);
    }

    public override int HeldItem
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value);
    }

    public override int TID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), (ushort)value);
    }

    public override int SID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), (ushort)value);
    }

    public override uint EXP
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x10));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value);
    }

    public override int Ability { get => Data[0x14]; set => Data[0x14] = (byte)value; }
    public override int AbilityNumber { get => Data[0x15] & 7; set => Data[0x15] = (byte)((Data[0x15] & ~7) | (value & 7)); }
    public bool Favorite { get => (Data[0x15] & 8) != 0; set => Data[0x15] = (byte)((Data[0x15] & ~8) | ((value ? 1 : 0) << 3)); }
    public override int MarkValue { get => ReadUInt16LittleEndian(Data.AsSpan(0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16), (ushort)value); }

    public override uint PID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x18));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x18), value);
    }

    public override int Nature { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
    public override bool FatefulEncounter { get => (Data[0x1D] & 1) == 1; set => Data[0x1D] = (byte)((Data[0x1D] & ~0x01) | (value ? 1 : 0)); }
    public override int Gender { get => (Data[0x1D] >> 1) & 0x3; set => Data[0x1D] = (byte)((Data[0x1D] & ~0x06) | (value << 1)); }
    public override byte Form { get => (byte)(Data[0x1D] >> 3); set => Data[0x1D] = (byte)((Data[0x1D] & 0x07) | (value << 3)); }
    public override int EV_HP { get => Data[0x1E]; set => Data[0x1E] = (byte)value; }
    public override int EV_ATK { get => Data[0x1F]; set => Data[0x1F] = (byte)value; }
    public override int EV_DEF { get => Data[0x20]; set => Data[0x20] = (byte)value; }
    public override int EV_SPE { get => Data[0x21]; set => Data[0x21] = (byte)value; }
    public override int EV_SPA { get => Data[0x22]; set => Data[0x22] = (byte)value; }
    public override int EV_SPD { get => Data[0x23]; set => Data[0x23] = (byte)value; }
    public byte AV_HP  { get => Data[0x24]; set => Data[0x24] = value; }
    public byte AV_ATK { get => Data[0x25]; set => Data[0x25] = value; }
    public byte AV_DEF { get => Data[0x26]; set => Data[0x26] = value; }
    public byte AV_SPE { get => Data[0x27]; set => Data[0x27] = value; }
    public byte AV_SPA { get => Data[0x28]; set => Data[0x28] = value; }
    public byte AV_SPD { get => Data[0x29]; set => Data[0x29] = value; }
    public ResortEventState ResortEventStatus { get => (ResortEventState)Data[0x2A]; set => Data[0x2A] = (byte)value; }
    private byte PKRS { get => Data[0x2B]; set => Data[0x2B] = value; }
    public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
    public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }
    public float HeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(0x2C)); set => WriteSingleLittleEndian(Data.AsSpan(0x2C), value); }
    // 0x38 Unused
    // 0x39 Unused
    public byte HeightScalar { get => Data[0x3A]; set => Data[0x3A] = value; }
    public byte WeightScalar { get => Data[0x3B]; set => Data[0x3B] = value; }
    public uint FormArgument { get => ReadUInt32LittleEndian(Data.AsSpan(0x3C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x3C), value); }
    public byte FormArgumentRemain { get => (byte)FormArgument; set => FormArgument = (FormArgument & ~0xFFu) | value; }
    public byte FormArgumentElapsed { get => (byte)(FormArgument >> 8); set => FormArgument = (FormArgument & ~0xFF00u) | (uint)(value << 8); }
    public byte FormArgumentMaximum { get => (byte)(FormArgument >> 16); set => FormArgument = (FormArgument & ~0xFF0000u) | (uint)(value << 16); }

    #endregion
    #region Block B
    public override string Nickname
    {
        get => StringConverter8.GetString(Nickname_Trash);
        set => StringConverter8.SetString(Nickname_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override ushort Move1
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x5A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), value);
    }

    public override ushort Move2
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x5C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x5C), value);
    }

    public override ushort Move3
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x5E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x5E), value);
    }

    public override ushort Move4
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x60));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x60), value);
    }

    public override int Move1_PP { get => Data[0x62]; set => Data[0x62] = (byte)value; }
    public override int Move2_PP { get => Data[0x63]; set => Data[0x63] = (byte)value; }
    public override int Move3_PP { get => Data[0x64]; set => Data[0x64] = (byte)value; }
    public override int Move4_PP { get => Data[0x65]; set => Data[0x65] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x66]; set => Data[0x66] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x67]; set => Data[0x67] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x68]; set => Data[0x68] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x69]; set => Data[0x69] = (byte)value; }

    public override ushort RelearnMove1
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x6A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x6A), value);
    }

    public override ushort RelearnMove2
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x6C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x6C), value);
    }

    public override ushort RelearnMove3
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x6E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x6E), value);
    }

    public override ushort RelearnMove4
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x70));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x70), value);
    }

    // 0x72 Unused
    // 0x73 Unused
    private uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x74)); set => WriteUInt32LittleEndian(Data.AsSpan(0x74), value); }
    public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
    public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
    public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }
    #endregion
    #region Block C
    public override string HT_Name
    {
        get => StringConverter8.GetString(HT_Trash);
        set => StringConverter8.SetString(HT_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override int HT_Gender { get => Data[0x92]; set => Data[0x92] = (byte)value; }
    public override int CurrentHandler { get => Data[0x93]; set => Data[0x93] = (byte)value; }
    // 0x94 Unused
    // 0x95 Unused
    // 0x96 Unused
    // 0x97 Unused
    // 0x98 Unused
    // 0x99 Unused
    // 0x9A Unused
    // 0x9B Unused
    // 0x9C Unused
    // 0x9D Unused
    // 0x9E Unused
    // 0x9F Unused
    // 0xA0 Unused
    // 0xA1 Unused
    public override int HT_Friendship { get => Data[0xA2]; set => Data[0xA2] = (byte)value; }
    // 0xA1 HT_Affection Unused
    public int HT_Intensity { get => Data[0xA4]; set => Data[0xA4] = (byte)value; }
    public int HT_Memory { get => Data[0xA5]; set => Data[0xA5] = (byte)value; }
    public int HT_Feeling { get => Data[0xA6]; set => Data[0xA6] = (byte)value; }
    // 0xA7 Unused
    public int HT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(0xA8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xA8), (ushort)value); }
    // 0xAA Unused
    // 0xAB Unused
    public byte FieldEventFatigue1 { get => Data[0xAC]; set => Data[0xAC] = value; }
    public byte FieldEventFatigue2 { get => Data[0xAD]; set => Data[0xAD] = value; }
    public override byte Fullness { get => Data[0xAE]; set => Data[0xAE] = value; }
    public override byte Enjoyment { get => Data[0xAF]; set => Data[0xAF] = value; }
    #endregion
    #region Block D
    public override string OT_Name
    {
        get => StringConverter8.GetString(OT_Trash);
        set => StringConverter8.SetString(OT_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override int OT_Friendship { get => Data[0xCA]; set => Data[0xCA] = (byte)value; }
    // 0xCB Unused
    // 0xCC Unused
    // 0xCD Unused
    // 0xCE Unused
    // 0xCF Unused
    // 0xD0 Unused
    public override int Egg_Year { get => Data[0xD1]; set => Data[0xD1] = (byte)value; }
    public override int Egg_Month { get => Data[0xD2]; set => Data[0xD2] = (byte)value; }
    public override int Egg_Day { get => Data[0xD3]; set => Data[0xD3] = (byte)value; }
    public override int Met_Year { get => Data[0xD4]; set => Data[0xD4] = (byte)value; }
    public override int Met_Month { get => Data[0xD5]; set => Data[0xD5] = (byte)value; }
    public override int Met_Day { get => Data[0xD6]; set => Data[0xD6] = (byte)value; }
    public int Rank { get => Data[0xD7]; set => Data[0xD7] = (byte)value; } // unused but fetched for stat calcs, and set for trpoke data?
    public override int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(0xD8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xD8), (ushort)value); }
    public override int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(0xDA)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDA), (ushort)value); }
    public override int Ball { get => Data[0xDC]; set => Data[0xDC] = (byte)value; }
    public override int Met_Level { get => Data[0xDD] & ~0x80; set => Data[0xDD] = (byte)((Data[0xDD] & 0x80) | value); }
    public override int OT_Gender { get => Data[0xDD] >> 7; set => Data[0xDD] = (byte)((Data[0xDD] & ~0x80) | (value << 7)); }
    public byte HyperTrainFlags { get => Data[0xDE]; set => Data[0xDE] = value; }
    public bool HT_HP { get => ((HyperTrainFlags >> 0) & 1) == 1;  set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 0)) | ((value ? 1 : 0) << 0)); }
    public bool HT_ATK { get => ((HyperTrainFlags >> 1) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 1)) | ((value ? 1 : 0) << 1)); }
    public bool HT_DEF { get => ((HyperTrainFlags >> 2) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 2)) | ((value ? 1 : 0) << 2)); }
    public bool HT_SPA { get => ((HyperTrainFlags >> 3) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 3)) | ((value ? 1 : 0) << 3)); }
    public bool HT_SPD { get => ((HyperTrainFlags >> 4) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 4)) | ((value ? 1 : 0) << 4)); }
    public bool HT_SPE { get => ((HyperTrainFlags >> 5) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 5)) | ((value ? 1 : 0) << 5)); }
    public override int Version { get => Data[0xDF]; set => Data[0xDF] = (byte)value; }
    // 0xE0 Unused
    // 0xE1 Unused
    // 0xE2 Unused
    public override int Language { get => Data[0xE3]; set => Data[0xE3] = (byte)value; }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(0xE4)); set => WriteSingleLittleEndian(Data.AsSpan(0xE4), value); }
    #endregion
    #region Battle Stats
    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0xE8)); set => WriteInt32LittleEndian(Data.AsSpan(0xE8), value); }
    public override int Stat_Level { get => Data[0xEC]; set => Data[0xEC] = (byte)value; }
    public byte DirtType { get => Data[0xED]; set => Data[0xED] = value; }
    public byte DirtLocation { get => Data[0xEE]; set => Data[0xEE] = value; }
    // 0xEF unused
    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0xF0)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF0), (ushort)value); }
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data.AsSpan(0xF2)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF2), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16LittleEndian(Data.AsSpan(0xF4)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF4), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16LittleEndian(Data.AsSpan(0xF6)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF6), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16LittleEndian(Data.AsSpan(0xF8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF8), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16LittleEndian(Data.AsSpan(0xFA)); set => WriteUInt16LittleEndian(Data.AsSpan(0xFA), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16LittleEndian(Data.AsSpan(0xFC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xFC), (ushort)value); }
    public int Stat_CP { get => ReadUInt16LittleEndian(Data.AsSpan(0xFE)); set => WriteUInt16LittleEndian(Data.AsSpan(0xFE), (ushort)value); }
    public bool Stat_Mega { get => Data[0x100] != 0; set => Data[0x100] = value ? (byte)1 : (byte)0; }
    public int Stat_MegaForm { get => Data[0x101]; set => Data[0x101] = (byte)value; }
    // 102/103 unused
    #endregion

    public override int MarkingCount => 6;

    public override int GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (MarkValue >> (index * 2)) & 3;
    }

    public override void SetMarking(int index, int value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        var shift = index * 2;
        MarkValue = (MarkValue & ~(0b11 << shift)) | ((value & 3) << shift);
    }

    protected override bool TradeOT(ITrainerInfo tr)
    {
        // Check to see if the OT matches the SAV's OT info.
        if (!(tr.TID == TID && tr.SID == SID && tr.Gender == OT_Gender && tr.OT == OT_Name))
            return false;

        CurrentHandler = 0;
        return true;
    }

    protected override void TradeHT(ITrainerInfo tr)
    {
        if (HT_Name != tr.OT)
        {
            HT_Friendship = CurrentFriendship; // copy friendship instead of resetting (don't alter CP)
            HT_Name = tr.OT;
        }
        CurrentHandler = 1;
        HT_Gender = tr.Gender;
    }

    public void FixMemories()
    {
        if (IsUntraded)
            HT_Friendship = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0;
    }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_7b;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_7b;
    public override int MaxAbilityID => Legal.MaxAbilityID_7_USUM;
    public override int MaxItemID => Legal.MaxItemID_7_USUM;
    public override int MaxBallID => Legal.MaxBallID_7;
    public override int MaxGameID => Legal.MaxGameID_7b;

    public override void LoadStats(IBaseStat p, Span<ushort> stats)
    {
        int level = CurrentLevel;
        int nature = Nature;
        int friend = CurrentFriendship; // stats +10% depending on friendship!
        int scalar = (int)(((friend / 255.0f / 10.0f) + 1.0f) * 100.0f);
        stats[0] = (ushort)(AV_HP  + GetStat(p.HP,  HT_HP  ? 31 : IV_HP,  level) + 10 + level);
        stats[1] = (ushort)(AV_ATK + (scalar * GetStat(p.ATK, HT_ATK ? 31 : IV_ATK, level, nature, 0) / 100));
        stats[2] = (ushort)(AV_DEF + (scalar * GetStat(p.DEF, HT_DEF ? 31 : IV_DEF, level, nature, 1) / 100));
        stats[3] = (ushort)(AV_SPE + (scalar * GetStat(p.SPE, HT_SPE ? 31 : IV_SPE, level, nature, 4) / 100));
        stats[4] = (ushort)(AV_SPA + (scalar * GetStat(p.SPA, HT_SPA ? 31 : IV_SPA, level, nature, 2) / 100));
        stats[5] = (ushort)(AV_SPD + (scalar * GetStat(p.SPD, HT_SPD ? 31 : IV_SPD, level, nature, 3) / 100));
    }

    /// <summary>
    /// Gets the initial stat value based on the base stat value, IV, and current level.
    /// </summary>
    /// <param name="baseStat"><see cref="PersonalInfo"/> stat.</param>
    /// <param name="iv">Current IV, already accounted for Hyper Training</param>
    /// <param name="level">Current Level</param>
    /// <returns>Initial Stat</returns>
    private static int GetStat(int baseStat, int iv, int level) => (iv + (2 * baseStat)) * level / 100;

    /// <summary>
    /// Gets the initial stat value with nature amplification applied. Used for all stats except HP.
    /// </summary>
    /// <param name="baseStat"><see cref="PersonalInfo"/> stat.</param>
    /// <param name="iv">Current IV, already accounted for Hyper Training</param>
    /// <param name="level">Current Level</param>
    /// <param name="nature"><see cref="PKM.Nature"/></param>
    /// <param name="statIndex">Stat amp index in the nature amp table</param>
    /// <returns>Initial Stat with nature amplification applied.</returns>
    private static int GetStat(int baseStat, int iv, int level, int nature, int statIndex)
    {
        int initial = GetStat(baseStat, iv, level) + 5;
        return AmplifyStat(nature, statIndex, initial);
    }

    private static int AmplifyStat(int nature, int index, int initial) => GetNatureAmp(nature, index) switch
    {
        1 => 110 * initial / 100, // 110%
        -1 => 90 * initial / 100, // 90%
        _ => initial,
    };

    private static sbyte GetNatureAmp(int nature, int index)
    {
        if ((uint)nature >= 25)
            return -1;
        return NatureAmpTable[(5 * nature) + index];
    }

    private static readonly sbyte[] NatureAmpTable =
    {
        0, 0, 0, 0, 0, // Hardy
        1,-1, 0, 0, 0, // Lonely
        1, 0, 0, 0,-1, // Brave
        1, 0,-1, 0, 0, // Adamant
        1, 0, 0,-1, 0, // Naughty
        -1, 1, 0, 0, 0, // Bold
        0, 0, 0, 0, 0, // Docile
        0, 1, 0, 0,-1, // Relaxed
        0, 1,-1, 0, 0, // Impish
        0, 1, 0,-1, 0, // Lax
        -1, 0, 0, 0, 1, // Timid
        0,-1, 0, 0, 1, // Hasty
        0, 0, 0, 0, 0, // Serious
        0, 0,-1, 0, 1, // Jolly
        0, 0, 0,-1, 1, // Naive
        -1, 0, 1, 0, 0, // Modest
        0,-1, 1, 0, 0, // Mild
        0, 0, 1, 0,-1, // Quiet
        0, 0, 0, 0, 0, // Bashful
        0, 0, 1,-1, 0, // Rash
        -1, 0, 0, 1, 0, // Calm
        0,-1, 0, 1, 0, // Gentle
        0, 0, 0, 1,-1, // Sassy
        0, 0,-1, 1, 0, // Careful
        0, 0, 0, 0, 0, // Quirky
    };

    public int CalcCP => Math.Min(10000, AwakeCP + BaseCP);

    public int BaseCP
    {
        get
        {
            var p = PersonalInfo;
            int level = CurrentLevel;
            int nature = Nature;
            int scalar = CPScalar;

            // Calculate stats for all, then sum together.
            // HP is not overriden to 1 like a regular stat calc for Shedinja.
            var statSum =
                (ushort)GetStat(p.HP, HT_HP ? 31 : IV_HP, level) + 10 + level
                + (ushort)(GetStat(p.ATK, HT_ATK ? 31 : IV_ATK, level, nature, 0) * scalar / 100)
                + (ushort)(GetStat(p.DEF, HT_DEF ? 31 : IV_DEF, level, nature, 1) * scalar / 100)
                + (ushort)(GetStat(p.SPE, HT_SPE ? 31 : IV_SPE, level, nature, 4) * scalar / 100)
                + (ushort)(GetStat(p.SPA, HT_SPA ? 31 : IV_SPA, level, nature, 2) * scalar / 100)
                + (ushort)(GetStat(p.SPD, HT_SPD ? 31 : IV_SPD, level, nature, 3) * scalar / 100);

            float result = statSum * 6f;
            result *= level;
            result /= 100f;
            return (int)result;
        }
    }

    public int CPScalar
    {
        get
        {
            int friend = CurrentFriendship; // stats +10% depending on friendship!
            float scalar = friend / 255f;
            scalar /= 10f;
            scalar++;
            scalar *= 100f;
            return (int)scalar;
        }
    }

    public int AwakeCP
    {
        get
        {
            var sum = this.AwakeningSum();
            if (sum == 0)
                return 0;
            var lvl = CurrentLevel;
            float scalar = lvl * 4f;
            scalar /= 100f;
            scalar += 2f;
            float result = sum * scalar;
            return (int)result;
        }
    }

    public void ResetCP() => Stat_CP = CalcCP;

    public void ResetCalculatedValues()
    {
        ResetCP();
        ResetHeight();
        ResetWeight();
    }

    public float HeightRatio => GetHeightRatio(HeightScalar);
    public float WeightRatio => GetWeightRatio(WeightScalar);

    public float CalcHeightAbsolute => GetHeightAbsolute(PersonalInfo, HeightScalar);
    public float CalcWeightAbsolute => GetWeightAbsolute(PersonalInfo, HeightScalar, WeightScalar);

    public void ResetHeight() => HeightAbsolute = CalcHeightAbsolute;
    public void ResetWeight() => WeightAbsolute = CalcWeightAbsolute;

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private static float GetHeightRatio(int heightScalar)
    {
        // + 40%, -20
        float result = heightScalar / 255f; // 0x437F0000
        result *= 0.79999995f; // 0x3F4CCCCC
        result += 0.6f; // 0x3F19999A
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private static float GetWeightRatio(int weightScalar)
    {
        // +/- 20%
        float result = weightScalar / 255f; // 0x437F0000
        result *= 0.40000004f; // 0x3ECCCCCE
        result += 0.8f; // 0x3F4CCCCD
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static float GetHeightAbsolute(IPersonalMisc p, int heightScalar)
    {
        float HeightRatio = GetHeightRatio(heightScalar);
        return HeightRatio * p.Height;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static float GetWeightAbsolute(IPersonalMisc p, int heightScalar, int weightScalar)
    {
        float HeightRatio = GetHeightRatio(heightScalar);
        float WeightRatio = GetWeightRatio(weightScalar);

        float weight = WeightRatio * p.Weight;
        return HeightRatio * weight;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static byte GetHeightScalar(float height, int avgHeight)
    {
        // height is already *100
        float biasH = avgHeight * -0.6f;
        float biasL = avgHeight * 0.79999995f;
        float numerator = biasH + height;
        float result = numerator / biasL;
        result *= 255f;
        int value = (int)result;
        int unsigned = value & ~(value >> 31);
        return (byte)Math.Min(255, unsigned);
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static byte GetWeightScalar(float height, float weight, int avgHeight, int avgWeight)
    {
        // height is already *100
        // weight is already *10
        float heightRatio = height / avgHeight;
        float weightComponent = heightRatio * weight;
        float top = avgWeight * -0.8f;
        top += weightComponent;
        float bot = avgWeight * 0.40000004f;
        float result = top / bot;
        result *= 255f;
        int value = (int)result;
        int unsigned = value & ~(value >> 31);
        return (byte)Math.Min(255, unsigned);
    }

    public static int GetRandomIndex(int bits, int characterIndex, int nature)
    {
        if (bits is 6 or 7)
            return GetRandomIndex(characterIndex);
        if (bits is 0)
            return 0;
        Span<sbyte> amps = NatureAmpTable.AsSpan(5 * nature, 5);
        if (amps[bits - 1] != -1) // not a negative stat
            return bits;

        // remap a negative stat to positive
        return 1 + amps.IndexOf((sbyte)1);
    }

    private static int GetRandomIndex(int characterIndex) => (characterIndex / 5) switch
    {
        0 => 0,
        1 => 1,
        2 => 2,
        3 => 5,
        4 => 3,
        5 => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(characterIndex)), // never happens, characteristic is always 0-29
    };

    public PK8 ConvertToPK8()
    {
        var pk8 = new PK8
        {
            EncryptionConstant = EncryptionConstant,
            Species = Species,
            TID = TID,
            SID = SID,
            EXP = EXP,
            PID = PID,
            Ability = Ability,
            AbilityNumber = AbilityNumber,
            MarkValue = MarkValue & 0b1111_1111_1111,
            Language = Language,
            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,
            EV_SPE = EV_SPE,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            RelearnMove1 = RelearnMove1,
            RelearnMove2 = RelearnMove2,
            RelearnMove3 = RelearnMove3,
            RelearnMove4 = RelearnMove4,
            IV_HP = IV_HP,
            IV_ATK = IV_ATK,
            IV_DEF = IV_DEF,
            IV_SPA = IV_SPA,
            IV_SPD = IV_SPD,
            IV_SPE = IV_SPE,
            IsNicknamed = IsNicknamed,
            FatefulEncounter = FatefulEncounter,
            Gender = Gender,
            Form = Form,
            Nature = Nature,
            Nickname = Nickname,
            Version = Version,
            OT_Name = OT_Name,
            MetDate = MetDate,
            Met_Location = Met_Location,
            Ball = Ball,
            Met_Level = Met_Level,
            OT_Gender = OT_Gender,
            HyperTrainFlags = HyperTrainFlags,

            // Memories don't exist in LGPE, and no memories are set on transfer.

            OT_Friendship = OT_Friendship,

            // No Ribbons or Markings on transfer.

            StatNature = Nature,
            HeightScalar = HeightScalar,
            WeightScalar = WeightScalar,

            Favorite = Favorite,
        };

        // Fix PP and Stats
        pk8.Heal();

        // Fix Checksum
        pk8.RefreshChecksum();

        return pk8; // Done!
    }
}
