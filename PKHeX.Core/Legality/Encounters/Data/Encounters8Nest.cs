using System;
using System.Collections.Generic;

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
        };

        // Abilities Allowed
        private const int A2 = 4; // Ability 4 only
        private const int A3 = 0; // 1/2 only
        private const int A4 = -1; // 1/2/H

        internal const int SharedNest = 162;
        internal const int Watchtower = 126;

        internal static readonly EncounterStatic8N[] Nest_Common =
        {
            new EncounterStatic8N(Nest000,0,0,1) { Species = 236, Ability = A3 }, // Tyrogue
            new EncounterStatic8N(Nest000,0,0,1) { Species = 066, Ability = A3 }, // Machop
            new EncounterStatic8N(Nest000,0,1,1) { Species = 532, Ability = A3 }, // Timburr
            new EncounterStatic8N(Nest000,1,2,2) { Species = 067, Ability = A3 }, // Machoke
            new EncounterStatic8N(Nest000,1,2,2) { Species = 533, Ability = A3 }, // Gurdurr
            new EncounterStatic8N(Nest000,4,4,4) { Species = 068, Ability = A4 }, // Machamp
            new EncounterStatic8N(Nest001,0,0,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest001,0,0,1) { Species = 517, Ability = A3 }, // Munna
            new EncounterStatic8N(Nest001,0,1,1) { Species = 677, Ability = A3 }, // Espurr
            new EncounterStatic8N(Nest001,0,1,1) { Species = 605, Ability = A3 }, // Elgyem
            new EncounterStatic8N(Nest001,1,2,2) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest001,2,4,4) { Species = 518, Ability = A3 }, // Musharna
            new EncounterStatic8N(Nest001,4,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest002,0,0,1) { Species = 438, Ability = A3 }, // Bonsly
            new EncounterStatic8N(Nest002,0,1,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest002,1,2,2) { Species = 111, Ability = A3 }, // Rhyhorn
            new EncounterStatic8N(Nest002,1,2,2) { Species = 525, Ability = A3 }, // Boldore
            new EncounterStatic8N(Nest002,2,3,3) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest002,2,4,4) { Species = 112, Ability = A3 }, // Rhydon
            new EncounterStatic8N(Nest002,2,4,4) { Species = 185, Ability = A3 }, // Sudowoodo
            new EncounterStatic8N(Nest002,4,4,4) { Species = 213, Ability = A4 }, // Shuckle
            new EncounterStatic8N(Nest003,0,0,1) { Species = 010, Ability = A3 }, // Caterpie
            new EncounterStatic8N(Nest003,0,0,1) { Species = 736, Ability = A3 }, // Grubbin
            new EncounterStatic8N(Nest003,0,1,1) { Species = 290, Ability = A3 }, // Nincada
            new EncounterStatic8N(Nest003,0,1,1) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest003,1,2,2) { Species = 011, Ability = A3 }, // Metapod
            new EncounterStatic8N(Nest003,1,2,2) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest003,2,3,3) { Species = 737, Ability = A3 }, // Charjabug
            new EncounterStatic8N(Nest003,2,4,4) { Species = 291, Ability = A3 }, // Ninjask
            new EncounterStatic8N(Nest003,2,4,4) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest003,3,4,4) { Species = 596, Ability = A4 }, // Galvantula
            new EncounterStatic8N(Nest003,4,4,4) { Species = 738, Ability = A4 }, // Vikavolt
            new EncounterStatic8N(Nest003,4,4,4) { Species = 632, Ability = A4 }, // Durant
            new EncounterStatic8N(Nest004,0,0,1) { Species = 010, Ability = A3 }, // Caterpie
            new EncounterStatic8N(Nest004,0,0,1) { Species = 415, Ability = A3 }, // Combee
            new EncounterStatic8N(Nest004,0,1,1) { Species = 742, Ability = A3 }, // Cutiefly
            new EncounterStatic8N(Nest004,0,1,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest004,1,2,2) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest004,1,2,2) { Species = 011, Ability = A3 }, // Metapod
            new EncounterStatic8N(Nest004,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest004,2,4,4) { Species = 596, Ability = A3 }, // Galvantula
            new EncounterStatic8N(Nest004,2,4,4) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest004,3,4,4) { Species = 743, Ability = A4 }, // Ribombee
            new EncounterStatic8N(Nest004,4,4,4) { Species = 416, Ability = A4 }, // Vespiquen
            new EncounterStatic8N(Nest004,4,4,4) { Species = 826, Ability = A4 }, // Orbeetle
            new EncounterStatic8N(Nest005,0,0,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest005,0,0,1) { Species = 355, Ability = A3 }, // Duskull
            new EncounterStatic8N(Nest005,0,1,1) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest005,0,1,1) { Species = 708, Ability = A3 }, // Phantump
            new EncounterStatic8N(Nest005,0,1,1) { Species = 592, Ability = A3 }, // Frillish
            new EncounterStatic8N(Nest005,1,2,2) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest005,2,3,3) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest005,2,4,4) { Species = 356, Ability = A3 }, // Dusclops
            new EncounterStatic8N(Nest005,2,4,4) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest005,3,4,4) { Species = 709, Ability = A4 }, // Trevenant
            new EncounterStatic8N(Nest005,4,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest005,4,4,4) { Species = 593, Ability = A4 }, // Jellicent
            new EncounterStatic8N(Nest006,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest006,0,0,1) { Species = 458, Ability = A3 }, // Mantyke
            new EncounterStatic8N(Nest006,1,2,2) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest006,2,3,3) { Species = 224, Ability = A3 }, // Octillery
            new EncounterStatic8N(Nest006,2,4,4) { Species = 226, Ability = A3 }, // Mantine
            new EncounterStatic8N(Nest006,2,4,4) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest006,3,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest006,4,4,4) { Species = 746, Ability = A4 }, // Wishiwashi
            new EncounterStatic8N(Nest006,4,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest007,0,0,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest007,0,0,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest007,0,1,1) { Species = 422, Ability = A3, Form = 1 }, // Shellos-1
            new EncounterStatic8N(Nest007,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest007,1,2,2) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest007,2,3,3) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest007,2,4,4) { Species = 834, Ability = A3 }, // Drednaw
            new EncounterStatic8N(Nest007,2,4,4) { Species = 847, Ability = A3 }, // Barraskewda
            new EncounterStatic8N(Nest007,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest007,4,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest007,4,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest008,0,0,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest008,0,0,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest008,0,1,1) { Species = 535, Ability = A3 }, // Tympole
            new EncounterStatic8N(Nest008,0,1,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest008,1,2,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest008,2,3,3) { Species = 834, Ability = A3 }, // Drednaw
            new EncounterStatic8N(Nest008,2,4,4) { Species = 195, Ability = A3 }, // Quagsire
            new EncounterStatic8N(Nest008,2,4,4) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest008,3,4,4) { Species = 091, Ability = A4 }, // Cloyster
            new EncounterStatic8N(Nest008,4,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest008,4,4,4) { Species = 342, Ability = A4 }, // Crawdaunt
            new EncounterStatic8N(Nest009,0,0,1) { Species = 236, Ability = A3 }, // Tyrogue
            new EncounterStatic8N(Nest009,0,0,1) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest009,0,1,1) { Species = 852, Ability = A3 }, // Clobbopus
            new EncounterStatic8N(Nest009,0,1,1) { Species = 674, Ability = A3 }, // Pancham
            new EncounterStatic8N(Nest009,2,4,4) { Species = 760, Ability = A3 }, // Bewear
            new EncounterStatic8N(Nest009,2,4,4) { Species = 675, Ability = A3 }, // Pangoro
            new EncounterStatic8N(Nest009,2,4,4) { Species = 701, Ability = A3 }, // Hawlucha
            new EncounterStatic8N(Nest009,4,4,4) { Species = 853, Ability = A4 }, // Grapploct
            new EncounterStatic8N(Nest009,4,4,4) { Species = 870, Ability = A4 }, // Falinks
            new EncounterStatic8N(Nest010,0,0,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest010,0,0,1) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest010,0,1,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest010,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest010,1,1,2) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest010,1,2,2) { Species = 878, Ability = A3 }, // Cufant
            new EncounterStatic8N(Nest010,2,4,4) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest010,2,4,4) { Species = 863, Ability = A3 }, // Perrserker
            new EncounterStatic8N(Nest010,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest010,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest010,4,4,4) { Species = 601, Ability = A4 }, // Klinklang
            new EncounterStatic8N(Nest010,4,4,4) { Species = 879, Ability = A4 }, // Copperajah
            new EncounterStatic8N(Nest011,0,0,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest011,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest011,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest011,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest011,1,1,2) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest011,1,2,2) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest011,2,4,4) { Species = 208, Ability = A3 }, // Steelix
            new EncounterStatic8N(Nest011,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest011,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest011,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest011,4,4,4) { Species = 777, Ability = A4 }, // Togedemaru
            new EncounterStatic8N(Nest012,0,0,1) { Species = 439, Ability = A3 }, // Mime Jr.
            new EncounterStatic8N(Nest012,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest012,2,3,3) { Species = 561, Ability = A3 }, // Sigilyph
            new EncounterStatic8N(Nest012,2,3,3) { Species = 178, Ability = A3 }, // Xatu
            new EncounterStatic8N(Nest012,4,4,4) { Species = 858, Ability = A4 }, // Hatterene
            new EncounterStatic8N(Nest013,0,0,1) { Species = 439, Ability = A3 }, // Mime Jr.
            new EncounterStatic8N(Nest013,0,0,1) { Species = 360, Ability = A3 }, // Wynaut
            new EncounterStatic8N(Nest013,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest013,0,1,1) { Species = 343, Ability = A3 }, // Baltoy
            new EncounterStatic8N(Nest013,1,1,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest013,1,3,3) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest013,2,3,3) { Species = 561, Ability = A3 }, // Sigilyph
            new EncounterStatic8N(Nest013,2,3,3) { Species = 178, Ability = A3 }, // Xatu
            new EncounterStatic8N(Nest013,3,4,4) { Species = 344, Ability = A4 }, // Claydol
            new EncounterStatic8N(Nest013,4,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest013,4,4,4) { Species = 202, Ability = A4 }, // Wobbuffet
            new EncounterStatic8N(Nest014,0,0,1) { Species = 837, Ability = A3 }, // Rolycoly
            new EncounterStatic8N(Nest014,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest014,1,1,1) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest014,1,2,2) { Species = 525, Ability = A3 }, // Boldore
            new EncounterStatic8N(Nest014,2,3,3) { Species = 558, Ability = A3 }, // Crustle
            new EncounterStatic8N(Nest014,2,4,4) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest014,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest015,0,0,1) { Species = 050, Ability = A3 }, // Diglett
            new EncounterStatic8N(Nest015,0,0,1) { Species = 749, Ability = A3 }, // Mudbray
            new EncounterStatic8N(Nest015,0,1,1) { Species = 290, Ability = A3 }, // Nincada
            new EncounterStatic8N(Nest015,0,1,1) { Species = 529, Ability = A3 }, // Drilbur
            new EncounterStatic8N(Nest015,1,1,1) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest015,1,2,2) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest015,2,3,3) { Species = 208, Ability = A3 }, // Steelix
            new EncounterStatic8N(Nest015,2,4,4) { Species = 340, Ability = A3 }, // Whiscash
            new EncounterStatic8N(Nest015,2,4,4) { Species = 660, Ability = A3 }, // Diggersby
            new EncounterStatic8N(Nest015,3,4,4) { Species = 051, Ability = A4 }, // Dugtrio
            new EncounterStatic8N(Nest015,4,4,4) { Species = 530, Ability = A4 }, // Excadrill
            new EncounterStatic8N(Nest015,4,4,4) { Species = 750, Ability = A4 }, // Mudsdale
            new EncounterStatic8N(Nest016,0,0,1) { Species = 843, Ability = A3 }, // Silicobra
            new EncounterStatic8N(Nest016,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest016,0,1,1) { Species = 449, Ability = A3 }, // Hippopotas
            new EncounterStatic8N(Nest016,1,2,2) { Species = 221, Ability = A3 }, // Piloswine
            new EncounterStatic8N(Nest016,4,4,4) { Species = 867, Ability = A3 }, // Runerigus
            new EncounterStatic8N(Nest016,4,4,4) { Species = 844, Ability = A4 }, // Sandaconda
            new EncounterStatic8N(Nest017,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest017,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest017,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest017,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest018,0,0,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest018,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest018,1,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest018,4,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest019,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest019,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest019,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest019,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest019,3,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest019,4,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest020,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest020,0,0,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest020,0,1,1) { Species = 459, Ability = A3 }, // Snover
            new EncounterStatic8N(Nest020,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest020,1,1,1) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest020,1,2,2) { Species = 583, Ability = A3 }, // Vanillish
            new EncounterStatic8N(Nest020,2,3,3) { Species = 221, Ability = A3 }, // Piloswine
            new EncounterStatic8N(Nest020,2,4,4) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest020,2,4,4) { Species = 460, Ability = A3 }, // Abomasnow
            new EncounterStatic8N(Nest020,3,4,4) { Species = 091, Ability = A4 }, // Cloyster
            new EncounterStatic8N(Nest020,4,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest020,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest021,0,0,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest021,0,0,1) { Species = 613, Ability = A3 }, // Cubchoo
            new EncounterStatic8N(Nest021,0,1,1) { Species = 872, Ability = A3 }, // Snom
            new EncounterStatic8N(Nest021,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest021,1,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest021,1,2,2) { Species = 221, Ability = A3 }, // Piloswine
            new EncounterStatic8N(Nest021,2,3,3) { Species = 091, Ability = A3 }, // Cloyster
            new EncounterStatic8N(Nest021,2,4,4) { Species = 614, Ability = A3 }, // Beartic
            new EncounterStatic8N(Nest021,2,4,4) { Species = 866, Ability = A3 }, // Mr. Rime
            new EncounterStatic8N(Nest021,3,4,4) { Species = 473, Ability = A4 }, // Mamoswine
            new EncounterStatic8N(Nest021,4,4,4) { Species = 873, Ability = A4 }, // Frosmoth
            new EncounterStatic8N(Nest021,4,4,4) { Species = 461, Ability = A4 }, // Weavile
            new EncounterStatic8N(Nest022,0,0,1) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest022,0,0,1) { Species = 872, Ability = A3 }, // Snom
            new EncounterStatic8N(Nest022,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest022,1,1,2) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest022,1,2,3) { Species = 459, Ability = A3 }, // Snover
            new EncounterStatic8N(Nest022,2,3,3) { Species = 460, Ability = A3 }, // Abomasnow
            new EncounterStatic8N(Nest022,2,4,4) { Species = 362, Ability = A3 }, // Glalie
            new EncounterStatic8N(Nest022,2,4,4) { Species = 866, Ability = A3 }, // Mr. Rime
            new EncounterStatic8N(Nest022,3,4,4) { Species = 873, Ability = A4 }, // Frosmoth
            new EncounterStatic8N(Nest022,4,4,4) { Species = 478, Ability = A4 }, // Froslass
            new EncounterStatic8N(Nest023,0,0,1) { Species = 172, Ability = A3 }, // Pichu
            new EncounterStatic8N(Nest023,0,0,1) { Species = 309, Ability = A3 }, // Electrike
            new EncounterStatic8N(Nest023,0,1,1) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest023,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest023,1,1,2) { Species = 737, Ability = A3 }, // Charjabug
            new EncounterStatic8N(Nest023,1,2,3) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest023,2,3,3) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest023,2,4,4) { Species = 310, Ability = A3 }, // Manectric
            new EncounterStatic8N(Nest023,2,4,4) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest023,3,4,4) { Species = 596, Ability = A4 }, // Galvantula
            new EncounterStatic8N(Nest023,4,4,4) { Species = 738, Ability = A4 }, // Vikavolt
            new EncounterStatic8N(Nest023,4,4,4) { Species = 026, Ability = A4 }, // Raichu
            new EncounterStatic8N(Nest024,0,0,1) { Species = 835, Ability = A3 }, // Yamper
            new EncounterStatic8N(Nest024,0,0,1) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest024,0,1,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest024,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest024,1,1,2) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest024,1,2,3) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest024,2,3,3) { Species = 836, Ability = A3 }, // Boltund
            new EncounterStatic8N(Nest024,2,4,4) { Species = 695, Ability = A3 }, // Heliolisk
            new EncounterStatic8N(Nest024,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest024,3,4,4) { Species = 871, Ability = A4 }, // Pincurchin
            new EncounterStatic8N(Nest024,4,4,4) { Species = 777, Ability = A4 }, // Togedemaru
            new EncounterStatic8N(Nest024,4,4,4) { Species = 877, Ability = A4 }, // Morpeko
            new EncounterStatic8N(Nest025,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest025,0,1,1) { Species = 761, Ability = A3 }, // Bounsweet
            new EncounterStatic8N(Nest025,0,1,1) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest025,1,2,3) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest025,2,3,3) { Species = 044, Ability = A3 }, // Gloom
            new EncounterStatic8N(Nest025,2,4,4) { Species = 762, Ability = A3 }, // Steenee
            new EncounterStatic8N(Nest025,3,4,4) { Species = 763, Ability = A4 }, // Tsareena
            new EncounterStatic8N(Nest025,4,4,4) { Species = 045, Ability = A4 }, // Vileplume
            new EncounterStatic8N(Nest025,4,4,4) { Species = 182, Ability = A4 }, // Bellossom
            new EncounterStatic8N(Nest026,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest026,0,0,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest026,0,1,1) { Species = 546, Ability = A3 }, // Cottonee
            new EncounterStatic8N(Nest026,0,1,1) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest026,1,1,2) { Species = 420, Ability = A3 }, // Cherubi
            new EncounterStatic8N(Nest026,1,2,2) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest026,2,3,3) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest026,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest026,2,4,4) { Species = 421, Ability = A3 }, // Cherrim
            new EncounterStatic8N(Nest026,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest026,4,4,4) { Species = 547, Ability = A4 }, // Whimsicott
            new EncounterStatic8N(Nest027,0,0,1) { Species = 710, Ability = A3, Form = 1 }, // Pumpkaboo-1
            new EncounterStatic8N(Nest027,0,0,1) { Species = 708, Ability = A3 }, // Phantump
            new EncounterStatic8N(Nest027,0,1,1) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest027,0,1,1) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest027,1,1,2) { Species = 710, Ability = A3, Form = 2 }, // Pumpkaboo-2
            new EncounterStatic8N(Nest027,1,2,2) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest027,2,3,3) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest027,2,4,4) { Species = 556, Ability = A3 }, // Maractus
            new EncounterStatic8N(Nest027,2,4,4) { Species = 709, Ability = A3 }, // Trevenant
            new EncounterStatic8N(Nest027,3,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest027,4,4,4) { Species = 781, Ability = A4 }, // Dhelmise
            new EncounterStatic8N(Nest027,4,4,4) { Species = 710, Ability = A4, Form = 3 }, // Pumpkaboo-3
            new EncounterStatic8N(Nest028,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest028,0,0,1) { Species = 568, Ability = A3 }, // Trubbish
            new EncounterStatic8N(Nest028,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest028,1,2,2) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest028,2,3,3) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest028,2,4,4) { Species = 452, Ability = A3 }, // Drapion
            new EncounterStatic8N(Nest028,2,4,4) { Species = 045, Ability = A3 }, // Vileplume
            new EncounterStatic8N(Nest028,4,4,4) { Species = 569, Ability = A4 }, // Garbodor
            new EncounterStatic8N(Nest029,0,0,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest029,0,0,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest029,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest029,0,1,1) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest029,1,1,2) { Species = 044, Ability = A3 }, // Gloom
            new EncounterStatic8N(Nest029,1,2,2) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest029,2,3,3) { Species = 109, Ability = A3 }, // Koffing
            new EncounterStatic8N(Nest029,2,4,4) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest029,2,4,4) { Species = 045, Ability = A3 }, // Vileplume
            new EncounterStatic8N(Nest029,3,4,4) { Species = 315, Ability = A4 }, // Roselia
            new EncounterStatic8N(Nest029,4,4,4) { Species = 849, Ability = A4 }, // Toxtricity
            new EncounterStatic8N(Nest029,4,4,4) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest030,0,0,1) { Species = 519, Ability = A3 }, // Pidove
            new EncounterStatic8N(Nest030,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest030,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest030,1,1,2) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest030,1,2,2) { Species = 520, Ability = A3 }, // Tranquill
            new EncounterStatic8N(Nest030,2,3,3) { Species = 521, Ability = A3 }, // Unfezant
            new EncounterStatic8N(Nest030,2,4,4) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest030,2,4,4) { Species = 528, Ability = A3 }, // Swoobat
            new EncounterStatic8N(Nest030,3,4,4) { Species = 178, Ability = A4 }, // Xatu
            new EncounterStatic8N(Nest030,4,4,4) { Species = 561, Ability = A4 }, // Sigilyph
            new EncounterStatic8N(Nest031,0,0,1) { Species = 821, Ability = A3 }, // Rookidee
            new EncounterStatic8N(Nest031,0,0,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest031,0,1,1) { Species = 278, Ability = A3 }, // Wingull
            new EncounterStatic8N(Nest031,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest031,1,1,2) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest031,1,2,2) { Species = 822, Ability = A3 }, // Corvisquire
            new EncounterStatic8N(Nest031,2,3,3) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest031,2,4,4) { Species = 279, Ability = A3 }, // Pelipper
            new EncounterStatic8N(Nest031,2,4,4) { Species = 178, Ability = A3 }, // Xatu
            new EncounterStatic8N(Nest031,3,4,4) { Species = 823, Ability = A4 }, // Corviknight
            new EncounterStatic8N(Nest031,4,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest031,4,4,4) { Species = 845, Ability = A4 }, // Cramorant
            new EncounterStatic8N(Nest032,0,0,1) { Species = 173, Ability = A3 }, // Cleffa
            new EncounterStatic8N(Nest032,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest032,0,1,1) { Species = 742, Ability = A3 }, // Cutiefly
            new EncounterStatic8N(Nest032,1,1,2) { Species = 035, Ability = A3 }, // Clefairy
            new EncounterStatic8N(Nest032,1,2,2) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest032,2,3,3) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest032,2,4,4) { Species = 036, Ability = A3 }, // Clefable
            new EncounterStatic8N(Nest032,2,4,4) { Species = 743, Ability = A3 }, // Ribombee
            new EncounterStatic8N(Nest032,3,4,4) { Species = 756, Ability = A4 }, // Shiinotic
            new EncounterStatic8N(Nest032,4,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest033,0,0,1) { Species = 439, Ability = A3 }, // Mime Jr.
            new EncounterStatic8N(Nest033,0,0,1) { Species = 868, Ability = A3 }, // Milcery
            new EncounterStatic8N(Nest033,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest033,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest033,1,1,2) { Species = 035, Ability = A3 }, // Clefairy
            new EncounterStatic8N(Nest033,1,2,2) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest033,2,3,3) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest033,2,4,4) { Species = 036, Ability = A3 }, // Clefable
            new EncounterStatic8N(Nest033,2,4,4) { Species = 282, Ability = A3 }, // Gardevoir
            new EncounterStatic8N(Nest033,3,4,4) { Species = 869, Ability = A4 }, // Alcremie
            new EncounterStatic8N(Nest033,4,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest034,0,0,1) { Species = 509, Ability = A3 }, // Purrloin
            new EncounterStatic8N(Nest034,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest034,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest034,0,1,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest034,1,1,2) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest034,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest034,2,3,3) { Species = 435, Ability = A3 }, // Skuntank
            new EncounterStatic8N(Nest034,2,4,4) { Species = 461, Ability = A3 }, // Weavile
            new EncounterStatic8N(Nest034,2,4,4) { Species = 687, Ability = A3 }, // Malamar
            new EncounterStatic8N(Nest034,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest034,4,4,4) { Species = 342, Ability = A4 }, // Crawdaunt
            new EncounterStatic8N(Nest035,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest035,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest035,0,1,1) { Species = 509, Ability = A3 }, // Purrloin
            new EncounterStatic8N(Nest035,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest035,1,2,2) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest035,2,3,3) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest035,2,4,4) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest035,2,4,4) { Species = 861, Ability = A3 }, // Grimmsnarl
            new EncounterStatic8N(Nest035,4,4,4) { Species = 862, Ability = A4 }, // Obstagoon
            new EncounterStatic8N(Nest036,0,0,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest036,0,1,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest036,1,2,2) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest037,0,0,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest037,0,0,1) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest037,0,1,1) { Species = 885, Ability = A3 }, // Dreepy
            new EncounterStatic8N(Nest037,1,1,2) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest037,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest037,2,3,3) { Species = 886, Ability = A3 }, // Drakloak
            new EncounterStatic8N(Nest037,2,4,4) { Species = 715, Ability = A3 }, // Noivern
            new EncounterStatic8N(Nest037,4,4,4) { Species = 887, Ability = A4 }, // Dragapult
            new EncounterStatic8N(Nest038,0,0,1) { Species = 659, Ability = A3 }, // Bunnelby
            new EncounterStatic8N(Nest038,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest038,0,1,1) { Species = 519, Ability = A3 }, // Pidove
            new EncounterStatic8N(Nest038,0,1,1) { Species = 572, Ability = A3 }, // Minccino
            new EncounterStatic8N(Nest038,1,1,2) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest038,1,2,2) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest038,2,3,3) { Species = 660, Ability = A3 }, // Diggersby
            new EncounterStatic8N(Nest038,2,4,4) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest038,2,4,4) { Species = 521, Ability = A3 }, // Unfezant
            new EncounterStatic8N(Nest038,3,4,4) { Species = 695, Ability = A4 }, // Heliolisk
            new EncounterStatic8N(Nest038,4,4,4) { Species = 573, Ability = A4 }, // Cinccino
            new EncounterStatic8N(Nest038,4,4,4) { Species = 760, Ability = A4 }, // Bewear
            new EncounterStatic8N(Nest039,0,0,1) { Species = 819, Ability = A3 }, // Skwovet
            new EncounterStatic8N(Nest039,0,0,1) { Species = 831, Ability = A3 }, // Wooloo
            new EncounterStatic8N(Nest039,0,1,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest039,0,1,1) { Species = 446, Ability = A3 }, // Munchlax
            new EncounterStatic8N(Nest039,1,2,2) { Species = 820, Ability = A3 }, // Greedent
            new EncounterStatic8N(Nest039,2,3,3) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest039,2,4,4) { Species = 820, Ability = A3 }, // Greedent
            new EncounterStatic8N(Nest039,2,4,4) { Species = 832, Ability = A3 }, // Dubwool
            new EncounterStatic8N(Nest039,3,4,4) { Species = 660, Ability = A4 }, // Diggersby
            new EncounterStatic8N(Nest039,4,4,4) { Species = 143, Ability = A4 }, // Snorlax
            new EncounterStatic8N(Nest040,0,0,1) { Species = 535, Ability = A3 }, // Tympole
            new EncounterStatic8N(Nest040,0,0,1) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest040,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest040,1,2,2) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest040,2,4,4) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest040,4,4,4) { Species = 847, Ability = A4 }, // Barraskewda
            new EncounterStatic8N(Nest041,0,0,1) { Species = 422, Ability = A3, Form = 1 }, // Shellos-1
            new EncounterStatic8N(Nest041,0,0,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest041,0,1,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest041,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest041,1,1,2) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest041,1,2,2) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest041,2,3,3) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest041,2,4,4) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest041,2,4,4) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest041,3,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest041,4,4,4) { Species = 593, Ability = A4 }, // Jellicent
            new EncounterStatic8N(Nest041,4,4,4) { Species = 834, Ability = A4 }, // Drednaw
            new EncounterStatic8N(Nest042,0,0,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest042,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest042,0,1,1) { Species = 854, Ability = A3 }, // Sinistea
            new EncounterStatic8N(Nest042,0,1,1) { Species = 355, Ability = A3 }, // Duskull
            new EncounterStatic8N(Nest042,1,2,2) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest042,2,3,3) { Species = 356, Ability = A3 }, // Dusclops
            new EncounterStatic8N(Nest042,4,4,4) { Species = 477, Ability = A4 }, // Dusknoir
            new EncounterStatic8N(Nest042,4,4,4) { Species = 094, Ability = A4 }, // Gengar
            new EncounterStatic8N(Nest043,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest043,0,0,1) { Species = 349, Ability = A3 }, // Feebas
            new EncounterStatic8N(Nest043,0,1,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest043,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest043,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest043,2,3,3) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest043,2,4,4) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest043,3,4,4) { Species = 771, Ability = A4 }, // Pyukumuku
            new EncounterStatic8N(Nest043,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest043,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest043,4,4,4) { Species = 350, Ability = A4 }, // Milotic
            new EncounterStatic8N(Nest044,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest044,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest044,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest044,0,1,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest044,1,2,2) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest044,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest044,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest044,3,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest044,4,4,4) { Species = 601, Ability = A4 }, // Klinklang
            new EncounterStatic8N(Nest044,4,4,4) { Species = 448, Ability = A4 }, // Lucario
            new EncounterStatic8N(Nest045,0,0,1) { Species = 767, Ability = A3 }, // Wimpod
            new EncounterStatic8N(Nest045,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest045,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest045,1,2,2) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest045,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest045,2,4,4) { Species = 826, Ability = A3 }, // Orbeetle
            new EncounterStatic8N(Nest045,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest045,3,4,4) { Species = 768, Ability = A4 }, // Golisopod
            new EncounterStatic8N(Nest045,4,4,4) { Species = 292, Ability = A4 }, // Shedinja
            new EncounterStatic8N(Nest046,0,0,1) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest046,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest046,0,1,1) { Species = 854, Ability = A3 }, // Sinistea
            new EncounterStatic8N(Nest046,0,1,1) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest046,1,2,2) { Species = 680, Ability = A3 }, // Doublade
            new EncounterStatic8N(Nest046,2,3,3) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest046,3,4,4) { Species = 855, Ability = A4 }, // Polteageist
            new EncounterStatic8N(Nest046,4,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest046,4,4,4) { Species = 681, Ability = A4 }, // Aegislash
            new EncounterStatic8N(Nest047,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest047,0,0,1) { Species = 066, Ability = A3 }, // Machop
            new EncounterStatic8N(Nest047,0,1,1) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest047,1,2,2) { Species = 760, Ability = A3 }, // Bewear
            new EncounterStatic8N(Nest047,1,3,3) { Species = 870, Ability = A3 }, // Falinks
            new EncounterStatic8N(Nest047,2,3,3) { Species = 067, Ability = A3 }, // Machoke
            new EncounterStatic8N(Nest047,3,4,4) { Species = 068, Ability = A4 }, // Machamp
            new EncounterStatic8N(Nest047,4,4,4) { Species = 448, Ability = A4 }, // Lucario
            new EncounterStatic8N(Nest047,4,4,4) { Species = 475, Ability = A4 }, // Gallade
            new EncounterStatic8N(Nest048,0,0,1) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest048,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest048,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest048,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest048,1,2,2) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest048,1,2,2) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest048,3,4,4) { Species = 863, Ability = A4 }, // Perrserker
            new EncounterStatic8N(Nest048,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest048,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest048,3,4,4) { Species = 618, Ability = A4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest048,4,4,4) { Species = 879, Ability = A4 }, // Copperajah
            new EncounterStatic8N(Nest048,4,4,4) { Species = 884, Ability = A4 }, // Duraludon
            new EncounterStatic8N(Nest049,0,0,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest049,0,0,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest049,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest049,0,1,1) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest049,1,2,2) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest049,1,2,2) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest049,2,3,3) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest049,2,4,4) { Species = 528, Ability = A3 }, // Swoobat
            new EncounterStatic8N(Nest049,3,4,4) { Species = 858, Ability = A4 }, // Hatterene
            new EncounterStatic8N(Nest049,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest049,4,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest049,4,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest050,0,0,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest050,0,0,1) { Species = 438, Ability = A3 }, // Bonsly
            new EncounterStatic8N(Nest050,0,1,1) { Species = 837, Ability = A3 }, // Rolycoly
            new EncounterStatic8N(Nest050,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest050,2,4,4) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest050,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest050,3,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest050,4,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest051,0,0,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest051,0,0,1) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest051,0,1,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest051,0,1,1) { Species = 622, Ability = A3 }, // Golett
            new EncounterStatic8N(Nest051,1,2,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest051,1,2,2) { Species = 195, Ability = A3 }, // Quagsire
            new EncounterStatic8N(Nest051,2,3,3) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest051,2,4,4) { Species = 623, Ability = A3 }, // Golurk
            new EncounterStatic8N(Nest051,3,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest051,3,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest051,4,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest051,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest052,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest052,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest052,0,1,1) { Species = 004, Ability = A3 }, // Charmander
            new EncounterStatic8N(Nest052,1,2,2) { Species = 005, Ability = A3 }, // Charmeleon
            new EncounterStatic8N(Nest052,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest052,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest052,3,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest052,4,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest052,4,4,4) { Species = 006, Ability = A4 }, // Charizard
            new EncounterStatic8N(Nest053,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest053,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest053,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest053,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest053,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest053,3,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest053,4,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest054,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest054,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest054,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest054,1,2,2) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest054,1,2,2) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest054,2,3,3) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest054,2,4,4) { Species = 362, Ability = A3 }, // Glalie
            new EncounterStatic8N(Nest054,3,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest054,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest054,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest055,0,0,1) { Species = 835, Ability = A3 }, // Yamper
            new EncounterStatic8N(Nest055,0,0,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest055,0,1,1) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest055,0,1,1) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest055,1,2,2) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest055,1,2,2) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest055,2,4,4) { Species = 836, Ability = A3 }, // Boltund
            new EncounterStatic8N(Nest055,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest055,3,4,4) { Species = 871, Ability = A4 }, // Pincurchin
            new EncounterStatic8N(Nest055,3,4,4) { Species = 596, Ability = A4 }, // Galvantula
            new EncounterStatic8N(Nest055,4,4,4) { Species = 777, Ability = A4 }, // Togedemaru
            new EncounterStatic8N(Nest055,4,4,4) { Species = 877, Ability = A4 }, // Morpeko
            new EncounterStatic8N(Nest056,0,0,1) { Species = 172, Ability = A3 }, // Pichu
            new EncounterStatic8N(Nest056,0,0,1) { Species = 309, Ability = A3 }, // Electrike
            new EncounterStatic8N(Nest056,0,1,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest056,0,1,1) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest056,1,2,2) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest056,1,2,2) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest056,2,4,4) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest056,2,4,4) { Species = 479, Ability = A3, Form = 5 }, // Rotom-5
            new EncounterStatic8N(Nest056,3,4,4) { Species = 479, Ability = A4, Form = 4 }, // Rotom-4
            new EncounterStatic8N(Nest056,3,4,4) { Species = 479, Ability = A4, Form = 3 }, // Rotom-3
            new EncounterStatic8N(Nest056,4,4,4) { Species = 479, Ability = A4, Form = 2 }, // Rotom-2
            new EncounterStatic8N(Nest056,4,4,4) { Species = 479, Ability = A4, Form = 1 }, // Rotom-1
            new EncounterStatic8N(Nest057,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest057,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest057,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest057,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest057,2,4,4) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest057,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest057,3,4,4) { Species = 598, Ability = A4 }, // Ferrothorn
            new EncounterStatic8N(Nest057,4,4,4) { Species = 407, Ability = A4 }, // Roserade
            new EncounterStatic8N(Nest058,0,0,1) { Species = 420, Ability = A3 }, // Cherubi
            new EncounterStatic8N(Nest058,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest058,0,1,1) { Species = 546, Ability = A3 }, // Cottonee
            new EncounterStatic8N(Nest058,1,2,2) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest058,2,4,4) { Species = 421, Ability = A3 }, // Cherrim
            new EncounterStatic8N(Nest058,2,4,4) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest058,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest058,3,4,4) { Species = 547, Ability = A4 }, // Whimsicott
            new EncounterStatic8N(Nest058,4,4,4) { Species = 781, Ability = A4 }, // Dhelmise
            new EncounterStatic8N(Nest059,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest059,0,0,1) { Species = 568, Ability = A3 }, // Trubbish
            new EncounterStatic8N(Nest059,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest059,0,1,1) { Species = 109, Ability = A3 }, // Koffing
            new EncounterStatic8N(Nest059,1,2,2) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest059,2,4,4) { Species = 569, Ability = A3 }, // Garbodor
            new EncounterStatic8N(Nest059,2,4,4) { Species = 452, Ability = A3 }, // Drapion
            new EncounterStatic8N(Nest059,3,4,4) { Species = 849, Ability = A4 }, // Toxtricity
            new EncounterStatic8N(Nest059,3,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest059,4,4,4) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest060,0,0,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest060,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest060,0,1,1) { Species = 821, Ability = A3 }, // Rookidee
            new EncounterStatic8N(Nest060,0,1,1) { Species = 278, Ability = A3 }, // Wingull
            new EncounterStatic8N(Nest060,1,2,2) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest060,1,2,2) { Species = 822, Ability = A3 }, // Corvisquire
            new EncounterStatic8N(Nest060,2,4,4) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest060,2,4,4) { Species = 279, Ability = A3 }, // Pelipper
            new EncounterStatic8N(Nest060,3,4,4) { Species = 178, Ability = A4 }, // Xatu
            new EncounterStatic8N(Nest060,3,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest060,4,4,4) { Species = 823, Ability = A4 }, // Corviknight
            new EncounterStatic8N(Nest060,4,4,4) { Species = 225, Ability = A4 }, // Delibird
            new EncounterStatic8N(Nest061,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest061,0,0,1) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest061,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest061,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest061,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest061,1,2,2) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest061,2,4,4) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest061,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest061,3,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest061,4,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest061,4,4,4) { Species = 778, Ability = A4 }, // Mimikyu
            new EncounterStatic8N(Nest062,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest062,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest062,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest062,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest062,1,2,2) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest062,2,4,4) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest062,2,4,4) { Species = 675, Ability = A3 }, // Pangoro
            new EncounterStatic8N(Nest062,3,4,4) { Species = 461, Ability = A4 }, // Weavile
            new EncounterStatic8N(Nest062,4,4,4) { Species = 862, Ability = A4 }, // Obstagoon
            new EncounterStatic8N(Nest063,0,0,1) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest063,1,2,2) { Species = 885, Ability = A3 }, // Dreepy
            new EncounterStatic8N(Nest063,3,4,4) { Species = 886, Ability = A4 }, // Drakloak
            new EncounterStatic8N(Nest063,4,4,4) { Species = 887, Ability = A4 }, // Dragapult
            new EncounterStatic8N(Nest064,0,0,1) { Species = 659, Ability = A3 }, // Bunnelby
            new EncounterStatic8N(Nest064,0,0,1) { Species = 519, Ability = A3 }, // Pidove
            new EncounterStatic8N(Nest064,0,1,1) { Species = 819, Ability = A3 }, // Skwovet
            new EncounterStatic8N(Nest064,0,1,1) { Species = 133, Ability = A3 }, // Eevee
            new EncounterStatic8N(Nest064,1,2,2) { Species = 520, Ability = A3 }, // Tranquill
            new EncounterStatic8N(Nest064,1,2,2) { Species = 831, Ability = A3 }, // Wooloo
            new EncounterStatic8N(Nest064,2,4,4) { Species = 521, Ability = A3 }, // Unfezant
            new EncounterStatic8N(Nest064,2,4,4) { Species = 832, Ability = A3 }, // Dubwool
            new EncounterStatic8N(Nest064,4,4,4) { Species = 133, Ability = A4 }, // Eevee
            new EncounterStatic8N(Nest064,4,4,4) { Species = 143, Ability = A4 }, // Snorlax
            new EncounterStatic8N(Nest065,0,0,1) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest065,0,1,2) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest065,1,2,3) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest065,2,3,3) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest065,3,4,4) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest065,4,4,4) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest066,0,0,1) { Species = 458, Ability = A3 }, // Mantyke
            new EncounterStatic8N(Nest066,0,0,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest066,0,1,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest066,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest066,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest066,2,3,3) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest066,2,4,4) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest066,3,4,4) { Species = 771, Ability = A4 }, // Pyukumuku
            new EncounterStatic8N(Nest066,3,4,4) { Species = 226, Ability = A4 }, // Mantine
            new EncounterStatic8N(Nest066,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest066,4,4,4) { Species = 134, Ability = A4 }, // Vaporeon
            new EncounterStatic8N(Nest067,0,0,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest067,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest067,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest067,0,1,1) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest067,1,2,2) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest067,1,2,2) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest067,2,3,3) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest067,2,4,4) { Species = 528, Ability = A3 }, // Swoobat
            new EncounterStatic8N(Nest067,3,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest067,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest067,4,4,4) { Species = 858, Ability = A4 }, // Hatterene
            new EncounterStatic8N(Nest067,4,4,4) { Species = 196, Ability = A4 }, // Espeon
            new EncounterStatic8N(Nest068,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest068,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest068,0,1,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest068,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest068,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest068,1,2,2) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest068,2,4,4) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest068,2,4,4) { Species = 675, Ability = A3 }, // Pangoro
            new EncounterStatic8N(Nest068,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest068,3,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest068,4,4,4) { Species = 862, Ability = A4 }, // Obstagoon
            new EncounterStatic8N(Nest068,4,4,4) { Species = 197, Ability = A4 }, // Umbreon
            new EncounterStatic8N(Nest069,0,0,1) { Species = 420, Ability = A3 }, // Cherubi
            new EncounterStatic8N(Nest069,0,0,1) { Species = 761, Ability = A3 }, // Bounsweet
            new EncounterStatic8N(Nest069,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest069,0,1,1) { Species = 546, Ability = A3 }, // Cottonee
            new EncounterStatic8N(Nest069,1,2,2) { Species = 762, Ability = A3 }, // Steenee
            new EncounterStatic8N(Nest069,1,2,2) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest069,2,4,4) { Species = 421, Ability = A3 }, // Cherrim
            new EncounterStatic8N(Nest069,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest069,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest069,3,4,4) { Species = 763, Ability = A4 }, // Tsareena
            new EncounterStatic8N(Nest069,4,4,4) { Species = 547, Ability = A4 }, // Whimsicott
            new EncounterStatic8N(Nest069,4,4,4) { Species = 470, Ability = A4 }, // Leafeon
            new EncounterStatic8N(Nest070,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest070,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest070,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest070,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest070,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest070,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest070,4,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest070,4,4,4) { Species = 136, Ability = A4 }, // Flareon
            new EncounterStatic8N(Nest071,0,0,1) { Species = 835, Ability = A3 }, // Yamper
            new EncounterStatic8N(Nest071,0,0,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest071,0,1,1) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest071,0,1,1) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest071,1,2,2) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest071,1,2,2) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest071,2,4,4) { Species = 836, Ability = A3 }, // Boltund
            new EncounterStatic8N(Nest071,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest071,3,4,4) { Species = 695, Ability = A4 }, // Heliolisk
            new EncounterStatic8N(Nest071,3,4,4) { Species = 738, Ability = A4 }, // Vikavolt
            new EncounterStatic8N(Nest071,4,4,4) { Species = 025, Ability = A4 }, // Pikachu
            new EncounterStatic8N(Nest071,4,4,4) { Species = 135, Ability = A4 }, // Jolteon
            new EncounterStatic8N(Nest072,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest072,0,0,1) { Species = 872, Ability = A3 }, // Snom
            new EncounterStatic8N(Nest072,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest072,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest072,1,2,2) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest072,1,2,2) { Species = 583, Ability = A3 }, // Vanillish
            new EncounterStatic8N(Nest072,2,3,3) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest072,2,4,4) { Species = 873, Ability = A3 }, // Frosmoth
            new EncounterStatic8N(Nest072,3,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest072,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest072,4,4,4) { Species = 478, Ability = A4 }, // Froslass
            new EncounterStatic8N(Nest072,4,4,4) { Species = 471, Ability = A4 }, // Glaceon
            new EncounterStatic8N(Nest073,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest073,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest073,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest073,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest073,1,2,2) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest073,2,4,4) { Species = 868, Ability = A3 }, // Milcery
            new EncounterStatic8N(Nest073,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest073,3,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest073,4,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest073,4,4,4) { Species = 700, Ability = A4 }, // Sylveon
            new EncounterStatic8N(Nest074,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest074,0,0,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest074,0,1,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest074,0,1,1) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest074,1,2,2) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest074,1,2,2) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest074,2,3,3) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest074,2,4,4) { Species = 340, Ability = A3 }, // Whiscash
            new EncounterStatic8N(Nest074,3,4,4) { Species = 211, Ability = A4 }, // Qwilfish
            new EncounterStatic8N(Nest074,3,4,4) { Species = 195, Ability = A4 }, // Quagsire
            new EncounterStatic8N(Nest074,4,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest074,4,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest075,0,0,1) { Species = 458, Ability = A3 }, // Mantyke
            new EncounterStatic8N(Nest075,0,0,1) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest075,0,1,1) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest075,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest075,1,2,2) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest075,1,2,2) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest075,2,3,3) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest075,3,4,4) { Species = 211, Ability = A4 }, // Qwilfish
            new EncounterStatic8N(Nest075,3,4,4) { Species = 224, Ability = A4 }, // Octillery
            new EncounterStatic8N(Nest075,4,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest075,4,4,4) { Species = 226, Ability = A4 }, // Mantine
            new EncounterStatic8N(Nest076,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest076,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest076,0,1,1) { Species = 004, Ability = A3 }, // Charmander
            new EncounterStatic8N(Nest076,1,2,2) { Species = 005, Ability = A3 }, // Charmeleon
            new EncounterStatic8N(Nest076,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest076,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest076,3,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest076,4,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest076,4,4,4) { Species = 006, Ability = A4, CanGigantamax = true }, // Charizard
            new EncounterStatic8N(Nest077,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest077,0,0,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest077,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest077,0,1,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest077,1,2,2) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest077,2,3,3) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest077,2,4,4) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest077,3,4,4) { Species = 746, Ability = A4 }, // Wishiwashi
            new EncounterStatic8N(Nest077,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest077,4,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest077,4,4,4) { Species = 834, Ability = A4, CanGigantamax = true }, // Drednaw
            new EncounterStatic8N(Nest078,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest078,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest078,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest078,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest078,2,4,4) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest078,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest078,3,4,4) { Species = 598, Ability = A4 }, // Ferrothorn
            new EncounterStatic8N(Nest078,4,4,4) { Species = 407, Ability = A4 }, // Roserade
            new EncounterStatic8N(Nest079,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest079,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest079,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest079,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest079,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest079,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest079,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest079,3,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest079,4,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest079,4,4,4) { Species = 851, Ability = A4, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8N(Nest081,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest081,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest081,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest081,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest081,1,2,2) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest081,2,3,3) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest081,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest081,3,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest081,4,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest081,4,4,4) { Species = 869, Ability = A4, CanGigantamax = true }, // Alcremie
            new EncounterStatic8N(Nest083,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest083,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest083,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest083,0,1,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest083,1,2,2) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest083,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest083,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest083,3,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest083,4,4,4) { Species = 601, Ability = A4 }, // Klinklang
            new EncounterStatic8N(Nest083,4,4,4) { Species = 884, Ability = A4, CanGigantamax = true }, // Duraludon
            new EncounterStatic8N(Nest084,0,0,1) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest084,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest084,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest084,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest084,1,2,2) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest084,1,2,2) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest084,2,3,3) { Species = 863, Ability = A3 }, // Perrserker
            new EncounterStatic8N(Nest084,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest084,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest084,3,4,4) { Species = 618, Ability = A4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest084,4,4,4) { Species = 884, Ability = A4 }, // Duraludon
            new EncounterStatic8N(Nest084,4,4,4) { Species = 879, Ability = A4, CanGigantamax = true }, // Copperajah
            new EncounterStatic8N(Nest085,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest085,0,0,1) { Species = 568, Ability = A3 }, // Trubbish
            new EncounterStatic8N(Nest085,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest085,0,1,1) { Species = 109, Ability = A3 }, // Koffing
            new EncounterStatic8N(Nest085,1,2,2) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest085,2,3,3) { Species = 452, Ability = A3 }, // Drapion
            new EncounterStatic8N(Nest085,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest085,3,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest085,3,4,4) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest085,4,4,4) { Species = 569, Ability = A4, CanGigantamax = true }, // Garbodor
            new EncounterStatic8N(Nest086,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest086,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest086,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest086,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest086,1,2,2) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest086,2,4,4) { Species = 868, Ability = A3 }, // Milcery
            new EncounterStatic8N(Nest086,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest086,3,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest086,4,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest086,4,4,4) { Species = 858, Ability = A4, CanGigantamax = true }, // Hatterene
            new EncounterStatic8N(Nest087,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest087,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest087,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest087,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest087,1,2,2) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest087,2,3,3) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest087,2,4,4) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest087,3,4,4) { Species = 675, Ability = A4 }, // Pangoro
            new EncounterStatic8N(Nest087,4,4,4) { Species = 861, Ability = A4, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8N(Nest088,0,0,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest088,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest088,0,1,1) { Species = 821, Ability = A3 }, // Rookidee
            new EncounterStatic8N(Nest088,0,1,1) { Species = 278, Ability = A3 }, // Wingull
            new EncounterStatic8N(Nest088,1,2,2) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest088,1,2,2) { Species = 822, Ability = A3 }, // Corvisquire
            new EncounterStatic8N(Nest088,2,3,3) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest088,2,4,4) { Species = 279, Ability = A3 }, // Pelipper
            new EncounterStatic8N(Nest088,3,4,4) { Species = 178, Ability = A4 }, // Xatu
            new EncounterStatic8N(Nest088,3,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest088,4,4,4) { Species = 561, Ability = A4 }, // Sigilyph
            new EncounterStatic8N(Nest088,4,4,4) { Species = 823, Ability = A4, CanGigantamax = true }, // Corviknight
            new EncounterStatic8N(Nest089,0,0,1) { Species = 767, Ability = A3 }, // Wimpod
            new EncounterStatic8N(Nest089,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest089,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest089,1,2,2) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest089,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest089,2,4,4) { Species = 826, Ability = A3 }, // Orbeetle
            new EncounterStatic8N(Nest089,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest089,3,4,4) { Species = 768, Ability = A4 }, // Golisopod
            new EncounterStatic8N(Nest089,0,4,4) { Species = 012, Ability = A4, CanGigantamax = true }, // Butterfree
            new EncounterStatic8N(Nest090,0,0,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest090,0,0,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest090,0,1,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest090,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest090,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest090,2,3,3) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest090,2,4,4) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest090,3,4,4) { Species = 771, Ability = A4 }, // Pyukumuku
            new EncounterStatic8N(Nest090,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest090,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest090,1,4,4) { Species = 099, Ability = A4, CanGigantamax = true }, // Kingler
            new EncounterStatic8N(Nest091,0,0,1) { Species = 767, Ability = A3 }, // Wimpod
            new EncounterStatic8N(Nest091,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest091,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest091,1,2,2) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest091,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest091,2,4,4) { Species = 826, Ability = A3 }, // Orbeetle
            new EncounterStatic8N(Nest091,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest091,3,4,4) { Species = 768, Ability = A4 }, // Golisopod
            new EncounterStatic8N(Nest091,2,4,4) { Species = 826, Ability = A4, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8N(Nest092,0,0,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest092,0,0,1) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest092,0,1,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest092,0,1,1) { Species = 622, Ability = A3 }, // Golett
            new EncounterStatic8N(Nest092,1,2,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest092,1,2,2) { Species = 195, Ability = A3 }, // Quagsire
            new EncounterStatic8N(Nest092,2,3,3) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest092,2,4,4) { Species = 623, Ability = A3 }, // Golurk
            new EncounterStatic8N(Nest092,3,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest092,3,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest092,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest092,3,4,4) { Species = 844, Ability = A4, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8N(Nest098,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest098,0,1,1) { Species = 174, Ability = A3 }, // Igglybuff
            new EncounterStatic8N(Nest098,0,1,1) { Species = 506, Ability = A3 }, // Lillipup
            new EncounterStatic8N(Nest098,1,2,2) { Species = 427, Ability = A3 }, // Buneary
            new EncounterStatic8N(Nest098,1,2,2) { Species = 039, Ability = A3 }, // Jigglypuff
            new EncounterStatic8N(Nest098,2,3,3) { Species = 039, Ability = A3 }, // Jigglypuff
            new EncounterStatic8N(Nest098,2,3,3) { Species = 507, Ability = A3 }, // Herdier
            new EncounterStatic8N(Nest098,3,4,4) { Species = 428, Ability = A4 }, // Lopunny
            new EncounterStatic8N(Nest098,3,4,4) { Species = 040, Ability = A4 }, // Wigglytuff
            new EncounterStatic8N(Nest098,4,4,4) { Species = 206, Ability = A4 }, // Dunsparce
            new EncounterStatic8N(Nest098,4,4,4) { Species = 508, Ability = A4 }, // Stoutland
            new EncounterStatic8N(Nest099,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest099,0,1,1) { Species = 506, Ability = A2 }, // Lillipup
            new EncounterStatic8N(Nest099,0,1,1) { Species = 759, Ability = A2 }, // Stufful
            new EncounterStatic8N(Nest099,1,2,2) { Species = 039, Ability = A2 }, // Jigglypuff
            new EncounterStatic8N(Nest099,1,2,2) { Species = 427, Ability = A2 }, // Buneary
            new EncounterStatic8N(Nest099,2,3,3) { Species = 039, Ability = A2 }, // Jigglypuff
            new EncounterStatic8N(Nest099,2,3,3) { Species = 206, Ability = A2 }, // Dunsparce
            new EncounterStatic8N(Nest099,3,4,4) { Species = 832, Ability = A2 }, // Dubwool
            new EncounterStatic8N(Nest099,3,4,4) { Species = 428, Ability = A2 }, // Lopunny
            new EncounterStatic8N(Nest099,3,4,4) { Species = 508, Ability = A2 }, // Stoutland
            new EncounterStatic8N(Nest099,4,4,4) { Species = 760, Ability = A2 }, // Bewear
            new EncounterStatic8N(Nest099,4,4,4) { Species = 040, Ability = A2 }, // Wigglytuff
            new EncounterStatic8N(Nest100,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest100,0,1,1) { Species = 293, Ability = A3 }, // Whismur
            new EncounterStatic8N(Nest100,0,1,1) { Species = 108, Ability = A3 }, // Lickitung
            new EncounterStatic8N(Nest100,1,2,2) { Species = 241, Ability = A3 }, // Miltank
            new EncounterStatic8N(Nest100,1,2,2) { Species = 294, Ability = A3 }, // Loudred
            new EncounterStatic8N(Nest100,2,3,3) { Species = 294, Ability = A3 }, // Loudred
            new EncounterStatic8N(Nest100,2,3,3) { Species = 108, Ability = A3 }, // Lickitung
            new EncounterStatic8N(Nest100,3,4,4) { Species = 241, Ability = A4 }, // Miltank
            new EncounterStatic8N(Nest100,3,4,4) { Species = 626, Ability = A4 }, // Bouffalant
            new EncounterStatic8N(Nest100,3,4,4) { Species = 128, Ability = A4 }, // Tauros
            new EncounterStatic8N(Nest100,4,4,4) { Species = 295, Ability = A4 }, // Exploud
            new EncounterStatic8N(Nest100,4,4,4) { Species = 463, Ability = A4 }, // Lickilicky
            new EncounterStatic8N(Nest101,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest101,0,1,1) { Species = 293, Ability = A2 }, // Whismur
            new EncounterStatic8N(Nest101,0,1,1) { Species = 128, Ability = A2 }, // Tauros
            new EncounterStatic8N(Nest101,1,2,2) { Species = 108, Ability = A2 }, // Lickitung
            new EncounterStatic8N(Nest101,1,2,2) { Species = 241, Ability = A2 }, // Miltank
            new EncounterStatic8N(Nest101,2,3,3) { Species = 241, Ability = A2 }, // Miltank
            new EncounterStatic8N(Nest101,2,3,3) { Species = 626, Ability = A2 }, // Bouffalant
            new EncounterStatic8N(Nest101,3,4,4) { Species = 128, Ability = A2 }, // Tauros
            new EncounterStatic8N(Nest101,3,4,4) { Species = 295, Ability = A2 }, // Exploud
            new EncounterStatic8N(Nest101,3,4,4) { Species = 573, Ability = A2 }, // Cinccino
            new EncounterStatic8N(Nest101,4,4,4) { Species = 295, Ability = A2 }, // Exploud
            new EncounterStatic8N(Nest101,4,4,4) { Species = 463, Ability = A2 }, // Lickilicky
            new EncounterStatic8N(Nest102,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest102,0,1,1) { Species = 027, Ability = A3 }, // Sandshrew
            new EncounterStatic8N(Nest102,0,1,1) { Species = 551, Ability = A3 }, // Sandile
            new EncounterStatic8N(Nest102,1,2,2) { Species = 104, Ability = A3 }, // Cubone
            new EncounterStatic8N(Nest102,1,2,2) { Species = 027, Ability = A3 }, // Sandshrew
            new EncounterStatic8N(Nest102,2,3,3) { Species = 552, Ability = A3 }, // Krokorok
            new EncounterStatic8N(Nest102,2,3,3) { Species = 028, Ability = A3 }, // Sandslash
            new EncounterStatic8N(Nest102,3,4,4) { Species = 844, Ability = A4 }, // Sandaconda
            new EncounterStatic8N(Nest102,3,4,4) { Species = 028, Ability = A4 }, // Sandslash
            new EncounterStatic8N(Nest102,3,4,4) { Species = 105, Ability = A4 }, // Marowak
            new EncounterStatic8N(Nest102,4,4,4) { Species = 553, Ability = A4 }, // Krookodile
            new EncounterStatic8N(Nest102,4,4,4) { Species = 115, Ability = A4 }, // Kangaskhan
            new EncounterStatic8N(Nest103,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest103,0,1,1) { Species = 027, Ability = A2 }, // Sandshrew
            new EncounterStatic8N(Nest103,0,1,1) { Species = 104, Ability = A2 }, // Cubone
            new EncounterStatic8N(Nest103,1,2,2) { Species = 328, Ability = A2 }, // Trapinch
            new EncounterStatic8N(Nest103,2,3,3) { Species = 552, Ability = A2 }, // Krokorok
            new EncounterStatic8N(Nest103,2,3,3) { Species = 028, Ability = A2 }, // Sandslash
            new EncounterStatic8N(Nest103,3,4,4) { Species = 105, Ability = A2 }, // Marowak
            new EncounterStatic8N(Nest103,3,4,4) { Species = 553, Ability = A2 }, // Krookodile
            new EncounterStatic8N(Nest103,3,4,4) { Species = 115, Ability = A2 }, // Kangaskhan
            new EncounterStatic8N(Nest103,4,4,4) { Species = 330, Ability = A2 }, // Flygon
            new EncounterStatic8N(Nest103,4,4,4) { Species = 623, Ability = A2 }, // Golurk
            new EncounterStatic8N(Nest104,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest104,0,1,1) { Species = 702, Ability = A3 }, // Dedenne
            new EncounterStatic8N(Nest104,0,1,1) { Species = 081, Ability = A3 }, // Magnemite
            new EncounterStatic8N(Nest104,1,2,2) { Species = 403, Ability = A3 }, // Shinx
            new EncounterStatic8N(Nest104,1,2,2) { Species = 877, Ability = A3 }, // Morpeko
            new EncounterStatic8N(Nest104,2,3,3) { Species = 702, Ability = A3 }, // Dedenne
            new EncounterStatic8N(Nest104,2,3,3) { Species = 404, Ability = A3 }, // Luxio
            new EncounterStatic8N(Nest104,3,4,4) { Species = 702, Ability = A4 }, // Dedenne
            new EncounterStatic8N(Nest104,3,4,4) { Species = 082, Ability = A4 }, // Magneton
            new EncounterStatic8N(Nest104,3,4,4) { Species = 871, Ability = A4 }, // Pincurchin
            new EncounterStatic8N(Nest104,4,4,4) { Species = 405, Ability = A4 }, // Luxray
            new EncounterStatic8N(Nest104,4,4,4) { Species = 462, Ability = A4 }, // Magnezone
            new EncounterStatic8N(Nest105,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest105,0,1,1) { Species = 403, Ability = A2 }, // Shinx
            new EncounterStatic8N(Nest105,0,1,1) { Species = 172, Ability = A2 }, // Pichu
            new EncounterStatic8N(Nest105,1,2,2) { Species = 025, Ability = A2 }, // Pikachu
            new EncounterStatic8N(Nest105,1,2,2) { Species = 871, Ability = A2 }, // Pincurchin
            new EncounterStatic8N(Nest105,2,3,3) { Species = 404, Ability = A2 }, // Luxio
            new EncounterStatic8N(Nest105,2,3,3) { Species = 026, Ability = A2 }, // Raichu
            new EncounterStatic8N(Nest105,3,4,4) { Species = 836, Ability = A2 }, // Boltund
            new EncounterStatic8N(Nest105,3,4,4) { Species = 702, Ability = A2 }, // Dedenne
            new EncounterStatic8N(Nest105,3,4,4) { Species = 310, Ability = A2 }, // Manectric
            new EncounterStatic8N(Nest105,4,4,4) { Species = 405, Ability = A2 }, // Luxray
            new EncounterStatic8N(Nest105,4,4,4) { Species = 462, Ability = A2 }, // Magnezone
            new EncounterStatic8N(Nest106,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest106,0,1,1) { Species = 661, Ability = A3 }, // Fletchling
            new EncounterStatic8N(Nest106,0,1,1) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest106,1,2,2) { Species = 587, Ability = A3 }, // Emolga
            new EncounterStatic8N(Nest106,2,3,3) { Species = 662, Ability = A3 }, // Fletchinder
            new EncounterStatic8N(Nest106,3,4,4) { Species = 587, Ability = A4 }, // Emolga
            new EncounterStatic8N(Nest106,3,4,4) { Species = 528, Ability = A4 }, // Swoobat
            new EncounterStatic8N(Nest106,4,4,4) { Species = 663, Ability = A4 }, // Talonflame
            new EncounterStatic8N(Nest107,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest107,0,1,1) { Species = 163, Ability = A2 }, // Hoothoot
            new EncounterStatic8N(Nest107,0,1,1) { Species = 519, Ability = A2 }, // Pidove
            new EncounterStatic8N(Nest107,1,2,2) { Species = 520, Ability = A2 }, // Tranquill
            new EncounterStatic8N(Nest107,2,3,3) { Species = 528, Ability = A2 }, // Swoobat
            new EncounterStatic8N(Nest107,2,3,3) { Species = 164, Ability = A2 }, // Noctowl
            new EncounterStatic8N(Nest107,3,4,4) { Species = 521, Ability = A2 }, // Unfezant
            new EncounterStatic8N(Nest107,3,4,4) { Species = 663, Ability = A2 }, // Talonflame
            new EncounterStatic8N(Nest107,3,4,4) { Species = 587, Ability = A2 }, // Emolga
            new EncounterStatic8N(Nest107,4,4,4) { Species = 663, Ability = A2 }, // Talonflame
            new EncounterStatic8N(Nest108,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest108,0,1,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest108,1,2,2) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest108,2,3,3) { Species = 558, Ability = A3 }, // Crustle
            new EncounterStatic8N(Nest108,3,4,4) { Species = 123, Ability = A4 }, // Scyther
            new EncounterStatic8N(Nest108,4,4,4) { Species = 826, Ability = A4 }, // Orbeetle
            new EncounterStatic8N(Nest108,4,4,4) { Species = 212, Ability = A4 }, // Scizor
            new EncounterStatic8N(Nest109,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest109,0,1,1) { Species = 123, Ability = A2 }, // Scyther
            new EncounterStatic8N(Nest109,1,2,2) { Species = 213, Ability = A2 }, // Shuckle
            new EncounterStatic8N(Nest109,1,2,2) { Species = 544, Ability = A2 }, // Whirlipede
            new EncounterStatic8N(Nest109,2,3,3) { Species = 123, Ability = A2 }, // Scyther
            new EncounterStatic8N(Nest109,2,3,3) { Species = 558, Ability = A2 }, // Crustle
            new EncounterStatic8N(Nest109,3,4,4) { Species = 545, Ability = A2 }, // Scolipede
            new EncounterStatic8N(Nest109,3,4,4) { Species = 617, Ability = A2 }, // Accelgor
            new EncounterStatic8N(Nest109,3,4,4) { Species = 589, Ability = A2 }, // Escavalier
            new EncounterStatic8N(Nest109,4,4,4) { Species = 212, Ability = A2 }, // Scizor
            new EncounterStatic8N(Nest110,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest110,0,1,1) { Species = 590, Ability = A3 }, // Foongus
            new EncounterStatic8N(Nest110,0,1,1) { Species = 753, Ability = A3 }, // Fomantis
            new EncounterStatic8N(Nest110,1,2,2) { Species = 548, Ability = A3 }, // Petilil
            new EncounterStatic8N(Nest110,1,2,2) { Species = 754, Ability = A3 }, // Lurantis
            new EncounterStatic8N(Nest110,2,3,3) { Species = 591, Ability = A3 }, // Amoonguss
            new EncounterStatic8N(Nest110,2,3,3) { Species = 114, Ability = A3 }, // Tangela
            new EncounterStatic8N(Nest110,3,4,4) { Species = 549, Ability = A4 }, // Lilligant
            new EncounterStatic8N(Nest110,3,4,4) { Species = 754, Ability = A4 }, // Lurantis
            new EncounterStatic8N(Nest110,4,4,4) { Species = 591, Ability = A4 }, // Amoonguss
            new EncounterStatic8N(Nest110,4,4,4) { Species = 465, Ability = A4 }, // Tangrowth
            new EncounterStatic8N(Nest111,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest111,0,1,1) { Species = 114, Ability = A2 }, // Tangela
            new EncounterStatic8N(Nest111,0,1,1) { Species = 753, Ability = A2 }, // Fomantis
            new EncounterStatic8N(Nest111,1,2,2) { Species = 590, Ability = A2 }, // Foongus
            new EncounterStatic8N(Nest111,1,2,2) { Species = 754, Ability = A2 }, // Lurantis
            new EncounterStatic8N(Nest111,2,3,3) { Species = 556, Ability = A2 }, // Maractus
            new EncounterStatic8N(Nest111,2,3,3) { Species = 549, Ability = A2 }, // Lilligant
            new EncounterStatic8N(Nest111,3,4,4) { Species = 754, Ability = A2 }, // Lurantis
            new EncounterStatic8N(Nest111,3,4,4) { Species = 591, Ability = A2 }, // Amoonguss
            new EncounterStatic8N(Nest111,3,4,4) { Species = 465, Ability = A2 }, // Tangrowth
            new EncounterStatic8N(Nest111,4,4,4) { Species = 549, Ability = A2 }, // Lilligant
            new EncounterStatic8N(Nest111,4,4,4) { Species = 460, Ability = A2 }, // Abomasnow
            new EncounterStatic8N(Nest112,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest112,0,1,1) { Species = 661, Ability = A3 }, // Fletchling
            new EncounterStatic8N(Nest112,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest112,1,2,2) { Species = 636, Ability = A3 }, // Larvesta
            new EncounterStatic8N(Nest112,1,2,2) { Species = 757, Ability = A3, Gender = 1 }, // Salandit
            new EncounterStatic8N(Nest112,2,3,3) { Species = 662, Ability = A3 }, // Fletchinder
            new EncounterStatic8N(Nest112,2,3,3) { Species = 636, Ability = A3 }, // Larvesta
            new EncounterStatic8N(Nest112,3,4,4) { Species = 324, Ability = A4 }, // Torkoal
            new EncounterStatic8N(Nest112,3,4,4) { Species = 663, Ability = A4 }, // Talonflame
            new EncounterStatic8N(Nest112,3,4,4) { Species = 758, Ability = A4 }, // Salazzle
            new EncounterStatic8N(Nest112,4,4,4) { Species = 324, Ability = A4 }, // Torkoal
            new EncounterStatic8N(Nest112,4,4,4) { Species = 637, Ability = A4 }, // Volcarona
            new EncounterStatic8N(Nest113,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest113,0,1,1) { Species = 636, Ability = A2 }, // Larvesta
            new EncounterStatic8N(Nest113,0,1,1) { Species = 607, Ability = A2 }, // Litwick
            new EncounterStatic8N(Nest113,1,2,2) { Species = 636, Ability = A2 }, // Larvesta
            new EncounterStatic8N(Nest113,1,2,2) { Species = 757, Ability = A2, Gender = 1 }, // Salandit
            new EncounterStatic8N(Nest113,2,3,3) { Species = 324, Ability = A2 }, // Torkoal
            new EncounterStatic8N(Nest113,2,3,3) { Species = 758, Ability = A2 }, // Salazzle
            new EncounterStatic8N(Nest113,3,4,4) { Species = 663, Ability = A2 }, // Talonflame
            new EncounterStatic8N(Nest113,3,4,4) { Species = 609, Ability = A2 }, // Chandelure
            new EncounterStatic8N(Nest113,3,4,4) { Species = 637, Ability = A2 }, // Volcarona
            new EncounterStatic8N(Nest113,4,4,4) { Species = 006, Ability = A2 }, // Charizard
            new EncounterStatic8N(Nest114,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest114,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest114,0,1,1) { Species = 111, Ability = A3 }, // Rhyhorn
            new EncounterStatic8N(Nest114,1,2,2) { Species = 744, Ability = A3 }, // Rockruff
            new EncounterStatic8N(Nest114,1,2,2) { Species = 525, Ability = A3 }, // Boldore
            new EncounterStatic8N(Nest114,2,3,3) { Species = 112, Ability = A3 }, // Rhydon
            new EncounterStatic8N(Nest114,2,3,3) { Species = 558, Ability = A3 }, // Crustle
            new EncounterStatic8N(Nest114,3,4,4) { Species = 112, Ability = A4 }, // Rhydon
            new EncounterStatic8N(Nest114,3,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest114,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest114,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest115,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest115,0,1,1) { Species = 744, Ability = A2 }, // Rockruff
            new EncounterStatic8N(Nest115,0,1,1) { Species = 438, Ability = A2 }, // Bonsly
            new EncounterStatic8N(Nest115,1,2,2) { Species = 111, Ability = A2 }, // Rhyhorn
            new EncounterStatic8N(Nest115,1,2,2) { Species = 744, Ability = A2 }, // Rockruff
            new EncounterStatic8N(Nest115,2,3,3) { Species = 112, Ability = A2 }, // Rhydon
            new EncounterStatic8N(Nest115,2,3,3) { Species = 213, Ability = A2 }, // Shuckle
            new EncounterStatic8N(Nest115,3,4,4) { Species = 185, Ability = A2 }, // Sudowoodo
            new EncounterStatic8N(Nest115,3,4,4) { Species = 526, Ability = A2 }, // Gigalith
            new EncounterStatic8N(Nest115,4,4,4) { Species = 558, Ability = A2 }, // Crustle
            new EncounterStatic8N(Nest115,4,4,4) { Species = 464, Ability = A2 }, // Rhyperior
            new EncounterStatic8N(Nest116,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest116,0,1,1) { Species = 102, Ability = A3 }, // Exeggcute
            new EncounterStatic8N(Nest116,0,1,1) { Species = 063, Ability = A3 }, // Abra
            new EncounterStatic8N(Nest116,1,2,2) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest116,1,2,2) { Species = 064, Ability = A3 }, // Kadabra
            new EncounterStatic8N(Nest116,2,3,3) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest116,3,4,4) { Species = 103, Ability = A4 }, // Exeggutor
            new EncounterStatic8N(Nest116,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest116,4,4,4) { Species = 065, Ability = A4 }, // Alakazam
            new EncounterStatic8N(Nest116,4,4,4) { Species = 121, Ability = A4 }, // Starmie
            new EncounterStatic8N(Nest117,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest117,0,1,1) { Species = 605, Ability = A2 }, // Elgyem
            new EncounterStatic8N(Nest117,0,1,1) { Species = 063, Ability = A2 }, // Abra
            new EncounterStatic8N(Nest117,1,2,2) { Species = 079, Ability = A2, Form = 1 }, // Slowpoke-1
            new EncounterStatic8N(Nest117,1,2,2) { Species = 605, Ability = A2 }, // Elgyem
            new EncounterStatic8N(Nest117,2,3,3) { Species = 079, Ability = A2, Form = 1 }, // Slowpoke-1
            new EncounterStatic8N(Nest117,3,4,4) { Species = 518, Ability = A2 }, // Musharna
            new EncounterStatic8N(Nest117,3,4,4) { Species = 606, Ability = A2 }, // Beheeyem
            new EncounterStatic8N(Nest117,4,4,4) { Species = 065, Ability = A2 }, // Alakazam
            new EncounterStatic8N(Nest118,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest118,0,1,1) { Species = 543, Ability = A3 }, // Venipede
            new EncounterStatic8N(Nest118,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest118,1,2,2) { Species = 072, Ability = A3 }, // Tentacool
            new EncounterStatic8N(Nest118,2,3,3) { Species = 544, Ability = A3 }, // Whirlipede
            new EncounterStatic8N(Nest118,3,4,4) { Species = 452, Ability = A4 }, // Drapion
            new EncounterStatic8N(Nest118,3,4,4) { Species = 073, Ability = A4 }, // Tentacruel
            new EncounterStatic8N(Nest118,4,4,4) { Species = 073, Ability = A4 }, // Tentacruel
            new EncounterStatic8N(Nest118,4,4,4) { Species = 545, Ability = A4 }, // Scolipede
            new EncounterStatic8N(Nest119,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest119,0,1,1) { Species = 747, Ability = A2 }, // Mareanie
            new EncounterStatic8N(Nest119,0,1,1) { Species = 211, Ability = A2 }, // Qwilfish
            new EncounterStatic8N(Nest119,1,2,2) { Species = 544, Ability = A2 }, // Whirlipede
            new EncounterStatic8N(Nest119,2,3,3) { Species = 211, Ability = A2 }, // Qwilfish
            new EncounterStatic8N(Nest119,2,3,3) { Species = 591, Ability = A2 }, // Amoonguss
            new EncounterStatic8N(Nest119,3,4,4) { Species = 748, Ability = A2 }, // Toxapex
            new EncounterStatic8N(Nest119,3,4,4) { Species = 545, Ability = A2 }, // Scolipede
            new EncounterStatic8N(Nest119,3,4,4) { Species = 452, Ability = A2 }, // Drapion
            new EncounterStatic8N(Nest119,4,4,4) { Species = 110, Ability = A2, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest119,4,4,4) { Species = 545, Ability = A2 }, // Scolipede
            new EncounterStatic8N(Nest120,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest120,0,1,1) { Species = 318, Ability = A3 }, // Carvanha
            new EncounterStatic8N(Nest120,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest120,1,2,2) { Species = 318, Ability = A3 }, // Carvanha
            new EncounterStatic8N(Nest120,1,2,2) { Species = 570, Ability = A3 }, // Zorua
            new EncounterStatic8N(Nest120,2,3,3) { Species = 319, Ability = A3 }, // Sharpedo
            new EncounterStatic8N(Nest120,2,3,3) { Species = 687, Ability = A3 }, // Malamar
            new EncounterStatic8N(Nest120,3,4,4) { Species = 452, Ability = A4 }, // Drapion
            new EncounterStatic8N(Nest120,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest120,3,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest120,4,4,4) { Species = 319, Ability = A4 }, // Sharpedo
            new EncounterStatic8N(Nest120,4,4,4) { Species = 571, Ability = A4 }, // Zoroark
            new EncounterStatic8N(Nest121,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest121,0,1,1) { Species = 570, Ability = A2 }, // Zorua
            new EncounterStatic8N(Nest121,0,1,1) { Species = 318, Ability = A2 }, // Carvanha
            new EncounterStatic8N(Nest121,1,2,2) { Species = 570, Ability = A2 }, // Zorua
            new EncounterStatic8N(Nest121,1,2,2) { Species = 686, Ability = A2 }, // Inkay
            new EncounterStatic8N(Nest121,2,3,3) { Species = 552, Ability = A2 }, // Krokorok
            new EncounterStatic8N(Nest121,2,3,3) { Species = 687, Ability = A2 }, // Malamar
            new EncounterStatic8N(Nest121,3,4,4) { Species = 828, Ability = A2 }, // Thievul
            new EncounterStatic8N(Nest121,3,4,4) { Species = 571, Ability = A2 }, // Zoroark
            new EncounterStatic8N(Nest121,3,4,4) { Species = 319, Ability = A2 }, // Sharpedo
            new EncounterStatic8N(Nest121,4,4,4) { Species = 510, Ability = A2 }, // Liepard
            new EncounterStatic8N(Nest121,4,4,4) { Species = 553, Ability = A2 }, // Krookodile
            new EncounterStatic8N(Nest122,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest122,0,1,1) { Species = 619, Ability = A3 }, // Mienfoo
            new EncounterStatic8N(Nest122,0,1,1) { Species = 852, Ability = A3 }, // Clobbopus
            new EncounterStatic8N(Nest122,1,2,2) { Species = 619, Ability = A3 }, // Mienfoo
            new EncounterStatic8N(Nest122,3,4,4) { Species = 620, Ability = A4 }, // Mienshao
            new EncounterStatic8N(Nest122,4,4,4) { Species = 853, Ability = A4 }, // Grapploct
            new EncounterStatic8N(Nest122,4,4,4) { Species = 620, Ability = A4 }, // Mienshao
            new EncounterStatic8N(Nest123,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest123,0,1,1) { Species = 619, Ability = A2 }, // Mienfoo
            new EncounterStatic8N(Nest123,1,2,2) { Species = 620, Ability = A2 }, // Mienshao
            new EncounterStatic8N(Nest123,2,3,3) { Species = 870, Ability = A2 }, // Falinks
            new EncounterStatic8N(Nest123,3,4,4) { Species = 620, Ability = A2 }, // Mienshao
            new EncounterStatic8N(Nest123,4,4,4) { Species = 853, Ability = A2 }, // Grapploct
            new EncounterStatic8N(Nest124,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest124,0,1,1) { Species = 174, Ability = A3 }, // Igglybuff
            new EncounterStatic8N(Nest124,0,1,1) { Species = 298, Ability = A3 }, // Azurill
            new EncounterStatic8N(Nest124,1,2,2) { Species = 764, Ability = A3 }, // Comfey
            new EncounterStatic8N(Nest124,1,2,2) { Species = 039, Ability = A3 }, // Jigglypuff
            new EncounterStatic8N(Nest124,2,3,3) { Species = 183, Ability = A3 }, // Marill
            new EncounterStatic8N(Nest124,2,3,3) { Species = 764, Ability = A3 }, // Comfey
            new EncounterStatic8N(Nest124,3,4,4) { Species = 707, Ability = A4 }, // Klefki
            new EncounterStatic8N(Nest124,3,4,4) { Species = 184, Ability = A4 }, // Azumarill
            new EncounterStatic8N(Nest124,3,4,4) { Species = 040, Ability = A4 }, // Wigglytuff
            new EncounterStatic8N(Nest124,4,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest124,4,4,4) { Species = 764, Ability = A4 }, // Comfey
            new EncounterStatic8N(Nest125,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest125,0,1,1) { Species = 173, Ability = A2 }, // Cleffa
            new EncounterStatic8N(Nest125,0,1,1) { Species = 755, Ability = A2 }, // Morelull
            new EncounterStatic8N(Nest125,1,2,2) { Species = 183, Ability = A2 }, // Marill
            new EncounterStatic8N(Nest125,1,2,2) { Species = 035, Ability = A2 }, // Clefairy
            new EncounterStatic8N(Nest125,2,3,3) { Species = 281, Ability = A2 }, // Kirlia
            new EncounterStatic8N(Nest125,2,3,3) { Species = 707, Ability = A2 }, // Klefki
            new EncounterStatic8N(Nest125,3,4,4) { Species = 764, Ability = A2 }, // Comfey
            new EncounterStatic8N(Nest125,3,4,4) { Species = 036, Ability = A2 }, // Clefable
            new EncounterStatic8N(Nest125,3,4,4) { Species = 282, Ability = A2 }, // Gardevoir
            new EncounterStatic8N(Nest125,4,4,4) { Species = 756, Ability = A2 }, // Shiinotic
            new EncounterStatic8N(Nest125,4,4,4) { Species = 184, Ability = A2 }, // Azumarill
            new EncounterStatic8N(Nest126,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest126,0,1,1) { Species = 769, Ability = A3 }, // Sandygast
            new EncounterStatic8N(Nest126,0,1,1) { Species = 592, Ability = A3 }, // Frillish
            new EncounterStatic8N(Nest126,1,2,2) { Species = 104, Ability = A3 }, // Cubone
            new EncounterStatic8N(Nest126,1,2,2) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest126,2,3,3) { Species = 593, Ability = A3 }, // Jellicent
            new EncounterStatic8N(Nest126,2,3,3) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest126,3,4,4) { Species = 770, Ability = A4 }, // Palossand
            new EncounterStatic8N(Nest126,3,4,4) { Species = 593, Ability = A4 }, // Jellicent
            new EncounterStatic8N(Nest126,3,4,4) { Species = 426, Ability = A4 }, // Drifblim
            new EncounterStatic8N(Nest126,4,4,4) { Species = 105, Ability = A4 }, // Marowak
            new EncounterStatic8N(Nest126,4,4,4) { Species = 770, Ability = A4 }, // Palossand
            new EncounterStatic8N(Nest127,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest127,0,1,1) { Species = 769, Ability = A2 }, // Sandygast
            new EncounterStatic8N(Nest127,0,1,1) { Species = 592, Ability = A2 }, // Frillish
            new EncounterStatic8N(Nest127,1,2,2) { Species = 769, Ability = A2 }, // Sandygast
            new EncounterStatic8N(Nest127,1,2,2) { Species = 425, Ability = A2 }, // Drifloon
            new EncounterStatic8N(Nest127,2,3,3) { Species = 593, Ability = A2 }, // Jellicent
            new EncounterStatic8N(Nest127,2,3,3) { Species = 426, Ability = A2 }, // Drifblim
            new EncounterStatic8N(Nest127,3,4,4) { Species = 711, Ability = A2 }, // Gourgeist
            new EncounterStatic8N(Nest127,3,4,4) { Species = 711, Ability = A2, Form = 1 }, // Gourgeist-1
            new EncounterStatic8N(Nest127,3,4,4) { Species = 711, Ability = A2, Form = 2 }, // Gourgeist-2
            new EncounterStatic8N(Nest127,4,4,4) { Species = 711, Ability = A2, Form = 3 }, // Gourgeist-3
            new EncounterStatic8N(Nest128,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest128,0,1,1) { Species = 707, Ability = A3 }, // Klefki
            new EncounterStatic8N(Nest128,0,1,1) { Species = 081, Ability = A3 }, // Magnemite
            new EncounterStatic8N(Nest128,1,2,2) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest128,1,2,2) { Species = 081, Ability = A3 }, // Magnemite
            new EncounterStatic8N(Nest128,2,3,3) { Species = 227, Ability = A3 }, // Skarmory
            new EncounterStatic8N(Nest128,2,3,3) { Species = 082, Ability = A3 }, // Magneton
            new EncounterStatic8N(Nest128,3,4,4) { Species = 082, Ability = A4 }, // Magneton
            new EncounterStatic8N(Nest128,3,4,4) { Species = 707, Ability = A4 }, // Klefki
            new EncounterStatic8N(Nest128,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest128,4,4,4) { Species = 462, Ability = A4 }, // Magnezone
            new EncounterStatic8N(Nest128,4,4,4) { Species = 227, Ability = A4 }, // Skarmory
            new EncounterStatic8N(Nest129,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest129,0,1,1) { Species = 081, Ability = A2 }, // Magnemite
            new EncounterStatic8N(Nest129,0,1,1) { Species = 227, Ability = A2 }, // Skarmory
            new EncounterStatic8N(Nest129,1,2,2) { Species = 436, Ability = A2 }, // Bronzor
            new EncounterStatic8N(Nest129,1,2,2) { Species = 052, Ability = A2, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest129,2,3,3) { Species = 082, Ability = A2 }, // Magneton
            new EncounterStatic8N(Nest129,2,3,3) { Species = 601, Ability = A2 }, // Klinklang
            new EncounterStatic8N(Nest129,3,4,4) { Species = 227, Ability = A2 }, // Skarmory
            new EncounterStatic8N(Nest129,3,4,4) { Species = 437, Ability = A2 }, // Bronzong
            new EncounterStatic8N(Nest129,3,4,4) { Species = 863, Ability = A2 }, // Perrserker
            new EncounterStatic8N(Nest129,4,4,4) { Species = 448, Ability = A2 }, // Lucario
            new EncounterStatic8N(Nest129,4,4,4) { Species = 625, Ability = A2 }, // Bisharp
            new EncounterStatic8N(Nest130,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest130,0,1,1) { Species = 116, Ability = A3 }, // Horsea
            new EncounterStatic8N(Nest130,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest130,1,2,2) { Species = 117, Ability = A3 }, // Seadra
            new EncounterStatic8N(Nest130,2,3,3) { Species = 621, Ability = A3 }, // Druddigon
            new EncounterStatic8N(Nest130,3,4,4) { Species = 621, Ability = A4 }, // Druddigon
            new EncounterStatic8N(Nest130,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest130,4,4,4) { Species = 230, Ability = A4 }, // Kingdra
            new EncounterStatic8N(Nest131,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest131,0,1,1) { Species = 116, Ability = A2 }, // Horsea
            new EncounterStatic8N(Nest131,0,1,1) { Species = 621, Ability = A2 }, // Druddigon
            new EncounterStatic8N(Nest131,2,3,3) { Species = 117, Ability = A2 }, // Seadra
            new EncounterStatic8N(Nest131,3,4,4) { Species = 621, Ability = A2 }, // Druddigon
            new EncounterStatic8N(Nest131,3,4,4) { Species = 715, Ability = A2 }, // Noivern
            new EncounterStatic8N(Nest131,4,4,4) { Species = 230, Ability = A2 }, // Kingdra
            new EncounterStatic8N(Nest132,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest132,0,1,1) { Species = 060, Ability = A3 }, // Poliwag
            new EncounterStatic8N(Nest132,0,1,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest132,1,2,2) { Species = 118, Ability = A3 }, // Goldeen
            new EncounterStatic8N(Nest132,1,2,2) { Species = 061, Ability = A3 }, // Poliwhirl
            new EncounterStatic8N(Nest132,2,3,3) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest132,2,3,3) { Species = 061, Ability = A3 }, // Poliwhirl
            new EncounterStatic8N(Nest132,3,4,4) { Species = 119, Ability = A4 }, // Seaking
            new EncounterStatic8N(Nest132,3,4,4) { Species = 342, Ability = A4 }, // Crawdaunt
            new EncounterStatic8N(Nest132,3,4,4) { Species = 195, Ability = A4 }, // Quagsire
            new EncounterStatic8N(Nest132,4,4,4) { Species = 062, Ability = A4 }, // Poliwrath
            new EncounterStatic8N(Nest132,4,4,4) { Species = 186, Ability = A4 }, // Politoed
            new EncounterStatic8N(Nest133,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest133,0,1,1) { Species = 341, Ability = A2 }, // Corphish
            new EncounterStatic8N(Nest133,0,1,1) { Species = 751, Ability = A2 }, // Dewpider
            new EncounterStatic8N(Nest133,1,2,2) { Species = 118, Ability = A2 }, // Goldeen
            new EncounterStatic8N(Nest133,1,2,2) { Species = 061, Ability = A2 }, // Poliwhirl
            new EncounterStatic8N(Nest133,2,3,3) { Species = 342, Ability = A2 }, // Crawdaunt
            new EncounterStatic8N(Nest133,2,3,3) { Species = 195, Ability = A2 }, // Quagsire
            new EncounterStatic8N(Nest133,3,4,4) { Species = 119, Ability = A2 }, // Seaking
            new EncounterStatic8N(Nest133,3,4,4) { Species = 062, Ability = A2 }, // Poliwrath
            new EncounterStatic8N(Nest133,3,4,4) { Species = 342, Ability = A2 }, // Crawdaunt
            new EncounterStatic8N(Nest133,4,4,4) { Species = 752, Ability = A2 }, // Araquanid
            new EncounterStatic8N(Nest133,4,4,4) { Species = 186, Ability = A2 }, // Politoed
            new EncounterStatic8N(Nest134,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest134,0,1,1) { Species = 054, Ability = A3 }, // Psyduck
            new EncounterStatic8N(Nest134,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest134,1,2,2) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest134,1,2,2) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest134,2,3,3) { Species = 055, Ability = A3 }, // Golduck
            new EncounterStatic8N(Nest134,2,3,3) { Species = 845, Ability = A3 }, // Cramorant
            new EncounterStatic8N(Nest134,3,4,4) { Species = 055, Ability = A4 }, // Golduck
            new EncounterStatic8N(Nest134,3,4,4) { Species = 847, Ability = A4 }, // Barraskewda
            new EncounterStatic8N(Nest134,3,4,4) { Species = 834, Ability = A4 }, // Drednaw
            new EncounterStatic8N(Nest134,4,4,4) { Species = 340, Ability = A4 }, // Whiscash
            new EncounterStatic8N(Nest134,4,4,4) { Species = 055, Ability = A4 }, // Golduck
            new EncounterStatic8N(Nest135,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest135,0,1,1) { Species = 846, Ability = A2 }, // Arrokuda
            new EncounterStatic8N(Nest135,0,1,1) { Species = 535, Ability = A2 }, // Tympole
            new EncounterStatic8N(Nest135,1,2,2) { Species = 054, Ability = A2 }, // Psyduck
            new EncounterStatic8N(Nest135,1,2,2) { Species = 536, Ability = A2 }, // Palpitoad
            new EncounterStatic8N(Nest135,2,3,3) { Species = 055, Ability = A2 }, // Golduck
            new EncounterStatic8N(Nest135,2,3,3) { Species = 340, Ability = A2 }, // Whiscash
            new EncounterStatic8N(Nest135,3,4,4) { Species = 055, Ability = A2 }, // Golduck
            new EncounterStatic8N(Nest135,3,4,4) { Species = 847, Ability = A2 }, // Barraskewda
            new EncounterStatic8N(Nest135,3,4,4) { Species = 537, Ability = A2 }, // Seismitoad
            new EncounterStatic8N(Nest135,4,4,4) { Species = 130, Ability = A2 }, // Gyarados
            new EncounterStatic8N(Nest136,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest136,0,1,1) { Species = 072, Ability = A3 }, // Tentacool
            new EncounterStatic8N(Nest136,0,1,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest136,1,2,2) { Species = 072, Ability = A3 }, // Tentacool
            new EncounterStatic8N(Nest136,1,2,2) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest136,2,3,3) { Species = 073, Ability = A3 }, // Tentacruel
            new EncounterStatic8N(Nest136,2,3,3) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest136,3,4,4) { Species = 224, Ability = A4 }, // Octillery
            new EncounterStatic8N(Nest136,3,4,4) { Species = 226, Ability = A4 }, // Mantine
            new EncounterStatic8N(Nest136,3,4,4) { Species = 099, Ability = A4 }, // Kingler
            new EncounterStatic8N(Nest136,4,4,4) { Species = 091, Ability = A4 }, // Cloyster
            new EncounterStatic8N(Nest136,4,4,4) { Species = 073, Ability = A4 }, // Tentacruel
            new EncounterStatic8N(Nest137,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest137,0,1,1) { Species = 090, Ability = A2 }, // Shellder
            new EncounterStatic8N(Nest137,0,1,1) { Species = 688, Ability = A2 }, // Binacle
            new EncounterStatic8N(Nest137,1,2,2) { Species = 747, Ability = A2 }, // Mareanie
            new EncounterStatic8N(Nest137,1,2,2) { Species = 223, Ability = A2 }, // Remoraid
            new EncounterStatic8N(Nest137,2,3,3) { Species = 073, Ability = A2 }, // Tentacruel
            new EncounterStatic8N(Nest137,2,3,3) { Species = 771, Ability = A2 }, // Pyukumuku
            new EncounterStatic8N(Nest137,3,4,4) { Species = 224, Ability = A2 }, // Octillery
            new EncounterStatic8N(Nest137,3,4,4) { Species = 226, Ability = A2 }, // Mantine
            new EncounterStatic8N(Nest137,3,4,4) { Species = 689, Ability = A2 }, // Barbaracle
            new EncounterStatic8N(Nest137,4,4,4) { Species = 091, Ability = A2 }, // Cloyster
            new EncounterStatic8N(Nest137,4,4,4) { Species = 748, Ability = A2 }, // Toxapex
            new EncounterStatic8N(Nest138,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest138,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest138,1,2,2) { Species = 120, Ability = A3 }, // Staryu
            new EncounterStatic8N(Nest138,2,3,3) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest138,2,3,3) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest138,3,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest138,3,4,4) { Species = 171, Ability = A4 }, // Lanturn
            new EncounterStatic8N(Nest138,3,4,4) { Species = 121, Ability = A4 }, // Starmie
            new EncounterStatic8N(Nest138,4,4,4) { Species = 319, Ability = A4 }, // Sharpedo
            new EncounterStatic8N(Nest139,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest139,0,1,1) { Species = 120, Ability = A2 }, // Staryu
            new EncounterStatic8N(Nest139,1,2,2) { Species = 320, Ability = A2 }, // Wailmer
            new EncounterStatic8N(Nest139,1,2,2) { Species = 279, Ability = A2 }, // Pelipper
            new EncounterStatic8N(Nest139,2,3,3) { Species = 171, Ability = A2 }, // Lanturn
            new EncounterStatic8N(Nest139,2,3,3) { Species = 117, Ability = A2 }, // Seadra
            new EncounterStatic8N(Nest139,3,4,4) { Species = 171, Ability = A2 }, // Lanturn
            new EncounterStatic8N(Nest139,3,4,4) { Species = 121, Ability = A2 }, // Starmie
            new EncounterStatic8N(Nest139,4,4,4) { Species = 319, Ability = A2 }, // Sharpedo
            new EncounterStatic8N(Nest140,0,0,1) { Species = 440, Ability = A3 }, // Happiny
            new EncounterStatic8N(Nest140,0,1,1) { Species = 440, Ability = A3 }, // Happiny
            new EncounterStatic8N(Nest140,1,2,2) { Species = 440, Ability = A3 }, // Happiny
            new EncounterStatic8N(Nest140,2,3,3) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest140,3,4,4) { Species = 113, Ability = A4 }, // Chansey
            new EncounterStatic8N(Nest140,4,4,4) { Species = 242, Ability = A4 }, // Blissey
            new EncounterStatic8N(Nest141,0,0,1) { Species = 113, Ability = A2 }, // Chansey
            new EncounterStatic8N(Nest141,0,1,1) { Species = 113, Ability = A2 }, // Chansey
            new EncounterStatic8N(Nest141,1,2,2) { Species = 113, Ability = A2 }, // Chansey
            new EncounterStatic8N(Nest141,2,3,3) { Species = 113, Ability = A2 }, // Chansey
            new EncounterStatic8N(Nest141,3,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest141,4,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest142,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest142,0,1,1) { Species = 415, Ability = A3 }, // Combee
            new EncounterStatic8N(Nest142,1,2,2) { Species = 415, Ability = A3 }, // Combee
            new EncounterStatic8N(Nest142,2,3,3) { Species = 415, Ability = A3 }, // Combee
            new EncounterStatic8N(Nest142,3,4,4) { Species = 416, Ability = A4 }, // Vespiquen
            new EncounterStatic8N(Nest142,4,4,4) { Species = 416, Ability = A4 }, // Vespiquen
            new EncounterStatic8N(Nest143,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest143,0,1,1) { Species = 415, Ability = A2, Gender = 1 }, // Combee
            new EncounterStatic8N(Nest143,1,2,2) { Species = 415, Ability = A2, Gender = 1 }, // Combee
            new EncounterStatic8N(Nest143,2,3,3) { Species = 416, Ability = A2 }, // Vespiquen
            new EncounterStatic8N(Nest143,3,4,4) { Species = 416, Ability = A2 }, // Vespiquen
            new EncounterStatic8N(Nest143,4,4,4) { Species = 416, Ability = A2 }, // Vespiquen
            new EncounterStatic8N(Nest144,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest144,0,1,1) { Species = 590, Ability = A2 }, // Foongus
            new EncounterStatic8N(Nest144,0,1,1) { Species = 102, Ability = A2 }, // Exeggcute
            new EncounterStatic8N(Nest144,1,2,2) { Species = 114, Ability = A2 }, // Tangela
            new EncounterStatic8N(Nest144,1,2,2) { Species = 315, Ability = A2 }, // Roselia
            new EncounterStatic8N(Nest144,2,3,3) { Species = 114, Ability = A2 }, // Tangela
            new EncounterStatic8N(Nest144,2,3,3) { Species = 315, Ability = A2 }, // Roselia
            new EncounterStatic8N(Nest144,3,4,4) { Species = 103, Ability = A2 }, // Exeggutor
            new EncounterStatic8N(Nest144,3,4,4) { Species = 003, Ability = A2 }, // Venusaur
            new EncounterStatic8N(Nest144,3,4,4) { Species = 465, Ability = A2 }, // Tangrowth
            new EncounterStatic8N(Nest144,4,4,4) { Species = 407, Ability = A2 }, // Roserade
            new EncounterStatic8N(Nest144,4,4,4) { Species = 003, Ability = A2, CanGigantamax = true }, // Venusaur
            new EncounterStatic8N(Nest145,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest145,0,1,1) { Species = 129, Ability = A2 }, // Magikarp
            new EncounterStatic8N(Nest145,0,1,1) { Species = 072, Ability = A2 }, // Tentacool
            new EncounterStatic8N(Nest145,1,2,2) { Species = 120, Ability = A2 }, // Staryu
            new EncounterStatic8N(Nest145,1,2,2) { Species = 688, Ability = A2 }, // Binacle
            new EncounterStatic8N(Nest145,2,3,3) { Species = 073, Ability = A2 }, // Tentacruel
            new EncounterStatic8N(Nest145,2,3,3) { Species = 130, Ability = A2 }, // Gyarados
            new EncounterStatic8N(Nest145,3,4,4) { Species = 073, Ability = A2 }, // Tentacruel
            new EncounterStatic8N(Nest145,3,4,4) { Species = 130, Ability = A2 }, // Gyarados
            new EncounterStatic8N(Nest145,3,4,4) { Species = 121, Ability = A2 }, // Starmie
            new EncounterStatic8N(Nest145,4,4,4) { Species = 689, Ability = A2 }, // Barbaracle
            new EncounterStatic8N(Nest145,4,4,4) { Species = 009, Ability = A2, CanGigantamax = true }, // Blastoise
            new EncounterStatic8N(Nest146,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest146,0,1,1) { Species = 098, Ability = A2 }, // Krabby
            new EncounterStatic8N(Nest146,0,1,1) { Species = 688, Ability = A2 }, // Binacle
            new EncounterStatic8N(Nest146,1,2,2) { Species = 072, Ability = A2 }, // Tentacool
            new EncounterStatic8N(Nest146,1,2,2) { Species = 223, Ability = A2 }, // Remoraid
            new EncounterStatic8N(Nest146,2,3,3) { Species = 073, Ability = A2 }, // Tentacruel
            new EncounterStatic8N(Nest146,2,3,3) { Species = 224, Ability = A2 }, // Octillery
            new EncounterStatic8N(Nest146,3,4,4) { Species = 713, Ability = A2 }, // Avalugg
            new EncounterStatic8N(Nest146,3,4,4) { Species = 614, Ability = A2 }, // Beartic
            new EncounterStatic8N(Nest146,3,4,4) { Species = 099, Ability = A2 }, // Kingler
            new EncounterStatic8N(Nest146,4,4,4) { Species = 091, Ability = A2 }, // Cloyster
            new EncounterStatic8N(Nest146,4,4,4) { Species = 099, Ability = A2, CanGigantamax = true }, // Kingler
            new EncounterStatic8N(Nest147,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest147,0,1,1) { Species = 833, Ability = A2 }, // Chewtle
            new EncounterStatic8N(Nest147,0,1,1) { Species = 054, Ability = A2 }, // Psyduck
            new EncounterStatic8N(Nest147,1,2,2) { Species = 339, Ability = A2 }, // Barboach
            new EncounterStatic8N(Nest147,2,3,3) { Species = 055, Ability = A2 }, // Golduck
            new EncounterStatic8N(Nest147,2,3,3) { Species = 845, Ability = A2 }, // Cramorant
            new EncounterStatic8N(Nest147,3,4,4) { Species = 055, Ability = A2 }, // Golduck
            new EncounterStatic8N(Nest147,3,4,4) { Species = 847, Ability = A2 }, // Barraskewda
            new EncounterStatic8N(Nest147,4,4,4) { Species = 340, Ability = A2 }, // Whiscash
            new EncounterStatic8N(Nest147,4,4,4) { Species = 834, Ability = A2, CanGigantamax = true }, // Drednaw
            new EncounterStatic8N(Nest148,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest148,0,1,1) { Species = 824, Ability = A2 }, // Blipbug
            new EncounterStatic8N(Nest148,0,1,1) { Species = 742, Ability = A2 }, // Cutiefly
            new EncounterStatic8N(Nest148,1,2,2) { Species = 595, Ability = A2 }, // Joltik
            new EncounterStatic8N(Nest148,2,3,3) { Species = 825, Ability = A2 }, // Dottler
            new EncounterStatic8N(Nest148,2,3,3) { Species = 291, Ability = A2 }, // Ninjask
            new EncounterStatic8N(Nest148,3,4,4) { Species = 826, Ability = A2 }, // Orbeetle
            new EncounterStatic8N(Nest148,3,4,4) { Species = 596, Ability = A2 }, // Galvantula
            new EncounterStatic8N(Nest148,3,4,4) { Species = 743, Ability = A2 }, // Ribombee
            new EncounterStatic8N(Nest148,4,4,4) { Species = 291, Ability = A2 }, // Ninjask
            new EncounterStatic8N(Nest148,4,4,4) { Species = 826, Ability = A2, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8N(Nest149,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest149,0,1,1) { Species = 843, Ability = A2 }, // Silicobra
            new EncounterStatic8N(Nest149,0,1,1) { Species = 529, Ability = A2 }, // Drilbur
            new EncounterStatic8N(Nest149,1,2,2) { Species = 843, Ability = A2 }, // Silicobra
            new EncounterStatic8N(Nest149,1,2,2) { Species = 529, Ability = A2 }, // Drilbur
            new EncounterStatic8N(Nest149,2,3,3) { Species = 028, Ability = A2 }, // Sandslash
            new EncounterStatic8N(Nest149,2,3,3) { Species = 552, Ability = A2 }, // Krokorok
            new EncounterStatic8N(Nest149,3,4,4) { Species = 844, Ability = A2 }, // Sandaconda
            new EncounterStatic8N(Nest149,3,4,4) { Species = 553, Ability = A2 }, // Krookodile
            new EncounterStatic8N(Nest149,3,4,4) { Species = 530, Ability = A2 }, // Excadrill
            new EncounterStatic8N(Nest149,4,4,4) { Species = 553, Ability = A2 }, // Krookodile
            new EncounterStatic8N(Nest149,4,4,4) { Species = 844, Ability = A2, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8N(Nest150,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest150,0,1,1) { Species = 840, Ability = A2 }, // Applin
            new EncounterStatic8N(Nest150,0,1,1) { Species = 420, Ability = A2 }, // Cherubi
            new EncounterStatic8N(Nest150,1,2,2) { Species = 420, Ability = A2 }, // Cherubi
            new EncounterStatic8N(Nest150,1,2,2) { Species = 840, Ability = A2 }, // Applin
            new EncounterStatic8N(Nest150,2,3,3) { Species = 762, Ability = A2 }, // Steenee
            new EncounterStatic8N(Nest150,3,4,4) { Species = 820, Ability = A2 }, // Greedent
            new EncounterStatic8N(Nest150,4,4,4) { Species = 763, Ability = A2 }, // Tsareena
            new EncounterStatic8N(Nest151,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest151,0,0,1) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest151,0,1,2) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest151,1,2,3) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest151,2,3,3) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest151,2,3,3) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest151,3,4,4) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest151,4,4,4) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest152,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest152,0,0,1) { Species = 132, Ability = A2 }, // Ditto
            new EncounterStatic8N(Nest152,0,1,2) { Species = 132, Ability = A2 }, // Ditto
            new EncounterStatic8N(Nest152,1,2,3) { Species = 132, Ability = A2 }, // Ditto
            new EncounterStatic8N(Nest152,2,3,3) { Species = 132, Ability = A2 }, // Ditto
            new EncounterStatic8N(Nest152,3,4,4) { Species = 132, Ability = A2 }, // Ditto
            new EncounterStatic8N(Nest152,4,4,4) { Species = 132, Ability = A2 }, // Ditto
            new EncounterStatic8N(Nest153,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest153,0,1,1) { Species = 590, Ability = A3 }, // Foongus
            new EncounterStatic8N(Nest153,0,1,1) { Species = 102, Ability = A3 }, // Exeggcute
            new EncounterStatic8N(Nest153,1,2,2) { Species = 753, Ability = A3 }, // Fomantis
            new EncounterStatic8N(Nest153,1,2,2) { Species = 114, Ability = A3 }, // Tangela
            new EncounterStatic8N(Nest153,2,3,3) { Species = 754, Ability = A3 }, // Lurantis
            new EncounterStatic8N(Nest153,2,3,3) { Species = 102, Ability = A3 }, // Exeggcute
            new EncounterStatic8N(Nest153,3,4,4) { Species = 103, Ability = A4 }, // Exeggutor
            new EncounterStatic8N(Nest153,3,4,4) { Species = 591, Ability = A4 }, // Amoonguss
            new EncounterStatic8N(Nest153,3,4,4) { Species = 754, Ability = A4 }, // Lurantis
            new EncounterStatic8N(Nest153,4,4,4) { Species = 465, Ability = A4 }, // Tangrowth
            new EncounterStatic8N(Nest153,4,4,4) { Species = 003, Ability = A4 }, // Venusaur
            new EncounterStatic8N(Nest154,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest154,0,1,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest154,0,1,1) { Species = 072, Ability = A3 }, // Tentacool
            new EncounterStatic8N(Nest154,1,2,2) { Species = 120, Ability = A3 }, // Staryu
            new EncounterStatic8N(Nest154,1,2,2) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest154,2,3,3) { Species = 073, Ability = A3 }, // Tentacruel
            new EncounterStatic8N(Nest154,2,3,3) { Species = 130, Ability = A3 }, // Gyarados
            new EncounterStatic8N(Nest154,3,4,4) { Species = 073, Ability = A4 }, // Tentacruel
            new EncounterStatic8N(Nest154,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest154,3,4,4) { Species = 121, Ability = A4 }, // Starmie
            new EncounterStatic8N(Nest154,4,4,4) { Species = 091, Ability = A4 }, // Cloyster
            new EncounterStatic8N(Nest154,4,4,4) { Species = 009, Ability = A4 }, // Blastoise
            new EncounterStatic8N(Nest155,2,4,4) { Species = 113, Ability = A3 }, // Chansey
            new EncounterStatic8N(Nest155,0,1,1) { Species = 744, Ability = A3 }, // Rockruff
            new EncounterStatic8N(Nest155,1,2,2) { Species = 744, Ability = A3 }, // Rockruff
            new EncounterStatic8N(Nest155,2,3,3) { Species = 744, Ability = A3 }, // Rockruff
            new EncounterStatic8N(Nest155,2,3,3) { Species = 744, Ability = A3, Form = 1 }, // Rockruff-1
            new EncounterStatic8N(Nest155,3,4,4) { Species = 745, Ability = A4 }, // Lycanroc
            new EncounterStatic8N(Nest155,3,4,4) { Species = 745, Ability = A4, Form = 1 }, // Lycanroc-1
            new EncounterStatic8N(Nest155,4,4,4) { Species = 745, Ability = A4, Form = 2 }, // Lycanroc-2
            new EncounterStatic8N(Nest156,2,4,4) { Species = 242, Ability = A2 }, // Blissey
            new EncounterStatic8N(Nest156,0,1,1) { Species = 744, Ability = A2 }, // Rockruff
            new EncounterStatic8N(Nest156,1,2,2) { Species = 744, Ability = A2 }, // Rockruff
            new EncounterStatic8N(Nest156,2,3,3) { Species = 744, Ability = A2 }, // Rockruff
            new EncounterStatic8N(Nest156,2,3,3) { Species = 744, Ability = A2, Form = 1 }, // Rockruff-1
            new EncounterStatic8N(Nest156,3,3,4) { Species = 745, Ability = A2 }, // Lycanroc
            new EncounterStatic8N(Nest156,3,3,4) { Species = 745, Ability = A2, Form = 1 }, // Lycanroc-1
            new EncounterStatic8N(Nest156,4,4,4) { Species = 745, Ability = A2 }, // Lycanroc
            new EncounterStatic8N(Nest156,4,4,4) { Species = 745, Ability = A2, Form = 1 }, // Lycanroc-1
            new EncounterStatic8N(Nest156,3,4,4) { Species = 745, Ability = A2, Form = 2 }, // Lycanroc-2
        };

        internal static readonly EncounterStatic8N[] Nest_SW =
        {
            new EncounterStatic8N(Nest000,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest000,2,3,3) { Species = 106, Ability = A3 }, // Hitmonlee
            new EncounterStatic8N(Nest000,2,4,4) { Species = 107, Ability = A3 }, // Hitmonchan
            new EncounterStatic8N(Nest000,2,4,4) { Species = 560, Ability = A3 }, // Scrafty
            new EncounterStatic8N(Nest000,3,4,4) { Species = 534, Ability = A4 }, // Conkeldurr
            new EncounterStatic8N(Nest000,4,4,4) { Species = 237, Ability = A4 }, // Hitmontop
            new EncounterStatic8N(Nest001,0,1,1) { Species = 574, Ability = A3 }, // Gothita
            new EncounterStatic8N(Nest001,2,3,3) { Species = 678, Ability = A3, Gender = 0 }, // Meowstic
            new EncounterStatic8N(Nest001,2,3,3) { Species = 575, Ability = A3 }, // Gothorita
            new EncounterStatic8N(Nest001,3,4,4) { Species = 576, Ability = A4 }, // Gothitelle
            new EncounterStatic8N(Nest001,4,4,4) { Species = 338, Ability = A4 }, // Solrock
            new EncounterStatic8N(Nest002,0,0,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest002,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest002,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest002,4,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest006,0,1,1) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest006,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest006,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest007,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest008,1,1,2) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest009,1,1,2) { Species = 083, Ability = A3, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8N(Nest009,1,2,2) { Species = 539, Ability = A3 }, // Sawk
            new EncounterStatic8N(Nest009,3,4,4) { Species = 865, Ability = A4 }, // Sirfetch’d
            new EncounterStatic8N(Nest011,4,4,4) { Species = 303, Ability = A4 }, // Mawile
            new EncounterStatic8N(Nest012,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest012,0,1,1) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest012,1,1,2) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest012,1,3,2) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest012,2,4,4) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest012,3,4,4) { Species = 561, Ability = A4 }, // Sigilyph
            new EncounterStatic8N(Nest012,4,4,4) { Species = 826, Ability = A4 }, // Orbeetle
            new EncounterStatic8N(Nest013,2,4,4) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest014,0,0,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest014,0,1,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest014,2,4,4) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest014,3,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest014,4,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest016,0,1,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest016,1,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest016,2,3,3) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest016,2,4,4) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest016,3,4,4) { Species = 450, Ability = A4 }, // Hippowdon
            new EncounterStatic8N(Nest016,4,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest017,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest017,1,1,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest017,1,2,2) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest017,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest017,2,4,4) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest017,3,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest017,4,4,4) { Species = 631, Ability = A4 }, // Heatmor
            new EncounterStatic8N(Nest017,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest018,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest018,0,1,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest018,1,2,2) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest018,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest018,2,4,4) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest018,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest018,3,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest018,4,4,4) { Species = 776, Ability = A4 }, // Turtonator
            new EncounterStatic8N(Nest019,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest019,1,1,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest019,1,2,2) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest019,2,3,3) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest019,2,4,4) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest019,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest022,0,1,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest022,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest025,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest025,1,1,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest025,2,4,4) { Species = 275, Ability = A3 }, // Shiftry
            new EncounterStatic8N(Nest026,4,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest028,0,1,1) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest028,1,1,2) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest028,3,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest028,4,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest030,0,1,1) { Species = 627, Ability = A3 }, // Rufflet
            new EncounterStatic8N(Nest030,4,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest032,0,1,1) { Species = 684, Ability = A3 }, // Swirlix
            new EncounterStatic8N(Nest032,4,4,4) { Species = 685, Ability = A4 }, // Slurpuff
            new EncounterStatic8N(Nest033,4,4,4) { Species = 303, Ability = A4 }, // Mawile
            new EncounterStatic8N(Nest034,4,4,4) { Species = 275, Ability = A4 }, // Shiftry
            new EncounterStatic8N(Nest035,1,1,2) { Species = 633, Ability = A3 }, // Deino
            new EncounterStatic8N(Nest035,3,4,4) { Species = 634, Ability = A4 }, // Zweilous
            new EncounterStatic8N(Nest035,4,4,4) { Species = 635, Ability = A4 }, // Hydreigon
            new EncounterStatic8N(Nest036,0,0,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest036,0,1,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest036,1,1,2) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest036,2,3,3) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest036,2,4,4) { Species = 611, Ability = A3 }, // Fraxure
            new EncounterStatic8N(Nest036,2,4,4) { Species = 612, Ability = A3 }, // Haxorus
            new EncounterStatic8N(Nest036,3,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest036,4,4,4) { Species = 776, Ability = A4 }, // Turtonator
            new EncounterStatic8N(Nest036,4,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest037,0,1,1) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest037,2,4,4) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest037,3,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest037,4,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest039,1,1,2) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest039,4,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest040,0,1,1) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest040,1,1,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest040,2,3,3) { Species = 091, Ability = A3 }, // Cloyster
            new EncounterStatic8N(Nest040,2,4,4) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest040,3,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest040,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest042,1,3,2) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest042,4,4,4) { Species = 867, Ability = A3 }, // Runerigus
            new EncounterStatic8N(Nest042,3,4,4) { Species = 855, Ability = A4 }, // Polteageist
            new EncounterStatic8N(Nest042,3,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest043,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest044,1,2,2) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest044,2,3,3) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest045,0,1,1) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest045,1,2,2) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest045,4,4,4) { Species = 589, Ability = A4 }, // Escavalier
            new EncounterStatic8N(Nest046,1,3,3) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest046,2,4,4) { Species = 711, Ability = A3 }, // Gourgeist
            new EncounterStatic8N(Nest046,3,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest047,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest047,2,4,4) { Species = 560, Ability = A3 }, // Scrafty
            new EncounterStatic8N(Nest047,3,4,4) { Species = 766, Ability = A4 }, // Passimian
            new EncounterStatic8N(Nest050,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest050,1,2,2) { Species = 185, Ability = A3 }, // Sudowoodo
            new EncounterStatic8N(Nest050,2,3,3) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest050,4,4,4) { Species = 874, Ability = A4 }, // Stonjourner
            new EncounterStatic8N(Nest052,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest052,1,2,2) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest052,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest053,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest053,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest053,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest053,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest053,4,4,4) { Species = 776, Ability = A4 }, // Turtonator
            new EncounterStatic8N(Nest054,0,0,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest054,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest057,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest057,1,2,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest057,2,4,4) { Species = 275, Ability = A3 }, // Shiftry
            new EncounterStatic8N(Nest057,4,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest058,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest058,1,2,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest058,4,4,4) { Species = 275, Ability = A4 }, // Shiftry
            new EncounterStatic8N(Nest059,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest059,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest061,2,4,4) { Species = 303, Ability = A3 }, // Mawile
            new EncounterStatic8N(Nest062,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest062,3,4,4) { Species = 560, Ability = A4 }, // Scrafty
            new EncounterStatic8N(Nest062,4,4,4) { Species = 635, Ability = A4 }, // Hydreigon
            new EncounterStatic8N(Nest063,0,0,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest063,0,1,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest063,0,1,1) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest063,1,2,2) { Species = 611, Ability = A3 }, // Fraxure
            new EncounterStatic8N(Nest063,2,4,4) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest063,2,4,4) { Species = 776, Ability = A3 }, // Turtonator
            new EncounterStatic8N(Nest063,3,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest063,4,4,4) { Species = 612, Ability = A4 }, // Haxorus
            new EncounterStatic8N(Nest064,3,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest064,3,4,4) { Species = 876, Ability = A4, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest066,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest070,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest070,0,1,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest070,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest070,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest073,0,0,1) { Species = 684, Ability = A3 }, // Swirlix
            new EncounterStatic8N(Nest073,2,4,4) { Species = 685, Ability = A3 }, // Slurpuff
            new EncounterStatic8N(Nest075,2,4,4) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest076,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest076,1,2,2) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest076,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest077,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest078,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest078,1,2,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest078,2,4,4) { Species = 275, Ability = A3 }, // Shiftry
            new EncounterStatic8N(Nest078,4,4,4) { Species = 841, Ability = A4, CanGigantamax = true }, // Flapple
            new EncounterStatic8N(Nest079,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest079,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest080,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest080,0,0,1) { Species = 066, Ability = A3 }, // Machop
            new EncounterStatic8N(Nest080,0,1,1) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest080,0,1,1) { Species = 083, Ability = A3, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8N(Nest080,1,2,2) { Species = 760, Ability = A3 }, // Bewear
            new EncounterStatic8N(Nest080,1,3,3) { Species = 067, Ability = A3 }, // Machoke
            new EncounterStatic8N(Nest080,2,3,3) { Species = 870, Ability = A3 }, // Falinks
            new EncounterStatic8N(Nest080,2,4,4) { Species = 701, Ability = A3 }, // Hawlucha
            new EncounterStatic8N(Nest080,3,4,4) { Species = 448, Ability = A4 }, // Lucario
            new EncounterStatic8N(Nest080,3,4,4) { Species = 475, Ability = A4 }, // Gallade
            new EncounterStatic8N(Nest080,4,4,4) { Species = 865, Ability = A4 }, // Sirfetch’d
            new EncounterStatic8N(Nest080,4,4,4) { Species = 068, Ability = A4, CanGigantamax = true }, // Machamp
            new EncounterStatic8N(Nest081,0,0,1) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest081,2,4,4) { Species = 303, Ability = A3 }, // Mawile
            new EncounterStatic8N(Nest082,0,0,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest082,0,0,1) { Species = 438, Ability = A3 }, // Bonsly
            new EncounterStatic8N(Nest082,0,1,1) { Species = 837, Ability = A3 }, // Rolycoly
            new EncounterStatic8N(Nest082,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest082,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest082,1,2,2) { Species = 185, Ability = A3 }, // Sudowoodo
            new EncounterStatic8N(Nest082,2,3,3) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest082,2,4,4) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest082,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest082,3,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest082,4,4,4) { Species = 874, Ability = A4 }, // Stonjourner
            new EncounterStatic8N(Nest082,4,4,4) { Species = 839, Ability = A4, CanGigantamax = true }, // Coalossal
            new EncounterStatic8N(Nest083,1,2,2) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest083,2,3,3) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest085,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest085,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest086,0,0,1) { Species = 684, Ability = A3 }, // Swirlix
            new EncounterStatic8N(Nest086,2,3,3) { Species = 685, Ability = A3 }, // Slurpuff
            new EncounterStatic8N(Nest087,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest087,3,4,4) { Species = 560, Ability = A4 }, // Scrafty
            new EncounterStatic8N(Nest087,4,4,4) { Species = 635, Ability = A4 }, // Hydreigon
            new EncounterStatic8N(Nest089,0,1,1) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest089,1,2,2) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest089,4,4,4) { Species = 589, Ability = A4 }, // Escavalier
            new EncounterStatic8N(Nest090,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest091,0,1,1) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest091,1,2,2) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest091,4,4,4) { Species = 589, Ability = A4 }, // Escavalier
            new EncounterStatic8N(Nest106,1,2,2) { Species = 627, Ability = A3 }, // Rufflet
            new EncounterStatic8N(Nest106,3,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest106,4,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest107,1,2,2) { Species = 627, Ability = A2 }, // Rufflet
            new EncounterStatic8N(Nest107,4,4,4) { Species = 628, Ability = A2 }, // Braviary
            new EncounterStatic8N(Nest108,0,1,1) { Species = 127, Ability = A3 }, // Pinsir
            new EncounterStatic8N(Nest108,1,2,2) { Species = 127, Ability = A3 }, // Pinsir
            new EncounterStatic8N(Nest108,2,3,3) { Species = 127, Ability = A3 }, // Pinsir
            new EncounterStatic8N(Nest108,3,4,4) { Species = 127, Ability = A4 }, // Pinsir
            new EncounterStatic8N(Nest109,0,1,1) { Species = 127, Ability = A2 }, // Pinsir
            new EncounterStatic8N(Nest109,4,4,4) { Species = 127, Ability = A2 }, // Pinsir
            new EncounterStatic8N(Nest113,4,4,4) { Species = 038, Ability = A2 }, // Ninetales
            new EncounterStatic8N(Nest114,4,4,4) { Species = 745, Ability = A4 }, // Lycanroc
            new EncounterStatic8N(Nest115,3,4,4) { Species = 745, Ability = A2 }, // Lycanroc
            new EncounterStatic8N(Nest116,2,3,3) { Species = 064, Ability = A3 }, // Kadabra
            new EncounterStatic8N(Nest117,2,3,3) { Species = 064, Ability = A2 }, // Kadabra
            new EncounterStatic8N(Nest117,3,4,4) { Species = 876, Ability = A2, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest117,4,4,4) { Species = 678, Ability = A2, Gender = 0 }, // Meowstic
            new EncounterStatic8N(Nest122,1,2,2) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest122,2,3,3) { Species = 766, Ability = A3 }, // Passimian
            new EncounterStatic8N(Nest122,2,3,3) { Species = 560, Ability = A3 }, // Scrafty
            new EncounterStatic8N(Nest122,3,4,4) { Species = 560, Ability = A4 }, // Scrafty
            new EncounterStatic8N(Nest123,0,1,1) { Species = 559, Ability = A2 }, // Scraggy
            new EncounterStatic8N(Nest123,1,2,2) { Species = 539, Ability = A2 }, // Sawk
            new EncounterStatic8N(Nest123,2,3,3) { Species = 766, Ability = A2 }, // Passimian
            new EncounterStatic8N(Nest123,3,4,4) { Species = 539, Ability = A2 }, // Sawk
            new EncounterStatic8N(Nest123,3,4,4) { Species = 560, Ability = A2 }, // Scrafty
            new EncounterStatic8N(Nest123,4,4,4) { Species = 865, Ability = A2 }, // Sirfetch’d
            new EncounterStatic8N(Nest127,4,4,4) { Species = 770, Ability = A2 }, // Palossand
            new EncounterStatic8N(Nest130,0,1,1) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest130,2,3,3) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest130,3,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest130,4,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest131,1,2,2) { Species = 776, Ability = A2 }, // Turtonator
            new EncounterStatic8N(Nest131,1,2,2) { Species = 782, Ability = A2 }, // Jangmo-o
            new EncounterStatic8N(Nest131,2,3,3) { Species = 783, Ability = A2 }, // Hakamo-o
            new EncounterStatic8N(Nest131,3,4,4) { Species = 776, Ability = A2 }, // Turtonator
            new EncounterStatic8N(Nest131,4,4,4) { Species = 784, Ability = A2 }, // Kommo-o
            new EncounterStatic8N(Nest135,4,4,4) { Species = 550, Ability = A2 }, // Basculin
            new EncounterStatic8N(Nest138,0,1,1) { Species = 692, Ability = A3 }, // Clauncher
            new EncounterStatic8N(Nest138,1,2,2) { Species = 692, Ability = A3 }, // Clauncher
            new EncounterStatic8N(Nest138,4,4,4) { Species = 693, Ability = A4 }, // Clawitzer
            new EncounterStatic8N(Nest139,0,1,1) { Species = 692, Ability = A2 }, // Clauncher
            new EncounterStatic8N(Nest139,3,4,4) { Species = 693, Ability = A2 }, // Clawitzer
            new EncounterStatic8N(Nest139,4,4,4) { Species = 693, Ability = A2 }, // Clawitzer
            new EncounterStatic8N(Nest147,1,2,2) { Species = 550, Ability = A2 }, // Basculin
            new EncounterStatic8N(Nest147,3,4,4) { Species = 550, Ability = A2 }, // Basculin
            new EncounterStatic8N(Nest148,1,2,2) { Species = 127, Ability = A2 }, // Pinsir
            new EncounterStatic8N(Nest150,2,3,3) { Species = 841, Ability = A2 }, // Flapple
            new EncounterStatic8N(Nest150,3,4,4) { Species = 841, Ability = A2 }, // Flapple
            new EncounterStatic8N(Nest150,4,4,4) { Species = 841, Ability = A2, CanGigantamax = true }, // Flapple
            new EncounterStatic8N(Nest155,4,4,4) { Species = 745, Ability = A4 }, // Lycanroc
        };

        internal static readonly EncounterStatic8N[] Nest_SH =
        {
            new EncounterStatic8N(Nest000,0,1,1) { Species = 453, Ability = A3 }, // Croagunk
            new EncounterStatic8N(Nest000,2,3,3) { Species = 107, Ability = A3 }, // Hitmonchan
            new EncounterStatic8N(Nest000,2,4,4) { Species = 106, Ability = A3 }, // Hitmonlee
            new EncounterStatic8N(Nest000,2,4,4) { Species = 454, Ability = A3 }, // Toxicroak
            new EncounterStatic8N(Nest000,3,4,4) { Species = 237, Ability = A4 }, // Hitmontop
            new EncounterStatic8N(Nest000,4,4,4) { Species = 534, Ability = A4 }, // Conkeldurr
            new EncounterStatic8N(Nest001,0,1,1) { Species = 577, Ability = A3 }, // Solosis
            new EncounterStatic8N(Nest001,2,3,3) { Species = 678, Ability = A3, Gender = 1, Form = 1 }, // Meowstic-1
            new EncounterStatic8N(Nest001,2,3,3) { Species = 578, Ability = A3 }, // Duosion
            new EncounterStatic8N(Nest001,3,4,4) { Species = 579, Ability = A4 }, // Reuniclus
            new EncounterStatic8N(Nest001,4,4,4) { Species = 337, Ability = A4 }, // Lunatone
            new EncounterStatic8N(Nest002,0,0,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest002,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest002,3,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest002,4,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest006,0,1,2) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest006,0,1,2) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest006,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest007,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest008,1,1,1) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest009,1,1,2) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest009,1,2,2) { Species = 538, Ability = A3 }, // Throh
            new EncounterStatic8N(Nest009,3,4,4) { Species = 760, Ability = A4 }, // Bewear
            new EncounterStatic8N(Nest011,4,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest012,0,1,2) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest012,0,1,2) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest012,1,1,2) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new EncounterStatic8N(Nest012,1,3,3) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest012,2,4,4) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest012,3,4,4) { Species = 765, Ability = A4 }, // Oranguru
            new EncounterStatic8N(Nest012,4,4,4) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest013,2,4,4) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest014,0,0,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest014,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest014,2,4,4) { Species = 839, Ability = A3 }, // Coalossal
            new EncounterStatic8N(Nest014,3,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest014,4,4,4) { Species = 095, Ability = A4 }, // Onix
            new EncounterStatic8N(Nest016,0,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest016,1,1,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest016,2,3,3) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest016,2,4,4) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest016,3,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest016,4,4,4) { Species = 450, Ability = A4 }, // Hippowdon
            new EncounterStatic8N(Nest017,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest017,1,1,1) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest017,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest017,2,3,3) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest017,2,4,4) { Species = 059, Ability = A3 }, // Arcanine
            new EncounterStatic8N(Nest017,3,4,4) { Species = 631, Ability = A4 }, // Heatmor
            new EncounterStatic8N(Nest017,4,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest017,4,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest018,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest018,0,1,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest018,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest018,2,3,3) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest018,2,4,4) { Species = 059, Ability = A3 }, // Arcanine
            new EncounterStatic8N(Nest018,2,4,4) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest018,3,4,4) { Species = 324, Ability = A4 }, // Torkoal
            new EncounterStatic8N(Nest018,4,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest019,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest019,1,1,1) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest019,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest019,2,3,3) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest019,2,4,4) { Species = 059, Ability = A3 }, // Arcanine
            new EncounterStatic8N(Nest019,4,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest022,0,1,1) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest022,4,4,4) { Species = 875, Ability = A4 }, // Eiscue
            new EncounterStatic8N(Nest025,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest025,1,1,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest025,2,4,4) { Species = 272, Ability = A3 }, // Ludicolo
            new EncounterStatic8N(Nest026,4,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest028,0,1,1) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest028,1,1,1) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest028,3,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest028,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest030,0,1,1) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest030,4,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest032,0,1,1) { Species = 682, Ability = A3 }, // Spritzee
            new EncounterStatic8N(Nest032,4,4,4) { Species = 683, Ability = A4 }, // Aromatisse
            new EncounterStatic8N(Nest033,4,4,4) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest034,4,4,4) { Species = 302, Ability = A4 }, // Sableye
            new EncounterStatic8N(Nest035,1,1,2) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest035,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest035,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest036,0,0,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest036,0,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest036,1,1,2) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest036,2,3,3) { Species = 611, Ability = A3 }, // Fraxure
            new EncounterStatic8N(Nest036,2,4,4) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest036,2,4,4) { Species = 330, Ability = A3 }, // Flygon
            new EncounterStatic8N(Nest036,3,4,4) { Species = 612, Ability = A4 }, // Haxorus
            new EncounterStatic8N(Nest036,4,4,4) { Species = 780, Ability = A4 }, // Drampa
            new EncounterStatic8N(Nest036,4,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest037,0,1,1) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest037,2,4,4) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest037,3,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest037,4,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest039,1,1,2) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest039,4,4,4) { Species = 765, Ability = A4 }, // Oranguru
            new EncounterStatic8N(Nest040,0,1,1) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest040,1,1,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest040,2,3,3) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest040,2,4,4) { Species = 091, Ability = A3 }, // Cloyster
            new EncounterStatic8N(Nest040,3,4,4) { Species = 746, Ability = A4 }, // Wishiwashi
            new EncounterStatic8N(Nest040,4,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest042,1,3,3) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest042,4,4,4) { Species = 302, Ability = A3 }, // Sableye
            new EncounterStatic8N(Nest042,3,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest042,3,4,4) { Species = 864, Ability = A4 }, // Cursola
            new EncounterStatic8N(Nest043,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest044,1,2,2) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest044,2,3,3) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest045,0,1,1) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest045,1,2,2) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest045,4,4,4) { Species = 617, Ability = A4 }, // Accelgor
            new EncounterStatic8N(Nest046,1,3,3) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest046,2,4,4) { Species = 302, Ability = A3 }, // Sableye
            new EncounterStatic8N(Nest046,3,4,4) { Species = 864, Ability = A4 }, // Cursola
            new EncounterStatic8N(Nest047,0,1,1) { Species = 453, Ability = A3 }, // Croagunk
            new EncounterStatic8N(Nest047,2,4,4) { Species = 454, Ability = A3 }, // Toxicroak
            new EncounterStatic8N(Nest047,3,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest050,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest050,1,2,2) { Species = 246, Ability = A3 }, // Larvitar
            new EncounterStatic8N(Nest050,2,3,3) { Species = 247, Ability = A3 }, // Pupitar
            new EncounterStatic8N(Nest050,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest052,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest052,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest052,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest053,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest053,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest053,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest053,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest053,4,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest054,0,0,1) { Species = 613, Ability = A3 }, // Cubchoo
            new EncounterStatic8N(Nest054,4,4,4) { Species = 875, Ability = A4 }, // Eiscue
            new EncounterStatic8N(Nest057,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest057,1,2,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest057,2,4,4) { Species = 272, Ability = A3 }, // Ludicolo
            new EncounterStatic8N(Nest057,4,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest058,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest058,1,2,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest058,4,4,4) { Species = 272, Ability = A4 }, // Ludicolo
            new EncounterStatic8N(Nest059,1,2,2) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest059,4,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest061,2,4,4) { Species = 078, Ability = A3, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest062,0,1,1) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest062,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest062,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest063,0,0,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest063,0,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest063,0,1,1) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest063,1,2,2) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest063,2,4,4) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest063,2,4,4) { Species = 780, Ability = A3 }, // Drampa
            new EncounterStatic8N(Nest063,3,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest063,4,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest064,3,4,4) { Species = 765, Ability = A4 }, // Oranguru
            new EncounterStatic8N(Nest064,3,4,4) { Species = 876, Ability = A4, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest066,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest070,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest070,0,1,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest070,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest070,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest073,0,0,1) { Species = 682, Ability = A3 }, // Spritzee
            new EncounterStatic8N(Nest073,2,4,4) { Species = 683, Ability = A3 }, // Aromatisse
            new EncounterStatic8N(Nest075,2,4,4) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest076,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest076,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest076,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest077,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest078,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest078,1,2,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest078,2,4,4) { Species = 272, Ability = A3 }, // Ludicolo
            new EncounterStatic8N(Nest078,4,4,4) { Species = 842, Ability = A4, CanGigantamax = true }, // Appletun
            new EncounterStatic8N(Nest079,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest079,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest080,0,0,1) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest080,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest080,0,1,1) { Species = 854, Ability = A3 }, // Sinistea
            new EncounterStatic8N(Nest080,0,1,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest080,1,2,2) { Species = 680, Ability = A3 }, // Doublade
            new EncounterStatic8N(Nest080,1,3,3) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest080,2,3,3) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest080,2,4,4) { Species = 302, Ability = A3 }, // Sableye
            new EncounterStatic8N(Nest080,3,4,4) { Species = 855, Ability = A4 }, // Polteageist
            new EncounterStatic8N(Nest080,3,4,4) { Species = 864, Ability = A4 }, // Cursola
            new EncounterStatic8N(Nest080,4,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest080,4,4,4) { Species = 094, Ability = A4, CanGigantamax = true }, // Gengar
            new EncounterStatic8N(Nest081,0,0,1) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new EncounterStatic8N(Nest081,2,4,4) { Species = 078, Ability = A3, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest082,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest082,0,0,1) { Species = 613, Ability = A3 }, // Cubchoo
            new EncounterStatic8N(Nest082,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest082,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest082,1,2,2) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest082,1,2,2) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest082,2,3,3) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest082,2,4,4) { Species = 362, Ability = A3 }, // Glalie
            new EncounterStatic8N(Nest082,3,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest082,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest082,4,4,4) { Species = 875, Ability = A4 }, // Eiscue
            new EncounterStatic8N(Nest082,4,4,4) { Species = 131, Ability = A4, CanGigantamax = true }, // Lapras
            new EncounterStatic8N(Nest083,1,2,2) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest083,2,3,3) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest085,1,2,2) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest085,4,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest086,0,0,1) { Species = 682, Ability = A3 }, // Spritzee
            new EncounterStatic8N(Nest086,2,3,3) { Species = 683, Ability = A3 }, // Aromatisse
            new EncounterStatic8N(Nest087,0,1,1) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest087,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest087,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest089,0,1,1) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest089,1,2,2) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest089,4,4,4) { Species = 617, Ability = A4 }, // Accelgor
            new EncounterStatic8N(Nest090,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest091,0,1,1) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest091,1,2,2) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest091,4,4,4) { Species = 617, Ability = A4 }, // Accelgor
            new EncounterStatic8N(Nest106,1,2,2) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest106,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest106,4,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest107,1,2,2) { Species = 629, Ability = A2 }, // Vullaby
            new EncounterStatic8N(Nest107,4,4,4) { Species = 630, Ability = A2 }, // Mandibuzz
            new EncounterStatic8N(Nest108,0,1,1) { Species = 214, Ability = A3 }, // Heracross
            new EncounterStatic8N(Nest108,1,2,2) { Species = 214, Ability = A3 }, // Heracross
            new EncounterStatic8N(Nest108,2,3,3) { Species = 214, Ability = A3 }, // Heracross
            new EncounterStatic8N(Nest108,3,4,4) { Species = 214, Ability = A4 }, // Heracross
            new EncounterStatic8N(Nest109,0,1,1) { Species = 214, Ability = A2 }, // Heracross
            new EncounterStatic8N(Nest109,4,4,4) { Species = 214, Ability = A2 }, // Heracross
            new EncounterStatic8N(Nest113,4,4,4) { Species = 059, Ability = A2 }, // Arcanine
            new EncounterStatic8N(Nest114,4,4,4) { Species = 745, Ability = A4, Form = 1 }, // Lycanroc-1
            new EncounterStatic8N(Nest115,3,4,4) { Species = 745, Ability = A2, Form = 1 }, // Lycanroc-1
            new EncounterStatic8N(Nest116,2,3,3) { Species = 765, Ability = A3 }, // Oranguru
            new EncounterStatic8N(Nest117,2,3,3) { Species = 765, Ability = A2 }, // Oranguru
            new EncounterStatic8N(Nest117,3,4,4) { Species = 876, Ability = A2, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest117,4,4,4) { Species = 678, Ability = A2, Gender = 1, Form = 1 }, // Meowstic-1
            new EncounterStatic8N(Nest122,1,2,2) { Species = 453, Ability = A3 }, // Croagunk
            new EncounterStatic8N(Nest122,2,3,3) { Species = 853, Ability = A3 }, // Grapploct
            new EncounterStatic8N(Nest122,2,3,3) { Species = 454, Ability = A3 }, // Toxicroak
            new EncounterStatic8N(Nest122,3,4,4) { Species = 454, Ability = A4 }, // Toxicroak
            new EncounterStatic8N(Nest123,0,1,1) { Species = 453, Ability = A2 }, // Croagunk
            new EncounterStatic8N(Nest123,1,2,2) { Species = 538, Ability = A2 }, // Throh
            new EncounterStatic8N(Nest123,2,3,3) { Species = 620, Ability = A2 }, // Mienshao
            new EncounterStatic8N(Nest123,3,4,4) { Species = 538, Ability = A2 }, // Throh
            new EncounterStatic8N(Nest123,3,4,4) { Species = 454, Ability = A2 }, // Toxicroak
            new EncounterStatic8N(Nest123,4,4,4) { Species = 870, Ability = A2 }, // Falinks
            new EncounterStatic8N(Nest127,4,4,4) { Species = 864, Ability = A2 }, // Cursola
            new EncounterStatic8N(Nest130,0,1,1) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest130,2,3,3) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest130,3,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest130,4,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest131,1,2,2) { Species = 780, Ability = A2 }, // Drampa
            new EncounterStatic8N(Nest131,1,2,2) { Species = 704, Ability = A2 }, // Goomy
            new EncounterStatic8N(Nest131,2,3,3) { Species = 705, Ability = A2 }, // Sliggoo
            new EncounterStatic8N(Nest131,3,4,4) { Species = 780, Ability = A2 }, // Drampa
            new EncounterStatic8N(Nest131,4,4,4) { Species = 706, Ability = A2 }, // Goodra
            new EncounterStatic8N(Nest135,4,4,4) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest138,0,1,1) { Species = 690, Ability = A3 }, // Skrelp
            new EncounterStatic8N(Nest138,1,2,2) { Species = 690, Ability = A3 }, // Skrelp
            new EncounterStatic8N(Nest138,4,4,4) { Species = 691, Ability = A4 }, // Dragalge
            new EncounterStatic8N(Nest139,0,1,1) { Species = 690, Ability = A2 }, // Skrelp
            new EncounterStatic8N(Nest139,3,4,4) { Species = 691, Ability = A2 }, // Dragalge
            new EncounterStatic8N(Nest139,4,4,4) { Species = 691, Ability = A2 }, // Dragalge
            new EncounterStatic8N(Nest147,1,2,2) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest147,3,4,4) { Species = 550, Ability = A2, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest148,1,2,2) { Species = 214, Ability = A2 }, // Heracross
            new EncounterStatic8N(Nest150,2,3,3) { Species = 842, Ability = A2 }, // Appletun
            new EncounterStatic8N(Nest150,3,4,4) { Species = 842, Ability = A2 }, // Appletun
            new EncounterStatic8N(Nest150,4,4,4) { Species = 842, Ability = A2, CanGigantamax = true }, // Appletun
            new EncounterStatic8N(Nest155,4,4,4) { Species = 745, Ability = A4, Form = 1 }, // Lycanroc-1
        };
        #endregion
    }
}
