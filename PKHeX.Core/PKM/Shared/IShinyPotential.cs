namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a shiny potential state indicating what kinds of <see cref="Core.Shiny"/> can be expressed.
/// </summary>
public interface IShinyPotential
{
    Shiny Shiny { get; }
}
