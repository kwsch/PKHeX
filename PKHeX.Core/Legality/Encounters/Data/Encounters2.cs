using System;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.EncounterGBLanguage;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Encounters
    /// </summary>
    internal static class Encounters2
    {
        internal static readonly EncounterArea2[] SlotsGD = Get("gold", "g2", GD);
        internal static readonly EncounterArea2[] SlotsSV = Get("silver", "g2", SV);
        internal static readonly EncounterArea2[] SlotsC = Get("crystal", "g2", C);

        internal static readonly EncounterArea2[] SlotsGS = ArrayUtil.ConcatAll(SlotsGD, SlotsSV);
        internal static readonly EncounterArea2[] SlotsGSC = ArrayUtil.ConcatAll(SlotsGS, SlotsC);
        private static EncounterArea2[] Get(string name, string ident, GameVersion game) =>
            EncounterArea2.GetAreas(BinLinker.Unpack(Util.GetBinaryResource($"encounter_{name}.pkl"), ident), game);

        static Encounters2() => MarkEncounterTradeStrings(TradeGift_GSC, TradeGift_GSC_OTs);

        private static readonly EncounterStatic2[] Encounter_GSC_Common =
        {
            new(152, 05, GSC) { Location = 001 }, // Chikorita @ New Bark Town
            new(155, 05, GSC) { Location = 001 }, // Cyndaquil @ New Bark Town
            new(158, 05, GSC) { Location = 001 }, // Totodile @ New Bark Town

            new(175, 05, GSC) { EggLocation = 256 }, // Togepi
            new(131, 20, GSC) { Location = 010 }, // Lapras @ Union Cave
            new(133, 20, GSC) { Location = 016 }, // Eevee @ Goldenrod City

            new(185, 20, GSC) { Location = 020 }, // Sudowoodo @ Route 36
            new(236, 10, GSC) { Location = 035 }, // Tyrogue @ Mt. Mortar

            new(130, 30, GSC) { Location = 038, Shiny = Shiny.Always, }, // Gyarados @ Lake of Rage
            new(074, 21, GSC) { Location = 036 }, // Geodude @ Rocket Hideout (Mahogany Town)
            new(109, 21, GSC) { Location = 036 }, // Koffing @ Rocket Hideout (Mahogany Town)
            new(100, 23, GSC) { Location = 036 }, // Voltorb @ Rocket Hideout (Mahogany Town)
            new(101, 23, GSC) { Location = 036 }, // Electrode @ Rocket Hideout (Mahogany Town)
            new(143, 50, GSC) { Location = 061 }, // Snorlax @ Vermillion City

            new(211, 05, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Old Rod)
            new(211, 20, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Good Rod)
            new(211, 40, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Super Rod)
        };

        private static readonly EncounterStatic2[] Encounter_GS_Exclusive =
        {
            new(245, 40, GS), // Suicune

            new(249, 70, GD), // Lugia @ Whirl Islands
            new(249, 40, SV), // Lugia @ Whirl Islands

            new(250, 40, GD), // Ho-Oh @ Tin Tower
            new(250, 70, SV), // Ho-Oh @ Tin Tower

            new(137, 15, GS), // Porygon @ Celadon Game Corner
            new(133, 15, GS), // Eevee @ Celadon Game Corner
            new(122, 15, GS), // Mr. Mime @ Celadon Game Corner

            new(063, 10, GS), // Abra @ Goldenrod City (Game Corner)
            new(147, 10, GS), // Dratini @ Goldenrod City (Game Corner)
            new(023, 10, GS), // Ekans @ Goldenrod City (Game Corner) (Gold)
            new(027, 10, GS), // Sandshrew @ Goldenrod City (Game Corner) (Silver)

            new(223, 05, GS), // Remoraid Swarm @ Route 44 (Old Rod)
            new(223, 20, GS), // Remoraid Swarm @ Route 44 (Good Rod)
            new(223, 40, GS), // Remoraid Swarm @ Route 44 (Super Rod)
        };

        private static readonly EncounterStatic2[] Encounter_C_Exclusive =
        {
            new(245, 40, C) { Location = 023 }, // Suicune @ Tin Tower

            new EncounterStatic2Odd(172), // Pichu Dizzy Punch
            new EncounterStatic2Odd(173), // Cleffa Dizzy Punch
            new EncounterStatic2Odd(174), // Igglybuff Dizzy Punch
            new EncounterStatic2Odd(236), // Tyrogue Dizzy Punch
            new EncounterStatic2Odd(238), // Smoochum Dizzy Punch
            new EncounterStatic2Odd(239), // Elekid Dizzy Punch
            new EncounterStatic2Odd(240), // Magby Dizzy Punch

            new(147, 15, C) { Location = 042, Moves = new [] {245} }, // Dratini ExtremeSpeed

            new(249, 60, C) { Location = 031 }, // Lugia @ Whirl Islands
            new(250, 60, C) { Location = 023 }, // Ho-Oh @ Tin Tower

            new(137, 15, C) { Location = 071 }, // Porygon @ Celadon Game Corner
            new(025, 25, C) { Location = 071 }, // Pikachu @ Celadon Game Corner
            new(246, 40, C) { Location = 071 }, // Larvitar @ Celadon Game Corner

            new(063, 05, C) { Location = 016 }, // Abra @ Goldenrod City (Game Corner)
            new(104, 15, C) { Location = 016 }, // Cubone @ Goldenrod City (Game Corner)
            new(202, 15, C) { Location = 016 }, // Wobbuffet @ Goldenrod City (Game Corner)
        };

        private static readonly EncounterStatic2[] Encounter_GSC_Roam =
        {
            new EncounterStatic2Roam(243, 40, GSC), // Raikou
            new EncounterStatic2Roam(244, 40, GSC), // Entei
            new EncounterStatic2Roam(245, 40, GS), // Suicune
        };

        private static readonly EncounterStatic2[] Encounter_GS = ArrayUtil.ConcatAll(Encounter_GSC_Common, Encounter_GS_Exclusive, Encounter_GSC_Roam);
        private static readonly EncounterStatic2[] Encounter_C = ArrayUtil.ConcatAll(Encounter_GSC_Common, Encounter_C_Exclusive, Encounter_GSC_Roam.AsSpan(0, 2));
        private static readonly EncounterStatic2[] Encounter_GSC = ArrayUtil.ConcatAll(Encounter_GSC_Common, Encounter_GS_Exclusive, Encounter_C_Exclusive, Encounter_GSC_Roam);

        internal static readonly EncounterTrade2[] TradeGift_GSC =
        {
            new(095, 03, 48926) { Gender = 0, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Onix @ Violet City for Bellsprout [wild]
            new(066, 05, 37460) { Gender = 1, IVs = new[] {12, 03, 07, 06, 06, 06} }, // Machop @ Goldenrod City for Drowzee [wild 9, hatched egg 5]
            new(100, 05, 29189) { Gender = 2, IVs = new[] {08, 09, 08, 08, 08, 08} }, // Voltorb @ Olivine City for Krabby [egg]
            new(112, 10, 00283) { Gender = 1, IVs = new[] {12, 07, 07, 06, 06, 06} }, // Rhydon @ Blackthorn City for Dragonair [wild]
            new(142, 05, 26491) { Gender = 0, IVs = new[] {08, 09, 06, 06, 06, 06}, OTGender = 1}, // Aerodactyl @ Route 14 for Chansey [egg]
            new(078, 14, 15616) { Gender = 0, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Rapidash @ Pewter City for Gloom [wild]

            new(085, 10, 00283) { Gender = 1, IVs = new[] {12, 07, 07, 06, 06, 06}, OTGender = 1}, // Dodrio @ Blackthorn City for Dragonair [wild]
            new(178, 15, 15616) { Gender = 0, IVs = new[] {08, 09, 06, 08, 06, 06} }, // Xatu @ Pewter City for Haunter [wild]
            new(082, 05, 50082) { Gender = 2, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Magneton @ Power Plant for Dugtrio [traded for Lickitung]

            new(021, 10, 01001) { Moves = new[] {64,45,43} }, // Spearow @ Goldenrod City for free
            new(213, 15, 00518), // Shuckle @ Cianwood City for free
        };

        private const string tradeGSC = "tradegsc";
        private static readonly string[][] TradeGift_GSC_OTs = Util.GetLanguageStrings8(tradeGSC);

        internal static readonly EncounterStatic2[] StaticGSC = Encounter_GSC;
        internal static readonly EncounterStatic2[] StaticGS = Encounter_GS;
        internal static readonly EncounterStatic2[] StaticC = Encounter_C;

        internal static readonly EncounterStatic2E[] StaticEventsVC =
        {
            new(251, 30, C) { Location = 014, Language = EncounterGBLanguage.Any }, // Celebi @ Ilex Forest (VC)
        };

        private static readonly int[] Farfetchd = {226, 14, 97, 163};
        private static readonly int[] Gligar = {89, 68, 17};
        private static readonly string[] PCNYx =  {"PCNYa", "PCNYb", "PCNYc", "PCNYd"};

        internal static readonly EncounterStatic2E[] StaticEventsGB =
        {
            // Stadium 2 Baton Pass Farfetch'd
            new(083, 05, C) {Moves = Farfetchd, Location = 127, TID = 2000, OT_Name = "スタジアム"},
            new(083, 05, C) {Moves = Farfetchd, Location = 127, TID = 2000, OT_Name = "Stadium", Language = International},
            new(083, 05, C) {Moves = Farfetchd, Location = 127, TID = 2001, OT_Names = new[]{"Stade", "Stadion", "Stadio", "Estadio"}, Language = International},

            // Stadium 2 Earthquake Gligar
            new(207, 05, C) {Moves = Gligar, Location = 127, TID = 2000, OT_Name = "スタジアム"},
            new(207, 05, C) {Moves = Gligar, Location = 127, TID = 2000, OT_Name = "Stadium", Language = International},
            new(207, 05, C) {Moves = Gligar, Location = 127, TID = 2001, OT_Names = new[]{"Stade", "Stadion", "Stadio", "Estadio"}, Language = International},

            //New York Pokémon Center Events

            // Legendary Beasts (November 22 to 29, 2001)
            new(243, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Raikou
            new(244, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Entei
            new(245, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Suicune

            // Legendary Birds (November 30 to December 6, 2001)
            new(144, 05, C) {OT_Names = PCNYx, CurrentLevel = 50, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Articuno
            new(145, 05, C) {OT_Names = PCNYx, CurrentLevel = 50, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Zapdos
            new(146, 05, C) {OT_Names = PCNYx, CurrentLevel = 50, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Moltres

            // Christmas Week (December 21 to 27, 2001)
            new(225, 05, GS) {OT_Names = PCNYx, Moves = new[] {006}, EggLocation = 256, EggCycles = 10, Language = International}, // Pay Day Delibird
            new(251, 05, C) {OT_Names = PCNYx, Location = 127, Language = International}, // Celebi

            // The Initial Three Set (December 28, 2001 to January 31, 2002)
            new(001, 05, GS) {OT_Names = PCNYx, Moves = new[] {246}, EggLocation = 256, EggCycles = 10, Language = International}, // Bulbasaur Ancientpower
            new(004, 05, GS) {OT_Names = PCNYx, Moves = new[] {242}, EggLocation = 256, EggCycles = 10, Language = International}, // Charmander Crunch
            new(007, 05, GS) {OT_Names = PCNYx, Moves = new[] {192}, EggLocation = 256, EggCycles = 10, Language = International}, // Squirtle Zap Cannon
            new(152, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Chikorita Petal Dance
            new(155, 05, GS) {OT_Names = PCNYx, Moves = new[] {038}, EggLocation = 256, EggCycles = 10, Language = International}, // Cyndaquil Double-Edge
            new(158, 05, GS) {OT_Names = PCNYx, Moves = new[] {066}, EggLocation = 256, EggCycles = 10, Language = International}, // Totodile Submission

            // Valentine Week (February 1 to 14, 2002)
            new(029, 05, GS) {OT_Names = PCNYx, Moves = new[] {142}, EggLocation = 256, EggCycles = 10, Language = International}, // Nidoran (F) Lovely Kiss
            new(029, 05, GS) {OT_Names = PCNYx, Moves = new[] {186}, EggLocation = 256, EggCycles = 10, Language = International}, // Nidoran (F) Sweet Kiss
            new(032, 05, GS) {OT_Names = PCNYx, Moves = new[] {142}, EggLocation = 256, EggCycles = 10, Language = International}, // Nidoran (M) Lovely Kiss
            new(032, 05, GS) {OT_Names = PCNYx, Moves = new[] {186}, EggLocation = 256, EggCycles = 10, Language = International}, // Nidoran (M) Sweet Kiss
            new(069, 05, GS) {OT_Names = PCNYx, Moves = new[] {142}, EggLocation = 256, EggCycles = 10, Language = International}, // Bellsprout Lovely Kiss
            new(069, 05, GS) {OT_Names = PCNYx, Moves = new[] {186}, EggLocation = 256, EggCycles = 10, Language = International}, // Bellsprout Sweet Kiss

            // Swarm Week (February 22 to March 14, 2002)
            new(183, 05, GS) {OT_Names = PCNYx, Moves = new[] {056}, EggLocation = 256, EggCycles = 10, Language = International}, // Marill Hydro Pump
            new(193, 05, GS) {OT_Names = PCNYx, Moves = new[] {211}, EggLocation = 256, EggCycles = 10, Language = International}, // Yanma Steel Wing
            new(206, 05, GS) {OT_Names = PCNYx, Moves = new[] {032}, EggLocation = 256, EggCycles = 10, Language = International}, // Dunsparce Horn Drill
            new(209, 05, GS) {OT_Names = PCNYx, Moves = new[] {142}, EggLocation = 256, EggCycles = 10, Language = International}, // Snubbull Lovely Kiss
            new(211, 05, GS) {OT_Names = PCNYx, Moves = new[] {038}, EggLocation = 256, EggCycles = 10, Language = International}, // Qwilfish Double-Edge
            new(223, 05, GS) {OT_Names = PCNYx, Moves = new[] {133}, EggLocation = 256, EggCycles = 10, Language = International}, // Remoraid Amnesia

            // Shiny RBY Starters (March 15 to 21, 2002)
            new(003, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Venusaur
            new(006, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Charizard
            new(009, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Blastoise

            // Babies Week (March 22 to April 11, 2002)
            new(172, 05, GS) {OT_Names = PCNYx, Moves = new[] {047}, EggLocation = 256, EggCycles = 10, Language = International}, // Pichu Sing
            new(173, 05, GS) {OT_Names = PCNYx, Moves = new[] {129}, EggLocation = 256, EggCycles = 10, Language = International}, // Cleffa Swift
            new(174, 05, GS) {OT_Names = PCNYx, Moves = new[] {102}, EggLocation = 256, EggCycles = 10, Language = International}, // Igglybuff Mimic
            new(238, 05, GS) {OT_Names = PCNYx, Moves = new[] {118}, EggLocation = 256, EggCycles = 10, Language = International}, // Smoochum Metronome
            new(239, 05, GS) {OT_Names = PCNYx, Moves = new[] {228}, EggLocation = 256, EggCycles = 10, Language = International}, // Elekid Pursuit
            new(240, 05, GS) {OT_Names = PCNYx, Moves = new[] {185}, EggLocation = 256, EggCycles = 10, Language = International}, // Magby Faint Attack

            // Spring Into Spring (April 12 to May 4, 2002)
            new(054, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Psyduck Petal Dance
            new(152, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Chikorita Petal Dance
            new(172, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Pichu Petal Dance
            new(173, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Cleffa Petal Dance
            new(174, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Igglybuff Petal Dance
            new(238, 05, GS) {OT_Names = PCNYx, Moves = new[] {080}, EggLocation = 256, EggCycles = 10, Language = International}, // Smoochum Petal Dance

            // Baby Weeks (May 5 to June 7, 2002)
            new(194, 05, GS) {Moves = new[] {187}, EggLocation = 256, EggCycles = 10, Language = International}, // Wooper Belly Drum

            // Tropical Promotion to Summer Festival 1 (June 8 to 21, 2002)
            new(060, 05, GS) {OT_Names = PCNYx, Moves = new[] {074}, EggLocation = 256, EggCycles = 10, Language = International}, // Poliwag Growth
            new(116, 05, GS) {OT_Names = PCNYx, Moves = new[] {114}, EggLocation = 256, EggCycles = 10, Language = International}, // Horsea Haze
            new(118, 05, GS) {OT_Names = PCNYx, Moves = new[] {014}, EggLocation = 256, EggCycles = 10, Language = International}, // Goldeen Swords Dance
            new(129, 05, GS) {OT_Names = PCNYx, Moves = new[] {179}, EggLocation = 256, EggCycles = 10, Language = International}, // Magikarp Reversal
            new(183, 05, GS) {OT_Names = PCNYx, Moves = new[] {146}, EggLocation = 256, EggCycles = 10, Language = International}, // Marill Dizzy Punch

            // Tropical Promotion to Summer Festival 2 (July 12 to August 8, 2002)
            new(054, 05, GS) {OT_Names = PCNYx, Moves = new[] {161}, EggLocation = 256, EggCycles = 10, Language = International}, // Psyduck Tri Attack
            new(072, 05, GS) {OT_Names = PCNYx, Moves = new[] {109}, EggLocation = 256, EggCycles = 10, Language = International}, // Tentacool Confuse Ray
            new(131, 05, GS) {OT_Names = PCNYx, Moves = new[] {044}, EggLocation = 256, EggCycles = 10, Language = International}, // Lapras Bite
            new(170, 05, GS) {OT_Names = PCNYx, Moves = new[] {113}, EggLocation = 256, EggCycles = 10, Language = International}, // Chinchou Light Screen
            new(223, 05, GS) {OT_Names = PCNYx, Moves = new[] {054}, EggLocation = 256, EggCycles = 10, Language = International}, // Remoraid Mist
            new(226, 05, GS) {OT_Names = PCNYx, Moves = new[] {016}, EggLocation = 256, EggCycles = 10, Language = International}, // Mantine Gust

            // Safari Week (August 9 to 29, 2002)
            new(029, 05, GS) {OT_Names = PCNYx, Moves = new[] {236}, EggLocation = 256, EggCycles = 10, Language = International}, // Nidoran (F) Moonlight
            new(032, 05, GS) {OT_Names = PCNYx, Moves = new[] {234}, EggLocation = 256, EggCycles = 10, Language = International}, // Nidoran (M) Morning Sun
            new(113, 05, GS) {OT_Names = PCNYx, Moves = new[] {230}, EggLocation = 256, EggCycles = 10, Language = International}, // Chansey Sweet Scent
            new(115, 05, GS) {OT_Names = PCNYx, Moves = new[] {185}, EggLocation = 256, EggCycles = 10, Language = International}, // Kangaskhan Faint Attack
            new(128, 05, GS) {OT_Names = PCNYx, Moves = new[] {098}, EggLocation = 256, EggCycles = 10, Language = International}, // Tauros Quick Attack
            new(147, 05, GS) {OT_Names = PCNYx, Moves = new[] {056}, EggLocation = 256, EggCycles = 10, Language = International}, // Dratini Hydro Pump

            // Sky Week (August 30 to September 26, 2002)
            new(021, 05, GS) {OT_Names = PCNYx, Moves = new[] {049}, EggLocation = 256, EggCycles = 10, Language = International}, // Spearow SonicBoom
            new(083, 05, GS) {OT_Names = PCNYx, Moves = new[] {210}, EggLocation = 256, EggCycles = 10, Language = International}, // Farfetch'd Fury Cutter
            new(084, 05, GS) {OT_Names = PCNYx, Moves = new[] {067}, EggLocation = 256, EggCycles = 10, Language = International}, // Doduo Low Kick
            new(177, 05, GS) {OT_Names = PCNYx, Moves = new[] {219}, EggLocation = 256, EggCycles = 10, Language = International}, // Natu Safeguard
            new(198, 05, GS) {OT_Names = PCNYx, Moves = new[] {251}, EggLocation = 256, EggCycles = 10, Language = International}, // Murkrow Beat Up
            new(227, 05, GS) {OT_Names = PCNYx, Moves = new[] {210}, EggLocation = 256, EggCycles = 10, Language = International}, // Skarmory Fury Cutter

            // The Kanto Initial Three Pokémon (September 27 to October 3, 2002)
            new(150, 05, C) {OT_Names = PCNYx, CurrentLevel = 70, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Mewtwo

            // Power Plant Pokémon (October 4 to October 10, 2002)
            new(172, 05, GS) {OT_Names = PCNYx, Moves = new[] {146}, EggLocation = 256, EggCycles = 10, Language = International}, // Pichu Dizzy Punch
            new(081, 05, GS) {OT_Names = PCNYx, Moves = new[] {097}, EggLocation = 256, EggCycles = 10, Language = International}, // Magnemite Agility
            new(239, 05, GS) {OT_Names = PCNYx, Moves = new[] {146}, EggLocation = 256, EggCycles = 10, Language = International}, // Elekid Dizzy Punch
            new(100, 05, GS) {OT_Names = PCNYx, Moves = new[] {097}, EggLocation = 256, EggCycles = 10, Language = International}, // Voltorb Agility

            // Scary Face Pokémon (October 25 to October 31, 2002)
            new(173, 05, GS) {OT_Names = PCNYx, Moves = new[] {184}, EggLocation = 256, EggCycles = 10, Language = International}, // Cleffa Scary Face
            new(174, 05, GS) {OT_Names = PCNYx, Moves = new[] {184}, EggLocation = 256, EggCycles = 10, Language = International}, // Igglybuff Scary Face
            new(183, 05, GS) {OT_Names = PCNYx, Moves = new[] {184}, EggLocation = 256, EggCycles = 10, Language = International}, // Marill Scary Face
            new(172, 05, GS) {OT_Names = PCNYx, Moves = new[] {184}, EggLocation = 256, EggCycles = 10, Language = International}, // Pichu Scary Face
            new(194, 05, GS) {OT_Names = PCNYx, Moves = new[] {184}, EggLocation = 256, EggCycles = 10, Language = International}, // Wooper Scary Face

            // Silver Cave (November 1 to November 7, 2002)
            new(114, 05, GS) {OT_Names = PCNYx, Moves = new[] {235}, EggLocation = 256, EggCycles = 10, Language = International}, // Tangela Synthesis
            new(077, 05, GS) {OT_Names = PCNYx, Moves = new[] {067}, EggLocation = 256, EggCycles = 10, Language = International}, // Ponyta Low Kick
            new(200, 05, GS) {OT_Names = PCNYx, Moves = new[] {095}, EggLocation = 256, EggCycles = 10, Language = International}, // Misdreavus Hypnosis
            new(246, 05, GS) {OT_Names = PCNYx, Moves = new[] {099}, EggLocation = 256, EggCycles = 10, Language = International}, // Larvitar Rage

            // Union Cave Pokémon (November 8 to 14, 2002)
            new(120, 05, GS) {OT_Names = PCNYx, Moves = new[] {239}, EggLocation = 256, EggCycles = 10, Language = International}, // Staryu Twister
            new(098, 05, GS) {OT_Names = PCNYx, Moves = new[] {232}, EggLocation = 256, EggCycles = 10, Language = International}, // Krabby Metal Claw
            new(095, 05, GS) {OT_Names = PCNYx, Moves = new[] {159}, EggLocation = 256, EggCycles = 10, Language = International}, // Onix Sharpen
            new(131, 05, GS) {OT_Names = PCNYx, Moves = new[] {248}, EggLocation = 256, EggCycles = 10, Language = International}, // Lapras Future Sight

            // Johto Legendary (November 15 to 21, 2002)
            new(250, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Ho-Oh
            new(249, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Lugia

            // Celebi Present SP (November 22 to 28, 2002)
            new(151, 05, C) {OT_Names = PCNYx, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Mew

            // Psychic Type Pokémon (November 29 to December 5, 2002)
            new(063, 05, GS) {OT_Names = PCNYx, Moves = new[] {193}, EggLocation = 256, EggCycles = 10, Language = International}, // Abra Foresight
            new(096, 05, GS) {OT_Names = PCNYx, Moves = new[] {133}, EggLocation = 256, EggCycles = 10, Language = International}, // Drowzee Amnesia
            new(102, 05, GS) {OT_Names = PCNYx, Moves = new[] {230}, EggLocation = 256, EggCycles = 10, Language = International}, // Exeggcute Sweet Scent
            new(122, 05, GS) {OT_Names = PCNYx, Moves = new[] {170}, EggLocation = 256, EggCycles = 10, Language = International}, // Mr. Mime Mind Reader

            // The Johto Initial Three Pokémon (December 6 to 12, 2002)
            new(154, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Meganium
            new(157, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Typhlosion
            new(160, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Feraligatr

            // Rock Tunnel Pokémon (December 13 to December 19, 2002)
            new(074, 05, GS) {OT_Names = PCNYx, Moves = new[] {229}, EggLocation = 256, EggCycles = 10, Language = International}, // Geodude Rapid Spin
            new(041, 05, GS) {OT_Names = PCNYx, Moves = new[] {175}, EggLocation = 256, EggCycles = 10, Language = International}, // Zubat Flail
            new(066, 05, GS) {OT_Names = PCNYx, Moves = new[] {037}, EggLocation = 256, EggCycles = 10, Language = International}, // Machop Thrash
            new(104, 05, GS) {OT_Names = PCNYx, Moves = new[] {031}, EggLocation = 256, EggCycles = 10, Language = International}, // Cubone Fury Attack

            // Ice Type Pokémon (December 20 to 26, 2002)
            new(225, 05, GS) {OT_Names = PCNYx, Moves = new[] {191}, EggLocation = 256, EggCycles = 10, Language = International}, // Delibird Spikes
            new(086, 05, GS) {OT_Names = PCNYx, Moves = new[] {175}, EggLocation = 256, EggCycles = 10, Language = International}, // Seel Flail
            new(220, 05, GS) {OT_Names = PCNYx, Moves = new[] {018}, EggLocation = 256, EggCycles = 10, Language = International}, // Swinub Whirlwind

            // Pokémon that Appear at Night only (December 27, 2002 to January 2, 2003)
            new(163, 05, GS) {OT_Names = PCNYx, Moves = new[] {101}, EggLocation = 256, EggCycles = 10, Language = International}, // Hoothoot Night Shade
            new(215, 05, GS) {OT_Names = PCNYx, Moves = new[] {236}, EggLocation = 256, EggCycles = 10, Language = International}, // Sneasel Moonlight

            // Grass Type ( January 3 to 9, 2003)
            new(191, 05, GS) {OT_Names = PCNYx, Moves = new[] {150}, EggLocation = 256, EggCycles = 10, Language = International}, // Sunkern Splash
            new(046, 05, GS) {OT_Names = PCNYx, Moves = new[] {235}, EggLocation = 256, EggCycles = 10, Language = International}, // Paras Synthesis
            new(187, 05, GS) {OT_Names = PCNYx, Moves = new[] {097}, EggLocation = 256, EggCycles = 10, Language = International}, // Hoppip Agility
            new(043, 05, GS) {OT_Names = PCNYx, Moves = new[] {073}, EggLocation = 256, EggCycles = 10, Language = International}, // Oddish Leech Seed

            // Normal Pokémon (January 10 to 16, 2003)
            new(161, 05, GS) {OT_Names = PCNYx, Moves = new[] {146}, EggLocation = 256, EggCycles = 10, Language = International}, // Sentret Dizzy Punch
            new(234, 05, GS) {OT_Names = PCNYx, Moves = new[] {219}, EggLocation = 256, EggCycles = 10, Language = International}, // Stantler Safeguard
            new(241, 05, GS) {OT_Names = PCNYx, Moves = new[] {025}, EggLocation = 256, EggCycles = 10, Language = International}, // Miltank Mega Kick
            new(190, 05, GS) {OT_Names = PCNYx, Moves = new[] {102}, EggLocation = 256, EggCycles = 10, Language = International}, // Aipom Mimic
            new(108, 05, GS) {OT_Names = PCNYx, Moves = new[] {003}, EggLocation = 256, EggCycles = 10, Language = International}, // Lickitung DoubleSlap
            new(143, 05, GS) {OT_Names = PCNYx, Moves = new[] {150}, EggLocation = 256, EggCycles = 10, Language = International}, // Snorlax Splash

            // Mt. Mortar (January 24 to 30, 2003)
            new(066, 05, GS) {OT_Names = PCNYx, Moves = new[] {206}, EggLocation = 256, EggCycles = 10, Language = International}, // Machop False Swipe
            new(129, 05, GS) {OT_Names = PCNYx, Moves = new[] {145}, EggLocation = 256, EggCycles = 10, Language = International}, // Magikarp Bubble
            new(236, 05, GS) {OT_Names = PCNYx, Moves = new[] {099}, EggLocation = 256, EggCycles = 10, Language = International}, // Tyrogue Rage

            // Dark Cave Pokémon (January 31 to February 6, 2003)
            new(206, 05, GS) {OT_Names = PCNYx, Moves = new[] {031}, EggLocation = 256, EggCycles = 10, Language = International}, // Dunsparce Fury Attack
            new(202, 05, GS) {OT_Names = PCNYx, Moves = new[] {102}, EggLocation = 256, EggCycles = 10, Language = International}, // Wobbuffet Mimic
            new(231, 05, GS) {OT_Names = PCNYx, Moves = new[] {071}, EggLocation = 256, EggCycles = 10, Language = International}, // Phanpy Absorb
            new(216, 05, GS) {OT_Names = PCNYx, Moves = new[] {230}, EggLocation = 256, EggCycles = 10, Language = International}, // Teddiursa Sweet Scent

            // Valentine's Day Special (February 7 to 13, 2003)
            new(060, 05, GS) {OT_Names = PCNYx, Moves = new[] {186}, EggLocation = 256, EggCycles = 10, Language = International}, // Poliwag Sweet Kiss
            new(060, 05, GS) {OT_Names = PCNYx, Moves = new[] {142}, EggLocation = 256, EggCycles = 10, Language = International}, // Poliwag Lovely Kiss
            new(143, 05, GS) {OT_Names = PCNYx, Moves = new[] {186}, EggLocation = 256, EggCycles = 10, Language = International}, // Snorlax Sweet Kiss
            new(143, 05, GS) {OT_Names = PCNYx, Moves = new[] {142}, EggLocation = 256, EggCycles = 10, Language = International}, // Snorlax Lovely Kiss

            // Rare Pokémon (February 21 to 27, 2003)
            new(140, 05, GS) {OT_Names = PCNYx, Moves = new[] {088}, EggLocation = 256, EggCycles = 10, Language = International}, // Kabuto Rock Throw
            new(138, 05, GS) {OT_Names = PCNYx, Moves = new[] {088}, EggLocation = 256, EggCycles = 10, Language = International}, // Omanyte Rock Throw
            new(142, 05, GS) {OT_Names = PCNYx, Moves = new[] {088}, EggLocation = 256, EggCycles = 10, Language = International}, // Aerodactyl Rock Throw
            new(137, 05, GS) {OT_Names = PCNYx, Moves = new[] {112}, EggLocation = 256, EggCycles = 10, Language = International}, // Porygon Barrier
            new(133, 05, GS) {OT_Names = PCNYx, Moves = new[] {074}, EggLocation = 256, EggCycles = 10, Language = International}, // Eevee Growth
            new(185, 05, GS) {OT_Names = PCNYx, Moves = new[] {164}, EggLocation = 256, EggCycles = 10, Language = International}, // Sudowoodo Substitute

            // Bug Type Pokémon (February 28 to March 6, 2003)
            new(123, 05, GS) {OT_Names = PCNYx, Moves = new[] {049}, EggLocation = 256, EggCycles = 10, Language = International}, // Scyther SonicBoom
            new(214, 05, GS) {OT_Names = PCNYx, Moves = new[] {069}, EggLocation = 256, EggCycles = 10, Language = International}, // Heracross Seismic Toss
            new(127, 05, GS) {OT_Names = PCNYx, Moves = new[] {088}, EggLocation = 256, EggCycles = 10, Language = International}, // Pinsir Rock Throw
            new(165, 05, GS) {OT_Names = PCNYx, Moves = new[] {112}, EggLocation = 256, EggCycles = 10, Language = International}, // Ledyba Barrier
            new(167, 05, GS) {OT_Names = PCNYx, Moves = new[] {074}, EggLocation = 256, EggCycles = 10, Language = International}, // Spinarak Growth
            new(193, 05, GS) {OT_Names = PCNYx, Moves = new[] {186}, EggLocation = 256, EggCycles = 10, Language = International}, // Yanma Sweet Kiss
            new(204, 05, GS) {OT_Names = PCNYx, Moves = new[] {164}, EggLocation = 256, EggCycles = 10, Language = International}, // Pineco Substitute

            // Japanese Only (all below)
            new(251, 30, GSC) {Location = 014}, // Celebi @ Ilex Forest (GBC)

            // Gen2 Events
            // Egg Cycles Subject to Change. OTs for Eggs are unknown.
            // Pokémon Center Mystery Egg #1 (December 15, 2001 to January 14, 2002)
            new(152, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Chikorita Petal Dance
            new(172, 05, GSC) {Moves = new[] {047}, EggLocation = 256, EggCycles = 10,}, // Pichu Sing
            new(173, 05, GSC) {Moves = new[] {129}, EggLocation = 256, EggCycles = 10,}, // Cleffa Swift
            new(194, 05, GSC) {Moves = new[] {187}, EggLocation = 256, EggCycles = 10,}, // Wooper Belly Drum
            new(231, 05, GSC) {Moves = new[] {227}, EggLocation = 256, EggCycles = 10,}, // Phanpy Encore
            new(238, 05, GSC) {Moves = new[] {118}, EggLocation = 256, EggCycles = 10,}, // Smoochum Metronome

            // Pokémon Center Mystery Egg #2 (March 16 to April 7, 2002)
            new(054, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Psyduck Petal Dance
            new(152, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Chikorita Petal Dance
            new(172, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Pichu Petal Dance
            new(173, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Cleffa Petal Dance
            new(174, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Igglybuff Petal Dance
            new(238, 05, GSC) {Moves = new[] {080}, EggLocation = 256, EggCycles = 10,}, // Smoochum Petal Dance

            // Pokémon Center Mystery Egg #3 (April 27 to May 12, 2002)
            new(001, 05, GSC) {Moves = new[] {246}, EggLocation = 256, EggCycles = 10,}, // Bulbasaur Ancientpower
            new(004, 05, GSC) {Moves = new[] {242}, EggLocation = 256, EggCycles = 10,}, // Charmander Crunch
            new(158, 05, GSC) {Moves = new[] {066}, EggLocation = 256, EggCycles = 10,}, // Totodile Submission
            new(163, 05, GSC) {Moves = new[] {101}, EggLocation = 256, EggCycles = 10,}, // Hoot-Hoot Night Shade
        };
    }
}
