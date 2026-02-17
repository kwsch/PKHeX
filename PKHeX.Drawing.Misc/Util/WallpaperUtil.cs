using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving and composing wallpaper images for Pokémon storage boxes.
/// </summary>
public static class WallpaperUtil
{
    private static Bitmap DefaultWallpaper => Resources.box_wp16xy;

    /// <summary>
    /// Gets the wallpaper image for the specified save file and box index.
    /// </summary>
    /// <param name="sav">The save file to get the wallpaper for.</param>
    /// <param name="box">The box index.</param>
    /// <returns>A <see cref="Bitmap"/> representing the wallpaper image.</returns>
    public static Bitmap WallpaperImage(this SaveFile sav, int box) => GetWallpaper(sav, box);

    private static Bitmap GetWallpaper(SaveFile sav, int box)
    {
        if (sav is not IBoxDetailWallpaper wp)
            return DefaultWallpaper;

        // City box wallpaper for Lumiose City
        if (sav is SAV9ZA)
            return Resources.box_wp02bdsp;

        int wallpaper = wp.GetBoxWallpaper(box);
        string s = GetWallpaperResourceName(sav.Version, wallpaper);
        return (Bitmap?)Resources.ResourceManager.GetObject(s) ?? DefaultWallpaper;
    }

    /// <summary>
    /// Gets the resource name for the wallpaper image based on game version and index.
    /// </summary>
    /// <param name="version">The game version.</param>
    /// <param name="index">The wallpaper index.</param>
    /// <returns>The resource name for the wallpaper image.</returns>
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

    private static string GetResourceSuffix(SaveFileType type, int index) => type switch
    {
        SaveFileType.Emerald => "e",
        SaveFileType.FRLG when index > 12 => "frlg",

        SaveFileType.Pt when index > 16 => "pt",
        SaveFileType.HGSS when index > 16 => "hgss",

        SaveFileType.B2W2 when index > 16 => "b2w2",
        SaveFileType.AO when index > 16 => "ao",
        SaveFileType.BDSP => "bdsp",
        SaveFileType.SWSH => "swsh",
        SaveFileType.SV => "sv",

        SaveFileType.LGPE => string.Empty,

        _ => type.Context switch
        {
            EntityContext.Gen3 => "rs",
            EntityContext.Gen4 => "dp",
            EntityContext.Gen5 => "bw",
            EntityContext.Gen6 or EntityContext.Gen7 => "xy", // roughly equivalent, only use X/Y's because they don't force checker-boxes.
            _ => string.Empty,
        },
    };

    private static string GetResourceSuffix(GameVersion version, int index) => version.Context switch
    {
        EntityContext.Gen3 when version == E => "e",
        EntityContext.Gen3 when FRLG.Contains(version) && index > 12 => "frlg",
        EntityContext.Gen3 => "rs",

        EntityContext.Gen4 when index <= 16 => "dp",
        EntityContext.Gen4 when version == Pt => "pt",
        EntityContext.Gen4 when HGSS.Contains(version) => "hgss",

        EntityContext.Gen5 => B2W2.Contains(version) && index > 16 ? "b2w2" : "bw",
        EntityContext.Gen6 => ORAS.Contains(version) && index > 16 ? "ao" : "xy",
        EntityContext.Gen7 => "xy", // roughly equivalent, only use X/Y's because they don't force checker-boxes.
        EntityContext.Gen8b => "bdsp",
        EntityContext.Gen8 => "swsh",
        EntityContext.Gen9 => "sv",
        _ => string.Empty,
    };
}
