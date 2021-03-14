namespace PKHeX.Core
{
    public static class Pokeblock
    {
        public static Flavor GetLikedBlockFlavor(uint nature) => (Flavor)(nature/5);
        public static Flavor GetDislikedBlockFlavor(uint nature) => (Flavor)(nature%5);

        public enum Flavor
        {
            Spicy,
            Sour,
            Sweet,
            Dry,
            Bitter,
        }
    }
}
