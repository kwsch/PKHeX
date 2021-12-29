using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class BattleSubway5 : SaveBlock
    {
        public BattleSubway5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public BattleSubway5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public int BP
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset), (ushort)value);
        }

        // Normal
        public int SinglePast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x08));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x08), (ushort)value);
        }

        public int SingleRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1A));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1A), (ushort)value);
        }

        public int DoublePast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0A));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0A), (ushort)value);
        }

        public int DoubleRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1C));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1C), (ushort)value);
        }

        public int MultiNPCPast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0C));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0C), (ushort)value);
        }

        public int MultiNPCRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1E));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1E), (ushort)value);
        }

        public int MultiFriendsPast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0E));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0E), (ushort)value);
        }

        public int MultiFriendsRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x20));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x20), (ushort)value);
        }

        // Super Check
        public int SuperCheck
        {
            get => Data[Offset + 0x04];
            set => Data[Offset + 0x04] = (byte)value;
        }

        // Super
        public int SuperSinglePast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x12));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x12), (ushort)value);
        }

        public int SuperSingleRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x24));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x24), (ushort)value);
        }

        public int SuperDoublePast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), (ushort)value);
        }

        public int SuperDoubleRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x26));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x26), (ushort)value);
        }

        public int SuperMultiNPCPast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x16));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x16), (ushort)value);
        }

        public int SuperMultiNPCRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x28));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x28), (ushort)value);
        }

        public int SuperMultiFriendsPast
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x18));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x18), (ushort)value);
        }

        public int SuperMultiFriendsRecord
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x2A));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x2A), (ushort)value);
        }

        // TODO: Wifi??

    }
}