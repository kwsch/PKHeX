using System;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterGift3Colo : IEncounterable, IEncounterMatch, IEncounterConvertible<CK3>,
    IRandomCorrelation, IFixedTrainer, IMoveset, ITrainerID16ReadOnly
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version { get; }
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.Never;
    public byte Form => 0;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public bool IsFixedTrainer => true;
    public bool IsJapaneseBonusDisk => Version == GameVersion.R;

    private readonly ReadOnlyMemory<string> TrainerNames;
    public ushort Species { get; }
    public byte Level { get; }
    public required byte Location { get; init; }
    public Moveset Moves { get; init; }
    public required ushort TID16 { get; init; }
    public required byte OriginalTrainerGender { get; init; }

    public EncounterGift3Colo(ushort species, byte level, ReadOnlyMemory<string> trainers, GameVersion game)
    {
        Species = species;
        Level = level;
        TrainerNames = trainers;
        Version = game;
    }

    public string Name => "Gift Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public CK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public CK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = GetTemplateLanguage(tr);
        var pi = PersonalTable.E[Species];
        var pk = new CK3
        {
            Species = Species,
            CurrentLevel = Level,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = Level,
            Version = Version,
            Ball = (byte)Ball.Poke,

            Language = lang,
            OriginalTrainerName = TrainerNames.Span[lang],
            OriginalTrainerGender = OriginalTrainerGender,
            ID32 = TID16,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr)
    {
        if (IsJapaneseBonusDisk)
            return 1; // Japanese
        return (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
    }

    private static void SetPINGA(CK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        if (criteria.Shiny != Shiny.Never)
            criteria = criteria with { Shiny = Shiny.Never }; // ensure no bad inputs
        if (criteria.IsSpecifiedIVsAll() && MethodCXD.SetFromIVs(pk, criteria, pi, noShiny: true))
            return;
        MethodCXD.SetRandom(pk, criteria, pi, noShiny: true, Util.Rand32());
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.Version != Version)
            return false;
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

    private static bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format == 3)
            return true;

        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 3) // Met Level lost on PK3=>PK4
            return evo.LevelMax >= Level;
        return pk.MetLevel == Level;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format != 3)
            return true; // transfer location verified later

        var met = pk.MetLocation;
        return Location == met;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (pk.Ball != (byte)FixedBall)
            return true;
        if (IsJapaneseBonusDisk && !pk.Japanese) // Japanese Colosseum
            return true;
        return false;
    }
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk) => type is PIDType.CXD or PIDType.CXDAnti ? Match : Mismatch;

    public PIDType GetSuggestedCorrelation() => PIDType.CXD;

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language)
    {
        if ((uint)language >= TrainerNames.Length)
            return false;

        var expect = TrainerNames.Span[language].AsSpan();
        if (pk is CK3 && expect.SequenceEqual(trainer))
            return true; // not yet transferred to mainline Gen3

        var max = language == 1 ? 5 : 7;
        if (expect.Length > max)
            expect = expect[..max];
        if (pk.Context == EntityContext.Gen3)
            return trainer.SequenceEqual(expect);
        if (IsSpanishDuking(language)) // Gen4+
            return trainer is Encounters3Colo.TrainerNameDukingSpanish4;
        return trainer.SequenceEqual(expect);
    }

    private bool IsSpanishDuking(int language) => language is (int)LanguageID.Spanish && Species is (int)Core.Species.Plusle;
}
