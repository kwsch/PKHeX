using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

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
    
    private static Dictionary<ushort, ushort> GetDictionary(IReadOnlyList<ushort> key, IReadOnlyList<ushort> held)
    {
        var result = new Dictionary<ushort, ushort>(held.Count);
        for (int i = 0; i < key.Count; i++)
            result.Add(key[i], held[i]);
        return result;
    }

    internal static readonly ushort[] HeldItems_SM = ItemStorage7SM.GetAllHeld();
    internal static readonly ushort[] HeldItems_USUM = ItemStorage7USUM.GetAllHeld();

    internal static readonly HashSet<ushort> AlolanOriginForms = new()
    {
        (int)Rattata,
        (int)Raticate,
        (int)Sandshrew,
        (int)Sandslash,
        (int)Vulpix,
        (int)Ninetales,
        (int)Diglett,
        (int)Dugtrio,
        (int)Meowth,
        (int)Persian,
        (int)Geodude,
        (int)Graveler,
        (int)Golem,
        (int)Grimer,
        (int)Muk,
    };

    internal static readonly HashSet<ushort> AlolanVariantEvolutions12 = new()
    {
        (int)Raichu,
        (int)Exeggutor,
        (int)Marowak,
    };

    public static readonly HashSet<ushort> PastGenAlolanNatives = new()
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

        100, 101, // Voltorb & Electrode
    };

    internal static readonly HashSet<ushort> Totem_Alolan = new()
    {
        (int)Raticate, // (Normal, Alolan, Totem)
        (int)Marowak, // (Normal, Alolan, Totem)
        (int)Mimikyu, // (Normal, Busted, Totem, Totem_Busted)
    };

    internal static readonly HashSet<ushort> Totem_NoTransfer = new()
    {
        (int)Marowak,
        (int)Araquanid,
        (int)Togedemaru,
        (int)Ribombee,
    };

    internal static readonly HashSet<ushort> Totem_USUM = new()
    {
        (int)Raticate,
        (int)Gumshoos,
        //(int)Wishiwashi,
        (int)Salazzle,
        (int)Lurantis,
        (int)Vikavolt,
        (int)Mimikyu,
        (int)Kommoo,
        (int)Marowak,
        (int)Araquanid,
        (int)Togedemaru,
        (int)Ribombee,
    };

    internal static readonly HashSet<ushort> ValidMet_SM = new()
    {
                       006, 008,
        010, 012, 014, 016, 018,
        020, 022, 024, 026, 028,
        030, 032, 034, 036, 038,
        040, 042, 044, 046, 048,
        050, 052, 054, 056, 058,
        060, 062, 064,      068,
        070, 072, 074, 076, 078,
             082, 084, 086, 088,
        090, 092, 094,
        100, 102, 104, 106, 108,
        110, 112, 114, 116, 118,
        120, 122, 124, 126, 128,
        130, 132, 134, 136, 138,
        140, 142, 144, 146, 148,
        150, 152, 154, 156, 158,
        160, 162, 164, 166, 168,
        170, 172, 174, 176, 178,
        180, 182, 184, 186, 188,
        190, 192,

        Locations.Pelago7, // 30016
    };

    internal static readonly HashSet<ushort> ValidMet_USUM = new(ValidMet_SM)
    {
        // 194, 195, 196, 197, // Unobtainable new Locations
                            198,
        200, 202, 204, 206, 208,
        210, 212, 214, 216, 218,
        220, 222, 224, 226, 228,
        230, 232,
    };

    #region Unreleased Items
    internal static readonly bool[] ReleasedHeldItems_7 = GetPermitList(MaxItemID_7_USUM, HeldItems_USUM, ItemStorage7SM.Unreleased);
    #endregion
}
