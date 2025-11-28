using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the selected traits of appearance of the player, minus hair style and eye cut.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayerAppearance9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public ulong SkinColor { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, Convert.ToUInt32(value)); }
    public ulong LipColor { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], Convert.ToUInt32(value)); }
    public ulong DarkCircles { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], Convert.ToUInt32(value)); }
    public ulong EyeColor { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], Convert.ToUInt32(value)); }
    public ulong EyebrowColor { get => ReadUInt32LittleEndian(Data[0x10..]); set => WriteUInt32LittleEndian(Data[0x10..], Convert.ToUInt32(value)); }
    public ulong EyebrowShape { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], Convert.ToUInt32(value)); }
    public ulong EyelashColor { get => ReadUInt32LittleEndian(Data[0x18..]); set => WriteUInt32LittleEndian(Data[0x18..], Convert.ToUInt32(value)); }
    public ulong EyelashShape { get => ReadUInt32LittleEndian(Data[0x1C..]); set => WriteUInt32LittleEndian(Data[0x1C..], Convert.ToUInt32(value)); }
    public ulong FirstBeautySpot { get => ReadUInt32LittleEndian(Data[0x20..]); set => WriteUInt32LittleEndian(Data[0x20..], Convert.ToUInt32(value)); }
    public ulong SecondBeautySpot { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], Convert.ToUInt32(value)); }
    public ulong Freckles { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], Convert.ToUInt32(value)); }
    public ulong HairColor { get => ReadUInt32LittleEndian(Data[0x2C..]); set => WriteUInt32LittleEndian(Data[0x2C..], Convert.ToUInt32(value)); }
    public ulong ColorBlocking { get => ReadUInt32LittleEndian(Data[0x30..]); set => WriteUInt32LittleEndian(Data[0x30..], Convert.ToUInt32(value)); }
    public ulong FirstBalayageFade { get => ReadUInt32LittleEndian(Data[0x34..]); set => WriteUInt32LittleEndian(Data[0x34..], Convert.ToUInt32(value)); }
    public ulong SecondBalayageFade { get => ReadUInt32LittleEndian(Data[0x38..]); set => WriteUInt32LittleEndian(Data[0x38..], Convert.ToUInt32(value)); }
    public ulong FaceShape { get => ReadUInt32LittleEndian(Data[0x3C..]); set => WriteUInt32LittleEndian(Data[0x3C..], Convert.ToUInt32(value)); }
    public ulong Bangs { get => ReadUInt32LittleEndian(Data[0x40..]); set => WriteUInt32LittleEndian(Data[0x44..], Convert.ToUInt32(value)); }
    public ulong HairColorMode { get => ReadUInt32LittleEndian(Data[0x44..]); set => WriteUInt32LittleEndian(Data[0x44..], Convert.ToUInt32(value)); }
}
