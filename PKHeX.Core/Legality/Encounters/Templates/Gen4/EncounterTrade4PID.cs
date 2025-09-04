using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Trade Encounter with a fixed PID value.
/// </summary>
public sealed record EncounterTrade4PID : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>,
    IFixedTrainer, IFixedNickname, IFixedIVSet, IContestStatsReadOnly, IMoveset, IFixedGender, IFixedNature, ITrainerID32ReadOnly
{
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    public Shiny Shiny => Shiny.FixedValue;
    public bool IsFixedNickname => true;
    public bool IsEgg => false;
    public ushort EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public bool IsFixedTrainer => true;
    public byte LevelMin => Level;
    public byte LevelMax => IsMetUnset ? Level : (byte)100;
    public ushort Location => IsMetUnset ? Locations.LinkTrade4NPC : MetLocation;

    private readonly ReadOnlyMemory<string> TrainerNames;
    private readonly ReadOnlyMemory<string> Nicknames;

    public ushort Species { get; }
    public byte Level { get; }
    public GameVersion Version { get; }
    public required AbilityPermission Ability { get; init; }
    public required byte OTGender { get; init; }
    public required ushort TID16 { get; init; }
    public required ushort SID16 { get; init; }
    public required byte Gender { get; init; }
    public required IndividualValueSet IVs { get; init; }
    public Moveset Moves { get; init; }
    public byte MetLocation { get; init; } // only set by Shuckle and Spearow in HG/SS
    public byte Contest { get; init; }

    public Nature Nature => (Nature)(PID % 25);
    public byte Form => 0;
    public uint ID32 => (uint)(TID16 | (SID16 << 16));
    private bool IsMetUnset => MetLocation == 0;

    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public readonly uint PID;

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;

    public byte ContestCool => Contest;
    public byte ContestBeauty => Contest;
    public byte ContestCute => Contest;
    public byte ContestSmart => Contest;
    public byte ContestTough => Contest;
    public byte ContestSheen => 0;

    public EncounterTrade4PID(ReadOnlySpan<string[]> names, byte index, GameVersion version, uint pid, ushort species, byte level)
    {
        Version = version;
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        PID = pid;
        Species = species;
        Level = level;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.DP[Species];
        var pk = new PK4
        {
            PID = PID,
            Species = Species,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateNDS(),
            Gender = Gender,
            Nature = Nature,
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = version,
            Language = GetReceivedLanguage(language, version),
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames.Span[language],

            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = true,
            Nickname = Nicknames.Span[language],

            HandlingTrainerName = tr.OT,
            HandlingTrainerGender = tr.Gender,
        };

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, Level);
        criteria.SetRandomIVs(pk, IVs);
        pk.PID = PID;
        pk.Gender = Gender;
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        this.CopyContestStatsTo(pk);
        pk.ResetPartyStats();

        return pk;
    }

    private int GetReceivedLanguage(int lang, GameVersion version)
    {
        if (Version == GameVersion.DPPt)
            return GetLanguageDPPt(lang, version);

        // HG/SS
        // Has English Language ID for all except English origin, which is French
        if (Species == (int)Core.Species.Pikachu)
            return (int)(lang == (int)LanguageID.English ? LanguageID.French : LanguageID.English);
        return lang;
    }

    private int GetLanguageDPPt(int lang, GameVersion version)
    {
        // Has German Language ID for all except German origin, which is English
        if (Species == (int)Core.Species.Magikarp)
            return (int)(lang == (int)LanguageID.German ? LanguageID.English : LanguageID.German);
        // All other trades received (D/P only): English games have a Japanese language ID instead of English.
        if (version is not GameVersion.Pt && lang == (int)LanguageID.English)
            return (int)LanguageID.Japanese;
        return lang;
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames.Span[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames.Span[language]);
    public string GetNickname(int language) => Nicknames.Span[(uint)language < Nicknames.Length ? language : 0];

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchLevel(pk, evo))
            return false;
        if (pk.ID32 != ID32)
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
            return false;
        if (!IsMatchNatureGenderShiny(pk))
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.OriginalTrainerGender != OTGender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;
        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        // Met location is lost on transfer
        if (pk is not G4PKM pk4)
            return true;

        var met = pk4.MetLocation;
        return met == Location;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return evo.LevelMax >= Level;

        if (!IsMetUnset)
            return pk.MetLevel == Level;
        return pk.MetLevel >= LevelMin;
    }

    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (pk.EncryptionConstant != PID)
            return false;
        if (Nature != pk.Nature)
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = EggLocation;
        if (pk is PB8)
            expect = Locations.Default8bNone;
        return pk.EggLocation == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion

    /// <summary>
    /// Language obtained by the trainer will be of a foreign language ID.
    /// </summary>
    /// <remarks>
    /// Does NOT indicate for bugged D/P English origin, which is Japanese.
    /// </remarks>
    public bool IsLanguageSwap => Species is (ushort)Core.Species.Magikarp or (ushort)Core.Species.Pikachu;

    public int DetectOriginalLanguage(PKM pk)
    {
        int lang = pk.Language;
        switch (Species)
        {
            case (int)Core.Species.Pikachu: // HG/SS Pikachu
                return DetectTradeLanguageG4SurgePikachu(pk, lang);
            case (int)Core.Species.Magikarp: // D/P/Pt Magikarp
                return DetectTradeLanguageG4MeisterMagikarp(pk, lang);
        }
        // D/P English origin are Japanese lang. Can't have LanguageID 2
        if (lang != 1 || pk.Version is not (GameVersion.D or GameVersion.P))
            return lang;

        // Since two locales (JPN/ENG) can have the same LanguageID, check which we should be validating with.
        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        var len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        var expect = TrainerNames.Span[(int)LanguageID.Japanese];
        var match = trainer.SequenceEqual(expect);
        if (!match)
            return 2; // verify strings with English locale instead.
        return lang;
    }

    private int DetectTradeLanguageG4MeisterMagikarp(PKM pk, int currentLanguageID)
    {
        if (currentLanguageID == (int)LanguageID.English)
            return (int)LanguageID.German;

        // All have German, regardless of origin version.
        var lang = DetectTradeLanguage(pk.OriginalTrainerName, currentLanguageID);
        if (lang == (int)LanguageID.English) // possible collision with FR/ES/DE. Check nickname
        {
            Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
            var len = pk.LoadString(pk.NicknameTrash, nickname);
            nickname = nickname[..len];

            var french = Nicknames.Span[(int)LanguageID.French];
            return nickname.SequenceEqual(french) ? (int)LanguageID.French : (int)LanguageID.Spanish; // Spanish is same as English
        }

        return lang;
    }

    private int DetectTradeLanguageG4SurgePikachu(PKM pk, int currentLanguageID)
    {
        if (currentLanguageID == (int)LanguageID.French)
            return (int)LanguageID.English;

        // All have English, regardless of origin version.
        var lang = DetectTradeLanguage(pk.OriginalTrainerName, currentLanguageID);
        if (lang == (int)LanguageID.English) // possible collision with ES/IT. Check nickname
        {
            Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
            var len = pk.LoadString(pk.NicknameTrash, nickname);
            nickname = nickname[..len];

            var italian = Nicknames.Span[(int)LanguageID.Italian];
            return nickname.SequenceEqual(italian) ? (int)LanguageID.Italian : (int)LanguageID.Spanish;
        }

        return lang;
    }

    private int DetectTradeLanguage(ReadOnlySpan<char> actual, int currentLanguageID)
    {
        var names = TrainerNames.Span;
        for (int lang = 1; lang < names.Length; lang++)
        {
            var expect = names[lang];
            var match = actual.SequenceEqual(expect);
            if (match)
                return lang;
        }
        return currentLanguageID;
    }

    public bool IsIncorrectEnglish(PKM pk)
    {
        // Localized English forgot to change the Language ID values.
        return pk is { Language: (int)LanguageID.English, Version: GameVersion.D or GameVersion.P };
    }
}
