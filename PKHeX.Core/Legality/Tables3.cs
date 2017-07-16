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
            407,424,429,430,461,462,463,464,465,466,467,468,469,470,471,472,473,474,475,476,477,478,700
        };

        internal static readonly int[] FutureEvolutionsGen3_LevelUpGen4 = 
        {
            424, 461, 462, 463, 465, 469, 470, 471, 472, 473, 476
        };
        // Ambipom Weavile Magnezone Lickilicky Tangrowth
        // Yanmega Leafeon Glaceon Mamoswine Gliscor Probopass
        internal static readonly int[] UnreleasedItems_3 =
        {
            005, // Safari Ball
        };
        internal static readonly bool[] ReleasedHeldItems_3 = Enumerable.Range(0, MaxItemID_3+1).Select(i => HeldItems_RS.Contains((ushort)i) && !UnreleasedItems_3.Contains(i)).ToArray();
        internal static readonly string[] EReaderBerriesNames_USA =
        {
            // USA Series 1
            "PUMKIN",
            "DRASH",
            "EGGANT",
            "STRIB",
            "CHILAN",
            "NUTPEA",
        };
        internal static readonly string[] EReaderBerriesNames_JP =
        {
            // JP Series 1
            "カチャ", // PUMKIN
            "ブ－カ", // DRASH
            "ビスナ", // EGGANT
            "エドマ", // STRIB
            "ホズ", // CHILAN
            "ラッカ", // NUTPEA
            "クオ", // KU
            // JP Series 2
            "ギネマ", // GINEMA
            "クオ", // KUO
            "ヤゴ", // YAGO
            "トウガ", // TOUGA
            "ニニク", // NINIKU
            "トポ" // TOPO
        };
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

        internal static readonly MysteryGift[] Encounter_Event3 =
        {
            new WC3 { Species = 251, Level = 10, TID = 31121, OT_Gender = 1, OT_Name = "アゲト", CardTitle = "Agate Celebi", Method = PIDType.CXD, Shiny = false, Language = 1 },
            new WC3 { Species = 025, Level = 10, TID = 31121, OT_Gender = 0, OT_Name = "コロシアム", CardTitle = "Colosseum Pikachu", Method = PIDType.CXD, Shiny = false, Language = 1 },

            new WC3 { Species = 385, Level = 05, TID = 20043, OT_Gender = 0, OT_Name = "WISHMKR", CardTitle = "Wishmaker Jirachi", Method = PIDType.BACD_R, Language = 2 },
            new WC3 { Species = 251, Level = 10, TID = 31121, OT_Gender = 1, OT_Name = "AGATE", CardTitle = "Agate Celebi", Method = PIDType.CXD, Shiny = false, Language = 2, NotDistributed = true  },
            new WC3 { Species = 025, Level = 10, TID = 31121, OT_Gender = 0, OT_Name = "COLOS", CardTitle = "Colosseum Pikachu", Method = PIDType.CXD, Shiny = false, Language = 2, NotDistributed = true },

            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "バトルやま", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 1 }, // JPN
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "MATTLE", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 2 }, // ENG
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "MT BATA", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 3 }, // FRE
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "DUELLBE", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 5 }, // GER
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "MONTE L", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 4 }, // ITA
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "ERNESTO", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 7 }, // SPA

            // CXD
            new WC3 { Species = 239, Level = 20, Language = 2, Fateful = true,  Met_Location = 164, TID = 41400, SID = -1, OT_Gender = 0, OT_Name = "HORDEL", CardTitle = "Trade Togepi", Method = PIDType.CXD, Moves = new[] {8,7,9,238} }, // Elekid @ Snagem Hideout
            new WC3 { Species = 307, Level = 20, Language = 2, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Trade Trapinch", Method = PIDType.CXD, Moves = new[] {223,93,247,197} }, // Meditite @ Pyrite Town
            new WC3 { Species = 213, Level = 20, Language = 2, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Trade Surskit", Method = PIDType.CXD, Moves = new[] {92,164,188,277} }, // Shuckle @ Pyrite Town
            new WC3 { Species = 246, Level = 20, Language = 2, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Trade Wooper", Method = PIDType.CXD, Moves = new[] {201,349,44,200} }, // Larvitar @ Pyrite Town
            new WC3 { Species = 311, Level = 13, Language = 2, Fateful = false, Met_Location = 254, TID = 37149, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Gift", Method = PIDType.CXD }, // Plusle @ Ingame Trade

            new WC3 { Species = 239, Level = 20, Language = 1, Fateful = true,  Met_Location = 164, TID = 41400, SID = -1, OT_Gender = 0, OT_Name = "ダニー",   CardTitle = "Trade Togepi", Method = PIDType.CXD, Moves = new[] {8,7,9,238} }, // Elekid @ Snagem Hideout
            new WC3 { Species = 307, Level = 20, Language = 1, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Trade Trapinch", Method = PIDType.CXD, Moves = new[] {223,93,247,197} }, // Meditite @ Pyrite Town
            new WC3 { Species = 213, Level = 20, Language = 1, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Trade Surskit", Method = PIDType.CXD, Moves = new[] {92,164,188,277} }, // Shuckle @ Pyrite Town
            new WC3 { Species = 246, Level = 20, Language = 1, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Trade Wooper", Method = PIDType.CXD, Moves = new[] {201,349,44,200} }, // Larvitar @ Pyrite Town
            new WC3 { Species = 311, Level = 13, Language = 1, Fateful = false, Met_Location = 254, TID = 37149, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Gift", Method = PIDType.CXD }, // Plusle @ Ingame Trade
        };

        internal static readonly MysteryGift[] Encounter_Event3_FRLG =
        {
            // PCJP - Egg Pokémon Present Eggs (March 21 to April 4, 2004)
            new WC3 { Species = 043, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{073} }, // Oddish with Leech Seed
            new WC3 { Species = 052, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{080} }, // Meowth with Petal Dance
            new WC3 { Species = 060, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{186} }, // Poliwag with Sweet Kiss
            new WC3 { Species = 069, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{298} }, // Bellsprout with Teeter Dance

            // PCNY - Wish Eggs (December 16, 2004, to January 2, 2005)
            new WC3 { Species = 083, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 281} }, // Farfetch'd with Wish & Yawn
            new WC3 { Species = 096, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 187} }, // Drowzee with Wish & Belly Drum
            new WC3 { Species = 102, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 230} }, // Exeggcute with Wish & Sweet Scent
            new WC3 { Species = 108, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 215} }, // Lickitung with Wish & Heal Bell
            new WC3 { Species = 113, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 230} }, // Chansey with Wish & Sweet Scent
            new WC3 { Species = 115, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 281} }, // Kangaskhan with Wish & Yawn

            // PokePark Eggs - Wondercard
            new WC3 { Species = 054, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Psyduck with Mud Sport
            new WC3 { Species = 172, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{266}, Method = PIDType.Method_2 }, // Pichu with Follow me
            new WC3 { Species = 174, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{321}, Method = PIDType.Method_2 }, // Igglybuff with Tickle
            new WC3 { Species = 222, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Corsola with Mud Sport
            new WC3 { Species = 276, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{297}, Method = PIDType.Method_2 }, // Taillow with Feather Dance
            new WC3 { Species = 283, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Surskit with Mud Sport
            new WC3 { Species = 293, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{298}, Method = PIDType.Method_2 }, // Whismur with Teeter Dance
            new WC3 { Species = 300, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{205}, Method = PIDType.Method_2 }, // Skitty with Rollout
            new WC3 { Species = 311, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{346}, Method = PIDType.Method_2 }, // Plusle with Water Sport
            new WC3 { Species = 312, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Minun with Mud Sport
            new WC3 { Species = 325, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{253}, Method = PIDType.Method_2 }, // Spoink with Uproar
            new WC3 { Species = 327, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{047}, Method = PIDType.Method_2 }, // Spinda with Sing
            new WC3 { Species = 331, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{227}, Method = PIDType.Method_2 }, // Cacnea with Encore
            new WC3 { Species = 341, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{346}, Method = PIDType.Method_2 }, // Corphish with Water Sport
            new WC3 { Species = 360, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{321}, Method = PIDType.Method_2 }, // Wynaut with Tickle
        };

        internal static readonly MysteryGift[] Encounter_Event3_RS =
        {   
            // PCJP - Pokémon Center 5th Anniversary Eggs (April 25 to May 18, 2003)
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{298} }, // Pichu with Teeter Dance
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Pichu with Wish
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R_S, Moves = new[]{298} }, // Pichu with Teeter Dance
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R_S, Moves = new[]{273} }, // Pichu with Wish
            new WC3 { Species = 280, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{204 } }, // Ralts with Charm
            new WC3 { Species = 280, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Ralts with Wish
            new WC3 { Species = 359, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{180} }, // Absol with Spite
            new WC3 { Species = 359, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Absol with Wish
            new WC3 { Species = 371, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{334} }, // Bagon with Iron Defense
            new WC3 { Species = 371, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Bagon with Wish
            
            // Negai Boshi Jirachi
            new WC3 { Species = 385, Level = 05, TID = 30719, OT_Gender = 0, OT_Name = "ネガイボシ", Method = PIDType.BACD_R, Language = 1, Shiny = false },

            // Berry Glitch Fix
            // PCJP - (December 29, 2003 to March 31, 2004)
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 1, Method = PIDType.BACD_R_S, TID = 21121, OT_Name = "ルビー", OT_Gender = 1, Shiny = true },
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 1, Method = PIDType.BACD_R_S, TID = 21121, OT_Name = "サファイア", OT_Gender = 0, Shiny = true },
            
            // EBGames/GameStop (March 1, 2004 to April 22, 2007), also via multi-game discs
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 2, Method = PIDType.BACD_R_S, TID = 30317, OT_Name = "RUBY", OT_Gender = 1 },
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 2, Method = PIDType.BACD_R_S, TID = 30317, OT_Name = "SAPHIRE", OT_Gender = 0 },

            // Channel Jirachi
            new WC3 { Species = 385, Level = 5, Version = (int)GameVersion.RS, Method = PIDType.Channel, TID = 40122, OT_Gender = 3,SID = -1, OT_Name = "CHANNEL", CardTitle = "Channel Jirachi", Met_Level = 0 },

            // Aura Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 2, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 3, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 4, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 5, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 7, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew

            // English Events
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latios

            // French
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latios

            // Italian
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Latios
            
            // German
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Latios

            // Spanish
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Latios

            new WC3 { Species = 375, Level = 30, Version = (int)GameVersion.R, Moves = new[] {036,093,232,287}, Language = 2, Method = PIDType.BACD_R, TID = 02005, OT_Name = "ROCKS", OT_Gender = 0, RibbonNational = true, Shiny = false }, // Metang
            new WC3 { Species = 386, Level = 70, Version = (int)GameVersion.R, Moves = new[] {322,105,354,063}, Language = 2, Method = PIDType.BACD_R, TID = 28606, OT_Name = "DOEL", Fateful = true, Shiny = false }, // Deoxys
            new WC3 { Species = 386, Level = 70, Version = (int)GameVersion.R, Moves = new[] {322,105,354,063}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "SPACE C", Fateful = true, Shiny = false }, // Deoxys
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 2, Method = PIDType.BACD_U, TID = 06930, OT_Name = "MYSTRY", Fateful = true, Shiny = false }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 2, Method = PIDType.BACD_R, TID = 06930, OT_Name = "MYSTRY", Fateful = true, Shiny = false }, // Mew

            // Party of the Decade
            new WC3 { Species = 001, Level = 70, Version = (int)GameVersion.R, Moves = new[] {230,074,076,235}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Bulbasaur
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Charizard
            new WC3 { Species = 009, Level = 70, Version = (int)GameVersion.R, Moves = new[] {182,240,130,056}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Blastoise
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,087,113,019}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 202, Shiny = false }, // Pikachu (Fly)
            new WC3 { Species = 065, Level = 70, Version = (int)GameVersion.R, Moves = new[] {248,347,094,271}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Alakazam
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Articuno
            new WC3 { Species = 145, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,197,065,268}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Zapdos
            new WC3 { Species = 146, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,203,053,219}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Moltres
            new WC3 { Species = 149, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,219,017,200}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Dragonite
            new WC3 { Species = 157, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,172,129,053}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Typhlosion
            new WC3 { Species = 196, Level = 70, Version = (int)GameVersion.R, Moves = new[] {060,244,094,234}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Espeon
            new WC3 { Species = 197, Level = 70, Version = (int)GameVersion.R, Moves = new[] {185,212,103,236}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Umbreon
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Suicune
            new WC3 { Species = 248, Level = 70, Version = (int)GameVersion.R, Moves = new[] {037,184,242,089}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Tyranitar
            new WC3 { Species = 257, Level = 70, Version = (int)GameVersion.R, Moves = new[] {299,163,119,327}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Blaziken
            new WC3 { Species = 359, Level = 70, Version = (int)GameVersion.R, Moves = new[] {104,163,248,195}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Absol
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latios
            
            // Journey Across America
            new WC3 { Species = 001, Level = 70, Version = (int)GameVersion.R, Moves = new[] {230,074,076,235}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Bulbasaur
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Charizard
            new WC3 { Species = 009, Level = 70, Version = (int)GameVersion.R, Moves = new[] {182,240,130,056}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Blastoise
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 202, Shiny = false }, // Pikachu (No Fly)
            new WC3 { Species = 065, Level = 70, Version = (int)GameVersion.R, Moves = new[] {248,347,094,271}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Alakazam
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Articuno
            new WC3 { Species = 145, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,197,065,268}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Zapdos
            new WC3 { Species = 146, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,203,053,219}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Moltres
            new WC3 { Species = 149, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,219,017,200}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Dragonite
            new WC3 { Species = 157, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,172,129,053}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Typhlosion
            new WC3 { Species = 196, Level = 70, Version = (int)GameVersion.R, Moves = new[] {060,244,094,234}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Espeon
            new WC3 { Species = 197, Level = 70, Version = (int)GameVersion.R, Moves = new[] {185,212,103,236}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Umbreon
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Suicune
            new WC3 { Species = 248, Level = 70, Version = (int)GameVersion.R, Moves = new[] {037,184,242,089}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Tyranitar
            new WC3 { Species = 251, Level = 70, Version = (int)GameVersion.R, Moves = new[] {246,248,226,195}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Celebi
            new WC3 { Species = 257, Level = 70, Version = (int)GameVersion.R, Moves = new[] {299,163,119,327}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Blaziken
            new WC3 { Species = 359, Level = 70, Version = (int)GameVersion.R, Moves = new[] {104,163,248,195}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Absol
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latios
        };

        internal static readonly MysteryGift[] Encounter_Event3_Common =
        {
            // Pokémon Box
            new WC3 { Species = 333, IsEgg = true, Level = 05, Moves = new[]{206}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Swablu Egg with False Swipe
            new WC3 { Species = 263, IsEgg = true, Level = 05, Moves = new[]{245}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Zigzagoon Egg with Extreme Speed
            new WC3 { Species = 300, IsEgg = true, Level = 05, Moves = new[]{006}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Skitty Egg with Pay Day
            new WC3 { Species = 172, IsEgg = true, Level = 05, Moves = new[]{057}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Pichu Egg with Surf
 
            // PokePark Eggs - DS Download Play
            new WC3 { Species = 054, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Psyduck with Mud Sport
            new WC3 { Species = 172, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{266}, Method = PIDType.BACD_R }, // Pichu with Follow me
            new WC3 { Species = 174, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{321}, Method = PIDType.BACD_R }, // Igglybuff with Tickle
            new WC3 { Species = 222, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Corsola with Mud Sport
            new WC3 { Species = 276, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{297}, Method = PIDType.BACD_R }, // Taillow with Feather Dance
            new WC3 { Species = 283, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Surskit with Mud Sport
            new WC3 { Species = 293, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{298}, Method = PIDType.BACD_R }, // Whismur with Teeter Dance
            new WC3 { Species = 300, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{205}, Method = PIDType.BACD_R }, // Skitty with Rollout
            new WC3 { Species = 311, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{346}, Method = PIDType.BACD_R }, // Plusle with Water Sport
            new WC3 { Species = 312, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Minun with Mud Sport
            new WC3 { Species = 325, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{253}, Method = PIDType.BACD_R }, // Spoink with Uproar
            new WC3 { Species = 327, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{047}, Method = PIDType.BACD_R }, // Spinda with Sing
            new WC3 { Species = 331, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{227}, Method = PIDType.BACD_R }, // Cacnea with Encore
            new WC3 { Species = 341, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{346}, Method = PIDType.BACD_R }, // Corphish with Water Sport
            new WC3 { Species = 360, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{321}, Method = PIDType.BACD_R }, // Wynaut with Tickle
        };

        internal static readonly MysteryGift[] Encounter_WC3 = Encounter_Event3.Concat(Encounter_Event3_RS).Concat(Encounter_Event3_FRLG.Concat(Encounter_Event3_Common)).ToArray();

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
            new EncounterStatic { Species = 380, Level = 50, Location = 073, Version = GameVersion.R }, // Latias @ Southern Island
            new EncounterStatic { Species = 380, Level = 50, Location = 073, Version = GameVersion.E, Fateful = true }, // Latias @ Southern Island
            new EncounterStatic { Species = 381, Level = 50, Location = 073, Version = GameVersion.S }, // Latios @ Southern Island
            new EncounterStatic { Species = 381, Level = 50, Location = 073, Version = GameVersion.E, Fateful = true }, // Latios @ Southern Island
            new EncounterStatic { Species = 382, Level = 45, Location = 072, Version = GameVersion.S, }, // Kyogre @ Cave of Origin
            new EncounterStatic { Species = 382, Level = 70, Location = 203, Version = GameVersion.E, }, // Kyogre @ Marine Cave
            new EncounterStatic { Species = 383, Level = 45, Location = 072, Version = GameVersion.R, }, // Groudon @ Cave of Origin
            new EncounterStatic { Species = 383, Level = 70, Location = 205, Version = GameVersion.E, }, // Groudon @ Terra Cave
            new EncounterStatic { Species = 384, Level = 70, Location = 085, }, // Rayquaza @ Sky Pillar

            // Event
            new EncounterStatic { Species = 151, Level = 30, Location = 201, Version = GameVersion.E, Fateful = true }, // Mew @ Faraway Island (Unreleased outside of Japan)
            new EncounterStatic { Species = 249, Level = 70, Location = 211, Version = GameVersion.E, Fateful = true }, // Lugia @ Navel Rock
            new EncounterStatic { Species = 250, Level = 70, Location = 211, Version = GameVersion.E, Fateful = true }, // Ho-Oh @ Navel Rock
            new EncounterStatic { Species = 386, Level = 30, Location = 200, Version = GameVersion.E, Fateful = true, Form = 3 }, // Deoxys @ Birth Island
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
            new EncounterStatic { Gift = true, Species = 7, Level = 05, Location = 088, }, // Squirtle

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
            
            // Celadon City Game Corner
            new EncounterStatic { Gift = true, Species = 063, Level = 09, Location = 94, Version = GameVersion.FR }, // Abra
            new EncounterStatic { Gift = true, Species = 035, Level = 08, Location = 94, Version = GameVersion.FR }, // Clefairy
            new EncounterStatic { Gift = true, Species = 123, Level = 25, Location = 94, Version = GameVersion.FR }, // Scyther
            new EncounterStatic { Gift = true, Species = 147, Level = 18, Location = 94, Version = GameVersion.FR }, // Dratini
            new EncounterStatic { Gift = true, Species = 137, Level = 26, Location = 94, Version = GameVersion.FR }, // Porygon

            new EncounterStatic { Gift = true, Species = 063, Level = 07, Location = 94, Version = GameVersion.LG }, // Abra
            new EncounterStatic { Gift = true, Species = 035, Level = 12, Location = 94, Version = GameVersion.LG }, // Clefairy
            new EncounterStatic { Gift = true, Species = 127, Level = 18, Location = 94, Version = GameVersion.LG }, // Pinsir
            new EncounterStatic { Gift = true, Species = 147, Level = 24, Location = 94, Version = GameVersion.LG }, // Dratini
            new EncounterStatic { Gift = true, Species = 137, Level = 18, Location = 94, Version = GameVersion.LG }, // Porygon

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
            new EncounterStatic { Species = 249, Level = 70, Location = 174, Fateful = true }, // Lugia @ Navel Rock
            new EncounterStatic { Species = 250, Level = 70, Location = 174, Fateful = true }, // Ho-Oh @ Navel Rock
            new EncounterStatic { Species = 386, Level = 30, Location = 187, Version = GameVersion.FR, Form = 1, Fateful = true }, // Deoxys @ Birth Island
            new EncounterStatic { Species = 386, Level = 30, Location = 187, Version = GameVersion.LG, Form = 2, Fateful = true }, // Deoxys @ Birth Island
        };

        internal static readonly EncounterStatic[] Encounter_RSE = Encounter_RSE_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_RSE)).Concat(Encounter_RSE_Regular).ToArray();
        internal static readonly EncounterStatic[] Encounter_FRLG = Encounter_FRLG_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_FRLG)).Concat(Encounter_FRLG_Stationary).ToArray();

        private static readonly int[] TradeContest_Cool =   {30, 05, 05, 05, 05, 10};
        private static readonly int[] TradeContest_Beauty = {05, 30, 05, 05, 05, 10};
        private static readonly int[] TradeContest_Cute =   {05, 05, 30, 05, 05, 10};
        private static readonly int[] TradeContest_Clever = {05, 05, 05, 30, 05, 10};
        private static readonly int[] TradeContest_Tough =  {05, 05, 05, 05, 30, 10};
        internal static readonly EncounterTrade[] TradeGift_RSE =
        {
            new EncounterTrade { Species = 296, Ability = 2, TID = 49562, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,5,4,4,4,4}, Level = 05, Nature = Nature.Hardy, Contest = TradeContest_Tough, Version = GameVersion.RS, }, // Slakoth (Level 5 Breeding) -> Makuhita
            new EncounterTrade { Species = 300, Ability = 1, TID = 02259, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {5,4,4,5,4,4}, Level = 03, Nature = Nature.Timid, Contest = TradeContest_Cute, Version = GameVersion.RS, }, // Pikachu (Level 3 Viridiam Forest) -> Skitty
            new EncounterTrade { Species = 222, Ability = 2, TID = 50183, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {4,4,5,4,4,5}, Level = 21, Nature = Nature.Calm, Contest = TradeContest_Beauty, Version = GameVersion.RS, }, // Bellossom (Level 21 Odish -> Gloom -> Bellossom) -> Corsola
            new EncounterTrade { Species = 273, Ability = 2, TID = 38726, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,5,4,4,4}, Level = 04, Nature = Nature.Relaxed, Contest = TradeContest_Cool, Version = GameVersion.E, }, // Ralts (Level 4 Route 102) -> Seedot
            new EncounterTrade { Species = 311, Ability = 1, TID = 08460, SID = 00001, OTGender = 0, Gender = 1, IVs = new[] {4,4,4,5,5,4}, Level = 05, Nature = Nature.Hasty, Contest = TradeContest_Cute, Version = GameVersion.E, }, // Volbeat (Level 5 Breeding) -> Plusle
            new EncounterTrade { Species = 116, Ability = 1, TID = 46285, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,4,4,5,4}, Level = 05, Nature = Nature.Brave, Contest = TradeContest_Tough, Version = GameVersion.E, }, // Bagon 	Bagon (Level 5 Breeding) -> Horsea*
            new EncounterTrade { Species = 052, Ability = 1, TID = 25945, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {4,5,4,5,4,4}, Level = 03, Nature = Nature.Naive, Contest = TradeContest_Clever, Version = GameVersion.E, }, // Skitty (Level 3 Trade)-> Meowth*
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
            new EncounterTrade { Species = 083, Ability = 1, TID = 08810, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,25,21,24,15,20}, Nature = Nature.Adamant, Contest = TradeContest_Cool, }, // Farfetch'd
            new EncounterTrade { Species = 101, Ability = 2, TID = 50298, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {19,16,18,25,25,19}, Nature = Nature.Hasty, Contest = TradeContest_Cool, }, // Electrode
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
                Location = 194, // Viapois Chamber
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
        // 155 - 158 Sevii Isle 6-9 Unused
        // 171 - 173 Sevii Isle 22-24 Unused
        internal static readonly int[] ValidMet_FRLG =
        {
            087, 088, 089, 090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
            100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
            120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
            140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 159, 160, 161, 162, 163,
            164, 165, 166, 167, 168, 169, 170, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186,
            187, 188, 189, 190, 191, 192, 193, 194, 195, 196
        };
        internal static readonly int[] ValidMet_E = ValidMet_RS.Concat(new[]
        {
            196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
        }).ToArray();

        #region Colosseum
        internal static readonly EncounterStatic[] Encounter_Colo =
        {
            new EncounterStatic { Gift = true, Species = 196, Level = 25, Location = 254 }, // Espeon
            new EncounterStatic { Gift = true, Species = 197, Level = 26, Location = 254, Moves = new[] {044} }, // Umbreon (Bite)

            new EncounterStaticShadow { Species = 296, Level = 30, Gauge = 03000, Moves = new[] {193,116,233,238}, Location = 005 }, // Makuhita: Miror B.Peon Trudly @ Phenac City

            // missing locs: Realgam Tower
            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 003 }, // Bayleef: Cipher Peon Verde @ Phenac City
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 003 }, // Quilava: Cipher Peon Rosso @ Phenac City
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 003 }, // Croconaw: Cipher Peon Bluno @ Phenac City
            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 132 }, // Bayleef: Cipher Peon Verde @ Snagem Hideout
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 132 }, // Quilava: Cipher Peon Rosso @ Snagem Hideout
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 132 }, // Croconaw: Cipher Peon Bluno @ Snagem Hideout
            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 069 }, // Bayleef: Cipher Peon Verde @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 069 }, // Quilava: Cipher Peon Rosso @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 069 }, // Croconaw: Cipher Peon Bluno @ Shadow PKMN Lab

            new EncounterStaticShadow { Species = 218, Level = 30, Gauge = 04000, Moves = new[] {241,281,088,053}, Location = 015 }, // Slugma: Roller Boy Lon @ Pyrite Town
            new EncounterStaticShadow { Species = 164, Level = 30, Gauge = 03000, Moves = new[] {211,095,115,019}, Location = 015 }, // Noctowl: Rider Nover @ Pyrite Town
            new EncounterStaticShadow { Species = 180, Level = 30, Gauge = 03000, Moves = new[] {085,086,178,084}, Location = 015 }, // Flaaffy: St.Performer Diogo @ Pyrite Town
            new EncounterStaticShadow { Species = 188, Level = 30, Gauge = 03000, Moves = new[] {235,079,178,072}, Location = 015 }, // Skiploom: Rider Leba @ Pyrite Town
            new EncounterStaticShadow { Species = 195, Level = 30, Gauge = 04000, Moves = new[] {341,133,021,057}, Location = 015 }, // Quagsire: Bandana Guy Divel @ Pyrite Town
            new EncounterStaticShadow { Species = 200, Level = 30, Gauge = 04000, Moves = new[] {060,109,212,247}, Location = 015 }, // Misdreavus: Rider Vant @ Pyrite Town
            new EncounterStaticShadow { Species = 162, Level = 33, Gauge = 05000, Moves = new[] {231,270,098,070}, Location = 015 }, // Furret: Rogue Cail @ Pyrite Town

            // missing loc: Snagem Hideout
            new EncounterStaticShadow { Species = 193, Level = 33, Gauge = 05000, Moves = new[] {197,048,049,253}, Location = 025 }, // Yanma: Cipher Peon Nore @ Pyrite Bldg
            
            new EncounterStaticShadow { Species = 223, Level = 20, Gauge = 04000, Moves = new[] {061,199,060,062}, Location = 028 }, // Remoraid: Miror B.Peon Reath @ Pyrite Bldg
            new EncounterStaticShadow { Species = 223, Level = 20, Gauge = 04000, Moves = new[] {061,199,060,062}, Location = 030 }, // Remoraid: Miror B.Peon Reath @ Pyrite Cave
            new EncounterStaticShadow { Species = 226, Level = 33, Gauge = 05000, Moves = new[] {017,048,061,036}, Location = 028 }, // Mantine: Miror B.Peon Ferma @ Pyrite Bldg
            new EncounterStaticShadow { Species = 226, Level = 33, Gauge = 05000, Moves = new[] {017,048,061,036}, Location = 030 }, // Mantine: Miror B.Peon Ferma @ Pyrite Cave

            new EncounterStaticShadow { Species = 211, Level = 33, Gauge = 05000, Moves = new[] {042,107,040,057}, Location = 015 }, // Qwilfish: Hunter Doken @ Pyrite Bldg
            new EncounterStaticShadow { Species = 307, Level = 33, Gauge = 05000, Moves = new[] {197,347,093,136}, Location = 031 }, // Meditite: Rider Twan @ Pyrite Cave
            new EncounterStaticShadow { Species = 206, Level = 33, Gauge = 05000, Moves = new[] {180,137,281,036}, Location = 029 }, // Dunsparce: Rider Sosh @ Pyrite Cave
            new EncounterStaticShadow { Species = 333, Level = 33, Gauge = 05000, Moves = new[] {119,047,219,019}, Location = 032 }, // Swablu: Hunter Zalo @ Pyrite Cave

            new EncounterStaticShadow { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 125 }, // Sudowoodo: Cipher Admin Miror B. @ Deep Colosseum -- (Realgam Tower missing)
            new EncounterStaticShadow { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 030 }, // Sudowoodo: Cipher Admin Miror B. @ Pyrite Cave

            // missing locs: Shadow PKMN Lab
            new EncounterStaticShadow { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 039 }, // Hitmontop: Cipher Peon Skrub @ Agate Village
            new EncounterStaticShadow { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 132 }, // Hitmontop: Cipher Peon Skrub @ Snagem Hideout

            new EncounterStaticShadow { Species = 166, Level = 40, Gauge = 06000, Moves = new[] {226,219,048,004}, Location = 047 }, // Ledian: Cipher Peon Kloak @ The Under
            new EncounterStaticShadow { Species = 166, Level = 40, Gauge = 06000, Moves = new[] {226,219,048,004}, Location = 132 }, // Ledian: Cipher Peon Kloak @ Snagem Hideout

            // missing locs: Realgam Tower/Deep Colosseum
            new EncounterStaticShadow { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 076 }, // Entei: Cipher Admin Dakim @ Mt. Battle

            // missing loc: Realgam Tower
            new EncounterStaticShadow { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,057}, Location = 055 }, // Suicune (Surf): Cipher Admin Venus @ The Under
            new EncounterStaticShadow { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,056}, Location = 000 }, // Suicune (Hydro Pump): Cipher Admin Venus @ Deep Colosseum

            // missing locs: Realgam Tower/Deep Colosseum
            new EncounterStaticShadow { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 069 }, // Raikou: Cipher Admin Ein @ Shadow PKMN Lab

            new EncounterStaticShadow { Species = 207, Level = 43, Gauge = 06000, Moves = new[] {185,028,040,163}, Location = 058 }, // Gligar: Hunter Frena @ The Under Subway
            new EncounterStaticShadow { Species = 207, Level = 43, Gauge = 06000, Moves = new[] {185,028,040,163}, Location = 133 }, // Gligar: Hunter Frena @ Snagem Hideout
            new EncounterStaticShadow { Species = 234, Level = 43, Gauge = 06000, Moves = new[] {310,095,043,036}, Location = 058 }, // Stantler: Chaser Liaks @ The Under Subway
            new EncounterStaticShadow { Species = 234, Level = 43, Gauge = 06000, Moves = new[] {310,095,043,036}, Location = 133 }, // Stantler: Chaser Liaks @ Snagem Hideout
            new EncounterStaticShadow { Species = 221, Level = 43, Gauge = 06000, Moves = new[] {203,316,091,059}, Location = 058 }, // Piloswine: Bodybuilder Lonia @ The Under Subway
            new EncounterStaticShadow { Species = 221, Level = 43, Gauge = 06000, Moves = new[] {203,316,091,059}, Location = 134 }, // Piloswine: Bodybuilder Lonia @ Snagem Hideout
            new EncounterStaticShadow { Species = 215, Level = 43, Gauge = 06000, Moves = new[] {185,103,154,196}, Location = 058 }, // Sneasel: Rider Nelis @ The Under Subway
            new EncounterStaticShadow { Species = 215, Level = 43, Gauge = 06000, Moves = new[] {185,103,154,196}, Location = 134 }, // Sneasel: Rider Nelis @ Snagem Hideout
            new EncounterStaticShadow { Species = 190, Level = 43, Gauge = 06000, Moves = new[] {226,321,154,129}, Location = 067 }, // Aipom: Cipher Peon Cole @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 198, Level = 43, Gauge = 06000, Moves = new[] {185,212,101,019}, Location = 067 }, // Murkrow: Cipher Peon Lare @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 205, Level = 43, Gauge = 06000, Moves = new[] {153,182,117,229}, Location = 067 }, // Forretress: Cipher Peon Vana @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 168, Level = 43, Gauge = 06000, Moves = new[] {169,184,141,188}, Location = 069 }, // Ariados: Cipher Peon Lesar @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 210, Level = 43, Gauge = 06000, Moves = new[] {044,184,046,070}, Location = 069 }, // Granbull: Cipher Peon Tanie @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 329, Level = 43, Gauge = 06000, Moves = new[] {242,103,328,225}, Location = 068 }, // Vibrava: Cipher Peon Remil @ Shadow PKMN Lab
            
            new EncounterStaticShadow { Species = 192, Level = 45, Gauge = 07000, Moves = new[] {241,074,275,076}, Location = 109 }, // Sunflora: Cipher Peon Baila @ Realgam Tower
            new EncounterStaticShadow { Species = 225, Level = 45, Gauge = 07000, Moves = new[] {059,213,217,019}, Location = 109 }, // Delibird: Cipher Peon Arton @ Realgam Tower
            new EncounterStaticShadow { Species = 214, Level = 45, Gauge = 07000, Moves = new[] {179,203,068,280}, Location = 111 }, // Heracross: Cipher Peon Dioge @ Realgam Tower
            new EncounterStaticShadow { Species = 227, Level = 47, Gauge = 13000, Moves = new[] {065,319,314,211}, Location = 117 }, // Skarmory: Snagem Head Gonzap @ Realgam Tower
            new EncounterStaticShadow { Species = 192, Level = 45, Gauge = 07000, Moves = new[] {241,074,275,076}, Location = 132 }, // Sunflora: Cipher Peon Baila @ Snagem Hideout
            new EncounterStaticShadow { Species = 225, Level = 45, Gauge = 07000, Moves = new[] {059,213,217,019}, Location = 132 }, // Delibird: Cipher Peon Arton @ Snagem Hideout
            new EncounterStaticShadow { Species = 214, Level = 45, Gauge = 07000, Moves = new[] {179,203,068,280}, Location = 132 }, // Heracross: Cipher Peon Dioge @ Snagem Hideout
            new EncounterStaticShadow { Species = 227, Level = 47, Gauge = 13000, Moves = new[] {065,319,314,211}, Location = 133 }, // Skarmory: Snagem Head Gonzap @ Snagem Hideout

            new EncounterStaticShadow { Species = 241, Level = 48, Gauge = 07000, Moves = new[] {208,111,205,034}, Location = 118 }, // Miltank: Bodybuilder Jomas @ Tower Colosseum
            new EncounterStaticShadow { Species = 359, Level = 48, Gauge = 07000, Moves = new[] {195,014,163,185}, Location = 118 }, // Absol: Rider Delan @ Tower Colosseum
            new EncounterStaticShadow { Species = 229, Level = 48, Gauge = 07000, Moves = new[] {185,336,123,053}, Location = 118 }, // Houndoom: Cipher Peon Nella @ Tower Colosseum
            new EncounterStaticShadow { Species = 357, Level = 49, Gauge = 07000, Moves = new[] {076,235,345,019}, Location = 118 }, // Tropius: Cipher Peon Ston @ Tower Colosseum
            new EncounterStaticShadow { Species = 376, Level = 50, Gauge = 15000, Moves = new[] {063,334,232,094}, Location = 118 }, // Metagross: Cipher Nascour @ Tower Colosseum
            new EncounterStaticShadow { Species = 248, Level = 55, Gauge = 20000, Moves = new[] {242,087,157,059}, Location = 118 }, // Tyranitar: Cipher Head Evice @ Tower Colosseum
            new EncounterStaticShadow { Species = 235, Level = 45, Gauge = 07000, Moves = new[] {166,039,003,231}, Location = 132 }, // Smeargle: Team Snagem Biden @ Snagem Hideout
            new EncounterStaticShadow { Species = 217, Level = 45, Gauge = 07000, Moves = new[] {185,313,122,163}, Location = 132 }, // Ursaring: Team Snagem Agrev @ Snagem Hideout
            new EncounterStaticShadow { Species = 213, Level = 45, Gauge = 07000, Moves = new[] {219,227,156,117}, Location = 125 }, // Shuckle: Deep King Agnol @ Deep Colosseum
            new EncounterStaticShadow { Species = 176, Level = 20, Gauge = 05000, Moves = new[] {118,204,186,281}, Location = 001 }, // Togetic: Cipher Peon Fein @ Outskirt Stand
            new EncounterStaticShadow { Species = 175, Level = 20, Gauge = 00000, Moves = new[] {118,204,186,281}, IVs = new[] {0,0,0,0,0,0}, EReader = true }, // Togepi: Chaser ボデス @ Card e Room (Japanese games only)
            new EncounterStaticShadow { Species = 179, Level = 37, Gauge = 00000, Moves = new[] {087,084,086,178}, IVs = new[] {0,0,0,0,0,0}, EReader = true }, // Mareep: Hunter ホル @ Card e Room (Japanese games only)
            new EncounterStaticShadow { Species = 212, Level = 50, Gauge = 00000, Moves = new[] {210,232,014,163}, IVs = new[] {0,0,0,0,0,0}, EReader = true }, // Scizor: Bodybuilder ワーバン @ Card e Room (Japanese games only)
        };
        #endregion

        #region XD

        internal static readonly EncounterStatic[] Encounter_XD =
        {
            new EncounterStatic { Fateful = true, Gift = true, Species = 133, Level = 10, Location = 000, Moves = new[] {044} }, // Eevee (Bite)
            new EncounterStatic { Fateful = true, Gift = true, Species = 152, Level = 05, Location = 016, Moves = new[] {246,033,045,338} }, // Chikorita
            new EncounterStatic { Fateful = true, Gift = true, Species = 155, Level = 05, Location = 016, Moves = new[] {179,033,043,307} }, // Cyndaquil
            new EncounterStatic { Fateful = true, Gift = true, Species = 158, Level = 05, Location = 016, Moves = new[] {242,010,043,308} }, // Totodile

            new EncounterStaticShadow { Fateful = true, Species = 216, Level = 11, Gauge = 03000, Moves = new[] {216,287,122,232}, Location = 143, }, // Teddiursa: Cipher Peon Naps @ Pokémon HQ Lab
            new EncounterStaticShadow { Fateful = true, Species = 165, Level = 10, Gauge = 02500, Moves = new[] {060,287,332,048}, Location = 153, }, // Ledyba: Casual Guy Cyle @ Gateon Port
            new EncounterStaticShadow { Fateful = true, Species = 261, Level = 10, Gauge = 02500, Moves = new[] {091,215,305,336}, Location = 162, }, // Poochyena: Bodybuilder Kilen @ Gateon Port
            new EncounterStaticShadow { Fateful = true, Species = 228, Level = 17, Gauge = 01500, Moves = new[] {185,204,052,046}, Location = 011, }, // Houndour: Cipher Peon Resix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 343, Level = 17, Gauge = 01500, Moves = new[] {317,287,189,060}, Location = 011, }, // Baltoy: Cipher Peon Browsix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 179, Level = 17, Gauge = 01500, Moves = new[] {034,215,084,086}, Location = 011, }, // Mareep: Cipher Peon Yellosix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 273, Level = 17, Gauge = 01500, Moves = new[] {202,287,331,290}, Location = 011, }, // Seedot: Cipher Peon Greesix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 363, Level = 17, Gauge = 01500, Moves = new[] {062,204,055,189}, Location = 011, }, // Spheal: Cipher Peon Blusix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 316, Level = 17, Gauge = 01500, Moves = new[] {351,047,124,092}, Location = 011, }, // Gulpin: Cipher Peon Purpsix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 167, Level = 14, Gauge = 01500, Moves = new[] {091,287,324,101}, Location = 010, }, // Spinarak: Cipher Peon Nexir @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 322, Level = 14, Gauge = 01500, Moves = new[] {036,204,091,052}, Location = 009, }, // Numel: Cipher Peon Solox @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 318, Level = 15, Gauge = 01700, Moves = new[] {352,287,184,044}, Location = 008, }, // Carvanha: Cipher Peon Cabol @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 285, Level = 15, Gauge = 01800, Moves = new[] {206,287,072,078}, Location = 008, }, // Shroomish: Cipher R&D Klots @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 301, Level = 18, Gauge = 02500, Moves = new[] {290,186,213,351}, Location = 008, }, // Delcatty: Cipher Admin Lovrina @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 100, Level = 19, Gauge = 02500, Moves = new[] {243,287,209,129}, Location = 092, }, // Voltorb: Wanderer Miror B. @ Cave Poké Spot
            new EncounterStaticShadow { Fateful = true, Species = 296, Level = 18, Gauge = 02000, Moves = new[] {280,287,292,317}, Location = 109, }, // Makuhita: Cipher Peon Torkin @ ONBS Building
            new EncounterStaticShadow { Fateful = true, Species = 037, Level = 18, Gauge = 02000, Moves = new[] {257,204,052,091}, Location = 109, }, // Vulpix: Cipher Peon Mesin @ ONBS Building
            new EncounterStaticShadow { Fateful = true, Species = 355, Level = 19, Gauge = 02200, Moves = new[] {247,270,310,109}, Location = 110, }, // Duskull: Cipher Peon Lobar @ ONBS Building
            new EncounterStaticShadow { Fateful = true, Species = 280, Level = 20, Gauge = 02200, Moves = new[] {351,047,115,093}, Location = 119, }, // Ralts: Cipher Peon Feldas @ ONBS Building
            new EncounterStaticShadow { Fateful = true, Species = 303, Level = 22, Gauge = 02500, Moves = new[] {206,047,011,334}, Location = 111, }, // Mawile: Cipher Cmdr Exol @ ONBS Building
            new EncounterStaticShadow { Fateful = true, Species = 361, Level = 20, Gauge = 02500, Moves = new[] {352,047,044,196}, Location = 097, }, // Snorunt: Cipher Peon Exinn @ Phenac City
            new EncounterStaticShadow { Fateful = true, Species = 204, Level = 20, Gauge = 02500, Moves = new[] {042,287,191,068}, Location = 096, }, // Pineco: Cipher Peon Gonrap @ Phenac City
            new EncounterStaticShadow { Fateful = true, Species = 177, Level = 22, Gauge = 02500, Moves = new[] {248,226,101,332}, Location = 094, }, // Natu: Cipher Peon Eloin @ Phenac City

            new EncounterStaticShadow { Fateful = true, Species = 315, Level = 22, Gauge = 03000, Moves = new[] {345,186,320,073}, Location = 113 }, // Roselia: Cipher Peon Fasin @ Phenac City
            new EncounterStaticShadow { Fateful = true, Species = 315, Level = 22, Gauge = 03000, Moves = new[] {345,186,320,073}, Location = 094 }, // Roselia: Cipher Peon Fasin @ Phenac City
            new EncounterStaticShadow { Fateful = true, Species = 052, Level = 22, Gauge = 03500, Moves = new[] {163,047,006,044}, Location = 113 }, // Meowth: Cipher Peon Fostin @ Phenac City
            new EncounterStaticShadow { Fateful = true, Species = 052, Level = 22, Gauge = 03500, Moves = new[] {163,047,006,044}, Location = 094 }, // Meowth: Cipher Peon Fostin @ Phenac City

            new EncounterStaticShadow { Fateful = true, Species = 220, Level = 22, Gauge = 02500, Moves = new[] {246,204,054,341}, Location = 100 }, // Swinub: Cipher Peon Greck @ Phenac City

            new EncounterStaticShadow { Fateful = true, Species = 021, Level = 22, Gauge = 04500, Moves = new[] {206,226,043,332}, Location = 059 }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
            new EncounterStaticShadow { Fateful = true, Species = 021, Level = 22, Gauge = 04500, Moves = new[] {206,226,043,332}, Location = 107 }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
            new EncounterStaticShadow { Fateful = true, Species = 088, Level = 23, Gauge = 03000, Moves = new[] {188,270,325,107}, Location = 059 }, // Grimer: Cipher Peon Faltly @ Phenac Stadium
            new EncounterStaticShadow { Fateful = true, Species = 088, Level = 23, Gauge = 03000, Moves = new[] {188,270,325,107}, Location = 107 }, // Grimer: Cipher Peon Faltly @ Phenac Stadium

            new EncounterStaticShadow { Fateful = true, Species = 086, Level = 23, Gauge = 03500, Moves = new[] {057,270,219,058}, Location = 107 }, // Seel: Cipher Peon Egrog @ Phenac Stadium
            new EncounterStaticShadow { Fateful = true, Species = 337, Level = 25, Gauge = 05000, Moves = new[] {094,226,240,317}, Location = 107 }, // Lunatone: Cipher Admin Snattle @ Phenac Stadium
            new EncounterStaticShadow { Fateful = true, Species = 175, Level = 25, Gauge = 04500, Moves = new[] {266,161,246,270}, Location = 164, Gift = true }, // Togepi: Pokémon Trainer Hordel @ Outskirt Stand

            new EncounterStaticShadow { Fateful = true, Species = 299, Level = 26, Gauge = 04000, Moves = new[] {085,270,086,157}, Location = 090 }, // Nosepass: Wanderer Miror B. @ Pyrite Colosseum/Realgam Colosseum/Poké Spots
            new EncounterStaticShadow { Fateful = true, Species = 299, Level = 26, Gauge = 04000, Moves = new[] {085,270,086,157}, Location = 113 }, // Nosepass: Wanderer Miror B. @ Pyrite Colosseum/Realgam Colosseum/Poké Spots
            
            new EncounterStaticShadow { Fateful = true, Species = 335, Level = 28, Gauge = 05000, Moves = new[] {280,287,068,306}, Location = 071 }, // Zangoose: Thug Zook @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 335, Level = 28, Gauge = 05000, Moves = new[] {280,287,068,306}, Location = 090 }, // Zangoose: Thug Zook @ Cipher Key Lair

            new EncounterStaticShadow { Fateful = true, Species = 046, Level = 28, Gauge = 04000, Moves = new[] {147,287,163,206}, Location = 064 }, // Paras: Cipher Peon Humah @ Cipher Key Lair

            new EncounterStaticShadow { Fateful = true, Species = 058, Level = 28, Gauge = 04000, Moves = new[] {053,204,044,036}, Location = 064 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 058, Level = 28, Gauge = 04000, Moves = new[] {053,204,044,036}, Location = 113 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
            
            new EncounterStaticShadow { Fateful = true, Species = 015, Level = 30, Gauge = 04500, Moves = new[] {188,226,041,014}, Location = 059 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 012, Level = 30, Gauge = 04000, Moves = new[] {094,234,079,332}, Location = 059 }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 049, Level = 32, Gauge = 04000, Moves = new[] {318,287,164,094}, Location = 059 }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 097, Level = 34, Gauge = 05500, Moves = new[] {094,226,096,247}, Location = 059 }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 354, Level = 37, Gauge = 07000, Moves = new[] {185,270,247,174}, Location = 059 }, // Banette: Cipher Peon Litnar @ Citadark Isle

            new EncounterStaticShadow { Fateful = true, Species = 090, Level = 29, Gauge = 04000, Moves = new[] {036,287,057,062}, Location = 065 }, // Shellder: Cipher Peon Gorog @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 015, Level = 30, Gauge = 04500, Moves = new[] {188,226,041,014}, Location = 066 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 017, Level = 30, Gauge = 04000, Moves = new[] {017,287,211,297}, Location = 066 }, // Pidgeotto: Cipher Peon Lok @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 114, Level = 30, Gauge = 04000, Moves = new[] {076,234,241,275}, Location = 067 }, // Tangela: Cipher Peon Targ @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 012, Level = 30, Gauge = 04000, Moves = new[] {094,234,079,332}, Location = 067 }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 082, Level = 30, Gauge = 04500, Moves = new[] {038,287,240,087}, Location = 067 }, // Magneton: Cipher Peon Snidle @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 049, Level = 32, Gauge = 04000, Moves = new[] {318,287,164,094}, Location = 070 }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 070, Level = 32, Gauge = 04000, Moves = new[] {345,234,188,230}, Location = 070 }, // Weepinbell: Cipher Peon Angic @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 024, Level = 33, Gauge = 05000, Moves = new[] {188,287,137,044}, Location = 070 }, // Arbok: Cipher Peon Smarton @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 057, Level = 34, Gauge = 06000, Moves = new[] {238,270,116,179}, Location = 069 }, // Primeape: Cipher Admin Gorigan @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 097, Level = 34, Gauge = 05500, Moves = new[] {094,226,096,247}, Location = 069 }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 055, Level = 33, Gauge = 06500, Moves = new[] {127,204,244,280}, Location = 088 }, // Golduck: Navigator Abson @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 302, Level = 33, Gauge = 07000, Moves = new[] {247,270,185,105}, Location = 088 }, // Sableye: Navigator Abson @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 085, Level = 34, Gauge = 08000, Moves = new[] {065,226,097,161}, Location = 076 }, // Dodrio: Chaser Furgy @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 020, Level = 34, Gauge = 06000, Moves = new[] {162,287,184,158}, Location = 076 }, // Raticate: Chaser Furgy @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 083, Level = 36, Gauge = 05500, Moves = new[] {163,226,014,332}, Location = 076 }, // Farfetch'd: Cipher Admin Lovrina @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 334, Level = 36, Gauge = 06500, Moves = new[] {225,215,076,332}, Location = 076 }, // Altaria: Cipher Admin Lovrina @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 115, Level = 35, Gauge = 06000, Moves = new[] {089,047,039,146}, Location = 085 }, // Kangaskhan: Cipher Peon Litnar @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 354, Level = 37, Gauge = 07000, Moves = new[] {185,270,247,174}, Location = 085 }, // Banette: Cipher Peon Litnar @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 126, Level = 36, Gauge = 07000, Moves = new[] {126,266,238,009}, Location = 077 }, // Magmar: Cipher Peon Grupel @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 127, Level = 35, Gauge = 07000, Moves = new[] {012,270,206,066}, Location = 077 }, // Pinsir: Cipher Peon Grupel @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 078, Level = 40, Gauge = 06000, Moves = new[] {076,226,241,053}, Location = 080 }, // Rapidash: Cipher Peon Kolest @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 219, Level = 38, Gauge = 05500, Moves = new[] {257,287,089,053}, Location = 080 }, // Magcargo: Cipher Peon Kolest @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 107, Level = 38, Gauge = 06000, Moves = new[] {005,270,170,327}, Location = 081 }, // Hitmonchan: Cipher Peon Karbon @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 106, Level = 38, Gauge = 07000, Moves = new[] {136,287,170,025}, Location = 081 }, // Hitmonlee: Cipher Peon Petro @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 108, Level = 38, Gauge = 05000, Moves = new[] {038,270,111,205}, Location = 084 }, // Lickitung: Cipher Peon Geftal @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 123, Level = 40, Gauge = 08000, Moves = new[] {013,234,318,163}, Location = 084 }, // Scyther: Cipher Peon Leden @ Citadark Isle

            new EncounterStaticShadow { Fateful = true, Species = 113, Level = 39, Gauge = 04000, Moves = new[] {085,186,135,285}, Location = 084 }, // Chansey: Cipher Peon Leden @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 113, Level = 39, Gauge = 04000, Moves = new[] {085,186,135,285}, Location = 087 }, // Chansey: Cipher Peon Leden @ Citadark Isle

            new EncounterStaticShadow { Fateful = true, Species = 338, Level = 41, Gauge = 07500, Moves = new[] {094,226,241,322}, Location = 087 }, // Solrock: Cipher Admin Snattle @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 121, Level = 41, Gauge = 07500, Moves = new[] {127,287,058,105}, Location = 087 }, // Starmie: Cipher Admin Snattle @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 277, Level = 43, Gauge = 07000, Moves = new[] {143,226,097,263}, Location = 087 }, // Swellow: Cipher Admin Ardos @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 125, Level = 43, Gauge = 07000, Moves = new[] {238,266,086,085}, Location = 087 }, // Electabuzz: Cipher Admin Ardos @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 143, Level = 43, Gauge = 09000, Moves = new[] {090,287,174,034}, Location = 087 }, // Snorlax: Cipher Admin Ardos @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 062, Level = 42, Gauge = 07500, Moves = new[] {056,270,240,280}, Location = 087 }, // Poliwrath: Cipher Admin Gorigan @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 122, Level = 42, Gauge = 06500, Moves = new[] {094,266,227,009}, Location = 087 }, // Mr. Mime: Cipher Admin Gorigan @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 051, Level = 40, Gauge = 05000, Moves = new[] {089,204,201,161}, Location = 075 }, // Dugtrio: Cipher Peon Kolax @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 310, Level = 44, Gauge = 07000, Moves = new[] {087,287,240,044}, Location = 073 }, // Manectric: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 373, Level = 50, Gauge = 09000, Moves = new[] {337,287,349,332}, Location = 073 }, // Salamence: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 105, Level = 44, Gauge = 06500, Moves = new[] {089,047,014,157}, Location = 073 }, // Marowak: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 131, Level = 44, Gauge = 06000, Moves = new[] {056,215,240,059}, Location = 073 }, // Lapras: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 249, Level = 50, Gauge = 12000, Moves = new[] {354,297,089,056}, Location = 074 }, // Lugia: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 112, Level = 46, Gauge = 07000, Moves = new[] {224,270,184,089}, Location = 074 }, // Rhydon: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 146, Level = 50, Gauge = 10000, Moves = new[] {326,234,261,053}, Location = 074 }, // Moltres: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 103, Level = 46, Gauge = 09000, Moves = new[] {094,287,095,246}, Location = 074 }, // Exeggutor: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 128, Level = 46, Gauge = 09000, Moves = new[] {089,287,039,034}, Location = 074 }, // Tauros: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 144, Level = 50, Gauge = 10000, Moves = new[] {326,215,114,058}, Location = 074 }, // Articuno: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 145, Level = 50, Gauge = 10000, Moves = new[] {326,226,319,085}, Location = 074 }, // Zapdos: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 149, Level = 55, Gauge = 09000, Moves = new[] {063,215,349,089}, Location = 162 }, // Dragonite: Wanderer Miror B. @ Gateon Port
        };

        internal static readonly EncounterArea[] SlotsXD =
        {
            new EncounterArea { Location = 090, Slots = new[] // Rock
                {
                    new EncounterSlot {Species = 027, LevelMin = 10, LevelMax = 23, SlotNumber = 0}, // Sandshrew
                    new EncounterSlot {Species = 207, LevelMin = 10, LevelMax = 20, SlotNumber = 1}, // Gligar
                    new EncounterSlot {Species = 328, LevelMin = 10, LevelMax = 20, SlotNumber = 2}, // Trapinch
                }
            },
            new EncounterArea { Location = 091, Slots = new[] // Oasis
                {
                    new EncounterSlot {Species = 187, LevelMin = 10, LevelMax = 20, SlotNumber = 0}, // Hoppip
                    new EncounterSlot {Species = 231, LevelMin = 10, LevelMax = 20, SlotNumber = 1}, // Phanpy
                    new EncounterSlot {Species = 283, LevelMin = 10, LevelMax = 20, SlotNumber = 2}, // Surskit
                }
            },
            new EncounterArea { Location = 092, Slots = new[] // Cave
                {
                    new EncounterSlot {Species = 041, LevelMin = 10, LevelMax = 21, SlotNumber = 0}, // Zubat
                    new EncounterSlot {Species = 304, LevelMin = 10, LevelMax = 21, SlotNumber = 1}, // Aron
                    new EncounterSlot {Species = 194, LevelMin = 10, LevelMax = 21, SlotNumber = 2}, // Wooper
                }
            },
        };

        internal static readonly EncounterStatic[] Encounter_CXD = Encounter_Colo.Concat(Encounter_XD).ToArray();

        #endregion
    }
}
