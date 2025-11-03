using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Provides functionality for managing and interacting with "Colorful Screws" in the game.
/// </summary>
/// <remarks>This class includes methods for collecting screws, updating inventory, and retrieving the locations
/// of screws. It operates on the provided save data and event work flag storage to manage the state of
/// screws.</remarks>
public static class ColorfulScrew9a
{
    public const ushort ColorfulScrewItemIndex = 2619;

    /// <summary>
    /// Collects screws from the specified save data and updates the player's inventory with the collected count.
    /// </summary>
    /// <returns>The number of screws collected. Returns 0 if no screws were found.</returns>
    public static int CollectScrews(SAV9ZA sav)
    {
        // Collect Screws
        var block = sav.Blocks.FieldItems;
        var count = CollectScrews(block);
        if (count == 0)
            return count;

        // Update Inventory with updated quantity.
        var items = sav.Items;
        var exist = items.GetItemQuantity(ColorfulScrewItemIndex);

        var update = (ushort)(exist + count);
        if (update > 100)
            update = 100; // Why did the player cheat in extra quantity? Just be safe.

        items.SetItemQuantity(ColorfulScrewItemIndex, update);
        return count;
    }

    private static int CollectScrews(EventWorkFlagStorage block)
    {
        // "Collect" the screw by marking the overworld item-ball spawner as inactive (hidden = true).
        int count = 0;
        foreach (var screw in ColorfulScrews)
        {
            var hash = FnvHash.HashFnv1a_64(screw.FieldItem);
            var index = block.GetIndex(hash);
            if (index == -1)
                continue; // Shouldn't happen. All screws should be populated in a save file.

            var value = block.GetValue(index);
            if (value)
                continue; // Already collected.

            block.SetValue(index, true);
            count++;
        }
        // Return the count of screws collected, that should be added to the player's inventory.
        return count;
    }

    /// <inheritdoc cref="GetScrewLocations(EventWorkFlagStorage, bool)"/>
    public static IEnumerable<(string FieldItem, (float X, float Y, float Z) Point)> GetScrewLocations(SAV9ZA sav, bool state) =>
        GetScrewLocations(sav.Blocks.FieldItems, state);

    /// <summary>
    /// Retrieves the locations of screws that match the specified state.
    /// </summary>
    /// <param name="block">The storage block containing screw data and their associated states.</param>
    /// <param name="state">The state to filter screws by. Only screws with this state will be included.</param>
    public static IEnumerable<(string FieldItem, (float X, float Y, float Z) Point)> GetScrewLocations(EventWorkFlagStorage block, bool state = false)
    {
        foreach (var screw in ColorfulScrews)
        {
            var hash = FnvHash.HashFnv1a_64(screw.FieldItem);
            var index = block.GetIndex(hash);
            if (index == -1)
                continue; // Shouldn't happen. All screws should be populated in a save file.
            var value = block.GetValue(index);
            if (value != state)
                continue;
            yield return screw;
        }
    }

    /// <summary>
    /// Locations of all Colorful Screws in the game.
    /// </summary>
    private static readonly (string FieldItem, (float X, float Y, float Z) Point)[] ColorfulScrews =
    [
        // 99 screws outside.
        ("itb_a0101_25", (-611.7629f ,0.2509685f ,-594.8188f)),
        ("itb_a0101_81", (-912.0365f ,9.002547f ,-750.05414f)),
        ("itb_a0101_83", (-846.563f ,4.1388083f ,-811.1325f)),
        ("itb_a0101_84", (-848.9131f ,12.781971f ,-737.83496f)),
        ("itb_a0101_89", (-681.5279f ,17.51269f ,-591.0863f)),
        ("itb_a0101_94", (-619.4882f ,9.134821f ,-554.0143f)),
        ("itb_a0101_97", (-687.6974f ,18.378315f ,-627.687f)),
        ("itb_a0101_99", (-575.8895f ,4.3793745f ,-552.3689f)),
        ("itb_a0101_101", (-751.99054f ,5.4620757f ,-732.8448f)),
        ("itb_a0101_103", (-921.3577f ,11.460509f ,-677.5147f)),
        ("itb_a0101_105", (-823.5497f ,17.546358f ,-783.28516f)),
        ("itb_a0101_106", (-710.679f ,27.860916f ,-843.5154f)),
        ("itb_a0101_114", (-734.02185f ,23.055899f ,-713.5946f)),
        ("itb_a0102_40", (-487.75266f ,6.8545384f ,-947.9376f)),
        ("itb_a0102_87", (-535.5792f ,20.537054f ,-730.7836f)),
        ("itb_a0102_108", (-612.5779f ,6.74156f ,-840.2996f)),
        ("itb_a0102_117", (-613.5178f ,7.555528f ,-759.1486f)),
        ("itb_a0102_120", (-574.815f ,18.27181f ,-826.5897f)),
        ("itb_a0102_122", (-554.20825f ,12.109058f ,-731.9376f)),
        ("itb_a0102_128", (-546.3892f ,3.1391847f ,-755.3944f)),
        ("itb_a0102_130", (-560.5767f ,14.321938f ,-679.1879f)),
        ("itb_a0102_131", (-687.17236f ,4.5935855f ,-831.19507f)),
        ("itb_a0201_58", (-366.6956f ,15.044577f ,-831.9763f)),
        ("itb_a0201_61", (-406.0901f ,11.309174f ,-975.0717f)),
        ("itb_a0201_64", (-356.07993f ,8.858809f ,-902.7882f)),
        ("itb_a0201_67", (-323.23798f ,12.752846f ,-773.8171f)),
        ("itb_a0201_70", (-334.69022f ,19.0464f ,-754.7411f)),
        ("itb_a0201_73", (-481.9776f ,7.3682528f ,-690.87823f)),
        ("itb_a0201_75", (-431.6641f ,10.888694f ,-756.9131f)),
        ("itb_a0201_78", (-484.75323f ,17.47603f ,-923.1219f)),
        ("itb_a0201_79", (-311.2337f ,18.81803f ,-785.0195f)),
        ("itb_a0201_80", (-404.22302f ,4.299103f ,-976.53735f)),
        ("itb_a0201_81", (-331.591f ,17.176617f ,-910.36633f)),
        ("itb_a0202_51", (-298.79434f ,18.605236f ,-643.80164f)),
        ("itb_a0202_89", (-113.88871f ,12.77f ,-797.11523f)),
        ("itb_a0202_94", (-45.78526f ,0.14146906f ,-674.23206f)),
        ("itb_a0202_96", (-302.68326f ,21.765139f ,-744.53894f)),
        ("itb_a0202_99", (-190.47188f ,3.3071632f ,-706.84216f)),
        ("itb_a0202_104", (-188.0446f ,10.365694f ,-705.0904f)),
        ("itb_a0301_12", (-133.65852f ,9.468117f ,-557.42645f)),
        ("itb_a0301_85", (-22.70829f ,5.66999f ,-422.1258f)),
        ("itb_a0301_87", (-55.555252f ,15.3061905f ,-501.03577f)),
        ("itb_a0301_90", (-390.6782f ,4.24647f ,-525.0898f)),
        ("itb_a0301_93", (-139.23645f ,14.698685f ,-433.89874f)),
        ("itb_a0301_96", (-139.34392f ,2.9998174f ,-430.98022f)),
        ("itb_a0301_101", (-212.95015f ,0.16039793f ,-447.62796f)),
        ("itb_a0301_102", (-55.400597f ,6.0117397f ,-501.29865f)),
        ("itb_a0301_106", (-111.60038f ,30.824907f ,-529.86273f)),
        ("itb_a0302_101", (-153.0837f ,24.55992f ,-206.02f)),
        ("itb_a0302_81", (-124.19311f ,12.239148f ,-215.39864f)),
        ("itb_a0302_87", (-107.24101f ,13.461311f ,-218.01112f)),
        ("itb_a0302_92", (-113.791084f ,1.5038356f ,-324.2796f)),
        ("itb_a0302_94", (-400.23975f ,9.024075f ,-407.45197f)),
        ("itb_a0302_96", (-180.98009f ,25.06243f ,-191.85275f)),
        ("itb_a0302_104", (-174.84079f ,15.428678f ,-321.31717f)),
        ("itb_a0401_17", (-453.57413f ,17.11317f ,-213.88242f)),
        ("itb_a0401_29", (-362.32007f ,13.29676f ,-162.73178f)),
        ("itb_a0401_31", (-314.40524f ,26.549751f ,-164.79158f)),
        ("itb_a0401_63", (-316.57678f ,23.494171f ,-220.47177f)),
        ("itb_a0401_71", (-347.9144f ,19.847963f ,-74.24183f)),
        ("itb_a0401_75", (-267.1446f ,7.992974f ,-78.38538f)),
        ("itb_a0401_77", (-258.93777f ,12.331233f ,-81.74573f)),
        ("itb_a0401_79", (-485.23636f ,3.8256838f ,-108.836044f)),
        ("itb_a0401_80", (-382.20947f ,11.210755f ,-306.4857f)),
        ("itb_a0401_83", (-388.05774f ,12.896254f ,-287.38943f)),
        ("itb_a0401_85", (-447.53406f ,13.137535f ,-324.10864f)),
        ("itb_a0401_88", (-460.5163f ,2.1669445f ,-340.6761f)),
        ("itb_a0401_91", (-419.12234f ,3.9772782f ,-111.23066f)),
        ("itb_a0401_93", (-476.40057f ,19.878263f ,-119.772644f)),
        ("itb_a0401_96", (-394.24576f ,23.499132f ,-279.23523f)),
        ("itb_a0402_68", (-588.6553f ,8.951744f ,-333.55008f)),
        ("itb_a0402_72", (-722.8337f ,8.128692f ,-172.50806f)),
        ("itb_a0402_74", (-666.81683f ,14.745333f ,-131.40936f)),
        ("itb_a0402_77", (-671.4622f ,7.892555f ,-104.47086f)),
        ("itb_a0402_81", (-572.95905f ,11.094018f ,-22.257357f)),
        ("itb_a0402_85", (-550.1979f ,7.9077163f ,-21.212116f)),
        ("itb_a0402_90", (-609.4243f ,25.186445f ,-51.041454f)),
        ("itb_a0402_91", (-608.473f ,5.526315f ,-273.13983f)),
        ("itb_a0402_92", (-666.3514f ,3.4637678f ,-115.272934f)),
        ("itb_a0402_97", (-637.05756f ,14.554916f ,-233.7284f)),
        ("itb_a0501_77", (-915.48694f ,15.225559f ,-293.09912f)),
        ("itb_a0501_81", (-663.87964f ,9.650265f ,-294.52002f)),
        ("itb_a0501_84", (-629.3896f ,20.765638f ,-396.69833f)),
        ("itb_a0501_87", (-891.1155f ,15.639636f ,-270.57614f)),
        ("itb_a0501_91", (-742.5943f ,3.4883833f ,-273.4928f)),
        ("itb_a0501_99", (-791.7427f ,26.136696f ,-315.88742f)),
        ("itb_a0501_100", (-661.34973f ,25.947884f ,-397.19485f)),
        ("itb_a0501_102", (-710.76935f ,6.968615f ,-351.74802f)),
        ("itb_a0501_103", (-611.01355f ,28.322855f ,-369.91714f)),
        ("itb_a0501_104", (-851.59247f ,22.666864f ,-211.58481f)),
        ("itb_a0502_87", (-762.0602f ,15.46941f ,-495.2859f)),
        ("itb_a0502_89", (-862.32495f ,32.34224f ,-456.9059f)),
        ("itb_a0502_95", (-853.2598f ,25.766558f ,-493.55304f)),
        ("itb_a0502_96", (-647.7033f ,3.0009127f ,-473.63446f)),
        ("itb_a0502_97", (-934.56256f ,22.475294f ,-444.34705f)),
        ("itb_a0502_98", (-800.14276f ,20.435688f ,-561.7759f)),
        ("itb_a0502_109", (-833.0822f ,29.57602f ,-451.5477f)),
        ("itb_a0502_110", (-683.0301f ,26.340189f ,-482.3554f)),
        ("itb_a0502_111", (-923.0192f ,23.67176f ,-419.16327f)),

        // 100th screw is indoors.
        ("itb_t1_i011_01", (-1.736374f ,0.036360174f ,-10.29498f)),
    ];

    /// <summary>
    /// Utility function to set all screws to collected or uncollected state.
    /// </summary>
    public static void SetAllScrews(SAV9ZA sav, bool collected = false)
    {
        foreach (var screw in ColorfulScrews)
        {
            var hash = FnvHash.HashFnv1a_64(screw.FieldItem);
            var index = sav.Blocks.FieldItems.GetIndex(hash);
            if (index == -1)
                continue; // Shouldn't happen. All screws should be populated in a save file.
            sav.Blocks.FieldItems.SetValue(index, collected);
        }
    }
}
