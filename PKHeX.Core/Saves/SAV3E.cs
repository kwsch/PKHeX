using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.E"/>.
    /// </summary>
    /// <inheritdoc cref="SAV3" />
    public sealed class SAV3E : SAV3, IGen3Hoenn, IGen3Joyful, IGen3Wonder
    {
        // Configuration
        protected override SaveFile CloneInternal() => new SAV3E(Write());
        public override GameVersion Version { get => GameVersion.E; protected set { } }
        public override PersonalTable Personal => PersonalTable.E;

        protected override int EventFlagMax => 8 * 300;
        protected override int EventConstMax => 0x100;
        protected override int DaycareSlotSize => SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
        public override int DaycareSeedSize => 8; // 32bit
        protected override int EggEventFlag => 0x86;
        protected override int BadgeFlagStart => 0x867;

        public SAV3E(byte[] data) : base(data) => Initialize();
        public SAV3E(bool japanese = false) : base(japanese) => Initialize();

        private void Initialize()
        {
            // small
            PokeDex = 0x18;

            // large
            EventFlag = 0x1270;
            EventConst = 0x139C;
            DaycareOffset = 0x3030;

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
                SetEventFlag(0x896, value);
                SetEventConst(0x46, PokedexNationalUnlockWorkRSE);
            }
        }

        public override uint SecurityKey
        {
            get => BitConverter.ToUInt32(Small, 0xAC);
            set => SetData(Small, BitConverter.GetBytes(value), 0xAC);
        }

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

        public ushort JoyfulJumpInRow           { get => BitConverter.ToUInt16(Small, 0x1FC); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0x1FC); }
        // u16 field2;
        public ushort JoyfulJump5InRow          { get => BitConverter.ToUInt16(Small, 0x200); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0x200); }
        public ushort JoyfulJumpGamesMaxPlayers { get => BitConverter.ToUInt16(Small, 0x202); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0x202); }
        // u32 field8;
        public uint   JoyfulJumpScore           { get => BitConverter.ToUInt16(Small, 0x208); set => SetData(Small, BitConverter.GetBytes(Math.Min(        9999, value)), 0x208); }

        public uint   JoyfulBerriesScore        { get => BitConverter.ToUInt16(Small, 0x20C); set => SetData(Small, BitConverter.GetBytes(Math.Min(        9999, value)), 0x20C); }
        public ushort JoyfulBerriesInRow        { get => BitConverter.ToUInt16(Small, 0x210); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0x210); }
        public ushort JoyfulBerries5InRow       { get => BitConverter.ToUInt16(Small, 0x212); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0x212); }

        public uint BP
        {
            get => BitConverter.ToUInt16(Small, 0xEB8);
            set
            {
                if (value > 9999)
                    value = 9999;
                BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xEB8);
            }
        }

        public uint BPEarned
        {
            get => BitConverter.ToUInt16(Small, 0xEBA);
            set
            {
                if (value > 65535)
                    value = 65535;
                BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xEBA);
            }
        }
        #endregion

        #region Large
        public override int PartyCount { get => Large[0x234]; protected set => Large[0x234] = (byte)value; }
        public override int GetPartyOffset(int slot) => 0x238 + (SIZE_PARTY * slot);

        public override uint Money
        {
            get => BitConverter.ToUInt32(Large, 0x0490) ^ SecurityKey;
            set => SetData(Large, BitConverter.GetBytes(value ^ SecurityKey), 0x0490);
        }

        public override uint Coin
        {
            get => (ushort)(BitConverter.ToUInt16(Large, 0x0494) ^ SecurityKey);
            set => SetData(Large, BitConverter.GetBytes((ushort)(value ^ SecurityKey)), 0x0494);
        }

        private const int OFS_PCItem = 0x0498;
        private const int OFS_PouchHeldItem = 0x0560;
        private const int OFS_PouchKeyItem = 0x05D8;
        private const int OFS_PouchBalls = 0x0650;
        private const int OFS_PouchTMHM = 0x0690;
        private const int OFS_PouchBerry = 0x0790;

        protected override InventoryPouch3[] GetItems()
        {
            const int max = 99;
            var PCItems = ArrayUtil.ConcatAll(Legal.Pouch_Items_RS, Legal.Pouch_Key_E, Legal.Pouch_Ball_RS, Legal.Pouch_TMHM_RS, Legal.Pouch_Berries_RS);
            return new InventoryPouch3[]
            {
                new(InventoryType.Items, Legal.Pouch_Items_RS, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
                new(InventoryType.KeyItems, Legal.Pouch_Key_E, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
                new(InventoryType.Balls, Legal.Pouch_Ball_RS, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
                new(InventoryType.TMHMs, Legal.Pouch_TMHM_RS, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
                new(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 46),
                new(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
            };
        }

        public PokeBlock3Case PokeBlocks
        {
            get => new(Large, 0x848);
            set => SetData(Large, value.Write(), 0x848);
        }

        protected override int SeenOffset2 => 0x988;

        public DecorationInventory3 Decorations
        {
            get => Large.Slice(0x2734, DecorationInventory3.SIZE).ToStructure<DecorationInventory3>();
            set => SetData(Large, value.ToBytes(), 0x2734);
        }

        protected override int MailOffset => 0x2BE0;

        protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(0, slot + 1) - 4; // @ end of each pkm slot
        public override string GetDaycareRNGSeed(int loc) => BitConverter.ToUInt32(Large, GetDaycareSlotOffset(0, 2)).ToString("X8");  // after the 2 slots, before the step counter
        public override void SetDaycareRNGSeed(int loc, string seed) => BitConverter.GetBytes(Util.GetHexValue(seed)).CopyTo(Large, GetDaycareEXPOffset(2));

        private const int ExternalEventFlags = 0x31C7;

        public bool HasReceivedWishmkrJirachi
        {
            get => GetFlag(ExternalEventFlags + 2, 0);
            set => SetFlag(ExternalEventFlags + 2, 0, value);
        }

        #region eBerry
        private const int OFFSET_EBERRY = 0x31F8;
        private const int SIZE_EBERRY = 0x134;

        public byte[] GetEReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
        public void SetEReaderBerry(byte[] data) => SetData(Large, data, OFFSET_EBERRY);

        public override string EBerryName => GetString(Large, OFFSET_EBERRY, 7);
        public override bool IsEBerryEngima => Large[OFFSET_EBERRY] is 0 or 0xFF;
        #endregion

        public int WonderOffset => WonderNewsOffset;
        private const int WonderNewsOffset = 0x322C;
        private const int WonderCardOffset = WonderNewsOffset + WonderNews3.SIZE;
        private const int WonderCardExtraOffset = WonderCardOffset + WonderCard3.SIZE;

        public WonderNews3 WonderNews { get => new(Large.Slice(WonderNewsOffset, WonderNews3.SIZE)); set => SetData(Large, value.Data, WonderOffset); }
        public WonderCard3 WonderCard { get => new(Large.Slice(WonderCardOffset, WonderCard3.SIZE)); set => SetData(Large, value.Data, WonderCardOffset); }
        public WonderCard3Extra WonderCardExtra { get => new(Large.Slice(WonderCardExtraOffset, WonderCard3Extra.SIZE)); set => SetData(Large, value.Data, WonderCardExtraOffset); }
        // 0x338: 4 easy chat words
        // 0x340: news MENewsJisanStruct
        // 0x344: uint[5], uint[5] tracking?

        public override MysteryEvent3 MysteryEvent
        {
            get => new(Large.Slice(0x3728, MysteryEvent3.SIZE));
            set => SetData(Large, value.Data, 0x3728);
        }

        protected override int SeenOffset3 => 0x3B24;
        #endregion

        private const uint EXTRADATA_SENTINEL = 0x0000B39D;
        private const int OFS_BV = 31 * 0x1000; // last sector of the save
        public bool HasBattleVideo => Data.Length > SaveUtil.SIZE_G3RAWHALF && BitConverter.ToUInt32(Data, OFS_BV) == EXTRADATA_SENTINEL;

        public BV3 BattleVideo
        {
            get => !HasBattleVideo ? new BV3() : new BV3(Data.Slice(OFS_BV + 4, BV3.SIZE));
            set => SetData(Data, value.Data, OFS_BV + 4);
        }
    }
}
