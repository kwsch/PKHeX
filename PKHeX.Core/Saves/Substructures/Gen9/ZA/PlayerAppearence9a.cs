using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the selected traits of appearance of the player, minus hairstyle and eye cut.
/// </summary>
public sealed class PlayerAppearance9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public uint SkinColor          { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint LipColor           { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }
    public uint DarkCircles        { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    public uint EyeColor           { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }
    public uint EyebrowColor       { get => ReadUInt32LittleEndian(Data[0x10..]); set => WriteUInt32LittleEndian(Data[0x10..], value); }
    public uint EyebrowShape       { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], value); }
    public uint EyelashColor       { get => ReadUInt32LittleEndian(Data[0x18..]); set => WriteUInt32LittleEndian(Data[0x18..], value); }
    public uint EyelashShape       { get => ReadUInt32LittleEndian(Data[0x1C..]); set => WriteUInt32LittleEndian(Data[0x1C..], value); }
    public uint BeautySpotFirst    { get => ReadUInt32LittleEndian(Data[0x20..]); set => WriteUInt32LittleEndian(Data[0x20..], value); }
    public uint BeautySpotSecond   { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], value); }
    public uint Freckles           { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], value); }
    public uint HairColor          { get => ReadUInt32LittleEndian(Data[0x2C..]); set => WriteUInt32LittleEndian(Data[0x2C..], value); }
    public uint ColorBlocking      { get => ReadUInt32LittleEndian(Data[0x30..]); set => WriteUInt32LittleEndian(Data[0x30..], value); }
    public uint BalayageFadeFirst  { get => ReadUInt32LittleEndian(Data[0x34..]); set => WriteUInt32LittleEndian(Data[0x34..], value); }
    public uint BalayageFadeSecond { get => ReadUInt32LittleEndian(Data[0x38..]); set => WriteUInt32LittleEndian(Data[0x38..], value); }
    public uint FaceShape          { get => ReadUInt32LittleEndian(Data[0x3C..]); set => WriteUInt32LittleEndian(Data[0x3C..], value); }
    public uint Bangs              { get => ReadUInt32LittleEndian(Data[0x40..]); set => WriteUInt32LittleEndian(Data[0x44..], value); }
    public uint HairColorMode      { get => ReadUInt32LittleEndian(Data[0x44..]); set => WriteUInt32LittleEndian(Data[0x44..], value); }
}
