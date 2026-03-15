using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class PlayerSpriteUtil
{
    public static SKBitmap? Sprite(this SaveFile sav) => GetSprite(sav);

    private static SKBitmap? GetSprite(SaveFile sav)
    {
        if (sav is IMultiplayerSprite ms)
        {
            string file = $"tr_{ms.MultiplayerSpriteID:00}";
            return ResourceLoader.GetObject(file) ?? ResourceLoader.Get("tr_00");
        }
        return null;
    }
}
