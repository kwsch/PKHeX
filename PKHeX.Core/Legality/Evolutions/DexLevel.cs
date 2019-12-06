namespace PKHeX.Core
{
    /// <summary>
    /// Small general purpose value passing object with misc data pertaining to an encountered Species.
    /// </summary>
    public class DexLevel
    {
        public readonly int Species;
        public readonly int Form;

        public int Level { get; set; }

        protected DexLevel(int species, int form)
        {
            Species = species;
            Form = form;
        }
    }
}