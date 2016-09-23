using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public sealed class SAV4BR : SaveFile
    {
        public override string BAKName => $"{FileName} [{Version} #{SaveCount.ToString("0000")}].bak";
        public override string Filter => "PbrSaveData|*";
        public override string Extension => "";

        public SAV4BR(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G4BR] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (SaveUtil.getIsG4BRSAV(Data) != GameVersion.BATREV)
                return;

            Data = DecryptPBRSaveData(data);

            // Detect active save
            if (Util.SwapEndianness(BitConverter.ToUInt32(Data, 0x1C004C)) >
                Util.SwapEndianness(BitConverter.ToUInt32(Data, 0x4C)))
            {
                byte[] tempData = new byte[0x1C0000];
                Array.Copy(Data, 0, tempData, 0, 0x1C0000);
                Array.Copy(Data, 0x1C0000, Data, 0, 0x1C0000);
                tempData.CopyTo(Data, 0x1C0000);
            }

            SaveSlots = new List<int>();
            SaveNames = new string[4];
            for (int i = 0; i < 4; i++)
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

        private readonly int SaveCount;
        public override byte[] Write(bool dsv = false)
        {
            setChecksums();
            return EncryptPBRSaveData(Data);
        }

        // Configuration
        public override SaveFile Clone() { return new SAV4BR(Write()); }

        public List<int> SaveSlots;
        public string[] SaveNames;
        public int CurrentSlot;
        protected override int Box {
            get { return 0x978 + 0x6FF00*CurrentSlot; }
            set { }
        }

        public override int SIZE_STORED => PKX.SIZE_4STORED;
        public override int SIZE_PARTY => PKX.SIZE_4PARTY - 0x10; // PBR has a party
        public override PKM BlankPKM => new BK4();
        public override Type PKMType => typeof(BK4);

        public override int MaxMoveID => 467;
        public override int MaxSpeciesID => 493;
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
        public override string ChecksumInfo
        {
            get
            {
                return $"Checksums valid: {ChecksumsValid}.";
            }
        }

        // Trainer Info
        public override GameVersion Version { get { return GameVersion.BATREV; } protected set { } }

        // Storage
        public override int getPartyOffset(int slot)
        {
            return -1;
        }
        public override int getBoxOffset(int box)
        {
            return Box + SIZE_STORED * box * 30;
        }
        public override int CurrentBox
        {
            get { return 0; }
            set { /* This isn't a real field. */ }
        }
        public override int getBoxWallpaper(int box)
        {
            return 0;
        }
        public override string getBoxName(int box)
        {
            return $"BOX {box + 1}";
        }
        public override void setBoxName(int box, string value)
        {
            /* No custom box names here. */
        }
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

        public override void setStoredSlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new InvalidCastException($"PKM Format needs to be {PKMType} when setting to a Battle Revolution Save File.");
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);
            byte[] data = pkm.EncryptedBoxData;
            setData(data, offset);

            Edited = true;
        }

        public static byte[] DecryptPBRSaveData(byte[] input)
        {
            byte[] output = new byte[input.Length];
            for (int base_ofs = 0; base_ofs < SaveUtil.SIZE_G4BR; base_ofs += 0x1C0000)
            {

                Array.Copy(input, base_ofs, output, base_ofs, 8);

                ushort[] keys = new ushort[4];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = Util.SwapEndianness(BitConverter.ToUInt16(input, base_ofs + i * 2));

                for (int ofs = base_ofs + 8; ofs < base_ofs + 0x1C0000; ofs += 8)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        ushort val = Util.SwapEndianness(BitConverter.ToUInt16(input, ofs + i*2));
                        val -= keys[i];
                        BitConverter.GetBytes(Util.SwapEndianness(val)).CopyTo(output, ofs + i*2);
                    }
                    ushort[] oldKeys = (ushort[])keys.Clone();
                    oldKeys[0] += 0x43;
                    oldKeys[1] += 0x29;
                    oldKeys[2] += 0x17;
                    oldKeys[3] += 0x13;
                    keys[0] = (ushort)((oldKeys[0] & 0xf) | ((oldKeys[1] << 4) & 0xf0) | ((oldKeys[2] << 8) & 0xf00) | ((oldKeys[3] << 12) & 0xf000));
                    keys[1] = (ushort)(((oldKeys[0] >> 4) & 0xf) | (oldKeys[1] & 0xf0) | ((oldKeys[2] << 4) & 0xf00) | ((oldKeys[3] << 8) & 0xf000));
                    keys[2] = (ushort)((oldKeys[2] & 0xf00) | ((oldKeys[1] & 0xf00) >> 4) | ((oldKeys[0] & 0xf00) >> 8) | ((oldKeys[3] << 4) & 0xf000));
                    keys[3] = (ushort)(((oldKeys[0] >> 12) & 0xf) | ((oldKeys[1] >> 8) & 0xf0) | ((oldKeys[2] >> 4) & 0xf00) | (oldKeys[3] & 0xf000));
                }
            }
            return output;
        }

        public static byte[] EncryptPBRSaveData(byte[] input)
        {
            byte[] output = new byte[input.Length];
            for (int base_ofs = 0; base_ofs < SaveUtil.SIZE_G4BR; base_ofs += 0x1C0000)
            {

                Array.Copy(input, base_ofs, output, base_ofs, 8);

                ushort[] keys = new ushort[4];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = Util.SwapEndianness(BitConverter.ToUInt16(input, base_ofs + i * 2));

                for (int ofs = base_ofs + 8; ofs < base_ofs + 0x1C0000; ofs += 8)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        ushort val = Util.SwapEndianness(BitConverter.ToUInt16(input, ofs + i * 2));
                        val += keys[i];
                        BitConverter.GetBytes(Util.SwapEndianness(val)).CopyTo(output, ofs + i * 2);
                    }
                    ushort[] oldKeys = (ushort[])keys.Clone();
                    oldKeys[0] += 0x43;
                    oldKeys[1] += 0x29;
                    oldKeys[2] += 0x17;
                    oldKeys[3] += 0x13;
                    keys[0] = (ushort)((oldKeys[0] & 0xf) | ((oldKeys[1] << 4) & 0xf0) | ((oldKeys[2] << 8) & 0xf00) | ((oldKeys[3] << 12) & 0xf000));
                    keys[1] = (ushort)(((oldKeys[0] >> 4) & 0xf) | (oldKeys[1] & 0xf0) | ((oldKeys[2] << 4) & 0xf00) | ((oldKeys[3] << 8) & 0xf000));
                    keys[2] = (ushort)((oldKeys[2] & 0xf00) | ((oldKeys[1] & 0xf00) >> 4) | ((oldKeys[0] & 0xf00) >> 8) | ((oldKeys[3] << 4) & 0xf000));
                    keys[3] = (ushort)(((oldKeys[0] >> 12) & 0xf) | ((oldKeys[1] >> 8) & 0xf0) | ((oldKeys[2] >> 4) & 0xf00) | (oldKeys[3] & 0xf000));
                }
            }
            return output;
        }

        public static bool VerifyChecksum(byte[] input, int offset, int len, int checksum_offset)
        {
            uint[] storedChecksums = new uint[16];
            for (int i = 0; i < storedChecksums.Length; i++)
            {
                storedChecksums[i] = Util.SwapEndianness(BitConverter.ToUInt32(input, checksum_offset + i*4));
                BitConverter.GetBytes((uint) 0).CopyTo(input, checksum_offset + i*4);
            }

            uint[] checksums = new uint[16];

            for (int i = 0; i < len; i += 2)
            {
                ushort val = Util.SwapEndianness(BitConverter.ToUInt16(input, offset + i));
                for (int j = 0; j < 16; j++)
                {
                    checksums[j] += (uint)((val >> j) & 1);
                }
            }

            for (int i = 0; i < storedChecksums.Length; i++)
            {
                BitConverter.GetBytes(Util.SwapEndianness(storedChecksums[i])).CopyTo(input, checksum_offset + i*4);
            }

            return checksums.SequenceEqual(storedChecksums);
        }

        public static void SetChecksum(byte[] input, int offset, int len, int checksum_offset)
        {
            uint[] storedChecksums = new uint[16];
            for (int i = 0; i < storedChecksums.Length; i++)
            {
                storedChecksums[i] = Util.SwapEndianness(BitConverter.ToUInt32(input, checksum_offset + i * 4));
                BitConverter.GetBytes((uint)0).CopyTo(input, checksum_offset + i * 4);
            }

            uint[] checksums = new uint[16];

            for (int i = 0; i < len; i += 2)
            {
                ushort val = Util.SwapEndianness(BitConverter.ToUInt16(input, offset + i));
                for (int j = 0; j < 16; j++)
                {
                    checksums[j] += (uint)((val >> j) & 1);
                }
            }

            for (int i = 0; i < checksums.Length; i++)
            {
                BitConverter.GetBytes(Util.SwapEndianness(checksums[i])).CopyTo(input, checksum_offset + i * 4);
            }
        }

        private void SwapBytes(byte[] input, int offset, int num_bytes)
        {
            input.Skip(offset).Take(num_bytes).Reverse().ToArray().CopyTo(input, offset);
        }
    }
}
