using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 1
/// </summary>
public sealed record EncounterGift1(ushort Species, byte Level, GameVersion Version = GameVersion.RB)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK1>, IFixedGBLanguage, IMoveset, IFixedIVSet
{
    public int Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool EggEncounter => false;
    public int EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public bool IsShiny => false;
    public int Location => 0;

    public const ushort UnspecifiedID = 0;

    public Shiny Shiny { get; init; } = Shiny.Random;
    public byte Form => 0;

    public EncounterGBLanguage Language { get; init; } = EncounterGBLanguage.Japanese;

    /// <summary> Trainer name for the event. </summary>
    public string OT_Name { get; init; } = string.Empty;

    public IReadOnlyList<string> OT_Names { get; init; } = [];

    /// <summary> Trainer ID for the event. </summary>
    public ushort TID16 { get; init; } = UnspecifiedID;

    public IndividualValueSet IVs { get; init; }
    public Moveset Moves { get; init; }

    public string Name => "Event Gift";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);

    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var lang = GetTemplateLanguage(tr);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pi = EncounterUtil1.GetPersonal1(Version, Species);
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = LevelMin,
            Catch_Rate = GetInitialCatchRate(),
            DV16 = IVs.IsSpecified ? EncounterUtil1.GetDV16(IVs) : EncounterUtil1.GetRandomDVs(Util.Rand),

            OT_Name = EncounterUtil1.GetTrainerName(tr, lang),
            TID16 = tr.TID16,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            Type1 = pi.Type1,
            Type2 = pi.Type2,
        };

        if (TID16 != UnspecifiedID)
            pk.TID16 = TID16;
        if (OT_Name.Length != 0)
            pk.OT_Name = OT_Name;
        else if (OT_Names.Count != 0)
            pk.OT_Name = OT_Names[Util.Rand.Next(OT_Names.Count)];

        if (Version == GameVersion.Stadium)
        {
            // Amnesia Psyduck has different catch rates depending on language
            if (Species == (int)Core.Species.Psyduck)
                pk.Catch_Rate = pk.Japanese ? (byte)167 : (byte)168;
            else
                pk.Catch_Rate = Util.Rand.Next(2) == 0 ? (byte)167 : (byte)168;
        }

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr)
    {
        // Japanese events must be Japanese
        if (Language == EncounterGBLanguage.Japanese)
            return 1;

        // International events must be non-Japanese
        var lang = (int)Core.Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        if (lang == 1 && Language == EncounterGBLanguage.International)
            return 2;
        return lang;
    }

    #endregion

    private byte GetInitialCatchRate()
    {
        if (Version == GameVersion.Stadium)
        {
            // Amnesia Psyduck has different catch rates depending on language
            if (Species == (int)Core.Species.Psyduck)
                return (Language == EncounterGBLanguage.Japanese) ? (byte)167 : (byte)168;
        }

        // Encounters can have different Catch Rates (RBG vs Y)
        return EncounterUtil1.GetWildCatchRate(Version, Species);
    }

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Language != EncounterGBLanguage.Any && pk.Japanese != (Language == EncounterGBLanguage.Japanese))
            return false;
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
        if (IVs.IsSpecified)
        {
            if (Shiny == Shiny.Always && !pk.IsShiny)
                return false;
            if (Shiny == Shiny.Never && pk.IsShiny)
                return false;
            if (pk.Format <= 2)
            {
                if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
                    return false;
            }
        }

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;

        if (TID16 != UnspecifiedID && pk.TID16 != TID16)
            return false;

        if (OT_Name.Length != 0)
        {
            if (pk.OT_Name != OT_Name)
                return false;
        }
        else if (OT_Names.Count != 0)
        {
            if (!OT_Names.Contains(pk.OT_Name))
                return false;
        }

        return true;
    }

    private bool IsShinyValid(PKM pk) => Shiny switch
    {
        Shiny.Never => !pk.IsShiny,
        Shiny.Always => pk.IsShiny,
        _ => true,
    };

    private static bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format <= 2)
            return true;

        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.Egg_Location == expect;
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
        return !IsCatchRateValid(pk1.Catch_Rate);
    }

    private bool IsCatchRateValid(byte catch_rate)
    {
        if (ParseSettings.AllowGen1Tradeback && PK1.IsCatchRateHeldItem(catch_rate))
            return true;

        if (Version == GameVersion.Stadium)
        {
            // Amnesia Psyduck has different catch rates depending on language
            if (Species == (int)Core.Species.Psyduck)
                return catch_rate == (Language == EncounterGBLanguage.Japanese ? 167 : 168);
            return catch_rate is 167 or 168;
        }

        // Encounters can have different Catch Rates (RBG vs Y)
        return GBRestrictions.RateMatchesEncounter(Species, Version, catch_rate);
    }

    #endregion
}
