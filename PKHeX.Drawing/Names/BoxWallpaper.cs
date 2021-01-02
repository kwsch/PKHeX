using PKHeX.Core;

using static PKHeX.Core.GameVersion;

namespace PKHeX.Drawing
{
    /// <summary>
    /// Retrieves Box Storage wallpaper metadata.
    /// </summary>
    public static class BoxWallpaper
    {
        public static string GetWallpaperResourceName(GameVersion version, int index)
        {
            index++; // start indexes at 1
            var suffix = GetResourceSuffix(version, index);
            return $"box_wp{index:00}{suffix}";
        }

        private static string GetResourceSuffix(GameVersion version, int index) => version.GetGeneration() switch
        {
            3 when version == E => "e",
            3 when FRLG.Contains(version) && index > 12 => "frlg",
            3 => "rs",

            4 when index < 16 => "dp",
            4 when version == Pt => "pt",
            4 when HGSS.Contains(version) => "hgss",

            5 => B2W2.Contains(version) && index > 16 ? "b2w2" : "bw",
            6 => ORAS.Contains(version) && index > 16 ? "ao" : "xy",
            7 when !GG.Contains(version) => "xy",
            8 => "swsh",
            _ => string.Empty
        };

        public static bool IsWallpaperRed(GameVersion version, int wallpaperID)
        {
            switch (version.GetGeneration())
            {
                case 3:
                    if (CXD.Contains(version))
                        return wallpaperID == 7; // flame pattern in XD

                    return wallpaperID switch
                    {
                        5 => true, // Volcano
                        13 => E == version, // PokéCenter
                        _ => false,
                    };
                case 4:
                    return wallpaperID switch
                    {
                        5 => true, // Volcano
                        12 => true, // Checks
                        13 => true, // PokéCenter
                        22 => true, // Special
                        _ => false
                    };
                case 5:
                    return wallpaperID switch
                    {
                        5 => true, // Volcano
                        12 => true, // Checks
                        19 => B2W2.Contains(version), // PWT
                        22 => B2W2.Contains(version), // Reshiram
                        21 => BW.Contains(version), // Zoroark
                        23 => BW.Contains(version), // Musical
                        _ => false
                    };
                case 6 or 7:
                    return wallpaperID switch
                    {
                        5 => true, // Volcano
                        12 => true, // PokéCenter
                        20 => true, // Special5 Flare/Magma
                        _ => false
                    };
                case 8:
                    return wallpaperID switch
                    {
                        _ => true, // Bad contrast with lots of void space, better to just highlight the shiny red.
                    };
                default:
                    return false;
            }
        }
    }
}
