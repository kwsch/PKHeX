namespace PKHeX.Core
{
    /// <summary>
    /// Contains information pertaining the floor tile the <see cref="IEncounterable"/> was obtained on in <see cref="GameVersion.Gen4"/>.
    /// </summary>
    /// <remarks>
    /// <seealso cref="EncounterSlot4"/>
    /// <seealso cref="EncounterStatic4"/>
    /// </remarks>
    public interface IEncounterTypeTile
    {
        /// <summary>
        /// Tile Type the <see cref="IEncounterable"/> was obtained on.
        /// </summary>
        EncounterType TypeEncounter { get; }
    }

    public static class EncounterTypeTileExtensions
    {
        /// <summary>
        /// Gets if the resulting <see cref="PKM"/> will still have a value depending on the current <see cref="format"/>.
        /// </summary>
        /// <remarks>Generation 6 no longer stores this value.</remarks>
        public static bool HasTypeEncounter(this IEncounterTypeTile _, int format) => format is 4 or 5;
    }
}
