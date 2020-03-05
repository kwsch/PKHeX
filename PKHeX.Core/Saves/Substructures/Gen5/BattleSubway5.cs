using System;

namespace PKHeX.Core
{
    public sealed class BattleSubway5 : SaveBlock
    {
        public BattleSubway5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public BattleSubway5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public int BP
        {
            get => BitConverter.ToUInt16(Data, Offset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset);
        }

        // Normal
        public int SinglePast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x08);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x08);
        }

        public int SingleRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x1A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x1A);
        }

        public int DoublePast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x0A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x0A);
        }

        public int DoubleRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x1C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x1C);
        }

        public int MultiNPCPast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x0C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x0C);
        }

        public int MultiNPCRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x1E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x1E);
        }

        public int MultiFriendsPast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x0E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x0E);
        }

        public int MultiFriendsRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x20);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x20);
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
            get => BitConverter.ToUInt16(Data, Offset + 0x12);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x12);
        }

        public int SuperSingleRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x24);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x24);
        }

        public int SuperDoublePast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x14);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x14);
        }

        public int SuperDoubleRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x26);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x26);
        }

        public int SuperMultiNPCPast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x16);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x16);
        }

        public int SuperMultiNPCRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x28);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x28);
        }

        public int SuperMultiFriendsPast
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x18);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x18);
        }

        public int SuperMultiFriendsRecord
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x2A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x2A);
        }

        // TODO: Wifi??

    }
}