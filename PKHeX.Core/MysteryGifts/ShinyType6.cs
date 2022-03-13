namespace PKHeX.Core;

/// <summary>
/// Specification for Generation 6/7 Mystery Gift shiny indications.
/// </summary>
/// <see cref="WC6.PIDType"/>
/// <see cref="WC7.PIDType"/>
/// <see cref="WB7.PIDType"/>
public enum ShinyType6 : byte
{
    /// <summary> PID is fixed to a specified value. </summary>
    FixedValue = 0,

    /// <summary> PID is purely random; can be shiny or not shiny. </summary>
    Random = 1,

    /// <summary> PID is randomly created and forced to be shiny. </summary>
    Always = 2,

    /// <summary> PID is randomly created and forced to be not shiny. </summary>
    Never = 3,
}
