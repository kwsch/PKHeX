namespace PKHeX.Core;

/// <summary>
/// Specification for Generation 6/7 Mystery Gift shiny indications.
/// </summary>
/// <see cref="WA8.PIDType"/>
/// <see cref="WB8.PIDType"/>
/// <see cref="WC8.PIDType"/>
public enum ShinyType8
{
    /// <summary> PID is randomly created and forced to be not shiny. </summary>
    Never = 0,

    /// <summary> PID is purely random; can be shiny or not shiny. </summary>
    Random = 1,

    /// <summary> PID is randomly created and forced to be shiny as Stars. </summary>
    AlwaysStar = 2,

    /// <summary> PID is randomly created and forced to be shiny as Squares. </summary>
    AlwaysSquare = 3,

    /// <summary> PID is fixed to a specified value. </summary>
    FixedValue = 4,
}
