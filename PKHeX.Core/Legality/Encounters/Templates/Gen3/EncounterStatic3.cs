namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterStatic3(ushort Species, byte Level, GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK3>, IFatefulEncounterReadOnly, IFixedGender
{
    public int Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool Roaming { get; init; }
    int ILocation.EggLocation => 0;
    int ILocation.Location => Location;
    public bool IsShiny => false;
    private bool Gift => FixedBall == Ball.Poke;
    public Shiny Shiny { get; init; } = Shiny.Random;

    public AbilityPermission Ability => AbilityPermission.Any12;

    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

    public required byte Location { get; init; }
    public byte Form { get; init; }
    public bool EggEncounter { get; init; }
    public sbyte Gender { get; init; } = -1;
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
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.E[Species].BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)version,
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            Language = lang,
            OT_Name = tr.Language == lang ? tr.OT : lang == 1 ? "ゲーフリ" : "GF",
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

        SetPINGA(pk, criteria);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        SetEncounterMoves(pk);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr)
    {
        // Old Sea Map was only distributed to Japanese games.
        if (Species is (ushort)Core.Species.Mew)
            return 1; // Japanese

        // Deoxys for Emerald was not available for Japanese games.
        var lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        if (lang == 1 && Species is (ushort)Core.Species.Deoxys)
            return 2; // English
        return lang;
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
                PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.Method_1_Unown);
                ability ^= 1; // some nature-forms cannot have a certain PID-ability set, so just flip it as Unown doesn't have dual abilities.
            } while (pk.Form != Form);
        }
        else
        {
            PIDType type = this switch
            {
                { Roaming: true, Version: not GameVersion.E } => PIDType.Method_1_Roamer,
                { Version: GameVersion.COLO or GameVersion.XD } => PIDType.CXD,
                _ => PIDType.Method_1,
            };
            do
            {
                PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, type);
            } while (Shiny == Shiny.Never && pk.IsShiny);
        }
    }

    private void SetEncounterMoves(PKM pk) => EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
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
        if (Gender != -1 && pk.Gender != Gender)
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
    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);

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
        if (IsDeferredWurmple(pk))
            return true;
        if (Gift && pk.Ball != (byte)FixedBall)
            return true;
        return false;
    }
    #endregion
}
