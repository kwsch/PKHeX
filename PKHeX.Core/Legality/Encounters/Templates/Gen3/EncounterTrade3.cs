using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Trade Encounter
/// </summary>
public sealed record EncounterTrade3 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname,
    IFixedGender, IFixedNature, IFixedIVSet, IEncounterConvertible<PK3>, IContestStatsReadOnly, ITrainerID32ReadOnly
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public ushort Location => Locations.LinkTrade3NPC;
    public Shiny Shiny => Shiny.FixedValue;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;
    public Nature Nature => (Nature)(PID % 25);

    private readonly ReadOnlyMemory<string> TrainerNames;
    private readonly ReadOnlyMemory<string> Nicknames;

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
    public uint ID32 => TID16 | (uint)(SID16 << 16);

    public byte ContestCool   { get; private init; }
    public byte ContestBeauty { get; private init; }
    public byte ContestCute   { get; private init; }
    public byte ContestSmart  { get; private init; }
    public byte ContestTough  { get; private init; }
    public byte ContestSheen  { get; private init; }

    public required ReadOnlySpan<byte> Contest
    {
        init
        {
            ContestCool   = value[0];
            ContestBeauty = value[1];
            ContestCute   = value[2];
            ContestSmart  = value[3];
            ContestTough  = value[4];
            ContestSheen  = value[5];
        }
    }

    public EncounterTrade3(ReadOnlySpan<string[]> names, byte index, GameVersion version, uint pid, ushort species, byte level)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Version = version;
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
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = Level,

            MetLocation = Location,
            MetLevel = Level,
            Version = version,
            Ball = (byte)FixedBall,
            OriginalTrainerFriendship = pi.BaseFriendship,

            Language = language,
            OriginalTrainerGender = OTGender,
            TID16 = TID16,
            SID16 = SID16,
        };

        // Italian LG Jynx untranslated from English name
        if (Species == (int)Core.Species.Jynx && version == GameVersion.LG && language == (int)LanguageID.Italian)
            language = 2;
        pk.Nickname = Nicknames.Span[language];
        pk.OriginalTrainerName = TrainerNames.Span[language];

        EncounterUtil.SetEncounterMoves(pk, Version, Level);
        SetPINGA(pk, criteria);

        pk.ResetPartyStats();
        this.CopyContestStatsTo(pk);

        return pk;
    }

    private void SetPINGA(PK3 pk, in EncounterCriteria criteria)
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
        if (pk.OriginalTrainerGender != OTGender)
            return false;
        if (pk.EggLocation != 0)
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language)
    {
        if (Species == (int)Core.Species.Jynx && pk.Version == GameVersion.LG && language == (int)LanguageID.Italian)
            language = 2;

        var names = TrainerNames.Span;
        if (language == 0 || (uint)language >= names.Length)
            return false;
        var name = names[language];
        if (pk.Context == EntityContext.Gen3)
            return trainer.SequenceEqual(name);

        Span<char> tmp = stackalloc char[name.Length];
        StringConverter345.TransferGlyphs34(name, language, tmp);
        return trainer.SequenceEqual(tmp);
    }

    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language)
    {
        if (Species == (int)Core.Species.Jynx && pk.Version == GameVersion.LG && language == (int)LanguageID.Italian)
            language = 2;

        var names = Nicknames.Span;
        if (language == 0 || (uint)language >= names.Length)
            return false;
        var name = names[language];
        if (pk.Context == EntityContext.Gen3)
            return nickname.SequenceEqual(name);

        Span<char> tmp = stackalloc char[name.Length];
        StringConverter345.TransferGlyphs34(name, language, tmp);
        if (pk.Context >= EntityContext.Gen6)
            StringConverter345.TransferGlyphs56(tmp); // Apostrophe ' vs ’
        return nickname.SequenceEqual(tmp);
    }

    public string GetNickname(int language) => Nicknames.Span[(uint)language < Nicknames.Length ? language : 0];

    #endregion
}
