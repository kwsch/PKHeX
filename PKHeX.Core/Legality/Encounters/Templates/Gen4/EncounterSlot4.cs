namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen4"/>.
/// </summary>
public sealed record EncounterSlot4(EncounterArea4 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IMagnetStatic, INumberedSlot, IGroundTypeTile, ISlotRNGType, IEncounterFormRandom, IRandomCorrelation
{
    public int Generation => 4;
    int ILocation.Location => Location;
    public EntityContext Context => EntityContext.Gen4;
    public bool EggEncounter => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => GetRequiredBallValue();
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType Type => Parent.Type;
    public GroundTileAllowed GroundTile => Parent.GroundTile;

    public bool CanUseRadar => Version is not (GameVersion.HG or GameVersion.SS) && GroundTile.HasFlag(GroundTileAllowed.Grass) && !Locations4.IsSafariZoneLocation(Location);

    private Ball GetRequiredBallValue(Ball fallback = Ball.None)
    {
        if (Type is SlotType.BugContest)
            return Ball.Sport;
        return Locations4.IsSafariZoneLocation(Location) ? Ball.Safari : fallback;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.HGSS[Species];
        var pk = new PK4
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            OT_Friendship = pi.BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            GroundTile = GroundTile.GetIndex(),
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)GetRequiredBallValue(Ball.Poke),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form == EncounterUtil.FormRandom) // flagged as totally random
            return (byte)Util.Rand.Next(PersonalTable.HGSS[Species].FormCount);
        return form;
    }

    private void SetPINGA(PK4 pk, EncounterCriteria criteria, PersonalInfo4 pi)
    {
        int gender = criteria.GetGender(pi);
        int nature = (int)criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        int ctr = 0;
        do
        {
            PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.Method_1);
            var pidiv = MethodFinder.Analyze(pk);
            var frames = FrameFinder.GetFrames(pidiv, pk);
            foreach (var frame in frames)
            {
                if (frame.IsSlotCompatibile(this, pk))
                    return;
            }
        } while (ctr++ < 10_000);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Form != evo.Form && Species is not (int)Core.Species.Burmy)
        {
            // Unown forms are random, not specific form IDs
            if (!IsRandomUnspecificForm)
                return false;
        }

        if (pk.Format == 4)
        {
            // Must match level exactly.
            if (!this.IsLevelWithinRange(pk.Met_Level))
                return false;
        }
        else
        {
            if (evo.LevelMax < LevelMin)
                return false;
        }

        // A/B/C tables, only Munchlax is a 'C' encounter, and A/B are accessible from any tree.
        // C table encounters are only available from 4 trees, which are determined by TID16/SID16 of the save file.
        if (IsInvalidMunchlaxTree(pk))
            return false;

        return true;
    }

    public bool IsInvalidMunchlaxTree(PKM pk)
    {
        if (Type is not SlotType.HoneyTree)
            return false;
        return Species == (int)Core.Species.Munchlax && !Parent.IsMunchlaxTree(pk);
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if ((pk.Ball == (int)Ball.Safari) != Locations4.IsSafariZoneLocation(Location))
            return EncounterMatchRating.PartialMatch;
        if ((pk.Ball == (int)Ball.Sport) != (Type == SlotType.BugContest))
        {
            // Nincada => Shedinja can wipe the ball back to Poke
            if (pk.Species != (int)Core.Species.Shedinja || pk.Ball != (int)Ball.Poke)
                return EncounterMatchRating.PartialMatch;
        }
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }
    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);
    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (val is PIDType.Method_1)
            return true;
        // Chain shiny with Poké Radar is only possible in D/P/Pt, in grass.
        // Safari Zone does not allow using the Poké Radar
        if (val is PIDType.ChainShiny)
            return pk.IsShiny && CanUseRadar;
        if (val is PIDType.CuteCharm)
            return pk.Gender is 0 or 1 && MethodFinder.IsCuteCharm4Valid(this, pk);
        return false;
    }

    public PIDType GetSuggestedCorrelation() => PIDType.Method_1;
}
