using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV7b"/>.
/// </summary>
public sealed class SaveBlockAccessor7b(SAV7b sav) : ISaveBlockAccessor<BlockInfo7b>, ISaveBlock7b
{
    private const int boGG = 0xB8800 - 0x200; // nowhere near 1MB (savedata.bin size)

    private static readonly BlockInfo7b[] BlockInfoGG =
    [
        new(boGG, 00, 0x00000, 0x00D90),
        new(boGG, 01, 0x00E00, 0x00200),
        new(boGG, 02, 0x01000, 0x00168),
        new(boGG, 03, 0x01200, 0x01800),
        new(boGG, 04, 0x02A00, 0x020E8),
        new(boGG, 05, 0x04C00, 0x00930),
        new(boGG, 06, 0x05600, 0x00004),
        new(boGG, 07, 0x05800, 0x00130),
        new(boGG, 08, 0x05A00, 0x00012),
        new(boGG, 09, 0x05C00, 0x3F7A0),
        new(boGG, 10, 0x45400, 0x00008),
        new(boGG, 11, 0x45600, 0x00E90),
        new(boGG, 12, 0x46600, 0x010A4),
        new(boGG, 13, 0x47800, 0x000F0),
        new(boGG, 14, 0x47A00, 0x06010),
        new(boGG, 15, 0x4DC00, 0x00200),
        new(boGG, 16, 0x4DE00, 0x00098),
        new(boGG, 17, 0x4E000, 0x00068),
        new(boGG, 18, 0x4E200, 0x69780),
        new(boGG, 19, 0xB7A00, 0x000B0),
        new(boGG, 20, 0xB7C00, 0x00940),
    ];

    public IReadOnlyList<BlockInfo7b> BlockInfo => BlockInfoGG;

    public MyItem7b Items { get; } = new(sav, Block(sav, BelugaBlockIndex.MyItem));
    public Coordinates7b Coordinates { get; } = new(sav, Block(sav, BelugaBlockIndex.Coordinates));
    public Misc7b Misc { get; } = new(sav, Block(sav, BelugaBlockIndex.Misc));
    public Zukan7b Zukan { get; } = new(sav, Block(sav, BelugaBlockIndex.Zukan), 0x550);
    public MyStatus7b Status { get; } = new(sav, Block(sav, BelugaBlockIndex.MyStatus));
    public PlayTime7b Played { get; } = new(sav, Block(sav, BelugaBlockIndex.PlayTime));
    public ConfigSave7b Config { get; } = new(sav, Block(sav, BelugaBlockIndex.ConfigSave));
    public EventWork7b EventWork { get; } = new(sav, Block(sav, BelugaBlockIndex.EventWork));
    public PokeListHeader Storage { get; } = new(sav, Block(sav, BelugaBlockIndex.PokeListHeader), sav.State.Exportable);
    public WB7Records GiftRecords { get; } = new(sav, Block(sav, BelugaBlockIndex.WB7Record));
    public CaptureRecords Captured { get; } = new(sav, Block(sav, BelugaBlockIndex.CaptureRecord));
    public Daycare7b Daycare { get; } = new(sav, Block(sav, BelugaBlockIndex.Daycare));
    public Fashion7b FashionPlayer { get; } = new(sav, Block(sav, BelugaBlockIndex.FashionPlayer));
    public Fashion7b FashionStarter { get; } = new(sav, Block(sav, BelugaBlockIndex.FashionStarter));
    public GoParkStorage Park { get; } = new(sav, Block(sav, BelugaBlockIndex.GoParkEntities));
    public PlayerGeoLocation7b PlayerGeoLocation { get; } = new(sav, Block(sav, BelugaBlockIndex.PlayerGeoLocation));

    public static Memory<byte> Block(SAV7b sav, BelugaBlockIndex index)
    {
        var block = BlockInfoGG[(int)index];
        return sav.Data.AsMemory(block.Offset, block.Length);
    }
}
