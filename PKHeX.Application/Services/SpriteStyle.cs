namespace PKHeX.Application.Services;

/// <summary>
/// Resolved sprite rendering style. Selects which bundled sprite set <see cref="SpriteLoader"/> reads.
/// </summary>
public enum SpriteStyle
{
    /// <summary>Classic 68×56 box sprites (b_*, "Big Pokemon Sprites").</summary>
    Classic,
    /// <summary>Legends: Arceus circular mugshots (c_*, "Legends Arceus Sprites").</summary>
    Mugshot,
    /// <summary>HOME-style artwork (a_*, "Artwork Pokemon Sprites").</summary>
    Artwork,
}

/// <summary>
/// User preference for sprite style. Mirrors upstream PKHeX's SpriteBuilderPreference (subset).
/// </summary>
public enum SpritePreference
{
    /// <summary>Pick the style automatically from the loaded save file.</summary>
    UseSuggested,
    /// <summary>Always use classic box sprites.</summary>
    ForceSprites,
    /// <summary>Always use Legends: Arceus mugshots.</summary>
    ForceMugshots,
    /// <summary>Always use HOME-style artwork.</summary>
    ForceArtwork,
}
