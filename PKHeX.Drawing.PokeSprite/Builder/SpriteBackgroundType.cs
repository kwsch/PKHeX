namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Specifies the type of background to render behind a Pokémon sprite.
/// </summary>
public enum SpriteBackgroundType
{
    /// <summary>
    /// No background is rendered.
    /// </summary>
    None,
    /// <summary>
    /// A colored stripe is rendered at the bottom of the sprite.
    /// </summary>
    BottomStripe,
    /// <summary>
    /// The entire background behind the sprite is filled.
    /// </summary>
    FullBackground,
    /// <summary>
    /// A colored stripe is rendered at the top of the sprite.
    /// </summary>
    TopStripe,
}
