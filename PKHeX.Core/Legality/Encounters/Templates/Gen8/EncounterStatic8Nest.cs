using System;
using static PKHeX.Core.Encounters8Nest;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Raid)
/// </summary>
public abstract record EncounterStatic8Nest<T>(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK8>, IMoveset,
        IFlawlessIVCount, IFixedGender, IDynamaxLevelReadOnly, IGigantamaxReadOnly where T : EncounterStatic8Nest<T>
{
    public int Generation => 8;
    public EntityContext Context => EntityContext.Gen8;

    int ILocation.Location => SharedNest;
    private const ushort Location = SharedNest;

    public bool IsShiny => Shiny == Shiny.Always;
    public bool EggEncounter => false;
    int ILocation.EggLocation => 0;
    public Ball FixedBall => Ball.None;

    public ushort Species { get; init; }
    public byte Form { get; init; }
    public virtual byte Level { get; init; }
    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }
    public byte DynamaxLevel { get; init; }
    public Shiny Shiny { get; init; }
    public AbilityPermission Ability { get; init; }
    public sbyte Gender { get; init; } = -1;
    public byte FlawlessIVCount { get; init; }
    public bool CanGigantamax { get; init; }

    public abstract string Name { get; }
    public string LongName => Name;
    public virtual byte LevelMin => Level;
    public virtual byte LevelMax => Level;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pk = new PK8
        {
            Species = Species,
            CurrentLevel = Level,
            Met_Location = GetLocation(),
            Met_Level = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)Ball.Poke,

            ID32 = tr.ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = tr.Gender,
            OT_Name = tr.OT,
            OT_Friendship = PersonalTable.SWSH[Species, Form].BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),

            DynamaxLevel = DynamaxLevel,
            CanGigantamax = CanGigantamax,
        };

        SetPINGA(pk, criteria);

        EncounterUtil1.SetEncounterMoves(pk, version, Level);
        pk.ResetPartyStats();

        return pk;
    }

    protected virtual ushort GetLocation() => Location;

    private void SetPINGA(PK8 pk, EncounterCriteria criteria)
    {
        bool requestShiny = criteria.Shiny.IsShiny();
        bool checkShiny = requestShiny && Shiny != Shiny.Never;
        var pi = PersonalTable.SWSH.GetFormEntry(Species, Form);
        var ratio = pi.Gender;
        var abil = RemapAbilityToParam(Ability);
        Span<int> iv = stackalloc int[6];

        int ctr = 0;
        var rand = new Xoroshiro128Plus(Util.Rand.Rand64());
        while (ctr++ < 100_000)
        {
            var seed = rand.Next();
            ApplyDetailsTo(pk, seed, iv, abil, ratio);

            if (criteria.IV_ATK != 31 && pk.IV_ATK != criteria.IV_ATK)
                continue;
            if (criteria.IV_SPE != 31 && pk.IV_SPE != criteria.IV_SPE)
                continue;
            if (checkShiny && pk.IsShiny != requestShiny)
                continue;
            break;
        }

        if ((byte)criteria.Nature != pk.Nature && criteria.Nature.IsMint())
            pk.StatNature = (byte)criteria.Nature;
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;

        // Required Ability
        if (Ability == OnlyHidden && pk.AbilityNumber != 4)
            return false; // H

        if (Version != GameVersion.SWSH && pk.Version != (int)Version && pk.Met_Location != SharedNest)
            return false;

        if (pk is IRibbonSetMark8 { HasMarkEncounter8: true })
            return false;
        if (pk.Species == (int)Core.Species.Shedinja && pk is IRibbonSetAffixed { AffixedRibbon: >= (int)RibbonIndex.MarkLunchtime })
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

    protected virtual bool IsMatchLocation(PKM pk) => Location == pk.Met_Location;
    private static bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.Egg_Location == expect;
    }

    protected virtual bool IsMatchLevel(PKM pk) => pk.Met_Level == Level;
    private bool IsMatchGender(PKM pk) => Gender == -1 || Gender == pk.Gender;
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
            }
        }

        return EncounterMatchRating.Match;
    }

    protected bool IsMatchPartial(PKM pk)
    {
        if (pk is PK8 and IGigantamax g && g.CanGigantamax != CanGigantamax && !g.CanToggleGigantamax(pk.Species, pk.Form, Species, Form))
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
    public bool Verify(PKM pk, ulong seed, bool forceNoShiny = false)
    {
        var pi = PersonalTable.SWSH.GetFormEntry(Species, Form);
        var ratio = pi.Gender;
        var abil = RemapAbilityToParam(Ability);

        Span<int> iv = stackalloc int[6];
        LoadIVs(iv);
        return RaidRNG.Verify(pk, seed, iv, Species, FlawlessIVCount, abil, ratio, forceNoShiny: forceNoShiny);
    }

    private void ApplyDetailsTo(PK8 pk, ulong seed, Span<int> iv, byte abil, byte ratio)
    {
        LoadIVs(iv);
        RaidRNG.ApplyDetailsTo(pk, seed, iv, Species, FlawlessIVCount, abil, ratio);
    }

    private void LoadIVs(Span<int> span)
    {
        // Template stores with speed in middle (standard), convert for generator purpose.
        var ivs = IVs;
        if (ivs.IsSpecified)
            ivs.CopyToSpeedLast(span);
        else
            span.Fill(-1);
    }

    private static byte RemapAbilityToParam(AbilityPermission a) => a switch
    {
        Any12H => 254,
        Any12 => 255,
        _ => a.GetSingleValue(),
    };

    private bool IsMatchCorrelation(PKM pk)
    {
        if (pk.IsShiny)
            return true;

        var ec = pk.EncryptionConstant;
        var pid = pk.PID;
        var seeds = new XoroMachineSkip(ec, pid);
        foreach (var seed in seeds)
        {
            if (IsMatchSeed(pk, seed))
                return true;
        }
        seeds = new XoroMachineSkip(ec, pid ^ 0x1000_0000);
        foreach (var seed in seeds)
        {
            if (IsMatchSeed(pk, seed))
                return true;
        }
        return false;
    }

    protected virtual bool IsMatchSeed(PKM pk, ulong seed) => Verify(pk, seed);

    #endregion
}
