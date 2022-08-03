namespace PKHeX.Core;

public static class MoveTutor
{
    /// <summary> Rotom Moves that correspond to a specific form (form-0 ignored). </summary>
    public static int GetRotomFormMove(int form) => form switch
    {
        1 => (int)Move.Overheat,
        2 => (int)Move.HydroPump,
        3 => (int)Move.Blizzard,
        4 => (int)Move.AirSlash,
        5 => (int)Move.LeafStorm,
        _ => 0,
    };
}
