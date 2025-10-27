using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InfiniteRoyale9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public uint Value0      { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint Value1      { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }
    public uint Wins        { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    public uint PrizeMedals { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }
    public uint Value4      { get => ReadUInt32LittleEndian(Data[0x10..]); set => WriteUInt32LittleEndian(Data[0x10..], value); }
    public uint Value5      { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], value); }
}
