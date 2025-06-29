using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving type sprite images for Pokémon types.
/// </summary>
public static class TypeSpriteUtil
{
    private static Bitmap? Get(string name) => Resources.ResourceManager.GetObject(name) as Bitmap;

    /// <summary>
    /// Gets the wide type sprite image for the specified type and generation.
    /// </summary>
    /// <param name="type">The type index.</param>
    /// <param name="generation">The game generation (default: latest).</param>
    /// <returns>A <see cref="Bitmap"/> representing the wide type sprite, or null if not available.</returns>
    public static Bitmap? GetTypeSpriteWide(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_wide_{type:00}");
    }

    /// <summary>
    /// Gets the icon type sprite image for the specified type and generation.
    /// </summary>
    /// <param name="type">The type index.</param>
    /// <param name="generation">The game generation (default: latest).</param>
    /// <returns>A <see cref="Bitmap"/> representing the icon type sprite, or null if not available.</returns>
    public static Bitmap? GetTypeSpriteIcon(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_{type:00}");
    }

    /// <summary>
    /// Gets the small icon type sprite image for the specified type and generation.
    /// </summary>
    /// <param name="type">The type index.</param>
    /// <param name="generation">The game generation (default: latest).</param>
    /// <returns>A <see cref="Bitmap"/> representing the small icon type sprite, or null if not available.</returns>
    public static Bitmap? GetTypeSpriteIconSmall(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_s_{type:00}");
    }

    /// <summary>
    /// Gets the gem type sprite image for the specified type.
    /// </summary>
    /// <param name="type">The type index.</param>
    /// <returns>A <see cref="Bitmap"/> representing the gem type sprite, or null if not available.</returns>
    public static Bitmap? GetTypeSpriteGem(byte type)
    {
        return Get($"gem_{type:00}");
    }
}
