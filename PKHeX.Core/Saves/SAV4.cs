using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class SAV4 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {PlayTimeString}].bak";
        public override string Filter => (Footer.Length > 0 ? "DeSmuME DSV|*.dsv|" : "") + "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";
        public SAV4(byte[] data = null, GameVersion versionOverride = GameVersion.Any)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G4RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Get Version
            if (data == null)
                Version = GameVersion.HGSS;
            else if (versionOverride != GameVersion.Any)
                Version = versionOverride;
            else Version = SaveUtil.getIsG4SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            getActiveGeneralBlock();
            getActiveStorageBlock();
            getSAVOffsets();

            switch (Version)
            {
                case GameVersion.DP: Personal = PersonalTable.DP; break;
                case GameVersion.Pt: Personal = PersonalTable.Pt; break;
                case GameVersion.HGSS: Personal = PersonalTable.HGSS; break;
            }

            if (!Exportable)
                resetBoxes();
        }

        // Configuration
        public override SaveFile Clone() { return new SAV4(Data, Version); }

        public override int SIZE_STORED => PKX.SIZE_4STORED;
        public override int SIZE_PARTY => PKX.SIZE_4PARTY;
        public override PKM BlankPKM => new PK4();
        public override Type PKMType => typeof(PK4);

        public override int BoxCount => 18;
        public override int MaxEV => 255;
        public override int Generation => 4;
        protected override int EventFlagMax => int.MinValue;
        protected override int EventConstMax => int.MinValue;
        protected override int GiftCountMax => 11;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override int MaxMoveID => Legal.MaxMoveID_4;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_4;
        public override int MaxItemID => Version == GameVersion.HGSS ? Legal.MaxItemID_4_HGSS : Version == GameVersion.Pt ? Legal.MaxItemID_4_Pt : Legal.MaxItemID_4_DP;
        public override int MaxAbilityID => Legal.MaxAbilityID_4;
        public override int MaxBallID => Legal.MaxBallID_4;
        public override int MaxGameID => 15; // Colo/XD

        // Checksums
        private static int[][] getCHKOffsets(GameVersion g)
        {
            // start, end, chkoffset
            switch (g)
            {
                case GameVersion.DP:
                    return new[] {new[] {0x0000, 0xC0EC, 0xC0FE}, new[] {0xc100, 0x1E2CC, 0x1E2DE}};
                case GameVersion.Pt:
                    return new[] {new[] {0x0000, 0xCF18, 0xCF2A}, new[] {0xCF2C, 0x1F0FC, 0x1F10E}};
                case GameVersion.HGSS:
                    return new[] {new[] {0x0000, 0xF618, 0xF626}, new[] {0xF700, 0x21A00, 0x21A0E}};
                    
                default:
                    return null;
            }
        }
        protected override void setChecksums()
        {
            int[][] c = getCHKOffsets(Version);
            if (c == null)
                return;

            BitConverter.GetBytes(SaveUtil.ccitt16(getData(c[0][0] + GBO, c[0][1] - c[0][0]))).CopyTo(Data, c[0][2] + GBO);
            BitConverter.GetBytes(SaveUtil.ccitt16(getData(c[1][0] + SBO, c[1][1] - c[1][0]))).CopyTo(Data, c[1][2] + SBO);
        }
        public override bool ChecksumsValid
        {
            get
            {
                int[][] c = getCHKOffsets(Version);
                if (c == null)
                    return false;

                if (SaveUtil.ccitt16(getData(c[0][0] + GBO, c[0][1] - c[0][0])) != BitConverter.ToUInt16(Data, c[0][2] + GBO))
                    return false; // Small Fail
                if (SaveUtil.ccitt16(getData(c[1][0] + SBO, c[1][1] - c[1][0])) != BitConverter.ToUInt16(Data, c[1][2] + SBO))
                    return false; // Large Fail

                return true;
            }
        }
        public override string ChecksumInfo
        {
            get
            {
                int[][] c = getCHKOffsets(Version);
                if (c == null)
                    return "Unable to check Save File.";

                string r = "";
                if (SaveUtil.ccitt16(getData(c[0][0] + GBO, c[0][1] - c[0][0])) != BitConverter.ToUInt16(Data, c[0][2] + GBO))
                    r += "Small block checksum is invalid" + Environment.NewLine;
                if (SaveUtil.ccitt16(getData(c[1][0] + SBO, c[1][1] - c[1][0])) != BitConverter.ToUInt16(Data, c[1][2] + SBO))
                    r += "Large block checksum is invalid" + Environment.NewLine;

                return r.Length == 0 ? "Checksums valid." : r.TrimEnd();
            }
        }

        // Blocks & Offsets
        private int generalBlock = -1; // Small Block
        private int storageBlock = -1; // Big Block
        private int hofBlock = -1; // Hall of Fame Block
        private int SBO => 0x40000 * storageBlock;
        private int GBO => 0x40000 * generalBlock;
        private int HBO => 0x40000 * hofBlock;
        public int getGBO => GBO;
        private void getActiveGeneralBlock()
        {
            if (Version < 0)
                return;
            int ofs = 0;

            // Check to see if the save is initialized completely
            // if the block is not initialized, fall back to the other save.
            if (getData(0x00000, 10).SequenceEqual(Enumerable.Repeat((byte)0xFF, 10)))
            { generalBlock = 1; return; }
            if (getData(0x40000, 10).SequenceEqual(Enumerable.Repeat((byte)0xFF, 10)))
            { generalBlock = 0; return; }

            // Check SaveCount for current save
            if (Version == GameVersion.DP) ofs = 0xC0F0; // DP
            else if (Version == GameVersion.Pt) ofs = 0xCF1C; // PT
            else if (Version == GameVersion.HGSS) ofs = 0xF618; // HGSS
            generalBlock = BitConverter.ToUInt16(Data, ofs) >= BitConverter.ToUInt16(Data, ofs + 0x40000) ? 0 : 1;
        }
        private void getActiveStorageBlock()
        {
            if (Version < 0)
                return;
            int ofs = 0;

            // Check SaveCount for current save
            if (Version == GameVersion.DP) ofs = 0x1E2D0; // DP
            else if (Version == GameVersion.Pt) ofs = 0x1F100; // PT
            else if (Version == GameVersion.HGSS) ofs = 0x21A00; // HGSS

            // Check to see if the save is initialized completely
            // if the block is not initialized, fall back to the other save.
            if (getData(ofs + 0x00000, 10).SequenceEqual(Enumerable.Repeat((byte)0xFF, 10)))
            { storageBlock = 1; return; }
            if (getData(ofs + 0x40000, 10).SequenceEqual(Enumerable.Repeat((byte)0xFF, 10)))
            { storageBlock = 0; return; }

            storageBlock = BitConverter.ToUInt16(Data, ofs) >= BitConverter.ToUInt16(Data, ofs + 0x40000) ? 0 : 1;
        }
        private void getSAVOffsets()
        {
            if (Version < 0)
                return;

            switch (Version)
            {
                case GameVersion.DP:
                    AdventureInfo = 0 + GBO;
                    Trainer1 = 0x64 + GBO;
                    Party = 0x98 + GBO;
                    PokeDex = 0x12DC + GBO;
                    WondercardFlags = 0xA6D0 + GBO;
                    WondercardData = 0xA7fC + GBO;

                    OFS_PouchHeldItem = 0x624 + GBO;
                    OFS_PouchKeyItem = 0x8B8 + GBO;
                    OFS_PouchTMHM = 0x980 + GBO;
                    OFS_PouchMedicine = 0xB40 + GBO;
                    OFS_PouchBerry = 0xBE0 + GBO;
                    OFS_PouchBalls = 0xCE0 + GBO;
                    OFS_BattleItems = 0xD1C + GBO;
                    OFS_MailItems = 0xD50 + GBO;
                    LegalItems = Legal.Pouch_Items_DP;
                    LegalKeyItems = Legal.Pouch_Key_DP;
                    LegalTMHMs = Legal.Pouch_TMHM_DP;
                    LegalMedicine = Legal.Pouch_Medicine_DP;
                    LegalBerries = Legal.Pouch_Berries_DP;
                    LegalBalls = Legal.Pouch_Ball_DP;
                    LegalBattleItems = Legal.Pouch_Battle_DP;
                    LegalMailItems = Legal.Pouch_Mail_DP;

                    HeldItems = Legal.HeldItems_DP;

                    Daycare = 0x141C + GBO;
                    Box = 0xC104 + SBO;
                    break;
                case GameVersion.Pt:
                    AdventureInfo = 0 + GBO;
                    Trainer1 = 0x68 + GBO;
                    Party = 0xA0 + GBO;
                    PokeDex = 0x1328 + GBO;
                    WondercardFlags = 0xB4C0 + GBO;
                    WondercardData = 0xB5C0 + GBO;

                    OFS_PouchHeldItem = 0x630 + GBO;
                    OFS_PouchKeyItem = 0x8C4 + GBO;
                    OFS_PouchTMHM = 0x98C + GBO;
                    OFS_MailItems = 0xB1C + GBO;
                    OFS_PouchMedicine = 0xB4C + GBO;
                    OFS_PouchBerry = 0xBEC + GBO;
                    OFS_PouchBalls = 0xCEC + GBO;
                    OFS_BattleItems = 0xD28 + GBO;
                    LegalItems = Legal.Pouch_Items_Pt;
                    LegalKeyItems = Legal.Pouch_Key_Pt;
                    LegalTMHMs = Legal.Pouch_TMHM_Pt;
                    LegalMedicine = Legal.Pouch_Medicine_Pt;
                    LegalBerries = Legal.Pouch_Berries_Pt;
                    LegalBalls = Legal.Pouch_Ball_Pt;
                    LegalBattleItems = Legal.Pouch_Battle_Pt;
                    LegalMailItems = Legal.Pouch_Mail_Pt;

                    HeldItems = Legal.HeldItems_Pt;

                    Daycare = 0x1654 + GBO;
                    Box = 0xCF30 + SBO;
                    break;
                case GameVersion.HGSS:
                    AdventureInfo = 0 + GBO;
                    Trainer1 = 0x64 + GBO;
                    Party = 0x98 + GBO;
                    PokeDex = 0x12B8 + GBO;
                    WondercardFlags = 0x9D3C + GBO;
                    WondercardData = 0x9E3C + GBO;

                    OFS_PouchHeldItem = 0x644 + GBO; // 0x644-0x8D7 (0x8CB)
                    OFS_PouchKeyItem = 0x8D8 + GBO; // 0x8D8-0x99F (0x979)
                    OFS_PouchTMHM = 0x9A0 + GBO; // 0x9A0-0xB33 (0xB2F)
                    OFS_MailItems = 0xB34 + GBO; // 0xB34-0xB63 (0xB63)
                    OFS_PouchMedicine = 0xB64 + GBO; // 0xB64-0xC03 (0xBFB)
                    OFS_PouchBerry = 0xC04 + GBO; // 0xC04-0xD03
                    OFS_PouchBalls = 0xD04 + GBO; // 0xD04-0xD63
                    OFS_BattleItems = 0xD64 + GBO; // 0xD64-0xD97
                    LegalItems = Legal.Pouch_Items_HGSS;
                    LegalKeyItems = Legal.Pouch_Key_HGSS;
                    LegalTMHMs = Legal.Pouch_TMHM_HGSS;
                    LegalMedicine = Legal.Pouch_Medicine_HGSS;
                    LegalBerries = Legal.Pouch_Berries_HGSS;
                    LegalBalls = Legal.Pouch_Ball_HGSS;
                    LegalBattleItems = Legal.Pouch_Battle_HGSS;
                    LegalMailItems = Legal.Pouch_Mail_HGSS;

                    HeldItems = Legal.HeldItems_HGSS;

                    Daycare = 0x15FC + GBO;
                    Box = 0xF700 + SBO;
                    break;
            }
        }

        private int WondercardFlags = int.MinValue;
        private int AdventureInfo = int.MinValue;

        // Inventory
        private ushort[] LegalItems, LegalKeyItems, LegalTMHMs, LegalMedicine, LegalBerries, LegalBalls, LegalBattleItems, LegalMailItems;
        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, LegalItems, 995, OFS_PouchHeldItem),
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem),
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 95, OFS_PouchTMHM),
                    new InventoryPouch(InventoryType.Medicine, LegalMedicine, 995, OFS_PouchMedicine),
                    new InventoryPouch(InventoryType.Berries, LegalBerries, 995, OFS_PouchBerry),
                    new InventoryPouch(InventoryType.Balls, LegalBalls, 995, OFS_PouchBalls),
                    new InventoryPouch(InventoryType.BattleItems, LegalBattleItems, 995, OFS_BattleItems),
                    new InventoryPouch(InventoryType.MailItems, LegalMailItems, 995, OFS_MailItems),
                };
                foreach (var p in pouch)
                    p.getPouch(ref Data);
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.setPouch(ref Data);
            }
        }

        // Storage
        public override int PartyCount
        {
            get { return Data[Party - 4]; }
            protected set { Data[Party - 4] = (byte)value; }
        }
        public override int getBoxOffset(int box)
        {
            return Box + SIZE_STORED*box*30 + (Version == GameVersion.HGSS ? box * 0x10 : 0);
        }
        public override int getPartyOffset(int slot)
        {
            return Party + SIZE_PARTY*slot;
        }

        // Trainer Info
        public override string OT
        {
            get { return getString(Trainer1, 16); }
            set { setString(value, OTLength).CopyTo(Data, Trainer1); }
        }
        public override ushort TID
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x10 + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x10 + 0); }
        }
        public override ushort SID
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x12); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x12); }
        }
        public override uint Money
        {
            get { return BitConverter.ToUInt32(Data, Trainer1 + 0x14); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x14); }
        }
        public override int Gender
        {
            get { return Data[Trainer1 + 0x18]; }
            set { Data[Trainer1 + 0x18] = (byte)value; }
        }
        public override int Language
        {
            get { return Data[Trainer1 + 0x19]; }
            set { Data[Trainer1 + 0x19] = (byte)value; }
        }
        public int Badges
        {
            get { return Data[Trainer1 + 0x1A]; }
            set { if (value < 0) return; Data[Trainer1 + 0x1A] = (byte)value; }
        }
        public int Sprite
        {
            get { return Data[Trainer1 + 0x1B]; }
            set { if (value < 0) return; Data[Trainer1 + 0x1B] = (byte)value; }
        }
        public int Badges16
        {
            get
            {
                if (Version != GameVersion.HGSS)
                    return -1;
                return Data[Trainer1 + 0x1F];
            }
            set
            {
                if (value < 0)
                    return;
                if (Version != GameVersion.HGSS)
                    return;
                Data[Trainer1 + 0x1F] = (byte)value;
            }
        }
        public override int PlayedHours
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x22); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x22); }
        }
        public override int PlayedMinutes
        {
            get { return Data[Trainer1 + 0x24]; }
            set { Data[Trainer1 + 0x24] = (byte)value; }
        }
        public override int PlayedSeconds
        {
            get { return Data[Trainer1 + 0x25]; }
            set { Data[Trainer1 + 0x25] = (byte)value; }
        }
        public int M
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x1238; break;
                    case GameVersion.HGSS: ofs = 0x1234; break;
                    case GameVersion.Pt: ofs = 0x1280; break;
                }
                ofs += GBO;
                return BitConverter.ToUInt16(Data, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x1238; break;
                    case GameVersion.HGSS: ofs = 0x1234; break;
                    case GameVersion.Pt: ofs = 0x1280; break;
                }
                ofs += GBO;
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
            }
        }
        public int X
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x25FA; break;
                    case GameVersion.Pt: ofs = 0x287E; break;
                    case GameVersion.HGSS: ofs = 0x236E; break;
                }
                ofs += GBO;
                return BitConverter.ToUInt16(Data, ofs);
            }
            set
            {
                int ofs = 0; int ofs2 = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x25FA; ofs2 = 0x1240; break;
                    case GameVersion.Pt: ofs = 0x287E; ofs2 = 0x1288; break;
                    case GameVersion.HGSS: ofs = 0x236E; ofs2 = 0x123C; break;
                }
                ofs += GBO; ofs2 += GBO;
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs2);
            }
        }
        public int Z
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x2602; break;
                    case GameVersion.Pt: ofs = 0x2886; break;
                    case GameVersion.HGSS: ofs = 0x2376; break;
                }
                ofs += GBO;
                return BitConverter.ToUInt16(Data, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x2602; break;
                    case GameVersion.Pt: ofs = 0x2886; break;
                    case GameVersion.HGSS: ofs = 0x2376; break;
                }
                ofs += GBO;
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
            }
        }
        public int Y
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x25FE; break;
                    case GameVersion.Pt: ofs = 0x2882; break;
                    case GameVersion.HGSS: ofs = 0x2372; break;
                }
                ofs += GBO;
                return BitConverter.ToUInt16(Data, ofs);
            }
            set
            {
                int ofs = 0; int ofs2 = 0;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x25FE; ofs2 = 0x1244; break;
                    case GameVersion.Pt: ofs = 0x2882; ofs2 = 0x128C; break;
                    case GameVersion.HGSS: ofs = 0x2372; ofs2 = 0x1240; break;
                }
                ofs += GBO; ofs2 += GBO;
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs2);
            }
        }
        public override int SecondsToStart { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x34); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x34); } }
        public override int SecondsToFame { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x3C); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x3C); } }

        // Storage
        public override int CurrentBox
        {
            get { return Data[Version == GameVersion.HGSS ? getBoxOffset(BoxCount) : Box - 4]; }
            set { Data[Version == GameVersion.HGSS ? getBoxOffset(BoxCount) : Box - 4] = (byte)value; }
        }
        protected override int getBoxWallpaperOffset(int box)
        {
            // Box Wallpaper is directly after the Box Names
            int offset = getBoxOffset(BoxCount);
            if (Version == GameVersion.HGSS) offset += 0x18;
            offset += BoxCount*0x28 + box;
            return offset;
        }
        public override string getBoxName(int box)
        {
            int offset = getBoxOffset(BoxCount);
            if (Version == GameVersion.HGSS) offset += 0x8;
            return getString(offset + box*0x28, 0x28);
        }
        public override void setBoxName(int box, string value)
        {
            if (value.Length > 13)
                value = value.Substring(0, 13); // Hard cap
            int offset = getBoxOffset(BoxCount);
            if (Version == GameVersion.HGSS) offset += 0x8;
            setData(setString(value, 13), offset + box*0x28);
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK4(data);
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray45(data);
        }

        // Daycare
        public override int getDaycareSlotOffset(int loc, int slot)
        {
            return Daycare + slot * SIZE_PARTY;
        }
        public override uint? getDaycareEXP(int loc, int slot)
        {
            int ofs = Daycare + (slot+1)*SIZE_PARTY - 4;
            return BitConverter.ToUInt32(Data, ofs);
        }
        public override bool? getDaycareOccupied(int loc, int slot)
        {
            return null;
        }
        public override void setDaycareEXP(int loc, int slot, uint EXP)
        {
            int ofs = Daycare + (slot+1)*SIZE_PARTY - 4;
            BitConverter.GetBytes(EXP).CopyTo(Data, ofs);
        }
        public override void setDaycareOccupied(int loc, int slot, bool occupied)
        {

        }

        // Mystery Gift
        public bool MysteryGiftActive { get { return (Data[GBO + 72] & 1) == 1; } set { Data[GBO + 72] = (byte)((Data[GBO + 72] & 0xFE) | (value ? 1 : 0)); } }
        private static bool getIsMysteryGiftAvailable(MysteryGift[] value)
        {
            if (value == null)
                return false;
            for (int i = 0; i < 8; i++) // 8 PGT
                if ((value[i] as PGT)?.CardType != 0)
                    return true;
            for (int i = 8; i < 11; i++) // 3 PCD
                if ((value[i] as PCD)?.Gift.CardType != 0)
                    return true;
            return false;
        }

        private static int[] matchMysteryGifts(MysteryGift[] value)
        {
            if (value == null)
                return null;

            int[] cardMatch = new int[8];
            for (int i = 0; i < 8; i++)
            {
                var pgt = value[i] as PGT;
                if (pgt == null)
                    continue;

                if (pgt.CardType == 0) // empty
                {
                    cardMatch[i] = pgt.Slot = 0;
                    continue;
                }

                cardMatch[i] = pgt.Slot = 3;
                for (byte j = 0; j < 3; j++)
                {
                    var pcd = value[8 + j] as PCD;
                    if (pcd == null)
                        continue;

                    // Check if data matches (except Slot @ 0x02)
                    if (!pcd.GiftEquals(pgt))
                        continue;

                    cardMatch[i] = pgt.Slot = j;
                    break;
                }
            }
            return cardMatch;
        }
        public override MysteryGiftAlbum GiftAlbum
        {
            get
            {
                var album = new MysteryGiftAlbum
                {
                    Flags = MysteryGiftReceivedFlags,
                    Gifts = MysteryGiftCards,
                };
                album.Flags[2047] = false;
                return album;
            }
            set
            {
                bool available = getIsMysteryGiftAvailable(value.Gifts);
                MysteryGiftActive |= available;
                value.Flags[2047] = available;

                MysteryGiftReceivedFlags = value.Flags;
                MysteryGiftCards = value.Gifts;

                if (Version != GameVersion.DP)
                    return;

                bool[] activeGifts = value.Gifts.Select(gift => !gift.Empty).ToArray();
                MysteryGiftDPSlotActiveFlags = activeGifts;
            }
        }
        protected override bool[] MysteryGiftReceivedFlags
        {
            get
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return null;

                bool[] r = new bool[GiftFlagMax];
                for (int i = 0; i < r.Length; i++)
                    r[i] = (Data[WondercardFlags + (i >> 3)] >> (i & 7) & 0x1) == 1;
                return r;
            }
            set
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return;
                if (GiftFlagMax != value?.Length)
                    return;

                byte[] data = new byte[value.Length / 8];
                for (int i = 0; i < value.Length; i++)
                    if (value[i])
                        data[i >> 3] |= (byte)(1 << (i & 7));

                data.CopyTo(Data, WondercardFlags);
                Edited = true;
            }
        }

        private const uint MysteryGiftDPSlotActive = 0xEDB88320;
        private bool[] MysteryGiftDPSlotActiveFlags
        {
            get
            {
                if (Version != GameVersion.DP)
                    return null;

                int ofs = WondercardFlags + 0x100; // skip over flags
                bool[] active = new bool[GiftCountMax]; // 8 PGT, 3 PCD
                for (int i = 0; i < active.Length; i++)
                    active[i] = BitConverter.ToUInt32(Data, ofs + 4*i) == MysteryGiftDPSlotActive;

                return active;
            }
            set
            {
                if (Version != GameVersion.DP)
                    return;
                if (value == null || value.Length != GiftCountMax)
                    return;

                int ofs = WondercardFlags + 0x100; // skip over flags
                for (int i = 0; i < value.Length; i++)
                {
                    byte[] magic = BitConverter.GetBytes(value[i] ? MysteryGiftDPSlotActive : 0); // 4 bytes
                    setData(magic, ofs + 4*i);
                }
            }
        }

        protected override MysteryGift[] MysteryGiftCards
        {
            get
            {
                MysteryGift[] cards = new MysteryGift[8 + 3];
                for (int i = 0; i < 8; i++) // 8 PGT
                    cards[i] = new PGT(getData(WondercardData + i * PGT.Size, PGT.Size));
                for (int i = 8; i < 11; i++) // 3 PCD
                    cards[i] = new PCD(getData(WondercardData + 8 * PGT.Size + (i-8) * PCD.Size, PCD.Size));
                return cards;
            }
            set
            {
                if (value == null)
                    return;

                var Matches = matchMysteryGifts(value); // automatically applied
                if (Matches == null)
                    return;

                for (int i = 0; i < 8; i++) // 8 PGT
                    if (value[i] is PGT)
                        setData(value[i].Data, WondercardData + i*PGT.Size);
                for (int i = 8; i < 11; i++) // 3 PCD
                    if (value[i] is PCD)
                        setData(value[i].Data, WondercardData + 8*PGT.Size + (i - 8)*PCD.Size);
            }
        }

        protected override void setDex(PKM pkm)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;
            if (PokeDex < 0)
                return;

            const int brSize = 0x40;
            int bit = pkm.Species - 1;
            byte mask = (byte)(1 << (bit & 7));
            int ofs = PokeDex + (bit >> 3) + 0x4;

            /* 4 BitRegions with 0x40*8 bits
            * Region 0: Caught (Captured/Owned) flags
            * Region 1: Seen flags
            * Region 2/3: Toggle for gender display
            * 4 possible states: 00, 01, 10, 11
            * 00 - 1Seen: Male Only
            * 01 - 2Seen: Male First, Female Second
            * 10 - 2Seen: Female First, Male Second
            * 11 - 1Seen: Female Only
            * (bit1 ^ bit2) + 1 = forms in dex
            * bit2 = male/female shown first toggle
            */

            // Set the Species Owned Flag
            Data[ofs + brSize * 0] |= mask;

            // Check if already Seen
            if ((Data[ofs + brSize * 1] & mask) == 0) // Not seen
            {
                int gr = pkm.PersonalInfo.Gender;
                switch (gr)
                {
                    case 255: // Genderless
                    case 0: // Male Only
                        Data[ofs + brSize * 1] &= mask;
                        Data[ofs + brSize * 2] &= mask;
                        break;
                    case 254: // Female Only
                        Data[ofs + brSize * 1] |= mask;
                        Data[ofs + brSize * 2] |= mask;
                        break;
                    default: // Male or Female
                        bool m = (Data[ofs + brSize * 1] & mask) != 0;
                        bool f = (Data[ofs + brSize * 2] & mask) != 0;
                        if (!(m || f)) // Add both forms (not a single form == 00 or 11).
                        {
                            int gender = pkm.Gender & 1;
                            gender ^= 1; // Set OTHER gender seen bit so it appears second
                            Data[ofs + brSize * (1 + gender)] |= mask;
                        }
                        break;
                }
            }

            int FormOffset1 = PokeDex + 4 + brSize * 4 + 4;
            var forms = getForms(pkm.Species);
            if (forms != null)
            {
                if (pkm.Species == 201) // Unown
                {
                    for (int i = 0; i < 0x1C; i++)
                    {
                        byte val = Data[FormOffset1 + 4 + i];
                        if (val == pkm.AltForm)
                            break; // already set
                        if (val != 0xFF)
                            continue; // keep searching

                        Data[FormOffset1 + 4 + i] = (byte)pkm.AltForm;
                        break; // form now set
                    }
                }
                else
                {
                    checkInsertForm(ref forms, pkm.AltForm);
                    setForms(pkm.Species, forms);
                }
            }

            int[] DPLangSpecies = { 23, 25, 54, 77, 120, 129, 202, 214, 215, 216, 228, 278, 287, 315 };
            int dpl = 1 + Array.IndexOf(DPLangSpecies, pkm.Species);
            if (DP && dpl <= 0)
                return;

            // Set the Language
            int lang = pkm.Language - 1;
            if (lang > 5) lang = 0; // no KOR
            if (lang < 0) lang = 1;
            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            Data[PokeDexLanguageFlags + (DP ? dpl : pkm.Species)] |= (byte)(1 << lang);
        }

        public override bool getCaught(int species)
        {
            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x4; // Magic
            return (1 << bm & Data[ofs + bd]) != 0;
        }
        public override bool getSeen(int species)
        {
            const int brSize = 0x40;

            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x4; // Magic

            return (1 << bm & Data[ofs + bd + brSize*1]) != 0;
        }

        public int[] getForms(int species)
        {
            const int brSize = 0x40;
            if (species == 386)
            {
                uint val = (uint) (Data[PokeDex + 0x4 + 1*brSize - 1] | Data[PokeDex + 0x4 + 2*brSize - 1] << 8);
                return getDexFormValues(val, 4, 4);
            }

            int FormOffset1 = PokeDex + 4 + 4*brSize + 4;
            switch (species)
            {
                case 422: // Shellos
                    return getDexFormValues(Data[FormOffset1 + 0], 1, 2);
                case 423: // Gastrodon
                    return getDexFormValues(Data[FormOffset1 + 1], 1, 2);
                case 412: // Burmy
                    return getDexFormValues(Data[FormOffset1 + 2], 2, 3);
                case 413: // Wormadam
                    return getDexFormValues(Data[FormOffset1 + 3], 2, 3);
                case 201: // Unown
                    return getData(FormOffset1 + 4, 0x1C).Select(i => (int)i).ToArray();
            }
            if (DP)
                return null;

            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
            switch (species)
            {
                case 479: // Rotom
                    return getDexFormValues(BitConverter.ToUInt32(Data, FormOffset2), 3, 6);
                case 492: // Shaymin
                    return getDexFormValues(Data[FormOffset2 + 4], 1, 2);
                case 487: // Giratina
                    return getDexFormValues(Data[FormOffset2 + 5], 1, 2);
                case 172:
                    if (!HGSS)
                        return null;
                    return getDexFormValues(Data[FormOffset2 + 6], 2, 3);
            }

            return null;
        }
        public void setForms(int spec, int[] forms)
        {
            const int brSize = 0x40;
            switch (spec)
            {
                case 386: // Deoxys
                    uint newval = setDexFormValues(forms, 4, 4);
                    Data[PokeDex + 0x4 + 1*brSize - 1] = (byte) (newval & 0xFF);
                    Data[PokeDex + 0x4 + 2*brSize - 1] = (byte) ((newval >> 8) & 0xFF);
                    break;
            }

            int FormOffset1 = PokeDex + 4 + 4*brSize + 4;
            switch (spec)
            {
                case 422: // Shellos
                    Data[FormOffset1 + 0] = (byte)setDexFormValues(forms, 1, 2);
                    return;
                case 423: // Gastrodon
                    Data[FormOffset1 + 1] = (byte)setDexFormValues(forms, 1, 2);
                    return;
                case 412: // Burmy
                    Data[FormOffset1 + 2] = (byte)setDexFormValues(forms, 2, 3);
                    return;
                case 413: // Wormadam
                    Data[FormOffset1 + 3] = (byte)setDexFormValues(forms, 2, 3);
                    return;
                case 201: // Unown
                    int ofs = FormOffset1 + 4;
                    int len = forms.Length;
                    Array.Resize(ref forms, 0x1C);
                    for (int i = len; i < forms.Length; i++)
                        forms[i] = 0xFF;
                    Array.Copy(forms.Select(b => (byte)b).ToArray(), 0, Data, ofs, forms.Length);
                    return;
            }

            if (DP)
                return;

            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
            switch (spec)
            {
                case 479: // Rotom
                    BitConverter.GetBytes(setDexFormValues(forms, 3, 6)).CopyTo(Data, FormOffset2);
                    return;
                case 492: // Shaymin
                    Data[FormOffset2 + 4] = (byte)setDexFormValues(forms, 1, 2);
                    return;
                case 487: // Giratina
                    Data[FormOffset2 + 5] = (byte)setDexFormValues(forms, 1, 2);
                    return;
                case 172: // Pichu
                    if (!HGSS)
                        return;
                    Data[FormOffset2 + 6] = (byte)setDexFormValues(forms, 2, 3);
                    return;
            }
        }

        private static int[] getDexFormValues(uint Value, int BitsPerForm, int readCt)
        {
            int[] Forms = new int[readCt];
            int n1 = 0xFF >> (8 - BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = (int)(Value >> (i * BitsPerForm)) & n1;
                if (n1 == val && BitsPerForm > 1)
                    Forms[i] = -1;
                else
                    Forms[i] = val;
            }
            return Forms;
        }
        private static uint setDexFormValues(int[] Forms, int BitsPerForm, int readCt)
        {
            int n1 = 0xFF >> (8 - BitsPerForm);
            uint Value = 0xFFFFFFFF << (Forms.Length*BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = Forms[i];
                if (val == -1)
                    val = n1;

                Value |= (uint)(val << (BitsPerForm*i));
            }
            return Value;
        }
        private static bool checkInsertForm(ref int[] Forms, int FormNum)
        {
            if (Forms.Any(num => num == FormNum))
            {
                return false; // already in list
            }
            if (Forms.All(num => num == -1))
            {
                Forms[0] = FormNum;
                return true; // none in list, insert at top
            }

            // insert at first empty
            int n1 = Array.IndexOf(Forms, -1);
            if (n1 < 0)
                return false;

            Forms[n1] = FormNum;
            return true;
        }
        public int DexUpgraded
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.DP:
                        if ((Data[0x1413 + GBO] & 1) != 0) return 4;
                        else if ((Data[0x1415 + GBO] & 1) != 0) return 3;
                        else if ((Data[0x1404 + GBO] & 1) != 0) return 2;
                        else if ((Data[0x1414 + GBO] & 1) != 0) return 1;
                        else return 0;
                    case GameVersion.HGSS:
                        if ((Data[0x15ED + GBO] & 1) != 0) return 3;
                        else if ((Data[0x15EF + GBO] & 1) != 0) return 2;
                        else if ((Data[0x15EE + GBO] & 1) != 0 && (Data[0x10D1 + GBO] & 8) != 0) return 1;
                        else return 0;
                    // case GameVersion.Pt: break;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.DP:
                        Data[0x1413 + GBO] = (byte)((Data[0x1413 + GBO] & 0xFE) | (value == 4 ? 1 : 0));
                        Data[0x1415 + GBO] = (byte)((Data[0x1415 + GBO] & 0xFE) | (value >= 3 ? 1 : 0));
                        Data[0x1404 + GBO] = (byte)((Data[0x1404 + GBO] & 0xFE) | (value >= 2 ? 1 : 0));
                        Data[0x1414 + GBO] = (byte)((Data[0x1414 + GBO] & 0xFE) | (value >= 1 ? 1 : 0));
                        break;
                    case GameVersion.HGSS:
                        Data[0x15ED + GBO] = (byte)((Data[0x15ED + GBO] & 0xFE) | (value == 3 ? 1 : 0));
                        Data[0x15EF + GBO] = (byte)((Data[0x15EF + GBO] & 0xFE) | (value >= 2 ? 1 : 0));
                        Data[0x15EE + GBO] = (byte)((Data[0x15EE + GBO] & 0xFE) | (value >= 1 ? 1 : 0));
                        Data[0x10D1 + GBO] = (byte)((Data[0x10D1 + GBO] & 0xF7) | (value >= 1 ? 8 : 0));
                        break;
                    // case GameVersion.Pt: break;
                    default: return;
                }
            }
        }

        // Honey Trees
        private const int HONEY_DP = 0x72E4;
        private const int HONEY_PT = 0x7F38;
        private const int HONEY_SIZE = 8;
        public HoneyTree getHoneyTree(int index)
        {
            if (index > 21)
                return null;
            switch (Version)
            {
                case GameVersion.DP:
                    return new HoneyTree(getData(HONEY_DP + HONEY_SIZE*index, HONEY_SIZE));
                case GameVersion.Pt:
                    return new HoneyTree(getData(HONEY_PT + HONEY_SIZE*index, HONEY_SIZE));
            }
            return null;
        }
        public void setHoneyTree(HoneyTree tree, int index)
        {
            if (index > 21)
                return;
            switch (Version)
            {
                case GameVersion.DP:
                    setData(tree.Data, HONEY_DP + HONEY_SIZE*index);
                    break;
                case GameVersion.Pt:
                    setData(tree.Data, HONEY_PT + HONEY_SIZE*index);
                    break;
            }
        }
        public int[] MunchlaxTrees
        {
            get
            {
                int A = (TID >> 8) % 21;
                int B = (TID & 0x00FF) % 21;
                int C = (SID >> 8) % 21;
                int D = (SID & 0x00FF) % 21;

                if (A == B) B = (B + 1) % 21;
                if (A == C) C = (C + 1) % 21;
                if (B == C) C = (C + 1) % 21;
                if (A == D) D = (D + 1) % 21;
                if (B == D) D = (D + 1) % 21;
                if (C == D) D = (D + 1) % 21;

                return new[] { A, B, C, D };
            }
        }
        public int PoketchApps
        {
            get
            {
                int ret = 0;
                int ofs;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x114F; break;
                    default: return ret;
                }
                ofs += GBO;
                for (int i = 0; i < 25; i++)
                {
                    if (Data[ofs + i] != 0) ret |= 1 << i;
                }
                return ret;
            }
            set
            {
                int c = 0;
                int ofs;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x114F; break;
                    default: return;
                }
                ofs += GBO;
                for (int i = 0; i < 25; i++)
                {
                    if ((value & 1 << i) != 0)
                    {
                        c++;
                        if (Data[ofs + i] == 0)
                            Data[ofs + i] = 1;
                    }
                    else Data[ofs + i] = 0;
                }
                Data[ofs - 2] = (byte)c;
                Data[ofs - 1] = 0; // current used, force set for first App.
            }
        }
        public byte[] PoketchDotArtist
        {
            get
            {
                byte[] ret = new byte[120]; // 2bit*24px*20px
                int ofs;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x1176; break;
                    default: return ret;
                }
                ofs += GBO;
                for (int i = 0; i < 120; i++)
                    ret[i] = Data[ofs + i];
                return ret;
            }
            set
            {
                int ofs;
                switch (Version)
                {
                    case GameVersion.DP: ofs = 0x1176; break;
                    default: return;
                }
                ofs += GBO;
                for (int i = 0; i < 120; i++)
                    Data[ofs + i] = value[i];
                Data[ofs - 0x2A] |= 0x04; // 0x114C "Touch!"
            }
        }
        
        public override string getString(int Offset, int Count) => PKX.getString4(Data, Offset, Count);
        public override byte[] setString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return PKX.setString4(value, maxLength, PadToSize, PadWith);
        }
    }
}
