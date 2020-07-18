namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data with a fixed Catch Rate
    /// </summary>
    /// <remarks>
    /// Generation 1 specific value used in detecting unmodified/untraded Generation 1 Trade Encounter data.
    /// </remarks>
    public sealed class EncounterTrade1 : EncounterTradeGB
    {
        /// <summary>
        /// <see cref="PK1.Catch_Rate"/> value the encounter is found with.
        /// </summary>
        /// <remarks>
        /// Gen1 Pokémon have a Catch Rate value when created by the game; depends on the origin version.
        /// Few encounters use a value not from the game's Personal data.
        /// </remarks>
        private readonly byte Catch_Rate;

        private bool HasOddCatchRate => Catch_Rate != 0;

        public EncounterTrade1(int species, int level, byte rate) : this(species, level) => Catch_Rate = rate;
        public EncounterTrade1(int species, int level) : base(species, level) { }

        public byte GetInitialCatchRate()
        {
            if (HasOddCatchRate)
                return Catch_Rate;

            var pt = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
            return (byte)pt[Species].CatchRate;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pk1 = (PK1)pk;
            pk1.Catch_Rate = GetInitialCatchRate();
        }

        public override bool IsMatch(PKM pkm)
        {
            if (Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (!(pkm is PK1 pk1) || !pkm.Gen1_NotTradeback)
                return true;

            var req = GetInitialCatchRate();
            return req == pk1.Catch_Rate;
        }
    }
}
