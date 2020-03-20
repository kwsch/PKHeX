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
        internal static readonly EncounterStatic[] StaticGSC, StaticGS, StaticC;
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

        private static readonly int[] Roaming_MetLocation_GSC_Grass =
        {
            // Routes 29, 30-31, 33, 34, 35, 36-37, 38-39, 42, 43, 44, 45-46 can be encountered in grass
            2, 4, 5, 8, 11, 15, 18, 20, 21,
            25, 26, 34, 37, 39, 43, 45,
        };

        private static readonly EncounterArea2[] EncounterBCC_GSC = { new EncounterArea2 {
            Location = 19,
            Slots = new EncounterSlot[]
            {
                new EncounterSlot1 {Species = 010, LevelMin = 07, LevelMax = 18, Rate = 20, SlotNumber = 0}, // Caterpie
                new EncounterSlot1 {Species = 013, LevelMin = 07, LevelMax = 18, Rate = 20, SlotNumber = 1}, // Weedle
                new EncounterSlot1 {Species = 011, LevelMin = 09, LevelMax = 18, Rate = 10, SlotNumber = 2}, // Metapod
                new EncounterSlot1 {Species = 014, LevelMin = 09, LevelMax = 18, Rate = 10, SlotNumber = 3}, // Kakuna
                new EncounterSlot1 {Species = 012, LevelMin = 12, LevelMax = 15, Rate = 05, SlotNumber = 4}, // Butterfree
                new EncounterSlot1 {Species = 015, LevelMin = 12, LevelMax = 15, Rate = 05, SlotNumber = 5}, // Beedrill
                new EncounterSlot1 {Species = 048, LevelMin = 10, LevelMax = 16, Rate = 10, SlotNumber = 6}, // Venonat
                new EncounterSlot1 {Species = 046, LevelMin = 10, LevelMax = 17, Rate = 10, SlotNumber = 7}, // Paras
                new EncounterSlot1 {Species = 123, LevelMin = 13, LevelMax = 14, Rate = 05, SlotNumber = 8}, // Scyther
                new EncounterSlot1 {Species = 127, LevelMin = 13, LevelMax = 14, Rate = 05, SlotNumber = 9}, // Pinsir
            }
        }};

        private static readonly EncounterArea2[] EncounterSafari_GSC = { new EncounterArea2 {
            Location = 81,
            Slots = new EncounterSlot[]
            {
                new EncounterSlot1 {Species = 129, LevelMin = 10, LevelMax = 10, Type = SlotType.Old_Rod_Safari}, // Magikarp
                new EncounterSlot1 {Species = 098, LevelMin = 10, LevelMax = 10, Type = SlotType.Old_Rod_Safari}, // Krabby
                new EncounterSlot1 {Species = 098, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod_Safari}, // Krabby
                new EncounterSlot1 {Species = 129, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod_Safari}, // Magikarp
                new EncounterSlot1 {Species = 222, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod_Safari}, // Corsola
                new EncounterSlot1 {Species = 120, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod_Safari}, // Staryu
                new EncounterSlot1 {Species = 098, LevelMin = 40, LevelMax = 40, Type = SlotType.Super_Rod_Safari}, // Krabby
                new EncounterSlot1 {Species = 222, LevelMin = 40, LevelMax = 40, Type = SlotType.Super_Rod_Safari}, // Corsola
                new EncounterSlot1 {Species = 120, LevelMin = 40, LevelMax = 40, Type = SlotType.Super_Rod_Safari}, // Staryu
                new EncounterSlot1 {Species = 121, LevelMin = 40, LevelMax = 40, Type = SlotType.Super_Rod_Safari}, // Kingler
            }
        }};

        private static readonly EncounterStatic[] Encounter_GSC_Common =
        {
            new EncounterStatic { Species = 152, Level = 05, Location = 001, Version = GameVersion.GSC }, // Chikorita @ New Bark Town
            new EncounterStatic { Species = 155, Level = 05, Location = 001, Version = GameVersion.GSC }, // Cyndaquil @ New Bark Town
            new EncounterStatic { Species = 158, Level = 05, Location = 001, Version = GameVersion.GSC }, // Totodile @ New Bark Town

            new EncounterStatic { Species = 175, Level = 05, Version = GameVersion.GSC, EggLocation = 256 }, // Togepi
            new EncounterStatic { Species = 131, Level = 20, Location = 010, Version = GameVersion.GSC }, // Lapras @ Union Cave
            new EncounterStatic { Species = 133, Level = 20, Location = 016, Version = GameVersion.GSC }, // Eevee @ Goldenrod City

            new EncounterStatic { Species = 185, Level = 20, Location = 020, Version = GameVersion.GSC }, // Sudowoodo @ Route 36
            new EncounterStatic { Species = 236, Level = 10, Location = 035, Version = GameVersion.GSC }, // Tyrogue @ Mt. Mortar

            new EncounterStatic { Species = 130, Level = 30, Location = 038, Version = GameVersion.GSC, Shiny = Shiny.Always, }, // Gyarados @ Lake of Rage
            new EncounterStatic { Species = 074, Level = 21, Location = 036, Version = GameVersion.GSC }, // Geodude @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 109, Level = 21, Location = 036, Version = GameVersion.GSC }, // Koffing @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 100, Level = 23, Location = 036, Version = GameVersion.GSC }, // Voltorb @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 101, Level = 23, Location = 036, Version = GameVersion.GSC }, // Electrode @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 143, Level = 50, Location = 061, Version = GameVersion.GSC }, // Snorlax @ Vermillion City

            new EncounterStatic { Species = 211, Level = 05, Location = 008, Version = GameVersion.GSC }, // Qwilfish Swarm @ Route 32 (Old Rod)
            new EncounterStatic { Species = 211, Level = 20, Location = 008, Version = GameVersion.GSC }, // Qwilfish Swarm @ Route 32 (Good Rod)
            new EncounterStatic { Species = 211, Level = 40, Location = 008, Version = GameVersion.GSC }, // Qwilfish Swarm @ Route 32 (Super Rod)

            new EncounterStatic { Species = 083, Level = 05, Moves = new [] { 226, 14, 97, 163 }, Version = GameVersion.Stadium2 }, // Stadium 2 Baton Pass Farfetch'd
            new EncounterStatic { Species = 207, Level = 05, Moves = new [] { 89, 68, 17 }, Version = GameVersion.Stadium2 }, // Stadium 2 Earthquake Gligar

            // Gen2 Events
            // Pokémon Center Mystery Egg #1 (December 15, 2001 to January 14, 2002)
            new EncounterStatic { Species = 152, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Chikorita Petal Dance
            new EncounterStatic { Species = 173, Level = 05, Moves = new [] { 129 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Cleffa Swift
            new EncounterStatic { Species = 194, Level = 05, Moves = new [] { 187 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Wooper Belly Drum
            new EncounterStatic { Species = 231, Level = 05, Moves = new [] { 227 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Phanpy Encore
            new EncounterStatic { Species = 238, Level = 05, Moves = new [] { 118 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Smoochum Metronome

            // Pokémon Center Mystery Egg #2 (March 16 to April 7, 2002)
            new EncounterStatic { Species = 047, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Psyduck Petal Dance
            // new EncounterStatic { Species = 152, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Chikorita Petal Dance
            new EncounterStatic { Species = 172, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Pichu Petal Dance
            new EncounterStatic { Species = 173, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Cleffa Petal Dance
            new EncounterStatic { Species = 174, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Igglybuff Petal Dance
            new EncounterStatic { Species = 238, Level = 05, Moves = new [] { 080 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Smoochum Petal Dance

            // Pokémon Center Mystery Egg #3 (April 27 to May 12, 2002)
            new EncounterStatic { Species = 001, Level = 05, Moves = new [] { 246 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Bulbasaur Ancientpower
            new EncounterStatic { Species = 004, Level = 05, Moves = new [] { 242 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Charmander Crunch
            new EncounterStatic { Species = 158, Level = 05, Moves = new [] { 066 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Totodile Submission
            new EncounterStatic { Species = 163, Level = 05, Moves = new [] { 101 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Hoot-Hoot Night Shade
            new EncounterStatic { Species = 158, Level = 05, Moves = new [] { 047 }, Version = GameVersion.EventsGBGen2, EggLocation = 256 }, // Pichu Sing
        };

        private static readonly EncounterStatic[] Encounter_GS_Exclusive =
        {
            new EncounterStatic { Species = 245, Level = 40, Version = GameVersion.GS }, // Suicune

            new EncounterStatic { Species = 249, Level = 70, Version = GameVersion.GD }, // Lugia @ Whirl Islands
            new EncounterStatic { Species = 249, Level = 40, Version = GameVersion.SV }, // Lugia @ Whirl Islands

            new EncounterStatic { Species = 250, Level = 40, Version = GameVersion.GD }, // Ho-Oh @ Tin Tower
            new EncounterStatic { Species = 250, Level = 70, Version = GameVersion.SV }, // Ho-Oh @ Tin Tower

            new EncounterStatic { Species = 137, Level = 15, Version = GameVersion.GS }, // Porygon @ Celadon Game Corner
            new EncounterStatic { Species = 133, Level = 15, Version = GameVersion.GS }, // Eevee @ Celadon Game Corner
            new EncounterStatic { Species = 122, Level = 15, Version = GameVersion.GS }, // Mr. Mime @ Celadon Game Corner

            new EncounterStatic { Species = 063, Level = 10, Version = GameVersion.GS }, // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic { Species = 147, Level = 10, Version = GameVersion.GS }, // Dratini @ Goldenrod City (Game Corner)
            new EncounterStatic { Species = 023, Level = 10, Version = GameVersion.GS }, // Ekans @ Goldenrod City (Game Corner) (Gold)
            new EncounterStatic { Species = 027, Level = 10, Version = GameVersion.GS }, // Sandshrew @ Goldenrod City (Game Corner) (Silver)

            new EncounterStatic { Species = 223, Level = 05, Version = GameVersion.GS }, // Remoraid Swarm @ Route 44 (Old Rod)
            new EncounterStatic { Species = 223, Level = 20, Version = GameVersion.GS }, // Remoraid Swarm @ Route 44 (Good Rod)
            new EncounterStatic { Species = 223, Level = 40, Version = GameVersion.GS }, // Remoraid Swarm @ Route 44 (Super Rod)
        };

        private static readonly EncounterStatic[] Encounter_C_Exclusive =
        {
            new EncounterStatic { Species = 245, Level = 40, Location = 023, Version = GameVersion.C }, // Suicune @ Tin Tower

            new EncounterStatic { Species = 172, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Pichu Dizzy Punch
            new EncounterStatic { Species = 173, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Cleffa Dizzy Punch
            new EncounterStatic { Species = 174, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Igglybuff Dizzy Punch
            new EncounterStatic { Species = 236, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Tyrogue Dizzy Punch
            new EncounterStatic { Species = 238, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Smoochum Dizzy Punch
            new EncounterStatic { Species = 239, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Elekid Dizzy Punch
            new EncounterStatic { Species = 240, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256, EggCycles = 20 }, // Magby Dizzy Punch

            new EncounterStatic { Species = 147, Level = 15, Location = 042, Version = GameVersion.C, Moves = new [] {245} }, // Dratini ExtremeSpeed

            new EncounterStatic { Species = 249, Level = 60, Location = 031, Version = GameVersion.C }, // Lugia @ Whirl Islands
            new EncounterStatic { Species = 250, Level = 60, Location = 023, Version = GameVersion.C }, // Ho-Oh @ Tin Tower
            new EncounterStatic { Species = 251, Level = 30, Location = 014, Version = GameVersion.C }, // Celebi @ Ilex Forest (VC)
            new EncounterStatic { Species = 251, Level = 30, Location = 014, Version = GameVersion.EventsGBGen2 }, // Celebi @ Ilex Forest (GBC)

            new EncounterStatic { Species = 137, Level = 15, Location = 071, Version = GameVersion.C }, // Porygon @ Celadon Game Corner
            new EncounterStatic { Species = 025, Level = 25, Location = 071, Version = GameVersion.C }, // Pikachu @ Celadon Game Corner
            new EncounterStatic { Species = 246, Level = 40, Location = 071, Version = GameVersion.C }, // Larvitar @ Celadon Game Corner

            new EncounterStatic { Species = 063, Level = 05, Location = 016, Version = GameVersion.C }, // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic { Species = 104, Level = 15, Location = 016, Version = GameVersion.C }, // Cubone @ Goldenrod City (Game Corner)
            new EncounterStatic { Species = 202, Level = 15, Location = 016, Version = GameVersion.C }, // Wobbuffet @ Goldenrod City (Game Corner)
        };

        private static readonly EncounterStatic[] Encounter_GSC_Roam =
        {
            new EncounterStatic { Species = 243, Level = 40, Roaming = true }, // Raikou
            new EncounterStatic { Species = 244, Level = 40, Roaming = true }, // Entei
            new EncounterStatic { Species = 245, Level = 40, Roaming = true, Version = GameVersion.GS }, // Suicune
        };

        private static readonly EncounterStatic[] Encounter_GS = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_GSC_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_GSC_Grass))).ToArray();
        private static readonly EncounterStatic[] Encounter_C = Encounter_GSC_Common.Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam.Take(2).SelectMany(e => e.Clone(Roaming_MetLocation_GSC_Grass))).ToArray();
        private static readonly EncounterStatic[] Encounter_GSC = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_GSC_Grass))).ToArray();

        internal static readonly EncounterTrade[] TradeGift_GSC =
        {
            new EncounterTrade { Species = 095, Level = 03, Gender = 0, TID = 48926, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Onix @ Violet City for Bellsprout [wild]
            new EncounterTrade { Species = 066, Level = 05, Gender = 1, TID = 37460, IVs = new[] {12, 03, 07, 06, 06, 06} }, // Machop @ Goldenrod City for Drowzee [wild 9, hatched egg 5]
            new EncounterTrade { Species = 100, Level = 05, Gender = 2, TID = 29189, IVs = new[] {08, 09, 08, 08, 08, 08} }, // Voltorb @ Olivine City for Krabby [egg]
            new EncounterTrade { Species = 112, Level = 10, Gender = 1, TID = 00283, IVs = new[] {12, 07, 07, 06, 06, 06} }, // Rhydon @ Blackthorn City for Dragonair [wild]
            new EncounterTrade { Species = 142, Level = 05, Gender = 0, TID = 26491, IVs = new[] {08, 09, 06, 06, 06, 06}, OTGender = 1}, // Aerodactyl @ Route 14 for Chansey [egg]
            new EncounterTrade { Species = 078, Level = 14, Gender = 0, TID = 15616, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Rapidash @ Pewter City for Gloom [wild]

            new EncounterTrade { Species = 085, Level = 10, Gender = 1, TID = 00283, IVs = new[] {12, 07, 07, 06, 06, 06}, OTGender = 1}, // Dodrio @ Blackthorn City for Dragonair [wild]
            new EncounterTrade { Species = 178, Level = 15, Gender = 0, TID = 15616, IVs = new[] {08, 09, 06, 08, 06, 06} }, // Xatu @ Pewter City for Haunter [wild]
            new EncounterTrade { Species = 082, Level = 05, Gender = 2, TID = 50082, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Magneton @ Power Plant for Dugtrio [traded for Lickitung]

            new EncounterTrade { Species = 021, Level = 10, TID = 01001, Moves = new[] {64,45,43} }, // Spearow @ Goldenrod City for free
            new EncounterTrade { Species = 213, Level = 15, TID = 00518 }, // Shuckle @ Cianwood City for free
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
