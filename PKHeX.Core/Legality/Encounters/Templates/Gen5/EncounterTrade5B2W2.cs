using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Trade Encounter
/// </summary>
public sealed record EncounterTrade5B2W2 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IEncounterConvertible<PK5>, IFixedGender, IFixedNature, IFixedIVSet
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

    public byte Form { get; init; }
    public required byte Gender { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;

    private readonly string[] TrainerNames;
    private readonly string[] Nicknames;

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
        Nicknames = [];
        TrainerNames = names;
        IsFixedNickname = false;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
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
            Language = lang,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames[lang],

            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames[lang] : SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK5 pk, EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        if (pk.IsShiny)
            pk.PID ^= 0x1000_0000;
        var nature = criteria.GetNature(Nature);
        var gender = criteria.GetGender(Gender, pi);
        var ability = criteria.GetAbilityFromNumber(Ability);
        PIDGenerator.SetRandomWildPID5(pk, nature, ability, gender);
        criteria.SetRandomIVs(pk, IVs);
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames[language]);
    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

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
