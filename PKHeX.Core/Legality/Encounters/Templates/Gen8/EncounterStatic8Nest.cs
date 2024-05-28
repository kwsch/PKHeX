using System;
using static PKHeX.Core.Encounters8Nest;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Raid)
/// </summary>
public abstract record EncounterStatic8Nest<T>(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK8>, IMoveset, ISeedCorrelation64<PKM>,
        IFlawlessIVCount, IFixedIVSet, IFixedGender, IDynamaxLevelReadOnly, IGigantamaxReadOnly where T : EncounterStatic8Nest<T>
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8;

    ushort ILocation.Location => SharedNest;
    private const ushort Location = SharedNest;

    public bool IsShiny => Shiny == Shiny.Always;
    public bool IsEgg => false;
    ushort ILocation.EggLocation => 0;
    public Ball FixedBall => Ball.None;

    public ushort Species { get; init; }
    public byte Form { get; init; }
    public virtual byte Level { get; init; }
    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }
    public byte DynamaxLevel { get; init; }
    public Shiny Shiny { get; init; }
    public AbilityPermission Ability { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public byte FlawlessIVCount { get; init; }
    public bool CanGigantamax { get; init; }

    public abstract string Name { get; }
    public string LongName => Name;
    public virtual byte LevelMin => Level;
    public virtual byte LevelMax => Level;

    private PersonalInfo8SWSH Info => PersonalTable.SWSH[Species, Form];

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    protected virtual void SetTrainerName(ReadOnlySpan<char> name, PK8 pk) =>
        pk.SetString(pk.OriginalTrainerTrash, name, name.Length, StringConverterOption.None);

    public PK8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = Info;
        var pk = new PK8
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = GetLocation(),
            MetLevel = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)Ball.Poke,

            ID32 = tr.ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),

            DynamaxLevel = DynamaxLevel,
            CanGigantamax = CanGigantamax,
        };
        SetTrainerName(tr.OT, pk);

        SetPINGA(pk, criteria, pi);

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, Level);
        pk.ResetPartyStats();

        return pk;
    }

    protected virtual ushort GetLocation() => Location;

    private void SetPINGA(PK8 pk, EncounterCriteria criteria, PersonalInfo8SWSH pi)
    {
        bool requestShiny = criteria.Shiny.IsShiny();
        bool checkShiny = requestShiny && Shiny != Shiny.Never;
        Span<int> iv = stackalloc int[6];

        int ctr = 0;
        var rand = new Xoroshiro128Plus(Util.Rand.Rand64());
        var param = GetParam(pi);
        ulong seed;
        const int max = 100_000;
        do
        {
            if (!TryApply(pk, seed = rand.Next(), iv, param, criteria))
                continue;
            if (checkShiny && pk.IsShiny != requestShiny)
                continue;
            break;
        } while (++ctr < max);

        if (ctr == max) // fail
            while (!TryApply(pk, seed = rand.Next(), iv, param, EncounterCriteria.Unrestricted)) { }

        FinishCorrelation(pk, seed);
        if (criteria.IsSpecifiedNature() && criteria.Nature != pk.Nature && criteria.Nature.IsMint())
            pk.StatNature = criteria.Nature;
    }

    private GenerateParam8 GetParam(PersonalInfo8SWSH pi)
    {
        var ratio = RemapGenderToParam(Gender, pi);
        return new GenerateParam8(Species, ratio, FlawlessIVCount, Ability, Shiny, Nature.Random, IVs);
    }

    protected virtual void FinishCorrelation(PK8 pk, ulong seed) { }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;

        if (Version != GameVersion.SWSH && pk.Version != Version && pk.MetLocation != SharedNest)
            return false;

        if (pk is IRibbonSetMark8 { HasMarkEncounter8: true })
            return false;
        if (pk.Species == (int)Core.Species.Shedinja && pk is IRibbonSetAffixed x && ((RibbonIndex)x.AffixedRibbon).IsEncounterMark8())
            return false;

        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk))
            return false;
        if (!IsMatchGender(pk))
            return false;
        if (!IsMatchForm(pk, evo))
            return false;
        if (!IsMatchIVs(pk))
            return false;

        if (pk.FlawlessIVCount < FlawlessIVCount)
            return false;

        return true;
    }

    protected virtual bool IsMatchLocation(PKM pk) => Location == pk.MetLocation;
    private static bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    protected virtual bool IsMatchLevel(PKM pk) => pk.MetLevel == Level;
    private bool IsMatchGender(PKM pk) => Gender == FixedGenderUtil.GenderRandom || Gender == pk.Gender;
    private bool IsMatchForm(PKM pk, EvoCriteria evo) => Form == evo.Form || FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context);
    private bool IsMatchIVs(PKM pk)
    {
        if (!IVs.IsSpecified)
            return true; // nothing to check, IVs are random
        return Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk);
    }

    public virtual EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return IsMatchDeferred(pk);
    }

    private EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Ability != Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not OnlyHidden && !AbilityVerifier.CanAbilityPatch(8, PersonalTable.SWSH.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                if (Ability is OnlyFirst or OnlySecond && !AbilityVerifier.CanAbilityCapsule(8, PersonalTable.SWSH.GetFormEntry(Species, Form)))
                    return EncounterMatchRating.DeferredErrors;
                if (Ability is OnlyHidden) // Can't revert to hidden ability even if transferred from HOME and another game with HA reversion.
                    return EncounterMatchRating.DeferredErrors;
            }
        }

        return EncounterMatchRating.Match;
    }

    protected bool IsMatchPartial(PKM pk)
    {
        if (pk is PK8 and IGigantamax g && g.CanGigantamax != CanGigantamax && !Gigantamax.CanToggle(pk.Species, pk.Form, Species, Form))
            return true;
        if (Species == (int)Core.Species.Alcremie && pk is IFormArgument { FormArgument: not 0 })
            return true;
        if (Species == (int)Core.Species.Runerigus && pk is IFormArgument { FormArgument: not 0 })
            return true;

        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;

        if (!IsMatchCorrelation(pk))
            return true;

        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        return false;
    }

    #endregion

    #region RNG Matching
    /// <summary>
    /// Checks if the raid seed is valid for the given criteria.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="seed">Seed that generated the entity</param>
    /// <param name="forceNoShiny">Down-level specific override to force no shiny via special handling.</param>
    /// <returns>True if the seed is valid for the criteria.</returns>
    public bool Verify(PKM pk, ulong seed, bool forceNoShiny = false)
    {
        var param = GetParam(PersonalTable.SWSH.GetFormEntry(Species, Form));
        Span<int> iv = stackalloc int[6];
        return RaidRNG.Verify(pk, seed, iv, param, forceNoShiny: forceNoShiny);
    }

    protected virtual bool TryApply(PK8 pk, ulong seed, Span<int> iv, GenerateParam8 param, EncounterCriteria criteria)
    {
        return RaidRNG.TryApply(pk, seed, iv, param, criteria);
    }

    private static byte RemapGenderToParam(byte gender, PersonalInfo8SWSH pi) => gender switch
    {
        0 => PersonalInfo.RatioMagicMale,
        1 => PersonalInfo.RatioMagicFemale,
        2 => PersonalInfo.RatioMagicGenderless,
        _ => pi.Gender,
    };

    private bool IsMatchCorrelation(PKM pk)
    {
        if (pk.IsShiny)
            return true;

        return TryGetSeed(pk, out _);
    }

    public bool TryGetSeed(PKM pk, out ulong seed)
    {
        var ec = pk.EncryptionConstant;
        var pid = pk.PID;
        var seeds = new XoroMachineSkip(ec, pid);
        foreach (var s in seeds)
        {
            if (IsMatchSeed(pk, seed = s))
                return true;
        }
        seeds = new XoroMachineSkip(ec, pid ^ 0x1000_0000);
        foreach (var s in seeds)
        {
            if (IsMatchSeed(pk, seed = s))
                return true;
        }
        seed = 0;
        return false;
    }

    protected virtual bool IsMatchSeed(PKM pk, ulong seed) => Verify(pk, seed);

    #endregion
}
