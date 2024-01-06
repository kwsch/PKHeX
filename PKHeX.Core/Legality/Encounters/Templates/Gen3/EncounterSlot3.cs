namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
/// </summary>
public record EncounterSlot3(EncounterArea3 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK3>, IMagnetStatic, INumberedSlot, ISlotRNGType, IRandomCorrelation
{
    public int Generation => 3;
    int ILocation.Location => Location;
    public EntityContext Context => EntityContext.Gen3;
    public bool EggEncounter => false;
    public Ball FixedBall => GetRequiredBall();

    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public byte Location => Parent.Location;
    public SlotType Type => Parent.Type;

    private Ball GetRequiredBall(Ball fallback = Ball.None) => Locations.IsSafariZoneLocation3(Location) ? Ball.Safari : fallback;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = Version != GameVersion.RSE ? Version : GameVersion.RSE.Contains(tr.Game) ? (GameVersion)tr.Game : GameVersion.E;
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = pi.BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)version,
            Ball = (byte)GetRequiredBall(Ball.Poke),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        SetEncounterMoves(pk);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        int gender = criteria.GetGender(pi);
        int nature = (int)criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        if (Species == (int)Core.Species.Unown)
        {
            do
            {
                PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.Method_1_Unown);
                ability ^= 1; // some nature-forms cannot have a certain PID-ability set, so just flip it as Unown doesn't have dual abilities.
            } while (pk.Form != Form);
        }
        else
        {
            PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.Method_1);
        }
    }

    protected virtual void SetEncounterMoves(PKM pk) => EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Form != evo.Form && Species is not (int)Core.Species.Burmy)
            return false;

        if (pk.Format == 3)
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

        return true;
    }

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

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (Species != (int)Core.Species.Unown)
            return val is (PIDType.Method_1 or PIDType.Method_2 or PIDType.Method_3 or PIDType.Method_4);
        return val is (PIDType.Method_1_Unown or PIDType.Method_2_Unown or PIDType.Method_3_Unown or PIDType.Method_4_Unown);
    }

    public PIDType GetSuggestedCorrelation() => Species == (int)Core.Species.Unown ? PIDType.Method_1_Unown : PIDType.Method_1;
}
