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
        <   25 => XXS,  // 23:256
        <   60 => XS,   // 35:256
        <  100 => S,    // 40:256
        <= 155 => AV,   // 56:256
        <= 195 => L,    // 40:256
        <= 230 => XL,   // 35:256
        <  255 => XXL,  // 23:256
             _ => XXXL, //  1:256
    };
}
