using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class Situation7 : SaveBlock
    {
        public Situation7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public Situation7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        // "StartLocation"
        public int M { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x00), (ushort)value); }
        public float X { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x08)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x08), value); }
        public float Z { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x10), value); }
        public float Y { get => (int)ReadSingleLittleEndian(Data.AsSpan(Offset + 0x18)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x18), value); }
        public float R { get => (int)ReadSingleLittleEndian(Data.AsSpan(Offset + 0x20)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x20), value); }

        public void UpdateOverworldCoordinates()
        {
            var o = ((SAV7)SAV).Overworld;
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
            get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x70));
            set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x70), value);
        }

        public int LastZoneID
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x74));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x74), (ushort)value);
        }

        public int StepCountFriendship
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x76));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x76), (ushort)value);
        }

        public int StepCountAffection // Kawaigari
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x78));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x78), (ushort)value);
        }
    }
}