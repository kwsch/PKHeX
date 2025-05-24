using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Mystery Gift Template File (LGP/E)
/// </summary>
public sealed class WB7 : DataMysteryGift, ILangNick, IAwakened, IRelearn, IEncounterServerDate, INature, ILangNicknamedTemplate,
    IMetLevel, IRestrictVersion, IRibbonSetEvent3, IRibbonSetEvent4
{
    public WB7() : this(new byte[Size]) { }
    public WB7(Memory<byte> raw) : base(raw) { }

    public const int Size = 0x310;
    private const int CardStart = 0x208;
    private Span<byte> Card => Data.Slice(CardStart, 0x108);

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
        get => ReadUInt16LittleEndian(Card);
        set => WriteUInt16LittleEndian(Card, (ushort)value);
    }

    public override string CardTitle
    {
        // Max len 36 char, followed by null terminator
        get => StringConverter8.GetString(Card.Slice(2, 0x4A));
        set => StringConverter8.SetString(Card.Slice(2, 0x4A), value, 36, StringConverterOption.ClearZero);
    }

    private uint RawDate
    {
        get => ReadUInt32LittleEndian(Card[0x4C..]);
        set => WriteUInt32LittleEndian(Card[0x4C..], value);
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
            if (!DateUtil.IsValidDate(Year, Month, Day))
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

    public int CardLocation { get => Card[0x50]; set => Card[0x50] = (byte)value; }

    public int CardType { get => Card[0x51]; set => Card[0x51] = (byte)value; }
    public byte CardFlags { get => Card[0x52]; set => Card[0x52] = value; }

    public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
    public override bool GiftUsed { get => (CardFlags & 2) == 2; set => CardFlags = (byte)((CardFlags & ~2) | (value ? 2 : 0)); }
    public bool GiftOncePerDay { get => (CardFlags & 4) == 4; set => CardFlags = (byte)((CardFlags & ~4) | (value ? 4 : 0)); }

    public bool MultiObtain { get => Card[0x53] == 1; set => Card[0x53] = value ? (byte)1 : (byte)0; }

    // Item Properties
    public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }
    public int GetQuantity(int index) => ReadUInt16LittleEndian(Card[(0x6A + (0x4 * index))..]);
    public void SetQuantity(int index, ushort quantity) => WriteUInt16LittleEndian(Card[(0x6A + (4 * index))..], quantity);
    public override int ItemID { get => ReadUInt16LittleEndian(Card[0x68..]); set => WriteUInt16LittleEndian(Card[0x68..], (ushort)value); }
    public int GetItem(int index) => ReadUInt16LittleEndian(Card[(0x68 + (0x4 * index))..]);
    public void SetItem(int index, ushort item) => WriteUInt16LittleEndian(Card[(0x68 + (4 * index))..], item);

    public override int Quantity
    {
        get => ReadUInt16LittleEndian(Card[0x6A..]);
        set => WriteUInt16LittleEndian(Card[0x6A..], (ushort)value);
    }

    // Pokémon Properties
    public override bool IsEntity { get => CardType == 0; set { if (value) CardType = 0; } }
    public override bool IsShiny => Shiny.IsShiny();

    public override Shiny Shiny => PIDType switch
    {
        ShinyType6.FixedValue => FixedShinyType(),
        ShinyType6.Random => Shiny.Random,
        ShinyType6.Always => Shiny.Always,
        ShinyType6.Never => Shiny.Never,
        _ => throw new ArgumentOutOfRangeException(),
    };

    private Shiny FixedShinyType() => GetShinyXor() switch
    {
        <= 15 => Shiny.Always,
        _ => Shiny.Never,
    };

    private uint GetShinyXor()
    {
        var xor = PID ^ ID32;
        return (xor >> 16) ^ (xor & 0xFFFF);
    }

    public override uint ID32 { get => ReadUInt32LittleEndian(Card[0x68..]); set => WriteUInt32LittleEndian(Card[0x68..], value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Card[0x68..]); set => WriteUInt16LittleEndian(Card[0x68..], value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Card[0x6A..]); set => WriteUInt16LittleEndian(Card[0x6A..], value); }
    public int OriginGame { get => ReadInt32LittleEndian(Card[0x6C..]); set => WriteInt32LittleEndian(Card[0x6C..], value); }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Card[0x70..]); set => WriteUInt32LittleEndian(Card[0x70..], value); }
    public override byte Ball { get => Card[0x76]; set => Card[0x76] = value; }
    // held item: unused
    public override int HeldItem { get => ReadUInt16LittleEndian(Card[0x78..]); set => WriteUInt16LittleEndian(Card[0x78..], (ushort)value); }
    public ushort Move1 { get => ReadUInt16LittleEndian(Card[0x7A..]); set => WriteUInt16LittleEndian(Card[0x7A..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Card[0x7C..]); set => WriteUInt16LittleEndian(Card[0x7C..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Card[0x7E..]); set => WriteUInt16LittleEndian(Card[0x7E..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Card[0x80..]); set => WriteUInt16LittleEndian(Card[0x80..], value); }
    public override ushort Species { get => ReadUInt16LittleEndian(Card[0x82..]); set => WriteUInt16LittleEndian(Card[0x82..], value); }
    public override byte Form { get => Card[0x84]; set => Card[0x84] = value; }

    // public int Language { get => Card[0x85]; set => Card[0x85] = (byte)value; }

    // public string Nickname
    // {
    //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Card.Slice(0x86, 0x1A));
    //     set => Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Card.Slice(0x86);
    // }

    public Nature Nature { get => (Nature)Card[0xA0]; set => Card[0xA0] = (byte)value; }
    public override byte Gender { get => Card[0xA1]; set => Card[0xA1] = value; }
    public override int AbilityType { get => IsHOMEGift ? Card[0xA2] : 3; set => Card[0xA2] = (byte)value; } // no references, always ability 0/1
    public ShinyType6 PIDType { get => (ShinyType6)Card[0xA3]; set => Card[0xA3] = (byte)value; }
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Card[0xA4..]); set => WriteUInt16LittleEndian(Card[0xA4..], value); }
    public override ushort Location  { get => ReadUInt16LittleEndian(Card[0xA6..]); set => WriteUInt16LittleEndian(Card[0xA6..], value); }
    public byte MetLevel { get => Card[0xA8]; set => Card[0xA8] = value; }

    public int IV_HP { get => Card[0xAF]; set => Card[0xAF] = (byte)value; }
    public int IV_ATK { get => Card[0xB0]; set => Card[0xB0] = (byte)value; }
    public int IV_DEF { get => Card[0xB1]; set => Card[0xB1] = (byte)value; }
    public int IV_SPE { get => Card[0xB2]; set => Card[0xB2] = (byte)value; }
    public int IV_SPA { get => Card[0xB3]; set => Card[0xB3] = (byte)value; }
    public int IV_SPD { get => Card[0xB4]; set => Card[0xB4] = (byte)value; }

    public byte OTGender { get => Card[0xB5]; set => Card[0xB5] = value; }

    // public override string OriginalTrainerName
    // {
    //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Card.Slice(0xB6, 0x1A));
    //     set => Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Card.Slice(0xB6);
    // }

    public override byte Level { get => Card[0xD0]; set => Card[0xD0] = value; }
    public override bool IsEgg { get => Card[0xD1] == 1; set => Card[0xD1] = value ? (byte)1 : (byte)0; }
    public ushort AdditionalItem { get => ReadUInt16LittleEndian(Card[0xD2..]); set => WriteUInt16LittleEndian(Card[0xD2..], value); }

    public uint PID { get => ReadUInt32LittleEndian(Card[0xD4..]); set => WriteUInt32LittleEndian(Card[0xD4..], value); }
    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Card[0xD8..]); set => WriteUInt16LittleEndian(Card[0xD8..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Card[0xDA..]); set => WriteUInt16LittleEndian(Card[0xDA..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Card[0xDC..]); set => WriteUInt16LittleEndian(Card[0xDC..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Card[0xDE..]); set => WriteUInt16LittleEndian(Card[0xDE..], value); }

    public byte AV_HP  { get => Card[0xE5]; set => Card[0xE5] = value; }
    public byte AV_ATK { get => Card[0xE6]; set => Card[0xE6] = value; }
    public byte AV_DEF { get => Card[0xE7]; set => Card[0xE7] = value; }
    public byte AV_SPE { get => Card[0xE8]; set => Card[0xE8] = value; }
    public byte AV_SPA { get => Card[0xE9]; set => Card[0xE9] = value; }
    public byte AV_SPD { get => Card[0xEA]; set => Card[0xEA] = value; }
    private byte RIB0 { get => Data[0x74]; set => Data[0x74] = value; }
    private byte RIB1 { get => Data[0x75]; set => Data[0x75] = value; }

    public bool RibbonChampionBattle { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonChampionRegional { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonChampionNational { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonCountry { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonNational { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonEarth { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonWorld { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonEvent { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonChampionWorld { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonBirthday { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonSpecial { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonSouvenir { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonWishing { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonClassic { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonPremier { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RIB1_7 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

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
            for (int i = 1; i <= (int)LanguageID.ChineseT; i++)
                SetOT(i, value);
        }
    }

    public string Nickname => GetIsNicknamed(Language) ? GetNickname(Language) : string.Empty;
    public bool IsNicknamed => false;
    public int Language => 2;

    /// <summary>
    /// Can only be received by the Mainland China release of the game, which can only select a game-language of Simplified Chinese.
    /// </summary>
    /// <remarks>
    /// Gifts received can be locally traded to the international release, so there is no need to consider residence or transferability.
    /// </remarks>
    public bool IsMainlandChinaGift => CardID is (> 1500 and <= 1503);

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

    public bool GetHasOT(int language) => ReadUInt16LittleEndian(Data.Slice(GetOTOffset(language))) != 0;

    private Span<byte> GetNicknameSpan(int language) => Data.Slice(GetNicknameOffset(language), 0x1A);
    public string GetNickname(int language) => StringConverter8.GetString(GetNicknameSpan(language));
    public void SetNickname(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetNicknameSpan(language), value, 12, StringConverterOption.ClearZero);

    private Span<byte> GetOTSpan(int language) => Data.Slice(GetOTOffset(language), 0x1A);
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

    public bool IsHOMEGift => CardID >= 9000;

    public override PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        var rnd = Util.Rand;

        byte currentLevel = Level > 0 ? Level : (byte)(1 + rnd.Next(100));
        var metLevel = MetLevel > 0 ? MetLevel : currentLevel;
        var pi = PersonalTable.GG.GetFormEntry(Species, Form);

        var redeemLanguage = IsMainlandChinaGift ? (int)LanguageID.ChineseS : tr.Language;
        var language = GetLanguage(redeemLanguage);
        bool hasOT = GetHasOT(redeemLanguage);

        var pk = new PB7
        {
            Species = Species,
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

            RibbonSouvenir = RibbonSouvenir, // HOME Meltan
        };

        if (hasOT)
        {
            pk.HandlingTrainerName = tr.OT;
            pk.HandlingTrainerGender = tr.Gender;
            pk.CurrentHandler = 1;
        }

        pk.HealPP();

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

        pk.ReceivedDate = pk.MetDate = Date ?? GetSuggestedDate();
        pk.ReceivedTime = EncounterDate.GetTime();
        pk.IsNicknamed = GetIsNicknamed(redeemLanguage);
        pk.Nickname = pk.IsNicknamed ? GetNickname(redeemLanguage) : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

        SetPINGA(pk, criteria);

        if (IsEgg)
            SetEggMetData(pk);
        pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        if (IsHeightWeightFixed)
        {
            pk.HeightScalar = pk.WeightScalar = GetHomeScalar();
            pk.HeightAbsolute = GetHomeHeightAbsolute();
            pk.WeightAbsolute = GetHomeWeightAbsolute();
            pk.ResetCP(); //do not reset dimensions
        }
        else
        {
            pk.HeightScalar = (byte)rnd.Next(0x100);
            pk.WeightScalar = (byte)rnd.Next(0x100);
            pk.ResetCalculatedValues(); // cp & dimensions
        }

        pk.RefreshChecksum();
        return pk;
    }

    /// <summary>
    ///  HOME Meltan is a special case where height/weight is fixed.
    /// </summary>
    public bool IsHeightWeightFixed => CardID is 9028;

    private byte GetHomeScalar() => CardID switch
    {
        9028 => 128,
        _ => throw new ArgumentException(),
    };

    public float GetHomeHeightAbsolute() => CardID switch
    {
        9028 => 18.1490211f,
        _ => throw new ArgumentException(),
    };

    public float GetHomeWeightAbsolute() => CardID switch
    {
        9028 => 77.09419f,
        _ => throw new ArgumentException(),
    };

    private DateOnly GetSuggestedDate()
    {
        if (!IsDateRestricted)
            return EncounterDate.GetDateSwitch();
        if (this.GetDistributionWindow(out var window))
            return window.GetGenerateDate();
        return EncounterDate.GetDateSwitch();
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

        if (IsMainlandChinaGift)
            return language == (int)LanguageID.ChineseS;

        if (CanBeAnyLanguage())
            return true;

        return Data.Slice(0x1D8, 9).Contains((byte)language);
    }

    public bool CanBeAnyLanguage()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Data[0x1D8 + i] != 0)
                return false;
        }

        return !IsMainlandChinaGift;
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

        if (IsHeightWeightFixed)
        {
            var scalar = GetHomeScalar();
            if (pk is IScaledSize hw && (hw.HeightScalar != scalar || hw.WeightScalar != scalar))
                return false;
            if (pk is IScaledSize3 sc && sc.Scale != scalar)
                return false;
        }

        if (pk is IAwakened s && s.IsAwakeningBelow(this))
            return false;

        if (PIDType is ShinyType6.FixedValue && pk.PID != PID)
            return false;

        return true;
    }

    public bool IsDateRestricted => IsHOMEGift;
    protected override bool IsMatchDeferred(PKM pk) => false;
    protected override bool IsMatchPartial(PKM pk) => false;
}
