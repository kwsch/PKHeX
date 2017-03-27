using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesIndex_3 = 412;
        internal const int MaxSpeciesID_3 = 386;
        internal const int MaxMoveID_3 = 354;
        internal const int MaxItemID_3 = 374;
        internal const int MaxAbilityID_3 = 77;
        internal const int MaxBallID_3 = 0xC;

        public static readonly int[] SplitBreed_3 =
        {
            // Incense
            183, 184, // Marill
            202, // Wobbuffet
        };

        #region RS
        internal static readonly ushort[] Pouch_Items_RS = {
            13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76, 77, 78, 79, 80, 81, 83, 84, 85, 86, 93, 94, 95, 96, 97, 98, 103, 104, 106, 107, 108, 109, 110, 111, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 254, 255, 256, 257, 258
        };
        internal static readonly ushort[] Pouch_Key_RS = {
            259, 260, 261, 262, 263, 264, 265, 266, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288
        };
        internal static readonly ushort[] Pouch_TM_RS = {
            289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338,
        };

        public static readonly ushort[] Pouch_HM_RS = {
            339, 340, 341, 342, 343, 344, 345, 346
        };
        internal static readonly ushort[] Pouch_Berries_RS = {
            133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175
        };
        internal static readonly ushort[] Pouch_Ball_RS = {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        };
        internal static readonly ushort[] Pouch_Key_FRLG = Pouch_Key_RS.Concat(new ushort[] { 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374 }).ToArray();
        internal static readonly ushort[] Pouch_Key_E = Pouch_Key_FRLG.Concat(new ushort[] { 375, 376 }).ToArray();

        internal static readonly ushort[] Pouch_TMHM_RS = Pouch_TM_RS.Concat(Pouch_HM_RS).ToArray();
        internal static readonly ushort[] HeldItems_RS = new ushort[1].Concat(Pouch_Items_RS).Concat(Pouch_Ball_RS).Concat(Pouch_Berries_RS).Concat(Pouch_TM_RS).ToArray();
        #endregion

        internal static readonly int[] MovePP_RS =
        {
            00, 
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 10, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20, 
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 20, 10, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20, 
            15, 10, 40, 15, 20, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40, 
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 15, 
            10, 05, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15, 
            10, 10, 10, 10, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15, 
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 10, 05, 40, 15, 20, 20, 05, 15, 20, 30, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 40, 15, 20, 15, 20, 15, 20, 10, 
            20, 20, 05, 05, 
        };

        internal static readonly ushort[] Pouch_Cologne_CXD = {543, 544, 545};
        internal static readonly ushort[] Pouch_Items_COLO = Pouch_Items_RS.Concat(new ushort[] {537}).ToArray(); // Time Flute
        internal static readonly ushort[] HeldItems_COLO = new ushort[1].Concat(Pouch_Items_COLO).Concat(Pouch_Ball_RS).Concat(Pouch_Berries_RS).Concat(Pouch_TM_RS).ToArray();
        internal static readonly ushort[] Pouch_Key_COLO =
        {
            500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
            510, 511, 512, 513, 514, 515, 516, 517, 518, 519,
            520, 521, 522, 523, 524, 525, 526, 527, 528, 529,
            530, 531, 532, 533, 534, 535, 536,      538, 539,
            540, 541, 542,                546, 547,
        };

        internal static readonly ushort[] Pouch_Items_XD = Pouch_Items_RS.Concat(new ushort[] {511}).ToArray(); // Poké Snack
        internal static readonly ushort[] HeldItems_XD = new ushort[1].Concat(Pouch_Items_XD).Concat(Pouch_Ball_RS).Concat(Pouch_Berries_RS).Concat(Pouch_TM_RS).ToArray();
        internal static readonly ushort[] Pouch_Key_XD =
        {
            500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
            510,      512,                516, 517, 518, 519,
                           523, 524, 525, 526, 527, 528, 529,
            530, 531, 532, 533
        };
        internal static readonly ushort[] Pouch_Disc_XD =
        {
            534, 535, 536, 537, 538, 539,
            540, 541, 542, 543, 544, 545, 546, 547, 548, 549,
            550, 551, 552, 553, 554, 555, 556, 557, 558, 559,
            560, 561, 562, 563, 564, 565, 566, 567, 568, 569,
            570, 571, 572, 573, 574, 575, 576, 577, 578, 579,
            580, 581, 582, 583, 584, 585, 586, 587, 588, 589,
            590, 591, 592, 593
        };
        internal static readonly int[] WildPokeBalls3 = {1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12};

        internal static readonly int[] FutureEvolutionsGen3 =
        {
            407,424,429,430,461,462,463,464,465,466,467,468,469,470,471,472,473,474,475,476,477,700
        };

        internal static readonly int[] FutureEvolutionsGen3_LevelUpGen4 = 
        {
            424, 461, 462, 463, 465, 469, 470, 471, 472, 473, 476
        };
        // Ambipom Weavile Magnezone Lickilicky Tangrowth
        // Yanmega Leafeon Glaceon Mamoswine Gliscor Probopass
        internal static readonly int[] UnreleasedItems_3 =
        {
            // todo
        };
        internal static readonly bool[] ReleasedHeldItems_3 = Enumerable.Range(0, MaxItemID_3+1).Select(i => HeldItems_RS.Contains((ushort)i) && !UnreleasedItems_3.Contains(i)).ToArray();
        internal static readonly int[] TM_3 =
        {
            264, 337, 352, 347, 046, 092, 258, 339, 331, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
            218, 076, 231, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
            259, 263, 290, 156, 213, 168, 211, 285, 289, 315,
        };
        internal static readonly int[] HM_3 = {15, 19, 57, 70, 148, 249, 127, 291};
        internal static readonly int[] TypeTutor3 = {338, 307, 308};
        internal static readonly int[] Tutor_3Mew =
        {
            185, // Feint Attack
            252, // Fake Out
            095, // Hypnosis
            101, // Night Shade
            272, // Role Play
            192, // Zap Cannon
        };
        internal static readonly int[][] Tutor_Frontier =
        {
            new[] {135, 069, 138, 005, 025, 034, 157, 068, 086, 014},
            new[] {111, 173, 189, 129, 196, 203, 244, 008, 009, 007},
        };

        internal static readonly int[] Tutor_E =
        {
            005, 014, 025, 034, 038, 068, 069, 102, 118, 135,
            138, 086, 153, 157, 164, 223, 205, 244, 173, 196,
            203, 189, 008, 207, 214, 129, 111, 009, 007, 210
        };

        internal static readonly int[] Tutor_FRLG =
        {
            005, 014, 025, 034, 038, 068, 069, 102, 118, 135,
            138, 086, 153, 157, 164
        };

        internal static readonly int[] SpecialTutors_FRLG =
        {
            307, 308, 338
        };

        internal static readonly int[][] SpecialTutors_Compatibility_FRLG =
        {
            new[] { 6 },
            new[] { 9 },
            new[] { 3 },
        };

        // Tutor moves from XD that can be learned as tutor moves in emerald
        // For this moves compatibility data is the same in XD and Emerald
        internal static readonly int[] SpecialTutors_XD_Emerald =
        {
            034, 038, 069, 086, 102, 120, 138, 143, 164, 171, 196, 207,
        };

        internal static readonly int[] SpecialTutors_XD_Exclusive =
        {
            120, 143, 171
        };

        internal static readonly int[] SpecialTutors_XD = SpecialTutors_XD_Emerald.Concat(SpecialTutors_XD_Exclusive).ToArray();

        internal static readonly int[][] SpecialTutors_Compatibility_XD_Exclusive =
        {
            new[] { 074, 075, 076, 088, 089, 090, 091, 092, 093, 094, 095,
                    100, 101, 102, 103, 109, 110, 143, 150, 151, 185, 204,
                    205, 208, 211, 218, 219, 222, 273, 274, 275, 299, 316,
                    317, 320, 321, 323, 324, 337, 338, 343, 344, 362, 375,
                    376, 377, 378, 379 },

            new[] { 016, 017, 018, 021, 022, 084, 085, 142, 144, 145, 146,
                    151, 163, 164, 176, 177, 178, 198, 225, 227, 250, 276,
                    277, 278, 279, 333, 334 },

            new[] { 012, 035, 036, 039, 040, 052, 053, 063, 064, 065, 079,
                    080, 092, 093, 094, 096, 097, 102, 103, 108, 121, 122,
                    124, 131, 137, 150, 151, 163, 164, 173, 174, 177, 178,
                    190, 196, 197, 198, 199, 200, 203, 206, 215, 228, 229,
                    233, 234, 238, 248, 249, 250, 251, 280, 281, 282, 284,
                    292, 302, 315, 316, 317, 327, 353, 354, 355, 356, 358,
                    359, 385, 386 }
        };

        internal static readonly int[] Roaming_MetLocation_FRLG =
        {
             //Route 1-25 encounter is possible either in grass or on water
             101,102,103,104,105,106,107,108,109,110,
             111,112,113,114,115,116,117,118,119,120,
             121,122,123,124,125
        };

        internal static readonly int[] Roaming_MetLocation_RSE =
        {
              //Roaming encounter is possible in tall grass and on water
              //Route 101-138 
              16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
              26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
              36, 37, 38, 39, 40, 41, 42, 43, 44, 45,
              46, 47, 48, 49,
        };
        internal static readonly EncounterStatic[] Encounter_Box =
        {
            new EncounterStatic { Species = 333, Level = 05, EggLocation = 255, Version = GameVersion.RSBOX, Moves = new[]{206} }, // Swablu Egg with False Swipe
            new EncounterStatic { Species = 263, Level = 05, EggLocation = 255, Version = GameVersion.RSBOX, Moves = new[]{245} }, // Zigzagoon Egg with Extreme Speed
            new EncounterStatic { Species = 300, Level = 05, EggLocation = 255, Version = GameVersion.RSBOX, Moves = new[]{6} }, // Skitty Egg with Pay Day
            new EncounterStatic { Species = 172, Level = 05, EggLocation = 255, Version = GameVersion.RSBOX, Moves = new[]{57} }, // Pichu Egg with Surf
        };

        internal static readonly EncounterStatic[] Encounter_RSE_Roam =
        {
            new EncounterStatic { Species = 380, Level = 40, Version = GameVersion.S, Roaming = true }, // Latias
            new EncounterStatic { Species = 380, Level = 40, Version = GameVersion.E, Roaming = true }, // Latias
            new EncounterStatic { Species = 381, Level = 40, Version = GameVersion.R, Roaming = true }, // Latios
            new EncounterStatic { Species = 381, Level = 40, Version = GameVersion.E, Roaming = true }, // Latios
        };
        internal static readonly EncounterStatic[] Encounter_RSE_Regular =
        {
            // Starters 
            new EncounterStatic { Gift = true, Species = 152, Level = 05, Location = 000, Version = GameVersion.E, }, // Chikorita @ Littleroot Town
            new EncounterStatic { Gift = true, Species = 155, Level = 05, Location = 000, Version = GameVersion.E, }, // Cyndaquil
            new EncounterStatic { Gift = true, Species = 158, Level = 05, Location = 000, Version = GameVersion.E, }, // Totodile
            new EncounterStatic { Gift = true, Species = 252, Level = 05, Location = 016, }, // Treecko @ Route 101
            new EncounterStatic { Gift = true, Species = 255, Level = 05, Location = 016, }, // Torchic
            new EncounterStatic { Gift = true, Species = 258, Level = 05, Location = 016, }, // Mudkip

            // Fossil @ Rustboro City
            new EncounterStatic { Gift = true, Species = 345, Level = 20, Location = 010, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20, Location = 010, }, // Anorith

            // Gift
            new EncounterStatic { Gift = true, Species = 351, Level = 25, Location = 034, }, // Castform @ Weather Institute
            new EncounterStatic { Gift = true, Species = 374, Level = 05, Location = 013, }, // Beldum @ Mossdeep City
            new EncounterStatic { Gift = true, Species = 360, Level = 05, EggLocation = 253}, // Wynaut Egg

            // Stationary
            new EncounterStatic { Species = 352, Level = 30, Location = 034, }, // Kecleon @ Route 119
            new EncounterStatic { Species = 352, Level = 30, Location = 035, }, // Kecleon @ Route 120
            new EncounterStatic { Species = 101, Level = 30, Location = 066, Version = GameVersion.RS, }, // Electrode @ Hideout (R:Magma Hideout/S:Aqua Hideout)
            new EncounterStatic { Species = 101, Level = 30, Location = 197, Version = GameVersion.E,  }, // Electrode @ Aqua Hideout
            new EncounterStatic { Species = 185, Level = 40, Location = 058, Version = GameVersion.E,  }, // Sudowoodo @ Battle Frontier

            // Stationary Lengendary
            new EncounterStatic { Species = 377, Level = 40, Location = 082, }, // Regirock @ Desert Ruins
            new EncounterStatic { Species = 378, Level = 40, Location = 081, }, // Regice @ Island Cave
            new EncounterStatic { Species = 379, Level = 40, Location = 083, }, // Registeel @ Ancient Tomb
            new EncounterStatic { Species = 380, Level = 50, Location = 073, Version = GameVersion.R, }, // Latias @ Southern Island
            new EncounterStatic { Species = 380, Level = 50, Location = 073, Version = GameVersion.E, }, // Latias @ Southern Island
            new EncounterStatic { Species = 381, Level = 50, Location = 073, Version = GameVersion.S, }, // Latios @ Southern Island
            new EncounterStatic { Species = 381, Level = 50, Location = 073, Version = GameVersion.E, }, // Latios @ Southern Island
            new EncounterStatic { Species = 382, Level = 45, Location = 072, Version = GameVersion.S, }, // Kyogre @ Cave of Origin
            new EncounterStatic { Species = 382, Level = 70, Location = 203, Version = GameVersion.E, }, // Kyogre @ Marine Cave
            new EncounterStatic { Species = 383, Level = 45, Location = 072, Version = GameVersion.R, }, // Groudon @ Cave of Origin
            new EncounterStatic { Species = 383, Level = 70, Location = 205, Version = GameVersion.E, }, // Groudon @ Terra Cave
            new EncounterStatic { Species = 384, Level = 70, Location = 085, }, // Rayquaza @ Sky Pillar

            // Event
            new EncounterStatic { Species = 151, Level = 30, Location = 201, Version = GameVersion.E, Fateful = true }, // Mew @ Faraway Island (Unreleased outside of Japan)
            new EncounterStatic { Species = 249, Level = 70, Location = 211, Version = GameVersion.E, }, // Lugia @ Navel Rock
            new EncounterStatic { Species = 250, Level = 70, Location = 211, Version = GameVersion.E, }, // Ho-Oh @ Navel Rock
            new EncounterStatic { Species = 386, Level = 30, Location = 200, Version = GameVersion.E, Form = 3, Fateful = true }, // Deoxys @ Birth Island
        };


        internal static readonly EncounterStatic[] Encounter_FRLG_Roam =
        {
            new EncounterStatic { Species = 243, Level = 50, Roaming = true, }, // Raikou
            new EncounterStatic { Species = 244, Level = 50, Roaming = true, }, // Entei
            new EncounterStatic { Species = 245, Level = 50, Roaming = true, }, // Suicune
        };
        internal static readonly EncounterStatic[] Encounter_FRLG_Stationary =
        {
            // Starters @ Pallet Town
            new EncounterStatic { Gift = true, Species = 1, Level = 05, Location = 088, }, // Bulbasaur 
            new EncounterStatic { Gift = true, Species = 4, Level = 05, Location = 088, }, // Charmander
            new EncounterStatic { Gift = true, Species = 9, Level = 05, Location = 088, }, // Squirtle

            // Fossil @ Cinnabar Island
            new EncounterStatic { Gift = true, Species = 138, Level = 05, Location = 096, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 05, Location = 096, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 05, Location = 096, }, // Aerodactyl

            // Gift
            new EncounterStatic { Gift = true, Species = 106, Level = 25, Location = 098, }, // Hitmonlee @ Saffron City
            new EncounterStatic { Gift = true, Species = 107, Level = 25, Location = 098, }, // Hitmonchan @ Saffron City
            new EncounterStatic { Gift = true, Species = 129, Level = 05, Location = 099, }, // Magikarp @ Route 4
            new EncounterStatic { Gift = true, Species = 131, Level = 25, Location = 134, }, // Lapras @ Silph Co.
            new EncounterStatic { Gift = true, Species = 133, Level = 25, Location = 094, }, // Eevee @ Celadon City
            new EncounterStatic { Gift = true, Species = 175, Level = 05, EggLocation = 253 }, // Togepi Egg

            // Stationary
            new EncounterStatic { Species = 143, Level = 30, Location = 112, }, // Snorlax @ Route 12
            new EncounterStatic { Species = 143, Level = 30, Location = 116, }, // Snorlax @ Route 16
            new EncounterStatic { Species = 101, Level = 34, Location = 142, }, // Electrode @ Power Plant
            new EncounterStatic { Species = 097, Level = 30, Location = 176, }, // Hypno @ Berry Forest

            // Stationary Lengerdary
            new EncounterStatic { Species = 144, Level = 50, Location = 139, }, // Articuno @ Seafoam Islands
            new EncounterStatic { Species = 145, Level = 50, Location = 142, }, // Zapdos @ Power Plant
            new EncounterStatic { Species = 146, Level = 50, Location = 175, }, // Moltres @ Mt. Ember. 
            new EncounterStatic { Species = 150, Level = 70, Location = 141, }, // Mewtwo @ Cerulean Cave

            // Event
            new EncounterStatic { Species = 249, Level = 70, Location = 174, }, // Lugia @ Navel Rock
            new EncounterStatic { Species = 250, Level = 70, Location = 174, }, // Ho-Oh @ Navel Rock
            new EncounterStatic { Species = 386, Level = 30, Location = 187, Version = GameVersion.FR, Form = 1, Fateful = true }, // Deoxys @ Birth Island
            new EncounterStatic { Species = 386, Level = 30, Location = 187, Version = GameVersion.LG, Form = 2, Fateful = true }, // Deoxys @ Birth Island
        };

        internal static readonly EncounterStatic[] Encounter_RSE = Encounter_RSE_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_RSE)).Concat(Encounter_RSE_Regular).Concat(Encounter_Box).ToArray();
        internal static readonly EncounterStatic[] Encounter_FRLG = Encounter_FRLG_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_FRLG)).Concat(Encounter_FRLG_Stationary).Concat(Encounter_Box).ToArray();

        private static readonly int[] TradeContest_Cool =   {30, 05, 05, 05, 05, 10};
        private static readonly int[] TradeContest_Beauty = {05, 30, 05, 05, 05, 10};
        private static readonly int[] TradeContest_Cute =   {05, 05, 30, 05, 05, 10};
        private static readonly int[] TradeContest_Clever = {05, 05, 05, 30, 05, 10};
        private static readonly int[] TradeContest_Tough =  {05, 05, 05, 05, 30, 10};
        internal static readonly EncounterTrade[] TradeGift_RSE =
        {
            new EncounterTrade { Species = 296, Ability = 2, TID = 49562, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,5,4,4,4,4}, Nature = Nature.Hardy, Contest = TradeContest_Tough, Version = GameVersion.RS, }, // Makuhita
            new EncounterTrade { Species = 300, Ability = 1, TID = 02259, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {5,4,4,5,4,4}, Nature = Nature.Timid, Contest = TradeContest_Cute, Version = GameVersion.RS, }, // Skitty
            new EncounterTrade { Species = 222, Ability = 2, TID = 50183, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {4,4,5,4,4,5}, Nature = Nature.Calm, Contest = TradeContest_Beauty, Version = GameVersion.RS, }, // Corsola
            new EncounterTrade { Species = 273, Ability = 2, TID = 38726, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,5,4,4,4}, Nature = Nature.Relaxed, Contest = TradeContest_Cool, Version = GameVersion.E, }, // Seedot
            new EncounterTrade { Species = 311, Ability = 1, TID = 08460, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {4,4,4,5,5,4}, Nature = Nature.Hasty, Contest = TradeContest_Cute, Version = GameVersion.E, }, // Plusle
            new EncounterTrade { Species = 116, Ability = 1, TID = 46285, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,4,4,5,4}, Nature = Nature.Brave, Contest = TradeContest_Tough, Version = GameVersion.E, }, // Horsea*
            new EncounterTrade { Species = 052, Ability = 1, TID = 25945, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {4,5,4,5,4,4}, Nature = Nature.Naive, Contest = TradeContest_Clever, Version = GameVersion.E, }, // Meowth*
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };
        internal static readonly EncounterTrade[] TradeGift_FRLG =
        {
            new EncounterTrade { Species = 122, Ability = 1, TID = 01985, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,15,17,24,23,22}, Nature = Nature.Timid, Contest = TradeContest_Clever,}, // Mr. Mime
            new EncounterTrade { Species = 029, Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {22,18,25,19,15,22}, Nature = Nature.Bold, Contest = TradeContest_Tough, Version = GameVersion.FR, }, // Nidoran♀
            new EncounterTrade { Species = 032, Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {19,25,18,22,22,15}, Nature = Nature.Lonely, Contest = TradeContest_Cool, Version = GameVersion.LG, }, // Nidoran♂ *
            new EncounterTrade { Species = 030, Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,25,18,19,22,15}, Nature = Nature.Lonely, Contest = TradeContest_Cute, Version = GameVersion.FR,}, // Nidorina *
            new EncounterTrade { Species = 033, Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {19,18,25,22,15,22}, Nature = Nature.Bold, Contest = TradeContest_Tough, Version = GameVersion.LG,}, // Nidorino  *
            new EncounterTrade { Species = 108, Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Nature = Nature.Relaxed, Contest = TradeContest_Tough, }, // Lickitung  * 
            new EncounterTrade { Species = 124, Ability = 1, TID = 36728, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {18,17,18,22,25,21}, Nature = Nature.Mild, Contest = TradeContest_Beauty, }, // Jynx
            new EncounterTrade { Species = 083, Ability = 1, TID = 08810, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,25,21,24,15,20}, Nature = Nature.Adamant, Contest = TradeContest_Cool, }, // Farfetch'd
            new EncounterTrade { Species = 101, Ability = 1, TID = 50298, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {19,16,18,25,25,19}, Nature = Nature.Hasty, Contest = TradeContest_Cool, }, // Electrode
            new EncounterTrade { Species = 114, Ability = 1, TID = 60042, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {22,17,25,16,23,20}, Nature = Nature.Sassy, Contest = TradeContest_Cute, }, // Tangela
            new EncounterTrade { Species = 086, Ability = 1, TID = 09853, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,15,22,16,23,22}, Nature = Nature.Bold, Contest = TradeContest_Tough, }, // Seel *
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };

        #region AltSlots
        internal static readonly int[] SafariZoneLocation_3 =
        {
            57, 136
        };
        private static readonly EncounterArea[] SlotsRSEAlt =
        {
            // Swarm can be passed from one game to another via mixing records, that means emerald swarms can occurs in r/s and r/s swarms in emerald
            // Ruby and Sapphire Swarm
            new EncounterArea {
                Location = 17, // Route 102
                Slots = new[]
                {
                    new EncounterSlot { Species = 283, LevelMin = 3, LevelMax = 3, Type = SlotType.Grass}, // Surskit
                },},
            new EncounterArea {
                Location = 29, // Route 114
                Slots = new[]
                {
                    new EncounterSlot { Species = 283, LevelMin = 15, LevelMax = 15, Type = SlotType.Grass}, // Surskit
                },},
            new EncounterArea {
                Location = 31, // Route 116
                Slots = new[]
                {
                    new EncounterSlot { Species = 300, LevelMin = 15, LevelMax = 15, Type = SlotType.Grass}, // Skitty
                },},
            new EncounterArea {
                Location = 32, // Route 117
                Slots = new[]
                {
                    new EncounterSlot { Species = 283, LevelMin = 15, LevelMax = 15, Type = SlotType.Grass}, // Surskit
                },},
            new EncounterArea {
                Location = 35, // Route 120
                Slots = new[]
                {
                    new EncounterSlot { Species = 283, LevelMin = 28, LevelMax = 28, Type = SlotType.Grass}, // Surskit
                },},

            //Emerald Swarm
            new EncounterArea {
                Location = 17, // Route 102
                Slots = new[]
                {
                    new EncounterSlot { Species = 273, LevelMin = 3, LevelMax = 3, Type = SlotType.Grass}, // Seedot
                },},
            new EncounterArea {
                Location = 29, // Route 114
                Slots = new[]
                {
                    new EncounterSlot { Species = 274, LevelMin = 15, LevelMax = 15, Type = SlotType.Grass}, // Nuzleaf
                },},
            new EncounterArea {
                Location = 31, // Route 116
                Slots = new[]
                {
                    new EncounterSlot { Species = 300, LevelMin = 8, LevelMax = 8, Type = SlotType.Grass}, // Skitty
                },},
            new EncounterArea {
                Location = 32, // Route 117
                Slots = new[]
                {
                    new EncounterSlot { Species = 273, LevelMin = 13, LevelMax = 13, Type = SlotType.Grass}, // Seedot
                },},
            new EncounterArea {
                Location = 35, // Route 120
                Slots = new[]
                {
                    new EncounterSlot { Species = 273, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass}, // Seedot
                },},
            //Feebas fishing spot
            new EncounterArea {
                Location = 34, // Route 119
                Slots = new[]
                {
                    new EncounterSlot { Species = 349, LevelMin = 20, LevelMax = 25, Type = SlotType.Super_Rod } // Feebas
                },},
        };
        private static readonly EncounterArea[] SlotsFRLGAlt =
        {
            new EncounterArea {
                Location = 188, // Monean Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 0 }, // Unown A
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 26 }, // Unown ?
                },},
            new EncounterArea {
                Location = 189, // Liptoo Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 2 }, // Unown C
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 3 }, // Unown D
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 7 }, // Unown H
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 14 }, // Unown O
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 20 }, // Unown U
                },},
            new EncounterArea {
                Location = 190, // Weepth Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 4 }, // Unown E
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 8 }, // Unown I
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 13 }, // Unown N
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 18 }, // Unown S
                },},
            new EncounterArea {
                Location = 191, // Dilford Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 9 }, // Unown J
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 11 }, // Unown L
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 15 }, // Unown P
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 16 }, // Unown Q
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 17 }, // Unown R
                },},
            new EncounterArea {
                Location = 192, // Scufib Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 5 }, // Unown F
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 6 }, // Unown G
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 10 }, // Unown K
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 19 }, // Unown T
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 24 }, // Unown Y
                },},
            new EncounterArea {
                Location = 193, // Rixy Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 1 }, // Unown B
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 12 }, // Unown M
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 21 }, // Unown V
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 22 }, // Unown W
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 23 }, // Unown X
                },},
            new EncounterArea {
                Location = 193, // Viapois Chamber
                Slots = new[]
                {
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 25 }, // Unown Z
                    new EncounterSlot { Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass, Form = 27 }, // Unown !
                },}
        };
        #endregion
        internal static readonly int[] ValidEggMet_RSE =
        {
            32, //Route 117 
            253, //Ingame egg gift
            255 // event/pokemon box
        };
        internal static readonly int[] ValidEggMet_FRLG =
        {
            146, //Four Island
            253, //Ingame egg gift
            255 // event/pokemon box
        };
        // 064 is an unused location for metor falls
        // 084 is Inside of a truck, no possible pokemon can be hatched there
        internal static readonly int[] ValidMet_RS =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
            020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
            040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
            060, 061, 062, 063, 065, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078, 079, 080,
            081, 082, 083, 085, 086, 087,
        };
        internal static readonly int[] ValidMet_FRLG =
        {
            087, 088, 089, 090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
            100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
            120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
            140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
            160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179,
            180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196
        };
        internal static readonly int[] ValidMet_E = ValidMet_RS.Concat(new[]
        {
            196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
        }).ToArray();
    }
}
