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
    public const sbyte None = -1;
    public const sbyte Max = (sbyte)RibbonIndex.MAX_COUNT - 1;
}
