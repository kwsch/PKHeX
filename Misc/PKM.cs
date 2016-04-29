using System;
using System.Linq;

namespace PKHeX
{
    public class PKM // Past Gen
    {
        internal static uint LCRNG(uint seed)
        {
            const uint a = 0x41C64E6D;
            const uint c = 0x00006073;

            return seed * a + c;
        }
        internal static uint LCRNG(ref uint seed)
        {
            const uint a = 0x41C64E6D;
            const uint c = 0x00006073;

            return seed = seed * a + c;
        }
        
        internal static readonly string[] speclang_ja = Util.getStringList("species", "ja");
        internal static readonly string[] speclang_en = Util.getStringList("species", "en");
        internal static readonly string[] speclang_fr = Util.getStringList("species", "fr");
        internal static readonly string[] speclang_it = Util.getStringList("species", "it");
        internal static readonly string[] speclang_de = Util.getStringList("species", "de");
        internal static readonly string[] speclang_es = Util.getStringList("species", "es");

        internal static string TrimFromFFFF(string input)
        {
            int index = input.IndexOf((char)0xFFFF);
            return index < 0 ? input : input.Substring(0, index);
        }

        // Past Gen Manipulation
        internal static readonly byte[][] blockPosition =
        {
            new byte[] {0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3},
            new byte[] {1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2},
            new byte[] {2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1},
            new byte[] {3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0},
        };
        internal static readonly byte[] blockPositionInvert =
        {
            0, 1, 2, 4, 3, 5, 6, 7, 12, 18, 13, 19, 8, 10, 14, 20, 16, 22, 9, 11, 15, 21, 17, 23
        };
        internal static byte[] shuffleArray(byte[] data, uint sv)
        {
            byte[] sdata = new byte[PK4.SIZE_PARTY];
            Array.Copy(data, sdata, 8); // Copy unshuffled bytes

            // Shuffle Away!
            for (int block = 0; block < 4; block++)
                Array.Copy(data, 8 + 32 * blockPosition[block][sv], sdata, 8 + 32 * block, 32);

            // Fill the Battle Stats back
            if (data.Length > 136)
                Array.Copy(data, 136, sdata, 136, 100);

            return sdata;
        }
        internal static byte[] decryptArray(byte[] ekm, uint seed = 0x10000)
        {
            byte[] pkm = (byte[])ekm.Clone();

            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint sv = ((pv & 0x3E000) >> 0xD) % 24;

            seed = seed > 0xFFFF ? pv : seed;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 136; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkm, i);

            // Deshuffle
            pkm = shuffleArray(pkm, sv);

            // Decrypt the Party Stats
            seed = pv;
            if (pkm.Length <= 136) return pkm;
            for (int i = 136; i < 236; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkm, i);

            return pkm;
        }
        internal static byte[] encryptArray(byte[] pkm, uint seed = 0x10000)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint sv = ((pv & 0x3E000) >> 0xD) % 24;

            byte[] ekm = (byte[])pkm.Clone();

            ekm = shuffleArray(ekm, blockPositionInvert[sv]);

            seed = seed > 0xFFFF ? pv : seed;

            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 136; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(ekm, i);

            // If no party stats, return.
            if (ekm.Length <= 136) return ekm;

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 136; i < 236; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(ekm, i);

            // Done
            return ekm;
        }

        /// <summary>Calculates the CRC16-CCITT checksum over an input byte array.</summary>
        /// <param name="chunk">Input byte array</param>
        /// <returns>Checksum</returns>
        internal static ushort ccitt16(byte[] chunk)
        {
            ushort crc = 0xFFFF;
            foreach (byte t in chunk)
            {
                crc ^= (ushort)(t << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)(crc << 1 ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }

        internal static int getUnownForm(uint PID)
        {
            byte[] data = BitConverter.GetBytes(PID);
            return (((data[3] & 3) << 6) + ((data[2] & 3) << 4) + ((data[1] & 3) << 2) + ((data[0] & 3) << 0)) % 28;
        }

        internal static ushort val2charG4(ushort val)
        {
            int index = Array.IndexOf(G4Values, val);
            return index > -1 ? G4Chars[index] : (ushort)0xFFFF;
        }
        internal static ushort char2valG4(ushort chr)
        {
            int index = Array.IndexOf(G4Chars, chr);
            return index > -1 ? G4Values[index] : (ushort)0xFFFF;
        }

        internal static string array2strG4(byte[] strdata)
        {
            string s = "";
            for (int i = 0; i < strdata.Length; i += 2)
            {
                ushort val = BitConverter.ToUInt16(strdata, i);
                if (val == 0xFFFF) break;
                ushort chr = val2charG4(val);
                if (chr == 0xFFFF) break;
                s += (char)chr;
            }
            return s;
        }
        internal static byte[] str2arrayG4(string str)
        {
            byte[] strdata = new byte[str.Length * 2 + 2]; // +2 for 0xFFFF
            for (int i = 0; i < str.Length; i++)
            {
                ushort chr = str[i];
                ushort val = char2valG4(chr);
                if (val == 0xFFFF || chr == 0xFFFF)
                { Array.Resize(ref strdata, i * 2); break; }
                BitConverter.GetBytes(val).CopyTo(strdata, i * 2);
            }
            BitConverter.GetBytes((ushort)0xFFFF).CopyTo(strdata, strdata.Length - 2);
            return strdata;
        }

        // Gen3 && 3->4 Conversion has two character tables, and translates to the same character map.
        internal static ushort getG4Val(byte val, bool jp) { return jp ? G34_4J[val] : G34_4E[val]; }
        internal static ushort getG3Char(byte val, bool jp) { return val2charG4(getG4Val(val, jp)); }
        internal static byte setG3Char(ushort chr, bool jp)
        {
            int index = Array.IndexOf(jp ? G34_4J : G34_4E, char2valG4(chr));
            return (byte)(index > -1 ? index : 0xFF);
        }
        internal static string getG3Str(byte[] strdata, bool jp)
        {
            return strdata
                .TakeWhile(val => val < 247) // Take valid values
                .Select(val => getG3Char(val, jp)) // Convert to Unicode
                .TakeWhile(chr => chr != 0xFF) // Stop if Terminator
                .Aggregate("", (current, chr) => current + (char)chr);
        }
        internal static byte[] setG3Str(string str, bool jp)
        {
            byte[] strdata = new byte[str.Length + 1]; // +1 for 0xFF
            for (int i = 0; i < str.Length; i++)
            {
                ushort chr = str[i];
                byte val = setG3Char(chr, jp);
                if (val == 0xFF || chr == 0xFF)
                { Array.Resize(ref strdata, i); break; }
                strdata[i] = val;
            }
            strdata[str.Length] = 0xFF;
            return strdata;
        }

        internal static int getG4Species(int g3index)
        {
            int index = Array.IndexOf(oldindex, g3index);
            return newindex[index > -1 ? index : 0];
        }
        internal static int getG3Species(int g4index)
        {
            int index = Array.IndexOf(newindex, g4index);
            return oldindex[index > -1 ? index : 0];
        }

        internal static int getGender(int species, uint PID)
        {
            int genderratio = PKX.Personal[species].Gender;
            switch (genderratio)
            {
                case 255: return 2;
                case 254: return 1;
                case 0: return 0;
                default: return (PID & 0xFF) <= genderratio ? 1 : 0;
            }
        }

        internal static string getSpeciesName(int species, int language)
        {
            try
            {
                return new[]
                {
                    speclang_ja[species].ToUpper(),
                    speclang_en[species].ToUpper(),
                    speclang_fr[species].ToUpper(),
                    speclang_it[species].ToUpper(),
                    speclang_de[species].ToUpper(),
                    speclang_ja[species].ToLower(),
                    speclang_es[species].ToUpper(),
                }[language];
            }
            catch { return ""; }
        }

        #region Gen 3 Species Table
        internal static readonly int[] newindex =
        {
            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
            31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,
            59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,77,79,80,81,82,83,84,85,86,
            87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,
            111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,
            132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,
            153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,
            174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,
            195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
            216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,
            237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,
            258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,290,291,292,
            276,277,285,286,327,278,279,283,284,320,321,300,301,352,343,344,299,324,302,339,340,
            370,341,342,349,350,318,319,328,329,330,296,297,309,310,322,323,363,364,365,331,332,
            361,362,337,338,298,325,326,311,312,303,307,308,333,334,360,355,356,315,287,288,289,
            316,317,357,293,294,295,366,367,368,359,353,354,336,335,369,304,305,306,351,313,314,
            345,346,347,348,280,281,282,371,372,373,374,375,376,377,378,379,382,383,384,380,381,
            385,386,358,
        };
        internal static readonly int[] oldindex =
        {
            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
            31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,
            59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,
            87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,
            111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,
            132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,
            153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,
            174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,
            195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
            216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,
            237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,277,278,279,280,281,282,
            283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,
            304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,
            325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,
            346,347,348,349,350,351,352,353,354,355,356,357,358,359,360,361,362,363,364,365,366,
            367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,
            388,389,390,391,392,393,394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,
            409,410,411,
        };
        #endregion
        #region Gen 3 Ability Table
        internal static readonly byte[][] Gen3Abilities = 
        {
            new byte[] {0x00, 0x00}, // 000
            new byte[] {0x41, 0x41}, // 001
            new byte[] {0x41, 0x41}, // 002
            new byte[] {0x41, 0x41}, // 003
            new byte[] {0x42, 0x42}, // 004
            new byte[] {0x42, 0x42}, // 005
            new byte[] {0x42, 0x42}, // 006
            new byte[] {0x43, 0x43}, // 007
            new byte[] {0x43, 0x43}, // 008
            new byte[] {0x43, 0x43}, // 009
            new byte[] {0x13, 0x13}, // 010
            new byte[] {0x3d, 0x3d}, // 011
            new byte[] {0x0e, 0x0e}, // 012
            new byte[] {0x13, 0x13}, // 013
            new byte[] {0x3d, 0x3d}, // 014
            new byte[] {0x44, 0x44}, // 015
            new byte[] {0x33, 0x33}, // 016
            new byte[] {0x33, 0x33}, // 017
            new byte[] {0x33, 0x33}, // 018
            new byte[] {0x32, 0x3e}, // 019
            new byte[] {0x32, 0x3e}, // 020
            new byte[] {0x33, 0x33}, // 021
            new byte[] {0x33, 0x33}, // 022
            new byte[] {0x3d, 0x16}, // 023
            new byte[] {0x3d, 0x16}, // 024
            new byte[] {0x09, 0x09}, // 025
            new byte[] {0x09, 0x09}, // 026
            new byte[] {0x08, 0x08}, // 027
            new byte[] {0x08, 0x08}, // 028
            new byte[] {0x26, 0x26}, // 029
            new byte[] {0x26, 0x26}, // 030
            new byte[] {0x26, 0x26}, // 031
            new byte[] {0x26, 0x26}, // 032
            new byte[] {0x26, 0x26}, // 033
            new byte[] {0x26, 0x26}, // 034
            new byte[] {0x38, 0x38}, // 035
            new byte[] {0x38, 0x38}, // 036
            new byte[] {0x12, 0x12}, // 037
            new byte[] {0x12, 0x12}, // 038
            new byte[] {0x38, 0x38}, // 039
            new byte[] {0x38, 0x38}, // 040
            new byte[] {0x27, 0x27}, // 041
            new byte[] {0x27, 0x27}, // 042
            new byte[] {0x22, 0x22}, // 043
            new byte[] {0x22, 0x22}, // 044
            new byte[] {0x22, 0x22}, // 045
            new byte[] {0x1b, 0x1b}, // 046
            new byte[] {0x1b, 0x1b}, // 047
            new byte[] {0x0e, 0x0e}, // 048
            new byte[] {0x13, 0x13}, // 049
            new byte[] {0x08, 0x47}, // 050
            new byte[] {0x08, 0x47}, // 051
            new byte[] {0x35, 0x35}, // 052
            new byte[] {0x07, 0x07}, // 053
            new byte[] {0x06, 0x0d}, // 054
            new byte[] {0x06, 0x0d}, // 055
            new byte[] {0x48, 0x48}, // 056
            new byte[] {0x48, 0x48}, // 057
            new byte[] {0x16, 0x12}, // 058
            new byte[] {0x16, 0x12}, // 059
            new byte[] {0x06, 0x0b}, // 060
            new byte[] {0x06, 0x0b}, // 061
            new byte[] {0x06, 0x0b}, // 062
            new byte[] {0x1c, 0x27}, // 063
            new byte[] {0x1c, 0x27}, // 064
            new byte[] {0x1c, 0x27}, // 065
            new byte[] {0x3e, 0x3e}, // 066
            new byte[] {0x3e, 0x3e}, // 067
            new byte[] {0x3e, 0x3e}, // 068
            new byte[] {0x22, 0x22}, // 069
            new byte[] {0x22, 0x22}, // 070
            new byte[] {0x22, 0x22}, // 071
            new byte[] {0x1d, 0x40}, // 072
            new byte[] {0x1d, 0x40}, // 073
            new byte[] {0x45, 0x05}, // 074
            new byte[] {0x45, 0x05}, // 075
            new byte[] {0x45, 0x05}, // 076
            new byte[] {0x32, 0x12}, // 077
            new byte[] {0x32, 0x12}, // 078
            new byte[] {0x0c, 0x14}, // 079
            new byte[] {0x0c, 0x14}, // 080
            new byte[] {0x2a, 0x05}, // 081
            new byte[] {0x2a, 0x05}, // 082
            new byte[] {0x33, 0x27}, // 083
            new byte[] {0x32, 0x30}, // 084
            new byte[] {0x32, 0x30}, // 085
            new byte[] {0x2f, 0x2f}, // 086
            new byte[] {0x2f, 0x2f}, // 087
            new byte[] {0x01, 0x3c}, // 088
            new byte[] {0x01, 0x3c}, // 089
            new byte[] {0x4b, 0x4b}, // 090
            new byte[] {0x4b, 0x4b}, // 091
            new byte[] {0x1a, 0x1a}, // 092
            new byte[] {0x1a, 0x1a}, // 093
            new byte[] {0x1a, 0x1a}, // 094
            new byte[] {0x45, 0x05}, // 095
            new byte[] {0x0f, 0x0f}, // 096
            new byte[] {0x0f, 0x0f}, // 097
            new byte[] {0x34, 0x4b}, // 098
            new byte[] {0x34, 0x4b}, // 099
            new byte[] {0x2b, 0x09}, // 100
            new byte[] {0x2b, 0x09}, // 101
            new byte[] {0x22, 0x22}, // 102
            new byte[] {0x22, 0x22}, // 103
            new byte[] {0x45, 0x1f}, // 104
            new byte[] {0x45, 0x1f}, // 105
            new byte[] {0x07, 0x07}, // 106
            new byte[] {0x33, 0x33}, // 107
            new byte[] {0x0c, 0x14}, // 108
            new byte[] {0x1a, 0x1a}, // 109
            new byte[] {0x1a, 0x1a}, // 110
            new byte[] {0x45, 0x1f}, // 111
            new byte[] {0x45, 0x1f}, // 112
            new byte[] {0x1e, 0x20}, // 113
            new byte[] {0x22, 0x22}, // 114
            new byte[] {0x30, 0x30}, // 115
            new byte[] {0x21, 0x21}, // 116
            new byte[] {0x26, 0x26}, // 117
            new byte[] {0x21, 0x29}, // 118
            new byte[] {0x21, 0x29}, // 119
            new byte[] {0x23, 0x1e}, // 120
            new byte[] {0x23, 0x1e}, // 121
            new byte[] {0x2b, 0x2b}, // 122
            new byte[] {0x44, 0x44}, // 123
            new byte[] {0x0c, 0x0c}, // 124
            new byte[] {0x09, 0x09}, // 125
            new byte[] {0x31, 0x31}, // 126
            new byte[] {0x34, 0x34}, // 127
            new byte[] {0x16, 0x16}, // 128
            new byte[] {0x21, 0x21}, // 129
            new byte[] {0x16, 0x16}, // 130
            new byte[] {0x0b, 0x4b}, // 131
            new byte[] {0x07, 0x07}, // 132
            new byte[] {0x32, 0x32}, // 133
            new byte[] {0x0b, 0x0b}, // 134
            new byte[] {0x0a, 0x0a}, // 135
            new byte[] {0x12, 0x12}, // 136
            new byte[] {0x24, 0x24}, // 137
            new byte[] {0x21, 0x4b}, // 138
            new byte[] {0x21, 0x4b}, // 139
            new byte[] {0x21, 0x04}, // 140
            new byte[] {0x21, 0x04}, // 141
            new byte[] {0x45, 0x2e}, // 142
            new byte[] {0x11, 0x2f}, // 143
            new byte[] {0x2e, 0x2e}, // 144
            new byte[] {0x2e, 0x2e}, // 145
            new byte[] {0x2e, 0x2e}, // 146
            new byte[] {0x3d, 0x3d}, // 147
            new byte[] {0x3d, 0x3d}, // 148
            new byte[] {0x27, 0x27}, // 149
            new byte[] {0x2e, 0x2e}, // 150
            new byte[] {0x1c, 0x1c}, // 151
            new byte[] {0x41, 0x41}, // 152
            new byte[] {0x41, 0x41}, // 153
            new byte[] {0x41, 0x41}, // 154
            new byte[] {0x42, 0x42}, // 155
            new byte[] {0x42, 0x42}, // 156
            new byte[] {0x42, 0x42}, // 157
            new byte[] {0x43, 0x43}, // 158
            new byte[] {0x43, 0x43}, // 159
            new byte[] {0x43, 0x43}, // 160
            new byte[] {0x32, 0x33}, // 161
            new byte[] {0x32, 0x33}, // 162
            new byte[] {0x0f, 0x33}, // 163
            new byte[] {0x0f, 0x33}, // 164
            new byte[] {0x44, 0x30}, // 165
            new byte[] {0x44, 0x30}, // 166
            new byte[] {0x0f, 0x44}, // 167
            new byte[] {0x0f, 0x44}, // 168
            new byte[] {0x27, 0x27}, // 169
            new byte[] {0x0a, 0x23}, // 170
            new byte[] {0x0a, 0x23}, // 171
            new byte[] {0x09, 0x09}, // 172
            new byte[] {0x38, 0x38}, // 173
            new byte[] {0x38, 0x38}, // 174
            new byte[] {0x37, 0x20}, // 175
            new byte[] {0x37, 0x20}, // 176
            new byte[] {0x1c, 0x30}, // 177
            new byte[] {0x1c, 0x30}, // 178
            new byte[] {0x09, 0x09}, // 179
            new byte[] {0x09, 0x09}, // 180
            new byte[] {0x09, 0x09}, // 181
            new byte[] {0x22, 0x22}, // 182
            new byte[] {0x2f, 0x25}, // 183
            new byte[] {0x2f, 0x25}, // 184
            new byte[] {0x45, 0x05}, // 185
            new byte[] {0x06, 0x0b}, // 186
            new byte[] {0x22, 0x22}, // 187
            new byte[] {0x22, 0x22}, // 188
            new byte[] {0x22, 0x22}, // 189
            new byte[] {0x32, 0x35}, // 190
            new byte[] {0x22, 0x22}, // 191
            new byte[] {0x22, 0x22}, // 192
            new byte[] {0x03, 0x0e}, // 193
            new byte[] {0x06, 0x0b}, // 194
            new byte[] {0x06, 0x0b}, // 195
            new byte[] {0x1c, 0x1c}, // 196
            new byte[] {0x1c, 0x1c}, // 197
            new byte[] {0x0f, 0x0f}, // 198
            new byte[] {0x0c, 0x14}, // 199
            new byte[] {0x1a, 0x1a}, // 200
            new byte[] {0x1a, 0x1a}, // 201
            new byte[] {0x17, 0x17}, // 202
            new byte[] {0x27, 0x30}, // 203
            new byte[] {0x05, 0x05}, // 204
            new byte[] {0x05, 0x05}, // 205
            new byte[] {0x20, 0x32}, // 206
            new byte[] {0x08, 0x34}, // 207
            new byte[] {0x45, 0x05}, // 208
            new byte[] {0x16, 0x32}, // 209
            new byte[] {0x16, 0x16}, // 210
            new byte[] {0x21, 0x26}, // 211
            new byte[] {0x44, 0x44}, // 212
            new byte[] {0x05, 0x05}, // 213
            new byte[] {0x44, 0x3e}, // 214
            new byte[] {0x27, 0x33}, // 215
            new byte[] {0x35, 0x35}, // 216
            new byte[] {0x3e, 0x3e}, // 217
            new byte[] {0x28, 0x31}, // 218
            new byte[] {0x28, 0x31}, // 219
            new byte[] {0x0c, 0x0c}, // 220
            new byte[] {0x0c, 0x0c}, // 221
            new byte[] {0x37, 0x1e}, // 222
            new byte[] {0x37, 0x37}, // 223
            new byte[] {0x15, 0x15}, // 224
            new byte[] {0x37, 0x48}, // 225
            new byte[] {0x21, 0x0b}, // 226
            new byte[] {0x33, 0x05}, // 227
            new byte[] {0x30, 0x12}, // 228
            new byte[] {0x30, 0x12}, // 229
            new byte[] {0x21, 0x21}, // 230
            new byte[] {0x35, 0x35}, // 231
            new byte[] {0x05, 0x05}, // 232
            new byte[] {0x24, 0x24}, // 233
            new byte[] {0x16, 0x16}, // 234
            new byte[] {0x14, 0x14}, // 235
            new byte[] {0x3e, 0x3e}, // 236
            new byte[] {0x16, 0x16}, // 237
            new byte[] {0x0c, 0x0c}, // 238
            new byte[] {0x09, 0x09}, // 239
            new byte[] {0x31, 0x31}, // 240
            new byte[] {0x2f, 0x2f}, // 241
            new byte[] {0x1e, 0x20}, // 242
            new byte[] {0x2e, 0x2e}, // 243
            new byte[] {0x2e, 0x2e}, // 244
            new byte[] {0x2e, 0x2e}, // 245
            new byte[] {0x3e, 0x3e}, // 246
            new byte[] {0x3d, 0x3d}, // 247
            new byte[] {0x2d, 0x2d}, // 248
            new byte[] {0x2e, 0x2e}, // 249
            new byte[] {0x2e, 0x2e}, // 250
            new byte[] {0x1e, 0x1e}, // 251
            new byte[] {0x41, 0x41}, // 252
            new byte[] {0x41, 0x41}, // 253
            new byte[] {0x41, 0x41}, // 254
            new byte[] {0x42, 0x42}, // 255
            new byte[] {0x42, 0x42}, // 256
            new byte[] {0x42, 0x42}, // 257
            new byte[] {0x43, 0x43}, // 258
            new byte[] {0x43, 0x43}, // 259
            new byte[] {0x43, 0x43}, // 260
            new byte[] {0x32, 0x32}, // 261
            new byte[] {0x16, 0x16}, // 262
            new byte[] {0x35, 0x35}, // 263
            new byte[] {0x35, 0x35}, // 264
            new byte[] {0x13, 0x13}, // 265
            new byte[] {0x3d, 0x3d}, // 266
            new byte[] {0x44, 0x44}, // 267
            new byte[] {0x3d, 0x3d}, // 268
            new byte[] {0x13, 0x13}, // 269
            new byte[] {0x21, 0x2c}, // 270
            new byte[] {0x21, 0x2c}, // 271
            new byte[] {0x21, 0x2c}, // 272
            new byte[] {0x22, 0x30}, // 273
            new byte[] {0x22, 0x30}, // 274
            new byte[] {0x22, 0x30}, // 275
            new byte[] {0x3e, 0x3e}, // 276
            new byte[] {0x3e, 0x3e}, // 277
            new byte[] {0x33, 0x33}, // 278
            new byte[] {0x33, 0x33}, // 279
            new byte[] {0x1c, 0x24}, // 280
            new byte[] {0x1c, 0x24}, // 281
            new byte[] {0x1c, 0x24}, // 282
            new byte[] {0x21, 0x21}, // 283
            new byte[] {0x16, 0x16}, // 284
            new byte[] {0x1b, 0x1b}, // 285
            new byte[] {0x1b, 0x1b}, // 286
            new byte[] {0x36, 0x36}, // 287
            new byte[] {0x48, 0x48}, // 288
            new byte[] {0x36, 0x36}, // 289
            new byte[] {0x0e, 0x0e}, // 290
            new byte[] {0x03, 0x03}, // 291
            new byte[] {0x19, 0x19}, // 292
            new byte[] {0x2b, 0x2b}, // 293
            new byte[] {0x2b, 0x2b}, // 294
            new byte[] {0x2b, 0x2b}, // 295
            new byte[] {0x2f, 0x3e}, // 296
            new byte[] {0x2f, 0x3e}, // 297
            new byte[] {0x2f, 0x3e}, // 298
            new byte[] {0x05, 0x2a}, // 299
            new byte[] {0x38, 0x38}, // 300
            new byte[] {0x38, 0x38}, // 301
            new byte[] {0x33, 0x33}, // 302
            new byte[] {0x34, 0x16}, // 303
            new byte[] {0x45, 0x05}, // 304
            new byte[] {0x45, 0x05}, // 305
            new byte[] {0x45, 0x05}, // 306
            new byte[] {0x4a, 0x4a}, // 307
            new byte[] {0x4a, 0x4a}, // 308
            new byte[] {0x09, 0x1f}, // 309
            new byte[] {0x09, 0x1f}, // 310
            new byte[] {0x39, 0x39}, // 311
            new byte[] {0x3a, 0x3a}, // 312
            new byte[] {0x23, 0x44}, // 313
            new byte[] {0x0c, 0x0c}, // 314
            new byte[] {0x1e, 0x26}, // 315
            new byte[] {0x40, 0x3c}, // 316
            new byte[] {0x40, 0x3c}, // 317
            new byte[] {0x18, 0x18}, // 318
            new byte[] {0x18, 0x18}, // 319
            new byte[] {0x29, 0x0c}, // 320
            new byte[] {0x29, 0x0c}, // 321
            new byte[] {0x0c, 0x0c}, // 322
            new byte[] {0x28, 0x28}, // 323
            new byte[] {0x49, 0x49}, // 324
            new byte[] {0x2f, 0x14}, // 325
            new byte[] {0x2f, 0x14}, // 326
            new byte[] {0x14, 0x14}, // 327
            new byte[] {0x34, 0x47}, // 328
            new byte[] {0x1a, 0x1a}, // 329
            new byte[] {0x1a, 0x1a}, // 330
            new byte[] {0x08, 0x08}, // 331
            new byte[] {0x08, 0x08}, // 332
            new byte[] {0x1e, 0x1e}, // 333
            new byte[] {0x1e, 0x1e}, // 334
            new byte[] {0x11, 0x11}, // 335
            new byte[] {0x3d, 0x3d}, // 336
            new byte[] {0x1a, 0x1a}, // 337
            new byte[] {0x1a, 0x1a}, // 338
            new byte[] {0x0c, 0x0c}, // 339
            new byte[] {0x0c, 0x0c}, // 340
            new byte[] {0x34, 0x4b}, // 341
            new byte[] {0x34, 0x4b}, // 342
            new byte[] {0x1a, 0x1a}, // 343
            new byte[] {0x1a, 0x1a}, // 344
            new byte[] {0x15, 0x15}, // 345
            new byte[] {0x15, 0x15}, // 346
            new byte[] {0x04, 0x04}, // 347
            new byte[] {0x04, 0x04}, // 348
            new byte[] {0x21, 0x21}, // 349
            new byte[] {0x3f, 0x3f}, // 350
            new byte[] {0x3b, 0x3b}, // 351
            new byte[] {0x10, 0x10}, // 352
            new byte[] {0x0f, 0x0f}, // 353
            new byte[] {0x0f, 0x0f}, // 354
            new byte[] {0x1a, 0x1a}, // 355
            new byte[] {0x2e, 0x2e}, // 356
            new byte[] {0x22, 0x22}, // 357
            new byte[] {0x1a, 0x1a}, // 358
            new byte[] {0x2e, 0x2e}, // 359
            new byte[] {0x17, 0x17}, // 360
            new byte[] {0x27, 0x27}, // 361
            new byte[] {0x27, 0x27}, // 362
            new byte[] {0x2f, 0x2f}, // 363
            new byte[] {0x2f, 0x2f}, // 364
            new byte[] {0x2f, 0x2f}, // 365
            new byte[] {0x4b, 0x4b}, // 366
            new byte[] {0x21, 0x21}, // 367
            new byte[] {0x21, 0x21}, // 368
            new byte[] {0x21, 0x45}, // 369
            new byte[] {0x21, 0x21}, // 370
            new byte[] {0x45, 0x45}, // 371
            new byte[] {0x45, 0x45}, // 372
            new byte[] {0x16, 0x16}, // 373
            new byte[] {0x1d, 0x1d}, // 374
            new byte[] {0x1d, 0x1d}, // 375
            new byte[] {0x1d, 0x1d}, // 376
            new byte[] {0x1d, 0x1d}, // 377
            new byte[] {0x1d, 0x1d}, // 378
            new byte[] {0x1d, 0x1d}, // 379
            new byte[] {0x1a, 0x1a}, // 380
            new byte[] {0x1a, 0x1a}, // 381
            new byte[] {0x02, 0x02}, // 382
            new byte[] {0x46, 0x46}, // 383
            new byte[] {0x4c, 0x4c}, // 384
            new byte[] {0x20, 0x20}, // 385
            new byte[] {0x2e, 0x2e}, // 386
        };
        #endregion
        #region Gen 3/4 Character Tables (Val->Unicode)
        internal static readonly ushort[] G4Values =
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75,
            76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
            102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123,
            124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145,
            146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
            168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
            190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211,
            212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234,
            235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256,
            257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278,
            279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300,
            301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322,
            323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344,
            345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366,
            367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388,
            389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410,
            411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432,
            433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454,
            455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476,
            477, 478, 479, 480, 481, 488, 489, 490, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034,
            1035, 1036, 1037, 1038, 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053,
            1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071,
            1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089,
            1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 1104, 1105, 1106, 1107,
            1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125,
            1126, 1127, 1128, 1129, 1130, 1131, 1132, 1133, 1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143,
            1144, 1145, 1146, 1147, 1148, 1149, 1150, 1151, 1152, 1153, 1154, 1155, 1156, 1157, 1158, 1159, 1160, 1161,
            1162, 1163, 1164, 1165, 1166, 1167, 1168, 1169, 1170, 1171, 1172, 1173, 1174, 1175, 1176, 1177, 1178, 1179,
            1180, 1181, 1182, 1183, 1184, 1185, 1186, 1187, 1188, 1189, 1190, 1191, 1192, 1193, 1194, 1195, 1196, 1197,
            1198, 1199, 1200, 1201, 1202, 1203, 1204, 1205, 1206, 1207, 1208, 1209, 1210, 1211, 1212, 1213, 1214, 1215,
            1216, 1217, 1218, 1219, 1220, 1221, 1222, 1223, 1224, 1225, 1226, 1227, 1228, 1229, 1230, 1231, 1232, 1233,
            1234, 1235, 1236, 1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250, 1251,
            1252, 1253, 1254, 1255, 1256, 1257, 1258, 1259, 1260, 1261, 1262, 1263, 1264, 1265, 1266, 1267, 1268, 1269,
            1270, 1271, 1272, 1273, 1274, 1275, 1276, 1277, 1278, 1279, 1280, 1281, 1282, 1283, 1284, 1285, 1286, 1287,
            1288, 1289, 1290, 1291, 1292, 1293, 1294, 1295, 1296, 1297, 1298, 1299, 1300, 1301, 1302, 1303, 1304, 1305,
            1306, 1307, 1308, 1309, 1310, 1311, 1312, 1313, 1314, 1315, 1316, 1317, 1318, 1319, 1320, 1321, 1322, 1323,
            1324, 1325, 1326, 1327, 1328, 1329, 1330, 1331, 1332, 1333, 1334, 1335, 1336, 1337, 1338, 1339, 1340, 1341,
            1342, 1343, 1344, 1345, 1346, 1347, 1348, 1349, 1350, 1351, 1352, 1353, 1354, 1355, 1356, 1357, 1358, 1359,
            1360, 1361, 1362, 1363, 1364, 1365, 1366, 1367, 1368, 1369, 1370, 1371, 1372, 1373, 1374, 1375, 1376, 1377,
            1378, 1379, 1380, 1381, 1382, 1383, 1384, 1385, 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395,
            1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412, 1413,
            1414, 1415, 1416, 1417, 1418, 1419, 1420, 1421, 1422, 1423, 1424, 1425, 1426, 1427, 1428, 1429, 1430, 1431,
            1432, 1433, 1434, 1435, 1436, 1437, 1438, 1439, 1440, 1441, 1442, 1443, 1444, 1445, 1446, 1447, 1448, 1449,
            1450, 1451, 1452, 1453, 1454, 1455, 1456, 1457, 1458, 1459, 1460, 1461, 1462, 1463, 1464, 1465, 1466, 1467,
            1468, 1469, 1470, 1471, 1472, 1473, 1474, 1475, 1476, 1477, 1478, 1479, 1480, 1481, 1482, 1483, 1484, 1485,
            1486, 1487, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503,
            1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 1515, 1516, 1517, 1518, 1519, 1520, 1521,
            1522, 1523, 1524, 1525, 1526, 1527, 1528, 1529, 1530, 1531, 1532, 1533, 1534, 1535, 1536, 1537, 1538, 1539,
            1540, 1541, 1542, 1543, 1544, 1545, 1546, 1547, 1548, 1549, 1550, 1551, 1552, 1553, 1554, 1555, 1556, 1557,
            1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568, 1569, 1570, 1571, 1572, 1573, 1574, 1575,
            1576, 1577, 1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 1589, 1590, 1591, 1592, 1593,
            1594, 1595, 1596, 1597, 1598, 1599, 1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608, 1609, 1610, 1611,
            1612, 1613, 1614, 1615, 1616, 1617, 1618, 1619, 1620, 1621, 1622, 1623, 1624, 1625, 1626, 1627, 1628, 1629,
            1630, 1631, 1632, 1633, 1634, 1635, 1636, 1637, 1638, 1639, 1640, 1641, 1642, 1643, 1644, 1645, 1646, 1647,
            1648, 1649, 1650, 1651, 1652, 1653, 1654, 1655, 1656, 1657, 1658, 1659, 1660, 1661, 1662, 1663, 1664, 1665,
            1666, 1667, 1668, 1669, 1670, 1671, 1672, 1673, 1674, 1675, 1676, 1677, 1678, 1679, 1680, 1681, 1682, 1683,
            1684, 1685, 1686, 1687, 1688, 1689, 1690, 1691, 1692, 1693, 1694, 1695, 1696, 1697, 1698, 1699, 1700, 1701,
            1702, 1703, 1704, 1705, 1706, 1707, 1708, 1709, 1710, 1711, 1712, 1713, 1714, 1715, 1716, 1717, 1718, 1719,
            1720, 1721, 1722, 1723, 1724, 1725, 1726, 1727, 1728, 1729, 1730, 1731, 1732, 1733, 1734, 1735, 1736, 1737,
            1738, 1739, 1740, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748, 1749, 1750, 1751, 1752, 1753, 1754, 1755,
            1756, 1757, 1758, 1759, 1760, 1761, 1762, 1763, 1764, 1765, 1766, 1767, 1768, 1769, 1770, 1771, 1772, 1773,
            1774, 1775, 1776, 1777, 1778, 1779, 1780, 1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1791,
            1792, 1793, 1794, 1795, 1796, 1797, 1798, 1799, 1800, 1801, 1802, 1803, 1804, 1805, 1806, 1807, 1808, 1809,
            1810, 1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823, 1824, 1825, 1826, 1827,
            1828, 1829, 1830, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839, 1840, 1841, 1842, 1843, 1844, 1845,
            1846, 1847, 1848, 1849, 1850, 1851, 1852, 1853, 1854, 1855, 1856, 1857, 1858, 1859, 1860, 1861, 1862, 1863,
            1864, 1865, 1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875, 1876, 1877, 1878, 1879, 1880, 1881,
            1882, 1883, 1884, 1885, 1886, 1887, 1888, 1889, 1890, 1891, 1892, 1893, 1894, 1895, 1896, 1897, 1898, 1899,
            1900, 1901, 1902, 1903, 1904, 1905, 1906, 1907, 1908, 1909, 1910, 1911, 1912, 1913, 1914, 1915, 1916, 1917,
            1918, 1919, 1920, 1921, 1922, 1923, 1924, 1925, 1926, 1927, 1928, 1929, 1930, 1931, 1932, 1933, 1934, 1935,
            1936, 1937, 1938, 1939, 1940, 1941, 1942, 1943, 1944, 1945, 1946, 1947, 1948, 1949, 1950, 1951, 1952, 1953,
            1954, 1955, 1956, 1957, 1958, 1959, 1960, 1961, 1962, 1963, 1964, 1965, 1966, 1967, 1968, 1969, 1970, 1971,
            1972, 1973, 1974, 1975, 1976, 1977, 1978, 1979, 1980, 1981, 1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989,
            1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007,
            2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025,
            2026, 2027, 2028, 2029, 2030, 2031, 2032, 2033, 2034, 2035, 2036, 2037, 2038, 2039, 2040, 2041, 2042, 2043,
            2044, 2045, 2046, 2047, 2048, 2049, 2050, 2051, 2052, 2053, 2054, 2055, 2056, 2057, 2058, 2059, 2060, 2061,
            2062, 2063, 2064, 2065, 2066, 2067, 2068, 2069, 2070, 2071, 2072, 2073, 2074, 2075, 2076, 2077, 2078, 2079,
            2080, 2081, 2082, 2083, 2084, 2085, 2086, 2087, 2088, 2089, 2090, 2091, 2092, 2093, 2094, 2095, 2096, 2097,
            2098, 2099, 2100, 2101, 2102, 2103, 2104, 2105, 2106, 2107, 2108, 2109, 2110, 2111, 2112, 2113, 2114, 2115,
            2116, 2117, 2118, 2119, 2120, 2121, 2122, 2123, 2124, 2125, 2126, 2127, 2128, 2129, 2130, 2131, 2132, 2133,
            2134, 2135, 2136, 2137, 2138, 2139, 2140, 2141, 2142, 2143, 2144, 2145, 2146, 2147, 2148, 2149, 2150, 2151,
            2152, 2153, 2154, 2155, 2156, 2157, 2158, 2159, 2160, 2161, 2162, 2163, 2164, 2165, 2166, 2167, 2168, 2169,
            2170, 2171, 2172, 2173, 2174, 2175, 2176, 2177, 2178, 2179, 2180, 2181, 2182, 2183, 2184, 2185, 2186, 2187,
            2188, 2189, 2190, 2191, 2192, 2193, 2194, 2195, 2196, 2197, 2198, 2199, 2200, 2201, 2202, 2203, 2204, 2205,
            2206, 2207, 2208, 2209, 2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219, 2220, 2221, 2222, 2223,
            2224, 2225, 2226, 2227, 2228, 2229, 2230, 2231, 2232, 2233, 2234, 2235, 2236, 2237, 2238, 2239, 2240, 2241,
            2242, 2243, 2244, 2245, 2246, 2247, 2248, 2249, 2250, 2251, 2252, 2253, 2254, 2255, 2256, 2257, 2258, 2259,
            2260, 2261, 2262, 2263, 2264, 2265, 2266, 2267, 2268, 2269, 2270, 2271, 2272, 2273, 2274, 2275, 2276, 2277,
            2278, 2279, 2280, 2281, 2282, 2283, 2284, 2285, 2286, 2287, 2288, 2289, 2290, 2291, 2292, 2293, 2294, 2295,
            2296, 2297, 2298, 2299, 2300, 2301, 2302, 2303, 2304, 2305, 2306, 2307, 2308, 2309, 2310, 2311, 2312, 2313,
            2314, 2315, 2316, 2317, 2318, 2319, 2320, 2321, 2322, 2323, 2324, 2325, 2326, 2327, 2328, 2329, 2330, 2331,
            2332, 2333, 2334, 2335, 2336, 2337, 2338, 2339, 2340, 2341, 2342, 2343, 2344, 2345, 2346, 2347, 2348, 2349,
            2350, 2351, 2352, 2353, 2354, 2355, 2356, 2357, 2358, 2359, 2360, 2361, 2362, 2363, 2364, 2365, 2366, 2367,
            2368, 2369, 2370, 2371, 2372, 2373, 2374, 2375, 2376, 2377, 2378, 2379, 2380, 2381, 2382, 2383, 2384, 2385,
            2386, 2387, 2388, 2389, 2390, 2391, 2392, 2393, 2394, 2395, 2396, 2397, 2398, 2399, 2400, 2401, 2402, 2403,
            2404, 2405, 2406, 2407, 2408, 2409, 2410, 2411, 2412, 2413, 2414, 2415, 2416, 2417, 2418, 2419, 2420, 2421,
            2422, 2423, 2424, 2425, 2426, 2427, 2428, 2429, 2430, 2431, 2432, 2433, 2434, 2435, 2436, 2437, 2438, 2439,
            2440, 2441, 2442, 2443, 2444, 2445, 2446, 2447, 2448, 2449, 2450, 2451, 2452, 2453, 2454, 2455, 2456, 2457,
            2458, 2459, 2460, 2461, 2462, 2463, 2464, 2465, 2466, 2467, 2468, 2469, 2470, 2471, 2472, 2473, 2474, 2475,
            2476, 2477, 2478, 2479, 2480, 2481, 2482, 2483, 2484, 2485, 2486, 2487, 2488, 2489, 2490, 2491, 2492, 2493,
            2494, 2495, 2496, 2497, 2498, 2499, 2500, 2501, 2502, 2503, 2504, 2505, 2506, 2507, 2508, 2509, 2510, 2511,
            2512, 2513, 2514, 2515, 2516, 2517, 2518, 2519, 2520, 2521, 2522, 2523, 2524, 2525, 2526, 2527, 2528, 2529,
            2530, 2531, 2532, 2533, 2534, 2535, 2536, 2537, 2538, 2539, 2540, 2541, 2542, 2543, 2544, 2545, 2546, 2547,
            2548, 2549, 2550, 2551, 2552, 2553, 2554, 2555, 2556, 2557, 2558, 2559, 2560, 2561, 2562, 2563, 2564, 2565,
            2566, 2567, 2568, 2569, 2570, 2571, 2572, 2573, 2574, 2575, 2576, 2577, 2578, 2579, 2580, 2581, 2582, 2583,
            2584, 2585, 2586, 2587, 2588, 2589, 2590, 2591, 2592, 2593, 2594, 2595, 2596, 2597, 2598, 2599, 2600, 2601,
            2602, 2603, 2604, 2605, 2606, 2607, 2608, 2609, 2610, 2611, 2612, 2613, 2614, 2615, 2616, 2617, 2618, 2619,
            2620, 2621, 2622, 2623, 2624, 2625, 2626, 2627, 2628, 2629, 2630, 2631, 2632, 2633, 2634, 2635, 2636, 2637,
            2638, 2639, 2640, 2641, 2642, 2643, 2644, 2645, 2646, 2647, 2648, 2649, 2650, 2651, 2652, 2653, 2654, 2655,
            2656, 2657, 2658, 2659, 2660, 2661, 2662, 2663, 2664, 2665, 2666, 2667, 2668, 2669, 2670, 2671, 2672, 2673,
            2674, 2675, 2676, 2677, 2678, 2679, 2680, 2681, 2682, 2683, 2684, 2685, 2686, 2687, 2688, 2689, 2690, 2691,
            2692, 2693, 2694, 2695, 2696, 2697, 2698, 2699, 2700, 2701, 2702, 2703, 2704, 2705, 2706, 2707, 2708, 2709,
            2710, 2711, 2712, 2713, 2714, 2715, 2716, 2717, 2718, 2719, 2720, 2721, 2722, 2723, 2724, 2725, 2726, 2727,
            2728, 2729, 2730, 2731, 2732, 2733, 2734, 2735, 2736, 2737, 2738, 2739, 2740, 2741, 2742, 2743, 2744, 2745,
            2746, 2747, 2748, 2749, 2750, 2751, 2752, 2753, 2754, 2755, 2756, 2757, 2758, 2759, 2760, 2761, 2762, 2763,
            2764, 2765, 2766, 2767, 2768, 2769, 2770, 2771, 2772, 2773, 2774, 2775, 2776, 2777, 2778, 2779, 2780, 2781,
            2782, 2783, 2784, 2785, 2786, 2787, 2788, 2789, 2790, 2791, 2792, 2793, 2794, 2795, 2796, 2797, 2798, 2799,
            2800, 2801, 2802, 2803, 2804, 2805, 2806, 2807, 2808, 2809, 2810, 2811, 2812, 2813, 2814, 2815, 2816, 2817,
            2818, 2819, 2820, 2821, 2822, 2823, 2824, 2825, 2826, 2827, 2828, 2829, 2830, 2831, 2832, 2833, 2834, 2835,
            2836, 2837, 2838, 2839, 2840, 2841, 2842, 2843, 2844, 2845, 2846, 2847, 2848, 2849, 2850, 2851, 2852, 2853,
            2854, 2855, 2856, 2857, 2858, 2859, 2860, 2861, 2862, 2863, 2864, 2865, 2866, 2867, 2868, 2869, 2870, 2871,
            2872, 2873, 2874, 2875, 2876, 2877, 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885, 2886, 2887, 2888, 2889,
            2890, 2891, 2892, 2893, 2894, 2895, 2896, 2897, 2898, 2899, 2900, 2901, 2902, 2903, 2904, 2905, 2906, 2907,
            2908, 2909, 2910, 2911, 2912, 2913, 2914, 2915, 2916, 2917, 2918, 2919, 2920, 2921, 2922, 2923, 2924, 2925,
            2926, 2927, 2928, 2929, 2930, 2931, 2932, 2933, 2934, 2935, 2936, 2937, 2938, 2939, 2940, 2941, 2942, 2943,
            2944, 2945, 2946, 2947, 2948, 2949, 2950, 2951, 2952, 2953, 2954, 2955, 2956, 2957, 2958, 2959, 2960, 2961,
            2962, 2963, 2964, 2965, 2966, 2967, 2968, 2969, 2970, 2971, 2972, 2973, 2974, 2975, 2976, 2977, 2978, 2979,
            2980, 2981, 2982, 2983, 2984, 2985, 2986, 2987, 2988, 2989, 2990, 2991, 2992, 2993, 2994, 2995, 2996, 2997,
            2998, 2999, 3000, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010, 3011, 3012, 3013, 3014, 3015,
            3016, 3017, 3018, 3019, 3020, 3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028, 3029, 3030, 3031, 3032, 3033,
            3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3044, 3045, 3046, 3047, 3048, 3049, 3050, 3051,
            3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3061, 3062, 3063, 3064, 3065, 3066, 3067, 3068, 3069,
            3070, 3071, 3072, 3073, 3074, 3075, 3076, 3077, 3078, 3079, 3080, 3081, 3082, 3083, 3084, 3085, 3086, 3087,
            3088, 3089, 3090, 3091, 3092, 3093, 3094, 3095, 3096, 3097, 3098, 3099, 3100, 3101, 3102, 3103, 3104, 3105,
            3106, 3107, 3108, 3109, 3110, 3111, 3112, 3113, 3114, 3115, 3116, 3117, 3118, 3119, 3120, 3121, 3122, 3123,
            3124, 3125, 3126, 3127, 3128, 3129, 3130, 3131, 3132, 3133, 3134, 3135, 3136, 3137, 3138, 3139, 3140, 3141,
            3142, 3143, 3144, 3145, 3146, 3147, 3148, 3149, 3150, 3151, 3152, 3153, 3154, 3155, 3156, 3157, 3158, 3159,
            3160, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3169, 3170, 3171, 3172, 3173, 3174, 3175, 3176, 3177,
            3178, 3179, 3180, 3181, 3182, 3183, 3184, 3185, 3186, 3187, 3188, 3189, 3190, 3191, 3192, 3193, 3194, 3195,
            3196, 3197, 3198, 3199, 3200, 3201, 3202, 3203, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213,
            3214, 3215, 3216, 3217, 3218, 3219, 3220, 3221, 3222, 3223, 3224, 3225, 3226, 3227, 3228, 3229, 3230, 3231,
            3232, 3233, 3234, 3235, 3236, 3237, 3238, 3239, 3240, 3241, 3242, 3243, 3244, 3245, 3246, 3247, 3248, 3249,
            3250, 3251, 3252, 3253, 3254, 3255, 3256, 3257, 3258, 3259, 3260, 3261, 3262, 3263, 3264, 3265, 3266, 3267,
            3268, 3269, 3270, 3271, 3272, 3273, 3274, 3275, 3276, 3277, 3278, 3279, 3280, 3281, 3282, 3283, 3284, 3285,
            3286, 3287, 3288, 3289, 3290, 3291, 3292, 3293, 3294, 3295, 3296, 3297, 3298, 3299, 3300, 3301, 3302, 3303,
            3304, 3305, 3306, 3307, 3308, 3309, 3310, 3311, 3312, 3313, 3314, 3315, 3316, 3317, 3318, 3319, 3320, 3321,
            3322, 3323, 3324, 3325, 3326, 3327, 3328, 3329, 3330, 3331, 3332, 3333, 3334, 3335, 3336, 3337, 3338, 3339,
            3340, 3341, 3342, 3343, 3344, 3345, 3346, 3347, 3348, 3349, 3350, 3351, 3352, 3353, 3354, 3355, 3356, 3357,
            3358, 3359, 3360, 3361, 3362, 3363, 3364, 3365, 3366, 3367, 3368, 3369, 3370, 3371, 3372, 3373, 3374, 3377,
            3378, 3379, 3380, 3381, 3382, 3383, 3384, 3385, 3386, 3387, 3388, 3389, 3390, 3391, 3392, 3393, 3394, 3395,
            3396, 3397, 3398, 3399, 3400, 3401, 3402, 3403, 3404, 3405, 3406, 3407, 3408, 3409, 3425, 3426, 3427, 3428,
            3429, 65535
        };
        internal static readonly ushort[] G4Chars =
        {
            12288, 12353, 12354, 12355, 12356, 12357, 12358, 12359, 12360, 12361, 12362, 12363,
            12364, 12365, 12366, 12367, 12368, 12369, 12370, 12371, 12372, 12373, 12374, 12375, 12376, 12377, 12378,
            12379, 12380, 12381, 12382, 12383, 12384, 12385, 12386, 12387, 12388, 12389, 12390, 12391, 12392, 12393,
            12394, 12395, 12396, 12397, 12398, 12399, 12400, 12401, 12402, 12403, 12404, 12405, 12406, 12407, 12408,
            12409, 12410, 12411, 12412, 12413, 12414, 12415, 12416, 12417, 12418, 12419, 12420, 12421, 12422, 12423,
            12424, 12425, 12426, 12427, 12428, 12429, 12431, 12434, 12435, 12449, 12450, 12451, 12452, 12453, 12454,
            12455, 12456, 12457, 12458, 12459, 12460, 12461, 12462, 12463, 12464, 12465, 12466, 12467, 12468, 12469,
            12470, 12471, 12472, 12473, 12474, 12475, 12476, 12477, 12478, 12479, 12480, 12481, 12482, 12483, 12484,
            12485, 12486, 12487, 12488, 12489, 12490, 12491, 12492, 12493, 12494, 12495, 12496, 12497, 12498, 12499,
            12500, 12501, 12502, 12503, 12504, 12505, 12506, 12507, 12508, 12509, 12510, 12511, 12512, 12513, 12514,
            12515, 12516, 12517, 12518, 12519, 12520, 12521, 12522, 12523, 12524, 12525, 12527, 12530, 12531, 65296,
            65297, 65298, 65299, 65300, 65301, 65302, 65303, 65304, 65305, 65313, 65314, 65315, 65316, 65317, 65318,
            65319, 65320, 65321, 65322, 65323, 65324, 65325, 65326, 65327, 65328, 65329, 65330, 65331, 65332, 65333,
            65334, 65335, 65336, 65337, 65338, 65345, 65346, 65347, 65348, 65349, 65350, 65351, 65352, 65353, 65354,
            65355, 65356, 65357, 65358, 65359, 65360, 65361, 65362, 65363, 65364, 65365, 65366, 65367, 65368, 65369,
            65370, 65281, 65311, 12289, 12290, 8943, 12539, 65295, 12300, 12301, 12302, 12303, 65288, 65289, 9325, 9326,
            65291, 65293, 9319, 9320, 65309, 65370, 65306, 65307, 65294, 65292, 9327, 9328, 9329, 9330, 9331, 9332, 9333,
            9334, 9335, 9336, 65312, 9337, 65285, 9338, 9339, 9341, 10052, 9739, 9812, 9813, 9738, 8663, 8664, 9790, 165,
            9800, 9801, 9802, 9803, 9804, 9805, 9806, 9807, 8592, 8593, 8595, 8594, 8227, 65286, 48, 49, 50, 51, 52, 53,
            54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87,
            88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
            117, 118, 119, 120, 121, 122, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
            230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
            252, 253, 254, 255, 338, 339, 350, 351, 170, 186, 185, 178, 179, 36, 161, 191, 33, 63, 44, 46, 9324, 65381,
            47, 8216, 8217, 8220, 8221, 8222, 12298, 12299, 40, 41, 9794, 9792, 43, 45, 42, 35, 61, 38, 126, 58, 59,
            9327, 9328, 9329, 9330, 9331, 9332, 9333, 9334, 9335, 9336, 64, 9337, 37, 9338, 9339, 9340, 9341, 9342, 9343,
            9344, 9345, 9346, 9347, 9348, 32, 9349, 9350, 9351, 176, 95, 65343, 44032, 44033, 44036, 44039, 44040, 44041,
            44042, 44048, 44049, 44050, 44051, 44052, 44053, 44054, 44055, 44057, 44058, 44059, 44060, 44061, 44064,
            44068, 44076, 44077, 44079, 44080, 44081, 44088, 44089, 44092, 44096, 44107, 44109, 44116, 44120, 44124,
            44144, 44145, 44148, 44151, 44152, 44154, 44160, 44161, 44163, 44164, 44165, 44166, 44169, 44170, 44171,
            44172, 44176, 44180, 44188, 44189, 44191, 44192, 44193, 44200, 44201, 44202, 44204, 44207, 44208, 44216,
            44217, 44219, 44220, 44221, 44225, 44228, 44232, 44236, 44245, 44247, 44256, 44257, 44260, 44263, 44264,
            44266, 44268, 44271, 44272, 44273, 44275, 44277, 44278, 44284, 44285, 44288, 44292, 44294, 44300, 44301,
            44303, 44305, 44312, 44316, 44320, 44329, 44332, 44333, 44340, 44341, 44344, 44348, 44356, 44357, 44359,
            44361, 44368, 44372, 44376, 44385, 44387, 44396, 44397, 44400, 44403, 44404, 44405, 44406, 44411, 44412,
            44413, 44415, 44417, 44418, 44424, 44425, 44428, 44432, 44444, 44445, 44452, 44471, 44480, 44481, 44484,
            44488, 44496, 44497, 44499, 44508, 44512, 44516, 44536, 44537, 44540, 44543, 44544, 44545, 44552, 44553,
            44555, 44557, 44564, 44592, 44593, 44596, 44599, 44600, 44602, 44608, 44609, 44611, 44613, 44614, 44618,
            44620, 44621, 44622, 44624, 44628, 44630, 44636, 44637, 44639, 44640, 44641, 44645, 44648, 44649, 44652,
            44656, 44664, 44665, 44667, 44668, 44669, 44676, 44677, 44684, 44732, 44733, 44734, 44736, 44740, 44748,
            44749, 44751, 44752, 44753, 44760, 44761, 44764, 44776, 44779, 44781, 44788, 44792, 44796, 44807, 44808,
            44813, 44816, 44844, 44845, 44848, 44850, 44852, 44860, 44861, 44863, 44865, 44866, 44867, 44872, 44873,
            44880, 44892, 44893, 44900, 44901, 44921, 44928, 44932, 44936, 44944, 44945, 44949, 44956, 44984, 44985,
            44988, 44992, 44999, 45000, 45001, 45003, 45005, 45006, 45012, 45020, 45032, 45033, 45040, 45041, 45044,
            45048, 45056, 45057, 45060, 45068, 45072, 45076, 45084, 45085, 45096, 45124, 45125, 45128, 45130, 45132,
            45134, 45139, 45140, 45141, 45143, 45145, 45149, 45180, 45181, 45184, 45188, 45196, 45197, 45199, 45201,
            45208, 45209, 45210, 45212, 45215, 45216, 45217, 45218, 45224, 45225, 45227, 45228, 45229, 45230, 45231,
            45233, 45235, 45236, 45237, 45240, 45244, 45252, 45253, 45255, 45256, 45257, 45264, 45265, 45268, 45272,
            45280, 45285, 45320, 45321, 45323, 45324, 45328, 45330, 45331, 45336, 45337, 45339, 45340, 45341, 45347,
            45348, 45349, 45352, 45356, 45364, 45365, 45367, 45368, 45369, 45376, 45377, 45380, 45384, 45392, 45393,
            45396, 45397, 45400, 45404, 45408, 45432, 45433, 45436, 45440, 45442, 45448, 45449, 45451, 45453, 45458,
            45459, 45460, 45464, 45468, 45480, 45516, 45520, 45524, 45532, 45533, 45535, 45544, 45545, 45548, 45552,
            45561, 45563, 45565, 45572, 45573, 45576, 45579, 45580, 45588, 45589, 45591, 45593, 45600, 45620, 45628,
            45656, 45660, 45664, 45672, 45673, 45684, 45685, 45692, 45700, 45701, 45705, 45712, 45713, 45716, 45720,
            45721, 45722, 45728, 45729, 45731, 45733, 45734, 45738, 45740, 45744, 45748, 45768, 45769, 45772, 45776,
            45778, 45784, 45785, 45787, 45789, 45794, 45796, 45797, 45798, 45800, 45803, 45804, 45805, 45806, 45807,
            45811, 45812, 45813, 45815, 45816, 45817, 45818, 45819, 45823, 45824, 45825, 45828, 45832, 45840, 45841,
            45843, 45844, 45845, 45852, 45908, 45909, 45910, 45912, 45915, 45916, 45918, 45919, 45924, 45925, 45927,
            45929, 45931, 45934, 45936, 45937, 45940, 45944, 45952, 45953, 45955, 45956, 45957, 45964, 45968, 45972,
            45984, 45985, 45992, 45996, 46020, 46021, 46024, 46027, 46028, 46030, 46032, 46036, 46037, 46039, 46041,
            46043, 46045, 46048, 46052, 46056, 46076, 46096, 46104, 46108, 46112, 46120, 46121, 46123, 46132, 46160,
            46161, 46164, 46168, 46176, 46177, 46179, 46181, 46188, 46208, 46216, 46237, 46244, 46248, 46252, 46261,
            46263, 46265, 46272, 46276, 46280, 46288, 46293, 46300, 46301, 46304, 46307, 46308, 46310, 46316, 46317,
            46319, 46321, 46328, 46356, 46357, 46360, 46363, 46364, 46372, 46373, 46375, 46376, 46377, 46378, 46384,
            46385, 46388, 46392, 46400, 46401, 46403, 46404, 46405, 46411, 46412, 46413, 46416, 46420, 46428, 46429,
            46431, 46432, 46433, 46496, 46497, 46500, 46504, 46506, 46507, 46512, 46513, 46515, 46516, 46517, 46523,
            46524, 46525, 46528, 46532, 46540, 46541, 46543, 46544, 46545, 46552, 46572, 46608, 46609, 46612, 46616,
            46629, 46636, 46644, 46664, 46692, 46696, 46748, 46749, 46752, 46756, 46763, 46764, 46769, 46804, 46832,
            46836, 46840, 46848, 46849, 46853, 46888, 46889, 46892, 46895, 46896, 46904, 46905, 46907, 46916, 46920,
            46924, 46932, 46933, 46944, 46948, 46952, 46960, 46961, 46963, 46965, 46972, 46973, 46976, 46980, 46988,
            46989, 46991, 46992, 46993, 46994, 46998, 46999, 47000, 47001, 47004, 47008, 47016, 47017, 47019, 47020,
            47021, 47028, 47029, 47032, 47047, 47049, 47084, 47085, 47088, 47092, 47100, 47101, 47103, 47104, 47105,
            47111, 47112, 47113, 47116, 47120, 47128, 47129, 47131, 47133, 47140, 47141, 47144, 47148, 47156, 47157,
            47159, 47160, 47161, 47168, 47172, 47185, 47187, 47196, 47197, 47200, 47204, 47212, 47213, 47215, 47217,
            47224, 47228, 47245, 47272, 47280, 47284, 47288, 47296, 47297, 47299, 47301, 47308, 47312, 47316, 47325,
            47327, 47329, 47336, 47337, 47340, 47344, 47352, 47353, 47355, 47357, 47364, 47384, 47392, 47420, 47421,
            47424, 47428, 47436, 47439, 47441, 47448, 47449, 47452, 47456, 47464, 47465, 47467, 47469, 47476, 47477,
            47480, 47484, 47492, 47493, 47495, 47497, 47498, 47501, 47502, 47532, 47533, 47536, 47540, 47548, 47549,
            47551, 47553, 47560, 47561, 47564, 47566, 47567, 47568, 47569, 47570, 47576, 47577, 47579, 47581, 47582,
            47585, 47587, 47588, 47589, 47592, 47596, 47604, 47605, 47607, 47608, 47609, 47610, 47616, 47617, 47624,
            47637, 47672, 47673, 47676, 47680, 47682, 47688, 47689, 47691, 47693, 47694, 47699, 47700, 47701, 47704,
            47708, 47716, 47717, 47719, 47720, 47721, 47728, 47729, 47732, 47736, 47747, 47748, 47749, 47751, 47756,
            47784, 47785, 47787, 47788, 47792, 47794, 47800, 47801, 47803, 47805, 47812, 47816, 47832, 47833, 47868,
            47872, 47876, 47885, 47887, 47889, 47896, 47900, 47904, 47913, 47915, 47924, 47925, 47926, 47928, 47931,
            47932, 47933, 47934, 47940, 47941, 47943, 47945, 47949, 47951, 47952, 47956, 47960, 47969, 47971, 47980,
            48008, 48012, 48016, 48036, 48040, 48044, 48052, 48055, 48064, 48068, 48072, 48080, 48083, 48120, 48121,
            48124, 48127, 48128, 48130, 48136, 48137, 48139, 48140, 48141, 48143, 48145, 48148, 48149, 48150, 48151,
            48152, 48155, 48156, 48157, 48158, 48159, 48164, 48165, 48167, 48169, 48173, 48176, 48177, 48180, 48184,
            48192, 48193, 48195, 48196, 48197, 48201, 48204, 48205, 48208, 48221, 48260, 48261, 48264, 48267, 48268,
            48270, 48276, 48277, 48279, 48281, 48282, 48288, 48289, 48292, 48295, 48296, 48304, 48305, 48307, 48308,
            48309, 48316, 48317, 48320, 48324, 48333, 48335, 48336, 48337, 48341, 48344, 48348, 48372, 48373, 48374,
            48376, 48380, 48388, 48389, 48391, 48393, 48400, 48404, 48420, 48428, 48448, 48456, 48457, 48460, 48464,
            48472, 48473, 48484, 48488, 48512, 48513, 48516, 48519, 48520, 48521, 48522, 48528, 48529, 48531, 48533,
            48537, 48538, 48540, 48548, 48560, 48568, 48596, 48597, 48600, 48604, 48617, 48624, 48628, 48632, 48640,
            48643, 48645, 48652, 48653, 48656, 48660, 48668, 48669, 48671, 48708, 48709, 48712, 48716, 48718, 48724,
            48725, 48727, 48729, 48730, 48731, 48736, 48737, 48740, 48744, 48746, 48752, 48753, 48755, 48756, 48757,
            48763, 48764, 48765, 48768, 48772, 48780, 48781, 48783, 48784, 48785, 48792, 48793, 48808, 48848, 48849,
            48852, 48855, 48856, 48864, 48867, 48868, 48869, 48876, 48897, 48904, 48905, 48920, 48921, 48923, 48924,
            48925, 48960, 48961, 48964, 48968, 48976, 48977, 48981, 49044, 49072, 49093, 49100, 49101, 49104, 49108,
            49116, 49119, 49121, 49212, 49233, 49240, 49244, 49248, 49256, 49257, 49296, 49297, 49300, 49304, 49312,
            49313, 49315, 49317, 49324, 49325, 49327, 49328, 49331, 49332, 49333, 49334, 49340, 49341, 49343, 49344,
            49345, 49349, 49352, 49353, 49356, 49360, 49368, 49369, 49371, 49372, 49373, 49380, 49381, 49384, 49388,
            49396, 49397, 49399, 49401, 49408, 49412, 49416, 49424, 49429, 49436, 49437, 49438, 49439, 49440, 49443,
            49444, 49446, 49447, 49452, 49453, 49455, 49456, 49457, 49462, 49464, 49465, 49468, 49472, 49480, 49481,
            49483, 49484, 49485, 49492, 49493, 49496, 49500, 49508, 49509, 49511, 49512, 49513, 49520, 49524, 49528,
            49541, 49548, 49549, 49550, 49552, 49556, 49558, 49564, 49565, 49567, 49569, 49573, 49576, 49577, 49580,
            49584, 49597, 49604, 49608, 49612, 49620, 49623, 49624, 49632, 49636, 49640, 49648, 49649, 49651, 49660,
            49661, 49664, 49668, 49676, 49677, 49679, 49681, 49688, 49689, 49692, 49695, 49696, 49704, 49705, 49707,
            49709, 49711, 49713, 49714, 49716, 49736, 49744, 49745, 49748, 49752, 49760, 49765, 49772, 49773, 49776,
            49780, 49788, 49789, 49791, 49793, 49800, 49801, 49808, 49816, 49819, 49821, 49828, 49829, 49832, 49836,
            49837, 49844, 49845, 49847, 49849, 49884, 49885, 49888, 49891, 49892, 49899, 49900, 49901, 49903, 49905,
            49910, 49912, 49913, 49915, 49916, 49920, 49928, 49929, 49932, 49933, 49939, 49940, 49941, 49944, 49948,
            49956, 49957, 49960, 49961, 49989, 50024, 50025, 50028, 50032, 50034, 50040, 50041, 50044, 50045, 50052,
            50056, 50060, 50112, 50136, 50137, 50140, 50143, 50144, 50146, 50152, 50153, 50157, 50164, 50165, 50168,
            50184, 50192, 50212, 50220, 50224, 50228, 50236, 50237, 50248, 50276, 50277, 50280, 50284, 50292, 50293,
            50297, 50304, 50324, 50332, 50360, 50364, 50409, 50416, 50417, 50420, 50424, 50426, 50431, 50432, 50433,
            50444, 50448, 50452, 50460, 50472, 50473, 50476, 50480, 50488, 50489, 50491, 50493, 50500, 50501, 50504,
            50505, 50506, 50508, 50509, 50510, 50515, 50516, 50517, 50519, 50520, 50521, 50525, 50526, 50528, 50529,
            50532, 50536, 50544, 50545, 50547, 50548, 50549, 50556, 50557, 50560, 50564, 50567, 50572, 50573, 50575,
            50577, 50581, 50583, 50584, 50588, 50592, 50601, 50612, 50613, 50616, 50617, 50619, 50620, 50621, 50622,
            50628, 50629, 50630, 50631, 50632, 50633, 50634, 50636, 50638, 50640, 50641, 50644, 50648, 50656, 50657,
            50659, 50661, 50668, 50669, 50670, 50672, 50676, 50678, 50679, 50684, 50685, 50686, 50687, 50688, 50689,
            50693, 50694, 50695, 50696, 50700, 50704, 50712, 50713, 50715, 50716, 50724, 50725, 50728, 50732, 50733,
            50734, 50736, 50739, 50740, 50741, 50743, 50745, 50747, 50752, 50753, 50756, 50760, 50768, 50769, 50771,
            50772, 50773, 50780, 50781, 50784, 50796, 50799, 50801, 50808, 50809, 50812, 50816, 50824, 50825, 50827,
            50829, 50836, 50837, 50840, 50844, 50852, 50853, 50855, 50857, 50864, 50865, 50868, 50872, 50873, 50874,
            50880, 50881, 50883, 50885, 50892, 50893, 50896, 50900, 50908, 50909, 50912, 50913, 50920, 50921, 50924,
            50928, 50936, 50937, 50941, 50948, 50949, 50952, 50956, 50964, 50965, 50967, 50969, 50976, 50977, 50980,
            50984, 50992, 50993, 50995, 50997, 50999, 51004, 51005, 51008, 51012, 51018, 51020, 51021, 51023, 51025,
            51026, 51027, 51028, 51029, 51030, 51031, 51032, 51036, 51040, 51048, 51051, 51060, 51061, 51064, 51068,
            51069, 51070, 51075, 51076, 51077, 51079, 51080, 51081, 51082, 51086, 51088, 51089, 51092, 51094, 51095,
            51096, 51098, 51104, 51105, 51107, 51108, 51109, 51110, 51116, 51117, 51120, 51124, 51132, 51133, 51135,
            51136, 51137, 51144, 51145, 51148, 51150, 51152, 51160, 51165, 51172, 51176, 51180, 51200, 51201, 51204,
            51208, 51210, 51216, 51217, 51219, 51221, 51222, 51228, 51229, 51232, 51236, 51244, 51245, 51247, 51249,
            51256, 51260, 51264, 51272, 51273, 51276, 51277, 51284, 51312, 51313, 51316, 51320, 51322, 51328, 51329,
            51331, 51333, 51334, 51335, 51339, 51340, 51341, 51348, 51357, 51359, 51361, 51368, 51388, 51389, 51396,
            51400, 51404, 51412, 51413, 51415, 51417, 51424, 51425, 51428, 51445, 51452, 51453, 51456, 51460, 51461,
            51462, 51468, 51469, 51471, 51473, 51480, 51500, 51508, 51536, 51537, 51540, 51544, 51552, 51553, 51555,
            51564, 51568, 51572, 51580, 51592, 51593, 51596, 51600, 51608, 51609, 51611, 51613, 51648, 51649, 51652,
            51655, 51656, 51658, 51664, 51665, 51667, 51669, 51670, 51673, 51674, 51676, 51677, 51680, 51682, 51684,
            51687, 51692, 51693, 51695, 51696, 51697, 51704, 51705, 51708, 51712, 51720, 51721, 51723, 51724, 51725,
            51732, 51736, 51753, 51788, 51789, 51792, 51796, 51804, 51805, 51807, 51808, 51809, 51816, 51837, 51844,
            51864, 51900, 51901, 51904, 51908, 51916, 51917, 51919, 51921, 51923, 51928, 51929, 51936, 51948, 51956,
            51976, 51984, 51988, 51992, 52000, 52001, 52033, 52040, 52041, 52044, 52048, 52056, 52057, 52061, 52068,
            52088, 52089, 52124, 52152, 52180, 52196, 52199, 52201, 52236, 52237, 52240, 52244, 52252, 52253, 52257,
            52258, 52263, 52264, 52265, 52268, 52270, 52272, 52280, 52281, 52283, 52284, 52285, 52286, 52292, 52293,
            52296, 52300, 52308, 52309, 52311, 52312, 52313, 52320, 52324, 52326, 52328, 52336, 52341, 52376, 52377,
            52380, 52384, 52392, 52393, 52395, 52396, 52397, 52404, 52405, 52408, 52412, 52420, 52421, 52423, 52425,
            52432, 52436, 52452, 52460, 52464, 52481, 52488, 52489, 52492, 52496, 52504, 52505, 52507, 52509, 52516,
            52520, 52524, 52537, 52572, 52576, 52580, 52588, 52589, 52591, 52593, 52600, 52616, 52628, 52629, 52632,
            52636, 52644, 52645, 52647, 52649, 52656, 52676, 52684, 52688, 52712, 52716, 52720, 52728, 52729, 52731,
            52733, 52740, 52744, 52748, 52756, 52761, 52768, 52769, 52772, 52776, 52784, 52785, 52787, 52789, 52824,
            52825, 52828, 52831, 52832, 52833, 52840, 52841, 52843, 52845, 52852, 52853, 52856, 52860, 52868, 52869,
            52871, 52873, 52880, 52881, 52884, 52888, 52896, 52897, 52899, 52900, 52901, 52908, 52909, 52929, 52964,
            52965, 52968, 52971, 52972, 52980, 52981, 52983, 52984, 52985, 52992, 52993, 52996, 53000, 53008, 53009,
            53011, 53013, 53020, 53024, 53028, 53036, 53037, 53039, 53040, 53041, 53048, 53076, 53077, 53080, 53084,
            53092, 53093, 53095, 53097, 53104, 53105, 53108, 53112, 53120, 53125, 53132, 53153, 53160, 53168, 53188,
            53216, 53217, 53220, 53224, 53232, 53233, 53235, 53237, 53244, 53248, 53252, 53265, 53272, 53293, 53300,
            53301, 53304, 53308, 53316, 53317, 53319, 53321, 53328, 53332, 53336, 53344, 53356, 53357, 53360, 53364,
            53372, 53373, 53377, 53412, 53413, 53416, 53420, 53428, 53429, 53431, 53433, 53440, 53441, 53444, 53448,
            53449, 53456, 53457, 53459, 53460, 53461, 53468, 53469, 53472, 53476, 53484, 53485, 53487, 53488, 53489,
            53496, 53517, 53552, 53553, 53556, 53560, 53562, 53568, 53569, 53571, 53572, 53573, 53580, 53581, 53584,
            53588, 53596, 53597, 53599, 53601, 53608, 53612, 53628, 53636, 53640, 53664, 53665, 53668, 53672, 53680,
            53681, 53683, 53685, 53690, 53692, 53696, 53720, 53748, 53752, 53767, 53769, 53776, 53804, 53805, 53808,
            53812, 53820, 53821, 53823, 53825, 53832, 53852, 53860, 53888, 53889, 53892, 53896, 53904, 53905, 53909,
            53916, 53920, 53924, 53932, 53937, 53944, 53945, 53948, 53951, 53952, 53954, 53960, 53961, 53963, 53972,
            53976, 53980, 53988, 53989, 54000, 54001, 54004, 54008, 54016, 54017, 54019, 54021, 54028, 54029, 54030,
            54032, 54036, 54038, 54044, 54045, 54047, 54048, 54049, 54053, 54056, 54057, 54060, 54064, 54072, 54073,
            54075, 54076, 54077, 54084, 54085, 54140, 54141, 54144, 54148, 54156, 54157, 54159, 54160, 54161, 54168,
            54169, 54172, 54176, 54184, 54185, 54187, 54189, 54196, 54200, 54204, 54212, 54213, 54216, 54217, 54224,
            54232, 54241, 54243, 54252, 54253, 54256, 54260, 54268, 54269, 54271, 54273, 54280, 54301, 54336, 54340,
            54364, 54368, 54372, 54381, 54383, 54392, 54393, 54396, 54399, 54400, 54402, 54408, 54409, 54411, 54413,
            54420, 54441, 54476, 54480, 54484, 54492, 54495, 54504, 54508, 54512, 54520, 54523, 54525, 54532, 54536,
            54540, 54548, 54549, 54551, 54588, 54589, 54592, 54596, 54604, 54605, 54607, 54609, 54616, 54617, 54620,
            54624, 54629, 54632, 54633, 54635, 54637, 54644, 54645, 54648, 54652, 54660, 54661, 54663, 54664, 54665,
            54672, 54693, 54728, 54729, 54732, 54736, 54738, 54744, 54745, 54747, 54749, 54756, 54757, 54760, 54764,
            54772, 54773, 54775, 54777, 54784, 54785, 54788, 54792, 54800, 54801, 54803, 54804, 54805, 54812, 54816,
            54820, 54829, 54840, 54841, 54844, 54848, 54853, 54856, 54857, 54859, 54861, 54865, 54868, 54869, 54872,
            54876, 54887, 54889, 54896, 54897, 54900, 54915, 54917, 54924, 54925, 54928, 54932, 54941, 54943, 54945,
            54952, 54956, 54960, 54969, 54971, 54980, 54981, 54984, 54988, 54993, 54996, 54999, 55001, 55008, 55012,
            55016, 55024, 55029, 55036, 55037, 55040, 55044, 55057, 55064, 55065, 55068, 55072, 55080, 55081, 55083,
            55085, 55092, 55093, 55096, 55100, 55108, 55111, 55113, 55120, 55121, 55124, 55126, 55127, 55128, 55129,
            55136, 55137, 55139, 55141, 55145, 55148, 55152, 55156, 55164, 55165, 55169, 55176, 55177, 55180, 55184,
            55192, 55193, 55195, 55197, 4352, 4353, 4354, 4355, 4356, 4357, 4358, 4359, 4360, 4361, 4362, 4363, 4364,
            4365, 4366, 4367, 4368, 4369, 4370, 4449, 4450, 4451, 4452, 4453, 4454, 4455, 4456, 4457, 4461, 4462, 4466,
            4467, 4469, 47252, 49968, 50108, 50388, 52012, 65535
        };
        internal static readonly ushort[] G34_4E =
        {
            478, 351, 352, 353, 358, 359, 360, 361, 362, 363, 020, 365, 366, 369, 370,
            371, 415, 376, 377, 378, 368, 382, 383, 384, 046, 358, 359, 392, 393, 394,
            395, 396, 397, 398, 401, 402, 403, 416, 408, 409, 410, 400, 420, 419, 479,
            450, 445, 003, 004, 006, 008, 010, 068, 449, 072, 013, 015, 017, 019, 021,
            023, 025, 027, 029, 031, 033, 035, 038, 040, 042, 049, 052, 055, 058, 061,
            050, 053, 056, 059, 062, 036, 426, 425, 480, 481, 091, 092, 094, 096, 098,
            364, 100, 102, 106, 108, 110, 112, 114, 117, 119, 121, 123, 124, 125, 385,
            127, 128, 131, 134, 137, 140, 396, 144, 145, 146, 147, 149, 151, 153, 154,
            155, 156, 157, 158, 159, 160, 161, 082, 084, 086, 088, 090, 148, 150, 152,
            093, 095, 097, 099, 101, 103, 105, 107, 109, 111, 113, 115, 118, 120, 122,
            129, 132, 135, 138, 141, 130, 133, 136, 139, 142, 116, 289, 290, 291, 292,
            293, 294, 295, 296, 297, 298, 427, 428, 430, 241, 230, 431, 436, 437, 434,
            435, 443, 444, 424, 429, 242, 433, 299, 300, 301, 302, 303, 304, 305, 306,
            307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321,
            322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336,
            337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 289,
            452, 355, 373, 379, 387, 405, 411
        };
        internal static readonly ushort[] G34_4J =
        {
            001, 003, 005, 007, 009, 011, 012, 014, 016, 018, 020, 022, 024, 026, 028,
            030, 032, 034, 037, 039, 041, 043, 044, 045, 046, 047, 048, 051, 054, 057,
            060, 063, 064, 065, 066, 067, 069, 071, 073, 074, 075, 076, 077, 078, 079,
            080, 081, 002, 004, 006, 008, 010, 068, 070, 072, 013, 015, 017, 019, 021,
            023, 025, 027, 029, 031, 033, 035, 038, 040, 042, 049, 052, 055, 058, 061,
            050, 053, 056, 059, 062, 036, 083, 085, 087, 089, 091, 092, 094, 096, 098,
            100, 102, 104, 106, 108, 110, 112, 114, 117, 119, 121, 123, 124, 125, 126,
            127, 128, 131, 134, 137, 140, 143, 144, 145, 146, 147, 149, 151, 153, 154,
            155, 156, 157, 158, 159, 160, 161, 082, 084, 086, 088, 090, 148, 150, 152,
            093, 095, 097, 099, 101, 103, 105, 107, 109, 111, 113, 115, 118, 120, 122,
            129, 132, 135, 138, 141, 130, 133, 136, 139, 142, 116, 162, 163, 164, 165,
            166, 167, 168, 169, 170, 171, 225, 226, 228, 241, 230, 229, 234, 235, 232,
            233, 443, 444, 424, 430, 242, 433, 172, 173, 174, 175, 176, 177, 178, 179,
            180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194,
            195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
            210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 289,
            452, 355, 373, 379, 387, 405, 411
        };
        #endregion

        internal static readonly byte[][] G4TransferTrashBytes = {
                new byte[] { }, // Unused
                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
                new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
                new byte[] { 0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
                new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
                new byte[] { }, // Unused
                new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            };
    }
    public class Converter
    {
        internal static int Country = 31;
        internal static int Region = 7;
        internal static int ConsoleRegion = 1;
        internal static string OT_Name = "PKHeX";
        internal static int OT_Gender;

        internal static PK6 ConvertPKMtoPK6(byte[] input)
        {
            // Detect Input Generation
            if (input.Length == 100 || input.Length == 80) // PK3
                return new PK3(input).convertToPK4().convertToPK5().convertToPK6();
            if (input.Length != 136 && input.Length != 236 && input.Length != 220)  // Invalid
                return null;
            if ((BitConverter.ToUInt16(input, 0x80) >= 0x3333 || input[0x5F] >= 0x10) && BitConverter.ToUInt16(input, 0x46) == 0) // PK5
                return new PK5(input).convertToPK6();
            return new PK4(input).convertToPK5().convertToPK6(); // PK4
        }

        internal static void updateConfig(int SUBREGION, int COUNTRY, int _3DSREGION, string TRAINERNAME, int TRAINERGENDER)
        {
            Region = SUBREGION;
            Country = COUNTRY;
            ConsoleRegion = _3DSREGION;
            OT_Name = TRAINERNAME;
            OT_Gender = TRAINERGENDER;
        }
    }
}
