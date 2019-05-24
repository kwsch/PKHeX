namespace PKHeX.Core
{
    public class SAV5B2W2 : SAV5
    {
        public SAV5B2W2() : base(SaveUtil.SIZE_G5RAW) => Initialize();
        public SAV5B2W2(byte[] data) : base(data) => Initialize();
        public override SaveFile Clone() => new SAV5B2W2((byte[])Data.Clone()) { Footer = (byte[])Footer.Clone() };
        protected override int EventConstMax => 0x1AF; // this doesn't seem right?
        protected override int EventFlagMax => 0xBF8;
        public override int MaxItemID => Legal.MaxItemID_5_B2W2;

        private void Initialize()
        {
            Blocks = BlockInfoNDS.BlocksB2W2;
            Personal = PersonalTable.B2W2;

            Items = new MyItem5B2W2(this, 0x18400);
            BattleBox = 0x20900;
            Trainer2 = 0x21100;
            EventConst = 0x1FF00;
            EventFlag = EventConst + 0x35E;
            Daycare = 0x20D00;
            PokeDex = 0x21400;
            PokeDexLanguageFlags = PokeDex + 0x328; // forme flags size is + 8 from bw with new formes (therians)
            BattleSubway = 0x21B00;
            CGearInfoOffset = 0x1C000;
            CGearDataOffset = 0x52800;
            EntreeForestOffset = 0x22A00;
            Zukan = new Zukan5(this, PokeDex, PokeDexLanguageFlags);
            DaycareBlock = new Daycare5(this, Daycare);

            MiscBlock = new Misc5(this, Trainer2);
            PWTBlock = new PWTBlock5(this, 0x23700);
            DaycareBlock = new Daycare5(this, Daycare);
            BattleSubwayBlock = new BattleSubway5(this, BattleSubway);
        }

        public PWTBlock5 PWTBlock { get; private set; }
    }
}