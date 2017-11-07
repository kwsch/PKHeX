namespace PKHeX.Core
{
    /// <summary>
    /// Small general purpose value passing object with misc data pertaining to an encountered Species.
    /// </summary>
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
