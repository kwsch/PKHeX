namespace PKHeX.Core
{
    public sealed class EvoCriteria : DexLevel
    {
        public int MinLevel { get; set; }
        public bool RequiresLvlUp { get; set; }
        public int Form { get; set; } = -1;
        public int Method { get; set; } = -1;

        public bool IsTradeRequired => ((EvolutionType) Method).IsTrade();
    }
}
