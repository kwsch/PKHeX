using System;
using static PKHeX.Core.EncounterGift1.TrainerType;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 1
/// </summary>
public sealed record EncounterGift1 : IEncounterable, IEncounterMatch, IEncounterConvertible<PK1>,
     IMoveset, IFixedIVSet
{
    public const int SerializedSize = 8;

    public byte Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool IsEgg => false;
    public ushort EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public bool IsShiny => false;
    public ushort Location => 0;
    public byte Form => 0;
    public Shiny Shiny => Shiny.Random;

    public Moveset Moves { get; }
    public IndividualValueSet IVs { get; }
    public ushort Species { get; }
    public byte Level { get; }
    public GameVersion Version { get; }
    public LanguageRestriction Language { get; }
    public TrainerType Trainer { get; }

    public string Name => "Event Gift";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public enum LanguageRestriction : byte
    {
        Any = 0,
        Japanese = 1,
        International = 2,
    }

    public enum TrainerType : byte
    {
        Recipient,
        VirtualConsoleMew = 1,
        Stadium = 2,
        EuropeTour = 3,
    }

    private const ushort TrainerIDStadiumJPN = 1999;
    private const ushort TrainerIDStadiumINT = 2000;
    private const ushort TrainerIDVirtualConsoleMew = 2_27_96; // Red/Green (Japan) release date!
    private const string StadiumENG = "STADIUM";
    private const string StadiumJPN = "スタジアム";
    private const string StadiumFRE = "STADE";
    private const string StadiumITA = "STADIO";
    private const string StadiumSPA = "ESTADIO";
    private const string VirtualConsoleMewINT = "GF";
    private const string VirtualConsoleMewJPN = "ゲーフリ";
    private const string FirstTourOT = "YOSHIRA";

    private static bool IsTourOT(ReadOnlySpan<char> str) => str switch
    {
        "YOSHIRA" => true,
        "YOSHIRB" => true,
        "YOSHIBA" => true,
        "YOSHIBB" => true,
        "LINKE" => true,
        "LINKW" => true,
        "LUIGE" => true,
        "LUIGW" => true,
        "LUIGIC" => true,
        "YOSHIC" => true,
        _ => false,
    };

    public EncounterGift1(ReadOnlySpan<byte> data)
    {
        Species = data[0];
        Level = data[1];
        Moves = new Moveset(data[2], data[3], data[4], data[5]);
        Language = (LanguageRestriction)data[6];
        Trainer = (TrainerType)data[7];

        if (Trainer is EuropeTour)
            IVs = new(5, 10, 1, 12, 5, 5);
        else if (Trainer is VirtualConsoleMew)
            IVs = new(15, 15, 15, 15, 15, 15);

        Version = Trainer == Stadium ? GameVersion.Stadium : GameVersion.RB;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var lang = GetLanguage((LanguageID)tr.Language);
        var isJapanese = lang == Japanese;
        var pi = PersonalTable.RB[Species];
        var rand = Util.Rand;
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = LevelMin,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, (int)lang, Generation),
            Type1 = pi.Type1,
            Type2 = pi.Type2,
            DV16 = IVs.IsSpecified ? EncounterUtil.GetDV16(IVs) : EncounterUtil.GetRandomDVs(rand),

            CatchRate = Trainer switch
            {
                Stadium => GorgeousBox, // be nice and give a Gorgeous Box
                _ => pi.CatchRate,
            },
            TID16 = Trainer switch
            {
                Recipient => tr.TID16,
                Stadium => lang == Japanese ? TrainerIDStadiumJPN : TrainerIDStadiumINT,
                VirtualConsoleMew => TrainerIDVirtualConsoleMew,
                _ => (ushort)rand.Next(10, 200),
            },

            OriginalTrainerName = Trainer switch
            {
                Stadium => lang switch
                {
                    Japanese => StadiumJPN,
                    English => StadiumENG,
                    French => StadiumFRE,
                    Italian => StadiumITA,
                    German => StadiumENG, // Same as English
                    Spanish => StadiumSPA,
                    _ => StadiumENG, // shouldn't hit here
                },
                VirtualConsoleMew => lang == Japanese ? VirtualConsoleMewJPN : VirtualConsoleMewINT,
                EuropeTour => FirstTourOT, // YOSHIRA
                _ => EncounterUtil.GetTrainerName(tr, (int)lang),
            },
        };

        pk.SetMoves(Moves);
        pk.ResetPartyStats();
        return pk;
    }

    private LanguageID GetLanguage(LanguageID request)
    {
        if (Language == LanguageRestriction.Japanese)
            return Japanese;

        if (Language == LanguageRestriction.International && request is not (English or French or Italian or German or Spanish))
            return English;

        if (request is Hacked or UNUSED_6 or >= Korean)
            return English;
        return request;
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Language != LanguageRestriction.Any && pk.Japanese != (Language == LanguageRestriction.Japanese))
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (Level > evo.LevelMax)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (IVs.IsSpecified)
        {
            if (Shiny == Shiny.Always && !pk.IsShiny)
                return false;
            if (Shiny == Shiny.Never && pk.IsShiny)
                return false;
            if (pk.Format <= 2)
            {
                if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
                    return false;
            }
        }

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;
        if (!IsTrainerIDValid(pk))
            return false;
        if (!IsTrainerNameValid(pk))
            return false;
        return true;
    }

    private bool IsTrainerNameValid(PKM pk)
    {
        if (Trainer == Recipient)
            return true;

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        if (Trainer == EuropeTour)
            return IsTourOT(trainer);

        var language = pk.Language;
        if (Trainer == VirtualConsoleMew)
            return trainer.SequenceEqual(language == 1 ? VirtualConsoleMewJPN : VirtualConsoleMewINT);

        if (Trainer == Stadium)
        {
            return language switch
            {
                (int)Japanese => trainer.SequenceEqual(StadiumJPN),
                _ => trainer switch
                {
                    StadiumENG => true,
                    StadiumFRE => true,
                    StadiumITA => true,
                    StadiumSPA => true,
                    _ => false,
                },
            };
        }
        return true;
    }

    private bool IsTrainerIDValid(PKM pk) => Trainer switch
    {
        Recipient => true,
        VirtualConsoleMew => pk.TID16 == TrainerIDVirtualConsoleMew,
        Stadium => pk.TID16 == (pk.Japanese ? TrainerIDStadiumJPN : TrainerIDStadiumINT),
        _ => true,
    };

    private bool IsShinyValid(PKM pk) => Shiny switch
    {
        Shiny.Never => !pk.IsShiny,
        Shiny.Always => pk.IsShiny,
        _ => true,
    };

    private static bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format <= 2)
            return true;

        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    private static bool IsMatchLocation(PKM pk)
    {
        // Met Location is not stored in the PK1 format.
        if (pk is ICaughtData2 { CaughtData: not 0 })
            return false;
        return true;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (pk is not PK1 pk1)
            return false;
        return !IsCatchRateValid(pk1.CatchRate);
    }

    private const byte NormalBox = 167;
    private const byte GorgeousBox = 168;

    private bool IsCatchRateValid(byte rate)
    {
        if (ParseSettings.AllowGen1Tradeback && PK1.IsCatchRateHeldItem(rate))
            return true;

        if (Version == GameVersion.Stadium)
        {
            if (Species == (ushort)Core.Species.Psyduck)
                return rate == GorgeousBox;
            return rate is NormalBox or GorgeousBox;
        }

        var pi = PersonalTable.RB[Species];
        return pi.CatchRate == rate;
    }

    #endregion
}
