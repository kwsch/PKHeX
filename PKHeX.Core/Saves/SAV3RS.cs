using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.RS"/>.
    /// </summary>
    /// <inheritdoc cref="SAV3" />
    public sealed class SAV3RS : SAV3, IGen3Hoenn
    {
        // Configuration
        protected override SaveFile CloneInternal() => new SAV3RS(Write());
        public override GameVersion Version { get => GameVersion.RS; protected set { } }
        public override PersonalTable Personal => PersonalTable.RS;

        protected override int EventFlagMax => 8 * 288;
        protected override int EventConstMax => 0x100;
        protected override int DaycareSlotSize => SIZE_STORED;
        public override int DaycareSeedSize => 4; // 16bit
        protected override int EggEventFlag => 0x86;
        protected override int BadgeFlagStart => 0x807;

        public SAV3RS(byte[] data) : base(data) => Initialize();
        public SAV3RS(bool japanese = false) : base(japanese) => Initialize();

        private void Initialize()
        {
            // small
            PokeDex = 0x18;

            // large
            EventFlag = 0x1220;
            EventConst = 0x1340;
            DaycareOffset = 0x2F9C;

            // storage
            Box = 0;
        }

        #region Small
        public override bool NationalDex
        {
            get => PokedexNationalMagicRSE == PokedexNationalUnlockRSE;
            set
            {
                PokedexMode = value ? (byte)1 : (byte)0; // mode
                PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : (byte)0; // magic
                SetEventFlag(0x836, value);
                SetEventConst(0x46, PokedexNationalUnlockWorkRSE);
            }
        }

        public override uint SecurityKey { get => 0; set { } }

        public RTC3 ClockInitial
        {
            get => new(GetData(Small, 0x98, RTC3.Size));
            set => SetData(Small, value.Data, 0x98);
        }

        public RTC3 ClockElapsed
        {
            get => new(GetData(Small, 0xA0, RTC3.Size));
            set => SetData(Small, value.Data, 0xA0);
        }
        #endregion

        #region Large
        public override int PartyCount { get => Large[0x234]; protected set => Large[0x234] = (byte)value; }
        public override int GetPartyOffset(int slot) => 0x238 + (SIZE_PARTY * slot);

        public override uint Money
        {
            get => BitConverter.ToUInt32(Large, 0x0490);
            set => SetData(Large, BitConverter.GetBytes(value), 0x0490);
        }

        public override uint Coin
        {
            get => BitConverter.ToUInt16(Large, 0x0494);
            set => SetData(Large, BitConverter.GetBytes((ushort)value), 0x0494);
        }

        private const int OFS_PCItem = 0x0498;
        private const int OFS_PouchHeldItem = 0x0560;
        private const int OFS_PouchKeyItem = 0x05B0;
        private const int OFS_PouchBalls = 0x0600;
        private const int OFS_PouchTMHM = 0x0640;
        private const int OFS_PouchBerry = 0x0740;

        protected override InventoryPouch3[] GetItems()
        {
            const int max = 99;
            var PCItems = ArrayUtil.ConcatAll(Legal.Pouch_Items_RS, Legal.Pouch_Key_RS, Legal.Pouch_Ball_RS, Legal.Pouch_TMHM_RS, Legal.Pouch_Berries_RS);
            return new InventoryPouch3[]
            {
                new(InventoryType.Items, Legal.Pouch_Items_RS, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
                new(InventoryType.KeyItems, Legal.Pouch_Key_RS, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
                new(InventoryType.Balls, Legal.Pouch_Ball_RS, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
                new(InventoryType.TMHMs, Legal.Pouch_TMHM_RS, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
                new(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 46),
                new(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
            };
        }

        public PokeBlock3Case PokeBlocks
        {
            get => new(Large, 0x7F8);
            set => SetData(Large, value.Write(), 0x7F8);
        }

        protected override int SeenOffset2 => 0x938;

        public DecorationInventory3 Decorations
        {
            get => Large.Slice(0x26A0, DecorationInventory3.SIZE).ToStructure<DecorationInventory3>();
            set => SetData(Large, value.ToBytes(), 0x26A0);
        }

        protected override int MailOffset => 0x2B4C;

        protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(0, 2) + (2 * 0x38) + (4 * slot); // consecutive vals, after both consecutive slots & 2 mail
        public override string GetDaycareRNGSeed(int loc) => BitConverter.ToUInt16(Large, GetDaycareEXPOffset(2)).ToString("X4");
        public override void SetDaycareRNGSeed(int loc, string seed) => BitConverter.GetBytes((ushort)Util.GetHexValue(seed)).CopyTo(Large, GetDaycareEXPOffset(2));

        private const int ExternalEventFlags = 0x312F;

        public bool HasReceivedWishmkrJirachi
        {
            get => GetFlag(ExternalEventFlags + 2, 0);
            set => SetFlag(ExternalEventFlags + 2, 0, value);
        }

        #region eBerry
        private const int OFFSET_EBERRY = 0x3160;
        private const int SIZE_EBERRY = 0x530;

        public byte[] GetEReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
        public void SetEReaderBerry(byte[] data) => SetData(Large, data, OFFSET_EBERRY);

        public override string EBerryName => GetString(Large, OFFSET_EBERRY, 7);
        public override bool IsEBerryEngima => Large[OFFSET_EBERRY] is 0 or 0xFF;
        #endregion

        public override MysteryEvent3 MysteryEvent
        {
            get => new(Large.Slice(0x3690, MysteryEvent3.SIZE));
            set => SetData(Large, value.Data, 0x3690);
        }

        protected override int SeenOffset3 => 0x3A8C;
        #endregion
    }
}
