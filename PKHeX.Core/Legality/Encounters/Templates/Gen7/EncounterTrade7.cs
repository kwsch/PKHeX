using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Trade Encounter
/// </summary>
public sealed record EncounterTrade7 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IEncounterConvertible<PK7>, IMemoryOTReadOnly, IFixedGender, IFixedNature
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    public int Location => Locations.LinkTrade6NPC;
    public byte OT_Memory => 1;
    public byte OT_Intensity => 3;
    public byte OT_Feeling => 5;
    public ushort OT_TextVar => 40;
    public Shiny Shiny => Shiny.Never;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public int EggLocation => 0;
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
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.USUM[Species, Form];
        var pk = new PK7
        {
            PID = Util.Rand32(),
            EncryptionConstant = Util.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            Met_Location = Location,
            Met_Level = Level,
            MetDate = EncounterDate.GetDate3DS(),
            Gender = Gender,
            Nature = (byte)Nature,
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = OTGender,
            OT_Name = TrainerNames[lang],

            OT_Memory = OT_Memory,
            OT_Intensity = OT_Intensity,
            OT_Feeling = OT_Feeling,
            OT_TextVar = OT_TextVar,
            OT_Friendship = pi.BaseFriendship,

            IsNicknamed = true,
            Nickname = Nicknames[lang],

            HT_Name = tr.OT,
            HT_Gender = tr.Gender,
            CurrentHandler = 1,
            HT_Friendship = pi.BaseFriendship,
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
        if (pk.Met_Level != Level)
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
        if (pk.OT_Gender != OTGender)
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
        return pk.Egg_Location == expect;
    }
    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (!Shiny.IsValid(pk))
            return false;
        if (Gender != pk.Gender)
            return false;
        if (Nature != Nature.Random && pk.Nature != (int)Nature)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
