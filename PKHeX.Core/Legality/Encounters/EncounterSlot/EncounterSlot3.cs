namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public record EncounterSlot3(EncounterArea3 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : EncounterSlot, IEncounterConvertible<PK3>, IMagnetStatic, INumberedSlot, ISlotRNGType, ILevelRange
{
    public int Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool EggEncounter => false;
    public Ball FixedBall => GetRequiredBall();

    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;

    private Ball GetRequiredBall(Ball fallback = Ball.None) => Locations.IsSafariZoneLocation3(Location) ? Ball.Safari : fallback;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.E[Species].BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            Ball = (byte)GetRequiredBall(Ball.Poke),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
        };

        SetPINGA(pk, criteria);
        SetEncounterMoves(pk);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK3 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);
        if (Species == (int)Core.Species.Unown)
        {
            do
            {
                PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender);
                ability ^= 1; // some nature-forms cannot have a certain PID-ability set, so just flip it as Unown doesn't have dual abilities.
            } while (pk.Form != Form);
        }
        else
        {
            PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender);
        }
    }

    protected virtual void SetEncounterMoves(PKM pk) => EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsDeferredSafari3(pk.Ball == (int)Ball.Safari))
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }
    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);

    private bool IsDeferredSafari3(bool IsSafariBall) => IsSafariBall != Locations.IsSafariZoneLocation3(Location);
    #endregion
}
