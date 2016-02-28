using System;
using System.Linq;

namespace PKHeX
{
    public class SAV5 : PKM
    {
        internal const int SIZERAW = 0x80000; // 512KB
        internal const int SIZE1 = 0x24000; // B/W
        internal const int SIZE2 = 0x26000; // B2/W2

        // Global Settings
        // Save Data Attributes
        public readonly byte[] Data;
        public bool Edited;
        public readonly bool Exportable;
        public readonly byte[] BAK;
        public string FileName, FilePath;
        public SAV5(byte[] data = null)
        {
            Data = (byte[])(data ?? new byte[SIZERAW]).Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Get Version
            // Validate Checksum over the Checksum Block to find BW or B2W2
            ushort chk2 = BitConverter.ToUInt16(Data, SIZE2 - 0x100 + 0x94 + 0x10);
            ushort actual2 = ccitt16(Data.Skip(SIZE2 - 0x100).Take(0x94).ToArray());
            bool B2W2 = chk2 == actual2;
            ushort chk1 = BitConverter.ToUInt16(Data, SIZE1 - 0x100 + 0x94 + 0x10);
            ushort actual1 = ccitt16(Data.Skip(SIZE1 - 0x100).Take(0x94).ToArray());
            bool BW = chk1 == actual1;

            if (!BW && !B2W2)
                Data = null; // Invalid/Not G5 Save File

            // Different Offsets for different games.
            BattleBox = B2W2 ? 0x20A00 : 0x20900;
        }

        private readonly int Box = 0x400;
        private readonly int Party = 0x18E00;
        private readonly int BattleBox;

        public int PartyCount
        {
            get { return Data[Party]; }
            set { Data[Party] = (byte)value; }
        }

        public PK5[] BoxData
        {
            get
            {
                PK5[] data = new PK5[24 * 30];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK5Stored(Box + PK5.SIZE_STORED * i);
                    data[i].Identifier = $"B{(i / 30 + 1).ToString("00")}:{(i % 30 + 1).ToString("00")}";
                }
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != 24 * 30)
                    throw new ArgumentException("Expected 720, got " + value.Length);

                for (int i = 0; i < value.Length; i++)
                    setPK5Stored(value[i], Box + PK5.SIZE_STORED * i);
            }
        }
        public PK5[] PartyData
        {
            get
            {
                PK5[] data = new PK5[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = getPK5Party(Party + 8 + PK5.SIZE_PARTY * i);
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0 || value.Length > 6)
                    throw new ArgumentException("Expected 1-6, got " + value.Length);
                if (value[0].Species == 0)
                    throw new ArgumentException("Can't have an empty first slot." + value.Length);

                PK5[] newParty = value.Where(pk => pk.Species != 0).ToArray();

                PartyCount = newParty.Length;
                Array.Resize(ref newParty, 6);

                for (int i = PartyCount; i < newParty.Length; i++)
                    newParty[i] = new PK5();
                for (int i = 0; i < newParty.Length; i++)
                    setPK5Party(newParty[i], Party + 8 + PK6.SIZE_PARTY * i);
            }
        }
        public PK5[] BattleBoxData
        {
            get
            {
                PK5[] data = new PK5[6];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK5Stored(BattleBox + PK5.SIZE_STORED * i);
                    if (data[i].Species == 0)
                        return data.Take(i).ToArray();
                }
                return data;
            }
        }

        public PK5 getPK5Party(int offset)
        {
            return new PK5(decryptArray(getData(offset, PK5.SIZE_PARTY)));
        }
        public PK5 getPK5Stored(int offset)
        {
            return new PK5(decryptArray(getData(offset, PK5.SIZE_STORED)));
        }
        public void setPK5Party(PK5 pk5, int offset)
        {
            if (pk5 == null) return;

            setData(pk5.EncryptedPartyData, offset);
            Edited = true;
        }
        public void setPK5Stored(PK5 pk5, int offset)
        {
            if (pk5 == null) return;

            setData(pk5.EncryptedBoxData, offset);
            Edited = true;
        }
        public byte[] getData(int Offset, int Length)
        {
            return Data.Skip(Offset).Take(Length).ToArray();
        }
        public void setData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
            Edited = true;
        }
    }
}
