using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public sealed class ItemStorage9ZA : IItemStorage
{
    public static readonly ItemStorage9ZA Instance = new();

    public static ReadOnlySpan<ushort> Medicine => // 0
    [
        0017, 0018, 0019, 0020, 0021, 0022, 0023, 0024, 0025, 0026,
        0027, 0028, 0029, 0030, 0031, 0032, 0033, 0708,
                                                        2684,
    ];

    public static ReadOnlySpan<ushort> Balls => // 1
    [
        0001, 0002, 0003, 0004, 0005, 0006, 0007, 0008, 0009, 0010,
        0011, 0012, 0013, 0014, 0015, 0016, 0492, 0493, 0494, 0495,
        0496, 0497, 0498, 0499, 0576, 0851,
    ];

    public static ReadOnlySpan<ushort> Other => // 2 (Items)
    [
        0045, 0046, 0047, 0048, 0049, 0050, 0052, 0080, 0081, 0082,
        0083, 0084, 0085, 0103, 0107, 0108, 0109, 0116, 0117, 0118,
        0119, 0214, 0217, 0218, 0221, 0222, 0230, 0231, 0232, 0233,
        0234, 0236, 0237, 0238, 0239, 0240, 0241, 0242, 0243, 0244,
        0245, 0246, 0247, 0248, 0249, 0250, 0251, 0252, 0253, 0258,
        0259, 0266, 0267, 0268, 0270, 0275, 0289, 0290, 0291, 0292,
        0293, 0294, 0296, 0324, 0534, 0535, 0537, 0538, 0540, 0564,
        0565, 0566, 0567, 0568, 0569, 0570, 0639, 0640, 0646, 0647,
        0710, 0711, 0795, 0796, 0849, 1124, 1125, 1126, 1127, 1128,
        1231, 1232, 1233, 1234, 1235, 1236, 1237, 1238, 1239, 1240,
        1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250,
        1251, 1582, 1592, 1691, 1861,       2344, 2401, 2558, 2618,

        2137, // Gimmighoul Coin cannot be held.
        2619, // Colorful Screw cannot be held.
    ];

    public static ReadOnlySpan<ushort> Treasure => // 3
    [
        0086, 0088, 0089, 0092, 0571, 0581, 0582,
    ];

    public static ReadOnlySpan<ushort> Key => // 4
    [
        0632, 0700, 0765, 0847, 1278, 2588, 2589, 2590, 2591, 2592,
        2595, 2596, 2597, 2598, 2599, 2600, 2601, 2602, 2603, 2604,
        2605, 2606, 2607, 2608, 2609, 2610, 2611, 2612, 2613, 2620,
        2621, 2622, 2623, 2624, 2625, 2626, 2627, 2628, 2629, 2630,
        2631, 2632, 2633, 2634,

    ];

    public static ReadOnlySpan<ushort> Berry => // 5
    [
        0149, 0150, 0151, 0152, 0153, 0155, 0156, 0157, 0158, 0169,
        0170, 0171, 0172, 0173, 0174, 0184, 0185, 0186, 0187, 0188,
        0189, 0190, 0191, 0192, 0193, 0194, 0195, 0196, 0197, 0198,
        0199, 0200, 0686,
                          2651, 2652, 2653, 2654, 2655, 2656, 2657,
        2658, 2659, 2660, 2661, 2662, 2663, 2664, 2665, 2666, 2667,
        2668, 2669, 2670, 2671, 2672, 2673, 2674, 2675, 2676, 2677,
        2678, 2679, 2680, 2681, 2682, 2683,
    ];

    public static ReadOnlySpan<ushort> TM => // 6
    [
        0328, 0329, 0330, 0331, 0332, 0333, 0334, 0335, 0336, 0337,
        0338, 0339, 0340, 0341, 0342, 0343, 0344, 0345, 0346, 0347,
        0348, 0349, 0350, 0351, 0352, 0353, 0354, 0355, 0356, 0357,
        0358, 0359, 0360, 0361, 0362, 0363, 0364, 0365, 0366, 0367,
        0368, 0369, 0370, 0371, 0372, 0373, 0374, 0375, 0376, 0377,
        0378, 0379, 0380, 0381, 0382, 0383, 0384, 0385, 0386, 0387,
        0388, 0389, 0390, 0391, 0392, 0393, 0394, 0395, 0396, 0397,
        0398, 0399, 0400, 0401, 0402, 0403, 0404, 0405, 0406, 0407,
        0408, 0409, 0410, 0411, 0412, 0413, 0414, 0415, 0416, 0417,
        0418, 0419, 0618, 0619, 0620, 0690, 0691, 0692, 0693, 2160,
        2162, 2163, 2164, 2165, 2166, 2167, 2168,
                                                  2169, 2170, 2171,
        2172, 2173, 2174, 2175, 2176, 2177, 2178, 2179, 2180, 2181,
        2182, 2183, 2184, 2185, 2186, 2187, 2188, 2189, 2190, 2191,
        2192, 2193, 2194, 2195, 2196, 2197, 2198, 2199, 2200, 2201,
        2202, 2203, 2204, 2205, 2206, 2207, 2208, 2209, 2210, 2211,
        2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219, 2220, 2221,

    ];

    public static ReadOnlySpan<ushort> MegaStones => // 7
    [
        0656, 0657, 0658, 0659, 0660, 0661, 0662, 0663, 0664, 0665,
        0666, 0667, 0668, 0669, 0670, 0671, 0672, 0673, 0674, 0675,
        0676, 0677, 0678, 0679, 0680, 0681, 0682, 0683, 0684, 0685,
        0752, 0753, 0754, 0755, 0756, 0757, 0758, 0759, 0760, 0761,
        0762, 0763, 0764, 0767, 0768, 0769, 0770, 2559, 2560, 2561,
        2562, 2563, 2564, 2565, 2566, 2567, 2568, 2569, 2570, 2571,
        2572, 2573, 2574, 2575, 2576, 2577, 2578, 2579, 2580, 2581,
        2582, 2583, 2584, 2585, 2586, 2587, 2635, 2636, 2637, 2638,
        2639, 2640, 2641, 2642, 2643, 2644, 2645, 2646, 2647, 2648,
        2649, 2650,
    ];

    internal static ReadOnlySpan<InventoryType> ValidTypes =>
    [
        // Display Order
        InventoryType.Medicine,
        InventoryType.Balls,
        InventoryType.Berries,
        InventoryType.Items, // Other
        InventoryType.TMHMs,
        InventoryType.MegaStones,
        InventoryType.Treasure,
        InventoryType.KeyItems,
    ];

    public static ReadOnlySpan<ushort> Unreleased =>
    [
        0016, // Cherish Ball

        0664, // Blazikenite
        0752, // Swampertite
        0753, // Sceptilite

        2640, // Garchompite Z
    ];

    public int GetMax(InventoryType type) => type switch
    {
        InventoryType.Medicine => 999,
        InventoryType.Balls => 999,
        InventoryType.Berries => 999,
        InventoryType.Items => 999, // Other
        InventoryType.TMHMs => 1,
        InventoryType.MegaStones => 1,
        InventoryType.Treasure => 999,
        InventoryType.KeyItems => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => !Unreleased.Contains((ushort)itemIndex);

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => GetLegal(type);

    public static ReadOnlySpan<ushort> GetLegal(InventoryType type) => type switch
    {
        InventoryType.Medicine => Medicine,
        InventoryType.Balls => Balls,
        InventoryType.Berries => Berry,
        InventoryType.Items => Other,
        InventoryType.TMHMs => TM,
        InventoryType.MegaStones => MegaStones,
        InventoryType.Treasure => Treasure,
        InventoryType.KeyItems => Key,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public static ushort[] GetAllHeld() => [..Medicine, ..Balls, ..Berry, ..MegaStones, ..Treasure, ..Other[..^2]]; // Exclude Colorful Screw and Gimmighoul Coin

    public static InventoryType GetInventoryPouch(ushort itemIndex)
    {
        foreach (var type in ValidTypes)
        {
            var legal = GetLegal(type);
            if (legal.Contains(itemIndex))
                return type;
        }
        return InventoryType.None;
    }

    public static bool IsMegaStone(ushort item) => MegaStones.Contains(item);
    public static bool IsUniqueHeldItem(ushort item) => IsMegaStone(item) || item is (0534 or 0535); // Primal Orbs
    public static ushort[] GetAllUniqueHeldItems() => [..MegaStones, 0534, 0535];

    /// <summary>
    /// Retrieves the expected Mega Stone or Primal Orb item ID for a given Pokémon species and form.
    /// </summary>
    /// <returns>0 if no item is expected for this species and form combination (in other words, not a Mega Evolution or Primal Reversion).</returns>
    public static ushort GetExpectedMegaStoneOrPrimalOrb(ushort species, byte form) => (Species)species switch
    {
        // Primal Reversions
        Groudon when form == 1 => 0534,
        Kyogre when form == 1 => 0535,

        // X/Y Mega Evolutions
        Gengar when form == 1 => 0656,
        Gardevoir when form == 1 => 0657,
        Ampharos when form == 1 => 0658,
        Venusaur when form == 1 => 0659,
        Charizard when form == 1 => 0660, // X
        Blastoise when form == 1 => 0661,
        Mewtwo when form == 1 => 0662, // X
        Mewtwo when form == 2 => 0663, // Y
        Blaziken when form == 1 => 0664,
        Medicham when form == 1 => 0665,
        Houndoom when form == 1 => 0666,
        Aggron when form == 1 => 0667,
        Banette when form == 1 => 0668,
        Tyranitar when form == 1 => 0669,
        Scizor when form == 1 => 0670,
        Pinsir when form == 1 => 0671,
        Aerodactyl when form == 1 => 0672,
        Lucario when form == 1 => 0673,
        Abomasnow when form == 1 => 0674,
        Kangaskhan when form == 1 => 0675,
        Gyarados when form == 1 => 0676,
        Absol when form == 1 => 0677,
        Charizard when form == 2 => 0678, // Y
        Alakazam when form == 1 => 0679,
        Heracross when form == 1 => 0680,
        Mawile when form == 1 => 0681,
        Manectric when form == 1 => 0682,
        Garchomp when form == 1 => 0683,
        Latias when form == 1 => 0684,
        Latios when form == 1 => 0685,

        // OR/AS Mega Evolutions
        Swampert when form == 1 => 0752,
        Sceptile when form == 1 => 0753,
        Sableye when form == 1 => 0754,
        Altaria when form == 1 => 0755,
        Gallade when form == 1 => 0756,
        Audino when form == 1 => 0757,
        Metagross when form == 1 => 0758,
        Sharpedo when form == 1 => 0759,
        Slowbro when form == 1 => 0760,
        Steelix when form == 1 => 0761,
        Pidgeot when form == 1 => 0762,
        Glalie when form == 1 => 0763,
        Diancie when form == 1 => 0764,
        Camerupt when form == 1 => 0767,
        Lopunny when form == 1 => 0768,
        Salamence when form == 1 => 0769,
        Beedrill when form == 1 => 0770,

        // Z-A Mega Evolutions
        Clefable when form == 1 => 2559,
        Victreebel when form == 1 => 2560,
        Starmie when form == 1 => 2561,
        Dragonite when form == 1 => 2562,
        Meganium when form == 1 => 2563,
        Feraligatr when form == 1 => 2564,
        Skarmory when form == 1 => 2565,
        Froslass when form == 1 => 2566,
        Heatran when form == 1 => 2567,
        Darkrai when form == 1 => 2568,
        Emboar when form == 1 => 2569,
        Excadrill when form == 1 => 2570,
        Scolipede when form == 1 => 2571,
        Scrafty when form == 1 => 2572,
        Eelektross when form == 1 => 2573,
        Chandelure when form == 1 => 2574,
        Chesnaught when form == 1 => 2575,
        Delphox when form == 1 => 2576,
        Greninja when form == 3 => 2577,
        Pyroar when form == 1 => 2578,
        Floette when form == 6 => 2579,
        Malamar when form == 1 => 2580,
        Barbaracle when form == 1 => 2581,
        Dragalge when form == 1 => 2582,
        Hawlucha when form == 1 => 2583,
        Zygarde when form == 5 => 2584,
        Drampa when form == 1 => 2585,
        Zeraora when form == 1 => 2586,
        Falinks when form == 1 => 2587,

        // Mega Dimension Mega Evolutions
        Raichu when form == 2 => 2635, // X
        Raichu when form == 3 => 2636, // Y
        Chimecho when form == 1 => 2637,
        Absol when form == 2 => 2638, // Z
        Staraptor when form == 1 => 2639,
        Garchomp when form == 2 => 2640, // Z
        Lucario when form == 2 => 2641, // Z
        Golurk when form == 1 => 2642,
        Meowstic when form == 2 => 2643,
        Crabominable when form == 1 => 2644,
        Golisopod when form == 1 => 2645,
        Magearna when form >= 2 => 2646,
        Scovillain when form == 1 => 2647,
        Baxcalibur when form == 1 => 2648,
        Tatsugiri when form >= 3 => 2649,
        Glimmora when form == 1 => 2650,
        _ => 0,
    };
}
