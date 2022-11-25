namespace PKHeX.Core;

/// <summary>
/// Interface that exposes an indication if the Pokémon is an alpha Pokémon.
/// </summary>
public interface IAlpha : IAlphaReadOnly
{
    new bool IsAlpha { get; set; }
}
