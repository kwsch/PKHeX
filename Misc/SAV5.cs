using System;
using System.Linq;

namespace PKHeX
{
    public class SAV5 : PKM
    {
        internal const int SIZERAW = 0x80000; // 512KB
        internal const int SIZE1 = 0x24000; // B/W
        internal const int SIZE2 = 0x26000; // B2/W2

        internal static int getIsG5SAV(byte[] data)
        {
            ushort chk1 = BitConverter.ToUInt16(data, SIZE1 - 0x100 + 0x94 + 0x10);
            ushort actual1 = ccitt16(data.Skip(SIZE1 - 0x100).Take(0x94).ToArray());
            if (chk1 == actual1)
                return 0;
            ushort chk2 = BitConverter.ToUInt16(data, SIZE2 - 0x100 + 0x94 + 0x10);
            ushort actual2 = ccitt16(data.Skip(SIZE2 - 0x100).Take(0x94).ToArray());
            if (chk2 == actual2)
                return 1;
            return -1;
        }

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
            int version = getIsG5SAV(Data);
            if (version < 0) // Invalidate Data
                Data = null;

            // Different Offsets for different games.
            BattleBox = version == 1 ? 0x20A00 : 0x20900;
        }

        private const int Box = 0x400;
        private const int Party = 0x18E00;
        private readonly int BattleBox;
        private const int Trainer = 0x19400;
        private const int Wondercard = 0x1C800;
        private const int wcSeed = 0x1D290;

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
                    setPK5Party(newParty[i], Party + 8 + PK5.SIZE_PARTY * i);
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

        public class MysteryGift
        {
            public readonly PGF[] Cards = new PGF[12];
            public readonly bool[] UsedFlags = new bool[0x800];
            public uint Seed;
        }
        public MysteryGift WondercardInfo
        {
            get
            {
                uint seed = BitConverter.ToUInt32(Data, wcSeed);
                MysteryGift Info = new MysteryGift { Seed = seed };
                byte[] wcData = Data.Skip(Wondercard).Take(0xA90).ToArray(); // Encrypted, Decrypt
                for (int i = 0; i < wcData.Length; i += 2)
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(wcData, i) ^ LCRNG(ref seed) >> 16)).CopyTo(wcData, i);

                // 0x100 Bytes for Used Flags
                for (int i = 0; i < Info.UsedFlags.Length; i++)
                    Info.UsedFlags[i] = (wcData[i/8] >> i%8 & 0x1) == 1;
                // 12 PGFs
                for (int i = 0; i < Info.Cards.Length; i++)
                    Info.Cards[i] = new PGF(Data.Skip(0x100 + i*PGF.Size).Take(PGF.Size).ToArray());

                return Info;
            }
            set
            {
                MysteryGift Info = value;
                byte[] wcData = new byte[0xA90];

                // Toss back into byte[]
                for (int i = 0; i < Info.UsedFlags.Length; i++)
                    if (Info.UsedFlags[i])
                        wcData[i/8] |= (byte)(1 << (i & 7));
                for (int i = 0; i < Info.Cards.Length; i++)
                    Info.Cards[i].Data.CopyTo(wcData, 0x100 + i*PGF.Size);

                // Decrypted, Encrypt
                uint seed = Info.Seed;
                for (int i = 0; i < wcData.Length; i += 2)
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(wcData, i) ^ LCRNG(ref seed) >> 16)).CopyTo(wcData, i);
                BitConverter.GetBytes(Info.Seed).CopyTo(Data, wcSeed);
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
