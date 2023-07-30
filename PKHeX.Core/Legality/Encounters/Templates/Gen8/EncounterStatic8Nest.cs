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
    public static Func<PKM, T, bool>? VerifyCorrelation { private get; set; }
    public static Action<PKM, T, EncounterCriteria>? GenerateData { private get; set; }

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

    public string Name => "Static Encounter";
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
            HeightScalar = PokeSizeUtil.GetRandomScalar(),
            WeightScalar = PokeSizeUtil.GetRandomScalar(),
        };

        SetPINGA(pk, criteria);

        EncounterUtil1.SetEncounterMoves(pk, version, Level);
        pk.ResetPartyStats();

        return pk;
    }

    protected virtual ushort GetLocation() => Location;

    private void SetPINGA(PK8 pk, EncounterCriteria criteria)
    {
        if (GenerateData != null)
        {
            GenerateData(pk, (T)this, criteria);
            return;
        }

        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(Gender, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        int ability = criteria.GetAbilityFromNumber(Ability);
        PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender);
        criteria.SetRandomIVs(pk);
        pk.StatNature = pk.Nature;
        pk.EncryptionConstant = Util.Rand32();
        if (Species == (int)Core.Species.Toxtricity)
        {
            while (true)
            {
                var result = EvolutionMethod.GetAmpLowKeyResult(pk.Nature);
                if (result == pk.Form)
                    break;
                pk.Nature = Util.Rand.Next(25);
            }

            // Might be originally generated with a Neutral nature, then above logic changes to another.
            // Realign the stat nature to Serious mint.
            if (pk.Nature != pk.StatNature && ((Nature)pk.StatNature).IsNeutral())
                pk.StatNature = (int)Nature.Serious;
        }
        var pid = pk.PID;
        RaidRNG.ForceShinyState(pk, Shiny == Shiny.Always, ref pid);
        pk.PID = pid;
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

        if (VerifyCorrelation != null && !VerifyCorrelation(pk, (T)this))
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
    private static bool IsMatchEggLocation(PKM pk) => pk.Egg_Location == 0;
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

        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        return false;
    }

    #endregion
}
