using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV6AODemo"/>.
/// </summary>
public sealed class SaveBlockAccessor6AODemo(SAV6AODemo sav) : ISaveBlockAccessor<BlockInfo6>, ISaveBlock6Core
{
    public const int BlockMetadataOffset = SaveUtil.SIZE_G6ORASDEMO - 0x200;
    private const int boAOdemo = BlockMetadataOffset;

    private static readonly BlockInfo6[] BlocksAODemo =
    [
        new(boAOdemo, 00, 0x00000, 0x00B90), // MyItem
        new(boAOdemo, 01, 0x00C00, 0x0002C), // ItemInfo6
        new(boAOdemo, 02, 0x00E00, 0x00038), // GameTime
        new(boAOdemo, 03, 0x01000, 0x00150), // Situation
        new(boAOdemo, 04, 0x01200, 0x00004), // RandomGroup (rand seeds)
        new(boAOdemo, 05, 0x01400, 0x00008), // PlayTime
        new(boAOdemo, 06, 0x01600, 0x00024), // temp variables (u32 id + 32 u8)
        new(boAOdemo, 07, 0x01800, 0x02100), // FieldMoveModelSave
        new(boAOdemo, 08, 0x03A00, 0x00130), // Misc
        new(boAOdemo, 09, 0x03C00, 0x00170), // MyStatus
        new(boAOdemo, 10, 0x03E00, 0x0061C), // PokePartySave
        new(boAOdemo, 11, 0x04600, 0x00504), // EventWork
        new(boAOdemo, 12, 0x04C00, 0x00004), // Packed Menu Bits
        new(boAOdemo, 13, 0x04E00, 0x00048), // Repel Info, (Swarm?) and other overworld info (roamer)
        new(boAOdemo, 14, 0x05000, 0x00400), // PokeDiarySave
        new(boAOdemo, 15, 0x05400, 0x0025C), // Record
    ];

    public IReadOnlyList<BlockInfo6> BlockInfo => BlocksAODemo;
    public MyItem6AO Items { get; } = new(sav, Block(sav, 0));
    public ItemInfo6 ItemInfo { get; } = new(sav, Block(sav, 1));
    public GameTime6 GameTime { get; } = new(sav, Block(sav, 2));
    public Situation6 Situation { get; } = new(sav, Block(sav, 3));
    public PlayTime6 Played { get; } = new(sav, Block(sav, 5));
    public Misc6AO Misc { get; } = new(sav, Block(sav, 8));
    public MyStatus6 Status { get; } = new(sav, Block(sav, 9));
    public EventWork6 EventWork { get; } = new(sav, Block(sav, 11));
    public RecordBlock6AO Records { get; } = new(sav, Block(sav, 15));

    MyItem ISaveBlock6Core.Items => Items;
    RecordBlock6 ISaveBlock6Core.Records => Records;

    private static Memory<byte> Block(SAV6AODemo sav, int index)
    {
        var data = sav.Data;
        var block = BlocksAODemo[index];
        return data.AsMemory(block.Offset, block.Length);
    }
}
