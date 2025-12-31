using System;
using static PKHeX.Core.RibbonIndex;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Mystery Gift Template File
/// </summary>
public sealed class WA9(Memory<byte> raw) : DataMysteryGift(raw), ILangNick, INature, IAlpha, IRibbonIndex, IMemoryOT,
    ILangNicknamedTemplate, IEncounterServerDate, IRelearn, IMetLevel, IEncounter9a,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetCommon7,
    IRibbonSetCommon8, IRibbonSetMark8, IRibbonSetCommon9, IRibbonSetMark9, IEncounterMarkExtra
{
    public WA9() : this(new byte[Size]) { }
    public override WC9 Clone() => new(Data.ToArray());

    public const int Size = 0x2C8;

    public override byte Generation => 9;
    public override EntityContext Context => EntityContext.Gen9a;
    public override bool FatefulEncounter => true;

    public enum GiftType : byte
    {
        None = 0,
        Pokemon = 1,
        Item = 2,
        BP = 3,
        Clothing = 4,
    }

    public bool CanBeReceivedByVersion(PKM pk) => pk.Version is GameVersion.ZA;

    // General Card Properties
    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data[0x8..]);
        set => WriteUInt16LittleEndian(Data[0x8..], (ushort)value);
    }

    public byte RestrictVersion { get => Data[0xE]; set => Data[0xE] = value; } // 0x01 = ZA only (only one game in this Context, so always ZA).
    public byte CardFlags { get => Data[0x10]; set => Data[0x10] = value; }
    public GiftType CardType { get => (GiftType)Data[0x11]; set => Data[0x11] = (byte)value; }
    public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
    public override bool GiftUsed { get => false; set { }  }

    public override int CardTitleIndex
    {
        get => Data[0x15];
        set => Data[0x15] = (byte) value;
    }

    public override string CardTitle
    {
        get => this.GetTitleFromIndex();
        set => throw new Exception();
    }

    // Item Properties
    public override bool IsItem { get => CardType == GiftType.Item; set { if (value) CardType = GiftType.Item; } }

    public override int ItemID
    {
        get => GetItem(0);
        set => SetItem(0, (ushort)value);
    }

    public override int Quantity
    {
        get => GetQuantity(0);
        set => SetQuantity(0, (ushort)value);
    }

    public int GetItem(int index) => ReadUInt16LittleEndian(Data[(0x18 + (0x4 * index))..]);
    public void SetItem(int index, ushort item) => WriteUInt16LittleEndian(Data[(0x18 + (4 * index))..], item);
    public int GetQuantity(int index) => ReadUInt16LittleEndian(Data[(0x1A + (0x4 * index))..]);
    public void SetQuantity(int index, ushort quantity) => WriteUInt16LittleEndian(Data[(0x1A + (4 * index))..], quantity);

    // Pokémon Properties
    public override bool IsEntity { get => CardType == GiftType.Pokemon; set { if (value) CardType = GiftType.Pokemon; } }

    public override bool IsShiny => Shiny.IsShiny();

    public override Shiny Shiny => PIDType switch
    {
        ShinyType8.FixedValue => FixedShinyType(),
        ShinyType8.Random => Shiny.Random,
        ShinyType8.Never => Shiny.Never,
        ShinyType8.AlwaysStar => Shiny.AlwaysStar,
        ShinyType8.AlwaysSquare => Shiny.AlwaysSquare,
        _ => throw new ArgumentOutOfRangeException(),
    };

    private Shiny FixedShinyType() => GetShinyXor() switch
    {
        0 => Shiny.AlwaysSquare,
        <= 15 => Shiny.AlwaysStar,
        _ => Shiny.Never,
    };

    private uint GetShinyXor()
    {
        // Player owned anti-shiny fixed PID
        if (ID32 == 0)
            return uint.MaxValue;
        return ShinyUtil.GetShinyXor(PID, ID32);
    }

    // When applying the ID32, the game sets the DisplayTID7 directly, then sets PA9.DisplaySID7 as (wa9.DisplaySID7 - wa9.CardID)
    // Since we expose the 16bit (PA9) component values here, just adjust them accordingly with an inlined calc.
    public override uint ID32 { get => ReadUInt32LittleEndian(Data[0x18..]); set => WriteUInt32LittleEndian(Data[0x18..], value); }

    private bool IsOldIDFormat => true;

    public uint ID32Old
    {
        get => ReadUInt32LittleEndian(Data[0x18..]) - (1000000u * (uint)CardID);
        set => WriteUInt32LittleEndian(Data[0x18..], value + (1000000u * (uint)CardID));
    }

    public int OriginGame { get => ReadInt32LittleEndian(Data[0x1C..]); set => WriteInt32LittleEndian(Data[0x1C..], value); }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[0x20..]); set => WriteUInt32LittleEndian(Data[0x20..], value); }
    public uint PID { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], value); }

    public override ushort TID16
    {
        get => (ushort)(ID32 & 0xFFFF);
        set => ID32 = (uint)SID16 << 16 | value;
    }

    public override ushort SID16
    {
        get => (ushort)(ID32 >> 16);
        set => ID32 = (uint)value << 16 | TID16;
    }

    // Nicknames, OT Names 0x28 - 0x220
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x258..]); set => WriteUInt16LittleEndian(Data[0x258..], value); }
    public override ushort Location { get => ReadUInt16LittleEndian(Data[0x25A..]); set => WriteUInt16LittleEndian(Data[0x25A..], value); }
    public override byte Ball { get => (byte)ReadUInt16LittleEndian(Data[0x25C..]); set => WriteUInt16LittleEndian(Data[0x25C..], value); }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data[0x25E..]); set => WriteUInt16LittleEndian(Data[0x25E..], (ushort)value); }
    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x260..]); set => WriteUInt16LittleEndian(Data[0x260..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x262..]); set => WriteUInt16LittleEndian(Data[0x262..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x264..]); set => WriteUInt16LittleEndian(Data[0x264..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x266..]); set => WriteUInt16LittleEndian(Data[0x266..], value); }
    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data[0x268..]); set => WriteUInt16LittleEndian(Data[0x268..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data[0x26A..]); set => WriteUInt16LittleEndian(Data[0x26A..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data[0x26C..]); set => WriteUInt16LittleEndian(Data[0x26C..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data[0x26E..]); set => WriteUInt16LittleEndian(Data[0x26E..], value); }

    public override ushort Species { get => SpeciesConverter.GetNational9(ReadUInt16LittleEndian(Data[0x270..])); set => WriteUInt16LittleEndian(Data[0x270..], SpeciesConverter.GetInternal9(value)); }
    public override byte Form { get => Data[0x272]; set => Data[0x272] = value; }
    public override byte Gender { get => Data[0x273]; set => Data[0x273] = value; }
    public override byte Level { get => Data[0x274]; set => Data[0x274] = value; }
    public override bool IsEgg { get => Data[0x275] == 1; set => Data[0x275] = value ? (byte)1 : (byte)0; }
    public Nature Nature
    {
        get
        {
            var value = (Nature)Data[0x276];
            if (value >= Nature.Random)
                return Nature.Random;
            return value;
        }
        set
        {
            if (value == Nature.Random)
                value = (Nature)255;
            Data[0x276] = (byte)value;
        }
    }

    public int AbilityType { get => Data[0x277]; set => Data[0x277] = (byte)value; }

    public ShinyType8 PIDType { get => (ShinyType8)Data[0x278]; set => Data[0x278] = (byte)value; }

    public byte MetLevel { get => Data[0x279]; set => Data[0x279] = value; }

    // Ribbons 0x24C-0x26C
    private const int RibbonBytesOffset = 0x27A;
    private const int RibbonBytesCount = 0x20;
    private const byte RibbonByteNone = 0xFF; // signed -1

    private Span<byte> RibbonSpan => Data.Slice(RibbonBytesOffset, RibbonBytesCount);

    public bool HasMarkEncounter8
    {
        get
        {
            foreach (var value in RibbonSpan)
            {
                if (((RibbonIndex)value).IsEncounterMark8)
                    return true;
            }
            return false;
        }
    }

    public bool HasMarkEncounter9
    {
        get
        {
            foreach (var value in RibbonSpan)
            {
                if (((RibbonIndex)value).IsEncounterMark9)
                    return true;
            }
            return false;
        }
    }

    public byte GetRibbonAtIndex(int byteIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)byteIndex, RibbonBytesCount);
        return RibbonSpan[byteIndex];
    }

    public void SetRibbonAtIndex(int byteIndex, byte ribbonIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)byteIndex, RibbonBytesCount);
        RibbonSpan[byteIndex] = ribbonIndex;
    }

    public int IV_HP  { get => Data[0x29A]; set => Data[0x29A] = (byte)value; }
    public int IV_ATK { get => Data[0x29B]; set => Data[0x29B] = (byte)value; }
    public int IV_DEF { get => Data[0x29C]; set => Data[0x29C] = (byte)value; }
    public int IV_SPE { get => Data[0x29D]; set => Data[0x29D] = (byte)value; }
    public int IV_SPA { get => Data[0x29E]; set => Data[0x29E] = (byte)value; }
    public int IV_SPD { get => Data[0x29F]; set => Data[0x29F] = (byte)value; }

    public byte OTGender { get => Data[0x2A0]; set => Data[0x2A0] = value; }

    public int EV_HP  { get => Data[0x2A1]; set => Data[0x2A1] = (byte)value; }
    public int EV_ATK { get => Data[0x2A2]; set => Data[0x2A2] = (byte)value; }
    public int EV_DEF { get => Data[0x2A3]; set => Data[0x2A3] = (byte)value; }
    public int EV_SPE { get => Data[0x2A4]; set => Data[0x2A4] = (byte)value; }
    public int EV_SPA { get => Data[0x2A5]; set => Data[0x2A5] = (byte)value; }
    public int EV_SPD { get => Data[0x2A6]; set => Data[0x2A6] = (byte)value; }

    // memories: unreferenced; retaining just in case
    public byte OriginalTrainerMemoryIntensity { get => Data[0x2A7]; set => Data[0x2A7] = value; }
    public byte OriginalTrainerMemory { get => Data[0x2A8]; set => Data[0x2A8] = value; }
    public byte OriginalTrainerMemoryFeeling { get => Data[0x2A9]; set => Data[0x2A9] = value; }
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data[0x2AA..]); set => WriteUInt16LittleEndian(Data[0x2AA..], value); }

    public ushort Scale { get => ReadUInt16LittleEndian(Data[0x2AC..]); set => WriteUInt16LittleEndian(Data[0x2AC..], value); }
    public bool IsAlpha { get => Data[0x2AE] != 0; set => Data[0x2AE] = value ? (byte)1 : (byte)0; }

    public ushort Checksum => ReadUInt16LittleEndian(Data[0x2C0..]);

    // Meta Accessible Properties
    public int[] IVs
    {
        get => [IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD];
        set
        {
            if (value.Length != 6)
                return;
            IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
            IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
        }
    }

    public override void GetIVs(Span<int> value)
    {
        if (value.Length != 6)
            return;
        value[0] = IV_HP;
        value[1] = IV_ATK;
        value[2] = IV_DEF;
        value[3] = IV_SPE;
        value[4] = IV_SPA;
        value[5] = IV_SPD;
    }

    public int[] EVs
    {
        get => [EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD];
        set
        {
            if (value.Length != 6)
                return;
            EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
            EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
        }
    }

    public bool GetIsNicknamed(int language) => ReadUInt16LittleEndian(Data[GetNicknameOffset(language)..]) != 0;

    public bool CanBeAnyLanguage()
    {
        for (int i = 0; i < 9; i++)
        {
            var ofs = GetLanguageOffset(i);
            var lang = ReadInt16LittleEndian(Data[ofs..]);
            if (lang != 0)
                return false;
        }
        return true;
    }

    public bool CanHaveLanguage(int language)
    {
        if (language is < (int)LanguageID.Japanese or > (int)LanguageID.ChineseT)
            return false;

        if (CanBeAnyLanguage())
            return true;

        for (int i = 0; i < 9; i++)
        {
            var ofs = GetLanguageOffset(i);
            var lang = ReadInt16LittleEndian(Data[ofs..]);
            if (lang == language)
                return true;
        }
        return GetLanguage(language) == 0;
    }

    public int GetLanguage(int redeemLanguage) => Data[GetLanguageOffset(GetLanguageIndex(redeemLanguage))];
    private static int GetLanguageOffset(int index) => 0x140 + (index * 0x1C) + 0x1A;

    public bool GetHasOT(int language) => ReadUInt16LittleEndian(Data[GetOTOffset(language)..]) != 0;

    private static int GetLanguageIndex(int language)
    {
        var lang = (LanguageID) language;
        if (lang is < LanguageID.Japanese or LanguageID.UNUSED_6 or > LanguageID.ChineseT)
            return (int) LanguageID.English; // fallback
        return lang < LanguageID.UNUSED_6 ? language - 1 : language - 2;
    }

    public override Moveset Moves
    {
        get => new(Move1, Move2, Move3, Move4);
        set
        {
            Move1 = value.Move1;
            Move2 = value.Move2;
            Move3 = value.Move3;
            Move4 = value.Move4;
        }
    }

    public Moveset Relearn
    {
        get => new(RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4);
        set
        {
            RelearnMove1 = value.Move1;
            RelearnMove2 = value.Move2;
            RelearnMove3 = value.Move3;
            RelearnMove4 = value.Move4;
        }
    }

    public override string OriginalTrainerName
    {
        get => GetOT(Language);
        set
        {
            for (int i = 1; i <= (int)LanguageID.ChineseT; i++)
                SetOT(i, value);
        }
    }

    public string Nickname => GetIsNicknamed(Language) ? GetNickname(Language) : string.Empty;
    public bool IsNicknamed => false;
    public int Language => 2;

    private Span<byte> GetNicknameSpan(int language) => Data.Slice(GetNicknameOffset(language), 0x1A);
    public string GetNickname(int language) => StringConverter8.GetString(GetNicknameSpan(language));
    public void SetNickname(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetNicknameSpan(language), value, 12, StringConverterOption.ClearZero);

    private Span<byte> GetOTSpan(int language) => Data.Slice(GetOTOffset(language), 0x1A);
    public string GetOT(int language) => StringConverter8.GetString(GetOTSpan(language));
    public void SetOT(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetOTSpan(language), value, 12, StringConverterOption.ClearZero);

    private static int GetNicknameOffset(int language)
    {
        int index = GetLanguageIndex(language);
        return 0x28 + (index * 0x1C);
    }

    private static int GetOTOffset(int language)
    {
        int index = GetLanguageIndex(language);
        return 0x140 + (index * 0x1C);
    }

    public bool CanHandleOT(int language) => !GetHasOT(language);

    public override GameVersion Version => OriginGame != 0 ? (GameVersion)OriginGame : GameVersion.SV;

    public override PA9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        var rnd = Util.Rand;
        byte currentLevel = Level > 0 ? Level : (byte)(1 + rnd.Next(100));
        var metLevel = MetLevel > 0 ? MetLevel : currentLevel;
        var pi = PersonalTable.ZA.GetFormEntry(Species, Form);
        var language = tr.Language;
        bool hasOT = GetHasOT(language);
        var version = OriginGame != 0 ? (GameVersion)OriginGame : this.GetCompatibleVersion(tr.Version);

        var pk = new PA9
        {
            EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : rnd.Rand32(),
            ID32 = OTGender >= 2 ? tr.ID32 : IsOldIDFormat ? ID32Old : ID32,
            Species = Species,
            Form = Form,
            CurrentLevel = currentLevel,
            Ball = Ball != 0 ? Ball : (byte)4, // Default is Poké Ball
            MetLevel = metLevel,
            HeldItem = HeldItem,

            EXP = Experience.GetEXP(currentLevel, pi.EXPGrowth),

            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            RelearnMove1 = RelearnMove1,
            RelearnMove2 = RelearnMove2,
            RelearnMove3 = RelearnMove3,
            RelearnMove4 = RelearnMove4,

            Version = version,

            OriginalTrainerName = hasOT ? GetOT(language) : tr.OT,
            OriginalTrainerGender = OTGender < 2 ? OTGender : tr.Gender,
            HandlingTrainerName = hasOT ? tr.OT : string.Empty,
            HandlingTrainerGender = hasOT ? tr.Gender : default,
            HandlingTrainerLanguage = (byte)(hasOT ? language : 0),
            CurrentHandler = hasOT ? (byte)1 : (byte)0,
            OriginalTrainerFriendship = pi.BaseFriendship,

            OriginalTrainerMemoryIntensity = OriginalTrainerMemoryIntensity,
            OriginalTrainerMemory = OriginalTrainerMemory,
            OriginalTrainerMemoryVariable = OriginalTrainerMemoryVariable,
            OriginalTrainerMemoryFeeling = OriginalTrainerMemoryFeeling,
            FatefulEncounter = true,

            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPE = EV_SPE,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,

            MetLocation = Location,
            EggLocation = EggLocation,
            IsAlpha = IsAlpha,
            ObedienceLevel = currentLevel,
            MetDate = GetSuggestedDate(),
        };

        var nickname_language = GetLanguage(language);
        pk.Language = nickname_language != 0 ? nickname_language : tr.Language;
        pk.IsNicknamed = GetIsNicknamed(language);
        pk.Nickname = pk.IsNicknamed ? GetNickname(language) : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

        // No ribbons set.
        // for (var i = 0; i < RibbonBytesCount; i++)
        // {
        //     var ribbon = GetRibbonAtIndex(i);
        //     if (ribbon == RibbonByteNone)
        //         continue;
        //     pk.SetRibbon(ribbon);
        //     pk.AffixedRibbon = (sbyte)ribbon;
        // }

        SetPINGA(pk, criteria, pi);
        SetMoves(currentLevel, pk, pi);
        pk.HealPP();

        if (IsEgg)
            SetEggMetData(pk);
        pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    private void SetMoves(byte currentLevel, PA9 pk, PersonalInfo9ZA pi)
    {
        var (learn, plus) = LearnSource9ZA.GetLearnsetAndPlus(Species, Form);
        pk.SetPlusFlagsEncounter(pi, plus, currentLevel);
        if (Move1 != 0) // Just in case they forget to set moves on an event.
            return;
        Span<ushort> moves = stackalloc ushort[4];
        learn.SetEncounterMoves(currentLevel, moves);
        pk.SetMoves(moves);
    }

    private DateOnly GetSuggestedDate()
    {
        if (!IsDateRestricted)
            return EncounterDate.GetDateSwitch();
        if (this.GetDistributionWindow(out var window))
            return window.GetGenerateDate();
        return EncounterDate.GetDateSwitch();
    }

    private void SetEggMetData(PA9 pk)
    {
        pk.IsEgg = true;
        pk.EggMetDate = EncounterDate.GetDateSwitch();
        pk.Nickname = SpeciesName.GetEggName(pk.Language, Generation);
        pk.IsNicknamed = true;
    }

    private void SetPINGA(PA9 pk, EncounterCriteria criteria, PersonalInfo9ZA pi)
    {
        var param = GetParams(pi);
        ulong init = Util.Rand.Rand64();
        var success = this.TryApply64(pk, init, param, criteria);
        if (!success && !this.TryApply64(pk, init, param, criteria.WithoutIVs()))
            this.TryApply64(pk, init, param, EncounterCriteria.Unrestricted);

        if (PIDType is not (ShinyType8.Never or ShinyType8.Random))
            pk.PID = GetPID(pk, PIDType);
    }

    public override AbilityPermission Ability => AbilityType switch
    {
        0 => AbilityPermission.OnlyFirst,
        1 => AbilityPermission.OnlySecond,
        2 => AbilityPermission.OnlyHidden,
        3 => AbilityPermission.Any12,
        _ => AbilityPermission.Any12H,
    };

    private uint GetPID(ITrainerID32 tr, ShinyType8 type) => type switch
    {
        ShinyType8.Never        => GetAntishiny(tr), // Random, Never Shiny
        ShinyType8.Random       => Util.Rand32(), // Random, Any
        ShinyType8.AlwaysStar   => (1u ^ (PID & 0xFFFF) ^ tr.TID16 ^ tr.SID16) << 16 | (PID & 0xFFFF), // Fixed, Force Star
        ShinyType8.AlwaysSquare => (0u ^ (PID & 0xFFFF) ^ tr.TID16 ^ tr.SID16) << 16 | (PID & 0xFFFF), // Fixed, Force Square
        ShinyType8.FixedValue   => GetFixedPID(tr),
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private uint GetFixedPID(ITrainerID32 tr)
    {
        var pid = PID;
        if (pid != 0 && ID32 != 0)
            return pid;

        if (!tr.IsShiny(pid, 9))
            return pid;
        return pid;
    }

    private static uint GetAntishiny(ITrainerID32 tr)
    {
        var pid = Util.Rand32();
        if (tr.IsShiny(pid, 9))
            return pid ^ 0x1000_0000;
        return pid;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsEgg)
        {
            if (OTGender < 2)
            {
                var expect = IsOldIDFormat ? ID32Old : ID32;
                if (expect != pk.ID32) return false;
                if (OTGender != pk.OriginalTrainerGender) return false;
            }

            var language = pk.Language;
            if (!CanBeAnyLanguage() && !CanHaveLanguage(language))
                return false;

            if (GetHasOT(language) && !IsMatchTrainerName(GetOTSpan(language), pk))
                return false;

            if (OriginGame != 0 && (GameVersion)OriginGame != pk.Version) return false;
            if (EncryptionConstant != 0)
            {
                if (EncryptionConstant != pk.EncryptionConstant)
                    return false;
            }
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (pk is IAlphaReadOnly t && t.IsAlpha != IsAlpha)
            return false;

        var shinyType = Shiny;
        if (PIDType == ShinyType8.FixedValue)
            shinyType = FixedShinyType();
        if (IsEgg)
        {
            if (EggLocation != pk.EggLocation) // traded
            {
                if (pk.EggLocation != Locations.LinkTrade6)
                    return false;
                if (PIDType == ShinyType8.Random && pk is { IsShiny: true, ShinyXor: > 1 })
                    return false; // shiny traded egg will always have xor0/1.
            }
            if (!shinyType.IsValid(pk))
            {
                return false; // can't be traded away for unshiny
            }

            if (pk is { IsEgg: true, Context: not EntityContext.Gen9a })
                return false;
        }
        else
        {
            if (!shinyType.IsValid(pk)) return false;
            if (!IsMatchEggLocation(pk)) return false;
            if (!IsMatchLocation(pk)) return false;
        }

        if (MetLevel != 0 && MetLevel != pk.MetLevel) return false;
        if ((Ball == 0 ? 4 : Ball) != pk.Ball) return false;
        if (OTGender < 2 && OTGender != pk.OriginalTrainerGender) return false;
        if (Nature != Nature.Random && pk.Nature != Nature) return false;
        if (Gender != 3 && Gender != pk.Gender) return false;

        if (pk is IScaledSize s)
        {
            if (Scale != 256)
            {
                var current = pk is IScaledSize3 s3 ? s3.Scale : s.HeightScalar;
                if (Scale != current)
                    return false;
            }
        }
        if (pk is IAlphaReadOnly a && a.IsAlpha != IsAlpha)
            return true;

        // PID Types 0 and 1 do not use the fixed PID value.
        // Values 2,3 are specific shiny states, and 4 is fixed value.
        // 2,3,4 can change if it is a traded egg to ensure the same shiny state.
        var type = PIDType;
        if (type is ShinyType8.Never or ShinyType8.Random)
            return true;
        return pk.PID == GetPID(pk, type);
    }

    private bool IsMatchTrainerName(ReadOnlySpan<byte> trainerTrash, PKM pk)
    {
        Span<char> trainerName = stackalloc char[12];
        int length = StringConverter8.LoadString(trainerTrash, trainerName);
        trainerName = trainerName[..length];

        Span<char> pkTrainerName = stackalloc char[pk.MaxStringLengthTrainer];
        int pkLength = pk.LoadString(pk.OriginalTrainerTrash, pkTrainerName);
        pkTrainerName = pkTrainerName[..pkLength];

        if (length == pkLength && trainerName.SequenceEqual(pkTrainerName))
            return true;

        var language = (LanguageID)pk.Language;
        var version = pk.Version;
        var other = ReplaceTrainerNameHOME.IsTriggerAndReplace(trainerName, pkTrainerName, language, version, Species, Form);
        return other is not EntityContext.None;
    }

    private bool IsMatchLocation(PKM pk) => IsMatchLocationExact(pk);
    private bool IsMatchLocationExact(PKM pk) => pk.MetLocation == Location;

    public bool IsDateRestricted => true;

    protected override bool IsMatchDeferred(PKM pk) => false;

    protected override bool IsMatchPartial(PKM pk) => TryGetSeed(pk, out _) != SeedCorrelationResult.Success;

    #region Lazy Ribbon Implementation

    private static bool HasRibbon(RibbonIndex _) => false; // HasRibbon(index); // ZA is hard-coded to never set ribbons, so we need to return false for validation/setting.
    public bool RibbonEarth { get => HasRibbon(Earth); set => this.SetRibbonIndex(Earth, value); }
    public bool RibbonNational { get => HasRibbon(National); set => this.SetRibbonIndex(National, value); }
    public bool RibbonCountry { get => HasRibbon(Country); set => this.SetRibbonIndex(Country, value); }
    public bool RibbonChampionBattle { get => HasRibbon(ChampionBattle); set => this.SetRibbonIndex(ChampionBattle, value); }
    public bool RibbonChampionRegional { get => HasRibbon(ChampionRegional); set => this.SetRibbonIndex(ChampionRegional, value); }
    public bool RibbonChampionNational { get => HasRibbon(ChampionNational); set => this.SetRibbonIndex(ChampionNational, value); }
    public bool RibbonClassic { get => HasRibbon(Classic); set => this.SetRibbonIndex(Classic, value); }
    public bool RibbonWishing { get => HasRibbon(Wishing); set => this.SetRibbonIndex(Wishing, value); }
    public bool RibbonPremier { get => HasRibbon(Premier); set => this.SetRibbonIndex(Premier, value); }
    public bool RibbonEvent { get => HasRibbon(Event); set => this.SetRibbonIndex(Event, value); }
    public bool RibbonBirthday { get => HasRibbon(Birthday); set => this.SetRibbonIndex(Birthday, value); }
    public bool RibbonSpecial { get => HasRibbon(Special); set => this.SetRibbonIndex(Special, value); }
    public bool RibbonWorld { get => HasRibbon(World); set => this.SetRibbonIndex(World, value); }
    public bool RibbonChampionWorld { get => HasRibbon(ChampionWorld); set => this.SetRibbonIndex(ChampionWorld, value); }
    public bool RibbonSouvenir { get => HasRibbon(Souvenir); set => this.SetRibbonIndex(Souvenir, value); }
    public bool RibbonChampionG3 { get => HasRibbon(ChampionG3); set => this.SetRibbonIndex(ChampionG3, value); }
    public bool RibbonArtist { get => HasRibbon(Artist); set => this.SetRibbonIndex(Artist, value); }
    public bool RibbonEffort { get => HasRibbon(Effort); set => this.SetRibbonIndex(Effort, value); }
    public bool RibbonChampionSinnoh { get => HasRibbon(ChampionSinnoh); set => this.SetRibbonIndex(ChampionSinnoh, value); }
    public bool RibbonAlert { get => HasRibbon(Alert); set => this.SetRibbonIndex(Alert, value); }
    public bool RibbonShock { get => HasRibbon(Shock); set => this.SetRibbonIndex(Shock, value); }
    public bool RibbonDowncast { get => HasRibbon(Downcast); set => this.SetRibbonIndex(Downcast, value); }
    public bool RibbonCareless { get => HasRibbon(Careless); set => this.SetRibbonIndex(Careless, value); }
    public bool RibbonRelax { get => HasRibbon(Relax); set => this.SetRibbonIndex(Relax, value); }
    public bool RibbonSnooze { get => HasRibbon(Snooze); set => this.SetRibbonIndex(Snooze, value); }
    public bool RibbonSmile { get => HasRibbon(Smile); set => this.SetRibbonIndex(Smile, value); }
    public bool RibbonGorgeous { get => HasRibbon(Gorgeous); set => this.SetRibbonIndex(Gorgeous, value); }
    public bool RibbonRoyal { get => HasRibbon(Royal); set => this.SetRibbonIndex(Royal, value); }
    public bool RibbonGorgeousRoyal { get => HasRibbon(GorgeousRoyal); set => this.SetRibbonIndex(GorgeousRoyal, value); }
    public bool RibbonFootprint { get => HasRibbon(Footprint); set => this.SetRibbonIndex(Footprint, value); }
    public bool RibbonRecord { get => HasRibbon(Record); set => this.SetRibbonIndex(Record, value); }
    public bool RibbonLegend { get => HasRibbon(Legend); set => this.SetRibbonIndex(Legend, value); }
    public bool RibbonChampionKalos { get => HasRibbon(ChampionKalos); set => this.SetRibbonIndex(ChampionKalos, value); }
    public bool RibbonChampionG6Hoenn { get => HasRibbon(ChampionG6Hoenn); set => this.SetRibbonIndex(ChampionG6Hoenn, value); }
    public bool RibbonBestFriends { get => HasRibbon(BestFriends); set => this.SetRibbonIndex(BestFriends, value); }
    public bool RibbonTraining { get => HasRibbon(Training); set => this.SetRibbonIndex(Training, value); }
    public bool RibbonBattlerSkillful { get => HasRibbon(BattlerSkillful); set => this.SetRibbonIndex(BattlerSkillful, value); }
    public bool RibbonBattlerExpert { get => HasRibbon(BattlerExpert); set => this.SetRibbonIndex(BattlerExpert, value); }
    public bool RibbonContestStar { get => HasRibbon(ContestStar); set => this.SetRibbonIndex(ContestStar, value); }
    public bool RibbonMasterCoolness { get => HasRibbon(MasterCoolness); set => this.SetRibbonIndex(MasterCoolness, value); }
    public bool RibbonMasterBeauty { get => HasRibbon(MasterBeauty); set => this.SetRibbonIndex(MasterBeauty, value); }
    public bool RibbonMasterCuteness { get => HasRibbon(MasterCuteness); set => this.SetRibbonIndex(MasterCuteness, value); }
    public bool RibbonMasterCleverness { get => HasRibbon(MasterCleverness); set => this.SetRibbonIndex(MasterCleverness, value); }
    public bool RibbonMasterToughness { get => HasRibbon(MasterToughness); set => this.SetRibbonIndex(MasterToughness, value); }

    public bool RibbonChampionAlola { get => HasRibbon(ChampionAlola); set => this.SetRibbonIndex(ChampionAlola, value); }
    public bool RibbonBattleRoyale { get => HasRibbon(BattleRoyale); set => this.SetRibbonIndex(BattleRoyale, value); }
    public bool RibbonBattleTreeGreat { get => HasRibbon(BattleTreeGreat); set => this.SetRibbonIndex(BattleTreeGreat, value); }
    public bool RibbonBattleTreeMaster { get => HasRibbon(BattleTreeMaster); set => this.SetRibbonIndex(BattleTreeMaster, value); }
    public bool RibbonChampionGalar { get => HasRibbon(ChampionGalar); set => this.SetRibbonIndex(ChampionGalar, value); }
    public bool RibbonTowerMaster { get => HasRibbon(TowerMaster); set => this.SetRibbonIndex(TowerMaster, value); }
    public bool RibbonMasterRank { get => HasRibbon(MasterRank); set => this.SetRibbonIndex(MasterRank, value); }
    public bool RibbonMarkLunchtime { get => HasRibbon(MarkLunchtime); set => this.SetRibbonIndex(MarkLunchtime, value); }
    public bool RibbonMarkSleepyTime { get => HasRibbon(MarkSleepyTime); set => this.SetRibbonIndex(MarkSleepyTime, value); }
    public bool RibbonMarkDusk { get => HasRibbon(MarkDusk); set => this.SetRibbonIndex(MarkDusk, value); }
    public bool RibbonMarkDawn { get => HasRibbon(MarkDawn); set => this.SetRibbonIndex(MarkDawn, value); }
    public bool RibbonMarkCloudy { get => HasRibbon(MarkCloudy); set => this.SetRibbonIndex(MarkCloudy, value); }
    public bool RibbonMarkRainy { get => HasRibbon(MarkRainy); set => this.SetRibbonIndex(MarkRainy, value); }
    public bool RibbonMarkStormy { get => HasRibbon(MarkStormy); set => this.SetRibbonIndex(MarkStormy, value); }
    public bool RibbonMarkSnowy { get => HasRibbon(MarkSnowy); set => this.SetRibbonIndex(MarkSnowy, value); }
    public bool RibbonMarkBlizzard { get => HasRibbon(MarkBlizzard); set => this.SetRibbonIndex(MarkBlizzard, value); }
    public bool RibbonMarkDry { get => HasRibbon(MarkDry); set => this.SetRibbonIndex(MarkDry, value); }
    public bool RibbonMarkSandstorm { get => HasRibbon(MarkSandstorm); set => this.SetRibbonIndex(MarkSandstorm, value); }
    public bool RibbonMarkMisty { get => HasRibbon(MarkMisty); set => this.SetRibbonIndex(MarkMisty, value); }
    public bool RibbonMarkDestiny { get => HasRibbon(MarkDestiny); set => this.SetRibbonIndex(MarkDestiny, value); }
    public bool RibbonMarkFishing { get => HasRibbon(MarkFishing); set => this.SetRibbonIndex(MarkFishing, value); }
    public bool RibbonMarkCurry { get => HasRibbon(MarkCurry); set => this.SetRibbonIndex(MarkCurry, value); }
    public bool RibbonMarkUncommon { get => HasRibbon(MarkUncommon); set => this.SetRibbonIndex(MarkUncommon, value); }
    public bool RibbonMarkRare { get => HasRibbon(MarkRare); set => this.SetRibbonIndex(MarkRare, value); }
    public bool RibbonMarkRowdy { get => HasRibbon(MarkRowdy); set => this.SetRibbonIndex(MarkRowdy, value); }
    public bool RibbonMarkAbsentMinded { get => HasRibbon(MarkAbsentMinded); set => this.SetRibbonIndex(MarkAbsentMinded, value); }
    public bool RibbonMarkJittery { get => HasRibbon(MarkJittery); set => this.SetRibbonIndex(MarkJittery, value); }
    public bool RibbonMarkExcited { get => HasRibbon(MarkExcited); set => this.SetRibbonIndex(MarkExcited, value); }
    public bool RibbonMarkCharismatic { get => HasRibbon(MarkCharismatic); set => this.SetRibbonIndex(MarkCharismatic, value); }
    public bool RibbonMarkCalmness { get => HasRibbon(MarkCalmness); set => this.SetRibbonIndex(MarkCalmness, value); }
    public bool RibbonMarkIntense { get => HasRibbon(MarkIntense); set => this.SetRibbonIndex(MarkIntense, value); }
    public bool RibbonMarkZonedOut { get => HasRibbon(MarkZonedOut); set => this.SetRibbonIndex(MarkZonedOut, value); }
    public bool RibbonMarkJoyful { get => HasRibbon(MarkJoyful); set => this.SetRibbonIndex(MarkJoyful, value); }
    public bool RibbonMarkAngry { get => HasRibbon(MarkAngry); set => this.SetRibbonIndex(MarkAngry, value); }
    public bool RibbonMarkSmiley { get => HasRibbon(MarkSmiley); set => this.SetRibbonIndex(MarkSmiley, value); }
    public bool RibbonMarkTeary { get => HasRibbon(MarkTeary); set => this.SetRibbonIndex(MarkTeary, value); }
    public bool RibbonMarkUpbeat { get => HasRibbon(MarkUpbeat); set => this.SetRibbonIndex(MarkUpbeat, value); }
    public bool RibbonMarkPeeved { get => HasRibbon(MarkPeeved); set => this.SetRibbonIndex(MarkPeeved, value); }
    public bool RibbonMarkIntellectual { get => HasRibbon(MarkIntellectual); set => this.SetRibbonIndex(MarkIntellectual, value); }
    public bool RibbonMarkFerocious { get => HasRibbon(MarkFerocious); set => this.SetRibbonIndex(MarkFerocious, value); }
    public bool RibbonMarkCrafty { get => HasRibbon(MarkCrafty); set => this.SetRibbonIndex(MarkCrafty, value); }
    public bool RibbonMarkScowling { get => HasRibbon(MarkScowling); set => this.SetRibbonIndex(MarkScowling, value); }
    public bool RibbonMarkKindly { get => HasRibbon(MarkKindly); set => this.SetRibbonIndex(MarkKindly, value); }
    public bool RibbonMarkFlustered { get => HasRibbon(MarkFlustered); set => this.SetRibbonIndex(MarkFlustered, value); }
    public bool RibbonMarkPumpedUp { get => HasRibbon(MarkPumpedUp); set => this.SetRibbonIndex(MarkPumpedUp, value); }
    public bool RibbonMarkZeroEnergy { get => HasRibbon(MarkZeroEnergy); set => this.SetRibbonIndex(MarkZeroEnergy, value); }
    public bool RibbonMarkPrideful { get => HasRibbon(MarkPrideful); set => this.SetRibbonIndex(MarkPrideful, value); }
    public bool RibbonMarkUnsure { get => HasRibbon(MarkUnsure); set => this.SetRibbonIndex(MarkUnsure, value); }
    public bool RibbonMarkHumble { get => HasRibbon(MarkHumble); set => this.SetRibbonIndex(MarkHumble, value); }
    public bool RibbonMarkThorny { get => HasRibbon(MarkThorny); set => this.SetRibbonIndex(MarkThorny, value); }
    public bool RibbonMarkVigor { get => HasRibbon(MarkVigor); set => this.SetRibbonIndex(MarkVigor, value); }
    public bool RibbonMarkSlump { get => HasRibbon(MarkSlump); set => this.SetRibbonIndex(MarkSlump, value); }
    public bool RibbonTwinklingStar { get => HasRibbon(TwinklingStar); set => this.SetRibbonIndex(TwinklingStar, value); }
    public bool RibbonHisui { get => HasRibbon(Hisui); set => this.SetRibbonIndex(Hisui, value); }
    public bool RibbonChampionPaldea { get => HasRibbon(ChampionPaldea); set => this.SetRibbonIndex(ChampionPaldea, value); }
    public bool RibbonMarkJumbo { get => HasRibbon(MarkJumbo); set => this.SetRibbonIndex(MarkJumbo, value); }
    public bool RibbonMarkMini { get => HasRibbon(MarkMini); set => this.SetRibbonIndex(MarkMini, value); }
    public bool RibbonMarkItemfinder { get => HasRibbon(MarkItemfinder); set => this.SetRibbonIndex(MarkItemfinder, value); }
    public bool RibbonMarkPartner { get => HasRibbon(MarkPartner); set => this.SetRibbonIndex(MarkPartner, value); }
    public bool RibbonMarkGourmand { get => HasRibbon(MarkGourmand); set => this.SetRibbonIndex(MarkGourmand, value); }
    public bool RibbonOnceInALifetime { get => HasRibbon(OnceInALifetime); set => this.SetRibbonIndex(OnceInALifetime, value); }
    public bool RibbonMarkAlpha { get => HasRibbon(MarkAlpha); set => this.SetRibbonIndex(MarkAlpha, value); }
    public bool RibbonMarkMightiest { get => HasRibbon(MarkMightiest); set => this.SetRibbonIndex(MarkMightiest, value); }
    public bool RibbonMarkTitan { get => HasRibbon(MarkTitan); set => this.SetRibbonIndex(MarkTitan, value); }
    public bool RibbonPartner { get => HasRibbon(Partner); set => this.SetRibbonIndex(Partner, value); }

    public int GetRibbonByte(int index) => RibbonSpan.IndexOf((byte)index);
    public bool GetRibbon(int index) => RibbonSpan.Contains((byte)index);

    public void SetRibbon(int index, bool value = true)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)RibbonIndexExtensions.MAX_G9);
        if (value)
        {
            if (GetRibbon(index))
                return;
            var openIndex = RibbonSpan.IndexOf(RibbonByteNone);
            ArgumentOutOfRangeException.ThrowIfNegative(openIndex); // Full?
            SetRibbonAtIndex(openIndex, (byte)index);
        }
        else
        {
            var ofs = GetRibbonByte(index);
            if (ofs < 0)
                return;
            SetRibbonAtIndex(ofs, RibbonByteNone);
        }
    }
    #endregion

    public bool IsMissingExtraMark(PKM pk, out RibbonIndex missing)
    {
        foreach (var value in RibbonSpan)
        {
            missing = (RibbonIndex)value;
            if (!missing.IsEncounterMark8)
                continue;
            if (pk is IRibbonSetMark8 m8 && !m8.HasMark8(missing))
                return true;
        }
        missing = default;
        return false;
    }

    public SeedCorrelationResult TryGetSeed(PKM pk, out ulong seed) =>
        GetParams(PersonalTable.ZA[Species, Form]).TryGetSeed(pk, out seed)
            ? SeedCorrelationResult.Success
            : SeedCorrelationResult.Invalid;

    public LumioseCorrelation Correlation => LumioseCorrelation.SkipTrainer;
    public byte FlawlessIVCount => GetFlawlessIVCount(IV_HP);

    public GenerateParam9a GetParams(PersonalInfo9ZA pi)
    {
        const byte rollCount = 1;
        var hp = IV_HP;
        var flawless = FlawlessIVCount;
        var ivs = flawless != 0 ? default : new IndividualValueSet((sbyte)hp, (sbyte)IV_ATK, (sbyte)IV_DEF, (sbyte)IV_SPE, (sbyte)IV_SPA, (sbyte)IV_SPD);
        var sizeType = Scale == 256 ? SizeType9.RANDOM : SizeType9.VALUE;
        var gender = Gender switch
        {
            0 => PersonalInfo.RatioMagicMale,
            1 => PersonalInfo.RatioMagicFemale,
            2 => PersonalInfo.RatioMagicGenderless,
            _ => pi.Gender,
        };
        return new GenerateParam9a(gender, flawless, rollCount, Correlation, sizeType, (byte)Scale, Nature, Ability, Shiny, ivs);
    }

    private static byte GetFlawlessIVCount(int hp)
    {
        var tryFlawless = hp - 0xFC;
        if ((uint)tryFlawless < 3)
            return (byte)(tryFlawless + 1);
        return 0;
    }
}
