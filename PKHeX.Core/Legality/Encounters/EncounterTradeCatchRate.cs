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
        public uint Catch_Rate;
    }
}