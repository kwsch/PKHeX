namespace PKHeX.Core
{
    public abstract record EncounterTradeGB : EncounterTrade
    {
        protected EncounterTradeGB(int species, int level)
        {
            Species = species;
            Level = level;
        }

        public abstract bool IsMatch(PKM pkm);
    }
}
