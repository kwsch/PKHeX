using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesIndex_5_BW = 667;
        internal const int MaxSpeciesIndex_5_B2W2 = 708;
        internal const int MaxSpeciesID_5 = 649;
        internal const int MaxMoveID_5 = 559;
        internal const int MaxItemID_5_BW = 632;
        internal const int MaxItemID_5_B2W2 = 638;
        internal const int MaxAbilityID_5 = 164;
        internal const int MaxBallID_5 = 0x19;
        internal const int MaxGameID_5 = 23; // B2

        internal static readonly int[] Met_BW2c = { 0, 60002, 30003 };

        internal static readonly int[] Met_BW2_0 =
        {
            1, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
            75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
            102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123,
            124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 139, 140, 141, 142, 143, 144, 145, 146,
            147, 148, 149, 150, 151, 152, 153,
        };

        internal static readonly int[] Met_BW2_3 =
        {
            30001, 30002, 30004, 30005, 30006, 30007, 30008, 30010, 30011, 30012,
            30013, 30014, 30015,
        };

        internal static readonly int[] Met_BW2_4 =
        {
            40001, 40002, 40003, 40004, 40005, 40006, 40007, 40008, 40009, 40010,
            40011, 40012, 40013, 40014, 40015, 40016, 40017, 40018, 40019, 40020, 40021, 40022, 40023, 40024, 40025,
            40026, 40027, 40028, 40029, 40030, 40031, 40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039, 40040,
            40041, 40042, 40043, 40044, 40045, 40046, 40047, 40048, 40049, 40050, 40051, 40052, 40053, 40054, 40055,
            40056, 40057, 40058, 40059, 40060, 40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069, 40070,
            40071, 40072, 40073, 40074, 40075, 40076, 40077, 40078, 40079, 40080, 40081, 40082, 40083, 40084, 40085,
            40086, 40087, 40088, 40089, 40090, 40091, 40092, 40093, 40094, 40095, 40096, 40097, 40098, 40099, 40100,
            40101, 40102, 40103, 40104, 40105, 40106, 40107, 40108, 40109,
        };

        internal static readonly int[] Met_BW2_6 = { 60001, 60003 };

        internal static readonly ushort[] Pouch_Items_BW = {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 116, 117, 118, 119, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 492, 493, 494, 495, 496, 497, 498, 499, 500, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 571, 572, 573, 575, 576, 577, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590,
        };
        internal static readonly ushort[] Pouch_Key_BW = {
            437, 442, 447, 450, 465, 466, 471, 504, 533, 574, 578, 579, 616, 617, 621, 623, 624, 625, 626,
        };
        internal static readonly ushort[] Pouch_TMHM_BW = {
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 618, 619, 620, 420, 421, 422, 423, 424, 425
        };
        internal static readonly ushort[] Pouch_Medicine_BW = {
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 134, 504, 565, 566, 567, 568, 569, 570, 591
        };
        internal static readonly ushort[] Pouch_Berries_BW = {
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212
        };
        internal static readonly ushort[] HeldItems_BW = new ushort[1].Concat(Pouch_Items_BW).Concat(Pouch_Medicine_BW).Concat(Pouch_Berries_BW).ToArray();

        internal static readonly ushort[] Pouch_Key_B2W2 = {
            437, 442, 447, 450, 453, 458, 465, 466, 471, 504, 578, 616, 617, 621, 626, 627, 628, 630, 631, 632, 633, 634, 635, 636, 637, 638,
        };
        internal static readonly int[] TMHM_BW =
        {
            468, 337, 473, 347, 046, 092, 258, 339, 474, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 477, 219,
            218, 076, 479, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
            259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
            502, 411, 412, 206, 503, 374, 451, 507, 510, 511,
            261, 512, 373, 153, 421, 371, 514, 416, 397, 148,
            444, 521, 086, 360, 014, 522, 244, 523, 524, 157,
            404, 525, 526, 398, 138, 447, 207, 365, 369, 164,
            430, 433, 528, 249, 555,

            015, 019, 057, 070, 127, 291
        };

        internal static readonly int[] MovePP_BW =
        {
            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 15, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 10, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 25, 15, 10, 40, 25, 10, 35, 30, 15, 10, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 10, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 15, 20, 15, 15, 35, 20, 15, 10, 10, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 10,
            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 10, 15, 15,
            10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15,
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 10, 05, 40, 15, 20, 20, 05, 15, 20, 30, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 40, 15, 20, 15, 20, 15, 20, 10,
            20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 10, 10, 15, 10, 20, 30, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15,
            10, 15, 20, 15, 10, 10, 10, 20, 10, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20,
            10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 10, 05, 05, 10, 05, 05, 05, 15, 10, 10, 10, 10, 10, 10, 15, 20, 15, 10, 15, 10, 15, 10, 20, 10, 15, 10, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 15, 20, 15, 10,
            15, 15, 15, 15, 10, 10, 10, 10, 10, 15, 15, 15, 15, 05, 05, 15, 05, 10, 10, 10, 20, 20, 20, 10, 10, 30, 15, 15, 10, 15, 25, 10, 20, 10, 10, 10, 20, 10, 10, 10, 10, 10, 15, 15, 05, 05, 10, 10, 10, 05,
            05, 10, 05, 05, 15, 10, 05, 05, 05,
        };
        internal static readonly int[] WildPokeBalls5 =
        {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            // Cherish ball not usable
            // HGSS balls not usable
            // Dream ball not usable in wild
        };

        internal static readonly int[] FutureEvolutionsGen5 =
        {
            700
        };
        internal static readonly int[] UnreleasedItems_5 =
        {
            // todo
        };
        internal static readonly bool[] ReleasedHeldItems_5 = Enumerable.Range(0, MaxItemID_5_B2W2 + 1).Select(i => HeldItems_BW.Contains((ushort)i) && !UnreleasedItems_5.Contains(i)).ToArray();
        internal static readonly int[][] Tutors_B2W2 =
        {
            new[] { 343, 450, 529, 340, 324, 442, 162, 253, 402, 530, 067, 441, 007, 009, 008 }, // Driftveil City
            new[] { 387, 334, 393, 277, 335, 304, 527, 196, 231, 401, 414, 428, 492, 276, 356, 406, 399 }, // Lentimas Town
            new[] { 020, 173, 215, 282, 235, 355, 143, 272, 257, 202, 409, 220, 366 }, // Humilau City
            new[] { 388, 380, 270, 495, 478, 472, 180, 278, 271, 446, 200, 283, 214, 285, 289, } // Nacrene City
        };

        internal static readonly int[] Roaming_MetLocation_BW =
        {
            25,26,27,28, // Route 12,13,14,15 Night latter half
            15,16,31,    // Route 2,3,18 Morning 
            17,18,29,    // Route 4,5,16 Daytime 
            19,20,21,    // Route 6,7,8 Evening 
            22,23,24,    // Route 9,10,11 Night former half
        };
        internal static readonly EncounterStatic[] Encounter_BW_Roam =
        {
            new EncounterStatic { Species = 641, Level = 40, Version = GameVersion.B, Roaming = true }, //Tornadus
            new EncounterStatic { Species = 642, Level = 40, Version = GameVersion.W, Roaming = true }, //Thundurus
        };

        internal static readonly EncounterStatic[] Encounter_BW_Regular =
        {
            //Starters @ Nuvema Town
            new EncounterStatic { Gift = true, Species = 495, Level = 5, Location = 4, }, // Snivys
            new EncounterStatic { Gift = true, Species = 498, Level = 5, Location = 4, }, // Tepig
            new EncounterStatic { Gift = true, Species = 501, Level = 5, Location = 4, }, // Oshawott

            //Fossil @ Nacrene City
            new EncounterStatic { Gift = true, Species = 138, Level = 25, Location = 7, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 25, Location = 7, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 25, Location = 7, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 25, Location = 7, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 25, Location = 7, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 25, Location = 7, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 25, Location = 7, }, // Shieldon
            new EncounterStatic { Gift = true, Species = 564, Level = 25, Location = 7, }, // Tirtouga
            new EncounterStatic { Gift = true, Species = 566, Level = 25, Location = 7, }, // Archen

            //Gift
            new EncounterStatic { Gift = true, Species = 511, Level = 10, Location = 32, }, // Pansage @ Dreamyard
            new EncounterStatic { Gift = true, Species = 513, Level = 10, Location = 32, }, // Pansear
            new EncounterStatic { Gift = true, Species = 515, Level = 10, Location = 32, }, // Panpour
            new EncounterStatic { Gift = true, Species = 129, Level = 05, Location = 68, }, // Magikarp @ Marvelous Bridge
            new EncounterStatic { Gift = true, Species = 636, Level = 01, EggLocation = 60003, }, // Larvesta Egg from Treasure Hunter

            //Stationary
            new EncounterStatic { Species = 518, Level = 50, Location = 32, Ability = 4, }, //Musharna @ Dreamyard Friday Only
            new EncounterStatic { Species = 590, Level = 20, Location = 19, }, //Foongus @ Route 6
            new EncounterStatic { Species = 590, Level = 30, Location = 23, }, //Foongus @ Route 10
            new EncounterStatic { Species = 591, Level = 40, Location = 23, }, //Amoonguss @ Route 10
            new EncounterStatic { Species = 555, Level = 35, Location = 34, Ability = 4, }, //Darmanitan @ Desert Resort
            new EncounterStatic { Species = 637, Level = 70, Location = 35, }, //Volcarona @ Relic Castle

            //Stationary Lengerdary
            new EncounterStatic { Species = 638, Level = 42, Location = 54,}, //Cobalion @ Mistralton Cave
            new EncounterStatic { Species = 639, Level = 42, Location = 40,}, //Terrakion @ Victory Road
            new EncounterStatic { Species = 640, Level = 42, Location = 33,}, //Virizion @ Pinwheel Forest
            new EncounterStatic { Species = 643, Level = 50, Location = 45, Shiny = false, Version = GameVersion.B, }, //Reshiram @ N'Castle
            new EncounterStatic { Species = 643, Level = 50, Location = 39, Shiny = false, Version = GameVersion.B, }, //Reshiram @ Dragonspiral Tower
            new EncounterStatic { Species = 644, Level = 50, Location = 45, Shiny = false, Version = GameVersion.W, }, //Zekrom @ N'Castle
            new EncounterStatic { Species = 644, Level = 50, Location = 39, Shiny = false, Version = GameVersion.W, }, //Zekrom @ Dragonspiral Tower
            new EncounterStatic { Species = 645, Level = 70, Location = 70,}, //Landorus @ Abundant Shrine
            new EncounterStatic { Species = 646, Level = 75, Location = 61,}, //Kyurem @ Giant Chasm

            //Event
            new EncounterStatic { Species = 494, Level = 15, Location = 62, Shiny = false}, // Victini @ Liberty Garden
            new EncounterStatic { Species = 570, Level = 10, Location = 32, Gender = 0, Gift = true, }, // Zorua @ Castelia City
            new EncounterStatic { Species = 571, Level = 25, Location = 72, Gender = 1, }, // Zoroark @ Lostlorn Forest
        };

        internal static readonly EncounterStatic[] Encounter_BW = Encounter_BW_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_BW)).Concat(Encounter_BW_Regular).ToArray();

        internal static readonly EncounterStatic[] Encounter_B2W2 =
        {
            //Starters @ Aspertia City
            new EncounterStatic { Gift = true, Species = 495, Level = 5, Location = 117, }, // Snivy
            new EncounterStatic { Gift = true, Species = 498, Level = 5, Location = 117, }, // Tepig
            new EncounterStatic { Gift = true, Species = 501, Level = 5, Location = 117, }, // Oshawott

            //Fossil @ Nacrene City
            new EncounterStatic { Gift = true, Species = 138, Level = 25, Location = 7, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 25, Location = 7, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 25, Location = 7, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 25, Location = 7, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 25, Location = 7, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 25, Location = 7, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 25, Location = 7, }, // Shieldon
            new EncounterStatic { Gift = true, Species = 564, Level = 25, Location = 7, }, // Tirtouga
            new EncounterStatic { Gift = true, Species = 566, Level = 25, Location = 7, }, // Archen

            //Gift
            new EncounterStatic { Gift = true, Species = 133, Level = 10, Ability = 4, Location = 8, }, //HA Eevee @ Castelia City
            new EncounterStatic { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 19, Form = 0, }, //HA Deerling @ Route 6
            new EncounterStatic { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 19, Form = 1, }, //HA Deerling @ Route 6
            new EncounterStatic { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 19, Form = 2, }, //HA Deerling @ Route 6
            new EncounterStatic { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 19, Form = 3, }, //HA Deerling @ Route 6
            new EncounterStatic { Gift = true, Species = 443, Level = 01, Shiny = true, Location = 122, Gender = 0, Version = GameVersion.B2, }, //Shiny Gible @ Floccesy Town
            new EncounterStatic { Gift = true, Species = 147, Level = 01, Shiny = true, Location = 122, Gender = 0, Version = GameVersion.W2, }, //Shiny Dratini @ Floccesy Town
            new EncounterStatic { Gift = true, Species = 129, Level = 05, Location = 68, }, // Magikarp @ Marvelous Bridge
            new EncounterStatic { Gift = true, Species = 440, Level = 01, Ability = 1, EggLocation = 60003, }, // Happiny Egg from PKMN Breeder

            //Stationary
            new EncounterStatic { Species = 590, Level = 29, Location = 19, }, // Foongus @ Route 6
            new EncounterStatic { Species = 591, Level = 47, Location = 24, }, // Amoonguss @ Route 11
            new EncounterStatic { Species = 593, Level = 40, Location = 71, Ability = 4, Version = GameVersion.B2, Gender = 0,}, // HA Jellicent @ Undella Bay Mon Only
            new EncounterStatic { Species = 593, Level = 40, Location = 71, Ability = 4, Version = GameVersion.W2, Gender = 1,}, // HA Jellicent @ Undella Bay Thurs Only
            new EncounterStatic { Species = 628, Level = 25, Location = 17, Ability = 4, Version = GameVersion.W2, Gender = 0,}, // HA Braviary @ Route 4 Mon Only
            new EncounterStatic { Species = 630, Level = 25, Location = 17, Ability = 4, Version = GameVersion.B2, Gender = 1,}, // HA Mandibuzz @ Route 4 Thurs Only
            new EncounterStatic { Species = 637, Level = 35, Location = 35, }, // Volcarona @ Relic Castle
            new EncounterStatic { Species = 637, Level = 65, Location = 35, }, // Volcarona @ Relic Castle
            new EncounterStatic { Species = 558, Level = 42, Location = 141, }, // Crustle @ Seaside Cave
            new EncounterStatic { Species = 612, Level = 60, Location = 147, Shiny = true}, // Haxorus @ Nature Preserve

            //Stationary Lengerdary
            new EncounterStatic { Species = 377, Level = 65, Location = 150,}, //Regirock @ Rock Peak Chamber
            new EncounterStatic { Species = 378, Level = 65, Location = 151,}, //Regice @ Iceberg Chamber
            new EncounterStatic { Species = 379, Level = 65, Location = 152,}, //Registeel @ Iron Chamber
            new EncounterStatic { Species = 380, Level = 68, Location = 032, Version = GameVersion.W2, }, // Latias @ Dreamyard
            new EncounterStatic { Species = 381, Level = 68, Location = 032, Version = GameVersion.B2, }, // Latios @ Dreamyard
            new EncounterStatic { Species = 480, Level = 65, Location = 007,}, //Uxie @ Nacrene City
            new EncounterStatic { Species = 481, Level = 65, Location = 056,}, //Mesprit @ Celestial Tower
            new EncounterStatic { Species = 482, Level = 65, Location = 128,}, //Azelf @ Route 23
            new EncounterStatic { Species = 485, Level = 68, Location = 132,}, //Heatran @ Reversal Mountain
            new EncounterStatic { Species = 486, Level = 68, Location = 038,}, //Regigigas @ Twist Mountain
            new EncounterStatic { Species = 488, Level = 68, Location = 068,}, //Cresselia @ Marvelous Bridge

            new EncounterStatic { Species = 638, Level = 45, Location = 026,}, // Cobalion @ Route 13
            new EncounterStatic { Species = 638, Level = 65, Location = 026,}, // Cobalion @ Route 13
            new EncounterStatic { Species = 639, Level = 45, Location = 127,}, // Terrakion @ Route 22
            new EncounterStatic { Species = 639, Level = 65, Location = 127,}, // Terrakion @ Route 22
            new EncounterStatic { Species = 640, Level = 45, Location = 024,}, // Virizion @ Route 11
            new EncounterStatic { Species = 640, Level = 65, Location = 024,}, // Virizion @ Route 11
            new EncounterStatic { Species = 643, Level = 70, Location = 039, Shiny = false, Version = GameVersion.W2, }, // Reshiram @ Dragonspiral Tower
            new EncounterStatic { Species = 644, Level = 70, Location = 039, Shiny = false, Version = GameVersion.B2, }, // Zekrom @ Dragonspiral Tower
            new EncounterStatic { Species = 646, Level = 70, Location = 061, Form = 0}, // Kyurem @ Giant Chasm
        };
        internal static readonly EncounterTrade[] TradeGift_BW =
        {
            new EncounterTrade { Species = 548, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Modest, Version = GameVersion.B, }, // Petilil
            new EncounterTrade { Species = 546, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Modest, Version = GameVersion.W, }, // Cottonee
            new EncounterTrade { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Version = GameVersion.B, Form = 0, }, // Basculin-Red
            new EncounterTrade { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Version = GameVersion.W, Form = 1, }, // Basculin-Blue
            new EncounterTrade { Species = 587, Level = 30, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,20,31,20,20,20}, Nature = Nature.Lax, }, // Emolga
            new EncounterTrade { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Gentle, }, // Rotom
            new EncounterTrade { Species = 446, Level = 60, Ability = 2, TID = 40217, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Serious, }, // Munchlax
        };
        internal static readonly EncounterTrade[] TradeGift_B2W2 =
        {
            new EncounterTrade { Species = 548, Level = 20, Ability = 2, TID = 65217, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Timid, Version = GameVersion.B2, }, // Petilil
            new EncounterTrade { Species = 546, Level = 20, Ability = 1, TID = 05720, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Modest, Version = GameVersion.W2, }, // Cottonee
            new EncounterTrade { Species = 526, Level = 35, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, }, // Gigalith
            new EncounterTrade { Species = 465, Level = 45, Ability = 1, TID = 27658, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Hardy, }, // Tangrowth
            new EncounterTrade { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Calm, }, // Rotom
            new EncounterTrade { Species = 424, Level = 40, Ability = 2, TID = 17074, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Jolly, }, // Ambipom
            new EncounterTrade { Species = 065, Level = 40, Ability = 1, TID = 17074, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Timid, }, // Alakazam
            // player is male
            new EncounterTrade { Species = 052, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 202, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 280, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 410, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 111, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 422, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, Form = 0, }, //Shellos-Red
            new EncounterTrade { Species = 303, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 442, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 143, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 216, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 327, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            new EncounterTrade { Species = 175, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1,},
            // player is female
            new EncounterTrade { Species = 056, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 202, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 280, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 408, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 111, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 422, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, Form = 1,}, //Shellos-Blue
            new EncounterTrade { Species = 302, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 442, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 143, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 231, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 327, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            new EncounterTrade { Species = 175, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0,},
            // Gift
            new EncounterTrade { Species = 570, Level = 25, Ability = 1, TID = 00002, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {30,30,30,30,30,30}, Nature = Nature.Hasty, Location = 10} //N's Zorua @ Driftveil City
        };
        #region Alt Slots
        private static readonly EncounterArea[] SlotsBW_Swarm =
        {
            //level range and Slottype will be marked later
            new EncounterArea {Location = 14, Slots = new[]{new EncounterSlot {Species = 083},},},// Farfetch'd @ Route 1
            new EncounterArea {Location = 15, Slots = new[]{new EncounterSlot {Species = 360},},},// Wynant @ Route 2
            new EncounterArea {Location = 17, Slots = new[]{new EncounterSlot {Species = 449},},},// Hippopotas @ Route 4
            new EncounterArea {Location = 18, Slots = new[]{new EncounterSlot {Species = 235},},},// Smeargle @ Route 5
            new EncounterArea {Location = 20, Slots = new[]{new EncounterSlot {Species = 161},},},// Sentret @ Route 7
            new EncounterArea {Location = 21, Slots = new[]{new EncounterSlot {Species = 453},},},// Croagunk @ Route 8
            new EncounterArea {Location = 23, Slots = new[]{new EncounterSlot {Species = 236},},},// Tyrogue @ Route 10
            new EncounterArea {Location = 25, Slots = new[]{new EncounterSlot {Species = 084},},},// Doduo @ Route 12
            new EncounterArea {Location = 26, Slots = new[]{new EncounterSlot {Species = 353},},},// Shuppet @ Route 13
            new EncounterArea {Location = 27, Slots = new[]{new EncounterSlot {Species = 193},},},// Yanma @ Route 14
            new EncounterArea {Location = 28, Slots = new[]{new EncounterSlot {Species = 056},},},// Mankey @ Route 15
            new EncounterArea {Location = 29, Slots = new[]{new EncounterSlot {Species = 204},},},// Pineco @ Route 16
            new EncounterArea {Location = 30, Slots = new[]{new EncounterSlot {Species = 102},},},// Exeggcutes @ Route 18
        };
        private static readonly EncounterArea[] SlotsB_Swarm = SlotsBW_Swarm.Concat(new[] {
                new EncounterArea {Location = 16, Slots = new[]{new EncounterSlot {Species = 313},},},// Volbeat @ Route 3
                new EncounterArea {Location = 19, Slots = new[]{new EncounterSlot {Species = 311},},},// Plusle @ Route 6
                new EncounterArea {Location = 22, Slots = new[]{new EncounterSlot {Species = 228},},},// Houndour @ Route 9
                new EncounterArea {Location = 24, Slots = new[]{new EncounterSlot {Species = 285},},},// Shroomish @ Route 11
        }).ToArray();
        private static readonly EncounterArea[] SlotsW_Swarm = SlotsBW_Swarm.Concat(new[] {
                new EncounterArea {Location = 16, Slots = new[]{new EncounterSlot {Species = 314},},},// Illumise @ Route 3
                new EncounterArea {Location = 19, Slots = new[]{new EncounterSlot {Species = 312},},},// Minun @ Route 6
                new EncounterArea {Location = 22, Slots = new[]{new EncounterSlot {Species = 261},},},// Poochyena @ Route 9
                new EncounterArea {Location = 24, Slots = new[]{new EncounterSlot {Species = 048},},},// Paras @ Route 11
        }).ToArray();

        private static readonly EncounterArea[] SlotsB2W2_Swarm =
        {
            //level range and Slottype will be marked later
            new EncounterArea {Location = 014, Slots = new[]{new EncounterSlot {Species = 083},},},// Farfetch'd @ Route 1
            new EncounterArea {Location = 018, Slots = new[]{new EncounterSlot {Species = 177},},},// Natu @ Route 5
            new EncounterArea {Location = 020, Slots = new[]{new EncounterSlot {Species = 162},},},// Furret @ Route 7
            new EncounterArea {Location = 021, Slots = new[]{new EncounterSlot {Species = 195},},},// Quagsire @ Route 8
            new EncounterArea {Location = 022, Slots = new[]{new EncounterSlot {Species = 317},},},// Swalot @ Route 9
            new EncounterArea {Location = 024, Slots = new[]{new EncounterSlot {Species = 284},},},// Masquerain @ Route 11
            new EncounterArea {Location = 025, Slots = new[]{new EncounterSlot {Species = 084},},},// Doduo @ Route 12
            new EncounterArea {Location = 026, Slots = new[]{new EncounterSlot {Species = 277},},},// Swellow @ Route 13
            new EncounterArea {Location = 028, Slots = new[]{new EncounterSlot {Species = 022},},},// Fearow @ Route 15
            new EncounterArea {Location = 029, Slots = new[]{new EncounterSlot {Species = 204},},},// Pineco @ Route 16
            new EncounterArea {Location = 030, Slots = new[]{new EncounterSlot {Species = 187},},},// Hoppip @ Route 18
            new EncounterArea {Location = 032, Slots = new[]{new EncounterSlot {Species = 097},},},// Hypno @ Dreamyard
            new EncounterArea {Location = 034, Slots = new[]{new EncounterSlot {Species = 450},},},// Hippowdon @ Desert Resort
            new EncounterArea {Location = 070, Slots = new[]{new EncounterSlot {Species = 079},},},// Slowpoke @ Abundant shrine
            new EncounterArea {Location = 132, Slots = new[]{new EncounterSlot {Species = 332},},},// Cacturne @ Reaversal Mountian
        };
        private static readonly EncounterArea[] SlotsB2_Swarm = SlotsB2W2_Swarm.Concat(new[] {
                new EncounterArea {Location = 016, Slots = new[]{new EncounterSlot {Species = 313},},},// Volbeat @ Route 3
                new EncounterArea {Location = 019, Slots = new[]{new EncounterSlot {Species = 311},},},// Plusle @ Route 6
                new EncounterArea {Location = 125, Slots = new[]{new EncounterSlot {Species = 184},},},// Sudowoodo @ Route 20
                new EncounterArea {Location = 127, Slots = new[]{new EncounterSlot {Species = 168},},},// Ariados @ Route 22
        }).ToArray();
        private static readonly EncounterArea[] SlotsW2_Swarm = SlotsB2W2_Swarm.Concat(new[] {
                new EncounterArea {Location = 016, Slots = new[]{new EncounterSlot {Species = 314},},},// Illumise @ Route 3
                new EncounterArea {Location = 019, Slots = new[]{new EncounterSlot {Species = 312},},},// Minun @ Route 6
                new EncounterArea {Location = 125, Slots = new[]{new EncounterSlot {Species = 122},},},// Mr.Mime @ Route 20
                new EncounterArea {Location = 127, Slots = new[]{new EncounterSlot {Species = 166},},},// Ledian @ Route 22
        }).ToArray();

        private static readonly EncounterSlot[] SlotsB2W2_HiddenGrottoEncounterSlots =
        {
            // reference http://bulbapedia.bulbagarden.net/wiki/Hidden_Grotto
            // Route 2
            new EncounterSlot{ Species = 029, LevelMin = 55, LevelMax = 60, }, // Nidoran-f
            new EncounterSlot{ Species = 032, LevelMin = 55, LevelMax = 60, }, // Nidoran-m
            new EncounterSlot{ Species = 210, LevelMin = 55, LevelMax = 60, }, // Granbull
            new EncounterSlot{ Species = 505, LevelMin = 55, LevelMax = 60, }, // Watchdog
            // Route 3
            new EncounterSlot{ Species = 310, LevelMin = 55, LevelMax = 60, }, // Manectric @ Dark grass
            new EncounterSlot{ Species = 417, LevelMin = 55, LevelMax = 60, }, // Pachirisu @ Dark grass
            new EncounterSlot{ Species = 523, LevelMin = 55, LevelMax = 60, }, // Zebstrika @ Dark grass
            new EncounterSlot{ Species = 048, LevelMin = 55, LevelMax = 60, }, // Venonat @ Pond
            new EncounterSlot{ Species = 271, LevelMin = 55, LevelMax = 60, }, // Lombre @ Pond
            new EncounterSlot{ Species = 400, LevelMin = 55, LevelMax = 60, }, // Bibarel @ Pond
            // Route 5
            new EncounterSlot{ Species = 510, LevelMin = 20, LevelMax = 25, }, // Liepard
            new EncounterSlot{ Species = 572, LevelMin = 20, LevelMax = 25, }, // Minccino 
            new EncounterSlot{ Species = 590, LevelMin = 20, LevelMax = 25, }, // Foongus 
            // Route 6
            new EncounterSlot{ Species = 206, LevelMin = 25, LevelMax = 30, }, // Dunsparce @ Near PKM Breeder 
            new EncounterSlot{ Species = 299, LevelMin = 25, LevelMax = 30, }, // Nospass @ Mistralton Cave
            new EncounterSlot{ Species = 527, LevelMin = 25, LevelMax = 30, }, // Woobat @ Both
            new EncounterSlot{ Species = 590, LevelMin = 25, LevelMax = 30, }, // Foongus @ Both
            // Route 7
            new EncounterSlot{ Species = 335, LevelMin = 30, LevelMax = 35, }, // Zangoose
            new EncounterSlot{ Species = 336, LevelMin = 30, LevelMax = 35, }, // Seviper
            new EncounterSlot{ Species = 505, LevelMin = 30, LevelMax = 35, }, // Watechdog
            new EncounterSlot{ Species = 613, LevelMin = 30, LevelMax = 35, }, // Cubchoo
            // Route 9
            new EncounterSlot{ Species = 089, LevelMin = 35, LevelMax = 40, }, // Muk
            new EncounterSlot{ Species = 510, LevelMin = 35, LevelMax = 40, }, // Liepard
            new EncounterSlot{ Species = 569, LevelMin = 35, LevelMax = 40, }, // Garbodor
            new EncounterSlot{ Species = 626, LevelMin = 35, LevelMax = 40, }, // Bouffalant
            // Route 13
            new EncounterSlot{ Species = 114, LevelMin = 35, LevelMax = 40, }, // Tangela @ Gaint Chasm
            new EncounterSlot{ Species = 363, LevelMin = 35, LevelMax = 40, }, // Spheal @ Stairs
            new EncounterSlot{ Species = 425, LevelMin = 35, LevelMax = 40, }, // Drifloon @ Stairs
            new EncounterSlot{ Species = 451, LevelMin = 35, LevelMax = 40, }, // Skorupi @ Gaint Chasm
            new EncounterSlot{ Species = 590, LevelMin = 35, LevelMax = 40, }, // Foongus @ Both
            // Route 18
            new EncounterSlot{ Species = 099, LevelMin = 55, LevelMax = 60, }, // Kingler
            new EncounterSlot{ Species = 149, LevelMin = 55, LevelMax = 60, }, // Dragonite
            new EncounterSlot{ Species = 222, LevelMin = 55, LevelMax = 60, }, // Corsola
            new EncounterSlot{ Species = 441, LevelMin = 55, LevelMax = 60, }, // Chatot
            // Pinwheel Forest
            new EncounterSlot{ Species = 061, LevelMin = 55, LevelMax = 60, }, // Poliwhirl @ Outer
            new EncounterSlot{ Species = 198, LevelMin = 55, LevelMax = 60, }, // Murkrow @ Inner
            new EncounterSlot{ Species = 286, LevelMin = 55, LevelMax = 60, }, // Breloom @ Inner
            new EncounterSlot{ Species = 297, LevelMin = 55, LevelMax = 60, }, // Hariyama @ Outer
            new EncounterSlot{ Species = 308, LevelMin = 55, LevelMax = 60, }, // Medicham @ Outer
            new EncounterSlot{ Species = 371, LevelMin = 55, LevelMax = 60, }, // Bagon @ Outer
            new EncounterSlot{ Species = 591, LevelMin = 55, LevelMax = 60, }, // Amoonguss @ Inner
            // Giant Chasm
            new EncounterSlot{ Species = 035, LevelMin = 45, LevelMax = 50, }, // Clefairy
            new EncounterSlot{ Species = 132, LevelMin = 45, LevelMax = 50, }, // Ditto
            new EncounterSlot{ Species = 215, LevelMin = 45, LevelMax = 50, }, // Sneasel
            new EncounterSlot{ Species = 375, LevelMin = 45, LevelMax = 50, }, // Metang
            // Abundant Shrine
            new EncounterSlot{ Species = 037, LevelMin = 35, LevelMax = 40, }, // Vulpix @ Near Youngster
            new EncounterSlot{ Species = 055, LevelMin = 35, LevelMax = 40, }, // Golduck @ Shrine
            new EncounterSlot{ Species = 333, LevelMin = 35, LevelMax = 40, }, // Swablu @ Shrine
            new EncounterSlot{ Species = 436, LevelMin = 35, LevelMax = 40, }, // Bronzor @ Near Youngster
            new EncounterSlot{ Species = 619, LevelMin = 35, LevelMax = 40, }, // Amoonguss @ Both
            // Lostlorn Forest
            new EncounterSlot{ Species = 127, LevelMin = 20, LevelMax = 25, }, // Pinsir
            new EncounterSlot{ Species = 214, LevelMin = 20, LevelMax = 25, }, // Heracross
            new EncounterSlot{ Species = 415, LevelMin = 20, LevelMax = 25, }, // Combee
            new EncounterSlot{ Species = 542, LevelMin = 20, LevelMax = 25, }, // Leavanny
            // Route 22
            new EncounterSlot{ Species = 279, LevelMin = 40, LevelMax = 45, }, // Peplipper
            new EncounterSlot{ Species = 591, LevelMin = 40, LevelMax = 45, }, // Amoonguss
            new EncounterSlot{ Species = 619, LevelMin = 50, LevelMax = 55, }, // Mienfoo
            // Route 23
            new EncounterSlot{ Species = 055, LevelMin = 50, LevelMax = 55, }, // Golduck
            new EncounterSlot{ Species = 207, LevelMin = 50, LevelMax = 55, }, // Gligar
            new EncounterSlot{ Species = 335, LevelMin = 50, LevelMax = 55, }, // Zangoose
            new EncounterSlot{ Species = 336, LevelMin = 50, LevelMax = 55, }, // Seviper
            new EncounterSlot{ Species = 359, LevelMin = 50, LevelMax = 55, }, // Absol
            // Floccesy Ranch
            new EncounterSlot{ Species = 183, LevelMin = 10, LevelMax = 15, }, // Marill
            new EncounterSlot{ Species = 206, LevelMin = 10, LevelMax = 15, }, // Dunsparce
            new EncounterSlot{ Species = 507, LevelMin = 10, LevelMax = 15, }, // Herdier
            // Funfest Missions
            // todo : check the level
            new EncounterSlot{Species = 133, LevelMin = 10, LevelMax = 60, }, //Eevee
            new EncounterSlot{Species = 134, LevelMin = 10, LevelMax = 60, },
            new EncounterSlot{Species = 135, LevelMin = 10, LevelMax = 60, },
            new EncounterSlot{Species = 136, LevelMin = 10, LevelMax = 60, },
            new EncounterSlot{Species = 196, LevelMin = 10, LevelMax = 60, },
            new EncounterSlot{Species = 197, LevelMin = 10, LevelMax = 60, },
            new EncounterSlot{Species = 470, LevelMin = 10, LevelMax = 60, },
            new EncounterSlot{Species = 471, LevelMin = 10, LevelMax = 60, }, 
        };
        private static readonly EncounterArea[] SlotsB2_HiddenGrotto =
        {
            new EncounterArea
            {
                Location = 143, // HiddenGrotto
                Slots = SlotsB2W2_HiddenGrottoEncounterSlots.Concat(new[]{
                    new EncounterSlot{Species = 015, LevelMin = 55, LevelMax = 60 }, // Beedrill @ Pinwheel Forest
                    new EncounterSlot{Species = 434, LevelMin = 10, LevelMax = 60 }, // Stunky from Funfest Missions
                }).ToArray(),
            }
        };
        private static readonly EncounterArea[] SlotsW2_HiddenGrotto =
        {
            new EncounterArea
            {
                Location = 143, // HiddenGrotto
                Slots = SlotsB2W2_HiddenGrottoEncounterSlots.Concat(new[]{
                    new EncounterSlot{Species = 012, LevelMin = 55, LevelMax = 60 }, // Butterfree @ Pinwheel Forest
                    new EncounterSlot{Species = 431, LevelMin = 10, LevelMax = 60 }, // Glameow from Funfest Missions
                }).ToArray(),
            }
        };

        #endregion

        internal static readonly int[] ValidMet_BW =
        {
                           004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020,
            021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037, 038, 039, 040,
            041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051, 052, 053, 054, 055, 056, 057, 058, 059, 060,
            061, 062, 063, 064, 065, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078, 079, 080,
            081, 082, 083, 084, 085, 086, 087, 088, 089, 090, 091, 092, 093, 094, 095, 096, 097, 098, 099, 100,
            101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
            
        };
        internal static readonly int[] ValidMet_B2W2 =
        {
                           004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020,
            021, 022,      024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037, 038, 039,      //023 Route 10, 040->134 Victory Road
            041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051, 052, 053, 054, 055, 056, 057, 058,      060, //059 Challenger's cave
            061, 062, 063, 064, 065, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078, 079, 080,
            081, 082, 083, 084, 085, 086, 087, 088, 089, 090, 091, 092, 093, 094, 095, 096, 097, 098, 099, 100,
            101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
            121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137,      139, 140, //138 ---
            141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153,
        };
    }
}
