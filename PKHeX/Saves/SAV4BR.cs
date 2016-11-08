using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public sealed class SAV4BR : SaveFile
    {
        public override string BAKName => $"{FileName} [{Version} #{SaveCount.ToString("0000")}].bak";
        public override string Filter => "PbrSaveData|*";
        public override string Extension => "";

        private const int SAVE_COUNT = 4;
        public SAV4BR(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G4BR] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (SaveUtil.getIsG4BRSAV(Data) != GameVersion.BATREV)
                return;

            Data = DecryptPBRSaveData(data);

            // Detect active save
            if (BigEndian.ToUInt32(Data, 0x1C004C) > BigEndian.ToUInt32(Data, 0x4C))
            {
                byte[] tempData = new byte[0x1C0000];
                Array.Copy(Data, 0, tempData, 0, 0x1C0000);
                Array.Copy(Data, 0x1C0000, Data, 0, 0x1C0000);
                tempData.CopyTo(Data, 0x1C0000);
            }

            SaveSlots = new List<int>();
            SaveNames = new string[SAVE_COUNT];
            for (int i = 0; i < SAVE_COUNT; i++)
            {
                if (BitConverter.ToUInt16(Data, 0x390 + 0x6FF00*i) != 0)
                {
                    SaveSlots.Add(i);
                    SaveNames[i] = Encoding.BigEndianUnicode.GetString(Data, 0x390 + 0x6FF00*i, 0x10);
                }
            }

            CurrentSlot = SaveSlots.First();

            Personal = PersonalTable.DP;
            HeldItems = Legal.HeldItems_DP;

            if (!Exportable)
                resetBoxes();
        }

        private readonly int SaveCount; // TODO : unique save identification
        public override byte[] Write(bool DSV)
        {
            setChecksums();
            return EncryptPBRSaveData(Data);
        }

        // Configuration
        public override SaveFile Clone() { return new SAV4BR(Write(DSV: false)); }

        public readonly List<int> SaveSlots;
        public readonly string[] SaveNames;
        public int CurrentSlot;
        protected override int Box { // 4 save slots, data reading depends on current slot
            get { return 0x978 + 0x6FF00*CurrentSlot; }
            set { }
        }

        public override int SIZE_STORED => PKX.SIZE_4STORED;
        public override int SIZE_PARTY => PKX.SIZE_4PARTY - 0x10; // PBR has a party
        public override PKM BlankPKM => new BK4();
        public override Type PKMType => typeof(BK4);

        public override int MaxMoveID => 467;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_4;
        public override int MaxAbilityID => 123;
        public override int MaxItemID => 536;
        public override int MaxBallID => 0x18;
        public override int MaxGameID => 15;

        public override int MaxEV => 255;
        public override int Generation => 4;
        protected override int GiftCountMax => 1;
        public override int OTLength => 8;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override int BoxCount => 18;
        public override bool HasParty => false;

        // Checksums
        protected override void setChecksums()
        {
            SetChecksum(Data, 0, 0x100, 8);
            SetChecksum(Data, 0, 0x1C0000, 0x1BFF80);
            SetChecksum(Data, 0x1C0000, 0x100, 0x1C0008);
            SetChecksum(Data, 0x1C0000, 0x1C0000, 0x1BFF80 + 0x1C0000);
        }
        public override bool ChecksumsValid
        {
            get {
                bool valid = VerifyChecksum(Data, 0, 0x1C0000, 0x1BFF80);
                valid &= VerifyChecksum(Data, 0, 0x100, 8);
                valid &= VerifyChecksum(Data, 0x1C0000, 0x1C0000, 0x1BFF80 + 0x1C0000);
                valid &= VerifyChecksum(Data, 0x1C0000, 0x100, 0x1C0008);
                return valid;
            }
        }
        public override string ChecksumInfo => $"Checksums valid: {ChecksumsValid}.";

        // Trainer Info
        public override GameVersion Version { get { return GameVersion.BATREV; } protected set { } }

        // Storage
        public override int getPartyOffset(int slot) // TODO
        {
            return -1;
        }
        public override int getBoxOffset(int box)
        {
            return Box + SIZE_STORED * box * 30;
        }

        // Save file does not have Box Name / Wallpaper info
        public override string getBoxName(int box) { return $"BOX {box + 1}"; }
        public override void setBoxName(int box, string value) { }

        public override PKM getPKM(byte[] data)
        {
            byte[] pkm = data.Take(SIZE_STORED).ToArray();
            PKM bk = new BK4(pkm);
            return bk;
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return data;
        }

        protected override void setDex(PKM pkm) { }

        public static byte[] DecryptPBRSaveData(byte[] input)
        {
            byte[] output = new byte[input.Length];
            for (int base_ofs = 0; base_ofs < SaveUtil.SIZE_G4BR; base_ofs += 0x1C0000)
            {
                Array.Copy(input, base_ofs, output, base_ofs, 8);

                ushort[] keys = new ushort[4];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = BigEndian.ToUInt16(input, base_ofs + i * 2);

                for (int ofs = base_ofs + 8; ofs < base_ofs + 0x1C0000; ofs += 8)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        ushort val = BigEndian.ToUInt16(input, ofs + i*2);
                        val -= keys[i];
                        BigEndian.GetBytes(val).CopyTo(output, ofs + i*2);
                    }
                    keys = SaveUtil.AdvanceGCKeys(keys);
                }
            }
            return output;
        }

        private static byte[] EncryptPBRSaveData(byte[] input)
        {
            byte[] output = new byte[input.Length];
            for (int base_ofs = 0; base_ofs < SaveUtil.SIZE_G4BR; base_ofs += 0x1C0000)
            {
                Array.Copy(input, base_ofs, output, base_ofs, 8);

                ushort[] keys = new ushort[4];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = BigEndian.ToUInt16(input, base_ofs + i * 2);

                for (int ofs = base_ofs + 8; ofs < base_ofs + 0x1C0000; ofs += 8)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        ushort val = BigEndian.ToUInt16(input, ofs + i * 2);
                        val += keys[i];
                        BigEndian.GetBytes(val).CopyTo(output, ofs + i * 2);
                    }
                    keys = SaveUtil.AdvanceGCKeys(keys);
                }
            }
            return output;
        }

        public static bool VerifyChecksum(byte[] input, int offset, int len, int checksum_offset)
        {
            uint[] storedChecksums = new uint[16];
            for (int i = 0; i < storedChecksums.Length; i++)
            {
                storedChecksums[i] = BigEndian.ToUInt32(input, checksum_offset + i*4);
                BitConverter.GetBytes((uint)0).CopyTo(input, checksum_offset + i*4);
            }

            uint[] checksums = new uint[16];

            for (int i = 0; i < len; i += 2)
            {
                ushort val = BigEndian.ToUInt16(input, offset + i);
                for (int j = 0; j < 16; j++)
                {
                    checksums[j] += (uint)((val >> j) & 1);
                }
            }

            for (int i = 0; i < storedChecksums.Length; i++)
            {
                BigEndian.GetBytes(storedChecksums[i]).CopyTo(input, checksum_offset + i*4);
            }

            return checksums.SequenceEqual(storedChecksums);
        }

        public static void SetChecksum(byte[] input, int offset, int len, int checksum_offset)
        {
            uint[] storedChecksums = new uint[16];
            for (int i = 0; i < storedChecksums.Length; i++)
            {
                storedChecksums[i] = BigEndian.ToUInt32(input, checksum_offset + i * 4);
                BitConverter.GetBytes((uint)0).CopyTo(input, checksum_offset + i * 4);
            }

            uint[] checksums = new uint[16];

            for (int i = 0; i < len; i += 2)
            {
                ushort val = BigEndian.ToUInt16(input, offset + i);
                for (int j = 0; j < 16; j++)
                {
                    checksums[j] += (uint)((val >> j) & 1);
                }
            }

            for (int i = 0; i < checksums.Length; i++)
            {
                BigEndian.GetBytes(checksums[i]).CopyTo(input, checksum_offset + i * 4);
            }
        }
    }
}
