namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a <see cref="Generation"/> to see which canonical generation the data originated in.
/// </summary>
public interface IGeneration
{
    /// <summary>
    /// The canonical generation the data originated in.
    /// </summary>
    byte Generation { get; }
}
