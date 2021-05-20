using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesIndex_3 = 412;
        internal const int MaxSpeciesID_3 = 386;
        internal const int MaxMoveID_3 = 354;
        internal const int MaxItemID_3 = 374;
        internal const int MaxItemID_3_COLO = 547;
        internal const int MaxItemID_3_XD = 593;
        internal const int MaxAbilityID_3 = 77;
        internal const int MaxBallID_3 = 0xC;
        internal const int MaxGameID_3 = 15; // CXD

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

        internal static readonly ushort[] Pouch_Key_FRLG = ArrayUtil.ConcatAll(Pouch_Key_RS, new ushort[] { 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374 });
        internal static readonly ushort[] Pouch_Key_E = ArrayUtil.ConcatAll(Pouch_Key_FRLG, new ushort[] { 375, 376 });

        internal static readonly ushort[] Pouch_TMHM_RS = ArrayUtil.ConcatAll(Pouch_TM_RS, Pouch_HM_RS);
        internal static readonly ushort[] HeldItems_RS = ArrayUtil.ConcatAll(Pouch_Items_RS, Pouch_Ball_RS, Pouch_Berries_RS, Pouch_TM_RS);
        #endregion

        internal static readonly byte[] MovePP_RS =
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

        internal static readonly ushort[] Pouch_Cologne_COLO = {543, 544, 545};
        internal static readonly ushort[] Pouch_Items_COLO = ArrayUtil.ConcatAll(Pouch_Items_RS, new ushort[] {537}); // Time Flute
        internal static readonly ushort[] HeldItems_COLO = ArrayUtil.ConcatAll(Pouch_Items_COLO, Pouch_Ball_RS, Pouch_Berries_RS, Pouch_TM_RS);

        internal static readonly ushort[] Pouch_Key_COLO =
        {
            500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
            510, 511, 512, 513, 514, 515, 516, 517, 518, 519,
            520, 521, 522, 523, 524, 525, 526, 527, 528, 529,
            530, 531, 532, 533, 534, 535, 536,      538, 539,
            540, 541, 542,                546, 547,
        };

        internal static readonly ushort[] Pouch_Cologne_XD = {513, 514, 515};
        internal static readonly ushort[] Pouch_Items_XD = ArrayUtil.ConcatAll(Pouch_Items_RS, new ushort[] {511}); // Poké Snack
        internal static readonly ushort[] HeldItems_XD = ArrayUtil.ConcatAll(Pouch_Items_XD, Pouch_Ball_RS, Pouch_Berries_RS, Pouch_TM_RS);

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

        internal static readonly bool[] ReleasedHeldItems_3 = GetPermitList(MaxItemID_3, HeldItems_RS, new ushort[] {005}); // Safari Ball

        internal static readonly int[] TM_3 =
        {
            264, 337, 352, 347, 046, 092, 258, 339, 331, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
            218, 076, 231, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
            259, 263, 290, 156, 213, 168, 211, 285, 289, 315,
        };

        internal static readonly HashSet<int> HM_3 = new() { 15, 19, 57, 70, 148, 249, 127, 291};

        internal static readonly int[] Tutor_3Mew =
        {
            (int)Move.FeintAttack,
            (int)Move.FakeOut,
            (int)Move.Hypnosis,
            (int)Move.NightShade,
            (int)Move.RolePlay,
            (int)Move.ZapCannon,
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

        internal static readonly int[] SpecialTutors_Compatibility_FRLG = { 6, 9, 3 };

        internal static readonly int[] SpecialTutors_XD_Exclusive =
        {
            120, 143, 171
        };

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

        // 064 is an unused location for Meteor Falls
        // 084 is Inside of a truck, no possible pokemon can be hatched there
        // 071 is Mirage island, cannot be obtained as the player is technically still on Route 130's map.
        internal static readonly HashSet<int> ValidMet_RS = new()
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
            020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
            040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
            060, 061, 062, 063, 065, 066, 067, 068, 069, 070     , 072, 073, 074, 075, 076, 077, 078, 079, 080,
            081, 082, 083, 085, 086, 087,
        };
        // 155 - 158 Sevii Isle 6-9 Unused
        // 171 - 173 Sevii Isle 22-24 Unused
        internal static readonly HashSet<int> ValidMet_FRLG = new()
        {
            087, 088, 089, 090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
            100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
            120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
            140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 159, 160, 161, 162, 163,
            164, 165, 166, 167, 168, 169, 170, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186,
            187, 188, 189, 190, 191, 192, 193, 194, 195, 196
        };

        internal static readonly HashSet<int> ValidMet_E = new(ValidMet_RS)
        {
            196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
        };

        /// <summary>
        /// Species ID that can be originated from Colosseum (using only Generation 3 max Species ID values).
        /// </summary>
        internal static readonly HashSet<int> ValidSpecies_Colo = new()
        {
            025, // Pikachu
            153, // Bayleef
            154, // Meganium
            156, // Quilava
            157, // Typhlosion
            159, // Croconaw
            160, // Feraligatr
            162, // Furret
            164, // Noctowl
            166, // Ledian
            168, // Ariados
            175, // Togepi
            176, // Togetic
            179, // Mareep
            180, // Flaaffy
            185, // Sudowoodo
            188, // Skiploom
            189, // Jumpluff
            190, // Aipom
            192, // Sunflora
            193, // Yanma
            195, // Quagsire
            196, // Espeon
            197, // Umbreon
            198, // Murkrow
            200, // Misdreavus
            205, // Forretress
            206, // Dunsparce
            207, // Gligar
            210, // Granbull
            211, // Qwilfish
            212, // Scizor
            213, // Shuckle
            214, // Heracross
            215, // Sneasel
            217, // Ursaring
            218, // Slugma
            219, // Magcargo
            221, // Piloswine
            223, // Remoraid
            225, // Delibird
            226, // Mantine
            227, // Skarmory
            229, // Houndoom
            234, // Stantler
            235, // Smeargle
            237, // Hitmontop
            241, // Miltank
            243, // Raikou
            244, // Entei
            245, // Suicune
            248, // Tyranitar
            250, // Ho-Oh
            251, // Celebi
            296, // Makuhita
            297, // Hariyama
            307, // Meditite
            308, // Medicham
            311, // Plusle
            329, // Vibrava
            330, // Flygon
            333, // Swablu
            334, // Altaria
            357, // Tropius
            359, // Absol
            376, // Metagross
        };
    }
}
