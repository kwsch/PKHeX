namespace PKHeX.Core;

/// <summary>
/// Generation 1 Static Encounter
/// </summary>
public sealed record EncounterStatic1(ushort Species, byte Level, GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK1>
{
    public byte Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool IsEgg => false;
    public ushort EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort Location => 0;

    private const byte LightBallPikachuCatchRate = 0xA3; // 163 - Light Ball
    public byte Form => 0;

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public bool IsStarterPikachu => Version == GameVersion.YW && Species == (int)Core.Species.Pikachu && Level == 5;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);

    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pi = EncounterUtil.GetPersonal1(Version, Species);
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = LevelMin,
            CatchRate = IsStarterPikachu ? LightBallPikachuCatchRate : pi.CatchRate,
            DV16 = EncounterUtil.GetRandomDVs(Util.Rand),

            OriginalTrainerName = EncounterUtil.GetTrainerName(tr, lang),
            TID16 = tr.TID16,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            Type1 = pi.Type1,
            Type2 = pi.Type2,
        };

        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (Level > evo.LevelMax)
            return false;
        // Encounters with this version have to originate from the Japanese Blue game.
        if (Version == GameVersion.BU && !pk.Japanese)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private static bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format <= 2)
            return true;

        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    private static bool IsMatchLocation(PKM pk)
    {
        // Met Location is not stored in the PK1 format.
        if (pk is ICaughtData2 { CaughtData: not 0 })
            return false;
        return true;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (pk is not PK1 pk1)
            return false;
        return !IsCatchRateValid(pk1.CatchRate);
    }

    private bool IsCatchRateValid(byte rate)
    {
        if (ParseSettings.AllowGen1Tradeback && PK1.IsCatchRateHeldItem(rate))
            return true;

        // Light Ball (Yellow) starter
        if (IsStarterPikachu)
            return rate == LightBallPikachuCatchRate;

        // Encounters can have different Catch Rates (RBG vs Y)
        return GBRestrictions.RateMatchesEncounter(Species, Version, rate);
    }

    #endregion
}
