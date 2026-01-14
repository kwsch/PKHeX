using System;
using static PKHeX.Core.Species;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Static Encounter
/// </summary>
public sealed record EncounterStatic2(ushort Species, byte Level, GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK2>, IHatchCycle, IFixedGender, IMoveset, IFixedIVSet, IEncounterTime
{
    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public byte Form => 0;
    public byte EggCycles => IsDizzyPunchEgg ? (byte)20 : (byte)0;
    public bool IsDizzyPunchEgg => IsEgg && Moves.HasMoves;

    public Ball FixedBall => Ball.Poke;
    ushort ILocation.Location => Location;
    public ushort EggLocation => 0;
    public bool IsShiny => Shiny == Shiny.Always;
    public AbilityPermission Ability => Species != (int)Koffing ? AbilityPermission.OnlyHidden : AbilityPermission.OnlyFirst;
    public bool IsRoaming => Species is (int)Entei or (int)Raikou or (int)Suicune && Location != 23;

    public Shiny Shiny { get; init; } = Shiny.Random;
    public byte Location { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public IndividualValueSet IVs { get; init; }
    public Moveset Moves { get; init; }
    public bool IsEgg { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    private const byte OddEggEXP = 125;
    private const byte UnhatchedEggOTGender = 0;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int language = (int)Language.GetSafeLanguage2((LanguageID)tr.Language);
        var pi = PersonalTable.C[Species];
        var pk = new PK2
        {
            Species = Species,
            CurrentLevel = LevelMin,

            TID16 = tr.TID16,

            OriginalTrainerFriendship = pi.BaseFriendship,
        };
        pk.SetNotNicknamed(language);

        pk.DV16 = IVs.IsSpecified ? EncounterUtil.GetDV16(IVs) :
            criteria.IsSpecifiedIVsAll() ? criteria.GetCombinedDVs() :
            EncounterUtil.GetRandomDVs(Util.Rand, criteria.Shiny.IsShiny(), criteria.HiddenPowerType);

        if (IsEgg)
        {
            // Gender and location not set for regular eggs
            if (IsDizzyPunchEgg) // Odd Egg: Fixed EXP value instead of exactly Level 5
            {
                pk.EXP = OddEggEXP;
                if (pk.IsEgg)
                {
                    pk.OriginalTrainerName = GetOddEggTrainerName((LanguageID)language);
                    pk.OriginalTrainerFriendship = EggCycles;
                }
                else
                {
                    pk.OriginalTrainerName = tr.OT;
                }
            }
            else
            {
                pk.OriginalTrainerName = tr.OT;
                if (pk.IsEgg)
                    pk.OriginalTrainerFriendship = EggCycles;
            }
        }
        else if (Version == C || (Version == GSC && tr.Version == C))
        {
            pk.OriginalTrainerName = tr.OT;
            pk.OriginalTrainerGender = tr.Gender;
            pk.MetLevel = LevelMin;
            pk.MetLocation = Location;
            pk.MetTimeOfDay = GetRandomTime();
        }
        else
        {
            pk.OriginalTrainerName = tr.OT;
        }

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Shiny == Shiny.Always && !pk.IsShiny)
            return false;
        if (IsDizzyPunchEgg) // Odd Egg
        {
            if (pk.Format > 2)
                return false; // Can't be transferred to Gen7+
            if (!pk.HasMove((int)Move.DizzyPunch))
                return false;

            // EXP is a fixed starting value for eggs
            if (pk.IsEgg)
            {
                if (pk.EXP != OddEggEXP)
                    return false;

                // Check OT Details
                if (pk.OriginalTrainerGender != UnhatchedEggOTGender)
                    return false;
                if (!IsOddEggTrainerNameValid(pk))
                    return false;
            }
            else
            {
                // Once hatched, EXP can vary. Must be at least the starting value.
                if (pk.EXP < OddEggEXP)
                    return false;
            }
        }

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
            else
            {
                if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
                    return false;
            }
        }
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private static LanguageID DetectOddEggLanguage(PKM pk)
    {
        // Only called when in Gen2 format, because they can only be transferred after hatching.
        var span = pk.OriginalTrainerTrash;
        Span<char> name = stackalloc char[span.Length];
        var len = pk.LoadString(span, name);
        name = name[..len];
        return DetectOddEggLanguage(name, pk.Japanese);
    }

    private static string GetOddEggTrainerName(LanguageID language) => language switch
    {
        // Japanese language ID can be "なぞナゾ" or "なぞ", but we use the longer form here.
        LanguageID.Japanese => "なぞナゾ",

        // Specific fixed names for other languages.
        LanguageID.English => "ODD",
        LanguageID.French => "BIZAR",
        LanguageID.Italian => "Strano",
        LanguageID.German => "Kurios",
        LanguageID.Spanish => "Raro",
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
    };

    private static LanguageID DetectOddEggLanguage(Span<char> name, bool japanese)
    {
        // Japanese egg names can only be "なぞナゾ" or "なぞ"
        // For the Japanese OT, it's initially set to なぞナゾ for the initial "post-trade" automatic save,
        // but the last two characters are then removed so that any subsequent saves have なぞ instead.
        // https://github.com/gb-mobile/pokecrystal-mobile-eng/blob/5ab6cd0617c4597400aeb963220747c8c778b1d6/mobile/mobile_45_stadium.asm#L133
        // Thus, both forms are valid for Japanese.
        if (japanese)
            return name is "なぞナゾ" or "なぞ" ? LanguageID.Japanese : LanguageID.None;

        // Other languages have fixed OT names, but can trade with different language games.
        // Thus, we only check for the known valid names.
        return name switch
        {
            "ODD" => LanguageID.English,
            "BIZAR" => LanguageID.French,
            "Strano" => LanguageID.Italian,
            "Kurios" => LanguageID.German,
            "Raro" => LanguageID.Spanish,
            _ => LanguageID.None,
        };
    }

    private static bool IsOddEggTrainerNameValid(PKM pk) => DetectOddEggLanguage(pk) != LanguageID.None;

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

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (evo.LevelMax < Level)
            return false;
        if (pk is ICaughtData2 { CaughtData: not 0 })
            return pk.MetLevel == (IsEgg ? 1 : Level);
        return true;
    }

    // Routes 29-46, except 40 & 41; total 16.
    // 02, 04, 05, 08, 11, 15, 18, 20,
    // 21, 25, 26, 34, 37, 39, 43, 45,
    private const ulong RoamLocations = 0b10_1000_1010_0100_0000_0110_0011_0100_1000_1001_0011_0100;

    private bool IsMatchLocation(PKM pk)
    {
        if (IsEgg)
            return true; // already checked by Egg Location check
        if (pk is not ICaughtData2 c2)
            return true;
        if (c2.CaughtData is 0 && Version != C)
            return true; // GS

        if (IsRoaming)
        {
            // Gen2 met location is always u8
            var loc = c2.MetLocation;
            return loc <= 45 && ((RoamLocations & (1UL << loc)) != 0);
        }
        if (Version is C or GSC)
        {
            if (c2.CaughtData is not 0)
                return Location == pk.MetLocation;
            if (Species == (int)Celebi)
                return false; // Cannot reset the Met data
        }
        return true;
    }

    #endregion

    public EncounterTime EncounterTime => EncounterTime.Any;

    public int GetRandomTime()
    {
        if (IsEgg)
            return 0;
        return EncounterTime.RandomValidTime();
    }
}
