using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_7 = 802;
        internal const int MaxMoveID_7 = 719;
        internal const int MaxItemID_7 = 920;
        internal const int MaxAbilityID_7 = 232;
        internal const int MaxBallID_7 = 0x1A; // 26
        internal const int MaxGameID_7 = 41; // Crystal (VC?)

        internal const int MaxSpeciesID_7_USUM = 807;
        internal const int MaxMoveID_7_USUM = 728;
        internal const int MaxItemID_7_USUM = 959;
        internal const int MaxAbilityID_7_USUM = 233;

        #region Met Locations

        internal static readonly int[] Met_SM_0 =
        {
            002, 004, // Invalid
            006, 008, 010, 012, 014, 016, 018, 020, 022, 024, 026, 028, 030, 032, 034, 036, 038, 040, 042, 044, 046, 048,
            050, 052, 054, 056, 058, 060, 062, 064, 066, 068, 070, 072, 074, 076, 078, 082, 084, 086, 088, 090, 092, 094,
            100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 136, 138, 140, 142, 144, 146, 148,
            150, 152, 154, 156, 158, 160, 162, 164, 166, 168, 170, 172, 174, 176, 178, 180, 182, 184, 186, 188, 190, 192,

            194, 195, 196, 197, // Invalid
            198,
            200, 202, 204, 206, 208, 210, 212, 214, 216, 218, 220, 222, 224, 226, 228, 230, 232,
        };

        internal static readonly int[] Met_SM_3 =
        {
            30001, 30003, 30004, 30005, 30006, 30007, 30008, 30009, 30010, 30011, 30012, 30013, 30014, 30015, 30016, 30017
        };

        internal static readonly int[] Met_SM_4 =
        {
            40001, 40002, 40003, 40004, 40005, 40006, 40007, 40008, 40009, 40010,
            40011, 40012, 40013, 40014, 40015, 40016, 40017, 40018, 40019, 40020, 40021, 40022, 40023, 40024, 40025,
            40026, 40027, 40028, 40029, 40030, 40031, 40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039, 40040,
            40041, 40042, 40043, 40044, 40045, 40046, 40047, 40048, 40049, 40050, 40051, 40052, 40053, 40054, 40055,
            40056, 40057, 40058, 40059, 40060, 40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069, 40070,
            40071, 40072, 40073, 40074, 40075, 40076, 40077, 40078, 40079,

            40080, 40081, 40082, 40083, 40084, 40085, 40086, 40087, 40088,
        };

        internal static readonly int[] Met_SM_6 = {/* XY */ 60001, 60003, /* ORAS */ 60004, };

        #endregion

        internal static readonly int[] Tutors_USUM =
        {
            450, 343, 162, 530, 324, 442, 402, 529, 340, 067, 441, 253, 009, 007, 008,
            277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401,      428, 406, 304, 231,
            020, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409,      264, 351, 352,
            380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446,      285,

            477, 502, 432, 710, 707, 675, 673
        };

        internal static readonly ushort[] Pouch_Regular_SM = // 00
        {
            068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087,
            088, 089, 090, 091, 092, 093, 094, 099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 116, 117, 118, 119, 135, 136, 137, 213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225,
            226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245,
            246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265,
            266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285,
            286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305,
            306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325,
            326, 327, 499, 534, 535, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551,
            552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 571, 572, 573, 580, 581, 582, 583,
            584, 585, 586, 587, 588, 589, 590, 639, 640, 644,      646, 647, 648, 649, 650, 656, 657, 658, 659,
            660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679,
            680, 681, 682, 683, 684, 685, 699, 704, 710, 711, 715, 752, 753, 754, 755, 756, 757, 758, 759, 760,
            761, 762, 763, 764, 767, 768, 769, 770, 795, 796, 844, 849, 853, 854, 855, 856, 879, 880, 881, 882,
            883, 884, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920,
        };

        internal static readonly ushort[] Pouch_Ball_SM = { // 08
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 492, 493, 494, 495, 496, 497, 498, 576,
            851
        };

        internal static readonly ushort[] Pouch_Battle_SM = { // 16
            55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 577,
            846,
        };

        internal static readonly ushort[] Pouch_Items_SM = Pouch_Regular_SM.Concat(Pouch_Ball_SM).Concat(Pouch_Battle_SM).ToArray();

        internal static readonly ushort[] Pouch_Key_SM = {
            216, 465, 466, 628, 629, 631, 632, 638,
            705, 706, 765, 773, 797,
            841, 842, 843, 845, 847, 850, 857, 858, 860,
        };

        internal static readonly ushort[] Pouch_Key_USUM = Pouch_Key_SM.Concat(new ushort[] {
            933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948,
            440,
        }).ToArray();

        public static readonly ushort[] Pouch_Roto_USUM = {
            949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959
        };

        internal static readonly ushort[] Pouch_TMHM_SM = { // 02
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345,
            346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363,
            364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381,
            382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399,
            400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
            418, 419, 618, 619, 620, 690, 691, 692, 693, 694,
        };

        internal static readonly ushort[] Pouch_Medicine_SM = { // 32
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 65, 66, 67, 134,
            504, 565, 566, 567, 568, 569, 570, 591, 645, 708, 709,
            852,
        };

        internal static readonly ushort[] Pouch_Berries_SM = {
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
            686, 687, 688,
        };

        internal static readonly ushort[] Pouch_ZCrystal_SM = { // Bead
            807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835,
        };

        internal static readonly ushort[] Pouch_ZCrystalHeld_SM = { // Piece
            776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 798, 799, 800, 801, 802, 803, 804, 805, 806, 836
        };

        internal static readonly ushort[] Pouch_ZCrystal_USUM = Pouch_ZCrystal_SM.Concat(new ushort[] { // Bead
            927, 928, 929, 930, 931, 932
        }).ToArray();

        internal static readonly ushort[] Pouch_ZCrystalHeld_USUM = Pouch_ZCrystalHeld_SM.Concat(new ushort[] { // Piece
            921, 922, 923, 924, 925, 926
        }).ToArray();

        public static readonly Dictionary<int, int> ZCrystalDictionary = Pouch_ZCrystal_USUM
            .Zip(Pouch_ZCrystalHeld_USUM, (k, v) => new KeyValuePair<int, int>(k, v))
            .ToDictionary(x => x.Key, x => x.Value);

        internal static readonly ushort[] HeldItems_SM = ArrayUtil.ConcatAll(Pouch_Items_SM, Pouch_Berries_SM, Pouch_Medicine_SM, Pouch_ZCrystalHeld_SM);
        internal static readonly ushort[] HeldItems_USUM = ArrayUtil.ConcatAll(Pouch_Items_SM, Pouch_Berries_SM, Pouch_Medicine_SM, Pouch_ZCrystalHeld_USUM, Pouch_Roto_USUM);

        internal static readonly HashSet<int> WildPokeballs7 = new HashSet<int> {
            0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, // Johto Balls
            0x1A, // Beast
        };

        internal static readonly HashSet<int> AlolanOriginForms = new HashSet<int>
        {
            019, // Rattata
            020, // Raticate
            027, // Sandshrew
            028, // Sandslash
            037, // Vulpix
            038, // Ninetales
            050, // Diglett
            051, // Dugtrio
            052, // Meowth
            053, // Persian
            074, // Geodude
            075, // Graveler
            076, // Golem
            088, // Grimer
            089, // Muk
        };

        internal static readonly HashSet<int> AlolanVariantEvolutions12 = new HashSet<int>
        {
            026, // Raichu
            103, // Exeggutor
            105, // Marowak
        };

        internal static readonly HashSet<int> EvolveToAlolanForms = new HashSet<int>(AlolanVariantEvolutions12.Concat(AlolanOriginForms));

        public static readonly HashSet<int> PastGenAlolanNatives = new HashSet<int>
        {
            010, 011, 012, 019, 020, 021, 022, 025, 026, 027, 028, 035, 036, 037, 038, 039, 040, 041, 042, 046, 047, 050,
            051, 052, 053, 054, 055, 056, 057, 058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 068, 072, 073, 074, 075,
            076, 079, 080, 081, 082, 088, 089, 090, 091, 092, 093, 094, 096, 097, 102, 103, 104, 105, 113, 115, 118, 119,
            120, 121, 123, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 142, 143, 147, 148, 149, 165,
            166, 167, 168, 169, 170, 171, 172, 173, 174, 185, 186, 196, 197, 198, 199, 200, 209, 210, 212, 215, 222, 225,
            227, 233, 235, 239, 240, 241, 242, 278, 279, 283, 284, 296, 297, 299, 302, 318, 319, 320, 321, 324, 327, 328,
            329, 330, 339, 340, 349, 350, 351, 359, 361, 362, 369, 370, 371, 372, 373, 374, 375, 376, 408, 409, 410, 411,
            422, 423, 425, 426, 429, 430, 438, 440, 443, 444, 445, 446, 447, 448, 456, 457, 461, 462, 466, 467, 470, 471,
            474, 476, 478, 506, 507, 508, 524, 525, 526, 546, 547, 548, 549, 551, 552, 553, 564, 565, 566, 567, 568, 569,
            582, 583, 584, 587, 594, 627, 628, 629, 630, 661, 662, 663, 674, 675, 700, 703, 704, 705, 706, 707, 708, 709,
            718,

            // Regular
            023, 086, 108, 122, 138, 140, 163, 177, 179, 190, 204,
            206, 214, 223, 228, 238, 246, 303, 309, 341, 343,
            345, 347, 352, 353, 357, 366, 427, 439, 458, 550,
            559, 570, 572, 592, 605, 619, 621, 622, 624, 636,
            667, 669, 676, 686, 690, 692, 696, 698, 701, 702,
            714,

            // Wormhole
            333, 334, // Altaria
            193, 469, // Yanmega
            561, // Sigilyph
            580, 581, // Swanna
            276, 277, // Swellow
            451, 452, // Drapion
            531, // Audino
            694, 695, // Heliolisk
            273, 274, 275, // Nuzleaf
            325, 326, // Gumpig
            459, 460, // Abomasnow
            307, 308, // Medicham
            449, 450, // Hippowdon
            557, 558, // Crustle
            218, 219, // Magcargo
            688, 689, // Barbaracle
            270, 271, 272, // Lombre
            618, // Stunfisk
            418, 419, // Floatzel
            194, 195, // Quagsire

            100, 101 // Voltorb & Electrode
        };

        public static readonly HashSet<int> PastGenAlolanNativesUncapturable = new HashSet<int>
        {
            142, // Aerodacyl
            137, 233, 474, // Porygon++
            138, 139, 140, 141,  // Gen1 Fossils
            345, 346, 347, 348,  // Gen3 Fossils
            408, 409, 410, 411,  // Gen4 Fossils
            564, 565, 566, 567,  // Gen5 Fossils
            696, 697, 698, 699,  // Gen6 Fossils
        };

        internal static readonly HashSet<int> PastGenAlolanScans = new HashSet<int>
        {
            069, // Bellsprout
            111, // Rhyhorn
            116, // Horsea
            152, // Chikorita
            155, // Cyndaquil
            158, // Totodile
            220, // Swinub
            363, // Spheal
            543, // Venipede
            574, // Gothita
            599, // Klink
            607, // Litwick
            610, // Axew
            633, // Deino
            679, // Honedge

            183, 298, // Marill {Azurill}
            315, 406, // Roselia {Budew}

            175, // [468] Togekiss (Togepi)
            287, // [288] Vigoroth (Slakoth)
            396, // [397] Staravia (Starly)
            403, // [404] Luxio (Shinx)
            495, // [497] Serperior (Snivy)
            577, // [578] Duosion (Solosis)

            498, // [500] Emboar (Tepig)
            501, // [503] Samurott (Oshawott)
            532, // [534] Conkeldurr (Timburr)
            540, // [542] Leavanny (Sewaddle)
            602, // [604] Eelektross (Tynamo)

            004, // Charmander
            007, // Squirtle
            095, // Onix
            663, 664, // Scatterbug
            001, // Bulbasaur
            280, // Ralts
            255, 256, // Combusken
            013, 014, 015, // Beedrill
            252, 253, // Grovyle
            258, 259, // Marshtomp
            393, 394, // Prinplup
            387, 388, // Grotle
            016, 017, 018, // Pidgeot
            389, 390, 391, // Monferno
            304, 305, 306, // Aggron
            479, // Rotom
            650, 651, 652, // Chesnaught
            656, 657, 658, // Greninja
            653, 654, 655, // Delphox
        };

        internal static readonly HashSet<int> Inherit_Apricorn6 = new HashSet<int>
        {
            010, 013, 016, 019, 021, 023, 025, 027, 029, 035, 037, 039, 041,
            043, 046, 048, 050, 052, 054, 056, 058, 060, 063, 066, 069, 072, 074, 077, 079, 083, 084, 086, 088, 090, 092,
            095, 096, 098, 102, 104, 108, 109, 111, 113, 114, 115, 116, 118, 122, 124, 125, 126, 129, 131, 143, 147, 161,
            163, 165, 167, 170, 177, 179, 183, 185, 187, 190, 191, 193, 194, 198, 200, 202, 203, 204, 206, 207, 209, 211,
            213, 214, 215, 216, 218, 220, 222, 223, 225, 226, 227, 228, 231, 234, 235, 241, 246, 261, 263, 265, 273, 276,
            278, 280, 285, 287, 293, 296, 302, 303, 307, 311, 312, 316, 322, 325, 327, 333, 339, 359, 366, 369, 370, 396,
            399, 401, 403, 406, 412, 415, 418, 420, 427, 433, 441, 455,

            032, // Via Nidoran-F

            440, // Via Chansey
            238, // Via Jynx
            239, // Via Electabuzz
            240, // Via Magmar
            298, // Via Marill
            360, // Via Wobbuffet
            438, // Via Sudowoodo
            439, // Via Mr. Mime
            446, // Via Snorlax
            458, // Via Mantine
            358, // Via Chingling
            172, // Via Pikachu
            173, // Via Clefairy
            174, // Via Jigglypuff
        };

        internal static readonly HashSet<int> AlolanCaptureOffspring = new HashSet<int>
        {
            010, 019, 021, 025, 027, 035, 037, 039, 041, 046,
            050, 052, 054, 056, 058, 060, 063, 066, 072, 074,
            079, 081, 088, 090, 092, 096, 102, 104, 113, 115,
            118, 120, 123, 127, 128, 129, 131, 132, 133, 143,
            147, 165, 167, 170, 172, 173, 174, 185, 198, 200,
            209, 212, 215, 222, 225, 227, 235, 239, 240, 241,
            278, 283, 296, 299, 302, 318, 320, 324, 327, 328,
            339, 349, 351, 359, 361, 369, 370, 371, 374, 422,
            425, 438, 440, 443, 446, 447, 456, 506, 524, 546,
            548, 551, 568, 582, 587, 594, 627, 629, 661, 674,
            703, 704, 707, 708,

            731, 734, 736, 739, 741, 742, 744, 746, 747, 749,
            751, 753, 755, 757, 759, 761, 764, 765, 766, 767,
            769, 771, 774, 775, 776, 777, 778, 779, 780, 781,
            782,

            // USUM Additions
            023, 086, 108, 122, 163, 177, 179, 190, 204,
            206, 214, 223, 226, 228, 238, 246, 303, 309, 341, 343,
            352, 353, 357, 366, 427, 439, 458, 550,
            559, 570, 572, 592, 605, 619, 621, 622, 624, 636,
            667, 669, 676, 686, 690, 692, 701, 702,
            714,

            // Wormhole
            333, 193, 561, 580, 276, 451, 531, 694, 273, 325,
            459, 307, 449, 557, 218, 688, 270, 618, 418, 194,

            // Static Encounters
            100,
        };

        internal static readonly HashSet<int> AlolanCaptureNoHeavyBall = new HashSet<int> { 374, 785, 786, 787, 788}; // Beldum & Tapus

        internal static readonly HashSet<int> Inherit_ApricornMale7 = new HashSet<int>
        {
            100, // Voltorb
            343, // Baltoy
            436, // Bronzor

            // Others are capturable in the Alola region
            // Magnemite, Staryu, Tauros
        };

        internal static readonly HashSet<int> Inherit_Apricorn7 = new HashSet<int> (Inherit_Apricorn6.Concat(Inherit_ApricornMale7).Concat(PastGenAlolanScans).Concat(AlolanCaptureOffspring).Distinct());

        internal static readonly HashSet<int> Inherit_SafariMale = new HashSet<int>
        {
            128, // Tauros

            081, // Magnemite
            100, // Voltorb
            337, // Lunatone
            338, // Solrock
            374, // Beldum
            436, // Bronzor
        };

        internal static readonly HashSet<int> Inherit_DreamMale = new HashSet<int>
        {
            // Starting with Gen7, Males pass Ball via breeding with Ditto.
            001, 004, 007, // Gen1 Starters
            025, // Pikachu
            128, // Tauros
            172, // Pichu
            236, // Tyrogue (100% Male)
            252, 255, 258, // Gen2 Starters
            387, 390, 393, // Gen3 Starters
            511, 513, 515, // Gen5 Monkeys
            538, // Throh
            539, // Sawk
            574, // Gothita

            081, // Magnemite
            100, // Voltorb
            120, // Staryu
            137, // Porygon
            337, // Lunatone
            338, // Solrock
            343, // Baltoy
            374, // Beldum
            436, // Bronzor
            479, // Rotom
            599, // Klink
            622, // Golett
        };

        internal static readonly HashSet<int> Ban_Gen3Ball_7 = new HashSet<int>
        {
            489, // Phione
            566, 567, 696, 697, 698, 699 // Fossil Only obtain
        };

        internal static readonly HashSet<int> Ban_Gen4Ball_7 = new HashSet<int>
        {
            489, // Phione
            566, 567, 696, 697, 698, 699 // Fossil Only obtain
        };

        internal static readonly HashSet<int> Ban_SafariBallHidden_7 = new HashSet<int>
        {
            029, 030, 031, 032, 033, 034, // Nidos
            313, 314, // Volbeat/Illumise

            081, // Magnemite
            100, // Voltorb
            115, // Kangaskhan
            128, // Tauros
            132, // Ditto
            241, // Miltank
            374, // Beldum
            436, // Bronzor
            440, // Happiny

            // others not possible
            236, // Tyrogue (100% Male)
            120, // Staryu
            337, // Lunatone
            338, // Solrock
            479, // Rotom
            599, // Klink
            622, // Golett
        };

        internal static readonly int[] ZygardeMoves =
        {
            245, // Extreme Speed
            349, // Dragon Dance
            614, // Thousand Arrows
            615, // Thousand Waves
            687, // Core Enforcer
        };

        internal static readonly HashSet<int> Totem_Alolan = new HashSet<int>
        {
            020, // Raticate (Normal, Alolan, Totem)
            105, // Marowak (Normal, Alolan, Totem)
            778, // Mimikyu (Normal, Busted, Totem, Totem_Busted)
        };

        internal static readonly HashSet<int> Totem_NoTransfer = new HashSet<int>
        {
            752, // Araquanid
            777, // Togedemaru
            743, // Ribombee
        };

        internal static readonly HashSet<int> Totem_USUM = new HashSet<int>
        {
            020, // Raticate
            735, // Gumshoos
            //746, // Wishiwashi
            758, // Salazzle
            754, // Lurantis
            738, // Vikavolt
            778, // Mimikyu
            784, // Kommo-o
            105, // Marowak
            752, // Araquanid
            777, // Togedemaru
            743, // Ribombee
        };

        internal static readonly int[] EggLocations7 = {Locations.Daycare5, Locations.LinkTrade6};

        internal static readonly HashSet<int> ValidMet_SM = new HashSet<int>
        {
            006, 008, 010, 012, 014, 016, 018, 020, 022, 024, 026, 028, 030, 032, 034, 036, 038, 040, 042, 044, 046, 048,
            050, 052, 054, 056, 058, 060, 062, 064, 066, 068, 070, 072, 074, 076, 078, 082, 084, 086, 088, 090, 092, 094,
            100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 136, 138, 140, 142, 144, 146, 148,
            150, 152, 154, 156, 158, 160, 162, 164, 166, 168, 170, 172, 174, 176, 178, 180, 182, 184, 186, 188, 190, 192,

            30016 // Poké Pelago
        };

        internal static readonly HashSet<int> ValidMet_USUM = new HashSet<int>(ValidMet_SM)
        {
            // 194, 195, 196, 197, // Unobtainable new Locations
            198,
            200, 202, 204, 206, 208, 210, 212, 214, 216, 218, 220, 222, 224, 226, 228, 230, 232,
        };

        internal static readonly int[] TMHM_SM =
        {
            526, 337, 473, 347, 046, 092, 258, 339, 474, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 355, 219,
            218, 076, 479, 085, 087, 089, 216, 141, 094, 247,
            280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
            259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
            211, 411, 412, 206, 503, 374, 451, 507, 693, 511,
            261, 512, 373, 153, 421, 371, 684, 416, 397, 694,
            444, 521, 086, 360, 014, 019, 244, 523, 524, 157,
            404, 525, 611, 398, 138, 447, 207, 214, 369, 164,
            430, 433, 528, 057, 555, 267, 399, 127, 605, 590,

            // No HMs
        };

        internal static readonly byte[] MovePP_SM =
        {
            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 20, 30, 35, 35, 20, 15, 20, 20, 25, 20, 30, 05, 10, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 10, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 20, 20, 20, 20, 15, 25, 15, 10, 20, 25, 10, 35, 30, 15, 10, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 10, 30, 10, 20, 10, 40, 40, 20, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 15, 20, 10, 15, 35, 20, 15, 10, 10, 30, 15, 40, 20, 10, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            20, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 25, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 10,
            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 10, 15, 15,
            10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15,
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 15, 05, 40, 15, 20, 20, 05, 15, 20, 20, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 25, 15, 20, 15, 20, 15, 20, 10,
            20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 10, 10, 15, 10, 20, 15, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15,
            10, 15, 15, 15, 10, 10, 10, 20, 10, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20,
            10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 10, 05, 05, 10, 05, 05, 05, 15, 10, 10, 10, 10, 10, 10, 15, 20, 15, 10, 15, 10, 15, 10, 20, 10, 10, 10, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 15, 20, 15, 10,
            15, 15, 15, 15, 10, 10, 10, 10, 10, 15, 15, 15, 15, 05, 05, 15, 05, 10, 10, 10, 20, 20, 20, 10, 10, 30, 15, 15, 10, 15, 25, 10, 15, 10, 10, 10, 20, 10, 10, 10, 10, 10, 15, 15, 05, 05, 10, 10, 10, 05,
            05, 10, 05, 05, 15, 10, 05, 05, 05, 10, 10, 10, 10, 20, 25, 10, 20, 30, 25, 20, 20, 15, 20, 15, 20, 20, 10, 10, 10, 10, 10, 20, 10, 30, 15, 10, 10, 10, 20, 20, 05, 05, 05, 20, 10, 10, 20, 15, 20, 20,
            10, 20, 30, 10, 10, 40, 40, 30, 20, 40, 20, 20, 10, 10, 10, 10, 05, 10, 10, 05, 05, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01,
            01, 01, 01, 01, 01, 01, 01, 01, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 40, 15, 20, 30, 20, 15, 15, 20, 10, 15, 15, 10, 05, 10, 10, 20, 15, 10, 15, 15, 15, 05, 15, 20, 20, 01, 01, 01, 01, 01, 01,
            01, 01, 01, 05, 05, 10, 10, 10, 20, 10, 10, 10, 05, 05, 20, 10, 10, 10, 01, 05, 15, 05, 01, 01, 01, 01, 01, 01,
        };

        internal static readonly HashSet<int> Ban_NoHidden7 = new HashSet<int>
        {
            // SOS slots have 0 call rate
            767, // Wimpod
            768, // Golisopod

            // No Encounter
            774, // Minior

            //Pre-Gen
            710 + (1 << 11), //Pumpkaboo-Small
            711 + (1 << 11), //Gourgeist-Small
            710 + (2 << 11), //Pumpkaboo-Large
            711 + (2 << 11), //Gourgeist-Large
        };

        internal static readonly HashSet<int> Ban_NoHidden7Apricorn = new HashSet<int>
        {
            029, // Nidoran
            032, // Nidoran
            100, // Voltorb
            436, // Bronzor
            669 + (3 << 11), // Flabébé-Blue
        };

        #region Unreleased Items
        internal static readonly HashSet<int> UnreleasedHeldItems_7 = new HashSet<int>
        {
            005, // Safari Ball
            016, // Cherish Ball
            064, // Fluffy Tail
            065, // Blue Flute
            066, // Yellow Flute
            067, // Red Flute
            068, // Black Flute
            069, // White Flute
            070, // Shoal Salt
            071, // Shoal Shell
            103, // Old Amber
            111, // Odd Keystone
            164, // Razz Berry
            166, // Nanab Berry
            167, // Wepear Berry
            175, // Cornn Berry
            176, // Magost Berry
            177, // Rabuta Berry
            178, // Nomel Berry
            179, // Spelon Berry
            180, // Pamtre Berry
            181, // Watmel Berry
            182, // Durin Berry
            183, // Belue Berry
            //208, // Enigma Berry
            //209, // Micle Berry
            //210, // Custap Berry
            //211, // Jaboca Berry
            //212, // Rowap Berry
            215, // Macho Brace
            260, // Red Scarf
            261, // Blue Scarf
            262, // Pink Scarf
            263, // Green Scarf
            264, // Yellow Scarf
            499, // Sport Ball
            548, // Fire Gem
            549, // Water Gem
            550, // Electric Gem
            551, // Grass Gem
            552, // Ice Gem
            553, // Fighting Gem
            554, // Poison Gem
            555, // Ground Gem
            556, // Flying Gem
            557, // Psychic Gem
            558, // Bug Gem
            559, // Rock Gem
            560, // Ghost Gem
            561, // Dragon Gem
            562, // Dark Gem
            563, // Steel Gem
            576, // Dream Ball
            584, // Relic Copper
            585, // Relic Silver
            587, // Relic Vase
            588, // Relic Band
            589, // Relic Statue
            590, // Relic Crown
            715, // Fairy Gem
        };
        #endregion
        internal static readonly bool[] ReleasedHeldItems_7 = Enumerable.Range(0, MaxItemID_7_USUM+1).Select(i => HeldItems_USUM.Contains((ushort)i) && !UnreleasedHeldItems_7.Contains(i)).ToArray();
    }
}
