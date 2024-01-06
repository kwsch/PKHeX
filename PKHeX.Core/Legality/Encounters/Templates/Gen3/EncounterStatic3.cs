namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterStatic3(ushort Species, byte Level, GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK3>, IFatefulEncounterReadOnly, IRandomCorrelation, IMoveset
{
    public int Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool Roaming { get; init; }
    int ILocation.EggLocation => 0;
    int ILocation.Location => Location;
    public bool IsShiny => false;
    private bool Gift => FixedBall == Ball.Poke;
    public Shiny Shiny => Shiny.Random;

    public AbilityPermission Ability => AbilityPermission.Any12;

    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

    public required byte Location { get; init; }
    public byte Form { get; init; }
    public bool EggEncounter { get; init; }
    public Moveset Moves { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = GetTemplateLanguage(tr);
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = pi.BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)version,
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            Language = lang,
            OT_Name = EncounterUtil.GetTrainerName(tr, lang),
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (EggEncounter)
        {
            // Fake as hatched.
            pk.Met_Level = EggStateLegality.EggMetLevel34;
            pk.Met_Location = version is GameVersion.FR or GameVersion.LG
                ? Locations.HatchLocationFRLG
                : Locations.HatchLocationRSE;
        }

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr)
    {
        // Old Sea Map was only distributed to Japanese games.
        if (Species is (ushort)Core.Species.Mew)
            return (int)LanguageID.Japanese;

        // Deoxys for Emerald was not available for Japanese games.
        if (Species is (ushort)Core.Species.Deoxys && tr.Language == 1)
            return (int)LanguageID.English;

        return (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
    }

    private void SetPINGA(PK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        int gender = criteria.GetGender(pi);
        int nature = (int)criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        var type = Roaming && Version != GameVersion.E ? PIDType.Method_1_Roamer : PIDType.Method_1;
        do
        {
            PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, type);
        } while (Shiny == Shiny.Never && pk.IsShiny);
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    private bool IsDeferredSafari3(bool IsSafariBall) => IsSafariBall != Locations.IsSafariZoneLocation3(Location);

    private static bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format == 3)
            return true;

        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.Egg_Location == expect;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 3) // Met Level lost on PK3=>PK4
            return evo.LevelMax >= Level;
        if (!EggEncounter)
            return pk.Met_Level == Level;
        return pk is { Met_Level: EggStateLegality.EggMetLevel34, CurrentLevel: >= 5 }; // met level 0, origin level 5
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format != 3)
            return true; // transfer location verified later

        if (EggEncounter)
            return !pk.IsEgg || pk.Met_Location == Location;

        var met = pk.Met_Location;
        if (!Roaming)
            return Location == met;

        // Route 101-138
        if (Version <= GameVersion.E)
            return met is >= 16 and <= 49;
        // Route 1-25 encounter is possible either in grass or on water
        return met is >= 101 and <= 125;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (IsDeferredSafari3(pk.Ball == (int)Ball.Safari))
            return true;
        if (Gift && pk.Ball != (byte)FixedBall)
            return true;
        if (FatefulEncounter != pk.FatefulEncounter)
            return true;
        return false;
    }
    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        var version = pk.Version;
        if (version is (int)GameVersion.E)
            return val is PIDType.Method_1;
        if (version is (int)GameVersion.FR or(int) GameVersion.LG)
            return Roaming ? IsRoamerPIDIV(val, pk) : val is PIDType.Method_1;
        // RS, roamer glitch && RSBox s/w emulation => method 4 available
        return Roaming ? IsRoamerPIDIV(val, pk) : val is (PIDType.Method_1 or PIDType.Method_4);
    }

    private static bool IsRoamerPIDIV(PIDType val, PKM pk)
    {
        // Roamer PIDIV is always Method 1.
        // M1 is checked before M1R. A M1R PIDIV can also be a M1 PIDIV, so check that collision.
        if (PIDType.Method_1_Roamer == val)
            return true;
        if (PIDType.Method_1 != val)
            return false;

        // only 8 bits are stored instead of 32 -- 5 bits HP, 3 bits for ATK.
        // return pk.IV32 <= 0xFF;
        return pk is { IV_DEF: 0, IV_SPE: 0, IV_SPA: 0, IV_SPD: 0, IV_ATK: <= 7 };
    }

    public PIDType GetSuggestedCorrelation() => PIDType.Method_1;
}
