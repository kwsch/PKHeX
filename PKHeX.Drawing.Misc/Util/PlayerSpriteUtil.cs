using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

public static class PlayerSpriteUtil
{
    public static Image? Sprite(this SaveFile sav) => GetSprite(sav);

    private static Image? GetSprite(SaveFile sav)
    {
        if (sav is IMultiplayerSprite ms)
        {
            // Gen6 only
            string file = $"tr_{ms.MultiplayerSpriteID:00}";
            return Resources.ResourceManager.GetObject(file) as Image ?? Resources.tr_00;
        }
        return null;
    }
}
