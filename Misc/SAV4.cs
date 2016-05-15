using System;
using System.Linq;

namespace PKHeX
{
    public class SAV4 : PKM
    {
        internal const int SIZERAW = 0x80000; // 512KB
        internal static int getIsG4SAV(byte[] data)
        {
            int version = -1;
            if (BitConverter.ToUInt16(data, 0xC0FE) == ccitt16(data.Take(0xC0EC).ToArray()))
                version = 0; // DP
            else if (BitConverter.ToUInt16(data, 0xCF2A) == ccitt16(data.Take(0xCF18).ToArray()))
                version = 1; // PT
            else if (BitConverter.ToUInt16(data, 0xF626) == ccitt16(data.Take(0xF618).ToArray()))
                version = 2; // HGSS
            return version;
        }
        
        // Global Settings
        // Save Data Attributes
        public readonly byte[] Data;
        public bool Edited;
        public readonly bool Exportable;
        public readonly byte[] BAK;
        public string FileName, FilePath;
        public SAV4(byte[] data = null)
        {
            Data = (byte[])(data ?? new byte[SIZERAW]).Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Get Version
            int version = getIsG4SAV(Data);
            getActiveBlock(version);
            getSAVOffsets(version);
        }

        private int generalBlock = -1;
        private int storageBlock = -1;
        private void getActiveBlock(int version)
        {
            if (version < 0)
                return;
            int ofs = 0;

            if (version == 0) ofs = 0xC0F0; // DP
            else if (version == 1) ofs = 0xCF1C; // PT
            else if (version == 2) ofs = 0xF626; // HGSS
            generalBlock = BitConverter.ToUInt16(Data, ofs) >= BitConverter.ToUInt16(Data, ofs + 0x40000) ? 0 : 1;
            
            if (version == 0) ofs = 0x1E2D0; // DP
            else if (version == 1) ofs = 0x1F100; // PT
            else if (version == 2) ofs = 0x21A00; // HGSS
            storageBlock = BitConverter.ToUInt16(Data, ofs) >= BitConverter.ToUInt16(Data, ofs + 0x40000) ? 0 : 1;
        }
        private void getSAVOffsets(int version)
        {
            if (version < 0)
                return;

            switch (version)
            {
                case 0: // DP
                    Party = 0x98 + 0x40000 * generalBlock;
                    Box = 0xC104 + 0x40000 * storageBlock;
                    break;
                case 1: // PT
                    Party = 0xA0 + 0x40000 * generalBlock;
                    Box = 0xCF34 + 0x40000 * storageBlock;
                    break;
                case 2: // HGSS
                    Party = 0x98 + 0x40000 * generalBlock;
                    Box = 0xF704 + 0x40000 * storageBlock;
                    break;
            }
        }

        private int Box, Party = -1;

        public int PartyCount
        {
            get { return Data[Party - 4]; }
            set { Data[Party - 4] = (byte)value; }
        }

        public int BoxCount
        {
            get { return Data[Box - 4]; }
            set { Data[Box - 4] = (byte)value; }
        }
        public PK4[] BoxData
        {
            get
            {
                PK4[] data = new PK4[18 * 30];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK4Stored(Box + PK4.SIZE_STORED * i);
                    data[i].Identifier = $"B{(i / 30 + 1).ToString("00")}:{(i % 30 + 1).ToString("00")}";
                }
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != 18 * 30)
                    throw new ArgumentException("Expected 540, got " + value.Length);

                for (int i = 0; i < value.Length; i++)
                    setPK4Stored(value[i], Box + PK4.SIZE_STORED * i);
            }
        }
        public PK4[] PartyData
        {
            get
            {
                PK4[] data = new PK4[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = getPK4Party(Party + PK4.SIZE_PARTY * i);
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

                PK4[] newParty = value.Where(pk => pk.Species != 0).ToArray();

                PartyCount = newParty.Length;
                Array.Resize(ref newParty, 6);

                for (int i = PartyCount; i < newParty.Length; i++)
                    newParty[i] = new PK4();
                for (int i = 0; i < newParty.Length; i++)
                    setPK4Party(newParty[i], Party + PK4.SIZE_PARTY * i);
            }
        }

        public PK4 getPK4Party(int offset)
        {
            return new PK4(decryptArray(getData(offset, PK4.SIZE_PARTY)));
        }
        public PK4 getPK4Stored(int offset)
        {
            return new PK4(decryptArray(getData(offset, PK4.SIZE_STORED)));
        }
        public void setPK4Party(PK4 pk4, int offset)
        {
            if (pk4 == null) return;

            setData(pk4.EncryptedPartyData, offset);
            Edited = true;
        }
        public void setPK4Stored(PK4 pk4, int offset)
        {
            if (pk4 == null) return;

            setData(pk4.EncryptedBoxData, offset);
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
