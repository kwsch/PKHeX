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
        private static readonly EncounterArea2[] SlotsG = Get("gold", "g2", GameVersion.GD);
        private static readonly EncounterArea2[] SlotsS = Get("silver", "g2", GameVersion.SV);
        internal static readonly EncounterArea2[] SlotsC = Get("crystal", "g2", GameVersion.C);

        internal static readonly EncounterArea2[] SlotsGS = ArrayUtil.ConcatAll(SlotsG, SlotsS);
        internal static readonly EncounterArea2[] SlotsGSC = ArrayUtil.ConcatAll(SlotsGS, SlotsC);
        private static readonly TreesArea[] HeadbuttTreesC = TreesArea.GetArray(BinLinker.Unpack(Util.GetBinaryResource("trees_h_c.pkl"), "ch"));
        private static EncounterArea2[] Get(string name, string ident, GameVersion game) =>
            EncounterArea2.GetAreas(BinLinker.Unpack(Util.GetBinaryResource($"encounter_{name}.pkl"), ident), game);

        static Encounters2()
        {
            MarkEncountersGeneration(2, StaticGS, StaticC, StaticGSC, TradeGift_GSC);

            MarkEncounterTradeStrings(TradeGift_GSC, TradeGift_GSC_OTs);

            StaticGSC.SetVersion(GameVersion.GSC);
            StaticGS.SetVersion(GameVersion.GS);
            StaticC.SetVersion(GameVersion.C);
            TradeGift_GSC.SetVersion(GameVersion.GSC);
        }

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

            var table = Area.GetTrees(encounter.Area!.Type);
            var trainerpivot = TID % 10;
            return table[trainerpivot];
        }

        internal static readonly EncounterStatic2[] StaticGSC = Encounter_GSC;
        internal static readonly EncounterStatic2[] StaticGS = Encounter_GS;
        internal static readonly EncounterStatic2[] StaticC = Encounter_C;
    }
}
