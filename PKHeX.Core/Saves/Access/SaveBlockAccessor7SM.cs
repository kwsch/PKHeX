using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Information for Accessing individual blocks within a <see cref="SAV7SM"/>.
    /// </summary>
    public sealed class SaveBlockAccessor7SM : ISaveBlockAccessor<BlockInfo7>, ISaveBlock7SM
    {
        public const int BlockMetadataOffset = SaveUtil.SIZE_G7SM - 0x200;
        private const int boSM = BlockMetadataOffset;

        private static readonly BlockInfo7[] BlockInfoSM =
        {
            new(boSM, 00, 0x00000, 0x00DE0), // 00 MyItem
            new(boSM, 01, 0x00E00, 0x0007C), // 01 Situation
            new(boSM, 02, 0x01000, 0x00014), // 02 RandomGroup
            new(boSM, 03, 0x01200, 0x000C0), // 03 MyStatus
            new(boSM, 04, 0x01400, 0x0061C), // 04 PokePartySave
            new(boSM, 05, 0x01C00, 0x00E00), // 05 EventWork
            new(boSM, 06, 0x02A00, 0x00F78), // 06 ZukanData
            new(boSM, 07, 0x03A00, 0x00228), // 07 GtsData
            new(boSM, 08, 0x03E00, 0x00104), // 08 UnionPokemon
            new(boSM, 09, 0x04000, 0x00200), // 09 Misc
            new(boSM, 10, 0x04200, 0x00020), // 10 FieldMenu
            new(boSM, 11, 0x04400, 0x00004), // 11 ConfigSave
            new(boSM, 12, 0x04600, 0x00058), // 12 GameTime
            new(boSM, 13, 0x04800, 0x005E6), // 13 BOX
            new(boSM, 14, 0x04E00, 0x36600), // 14 BoxPokemon
            new(boSM, 15, 0x3B400, 0x0572C), // 15 ResortSave
            new(boSM, 16, 0x40C00, 0x00008), // 16 PlayTime
            new(boSM, 17, 0x40E00, 0x01080), // 17 FieldMoveModelSave
            new(boSM, 18, 0x42000, 0x01A08), // 18 Fashion
            new(boSM, 19, 0x43C00, 0x06408), // 19 JoinFestaPersonalSave
            new(boSM, 20, 0x4A200, 0x06408), // 20 JoinFestaPersonalSave
            new(boSM, 21, 0x50800, 0x03998), // 21 JoinFestaDataSave
            new(boSM, 22, 0x54200, 0x00100), // 22 BerrySpot
            new(boSM, 23, 0x54400, 0x00100), // 23 FishingSpot
            new(boSM, 24, 0x54600, 0x10528), // 24 LiveMatchData
            new(boSM, 25, 0x64C00, 0x00204), // 25 BattleSpotData
            new(boSM, 26, 0x65000, 0x00B60), // 26 PokeFinderSave
            new(boSM, 27, 0x65C00, 0x03F50), // 27 MysteryGiftSave
            new(boSM, 28, 0x69C00, 0x00358), // 28 Record
            new(boSM, 29, 0x6A000, 0x00728), // 29 ValidationSave
            new(boSM, 30, 0x6A800, 0x00200), // 30 GameSyncSave
            new(boSM, 31, 0x6AA00, 0x00718), // 31 PokeDiarySave
            new(boSM, 32, 0x6B200, 0x001FC), // 32 BattleInstSave
            new(boSM, 33, 0x6B400, 0x00200), // 33 Sodateya
            new(boSM, 34, 0x6B600, 0x00120), // 34 WeatherSave
            new(boSM, 35, 0x6B800, 0x001C8), // 35 QRReaderSaveData
            new(boSM, 36, 0x6BA00, 0x00200), // 36 TurtleSalmonSave
        };

        public SaveBlockAccessor7SM(SAV7SM sav)
        {
            var bi = BlockInfo;

            Items = new MyItem7SM(sav, 0);
            Situation = new Situation7(sav, bi[01].Offset);
            MyStatus = new MyStatus7(sav, bi[03].Offset);
            Fame = new HallOfFame7(sav, bi[05].Offset + 0x9C4);
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

        public IReadOnlyList<BlockInfo7> BlockInfo => BlockInfoSM;

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
        public HallOfFame7 Fame { get; }
    }
}
