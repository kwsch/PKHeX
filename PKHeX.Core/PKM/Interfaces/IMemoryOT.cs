namespace PKHeX.Core;

/// <summary>
/// Exposes memory details for the Original Trainer.
/// </summary>
public interface IMemoryOT : IMemoryOTReadOnly
{
    new byte OriginalTrainerMemory { get; set; }
    new byte OriginalTrainerMemoryIntensity { get; set; }
    new byte OriginalTrainerMemoryFeeling { get; set; }
    new ushort OriginalTrainerMemoryVariable { get; set; }
}

public static partial class Extensions
{
    /// <summary>
    /// Sets all values to zero.
    /// </summary>
    public static void ClearMemoriesOT(this IMemoryOT ot)
    {
        ot.OriginalTrainerMemoryVariable = ot.OriginalTrainerMemory = ot.OriginalTrainerMemoryFeeling = ot.OriginalTrainerMemoryIntensity = 0;
    }
}
