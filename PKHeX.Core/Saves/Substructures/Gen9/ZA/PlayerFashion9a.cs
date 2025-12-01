using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the selected clothing choices of the player.
/// </summary>
public sealed class PlayerFashion9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    /*
    private static ReadOnlySpan<ulong> HairStyles =>
    [
        0x602243E9930CA16A,   //hi-top fade
        0x687687B775074243,   //undercut pixie
        0x24B41B3ED9600552,   //flyaway short cut
        0xDB129181F4248AE2,   //slicked back
        0x80DF9648F4352C39,   //simple short hair
        0x1E3D50B87ACACDA4,   //half-up chignon
        0xFDC0716E8FA64F1E,   //simple bob
        0x1457BCB559D4A5A4,   //low chignon
        0x51A05031BE8DABCC,   //asymmetrical bob
        0x85D18D31D168FE64,   //ponytail
        0x4CC69633553865E4,   //braids
        0xE75A2816E2B0B2B5,   //long and wavy
        0x6C59B2A77D1231F7,   //side ponytail
        0x21767F6261C9F4DD,   //pigtails
    ];

    private static ReadOnlySpan<ulong> EyeCuts =>
    [
        0xF99D26ADCE42E2BE,     //droopy
        0x22EF6E329FB657D1,     //half-open
        0xC8CA8B6B8EE7CC38,     //rounded
        0x93071833CC739399,     //angled
    ];
    */

    [TypeConverter(typeof(TypeConverterU64))] public ulong Base      { get => ReadUInt64LittleEndian(Data); set => WriteUInt64LittleEndian(Data, value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Accessory { get => ReadUInt64LittleEndian(Data[0x08..]); set => WriteUInt64LittleEndian(Data[0x08..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Bag       { get => ReadUInt64LittleEndian(Data[0x10..]); set => WriteUInt64LittleEndian(Data[0x10..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Eyewear   { get => ReadUInt64LittleEndian(Data[0x18..]); set => WriteUInt64LittleEndian(Data[0x18..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Footwear  { get => ReadUInt64LittleEndian(Data[0x20..]); set => WriteUInt64LittleEndian(Data[0x20..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Gloves    { get => ReadUInt64LittleEndian(Data[0x28..]); set => WriteUInt64LittleEndian(Data[0x28..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Headwear  { get => ReadUInt64LittleEndian(Data[0x30..]); set => WriteUInt64LittleEndian(Data[0x30..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Hairstyle { get => ReadUInt64LittleEndian(Data[0x38..]); set => WriteUInt64LittleEndian(Data[0x38..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Legwear   { get => ReadUInt64LittleEndian(Data[0x40..]); set => WriteUInt64LittleEndian(Data[0x40..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Clothing  { get => ReadUInt64LittleEndian(Data[0x48..]); set => WriteUInt64LittleEndian(Data[0x48..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong Eyes      { get => ReadUInt64LittleEndian(Data[0x50..]); set => WriteUInt64LittleEndian(Data[0x50..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong LowerBody { get => ReadUInt64LittleEndian(Data[0x58..]); set => WriteUInt64LittleEndian(Data[0x58..], value); }
    [TypeConverter(typeof(TypeConverterU64))] public ulong UpperBody { get => ReadUInt64LittleEndian(Data[0x60..]); set => WriteUInt64LittleEndian(Data[0x60..], value); }
}
