using System;
using System.Linq;

namespace PKHeX
{
    public class BlockInfo5
    {
        public int Offset;
        public int Length;
        public int ChecksumOffset;
        public int ChecksumMirror;
    }
    public class SAV5 : PKM
    {
        internal const int SIZERAW = 0x80000; // 512KB
        internal const int SIZE_BW = 0x24000; // B/W
        internal const int SIZE_B2W2 = 0x26000; // B2/W2

        internal static int getIsG5SAV(byte[] data)
        {
            ushort chk1 = BitConverter.ToUInt16(data, SIZE_BW - 0x100 + 0x8C + 0xE);
            ushort actual1 = ccitt16(data.Skip(SIZE_BW - 0x100).Take(0x8C).ToArray());
            if (chk1 == actual1)
                return 0;
            ushort chk2 = BitConverter.ToUInt16(data, SIZE_B2W2 - 0x100 + 0x94 + 0xE);
            ushort actual2 = ccitt16(data.Skip(SIZE_B2W2 - 0x100).Take(0x94).ToArray());
            if (chk2 == actual2)
                return 1;
            return -1;
        }

        // Global Settings
        // Save Data Attributes
        public readonly byte[] Data;
        public bool Edited;
        public readonly bool Exportable;
        public readonly byte[] BAK;
        public string FileName, FilePath;

        private BlockInfo5[] Blocks;

        public SAV5(byte[] data = null)
        {
            Data = (byte[])(data ?? new byte[SIZERAW]).Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Get Version
            int version = getIsG5SAV(Data);
            if (version < 0) // Invalidate Data
                Data = null;

            B2W2 = version == 1;
            BW = version == 0;

            // Different Offsets for different games.
            BattleBox = version == 1 ? 0x20A00 : 0x20900;

            #region Block Tables
            if (BW)
            {
                Blocks = new[]
                {
                    new BlockInfo5
                    {
                        Offset = 0x00000,
                        Length = 0x3E0,
                        ChecksumOffset = 0x003E2,
                        ChecksumMirror = 0x23F00 /* Box Names */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x00400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x013F2,
                        ChecksumMirror = 0x23F02 /* Box 1 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x01400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x023F2,
                        ChecksumMirror = 0x23F04 /* Box 2 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x02400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x033F2,
                        ChecksumMirror = 0x23F06 /* Box 3 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x03400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x043F2,
                        ChecksumMirror = 0x23F08 /* Box 4 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x04400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x053F2,
                        ChecksumMirror = 0x23F0A /* Box 5 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x05400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x063F2,
                        ChecksumMirror = 0x23F0C /* Box 6 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x06400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x073F2,
                        ChecksumMirror = 0x23F0E /* Box 7 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x07400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x083F2,
                        ChecksumMirror = 0x23F10 /* Box 8 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x08400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x093F2,
                        ChecksumMirror = 0x23F12 /* Box 9 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x09400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x0A3F2,
                        ChecksumMirror = 0x23F14 /* Box 10 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0A400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x0B3F2,
                        ChecksumMirror = 0x23F16 /* Box 11 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0B400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x0C3F2,
                        ChecksumMirror = 0x23F18 /* Box 12 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0C400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x0D3F2,
                        ChecksumMirror = 0x23F1A /* Box 13 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0D400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x0E3F2,
                        ChecksumMirror = 0x23F1C /* Box 14 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0E400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x0F3F2,
                        ChecksumMirror = 0x23F1E /* Box 15 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0F400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x103F2,
                        ChecksumMirror = 0x23F20 /* Box 16 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x10400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x113F2,
                        ChecksumMirror = 0x23F22 /* Box 17 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x11400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x123F2,
                        ChecksumMirror = 0x23F24 /* Box 18 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x12400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x133F2,
                        ChecksumMirror = 0x23F26 /* Box 19 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x13400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x143F2,
                        ChecksumMirror = 0x23F28 /* Box 20 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x14400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x153F2,
                        ChecksumMirror = 0x23F2A /* Box 21 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x15400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x163F2,
                        ChecksumMirror = 0x23F2C /* Box 22 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x16400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x173F2,
                        ChecksumMirror = 0x23F2E /* Box 23 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x17400,
                        Length = 0xFF0,
                        ChecksumOffset = 0x183F2,
                        ChecksumMirror = 0x23F30 /* Box 24 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x18400,
                        Length = 0x9C0,
                        ChecksumOffset = 0x18DC2,
                        ChecksumMirror = 0x23F32 /* Inventory */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x18E00,
                        Length = 0x534,
                        ChecksumOffset = 0x19336,
                        ChecksumMirror = 0x23F34 /* Party Pokemon */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x19400,
                        Length = 0x68,
                        ChecksumOffset = 0x19469,
                        ChecksumMirror = 0x23F36 /* Trainer Data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x19500,
                        Length = 0x9C,
                        ChecksumOffset = 0x1959E,
                        ChecksumMirror = 0x23F38 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x19600,
                        Length = 0x1338,
                        ChecksumOffset = 0x1A93A,
                        ChecksumMirror = 0x23F3A /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1AA00,
                        Length = 0x7C4,
                        ChecksumOffset = 0x1B1C6,
                        ChecksumMirror = 0x23F3C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1B200,
                        Length = 0xD54,
                        ChecksumOffset = 0x1BF56,
                        ChecksumMirror = 0x23F3E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1C000,
                        Length = 0x2C,
                        ChecksumOffset = 0x1C02E,
                        ChecksumMirror = 0x23F40 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1C100,
                        Length = 0x658,
                        ChecksumOffset = 0x1C75A,
                        ChecksumMirror = 0x23F42 /* ??? Gym badge data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1C800,
                        Length = 0xA94,
                        ChecksumOffset = 0x1D296,
                        ChecksumMirror = 0x23F44 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1D300,
                        Length = 0x1AC,
                        ChecksumOffset = 0x1D4AE,
                        ChecksumMirror = 0x23F46 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1D500,
                        Length = 0x3EC,
                        ChecksumOffset = 0x1D8EE,
                        ChecksumMirror = 0x23F48 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1D900,
                        Length = 0x5C,
                        ChecksumOffset = 0x1D95E,
                        ChecksumMirror = 0x23F4A /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1DA00,
                        Length = 0x1E0,
                        ChecksumOffset = 0x1DBE2,
                        ChecksumMirror = 0x23F4C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1DC00,
                        Length = 0xA8,
                        ChecksumOffset = 0x1DCAA,
                        ChecksumMirror = 0x23F4E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1DD00,
                        Length = 0x460,
                        ChecksumOffset = 0x1E162,
                        ChecksumMirror = 0x23F50 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1E200,
                        Length = 0x1400,
                        ChecksumOffset = 0x1F602,
                        ChecksumMirror = 0x23F52 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1F700,
                        Length = 0x2A4,
                        ChecksumOffset = 0x1F9A6,
                        ChecksumMirror = 0x23F54 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1FA00,
                        Length = 0x2DC,
                        ChecksumOffset = 0x1FCDE,
                        ChecksumMirror = 0x23F56 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1FD00,
                        Length = 0x34C,
                        ChecksumOffset = 0x2004E,
                        ChecksumMirror = 0x23F58 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20100,
                        Length = 0x3EC,
                        ChecksumOffset = 0x204EE,
                        ChecksumMirror = 0x23F5A /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20500,
                        Length = 0xF8,
                        ChecksumOffset = 0x205FA,
                        ChecksumMirror = 0x23F5C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20600,
                        Length = 0x2FC,
                        ChecksumOffset = 0x208FE,
                        ChecksumMirror = 0x23F5E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20900,
                        Length = 0x94,
                        ChecksumOffset = 0x20996,
                        ChecksumMirror = 0x23F60 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20A00,
                        Length = 0x35C,
                        ChecksumOffset = 0x20D5E,
                        ChecksumMirror = 0x23F62 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20E00,
                        Length = 0x1CC,
                        ChecksumOffset = 0x20FCE,
                        ChecksumMirror = 0x23F64 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21000,
                        Length = 0x168,
                        ChecksumOffset = 0x2116A,
                        ChecksumMirror = 0x23F66 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21200,
                        Length = 0xEC,
                        ChecksumOffset = 0x212EE,
                        ChecksumMirror = 0x23F68 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21300,
                        Length = 0x1B0,
                        ChecksumOffset = 0x214B2,
                        ChecksumMirror = 0x23F6A /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21500,
                        Length = 0x1C,
                        ChecksumOffset = 0x2151E,
                        ChecksumMirror = 0x23F6C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21600,
                        Length = 0x4D4,
                        ChecksumOffset = 0x21AD6,
                        ChecksumMirror = 0x23F6E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21B00,
                        Length = 0x34,
                        ChecksumOffset = 0x21B36,
                        ChecksumMirror = 0x23F70 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21C00,
                        Length = 0x3C,
                        ChecksumOffset = 0x21C3E,
                        ChecksumMirror = 0x23F72 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21D00,
                        Length = 0x1AC,
                        ChecksumOffset = 0x21EAE,
                        ChecksumMirror = 0x23F74 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21F00,
                        Length = 0xB90,
                        ChecksumOffset = 0x22A92,
                        ChecksumMirror = 0x23F76 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x22B00,
                        Length = 0x9C,
                        ChecksumOffset = 0x22B9E,
                        ChecksumMirror = 0x23F78 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x22C00,
                        Length = 0x850,
                        ChecksumOffset = 0x23452,
                        ChecksumMirror = 0x23F7A /* Entralink Forest pokémon data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23500,
                        Length = 0x28,
                        ChecksumOffset = 0x2352A,
                        ChecksumMirror = 0x23F7C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23600,
                        Length = 0x284,
                        ChecksumOffset = 0x23886,
                        ChecksumMirror = 0x23F7E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23900,
                        Length = 0x10,
                        ChecksumOffset = 0x23912,
                        ChecksumMirror = 0x23F80 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23A00,
                        Length = 0x5C,
                        ChecksumOffset = 0x23A5E,
                        ChecksumMirror = 0x23F82 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23B00,
                        Length = 0x16C,
                        ChecksumOffset = 0x23C6E,
                        ChecksumMirror = 0x23F84 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23D00,
                        Length = 0x40,
                        ChecksumOffset = 0x23D42,
                        ChecksumMirror = 0x23F86 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23E00,
                        Length = 0xFC,
                        ChecksumOffset = 0x23EFE,
                        ChecksumMirror = 0x23F88 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23F00,
                        Length = 0x8C,
                        ChecksumOffset = 0x23F9A,
                        ChecksumMirror = 0x23F9A /* Checksums */
                    }
                };
            }
            else if (B2W2)
            {
                Blocks = new[]
                {
                    new BlockInfo5
                    {
                        Offset = 0x00000,
                        Length = 0x3e0,
                        ChecksumOffset = 0x003E2,
                        ChecksumMirror = 0x25F00 /* Box Names */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x00400,
                        Length = 0xff0,
                        ChecksumOffset = 0x013F2,
                        ChecksumMirror = 0x25F02 /* Box 1 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x01400,
                        Length = 0xff0,
                        ChecksumOffset = 0x023F2,
                        ChecksumMirror = 0x25F04 /* Box 2 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x02400,
                        Length = 0xff0,
                        ChecksumOffset = 0x033F2,
                        ChecksumMirror = 0x25F06 /* Box 3 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x03400,
                        Length = 0xff0,
                        ChecksumOffset = 0x043F2,
                        ChecksumMirror = 0x25F08 /* Box 4 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x04400,
                        Length = 0xff0,
                        ChecksumOffset = 0x053F2,
                        ChecksumMirror = 0x25F0A /* Box 5 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x05400,
                        Length = 0xff0,
                        ChecksumOffset = 0x063F2,
                        ChecksumMirror = 0x25F0C /* Box 6 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x06400,
                        Length = 0xff0,
                        ChecksumOffset = 0x073F2,
                        ChecksumMirror = 0x25F0E /* Box 7 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x07400,
                        Length = 0xff0,
                        ChecksumOffset = 0x083F2,
                        ChecksumMirror = 0x25F10 /* Box 8 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x08400,
                        Length = 0xff0,
                        ChecksumOffset = 0x093F2,
                        ChecksumMirror = 0x25F12 /* Box 9 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x09400,
                        Length = 0xff0,
                        ChecksumOffset = 0x0A3F2,
                        ChecksumMirror = 0x25F14 /* Box 10 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0A400,
                        Length = 0xff0,
                        ChecksumOffset = 0x0B3F2,
                        ChecksumMirror = 0x25F16 /* Box 11 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0B400,
                        Length = 0xff0,
                        ChecksumOffset = 0x0C3F2,
                        ChecksumMirror = 0x25F18 /* Box 12 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0C400,
                        Length = 0xff0,
                        ChecksumOffset = 0x0D3F2,
                        ChecksumMirror = 0x25F1A /* Box 13 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0D400,
                        Length = 0xff0,
                        ChecksumOffset = 0x0E3F2,
                        ChecksumMirror = 0x25F1C /* Box 14 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0E400,
                        Length = 0xff0,
                        ChecksumOffset = 0x0F3F2,
                        ChecksumMirror = 0x25F1E /* Box 15 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x0F400,
                        Length = 0xff0,
                        ChecksumOffset = 0x103F2,
                        ChecksumMirror = 0x25F20 /* Box 16 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x10400,
                        Length = 0xff0,
                        ChecksumOffset = 0x113F2,
                        ChecksumMirror = 0x25F22 /* Box 17 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x11400,
                        Length = 0xff0,
                        ChecksumOffset = 0x123F2,
                        ChecksumMirror = 0x25F24 /* Box 18 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x12400,
                        Length = 0xff0,
                        ChecksumOffset = 0x133F2,
                        ChecksumMirror = 0x25F26 /* Box 19 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x13400,
                        Length = 0xff0,
                        ChecksumOffset = 0x143F2,
                        ChecksumMirror = 0x25F28 /* Box 20 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x14400,
                        Length = 0xff0,
                        ChecksumOffset = 0x153F2,
                        ChecksumMirror = 0x25F2A /* Box 21 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x15400,
                        Length = 0xff0,
                        ChecksumOffset = 0x163F2,
                        ChecksumMirror = 0x25F2C /* Box 22 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x16400,
                        Length = 0xff0,
                        ChecksumOffset = 0x173F2,
                        ChecksumMirror = 0x25F2E /* Box 23 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x17400,
                        Length = 0xff0,
                        ChecksumOffset = 0x183F2,
                        ChecksumMirror = 0x25F30 /* Box 24 */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x18400,
                        Length = 0x9ec,
                        ChecksumOffset = 0x18DEE,
                        ChecksumMirror = 0x25F32 /* Inventory */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x18E00,
                        Length = 0x534,
                        ChecksumOffset = 0x19336,
                        ChecksumMirror = 0x25F34 /* Party Pokemon */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x19400,
                        Length = 0xb0,
                        ChecksumOffset = 0x194B2,
                        ChecksumMirror = 0x25F36 /* Trainer Data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x19500,
                        Length = 0xa8,
                        ChecksumOffset = 0x195AA,
                        ChecksumMirror = 0x25F38 /* Trainer Position */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x19600,
                        Length = 0x1338,
                        ChecksumOffset = 0x1A93A,
                        ChecksumMirror = 0x25F3A /* Unity Tower and survey stuff */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1AA00,
                        Length = 0x7c4,
                        ChecksumOffset = 0x1B1C6,
                        ChecksumMirror = 0x25F3C /* Pal Pad Player Data (30d) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1B200,
                        Length = 0xd54,
                        ChecksumOffset = 0x1BF56,
                        ChecksumMirror = 0x25F3E /* Pal Pad Friend Data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1C000,
                        Length = 0x94,
                        ChecksumOffset = 0x1C096,
                        ChecksumMirror = 0x25F40 /* C-Gear */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1C100,
                        Length = 0x658,
                        ChecksumOffset = 0x1C75A,
                        ChecksumMirror = 0x25F42 /* Card Signature Block & ???? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1C800,
                        Length = 0xa94,
                        ChecksumOffset = 0x1D296,
                        ChecksumMirror = 0x25F44 /* Mystery Gift */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1D300,
                        Length = 0x1ac,
                        ChecksumOffset = 0x1D4AE,
                        ChecksumMirror = 0x25F46 /* Dream World Stuff (Catalog) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1D500,
                        Length = 0x3ec,
                        ChecksumOffset = 0x1D8EE,
                        ChecksumMirror = 0x25F48 /* Chatter */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1D900,
                        Length = 0x5c,
                        ChecksumOffset = 0x1D95E,
                        ChecksumMirror = 0x25F4A /* Adventure data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1DA00,
                        Length = 0x1e0,
                        ChecksumOffset = 0x1DBE2,
                        ChecksumMirror = 0x25F4C /* Trainer Card Records */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1DC00,
                        Length = 0xa8,
                        ChecksumOffset = 0x1DCAA,
                        ChecksumMirror = 0x25F4E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1DD00,
                        Length = 0x460,
                        ChecksumOffset = 0x1E162,
                        ChecksumMirror = 0x25F50 /* (40d) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1E200,
                        Length = 0x1400,
                        ChecksumOffset = 0x1F602,
                        ChecksumMirror = 0x25F52 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1F700,
                        Length = 0x2a4,
                        ChecksumOffset = 0x1F9A6,
                        ChecksumMirror = 0x25F54 /* Contains flags and references for downloaded data (Musical) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1FA00,
                        Length = 0xe0,
                        ChecksumOffset = 0x1FAE2,
                        ChecksumMirror = 0x25F56 /* Fused Reshiram/Zekrom Storage */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1FB00,
                        Length = 0x34c,
                        ChecksumOffset = 0x1FE4E,
                        ChecksumMirror = 0x25F58 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x1FF00,
                        Length = 0x4e0,
                        ChecksumOffset = 0x203E2,
                        ChecksumMirror = 0x25F5A /* Const Data Block and Event Flag Block (0x35E is the split) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20400,
                        Length = 0xf8,
                        ChecksumOffset = 0x204FA,
                        ChecksumMirror = 0x25F5C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20500,
                        Length = 0x2fc,
                        ChecksumOffset = 0x207FE,
                        ChecksumMirror = 0x25F5E /* Tournament Block */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20800,
                        Length = 0x94,
                        ChecksumOffset = 0x20896,
                        ChecksumMirror = 0x25F60 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20900,
                        Length = 0x35c,
                        ChecksumOffset = 0x20C5E,
                        ChecksumMirror = 0x25F62 /* Battle Box Block */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20D00,
                        Length = 0x1d4,
                        ChecksumOffset = 0x20ED6,
                        ChecksumMirror = 0x25F64 /* Daycare Block (50d) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x20F00,
                        Length = 0x1e0,
                        ChecksumOffset = 0x201E2,
                        ChecksumMirror = 0x25F66 /* Strength Boulder Status Block */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21100,
                        Length = 0xf0,
                        ChecksumOffset = 0x211F2,
                        ChecksumMirror = 0x25F68 /* Badge Flags, Money, Trainer Sayings */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21200,
                        Length = 0x1b4,
                        ChecksumOffset = 0x213B6,
                        ChecksumMirror = 0x25F6A /* Entralink (Level & Powers etc) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21400,
                        Length = 0x4dc,
                        ChecksumOffset = 0x218DE,
                        ChecksumMirror = 0x25F6C /* Pokedex */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21900,
                        Length = 0x34,
                        ChecksumOffset = 0x21936,
                        ChecksumMirror = 0x25F6E
                        /* Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21A00,
                        Length = 0x3c,
                        ChecksumOffset = 0x21A3E,
                        ChecksumMirror = 0x25F70 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21B00,
                        Length = 0x1ac,
                        ChecksumOffset = 0x21CAE,
                        ChecksumMirror = 0x25F72 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x21D00,
                        Length = 0xb90,
                        ChecksumOffset = 0x22892,
                        ChecksumMirror = 0x25F74 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x22900,
                        Length = 0xac,
                        ChecksumOffset = 0x229AE,
                        ChecksumMirror = 0x25F76 /* Online Records */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x22A00,
                        Length = 0x850,
                        ChecksumOffset = 0x23252,
                        ChecksumMirror = 0x25F78 /* Area NPC data - encrypted (60d) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23300,
                        Length = 0x284,
                        ChecksumOffset = 0x23586,
                        ChecksumMirror = 0x25F7A /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23600,
                        Length = 0x10,
                        ChecksumOffset = 0x23612,
                        ChecksumMirror = 0x25F7C /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23700,
                        Length = 0xa8,
                        ChecksumOffset = 0x237AA,
                        ChecksumMirror = 0x25F7E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23800,
                        Length = 0x16c,
                        ChecksumOffset = 0x2396E,
                        ChecksumMirror = 0x25F80 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23A00,
                        Length = 0x80,
                        ChecksumOffset = 0x23A82,
                        ChecksumMirror = 0x25F82 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23B00,
                        Length = 0xfc,
                        ChecksumOffset = 0x23BFE,
                        ChecksumMirror = 0x25F84 /* Hollow/Rival Block */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x23C00,
                        Length = 0x16a8,
                        ChecksumOffset = 0x252AA,
                        ChecksumMirror = 0x25F86 /* Join Avenue Block */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x25300,
                        Length = 0x498,
                        ChecksumOffset = 0x2579A,
                        ChecksumMirror = 0x25F88 /* Medal data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x25800,
                        Length = 0x60,
                        ChecksumOffset = 0x25862,
                        ChecksumMirror = 0x25F8A /* Key-related data */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x25900,
                        Length = 0xfc,
                        ChecksumOffset = 0x259FE,
                        ChecksumMirror = 0x25F8C /* (70d) */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x25A00,
                        Length = 0x3e4,
                        ChecksumOffset = 0x25DE6,
                        ChecksumMirror = 0x25F8E /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x25E00,
                        Length = 0xf0,
                        ChecksumOffset = 0x25EF2,
                        ChecksumMirror = 0x25F90 /* ??? */
                    },
                    new BlockInfo5
                    {
                        Offset = 0x25F00,
                        Length = 0x94,
                        ChecksumOffset = 0x25FA2,
                        ChecksumMirror = 0x25FA2 /* Checksum Block (73d) */
                    }
                };
            }
            else
            {
                Blocks = new BlockInfo5[] {};
            }
            #endregion
        }

        private const int Box = 0x400;
        private const int Party = 0x18E00;
        private readonly int BattleBox;
        private const int Trainer = 0x19400;
        private const int Wondercard = 0x1C800;
        private const int wcSeed = 0x1D290;

        private bool BW;
        private bool B2W2;

        private void setChecksums()
        {
            // Check for invalid block lengths
            if (Blocks.Length < 3) // arbitrary...
            {
                Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                return;
            }
            // Apply checksums
            for (int i = 0; i < Blocks.Length; i++)
            {
                byte[] array = Data.Skip(Blocks[i].Offset).Take(Blocks[i].Length).ToArray();
                ushort chk = ccitt16(array);
                BitConverter.GetBytes(chk).CopyTo(Data, Blocks[i].ChecksumOffset);
                BitConverter.GetBytes(chk).CopyTo(Data, Blocks[i].ChecksumMirror);
            }
        }

        public byte[] Write()
        {
            setChecksums();
            return Data;
        }

        public int PartyCount
        {
            get { return Data[Party]; }
            set { Data[Party] = (byte)value; }
        }

        public PK5[] BoxData
        {
            get
            {
                PK5[] data = new PK5[24 * 30];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK5Stored(Box + i/30 * 0x10 + PK5.SIZE_STORED * i);
                    data[i].Identifier = $"B{(i / 30 + 1).ToString("00")}:{(i % 30 + 1).ToString("00")}";
                }
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != 24 * 30)
                    throw new ArgumentException("Expected 720, got " + value.Length);

                for (int i = 0; i < value.Length; i++)
                    setPK5Stored(value[i], Box + i/30 * 0x10 + PK5.SIZE_STORED * i);
            }
        }
        public PK5[] PartyData
        {
            get
            {
                PK5[] data = new PK5[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = getPK5Party(Party + 8 + PK5.SIZE_PARTY * i);
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0 || value.Length > 6)
                    throw new ArgumentException("Expected 1-6, got " + value.Length);
                if (value[0].Species == 0)
                    throw new ArgumentException("Can't have an empty first slot." + value.Length);

                PK5[] newParty = value.Where(pk => pk.Species != 0).ToArray();

                PartyCount = newParty.Length;
                Array.Resize(ref newParty, 6);

                for (int i = PartyCount; i < newParty.Length; i++)
                    newParty[i] = new PK5();
                for (int i = 0; i < newParty.Length; i++)
                    setPK5Party(newParty[i], Party + 8 + PK5.SIZE_PARTY * i);
            }
        }
        public PK5[] BattleBoxData
        {
            get
            {
                PK5[] data = new PK5[6];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK5Stored(BattleBox + PK5.SIZE_STORED * i);
                    if (data[i].Species == 0)
                        return data.Take(i).ToArray();
                }
                return data;
            }
        }

        public class MysteryGift
        {
            public readonly PGF[] Cards = new PGF[12];
            public readonly bool[] UsedFlags = new bool[0x800];
            public uint Seed;
        }
        public MysteryGift WondercardInfo
        {
            get
            {
                uint seed = BitConverter.ToUInt32(Data, wcSeed);
                MysteryGift Info = new MysteryGift { Seed = seed };
                byte[] wcData = Data.Skip(Wondercard).Take(0xA90).ToArray(); // Encrypted, Decrypt
                for (int i = 0; i < wcData.Length; i += 2)
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(wcData, i) ^ LCRNG(ref seed) >> 16)).CopyTo(wcData, i);

                // 0x100 Bytes for Used Flags
                for (int i = 0; i < Info.UsedFlags.Length; i++)
                    Info.UsedFlags[i] = (wcData[i/8] >> i%8 & 0x1) == 1;
                // 12 PGFs
                for (int i = 0; i < Info.Cards.Length; i++)
                    Info.Cards[i] = new PGF(Data.Skip(0x100 + i*PGF.Size).Take(PGF.Size).ToArray());

                return Info;
            }
            set
            {
                MysteryGift Info = value;
                byte[] wcData = new byte[0xA90];

                // Toss back into byte[]
                for (int i = 0; i < Info.UsedFlags.Length; i++)
                    if (Info.UsedFlags[i])
                        wcData[i/8] |= (byte)(1 << (i & 7));
                for (int i = 0; i < Info.Cards.Length; i++)
                    Info.Cards[i].Data.CopyTo(wcData, 0x100 + i*PGF.Size);

                // Decrypted, Encrypt
                uint seed = Info.Seed;
                for (int i = 0; i < wcData.Length; i += 2)
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(wcData, i) ^ LCRNG(ref seed) >> 16)).CopyTo(wcData, i);
                BitConverter.GetBytes(Info.Seed).CopyTo(Data, wcSeed);
            }
        }

        public PK5 getPK5Party(int offset)
        {
            return new PK5(decryptArray(getData(offset, PK5.SIZE_PARTY)));
        }
        public PK5 getPK5Stored(int offset)
        {
            return new PK5(decryptArray(getData(offset, PK5.SIZE_STORED)));
        }
        public void setPK5Party(PK5 pk5, int offset)
        {
            if (pk5 == null) return;

            setData(pk5.EncryptedPartyData, offset);
            Edited = true;
        }
        public void setPK5Stored(PK5 pk5, int offset)
        {
            if (pk5 == null) return;

            setData(pk5.EncryptedBoxData, offset);
            Edited = true;
        }
        public byte[] getData(int Offset, int Length)
        {
            return Data.Skip(Offset).Take(Length).ToArray();
        }
        public void setData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
            Edited = true;
        }
    }
}
