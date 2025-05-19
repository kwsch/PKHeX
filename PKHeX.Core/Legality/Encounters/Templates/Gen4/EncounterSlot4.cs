using static PKHeX.Core.SlotType4;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="EntityContext.Gen4"/>.
/// </summary>
public sealed record EncounterSlot4(EncounterArea4 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IEncounterSlot4, IGroundTypeTile, IEncounterFormRandom, IRandomCorrelation
{
    public byte Generation => 4;
    ushort ILocation.Location => Location;
    public EntityContext Context => EntityContext.Gen4;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => GetRequiredBallValue();
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType4 Type => Parent.Type;
    public GroundTileAllowed GroundTile => Parent.GroundTile;
    public byte AreaRate => Parent.Rate;

    public bool CanUseRadar => Version >= GameVersion.D // HG/SS are below
                               && GroundTile.HasFlag(GroundTileAllowed.Grass)
                               && !Locations4.IsMarsh(Location);

    private Ball GetRequiredBallValue(Ball fallback = Ball.None)
    {
        if (Type is BugContest)
            return Ball.Sport;
        return Locations4.IsSafariBallRequired(Location) ? Ball.Safari : fallback;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.HGSS[Species];
        var pk = new PK4
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            GroundTile = GroundTile.GetIndex(),
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)GetRequiredBallValue(Ball.Poke),

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
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
        bool hgss = pk.HGSS;
        if (hgss)
        {
            if (!criteria.IsSpecifiedIVsAll() || !this.SetFromIVsK(pk, pi, criteria, out _))
                this.SetRandomK(pk, pi, criteria, Util.Rand32());
        }
        else
        {
            if (!criteria.IsSpecifiedIVsAll() || !this.SetFromIVsJ(pk, pi, criteria, out _))
                this.SetRandomJ(pk, pi, criteria, Util.Rand32());
        }
    }

    /// <summary>
    /// Gets a legal Unown form based on the game version and seed that generated the Method 1 spread.
    /// </summary>
    public static byte GetLegalUnownForm(uint seed, bool hgss)
    {
        // ABCD|E(Item)|F(Form) determination
        if (!hgss)
            return 8; // Always 100% form as 'I' in one of the rooms. Don't need to check rand(1) choice.

        var formSeed = LCRNG.Next6(seed);
        return RuinsOfAlph4.GetEntranceForm(formSeed); // !?
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
            if (!this.IsLevelWithinRange(pk.MetLevel))
            {
                if ((Type is not Grass || pk.MetLevel != PressureLevel) || ParseSettings.Settings.FramePattern.RNGFrameNotFound4 != Severity.Invalid)
                    return false; // Only allow Pressure Slots through if they'll be checked by the later Lead verification.
            }
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
        if (Type is not HoneyTree)
            return false;
        return Species == (int)Core.Species.Munchlax && !Parent.IsMunchlaxTree(pk);
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if ((pk.Ball == (int)Ball.Safari) != Locations4.IsSafariBallRequired(Location))
            return EncounterMatchRating.PartialMatch;
        if ((pk.Ball == (int)Ball.Sport) != (Type == BugContest))
        {
            // Nincada => Shedinja can wipe the ball back to Poke
            if (pk.Species != (int)Core.Species.Shedinja || pk.Ball != (int)Ball.Poke)
                return EncounterMatchRating.PartialMatch;
        }
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;
        if (Species == (int)Core.Species.Unown && !EncounterArea4.IsUnownFormValid(pk, pk.Form, Location is RuinsOfAlph4.Location))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk)
    {
        if (type is PIDType.Method_1)
            return Match;
        // Chain shiny with Poké Radar is only possible in D/P/Pt, in grass.
        // Safari Zone does not allow using the Poké Radar
        if (type is PIDType.ChainShiny)
            return pk.IsShiny && CanUseRadar ? Match : Mismatch;
        if (type is PIDType.CuteCharm)
            return CuteCharm4.IsValid(this, pk) ? Match : Mismatch;
        return Mismatch;
    }

    public PIDType GetSuggestedCorrelation() => PIDType.Method_1;

    public byte PressureLevel => Type != Grass ? LevelMax : Parent.GetPressureMax(Species, LevelMax);
    public bool IsBugContest => Type == BugContest;
    public bool IsSafariHGSS => Locations4.IsSafari(Location);
}
