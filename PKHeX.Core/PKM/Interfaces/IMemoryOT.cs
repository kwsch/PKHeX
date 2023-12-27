namespace PKHeX.Core;

/// <summary>
/// Exposes memory details for the Original Trainer.
/// </summary>
public interface IMemoryOT : IMemoryOTReadOnly
{
    new byte OT_Memory { get; set; }
    new byte OT_Intensity { get; set; }
    new byte OT_Feeling { get; set; }
    new ushort OT_TextVar { get; set; }
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
