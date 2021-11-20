using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Player status and info about the trainer
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class MyStatus8b : SaveBlock
    {
        public const int MAX_MONEY = 999_999;
        public const byte MAX_BADGE = 8;
     // public const byte MAX_RANK = 5; // unused?

        public MyStatus8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public string OT
        {
            get => SAV.GetString(Data, Offset + 0, 0x1A);
            set => SAV.SetData(Data, SAV.SetString(value, SAV.OTLength), Offset + 0);
        }

        public int TID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x1C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x1C);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x1E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x1E);
        }

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x20);
            set => BitConverter.GetBytes(Math.Min(MAX_MONEY, value)).CopyTo(Data, Offset + 0x20);
        }

        public bool Male { get => Data[Offset + 0x24] == 1; set => Data[Offset + 0x24] = (byte)(value ? 1 : 0); }
        // 3byte align
        public byte RegionCode { get => Data[Offset + 0x28]; set => Data[Offset + 0x28] = value; }
        public byte BadgeCount { get => Data[Offset + 0x29]; set => Data[Offset + 0x29] = Math.Min(MAX_BADGE, value); }
        public byte TrainerView { get => Data[Offset + 0x2A]; set => Data[Offset + 0x2A] = value; }
        public byte ROMCode { get => Data[Offset + 0x2B]; set => Data[Offset + 0x2B] = value; }
        public bool GameClear { get => Data[Offset + 0x2C] == 1; set => Data[Offset + 0x2C] = (byte)(value ? 1 : 0); }
        // 3byte align
        public byte BodyType { get => Data[Offset + 0x30]; set => Data[Offset + 0x30] = value; }
        public Fashion FashionID { get => (Fashion)Data[Offset + 0x31]; set => Data[Offset + 0x31] = (byte)value; }
        // 2byte align

        public MoveType StarterType
        {
            get => (MoveType)BitConverter.ToInt32(Data, Offset + 0x34);
            set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x34);
        }

        public bool DSPlayer { get => Data[Offset + 0x38] == 1; set => Data[Offset + 0x38] = (byte)(value ? 1 : 0); }
        // 3byte align

        /// <summary> turewalkMemberIndex </summary>
        public int FollowIndex { get => BitConverter.ToInt32(Data, Offset + 0x3C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x3C); }
        public int X { get => BitConverter.ToInt32(Data, Offset + 0x40); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x40); }
        public int Y { get => BitConverter.ToInt32(Data, Offset + 0x44); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x44); }
        public float Height { get => BitConverter.ToSingle(Data, Offset + 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x48); }
        public float Rotation { get => BitConverter.ToSingle(Data, Offset + 0x4C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4C); }

        // end structure!

        public int Game
        {
            get => ROMCode switch
            {
                0 => (int)GameVersion.BD,
                1 => (int)GameVersion.SP,
                _ => throw new ArgumentOutOfRangeException(nameof(Game)),
            };
            set => ROMCode = (GameVersion)value switch
            {
                GameVersion.BD => 0,
                GameVersion.SP => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(Game)),
            };
        }

        public enum Fashion : byte
        {
            Everyday_M = 0,
            Contest_M = 1,
            Pikachu_Hoodie_M = 2,
            Platinum_M = 3,
            Overalls_M = 4,
            Eevee_Jacket_M = 5,
            Gengar_Jacket_M = 6,
            Cyber_M = 7,
            Summer_M = 8,
            Winter_M = 9,
            Spring_M = 10,
            Casual_M = 11,
            Leather_Jacket_M = 12,

            Everyday_F = 100,
            Contest_F = 101,
            Pikachu_Hoodie_F = 102,
            Platinum_F = 103,
            Overalls_F = 104,
            Eevee_Jacket_F = 105,
            Gengar_Jacket_F = 106,
            Cyber_F = 107,
            Summer_F = 108,
            Winter_F = 109,
            Spring_F = 110,
            Casual_F = 111,
            Leather_Jacket_F = 112,
        }
    }
}
