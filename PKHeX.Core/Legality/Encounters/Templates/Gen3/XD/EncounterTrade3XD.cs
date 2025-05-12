using System;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterTrade3XD : IEncounterable, IEncounterMatch, IEncounterConvertible<XK3>,
    IRandomCorrelation, IFixedTrainer, IFixedNickname, IFatefulEncounterReadOnly, IMoveset, ITrainerID16ReadOnly
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version => GameVersion.XD;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    private bool Gift => FixedBall == Ball.Poke;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public bool FatefulEncounter => true;

    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => Nicknames.Length != 0;
    public ushort Species { get; }
    public byte Level { get; }

    public Ball FixedBall => Ball.Poke;

    public required byte Location { get; init; }
    public byte Form => 0;
    public bool IsEgg => false;
    public required Moveset Moves { get; init; }
    public required ushort TID16 { get; init; }
    // SID: Based on player ID

    private readonly ReadOnlyMemory<string> TrainerNames;

    private readonly ReadOnlyMemory<string> Nicknames;

    public EncounterTrade3XD(ushort species, byte level, ReadOnlyMemory<string> trainer) : this(species, level, trainer, ReadOnlyMemory<string>.Empty) { }

    public EncounterTrade3XD(ushort species, byte level, ReadOnlyMemory<string> trainer, ReadOnlyMemory<string> nicknames)
    {
        Species = species;
        Level = level;
        TrainerNames = trainer;
        Nicknames = nicknames;
    }

    public string Name => "Trade Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public XK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public XK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = GetTemplateLanguage(tr);
        var pi = PersonalTable.E[Species];
        var pk = new XK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = Level,
            Version = GameVersion.CXD,
            Ball = (byte)Ball.Poke,
            FatefulEncounter = FatefulEncounter,
            Language = lang,
            OriginalTrainerName = TrainerNames.Span[lang],
            OriginalTrainerGender = 0,
            TID16 = TID16,
            SID16 = tr.SID16,
            Nickname = IsFixedNickname ? GetNickname(lang) : SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr) => (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);

    private static void SetPINGA(XK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        if (criteria.IsSpecifiedIVsAll() && MethodCXD.SetFromIVs(pk, criteria, pi, noShiny: false))
            return;
        MethodCXD.SetRandom(pk, criteria, pi, noShiny: false, Util.Rand32());
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
        if (pk.TID16 != TID16) // SID is from player!
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
        if (Gift && pk.Ball != (byte)FixedBall)
            return true;
        return false;
    }
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk) => type is PIDType.CXD ? Match : Mismatch;
    public PIDType GetSuggestedCorrelation() => PIDType.CXD;

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language)
    {
        if ((uint)language >= TrainerNames.Length)
            return false;
        if (language == 0 || (uint)language >= TrainerNames.Length)
            return false;
        var name = TrainerNames.Span[language];
        if (pk.Context == EntityContext.Gen3)
            return trainer.SequenceEqual(name);
        if (IsSpanishDuking(language)) // Gen4+
            return trainer is Encounters3Colo.TrainerNameDukingSpanish4;
        return trainer.SequenceEqual(name);
    }

    private bool IsSpanishDuking(int language) => language is (int)LanguageID.Spanish && Species is not (int)Core.Species.Elekid;

    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language)
    {
        if (!IsFixedNickname)
            return true;
        if (language == 0 || (uint)language >= Nicknames.Length)
            return false;
        var name = Nicknames.Span[language];
        if (pk.Context == EntityContext.Gen3)
            return nickname.SequenceEqual(name);

        Span<char> tmp = stackalloc char[name.Length];
        StringConverter345.TransferGlyphs34(name, language, tmp);
        return nickname.SequenceEqual(tmp);
    }

    public string GetNickname(int language) => Nicknames.Span[(uint)language < Nicknames.Length ? language : 0];
}
