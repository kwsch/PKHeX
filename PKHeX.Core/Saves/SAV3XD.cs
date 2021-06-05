using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object for Pokémon XD saves.
    /// </summary>
    public sealed class SAV3XD : SaveFile, IGCSaveFile
    {
        protected internal override string ShortSummary => $"{OT} ({Version}) #{SaveCount:0000}";
        public override string Extension => this.GCExtension();
        public bool IsMemoryCardSave => MC != null;
        private readonly SAV3GCMemoryCard? MC;

        private const int SLOT_SIZE = 0x28000;
        private const int SLOT_START = 0x6000;
        private const int SLOT_COUNT = 2;

        private int SaveCount = -1;
        private int SaveIndex = -1;
        private int Trainer1;
        private int Memo;
        private int Shadow;
        private readonly StrategyMemo StrategyMemo;
        private readonly ShadowInfoTableXD ShadowInfo;
        public int MaxShadowID => ShadowInfo.Count;
        private int OFS_PouchHeldItem, OFS_PouchKeyItem, OFS_PouchBalls, OFS_PouchTMHM, OFS_PouchBerry, OFS_PouchCologne, OFS_PouchDisc;
        private readonly int[] subOffsets = new int[16];
        public SAV3XD(byte[] data, SAV3GCMemoryCard MC) : this(data, MC.Data) { this.MC = MC; }
        public SAV3XD(byte[] data) : this(data, (byte[])data.Clone()) { }

        public SAV3XD() : base(SaveUtil.SIZE_G3XD)
        {
            // create fake objects
            StrategyMemo = new StrategyMemo();
            ShadowInfo = new ShadowInfoTableXD();
            Initialize();
            ClearBoxes();
        }

        private SAV3XD(byte[] data, byte[] bak) : base(data, bak)
        {
            InitializeData(out StrategyMemo, out ShadowInfo);
            Initialize();
        }

        public override PersonalTable Personal => PersonalTable.RS;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_XD;

        private void InitializeData(out StrategyMemo memo, out ShadowInfoTableXD info)
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

                ushort[] keys = new ushort[4];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = BigEndian.ToUInt16(slot, 8 + (i * 2));

                // Decrypt Slot
                Data = GeniusCrypto.Decrypt(slot, 0x00010, 0x27FD8, keys);
            }

            // Get Offset Info
            ushort[] subLength = new ushort[16];
            for (int i = 0; i < 16; i++)
            {
                subLength[i] = BigEndian.ToUInt16(Data, 0x20 + (2 * i));
                subOffsets[i] = BigEndian.ToUInt16(Data, 0x40 + (4 * i)) | BigEndian.ToUInt16(Data, 0x40 + (4 * i) + 2) << 16;
            }

            // Offsets are displaced by the 0xA8 savedata region
            Trainer1 = subOffsets[1] + 0xA8;
            Party = Trainer1 + 0x30;
            Box = subOffsets[2] + 0xA8;
            DaycareOffset = subOffsets[4] + 0xA8;
            Memo = subOffsets[5] + 0xA8;
            Shadow = subOffsets[7] + 0xA8;
            // Purifier = subOffsets[14] + 0xA8;

            memo = new StrategyMemo(Data, Memo, xd: true);
            info = new ShadowInfoTableXD(Data.Slice(Shadow, subLength[7]));
        }

        private void Initialize()
        {
            OFS_PouchHeldItem = Trainer1 + 0x4C8;
            OFS_PouchKeyItem = Trainer1 + 0x540;
            OFS_PouchBalls = Trainer1 + 0x5EC;
            OFS_PouchTMHM = Trainer1 + 0x62C;
            OFS_PouchBerry = Trainer1 + 0x72C;
            OFS_PouchCologne = Trainer1 + 0x7E4;
            OFS_PouchDisc = Trainer1 + 0x7F0;

            // Since PartyCount is not stored in the save file,
            // Count up how many party slots are active.
            for (int i = 0; i < 6; i++)
            {
                if (GetPartySlot(Data, GetPartyOffset(i)).Species != 0)
                    PartyCount++;
            }
        }

        protected override byte[] GetFinalData()
        {
            var newFile = GetInnerData();

            // Return the gci if Memory Card is not being exported
            if (!IsMemoryCardSave)
                return newFile;

            MC!.SelectedSaveData = newFile;
            return MC.Data;
        }

        private byte[] GetInnerData()
        {
            // Set Memo Back
            StrategyMemo.Write(); // .CopyTo(Data, Memo);
            ShadowInfo.Write().CopyTo(Data, Shadow);
            SetChecksums();

            // Get updated save slot data
            ushort[] keys = new ushort[4];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = BigEndian.ToUInt16(Data, 8 + (i * 2));
            byte[] newSAV = GeniusCrypto.Encrypt(Data, 0x10, 0x27FD8, keys);

            // Put save slot back in original save data
            byte[] newFile = MC != null ? MC.SelectedSaveData : (byte[]) State.BAK.Clone();
            Array.Copy(newSAV, 0, newFile, SLOT_START + (SaveIndex * SLOT_SIZE), newSAV.Length);
            return newFile;
        }

        // Configuration
        protected override SaveFile CloneInternal()
        {
            var data = GetInnerData();
            var sav = IsMemoryCardSave ? new SAV3XD(data, MC!) : new SAV3XD(data);
            return sav;
        }

        protected override int SIZE_STORED => PokeCrypto.SIZE_3XSTORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_3XSTORED; // unused
        public override PKM BlankPKM => new XK3();
        public override Type PKMType => typeof(XK3);

        public override int MaxMoveID => Legal.MaxMoveID_3;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => Legal.MaxAbilityID_3;
        public override int MaxBallID => Legal.MaxBallID_3;
        public override int MaxItemID => Legal.MaxItemID_3_XD;
        public override int MaxGameID => Legal.MaxGameID_3;

        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 9999999;

        public override int BoxCount => 8;

        public override bool IsPKMPresent(byte[] data, int offset) => PKX.IsPKMPresentGC(Data, offset);

        // Checksums
        protected override void SetChecksums()
        {
            Data = SetChecksums(Data, subOffsets[0]);
        }

        public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");

        public override string ChecksumInfo
        {
            get
            {
                byte[] data = SetChecksums(Data, subOffsets[0]);

                const int start = 0xA8; // 0x88 + 0x20
                int oldHC = BigEndian.ToInt32(Data, start + subOffsets[0] + 0x38);
                int newHC = BigEndian.ToInt32(data, start + subOffsets[0] + 0x38);
                bool header = newHC == oldHC;

                var oldCHK = Data.AsSpan(0x10, 0x10);
                var newCHK = data.AsSpan(0x10, 0x10);
                bool body = newCHK.SequenceEqual(oldCHK);
                return $"Header Checksum {(header ? "V" : "Inv")}alid, Body Checksum {(body ? "V" : "Inv")}alid.";
            }
        }

        private static byte[] SetChecksums(byte[] input, int subOffset0)
        {
            if (input.Length != SLOT_SIZE)
                throw new ArgumentException("Input should be a slot, not the entire save binary.");

            byte[] data = (byte[])input.Clone();
            const int start = 0xA8; // 0x88 + 0x20

            // Header Checksum
            int newHC = 0;
            for (int i = 0; i < 8; i++)
                newHC += data[i];

            BigEndian.GetBytes(newHC).CopyTo(data, start + subOffset0 + 0x38);

            // Body Checksum
            data.AsSpan(0x10, 0x10).Fill(0); // Clear old Checksum Data
            uint[] checksum = new uint[4];
            int dt = 8;
            for (int i = 0; i < checksum.Length; i++)
            {
                uint val = 0;
                var end = dt + 0x9FF4;
                for (int j = dt; j < end; j += 2)
                    val += BigEndian.ToUInt16(data, j);
                dt = end;
                checksum[i] = val;
            }

            ushort[] newchks = new ushort[8];
            for (int i = 0; i < 4; i++)
            {
                newchks[i*2] = (ushort)(checksum[i] >> 16);
                newchks[(i * 2) + 1] = (ushort)checksum[i];
            }

            Array.Reverse(newchks);
            for (int i = 0; i < newchks.Length; i++)
                BigEndian.GetBytes(newchks[i]).CopyTo(data, 0x10 + (2 * i));

            return data;
        }
        // Trainer Info
        public override GameVersion Version { get => GameVersion.XD; protected set { } }
        public override string OT { get => GetString(Trainer1 + 0x00, 20); set => SetString(value, 10).CopyTo(Data, Trainer1 + 0x00); }
        public override int SID { get => BigEndian.ToUInt16(Data, Trainer1 + 0x2C); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x2C); }
        public override int TID { get => BigEndian.ToUInt16(Data, Trainer1 + 0x2E); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x2E); }

        public override int Gender { get => Data[Trainer1 + 0x8E0]; set => Data[Trainer1 + 0x8E0] = (byte)value; }
        public override uint Money { get => BigEndian.ToUInt32(Data, Trainer1 + 0x8E4); set => BigEndian.GetBytes(value).CopyTo(Data, Trainer1 + 0x8E4); }
        public uint Coupons { get => BigEndian.ToUInt32(Data, Trainer1 + 0x8E8); set => BigEndian.GetBytes(value).CopyTo(Data, Trainer1 + 0x8E8); }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_STORED * slot);
        private int GetBoxInfoOffset(int box) => Box + (((30 * SIZE_STORED) + 0x14) * box);
        public override int GetBoxOffset(int box) => GetBoxInfoOffset(box) + 20;
        public override string GetBoxName(int box) => GetString(GetBoxInfoOffset(box), 16);

        public override void SetBoxName(int box, string value)
        {
            if (value.Length > 8)
                value = value[..8]; // Hard cap
            SetString(value, 8).CopyTo(Data, GetBoxInfoOffset(box));
        }

        protected override PKM GetPKM(byte[] data)
        {
            if (data.Length != SIZE_STORED)
                Array.Resize(ref data, SIZE_STORED);
            return new XK3(data);
        }

        protected override byte[] DecryptPKM(byte[] data) => data;
        public override PKM GetPartySlot(byte[] data, int offset) => GetStoredSlot(data, offset);

        public override PKM GetStoredSlot(byte[] data, int offset)
        {
            // Get Shadow Data
            var pk = (XK3)base.GetStoredSlot(data, offset);
            if (pk.ShadowID > 0 && pk.ShadowID < ShadowInfo.Count)
                pk.Purification = ShadowInfo[pk.ShadowID].Purification;
            return pk;
        }

        protected override void SetPKM(PKM pkm, bool isParty = false)
        {
            if (pkm is not XK3 pk)
                return; // shouldn't ever hit

            if (pk.CurrentRegion == 0)
                pk.CurrentRegion = 2; // NTSC-U
            if (pk.OriginalRegion == 0)
                pk.OriginalRegion = 2; // NTSC-U

            // Set Shadow Data back to save
            if (pk.ShadowID <= 0 || pk.ShadowID >= ShadowInfo.Count)
                return;

            var entry = ShadowInfo[pk.ShadowID];
            entry.Purification = pk.Purification;
            entry.Species = pk.Species;
            entry.PID = pk.PID;
            entry.IV_HP  = pk.IV_HP ;
            entry.IV_ATK = pk.IV_ATK;
            entry.IV_DEF = pk.IV_DEF;
            entry.IV_SPA = pk.IV_SPA;
            entry.IV_SPD = pk.IV_SPD;
            entry.IV_SPE = pk.IV_SPE;
        }

        protected override void SetDex(PKM pkm)
        {
            /*
            if (pkm.Species is 0 or > Legal.MaxSpeciesID_3)
                return;
            if (pkm.IsEgg)
                return;

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
            */
        }

        public override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch3GC(InventoryType.Items, Legal.Pouch_Items_XD, 999, OFS_PouchHeldItem, 30), // 20 COLO, 30 XD
                    new InventoryPouch3GC(InventoryType.KeyItems, Legal.Pouch_Key_XD, 1, OFS_PouchKeyItem, 43),
                    new InventoryPouch3GC(InventoryType.Balls, Legal.Pouch_Ball_RS, 999, OFS_PouchBalls, 16),
                    new InventoryPouch3GC(InventoryType.TMHMs, Legal.Pouch_TM_RS, 999, OFS_PouchTMHM, 64),
                    new InventoryPouch3GC(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 46),
                    new InventoryPouch3GC(InventoryType.Medicine, Legal.Pouch_Cologne_XD, 999, OFS_PouchCologne, 3), // Cologne
                    new InventoryPouch3GC(InventoryType.BattleItems, Legal.Pouch_Disc_XD, 1, OFS_PouchDisc, 60)
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
        public override int GetDaycareSlotOffset(int loc, int slot) { return DaycareOffset + 8; }
        public override uint? GetDaycareEXP(int loc, int slot) { return null; }
        public override bool? IsDaycareOccupied(int loc, int slot) { return null; }
        public override void SetDaycareEXP(int loc, int slot, uint EXP) { /* todo */ }
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) { /* todo */ }

        public override string GetString(byte[] data, int offset, int length) => StringConverter3.GetBEString3(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter3.SetBEString3(value, maxLength, PadToSize, PadWith);
        }
}
}
