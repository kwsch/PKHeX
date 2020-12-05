using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SAV5B2W2 : SAV5, ISaveBlock5B2W2
    {
        public SAV5B2W2() : base(SaveUtil.SIZE_G5RAW)
        {
            Blocks = new SaveBlockAccessor5B2W2(this);
            Initialize();
        }

        public SAV5B2W2(byte[] data) : base(data)
        {
            Blocks = new SaveBlockAccessor5B2W2(this);
            Initialize();
        }

        public override PersonalTable Personal => PersonalTable.B2W2;
        public SaveBlockAccessor5B2W2 Blocks { get; }
        protected override SaveFile CloneInternal() => new SAV5B2W2((byte[]) Data.Clone());
        protected override int EventConstMax => 0x1AF; // this doesn't seem right?
        protected override int EventFlagMax => 0xBF8;
        public override int MaxItemID => Legal.MaxItemID_5_B2W2;

        private void Initialize()
        {
            BattleBoxOffset = 0x20900;
            EventConst = 0x1FF00;
            EventFlag = EventConst + 0x35E;
            CGearInfoOffset = 0x1C000;
            CGearDataOffset = 0x52800;
            EntreeForestOffset = 0x22A00;
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
        public PWTBlock5 PWT => Blocks.PWT;
    }
}