namespace PKHeX.Core;

/// <summary>
/// Lumped image type.
/// </summary>
public enum HeldItemLumpImage
{
    /// <summary>
    /// Held Item sprite is a specific held item.
    /// </summary>
    NotLump = 0,

    /// <summary>
    /// Held Item sprite should show the Technical Machine (TM) icon.
    /// </summary>
    TechnicalMachine,

    /// <summary>
    /// Held Item sprite should show the Technical Record (TR) icon.
    /// </summary>
    TechnicalRecord,
}

/// <summary>
/// Logic to check if a held item should show a lumped image sprite.
/// </summary>
public static class HeldItemLumpUtil
{
    /// <summary>
    /// Checks if the <see cref="image"/> is a lumped sprite.
    /// </summary>
    /// <param name="image">Evaluated type</param>
    /// <returns>True if the <see cref="image"/> is a lumped sprite.</returns>
    public static bool IsLump(this HeldItemLumpImage image) => image != HeldItemLumpImage.NotLump;

    /// <summary>
    /// Checks if the <see cref="item"/> should show a lumped sprite.
    /// </summary>
    /// <param name="item">Held Item index</param>
    /// <param name="context">Generation context</param>
    /// <returns>Evaluation result.</returns>
    public static HeldItemLumpImage GetIsLump(int item, EntityContext context) => item switch
    {
           (>= 0328 and <= 0425)        // TM001-TM092, HM01-06 (Gen2/3/4 TMs)
        or (>= 0618 and <= 0620)        // TM093-TM095
        or (>= 0690 and <= 0694) or 737 // TM096-TM099, HM07, TM100(pre-Gen9)
        or (>= 2160 and <= 2289)        // TM100-TM229
        => HeldItemLumpImage.TechnicalMachine,

        (>= 1130 and <= 1229) => HeldItemLumpImage.TechnicalRecord, // Gen8 TR
        _ => HeldItemLumpImage.NotLump,
    };
}
