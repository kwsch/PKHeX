using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Trade Encounter
/// </summary>
public sealed record EncounterTrade8 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IEncounterConvertible<PK8>, IDynamaxLevelReadOnly, IRelearn, IMemoryOTReadOnly, IFlawlessIVCount, IFixedGender, IFixedNature
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8;
    public ushort Location => Locations.LinkTrade6NPC;
    public Moveset Relearn { get; init; }

    public ushort OriginalTrainerMemoryVariable { get; }
    public byte OriginalTrainerMemory { get; }
    public byte OriginalTrainerMemoryFeeling { get; }
    public byte OriginalTrainerMemoryIntensity { get; }
    public byte DynamaxLevel { get; init; }
    public byte FlawlessIVCount { get; init; }
    public Shiny Shiny { get; }

    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname { get; }

    private string[] TrainerNames { get; }
    private string[] Nicknames { get; }

    public Nature Nature { get; init; } // always set by either constructor or initializer
    public required uint ID32 { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte Gender { get; init; }
    public required byte OTGender { get; init; }

    public required IndividualValueSet IVs { get; init; }

    public ushort Species { get; }
    public byte Form { get; init; }

    public byte Level { get; }
    public GameVersion Version { get; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public EncounterTrade8(ReadOnlySpan<string[]> names, byte index, GameVersion game, ushort species, byte level, byte memory, ushort arg, byte feel, byte intensity)
    {
        Version = game;
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Species = species;
        Level = level;
        Shiny = Shiny.Never;

        OriginalTrainerMemory = memory;
        OriginalTrainerMemoryVariable = arg;
        OriginalTrainerMemoryFeeling = feel;
        OriginalTrainerMemoryIntensity = intensity;
        IsFixedNickname = true;
    }

    [SetsRequiredMembers]
    public EncounterTrade8(string[] trainerNames, GameVersion game, ushort species, byte level, byte memory, ushort arg, byte feel, byte intensity)
    {
        Version = game;
        Nicknames = [];
        TrainerNames = trainerNames;
        Species = species;
        Level = level;
        Shiny = Shiny.Random;

        OriginalTrainerMemory = memory;
        OriginalTrainerMemoryVariable = arg;
        OriginalTrainerMemoryFeeling = feel;
        OriginalTrainerMemoryIntensity = intensity;
        IsFixedNickname = false;
        Gender = FixedGenderUtil.GenderRandom;
        Nature = Nature.Random;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.SWSH[Species, Form];
        var rnd = Util.Rand;
        var pk = new PK8
        {
            PID = rnd.Rand32(),
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)FixedBall,
            Gender = Gender,

            ID32 = ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames[lang],

            OriginalTrainerMemory = OriginalTrainerMemory,
            OriginalTrainerMemoryIntensity = OriginalTrainerMemoryIntensity,
            OriginalTrainerMemoryFeeling = OriginalTrainerMemoryFeeling,
            OriginalTrainerMemoryVariable = OriginalTrainerMemoryVariable,
            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames[lang] : SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            DynamaxLevel = DynamaxLevel,
            HandlingTrainerName = tr.OT,
            HandlingTrainerGender = tr.Gender,
            HandlingTrainerLanguage = (byte)tr.Language,
            CurrentHandler = 1,
            HandlingTrainerFriendship = pi.BaseFriendship,
        };
        if (Shiny == Shiny.Never && pk.IsShiny)
            pk.PID ^= 0x1000_0000u;
        pk.SetRelearnMoves(Relearn);

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        SetPINGA(pk, criteria, pi);

        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK8 pk, EncounterCriteria criteria, PersonalInfo8SWSH pi)
    {
        var gender = criteria.GetGender(Gender, pi);
        var nature = criteria.GetNature(Nature);
        int ability = criteria.GetAbilityFromNumber(Ability);
        pk.Nature = pk.StatNature = nature;
        pk.Gender = gender;
        pk.RefreshAbility(ability);
        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk, FlawlessIVCount);
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
        if (IVs.IsSpecified)
        {
            if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
                return false;
        }
        if (!IsMatchNatureGenderShiny(pk))
            return false;
        if (pk.ID32 != ID32)
            return false;
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
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
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
