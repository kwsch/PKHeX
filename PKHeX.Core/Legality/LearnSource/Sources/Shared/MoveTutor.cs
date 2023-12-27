namespace PKHeX.Core;

/// <summary>
/// <see cref="ILearnSource"/> shared logic for tutor moves.
/// </summary>
public static class MoveTutor
{
    /// <summary> Gets the move that a Rotom form is permitted to have while existing as a specific <see cref="form"/>. </summary>
    public static int GetRotomFormMove(byte form) => form switch
    {
        1 => (int)Move.Overheat,
        2 => (int)Move.HydroPump,
        3 => (int)Move.Blizzard,
        4 => (int)Move.AirSlash,
        5 => (int)Move.LeafStorm,
        _ => 0,
    };
}
