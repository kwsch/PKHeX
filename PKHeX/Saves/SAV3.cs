using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV3 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {PlayTimeString}].bak";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        /* SAV3 Structure:
         * 0xE000 per save file
         * 14 blocks @ 0x1000 each.
         * Blocks do not use all 0x1000 bytes allocated.
         * Via: http://bulbapedia.bulbagarden.net/wiki/Save_data_structure_in_Generation_III
         */
        private readonly int[] chunkLength =
        {
            0xf2c, // 0 | Trainer info
            0xf80, // 1 | Team / items
            0xf80, // 2 | Unknown
            0xf80, // 3 | Unknown
            0xf08, // 4 | Rival info
            0xf80, // 5 | PC Block 0
            0xf80, // 6 | PC Block 1
            0xf80, // 7 | PC Block 2
            0xf80, // 8 | PC Block 3
            0xf80, // 9 | PC Block 4
            0xf80, // A | PC Block 5
            0xf80, // B | PC Block 6
            0xf80, // C | PC Block 7
            0x7d0  // D | PC Block 8
        };

        public SAV3(byte[] data = null, GameVersion versionOverride = GameVersion.Any)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G3RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (data == null)
                Version = GameVersion.FRLG;
            else if (versionOverride != GameVersion.Any)
                Version = versionOverride;
            else Version = SaveUtil.getIsG3SAV(Data);
            if (Version == GameVersion.Invalid)
                return;
            
            int[] BlockOrder1 = new int[14];
            for (int i = 0; i < 14; i++)
                BlockOrder1[i] = BitConverter.ToInt16(Data, i*0x1000 + 0xFF4);
            int zeroBlock1 = Array.IndexOf(BlockOrder1, 0);

            if (data.Length > SaveUtil.SIZE_G3RAWHALF)
            {
                int[] BlockOrder2 = new int[14];
                for (int i = 0; i < 14; i++)
                    BlockOrder2[i] = BitConverter.ToInt16(Data, 0xE000 + i*0x1000 + 0xFF4);
                int zeroBlock2 = Array.IndexOf(BlockOrder2, 0);

                if (zeroBlock2 < 0)
                    ActiveSAV = 0;
                else if (zeroBlock1 < 0)
                    ActiveSAV = 1;
                else
                ActiveSAV = BitConverter.ToUInt32(Data, zeroBlock1*0x1000 + 0xFFC) >
                            BitConverter.ToUInt32(Data, zeroBlock2*0x1000 + 0xEFFC)
                    ? 0
                    : 1;
                BlockOrder = ActiveSAV == 0 ? BlockOrder1 : BlockOrder2;
            }
            else
            {
                ActiveSAV = 0;
                BlockOrder = BlockOrder1;
            }

            BlockOfs = new int[14];
            for (int i = 0; i < 14; i++)
                BlockOfs[i] = Array.IndexOf(BlockOrder, i)*0x1000 + ABO;

            // Set up PC data buffer beyond end of save file.
            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED); // More than enough empty space.

            // Copy chunk to the allocated location
            for (int i = 5; i < 14; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;
                Array.Copy(Data, blockIndex * 0x1000 + ABO, Data, Box + (i - 5)*0xF80, chunkLength[i]);
            }

            switch (Version)
            {
                case GameVersion.RS:
                    LegalKeyItems = Legal.Pouch_Key_RS;
                    OFS_PouchHeldItem = BlockOfs[1] + 0x0560;
                    OFS_PouchKeyItem = BlockOfs[1] + 0x05B0;
                    OFS_PouchBalls = BlockOfs[1] + 0x0600;
                    OFS_PouchTMHM = BlockOfs[1] + 0x0640;
                    OFS_PouchBerry = BlockOfs[1] + 0x0740;
                    Personal = PersonalTable.RS;
                    break;
                case GameVersion.FRLG:
                    LegalKeyItems = Legal.Pouch_Key_FRLG;
                    OFS_PouchHeldItem = BlockOfs[1] + 0x0310;
                    OFS_PouchKeyItem = BlockOfs[1] + 0x03B8;
                    OFS_PouchBalls = BlockOfs[1] + 0x0430;
                    OFS_PouchTMHM = BlockOfs[1] + 0x0464;
                    OFS_PouchBerry = BlockOfs[1] + 0x054C;
                    Personal = PersonalTable.FR;
                    break;
                case GameVersion.E:
                    LegalKeyItems = Legal.Pouch_Key_E;
                    OFS_PouchHeldItem = BlockOfs[1] + 0x0560;
                    OFS_PouchKeyItem = BlockOfs[1] + 0x05D8;
                    OFS_PouchBalls = BlockOfs[1] + 0x0650;
                    OFS_PouchTMHM = BlockOfs[1] + 0x0690;
                    OFS_PouchBerry = BlockOfs[1] + 0x0790;
                    Personal = PersonalTable.E;
                    break;
            }
            LegalItems = Legal.Pouch_Items_RS;
            LegalBalls = Legal.Pouch_Ball_RS;
            LegalTMHMs = Legal.Pouch_TMHM_RS;
            LegalBerries = Legal.Pouch_Berries_RS;

            HeldItems = Legal.HeldItems_RS;

            if (!Exportable)
                resetBoxes();
        }

        private const int SIZE_RESERVED = 0x10000; // unpacked box data
        public override byte[] Write(bool DSV)
        {
            // Copy Box data back
            for (int i = 5; i < 14; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;
                Array.Copy(Data, Box + (i - 5) * 0xF80, Data, blockIndex * 0x1000 + ABO, chunkLength[i]);
            }

            setChecksums();
            return Data.Take(Data.Length - SIZE_RESERVED).ToArray();
        }

        private readonly int ActiveSAV;
        private int ABO => ActiveSAV*0xE000;
        private readonly int[] BlockOrder;
        private readonly int[] BlockOfs;

        // Configuration
        public override SaveFile Clone() { return new SAV3(Write(DSV:false), Version); }
        public override bool IndeterminateGame => Version == GameVersion.Unknown;
        public override bool IndeterminateLanguage => true; // Unknown JP/International
        public override bool IndeterminateSubVersion => Version == GameVersion.FRLG;

        public override int SIZE_STORED => PKX.SIZE_3STORED;
        public override int SIZE_PARTY => PKX.SIZE_3PARTY;
        public override PKM BlankPKM => new PK3();
        public override Type PKMType => typeof(PK3);

        public override int MaxMoveID => 354;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => 77;
        public override int MaxItemID => 374;
        public override int MaxBallID => 0xC;
        public override int MaxGameID => 5;

        public override int BoxCount => 14;
        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 8;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override bool HasParty => true;

        // Checksums
        protected override void setChecksums()
        {
            for (int i = 0; i < 14; i++)
            {
                byte[] chunk = Data.Skip(ABO + i*0x1000).Take(chunkLength[BlockOrder[i]]).ToArray();
                ushort chk = SaveUtil.check32(chunk);
                BitConverter.GetBytes(chk).CopyTo(Data, ABO + i*0x1000 + 0xFF6);
            }
        }
        public override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < 14; i++)
                {
                    byte[] chunk = Data.Skip(ABO + i * 0x1000).Take(chunkLength[BlockOrder[i]]).ToArray();
                    ushort chk = SaveUtil.check32(chunk);
                    if (chk != BitConverter.ToUInt16(Data, ABO + i*0x1000 + 0xFF6))
                        return false;
                }
                return true;
            }
        }
        public override string ChecksumInfo
        {
            get
            {
                string r = "";
                for (int i = 0; i < 14; i++)
                {
                    byte[] chunk = Data.Skip(ABO + i * 0x1000).Take(chunkLength[BlockOrder[i]]).ToArray();
                    ushort chk = SaveUtil.check32(chunk);
                    ushort old = BitConverter.ToUInt16(Data, ABO + i*0x1000 + 0xFF6);
                    if (chk != old)
                        r += $"Block {BlockOrder[i].ToString("00")} @ {(i*0x1000).ToString("X5")} invalid." + Environment.NewLine;
                }
                return r.Length == 0 ? "Checksums valid." : r.TrimEnd();
            }
        }

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        private uint SecurityKey
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.E: return BitConverter.ToUInt32(Data, BlockOfs[0] + 0xAC);
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Data, BlockOfs[0] + 0xAF8);
                    default: return 0;
                }
            }
        }
        public override string OT
        {
            get
            {
                return PKX.getG3Str(Data.Skip(BlockOfs[0]).Take(0x10).ToArray(), Japanese)
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
                PKX.setG3Str(TempNick, Japanese).CopyTo(Data, BlockOfs[0]);
            }
        }
        public override int Gender
        {
            get { return Data[BlockOfs[0] + 8]; }
            set { Data[BlockOfs[0] + 8] = (byte)value; }
        }
        public override ushort TID
        {
            get { return BitConverter.ToUInt16(Data, BlockOfs[0] + 0xA + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, BlockOfs[0] + 0xA + 0); }
        }
        public override ushort SID
        {
            get { return BitConverter.ToUInt16(Data, BlockOfs[0] + 0xC); }
            set { BitConverter.GetBytes(value).CopyTo(Data, BlockOfs[0] + 0xC); }
        }
        public override int PlayedHours
        {
            get { return BitConverter.ToUInt16(Data, BlockOfs[0] + 0xE); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, BlockOfs[0] + 0xE); }
        }
        public override int PlayedMinutes
        {
            get { return Data[BlockOfs[0] + 0x10]; }
            set { Data[BlockOfs[0] + 0x10] = (byte)value; }
        }
        public override int PlayedSeconds
        {
            get { return Data[BlockOfs[0] + 0x11]; }
            set { Data[BlockOfs[0] + 0x11] = (byte)value; }
        }
        public int PlayedFrames
        {
            get { return Data[BlockOfs[0] + 0x12]; }
            set { Data[BlockOfs[0] + 0x12] = (byte)value; }
        }

        public override uint Money
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: return BitConverter.ToUInt32(Data, BlockOfs[1] + 0x0490) ^ SecurityKey;
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Data, BlockOfs[1] + 0x0290) ^ SecurityKey;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, BlockOfs[1] + 0x0490); break;
                    case GameVersion.FRLG: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, BlockOfs[1] + 0x0290); break;
                }
            }
        }
        public uint Coin
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: return BitConverter.ToUInt32(Data, BlockOfs[1] + 0x0494) ^ SecurityKey;
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Data, BlockOfs[1] + 0x0294) ^ SecurityKey;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, BlockOfs[1] + 0x0494); break;
                    case GameVersion.FRLG: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, BlockOfs[1] + 0x0294); break;
                }
            }
        }
        public int BP
        {
            get { return Data[BlockOfs[0] + 0xEB8]; }
            set { Data[BlockOfs[0] + 0xEB8] = (byte)value; }
        }

        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries;
        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, LegalItems, 95, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem)/4),
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem)/4),
                    new InventoryPouch(InventoryType.Balls, LegalBalls, 95, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls)/4),
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 95, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM)/4),
                    new InventoryPouch(InventoryType.Berries, LegalBerries, 95, OFS_PouchBerry, Version == GameVersion.FRLG ? 43 : 46),
                };
                foreach (var p in pouch)
                {
                    p.SecurityKey = SecurityKey;
                    p.getPouch(ref Data);
                }
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.setPouch(ref Data);
            }
        }

        public override int getDaycareSlotOffset(int loc, int slot)
        {
            return Daycare + slot * SIZE_PARTY;
        }
        public override uint? getDaycareEXP(int loc, int slot)
        {
            int ofs = Daycare + (slot + 1) * SIZE_PARTY - 4;
            return BitConverter.ToUInt32(Data, ofs);
        }
        public override bool? getDaycareOccupied(int loc, int slot)
        {
            return null;
        }
        public override void setDaycareEXP(int loc, int slot, uint EXP)
        {
            int ofs = Daycare + (slot + 1) * SIZE_PARTY - 4;
            BitConverter.GetBytes(EXP).CopyTo(Data, ofs);
        }
        public override void setDaycareOccupied(int loc, int slot, bool occupied)
        {

        }

        // Storage
        public override int PartyCount
        {
            get
            {
                int ofs = 0x34;
                if (GameVersion.FRLG != Version)
                    ofs += 0x200;
                return Data[BlockOfs[1] + ofs]; 
                
            }
            protected set
            {
                int ofs = 0x34;
                if (GameVersion.FRLG != Version)
                    ofs += 0x200;
                Data[BlockOfs[1] + ofs] = (byte)value; 
            }
        }
        public override int getBoxOffset(int box)
        {
            return Box + 4 + SIZE_STORED * box * 30;
        }
        public override int getPartyOffset(int slot)
        {
            int ofs = 0x38;
            if (GameVersion.FRLG != Version)
                ofs += 0x200;
            return BlockOfs[1] + ofs + SIZE_PARTY * slot;
        }
        public override int CurrentBox
        {
            get { return Data[Box]; }
            set { Data[Box] = (byte)value; }
        }
        protected override int getBoxWallpaperOffset(int box)
        {
            int offset = getBoxOffset(BoxCount);
            offset += BoxCount * 0x9 + box;
            return offset;
        }
        public override string getBoxName(int box)
        {
            int offset = getBoxOffset(BoxCount);
            return PKX.getG3Str(Data.Skip(offset + box * 9).Take(9).ToArray(), Japanese);
        }
        public override void setBoxName(int box, string value)
        {
            if (value.Length > 8)
                value = value.Substring(0, 8); // Hard cap
            int offset = getBoxOffset(BoxCount);
            PKX.setG3Str(value, Japanese).CopyTo(Data, offset + box * 9);
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK3(data);
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray3(data);
        }

        protected override void setDex(PKM pkm)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;
            if (BlockOfs.Any(z => z < 0))
                return;
            
            int bit = pkm.Species - 1;
            int ofs = bit/8;
            byte bitval = (byte)(1 << (bit%8));

            // Set the Captured Flag
            Data[BlockOfs[0] + 0x28 + ofs] |= bitval;

            // Set the Seen Flag
            Data[BlockOfs[0] + 0x5C + ofs] |= bitval;

            // Set the two other Seen flags (mirrored)
            switch (Version)
            {
                case GameVersion.RS:
                    Data[BlockOfs[1] + 0x938 + ofs] |= bitval;
                    Data[BlockOfs[4] + 0xC0C + ofs] |= bitval;
                    break;
                case GameVersion.E:
                    Data[BlockOfs[1] + 0x988 + ofs] |= bitval;
                    Data[BlockOfs[4] + 0xCA4 + ofs] |= bitval;
                    break;
                case GameVersion.FRLG:
                    Data[BlockOfs[1] + 0x5F8 + ofs] |= bitval;
                    Data[BlockOfs[4] + 0xB98 + ofs] |= bitval;
                    break;
            }
        }

        public bool NationalDex
        {
            get
            {
                if (BlockOfs.Any(z => z < 0))
                    return false;
                switch (Version) // only check natdex status in Block0
                {
                    case GameVersion.RS:
                    case GameVersion.E:
                        return BitConverter.ToUInt16(Data, BlockOfs[0] + 0x19) == 0xDA01;
                    case GameVersion.FRLG:
                        return Data[BlockOfs[0] + 0x1B] == 0xB9;
                }
                return false;
            }
            set
            {
                if (BlockOfs.Any(z => z < 0))
                    return;
                switch (Version)
                {
                    case GameVersion.RS:
                        BitConverter.GetBytes((ushort)(value ? 0xDA01 : 0)).CopyTo(Data, BlockOfs[0] + 0x19); // A
                        Data[BlockOfs[2] + 0x3A6] &= 0xBF;
                        Data[BlockOfs[2] + 0x3A6] |= (byte)(value ? 1 << 6 : 0); // B
                        BitConverter.GetBytes((ushort)(value ? 0x0302 : 0)).CopyTo(Data, BlockOfs[2] + 0x44C); // C
                        break;
                    case GameVersion.E:
                        BitConverter.GetBytes((ushort)(value ? 0xDA01 : 0)).CopyTo(Data, BlockOfs[0] + 0x19); // A
                        Data[BlockOfs[2] + 0x402] &= 0xBF; // Bit6
                        Data[BlockOfs[2] + 0x402] |= (byte)(value ? 1 << 6 : 0); // B
                        BitConverter.GetBytes((ushort)(value ? 0x6258 : 0)).CopyTo(Data, BlockOfs[2] + 0x4A8); // C
                        break;
                    case GameVersion.FRLG:
                        Data[BlockOfs[0] + 0x1B] = (byte)(value ? 0xB9 : 0); // A
                        Data[BlockOfs[2] + 0x68] &= 0xFE;
                        Data[BlockOfs[2] + 0x68] |= (byte)(value ? 1 : 0); // B
                        BitConverter.GetBytes((ushort)(value ? 0x6258 : 0)).CopyTo(Data, BlockOfs[2] + 0x11C); // C
                        break;
                }
            }
        }
    }
}
