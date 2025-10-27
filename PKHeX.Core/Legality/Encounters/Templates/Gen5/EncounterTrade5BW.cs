using System;

namespace PKHeX.Core;

/// <summary>Generation 5 Trade with Fixed PID</summary>
public sealed record EncounterTrade5BW : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>,
    IFixedTrainer, IFixedNickname, IFixedGender, IFixedNature, IFixedIVSet, ITrainerID32ReadOnly
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public ushort Location => Locations.LinkTrade5NPC;
    public bool IsFixedNickname => true;
    public GameVersion Version { get; }
    public Shiny Shiny => Shiny.Never;
    public bool IsEgg => false;
    public ushort EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public bool IsFixedTrainer => true;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    private readonly ReadOnlyMemory<string> TrainerNames;
    private readonly ReadOnlyMemory<string> Nicknames;

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte OTGender { get; init; }
    public required ushort TID16 { get; init; }
    public ushort SID16 => 0;
    public uint ID32 => TID16;
    public required byte Gender { get; init; }
    public required IndividualValueSet IVs { get; init; }
    public required Nature Nature { get; init; }
    public byte Form { get; init; }

    /// <summary> Fixed <see cref="PKM.PID"/> value the encounter must have.</summary>
    public uint PID { get; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;

    public EncounterTrade5BW(ReadOnlySpan<string[]> names, byte index, GameVersion version, uint pid)
    {
        Version = version;
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        PID = pid;
    }

    /// <summary>
    /// Checks if the language can be missing.
    /// </summary>
    /// <remarks>
    /// Generation 5 trades from B/W forgot to set the Language ID, so it remains as 0.
    /// <br/> This value is corrected when the entity is transferred from PK5->PK6.
    /// <br/> B2/W2 is unaffected by this game data bug.
    /// </remarks>
    public static bool IsValidMissingLanguage(PKM pk) => pk.Format == 5;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage456((LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.BW[Species];
        var pk = new PK5
        {
            PID = PID,
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateNDS(),
            Gender = Gender,
            Nature = Nature,
            Ball = (byte)FixedBall,

            TID16 = TID16,
            Version = version,
            Language = language == 1 ? 0 : language, // Trades for JPN games have language ID of 0, not 1.
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames.Span[language],

            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames.Span[language] : SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        criteria.SetRandomIVs(pk, IVs);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames.Span[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames.Span[language]);
    public string GetNickname(int language) => Nicknames.Span[(uint)language < Nicknames.Length ? language : 0];

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.MetLevel != Level)
            return false;
        if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
            return false;
        if (!IsMatchNatureGenderShiny(pk))
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

    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (PID != pk.EncryptionConstant)
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
}
