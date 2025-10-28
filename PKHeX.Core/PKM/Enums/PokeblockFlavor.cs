namespace PKHeX.Core;

/// <summary>
/// Flavor of Pokeblock
/// </summary>
public enum PokeblockFlavor : byte
{
    Spicy = 0,
    Sour = 1,
    Sweet = 2,
    Dry = 3,
    Bitter = 4,
}

/// <summary>
/// Logic for Poké Blocks used in generation 3/4.
/// </summary>
public static class PokeblockUtil
{
    private const int Flavors = 5;

    public static PokeblockFlavor GetLikedBlockFlavor(uint nature) => (PokeblockFlavor)(nature / Flavors);
    public static PokeblockFlavor GetDislikedBlockFlavor(uint nature) => (PokeblockFlavor)(nature % Flavors);
}
