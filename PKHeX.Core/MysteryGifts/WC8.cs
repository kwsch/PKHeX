using System;
using static PKHeX.Core.RibbonIndex;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Mystery Gift Template File
/// </summary>
public sealed class WC8 : DataMysteryGift, ILangNick, INature, IGigantamax, IDynamaxLevel, IRibbonIndex, IMemoryOT, ILangNicknamedTemplate, IEncounterServerDate,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMark8
{
    public const int Size = 0x2D0;
    public const int CardStart = 0x0;

    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8;

    public enum GiftType : byte
    {
        None = 0,
        Pokemon = 1,
        Item = 2,
        BP = 3,
        Clothing = 4,
    }

    public WC8() : this(new byte[Size]) { }
    public WC8(byte[] data) : base(data) { }

    public byte RestrictVersion { get => Data[0xE]; set => Data[0xE] = value; }

    public bool CanBeReceivedByVersion(int v) => RestrictVersion switch
    {
        0 when !IsEntity => true, // Whatever, essentially unrestricted for SW/SH receipt. No Entity gifts are 0.
        1 => v is (int)GameVersion.SW,
        2 => v is (int)GameVersion.SH,
        3 => v is (int)GameVersion.SW or (int)GameVersion.SH,
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
        ShinyType8.FixedValue => FixedShinyType(DateTime.UtcNow),
        ShinyType8.Random => Shiny.Random,
        ShinyType8.Never => Shiny.Never,
        ShinyType8.AlwaysStar => Shiny.AlwaysStar,
        ShinyType8.AlwaysSquare => Shiny.AlwaysSquare,
        _ => throw new ArgumentOutOfRangeException(),
    };

    private Shiny FixedShinyType(DateTime date) => IsHOMEGift && IsHOMEShinyPossible(date) ? Shiny.Random : GetShinyXor() switch
    {
        0 => Shiny.AlwaysSquare,
        <= 15 => Shiny.AlwaysStar,
        _ => Shiny.Never,
    };

    private int GetShinyXor()
    {
        // Player owned anti-shiny fixed PID
        if (TID == 0 && SID == 0)
            return int.MaxValue;

        var pid = PID;
        var psv = (int)((pid >> 16) ^ (pid & 0xFFFF));
        var tsv = (TID ^ SID);
        return psv ^ tsv;
    }

    public override int TID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x20));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x20), (ushort)value);
    }

    public override int SID {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22), (ushort)value);
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
    public override int EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x228)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x228), (ushort)value); }
    public int MetLocation { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22A)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22A), (ushort)value); }

    public override int Ball
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x22C));
        set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x22C), (ushort)value);
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
    public override int Gender { get => Data[CardStart + 0x243]; set => Data[CardStart + 0x243] = (byte)value; }
    public override byte Level { get => Data[CardStart + 0x244]; set => Data[CardStart + 0x244] = value; }
    public override bool IsEgg { get => Data[CardStart + 0x245] == 1; set => Data[CardStart + 0x245] = value ? (byte)1 : (byte)0; }
    public int Nature { get => (sbyte)Data[CardStart + 0x246]; set => Data[CardStart + 0x246] = (byte)value; }
    public override int AbilityType { get => Data[CardStart + 0x247]; set => Data[CardStart + 0x247] = (byte)value; }

    public ShinyType8 PIDType { get => (ShinyType8)Data[CardStart + 0x248]; set => Data[CardStart + 0x248] = (byte)value; }

    public int MetLevel { get => Data[CardStart + 0x249]; set => Data[CardStart + 0x249] = (byte)value; }
    public byte DynamaxLevel { get => Data[CardStart + 0x24A]; set => Data[CardStart + 0x24A] = value; }
    public bool CanGigantamax { get => Data[CardStart + 0x24B] != 0; set => Data[CardStart + 0x24B] = value ? (byte)1 : (byte)0; }

    // Ribbons 0x24C-0x26C
    private const int RibbonBytesOffset = 0x24C;
    private const int RibbonBytesCount = 0x20;
    private const int RibbonByteNone = 0xFF; // signed -1

    public bool HasMark()
    {
        for (int i = 0; i < RibbonBytesCount; i++)
        {
            var value = Data[RibbonBytesOffset + i];
            if (value == RibbonByteNone)
                return false;
            if ((RibbonIndex)value is >= MarkLunchtime and <= MarkSlump)
                return true;
        }
        return false;
    }

    public byte GetRibbonAtIndex(int byteIndex)
    {
        if ((uint)byteIndex >= RibbonBytesCount)
            throw new ArgumentOutOfRangeException(nameof(byteIndex));
        return Data[RibbonBytesOffset + byteIndex];
    }

    public void SetRibbonAtIndex(int byteIndex, byte ribbonIndex)
    {
        if ((uint)byteIndex >= RibbonBytesCount)
            throw new ArgumentOutOfRangeException(nameof(byteIndex));
        Data[RibbonBytesOffset + byteIndex] = ribbonIndex;
    }

    public int IV_HP  { get => Data[CardStart + 0x26C]; set => Data[CardStart + 0x26C] = (byte)value; }
    public int IV_ATK { get => Data[CardStart + 0x26D]; set => Data[CardStart + 0x26D] = (byte)value; }
    public int IV_DEF { get => Data[CardStart + 0x26E]; set => Data[CardStart + 0x26E] = (byte)value; }
    public int IV_SPE { get => Data[CardStart + 0x26F]; set => Data[CardStart + 0x26F] = (byte)value; }
    public int IV_SPA { get => Data[CardStart + 0x270]; set => Data[CardStart + 0x270] = (byte)value; }
    public int IV_SPD { get => Data[CardStart + 0x271]; set => Data[CardStart + 0x271] = (byte)value; }

    public int OTGender { get => Data[CardStart + 0x272]; set => Data[CardStart + 0x272] = (byte)value; }

    public int EV_HP  { get => Data[CardStart + 0x273]; set => Data[CardStart + 0x273] = (byte)value; }
    public int EV_ATK { get => Data[CardStart + 0x274]; set => Data[CardStart + 0x274] = (byte)value; }
    public int EV_DEF { get => Data[CardStart + 0x275]; set => Data[CardStart + 0x275] = (byte)value; }
    public int EV_SPE { get => Data[CardStart + 0x276]; set => Data[CardStart + 0x276] = (byte)value; }
    public int EV_SPA { get => Data[CardStart + 0x277]; set => Data[CardStart + 0x277] = (byte)value; }
    public int EV_SPD { get => Data[CardStart + 0x278]; set => Data[CardStart + 0x278] = (byte)value; }

    public byte OT_Intensity { get => Data[CardStart + 0x279]; set => Data[CardStart + 0x279] = value; }
    public byte OT_Memory { get => Data[CardStart + 0x27A]; set => Data[CardStart + 0x27A] = value; }
    public byte OT_Feeling { get => Data[CardStart + 0x27B]; set => Data[CardStart + 0x27B] = value; }
    public ushort OT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(CardStart + 0x27C)); set => WriteUInt16LittleEndian(Data.AsSpan(CardStart + 0x27C), value); }

    // Meta Accessible Properties
    public override int[] IVs
    {
        get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
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

    public int[] EVs
    {
        get => new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD };
        set
        {
            if (value.Length != 6) return;
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

    public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }

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

    public override Moveset Relearn
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

    public override string OT_Name
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

    public string GetNickname(int language) => StringConverter8.GetString(Data.AsSpan(GetNicknameOffset(language), 0x1A));
    public void SetNickname(int language, string value) => StringConverter8.SetString(Data.AsSpan(GetNicknameOffset(language), 0x1A), value.AsSpan(), 12, StringConverterOption.ClearZero);

    public string GetOT(int language) => StringConverter8.GetString(Data.AsSpan(GetOTOffset(language), 0x1A));
    public void SetOT(int language, string value) => StringConverter8.SetString(Data.AsSpan(GetOTOffset(language), 0x1A), value.AsSpan(), 12, StringConverterOption.ClearZero);

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

    public override GameVersion Version
    {
        get => OriginGame != 0 ? (GameVersion)OriginGame : GameVersion.SWSH;
        set { }
    }

    public override PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        int currentLevel = Level > 0 ? Level : 1 + Util.Rand.Next(100);
        int metLevel = MetLevel > 0 ? MetLevel : currentLevel;
        var pi = PersonalTable.SWSH.GetFormEntry(Species, Form);
        var language = tr.Language;
        bool hasOT = GetHasOT(language);
        var version = OriginGame != 0 ? OriginGame : (int)this.GetCompatibleVersion((GameVersion)tr.Game);

        var pk = new PK8
        {
            EncryptionConstant = EncryptionConstant != 0 || IsHOMEGift ? EncryptionConstant : Util.Rand32(),
            TID = TID,
            SID = SID,
            Species = Species,
            Form = Form,
            CurrentLevel = currentLevel,
            Ball = Ball != 0 ? Ball : 4, // Default is Pokeball
            Met_Level = metLevel,
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

            OT_Name = hasOT ? GetOT(language) : tr.OT,
            OT_Gender = OTGender < 2 ? OTGender : tr.Gender,
            HT_Name = hasOT ? tr.OT : string.Empty,
            HT_Gender = hasOT ? tr.Gender : 0,
            HT_Language = (byte)(hasOT ? language : 0),
            CurrentHandler = hasOT ? 1 : 0,
            OT_Friendship = pi.BaseFriendship,

            OT_Intensity = OT_Intensity,
            OT_Memory = OT_Memory,
            OT_TextVar = OT_TextVar,
            OT_Feeling = OT_Feeling,
            FatefulEncounter = true,

            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPE = EV_SPE,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,

            CanGigantamax = CanGigantamax,
            DynamaxLevel = DynamaxLevel,

            Met_Location = MetLocation,
            Egg_Location = EggLocation,
        };
        pk.SetMaximumPPCurrent();

        if ((tr.Generation > Generation && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
        {
            // give random valid game
            var rnd = Util.Rand;
            do { pk.Version = (int)GameVersion.SW + rnd.Next(2); }
            while (!CanBeReceivedByVersion(pk.Version));
        }

        if (OTGender >= 2)
        {
            pk.TID = tr.TID;
            pk.SID = tr.SID;

            if (IsHOMEGift)
            {
                pk.TrainerSID7 = 0;
                while (pk.TSV == 0)
                {
                    pk.TrainerID7 = Util.Rand.Next(16, 999_999 + 1);
                }
            }
        }

        // Official code explicitly corrects for Meowstic
        if (pk.Species == (int)Core.Species.Meowstic)
            pk.Form = (byte)(pk.Gender & 1);

        pk.MetDate = IsDateRestricted && EncounterServerDate.WC8Gifts.TryGetValue(CardID, out var dt) ? dt : DateTime.Now;

        var nickname_language = GetLanguage(language);
        pk.Language = nickname_language != 0 ? nickname_language : tr.Language;
        pk.IsNicknamed = GetIsNicknamed(language);
        pk.Nickname = pk.IsNicknamed ? GetNickname(language) : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

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

        if (!IsHOMEGift)
        {
            pk.HeightScalar = PokeSizeUtil.GetRandomScalar();
            pk.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }

        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    private void SetEggMetData(PKM pk)
    {
        pk.IsEgg = true;
        pk.EggMetDate = DateTime.Now;
        pk.Nickname = SpeciesName.GetEggName(pk.Language, Generation);
        pk.IsNicknamed = true;
    }

    private void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = PersonalTable.SWSH.GetFormEntry(Species, Form);
        pk.Nature = (int)criteria.GetNature(Nature == -1 ? Core.Nature.Random : (Nature)Nature);
        pk.StatNature = pk.Nature;
        pk.Gender = criteria.GetGender(Gender, pi);
        var av = GetAbilityIndex(criteria);
        pk.RefreshAbility(av);
        SetPID(pk, pk.MetDate ?? DateTime.UtcNow);
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

    private uint GetPID(ITrainerID tr, ShinyType8 type, DateTime date) => type switch
    {
        ShinyType8.Never        => GetAntishiny(tr), // Random, Never Shiny
        ShinyType8.Random       => Util.Rand32(), // Random, Any
        ShinyType8.AlwaysStar   => (uint) (((tr.TID ^ tr.SID ^ (PID & 0xFFFF) ^ 1) << 16) | (PID & 0xFFFF)), // Fixed, Force Star
        ShinyType8.AlwaysSquare => (uint) (((tr.TID ^ tr.SID ^ (PID & 0xFFFF) ^ 0) << 16) | (PID & 0xFFFF)), // Fixed, Force Square
        ShinyType8.FixedValue   => GetFixedPID(tr, date),
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private uint GetFixedPID(ITrainerID tr, DateTime date)
    {
        var pid = PID;
        if (pid != 0 && !(TID == 0 && SID == 0))
            return pid;

        if (!tr.IsShiny(pid, 8))
            return pid;
        if (IsHOMEGift && !IsHOMEShinyPossible(date))
            return GetAntishinyFixedHOME(tr);
        return pid;
    }

    private static uint GetAntishinyFixedHOME(ITrainerID tr)
    {
        var fid = (uint)(tr.SID << 16) | (uint)tr.TID;
        return fid ^ 0x10u;
    }

    private static uint GetAntishiny(ITrainerID tr)
    {
        var pid = Util.Rand32();
        if (tr.IsShiny(pid, 8))
            return pid ^ 0x1000_0000;
        return pid;
    }

    private void SetPID(PKM pk, DateTime date)
    {
        pk.PID = GetPID(pk, PIDType, date);
    }

    private void SetIVs(PKM pk)
    {
        Span<int> finalIVs = stackalloc int[6];
        GetIVs(finalIVs);
        var ivflag = finalIVs.Find(static iv => (byte)(iv - 0xFC) < 3);
        var rng = Util.Rand;
        if (ivflag == default) // Random IVs
        {
            for (int i = 0; i < finalIVs.Length; i++)
            {
                if (finalIVs[i] > 31)
                    finalIVs[i] = rng.Next(32);
            }
        }
        else // 1/2/3 perfect IVs
        {
            int IVCount = ivflag - 0xFB;
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
                if (SID != pk.SID) return false;
                if (TID != pk.TID) return false;
                if (OTGender != pk.OT_Gender) return false;
            }

            if (!CanBeAnyLanguage() && !CanHaveLanguage(pk.Language))
                return false;

            var OT = GetOT(pk.Language); // May not be guaranteed to work.
            if (!string.IsNullOrEmpty(OT) && OT != pk.OT_Name) return false;
            if (OriginGame != 0 && OriginGame != pk.Version) return false;
            if (EncryptionConstant != 0)
            {
                if (EncryptionConstant != pk.EncryptionConstant)
                    return false;
            }
            else if (IsHOMEGift)// 0
            {
                // HOME gifts -- PID and EC are zeroes...
                if (EncryptionConstant != pk.EncryptionConstant)
                    return false;

                if (pk.TSV == 0) // HOME doesn't assign TSV=0 to accounts.
                    return false;

                if (IsShiny)
                {
                    if (!pk.IsShiny)
                        return false;
                }
                else // Never or Random (HOME ID specific)
                {
                    if (pk.IsShiny && !IsHOMEShinyPossible(pk.MetDate ?? DateTime.UtcNow))
                        return false;
                }
            }
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, pk.Format))
            return false;

        var shinyType = Shiny;
        if (PIDType == ShinyType8.FixedValue)
            shinyType = FixedShinyType(pk.MetDate ?? DateTime.UtcNow);
        if (IsEgg)
        {
            if (EggLocation != pk.Egg_Location) // traded
            {
                if (pk.Egg_Location != Locations.LinkTrade6)
                    return false;
                if (PIDType == ShinyType8.Random && pk.IsShiny && pk.ShinyXor > 1)
                    return false; // shiny traded egg will always have xor0/1.
            }
            if (!shinyType.IsValid(pk))
            {
                return false; // can't be traded away for unshiny
            }

            if (pk.IsEgg && !pk.IsNative)
                return false;
        }
        else
        {
            if (!shinyType.IsValid(pk)) return false;
            if (!IsMatchEggLocation(pk)) return false;
            if (MetLocation != pk.Met_Location) return false;
        }

        if (MetLevel != 0 && MetLevel != pk.Met_Level) return false;
        if ((Ball == 0 ? 4 : Ball) != pk.Ball) return false;
        if (OTGender < 2 && OTGender != pk.OT_Gender) return false;
        if (Nature != -1 && pk.Nature != Nature) return false;
        if (Gender != 3 && Gender != pk.Gender) return false;

        if (pk is PK8 and IGigantamax g && g.CanGigantamax != CanGigantamax && !g.CanToggleGigantamax(pk.Species, pk.Form, Species, Form))
            return false;

        if (pk is PK8 pk8 && pk8.DynamaxLevel < DynamaxLevel)
            return false;

        if (IsHOMEGift && pk is IScaledSize s)
        {
            if (s.HeightScalar != 0)
                return false;
            if (s.WeightScalar != 0)
                return false;
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
        return pk.PID == GetPID(pk, type, pk.MetDate ?? DateTime.UtcNow);
    }

    private bool IsHOMEShinyPossible(DateTime date)
    {
        // no defined TID/SID and having a fixed PID can cause the player's TID/SID to match the PID's shiny calc.
        return TID == 0 && SID == 0 && PID != 0 && (CardID < 9015 && date < new DateTime(2022, 5, 18));
    }

    public bool IsDateRestricted => IsHOMEGift;

    protected override bool IsMatchDeferred(PKM pk) => Species != pk.Species;
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
    public bool RibbonPioneer { get => this.GetRibbonIndex(Pioneer); set => this.SetRibbonIndex(Pioneer, value); }

    public int GetRibbonByte(int index) => Array.IndexOf(Data, (byte)index, RibbonBytesOffset, RibbonBytesCount);
    public bool GetRibbon(int index) => GetRibbonByte(index) >= 0;

    public void SetRibbon(int index, bool value = true)
    {
        if ((uint)index > (uint)MarkSlump)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (value)
        {
            if (GetRibbon(index))
                return;
            var openIndex = Array.IndexOf(Data, RibbonByteNone, RibbonBytesOffset, RibbonBytesCount);
            if (openIndex == -1) // Full?
                throw new ArgumentOutOfRangeException(nameof(index));
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
