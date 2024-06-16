using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV5BW"/>.
/// </summary>
public sealed class SaveBlockAccessor5BW(SAV5BW sav) : ISaveBlockAccessor<BlockInfoNDS>, ISaveBlock5BW
{
    private static readonly BlockInfoNDS[] BlocksBW =
    [
        new(0x00000, 0x03E0, 0x003E2, 0x23F00), // 00 Box Names
        new(0x00400, 0x0FF0, 0x013F2, 0x23F02), // 01 Box 1
        new(0x01400, 0x0FF0, 0x023F2, 0x23F04), // 02 Box 2
        new(0x02400, 0x0FF0, 0x033F2, 0x23F06), // 03 Box 3
        new(0x03400, 0x0FF0, 0x043F2, 0x23F08), // 04 Box 4
        new(0x04400, 0x0FF0, 0x053F2, 0x23F0A), // 05 Box 5
        new(0x05400, 0x0FF0, 0x063F2, 0x23F0C), // 06 Box 6
        new(0x06400, 0x0FF0, 0x073F2, 0x23F0E), // 07 Box 7
        new(0x07400, 0x0FF0, 0x083F2, 0x23F10), // 08 Box 8
        new(0x08400, 0x0FF0, 0x093F2, 0x23F12), // 09 Box 9
        new(0x09400, 0x0FF0, 0x0A3F2, 0x23F14), // 10 Box 10
        new(0x0A400, 0x0FF0, 0x0B3F2, 0x23F16), // 11 Box 11
        new(0x0B400, 0x0FF0, 0x0C3F2, 0x23F18), // 12 Box 12
        new(0x0C400, 0x0FF0, 0x0D3F2, 0x23F1A), // 13 Box 13
        new(0x0D400, 0x0FF0, 0x0E3F2, 0x23F1C), // 14 Box 14
        new(0x0E400, 0x0FF0, 0x0F3F2, 0x23F1E), // 15 Box 15
        new(0x0F400, 0x0FF0, 0x103F2, 0x23F20), // 16 Box 16
        new(0x10400, 0x0FF0, 0x113F2, 0x23F22), // 17 Box 17
        new(0x11400, 0x0FF0, 0x123F2, 0x23F24), // 18 Box 18
        new(0x12400, 0x0FF0, 0x133F2, 0x23F26), // 19 Box 19
        new(0x13400, 0x0FF0, 0x143F2, 0x23F28), // 20 Box 20
        new(0x14400, 0x0FF0, 0x153F2, 0x23F2A), // 21 Box 21
        new(0x15400, 0x0FF0, 0x163F2, 0x23F2C), // 22 Box 22
        new(0x16400, 0x0FF0, 0x173F2, 0x23F2E), // 23 Box 23
        new(0x17400, 0x0FF0, 0x183F2, 0x23F30), // 24 Box 24
        new(0x18400, 0x09C0, 0x18DC2, 0x23F32), // 25 Inventory
        new(0x18E00, 0x0534, 0x19336, 0x23F34), // 26 Party Pokémon
        new(0x19400, 0x0068, 0x1946A, 0x23F36), // 27 Trainer Data
        new(0x19500, 0x009C, 0x1959E, 0x23F38), // 28 Trainer Position
        new(0x19600, 0x1338, 0x1A93A, 0x23F3A), // 29 Unity Tower and survey stuff
        new(0x1AA00, 0x07C4, 0x1B1C6, 0x23F3C), // 30 Pal Pad Player Data
        new(0x1B200, 0x0D54, 0x1BF56, 0x23F3E), // 31 Pal Pad Friend Data
        new(0x1C000, 0x002C, 0x1C02E, 0x23F40), // 32 Skin Info
        new(0x1C100, 0x0658, 0x1C75A, 0x23F42), // 33 ??? Gym badge data
        new(0x1C800, 0x0A94, 0x1D296, 0x23F44), // 34 Mystery Gift
        new(0x1D300, 0x01AC, 0x1D4AE, 0x23F46), // 35 Dream World Stuff (Catalog)
        new(0x1D500, 0x03EC, 0x1D8EE, 0x23F48), // 36 Chatter
        new(0x1D900, 0x005C, 0x1D95E, 0x23F4A), // 37 Adventure Info
        new(0x1DA00, 0x01E0, 0x1DBE2, 0x23F4C), // 38 Trainer Card Records
        new(0x1DC00, 0x00A8, 0x1DCAA, 0x23F4E), // 39 ???
        new(0x1DD00, 0x0460, 0x1E162, 0x23F50), // 40 Mail
        new(0x1E200, 0x1400, 0x1F602, 0x23F52), // 41 Overworld State
        new(0x1F700, 0x02A4, 0x1F9A6, 0x23F54), // 42 Musical
        new(0x1FA00, 0x02DC, 0x1FCDE, 0x23F56), // 43 White Forest + Black City Data
        new(0x1FD00, 0x034C, 0x2004E, 0x23F58), // 44 IR
        new(0x20100, 0x03EC, 0x204EE, 0x23F5A), // 45 EventWork
        new(0x20500, 0x00F8, 0x205FA, 0x23F5C), // 46 GTS
        new(0x20600, 0x02FC, 0x208FE, 0x23F5E), // 47 Regulation Tournament
        new(0x20900, 0x0094, 0x20996, 0x23F60), // 48 Gimmick
        new(0x20A00, 0x035C, 0x20D5E, 0x23F62), // 49 Battle Box
        new(0x20E00, 0x01CC, 0x20FCE, 0x23F64), // 50 Daycare
        new(0x21000, 0x0168, 0x2116A, 0x23F66), // 51 Strength Boulder Status
        new(0x21200, 0x00EC, 0x212EE, 0x23F68), // 52 Badge Flags, Money, Trainer Sayings
        new(0x21300, 0x01B0, 0x214B2, 0x23F6A), // 53 Entralink (Level & Powers etc)
        new(0x21500, 0x001C, 0x2151E, 0x23F6C), // 54 ???
        new(0x21600, 0x04D4, 0x21AD6, 0x23F6E), // 55 Pokedex
        new(0x21B00, 0x0034, 0x21B36, 0x23F70), // 56 Encount Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type
        new(0x21C00, 0x003C, 0x21C3E, 0x23F72), // 57 Battle Subway Play Info
        new(0x21D00, 0x01AC, 0x21EAE, 0x23F74), // 58 Battle Subway Score Info
        new(0x21F00, 0x0B90, 0x22A92, 0x23F76), // 59 Battle Subway Wi-Fi Info
        new(0x22B00, 0x009C, 0x22B9E, 0x23F78), // 60 Online Records
        new(0x22C00, 0x0850, 0x23452, 0x23F7A), // 61 Entralink Forest pokémon data
        new(0x23500, 0x0028, 0x2352A, 0x23F7C), // 62 ???
        new(0x23600, 0x0284, 0x23886, 0x23F7E), // 63 Answered Questions
        new(0x23900, 0x0010, 0x23912, 0x23F80), // 64 Unity Tower
        new(0x23A00, 0x005C, 0x23A5E, 0x23F82), // 65 ???
        new(0x23B00, 0x016C, 0x23C6E, 0x23F84), // 66 ???
        new(0x23D00, 0x0040, 0x23D42, 0x23F86), // 67 ???
        new(0x23E00, 0x00FC, 0x23EFE, 0x23F88), // 68 ???
        new(0x23F00, 0x008C, 0x23F9A, 0x23F9A), // 69 Checksums */
    ];

    public IReadOnlyList<BlockInfoNDS> BlockInfo => BlocksBW;
    public BoxLayout5 BoxLayout { get; } = new(sav, Block(sav, 0));
    public MyItem5BW Items { get; } = new(sav, Block(sav, 25));
    public PlayerData5 PlayerData { get; } = new(sav, Block(sav, 27));
    public PlayerPosition5 PlayerPosition { get; } = new(sav, Block(sav, 28));
    public UnityTower5 UnityTower { get; } = new(sav, Block(sav, 29));
    public MysteryBlock5 Mystery { get; } = new(sav, Block(sav, 34));
    public GlobalLink5 GlobalLink { get;  } = new(sav, Block(sav, 35));
    public Chatter5 Chatter { get; } = new(sav, Block(sav, 36));
    public AdventureInfo5 AdventureInfo { get; } = new(sav, Block(sav, 37));
    public Record5 Records { get; } = new(sav, Block(sav, 38));
    public Musical5 Musical { get; } = new(sav, Block(sav, 42));
    public WhiteBlack5BW Forest { get; } = new(sav, Block(sav, 43));
    public EventWork5BW EventWork { get; } = new(sav, Block(sav, 45));
    public GTS5 GTS { get; } = new(sav, Block(sav, 46));
    public BattleBox5 BattleBox { get; } = new(sav, Block(sav, 49));
    public Daycare5 Daycare { get; } = new(sav, Block(sav, 50));
    public Misc5BW Misc { get; } = new(sav, Block(sav, 52));
    public Entralink5BW Entralink { get; } = new(sav, Block(sav, 53));
    public Zukan5 Zukan { get; } = new(sav, Block(sav, 55), 0x320);
    public Encount5BW Encount { get; } = new(sav, Block(sav, 56));
    public BattleSubway5 BattleSubway { get; } = new(sav, Block(sav, 58));
    public EntreeForest EntreeForest { get; } = new(sav, Block(sav, 61));
    EventWork5 ISaveBlock5BW.EventWork => EventWork;
    Encount5 ISaveBlock5BW.Encount => Encount;
    MyItem ISaveBlock5BW.Items => Items;
    Entralink5 ISaveBlock5BW.Entralink => Entralink;
    Misc5 ISaveBlock5BW.Misc => Misc;

    public static Memory<byte> Block(SAV5BW sav, int index)
    {
        var block = BlocksBW[index];
        return sav.Data.AsMemory(block.Offset, block.Length);
    }
}
