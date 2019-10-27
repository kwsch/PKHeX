using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessorXY : ISaveBlockAccessor<BlockInfo6>, ISaveBlock6XY
    {
        public const int boXY = SaveUtil.SIZE_G6XY - 0x200;

        public static readonly BlockInfo6[] BlocksXY =
        {
            new BlockInfo6(boXY, 00, 0x00000, 0x002C8),
            new BlockInfo6(boXY, 01, 0x00400, 0x00B88),
            new BlockInfo6(boXY, 02, 0x01000, 0x0002C),
            new BlockInfo6(boXY, 03, 0x01200, 0x00038),
            new BlockInfo6(boXY, 04, 0x01400, 0x00150),
            new BlockInfo6(boXY, 05, 0x01600, 0x00004),
            new BlockInfo6(boXY, 06, 0x01800, 0x00008),
            new BlockInfo6(boXY, 07, 0x01A00, 0x001C0),
            new BlockInfo6(boXY, 08, 0x01C00, 0x000BE),
            new BlockInfo6(boXY, 09, 0x01E00, 0x00024),
            new BlockInfo6(boXY, 10, 0x02000, 0x02100),
            new BlockInfo6(boXY, 11, 0x04200, 0x00140),
            new BlockInfo6(boXY, 12, 0x04400, 0x00440),
            new BlockInfo6(boXY, 13, 0x04A00, 0x00574),
            new BlockInfo6(boXY, 14, 0x05000, 0x04E28),
            new BlockInfo6(boXY, 15, 0x0A000, 0x04E28),
            new BlockInfo6(boXY, 16, 0x0F000, 0x04E28),
            new BlockInfo6(boXY, 17, 0x14000, 0x00170),
            new BlockInfo6(boXY, 18, 0x14200, 0x0061C),
            new BlockInfo6(boXY, 19, 0x14A00, 0x00504),
            new BlockInfo6(boXY, 20, 0x15000, 0x006A0),
            new BlockInfo6(boXY, 21, 0x15800, 0x00644),
            new BlockInfo6(boXY, 22, 0x16000, 0x00104),
            new BlockInfo6(boXY, 23, 0x16200, 0x00004),
            new BlockInfo6(boXY, 24, 0x16400, 0x00420),
            new BlockInfo6(boXY, 25, 0x16A00, 0x00064),
            new BlockInfo6(boXY, 26, 0x16C00, 0x003F0),
            new BlockInfo6(boXY, 27, 0x17000, 0x0070C),
            new BlockInfo6(boXY, 28, 0x17800, 0x00180),
            new BlockInfo6(boXY, 29, 0x17A00, 0x00004),
            new BlockInfo6(boXY, 30, 0x17C00, 0x0000C),
            new BlockInfo6(boXY, 31, 0x17E00, 0x00048),
            new BlockInfo6(boXY, 32, 0x18000, 0x00054),
            new BlockInfo6(boXY, 33, 0x18200, 0x00644),
            new BlockInfo6(boXY, 34, 0x18A00, 0x005C8),
            new BlockInfo6(boXY, 35, 0x19000, 0x002F8),
            new BlockInfo6(boXY, 36, 0x19400, 0x01B40),
            new BlockInfo6(boXY, 37, 0x1B000, 0x001F4),
            new BlockInfo6(boXY, 38, 0x1B200, 0x001F0),
            new BlockInfo6(boXY, 39, 0x1B400, 0x00216),
            new BlockInfo6(boXY, 40, 0x1B800, 0x00390),
            new BlockInfo6(boXY, 41, 0x1BC00, 0x01A90),
            new BlockInfo6(boXY, 42, 0x1D800, 0x00308),
            new BlockInfo6(boXY, 43, 0x1DC00, 0x00618),
            new BlockInfo6(boXY, 44, 0x1E400, 0x0025C),
            new BlockInfo6(boXY, 45, 0x1E800, 0x00834),
            new BlockInfo6(boXY, 46, 0x1F200, 0x00318),
            new BlockInfo6(boXY, 47, 0x1F600, 0x007D0),
            new BlockInfo6(boXY, 48, 0x1FE00, 0x00C48),
            new BlockInfo6(boXY, 49, 0x20C00, 0x00078),
            new BlockInfo6(boXY, 50, 0x20E00, 0x00200),
            new BlockInfo6(boXY, 51, 0x21000, 0x00C84),
            new BlockInfo6(boXY, 52, 0x21E00, 0x00628),
            new BlockInfo6(boXY, 53, 0x22600, 0x34AD0),
            new BlockInfo6(boXY, 54, 0x57200, 0x0E058),
        };

        public IReadOnlyList<BlockInfo6> BlockInfo => BlocksXY;
        public MyItem Items { get; }
        public ItemInfo6 ItemInfo { get; }
        public GameTime6 GameTime { get; }
        public Situation6 Situation { get; }
        public PlayTime6 Played { get; }
        public MyStatus6 Status { get; }
        public RecordBlock6 Records { get; }

        public SaveBlockAccessorXY(SAV6XY sav)
        {
            Puff = new Puff6(sav, 0x00000);
            Items = new MyItem6XY(sav, 0x00400);
            ItemInfo = new ItemInfo6(sav, 0x01000);
            GameTime = new GameTime6(sav, 0x01200);
            Situation = new Situation6(sav, 0x01400);
            Played = new PlayTime6(sav, 0x01800);
            Fashion = new Fashion6XY(sav, 0x1A00);
            Misc = new Misc6XY(sav, 0x4200);
            BoxLayout = new BoxLayout6(sav, 0x4400);
            BattleBox = new BattleBox6(sav, 0x04A00);
            Status = new MyStatus6XY(sav, 0x14000);
            Zukan = new Zukan6XY(sav, 0x15000, 0x3C8);
            OPower = new OPower6(sav, 0x16A00);
            MysteryGift = new MysteryBlock6(sav, 0x1BC00);
            Records = new RecordBlock6(sav, 0x1E400);
            SuperTrain = new SuperTrainBlock(sav, 0x1F200);
            Link = new LinkBlock6(sav, 0x1FE00);
            Maison = new MaisonBlock(sav, 0x1B000);
        }

        public Puff6 Puff { get; }
        public BoxLayout6 BoxLayout { get; }
        public BattleBox6 BattleBox { get; }
        public OPower6 OPower { get; }
        public MysteryBlock6 MysteryGift { get; }
        public LinkBlock6 Link { get; }
        public SuperTrainBlock SuperTrain { get; }
        public MaisonBlock Maison { get; }

        public Misc6XY Misc { get; }
        public Zukan6XY Zukan { get; }
        public Fashion6XY Fashion { get; }
    }
}