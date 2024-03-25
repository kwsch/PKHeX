using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Trade Encounter
/// </summary>
public sealed record EncounterTrade7 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IEncounterConvertible<PK7>, IMemoryOTReadOnly, IFixedGender, IFixedNature
{
    public byte Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    public ushort Location => Locations.LinkTrade6NPC;
    public byte OriginalTrainerMemory => 1;
    public byte OriginalTrainerMemoryIntensity => 3;
    public byte OriginalTrainerMemoryFeeling => 5;
    public ushort OriginalTrainerMemoryVariable => 40;
    public Shiny Shiny => Shiny.Never;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;

    private string[] TrainerNames { get; }
    private string[] Nicknames { get; }

    public required Nature Nature { get; init; }
    public required uint ID32 { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte Gender { get; init; }
    public required byte OTGender { get; init; }

    public required IndividualValueSet IVs { get; init; }
    public required ushort Species { get; init; }
    public required byte Form { get; init; }

    public required byte Level { get; init; }
    public GameVersion Version { get; }
    public bool EvolveOnTrade { get; init; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public EncounterTrade7(ReadOnlySpan<string[]> names, byte index, GameVersion version)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Version = version;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.USUM[Species, Form];
        var rnd = Util.Rand;
        var pk = new PK7
        {
            PID = rnd.Rand32(),
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDate3DS(),
            Gender = Gender,
            Nature = Nature,
            Ball = (byte)FixedBall,

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

            IsNicknamed = true,
            Nickname = Nicknames[lang],

            HandlingTrainerName = tr.OT,
            HandlingTrainerGender = tr.Gender,
            CurrentHandler = 1,
            HandlingTrainerFriendship = pi.BaseFriendship,
        };
        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins(lang);

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        if (pk.IsShiny)
            pk.PID ^= 0x1000_0000;
        criteria.SetRandomIVs(pk, IVs);
        if (EvolveOnTrade)
            pk.Species++;

        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.ResetPartyStats();

        return pk;
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
        if (EvolveOnTrade && pk.Species == Species)
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
        if (Gender != pk.Gender)
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
