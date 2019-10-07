namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> format for <see cref="GameVersion.Pt"/>
    /// </summary>
    public sealed class SAV4Pt : SAV4Sinnoh
    {
        public SAV4Pt() => Initialize();
        public SAV4Pt(byte[] data) : base(data) => Initialize();
        protected override SAV4 CloneInternal() => Exportable ? new SAV4Pt(Data) : new SAV4Pt();

        protected override int GeneralSize => 0xCF2C;
        protected override int StorageSize => 0x121E4; // Start 0xCF2C, +4 starts box data

        private void Initialize()
        {
            Version = GameVersion.Pt;
            Personal = PersonalTable.Pt;
            GetSAVOffsets();
        }

        private void GetSAVOffsets()
        {
            AdventureInfo = 0;
            Trainer1 = 0x68;
            Party = 0xA0;
            PokeDex = 0x1328;
            WondercardFlags = 0xB4C0;
            WondercardData = 0xB5C0;

            OFS_PouchHeldItem = 0x630;
            OFS_PouchKeyItem = 0x8C4;
            OFS_PouchTMHM = 0x98C;
            OFS_MailItems = 0xB1C;
            OFS_PouchMedicine = 0xB4C;
            OFS_PouchBerry = 0xBEC;
            OFS_PouchBalls = 0xCEC;
            OFS_BattleItems = 0xD28;
            LegalItems = Legal.Pouch_Items_Pt;
            LegalKeyItems = Legal.Pouch_Key_Pt;
            LegalTMHMs = Legal.Pouch_TMHM_Pt;
            LegalMedicine = Legal.Pouch_Medicine_Pt;
            LegalBerries = Legal.Pouch_Berries_Pt;
            LegalBalls = Legal.Pouch_Ball_Pt;
            LegalBattleItems = Legal.Pouch_Battle_Pt;
            LegalMailItems = Legal.Pouch_Mail_Pt;

            HeldItems = Legal.HeldItems_Pt;
            EventConst = 0xDAC;
            EventFlag = 0xFEC;
            Daycare = 0x1654;
            OFS_HONEY = 0x7F38;

            OFS_UG_Stats = 0x3CB4;

            PoketchStart = 0x1160;

            OFS_PoffinCase = 0x52E8;
            Seal = 0x6494;

            Box = 4;
        }

        #region Storage
        private static int AdjustWallpaper(int value, int shift)
        {
            // Pt's  Special Wallpapers 1-8 are shifted by +0x8
            // HG/SS Special Wallpapers 1-8 (Primo Phrases) are shifted by +0x10
            if (value >= 0x10) // special
                return value + shift;
            return value;
        }

        public override int GetBoxWallpaper(int box)
        {
            if ((uint)box > 18)
                return 0;
            int value = Storage[GetBoxWallpaperOffset(box)];
            return AdjustWallpaper(value, -0x08);
        }

        public override void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box >= 18)
                return;
            value = AdjustWallpaper(value, 0x08);
            Storage[GetBoxWallpaperOffset(box)] = (byte)value;
        }
        #endregion
    }
}