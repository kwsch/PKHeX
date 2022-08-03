namespace PKHeX.Core;

/// <summary>
/// Flags to tweak behavior of the sprite builder for the destination display.
/// </summary>
public enum SpriteBuilderTweak
{
    None = 0,
    BoxBackgroundRed = 1,
}

public static class SpriteBuilderTweakExtensions
{
    public static bool HasFlagFast(this SpriteBuilderTweak value, SpriteBuilderTweak flag) => (value & flag) != 0;
}
