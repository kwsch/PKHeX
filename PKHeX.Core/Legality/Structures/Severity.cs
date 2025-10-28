namespace PKHeX.Core;

/// <summary> Severity indication of the associated <see cref="CheckResult"/> </summary>
/// <remarks>
/// Severity &gt;= <see cref="Valid"/> is green
/// Severity == <see cref="Fishy"/> is yellow
/// Severity &lt;= <see cref="Invalid"/> is red
/// </remarks>
public enum Severity : sbyte
{
    /// <summary>
    /// Definitively not valid.
    /// </summary>
    Invalid = -1,

    /// <summary>
    /// Suspicious values, but still valid.
    /// </summary>
    Fishy = 0,

    /// <summary>
    /// Values are valid.
    /// </summary>
    Valid = 1,
}
