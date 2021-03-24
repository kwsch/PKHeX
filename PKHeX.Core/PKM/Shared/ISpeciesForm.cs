namespace PKHeX.Core
{
    /// <summary>
    /// Simple identification values for an Pokémon Entity.
    /// </summary>
    public interface ISpeciesForm
    {
        /// <summary> Species ID of the entity (National Dex). </summary>
        int Species { get; }

        /// <summary> Form ID of the entity. </summary>
        int Form { get; }
    }
}
