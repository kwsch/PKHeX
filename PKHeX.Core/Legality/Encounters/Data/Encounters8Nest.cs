using System;
using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    internal static partial class Encounters8Nest
    {
        #region Nest
        // Location IDs for each Nest group
        private const byte Nest000 = 00;
        private const byte Nest001 = 01;
        private const byte Nest002 = 02;
        private const byte Nest003 = 03;
        private const byte Nest004 = 04;
        private const byte Nest005 = 05;
        private const byte Nest006 = 06;
        private const byte Nest007 = 07;
        private const byte Nest008 = 08;
        private const byte Nest009 = 09;
        private const byte Nest010 = 10;
        private const byte Nest011 = 11;
        private const byte Nest012 = 12;
        private const byte Nest013 = 13;
        private const byte Nest014 = 14;
        private const byte Nest015 = 15;
        private const byte Nest016 = 16;
        private const byte Nest017 = 17;
        private const byte Nest018 = 18;
        private const byte Nest019 = 19;
        private const byte Nest020 = 20;
        private const byte Nest021 = 21;
        private const byte Nest022 = 22;
        private const byte Nest023 = 23;
        private const byte Nest024 = 24;
        private const byte Nest025 = 25;
        private const byte Nest026 = 26;
        private const byte Nest027 = 27;
        private const byte Nest028 = 28;
        private const byte Nest029 = 29;
        private const byte Nest030 = 30;
        private const byte Nest031 = 31;
        private const byte Nest032 = 32;
        private const byte Nest033 = 33;
        private const byte Nest034 = 34;
        private const byte Nest035 = 35;
        private const byte Nest036 = 36;
        private const byte Nest037 = 37;
        private const byte Nest038 = 38;
        private const byte Nest039 = 39;
        private const byte Nest040 = 40;
        private const byte Nest041 = 41;
        private const byte Nest042 = 42;
        private const byte Nest043 = 43;
        private const byte Nest044 = 44;
        private const byte Nest045 = 45;
        private const byte Nest046 = 46;
        private const byte Nest047 = 47;
        private const byte Nest048 = 48;
        private const byte Nest049 = 49;
        private const byte Nest050 = 50;
        private const byte Nest051 = 51;
        private const byte Nest052 = 52;
        private const byte Nest053 = 53;
        private const byte Nest054 = 54;
        private const byte Nest055 = 55;
        private const byte Nest056 = 56;
        private const byte Nest057 = 57;
        private const byte Nest058 = 58;
        private const byte Nest059 = 59;
        private const byte Nest060 = 60;
        private const byte Nest061 = 61;
        private const byte Nest062 = 62;
        private const byte Nest063 = 63;
        private const byte Nest064 = 64;
        private const byte Nest065 = 65;
        private const byte Nest066 = 66;
        private const byte Nest067 = 67;
        private const byte Nest068 = 68;
        private const byte Nest069 = 69;
        private const byte Nest070 = 70;
        private const byte Nest071 = 71;
        private const byte Nest072 = 72;
        private const byte Nest073 = 73;
        private const byte Nest074 = 74;
        private const byte Nest075 = 75;
        private const byte Nest076 = 76;
        private const byte Nest077 = 77;
        private const byte Nest078 = 78;
        private const byte Nest079 = 79;
        private const byte Nest080 = 80;
        private const byte Nest081 = 81;
        private const byte Nest082 = 82;
        private const byte Nest083 = 83;
        private const byte Nest084 = 84;
        private const byte Nest085 = 85;
        private const byte Nest086 = 86;
        private const byte Nest087 = 87;
        private const byte Nest088 = 88;
        private const byte Nest089 = 89;
        private const byte Nest090 = 90;
        private const byte Nest091 = 91;
        private const byte Nest092 = 92;
      //private const byte Nest093 = 93;
      //private const byte Nest094 = 94;
      //private const byte Nest095 = 95;
      //private const byte Nest096 = 96;
      //private const byte Nest097 = 97;
        private const byte Nest098 = 98;
        private const byte Nest099 = 99;
        private const byte Nest100 = 100;
        private const byte Nest101 = 101;
        private const byte Nest102 = 102;
        private const byte Nest103 = 103;
        private const byte Nest104 = 104;
        private const byte Nest105 = 105;
        private const byte Nest106 = 106;
        private const byte Nest107 = 107;
        private const byte Nest108 = 108;
        private const byte Nest109 = 109;
        private const byte Nest110 = 110;
        private const byte Nest111 = 111;
        private const byte Nest112 = 112;
        private const byte Nest113 = 113;
        private const byte Nest114 = 114;
        private const byte Nest115 = 115;
        private const byte Nest116 = 116;
        private const byte Nest117 = 117;
        private const byte Nest118 = 118;
        private const byte Nest119 = 119;
        private const byte Nest120 = 120;
        private const byte Nest121 = 121;
        private const byte Nest122 = 122;
        private const byte Nest123 = 123;
        private const byte Nest124 = 124;
        private const byte Nest125 = 125;
        private const byte Nest126 = 126;
        private const byte Nest127 = 127;
        private const byte Nest128 = 128;
        private const byte Nest129 = 129;
        private const byte Nest130 = 130;
        private const byte Nest131 = 131;
        private const byte Nest132 = 132;
        private const byte Nest133 = 133;
        private const byte Nest134 = 134;
        private const byte Nest135 = 135;
        private const byte Nest136 = 136;
        private const byte Nest137 = 137;
        private const byte Nest138 = 138;
        private const byte Nest139 = 139;
        private const byte Nest140 = 140;
        private const byte Nest141 = 141;
        private const byte Nest142 = 142;
        private const byte Nest143 = 143;
        private const byte Nest144 = 144;
        private const byte Nest145 = 145;
        private const byte Nest146 = 146;
        private const byte Nest147 = 147;
        private const byte Nest148 = 148;
        private const byte Nest149 = 149;
        private const byte Nest150 = 150;
        private const byte Nest151 = 151;
        private const byte Nest152 = 152;
        private const byte Nest153 = 153;
        private const byte Nest154 = 154;
        private const byte Nest155 = 155;
        private const byte Nest156 = 156;
        private const byte Nest157 = 157;
        private const byte Nest158 = 158;
        private const byte Nest159 = 159;
        private const byte Nest160 = 160;
        private const byte Nest161 = 161;
        private const byte Nest162 = 162;
        private const byte Nest163 = 163;
        private const byte Nest164 = 164;
        private const byte Nest165 = 165;
        private const byte Nest166 = 166;
        private const byte Nest167 = 167;
        private const byte Nest168 = 168;
        private const byte Nest169 = 169;
        private const byte Nest170 = 170;
        private const byte Nest171 = 171;
        private const byte Nest172 = 172;
        private const byte Nest173 = 173;
        private const byte Nest174 = 174;
        private const byte Nest175 = 175;
        private const byte Nest176 = 176;
        private const byte Nest177 = 177;
        private const byte Nest178 = 178;
        private const byte Nest179 = 179;
        private const byte Nest180 = 180;
        private const byte Nest181 = 181;
        private const byte Nest182 = 182;
        private const byte Nest183 = 183;
        private const byte Nest184 = 184;
        private const byte Nest185 = 185;
        private const byte Nest186 = 186;
        private const byte Nest187 = 187;
        private const byte Nest188 = 188;
        private const byte Nest189 = 189;
        private const byte Nest190 = 190;
        private const byte Nest191 = 191;
        private const byte Nest192 = 192;
        private const byte Nest193 = 193;
        private const byte Nest194 = 194;
        private const byte Nest195 = 195;
        private const byte Nest196 = 196;

        internal static readonly IReadOnlyList<IReadOnlyList<byte>> NestLocations = new []
        {
            new byte[] {144, 134, 122},      // 000 : Stony Wilderness, South Lake Miloch, Rolling Fields
            new byte[] {144, 126},           // 001 : Stony Wilderness, Watchtower Ruins
            new byte[] {144, 122},           // 002 : Stony Wilderness, Rolling Fields
            new byte[] {142, 124, 122},      // 003 : Bridge Field, Dappled Grove, Rolling Fields
            new byte[] {142, 134},           // 004 : Bridge Field, South Lake Miloch
            new byte[] {144, 126},           // 005 : Stony Wilderness, Watchtower Ruins
            new byte[] {128, 130},           // 006 : East Lake Axewell, West Lake Axewell
            new byte[] {154, 142, 134},      // 007 : Lake of Outrage, Bridge Field, South Lake Miloch
            new byte[] {146, 130},           // 008 : Dusty Bowl, West Lake Axewell
            new byte[] {146, 138},           // 009 : Dusty Bowl, North Lake Miloch
            new byte[] {146, 136},           // 010 : Dusty Bowl, Giants Seat
            new byte[] {150, 144, 136},      // 011 : Hammerlocke Hills, Stony Wilderness, Giants Seat
            new byte[] {142},                // 012 : Bridge Field
            new byte[] {150, 144, 140},      // 013 : Hammerlocke Hills, Stony Wilderness, Motostoke Riverbank
            new byte[] {146, 136},           // 014 : Dusty Bowl, Giants Seat
            new byte[] {142, 122},           // 015 : Bridge Field, Rolling Fields
            new byte[] {146},                // 016 : Dusty Bowl
            new byte[] {154, 152, 144},      // 017 : Lake of Outrage, Giants Cap, Stony Wilderness
            new byte[] {150, 144},           // 018 : Hammerlocke Hills, Stony Wilderness
            new byte[] {146},                // 019 : Dusty Bowl
            new byte[] {146},                // 020 : Dusty Bowl
            new byte[] {144},                // 021 : Stony Wilderness
            new byte[] {150, 152},           // 022 : Hammerlocke Hills, Giants Cap
            new byte[] {152, 140},           // 023 : Giants Cap, Motostoke Riverbank
            new byte[] {154, 148},           // 024 : Lake of Outrage, Giants Mirror
            new byte[] {124},                // 025 : Dappled Grove
            new byte[] {148, 144, 142},      // 026 : Giants Mirror, Stony Wilderness, Bridge Field
            new byte[] {148, 124, 146},      // 027 : Giants Mirror, Dappled Grove AND Dusty Bowl (Giant's Mirror load-line overlap)
            new byte[] {138, 128},           // 028 : North Lake Miloch, East Lake Axewell
            new byte[] {150, 152, 140},      // 029 : Hammerlocke Hills, Giants Cap, Motostoke Riverbank
            new byte[] {128, 122},           // 030 : East Lake Axewell, Rolling Fields
            new byte[] {150, 152},           // 031 : Hammerlocke Hills, Giants Cap
            new byte[] {150, 122},           // 032 : Hammerlocke Hills, Rolling Fields
            new byte[] {154, 142},           // 033 : Lake of Outrage, Bridge Field
            new byte[] {144, 130},           // 034 : Stony Wilderness, West Lake Axewell
            new byte[] {142, 146, 148},      // 035 : Bridge Field, Dusty Bowl, Giants Mirror
            new byte[] {122},                // 036 : Rolling Fields
            new byte[] {132},                // 037 : Axew's Eye
            new byte[] {128, 122},           // 038 : East Lake Axewell, Rolling Fields
            new byte[] {144, 142, 140},      // 039 : Stony Wilderness, Bridge Field, Motostoke Riverbank
            new byte[] {134, 138},           // 040 : South Lake Miloch, North Lake Miloch
            new byte[] {148, 130},           // 041 : Giants Mirror, West Lake Axewell
            new byte[] {148, 144, 134, 146}, // 042 : Giants Mirror, Stony Wilderness, South Lake Miloch AND Dusty Bowl (Giant's Mirror load-line overlap)
            new byte[] {154, 142, 128, 130}, // 043 : Lake of Outrage, Bridge Field, East Lake Axewell, West Lake Axewell 
            new byte[] {150, 136},           // 044 : Hammerlocke Hills, Giants Seat
            new byte[] {142, 134, 122},      // 045 : Bridge Field, South Lake Miloch, Rolling Fields
            new byte[] {126},                // 046 : Watchtower Ruins
            new byte[] {146, 138, 122, 134}, // 047 : Dusty Bowl, North Lake Miloch, Rolling Fields, South Lake Miloch
            new byte[] {146, 136},           // 048 : Dusty Bowl, Giants Seat
            new byte[] {144, 140, 126},      // 049 : Stony Wilderness, Motostoke Riverbank, Watchtower Ruins
            new byte[] {144, 136, 122},      // 050 : Stony Wilderness, Giants Seat, Rolling Fields
            new byte[] {146, 142, 122},      // 051 : Dusty Bowl, Bridge Field, Rolling Fields
            new byte[] {150},                // 052 : Hammerlocke Hills
            new byte[] {146, 144},           // 053 : Dusty Bowl, Stony Wilderness
            new byte[] {152, 146, 144},      // 054 : Giants Cap, Dusty Bowl, Stony Wilderness
            new byte[] {154, 140},           // 055 : Lake of Outrage, Motostoke Riverbank
            new byte[] {150},                // 056 : Hammerlocke Hills
            new byte[] {124},                // 057 : Dappled Grove
            new byte[] {144, 142, 124},      // 058 : Stony Wilderness, Bridge Field, Dappled Grove
            new byte[] {152, 140, 138},      // 059 : Giants Cap, Motostoke Riverbank, North Lake Miloch
            new byte[] {150, 128},           // 060 : Hammerlocke Hills, East Lake Axewell
            new byte[] {150, 122},           // 061 : Hammerlocke Hills, Rolling Fields
            new byte[] {144, 142, 130},      // 062 : Stony Wilderness, Bridge Field, West Lake Axewell
            new byte[] {132, 122},           // 063 : Axew's Eye, Rolling Fields
            new byte[] {142, 140, 128, 122}, // 064 : Bridge Field, Motostoke Riverbank, East Lake Axewell, Rolling Fields 
            new byte[] {144},                // 065 : Stony Wilderness
            new byte[] {148},                // 066 : Giants Mirror
            new byte[] {150},                // 067 : Hammerlocke Hills
            new byte[] {148},                // 068 : Giants Mirror
            new byte[] {148},                // 069 : Giants Mirror
            new byte[] {152},                // 070 : Giants Cap
            new byte[] {148},                // 071 : Giants Mirror
            new byte[] {150},                // 072 : Hammerlocke Hills
            new byte[] {154},                // 073 : Lake of Outrage
            new byte[] {146, 130},           // 074 : Dusty Bowl, West Lake Axewell
            new byte[] {138, 134},           // 075 : North Lake Miloch, South Lake Miloch
            new byte[] {154},                // 076 : Lake of Outrage
            new byte[] {152},                // 077 : Giants Cap
            new byte[] {124},                // 078 : Dappled Grove
            new byte[] {144},                // 079 : Stony Wilderness
            new byte[] {144},                // 080 : Stony Wilderness
            new byte[] {142},                // 081 : Bridge Field
            new byte[] {136},                // 082 : Giants Seat
            new byte[] {136},                // 083 : Giants Seat
            new byte[] {144},                // 084 : Stony Wilderness
            new byte[] {128},                // 085 : East Lake Axewell
            new byte[] {142},                // 086 : Bridge Field
            new byte[] {146},                // 087 : Dusty Bowl
            new byte[] {152},                // 088 : Giants Cap
            new byte[] {122},                // 089 : Rolling Fields
            new byte[] {130, 134},           // 090 : West Lake Axewell, South Lake Miloch
            new byte[] {142, 124},           // 091 : Bridge Field, Dappled Grove
            new byte[] {146},                // 092 : Dusty Bowl
            Array.Empty<byte>(),             // 093 : None
            Array.Empty<byte>(),             // 094 : None
            Array.Empty<byte>(),             // 095 : None
            Array.Empty<byte>(),             // 096 : None
            Array.Empty<byte>(),             // 097 : None
            new byte[] {164, 166, 188, 190}, // 098 : Fields of Honor, Soothing Wetlands, Stepping-Stone Sea, Insular Sea
            new byte[] {164, 166, 188, 190}, // 099 : Fields of Honor, Soothing Wetlands, Stepping-Stone Sea, Insular Sea
            new byte[] {166, 176, 180},      // 100 : Soothing Wetlands, Courageous Cavern, Training Lowlands
            new byte[] {166, 176, 180},      // 101 : Soothing Wetlands, Courageous Cavern, Training Lowlands
            new byte[] {170, 176, 184, 188}, // 102 : Challenge Beach, Courageous Cavern, Potbottom Desert, Stepping-Stone Sea
            new byte[] {170, 176, 188},      // 103 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
            new byte[] {164, 168, 170, 192}, // 104 : Fields of Honor, Forest of Focus, Challenge Beach, Honeycalm Sea
            new byte[] {164, 168, 170, 192}, // 105 : Fields of Honor, Forest of Focus, Challenge Beach, Honeycalm Sea
            new byte[] {174, 178, 186, 188}, // 106 : Challenge Road, Loop Lagoon, Workout Sea, Stepping-Stone Sea
            new byte[] {174, 178, 186, 188}, // 107 : Challenge Road, Loop Lagoon, Workout Sea, Stepping-Stone Sea
            new byte[] {164, 168, 180, 186}, // 108 : Fields of Honor, Forest of Focus, Training Lowlands, Workout Sea
            new byte[] {164, 168, 186},      // 109 : Fields of Honor, Forest of Focus, Workout Sea
            new byte[] {164, 166, 180, 190}, // 110 : Fields of Honor, Soothing Wetlands, Training Lowlands, Insular Sea
            new byte[] {164, 166, 180, 190}, // 111 : Fields of Honor, Soothing Wetlands, Training Lowlands, Insular Sea
            new byte[] {164, 170, 178, 184}, // 112 : Fields of Honor, Challenge Beach, Loop Lagoon, Potbottom Desert
            new byte[] {164, 170, 178, 184}, // 113 : Fields of Honor, Challenge Beach, Loop Lagoon, Potbottom Desert
            new byte[] {164, 180, 184},      // 114 : Fields of Honor, Training Lowlands, Potbottom Desert
            new byte[] {164, 180, 184},      // 115 : Fields of Honor, Training Lowlands, Potbottom Desert
            new byte[] {166, 170, 180, 188}, // 116 : Soothing Wetlands, Challenge Beach, Training Lowlands, Stepping-Stone Sea
            new byte[] {166, 170, 180, 188}, // 117 : Soothing Wetlands, Challenge Beach, Training Lowlands, Stepping-Stone Sea
            new byte[] {166, 168, 180, 188}, // 118 : Soothing Wetlands, Forest of Focus, Training Lowlands, Stepping-Stone Sea
            new byte[] {166, 168, 180, 188}, // 119 : Soothing Wetlands, Forest of Focus, Training Lowlands, Stepping-Stone Sea
            new byte[] {166, 174, 186, 192}, // 120 : Soothing Wetlands, Challenge Road, Workout Sea, Honeycalm Sea
            new byte[] {166, 174, 186, 192}, // 121 : Soothing Wetlands, Challenge Road, Workout Sea, Honeycalm Sea
            new byte[] {164, 170, 174, 192}, // 122 : Fields of Honor, Challenge Beach, Challenge Road, Honeycalm Sea
            new byte[] {164, 170, 174, 192}, // 123 : Fields of Honor, Challenge Beach, Challenge Road, Honeycalm Sea
            new byte[] {164, 166, 168, 190}, // 124 : Fields of Honor, Soothing Wetlands, Forest of Focus, Insular Sea
            new byte[] {164, 166, 168, 190}, // 125 : Fields of Honor, Soothing Wetlands, Forest of Focus, Insular Sea
            new byte[] {170, 176, 188},      // 126 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
            new byte[] {170, 176, 188},      // 127 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
            new byte[] {172, 176, 188},      // 128 : Brawlers' Cave, Courageous Cavern, Stepping-Stone Sea
            new byte[] {172, 176, 188},      // 129 : Brawlers' Cave, Courageous Cavern, Stepping-Stone Sea
            new byte[] {178, 186, 192},      // 130 : Loop Lagoon, Workout Sea, Honeycalm Sea
            new byte[] {186, 192},           // 131 : Workout Sea, Honeycalm Sea
            new byte[] {164, 166, 176},      // 132 : Fields of Honor, Soothing Wetlands, Courageous Cavern
            new byte[] {164, 166, 176},      // 133 : Fields of Honor, Soothing Wetlands, Courageous Cavern
            new byte[] {166, 168, 170, 176}, // 134 : Soothing Wetlands, Forest of Focus, Challenge Beach, Courageous Cavern
            new byte[] {166, 168, 170},      // 135 : Soothing Wetlands, Forest of Focus, Challenge Beach
            new byte[] {164, 170, 178, 190}, // 136 : Fields of Honor, Challenge Beach, Loop Lagoon, Insular Sea
            new byte[] {164, 170, 178},      // 137 : Fields of Honor, Challenge Beach, Loop Lagoon
            new byte[] {186, 188, 190, 192}, // 138 : Workout Sea, Stepping-Stone Sea, Insular Sea, Honeycalm Sea
            new byte[] {186, 188, 190, 192}, // 139 : Workout Sea, Stepping-Stone Sea, Insular Sea, Honeycalm Sea
            Array.Empty<byte>(),             // 140 : None
            Array.Empty<byte>(),             // 141 : None
            new byte[] {194},                // 142 : Honeycalm Island
            new byte[] {194},                // 143 : Honeycalm Island
            new byte[] {168, 180},           // 144 : Forest of Focus, Training Lowlands
            new byte[] {186, 188},           // 145 : Workout Sea, Stepping-Stone Sea
            new byte[] {190},                // 146 : Insular Sea
            new byte[] {176},                // 147 : Courageous Cavern
            new byte[] {180},                // 148 : Training Lowlands
            new byte[] {184},                // 149 : Potbottom Desert
            new byte[] {178},                // 150 : Loop Lagoon
            new byte[] {186},                // 151 : Workout Sea
            new byte[] {186},                // 152 : Workout Sea
            new byte[] {168, 180},           // 153 : Forest of Focus, Training Lowlands
            new byte[] {186, 188},           // 154 : Workout Sea, Stepping-Stone Sea
            new byte[] {174},                // 155 : Challenge Road
            new byte[] {174},                // 156 : Challenge Road

            new byte[] {204,210,222,230},    // 157 : Slippery Slope, Giant's Bed, Giant's Foot, Ballimere Lake
            new byte[] {204,210,222,230},    // 158 : Slippery Slope, Giant's Bed, Giant's Foot, Ballimere Lake
            new byte[] {210,214,222,230},    // 159 : Giant's Bed, Snowslide Slope, Giant's Foot, Ballimere Lake
            new byte[] {210,214,222,230},    // 160 : Giant's Bed, Snowslide Slope, Giant's Foot, Ballimere Lake
            new byte[] {210,222,226,230},    // 161 : Giant's Bed, Giant's Foot, Frigid Sea, Ballimere Lake
            new byte[] {210,222,226,230},    // 162 : Giant's Bed, Giant's Foot, Frigid Sea, Ballimere Lake
            new byte[] {208,210,226,228,230},// 163 : Frostpoint Field, Giant's Bed, Frigid Sea, Three-Point Pass, Ballimere Lake
            new byte[] {208,210,226,228,230},// 164 : Frostpoint Field, Giant's Bed, Frigid Sea, Three-Point Pass, Ballimere Lake
            new byte[] {204,210,220,222,230},// 165 : Slippery Slope, Giant's Bed, Crown Shrine, Giant's Foot, Ballimere Lake
            new byte[] {204,210,220,222,230},// 166 : Slippery Slope, Giant's Bed, Crown Shrine, Giant's Foot, Ballimere Lake
            new byte[] {204,214,226},        // 167 : Slippery Slope, Snowslide Slope, Frigid Sea
            new byte[] {204,214,226},        // 168 : Slippery Slope, Snowslide Slope, Frigid Sea
            new byte[] {210,226},            // 169 : Giant's Bed, Frigid Sea
            new byte[] {210,226},            // 170 : Giant's Bed, Frigid Sea
            new byte[] {208,210,214,226,230},// 171 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
            new byte[] {208,210,214,226,230},// 172 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
            new byte[] {210,226,230},        // 173 : Giant's Bed, Frigid Sea, Ballimere Lake
            new byte[] {210,226,230},        // 174 : Giant's Bed, Frigid Sea, Ballimere Lake
            new byte[] {208,210,226,230,234},// 175 : Frostpoint Field, Giant's Bed, Frigid Sea, Ballimere Lake, Dyna Tree Hill
            new byte[] {208,210,226,230,234},// 176 : Frostpoint Field, Giant's Bed, Frigid Sea, Ballimere Lake, Dyna Tree Hill
            new byte[] {210,214,218,230},    // 177 : Giant's Bed, Snowslide Slope, Path to the Peak, Ballimere Lake
            new byte[] {210,214,218,230},    // 178 : Giant's Bed, Snowslide Slope, Path to the Peak, Ballimere Lake
            new byte[] {204,210,214,230},    // 179 : Slippery Slope, Giant's Bed, Snowslide Slope, Ballimere Lake
            new byte[] {204,210,214,230},    // 180 : Slippery Slope, Giant's Bed, Snowslide Slope, Ballimere Lake
            new byte[] {204,212,222,226,230},// 181 : Slippery Slope, Old Cemetery, Giant's Foot, Frigid Sea, Ballimere Lake
            new byte[] {204,212,222,226,230},// 182 : Slippery Slope, Old Cemetery, Giant's Foot, Frigid Sea, Ballimere Lake
            new byte[] {210,218,226,228,230},// 183 : Giant's Bed, Path to the Peak, Frigid Sea, Three-Point Pass, Ballimere Lake
            new byte[] {210,218,226,228,230},// 184 : Giant's Bed, Path to the Peak, Frigid Sea, Three-Point Pass, Ballimere Lake
            new byte[] {208,210,214,222,226},// 185 : Frostpoint Field, Giant's Bed, Snowslide Slope, Giant's Foot, Frigid Sea
            new byte[] {208,210,214,222,226},// 186 : Frostpoint Field, Giant's Bed, Snowslide Slope, Giant's Foot, Frigid Sea
            new byte[] {210,214,218,226},    // 187 : Giant's Bed, Snowslide Slope, Path to the Peak, Frigid Sea
            new byte[] {210,214,218,226},    // 188 : Giant's Bed, Snowslide Slope, Path to the Peak, Frigid Sea
            new byte[] {208,210,214,226,230},// 189 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
            new byte[] {208,210,214,226,230},// 190 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
            new byte[] {210,212,230},        // 191 : Giant's Bed, Old Cemetery, Ballimere Lake
            new byte[] {210,212,230},        // 192 : Giant's Bed, Old Cemetery, Ballimere Lake
            new byte[] {230},                // 193 : Ballimere Lake
            new byte[] {230},                // 194 : Ballimere Lake
            new byte[] {214},                // 195 : Snowslide Slope
            new byte[] {214},                // 196 : Snowslide Slope 
        };

        /// <summary>
        /// Location IDs containing Dens that cannot be accessed without Rotom Bike's Water Mode.
        /// </summary>
        internal static readonly HashSet<int> InaccessibleRank12DistributionLocations = new() {154,178,186,188,190,192,194,226,228,230,234}; // Areas that are entirely restricted to water

        /// <summary>
        /// Location IDs containing Dens that cannot be accessed without Rotom Bike's Water Mode.
        /// </summary>
        internal static readonly Dictionary<int, byte[]> InaccessibleRank12Nests = new()
        {
            {128, new byte[] {6,43}}, // East Lake Axewell
            {130, new byte[] {6,41,43}}, // West Lake Axewell
            {132, new byte[] {37,63}}, // Axew's Eye
            {134, new byte[] {7,40,75,90}}, // South Lake Miloch
            {138, new byte[] {40,75}}, // North Lake Miloch
            {142, new byte[] {7,43}}, // Bridge Field
            {146, new byte[] {8,74}}, // Dusty Bowl
            {148, new byte[] {41,66}}, // Giant's Mirror
            {154, new byte[] {7,17,24,33,43,55,73,76}}, // Lake of Outrage
            {164, new byte[] {136,137}}, // Fields of Honor
            {168, new byte[] {124,125,134,135,144,153}}, // Forest of Focus
            {170, new byte[] {126,127}}, // Challenge Beach
            {176, new byte[] {132,133}}, // Courageous Cavern
            {178, new byte[] {106,107,112,113,130,136,137,150}}, // Loop Lagoon
            {180, new byte[] {116,117}}, // Training Lowlands
            {186, new byte[] {106,107,108,109,120,121,130,131,138,139,145,151,152,154}}, // Workout Sea
            {188, new byte[] {98,99,102,103,106,107,116,117,118,119,126,127,128,129,138,139,145,154}}, // Stepping-Stone Sea
            {190, new byte[] {98,99,110,111,124,125,136,138,139,146}}, // Insular Sea
            {192, new byte[] {104,105,120,121,122,123,130,131,138,139}}, // Honeycalm Sea
            {194, new byte[] {142,143}}, // Honeycalm Island
            {210, new byte[] {169,170,183,184}}, // Giant's Bed
            {222, new byte[] {181,182,185,186}}, // Giant's Foot
            {226, new byte[] {161,162,163,164,167,168,169,170,171,172,173,174,175,176,181,182,183,184,185,186,187,188,189,190}}, // Frigid Sea
            {228, new byte[] {163,164,183,184}}, // Three-Point Pass
            {230, new byte[] {157,158,159,160,161,162,163,164,165,166,171,172,173,174,175,176,177,178,179,180,181,182,183,184,189,190,191,192,193,194}}, // Ballimere Lake
            {234, new byte[] {175,176}}, // Dyna Tree Hill

            {162, new byte[] {6,7,37,40,41,43,66,73,75,76,130,131,138,139,142,143,145,146,150,151,152,154,169,170,193,194}}, // Completely inaccessible
        };

        // Abilities Allowed
        private const int A0 = 1; // 1 only
        private const int A1 = 2; // 2 only
        private const int A2 = 4; // Ability 4 only
        private const int A3 = 0; // 1/2 only
        private const int A4 = -1; // 1/2/H

        internal const int SharedNest = 162;
        internal const int Watchtower = 126;
        internal const int MaxLair = 244;

        internal static readonly EncounterStatic8N[] Nest_Common =
        {
            new(Nest000,0,0,1,SWSH) { Species = 236, Ability = A3 }, // Tyrogue
            new(Nest000,0,0,1,SWSH) { Species = 066, Ability = A3 }, // Machop
            new(Nest000,0,1,1,SWSH) { Species = 532, Ability = A3 }, // Timburr
            new(Nest000,1,2,2,SWSH) { Species = 067, Ability = A3 }, // Machoke
            new(Nest000,1,2,2,SWSH) { Species = 533, Ability = A3 }, // Gurdurr
            new(Nest000,4,4,4,SWSH) { Species = 068, Ability = A4 }, // Machamp
            new(Nest001,0,0,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest001,0,0,1,SWSH) { Species = 517, Ability = A3 }, // Munna
            new(Nest001,0,1,1,SWSH) { Species = 677, Ability = A3 }, // Espurr
            new(Nest001,0,1,1,SWSH) { Species = 605, Ability = A3 }, // Elgyem
            new(Nest001,1,2,2,SWSH) { Species = 281, Ability = A3 }, // Kirlia
            new(Nest001,2,4,4,SWSH) { Species = 518, Ability = A3 }, // Musharna
            new(Nest001,4,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest002,0,0,1,SWSH) { Species = 438, Ability = A3 }, // Bonsly
            new(Nest002,0,1,1,SWSH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest002,1,2,2,SWSH) { Species = 111, Ability = A3 }, // Rhyhorn
            new(Nest002,1,2,2,SWSH) { Species = 525, Ability = A3 }, // Boldore
            new(Nest002,2,3,3,SWSH) { Species = 689, Ability = A3 }, // Barbaracle
            new(Nest002,2,4,4,SWSH) { Species = 112, Ability = A3 }, // Rhydon
            new(Nest002,2,4,4,SWSH) { Species = 185, Ability = A3 }, // Sudowoodo
            new(Nest002,4,4,4,SWSH) { Species = 213, Ability = A4 }, // Shuckle
            new(Nest003,0,0,1,SWSH) { Species = 010, Ability = A3 }, // Caterpie
            new(Nest003,0,0,1,SWSH) { Species = 736, Ability = A3 }, // Grubbin
            new(Nest003,0,1,1,SWSH) { Species = 290, Ability = A3 }, // Nincada
            new(Nest003,0,1,1,SWSH) { Species = 595, Ability = A3 }, // Joltik
            new(Nest003,1,2,2,SWSH) { Species = 011, Ability = A3 }, // Metapod
            new(Nest003,1,2,2,SWSH) { Species = 632, Ability = A3 }, // Durant
            new(Nest003,2,3,3,SWSH) { Species = 737, Ability = A3 }, // Charjabug
            new(Nest003,2,4,4,SWSH) { Species = 291, Ability = A3 }, // Ninjask
            new(Nest003,2,4,4,SWSH) { Species = 012, Ability = A3 }, // Butterfree
            new(Nest003,3,4,4,SWSH) { Species = 596, Ability = A4 }, // Galvantula
            new(Nest003,4,4,4,SWSH) { Species = 738, Ability = A4 }, // Vikavolt
            new(Nest003,4,4,4,SWSH) { Species = 632, Ability = A4 }, // Durant
            new(Nest004,0,0,1,SWSH) { Species = 010, Ability = A3 }, // Caterpie
            new(Nest004,0,0,1,SWSH) { Species = 415, Ability = A3 }, // Combee
            new(Nest004,0,1,1,SWSH) { Species = 742, Ability = A3 }, // Cutiefly
            new(Nest004,0,1,1,SWSH) { Species = 824, Ability = A3 }, // Blipbug
            new(Nest004,1,2,2,SWSH) { Species = 595, Ability = A3 }, // Joltik
            new(Nest004,1,2,2,SWSH) { Species = 011, Ability = A3 }, // Metapod
            new(Nest004,2,3,3,SWSH) { Species = 825, Ability = A3 }, // Dottler
            new(Nest004,2,4,4,SWSH) { Species = 596, Ability = A3 }, // Galvantula
            new(Nest004,2,4,4,SWSH) { Species = 012, Ability = A3 }, // Butterfree
            new(Nest004,3,4,4,SWSH) { Species = 743, Ability = A4 }, // Ribombee
            new(Nest004,4,4,4,SWSH) { Species = 416, Ability = A4 }, // Vespiquen
            new(Nest004,4,4,4,SWSH) { Species = 826, Ability = A4 }, // Orbeetle
            new(Nest005,0,0,1,SWSH) { Species = 092, Ability = A3 }, // Gastly
            new(Nest005,0,0,1,SWSH) { Species = 355, Ability = A3 }, // Duskull
            new(Nest005,0,1,1,SWSH) { Species = 425, Ability = A3 }, // Drifloon
            new(Nest005,0,1,1,SWSH) { Species = 708, Ability = A3 }, // Phantump
            new(Nest005,0,1,1,SWSH) { Species = 592, Ability = A3 }, // Frillish
            new(Nest005,1,2,2,SWSH) { Species = 710, Ability = A3 }, // Pumpkaboo
            new(Nest005,2,3,3,SWSH) { Species = 093, Ability = A3 }, // Haunter
            new(Nest005,2,4,4,SWSH) { Species = 356, Ability = A3 }, // Dusclops
            new(Nest005,2,4,4,SWSH) { Species = 426, Ability = A3 }, // Drifblim
            new(Nest005,3,4,4,SWSH) { Species = 709, Ability = A4 }, // Trevenant
            new(Nest005,4,4,4,SWSH) { Species = 711, Ability = A4 }, // Gourgeist
            new(Nest005,4,4,4,SWSH) { Species = 593, Ability = A4 }, // Jellicent
          //new(Nest006,0,0,1,SWSH) { Species = 129, Ability = A3 }, // Magikarp
          //new(Nest006,0,0,1,SWSH) { Species = 458, Ability = A3 }, // Mantyke
            new(Nest006,2,2,2,SWSH) { Species = 320, Ability = A3 }, // Wailmer
            new(Nest006,2,3,3,SWSH) { Species = 224, Ability = A3 }, // Octillery
            new(Nest006,2,4,4,SWSH) { Species = 226, Ability = A3 }, // Mantine
            new(Nest006,2,4,4,SWSH) { Species = 171, Ability = A3 }, // Lanturn
            new(Nest006,3,4,4,SWSH) { Species = 321, Ability = A4 }, // Wailord
            new(Nest006,4,4,4,SWSH) { Species = 746, Ability = A4 }, // Wishiwashi
            new(Nest006,4,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
          //new(Nest007,0,0,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
          //new(Nest007,0,0,1,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
          //new(Nest007,0,1,1,SWSH) { Species = 422, Ability = A3, Form = 1 }, // Shellos-1
          //new(Nest007,0,1,1,SWSH) { Species = 751, Ability = A3 }, // Dewpider
            new(Nest007,2,2,2,SWSH) { Species = 320, Ability = A3 }, // Wailmer
            new(Nest007,2,3,3,SWSH) { Species = 746, Ability = A3 }, // Wishiwashi
            new(Nest007,2,4,4,SWSH) { Species = 834, Ability = A3 }, // Drednaw
            new(Nest007,2,4,4,SWSH) { Species = 847, Ability = A3 }, // Barraskewda
            new(Nest007,3,4,4,SWSH) { Species = 752, Ability = A4 }, // Araquanid
            new(Nest007,4,4,4,SWSH) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new(Nest007,4,4,4,SWSH) { Species = 321, Ability = A4 }, // Wailord
            new(Nest008,0,0,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
            new(Nest008,0,0,1,SWSH) { Species = 194, Ability = A3 }, // Wooper
            new(Nest008,0,1,1,SWSH) { Species = 535, Ability = A3 }, // Tympole
            new(Nest008,0,1,1,SWSH) { Species = 341, Ability = A3 }, // Corphish
            new(Nest008,1,2,2,SWSH) { Species = 536, Ability = A3 }, // Palpitoad
            new(Nest008,2,3,3,SWSH) { Species = 834, Ability = A3 }, // Drednaw
            new(Nest008,2,4,4,SWSH) { Species = 195, Ability = A3 }, // Quagsire
            new(Nest008,2,4,4,SWSH) { Species = 771, Ability = A3 }, // Pyukumuku
            new(Nest008,3,4,4,SWSH) { Species = 091, Ability = A4 }, // Cloyster
            new(Nest008,4,4,4,SWSH) { Species = 537, Ability = A4 }, // Seismitoad
            new(Nest008,4,4,4,SWSH) { Species = 342, Ability = A4 }, // Crawdaunt
            new(Nest009,0,0,1,SWSH) { Species = 236, Ability = A3 }, // Tyrogue
            new(Nest009,0,0,1,SWSH) { Species = 759, Ability = A3 }, // Stufful
            new(Nest009,0,1,1,SWSH) { Species = 852, Ability = A3 }, // Clobbopus
            new(Nest009,0,1,1,SWSH) { Species = 674, Ability = A3 }, // Pancham
            new(Nest009,2,4,4,SWSH) { Species = 760, Ability = A3 }, // Bewear
            new(Nest009,2,4,4,SWSH) { Species = 675, Ability = A3 }, // Pangoro
            new(Nest009,2,4,4,SWSH) { Species = 701, Ability = A3 }, // Hawlucha
            new(Nest009,4,4,4,SWSH) { Species = 853, Ability = A4 }, // Grapploct
            new(Nest009,4,4,4,SWSH) { Species = 870, Ability = A4 }, // Falinks
            new(Nest010,0,0,1,SWSH) { Species = 599, Ability = A3 }, // Klink
            new(Nest010,0,0,1,SWSH) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new(Nest010,0,1,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest010,0,1,1,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest010,1,1,2,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest010,1,2,2,SWSH) { Species = 878, Ability = A3 }, // Cufant
            new(Nest010,2,4,4,SWSH) { Species = 600, Ability = A3 }, // Klang
            new(Nest010,2,4,4,SWSH) { Species = 863, Ability = A3 }, // Perrserker
            new(Nest010,2,4,4,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest010,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest010,4,4,4,SWSH) { Species = 601, Ability = A4 }, // Klinklang
            new(Nest010,4,4,4,SWSH) { Species = 879, Ability = A4 }, // Copperajah
            new(Nest011,0,0,1,SWSH) { Species = 599, Ability = A3 }, // Klink
            new(Nest011,0,0,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest011,0,1,1,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest011,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest011,1,1,2,SWSH) { Species = 599, Ability = A3 }, // Klink
            new(Nest011,1,2,2,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest011,2,4,4,SWSH) { Species = 208, Ability = A3 }, // Steelix
            new(Nest011,2,4,4,SWSH) { Species = 598, Ability = A3 }, // Ferrothorn
            new(Nest011,2,4,4,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest011,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest011,4,4,4,SWSH) { Species = 777, Ability = A4 }, // Togedemaru
            new(Nest012,0,0,1,SWSH) { Species = 439, Ability = A3 }, // Mime Jr.
            new(Nest012,0,0,1,SWSH) { Species = 824, Ability = A3 }, // Blipbug
            new(Nest012,2,3,3,SWSH) { Species = 561, Ability = A3 }, // Sigilyph
            new(Nest012,2,3,3,SWSH) { Species = 178, Ability = A3 }, // Xatu
            new(Nest012,4,4,4,SWSH) { Species = 858, Ability = A4 }, // Hatterene
            new(Nest013,0,0,1,SWSH) { Species = 439, Ability = A3 }, // Mime Jr.
            new(Nest013,0,0,1,SWSH) { Species = 360, Ability = A3 }, // Wynaut
            new(Nest013,0,1,1,SWSH) { Species = 177, Ability = A3 }, // Natu
            new(Nest013,0,1,1,SWSH) { Species = 343, Ability = A3 }, // Baltoy
            new(Nest013,1,1,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest013,1,3,3,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest013,2,3,3,SWSH) { Species = 561, Ability = A3 }, // Sigilyph
            new(Nest013,2,3,3,SWSH) { Species = 178, Ability = A3 }, // Xatu
            new(Nest013,3,4,4,SWSH) { Species = 344, Ability = A4 }, // Claydol
            new(Nest013,4,4,4,SWSH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest013,4,4,4,SWSH) { Species = 202, Ability = A4 }, // Wobbuffet
            new(Nest014,0,0,1,SWSH) { Species = 837, Ability = A3 }, // Rolycoly
            new(Nest014,0,1,1,SWSH) { Species = 688, Ability = A3 }, // Binacle
            new(Nest014,1,1,1,SWSH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest014,1,2,2,SWSH) { Species = 525, Ability = A3 }, // Boldore
            new(Nest014,2,3,3,SWSH) { Species = 558, Ability = A3 }, // Crustle
            new(Nest014,2,4,4,SWSH) { Species = 689, Ability = A3 }, // Barbaracle
            new(Nest014,4,4,4,SWSH) { Species = 464, Ability = A4 }, // Rhyperior
            new(Nest015,0,0,1,SWSH) { Species = 050, Ability = A3 }, // Diglett
            new(Nest015,0,0,1,SWSH) { Species = 749, Ability = A3 }, // Mudbray
            new(Nest015,0,1,1,SWSH) { Species = 290, Ability = A3 }, // Nincada
            new(Nest015,0,1,1,SWSH) { Species = 529, Ability = A3 }, // Drilbur
            new(Nest015,1,1,1,SWSH) { Species = 095, Ability = A3 }, // Onix
            new(Nest015,1,2,2,SWSH) { Species = 339, Ability = A3 }, // Barboach
            new(Nest015,2,3,3,SWSH) { Species = 208, Ability = A3 }, // Steelix
            new(Nest015,2,4,4,SWSH) { Species = 340, Ability = A3 }, // Whiscash
            new(Nest015,2,4,4,SWSH) { Species = 660, Ability = A3 }, // Diggersby
            new(Nest015,3,4,4,SWSH) { Species = 051, Ability = A4 }, // Dugtrio
            new(Nest015,4,4,4,SWSH) { Species = 530, Ability = A4 }, // Excadrill
            new(Nest015,4,4,4,SWSH) { Species = 750, Ability = A4 }, // Mudsdale
            new(Nest016,0,0,1,SWSH) { Species = 843, Ability = A3 }, // Silicobra
            new(Nest016,0,0,1,SWSH) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new(Nest016,0,1,1,SWSH) { Species = 449, Ability = A3 }, // Hippopotas
            new(Nest016,1,2,2,SWSH) { Species = 221, Ability = A3 }, // Piloswine
            new(Nest016,4,4,4,SWSH) { Species = 867, Ability = A3 }, // Runerigus
            new(Nest016,4,4,4,SWSH) { Species = 844, Ability = A4 }, // Sandaconda
            new(Nest017,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest017,0,1,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest017,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest017,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest018,0,0,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest018,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest018,1,1,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest018,4,4,4,SWSH) { Species = 609, Ability = A4 }, // Chandelure
            new(Nest019,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest019,0,1,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest019,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest019,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest019,3,4,4,SWSH) { Species = 851, Ability = A4 }, // Centiskorch
            new(Nest019,4,4,4,SWSH) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest020,0,0,1,SWSH) { Species = 582, Ability = A3 }, // Vanillite
            new(Nest020,0,0,1,SWSH) { Species = 220, Ability = A3 }, // Swinub
            new(Nest020,0,1,1,SWSH) { Species = 459, Ability = A3 }, // Snover
            new(Nest020,0,1,1,SWSH) { Species = 712, Ability = A3 }, // Bergmite
            new(Nest020,1,1,1,SWSH) { Species = 225, Ability = A3 }, // Delibird
            new(Nest020,1,2,2,SWSH) { Species = 583, Ability = A3 }, // Vanillish
            new(Nest020,2,3,3,SWSH) { Species = 221, Ability = A3 }, // Piloswine
            new(Nest020,2,4,4,SWSH) { Species = 713, Ability = A3 }, // Avalugg
            new(Nest020,2,4,4,SWSH) { Species = 460, Ability = A3 }, // Abomasnow
            new(Nest020,3,4,4,SWSH) { Species = 091, Ability = A4 }, // Cloyster
            new(Nest020,4,4,4,SWSH) { Species = 584, Ability = A4 }, // Vanilluxe
            new(Nest020,4,4,4,SWSH) { Species = 131, Ability = A4 }, // Lapras
            new(Nest021,0,0,1,SWSH) { Species = 220, Ability = A3 }, // Swinub
            new(Nest021,0,0,1,SWSH) { Species = 613, Ability = A3 }, // Cubchoo
            new(Nest021,0,1,1,SWSH) { Species = 872, Ability = A3 }, // Snom
            new(Nest021,0,1,1,SWSH) { Species = 215, Ability = A3 }, // Sneasel
            new(Nest021,1,1,1,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest021,1,2,2,SWSH) { Species = 221, Ability = A3 }, // Piloswine
            new(Nest021,2,3,3,SWSH) { Species = 091, Ability = A3 }, // Cloyster
            new(Nest021,2,4,4,SWSH) { Species = 614, Ability = A3 }, // Beartic
            new(Nest021,2,4,4,SWSH) { Species = 866, Ability = A3 }, // Mr. Rime
            new(Nest021,3,4,4,SWSH) { Species = 473, Ability = A4 }, // Mamoswine
            new(Nest021,4,4,4,SWSH) { Species = 873, Ability = A4 }, // Frosmoth
            new(Nest021,4,4,4,SWSH) { Species = 461, Ability = A4 }, // Weavile
            new(Nest022,0,0,1,SWSH) { Species = 361, Ability = A3 }, // Snorunt
            new(Nest022,0,0,1,SWSH) { Species = 872, Ability = A3 }, // Snom
            new(Nest022,0,1,1,SWSH) { Species = 215, Ability = A3 }, // Sneasel
            new(Nest022,1,1,2,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest022,1,2,3,SWSH) { Species = 459, Ability = A3 }, // Snover
            new(Nest022,2,3,3,SWSH) { Species = 460, Ability = A3 }, // Abomasnow
            new(Nest022,2,4,4,SWSH) { Species = 362, Ability = A3 }, // Glalie
            new(Nest022,2,4,4,SWSH) { Species = 866, Ability = A3 }, // Mr. Rime
            new(Nest022,3,4,4,SWSH) { Species = 873, Ability = A4 }, // Frosmoth
            new(Nest022,4,4,4,SWSH) { Species = 478, Ability = A4 }, // Froslass
            new(Nest023,0,0,1,SWSH) { Species = 172, Ability = A3 }, // Pichu
            new(Nest023,0,0,1,SWSH) { Species = 309, Ability = A3 }, // Electrike
            new(Nest023,0,1,1,SWSH) { Species = 595, Ability = A3 }, // Joltik
            new(Nest023,0,1,1,SWSH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest023,1,1,2,SWSH) { Species = 737, Ability = A3 }, // Charjabug
            new(Nest023,1,2,3,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest023,2,3,3,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest023,2,4,4,SWSH) { Species = 310, Ability = A3 }, // Manectric
            new(Nest023,2,4,4,SWSH) { Species = 171, Ability = A3 }, // Lanturn
            new(Nest023,3,4,4,SWSH) { Species = 596, Ability = A4 }, // Galvantula
            new(Nest023,4,4,4,SWSH) { Species = 738, Ability = A4 }, // Vikavolt
            new(Nest023,4,4,4,SWSH) { Species = 026, Ability = A4 }, // Raichu
            new(Nest024,0,0,1,SWSH) { Species = 835, Ability = A3 }, // Yamper
            new(Nest024,0,0,1,SWSH) { Species = 694, Ability = A3 }, // Helioptile
            new(Nest024,0,1,1,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest024,0,1,1,SWSH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest024,1,1,2,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest024,1,2,3,SWSH) { Species = 171, Ability = A3 }, // Lanturn
            new(Nest024,2,3,3,SWSH) { Species = 836, Ability = A3 }, // Boltund
            new(Nest024,2,4,4,SWSH) { Species = 695, Ability = A3 }, // Heliolisk
            new(Nest024,2,4,4,SWSH) { Species = 849, Ability = A3 }, // Toxtricity
            new(Nest024,3,4,4,SWSH) { Species = 871, Ability = A4 }, // Pincurchin
            new(Nest024,4,4,4,SWSH) { Species = 777, Ability = A4 }, // Togedemaru
            new(Nest024,4,4,4,SWSH) { Species = 877, Ability = A4 }, // Morpeko
            new(Nest025,0,0,1,SWSH) { Species = 406, Ability = A3 }, // Budew
            new(Nest025,0,1,1,SWSH) { Species = 761, Ability = A3 }, // Bounsweet
            new(Nest025,0,1,1,SWSH) { Species = 043, Ability = A3 }, // Oddish
            new(Nest025,1,2,3,SWSH) { Species = 315, Ability = A3 }, // Roselia
            new(Nest025,2,3,3,SWSH) { Species = 044, Ability = A3 }, // Gloom
            new(Nest025,2,4,4,SWSH) { Species = 762, Ability = A3 }, // Steenee
            new(Nest025,3,4,4,SWSH) { Species = 763, Ability = A4 }, // Tsareena
            new(Nest025,4,4,4,SWSH) { Species = 045, Ability = A4 }, // Vileplume
            new(Nest025,4,4,4,SWSH) { Species = 182, Ability = A4 }, // Bellossom
            new(Nest026,0,0,1,SWSH) { Species = 406, Ability = A3 }, // Budew
            new(Nest026,0,0,1,SWSH) { Species = 829, Ability = A3 }, // Gossifleur
            new(Nest026,0,1,1,SWSH) { Species = 546, Ability = A3 }, // Cottonee
            new(Nest026,0,1,1,SWSH) { Species = 840, Ability = A3 }, // Applin
            new(Nest026,1,1,2,SWSH) { Species = 420, Ability = A3 }, // Cherubi
            new(Nest026,1,2,2,SWSH) { Species = 315, Ability = A3 }, // Roselia
            new(Nest026,2,3,3,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest026,2,4,4,SWSH) { Species = 598, Ability = A3 }, // Ferrothorn
            new(Nest026,2,4,4,SWSH) { Species = 421, Ability = A3 }, // Cherrim
            new(Nest026,3,4,4,SWSH) { Species = 830, Ability = A4 }, // Eldegoss
            new(Nest026,4,4,4,SWSH) { Species = 547, Ability = A4 }, // Whimsicott
            new(Nest027,0,0,1,SWSH) { Species = 710, Ability = A3, Form = 1 }, // Pumpkaboo-1
            new(Nest027,0,0,1,SWSH) { Species = 708, Ability = A3 }, // Phantump
            new(Nest027,0,1,1,SWSH) { Species = 710, Ability = A3 }, // Pumpkaboo
            new(Nest027,0,1,1,SWSH) { Species = 755, Ability = A3 }, // Morelull
            new(Nest027,1,1,2,SWSH) { Species = 710, Ability = A3, Form = 2 }, // Pumpkaboo-2
            new(Nest027,1,2,2,SWSH) { Species = 315, Ability = A3 }, // Roselia
            new(Nest027,2,3,3,SWSH) { Species = 756, Ability = A3 }, // Shiinotic
            new(Nest027,2,4,4,SWSH) { Species = 556, Ability = A3 }, // Maractus
            new(Nest027,2,4,4,SWSH) { Species = 709, Ability = A3 }, // Trevenant
            new(Nest027,3,4,4,SWSH) { Species = 711, Ability = A4 }, // Gourgeist
            new(Nest027,4,4,4,SWSH) { Species = 781, Ability = A4 }, // Dhelmise
            new(Nest027,4,4,4,SWSH) { Species = 710, Ability = A4, Form = 3 }, // Pumpkaboo-3
            new(Nest028,0,0,1,SWSH) { Species = 434, Ability = A3 }, // Stunky
            new(Nest028,0,0,1,SWSH) { Species = 568, Ability = A3 }, // Trubbish
            new(Nest028,0,1,1,SWSH) { Species = 451, Ability = A3 }, // Skorupi
            new(Nest028,1,2,2,SWSH) { Species = 315, Ability = A3 }, // Roselia
            new(Nest028,2,3,3,SWSH) { Species = 211, Ability = A3 }, // Qwilfish
            new(Nest028,2,4,4,SWSH) { Species = 452, Ability = A3 }, // Drapion
            new(Nest028,2,4,4,SWSH) { Species = 045, Ability = A3 }, // Vileplume
            new(Nest028,4,4,4,SWSH) { Species = 569, Ability = A4 }, // Garbodor
            new(Nest029,0,0,1,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest029,0,0,1,SWSH) { Species = 092, Ability = A3 }, // Gastly
            new(Nest029,0,1,1,SWSH) { Species = 451, Ability = A3 }, // Skorupi
            new(Nest029,0,1,1,SWSH) { Species = 043, Ability = A3 }, // Oddish
            new(Nest029,1,1,2,SWSH) { Species = 044, Ability = A3 }, // Gloom
            new(Nest029,1,2,2,SWSH) { Species = 093, Ability = A3 }, // Haunter
            new(Nest029,2,3,3,SWSH) { Species = 109, Ability = A3 }, // Koffing
            new(Nest029,2,4,4,SWSH) { Species = 211, Ability = A3 }, // Qwilfish
            new(Nest029,2,4,4,SWSH) { Species = 045, Ability = A3 }, // Vileplume
            new(Nest029,3,4,4,SWSH) { Species = 315, Ability = A4 }, // Roselia
            new(Nest029,4,4,4,SWSH) { Species = 849, Ability = A4 }, // Toxtricity
            new(Nest029,4,4,4,SWSH) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new(Nest030,0,0,1,SWSH) { Species = 519, Ability = A3 }, // Pidove
            new(Nest030,0,0,1,SWSH) { Species = 163, Ability = A3 }, // Hoothoot
            new(Nest030,0,1,1,SWSH) { Species = 177, Ability = A3 }, // Natu
            new(Nest030,1,1,2,SWSH) { Species = 527, Ability = A3 }, // Woobat
            new(Nest030,1,2,2,SWSH) { Species = 520, Ability = A3 }, // Tranquill
            new(Nest030,2,3,3,SWSH) { Species = 521, Ability = A3 }, // Unfezant
            new(Nest030,2,4,4,SWSH) { Species = 164, Ability = A3 }, // Noctowl
            new(Nest030,2,4,4,SWSH) { Species = 528, Ability = A3 }, // Swoobat
            new(Nest030,3,4,4,SWSH) { Species = 178, Ability = A4 }, // Xatu
            new(Nest030,4,4,4,SWSH) { Species = 561, Ability = A4 }, // Sigilyph
            new(Nest031,0,0,1,SWSH) { Species = 821, Ability = A3 }, // Rookidee
            new(Nest031,0,0,1,SWSH) { Species = 714, Ability = A3 }, // Noibat
            new(Nest031,0,1,1,SWSH) { Species = 278, Ability = A3 }, // Wingull
            new(Nest031,0,1,1,SWSH) { Species = 177, Ability = A3 }, // Natu
            new(Nest031,1,1,2,SWSH) { Species = 425, Ability = A3 }, // Drifloon
            new(Nest031,1,2,2,SWSH) { Species = 822, Ability = A3 }, // Corvisquire
            new(Nest031,2,3,3,SWSH) { Species = 426, Ability = A3 }, // Drifblim
            new(Nest031,2,4,4,SWSH) { Species = 279, Ability = A3 }, // Pelipper
            new(Nest031,2,4,4,SWSH) { Species = 178, Ability = A3 }, // Xatu
            new(Nest031,3,4,4,SWSH) { Species = 823, Ability = A4 }, // Corviknight
            new(Nest031,4,4,4,SWSH) { Species = 701, Ability = A4 }, // Hawlucha
            new(Nest031,4,4,4,SWSH) { Species = 845, Ability = A4 }, // Cramorant
            new(Nest032,0,0,1,SWSH) { Species = 173, Ability = A3 }, // Cleffa
            new(Nest032,0,0,1,SWSH) { Species = 175, Ability = A3 }, // Togepi
            new(Nest032,0,1,1,SWSH) { Species = 742, Ability = A3 }, // Cutiefly
            new(Nest032,1,1,2,SWSH) { Species = 035, Ability = A3 }, // Clefairy
            new(Nest032,1,2,2,SWSH) { Species = 755, Ability = A3 }, // Morelull
            new(Nest032,2,3,3,SWSH) { Species = 176, Ability = A3 }, // Togetic
            new(Nest032,2,4,4,SWSH) { Species = 036, Ability = A3 }, // Clefable
            new(Nest032,2,4,4,SWSH) { Species = 743, Ability = A3 }, // Ribombee
            new(Nest032,3,4,4,SWSH) { Species = 756, Ability = A4 }, // Shiinotic
            new(Nest032,4,4,4,SWSH) { Species = 468, Ability = A4 }, // Togekiss
            new(Nest033,0,0,1,SWSH) { Species = 439, Ability = A3 }, // Mime Jr.
            new(Nest033,0,0,1,SWSH) { Species = 868, Ability = A3 }, // Milcery
            new(Nest033,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest033,0,1,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest033,1,1,2,SWSH) { Species = 035, Ability = A3 }, // Clefairy
            new(Nest033,1,2,2,SWSH) { Species = 281, Ability = A3 }, // Kirlia
            new(Nest033,2,3,3,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest033,2,4,4,SWSH) { Species = 036, Ability = A3 }, // Clefable
            new(Nest033,2,4,4,SWSH) { Species = 282, Ability = A3 }, // Gardevoir
            new(Nest033,3,4,4,SWSH) { Species = 869, Ability = A4 }, // Alcremie
            new(Nest033,4,4,4,SWSH) { Species = 861, Ability = A4 }, // Grimmsnarl
            new(Nest034,0,0,1,SWSH) { Species = 509, Ability = A3 }, // Purrloin
            new(Nest034,0,0,1,SWSH) { Species = 434, Ability = A3 }, // Stunky
            new(Nest034,0,1,1,SWSH) { Species = 215, Ability = A3 }, // Sneasel
            new(Nest034,0,1,1,SWSH) { Species = 686, Ability = A3 }, // Inkay
            new(Nest034,1,1,2,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest034,1,2,2,SWSH) { Species = 510, Ability = A3 }, // Liepard
            new(Nest034,2,3,3,SWSH) { Species = 435, Ability = A3 }, // Skuntank
            new(Nest034,2,4,4,SWSH) { Species = 461, Ability = A3 }, // Weavile
            new(Nest034,2,4,4,SWSH) { Species = 687, Ability = A3 }, // Malamar
            new(Nest034,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest034,4,4,4,SWSH) { Species = 342, Ability = A4 }, // Crawdaunt
            new(Nest035,0,0,1,SWSH) { Species = 827, Ability = A3 }, // Nickit
            new(Nest035,0,0,1,SWSH) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new(Nest035,0,1,1,SWSH) { Species = 509, Ability = A3 }, // Purrloin
            new(Nest035,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest035,1,2,2,SWSH) { Species = 828, Ability = A3 }, // Thievul
            new(Nest035,2,3,3,SWSH) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new(Nest035,2,4,4,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest035,2,4,4,SWSH) { Species = 861, Ability = A3 }, // Grimmsnarl
            new(Nest035,4,4,4,SWSH) { Species = 862, Ability = A4 }, // Obstagoon
            new(Nest036,0,0,1,SWSH) { Species = 714, Ability = A3 }, // Noibat
            new(Nest036,0,1,1,SWSH) { Species = 714, Ability = A3 }, // Noibat
            new(Nest036,1,2,2,SWSH) { Species = 329, Ability = A3 }, // Vibrava
          //new(Nest037,0,0,1,SWSH) { Species = 714, Ability = A3 }, // Noibat
          //new(Nest037,0,0,1,SWSH) { Species = 840, Ability = A3 }, // Applin
          //new(Nest037,0,1,1,SWSH) { Species = 885, Ability = A3 }, // Dreepy
          //new(Nest037,1,1,2,SWSH) { Species = 714, Ability = A3 }, // Noibat
            new(Nest037,2,2,2,SWSH) { Species = 840, Ability = A3 }, // Applin
            new(Nest037,2,3,3,SWSH) { Species = 886, Ability = A3 }, // Drakloak
            new(Nest037,2,4,4,SWSH) { Species = 715, Ability = A3 }, // Noivern
            new(Nest037,4,4,4,SWSH) { Species = 887, Ability = A4 }, // Dragapult
            new(Nest038,0,0,1,SWSH) { Species = 659, Ability = A3 }, // Bunnelby
            new(Nest038,0,0,1,SWSH) { Species = 163, Ability = A3 }, // Hoothoot
            new(Nest038,0,1,1,SWSH) { Species = 519, Ability = A3 }, // Pidove
            new(Nest038,0,1,1,SWSH) { Species = 572, Ability = A3 }, // Minccino
            new(Nest038,1,1,2,SWSH) { Species = 694, Ability = A3 }, // Helioptile
            new(Nest038,1,2,2,SWSH) { Species = 759, Ability = A3 }, // Stufful
            new(Nest038,2,3,3,SWSH) { Species = 660, Ability = A3 }, // Diggersby
            new(Nest038,2,4,4,SWSH) { Species = 164, Ability = A3 }, // Noctowl
            new(Nest038,2,4,4,SWSH) { Species = 521, Ability = A3 }, // Unfezant
            new(Nest038,3,4,4,SWSH) { Species = 695, Ability = A4 }, // Heliolisk
            new(Nest038,4,4,4,SWSH) { Species = 573, Ability = A4 }, // Cinccino
            new(Nest038,4,4,4,SWSH) { Species = 760, Ability = A4 }, // Bewear
            new(Nest039,0,0,1,SWSH) { Species = 819, Ability = A3 }, // Skwovet
            new(Nest039,0,0,1,SWSH) { Species = 831, Ability = A3 }, // Wooloo
            new(Nest039,0,1,1,SWSH) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new(Nest039,0,1,1,SWSH) { Species = 446, Ability = A3 }, // Munchlax
            new(Nest039,1,2,2,SWSH) { Species = 820, Ability = A3 }, // Greedent
            new(Nest039,2,3,3,SWSH) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new(Nest039,2,4,4,SWSH) { Species = 820, Ability = A3 }, // Greedent
            new(Nest039,2,4,4,SWSH) { Species = 832, Ability = A3 }, // Dubwool
            new(Nest039,3,4,4,SWSH) { Species = 660, Ability = A4 }, // Diggersby
            new(Nest039,4,4,4,SWSH) { Species = 143, Ability = A4 }, // Snorlax
          //new(Nest040,0,0,1,SWSH) { Species = 535, Ability = A3 }, // Tympole
          //new(Nest040,0,0,1,SWSH) { Species = 090, Ability = A3 }, // Shellder
          //new(Nest040,0,1,1,SWSH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest040,2,2,2,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
            new(Nest040,2,4,4,SWSH) { Species = 171, Ability = A3 }, // Lanturn
            new(Nest040,4,4,4,SWSH) { Species = 847, Ability = A4 }, // Barraskewda
          //new(Nest041,0,0,1,SWSH) { Species = 422, Ability = A3, Form = 1 }, // Shellos-1
          //new(Nest041,0,0,1,SWSH) { Species = 098, Ability = A3 }, // Krabby
          //new(Nest041,0,1,1,SWSH) { Species = 341, Ability = A3 }, // Corphish
          //new(Nest041,0,1,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
          //new(Nest041,1,1,2,SWSH) { Species = 688, Ability = A3 }, // Binacle
            new(Nest041,2,2,2,SWSH) { Species = 771, Ability = A3 }, // Pyukumuku
            new(Nest041,2,3,3,SWSH) { Species = 099, Ability = A3 }, // Kingler
            new(Nest041,2,4,4,SWSH) { Species = 342, Ability = A3 }, // Crawdaunt
            new(Nest041,2,4,4,SWSH) { Species = 689, Ability = A3 }, // Barbaracle
            new(Nest041,3,4,4,SWSH) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new(Nest041,4,4,4,SWSH) { Species = 593, Ability = A4 }, // Jellicent
            new(Nest041,4,4,4,SWSH) { Species = 834, Ability = A4 }, // Drednaw
            new(Nest042,0,0,1,SWSH) { Species = 092, Ability = A3 }, // Gastly
            new(Nest042,0,0,1,SWSH) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new(Nest042,0,1,1,SWSH) { Species = 854, Ability = A3 }, // Sinistea
            new(Nest042,0,1,1,SWSH) { Species = 355, Ability = A3 }, // Duskull
            new(Nest042,1,2,2,SWSH) { Species = 093, Ability = A3 }, // Haunter
            new(Nest042,2,3,3,SWSH) { Species = 356, Ability = A3 }, // Dusclops
            new(Nest042,4,4,4,SWSH) { Species = 477, Ability = A4 }, // Dusknoir
            new(Nest042,4,4,4,SWSH) { Species = 094, Ability = A4 }, // Gengar
          //new(Nest043,0,0,1,SWSH) { Species = 129, Ability = A3 }, // Magikarp
          //new(Nest043,0,0,1,SWSH) { Species = 349, Ability = A3 }, // Feebas
          //new(Nest043,0,1,1,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
          //new(Nest043,0,1,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
            new(Nest043,2,2,2,SWSH) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest043,2,3,3,SWSH) { Species = 211, Ability = A3 }, // Qwilfish
            new(Nest043,2,4,4,SWSH) { Species = 748, Ability = A3 }, // Toxapex
            new(Nest043,3,4,4,SWSH) { Species = 771, Ability = A4 }, // Pyukumuku
            new(Nest043,3,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
            new(Nest043,4,4,4,SWSH) { Species = 131, Ability = A4 }, // Lapras
            new(Nest043,4,4,4,SWSH) { Species = 350, Ability = A4 }, // Milotic
            new(Nest044,0,0,1,SWSH) { Species = 447, Ability = A3 }, // Riolu
            new(Nest044,0,0,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest044,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest044,0,1,1,SWSH) { Species = 599, Ability = A3 }, // Klink
            new(Nest044,1,2,2,SWSH) { Species = 095, Ability = A3 }, // Onix
            new(Nest044,2,4,4,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest044,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest044,3,4,4,SWSH) { Species = 208, Ability = A4 }, // Steelix
            new(Nest044,4,4,4,SWSH) { Species = 601, Ability = A4 }, // Klinklang
            new(Nest044,4,4,4,SWSH) { Species = 448, Ability = A4 }, // Lucario
            new(Nest045,0,0,1,SWSH) { Species = 767, Ability = A3 }, // Wimpod
            new(Nest045,0,0,1,SWSH) { Species = 824, Ability = A3 }, // Blipbug
            new(Nest045,0,1,1,SWSH) { Species = 751, Ability = A3 }, // Dewpider
            new(Nest045,1,2,2,SWSH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest045,2,3,3,SWSH) { Species = 825, Ability = A3 }, // Dottler
            new(Nest045,2,4,4,SWSH) { Species = 826, Ability = A3 }, // Orbeetle
            new(Nest045,3,4,4,SWSH) { Species = 752, Ability = A4 }, // Araquanid
            new(Nest045,3,4,4,SWSH) { Species = 768, Ability = A4 }, // Golisopod
            new(Nest045,4,4,4,SWSH) { Species = 292, Ability = A4 }, // Shedinja
            new(Nest046,0,0,1,SWSH) { Species = 679, Ability = A3 }, // Honedge
            new(Nest046,0,0,1,SWSH) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new(Nest046,0,1,1,SWSH) { Species = 854, Ability = A3 }, // Sinistea
            new(Nest046,0,1,1,SWSH) { Species = 425, Ability = A3 }, // Drifloon
            new(Nest046,1,2,2,SWSH) { Species = 680, Ability = A3 }, // Doublade
            new(Nest046,2,3,3,SWSH) { Species = 426, Ability = A3 }, // Drifblim
            new(Nest046,3,4,4,SWSH) { Species = 855, Ability = A4 }, // Polteageist
            new(Nest046,4,4,4,SWSH) { Species = 867, Ability = A4 }, // Runerigus
            new(Nest046,4,4,4,SWSH) { Species = 681, Ability = A4 }, // Aegislash
            new(Nest047,0,0,1,SWSH) { Species = 447, Ability = A3 }, // Riolu
            new(Nest047,0,0,1,SWSH) { Species = 066, Ability = A3 }, // Machop
            new(Nest047,0,1,1,SWSH) { Species = 759, Ability = A3 }, // Stufful
            new(Nest047,1,2,2,SWSH) { Species = 760, Ability = A3 }, // Bewear
            new(Nest047,1,3,3,SWSH) { Species = 870, Ability = A3 }, // Falinks
            new(Nest047,2,3,3,SWSH) { Species = 067, Ability = A3 }, // Machoke
            new(Nest047,3,4,4,SWSH) { Species = 068, Ability = A4 }, // Machamp
            new(Nest047,4,4,4,SWSH) { Species = 448, Ability = A4 }, // Lucario
            new(Nest047,4,4,4,SWSH) { Species = 475, Ability = A4 }, // Gallade
            new(Nest048,0,0,1,SWSH) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new(Nest048,0,0,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest048,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest048,0,1,1,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest048,1,2,2,SWSH) { Species = 679, Ability = A3 }, // Honedge
            new(Nest048,1,2,2,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest048,3,4,4,SWSH) { Species = 863, Ability = A4 }, // Perrserker
            new(Nest048,2,4,4,SWSH) { Species = 598, Ability = A3 }, // Ferrothorn
            new(Nest048,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest048,3,4,4,SWSH) { Species = 618, Ability = A4, Form = 1 }, // Stunfisk-1
            new(Nest048,4,4,4,SWSH) { Species = 879, Ability = A4 }, // Copperajah
            new(Nest048,4,4,4,SWSH) { Species = 884, Ability = A4 }, // Duraludon
            new(Nest049,0,0,1,SWSH) { Species = 686, Ability = A3 }, // Inkay
            new(Nest049,0,0,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest049,0,1,1,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest049,0,1,1,SWSH) { Species = 527, Ability = A3 }, // Woobat
            new(Nest049,1,2,2,SWSH) { Species = 856, Ability = A3 }, // Hatenna
            new(Nest049,1,2,2,SWSH) { Species = 857, Ability = A3 }, // Hattrem
            new(Nest049,2,3,3,SWSH) { Species = 281, Ability = A3 }, // Kirlia
            new(Nest049,2,4,4,SWSH) { Species = 528, Ability = A3 }, // Swoobat
            new(Nest049,3,4,4,SWSH) { Species = 858, Ability = A4 }, // Hatterene
            new(Nest049,3,4,4,SWSH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest049,4,4,4,SWSH) { Species = 687, Ability = A4 }, // Malamar
            new(Nest049,4,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest050,0,0,1,SWSH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest050,0,0,1,SWSH) { Species = 438, Ability = A3 }, // Bonsly
            new(Nest050,0,1,1,SWSH) { Species = 837, Ability = A3 }, // Rolycoly
            new(Nest050,1,2,2,SWSH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest050,2,4,4,SWSH) { Species = 095, Ability = A3 }, // Onix
            new(Nest050,3,4,4,SWSH) { Species = 558, Ability = A4 }, // Crustle
            new(Nest050,3,4,4,SWSH) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest050,4,4,4,SWSH) { Species = 208, Ability = A4 }, // Steelix
            new(Nest051,0,0,1,SWSH) { Species = 194, Ability = A3 }, // Wooper
            new(Nest051,0,0,1,SWSH) { Species = 339, Ability = A3 }, // Barboach
            new(Nest051,0,1,1,SWSH) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new(Nest051,0,1,1,SWSH) { Species = 622, Ability = A3 }, // Golett
            new(Nest051,1,2,2,SWSH) { Species = 536, Ability = A3 }, // Palpitoad
            new(Nest051,1,2,2,SWSH) { Species = 195, Ability = A3 }, // Quagsire
            new(Nest051,2,3,3,SWSH) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new(Nest051,2,4,4,SWSH) { Species = 623, Ability = A3 }, // Golurk
            new(Nest051,3,4,4,SWSH) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new(Nest051,3,4,4,SWSH) { Species = 537, Ability = A4 }, // Seismitoad
            new(Nest051,4,4,4,SWSH) { Species = 867, Ability = A4 }, // Runerigus
            new(Nest051,4,4,4,SWSH) { Species = 464, Ability = A4 }, // Rhyperior
            new(Nest052,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest052,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest052,0,1,1,SWSH) { Species = 004, Ability = A3 }, // Charmander
            new(Nest052,1,2,2,SWSH) { Species = 005, Ability = A3 }, // Charmeleon
            new(Nest052,2,3,3,SWSH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest052,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest052,3,4,4,SWSH) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new(Nest052,4,4,4,SWSH) { Species = 851, Ability = A4 }, // Centiskorch
            new(Nest052,4,4,4,SWSH) { Species = 006, Ability = A4 }, // Charizard
            new(Nest053,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest053,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest053,0,1,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest053,1,2,2,SWSH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest053,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest053,3,4,4,SWSH) { Species = 609, Ability = A4 }, // Chandelure
            new(Nest053,4,4,4,SWSH) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest054,0,0,1,SWSH) { Species = 582, Ability = A3 }, // Vanillite
            new(Nest054,0,1,1,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest054,0,1,1,SWSH) { Species = 712, Ability = A3 }, // Bergmite
            new(Nest054,1,2,2,SWSH) { Species = 361, Ability = A3 }, // Snorunt
            new(Nest054,1,2,2,SWSH) { Species = 225, Ability = A3 }, // Delibird
            new(Nest054,2,3,3,SWSH) { Species = 713, Ability = A3 }, // Avalugg
            new(Nest054,2,4,4,SWSH) { Species = 362, Ability = A3 }, // Glalie
            new(Nest054,3,4,4,SWSH) { Species = 584, Ability = A4 }, // Vanilluxe
            new(Nest054,3,4,4,SWSH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest054,4,4,4,SWSH) { Species = 131, Ability = A4 }, // Lapras
            new(Nest055,0,0,1,SWSH) { Species = 835, Ability = A3 }, // Yamper
            new(Nest055,0,0,1,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest055,0,1,1,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest055,0,1,1,SWSH) { Species = 595, Ability = A3 }, // Joltik
            new(Nest055,1,2,2,SWSH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest055,1,2,2,SWSH) { Species = 171, Ability = A3 }, // Lanturn
            new(Nest055,2,4,4,SWSH) { Species = 836, Ability = A3 }, // Boltund
            new(Nest055,2,4,4,SWSH) { Species = 849, Ability = A3 }, // Toxtricity
            new(Nest055,3,4,4,SWSH) { Species = 871, Ability = A4 }, // Pincurchin
            new(Nest055,3,4,4,SWSH) { Species = 596, Ability = A4 }, // Galvantula
            new(Nest055,4,4,4,SWSH) { Species = 777, Ability = A4 }, // Togedemaru
            new(Nest055,4,4,4,SWSH) { Species = 877, Ability = A4 }, // Morpeko
            new(Nest056,0,0,1,SWSH) { Species = 172, Ability = A3 }, // Pichu
            new(Nest056,0,0,1,SWSH) { Species = 309, Ability = A3 }, // Electrike
            new(Nest056,0,1,1,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest056,0,1,1,SWSH) { Species = 694, Ability = A3 }, // Helioptile
            new(Nest056,1,2,2,SWSH) { Species = 595, Ability = A3 }, // Joltik
            new(Nest056,1,2,2,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest056,2,4,4,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest056,2,4,4,SWSH) { Species = 479, Ability = A3, Form = 5 }, // Rotom-5
            new(Nest056,3,4,4,SWSH) { Species = 479, Ability = A4, Form = 4 }, // Rotom-4
            new(Nest056,3,4,4,SWSH) { Species = 479, Ability = A4, Form = 3 }, // Rotom-3
            new(Nest056,4,4,4,SWSH) { Species = 479, Ability = A4, Form = 2 }, // Rotom-2
            new(Nest056,4,4,4,SWSH) { Species = 479, Ability = A4, Form = 1 }, // Rotom-1
            new(Nest057,0,0,1,SWSH) { Species = 406, Ability = A3 }, // Budew
            new(Nest057,0,1,1,SWSH) { Species = 829, Ability = A3 }, // Gossifleur
            new(Nest057,0,1,1,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest057,1,2,2,SWSH) { Species = 840, Ability = A3 }, // Applin
            new(Nest057,2,4,4,SWSH) { Species = 315, Ability = A3 }, // Roselia
            new(Nest057,3,4,4,SWSH) { Species = 830, Ability = A4 }, // Eldegoss
            new(Nest057,3,4,4,SWSH) { Species = 598, Ability = A4 }, // Ferrothorn
            new(Nest057,4,4,4,SWSH) { Species = 407, Ability = A4 }, // Roserade
            new(Nest058,0,0,1,SWSH) { Species = 420, Ability = A3 }, // Cherubi
            new(Nest058,0,1,1,SWSH) { Species = 829, Ability = A3 }, // Gossifleur
            new(Nest058,0,1,1,SWSH) { Species = 546, Ability = A3 }, // Cottonee
            new(Nest058,1,2,2,SWSH) { Species = 755, Ability = A3 }, // Morelull
            new(Nest058,2,4,4,SWSH) { Species = 421, Ability = A3 }, // Cherrim
            new(Nest058,2,4,4,SWSH) { Species = 756, Ability = A3 }, // Shiinotic
            new(Nest058,3,4,4,SWSH) { Species = 830, Ability = A4 }, // Eldegoss
            new(Nest058,3,4,4,SWSH) { Species = 547, Ability = A4 }, // Whimsicott
            new(Nest058,4,4,4,SWSH) { Species = 781, Ability = A4 }, // Dhelmise
            new(Nest059,0,0,1,SWSH) { Species = 434, Ability = A3 }, // Stunky
            new(Nest059,0,0,1,SWSH) { Species = 568, Ability = A3 }, // Trubbish
            new(Nest059,0,1,1,SWSH) { Species = 451, Ability = A3 }, // Skorupi
            new(Nest059,0,1,1,SWSH) { Species = 109, Ability = A3 }, // Koffing
            new(Nest059,1,2,2,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest059,2,4,4,SWSH) { Species = 569, Ability = A3 }, // Garbodor
            new(Nest059,2,4,4,SWSH) { Species = 452, Ability = A3 }, // Drapion
            new(Nest059,3,4,4,SWSH) { Species = 849, Ability = A4 }, // Toxtricity
            new(Nest059,3,4,4,SWSH) { Species = 435, Ability = A4 }, // Skuntank
            new(Nest059,4,4,4,SWSH) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new(Nest060,0,0,1,SWSH) { Species = 177, Ability = A3 }, // Natu
            new(Nest060,0,0,1,SWSH) { Species = 163, Ability = A3 }, // Hoothoot
            new(Nest060,0,1,1,SWSH) { Species = 821, Ability = A3 }, // Rookidee
            new(Nest060,0,1,1,SWSH) { Species = 278, Ability = A3 }, // Wingull
            new(Nest060,1,2,2,SWSH) { Species = 012, Ability = A3 }, // Butterfree
            new(Nest060,1,2,2,SWSH) { Species = 822, Ability = A3 }, // Corvisquire
            new(Nest060,2,4,4,SWSH) { Species = 164, Ability = A3 }, // Noctowl
            new(Nest060,2,4,4,SWSH) { Species = 279, Ability = A3 }, // Pelipper
            new(Nest060,3,4,4,SWSH) { Species = 178, Ability = A4 }, // Xatu
            new(Nest060,3,4,4,SWSH) { Species = 701, Ability = A4 }, // Hawlucha
            new(Nest060,4,4,4,SWSH) { Species = 823, Ability = A4 }, // Corviknight
            new(Nest060,4,4,4,SWSH) { Species = 225, Ability = A4 }, // Delibird
            new(Nest061,0,0,1,SWSH) { Species = 175, Ability = A3 }, // Togepi
            new(Nest061,0,0,1,SWSH) { Species = 755, Ability = A3 }, // Morelull
            new(Nest061,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest061,0,1,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest061,1,2,2,SWSH) { Species = 176, Ability = A3 }, // Togetic
            new(Nest061,1,2,2,SWSH) { Species = 756, Ability = A3 }, // Shiinotic
            new(Nest061,2,4,4,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest061,3,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest061,3,4,4,SWSH) { Species = 468, Ability = A4 }, // Togekiss
            new(Nest061,4,4,4,SWSH) { Species = 861, Ability = A4 }, // Grimmsnarl
            new(Nest061,4,4,4,SWSH) { Species = 778, Ability = A4 }, // Mimikyu
            new(Nest062,0,0,1,SWSH) { Species = 827, Ability = A3 }, // Nickit
            new(Nest062,0,0,1,SWSH) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new(Nest062,0,1,1,SWSH) { Species = 215, Ability = A3 }, // Sneasel
            new(Nest062,1,2,2,SWSH) { Species = 510, Ability = A3 }, // Liepard
            new(Nest062,1,2,2,SWSH) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new(Nest062,2,4,4,SWSH) { Species = 828, Ability = A3 }, // Thievul
            new(Nest062,2,4,4,SWSH) { Species = 675, Ability = A3 }, // Pangoro
            new(Nest062,3,4,4,SWSH) { Species = 461, Ability = A4 }, // Weavile
            new(Nest062,4,4,4,SWSH) { Species = 862, Ability = A4 }, // Obstagoon
            new(Nest063,0,0,1,SWSH) { Species = 840, Ability = A3 }, // Applin
            new(Nest063,1,2,2,SWSH) { Species = 885, Ability = A3 }, // Dreepy
            new(Nest063,3,4,4,SWSH) { Species = 886, Ability = A4 }, // Drakloak
            new(Nest063,4,4,4,SWSH) { Species = 887, Ability = A4 }, // Dragapult
            new(Nest064,0,0,1,SWSH) { Species = 659, Ability = A3 }, // Bunnelby
            new(Nest064,0,0,1,SWSH) { Species = 519, Ability = A3 }, // Pidove
            new(Nest064,0,1,1,SWSH) { Species = 819, Ability = A3 }, // Skwovet
            new(Nest064,0,1,1,SWSH) { Species = 133, Ability = A3 }, // Eevee
            new(Nest064,1,2,2,SWSH) { Species = 520, Ability = A3 }, // Tranquill
            new(Nest064,1,2,2,SWSH) { Species = 831, Ability = A3 }, // Wooloo
            new(Nest064,2,4,4,SWSH) { Species = 521, Ability = A3 }, // Unfezant
            new(Nest064,2,4,4,SWSH) { Species = 832, Ability = A3 }, // Dubwool
            new(Nest064,4,4,4,SWSH) { Species = 133, Ability = A4 }, // Eevee
            new(Nest064,4,4,4,SWSH) { Species = 143, Ability = A4 }, // Snorlax
            new(Nest065,0,0,1,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest065,0,1,2,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest065,1,2,3,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest065,2,3,3,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest065,3,4,4,SWSH) { Species = 132, Ability = A4 }, // Ditto
            new(Nest065,4,4,4,SWSH) { Species = 132, Ability = A4 }, // Ditto
          //new(Nest066,0,0,1,SWSH) { Species = 458, Ability = A3 }, // Mantyke
          //new(Nest066,0,0,1,SWSH) { Species = 341, Ability = A3 }, // Corphish
          //new(Nest066,0,1,1,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
          //new(Nest066,0,1,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
            new(Nest066,2,2,2,SWSH) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest066,2,3,3,SWSH) { Species = 342, Ability = A3 }, // Crawdaunt
            new(Nest066,2,4,4,SWSH) { Species = 748, Ability = A3 }, // Toxapex
            new(Nest066,3,4,4,SWSH) { Species = 771, Ability = A4 }, // Pyukumuku
            new(Nest066,3,4,4,SWSH) { Species = 226, Ability = A4 }, // Mantine
            new(Nest066,4,4,4,SWSH) { Species = 131, Ability = A4 }, // Lapras
            new(Nest066,4,4,4,SWSH) { Species = 134, Ability = A4 }, // Vaporeon
            new(Nest067,0,0,1,SWSH) { Species = 686, Ability = A3 }, // Inkay
            new(Nest067,0,0,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest067,0,1,1,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest067,0,1,1,SWSH) { Species = 527, Ability = A3 }, // Woobat
            new(Nest067,1,2,2,SWSH) { Species = 856, Ability = A3 }, // Hatenna
            new(Nest067,1,2,2,SWSH) { Species = 857, Ability = A3 }, // Hattrem
            new(Nest067,2,3,3,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest067,2,4,4,SWSH) { Species = 528, Ability = A3 }, // Swoobat
            new(Nest067,3,4,4,SWSH) { Species = 687, Ability = A4 }, // Malamar
            new(Nest067,3,4,4,SWSH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest067,4,4,4,SWSH) { Species = 858, Ability = A4 }, // Hatterene
            new(Nest067,4,4,4,SWSH) { Species = 196, Ability = A4 }, // Espeon
            new(Nest068,0,0,1,SWSH) { Species = 827, Ability = A3 }, // Nickit
            new(Nest068,0,0,1,SWSH) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new(Nest068,0,1,1,SWSH) { Species = 686, Ability = A3 }, // Inkay
            new(Nest068,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest068,1,2,2,SWSH) { Species = 510, Ability = A3 }, // Liepard
            new(Nest068,1,2,2,SWSH) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new(Nest068,2,4,4,SWSH) { Species = 828, Ability = A3 }, // Thievul
            new(Nest068,2,4,4,SWSH) { Species = 675, Ability = A3 }, // Pangoro
            new(Nest068,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest068,3,4,4,SWSH) { Species = 687, Ability = A4 }, // Malamar
            new(Nest068,4,4,4,SWSH) { Species = 862, Ability = A4 }, // Obstagoon
            new(Nest068,4,4,4,SWSH) { Species = 197, Ability = A4 }, // Umbreon
            new(Nest069,0,0,1,SWSH) { Species = 420, Ability = A3 }, // Cherubi
            new(Nest069,0,0,1,SWSH) { Species = 761, Ability = A3 }, // Bounsweet
            new(Nest069,0,1,1,SWSH) { Species = 829, Ability = A3 }, // Gossifleur
            new(Nest069,0,1,1,SWSH) { Species = 546, Ability = A3 }, // Cottonee
            new(Nest069,1,2,2,SWSH) { Species = 762, Ability = A3 }, // Steenee
            new(Nest069,1,2,2,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest069,2,4,4,SWSH) { Species = 421, Ability = A3 }, // Cherrim
            new(Nest069,2,4,4,SWSH) { Species = 598, Ability = A3 }, // Ferrothorn
            new(Nest069,3,4,4,SWSH) { Species = 830, Ability = A4 }, // Eldegoss
            new(Nest069,3,4,4,SWSH) { Species = 763, Ability = A4 }, // Tsareena
            new(Nest069,4,4,4,SWSH) { Species = 547, Ability = A4 }, // Whimsicott
            new(Nest069,4,4,4,SWSH) { Species = 470, Ability = A4 }, // Leafeon
            new(Nest070,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest070,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest070,1,2,2,SWSH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest070,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest070,3,4,4,SWSH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest070,3,4,4,SWSH) { Species = 038, Ability = A4 }, // Ninetales
            new(Nest070,4,4,4,SWSH) { Species = 609, Ability = A4 }, // Chandelure
            new(Nest070,4,4,4,SWSH) { Species = 136, Ability = A4 }, // Flareon
            new(Nest071,0,0,1,SWSH) { Species = 835, Ability = A3 }, // Yamper
            new(Nest071,0,0,1,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest071,0,1,1,SWSH) { Species = 025, Ability = A3 }, // Pikachu
            new(Nest071,0,1,1,SWSH) { Species = 694, Ability = A3 }, // Helioptile
            new(Nest071,1,2,2,SWSH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest071,1,2,2,SWSH) { Species = 171, Ability = A3 }, // Lanturn
            new(Nest071,2,4,4,SWSH) { Species = 836, Ability = A3 }, // Boltund
            new(Nest071,2,4,4,SWSH) { Species = 849, Ability = A3 }, // Toxtricity
            new(Nest071,3,4,4,SWSH) { Species = 695, Ability = A4 }, // Heliolisk
            new(Nest071,3,4,4,SWSH) { Species = 738, Ability = A4 }, // Vikavolt
            new(Nest071,4,4,4,SWSH) { Species = 025, Ability = A4 }, // Pikachu
            new(Nest071,4,4,4,SWSH) { Species = 135, Ability = A4 }, // Jolteon
            new(Nest072,0,0,1,SWSH) { Species = 582, Ability = A3 }, // Vanillite
            new(Nest072,0,0,1,SWSH) { Species = 872, Ability = A3 }, // Snom
            new(Nest072,0,1,1,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest072,0,1,1,SWSH) { Species = 712, Ability = A3 }, // Bergmite
            new(Nest072,1,2,2,SWSH) { Species = 361, Ability = A3 }, // Snorunt
            new(Nest072,1,2,2,SWSH) { Species = 583, Ability = A3 }, // Vanillish
            new(Nest072,2,3,3,SWSH) { Species = 713, Ability = A3 }, // Avalugg
            new(Nest072,2,4,4,SWSH) { Species = 873, Ability = A3 }, // Frosmoth
            new(Nest072,3,4,4,SWSH) { Species = 584, Ability = A4 }, // Vanilluxe
            new(Nest072,3,4,4,SWSH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest072,4,4,4,SWSH) { Species = 478, Ability = A4 }, // Froslass
            new(Nest072,4,4,4,SWSH) { Species = 471, Ability = A4 }, // Glaceon
          //new(Nest073,0,0,1,SWSH) { Species = 175, Ability = A3 }, // Togepi
          //new(Nest073,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
          //new(Nest073,0,1,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest073,2,2,2,SWSH) { Species = 176, Ability = A3 }, // Togetic
            new(Nest073,2,2,2,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest073,2,4,4,SWSH) { Species = 868, Ability = A3 }, // Milcery
            new(Nest073,3,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest073,3,4,4,SWSH) { Species = 861, Ability = A4 }, // Grimmsnarl
            new(Nest073,4,4,4,SWSH) { Species = 468, Ability = A4 }, // Togekiss
            new(Nest073,4,4,4,SWSH) { Species = 700, Ability = A4 }, // Sylveon
            new(Nest074,0,0,1,SWSH) { Species = 129, Ability = A3 }, // Magikarp
            new(Nest074,0,0,1,SWSH) { Species = 751, Ability = A3 }, // Dewpider
            new(Nest074,0,1,1,SWSH) { Species = 194, Ability = A3 }, // Wooper
            new(Nest074,0,1,1,SWSH) { Species = 339, Ability = A3 }, // Barboach
            new(Nest074,1,2,2,SWSH) { Species = 098, Ability = A3 }, // Krabby
            new(Nest074,1,2,2,SWSH) { Species = 746, Ability = A3 }, // Wishiwashi
            new(Nest074,2,3,3,SWSH) { Species = 099, Ability = A3 }, // Kingler
            new(Nest074,2,4,4,SWSH) { Species = 340, Ability = A3 }, // Whiscash
            new(Nest074,3,4,4,SWSH) { Species = 211, Ability = A4 }, // Qwilfish
            new(Nest074,3,4,4,SWSH) { Species = 195, Ability = A4 }, // Quagsire
            new(Nest074,4,4,4,SWSH) { Species = 752, Ability = A4 }, // Araquanid
            new(Nest074,4,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
          //new(Nest075,0,0,1,SWSH) { Species = 458, Ability = A3 }, // Mantyke
          //new(Nest075,0,0,1,SWSH) { Species = 223, Ability = A3 }, // Remoraid
          //new(Nest075,0,1,1,SWSH) { Species = 320, Ability = A3 }, // Wailmer
          //new(Nest075,0,1,1,SWSH) { Species = 688, Ability = A3 }, // Binacle
            new(Nest075,2,2,2,SWSH) { Species = 098, Ability = A3 }, // Krabby
            new(Nest075,2,2,2,SWSH) { Species = 771, Ability = A3 }, // Pyukumuku
            new(Nest075,2,3,3,SWSH) { Species = 099, Ability = A3 }, // Kingler
            new(Nest075,3,4,4,SWSH) { Species = 211, Ability = A4 }, // Qwilfish
            new(Nest075,3,4,4,SWSH) { Species = 224, Ability = A4 }, // Octillery
            new(Nest075,4,4,4,SWSH) { Species = 321, Ability = A4 }, // Wailord
            new(Nest075,4,4,4,SWSH) { Species = 226, Ability = A4 }, // Mantine
          //new(Nest076,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
          //new(Nest076,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
          //new(Nest076,0,1,1,SWSH) { Species = 004, Ability = A3 }, // Charmander
            new(Nest076,2,2,2,SWSH) { Species = 005, Ability = A3 }, // Charmeleon
            new(Nest076,2,3,3,SWSH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest076,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest076,3,4,4,SWSH) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new(Nest076,4,4,4,SWSH) { Species = 851, Ability = A4 }, // Centiskorch
            new(Nest076,4,4,4,SWSH) { Species = 006, Ability = A4, CanGigantamax = true }, // Charizard
            new(Nest077,0,0,1,SWSH) { Species = 129, Ability = A3 }, // Magikarp
            new(Nest077,0,0,1,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
            new(Nest077,0,1,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
            new(Nest077,0,1,1,SWSH) { Species = 098, Ability = A3 }, // Krabby
            new(Nest077,1,2,2,SWSH) { Species = 771, Ability = A3 }, // Pyukumuku
            new(Nest077,2,3,3,SWSH) { Species = 211, Ability = A3 }, // Qwilfish
            new(Nest077,2,4,4,SWSH) { Species = 099, Ability = A3 }, // Kingler
            new(Nest077,3,4,4,SWSH) { Species = 746, Ability = A4 }, // Wishiwashi
            new(Nest077,3,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
            new(Nest077,4,4,4,SWSH) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new(Nest077,4,4,4,SWSH) { Species = 834, Ability = A4, CanGigantamax = true }, // Drednaw
            new(Nest078,0,0,1,SWSH) { Species = 406, Ability = A3 }, // Budew
            new(Nest078,0,1,1,SWSH) { Species = 829, Ability = A3 }, // Gossifleur
            new(Nest078,0,1,1,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest078,1,2,2,SWSH) { Species = 840, Ability = A3 }, // Applin
            new(Nest078,2,4,4,SWSH) { Species = 315, Ability = A3 }, // Roselia
            new(Nest078,3,4,4,SWSH) { Species = 830, Ability = A4 }, // Eldegoss
            new(Nest078,3,4,4,SWSH) { Species = 598, Ability = A4 }, // Ferrothorn
            new(Nest078,4,4,4,SWSH) { Species = 407, Ability = A4 }, // Roserade
            new(Nest079,0,0,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest079,0,1,1,SWSH) { Species = 607, Ability = A3 }, // Litwick
            new(Nest079,0,1,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest079,1,2,2,SWSH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest079,1,2,2,SWSH) { Species = 608, Ability = A3 }, // Lampent
            new(Nest079,2,3,3,SWSH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest079,2,4,4,SWSH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest079,3,4,4,SWSH) { Species = 609, Ability = A4 }, // Chandelure
            new(Nest079,4,4,4,SWSH) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest079,4,4,4,SWSH) { Species = 851, Ability = A4, CanGigantamax = true }, // Centiskorch
            new(Nest081,0,0,1,SWSH) { Species = 175, Ability = A3 }, // Togepi
            new(Nest081,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest081,0,1,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest081,1,2,2,SWSH) { Species = 176, Ability = A3 }, // Togetic
            new(Nest081,1,2,2,SWSH) { Species = 756, Ability = A3 }, // Shiinotic
            new(Nest081,2,3,3,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest081,3,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest081,3,4,4,SWSH) { Species = 468, Ability = A4 }, // Togekiss
            new(Nest081,4,4,4,SWSH) { Species = 861, Ability = A4 }, // Grimmsnarl
            new(Nest081,4,4,4,SWSH) { Species = 869, Ability = A4, CanGigantamax = true }, // Alcremie
            new(Nest083,0,0,1,SWSH) { Species = 447, Ability = A3 }, // Riolu
            new(Nest083,0,0,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest083,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest083,0,1,1,SWSH) { Species = 599, Ability = A3 }, // Klink
            new(Nest083,1,2,2,SWSH) { Species = 095, Ability = A3 }, // Onix
            new(Nest083,2,4,4,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest083,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest083,3,4,4,SWSH) { Species = 208, Ability = A4 }, // Steelix
            new(Nest083,4,4,4,SWSH) { Species = 601, Ability = A4 }, // Klinklang
            new(Nest083,4,4,4,SWSH) { Species = 884, Ability = A4, CanGigantamax = true }, // Duraludon
            new(Nest084,0,0,1,SWSH) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new(Nest084,0,0,1,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest084,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest084,0,1,1,SWSH) { Species = 597, Ability = A3 }, // Ferroseed
            new(Nest084,1,2,2,SWSH) { Species = 679, Ability = A3 }, // Honedge
            new(Nest084,1,2,2,SWSH) { Species = 437, Ability = A3 }, // Bronzong
            new(Nest084,2,3,3,SWSH) { Species = 863, Ability = A3 }, // Perrserker
            new(Nest084,2,4,4,SWSH) { Species = 598, Ability = A3 }, // Ferrothorn
            new(Nest084,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest084,3,4,4,SWSH) { Species = 618, Ability = A4, Form = 1 }, // Stunfisk-1
            new(Nest084,4,4,4,SWSH) { Species = 884, Ability = A4 }, // Duraludon
            new(Nest084,4,4,4,SWSH) { Species = 879, Ability = A4, CanGigantamax = true }, // Copperajah
            new(Nest085,0,0,1,SWSH) { Species = 434, Ability = A3 }, // Stunky
            new(Nest085,0,0,1,SWSH) { Species = 568, Ability = A3 }, // Trubbish
            new(Nest085,0,1,1,SWSH) { Species = 451, Ability = A3 }, // Skorupi
            new(Nest085,0,1,1,SWSH) { Species = 109, Ability = A3 }, // Koffing
            new(Nest085,1,2,2,SWSH) { Species = 848, Ability = A3 }, // Toxel
            new(Nest085,2,3,3,SWSH) { Species = 452, Ability = A3 }, // Drapion
            new(Nest085,2,4,4,SWSH) { Species = 849, Ability = A3 }, // Toxtricity
            new(Nest085,3,4,4,SWSH) { Species = 435, Ability = A4 }, // Skuntank
            new(Nest085,3,4,4,SWSH) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new(Nest085,4,4,4,SWSH) { Species = 569, Ability = A4, CanGigantamax = true }, // Garbodor
            new(Nest086,0,0,1,SWSH) { Species = 175, Ability = A3 }, // Togepi
            new(Nest086,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest086,0,1,1,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest086,1,2,2,SWSH) { Species = 176, Ability = A3 }, // Togetic
            new(Nest086,1,2,2,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest086,2,4,4,SWSH) { Species = 868, Ability = A3 }, // Milcery
            new(Nest086,3,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest086,3,4,4,SWSH) { Species = 861, Ability = A4 }, // Grimmsnarl
            new(Nest086,4,4,4,SWSH) { Species = 468, Ability = A4 }, // Togekiss
            new(Nest086,4,4,4,SWSH) { Species = 858, Ability = A4, CanGigantamax = true }, // Hatterene
            new(Nest087,0,0,1,SWSH) { Species = 827, Ability = A3 }, // Nickit
            new(Nest087,0,0,1,SWSH) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new(Nest087,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest087,1,2,2,SWSH) { Species = 510, Ability = A3 }, // Liepard
            new(Nest087,1,2,2,SWSH) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new(Nest087,2,3,3,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest087,2,4,4,SWSH) { Species = 828, Ability = A3 }, // Thievul
            new(Nest087,3,4,4,SWSH) { Species = 675, Ability = A4 }, // Pangoro
            new(Nest087,4,4,4,SWSH) { Species = 861, Ability = A4, CanGigantamax = true }, // Grimmsnarl
            new(Nest088,0,0,1,SWSH) { Species = 177, Ability = A3 }, // Natu
            new(Nest088,0,0,1,SWSH) { Species = 163, Ability = A3 }, // Hoothoot
            new(Nest088,0,1,1,SWSH) { Species = 821, Ability = A3 }, // Rookidee
            new(Nest088,0,1,1,SWSH) { Species = 278, Ability = A3 }, // Wingull
            new(Nest088,1,2,2,SWSH) { Species = 012, Ability = A3 }, // Butterfree
            new(Nest088,1,2,2,SWSH) { Species = 822, Ability = A3 }, // Corvisquire
            new(Nest088,2,3,3,SWSH) { Species = 164, Ability = A3 }, // Noctowl
            new(Nest088,2,4,4,SWSH) { Species = 279, Ability = A3 }, // Pelipper
            new(Nest088,3,4,4,SWSH) { Species = 178, Ability = A4 }, // Xatu
            new(Nest088,3,4,4,SWSH) { Species = 701, Ability = A4 }, // Hawlucha
            new(Nest088,4,4,4,SWSH) { Species = 561, Ability = A4 }, // Sigilyph
            new(Nest088,4,4,4,SWSH) { Species = 823, Ability = A4, CanGigantamax = true }, // Corviknight
            new(Nest089,0,0,1,SWSH) { Species = 767, Ability = A3 }, // Wimpod
            new(Nest089,0,0,1,SWSH) { Species = 824, Ability = A3 }, // Blipbug
            new(Nest089,0,1,1,SWSH) { Species = 751, Ability = A3 }, // Dewpider
            new(Nest089,1,2,2,SWSH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest089,2,3,3,SWSH) { Species = 825, Ability = A3 }, // Dottler
            new(Nest089,2,4,4,SWSH) { Species = 826, Ability = A3 }, // Orbeetle
            new(Nest089,3,4,4,SWSH) { Species = 752, Ability = A4 }, // Araquanid
            new(Nest089,3,4,4,SWSH) { Species = 768, Ability = A4 }, // Golisopod
            new(Nest089,0,4,4,SWSH) { Species = 012, Ability = A4, CanGigantamax = true }, // Butterfree
            new(Nest090,0,0,1,SWSH) { Species = 341, Ability = A3 }, // Corphish
            new(Nest090,0,0,1,SWSH) { Species = 098, Ability = A3 }, // Krabby
            new(Nest090,0,1,1,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
            new(Nest090,0,1,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
            new(Nest090,1,2,2,SWSH) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest090,2,3,3,SWSH) { Species = 342, Ability = A3 }, // Crawdaunt
            new(Nest090,2,4,4,SWSH) { Species = 748, Ability = A3 }, // Toxapex
            new(Nest090,3,4,4,SWSH) { Species = 771, Ability = A4 }, // Pyukumuku
            new(Nest090,3,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
            new(Nest090,4,4,4,SWSH) { Species = 131, Ability = A4 }, // Lapras
            new(Nest090,1,4,4,SWSH) { Species = 099, Ability = A4, CanGigantamax = true }, // Kingler
            new(Nest091,0,0,1,SWSH) { Species = 767, Ability = A3 }, // Wimpod
            new(Nest091,0,0,1,SWSH) { Species = 824, Ability = A3 }, // Blipbug
            new(Nest091,0,1,1,SWSH) { Species = 751, Ability = A3 }, // Dewpider
            new(Nest091,1,2,2,SWSH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest091,2,3,3,SWSH) { Species = 825, Ability = A3 }, // Dottler
            new(Nest091,2,4,4,SWSH) { Species = 826, Ability = A3 }, // Orbeetle
            new(Nest091,3,4,4,SWSH) { Species = 752, Ability = A4 }, // Araquanid
            new(Nest091,3,4,4,SWSH) { Species = 768, Ability = A4 }, // Golisopod
            new(Nest091,2,4,4,SWSH) { Species = 826, Ability = A4, CanGigantamax = true }, // Orbeetle
            new(Nest092,0,0,1,SWSH) { Species = 194, Ability = A3 }, // Wooper
            new(Nest092,0,0,1,SWSH) { Species = 339, Ability = A3 }, // Barboach
            new(Nest092,0,1,1,SWSH) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new(Nest092,0,1,1,SWSH) { Species = 622, Ability = A3 }, // Golett
            new(Nest092,1,2,2,SWSH) { Species = 536, Ability = A3 }, // Palpitoad
            new(Nest092,1,2,2,SWSH) { Species = 195, Ability = A3 }, // Quagsire
            new(Nest092,2,3,3,SWSH) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new(Nest092,2,4,4,SWSH) { Species = 623, Ability = A3 }, // Golurk
            new(Nest092,3,4,4,SWSH) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new(Nest092,3,4,4,SWSH) { Species = 537, Ability = A4 }, // Seismitoad
            new(Nest092,4,4,4,SWSH) { Species = 464, Ability = A4 }, // Rhyperior
            new(Nest092,3,4,4,SWSH) { Species = 844, Ability = A4, CanGigantamax = true }, // Sandaconda
            new(Nest098,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest098,0,1,1,SWSH) { Species = 174, Ability = A3 }, // Igglybuff
            new(Nest098,0,1,1,SWSH) { Species = 506, Ability = A3 }, // Lillipup
            new(Nest098,1,2,2,SWSH) { Species = 427, Ability = A3 }, // Buneary
            new(Nest098,1,2,2,SWSH) { Species = 039, Ability = A3 }, // Jigglypuff
            new(Nest098,2,3,3,SWSH) { Species = 039, Ability = A3 }, // Jigglypuff
            new(Nest098,2,3,3,SWSH) { Species = 507, Ability = A3 }, // Herdier
            new(Nest098,3,4,4,SWSH) { Species = 428, Ability = A4 }, // Lopunny
            new(Nest098,3,4,4,SWSH) { Species = 040, Ability = A4 }, // Wigglytuff
            new(Nest098,4,4,4,SWSH) { Species = 206, Ability = A4 }, // Dunsparce
            new(Nest098,4,4,4,SWSH) { Species = 508, Ability = A4 }, // Stoutland
            new(Nest099,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest099,0,1,1,SWSH) { Species = 506, Ability = A2 }, // Lillipup
            new(Nest099,0,1,1,SWSH) { Species = 759, Ability = A2 }, // Stufful
            new(Nest099,1,2,2,SWSH) { Species = 039, Ability = A2 }, // Jigglypuff
            new(Nest099,1,2,2,SWSH) { Species = 427, Ability = A2 }, // Buneary
            new(Nest099,2,3,3,SWSH) { Species = 039, Ability = A2 }, // Jigglypuff
            new(Nest099,2,3,3,SWSH) { Species = 206, Ability = A2 }, // Dunsparce
            new(Nest099,3,4,4,SWSH) { Species = 832, Ability = A2 }, // Dubwool
            new(Nest099,3,4,4,SWSH) { Species = 428, Ability = A2 }, // Lopunny
            new(Nest099,3,4,4,SWSH) { Species = 508, Ability = A2 }, // Stoutland
            new(Nest099,4,4,4,SWSH) { Species = 760, Ability = A2 }, // Bewear
            new(Nest099,4,4,4,SWSH) { Species = 040, Ability = A2 }, // Wigglytuff
            new(Nest100,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest100,0,1,1,SWSH) { Species = 293, Ability = A3 }, // Whismur
            new(Nest100,0,1,1,SWSH) { Species = 108, Ability = A3 }, // Lickitung
            new(Nest100,1,2,2,SWSH) { Species = 241, Ability = A3 }, // Miltank
            new(Nest100,1,2,2,SWSH) { Species = 294, Ability = A3 }, // Loudred
            new(Nest100,2,3,3,SWSH) { Species = 294, Ability = A3 }, // Loudred
            new(Nest100,2,3,3,SWSH) { Species = 108, Ability = A3 }, // Lickitung
            new(Nest100,3,4,4,SWSH) { Species = 241, Ability = A4 }, // Miltank
            new(Nest100,3,4,4,SWSH) { Species = 626, Ability = A4 }, // Bouffalant
            new(Nest100,3,4,4,SWSH) { Species = 128, Ability = A4 }, // Tauros
            new(Nest100,4,4,4,SWSH) { Species = 295, Ability = A4 }, // Exploud
            new(Nest100,4,4,4,SWSH) { Species = 463, Ability = A4 }, // Lickilicky
            new(Nest101,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest101,0,1,1,SWSH) { Species = 293, Ability = A2 }, // Whismur
            new(Nest101,0,1,1,SWSH) { Species = 128, Ability = A2 }, // Tauros
            new(Nest101,1,2,2,SWSH) { Species = 108, Ability = A2 }, // Lickitung
            new(Nest101,1,2,2,SWSH) { Species = 241, Ability = A2 }, // Miltank
            new(Nest101,2,3,3,SWSH) { Species = 241, Ability = A2 }, // Miltank
            new(Nest101,2,3,3,SWSH) { Species = 626, Ability = A2 }, // Bouffalant
            new(Nest101,3,4,4,SWSH) { Species = 128, Ability = A2 }, // Tauros
            new(Nest101,3,4,4,SWSH) { Species = 295, Ability = A2 }, // Exploud
            new(Nest101,3,4,4,SWSH) { Species = 573, Ability = A2 }, // Cinccino
            new(Nest101,4,4,4,SWSH) { Species = 295, Ability = A2 }, // Exploud
            new(Nest101,4,4,4,SWSH) { Species = 463, Ability = A2 }, // Lickilicky
            new(Nest102,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest102,0,1,1,SWSH) { Species = 027, Ability = A3 }, // Sandshrew
            new(Nest102,0,1,1,SWSH) { Species = 551, Ability = A3 }, // Sandile
            new(Nest102,1,2,2,SWSH) { Species = 104, Ability = A3 }, // Cubone
            new(Nest102,1,2,2,SWSH) { Species = 027, Ability = A3 }, // Sandshrew
            new(Nest102,2,3,3,SWSH) { Species = 552, Ability = A3 }, // Krokorok
            new(Nest102,2,3,3,SWSH) { Species = 028, Ability = A3 }, // Sandslash
            new(Nest102,3,4,4,SWSH) { Species = 844, Ability = A4 }, // Sandaconda
            new(Nest102,3,4,4,SWSH) { Species = 028, Ability = A4 }, // Sandslash
            new(Nest102,3,4,4,SWSH) { Species = 105, Ability = A4 }, // Marowak
            new(Nest102,4,4,4,SWSH) { Species = 553, Ability = A4 }, // Krookodile
            new(Nest102,4,4,4,SWSH) { Species = 115, Ability = A4 }, // Kangaskhan
            new(Nest103,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest103,0,1,1,SWSH) { Species = 027, Ability = A2 }, // Sandshrew
            new(Nest103,0,1,1,SWSH) { Species = 104, Ability = A2 }, // Cubone
            new(Nest103,1,2,2,SWSH) { Species = 328, Ability = A2 }, // Trapinch
            new(Nest103,2,3,3,SWSH) { Species = 552, Ability = A2 }, // Krokorok
            new(Nest103,2,3,3,SWSH) { Species = 028, Ability = A2 }, // Sandslash
            new(Nest103,3,4,4,SWSH) { Species = 105, Ability = A2 }, // Marowak
            new(Nest103,3,4,4,SWSH) { Species = 553, Ability = A2 }, // Krookodile
            new(Nest103,3,4,4,SWSH) { Species = 115, Ability = A2 }, // Kangaskhan
            new(Nest103,4,4,4,SWSH) { Species = 330, Ability = A2 }, // Flygon
            new(Nest103,4,4,4,SWSH) { Species = 623, Ability = A2 }, // Golurk
            new(Nest104,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest104,0,1,1,SWSH) { Species = 702, Ability = A3 }, // Dedenne
            new(Nest104,0,1,1,SWSH) { Species = 081, Ability = A3 }, // Magnemite
            new(Nest104,1,2,2,SWSH) { Species = 403, Ability = A3 }, // Shinx
            new(Nest104,1,2,2,SWSH) { Species = 877, Ability = A3 }, // Morpeko
            new(Nest104,2,3,3,SWSH) { Species = 702, Ability = A3 }, // Dedenne
            new(Nest104,2,3,3,SWSH) { Species = 404, Ability = A3 }, // Luxio
            new(Nest104,3,4,4,SWSH) { Species = 702, Ability = A4 }, // Dedenne
            new(Nest104,3,4,4,SWSH) { Species = 082, Ability = A4 }, // Magneton
            new(Nest104,3,4,4,SWSH) { Species = 871, Ability = A4 }, // Pincurchin
            new(Nest104,4,4,4,SWSH) { Species = 405, Ability = A4 }, // Luxray
            new(Nest104,4,4,4,SWSH) { Species = 462, Ability = A4 }, // Magnezone
            new(Nest105,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest105,0,1,1,SWSH) { Species = 403, Ability = A2 }, // Shinx
            new(Nest105,0,1,1,SWSH) { Species = 172, Ability = A2 }, // Pichu
            new(Nest105,1,2,2,SWSH) { Species = 025, Ability = A2 }, // Pikachu
            new(Nest105,1,2,2,SWSH) { Species = 871, Ability = A2 }, // Pincurchin
            new(Nest105,2,3,3,SWSH) { Species = 404, Ability = A2 }, // Luxio
            new(Nest105,2,3,3,SWSH) { Species = 026, Ability = A2 }, // Raichu
            new(Nest105,3,4,4,SWSH) { Species = 836, Ability = A2 }, // Boltund
            new(Nest105,3,4,4,SWSH) { Species = 702, Ability = A2 }, // Dedenne
            new(Nest105,3,4,4,SWSH) { Species = 310, Ability = A2 }, // Manectric
            new(Nest105,4,4,4,SWSH) { Species = 405, Ability = A2 }, // Luxray
            new(Nest105,4,4,4,SWSH) { Species = 462, Ability = A2 }, // Magnezone
            new(Nest106,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest106,0,1,1,SWSH) { Species = 661, Ability = A3 }, // Fletchling
            new(Nest106,0,1,1,SWSH) { Species = 527, Ability = A3 }, // Woobat
            new(Nest106,1,2,2,SWSH) { Species = 587, Ability = A3 }, // Emolga
            new(Nest106,2,3,3,SWSH) { Species = 662, Ability = A3 }, // Fletchinder
            new(Nest106,3,4,4,SWSH) { Species = 587, Ability = A4 }, // Emolga
            new(Nest106,3,4,4,SWSH) { Species = 528, Ability = A4 }, // Swoobat
            new(Nest106,4,4,4,SWSH) { Species = 663, Ability = A4 }, // Talonflame
            new(Nest107,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest107,0,1,1,SWSH) { Species = 163, Ability = A2 }, // Hoothoot
            new(Nest107,0,1,1,SWSH) { Species = 519, Ability = A2 }, // Pidove
            new(Nest107,1,2,2,SWSH) { Species = 520, Ability = A2 }, // Tranquill
            new(Nest107,2,3,3,SWSH) { Species = 528, Ability = A2 }, // Swoobat
            new(Nest107,2,3,3,SWSH) { Species = 164, Ability = A2 }, // Noctowl
            new(Nest107,3,4,4,SWSH) { Species = 521, Ability = A2 }, // Unfezant
            new(Nest107,3,4,4,SWSH) { Species = 663, Ability = A2 }, // Talonflame
            new(Nest107,3,4,4,SWSH) { Species = 587, Ability = A2 }, // Emolga
            new(Nest107,4,4,4,SWSH) { Species = 663, Ability = A2 }, // Talonflame
            new(Nest108,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest108,0,1,1,SWSH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest108,1,2,2,SWSH) { Species = 825, Ability = A3 }, // Dottler
            new(Nest108,2,3,3,SWSH) { Species = 558, Ability = A3 }, // Crustle
            new(Nest108,3,4,4,SWSH) { Species = 123, Ability = A4 }, // Scyther
            new(Nest108,4,4,4,SWSH) { Species = 826, Ability = A4 }, // Orbeetle
            new(Nest108,4,4,4,SWSH) { Species = 212, Ability = A4 }, // Scizor
            new(Nest109,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest109,0,1,1,SWSH) { Species = 123, Ability = A2 }, // Scyther
            new(Nest109,1,2,2,SWSH) { Species = 213, Ability = A2 }, // Shuckle
            new(Nest109,1,2,2,SWSH) { Species = 544, Ability = A2 }, // Whirlipede
            new(Nest109,2,3,3,SWSH) { Species = 123, Ability = A2 }, // Scyther
            new(Nest109,2,3,3,SWSH) { Species = 558, Ability = A2 }, // Crustle
            new(Nest109,3,4,4,SWSH) { Species = 545, Ability = A2 }, // Scolipede
            new(Nest109,3,4,4,SWSH) { Species = 617, Ability = A2 }, // Accelgor
            new(Nest109,3,4,4,SWSH) { Species = 589, Ability = A2 }, // Escavalier
            new(Nest109,4,4,4,SWSH) { Species = 212, Ability = A2 }, // Scizor
            new(Nest110,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest110,0,1,1,SWSH) { Species = 590, Ability = A3 }, // Foongus
            new(Nest110,0,1,1,SWSH) { Species = 753, Ability = A3 }, // Fomantis
            new(Nest110,1,2,2,SWSH) { Species = 548, Ability = A3 }, // Petilil
            new(Nest110,1,2,2,SWSH) { Species = 754, Ability = A3 }, // Lurantis
            new(Nest110,2,3,3,SWSH) { Species = 591, Ability = A3 }, // Amoonguss
            new(Nest110,2,3,3,SWSH) { Species = 114, Ability = A3 }, // Tangela
            new(Nest110,3,4,4,SWSH) { Species = 549, Ability = A4 }, // Lilligant
            new(Nest110,3,4,4,SWSH) { Species = 754, Ability = A4 }, // Lurantis
            new(Nest110,4,4,4,SWSH) { Species = 591, Ability = A4 }, // Amoonguss
            new(Nest110,4,4,4,SWSH) { Species = 465, Ability = A4 }, // Tangrowth
            new(Nest111,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest111,0,1,1,SWSH) { Species = 114, Ability = A2 }, // Tangela
            new(Nest111,0,1,1,SWSH) { Species = 753, Ability = A2 }, // Fomantis
            new(Nest111,1,2,2,SWSH) { Species = 590, Ability = A2 }, // Foongus
            new(Nest111,1,2,2,SWSH) { Species = 754, Ability = A2 }, // Lurantis
            new(Nest111,2,3,3,SWSH) { Species = 556, Ability = A2 }, // Maractus
            new(Nest111,2,3,3,SWSH) { Species = 549, Ability = A2 }, // Lilligant
            new(Nest111,3,4,4,SWSH) { Species = 754, Ability = A2 }, // Lurantis
            new(Nest111,3,4,4,SWSH) { Species = 591, Ability = A2 }, // Amoonguss
            new(Nest111,3,4,4,SWSH) { Species = 465, Ability = A2 }, // Tangrowth
            new(Nest111,4,4,4,SWSH) { Species = 549, Ability = A2 }, // Lilligant
            new(Nest111,4,4,4,SWSH) { Species = 460, Ability = A2 }, // Abomasnow
            new(Nest112,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest112,0,1,1,SWSH) { Species = 661, Ability = A3 }, // Fletchling
            new(Nest112,0,1,1,SWSH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest112,1,2,2,SWSH) { Species = 636, Ability = A3 }, // Larvesta
            new(Nest112,1,2,2,SWSH) { Species = 757, Ability = A3, Gender = 1 }, // Salandit
            new(Nest112,2,3,3,SWSH) { Species = 662, Ability = A3 }, // Fletchinder
            new(Nest112,2,3,3,SWSH) { Species = 636, Ability = A3 }, // Larvesta
            new(Nest112,3,4,4,SWSH) { Species = 324, Ability = A4 }, // Torkoal
            new(Nest112,3,4,4,SWSH) { Species = 663, Ability = A4 }, // Talonflame
            new(Nest112,3,4,4,SWSH) { Species = 758, Ability = A4 }, // Salazzle
            new(Nest112,4,4,4,SWSH) { Species = 324, Ability = A4 }, // Torkoal
            new(Nest112,4,4,4,SWSH) { Species = 637, Ability = A4 }, // Volcarona
            new(Nest113,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest113,0,1,1,SWSH) { Species = 636, Ability = A2 }, // Larvesta
            new(Nest113,0,1,1,SWSH) { Species = 607, Ability = A2 }, // Litwick
            new(Nest113,1,2,2,SWSH) { Species = 636, Ability = A2 }, // Larvesta
            new(Nest113,1,2,2,SWSH) { Species = 757, Ability = A2, Gender = 1 }, // Salandit
            new(Nest113,2,3,3,SWSH) { Species = 324, Ability = A2 }, // Torkoal
            new(Nest113,2,3,3,SWSH) { Species = 758, Ability = A2 }, // Salazzle
            new(Nest113,3,4,4,SWSH) { Species = 663, Ability = A2 }, // Talonflame
            new(Nest113,3,4,4,SWSH) { Species = 609, Ability = A2 }, // Chandelure
            new(Nest113,3,4,4,SWSH) { Species = 637, Ability = A2 }, // Volcarona
            new(Nest113,4,4,4,SWSH) { Species = 006, Ability = A2 }, // Charizard
            new(Nest114,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest114,0,1,1,SWSH) { Species = 524, Ability = A3 }, // Roggenrola
            new(Nest114,0,1,1,SWSH) { Species = 111, Ability = A3 }, // Rhyhorn
            new(Nest114,1,2,2,SWSH) { Species = 744, Ability = A3 }, // Rockruff
            new(Nest114,1,2,2,SWSH) { Species = 525, Ability = A3 }, // Boldore
            new(Nest114,2,3,3,SWSH) { Species = 112, Ability = A3 }, // Rhydon
            new(Nest114,2,3,3,SWSH) { Species = 558, Ability = A3 }, // Crustle
            new(Nest114,3,4,4,SWSH) { Species = 112, Ability = A4 }, // Rhydon
            new(Nest114,3,4,4,SWSH) { Species = 526, Ability = A4 }, // Gigalith
            new(Nest114,3,4,4,SWSH) { Species = 558, Ability = A4 }, // Crustle
            new(Nest114,4,4,4,SWSH) { Species = 464, Ability = A4 }, // Rhyperior
            new(Nest115,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest115,0,1,1,SWSH) { Species = 744, Ability = A2 }, // Rockruff
            new(Nest115,0,1,1,SWSH) { Species = 438, Ability = A2 }, // Bonsly
            new(Nest115,1,2,2,SWSH) { Species = 111, Ability = A2 }, // Rhyhorn
            new(Nest115,1,2,2,SWSH) { Species = 744, Ability = A2 }, // Rockruff
            new(Nest115,2,3,3,SWSH) { Species = 112, Ability = A2 }, // Rhydon
            new(Nest115,2,3,3,SWSH) { Species = 213, Ability = A2 }, // Shuckle
            new(Nest115,3,4,4,SWSH) { Species = 185, Ability = A2 }, // Sudowoodo
            new(Nest115,3,4,4,SWSH) { Species = 526, Ability = A2 }, // Gigalith
            new(Nest115,4,4,4,SWSH) { Species = 558, Ability = A2 }, // Crustle
            new(Nest115,4,4,4,SWSH) { Species = 464, Ability = A2 }, // Rhyperior
            new(Nest116,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest116,0,1,1,SWSH) { Species = 102, Ability = A3 }, // Exeggcute
            new(Nest116,0,1,1,SWSH) { Species = 063, Ability = A3 }, // Abra
            new(Nest116,1,2,2,SWSH) { Species = 280, Ability = A3 }, // Ralts
            new(Nest116,1,2,2,SWSH) { Species = 064, Ability = A3 }, // Kadabra
            new(Nest116,2,3,3,SWSH) { Species = 281, Ability = A3 }, // Kirlia
            new(Nest116,3,4,4,SWSH) { Species = 103, Ability = A4 }, // Exeggutor
            new(Nest116,3,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest116,4,4,4,SWSH) { Species = 065, Ability = A4 }, // Alakazam
            new(Nest116,4,4,4,SWSH) { Species = 121, Ability = A4 }, // Starmie
            new(Nest117,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest117,0,1,1,SWSH) { Species = 605, Ability = A2 }, // Elgyem
            new(Nest117,0,1,1,SWSH) { Species = 063, Ability = A2 }, // Abra
            new(Nest117,1,2,2,SWSH) { Species = 079, Ability = A2, Form = 1 }, // Slowpoke-1
            new(Nest117,1,2,2,SWSH) { Species = 605, Ability = A2 }, // Elgyem
            new(Nest117,2,3,3,SWSH) { Species = 079, Ability = A2, Form = 1 }, // Slowpoke-1
            new(Nest117,3,4,4,SWSH) { Species = 518, Ability = A2 }, // Musharna
            new(Nest117,3,4,4,SWSH) { Species = 606, Ability = A2 }, // Beheeyem
            new(Nest117,4,4,4,SWSH) { Species = 065, Ability = A2 }, // Alakazam
            new(Nest118,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest118,0,1,1,SWSH) { Species = 543, Ability = A3 }, // Venipede
            new(Nest118,0,1,1,SWSH) { Species = 451, Ability = A3 }, // Skorupi
            new(Nest118,1,2,2,SWSH) { Species = 072, Ability = A3 }, // Tentacool
            new(Nest118,2,3,3,SWSH) { Species = 544, Ability = A3 }, // Whirlipede
            new(Nest118,3,4,4,SWSH) { Species = 452, Ability = A4 }, // Drapion
            new(Nest118,3,4,4,SWSH) { Species = 073, Ability = A4 }, // Tentacruel
            new(Nest118,4,4,4,SWSH) { Species = 073, Ability = A4 }, // Tentacruel
            new(Nest118,4,4,4,SWSH) { Species = 545, Ability = A4 }, // Scolipede
            new(Nest119,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest119,0,1,1,SWSH) { Species = 747, Ability = A2 }, // Mareanie
            new(Nest119,0,1,1,SWSH) { Species = 211, Ability = A2 }, // Qwilfish
            new(Nest119,1,2,2,SWSH) { Species = 544, Ability = A2 }, // Whirlipede
            new(Nest119,2,3,3,SWSH) { Species = 211, Ability = A2 }, // Qwilfish
            new(Nest119,2,3,3,SWSH) { Species = 591, Ability = A2 }, // Amoonguss
            new(Nest119,3,4,4,SWSH) { Species = 748, Ability = A2 }, // Toxapex
            new(Nest119,3,4,4,SWSH) { Species = 545, Ability = A2 }, // Scolipede
            new(Nest119,3,4,4,SWSH) { Species = 452, Ability = A2 }, // Drapion
            new(Nest119,4,4,4,SWSH) { Species = 110, Ability = A2, Form = 1 }, // Weezing-1
            new(Nest119,4,4,4,SWSH) { Species = 545, Ability = A2 }, // Scolipede
            new(Nest120,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest120,0,1,1,SWSH) { Species = 318, Ability = A3 }, // Carvanha
            new(Nest120,0,1,1,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest120,1,2,2,SWSH) { Species = 318, Ability = A3 }, // Carvanha
            new(Nest120,1,2,2,SWSH) { Species = 570, Ability = A3 }, // Zorua
            new(Nest120,2,3,3,SWSH) { Species = 319, Ability = A3 }, // Sharpedo
            new(Nest120,2,3,3,SWSH) { Species = 687, Ability = A3 }, // Malamar
            new(Nest120,3,4,4,SWSH) { Species = 452, Ability = A4 }, // Drapion
            new(Nest120,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest120,3,4,4,SWSH) { Species = 687, Ability = A4 }, // Malamar
            new(Nest120,4,4,4,SWSH) { Species = 319, Ability = A4 }, // Sharpedo
            new(Nest120,4,4,4,SWSH) { Species = 571, Ability = A4 }, // Zoroark
            new(Nest121,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest121,0,1,1,SWSH) { Species = 570, Ability = A2 }, // Zorua
            new(Nest121,0,1,1,SWSH) { Species = 318, Ability = A2 }, // Carvanha
            new(Nest121,1,2,2,SWSH) { Species = 570, Ability = A2 }, // Zorua
            new(Nest121,1,2,2,SWSH) { Species = 686, Ability = A2 }, // Inkay
            new(Nest121,2,3,3,SWSH) { Species = 552, Ability = A2 }, // Krokorok
            new(Nest121,2,3,3,SWSH) { Species = 687, Ability = A2 }, // Malamar
            new(Nest121,3,4,4,SWSH) { Species = 828, Ability = A2 }, // Thievul
            new(Nest121,3,4,4,SWSH) { Species = 571, Ability = A2 }, // Zoroark
            new(Nest121,3,4,4,SWSH) { Species = 319, Ability = A2 }, // Sharpedo
            new(Nest121,4,4,4,SWSH) { Species = 510, Ability = A2 }, // Liepard
            new(Nest121,4,4,4,SWSH) { Species = 553, Ability = A2 }, // Krookodile
            new(Nest122,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest122,0,1,1,SWSH) { Species = 619, Ability = A3 }, // Mienfoo
            new(Nest122,0,1,1,SWSH) { Species = 852, Ability = A3 }, // Clobbopus
            new(Nest122,1,2,2,SWSH) { Species = 619, Ability = A3 }, // Mienfoo
            new(Nest122,3,4,4,SWSH) { Species = 620, Ability = A4 }, // Mienshao
            new(Nest122,4,4,4,SWSH) { Species = 853, Ability = A4 }, // Grapploct
            new(Nest122,4,4,4,SWSH) { Species = 620, Ability = A4 }, // Mienshao
            new(Nest123,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest123,0,1,1,SWSH) { Species = 619, Ability = A2 }, // Mienfoo
            new(Nest123,1,2,2,SWSH) { Species = 620, Ability = A2 }, // Mienshao
            new(Nest123,2,3,3,SWSH) { Species = 870, Ability = A2 }, // Falinks
            new(Nest123,3,4,4,SWSH) { Species = 620, Ability = A2 }, // Mienshao
            new(Nest123,4,4,4,SWSH) { Species = 853, Ability = A2 }, // Grapploct
            new(Nest124,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest124,0,1,1,SWSH) { Species = 174, Ability = A3 }, // Igglybuff
            new(Nest124,0,1,1,SWSH) { Species = 298, Ability = A3 }, // Azurill
            new(Nest124,1,2,2,SWSH) { Species = 764, Ability = A3 }, // Comfey
            new(Nest124,1,2,2,SWSH) { Species = 039, Ability = A3 }, // Jigglypuff
            new(Nest124,2,3,3,SWSH) { Species = 183, Ability = A3 }, // Marill
            new(Nest124,2,3,3,SWSH) { Species = 764, Ability = A3 }, // Comfey
            new(Nest124,3,4,4,SWSH) { Species = 707, Ability = A4 }, // Klefki
            new(Nest124,3,4,4,SWSH) { Species = 184, Ability = A4 }, // Azumarill
            new(Nest124,3,4,4,SWSH) { Species = 040, Ability = A4 }, // Wigglytuff
            new(Nest124,4,4,4,SWSH) { Species = 282, Ability = A4 }, // Gardevoir
            new(Nest124,4,4,4,SWSH) { Species = 764, Ability = A4 }, // Comfey
            new(Nest125,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest125,0,1,1,SWSH) { Species = 173, Ability = A2 }, // Cleffa
            new(Nest125,0,1,1,SWSH) { Species = 755, Ability = A2 }, // Morelull
            new(Nest125,1,2,2,SWSH) { Species = 183, Ability = A2 }, // Marill
            new(Nest125,1,2,2,SWSH) { Species = 035, Ability = A2 }, // Clefairy
            new(Nest125,2,3,3,SWSH) { Species = 281, Ability = A2 }, // Kirlia
            new(Nest125,2,3,3,SWSH) { Species = 707, Ability = A2 }, // Klefki
            new(Nest125,3,4,4,SWSH) { Species = 764, Ability = A2 }, // Comfey
            new(Nest125,3,4,4,SWSH) { Species = 036, Ability = A2 }, // Clefable
            new(Nest125,3,4,4,SWSH) { Species = 282, Ability = A2 }, // Gardevoir
            new(Nest125,4,4,4,SWSH) { Species = 756, Ability = A2 }, // Shiinotic
            new(Nest125,4,4,4,SWSH) { Species = 184, Ability = A2 }, // Azumarill
            new(Nest126,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest126,0,1,1,SWSH) { Species = 769, Ability = A3 }, // Sandygast
            new(Nest126,0,1,1,SWSH) { Species = 592, Ability = A3 }, // Frillish
            new(Nest126,1,2,2,SWSH) { Species = 104, Ability = A3 }, // Cubone
            new(Nest126,1,2,2,SWSH) { Species = 425, Ability = A3 }, // Drifloon
            new(Nest126,2,3,3,SWSH) { Species = 593, Ability = A3 }, // Jellicent
            new(Nest126,2,3,3,SWSH) { Species = 426, Ability = A3 }, // Drifblim
            new(Nest126,3,4,4,SWSH) { Species = 770, Ability = A4 }, // Palossand
            new(Nest126,3,4,4,SWSH) { Species = 593, Ability = A4 }, // Jellicent
            new(Nest126,3,4,4,SWSH) { Species = 426, Ability = A4 }, // Drifblim
            new(Nest126,4,4,4,SWSH) { Species = 105, Ability = A4 }, // Marowak
            new(Nest126,4,4,4,SWSH) { Species = 770, Ability = A4 }, // Palossand
            new(Nest127,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest127,0,1,1,SWSH) { Species = 769, Ability = A2 }, // Sandygast
            new(Nest127,0,1,1,SWSH) { Species = 592, Ability = A2 }, // Frillish
            new(Nest127,1,2,2,SWSH) { Species = 769, Ability = A2 }, // Sandygast
            new(Nest127,1,2,2,SWSH) { Species = 425, Ability = A2 }, // Drifloon
            new(Nest127,2,3,3,SWSH) { Species = 593, Ability = A2 }, // Jellicent
            new(Nest127,2,3,3,SWSH) { Species = 426, Ability = A2 }, // Drifblim
            new(Nest127,3,4,4,SWSH) { Species = 711, Ability = A2 }, // Gourgeist
            new(Nest127,3,4,4,SWSH) { Species = 711, Ability = A2, Form = 1 }, // Gourgeist-1
            new(Nest127,3,4,4,SWSH) { Species = 711, Ability = A2, Form = 2 }, // Gourgeist-2
            new(Nest127,4,4,4,SWSH) { Species = 711, Ability = A2, Form = 3 }, // Gourgeist-3
            new(Nest128,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest128,0,1,1,SWSH) { Species = 707, Ability = A3 }, // Klefki
            new(Nest128,0,1,1,SWSH) { Species = 081, Ability = A3 }, // Magnemite
            new(Nest128,1,2,2,SWSH) { Species = 624, Ability = A3 }, // Pawniard
            new(Nest128,1,2,2,SWSH) { Species = 081, Ability = A3 }, // Magnemite
            new(Nest128,2,3,3,SWSH) { Species = 227, Ability = A3 }, // Skarmory
            new(Nest128,2,3,3,SWSH) { Species = 082, Ability = A3 }, // Magneton
            new(Nest128,3,4,4,SWSH) { Species = 082, Ability = A4 }, // Magneton
            new(Nest128,3,4,4,SWSH) { Species = 707, Ability = A4 }, // Klefki
            new(Nest128,3,4,4,SWSH) { Species = 625, Ability = A4 }, // Bisharp
            new(Nest128,4,4,4,SWSH) { Species = 462, Ability = A4 }, // Magnezone
            new(Nest128,4,4,4,SWSH) { Species = 227, Ability = A4 }, // Skarmory
            new(Nest129,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest129,0,1,1,SWSH) { Species = 081, Ability = A2 }, // Magnemite
            new(Nest129,0,1,1,SWSH) { Species = 227, Ability = A2 }, // Skarmory
            new(Nest129,1,2,2,SWSH) { Species = 436, Ability = A2 }, // Bronzor
            new(Nest129,1,2,2,SWSH) { Species = 052, Ability = A2, Form = 2 }, // Meowth-2
            new(Nest129,2,3,3,SWSH) { Species = 082, Ability = A2 }, // Magneton
            new(Nest129,2,3,3,SWSH) { Species = 601, Ability = A2 }, // Klinklang
            new(Nest129,3,4,4,SWSH) { Species = 227, Ability = A2 }, // Skarmory
            new(Nest129,3,4,4,SWSH) { Species = 437, Ability = A2 }, // Bronzong
            new(Nest129,3,4,4,SWSH) { Species = 863, Ability = A2 }, // Perrserker
            new(Nest129,4,4,4,SWSH) { Species = 448, Ability = A2 }, // Lucario
            new(Nest129,4,4,4,SWSH) { Species = 625, Ability = A2 }, // Bisharp
            new(Nest130,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
          //new(Nest130,0,1,1,SWSH) { Species = 116, Ability = A3 }, // Horsea
            new(Nest130,2,2,2,SWSH) { Species = 840, Ability = A3 }, // Applin
            new(Nest130,1,2,2,SWSH) { Species = 117, Ability = A3 }, // Seadra
            new(Nest130,2,3,3,SWSH) { Species = 621, Ability = A3 }, // Druddigon
            new(Nest130,3,4,4,SWSH) { Species = 621, Ability = A4 }, // Druddigon
            new(Nest130,3,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
            new(Nest130,4,4,4,SWSH) { Species = 230, Ability = A4 }, // Kingdra
            new(Nest131,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest131,0,1,1,SWSH) { Species = 116, Ability = A2 }, // Horsea
          //new(Nest131,0,1,1,SWSH) { Species = 621, Ability = A2 }, // Druddigon
            new(Nest131,2,3,3,SWSH) { Species = 117, Ability = A2 }, // Seadra
            new(Nest131,3,4,4,SWSH) { Species = 621, Ability = A2 }, // Druddigon
            new(Nest131,3,4,4,SWSH) { Species = 715, Ability = A2 }, // Noivern
            new(Nest131,4,4,4,SWSH) { Species = 230, Ability = A2 }, // Kingdra
            new(Nest132,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest132,0,1,1,SWSH) { Species = 060, Ability = A3 }, // Poliwag
            new(Nest132,0,1,1,SWSH) { Species = 194, Ability = A3 }, // Wooper
            new(Nest132,1,2,2,SWSH) { Species = 118, Ability = A3 }, // Goldeen
            new(Nest132,1,2,2,SWSH) { Species = 061, Ability = A3 }, // Poliwhirl
            new(Nest132,2,3,3,SWSH) { Species = 342, Ability = A3 }, // Crawdaunt
            new(Nest132,2,3,3,SWSH) { Species = 061, Ability = A3 }, // Poliwhirl
            new(Nest132,3,4,4,SWSH) { Species = 119, Ability = A4 }, // Seaking
            new(Nest132,3,4,4,SWSH) { Species = 342, Ability = A4 }, // Crawdaunt
            new(Nest132,3,4,4,SWSH) { Species = 195, Ability = A4 }, // Quagsire
            new(Nest132,4,4,4,SWSH) { Species = 062, Ability = A4 }, // Poliwrath
            new(Nest132,4,4,4,SWSH) { Species = 186, Ability = A4 }, // Politoed
            new(Nest133,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest133,0,1,1,SWSH) { Species = 341, Ability = A2 }, // Corphish
            new(Nest133,0,1,1,SWSH) { Species = 751, Ability = A2 }, // Dewpider
            new(Nest133,1,2,2,SWSH) { Species = 118, Ability = A2 }, // Goldeen
            new(Nest133,1,2,2,SWSH) { Species = 061, Ability = A2 }, // Poliwhirl
            new(Nest133,2,3,3,SWSH) { Species = 342, Ability = A2 }, // Crawdaunt
            new(Nest133,2,3,3,SWSH) { Species = 195, Ability = A2 }, // Quagsire
            new(Nest133,3,4,4,SWSH) { Species = 119, Ability = A2 }, // Seaking
            new(Nest133,3,4,4,SWSH) { Species = 062, Ability = A2 }, // Poliwrath
            new(Nest133,3,4,4,SWSH) { Species = 342, Ability = A2 }, // Crawdaunt
            new(Nest133,4,4,4,SWSH) { Species = 752, Ability = A2 }, // Araquanid
            new(Nest133,4,4,4,SWSH) { Species = 186, Ability = A2 }, // Politoed
            new(Nest134,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest134,0,1,1,SWSH) { Species = 054, Ability = A3 }, // Psyduck
            new(Nest134,0,1,1,SWSH) { Species = 833, Ability = A3 }, // Chewtle
            new(Nest134,1,2,2,SWSH) { Species = 846, Ability = A3 }, // Arrokuda
            new(Nest134,1,2,2,SWSH) { Species = 339, Ability = A3 }, // Barboach
            new(Nest134,2,3,3,SWSH) { Species = 055, Ability = A3 }, // Golduck
            new(Nest134,2,3,3,SWSH) { Species = 845, Ability = A3 }, // Cramorant
            new(Nest134,3,4,4,SWSH) { Species = 055, Ability = A4 }, // Golduck
            new(Nest134,3,4,4,SWSH) { Species = 847, Ability = A4 }, // Barraskewda
            new(Nest134,3,4,4,SWSH) { Species = 834, Ability = A4 }, // Drednaw
            new(Nest134,4,4,4,SWSH) { Species = 340, Ability = A4 }, // Whiscash
            new(Nest134,4,4,4,SWSH) { Species = 055, Ability = A4 }, // Golduck
            new(Nest135,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest135,0,1,1,SWSH) { Species = 846, Ability = A2 }, // Arrokuda
            new(Nest135,0,1,1,SWSH) { Species = 535, Ability = A2 }, // Tympole
            new(Nest135,1,2,2,SWSH) { Species = 054, Ability = A2 }, // Psyduck
            new(Nest135,1,2,2,SWSH) { Species = 536, Ability = A2 }, // Palpitoad
            new(Nest135,2,3,3,SWSH) { Species = 055, Ability = A2 }, // Golduck
            new(Nest135,2,3,3,SWSH) { Species = 340, Ability = A2 }, // Whiscash
            new(Nest135,3,4,4,SWSH) { Species = 055, Ability = A2 }, // Golduck
            new(Nest135,3,4,4,SWSH) { Species = 847, Ability = A2 }, // Barraskewda
            new(Nest135,3,4,4,SWSH) { Species = 537, Ability = A2 }, // Seismitoad
            new(Nest135,4,4,4,SWSH) { Species = 130, Ability = A2 }, // Gyarados
            new(Nest136,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest136,0,1,1,SWSH) { Species = 072, Ability = A3 }, // Tentacool
            new(Nest136,0,1,1,SWSH) { Species = 098, Ability = A3 }, // Krabby
            new(Nest136,1,2,2,SWSH) { Species = 072, Ability = A3 }, // Tentacool
            new(Nest136,1,2,2,SWSH) { Species = 223, Ability = A3 }, // Remoraid
            new(Nest136,2,3,3,SWSH) { Species = 073, Ability = A3 }, // Tentacruel
            new(Nest136,2,3,3,SWSH) { Species = 746, Ability = A3 }, // Wishiwashi
            new(Nest136,3,4,4,SWSH) { Species = 224, Ability = A4 }, // Octillery
            new(Nest136,3,4,4,SWSH) { Species = 226, Ability = A4 }, // Mantine
            new(Nest136,3,4,4,SWSH) { Species = 099, Ability = A4 }, // Kingler
            new(Nest136,4,4,4,SWSH) { Species = 091, Ability = A4 }, // Cloyster
            new(Nest136,4,4,4,SWSH) { Species = 073, Ability = A4 }, // Tentacruel
            new(Nest137,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest137,0,1,1,SWSH) { Species = 090, Ability = A2 }, // Shellder
            new(Nest137,0,1,1,SWSH) { Species = 688, Ability = A2 }, // Binacle
            new(Nest137,1,2,2,SWSH) { Species = 747, Ability = A2 }, // Mareanie
            new(Nest137,1,2,2,SWSH) { Species = 223, Ability = A2 }, // Remoraid
            new(Nest137,2,3,3,SWSH) { Species = 073, Ability = A2 }, // Tentacruel
            new(Nest137,2,3,3,SWSH) { Species = 771, Ability = A2 }, // Pyukumuku
            new(Nest137,3,4,4,SWSH) { Species = 224, Ability = A2 }, // Octillery
            new(Nest137,3,4,4,SWSH) { Species = 226, Ability = A2 }, // Mantine
            new(Nest137,3,4,4,SWSH) { Species = 689, Ability = A2 }, // Barbaracle
            new(Nest137,4,4,4,SWSH) { Species = 091, Ability = A2 }, // Cloyster
            new(Nest137,4,4,4,SWSH) { Species = 748, Ability = A2 }, // Toxapex
            new(Nest138,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
          //new(Nest138,0,1,1,SWSH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest138,2,2,2,SWSH) { Species = 120, Ability = A3 }, // Staryu
            new(Nest138,2,3,3,SWSH) { Species = 320, Ability = A3 }, // Wailmer
            new(Nest138,2,3,3,SWSH) { Species = 746, Ability = A3 }, // Wishiwashi
            new(Nest138,3,4,4,SWSH) { Species = 321, Ability = A4 }, // Wailord
            new(Nest138,3,4,4,SWSH) { Species = 171, Ability = A4 }, // Lanturn
            new(Nest138,3,4,4,SWSH) { Species = 121, Ability = A4 }, // Starmie
            new(Nest138,4,4,4,SWSH) { Species = 319, Ability = A4 }, // Sharpedo
            new(Nest139,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest139,0,1,1,SWSH) { Species = 120, Ability = A2 }, // Staryu
            new(Nest139,2,2,2,SWSH) { Species = 320, Ability = A2 }, // Wailmer
            new(Nest139,2,2,2,SWSH) { Species = 279, Ability = A2 }, // Pelipper
            new(Nest139,2,3,3,SWSH) { Species = 171, Ability = A2 }, // Lanturn
            new(Nest139,2,3,3,SWSH) { Species = 117, Ability = A2 }, // Seadra
            new(Nest139,3,4,4,SWSH) { Species = 171, Ability = A2 }, // Lanturn
            new(Nest139,3,4,4,SWSH) { Species = 121, Ability = A2 }, // Starmie
            new(Nest139,4,4,4,SWSH) { Species = 319, Ability = A2 }, // Sharpedo
            new(Nest140,0,0,1,SWSH) { Species = 440, Ability = A3 }, // Happiny
            new(Nest140,0,1,1,SWSH) { Species = 440, Ability = A3 }, // Happiny
            new(Nest140,1,2,2,SWSH) { Species = 440, Ability = A3 }, // Happiny
            new(Nest140,2,3,3,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest140,3,4,4,SWSH) { Species = 113, Ability = A4 }, // Chansey
            new(Nest140,4,4,4,SWSH) { Species = 242, Ability = A4 }, // Blissey
            new(Nest141,0,0,1,SWSH) { Species = 113, Ability = A2 }, // Chansey
            new(Nest141,0,1,1,SWSH) { Species = 113, Ability = A2 }, // Chansey
            new(Nest141,1,2,2,SWSH) { Species = 113, Ability = A2 }, // Chansey
            new(Nest141,2,3,3,SWSH) { Species = 113, Ability = A2 }, // Chansey
            new(Nest141,3,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest141,4,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest142,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
          //new(Nest142,0,1,1,SWSH) { Species = 415, Ability = A3 }, // Combee
            new(Nest142,2,2,2,SWSH) { Species = 415, Ability = A3 }, // Combee
            new(Nest142,2,3,3,SWSH) { Species = 415, Ability = A3 }, // Combee
            new(Nest142,3,4,4,SWSH) { Species = 416, Ability = A4 }, // Vespiquen
            new(Nest142,4,4,4,SWSH) { Species = 416, Ability = A4 }, // Vespiquen
            new(Nest143,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest143,0,1,1,SWSH) { Species = 415, Ability = A2, Gender = 1 }, // Combee
            new(Nest143,2,2,2,SWSH) { Species = 415, Ability = A2, Gender = 1 }, // Combee
            new(Nest143,2,3,3,SWSH) { Species = 416, Ability = A2 }, // Vespiquen
            new(Nest143,3,4,4,SWSH) { Species = 416, Ability = A2 }, // Vespiquen
            new(Nest143,4,4,4,SWSH) { Species = 416, Ability = A2 }, // Vespiquen
            new(Nest144,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest144,0,1,1,SWSH) { Species = 590, Ability = A2 }, // Foongus
            new(Nest144,0,1,1,SWSH) { Species = 102, Ability = A2 }, // Exeggcute
            new(Nest144,1,2,2,SWSH) { Species = 114, Ability = A2 }, // Tangela
            new(Nest144,1,2,2,SWSH) { Species = 315, Ability = A2 }, // Roselia
            new(Nest144,2,3,3,SWSH) { Species = 114, Ability = A2 }, // Tangela
            new(Nest144,2,3,3,SWSH) { Species = 315, Ability = A2 }, // Roselia
            new(Nest144,3,4,4,SWSH) { Species = 103, Ability = A2 }, // Exeggutor
            new(Nest144,3,4,4,SWSH) { Species = 003, Ability = A2 }, // Venusaur
            new(Nest144,3,4,4,SWSH) { Species = 465, Ability = A2 }, // Tangrowth
            new(Nest144,4,4,4,SWSH) { Species = 407, Ability = A2 }, // Roserade
            new(Nest144,4,4,4,SWSH) { Species = 003, Ability = A2, CanGigantamax = true }, // Venusaur
            new(Nest145,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest145,0,1,1,SWSH) { Species = 129, Ability = A2 }, // Magikarp
          //new(Nest145,0,1,1,SWSH) { Species = 072, Ability = A2 }, // Tentacool
            new(Nest145,2,2,2,SWSH) { Species = 120, Ability = A2 }, // Staryu
            new(Nest145,2,2,2,SWSH) { Species = 688, Ability = A2 }, // Binacle
            new(Nest145,2,3,3,SWSH) { Species = 073, Ability = A2 }, // Tentacruel
            new(Nest145,2,3,3,SWSH) { Species = 130, Ability = A2 }, // Gyarados
            new(Nest145,3,4,4,SWSH) { Species = 073, Ability = A2 }, // Tentacruel
            new(Nest145,3,4,4,SWSH) { Species = 130, Ability = A2 }, // Gyarados
            new(Nest145,3,4,4,SWSH) { Species = 121, Ability = A2 }, // Starmie
            new(Nest145,4,4,4,SWSH) { Species = 689, Ability = A2 }, // Barbaracle
            new(Nest145,4,4,4,SWSH) { Species = 009, Ability = A2, CanGigantamax = true }, // Blastoise
            new(Nest146,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest146,0,1,1,SWSH) { Species = 098, Ability = A2 }, // Krabby
          //new(Nest146,0,1,1,SWSH) { Species = 688, Ability = A2 }, // Binacle
            new(Nest146,2,2,2,SWSH) { Species = 072, Ability = A2 }, // Tentacool
            new(Nest146,2,2,2,SWSH) { Species = 223, Ability = A2 }, // Remoraid
            new(Nest146,2,3,3,SWSH) { Species = 073, Ability = A2 }, // Tentacruel
            new(Nest146,2,3,3,SWSH) { Species = 224, Ability = A2 }, // Octillery
            new(Nest146,3,4,4,SWSH) { Species = 713, Ability = A2 }, // Avalugg
            new(Nest146,3,4,4,SWSH) { Species = 614, Ability = A2 }, // Beartic
            new(Nest146,3,4,4,SWSH) { Species = 099, Ability = A2 }, // Kingler
            new(Nest146,4,4,4,SWSH) { Species = 091, Ability = A2 }, // Cloyster
            new(Nest146,4,4,4,SWSH) { Species = 099, Ability = A2, CanGigantamax = true }, // Kingler
            new(Nest147,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest147,0,1,1,SWSH) { Species = 833, Ability = A2 }, // Chewtle
            new(Nest147,0,1,1,SWSH) { Species = 054, Ability = A2 }, // Psyduck
            new(Nest147,1,2,2,SWSH) { Species = 339, Ability = A2 }, // Barboach
            new(Nest147,2,3,3,SWSH) { Species = 055, Ability = A2 }, // Golduck
            new(Nest147,2,3,3,SWSH) { Species = 845, Ability = A2 }, // Cramorant
            new(Nest147,3,4,4,SWSH) { Species = 055, Ability = A2 }, // Golduck
            new(Nest147,3,4,4,SWSH) { Species = 847, Ability = A2 }, // Barraskewda
            new(Nest147,4,4,4,SWSH) { Species = 340, Ability = A2 }, // Whiscash
            new(Nest147,4,4,4,SWSH) { Species = 834, Ability = A2, CanGigantamax = true }, // Drednaw
            new(Nest148,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest148,0,1,1,SWSH) { Species = 824, Ability = A2 }, // Blipbug
            new(Nest148,0,1,1,SWSH) { Species = 742, Ability = A2 }, // Cutiefly
            new(Nest148,1,2,2,SWSH) { Species = 595, Ability = A2 }, // Joltik
            new(Nest148,2,3,3,SWSH) { Species = 825, Ability = A2 }, // Dottler
            new(Nest148,2,3,3,SWSH) { Species = 291, Ability = A2 }, // Ninjask
            new(Nest148,3,4,4,SWSH) { Species = 826, Ability = A2 }, // Orbeetle
            new(Nest148,3,4,4,SWSH) { Species = 596, Ability = A2 }, // Galvantula
            new(Nest148,3,4,4,SWSH) { Species = 743, Ability = A2 }, // Ribombee
            new(Nest148,4,4,4,SWSH) { Species = 291, Ability = A2 }, // Ninjask
            new(Nest148,4,4,4,SWSH) { Species = 826, Ability = A2, CanGigantamax = true }, // Orbeetle
            new(Nest149,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest149,0,1,1,SWSH) { Species = 843, Ability = A2 }, // Silicobra
            new(Nest149,0,1,1,SWSH) { Species = 529, Ability = A2 }, // Drilbur
            new(Nest149,1,2,2,SWSH) { Species = 843, Ability = A2 }, // Silicobra
            new(Nest149,1,2,2,SWSH) { Species = 529, Ability = A2 }, // Drilbur
            new(Nest149,2,3,3,SWSH) { Species = 028, Ability = A2 }, // Sandslash
            new(Nest149,2,3,3,SWSH) { Species = 552, Ability = A2 }, // Krokorok
            new(Nest149,3,4,4,SWSH) { Species = 844, Ability = A2 }, // Sandaconda
            new(Nest149,3,4,4,SWSH) { Species = 553, Ability = A2 }, // Krookodile
            new(Nest149,3,4,4,SWSH) { Species = 530, Ability = A2 }, // Excadrill
            new(Nest149,4,4,4,SWSH) { Species = 553, Ability = A2 }, // Krookodile
            new(Nest149,4,4,4,SWSH) { Species = 844, Ability = A2, CanGigantamax = true }, // Sandaconda
            new(Nest150,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest150,0,1,1,SWSH) { Species = 840, Ability = A2 }, // Applin
          //new(Nest150,0,1,1,SWSH) { Species = 420, Ability = A2 }, // Cherubi (DLC1)
          //new(Nest150,0,1,1,SWSH) { Species = 761, Ability = A2 }, // Bounsweet (DLC2)
            new(Nest150,2,2,2,SWSH) { Species = 420, Ability = A2 }, // Cherubi
            new(Nest150,2,2,2,SWSH) { Species = 840, Ability = A2 }, // Applin
            new(Nest150,2,3,3,SWSH) { Species = 762, Ability = A2 }, // Steenee
            new(Nest150,3,4,4,SWSH) { Species = 820, Ability = A2 }, // Greedent
            new(Nest150,4,4,4,SWSH) { Species = 763, Ability = A2 }, // Tsareena
            new(Nest151,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
          //new(Nest151,0,0,1,SWSH) { Species = 132, Ability = A3 }, // Ditto
          //new(Nest151,0,1,2,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest151,2,2,3,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest151,2,3,3,SWSH) { Species = 132, Ability = A3 }, // Ditto
            new(Nest151,2,3,3,SWSH) { Species = 132, Ability = A4 }, // Ditto
            new(Nest151,3,4,4,SWSH) { Species = 132, Ability = A4 }, // Ditto
            new(Nest151,4,4,4,SWSH) { Species = 132, Ability = A4 }, // Ditto
            new(Nest152,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
          //new(Nest152,0,0,1,SWSH) { Species = 132, Ability = A2 }, // Ditto
          //new(Nest152,0,1,2,SWSH) { Species = 132, Ability = A2 }, // Ditto
            new(Nest152,2,2,3,SWSH) { Species = 132, Ability = A2 }, // Ditto
            new(Nest152,2,3,3,SWSH) { Species = 132, Ability = A2 }, // Ditto
            new(Nest152,3,4,4,SWSH) { Species = 132, Ability = A2 }, // Ditto
            new(Nest152,4,4,4,SWSH) { Species = 132, Ability = A2 }, // Ditto
            new(Nest153,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest153,0,1,1,SWSH) { Species = 590, Ability = A3 }, // Foongus
            new(Nest153,0,1,1,SWSH) { Species = 102, Ability = A3 }, // Exeggcute
            new(Nest153,1,2,2,SWSH) { Species = 753, Ability = A3 }, // Fomantis
            new(Nest153,1,2,2,SWSH) { Species = 114, Ability = A3 }, // Tangela
            new(Nest153,2,3,3,SWSH) { Species = 754, Ability = A3 }, // Lurantis
            new(Nest153,2,3,3,SWSH) { Species = 102, Ability = A3 }, // Exeggcute
            new(Nest153,3,4,4,SWSH) { Species = 103, Ability = A4 }, // Exeggutor
            new(Nest153,3,4,4,SWSH) { Species = 591, Ability = A4 }, // Amoonguss
            new(Nest153,3,4,4,SWSH) { Species = 754, Ability = A4 }, // Lurantis
            new(Nest153,4,4,4,SWSH) { Species = 465, Ability = A4 }, // Tangrowth
            new(Nest153,4,4,4,SWSH) { Species = 003, Ability = A4 }, // Venusaur
            new(Nest154,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
          //new(Nest154,0,1,1,SWSH) { Species = 129, Ability = A3 }, // Magikarp
          //new(Nest154,0,1,1,SWSH) { Species = 072, Ability = A3 }, // Tentacool
            new(Nest154,2,2,2,SWSH) { Species = 120, Ability = A3 }, // Staryu
            new(Nest154,1,2,2,SWSH) { Species = 090, Ability = A3 }, // Shellder
            new(Nest154,2,3,3,SWSH) { Species = 073, Ability = A3 }, // Tentacruel
            new(Nest154,2,3,3,SWSH) { Species = 130, Ability = A3 }, // Gyarados
            new(Nest154,3,4,4,SWSH) { Species = 073, Ability = A4 }, // Tentacruel
            new(Nest154,3,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
            new(Nest154,3,4,4,SWSH) { Species = 121, Ability = A4 }, // Starmie
            new(Nest154,4,4,4,SWSH) { Species = 091, Ability = A4 }, // Cloyster
            new(Nest154,4,4,4,SWSH) { Species = 009, Ability = A4 }, // Blastoise
            new(Nest155,2,4,4,SWSH) { Species = 113, Ability = A3 }, // Chansey
            new(Nest155,0,1,1,SWSH) { Species = 744, Ability = A3 }, // Rockruff
            new(Nest155,1,2,2,SWSH) { Species = 744, Ability = A3 }, // Rockruff
            new(Nest155,2,3,3,SWSH) { Species = 744, Ability = A3 }, // Rockruff
            new(Nest155,2,3,3,SWSH) { Species = 744, Ability = A3, Form = 1 }, // Rockruff-1
            new(Nest155,3,4,4,SWSH) { Species = 745, Ability = A4 }, // Lycanroc
            new(Nest155,3,4,4,SWSH) { Species = 745, Ability = A4, Form = 1 }, // Lycanroc-1
            new(Nest155,4,4,4,SWSH) { Species = 745, Ability = A4, Form = 2 }, // Lycanroc-2
            new(Nest156,2,4,4,SWSH) { Species = 242, Ability = A2 }, // Blissey
            new(Nest156,0,1,1,SWSH) { Species = 744, Ability = A2 }, // Rockruff
            new(Nest156,1,2,2,SWSH) { Species = 744, Ability = A2 }, // Rockruff
            new(Nest156,2,3,3,SWSH) { Species = 744, Ability = A2 }, // Rockruff
            new(Nest156,2,3,3,SWSH) { Species = 744, Ability = A2, Form = 1 }, // Rockruff-1
            new(Nest156,3,3,4,SWSH) { Species = 745, Ability = A2 }, // Lycanroc
            new(Nest156,3,3,4,SWSH) { Species = 745, Ability = A2, Form = 1 }, // Lycanroc-1
            new(Nest156,4,4,4,SWSH) { Species = 745, Ability = A2 }, // Lycanroc
            new(Nest156,4,4,4,SWSH) { Species = 745, Ability = A2, Form = 1 }, // Lycanroc-1
            new(Nest156,3,4,4,SWSH) { Species = 745, Ability = A2, Form = 2 }, // Lycanroc-2
            new(Nest157,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest157,0,1,1,SWSH) { Species = 333, Ability = A3 }, // Swablu
            new(Nest157,0,1,1,SWSH) { Species = 831, Ability = A3 }, // Wooloo
            new(Nest157,1,2,2,SWSH) { Species = 333, Ability = A3 }, // Swablu
            new(Nest157,1,2,2,SWSH) { Species = 446, Ability = A3 }, // Munchlax
            new(Nest157,2,3,3,SWSH) { Species = 820, Ability = A3 }, // Greedent
            new(Nest157,2,3,3,SWSH) { Species = 832, Ability = A3 }, // Dubwool
            new(Nest157,3,4,4,SWSH) { Species = 334, Ability = A4 }, // Altaria
            new(Nest157,3,4,4,SWSH) { Species = 832, Ability = A4 }, // Dubwool
            new(Nest157,4,4,4,SWSH) { Species = 143, Ability = A4 }, // Snorlax
            new(Nest158,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest158,0,1,2,SWSH) { Species = 333, Ability = A2 }, // Swablu
            new(Nest158,0,1,2,SWSH) { Species = 819, Ability = A2 }, // Skwovet
            new(Nest158,1,2,3,SWSH) { Species = 333, Ability = A2 }, // Swablu
            new(Nest158,1,2,3,SWSH) { Species = 820, Ability = A2 }, // Greedent
            new(Nest158,2,3,4,SWSH) { Species = 820, Ability = A2 }, // Greedent
            new(Nest158,2,3,4,SWSH) { Species = 832, Ability = A2 }, // Dubwool
            new(Nest158,3,4,5,SWSH) { Species = 334, Ability = A2 }, // Altaria
            new(Nest158,3,4,5,SWSH) { Species = 832, Ability = A2 }, // Dubwool
            new(Nest158,4,4,5,SWSH) { Species = 143, Ability = A2 }, // Snorlax
            new(Nest158,4,4,5,SWSH) { Species = 143, Ability = A2, CanGigantamax = true }, // Snorlax
            new(Nest159,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest159,0,1,1,SWSH) { Species = 240, Ability = A3 }, // Magby
            new(Nest159,0,1,1,SWSH) { Species = 850, Ability = A3 }, // Sizzlipede
            new(Nest159,1,2,2,SWSH) { Species = 240, Ability = A3 }, // Magby
            new(Nest159,1,2,2,SWSH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest159,2,3,3,SWSH) { Species = 608, Ability = A3 }, // Lampent
            new(Nest159,2,3,3,SWSH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest159,3,4,4,SWSH) { Species = 126, Ability = A4 }, // Magmar
            new(Nest159,3,4,4,SWSH) { Species = 851, Ability = A4 }, // Centiskorch
            new(Nest159,3,4,4,SWSH) { Species = 609, Ability = A4 }, // Chandelure
            new(Nest159,4,4,4,SWSH) { Species = 126, Ability = A4 }, // Magmar
            new(Nest159,4,4,4,SWSH) { Species = 467, Ability = A4 }, // Magmortar
            new(Nest160,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest160,0,1,2,SWSH) { Species = 240, Ability = A2 }, // Magby
            new(Nest160,1,2,3,SWSH) { Species = 126, Ability = A2 }, // Magmar
            new(Nest160,1,2,3,SWSH) { Species = 631, Ability = A2 }, // Heatmor
            new(Nest160,2,3,4,SWSH) { Species = 126, Ability = A2 }, // Magmar
            new(Nest160,2,3,4,SWSH) { Species = 851, Ability = A2 }, // Centiskorch
            new(Nest160,3,4,5,SWSH) { Species = 609, Ability = A2 }, // Chandelure
            new(Nest160,3,4,5,SWSH) { Species = 467, Ability = A2 }, // Magmortar
            new(Nest160,4,4,5,SWSH) { Species = 467, Ability = A2 }, // Magmortar
            new(Nest160,4,4,5,SWSH) { Species = 851, Ability = A2, CanGigantamax = true }, // Centiskorch
            new(Nest161,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest161,0,1,1,SWSH) { Species = 349, Ability = A3 }, // Feebas
            new(Nest161,1,2,2,SWSH) { Species = 349, Ability = A3 }, // Feebas
            new(Nest161,2,3,3,SWSH) { Species = 340, Ability = A3 }, // Whiscash
            new(Nest161,3,4,4,SWSH) { Species = 130, Ability = A4 }, // Gyarados
            new(Nest161,4,4,4,SWSH) { Species = 350, Ability = A4 }, // Milotic
            new(Nest161,4,4,4,SWSH) { Species = 369, Ability = A4 }, // Relicanth
            new(Nest162,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest162,0,1,2,SWSH) { Species = 349, Ability = A2 }, // Feebas
            new(Nest162,1,2,3,SWSH) { Species = 349, Ability = A2 }, // Feebas
            new(Nest162,1,2,3,SWSH) { Species = 369, Ability = A2 }, // Relicanth
            new(Nest162,2,3,4,SWSH) { Species = 099, Ability = A2 }, // Kingler
            new(Nest162,3,4,5,SWSH) { Species = 369, Ability = A2 }, // Relicanth
            new(Nest162,3,4,5,SWSH) { Species = 350, Ability = A2 }, // Milotic
            new(Nest162,4,4,5,SWSH) { Species = 130, Ability = A2 }, // Gyarados
            new(Nest162,4,4,5,SWSH) { Species = 099, Ability = A2, CanGigantamax = true }, // Kingler
            new(Nest163,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest163,0,1,1,SWSH) { Species = 239, Ability = A3 }, // Elekid
            new(Nest163,0,1,1,SWSH) { Species = 595, Ability = A3 }, // Joltik
            new(Nest163,1,2,2,SWSH) { Species = 239, Ability = A3 }, // Elekid
            new(Nest163,1,2,2,SWSH) { Species = 871, Ability = A3 }, // Pincurchin
            new(Nest163,2,3,3,SWSH) { Species = 125, Ability = A3 }, // Electabuzz
            new(Nest163,2,3,3,SWSH) { Species = 778, Ability = A3 }, // Mimikyu
            new(Nest163,3,4,4,SWSH) { Species = 596, Ability = A4 }, // Galvantula
            new(Nest163,3,4,4,SWSH) { Species = 871, Ability = A4 }, // Pincurchin
            new(Nest163,3,4,4,SWSH) { Species = 836, Ability = A4 }, // Boltund
            new(Nest163,4,4,4,SWSH) { Species = 125, Ability = A4 }, // Electabuzz
            new(Nest163,4,4,4,SWSH) { Species = 466, Ability = A4 }, // Electivire
            new(Nest164,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest164,0,1,2,SWSH) { Species = 239, Ability = A2 }, // Elekid
            new(Nest164,1,2,3,SWSH) { Species = 702, Ability = A2 }, // Dedenne
            new(Nest164,1,2,3,SWSH) { Species = 596, Ability = A2 }, // Galvantula
            new(Nest164,2,3,4,SWSH) { Species = 125, Ability = A2 }, // Electabuzz
            new(Nest164,2,3,4,SWSH) { Species = 836, Ability = A2 }, // Boltund
            new(Nest164,3,4,5,SWSH) { Species = 871, Ability = A2 }, // Pincurchin
            new(Nest164,3,4,5,SWSH) { Species = 466, Ability = A2 }, // Electivire
            new(Nest164,4,4,5,SWSH) { Species = 466, Ability = A2 }, // Electivire
            new(Nest165,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest165,0,1,1,SWSH) { Species = 829, Ability = A3 }, // Gossifleur
            new(Nest165,1,2,2,SWSH) { Species = 347, Ability = A3 }, // Anorith
            new(Nest165,1,2,2,SWSH) { Species = 345, Ability = A3 }, // Lileep
            new(Nest165,2,3,3,SWSH) { Species = 830, Ability = A3 }, // Eldegoss
            new(Nest165,3,4,4,SWSH) { Species = 752, Ability = A4 }, // Araquanid
            new(Nest165,3,4,4,SWSH) { Species = 830, Ability = A4 }, // Eldegoss
            new(Nest165,4,4,4,SWSH) { Species = 598, Ability = A4 }, // Ferrothorn
            new(Nest166,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest166,0,1,2,SWSH) { Species = 347, Ability = A2 }, // Anorith
            new(Nest166,0,1,2,SWSH) { Species = 345, Ability = A2 }, // Lileep
            new(Nest166,1,2,3,SWSH) { Species = 347, Ability = A2 }, // Anorith
            new(Nest166,1,2,3,SWSH) { Species = 345, Ability = A2 }, // Lileep
            new(Nest166,2,3,4,SWSH) { Species = 752, Ability = A2 }, // Araquanid
            new(Nest166,2,3,4,SWSH) { Species = 012, Ability = A2 }, // Butterfree
            new(Nest166,3,4,5,SWSH) { Species = 348, Ability = A2 }, // Armaldo
            new(Nest166,3,4,5,SWSH) { Species = 346, Ability = A2 }, // Cradily
            new(Nest166,3,4,5,SWSH) { Species = 830, Ability = A2 }, // Eldegoss
            new(Nest166,4,4,5,SWSH) { Species = 012, Ability = A2, CanGigantamax = true }, // Butterfree
            new(Nest167,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest167,0,1,1,SWSH) { Species = 238, Ability = A3 }, // Smoochum
            new(Nest167,1,2,2,SWSH) { Species = 238, Ability = A3 }, // Smoochum
            new(Nest167,1,2,2,SWSH) { Species = 698, Ability = A3 }, // Amaura
            new(Nest167,2,3,3,SWSH) { Species = 221, Ability = A3 }, // Piloswine
            new(Nest167,2,3,3,SWSH) { Species = 460, Ability = A3 }, // Abomasnow
            new(Nest167,3,4,4,SWSH) { Species = 124, Ability = A4 }, // Jynx
            new(Nest167,3,4,4,SWSH) { Species = 873, Ability = A4 }, // Frosmoth
            new(Nest167,4,4,4,SWSH) { Species = 699, Ability = A4 }, // Aurorus
            new(Nest167,4,4,4,SWSH) { Species = 362, Ability = A4 }, // Glalie
            new(Nest168,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest168,0,1,2,SWSH) { Species = 361, Ability = A2 }, // Snorunt
            new(Nest168,1,2,3,SWSH) { Species = 238, Ability = A2 }, // Smoochum
            new(Nest168,1,2,3,SWSH) { Species = 698, Ability = A2 }, // Amaura
            new(Nest168,2,3,4,SWSH) { Species = 362, Ability = A2 }, // Glalie
            new(Nest168,2,3,4,SWSH) { Species = 460, Ability = A2 }, // Abomasnow
            new(Nest168,3,4,5,SWSH) { Species = 124, Ability = A2 }, // Jynx
            new(Nest168,3,4,5,SWSH) { Species = 873, Ability = A2 }, // Frosmoth
            new(Nest168,4,4,5,SWSH) { Species = 699, Ability = A2 }, // Aurorus
            new(Nest168,4,4,5,SWSH) { Species = 473, Ability = A2 }, // Mamoswine
            new(Nest169,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
          //new(Nest169,0,1,1,SWSH) { Species = 363, Ability = A3 }, // Spheal
            new(Nest169,2,2,2,SWSH) { Species = 363, Ability = A3 }, // Spheal
            new(Nest169,2,3,3,SWSH) { Species = 364, Ability = A3 }, // Sealeo
            new(Nest169,2,3,3,SWSH) { Species = 615, Ability = A3 }, // Cryogonal
            new(Nest169,3,4,4,SWSH) { Species = 584, Ability = A4 }, // Vanilluxe
            new(Nest169,3,4,4,SWSH) { Species = 614, Ability = A4 }, // Beartic
            new(Nest169,3,4,4,SWSH) { Species = 365, Ability = A4 }, // Walrein
            new(Nest169,4,4,4,SWSH) { Species = 713, Ability = A4 }, // Avalugg
            new(Nest169,4,4,4,SWSH) { Species = 131, Ability = A4 }, // Lapras
            new(Nest170,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
          //new(Nest170,0,1,2,SWSH) { Species = 131, Ability = A2 }, // Lapras
          //new(Nest170,0,1,2,SWSH) { Species = 363, Ability = A2 }, // Spheal
            new(Nest170,2,2,3,SWSH) { Species = 364, Ability = A2 }, // Sealeo
            new(Nest170,2,3,4,SWSH) { Species = 713, Ability = A2 }, // Avalugg
            new(Nest170,2,3,4,SWSH) { Species = 615, Ability = A2 }, // Cryogonal
            new(Nest170,3,4,5,SWSH) { Species = 365, Ability = A2 }, // Walrein
            new(Nest170,3,4,5,SWSH) { Species = 131, Ability = A2 }, // Lapras
            new(Nest170,3,4,5,SWSH) { Species = 584, Ability = A2 }, // Vanilluxe
            new(Nest170,4,4,5,SWSH) { Species = 365, Ability = A2 }, // Walrein
            new(Nest171,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest171,0,1,1,SWSH) { Species = 532, Ability = A3 }, // Timburr
            new(Nest171,0,1,1,SWSH) { Species = 622, Ability = A3 }, // Golett
            new(Nest171,1,2,2,SWSH) { Species = 622, Ability = A3 }, // Golett
            new(Nest171,1,2,2,SWSH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest171,2,3,3,SWSH) { Species = 533, Ability = A3 }, // Gurdurr
            new(Nest171,2,3,3,SWSH) { Species = 623, Ability = A3 }, // Golurk
            new(Nest171,3,4,4,SWSH) { Species = 534, Ability = A4 }, // Conkeldurr
            new(Nest171,3,4,4,SWSH) { Species = 623, Ability = A4 }, // Golurk
            new(Nest171,3,4,4,SWSH) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest171,4,4,4,SWSH) { Species = 623, Ability = A4 }, // Golurk
            new(Nest171,4,4,4,SWSH) { Species = 534, Ability = A4 }, // Conkeldurr
            new(Nest172,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest172,0,1,2,SWSH) { Species = 870, Ability = A2 }, // Falinks
            new(Nest172,0,1,2,SWSH) { Species = 236, Ability = A2 }, // Tyrogue
            new(Nest172,1,2,3,SWSH) { Species = 533, Ability = A2 }, // Gurdurr
            new(Nest172,2,3,4,SWSH) { Species = 870, Ability = A2 }, // Falinks
            new(Nest172,2,3,4,SWSH) { Species = 623, Ability = A2 }, // Golurk
            new(Nest172,3,4,5,SWSH) { Species = 534, Ability = A2 }, // Conkeldurr
            new(Nest172,4,4,5,SWSH) { Species = 237, Ability = A2 }, // Hitmontop
            new(Nest173,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest173,0,1,1,SWSH) { Species = 041, Ability = A3 }, // Zubat
            new(Nest173,1,2,2,SWSH) { Species = 029, Ability = A3 }, // Nidoran♀
            new(Nest173,1,2,2,SWSH) { Species = 032, Ability = A3 }, // Nidoran♂
            new(Nest173,2,3,3,SWSH) { Species = 030, Ability = A3 }, // Nidorina
            new(Nest173,2,3,3,SWSH) { Species = 033, Ability = A3 }, // Nidorino
            new(Nest173,3,4,4,SWSH) { Species = 042, Ability = A4 }, // Golbat
            new(Nest173,4,4,4,SWSH) { Species = 031, Ability = A4 }, // Nidoqueen
            new(Nest173,4,4,4,SWSH) { Species = 034, Ability = A4 }, // Nidoking
            new(Nest174,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest174,0,1,2,SWSH) { Species = 041, Ability = A2 }, // Zubat
            new(Nest174,0,1,2,SWSH) { Species = 568, Ability = A2 }, // Trubbish
            new(Nest174,1,2,3,SWSH) { Species = 079, Ability = A2, Form = 1 }, // Slowpoke-1
            new(Nest174,2,3,4,SWSH) { Species = 042, Ability = A2 }, // Golbat
            new(Nest174,2,3,4,SWSH) { Species = 569, Ability = A2 }, // Garbodor
            new(Nest174,3,4,5,SWSH) { Species = 031, Ability = A2 }, // Nidoqueen
            new(Nest174,3,4,5,SWSH) { Species = 034, Ability = A2 }, // Nidoking
            new(Nest174,4,4,5,SWSH) { Species = 169, Ability = A2 }, // Crobat
            new(Nest174,4,4,5,SWSH) { Species = 569, Ability = A2, CanGigantamax = true }, // Garbodor
            new(Nest175,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest175,0,1,1,SWSH) { Species = 041, Ability = A3 }, // Zubat
            new(Nest175,0,1,1,SWSH) { Species = 714, Ability = A3 }, // Noibat
            new(Nest175,1,2,2,SWSH) { Species = 333, Ability = A3 }, // Swablu
            new(Nest175,1,2,2,SWSH) { Species = 042, Ability = A3 }, // Golbat
            new(Nest175,2,3,3,SWSH) { Species = 042, Ability = A3 }, // Golbat
            new(Nest175,2,3,3,SWSH) { Species = 822, Ability = A3 }, // Corvisquire
            new(Nest175,3,4,4,SWSH) { Species = 042, Ability = A4 }, // Golbat
            new(Nest175,3,4,4,SWSH) { Species = 334, Ability = A4 }, // Altaria
            new(Nest175,3,4,4,SWSH) { Species = 715, Ability = A4 }, // Noivern
            new(Nest175,4,4,4,SWSH) { Species = 823, Ability = A4 }, // Corviknight
            new(Nest175,4,4,4,SWSH) { Species = 169, Ability = A4 }, // Crobat
            new(Nest176,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest176,0,1,2,SWSH) { Species = 041, Ability = A2 }, // Zubat
            new(Nest176,0,1,2,SWSH) { Species = 527, Ability = A2 }, // Woobat
            new(Nest176,1,2,3,SWSH) { Species = 822, Ability = A2 }, // Corvisquire
            new(Nest176,1,2,3,SWSH) { Species = 042, Ability = A2 }, // Golbat
            new(Nest176,2,3,4,SWSH) { Species = 528, Ability = A2 }, // Swoobat
            new(Nest176,2,3,4,SWSH) { Species = 823, Ability = A2 }, // Corviknight
            new(Nest176,3,4,5,SWSH) { Species = 142, Ability = A2 }, // Aerodactyl
            new(Nest176,3,4,5,SWSH) { Species = 334, Ability = A2 }, // Altaria
            new(Nest176,3,4,5,SWSH) { Species = 169, Ability = A2 }, // Crobat
            new(Nest176,4,4,5,SWSH) { Species = 715, Ability = A2 }, // Noivern
            new(Nest176,4,4,5,SWSH) { Species = 823, Ability = A2, CanGigantamax = true }, // Corviknight
            new(Nest177,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest177,0,1,1,SWSH) { Species = 439, Ability = A3 }, // Mime Jr.
            new(Nest177,1,2,2,SWSH) { Species = 436, Ability = A3 }, // Bronzor
            new(Nest177,1,2,2,SWSH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest177,2,3,3,SWSH) { Species = 344, Ability = A3 }, // Claydol
            new(Nest177,4,4,4,SWSH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest177,4,4,4,SWSH) { Species = 437, Ability = A4 }, // Bronzong
            new(Nest178,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest178,1,2,3,SWSH) { Species = 122, Ability = A2, Form = 1 }, // Mr. Mime-1
            new(Nest178,1,2,3,SWSH) { Species = 079, Ability = A2, Form = 1 }, // Slowpoke-1
            new(Nest178,2,3,4,SWSH) { Species = 375, Ability = A2 }, // Metang
            new(Nest178,3,4,5,SWSH) { Species = 866, Ability = A2 }, // Mr. Rime
            new(Nest178,4,4,5,SWSH) { Species = 376, Ability = A2 }, // Metagross
            new(Nest179,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest179,0,1,1,SWSH) { Species = 304, Ability = A3 }, // Aron
            new(Nest179,1,2,2,SWSH) { Species = 304, Ability = A3 }, // Aron
            new(Nest179,2,3,3,SWSH) { Species = 305, Ability = A3 }, // Lairon
            new(Nest179,3,4,4,SWSH) { Species = 305, Ability = A4 }, // Lairon
            new(Nest179,3,4,4,SWSH) { Species = 703, Ability = A4 }, // Carbink
            new(Nest179,4,4,4,SWSH) { Species = 306, Ability = A4 }, // Aggron
            new(Nest179,4,4,4,SWSH) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest180,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest180,0,1,2,SWSH) { Species = 304, Ability = A2 }, // Aron
            new(Nest180,1,2,3,SWSH) { Species = 305, Ability = A2 }, // Lairon
            new(Nest180,2,3,4,SWSH) { Species = 213, Ability = A2 }, // Shuckle
            new(Nest180,3,4,5,SWSH) { Species = 839, Ability = A2 }, // Coalossal
            new(Nest180,3,4,5,SWSH) { Species = 306, Ability = A2 }, // Aggron
            new(Nest180,4,4,5,SWSH) { Species = 306, Ability = A2 }, // Aggron
            new(Nest181,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest181,0,1,1,SWSH) { Species = 885, Ability = A3 }, // Dreepy
            new(Nest181,0,1,1,SWSH) { Species = 708, Ability = A3 }, // Phantump
            new(Nest181,1,2,2,SWSH) { Species = 778, Ability = A3 }, // Mimikyu
            new(Nest181,1,2,2,SWSH) { Species = 361, Ability = A3 }, // Snorunt
            new(Nest181,2,3,3,SWSH) { Species = 886, Ability = A3 }, // Drakloak
            new(Nest181,2,3,3,SWSH) { Species = 778, Ability = A3 }, // Mimikyu
            new(Nest181,3,4,4,SWSH) { Species = 362, Ability = A4 }, // Glalie
            new(Nest181,3,4,4,SWSH) { Species = 478, Ability = A4 }, // Froslass
            new(Nest181,4,4,4,SWSH) { Species = 709, Ability = A4 }, // Trevenant
            new(Nest181,4,4,4,SWSH) { Species = 778, Ability = A4 }, // Mimikyu
            new(Nest182,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest182,0,1,2,SWSH) { Species = 885, Ability = A2 }, // Dreepy
            new(Nest182,1,2,3,SWSH) { Species = 885, Ability = A2 }, // Dreepy
            new(Nest182,2,3,4,SWSH) { Species = 709, Ability = A2 }, // Trevenant
            new(Nest182,3,4,5,SWSH) { Species = 887, Ability = A2 }, // Dragapult
            new(Nest182,4,4,5,SWSH) { Species = 887, Ability = A2 }, // Dragapult
            new(Nest183,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest183,0,1,1,SWSH) { Species = 621, Ability = A3 }, // Druddigon
            new(Nest183,1,2,2,SWSH) { Species = 696, Ability = A3 }, // Tyrunt
            new(Nest183,2,3,3,SWSH) { Species = 147, Ability = A3 }, // Dratini
            new(Nest183,3,4,4,SWSH) { Species = 621, Ability = A4 }, // Druddigon
            new(Nest183,3,4,4,SWSH) { Species = 697, Ability = A4 }, // Tyrantrum
            new(Nest184,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest184,0,1,2,SWSH) { Species = 884, Ability = A2 }, // Duraludon
            new(Nest184,1,2,3,SWSH) { Species = 696, Ability = A2 }, // Tyrunt
            new(Nest184,2,3,4,SWSH) { Species = 884, Ability = A2 }, // Duraludon
            new(Nest184,3,4,5,SWSH) { Species = 149, Ability = A2 }, // Dragonite
            new(Nest184,3,4,5,SWSH) { Species = 697, Ability = A2 }, // Tyrantrum
            new(Nest184,4,4,5,SWSH) { Species = 884, Ability = A2, CanGigantamax = true }, // Duraludon
            new(Nest185,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest185,0,1,1,SWSH) { Species = 215, Ability = A3 }, // Sneasel
            new(Nest185,0,1,1,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest185,1,2,2,SWSH) { Species = 859, Ability = A3 }, // Impidimp
            new(Nest185,1,2,2,SWSH) { Species = 860, Ability = A3 }, // Morgrem
            new(Nest185,2,3,3,SWSH) { Species = 215, Ability = A3 }, // Sneasel
            new(Nest185,2,3,3,SWSH) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new(Nest185,3,4,4,SWSH) { Species = 861, Ability = A4 }, // Grimmsnarl
            new(Nest185,3,4,4,SWSH) { Species = 359, Ability = A4 }, // Absol
            new(Nest185,3,4,4,SWSH) { Species = 862, Ability = A4 }, // Obstagoon
            new(Nest185,4,4,4,SWSH) { Species = 359, Ability = A4 }, // Absol
            new(Nest185,4,4,4,SWSH) { Species = 461, Ability = A4 }, // Weavile
            new(Nest186,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest186,0,1,2,SWSH) { Species = 859, Ability = A2 }, // Impidimp
            new(Nest186,0,1,2,SWSH) { Species = 359, Ability = A2 }, // Absol
            new(Nest186,1,2,3,SWSH) { Species = 215, Ability = A2 }, // Sneasel
            new(Nest186,2,3,4,SWSH) { Species = 828, Ability = A2 }, // Thievul
            new(Nest186,2,3,4,SWSH) { Species = 510, Ability = A2 }, // Liepard
            new(Nest186,3,4,5,SWSH) { Species = 359, Ability = A2 }, // Absol
            new(Nest186,3,4,5,SWSH) { Species = 861, Ability = A2 }, // Grimmsnarl
            new(Nest186,3,4,5,SWSH) { Species = 461, Ability = A2 }, // Weavile
            new(Nest186,4,4,5,SWSH) { Species = 359, Ability = A2 }, // Absol
            new(Nest186,4,4,5,SWSH) { Species = 861, Ability = A2, CanGigantamax = true }, // Grimmsnarl
            new(Nest187,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest187,0,1,1,SWSH) { Species = 304, Ability = A3 }, // Aron
            new(Nest187,0,1,1,SWSH) { Species = 632, Ability = A3 }, // Durant
            new(Nest187,1,2,2,SWSH) { Species = 304, Ability = A3 }, // Aron
            new(Nest187,1,2,2,SWSH) { Species = 374, Ability = A3 }, // Beldum
            new(Nest187,2,3,3,SWSH) { Species = 305, Ability = A3 }, // Lairon
            new(Nest187,2,3,3,SWSH) { Species = 375, Ability = A3 }, // Metang
            new(Nest187,3,4,4,SWSH) { Species = 823, Ability = A4 }, // Corviknight
            new(Nest187,3,4,4,SWSH) { Species = 632, Ability = A4 }, // Durant
            new(Nest187,3,4,4,SWSH) { Species = 879, Ability = A4 }, // Copperajah
            new(Nest187,4,4,4,SWSH) { Species = 306, Ability = A4 }, // Aggron
            new(Nest188,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest188,0,1,2,SWSH) { Species = 304, Ability = A2 }, // Aron
            new(Nest188,0,1,2,SWSH) { Species = 052, Ability = A2, Form = 2 }, // Meowth-2
            new(Nest188,1,2,3,SWSH) { Species = 632, Ability = A2 }, // Durant
            new(Nest188,1,2,3,SWSH) { Species = 305, Ability = A2 }, // Lairon
            new(Nest188,2,3,4,SWSH) { Species = 863, Ability = A2 }, // Perrserker
            new(Nest188,3,4,5,SWSH) { Species = 879, Ability = A2 }, // Copperajah
            new(Nest188,3,4,5,SWSH) { Species = 306, Ability = A2 }, // Aggron
            new(Nest188,3,4,5,SWSH) { Species = 376, Ability = A2 }, // Metagross
            new(Nest188,4,4,5,SWSH) { Species = 376, Ability = A2 }, // Metagross
            new(Nest188,4,4,5,SWSH) { Species = 879, Ability = A2, CanGigantamax = true }, // Copperajah
            new(Nest189,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest189,0,1,1,SWSH) { Species = 173, Ability = A3 }, // Cleffa
            new(Nest189,0,1,1,SWSH) { Species = 703, Ability = A3 }, // Carbink
            new(Nest189,1,2,2,SWSH) { Species = 856, Ability = A3 }, // Hatenna
            new(Nest189,1,2,2,SWSH) { Species = 173, Ability = A3 }, // Cleffa
            new(Nest189,2,3,3,SWSH) { Species = 857, Ability = A3 }, // Hattrem
            new(Nest189,2,3,3,SWSH) { Species = 035, Ability = A3 }, // Clefairy
            new(Nest189,3,4,4,SWSH) { Species = 703, Ability = A4 }, // Carbink
            new(Nest189,3,4,4,SWSH) { Species = 036, Ability = A4 }, // Clefable
            new(Nest189,4,4,4,SWSH) { Species = 547, Ability = A4 }, // Whimsicott
            new(Nest189,4,4,4,SWSH) { Species = 858, Ability = A4 }, // Hatterene
            new(Nest190,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest190,0,1,2,SWSH) { Species = 703, Ability = A2 }, // Carbink
            new(Nest190,0,1,2,SWSH) { Species = 546, Ability = A2 }, // Cottonee
            new(Nest190,1,2,3,SWSH) { Species = 035, Ability = A2 }, // Clefairy
            new(Nest190,1,2,3,SWSH) { Species = 703, Ability = A2 }, // Carbink
            new(Nest190,2,3,4,SWSH) { Species = 703, Ability = A2 }, // Carbink
            new(Nest190,2,3,4,SWSH) { Species = 547, Ability = A2 }, // Whimsicott
            new(Nest190,3,4,5,SWSH) { Species = 110, Ability = A2, Form = 1 }, // Weezing-1
            new(Nest190,3,4,5,SWSH) { Species = 858, Ability = A2 }, // Hatterene
            new(Nest190,3,4,5,SWSH) { Species = 036, Ability = A2 }, // Clefable
            new(Nest190,4,4,5,SWSH) { Species = 110, Ability = A2, Form = 1 }, // Weezing-1
            new(Nest190,4,4,5,SWSH) { Species = 858, Ability = A2, CanGigantamax = true }, // Hatterene
            new(Nest191,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest191,0,1,1,SWSH) { Species = 854, Ability = A3 }, // Sinistea
            new(Nest191,1,2,2,SWSH) { Species = 854, Ability = A3 }, // Sinistea
            new(Nest191,2,3,3,SWSH) { Species = 854, Ability = A3 }, // Sinistea
            new(Nest191,3,4,4,SWSH) { Species = 854, Ability = A4 }, // Sinistea
            new(Nest191,4,4,4,SWSH) { Species = 854, Ability = A4 }, // Sinistea
            new(Nest191,2,4,4,SWSH) { Species = 854, Ability = A4, Form = 1 }, // Sinistea-1
            new(Nest192,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest192,0,0,2,SWSH) { Species = 854, Ability = A2 }, // Sinistea
            new(Nest192,1,1,2,SWSH) { Species = 854, Ability = A2 }, // Sinistea
            new(Nest192,1,1,3,SWSH) { Species = 854, Ability = A2 }, // Sinistea
            new(Nest192,2,2,4,SWSH) { Species = 854, Ability = A2 }, // Sinistea
            new(Nest192,3,3,4,SWSH) { Species = 854, Ability = A2 }, // Sinistea
            new(Nest192,0,3,5,SWSH) { Species = 854, Ability = A2, Form = 1 }, // Sinistea-1
            new(Nest192,4,4,5,SWSH) { Species = 855, Ability = A2 }, // Polteageist
            new(Nest192,4,4,5,SWSH) { Species = 855, Ability = A2, Form = 1 }, // Polteageist-1
            new(Nest192,4,4,5,SWSH) { Species = 869, Ability = A2, CanGigantamax = true }, // Alcremie
            new(Nest193,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
          //new(Nest193,0,1,1,SWSH) { Species = 133, Ability = A3 }, // Eevee
            new(Nest193,2,4,2,SWSH) { Species = 133, Ability = A3 }, // Eevee
            new(Nest193,2,4,3,SWSH) { Species = 133, Ability = A3 }, // Eevee
            new(Nest193,2,4,4,SWSH) { Species = 136, Ability = A3 }, // Flareon
            new(Nest193,2,4,4,SWSH) { Species = 135, Ability = A3 }, // Jolteon
            new(Nest193,2,4,4,SWSH) { Species = 134, Ability = A3 }, // Vaporeon
            new(Nest193,2,4,4,SWSH) { Species = 196, Ability = A4 }, // Espeon
            new(Nest193,2,4,4,SWSH) { Species = 197, Ability = A4 }, // Umbreon
            new(Nest193,2,4,4,SWSH) { Species = 470, Ability = A4 }, // Leafeon
            new(Nest193,2,4,4,SWSH) { Species = 471, Ability = A4 }, // Glaceon
            new(Nest193,2,4,4,SWSH) { Species = 700, Ability = A4 }, // Sylveon
            new(Nest194,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
          //new(Nest194,0,1,2,SWSH) { Species = 133, Ability = A2 }, // Eevee
            new(Nest194,2,4,3,SWSH) { Species = 133, Ability = A2 }, // Eevee
            new(Nest194,2,4,4,SWSH) { Species = 133, Ability = A2, Gender = 1 }, // Eevee
            new(Nest194,2,4,5,SWSH) { Species = 136, Ability = A2 }, // Flareon
            new(Nest194,2,4,5,SWSH) { Species = 135, Ability = A2 }, // Jolteon
            new(Nest194,2,4,5,SWSH) { Species = 134, Ability = A2 }, // Vaporeon
            new(Nest194,2,4,5,SWSH) { Species = 196, Ability = A2 }, // Espeon
            new(Nest194,2,4,5,SWSH) { Species = 197, Ability = A2 }, // Umbreon
            new(Nest194,2,4,5,SWSH) { Species = 470, Ability = A2 }, // Leafeon
            new(Nest194,2,4,5,SWSH) { Species = 471, Ability = A2 }, // Glaceon
            new(Nest194,2,4,5,SWSH) { Species = 700, Ability = A2 }, // Sylveon
            new(Nest195,2,4,4,SWSH) { Species = 531, Ability = A4 }, // Audino
            new(Nest195,1,2,2,SWSH) { Species = 696, Ability = A3 }, // Tyrunt
            new(Nest195,1,2,2,SWSH) { Species = 698, Ability = A3 }, // Amaura
            new(Nest195,2,3,3,SWSH) { Species = 348, Ability = A3 }, // Armaldo
            new(Nest195,2,3,3,SWSH) { Species = 346, Ability = A3 }, // Cradily
            new(Nest195,4,4,4,SWSH) { Species = 142, Ability = A4 }, // Aerodactyl
            new(Nest196,2,4,5,SWSH) { Species = 225, Ability = A2 }, // Delibird
            new(Nest196,2,2,3,SWSH) { Species = 142, Ability = A2 }, // Aerodactyl
            new(Nest196,3,4,5,SWSH) { Species = 142, Ability = A2 }, // Aerodactyl
            new(Nest196,4,4,5,SWSH) { Species = 880, Ability = A2 }, // Dracozolt
            new(Nest196,4,4,5,SWSH) { Species = 882, Ability = A2 }, // Dracovish
            new(Nest196,3,4,5,SWSH) { Species = 881, Ability = A2 }, // Arctozolt
            new(Nest196,3,4,5,SWSH) { Species = 883, Ability = A2 }, // Arctovish
        };

        internal static readonly EncounterStatic8N[] Nest_SW =
        {
            new(Nest000,0,1,1,SW) { Species = 559, Ability = A3 }, // Scraggy
            new(Nest000,2,3,3,SW) { Species = 106, Ability = A3 }, // Hitmonlee
            new(Nest000,2,4,4,SW) { Species = 107, Ability = A3 }, // Hitmonchan
            new(Nest000,2,4,4,SW) { Species = 560, Ability = A3 }, // Scrafty
            new(Nest000,3,4,4,SW) { Species = 534, Ability = A4 }, // Conkeldurr
            new(Nest000,4,4,4,SW) { Species = 237, Ability = A4 }, // Hitmontop
            new(Nest001,0,1,1,SW) { Species = 574, Ability = A3 }, // Gothita
            new(Nest001,2,3,3,SW) { Species = 678, Ability = A3, Gender = 0 }, // Meowstic
            new(Nest001,2,3,3,SW) { Species = 575, Ability = A3 }, // Gothorita
            new(Nest001,3,4,4,SW) { Species = 576, Ability = A4 }, // Gothitelle
            new(Nest001,4,4,4,SW) { Species = 338, Ability = A4 }, // Solrock
            new(Nest002,0,0,1,SW) { Species = 524, Ability = A3 }, // Roggenrola
            new(Nest002,0,1,1,SW) { Species = 688, Ability = A3 }, // Binacle
            new(Nest002,3,4,4,SW) { Species = 558, Ability = A4 }, // Crustle
            new(Nest002,4,4,4,SW) { Species = 526, Ability = A4 }, // Gigalith
          //new(Nest006,0,1,1,SW) { Species = 223, Ability = A3 }, // Remoraid
          //new(Nest006,0,1,1,SW) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest006,2,2,2,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest007,2,2,2,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest008,1,1,2,SW) { Species = 090, Ability = A3 }, // Shellder
            new(Nest009,1,1,2,SW) { Species = 083, Ability = A3, Form = 1 }, // Farfetch’d-1
            new(Nest009,1,2,2,SW) { Species = 539, Ability = A3 }, // Sawk
            new(Nest009,3,4,4,SW) { Species = 865, Ability = A4 }, // Sirfetch’d
            new(Nest011,4,4,4,SW) { Species = 303, Ability = A4 }, // Mawile
            new(Nest012,0,1,1,SW) { Species = 177, Ability = A3 }, // Natu
            new(Nest012,0,1,1,SW) { Species = 856, Ability = A3 }, // Hatenna
            new(Nest012,1,1,2,SW) { Species = 825, Ability = A3 }, // Dottler
            new(Nest012,1,3,2,SW) { Species = 857, Ability = A3 }, // Hattrem
            new(Nest012,2,4,4,SW) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new(Nest012,3,4,4,SW) { Species = 561, Ability = A4 }, // Sigilyph
            new(Nest012,4,4,4,SW) { Species = 826, Ability = A4 }, // Orbeetle
            new(Nest013,2,4,4,SW) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new(Nest014,0,0,1,SW) { Species = 524, Ability = A3 }, // Roggenrola
            new(Nest014,0,1,1,SW) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest014,2,4,4,SW) { Species = 095, Ability = A3 }, // Onix
            new(Nest014,3,4,4,SW) { Species = 839, Ability = A4 }, // Coalossal
            new(Nest014,4,4,4,SW) { Species = 526, Ability = A4 }, // Gigalith
            new(Nest016,0,1,1,SW) { Species = 220, Ability = A3 }, // Swinub
            new(Nest016,1,1,1,SW) { Species = 328, Ability = A3 }, // Trapinch
            new(Nest016,2,3,3,SW) { Species = 329, Ability = A3 }, // Vibrava
            new(Nest016,2,4,4,SW) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new(Nest016,3,4,4,SW) { Species = 450, Ability = A4 }, // Hippowdon
            new(Nest016,4,4,4,SW) { Species = 330, Ability = A4 }, // Flygon
            new(Nest017,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest017,1,1,1,SW) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new(Nest017,1,2,2,SW) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest017,2,3,3,SW) { Species = 608, Ability = A3 }, // Lampent
            new(Nest017,2,4,4,SW) { Species = 038, Ability = A3 }, // Ninetales
            new(Nest017,3,4,4,SW) { Species = 851, Ability = A4 }, // Centiskorch
            new(Nest017,4,4,4,SW) { Species = 631, Ability = A4 }, // Heatmor
            new(Nest017,4,4,4,SW) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new(Nest018,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest018,0,1,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest018,1,2,2,SW) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest018,2,3,3,SW) { Species = 608, Ability = A3 }, // Lampent
            new(Nest018,2,4,4,SW) { Species = 038, Ability = A3 }, // Ninetales
            new(Nest018,2,4,4,SW) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest018,3,4,4,SW) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new(Nest018,4,4,4,SW) { Species = 776, Ability = A4 }, // Turtonator
            new(Nest019,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest019,1,1,1,SW) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new(Nest019,1,2,2,SW) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest019,2,3,3,SW) { Species = 838, Ability = A3 }, // Carkol
            new(Nest019,2,4,4,SW) { Species = 038, Ability = A3 }, // Ninetales
            new(Nest019,4,4,4,SW) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new(Nest022,0,1,1,SW) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new(Nest022,4,4,4,SW) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new(Nest025,0,0,1,SW) { Species = 273, Ability = A3 }, // Seedot
            new(Nest025,1,1,2,SW) { Species = 274, Ability = A3 }, // Nuzleaf
            new(Nest025,2,4,4,SW) { Species = 275, Ability = A3 }, // Shiftry
            new(Nest026,4,4,4,SW) { Species = 841, Ability = A4 }, // Flapple
            new(Nest028,0,1,1,SW) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest028,1,1,2,SW) { Species = 043, Ability = A3 }, // Oddish
            new(Nest028,3,4,4,SW) { Species = 748, Ability = A4 }, // Toxapex
            new(Nest028,4,4,4,SW) { Species = 435, Ability = A4 }, // Skuntank
            new(Nest030,0,1,1,SW) { Species = 627, Ability = A3 }, // Rufflet
            new(Nest030,4,4,4,SW) { Species = 628, Ability = A4 }, // Braviary
            new(Nest032,0,1,1,SW) { Species = 684, Ability = A3 }, // Swirlix
            new(Nest032,4,4,4,SW) { Species = 685, Ability = A4 }, // Slurpuff
            new(Nest033,4,4,4,SW) { Species = 303, Ability = A4 }, // Mawile
            new(Nest034,4,4,4,SW) { Species = 275, Ability = A4 }, // Shiftry
            new(Nest035,1,1,2,SW) { Species = 633, Ability = A3 }, // Deino
            new(Nest035,3,4,4,SW) { Species = 634, Ability = A4 }, // Zweilous
            new(Nest035,4,4,4,SW) { Species = 635, Ability = A4 }, // Hydreigon
            new(Nest036,0,0,1,SW) { Species = 328, Ability = A3 }, // Trapinch
            new(Nest036,0,1,1,SW) { Species = 610, Ability = A3 }, // Axew
            new(Nest036,1,1,2,SW) { Species = 782, Ability = A3 }, // Jangmo-o
            new(Nest036,2,3,3,SW) { Species = 783, Ability = A3 }, // Hakamo-o
            new(Nest036,2,4,4,SW) { Species = 611, Ability = A3 }, // Fraxure
            new(Nest036,2,4,4,SW) { Species = 612, Ability = A3 }, // Haxorus
            new(Nest036,3,4,4,SW) { Species = 330, Ability = A4 }, // Flygon
            new(Nest036,4,4,4,SW) { Species = 776, Ability = A4 }, // Turtonator
            new(Nest036,4,4,4,SW) { Species = 784, Ability = A4 }, // Kommo-o
          //new(Nest037,0,1,1,SW) { Species = 782, Ability = A3 }, // Jangmo-o
            new(Nest037,2,4,4,SW) { Species = 783, Ability = A3 }, // Hakamo-o
            new(Nest037,3,4,4,SW) { Species = 784, Ability = A4 }, // Kommo-o
            new(Nest037,4,4,4,SW) { Species = 841, Ability = A4 }, // Flapple
            new(Nest039,1,1,2,SW) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new(Nest039,4,4,4,SW) { Species = 628, Ability = A4 }, // Braviary
          //new(Nest040,0,1,1,SW) { Species = 747, Ability = A3 }, // Mareanie
          //new(Nest040,1,1,2,SW) { Species = 536, Ability = A3 }, // Palpitoad
            new(Nest040,2,3,3,SW) { Species = 091, Ability = A3 }, // Cloyster
            new(Nest040,2,4,4,SW) { Species = 746, Ability = A3 }, // Wishiwashi
            new(Nest040,3,4,4,SW) { Species = 537, Ability = A4 }, // Seismitoad
            new(Nest040,4,4,4,SW) { Species = 748, Ability = A4 }, // Toxapex
            new(Nest042,1,3,2,SW) { Species = 710, Ability = A3 }, // Pumpkaboo
            new(Nest042,4,4,4,SW) { Species = 867, Ability = A3 }, // Runerigus
            new(Nest042,3,4,4,SW) { Species = 855, Ability = A4 }, // Polteageist
            new(Nest042,3,4,4,SW) { Species = 711, Ability = A4 }, // Gourgeist
            new(Nest043,2,2,2,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest044,1,2,2,SW) { Species = 632, Ability = A3 }, // Durant
            new(Nest044,2,3,3,SW) { Species = 600, Ability = A3 }, // Klang
            new(Nest045,0,1,1,SW) { Species = 588, Ability = A3 }, // Karrablast
            new(Nest045,1,2,2,SW) { Species = 616, Ability = A3 }, // Shelmet
            new(Nest045,4,4,4,SW) { Species = 589, Ability = A4 }, // Escavalier
            new(Nest046,1,3,3,SW) { Species = 710, Ability = A3 }, // Pumpkaboo
            new(Nest046,2,4,4,SW) { Species = 711, Ability = A3 }, // Gourgeist
            new(Nest046,3,4,4,SW) { Species = 711, Ability = A4 }, // Gourgeist
            new(Nest047,0,1,1,SW) { Species = 559, Ability = A3 }, // Scraggy
            new(Nest047,2,4,4,SW) { Species = 560, Ability = A3 }, // Scrafty
            new(Nest047,3,4,4,SW) { Species = 766, Ability = A4 }, // Passimian
            new(Nest050,0,1,1,SW) { Species = 688, Ability = A3 }, // Binacle
            new(Nest050,1,2,2,SW) { Species = 185, Ability = A3 }, // Sudowoodo
            new(Nest050,2,3,3,SW) { Species = 689, Ability = A3 }, // Barbaracle
            new(Nest050,4,4,4,SW) { Species = 874, Ability = A4 }, // Stonjourner
            new(Nest052,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest052,1,2,2,SW) { Species = 038, Ability = A3 }, // Ninetales
            new(Nest052,3,4,4,SW) { Species = 038, Ability = A4 }, // Ninetales
            new(Nest053,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest053,1,2,2,SW) { Species = 608, Ability = A3 }, // Lampent
            new(Nest053,2,3,3,SW) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest053,3,4,4,SW) { Species = 038, Ability = A4 }, // Ninetales
            new(Nest053,4,4,4,SW) { Species = 776, Ability = A4 }, // Turtonator
            new(Nest054,0,0,1,SW) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new(Nest054,4,4,4,SW) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new(Nest057,0,0,1,SW) { Species = 273, Ability = A3 }, // Seedot
            new(Nest057,1,2,2,SW) { Species = 274, Ability = A3 }, // Nuzleaf
            new(Nest057,2,4,4,SW) { Species = 275, Ability = A3 }, // Shiftry
            new(Nest057,4,4,4,SW) { Species = 841, Ability = A4 }, // Flapple
            new(Nest058,0,0,1,SW) { Species = 273, Ability = A3 }, // Seedot
            new(Nest058,1,2,2,SW) { Species = 274, Ability = A3 }, // Nuzleaf
            new(Nest058,4,4,4,SW) { Species = 275, Ability = A4 }, // Shiftry
            new(Nest059,1,2,2,SW) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest059,4,4,4,SW) { Species = 748, Ability = A4 }, // Toxapex
            new(Nest061,2,4,4,SW) { Species = 303, Ability = A3 }, // Mawile
            new(Nest062,0,1,1,SW) { Species = 559, Ability = A3 }, // Scraggy
            new(Nest062,3,4,4,SW) { Species = 560, Ability = A4 }, // Scrafty
            new(Nest062,4,4,4,SW) { Species = 635, Ability = A4 }, // Hydreigon
            new(Nest063,0,0,1,SW) { Species = 328, Ability = A3 }, // Trapinch
            new(Nest063,0,1,1,SW) { Species = 610, Ability = A3 }, // Axew
            new(Nest063,0,1,1,SW) { Species = 782, Ability = A3 }, // Jangmo-o
            new(Nest063,1,2,2,SW) { Species = 611, Ability = A3 }, // Fraxure
            new(Nest063,2,4,4,SW) { Species = 783, Ability = A3 }, // Hakamo-o
            new(Nest063,2,4,4,SW) { Species = 776, Ability = A3 }, // Turtonator
            new(Nest063,3,4,4,SW) { Species = 784, Ability = A4 }, // Kommo-o
            new(Nest063,4,4,4,SW) { Species = 612, Ability = A4 }, // Haxorus
            new(Nest064,3,4,4,SW) { Species = 628, Ability = A4 }, // Braviary
            new(Nest064,3,4,4,SW) { Species = 876, Ability = A4, Gender = 0 }, // Indeedee
            new(Nest066,2,2,2,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest070,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest070,0,1,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest070,1,2,2,SW) { Species = 608, Ability = A3 }, // Lampent
            new(Nest070,2,3,3,SW) { Species = 631, Ability = A3 }, // Heatmor
          //new(Nest073,0,0,1,SW) { Species = 684, Ability = A3 }, // Swirlix
            new(Nest073,2,4,4,SW) { Species = 685, Ability = A3 }, // Slurpuff
            new(Nest075,2,4,4,SW) { Species = 550, Ability = A3 }, // Basculin
          //new(Nest076,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest076,2,2,2,SW) { Species = 038, Ability = A3 }, // Ninetales
            new(Nest076,3,4,4,SW) { Species = 038, Ability = A4 }, // Ninetales
            new(Nest077,1,2,2,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest078,0,0,1,SW) { Species = 273, Ability = A3 }, // Seedot
            new(Nest078,1,2,2,SW) { Species = 274, Ability = A3 }, // Nuzleaf
            new(Nest078,2,4,4,SW) { Species = 275, Ability = A3 }, // Shiftry
            new(Nest078,4,4,4,SW) { Species = 841, Ability = A4, CanGigantamax = true }, // Flapple
            new(Nest079,0,0,1,SW) { Species = 037, Ability = A3 }, // Vulpix
            new(Nest079,3,4,4,SW) { Species = 038, Ability = A4 }, // Ninetales
            new(Nest080,0,0,1,SW) { Species = 447, Ability = A3 }, // Riolu
            new(Nest080,0,0,1,SW) { Species = 066, Ability = A3 }, // Machop
            new(Nest080,0,1,1,SW) { Species = 759, Ability = A3 }, // Stufful
            new(Nest080,0,1,1,SW) { Species = 083, Ability = A3, Form = 1 }, // Farfetch’d-1
            new(Nest080,1,2,2,SW) { Species = 760, Ability = A3 }, // Bewear
            new(Nest080,1,3,3,SW) { Species = 067, Ability = A3 }, // Machoke
            new(Nest080,2,3,3,SW) { Species = 870, Ability = A3 }, // Falinks
            new(Nest080,2,4,4,SW) { Species = 701, Ability = A3 }, // Hawlucha
            new(Nest080,3,4,4,SW) { Species = 448, Ability = A4 }, // Lucario
            new(Nest080,3,4,4,SW) { Species = 475, Ability = A4 }, // Gallade
            new(Nest080,4,4,4,SW) { Species = 865, Ability = A4 }, // Sirfetch’d
            new(Nest080,4,4,4,SW) { Species = 068, Ability = A4, CanGigantamax = true }, // Machamp
            new(Nest081,0,0,1,SW) { Species = 755, Ability = A3 }, // Morelull
            new(Nest081,2,4,4,SW) { Species = 303, Ability = A3 }, // Mawile
            new(Nest082,0,0,1,SW) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest082,0,0,1,SW) { Species = 438, Ability = A3 }, // Bonsly
            new(Nest082,0,1,1,SW) { Species = 837, Ability = A3 }, // Rolycoly
            new(Nest082,0,1,1,SW) { Species = 688, Ability = A3 }, // Binacle
            new(Nest082,1,2,2,SW) { Species = 838, Ability = A3 }, // Carkol
            new(Nest082,1,2,2,SW) { Species = 185, Ability = A3 }, // Sudowoodo
            new(Nest082,2,3,3,SW) { Species = 689, Ability = A3 }, // Barbaracle
            new(Nest082,2,4,4,SW) { Species = 095, Ability = A3 }, // Onix
            new(Nest082,3,4,4,SW) { Species = 558, Ability = A4 }, // Crustle
            new(Nest082,3,4,4,SW) { Species = 208, Ability = A4 }, // Steelix
            new(Nest082,4,4,4,SW) { Species = 874, Ability = A4 }, // Stonjourner
            new(Nest082,4,4,4,SW) { Species = 839, Ability = A4, CanGigantamax = true }, // Coalossal
            new(Nest083,1,2,2,SW) { Species = 632, Ability = A3 }, // Durant
            new(Nest083,2,3,3,SW) { Species = 600, Ability = A3 }, // Klang
            new(Nest085,1,2,2,SW) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest085,4,4,4,SW) { Species = 748, Ability = A4 }, // Toxapex
            new(Nest086,0,0,1,SW) { Species = 684, Ability = A3 }, // Swirlix
            new(Nest086,2,3,3,SW) { Species = 685, Ability = A3 }, // Slurpuff
            new(Nest087,0,1,1,SW) { Species = 559, Ability = A3 }, // Scraggy
            new(Nest087,3,4,4,SW) { Species = 560, Ability = A4 }, // Scrafty
            new(Nest087,4,4,4,SW) { Species = 635, Ability = A4 }, // Hydreigon
            new(Nest089,0,1,1,SW) { Species = 588, Ability = A3 }, // Karrablast
            new(Nest089,1,2,2,SW) { Species = 616, Ability = A3 }, // Shelmet
            new(Nest089,4,4,4,SW) { Species = 589, Ability = A4 }, // Escavalier
            new(Nest090,1,2,2,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest091,0,1,1,SW) { Species = 588, Ability = A3 }, // Karrablast
            new(Nest091,1,2,2,SW) { Species = 616, Ability = A3 }, // Shelmet
            new(Nest091,4,4,4,SW) { Species = 589, Ability = A4 }, // Escavalier
            new(Nest106,1,2,2,SW) { Species = 627, Ability = A3 }, // Rufflet
            new(Nest106,3,4,4,SW) { Species = 628, Ability = A4 }, // Braviary
            new(Nest106,4,4,4,SW) { Species = 628, Ability = A4 }, // Braviary
            new(Nest107,1,2,2,SW) { Species = 627, Ability = A2 }, // Rufflet
            new(Nest107,4,4,4,SW) { Species = 628, Ability = A2 }, // Braviary
            new(Nest108,0,1,1,SW) { Species = 127, Ability = A3 }, // Pinsir
            new(Nest108,1,2,2,SW) { Species = 127, Ability = A3 }, // Pinsir
            new(Nest108,2,3,3,SW) { Species = 127, Ability = A3 }, // Pinsir
            new(Nest108,3,4,4,SW) { Species = 127, Ability = A4 }, // Pinsir
            new(Nest109,0,1,1,SW) { Species = 127, Ability = A2 }, // Pinsir
            new(Nest109,4,4,4,SW) { Species = 127, Ability = A2 }, // Pinsir
            new(Nest113,4,4,4,SW) { Species = 038, Ability = A2 }, // Ninetales
            new(Nest114,4,4,4,SW) { Species = 745, Ability = A4 }, // Lycanroc
            new(Nest115,3,4,4,SW) { Species = 745, Ability = A2 }, // Lycanroc
            new(Nest116,2,3,3,SW) { Species = 064, Ability = A3 }, // Kadabra
            new(Nest117,2,3,3,SW) { Species = 064, Ability = A2 }, // Kadabra
            new(Nest117,3,4,4,SW) { Species = 876, Ability = A2, Gender = 0 }, // Indeedee
            new(Nest117,4,4,4,SW) { Species = 678, Ability = A2, Gender = 0 }, // Meowstic
            new(Nest122,1,2,2,SW) { Species = 559, Ability = A3 }, // Scraggy
            new(Nest122,2,3,3,SW) { Species = 766, Ability = A3 }, // Passimian
            new(Nest122,2,3,3,SW) { Species = 560, Ability = A3 }, // Scrafty
            new(Nest122,3,4,4,SW) { Species = 560, Ability = A4 }, // Scrafty
            new(Nest123,0,1,1,SW) { Species = 559, Ability = A2 }, // Scraggy
            new(Nest123,1,2,2,SW) { Species = 539, Ability = A2 }, // Sawk
            new(Nest123,2,3,3,SW) { Species = 766, Ability = A2 }, // Passimian
            new(Nest123,3,4,4,SW) { Species = 539, Ability = A2 }, // Sawk
            new(Nest123,3,4,4,SW) { Species = 560, Ability = A2 }, // Scrafty
            new(Nest123,4,4,4,SW) { Species = 865, Ability = A2 }, // Sirfetch’d
            new(Nest127,4,4,4,SW) { Species = 770, Ability = A2 }, // Palossand
          //new(Nest130,0,1,1,SW) { Species = 782, Ability = A3 }, // Jangmo-o
            new(Nest130,2,3,3,SW) { Species = 783, Ability = A3 }, // Hakamo-o
            new(Nest130,3,4,4,SW) { Species = 841, Ability = A4 }, // Flapple
            new(Nest130,4,4,4,SW) { Species = 784, Ability = A4 }, // Kommo-o
            new(Nest131,2,2,2,SW) { Species = 776, Ability = A2 }, // Turtonator
            new(Nest131,2,2,2,SW) { Species = 782, Ability = A2 }, // Jangmo-o
            new(Nest131,2,3,3,SW) { Species = 783, Ability = A2 }, // Hakamo-o
            new(Nest131,3,4,4,SW) { Species = 776, Ability = A2 }, // Turtonator
            new(Nest131,4,4,4,SW) { Species = 784, Ability = A2 }, // Kommo-o
            new(Nest135,4,4,4,SW) { Species = 550, Ability = A2 }, // Basculin
          //new(Nest138,0,1,1,SW) { Species = 692, Ability = A3 }, // Clauncher
            new(Nest138,2,2,2,SW) { Species = 692, Ability = A3 }, // Clauncher
            new(Nest138,4,4,4,SW) { Species = 693, Ability = A4 }, // Clawitzer
          //new(Nest139,0,1,1,SW) { Species = 692, Ability = A2 }, // Clauncher
            new(Nest139,3,4,4,SW) { Species = 693, Ability = A2 }, // Clawitzer
            new(Nest139,4,4,4,SW) { Species = 693, Ability = A2 }, // Clawitzer
            new(Nest147,1,2,2,SW) { Species = 550, Ability = A2 }, // Basculin
            new(Nest147,3,4,4,SW) { Species = 550, Ability = A2 }, // Basculin
            new(Nest148,1,2,2,SW) { Species = 127, Ability = A2 }, // Pinsir
            new(Nest150,2,3,3,SW) { Species = 841, Ability = A2 }, // Flapple
            new(Nest150,3,4,4,SW) { Species = 841, Ability = A2 }, // Flapple
            new(Nest150,4,4,4,SW) { Species = 841, Ability = A2, CanGigantamax = true }, // Flapple
            new(Nest155,4,4,4,SW) { Species = 745, Ability = A4 }, // Lycanroc
            new(Nest160,3,4,5,SW) { Species = 038, Ability = A2 }, // Ninetales
            new(Nest161,0,1,1,SW) { Species = 138, Ability = A3 }, // Omanyte
            new(Nest161,1,2,2,SW) { Species = 138, Ability = A3 }, // Omanyte
            new(Nest161,2,3,3,SW) { Species = 550, Ability = A3 }, // Basculin
            new(Nest161,3,4,4,SW) { Species = 139, Ability = A4 }, // Omastar
            new(Nest161,3,4,4,SW) { Species = 550, Ability = A4 }, // Basculin
            new(Nest162,0,1,2,SW) { Species = 138, Ability = A2 }, // Omanyte
            new(Nest162,2,3,4,SW) { Species = 550, Ability = A2 }, // Basculin
            new(Nest162,3,4,5,SW) { Species = 139, Ability = A2 }, // Omastar
            new(Nest164,3,4,5,SW) { Species = 849, Ability = A2 }, // Toxtricity
            new(Nest164,4,4,5,SW) { Species = 849, Ability = A2, CanGigantamax = true }, // Toxtricity
            new(Nest165,0,1,1,SW) { Species = 347, Ability = A3 }, // Anorith
            new(Nest165,2,3,3,SW) { Species = 347, Ability = A3 }, // Anorith
            new(Nest165,3,4,4,SW) { Species = 348, Ability = A4 }, // Armaldo
            new(Nest165,4,4,4,SW) { Species = 346, Ability = A4 }, // Cradily
            new(Nest166,4,4,5,SW) { Species = 348, Ability = A2 }, // Armaldo
            new(Nest167,0,1,1,SW) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new(Nest167,3,4,4,SW) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new(Nest168,0,1,2,SW) { Species = 554, Ability = A2, Form = 1 }, // Darumaka-1
            new(Nest168,3,4,5,SW) { Species = 555, Ability = A2, Form = 2 }, // Darmanitan-2
          //new(Nest169,0,1,1,SW) { Species = 613, Ability = A3 }, // Cubchoo
            new(Nest169,2,2,2,SW) { Species = 712, Ability = A3 }, // Bergmite
            new(Nest170,4,4,5,SW) { Species = 131, Ability = A2 }, // Lapras
            new(Nest172,1,2,3,SW) { Species = 083, Ability = A2, Form = 1 }, // Farfetch’d-1
            new(Nest172,3,4,5,SW) { Species = 865, Ability = A2 }, // Sirfetch’d
            new(Nest172,3,4,5,SW) { Species = 106, Ability = A2 }, // Hitmonlee
            new(Nest172,4,4,5,SW) { Species = 068, Ability = A2, CanGigantamax = true }, // Machamp
            new(Nest173,0,1,1,SW) { Species = 029, Ability = A3 }, // Nidoran♀
            new(Nest173,3,4,4,SW) { Species = 031, Ability = A4 }, // Nidoqueen
            new(Nest174,1,2,3,SW) { Species = 030, Ability = A2 }, // Nidorina
            new(Nest177,0,1,1,SW) { Species = 343, Ability = A3 }, // Baltoy
            new(Nest177,2,3,3,SW) { Species = 575, Ability = A3 }, // Gothorita
            new(Nest177,3,4,4,SW) { Species = 876, Ability = A4, Gender = 0 }, // Indeedee
            new(Nest177,3,4,4,SW) { Species = 576, Ability = A4 }, // Gothitelle
            new(Nest177,3,4,4,SW) { Species = 344, Ability = A4 }, // Claydol
            new(Nest178,0,1,2,SW) { Species = 876, Ability = A2, Gender = 0 }, // Indeedee
            new(Nest178,0,1,2,SW) { Species = 574, Ability = A2 }, // Gothita
            new(Nest178,2,3,4,SW) { Species = 876, Ability = A2, Gender = 0 }, // Indeedee
            new(Nest178,3,4,5,SW) { Species = 876, Ability = A2, Gender = 0 }, // Indeedee
            new(Nest178,3,4,5,SW) { Species = 576, Ability = A2 }, // Gothitelle
            new(Nest178,4,4,5,SW) { Species = 576, Ability = A2 }, // Gothitelle
            new(Nest179,0,1,1,SW) { Species = 874, Ability = A3 }, // Stonjourner
            new(Nest179,1,2,2,SW) { Species = 874, Ability = A3 }, // Stonjourner
            new(Nest179,2,3,3,SW) { Species = 874, Ability = A3 }, // Stonjourner
            new(Nest179,3,4,4,SW) { Species = 303, Ability = A4 }, // Mawile
            new(Nest180,0,1,2,SW) { Species = 303, Ability = A2 }, // Mawile
            new(Nest180,2,3,4,SW) { Species = 303, Ability = A2 }, // Mawile
            new(Nest180,3,4,5,SW) { Species = 303, Ability = A2 }, // Mawile
            new(Nest180,4,4,5,SW) { Species = 839, Ability = A2, CanGigantamax = true }, // Coalossal
            new(Nest181,3,4,4,SW) { Species = 303, Ability = A4 }, // Mawile
            new(Nest182,1,2,3,SW) { Species = 093, Ability = A2 }, // Haunter
            new(Nest182,2,3,4,SW) { Species = 303, Ability = A2 }, // Mawile
            new(Nest182,3,4,5,SW) { Species = 709, Ability = A2 }, // Trevenant
            new(Nest182,3,4,5,SW) { Species = 303, Ability = A2 }, // Mawile
            new(Nest182,4,4,5,SW) { Species = 094, Ability = A2 }, // Gengar
            new(Nest183,0,1,1,SW) { Species = 371, Ability = A3 }, // Bagon
            new(Nest183,1,2,2,SW) { Species = 371, Ability = A3 }, // Bagon
            new(Nest183,2,3,3,SW) { Species = 372, Ability = A3 }, // Shelgon
            new(Nest183,3,4,4,SW) { Species = 372, Ability = A4 }, // Shelgon
            new(Nest183,4,4,4,SW) { Species = 373, Ability = A4 }, // Salamence
            new(Nest184,0,1,2,SW) { Species = 371, Ability = A2 }, // Bagon
            new(Nest184,1,2,3,SW) { Species = 776, Ability = A2 }, // Turtonator
            new(Nest184,2,3,4,SW) { Species = 372, Ability = A2 }, // Shelgon
            new(Nest184,3,4,5,SW) { Species = 373, Ability = A2 }, // Salamence
            new(Nest184,4,4,5,SW) { Species = 373, Ability = A2 }, // Salamence
            new(Nest192,3,4,5,SW) { Species = 854, Ability = A2 }, // Sinistea
            new(Nest195,0,1,1,SW) { Species = 138, Ability = A3 }, // Omanyte
            new(Nest195,0,1,1,SW) { Species = 347, Ability = A3 }, // Anorith
            new(Nest195,3,4,4,SW) { Species = 139, Ability = A4 }, // Omastar
            new(Nest195,3,4,4,SW) { Species = 348, Ability = A4 }, // Armaldo
            new(Nest195,3,4,4,SW) { Species = 697, Ability = A4 }, // Tyrantrum
            new(Nest195,4,4,4,SW) { Species = 699, Ability = A4 }, // Aurorus
            new(Nest196,0,1,2,SW) { Species = 138, Ability = A2 }, // Omanyte
            new(Nest196,2,2,2,SW) { Species = 139, Ability = A2 }, // Omastar
            new(Nest196,2,2,3,SW) { Species = 881, Ability = A2 }, // Arctozolt
            new(Nest196,2,3,4,SW) { Species = 880, Ability = A2 }, // Dracozolt
            new(Nest196,3,3,4,SW) { Species = 882, Ability = A2 }, // Dracovish
        };

        internal static readonly EncounterStatic8N[] Nest_SH =
        {
            new(Nest000,0,1,1,SH) { Species = 453, Ability = A3 }, // Croagunk
            new(Nest000,2,3,3,SH) { Species = 107, Ability = A3 }, // Hitmonchan
            new(Nest000,2,4,4,SH) { Species = 106, Ability = A3 }, // Hitmonlee
            new(Nest000,2,4,4,SH) { Species = 454, Ability = A3 }, // Toxicroak
            new(Nest000,3,4,4,SH) { Species = 237, Ability = A4 }, // Hitmontop
            new(Nest000,4,4,4,SH) { Species = 534, Ability = A4 }, // Conkeldurr
            new(Nest001,0,1,1,SH) { Species = 577, Ability = A3 }, // Solosis
            new(Nest001,2,3,3,SH) { Species = 678, Ability = A3, Gender = 1, Form = 1 }, // Meowstic-1
            new(Nest001,2,3,3,SH) { Species = 578, Ability = A3 }, // Duosion
            new(Nest001,3,4,4,SH) { Species = 579, Ability = A4 }, // Reuniclus
            new(Nest001,4,4,4,SH) { Species = 337, Ability = A4 }, // Lunatone
            new(Nest002,0,0,1,SH) { Species = 688, Ability = A3 }, // Binacle
            new(Nest002,0,1,1,SH) { Species = 524, Ability = A3 }, // Roggenrola
            new(Nest002,3,4,4,SH) { Species = 526, Ability = A4 }, // Gigalith
            new(Nest002,4,4,4,SH) { Species = 558, Ability = A4 }, // Crustle
          //new(Nest006,0,1,2,SH) { Species = 223, Ability = A3 }, // Remoraid
          //new(Nest006,0,1,2,SH) { Species = 170, Ability = A3 }, // Chinchou
            new(Nest006,2,2,2,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest007,2,2,2,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest008,1,1,1,SH) { Species = 090, Ability = A3 }, // Shellder
            new(Nest009,1,1,2,SH) { Species = 759, Ability = A3 }, // Stufful
            new(Nest009,1,2,2,SH) { Species = 538, Ability = A3 }, // Throh
            new(Nest009,3,4,4,SH) { Species = 760, Ability = A4 }, // Bewear
            new(Nest011,4,4,4,SH) { Species = 208, Ability = A4 }, // Steelix
            new(Nest012,0,1,2,SH) { Species = 177, Ability = A3 }, // Natu
            new(Nest012,0,1,2,SH) { Species = 856, Ability = A3 }, // Hatenna
            new(Nest012,1,1,2,SH) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new(Nest012,1,3,3,SH) { Species = 857, Ability = A3 }, // Hattrem
            new(Nest012,2,4,4,SH) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest012,3,4,4,SH) { Species = 765, Ability = A4 }, // Oranguru
            new(Nest012,4,4,4,SH) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new(Nest013,2,4,4,SH) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest014,0,0,1,SH) { Species = 557, Ability = A3 }, // Dwebble
            new(Nest014,0,1,1,SH) { Species = 524, Ability = A3 }, // Roggenrola
            new(Nest014,2,4,4,SH) { Species = 839, Ability = A3 }, // Coalossal
            new(Nest014,3,4,4,SH) { Species = 526, Ability = A4 }, // Gigalith
            new(Nest014,4,4,4,SH) { Species = 095, Ability = A4 }, // Onix
            new(Nest016,0,1,1,SH) { Species = 328, Ability = A3 }, // Trapinch
            new(Nest016,1,1,1,SH) { Species = 220, Ability = A3 }, // Swinub
            new(Nest016,2,3,3,SH) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new(Nest016,2,4,4,SH) { Species = 329, Ability = A3 }, // Vibrava
            new(Nest016,3,4,4,SH) { Species = 330, Ability = A4 }, // Flygon
            new(Nest016,4,4,4,SH) { Species = 450, Ability = A4 }, // Hippowdon
            new(Nest017,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest017,1,1,1,SH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest017,1,2,2,SH) { Species = 608, Ability = A3 }, // Lampent
            new(Nest017,2,3,3,SH) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest017,2,4,4,SH) { Species = 059, Ability = A3 }, // Arcanine
            new(Nest017,3,4,4,SH) { Species = 631, Ability = A4 }, // Heatmor
            new(Nest017,4,4,4,SH) { Species = 851, Ability = A4 }, // Centiskorch
            new(Nest017,4,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest018,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest018,0,1,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest018,1,2,2,SH) { Species = 608, Ability = A3 }, // Lampent
            new(Nest018,2,3,3,SH) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest018,2,4,4,SH) { Species = 059, Ability = A3 }, // Arcanine
            new(Nest018,2,4,4,SH) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest018,3,4,4,SH) { Species = 324, Ability = A4 }, // Torkoal
            new(Nest018,4,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest019,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest019,1,1,1,SH) { Species = 324, Ability = A3 }, // Torkoal
            new(Nest019,1,2,2,SH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest019,2,3,3,SH) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new(Nest019,2,4,4,SH) { Species = 059, Ability = A3 }, // Arcanine
            new(Nest019,4,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest022,0,1,1,SH) { Species = 225, Ability = A3 }, // Delibird
            new(Nest022,4,4,4,SH) { Species = 875, Ability = A4 }, // Eiscue
            new(Nest025,0,0,1,SH) { Species = 270, Ability = A3 }, // Lotad
            new(Nest025,1,1,2,SH) { Species = 271, Ability = A3 }, // Lombre
            new(Nest025,2,4,4,SH) { Species = 272, Ability = A3 }, // Ludicolo
            new(Nest026,4,4,4,SH) { Species = 842, Ability = A4 }, // Appletun
            new(Nest028,0,1,1,SH) { Species = 043, Ability = A3 }, // Oddish
            new(Nest028,1,1,1,SH) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest028,3,4,4,SH) { Species = 435, Ability = A4 }, // Skuntank
            new(Nest028,4,4,4,SH) { Species = 748, Ability = A4 }, // Toxapex
            new(Nest030,0,1,1,SH) { Species = 629, Ability = A3 }, // Vullaby
            new(Nest030,4,4,4,SH) { Species = 630, Ability = A4 }, // Mandibuzz
            new(Nest032,0,1,1,SH) { Species = 682, Ability = A3 }, // Spritzee
            new(Nest032,4,4,4,SH) { Species = 683, Ability = A4 }, // Aromatisse
            new(Nest033,4,4,4,SH) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new(Nest034,4,4,4,SH) { Species = 302, Ability = A4 }, // Sableye
            new(Nest035,1,1,2,SH) { Species = 629, Ability = A3 }, // Vullaby
            new(Nest035,3,4,4,SH) { Species = 630, Ability = A4 }, // Mandibuzz
            new(Nest035,4,4,4,SH) { Species = 248, Ability = A4 }, // Tyranitar
            new(Nest036,0,0,1,SH) { Species = 610, Ability = A3 }, // Axew
            new(Nest036,0,1,1,SH) { Species = 328, Ability = A3 }, // Trapinch
            new(Nest036,1,1,2,SH) { Species = 704, Ability = A3 }, // Goomy
            new(Nest036,2,3,3,SH) { Species = 611, Ability = A3 }, // Fraxure
            new(Nest036,2,4,4,SH) { Species = 705, Ability = A3 }, // Sliggoo
            new(Nest036,2,4,4,SH) { Species = 330, Ability = A3 }, // Flygon
            new(Nest036,3,4,4,SH) { Species = 612, Ability = A4 }, // Haxorus
            new(Nest036,4,4,4,SH) { Species = 780, Ability = A4 }, // Drampa
            new(Nest036,4,4,4,SH) { Species = 706, Ability = A4 }, // Goodra
          //new(Nest037,0,1,1,SH) { Species = 704, Ability = A3 }, // Goomy
            new(Nest037,2,4,4,SH) { Species = 705, Ability = A3 }, // Sliggoo
            new(Nest037,3,4,4,SH) { Species = 706, Ability = A4 }, // Goodra
            new(Nest037,4,4,4,SH) { Species = 842, Ability = A4 }, // Appletun
            new(Nest039,1,1,2,SH) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest039,4,4,4,SH) { Species = 765, Ability = A4 }, // Oranguru
          //new(Nest040,0,1,1,SH) { Species = 536, Ability = A3 }, // Palpitoad
          //new(Nest040,1,1,2,SH) { Species = 747, Ability = A3 }, // Mareanie
            new(Nest040,2,3,3,SH) { Species = 748, Ability = A3 }, // Toxapex
            new(Nest040,2,4,4,SH) { Species = 091, Ability = A3 }, // Cloyster
            new(Nest040,3,4,4,SH) { Species = 746, Ability = A4 }, // Wishiwashi
            new(Nest040,4,4,4,SH) { Species = 537, Ability = A4 }, // Seismitoad
            new(Nest042,1,3,3,SH) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new(Nest042,4,4,4,SH) { Species = 302, Ability = A3 }, // Sableye
            new(Nest042,3,4,4,SH) { Species = 867, Ability = A4 }, // Runerigus
            new(Nest042,3,4,4,SH) { Species = 864, Ability = A4 }, // Cursola
            new(Nest043,2,2,2,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest044,1,2,2,SH) { Species = 600, Ability = A3 }, // Klang
            new(Nest044,2,3,3,SH) { Species = 632, Ability = A3 }, // Durant
            new(Nest045,0,1,1,SH) { Species = 616, Ability = A3 }, // Shelmet
            new(Nest045,1,2,2,SH) { Species = 588, Ability = A3 }, // Karrablast
            new(Nest045,4,4,4,SH) { Species = 617, Ability = A4 }, // Accelgor
            new(Nest046,1,3,3,SH) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new(Nest046,2,4,4,SH) { Species = 302, Ability = A3 }, // Sableye
            new(Nest046,3,4,4,SH) { Species = 864, Ability = A4 }, // Cursola
            new(Nest047,0,1,1,SH) { Species = 453, Ability = A3 }, // Croagunk
            new(Nest047,2,4,4,SH) { Species = 454, Ability = A3 }, // Toxicroak
            new(Nest047,3,4,4,SH) { Species = 701, Ability = A4 }, // Hawlucha
            new(Nest050,0,1,1,SH) { Species = 524, Ability = A3 }, // Roggenrola
            new(Nest050,1,2,2,SH) { Species = 246, Ability = A3 }, // Larvitar
            new(Nest050,2,3,3,SH) { Species = 247, Ability = A3 }, // Pupitar
            new(Nest050,4,4,4,SH) { Species = 248, Ability = A4 }, // Tyranitar
            new(Nest052,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest052,1,2,2,SH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest052,3,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest053,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest053,1,2,2,SH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest053,2,3,3,SH) { Species = 608, Ability = A3 }, // Lampent
            new(Nest053,3,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest053,4,4,4,SH) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new(Nest054,0,0,1,SH) { Species = 613, Ability = A3 }, // Cubchoo
            new(Nest054,4,4,4,SH) { Species = 875, Ability = A4 }, // Eiscue
            new(Nest057,0,0,1,SH) { Species = 270, Ability = A3 }, // Lotad
            new(Nest057,1,2,2,SH) { Species = 271, Ability = A3 }, // Lombre
            new(Nest057,2,4,4,SH) { Species = 272, Ability = A3 }, // Ludicolo
            new(Nest057,4,4,4,SH) { Species = 842, Ability = A4 }, // Appletun
            new(Nest058,0,0,1,SH) { Species = 270, Ability = A3 }, // Lotad
            new(Nest058,1,2,2,SH) { Species = 271, Ability = A3 }, // Lombre
            new(Nest058,4,4,4,SH) { Species = 272, Ability = A4 }, // Ludicolo
            new(Nest059,1,2,2,SH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest059,4,4,4,SH) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new(Nest061,2,4,4,SH) { Species = 078, Ability = A3, Form = 1 }, // Rapidash-1
            new(Nest062,0,1,1,SH) { Species = 629, Ability = A3 }, // Vullaby
            new(Nest062,3,4,4,SH) { Species = 630, Ability = A4 }, // Mandibuzz
            new(Nest062,4,4,4,SH) { Species = 248, Ability = A4 }, // Tyranitar
            new(Nest063,0,0,1,SH) { Species = 610, Ability = A3 }, // Axew
            new(Nest063,0,1,1,SH) { Species = 328, Ability = A3 }, // Trapinch
            new(Nest063,0,1,1,SH) { Species = 704, Ability = A3 }, // Goomy
            new(Nest063,1,2,2,SH) { Species = 329, Ability = A3 }, // Vibrava
            new(Nest063,2,4,4,SH) { Species = 705, Ability = A3 }, // Sliggoo
            new(Nest063,2,4,4,SH) { Species = 780, Ability = A3 }, // Drampa
            new(Nest063,3,4,4,SH) { Species = 706, Ability = A4 }, // Goodra
            new(Nest063,4,4,4,SH) { Species = 330, Ability = A4 }, // Flygon
            new(Nest064,3,4,4,SH) { Species = 765, Ability = A4 }, // Oranguru
            new(Nest064,3,4,4,SH) { Species = 876, Ability = A4, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest066,2,2,2,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest070,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest070,0,1,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest070,1,2,2,SH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest070,2,3,3,SH) { Species = 608, Ability = A3 }, // Lampent
          //new(Nest073,0,0,1,SH) { Species = 682, Ability = A3 }, // Spritzee
            new(Nest073,2,4,4,SH) { Species = 683, Ability = A3 }, // Aromatisse
            new(Nest075,2,4,4,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
          //new(Nest076,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest076,2,2,2,SH) { Species = 631, Ability = A3 }, // Heatmor
            new(Nest076,3,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest077,1,2,2,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest078,0,0,1,SH) { Species = 270, Ability = A3 }, // Lotad
            new(Nest078,1,2,2,SH) { Species = 271, Ability = A3 }, // Lombre
            new(Nest078,2,4,4,SH) { Species = 272, Ability = A3 }, // Ludicolo
            new(Nest078,4,4,4,SH) { Species = 842, Ability = A4, CanGigantamax = true }, // Appletun
            new(Nest079,0,0,1,SH) { Species = 058, Ability = A3 }, // Growlithe
            new(Nest079,3,4,4,SH) { Species = 059, Ability = A4 }, // Arcanine
            new(Nest080,0,0,1,SH) { Species = 679, Ability = A3 }, // Honedge
            new(Nest080,0,0,1,SH) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new(Nest080,0,1,1,SH) { Species = 854, Ability = A3 }, // Sinistea
            new(Nest080,0,1,1,SH) { Species = 092, Ability = A3 }, // Gastly
            new(Nest080,1,2,2,SH) { Species = 680, Ability = A3 }, // Doublade
            new(Nest080,1,3,3,SH) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new(Nest080,2,3,3,SH) { Species = 093, Ability = A3 }, // Haunter
            new(Nest080,2,4,4,SH) { Species = 302, Ability = A3 }, // Sableye
            new(Nest080,3,4,4,SH) { Species = 855, Ability = A4 }, // Polteageist
            new(Nest080,3,4,4,SH) { Species = 864, Ability = A4 }, // Cursola
            new(Nest080,4,4,4,SH) { Species = 867, Ability = A4 }, // Runerigus
            new(Nest080,4,4,4,SH) { Species = 094, Ability = A4, CanGigantamax = true }, // Gengar
            new(Nest081,0,0,1,SH) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new(Nest081,2,4,4,SH) { Species = 078, Ability = A3, Form = 1 }, // Rapidash-1
            new(Nest082,0,0,1,SH) { Species = 582, Ability = A3 }, // Vanillite
            new(Nest082,0,0,1,SH) { Species = 613, Ability = A3 }, // Cubchoo
            new(Nest082,0,1,1,SH) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new(Nest082,0,1,1,SH) { Species = 712, Ability = A3 }, // Bergmite
            new(Nest082,1,2,2,SH) { Species = 361, Ability = A3 }, // Snorunt
            new(Nest082,1,2,2,SH) { Species = 225, Ability = A3 }, // Delibird
            new(Nest082,2,3,3,SH) { Species = 713, Ability = A3 }, // Avalugg
            new(Nest082,2,4,4,SH) { Species = 362, Ability = A3 }, // Glalie
            new(Nest082,3,4,4,SH) { Species = 584, Ability = A4 }, // Vanilluxe
            new(Nest082,3,4,4,SH) { Species = 866, Ability = A4 }, // Mr. Rime
            new(Nest082,4,4,4,SH) { Species = 875, Ability = A4 }, // Eiscue
            new(Nest082,4,4,4,SH) { Species = 131, Ability = A4, CanGigantamax = true }, // Lapras
            new(Nest083,1,2,2,SH) { Species = 600, Ability = A3 }, // Klang
            new(Nest083,2,3,3,SH) { Species = 632, Ability = A3 }, // Durant
            new(Nest085,1,2,2,SH) { Species = 757, Ability = A3 }, // Salandit
            new(Nest085,4,4,4,SH) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new(Nest086,0,0,1,SH) { Species = 682, Ability = A3 }, // Spritzee
            new(Nest086,2,3,3,SH) { Species = 683, Ability = A3 }, // Aromatisse
            new(Nest087,0,1,1,SH) { Species = 629, Ability = A3 }, // Vullaby
            new(Nest087,3,4,4,SH) { Species = 630, Ability = A4 }, // Mandibuzz
            new(Nest087,4,4,4,SH) { Species = 248, Ability = A4 }, // Tyranitar
            new(Nest089,0,1,1,SH) { Species = 616, Ability = A3 }, // Shelmet
            new(Nest089,1,2,2,SH) { Species = 588, Ability = A3 }, // Karrablast
            new(Nest089,4,4,4,SH) { Species = 617, Ability = A4 }, // Accelgor
            new(Nest090,1,2,2,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest091,0,1,1,SH) { Species = 616, Ability = A3 }, // Shelmet
            new(Nest091,1,2,2,SH) { Species = 588, Ability = A3 }, // Karrablast
            new(Nest091,4,4,4,SH) { Species = 617, Ability = A4 }, // Accelgor
            new(Nest106,1,2,2,SH) { Species = 629, Ability = A3 }, // Vullaby
            new(Nest106,3,4,4,SH) { Species = 630, Ability = A4 }, // Mandibuzz
            new(Nest106,4,4,4,SH) { Species = 630, Ability = A4 }, // Mandibuzz
            new(Nest107,1,2,2,SH) { Species = 629, Ability = A2 }, // Vullaby
            new(Nest107,4,4,4,SH) { Species = 630, Ability = A2 }, // Mandibuzz
            new(Nest108,0,1,1,SH) { Species = 214, Ability = A3 }, // Heracross
            new(Nest108,1,2,2,SH) { Species = 214, Ability = A3 }, // Heracross
            new(Nest108,2,3,3,SH) { Species = 214, Ability = A3 }, // Heracross
            new(Nest108,3,4,4,SH) { Species = 214, Ability = A4 }, // Heracross
            new(Nest109,0,1,1,SH) { Species = 214, Ability = A2 }, // Heracross
            new(Nest109,4,4,4,SH) { Species = 214, Ability = A2 }, // Heracross
            new(Nest113,4,4,4,SH) { Species = 059, Ability = A2 }, // Arcanine
            new(Nest114,4,4,4,SH) { Species = 745, Ability = A4, Form = 1 }, // Lycanroc-1
            new(Nest115,3,4,4,SH) { Species = 745, Ability = A2, Form = 1 }, // Lycanroc-1
            new(Nest116,2,3,3,SH) { Species = 765, Ability = A3 }, // Oranguru
            new(Nest117,2,3,3,SH) { Species = 765, Ability = A2 }, // Oranguru
            new(Nest117,3,4,4,SH) { Species = 876, Ability = A2, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest117,4,4,4,SH) { Species = 678, Ability = A2, Gender = 1, Form = 1 }, // Meowstic-1
            new(Nest122,1,2,2,SH) { Species = 453, Ability = A3 }, // Croagunk
            new(Nest122,2,3,3,SH) { Species = 853, Ability = A3 }, // Grapploct
            new(Nest122,2,3,3,SH) { Species = 454, Ability = A3 }, // Toxicroak
            new(Nest122,3,4,4,SH) { Species = 454, Ability = A4 }, // Toxicroak
            new(Nest123,0,1,1,SH) { Species = 453, Ability = A2 }, // Croagunk
            new(Nest123,1,2,2,SH) { Species = 538, Ability = A2 }, // Throh
            new(Nest123,2,3,3,SH) { Species = 620, Ability = A2 }, // Mienshao
            new(Nest123,3,4,4,SH) { Species = 538, Ability = A2 }, // Throh
            new(Nest123,3,4,4,SH) { Species = 454, Ability = A2 }, // Toxicroak
            new(Nest123,4,4,4,SH) { Species = 870, Ability = A2 }, // Falinks
            new(Nest127,4,4,4,SH) { Species = 864, Ability = A2 }, // Cursola
          //new(Nest130,0,1,1,SH) { Species = 704, Ability = A3 }, // Goomy
            new(Nest130,2,3,3,SH) { Species = 705, Ability = A3 }, // Sliggoo
            new(Nest130,3,4,4,SH) { Species = 842, Ability = A4 }, // Appletun
            new(Nest130,4,4,4,SH) { Species = 706, Ability = A4 }, // Goodra
            new(Nest131,2,2,2,SH) { Species = 780, Ability = A2 }, // Drampa
            new(Nest131,2,2,2,SH) { Species = 704, Ability = A2 }, // Goomy
            new(Nest131,2,3,3,SH) { Species = 705, Ability = A2 }, // Sliggoo
            new(Nest131,3,4,4,SH) { Species = 780, Ability = A2 }, // Drampa
            new(Nest131,4,4,4,SH) { Species = 706, Ability = A2 }, // Goodra
            new(Nest135,4,4,4,SH) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
          //new(Nest138,0,1,1,SH) { Species = 690, Ability = A3 }, // Skrelp
            new(Nest138,2,2,2,SH) { Species = 690, Ability = A3 }, // Skrelp
            new(Nest138,4,4,4,SH) { Species = 691, Ability = A4 }, // Dragalge
          //new(Nest139,0,1,1,SH) { Species = 690, Ability = A2 }, // Skrelp
            new(Nest139,3,4,4,SH) { Species = 691, Ability = A2 }, // Dragalge
            new(Nest139,4,4,4,SH) { Species = 691, Ability = A2 }, // Dragalge
            new(Nest147,1,2,2,SH) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
            new(Nest147,3,4,4,SH) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
            new(Nest148,1,2,2,SH) { Species = 214, Ability = A2 }, // Heracross
            new(Nest150,2,3,3,SH) { Species = 842, Ability = A2 }, // Appletun
            new(Nest150,3,4,4,SH) { Species = 842, Ability = A2 }, // Appletun
            new(Nest150,4,4,4,SH) { Species = 842, Ability = A2, CanGigantamax = true }, // Appletun
            new(Nest155,4,4,4,SH) { Species = 745, Ability = A4, Form = 1 }, // Lycanroc-1
            new(Nest160,3,4,5,SH) { Species = 059, Ability = A2 }, // Arcanine
            new(Nest161,0,1,1,SH) { Species = 140, Ability = A3 }, // Kabuto
            new(Nest161,1,2,2,SH) { Species = 140, Ability = A3 }, // Kabuto
            new(Nest161,2,3,3,SH) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new(Nest161,3,4,4,SH) { Species = 141, Ability = A4 }, // Kabutops
            new(Nest161,3,4,4,SH) { Species = 550, Ability = A4, Form = 1 }, // Basculin-1
            new(Nest162,0,1,2,SH) { Species = 140, Ability = A2 }, // Kabuto
            new(Nest162,2,3,4,SH) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
            new(Nest162,3,4,5,SH) { Species = 141, Ability = A2 }, // Kabutops
            new(Nest164,3,4,5,SH) { Species = 849, Ability = A2, Form = 1 }, // Toxtricity-1
            new(Nest164,4,4,5,SH) { Species = 849, Ability = A2, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(Nest165,0,1,1,SH) { Species = 345, Ability = A3 }, // Lileep
            new(Nest165,2,3,3,SH) { Species = 345, Ability = A3 }, // Lileep
            new(Nest165,3,4,4,SH) { Species = 346, Ability = A4 }, // Cradily
            new(Nest165,4,4,4,SH) { Species = 348, Ability = A4 }, // Armaldo
            new(Nest166,4,4,5,SH) { Species = 346, Ability = A2 }, // Cradily
            new(Nest167,0,1,1,SH) { Species = 220, Ability = A3 }, // Swinub
          //new(Nest169,0,1,1,SH) { Species = 875, Ability = A3 }, // Eiscue
            new(Nest169,2,2,2,SH) { Species = 875, Ability = A3 }, // Eiscue
            new(Nest170,4,4,5,SH) { Species = 131, Ability = A2, CanGigantamax = true }, // Lapras
            new(Nest172,3,4,5,SH) { Species = 107, Ability = A2 }, // Hitmonchan
            new(Nest172,4,4,5,SH) { Species = 068, Ability = A2 }, // Machamp
            new(Nest173,0,1,1,SH) { Species = 032, Ability = A3 }, // Nidoran♂
            new(Nest173,3,4,4,SH) { Species = 034, Ability = A4 }, // Nidoking
            new(Nest174,1,2,3,SH) { Species = 033, Ability = A2 }, // Nidorino
            new(Nest177,0,1,1,SH) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new(Nest177,2,3,3,SH) { Species = 578, Ability = A3 }, // Duosion
            new(Nest177,3,4,4,SH) { Species = 876, Ability = A4, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest177,3,4,4,SH) { Species = 579, Ability = A4 }, // Reuniclus
            new(Nest177,3,4,4,SH) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new(Nest178,0,1,2,SH) { Species = 876, Ability = A2, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest178,0,1,2,SH) { Species = 577, Ability = A2 }, // Solosis
            new(Nest178,2,3,4,SH) { Species = 876, Ability = A2, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest178,3,4,5,SH) { Species = 876, Ability = A2, Gender = 1, Form = 1 }, // Indeedee-1
            new(Nest178,3,4,5,SH) { Species = 579, Ability = A2 }, // Reuniclus
            new(Nest178,4,4,5,SH) { Species = 579, Ability = A2 }, // Reuniclus
            new(Nest179,0,1,1,SH) { Species = 837, Ability = A3 }, // Rolycoly
            new(Nest179,1,2,2,SH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest179,2,3,3,SH) { Species = 838, Ability = A3 }, // Carkol
            new(Nest179,3,4,4,SH) { Species = 302, Ability = A4 }, // Sableye
            new(Nest180,0,1,2,SH) { Species = 302, Ability = A2 }, // Sableye
            new(Nest180,2,3,4,SH) { Species = 302, Ability = A2 }, // Sableye
            new(Nest180,3,4,5,SH) { Species = 302, Ability = A2 }, // Sableye
            new(Nest180,4,4,5,SH) { Species = 839, Ability = A2 }, // Coalossal
            new(Nest181,3,4,4,SH) { Species = 302, Ability = A4 }, // Sableye
            new(Nest182,1,2,3,SH) { Species = 222, Ability = A2, Form = 1 }, // Corsola-1
            new(Nest182,2,3,4,SH) { Species = 302, Ability = A2 }, // Sableye
            new(Nest182,3,4,5,SH) { Species = 864, Ability = A2 }, // Cursola
            new(Nest182,3,4,5,SH) { Species = 302, Ability = A2 }, // Sableye
            new(Nest182,4,4,5,SH) { Species = 094, Ability = A2, CanGigantamax = true }, // Gengar
            new(Nest183,0,1,1,SH) { Species = 443, Ability = A3 }, // Gible
            new(Nest183,1,2,2,SH) { Species = 443, Ability = A3 }, // Gible
            new(Nest183,2,3,3,SH) { Species = 444, Ability = A3 }, // Gabite
            new(Nest183,3,4,4,SH) { Species = 444, Ability = A4 }, // Gabite
            new(Nest183,4,4,4,SH) { Species = 445, Ability = A4 }, // Garchomp
            new(Nest184,0,1,2,SH) { Species = 443, Ability = A2 }, // Gible
            new(Nest184,1,2,3,SH) { Species = 780, Ability = A2 }, // Drampa
            new(Nest184,2,3,4,SH) { Species = 444, Ability = A2 }, // Gabite
            new(Nest184,3,4,5,SH) { Species = 445, Ability = A2 }, // Garchomp
            new(Nest184,4,4,5,SH) { Species = 445, Ability = A2 }, // Garchomp
            new(Nest192,3,4,5,SH) { Species = 869, Ability = A2 }, // Alcremie
            new(Nest195,0,1,1,SH) { Species = 140, Ability = A3 }, // Kabuto
            new(Nest195,0,1,1,SH) { Species = 345, Ability = A3 }, // Lileep
            new(Nest195,3,4,4,SH) { Species = 141, Ability = A4 }, // Kabutops
            new(Nest195,3,4,4,SH) { Species = 346, Ability = A4 }, // Cradily
            new(Nest195,3,4,4,SH) { Species = 699, Ability = A4 }, // Aurorus
            new(Nest195,4,4,4,SH) { Species = 697, Ability = A4 }, // Tyrantrum
            new(Nest196,0,1,2,SH) { Species = 140, Ability = A2 }, // Kabuto
            new(Nest196,2,2,2,SH) { Species = 141, Ability = A2 }, // Kabutops
            new(Nest196,2,2,3,SH) { Species = 883, Ability = A2 }, // Arctovish
            new(Nest196,2,3,4,SH) { Species = 882, Ability = A2 }, // Dracovish
            new(Nest196,3,3,4,SH) { Species = 880, Ability = A2 }, // Dracozolt
        };
        #endregion
    }
}
