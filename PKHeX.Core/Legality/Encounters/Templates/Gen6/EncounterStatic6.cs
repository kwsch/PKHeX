namespace PKHeX.Core;

/// <summary>
/// Generation 6 Static Encounter
/// </summary>
public sealed record EncounterStatic6(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK6>, IContestStatsReadOnly, IHatchCycle, IFlawlessIVCount, IFatefulEncounterReadOnly, IFixedGender, IFixedNature, IMoveset, IFixedIVSet
{
    public int Generation => 6;
    public EntityContext Context => EntityContext.Gen6;
    int ILocation.Location => Location;
    int ILocation.EggLocation => EggLocation;
    public bool IsShiny => false;
    public bool EggEncounter => EggLocation != 0;
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

    public byte CNT_Cool   { get; init; }
    public byte CNT_Beauty { get; init; }
    public byte CNT_Cute   { get; init; }
    public byte CNT_Smart  { get; init; }
    public byte CNT_Tough  { get; init; }
    public byte CNT_Sheen  { get; init; }

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
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.AO[Species];
        var pk = new PK6
        {
            EncryptionConstant = Util.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            MetDate = EncounterDate.GetDate3DS(),
            Ball = (byte)(FixedBall is Ball.None ? Ball.Poke : FixedBall),
            FatefulEncounter = FatefulEncounter,
            ID32 = tr.ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = tr.Gender,
            OT_Name = tr.OT,

            OT_Friendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (EggEncounter)
        {
            // Fake as hatched.
            pk.Met_Location = version is GameVersion.X or GameVersion.Y ? Locations.HatchLocation6XY : Locations.HatchLocation6AO;
            pk.Met_Level = EggStateLegality.EggMetLevel;
            pk.Egg_Location = EggLocation;
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
        pk.PID = Util.Rand32();
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
        pk.Nature = (int)criteria.GetNature(Nature);
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
        if (pk.Met_Level != Level)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (Nature != Nature.Random && pk.Nature != (int)Nature)
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
        if (EggEncounter)
            return true;
        var met = pk.Met_Location;
        if (met == Location)
            return true;

        if (Species != (int)Core.Species.Pikachu)
            return false;

        // Cosplay Pikachu is given from multiple locations
        return met is 180 or 186 or 194;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (!EggEncounter)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.Egg_Location == expect;
        }

        var eggloc = pk.Egg_Location;
        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade6;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (0 or Locations.LinkTrade6))
            return false;
        return true;
    }
    #endregion
}
