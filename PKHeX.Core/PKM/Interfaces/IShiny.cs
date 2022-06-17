namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a boolean indicating if the object is a shiny.
/// </summary>
public interface IShiny
{
    /// <summary>
    /// Is definitively a shiny.
    /// </summary>
    bool IsShiny { get; }
}
