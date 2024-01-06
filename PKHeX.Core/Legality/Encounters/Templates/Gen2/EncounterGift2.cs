using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 2
/// </summary>
public sealed record EncounterGift2(ushort Species, byte Level, GameVersion Version = GameVersion.GS)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK2>, IFixedGBLanguage, IHatchCycle, IMoveset, IFixedIVSet
{
    public int Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public byte Form => 0;

    public Ball FixedBall => Ball.Poke;
    int ILocation.Location => Location;
    public int EggLocation => 0;
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
    public string OT_Name { get; init; } = string.Empty;

    public IReadOnlyList<string> OT_Names { get; init; } = [];

    private const ushort UnspecifiedID = 0;

    /// <summary> Trainer ID for the event. </summary>
    public ushort TID16 { get; init; } = UnspecifiedID;

    public bool IsGift => TID16 != UnspecifiedID;

    public sbyte CurrentLevel { get; init; } = -1;

    public byte EggCycles { get; init; }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = GetTemplateLanguage(tr);
        var pi = PersonalTable.C[Species];
        var pk = new PK2
        {
            Species = Species,
            CurrentLevel = CurrentLevel == -1 ? LevelMin : CurrentLevel,

            TID16 = TID16 != UnspecifiedID ? TID16 : tr.TID16,
            OT_Name = GetInitialOT(tr),

            OT_Friendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (EggEncounter)
        {
        }
        else if (Version == GameVersion.C || (Version == GameVersion.GSC && tr.Game == (int)GameVersion.C))
        {
            if (!IsGift)
                pk.OT_Gender = tr.Gender;
            pk.Met_Level = LevelMin;
            pk.Met_Location = Location;
            pk.Met_TimeOfDay = EncounterTime.Any.RandomValidTime();
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
        if (OT_Name.Length != 0)
            return OT_Name;
        if (OT_Names.Count != 0)
            return OT_Names[Util.Rand.Next(OT_Names.Count)];
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

        if (CurrentLevel != -1 && CurrentLevel > pk.CurrentLevel)
            return false;

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;

        if (EggEncounter && !pk.IsEgg)
            return true;

        // Check OT Details
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

    private bool IsMatchEggLocation(PKM pk)
    {
        if (pk is not ICaughtData2 c2)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.Egg_Location == expect;
        }

        if (pk.IsEgg)
        {
            if (!EggEncounter)
                return false;
            if (c2.Met_Location != 0 && c2.Met_Level != 0)
                return false;
            if (pk.OT_Friendship > EggCycles)
                return false;
        }
        else
        {
            switch (c2.Met_Level)
            {
                case 0 when c2.Met_Location != 0:
                    return false;
                case 1: // 0 = second floor of every Pokémon Center, valid
                    return true;
                default:
                    if (pk.Met_Location == 0 && c2.Met_Level != 0)
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
                return Location == pk.Met_Location;
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
            return pk.Met_Level == (EggEncounter ? 1 : Level);
        return true;
    }

    #endregion
}
