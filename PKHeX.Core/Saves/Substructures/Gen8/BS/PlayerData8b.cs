using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Player Map Location Save Data
/// </summary>
/// <remarks>size 0x80, struct_name PLAYER_SAVE_DATA</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayerData8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    private const int SIZE_LOCATION = 4 + (4 * 3) + 4; // 20 (0x14)

    private const int OFS_LOC1 = 0x10;
    private const int OFS_LOC2 = OFS_LOC1 + SIZE_LOCATION;
    private const int OFS_LOC3 = OFS_LOC2 + SIZE_LOCATION;
    private const int OFS_PART2 = OFS_LOC3 + SIZE_LOCATION;
    private const int OFS_MAP = OFS_PART2 + 4 + 4;
    private const int OFS_TOKUSHU_BOOL = OFS_MAP + SIZE_LOCATION;
    private const int OFS_TOKUSHU = OFS_TOKUSHU_BOOL + 4;
    public const int SIZE = OFS_TOKUSHU + SIZE_LOCATION; // 0x80

    public bool GearType
    {
        get => ReadUInt32LittleEndian(Data) == 1;
        set => WriteUInt32LittleEndian(Data, value ? 1u : 0);
    }
    public bool ShoesFlag
    {
        get => ReadUInt32LittleEndian(Data[0x04..]) == 1;
        set => WriteUInt32LittleEndian(Data[0x04..], value ? 1u : 0);
    }
    public uint Form
    {
        get => ReadUInt32LittleEndian(Data[0x08..]);
        set => WriteUInt32LittleEndian(Data[0x08..], value);
    }
    public byte BikeColor { get => Data[0x0C]; set => Data[0x0C] = value; }
    // 0x10: WorpPoint - Teleport
    // 0x10: WorpPoint - Zenmetu
    // 0x10: WorpPoint - Ananuke

    public int WarpTeleportZone { get =>  ReadInt32LittleEndian(Data[(OFS_LOC1 + 0x00)..]); set => WriteInt32LittleEndian(Data[(OFS_LOC1 + 0x00)..], value); }
    public float WarpTeleportX  { get => ReadSingleLittleEndian(Data[(OFS_LOC1 + 0x04)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC1 + 0x04)..], value); }
    public float WarpTeleportY  { get => ReadSingleLittleEndian(Data[(OFS_LOC1 + 0x08)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC1 + 0x08)..], value); }
    public float WarpTeleportZ  { get => ReadSingleLittleEndian(Data[(OFS_LOC1 + 0x0C)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC1 + 0x0C)..], value); }
    public int WarpTeleportDir  { get =>  ReadInt32LittleEndian(Data[(OFS_LOC1 + 0x10)..]); set => WriteInt32LittleEndian(Data[(OFS_LOC1 + 0x10)..], value); }

    public int WarpZenmetuZone { get =>  ReadInt32LittleEndian(Data[(OFS_LOC2 + 0x00)..]); set => WriteInt32LittleEndian(Data[(OFS_LOC2 + 0x00)..], value); }
    public float WarpZenmetuX  { get => ReadSingleLittleEndian(Data[(OFS_LOC2 + 0x04)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC2 + 0x04)..], value); }
    public float WarpZenmetuY  { get => ReadSingleLittleEndian(Data[(OFS_LOC2 + 0x08)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC2 + 0x08)..], value); }
    public float WarpZenmetuZ  { get => ReadSingleLittleEndian(Data[(OFS_LOC2 + 0x0C)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC2 + 0x0C)..], value); }
    public int WarpZenmetuDir  { get =>  ReadInt32LittleEndian(Data[(OFS_LOC2 + 0x10)..]); set => WriteInt32LittleEndian(Data[(OFS_LOC2 + 0x10)..], value); }

    public int WarpAnanukeZone { get =>  ReadInt32LittleEndian(Data[(OFS_LOC3 + 0x00)..]); set => WriteInt32LittleEndian(Data[(OFS_LOC3 + 0x00)..], value); }
    public float WarpAnanukeX  { get => ReadSingleLittleEndian(Data[(OFS_LOC3 + 0x04)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC3 + 0x04)..], value); }
    public float WarpAnanukeY  { get => ReadSingleLittleEndian(Data[(OFS_LOC3 + 0x08)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC3 + 0x08)..], value); }
    public float WarpAnanukeZ  { get => ReadSingleLittleEndian(Data[(OFS_LOC3 + 0x0C)..]); set => WriteSingleLittleEndian(Data[(OFS_LOC3 + 0x0C)..], value); }
    public int WarpAnanukeDir  { get =>  ReadInt32LittleEndian(Data[(OFS_LOC3 + 0x10)..]); set => WriteInt32LittleEndian(Data[(OFS_LOC3 + 0x10)..], value); }

    public float WalkCount
    {
        get => ReadSingleLittleEndian(Data[(OFS_PART2 + 0x00)..]);
        set => WriteSingleLittleEndian(Data[(OFS_PART2 + 0x04)..], value);
    }
    public int NatukiWalkCount
    {
        get => ReadInt32LittleEndian(Data[(OFS_PART2 + 0x04)..]);
        set => WriteSingleLittleEndian(Data[(OFS_PART2 + 0x04)..], value);
    }

    public int TownMapZone { get =>  ReadInt32LittleEndian(Data[(OFS_MAP + 0x00)..]); set => WriteInt32LittleEndian(Data[(OFS_MAP + 0x00)..], value); }
    public float TownMapX  { get => ReadSingleLittleEndian(Data[(OFS_MAP + 0x04)..]); set => WriteSingleLittleEndian(Data[(OFS_MAP + 0x04)..], value); }
    public float TownMapY  { get => ReadSingleLittleEndian(Data[(OFS_MAP + 0x08)..]); set => WriteSingleLittleEndian(Data[(OFS_MAP + 0x08)..], value); }
    public float TownMapZ  { get => ReadSingleLittleEndian(Data[(OFS_MAP + 0x0C)..]); set => WriteSingleLittleEndian(Data[(OFS_MAP + 0x0C)..], value); }
    public int TownMapDir  { get =>  ReadInt32LittleEndian(Data[(OFS_MAP + 0x10)..]); set => WriteInt32LittleEndian(Data[(OFS_MAP + 0x10)..], value); }

    public bool IsTokushuLocation
    {
        get => ReadUInt32LittleEndian(Data[(OFS_TOKUSHU_BOOL + 0x00)..]) == 1;
        set => WriteUInt32LittleEndian(Data[(OFS_TOKUSHU_BOOL + 0x00)..], value ? 1u : 0u);
    }

    public int TokushuZone { get =>  ReadInt32LittleEndian(Data[(OFS_TOKUSHU + 0x00)..]); set => WriteInt32LittleEndian(Data[(OFS_TOKUSHU + 0x00)..], value); }
    public float TokushuX  { get => ReadSingleLittleEndian(Data[(OFS_TOKUSHU + 0x04)..]); set => WriteSingleLittleEndian(Data[(OFS_TOKUSHU + 0x04)..], value); }
    public float TokushuY  { get => ReadSingleLittleEndian(Data[(OFS_TOKUSHU + 0x08)..]); set => WriteSingleLittleEndian(Data[(OFS_TOKUSHU + 0x08)..], value); }
    public float TokushuZ  { get => ReadSingleLittleEndian(Data[(OFS_TOKUSHU + 0x0C)..]); set => WriteSingleLittleEndian(Data[(OFS_TOKUSHU + 0x0C)..], value); }
    public int TokushuDir  { get =>  ReadInt32LittleEndian(Data[(OFS_TOKUSHU + 0x10)..]); set => WriteInt32LittleEndian(Data[(OFS_TOKUSHU + 0x10)..], value); }
}
