using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV6XY"/>.
/// </summary>
public sealed class SaveBlockAccessor6XY(SAV6XY sav) : ISaveBlockAccessor<BlockInfo6>, ISaveBlock6XY
{
    public const int BlockMetadataOffset = SaveUtil.SIZE_G6XY - 0x200;
    private const int boXY = BlockMetadataOffset;

    private static readonly BlockInfo6[] BlocksXY =
    [
        new(boXY, 00, 0x00000, 0x002C8), // 00 Puff
        new(boXY, 01, 0x00400, 0x00B88), // 01 MyItem
        new(boXY, 02, 0x01000, 0x0002C), // 02 ItemInfo (Select Bound Items)
        new(boXY, 03, 0x01200, 0x00038), // 03 GameTime
        new(boXY, 04, 0x01400, 0x00150), // 04 Situation
        new(boXY, 05, 0x01600, 0x00004), // 05 RandomGroup (rand seeds)
        new(boXY, 06, 0x01800, 0x00008), // 06 PlayTime
        new(boXY, 07, 0x01A00, 0x001C0), // 07 Fashion
        new(boXY, 08, 0x01C00, 0x000BE), // 08 Amie minigame records
        new(boXY, 09, 0x01E00, 0x00024), // 09 temp variables (u32 id + 32 u8)
        new(boXY, 10, 0x02000, 0x02100), // 10 FieldMoveModelSave
        new(boXY, 11, 0x04200, 0x00140), // 11 Misc
        new(boXY, 12, 0x04400, 0x00440), // 12 BOX
        new(boXY, 13, 0x04A00, 0x00574), // 13 BattleBox
        new(boXY, 14, 0x05000, 0x04E28), // 14 PSS1
        new(boXY, 15, 0x0A000, 0x04E28), // 15 PSS2
        new(boXY, 16, 0x0F000, 0x04E28), // 16 PSS3
        new(boXY, 17, 0x14000, 0x00170), // 17 MyStatus
        new(boXY, 18, 0x14200, 0x0061C), // 18 PokePartySave
        new(boXY, 19, 0x14A00, 0x00504), // 19 EventWork
        new(boXY, 20, 0x15000, 0x006A0), // 20 ZukanData
        new(boXY, 21, 0x15800, 0x00644), // 21 hologram clips
        new(boXY, 22, 0x16000, 0x00104), // 22 UnionPokemon (Fused)
        new(boXY, 23, 0x16200, 0x00004), // 23 ConfigSave
        new(boXY, 24, 0x16400, 0x00420), // 24 Amie decoration stuff
        new(boXY, 25, 0x16A00, 0x00064), // 25 OPower
        new(boXY, 26, 0x16C00, 0x003F0), // 26 Strength Rock position (xyz float: 84 entries, 12bytes/entry)
        new(boXY, 27, 0x17000, 0x0070C), // 27 Trainer PR Video
        new(boXY, 28, 0x17800, 0x00180), // 28 GtsData
        new(boXY, 29, 0x17A00, 0x00004), // 29 Packed Menu Bits
        new(boXY, 30, 0x17C00, 0x0000C), // 30 PSS Profile Q&A (6*questions, 6*answer)
        new(boXY, 31, 0x17E00, 0x00048), // 31 Repel Info, (Swarm?) and other overworld info (roamer)
        new(boXY, 32, 0x18000, 0x00054), // 32 BOSS data fetch history (serial/mystery gift), 4byte intro & 20*4byte entries
        new(boXY, 33, 0x18200, 0x00644), // 33 Streetpass history
        new(boXY, 34, 0x18A00, 0x005C8), // 34 LiveMatchData/BattleSpotData
        new(boXY, 35, 0x19000, 0x002F8), // 35 MAC Address & Network Connection Logging (0x98 per entry, 5 entries)
        new(boXY, 36, 0x19400, 0x01B40), // 36 Dendou (Hall of Fame)
        new(boXY, 37, 0x1B000, 0x001F4), // 37 BattleHouse (Maison)
        new(boXY, 38, 0x1B200, 0x001F0), // 38 Sodateya (Daycare)
        new(boXY, 39, 0x1B400, 0x00216), // 39 TrialHouse (Battle Institute)
        new(boXY, 40, 0x1B800, 0x00390), // 40 BerryField
        new(boXY, 41, 0x1BC00, 0x01A90), // 41 MysteryGiftSave
        new(boXY, 42, 0x1D800, 0x00308), // 42 [SubE]vent Log
        new(boXY, 43, 0x1DC00, 0x00618), // 43 PokeDiarySave
        new(boXY, 44, 0x1E400, 0x0025C), // 44 Record
        new(boXY, 45, 0x1E800, 0x00834), // 45 Friend Safari (0x15 per entry, 100 entries)
        new(boXY, 46, 0x1F200, 0x00318), // 46 SuperTrain
        new(boXY, 47, 0x1F600, 0x007D0), // 47 Unused (lmao)
        new(boXY, 48, 0x1FE00, 0x00C48), // 48 LinkInfo
        new(boXY, 49, 0x20C00, 0x00078), // 49 PSS usage info
        new(boXY, 50, 0x20E00, 0x00200), // 50 GameSyncSave
        new(boXY, 51, 0x21000, 0x00C84), // 51 PSS Icon (bool32 data present, 40x40 u16 pic, unused)
        new(boXY, 52, 0x21E00, 0x00628), // 52 ValidationSave (updateable Public Key for legal check api calls)
        new(boXY, 53, 0x22600, 0x34AD0), // 53 Box
        new(boXY, 54, 0x57200, 0x0E058), // 54 JPEG
    ];

    public IReadOnlyList<BlockInfo6> BlockInfo => BlocksXY;

    public Puff6 Puff { get; } = new(sav, 0x00000);
    public MyItem Items { get; } = new MyItem6XY(sav, 0x00400);
    public ItemInfo6 ItemInfo { get; } = new(sav, 0x01000);
    public GameTime6 GameTime { get; } = new(sav, 0x01200);
    public Situation6 Situation { get; } = new(sav, 0x01400);
    public PlayTime6 Played { get; } = new(sav, 0x01800);
    public Fashion6XY Fashion { get; } = new(sav, 0x1A00);
    public Misc6XY Misc { get; } = new(sav, 0x4200);
    public BoxLayout6 BoxLayout { get; } = new(sav, 0x4400);
    public BattleBox6 BattleBox { get; } = new(sav, 0x04A00);
    public MyStatus6 Status { get; } = new MyStatus6XY(sav, 0x14000);
    public Zukan6XY Zukan { get; } = new(sav, 0x15000, 0x3C8);
    public ConfigSave6 Config { get; } = new(sav, 0x16200);
    public OPower6 OPower { get; } = new(sav, 0x16A00);
    public Encount6 Encount { get; } = new(sav, 0x17E00);
    public MysteryBlock6 MysteryGift { get; } = new(sav, 0x1BC00);
    public SubEventLog6 SUBE { get; } = new SubEventLog6XY(sav, 0x1D800);
    public RecordBlock6 Records { get; } = new RecordBlock6XY(sav, 0x1E400);
    public SuperTrainBlock SuperTrain { get; } = new(sav, 0x1F200);
    public LinkBlock6 Link { get; } = new(sav, 0x1FE00);
    public MaisonBlock Maison { get; } = new(sav, 0x1B000);
}
