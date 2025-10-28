namespace PKHeX.Core;

/// <summary>
/// Interface for looking up evolutions by species and form.
/// </summary>
public interface IEvolutionLookup
{
    /// <summary>
    /// Gets a read-only reference to the <see cref="EvolutionNode"/> associated with the specified species and form.
    /// </summary>
    /// <remarks>This indexer allows efficient access to evolution nodes by species and form identifiers.  The
    /// returned reference is read-only, ensuring that the underlying data cannot be modified.</remarks>
    /// <param name="species">The species identifier used to locate the evolution node.</param>
    /// <param name="form">The form identifier used to locate the evolution node.</param>
    /// <returns>A read-only reference to the <see cref="EvolutionNode"/> corresponding to the specified species and form.</returns>
    ref readonly EvolutionNode this[ushort species, byte form] { get; }
}
