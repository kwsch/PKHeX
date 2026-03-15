using PKHeX.Core;
using SkiaSharp;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class WallpaperUtil
{
    private static SKBitmap DefaultWallpaper => ResourceLoader.Get("box_wp16xy");

    public static SKBitmap WallpaperImage(this SaveFile sav, int box) => GetWallpaper(sav, box);

    private static SKBitmap GetWallpaper(SaveFile sav, int box)
    {
        if (sav is not IBoxDetailWallpaper wp)
            return DefaultWallpaper;

        if (sav is SAV9ZA)
            return ResourceLoader.Get("box_wp02bdsp");

        int wallpaper = wp.GetBoxWallpaper(box);
        string s = GetWallpaperResourceName(sav.Version, wallpaper);
        return ResourceLoader.GetObject(s) ?? DefaultWallpaper;
    }

    public static string GetWallpaperResourceName(GameVersion version, int index)
    {
        index++;
        var suffix = GetResourceSuffix(version, index);
        var variant = version switch
        {
            SL when index is 20 => "_n",
            VL when index is 20 => "_u",
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
            EntityContext.Gen6 or EntityContext.Gen7 => "xy",
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
        EntityContext.Gen7 => "xy",
        EntityContext.Gen8b => "bdsp",
        EntityContext.Gen8 => "swsh",
        EntityContext.Gen9 => "sv",
        _ => string.Empty,
    };
}
