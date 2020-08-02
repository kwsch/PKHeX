namespace PKHeX.Core
{
    /// <summary>
    /// Exposes memory details for the Handling Trainer (not OT).
    /// </summary>
    public interface IMemoryHT
    {
        int HT_Memory { get; set; }
        int HT_TextVar { get; set; }
        int HT_Feeling { get; set; }
        int HT_Intensity { get; set; }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Sets a Link Trade memory to the <see cref="ht"/>.
        /// </summary>
        public static void SetTradeMemoryHT(this IMemoryHT ht, bool bank)
        {
            ht.HT_Memory = 4; // Link trade to [VAR: General Location]
            ht.HT_TextVar = bank ? 0 : 9; // Somewhere (Bank) : Pokécenter (Trade)
            ht.HT_Intensity = 1;
            ht.HT_Feeling = Memories.GetRandomFeeling(4, bank ? 10 : 20); // 0-9 Bank, 0-19 Trade
        }

        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public static void ClearMemoriesHT(this IMemoryHT ht)
        {
            ht.HT_Memory = ht.HT_Feeling = ht.HT_Intensity = ht.HT_TextVar = 0;
        }
    }
}
