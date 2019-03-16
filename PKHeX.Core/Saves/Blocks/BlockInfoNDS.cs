using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Gen5 Block Info
    /// </summary>
    public sealed class BlockInfoNDS : BlockInfo
    {
        private readonly int ChecksumOffset;
        private readonly int ChecksumMirror;

        private BlockInfoNDS(int offset, int length, int chkOffset, int chkMirror)
        {
            Offset = offset;
            Length = length;
            ChecksumOffset = chkOffset;
            ChecksumMirror = chkMirror;
        }

        private ushort GetChecksum(byte[] data) => Checksums.CRC16_CCITT(data, Offset, Length);

        protected override bool ChecksumValid(byte[] data)
        {
            ushort chk = GetChecksum(data);
            if (chk != BitConverter.ToUInt16(data, ChecksumOffset))
                return false;
            if (chk != BitConverter.ToUInt16(data, ChecksumMirror))
                return false;
            return true;
        }

        protected override void SetChecksum(byte[] data)
        {
            ushort chk = GetChecksum(data);
            var bytes = BitConverter.GetBytes(chk);
            bytes.CopyTo(data, ChecksumOffset);
            bytes.CopyTo(data, ChecksumMirror);
        }

        // Offset, Length, chkOffset, ChkMirror
        public static readonly BlockInfoNDS[] BlocksBW =
        {
            new BlockInfoNDS(0x00000, 0x03E0, 0x003E2, 0x23F00), // Box Names
            new BlockInfoNDS(0x00400, 0x0FF0, 0x013F2, 0x23F02), // Box 1
            new BlockInfoNDS(0x01400, 0x0FF0, 0x023F2, 0x23F04), // Box 2
            new BlockInfoNDS(0x02400, 0x0FF0, 0x033F2, 0x23F06), // Box 3
            new BlockInfoNDS(0x03400, 0x0FF0, 0x043F2, 0x23F08), // Box 4
            new BlockInfoNDS(0x04400, 0x0FF0, 0x053F2, 0x23F0A), // Box 5
            new BlockInfoNDS(0x05400, 0x0FF0, 0x063F2, 0x23F0C), // Box 6
            new BlockInfoNDS(0x06400, 0x0FF0, 0x073F2, 0x23F0E), // Box 7
            new BlockInfoNDS(0x07400, 0x0FF0, 0x083F2, 0x23F10), // Box 8
            new BlockInfoNDS(0x08400, 0x0FF0, 0x093F2, 0x23F12), // Box 9
            new BlockInfoNDS(0x09400, 0x0FF0, 0x0A3F2, 0x23F14), // Box 10
            new BlockInfoNDS(0x0A400, 0x0FF0, 0x0B3F2, 0x23F16), // Box 11
            new BlockInfoNDS(0x0B400, 0x0FF0, 0x0C3F2, 0x23F18), // Box 12
            new BlockInfoNDS(0x0C400, 0x0FF0, 0x0D3F2, 0x23F1A), // Box 13
            new BlockInfoNDS(0x0D400, 0x0FF0, 0x0E3F2, 0x23F1C), // Box 14
            new BlockInfoNDS(0x0E400, 0x0FF0, 0x0F3F2, 0x23F1E), // Box 15
            new BlockInfoNDS(0x0F400, 0x0FF0, 0x103F2, 0x23F20), // Box 16
            new BlockInfoNDS(0x10400, 0x0FF0, 0x113F2, 0x23F22), // Box 17
            new BlockInfoNDS(0x11400, 0x0FF0, 0x123F2, 0x23F24), // Box 18
            new BlockInfoNDS(0x12400, 0x0FF0, 0x133F2, 0x23F26), // Box 19
            new BlockInfoNDS(0x13400, 0x0FF0, 0x143F2, 0x23F28), // Box 20
            new BlockInfoNDS(0x14400, 0x0FF0, 0x153F2, 0x23F2A), // Box 21
            new BlockInfoNDS(0x15400, 0x0FF0, 0x163F2, 0x23F2C), // Box 22
            new BlockInfoNDS(0x16400, 0x0FF0, 0x173F2, 0x23F2E), // Box 23
            new BlockInfoNDS(0x17400, 0x0FF0, 0x183F2, 0x23F30), // Box 24
            new BlockInfoNDS(0x18400, 0x09C0, 0x18DC2, 0x23F32), // Inventory
            new BlockInfoNDS(0x18E00, 0x0534, 0x19336, 0x23F34), // Party Pokemon
            new BlockInfoNDS(0x19400, 0x0068, 0x1946A, 0x23F36), // Trainer Data
            new BlockInfoNDS(0x19500, 0x009C, 0x1959E, 0x23F38), // Trainer Position
            new BlockInfoNDS(0x19600, 0x1338, 0x1A93A, 0x23F3A), // Unity Tower and survey stuff
            new BlockInfoNDS(0x1AA00, 0x07C4, 0x1B1C6, 0x23F3C), // Pal Pad Player Data
            new BlockInfoNDS(0x1B200, 0x0D54, 0x1BF56, 0x23F3E), // Pal Pad Friend Data
            new BlockInfoNDS(0x1C000, 0x002C, 0x1C02E, 0x23F40), // Skin Info
            new BlockInfoNDS(0x1C100, 0x0658, 0x1C75A, 0x23F42), // ??? Gym badge data
            new BlockInfoNDS(0x1C800, 0x0A94, 0x1D296, 0x23F44), // Mystery Gift
            new BlockInfoNDS(0x1D300, 0x01AC, 0x1D4AE, 0x23F46), // Dream World Stuff (Catalog)
            new BlockInfoNDS(0x1D500, 0x03EC, 0x1D8EE, 0x23F48), // Chatter
            new BlockInfoNDS(0x1D900, 0x005C, 0x1D95E, 0x23F4A), // Adventure Info
            new BlockInfoNDS(0x1DA00, 0x01E0, 0x1DBE2, 0x23F4C), // Trainer Card Records
            new BlockInfoNDS(0x1DC00, 0x00A8, 0x1DCAA, 0x23F4E), // ???
            new BlockInfoNDS(0x1DD00, 0x0460, 0x1E162, 0x23F50), // (40d)
            new BlockInfoNDS(0x1E200, 0x1400, 0x1F602, 0x23F52), // ???
            new BlockInfoNDS(0x1F700, 0x02A4, 0x1F9A6, 0x23F54), // Contains flags and references for downloaded data (Musical)
            new BlockInfoNDS(0x1FA00, 0x02DC, 0x1FCDE, 0x23F56), // ???
            new BlockInfoNDS(0x1FD00, 0x034C, 0x2004E, 0x23F58), // ???
            new BlockInfoNDS(0x20100, 0x03EC, 0x204EE, 0x23F5A), // ???
            new BlockInfoNDS(0x20500, 0x00F8, 0x205FA, 0x23F5C), // ???
            new BlockInfoNDS(0x20600, 0x02FC, 0x208FE, 0x23F5E), // Tournament Block
            new BlockInfoNDS(0x20900, 0x0094, 0x20996, 0x23F60), // ???
            new BlockInfoNDS(0x20A00, 0x035C, 0x20D5E, 0x23F62), // Battle Box Block
            new BlockInfoNDS(0x20E00, 0x01CC, 0x20FCE, 0x23F64), // Daycare Block
            new BlockInfoNDS(0x21000, 0x0168, 0x2116A, 0x23F66), // Strength Boulder Status Block
            new BlockInfoNDS(0x21200, 0x00EC, 0x212EE, 0x23F68), // Badge Flags, Money, Trainer Sayings
            new BlockInfoNDS(0x21300, 0x01B0, 0x214B2, 0x23F6A), // Entralink (Level & Powers etc)
            new BlockInfoNDS(0x21500, 0x001C, 0x2151E, 0x23F6C), // ???
            new BlockInfoNDS(0x21600, 0x04D4, 0x21AD6, 0x23F6E), // Pokedex
            new BlockInfoNDS(0x21B00, 0x0034, 0x21B36, 0x23F70), // Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type
            new BlockInfoNDS(0x21C00, 0x003C, 0x21C3E, 0x23F72), // ???
            new BlockInfoNDS(0x21D00, 0x01AC, 0x21EAE, 0x23F74), // Battle Subway
            new BlockInfoNDS(0x21F00, 0x0B90, 0x22A92, 0x23F76), // ???
            new BlockInfoNDS(0x22B00, 0x009C, 0x22B9E, 0x23F78), // Online Records
            new BlockInfoNDS(0x22C00, 0x0850, 0x23452, 0x23F7A), // Entralink Forest pokémon data
            new BlockInfoNDS(0x23500, 0x0028, 0x2352A, 0x23F7C), // ???
            new BlockInfoNDS(0x23600, 0x0284, 0x23886, 0x23F7E), // ???
            new BlockInfoNDS(0x23900, 0x0010, 0x23912, 0x23F80), // ???
            new BlockInfoNDS(0x23A00, 0x005C, 0x23A5E, 0x23F82), // ???
            new BlockInfoNDS(0x23B00, 0x016C, 0x23C6E, 0x23F84), // ???
            new BlockInfoNDS(0x23D00, 0x0040, 0x23D42, 0x23F86), // ???
            new BlockInfoNDS(0x23E00, 0x00FC, 0x23EFE, 0x23F88), // ???
            new BlockInfoNDS(0x23F00, 0x008C, 0x23F9A, 0x23F9A), // Checksums */
        };

        public static readonly BlockInfoNDS[] BlocksB2W2 =
        {
            new BlockInfoNDS(0x00000, 0x03e0, 0x003E2, 0x25F00), // Box Names
            new BlockInfoNDS(0x00400, 0x0ff0, 0x013F2, 0x25F02), // Box 1
            new BlockInfoNDS(0x01400, 0x0ff0, 0x023F2, 0x25F04), // Box 2
            new BlockInfoNDS(0x02400, 0x0ff0, 0x033F2, 0x25F06), // Box 3
            new BlockInfoNDS(0x03400, 0x0ff0, 0x043F2, 0x25F08), // Box 4
            new BlockInfoNDS(0x04400, 0x0ff0, 0x053F2, 0x25F0A), // Box 5
            new BlockInfoNDS(0x05400, 0x0ff0, 0x063F2, 0x25F0C), // Box 6
            new BlockInfoNDS(0x06400, 0x0ff0, 0x073F2, 0x25F0E), // Box 7
            new BlockInfoNDS(0x07400, 0x0ff0, 0x083F2, 0x25F10), // Box 8
            new BlockInfoNDS(0x08400, 0x0ff0, 0x093F2, 0x25F12), // Box 9
            new BlockInfoNDS(0x09400, 0x0ff0, 0x0A3F2, 0x25F14), // Box 10
            new BlockInfoNDS(0x0A400, 0x0ff0, 0x0B3F2, 0x25F16), // Box 11
            new BlockInfoNDS(0x0B400, 0x0ff0, 0x0C3F2, 0x25F18), // Box 12
            new BlockInfoNDS(0x0C400, 0x0ff0, 0x0D3F2, 0x25F1A), // Box 13
            new BlockInfoNDS(0x0D400, 0x0ff0, 0x0E3F2, 0x25F1C), // Box 14
            new BlockInfoNDS(0x0E400, 0x0ff0, 0x0F3F2, 0x25F1E), // Box 15
            new BlockInfoNDS(0x0F400, 0x0ff0, 0x103F2, 0x25F20), // Box 16
            new BlockInfoNDS(0x10400, 0x0ff0, 0x113F2, 0x25F22), // Box 17
            new BlockInfoNDS(0x11400, 0x0ff0, 0x123F2, 0x25F24), // Box 18
            new BlockInfoNDS(0x12400, 0x0ff0, 0x133F2, 0x25F26), // Box 19
            new BlockInfoNDS(0x13400, 0x0ff0, 0x143F2, 0x25F28), // Box 20
            new BlockInfoNDS(0x14400, 0x0ff0, 0x153F2, 0x25F2A), // Box 21
            new BlockInfoNDS(0x15400, 0x0ff0, 0x163F2, 0x25F2C), // Box 22
            new BlockInfoNDS(0x16400, 0x0ff0, 0x173F2, 0x25F2E), // Box 23
            new BlockInfoNDS(0x17400, 0x0ff0, 0x183F2, 0x25F30), // Box 24
            new BlockInfoNDS(0x18400, 0x09ec, 0x18DEE, 0x25F32), // Inventory
            new BlockInfoNDS(0x18E00, 0x0534, 0x19336, 0x25F34), // Party Pokemon
            new BlockInfoNDS(0x19400, 0x00b0, 0x194B2, 0x25F36), // Trainer Data
            new BlockInfoNDS(0x19500, 0x00a8, 0x195AA, 0x25F38), // Trainer Position
            new BlockInfoNDS(0x19600, 0x1338, 0x1A93A, 0x25F3A), // Unity Tower and survey stuff
            new BlockInfoNDS(0x1AA00, 0x07c4, 0x1B1C6, 0x25F3C), // Pal Pad Player Data (30d)
            new BlockInfoNDS(0x1B200, 0x0d54, 0x1BF56, 0x25F3E), // Pal Pad Friend Data
            new BlockInfoNDS(0x1C000, 0x0094, 0x1C096, 0x25F40), // Options / Skin Info
            new BlockInfoNDS(0x1C100, 0x0658, 0x1C75A, 0x25F42), // Trainer Card
            new BlockInfoNDS(0x1C800, 0x0a94, 0x1D296, 0x25F44), // Mystery Gift
            new BlockInfoNDS(0x1D300, 0x01ac, 0x1D4AE, 0x25F46), // Dream World Stuff (Catalog)
            new BlockInfoNDS(0x1D500, 0x03ec, 0x1D8EE, 0x25F48), // Chatter
            new BlockInfoNDS(0x1D900, 0x005c, 0x1D95E, 0x25F4A), // Adventure data
            new BlockInfoNDS(0x1DA00, 0x01e0, 0x1DBE2, 0x25F4C), // Record
            new BlockInfoNDS(0x1DC00, 0x00a8, 0x1DCAA, 0x25F4E), // ???
            new BlockInfoNDS(0x1DD00, 0x0460, 0x1E162, 0x25F50), // Mail (40d)
            new BlockInfoNDS(0x1E200, 0x1400, 0x1F602, 0x25F52), // ???
            new BlockInfoNDS(0x1F700, 0x02a4, 0x1F9A6, 0x25F54), // Musical
            new BlockInfoNDS(0x1FA00, 0x00e0, 0x1FAE2, 0x25F56), // Fused Reshiram/Zekrom Storage
            new BlockInfoNDS(0x1FB00, 0x034c, 0x1FE4E, 0x25F58), // IR
            new BlockInfoNDS(0x1FF00, 0x04e0, 0x203E2, 0x25F5A), // EventWork
            new BlockInfoNDS(0x20400, 0x00f8, 0x204FA, 0x25F5C), // ???
            new BlockInfoNDS(0x20500, 0x02fc, 0x207FE, 0x25F5E), // Regulation
            new BlockInfoNDS(0x20800, 0x0094, 0x20896, 0x25F60), // Gimmick
            new BlockInfoNDS(0x20900, 0x035c, 0x20C5E, 0x25F62), // Battle Box
            new BlockInfoNDS(0x20D00, 0x01d4, 0x20ED6, 0x25F64), // Daycare (50d)
            new BlockInfoNDS(0x20F00, 0x01e0, 0x210E2, 0x25F66), // Strength Boulder Status Block
            new BlockInfoNDS(0x21100, 0x00f0, 0x211F2, 0x25F68), // Misc (Badge Flags, Money, Trainer Sayings)
            new BlockInfoNDS(0x21200, 0x01b4, 0x213B6, 0x25F6A), // Entralink (Level & Powers etc)
            new BlockInfoNDS(0x21400, 0x04dc, 0x218DE, 0x25F6C), // Pokedex
            new BlockInfoNDS(0x21900, 0x0034, 0x21936, 0x25F6E), // Encount (Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type)
            new BlockInfoNDS(0x21A00, 0x003c, 0x21A3E, 0x25F70), // Battle Subway Play Info
            new BlockInfoNDS(0x21B00, 0x01ac, 0x21CAE, 0x25F72), // Battle Subway Score Info
            new BlockInfoNDS(0x21D00, 0x0b90, 0x22892, 0x25F74), // Battle Subway WiFI Info
            new BlockInfoNDS(0x22900, 0x00ac, 0x229AE, 0x25F76), // Online Records
            new BlockInfoNDS(0x22A00, 0x0850, 0x23252, 0x25F78), // Entralink Forest pokémon data (60d)
            new BlockInfoNDS(0x23300, 0x0284, 0x23586, 0x25F7A), // ???
            new BlockInfoNDS(0x23600, 0x0010, 0x23612, 0x25F7C), // ???
            new BlockInfoNDS(0x23700, 0x00a8, 0x237AA, 0x25F7E), // PWT related data
            new BlockInfoNDS(0x23800, 0x016c, 0x2396E, 0x25F80), // ???
            new BlockInfoNDS(0x23A00, 0x0080, 0x23A82, 0x25F82), // ???
            new BlockInfoNDS(0x23B00, 0x00fc, 0x23BFE, 0x25F84), // Hollow/Rival Block
            new BlockInfoNDS(0x23C00, 0x16a8, 0x252AA, 0x25F86), // Join Avenue Block
            new BlockInfoNDS(0x25300, 0x0498, 0x2579A, 0x25F88), // Medal
            new BlockInfoNDS(0x25800, 0x0060, 0x25862, 0x25F8A), // Key-related data
            new BlockInfoNDS(0x25900, 0x00fc, 0x259FE, 0x25F8C), // Festa Missions (70d)
            new BlockInfoNDS(0x25A00, 0x03e4, 0x25DE6, 0x25F8E), // ???
            new BlockInfoNDS(0x25E00, 0x00f0, 0x25EF2, 0x25F90), // ???
            new BlockInfoNDS(0x25F00, 0x0094, 0x25FA2, 0x25FA2), // Checksum Block (73d)
        };
    }
}