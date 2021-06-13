using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Information for Accessing individual blocks within a <see cref="SAV6AO"/>.
    /// </summary>
    public sealed class SaveBlockAccessor6AO : ISaveBlockAccessor<BlockInfo6>, ISaveBlock6Main
    {
        public const int BlockMetadataOffset = SaveUtil.SIZE_G6ORAS - 0x200;
        private const int boAO = BlockMetadataOffset;

        private static readonly BlockInfo6[] BlocksAO =
        {
            new(boAO, 00, 0x00000, 0x002C8), // 00 Puff
            new(boAO, 01, 0x00400, 0x00B90), // 01 MyItem
            new(boAO, 02, 0x01000, 0x0002C), // 02 ItemInfo (Select Bound Items)
            new(boAO, 03, 0x01200, 0x00038), // 03 GameTime
            new(boAO, 04, 0x01400, 0x00150), // 04 Situation
            new(boAO, 05, 0x01600, 0x00004), // 05 RandomGroup (rand seeds)
            new(boAO, 06, 0x01800, 0x00008), // 06 PlayTime
            new(boAO, 07, 0x01A00, 0x001C0), // 07 Fashion
            new(boAO, 08, 0x01C00, 0x000BE), // 08 Amie minigame records
            new(boAO, 09, 0x01E00, 0x00024), // 09 temp variables (u32 id + 32 u8)
            new(boAO, 10, 0x02000, 0x02100), // 10 FieldMoveModelSave
            new(boAO, 11, 0x04200, 0x00130), // 11 Misc
            new(boAO, 12, 0x04400, 0x00440), // 12 BOX
            new(boAO, 13, 0x04A00, 0x00574), // 13 BattleBox
            new(boAO, 14, 0x05000, 0x04E28), // 14 PSS1
            new(boAO, 15, 0x0A000, 0x04E28), // 15 PSS2
            new(boAO, 16, 0x0F000, 0x04E28), // 16 PSS3
            new(boAO, 17, 0x14000, 0x00170), // 17 MyStatus
            new(boAO, 18, 0x14200, 0x0061C), // 18 PokePartySave
            new(boAO, 19, 0x14A00, 0x00504), // 19 EventWork
            new(boAO, 20, 0x15000, 0x011CC), // 20 ZukanData
            new(boAO, 21, 0x16200, 0x00644), // 21 hologram clips
            new(boAO, 22, 0x16A00, 0x00104), // 22 UnionPokemon (Fused)
            new(boAO, 23, 0x16C00, 0x00004), // 23 ConfigSave
            new(boAO, 24, 0x16E00, 0x00420), // 24 Amie decoration stuff
            new(boAO, 25, 0x17400, 0x00064), // 25 OPower
            new(boAO, 26, 0x17600, 0x003F0), // 26 Strength Rock position (xyz float: 84 entries, 12bytes/entry)
            new(boAO, 27, 0x17A00, 0x0070C), // 27 Trainer PR Video
            new(boAO, 28, 0x18200, 0x00180), // 28 GtsData
            new(boAO, 29, 0x18400, 0x00004), // 29 Packed Menu Bits
            new(boAO, 30, 0x18600, 0x0000C), // 30 PSS Profile Q&A (6*questions, 6*answer)
            new(boAO, 31, 0x18800, 0x00048), // 31 Repel Info, (Swarm?) and other overworld info (roamer)
            new(boAO, 32, 0x18A00, 0x00054), // 32 BOSS data fetch history (serial/mystery gift), 4byte intro & 20*4byte entries
            new(boAO, 33, 0x18C00, 0x00644), // 33 Streetpass history
            new(boAO, 34, 0x19400, 0x005C8), // 34 LiveMatchData/BattleSpotData
            new(boAO, 35, 0x19A00, 0x002F8), // 35 MAC Address & Network Connection Logging (0x98 per entry, 5 entries)
            new(boAO, 36, 0x19E00, 0x01B40), // 36 Dendou (Hall of Fame)
            new(boAO, 37, 0x1BA00, 0x001F4), // 37 BattleHouse (Maison)
            new(boAO, 38, 0x1BC00, 0x003E0), // 38 Sodateya (Daycare)
            new(boAO, 39, 0x1C000, 0x00216), // 39 TrialHouse (Battle Institute)
            new(boAO, 40, 0x1C400, 0x00640), // 40 BerryField
            new(boAO, 41, 0x1CC00, 0x01A90), // 41 MysteryGiftSave
            new(boAO, 42, 0x1E800, 0x00400), // 42 [SubE]vent Log
            new(boAO, 43, 0x1EC00, 0x00618), // 43 PokeDiarySave
            new(boAO, 44, 0x1F400, 0x0025C), // 44 Record
            new(boAO, 45, 0x1F800, 0x00834), // 45 Friend Safari (0x15 per entry, 100 entries)
            new(boAO, 46, 0x20200, 0x00318), // 46 SuperTrain
            new(boAO, 47, 0x20600, 0x007D0), // 47 Unused (lmao)
            new(boAO, 48, 0x20E00, 0x00C48), // 48 LinkInfo
            new(boAO, 49, 0x21C00, 0x00078), // 49 PSS usage info
            new(boAO, 50, 0x21E00, 0x00200), // 50 GameSyncSave
            new(boAO, 51, 0x22000, 0x00C84), // 51 PSS Icon (bool32 data present, 40x40 u16 pic, unused)
            new(boAO, 52, 0x22E00, 0x00628), // 52 ValidationSave (updateable Public Key for legal check api calls)
            new(boAO, 53, 0x23600, 0x00400), // 53 Contest
            new(boAO, 54, 0x23A00, 0x07AD0), // 54 SecretBase
            new(boAO, 55, 0x2B600, 0x078B0), // 55 EonTicket
            new(boAO, 56, 0x33000, 0x34AD0), // 56 Box
            new(boAO, 57, 0x67C00, 0x0E058), // 57 JPEG
        };

        public IReadOnlyList<BlockInfo6> BlockInfo => BlocksAO;
        public MyItem Items { get; }
        public ItemInfo6 ItemInfo { get; }
        public GameTime6 GameTime { get; }
        public Situation6 Situation { get; }
        public PlayTime6 Played { get; }
        public MyStatus6 Status { get; }
        public RecordBlock6 Records { get; }

        public Zukan6AO Zukan { get; }
        public Puff6 Puff { get; }
        public BoxLayout6 BoxLayout { get; }
        public BattleBox6 BattleBox { get; }
        public OPower6 OPower { get; }
        public MysteryBlock6 MysteryGift { get; }
        public SangoInfoBlock Sango { get; }
        public LinkBlock6 Link { get; }
        public Misc6AO Misc { get; }
        public SuperTrainBlock SuperTrain { get; }
        public MaisonBlock Maison { get; }
        public SubEventLog6 SUBE { get; }
        public ConfigSave6 Config { get; }
        public Encount6 Encount { get; }
        public SecretBase6Block SecretBase { get; }

        public SaveBlockAccessor6AO(SAV6AO sav)
        {
            Puff = new Puff6(sav, 0x0000);
            Items = new MyItem6AO(sav, 0x00400);
            ItemInfo = new ItemInfo6(sav, 0x1000);
            GameTime = new GameTime6(sav, 0x01200);
            Situation = new Situation6(sav, 0x01400);
            Played = new PlayTime6(sav, 0x01800);
            Misc = new Misc6AO(sav, 0x04200);
            BoxLayout = new BoxLayout6(sav, 0x04400);
            BattleBox = new BattleBox6(sav, 0x04A00);
            Status = new MyStatus6(sav, 0x14000);
            Zukan = new Zukan6AO(sav, 0x15000, 0x400);
            Config = new ConfigSave6(sav, 0x16C00);
            OPower = new OPower6(sav, 0x17400);
            Encount = new Encount6(sav, 0x18800);
            Maison = new MaisonBlock(sav, 0x1BA00);
            MysteryGift = new MysteryBlock6(sav, 0x1CC00);
            SUBE = new SubEventLog6AO(sav, 0x1E800);
            Records = new RecordBlock6(sav, 0x1F400);
            SuperTrain = new SuperTrainBlock(sav, 0x20200);
            Link = new LinkBlock6(sav, 0x20E00);
            SecretBase = new SecretBase6Block(sav, 0x23A00);
            Sango = new SangoInfoBlock(sav, 0x2B600);
        }
    }
}
