using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Trade Encounter
/// </summary>
public sealed record EncounterTrade3 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IFixedGender, IFixedNature, IEncounterConvertible<PK3>, IContestStatsReadOnly
{
    public int Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public int Location => Locations.LinkTrade3NPC;
    public Shiny Shiny => Shiny.FixedValue;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;
    public Nature Nature => (Nature)(PID % 25);

    private string[] TrainerNames { get; }
    private string[] Nicknames { get; }

    public required AbilityPermission Ability { get; init; }
    public required byte Gender { get; init; }
    public required byte OTGender { get; init; }
    public required IndividualValueSet IVs { get; init; }
    public ushort Species { get; }
    public byte Form => 0;
    public byte Level { get; }
    public GameVersion Version { get; }

    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public uint PID { get; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => 100;

    public required ushort TID16 { get; init; }
    public ushort SID16 { get; init; }

    public byte CNT_Cool   { get; private init; }
    public byte CNT_Beauty { get; private init; }
    public byte CNT_Cute   { get; private init; }
    public byte CNT_Smart  { get; private init; }
    public byte CNT_Tough  { get; private init; }
    public byte CNT_Sheen  { get; private init; }

    public required ReadOnlySpan<byte> Contest
    {
        init
        {
            CNT_Cool   = value[0];
            CNT_Beauty = value[1];
            CNT_Cute   = value[2];
            CNT_Smart  = value[3];
            CNT_Tough  = value[4];
            CNT_Sheen  = value[5];
        }
    }

    public EncounterTrade3(ReadOnlySpan<string[]> names, byte index, GameVersion game, uint pid, ushort species, byte level)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Version = game;
        PID = pid;
        Species = species;
        Level = level;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = Level,

            Met_Location = Location,
            Met_Level = Level,
            Version = (byte)version,
            Ball = (byte)FixedBall,
            OT_Friendship = pi.BaseFriendship,

            Language = lang,
            OT_Gender = OTGender,
            TID16 = TID16,
            SID16 = SID16,
        };

        // Italian LG Jynx untranslated from English name
        if (Species == (int)Core.Species.Jynx && version == GameVersion.LG && lang == (int)LanguageID.Italian)
            lang = 2;
        pk.Nickname = Nicknames[lang];
        pk.OT_Name = TrainerNames[lang];

        EncounterUtil.SetEncounterMoves(pk, Version, Level);
        SetPINGA(pk, criteria);

        pk.ResetPartyStats();
        this.CopyContestStatsTo(pk);

        return pk;
    }

    private void SetPINGA(PK3 pk, EncounterCriteria criteria)
    {
        pk.PID = PID;
        criteria.SetRandomIVs(pk, IVs);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        // Nature and Gender are derived from PID.
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.EncryptionConstant != PID)
            return false;
        if (!Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (pk.TID16 != TID16)
            return false;
        if (pk.SID16 != SID16)
            return false;
        if (evo.LevelMax < Level)
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.Gender != Gender)
            return false;
        if (pk.OT_Gender != OTGender)
            return false;
        if (pk.Egg_Location != 0)
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language)
    {
        if (Species == (int)Core.Species.Jynx && pk.Version == (int)GameVersion.LG && language == (int)LanguageID.Italian)
            language = 2;
        return language != 0 && (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames[language]);
    }

    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language)
    {
        if (Species == (int)Core.Species.Jynx && pk.Version == (int)GameVersion.LG && language == (int)LanguageID.Italian)
            language = 2;
        return language != 0 && (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames[language]);
    }

    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    #endregion
}
