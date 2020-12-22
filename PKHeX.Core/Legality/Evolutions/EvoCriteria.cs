namespace PKHeX.Core
{
    public sealed class EvoCriteria : DexLevel
    {
        public EvoCriteria(int species, int form) : base(species, form)
        {
        }

        public int MinLevel { get; set; }
        public bool RequiresLvlUp { get; set; }
        public int Method { get; init; } = -1;

        public bool IsTradeRequired => ((EvolutionType) Method).IsTrade();

        public override string ToString() => $"{(Species) Species} [{MinLevel},{Level}] via {(EvolutionType) Method}";
    }
}
