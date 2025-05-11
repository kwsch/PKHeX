namespace PKHeX.Core;

/// <summary>
/// Style to display moves.
/// </summary>
public enum MoveDisplayStyle : byte
{
    /// <summary>
    /// Moves are slots 1-4, with no empty slots, and correspond to the rectangular grid without empty spaces.
    /// </summary>
    Fill,

    /// <summary>
    /// Move slots are assigned to the directional pad, and unused directional slots are not displayed.
    /// </summary>
    Directional,
}
