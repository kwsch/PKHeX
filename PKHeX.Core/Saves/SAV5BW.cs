using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SAV5BW : SAV5
    {
        public SAV5BW() : base(SaveUtil.SIZE_G5RAW)
        {
            Blocks = new SaveBlockAccessor5BW(this);
            Initialize();
        }

        public SAV5BW(byte[] data) : base(data)
        {
            Blocks = new SaveBlockAccessor5BW(this);
            Initialize();
        }

        public override PersonalTable Personal => PersonalTable.BW;
        public SaveBlockAccessor5BW Blocks { get; }
        protected override SaveFile CloneInternal() => new SAV5BW((byte[])Data.Clone());
        protected override int EventConstMax => 0x13E;
        protected override int EventFlagMax => 0xB60;
        public override int MaxItemID => Legal.MaxItemID_5_BW;

        private void Initialize()
        {
            BattleBoxOffset = 0x20A00;
            EventConst = 0x20100;
            EventFlag = EventConst + 0x27C;
            CGearInfoOffset = 0x1C000;
            CGearDataOffset = 0x52000;
            EntreeForestOffset = 0x22C00;
            PokeDex = Blocks.Zukan.PokeDex;
            WondercardData = Blocks.Mystery.Offset;
            DaycareOffset = Blocks.Daycare.Offset;
        }

        public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
        public override MyItem Items => Blocks.Items;
        public override Zukan5 Zukan => Blocks.Zukan;
        public override Misc5 Misc => Blocks.Misc;
        public override MysteryBlock5 Mystery => Blocks.Mystery;
        public override Daycare5 Daycare => Blocks.Daycare;
        public override BoxLayout5 BoxLayout => Blocks.BoxLayout;
        public override PlayerData5 PlayerData => Blocks.PlayerData;
        public override BattleSubway5 BattleSubway => Blocks.BattleSubway;
    }
}