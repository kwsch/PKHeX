using static PKHeX.Core.PokeSizeDetailed;

namespace PKHeX.Core;

/// <summary>
/// Size Groupings for an entity's Height and Weight
/// </summary>
public enum PokeSizeDetailed
{
    XXXS,
    XXS,
    XS,
    S,
    AV,
    L,
    XL,
    XXL,
    XXXL,
}

public static class PokeSizeDetailedUtil
{
    /// <summary>
    /// Compares the sizing scalar to different thresholds to determine the size rating.
    /// </summary>
    /// <param name="scalar">Sizing scalar (0-255)</param>
    /// <returns>0-4 rating</returns>
    public static PokeSizeDetailed GetSizeRating(byte scalar) => scalar switch
    {
             0 => XXXS, //  1:256
        <=  30 => XXS,  // 29:256
        <=  60 => XS,   // 30:256
        <  100 => S,    // 39:256
        <= 160 => AV,   // 61:256
        <= 195 => L,    // 35:256
        <= 241 => XL,   // 44:256
        <  255 => XXL,  // 13:256
             _ => XXXL, //  1:256
    };
}
