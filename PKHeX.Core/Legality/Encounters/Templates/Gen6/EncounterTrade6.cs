using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Trade Encounter
/// </summary>
public sealed record EncounterTrade6 : IEncounterable, IEncounterMatch, IEncounterConvertible<PK6>,
    IFixedTrainer, IFixedNickname, IFixedGender, IFixedNature, IFixedIVSet, IMemoryOTReadOnly, ITrainerID32ReadOnly
{
    public byte Generation => 6;
    public EntityContext Context => EntityContext.Gen6;
    public ushort Location => Locations.LinkTrade6NPC;
    public Shiny Shiny => Shiny.Never;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname { get; init; } = true;

    private readonly ReadOnlyMemory<string> TrainerNames;
    private readonly ReadOnlyMemory<string> Nicknames;

    public required Nature Nature { get; init; }
    public required ushort TID16 { get; init; }
    public ushort SID16 => 0;
    public uint ID32 => TID16;
    public required AbilityPermission Ability { get; init; }
    public required byte Gender { get; init; }
    public required byte OTGender { get; init; }

    public required IndividualValueSet IVs { get; init; }
    public required ushort Species { get; init; }
    public byte Form => 0;
    public required byte Level { get; init; }
    public GameVersion Version { get; }
    public byte OriginalTrainerMemory { get; }
    public byte OriginalTrainerMemoryIntensity { get; }
    public byte OriginalTrainerMemoryFeeling { get; }
    public ushort OriginalTrainerMemoryVariable { get; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public EncounterTrade6(ReadOnlySpan<string[]> names, byte index, GameVersion version, byte m, byte i, byte f, ushort v)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Version = version;
        OriginalTrainerMemory = m;
        OriginalTrainerMemoryIntensity = i;
        OriginalTrainerMemoryFeeling = f;
        OriginalTrainerMemoryVariable = v;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK6 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK6 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.AO[Species];
        var rnd = Util.Rand;
        var geo = tr.GetRegionOrigin(language);
        var pk = new PK6
        {
            PID = EncounterUtil.GetRandomPID(tr, rnd, Shiny),
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDate3DS(),
            Gender = Gender,
            Nature = Nature,
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = version,
            Language = language,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames.Span[language],

            OriginalTrainerMemory = OriginalTrainerMemory,
            OriginalTrainerMemoryIntensity = OriginalTrainerMemoryIntensity,
            OriginalTrainerMemoryFeeling = OriginalTrainerMemoryFeeling,
            OriginalTrainerMemoryVariable = OriginalTrainerMemoryVariable,
            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames.Span[language] : SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),

            HandlingTrainerName = tr.OT,
            HandlingTrainerGender = tr.Gender,
            CurrentHandler = 1,
            HandlingTrainerFriendship = pi.BaseFriendship,

            ConsoleRegion = geo.ConsoleRegion,
            Country = geo.Country,
            Region = geo.Region,
        };

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        if (pk.IsShiny)
            pk.PID ^= 0x1000_0000;
        criteria.SetRandomIVs(pk, IVs);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames.Span[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language)
    {
        if (Species is (ushort)Core.Species.Farfetchd && nickname is "Quacklin’" or "Quacklin'")
            return true;
        return (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames.Span[language]);
    }
    public string GetNickname(int language) => Nicknames.Span[(uint)language < Nicknames.Length ? language : 0];

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.MetLevel != Level)
            return false;
        if (IVs.IsSpecified)
        {
            if (!Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
                return false;
        }
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
        if (pk is PB8 && expect is 0)
            expect = Locations.Default8bNone;
        return pk.EggLocation == expect;
    }
    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (!Shiny.IsValid(pk))
            return false;
        if (Gender != pk.Gender)
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
