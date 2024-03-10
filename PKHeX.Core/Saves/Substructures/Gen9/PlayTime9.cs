using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PlayTime9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public int PlayedHours
    {
        get => ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, (ushort)value);
    }

    public int PlayedMinutes
    {
        get => ReadInt32LittleEndian(Data[4..]);
        set => WriteInt32LittleEndian(Data[4..], (ushort)value);
    }

    public int PlayedSeconds
    {
        get => ReadInt32LittleEndian(Data[8..]);
        set => WriteInt32LittleEndian(Data[8..], (ushort)value);
    }
}
