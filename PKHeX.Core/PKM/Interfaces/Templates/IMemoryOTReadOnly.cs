namespace PKHeX.Core;

public interface IMemoryOTReadOnly
{
    byte OriginalTrainerMemory { get; }
    byte OriginalTrainerMemoryIntensity { get; }
    byte OriginalTrainerMemoryFeeling { get; }
    ushort OriginalTrainerMemoryVariable { get; }
}
