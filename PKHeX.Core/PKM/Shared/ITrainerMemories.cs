namespace PKHeX.Core
{
    public interface ITrainerMemories : IMemoryOT, IMemoryHT
    {
    }

    public static partial class Extensions
    {
        public static void SetTradeMemory(this ITrainerMemories m, bool bank)
        {
            m.SetTradeMemoryHT(bank);
        }
    }
}
