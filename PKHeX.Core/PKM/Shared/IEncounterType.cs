namespace PKHeX.Core
{
    /// <summary>
    /// Indicates the Type of Encounter Tile the Pokémon was encountered on.
    /// </summary>
    public interface IEncounterType
    {
        /// <summary>
        /// Type of Encounter Tile the Pokémon was encountered on.
        /// </summary>
        int EncounterType { get; set; }
    }
}
