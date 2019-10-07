using System;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> format for <see cref="GameVersion.DP"/>
    /// </summary>
    public sealed class SAV4DP : SAV4Sinnoh
    {
        public SAV4DP() => Initialize();
        public SAV4DP(byte[] data) : base(data) => Initialize();
        protected override SAV4 CloneInternal() => Exportable ? new SAV4DP(Data) : new SAV4DP();

        protected override int GeneralSize => 0xC100;
        protected override int StorageSize => 0x121E0; // Start 0xC100, +4 starts box data

        private void Initialize()
        {
            Version = GameVersion.DP;
            Personal = PersonalTable.DP;
            GetSAVOffsets();
        }

        private void GetSAVOffsets()
        {
            AdventureInfo = 0;
            Trainer1 = 0x64;
            Party = 0x98;
            PokeDex = 0x12DC;
            WondercardFlags = 0xA6D0;
            WondercardData = 0xA7fC;

            OFS_PouchHeldItem = 0x624;
            OFS_PouchKeyItem = 0x8B8;
            OFS_PouchTMHM = 0x980;
            OFS_MailItems = 0xB10;
            OFS_PouchMedicine = 0xB40;
            OFS_PouchBerry = 0xBE0;
            OFS_PouchBalls = 0xCE0;
            OFS_BattleItems = 0xD1C;
            LegalItems = Legal.Pouch_Items_DP;
            LegalKeyItems = Legal.Pouch_Key_DP;
            LegalTMHMs = Legal.Pouch_TMHM_DP;
            LegalMedicine = Legal.Pouch_Medicine_DP;
            LegalBerries = Legal.Pouch_Berries_DP;
            LegalBalls = Legal.Pouch_Ball_DP;
            LegalBattleItems = Legal.Pouch_Battle_DP;
            LegalMailItems = Legal.Pouch_Mail_DP;

            HeldItems = Legal.HeldItems_DP;
            EventConst = 0xD9C;
            EventFlag = 0xFDC;
            Daycare = 0x141C;
            OFS_HONEY = 0x72E4;

            OFS_UG_Stats = 0x3A2C;

            PoketchStart = 0x114C;
            Seal = 0x6178;

            OFS_PoffinCase = 0x5050;

            Box = 4;
        }

        #region Storage
        public override int GetBoxWallpaper(int box)
        {
            if ((uint)box >= 18)
                return 0;
            return Storage[GetBoxWallpaperOffset(box)];
        }

        public override void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box >= 18)
                return;
            Storage[GetBoxWallpaperOffset(box)] = (byte)value;
        }
        #endregion

        private const uint MysteryGiftDPSlotActive = 0xEDB88320;

        private bool[] MysteryGiftDPSlotActiveFlags
        {
            get
            {
                int ofs = WondercardFlags + 0x100; // skip over flags
                bool[] active = new bool[GiftCountMax]; // 8 PGT, 3 PCD
                for (int i = 0; i < active.Length; i++)
                    active[i] = BitConverter.ToUInt32(General, ofs + (4 * i)) == MysteryGiftDPSlotActive;

                return active;
            }
            set
            {
                if (value?.Length != GiftCountMax)
                    return;

                int ofs = WondercardFlags + 0x100; // skip over flags
                for (int i = 0; i < value.Length; i++)
                {
                    byte[] magic = BitConverter.GetBytes(value[i] ? MysteryGiftDPSlotActive : 0); // 4 bytes
                    SetData(General, magic, ofs + (4 * i));
                }
            }
        }

        public override MysteryGiftAlbum GiftAlbum
        {
            get => base.GiftAlbum;
            set
            {
                base.GiftAlbum = value;
                SetActiveGiftFlags(value.Gifts);
            }
        }

        private void SetActiveGiftFlags(MysteryGift[] gifts)
        {
            var arr = new bool[gifts.Length];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = !gifts[i].Empty;
            MysteryGiftDPSlotActiveFlags = arr;
        }
    }
}