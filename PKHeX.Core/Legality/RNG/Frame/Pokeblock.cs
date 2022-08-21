namespace PKHeX.Core;

/// <summary>
/// Logic for Poké Blocks used in generation 3/4.
/// </summary>
public static class Pokeblock
{
    public static Flavor GetLikedBlockFlavor(uint nature) => (Flavor)(nature/5);
    public static Flavor GetDislikedBlockFlavor(uint nature) => (Flavor)(nature%5);

    public enum Flavor
    {
        Spicy,
        Sour,
        Sweet,
        Dry,
        Bitter,
    }
}
