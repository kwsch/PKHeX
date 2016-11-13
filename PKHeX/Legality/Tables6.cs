using System.Linq;

namespace PKHeX
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_6 = 721;

        // PKHeX Valid Array Storage

        #region Met Locations
        internal static readonly int[] Met_XYc = {0, 60002, 30002,};

        internal static readonly int[] Met_XY_0 =
        {
   /* XY */ 2, 6, 8, 10, 12, 14, 16, 17, 18, 20, 22, 24, 26, 28, 30, 32,
            34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 82, 84, 86, 88,
            90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134,
            136, 138, 140, 142, 144, 146, 148, 150, 152, 154, 156, 158, 160, 162, 164, 166, 168, /* ORAS */ 170, 172,
            174, 176, 178, 180, 182, 184, 186, 188, 190, 192, 194, 196, 198, 200, 202, 204, 206, 208, 210, 212, 214, 216,
            218, 220, 222, 224, 226, 228, 230, 232, 234, 236, 238, 240, 242, 244, 246, 248, 250, 252, 254, 256, 258, 260,
            262, 264, 266, 268, 270, 272, 274, 276, 278, 280, 282, 284, 286, 288, 290, 292, 294, 296, 298, 300, 302, 304,
            306, 308, 310, 312, 314, 316, 318, 320, 322, 324, 326, 328, 330, 332, 334, 336, 338, 340, 342, 344, 346, 348,
            350, 352, 354,
        };

        internal static readonly int[] Met_XY_3 =
        {
            30001, 30003, 30004, 30005, 30006, 30007, 30008, 30009, 30010, 30011,
        };

        internal static readonly int[] Met_XY_4 =
        {
            40001, 40002, 40003, 40004, 40005, 40006, 40007, 40008, 40009, 40010,
            40011, 40012, 40013, 40014, 40015, 40016, 40017, 40018, 40019, 40020, 40021, 40022, 40023, 40024, 40025,
            40026, 40027, 40028, 40029, 40030, 40031, 40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039, 40040,
            40041, 40042, 40043, 40044, 40045, 40046, 40047, 40048, 40049, 40050, 40051, 40052, 40053, 40054, 40055,
            40056, 40057, 40058, 40059, 40060, 40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069, 40070,
            40071, 40072, 40073, 40074, 40075, 40076, 40077, 40078, 40079,
        };

        internal static readonly int[] Met_XY_6 = {/* XY */ 60001, 60003, /* ORAS */ 60004,};

        #endregion

        #region Inventory Pouch

        internal static readonly ushort[] Pouch_Items_XY =
        {
                 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 055, 056,
            057, 058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075,
            076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087, 088, 089, 090, 091, 092, 093, 094,
            099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 112, 116, 117, 118, 119, 135, 136,
            213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232,
            233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
            252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270,
            271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289,
            290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308,
            309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327,
            492, 493, 494, 495, 496, 497, 498, 499, 500, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546,
            547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 571,
            572, 573, 576, 577, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 639, 640, 644, 646,
            647, 648, 649, 650, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666,
            667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685,
            699, 704, 710, 711, 715,
        };

        internal static readonly ushort[] Pouch_Items_AO =
        {
                 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 055, 056,
            057, 058, 059, 060, 061, 062, 063, 064, 068, 069, 070, 071, 072, 073, 074, 075,
            076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087, 088, 089, 090, 091, 092, 093, 094,
            099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 112, 116, 117, 118, 119, 135, 136,
            213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232,
            233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
            252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270,
            271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289,
            290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308,
            309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327,
            492, 493, 494, 495, 496, 497, 498, 499, 500, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546,
            547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 571,
            572, 573, 576, 577, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 639, 640, 644, 646,
            647, 648, 649, 650, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666,
            667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685,
            699, 704, 710, 711, 715,

            // ORAS
            534, 535,
            752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 767, 768, 769, 770,
        };

        internal static readonly ushort[] Pouch_Key_XY =
        {
                 216, 431, 442, 445, 446, 447, 450, 465, 466, 471, 628,
            629, 631, 632, 638, 641, 642, 643, 689, 695, 696, 697, 698,
            700, 701, 702, 703, 705, 706, 707, 712, 713, 714,

            // Illegal
            716, 717, // For the cheaters who want useless items... 
        };

        internal static readonly ushort[] Pouch_Key_AO =
        {
                 216, 445, 446, 447, 465, 466, 471, 628,
            629, 631, 632, 638, 697,

            // Illegal
            716, 717, 745, 746, 747, 748, 749, 750, // For the cheaters who want useless items...

            // ORAS
            457, 474, 503,

            718, 719,
            720, 721, 722, 723, 724, 725, 726, 727, 728, 729,
            730, 731, 732, 733, 734, 735, 736, 738, 739,
            740, 741, 742, 743, 744,
            751, 765, 766, 771, 772, 774, 775,
        };

        internal static readonly ushort[] Pouch_TMHM_XY =
        {
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345,
            346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363,
            364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381,
            382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399,
            400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
            418, 419, 618, 619, 620, 690, 691, 692, 693, 694,

            420, 421, 422, 423, 424,
        };

        internal static readonly ushort[] Pouch_TMHM_AO =
        {
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345,
            346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363,
            364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381,
            382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399,
            400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
            418, 419, 618, 619, 620, 690, 691, 692, 693, 694,

            420, 421, 422, 423, 424,

            // ORAS
            425, 737,
        };

        internal static readonly ushort[] Pouch_Medicine_XY =
        {
                 017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033,
            034, 035, 036, 037, 038, 039, 040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051,
            052, 053, 054, 134, 504, 565, 566, 567, 568, 569, 570, 571, 591, 645, 708, 709,
        };

        internal static readonly ushort[] Pouch_Medicine_AO =
        {
                 017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033,
            034, 035, 036, 037, 038, 039, 040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051,
            052, 053, 054, 134, 504, 565, 566, 567, 568, 569, 570, 571, 591, 645, 708, 709,

            //ORAS
            065, 066, 067
        };

        internal static readonly ushort[] Pouch_Berry_XY =
        {
                 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162,
            163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177,
            178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192,
            193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 686, 687, 688,
        };

        internal static readonly ushort[] HeldItem_XY = new ushort[1].Concat(Pouch_Items_XY).Concat(Pouch_Medicine_XY).Concat(Pouch_Berry_XY).ToArray();
        internal static readonly ushort[] HeldItem_AO = new ushort[1].Concat(Pouch_Items_AO).Concat(Pouch_Medicine_AO).Concat(Pouch_Berry_XY).ToArray();
        #endregion

        #region TMHM

        internal static readonly int[] TMHM_AO =
        {
            468, 337, 473, 347, 046, 092, 258, 339, 474, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 355, 219,
            218, 076, 479, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
            259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
            211, 411, 412, 206, 503, 374, 451, 507, 510, 511,
            261, 512, 373, 153, 421, 371, 514, 416, 397, 148,
            444, 521, 086, 360, 014, 522, 244, 523, 524, 157,
            404, 525, 611, 398, 138, 447, 207, 214, 369, 164,
            430, 433, 528, 290, 555, 267, 399, 612, 605, 590,

            15, 19, 57, 70, 127, 249, 291,
        };

        internal static readonly int[] TMHM_XY =
        {
            468, 337, 473, 347, 046, 092, 258, 339, 474, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 355, 219,
            218, 076, 479, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
            259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
            211, 411, 412, 206, 503, 374, 451, 507, 510, 511,
            261, 512, 373, 153, 421, 371, 514, 416, 397, 148,
            444, 521, 086, 360, 014, 522, 244, 523, 524, 157,
            404, 525, 611, 398, 138, 447, 207, 214, 369, 164,
            430, 433, 528, 249, 555, 267, 399, 612, 605, 590,

            15, 19, 57, 70, 127,
        };

        internal static readonly int[] TypeTutor = {338, 307, 308, 520, 519, 518, 434, 620};

        internal static readonly int[][] Tutors_AO =
        {
            new[] {450, 343, 162, 530, 324, 442, 402, 529, 340, 67, 441, 253, 9, 7, 8},
            new[] {277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401, 399, 428, 406, 304, 231},
            new[] {20, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409, 314, 264, 351, 352},
            new[] {380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446, 214, 285},
        };

        #endregion

        // Legality
        internal const int Struggle = 165;
        internal const int Chatter = 448;
        internal static readonly int[] InvalidSketch =
        {
            // Regular Moves
            Struggle, Chatter
            // Z-Moves
            // TODO
        };
        internal static readonly int[] EggLocations = {60002, 30002};
        internal static readonly int[] ValidMet_XY =
        {
            006, 008, 009, 010, 012, 013, 014, 016, 017, 018, 020, 021, 022, 024, 026, 028, 029, 030, 032, 034, 035, 036,
            038, 039, 040, 042, 043, 044, 046, 047, 048, 050, 051, 052, 054, 055, 056, 058, 060, 062, 063, 064, 066, 067,
            068, 069, 070, 072, 074, 075, 076, 078, 079, 082, 084, 085, 086, 088, 089, 090, 092, 093, 094, 096, 097, 098,
            100, 101, 102, 103, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 135, 136,
            138, 140, 142, 144, 146, 148, 150, 152, 154, 156, 158, 160, 162, 164, 166, 168
        };
        internal static readonly int[] ValidMet_AO =
        {
            170, 172, 174, 176, 178, 180, 182, 184, 186, 188, 190, 192, 194, 196, 198,
            200, 202, 204, 206, 208, 210, 212, 214, 216, 218, 220, 222, 224, 226, 228, 230, 232, 234, 236, 238, 240, 242,
            244, 246, 248, 250, 252, 254, 256, 258, 260, 262, 264, 266, 268, 270, 272, 274, 276, 278, 280, 282, 284, 286,
            288, 290, 292, 294, 296, 298, 300, 302, 304, 306, 308, 310, 312, 314, 316, 318, 320, 322, 324, 326, 328, 330,
            332, 334, 336, 338, 340, 342, 344, 346, 348, 350, 352, 354
        };
        internal static readonly int[] FriendSafari =
        {
            190, 206, 216, 506, 294, 352, 531, 572, 113, 132, 133, 235,
            012, 046, 165, 415, 267, 284, 313, 314, 049, 127, 214, 666,
            262, 274, 624, 629, 215, 332, 342, 551, 302, 359, 510, 686,
            444, 611, 148, 372, 714, 621, 705,
            101, 417, 587, 702, 025, 125, 618, 694, 310, 404, 523, 596,
            175, 209, 281, 702, 039, 303, 682, 684, 035, 670,
            056, 067, 307, 619, 538, 539, 674, 236, 286, 297, 447,
            058, 077, 126, 513, 005, 218, 636, 668, 038, 654, 662,
            016, 021, 083, 084, 163, 520, 527, 581, 357, 627, 662, 701,
            353, 608, 708, 710, 356, 426, 442, 623,
            043, 114, 191, 511, 002, 541, 548, 586, 556, 651, 673,
            027, 194, 231, 328, 051, 105, 290, 323, 423, 536, 660,
            225, 361, 363, 459, 215, 614, 712, 087, 091, 131, 221,
            014, 044, 268, 336, 049, 168, 317, 569, 089, 452, 454, 544,
            063, 096, 326, 517, 202, 561, 677, 178, 203, 575, 578,
            299, 525, 557, 095, 219, 222, 247, 112, 213, 689,
            082, 303, 597, 205, 227, 375, 600, 437, 530, 707,
            098, 224, 400, 515, 008, 130, 195, 419, 061, 184, 657
        };
        internal static readonly int[] PikachuMoves = { 0, 309, 556, 577, 604, 560 };
        internal static readonly int[] WildPokeballs6 = { 0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        internal static readonly string[][] TradeXY =
        {
            new string[0],                       // 0 - None
            Util.getStringList("tradexy", "ja"), // 1
            Util.getStringList("tradexy", "en"), // 2
            Util.getStringList("tradexy", "fr"), // 3
            Util.getStringList("tradexy", "it"), // 4
            Util.getStringList("tradexy", "de"), // 5
            new string[0],                       // 6 - None
            Util.getStringList("tradexy", "es"), // 7
            Util.getStringList("tradexy", "ko"), // 8
        };
        internal static readonly string[][] TradeAO =
        {
            new string[0],                       // 0 - None
            Util.getStringList("tradeao", "ja"), // 1
            Util.getStringList("tradeao", "en"), // 2
            Util.getStringList("tradeao", "fr"), // 3
            Util.getStringList("tradeao", "it"), // 4
            Util.getStringList("tradeao", "de"), // 5
            new string[0],                       // 6 - None
            Util.getStringList("tradeao", "es"), // 7
            Util.getStringList("tradeao", "ko"), // 8
        };

        #region XY Alt Slots
        private static readonly EncounterArea[] SlotsXYAlt =
        {
            new EncounterArea {
                Location = 104, // Victory Road
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 075, LevelMin = 57, LevelMax = 57, Form = 0 }, // Graveler
                    new EncounterSlot { Species = 168, LevelMin = 58, LevelMax = 59, Form = 0 }, // Ariados
                    new EncounterSlot { Species = 714, LevelMin = 57, LevelMax = 59, Form = 0 }, // Noibat

                    // Swoops
                    new EncounterSlot { Species = 022, LevelMin = 57, LevelMax = 59, Form = 0 }, // Fearow
                    new EncounterSlot { Species = 227, LevelMin = 57, LevelMax = 59, Form = 0 }, // Skarmory
                    new EncounterSlot { Species = 635, LevelMin = 59, LevelMax = 59, Form = 0 }, // Hydreigon
                },},
            new EncounterArea {
                    Location = 34, // Route 6
                Slots = new[]
                {
                    // Rustling Bush
                    new EncounterSlot { Species = 543, LevelMin = 10, LevelMax = 12, Form = 0 }, // Venipede
                    new EncounterSlot { Species = 531, LevelMin = 10, LevelMax = 12, Form = 0 }, // Audino
                },},

            new EncounterArea { Location = 38, // Route 7
                Slots = new[]
                {
                    // Berry Field
                    new EncounterSlot { Species = 165, LevelMin = 14, LevelMax = 15, Form = 0 }, // Ledyba
                    new EncounterSlot { Species = 313, LevelMin = 14, LevelMax = 15, Form = 0 }, // Volbeat
                    new EncounterSlot { Species = 314, LevelMin = 14, LevelMax = 15, Form = 0 }, // Illumise
                    new EncounterSlot { Species = 412, LevelMin = 14, LevelMax = 15, Form = 0 }, // Burmy
                    new EncounterSlot { Species = 415, LevelMin = 14, LevelMax = 15, Form = 0 }, // Combee
                    new EncounterSlot { Species = 665, LevelMin = 14, LevelMax = 15, Form = 0 }, // Spewpa
                },},

            new EncounterArea { Location = 88, // Route 18
                Slots = new[]
                {
                    // Rustling Bush
                    new EncounterSlot { Species = 632, LevelMin = 44, LevelMax = 46, Form = 0 }, // Durant
                    new EncounterSlot { Species = 631, LevelMin = 45, LevelMax = 45, Form = 0 }, // Heatmor
                },},

            new EncounterArea { Location = 132, // Glittering Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 527, LevelMin = 15, LevelMax = 17, Form = 0 }, // Woobat
                    new EncounterSlot { Species = 597, LevelMin = 15, LevelMax = 17, Form = 0 }, // Ferroseed
                },},

            new EncounterArea { Location = 56, // Reflection Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 527, LevelMin = 21, LevelMax = 23, Form = 0 }, // Woobat
                    new EncounterSlot { Species = 597, LevelMin = 21, LevelMax = 23, Form = 0 }, // Ferroseed
                },},

            new EncounterArea { Location = 140, // Terminus Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 168, LevelMin = 44, LevelMax = 46, Form = 0 }, // Ariados
                    new EncounterSlot { Species = 714, LevelMin = 44, LevelMax = 46, Form = 0 }, // Noibat
                },},
        };
        #endregion
        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic[] Encounter_XY =
        {
            new EncounterStatic { Species = 650, Level = 5, Location = 10, Gift = true }, // Chespin
            new EncounterStatic { Species = 653, Level = 5, Location = 10, Gift = true }, // Fennekin
            new EncounterStatic { Species = 656, Level = 5, Location = 10, Gift = true }, // Froakie

            new EncounterStatic { Species = 1, Level = 10, Location = 22, Gift = true }, // Bulbasaur
            new EncounterStatic { Species = 4, Level = 10, Location = 22, Gift = true }, // Charmander
            new EncounterStatic { Species = 7, Level = 10, Location = 22, Gift = true }, // Squirtle

            new EncounterStatic { Species = 448, Level = 32, Location = 60, Ability = 1, Nature = Nature.Hasty, Gender = 0, IVs = new[] {6, 25, 16, 31, 25, 19}, Gift = true, Shiny = false }, // Lucario
            new EncounterStatic { Species = 131, Level = 30, Location = 62, Nature = Nature.Docile, IVs = new[] {31, 20, 20, 20, 20, 20}, Gift = true }, // Lapras
            
            new EncounterStatic { Species = 143, Level = 15, Location = 38 }, // Snorlax
            new EncounterStatic { Species = 568, Level = 35, Location = 142 }, // Trubbish
            new EncounterStatic { Species = 569, Level = 36, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 37, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 38, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 479, Level = 38, Location = 142 }, // Rotom

            new EncounterStatic { Species = 569, Level = 46, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 47, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 48, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 49, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 50, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 354, Level = 46, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 47, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 48, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 49, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 50, Location = 98 }, // Banette
            
            new EncounterStatic { Species = 716, Level = 50, Location = 138, Version = GameVersion.X, Shiny = false, IV3 = true }, // Xerneas
            new EncounterStatic { Species = 717, Level = 50, Location = 138, Version = GameVersion.Y, Shiny = false, IV3 = true }, // Yveltal
            new EncounterStatic { Species = 718, Level = 70, Location = 140, Shiny = false, IV3 = true }, // Zygarde
            
            new EncounterStatic { Species = 150, Level = 70, Location = 168, Shiny = false, IV3 = true }, // Mewtwo

            new EncounterStatic { Species = 144, Level = 70, Location = 146, Shiny = false, IV3 = true }, // Articuno
            new EncounterStatic { Species = 145, Level = 70, Location = 146, Shiny = false, IV3 = true }, // Zapdos
            new EncounterStatic { Species = 146, Level = 70, Location = 146, Shiny = false, IV3 = true }, // Moltres
        };
        private static readonly EncounterStatic[] Encounter_AO =
        {
            new EncounterStatic { Species = 252, Level = 5, Location = 204, Gift = true }, // Treeko
            new EncounterStatic { Species = 255, Level = 5, Location = 204, Gift = true }, // Torchic
            new EncounterStatic { Species = 258, Level = 5, Location = 204, Gift = true }, // Mudkip
            
            new EncounterStatic { Species = 152, Level = 5, Location = 204, Gift = true }, // Chikorita
            new EncounterStatic { Species = 155, Level = 5, Location = 204, Gift = true }, // Cyndaquil
            new EncounterStatic { Species = 158, Level = 5, Location = 204, Gift = true }, // Totodile

            new EncounterStatic { Species = 387, Level = 5, Location = 204, Gift = true }, // Turtwig
            new EncounterStatic { Species = 390, Level = 5, Location = 204, Gift = true }, // Chimchar
            new EncounterStatic { Species = 393, Level = 5, Location = 204, Gift = true }, // Piplup

            new EncounterStatic { Species = 495, Level = 5, Location = 204, Gift = true }, // Snivy
            new EncounterStatic { Species = 498, Level = 5, Location = 204, Gift = true }, // Tepig
            new EncounterStatic { Species = 501, Level = 5, Location = 204, Gift = true }, // Oshawott

            new EncounterStatic { Species = 25, Level = 20, Location = 178, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 180, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 186, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 194, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false }, // Pikachu

            new EncounterStatic { Species = 360, Level = 1, EggLocation = 60004, Ability = 1, Gift = true }, // Wynaut
            new EncounterStatic { Species = 175, Level = 1, EggLocation = 60004, Ability = 1, Gift = true }, // Togepi
            new EncounterStatic { Species = 374, Level = 1, Location = 196, Ability = 1, IVs = new[] {-1, -1, 31, -1, -1, 31}, Gift = true }, // Beldum

            new EncounterStatic { Species = 351, Level = 30, Location = 240, Nature = Nature.Lax, Ability = 1, IVs = new[] {-1, -1, -1, -1, 31, -1}, Contest = new[] {0,100,0,0,0,0}, Gift = true }, // Castform
            new EncounterStatic { Species = 319, Level = 40, Location = 318, Gender = 1, Ability = 1, Nature = Nature.Adamant, Gift = true }, // Sharpedo
            new EncounterStatic { Species = 323, Level = 40, Location = 318, Gender = 1, Ability = 1, Nature = Nature.Quiet, Gift = true }, // Camerupt
            
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.AS, Ability = 1, Gift = true, IV3 = true }, // Latias
            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.OR, Ability = 1, Gift = true, IV3 = true }, // Latios

            new EncounterStatic { Species = 382, Level = 45, Location = 296, Version = GameVersion.AS, Shiny = false, IV3 = true }, // Kyogre
            new EncounterStatic { Species = 383, Level = 45, Location = 296, Version = GameVersion.OR, Shiny = false, IV3 = true }, // Groudon
            new EncounterStatic { Species = 384, Level = 70, Location = 316, Shiny = false, IV3 = true }, // Rayquaza
            new EncounterStatic { Species = 386, Level = 80, Location = 316, Shiny = false, IV3 = true, Fateful = true }, // Deoxys

            new EncounterStatic { Species = 377, Level = 40, Location = 278, IV3 = true }, // Regirock
            new EncounterStatic { Species = 378, Level = 40, Location = 306, IV3 = true }, // Regice
            new EncounterStatic { Species = 379, Level = 40, Location = 308, IV3 = true }, // Registeel
            new EncounterStatic { Species = 486, Level = 50, Location = 306, IV3 = true }, // Regigigas
            
            new EncounterStatic { Species = 249, Level = 50, Location = 304, Version = GameVersion.AS, IV3 = true }, // Lugia
            new EncounterStatic { Species = 250, Level = 50, Location = 304, Version = GameVersion.OR, IV3 = true }, // Ho-oh

            new EncounterStatic { Species = 483, Level = 50, Location = 348, Version = GameVersion.AS, IV3 = true }, // Dialga
            new EncounterStatic { Species = 484, Level = 50, Location = 348, Version = GameVersion.OR, IV3 = true }, // Palkia

            new EncounterStatic { Species = 644, Level = 50, Location = 340, Version = GameVersion.AS, IV3 = true }, // Zekrom
            new EncounterStatic { Species = 643, Level = 50, Location = 340, Version = GameVersion.OR, IV3 = true }, // Reshiram

            new EncounterStatic { Species = 642, Level = 50, Location = 348, Version = GameVersion.AS, IV3 = true }, // Thundurus
            new EncounterStatic { Species = 641, Level = 50, Location = 348, Version = GameVersion.OR, IV3 = true }, // Tornadus

            new EncounterStatic { Species = 485, Level = 50, Location = 312, IV3 = true }, // Heatran
            new EncounterStatic { Species = 487, Level = 50, Location = 348, IV3 = true }, // Giratina
            new EncounterStatic { Species = 488, Level = 50, Location = 344, IV3 = true }, // Cresselia
            new EncounterStatic { Species = 645, Level = 50, Location = 348, IV3 = true }, // Landorus
            new EncounterStatic { Species = 646, Level = 50, Location = 342, IV3 = true }, // Kyurem
            
            new EncounterStatic { Species = 243, Level = 50, Location = 334, IV3 = true }, // Raikou
            new EncounterStatic { Species = 244, Level = 50, Location = 334, IV3 = true }, // Entei
            new EncounterStatic { Species = 245, Level = 50, Location = 334, IV3 = true }, // Suicune

            new EncounterStatic { Species = 480, Level = 50, Location = 338, IV3 = true }, // Uxie
            new EncounterStatic { Species = 481, Level = 50, Location = 338, IV3 = true }, // Mesprit
            new EncounterStatic { Species = 482, Level = 50, Location = 338, IV3 = true }, // Azelf

            new EncounterStatic { Species = 638, Level = 50, Location = 336, IV3 = true }, // Cobalion
            new EncounterStatic { Species = 639, Level = 50, Location = 336, IV3 = true }, // Terrakion
            new EncounterStatic { Species = 640, Level = 50, Location = 336, IV3 = true }, // Virizion
            
            new EncounterStatic { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119
            new EncounterStatic { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120
            new EncounterStatic { Species = 352, Level = 40, Location = 176 }, // Kecleon @ Lavaridge
            new EncounterStatic { Species = 352, Level = 45, Location = 196 }, // Kecleon @ Mossdeep City

            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.AS, IV3 = true }, // Latios
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.OR, IV3 = true }, // Latias
            
            new EncounterStatic { Species = 101, Level = 40, Location = 292, Version = GameVersion.AS }, // Electrode
            new EncounterStatic { Species = 101, Level = 40, Location = 314, Version = GameVersion.OR }, // Electrode
            
            new EncounterStatic { Species = 100, Level = 20, Location = 302 }, // Voltorb @ Route 119
            new EncounterStatic { Species = 442, Level = 50, Location = 304 }, // Spiritomb @ Route 120

            // Soaring in the Sky
            new EncounterStatic { Species = 198, Level = 45, Location = 348 }, // Murkrow
            new EncounterStatic { Species = 276, Level = 40, Location = 348 }, // Taillow
            new EncounterStatic { Species = 278, Level = 40, Location = 348 }, // Wingull
            new EncounterStatic { Species = 279, Level = 40, Location = 348 }, // Pelipper
            new EncounterStatic { Species = 333, Level = 40, Location = 348 }, // Swablu
            new EncounterStatic { Species = 425, Level = 45, Location = 348 }, // Drifloon
            new EncounterStatic { Species = 628, Level = 45, Location = 348 }, // Braviary
        };
        #endregion
        #region Trade Tables
        internal static readonly EncounterTrade[] TradeGift_XY =
        {
            new EncounterTrade { Species = 129, Level = 5, Ability = 1, Gender = 0, TID = 44285, Nature = Nature.Adamant, }, // Magikarp
            new EncounterTrade { Species = 133, Level = 5, Ability = 1, Gender = 1, TID = 29294, Nature = Nature.Docile, }, // Eevee

            new EncounterTrade { Species = 83, Level = 10, Ability = 1, Gender = 0, TID = 00185, Nature = Nature.Jolly, IVs = new[] {-1, -1, -1, 31, -1, -1}, }, // Farfetch'd
            new EncounterTrade { Species = 208, Level = 20, Ability = 1, Gender = 1, TID = 19250, Nature = Nature.Impish, IVs = new[] {-1, -1, 31, -1, -1, -1}, }, // Steelix
            new EncounterTrade { Species = 625, Level = 50, Ability = 1, Gender = 0, TID = 03447, Nature = Nature.Adamant, IVs = new[] {-1, 31, -1, -1, -1, -1}, }, // Bisharp

            new EncounterTrade { Species = 656, Level = 5, Ability = 1, Gender = 0, TID = 00037, Nature = Nature.Jolly, IVs = new[] {20, 20, 20, 31, 20, 20}, }, // Froakie
            new EncounterTrade { Species = 650, Level = 5, Ability = 1, Gender = 0, TID = 00037, Nature = Nature.Adamant, IVs = new[] {20, 31, 20, 20, 20, 20}, }, // Chespin
            new EncounterTrade { Species = 653, Level = 5, Ability = 1, Gender = 0, TID = 00037, Nature = Nature.Modest, IVs = new[] {20, 20, 20, 20, 31, 20}, }, // Fennekin

            new EncounterTrade { Species = 280, Level = 5, Ability = 1, Gender = 1, TID = 37110, Nature = Nature.Modest, IVs = new[] {20, 20, 20, 31, 31, 20}, }, // Ralts
        };
        internal static readonly EncounterTrade[] TradeGift_AO =
        {
            new EncounterTrade { Species = 296, Level = 9, Ability = 2, Gender = 0, TID = 30724, Nature = Nature.Brave, IVs = new[] {-1, 31, -1, -1, -1, -1}, }, // Makuhita
            new EncounterTrade { Species = 300, Level = 30, Ability = 1, Gender = 1, TID = 03239, Nature = Nature.Naughty, IVs = new[] {-1, -1, -1, 31, -1, -1}, }, // Skitty
            new EncounterTrade { Species = 222, Level = 50, Ability = 4, Gender = 1, TID = 00325, Nature = Nature.Calm, IVs = new[] {31, -1, -1, -1, -1, 31}, }, // Corsola
        };
        #endregion
        #region Pokémon Link Gifts

        private static readonly EncounterLink[] LinkGifts6 =
        {
            new EncounterLink { Species = 154, Level = 50, Ability = 4, XY = true, ORAS = true }, // Meganium
            new EncounterLink { Species = 157, Level = 50, Ability = 4, XY = true, ORAS = true }, // Typhlosion
            new EncounterLink { Species = 160, Level = 50, Ability = 4, XY = true, ORAS = true }, // Feraligatr

            new EncounterLink { Species = 251, Level = 10, Ability = 1, RelearnMoves = new[] {610, 0, 0, 0}, Ball = 11, XY = true }, // Celebi

            new EncounterLink { Species = 377, Level = 50, Ability = 4, RelearnMoves = new[] {153, 8, 444, 359}, XY = true, ORAS = true }, // Regirock
            new EncounterLink { Species = 378, Level = 50, Ability = 4, RelearnMoves = new[] {85, 133, 58, 258}, XY = true, ORAS = true }, // Regice
            new EncounterLink { Species = 379, Level = 50, Ability = 4, RelearnMoves = new[] {442, 157, 356, 334}, XY = true, ORAS = true }, // Registeel

            new EncounterLink { Species = 208, Level = 40, Ability = 1, Classic = false, ORAS = true, OT = false }, // Steelix
            new EncounterLink { Species = 362, Level = 40, Ability = 1, Classic = false, ORAS = true, OT = false }, // Glalie
        };

        #endregion
        #region Ball Table
        internal static readonly int[] Inherit_Sport =
        {
            010, 013, 046, 048, 123, 127, 265, 290, 314, 401, 415,
            
            313, // Via Illumise
        };
        internal static readonly int[] Inherit_Apricorn =
        {
            010, 013, 016, 019, 021, 023, 025, 027, 029, 035, 037, 039, 041,
            043, 046, 048, 050, 052, 054, 056, 058, 060, 063, 066, 069, 072, 074, 077, 079, 083, 084, 086, 088, 090, 092,
            095, 096, 098, 102, 104, 108, 109, 111, 113, 114, 115, 116, 118, 122, 124, 125, 126, 129, 131, 143, 147, 161,
            163, 165, 167, 170, 177, 179, 183, 185, 187, 190, 191, 193, 194, 198, 200, 202, 203, 204, 206, 207, 209, 211,
            213, 214, 215, 216, 218, 220, 222, 223, 225, 226, 227, 228, 231, 234, 235, 241, 246, 261, 263, 265, 273, 276,
            278, 280, 285, 287, 293, 296, 302, 303, 307, 311, 312, 316, 322, 325, 327, 333, 340, 359, 366, 369, 370, 396,
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
        };

        internal static readonly int[] Inherit_Safari =
        {
            016, 019, 020, 021, 022, 023, 024, 025, 027, 029, 035, 039, 041,
            043, 046, 048, 050, 054, 055, 060, 063, 066, 069, 070, 074, 077, 079, 080, 083, 084, 088, 092, 095, 096, 098,
            099, 102, 104, 108, 109, 111, 113, 114, 115, 118, 122, 123, 125, 126, 127, 129, 131, 147, 161, 163, 165, 167,
            177, 179, 183, 187, 189, 190, 191, 193, 194, 198, 200, 202, 203, 204, 207, 209, 213, 214, 216, 223, 228, 229,
            231, 234, 235, 241, 246, 263, 264, 270, 271, 273, 283, 284, 285, 286, 288, 298, 299, 304, 305, 307, 308, 309,
            310, 314, 315, 316, 318, 324, 327, 328, 331, 332, 335, 336, 339, 341, 352, 353, 354, 355, 356, 357, 358, 363,
            364, 371, 372, 397, 399, 400, 403, 404, 406, 417, 418, 419, 433, 443, 447, 449, 451, 453, 455,

            032, // Via Nidoran-F
            313, // Via Illumise

            440, // Via Chansey
            239, // Via Electabuzz
            240, // Via Magmar
            298, // Via Marill
            360, // Via Wobbuffet
            406, // Via Roselia
            433, // Via Chimecho
            439, // Via Mr. Mime
        };
        internal static readonly int[] Inherit_Dream =
        {
            012, 015, 016, 019, 021, 023, 027, 029, 037, 041, 043, 046, 048, 050, 052, 054, 056, 058, 060, 063, 066, 069,
            072, 074, 077, 079, 083, 084, 086, 088, 090, 092, 095, 096, 098, 102, 104, 108, 109, 111, 113, 114, 115, 116,
            118, 122, 123, 127, 129, 131, 133, 138, 140, 142, 143, 147, 161, 163, 165, 167, 170, 173, 174, 175, 177, 179,
            183, 185, 187, 190, 191, 193, 194, 198, 200, 202, 203, 204, 206, 207, 209, 211, 213, 214, 215, 216, 218, 220,
            222, 223, 225, 226, 227, 228, 231, 234, 235, 238, 239, 240, 241, 246, 261, 263, 265, 270, 273, 276, 278, 280,
            283, 285, 287, 290, 293, 296, 299, 300, 302, 303, 304, 307, 309, 311, 312, 314, 315, 316, 318, 320, 322, 324,
            325, 327, 328, 331, 333, 335, 336, 339, 341, 345, 347, 349, 351, 352, 353, 355, 357, 358, 359, 361, 363, 366,
            369, 370, 371, 397, 399, 401, 403, 408, 410, 412, 415, 417, 418, 420, 422, 425, 427, 431, 434, 441, 442, 443,
            447, 449, 451, 453, 455, 456, 459, 517, 519, 525, 529, 531, 533, 535, 545, 546, 548, 550, 553, 556, 558, 559,
            561, 564, 578, 580, 583, 587, 588, 594, 596, 605, 610, 616, 618, 621, 624, 631, 632,

            032, // Via Nidoran-F
            313, // Via Illumise
        };
        internal static readonly int[] Ban_Gen3Ball =
        {
            252, 255, 258, //1 - Treeko, Torchic, Mudkip
            253, 256, 259, //2
            254, 257, 260, //3
            387, 390, 393, //1 - Turtwig, Chimchar, Piplup
            388, 391, 394, //2
            389, 392, 395, //3
            495, 498, 501, //1 - Snivy, Tepig, Oshawott
            496, 499, 502, //2
            497, 500, 503, //3
            566, 567, 696, 697, 698, 699 // Fossil Only obtain
        };
        internal static readonly int[] Ban_Gen4Ball =
        {
            152, 155, 158, //1 - Chikorita, Cyndaquil, Totodile
            153, 156, 159, //2
            154, 157, 160, //3
            252, 255, 258, //1 - Treeko, Torchic, Mudkip
            253, 256, 259, //2
            254, 257, 260, //3
            387, 390, 393, //1 - Turtwig, Chimchar, Piplup
            388, 391, 394, //2
            389, 392, 395, //3
            495, 498, 501, //1 - Snivy, Tepig, Oshawott
            496, 499, 502, //2
            497, 500, 503, //3
            566, 567, 696, 697, 698, 699 // Fossil Only obtain
        };
        internal static readonly int[] WurmpleEvolutions =
        {
            266, 267, // Silcoon Beautifly
            268, 269, // Cascoon Dustox
        };
        #endregion
        #region Memory Table
        internal static readonly int[] Memory_NotXY =
        {
            65, // {0} was with {1} when (he/she) built a Secret Base. {4} that {3}.
            66, // {0} participated in a contest with {1} and impressed many people. {4} that {3}.
            67, // {0} participated in a contest with {1} and won the title. {4} that {3}.
            68, // {0} soared through the sky with {1} and went to many different places. {4} that {3}.
            69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
        };
        internal static readonly int[] Memory_NotAO =
        {
            11, // {0} went clothes shopping with {1}. {4} that {3}.
            43, // {0} was impressed by the speed of the train it took with {1}. {4} that {3}.
            44, // {0} encountered {2} with {1} using the Poké Radar. {4} that {3}.
            56, // {0} was with {1} when (he/she) went to a boutique and tried on clothes, but (he/she) left the boutique without buying anything. {4} that {3}.
            57, // {0} went to a nice restaurant with {1} and ate until it got totally full. {4} that {3}.
            62, // {0} saw itself in a mirror in a mirror cave that it went to with {1}. {4} that {3}.
        };
        internal static readonly int[][] MoveSpecificMemories =
        {
            new[] {
                20, // {0} surfed across the water, carrying {1} on its back. {4} that {3}.
                24, // {0} flew, carrying {1} on its back, to {2}. {4} that {3}.
                35, // {0} proudly used Strength at {1}’s instruction in... {2}. {4} that {3}.
                36, // {0} proudly used Cut at {1}’s instruction in... {2}. {4} that {3}.
                37, // {0} shattered rocks to its heart’s content at {1}’s instruction in... {2}. {4} that {3}.
                38, // {0} used Waterfall while carrying {1} on its back in... {2}. {4} that {3}.
                69, // {1} asked {0} to dive. Down it went, deep into the ocean, to explore the bottom of the sea. {4} that {3}.
            },
            new[] { 57, 19, 70, 15, 249, 127, 291}, // Move IDs
        };
        internal static readonly int[][] LocationsWithPKCenter =
        {
            new[] {
                // Kalos locations with a PKMN CENTER
                18,  // Santalune City
                22,  // Lumiose City
                30,  // Camphrier Town
                40,  // Cyllage City
                44,  // Ambrette Town
                52,  // Geosenge Towny
                58,  // Shalour City
                64,  // Coumarine City
                70,  // Laverre City
                76,  // Dendemille Town
                86,  // Anistar City
                90,  // Couriway Town
                94,  // Snowbelle City
                106, // Pokémon League (X/Y)
                // Hoenn locations with a PKMN CENTER
                172, // Oldale Town
                174, // Dewford Town
                176, // Lavaridge Town
                178, // Fallarbor Town
                180, // Verdanturf Town
                182, // Pacifidlog Town
                184, // Petalburg City
                186, // Slateport City
                188, // Mauville City
                190, // Rustboro City
                192, // Fortree City
                194, // Lilycove City
                196, // Mossdeep City
                198, // Sootopolis City
                200, // Ever Grande City
                202, // Pokémon League (OR/AS)
            },
            new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, // Region matching
        };
        #endregion

        internal static readonly int[] MovePP_XY =
        {
            00, 
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 20, 30, 35, 35, 20, 15, 20, 20, 25, 20, 30, 05, 10, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 10, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20, 
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 25, 15, 10, 20, 25, 10, 35, 30, 15, 10, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20, 
            15, 10, 40, 15, 10, 30, 10, 20, 10, 40, 40, 20, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 15, 20, 10, 15, 35, 20, 15, 10, 10, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40, 
            20, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 25, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 10, 
            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 10, 15, 15, 
            10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15, 
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 15, 05, 40, 15, 20, 20, 05, 15, 20, 20, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 25, 15, 20, 15, 20, 15, 20, 10, 
            20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 10, 10, 15, 10, 20, 15, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15, 
            10, 15, 15, 15, 10, 10, 10, 20, 10, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20, 
            10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 10, 05, 05, 10, 05, 05, 05, 15, 10, 10, 10, 10, 10, 10, 15, 20, 15, 10, 15, 10, 15, 10, 20, 10, 15, 10, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 15, 20, 15, 10, 
            15, 15, 15, 15, 10, 10, 10, 10, 10, 15, 15, 15, 15, 05, 05, 15, 05, 10, 10, 10, 20, 20, 20, 10, 10, 30, 15, 15, 10, 15, 25, 10, 15, 10, 10, 10, 20, 10, 10, 10, 10, 10, 15, 15, 05, 05, 10, 10, 10, 05, 
            05, 10, 05, 05, 15, 10, 05, 05, 05, 10, 10, 10, 10, 20, 25, 10, 20, 30, 25, 20, 20, 15, 20, 15, 20, 20, 10, 10, 10, 10, 10, 20, 10, 30, 15, 10, 10, 10, 20, 20, 05, 05, 05, 20, 10, 10, 20, 15, 20, 20, 
            10, 20, 30, 10, 10, 40, 40, 30, 20, 40, 20, 20, 10, 10, 10, 10, 05, 10, 10, 05, 05,
        };
    }
}
