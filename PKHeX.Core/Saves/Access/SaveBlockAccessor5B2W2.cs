using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessor5B2W2 : ISaveBlock5BW, ISaveBlock5B2W2
    {
        public static readonly BlockInfoNDS[] BlocksB2W2 =
        {
            new BlockInfoNDS(0x00000, 0x03e0, 0x003E2, 0x25F00), // 00 Box Names
            new BlockInfoNDS(0x00400, 0x0ff0, 0x013F2, 0x25F02), // 01 Box 1
            new BlockInfoNDS(0x01400, 0x0ff0, 0x023F2, 0x25F04), // 02 Box 2
            new BlockInfoNDS(0x02400, 0x0ff0, 0x033F2, 0x25F06), // 03 Box 3
            new BlockInfoNDS(0x03400, 0x0ff0, 0x043F2, 0x25F08), // 04 Box 4
            new BlockInfoNDS(0x04400, 0x0ff0, 0x053F2, 0x25F0A), // 05 Box 5
            new BlockInfoNDS(0x05400, 0x0ff0, 0x063F2, 0x25F0C), // 06 Box 6
            new BlockInfoNDS(0x06400, 0x0ff0, 0x073F2, 0x25F0E), // 07 Box 7
            new BlockInfoNDS(0x07400, 0x0ff0, 0x083F2, 0x25F10), // 08 Box 8
            new BlockInfoNDS(0x08400, 0x0ff0, 0x093F2, 0x25F12), // 09 Box 9
            new BlockInfoNDS(0x09400, 0x0ff0, 0x0A3F2, 0x25F14), // 10 Box 10
            new BlockInfoNDS(0x0A400, 0x0ff0, 0x0B3F2, 0x25F16), // 11 Box 11
            new BlockInfoNDS(0x0B400, 0x0ff0, 0x0C3F2, 0x25F18), // 12 Box 12
            new BlockInfoNDS(0x0C400, 0x0ff0, 0x0D3F2, 0x25F1A), // 13 Box 13
            new BlockInfoNDS(0x0D400, 0x0ff0, 0x0E3F2, 0x25F1C), // 14 Box 14
            new BlockInfoNDS(0x0E400, 0x0ff0, 0x0F3F2, 0x25F1E), // 15 Box 15
            new BlockInfoNDS(0x0F400, 0x0ff0, 0x103F2, 0x25F20), // 16 Box 16
            new BlockInfoNDS(0x10400, 0x0ff0, 0x113F2, 0x25F22), // 17 Box 17
            new BlockInfoNDS(0x11400, 0x0ff0, 0x123F2, 0x25F24), // 18 Box 18
            new BlockInfoNDS(0x12400, 0x0ff0, 0x133F2, 0x25F26), // 19 Box 19
            new BlockInfoNDS(0x13400, 0x0ff0, 0x143F2, 0x25F28), // 20 Box 20
            new BlockInfoNDS(0x14400, 0x0ff0, 0x153F2, 0x25F2A), // 21 Box 21
            new BlockInfoNDS(0x15400, 0x0ff0, 0x163F2, 0x25F2C), // 22 Box 22
            new BlockInfoNDS(0x16400, 0x0ff0, 0x173F2, 0x25F2E), // 23 Box 23
            new BlockInfoNDS(0x17400, 0x0ff0, 0x183F2, 0x25F30), // 24 Box 24
            new BlockInfoNDS(0x18400, 0x09ec, 0x18DEE, 0x25F32), // 25 Inventory
            new BlockInfoNDS(0x18E00, 0x0534, 0x19336, 0x25F34), // 26 Party Pokemon
            new BlockInfoNDS(0x19400, 0x00b0, 0x194B2, 0x25F36), // 27 Trainer Data
            new BlockInfoNDS(0x19500, 0x00a8, 0x195AA, 0x25F38), // 28 Trainer Position
            new BlockInfoNDS(0x19600, 0x1338, 0x1A93A, 0x25F3A), // 29 Unity Tower and survey stuff
            new BlockInfoNDS(0x1AA00, 0x07c4, 0x1B1C6, 0x25F3C), // 30 Pal Pad Player Data
            new BlockInfoNDS(0x1B200, 0x0d54, 0x1BF56, 0x25F3E), // 31 Pal Pad Friend Data
            new BlockInfoNDS(0x1C000, 0x0094, 0x1C096, 0x25F40), // 32 Options / Skin Info
            new BlockInfoNDS(0x1C100, 0x0658, 0x1C75A, 0x25F42), // 33 Trainer Card
            new BlockInfoNDS(0x1C800, 0x0a94, 0x1D296, 0x25F44), // 34 Mystery Gift
            new BlockInfoNDS(0x1D300, 0x01ac, 0x1D4AE, 0x25F46), // 35 Dream World Stuff (Catalog)
            new BlockInfoNDS(0x1D500, 0x03ec, 0x1D8EE, 0x25F48), // 36 Chatter
            new BlockInfoNDS(0x1D900, 0x005c, 0x1D95E, 0x25F4A), // 37 Adventure data
            new BlockInfoNDS(0x1DA00, 0x01e0, 0x1DBE2, 0x25F4C), // 38 Record
            new BlockInfoNDS(0x1DC00, 0x00a8, 0x1DCAA, 0x25F4E), // 39 ???
            new BlockInfoNDS(0x1DD00, 0x0460, 0x1E162, 0x25F50), // 40 Mail
            new BlockInfoNDS(0x1E200, 0x1400, 0x1F602, 0x25F52), // 41 ???
            new BlockInfoNDS(0x1F700, 0x02a4, 0x1F9A6, 0x25F54), // 42 Musical
            new BlockInfoNDS(0x1FA00, 0x00e0, 0x1FAE2, 0x25F56), // 43 Fused Reshiram/Zekrom Storage
            new BlockInfoNDS(0x1FB00, 0x034c, 0x1FE4E, 0x25F58), // 44 IR
            new BlockInfoNDS(0x1FF00, 0x04e0, 0x203E2, 0x25F5A), // 45 EventWork
            new BlockInfoNDS(0x20400, 0x00f8, 0x204FA, 0x25F5C), // 46 ???
            new BlockInfoNDS(0x20500, 0x02fc, 0x207FE, 0x25F5E), // 47 Regulation
            new BlockInfoNDS(0x20800, 0x0094, 0x20896, 0x25F60), // 48 Gimmick
            new BlockInfoNDS(0x20900, 0x035c, 0x20C5E, 0x25F62), // 49 Battle Box
            new BlockInfoNDS(0x20D00, 0x01d4, 0x20ED6, 0x25F64), // 50 Daycare
            new BlockInfoNDS(0x20F00, 0x01e0, 0x210E2, 0x25F66), // 51 Strength Boulder Status Block
            new BlockInfoNDS(0x21100, 0x00f0, 0x211F2, 0x25F68), // 52 Misc (Badge Flags, Money, Trainer Sayings)
            new BlockInfoNDS(0x21200, 0x01b4, 0x213B6, 0x25F6A), // 53 Entralink (Level & Powers etc)
            new BlockInfoNDS(0x21400, 0x04dc, 0x218DE, 0x25F6C), // 54 Pokedex
            new BlockInfoNDS(0x21900, 0x0034, 0x21936, 0x25F6E), // 55 Encount (Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type)
            new BlockInfoNDS(0x21A00, 0x003c, 0x21A3E, 0x25F70), // 56 Battle Subway Play Info
            new BlockInfoNDS(0x21B00, 0x01ac, 0x21CAE, 0x25F72), // 57 Battle Subway Score Info
            new BlockInfoNDS(0x21D00, 0x0b90, 0x22892, 0x25F74), // 58 Battle Subway WiFI Info
            new BlockInfoNDS(0x22900, 0x00ac, 0x229AE, 0x25F76), // 59 Online Records
            new BlockInfoNDS(0x22A00, 0x0850, 0x23252, 0x25F78), // 60 Entralink Forest pokémon data
            new BlockInfoNDS(0x23300, 0x0284, 0x23586, 0x25F7A), // 61 ???
            new BlockInfoNDS(0x23600, 0x0010, 0x23612, 0x25F7C), // 62 ???
            new BlockInfoNDS(0x23700, 0x00a8, 0x237AA, 0x25F7E), // 63 PWT related data
            new BlockInfoNDS(0x23800, 0x016c, 0x2396E, 0x25F80), // 64 ???
            new BlockInfoNDS(0x23A00, 0x0080, 0x23A82, 0x25F82), // 65 ???
            new BlockInfoNDS(0x23B00, 0x00fc, 0x23BFE, 0x25F84), // 66 Hollow/Rival Block
            new BlockInfoNDS(0x23C00, 0x16a8, 0x252AA, 0x25F86), // 67 Join Avenue Block
            new BlockInfoNDS(0x25300, 0x0498, 0x2579A, 0x25F88), // 68 Medal
            new BlockInfoNDS(0x25800, 0x0060, 0x25862, 0x25F8A), // 69 Key-related data
            new BlockInfoNDS(0x25900, 0x00fc, 0x259FE, 0x25F8C), // 70 Festa Missions
            new BlockInfoNDS(0x25A00, 0x03e4, 0x25DE6, 0x25F8E), // 71 ???
            new BlockInfoNDS(0x25E00, 0x00f0, 0x25EF2, 0x25F90), // 72 ???
            new BlockInfoNDS(0x25F00, 0x0094, 0x25FA2, 0x25FA2), // 73 Checksum Block
        };

        public SaveBlockAccessor5B2W2(SAV5B2W2 sav)
        {
            BoxLayout = new BoxLayout5(sav, 0x00000);
            Items = new MyItem5B2W2(sav, 0x18400);
            PlayerData = new PlayerData5(sav, 0x19400);
            Mystery = new MysteryBlock5(sav, 0x1C800);
            Daycare = new Daycare5(sav, 0x20D00);
            Misc = new Misc5B2W2(sav, 0x21100);
            Zukan = new Zukan5(sav, 0x21400, 0x328); // forme flags size is + 8 from bw with new formes (therians)
            BattleSubway = new BattleSubway5(sav, 0x21B00);
            PWT = new PWTBlock5(sav, 0x23700);
        }

        public IReadOnlyList<BlockInfoNDS> BlockInfo => BlocksB2W2;
        public MyItem Items { get; }
        public Zukan5 Zukan { get; }
        public Misc5 Misc { get; }
        public MysteryBlock5 Mystery { get; }
        public Daycare5 Daycare { get; }
        public BoxLayout5 BoxLayout { get; }
        public PlayerData5 PlayerData { get; }
        public BattleSubway5 BattleSubway { get; }
        public PWTBlock5 PWT { get; }
    }
}