namespace PKHeX.Core;

/// <summary>
/// Interface describing how to obtain a <see cref="T"/> from the implementer.
/// </summary>
/// <typeparam name="T">Type of sprite that can be generated.</typeparam>
public interface ISpriteBuilder<T>
{
    /// <summary>
    /// Gets a sprite using the requested parameters.
    /// </summary>
    T GetSprite(ushort species, byte form, byte gender, uint formarg, int heldItem, bool isEgg, Shiny shiny,
        EntityContext context = EntityContext.None);

    /// <summary>
    /// Revises the sprite using the requested parameters.
    /// </summary>
    T GetSprite(T baseSprite, ushort species, int heldItem, bool isEgg, Shiny shiny,
        EntityContext context = EntityContext.None);

    /// <summary>
    /// Initializes the implementation with the context details from the <see cref="sav"/>.
    /// </summary>
    /// <param name="sav">Save File context the sprites will be generated for</param>
    void Initialize(SaveFile sav);
}
