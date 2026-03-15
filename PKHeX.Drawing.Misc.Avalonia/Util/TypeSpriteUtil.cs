using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class TypeSpriteUtil
{
    private static SKBitmap? Get(string name) => ResourceLoader.GetObject(name);

    public static SKBitmap? GetTypeSpriteWide(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_wide_{type:00}");
    }

    public static SKBitmap? GetTypeSpriteIcon(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_{type:00}");
    }

    public static SKBitmap? GetTypeSpriteIconSmall(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_s_{type:00}");
    }

    public static SKBitmap? GetTypeSpriteGem(byte type)
    {
        return Get($"gem_{type:00}");
    }
}
