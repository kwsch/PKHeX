using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Retrieves Box Storage wallpaper metadata.
    /// </summary>
    public static class BoxWallpaper
    {
        public static string GetWallpaperResourceName(GameVersion version, int index)
        {
            index++;
            var suffix = GetResourceSuffix(version, index);
            return $"box_wp{index:00}{suffix}";
        }

        private static string GetResourceSuffix(GameVersion version, int index)
        {
            switch (version.GetGeneration())
            {
                case 7:
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
                    switch (wallpaperID)
                    {
                        case 5: // Volcano
                            return true;
                        case 13: // PokéCenter
                            return E == version;
                        default:
                            return false;
                    }
                case 4:
                    switch (wallpaperID)
                    {
                        case 5: // Volcano
                        case 12: // Checks
                        case 13: // PokéCenter
                        case 22: // Special
                            return true;
                        default:
                            return false;
                    }
                case 5:
                    switch (wallpaperID)
                    {
                        case 5: // Volcano
                        case 12: // Checks
                            return true;
                        case 19: // PWT
                        case 22: // Reshiram
                            return B2W2.Contains(version);
                        case 21: // Zoroark
                        case 23: // Musical
                            return BW.Contains(version);
                        default:
                            return false;
                    }
                case 6:
                case 7:
                    switch (wallpaperID)
                    {
                        case 5: // Volcano
                        case 12: // PokéCenter
                        case 20: // Special5 Flare/Magma
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }
    }
}
