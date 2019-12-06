namespace PKHeX.Core
{
    public sealed class EvoCriteria : DexLevel
    {
        public EvoCriteria(int species, int form) : base(species, form)
        {
        }

        public int MinLevel { get; set; }
        public bool RequiresLvlUp { get; set; }
        public int Method { get; set; } = -1;

        public bool IsTradeRequired => ((EvolutionType) Method).IsTrade();
    }
}
