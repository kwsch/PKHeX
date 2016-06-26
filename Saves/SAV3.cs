using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV3 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version})" +/* - {LastSavedTime}*/ "].bak";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => "";

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

        public SAV3(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G3RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            BlockOrder = new int[14];
            ActiveSAV = SaveUtil.SIZE_G3RAWHALF == data.Length || BitConverter.ToUInt32(Data, 0xFFC) > BitConverter.ToUInt32(Data, 0xEFFC) ? 0 : 1;
            for (int i = 0; i < 14; i++)
                BlockOrder[i] = BitConverter.ToInt16(Data, ABO + i*0x1000 + 0xFF4);

            // Detect RS/E/FRLG
            // Section 0 stores Game Code @ 0x00AC; 0 for RS, 1 for FRLG, else for Emerald
            Trainer1 = Array.IndexOf(BlockOrder, 0)*0x1000 + ABO;
            Party = Array.IndexOf(BlockOrder, 1)*0x1000 + ABO;
            Block2 = Array.IndexOf(BlockOrder, 2)*0x1000 + ABO;
            Block3 = Array.IndexOf(BlockOrder, 3)*0x1000 + ABO;

            uint GameCode = BitConverter.ToUInt32(Data, ABO + Array.IndexOf(BlockOrder, 0)*0x1000);
            SaveVersion = GameCode < 2 ? (int)GameCode : 2;

            // Set up PC data buffer beyond end of save file.
            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + 0x10000); // More than enough empty space.

            // Copy chunk to the allocated location
            for (int i = 5; i < 14; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;
                Array.Copy(Data, blockIndex * 0x1000 + ABO, Data, Box + (i - 5)*0xF80, chunkLength[i]);
            }

            HeldItems = Legal.HeldItems_RS;

            if (!Exportable)
                resetBoxes();
        }

        private readonly int SaveVersion;
        private readonly int ActiveSAV;
        private int ABO => ActiveSAV*0xE000;
        private readonly int[] BlockOrder;

        // Configuration
        public override byte[] BAK { get; }
        public override bool Exportable { get; }
        public override SaveFile Clone() { return new SAV3(Data.Take(Box).ToArray()); }

        public override int SIZE_STORED => PKX.SIZE_3STORED;
        public override int SIZE_PARTY => PKX.SIZE_3PARTY;
        public override PKM BlankPKM => new PK3();
        protected override Type PKMType => typeof(PK3);

        public override int MaxMoveID => 354;
        public override int MaxSpeciesID => 386;
        public override int MaxAbilityID => 77;
        public override int MaxItemID => 374;
        public override int MaxBallID => 0xC;
        public override int MaxGameID => 5;

        public override int BoxCount => 14;
        public override int MaxEV => 252;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 8;
        public override int NickLength => 10;

        private int Block2, Block3;

        // Checksums
        protected override void setChecksums()
        {
            for (int i = 0; i < 14; i++)
            {
                byte[] chunk = Data.Skip(ABO + i*0x1000).Take(chunkLength[BlockOrder[i]]).ToArray();
                ushort chk = SaveUtil.check32(chunk);
                BitConverter.GetBytes(chk).CopyTo(Data, ABO + i + 0xFF4);
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
                    if (chk != BitConverter.ToUInt16(Data, ABO + i*0xFF4))
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
                    if (chk != BitConverter.ToUInt16(Data, ABO + i * 0xFF4))
                        r += $"Block {BlockOrder[i]} @ {i*0x1000} (len {chunkLength[BlockOrder[i]]}) invalid." + Environment.NewLine;
                }
                return r.Length == 0 ? "Checksums valid." : r.TrimEnd();
            }
        }

        // Trainer Info
        public override GameVersion Version
        {
            get
            {
                switch (SaveVersion)
                {
                    case 0: return GameVersion.RS;
                    case 1: return GameVersion.FRLG;
                    case 2: return GameVersion.E;
                }
                return GameVersion.Unknown;
            }
        }

        private bool Japanese;
        private uint SecurityKey
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.E: return BitConverter.ToUInt32(Data, Trainer1 + 0xAC);
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Data, Trainer1 + 0xAF8);
                    default: return 0;
                }
            }
        }
        public override string OT
        {
            get
            {
                return PKX.getG3Str(Data.Skip(Trainer1).Take(0x10).ToArray(), Japanese)
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
                PKX.setG3Str(TempNick, Japanese).CopyTo(Data, Trainer1);
            }
        }
        public override int Gender
        {
            get { return Data[Trainer1 + 8]; }
            set { Data[Trainer1 + 8] = (byte)value; }
        }
        public override ushort TID
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0xA + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0xA + 0); }
        }
        public override ushort SID
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0xC); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0xC); }
        }
        public override int PlayedHours
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0xE); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0xE); }
        }
        public override int PlayedMinutes
        {
            get { return Data[Trainer1 + 0x10]; }
            set { Data[Trainer1 + 0x10] = (byte)value; }
        }
        public override int PlayedSeconds
        {
            get { return Data[Trainer1 + 0x11]; }
            set { Data[Trainer1 + 0x11] = (byte)value; }
        }
        public int PlayedFrames
        {
            get { return Data[Trainer1 + 0x12]; }
            set { Data[Trainer1 + 0x12] = (byte)value; }
        }

        public override uint Money
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: return BitConverter.ToUInt32(Data, Party + 0x0490) ^ SecurityKey;
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Data, Party + 0x0290) ^ SecurityKey;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, Party + 0x0490); break;
                    case GameVersion.FRLG: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, Party + 0x0290); break;
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
                    case GameVersion.E: return BitConverter.ToUInt32(Data, Party + 0x0494) ^ SecurityKey;
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Data, Party + 0x0294) ^ SecurityKey;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, Party + 0x0494); break;
                    case GameVersion.FRLG: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Data, Party + 0x0294); break;
                }
            }
        }
        public int BP
        {
            get { return Data[Trainer1 + 0xEB8]; }
            set { Data[Trainer1 + 0xEB8] = (byte)value; }
        }

        public override InventoryPouch[] Inventory
        {
            get
            {
                return null; 
                
            }
            set { }
        }
        public override int Language { get { return 0; } set { } }

        public override int getDaycareSlotOffset(int loc, int slot)
        {
            return Daycare + slot * SIZE_PARTY;
        }
        public override ulong? getDaycareRNGSeed(int loc)
        {
            return null;
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
                return Data[Party + ofs]; 
                
            }
            protected set
            {
                int ofs = 0x34;
                if (GameVersion.FRLG != Version)
                    ofs += 0x200;
                Data[Party + ofs] = (byte)value; 
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
            return Party + ofs + SIZE_PARTY * slot;
        }
        public override int CurrentBox
        {
            get { return Data[Box]; }
            set { Data[Box] = (byte)value; }
        }
        public override int getBoxWallpaper(int box)
        {
            // Box Wallpaper is directly after the Box Names
            int offset = getBoxOffset(BoxCount);
            offset += BoxCount * 0x9;
            return Data[offset];
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
    }
}
