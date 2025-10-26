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
        0083, 0084, 0085, 0103, 0107, 0108, 0109, 0214, 0217, 0218,
        0221, 0222, 0230, 0231, 0232, 0233, 0234, 0236, 0237, 0238,
        0239, 0240, 0241, 0242, 0243, 0244, 0245, 0246, 0247, 0248,
        0249, 0250, 0251, 0253, 0266, 0267, 0268, 0270, 0275, 0289,
        0290, 0291, 0292, 0293, 0294, 0296, 0538, 0540, 0564, 0565,
        0566, 0567, 0568, 0569, 0570, 0639, 0640, 0646, 0647, 0710,
        0711, 0795, 0796, 0849, 1124, 1125, 1126, 1127, 1128, 1231,
        1232, 1233, 1234, 1235, 1236, 1237, 1238, 1239, 1240, 1241,
        1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250, 1251,
        1582, 1592, 2401, 2558, 2618, 2619, // Colorful Screw cannot be held.
    ];

    public static ReadOnlySpan<ushort> Treasure => // 3
    [
        0086, 0088, 0089, 0092, 0571, 0581, 0582,
    ];

    public static ReadOnlySpan<ushort> Key => // 4
    [
        0632, 0700, 0765, 0847, 2588, 2589, 2590, 2591, 2592, 2595,
        2596, 2597, 2598, 2599, 2600, 2620, 2621, 2622, 2623, 2624,
        2625, 2626, 2627, 2628, 2629, 2630, 2631, 2632, 2633, 2634,

    ];

    public static ReadOnlySpan<ushort> Berry => // 5
    [
        0149, 0150, 0151, 0152, 0153, 0155, 0156, 0157, 0158, 0169,
        0170, 0171, 0172, 0173, 0174, 0184, 0185, 0186, 0187, 0188,
        0189, 0190, 0191, 0192, 0193, 0194, 0195, 0196, 0197, 0198,
        0199, 0200, 0686,
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
    ];

    public static ReadOnlySpan<ushort> MegaStones => // 7
    [
        0656, 0657, 0658, 0659, 0660, 0661, 0662, 0663, 0665, 0666,
        0667, 0668, 0669, 0670, 0671, 0672, 0673, 0674, 0675, 0676,
        0677, 0678, 0679, 0680, 0681, 0682, 0683, 0754, 0755, 0756,
        0757, 0758, 0759, 0760, 0761, 0762, 0763, 0764, 0767, 0768,
        0769, 0770, 2559, 2560, 2561, 2562, 2563, 2564, 2565, 2566,
        2569, 2570, 2571, 2572, 2573, 2574, 2575, 2576, 2577, 2578,
        2579, 2580, 2581, 2582, 2583, 2584, 2585, 2587,
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
        0005, // Safari Ball
        0499, // Sport Ball (first Season end reward)

        0662, // Mewtwonite X
        0663, // Mewtwonite Y
        0764, // Diancite

        0576, // Dream Ball

        0851, // Beast Ball

        2575, // Chesnaughtite
        2576, // Delphoxite
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

    public static ushort[] GetAllHeld() => [..Medicine, ..Balls, ..Berry, ..MegaStones, ..Treasure, ..Other[..^1]]; // Exclude Colorful Screw

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

    /// <summary>
    /// Retrieves the expected Mega Stone item ID for a given Pokémon species and form.
    /// </summary>
    /// <returns>0 if no item is expected for this species and form combination (in other words, not a mega form).</returns>
    public static ushort GetExpectedMegaStone(ushort species, byte form) => (Species)species switch
    {
        Gengar when form == 1 => 0656,
        Gardevoir when form == 1 => 0657,
        Ampharos when form == 1 => 0658,
        Venusaur when form == 1 => 0659,
        Charizard when form == 1 => 0660, // X
        Blastoise when form == 1 => 0661,
        Mewtwo when form == 1 => 0662, // X
        Mewtwo when form == 2 => 0663, // Y
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
        Clefable when form == 1 => 2559,
        Victreebel when form == 1 => 2560,
        Starmie when form == 1 => 2561,
        Dragonite when form == 1 => 2562,
        Meganium when form == 1 => 2563,
        Feraligatr when form == 1 => 2564,
        Skarmory when form == 1 => 2565,
        Froslass when form == 1 => 2566,
        Emboar when  form == 1 => 2569,
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
        Falinks when form == 1 => 2587,
        _ => 0,
    };
}
