using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the selected appearance choices of the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayerFashion8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Data)
{
    public ulong Hair     { get => ReadUInt64LittleEndian(Data); set => WriteUInt64LittleEndian(Data, value); }
    public ulong Contacts { get => ReadUInt64LittleEndian(Data[0x08..]); set => WriteUInt64LittleEndian(Data[0x08..], value); }
    public ulong Eyebrows { get => ReadUInt64LittleEndian(Data[0x10..]); set => WriteUInt64LittleEndian(Data[0x10..], value); }
    public ulong Glasses  { get => ReadUInt64LittleEndian(Data[0x18..]); set => WriteUInt64LittleEndian(Data[0x18..], value); }
    public ulong Hat      { get => ReadUInt64LittleEndian(Data[0x20..]); set => WriteUInt64LittleEndian(Data[0x20..], value); }
    public ulong Top      { get => ReadUInt64LittleEndian(Data[0x28..]); set => WriteUInt64LittleEndian(Data[0x28..], value); }
    public ulong Bottoms  { get => ReadUInt64LittleEndian(Data[0x30..]); set => WriteUInt64LittleEndian(Data[0x30..], value); }
    public ulong _38      { get => ReadUInt64LittleEndian(Data[0x38..]); set => WriteUInt64LittleEndian(Data[0x38..], value); }
    public ulong Shoes    { get => ReadUInt64LittleEndian(Data[0x40..]); set => WriteUInt64LittleEndian(Data[0x40..], value); }
    public ulong _48      { get => ReadUInt64LittleEndian(Data[0x48..]); set => WriteUInt64LittleEndian(Data[0x48..], value); }
    public ulong _50      { get => ReadUInt64LittleEndian(Data[0x50..]); set => WriteUInt64LittleEndian(Data[0x50..], value); }
    public ulong Skin     { get => ReadUInt64LittleEndian(Data[0x58..]); set => WriteUInt64LittleEndian(Data[0x58..], value); }
}
