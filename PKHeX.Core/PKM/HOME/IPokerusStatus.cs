namespace PKHeX.Core;

/// <summary>
/// Holds the Pokerus status of a <see cref="PKM"/>.
/// </summary>
public interface IPokerusStatus
{
    /// <summary>
    /// Pokerus Strain and Duration
    /// </summary>
    byte PKRS { get; set; }
}
