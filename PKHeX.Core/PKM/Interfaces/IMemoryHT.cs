namespace PKHeX.Core;

/// <summary>
/// Exposes memory details for the Handling Trainer (not OT).
/// </summary>
public interface IMemoryHT
{
    byte HandlingTrainerMemory { get; set; }
    ushort HandlingTrainerMemoryVariable { get; set; }
    byte HandlingTrainerMemoryFeeling { get; set; }
    byte HandlingTrainerMemoryIntensity { get; set; }
}

public static partial class Extensions
{
    extension(IMemoryHT ht)
    {
        /// <summary>
        /// Sets a Link Trade memory to the <see cref="ht"/>.
        /// </summary>
        public void SetTradeMemoryHT6(bool bank)
        {
            ht.HandlingTrainerMemory = 4; // Link trade to [VAR: General Location]
            ht.HandlingTrainerMemoryVariable = bank ? (byte)0 : (byte)9; // Somewhere (Bank) : Pokécenter (Trade)
            ht.HandlingTrainerMemoryIntensity = 1;
            ht.HandlingTrainerMemoryFeeling = MemoryContext6.GetRandomFeeling6(4, bank ? 10 : 20); // 0-9 Bank, 0-19 Trade
        }

        /// <summary>
        /// Sets a Link Trade memory to the <see cref="ht"/>.
        /// </summary>
        public void SetTradeMemoryHT8()
        {
            ht.HandlingTrainerMemory = 4; // Link trade to [VAR: General Location]
            ht.HandlingTrainerMemoryVariable = 9; // Pokécenter (Trade)
            ht.HandlingTrainerMemoryIntensity = 1;
            ht.HandlingTrainerMemoryFeeling = MemoryContext8.GetRandomFeeling8(4, 20); // 0-19 Trade
        }

        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public void ClearMemoriesHT()
        {
            ht.HandlingTrainerMemoryVariable = ht.HandlingTrainerMemory = ht.HandlingTrainerMemoryFeeling = ht.HandlingTrainerMemoryIntensity = 0;
        }
    }
}
