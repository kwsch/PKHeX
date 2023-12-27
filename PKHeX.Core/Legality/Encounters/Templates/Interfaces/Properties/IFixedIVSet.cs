namespace PKHeX.Core;

/// <summary>
/// Contains information about the specified IVs the object has.
/// </summary>
public interface IFixedIVSet
{
    /// <summary>
    /// Contains information about the specified IVs the object has.
    /// </summary>
    /// <remarks>
    /// Be sure to check if the value actually has a value via <see cref="IndividualValueSet.IsSpecified"/> before using it.
    /// </remarks>
    IndividualValueSet IVs { get; }
}
