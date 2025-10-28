namespace PKHeX.Core;

/// <summary>
/// Exposes details about an encounter with a specific ball ID required.
/// </summary>
public interface IFixedBall
{
    /// <summary>
    /// Specific ball ID that is required from this object.
    /// </summary>
    /// <remarks>If <see cref="Ball.None"/>, no specific ball is required (must be one of the permitted balls).</remarks>
    Ball FixedBall { get; }
}
