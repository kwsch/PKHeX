using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_4 = 493;
    internal const int MaxMoveID_4 = 467;
    internal const int MaxItemID_4_DP = 464;
    internal const int MaxItemID_4_Pt = 467;
    internal const int MaxItemID_4_HGSS = 536;
    internal const int MaxAbilityID_4 = 123;
    internal const int MaxBallID_4 = 0x18;
    internal const int MaxGameID_4 = 15; // CXD

    #region DP
    internal static readonly ushort[] Pouch_Items_DP = {
        68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 135, 136, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327,
    };

    internal static readonly ushort[] Pouch_Key_DP = {
        428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464,
    };

    internal static readonly ushort[] Pouch_TMHM_DP = {
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427,
    };

    internal static readonly ushort[] Pouch_Mail_DP = {
        137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148,
    };

    internal static readonly ushort[] Pouch_Medicine_DP = {
        17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54,
    };

    internal static readonly ushort[] Pouch_Berries_DP = {
        149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
    };

    internal static readonly ushort[] Pouch_Ball_DP = {
        1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
    };

    internal static readonly ushort[] Pouch_Battle_DP = {
        55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67,
    };

    internal static readonly ushort[] HeldItems_DP = ArrayUtil.ConcatAll(Pouch_Items_DP, Pouch_Mail_DP, Pouch_Medicine_DP, Pouch_Berries_DP, Pouch_Ball_DP, Pouch_TMHM_DP.Slice(0, Pouch_TMHM_DP.Length - 8));
    #endregion

    #region Pt
    internal static readonly ushort[] Pouch_Items_Pt = {
        68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 135, 136, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327,
    };

    internal static readonly ushort[] Pouch_Key_Pt = {
        428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467,
    };

    internal static readonly ushort[] Pouch_TMHM_Pt = Pouch_TMHM_DP;
    internal static readonly ushort[] Pouch_Mail_Pt = Pouch_Mail_DP;
    internal static readonly ushort[] Pouch_Medicine_Pt = Pouch_Medicine_DP;
    internal static readonly ushort[] Pouch_Berries_Pt = Pouch_Berries_DP;
    internal static readonly ushort[] Pouch_Ball_Pt = Pouch_Ball_DP;
    internal static readonly ushort[] Pouch_Battle_Pt = Pouch_Battle_DP;

    internal static readonly ushort[] HeldItems_Pt = ArrayUtil.ConcatAll(Pouch_Items_Pt, Pouch_Mail_Pt, Pouch_Medicine_Pt, Pouch_Berries_Pt, Pouch_Ball_Pt, Pouch_TMHM_Pt.Slice(0, Pouch_TMHM_Pt.Length - 8));
    #endregion

    #region HGSS
    internal static readonly ushort[] Pouch_Items_HGSS = Pouch_Items_Pt;

    internal static readonly ushort[] Pouch_Key_HGSS = {
        434, 435, 437, 444, 445, 446, 447, 450, 456, 464, 465, 466, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 501, 502, 503, 504, 532, 533, 534, 535, 536,
    };

    internal static readonly ushort[] Pouch_TMHM_HGSS = Pouch_TMHM_DP;
    internal static readonly ushort[] Pouch_Mail_HGSS = Pouch_Mail_DP;
    internal static readonly ushort[] Pouch_Medicine_HGSS = Pouch_Medicine_DP;
    internal static readonly ushort[] Pouch_Berries_HGSS = Pouch_Berries_DP;

    internal static readonly ushort[] Pouch_Ball_HGSS = {
        1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 492, 493, 494, 495, 496, 497, 498, 499, 500,
    };

    internal static readonly ushort[] Pouch_Battle_HGSS = Pouch_Battle_DP;

    internal static readonly ushort[] HeldItems_HGSS = ArrayUtil.ConcatAll(Pouch_Items_HGSS, Pouch_Mail_HGSS, Pouch_Medicine_HGSS, Pouch_Berries_HGSS, Pouch_Ball_Pt, Pouch_TMHM_HGSS.Slice(0, Pouch_TMHM_HGSS.Length - 8));
    #endregion

    internal static readonly bool[] ReleasedHeldItems_4 = GetPermitList(MaxItemID_4_HGSS, HeldItems_HGSS, new ushort[]
    {
        005, // Safari Ball
        016, // Cherish Ball
        147, // Mosaic Mail
        499, // Sport Ball
        500, // Park Ball
    });

    internal static readonly HashSet<ushort> ValidMet_DP = new()
    {
        // 063: Flower Paradise unreleased DP event
        // 079: Newmoon Island unreleased DP event
        // 085: Seabreak Path unreleased DP event
        // 086: Hall of Origin unreleased event
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020,
        021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037, 038, 039, 040,
        041, 042, 043, 044, 045, 046, 047, 048, 049, 050, 051, 052, 053, 054, 055, 056, 057, 058, 059, 060,
        061, 062,      064, 065, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078,      080,
        081, 082, 083, 084,           087, 088, 089, 090, 091, 092, 093, 094, 095, 096, 097, 098, 099, 100,
        101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
    };

    internal static readonly HashSet<ushort> ValidMet_Pt = new(ValidMet_DP)
    {
        63, 79, 85, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125,
    };

    internal static readonly HashSet<ushort> ValidMet_HGSS = new()
    {
        080, 112, 113, 114, 115, 116,
        126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
        141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160,
        161, 162, 163, 164, 165, 166, 167, 168, 169, 170,      172, 173, 174, 175, 176, 177, 178, 179, 180, //171: Route 23 no longer exists
        181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200,
        201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220,
        221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232,      234,                               //233: Pokéwalker
    };

    internal static readonly HashSet<ushort> ValidMet_4 = new(ValidMet_Pt.Concat(ValidMet_HGSS));

    internal static readonly HashSet<ushort> GiftEggLocation4 = new()
    {
        2009, 2010, 2011, 2013, 2014,
    };

    internal static int GetTransfer45MetLocation(PKM pk)
    {
        if (!pk.Gen4 || !pk.FatefulEncounter)
            return Locations.Transfer4; // Pokétransfer

        return pk.Species switch
        {
            243 or 244 or 245 => Locations.Transfer4_CrownUnused, // Beast
            251 => Locations.Transfer4_CelebiUnused, // Celebi
            _ => Locations.Transfer4,
        };
    }
}
