namespace PKHeX.Core
{
    public abstract class EncounterTradeGB : EncounterTrade
    {
        protected EncounterTradeGB(int species, int level)
        {
            Species = species;
            Level = level;
        }

        public abstract bool IsMatch(PKM pkm);
    }
}
