using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Mystery Gift Template File
/// </summary>
public sealed class WC6(byte[] Data) : DataMysteryGift(Data), IRibbonSetEvent3, IRibbonSetEvent4,
    ILangNick, IContestStats, INature, IMemoryOT, IRelearn, IRestrictVersion
{
    public WC6() : this(new byte[Size]) { }

    public const int Size = 0x108;
    public const uint EonTicketConst = 0x225D73C2;
    public override byte Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;
    public override bool FatefulEncounter => !IsLinkGift; // Link gifts do not set fateful encounter
    public bool IsLinkGift => Location == Locations.LinkGift6;

    public int RestrictLanguage { get; set; } // None
    public byte RestrictVersion { get; set; } // Permit All

    public bool CanBeReceivedByVersion(GameVersion version)
    {
        if (version is < GameVersion.X or > GameVersion.OR)
            return false;
        if (RestrictVersion == 0)
            return true; // no data
        var bitIndex = (int)(version - GameVersion.X);
        var bit = 1 << bitIndex;
        return (RestrictVersion & bit) != 0;
    }

    // General Card Properties
    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0));
        set => WriteUInt16LittleEndian(Data.AsSpan(0), (ushort)value);
    }

    public override string CardTitle
    {
        // Max len 36 char, followed by null terminator
        get => StringConverter6.GetString(Data.AsSpan(2, 0x4A));
        set => StringConverter6.SetString(Data.AsSpan(2, 0x4A), value, 36, Language, StringConverterOption.ClearZero);
    }

    internal uint RawDate
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x4C));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x4C), value);
    }

    private uint Year
    {
        get => RawDate / 10000;
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

    public static uint SetDate(uint year, uint month, uint day) => (year * 10000) + (month * 100) + day;

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

    public int CardLocation { get => Data[0x50]; set => Data[0x50] = (byte)value; }

    public int CardType { get => Data[0x51]; set => Data[0x51] = (byte)value; }
    public override bool GiftUsed { get => Data[0x52] >> 1 > 0; set => Data[0x52] = (byte)((Data[0x52] & ~2) | (value ? 2 : 0)); }
    public bool MultiObtain { get => Data[0x53] == 1; set => Data[0x53] = value ? (byte)1 : (byte)0; }

    // Item Properties
    public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }

    public override int ItemID {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x68));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x68), (ushort)value); }

    public override int Quantity {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x70));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x70), (ushort)value); }

    // Pokémon Properties
    public override bool IsEntity { get => CardType == 0; set { if (value) CardType = 0; } }
    public override bool IsShiny => Shiny.IsShiny();

    public override Shiny Shiny => IsEgg ? Shiny.Random : PIDType switch
    {
        ShinyType6.FixedValue => GetShinyXor() switch
        {
            0 => Shiny.AlwaysSquare,
            <= 15 => Shiny.AlwaysStar,
            _ => Shiny.Never,
        },
        ShinyType6.Random => Shiny.Random,
        ShinyType6.Never => Shiny.Never,
        ShinyType6.Always => Shiny.Always,
        _ => throw new ArgumentOutOfRangeException(),
    };

    private uint GetShinyXor()
    {
        // Player owned anti-shiny fixed PID
        if (ID32 == 0)
            return uint.MaxValue;

        var xor = PID ^ ID32;
        return (xor >> 16) ^ (xor & 0xFFFF);
    }

    public override uint ID32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x68)); set => WriteUInt32LittleEndian(Data.AsSpan(0x68), value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x68)); set => WriteUInt16LittleEndian(Data.AsSpan(0x68), value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x6A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x6A), value); }
    public byte OriginGame { get => Data[0x6C]; set => Data[0x6C] = value; }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data.AsSpan(0x70)); set => WriteUInt32LittleEndian(Data.AsSpan(0x70), value); }
    public override byte Ball { get => Data[0x76]; set => Data[0x76] = value; }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x78)); set => WriteUInt16LittleEndian(Data.AsSpan(0x78), (ushort)value); }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x7A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x7A), value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x7C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x7C), value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x7E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x7E), value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x80)); set => WriteUInt16LittleEndian(Data.AsSpan(0x80), value); }

    public override ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x82)); set => WriteUInt16LittleEndian(Data.AsSpan(0x82), value); }
    public override byte Form { get => Data[0x84]; set => Data[0x84] = value; }
    public int Language { get => Data[0x85]; set => Data[0x85] = (byte)value; }

    public string Nickname
    {
        get => StringConverter6.GetString(Data.AsSpan(0x86, 0x1A));
        set => StringConverter6.SetString(Data.AsSpan(0x86, 0x1A), value, 12, Language, StringConverterOption.ClearZero);
    }

    public Nature Nature { get => (Nature)Data[0xA0]; set => Data[0xA0] = (byte)value; }
    public override byte Gender { get => Data[0xA1]; set => Data[0xA1] = value; }
    public override int AbilityType { get => Data[0xA2]; set => Data[0xA2] = (byte)value; }
    public ShinyType6 PIDType { get => (ShinyType6)Data[0xA3]; set => Data[0xA3] = (byte)value; }
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0xA4)); set => WriteUInt16LittleEndian(Data.AsSpan(0xA4), value); }
    public override ushort Location { get => ReadUInt16LittleEndian(Data.AsSpan(0xA6)); set => WriteUInt16LittleEndian(Data.AsSpan(0xA6), value); }

    public byte ContestCool   { get => Data[0xA9]; set => Data[0xA9] = value; }
    public byte ContestBeauty { get => Data[0xAA]; set => Data[0xAA] = value; }
    public byte ContestCute   { get => Data[0xAB]; set => Data[0xAB] = value; }
    public byte ContestSmart  { get => Data[0xAC]; set => Data[0xAC] = value; }
    public byte ContestTough  { get => Data[0xAD]; set => Data[0xAD] = value; }
    public byte ContestSheen  { get => Data[0xAE]; set => Data[0xAE] = value; }

    public int IV_HP { get => Data[0xAF]; set => Data[0xAF] = (byte)value; }
    public int IV_ATK { get => Data[0xB0]; set => Data[0xB0] = (byte)value; }
    public int IV_DEF { get => Data[0xB1]; set => Data[0xB1] = (byte)value; }
    public int IV_SPE { get => Data[0xB2]; set => Data[0xB2] = (byte)value; }
    public int IV_SPA { get => Data[0xB3]; set => Data[0xB3] = (byte)value; }
    public int IV_SPD { get => Data[0xB4]; set => Data[0xB4] = (byte)value; }

    public byte OTGender { get => Data[0xB5]; set => Data[0xB5] = value; }

    public override string OriginalTrainerName
    {
        get => StringConverter6.GetString(Data.AsSpan(0xB6, 0x1A));
        set => StringConverter6.SetString(Data.AsSpan(0xB6, 0x1A), value, 12, Language, StringConverterOption.ClearZero);
    }

    public bool IsOriginalTrainerNameSet => Data[0xB6] != 0 || Data[0xB7] != 0;

    public override byte Level { get => Data[0xD0]; set => Data[0xD0] = value; }
    public override bool IsEgg { get => Data[0xD1] == 1; set => Data[0xD1] = value ? (byte)1 : (byte)0; }
    public uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0xD4)); set => WriteUInt32LittleEndian(Data.AsSpan(0xD4), value); }

    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(0xD8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xD8), value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(0xDA)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDA), value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(0xDC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDC), value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(0xDE)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDE), value); }

    public byte OriginalTrainerMemoryIntensity { get => Data[0xE0]; set => Data[0xE0] = value; }
    public byte OriginalTrainerMemory { get => Data[0xE1]; set => Data[0xE1] = value; }
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data.AsSpan(0xE2)); set => WriteUInt16LittleEndian(Data.AsSpan(0xE2), value); }
    public byte OriginalTrainerMemoryFeeling { get => Data[0xE4]; set => Data[0xE4] = value; }

    public int EV_HP { get => Data[0xE5]; set => Data[0xE5] = (byte)value; }
    public int EV_ATK { get => Data[0xE6]; set => Data[0xE6] = (byte)value; }
    public int EV_DEF { get => Data[0xE7]; set => Data[0xE7] = (byte)value; }
    public int EV_SPE { get => Data[0xE8]; set => Data[0xE8] = (byte)value; }
    public int EV_SPA { get => Data[0xE9]; set => Data[0xE9] = (byte)value; }
    public int EV_SPD { get => Data[0xEA]; set => Data[0xEA] = (byte)value; }

    private byte RIB0 { get => Data[0x74]; set => Data[0x74] = value; }
    private byte RIB1 { get => Data[0x75]; set => Data[0x75] = value; }
    public bool RibbonChampionBattle   { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonChampionRegional { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonChampionNational { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonCountry          { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonNational         { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonEarth            { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonWorld            { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonEvent            { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonChampionWorld    { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonBirthday         { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonSpecial          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonSouvenir         { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonWishing          { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonClassic          { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonPremier          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RIB1_7                 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

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

    public bool IsNicknamed => Nickname.Length != 0;

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

    public override PK6 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        var rnd = Util.Rand;
        byte currentLevel = Level > 0 ? Level : (byte)(1 + rnd.Next(100));
        var pi = PersonalTable.AO.GetFormEntry(Species, Form);
        PK6 pk = new()
        {
            Species = Species,
            HeldItem = HeldItem,
            TID16 = TID16,
            SID16 = SID16,
            MetLevel = currentLevel,
            Form = Form,
            EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : rnd.Rand32(),
            Version = OriginGame != 0 ? (GameVersion)OriginGame : tr.Version,
            Language = Language != 0 ? Language : tr.Language,
            Ball = Ball,
            Move1 = Move1, Move2 = Move2, Move3 = Move3, Move4 = Move4,
            RelearnMove1 = RelearnMove1, RelearnMove2 = RelearnMove2,
            RelearnMove3 = RelearnMove3, RelearnMove4 = RelearnMove4,
            MetLocation = Location,
            EggLocation = EggLocation,
            ContestCool = ContestCool,
            ContestBeauty = ContestBeauty,
            ContestCute = ContestCute,
            ContestSmart = ContestSmart,
            ContestTough = ContestTough,
            ContestSheen = ContestSheen,

            OriginalTrainerName = IsOriginalTrainerNameSet ? OriginalTrainerName : tr.OT,
            OriginalTrainerGender = OTGender != 3 ? (byte)(OTGender % 2) : tr.Gender,

            EXP = Experience.GetEXP(Level, pi.EXPGrowth),

            // Ribbons
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,

            RibbonEarth = RibbonEarth,
            RibbonWorld = RibbonWorld,
            RibbonClassic = RibbonClassic,
            RibbonPremier = RibbonPremier,
            RibbonEvent = RibbonEvent,
            RibbonBirthday = RibbonBirthday,
            RibbonSpecial = RibbonSpecial,
            RibbonSouvenir = RibbonSouvenir,

            RibbonWishing = RibbonWishing,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,
            RibbonChampionWorld = RibbonChampionWorld,

            OriginalTrainerFriendship = pi.BaseFriendship,
            OriginalTrainerMemoryIntensity = OriginalTrainerMemoryIntensity,
            OriginalTrainerMemory = OriginalTrainerMemory,
            OriginalTrainerMemoryVariable = OriginalTrainerMemoryVariable,
            OriginalTrainerMemoryFeeling = OriginalTrainerMemoryFeeling,
            FatefulEncounter = FatefulEncounter,

            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPE = EV_SPE,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,
        };

        if (IsOriginalTrainerNameSet)
        {
            pk.HandlingTrainerName = tr.OT;
            pk.HandlingTrainerGender = tr.Gender;
            pk.HandlingTrainerFriendship = pk.OriginalTrainerFriendship;
            pk.CurrentHandler = 1;
        }

        if (tr is IRegionOrigin o)
        {
            pk.Country = o.Country;
            pk.Region = o.Region;
            pk.ConsoleRegion = o.ConsoleRegion;
        }
        else
        {
            pk.SetDefaultRegionOrigins(pk.Language);
        }

        pk.SetMaximumPPCurrent();

        pk.MetDate = Date ?? EncounterDate.GetDate3DS();

        if ((tr.Generation > Generation && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
        {
            // give random valid game
            do { pk.Version = GameVersion.X + (byte)rnd.Next(4); }
            while (!CanBeReceivedByVersion(pk.Version));
        }

        if (!IsEgg)
        {
            if (pk.CurrentHandler == 0) // OT
            {
                pk.OriginalTrainerMemory = 3;
                pk.OriginalTrainerMemoryVariable = 9;
                pk.OriginalTrainerMemoryIntensity = 1;
                pk.OriginalTrainerMemoryFeeling = MemoryContext6.GetRandomFeeling6(pk.OriginalTrainerMemory, 10); // 0-9
            }
            else
            {
                pk.HandlingTrainerMemory = 3;
                pk.HandlingTrainerMemoryVariable = 9;
                pk.HandlingTrainerMemoryIntensity = 1;
                pk.HandlingTrainerMemoryFeeling = MemoryContext6.GetRandomFeeling6(pk.HandlingTrainerMemory, 10); // 0-9
                pk.HandlingTrainerFriendship = pk.OriginalTrainerFriendship;
            }
        }

        pk.IsNicknamed = IsNicknamed;
        pk.Nickname = IsNicknamed ? Nickname : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

        SetPINGA(pk, criteria);

        if (IsEgg)
            SetEggMetData(pk);
        pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        pk.RefreshChecksum();
        return pk;
    }

    private void SetEggMetData(PK6 pk)
    {
        pk.IsEgg = true;
        pk.EggMetDate = Date;
        pk.Nickname = SpeciesName.GetEggName(pk.Language, Generation);
        pk.IsNicknamed = true;
    }

    private void SetPINGA(PK6 pk, EncounterCriteria criteria)
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

    private void SetPID(PK6 pk)
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
                pk.PID = (low ^ pk.TID16 ^ pk.SID16) << 16 | low;
                break;
            case ShinyType6.Never: // Random Nonshiny
                pk.PID = Util.Rand32();
                if (pk.IsShiny) pk.PID ^= 0x10000000;
                break;
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

    private void SetIVs(PK6 pk)
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

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsEgg)
        {
            if (OTGender != 3)
            {
                // Skip ID check if it is an OR/AS Demo Simulated wc6
                if (CardID != 0)
                {
                    if (SID16 != pk.SID16) return false;
                    if (TID16 != pk.TID16) return false;
                }
                if (OTGender != pk.OriginalTrainerGender) return false;
            }
            if (IsOriginalTrainerNameSet && OriginalTrainerName != pk.OriginalTrainerName) return false;
            if (PIDType == ShinyType6.FixedValue && pk.PID != PID) return false;
            if (!Shiny.IsValid(pk)) return false;
            if (OriginGame != 0 && (GameVersion)OriginGame != pk.Version) return false;
            if (EncryptionConstant != 0 && EncryptionConstant != pk.EncryptionConstant) return false;
            if (Language != 0 && Language != pk.Language) return false;
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

            if (pk is { IsEgg: true, Format: not 6 })
                return false;
        }
        else
        {
            if (!IsMatchEggLocation(pk)) return false;
            if (Location != pk.MetLocation) return false;
        }

        if (Level != pk.MetLevel) return false;
        if (Ball != pk.Ball) return false;
        if (OTGender < 3 && OTGender != pk.OriginalTrainerGender) return false;
        if ((sbyte)Nature != -1 && pk.Nature != Nature) return false;
        if (Gender != 3 && Gender != pk.Gender) return false;

        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;

        return true;
    }

    protected override bool IsMatchDeferred(PKM pk)
    {
        switch (CardID)
        {
            case 0184 when Species != pk.Species && pk.OriginalTrainerFriendship != PersonalTable.AO[Species].BaseFriendship: // Eevee Evolutions
            case 0525 when IV_HP == 0xFE: // Diancie was distributed with no IV enforcement & 3IVs
            case 0504 when RibbonClassic != ((IRibbonSetEvent4)pk).RibbonClassic: // Magmar with/without classic
                return true;
        }
        return false;
    }

    public override GameVersion Version => RestrictVersion switch
    {
        1 => GameVersion.X,
        2 => GameVersion.Y,
        3 => GameVersion.XY,
        4 => GameVersion.AS,
        8 => GameVersion.OR,
        12 => GameVersion.ORAS,
        _ => GameVersion.Gen6,
    };

    protected override bool IsMatchPartial(PKM pk)
    {
        if (RestrictLanguage != 0 && RestrictLanguage != pk.Language)
            return true;
        if (!CanBeReceivedByVersion(pk.Version))
        {
            if (!IsEgg || pk.IsEgg)
                return true;
            if (pk.EggLocation != Locations.LinkTrade6)
                return true;
        }
        return false;
    }
}
