namespace PKHeX.Core
{
    public interface IEncounterTypeTile
    {
        EncounterType TypeEncounter { get; }
    }

    public static class EncounterTypeTileExtensions
    {
        /// <summary>
        /// Gets if the resulting <see cref="PKM"/> will still have a value depending on the current <see cref="format"/>.
        /// </summary>
        /// <remarks>Generation 6 no longer stores this value.</remarks>
        public static bool HasTypeEncounter(this IEncounterTypeTile _, int format) => format == 4 || format == 5;
    }
}
