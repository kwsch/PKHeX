namespace PKHeX.Core;

/// <summary>
/// Specifies that a single ribbon index is prominently selected.
/// </summary>
/// <remarks>
/// <see cref="RibbonIndex"/> values.
/// </remarks>
public interface IRibbonSetAffixed
{
    sbyte AffixedRibbon { get; set; }
}

public static class AffixedRibbon
{
    /// <summary>
    /// Value present when no ribbon is affixed.
    /// </summary>
    public const sbyte None = -1;

    /// <summary>
    /// Represents the maximum allowable value for an affixed ribbon index.
    /// </summary>
    public const sbyte Max = (sbyte)RibbonIndex.MAX_COUNT - 1;
}
