using System;
using static PKHeX.Core.RibbonIndex;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Mystery Gift Template File
/// </summary>
public sealed class WC8(byte[] Data) : DataMysteryGift(Data), ILangNick, INature, IGigantamax, IDynamaxLevel, IRibbonIndex, IMemoryOT, ILangNicknamedTemplate, IRelearn, IEncounterServerDate, IRestrictVersion,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMark8, ITrashUnderlaySpecies
{
    public WC8() : this(new byte[Size]) { }

    public const int Size = 0x2D0;
    public const int CardStart = 0x0;

    public override byte Generation => 8;
    public override EntityContext Context => EntityContext.Gen8;
    public override bool FatefulEncounter => true;

    public enum GiftType : byte
    {
        None = 0,
        Pokemon = 1,
        Item = 2,
        BP = 3,
        Clothing = 4,
    }

    public byte RestrictVersion { get => Data[0xE]; set => Data[0xE] = value; }

    public bool CanBeReceivedByVersion(GameVersion version) => RestrictVersion switch
    {
        0 when !IsEntity => true, // Whatever, essentially unrestricted for SW/SH receipt. No Entity gifts are 0.
        1 => version is GameVersion.SW,
        2 => version is GameVersion.SH,
        3 => version is GameVersion.SW or GameVersion.SH,
        _ => throw new ArgumentOutOfRangeException(nameof(RestrictVersion), RestrictVersion, null),
    };

    // General Card Properties
    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x8));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x8), (ushort)value);
    }

    public byte CardFlags { get => Data[CardStart + 0x10]; set => Data[CardStart + 0x10] = value; }
    public GiftType CardType { get => (GiftType)Data[CardStart + 0x11]; set => Data[CardStart + 0x11] = (byte)value; }
    public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
    public override bool GiftUsed { get => false; set { }  }

    public int CardTitleIndex
    {
        get => Data[CardStart + 0x15];
        set => Data[CardStart + 0x15] = (byte) value;
    }

    public override string CardTitle
    {
        get => "Mystery Gift"; // TODO: Use text string from CardTitleIndex
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

    public int GetItem(int index) => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x20 + (0x4 * index)));
    public void SetItem(int index, ushort item) => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x20 + (4 * index)), item);
    public int GetQuantity(int index) => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22 + (0x4 * index)));
    public void SetQuantity(int index, ushort quantity) => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22 + (4 * index)), quantity);

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

    private Shiny FixedShinyType() => IsHOMEGift && IsHOMEShinyPossible() ? Shiny.Random : GetShinyXor() switch
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

        var xor = PID ^ ID32;
        return (xor >> 16) ^ (xor & 0xFFFF);
    }

    public override uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(CardStart + 0x20));
        set => WriteUInt32LittleEndian(Data.AsSpan(CardStart + 0x20), value);
    }

    public override ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x20));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x20), value);
    }

    public override ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22), value);
    }

    public int OriginGame
    {
        get => ReadInt32LittleEndian(Data.AsSpan(CardStart + 0x24));
        set => WriteInt32LittleEndian(Data.AsSpan(CardStart + 0x24), value);
    }

    public uint EncryptionConstant
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(CardStart + 0x28));
        set => WriteUInt32LittleEndian(Data.AsSpan(CardStart + 0x28), value);
    }

    public uint PID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(CardStart + 0x2C));
        set => WriteUInt32LittleEndian(Data.AsSpan(CardStart + 0x2C), value);
    }

    // Nicknames, OT Names 0x30 - 0x228
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x228)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x228), value); }
    public override ushort Location { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22A)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22A), value); }

    public override byte Ball
    {
        get => (byte)ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22C));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22C), value);
    }

    public override int HeldItem
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22E));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22E), (ushort)value);
    }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x230)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x230), value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x232)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x232), value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x234)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x234), value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x236)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x236), value); }
    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x238)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x238), value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x23A)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x23A), value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x23C)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x23C), value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x23E)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x23E), value); }

    public override ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x240)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x240), value); }
    public override byte Form { get => Data[CardStart + 0x242]; set => Data[CardStart + 0x242] = value; }
    public override byte Gender { get => Data[CardStart + 0x243]; set => Data[CardStart + 0x243] = value; }
    public override byte Level { get => Data[CardStart + 0x244]; set => Data[CardStart + 0x244] = value; }
    public override bool IsEgg { get => Data[CardStart + 0x245] == 1; set => Data[CardStart + 0x245] = value ? (byte)1 : (byte)0; }
    public Nature Nature { get => (Nature)Data[CardStart + 0x246]; set => Data[CardStart + 0x246] = (byte)value; }
    public override int AbilityType { get => Data[CardStart + 0x247]; set => Data[CardStart + 0x247] = (byte)value; }

    public ShinyType8 PIDType { get => (ShinyType8)Data[CardStart + 0x248]; set => Data[CardStart + 0x248] = (byte)value; }

    public byte MetLevel { get => Data[CardStart + 0x249]; set => Data[CardStart + 0x249] = value; }
    public byte DynamaxLevel { get => Data[CardStart + 0x24A]; set => Data[CardStart + 0x24A] = value; }
    public bool CanGigantamax { get => Data[CardStart + 0x24B] != 0; set => Data[CardStart + 0x24B] = value ? (byte)1 : (byte)0; }

    // Ribbons 0x24C-0x26C
    private const int RibbonBytesOffset = 0x24C;
    private const int RibbonBytesCount = 0x20;
    private const int RibbonByteNone = 0xFF; // signed -1

    private ReadOnlySpan<byte> RibbonSpan => Data.AsSpan(RibbonBytesOffset, RibbonBytesCount);

    public bool HasMarkEncounter8
    {
        get
        {
            foreach (var value in RibbonSpan)
            {
                if (value == RibbonByteNone)
                    return false; // end
                if (((RibbonIndex)value).IsEncounterMark8())
                    return true;
            }

            return false;
        }
    }

    public byte GetRibbonAtIndex(int byteIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)byteIndex, RibbonBytesCount);
        return Data[RibbonBytesOffset + byteIndex];
    }

    public void SetRibbonAtIndex(int byteIndex, byte ribbonIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)byteIndex, RibbonBytesCount);
        Data[RibbonBytesOffset + byteIndex] = ribbonIndex;
    }

    public int IV_HP  { get => Data[CardStart + 0x26C]; set => Data[CardStart + 0x26C] = (byte)value; }
    public int IV_ATK { get => Data[CardStart + 0x26D]; set => Data[CardStart + 0x26D] = (byte)value; }
    public int IV_DEF { get => Data[CardStart + 0x26E]; set => Data[CardStart + 0x26E] = (byte)value; }
    public int IV_SPE { get => Data[CardStart + 0x26F]; set => Data[CardStart + 0x26F] = (byte)value; }
    public int IV_SPA { get => Data[CardStart + 0x270]; set => Data[CardStart + 0x270] = (byte)value; }
    public int IV_SPD { get => Data[CardStart + 0x271]; set => Data[CardStart + 0x271] = (byte)value; }

    public byte OTGender { get => Data[CardStart + 0x272]; set => Data[CardStart + 0x272] = value; }

    public int EV_HP  { get => Data[CardStart + 0x273]; set => Data[CardStart + 0x273] = (byte)value; }
    public int EV_ATK { get => Data[CardStart + 0x274]; set => Data[CardStart + 0x274] = (byte)value; }
    public int EV_DEF { get => Data[CardStart + 0x275]; set => Data[CardStart + 0x275] = (byte)value; }
    public int EV_SPE { get => Data[CardStart + 0x276]; set => Data[CardStart + 0x276] = (byte)value; }
    public int EV_SPA { get => Data[CardStart + 0x277]; set => Data[CardStart + 0x277] = (byte)value; }
    public int EV_SPD { get => Data[CardStart + 0x278]; set => Data[CardStart + 0x278] = (byte)value; }

    public byte OriginalTrainerMemoryIntensity { get => Data[CardStart + 0x279]; set => Data[CardStart + 0x279] = value; }
    public byte OriginalTrainerMemory { get => Data[CardStart + 0x27A]; set => Data[CardStart + 0x27A] = value; }
    public byte OriginalTrainerMemoryFeeling { get => Data[CardStart + 0x27B]; set => Data[CardStart + 0x27B] = value; }
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x27C)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x27C), value); }

    public ushort Checksum => ReadUInt16LittleEndian(Data.AsSpan(0x2CC));

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

    public bool GetIsNicknamed(int language) => ReadUInt16LittleEndian(Data.AsSpan(GetNicknameOffset(language))) != 0;

    public bool CanBeAnyLanguage()
    {
        for (int i = 0; i < 9; i++)
        {
            var ofs = GetLanguageOffset(i);
            var lang = ReadInt16LittleEndian(Data.AsSpan(ofs));
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
            var lang = ReadInt16LittleEndian(Data.AsSpan(ofs));
            if (lang == language)
                return true;
        }
        return GetLanguage(language) == 0;
    }

    public int GetLanguage(int redeemLanguage) => Data[GetLanguageOffset(GetLanguageIndex(redeemLanguage))];
    private static int GetLanguageOffset(int index) => 0x30 + (index * 0x1C) + 0x1A;

    public bool GetHasOT(int language) => ReadUInt16LittleEndian(Data.AsSpan(GetOTOffset(language))) != 0;

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

    private Span<byte> GetNicknameSpan(int language) => Data.AsSpan(GetNicknameOffset(language), 0x1A);
    public string GetNickname(int language) => StringConverter8.GetString(GetNicknameSpan(language));
    public void SetNickname(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetNicknameSpan(language), value, 12, StringConverterOption.ClearZero);

    private Span<byte> GetOTSpan(int language) => Data.AsSpan(GetOTOffset(language), 0x1A);
    public string GetOT(int language) => StringConverter8.GetString(GetOTSpan(language));
    public void SetOT(int language, ReadOnlySpan<char> value) => StringConverter8.SetString(GetOTSpan(language), value, 12, StringConverterOption.ClearZero);

    private static int GetNicknameOffset(int language)
    {
        int index = GetLanguageIndex(language);
        return 0x30 + (index * 0x1C);
    }

    private static int GetOTOffset(int language)
    {
        int index = GetLanguageIndex(language);
        return 0x12C + (index * 0x1C);
    }

    public bool IsHOMEGift => CardID >= 9000;

    public bool CanHandleOT(int language) => !GetHasOT(language);
    public bool IsTrashUnderlaySpecies(PKM pk) => GetIsNicknamed(pk.Language);

    public override GameVersion Version => OriginGame != 0 ? (GameVersion)OriginGame : GameVersion.SWSH;

    public override PK8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        var rnd = Util.Rand;
        byte currentLevel = Level > 0 ? Level : (byte)(1 + rnd.Next(100));
        var metLevel = MetLevel > 0 ? MetLevel : currentLevel;
        var pi = PersonalTable.SWSH.GetFormEntry(Species, Form);
        var version = OriginGame != 0 ? (GameVersion)OriginGame : this.GetCompatibleVersion(tr.Version);
        var language = (int)Core.Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        bool hasOT = GetHasOT(language);

        var pk = new PK8
        {
            EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : Util.Rand32(),
            TID16 = TID16,
            SID16 = SID16,
            Species = Species,
            Form = Form,
            CurrentLevel = currentLevel,
            Ball = Ball != 0 ? Ball : (byte)4, // Default is Pokeball
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
            HandlingTrainerLanguage = (byte)(hasOT ? language : default),
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

            CanGigantamax = CanGigantamax,
            DynamaxLevel = DynamaxLevel,

            MetLocation = Location,
            EggLocation = EggLocation,
        };
        pk.SetMaximumPPCurrent();

        if ((tr.Generation > Generation && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
        {
            // give random valid game
            do { pk.Version = GameVersion.SW + (byte)rnd.Next(2); }
            while (!CanBeReceivedByVersion(pk.Version));
        }

        if (OTGender >= 2)
        {
            pk.TID16 = tr.TID16;
            pk.SID16 = tr.SID16;

            // Initial updates of HOME did not assign a TSV of 0.
            // After enough server updates, HOME can now assign a TSV of 0.
            // They will XOR the PID to ensure the shiny state of gifts is matched.
            // Don't set a Secret ID.
            if (IsHOMEGift)
                pk.ID32 %= 1_000_000;
        }

        // Official code explicitly corrects for Meowstic
        if (pk.Species == (int)Core.Species.Meowstic)
            pk.Form = (byte)(pk.Gender & 1);

        var date = GetSuggestedDate();
        pk.MetDate = date;

        // Prior to 3.0.0, HOME would set the Encryption Constant exactly and not give a random value if it was 0.
        if (IsHOMEGiftOld(date))
            pk.EncryptionConstant = EncryptionConstant;

        var nickname_language = GetLanguage(language);
        pk.Language = nickname_language != 0 ? nickname_language : tr.Language;
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);
        if (GetIsNicknamed(language))
            pk.Nickname = GetNickname(language);

        for (var i = 0; i < RibbonBytesCount; i++)
        {
            var ribbon = GetRibbonAtIndex(i);
            if (ribbon != RibbonByteNone)
                pk.SetRibbon(ribbon);
        }

        SetPINGA(pk, criteria);

        if (IsEgg)
            SetEggMetData(pk);
        pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        if (!IsHOMEGiftOld(date))
        {
            pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
            pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        }

        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    private bool IsHOMEGiftOld(DateOnly date)
    {
        // 2023/05/30 -- every date prior [*,29th] is an old gift.
        const int DayNumberHOME300 = 738669;
        return IsHOMEGift && date.DayNumber < DayNumberHOME300;
    }

    private DateOnly GetSuggestedDate()
    {
        if (!IsDateRestricted)
            return EncounterDate.GetDateSwitch();
        if (EncounterServerDate.WC8GiftsChk.TryGetValue(Checksum, out var range))
            return range.Start;
        if (EncounterServerDate.WC8Gifts.TryGetValue(CardID, out range))
            return range.Start;
        return EncounterDate.GetDateSwitch();
    }

    private void SetEggMetData(PK8 pk)
    {
        pk.IsEgg = true;
        pk.EggMetDate = EncounterDate.GetDateSwitch();
        pk.Nickname = SpeciesName.GetEggName(pk.Language, Generation);
        pk.IsNicknamed = true;
    }

    private void SetPINGA(PK8 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        pk.Nature = pk.StatNature = criteria.GetNature((sbyte)Nature == -1 ? Nature.Random : Nature);
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

    private uint GetPID(PKM tr, ShinyType8 type) => type switch
    {
        ShinyType8.Never        => GetAntishiny(tr), // Random, Never Shiny
        ShinyType8.Random       => Util.Rand32(), // Random, Any
        ShinyType8.AlwaysStar   => (1u ^ (PID & 0xFFFF) ^ tr.TID16 ^ tr.SID16) << 16 | (PID & 0xFFFF), // Fixed, Force Star
        ShinyType8.AlwaysSquare => (0u ^ (PID & 0xFFFF) ^ tr.TID16 ^ tr.SID16) << 16 | (PID & 0xFFFF), // Fixed, Force Square
        ShinyType8.FixedValue   => GetFixedPID(tr),
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private uint GetFixedPID(PKM tr)
    {
        var pid = PID;
        if (pid != 0 && ID32 != 0)
            return pid;

        if (!tr.IsShiny(pid, 8))
            return pid;
        if (IsHOMEGift && !IsHOMEShinyPossible())
            return ForceAntiShiny(pid);
        return pid;
    }

    private static uint ForceAntiShiny(uint pid) => pid ^ 0x1000_0000;

    private static uint GetAntishiny(ITrainerID32 tr)
    {
        var pid = Util.Rand32();
        if (tr.IsShiny(pid, 8))
            return ForceAntiShiny(pid);
        return pid;
    }

    private void SetPID(PK8 pk)
    {
        pk.PID = GetPID(pk, PIDType);
    }

    private void SetIVs(PKM pk)
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
            if (OTGender < 2)
            {
                if (SID16 != pk.SID16) return false;
                if (TID16 != pk.TID16) return false;
                if (OTGender != pk.OriginalTrainerGender) return false;
            }

            if (!CanBeAnyLanguage() && !CanHaveLanguage(pk.Language))
                return false;

            var OT = GetOT(pk.Language); // May not be guaranteed to work.
            if (!string.IsNullOrEmpty(OT) && OT != pk.OriginalTrainerName) return false;
            if (OriginGame != 0 && (GameVersion)OriginGame != pk.Version) return false;
            if (EncryptionConstant != 0)
            {
                if (EncryptionConstant != pk.EncryptionConstant)
                    return false;
            }
            else if (IsHOMEGift)
            {
                // Prior to 3.0.0, HOME would set the Encryption Constant exactly and not give a random value if it was 0.
                if (pk.MetDate is { } x && IsHOMEGiftOld(x))
                {
                    // HOME gifts -- PID and EC are zeroes...
                    if (EncryptionConstant != pk.EncryptionConstant)
                        return false;
                }
                if (IsShiny)
                {
                    if (!pk.IsShiny)
                        return false;
                }
                else // Never or Random (HOME ID specific)
                {
                    if (pk.IsShiny && !IsHOMEShinyPossible())
                        return false;
                }
            }
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
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

            if (pk is { IsEgg: true, Context: EntityContext.Gen8 })
                return false;
        }
        else
        {
            if (!shinyType.IsValid(pk)) return false;
            if (!IsMatchEggLocation(pk)) return false;
            if (Location != pk.MetLocation) return false;
        }

        if (MetLevel != 0 && MetLevel != pk.MetLevel) return false;
        if ((Ball == 0 ? 4 : Ball) != pk.Ball) return false;
        if (OTGender < 2 && OTGender != pk.OriginalTrainerGender) return false;
        if ((sbyte)Nature != -1 && pk.Nature != Nature) return false;
        if (Gender != 3 && Gender != pk.Gender) return false;

        if (pk is PK8 and IGigantamax g && g.CanGigantamax != CanGigantamax && !Gigantamax.CanToggle(pk.Species, pk.Form, Species, Form))
            return false;

        if (pk is PK8 pk8 && pk8.DynamaxLevel < DynamaxLevel)
            return false;

        if (IsHOMEGift)
        {
            // Prior to 3.0.0, HOME would set the Height and Weight exactly and not give a random value if it was 0.
            // However, entering HOME will re-randomize the Height and Weight.
            if (pk.MetDate is { } x && !IsHOMEGiftOld(x))
            {
                // This was later fixed on 6/15/2023 to stop re-rolling old gifts. New gifts would be random.
                // Gifts that were already re-rolled would stay re-rolled.
                if (pk is IScaledSize { HeightScalar: 0, WeightScalar: 0 })
                    return false; // Disallow anything that's not an Old HOME gift from having 0/0.
            }
        }

        // Duplicate card; one with Nickname specified and another without.
        if (CardID == 0122 && pk.IsNicknamed != GetIsNicknamed(pk.Language))
            return false;

        // PID Types 0 and 1 do not use the fixed PID value.
        // Values 2,3 are specific shiny states, and 4 is fixed value.
        // 2,3,4 can change if it is a traded egg to ensure the same shiny state.
        var type = PIDType;
        if (type is ShinyType8.Never or ShinyType8.Random)
            return true;
        if (pk.PID == GetPID(pk, type))
            return true;

        return false;
    }

    private bool IsHOMEShinyPossible()
    {
        // no defined TID16/SID16 and having a fixed PID can cause the player's TID16/SID16 to match the PID's shiny calc.
        // All PIDs are fixed for HOME gifts.
        return ID32 == 0 && PID != 0;
    }

    public bool IsDateRestricted => IsHOMEGift;

    protected override bool IsMatchDeferred(PKM pk) => false;
    protected override bool IsMatchPartial(PKM pk) => false; // no version compatibility checks yet.

    #region Lazy Ribbon Implementation
    public bool RibbonEarth { get => this.GetRibbonIndex(Earth); set => this.SetRibbonIndex(Earth, value); }
    public bool RibbonNational { get => this.GetRibbonIndex(National); set => this.SetRibbonIndex(National, value); }
    public bool RibbonCountry { get => this.GetRibbonIndex(Country); set => this.SetRibbonIndex(Country, value); }
    public bool RibbonChampionBattle { get => this.GetRibbonIndex(ChampionBattle); set => this.SetRibbonIndex(ChampionBattle, value); }
    public bool RibbonChampionRegional { get => this.GetRibbonIndex(ChampionRegional); set => this.SetRibbonIndex(ChampionRegional, value); }
    public bool RibbonChampionNational { get => this.GetRibbonIndex(ChampionNational); set => this.SetRibbonIndex(ChampionNational, value); }
    public bool RibbonClassic { get => this.GetRibbonIndex(Classic); set => this.SetRibbonIndex(Classic, value); }
    public bool RibbonWishing { get => this.GetRibbonIndex(Wishing); set => this.SetRibbonIndex(Wishing, value); }
    public bool RibbonPremier { get => this.GetRibbonIndex(Premier); set => this.SetRibbonIndex(Premier, value); }
    public bool RibbonEvent { get => this.GetRibbonIndex(Event); set => this.SetRibbonIndex(Event, value); }
    public bool RibbonBirthday { get => this.GetRibbonIndex(Birthday); set => this.SetRibbonIndex(Birthday, value); }
    public bool RibbonSpecial { get => this.GetRibbonIndex(Special); set => this.SetRibbonIndex(Special, value); }
    public bool RibbonWorld { get => this.GetRibbonIndex(World); set => this.SetRibbonIndex(World, value); }
    public bool RibbonChampionWorld { get => this.GetRibbonIndex(ChampionWorld); set => this.SetRibbonIndex(ChampionWorld, value); }
    public bool RibbonSouvenir { get => this.GetRibbonIndex(Souvenir); set => this.SetRibbonIndex(Souvenir, value); }
    public bool RibbonChampionG3 { get => this.GetRibbonIndex(ChampionG3); set => this.SetRibbonIndex(ChampionG3, value); }
    public bool RibbonArtist { get => this.GetRibbonIndex(Artist); set => this.SetRibbonIndex(Artist, value); }
    public bool RibbonEffort { get => this.GetRibbonIndex(Effort); set => this.SetRibbonIndex(Effort, value); }
    public bool RibbonChampionSinnoh { get => this.GetRibbonIndex(ChampionSinnoh); set => this.SetRibbonIndex(ChampionSinnoh, value); }
    public bool RibbonAlert { get => this.GetRibbonIndex(Alert); set => this.SetRibbonIndex(Alert, value); }
    public bool RibbonShock { get => this.GetRibbonIndex(Shock); set => this.SetRibbonIndex(Shock, value); }
    public bool RibbonDowncast { get => this.GetRibbonIndex(Downcast); set => this.SetRibbonIndex(Downcast, value); }
    public bool RibbonCareless { get => this.GetRibbonIndex(Careless); set => this.SetRibbonIndex(Careless, value); }
    public bool RibbonRelax { get => this.GetRibbonIndex(Relax); set => this.SetRibbonIndex(Relax, value); }
    public bool RibbonSnooze { get => this.GetRibbonIndex(Snooze); set => this.SetRibbonIndex(Snooze, value); }
    public bool RibbonSmile { get => this.GetRibbonIndex(Smile); set => this.SetRibbonIndex(Smile, value); }
    public bool RibbonGorgeous { get => this.GetRibbonIndex(Gorgeous); set => this.SetRibbonIndex(Gorgeous, value); }
    public bool RibbonRoyal { get => this.GetRibbonIndex(Royal); set => this.SetRibbonIndex(Royal, value); }
    public bool RibbonGorgeousRoyal { get => this.GetRibbonIndex(GorgeousRoyal); set => this.SetRibbonIndex(GorgeousRoyal, value); }
    public bool RibbonFootprint { get => this.GetRibbonIndex(Footprint); set => this.SetRibbonIndex(Footprint, value); }
    public bool RibbonRecord { get => this.GetRibbonIndex(Record); set => this.SetRibbonIndex(Record, value); }
    public bool RibbonLegend { get => this.GetRibbonIndex(Legend); set => this.SetRibbonIndex(Legend, value); }
    public bool RibbonChampionKalos { get => this.GetRibbonIndex(ChampionKalos); set => this.SetRibbonIndex(ChampionKalos, value); }
    public bool RibbonChampionG6Hoenn { get => this.GetRibbonIndex(ChampionG6Hoenn); set => this.SetRibbonIndex(ChampionG6Hoenn, value); }
    public bool RibbonBestFriends { get => this.GetRibbonIndex(BestFriends); set => this.SetRibbonIndex(BestFriends, value); }
    public bool RibbonTraining { get => this.GetRibbonIndex(Training); set => this.SetRibbonIndex(Training, value); }
    public bool RibbonBattlerSkillful { get => this.GetRibbonIndex(BattlerSkillful); set => this.SetRibbonIndex(BattlerSkillful, value); }
    public bool RibbonBattlerExpert { get => this.GetRibbonIndex(BattlerExpert); set => this.SetRibbonIndex(BattlerExpert, value); }
    public bool RibbonContestStar { get => this.GetRibbonIndex(ContestStar); set => this.SetRibbonIndex(ContestStar, value); }
    public bool RibbonMasterCoolness { get => this.GetRibbonIndex(MasterCoolness); set => this.SetRibbonIndex(MasterCoolness, value); }
    public bool RibbonMasterBeauty { get => this.GetRibbonIndex(MasterBeauty); set => this.SetRibbonIndex(MasterBeauty, value); }
    public bool RibbonMasterCuteness { get => this.GetRibbonIndex(MasterCuteness); set => this.SetRibbonIndex(MasterCuteness, value); }
    public bool RibbonMasterCleverness { get => this.GetRibbonIndex(MasterCleverness); set => this.SetRibbonIndex(MasterCleverness, value); }
    public bool RibbonMasterToughness { get => this.GetRibbonIndex(MasterToughness); set => this.SetRibbonIndex(MasterToughness, value); }

    public bool RibbonChampionAlola { get => this.GetRibbonIndex(ChampionAlola); set => this.SetRibbonIndex(ChampionAlola, value); }
    public bool RibbonBattleRoyale { get => this.GetRibbonIndex(BattleRoyale); set => this.SetRibbonIndex(BattleRoyale, value); }
    public bool RibbonBattleTreeGreat { get => this.GetRibbonIndex(BattleTreeGreat); set => this.SetRibbonIndex(BattleTreeGreat, value); }
    public bool RibbonBattleTreeMaster { get => this.GetRibbonIndex(BattleTreeMaster); set => this.SetRibbonIndex(BattleTreeMaster, value); }
    public bool RibbonChampionGalar { get => this.GetRibbonIndex(ChampionGalar); set => this.SetRibbonIndex(ChampionGalar, value); }
    public bool RibbonTowerMaster { get => this.GetRibbonIndex(TowerMaster); set => this.SetRibbonIndex(TowerMaster, value); }
    public bool RibbonMasterRank { get => this.GetRibbonIndex(MasterRank); set => this.SetRibbonIndex(MasterRank, value); }
    public bool RibbonMarkLunchtime { get => this.GetRibbonIndex(MarkLunchtime); set => this.SetRibbonIndex(MarkLunchtime, value); }
    public bool RibbonMarkSleepyTime { get => this.GetRibbonIndex(MarkSleepyTime); set => this.SetRibbonIndex(MarkSleepyTime, value); }
    public bool RibbonMarkDusk { get => this.GetRibbonIndex(MarkDusk); set => this.SetRibbonIndex(MarkDusk, value); }
    public bool RibbonMarkDawn { get => this.GetRibbonIndex(MarkDawn); set => this.SetRibbonIndex(MarkDawn, value); }
    public bool RibbonMarkCloudy { get => this.GetRibbonIndex(MarkCloudy); set => this.SetRibbonIndex(MarkCloudy, value); }
    public bool RibbonMarkRainy { get => this.GetRibbonIndex(MarkRainy); set => this.SetRibbonIndex(MarkRainy, value); }
    public bool RibbonMarkStormy { get => this.GetRibbonIndex(MarkStormy); set => this.SetRibbonIndex(MarkStormy, value); }
    public bool RibbonMarkSnowy { get => this.GetRibbonIndex(MarkSnowy); set => this.SetRibbonIndex(MarkSnowy, value); }
    public bool RibbonMarkBlizzard { get => this.GetRibbonIndex(MarkBlizzard); set => this.SetRibbonIndex(MarkBlizzard, value); }
    public bool RibbonMarkDry { get => this.GetRibbonIndex(MarkDry); set => this.SetRibbonIndex(MarkDry, value); }
    public bool RibbonMarkSandstorm { get => this.GetRibbonIndex(MarkSandstorm); set => this.SetRibbonIndex(MarkSandstorm, value); }
    public bool RibbonMarkMisty { get => this.GetRibbonIndex(MarkMisty); set => this.SetRibbonIndex(MarkMisty, value); }
    public bool RibbonMarkDestiny { get => this.GetRibbonIndex(MarkDestiny); set => this.SetRibbonIndex(MarkDestiny, value); }
    public bool RibbonMarkFishing { get => this.GetRibbonIndex(MarkFishing); set => this.SetRibbonIndex(MarkFishing, value); }
    public bool RibbonMarkCurry { get => this.GetRibbonIndex(MarkCurry); set => this.SetRibbonIndex(MarkCurry, value); }
    public bool RibbonMarkUncommon { get => this.GetRibbonIndex(MarkUncommon); set => this.SetRibbonIndex(MarkUncommon, value); }
    public bool RibbonMarkRare { get => this.GetRibbonIndex(MarkRare); set => this.SetRibbonIndex(MarkRare, value); }
    public bool RibbonMarkRowdy { get => this.GetRibbonIndex(MarkRowdy); set => this.SetRibbonIndex(MarkRowdy, value); }
    public bool RibbonMarkAbsentMinded { get => this.GetRibbonIndex(MarkAbsentMinded); set => this.SetRibbonIndex(MarkAbsentMinded, value); }
    public bool RibbonMarkJittery { get => this.GetRibbonIndex(MarkJittery); set => this.SetRibbonIndex(MarkJittery, value); }
    public bool RibbonMarkExcited { get => this.GetRibbonIndex(MarkExcited); set => this.SetRibbonIndex(MarkExcited, value); }
    public bool RibbonMarkCharismatic { get => this.GetRibbonIndex(MarkCharismatic); set => this.SetRibbonIndex(MarkCharismatic, value); }
    public bool RibbonMarkCalmness { get => this.GetRibbonIndex(MarkCalmness); set => this.SetRibbonIndex(MarkCalmness, value); }
    public bool RibbonMarkIntense { get => this.GetRibbonIndex(MarkIntense); set => this.SetRibbonIndex(MarkIntense, value); }
    public bool RibbonMarkZonedOut { get => this.GetRibbonIndex(MarkZonedOut); set => this.SetRibbonIndex(MarkZonedOut, value); }
    public bool RibbonMarkJoyful { get => this.GetRibbonIndex(MarkJoyful); set => this.SetRibbonIndex(MarkJoyful, value); }
    public bool RibbonMarkAngry { get => this.GetRibbonIndex(MarkAngry); set => this.SetRibbonIndex(MarkAngry, value); }
    public bool RibbonMarkSmiley { get => this.GetRibbonIndex(MarkSmiley); set => this.SetRibbonIndex(MarkSmiley, value); }
    public bool RibbonMarkTeary { get => this.GetRibbonIndex(MarkTeary); set => this.SetRibbonIndex(MarkTeary, value); }
    public bool RibbonMarkUpbeat { get => this.GetRibbonIndex(MarkUpbeat); set => this.SetRibbonIndex(MarkUpbeat, value); }
    public bool RibbonMarkPeeved { get => this.GetRibbonIndex(MarkPeeved); set => this.SetRibbonIndex(MarkPeeved, value); }
    public bool RibbonMarkIntellectual { get => this.GetRibbonIndex(MarkIntellectual); set => this.SetRibbonIndex(MarkIntellectual, value); }
    public bool RibbonMarkFerocious { get => this.GetRibbonIndex(MarkFerocious); set => this.SetRibbonIndex(MarkFerocious, value); }
    public bool RibbonMarkCrafty { get => this.GetRibbonIndex(MarkCrafty); set => this.SetRibbonIndex(MarkCrafty, value); }
    public bool RibbonMarkScowling { get => this.GetRibbonIndex(MarkScowling); set => this.SetRibbonIndex(MarkScowling, value); }
    public bool RibbonMarkKindly { get => this.GetRibbonIndex(MarkKindly); set => this.SetRibbonIndex(MarkKindly, value); }
    public bool RibbonMarkFlustered { get => this.GetRibbonIndex(MarkFlustered); set => this.SetRibbonIndex(MarkFlustered, value); }
    public bool RibbonMarkPumpedUp { get => this.GetRibbonIndex(MarkPumpedUp); set => this.SetRibbonIndex(MarkPumpedUp, value); }
    public bool RibbonMarkZeroEnergy { get => this.GetRibbonIndex(MarkZeroEnergy); set => this.SetRibbonIndex(MarkZeroEnergy, value); }
    public bool RibbonMarkPrideful { get => this.GetRibbonIndex(MarkPrideful); set => this.SetRibbonIndex(MarkPrideful, value); }
    public bool RibbonMarkUnsure { get => this.GetRibbonIndex(MarkUnsure); set => this.SetRibbonIndex(MarkUnsure, value); }
    public bool RibbonMarkHumble { get => this.GetRibbonIndex(MarkHumble); set => this.SetRibbonIndex(MarkHumble, value); }
    public bool RibbonMarkThorny { get => this.GetRibbonIndex(MarkThorny); set => this.SetRibbonIndex(MarkThorny, value); }
    public bool RibbonMarkVigor { get => this.GetRibbonIndex(MarkVigor); set => this.SetRibbonIndex(MarkVigor, value); }
    public bool RibbonMarkSlump { get => this.GetRibbonIndex(MarkSlump); set => this.SetRibbonIndex(MarkSlump, value); }
    public bool RibbonTwinklingStar { get => this.GetRibbonIndex(TwinklingStar); set => this.SetRibbonIndex(TwinklingStar, value); }
    public bool RibbonHisui { get => this.GetRibbonIndex(Hisui); set => this.SetRibbonIndex(Hisui, value); }

    public int GetRibbonByte(int index) => Array.IndexOf(Data, (byte)index, RibbonBytesOffset, RibbonBytesCount);
    public bool GetRibbon(int index) => RibbonSpan.Contains((byte)index);

    public void SetRibbon(int index, bool value = true)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)RibbonIndexExtensions.MAX_G8);
        if (value)
        {
            if (GetRibbon(index))
                return;
            var openIndex = Array.IndexOf(Data, RibbonByteNone, RibbonBytesOffset, RibbonBytesCount);
            ArgumentOutOfRangeException.ThrowIfNegative(openIndex, nameof(openIndex)); // Full?
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
}
