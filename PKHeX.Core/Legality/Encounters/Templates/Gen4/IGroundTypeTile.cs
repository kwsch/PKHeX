namespace PKHeX.Core;

/// <summary>
/// Contains information pertaining the floor tile the <see cref="IEncounterTemplate"/> was obtained on in <see cref="GameVersion.Gen4"/>.
/// </summary>
/// <remarks>
/// <seealso cref="EncounterSlot4"/>
/// <seealso cref="EncounterStatic4"/>
/// </remarks>
public interface IGroundTypeTile
{
    /// <summary>
    /// Tile Type the <see cref="IEncounterTemplate"/> was obtained on.
    /// </summary>
    GroundTileAllowed GroundTile { get; }

    /// <summary>
    /// Gets if the resulting <see cref="PKM"/> will still have a value depending on the current <see cref="format"/>.
    /// </summary>
    /// <remarks>Generation 7 no longer stores this value.</remarks>
    bool HasGroundTile(byte format) => format is (4 or 5 or 6);
}
