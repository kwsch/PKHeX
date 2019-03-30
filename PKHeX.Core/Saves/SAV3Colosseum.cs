using System;
using System.Linq;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object for Pokémon Colosseum saves.
    /// </summary>
    public sealed class SAV3Colosseum : SaveFile, IDisposable, IGCSaveFile
    {
        protected override string BAKText => $"{OT} ({Version}) - {PlayTimeString}";
        public override string Filter => this.GCFilter();
        public override string Extension => this.GCExtension();
        public bool IsMemoryCardSave => MC != null;
        private readonly SAV3GCMemoryCard MC;

        // 3 Save files are stored
        // 0x0000-0x6000 contains memory card data
        // 0x6000-0x60000 contains the 3 save slots
        // 0x5A000 / 3 = 0x1E000 per save slot
        // Checksum is SHA1 over 0-0x1DFD7, stored in the last 20 bytes of the save slot.
        // Another SHA1 hash is 0x1DFD8, for 20 bytes. Unknown purpose.
        // Checksum is used as the crypto key.

        private const int SLOT_SIZE = 0x1E000;
        private const int SLOT_START = 0x6000;
        private const int SLOT_COUNT = 3;

        private int SaveCount = -1;
        private int SaveIndex = -1;
        private readonly StrategyMemo StrategyMemo;
        public int MaxShadowID => 0x80; // 128
        private readonly int Memo;
        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries, LegalCologne;
        private readonly int OFS_PouchCologne;
        public SAV3Colosseum(byte[] data, SAV3GCMemoryCard MC) : this(data) { this.MC = MC; BAK = MC.Data; }

        public SAV3Colosseum(byte[] data = null)
        {
            Data = data ?? new byte[SaveUtil.SIZE_G3COLO];
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            Personal = PersonalTable.RS;
            HeldItems = Legal.HeldItems_COLO;

            if (SaveUtil.GetIsG3COLOSAV(Data) == GameVersion.COLO)
                InitializeData();

            Trainer1 = 0x00078;
            Party = 0x000A8;
            OFS_PouchHeldItem = 0x007F8;
            OFS_PouchKeyItem = 0x00848;
            OFS_PouchBalls = 0x008F4;
            OFS_PouchTMHM = 0x00934;
            OFS_PouchBerry = 0x00A34;
            OFS_PouchCologne = 0x00AEC; // Cologne

            Box = 0x00B90;
            Daycare = 0x08170;
            Memo = 0x082B0;
            StrategyMemo = new StrategyMemo(Data, Memo, xd:false);

            LegalItems = Legal.Pouch_Items_COLO;
            LegalKeyItems = Legal.Pouch_Key_COLO;
            LegalBalls = Legal.Pouch_Ball_RS;
            LegalTMHMs = Legal.Pouch_TM_RS; // not HMs
            LegalBerries = Legal.Pouch_Berries_RS;
            LegalCologne = Legal.Pouch_Cologne_COLO;

            if (!Exportable)
                ClearBoxes();

            // Since PartyCount is not stored in the save file,
            // Count up how many party slots are active.
            for (int i = 0; i < 6; i++)
            {
                if (GetPartySlot(GetPartyOffset(i)).Species != 0)
                    PartyCount++;
            }
        }

        private void InitializeData()
        {
            // Scan all 3 save slots for the highest counter
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                int slotOffset = SLOT_START + (i * SLOT_SIZE);
                int SaveCounter = BigEndian.ToInt32(Data, slotOffset + 4);
                if (SaveCounter <= SaveCount)
                    continue;

                SaveCount = SaveCounter;
                SaveIndex = i;
            }

            // Decrypt most recent save slot
            {
                byte[] slot = new byte[SLOT_SIZE];
                int slotOffset = SLOT_START + (SaveIndex * SLOT_SIZE);
                Array.Copy(Data, slotOffset, slot, 0, slot.Length);
                byte[] digest = new byte[20];
                Array.Copy(slot, SLOT_SIZE - 20, digest, 0, digest.Length);

                // Decrypt Slot
                Data = DecryptColosseum(slot, digest);
            }
        }

        protected override byte[] GetFinalData()
        {
            var newFile = GetInnerData();

            // Return the gci if Memory Card is not being exported
            if (!IsMemoryCardSave)
                return newFile;

            MC.SelectedSaveData = newFile;
            return MC.Data;
        }

        private byte[] GetInnerData()
        {
            StrategyMemo.FinalData.CopyTo(Data, Memo);
            SetChecksums();

            // Get updated save slot data
            byte[] digest = Data.Skip(Data.Length - 20).Take(20).ToArray();
            byte[] newSAV = EncryptColosseum(Data, digest);

            // Put save slot back in original save data
            byte[] newFile = MC != null ? MC.SelectedSaveData : (byte[])BAK.Clone();
            Array.Copy(newSAV, 0, newFile, SLOT_START + (SaveIndex * SLOT_SIZE), newSAV.Length);
            return newFile;
        }

        // Configuration
        public override SaveFile Clone()
        {
            var data = GetInnerData();
            var sav = IsMemoryCardSave ? new SAV3Colosseum(data, MC) : new SAV3Colosseum(data);
            sav.Header = (byte[])Header.Clone();
            return sav;
        }

        public override int SIZE_STORED => PKX.SIZE_3CSTORED;
        protected override int SIZE_PARTY => PKX.SIZE_3CSTORED; // unused
        public override PKM BlankPKM => new CK3();
        public override Type PKMType => typeof(CK3);

        public override int MaxMoveID => Legal.MaxMoveID_3;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => Legal.MaxAbilityID_3;
        public override int MaxBallID => Legal.MaxBallID_3;
        public override int MaxItemID => Legal.MaxItemID_3_COLO;
        public override int MaxGameID => Legal.MaxGameID_3;

        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 10; // as evident by Mattle Ho-Oh
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override int BoxCount => 3;
        public override bool IsPKMPresent(int offset) => PKX.IsPKMPresentGC(Data, offset);

        // Checksums
        private readonly SHA1 sha1 = SHA1.Create();
        public void Dispose() => sha1?.Dispose();

        private byte[] EncryptColosseum(byte[] input, byte[] digest)
        {
            if (input.Length != SLOT_SIZE)
                return null;

            byte[] d = (byte[])input.Clone();
            byte[] k = (byte[])digest.Clone(); // digest

            // NOT key
            for (int i = 0; i < 20; i++)
                k[i] = (byte)~k[i];

            for (int i = 0x18; i < 0x1DFD8; i += 20)
            {
                for (int j = 0; j < 20; j++)
                    d[i + j] ^= k[j];
                k = sha1.ComputeHash(d, i, 20); // update digest
            }
            return d;
        }

        private byte[] DecryptColosseum(byte[] input, byte[] digest)
        {
            if (input.Length != SLOT_SIZE)
                return null;

            byte[] d = (byte[])input.Clone();
            byte[] k = (byte[])digest.Clone();

            // NOT key
            for (int i = 0; i < 20; i++)
                k[i] = (byte)~k[i];

            for (int i = 0x18; i < 0x1DFD8; i += 20)
            {
                byte[] key = sha1.ComputeHash(d, i, 20); // update digest
                for (int j = 0; j < 20; j++)
                    d[i + j] ^= k[j];
                Array.Copy(key, k, 20); // for use in next loop
            }
            return d;
        }

        protected override void SetChecksums()
        {
            // Clear Header Checksum
            BitConverter.GetBytes(0).CopyTo(Data, 12);
            // Compute checksum of data
            byte[] checksum = sha1.ComputeHash(Data, 0, 0x1DFD8);
            // Set Checksum to end
            Array.Copy(checksum, 0, Data, Data.Length - 20, 20);

            // Header Integrity
            byte[] H = new byte[8]; Array.Copy(checksum, 0, H, 0, 8);
            byte[] D = new byte[8]; Array.Copy(Data, 0x18, D, 0, 8);
            // Decrypt Checksum
            for (int i = 0; i < 8; i++)
                D[i] ^= (byte)~H[i];

            // Compute new header checksum
            int newHC = 0;
            for (int i = 0; i < 0x18; i += 4)
                newHC -= BigEndian.ToInt32(Data, i);

            newHC -= BigEndian.ToInt32(D, 0);
            newHC -= BigEndian.ToInt32(D, 4);

            // Set Header Checksum
            BigEndian.GetBytes(newHC).CopyTo(Data, 12);
        }

        public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");

        public override string ChecksumInfo
        {
            get
            {
                byte[] data = (byte[])Data.Clone();
                int oldHC = BigEndian.ToInt32(data, 12);
                // Clear Header Checksum
                BitConverter.GetBytes(0).CopyTo(data, 12);
                byte[] checksum = sha1.ComputeHash(data, 0, 0x1DFD8);
                // Header Integrity
                byte[] H = new byte[8];
                byte[] D = new byte[8];

                Array.Copy(checksum, 0, H, 0, H.Length);
                Array.Copy(data, 0x00018, D, 0, D.Length);
                for (int i = 0; i < 8; i++)
                    D[i] ^= (byte)~H[i];

                // Compute new header checksum
                int newHC = 0;
                for (int i = 0; i < 0x18; i += 4)
                    newHC -= BigEndian.ToInt32(data, i);

                newHC -= BigEndian.ToInt32(D, 0);
                newHC -= BigEndian.ToInt32(D, 4);

                byte[] chk = data.Skip(data.Length - 20).Take(20).ToArray();

                bool header = newHC == oldHC;
                bool body = chk.SequenceEqual(checksum);
                string valid(bool s) => s ? "Valid" : "Invalid";
                return $"Header Checksum {valid(header)}, Body Checksum {valid(body)}alid.";
            }
        }

        // Trainer Info
        public override GameVersion Version { get => GameVersion.COLO; protected set { } }

        // Storage
        public override int GetPartyOffset(int slot)
        {
            return Party + (SIZE_STORED * slot);
        }

        public override int GetBoxOffset(int box)
        {
            return Box + (((30 * SIZE_STORED) + 0x14)*box) + 0x14;
        }

        public override string GetBoxName(int box)
        {
            return GetString(Box + (0x24A4 * box), 16);
        }

        public override void SetBoxName(int box, string value)
        {
            SetString(value, 8).CopyTo(Data, Box + (0x24A4 * box));
        }

        protected override PKM GetPKM(byte[] data)
        {
            if (data.Length != SIZE_STORED)
                Array.Resize(ref data, SIZE_STORED);
            return new CK3(data);
        }

        protected override byte[] DecryptPKM(byte[] data)
        {
            return data;
        }

        protected override void SetPKM(PKM pkm)
        {
            if (!(pkm is CK3 pk))
                return;

            if (pk.CurrentRegion == 0)
                pk.CurrentRegion = 2; // NTSC-U
            if (pk.OriginalRegion == 0)
                pk.OriginalRegion = 2; // NTSC-U
        }

        protected override void SetDex(PKM pkm)
        {
            // Dex Related
            var entry = StrategyMemo.GetEntry(pkm.Species);
            if (entry.IsEmpty) // Populate
            {
                entry.Species = pkm.Species;
                entry.PID = pkm.PID;
                entry.TID = pkm.TID;
                entry.SID = pkm.SID;
            }
            if (entry.Matches(pkm.Species, pkm.PID, pkm.TID, pkm.SID))
            {
                entry.Seen = true;
                entry.Owned = true;
            }
            StrategyMemo.SetEntry(entry);
        }

        private TimeSpan PlayedSpan
        {
            get => TimeSpan.FromSeconds((double)(BigEndian.ToUInt32(Data, 40) - 0x47000000) / 128);
            set => BigEndian.GetBytes((uint)(value.TotalSeconds * 128) + 0x47000000).CopyTo(Data, 40);
        }

        public override int PlayedHours
        {
            get => (ushort)PlayedSpan.Hours;
            set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromHours(time.Hours) + TimeSpan.FromHours(value); }
        }

        public override int PlayedMinutes
        {
            get => (byte)PlayedSpan.Minutes;
            set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromMinutes(time.Minutes) + TimeSpan.FromMinutes(value); }
        }

        public override int PlayedSeconds
        {
            get => (byte)PlayedSpan.Seconds;
            set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromSeconds(time.Seconds) + TimeSpan.FromSeconds(value); }
        }

        // Trainer Info (offset 0x78, length 0xB18, end @ 0xB90)
        public override string OT { get => GetString(0x78, 20); set { SetString(value, 10).CopyTo(Data, 0x78); OT2 = value; } }
        private string OT2 { get => GetString(0x8C, 20); set => SetString(value, 10).CopyTo(Data, 0x8C); }
        public override int SID { get => BigEndian.ToUInt16(Data, 0xA4); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xA4); }
        public override int TID { get => BigEndian.ToUInt16(Data, 0xA6); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xA6); }

        public override int Gender { get => Data[0xAF8]; set => Data[0xAF8] = (byte)value; }
        public override uint Money { get => BigEndian.ToUInt32(Data, 0xAFC); set => BigEndian.GetBytes(value).CopyTo(Data, 0xAFC); }
        public uint Coupons { get => BigEndian.ToUInt32(Data, 0xB00); set => BigEndian.GetBytes(value).CopyTo(Data, 0xB00); }
        public string RUI_Name { get => GetString(0xB3A, 20); set => SetString(value, 10).CopyTo(Data, 0xB3A); }

        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch3GC(InventoryType.Items, LegalItems, 999, OFS_PouchHeldItem, 20), // 20 COLO, 30 XD
                    new InventoryPouch3GC(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem, 43),
                    new InventoryPouch3GC(InventoryType.Balls, LegalBalls, 999, OFS_PouchBalls, 16),
                    new InventoryPouch3GC(InventoryType.TMHMs, LegalTMHMs, 999, OFS_PouchTMHM, 64),
                    new InventoryPouch3GC(InventoryType.Berries, LegalBerries, 999, OFS_PouchBerry, 46),
                    new InventoryPouch3GC(InventoryType.Medicine, LegalCologne, 999, OFS_PouchCologne, 3), // Cologne
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }

        // Daycare Structure:
        // 0x00 -- Occupied
        // 0x01 -- Deposited Level
        // 0x02-0x03 -- unused?
        // 0x04-0x07 -- Initial EXP
        public override int GetDaycareSlotOffset(int loc, int slot) { return Daycare + 8; }
        public override uint? GetDaycareEXP(int loc, int slot) { return null; }
        public override bool? IsDaycareOccupied(int loc, int slot) { return null; }
        public override void SetDaycareEXP(int loc, int slot, uint EXP) { }
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) { }

        public override string GetString(byte[] data, int offset, int length) => StringConverter3.GetBEString3(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter3.SetBEString3(value, maxLength, PadToSize, PadWith);
        }
    }
}
