namespace PKHeX.Core
{
    public class DexLevel
    {
        public int Species { get; set; }
        public int Level { get; set; }
        public int MinLevel { get; set; }
        public bool RequiresLvlUp { get; set; }
        public int Form { get; set; } = -1;
        public int Flag { get; set; } = -1;
    }
}
