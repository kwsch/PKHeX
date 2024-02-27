using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

public static class TypeSpriteUtil
{
    private static Bitmap? Get(string name) => Resources.ResourceManager.GetObject(name) as Bitmap;

    public static Bitmap? GetTypeSpriteWide(byte type, byte generation = PKX.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_wide_{type:00}");
    }

    public static Bitmap? GetTypeSpriteIcon(byte type, byte generation = PKX.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_{type:00}");
    }

    public static Bitmap? GetTypeSpriteIconSmall(byte type, byte generation = PKX.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_s_{type:00}");
    }

    public static Bitmap? GetTypeSpriteGem(byte type)
    {
        return Get($"gem_{type:00}");
    }
}
