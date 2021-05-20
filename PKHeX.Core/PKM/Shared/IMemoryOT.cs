namespace PKHeX.Core
{
    /// <summary>
    /// Exposes memory details for the Original Trainer.
    /// </summary>
    public interface IMemoryOT
    {
        int OT_Memory { get; set; }
        int OT_Intensity { get; set; }
        int OT_Feeling { get; set; }
        int OT_TextVar { get; set; }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public static void ClearMemoriesOT(this IMemoryOT ot)
        {
            ot.OT_Memory = ot.OT_Feeling = ot.OT_Intensity = ot.OT_TextVar = 0;
        }
    }
}
