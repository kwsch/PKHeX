using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV7USUM"/>.
/// </summary>
public sealed class SaveBlockAccessor7USUM(SAV7USUM sav) : ISaveBlockAccessor<BlockInfo7>, ISaveBlock7USUM
{
    public const int BlockMetadataOffset = SaveUtil.SIZE_G7USUM - 0x200;
    private const int boUU = BlockMetadataOffset;

    private static readonly BlockInfo7[] BlockInfoUSUM =
    [
        new(boUU, 00, 0x00000, 0x00E28), // 00 MyItem
        new(boUU, 01, 0x01000, 0x0007C), // 01 Situation
        new(boUU, 02, 0x01200, 0x00014), // 02 RandomGroup
        new(boUU, 03, 0x01400, 0x000C0), // 03 MyStatus
        new(boUU, 04, 0x01600, 0x0061C), // 04 PokePartySave
        new(boUU, 05, 0x01E00, 0x00E00), // 05 EventWork
        new(boUU, 06, 0x02C00, 0x00F78), // 06 ZukanData
        new(boUU, 07, 0x03C00, 0x00228), // 07 GtsData
        new(boUU, 08, 0x04000, 0x0030C), // 08 UnionPokemon
        new(boUU, 09, 0x04400, 0x001FC), // 09 Misc
        new(boUU, 10, 0x04600, 0x0004C), // 10 FieldMenu
        new(boUU, 11, 0x04800, 0x00004), // 11 ConfigSave
        new(boUU, 12, 0x04A00, 0x00058), // 12 GameTime
        new(boUU, 13, 0x04C00, 0x005E6), // 13 BOX
        new(boUU, 14, 0x05200, 0x36600), // 14 BoxPokemon
        new(boUU, 15, 0x3B800, 0x0572C), // 15 ResortSave
        new(boUU, 16, 0x41000, 0x00008), // 16 PlayTime
        new(boUU, 17, 0x41200, 0x01218), // 17 FieldMoveModelSave
        new(boUU, 18, 0x42600, 0x01A08), // 18 Fashion
        new(boUU, 19, 0x44200, 0x06408), // 19 JoinFestaPersonalSave
        new(boUU, 20, 0x4A800, 0x06408), // 20 JoinFestaPersonalSave
        new(boUU, 21, 0x50E00, 0x03998), // 21 JoinFestaDataSave
        new(boUU, 22, 0x54800, 0x00100), // 22 BerrySpot
        new(boUU, 23, 0x54A00, 0x00100), // 23 FishingSpot
        new(boUU, 24, 0x54C00, 0x10528), // 24 LiveMatchData
        new(boUU, 25, 0x65200, 0x00204), // 25 BattleSpotData
        new(boUU, 26, 0x65600, 0x00B60), // 26 PokeFinderSave
        new(boUU, 27, 0x66200, 0x03F50), // 27 MysteryGiftSave
        new(boUU, 28, 0x6A200, 0x00358), // 28 Record
        new(boUU, 29, 0x6A600, 0x00728), // 29 ValidationSave
        new(boUU, 30, 0x6AE00, 0x00200), // 30 GameSyncSave
        new(boUU, 31, 0x6B000, 0x00718), // 31 PokeDiarySave
        new(boUU, 32, 0x6B800, 0x001FC), // 32 BattleInstSave
        new(boUU, 33, 0x6BA00, 0x00200), // 33 Sodateya
        new(boUU, 34, 0x6BC00, 0x00120), // 34 WeatherSave
        new(boUU, 35, 0x6BE00, 0x001C8), // 35 QRReaderSaveData
        new(boUU, 36, 0x6C000, 0x00200), // 36 TurtleSalmonSave
        new(boUU, 37, 0x6C200, 0x0039C), // 37 BattleFesSave
        new(boUU, 38, 0x6C600, 0x00400), // 38 FinderStudioSave
    ];

    public IReadOnlyList<BlockInfo7> BlockInfo => BlockInfoUSUM;
    public MyItem7USUM Items { get; } = new(sav, Block(sav, 00));
    public Situation7 Situation { get; } = new(sav, Block(sav, 01));
    public MyStatus7 MyStatus { get; } = new(sav, Block(sav, 03));
    public EventWork7USUM EventWork { get; } = new(sav, Block(sav, 5));
    public Zukan7 Zukan { get; } = new(sav, Block(sav, 06), 0x550);
    public GTS7 GTS { get; } = new(sav, Block(sav, 07));
    public UnionPokemon7 Fused { get; } = new(sav, Block(sav, 08));
    public Misc7 Misc { get; } = new(sav, Block(sav, 09));
    public FieldMenu7 FieldMenu { get; } = new(sav, Block(sav, 10));
    public ConfigSave7 Config { get; } = new(sav, Block(sav, 11));
    public GameTime7 GameTime { get; } = new(sav, Block(sav, 12));
    public BoxLayout7 BoxLayout { get; } = new(sav, Block(sav, 13));
    public ResortSave7 ResortSave { get; } = new(sav, Block(sav, 15));
    public PlayTime6 Played { get; } = new(sav, Block(sav, 16));
    public FieldMoveModelSave7 Overworld { get; } = new(sav, Block(sav, 17));
    public FashionBlock7 Fashion { get; } = new(sav, Block(sav, 18));
    public JoinFesta7 Festa { get; } = new(sav, Block(sav, 21));
    public PokeFinder7 PokeFinder { get; } = new(sav, Block(sav, 26));
    public MysteryBlock7 MysteryGift { get; } = new(sav, Block(sav, 27));
    public RecordBlock7USUM Records { get; } = new(sav, Block(sav, 28));
    public BattleTree7 BattleTree { get; } = new(sav, Block(sav, 32));
    public Daycare7 Daycare { get; } = new(sav, Block(sav, 33));
    public BattleAgency7 BattleAgency { get; } = new(sav, Block(sav, 37));

    MyItem ISaveBlock7Main.Items => Items;
    EventWork7 ISaveBlock7Main.EventWork => EventWork;
    RecordBlock6 ISaveBlock7Main.Records => Records;

    private static Memory<byte> Block(SAV7USUM sav, int index)
    {
        var data = sav.Buffer;
        var block = BlockInfoUSUM[index];
        return data.Slice(block.Offset, block.Length);
    }
}
