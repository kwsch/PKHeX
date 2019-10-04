namespace PKHeX.Core
{
    public sealed class SAV7USUM : SAV7
    {
        public SAV7USUM(byte[] data) : base(data, BlocksUSUM, boUU)
        {
            Initialize();
        }

        public SAV7USUM() : base(SaveUtil.SIZE_G7USUM, BlocksUSUM, boUU)
        {
            Initialize();
        }

        public override SaveFile Clone() => new SAV7USUM((byte[])Data.Clone());

        private void Initialize()
        {
            Personal = PersonalTable.USUM;
            HeldItems = Legal.HeldItems_USUM;

            Items = new MyItem7USUM(this, Bag);
            Zukan = new Zukan7(this, PokeDex, PokeDexLanguageFlags);
            Records = new Record6(this, Record, Core.Records.MaxType_USUM);
        }

        protected override int EventFlagMax => 4928;
        public override int MaxMoveID => Legal.MaxMoveID_7_USUM;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7_USUM;
        public override int MaxItemID => Legal.MaxItemID_7_USUM;
        public override int MaxAbilityID => Legal.MaxAbilityID_7_USUM;

        private const int boUU = SaveUtil.SIZE_G7USUM - 0x200;

        public static readonly BlockInfo[] BlocksUSUM =
        {
            new BlockInfo7(boUU, 00, 0x00000, 0x00E28),
            new BlockInfo7(boUU, 01, 0x01000, 0x0007C),
            new BlockInfo7(boUU, 02, 0x01200, 0x00014),
            new BlockInfo7(boUU, 03, 0x01400, 0x000C0),
            new BlockInfo7(boUU, 04, 0x01600, 0x0061C),
            new BlockInfo7(boUU, 05, 0x01E00, 0x00E00),
            new BlockInfo7(boUU, 06, 0x02C00, 0x00F78),
            new BlockInfo7(boUU, 07, 0x03C00, 0x00228),
            new BlockInfo7(boUU, 08, 0x04000, 0x0030C),
            new BlockInfo7(boUU, 09, 0x04400, 0x001FC),
            new BlockInfo7(boUU, 10, 0x04600, 0x0004C),
            new BlockInfo7(boUU, 11, 0x04800, 0x00004),
            new BlockInfo7(boUU, 12, 0x04A00, 0x00058),
            new BlockInfo7(boUU, 13, 0x04C00, 0x005E6),
            new BlockInfo7(boUU, 14, 0x05200, 0x36600),
            new BlockInfo7(boUU, 15, 0x3B800, 0x0572C),
            new BlockInfo7(boUU, 16, 0x41000, 0x00008),
            new BlockInfo7(boUU, 17, 0x41200, 0x01218),
            new BlockInfo7(boUU, 18, 0x42600, 0x01A08),
            new BlockInfo7(boUU, 19, 0x44200, 0x06408),
            new BlockInfo7(boUU, 20, 0x4A800, 0x06408),
            new BlockInfo7(boUU, 21, 0x50E00, 0x03998),
            new BlockInfo7(boUU, 22, 0x54800, 0x00100),
            new BlockInfo7(boUU, 23, 0x54A00, 0x00100),
            new BlockInfo7(boUU, 24, 0x54C00, 0x10528),
            new BlockInfo7(boUU, 25, 0x65200, 0x00204),
            new BlockInfo7(boUU, 26, 0x65600, 0x00B60),
            new BlockInfo7(boUU, 27, 0x66200, 0x03F50),
            new BlockInfo7(boUU, 28, 0x6A200, 0x00358),
            new BlockInfo7(boUU, 29, 0x6A600, 0x00728),
            new BlockInfo7(boUU, 30, 0x6AE00, 0x00200),
            new BlockInfo7(boUU, 31, 0x6B000, 0x00718),
            new BlockInfo7(boUU, 32, 0x6B800, 0x001FC),
            new BlockInfo7(boUU, 33, 0x6BA00, 0x00200),
            new BlockInfo7(boUU, 34, 0x6BC00, 0x00120),
            new BlockInfo7(boUU, 35, 0x6BE00, 0x001C8),
            new BlockInfo7(boUU, 36, 0x6C000, 0x00200),
            new BlockInfo7(boUU, 37, 0x6C200, 0x0039C),
            new BlockInfo7(boUU, 38, 0x6C600, 0x00400),
        };
    }
}