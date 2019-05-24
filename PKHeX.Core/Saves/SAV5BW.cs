namespace PKHeX.Core
{
    public class SAV5BW : SAV5
    {
        public SAV5BW() : base(SaveUtil.SIZE_G5RAW) => Initialize();
        public SAV5BW(byte[] data) : base(data) => Initialize();
        public override SaveFile Clone() => new SAV5BW((byte[])Data.Clone()) { Footer = (byte[])Footer.Clone() };
        protected override int EventConstMax => 0x13E;
        protected override int EventFlagMax => 0xB60;
        public override int MaxItemID => Legal.MaxItemID_5_BW;

        private void Initialize()
        {
            Blocks = BlockInfoNDS.BlocksBW;
            Personal = PersonalTable.BW;

            Items = new MyItem5BW(this, 0x18400);

            BattleBox = 0x20A00;
            Trainer2 = 0x21200;
            EventConst = 0x20100;
            EventFlag = EventConst + 0x27C;
            Daycare = 0x20E00;
            PokeDex = 0x21600;
            PokeDexLanguageFlags = PokeDex + 0x320;
            BattleSubway = 0x21D00;
            CGearInfoOffset = 0x1C000;
            CGearDataOffset = 0x52000;
            EntreeForestOffset = 0x22C00;
            MiscBlock = new Misc5(this, Trainer2);
            Zukan = new Zukan5(this, PokeDex, PokeDexLanguageFlags);
            DaycareBlock = new Daycare5(this, Daycare);
            BattleSubwayBlock = new BattleSubway5(this, BattleSubway);
            // Inventory offsets are the same for each game.
        }
    }
}