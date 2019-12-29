using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessor5BW : ISaveBlockAccessor<BlockInfoNDS>, ISaveBlock5BW
    {
        // Offset, Length, chkOffset, ChkMirror
        public static readonly BlockInfoNDS[] BlocksBW =
        {
            new BlockInfoNDS(0x00000, 0x03E0, 0x003E2, 0x23F00), // 00 Box Names
            new BlockInfoNDS(0x00400, 0x0FF0, 0x013F2, 0x23F02), // 01 Box 1
            new BlockInfoNDS(0x01400, 0x0FF0, 0x023F2, 0x23F04), // 02 Box 2
            new BlockInfoNDS(0x02400, 0x0FF0, 0x033F2, 0x23F06), // 03 Box 3
            new BlockInfoNDS(0x03400, 0x0FF0, 0x043F2, 0x23F08), // 04 Box 4
            new BlockInfoNDS(0x04400, 0x0FF0, 0x053F2, 0x23F0A), // 05 Box 5
            new BlockInfoNDS(0x05400, 0x0FF0, 0x063F2, 0x23F0C), // 06 Box 6
            new BlockInfoNDS(0x06400, 0x0FF0, 0x073F2, 0x23F0E), // 07 Box 7
            new BlockInfoNDS(0x07400, 0x0FF0, 0x083F2, 0x23F10), // 08 Box 8
            new BlockInfoNDS(0x08400, 0x0FF0, 0x093F2, 0x23F12), // 09 Box 9
            new BlockInfoNDS(0x09400, 0x0FF0, 0x0A3F2, 0x23F14), // 10 Box 10
            new BlockInfoNDS(0x0A400, 0x0FF0, 0x0B3F2, 0x23F16), // 11 Box 11
            new BlockInfoNDS(0x0B400, 0x0FF0, 0x0C3F2, 0x23F18), // 12 Box 12
            new BlockInfoNDS(0x0C400, 0x0FF0, 0x0D3F2, 0x23F1A), // 13 Box 13
            new BlockInfoNDS(0x0D400, 0x0FF0, 0x0E3F2, 0x23F1C), // 14 Box 14
            new BlockInfoNDS(0x0E400, 0x0FF0, 0x0F3F2, 0x23F1E), // 15 Box 15
            new BlockInfoNDS(0x0F400, 0x0FF0, 0x103F2, 0x23F20), // 16 Box 16
            new BlockInfoNDS(0x10400, 0x0FF0, 0x113F2, 0x23F22), // 17 Box 17
            new BlockInfoNDS(0x11400, 0x0FF0, 0x123F2, 0x23F24), // 18 Box 18
            new BlockInfoNDS(0x12400, 0x0FF0, 0x133F2, 0x23F26), // 19 Box 19
            new BlockInfoNDS(0x13400, 0x0FF0, 0x143F2, 0x23F28), // 20 Box 20
            new BlockInfoNDS(0x14400, 0x0FF0, 0x153F2, 0x23F2A), // 21 Box 21
            new BlockInfoNDS(0x15400, 0x0FF0, 0x163F2, 0x23F2C), // 22 Box 22
            new BlockInfoNDS(0x16400, 0x0FF0, 0x173F2, 0x23F2E), // 23 Box 23
            new BlockInfoNDS(0x17400, 0x0FF0, 0x183F2, 0x23F30), // 24 Box 24
            new BlockInfoNDS(0x18400, 0x09C0, 0x18DC2, 0x23F32), // 25 Inventory
            new BlockInfoNDS(0x18E00, 0x0534, 0x19336, 0x23F34), // 26 Party Pokemon
            new BlockInfoNDS(0x19400, 0x0068, 0x1946A, 0x23F36), // 27 Trainer Data
            new BlockInfoNDS(0x19500, 0x009C, 0x1959E, 0x23F38), // 28 Trainer Position
            new BlockInfoNDS(0x19600, 0x1338, 0x1A93A, 0x23F3A), // 29 Unity Tower and survey stuff
            new BlockInfoNDS(0x1AA00, 0x07C4, 0x1B1C6, 0x23F3C), // 30 Pal Pad Player Data
            new BlockInfoNDS(0x1B200, 0x0D54, 0x1BF56, 0x23F3E), // 31 Pal Pad Friend Data
            new BlockInfoNDS(0x1C000, 0x002C, 0x1C02E, 0x23F40), // 32 Skin Info
            new BlockInfoNDS(0x1C100, 0x0658, 0x1C75A, 0x23F42), // 33 ??? Gym badge data
            new BlockInfoNDS(0x1C800, 0x0A94, 0x1D296, 0x23F44), // 34 Mystery Gift
            new BlockInfoNDS(0x1D300, 0x01AC, 0x1D4AE, 0x23F46), // 35 Dream World Stuff (Catalog)
            new BlockInfoNDS(0x1D500, 0x03EC, 0x1D8EE, 0x23F48), // 36 Chatter
            new BlockInfoNDS(0x1D900, 0x005C, 0x1D95E, 0x23F4A), // 37 Adventure Info
            new BlockInfoNDS(0x1DA00, 0x01E0, 0x1DBE2, 0x23F4C), // 38 Trainer Card Records
            new BlockInfoNDS(0x1DC00, 0x00A8, 0x1DCAA, 0x23F4E), // 39 ???
            new BlockInfoNDS(0x1DD00, 0x0460, 0x1E162, 0x23F50), // 40 ???
            new BlockInfoNDS(0x1E200, 0x1400, 0x1F602, 0x23F52), // 41 ???
            new BlockInfoNDS(0x1F700, 0x02A4, 0x1F9A6, 0x23F54), // 42 Contains flags and references for downloaded data (Musical)
            new BlockInfoNDS(0x1FA00, 0x02DC, 0x1FCDE, 0x23F56), // 43 ???
            new BlockInfoNDS(0x1FD00, 0x034C, 0x2004E, 0x23F58), // 44 ???
            new BlockInfoNDS(0x20100, 0x03EC, 0x204EE, 0x23F5A), // 45 ???
            new BlockInfoNDS(0x20500, 0x00F8, 0x205FA, 0x23F5C), // 46 ???
            new BlockInfoNDS(0x20600, 0x02FC, 0x208FE, 0x23F5E), // 47 Tournament Block
            new BlockInfoNDS(0x20900, 0x0094, 0x20996, 0x23F60), // 48 ???
            new BlockInfoNDS(0x20A00, 0x035C, 0x20D5E, 0x23F62), // 49 Battle Box Block
            new BlockInfoNDS(0x20E00, 0x01CC, 0x20FCE, 0x23F64), // 50 Daycare Block
            new BlockInfoNDS(0x21000, 0x0168, 0x2116A, 0x23F66), // 51 Strength Boulder Status Block
            new BlockInfoNDS(0x21200, 0x00EC, 0x212EE, 0x23F68), // 52 Badge Flags, Money, Trainer Sayings
            new BlockInfoNDS(0x21300, 0x01B0, 0x214B2, 0x23F6A), // 53 Entralink (Level & Powers etc)
            new BlockInfoNDS(0x21500, 0x001C, 0x2151E, 0x23F6C), // 54 ???
            new BlockInfoNDS(0x21600, 0x04D4, 0x21AD6, 0x23F6E), // 55 Pokedex
            new BlockInfoNDS(0x21B00, 0x0034, 0x21B36, 0x23F70), // 56 Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type
            new BlockInfoNDS(0x21C00, 0x003C, 0x21C3E, 0x23F72), // 57 ???
            new BlockInfoNDS(0x21D00, 0x01AC, 0x21EAE, 0x23F74), // 58 Battle Subway
            new BlockInfoNDS(0x21F00, 0x0B90, 0x22A92, 0x23F76), // 59 ???
            new BlockInfoNDS(0x22B00, 0x009C, 0x22B9E, 0x23F78), // 60 Online Records
            new BlockInfoNDS(0x22C00, 0x0850, 0x23452, 0x23F7A), // 61 Entralink Forest pokémon data
            new BlockInfoNDS(0x23500, 0x0028, 0x2352A, 0x23F7C), // 62 ???
            new BlockInfoNDS(0x23600, 0x0284, 0x23886, 0x23F7E), // 63 ???
            new BlockInfoNDS(0x23900, 0x0010, 0x23912, 0x23F80), // 64 ???
            new BlockInfoNDS(0x23A00, 0x005C, 0x23A5E, 0x23F82), // 65 ???
            new BlockInfoNDS(0x23B00, 0x016C, 0x23C6E, 0x23F84), // 66 ???
            new BlockInfoNDS(0x23D00, 0x0040, 0x23D42, 0x23F86), // 67 ???
            new BlockInfoNDS(0x23E00, 0x00FC, 0x23EFE, 0x23F88), // 68 ???
            new BlockInfoNDS(0x23F00, 0x008C, 0x23F9A, 0x23F9A), // 69 Checksums */
        };

        public SaveBlockAccessor5BW(SAV5BW sav)
        {
            BoxLayout = new BoxLayout5(sav, 0x00000);
            Items = new MyItem5BW(sav, 0x18400);
            PlayerData = new PlayerData5(sav, 0x19400);
            Mystery = new MysteryBlock5(sav, 0x1C800);
            Daycare = new Daycare5(sav, 0x20E00);
            Misc = new Misc5BW(sav, 0x21200);
            Zukan = new Zukan5(sav, 0x21600, 0x320);
            BattleSubway = new BattleSubway5(sav, 0x21D00);
        }

        public IReadOnlyList<BlockInfoNDS> BlockInfo => BlocksBW;
        public MyItem Items { get; }
        public Zukan5 Zukan { get; }
        public Misc5 Misc { get; }
        public MysteryBlock5 Mystery { get; }
        public Daycare5 Daycare { get; }
        public BoxLayout5 BoxLayout { get; }
        public PlayerData5 PlayerData { get; }
        public BattleSubway5 BattleSubway { get; }
    }
}