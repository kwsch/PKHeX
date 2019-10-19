using System;

namespace PKHeX.Core
{
    public sealed class Situation8 : SaveBlock
    {
        public Situation8(SAV8SWSH sav, int offset) : base(sav) => Offset = offset;

        // "StartLocation"
        public int M { get => BitConverter.ToUInt16(Data, Offset + 0x00); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x00); }
        public float X { get => BitConverter.ToSingle(Data, Offset + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08); }
        public float Z { get => BitConverter.ToSingle(Data, Offset + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10); }
        public float Y { get => (int)BitConverter.ToSingle(Data, Offset + 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18); }
        public float R { get => (int)BitConverter.ToSingle(Data, Offset + 0x20); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x20); }

        public void UpdateOverworldCoordinates()
        {
            var o = ((SAV8)SAV).Overworld;
            o.M = M;
            o.X = X;
            o.Z = Z;
            o.Y = Y;
            o.R = R;
        }

        public int SpecialLocation
        {
            get => Data[Offset + 0x24];
            set => Data[Offset + 0x24] = (byte)value;
        }

        public int WarpContinueRequest
        {
            get => Data[Offset + 0x6E];
            set => Data[Offset + 0x6E] = (byte)value;
        }

        public int StepCountEgg
        {
            get => BitConverter.ToInt32(Data, Offset + 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x70);
        }

        public int LastZoneID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x74);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x74);
        }

        public int StepCountFriendship
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x76);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x76);
        }

        public int StepCountAffection // Kawaigari
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x78);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x78);
        }
    }
}