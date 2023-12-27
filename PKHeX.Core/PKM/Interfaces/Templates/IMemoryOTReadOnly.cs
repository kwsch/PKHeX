namespace PKHeX.Core;

public interface IMemoryOTReadOnly
{
    byte OT_Memory { get; }
    byte OT_Intensity { get; }
    byte OT_Feeling { get; }
    ushort OT_TextVar { get; }
}
