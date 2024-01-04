using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 LGP/E Trade Encounter
/// </summary>
public sealed record EncounterTrade7b(GameVersion Version) : IEncounterable, IEncounterMatch, IFixedTrainer, IEncounterConvertible<PB7>
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    public int Location => Locations.LinkTrade6NPC;
    public Shiny Shiny => Shiny.Random;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsFixedTrainer => true;
    public AbilityPermission Ability => AbilityPermission.Any12;

    public required string[] TrainerNames { get; init; }

    public required uint ID32 { get; init; }
    public required byte OTGender { get; init; }
    public required ushort Species { get; init; }
    public required byte Form { get; init; }
    public required byte Level { get; init; }

    public required IndividualValueSet IVs { get; init; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PB7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.GG[Species, Form];
        var pk = new PB7
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            Met_Location = Location,
            Met_Level = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = OTGender,
            OT_Name = TrainerNames[lang],

            OT_Friendship = pi.BaseFriendship,

            HeightScalar = PokeSizeUtil.GetRandomScalar(),
            WeightScalar = PokeSizeUtil.GetRandomScalar(),

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),

            HT_Name = tr.OT,
            HT_Gender = tr.Gender,
            CurrentHandler = 1,
            HT_Friendship = pi.BaseFriendship,
        };

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        pk.ResetHeight();
        pk.ResetWeight();
        pk.ResetCP();
        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PB7 pk, EncounterCriteria criteria, PersonalInfo7GG pi)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        criteria.SetRandomIVs(pk, IVs);
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames[language]);

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.Met_Level != Level)
            return false;
        if (IVs.IsSpecified)
        {
            if (!Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
                return false;
        }
        if (pk.ID32 != ID32)
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.OT_Gender != OTGender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.Egg_Location == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
