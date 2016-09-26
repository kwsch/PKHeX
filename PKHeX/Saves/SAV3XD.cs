using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV3XD : SaveFile
    {
        public override string BAKName => $"{FileName} [{Version} #{SaveCount.ToString("0000")}].bak";
        public override string Filter => "GameCube Save File|*.gci";
        public override string Extension => ".gci";

        private const int SLOT_SIZE = 0x1E000;
        private const int SLOT_START = 0x6000;
        private const int SLOT_COUNT = 3;

        private readonly int SaveCount = -1;
        private readonly int SaveIndex = -1;
        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries, LegalCologne, LegalDisc;
        private readonly int OFS_PouchCologne, OFS_PouchDisc;
        public SAV3XD(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G3BOX] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (SaveUtil.getIsG3CXDSAV(Data) != GameVersion.XD)
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
                
            }

            OFS_PouchHeldItem = 1224;
            OFS_PouchKeyItem = 1344;
            OFS_PouchBalls = 1516;
            OFS_PouchTMHM = 1580;
            OFS_PouchBerry = 1836;
            OFS_PouchCologne = 2020;
            OFS_PouchDisc = 2032;

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
        }

        private readonly byte[] OriginalData;
        public override byte[] Write(bool DSV)
        {
            setChecksums();

            // Get updated save slot data
            byte[] newSAV = null;

            // Put save slot back in original save data
            byte[] newFile = (byte[])OriginalData.Clone();
            Array.Copy(newSAV, 0, newFile, SLOT_START + SaveIndex * SLOT_SIZE, newSAV.Length);
            return newFile;
        }

        // Configuration
        public override SaveFile Clone() { return new SAV3(Write(DSV: false), Version); }

        public override int SIZE_STORED => PKX.SIZE_3XSTORED;
        public override int SIZE_PARTY => PKX.SIZE_3XSTORED; // unused
        public override PKM BlankPKM => new XK3();
        public override Type PKMType => typeof(XK3);

        public override int MaxMoveID => 354;
        public override int MaxSpeciesID => 386;
        public override int MaxAbilityID => 77;
        public override int MaxItemID => 374;
        public override int MaxBallID => 0xC;
        public override int MaxGameID => 5;
        
        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 8;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override int BoxCount => 8;
        public override bool HasParty => false;

        // Checksums
        protected override void setChecksums()
        {
            
        }
        public override bool ChecksumsValid
        {
            get { return false; }
        }
        public override string ChecksumInfo
        {
            get { return ""; }
        }

        // Trainer Info
        public override GameVersion Version { get { return GameVersion.RSBOX; } protected set { } }

        // Storage
        public override int getPartyOffset(int slot)
        {
            return Party + SIZE_STORED * slot;
        }
        public override int getBoxOffset(int box)
        {
            return Box + (30 * SIZE_STORED + 0x14)*box + 0x14;
        }
        public override int CurrentBox { get { return 0; } set { } }
        public override int getBoxWallpaper(int box)
        {
            return box;
        }
        public override string getBoxName(int box)
        {
            return PKX.getColoStr(Data, Box + 0x24A4*box, 8);
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

        protected override void setDex(PKM pkm) { }
        
        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, LegalItems, 995, OFS_PouchHeldItem, 30), // 20 COLO, 30 XD
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem, 43),
                    new InventoryPouch(InventoryType.TMHMs, LegalBalls, 995, OFS_PouchTMHM, 16),
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 995, OFS_PouchTMHM, 64),
                    new InventoryPouch(InventoryType.Berries, LegalBerries, 995, OFS_PouchBerry, 46),
                    new InventoryPouch(InventoryType.Medicine, LegalCologne, 995, OFS_PouchCologne, 3), // Cologne
                    new InventoryPouch(InventoryType.Medicine, LegalDisc, 995, OFS_PouchDisc, 60), // Cologne
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
    }
}
