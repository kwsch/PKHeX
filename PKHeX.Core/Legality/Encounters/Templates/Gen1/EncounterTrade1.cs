using System;

namespace PKHeX.Core;

/// <summary>
/// Trade Encounter data with a fixed Catch Rate
/// </summary>
/// <remarks>
/// Generation 1 specific value used in detecting unmodified/un-traded Generation 1 Trade Encounter data.
/// Species &amp; Minimum level (legal) possible to acquire at.
/// </remarks>
public sealed record EncounterTrade1 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IEncounterConvertible<PK1>
{
    public byte Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => Species == (ushort)Core.Species.Haunter ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort Location => 0;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;

    private string[] Nicknames { get; }
    public ushort Species { get; }
    public byte Form => 0;
    public bool EvolveOnTrade { get; init; }
    public GameVersion Version { get; }
    public byte LevelMinRBY { get; }
    public byte LevelMinGSC { get; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => CanObtainMinGSC() ? LevelMinGSC : LevelMinRBY;
    public byte LevelMax => 100;

    public EncounterTrade1(ReadOnlySpan<string[]> names, byte index, ushort species, GameVersion version, byte rby) : this(names, index, species, version, rby, rby) { }

    public EncounterTrade1(ReadOnlySpan<string[]> names, byte index, ushort species, GameVersion version, byte levelMinRBY, byte levelMinGSC)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        Species = species;
        Version = version;
        LevelMinRBY = levelMinRBY;
        LevelMinGSC = levelMinGSC;
    }

    private bool IsNicknameValid(PKM pk, ReadOnlySpan<char> nick)
    {
        if (pk.Format <= 2)
            return IsNicknameAnyMatch(nick);

        // Converted string 1/2->7 to language specific value
        // Nicknames can be from any of the languages it can trade between.
        int lang = pk.Language;
        if (lang == 1)
        {
            // Special consideration for Hiragana strings that are transferred
            if (Version == GameVersion.YW && Species == (int)Core.Species.Dugtrio)
                return nick is "ぐりお";
            return nick.SequenceEqual(Nicknames[1]);
        }

        return GetNicknameIndex(nick) >= 2;
    }

    private bool IsNicknameAnyMatch(ReadOnlySpan<char> current) => GetNicknameIndex(current) >= 0;

    private static bool IsTrainerNameValid(PKM pk)
    {
        if (pk.Format <= 2)
            return pk.OriginalTrainerTrash is [StringConverter1.TradeOTCode, StringConverter1.TerminatorCode, ..];
        var lang = pk.Language;
        var expect = StringConverter12Transporter.GetTradeNameGen1(lang);

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        return trainer.SequenceEqual(expect);
    }

    private int GetNicknameIndex(ReadOnlySpan<char> nickname) => GetIndex(nickname, Nicknames);

    private static int GetIndex(ReadOnlySpan<char> name, ReadOnlySpan<string> arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (name.SequenceEqual(arr[i]))
                return i;
        }

        return -1;
    }

    private bool CanObtainMinGSC()
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return false;
        if (Version == GameVersion.BU && EvolveOnTrade)
            return ParseSettings.AllowGBStadium2;
        return true;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        bool gsc = CanObtainMinGSC();
        var level = gsc ? LevelMinGSC : LevelMinRBY;
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pi = EncounterUtil.GetPersonal1(Version, Species);
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = level,
            CatchRate = pi.CatchRate,
            DV16 = EncounterUtil.GetRandomDVs(Util.Rand),

            Nickname = Nicknames[lang],
            TID16 = tr.TID16,
            Type1 = pi.Type1,
            Type2 = pi.Type2,
        };
        pk.OriginalTrainerTrash[0] = StringConverter1.TradeOTCode;

        EncounterUtil.SetEncounterMoves(pk, Version, level);
        if (EvolveOnTrade)
            pk.Species++;

        pk.ResetPartyStats();

        return pk;
    }
    #endregion

    #region Matching
    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => IsTrainerNameValid(pk);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => IsNicknameValid(pk, nickname);
    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchLevel(pk, evo.LevelMax)) // minimum required level
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (Version == GameVersion.BU)
        {
            // Encounters with this version have to originate from the Japanese Blue game.
            if (!pk.Japanese)
                return false;
            // Stadium 2 can transfer from GSC->RBY without a "Trade", thus allowing un-evolved outsiders
            if (EvolveOnTrade && !ParseSettings.AllowGBStadium2 && pk.CurrentLevel < LevelMinRBY)
                return false;
        }
        return true;
    }

    private bool IsMatchLevel(PKM pk, int lvl)
    {
        if (pk is not PK1 || CanObtainMinGSC())
            return lvl >= LevelMinGSC;
        return lvl >= LevelMin;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (!IsTrainerNameValid(pk))
            return true;
        if (!IsNicknameValid(pk, pk.Nickname))
            return true;
        if (EvolveOnTrade && pk.Species == Species)
            return false;
        if (ParseSettings.AllowGen1Tradeback)
            return false;
        if (pk is not PK1 pk1)
            return false;

        var req = EncounterUtil.GetPersonal1(Version, Species).CatchRate;
        return req != pk1.CatchRate;
    }

    #endregion
}
