namespace PKHeX.Core;

/// <summary>
/// Generation 6 Static Encounter
/// </summary>
public sealed record EncounterStatic6(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK6>, IContestStatsReadOnly, IHatchCycle, IFlawlessIVCount, IFatefulEncounterReadOnly, IFixedGender, IFixedNature, IMoveset, IFixedIVSet
{
    public byte Generation => 6;
    public EntityContext Context => EntityContext.Gen6;
    ushort ILocation.Location => Location;
    ushort ILocation.EggLocation => EggLocation;
    public bool IsShiny => false;
    public bool IsEgg => EggLocation != 0;
    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required ushort Location { get; init; }
    public AbilityPermission Ability { get; init; }
    public byte Form { get; init; }
    public Shiny Shiny { get; init; }
    public ushort EggLocation { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;

    public byte ContestCool   { get; init; }
    public byte ContestBeauty { get; init; }
    public byte ContestCute   { get; init; }
    public byte ContestSmart  { get; init; }
    public byte ContestTough  { get; init; }
    public byte ContestSheen  { get; init; }

    public byte EggCycles { get; init; }
    public byte FlawlessIVCount { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public Moveset Moves { get; init; }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK6 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK6 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.AO[Species];
        var rnd = Util.Rand;
        var pk = new PK6
        {
            EncryptionConstant = rnd.Rand32(),
            PID = rnd.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDate3DS(),
            Ball = (byte)(FixedBall is Ball.None ? Ball.Poke : FixedBall),
            FatefulEncounter = FatefulEncounter,
            ID32 = tr.ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerName = tr.OT,

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (IsEgg)
        {
            // Fake as hatched.
            pk.MetLocation = version is GameVersion.X or GameVersion.Y ? Locations.HatchLocation6XY : Locations.HatchLocation6AO;
            pk.MetLevel = EggStateLegality.EggMetLevel;
            pk.EggLocation = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins(lang);

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);
        this.CopyContestStatsTo(pk);

        pk.SetRandomMemory6();
        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK6 pk, EncounterCriteria criteria, PersonalInfo6AO pi)
    {
        if (pk.IsShiny)
        {
            if (Shiny == Shiny.Never || (Shiny != Shiny.Always && !criteria.Shiny.IsShiny()))
                pk.PID ^= 0x1000_0000;
        }
        else if (Shiny == Shiny.Always || (Shiny != Shiny.Never && criteria.Shiny.IsShiny()))
        {
            var low = pk.PID & 0xFFFF;
            pk.PID = ((low ^ pk.TID16 ^ pk.SID16) << 16) | low;
        }

        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk, FlawlessIVCount);

        var ability = criteria.GetAbilityFromNumber(Ability);
        pk.Nature = criteria.GetNature(Nature);
        pk.Gender = criteria.GetGender(Gender, pi);
        pk.AbilityNumber = 1 << ability;
        pk.Ability = pi.GetAbilityAtIndex(ability);
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (pk.MetLevel != Level)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        return true;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (IsEgg)
            return true;
        var met = pk.MetLocation;
        if (met == Location)
            return true;

        if (Species != (int)Core.Species.Pikachu)
            return false;

        // Cosplay Pikachu is given from multiple locations
        return met is 180 or 186 or 194;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (!IsEgg)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.EggLocation == expect;
        }

        var eggLoc = pk.EggLocation;
        if (!pk.IsEgg) // hatched
            return eggLoc == EggLocation || eggLoc == Locations.LinkTrade6;

        // Unhatched:
        if (eggLoc != EggLocation)
            return false;
        if (pk.MetLocation is not (0 or Locations.LinkTrade6))
            return false;
        return true;
    }
    #endregion
}
