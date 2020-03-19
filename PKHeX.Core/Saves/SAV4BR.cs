using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 <see cref="SaveFile"/> object for Pokémon Battle Revolution saves.
    /// </summary>
    public sealed class SAV4BR : SaveFile
    {
        protected override string BAKText => $"{Version} #{SaveCount:0000}";
        public override string Filter => "PbrSaveData|*";
        public override string Extension => string.Empty;
        public override PersonalTable Personal => PersonalTable.DP;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_DP;

        private const int SAVE_COUNT = 4;

        public SAV4BR() : base(SaveUtil.SIZE_G4BR)
        {
            ClearBoxes();
        }

        public SAV4BR(byte[] data) : base(data)
        {
            InitializeData(data);
        }

        private void InitializeData(byte[] data)
        {
            Data = DecryptPBRSaveData(data);

            // Detect active save
            SaveCount = Math.Max(BigEndian.ToUInt32(Data, 0x1C004C), BigEndian.ToUInt32(Data, 0x4C));
            if (BigEndian.ToUInt32(Data, 0x1C004C) > BigEndian.ToUInt32(Data, 0x4C))
            {
                byte[] tempData = new byte[0x1C0000];
                Array.Copy(Data, 0, tempData, 0, 0x1C0000);
                Array.Copy(Data, 0x1C0000, Data, 0, 0x1C0000);
                tempData.CopyTo(Data, 0x1C0000);
            }

            for (int i = 0; i < SAVE_COUNT; i++)
            {
                if (!IsOTNamePresent(i))
                    continue;
                SaveSlots.Add(i);
                SaveNames.Add(GetOTName(i));
            }

            CurrentSlot = SaveSlots[0];
        }

        private bool IsOTNamePresent(int i)
        {
            return BitConverter.ToUInt16(Data, 0x390 + (0x6FF00 * i)) != 0;
        }

        private uint SaveCount;

        protected override byte[] GetFinalData()
        {
            SetChecksums();
            return EncryptPBRSaveData(Data);
        }

        // Configuration
        public override SaveFile Clone() => new SAV4BR(Write());

        public readonly List<int> SaveSlots = new List<int>(SAVE_COUNT);
        public readonly List<string> SaveNames = new List<string>(SAVE_COUNT);

        private int _currentSlot;

        public int CurrentSlot
        {
            get => SaveSlots.IndexOf(_currentSlot);
            // 4 save slots, data reading depends on current slot
            set
            {
                _currentSlot = SaveSlots[value];
                var ofs = 0x6FF00 * _currentSlot;
                Box = ofs + 0x978;
                Party = ofs + 0x13A54; // first team slot after boxes
                BoxName = ofs + 0x58674;
            }
        }

        public override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_4STORED + 4;
        public override PKM BlankPKM => new BK4();
        public override Type PKMType => typeof(BK4);

        public override int MaxMoveID => 467;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_4;
        public override int MaxAbilityID => Legal.MaxAbilityID_4;
        public override int MaxItemID => Legal.MaxItemID_4_HGSS;
        public override int MaxBallID => Legal.MaxBallID_4;
        public override int MaxGameID => Legal.MaxGameID_4;

        public override int MaxEV => 255;
        public override int Generation => 4;
        protected override int GiftCountMax => 1;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;
        public override int Language => (int)LanguageID.English; // prevent KOR from inhabiting

        public override int BoxCount => 18;

        public override int PartyCount
        {
            get
            {
                int ctr = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (Data[GetPartyOffset(i) + 4] != 0) // sanity
                        ctr++;
                }
                return ctr;
            }
            protected set
            {
                // Ignore, value is calculated
            }
        }

        // Checksums
        protected override void SetChecksums()
        {
            SetChecksum(Data, 0, 0x100, 8);
            SetChecksum(Data, 0, 0x1C0000, 0x1BFF80);
            SetChecksum(Data, 0x1C0000, 0x100, 0x1C0008);
            SetChecksum(Data, 0x1C0000, 0x1C0000, 0x1BFF80 + 0x1C0000);
        }

        public override bool ChecksumsValid => IsChecksumsValid(Data);
        public override string ChecksumInfo => $"Checksums valid: {ChecksumsValid}.";

        public static bool IsChecksumsValid(byte[] sav)
        {
            return VerifyChecksum(sav, 0x000000, 0x1C0000, 0x1BFF80)
                && VerifyChecksum(sav, 0x000000, 0x000100, 0x000008)
                && VerifyChecksum(sav, 0x1C0000, 0x1C0000, 0x1BFF80 + 0x1C0000)
                && VerifyChecksum(sav, 0x1C0000, 0x000100, 0x1C0008);
        }

        // Trainer Info
        public override GameVersion Version { get => GameVersion.BATREV; protected set { } }

        private string GetOTName(int slot)
        {
            var ofs = 0x390 + (0x6FF00 * slot);
            var str = Encoding.BigEndianUnicode.GetString(Data, ofs, 0x10);
            return Util.TrimFromZero(str);
        }

        private void SetOTName(int slot, string name)
        {
            if (name.Length > 7)
                name = name.Substring(0, 7);
            var bytes = Encoding.BigEndianUnicode.GetBytes(name.PadRight(8, '\0'));
            var ofs = 0x390 + (0x6FF00 * slot);
            SetData(bytes, ofs);
        }

        public string CurrentOT { get => GetOTName(_currentSlot); set => SetOTName(_currentSlot, value); }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);

        // Save file does not have Box Name / Wallpaper info
        private int BoxName = -1;
        private const int BoxNameLength = 0x28;

        public override string GetBoxName(int box)
        {
            if (BoxName < 0)
                return $"BOX {box + 1}";

            var str = Encoding.BigEndianUnicode.GetString(Data, BoxName + (box * BoxNameLength), BoxNameLength);
            str = Util.TrimFromZero(str);
            if (string.IsNullOrWhiteSpace(str))
                return $"BOX {box + 1}";
            return str;
        }

        public override void SetBoxName(int box, string value)
        {
            if (BoxName < 0)
                return;

            int ofs = BoxName + (box * BoxNameLength);
            var str = Encoding.BigEndianUnicode.GetString(Data, ofs, BoxNameLength);
            str = Util.TrimFromZero(str);
            if (string.IsNullOrWhiteSpace(str))
                return;

            var data = Encoding.BigEndianUnicode.GetBytes(value.PadLeft(BoxNameLength / 2, '\0'));
            SetData(data, ofs);
        }

        protected override PKM GetPKM(byte[] data)
        {
            if (data.Length != SIZE_STORED)
                Array.Resize(ref data, SIZE_STORED);
            return BK4.ReadUnshuffle(data);
        }

        protected override byte[] DecryptPKM(byte[] data) => data;

        protected override void SetDex(PKM pkm) { /* There's no PokéDex */ }

        protected override void SetPKM(PKM pkm)
        {
            var pk4 = (BK4)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            if (pk4.Trade(OT, TID, SID, Gender, Date.Day, Date.Month, Date.Year))
                pkm.RefreshChecksum();
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            pkm.Sanity = (ushort)(isParty ? 0xC000 : 0x4000);
        }

        public static byte[] DecryptPBRSaveData(byte[] input)
        {
            byte[] output = new byte[input.Length];
            for (int i = 0; i < SaveUtil.SIZE_G4BR; i += 0x1C0000)
            {
                var keys = GetKeys(input, i);
                Array.Copy(input, i, output, i, 8);
                GCSaveUtil.Decrypt(input, i + 8, i + 0x1C0000, keys, output);
            }
            return output;
        }

        private static byte[] EncryptPBRSaveData(byte[] input)
        {
            byte[] output = new byte[input.Length];
            for (int i = 0; i < SaveUtil.SIZE_G4BR; i += 0x1C0000)
            {
                var keys = GetKeys(input, i);
                Array.Copy(input, i, output, i, 8);
                GCSaveUtil.Encrypt(input, i + 8, i + 0x1C0000, keys, output);
            }
            return output;
        }

        private static ushort[] GetKeys(byte[] input, int ofs)
        {
            ushort[] keys = new ushort[4];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = BigEndian.ToUInt16(input, ofs + (i * 2));
            return keys;
        }

        public static bool VerifyChecksum(byte[] input, int offset, int len, int checksum_offset)
        {
            uint[] storedChecksums = new uint[16];
            for (int i = 0; i < storedChecksums.Length; i++)
            {
                storedChecksums[i] = BigEndian.ToUInt32(input, checksum_offset + (i * 4));
                BitConverter.GetBytes(0u).CopyTo(input, checksum_offset + (i * 4));
            }

            uint[] checksums = new uint[16];

            for (int i = 0; i < len; i += 2)
            {
                uint val = BigEndian.ToUInt16(input, offset + i);
                for (int j = 0; j < 16; j++)
                {
                    checksums[j] += ((val >> j) & 1);
                }
            }

            for (int i = 0; i < storedChecksums.Length; i++)
            {
                BigEndian.GetBytes(storedChecksums[i]).CopyTo(input, checksum_offset + (i * 4));
            }

            return checksums.SequenceEqual(storedChecksums);
        }

        private static void SetChecksum(byte[] input, int offset, int len, int checksum_offset)
        {
            uint[] storedChecksums = new uint[16];
            for (int i = 0; i < storedChecksums.Length; i++)
            {
                storedChecksums[i] = BigEndian.ToUInt32(input, checksum_offset + (i * 4));
                BitConverter.GetBytes(0u).CopyTo(input, checksum_offset + (i * 4));
            }

            uint[] checksums = new uint[16];

            for (int i = 0; i < len; i += 2)
            {
                uint val = BigEndian.ToUInt16(input, offset + i);
                for (int j = 0; j < 16; j++)
                    checksums[j] += ((val >> j) & 1);
            }

            for (int i = 0; i < checksums.Length; i++)
            {
                BigEndian.GetBytes(checksums[i]).CopyTo(input, checksum_offset + (i * 4));
            }
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter4.GetBEString4(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter4.SetBEString4(value, maxLength, PadToSize, PadWith);
        }
    }
}
