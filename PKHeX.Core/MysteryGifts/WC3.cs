using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Mystery Gift Template File
/// </summary>
/// <remarks>
/// This is fabricated data built to emulate the future generation Mystery Gift objects.
/// Data here is not stored in any save file and cannot be naturally exported.
/// </remarks>
public sealed class WC3(GameVersion Version, bool Fateful = false)
    : MysteryGift, IRibbonSetEvent3, ILangNicknamedTemplate, IRandomCorrelation
{
    public override MysteryGift Clone() => (WC3)MemberwiseClone();

    /// <summary>
    /// Matched <see cref="PIDIV"/> Type
    /// </summary>
    public PIDType Method { get; init; }
    public PIDType GetSuggestedCorrelation() => Method;
    public bool IsCompatible(PIDType type, PKM pk)
    {
        if (type == Method)
            return true;

        // forced shiny eggs, when hatched, can lose their detectable correlation.
        if (!IsEgg || pk.IsEgg)
            return false;
        return type is PIDType.BACD_R_S or PIDType.BACD_U_S;
    }

    private const ushort UnspecifiedID = ushort.MaxValue;

    public override string OriginalTrainerName { get; set; } = string.Empty;
    public byte OriginalTrainerGender { get; init; } = 3;
    public override uint ID32 { get => (uint)(SID16 << 16 | TID16); set => (SID16, TID16) = ((ushort)(value >> 16), (ushort)value); }
    public override ushort TID16 { get; set; } = UnspecifiedID;
    public override ushort SID16 { get; set; } = UnspecifiedID;
    public override ushort Location { get; set; } = 255; // Event
    public override ushort EggLocation { get => 0; set {} }
    public byte Language { get; init; } // default 0 for eggs
    public override ushort Species { get; set; }
    public override bool IsEgg { get; set; }
    public override Moveset Moves { get; set; }
    public override Shiny Shiny { get; init; }
    public override GameVersion Version { get; } = Version;
    public override bool FatefulEncounter { get; } = Fateful; // Obedience Flag

    // Mystery Gift Properties
    public override byte Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    public override byte Level { get; set; }
    public override byte Ball { get; set; } = 4;
    public override bool IsShiny => Shiny == Shiny.Always;
    public override bool HasFixedIVs => false;
    public bool RibbonEarth { get; set; }
    public bool RibbonNational { get; set; }
    public bool RibbonCountry { get; set; }
    public bool RibbonChampionBattle { get; set; }
    public bool RibbonChampionRegional { get; set; }
    public bool RibbonChampionNational { get; set; }

    public string? Nickname { get; init; }

    // Description
    public override string CardTitle { get; set; } = "Generation 3 Event";
    public override string CardHeader => CardTitle;

    // Unused
    public override bool GiftUsed { get; set; }
    public override int CardID { get; set; }
    public override bool IsItem { get; set; }
    public override int ItemID { get; set; }
    public override bool IsEntity { get; set; } = true;
    public override bool Empty => false;
    public override byte Gender { get; set; }
    public override byte Form { get; set; }

    // Synthetic
    private readonly byte? _metLevel;

    public byte MetLevel
    {
        get => _metLevel ?? (IsEgg ? (byte)0 : Level);
        init => _metLevel = value;
    }

    public override AbilityPermission Ability => AbilityPermission.Any12;

    public override PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        PK3 pk = new()
        {
            Species = Species,
            MetLevel = MetLevel,
            MetLocation = Location,
            Ball = 4,

            // Ribbons
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,
            RibbonEarth = RibbonEarth,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,

            FatefulEncounter = FatefulEncounter,
            Version = GetVersion(tr),
        };
        pk.EXP = Experience.GetEXP(Level, pk.PersonalInfo.EXPGrowth);
        SetMoves(pk);

        bool hatchedEgg = IsEgg && tr.Generation != 3;
        if (hatchedEgg)
        {
            SetForceHatchDetails(pk, tr);
        }
        else
        {
            pk.OriginalTrainerGender = OriginalTrainerGender != 3 ? (byte)(OriginalTrainerGender & 1): tr.Gender;
            pk.TID16 = TID16;
            pk.SID16 = SID16;

            pk.Language = (int)GetSafeLanguage((LanguageID)tr.Language);
            pk.OriginalTrainerName = !string.IsNullOrWhiteSpace(OriginalTrainerName) ? OriginalTrainerName : tr.OT;
            if (IsEgg)
            {
                pk.IsEgg = true; // lang should be set to japanese already
                if (pk.OriginalTrainerTrash[0] == 0xFF)
                    pk.OriginalTrainerName = "ゲーフリ";
            }
        }
        pk.Nickname = Nickname ?? SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter

        var pi = pk.PersonalInfo;
        pk.OriginalTrainerFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        // Generate PIDIV
        SetPINGA(pk, criteria);
        pk.HeldItem = 0; // clear, only random for Jirachi (?), no loss

        if (Version == GameVersion.XD)
            pk.FatefulEncounter = true; // pk3 is already converted from xk3

        pk.RefreshChecksum();
        return pk;
    }

    private void SetForceHatchDetails(PK3 pk, ITrainerInfo sav)
    {
        pk.Language = (int)GetSafeLanguageNotEgg((LanguageID)sav.Language);
        pk.OriginalTrainerName = sav.OT;
        // ugly workaround for character table interactions
        if (string.IsNullOrWhiteSpace(pk.OriginalTrainerName))
        {
            pk.Language = (int)LanguageID.English;
            pk.OriginalTrainerName = "PKHeX";
        }

        pk.OriginalTrainerGender = sav.Gender;
        pk.ID32 = sav.ID32;
        pk.MetLocation = pk.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE;
        pk.FatefulEncounter &= pk.FRLG; // clear flag for RSE
        pk.MetLevel = 0; // hatched
    }

    private GameVersion GetVersion(ITrainerInfo sav)
    {
        if (Version != 0)
            return GetRandomVersion(Version);
        bool gen3 = sav.Version <= GameVersion.CXD && GameVersion.Gen3.Contains(sav.Version);
        return gen3 ? sav.Version : GameVersion.R;
    }

    private void SetMoves(PK3 pk)
    {
        if (!Moves.HasMoves) // not completely defined
        {
            var version = pk.Version;
            ILearnSource ls = version switch
            {
                GameVersion.R or GameVersion.S => LearnSource3RS.Instance,
                GameVersion.FR => LearnSource3FR.Instance,
                GameVersion.LG => LearnSource3LG.Instance,
                _ => LearnSource3E.Instance,
            };

            var learn = ls.GetLearnset(Species, Form);
            Span<ushort> moves = stackalloc ushort[4];
            learn.SetEncounterMoves(Level, moves);
            Moves = new(moves[0], moves[1], moves[2], moves[3]);
        }

        pk.SetMoves(Moves);
        pk.SetMaximumPPCurrent(Moves);
    }

    private void SetPINGA(PK3 pk, EncounterCriteria _)
    {
        var seed = Util.Rand32();
        seed = TID16 == 06930 ? MystryMew.GetSeed(seed, Method) : GetSaneSeed(seed);
        PIDGenerator.SetValuesFromSeed(pk, Method, seed);
        pk.RefreshAbility((int)(pk.EncryptionConstant & 1));
    }

    private uint GetSaneSeed(uint seed) => Method switch
    {
        PIDType.BACD_R => seed & 0x0000FFFF, // u16
        PIDType.BACD_R_S => seed & 0x000000FF, // u8
        PIDType.Channel => ChannelJirachi.SkipToPIDIV(seed),
        _ => seed,
    };

    private LanguageID GetSafeLanguage(LanguageID hatchLang)
    {
        if (IsEgg)
            return LanguageID.Japanese;
        return GetSafeLanguageNotEgg(hatchLang);
    }

    private LanguageID GetSafeLanguageNotEgg(LanguageID hatchLang)
    {
        if (Language != 0)
            return (LanguageID) Language;
        if (hatchLang < LanguageID.Korean && hatchLang != LanguageID.Hacked)
            return hatchLang;
        return LanguageID.English; // fallback
    }

    private static GameVersion GetRandomVersion(GameVersion version)
    {
        if (version is <= GameVersion.CXD and > 0) // single game
            return version;

        return version switch
        {
            GameVersion.FRLG => Util.Rand.Next(2) == 0 ? GameVersion.FR : GameVersion.LG,
            GameVersion.RS or GameVersion.RSE => Util.Rand.Next(2) == 0 ? GameVersion.R : GameVersion.S,
            GameVersion.COLO or GameVersion.XD => GameVersion.CXD,
            _ => throw new Exception($"Unknown GameVersion: {version}"),
        };
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        // Gen3 Version MUST match.
        if (Version != 0 && !Version.Contains(pk.Version))
            return false;

        bool hatchedEgg = IsEgg && !pk.IsEgg;
        if (!hatchedEgg)
        {
            if (SID16 != UnspecifiedID && SID16 != pk.SID16) return false;
            if (TID16 != UnspecifiedID && TID16 != pk.TID16) return false;
            if (OriginalTrainerGender < 3 && OriginalTrainerGender != pk.OriginalTrainerGender) return false;
            var wcOT = OriginalTrainerName;
            if (!string.IsNullOrEmpty(wcOT))
            {
                if (wcOT.Length > 7) // Colosseum MATTLE Ho-Oh
                {
                    if (!GetIsValidOTMattleHoOh(wcOT, pk.OriginalTrainerName, pk is CK3))
                        return false;
                }
                else if (wcOT != pk.OriginalTrainerName)
                {
                    return false;
                }
            }
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (Language != 0 && Language != pk.Language) return false;
        if (Ball != pk.Ball) return false;
        if (FatefulEncounter != pk.FatefulEncounter && !IsEgg)
            return false;

        if (pk.Format == 3)
        {
            if (hatchedEgg)
                return true; // defer egg specific checks to later.
            if (MetLevel != pk.MetLevel)
                return false;
            if (Location != pk.MetLocation)
                return false;
        }
        else
        {
            if (pk.IsEgg)
                return false;
            if (Level > pk.MetLevel)
                return false;
            if (pk.EggLocation != LocationEdits.GetNoneLocation(pk.Context))
                return false;
        }
        return true;
    }

    private static bool GetIsValidOTMattleHoOh(ReadOnlySpan<char> wc, ReadOnlySpan<char> ot, bool ck3)
    {
        if (ck3) // match original if still ck3, otherwise must be truncated 7char
            return wc.SequenceEqual(ot);
        return ot.Length == 7 && wc.StartsWith(ot, StringComparison.Ordinal);
    }

    protected override bool IsMatchDeferred(PKM pk) => false;
    protected override bool IsMatchPartial(PKM pk) => false;

    public string GetNickname(int language) => Nickname ?? string.Empty;
    public bool GetIsNicknamed(int language) => Nickname != null;
    public bool CanBeAnyLanguage() => false;
    public bool CanHaveLanguage(int language) => Language == language;
    public bool CanHandleOT(int language) => IsEgg;
}
