namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data with a fixed Catch Rate
    /// </summary>
    /// <remarks>
    /// Generation 1 specific value used in detecting unmodified/un-traded Generation 1 Trade Encounter data.
    /// Species & Minimum level (legal) possible to acquire at.
    /// </remarks>
    public sealed class EncounterTrade1 : EncounterTradeGB
    {
        public override int Generation => 1;
        public bool GBEra { private get; set; }

        public EncounterTrade1(int species, int level, GameVersion game) : base(species, level)
        {
            Version = game;
            TrainerNames = StringConverter12.G1TradeOTName;
        }

        public byte GetInitialCatchRate()
        {
            var pt = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
            return (byte)pt[Species].CatchRate;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pk1 = (PK1)pk;
            pk1.Catch_Rate = GetInitialCatchRate();
        }

        internal bool IsEncounterTrade1Valid(PKM pkm)
        {
            string ot = pkm.OT_Name;
            if (pkm.Format <= 2)
                return ot == StringConverter12.G1TradeOTStr;
            // Converted string 1/2->7 to language specific value
            var tr = GetOT(pkm.Language);
            return ot == tr;
        }

        public override bool IsMatch(PKM pkm)
        {
            if (Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (!(pkm is PK1 pk1) || !pkm.Gen1_NotTradeback)
                return true;

            if (Version == GameVersion.BU)
            {
                // Encounters with this version have to originate from the Japanese Blue game.
                if (!pkm.Japanese)
                    return false;
                // Stadium 2 can transfer from GSC->RBY without a "Trade", thus allowing un-evolved outsiders
                if (GBEra && !ParseSettings.AllowGBCartEra)
                    return false;
            }

            var req = GetInitialCatchRate();
            return req == pk1.Catch_Rate;
        }
    }
}
