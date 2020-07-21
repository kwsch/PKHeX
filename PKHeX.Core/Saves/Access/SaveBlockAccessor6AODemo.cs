using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessor6AODemo : ISaveBlockAccessor<BlockInfo6>, ISaveBlock6Core
    {
        public const int boAOdemo = SaveUtil.SIZE_G6ORASDEMO - 0x200;

        private static readonly BlockInfo6[] BlocksAODemo =
        {
            new BlockInfo6(boAOdemo, 00, 0x00000, 0x00B90), // MyItem
            new BlockInfo6(boAOdemo, 01, 0x00C00, 0x0002C), // ItemInfo6
            new BlockInfo6(boAOdemo, 02, 0x00E00, 0x00038), // GameTime
            new BlockInfo6(boAOdemo, 03, 0x01000, 0x00150), // Situation
            new BlockInfo6(boAOdemo, 04, 0x01200, 0x00004), // RandomGroup (rand seeds)
            new BlockInfo6(boAOdemo, 05, 0x01400, 0x00008), // PlayTime
            new BlockInfo6(boAOdemo, 06, 0x01600, 0x00024), // temp variables (u32 id + 32 u8)
            new BlockInfo6(boAOdemo, 07, 0x01800, 0x02100), // FieldMoveModelSave
            new BlockInfo6(boAOdemo, 08, 0x03A00, 0x00130), // Misc
            new BlockInfo6(boAOdemo, 09, 0x03C00, 0x00170), // MyStatus
            new BlockInfo6(boAOdemo, 10, 0x03E00, 0x0061C), // PokePartySave
            new BlockInfo6(boAOdemo, 11, 0x04600, 0x00504), // EventWork
            new BlockInfo6(boAOdemo, 12, 0x04C00, 0x00004), // Packed Menu Bits
            new BlockInfo6(boAOdemo, 13, 0x04E00, 0x00048), // Repel Info, (Swarm?) and other overworld info (roamer)
            new BlockInfo6(boAOdemo, 14, 0x05000, 0x00400), // PokeDiarySave
            new BlockInfo6(boAOdemo, 15, 0x05400, 0x0025C), // Record
        };

        public IReadOnlyList<BlockInfo6> BlockInfo => BlocksAODemo;
        public MyItem Items { get; }
        public ItemInfo6 ItemInfo { get; }
        public GameTime6 GameTime { get; }
        public Situation6 Situation { get; }
        public PlayTime6 Played { get; }
        public MyStatus6 Status { get; }
        public RecordBlock6 Records { get; }
        public Misc6AO Misc { get; }

        public SaveBlockAccessor6AODemo(SAV6AODemo sav)
        {
            Items = new MyItem6AO(sav, 0x00000);
            ItemInfo = new ItemInfo6(sav, 0x00C00);
            GameTime = new GameTime6(sav, 0x00E00);
            Situation = new Situation6(sav, 0x01000);
            Played = new PlayTime6(sav, 0x01400);
            Status = new MyStatus6(sav, 0x03C00);
            Records = new RecordBlock6(sav, 0x05400);
            Misc = new Misc6AO(sav, 0x03A00);
        }
    }
}