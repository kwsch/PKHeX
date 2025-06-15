using System;
using static PKHeX.Core.PIDType;
using static PKHeX.Core.CommonEvent3;
using static PKHeX.Core.CommonEvent3Checker;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Event Gift
/// </summary>
public sealed record EncounterGift3 : IEncounterable, IEncounterMatch, IMoveset, IFatefulEncounterReadOnly,
    IRibbonSetEvent3, IRandomCorrelationEvent3, IFixedTrainer, IMetLevel, IGenerateSeed32
{
    public ushort Species { get; }
    public byte Form => 0;
    public byte Level { get; }
    public GameVersion Version { get; }
    public byte MetLevel { get; }
    public bool IsEgg { get; }

    private const ushort UnspecifiedID = ushort.MaxValue;

    /// <summary>
    /// Matched <see cref="PIDIV"/> Type
    /// </summary>
    public required PIDType Method { get; init; }

    public required Moveset Moves { get; init; }
    public required string OriginalTrainerName { get; init; }
    public GiftGender3 OriginalTrainerGender { get; init; }
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

    public EncounterGift3(ushort species, byte level, GameVersion version) : this(species, level, version, false, level) { }

    public EncounterGift3(ushort species, byte level, GameVersion version, bool egg, byte met = 0)
    {
        Species = species;
        Level = level;
        Version = version;
        IsEgg = egg;
        MetLevel = met;
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
        if (TID16 is not UnspecifiedID)
            pk.TID16 = TID16;
        if (SID16 is not UnspecifiedID)
            pk.SID16 = SID16;

        pk.SetMoves(Moves);
        pk.SetMaximumPPCurrent(Moves);

        // Generate PIDIV
        var seed = SetPINGA(pk, criteria, pi);
        bool hatchedEgg = IsEgg && tr.Generation != 3;
        if (hatchedEgg)
        {
            SetForceHatchDetails(pk, tr);
        }
        else
        {
            pk.Language = (int)GetSafeLanguage((LanguageID)tr.Language);
            pk.OriginalTrainerName = !string.IsNullOrWhiteSpace(OriginalTrainerName) ? OriginalTrainerName : tr.OT;
            if (OriginalTrainerGender is not GiftGender3.RandAlgo)
                pk.OriginalTrainerGender = OriginalTrainerGender == GiftGender3.Recipient ? tr.Gender : (byte)GetGender(seed);

            if (IsEgg)
            {
                pk.IsEgg = true; // lang should be set to japanese already
                if (pk.OriginalTrainerTrash[0] == 0xFF)
                    pk.OriginalTrainerName = TrainerName.GameFreakJPN;
            }
        }
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter
        pk.OriginalTrainerFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        pk.RefreshChecksum();
        return pk;
    }

    private uint GetGender(uint seed) => OriginalTrainerGender switch
    {
        GiftGender3.Only0 or GiftGender3.RandD3_0 => 0,
        GiftGender3.Only1 or GiftGender3.RandD3_1 => 1,
        GiftGender3.RandD3 => GetGenderBit0(LCRNG.Next(seed) >> 16),
        GiftGender3.RandS3 => GetGenderBit3(LCRNG.Next(seed) >> 16),
        GiftGender3.RandS7 => GetGenderBit7(LCRNG.Next(seed) >> 16),
        GiftGender3.RandSG15 => GetGenderBit15(LCRNG.Next2(seed) >> 16),
        _ => 0,
    };

    private bool IsGenderSpecificMatch(byte value) => OriginalTrainerGender switch
    {
        GiftGender3.Only0 or GiftGender3.RandD3_0 => value == 0,
        GiftGender3.Only1 or GiftGender3.RandD3_1 => value == 1,
        _ => true, // Algorithmic check later.
    };

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

    private GameVersion GetVersion(ITrainerInfo tr)
    {
        if (Version.IsValidSavedVersion())
            return Version;
        if (Version == GameVersion.Gen3 && tr.Version.IsValidSavedVersion())
            return GameVersion.Gen3.Contains(tr.Version) ? tr.Version : GameVersion.R;
        if (Version == GameVersion.EFL && tr.Version.IsValidSavedVersion())
            return GameVersion.EFL.Contains(tr.Version) ? tr.Version : GameVersion.FR;
        return GetRandomVersion(Version);
    }

    private uint SetPINGA(PK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        if (Method is Channel)
            return SetPINGAChannel(pk, criteria);
        if (ID32 is Wishmkr.TrainerID && criteria.Shiny.IsShiny() && TrySetWishmkrShiny(pk, criteria))
            return 0;

        var gr = pi.Gender;
        uint idXor = pk.TID16 ^ (uint)pk.SID16;
        while (true)
        {
            uint seed = Util.Rand32();
            seed = GetSaneSeed(seed);
            var pid = Shiny switch
            {
                Shiny.Never when Method is BACD_U_AX => GetAntishiny(ref seed, idXor),
                Shiny.Never => GetRegularAntishiny(ref seed, idXor),
                Shiny.Always => GetForceShiny(ref seed, idXor),
                _ when Method is Method_2 => GetMethod2(ref seed),
                _ => GetRegular(ref seed),
            };
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue; // try again
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, gr)))
                continue;

            pk.PID = pid;
            pk.IV32 = ClassicEraRNG.GetSequentialIVs(ref seed);
            pk.RefreshAbility((int)(pk.PID & 1));
            return seed;
        }
    }

    public bool GenerateSeed32(PKM pk, uint seed)
    {
        var pk3 = (PK3)pk;
        if (Method is Channel)
        {
            seed = ChannelJirachi.SkipToPIDIV(seed);
            SetValuesFromSeedChannel(pk3, seed);
            return true;
        }

        uint idXor = pk.TID16 ^ (uint)pk.SID16;
        pk3.PID = Shiny switch
        {
            Shiny.Never when Method is BACD_U_AX => GetAntishiny(ref seed, idXor),
            Shiny.Never => GetRegularAntishiny(ref seed, idXor),
            Shiny.Always => GetForceShiny(ref seed, idXor),
            _ when Method is Method_2 => GetMethod2(ref seed),
            _ => GetRegular(ref seed),
        };
        pk3.IV32 = ClassicEraRNG.GetSequentialIVs(ref seed);
        return true;
    }

    private static bool TrySetWishmkrShiny(PK3 pk, EncounterCriteria criteria)
    {
        bool filterIVs = criteria.IsSpecifiedIVs(2);
        bool filterNature = criteria.IsSpecifiedNature();
        foreach (var s in Wishmkr.All9)
        {
            uint seed = s;
            var pid = GetRegular(ref seed);
            if (filterNature && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue; // try again

            var iv32 = ClassicEraRNG.GetSequentialIVs(ref seed);
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue; // try again
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue; // try again
            pk.PID = pid;
            pk.IV32 = iv32;
        }
        return false;
    }

    private static uint GetMethod2(ref uint seed)
    {
        var a = LCRNG.Next16(ref seed);
        var b = LCRNG.Next16(ref seed);
        var pid = GenerateMethodH.GetPIDRegular(a, b);
        seed = LCRNG.Next(seed); // VBlank
        return pid;
    }

    private static uint SetPINGAChannel(PK3 pk, EncounterCriteria criteria)
    {
        if (criteria.IsSpecifiedIVsAll())
        {
            Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsChannel];
            var count = XDRNG.GetSeedsChannel(seeds, (uint)criteria.IV_HP, (uint)criteria.IV_ATK, (uint)criteria.IV_DEF, (uint)criteria.IV_SPA, (uint)criteria.IV_SPD, (uint)criteria.IV_SPE);
            foreach (var seed in seeds[..count])
            {
                if (!ChannelJirachi.IsPossible(seed))
                    continue;
                SetValuesFromSeedChannel(pk, seed);
                var pid = pk.EncryptionConstant;
                if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                    continue; // try again
                if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny(pk.ID32, pid, 8))
                    continue; // try again
                return seed;
            }
        }

        bool filterIVs = criteria.IsSpecifiedIVs(2);
        while (true)
        {
            uint seed = Util.Rand32();
            seed = ChannelJirachi.SkipToPIDIV(seed);
            SetValuesFromSeedChannel(pk, seed);

            var pid = pk.EncryptionConstant;
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue; // try again
            if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny(pk.ID32, pid, 8))
                continue; // try again
            var iv32 = pk.IV32;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue; // try again
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            return seed;
        }
    }

    public static void SetValuesFromSeedChannel(PK3 pk, uint seed)
    {
        const ushort TID16 = 40122;
        var sid = XDRNG.Next16(ref seed);
        pk.ID32 = (sid << 16) | TID16;

        var pid1 = XDRNG.Next16(ref seed);
        var pid2 = XDRNG.Next16(ref seed);
        var pid = (pid1 << 16) | pid2;
        if ((pid2 > 7 ? 0 : 1) != (pid1 ^ sid ^ TID16))
            pid ^= 0x80000000;
        pk.PID = pid;

        pk.HeldItem = (ushort)((XDRNG.Next16(ref seed) >> 15) + 169u); // 0-Ganlon, 1-Salac
        pk.Version = GameVersion.S + (byte)(XDRNG.Next16(ref seed) >> 15); // 0-Sapphire, 1-Ruby
        pk.OriginalTrainerGender = (byte)(XDRNG.Next16(ref seed) >> 15);

        var iv32 = XDRNG.GetSequentialIV32(seed);
        pk.SetIVs(iv32);
    }

    private uint GetSaneSeed(uint seed) => Method switch
    {
        BACD_RBCD => Math.Clamp(seed, 3, 213), // BCD digit sum
        BACD_T2 when Species is (ushort)Core.Species.Jirachi
            => LCRNG.Next2(seed & 0xFFFF),
        BACD_T2
            => LCRNG.Next2(PCJPFifthAnniversary.GetSeedForResult(Species, Shiny == Shiny.Always, Moves.Contains((ushort)Move.Wish), seed)),
        BACD_T3
            => LCRNG.Next2(PCJPFifthAnniversary.GetSeedForResult(Species, Shiny == Shiny.Always, Moves.Contains((ushort)Move.Wish), seed)),

        BACD_M => MystryMew.GetSeed(seed),
        _ when OriginalTrainerGender is GiftGender3.RandD3_0 => GetRandomRestrictedGenderBit0(seed, 0),
        _ when OriginalTrainerGender is GiftGender3.RandD3_1 => GetRandomRestrictedGenderBit0(seed, 1),
        _ => Method.IsRestricted() ? seed & 0x0000FFFF : seed,
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
        if (language < LanguageID.Korean && language != LanguageID.None)
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
            GameVersion.EFL => GameVersion.E,
            _ => Util.Rand.Next(2) == 0 ? GameVersion.R : GameVersion.S,
        };
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        // Gen3 Version MUST match.
        if (!Version.Contains(pk.Version))
            return false;

        if (pk.IsEgg && !IsEgg)
            return false;

        bool hatchedEgg = IsEgg && !pk.IsEgg;
        if (!hatchedEgg)
        {
            if (SID16 != UnspecifiedID && SID16 != pk.SID16) return false;
            if (TID16 != UnspecifiedID && TID16 != pk.TID16) return false;
            if (!IsGenderSpecificMatch(pk.OriginalTrainerGender)) return false;
            var wcOT = OriginalTrainerName;
            if (!string.IsNullOrEmpty(wcOT) && wcOT != pk.OriginalTrainerName)
                return false;
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

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsEgg && pk.IsEgg && FatefulEncounter != pk.FatefulEncounter)
            return EncounterMatchRating.DeferredErrors;
        return EncounterMatchRating.Match;
    }

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => true; // checked in explicit match

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk) => type == Method ? Match : Mismatch;

    public RandomCorrelationRating IsCompatibleReviseReset(ref PIDIV value, PKM pk)
    {
        var prev = value.Mutated; // if previously revised, use that instead.
        var type = prev is 0 ? value.Type : prev;

        if (type is BACD_EA or BACD_ES && !IsEgg)
            return Mismatch;

        if (OriginalTrainerGender is not (GiftGender3.RandAlgo or GiftGender3.Recipient) && (!IsEgg || pk.IsEgg) && !IsMatchGender(pk, value.OriginSeed))
            return Mismatch;

        bool result = Method switch
        {
            BACD_U => type is BACD,
            BACD_R => IsRestrictedSimple(ref value, type),
            BACD_R_A => IsRestrictedAnti(ref value, type),
            BACD_U_AX =>  IsUnrestrictedAntiX(ref value, type),

            BACD_T2 => IsRestrictedTable2(ref value, type, Species, Moves.Contains((ushort)Move.Wish)),
            BACD_T3  => IsRestrictedTable3(ref value, type, Species, Moves.Contains((ushort)Move.Wish)),
            BACD_RBCD => IsBerryFixShiny(ref value, type),
            BACD_M => IsMystryMew(ref value, type),
            Channel => IsChannelJirachi(ref value, type),
            Method_2 => type is Method_2 or (Method_1 or Method_4), // via PID modulo VBlank abuse
            _ => false,
        };

        if (result)
            return Match;
        return Mismatch;
    }

    private bool IsMatchGender(PKM pk, uint seed)
    {
        var expect = OriginalTrainerGender switch
        {
            GiftGender3.RandD3_0 or GiftGender3.RandD3_1 => GetGenderBit0(LCRNG.Next6(seed) >> 16),
            _ => GetGender(LCRNG.Next4(seed)),
        }; // another implicit advance inside the method
        return pk.OriginalTrainerGender == expect;
    }
}

/// <summary>
/// Trainer Gender for Gift Events in Gen3
/// </summary>
/// <remarks>If random, determined after the PID/IV (and potentially item).</remarks>
public enum GiftGender3 : byte
{
    /// <summary> Match the recipient. Either 0 or 1. </summary>
    Recipient,
    /// <summary> Must be 0. </summary>
    Only0,
    /// <summary> Must be 1. </summary>
    Only1,

    /// <summary> Determined by a separate algorithm and shouldn't be checked via regular logic. </summary>
    RandAlgo,

    /// <summary> Divide by 3. </summary>
    RandD3,
    /// <summary> Shift by 3. </summary>
    RandS3,
    /// <summary> Shift by 7. </summary>
    RandS7,
    /// <summary> Shift by 15, after Item. </summary>
    RandSG15,

    /// <summary> <see cref="RandD3"/>, but MUST be 0. Used for dual-OT events. </summary>
    RandD3_0,
    /// <summary> <see cref="RandD3"/>, but MUST be 1. Used for dual-OT events. </summary>
    RandD3_1,
}
