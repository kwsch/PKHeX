using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the selected clothing choices of the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayerFashion9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public ulong Base      { get => ReadUInt64LittleEndian(Data); set => WriteUInt64LittleEndian(Data, value); }
    public ulong Accessory { get => ReadUInt64LittleEndian(Data[0x08..]); set => WriteUInt64LittleEndian(Data[0x08..], value); }
    public ulong Bag       { get => ReadUInt64LittleEndian(Data[0x10..]); set => WriteUInt64LittleEndian(Data[0x10..], value); }
    public ulong Eyewear   { get => ReadUInt64LittleEndian(Data[0x18..]); set => WriteUInt64LittleEndian(Data[0x18..], value); }
    public ulong Footwear  { get => ReadUInt64LittleEndian(Data[0x20..]); set => WriteUInt64LittleEndian(Data[0x20..], value); }
    public ulong Gloves    { get => ReadUInt64LittleEndian(Data[0x28..]); set => WriteUInt64LittleEndian(Data[0x28..], value); }
    public ulong Headwear  { get => ReadUInt64LittleEndian(Data[0x30..]); set => WriteUInt64LittleEndian(Data[0x30..], value); }
    public ulong Hairstyle { get => ReadUInt64LittleEndian(Data[0x38..]); set => WriteUInt64LittleEndian(Data[0x38..], value); }
    public ulong Legwear   { get => ReadUInt64LittleEndian(Data[0x40..]); set => WriteUInt64LittleEndian(Data[0x40..], value); }
    public ulong Clothing  { get => ReadUInt64LittleEndian(Data[0x48..]); set => WriteUInt64LittleEndian(Data[0x48..], value); }
    public ulong Face      { get => ReadUInt64LittleEndian(Data[0x50..]); set => WriteUInt64LittleEndian(Data[0x50..], value); }
}
