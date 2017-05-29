using System;
using System.Linq;

namespace PKHeX.Core
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
        private const int SIZE_BLOCK = 0x1000;
        private const int BLOCK_COUNT = 14;
        private const int SIZE_RESERVED = 0x10000; // unpacked box data will start after the save data
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
            
            int[] BlockOrder1 = new int[BLOCK_COUNT];
            for (int i = 0; i < BLOCK_COUNT; i++)
                BlockOrder1[i] = BitConverter.ToInt16(Data, i*SIZE_BLOCK + 0xFF4);
            int zeroBlock1 = Array.IndexOf(BlockOrder1, 0);

            if (Data.Length > SaveUtil.SIZE_G3RAWHALF)
            {
                int[] BlockOrder2 = new int[BLOCK_COUNT];
                for (int i = 0; i < BLOCK_COUNT; i++)
                    BlockOrder2[i] = BitConverter.ToInt16(Data, 0xE000 + i*SIZE_BLOCK + 0xFF4);
                int zeroBlock2 = Array.IndexOf(BlockOrder2, 0);

                if (zeroBlock2 < 0)
                    ActiveSAV = 0;
                else if (zeroBlock1 < 0)
                    ActiveSAV = 1;
                else
                ActiveSAV = BitConverter.ToUInt32(Data, zeroBlock1*SIZE_BLOCK + 0xFFC) >
                            BitConverter.ToUInt32(Data, zeroBlock2*SIZE_BLOCK + 0xEFFC)
                    ? 0
                    : 1;
                BlockOrder = ActiveSAV == 0 ? BlockOrder1 : BlockOrder2;
            }
            else
            {
                ActiveSAV = 0;
                BlockOrder = BlockOrder1;
            }

            BlockOfs = new int[BLOCK_COUNT];
            for (int i = 0; i < BLOCK_COUNT; i++)
            {
                int index = Array.IndexOf(BlockOrder, i);
                BlockOfs[i] = index < 0 ? int.MinValue : index*SIZE_BLOCK + ABO;
            }

            // Set up PC data buffer beyond end of save file.
            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED); // More than enough empty space.

            // Copy chunk to the allocated location
            for (int i = 5; i < BLOCK_COUNT; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;
                Array.Copy(Data, blockIndex * SIZE_BLOCK + ABO, Data, Box + (i - 5)*0xF80, chunkLength[i]);
            }

            // Japanese games are limited to 5 character OT names; any unused characters are 0xFF.
            // 5 for JP, 7 for INT. There's always 1 terminator, thus we can check 0x6-0x7 being 0xFFFF = INT
            // OT name is stored at the top of the first block.
            Japanese = BitConverter.ToInt16(data, BlockOfs[0] + 0x6) == 0;

            switch (Version)
            {
                case GameVersion.RS:
                    LegalKeyItems = Legal.Pouch_Key_RS;
                    OFS_PCItem = BlockOfs[1] + 0x0498;
                    OFS_PouchHeldItem = BlockOfs[1] + 0x0560;
                    OFS_PouchKeyItem = BlockOfs[1] + 0x05B0;
                    OFS_PouchBalls = BlockOfs[1] + 0x0600;
                    OFS_PouchTMHM = BlockOfs[1] + 0x0640;
                    OFS_PouchBerry = BlockOfs[1] + 0x0740;
                    Personal = PersonalTable.RS;
                    SeenFlagOffsets = new[] {BlockOfs[0] + 0x5C, BlockOfs[1] + 0x938, BlockOfs[4] + 0xC0C};
                    break;
                case GameVersion.E:
                    LegalKeyItems = Legal.Pouch_Key_E;
                    OFS_PCItem = BlockOfs[1] + 0x0498;
                    OFS_PouchHeldItem = BlockOfs[1] + 0x0560;
                    OFS_PouchKeyItem = BlockOfs[1] + 0x05D8;
                    OFS_PouchBalls = BlockOfs[1] + 0x0650;
                    OFS_PouchTMHM = BlockOfs[1] + 0x0690;
                    OFS_PouchBerry = BlockOfs[1] + 0x0790;
                    Personal = PersonalTable.E;
                    SeenFlagOffsets = new[] {BlockOfs[0] + 0x5C, BlockOfs[1] + 0x988, BlockOfs[4] + 0xCA4};
                    break;
                case GameVersion.FRLG:
                    LegalKeyItems = Legal.Pouch_Key_FRLG;
                    OFS_PCItem = BlockOfs[1] + 0x0298;
                    OFS_PouchHeldItem = BlockOfs[1] + 0x0310;
                    OFS_PouchKeyItem = BlockOfs[1] + 0x03B8;
                    OFS_PouchBalls = BlockOfs[1] + 0x0430;
                    OFS_PouchTMHM = BlockOfs[1] + 0x0464;
                    OFS_PouchBerry = BlockOfs[1] + 0x054C;
                    Personal = PersonalTable.FR;
                    SeenFlagOffsets = new[] {BlockOfs[0] + 0x5C, BlockOfs[1] + 0x5F8, BlockOfs[4] + 0xB98};
                    break;
            }
            LoadEReaderBerryData();
            LegalItems = Legal.Pouch_Items_RS;
            LegalBalls = Legal.Pouch_Ball_RS;
            LegalTMHMs = Legal.Pouch_TMHM_RS;
            LegalBerries = Legal.Pouch_Berries_RS;
            HeldItems = Legal.HeldItems_RS;

            // Sanity Check SeenFlagOffsets -- early saves may not have block 4 initialized yet
            SeenFlagOffsets = SeenFlagOffsets.Where(z => z >= 0).ToArray();

            if (!Exportable)
                resetBoxes();
        }

        public override byte[] Write(bool DSV)
        {
            // Copy Box data back
            for (int i = 5; i < BLOCK_COUNT; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;
                Array.Copy(Data, Box + (i - 5) * 0xF80, Data, blockIndex * SIZE_BLOCK + ABO, chunkLength[i]);
            }

            setChecksums();
            return Data.Take(Data.Length - SIZE_RESERVED).ToArray();
        }

        private readonly int ActiveSAV;
        private int ABO => ActiveSAV*SIZE_BLOCK*0xE;
        private readonly int[] BlockOrder;
        private readonly int[] BlockOfs;
        public int getBlockOffset(int block) => BlockOfs[block];

        // Configuration
        public override SaveFile Clone() { return new SAV3(Write(DSV:false), Version) {Japanese = Japanese}; }
        public override bool IndeterminateGame => Version == GameVersion.Unknown;
        public override bool IndeterminateSubVersion => Version == GameVersion.FRLG;

        public override int SIZE_STORED => PKX.SIZE_3STORED;
        public override int SIZE_PARTY => PKX.SIZE_3PARTY;
        public override PKM BlankPKM => new PK3();
        public override Type PKMType => typeof(PK3);

        public override int MaxMoveID => Legal.MaxMoveID_3;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => Legal.MaxAbilityID_3;
        public override int MaxItemID => Legal.MaxItemID_3;
        public override int MaxBallID => Legal.MaxBallID_3;
        public override int MaxGameID => 5;

        public override int BoxCount => 14;
        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;

        public override bool HasParty => true;

        // Checksums
        protected override void setChecksums()
        {
            for (int i = 0; i < BLOCK_COUNT; i++)
            {
                byte[] chunk = Data.Skip(ABO + i*SIZE_BLOCK).Take(chunkLength[BlockOrder[i]]).ToArray();
                ushort chk = SaveUtil.check32(chunk);
                BitConverter.GetBytes(chk).CopyTo(Data, ABO + i*SIZE_BLOCK + 0xFF6);
            }
        }
        public override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    byte[] chunk = Data.Skip(ABO + i * SIZE_BLOCK).Take(chunkLength[BlockOrder[i]]).ToArray();
                    ushort chk = SaveUtil.check32(chunk);
                    if (chk != BitConverter.ToUInt16(Data, ABO + i*SIZE_BLOCK + 0xFF6))
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
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    byte[] chunk = Data.Skip(ABO + i * SIZE_BLOCK).Take(chunkLength[BlockOrder[i]]).ToArray();
                    ushort chk = SaveUtil.check32(chunk);
                    ushort old = BitConverter.ToUInt16(Data, ABO + i*SIZE_BLOCK + 0xFF6);
                    if (chk != old)
                        r += $"Block {BlockOrder[i]:00} @ {i*SIZE_BLOCK:X5} invalid." + Environment.NewLine;
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
            get => getString(BlockOfs[0], 0x10);
            set => setString(value, 7).CopyTo(Data, BlockOfs[0]);
        }
        public override int Gender
        {
            get => Data[BlockOfs[0] + 8];
            set => Data[BlockOfs[0] + 8] = (byte)value;
        }
        public override ushort TID
        {
            get => BitConverter.ToUInt16(Data, BlockOfs[0] + 0xA + 0);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockOfs[0] + 0xA + 0);
        }
        public override ushort SID
        {
            get => BitConverter.ToUInt16(Data, BlockOfs[0] + 0xC);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockOfs[0] + 0xC);
        }
        public override int PlayedHours
        {
            get => BitConverter.ToUInt16(Data, BlockOfs[0] + 0xE);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, BlockOfs[0] + 0xE);
        }
        public override int PlayedMinutes
        {
            get => Data[BlockOfs[0] + 0x10];
            set => Data[BlockOfs[0] + 0x10] = (byte)value;
        }
        public override int PlayedSeconds
        {
            get => Data[BlockOfs[0] + 0x11];
            set => Data[BlockOfs[0] + 0x11] = (byte)value;
        }
        public int PlayedFrames
        {
            get => Data[BlockOfs[0] + 0x12];
            set => Data[BlockOfs[0] + 0x12] = (byte)value;
        }
        public int Badges
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.E: return BitConverter.ToUInt16(Data, BlockOfs[2] + 0x3FC) >> 7 & 0xFF;
                    case GameVersion.FRLG: return Data[BlockOfs[2] + 0x64];
                    default: return 0; // RS
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.E:
                        BitConverter.GetBytes(BitConverter.ToUInt16(Data, BlockOfs[2] + 0x3FC) & ~(0xFF << 7) | (value << 7)).CopyTo(Data, BlockOfs[2] + 0x3FC);
                        break;
                    case GameVersion.FRLG: Data[BlockOfs[2] + 0x64] = (byte)value; break;
                    default: return; // RS
                }
            }
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
                    case GameVersion.E: return (ushort)(BitConverter.ToUInt16(Data, BlockOfs[1] + 0x0494) ^ SecurityKey);
                    case GameVersion.FRLG: return (ushort)(BitConverter.ToUInt16(Data, BlockOfs[1] + 0x0294) ^ SecurityKey);
                    default: return 0;
                }
            }
            set
            {
                if (value > 9999)
                    value = 9999;
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes((ushort)(value ^ SecurityKey)).CopyTo(Data, BlockOfs[1] + 0x0494); break;
                    case GameVersion.FRLG: BitConverter.GetBytes((ushort)(value ^ SecurityKey)).CopyTo(Data, BlockOfs[1] + 0x0294); break;
                }
            }
        }
        public uint BP
        {
            get => BitConverter.ToUInt16(Data, BlockOfs[0] + 0xEB8);
            set
            {
                if (value > 9999)
                    value = 9999;
                BitConverter.GetBytes((ushort)value).CopyTo(Data, BlockOfs[0] + 0xEB8);
            }
        }

        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries;
        public override InventoryPouch[] Inventory
        {
            get
            {
                int max = Version == GameVersion.FRLG ? 995 : 95;
                var PCItems = new [] {LegalItems, LegalKeyItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries}.SelectMany(a => a).ToArray();
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, LegalItems, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem)/4),
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem)/4),
                    new InventoryPouch(InventoryType.Balls, LegalBalls, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls)/4),
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM)/4),
                    new InventoryPouch(InventoryType.Berries, LegalBerries, max, OFS_PouchBerry, Version == GameVersion.FRLG ? 43 : 46),
                    new InventoryPouch(InventoryType.PCItems, PCItems, max, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem)/4),
                };
                foreach (var p in pouch)
                {
                    if (p.Type != InventoryType.PCItems)
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
            get => Data[Box];
            set => Data[Box] = (byte)value;
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
            return PKX.getString3(Data, offset + box * 9, 9, Japanese);
        }
        public override void setBoxName(int box, string value)
        {
            int offset = getBoxOffset(BoxCount);
            setString(value, 8).CopyTo(Data, offset + box * 9);
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK3(data);
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray3(data);
        }

        // Pokédex
        private readonly int[] SeenFlagOffsets;
        public override bool HasPokeDex => true;
        protected override void setDex(PKM pkm)
        {
            int species = pkm.Species;
            if (!canSetDex(species))
                return;
            
            setCaught(pkm.Species, true);
            setSeen(pkm.Species, true);
        }
        private bool canSetDex(int species)
        {
            if (species <= 0)
                return false;
            if (species > MaxSpeciesID)
                return false;
            if (Version == GameVersion.Unknown)
                return false;
            if (BlockOfs.Any(z => z < 0))
                return false;
            return true;
        }

        public override bool getCaught(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte) (1 << (bit&7));

            int caughtOffset = BlockOfs[0] + 0x28 + ofs;

            return (Data[caughtOffset] & bitval) != 0;
        }
        public override void setCaught(int species, bool caught)
        {
            int bit = species - 1;
            int ofs = bit / 8;
            int bitval = 1 << (bit&7);
            int caughtOffset = BlockOfs[0] + 0x28 + ofs;

            if (caught)
                Data[caughtOffset] |= (byte)bitval;
            else
                Data[caughtOffset] &= (byte)~bitval;
        }

        public override bool getSeen(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit&7));

            int seenOffset = BlockOfs[0] + 0x5C + ofs;
            return (Data[seenOffset] & bitval) != 0;
        }
        public override void setSeen(int species, bool seen)
        {
            int bit = species - 1;
            int ofs = bit / 8;
            int bitval = 1 << (bit&7);

            if (seen)
            {
                foreach (int o in SeenFlagOffsets)
                    Data[o + ofs] |= (byte)bitval;
            }
            else
            {
                foreach (int o in SeenFlagOffsets)
                    Data[o + ofs] &= (byte)~bitval;
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
        public override string getString(int Offset, int Count) => PKX.getString3(Data, Offset, Count, Japanese);
        public override byte[] setString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return PKX.setString3(value, maxLength, Japanese, PadToSize, PadWith);
        }

        #region eBerry
        // Offset and checksum code based from
        // https://github.com/suloku/wc-tool by Suloku
        private const int SIZE_EBERRY = 0x530;
        private const int OFFSET_EBERRY = 0x2E0;

        private uint eBerryChecksum => BitConverter.ToUInt32(Data, BlockOfs[4] + OFFSET_EBERRY + SIZE_EBERRY - 4);
        private bool eBerryChecksumValid { get; set; }

        public override string eBerryName
        {
            get
            {
                if (!GameVersion.RS.Contains(Version) || !eBerryChecksumValid)
                    return string.Empty;
                return PKX.getString3(Data, BlockOfs[4] + OFFSET_EBERRY, 7, Japanese).Trim();
            }
        }
        public override bool eBerryIsEnigma => string.IsNullOrEmpty(eBerryName.Trim());

        private void LoadEReaderBerryData()
        {
            if (!GameVersion.RS.Contains(Version))
                return;

            byte[] data = getData(BlockOfs[4] + OFFSET_EBERRY, SIZE_EBERRY - 4);

            // 8 bytes are 0x00 for chk calculation
            for (int i = 0; i < 8; i++)
                data[0xC + i] = 0x00;
            uint chk = (uint)data.Sum(z => z);
            eBerryChecksumValid = eBerryChecksum == chk;
        }
        #endregion

        // RTC
        public class RTC3
        {
            public readonly byte[] Data;
            private const int Size = 8;
            public RTC3(byte[] data = null)
            {
                if (data == null || data.Length != Size)
                    data = new byte[8];
                Data = data;
            }

            public int Day { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x00); }
            public int Hour { get => Data[2]; set => Data[2] = (byte)value; }
            public int Minute { get => Data[3]; set => Data[3] = (byte)value; }
            public int Second { get => Data[4]; set => Data[4] = (byte)value; }
        }
        public RTC3 ClockInitial
        {
            get
            {
                if (FRLG)
                    return null;
                int block0 = getBlockOffset(0);
                return new RTC3(getData(block0 + 0x98, 8));
            }
            set
            {
                if (value?.Data == null || FRLG)
                    return;
                int block0 = getBlockOffset(0);
                setData(value.Data, block0 + 0x98);
            }
        }
        public RTC3 ClockElapsed
        {
            get
            {
                if (FRLG)
                    return null;
                int block0 = getBlockOffset(0);
                return new RTC3(getData(block0 + 0xA0, 8));
            }
            set
            {
                if (value?.Data == null || FRLG)
                    return;
                int block0 = getBlockOffset(0);
                setData(value.Data, block0 + 0xA0);
            }
        }
    }
}
