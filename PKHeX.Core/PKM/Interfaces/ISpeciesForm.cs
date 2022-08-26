namespace PKHeX.Core;

/// <summary>
/// Simple identification values for an Pokémon Entity.
/// </summary>
public interface ISpeciesForm
{
    /// <summary> Species ID of the entity (National Dex). </summary>
    ushort Species { get; }

    /// <summary> Form ID of the entity. </summary>
    byte Form { get; }
}
