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
