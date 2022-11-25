using PKHeX.Core;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Drawing.PokeSprite;

internal static class BoxWallpaper
{
    public static bool IsWallpaperRed(this SaveFile sav, int box) => IsWallpaperRed(sav.Context, sav.Version, sav.GetBoxWallpaper(box));

    public static bool IsWallpaperRed(EntityContext context, GameVersion version, int wallpaperID) => context switch
    {
        Gen3 when GameVersion.CXD.Contains(version) => wallpaperID == 7, // flame pattern in XD
        Gen3 => wallpaperID switch
        {
            5 => true, // Volcano
            13 => version == GameVersion.E, // PokéCenter
            _ => false,
        },
        Gen4 => wallpaperID switch
        {
            5 => true, // Volcano
            12 => true, // Checks
            13 => true, // PokéCenter
            22 => true, // Special
            _ => false,
        },
        Gen5 => wallpaperID switch
        {
            5 => true, // Volcano
            12 => true, // Checks
            19 => GameVersion.B2W2.Contains(version), // PWT
            22 => GameVersion.B2W2.Contains(version), // Reshiram
            21 => GameVersion.BW.Contains(version), // Zoroark
            23 => GameVersion.BW.Contains(version), // Musical
            _ => false,
        },
        Gen6 or Gen7 => wallpaperID switch
        {
            5 => true, // Volcano
            12 => true, // PokéCenter
            20 => true, // Special5 Flare/Magma
            _ => false,
        },
        Gen8b => wallpaperID switch
        {
            6 => true, // Volcano
            15 => true, // Checks
            21 => true, // Trio
            29 => true, // Nostalgic (Platinum)
            30 => true, // Legend (Platinum)
            _ => false,
        },
        Gen8 or Gen9 => true, // Bad contrast with lots of void space, better to just highlight the shiny red.
        _ => false,
    };
}
