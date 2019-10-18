using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessor7b : ISaveBlockAccessor<BlockInfo7b>
    {
        private const int boGG = 0xB8800 - 0x200; // nowhere near 1MB (savedata.bin size)

        private static readonly BlockInfo7b[] BlockInfoGG =
        {
            new BlockInfo7b(boGG, 00, 0x00000, 0x00D90),
            new BlockInfo7b(boGG, 01, 0x00E00, 0x00200),
            new BlockInfo7b(boGG, 02, 0x01000, 0x00168),
            new BlockInfo7b(boGG, 03, 0x01200, 0x01800),
            new BlockInfo7b(boGG, 04, 0x02A00, 0x020E8),
            new BlockInfo7b(boGG, 05, 0x04C00, 0x00930),
            new BlockInfo7b(boGG, 06, 0x05600, 0x00004),
            new BlockInfo7b(boGG, 07, 0x05800, 0x00130),
            new BlockInfo7b(boGG, 08, 0x05A00, 0x00012),
            new BlockInfo7b(boGG, 09, 0x05C00, 0x3F7A0),
            new BlockInfo7b(boGG, 10, 0x45400, 0x00008),
            new BlockInfo7b(boGG, 11, 0x45600, 0x00E90),
            new BlockInfo7b(boGG, 12, 0x46600, 0x010A4),
            new BlockInfo7b(boGG, 13, 0x47800, 0x000F0),
            new BlockInfo7b(boGG, 14, 0x47A00, 0x06010),
            new BlockInfo7b(boGG, 15, 0x4DC00, 0x00200),
            new BlockInfo7b(boGG, 16, 0x4DE00, 0x00098),
            new BlockInfo7b(boGG, 17, 0x4E000, 0x00068),
            new BlockInfo7b(boGG, 18, 0x4E200, 0x69780),
            new BlockInfo7b(boGG, 19, 0xB7A00, 0x000B0),
            new BlockInfo7b(boGG, 20, 0xB7C00, 0x00940),
        };

        public IReadOnlyList<BlockInfo7b> BlockInfo => BlockInfoGG;

        public SaveBlockAccessor7b(SAV7b sav)
        {
            Zukan = new Zukan7b(sav, GetBlockOffset(BelugaBlockIndex.Zukan), 0x550);
            Config = new ConfigSave7b(sav, GetBlockOffset(BelugaBlockIndex.ConfigSave));
            Items = new MyItem7b(sav, GetBlockOffset(BelugaBlockIndex.MyItem));
            Storage = new PokeListHeader(sav, GetBlockOffset(BelugaBlockIndex.PokeListHeader));
            Status = new MyStatus7b(sav, GetBlockOffset(BelugaBlockIndex.MyStatus));
            Played = new PlayTime7b(sav, GetBlockOffset(BelugaBlockIndex.PlayTime));
            Misc = new Misc7b(sav, GetBlockOffset(BelugaBlockIndex.Misc));
            EventWork = new EventWork7b(sav, GetBlockOffset(BelugaBlockIndex.EventWork));
            GiftRecords = new WB7Records(sav, GetBlockOffset(BelugaBlockIndex.WB7Record));
        }

        public readonly MyItem Items;
        public readonly Misc7b Misc;
        public readonly Zukan7b Zukan;
        public readonly MyStatus7b Status;
        public readonly PlayTime7b Played;
        public readonly ConfigSave7b Config;
        public readonly EventWork7b EventWork;
        public readonly PokeListHeader Storage;
        public readonly WB7Records GiftRecords;
        public BlockInfo GetBlock(BelugaBlockIndex index) => BlockInfo[(int)index];
        public int GetBlockOffset(BelugaBlockIndex index) => GetBlock(index).Offset;
    }
}