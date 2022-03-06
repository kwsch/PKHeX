namespace PKHeX.Core
{
    /// <summary>
    /// Exposes memory details for the Original Trainer.
    /// </summary>
    public interface IMemoryOT
    {
        byte OT_Memory { get; set; }
        byte OT_Intensity { get; set; }
        byte OT_Feeling { get; set; }
        ushort OT_TextVar { get; set; }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public static void ClearMemoriesOT(this IMemoryOT ot)
        {
            ot.OT_TextVar = ot.OT_Memory = ot.OT_Feeling = ot.OT_Intensity = 0;
        }
    }
}
