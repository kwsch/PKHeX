using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Drawing.Misc;

public static class WallpaperUtil
{
    private static Bitmap DefaultWallpaper => Resources.box_wp16xy;

    public static Bitmap WallpaperImage(this SaveFile sav, int box) => GetWallpaper(sav, box);

    private static Bitmap GetWallpaper(SaveFile sav, int box)
    {
        if (sav is not IBoxDetailWallpaper wp)
            return DefaultWallpaper;

        int wallpaper = wp.GetBoxWallpaper(box);
        string s = GetWallpaperResourceName(sav.Version, wallpaper);
        return (Bitmap?)Resources.ResourceManager.GetObject(s) ?? DefaultWallpaper;
    }

    public static string GetWallpaperResourceName(GameVersion version, int index)
    {
        index++; // start indexes at 1
        var suffix = GetResourceSuffix(version, index);
        var variant = version switch
        {
            SL when index is 20 => "_n", // Naranja
            VL when index is 20 => "_u", // Uva
            _ => string.Empty,
        };

        return $"box_wp{index:00}{suffix}{variant}";
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
        8 when !SWSH.Contains(version) => "bdsp",
        8 => "swsh",
        9 => "sv",
        _ => string.Empty,
    };
}
