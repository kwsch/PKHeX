namespace PKHeX.Core;

/// <summary>
/// Indicates the Type of Encounter Tile the Pokémon was encountered on.
/// </summary>
public interface IGroundTile
{
    /// <summary>
    /// Type of Encounter Tile the Pokémon was encountered on.
    /// </summary>
    GroundTileType GroundTile { get; set; }
}
