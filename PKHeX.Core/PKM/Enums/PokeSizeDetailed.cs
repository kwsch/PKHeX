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
        <=  24 => XXS,  // 23:256
        <=  59 => XS,   // 34:256
        <  100 => S,    // 39:256
        <= 155 => AV,   // 55:256
        <= 195 => L,    // 39:256
        <= 230 => XL,   // 34:256
        <  255 => XXL,  // 23:256
             _ => XXXL, //  1:256
    };
}
