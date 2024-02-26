namespace PKHeX.Core;

/// <summary>
/// Specification for <see cref="PKM.IsShiny"/>, used for creating and validating.
/// </summary>
public enum Shiny : byte
{
    /// <summary> PID is purely random; can be shiny or not shiny. </summary>
    Random = 0,

    /// <summary> PID is randomly created and forced to be not shiny. </summary>
    Never,

    /// <summary> PID is randomly created and forced to be shiny. </summary>
    Always,

    /// <summary> PID is randomly created and forced to be shiny as Stars. </summary>
    AlwaysStar,

    /// <summary> PID is randomly created and forced to be shiny as Squares. </summary>
    AlwaysSquare,

    /// <summary> PID is fixed to a specified value. </summary>
    FixedValue,
}

/// <summary>
/// Extension methods for <see cref="Shiny"/>.
/// </summary>
public static class ShinyExtensions
{
    public static bool ShowSquareBeforeGen8 { get; set; }

    public static bool IsValid(this Shiny s, PKM pk) => s switch
    {
        Shiny.Always => pk.IsShiny,
        Shiny.Never => !pk.IsShiny,
        Shiny.AlwaysSquare => pk.ShinyXor == 0,
        Shiny.AlwaysStar => pk.ShinyXor == 1,
        _ => true,
    };

    public static bool IsShiny(this Shiny s) => s switch
    {
        Shiny.Always => true,
        Shiny.AlwaysSquare => true,
        Shiny.AlwaysStar => true,
        _ => false,
    };

    public static Shiny GetType(PKM pk)
    {
        bool shiny = pk.IsShiny;
        if (!shiny)
            return Shiny.Never;

        if (IsSquareShinyExist(pk))
            return Shiny.AlwaysSquare;
        return Shiny.AlwaysStar;
    }

    /// <summary>
    /// Indicates if square shiny exists and is logical to show to the user.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <returns>True if square shiny exists and is logical to show to the user.</returns>
    public static bool IsSquareShinyExist(PKM pk)
    {
        if (pk.Format < 8 && !ShowSquareBeforeGen8)
            return false;
        return pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == GameVersion.GO;
    }
}
