using System;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 2
/// </summary>
public sealed record EncounterGift2(ushort Species, byte Level, GameVersion Version = GameVersion.GS)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK2>, IFixedGBLanguage, IHatchCycle, IMoveset, IFixedIVSet
{
    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public byte Form => 0;

    public Ball FixedBall => Ball.Poke;
    ushort ILocation.Location => Location;
    public ushort EggLocation => 0;
    public bool IsShiny => Shiny == Shiny.Always;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;

    public Shiny Shiny { get; init; } = Shiny.Random;
    public byte Location { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Moveset Moves { get; init; }
    public bool EggEncounter => EggCycles != 0;

    public string Name => "GB Era Event Gift";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public EncounterGBLanguage Language { get; init; } = EncounterGBLanguage.Japanese;

    /// <summary> Trainer name for the event. </summary>
    public string OriginalTrainerName { get; init; } = string.Empty;

    public ReadOnlyMemory<string> TrainerNames { get; init; }

    private const ushort UnspecifiedID = 0;

    /// <summary> Trainer ID for the event. </summary>
    public ushort TID16 { get; init; } = UnspecifiedID;

    public bool IsGift => TID16 != UnspecifiedID;

    public byte CurrentLevel { get; init; }

    public byte EggCycles { get; init; }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = GetTemplateLanguage(tr);
        var pi = PersonalTable.C[Species];
        var pk = new PK2
        {
            Species = Species,
            CurrentLevel = CurrentLevel == 0 ? LevelMin : CurrentLevel,

            TID16 = TID16 != UnspecifiedID ? TID16 : tr.TID16,
            OriginalTrainerName = GetInitialOT(tr),

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (EggEncounter)
        {
        }
        else if (Version == GameVersion.C || (Version == GameVersion.GSC && tr.Version == GameVersion.C))
        {
            if (!IsGift)
                pk.OriginalTrainerGender = tr.Gender;
            pk.MetLevel = LevelMin;
            pk.MetLocation = Location;
            pk.MetTimeOfDay = EncounterTime.Any.RandomValidTime();
        }

        if (Shiny == Shiny.Always)
            pk.SetShiny();

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk);

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

    private string GetInitialOT(ITrainerInfo tr)
    {
        if (OriginalTrainerName.Length != 0)
            return OriginalTrainerName;
        if (TrainerNames.Length != 0)
            return TrainerNames.Span[Util.Rand.Next(TrainerNames.Length)];
        return tr.OT;
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Shiny == Shiny.Always && !pk.IsShiny)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
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
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (Language != EncounterGBLanguage.Any && pk.Japanese != (Language == EncounterGBLanguage.Japanese))
            return false;

        if (CurrentLevel != 0 && CurrentLevel > pk.CurrentLevel)
            return false;

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;

        if (EggEncounter && !pk.IsEgg)
            return true;

        // Check OT Details
        if (TID16 != UnspecifiedID && pk.TID16 != TID16)
            return false;

        if (OriginalTrainerName.Length != 0)
        {
            if (pk.OriginalTrainerName != OriginalTrainerName)
                return false;
        }
        else if (TrainerNames.Length != 0)
        {
            if (!TrainerNames.Span.Contains(pk.OriginalTrainerName))
                return false;
        }

        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (pk is not ICaughtData2 c2)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.EggLocation == expect;
        }

        if (pk.IsEgg)
        {
            if (!EggEncounter)
                return false;
            if (c2.MetLocation != 0 && c2.MetLevel != 0)
                return false;
            if (pk.OriginalTrainerFriendship > EggCycles)
                return false;
        }
        else
        {
            switch (c2.MetLevel)
            {
                case 0 when c2.MetLocation != 0:
                    return false;
                case 1: // 0 = second floor of every Pokémon Center, valid
                    return true;
                default:
                    if (pk.MetLocation == 0 && c2.MetLevel != 0)
                        return false;
                    break;
            }
        }

        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (EggEncounter)
            return true;
        if (pk is not ICaughtData2 c2)
            return true;

        if (Version is GameVersion.C or GameVersion.GSC)
        {
            if (c2.CaughtData is not 0)
                return Location == pk.MetLocation;
            if (pk.Species == (int)Core.Species.Celebi)
                return false; // Cannot reset the Met data
        }
        return true;
    }

    private bool IsShinyValid(PKM pk) => Shiny switch
    {
        Shiny.Never => !pk.IsShiny,
        Shiny.Always => pk.IsShiny,
        _ => true,
    };

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (evo.LevelMax < Level)
            return false;
        if (pk is ICaughtData2 { CaughtData: not 0 })
            return pk.MetLevel == (EggEncounter ? 1 : Level);
        return true;
    }

    #endregion
}
