using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

public static class TypeSpriteUtil
{
    private static Bitmap? Get(string name) => Resources.ResourceManager.GetObject(name) as Bitmap;

    public static Image? GetTypeSpriteWide(byte type, int generation = PKX.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_wide_{type:00}");
    }

    public static Image? GetTypeSpriteIcon(byte type, int generation = PKX.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_{type:00}");
    }

    public static Image? GetTypeSpriteGem(byte type)
    {
        return Get($"gem_{type:00}");
    }
}
