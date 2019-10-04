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

        private static string GetResourceSuffix(GameVersion version, int index)
        {
            switch (version.GetGeneration())
            {
                case 8:
                    return "swsh";
                case 7 when !GG.Contains(version):
                    return "xy";
                case 6:
                    return ORAS.Contains(version) && index > 16 ? "ao" : "xy";
                case 5:
                    return B2W2.Contains(version) && index > 16 ? "b2w2" : "bw";
                case 4:
                    if (index > 16)
                    {
                        if (Pt == version)
                            return "pt";
                        if (HGSS.Contains(version))
                            return "hgss";
                    }
                    return "dp";
                case 3:
                    if (E == version)
                        return "e";
                    else if (FRLG.Contains(version) && index > 12)
                        return "frlg";
                    else
                        return "rs";
                default:
                    return string.Empty;
            }
        }

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
                case 6:
                case 7:
                    return wallpaperID switch
                    {
                        5 => true, // Volcano
                        12 => true, // PokéCenter
                        20 => true, // Special5 Flare/Magma
                        _ => false
                    };
                case 8: // todo swsh
                    return true;
                default:
                    return false;
            }
        }
    }
}
