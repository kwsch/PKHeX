namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data with a fixed Catch Rate
    /// </summary>
    /// <remarks>
    /// Generation 1 specific value used in detecting unmodified/untraded Generation 1 Trade Encounter data.
    /// </remarks>
    public sealed class EncounterTradeCatchRate : EncounterTrade
    {
        /// <summary>
        /// <see cref="PK1.Catch_Rate"/> value the encounter is found with.
        /// </summary>
        public uint Catch_Rate;
    }
}