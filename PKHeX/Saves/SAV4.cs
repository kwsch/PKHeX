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
        public override int OTLength => 8;
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
        public override bool HasPokeDex => false;

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
            get
            {
                return PKX.array2strG4(getData(Trainer1, 16))
                    .Replace("\uE08F", "\u2640") // Nidoran ♂
                    .Replace("\uE08E", "\u2642") // Nidoran ♀
                    .Replace("\u2019", "\u0027"); // Farfetch'd
            }
            set
            {
                if (value.Length > 7)
                    value = value.Substring(0, 7); // Hard cap
                string TempNick = value // Replace Special Characters and add Terminator
                .Replace("\u2640", "\uE08F") // Nidoran ♂
                .Replace("\u2642", "\uE08E") // Nidoran ♀
                .Replace("\u0027", "\u2019"); // Farfetch'd
                setData(PKX.str2arrayG4(TempNick), Trainer1);
            }
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
            return PKX.array2strG4(getData(offset + box*0x28, 0x28));
        }
        public override void setBoxName(int box, string value)
        {
            if (value.Length > 13)
                value = value.Substring(0, 13); // Hard cap
            int offset = getBoxOffset(BoxCount);
            if (Version == GameVersion.HGSS) offset += 0x8;
            setData(PKX.str2arrayG4(value), offset + box*0x28);
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
            byte mask = (byte) (1 << (bit&7));
            int ofs = PokeDex + (bit>>3) + 0x4;

            // Set the Species Owned Flag
            Data[ofs + brSize*0] |= mask;

            // Check if already Seen
            int gr = pkm.PersonalInfo.Gender;
            if (gr != 0 && gr < 254) // Not genderless or fixed gender
            {
                bool m = (Data[ofs + brSize*1] & mask) != 0;
                bool f = (Data[ofs + brSize*2] & mask) != 0;
                if (!(m || f)) // not yet seen
                {
                    // Set OTHER gender seen bit so it appears second
                    int gender = pkm.Gender & 1;
                    gender ^= 1; // other gender
                    Data[ofs + brSize*(1 + gender)] |= mask;
                }
            }

            int FormOffset1 = PokeDex + 0x108;
            int PokeDexLanguageFlags = FormOffset1 + 0x20;

            // Formes : Castform & Cherrim do not have entries (Battle Only formes)
            // Lowest sub-value of formevalue is displayed, else is order of formes displayed.

            // Deoxys forms 1-2 are stored in the last byte of the first bitRegion.
            // Deoxys forms 3-4 are stored in the last byte of the second bitRegion.
            if (pkm.Species == 386)
            {
                uint val = (uint)(Data[PokeDex + 0x4 + 1*brSize - 1] | Data[PokeDex + 0x4 + 2*brSize - 1] << 8);
                int[] forms = getDexFormValues(val, 4, 4);
                checkInsertForm(ref forms, pkm.AltForm);
                uint newval = setDexFormValues(forms, 4, 4);

                Data[PokeDex + 0x4 + 1*brSize - 1] = (byte)(newval & 0xFF);
                Data[PokeDex + 0x4 + 2*brSize - 1] = (byte)((newval>>8) & 0xFF);
            }

            // After the BitRegions is 0x20 bytes for the rest of the formes.
            // Standard Forme Bytes (DP)
            // [Shellos-Gastrodon-Burmy-Wormadam],[Unown*0x1C]
            if (pkm.Species == 422) // Shellos
            {
                int[] forms = getDexFormValues(Data[FormOffset1 + 0], 2, 2);
                checkInsertForm(ref forms, pkm.AltForm);
                uint newval = setDexFormValues(forms, 2, 2);
                Data[FormOffset1 + 0] = (byte)newval;
            }
            if (pkm.Species == 423) // Gastrodon
            {
                int[] forms = getDexFormValues(Data[FormOffset1 + 1], 2, 2);
                checkInsertForm(ref forms, pkm.AltForm);
                uint newval = setDexFormValues(forms, 2, 2);
                Data[FormOffset1 + 1] = (byte)newval;
            }
            if (pkm.Species == 412) // Burmy
            {
                int[] forms = getDexFormValues(Data[FormOffset1 + 2], 2, 3);
                checkInsertForm(ref forms, pkm.AltForm);
                uint newval = setDexFormValues(forms, 2, 3);
                Data[FormOffset1 + 2] = (byte)newval;
            }
            if (pkm.Species == 413) // Wormadam
            {
                int[] forms = getDexFormValues(Data[FormOffset1 + 3], 2, 3);
                checkInsertForm(ref forms, pkm.AltForm);
                uint newval = setDexFormValues(forms, 2, 3);
                Data[FormOffset1 + 3] = (byte)newval;
            }
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

            // DP stops here.
            if (DP)
                return;

            // Set the Language
            int lang = pkm.Language - 1;
            if (lang > 5) lang = 0; // no KOR
            if (lang < 0) lang = 1;
            Data[PokeDexLanguageFlags + pkm.Species] |= (byte) (1 << lang);

            int FormOffset2 = PokeDexLanguageFlags + 0x210;
            // PtHGSS added more forms.
            // [Rotom*4-highest bits unused],[Shaymin],[Giratina],[Pichu-HGSS ONLY]
            if (pkm.Species == 479) // Rotom
            {
                int[] forms = getDexFormValues(BitConverter.ToUInt32(Data, FormOffset2), 3, 6);
                checkInsertForm(ref forms, pkm.AltForm);
                uint newval = setDexFormValues(forms, 3, 6);
                BitConverter.GetBytes(newval).CopyTo(Data, FormOffset2);
            }
            if (pkm.Species == 492) // Shaymin
            {
                int[] forms = getDexFormValues(Data[FormOffset2 + 4], 2, 2);
                uint newval = setDexFormValues(forms, 2, 2);
                Data[FormOffset1 + 3] = (byte)newval;
            }
            if (pkm.Species == 487) // Giratina
            {
                int[] forms = getDexFormValues(Data[FormOffset2 + 5], 2, 2);
                uint newval = setDexFormValues(forms, 2, 2);
                Data[FormOffset1 + 3] = (byte)newval;
            }
            if (pkm.Species == 172 && HGSS) // Pichu
            {
                int[] forms = getDexFormValues(Data[FormOffset2 + 6], 2, 3);
                uint newval = setDexFormValues(forms, 2, 3);
                Data[FormOffset1 + 3] = (byte)newval;
            }
        }
        private int[] getDexFormValues(uint Value, int BitsPerForm, int readCt)
        {
            int[] Forms = new int[readCt];
            int n1 = 0xFF >> (8 - BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = (int)(Value >> (i*BitsPerForm))&n1;
                if (n1 == val)
                    Forms[i] = -1;
                else
                    Forms[i] = val;
            }
            return Forms;
        }
        private uint setDexFormValues(int[] Forms, int BitsPerForm, int readCt)
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
        private bool checkInsertForm(ref int[] Forms, int FormNum)
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
    }
}
