using System;
using static System.Buffers.Binary. BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Mystery Gift Template File (LGP/E)
/// </summary>
public sealed class WB7(byte[] Data)
    : DataMysteryGift(Data), ILangNick, IAwakened, IRelearn, INature, ILangNicknamedTemplate, IRestrictVersion
{
    public WB7() : this(new byte[Size]) { }

    public const int Size = 0x310;
    private const int CardStart = 0x208;
    public override bool FatefulEncounter => true;

    public override byte Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;
    public override GameVersion Version => GameVersion.GG;

    public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }

    public bool CanBeReceivedByVersion(GameVersion version)
    {
        if (version is not (GameVersion.GP or GameVersion.GE))
            return false;
        if (RestrictVersion == 0)
            return true; // no data
        var bitIndex = version - GameVersion.GP;
        var bit = 1 << bitIndex;
        return (RestrictVersion & bit) != 0;
    }

    // General Card Properties
    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0), (ushort)value);
    }

    public override string CardTitle
    {
        // Max len 36 char, followed by null terminator
        get => StringConverter8.GetString(Data.AsSpan(CardStart + 2, 0x4A));
        set => StringConverter8.SetString(Data.AsSpan(CardStart + 2, 0x4A), value, 36, StringConverterOption.ClearZero);
    }

    private uint RawDate
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(CardStart + 0x4C));
        set => WriteUInt32LittleEndian(Data.AsSpan(CardStart + 0x4C), value);
    }

    private uint Year
    {
        get => (RawDate / 10000) + 2000;
        set => RawDate = SetDate(value, Month, Day);
    }

    private uint Month
    {
        get => RawDate % 10000 / 100;
        set => RawDate = SetDate(Year, value, Day);
    }

    private uint Day
    {
        get => RawDate % 100;
        set => RawDate = SetDate(Year, Month, value);
    }

    private static uint SetDate(uint year, uint month, uint day) => (Math.Max(0, year - 2000) * 10000) + (month * 100) + day;

    /// <summary>
    /// Gets or sets the date of the card.
    /// </summary>
    public DateOnly? Date
    {
        get
        {
            // Check to see if date is valid
            if (!DateUtil.IsDateValid(Year, Month, Day))
                return null;

            return new DateOnly((int)Year, (int)Month, (int)Day);
        }
        set
        {
            if (value is { } dt)
            {
                // Only update the properties if a value is provided.
                Year = (ushort)dt.Year;
                Month = (byte)dt.Month;
                Day = (byte)dt.Day;
            }
            else
            {
                // Clear the Met Date.
                // If code tries to access MetDate again, null will be returned.
                Year = 0;
                Month = 0;
                Day = 0;
            }
        }
    }

    public int CardLocation { get => Data[CardStart + 0x50]; set => Data[CardStart + 0x50] = (byte)value; }

    public int CardType { get => Data[CardStart + 0x51]; set => Data[CardStart + 0x51] = (byte)value; }
    public byte CardFlags { get => Data[CardStart + 0x52]; set => Data[CardStart + 0x52] = value; }

    public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
    public override bool GiftUsed { get => (CardFlags & 2) == 2; set => CardFlags = (byte)((CardFlags & ~2) | (value ? 2 : 0)); }
    public bool GiftOncePerDay { get => (CardFlags & 4) == 4; set => CardFlags = (byte)((CardFlags & ~4) | (value ? 4 : 0)); }

    public bool MultiObtain { get => Data[CardStart + 0x53] == 1; set => Data[CardStart + 0x53] = value ? (byte)1 : (byte)0; }

    // Item Properties
    public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }
    public override int ItemID { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x68)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x68), (ushort)value); }
    public int GetItem(int index) => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x68 + (0x4 * index)));
    public void SetItem(int index, ushort item) => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x68 + (4 * index)), item);
    public int GetQuantity(int index) => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x6A + (0x4 * index)));
    public void SetQuantity(int index, ushort quantity) => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x6A + (4 * index)), quantity);

    public override int Quantity
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x6A));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x6A), (ushort)value);
    }

    // Pokémon Properties
    public override bool IsEntity { get => CardType == 0; set { if (value) CardType = 0; } }
    public override bool IsShiny => PIDType == ShinyType6.Always;

    public override Shiny Shiny => PIDType switch
    {
        ShinyType6.FixedValue => Shiny.FixedValue,
        ShinyType6.Random => Shiny.Random,
        ShinyType6.Always => Shiny.Always,
        ShinyType6.Never => Shiny.Never,
        _ => throw new ArgumentOutOfRangeException(),
    };

    public override uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(CardStart + 0x68));
        set => WriteUInt32LittleEndian(Data.AsSpan(CardStart + 0x68), value);
    }

    public override ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x68));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x68), value);
    }

    public override ushort SID16 {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x6A));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x6A), value);
    }

    public int OriginGame
    {
        get => ReadInt32LittleEndian(Data.AsSpan(CardStart + 0x6C));
        set => WriteInt32LittleEndian(Data.AsSpan(CardStart + 0x6C), value);
    }

    public uint EncryptionConstant {
        get => ReadUInt32LittleEndian(Data.AsSpan(CardStart + 0x70));
        set => WriteUInt32LittleEndian(Data.AsSpan(CardStart + 0x70), value);
    }

    public override byte Ball
    {
        get => Data[CardStart + 0x76];
        set => Data[CardStart + 0x76] = value; }

    public override int HeldItem // no references
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x78));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x78), (ushort)value);
    }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x7A)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x7A), value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x7C)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x7C), value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x7E)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x7E), value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x80)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x80), value); }
    public override ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x82)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x82), value); }
    public override byte Form { get => Data[CardStart + 0x84]; set => Data[CardStart + 0x84] = value; }

    // public int Language { get => Data[CardStart + 0x85]; set => Data[CardStart + 0x85] = (byte)value; }

    // public string Nickname
    // {
    //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 0x86, 0x1A));
    //     set => Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Data, CardStart + 0x86);
    // }

    public Nature Nature { get => (Nature)Data[CardStart + 0xA0]; set => Data[CardStart + 0xA0] = (byte)value; }
    public override byte Gender { get => Data[CardStart + 0xA1]; set => Data[CardStart + 0xA1] = value; }
    public override int AbilityType { get => 3; set => Data[CardStart + 0xA2] = (byte)value; } // no references, always ability 0/1
    public ShinyType6 PIDType { get => (ShinyType6)Data[CardStart + 0xA3]; set => Data[CardStart + 0xA3] = (byte)value; }
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xA4)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xA4), value); }
    public override ushort Location  { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xA6)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xA6), value); }
    public byte MetLevel { get => Data[CardStart + 0xA8]; set => Data[CardStart + 0xA8] = value; }

    public int IV_HP { get => Data[CardStart + 0xAF]; set => Data[CardStart + 0xAF] = (byte)value; }
    public int IV_ATK { get => Data[CardStart + 0xB0]; set => Data[CardStart + 0xB0] = (byte)value; }
    public int IV_DEF { get => Data[CardStart + 0xB1]; set => Data[CardStart + 0xB1] = (byte)value; }
    public int IV_SPE { get => Data[CardStart + 0xB2]; set => Data[CardStart + 0xB2] = (byte)value; }
    public int IV_SPA { get => Data[CardStart + 0xB3]; set => Data[CardStart + 0xB3] = (byte)value; }
    public int IV_SPD { get => Data[CardStart + 0xB4]; set => Data[CardStart + 0xB4] = (byte)value; }

    public byte OTGender { get => Data[CardStart + 0xB5]; set => Data[CardStart + 0xB5] = value; }

    // public override string OriginalTrainerName
    // {
    //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 0xB6, 0x1A));
    //     set => Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, CardStart + 0xB6);
    // }

    public override byte Level { get => Data[CardStart + 0xD0]; set => Data[CardStart + 0xD0] = value; }
    public override bool IsEgg { get => Data[CardStart + 0xD1] == 1; set => Data[CardStart + 0xD1] = value ? (byte)1 : (byte)0; }
    public ushort AdditionalItem { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xD2)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xD2), value); }

    public uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0xD4)); set => WriteUInt32LittleEndian(Data.AsSpan(0xD4), value); }
    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xD8)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xD8), value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xDA)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xDA), value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xDC)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xDC), value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0xDE)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0xDE), value); }

    public byte AV_HP  { get => Data[CardStart + 0xE5]; set => Data[CardStart + 0xE5] = value; }
    public byte AV_ATK { get => Data[CardStart + 0xE6]; set => Data[CardStart + 0xE6] = value; }
    public byte AV_DEF { get => Data[CardStart + 0xE7]; set => Data[CardStart + 0xE7] = value; }
    public byte AV_SPE { get => Data[CardStart + 0xE8]; set => Data[CardStart + 0xE8] = value; }
    public byte AV_SPA { get => Data[CardStart + 0xE9]; set => Data[CardStart + 0xE9] = value; }
    public byte AV_SPD { get => Data[CardStart + 0xEA]; set => Data[CardStart + 0xEA] = value; }

    // Meta Accessible Properties
    public int[] IVs
    {
        get => [IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD];
        set
        {
            if (value.Length != 6) return;
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

    public bool GetIsNicknamed(int language) => Data[GetNicknameOffset(language)] != 0;

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
            for (int i = 1; i < (int)LanguageID.ChineseT; i++)
                SetOT(i, value);
        }
    }

    public string Nickname => GetIsNicknamed(Language) ? GetNickname(Language) : string.Empty;
    public bool IsNicknamed => false;
    public int Language => 2;

    public int GetLanguage(int redeemLanguage)
    {
        var languageOffset = GetLanguageIndex(redeemLanguage);
        var value = Data[0x1D8 + languageOffset];
        if (value != 0) // Fixed receiving language
            return value;

        // Use redeeming language (clamped to legal values for our sake)
        if (redeemLanguage is < (int)LanguageID.Japanese or (int)LanguageID.UNUSED_6 or > (int)LanguageID.ChineseT)
            return (int)LanguageID.English; // fallback
        return redeemLanguage;
    }

    public bool GetHasOT(int language) => ReadUInt16LittleEndian(Data.AsSpan(GetOTOffset(language))) != 0;

    private Span<byte> GetNicknameSpan(int language) => Data.AsSpan(GetNicknameOffset(language), 0x1A);
    public string GetNickname(int language) => StringConverter8.GetString(GetNicknameSpan(language));
    public void SetNickname(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetNicknameSpan(language), value, 12, StringConverterOption.ClearZero);

    private Span<byte> GetOTSpan(int language) => Data.AsSpan(GetOTOffset(language), 0x1A);
    public string GetOT(int language) => StringConverter8.GetString(GetOTSpan(language));
    public void SetOT(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetOTSpan(language), value, 12, StringConverterOption.ClearZero);

    private static int GetNicknameOffset(int language)
    {
        int index = GetLanguageIndex(language);
        return 0x04 + (index * 0x1A);
    }

    private static int GetOTOffset(int language)
    {
        int index = GetLanguageIndex(language);
        return 0xEE + (index * 0x1A);
    }

    public override PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        var rnd = Util.Rand;

        byte currentLevel = Level > 0 ? Level : (byte)(1 + rnd.Next(100));
        var metLevel = MetLevel > 0 ? MetLevel : currentLevel;
        var pi = PersonalTable.GG.GetFormEntry(Species, Form);

        var redeemLanguage = tr.Language;
        var language = GetLanguage(redeemLanguage);
        bool hasOT = GetHasOT(redeemLanguage);

        var pk = new PB7
        {
            Species = Species,
            HeldItem = HeldItem,
            TID16 = TID16,
            SID16 = SID16,
            MetLevel = metLevel,
            Form = Form,
            EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : rnd.Rand32(),
            Version = OriginGame != 0 ? (GameVersion)OriginGame : tr.Version,
            Language = language,
            Ball = Ball,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            RelearnMove1 = RelearnMove1,
            RelearnMove2 = RelearnMove2,
            RelearnMove3 = RelearnMove3,
            RelearnMove4 = RelearnMove4,
            MetLocation = Location,
            EggLocation = EggLocation,
            AV_HP = AV_HP,
            AV_ATK = AV_ATK,
            AV_DEF = AV_DEF,
            AV_SPE = AV_SPE,
            AV_SPA = AV_SPA,
            AV_SPD = AV_SPD,

            OriginalTrainerName = hasOT ? GetOT(redeemLanguage) : tr.OT,
            OriginalTrainerGender = (byte)(OTGender != 3 ? OTGender % 2 : tr.Gender),

            EXP = Experience.GetEXP(currentLevel, pi.EXPGrowth),

            OriginalTrainerFriendship = pi.BaseFriendship,
            FatefulEncounter = true,
        };

        if (hasOT)
        {
            pk.HandlingTrainerName = tr.OT;
            pk.HandlingTrainerGender = tr.Gender;
            pk.CurrentHandler = 1;
        }

        pk.SetMaximumPPCurrent();

        if ((tr.Generation > Generation && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
        {
            // give random valid game
            do { pk.Version = GameVersion.GP + (byte)rnd.Next(2); }
            while (!CanBeReceivedByVersion(pk.Version));
        }

        if (OTGender == 3)
        {
            pk.TID16 = tr.TID16;
            pk.SID16 = tr.SID16;
        }

        pk.MetDate = Date ?? EncounterDate.GetDateSwitch();
        pk.IsNicknamed = GetIsNicknamed(redeemLanguage);
        pk.Nickname = pk.IsNicknamed ? GetNickname(redeemLanguage) : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

        SetPINGA(pk, criteria);

        if (IsEgg)
            SetEggMetData(pk);
        pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        pk.HeightScalar = (byte)rnd.Next(0x100);
        pk.WeightScalar = (byte)rnd.Next(0x100);
        pk.ResetCalculatedValues(); // cp & dimensions

        pk.RefreshChecksum();
        return pk;
    }

    private void SetEggMetData(PB7 pk)
    {
        pk.IsEgg = true;
        pk.EggMetDate = Date;
        pk.Nickname = SpeciesName.GetEggName(pk.Language, Generation);
        pk.IsNicknamed = true;
    }

    private void SetPINGA(PB7 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        pk.Nature = criteria.GetNature(Nature);
        pk.Gender = criteria.GetGender(Gender, pi);
        var av = GetAbilityIndex(criteria);
        pk.RefreshAbility(av);
        SetPID(pk);
        SetIVs(pk);
    }

    private int GetAbilityIndex(EncounterCriteria criteria) => AbilityType switch
    {
        00 or 01 or 02 => AbilityType, // Fixed 0/1/2
        03 or 04 => criteria.GetAbilityFromNumber(Ability), // 0/1 or 0/1/H
        _ => throw new ArgumentOutOfRangeException(nameof(AbilityType)),
    };

    public override AbilityPermission Ability => AbilityType switch
    {
        0 => AbilityPermission.OnlyFirst,
        1 => AbilityPermission.OnlySecond,
        2 => AbilityPermission.OnlyHidden,
        3 => AbilityPermission.Any12,
        _ => AbilityPermission.Any12H,
    };

    private void SetPID(PB7 pk)
    {
        switch (PIDType)
        {
            case ShinyType6.FixedValue: // Specified
                pk.PID = PID;
                break;
            case ShinyType6.Random: // Random
                pk.PID = Util.Rand32();
                break;
            case ShinyType6.Always: // Random Shiny
                var low = Util.Rand32() & 0xFFFF;
                pk.PID = ((low ^ pk.TID16 ^ pk.SID16) << 16) | low;
                break;
            case ShinyType6.Never: // Random Nonshiny
                pk.PID = Util.Rand32();
                if (pk.IsShiny) pk.PID ^= 0x10000000;
                break;
        }
    }

    private void SetIVs(PB7 pk)
    {
        Span<int> finalIVs = stackalloc int[6];
        GetIVs(finalIVs);
        var ivflag = finalIVs.IndexOfAny(0xFC, 0xFD, 0xFE);
        var rng = Util.Rand;
        if (ivflag == -1) // Random IVs
        {
            for (int i = 0; i < finalIVs.Length; i++)
            {
                if (finalIVs[i] > 31)
                    finalIVs[i] = rng.Next(32);
            }
        }
        else // 1/2/3 perfect IVs
        {
            int IVCount = finalIVs[ivflag] - 0xFB;
            do { finalIVs[rng.Next(6)] = 31; }
            while (finalIVs.Count(31) < IVCount);
            for (int i = 0; i < finalIVs.Length; i++)
            {
                if (finalIVs[i] != 31)
                    finalIVs[i] = rng.Next(32);
            }
        }
        pk.SetIVs(finalIVs);
    }

    public bool CanHaveLanguage(int language)
    {
        if (language is < (int) LanguageID.Japanese or > (int) LanguageID.ChineseT)
            return false;

        if (CanBeAnyLanguage())
            return true;

        return Array.IndexOf(Data, (byte)language, 0x1D8, 9) >= 0;
    }

    public bool CanBeAnyLanguage()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Data[0x1D8 + i] != 0)
                return false;
        }
        return true;
    }

    public bool CanHandleOT(int language) => string.IsNullOrEmpty(GetOT(language));

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsEgg)
        {
            if (OTGender != 3)
            {
                if (SID16 != pk.SID16) return false;
                if (TID16 != pk.TID16) return false;
                if (OTGender != pk.OriginalTrainerGender) return false;
            }
            var OT = GetOT(pk.Language);
            if (!string.IsNullOrEmpty(OT) && OT != pk.OriginalTrainerName) return false;
            if (OriginGame != 0 && (GameVersion)OriginGame != pk.Version) return false;
            if (EncryptionConstant != 0 && EncryptionConstant != pk.EncryptionConstant) return false;

            if (!IsMatchEggLocation(pk)) return false;
            if (!CanBeAnyLanguage() && !CanHaveLanguage(pk.Language))
                return false;
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (IsEgg)
        {
            if (EggLocation != pk.EggLocation) // traded
            {
                if (pk.EggLocation != Locations.LinkTrade6)
                    return false;
            }
            else if (PIDType == 0 && pk.IsShiny)
            {
                return false; // can't be traded away for unshiny
            }

            if (pk is { IsEgg: true, Context: not EntityContext.Gen7b })
                return false;
        }
        else
        {
            if (!Shiny.IsValid(pk)) return false;
            if (!IsMatchEggLocation(pk)) return false;
            if (Location != pk.MetLocation) return false;
        }

        if (MetLevel != pk.MetLevel) return false;
        if (Ball != pk.Ball) return false;
        if (OTGender < 3 && OTGender != pk.OriginalTrainerGender) return false;
        if ((sbyte)Nature != -1 && pk.Nature != Nature) return false;
        if (Gender != 3 && Gender != pk.Gender) return false;

        if (pk is IAwakened s && s.IsAwakeningBelow(this))
            return false;

        return true;
    }

    protected override bool IsMatchDeferred(PKM pk) => false;
    protected override bool IsMatchPartial(PKM pk) => false;
}
