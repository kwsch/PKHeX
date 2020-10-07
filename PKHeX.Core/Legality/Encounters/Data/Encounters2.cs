using System;
using System.Linq;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Encounters
    /// </summary>
    internal static class Encounters2
    {
        private static readonly EncounterArea2[] SlotsG = Get("gold", "g2", GameVersion.GD);
        private static readonly EncounterArea2[] SlotsS = Get("silver", "g2", GameVersion.SV);
        internal static readonly EncounterArea2[] SlotsC = Get("crystal", "g2", GameVersion.C);

        internal static readonly EncounterArea2[] SlotsGS = ArrayUtil.ConcatAll(SlotsG, SlotsS);
        internal static readonly EncounterArea2[] SlotsGSC = ArrayUtil.ConcatAll(SlotsGS, SlotsC);
        private static readonly TreesArea[] HeadbuttTreesC = TreesArea.GetArray(BinLinker.Unpack(Util.GetBinaryResource("trees_h_c.pkl"), "ch"));
        private static EncounterArea2[] Get(string name, string ident, GameVersion game) =>
            EncounterArea2.GetAreas(BinLinker.Unpack(Util.GetBinaryResource($"encounter_{name}.pkl"), ident), game);

        static Encounters2() => MarkEncounterTradeStrings(TradeGift_GSC, TradeGift_GSC_OTs);

        private static readonly EncounterStatic2[] Encounter_GSC_Common =
        {
            new EncounterStatic2(152, 05, GSC) { Location = 001 }, // Chikorita @ New Bark Town
            new EncounterStatic2(155, 05, GSC) { Location = 001 }, // Cyndaquil @ New Bark Town
            new EncounterStatic2(158, 05, GSC) { Location = 001 }, // Totodile @ New Bark Town

            new EncounterStatic2(175, 05, GSC) { EggLocation = 256 }, // Togepi
            new EncounterStatic2(131, 20, GSC) { Location = 010 }, // Lapras @ Union Cave
            new EncounterStatic2(133, 20, GSC) { Location = 016 }, // Eevee @ Goldenrod City

            new EncounterStatic2(185, 20, GSC) { Location = 020 }, // Sudowoodo @ Route 36
            new EncounterStatic2(236, 10, GSC) { Location = 035 }, // Tyrogue @ Mt. Mortar

            new EncounterStatic2(130, 30, GSC) { Location = 038, Shiny = Shiny.Always, }, // Gyarados @ Lake of Rage
            new EncounterStatic2(074, 21, GSC) { Location = 036 }, // Geodude @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(109, 21, GSC) { Location = 036 }, // Koffing @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(100, 23, GSC) { Location = 036 }, // Voltorb @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(101, 23, GSC) { Location = 036 }, // Electrode @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(143, 50, GSC) { Location = 061 }, // Snorlax @ Vermillion City

            new EncounterStatic2(211, 05, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Old Rod)
            new EncounterStatic2(211, 20, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Good Rod)
            new EncounterStatic2(211, 40, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Super Rod)
        };

        private static readonly EncounterStatic2[] Encounter_GS_Exclusive =
        {
            new EncounterStatic2(245, 40, GS), // Suicune

            new EncounterStatic2(249, 70, GD), // Lugia @ Whirl Islands
            new EncounterStatic2(249, 40, SV), // Lugia @ Whirl Islands

            new EncounterStatic2(250, 40, GD), // Ho-Oh @ Tin Tower
            new EncounterStatic2(250, 70, SV), // Ho-Oh @ Tin Tower

            new EncounterStatic2(137, 15, GS), // Porygon @ Celadon Game Corner
            new EncounterStatic2(133, 15, GS), // Eevee @ Celadon Game Corner
            new EncounterStatic2(122, 15, GS), // Mr. Mime @ Celadon Game Corner

            new EncounterStatic2(063, 10, GS), // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic2(147, 10, GS), // Dratini @ Goldenrod City (Game Corner)
            new EncounterStatic2(023, 10, GS), // Ekans @ Goldenrod City (Game Corner) (Gold)
            new EncounterStatic2(027, 10, GS), // Sandshrew @ Goldenrod City (Game Corner) (Silver)

            new EncounterStatic2(223, 05, GS), // Remoraid Swarm @ Route 44 (Old Rod)
            new EncounterStatic2(223, 20, GS), // Remoraid Swarm @ Route 44 (Good Rod)
            new EncounterStatic2(223, 40, GS), // Remoraid Swarm @ Route 44 (Super Rod)
        };

        private static readonly EncounterStatic2[] Encounter_C_Exclusive =
        {
            new EncounterStatic2(245, 40, C) { Location = 023 }, // Suicune @ Tin Tower

            new EncounterStatic2Odd(172), // Pichu Dizzy Punch
            new EncounterStatic2Odd(173), // Cleffa Dizzy Punch
            new EncounterStatic2Odd(174), // Igglybuff Dizzy Punch
            new EncounterStatic2Odd(236), // Tyrogue Dizzy Punch
            new EncounterStatic2Odd(238), // Smoochum Dizzy Punch
            new EncounterStatic2Odd(239), // Elekid Dizzy Punch
            new EncounterStatic2Odd(240), // Magby Dizzy Punch

            new EncounterStatic2(147, 15, C) { Location = 042, Moves = new [] {245} }, // Dratini ExtremeSpeed

            new EncounterStatic2(249, 60, C) { Location = 031 }, // Lugia @ Whirl Islands
            new EncounterStatic2(250, 60, C) { Location = 023 }, // Ho-Oh @ Tin Tower

            new EncounterStatic2(137, 15, C) { Location = 071 }, // Porygon @ Celadon Game Corner
            new EncounterStatic2(025, 25, C) { Location = 071 }, // Pikachu @ Celadon Game Corner
            new EncounterStatic2(246, 40, C) { Location = 071 }, // Larvitar @ Celadon Game Corner

            new EncounterStatic2(063, 05, C) { Location = 016 }, // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic2(104, 15, C) { Location = 016 }, // Cubone @ Goldenrod City (Game Corner)
            new EncounterStatic2(202, 15, C) { Location = 016 }, // Wobbuffet @ Goldenrod City (Game Corner)
        };

        private static readonly EncounterStatic2[] Encounter_GSC_Roam =
        {
            new EncounterStatic2Roam(243, 40, GSC), // Raikou
            new EncounterStatic2Roam(244, 40, GSC), // Entei
            new EncounterStatic2Roam(245, 40, GS), // Suicune
        };

        private static readonly EncounterStatic2[] Encounter_GS = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_GSC_Roam).ToArray();
        private static readonly EncounterStatic2[] Encounter_C = Encounter_GSC_Common.Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam.Slice(0, 2)).ToArray();
        private static readonly EncounterStatic2[] Encounter_GSC = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam).ToArray();

        internal static readonly EncounterTrade2[] TradeGift_GSC =
        {
            new EncounterTrade2(095, 03, 48926) { Gender = 0, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Onix @ Violet City for Bellsprout [wild]
            new EncounterTrade2(066, 05, 37460) { Gender = 1, IVs = new[] {12, 03, 07, 06, 06, 06} }, // Machop @ Goldenrod City for Drowzee [wild 9, hatched egg 5]
            new EncounterTrade2(100, 05, 29189) { Gender = 2, IVs = new[] {08, 09, 08, 08, 08, 08} }, // Voltorb @ Olivine City for Krabby [egg]
            new EncounterTrade2(112, 10, 00283) { Gender = 1, IVs = new[] {12, 07, 07, 06, 06, 06} }, // Rhydon @ Blackthorn City for Dragonair [wild]
            new EncounterTrade2(142, 05, 26491) { Gender = 0, IVs = new[] {08, 09, 06, 06, 06, 06}, OTGender = 1}, // Aerodactyl @ Route 14 for Chansey [egg]
            new EncounterTrade2(078, 14, 15616) { Gender = 0, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Rapidash @ Pewter City for Gloom [wild]

            new EncounterTrade2(085, 10, 00283) { Gender = 1, IVs = new[] {12, 07, 07, 06, 06, 06}, OTGender = 1}, // Dodrio @ Blackthorn City for Dragonair [wild]
            new EncounterTrade2(178, 15, 15616) { Gender = 0, IVs = new[] {08, 09, 06, 08, 06, 06} }, // Xatu @ Pewter City for Haunter [wild]
            new EncounterTrade2(082, 05, 50082) { Gender = 2, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Magneton @ Power Plant for Dugtrio [traded for Lickitung]

            new EncounterTrade2(021, 10, 01001) { Moves = new[] {64,45,43} }, // Spearow @ Goldenrod City for free
            new EncounterTrade2(213, 15, 00518), // Shuckle @ Cianwood City for free
        };

        private const string tradeGSC = "tradegsc";
        private static readonly string[][] TradeGift_GSC_OTs = Util.GetLanguageStrings8(tradeGSC);

        internal static TreeEncounterAvailable GetGSCHeadbuttAvailability(EncounterSlot encounter, int trainerID)
        {
            var area = Array.Find(HeadbuttTreesC, a => a.Location == encounter.Location);
            if (area == null) // Failsafe, every area with headbutt encounters has a tree area
                return TreeEncounterAvailable.Impossible;

            var table = area.GetTrees(encounter.Area.Type);
            var trainerpivot = trainerID % 10;
            return table[trainerpivot];
        }

        internal static readonly EncounterStatic2[] StaticGSC = Encounter_GSC;
        internal static readonly EncounterStatic2[] StaticGS = Encounter_GS;
        internal static readonly EncounterStatic2[] StaticC = Encounter_C;

        internal static readonly EncounterStatic2E[] StaticEventsVC =
        {
            new EncounterStatic2E(251, 30, C) { Location = 014, Language = EncounterGBLanguage.Any }, // Celebi @ Ilex Forest (VC)
        };

        private static readonly int[] Farfetchd = {226, 14, 97, 163};
        private static readonly int[] Gligar = {89, 68, 17};

        internal static readonly EncounterStatic2E[] StaticEventsGB =
        {
            // Stadium 2 Baton Pass Farfetch'd
            new EncounterStatic2E(083, 05, GS) {Moves = Farfetchd, TID = 2000, OT_Name = "スタジアム"},
            new EncounterStatic2E(083, 05, GS) {Moves = Farfetchd, TID = 2000, OT_Name = "Stadium", Language = EncounterGBLanguage.International},
            new EncounterStatic2E(083, 05, GS) {Moves = Farfetchd, TID = 2001, OT_Name = "Stade",   Language = EncounterGBLanguage.International},
            new EncounterStatic2E(083, 05, GS) {Moves = Farfetchd, TID = 2001, OT_Name = "Stadion", Language = EncounterGBLanguage.International},
            new EncounterStatic2E(083, 05, GS) {Moves = Farfetchd, TID = 2001, OT_Name = "Stadio",  Language = EncounterGBLanguage.International},
            new EncounterStatic2E(083, 05, GS) {Moves = Farfetchd, TID = 2001, OT_Name = "Estadio", Language = EncounterGBLanguage.International},

            // Stadium 2 Earthquake Gligar
            new EncounterStatic2E(207, 05, GS) {Moves = Gligar, TID = 2000, OT_Name = "スタジアム"},
            new EncounterStatic2E(207, 05, GS) {Moves = Gligar, TID = 2000, OT_Name = "Stadium", Language = EncounterGBLanguage.International},
            new EncounterStatic2E(207, 05, GS) {Moves = Gligar, TID = 2001, OT_Name = "Stade",   Language = EncounterGBLanguage.International},
            new EncounterStatic2E(207, 05, GS) {Moves = Gligar, TID = 2001, OT_Name = "Stadion", Language = EncounterGBLanguage.International},
            new EncounterStatic2E(207, 05, GS) {Moves = Gligar, TID = 2001, OT_Name = "Stadio",  Language = EncounterGBLanguage.International},
            new EncounterStatic2E(207, 05, GS) {Moves = Gligar, TID = 2001, OT_Name = "Estadio", Language = EncounterGBLanguage.International},

            //New York Pokémon Center Events

            // Christmas Week (December 21 to 27, 2001)
            // Pay Day Delibird

            // The Initial Three Set (December 28, 2001 to January 31, 2002)
            new EncounterStatic2E(001, 05, GS) {Moves = new[] {246}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Bulbasaur Ancientpower
            new EncounterStatic2E(004, 05, GS) {Moves = new[] {242}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Charmander Crunch
            new EncounterStatic2E(007, 05, GS) {Moves = new[] {192}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Squirtle Zap Cannon
            new EncounterStatic2E(152, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Chikorita Petal Dance
            new EncounterStatic2E(155, 05, GS) {Moves = new[] {038}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Cyndaquil Double-Edge
            new EncounterStatic2E(158, 05, GS) {Moves = new[] {066}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Totodile Submission

            // Valentine Week (February 1 to 14, 2002)
            new EncounterStatic2E(029, 05, GS) {Moves = new[] {142}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Nidoran (F) Lovely Kiss
            new EncounterStatic2E(029, 05, GS) {Moves = new[] {186}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Nidoran (F) Sweet Kiss
            new EncounterStatic2E(032, 05, GS) {Moves = new[] {142}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Nidoran (M) Lovely Kiss
            new EncounterStatic2E(032, 05, GS) {Moves = new[] {186}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Nidoran (M) Sweet Kiss
            new EncounterStatic2E(069, 05, GS) {Moves = new[] {142}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Bellsprout Lovely Kiss
            new EncounterStatic2E(069, 05, GS) {Moves = new[] {186}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Bellsprout Sweet Kiss

            // Swarm Week (February 22 to March 14, 2002)
            new EncounterStatic2E(183, 05, GS) {Moves = new[] {056}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Marill Hydro Pump
            new EncounterStatic2E(193, 05, GS) {Moves = new[] {211}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Yanma Steel Wing
            new EncounterStatic2E(206, 05, GS) {Moves = new[] {032}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Dunsparce Horn Drill
            new EncounterStatic2E(209, 05, GS) {Moves = new[] {142}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Snubbull Lovely Kiss
            new EncounterStatic2E(211, 05, GS) {Moves = new[] {038}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Qwilfish Double-Edge
            new EncounterStatic2E(223, 05, GS) {Moves = new[] {133}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Remoraid Amnesia

            // Babies Week (March 22 to April 11, 2002)
            new EncounterStatic2E(172, 05, GS) {Moves = new[] {047}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Pichu Sing
            new EncounterStatic2E(173, 05, GS) {Moves = new[] {129}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Cleffa Swift
            new EncounterStatic2E(174, 05, GS) {Moves = new[] {102}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Igglybuff Mimic
            new EncounterStatic2E(238, 05, GS) {Moves = new[] {118}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Smoochum Metronome
            new EncounterStatic2E(239, 05, GS) {Moves = new[] {228}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Elekid Pursuit
            new EncounterStatic2E(240, 05, GS) {Moves = new[] {185}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Magby Faint Attack

            // Spring Into Spring (April 12 to May 4, 2002)
            new EncounterStatic2E(054, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Psyduck Petal Dance
            new EncounterStatic2E(152, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Chikorita Petal Dance
            new EncounterStatic2E(172, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Pichu Petal Dance
            new EncounterStatic2E(173, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Cleffa Petal Dance
            new EncounterStatic2E(174, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Igglybuff Petal Dance
            new EncounterStatic2E(238, 05, GS) {Moves = new[] {080}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Smoochum Petal Dance

            // Baby Weeks (May 5 to June 7, 2002)
            new EncounterStatic2E(194, 05, GS) {Moves = new[] {187}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Wooper Belly Drum
          
            // Tropical Promotion to Summer Festival 1 (June 8 to 21, 2002)
            new EncounterStatic2E(060, 05, GS) {Moves = new[] {074}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Poliwag Growth
            new EncounterStatic2E(116, 05, GS) {Moves = new[] {114}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Horsea Haze
            new EncounterStatic2E(118, 05, GS) {Moves = new[] {014}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Goldeen Swords Dance
            new EncounterStatic2E(129, 05, GS) {Moves = new[] {179}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Magikarp Reversal
            new EncounterStatic2E(183, 05, GS) {Moves = new[] {146}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Marill Dizzy Punch
            
            // Tropical Promotion to Summer Festival 2 (July 12 to August 8, 2002)
            new EncounterStatic2E(054, 05, GS) {Moves = new[] {161}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Psyduck Tri Attack
            new EncounterStatic2E(072, 05, GS) {Moves = new[] {109}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Tentacool Confuse Ray
            new EncounterStatic2E(131, 05, GS) {Moves = new[] {044}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Lapras Bite
            new EncounterStatic2E(170, 05, GS) {Moves = new[] {113}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Chinchou Light Screen
            new EncounterStatic2E(223, 05, GS) {Moves = new[] {054}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Remoraid Mist
            new EncounterStatic2E(226, 05, GS) {Moves = new[] {016}, EggLocation = 256, Language = EncounterGBLanguage.International}, // Mantine Gust

            // Japanese Only (all below)
            new EncounterStatic2E(251, 30, GSC) {Location = 014}, // Celebi @ Ilex Forest (GBC)

            // Gen2 Events
            // Pokémon Center Mystery Egg #1 (December 15, 2001 to January 14, 2002)
            new EncounterStatic2E(152, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Chikorita Petal Dance
            new EncounterStatic2E(172, 05, GS) {Moves = new[] {047}, EggLocation = 256}, // Pichu Sing
            new EncounterStatic2E(173, 05, GS) {Moves = new[] {129}, EggLocation = 256}, // Cleffa Swift
            new EncounterStatic2E(194, 05, GS) {Moves = new[] {187}, EggLocation = 256}, // Wooper Belly Drum
            new EncounterStatic2E(231, 05, GS) {Moves = new[] {227}, EggLocation = 256}, // Phanpy Encore
            new EncounterStatic2E(238, 05, GS) {Moves = new[] {118}, EggLocation = 256}, // Smoochum Metronome

            // Pokémon Center Mystery Egg #2 (March 16 to April 7, 2002)
            new EncounterStatic2E(054, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Psyduck Petal Dance
            new EncounterStatic2E(152, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Chikorita Petal Dance
            new EncounterStatic2E(172, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Pichu Petal Dance
            new EncounterStatic2E(173, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Cleffa Petal Dance
            new EncounterStatic2E(174, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Igglybuff Petal Dance
            new EncounterStatic2E(238, 05, GS) {Moves = new[] {080}, EggLocation = 256}, // Smoochum Petal Dance

            // Pokémon Center Mystery Egg #3 (April 27 to May 12, 2002)
            new EncounterStatic2E(001, 05, GS) {Moves = new[] {246}, EggLocation = 256}, // Bulbasaur Ancientpower
            new EncounterStatic2E(004, 05, GS) {Moves = new[] {242}, EggLocation = 256}, // Charmander Crunch
            new EncounterStatic2E(158, 05, GS) {Moves = new[] {066}, EggLocation = 256}, // Totodile Submission
            new EncounterStatic2E(163, 05, GS) {Moves = new[] {101}, EggLocation = 256}, // Hoot-Hoot Night Shade
        };
    }
}
