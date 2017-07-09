using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_2 = 251;
        internal const int MaxMoveID_2 = 251;
        internal const int MaxItemID_2 = 255;
        internal const int MaxAbilityID_2 = 0;
        
        internal static readonly ushort[] Pouch_Items_GSC = {
            3, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 46, 47, 48, 49, 51, 52, 53, 57, 60, 62, 63, 64, 65, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92, 93, 94, 95, 96, 97, 98, 99, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 117, 118, 119, 121, 122, 123, 124, 125, 126, 131, 132, 138, 139, 140, 143, 144, 146, 150, 151, 152, 156, 158, 163, 168, 169, 170, 172, 173, 174, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189
        };
        internal static readonly ushort[] Pouch_Ball_GSC = {
            1, 2, 4, 5, 157, 159, 160, 161, 164, 165, 166
        };
        internal static readonly ushort[] Pouch_Key_GS = {
            7, 54, 55, 58, 59, 61, 66, 67, 68, 69, 71, 127, 128, 130, 133, 134, 175, 178
        };
        internal static readonly ushort[] Pouch_Key_C = Pouch_Key_GS.Concat(new ushort[]{70, 115, 116, 129}).ToArray();
        internal static readonly ushort[] Pouch_TMHM_GSC = {
            191, 192, 193, 194, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249
        };

        internal static readonly ushort[] HeldItems_GSC = new ushort[1].Concat(Pouch_Items_GSC).Concat(Pouch_Ball_GSC).Concat(Pouch_TMHM_GSC).ToArray();

        internal static readonly int[] MovePP_GSC =
        {
            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 10, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 20, 10, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 20, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 15,
            10, 05, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15,
            10, 00, 00, 00, 00
        };
        internal static readonly int[] TMHM_GSC =
        {
            223, 029, 174, 205, 046, 092, 192, 249, 244, 237,
            241, 230, 173, 059, 063, 196, 182, 240, 202, 203,
            218, 076, 231, 225, 087, 089, 216, 091, 094, 247,
            189, 104, 008, 207, 214, 188, 201, 126, 129, 111,
            009, 138, 197, 156, 213, 168, 211, 007, 210, 171,

            015, 019, 057, 070, 148, 250, 127
        };
        internal static readonly int[] Tutors_GSC = {53, 85, 58}; // Flamethrower, Thunderbolt & Ice Beam
        internal static readonly int[] WildPokeBalls2 = { 4 };

        internal static readonly int[] FutureEvolutionsGen2 =
        {
            424,429,430,461,462,463,464,465,466,467,468,469,470,471,472,473,474,700
        };
        internal static readonly int[] Roaming_MetLocation_GSC_Grass =
        {
            // Routes 29, 30-31, 33, 34, 35, 36-37, 38-39, 42, 43, 44, 45-46 can be encountered in grass
            2, 4, 5, 8, 11, 15, 18, 20, 21,
            25, 26, 34, 37, 39, 43, 45,
        };

        internal static readonly EncounterArea[] EncounterBCC_GSC = { new EncounterArea {
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

        internal static readonly EncounterArea[] EncounterSafari_GSC = { new EncounterArea {
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

        internal static readonly EncounterStatic[] Encounter_GSC_Common =
        {
            new EncounterStatic { Species = 152, Level = 05, Location = 001, Version = GameVersion.GSC }, // Chikorita @ New Bark Town
            new EncounterStatic { Species = 155, Level = 05, Location = 001, Version = GameVersion.GSC }, // Cyndaquil @ New Bark Town
            new EncounterStatic { Species = 158, Level = 05, Location = 001, Version = GameVersion.GSC }, // Totodile @ New Bark Town
            
            new EncounterStatic { Species = 175, Level = 05, Version = GameVersion.GSC, EggLocation = 256 }, // Togepi
            new EncounterStatic { Species = 131, Level = 20, Location = 010, Version = GameVersion.GSC }, // Lapras @ Union Cave
            new EncounterStatic { Species = 133, Level = 20, Location = 016, Version = GameVersion.GSC }, // Eevee @ Goldenrod City
            
            new EncounterStatic { Species = 185, Level = 20, Location = 020, Version = GameVersion.GSC }, // Sudowoodo @ Route 36
            new EncounterStatic { Species = 236, Level = 10, Location = 035, Version = GameVersion.GSC }, // Tyrogue @ Mt. Mortar
            
            new EncounterStatic { Species = 130, Level = 30, Location = 038, Version = GameVersion.GSC, Shiny = true, }, // Gyarados @ Lake of Rage
            new EncounterStatic { Species = 074, Level = 21, Location = 036, Version = GameVersion.GSC }, // Geodude @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 109, Level = 21, Location = 036, Version = GameVersion.GSC }, // Koffing @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 100, Level = 23, Location = 036, Version = GameVersion.GSC }, // Voltorb @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 101, Level = 23, Location = 036, Version = GameVersion.GSC }, // Electrode @ Rocket Hideout (Mahogany Town)
            new EncounterStatic { Species = 143, Level = 50, Location = 061, Version = GameVersion.GSC }, // Snorlax @ Vermillion City
            
            new EncounterStatic { Species = 083, Level = 05, Moves = new [] { 226, 14, 97, 37 }, Version = GameVersion.Stadium2 }, // Stadium 2 Baton Pass Farfetch'd
            new EncounterStatic { Species = 207, Level = 05, Moves = new [] { 89, 68, 17 }, Version = GameVersion.Stadium2 }, // Stadium 2 Earthquake Gligar
        };

        internal static readonly EncounterStatic[] Encounter_GS_Exclusive = 
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
        };

        internal static readonly EncounterStatic[] Encounter_C_Exclusive = 
        {
            new EncounterStatic { Species = 245, Level = 40, Location = 023, Version = GameVersion.C }, // Suicune @ Tin Tower
            
            new EncounterStatic { Species = 172, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Pichu Dizzy Punch
            new EncounterStatic { Species = 173, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Cleffa Dizzy Punch
            new EncounterStatic { Species = 174, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Igglybuff Dizzy Punch
            new EncounterStatic { Species = 236, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Tyrogue Dizzy Punch
            new EncounterStatic { Species = 238, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Smoochum Dizzy Punch
            new EncounterStatic { Species = 239, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Elekid Dizzy Punch
            new EncounterStatic { Species = 240, Level = 05, Version = GameVersion.C, Moves = new [] {146}, EggLocation = 256 }, // Magby Dizzy Punch
            
            new EncounterStatic { Species = 147, Level = 15, Location = 042, Version = GameVersion.C, Moves = new [] {245} }, // Dratini ExtremeSpeed
            
            new EncounterStatic { Species = 249, Level = 60, Location = 031, Version = GameVersion.C }, // Lugia @ Whirl Islands
            new EncounterStatic { Species = 250, Level = 60, Location = 023, Version = GameVersion.C }, // Ho-Oh @ Tin Tower
            new EncounterStatic { Species = 251, Level = 30, Location = 014, Version = GameVersion.EventsGBGen2 }, // Celebi @ Ilex Forest

            new EncounterStatic { Species = 137, Level = 15, Location = 071, Version = GameVersion.C }, // Porygon @ Celadon Game Corner
            new EncounterStatic { Species = 025, Level = 25, Location = 071, Version = GameVersion.C }, // Pikachu @ Celadon Game Corner
            new EncounterStatic { Species = 246, Level = 40, Location = 071, Version = GameVersion.C }, // Larvitar @ Celadon Game Corner

            new EncounterStatic { Species = 063, Level = 05, Location = 016, Version = GameVersion.C }, // Abra @ Goldenrod City (Game Corner)
            new EncounterStatic { Species = 104, Level = 15, Location = 016, Version = GameVersion.C }, // Cubone @ Goldenrod City (Game Corner)
            new EncounterStatic { Species = 202, Level = 15, Location = 016, Version = GameVersion.C }, // Wobbuffet @ Goldenrod City (Game Corner)
        };

        internal static readonly EncounterStatic[] Encounter_GSC_Roam =
        {
            new EncounterStatic { Species = 243, Level = 40, Roaming = true }, // Raikou
            new EncounterStatic { Species = 244, Level = 40, Roaming = true }, // Entei
            new EncounterStatic { Species = 245, Level = 40, Roaming = true, Version = GameVersion.GS }, // Suicune
        };

        internal static readonly EncounterStatic[] Encounter_GS = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_GSC_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_GSC_Grass))).ToArray();
        internal static readonly EncounterStatic[] Encounter_C = Encounter_GSC_Common.Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam.Take(2).SelectMany(e => e.Clone(Roaming_MetLocation_GSC_Grass))).ToArray();
        internal static readonly EncounterStatic[] Encounter_GSC = Encounter_GSC_Common.Concat(Encounter_GS_Exclusive).Concat(Encounter_C_Exclusive).Concat(Encounter_GSC_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_GSC_Grass))).ToArray();

        internal static readonly EncounterTrade[] TradeGift_GSC =
        {
            new EncounterTrade { Species = 095, Generation = 2, Level = 03, Gender = 0, TID = 48926, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Onix @ Violet City for Bellsprout [wild]
            new EncounterTrade { Species = 066, Generation = 2, Level = 05, Gender = 1, TID = 37460, IVs = new[] {12, 03, 07, 06, 06, 06} }, // Machop @ Goldenrod City for Drowzee [wild 9, hatched egg 5]
            new EncounterTrade { Species = 100, Generation = 2, Level = 05, Gender = 2, TID = 29189, IVs = new[] {08, 09, 08, 08, 08, 08} }, // Voltorb @ Olivine City for Krabby [egg]
            new EncounterTrade { Species = 112, Generation = 2, Level = 30, Gender = 0, TID = 00283, IVs = new[] {12, 07, 07, 06, 06, 06} }, // Rhydon @ Blackthorn City for Dragonair [blue jp game corner]
            new EncounterTrade { Species = 142, Generation = 2, Level = 05, Gender = 0, TID = 26491, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Aerodactyl @ Route 14 for Chansey [egg]
            new EncounterTrade { Species = 078, Generation = 2, Level = 14, Gender = 0, TID = 15616, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Rapidash @ Pewter City for Gloom [wild]

            new EncounterTrade { Species = 085, Generation = 2, Level = 30, Gender = 0, TID = 00283, IVs = new[] {12, 07, 07, 06, 06, 06} }, // Dodrio @ Blackthorn City for Dragonair [blue jp game corner]
            new EncounterTrade { Species = 178, Generation = 2, Level = 15, Gender = 0, TID = 15616, IVs = new[] {08, 09, 06, 08, 06, 06} }, // Xatu @ Pewter City for Haunter [wild]
            new EncounterTrade { Species = 082, Generation = 2, Level = 16, Gender = 2, TID = 50082, IVs = new[] {08, 09, 06, 06, 06, 06} }, // Magneton @ Power Plant for Dugtrio [wild]
            
            new EncounterTrade { Species = 213, Generation = 2, Level = 15, TID = 00518 } // Shuckle @ Cianwood City for free
        };
        internal static readonly string[][] TradeGift_GSC_OTs =
        {
            new[] { "コンタ", "KYLE" },
            new[] { "ナオキ", "MIKE" },
            new[] { "ゲン", "TIM" },
            new[] { "ミサコ", "EMY" },
            new[] { "キヨミ", "KIM" },
            new[] { "デンジ", "CHRIS" },

            new[] { "ミサコ", "EMY" },
            new[] { "デンジ", "CHRIS" },
            new[] { "モリオ", "FOREST" },

            new[] { "セイジ", "MANIA" },
        };
        internal static readonly int[] UnreleasedItems_2 =
        {
            // todo
        };
        internal static readonly bool[] ReleasedHeldItems_2 = Enumerable.Range(0, MaxItemID_2+1).Select(i => HeldItems_GSC.Contains((ushort)i) && !UnreleasedItems_2.Contains(i)).ToArray();
        internal static readonly int[] TransferSpeciesDefaultAbility_2 = {92, 93, 94, 109, 110, 151, 200, 201, 247, 251}; // todo VC2: 247 (pupitar)
    }
}
