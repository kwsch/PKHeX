using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV3XD : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version}) #{SaveCount.ToString("0000")}].bak";
        public override string Filter => "GameCube Save File|*.gci|All Files|*.*";
        public override string Extension => ".gci";

        private const int SLOT_SIZE = 0x28000;
        private const int SLOT_START = 0x6000;
        private const int SLOT_COUNT = 2;

        private readonly int SaveCount = -1;
        private readonly int SaveIndex = -1;
        private readonly int Memo, Shadow;
        private readonly StrategyMemo StrategyMemo;
        private readonly ShadowInfoTableXD ShadowInfo;
        public override int MaxShadowID => ShadowInfo.Count;
        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries, LegalCologne, LegalDisc;
        private readonly int OFS_PouchCologne, OFS_PouchDisc;
        private readonly int[] subOffsets = new int[16];
        public SAV3XD(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G3XD] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (SaveUtil.getIsG3XDSAV(Data) != GameVersion.XD)
                return;

            OriginalData = (byte[])Data.Clone();

            // Scan all 3 save slots for the highest counter
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                int slotOffset = SLOT_START + i * SLOT_SIZE;
                int SaveCounter = BigEndian.ToInt32(Data, slotOffset + 4);
                if (SaveCounter <= SaveCount)
                    continue;

                SaveCount = SaveCounter;
                SaveIndex = i;
            }

            // Decrypt most recent save slot
            {
                byte[] slot = new byte[SLOT_SIZE];
                int slotOffset = SLOT_START + SaveIndex * SLOT_SIZE;
                Array.Copy(Data, slotOffset, slot, 0, slot.Length);

                ushort[] keys = new ushort[4];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = BigEndian.ToUInt16(slot, 8 + i * 2);

                // Decrypt Slot
                Data = SaveUtil.DecryptGC(slot, 0x00010, 0x27FD8, keys);
            }

            // Get Offset Info
            ushort[] subLength = new ushort[16];
            for (int i = 0; i < 16; i++)
            {
                subLength[i] = BigEndian.ToUInt16(Data, 0x20 + 2*i);
                subOffsets[i] = BigEndian.ToUInt16(Data, 0x40 + 4*i) | BigEndian.ToUInt16(Data, 0x40 + 4*i + 2) << 16;
            }
            // Offsets are displaced by the 0xA8 savedata region
            Trainer1 = subOffsets[1] + 0xA8;
            Party = Trainer1 + 0x30;
            Box = subOffsets[2] + 0xA8;
            Daycare = subOffsets[4] + 0xA8;
            Memo = subOffsets[5] + 0xA8;
            Shadow = subOffsets[7] + 0xA8;
            // Purifier = subOffsets[14] + 0xA8;

            StrategyMemo = new StrategyMemo(Data, Memo, xd: true);
            ShadowInfo = new ShadowInfoTableXD(Data.Skip(Shadow).Take(subLength[7]).ToArray());

            OFS_PouchHeldItem = Trainer1 + 0x4C8;
            OFS_PouchKeyItem = Trainer1 + 0x540;
            OFS_PouchBalls = Trainer1 + 0x5EC;
            OFS_PouchTMHM = Trainer1 + 0x62C;
            OFS_PouchBerry = Trainer1 + 0x72C;
            OFS_PouchCologne = Trainer1 + 0x7E4;
            OFS_PouchDisc = Trainer1 + 0x7F0;

            LegalItems = Legal.Pouch_Items_XD;
            LegalKeyItems = Legal.Pouch_Key_XD;
            LegalBalls = Legal.Pouch_Ball_RS;
            LegalTMHMs = Legal.Pouch_TM_RS; // not HMs
            LegalBerries = Legal.Pouch_Berries_RS;
            LegalCologne = Legal.Pouch_Cologne_CXD;
            LegalDisc = Legal.Pouch_Disc_XD;

            Personal = PersonalTable.RS;
            HeldItems = Legal.HeldItems_XD;

            if (!Exportable)
                resetBoxes();

            // Since PartyCount is not stored in the save file,
            // Count up how many party slots are active.
            for (int i = 0; i < 6; i++)
                if (getPartySlot(getPartyOffset(i)).Species != 0)
                    PartyCount++;
        }

        private readonly byte[] OriginalData;
        public override byte[] Write(bool DSV)
        {
            // Set Memo Back
            StrategyMemo.FinalData.CopyTo(Data, Memo);
            ShadowInfo.FinalData.CopyTo(Data, Shadow);
            setChecksums();

            // Get updated save slot data
            ushort[] keys = new ushort[4];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = BigEndian.ToUInt16(Data, 8 + i * 2);
            byte[] newSAV = SaveUtil.EncryptGC(Data, 0x10, 0x27FD8, keys);

            // Put save slot back in original save data
            byte[] newFile = (byte[])OriginalData.Clone();
            Array.Copy(newSAV, 0, newFile, SLOT_START + SaveIndex * SLOT_SIZE, newSAV.Length);
            return Header.Concat(newFile).ToArray();
        }

        // Configuration
        public override SaveFile Clone() { return new SAV3XD(Write(DSV: false)); }

        public override int SIZE_STORED => PKX.SIZE_3XSTORED;
        public override int SIZE_PARTY => PKX.SIZE_3XSTORED; // unused
        public override PKM BlankPKM => new XK3();
        public override Type PKMType => typeof(XK3);

        public override int MaxMoveID => 354;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => 77;
        public override int MaxItemID => 593;
        public override int MaxBallID => 0xC;
        public override int MaxGameID => 5;
        
        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 8;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override int BoxCount => 8;

        // Checksums
        protected override void setChecksums()
        {
            Data = setXDChecksums(Data, subOffsets[0]);
        }
        public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");
        public override string ChecksumInfo
        {
            get
            {
                byte[] data = setXDChecksums(Data, subOffsets[0]);

                const int start = 0xA8; // 0x88 + 0x20
                int oldHC = BigEndian.ToInt32(Data, start + subOffsets[0] + 0x38);
                int newHC = BigEndian.ToInt32(data, start + subOffsets[0] + 0x38);
                bool header = newHC == oldHC;

                var oldCHK = Data.Skip(0x10).Take(0x10);
                var newCHK = data.Skip(0x10).Take(0x10);
                bool body = newCHK.SequenceEqual(oldCHK);
                return $"Header Checksum {(header ? "V" : "Inv")}alid, Body Checksum {(body ? "V" : "Inv")}alid.";
            }
        }
        private static byte[] setXDChecksums(byte[] input, int subOffset0)
        {
            if (input.Length != 0x28000)
                throw new ArgumentException("Input should be a slot, not the entire save binary.");

            byte[] data = (byte[])input.Clone();
            const int start = 0xA8; // 0x88 + 0x20

            // Header Checksum
            int newHC = 0;
            for (int i = 0; i < 8; i++)
                newHC += data[i];

            BigEndian.GetBytes(newHC).CopyTo(data, start + subOffset0 + 0x38);

            // Body Checksum
            new byte[16].CopyTo(data, 0x10); // Clear old Checksum Data
            uint[] checksum = new uint[4];
            int dt = 8;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 0x9FF4; j += 2, dt += 2)
                    checksum[i] += BigEndian.ToUInt16(data, dt);

            ushort[] newchks = new ushort[8];
            for (int i = 0; i < 4; i++)
            {
                newchks[i*2] = (ushort)(checksum[i] >> 16);
                newchks[i*2+1] = (ushort)checksum[i];
            }

            Array.Reverse(newchks);
            for (int i = 0; i < newchks.Length; i++)
                BigEndian.GetBytes(newchks[i]).CopyTo(data, 0x10 + 2*i);

            return data;
        }
        // Trainer Info
        public override GameVersion Version { get { return GameVersion.XD; } protected set { } }
        public override string OT { get { return PKX.getColoStr(Data, Trainer1 + 0x00, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, Trainer1 + 0x00);  } }
        public override ushort SID { get { return BigEndian.ToUInt16(Data, Trainer1 + 0x2C); } set { BigEndian.GetBytes(value).CopyTo(Data, Trainer1 + 0x2C); } }
        public override ushort TID { get { return BigEndian.ToUInt16(Data, Trainer1 + 0x2E); } set { BigEndian.GetBytes(value).CopyTo(Data, Trainer1 + 0x2E); } }

        public override int Gender { get { return Data[Trainer1 + 0x8E0]; } set { Data[Trainer1 + 0x8E0] = (byte)value; } }
        public override uint Money { get { return BigEndian.ToUInt32(Data, Trainer1 + 0x8E4); } set { BigEndian.GetBytes(value).CopyTo(Data, Trainer1 + 0x8E4); } }
        public uint Coupons { get { return BigEndian.ToUInt32(Data, Trainer1 + 0x8E8); } set { BigEndian.GetBytes(value).CopyTo(Data, Trainer1 + 0x8E8); } }

        // Storage
        public override int getPartyOffset(int slot)
        {
            return Party + SIZE_STORED * slot;
        }
        public override int getBoxOffset(int box)
        {
            return Box + (30 * SIZE_STORED + 0x14)*box + 0x14;
        }
        public override string getBoxName(int box)
        {
            return PKX.getColoStr(Data, Box + (30 * SIZE_STORED + 0x14)*box, 8);
        }
        public override void setBoxName(int box, string value)
        {
            if (value.Length > 8)
                value = value.Substring(0, 8); // Hard cap
            PKX.setColoStr(value, 8).CopyTo(Data, Box + 0x24A4*box);
        }
        public override PKM getPKM(byte[] data)
        {
            return new XK3(data.Take(SIZE_STORED).ToArray());
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return data;
        }

        public override PKM getPartySlot(int offset)
        {
            return getStoredSlot(offset);
        }
        public override PKM getStoredSlot(int offset)
        {
            // Get Shadow Data
            var pk = getPKM(decryptPKM(getData(offset, SIZE_STORED))) as XK3;
            if (pk?.ShadowID > 0 && pk.ShadowID < ShadowInfo.Count)
                pk.Purification = ShadowInfo[pk.ShadowID - 1].Purification;
            return pk;
        }
        protected override void setPKM(PKM pkm)
        {
            XK3 pk = pkm as XK3;
            if (pk == null)
                return; // shouldn't ever hit
            
            // Set Shadow Data back to save
            if (pk.ShadowID <= 0 || pk.ShadowID >= ShadowInfo.Count)
                return;

            var entry = ShadowInfo[pk.ShadowID - 1];
            entry.Purification = pk.Purification;
            entry.Species = pk.Species;
            entry.PID = pk.PID;
            entry.IsPurified = pk.Purification == 0;
        }

        protected override void setDex(PKM pkm)
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
        
        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, LegalItems, 995, OFS_PouchHeldItem, 30), // 20 COLO, 30 XD
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem, 43),
                    new InventoryPouch(InventoryType.Balls, LegalBalls, 995, OFS_PouchBalls, 16),
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 995, OFS_PouchTMHM, 64),
                    new InventoryPouch(InventoryType.Berries, LegalBerries, 995, OFS_PouchBerry, 46),
                    new InventoryPouch(InventoryType.Medicine, LegalCologne, 995, OFS_PouchCologne, 3), // Cologne
                    new InventoryPouch(InventoryType.BattleItems, LegalDisc, 995, OFS_PouchDisc, 60)
                };
                foreach (var p in pouch)
                    p.getPouchBigEndian(ref Data);
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.setPouchBigEndian(ref Data);
            }
        }

        // Daycare Structure:
        // 0x00 -- Occupied
        // 0x01 -- Deposited Level
        // 0x02-0x03 -- unused?
        // 0x04-0x07 -- Initial EXP
        public override int getDaycareSlotOffset(int loc, int slot) { return Daycare + 8; }
        public override uint? getDaycareEXP(int loc, int slot) { return null; }
        public override bool? getDaycareOccupied(int loc, int slot) { return null; }
        public override void setDaycareEXP(int loc, int slot, uint EXP) { }
        public override void setDaycareOccupied(int loc, int slot, bool occupied) { }
    }
}
