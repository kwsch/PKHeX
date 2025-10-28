using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving player sprite images from save files.
/// </summary>
public static class PlayerSpriteUtil
{
    /// <summary>
    /// Gets the player sprite image for the specified <see cref="SaveFile"/>.
    /// </summary>
    /// <param name="sav">The save file to get the player sprite for.</param>
    /// <returns>A <see cref="Bitmap"/> representing the player sprite, or null if not available.</returns>
    public static Bitmap? Sprite(this SaveFile sav) => GetSprite(sav);

    private static Bitmap? GetSprite(SaveFile sav)
    {
        if (sav is IMultiplayerSprite ms)
        {
            // Gen6 only
            string file = $"tr_{ms.MultiplayerSpriteID:00}";
            return Resources.ResourceManager.GetObject(file) as Bitmap ?? Resources.tr_00;
        }
        return null;
    }
}
