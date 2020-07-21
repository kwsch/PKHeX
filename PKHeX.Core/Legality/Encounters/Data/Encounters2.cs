using System;
using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Encounters
    /// </summary>
    internal static class Encounters2
    {
        internal static readonly EncounterArea2[] SlotsGSC, SlotsGS, SlotsC;
        internal static readonly EncounterStatic2[] StaticGSC, StaticGS, StaticC;
        private static readonly TreesArea[] HeadbuttTreesC = TreesArea.GetArray(BinLinker.Unpack(Util.GetBinaryResource("trees_h_c.pkl"), "ch"));

        static Encounters2()
        {
            StaticGS = Encounter_GS;
            StaticC = Encounter_C;
            StaticGSC = Encounter_GSC;
            SlotsGS = GetTables2(GameVersion.GS);
            SlotsC = GetTables2(GameVersion.C);
            SlotsGSC = GetTables2(GameVersion.GSC);
            MarkEncounterAreaArray(SlotsGS, SlotsC, SlotsGSC, EncounterSafari_GSC, EncounterBCC_GSC);
            ReduceAreasSize(ref SlotsGS);
            ReduceAreasSize(ref SlotsC);
            ReduceAreasSize(ref SlotsGSC);
            MarkEncountersGeneration(2, SlotsGS, SlotsC, SlotsGSC);
            MarkEncountersGeneration(2, StaticGS, StaticC, StaticGSC, TradeGift_GSC);

            MarkEncounterTradeStrings(TradeGift_GSC, TradeGift_GSC_OTs);

            SlotsGSC.SetVersion(GameVersion.GSC);
            SlotsGS.SetVersion(GameVersion.GS);
            SlotsC.SetVersion(GameVersion.C);
            StaticGSC.SetVersion(GameVersion.GSC);
            StaticGS.SetVersion(GameVersion.GS);
            StaticC.SetVersion(GameVersion.C);
            TradeGift_GSC.SetVersion(GameVersion.GSC);
        }

        private static EncounterArea2[] GetTables2(GameVersion Version)
        {
            // Fishing
            var f = EncounterArea2.GetArray2Fishing(Util.GetBinaryResource("encounter_gsc_f.pkl"));

            var Slots = Array.Empty<EncounterArea2>();
            if (Version.Contains(GameVersion.GS))
                Slots = GetSlots_GS(f);
            if (Version.Contains(GameVersion.C))
                Slots = AddExtraTableSlots(Slots, GetSlots_C(f));

            return Slots;
        }

        private static EncounterArea2[] GetSlots_GS(EncounterArea2[] f)
        {
            // Grass/Water
            var g = EncounterArea2.GetArray2GrassWater(Util.GetBinaryResource("encounter_gold.pkl"));
            var s = EncounterArea2.GetArray2GrassWater(Util.GetBinaryResource("encounter_silver.pkl"));
            // Headbutt/Rock Smash
            var h_g = EncounterArea2.GetArray2Headbutt(Util.GetBinaryResource("encounter_gold_h.pkl"));
            var h_s = EncounterArea2.GetArray2Headbutt(Util.GetBinaryResource("encounter_silver_h.pkl"));
            var safari_gs = EncounterSafari_GSC;
            var bcc_gs = EncounterBCC_GSC;

            MarkEncountersVersion(bcc_gs, GameVersion.GS);
            MarkEncountersVersion(f, GameVersion.GS);
            MarkEncountersVersion(g, GameVersion.GD);
            MarkEncountersVersion(s, GameVersion.SV);
            MarkEncountersVersion(h_g, GameVersion.GD);
            MarkEncountersVersion(h_s, GameVersion.SV);
            MarkEncountersVersion(safari_gs, GameVersion.GS);

            return AddExtraTableSlots(g, s, h_g, h_s, f, bcc_gs, safari_gs);
        }

        private static EncounterArea2[] GetSlots_C(EncounterArea2[] f)
        {
            // Grass/Water
            var c = EncounterArea2.GetArray2GrassWater(Util.GetBinaryResource("encounter_crystal.pkl"));
            // Headbutt/Rock Smash
            var h_c = EncounterArea2.GetArray2Headbutt(Util.GetBinaryResource("encounter_crystal_h.pkl"));
            var safari_c = EncounterSafari_GSC;
            var bcc_c = EncounterBCC_GSC;

            MarkEncountersVersion(bcc_c, GameVersion.C);
            MarkEncountersVersion(safari_c, GameVersion.C);
            MarkEncountersVersion(f, GameVersion.C);
            MarkEncountersVersion(c, GameVersion.C);
            MarkEncountersVersion(h_c, GameVersion.C);

            var extra = AddExtraTableSlots(c, h_c, f, bcc_c, safari_c);
            return extra;
        }

        private static readonly EncounterArea2[] EncounterBCC_GSC = { new EncounterArea2 {
            Location = 19,
            Slots = new EncounterSlot[]
            {
                new EncounterSlot2(010, 07, 18, 20, SlotType.BugContest, 0), // Caterpie
                new EncounterSlot2(013, 07, 18, 20, SlotType.BugContest, 1), // Weedle
                new EncounterSlot2(011, 09, 18, 10, SlotType.BugContest, 2), // Metapod
                new EncounterSlot2(014, 09, 18, 10, SlotType.BugContest, 3), // Kakuna
                new EncounterSlot2(012, 12, 15, 05, SlotType.BugContest, 4), // Butterfree
                new EncounterSlot2(015, 12, 15, 05, SlotType.BugContest, 5), // Beedrill
                new EncounterSlot2(048, 10, 16, 10, SlotType.BugContest, 6), // Venonat
                new EncounterSlot2(046, 10, 17, 10, SlotType.BugContest, 7), // Paras
                new EncounterSlot2(123, 13, 14, 05, SlotType.BugContest, 8), // Scyther
                new EncounterSlot2(127, 13, 14, 05, SlotType.BugContest, 9), // Pinsir
            }
        }};

        private static readonly EncounterArea2[] EncounterSafari_GSC = { new EncounterArea2 {
            Location = 81,
            Slots = new EncounterSlot[]
            {
                new EncounterSlot2(129, 10, 10, 100, SlotType.Old_Rod_Safari, 0), // Magikarp
                new EncounterSlot2(098, 10, 10, 100, SlotType.Old_Rod_Safari, 1), // Krabby
                new EncounterSlot2(098, 20, 20, 100, SlotType.Good_Rod_Safari, 0), // Krabby
                new EncounterSlot2(129, 20, 20, 100, SlotType.Good_Rod_Safari, 1), // Magikarp
                new EncounterSlot2(222, 20, 20, 100, SlotType.Good_Rod_Safari, 2), // Corsola
                new EncounterSlot2(120, 20, 20, 100, SlotType.Good_Rod_Safari, 3), // Staryu
                new EncounterSlot2(098, 40, 40, 100, SlotType.Super_Rod_Safari, 0), // Krabby
                new EncounterSlot2(222, 40, 40, 100, SlotType.Super_Rod_Safari, 1), // Corsola
                new EncounterSlot2(120, 40, 40, 100, SlotType.Super_Rod_Safari, 2), // Staryu
                new EncounterSlot2(121, 40, 40, 100, SlotType.Super_Rod_Safari, 3), // Kingler
            }
        }};

        private static readonly EncounterStatic2[] Encounter_GSC_Common =
        {
            new EncounterStatic2(152, 05) { Location = 001, Version = GameVersion.GSC }, // Chikorita @ New Bark Town
            new EncounterStatic2(155, 05) { Location = 001, Version = GameVersion.GSC }, // Cyndaquil @ New Bark Town
            new EncounterStatic2(158, 05) { Location = 001, Version = GameVersion.GSC }, // Totodile @ New Bark Town

            new EncounterStatic2(175, 05) { Version = GameVersion.GSC, EggLocation = 256 }, // Togepi
            new EncounterStatic2(131, 20) { Location = 010, Version = GameVersion.GSC }, // Lapras @ Union Cave
            new EncounterStatic2(133, 20) { Location = 016, Version = GameVersion.GSC }, // Eevee @ Goldenrod City

            new EncounterStatic2(185, 20) { Location = 020, Version = GameVersion.GSC }, // Sudowoodo @ Route 36
            new EncounterStatic2(236, 10) { Location = 035, Version = GameVersion.GSC }, // Tyrogue @ Mt. Mortar

            new EncounterStatic2(130, 30) { Location = 038, Version = GameVersion.GSC, Shiny = Shiny.Always, }, // Gyarados @ Lake of Rage
            new EncounterStatic2(074, 21) { Location = 036, Version = GameVersion.GSC }, // Geodude @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(109, 21) { Location = 036, Version = GameVersion.GSC }, // Koffing @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(100, 23) { Location = 036, Version = GameVersion.GSC }, // Voltorb @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(101, 23) { Location = 036, Version = GameVersion.GSC }, // Electrode @ Rocket Hideout (Mahogany Town)
            new EncounterStatic2(143, 50) { Location = 061, Version = GameVersion.GSC }, // Snorlax @ Vermillion City

            new EncounterStatic2(211, 05) { Location = 008, Version = GameVersion.GSC }, // Qwilfish Swarm @ Route 32 (Old Rod)
            new EncounterStatic2(211, 20) { Location = 008, Version = GameVersion.GSC }, // Qwilfish Swarm @ Route 32 (Good Rod)
            new EncounterStatic2(211, 40) { Location = 008, Version = GameVersion.GSC }, // Qwilfish Swarm @ Route 32 (Super Rod)

            new EncounterStatic2(083, 05) { Moves = new [] { 226, 14, 97, 163 }, Version = GameVersion.Stadium2 }, // Stadium 2 Baton Pass Farfetch'd
            new EncounterStatic2(207, 05) { Moves = new [] { 89, 68, 17 }, Version = GameVersion.Stadium2 }, // Stadium 2 Earthquake Gligar

            // Gen2 Events
            // Pokémon Center Mystery Egg #1 (December 15, 2001 to January 14, 2002)
            new EncounterStatic2(152, 05) { Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Chikorita Petal Dance
            new EncounterStatic2(173, 05) { Moves = new [] { 129 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Cleffa Swift
            new EncounterStatic2(194, 05) { Moves = new [] { 187 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Wooper Belly Drum
            new EncounterStatic2(231, 05) { Moves = new [] { 227 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Phanpy Encore
            new EncounterStatic2(238, 05) { Moves = new [] { 118 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Smoochum Metronome

            // Pokémon Center Mystery Egg #2 (March 16 to April 7, 2002)
            new EncounterStatic2(047, 05) { Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Psyduck Petal Dance
            // new EncounterStatic(152, 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Chikorita Petal Dance
            new EncounterStatic2(172, 05) { Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Pichu Petal Dance
            new EncounterStatic2(173, 05) { Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Cleffa Petal Dance
            new EncounterStatic2(174, 05) { Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Igglybuff Petal Dance
            new EncounterStatic2(238, 05) { Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Smoochum Petal Dance

            // Pokémon Center Mystery Egg #3 (April 27 to May 12, 2002)
            new EncounterStatic2(001, 05) { Moves = new [] { 246 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Bulbasaur Ancientpower
            new EncounterStatic2(004, 05) { Moves = new [] { 242 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Charmander Crunch
            new EncounterStatic2(158, 05) { Moves = new [] { 066 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Totodile Submission
            new EncounterStatic2(163, 05) { Moves = new [] { 101 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Hoot-Hoot Night Shade
            new EncounterStatic2(158, 05) { Moves = new [] { 047 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Pichu Sing
        };

        private static readonly EncounterStatic2[] Encounter_GS_Exclusive =
        {
            new EncounterStatic2(245, 40) { Version = GameVersion.GS }, // Suicune

            new EncounterStatic2(249, 70) { Version = GameVersion.GD }, // Lugia @ Whirl Islands
            new EncounterStatic2(249, 40) { Version = GameVersion.SV }, // Lugia @ Whirl Islands

            new EncounterStatic2(250, 40) { Version = GameVersion.GD }, // Ho-Oh @ Tin Tower
            new EncounterStatic2(250, 70) { Version = GameVersion.SV }, // Ho-Oh @ Tin Tower

            new EncounterStatic2(137, 15) { Version = GameVersion.GS }, // Porygon @ Celadon Game Corner
            new EncounterStatic2(133, 15) { Version = GameVersion.GS }, // Eevee @ Celadon Game Corner
            new EncounterStatic2(122, 15) { Version = GameVersion.GS }, // Mr. Mime @ Celadon Game Corner

            new EncounterStatic2(063, 10) { Version = GameVersion.GS }, // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic2(147, 10) { Version = GameVersion.GS }, // Dratini @ Goldenrod City (Game Corner)
            new EncounterStatic2(023, 10) { Version = GameVersion.GS }, // Ekans @ Goldenrod City (Game Corner) (Gold)
            new EncounterStatic2(027, 10) { Version = GameVersion.GS }, // Sandshrew @ Goldenrod City (Game Corner) (Silver)

            new EncounterStatic2(223, 05) { Version = GameVersion.GS }, // Remoraid Swarm @ Route 44 (Old Rod)
            new EncounterStatic2(223, 20) { Version = GameVersion.GS }, // Remoraid Swarm @ Route 44 (Good Rod)
            new EncounterStatic2(223, 40) { Version = GameVersion.GS }, // Remoraid Swarm @ Route 44 (Super Rod)
        };

        private static readonly EncounterStatic2[] Encounter_C_Exclusive =
        {
            new EncounterStatic2(245, 40) { Location = 023, Version = GameVersion.C }, // Suicune @ Tin Tower

            new EncounterStatic2Odd(172), // Pichu Dizzy Punch
            new EncounterStatic2Odd(173), // Cleffa Dizzy Punch
            new EncounterStatic2Odd(174), // Igglybuff Dizzy Punch
            new EncounterStatic2Odd(236), // Tyrogue Dizzy Punch
            new EncounterStatic2Odd(238), // Smoochum Dizzy Punch
            new EncounterStatic2Odd(239), // Elekid Dizzy Punch
            new EncounterStatic2Odd(240), // Magby Dizzy Punch

            new EncounterStatic2(147, 15) { Location = 042, Version = GameVersion.C, Moves = new [] {245} }, // Dratini ExtremeSpeed

            new EncounterStatic2(249, 60) { Location = 031, Version = GameVersion.C }, // Lugia @ Whirl Islands
            new EncounterStatic2(250, 60) { Location = 023, Version = GameVersion.C }, // Ho-Oh @ Tin Tower
            new EncounterStatic2(251, 30) { Location = 014, Version = GameVersion.C }, // Celebi @ Ilex Forest (VC)
            new EncounterStatic2(251, 30) { Location = 014, Version = GameVersion.EventsGBGen2 }, // Celebi @ Ilex Forest (GBC)

            new EncounterStatic2(137, 15) { Location = 071, Version = GameVersion.C }, // Porygon @ Celadon Game Corner
            new EncounterStatic2(025, 25) { Location = 071, Version = GameVersion.C }, // Pikachu @ Celadon Game Corner
            new EncounterStatic2(246, 40) { Location = 071, Version = GameVersion.C }, // Larvitar @ Celadon Game Corner

            new EncounterStatic2(063, 05) { Location = 016, Version = GameVersion.C }, // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic2(104, 15) { Location = 016, Version = GameVersion.C }, // Cubone @ Goldenrod City (Game Corner)
            new EncounterStatic2(202, 15) { Location = 016, Version = GameVersion.C }, // Wobbuffet @ Goldenrod City (Game Corner)
        };

        private static readonly EncounterStatic2[] Encounter_GSC_Roam =
        {
            new EncounterStatic2Roam(243, 40) { Version = GameVersion.GSC }, // Raikou
            new EncounterStatic2Roam(244, 40) { Version = GameVersion.GSC }, // Entei
            new EncounterStatic2Roam(245, 40) { Version = GameVersion.GS }, // Suicune
        };

        private static readonly EncounterStatic2[] Encounter_GS = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_GSC_Roam).ToArray();
        private static readonly EncounterStatic2[] Encounter_C = Encounter_GSC_Common.Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam.Slice(0, 2)).ToArray();
        private static readonly EncounterStatic2[] Encounter_GSC = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam).ToArray();

        internal static readonly EncounterTradeGB[] TradeGift_GSC =
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

        internal static TreeEncounterAvailable GetGSCHeadbuttAvailability(EncounterSlot encounter, int TID)
        {
            var Area = Array.Find(HeadbuttTreesC, a => a.Location == encounter.Location);
            if (Area == null) // Failsafe, every area with headbutt encounters has a tree area
                return TreeEncounterAvailable.Impossible;

            var table = Area.GetTrees(encounter.Type);
            var trainerpivot = TID % 10;
            return table[trainerpivot];
        }
    }
}
