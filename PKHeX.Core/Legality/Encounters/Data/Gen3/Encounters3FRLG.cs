using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Encounters
/// </summary>
internal static class Encounters3FRLG
{
    internal static readonly EncounterArea3[] SlotsFR = GetRegular("fr", "fr"u8, FR);
    internal static readonly EncounterArea3[] SlotsLG = GetRegular("lg", "lg"u8, LG);

    private static EncounterArea3[] GetRegular([ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident, [ConstantExpected] GameVersion game) => EncounterArea3.GetAreas(Get(resource, ident), game);

    private const string tradeFRLG = "tradefrlg";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings7(tradeFRLG);

    public static readonly EncounterStatic3[] StaticFRLG =
    [
        new(243, 50, FRLG) { Roaming = true, Location = 101 }, // Raikou
        new(244, 50, FRLG) { Roaming = true, Location = 101 }, // Entei
        new(245, 50, FRLG) { Roaming = true, Location = 101 }, // Suicune

        // Starters @ Pallet Town
        new(001, 05, FRLG) { FixedBall = Ball.Poke, Location = 088 }, // Bulbasaur
        new(004, 05, FRLG) { FixedBall = Ball.Poke, Location = 088 }, // Charmander
        new(007, 05, FRLG) { FixedBall = Ball.Poke, Location = 088 }, // Squirtle

        // Fossil @ Cinnabar Island
        new(138, 05, FRLG) { FixedBall = Ball.Poke, Location = 096 }, // Omanyte
        new(140, 05, FRLG) { FixedBall = Ball.Poke, Location = 096 }, // Kabuto
        new(142, 05, FRLG) { FixedBall = Ball.Poke, Location = 096 }, // Aerodactyl

        // Gift
        new(106, 25, FRLG) { FixedBall = Ball.Poke, Location = 098 }, // Hitmonlee @ Saffron City
        new(107, 25, FRLG) { FixedBall = Ball.Poke, Location = 098 }, // Hitmonchan @ Saffron City
        new(129, 05, FRLG) { FixedBall = Ball.Poke, Location = 099 }, // Magikarp @ Route 4
        new(131, 25, FRLG) { FixedBall = Ball.Poke, Location = 134 }, // Lapras @ Silph Co.
        new(133, 25, FRLG) { FixedBall = Ball.Poke, Location = 094 }, // Eevee @ Celadon City
        new(175, 05, FRLG) { FixedBall = Ball.Poke, Location = 253, IsEgg = true, Moves = new(045,204,118) }, // Togepi Egg

        // Stationary
        new(143, 30, FRLG) { Location = 112 }, // Snorlax @ Route 12
        new(143, 30, FRLG) { Location = 116 }, // Snorlax @ Route 16
        new(101, 34, FRLG) { Location = 142 }, // Electrode @ Power Plant
        new(097, 30, FRLG) { Location = 176 }, // Hypno @ Berry Forest

        // Stationary Legendary
        new(144, 50, FRLG) { Location = 139 }, // Articuno @ Seafoam Islands
        new(145, 50, FRLG) { Location = 142 }, // Zapdos @ Power Plant
        new(146, 50, FRLG) { Location = 175 }, // Moltres @ Mt. Ember.
        new(150, 70, FRLG) { Location = 141 }, // Mewtwo @ Cerulean Cave

        // Event
        new(249, 70, FRLG) { Location = 174, FatefulEncounter = true }, // Lugia @ Navel Rock
        new(250, 70, FRLG) { Location = 174, FatefulEncounter = true }, // Ho-Oh @ Navel Rock
    ];

    public static readonly EncounterStatic3[] StaticFR =
    [
        // Celadon City Game Corner
        new(063, 09, FR) { FixedBall = Ball.Poke, Location = 94 }, // Abra
        new(035, 08, FR) { FixedBall = Ball.Poke, Location = 94 }, // Clefairy
        new(123, 25, FR) { FixedBall = Ball.Poke, Location = 94 }, // Scyther
        new(147, 18, FR) { FixedBall = Ball.Poke, Location = 94 }, // Dratini
        new(137, 26, FR) { FixedBall = Ball.Poke, Location = 94 }, // Porygon

        new(386, 30, FR  ) { Location = 187, FatefulEncounter = true, Form = 1 }, // Deoxys @ Birth Island
    ];

    public static readonly EncounterStatic3[] StaticLG =
    [
        // Celadon City Game Corner
        new(063, 09, FR) { FixedBall = Ball.Poke, Location = 94 }, // Abra
        new(035, 08, FR) { FixedBall = Ball.Poke, Location = 94 }, // Clefairy
        new(123, 25, FR) { FixedBall = Ball.Poke, Location = 94 }, // Scyther
        new(147, 18, FR) { FixedBall = Ball.Poke, Location = 94 }, // Dratini
        new(137, 26, FR) { FixedBall = Ball.Poke, Location = 94 }, // Porygon

        new(063, 07, LG) { FixedBall = Ball.Poke, Location = 94 }, // Abra
        new(035, 12, LG) { FixedBall = Ball.Poke, Location = 94 }, // Clefairy
        new(127, 18, LG) { FixedBall = Ball.Poke, Location = 94 }, // Pinsir
        new(147, 24, LG) { FixedBall = Ball.Poke, Location = 94 }, // Dratini
        new(137, 18, LG) { FixedBall = Ball.Poke, Location = 94 }, // Porygon
        new(386, 30,   LG) { Location = 187, FatefulEncounter = true, Form = 2 }, // Deoxys @ Birth Island
    ];

    private static ReadOnlySpan<byte> TradeContest_Cool   => [ 30, 05, 05, 05, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Beauty => [ 05, 30, 05, 05, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Cute   => [ 05, 05, 30, 05, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Clever => [ 05, 05, 05, 30, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Tough  => [ 05, 05, 05, 05, 30, 10 ];

    internal static readonly EncounterTrade3[] TradeGift_FRLG =
    [
        new(TradeNames, 00, FRLG, 0x00009CAE, 122, 05) { Ability = OnlyFirst,  TID16 = 01985, OTGender = 0, Gender = 0, IVs = new(20,15,17,24,23,22), Contest = TradeContest_Clever }, // Abra (Level 5 Breeding) -> Mr. Mime
        new(TradeNames, 07, FRLG, 0x498A2E1D, 124, 20) { Ability = OnlyFirst,  TID16 = 36728, OTGender = 0, Gender = 1, IVs = new(18,17,18,22,25,21), Contest = TradeContest_Beauty }, // Poliwhirl (Level 20) -> Jynx
        new(TradeNames, 08, FRLG, 0x151943D7, 083, 03) { Ability = OnlyFirst,  TID16 = 08810, OTGender = 0, Gender = 0, IVs = new(20,25,21,24,15,20), Contest = TradeContest_Cool }, // Spearow (Level 3 Capture) -> Farfetch'd
        new(TradeNames, 09, FRLG, 0x06341016, 101, 03) { Ability = OnlySecond, TID16 = 50298, OTGender = 0, Gender = 2, IVs = new(19,16,18,25,25,19), Contest = TradeContest_Cool }, // Raichu (Level 3) -> Electrode
        new(TradeNames, 10, FRLG, 0x5C77ECFA, 114, 05) { Ability = OnlyFirst,  TID16 = 60042, OTGender = 1, Gender = 0, IVs = new(22,17,25,16,23,20), Contest = TradeContest_Cute }, // Venonat (Level 5 Breeding) -> Tangela
        new(TradeNames, 11, FRLG, 0x482CAC89, 086, 05) { Ability = OnlyFirst,  TID16 = 09853, OTGender = 0, Gender = 0, IVs = new(24,15,22,16,23,22), Contest = TradeContest_Tough }, // Ponyta (Level 5 Breeding) -> Seel *
        //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
    ];

    internal static readonly EncounterTrade3[] TradeGift_FR =
    [
        new(TradeNames, 01, FR  , 0x4C970B89, 029, 05) { Ability = OnlyFirst,  TID16 = 63184, OTGender = 1, Gender = 1, IVs = new(22,18,25,19,15,22), Contest = TradeContest_Tough }, // Nidoran♀
        new(TradeNames, 03, FR  , 0x00EECA15, 030, 16) { Ability = OnlyFirst,  TID16 = 13637, OTGender = 0, Gender = 1, IVs = new(22,25,18,19,22,15), Contest = TradeContest_Cute }, // Nidorina *
        new(TradeNames, 05, FR  , 0x451308AB, 108, 25) { Ability = OnlyFirst,  TID16 = 01239, OTGender = 0, Gender = 0, IVs = new(24,19,21,15,23,21), Contest = TradeContest_Tough }, // Golduck (Level 25) -> Lickitung  *
        //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
    ];

    internal static readonly EncounterTrade3[] TradeGift_LG =
    [
        new(TradeNames, 02,   LG, 0x4C970B9E, 032, 05) { Ability = OnlyFirst,  TID16 = 63184, OTGender = 1, Gender = 0, IVs = new(19,25,18,22,22,15), Contest = TradeContest_Cool }, // Nidoran♂ *
        new(TradeNames, 04,   LG, 0x00EECA19, 033, 16) { Ability = OnlyFirst,  TID16 = 13637, OTGender = 0, Gender = 0, IVs = new(19,18,25,22,15,22), Contest = TradeContest_Tough }, // Nidorino  *
        new(TradeNames, 06,   LG, 0x451308AB, 108, 25) { Ability = OnlyFirst,  TID16 = 01239, OTGender = 0, Gender = 0, IVs = new(24,19,21,15,23,21), Contest = TradeContest_Tough }, // Slowbro (Level 25) -> Lickitung  *
        //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
    ];
}
