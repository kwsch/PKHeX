using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV5 : SaveFile, ISaveBlock5BW
    {
        protected override PKM GetPKM(byte[] data) => new PK5(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray45(data);

        protected override string BAKText => $"{OT} ({(GameVersion)Game}) - {PlayTimeString}";
        public override string Filter => (Footer.Length != 0 ? "DeSmuME DSV|*.dsv|" : string.Empty) + "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_BW;
        public override int SIZE_STORED => PokeCrypto.SIZE_5STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_5PARTY;
        public override PKM BlankPKM => new PK5();
        public override Type PKMType => typeof(PK5);

        public override int BoxCount => 24;
        public override int MaxEV => 255;
        public override int Generation => 5;
        public override int OTLength => 7;
        public override int NickLength => 10;
        protected override int GiftCountMax => 12;

        public override int MaxMoveID => Legal.MaxMoveID_5;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_5;
        public override int MaxAbilityID => Legal.MaxAbilityID_5;
        public override int MaxBallID => Legal.MaxBallID_5;
        public override int MaxGameID => Legal.MaxGameID_5; // B2

        protected SAV5(int size) : base(size)
        {
            Initialize();
            ClearBoxes();
        }

        protected SAV5(byte[] data) : base(data)
        {
            Initialize();
        }

        public override GameVersion Version
        {
            get => (GameVersion)Game;
            protected set => Game = (int)value;
        }

        private void Initialize()
        {
            Box = 0x400;
            Party = 0x18E00;
            AdventureInfo = 0x1D900;
        }

        // Blocks & Offsets
        protected override void SetChecksums() => AllBlocks.SetChecksums(Data);
        public override bool ChecksumsValid => AllBlocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => AllBlocks.GetChecksumInfo(Data);

        protected int CGearInfoOffset;
        protected int CGearDataOffset;
        protected int EntreeForestOffset;
        private int AdventureInfo;
        public int GTS { get; protected set; } = int.MinValue;
        public int Fused { get; protected set; } = int.MinValue;

        // Daycare
        public override int DaycareSeedSize => Daycare5.DaycareSeedSize;
        public override bool? IsDaycareOccupied(int loc, int slot) => Daycare.IsOccupied(slot);
        public override int GetDaycareSlotOffset(int loc, int slot) => Daycare.GetPKMOffset(slot);
        public override uint? GetDaycareEXP(int loc, int slot) => Daycare.GetEXP(slot);
        public override string GetDaycareRNGSeed(int loc) => Daycare.GetSeed()?.ToString("X16") ?? string.Empty;
        public override void SetDaycareEXP(int loc, int slot, uint EXP) => Daycare.SetEXP(slot, EXP);
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) => Daycare.SetOccupied(slot, occupied);
        public override void SetDaycareRNGSeed(int loc, string seed) => Daycare.SetSeed(seed);

        // Storage
        public override int PartyCount
        {
            get => Data[Party + 4];
            protected set => Data[Party + 4] = (byte)value;
        }

        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30) + (box * 0x10);
        public override int GetPartyOffset(int slot) => Party + 8 + (SIZE_PARTY * slot);

        protected override int GetBoxWallpaperOffset(int box) => BoxLayout.GetBoxWallpaperOffset(box);
        public override int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
        public override void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
        public override string GetBoxName(int box) => BoxLayout[box];
        public override void SetBoxName(int box, string value) => BoxLayout[box] = value;
        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }

        protected int BattleBoxOffset;

        public bool BattleBoxLocked
        {
            get => Data[BattleBoxOffset + 0x358] != 0; // wifi/live
            set => Data[BattleBoxOffset + 0x358] = (byte)(value ? 1 : 0);
        }

        protected override void SetPKM(PKM pkm)
        {
            var pk5 = (PK5)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            if (pk5.Trade(OT, TID, SID, Gender, Date.Day, Date.Month, Date.Year))
                pkm.RefreshChecksum();
        }

        // Player Data
        public override string OT { get => PlayerData.OT; set => PlayerData.OT = value; }
        public override int TID { get => PlayerData.TID; set => PlayerData.TID = value; }
        public override int SID { get => PlayerData.SID; set => PlayerData.SID = value; }
        public override int Language { get => PlayerData.Language; set => PlayerData.Language = value; }
        public override int Game { get => PlayerData.Game; set => PlayerData.Game = value; }
        public override int Gender { get => PlayerData.Gender; set => PlayerData.Gender = value; }
        public override int PlayedHours { get => PlayerData.PlayedHours; set => PlayerData.PlayedHours = value; }
        public override int PlayedMinutes { get => PlayerData.PlayedMinutes; set => PlayerData.PlayedMinutes = value; }
        public override int PlayedSeconds { get => PlayerData.PlayedSeconds; set => PlayerData.PlayedSeconds = value; }
        public override uint Money { get => Misc.Money; set => Misc.Money = value; }
        public override uint SecondsToStart { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x34); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x34); }
        public override uint SecondsToFame { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x3C); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x3C); }
        public override MysteryGiftAlbum GiftAlbum { get => Mystery.GiftAlbum; set => Mystery.GiftAlbum = (EncryptedMysteryGiftAlbum)value; }
        public override InventoryPouch[] Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString5(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString5(value, maxLength, PadToSize, PadWith);
        }

        // DLC
        private int CGearSkinInfoOffset => CGearInfoOffset + (this is SAV5B2W2 ? 0x10 : 0) + 0x24;

        private bool CGearSkinPresent
        {
            get => Data[CGearSkinInfoOffset + 2] == 1;
            set => Data[CGearSkinInfoOffset + 2] = Data[PlayerData.Offset + (this is SAV5B2W2 ? 0x6C : 0x54)] = (byte) (value ? 1 : 0);
        }

        public byte[] CGearSkinData
        {
            get
            {
                byte[] data = new byte[CGearBackground.SIZE_CGB];
                if (CGearSkinPresent)
                    Array.Copy(Data, CGearDataOffset, data, 0, data.Length);
                return data;
            }
            set
            {
                byte[] dlcfooter = { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x27, 0x00, 0x00, 0x27, 0x35, 0x05, 0x31, 0x00, 0x00 };

                byte[] bgdata = value;
                SetData(bgdata, CGearDataOffset);

                ushort chk = Checksums.CRC16_CCITT(bgdata);
                var chkbytes = BitConverter.GetBytes(chk);
                int footer = CGearDataOffset + bgdata.Length;

                BitConverter.GetBytes((ushort)1).CopyTo(Data, footer); // block updated once
                chkbytes.CopyTo(Data, footer + 2); // checksum
                chkbytes.CopyTo(Data, footer + 0x100); // second checksum
                dlcfooter.CopyTo(Data, footer + 0x102);
                ushort skinchkval = Checksums.CRC16_CCITT(Data, footer + 0x100, 4);
                BitConverter.GetBytes(skinchkval).CopyTo(Data, footer + 0x112);

                // Indicate in the save file that data is present
                BitConverter.GetBytes((ushort)0xC21E).CopyTo(Data, 0x19438);

                chkbytes.CopyTo(Data, CGearSkinInfoOffset);
                CGearSkinPresent = true;

                Edited = true;
            }
        }

        public EntreeForest EntreeData
        {
            get => new EntreeForest(GetData(EntreeForestOffset, 0x850));
            set => SetData(value.Write(), EntreeForestOffset);
        }

        public abstract IReadOnlyList<BlockInfo> AllBlocks { get; }
        public abstract MyItem Items { get; }
        public abstract Zukan5 Zukan { get; }
        public abstract Misc5 Misc { get; }
        public abstract MysteryBlock5 Mystery { get; }
        public abstract Daycare5 Daycare { get; }
        public abstract BoxLayout5 BoxLayout { get; }
        public abstract PlayerData5 PlayerData { get; }
        public abstract BattleSubway5 BattleSubway { get; }

        public static int GetMailOffset(int index) => (index * Mail5.SIZE) + 0x1DD00;
        public byte[] GetMailData(int offset) => GetData(offset, Mail5.SIZE);
        public int GetBattleBoxSlot(int slot) => BattleBoxOffset + (slot * SIZE_STORED);
    }
}
