using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV5B2W2"/>.
/// </summary>
public sealed class SaveBlockAccessor5B2W2(SAV5B2W2 sav)
    : ISaveBlockAccessor<BlockInfoNDS>, ISaveBlock5BW, ISaveBlock5B2W2
{
    private static readonly BlockInfoNDS[] BlocksB2W2 =
    [
        new(0x00000, 0x03e0, 0x003E2, 0x25F00), // 00 Box Names
        new(0x00400, 0x0ff0, 0x013F2, 0x25F02), // 01 Box 1
        new(0x01400, 0x0ff0, 0x023F2, 0x25F04), // 02 Box 2
        new(0x02400, 0x0ff0, 0x033F2, 0x25F06), // 03 Box 3
        new(0x03400, 0x0ff0, 0x043F2, 0x25F08), // 04 Box 4
        new(0x04400, 0x0ff0, 0x053F2, 0x25F0A), // 05 Box 5
        new(0x05400, 0x0ff0, 0x063F2, 0x25F0C), // 06 Box 6
        new(0x06400, 0x0ff0, 0x073F2, 0x25F0E), // 07 Box 7
        new(0x07400, 0x0ff0, 0x083F2, 0x25F10), // 08 Box 8
        new(0x08400, 0x0ff0, 0x093F2, 0x25F12), // 09 Box 9
        new(0x09400, 0x0ff0, 0x0A3F2, 0x25F14), // 10 Box 10
        new(0x0A400, 0x0ff0, 0x0B3F2, 0x25F16), // 11 Box 11
        new(0x0B400, 0x0ff0, 0x0C3F2, 0x25F18), // 12 Box 12
        new(0x0C400, 0x0ff0, 0x0D3F2, 0x25F1A), // 13 Box 13
        new(0x0D400, 0x0ff0, 0x0E3F2, 0x25F1C), // 14 Box 14
        new(0x0E400, 0x0ff0, 0x0F3F2, 0x25F1E), // 15 Box 15
        new(0x0F400, 0x0ff0, 0x103F2, 0x25F20), // 16 Box 16
        new(0x10400, 0x0ff0, 0x113F2, 0x25F22), // 17 Box 17
        new(0x11400, 0x0ff0, 0x123F2, 0x25F24), // 18 Box 18
        new(0x12400, 0x0ff0, 0x133F2, 0x25F26), // 19 Box 19
        new(0x13400, 0x0ff0, 0x143F2, 0x25F28), // 20 Box 20
        new(0x14400, 0x0ff0, 0x153F2, 0x25F2A), // 21 Box 21
        new(0x15400, 0x0ff0, 0x163F2, 0x25F2C), // 22 Box 22
        new(0x16400, 0x0ff0, 0x173F2, 0x25F2E), // 23 Box 23
        new(0x17400, 0x0ff0, 0x183F2, 0x25F30), // 24 Box 24
        new(0x18400, 0x09ec, 0x18DEE, 0x25F32), // 25 Inventory
        new(0x18E00, 0x0534, 0x19336, 0x25F34), // 26 Party Pokémon
        new(0x19400, 0x00b0, 0x194B2, 0x25F36), // 27 Trainer Data
        new(0x19500, 0x00a8, 0x195AA, 0x25F38), // 28 Trainer Position
        new(0x19600, 0x1338, 0x1A93A, 0x25F3A), // 29 Unity Tower and survey stuff
        new(0x1AA00, 0x07c4, 0x1B1C6, 0x25F3C), // 30 Pal Pad Player Data
        new(0x1B200, 0x0d54, 0x1BF56, 0x25F3E), // 31 Pal Pad Friend Data
        new(0x1C000, 0x0094, 0x1C096, 0x25F40), // 32 Options / Skin Info
        new(0x1C100, 0x0658, 0x1C75A, 0x25F42), // 33 Trainer Card
        new(0x1C800, 0x0a94, 0x1D296, 0x25F44), // 34 Mystery Gift
        new(0x1D300, 0x01ac, 0x1D4AE, 0x25F46), // 35 Dream World Stuff (Catalog)
        new(0x1D500, 0x03ec, 0x1D8EE, 0x25F48), // 36 Chatter
        new(0x1D900, 0x005c, 0x1D95E, 0x25F4A), // 37 Adventure data
        new(0x1DA00, 0x01e0, 0x1DBE2, 0x25F4C), // 38 Trainer Card Records
        new(0x1DC00, 0x00a8, 0x1DCAA, 0x25F4E), // 39 ???
        new(0x1DD00, 0x0460, 0x1E162, 0x25F50), // 40 Mail
        new(0x1E200, 0x1400, 0x1F602, 0x25F52), // 41 Overworld State
        new(0x1F700, 0x02a4, 0x1F9A6, 0x25F54), // 42 Musical
        new(0x1FA00, 0x00e0, 0x1FAE2, 0x25F56), // 43 White Forest + Black City Data, Fused Reshiram/Zekrom Storage
        new(0x1FB00, 0x034c, 0x1FE4E, 0x25F58), // 44 IR
        new(0x1FF00, 0x04e0, 0x203E2, 0x25F5A), // 45 EventWork
        new(0x20400, 0x00f8, 0x204FA, 0x25F5C), // 46 GTS
        new(0x20500, 0x02fc, 0x207FE, 0x25F5E), // 47 Regulation Tournament
        new(0x20800, 0x0094, 0x20896, 0x25F60), // 48 Gimmick
        new(0x20900, 0x035c, 0x20C5E, 0x25F62), // 49 Battle Box
        new(0x20D00, 0x01d4, 0x20ED6, 0x25F64), // 50 Daycare
        new(0x20F00, 0x01e0, 0x210E2, 0x25F66), // 51 Strength Boulder Status
        new(0x21100, 0x00f0, 0x211F2, 0x25F68), // 52 Misc (Badge Flags, Money, Trainer Sayings)
        new(0x21200, 0x01b4, 0x213B6, 0x25F6A), // 53 Entralink (Level & Powers etc)
        new(0x21400, 0x04dc, 0x218DE, 0x25F6C), // 54 Pokedex
        new(0x21900, 0x0034, 0x21936, 0x25F6E), // 55 Encount (Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type)
        new(0x21A00, 0x003c, 0x21A3E, 0x25F70), // 56 Battle Subway Play Info
        new(0x21B00, 0x01ac, 0x21CAE, 0x25F72), // 57 Battle Subway Score Info
        new(0x21D00, 0x0b90, 0x22892, 0x25F74), // 58 Battle Subway Wi-Fi Info
        new(0x22900, 0x00ac, 0x229AE, 0x25F76), // 59 Online Records
        new(0x22A00, 0x0850, 0x23252, 0x25F78), // 60 Entralink Forest pokémon data
        new(0x23300, 0x0284, 0x23586, 0x25F7A), // 61 Answered Questions
        new(0x23600, 0x0010, 0x23612, 0x25F7C), // 62 Unity Tower
        new(0x23700, 0x00a8, 0x237AA, 0x25F7E), // 63 PWT related data
        new(0x23800, 0x016c, 0x2396E, 0x25F80), // 64 ???
        new(0x23A00, 0x0080, 0x23A82, 0x25F82), // 65 ???
        new(0x23B00, 0x00fc, 0x23BFE, 0x25F84), // 66 Hollow/Rival Block
        new(0x23C00, 0x16a8, 0x252AA, 0x25F86), // 67 Join Avenue Block
        new(0x25300, 0x0498, 0x2579A, 0x25F88), // 68 Medal
        new(0x25800, 0x0060, 0x25862, 0x25F8A), // 69 Key-related data
        new(0x25900, 0x00fc, 0x259FE, 0x25F8C), // 70 Festa Missions
        new(0x25A00, 0x03e4, 0x25DE6, 0x25F8E), // 71 Pokestar Studios
        new(0x25E00, 0x00f0, 0x25EF2, 0x25F90), // 72 ???
        new(0x25F00, 0x0094, 0x25FA2, 0x25FA2), // 73 Checksum Block
    ];

    public IReadOnlyList<BlockInfoNDS> BlockInfo => BlocksB2W2;
    public BoxLayout5 BoxLayout { get; } = new(sav, Block(sav, 0));
    public MyItem5B2W2 Items { get; } = new(sav, Block(sav, 25));
    public PlayerData5 PlayerData { get; } = new(sav, Block(sav, 27));
    public PlayerPosition5 PlayerPosition { get; } = new(sav, Block(sav, 28));
    public UnityTower5 UnityTower { get; } = new(sav, Block(sav, 29));
    public MysteryBlock5 Mystery { get; } = new(sav, Block(sav, 34));
    public GlobalLink5 GlobalLink { get; } = new(sav, Block(sav, 35));
    public Chatter5 Chatter { get; } = new(sav, Block(sav, 36));
    public AdventureInfo5 AdventureInfo { get; } = new(sav, Block(sav, 37));
    public Record5 Records { get; } = new(sav, Block(sav, 38));
    public Musical5 Musical { get; } = new(sav, Block(sav, 42));
    public WhiteBlack5B2W2 Forest { get; } = new(sav, Block(sav, 43));
    public EventWork5B2W2 EventWork { get; } = new(sav, Block(sav, 45));
    public GTS5 GTS { get; } = new(sav, Block(sav, 46));
    public BattleBox5 BattleBox { get; } = new(sav, Block(sav, 49));
    public Daycare5 Daycare { get; } = new(sav, Block(sav, 50));
    public Misc5B2W2 Misc { get; } = new(sav, Block(sav, 52));
    public Entralink5B2W2 Entralink { get; } = new(sav, Block(sav, 53));
    public Zukan5 Zukan { get; } = new(sav, Block(sav, 54), 0x328); // form flags size is + 8 from B/W with new forms (Therians)
    public Encount5B2W2 Encount { get; } = new(sav, Block(sav, 55));
    public BattleSubway5 BattleSubway { get; } = new(sav, Block(sav, 57));
    public EntreeForest EntreeForest { get; } = new(sav, Block(sav, 60));
    public PWTBlock5 PWT { get; } = new(sav, Block(sav, 63));
    public MedalList5 Medals { get; } = new(sav, Block(sav, 68));
    public KeySystem5 Keys { get; } = new(sav, Block(sav, 69));
    public FestaBlock5 Festa { get; } = new(sav, Block(sav, 70));
    EventWork5 ISaveBlock5BW.EventWork => EventWork;
    Encount5 ISaveBlock5BW.Encount => Encount;
    MyItem ISaveBlock5BW.Items => Items;
    Entralink5 ISaveBlock5BW.Entralink => Entralink;
    Misc5 ISaveBlock5BW.Misc => Misc;

    public static Memory<byte> Block(SAV5B2W2 sav, int index)
    {
        var block = BlocksB2W2[index];
        return sav.Data.AsMemory(block.Offset, block.Length);
    }
}
