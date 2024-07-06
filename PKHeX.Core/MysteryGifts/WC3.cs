using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Mystery Gift Template File
/// </summary>
/// <remarks>
/// This is fabricated data built to emulate the future generation Mystery Gift objects.
/// Data here is not stored in any save file and cannot be naturally exported.
/// </remarks>
public sealed class WC3
    : IEncounterable, IMoveset, IFatefulEncounterReadOnly, IEncounterMatch, IRibbonSetEvent3, IRandomCorrelation, IFixedTrainer, IMetLevel
{
    public ushort Species { get; }
    public byte Form => 0;
    public byte Level { get; }
    public GameVersion Version { get; }
    public byte MetLevel { get; }
    public bool IsEgg { get; }

    private const ushort UnspecifiedID = ushort.MaxValue;
    private const byte UnspecifiedTrainerGender = 3;

    /// <summary>
    /// Matched <see cref="PIDIV"/> Type
    /// </summary>
    public required PIDType Method { get; init; }

    public required Moveset Moves { get; init; }
    public string OriginalTrainerName { get; init; } = string.Empty;
    public byte OriginalTrainerGender { get; init; } = UnspecifiedTrainerGender;
    public uint ID32 { get => (uint)(SID16 << 16 | TID16); init => (SID16, TID16) = ((ushort)(value >> 16), (ushort)value); }
    public ushort TID16 { get; init; } = UnspecifiedID;
    public ushort SID16 { get; init; } = UnspecifiedID;
    public byte Language { get; init; } // default 0 for eggs
    public Shiny Shiny { get; init; }
    public bool FatefulEncounter { get; init; }
    public bool RibbonNational { get; set; } // pls don't mutate

    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public ushort Location => 255; // Event
    public ushort EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public string Name => "Event Gift";
    public string LongName => Name;

    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool IsShiny => Shiny == Shiny.Always;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public PIDType GetSuggestedCorrelation() => Method;
    public bool IsFixedTrainer => !string.IsNullOrWhiteSpace(OriginalTrainerName);

    // Interface dummies
    public bool RibbonEarth { get => false; set { } }
    public bool RibbonCountry { get => false; set { } }
    public bool RibbonChampionBattle { get => false; set { } }
    public bool RibbonChampionRegional { get => false; set { } }
    public bool RibbonChampionNational { get => false; set { } }

    public WC3(ushort species, byte level, GameVersion version) : this(species, level, version, false, level) { }

    public WC3(ushort species, byte level, GameVersion version, bool egg, byte met = 0)
    {
        Species = species;
        Level = level;
        Version = version;
        IsEgg = egg;
        MetLevel = met;
    }

    public bool IsCompatible(PIDType type, PKM pk)
    {
        if (type == Method)
            return true;

        // forced shiny eggs, when hatched, can lose their detectable correlation.
        if (!IsEgg || pk.IsEgg)
            return false;
        return type is PIDType.BACD_R_S or PIDType.BACD_U_S;
    }

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pi = PersonalTable.RS[Species];
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
            EXP = Experience.GetEXP(Level, pi.EXPGrowth),
        };
        SetMoves(pk);

        bool hatchedEgg = IsEgg && tr.Generation != 3;
        if (hatchedEgg)
        {
            SetForceHatchDetails(pk, tr);
        }
        else
        {
            pk.OriginalTrainerGender = OriginalTrainerGender != 3 ? (byte)(OriginalTrainerGender & 1): tr.Gender;
            pk.ID32 = TID16;
            pk.SID16 = SID16;

            pk.Language = (int)GetSafeLanguage((LanguageID)tr.Language);
            pk.OriginalTrainerName = !string.IsNullOrWhiteSpace(OriginalTrainerName) ? OriginalTrainerName : tr.OT;
            if (IsEgg)
            {
                pk.IsEgg = true; // lang should be set to japanese already
                if (pk.OriginalTrainerTrash[0] == 0xFF)
                    pk.OriginalTrainerName = TrainerName.GameFreakJPN;
            }
        }
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter
        pk.OriginalTrainerFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        // Generate PIDIV
        SetPINGA(pk, criteria);

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
            pk.OriginalTrainerName = TrainerName.ProgramINT;
        }

        pk.OriginalTrainerGender = sav.Gender;
        pk.ID32 = sav.ID32;
        bool frlg = pk.FRLG;
        pk.MetLocation = frlg ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE;
        pk.FatefulEncounter &= frlg; // clear flag for RSE
        pk.MetLevel = 0; // hatched
    }

    private GameVersion GetVersion(ITrainerInfo sav)
    {
        if (Version != GameVersion.Gen3)
            return GetRandomVersion(Version);
        bool gen3 = sav.Version < GameVersion.CXD && GameVersion.Gen3.Contains(sav.Version);
        return gen3 ? sav.Version : GameVersion.R;
    }

    private void SetMoves(PK3 pk)
    {
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

    private LanguageID GetSafeLanguageNotEgg(LanguageID language)
    {
        if (Language != 0)
            return (LanguageID) Language;
        if (language < LanguageID.Korean && language != LanguageID.Hacked)
        {
            if (Language == 0 && language is not LanguageID.Japanese)
                return language;
        }
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
            _ => throw new Exception($"Unknown GameVersion: {version}"),
        };
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
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

        if (Language != 0)
        {
            if (Language != pk.Language) return false;
        }
        else
        {
            if (!IsEgg && pk.Language == (int)LanguageID.Japanese)
                return false;
        }

        if ((byte)FixedBall != pk.Ball) return false;
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

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    private static bool GetIsValidOTMattleHoOh(ReadOnlySpan<char> wc, ReadOnlySpan<char> ot, bool ck3)
    {
        if (ck3) // match original if still ck3, otherwise must be truncated 7char
            return wc.SequenceEqual(ot);
        return ot.Length == 7 && wc.StartsWith(ot, StringComparison.Ordinal);
    }

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => true; // checked in explicit match
}
