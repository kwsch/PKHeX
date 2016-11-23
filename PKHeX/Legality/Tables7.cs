using System.Linq;

namespace PKHeX
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_7 = 802;
        // PKHeX Valid Array Storage

        #region Met Locations

        internal static readonly int[] Met_SMc = { 0, 60002, 30002, };

        internal static readonly int[] Met_SM_0 =
        {
            002, 004, // Invalid
            006, 008, 010, 012, 014, 016, 018, 020, 022, 024, 026, 028, 030, 032, 034, 036, 038, 040, 042, 044, 046, 048,
            050, 052, 054, 056, 058, 060, 062, 064, 066, 068, 070, 072, 074, 076, 078, 082, 084, 086, 088, 090, 092, 094,
            100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 136, 138, 140, 142, 144, 146, 148,
            150, 152, 154, 156, 158, 160, 162, 164, 166, 168, 170, 172, 174, 176, 178, 180, 182, 184, 186, 188, 190, 192
        };

        internal static readonly int[] Met_SM_3 =
        {
            30001, 30003, 30004, 30005, 30006, 30007, 30008, 30009, 30010, 30011, 30012, 30013, 30014, 30015, 30016,
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
            584, 585, 586, 587, 588, 589, 590, 639, 640, 644, 645, 646, 647, 648, 649, 650, 656, 657, 658, 659,
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
            216, 465, 466, 628, 629, 631, 632, 633, 638, 696,
            705, 706, 765, 773, 797,
            841, 842, 843, 845, 847, 850, 857, 858, 860,
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
            504, 565, 566, 567, 568, 569, 570, 591, 708, 709,
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
        internal static readonly ushort[] HeldItems_SM = new ushort[1].Concat(Pouch_Items_SM).Concat(Pouch_Berries_SM).Concat(Pouch_Medicine_SM).Concat(Pouch_ZCrystalHeld_SM).ToArray();

        #region Encounters
        private static readonly EncounterStatic[] Encounter_SM = // @ a\1\5\5
        {
            // Gifts - 0.bin
            new EncounterStatic { Gift = true, Species = 722, Level = 5,  Location = 24, }, // Rowlet
            new EncounterStatic { Gift = true, Species = 725, Level = 5,  Location = 24, }, // Litten
            new EncounterStatic { Gift = true, Species = 728, Level = 5,  Location = 24, }, // Popplio
            new EncounterStatic { Gift = true, Species = 138, Level = 15, Location = 58, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 15, Location = 58, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 15, Location = 58, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 15, Location = 58, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 15, Location = 58, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 15, Location = 58, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 15, Location = 58, }, // Shieldon
            new EncounterStatic { Gift = true, Species = 564, Level = 15, Location = 58, }, // Tirtouga
            new EncounterStatic { Gift = true, Species = 566, Level = 15, Location = 58, }, // Archen
            new EncounterStatic { Gift = true, Species = 696, Level = 15, Location = 58, }, // Tyrunt
            new EncounterStatic { Gift = true, Species = 698, Level = 15, Location = 58, }, // Amaura
            new EncounterStatic { Gift = true, Species = 133, Level = 1,  EggLocation = 60002, }, // Eevee @ Nursery helpers
            new EncounterStatic { Gift = true, Species = 137, Level = 30, Location = 116, }, // Porygon @ Route 15
            new EncounterStatic { Gift = true, Species = 772, Level = 40, Location = 188, IV3 = true, }, // Type: Null
            new EncounterStatic { Gift = true, Species = 789, Level = 5,  Location = 142, Shiny = false, IV3 = true, Version = GameVersion.SN}, // Cosmog                00 FF
            new EncounterStatic { Gift = true, Species = 789, Level = 5,  Location = 144, Shiny = false, IV3 = true, Version = GameVersion.MN}, // Cosmog                00 FF
            new EncounterStatic { Gift = true, Species = 142, Level = 40, Location = 172, }, // Aerodactyl @ Seafolk Village
            
            new EncounterStatic // Magearna (Bottle Cap) 00 FF
            {
                Gift = true, Species = 801, Level = 50, Location = 40001, Shiny = false, IV3 = true,
                Fateful = true, RibbonWishing = true, Relearn = new [] {705, 430, 381, 270}, Ball = 0x10, // Cherish
            },

            // Static Encounters - 1.bin
            new EncounterStatic { Species = 731, Form = 0, Level = 03, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = -1, }, // Pikipek
            // new EncounterStatic { Species = 793, Form = 0, Level = 27, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, IVs = new[] {31, 01, 31, 01, 31, 31}, Location = -1, IV3 = true, }, // Nihilego
            new EncounterStatic { Species = 791, Form = 0, Level = 55, Relearn = new[]{713, 322, 242, 428}, Shiny = false, Ability = 1, Location = 176, IV3 = true, Version = GameVersion.SN}, // Solgaleo
            new EncounterStatic { Species = 792, Form = 0, Level = 55, Relearn = new[]{714, 322, 539, 247}, Shiny = false, Ability = 1, Location = 178, IV3 = true, Version = GameVersion.MN}, // Lunala
            new EncounterStatic { Species = 735, Form = 1, Level = 12, Relearn = new[]{162, 044, 043, 184}, Shiny = false, Ability = 4, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, Gender = 0, HeldItem = 151, }, // Gumshoos-1
            new EncounterStatic { Species = 734, Form = 0, Level = 11, Relearn = new[]{028, 033, 158, 043}, Shiny = false, Ability = 2, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Yungoos
            new EncounterStatic { Species = 735, Form = 0, Level = 11, Relearn = new[]{028, 044, 162, 043}, Shiny = false, Ability = 4, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Gumshoos
            new EncounterStatic { Species = 734, Form = 0, Level = 11, Relearn = new[]{028, 033, 162, 043}, Shiny = false, Ability = 2, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Yungoos
            new EncounterStatic { Species = 734, Form = 0, Level = 10, Relearn = new[]{043, 033, 228, 117}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Yungoos
            new EncounterStatic { Species = 020, Form = 2, Level = 12, Relearn = new[]{033, 044, 039, 184}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, Gender = 0, HeldItem = 151, }, // Raticate-2
            new EncounterStatic { Species = 019, Form = 1, Level = 11, Relearn = new[]{033, 039, 098, 158}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Rattata-1
            new EncounterStatic { Species = 020, Form = 1, Level = 11, Relearn = new[]{116, 039, 033, 162}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Raticate-1
            new EncounterStatic { Species = 019, Form = 1, Level = 11, Relearn = new[]{033, 039, 098, 162}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Rattata-1
            new EncounterStatic { Species = 019, Form = 1, Level = 10, Relearn = new[]{033, 039, 098, 116}, Shiny = false, Ability = 2, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Rattata-1
            new EncounterStatic { Species = 746, Form = 1, Level = 20, Relearn = new[]{055, 045, 240, 487}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, Gender = 0, HeldItem = 158, }, // Wishiwashi-1
            new EncounterStatic { Species = 746, Form = 0, Level = 17, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 86, }, // Wishiwashi
            new EncounterStatic { Species = 746, Form = 0, Level = 18, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 86, }, // Wishiwashi
            new EncounterStatic { Species = 594, Form = 0, Level = 18, Relearn = new[]{270, 003, 505, 055}, Shiny = false, Ability = 1, IVs = new[] {00, 00, 00, 00, 00, 00}, Location = -1, }, // Alomomola
            new EncounterStatic { Species = 746, Form = 0, Level = 18, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 86, }, // Wishiwashi
            new EncounterStatic { Species = 758, Form = 1, Level = 22, Relearn = new[]{259, 599, 092, 481}, Shiny = false, Ability = 1, IVs = new[] {31, 01, 15, 01, 15, 01}, Location = -1, Gender = 1, HeldItem = 204, }, // Salazzle-1
            new EncounterStatic { Species = 105, Form = 1, Level = 18, Relearn = new[]{172, 125, 039, 045}, Shiny = false, Ability = 1, Location = -1, }, // Marowak-1
            new EncounterStatic { Species = 105, Form = 1, Level = 18, Relearn = new[]{172, 125, 039, 045}, Shiny = false, Ability = 1, Location = -1, }, // Marowak-1
            new EncounterStatic { Species = 025, Form = 0, Level = 20, Relearn = new[]{486, 086, 098, 589}, Shiny = false, Ability = 1, Location = -1, }, // Pikachu
            new EncounterStatic { Species = 757, Form = 0, Level = 20, Relearn = new[]{139, 474, 269, 010}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, Gender = 0, }, // Salandit
            new EncounterStatic { Species = 754, Form = 1, Level = 24, Relearn = new[]{075, 404, 669, 235}, Shiny = false, Ability = 1, IVs = new[] {31, 31, 31, 20, 31, 31}, Location = -1, Gender = 1, HeldItem = 271, }, // Lurantis-1
            new EncounterStatic { Species = 047, Form = 0, Level = 22, Relearn = new[]{440, 141, 210, 147}, Shiny = false, Ability = 2, Location = -1, }, // Parasect
            new EncounterStatic { Species = 756, Form = 0, Level = 22, Relearn = new[]{310, 275, 072, 079}, Shiny = false, Ability = 1, Location = -1, }, // Shiinotic
            new EncounterStatic { Species = 753, Form = 0, Level = 23, Relearn = new[]{075, 670, 210, 074}, Shiny = false, Ability = 1, Location = -1, }, // Fomantis
            new EncounterStatic { Species = 753, Form = 0, Level = 23, Relearn = new[]{075, 670, 210, 074}, Shiny = false, Ability = 1, Location = -1, }, // Fomantis
            new EncounterStatic { Species = 753, Form = 0, Level = 23, Relearn = new[]{670, 210, 275, 075}, Shiny = false, Ability = 1, Location = -1, }, // Fomantis
            new EncounterStatic { Species = 753, Form = 0, Level = 23, Relearn = new[]{670, 210, 275, 075}, Shiny = false, Ability = 1, Location = -1, }, // Fomantis
            new EncounterStatic { Species = 351, Form = 0, Level = 22, Relearn = new[]{241, 055, 029, 311}, Shiny = false, Ability = 1, IVs = new[] {30, 15, 30, 30, 30, 15}, Location = -1, }, // Castform
            new EncounterStatic { Species = 732, Form = 0, Level = 22, Relearn = new[]{350, 365, 048, 103}, Shiny = false, Ability = 1, IVs = new[] {10, 10, 30, 10, 30, 10}, Location = -1, }, // Trumbeak
            new EncounterStatic { Species = 738, Form = 1, Level = 29, Relearn = new[]{011, 268, 450, 209}, Shiny = false, Ability = 1, IVs = new[] {31, 01, 31, 01, 31, 01}, Location = -1, Gender = 0, HeldItem = 184, }, // Vikavolt-1
            new EncounterStatic { Species = 736, Form = 0, Level = 27, Relearn = new[]{011, 081, 044, 450}, Shiny = false, Ability = 1, Location = -1, }, // Grubbin
            new EncounterStatic { Species = 737, Form = 0, Level = 27, Relearn = new[]{268, 011, 450, 209}, Shiny = false, Ability = 1, Location = -1, }, // Charjabug
            new EncounterStatic { Species = 737, Form = 0, Level = 27, Relearn = new[]{268, 189, 512, 209}, Shiny = false, Ability = 1, Location = -1, }, // Charjabug
            new EncounterStatic { Species = 737, Form = 0, Level = 27, Relearn = new[]{081, 011, 189, 086}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Charjabug
            new EncounterStatic { Species = 737, Form = 0, Level = 28, Relearn = new[]{044, 086, 189, 209}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Charjabug
            new EncounterStatic { Species = 778, Form = 2, Level = 33, Relearn = new[]{421, 583, 310, 102}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, Gender = 1, HeldItem = 157, }, // Mimikyu-2
            new EncounterStatic { Species = 092, Form = 0, Level = 30, Relearn = new[]{174, 109, 212, 095}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Gastly
            new EncounterStatic { Species = 093, Form = 0, Level = 30, Relearn = new[]{122, 101, 389, 095}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Haunter
            new EncounterStatic { Species = 094, Form = 0, Level = 30, Relearn = new[]{101, 325, 247, 095}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Gengar
            new EncounterStatic { Species = 093, Form = 0, Level = 27, Relearn = new[]{122, 101, 389, 095}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Haunter
            new EncounterStatic { Species = 094, Form = 0, Level = 27, Relearn = new[]{101, 325, 247, 095}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Gengar
            new EncounterStatic { Species = 784, Form = 1, Level = 45, Relearn = new[]{691, 327, 430, 182}, Shiny = false, Ability = 2, IVs = new[] {31, 01, 31, 01, 31, 01}, Location = -1, Gender = 0, HeldItem = 219, }, // Kommo-o-1
            new EncounterStatic { Species = 782, Form = 0, Level = 40, Relearn = new[]{117, 033, 043, 029}, Shiny = false, Ability = 1, IVs = new[] {31, 31, 31, 31, 31, 31}, Location = -1, }, // Jangmo-o
            new EncounterStatic { Species = 783, Form = 0, Level = 40, Relearn = new[]{029, 475, 526, 327}, Shiny = false, Ability = 1, IVs = new[] {31, 31, 31, 31, 31, 31}, Location = -1, }, // Hakamo-o
            new EncounterStatic { Species = 783, Form = 0, Level = 32, Relearn = new[]{327, 475, 526, 117}, Shiny = false, Ability = 1, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Hakamo-o
            new EncounterStatic { Species = 212, Form = 0, Level = 32, Relearn = new[]{232, 228, 043, 210}, Shiny = false, Ability = 2, IVs = new[] {15, 15, 15, 15, 15, 15}, Location = -1, }, // Scizor
            new EncounterStatic { Species = 793, Form = 0, Level = 55, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 082, IV3 = true, }, // Nihilego @ Wela Volcano Park
            new EncounterStatic { Species = 793, Form = 0, Level = 55, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 100, IV3 = true, }, // Nihilego @ Diglett’s Tunnel
            new EncounterStatic { Species = 794, Form = 0, Level = 65, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 040, IV3 = true, Version = GameVersion.SN }, // Buzzwole @ Melemele Meadow
            new EncounterStatic { Species = 795, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 046, IV3 = true, Version = GameVersion.MN }, // Pheromosa @ Verdant Cavern (Trial Site)
            new EncounterStatic { Species = 796, Form = 0, Level = 65, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 090, IV3 = true, }, // Xurkitree @ Lush Jungle
            new EncounterStatic { Species = 796, Form = 0, Level = 65, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 076, IV3 = true, }, // Xurkitree @ Memorial Hill
            new EncounterStatic { Species = 798, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 134, IV3 = true, Version = GameVersion.SN }, // Kartana @ Malie Garden
            new EncounterStatic { Species = 798, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 120, IV3 = true, Version = GameVersion.SN }, // Kartana @ Route 17
            new EncounterStatic { Species = 797, Form = 0, Level = 65, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 124, IV3 = true, Version = GameVersion.MN }, // Celesteela @ Haina Desert
            new EncounterStatic { Species = 797, Form = 0, Level = 65, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 134, IV3 = true, Version = GameVersion.MN }, // Celesteela @ Malie Garden
            new EncounterStatic { Species = 799, Form = 0, Level = 70, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 182, IV3 = true, }, // Guzzlord @ Resolution Cave
            new EncounterStatic { Species = 800, Form = 0, Level = 75, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 036, IV3 = true, }, // Necrozma @ Ten Carat Hill (Farthest Hollow)
            new EncounterStatic { Species = 155, Form = 0, Level = 12, Relearn = new[]{024, 052, 108, 043}, Location = -1, }, // Cyndaquil
            new EncounterStatic { Species = 158, Form = 0, Level = 12, Relearn = new[]{232, 099, 055, 043}, Location = -1, }, // Totodile
            new EncounterStatic { Species = 633, Form = 0, Level = 13, Relearn = new[]{372, 029, 044, 116}, Location = -1, }, // Deino
            new EncounterStatic { Species = 116, Form = 0, Level = 18, Relearn = new[]{225, 239, 055, 043}, Location = -1, }, // Horsea
            new EncounterStatic { Species = 599, Form = 0, Level = 08, Relearn = new[]{268, 011, 000, 000}, Location = -1, }, // Klink
            new EncounterStatic { Species = 152, Form = 0, Level = 10, Relearn = new[]{073, 077, 075, 045}, Location = -1, }, // Chikorita
            new EncounterStatic { Species = 607, Form = 0, Level = 10, Relearn = new[]{051, 109, 083, 123}, Location = -1, }, // Litwick
            new EncounterStatic { Species = 574, Form = 0, Level = 17, Relearn = new[]{399, 060, 003, 313}, Location = -1, }, // Gothita
            new EncounterStatic { Species = 363, Form = 0, Level = 19, Relearn = new[]{392, 362, 301, 227}, Location = -1, }, // Spheal
            new EncounterStatic { Species = 404, Form = 0, Level = 20, Relearn = new[]{598, 044, 209, 268}, Location = -1, }, // Luxio
            new EncounterStatic { Species = 679, Form = 0, Level = 23, Relearn = new[]{194, 332, 425, 475}, Location = -1, }, // Honedge
            new EncounterStatic { Species = 543, Form = 0, Level = 14, Relearn = new[]{390, 228, 103, 040}, Location = -1, }, // Venipede
            new EncounterStatic { Species = 069, Form = 0, Level = 16, Relearn = new[]{491, 077, 079, 035}, Location = -1, }, // Bellsprout
            new EncounterStatic { Species = 183, Form = 0, Level = 17, Relearn = new[]{453, 270, 061, 205}, Location = -1, }, // Marill
            new EncounterStatic { Species = 111, Form = 0, Level = 30, Relearn = new[]{130, 350, 498, 523}, Location = -1, }, // Rhyhorn
            new EncounterStatic { Species = 220, Form = 0, Level = 31, Relearn = new[]{573, 036, 420, 196}, Location = -1, }, // Swinub
            new EncounterStatic { Species = 578, Form = 0, Level = 33, Relearn = new[]{101, 248, 283, 473}, Location = -1, }, // Duosion
            new EncounterStatic { Species = 315, Form = 0, Level = 34, Relearn = new[]{437, 275, 230, 390}, Location = -1, }, // Roselia
            new EncounterStatic { Species = 315, Form = 0, Level = 34, Relearn = new[]{437, 275, 230, 390}, Location = -1, }, // Roselia
            new EncounterStatic { Species = 397, Form = 0, Level = 27, Relearn = new[]{355, 018, 283, 104}, Location = -1, }, // Staravia
            new EncounterStatic { Species = 288, Form = 0, Level = 27, Relearn = new[]{359, 498, 163, 203}, Location = -1, }, // Vigoroth
            new EncounterStatic { Species = 610, Form = 0, Level = 28, Relearn = new[]{231, 337, 206, 163}, Location = -1, }, // Axew
            new EncounterStatic { Species = 604, Form = 0, Level = 55, Relearn = new[]{435, 051, 029, 306}, Location = -1, }, // Eelektross
            new EncounterStatic { Species = 534, Form = 0, Level = 57, Relearn = new[]{409, 276, 264, 444}, Location = -1, }, // Conkeldurr
            new EncounterStatic { Species = 468, Form = 0, Level = 59, Relearn = new[]{248, 403, 396, 245}, Location = -1, }, // Togekiss
            new EncounterStatic { Species = 542, Form = 0, Level = 57, Relearn = new[]{382, 437, 014, 494}, Location = -1, }, // Leavanny
            new EncounterStatic { Species = 497, Form = 0, Level = 43, Relearn = new[]{137, 489, 348, 021}, Location = -1, }, // Serperior
            new EncounterStatic { Species = 503, Form = 0, Level = 43, Relearn = new[]{362, 227, 453, 279}, Location = -1, }, // Samurott
            new EncounterStatic { Species = 500, Form = 0, Level = 43, Relearn = new[]{276, 053, 372, 535}, Location = -1, }, // Emboar
            new EncounterStatic { Species = 785, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 030, IV3 = true, }, // Tapu Koko
            new EncounterStatic { Species = 786, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 092, IV3 = true, }, // Tapu Lele
            new EncounterStatic { Species = 787, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 140, IV3 = true, }, // Tapu Bulu
            new EncounterStatic { Species = 788, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 180, IV3 = true, }, // Tapu Fini
            new EncounterStatic { Species = 296, Form = 0, Level = 09, Relearn = new[]{000, 000, 000, 000}, Ability = 1, Location = -1, }, // Makuhita
            new EncounterStatic { Species = 103, Form = 1, Level = 40, Relearn = new[]{000, 000, 000, 000}, Ability = 1, Location = 184, }, // Exeggutor-1 @ Exeggutor Island
            new EncounterStatic { Species = 785, Form = 0, Level = 60, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 030, }, // Tapu Koko
            new EncounterStatic { Species = 542, Form = 0, Level = 57, Relearn = new[]{382, 437, 014, 494}, Location = -1, }, // Leavanny
            
            new EncounterStatic { Species = 718, Form = 0, Level = 50, Relearn = new[]{000, 000, 000, 000}, Shiny = false, Ability = 1, Location = 118, IV3 = true, }, // Zygarde
        };
        private static readonly EncounterTrade[] TradeGift_SM = // @ a\1\5\5
        {
            // Trades - 4.bin
            new EncounterTrade { Species = 066, Form = 0, Level = 09, Ability = 2, TID = 00410, SID = 00000, OTGender = 1, Gender = 0, Nature = Nature.Brave, }, // Machop
            new EncounterTrade { Species = 761, Form = 0, Level = 16, Ability = 1, TID = 20683, SID = 00009, OTGender = 0, Gender = 1, Nature = Nature.Adamant, }, // Bounsweet
            new EncounterTrade { Species = 061, Form = 0, Level = 22, Ability = 2, TID = 01092, SID = 00009, OTGender = 1, Gender = 1, Nature = Nature.Naughty, }, // Poliwhirl
            new EncounterTrade { Species = 440, Form = 0, Level = 27, Ability = 2, TID = 10913, SID = 00000, OTGender = 1, Gender = 1, Nature = Nature.Calm, }, // Happiny
            new EncounterTrade { Species = 075, Form = 1, Level = 32, Ability = 1, TID = 20778, SID = 00009, OTGender = 0, Gender = 0, Nature = Nature.Impish, }, // Graveler-1
            new EncounterTrade { Species = 762, Form = 0, Level = 43, Ability = 1, TID = 20679, SID = 00009, OTGender = 1, Gender = 1, Nature = Nature.Careful, }, // Steenee
            new EncounterTrade { Species = 663, Form = 0, Level = 59, Ability = 4, TID = 56734, SID = 00008, OTGender = 0, Gender = 0, Nature = Nature.Jolly, }, // Talonflame
        };
        #endregion
        private static readonly EncounterLink[] LinkGifts7 =
        {

        };

        private static readonly int[] WildPokeballs7 = {
            0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, // Johto Balls
            0x1A, // Beast
        };
        internal static readonly int[] AlolanOriginForms =
        {
            019, // Rattata
            020, // Raticate
            025, // Pikachu (Cosplay not transferrable)
            026, // Raichu
            027, // Sandshrew
            028, // Sandslash
            037, // Vulpix
            038, // Ninetails
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
        internal static readonly int[] PastGenAlolanNatives =
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
            718
        };

        internal static readonly int[] PastGenAlolanScans =
        {
            152, // Chikorita
            155, // Cyndaquil
            158, // Totodile

            069, // Bellsprout
            111, // Rhyhorn
            116, // Horsea
            183, // Marill
            220, // Swinub
            315, // Roselia
            363, // Spheal
            404, // Luxio
            543, // Venipede
            574, // Gothita
            578, // Duosion
            599, // Klink
            607, // Litwick
            610, // Axew
            633, // Deino
            679, // Honedge

            175, // [468] Togekiss (Togepi)
            287, // [288] Vigoroth (Slakoth)
            396, // [397] Staravia (Starly)
            495, // [497] Serperior (Snivy)

            498, // [500] Emboar (Tepig)
            501, // [503] Samurott (Oshawott)
            532, // [534] Conkeldurr (Timburr)
            540, // [542] Leavanny (Sewaddle)
            602, // [604] Eelektross (Tynamo)
        };
        internal static readonly int[] Inherit_SafariMale =
        {
            081, 100, 128, 337, 338, 374, 436,
        };
        internal static readonly int[] InheritDreamMale =
        {
            001, 004, 007, 25, 81, 100, 236, 120, 128, 137,
            172,
            252, 255, 258, 337, 338, 343, 374,
            387, 390, 393, 436, 479,
            511, 513, 515, 538, 539, 574, 599, 622,
        };
        internal static readonly int[] Ban_Gen3Ball_AllowG7 =
        {
            495, 498, 501, //1 - Snivy, Tepig, Oshawott
            496, 499, 502, //2
            497, 500, 503, //3
        };
        internal static readonly int[] Ban_Gen4Ball_AllowG7 =
        {
            152, 155, 158, //1 - Chikorita, Cyndaquil, Totodile
            153, 156, 159, //2
            154, 157, 160, //3
            495, 498, 501, //1 - Snivy, Tepig, Oshawott
            496, 499, 502, //2
            497, 500, 503, //3
        };

        internal static readonly int[] ZygardeMoves =
        {
            245, // Extreme Speed
            349, // Dragon Dance
            614, // Thousand Arrows
            615, // Thousand Waves
            687, // Core Enforcer
        };

        internal static readonly int[] ValidMet_SM =
        {
            006, 008, 010, 012, 014, 016, 018, 020, 022, 024, 026, 028, 030, 032, 034, 036, 038, 040, 042, 044, 046, 048,
            050, 052, 054, 056, 058, 060, 062, 064, 066, 068, 070, 072, 074, 076, 078, 082, 084, 086, 088, 090, 092, 094,
            100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 136, 138, 140, 142, 144, 146, 148,
            150, 152, 154, 156, 158, 160, 162, 164, 166, 168, 170, 172, 174, 176, 178, 180, 182, 184, 186, 188, 190, 192,

            30016 // Poké Pelago
        };

        private static readonly int[] TMHM_SM =
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

        internal static readonly int[] MovePP_SM =
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
            01, 01, 01, 05, 05, 10, 10, 10, 20, 10, 10, 10, 05, 05, 20, 10, 10, 10, 01
        };
    }
}
