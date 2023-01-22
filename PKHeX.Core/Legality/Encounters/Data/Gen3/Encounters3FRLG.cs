using System;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Encounters
/// </summary>
internal static class Encounters3FRLG
{
    internal static readonly EncounterArea3[] SlotsFR = GetRegular("fr", "fr", FR);
    internal static readonly EncounterArea3[] SlotsLG = GetRegular("lg", "lg", LG);

    private static EncounterArea3[] GetRegular(string resource, string ident, GameVersion game) => EncounterArea3.GetAreas(Get(resource, ident), game);

    static Encounters3FRLG() => MarkEncounterTradeStrings(TradeGift_FRLG, TradeFRLG);

    private static readonly EncounterStatic3[] Encounter_FRLG_Roam =
    {
        new(243, 50, FRLG) { Roaming = true, Location = 16 }, // Raikou
        new(244, 50, FRLG) { Roaming = true, Location = 16 }, // Entei
        new(245, 50, FRLG) { Roaming = true, Location = 16 }, // Suicune
    };

    private static readonly EncounterStatic3[] Encounter_FRLG_Stationary =
    {
        // Starters @ Pallet Town
        new(001, 05, FRLG) { Gift = true, Location = 088 }, // Bulbasaur
        new(004, 05, FRLG) { Gift = true, Location = 088 }, // Charmander
        new(007, 05, FRLG) { Gift = true, Location = 088 }, // Squirtle

        // Fossil @ Cinnabar Island
        new(138, 05, FRLG) { Gift = true, Location = 096 }, // Omanyte
        new(140, 05, FRLG) { Gift = true, Location = 096 }, // Kabuto
        new(142, 05, FRLG) { Gift = true, Location = 096 }, // Aerodactyl

        // Gift
        new(106, 25, FRLG) { Gift = true, Location = 098 }, // Hitmonlee @ Saffron City
        new(107, 25, FRLG) { Gift = true, Location = 098 }, // Hitmonchan @ Saffron City
        new(129, 05, FRLG) { Gift = true, Location = 099 }, // Magikarp @ Route 4
        new(131, 25, FRLG) { Gift = true, Location = 134 }, // Lapras @ Silph Co.
        new(133, 25, FRLG) { Gift = true, Location = 094 }, // Eevee @ Celadon City
        new(175, 05, FRLG) { Gift = true, EggLocation = 253 }, // Togepi Egg

        // Celadon City Game Corner
        new(063, 09, FR) { Gift = true, Location = 94 }, // Abra
        new(035, 08, FR) { Gift = true, Location = 94 }, // Clefairy
        new(123, 25, FR) { Gift = true, Location = 94 }, // Scyther
        new(147, 18, FR) { Gift = true, Location = 94 }, // Dratini
        new(137, 26, FR) { Gift = true, Location = 94 }, // Porygon

        new(063, 07, LG) { Gift = true, Location = 94 }, // Abra
        new(035, 12, LG) { Gift = true, Location = 94 }, // Clefairy
        new(127, 18, LG) { Gift = true, Location = 94 }, // Pinsir
        new(147, 24, LG) { Gift = true, Location = 94 }, // Dratini
        new(137, 18, LG) { Gift = true, Location = 94 }, // Porygon

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
        new(386, 30, FR  ) { Location = 187, FatefulEncounter = true, Form = 1 }, // Deoxys @ Birth Island
        new(386, 30,   LG) { Location = 187, FatefulEncounter = true, Form = 2 }, // Deoxys @ Birth Island
    };

    private static readonly EncounterStatic3[] Encounter_FRLG = ArrayUtil.ConcatAll(Encounter_FRLG_Roam, Encounter_FRLG_Stationary);

    private static ReadOnlySpan<byte> TradeContest_Cool   => new byte[] { 30, 05, 05, 05, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Beauty => new byte[] { 05, 30, 05, 05, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Cute   => new byte[] { 05, 05, 30, 05, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Clever => new byte[] { 05, 05, 05, 30, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Tough  => new byte[] { 05, 05, 05, 05, 30, 10 };

    internal static readonly EncounterTrade3[] TradeGift_FRLG =
    {
        new(FRLG, 0x00009CAE, 122, 05) { Ability = OnlyFirst,  TID16 = 01985, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(20,15,17,24,23,22), Contest = TradeContest_Clever }, // Abra (Level 5 Breeding) -> Mr. Mime
        new(FR  , 0x4C970B89, 029, 05) { Ability = OnlyFirst,  TID16 = 63184, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(22,18,25,19,15,22), Contest = TradeContest_Tough }, // Nidoran♀
        new(  LG, 0x4C970B9E, 032, 05) { Ability = OnlyFirst,  TID16 = 63184, SID16 = 00000, OTGender = 1, Gender = 0, IVs = new(19,25,18,22,22,15), Contest = TradeContest_Cool }, // Nidoran♂ *
        new(FR  , 0x00EECA15, 030, 16) { Ability = OnlyFirst,  TID16 = 13637, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(22,25,18,19,22,15), Contest = TradeContest_Cute }, // Nidorina *
        new(  LG, 0x00EECA19, 033, 16) { Ability = OnlyFirst,  TID16 = 13637, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(19,18,25,22,15,22), Contest = TradeContest_Tough }, // Nidorino  *
        new(FR  , 0x451308AB, 108, 25) { Ability = OnlyFirst,  TID16 = 01239, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(24,19,21,15,23,21), Contest = TradeContest_Tough }, // Golduck (Level 25) -> Lickitung  *
        new(  LG, 0x451308AB, 108, 25) { Ability = OnlyFirst,  TID16 = 01239, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(24,19,21,15,23,21), Contest = TradeContest_Tough }, // Slowbro (Level 25) -> Lickitung  *
        new(FRLG, 0x498A2E1D, 124, 20) { Ability = OnlyFirst,  TID16 = 36728, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(18,17,18,22,25,21), Contest = TradeContest_Beauty }, // Poliwhirl (Level 20) -> Jynx
        new(FRLG, 0x151943D7, 083, 03) { Ability = OnlyFirst,  TID16 = 08810, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(20,25,21,24,15,20), Contest = TradeContest_Cool }, // Spearow (Level 3 Capture) -> Farfetch'd
        new(FRLG, 0x06341016, 101, 03) { Ability = OnlySecond, TID16 = 50298, SID16 = 00000, OTGender = 0, Gender = 2, IVs = new(19,16,18,25,25,19), Contest = TradeContest_Cool }, // Raichu (Level 3) -> Electrode
        new(FRLG, 0x5C77ECFA, 114, 05) { Ability = OnlyFirst,  TID16 = 60042, SID16 = 00000, OTGender = 1, Gender = 0, IVs = new(22,17,25,16,23,20), Contest = TradeContest_Cute }, // Venonat (Level 5 Breeding) -> Tangela
        new(FRLG, 0x482CAC89, 086, 05) { Ability = OnlyFirst,  TID16 = 09853, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(24,15,22,16,23,22), Contest = TradeContest_Tough }, // Ponyta (Level 5 Breeding) -> Seel *
        //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
    };

    private const string tradeFRLG = "tradefrlg";
    private static readonly string[][] TradeFRLG = Util.GetLanguageStrings7(tradeFRLG);

    internal static readonly EncounterStatic3[] StaticFR = GetEncounters(Encounter_FRLG, FR);
    internal static readonly EncounterStatic3[] StaticLG = GetEncounters(Encounter_FRLG, LG);
}
