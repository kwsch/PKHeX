namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen4"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot4(EncounterArea4 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : EncounterSlot, IEncounterConvertible<PK4>, ILevelRange, IMagnetStatic, INumberedSlot, IGroundTypeTile, ISlotRNGType, IEncounterFormRandom
{
    public int Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    public bool EggEncounter => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => GetRequiredBallValue();
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil1.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;
    public GroundTileAllowed GroundTile => Parent.GroundTile;

    public bool CanUseRadar => !GameVersion.HGSS.Contains(Version) && GroundTile.HasFlag(GroundTileAllowed.Grass);

    private Ball GetRequiredBallValue(Ball fallback = Ball.None)
    {
        if (Type is SlotType.BugContest)
            return Ball.Sport;
        return Locations.IsSafariZoneLocation4(Location) ? Ball.Safari : fallback;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pk = new PK4
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.HGSS[Species].BaseFriendship,

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
        };

        pk.Form = GetWildForm(pk, Form);
        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

        pk.ResetPartyStats();
        return pk;
    }

    private static byte GetWildForm(PK4 pk, byte form)
    {
        if (form == EncounterUtil1.FormRandom) // flagged as totally random
            return (byte)Util.Rand.Next(pk.PersonalInfo.FormCount);
        return form;
    }

    private void SetPINGA(PK4 pk, EncounterCriteria criteria)
    {
        int ctr = 0;
        do
        {
            SetPINGAInner(pk, criteria);
            var pidiv = MethodFinder.Analyze(pk);
            var frames = FrameFinder.GetFrames(pidiv, pk);
            foreach (var frame in frames)
            {
                if (frame.IsSlotCompatibile(this, pk))
                    return;
            }
        } while (ctr++ < 10_000);
    }

    private void SetPINGAInner(PK4 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);
        PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender);
        pk.Gender = gender;
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if ((pk.Ball == (int)Ball.Safari) != Locations.IsSafariZoneLocation4(Location))
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
}
