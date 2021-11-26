using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Player Map Location Save Data
    /// </summary>
    /// <remarks>size 0x80, struct_name PLAYER_SAVE_DATA</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class PlayerData8b : SaveBlock
    {
        private const int SIZE_LOCATION = 4 + (4 * 3) + 4; // 20 (0x14)

        private const int OFS_LOC1 = 0x10;
        private const int OFS_LOC2 = OFS_LOC1 + SIZE_LOCATION;
        private const int OFS_LOC3 = OFS_LOC2 + SIZE_LOCATION;
        private const int OFS_PART2 = OFS_LOC3 + SIZE_LOCATION;
        private const int OFS_MAP = OFS_PART2 + 4 + 4;
        private const int OFS_TOKUSHU_BOOL = OFS_MAP + SIZE_LOCATION;
        private const int OFS_TOKUSHU = OFS_TOKUSHU_BOOL + 4;
        private const int SIZE = OFS_TOKUSHU + SIZE_LOCATION; // 0x80

        public PlayerData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

        public bool GearType
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x00) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + 0x00);
        }
        public bool ShoesFlag
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x04) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + 0x04);
        }
        public uint Form
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x08);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08);
        }
        public byte BikeColor { get => Data[Offset + 0x0C]; set => Data[Offset + 0x0C] = value; }
        // 0x10: WorpPoint - Teleport
        // 0x10: WorpPoint - Zenmetu
        // 0x10: WorpPoint - Ananuke

        public int WarpTeleportZone { get => BitConverter.ToInt32 (Data, Offset + OFS_LOC1 + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC1 + 0x00); }
        public float WarpTeleportX  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC1 + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC1 + 0x04); }
        public float WarpTeleportY  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC1 + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC1 + 0x08); }
        public float WarpTeleportZ  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC1 + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC1 + 0x0C); }
        public int WarpTeleportDir  { get => BitConverter.ToInt32 (Data, Offset + OFS_LOC1 + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC1 + 0x10); }

        public int WarpZenmetuZone { get => BitConverter.ToInt32 (Data, Offset + OFS_LOC2 + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC2 + 0x00); }
        public float WarpZenmetuX  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC2 + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC2 + 0x04); }
        public float WarpZenmetuY  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC2 + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC2 + 0x08); }
        public float WarpZenmetuZ  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC2 + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC2 + 0x0C); }
        public int WarpZenmetuDir  { get => BitConverter.ToInt32 (Data, Offset + OFS_LOC2 + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC2 + 0x10); }

        public int WarpAnanukeZone { get => BitConverter.ToInt32 (Data, Offset + OFS_LOC3 + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC3 + 0x00); }
        public float WarpAnanukeX  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC3 + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC3 + 0x04); }
        public float WarpAnanukeY  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC3 + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC3 + 0x08); }
        public float WarpAnanukeZ  { get => BitConverter.ToSingle(Data, Offset + OFS_LOC3 + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC3 + 0x0C); }
        public int WarpAnanukeDir  { get => BitConverter.ToInt32 (Data, Offset + OFS_LOC3 + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_LOC3 + 0x10); }

        public float WalkCount
        {
            get => BitConverter.ToSingle(Data, Offset + OFS_PART2 + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_PART2 + 0x04);
        }
        public int NatukiWalkCount
        {
            get => BitConverter.ToInt32(Data, Offset + OFS_PART2 + 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_PART2 + 0x04);
        }

        public int TownMapZone { get => BitConverter.ToInt32 (Data, Offset + OFS_MAP + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_MAP + 0x00); }
        public float TownMapX  { get => BitConverter.ToSingle(Data, Offset + OFS_MAP + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_MAP + 0x04); }
        public float TownMapY  { get => BitConverter.ToSingle(Data, Offset + OFS_MAP + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_MAP + 0x08); }
        public float TownMapZ  { get => BitConverter.ToSingle(Data, Offset + OFS_MAP + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_MAP + 0x0C); }
        public int TownMapDir  { get => BitConverter.ToInt32 (Data, Offset + OFS_MAP + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_MAP + 0x10); }

        public bool IsTokushuLocation
        {
            get => BitConverter.ToUInt32(Data, Offset + OFS_TOKUSHU_BOOL + 0x00) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + OFS_TOKUSHU_BOOL + 0x00);
        }

        public int TokushuZone { get => BitConverter.ToInt32 (Data, Offset + OFS_TOKUSHU + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_TOKUSHU + 0x00); }
        public float TokushuX  { get => BitConverter.ToSingle(Data, Offset + OFS_TOKUSHU + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_TOKUSHU + 0x04); }
        public float TokushuY  { get => BitConverter.ToSingle(Data, Offset + OFS_TOKUSHU + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_TOKUSHU + 0x08); }
        public float TokushuZ  { get => BitConverter.ToSingle(Data, Offset + OFS_TOKUSHU + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_TOKUSHU + 0x0C); }
        public int TokushuDir  { get => BitConverter.ToInt32 (Data, Offset + OFS_TOKUSHU + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_TOKUSHU + 0x10); }
    }
}
