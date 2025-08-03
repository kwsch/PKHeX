using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Trade Encounter
/// </summary>
public sealed record EncounterTrade5B2W2 : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>,
    IFixedTrainer, IFixedNickname, IFixedGender, IFixedNature, IFixedIVSet, ITrainerID32ReadOnly
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public ushort Location => Locations.LinkTrade5NPC;
    public bool IsFixedNickname { get; init; }
    public GameVersion Version { get; }
    public Shiny Shiny => Shiny.Never;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte OTGender { get; init; }
    public required uint ID32 { get; init; }
    public ushort TID16 => (ushort)ID32;
    public ushort SID16 => (ushort)(ID32 >> 16);

    public byte Form { get; init; }
    public required byte Gender { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;

    private readonly ReadOnlyMemory<string> TrainerNames;
    private readonly ReadOnlyMemory<string> Nicknames;

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public EncounterTrade5B2W2(ReadOnlySpan<string[]> names, byte index, GameVersion version)
    {
        Version = version;
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        IsFixedNickname = true;
    }

    [SetsRequiredMembers]
    public EncounterTrade5B2W2(string[] names, GameVersion version)
    {
        Version = version;
        Gender = FixedGenderUtil.GenderRandom;
        Nature = Nature.Random;
        Nicknames = ReadOnlyMemory<string>.Empty;
        TrainerNames = names;
        IsFixedNickname = false;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            PID = Util.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = version,
            Language = language,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames.Span[language],

            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames.Span[language] : SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK5 pk, in EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        var seed = Util.Rand.Rand64();
        MonochromeRNG.Generate(pk, criteria, pi.Gender, seed, false, Shiny, Ability, Gender);

        pk.Nature = criteria.GetNature(Nature);
        var abilityIndex = Ability == AbilityPermission.OnlyHidden ? 2 : (int)(pk.PID >> 16) & 1;
        pk.RefreshAbility(abilityIndex);
        criteria.SetRandomIVs(pk, IVs);
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
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
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

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = EggLocation;
        if (pk is PB8)
            expect = Locations.Default8bNone;
        return pk.EggLocation == expect;
    }
    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (!Shiny.IsValid(pk))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && Gender != pk.Gender)
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
