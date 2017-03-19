using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_4 = 493;
        internal const int MaxMoveID_4 = 467;
        internal const int MaxItemID_4_DP = 464;
        internal const int MaxItemID_4_Pt = 467;
        internal const int MaxItemID_4_HGSS = 536;
        internal const int MaxAbilityID_4 = 123;
        internal const int MaxBallID_4 = 0x18;

        internal static readonly int[] Met_HGSS_0 =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
            19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45,
            46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72,
            73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99,
            100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121,
            122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
            144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165,
            166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187,
            188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
            210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231,
            232, 233, 234,
        };

        internal static readonly int[] Met_HGSS_2 =
        {
            2000, 2001, 2002, 2003, 2004, 2005, 2006, 2008, 2009, 2010, 2011,
            2012, 2013, 2014,
        };

        internal static readonly int[] Met_HGSS_3 =
        {
            3000, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010,
            3011, 3012, 3013, 3014, 3015, 3016, 3017, 3018, 3019, 3020, 3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028,
            3029, 3030, 3031, 3032, 3033, 3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3044, 3045, 3046,
            3047, 3048, 3049, 3050, 3051, 3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3061, 3062, 3063, 3064,
            3065, 3066, 3067, 3068, 3069, 3070, 3071, 3072, 3073, 3074, 3075, 3076
        };

        #region DP
        internal static readonly ushort[] Pouch_Items_DP = {
            68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 135, 136, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327
        };
        internal static readonly ushort[] Pouch_Key_DP = {
            428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464
        };
        internal static readonly ushort[] Pouch_TMHM_DP = {
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427
        };
        internal static readonly ushort[] Pouch_Mail_DP = {
            137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148
        };
        internal static readonly ushort[] Pouch_Medicine_DP = {
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54
        };
        internal static readonly ushort[] Pouch_Berries_DP = {
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212
        };
        internal static readonly ushort[] Pouch_Ball_DP = {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
        };
        internal static readonly ushort[] Pouch_Battle_DP = {
            55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67
        };
        internal static readonly ushort[] HeldItems_DP = new ushort[1].Concat(Pouch_Items_DP).Concat(Pouch_Mail_DP).Concat(Pouch_Medicine_DP).Concat(Pouch_Berries_DP).Concat(Pouch_Ball_DP).Concat(Pouch_TMHM_DP.Take(Pouch_TMHM_DP.Length - 8)).ToArray();
        #endregion

        #region Pt
        internal static readonly ushort[] Pouch_Items_Pt = {
            68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 135, 136, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327
        };
        internal static readonly ushort[] Pouch_Key_Pt = {
            428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467
        };
        internal static readonly ushort[] Pouch_TMHM_Pt = Pouch_TMHM_DP;
        internal static readonly ushort[] Pouch_Mail_Pt = Pouch_Mail_DP;
        internal static readonly ushort[] Pouch_Medicine_Pt = Pouch_Medicine_DP;
        internal static readonly ushort[] Pouch_Berries_Pt = Pouch_Berries_DP;
        internal static readonly ushort[] Pouch_Ball_Pt = Pouch_Ball_DP;
        internal static readonly ushort[] Pouch_Battle_Pt = Pouch_Battle_DP;

        internal static readonly ushort[] HeldItems_Pt = new ushort[1].Concat(Pouch_Items_Pt).Concat(Pouch_Mail_Pt).Concat(Pouch_Medicine_Pt).Concat(Pouch_Berries_Pt).Concat(Pouch_Ball_Pt).Concat(Pouch_TMHM_Pt.Take(Pouch_TMHM_Pt.Length - 8)).ToArray();
        #endregion

        #region HGSS
        internal static readonly ushort[] Pouch_Items_HGSS = Pouch_Items_Pt;
        internal static readonly ushort[] Pouch_Key_HGSS = {
            434, 435, 437, 444, 445, 446, 447, 450, 456, 464, 465, 466, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 501, 502, 503, 504, 532, 533, 534, 535, 536
        };
        internal static readonly ushort[] Pouch_TMHM_HGSS = Pouch_TMHM_DP;
        internal static readonly ushort[] Pouch_Mail_HGSS = Pouch_Mail_DP;
        internal static readonly ushort[] Pouch_Medicine_HGSS = Pouch_Medicine_DP;
        internal static readonly ushort[] Pouch_Berries_HGSS = Pouch_Berries_DP;
        internal static readonly ushort[] Pouch_Ball_HGSS = {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 492, 493, 494, 495, 496, 497, 498, 499, 500
        };
        internal static readonly ushort[] Pouch_Battle_HGSS = Pouch_Battle_DP;

        internal static readonly ushort[] HeldItems_HGSS = new ushort[1].Concat(Pouch_Items_HGSS).Concat(Pouch_Mail_HGSS).Concat(Pouch_Medicine_HGSS).Concat(Pouch_Berries_HGSS).Concat(Pouch_Ball_Pt).Concat(Pouch_TMHM_HGSS.Take(Pouch_TMHM_HGSS.Length - 8)).ToArray();
        #endregion

        internal static readonly int[] MovePP_DP =
        {
            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 15, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 25, 15, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 10, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 15,
            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15,
            10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15,
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 10, 05, 40, 15, 20, 20, 05, 15, 20, 30, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 40, 15, 20, 15, 20, 15, 20, 10,
            20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 10, 10, 15, 10, 20, 30, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15,
            10, 15, 20, 15, 10, 10, 10, 20, 05, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20,
            10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 10, 05, 05, 10, 05, 05, 05,
        };
        internal static readonly int[] WildPokeBalls4_DPPt =
        {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            // Cherish ball not usable
        };
        internal static readonly int[] WildPokeBalls4_HGSS =
        {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            // Cherish ball not usable
            17, 18, 19, 20, 21, 22,
            // Comp Ball not usable in wild
        };

        internal static readonly int[] FutureEvolutionsGen4 =
        {
            700
        };
        internal static readonly int[] UnreleasedItems_4 =
        {
            // todo
        };
        internal static readonly bool[] ReleasedHeldItems_4 = Enumerable.Range(0, MaxItemID_4_HGSS+1).Select(i => HeldItems_HGSS.Contains((ushort)i) && !UnreleasedItems_4.Contains(i)).ToArray();
        internal static readonly int[] CrownBeasts = {251, 243, 244, 245};

        internal static readonly int[] Tutors_4 =
        {
            291, 189, 210, 196, 205, 009, 007, 276,
            008, 442, 401, 466, 380, 173, 180, 314,
            270, 283, 200, 246, 235, 324, 428, 410,
            414, 441, 239, 402, 334, 393, 387, 340,
            271, 257, 282, 389, 129, 253, 162, 220,
            081, 366, 356, 388, 277, 272, 215, 067,
            143, 335, 450,
        };
        internal static readonly EncounterStatic[] Encounter_DPPt =
        {
            //todo
        };
        internal static readonly EncounterStatic[] Encounter_HGSS =
        {
            //Starters
            new EncounterStatic { Gift = true, Species = 1, Level = 5,  Location = 138, }, // Bulbasaur @ Pallet Town
            new EncounterStatic { Gift = true, Species = 4, Level = 5,  Location = 138, }, // Charmander
            new EncounterStatic { Gift = true, Species = 9, Level = 5,  Location = 138, }, // Squirtle
            new EncounterStatic { Gift = true, Species = 152, Level = 5,  Location = 126, }, // Chikorita @ New Bark Town
            new EncounterStatic { Gift = true, Species = 155, Level = 5,  Location = 126, }, // Cyndaquil
            new EncounterStatic { Gift = true, Species = 158, Level = 5,  Location = 126, }, // Totodile
            new EncounterStatic { Gift = true, Species = 252, Level = 5,  Location = 148, }, // Treecko @ Saffron City
            new EncounterStatic { Gift = true, Species = 255, Level = 5,  Location = 148, }, // Torchic
            new EncounterStatic { Gift = true, Species = 258, Level = 5,  Location = 148, }, // Mudkip

            //Fossil @ Pewter City
            new EncounterStatic { Gift = true, Species = 138, Level = 20,  Location = 140, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 20,  Location = 140, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 20,  Location = 140, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 20,  Location = 140, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20,  Location = 140, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 20,  Location = 140, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 20,  Location = 140, }, // Shieldon

            //Gift
            new EncounterStatic { Gift = true, Species = 133, Level = 5,  Location = 131, }, // Eevee @ Goldenrod Cityx
            new EncounterStatic { Gift = true, Species = 147, Level = 15,  Location = 222, Moves = new int[] {245, 086, 239, 082}, }, // Dratini @ Dragon's Den (ExtremeSpeed)
            new EncounterStatic { Gift = true, Species = 147, Level = 15,  Location = 222, Moves = new int[] {043, 086, 239, 082}, }, // Dratini @ Dragon's Den (Non-ExtremeSpeed)
            new EncounterStatic { Gift = true, Species = 236, Level = 10,  Location = 216, }, // Tyrogue @ Mt. Mortar

            //Stationary
            new EncounterStatic { Species = 130, Level = 30, Shiny = true ,Location = 135, }, //Gyarados @ Lake of Rage
            new EncounterStatic { Species = 131, Level = 20, Location = 210, }, //Lapras @ Union Cave
            new EncounterStatic { Species = 143, Level = 50, Location = 159, }, //Snorlax @ Route 11
            new EncounterStatic { Species = 143, Level = 50, Location = 160, }, //Snorlax @ Route 12
            new EncounterStatic { Species = 185, Level = 20, Location = 184, }, //Sudowoodo @ Route 36

            //Stationary Lengerdary
            new EncounterStatic { Species = 144, Level = 50, Location = 203, }, //Articuno @ Seafoam Islands
            new EncounterStatic { Species = 145, Level = 50, Location = 158, }, //Zapdos @ Route 10
            new EncounterStatic { Species = 146, Level = 50, Location = 137, }, //Moltres @ Mt. Silver
            new EncounterStatic { Species = 150, Level = 70, Location = 199, }, //Mewtwo @ Cerulean Cave
            new EncounterStatic { Species = 245, Level = 40, Location = 173, }, //Suicune @ Route 25
            new EncounterStatic { Species = 245, Level = 40, Location = 206, }, //Suicune @ Burned Tower
            new EncounterStatic { Species = 249, Level = 45, Location = 218, Version = GameVersion.SS, }, //Lugia @ Whirl Islands
            new EncounterStatic { Species = 249, Level = 70, Location = 218, Version = GameVersion.HG, }, //Lugia @ Whirl Islands
            new EncounterStatic { Species = 250, Level = 45, Location = 205, Version = GameVersion.HG, }, //Ho-Oh @ Bell Tower
            new EncounterStatic { Species = 250, Level = 70, Location = 205, Version = GameVersion.SS, }, //Ho-Oh @ Bell Tower
            new EncounterStatic { Species = 380, Level = 40, Location = 140, Version = GameVersion.SS, }, //Latias @ Pewter City
            new EncounterStatic { Species = 381, Level = 40, Location = 140, Version = GameVersion.HG, }, //Latios @ Pewter City
            new EncounterStatic { Species = 382, Level = 50, Location = 232, Version = GameVersion.HG, }, //Kyogre @ Embedded Tower
            new EncounterStatic { Species = 383, Level = 50, Location = 232, Version = GameVersion.SS, }, //Groudon @ Embedded Tower
            new EncounterStatic { Species = 384, Level = 50, Location = 232, }, //Rayquaza @ Embedded Tower
            new EncounterStatic { Species = 483, Level = 1, Location = 231, }, //Dialga @ Sinjoh Ruins
            new EncounterStatic { Species = 484, Level = 1, Location = 231, }, //Palkia @ Sinjoh Ruins
            new EncounterStatic { Species = 487, Level = 1, Location = 231, }, //Giratina @ Sinjoh Ruins

            //Swarm
            new EncounterStatic { Species = 113, Level = 23, Location = 161, }, //Chansey @ Route 13
            new EncounterStatic { Species = 132, Level = 34, Location = 195, }, //Ditto @ Route 47
            new EncounterStatic { Species = 183, Level = 15, Location = 216, }, //Marill @ Mt. Mortar
            new EncounterStatic { Species = 193, Level = 12, Location = 183, }, //Yanma @ Route 35
            new EncounterStatic { Species = 206, Level = 2, Location = 220, }, //Dunsparce @ Dark Cave
            new EncounterStatic { Species = 206, Level = 3, Location = 220, }, //Dunsparce @ Dark Cave
            new EncounterStatic { Species = 209, Level = 16, Location = 186, }, //Snubbull @ Route 38
            new EncounterStatic { Species = 211, Level = 40, Location = 180, }, //Qwilfish @ Route 32
            new EncounterStatic { Species = 223, Level = 20, Location = 192, }, //Remoraid @ Route 44
            new EncounterStatic { Species = 261, Level = 2, Location = 149, }, //Poochyena @ Route 1
            new EncounterStatic { Species = 278, Level = 35, Location = 143, }, //Wingull @ Vermillion City
            new EncounterStatic { Species = 280, Level = 10, Location = 182, }, //Ralts @ Route 34
            new EncounterStatic { Species = 280, Level = 11, Location = 182, }, //Ralts @ Route 34
            new EncounterStatic { Species = 302, Level = 13, Location = 157, Version = GameVersion.HG,}, //Sableye @ Route 9
            new EncounterStatic { Species = 302, Level = 14, Location = 157, Version = GameVersion.HG,}, //Sableye @ Route 9
            new EncounterStatic { Species = 302, Level = 15, Location = 157, Version = GameVersion.HG,}, //Sableye @ Route 9
            new EncounterStatic { Species = 303, Level = 13, Location = 157, Version = GameVersion.SS,}, //Mawile @ Route 9
            new EncounterStatic { Species = 303, Level = 14, Location = 157, Version = GameVersion.SS,}, //Mawile @ Route 9
            new EncounterStatic { Species = 303, Level = 15, Location = 157, Version = GameVersion.SS,}, //Mawile @ Route 9
            new EncounterStatic { Species = 316, Level = 5, Location = 151, Version = GameVersion.SS,}, //Gulpin @ Route 3
            new EncounterStatic { Species = 333, Level = 23, Location = 193, }, //Swablu @ Route 45
            new EncounterStatic { Species = 340, Level = 10, Location = 128, }, //Whiscash @ Violet City
            new EncounterStatic { Species = 340, Level = 20, Location = 128, }, //Whiscash @ Violet City
            new EncounterStatic { Species = 340, Level = 40, Location = 128, }, //Whiscash @ Violet City
            new EncounterStatic { Species = 343, Level = 5, Location = 151, Version = GameVersion.HG,}, //Baltoy @ Route 3
            new EncounterStatic { Species = 366, Level = 35, Location = 167, }, //Clamperl @ Route 19
            new EncounterStatic { Species = 369, Level = 40, Location = 160, }, //Relicanth @ Route 12
            new EncounterStatic { Species = 370, Level = 20, Location = 175, }, //Luvdisc @ Route 27
            new EncounterStatic { Species = 401, Level = 3, Location = 224, }, //Kricketot @ Viridian Forest
            new EncounterStatic { Species = 427, Level = 8, Location = 173, }, //Buneary @ Route 25
            new EncounterStatic { Species = 427, Level = 9, Location = 173, }, //Buneary @ Route 25
            new EncounterStatic { Species = 427, Level = 10, Location = 173, }, //Buneary @ Route 25

            //Roaming
            new EncounterStatic { Species = 243, Level = 40, }, //Raikou
            new EncounterStatic { Species = 244, Level = 40, }, //Entei
            new EncounterStatic { Species = 380, Level = 35, Version = GameVersion.HG, }, //Latias
            new EncounterStatic { Species = 381, Level = 35, Version = GameVersion.SS, }, //Latios
        };
        internal static readonly EncounterTrade[] TradeGift_DPPt =
        {
            //todo
        };
        internal static readonly EncounterTrade[] TradeGift_HGSS =
        {
            //todo
        };
    }
}
