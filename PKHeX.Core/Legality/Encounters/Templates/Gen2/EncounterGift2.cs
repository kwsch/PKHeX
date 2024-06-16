using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LanguageID;
using static PKHeX.Core.EncounterGift2.TrainerType;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 2
/// </summary>
public sealed record EncounterGift2
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK2>, IHatchCycle, IMoveset, IFixedIVSet
{
    public const int SerializedSize = 12;

    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public byte Form => 0;
    public Ball FixedBall => Ball.Poke;
    ushort ILocation.Location => Location;
    public ushort EggLocation => 0;
    public bool IsShiny => Shiny == Shiny.Always;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public bool IsEgg => EggCycles != 0;

    public Moveset Moves { get; }
    public IndividualValueSet IVs => default; // future?
    public ushort Species { get; }
    public byte Level { get; }
    public GameVersion Version { get; }
    public TrainerType Trainer { get; }
    public byte CurrentLevel { get; }
    public byte EggCycles { get; }
    public byte Location { get; }
    public Shiny Shiny { get; }
    public LanguageRestriction Language { get; }

    public string Name => "GB Era Event Gift";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public enum LanguageRestriction : byte
    {
        International = 0,
        Japanese = 1,
        English = 2,
        InternationalNotEnglish = 3,
    }

    public enum TrainerType : byte
    {
        Recipient,
        GiftStadiumJPN = 1,
        GiftStadiumENG = 2,
        GiftStadiumINT = 3,
        PokemonCenterNewYork = 4,
    }

    private const ushort TrainerIDStadiumJPN = 2000;
    private const ushort TrainerIDStadiumENG = 2000;
    private const ushort TrainerIDStadiumINT = 2001;
    private const string StadiumJPN = "スタジアム";
    private const string StadiumENG = "Stadium";
    private const string StadiumFRE = "Stade";
    private const string StadiumGER = "Stadion";
    private const string StadiumITA = "Stadio";
    private const string StadiumSPA = "Estadio";
    private const string FirstPCNY = "PCNYa";

    public static bool IsTrainerPCNY(ReadOnlySpan<char> str) => str is "PCNYa" or "PCNYb" or "PCNYc" or "PCNYd";

    public EncounterGift2(ReadOnlySpan<byte> data)
    {
        Species = data[0];
        Level = data[1];
        Moves = new(data[2], data[3], data[4], data[5]);
        Location = data[6];
        CurrentLevel = data[7];
        Shiny = data[8] == 0 ? Shiny.Random : Shiny.Always;
        EggCycles = data[9] == 1 ? (byte)10 : (byte)0;
        Language = (LanguageRestriction)data[10];
        Trainer = (TrainerType)data[11];

        Version = Location != 0 ? GameVersion.C : GameVersion.GS;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var lang = GetLanguage((LanguageID)tr.Language);
        var pi = PersonalTable.C[Species];
        var pk = new PK2
        {
            Species = Species,
            CurrentLevel = CurrentLevel == 0 ? LevelMin : CurrentLevel,
            OriginalTrainerFriendship = pi.BaseFriendship,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, (int)lang, Generation),

            TID16 = Trainer switch
            {
                Recipient => tr.TID16,
                GiftStadiumJPN => TrainerIDStadiumJPN,
                GiftStadiumENG => TrainerIDStadiumENG,
                GiftStadiumINT => TrainerIDStadiumINT,
                _ => (ushort)Util.Rand.Next(10, 200),
            },
            OriginalTrainerName = Trainer switch
            {
                Recipient => EncounterUtil.GetTrainerName(tr, (int)lang),
                GiftStadiumJPN => StadiumJPN,
                GiftStadiumENG => StadiumENG,
                GiftStadiumINT => lang switch
                {
                    French => StadiumFRE,
                    Italian => StadiumITA,
                    German => StadiumGER,
                    Spanish => StadiumSPA,
                    _ => StadiumENG, // shouldn't hit here
                },
                PokemonCenterNewYork => FirstPCNY,
                _ => EncounterUtil.GetTrainerName(tr, 1),
            },
        };

        if (IsEgg)
        {
            // Fake as hatched on G/S.
        }
        else
        {
            pk.MetLevel = LevelMin;
            pk.MetLocation = Location;
            //pk.MetTimeOfDay = 0;
        }

        if (Shiny == Shiny.Always)
            pk.SetShiny();
        pk.SetMoves(Moves);
        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk);

        pk.ResetPartyStats();
        return pk;
    }

    private LanguageID GetLanguage(LanguageID request)
    {
        if (Language == LanguageRestriction.Japanese)
            return Japanese;
        if (Language == LanguageRestriction.English)
            return English;
        if (Language == LanguageRestriction.International && request is not (English or French or Italian or German or Spanish))
            return English;
        if (Language == LanguageRestriction.InternationalNotEnglish && request is not (French or Italian or German or Spanish))
            return French;

        if (request is Hacked or UNUSED_6 or >= Korean)
            return English;
        return request;
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Shiny == Shiny.Always && !pk.IsShiny)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
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
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (!IsLanguageValid(pk.Language))
            return false;

        if (CurrentLevel != 0 && CurrentLevel > pk.CurrentLevel)
            return false;

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;

        if (IsEgg && !pk.IsEgg)
            return true;

        // Check OT Details
        if (!IsTrainerIDValid(pk))
            return false;
        if (!IsTrainerNameValid(pk))
            return false;
        return true;
    }

    private bool IsLanguageValid(int pkLanguage)
    {
        if (pkLanguage == (int)Japanese)
            return Language is LanguageRestriction.Japanese;
        return Language is not LanguageRestriction.Japanese;
    }

    private bool IsTrainerNameValid(PKM pk) => Trainer switch
    {
        Recipient => true,
        GiftStadiumINT => pk.OriginalTrainerName switch
        {
            StadiumGER => true,
            StadiumFRE => true,
            StadiumITA => true,
            StadiumSPA => true,
            _ => false,
        },
        GiftStadiumJPN => IsTrainerName(pk, StadiumJPN),
        GiftStadiumENG => IsTrainerName(pk, StadiumENG),
        PokemonCenterNewYork => IsTrainerPCNY(pk),
        _ => true,
    };

    private static bool IsTrainerPCNY(PKM pk)
    {
        Span<char> ot = stackalloc char[pk.MaxStringLengthTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, ot);
        return IsTrainerPCNY(ot[..len]);
    }

    private static bool IsTrainerName(PKM pk, [ConstantExpected] string name)
    {
        Span<char> ot = stackalloc char[pk.MaxStringLengthTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, ot);
        return ot[..len].SequenceEqual(name);
    }

    private bool IsTrainerIDValid(ITrainerID16 pk) => Trainer switch
    {
        Recipient => true,
        GiftStadiumJPN => pk.TID16 == TrainerIDStadiumJPN,
        GiftStadiumENG => pk.TID16 == TrainerIDStadiumENG,
        GiftStadiumINT => pk.TID16 == TrainerIDStadiumINT,
        _ => true,
    };

    private bool IsMatchEggLocation(PKM pk)
    {
        if (pk is not ICaughtData2 c2)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.EggLocation == expect;
        }

        if (pk.IsEgg)
        {
            if (!IsEgg)
                return false;
            if (c2.MetLocation != 0 && c2.MetLevel != 0)
                return false;
            if (pk.OriginalTrainerFriendship > EggCycles)
                return false;
        }
        else
        {
            switch (c2.MetLevel)
            {
                case 0 when c2.MetLocation != 0:
                    return false;
                case 1: // 0 = second floor of every Pokémon Center, valid
                    return true;
                default:
                    if (pk.MetLocation == 0 && c2.MetLevel != 0)
                        return false;
                    break;
            }
        }

        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (IsEgg)
            return true;
        if (pk is not ICaughtData2 c2)
            return true;

        if (Version is GameVersion.C or GameVersion.GSC)
        {
            if (c2.CaughtData is not 0)
                return Location == pk.MetLocation;
            if (pk.Species == (int)Core.Species.Celebi)
                return false; // Cannot reset the Met data
        }
        return true;
    }

    private bool IsShinyValid(PKM pk) => Shiny switch
    {
        Shiny.Never => !pk.IsShiny,
        Shiny.Always => pk.IsShiny,
        _ => true,
    };

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (evo.LevelMax < Level)
            return false;
        if (pk is ICaughtData2 { CaughtData: not 0 })
            return pk.MetLevel == (IsEgg ? 1 : Level);
        return true;
    }

    #endregion
}
