using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 BD/SP Trade Encounter
/// </summary>
public sealed record EncounterTrade8b : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IEncounterConvertible<PB8>, IScaledSizeReadOnly, IFixedOTFriendship, IMoveset, IContestStatsReadOnly, IFixedGender, IFixedNature
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8b;
    public ushort Location => Locations.LinkTrade6NPC;
    public Shiny Shiny => Shiny.Never;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public ushort EggLocation => Locations.Default8bNone;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;
    public GameVersion Version { get; }

    private string[] TrainerNames { get; }
    private string[] Nicknames { get; }

    public required Nature Nature { get; init; }
    public required ushort ID32 { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte Gender { get; init; }
    public required byte OTGender { get; init; }
    public required uint PID { get; init; }
    public required uint EncryptionConstant { get; init; }
    public required byte HeightScalar { get; init; }
    public required byte WeightScalar { get; init; }
    public required Moveset Moves { get; init; }
    public required IndividualValueSet IVs { get; init; }
    public required ushort Species { get; init; }
    public required byte Level { get; init; }

    public byte OriginalTrainerFriendship => Species == (int)Core.Species.Chatot ? (byte)35 : (byte)50;
    private byte BaseContest => Species == (int)Core.Species.Chatot ? (byte)20 : (byte)0;

    public byte ContestCool => BaseContest;
    public byte ContestBeauty => BaseContest;
    public byte ContestCute => BaseContest;
    public byte ContestSmart => BaseContest;
    public byte ContestTough => BaseContest;
    public byte ContestSheen => 0;

    public byte Form => 0;

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public EncounterTrade8b(ReadOnlySpan<string[]> names, byte index, GameVersion game)
    {
        Version = game;
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PB8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.BDSP[Species, Form];
        var pk = new PB8
        {
            PID = PID,
            EncryptionConstant = EncryptionConstant,
            Species = Species,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Gender = Gender,
            Nature = Nature,
            StatNature = Nature,
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames[lang],

            OriginalTrainerFriendship = OriginalTrainerFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames[lang] : SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            HeightScalar = HeightScalar,
            WeightScalar = WeightScalar,
            HandlingTrainerName = tr.OT,
            HandlingTrainerLanguage = (byte)tr.Language,
            HandlingTrainerFriendship = pi.BaseFriendship,
        };

        // Has German Language ID for all except German origin, which is Japanese
        if (Species == (int)Core.Species.Magikarp)
            pk.Language = (int)(pk.Language == (int)LanguageID.German ? LanguageID.Japanese : LanguageID.German);

        this.CopyContestStatsTo(pk);
        pk.SetMoves(Moves);
        criteria.SetRandomIVs(pk, IVs);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames[language]);
    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.MetLevel != Level)
            return false;
        if (pk.EncryptionConstant != EncryptionConstant)
            return false;
        if (pk.PID != PID)
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;
        if (pk is IScaledSize h && h.HeightScalar != HeightScalar)
            return false;
        if (pk is IScaledSize w && w.WeightScalar != WeightScalar)
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
            return false;
        if (pk.Gender != Gender)
            return false;
        if (pk.Nature != Nature)
            return false;
        if (pk.ID32 != ID32)
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.OriginalTrainerGender != OTGender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchLocationRemapped(pk);
        return IsMatchLocationExact(pk) || IsMatchLocationRemapped(pk);
    }

    private bool IsMatchLocationExact(PKM pk) => pk.MetLocation == Location;

    private bool IsMatchLocationRemapped(PKM pk)
    {
        var met = pk.MetLocation;
        var version = pk.Version;
        if (pk.Context == EntityContext.Gen8)
            return LocationsHOME.IsValidMetBDSP(met, version);
        return LocationsHOME.GetMetSWSH(Location, version) == met;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchEggLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchEggLocationRemapped(pk);
        // Either
        return IsMatchEggLocationExact(pk) || IsMatchEggLocationRemapped(pk);
    }

    private static bool IsMatchEggLocationRemapped(PKM pk) => pk.EggLocation == 0;
    private bool IsMatchEggLocationExact(PKM pk) => pk.EggLocation == EggLocation;

    public int DetectMeisterMagikarpLanguage(ReadOnlySpan<char> nick, ReadOnlySpan<char> ot, int currentLanguageID)
    {
        // Receiving the trade on a German game -> Japanese LanguageID.
        // Receiving the trade on any other language -> German LanguageID.
        if (currentLanguageID is not ((int)LanguageID.Japanese or (int)LanguageID.German))
            return -1;

        for (int i = 1; i < (int)LanguageID.ChineseT; i++)
        {
            if (!nick.SequenceEqual(Nicknames[i]))
                continue;
            if (!ot.SequenceEqual(TrainerNames[i]))
                continue;

            // Language gets flipped to another language ID; can't be equal.
            var shouldNotBe = currentLanguageID == (int)LanguageID.German ? LanguageID.German : LanguageID.Japanese;
            return i != (int)shouldNotBe ? i : 0;
        }
        return -1;
    }

    #endregion

    /// <summary>
    /// Traded between players within BD/SP, the original OT is replaced with the above OT (version dependent) as the original OT is >6 chars in length.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <returns>True if matches the pattern of a traded Magikarp.</returns>
    public bool IsMagikarpJapaneseTradedBDSP(PKM pk)
    {
        return Species is (int)Core.Species.Magikarp && pk is { Language: (int)LanguageID.Japanese, OriginalTrainerName: "Diamond." or "Pearl." };
    }

    /// <summary>
    /// Nintendo Switch updates NgWord disallows the French nickname (Pijouk) and resets it back to default (Pijako).
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <returns>True if matches the pattern of a reverted Nickname Pijako.</returns>
    public bool IsPijako(PKM pk)
    {
        return Species == (int)Core.Species.Chatot && pk is { Language: (int)LanguageID.French, IsNicknamed: false };
    }
}
