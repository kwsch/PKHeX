using PKHeX.Core;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Drawing.PokeSprite;

internal static class BoxWallpaper
{
    public static bool IsWallpaperRed(GameVersion version, int wallpaperID) => version.GetGeneration() switch
    {
        3 when CXD.Contains(version) => wallpaperID == 7, // flame pattern in XD
        3 => wallpaperID switch
        {
            5 => true, // Volcano
            13 => E == version, // PokéCenter
            _ => false,
        },
        4 => wallpaperID switch
        {
            5 => true, // Volcano
            12 => true, // Checks
            13 => true, // PokéCenter
            22 => true, // Special
            _ => false,
        },
        5 => wallpaperID switch
        {
            5 => true, // Volcano
            12 => true, // Checks
            19 => B2W2.Contains(version), // PWT
            22 => B2W2.Contains(version), // Reshiram
            21 => BW.Contains(version), // Zoroark
            23 => BW.Contains(version), // Musical
            _ => false,
        },
        6 or 7 => wallpaperID switch
        {
            5 => true, // Volcano
            12 => true, // PokéCenter
            20 => true, // Special5 Flare/Magma
            _ => false,
        },
        8 when BDSP.Contains(version) => wallpaperID switch
        {
            6 => true, // Volcano
            15 => true, // Checks
            21 => true, // Trio
            29 => true, // Nostalgic (Platinum)
            30 => true, // Legend (Platinum)
            _ => false,
        },
        8 => true, // Bad contrast with lots of void space, better to just highlight the shiny red.
        _ => false,
    };
}
