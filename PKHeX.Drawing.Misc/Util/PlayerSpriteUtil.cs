using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

public static class PlayerSpriteUtil
{
    public static Image? Sprite(this SaveFile sav) => GetSprite(sav);

    private static Image? GetSprite(SaveFile sav)
    {
        if (sav is SAV6XY or SAV6AO)
        {
            string file = $"tr_{sav.MultiplayerSpriteID:00}";
            return Resources.ResourceManager.GetObject(file) as Image ?? Resources.tr_00;
        }
        return null;
    }
}
