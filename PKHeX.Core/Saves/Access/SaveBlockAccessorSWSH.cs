using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessorSWSH : ISaveBlockAccessor<BlockInfo7b>, ISaveBlock8Main
    {
        public const int boGG = -1;

        private static readonly BlockInfo7b[] BlockInfoSWSH =
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

        public IReadOnlyList<BlockInfo7b> BlockInfo => BlockInfoSWSH;
        public MyItem Items { get; }
        public Record8 Records { get; }
        public PlayTime8 Played { get; }
        public MyStatus8 MyStatus { get; }
        public ConfigSave8 Config { get; }
        public GameTime8 GameTime { get; }
        public Misc8 MiscBlock { get; }
        public Zukan8 Zukan { get; }
        public EventWork8 EventWork { get; }
        public BoxLayout8 BoxLayout { get; }
        public Situation8 Situation { get; }
        public FieldMoveModelSave8 OverworldBlock { get; }

        public SaveBlockAccessorSWSH(SAV8SWSH sav)
        {
            const int langFlagStart = 0x550; // todo
            Items = new MyItem8(sav); // todo - at offset 0?
            Zukan = new Zukan8(sav, GetBlockOffset(SAV8BlockIndex.Pokedex), langFlagStart);
            MyStatus = new MyStatus8(sav, GetBlockOffset(SAV8BlockIndex.MyStatus));
            Played = new PlayTime8(sav, GetBlockOffset(SAV8BlockIndex.PlayTime));
            MiscBlock = new Misc8(sav, GetBlockOffset(SAV8BlockIndex.Misc));
            GameTime = new GameTime8(sav, GetBlockOffset(SAV8BlockIndex.GameTime));
            OverworldBlock = new FieldMoveModelSave8(sav, GetBlockOffset(SAV8BlockIndex.FieldMoveModelSave));
            Records = new Record8(sav, GetBlockOffset(SAV8BlockIndex.Records), Core.Records.MaxType_SWSH);
            Situation = new Situation8(sav, GetBlockOffset(SAV8BlockIndex.Situation));
            EventWork = new EventWork8(sav);
            BoxLayout = new BoxLayout8(sav, GetBlockOffset(SAV8BlockIndex.BOX));
            Config = new ConfigSave8(sav, GetBlockOffset(SAV8BlockIndex.ConfigSave));
        }

        public BlockInfo GetBlock(SAV8BlockIndex index) => BlockInfo[(int)index];
        public int GetBlockOffset(SAV8BlockIndex index) => GetBlock(index).Offset;
    }
}