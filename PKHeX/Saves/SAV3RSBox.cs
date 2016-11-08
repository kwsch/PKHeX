using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV3RSBox : SaveFile
    {
        public override string BAKName => $"{FileName} [{Version} #{SaveCount.ToString("0000")}].bak";
        public override string Filter => "GameCube Save File|*.gci|All Files|*.*";
        public override string Extension => ".gci";

        public SAV3RSBox(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G3BOX] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);
            
            if (SaveUtil.getIsG3BOXSAV(Data) != GameVersion.RSBOX)
                return;
            
            Blocks = new RSBOX_Block[2*BLOCK_COUNT];
            for (int i = 0; i < Blocks.Length; i++)
            {
                int offset = BLOCK_SIZE + i* BLOCK_SIZE;
                Blocks[i] = new RSBOX_Block(Data.Skip(offset).Take(BLOCK_SIZE).ToArray(), offset);
            }

            // Detect active save
            int[] SaveCounts = Blocks.Select(block => (int)block.SaveCount).ToArray();
            SaveCount = SaveCounts.Max();
            int ActiveSAV = Array.IndexOf(SaveCounts, SaveCount) / BLOCK_COUNT;
            Blocks = Blocks.Skip(ActiveSAV*BLOCK_COUNT).Take(BLOCK_COUNT).OrderBy(b => b.BlockNumber).ToArray();
            
            // Set up PC data buffer beyond end of save file.
            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED); // More than enough empty space.

            // Copy block to the allocated location
            foreach (RSBOX_Block b in Blocks)
                Array.Copy(b.Data, 0xC, Data, Box + b.BlockNumber*(BLOCK_SIZE - 0x10), b.Data.Length - 0x10);

            Personal = PersonalTable.RS;
            HeldItems = Legal.HeldItems_RS;

            if (!Exportable)
                resetBoxes();
        }

        private readonly RSBOX_Block[] Blocks;
        private readonly int SaveCount;
        private const int BLOCK_COUNT = 23;
        private const int BLOCK_SIZE = 0x2000;
        private const int SIZE_RESERVED = BLOCK_COUNT * BLOCK_SIZE; // unpacked box data
        public override byte[] Write(bool DSV)
        {
            // Copy Box data back to block
            foreach (RSBOX_Block b in Blocks)
                Array.Copy(Data, Box + b.BlockNumber * (BLOCK_SIZE - 0x10), b.Data, 0xC, b.Data.Length - 0x10);

            setChecksums();

            // Set Data Back
            foreach (RSBOX_Block b in Blocks)
                b.Data.CopyTo(Data, b.Offset);
            byte[] newFile = Data.Take(Data.Length - SIZE_RESERVED).ToArray();
            return Header.Concat(newFile).ToArray();
        }

        // Configuration
        public override SaveFile Clone() { return new SAV3(Write(DSV: false), Version); }

        public override int SIZE_STORED => PKX.SIZE_3STORED + 4;
        public override int SIZE_PARTY => PKX.SIZE_3PARTY; // unused
        public override PKM BlankPKM => new PK3();
        public override Type PKMType => typeof(PK3);

        public override int MaxMoveID => 354;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
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

        public override int BoxCount => 50;
        public override bool HasParty => false;

        // Checksums
        protected override void setChecksums()
        {
            foreach (RSBOX_Block b in Blocks)
                b.SetChecksums();
        }
        public override bool ChecksumsValid
        {
            get { return Blocks.All(t => t.ChecksumsValid); }
        }
        public override string ChecksumInfo
        {
            get
            {
                return string.Join(Environment.NewLine, 
                    Blocks.Where(b => !b.ChecksumsValid).Select(b => $"Block {b.BlockNumber.ToString("00")} invalid"));
            }
        }

        // Trainer Info
        public override GameVersion Version { get { return GameVersion.RSBOX; } protected set { } }

        // Storage
        public override int getPartyOffset(int slot)
        {
            return -1;
        }
        public override int getBoxOffset(int box)
        {
            return Box + 8 + SIZE_STORED * box * 30;
        }
        public override int CurrentBox
        {
            get { return Data[Box + 4]*2; }
            set { Data[Box + 4] = (byte)(value/2); }
        }
        protected override int getBoxWallpaperOffset(int box)
        {
            // Box Wallpaper is directly after the Box Names
            int offset = Box + 0x1ED19 + box/2;
            return offset;
        }
        public override string getBoxName(int box)
        {
            // Tweaked for the 1-30/31-60 box showing
            string lo = (30*(box%2) + 1).ToString("00");
            string hi = (30*(box%2 + 1)).ToString("00");
            string boxName = $"[{lo}-{hi}] ";
            box = box / 2;

            int offset = Box + 0x1EC38 + 9 * box;
            if (Data[offset] == 0 || Data[offset] == 0xFF)
                boxName += $"BOX {box + 1}";
            boxName += PKX.getG3Str(Data.Skip(offset).Take(9).ToArray(), Japanese);

            return boxName;
        }
        public override void setBoxName(int box, string value)
        {
            int offset = Box + 0x1EC38 + 9 * box;
            if (value.Length > 8)
                value = value.Substring(0, 8); // Hard cap
            if (value == "BOX " + (box + 1))
                new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }.CopyTo(Data, offset);
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK3(data.Take(PKX.SIZE_3STORED).ToArray());
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray3(data.Take(PKX.SIZE_3STORED).ToArray());
        }

        protected override void setDex(PKM pkm) { }
        
        public override void setStoredSlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new InvalidCastException($"PKM Format needs to be {PKMType} when setting to a Gen{Generation} Save File.");
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);
            byte[] data = pkm.EncryptedBoxData;
            setData(data, offset);

            BitConverter.GetBytes((ushort)pkm.TID).CopyTo(Data, offset + data.Length + 0);
            BitConverter.GetBytes((ushort)pkm.SID).CopyTo(Data, offset + data.Length + 2);
            Edited = true;
        }
    }
}
