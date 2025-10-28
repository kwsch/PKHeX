namespace PKHeX.Core;

/// <summary>
/// Provides details about box wallpaper values within the save file.
/// </summary>
public interface IBoxDetailWallpaper
{
    public int GetBoxWallpaper(int box);
    public void SetBoxWallpaper(int box, int value);
}

public static class BoxDetailWallpaperExtension
{
    public static void MoveWallpaper(this IBoxDetailWallpaper obj, int box, int insertBeforeBox)
    {
        if (box == insertBeforeBox)
            return;
        var value = obj.GetBoxWallpaper(box);
        // Shift all wallpapers between the two boxes
        if (box < insertBeforeBox)
        {
            for (int i = box; i < insertBeforeBox; i++)
                obj.SetBoxWallpaper(i, obj.GetBoxWallpaper(i + 1));
        }
        else
        {
            for (int i = box; i > insertBeforeBox; i--)
                obj.SetBoxWallpaper(i, obj.GetBoxWallpaper(i - 1));
        }
        obj.SetBoxWallpaper(insertBeforeBox, value);
    }

    public static void SwapWallpaper(this IBoxDetailWallpaper obj, int box1, int box2)
    {
        if (box1 == box2)
            return;
        var value1 = obj.GetBoxWallpaper(box1);
        var value2 = obj.GetBoxWallpaper(box2);
        obj.SetBoxWallpaper(box1, value2);
        obj.SetBoxWallpaper(box2, value1);
    }
}
