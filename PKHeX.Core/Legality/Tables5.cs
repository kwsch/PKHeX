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
            437, 442, 447, 450, 453, 458, 465, 466, 471, 504, 578, 616, 617, 621, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638,
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
        internal static readonly int[] DreamWorldBalls = WildPokeBalls5.Concat(new[] { 25 }).ToArray();

        internal static readonly int[] FutureEvolutionsGen5 =
        {
            700
        };
        internal static readonly int[] UnreleasedItems_5 =
        {
            005, // Safari Ball
            016, // Cherish Ball
            260, // Red Scarf
            261, // Blue Scarf
            262, // Pink Scarf
            263, // Green Scarf
            264, // Yellow Scarf
            492, // Fast Ball
            493, // Level Ball
            494, // Lure Ball
            495, // Heavy Ball
            496, // Love Ball
            497, // Friend Ball
            498, // Moon Ball
            499, // Sport Ball
            500, // Park Ball
            576, // Dream Ball
        };
        internal static readonly bool[] ReleasedHeldItems_5 = Enumerable.Range(0, MaxItemID_5_B2W2 + 1).Select(i => HeldItems_BW.Contains((ushort)i) && !UnreleasedItems_5.Contains(i)).ToArray();
        internal static readonly int[][] Tutors_B2W2 =
        {
            new[] { 450, 343, 162, 530, 324, 442, 402, 529, 340, 067, 441, 253, 009, 007, 008 }, // Driftveil City
            new[] { 277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401, 399, 428, 406, 304, 231 }, // Lentimas Town
            new[] { 020, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409, 355 }, // Humilau City
            new[] { 380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446, 214, 285 } // Nacrene City
        };

        #region Dream Radar Tables
        internal static readonly EncounterStatic[] Encounter_DreamRadar =
        {
            new EncounterStatic { Shiny=false, Species=120, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=137, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=174, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=175, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=213, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=238, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=280, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=333, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=425, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=436, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=442, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=447, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=479,},             // Rotom has no HA
            new EncounterStatic { Shiny=false, Species=517, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=561, Ability = 4 },
            new EncounterStatic { Shiny=false, Species=641, Ability = 4, Form = 1}, //Tornadus-T
            new EncounterStatic { Shiny=false, Species=642, Ability = 4, Form = 1}, //Thundurus-T
            new EncounterStatic { Shiny=false, Species=645, Ability = 4, Form = 1}, //Landorus-T
            new EncounterStatic { Shiny=false, Species=249, Ability = 4 }, //SS
            new EncounterStatic { Shiny=false, Species=250, Ability = 4 }, //HG
            new EncounterStatic { Shiny=false, Species=483, Ability = 4 }, //D
            new EncounterStatic { Shiny=false, Species=484, Ability = 4 }, //P
            new EncounterStatic { Shiny=false, Species=487, Ability = 4 }, //Pt
            new EncounterStatic { Shiny=false, Species=079, Ability = 4 }, //Slowpoke
            new EncounterStatic { Shiny=false, Species=163, Ability = 4 }, //Hoothoot
            new EncounterStatic { Shiny=false, Species=374, Ability = 4 }, //Beldum
        };
        #endregion
        #region DreamWorld Encounter
        internal static readonly EncounterStatic[] DreamWorld_Common =
        {
            // Pleasant forest
            new EncounterStatic { Species=019, Level = 10, Moves = new[]{098, 382, 231}, },	//Rattata
            new EncounterStatic { Species=043, Level = 10, Moves = new[]{230, 298, 202}, },	//Oddish
            new EncounterStatic { Species=069, Level = 10, Moves = new[]{022, 235, 402}, },	//Bellsprout
            new EncounterStatic { Species=077, Level = 10, Moves = new[]{033, 037, 257}, },	//Ponyta
            new EncounterStatic { Species=083, Level = 10, Moves = new[]{210, 355, 348}, },	//Farfetch'd
            new EncounterStatic { Species=084, Level = 10, Moves = new[]{045, 175, 355}, },	//Doduo
            new EncounterStatic { Species=102, Level = 10, Moves = new[]{140, 235, 202}, },	//Exeggcute
            new EncounterStatic { Species=108, Level = 10, Moves = new[]{122, 214, 431}, },	//Lickitung
            new EncounterStatic { Species=114, Level = 10, Moves = new[]{079, 073, 402}, },	//Tangela
            new EncounterStatic { Species=115, Level = 10, Moves = new[]{252, 068, 409}, },	//Kangaskhan
            new EncounterStatic { Species=161, Level = 10, Moves = new[]{010, 203, 343}, },	//Sentret
            new EncounterStatic { Species=179, Level = 10, Moves = new[]{084, 115, 351}, },	//Mareep
            new EncounterStatic { Species=191, Level = 10, Moves = new[]{072, 230, 414}, },	//Sunkern
            new EncounterStatic { Species=234, Level = 10, Moves = new[]{033, 050, 285}, },	//Stantler
            new EncounterStatic { Species=261, Level = 10, Moves = new[]{336, 305, 399}, },	//Poochyena
            new EncounterStatic { Species=283, Level = 10, Moves = new[]{145, 056, 202}, },	//Surskit
            new EncounterStatic { Species=399, Level = 10, Moves = new[]{033, 401, 290}, },	//Bidoof
            new EncounterStatic { Species=403, Level = 10, Moves = new[]{268, 393, 400}, },	//Shinx
            new EncounterStatic { Species=431, Level = 10, Moves = new[]{252, 372, 290}, },	//Glameow
            new EncounterStatic { Species=054, Level = 10, Moves = new[]{346, 227, 362}, },	//Psyduck
            new EncounterStatic { Species=058, Level = 10, Moves = new[]{044, 034, 203}, },	//Growlithe
            new EncounterStatic { Species=123, Level = 10, Moves = new[]{098, 226, 366}, },	//Scyther
            new EncounterStatic { Species=128, Level = 10, Moves = new[]{099, 231, 431}, },	//Tauros
            new EncounterStatic { Species=183, Level = 10, Moves = new[]{111, 453, 008}, },	//Marill
            new EncounterStatic { Species=185, Level = 10, Moves = new[]{175, 205, 272}, },	//Sudowoodo
            new EncounterStatic { Species=203, Level = 10, Moves = new[]{093, 243, 285}, },	//Girafarig
            new EncounterStatic { Species=241, Level = 10, Moves = new[]{111, 174, 231}, },	//Miltank
            new EncounterStatic { Species=263, Level = 10, Moves = new[]{033, 271, 387}, },	//Zigzagoon
            new EncounterStatic { Species=427, Level = 10, Moves = new[]{193, 252, 409}, },	//Buneary
            new EncounterStatic { Species=037, Level = 10, Moves = new[]{046, 257, 399}, },	//Vulpix
            new EncounterStatic { Species=060, Level = 10, Moves = new[]{095, 054, 214}, },	//Poliwag
            new EncounterStatic { Species=177, Level = 10, Moves = new[]{101, 297, 202}, },	//Natu
            new EncounterStatic { Species=239, Level = 10, Moves = new[]{084, 238, 393}, },	//Elekid
            new EncounterStatic { Species=300, Level = 10, Moves = new[]{193, 321, 445}, },	//Skitty
	        // Windskept Sky
            new EncounterStatic { Species=016, Level = 10, Moves = new[]{016, 211, 290}, },	//Pidgey
            new EncounterStatic { Species=021, Level = 10, Moves = new[]{064, 185, 211}, },	//Spearow
            new EncounterStatic { Species=041, Level = 10, Moves = new[]{048, 095, 162}, },	//Zubat
            new EncounterStatic { Species=142, Level = 10, Moves = new[]{044, 372, 446}, },	//Aerodactyl
            new EncounterStatic { Species=165, Level = 10, Moves = new[]{004, 450, 009}, },	//Ledyba
            new EncounterStatic { Species=187, Level = 10, Moves = new[]{235, 227, 340}, },	//Hoppip
            new EncounterStatic { Species=193, Level = 10, Moves = new[]{098, 364, 202}, },	//Yanma
            new EncounterStatic { Species=198, Level = 10, Moves = new[]{064, 109, 355}, },	//Murkrow
            new EncounterStatic { Species=207, Level = 10, Moves = new[]{028, 364, 366}, },	//Gligar
            new EncounterStatic { Species=225, Level = 10, Moves = new[]{217, 420, 264}, },	//Delibird
            new EncounterStatic { Species=276, Level = 10, Moves = new[]{064, 203, 413}, },	//Taillow
            new EncounterStatic { Species=397, Level = 14, Moves = new[]{017, 297, 366}, },	//Staravia
            new EncounterStatic { Species=227, Level = 10, Moves = new[]{064, 065, 355}, },	//Skarmory
            new EncounterStatic { Species=357, Level = 10, Moves = new[]{016, 073, 318}, },	//Tropius
	        // Sparkling Sea
            new EncounterStatic { Species=086, Level = 10, Moves = new[]{029, 333, 214}, },	//Seel
            new EncounterStatic { Species=090, Level = 10, Moves = new[]{110, 112, 196}, },	//Shellder
            new EncounterStatic { Species=116, Level = 10, Moves = new[]{145, 190, 362}, },	//Horsea
            new EncounterStatic { Species=118, Level = 10, Moves = new[]{064, 060, 352}, },	//Goldeen
            new EncounterStatic { Species=129, Level = 10, Moves = new[]{150, 175, 340}, },	//Magikarp
            new EncounterStatic { Species=138, Level = 10, Moves = new[]{044, 330, 196}, },	//Omanyte
            new EncounterStatic { Species=140, Level = 10, Moves = new[]{071, 175, 446}, },	//Kabuto
            new EncounterStatic { Species=170, Level = 10, Moves = new[]{086, 133, 351}, },	//Chinchou
            new EncounterStatic { Species=194, Level = 10, Moves = new[]{055, 034, 401}, },	//Wooper
            new EncounterStatic { Species=211, Level = 10, Moves = new[]{040, 453, 290}, },	//Qwilfish
            new EncounterStatic { Species=223, Level = 10, Moves = new[]{199, 350, 362}, },	//Remoraid
            new EncounterStatic { Species=226, Level = 10, Moves = new[]{048, 243, 314}, },	//Mantine
            new EncounterStatic { Species=320, Level = 10, Moves = new[]{055, 214, 340}, },	//Wailmer
            new EncounterStatic { Species=339, Level = 10, Moves = new[]{189, 214, 209}, },	//Barboach
            new EncounterStatic { Species=366, Level = 10, Moves = new[]{250, 445, 392}, },	//Clamperl
            new EncounterStatic { Species=369, Level = 10, Moves = new[]{055, 214, 414}, },	//Relicanth
            new EncounterStatic { Species=370, Level = 10, Moves = new[]{204, 300, 196}, },	//Luvdisc
            new EncounterStatic { Species=418, Level = 10, Moves = new[]{346, 163, 352}, },	//Buizel
            new EncounterStatic { Species=456, Level = 10, Moves = new[]{213, 186, 352}, },	//Finneon
            new EncounterStatic { Species=072, Level = 10, Moves = new[]{048, 367, 202}, },	//Tentacool
            new EncounterStatic { Species=318, Level = 10, Moves = new[]{044, 037, 399}, },	//Carvanha
            new EncounterStatic { Species=341, Level = 10, Moves = new[]{106, 232, 283}, },	//Corphish
            new EncounterStatic { Species=345, Level = 10, Moves = new[]{051, 243, 202}, },	//Lileep
            new EncounterStatic { Species=347, Level = 10, Moves = new[]{010, 446, 440}, },	//Anorith
            new EncounterStatic { Species=349, Level = 10, Moves = new[]{150, 445, 243}, },	//Feebas
            new EncounterStatic { Species=131, Level = 10, Moves = new[]{109, 032, 196}, },	//Lapras
            new EncounterStatic { Species=147, Level = 10, Moves = new[]{086, 352, 225}, },	//Dratini
 	        // Spooky Mannor
            new EncounterStatic { Species=092, Level = 10, Moves = new[]{095, 050, 482}, },	//Gastly
            new EncounterStatic { Species=096, Level = 10, Moves = new[]{095, 427, 409}, },	//Drowzee
            new EncounterStatic { Species=122, Level = 10, Moves = new[]{112, 298, 285}, },	//Mr. Mime
            new EncounterStatic { Species=167, Level = 10, Moves = new[]{040, 527, 450}, },	//Spinarak
            new EncounterStatic { Species=200, Level = 10, Moves = new[]{149, 194, 517}, },	//Misdreavus
            new EncounterStatic { Species=228, Level = 10, Moves = new[]{336, 364, 399}, },	//Houndour
            new EncounterStatic { Species=325, Level = 10, Moves = new[]{149, 285, 278}, },	//Spoink
            new EncounterStatic { Species=353, Level = 10, Moves = new[]{101, 194, 220}, },	//Shuppet
            new EncounterStatic { Species=355, Level = 10, Moves = new[]{050, 220, 271}, },	//Duskull
            new EncounterStatic { Species=358, Level = 10, Moves = new[]{035, 095, 304}, },	//Chimecho
            new EncounterStatic { Species=434, Level = 10, Moves = new[]{103, 492, 389}, },	//Stunky
            new EncounterStatic { Species=209, Level = 10, Moves = new[]{204, 370, 038}, },	//Snubbull
            new EncounterStatic { Species=235, Level = 10, Moves = new[]{166, 445, 214}, },	//Smeargle
            new EncounterStatic { Species=313, Level = 10, Moves = new[]{148, 271, 366}, },	//Volbeat
            new EncounterStatic { Species=314, Level = 10, Moves = new[]{204, 313, 366}, },	//Illumise
            new EncounterStatic { Species=063, Level = 10, Moves = new[]{100, 285, 356}, },	//Abra
	        // Rugged Mountain
            new EncounterStatic { Species=066, Level = 10, Moves = new[]{067, 418, 270}, },	//Machop
            new EncounterStatic { Species=081, Level = 10, Moves = new[]{319, 278, 356}, },	//Magnemite
            new EncounterStatic { Species=109, Level = 10, Moves = new[]{123, 399, 482}, },	//Koffing
            new EncounterStatic { Species=218, Level = 10, Moves = new[]{052, 517, 257}, },	//Slugma
            new EncounterStatic { Species=246, Level = 10, Moves = new[]{044, 399, 446}, },	//Larvitar
            new EncounterStatic { Species=324, Level = 10, Moves = new[]{052, 090, 446}, },	//Torkoal
            new EncounterStatic { Species=328, Level = 10, Moves = new[]{044, 324, 202}, },	//Trapinch
            new EncounterStatic { Species=331, Level = 10, Moves = new[]{071, 298, 009}, },	//Cacnea
            new EncounterStatic { Species=412, Level = 10, Moves = new[]{182, 450, 173}, },	//Burmy
            new EncounterStatic { Species=449, Level = 10, Moves = new[]{044, 254, 276}, },	//Hippopotas
            new EncounterStatic { Species=240, Level = 10, Moves = new[]{052, 009, 257}, },	//Magby
            new EncounterStatic { Species=322, Level = 10, Moves = new[]{052, 034, 257}, },	//Numel
            new EncounterStatic { Species=359, Level = 10, Moves = new[]{364, 224, 276}, },	//Absol
            new EncounterStatic { Species=453, Level = 10, Moves = new[]{040, 409, 441}, },	//Croagunk
            new EncounterStatic { Species=236, Level = 10, Moves = new[]{252, 364, 183}, },	//Tyrogue
            new EncounterStatic { Species=371, Level = 10, Moves = new[]{044, 349, 200}, },	//Bagon
	        // Icy Cave
            new EncounterStatic { Species=027, Level = 10, Moves = new[]{028, 068, 162}, },	//Sandshrew
            new EncounterStatic { Species=074, Level = 10, Moves = new[]{111, 446, 431}, },	//Geodude
            new EncounterStatic { Species=095, Level = 10, Moves = new[]{020, 446, 431}, },	//Onix
            new EncounterStatic { Species=100, Level = 10, Moves = new[]{268, 324, 363}, },	//Voltorb
            new EncounterStatic { Species=104, Level = 10, Moves = new[]{125, 195, 067}, },	//Cubone
            new EncounterStatic { Species=293, Level = 10, Moves = new[]{253, 283, 428}, },	//Whismur
            new EncounterStatic { Species=304, Level = 10, Moves = new[]{106, 283, 457}, },	//Aron
            new EncounterStatic { Species=337, Level = 10, Moves = new[]{093, 414, 236}, },	//Lunatone
            new EncounterStatic { Species=338, Level = 10, Moves = new[]{093, 428, 234}, },	//Solrock
            new EncounterStatic { Species=343, Level = 10, Moves = new[]{229, 356, 428}, },	//Baltoy
            new EncounterStatic { Species=459, Level = 10, Moves = new[]{075, 419, 202}, },	//Snover
            new EncounterStatic { Species=050, Level = 10, Moves = new[]{028, 251, 446}, },	//Diglett
            new EncounterStatic { Species=215, Level = 10, Moves = new[]{269, 008, 067}, },	//Sneasel
            new EncounterStatic { Species=361, Level = 10, Moves = new[]{181, 311, 352}, },	//Snorunt
            new EncounterStatic { Species=220, Level = 10, Moves = new[]{316, 246, 333}, },	//Swinub
            new EncounterStatic { Species=443, Level = 10, Moves = new[]{082, 200, 203}, },	//Gible
	        // Dream Park
            new EncounterStatic { Species=046, Level = 10, Moves = new[]{078, 440, 235}, },	//Paras
            new EncounterStatic { Species=204, Level = 10, Moves = new[]{120, 390, 356}, },	//Pineco
            new EncounterStatic { Species=265, Level = 10, Moves = new[]{040, 450, 173}, },	//Wurmple
            new EncounterStatic { Species=273, Level = 10, Moves = new[]{074, 331, 492}, },	//Seedot
            new EncounterStatic { Species=287, Level = 10, Moves = new[]{281, 400, 389}, },	//Slakoth
            new EncounterStatic { Species=290, Level = 10, Moves = new[]{141, 203, 400}, },	//Nincada
            new EncounterStatic { Species=311, Level = 10, Moves = new[]{086, 435, 324}, },	//Plusle
            new EncounterStatic { Species=312, Level = 10, Moves = new[]{086, 435, 324}, },	//Minun
            new EncounterStatic { Species=316, Level = 10, Moves = new[]{139, 151, 202}, },	//Gulpin
            new EncounterStatic { Species=352, Level = 10, Moves = new[]{185, 285, 513}, },	//Kecleon
            new EncounterStatic { Species=401, Level = 10, Moves = new[]{522, 283, 253}, },	//Kricketot
            new EncounterStatic { Species=420, Level = 10, Moves = new[]{073, 505, 331}, },	//Cherubi
            new EncounterStatic { Species=455, Level = 10, Moves = new[]{044, 476, 380}, },	//Carnivine
            new EncounterStatic { Species=023, Level = 10, Moves = new[]{040, 251, 399}, },	//Ekans
            new EncounterStatic { Species=175, Level = 10, Moves = new[]{118, 381, 253}, },	//Togepi
            new EncounterStatic { Species=190, Level = 10, Moves = new[]{010, 252, 007}, },	//Aipom
            new EncounterStatic { Species=285, Level = 10, Moves = new[]{078, 331, 264}, },	//Shroomish
            new EncounterStatic { Species=315, Level = 10, Moves = new[]{074, 079, 129}, },	//Roselia
            new EncounterStatic { Species=113, Level = 10, Moves = new[]{045, 068, 270}, },	//Chansey
            new EncounterStatic { Species=127, Level = 10, Moves = new[]{011, 370, 382}, },	//Pinsir
            new EncounterStatic { Species=133, Level = 10, Moves = new[]{028, 204, 129}, },	//Eevee
            new EncounterStatic { Species=143, Level = 10, Moves = new[]{133, 007, 278}, },	//Snorlax
            new EncounterStatic { Species=214, Level = 10, Moves = new[]{030, 175, 264}, },	//Heracross
            // Pokémon Café Forest
            new EncounterStatic { Species=061, Level = 25, Moves = new[]{240, 114, 352}, },	//Poliwhirl
            new EncounterStatic { Species=133, Level = 10, Moves = new[]{270, 204, 129}, },	//Eevee
            new EncounterStatic { Species=235, Level = 10, Moves = new[]{166, 445, 214}, },	//Smeargle
            new EncounterStatic { Species=412, Level = 10, Moves = new[]{182, 450, 173}, },	//Burmy
            //PGL
            new EncounterStatic { Species=212, Level = 10, Moves = new[]{211}, Gender = 0, }, //Scizor
            new EncounterStatic { Species=445, Level = 48, Gender = 0, },                     //Garchomp
            new EncounterStatic { Species=149, Level = 55, Moves = new[]{245}, Gender = 0, }, //Dragonite
            new EncounterStatic { Species=248, Level = 55, Moves = new[]{069}, Gender = 0, }, //Tyranitar
            new EncounterStatic { Species=376, Level = 45, Moves = new[]{038}, Gender = 2, }, //Metagross
        };

        internal static readonly EncounterStatic[] BW_DreamWorld = DreamWorld_Common.Concat(new[]
        {
            // Pleasant forest
            new EncounterStatic { Species=029, Level = 10, Moves = new[]{010, 389, 162}, },	//Nidoran (F)
            new EncounterStatic { Species=032, Level = 10, Moves = new[]{064, 068, 162}, },	//Nidoran (M)
            new EncounterStatic { Species=174, Level = 10, Moves = new[]{047, 313, 270}, },	//Igglybuff  
            new EncounterStatic { Species=187, Level = 10, Moves = new[]{235, 270, 331}, },	//Hoppip     
            new EncounterStatic { Species=270, Level = 10, Moves = new[]{071, 073, 352}, },	//Lotad      
            new EncounterStatic { Species=276, Level = 10, Moves = new[]{064, 119, 366}, },	//Taillow    
            new EncounterStatic { Species=309, Level = 10, Moves = new[]{086, 423, 324}, },	//Electrike  
            new EncounterStatic { Species=351, Level = 10, Moves = new[]{052, 466, 352}, },	//Castform   
            new EncounterStatic { Species=417, Level = 10, Moves = new[]{098, 343, 351}, },	//Pachirisu  
            // Windskept Sky
            new EncounterStatic { Species=012, Level = 10, Moves = new[]{093, 355, 314}, },	//Butterfree 
            new EncounterStatic { Species=163, Level = 10, Moves = new[]{193, 101, 278}, },	//Hoothoot   
            new EncounterStatic { Species=278, Level = 10, Moves = new[]{055, 239, 351}, },	//Wingull     
            new EncounterStatic { Species=333, Level = 10, Moves = new[]{064, 297, 355}, },	//Swablu      
            new EncounterStatic { Species=425, Level = 10, Moves = new[]{107, 095, 285}, },	//Drifloon    
            new EncounterStatic { Species=441, Level = 10, Moves = new[]{119, 417, 272}, },	//Chatot      
            // Sparkling Sea
            new EncounterStatic { Species=079, Level = 10, Moves = new[]{281, 335, 362}, },	//Slowpoke    
            new EncounterStatic { Species=098, Level = 10, Moves = new[]{011, 133, 290}, },	//Krabby      
            new EncounterStatic { Species=119, Level = 33, Moves = new[]{352, 214, 203}, },	//Seaking     
            new EncounterStatic { Species=120, Level = 10, Moves = new[]{055, 278, 196}, },	//Staryu      
            new EncounterStatic { Species=222, Level = 10, Moves = new[]{145, 109, 446}, },	//Corsola     
            new EncounterStatic { Species=422, Level = 10, Moves = new[]{189, 281, 290}, Form = 0 },	//Shellos
            new EncounterStatic { Species=422, Level = 10, Moves = new[]{189, 281, 290}, Form = 1 },
            // Spooky Mannor
            new EncounterStatic { Species=202, Level = 15, Moves = new[]{243, 204, 227}, },	//Wobbuffet   
            new EncounterStatic { Species=238, Level = 10, Moves = new[]{186, 445, 285}, },	//Smoochum    
            new EncounterStatic { Species=303, Level = 10, Moves = new[]{313, 424, 008}, }, //Mawile      
            new EncounterStatic { Species=307, Level = 10, Moves = new[]{096, 409, 203}, },	//Meditite    
            new EncounterStatic { Species=436, Level = 10, Moves = new[]{095, 285, 356}, },	//Bronzor     
            new EncounterStatic { Species=052, Level = 10, Moves = new[]{010, 095, 290}, },	//Meowth      
            new EncounterStatic { Species=479, Level = 10, Moves = new[]{086, 351, 324}, },	//Rotom       
            new EncounterStatic { Species=280, Level = 10, Moves = new[]{093, 194, 270}, },	//Ralts       
            new EncounterStatic { Species=302, Level = 10, Moves = new[]{193, 389, 180}, },	//Sableye     
            new EncounterStatic { Species=442, Level = 10, Moves = new[]{180, 220, 196}, },	//Spiritomb   
            // Rugged Mountain
            new EncounterStatic { Species=056, Level = 10, Moves = new[]{067, 179, 009}, },	//Mankey      
            new EncounterStatic { Species=111, Level = 10, Moves = new[]{030, 068, 038}, },	//Rhyhorn     
            new EncounterStatic { Species=231, Level = 10, Moves = new[]{175, 484, 402}, },	//Phanpy      
            new EncounterStatic { Species=451, Level = 10, Moves = new[]{044, 097, 401}, },	//Skorupi     
            new EncounterStatic { Species=216, Level = 10, Moves = new[]{313, 242, 264}, },	//Teddiursa   
            new EncounterStatic { Species=296, Level = 10, Moves = new[]{292, 270, 008}, },	//Makuhita    
            new EncounterStatic { Species=327, Level = 10, Moves = new[]{383, 252, 276}, },	//Spinda      
            new EncounterStatic { Species=374, Level = 10, Moves = new[]{036, 428, 442}, },	//Beldum      
            new EncounterStatic { Species=447, Level = 10, Moves = new[]{203, 418, 264}, },	//Riolu       
            // Icy Cave
            new EncounterStatic { Species=173, Level = 10, Moves = new[]{227, 312, 214}, },	//Cleffa      
            new EncounterStatic { Species=213, Level = 10, Moves = new[]{227, 270, 504}, },	//Shuckle     
            new EncounterStatic { Species=299, Level = 10, Moves = new[]{033, 446, 246}, },	//Nosepass    
            new EncounterStatic { Species=363, Level = 10, Moves = new[]{181, 090, 401}, },	//Spheal      
            new EncounterStatic { Species=408, Level = 10, Moves = new[]{029, 442, 007}, },	//Cranidos    
            new EncounterStatic { Species=206, Level = 10, Moves = new[]{111, 277, 446}, },	//Dunsparce   
            new EncounterStatic { Species=410, Level = 10, Moves = new[]{182, 068, 090}, },	//Shieldon    
            // Dream Park
            new EncounterStatic { Species=048, Level = 10, Moves = new[]{050, 226, 285}, }, //Venonat     
            new EncounterStatic { Species=088, Level = 10, Moves = new[]{139, 114, 425}, },	//Grimer      
            new EncounterStatic { Species=415, Level = 10, Moves = new[]{016, 366, 314}, },	//Combee      
            new EncounterStatic { Species=015, Level = 10, Moves = new[]{031, 314, 210}, },	//Beedrill    
            new EncounterStatic { Species=335, Level = 10, Moves = new[]{098, 458, 067}, },	//Zangoose    
            new EncounterStatic { Species=336, Level = 10, Moves = new[]{044, 034, 401}, },	//Seviper    
            // PGL
            new EncounterStatic { Species=134, Level = 10, Gender = 0, }, //Vaporeon
            new EncounterStatic { Species=135, Level = 10, Gender = 0, }, //Jolteon
            new EncounterStatic { Species=136, Level = 10, Gender = 0, }, //Flareon
            new EncounterStatic { Species=196, Level = 10, Gender = 0, }, //Espeon
            new EncounterStatic { Species=197, Level = 10, Gender = 0, }, //Umbreon
            new EncounterStatic { Species=470, Level = 10, Gender = 0, }, //Leafeon
            new EncounterStatic { Species=471, Level = 10, Gender = 0, }, //Glaceon
            new EncounterStatic { Species=001, Level = 10, Gender = 0, }, //Bulbasaur
            new EncounterStatic { Species=004, Level = 10, Gender = 0, }, //Charmander
            new EncounterStatic { Species=007, Level = 10, Gender = 0, }, //Squirtle
            new EncounterStatic { Species=453, Level = 10, Gender = 0, }, //Croagunk
            new EncounterStatic { Species=387, Level = 10, Gender = 0, }, //Turtwig
            new EncounterStatic { Species=390, Level = 10, Gender = 0, }, //Chimchar
            new EncounterStatic { Species=393, Level = 10, Gender = 0, }, //Piplup
            new EncounterStatic { Species=493, Level = 100 },             //Arceus
            new EncounterStatic { Species=252, Level = 10, Gender = 0, }, //Treecko
            new EncounterStatic { Species=255, Level = 10, Gender = 0, }, //Torchic
            new EncounterStatic { Species=258, Level = 10, Gender = 0, }, //Mudkip
            new EncounterStatic { Species=468, Level = 10, Moves = new[]{217}, Gender = 0, }, //Togekiss
            new EncounterStatic { Species=473, Level = 34, Gender = 0, }, //Mamoswine
            new EncounterStatic { Species=137, Level = 10 },              //Porygon
            new EncounterStatic { Species=384, Level = 50 },              //Rayquaza
            new EncounterStatic { Species=354, Level = 37, Moves = new[]{538}, Gender = 1, }, //Banette
            new EncounterStatic { Species=453, Level = 10, Moves = new[]{398}, Gender = 0, }, //Croagunk
            new EncounterStatic { Species=334, Level = 35, Moves = new[]{206}, Gender = 0,},  //Altaria
            new EncounterStatic { Species=242, Level = 10 },              //Blissey
            new EncounterStatic { Species=448, Level = 10, Moves = new[]{418}, Gender = 0, }, //Lucario
            new EncounterStatic { Species=189, Level = 27, Moves = new[]{206}, Gender = 0, }, //Jumpluff 
        }).ToArray();

        internal static readonly EncounterStatic[] B2W2_DreamWorld = DreamWorld_Common.Concat(new[]
        {
            // Pleasant forest
            new EncounterStatic { Species=535, Level = 10, Moves = new[]{496, 414, 352}, },	//Tympole    
            new EncounterStatic { Species=546, Level = 10, Moves = new[]{073, 227, 388}, },	//Cottonee   
            new EncounterStatic { Species=548, Level = 10, Moves = new[]{079, 204, 230}, },	//Petilil    
            new EncounterStatic { Species=588, Level = 10, Moves = new[]{203, 224, 450}, },	//Karrablast 
            new EncounterStatic { Species=616, Level = 10, Moves = new[]{051, 226, 227}, },	//Shelmet    
            new EncounterStatic { Species=545, Level = 30, Moves = new[]{342, 390, 276}, },	//Scolipede  
            // Windskept Sky
            new EncounterStatic { Species=519, Level = 10, Moves = new[]{016, 095, 234}, },	//Pidove      
            new EncounterStatic { Species=561, Level = 10, Moves = new[]{095, 500, 257}, },	//Sigilyph    
            new EncounterStatic { Species=580, Level = 10, Moves = new[]{432, 362, 382}, },	//Ducklett    
            new EncounterStatic { Species=587, Level = 10, Moves = new[]{098, 403, 204}, },	//Emolga      
            // Sparkling Sea
            new EncounterStatic { Species=550, Level = 10, Moves = new[]{029, 097, 428}, Form = 0 },//Basculin
            new EncounterStatic { Species=550, Level = 10, Moves = new[]{029, 097, 428}, Form = 1 },
            new EncounterStatic { Species=594, Level = 10, Moves = new[]{392, 243, 220}, },	//Alomomola   
            new EncounterStatic { Species=618, Level = 10, Moves = new[]{189, 174, 281}, },	//Stunfisk    
            new EncounterStatic { Species=564, Level = 10, Moves = new[]{205, 175, 334}, },	//Tirtouga    
            // Spooky Mannor
            new EncounterStatic { Species=605, Level = 10, Moves = new[]{377, 112, 417}, },	//Elgyem      
            new EncounterStatic { Species=624, Level = 10, Moves = new[]{210, 427, 389}, },	//Pawniard    
            new EncounterStatic { Species=596, Level = 36, Moves = new[]{486, 050, 228}, },	//Galvantula  
            new EncounterStatic { Species=578, Level = 32, Moves = new[]{105, 286, 271}, },	//Duosion     
            new EncounterStatic { Species=622, Level = 10, Moves = new[]{205, 007, 009}, },	//Golett 
            // Rugged Mountain
            new EncounterStatic { Species=631, Level = 10, Moves = new[]{510, 257, 202}, },	//Heatmor     
            new EncounterStatic { Species=632, Level = 10, Moves = new[]{210, 203, 422}, },	//Durant      
            new EncounterStatic { Species=556, Level = 10, Moves = new[]{042, 073, 191}, },	//Maractus    
            new EncounterStatic { Species=558, Level = 34, Moves = new[]{157, 068, 400}, },	//Crustle     
            new EncounterStatic { Species=553, Level = 40, Moves = new[]{242, 068, 212}, },	//Krookodile  
            // Icy Cave
            new EncounterStatic { Species=529, Level = 10, Moves = new[]{229, 319, 431}, },	//Drilbur     
            new EncounterStatic { Species=621, Level = 10, Moves = new[]{044, 424, 389}, },	//Druddigon   
            new EncounterStatic { Species=525, Level = 25, Moves = new[]{479, 174, 484}, },	//Boldore     
            new EncounterStatic { Species=583, Level = 35, Moves = new[]{429, 420, 286}, },	//Vanillish   
            new EncounterStatic { Species=600, Level = 38, Moves = new[]{451, 356, 393}, },	//Klang       
            new EncounterStatic { Species=610, Level = 10, Moves = new[]{082, 068, 400}, },	//Axew        
            // Dream Park
            new EncounterStatic { Species=531, Level = 10, Moves = new[]{270, 227, 281}, },	//Audino      
            new EncounterStatic { Species=538, Level = 10, Moves = new[]{020, 008, 276}, },	//Throh       
            new EncounterStatic { Species=539, Level = 10, Moves = new[]{249, 009, 530}, },	//Sawk        
            new EncounterStatic { Species=559, Level = 10, Moves = new[]{067, 252, 409}, },	//Scraggy     
            new EncounterStatic { Species=533, Level = 25, Moves = new[]{067, 183, 409}, },	//Gurdurr   
            // PGL
            new EncounterStatic { Species=575, Level = 32, Moves = new[]{243}, Gender = 0, }, //Gothorita
            new EncounterStatic { Species=025, Level = 10, Moves = new[]{029}, Gender = 0, }, //Pikachu
            new EncounterStatic { Species=511, Level = 10, Moves = new[]{437}, Gender = 0, }, //Pansage
            new EncounterStatic { Species=513, Level = 10, Moves = new[]{257}, Gender = 0, }, //Pansear
            new EncounterStatic { Species=515, Level = 10, Moves = new[]{056}, Gender = 0, }, //Panpour
            new EncounterStatic { Species=387, Level = 10, Moves = new[]{254}, Gender = 0, }, //Turtwig
            new EncounterStatic { Species=390, Level = 10, Moves = new[]{252}, Gender = 0, }, //Chimchar
            new EncounterStatic { Species=393, Level = 10, Moves = new[]{297}, Gender = 0, }, //Piplup
            new EncounterStatic { Species=575, Level = 32, Moves = new[]{286}, Gender = 0, }, //Gothorita  
        }).ToArray();
        #endregion
        #region Static Encounter/Gift Tables
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
            new EncounterStatic { Species = 638, Level = 42, Location = 74,}, //Cobalion @ Guidance Chamber
            new EncounterStatic { Species = 639, Level = 42, Location = 73,}, //Terrakion @ Trial Chamber
            new EncounterStatic { Species = 640, Level = 42, Location = 55,}, //Virizion @ Rumination Field
            new EncounterStatic { Species = 643, Level = 50, Location = 45, Shiny = false, Version = GameVersion.B, }, //Reshiram @ N'Castle
            new EncounterStatic { Species = 643, Level = 50, Location = 39, Shiny = false, Version = GameVersion.B, }, //Reshiram @ Dragonspiral Tower
            new EncounterStatic { Species = 644, Level = 50, Location = 45, Shiny = false, Version = GameVersion.W, }, //Zekrom @ N'Castle
            new EncounterStatic { Species = 644, Level = 50, Location = 39, Shiny = false, Version = GameVersion.W, }, //Zekrom @ Dragonspiral Tower
            new EncounterStatic { Species = 645, Level = 70, Location = 70,}, //Landorus @ Abundant Shrine
            new EncounterStatic { Species = 646, Level = 75, Location = 61,}, //Kyurem @ Giant Chasm
            //Event
            new EncounterStatic { Species = 494, Level = 15, Location = 62, Shiny = false}, // Victini @ Liberty Garden
            new EncounterStatic { Species = 570, Level = 10, Location = 08, Gender = 0, }, // Zorua @ Castelia City
            new EncounterStatic { Species = 571, Level = 25, Location = 72, Gender = 1, }, // Zoroark @ Lostlorn Forest
        };
        internal static readonly EncounterStatic[] Encounter_BW = Encounter_BW_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_BW)).Concat(Encounter_BW_Regular).ToArray();
        internal static readonly EncounterStatic[] Encounter_B2W2_Regular =
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
            new EncounterStatic { Gift = true, Species = 440, Level = 01, EggLocation = 60003, }, // Happiny Egg from PKMN Breeder
            //Stationary
            new EncounterStatic { Species = 590, Level = 29, Location = 19, }, // Foongus @ Route 6
            new EncounterStatic { Species = 591, Level = 43, Location = 24, }, // Amoonguss @ Route 11
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
            //N's Pokemon
            new EncounterStatic { Species = 509, Level = 07, Location = 15, NSparkle = true, Ability = 2, Nature = Nature.Timid }, //Purloin @ Route 2
            new EncounterStatic { Species = 519, Level = 13, Location = 33, NSparkle = true, Ability = 2, Nature = Nature.Sassy }, //Pidove @ Pinwheel Forest
            new EncounterStatic { Species = 532, Level = 13, Location = 33, NSparkle = true, Ability = 1, Nature = Nature.Rash }, //Timburr @ Pinwheel Forest
            new EncounterStatic { Species = 535, Level = 13, Location = 33, NSparkle = true, Ability = 2, Nature = Nature.Modest }, //Tympole @ Pinwheel Forest
            new EncounterStatic { Species = 527, Level = 55, Location = 53, NSparkle = true, Ability = 1, Nature = Nature.Timid }, //Woobat @ Wellspring Cave
            new EncounterStatic { Species = 551, Level = 22, Location = 34, NSparkle = true, Ability = 2, Nature = Nature.Docile }, //Sandile @ Desert Resort
            new EncounterStatic { Species = 554, Level = 22, Location = 34, NSparkle = true, Ability = 1, Nature = Nature.Naive }, //Darumaka @ Desert Resort
            new EncounterStatic { Species = 555, Level = 35, Location = 34, NSparkle = true, Ability = 4, Nature = Nature.Calm }, //Darmanitan @ Desert Resort
            new EncounterStatic { Species = 559, Level = 22, Location = 34, NSparkle = true, Ability = 1, Nature = Nature.Lax }, //Scaggy @ Desert Resort
            new EncounterStatic { Species = 561, Level = 22, Location = 34, NSparkle = true, Ability = 2, Nature = Nature.Gentle }, //Sigilyph @ Desert Resort
            new EncounterStatic { Species = 525, Level = 28, Location = 37, NSparkle = true, Ability = 1, Nature = Nature.Naive }, //Boldore @ Chargestone Cave
            new EncounterStatic { Species = 595, Level = 28, Location = 37, NSparkle = true, Ability = 2, Nature = Nature.Docile }, //Joltik @ Chargestone Cave
            new EncounterStatic { Species = 597, Level = 28, Location = 37, NSparkle = true, Ability = 1, Nature = Nature.Bashful }, //Ferroseed @ Chargestone Cave
            new EncounterStatic { Species = 599, Level = 28, Location = 37, NSparkle = true, Ability = 1, Nature = Nature.Rash }, //Klink @ Chargestone Cave
            new EncounterStatic { Species = 570, Level = 25, Location = 10, NSparkle = true, Ability = 1, Nature = Nature.Hasty, Gift = true} //N's Zorua @ Driftveil City
        };
        internal static readonly EncounterStatic[] Encounter_B2W2 = Encounter_DreamRadar.SelectMany(e => e.DreamRadarClone()).Concat(Encounter_B2W2_Regular).ToArray();
        #endregion
        #region Trade Tables
        internal static readonly EncounterTrade[] TradeGift_BW =
        {
            new EncounterTrade { Species = 548, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest, Version = GameVersion.B, }, // Petilil
            new EncounterTrade { Species = 546, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest, Version = GameVersion.W, }, // Cottonee
            new EncounterTrade { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Version = GameVersion.B, Form = 0, }, // Basculin-Red
            new EncounterTrade { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Version = GameVersion.W, Form = 1, }, // Basculin-Blue
            new EncounterTrade { Species = 587, Level = 30, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,20,31,20,20,20}, Nature = Nature.Lax, }, // Emolga
            new EncounterTrade { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Gentle, }, // Rotom
            new EncounterTrade { Species = 446, Level = 60, Ability = 2, TID = 40217, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Serious, }, // Munchlax
        };
        internal static readonly EncounterTrade[] TradeGift_B2W2 =
        {
            new EncounterTrade { Species = 548, Level = 20, Ability = 2, TID = 65217, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Timid, Version = GameVersion.B2, }, // Petilil
            new EncounterTrade { Species = 546, Level = 20, Ability = 1, TID = 05720, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest, Version = GameVersion.W2, }, // Cottonee
            new EncounterTrade { Species = 526, Level = 35, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, }, // Gigalith
            new EncounterTrade { Species = 465, Level = 45, Ability = 1, TID = 27658, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Hardy, }, // Tangrowth
            new EncounterTrade { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Calm, }, // Rotom
            new EncounterTrade { Species = 424, Level = 40, Ability = 2, TID = 17074, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Jolly, }, // Ambipom
            new EncounterTrade { Species = 065, Level = 40, Ability = 1, TID = 17074, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Timid, }, // Alakazam
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
        };
        #endregion
        #region Alt Slots

        // White forest white version only
        private static readonly int[] WhiteForest_GrassSpecies =
        {
            016,029,032,043,063,066,069,081,092,111,
            137,175,179,187,239,240,265,270,273,280,
            287,293,298,304,328,371,396,403,406,440,
        };
        private static readonly int[] WhiteForest_SurfSpecies =
        {
            194,270,283,341,
        };
        private static readonly EncounterArea[] WhiteForestSlot = EncounterArea.GetSimpleEncounterArea(WhiteForest_GrassSpecies, new[] { 5, 5 }, 51, SlotType.Grass).Concat(
                                                                  EncounterArea.GetSimpleEncounterArea(WhiteForest_SurfSpecies, new[] { 5, 5 }, 51, SlotType.Surf)).ToArray();
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
                new EncounterArea {Location = 24, Slots = new[]{new EncounterSlot {Species = 046},},},// Paras @ Route 11
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
            new EncounterArea {Location = 031, Slots = new[]{new EncounterSlot {Species = 187},},},// Hoppip @ Route 18
            new EncounterArea {Location = 032, Slots = new[]{new EncounterSlot {Species = 097},},},// Hypno @ Dreamyard
            new EncounterArea {Location = 034, Slots = new[]{new EncounterSlot {Species = 450},},},// Hippowdon @ Desert Resort
            new EncounterArea {Location = 070, Slots = new[]{new EncounterSlot {Species = 079},},},// Slowpoke @ Abundant shrine
            new EncounterArea {Location = 132, Slots = new[]{new EncounterSlot {Species = 332},},},// Cacturne @ Reaversal Mountian
        };
        private static readonly EncounterArea[] SlotsB2_Swarm = SlotsB2W2_Swarm.Concat(new[] {
                new EncounterArea {Location = 016, Slots = new[]{new EncounterSlot {Species = 313},},},// Volbeat @ Route 3
                new EncounterArea {Location = 019, Slots = new[]{new EncounterSlot {Species = 311},},},// Plusle @ Route 6
                new EncounterArea {Location = 125, Slots = new[]{new EncounterSlot {Species = 185},},},// Sudowoodo @ Route 20
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
        internal static readonly int[] EggLocations5 =
        {
            30002, // Link Trade (NPC)
            30003, // Link Trade
            60002, // Daycare
            60003, // Breeder (NPC)
        };
    }
}
