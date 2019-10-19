using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SaveBlockAccessor7USUM : ISaveBlock7USUM
    {
        public const int boUU = SaveUtil.SIZE_G7USUM - 0x200;

        private static readonly BlockInfo7[] BlockInfoUSUM =
        {
            new BlockInfo7(boUU, 00, 0x00000, 0x00E28), // 00 MyItem
            new BlockInfo7(boUU, 01, 0x01000, 0x0007C), // 01 Situation
            new BlockInfo7(boUU, 02, 0x01200, 0x00014), // 02 RandomGroup
            new BlockInfo7(boUU, 03, 0x01400, 0x000C0), // 03 MyStatus
            new BlockInfo7(boUU, 04, 0x01600, 0x0061C), // 04 PokePartySave
            new BlockInfo7(boUU, 05, 0x01E00, 0x00E00), // 05 EventWork
            new BlockInfo7(boUU, 06, 0x02C00, 0x00F78), // 06 ZukanData
            new BlockInfo7(boUU, 07, 0x03C00, 0x00228), // 07 GtsData
            new BlockInfo7(boUU, 08, 0x04000, 0x0030C), // 08 UnionPokemon
            new BlockInfo7(boUU, 09, 0x04400, 0x001FC), // 09 Misc
            new BlockInfo7(boUU, 10, 0x04600, 0x0004C), // 10 FieldMenu
            new BlockInfo7(boUU, 11, 0x04800, 0x00004), // 11 ConfigSave
            new BlockInfo7(boUU, 12, 0x04A00, 0x00058), // 12 GameTime
            new BlockInfo7(boUU, 13, 0x04C00, 0x005E6), // 13 BOX
            new BlockInfo7(boUU, 14, 0x05200, 0x36600), // 14 BoxPokemon
            new BlockInfo7(boUU, 15, 0x3B800, 0x0572C), // 15 ResortSave
            new BlockInfo7(boUU, 16, 0x41000, 0x00008), // 16 PlayTime
            new BlockInfo7(boUU, 17, 0x41200, 0x01218), // 17 FieldMoveModelSave
            new BlockInfo7(boUU, 18, 0x42600, 0x01A08), // 18 Fashion
            new BlockInfo7(boUU, 19, 0x44200, 0x06408), // 19 JoinFestaPersonalSave
            new BlockInfo7(boUU, 20, 0x4A800, 0x06408), // 20 JoinFestaPersonalSave
            new BlockInfo7(boUU, 21, 0x50E00, 0x03998), // 21 JoinFestaDataSave
            new BlockInfo7(boUU, 22, 0x54800, 0x00100), // 22 BerrySpot
            new BlockInfo7(boUU, 23, 0x54A00, 0x00100), // 23 FishingSpot
            new BlockInfo7(boUU, 24, 0x54C00, 0x10528), // 24 LiveMatchData
            new BlockInfo7(boUU, 25, 0x65200, 0x00204), // 25 BattleSpotData
            new BlockInfo7(boUU, 26, 0x65600, 0x00B60), // 26 PokeFinderSave
            new BlockInfo7(boUU, 27, 0x66200, 0x03F50), // 27 MysteryGiftSave
            new BlockInfo7(boUU, 28, 0x6A200, 0x00358), // 28 Record
            new BlockInfo7(boUU, 29, 0x6A600, 0x00728), // 29 ValidationSave
            new BlockInfo7(boUU, 30, 0x6AE00, 0x00200), // 30 GameSyncSave
            new BlockInfo7(boUU, 31, 0x6B000, 0x00718), // 31 PokeDiarySave
            new BlockInfo7(boUU, 32, 0x6B800, 0x001FC), // 32 BattleInstSave
            new BlockInfo7(boUU, 33, 0x6BA00, 0x00200), // 33 Sodateya
            new BlockInfo7(boUU, 34, 0x6BC00, 0x00120), // 34 WeatherSave
            new BlockInfo7(boUU, 35, 0x6BE00, 0x001C8), // 35 QRReaderSaveData
            new BlockInfo7(boUU, 36, 0x6C000, 0x00200), // 36 TurtleSalmonSave
            new BlockInfo7(boUU, 37, 0x6C200, 0x0039C), // 37 BattleFesSave
            new BlockInfo7(boUU, 38, 0x6C600, 0x00400), // 38 FinderStudioSave
        };

        public SaveBlockAccessor7USUM(SAV7USUM sav)
        {
            var bi = BlockInfo;

            Items = new MyItem7USUM(sav, 0);
            Situation = new Situation7(sav, bi[01].Offset);
            MyStatus = new MyStatus7(sav, bi[03].Offset);
            Zukan = new Zukan7(sav, bi[06].Offset, 0x550);
            Misc = new Misc7(sav, bi[09].Offset);
            FieldMenu = new FieldMenu7(sav, bi[10].Offset);
            Config = new ConfigSave7(sav, bi[11].Offset);
            GameTime = new GameTime7(sav, bi[12].Offset);
            BoxLayout = new BoxLayout7(sav, bi[13].Offset);
            ResortSave = new ResortSave7(sav, bi[15].Offset);
            Played = new PlayTime6(sav, bi[16].Offset);
            Overworld = new FieldMoveModelSave7(sav, bi[17].Offset);
            Fashion = new FashionBlock7(sav, bi[18].Offset);
            Festa = new JoinFesta7(sav, bi[21].Offset);
            PokeFinder = new PokeFinder7(sav, bi[26].Offset);
            MysteryGift = new MysteryBlock7(sav, bi[27].Offset);
            Records = new RecordBlock6(sav, bi[28].Offset);
            BattleTree = new BattleTree7(sav, bi[32].Offset);
            Daycare = new Daycare7(sav, bi[33].Offset);
        }

        public IReadOnlyList<BlockInfo7> BlockInfo => BlockInfoUSUM;

        public MyItem Items { get; }
        public MysteryBlock7 MysteryGift { get; }
        public PokeFinder7 PokeFinder { get; }
        public JoinFesta7 Festa { get; }
        public Daycare7 Daycare { get; }
        public RecordBlock6 Records { get; }
        public PlayTime6 Played { get; }
        public MyStatus7 MyStatus { get; }
        public FieldMoveModelSave7 Overworld { get; }
        public Situation7 Situation { get; }
        public ConfigSave7 Config { get; }
        public GameTime7 GameTime { get; }
        public Misc7 Misc { get; }
        public Zukan7 Zukan { get; }
        public BoxLayout7 BoxLayout { get; }
        public BattleTree7 BattleTree { get; }
        public ResortSave7 ResortSave { get; }
        public FieldMenu7 FieldMenu { get; }
        public FashionBlock7 Fashion { get; }
    }
}