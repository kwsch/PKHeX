namespace PKHeX.Core
{
    public interface IMemoryOT
    {
        int OT_Memory { get; set; }
        int OT_Intensity { get; set; }
        int OT_Feeling { get; set; }
        int OT_TextVar { get; set; }
    }

    public static partial class Extensions
    {
        public static void ClearMemoriesOT(this IMemoryOT ht)
        {
            ht.OT_Memory = ht.OT_Feeling = ht.OT_Intensity = ht.OT_TextVar = 0;
        }
    }
}
