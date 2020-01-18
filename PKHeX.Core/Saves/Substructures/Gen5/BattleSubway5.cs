using System;

namespace PKHeX.Core
{
    public sealed class BattleSubway5 : SaveBlock
    {
        public BattleSubway5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public BattleSubway5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        private const int superCheckOffset = 0x21D04;

        private const int singlePasOffset = 0x21D08;
        private const int singleRecOffset = 0x21D1A;
        private const int doublePasOffset = 0x21D0A;
        private const int doubleRecOffset = 0x21D1C;
        private const int multiNpcPasOffset = 0x21D0C;
        private const int multiNpcRecOffset = 0x21D1E;
        private const int multiFrPasOffset = 0x21D0E;
        private const int multiFrRecOffset = 0x21D20;

        // TODO: Super 
        private const int superSinglePasOffset = 0x21D12;
        private const int superSingleRecOffset = 0x21D24;
        private const int superDoublePasOffset = 0x21D14;
        private const int superDoubleRecOffset = 0x21D26;
        private const int superMultiNpcPasOffset = 0x21D16;
        private const int superMultiNpcRecOffset = 0x21D28;
        private const int superMultiFrPasOffset = 0x21D18;
        private const int superMultiFrRecOffset = 0x21D2A;

        // TODO: Wifi???

        public int BP
        {
            get => BitConverter.ToUInt16(Data, Offset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset);
        }

        // Normal
        public int SinglePast
        {
            get => BitConverter.ToUInt16(Data, singlePasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, singlePasOffset);
        }

        public int SingleRecord
        {
            get => BitConverter.ToUInt16(Data, singleRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, singleRecOffset);
        }

        public int DoublePast
        {
            get => BitConverter.ToUInt16(Data, doublePasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, doublePasOffset);
        }

        public int DoubleRecord
        {
            get => BitConverter.ToUInt16(Data, doubleRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, doubleRecOffset);
        }

        public int MultiNPCPast
        {
            get => BitConverter.ToUInt16(Data, multiNpcPasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, multiNpcPasOffset);
        }

        public int MultiNPCRecord
        {
            get => BitConverter.ToUInt16(Data, multiNpcRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, multiNpcRecOffset);
        }

        public int MultiFriendsPast
        {
            get => BitConverter.ToUInt16(Data, multiFrPasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, multiFrPasOffset);
        }

        public int MultiFriendsRecord
        {
            get => BitConverter.ToUInt16(Data, multiFrRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, multiFrRecOffset);
        }

        // Super Check
        public int SuperCheck
        {
            get => BitConverter.ToUInt16(Data, superCheckOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superCheckOffset);
        }

        // Super
        public int SuperSinglePast
        {
            get => BitConverter.ToUInt16(Data, superSinglePasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superSinglePasOffset);
        }

        public int SuperSingleRecord
        {
            get => BitConverter.ToUInt16(Data, superSingleRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superSingleRecOffset);
        }

        public int SuperDoublePast
        {
            get => BitConverter.ToUInt16(Data, superDoublePasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superDoublePasOffset);
        }

        public int SuperDoubleRecord
        {
            get => BitConverter.ToUInt16(Data, superDoubleRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superDoubleRecOffset);
        }
        public int SuperMultiNPCPast
        {
            get => BitConverter.ToUInt16(Data, superMultiNpcPasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superMultiNpcPasOffset);
        }

        public int SuperMultiNPCRecord
        {
            get => BitConverter.ToUInt16(Data, superMultiNpcRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superMultiNpcRecOffset);
        }
        public int SuperMultiFriendsPast
        {
            get => BitConverter.ToUInt16(Data, superMultiFrPasOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superMultiFrPasOffset);
        }

        public int SuperMultiFriendsRecord
        {
            get => BitConverter.ToUInt16(Data, superMultiFrRecOffset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, superMultiFrRecOffset);
        }


    }
}