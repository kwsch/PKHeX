using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object for Pokémon Ruby Sapphire Box saves.
    /// </summary>
    public sealed class SAV3RSBox : SaveFile, IGCSaveFile
    {
        protected override string BAKText => $"{Version} #{SaveCount:0000}";
        public override string Filter => this.GCFilter();
        public override string Extension => this.GCExtension();
        public bool IsMemoryCardSave => MC != null;
        private readonly SAV3GCMemoryCard MC;

        public SAV3RSBox(byte[] data, SAV3GCMemoryCard MC) : this(data) { this.MC = MC; BAK = MC.Data; }

        public SAV3RSBox(byte[] data = null)
        {
            Data = data ?? new byte[SaveUtil.SIZE_G3BOX];
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            if (SaveUtil.GetIsG3BOXSAV(Data) != GameVersion.RSBOX)
                return;

            Blocks = new BlockInfoRSBOX[2*BLOCK_COUNT];
            for (int i = 0; i < Blocks.Length; i++)
            {
                int offset = BLOCK_SIZE + (i * BLOCK_SIZE);
                Blocks[i] = new BlockInfoRSBOX(Data, offset);
            }

            // Detect active save
            int[] SaveCounts = Blocks.Select(block => (int)block.SaveCount).ToArray();
            SaveCount = SaveCounts.Max();
            int ActiveSAV = Array.IndexOf(SaveCounts, SaveCount) / BLOCK_COUNT;
            Blocks = Blocks.Skip(ActiveSAV*BLOCK_COUNT).Take(BLOCK_COUNT).OrderBy(b => b.ID).ToArray();

            // Set up PC data buffer beyond end of save file.
            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED); // More than enough empty space.

            // Copy block to the allocated location
            const int copySize = BLOCK_SIZE - 0x10;
            foreach (var b in Blocks)
                Array.Copy(Data, b.Offset + 0xC, Data, (int)(Box + (b.ID*copySize)), copySize);

            Personal = PersonalTable.RS;
            HeldItems = Legal.HeldItems_RS;

            if (!Exportable)
                ClearBoxes();
        }

        private readonly BlockInfoRSBOX[] Blocks;
        private readonly int SaveCount;
        private const int BLOCK_COUNT = 23;
        private const int BLOCK_SIZE = 0x2000;
        private const int SIZE_RESERVED = BLOCK_COUNT * BLOCK_SIZE; // unpacked box data

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
            // Copy Box data back
            const int copySize = BLOCK_SIZE - 0x10;
            foreach (var b in Blocks)
                Array.Copy(Data, (int) (Box + (b.ID * copySize)), Data, b.Offset + 0xC, copySize);

            SetChecksums();

            return GetData(0, Data.Length - SIZE_RESERVED);
        }

        // Configuration
        public override SaveFile Clone()
        {
            var data = GetInnerData();
            var sav = IsMemoryCardSave ? new SAV3RSBox(data, MC) : new SAV3RSBox(data);
            sav.Header = (byte[])Header.Clone();
            return sav;
        }

        public override int SIZE_STORED => PKX.SIZE_3STORED + 4;
        protected override int SIZE_PARTY => PKX.SIZE_3PARTY; // unused
        public override PKM BlankPKM => new PK3();
        public override Type PKMType => typeof(PK3);

        public override int MaxMoveID => Legal.MaxMoveID_3;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => Legal.MaxAbilityID_3;
        public override int MaxItemID => Legal.MaxItemID_3;
        public override int MaxBallID => Legal.MaxBallID_3;
        public override int MaxGameID => Legal.MaxGameID_3;

        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;
        public override bool HasBoxWallpapers => false;

        public override int BoxCount => 50;
        public override bool HasParty => false;
        public override bool IsPKMPresent(int offset) => PKX.IsPKMPresentGBA(Data, offset);

        // Checksums
        protected override void SetChecksums() => Blocks.SetChecksums(Data);
        public override bool ChecksumsValid => Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => Blocks.GetChecksumInfo(Data);

        // Trainer Info
        public override GameVersion Version { get => GameVersion.RSBOX; protected set { } }

        // Storage
        public override int GetPartyOffset(int slot) => -1;
        public override int GetBoxOffset(int box) => Box + 8 + (SIZE_STORED * box * 30);

        public override int CurrentBox
        {
            get => Data[Box + 4] * 2;
            set => Data[Box + 4] = (byte)(value / 2);
        }

        protected override int GetBoxWallpaperOffset(int box)
        {
            // Box Wallpaper is directly after the Box Names
            int offset = Box + 0x1ED19 + (box / 2);
            return offset;
        }

        public override string GetBoxName(int box)
        {
            // Tweaked for the 1-30/31-60 box showing
            int lo = (30 *(box%2)) + 1;
            int hi = 30*((box % 2) + 1);
            string boxName = $"[{lo:00}-{hi:00}] ";
            box /= 2;

            int offset = Box + 0x1EC38 + (9 * box);
            if (Data[offset] == 0 || Data[offset] == 0xFF)
                boxName += $"BOX {box + 1}";
            boxName += GetString(offset, 9);

            return boxName;
        }

        public override void SetBoxName(int box, string value)
        {
            int offset = Box + 0x1EC38 + (9 * box);
            byte[] data = value == $"BOX {box + 1}" ? new byte[9] : SetString(value, 8);
            SetData(data, offset);
        }

        protected override PKM GetPKM(byte[] data)
        {
            if (data.Length != PKX.SIZE_3STORED)
                Array.Resize(ref data, PKX.SIZE_3STORED);
            return new PK3(data);
        }

        protected override byte[] DecryptPKM(byte[] data)
        {
            if (data.Length != PKX.SIZE_3STORED)
                Array.Resize(ref data, PKX.SIZE_3STORED);
            return PKX.DecryptArray3(data);
        }

        protected override void SetDex(PKM pkm) { /* No Pokedex for this game, do nothing */ }

        public override void SetStoredSlot(PKM pkm, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
        {
            base.SetStoredSlot(pkm, offset, trade, dex);
            BitConverter.GetBytes((ushort)pkm.TID).CopyTo(Data, offset + PKX.SIZE_3STORED + 0);
            BitConverter.GetBytes((ushort)pkm.SID).CopyTo(Data, offset + PKX.SIZE_3STORED + 2);
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter3.GetString3(data, offset, length, Japanese);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter3.SetString3(value, maxLength, Japanese, PadToSize, PadWith);
        }
    }
}
