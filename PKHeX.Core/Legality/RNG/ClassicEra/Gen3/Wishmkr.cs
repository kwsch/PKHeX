using System;

namespace PKHeX.Core;

/// <summary>
/// Colosseum Bonus Disc Distribution - 20043 WISHMKR Jirachi
/// </summary>
public static class Wishmkr
{
    /// <summary>
    /// Trainer ID for the WISHMKR Jirachi.
    /// </summary>
    public const ushort TrainerID = 20043;

    /// <summary>
    /// Of the 65536 possible seeds, only 9 give a shiny Jirachi. No duplicate for natures.
    /// </summary>
    /// <remarks>
    /// Index is nature, value is seed. 0 means no shiny for that nature.
    /// </remarks>
    private static ReadOnlySpan<ushort> Seeds =>
    [
        0x0000, 0x7236, 0x0000, 0x0000, 0xA030, 0x0000,
        0xECDD, 0x0000, 0x0000, 0x0000, 0x7360, 0x9359,
        0x3D60, 0xCF37, 0x0000, 0x0000, 0x0000, 0x0000,
        0x353D, 0x0000, 0x0000, 0x0000, 0x0000, 0xF500,
    ];

    /// <summary>
    /// All 9 seeds that generate a shiny Jirachi.
    /// </summary>
    /// <remarks>
    /// <see cref="Seeds"/> for index mapping.
    /// </remarks>
    public static ReadOnlySpan<ushort> All9 =>
    [
        0x7236, // Lonely
        0xA030, // Naughty
        0xECDD, // Docile
        0x7360, // Timid
        0x9359, // Hasty
        0x3D60, // Serious
        0xCF37, // Jolly
        0x353D, // Bashful
        0xF500, // Careful
    ];

    /// <summary>
    /// Gets the shiny seed for the requested nature.
    /// </summary>
    /// <param name="nature">Nature to get the seed for.</param>
    /// <param name="seed">Seed for the nature.</param>
    /// <returns>True if the seed for the requested nature was found.</returns>
    public static bool TryGetSeed(Nature nature, out ushort seed)
    {
        if ((uint)nature >= Seeds.Length)
        {
            seed = 0;
            return false;
        }
        seed = Seeds[(int)nature];
        return seed != 0;
    }
}
