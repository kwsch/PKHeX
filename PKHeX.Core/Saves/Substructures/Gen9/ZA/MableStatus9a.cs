using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

public sealed class MableStatus9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    /// <summary>
    /// Current level achieved for Mable's Research.
    /// </summary>
    public uint LevelCurrent { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    /// <summary>
    /// Last viewed/claimed level for Mable's Research.
    /// </summary>
    public uint LevelViewed  { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }

    /// <summary>
    /// Research Points accumulated for Mable's Research.
    /// </summary>
    public uint Points       { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
}
