using static PKHeX.Core.PIDType;
using static PKHeX.Core.SlotType3;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
/// </summary>
public record EncounterSlot3(EncounterArea3 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK3>, IEncounterSlot3, IRandomCorrelation
{
    public byte Generation => 3;
    ushort ILocation.Location => Location;
    public EntityContext Context => EntityContext.Gen3;
    public bool IsEgg => false;
    public Ball FixedBall => GetRequiredBall();
    public byte AreaRate => Parent.Rate;

    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public byte Location => Parent.Location;
    public SlotType3 Type => Parent.Type;
    public bool IsSafari => Locations.IsSafariZoneLocation3(Location);
    public bool IsSafariHoenn => Locations.IsSafariZoneLocation3RSE(Location);

    private Ball GetRequiredBall(Ball fallback = Ball.None) => IsSafari ? Ball.Safari : fallback;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = Version != GameVersion.RSE ? Version : GameVersion.RSE.Contains(tr.Version) ? tr.Version : GameVersion.E;
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            Ball = (byte)GetRequiredBall(Ball.Poke),

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
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
        if (Species != (int)Core.Species.Unown)
        {
            if (criteria.IsSpecifiedIVs() && this.SetFromIVs(pk, criteria))
                return;
            this.SetRandom(pk, pi, criteria, Util.Rand32());
        }
        else
        {
            if (criteria.IsSpecifiedIVs() && this.SetFromIVsUnown(pk, criteria))
                return;
            this.SetRandomUnown(pk, criteria);
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
            if (!this.IsLevelWithinRange(pk.MetLevel))
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
            return val is (Method_1 or Method_2 or Method_3 or Method_4);
        return val is (Method_1_Unown or Method_2_Unown or Method_3_Unown or Method_4_Unown);
    }

    public PIDType GetSuggestedCorrelation() => Species == (int)Core.Species.Unown ? Method_1_Unown : Method_1;

    public byte PressureLevel => Type != Grass ? LevelMax : Parent.GetPressureMax(Species, LevelMax);
}
