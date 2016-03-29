namespace PKHeX
{
    public static partial class Legal
    {
        // PKHeX Valid Array Storage

        #region Items

        internal static readonly int[] Items_Held =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 017, 018, 019, 020, 021, 022,
            023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035,
            036, 037, 038, 039, 040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051, 052, 053, 054, 055, 056, 057,
            058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 068, 069, 070,
            071, 072, 073, 074, 075, 076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087, 088, 089, 090, 091, 092,
            093, 094, 099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
            110, 112, 116, 117, 118, 119, 134, 135, 136, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161,
            162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174,
            175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196,
            197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
            210, 211, 212, 213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232,
            233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244,
            245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266,
            267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279,
            280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301,
            302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314,
            315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 504, 537, 538, 539, 540, 541, 542, 543, 544,
            545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557,
            558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 577, 580, 581, 582, 583, 584,
            585, 586, 587, 588, 589, 590, 591, 639, 640, 644, 645, 646, 647,
            648, 649, 650, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670,
            671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683,
            684, 685, 686, 687, 688, 699, 704, 708, 709, 710, 711, 715,

            // Appended ORAS Items (Orbs & Mega Stones)
            534, 535,
            752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 767, 768, 769, 770,
        };

        internal static readonly int[] Items_Ball =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012,
            013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576,
        };

        internal static readonly int[] Items_CommonBall = {4, 3, 2, 1};

        internal static readonly int[] Items_UncommonBall =
        {
            7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6,
            12, 15, 9, 5, 499, 10, 16
        };

        #endregion
        
        #region Games

        internal static readonly int[] Games_6xy = {24, 25};
        internal static readonly int[] Games_6oras = {26, 27};
        internal static readonly int[] Games_5 = {20, 21, 22, 23};
        internal static readonly int[] Games_4 = {10, 11,};
        internal static readonly int[] Games_4e = {12};
        internal static readonly int[] Games_4r = {7, 8};
        internal static readonly int[] Games_3 = {1, 2};
        internal static readonly int[] Games_3e = {3};
        internal static readonly int[] Games_3r = {4, 5};
        internal static readonly int[] Games_3s = {15};

        #endregion

        #region Met Locations

        internal static readonly int[] Met_BW2c = {0, 60002, 30003};

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

        internal static readonly int[] Met_BW2_6 = {60001, 60003};
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

        #endregion

        #region Inventory Pouch

        internal static readonly ushort[] Pouch_Items_XY =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 055, 056,
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

        internal static readonly ushort[] Pouch_Items_ORAS =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 055, 056,
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
            000, 216, 431, 442, 445, 446, 447, 450, 465, 466, 471, 628,
            629, 631, 632, 638, 641, 642, 643, 689, 695, 696, 697, 698,
            700, 701, 702, 703, 705, 706, 707, 712, 713, 714,

            // Illegal
            716, 717, // For the cheaters who want useless items... 
        };

        internal static readonly ushort[] Pouch_Key_ORAS =
        {
            000, 216, 445, 446, 447, 465, 466, 471, 628,
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
            0,
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345,
            346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363,
            364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381,
            382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399,
            400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
            418, 419, 618, 619, 620, 690, 691, 692, 693, 694,

            420, 421, 422, 423, 424,
        };

        internal static readonly ushort[] Pouch_TMHM_ORAS =
        {
            0,
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
            000, 017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033,
            034, 035, 036, 037, 038, 039, 040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051,
            052, 053, 054, 134, 504, 565, 566, 567, 568, 569, 570, 571, 591, 645, 708, 709,
        };

        internal static readonly ushort[] Pouch_Medicine_ORAS =
        {
            000, 017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033,
            034, 035, 036, 037, 038, 039, 040, 041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051,
            052, 053, 054, 134, 504, 565, 566, 567, 568, 569, 570, 571, 591, 645, 708, 709,

            //ORAS
            065, 066, 067
        };

        internal static readonly ushort[] Pouch_Berry_XY =
        {
            0, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162,
            163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177,
            178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192,
            193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 686, 687, 688,
        };

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
        internal static readonly int[] Gen4EncounterTypes = { 1, 2, 4, 5, 7, 9, 10, 12, 23, 24 };
        internal static readonly int Struggle = 165;
        internal static readonly int Chatter = 448;
        internal static readonly int[] InvalidSketch = {Struggle, Chatter};
        internal static readonly int[] EggLocations = {318, 60002, 30002};
        internal static readonly int[] LightBall = {25, 26, 172};
        internal static readonly int[] Fossils = {138, 140, 142, 345, 347, 408, 410, 564, 566, 696, 698};
        internal static readonly int[] WildForms =
        {
            422, 423, // Shellos
            550, // Basculin
            669, 670, 671 // Flabébé
        };
        internal static readonly int[] SplitBreed =
        {
            // Incense
            113, 242, // Chansey
            122, // Mr. Mime
            143, // Snorlax
            183, 184, // Marill
            185, // Sudowoodo
            202, // Wobbuffet
            226, // Mantine
            315, 407, // Roselia
            358, // Chimecho
        };
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
        internal static readonly int[] RotomMoves = { 0, 315, 056, 059, 403, 437 };
        internal static readonly int[] PikachuMoves = { 0, 309, 556, 577, 604, 560 };
        internal static readonly int[] WildPokeballs = { 0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        internal static readonly int[] FormChange = // Pokémon that can change form
        {
            25, // Pikachu
            351, // Castform
            386, // Deoxys
            412, // Burmy
            421, // Cherrim
            479, // Rotom
            487, // Giratina
            492, // Shaymin
            493, // Arceus
            641, // Tornadus
            642, // Thundurus
            645, // Landorus
            646, // Kyurem
            647, // Keldeo
            649, // Genesect
            676, // Furfrou
            720, // Hoopa
        };
        internal static readonly int[] FormChangeMoves =
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
        };
        internal static readonly int[] BreedMaleOnly =
        {
            128, // Tauros
            627, 628, // Rufflet
            236, 106, 107, 237, // Tyrogue
            538, 539, // Sawk & Throh
        };

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
            new EncounterStatic { Species = 131, Level = 32, Location = 62, Nature = Nature.Docile, IVs = new[] {31, 20, 20, 20, 20, 20}, Gift = true }, // Lapras
            
            new EncounterStatic { Species = 143, Level = 15, Location = 38 }, // Snorlax
            new EncounterStatic { Species = 568, Level = 35, Location = 142 }, // Trubbish
            new EncounterStatic { Species = 569, Level = 36, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 37, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 38, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 479, Level = 38, Location = 142 }, // Rotom
            
            new EncounterStatic { Species = 716, Level = 50, Location = 138, Version = GameVersion.X, Shiny = false }, // Xerneas
            new EncounterStatic { Species = 717, Level = 50, Location = 138, Version = GameVersion.Y, Shiny = false }, // Yveltal
            new EncounterStatic { Species = 718, Level = 70, Location = 140, Shiny = false }, // Zygarde
            
            new EncounterStatic { Species = 150, Level = 70, Location = 168, Shiny = false }, // Mewtwo

            new EncounterStatic { Species = 144, Level = 70, Location = 146, Shiny = false }, // Articuno
            new EncounterStatic { Species = 145, Level = 70, Location = 146, Shiny = false }, // Zapdos
            new EncounterStatic { Species = 146, Level = 70, Location = 146, Shiny = false }, // Moltres
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

            new EncounterStatic { Species = 25, Level = 20, Location = 186, Gender = 1, Ability = 4, Form = 1, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 186, Gender = 1, Ability = 4, Form = 3, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false }, // Pikachu
            new EncounterStatic { Species = 360, Level = 1, EggLocation = 60004, Ability = 1, Gift = true }, // Wynaut
            new EncounterStatic { Species = 175, Level = 1, EggLocation = 60004, Ability = 1, Gift = true }, // Togepi
            new EncounterStatic { Species = 374, Level = 1, Location = 196, Ability = 1, IVs = new[] {-1, -1, 31, -1, -1, 31}, Gift = true }, // Beldum

            new EncounterStatic { Species = 351, Level = 30, Location = 240, Nature = Nature.Lax, Ability = 1, IVs = new[] {-1, -1, -1, -1, 31, -1}, Contest = new[] {0,100,0,0,0,0}, Gift = true }, // Castform
            new EncounterStatic { Species = 319, Level = 40, Location = 318, Gender = 1, Ability = 1, Nature = Nature.Adamant, Gift = true }, // Sharpedo
            new EncounterStatic { Species = 323, Level = 40, Location = 318, Gender = 1, Ability = 1, Nature = Nature.Quiet, Gift = true }, // Camerupt
            
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.AS, Ability = 1, Gift = true }, // Latias
            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.OR, Ability = 1, Gift = true }, // Latios

            new EncounterStatic { Species = 382, Level = 45, Location = 296, Version = GameVersion.AS, Shiny = false }, // Kyogre
            new EncounterStatic { Species = 383, Level = 45, Location = 296, Version = GameVersion.OR, Shiny = false }, // Groudon
            new EncounterStatic { Species = 384, Level = 70, Location = 316, Shiny = false }, // Rayquaza
            new EncounterStatic { Species = 386, Level = 80, Location = 316, Shiny = false }, // Deoxys

            new EncounterStatic { Species = 377, Level = 40, Location = 278 }, // Regirock
            new EncounterStatic { Species = 378, Level = 40, Location = 306 }, // Regice
            new EncounterStatic { Species = 379, Level = 40, Location = 308 }, // Registeel
            new EncounterStatic { Species = 486, Level = 50, Location = 306 }, // Regigigas
            
            new EncounterStatic { Species = 249, Level = 50, Location = 304, Version = GameVersion.AS }, // Lugia
            new EncounterStatic { Species = 250, Level = 50, Location = 304, Version = GameVersion.OR }, // Ho-oh

            new EncounterStatic { Species = 483, Level = 50, Location = 348, Version = GameVersion.AS }, // Dialga
            new EncounterStatic { Species = 484, Level = 50, Location = 348, Version = GameVersion.OR }, // Palia

            new EncounterStatic { Species = 644, Level = 50, Location = 340, Version = GameVersion.AS }, // Zekrom
            new EncounterStatic { Species = 643, Level = 50, Location = 340, Version = GameVersion.OR }, // Reshiram

            new EncounterStatic { Species = 642, Level = 50, Location = 348, Version = GameVersion.AS }, // Thundurus
            new EncounterStatic { Species = 641, Level = 50, Location = 348, Version = GameVersion.OR }, // Tornadus

            new EncounterStatic { Species = 485, Level = 50, Location = 312 }, // Heatran
            new EncounterStatic { Species = 487, Level = 50, Location = 348 }, // Giratina
            new EncounterStatic { Species = 488, Level = 50, Location = 344 }, // Cresselia
            new EncounterStatic { Species = 645, Level = 50, Location = 348 }, // Landorus
            new EncounterStatic { Species = 646, Level = 50, Location = 342 }, // Kyurem
            
            new EncounterStatic { Species = 243, Level = 50, Location = 334 }, // Raikou
            new EncounterStatic { Species = 244, Level = 50, Location = 334 }, // Entei
            new EncounterStatic { Species = 245, Level = 50, Location = 334 }, // Suicune

            new EncounterStatic { Species = 480, Level = 50, Location = 338 }, // Uxie
            new EncounterStatic { Species = 481, Level = 50, Location = 338 }, // Mesprit
            new EncounterStatic { Species = 482, Level = 50, Location = 338 }, // Azelf

            new EncounterStatic { Species = 638, Level = 50, Location = 336 }, // Cobalion
            new EncounterStatic { Species = 639, Level = 50, Location = 336 }, // Terrakion
            new EncounterStatic { Species = 640, Level = 50, Location = 336 }, // Virizion
            
            new EncounterStatic { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119
            new EncounterStatic { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120
            new EncounterStatic { Species = 352, Level = 40, Location = 176 }, // Kecleon @ Lavaridge
            new EncounterStatic { Species = 352, Level = 45, Location = 196 }, // Kecleon @ Mossdeep City

            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.AS }, // Latios
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.OR }, // Latias
            
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
            new EncounterTrade { Species = 300, Level = 25, Ability = 1, Gender = 1, TID = 03239, Nature = Nature.Naughty, IVs = new[] {-1, -1, -1, 31, -1, -1}, }, // Skitty
            new EncounterTrade { Species = 222, Level = 50, Ability = 4, Gender = 1, TID = 00325, Nature = Nature.Calm, IVs = new[] {31, -1, -1, -1, -1, 31}, }, // Corsola
        };
        #endregion
        #region Pokémon Link Gifts

        private static readonly EncounterLink[] LinkGifts =
        {
            new EncounterLink { Species = 154, Level = 50, Ability = 4 }, // Meganium
            new EncounterLink { Species = 157, Level = 50, Ability = 4 }, // Typhlosion
            new EncounterLink { Species = 160, Level = 50, Ability = 4 }, // Feraligatr

            new EncounterLink {Species = 251, Level = 10, RelearnMoves = new[] {610, 0, 0, 0}, Ball = 11, ORAS = false }, // Celebi

            new EncounterLink { Species = 377, Level = 50, RelearnMoves = new[] {153, 8, 444, 359 }, Ability = 4 }, // Regirock
            new EncounterLink { Species = 378, Level = 50, RelearnMoves = new[] {85, 133, 58, 258 }, Ability = 4 }, // Regice
            new EncounterLink { Species = 379, Level = 50, RelearnMoves = new[] {442, 157, 356, 334 }, Ability = 4 }, // Registeel

            new EncounterLink { Species = 208, Level = 40, Classic = false, Ability = 1, XY = false }, // Steelix
            new EncounterLink { Species = 362, Level = 40, Classic = false, Ability = 1, XY = false }, // Glalie
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
            495, 498, 501, //1 - Snivy, Tepig, Oshawott
            496, 499, 502, //2
            497, 500, 503, //3
            566, 567, 696, 697, 698, 699 // Fossil Only obtain
        };
        #endregion
    }
}
