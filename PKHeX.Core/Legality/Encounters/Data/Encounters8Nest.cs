using System.Collections.Generic;

namespace PKHeX.Core
{
    internal static partial class Encounters8Nest
    {
        #region Nest
        // Location IDs for each Nest group
        private const byte Nest00 = 00;
        private const byte Nest01 = 01;
        private const byte Nest02 = 02;
        private const byte Nest03 = 03;
        private const byte Nest04 = 04;
        private const byte Nest05 = 05;
        private const byte Nest06 = 06;
        private const byte Nest07 = 07;
        private const byte Nest08 = 08;
        private const byte Nest09 = 09;
        private const byte Nest10 = 10;
        private const byte Nest11 = 11;
        private const byte Nest12 = 12;
        private const byte Nest13 = 13;
        private const byte Nest14 = 14;
        private const byte Nest15 = 15;
        private const byte Nest16 = 16;
        private const byte Nest17 = 17;
        private const byte Nest18 = 18;
        private const byte Nest19 = 19;
        private const byte Nest20 = 20;
        private const byte Nest21 = 21;
        private const byte Nest22 = 22;
        private const byte Nest23 = 23;
        private const byte Nest24 = 24;
        private const byte Nest25 = 25;
        private const byte Nest26 = 26;
        private const byte Nest27 = 27;
        private const byte Nest28 = 28;
        private const byte Nest29 = 29;
        private const byte Nest30 = 30;
        private const byte Nest31 = 31;
        private const byte Nest32 = 32;
        private const byte Nest33 = 33;
        private const byte Nest34 = 34;
        private const byte Nest35 = 35;
        private const byte Nest36 = 36;
        private const byte Nest37 = 37;
        private const byte Nest38 = 38;
        private const byte Nest39 = 39;
        private const byte Nest40 = 40;
        private const byte Nest41 = 41;
        private const byte Nest42 = 42;
        private const byte Nest43 = 43;
        private const byte Nest44 = 44;
        private const byte Nest45 = 45;
        private const byte Nest46 = 46;
        private const byte Nest47 = 47;
        private const byte Nest48 = 48;
        private const byte Nest49 = 49;
        private const byte Nest50 = 50;
        private const byte Nest51 = 51;
        private const byte Nest52 = 52;
        private const byte Nest53 = 53;
        private const byte Nest54 = 54;
        private const byte Nest55 = 55;
        private const byte Nest56 = 56;
        private const byte Nest57 = 57;
        private const byte Nest58 = 58;
        private const byte Nest59 = 59;
        private const byte Nest60 = 60;
        private const byte Nest61 = 61;
        private const byte Nest62 = 62;
        private const byte Nest63 = 63;
        private const byte Nest64 = 64;
        private const byte Nest65 = 65;
        private const byte Nest66 = 66;
        private const byte Nest67 = 67;
        private const byte Nest68 = 68;
        private const byte Nest69 = 69;
        private const byte Nest70 = 70;
        private const byte Nest71 = 71;
        private const byte Nest72 = 72;
        private const byte Nest73 = 73;
        private const byte Nest74 = 74;
        private const byte Nest75 = 75;
        private const byte Nest76 = 76;
        private const byte Nest77 = 77;
        private const byte Nest78 = 78;
        private const byte Nest79 = 79;
        private const byte Nest80 = 80;
        private const byte Nest81 = 81;
        private const byte Nest82 = 82;
        private const byte Nest83 = 83;
        private const byte Nest84 = 84;
        private const byte Nest85 = 85;
        private const byte Nest86 = 86;
        private const byte Nest87 = 87;
        private const byte Nest88 = 88;
        private const byte Nest89 = 89;
        private const byte Nest90 = 90;
        private const byte Nest91 = 91;
        private const byte Nest92 = 92;

        internal static readonly IReadOnlyList<IReadOnlyList<byte>> NestLocations = new []
        {
            new byte[] {144, 134, 122},      // 00 : Stony Wilderness, South Lake Miloch, Rolling Fields
            new byte[] {144, 126},           // 01 : Stony Wilderness, Watchtower Ruins
            new byte[] {144, 122},           // 02 : Stony Wilderness, Rolling Fields
            new byte[] {142, 124, 122},      // 03 : Bridge Field, Dappled Grove, Rolling Fields
            new byte[] {142, 134},           // 04 : Bridge Field, South Lake Miloch
            new byte[] {144, 126},           // 05 : Stony Wilderness, Watchtower Ruins
            new byte[] {128, 130},           // 06 : East Lake Axewell, West Lake Axewell
            new byte[] {154, 142, 134},      // 07 : Lake of Outrage, Bridge Field, South Lake Miloch
            new byte[] {146, 130},           // 08 : Dusty Bowl, West Lake Axewell
            new byte[] {146, 138},           // 09 : Dusty Bowl, North Lake Miloch
            new byte[] {146, 136},           // 10 : Dusty Bowl, Giants Seat
            new byte[] {150, 144, 136},      // 11 : Hammerlocke Hills, Stony Wilderness, Giants Seat
            new byte[] {142},                // 12 : Bridge Field
            new byte[] {150, 144, 140},      // 13 : Hammerlocke Hills, Stony Wilderness, Motostoke Riverbank
            new byte[] {146, 136},           // 14 : Dusty Bowl, Giants Seat
            new byte[] {142, 122},           // 15 : Bridge Field, Rolling Fields
            new byte[] {146},                // 16 : Dusty Bowl
            new byte[] {154, 152, 144},      // 17 : Lake of Outrage, Giants Cap, Stony Wilderness
            new byte[] {150, 144},           // 18 : Hammerlocke Hills, Stony Wilderness
            new byte[] {146},                // 19 : Dusty Bowl
            new byte[] {146},                // 20 : Dusty Bowl
            new byte[] {144},                // 21 : Stony Wilderness
            new byte[] {150, 152},           // 22 : Hammerlocke Hills, Giants Cap
            new byte[] {152, 140},           // 23 : Giants Cap, Motostoke Riverbank
            new byte[] {154, 148},           // 24 : Lake of Outrage, Giants Mirror
            new byte[] {124},                // 25 : Dappled Grove
            new byte[] {148, 144, 142},      // 26 : Giants Mirror, Stony Wilderness, Bridge Field
            new byte[] {148, 124, 146},      // 27 : Giants Mirror, Dappled Grove AND Dusty Bowl (Giant's Mirror load-line overlap)
            new byte[] {138, 128},           // 28 : North Lake Miloch, East Lake Axewell
            new byte[] {150, 152, 140},      // 29 : Hammerlocke Hills, Giants Cap, Motostoke Riverbank
            new byte[] {128, 122},           // 30 : East Lake Axewell, Rolling Fields
            new byte[] {150, 152},           // 31 : Hammerlocke Hills, Giants Cap
            new byte[] {150, 122},           // 32 : Hammerlocke Hills, Rolling Fields
            new byte[] {154, 142},           // 33 : Lake of Outrage, Bridge Field
            new byte[] {144, 130},           // 34 : Stony Wilderness, West Lake Axewell
            new byte[] {142, 146, 148},      // 35 : Bridge Field, Dusty Bowl, Giants Mirror
            new byte[] {122},                // 36 : Rolling Fields
            new byte[] {132},                // 37 : Axew's Eye
            new byte[] {128, 122},           // 38 : East Lake Axewell, Rolling Fields
            new byte[] {144, 142, 140},      // 39 : Stony Wilderness, Bridge Field, Motostoke Riverbank
            new byte[] {134, 138},           // 40 : South Lake Miloch, North Lake Miloch
            new byte[] {148, 130},           // 41 : Giants Mirror, West Lake Axewell
            new byte[] {148, 144, 134, 146}, // 42 : Giants Mirror, Stony Wilderness, South Lake Miloch AND Dusty Bowl (Giant's Mirror load-line overlap)
            new byte[] {154, 142, 128, 130}, // 43 : Lake of Outrage, Bridge Field, East Lake Axewell, West Lake Axewell 
            new byte[] {150, 136},           // 44 : Hammerlocke Hills, Giants Seat
            new byte[] {142, 134, 122},      // 45 : Bridge Field, South Lake Miloch, Rolling Fields
            new byte[] {126},                // 46 : Watchtower Ruins
            new byte[] {146, 138, 122, 134}, // 47 : Dusty Bowl, North Lake Miloch, Rolling Fields, South Lake Miloch
            new byte[] {146, 136},           // 48 : Dusty Bowl, Giants Seat
            new byte[] {144, 140, 126},      // 49 : Stony Wilderness, Motostoke Riverbank, Watchtower Ruins
            new byte[] {144, 136, 122},      // 50 : Stony Wilderness, Giants Seat, Rolling Fields
            new byte[] {146, 142, 122},      // 51 : Dusty Bowl, Bridge Field, Rolling Fields
            new byte[] {150},                // 52 : Hammerlocke Hills
            new byte[] {146, 144},           // 53 : Dusty Bowl, Stony Wilderness
            new byte[] {152, 146, 144},      // 54 : Giants Cap, Dusty Bowl, Stony Wilderness
            new byte[] {154, 140},           // 55 : Lake of Outrage, Motostoke Riverbank
            new byte[] {150},                // 56 : Hammerlocke Hills
            new byte[] {124},                // 57 : Dappled Grove
            new byte[] {144, 142, 124},      // 58 : Stony Wilderness, Bridge Field, Dappled Grove
            new byte[] {152, 140, 138},      // 59 : Giants Cap, Motostoke Riverbank, North Lake Miloch
            new byte[] {150, 128},           // 60 : Hammerlocke Hills, East Lake Axewell
            new byte[] {150, 122},           // 61 : Hammerlocke Hills, Rolling Fields
            new byte[] {144, 142, 130},      // 62 : Stony Wilderness, Bridge Field, West Lake Axewell
            new byte[] {132, 122},           // 63 : Axew's Eye, Rolling Fields
            new byte[] {142, 140, 128, 122}, // 64 : Bridge Field, Motostoke Riverbank, East Lake Axewell, Rolling Fields 
            new byte[] {144},                // 65 : Stony Wilderness
            new byte[] {148},                // 66 : Giants Mirror
            new byte[] {150},                // 67 : Hammerlocke Hills
            new byte[] {148},                // 68 : Giants Mirror
            new byte[] {148},                // 69 : Giants Mirror
            new byte[] {152},                // 70 : Giants Cap
            new byte[] {148},                // 71 : Giants Mirror
            new byte[] {150},                // 72 : Hammerlocke Hills
            new byte[] {154},                // 73 : Lake of Outrage
            new byte[] {146, 130},           // 74 : Dusty Bowl, West Lake Axewell
            new byte[] {138, 134},           // 75 : North Lake Miloch, South Lake Miloch
            new byte[] {154},                // 76 : Lake of Outrage
            new byte[] {152},                // 77 : Giants Cap
            new byte[] {124},                // 78 : Dappled Grove
            new byte[] {144},                // 79 : Stony Wilderness
            new byte[] {144},                // 80 : Stony Wilderness
            new byte[] {142},                // 81 : Bridge Field
            new byte[] {136},                // 82 : Giants Seat
            new byte[] {136},                // 83 : Giants Seat
            new byte[] {144},                // 84 : Stony Wilderness
            new byte[] {128},                // 85 : East Lake Axewell
            new byte[] {142},                // 86 : Bridge Field
            new byte[] {146},                // 87 : Dusty Bowl
            new byte[] {152},                // 88 : Giants Cap
            new byte[] {122},                // 89 : Rolling Fields
            new byte[] {130, 134},           // 90 : West Lake Axewell, South Lake Miloch
            new byte[] {142, 124},           // 91 : Bridge Field, Dappled Grove
            new byte[] {146},                // 92 : Dusty Bowl
        };

        // Abilities Allowed
        private const int A2 = 4; // Ability 4 only??
        private const int A3 = 0; // 1/2 only
        internal const int A4 = -1; // 1/2/H

        internal const int SharedNest = 162;
        internal const int Watchtower = 126;

        internal static readonly EncounterStatic8N[] Nest_Common =
        {
            new EncounterStatic8N(Nest00,0,0,1) { Species = 236, Ability = A3 }, // Tyrogue
            new EncounterStatic8N(Nest00,0,0,1) { Species = 066, Ability = A3 }, // Machop
            new EncounterStatic8N(Nest00,0,1,1) { Species = 532, Ability = A3 }, // Timburr
            new EncounterStatic8N(Nest00,1,2,2) { Species = 067, Ability = A3 }, // Machoke
            new EncounterStatic8N(Nest00,1,2,2) { Species = 533, Ability = A3 }, // Gurdurr
            new EncounterStatic8N(Nest00,4,4,4) { Species = 068, Ability = A4 }, // Machamp
            new EncounterStatic8N(Nest01,0,0,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest01,0,0,1) { Species = 517, Ability = A3 }, // Munna
            new EncounterStatic8N(Nest01,0,1,1) { Species = 677, Ability = A3 }, // Espurr
            new EncounterStatic8N(Nest01,0,1,1) { Species = 605, Ability = A3 }, // Elgyem
            new EncounterStatic8N(Nest01,1,2,2) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest01,2,4,4) { Species = 518, Ability = A3 }, // Musharna
            new EncounterStatic8N(Nest01,4,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest02,0,0,1) { Species = 438, Ability = A3 }, // Bonsly
            new EncounterStatic8N(Nest02,0,1,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest02,1,2,2) { Species = 111, Ability = A3 }, // Rhyhorn
            new EncounterStatic8N(Nest02,1,2,2) { Species = 525, Ability = A3 }, // Boldore
            new EncounterStatic8N(Nest02,2,3,3) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest02,2,4,4) { Species = 112, Ability = A3 }, // Rhydon
            new EncounterStatic8N(Nest02,2,4,4) { Species = 185, Ability = A3 }, // Sudowoodo
            new EncounterStatic8N(Nest02,4,4,4) { Species = 213, Ability = A4 }, // Shuckle
            new EncounterStatic8N(Nest03,0,0,1) { Species = 010, Ability = A3 }, // Caterpie
            new EncounterStatic8N(Nest03,0,0,1) { Species = 736, Ability = A3 }, // Grubbin
            new EncounterStatic8N(Nest03,0,1,1) { Species = 290, Ability = A3 }, // Nincada
            new EncounterStatic8N(Nest03,0,1,1) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest03,1,2,2) { Species = 011, Ability = A3 }, // Metapod
            new EncounterStatic8N(Nest03,1,2,2) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest03,2,3,3) { Species = 737, Ability = A3 }, // Charjabug
            new EncounterStatic8N(Nest03,2,4,4) { Species = 291, Ability = A3 }, // Ninjask
            new EncounterStatic8N(Nest03,2,4,4) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest03,3,4,4) { Species = 596, Ability = A4 }, // Galvantula
            new EncounterStatic8N(Nest03,4,4,4) { Species = 738, Ability = A4 }, // Vikavolt
            new EncounterStatic8N(Nest03,4,4,4) { Species = 632, Ability = A4 }, // Durant
            new EncounterStatic8N(Nest04,0,0,1) { Species = 010, Ability = A3 }, // Caterpie
            new EncounterStatic8N(Nest04,0,0,1) { Species = 415, Ability = A3 }, // Combee
            new EncounterStatic8N(Nest04,0,1,1) { Species = 742, Ability = A3 }, // Cutiefly
            new EncounterStatic8N(Nest04,0,1,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest04,1,2,2) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest04,1,2,2) { Species = 011, Ability = A3 }, // Metapod
            new EncounterStatic8N(Nest04,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest04,2,4,4) { Species = 596, Ability = A3 }, // Galvantula
            new EncounterStatic8N(Nest04,2,4,4) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest04,3,4,4) { Species = 743, Ability = A4 }, // Ribombee
            new EncounterStatic8N(Nest04,4,4,4) { Species = 416, Ability = A4 }, // Vespiquen
            new EncounterStatic8N(Nest04,4,4,4) { Species = 826, Ability = A4 }, // Orbeetle
            new EncounterStatic8N(Nest05,0,0,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest05,0,0,1) { Species = 355, Ability = A3 }, // Duskull
            new EncounterStatic8N(Nest05,0,1,1) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest05,0,1,1) { Species = 708, Ability = A3 }, // Phantump
            new EncounterStatic8N(Nest05,0,1,1) { Species = 592, Ability = A3 }, // Frillish
            new EncounterStatic8N(Nest05,1,2,2) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest05,2,3,3) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest05,2,4,4) { Species = 356, Ability = A3 }, // Dusclops
            new EncounterStatic8N(Nest05,2,4,4) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest05,3,4,4) { Species = 709, Ability = A4 }, // Trevenant
            new EncounterStatic8N(Nest05,4,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest05,4,4,4) { Species = 593, Ability = A4 }, // Jellicent
            new EncounterStatic8N(Nest06,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest06,0,0,1) { Species = 458, Ability = A3 }, // Mantyke
            new EncounterStatic8N(Nest06,1,2,2) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest06,2,3,3) { Species = 224, Ability = A3 }, // Octillery
            new EncounterStatic8N(Nest06,2,4,4) { Species = 226, Ability = A3 }, // Mantine
            new EncounterStatic8N(Nest06,2,4,4) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest06,3,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest06,4,4,4) { Species = 746, Ability = A4 }, // Wishiwashi
            new EncounterStatic8N(Nest06,4,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest07,0,0,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest07,0,0,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest07,0,1,1) { Species = 422, Ability = A3, Form = 1 }, // Shellos-1
            new EncounterStatic8N(Nest07,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest07,1,2,2) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest07,2,3,3) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest07,2,4,4) { Species = 834, Ability = A3 }, // Drednaw
            new EncounterStatic8N(Nest07,2,4,4) { Species = 847, Ability = A3 }, // Barraskewda
            new EncounterStatic8N(Nest07,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest07,4,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest07,4,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest08,0,0,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest08,0,0,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest08,0,1,1) { Species = 535, Ability = A3 }, // Tympole
            new EncounterStatic8N(Nest08,0,1,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest08,1,2,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest08,2,3,3) { Species = 834, Ability = A3 }, // Drednaw
            new EncounterStatic8N(Nest08,2,4,4) { Species = 195, Ability = A3 }, // Quagsire
            new EncounterStatic8N(Nest08,2,4,4) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest08,3,4,4) { Species = 091, Ability = A4 }, // Cloyster
            new EncounterStatic8N(Nest08,4,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest08,4,4,4) { Species = 342, Ability = A4 }, // Crawdaunt
            new EncounterStatic8N(Nest09,0,0,1) { Species = 236, Ability = A3 }, // Tyrogue
            new EncounterStatic8N(Nest09,0,0,1) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest09,0,1,1) { Species = 852, Ability = A3 }, // Clobbopus
            new EncounterStatic8N(Nest09,0,1,1) { Species = 674, Ability = A3 }, // Pancham
            new EncounterStatic8N(Nest09,2,4,4) { Species = 760, Ability = A3 }, // Bewear
            new EncounterStatic8N(Nest09,2,4,4) { Species = 675, Ability = A3 }, // Pangoro
            new EncounterStatic8N(Nest09,2,4,4) { Species = 701, Ability = A3 }, // Hawlucha
            new EncounterStatic8N(Nest09,4,4,4) { Species = 853, Ability = A4 }, // Grapploct
            new EncounterStatic8N(Nest09,4,4,4) { Species = 870, Ability = A4 }, // Falinks
            new EncounterStatic8N(Nest10,0,0,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest10,0,0,1) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest10,0,1,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest10,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest10,1,1,2) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest10,1,2,2) { Species = 878, Ability = A3 }, // Cufant
            new EncounterStatic8N(Nest10,2,4,4) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest10,2,4,4) { Species = 863, Ability = A3 }, // Perrserker
            new EncounterStatic8N(Nest10,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest10,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest10,4,4,4) { Species = 601, Ability = A4 }, // Klinklang
            new EncounterStatic8N(Nest10,4,4,4) { Species = 879, Ability = A4 }, // Copperajah
            new EncounterStatic8N(Nest11,0,0,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest11,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest11,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest11,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest11,1,1,2) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest11,1,2,2) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest11,2,4,4) { Species = 208, Ability = A3 }, // Steelix
            new EncounterStatic8N(Nest11,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest11,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest11,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest11,4,4,4) { Species = 777, Ability = A4 }, // Togedemaru
            new EncounterStatic8N(Nest12,0,0,1) { Species = 439, Ability = A3 }, // Mime Jr.
            new EncounterStatic8N(Nest12,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest12,2,3,3) { Species = 561, Ability = A3 }, // Sigilyph
            new EncounterStatic8N(Nest12,2,3,3) { Species = 178, Ability = A3 }, // Xatu
            new EncounterStatic8N(Nest12,4,4,4) { Species = 858, Ability = A4 }, // Hatterene
            new EncounterStatic8N(Nest13,0,0,1) { Species = 439, Ability = A3 }, // Mime Jr.
            new EncounterStatic8N(Nest13,0,0,1) { Species = 360, Ability = A3 }, // Wynaut
            new EncounterStatic8N(Nest13,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest13,0,1,1) { Species = 343, Ability = A3 }, // Baltoy
            new EncounterStatic8N(Nest13,1,1,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest13,1,3,3) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest13,2,3,3) { Species = 561, Ability = A3 }, // Sigilyph
            new EncounterStatic8N(Nest13,2,3,3) { Species = 178, Ability = A3 }, // Xatu
            new EncounterStatic8N(Nest13,3,4,4) { Species = 344, Ability = A4 }, // Claydol
            new EncounterStatic8N(Nest13,4,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest13,4,4,4) { Species = 202, Ability = A4 }, // Wobbuffet
            new EncounterStatic8N(Nest14,0,0,1) { Species = 837, Ability = A3 }, // Rolycoly
            new EncounterStatic8N(Nest14,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest14,1,1,1) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest14,1,2,2) { Species = 525, Ability = A3 }, // Boldore
            new EncounterStatic8N(Nest14,2,3,3) { Species = 558, Ability = A3 }, // Crustle
            new EncounterStatic8N(Nest14,2,4,4) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest14,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest15,0,0,1) { Species = 050, Ability = A3 }, // Diglett
            new EncounterStatic8N(Nest15,0,0,1) { Species = 749, Ability = A3 }, // Mudbray
            new EncounterStatic8N(Nest15,0,1,1) { Species = 290, Ability = A3 }, // Nincada
            new EncounterStatic8N(Nest15,0,1,1) { Species = 529, Ability = A3 }, // Drilbur
            new EncounterStatic8N(Nest15,1,1,1) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest15,1,2,2) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest15,2,3,3) { Species = 208, Ability = A3 }, // Steelix
            new EncounterStatic8N(Nest15,2,4,4) { Species = 340, Ability = A3 }, // Whiscash
            new EncounterStatic8N(Nest15,2,4,4) { Species = 660, Ability = A3 }, // Diggersby
            new EncounterStatic8N(Nest15,3,4,4) { Species = 051, Ability = A4 }, // Dugtrio
            new EncounterStatic8N(Nest15,4,4,4) { Species = 530, Ability = A4 }, // Excadrill
            new EncounterStatic8N(Nest15,4,4,4) { Species = 750, Ability = A4 }, // Mudsdale
            new EncounterStatic8N(Nest16,0,0,1) { Species = 843, Ability = A3 }, // Silicobra
            new EncounterStatic8N(Nest16,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest16,0,1,1) { Species = 449, Ability = A3 }, // Hippopotas
            new EncounterStatic8N(Nest16,1,2,2) { Species = 221, Ability = A3 }, // Piloswine
            new EncounterStatic8N(Nest16,4,4,4) { Species = 867, Ability = A3 }, // Runerigus
            new EncounterStatic8N(Nest16,4,4,4) { Species = 844, Ability = A4 }, // Sandaconda
            new EncounterStatic8N(Nest17,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest17,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest17,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest17,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest18,0,0,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest18,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest18,1,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest18,4,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest19,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest19,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest19,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest19,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest19,3,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest19,4,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest20,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest20,0,0,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest20,0,1,1) { Species = 459, Ability = A3 }, // Snover
            new EncounterStatic8N(Nest20,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest20,1,1,1) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest20,1,2,2) { Species = 583, Ability = A3 }, // Vanillish
            new EncounterStatic8N(Nest20,2,3,3) { Species = 221, Ability = A3 }, // Piloswine
            new EncounterStatic8N(Nest20,2,4,4) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest20,2,4,4) { Species = 460, Ability = A3 }, // Abomasnow
            new EncounterStatic8N(Nest20,3,4,4) { Species = 091, Ability = A4 }, // Cloyster
            new EncounterStatic8N(Nest20,4,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest20,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest21,0,0,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest21,0,0,1) { Species = 613, Ability = A3 }, // Cubchoo
            new EncounterStatic8N(Nest21,0,1,1) { Species = 872, Ability = A3 }, // Snom
            new EncounterStatic8N(Nest21,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest21,1,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest21,1,2,2) { Species = 221, Ability = A3 }, // Piloswine
            new EncounterStatic8N(Nest21,2,3,3) { Species = 091, Ability = A3 }, // Cloyster
            new EncounterStatic8N(Nest21,2,4,4) { Species = 614, Ability = A3 }, // Beartic
            new EncounterStatic8N(Nest21,2,4,4) { Species = 866, Ability = A3 }, // Mr. Rime
            new EncounterStatic8N(Nest21,3,4,4) { Species = 473, Ability = A4 }, // Mamoswine
            new EncounterStatic8N(Nest21,4,4,4) { Species = 873, Ability = A4 }, // Frosmoth
            new EncounterStatic8N(Nest21,4,4,4) { Species = 461, Ability = A4 }, // Weavile
            new EncounterStatic8N(Nest22,0,0,1) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest22,0,0,1) { Species = 872, Ability = A3 }, // Snom
            new EncounterStatic8N(Nest22,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest22,1,1,2) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest22,1,2,3) { Species = 459, Ability = A3 }, // Snover
            new EncounterStatic8N(Nest22,2,3,3) { Species = 460, Ability = A3 }, // Abomasnow
            new EncounterStatic8N(Nest22,2,4,4) { Species = 362, Ability = A3 }, // Glalie
            new EncounterStatic8N(Nest22,2,4,4) { Species = 866, Ability = A3 }, // Mr. Rime
            new EncounterStatic8N(Nest22,3,4,4) { Species = 873, Ability = A4 }, // Frosmoth
            new EncounterStatic8N(Nest22,4,4,4) { Species = 478, Ability = A4 }, // Froslass
            new EncounterStatic8N(Nest23,0,0,1) { Species = 172, Ability = A3 }, // Pichu
            new EncounterStatic8N(Nest23,0,0,1) { Species = 309, Ability = A3 }, // Electrike
            new EncounterStatic8N(Nest23,0,1,1) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest23,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest23,1,1,2) { Species = 737, Ability = A3 }, // Charjabug
            new EncounterStatic8N(Nest23,1,2,3) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest23,2,3,3) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest23,2,4,4) { Species = 310, Ability = A3 }, // Manectric
            new EncounterStatic8N(Nest23,2,4,4) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest23,3,4,4) { Species = 596, Ability = A4 }, // Galvantula
            new EncounterStatic8N(Nest23,4,4,4) { Species = 738, Ability = A4 }, // Vikavolt
            new EncounterStatic8N(Nest23,4,4,4) { Species = 026, Ability = A4 }, // Raichu
            new EncounterStatic8N(Nest24,0,0,1) { Species = 835, Ability = A3 }, // Yamper
            new EncounterStatic8N(Nest24,0,0,1) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest24,0,1,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest24,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest24,1,1,2) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest24,1,2,3) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest24,2,3,3) { Species = 836, Ability = A3 }, // Boltund
            new EncounterStatic8N(Nest24,2,4,4) { Species = 695, Ability = A3 }, // Heliolisk
            new EncounterStatic8N(Nest24,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest24,3,4,4) { Species = 871, Ability = A4 }, // Pincurchin
            new EncounterStatic8N(Nest24,4,4,4) { Species = 777, Ability = A4 }, // Togedemaru
            new EncounterStatic8N(Nest24,4,4,4) { Species = 877, Ability = A4 }, // Morpeko
            new EncounterStatic8N(Nest25,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest25,0,1,1) { Species = 761, Ability = A3 }, // Bounsweet
            new EncounterStatic8N(Nest25,0,1,1) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest25,1,2,3) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest25,2,3,3) { Species = 044, Ability = A3 }, // Gloom
            new EncounterStatic8N(Nest25,2,4,4) { Species = 762, Ability = A3 }, // Steenee
            new EncounterStatic8N(Nest25,3,4,4) { Species = 763, Ability = A4 }, // Tsareena
            new EncounterStatic8N(Nest25,4,4,4) { Species = 045, Ability = A4 }, // Vileplume
            new EncounterStatic8N(Nest25,4,4,4) { Species = 182, Ability = A4 }, // Bellossom
            new EncounterStatic8N(Nest26,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest26,0,0,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest26,0,1,1) { Species = 546, Ability = A3 }, // Cottonee
            new EncounterStatic8N(Nest26,0,1,1) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest26,1,1,2) { Species = 420, Ability = A3 }, // Cherubi
            new EncounterStatic8N(Nest26,1,2,2) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest26,2,3,3) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest26,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest26,2,4,4) { Species = 421, Ability = A3 }, // Cherrim
            new EncounterStatic8N(Nest26,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest26,4,4,4) { Species = 547, Ability = A4 }, // Whimsicott
            new EncounterStatic8N(Nest27,0,0,1) { Species = 710, Ability = A3, Form = 1 }, // Pumpkaboo-1
            new EncounterStatic8N(Nest27,0,0,1) { Species = 708, Ability = A3 }, // Phantump
            new EncounterStatic8N(Nest27,0,1,1) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest27,0,1,1) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest27,1,1,2) { Species = 710, Ability = A3, Form = 2 }, // Pumpkaboo-2
            new EncounterStatic8N(Nest27,1,2,2) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest27,2,3,3) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest27,2,4,4) { Species = 556, Ability = A3 }, // Maractus
            new EncounterStatic8N(Nest27,2,4,4) { Species = 709, Ability = A3 }, // Trevenant
            new EncounterStatic8N(Nest27,3,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest27,4,4,4) { Species = 781, Ability = A4 }, // Dhelmise
            new EncounterStatic8N(Nest27,4,4,4) { Species = 710, Ability = A4, Form = 3 }, // Pumpkaboo-3
            new EncounterStatic8N(Nest28,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest28,0,0,1) { Species = 568, Ability = A3 }, // Trubbish
            new EncounterStatic8N(Nest28,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest28,1,2,2) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest28,2,3,3) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest28,2,4,4) { Species = 452, Ability = A3 }, // Drapion
            new EncounterStatic8N(Nest28,2,4,4) { Species = 045, Ability = A3 }, // Vileplume
            new EncounterStatic8N(Nest28,4,4,4) { Species = 569, Ability = A4 }, // Garbodor
            new EncounterStatic8N(Nest29,0,0,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest29,0,0,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest29,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest29,0,1,1) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest29,1,1,2) { Species = 044, Ability = A3 }, // Gloom
            new EncounterStatic8N(Nest29,1,2,2) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest29,2,3,3) { Species = 109, Ability = A3 }, // Koffing
            new EncounterStatic8N(Nest29,2,4,4) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest29,2,4,4) { Species = 045, Ability = A3 }, // Vileplume
            new EncounterStatic8N(Nest29,3,4,4) { Species = 315, Ability = A4 }, // Roselia
            new EncounterStatic8N(Nest29,4,4,4) { Species = 849, Ability = A4 }, // Toxtricity
            new EncounterStatic8N(Nest29,4,4,4) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest30,0,0,1) { Species = 519, Ability = A3 }, // Pidove
            new EncounterStatic8N(Nest30,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest30,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest30,1,1,2) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest30,1,2,2) { Species = 520, Ability = A3 }, // Tranquill
            new EncounterStatic8N(Nest30,2,3,3) { Species = 521, Ability = A3 }, // Unfezant
            new EncounterStatic8N(Nest30,2,4,4) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest30,2,4,4) { Species = 528, Ability = A3 }, // Swoobat
            new EncounterStatic8N(Nest30,3,4,4) { Species = 178, Ability = A4 }, // Xatu
            new EncounterStatic8N(Nest30,4,4,4) { Species = 561, Ability = A4 }, // Sigilyph
            new EncounterStatic8N(Nest31,0,0,1) { Species = 821, Ability = A3 }, // Rookidee
            new EncounterStatic8N(Nest31,0,0,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest31,0,1,1) { Species = 278, Ability = A3 }, // Wingull
            new EncounterStatic8N(Nest31,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest31,1,1,2) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest31,1,2,2) { Species = 822, Ability = A3 }, // Corvisquire
            new EncounterStatic8N(Nest31,2,3,3) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest31,2,4,4) { Species = 279, Ability = A3 }, // Pelipper
            new EncounterStatic8N(Nest31,2,4,4) { Species = 178, Ability = A3 }, // Xatu
            new EncounterStatic8N(Nest31,3,4,4) { Species = 823, Ability = A4 }, // Corviknight
            new EncounterStatic8N(Nest31,4,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest31,4,4,4) { Species = 845, Ability = A4 }, // Cramorant
            new EncounterStatic8N(Nest32,0,0,1) { Species = 173, Ability = A3 }, // Cleffa
            new EncounterStatic8N(Nest32,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest32,0,1,1) { Species = 742, Ability = A3 }, // Cutiefly
            new EncounterStatic8N(Nest32,1,1,2) { Species = 035, Ability = A3 }, // Clefairy
            new EncounterStatic8N(Nest32,1,2,2) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest32,2,3,3) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest32,2,4,4) { Species = 036, Ability = A3 }, // Clefable
            new EncounterStatic8N(Nest32,2,4,4) { Species = 743, Ability = A3 }, // Ribombee
            new EncounterStatic8N(Nest32,3,4,4) { Species = 756, Ability = A4 }, // Shiinotic
            new EncounterStatic8N(Nest32,4,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest33,0,0,1) { Species = 439, Ability = A3 }, // Mime Jr.
            new EncounterStatic8N(Nest33,0,0,1) { Species = 868, Ability = A3 }, // Milcery
            new EncounterStatic8N(Nest33,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest33,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest33,1,1,2) { Species = 035, Ability = A3 }, // Clefairy
            new EncounterStatic8N(Nest33,1,2,2) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest33,2,3,3) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest33,2,4,4) { Species = 036, Ability = A3 }, // Clefable
            new EncounterStatic8N(Nest33,2,4,4) { Species = 282, Ability = A3 }, // Gardevoir
            new EncounterStatic8N(Nest33,3,4,4) { Species = 869, Ability = A4 }, // Alcremie
            new EncounterStatic8N(Nest33,4,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest34,0,0,1) { Species = 509, Ability = A3 }, // Purrloin
            new EncounterStatic8N(Nest34,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest34,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest34,0,1,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest34,1,1,2) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest34,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest34,2,3,3) { Species = 435, Ability = A3 }, // Skuntank
            new EncounterStatic8N(Nest34,2,4,4) { Species = 461, Ability = A3 }, // Weavile
            new EncounterStatic8N(Nest34,2,4,4) { Species = 687, Ability = A3 }, // Malamar
            new EncounterStatic8N(Nest34,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest34,4,4,4) { Species = 342, Ability = A4 }, // Crawdaunt
            new EncounterStatic8N(Nest35,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest35,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest35,0,1,1) { Species = 509, Ability = A3 }, // Purrloin
            new EncounterStatic8N(Nest35,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest35,1,2,2) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest35,2,3,3) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest35,2,4,4) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest35,2,4,4) { Species = 861, Ability = A3 }, // Grimmsnarl
            new EncounterStatic8N(Nest35,4,4,4) { Species = 862, Ability = A4 }, // Obstagoon
            new EncounterStatic8N(Nest36,0,0,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest36,0,1,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest36,1,2,2) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest37,0,0,1) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest37,0,0,1) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest37,0,1,1) { Species = 885, Ability = A3 }, // Dreepy
            new EncounterStatic8N(Nest37,1,1,2) { Species = 714, Ability = A3 }, // Noibat
            new EncounterStatic8N(Nest37,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest37,2,3,3) { Species = 886, Ability = A3 }, // Drakloak
            new EncounterStatic8N(Nest37,2,4,4) { Species = 715, Ability = A3 }, // Noivern
            new EncounterStatic8N(Nest37,4,4,4) { Species = 887, Ability = A4 }, // Dragapult
            new EncounterStatic8N(Nest38,0,0,1) { Species = 659, Ability = A3 }, // Bunnelby
            new EncounterStatic8N(Nest38,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest38,0,1,1) { Species = 519, Ability = A3 }, // Pidove
            new EncounterStatic8N(Nest38,0,1,1) { Species = 572, Ability = A3 }, // Minccino
            new EncounterStatic8N(Nest38,1,1,2) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest38,1,2,2) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest38,2,3,3) { Species = 660, Ability = A3 }, // Diggersby
            new EncounterStatic8N(Nest38,2,4,4) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest38,2,4,4) { Species = 521, Ability = A3 }, // Unfezant
            new EncounterStatic8N(Nest38,3,4,4) { Species = 695, Ability = A4 }, // Heliolisk
            new EncounterStatic8N(Nest38,4,4,4) { Species = 573, Ability = A4 }, // Cinccino
            new EncounterStatic8N(Nest38,4,4,4) { Species = 760, Ability = A4 }, // Bewear
            new EncounterStatic8N(Nest39,0,0,1) { Species = 819, Ability = A3 }, // Skwovet
            new EncounterStatic8N(Nest39,0,0,1) { Species = 831, Ability = A3 }, // Wooloo
            new EncounterStatic8N(Nest39,0,1,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest39,0,1,1) { Species = 446, Ability = A3 }, // Munchlax
            new EncounterStatic8N(Nest39,1,2,2) { Species = 820, Ability = A3 }, // Greedent
            new EncounterStatic8N(Nest39,2,3,3) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest39,2,4,4) { Species = 820, Ability = A3 }, // Greedent
            new EncounterStatic8N(Nest39,2,4,4) { Species = 832, Ability = A3 }, // Dubwool
            new EncounterStatic8N(Nest39,3,4,4) { Species = 660, Ability = A4 }, // Diggersby
            new EncounterStatic8N(Nest39,4,4,4) { Species = 143, Ability = A4 }, // Snorlax
            new EncounterStatic8N(Nest40,0,0,1) { Species = 535, Ability = A3 }, // Tympole
            new EncounterStatic8N(Nest40,0,0,1) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest40,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest40,1,2,2) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest40,2,4,4) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest40,4,4,4) { Species = 847, Ability = A4 }, // Barraskewda
            new EncounterStatic8N(Nest41,0,0,1) { Species = 422, Ability = A3, Form = 1 }, // Shellos-1
            new EncounterStatic8N(Nest41,0,0,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest41,0,1,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest41,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest41,1,1,2) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest41,1,2,2) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest41,2,3,3) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest41,2,4,4) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest41,2,4,4) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest41,3,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest41,4,4,4) { Species = 593, Ability = A4 }, // Jellicent
            new EncounterStatic8N(Nest41,4,4,4) { Species = 834, Ability = A4 }, // Drednaw
            new EncounterStatic8N(Nest42,0,0,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest42,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest42,0,1,1) { Species = 854, Ability = A3 }, // Sinistea
            new EncounterStatic8N(Nest42,0,1,1) { Species = 355, Ability = A3 }, // Duskull
            new EncounterStatic8N(Nest42,1,2,2) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest42,2,3,3) { Species = 356, Ability = A3 }, // Dusclops
            new EncounterStatic8N(Nest42,4,4,4) { Species = 477, Ability = A4 }, // Dusknoir
            new EncounterStatic8N(Nest42,4,4,4) { Species = 094, Ability = A4 }, // Gengar
            new EncounterStatic8N(Nest43,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest43,0,0,1) { Species = 349, Ability = A3 }, // Feebas
            new EncounterStatic8N(Nest43,0,1,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest43,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest43,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest43,2,3,3) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest43,2,4,4) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest43,3,4,4) { Species = 771, Ability = A4 }, // Pyukumuku
            new EncounterStatic8N(Nest43,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest43,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest43,4,4,4) { Species = 350, Ability = A4 }, // Milotic
            new EncounterStatic8N(Nest44,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest44,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest44,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest44,0,1,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest44,1,2,2) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest44,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest44,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest44,3,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest44,4,4,4) { Species = 601, Ability = A4 }, // Klinklang
            new EncounterStatic8N(Nest44,4,4,4) { Species = 448, Ability = A4 }, // Lucario
            new EncounterStatic8N(Nest45,0,0,1) { Species = 767, Ability = A3 }, // Wimpod
            new EncounterStatic8N(Nest45,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest45,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest45,1,2,2) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest45,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest45,2,4,4) { Species = 826, Ability = A3 }, // Orbeetle
            new EncounterStatic8N(Nest45,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest45,3,4,4) { Species = 768, Ability = A4 }, // Golisopod
            new EncounterStatic8N(Nest45,4,4,4) { Species = 292, Ability = A4 }, // Shedinja
            new EncounterStatic8N(Nest46,0,0,1) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest46,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest46,0,1,1) { Species = 854, Ability = A3 }, // Sinistea
            new EncounterStatic8N(Nest46,0,1,1) { Species = 425, Ability = A3 }, // Drifloon
            new EncounterStatic8N(Nest46,1,2,2) { Species = 680, Ability = A3 }, // Doublade
            new EncounterStatic8N(Nest46,2,3,3) { Species = 426, Ability = A3 }, // Drifblim
            new EncounterStatic8N(Nest46,3,4,4) { Species = 855, Ability = A4 }, // Polteageist
            new EncounterStatic8N(Nest46,4,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest46,4,4,4) { Species = 681, Ability = A4 }, // Aegislash
            new EncounterStatic8N(Nest47,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest47,0,0,1) { Species = 066, Ability = A3 }, // Machop
            new EncounterStatic8N(Nest47,0,1,1) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest47,1,2,2) { Species = 760, Ability = A3 }, // Bewear
            new EncounterStatic8N(Nest47,1,3,3) { Species = 870, Ability = A3 }, // Falinks
            new EncounterStatic8N(Nest47,2,3,3) { Species = 067, Ability = A3 }, // Machoke
            new EncounterStatic8N(Nest47,3,4,4) { Species = 068, Ability = A4 }, // Machamp
            new EncounterStatic8N(Nest47,4,4,4) { Species = 448, Ability = A4 }, // Lucario
            new EncounterStatic8N(Nest47,4,4,4) { Species = 475, Ability = A4 }, // Gallade
            new EncounterStatic8N(Nest48,0,0,1) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest48,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest48,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest48,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest48,1,2,2) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest48,1,2,2) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest48,3,4,4) { Species = 863, Ability = A4 }, // Perrserker
            new EncounterStatic8N(Nest48,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest48,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest48,3,4,4) { Species = 618, Ability = A4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest48,4,4,4) { Species = 879, Ability = A4 }, // Copperajah
            new EncounterStatic8N(Nest48,4,4,4) { Species = 884, Ability = A4 }, // Duraludon
            new EncounterStatic8N(Nest49,0,0,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest49,0,0,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest49,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest49,0,1,1) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest49,1,2,2) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest49,1,2,2) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest49,2,3,3) { Species = 281, Ability = A3 }, // Kirlia
            new EncounterStatic8N(Nest49,2,4,4) { Species = 528, Ability = A3 }, // Swoobat
            new EncounterStatic8N(Nest49,3,4,4) { Species = 858, Ability = A4 }, // Hatterene
            new EncounterStatic8N(Nest49,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest49,4,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest49,4,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest50,0,0,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest50,0,0,1) { Species = 438, Ability = A3 }, // Bonsly
            new EncounterStatic8N(Nest50,0,1,1) { Species = 837, Ability = A3 }, // Rolycoly
            new EncounterStatic8N(Nest50,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest50,2,4,4) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest50,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest50,3,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest50,4,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest51,0,0,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest51,0,0,1) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest51,0,1,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest51,0,1,1) { Species = 622, Ability = A3 }, // Golett
            new EncounterStatic8N(Nest51,1,2,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest51,1,2,2) { Species = 195, Ability = A3 }, // Quagsire
            new EncounterStatic8N(Nest51,2,3,3) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest51,2,4,4) { Species = 623, Ability = A3 }, // Golurk
            new EncounterStatic8N(Nest51,3,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest51,3,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest51,4,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest51,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest52,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest52,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest52,0,1,1) { Species = 004, Ability = A3 }, // Charmander
            new EncounterStatic8N(Nest52,1,2,2) { Species = 005, Ability = A3 }, // Charmeleon
            new EncounterStatic8N(Nest52,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest52,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest52,3,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest52,4,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest52,4,4,4) { Species = 006, Ability = A4 }, // Charizard
            new EncounterStatic8N(Nest53,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest53,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest53,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest53,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest53,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest53,3,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest53,4,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest54,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest54,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest54,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest54,1,2,2) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest54,1,2,2) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest54,2,3,3) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest54,2,4,4) { Species = 362, Ability = A3 }, // Glalie
            new EncounterStatic8N(Nest54,3,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest54,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest54,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest55,0,0,1) { Species = 835, Ability = A3 }, // Yamper
            new EncounterStatic8N(Nest55,0,0,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest55,0,1,1) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest55,0,1,1) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest55,1,2,2) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest55,1,2,2) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest55,2,4,4) { Species = 836, Ability = A3 }, // Boltund
            new EncounterStatic8N(Nest55,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest55,3,4,4) { Species = 871, Ability = A4 }, // Pincurchin
            new EncounterStatic8N(Nest55,3,4,4) { Species = 596, Ability = A4 }, // Galvantula
            new EncounterStatic8N(Nest55,4,4,4) { Species = 777, Ability = A4 }, // Togedemaru
            new EncounterStatic8N(Nest55,4,4,4) { Species = 877, Ability = A4 }, // Morpeko
            new EncounterStatic8N(Nest56,0,0,1) { Species = 172, Ability = A3 }, // Pichu
            new EncounterStatic8N(Nest56,0,0,1) { Species = 309, Ability = A3 }, // Electrike
            new EncounterStatic8N(Nest56,0,1,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest56,0,1,1) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest56,1,2,2) { Species = 595, Ability = A3 }, // Joltik
            new EncounterStatic8N(Nest56,1,2,2) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest56,2,4,4) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest56,2,4,4) { Species = 479, Ability = A3, Form = 5 }, // Rotom-5
            new EncounterStatic8N(Nest56,3,4,4) { Species = 479, Ability = A4, Form = 4 }, // Rotom-4
            new EncounterStatic8N(Nest56,3,4,4) { Species = 479, Ability = A4, Form = 3 }, // Rotom-3
            new EncounterStatic8N(Nest56,4,4,4) { Species = 479, Ability = A4, Form = 2 }, // Rotom-2
            new EncounterStatic8N(Nest56,4,4,4) { Species = 479, Ability = A4, Form = 1 }, // Rotom-1
            new EncounterStatic8N(Nest57,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest57,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest57,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest57,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest57,2,4,4) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest57,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest57,3,4,4) { Species = 598, Ability = A4 }, // Ferrothorn
            new EncounterStatic8N(Nest57,4,4,4) { Species = 407, Ability = A4 }, // Roserade
            new EncounterStatic8N(Nest58,0,0,1) { Species = 420, Ability = A3 }, // Cherubi
            new EncounterStatic8N(Nest58,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest58,0,1,1) { Species = 546, Ability = A3 }, // Cottonee
            new EncounterStatic8N(Nest58,1,2,2) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest58,2,4,4) { Species = 421, Ability = A3 }, // Cherrim
            new EncounterStatic8N(Nest58,2,4,4) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest58,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest58,3,4,4) { Species = 547, Ability = A4 }, // Whimsicott
            new EncounterStatic8N(Nest58,4,4,4) { Species = 781, Ability = A4 }, // Dhelmise
            new EncounterStatic8N(Nest59,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest59,0,0,1) { Species = 568, Ability = A3 }, // Trubbish
            new EncounterStatic8N(Nest59,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest59,0,1,1) { Species = 109, Ability = A3 }, // Koffing
            new EncounterStatic8N(Nest59,1,2,2) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest59,2,4,4) { Species = 569, Ability = A3 }, // Garbodor
            new EncounterStatic8N(Nest59,2,4,4) { Species = 452, Ability = A3 }, // Drapion
            new EncounterStatic8N(Nest59,3,4,4) { Species = 849, Ability = A4 }, // Toxtricity
            new EncounterStatic8N(Nest59,3,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest59,4,4,4) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest60,0,0,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest60,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest60,0,1,1) { Species = 821, Ability = A3 }, // Rookidee
            new EncounterStatic8N(Nest60,0,1,1) { Species = 278, Ability = A3 }, // Wingull
            new EncounterStatic8N(Nest60,1,2,2) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest60,1,2,2) { Species = 822, Ability = A3 }, // Corvisquire
            new EncounterStatic8N(Nest60,2,4,4) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest60,2,4,4) { Species = 279, Ability = A3 }, // Pelipper
            new EncounterStatic8N(Nest60,3,4,4) { Species = 178, Ability = A4 }, // Xatu
            new EncounterStatic8N(Nest60,3,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest60,4,4,4) { Species = 823, Ability = A4 }, // Corviknight
            new EncounterStatic8N(Nest60,4,4,4) { Species = 225, Ability = A4 }, // Delibird
            new EncounterStatic8N(Nest61,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest61,0,0,1) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest61,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest61,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest61,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest61,1,2,2) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest61,2,4,4) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest61,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest61,3,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest61,4,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest61,4,4,4) { Species = 778, Ability = A4 }, // Mimikyu
            new EncounterStatic8N(Nest62,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest62,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest62,0,1,1) { Species = 215, Ability = A3 }, // Sneasel
            new EncounterStatic8N(Nest62,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest62,1,2,2) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest62,2,4,4) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest62,2,4,4) { Species = 675, Ability = A3 }, // Pangoro
            new EncounterStatic8N(Nest62,3,4,4) { Species = 461, Ability = A4 }, // Weavile
            new EncounterStatic8N(Nest62,4,4,4) { Species = 862, Ability = A4 }, // Obstagoon
            new EncounterStatic8N(Nest63,0,0,1) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest63,1,2,2) { Species = 885, Ability = A3 }, // Dreepy
            new EncounterStatic8N(Nest63,3,4,4) { Species = 886, Ability = A4 }, // Drakloak
            new EncounterStatic8N(Nest63,4,4,4) { Species = 887, Ability = A4 }, // Dragapult
            new EncounterStatic8N(Nest64,0,0,1) { Species = 659, Ability = A3 }, // Bunnelby
            new EncounterStatic8N(Nest64,0,0,1) { Species = 519, Ability = A3 }, // Pidove
            new EncounterStatic8N(Nest64,0,1,1) { Species = 819, Ability = A3 }, // Skwovet
            new EncounterStatic8N(Nest64,0,1,1) { Species = 133, Ability = A3 }, // Eevee
            new EncounterStatic8N(Nest64,1,2,2) { Species = 520, Ability = A3 }, // Tranquill
            new EncounterStatic8N(Nest64,1,2,2) { Species = 831, Ability = A3 }, // Wooloo
            new EncounterStatic8N(Nest64,2,4,4) { Species = 521, Ability = A3 }, // Unfezant
            new EncounterStatic8N(Nest64,2,4,4) { Species = 832, Ability = A3 }, // Dubwool
            new EncounterStatic8N(Nest64,4,4,4) { Species = 133, Ability = A4 }, // Eevee
            new EncounterStatic8N(Nest64,4,4,4) { Species = 143, Ability = A4 }, // Snorlax
            new EncounterStatic8N(Nest65,0,0,1) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest65,0,1,2) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest65,1,2,3) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest65,2,3,3) { Species = 132, Ability = A3 }, // Ditto
            new EncounterStatic8N(Nest65,3,4,4) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest65,4,4,4) { Species = 132, Ability = A4 }, // Ditto
            new EncounterStatic8N(Nest66,0,0,1) { Species = 458, Ability = A3 }, // Mantyke
            new EncounterStatic8N(Nest66,0,0,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest66,0,1,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest66,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest66,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest66,2,3,3) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest66,2,4,4) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest66,3,4,4) { Species = 771, Ability = A4 }, // Pyukumuku
            new EncounterStatic8N(Nest66,3,4,4) { Species = 226, Ability = A4 }, // Mantine
            new EncounterStatic8N(Nest66,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest66,4,4,4) { Species = 134, Ability = A4 }, // Vaporeon
            new EncounterStatic8N(Nest67,0,0,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest67,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest67,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest67,0,1,1) { Species = 527, Ability = A3 }, // Woobat
            new EncounterStatic8N(Nest67,1,2,2) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest67,1,2,2) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest67,2,3,3) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest67,2,4,4) { Species = 528, Ability = A3 }, // Swoobat
            new EncounterStatic8N(Nest67,3,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest67,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest67,4,4,4) { Species = 858, Ability = A4 }, // Hatterene
            new EncounterStatic8N(Nest67,4,4,4) { Species = 196, Ability = A4 }, // Espeon
            new EncounterStatic8N(Nest68,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest68,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest68,0,1,1) { Species = 686, Ability = A3 }, // Inkay
            new EncounterStatic8N(Nest68,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest68,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest68,1,2,2) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest68,2,4,4) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest68,2,4,4) { Species = 675, Ability = A3 }, // Pangoro
            new EncounterStatic8N(Nest68,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest68,3,4,4) { Species = 687, Ability = A4 }, // Malamar
            new EncounterStatic8N(Nest68,4,4,4) { Species = 862, Ability = A4 }, // Obstagoon
            new EncounterStatic8N(Nest68,4,4,4) { Species = 197, Ability = A4 }, // Umbreon
            new EncounterStatic8N(Nest69,0,0,1) { Species = 420, Ability = A3 }, // Cherubi
            new EncounterStatic8N(Nest69,0,0,1) { Species = 761, Ability = A3 }, // Bounsweet
            new EncounterStatic8N(Nest69,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest69,0,1,1) { Species = 546, Ability = A3 }, // Cottonee
            new EncounterStatic8N(Nest69,1,2,2) { Species = 762, Ability = A3 }, // Steenee
            new EncounterStatic8N(Nest69,1,2,2) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest69,2,4,4) { Species = 421, Ability = A3 }, // Cherrim
            new EncounterStatic8N(Nest69,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest69,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest69,3,4,4) { Species = 763, Ability = A4 }, // Tsareena
            new EncounterStatic8N(Nest69,4,4,4) { Species = 547, Ability = A4 }, // Whimsicott
            new EncounterStatic8N(Nest69,4,4,4) { Species = 470, Ability = A4 }, // Leafeon
            new EncounterStatic8N(Nest70,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest70,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest70,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest70,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest70,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest70,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest70,4,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest70,4,4,4) { Species = 136, Ability = A4 }, // Flareon
            new EncounterStatic8N(Nest71,0,0,1) { Species = 835, Ability = A3 }, // Yamper
            new EncounterStatic8N(Nest71,0,0,1) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest71,0,1,1) { Species = 025, Ability = A3 }, // Pikachu
            new EncounterStatic8N(Nest71,0,1,1) { Species = 694, Ability = A3 }, // Helioptile
            new EncounterStatic8N(Nest71,1,2,2) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest71,1,2,2) { Species = 171, Ability = A3 }, // Lanturn
            new EncounterStatic8N(Nest71,2,4,4) { Species = 836, Ability = A3 }, // Boltund
            new EncounterStatic8N(Nest71,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest71,3,4,4) { Species = 695, Ability = A4 }, // Heliolisk
            new EncounterStatic8N(Nest71,3,4,4) { Species = 738, Ability = A4 }, // Vikavolt
            new EncounterStatic8N(Nest71,4,4,4) { Species = 025, Ability = A4 }, // Pikachu
            new EncounterStatic8N(Nest71,4,4,4) { Species = 135, Ability = A4 }, // Jolteon
            new EncounterStatic8N(Nest72,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest72,0,0,1) { Species = 872, Ability = A3 }, // Snom
            new EncounterStatic8N(Nest72,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest72,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest72,1,2,2) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest72,1,2,2) { Species = 583, Ability = A3 }, // Vanillish
            new EncounterStatic8N(Nest72,2,3,3) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest72,2,4,4) { Species = 873, Ability = A3 }, // Frosmoth
            new EncounterStatic8N(Nest72,3,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest72,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest72,4,4,4) { Species = 478, Ability = A4 }, // Froslass
            new EncounterStatic8N(Nest72,4,4,4) { Species = 471, Ability = A4 }, // Glaceon
            new EncounterStatic8N(Nest73,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest73,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest73,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest73,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest73,1,2,2) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest73,2,4,4) { Species = 868, Ability = A3 }, // Milcery
            new EncounterStatic8N(Nest73,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest73,3,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest73,4,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest73,4,4,4) { Species = 700, Ability = A4 }, // Sylveon
            new EncounterStatic8N(Nest74,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest74,0,0,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest74,0,1,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest74,0,1,1) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest74,1,2,2) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest74,1,2,2) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest74,2,3,3) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest74,2,4,4) { Species = 340, Ability = A3 }, // Whiscash
            new EncounterStatic8N(Nest74,3,4,4) { Species = 211, Ability = A4 }, // Qwilfish
            new EncounterStatic8N(Nest74,3,4,4) { Species = 195, Ability = A4 }, // Quagsire
            new EncounterStatic8N(Nest74,4,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest74,4,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest75,0,0,1) { Species = 458, Ability = A3 }, // Mantyke
            new EncounterStatic8N(Nest75,0,0,1) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest75,0,1,1) { Species = 320, Ability = A3 }, // Wailmer
            new EncounterStatic8N(Nest75,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest75,1,2,2) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest75,1,2,2) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest75,2,3,3) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest75,3,4,4) { Species = 211, Ability = A4 }, // Qwilfish
            new EncounterStatic8N(Nest75,3,4,4) { Species = 224, Ability = A4 }, // Octillery
            new EncounterStatic8N(Nest75,4,4,4) { Species = 321, Ability = A4 }, // Wailord
            new EncounterStatic8N(Nest75,4,4,4) { Species = 226, Ability = A4 }, // Mantine
            new EncounterStatic8N(Nest76,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest76,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest76,0,1,1) { Species = 004, Ability = A3 }, // Charmander
            new EncounterStatic8N(Nest76,1,2,2) { Species = 005, Ability = A3 }, // Charmeleon
            new EncounterStatic8N(Nest76,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest76,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest76,3,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest76,4,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest76,4,4,4) { Species = 006, Ability = A4, CanGigantamax = true }, // Charizard
            new EncounterStatic8N(Nest77,0,0,1) { Species = 129, Ability = A3 }, // Magikarp
            new EncounterStatic8N(Nest77,0,0,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest77,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest77,0,1,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest77,1,2,2) { Species = 771, Ability = A3 }, // Pyukumuku
            new EncounterStatic8N(Nest77,2,3,3) { Species = 211, Ability = A3 }, // Qwilfish
            new EncounterStatic8N(Nest77,2,4,4) { Species = 099, Ability = A3 }, // Kingler
            new EncounterStatic8N(Nest77,3,4,4) { Species = 746, Ability = A4 }, // Wishiwashi
            new EncounterStatic8N(Nest77,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest77,4,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest77,4,4,4) { Species = 834, Ability = A4, CanGigantamax = true }, // Drednaw
            new EncounterStatic8N(Nest78,0,0,1) { Species = 406, Ability = A3 }, // Budew
            new EncounterStatic8N(Nest78,0,1,1) { Species = 829, Ability = A3 }, // Gossifleur
            new EncounterStatic8N(Nest78,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest78,1,2,2) { Species = 840, Ability = A3 }, // Applin
            new EncounterStatic8N(Nest78,2,4,4) { Species = 315, Ability = A3 }, // Roselia
            new EncounterStatic8N(Nest78,3,4,4) { Species = 830, Ability = A4 }, // Eldegoss
            new EncounterStatic8N(Nest78,3,4,4) { Species = 598, Ability = A4 }, // Ferrothorn
            new EncounterStatic8N(Nest78,4,4,4) { Species = 407, Ability = A4 }, // Roserade
            new EncounterStatic8N(Nest79,0,0,1) { Species = 850, Ability = A3 }, // Sizzlipede
            new EncounterStatic8N(Nest79,0,1,1) { Species = 607, Ability = A3 }, // Litwick
            new EncounterStatic8N(Nest79,0,1,1) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest79,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest79,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest79,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest79,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest79,3,4,4) { Species = 609, Ability = A4 }, // Chandelure
            new EncounterStatic8N(Nest79,4,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest79,4,4,4) { Species = 851, Ability = A4, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8N(Nest81,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest81,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest81,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest81,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest81,1,2,2) { Species = 756, Ability = A3 }, // Shiinotic
            new EncounterStatic8N(Nest81,2,3,3) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest81,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest81,3,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest81,4,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest81,4,4,4) { Species = 869, Ability = A4, CanGigantamax = true }, // Alcremie
            new EncounterStatic8N(Nest83,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest83,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest83,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest83,0,1,1) { Species = 599, Ability = A3 }, // Klink
            new EncounterStatic8N(Nest83,1,2,2) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest83,2,4,4) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest83,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest83,3,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest83,4,4,4) { Species = 601, Ability = A4 }, // Klinklang
            new EncounterStatic8N(Nest83,4,4,4) { Species = 884, Ability = A4, CanGigantamax = true }, // Duraludon
            new EncounterStatic8N(Nest84,0,0,1) { Species = 052, Ability = A3, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest84,0,0,1) { Species = 436, Ability = A3 }, // Bronzor
            new EncounterStatic8N(Nest84,0,1,1) { Species = 624, Ability = A3 }, // Pawniard
            new EncounterStatic8N(Nest84,0,1,1) { Species = 597, Ability = A3 }, // Ferroseed
            new EncounterStatic8N(Nest84,1,2,2) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest84,1,2,2) { Species = 437, Ability = A3 }, // Bronzong
            new EncounterStatic8N(Nest84,2,3,3) { Species = 863, Ability = A3 }, // Perrserker
            new EncounterStatic8N(Nest84,2,4,4) { Species = 598, Ability = A3 }, // Ferrothorn
            new EncounterStatic8N(Nest84,3,4,4) { Species = 625, Ability = A4 }, // Bisharp
            new EncounterStatic8N(Nest84,3,4,4) { Species = 618, Ability = A4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest84,4,4,4) { Species = 884, Ability = A4 }, // Duraludon
            new EncounterStatic8N(Nest84,4,4,4) { Species = 879, Ability = A4, CanGigantamax = true }, // Copperajah
            new EncounterStatic8N(Nest85,0,0,1) { Species = 434, Ability = A3 }, // Stunky
            new EncounterStatic8N(Nest85,0,0,1) { Species = 568, Ability = A3 }, // Trubbish
            new EncounterStatic8N(Nest85,0,1,1) { Species = 451, Ability = A3 }, // Skorupi
            new EncounterStatic8N(Nest85,0,1,1) { Species = 109, Ability = A3 }, // Koffing
            new EncounterStatic8N(Nest85,1,2,2) { Species = 848, Ability = A3 }, // Toxel
            new EncounterStatic8N(Nest85,2,3,3) { Species = 452, Ability = A3 }, // Drapion
            new EncounterStatic8N(Nest85,2,4,4) { Species = 849, Ability = A3 }, // Toxtricity
            new EncounterStatic8N(Nest85,3,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest85,3,4,4) { Species = 110, Ability = A4, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest85,4,4,4) { Species = 569, Ability = A4, CanGigantamax = true }, // Garbodor
            new EncounterStatic8N(Nest86,0,0,1) { Species = 175, Ability = A3 }, // Togepi
            new EncounterStatic8N(Nest86,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest86,0,1,1) { Species = 280, Ability = A3 }, // Ralts
            new EncounterStatic8N(Nest86,1,2,2) { Species = 176, Ability = A3 }, // Togetic
            new EncounterStatic8N(Nest86,1,2,2) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest86,2,4,4) { Species = 868, Ability = A3 }, // Milcery
            new EncounterStatic8N(Nest86,3,4,4) { Species = 282, Ability = A4 }, // Gardevoir
            new EncounterStatic8N(Nest86,3,4,4) { Species = 861, Ability = A4 }, // Grimmsnarl
            new EncounterStatic8N(Nest86,4,4,4) { Species = 468, Ability = A4 }, // Togekiss
            new EncounterStatic8N(Nest86,4,4,4) { Species = 858, Ability = A4, CanGigantamax = true }, // Hatterene
            new EncounterStatic8N(Nest87,0,0,1) { Species = 827, Ability = A3 }, // Nickit
            new EncounterStatic8N(Nest87,0,0,1) { Species = 263, Ability = A3, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest87,0,1,1) { Species = 859, Ability = A3 }, // Impidimp
            new EncounterStatic8N(Nest87,1,2,2) { Species = 510, Ability = A3 }, // Liepard
            new EncounterStatic8N(Nest87,1,2,2) { Species = 264, Ability = A3, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest87,2,3,3) { Species = 860, Ability = A3 }, // Morgrem
            new EncounterStatic8N(Nest87,2,4,4) { Species = 828, Ability = A3 }, // Thievul
            new EncounterStatic8N(Nest87,3,4,4) { Species = 675, Ability = A4 }, // Pangoro
            new EncounterStatic8N(Nest87,4,4,4) { Species = 861, Ability = A4, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8N(Nest88,0,0,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest88,0,0,1) { Species = 163, Ability = A3 }, // Hoothoot
            new EncounterStatic8N(Nest88,0,1,1) { Species = 821, Ability = A3 }, // Rookidee
            new EncounterStatic8N(Nest88,0,1,1) { Species = 278, Ability = A3 }, // Wingull
            new EncounterStatic8N(Nest88,1,2,2) { Species = 012, Ability = A3 }, // Butterfree
            new EncounterStatic8N(Nest88,1,2,2) { Species = 822, Ability = A3 }, // Corvisquire
            new EncounterStatic8N(Nest88,2,3,3) { Species = 164, Ability = A3 }, // Noctowl
            new EncounterStatic8N(Nest88,2,4,4) { Species = 279, Ability = A3 }, // Pelipper
            new EncounterStatic8N(Nest88,3,4,4) { Species = 178, Ability = A4 }, // Xatu
            new EncounterStatic8N(Nest88,3,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest88,4,4,4) { Species = 561, Ability = A4 }, // Sigilyph
            new EncounterStatic8N(Nest88,4,4,4) { Species = 823, Ability = A4, CanGigantamax = true }, // Corviknight
            new EncounterStatic8N(Nest89,0,0,1) { Species = 767, Ability = A3 }, // Wimpod
            new EncounterStatic8N(Nest89,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest89,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest89,1,2,2) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest89,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest89,2,4,4) { Species = 826, Ability = A3 }, // Orbeetle
            new EncounterStatic8N(Nest89,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest89,3,4,4) { Species = 768, Ability = A4 }, // Golisopod
            new EncounterStatic8N(Nest89,0,4,4) { Species = 012, Ability = A4, CanGigantamax = true }, // Butterfree
            new EncounterStatic8N(Nest90,0,0,1) { Species = 341, Ability = A3 }, // Corphish
            new EncounterStatic8N(Nest90,0,0,1) { Species = 098, Ability = A3 }, // Krabby
            new EncounterStatic8N(Nest90,0,1,1) { Species = 846, Ability = A3 }, // Arrokuda
            new EncounterStatic8N(Nest90,0,1,1) { Species = 833, Ability = A3 }, // Chewtle
            new EncounterStatic8N(Nest90,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest90,2,3,3) { Species = 342, Ability = A3 }, // Crawdaunt
            new EncounterStatic8N(Nest90,2,4,4) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest90,3,4,4) { Species = 771, Ability = A4 }, // Pyukumuku
            new EncounterStatic8N(Nest90,3,4,4) { Species = 130, Ability = A4 }, // Gyarados
            new EncounterStatic8N(Nest90,4,4,4) { Species = 131, Ability = A4 }, // Lapras
            new EncounterStatic8N(Nest90,1,4,4) { Species = 099, Ability = A4, CanGigantamax = true }, // Kingler
            new EncounterStatic8N(Nest91,0,0,1) { Species = 767, Ability = A3 }, // Wimpod
            new EncounterStatic8N(Nest91,0,0,1) { Species = 824, Ability = A3 }, // Blipbug
            new EncounterStatic8N(Nest91,0,1,1) { Species = 751, Ability = A3 }, // Dewpider
            new EncounterStatic8N(Nest91,1,2,2) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest91,2,3,3) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest91,2,4,4) { Species = 826, Ability = A3 }, // Orbeetle
            new EncounterStatic8N(Nest91,3,4,4) { Species = 752, Ability = A4 }, // Araquanid
            new EncounterStatic8N(Nest91,3,4,4) { Species = 768, Ability = A4 }, // Golisopod
            new EncounterStatic8N(Nest91,2,4,4) { Species = 826, Ability = A4, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8N(Nest92,0,0,1) { Species = 194, Ability = A3 }, // Wooper
            new EncounterStatic8N(Nest92,0,0,1) { Species = 339, Ability = A3 }, // Barboach
            new EncounterStatic8N(Nest92,0,1,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest92,0,1,1) { Species = 622, Ability = A3 }, // Golett
            new EncounterStatic8N(Nest92,1,2,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest92,1,2,2) { Species = 195, Ability = A3 }, // Quagsire
            new EncounterStatic8N(Nest92,2,3,3) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest92,2,4,4) { Species = 623, Ability = A3 }, // Golurk
            new EncounterStatic8N(Nest92,3,4,4) { Species = 423, Ability = A4, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest92,3,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest92,4,4,4) { Species = 464, Ability = A4 }, // Rhyperior
            new EncounterStatic8N(Nest92,3,4,4) { Species = 844, Ability = A4, CanGigantamax = true }, // Sandaconda
        };

        internal static readonly EncounterStatic8N[] Nest_SW =
        {
            new EncounterStatic8N(Nest00,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest00,2,3,3) { Species = 106, Ability = A3 }, // Hitmonlee
            new EncounterStatic8N(Nest00,2,4,4) { Species = 107, Ability = A3 }, // Hitmonchan
            new EncounterStatic8N(Nest00,2,4,4) { Species = 560, Ability = A3 }, // Scrafty
            new EncounterStatic8N(Nest00,3,4,4) { Species = 534, Ability = A4 }, // Conkeldurr
            new EncounterStatic8N(Nest00,4,4,4) { Species = 237, Ability = A4 }, // Hitmontop
            new EncounterStatic8N(Nest01,0,1,1) { Species = 574, Ability = A3 }, // Gothita
            new EncounterStatic8N(Nest01,2,3,3) { Species = 678, Ability = A3, Gender = 0 }, // Meowstic
            new EncounterStatic8N(Nest01,2,3,3) { Species = 575, Ability = A3 }, // Gothorita
            new EncounterStatic8N(Nest01,3,4,4) { Species = 576, Ability = A4 }, // Gothitelle
            new EncounterStatic8N(Nest01,4,4,4) { Species = 338, Ability = A4 }, // Solrock
            new EncounterStatic8N(Nest02,0,0,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest02,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest02,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest02,4,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest06,0,1,1) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest06,0,1,1) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest06,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest07,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest08,1,1,2) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest09,1,1,2) { Species = 083, Ability = A3, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8N(Nest09,1,2,2) { Species = 539, Ability = A3 }, // Sawk
            new EncounterStatic8N(Nest09,3,4,4) { Species = 865, Ability = A4 }, // Sirfetch’d
            new EncounterStatic8N(Nest11,4,4,4) { Species = 303, Ability = A4 }, // Mawile
            new EncounterStatic8N(Nest12,0,1,1) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest12,0,1,1) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest12,1,1,2) { Species = 825, Ability = A3 }, // Dottler
            new EncounterStatic8N(Nest12,1,3,2) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest12,2,4,4) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest12,3,4,4) { Species = 561, Ability = A4 }, // Sigilyph
            new EncounterStatic8N(Nest12,4,4,4) { Species = 826, Ability = A4 }, // Orbeetle
            new EncounterStatic8N(Nest13,2,4,4) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest14,0,0,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest14,0,1,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest14,2,4,4) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest14,3,4,4) { Species = 839, Ability = A4 }, // Coalossal
            new EncounterStatic8N(Nest14,4,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest16,0,1,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest16,1,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest16,2,3,3) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest16,2,4,4) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest16,3,4,4) { Species = 450, Ability = A4 }, // Hippowdon
            new EncounterStatic8N(Nest16,4,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest17,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest17,1,1,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest17,1,2,2) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest17,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest17,2,4,4) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest17,3,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest17,4,4,4) { Species = 631, Ability = A4 }, // Heatmor
            new EncounterStatic8N(Nest17,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest18,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest18,0,1,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest18,1,2,2) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest18,2,4,4) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest18,2,4,4) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest18,3,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,4,4,4) { Species = 776, Ability = A4 }, // Turtonator
            new EncounterStatic8N(Nest19,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest19,1,1,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest19,1,2,2) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest19,2,3,3) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest19,2,4,4) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest19,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest22,0,1,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest22,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest25,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest25,1,1,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest25,2,4,4) { Species = 275, Ability = A3 }, // Shiftry
            new EncounterStatic8N(Nest26,4,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest28,0,1,1) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest28,1,1,2) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest28,3,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest28,4,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest30,0,1,1) { Species = 627, Ability = A3 }, // Rufflet
            new EncounterStatic8N(Nest30,4,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest32,0,1,1) { Species = 684, Ability = A3 }, // Swirlix
            new EncounterStatic8N(Nest32,4,4,4) { Species = 685, Ability = A4 }, // Slurpuff
            new EncounterStatic8N(Nest33,4,4,4) { Species = 303, Ability = A4 }, // Mawile
            new EncounterStatic8N(Nest34,4,4,4) { Species = 275, Ability = A4 }, // Shiftry
            new EncounterStatic8N(Nest35,1,1,2) { Species = 633, Ability = A3 }, // Deino
            new EncounterStatic8N(Nest35,3,4,4) { Species = 634, Ability = A4 }, // Zweilous
            new EncounterStatic8N(Nest35,4,4,4) { Species = 635, Ability = A4 }, // Hydreigon
            new EncounterStatic8N(Nest36,0,0,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest36,0,1,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest36,1,1,2) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest36,2,3,3) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest36,2,4,4) { Species = 611, Ability = A3 }, // Fraxure
            new EncounterStatic8N(Nest36,2,4,4) { Species = 612, Ability = A3 }, // Haxorus
            new EncounterStatic8N(Nest36,3,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest36,4,4,4) { Species = 776, Ability = A4 }, // Turtonator
            new EncounterStatic8N(Nest36,4,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest37,0,1,1) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest37,2,4,4) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest37,3,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest37,4,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest39,1,1,2) { Species = 876, Ability = A3, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest39,4,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest40,0,1,1) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest40,1,1,2) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest40,2,3,3) { Species = 091, Ability = A3 }, // Cloyster
            new EncounterStatic8N(Nest40,2,4,4) { Species = 746, Ability = A3 }, // Wishiwashi
            new EncounterStatic8N(Nest40,3,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest40,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest42,1,3,2) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest42,4,4,4) { Species = 867, Ability = A3 }, // Runerigus
            new EncounterStatic8N(Nest42,3,4,4) { Species = 855, Ability = A4 }, // Polteageist
            new EncounterStatic8N(Nest42,3,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest43,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest44,1,2,2) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest44,2,3,3) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest45,0,1,1) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest45,1,2,2) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest45,4,4,4) { Species = 589, Ability = A4 }, // Escavalier
            new EncounterStatic8N(Nest46,1,3,3) { Species = 710, Ability = A3 }, // Pumpkaboo
            new EncounterStatic8N(Nest46,2,4,4) { Species = 711, Ability = A3 }, // Gourgeist
            new EncounterStatic8N(Nest46,3,4,4) { Species = 711, Ability = A4 }, // Gourgeist
            new EncounterStatic8N(Nest47,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest47,2,4,4) { Species = 560, Ability = A3 }, // Scrafty
            new EncounterStatic8N(Nest47,3,4,4) { Species = 766, Ability = A4 }, // Passimian
            new EncounterStatic8N(Nest50,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest50,1,2,2) { Species = 185, Ability = A3 }, // Sudowoodo
            new EncounterStatic8N(Nest50,2,3,3) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest50,4,4,4) { Species = 874, Ability = A4 }, // Stonjourner
            new EncounterStatic8N(Nest52,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest52,1,2,2) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest52,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest53,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest53,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest53,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest53,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest53,4,4,4) { Species = 776, Ability = A4 }, // Turtonator
            new EncounterStatic8N(Nest54,0,0,1) { Species = 554, Ability = A3, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest54,4,4,4) { Species = 555, Ability = A4, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest57,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest57,1,2,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest57,2,4,4) { Species = 275, Ability = A3 }, // Shiftry
            new EncounterStatic8N(Nest57,4,4,4) { Species = 841, Ability = A4 }, // Flapple
            new EncounterStatic8N(Nest58,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest58,1,2,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest58,4,4,4) { Species = 275, Ability = A4 }, // Shiftry
            new EncounterStatic8N(Nest59,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest59,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest61,2,4,4) { Species = 303, Ability = A3 }, // Mawile
            new EncounterStatic8N(Nest62,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest62,3,4,4) { Species = 560, Ability = A4 }, // Scrafty
            new EncounterStatic8N(Nest62,4,4,4) { Species = 635, Ability = A4 }, // Hydreigon
            new EncounterStatic8N(Nest63,0,0,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest63,0,1,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest63,0,1,1) { Species = 782, Ability = A3 }, // Jangmo-o
            new EncounterStatic8N(Nest63,1,2,2) { Species = 611, Ability = A3 }, // Fraxure
            new EncounterStatic8N(Nest63,2,4,4) { Species = 783, Ability = A3 }, // Hakamo-o
            new EncounterStatic8N(Nest63,2,4,4) { Species = 776, Ability = A3 }, // Turtonator
            new EncounterStatic8N(Nest63,3,4,4) { Species = 784, Ability = A4 }, // Kommo-o
            new EncounterStatic8N(Nest63,4,4,4) { Species = 612, Ability = A4 }, // Haxorus
            new EncounterStatic8N(Nest64,3,4,4) { Species = 628, Ability = A4 }, // Braviary
            new EncounterStatic8N(Nest64,3,4,4) { Species = 876, Ability = A4, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest66,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest70,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest70,0,1,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest70,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest70,2,3,3) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest73,0,0,1) { Species = 684, Ability = A3 }, // Swirlix
            new EncounterStatic8N(Nest73,2,4,4) { Species = 685, Ability = A3 }, // Slurpuff
            new EncounterStatic8N(Nest75,2,4,4) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest76,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest76,1,2,2) { Species = 038, Ability = A3 }, // Ninetales
            new EncounterStatic8N(Nest76,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest77,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest78,0,0,1) { Species = 273, Ability = A3 }, // Seedot
            new EncounterStatic8N(Nest78,1,2,2) { Species = 274, Ability = A3 }, // Nuzleaf
            new EncounterStatic8N(Nest78,2,4,4) { Species = 275, Ability = A3 }, // Shiftry
            new EncounterStatic8N(Nest78,4,4,4) { Species = 841, Ability = A4, CanGigantamax = true }, // Flapple
            new EncounterStatic8N(Nest79,0,0,1) { Species = 037, Ability = A3 }, // Vulpix
            new EncounterStatic8N(Nest79,3,4,4) { Species = 038, Ability = A4 }, // Ninetales
            new EncounterStatic8N(Nest80,0,0,1) { Species = 447, Ability = A3 }, // Riolu
            new EncounterStatic8N(Nest80,0,0,1) { Species = 066, Ability = A3 }, // Machop
            new EncounterStatic8N(Nest80,0,1,1) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest80,0,1,1) { Species = 083, Ability = A3, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8N(Nest80,1,2,2) { Species = 760, Ability = A3 }, // Bewear
            new EncounterStatic8N(Nest80,1,3,3) { Species = 067, Ability = A3 }, // Machoke
            new EncounterStatic8N(Nest80,2,3,3) { Species = 870, Ability = A3 }, // Falinks
            new EncounterStatic8N(Nest80,2,4,4) { Species = 701, Ability = A3 }, // Hawlucha
            new EncounterStatic8N(Nest80,3,4,4) { Species = 448, Ability = A4 }, // Lucario
            new EncounterStatic8N(Nest80,3,4,4) { Species = 475, Ability = A4 }, // Gallade
            new EncounterStatic8N(Nest80,4,4,4) { Species = 865, Ability = A4 }, // Sirfetch’d
            new EncounterStatic8N(Nest80,4,4,4) { Species = 068, Ability = A4, CanGigantamax = true }, // Machamp
            new EncounterStatic8N(Nest81,0,0,1) { Species = 755, Ability = A3 }, // Morelull
            new EncounterStatic8N(Nest81,2,4,4) { Species = 303, Ability = A3 }, // Mawile
            new EncounterStatic8N(Nest82,0,0,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest82,0,0,1) { Species = 438, Ability = A3 }, // Bonsly
            new EncounterStatic8N(Nest82,0,1,1) { Species = 837, Ability = A3 }, // Rolycoly
            new EncounterStatic8N(Nest82,0,1,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest82,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest82,1,2,2) { Species = 185, Ability = A3 }, // Sudowoodo
            new EncounterStatic8N(Nest82,2,3,3) { Species = 689, Ability = A3 }, // Barbaracle
            new EncounterStatic8N(Nest82,2,4,4) { Species = 095, Ability = A3 }, // Onix
            new EncounterStatic8N(Nest82,3,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest82,3,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest82,4,4,4) { Species = 874, Ability = A4 }, // Stonjourner
            new EncounterStatic8N(Nest82,4,4,4) { Species = 839, Ability = A4, CanGigantamax = true }, // Coalossal
            new EncounterStatic8N(Nest83,1,2,2) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest83,2,3,3) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest85,1,2,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest85,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest86,0,0,1) { Species = 684, Ability = A3 }, // Swirlix
            new EncounterStatic8N(Nest86,2,3,3) { Species = 685, Ability = A3 }, // Slurpuff
            new EncounterStatic8N(Nest87,0,1,1) { Species = 559, Ability = A3 }, // Scraggy
            new EncounterStatic8N(Nest87,3,4,4) { Species = 560, Ability = A4 }, // Scrafty
            new EncounterStatic8N(Nest87,4,4,4) { Species = 635, Ability = A4 }, // Hydreigon
            new EncounterStatic8N(Nest89,0,1,1) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest89,1,2,2) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest89,4,4,4) { Species = 589, Ability = A4 }, // Escavalier
            new EncounterStatic8N(Nest90,1,2,2) { Species = 550, Ability = A3 }, // Basculin
            new EncounterStatic8N(Nest91,0,1,1) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest91,1,2,2) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest91,4,4,4) { Species = 589, Ability = A4 }, // Escavalier
        };

        internal static readonly EncounterStatic8N[] Nest_SH =
        {
            new EncounterStatic8N(Nest00,0,1,1) { Species = 453, Ability = A3 }, // Croagunk
            new EncounterStatic8N(Nest00,2,3,3) { Species = 107, Ability = A3 }, // Hitmonchan
            new EncounterStatic8N(Nest00,2,4,4) { Species = 106, Ability = A3 }, // Hitmonlee
            new EncounterStatic8N(Nest00,2,4,4) { Species = 454, Ability = A3 }, // Toxicroak
            new EncounterStatic8N(Nest00,3,4,4) { Species = 237, Ability = A4 }, // Hitmontop
            new EncounterStatic8N(Nest00,4,4,4) { Species = 534, Ability = A4 }, // Conkeldurr
            new EncounterStatic8N(Nest01,0,1,1) { Species = 577, Ability = A3 }, // Solosis
            new EncounterStatic8N(Nest01,2,3,3) { Species = 678, Ability = A3, Gender = 1, Form = 1 }, // Meowstic-1
            new EncounterStatic8N(Nest01,2,3,3) { Species = 578, Ability = A3 }, // Duosion
            new EncounterStatic8N(Nest01,3,4,4) { Species = 579, Ability = A4 }, // Reuniclus
            new EncounterStatic8N(Nest01,4,4,4) { Species = 337, Ability = A4 }, // Lunatone
            new EncounterStatic8N(Nest02,0,0,1) { Species = 688, Ability = A3 }, // Binacle
            new EncounterStatic8N(Nest02,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest02,3,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest02,4,4,4) { Species = 558, Ability = A4 }, // Crustle
            new EncounterStatic8N(Nest06,0,1,2) { Species = 223, Ability = A3 }, // Remoraid
            new EncounterStatic8N(Nest06,0,1,2) { Species = 170, Ability = A3 }, // Chinchou
            new EncounterStatic8N(Nest06,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest07,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest08,1,1,1) { Species = 090, Ability = A3 }, // Shellder
            new EncounterStatic8N(Nest09,1,1,2) { Species = 759, Ability = A3 }, // Stufful
            new EncounterStatic8N(Nest09,1,2,2) { Species = 538, Ability = A3 }, // Throh
            new EncounterStatic8N(Nest09,3,4,4) { Species = 760, Ability = A4 }, // Bewear
            new EncounterStatic8N(Nest11,4,4,4) { Species = 208, Ability = A4 }, // Steelix
            new EncounterStatic8N(Nest12,0,1,2) { Species = 177, Ability = A3 }, // Natu
            new EncounterStatic8N(Nest12,0,1,2) { Species = 856, Ability = A3 }, // Hatenna
            new EncounterStatic8N(Nest12,1,1,2) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new EncounterStatic8N(Nest12,1,3,3) { Species = 857, Ability = A3 }, // Hattrem
            new EncounterStatic8N(Nest12,2,4,4) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest12,3,4,4) { Species = 765, Ability = A4 }, // Oranguru
            new EncounterStatic8N(Nest12,4,4,4) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest13,2,4,4) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest14,0,0,1) { Species = 557, Ability = A3 }, // Dwebble
            new EncounterStatic8N(Nest14,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest14,2,4,4) { Species = 839, Ability = A3 }, // Coalossal
            new EncounterStatic8N(Nest14,3,4,4) { Species = 526, Ability = A4 }, // Gigalith
            new EncounterStatic8N(Nest14,4,4,4) { Species = 095, Ability = A4 }, // Onix
            new EncounterStatic8N(Nest16,0,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest16,1,1,1) { Species = 220, Ability = A3 }, // Swinub
            new EncounterStatic8N(Nest16,2,3,3) { Species = 618, Ability = A3, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest16,2,4,4) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest16,3,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest16,4,4,4) { Species = 450, Ability = A4 }, // Hippowdon
            new EncounterStatic8N(Nest17,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest17,1,1,1) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest17,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest17,2,3,3) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest17,2,4,4) { Species = 059, Ability = A3 }, // Arcanine
            new EncounterStatic8N(Nest17,3,4,4) { Species = 631, Ability = A4 }, // Heatmor
            new EncounterStatic8N(Nest17,4,4,4) { Species = 851, Ability = A4 }, // Centiskorch
            new EncounterStatic8N(Nest17,4,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest18,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest18,0,1,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest18,1,2,2) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest18,2,3,3) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,2,4,4) { Species = 059, Ability = A3 }, // Arcanine
            new EncounterStatic8N(Nest18,2,4,4) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,3,4,4) { Species = 324, Ability = A4 }, // Torkoal
            new EncounterStatic8N(Nest18,4,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest19,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest19,1,1,1) { Species = 324, Ability = A3 }, // Torkoal
            new EncounterStatic8N(Nest19,1,2,2) { Species = 838, Ability = A3 }, // Carkol
            new EncounterStatic8N(Nest19,2,3,3) { Species = 758, Ability = A3, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest19,2,4,4) { Species = 059, Ability = A3 }, // Arcanine
            new EncounterStatic8N(Nest19,4,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest22,0,1,1) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest22,4,4,4) { Species = 875, Ability = A4 }, // Eiscue
            new EncounterStatic8N(Nest25,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest25,1,1,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest25,2,4,4) { Species = 272, Ability = A3 }, // Ludicolo
            new EncounterStatic8N(Nest26,4,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest28,0,1,1) { Species = 043, Ability = A3 }, // Oddish
            new EncounterStatic8N(Nest28,1,1,1) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest28,3,4,4) { Species = 435, Ability = A4 }, // Skuntank
            new EncounterStatic8N(Nest28,4,4,4) { Species = 748, Ability = A4 }, // Toxapex
            new EncounterStatic8N(Nest30,0,1,1) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest30,4,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest32,0,1,1) { Species = 682, Ability = A3 }, // Spritzee
            new EncounterStatic8N(Nest32,4,4,4) { Species = 683, Ability = A4 }, // Aromatisse
            new EncounterStatic8N(Nest33,4,4,4) { Species = 078, Ability = A4, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest34,4,4,4) { Species = 302, Ability = A4 }, // Sableye
            new EncounterStatic8N(Nest35,1,1,2) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest35,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest35,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest36,0,0,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest36,0,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest36,1,1,2) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest36,2,3,3) { Species = 611, Ability = A3 }, // Fraxure
            new EncounterStatic8N(Nest36,2,4,4) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest36,2,4,4) { Species = 330, Ability = A3 }, // Flygon
            new EncounterStatic8N(Nest36,3,4,4) { Species = 612, Ability = A4 }, // Haxorus
            new EncounterStatic8N(Nest36,4,4,4) { Species = 780, Ability = A4 }, // Drampa
            new EncounterStatic8N(Nest36,4,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest37,0,1,1) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest37,2,4,4) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest37,3,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest37,4,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest39,1,1,2) { Species = 876, Ability = A3, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest39,4,4,4) { Species = 765, Ability = A4 }, // Oranguru
            new EncounterStatic8N(Nest40,0,1,1) { Species = 536, Ability = A3 }, // Palpitoad
            new EncounterStatic8N(Nest40,1,1,2) { Species = 747, Ability = A3 }, // Mareanie
            new EncounterStatic8N(Nest40,2,3,3) { Species = 748, Ability = A3 }, // Toxapex
            new EncounterStatic8N(Nest40,2,4,4) { Species = 091, Ability = A3 }, // Cloyster
            new EncounterStatic8N(Nest40,3,4,4) { Species = 746, Ability = A4 }, // Wishiwashi
            new EncounterStatic8N(Nest40,4,4,4) { Species = 537, Ability = A4 }, // Seismitoad
            new EncounterStatic8N(Nest42,1,3,3) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest42,4,4,4) { Species = 302, Ability = A3 }, // Sableye
            new EncounterStatic8N(Nest42,3,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest42,3,4,4) { Species = 864, Ability = A4 }, // Cursola
            new EncounterStatic8N(Nest43,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest44,1,2,2) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest44,2,3,3) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest45,0,1,1) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest45,1,2,2) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest45,4,4,4) { Species = 617, Ability = A4 }, // Accelgor
            new EncounterStatic8N(Nest46,1,3,3) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest46,2,4,4) { Species = 302, Ability = A3 }, // Sableye
            new EncounterStatic8N(Nest46,3,4,4) { Species = 864, Ability = A4 }, // Cursola
            new EncounterStatic8N(Nest47,0,1,1) { Species = 453, Ability = A3 }, // Croagunk
            new EncounterStatic8N(Nest47,2,4,4) { Species = 454, Ability = A3 }, // Toxicroak
            new EncounterStatic8N(Nest47,3,4,4) { Species = 701, Ability = A4 }, // Hawlucha
            new EncounterStatic8N(Nest50,0,1,1) { Species = 524, Ability = A3 }, // Roggenrola
            new EncounterStatic8N(Nest50,1,2,2) { Species = 246, Ability = A3 }, // Larvitar
            new EncounterStatic8N(Nest50,2,3,3) { Species = 247, Ability = A3 }, // Pupitar
            new EncounterStatic8N(Nest50,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest52,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest52,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest52,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest53,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest53,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest53,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest53,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest53,4,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest54,0,0,1) { Species = 613, Ability = A3 }, // Cubchoo
            new EncounterStatic8N(Nest54,4,4,4) { Species = 875, Ability = A4 }, // Eiscue
            new EncounterStatic8N(Nest57,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest57,1,2,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest57,2,4,4) { Species = 272, Ability = A3 }, // Ludicolo
            new EncounterStatic8N(Nest57,4,4,4) { Species = 842, Ability = A4 }, // Appletun
            new EncounterStatic8N(Nest58,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest58,1,2,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest58,4,4,4) { Species = 272, Ability = A4 }, // Ludicolo
            new EncounterStatic8N(Nest59,1,2,2) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest59,4,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest61,2,4,4) { Species = 078, Ability = A3, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest62,0,1,1) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest62,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest62,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest63,0,0,1) { Species = 610, Ability = A3 }, // Axew
            new EncounterStatic8N(Nest63,0,1,1) { Species = 328, Ability = A3 }, // Trapinch
            new EncounterStatic8N(Nest63,0,1,1) { Species = 704, Ability = A3 }, // Goomy
            new EncounterStatic8N(Nest63,1,2,2) { Species = 329, Ability = A3 }, // Vibrava
            new EncounterStatic8N(Nest63,2,4,4) { Species = 705, Ability = A3 }, // Sliggoo
            new EncounterStatic8N(Nest63,2,4,4) { Species = 780, Ability = A3 }, // Drampa
            new EncounterStatic8N(Nest63,3,4,4) { Species = 706, Ability = A4 }, // Goodra
            new EncounterStatic8N(Nest63,4,4,4) { Species = 330, Ability = A4 }, // Flygon
            new EncounterStatic8N(Nest64,3,4,4) { Species = 765, Ability = A4 }, // Oranguru
            new EncounterStatic8N(Nest64,3,4,4) { Species = 876, Ability = A4, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest66,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest70,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest70,0,1,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest70,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest70,2,3,3) { Species = 608, Ability = A3 }, // Lampent
            new EncounterStatic8N(Nest73,0,0,1) { Species = 682, Ability = A3 }, // Spritzee
            new EncounterStatic8N(Nest73,2,4,4) { Species = 683, Ability = A3 }, // Aromatisse
            new EncounterStatic8N(Nest75,2,4,4) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest76,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest76,1,2,2) { Species = 631, Ability = A3 }, // Heatmor
            new EncounterStatic8N(Nest76,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest77,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest78,0,0,1) { Species = 270, Ability = A3 }, // Lotad
            new EncounterStatic8N(Nest78,1,2,2) { Species = 271, Ability = A3 }, // Lombre
            new EncounterStatic8N(Nest78,2,4,4) { Species = 272, Ability = A3 }, // Ludicolo
            new EncounterStatic8N(Nest78,4,4,4) { Species = 842, Ability = A4, CanGigantamax = true }, // Appletun
            new EncounterStatic8N(Nest79,0,0,1) { Species = 058, Ability = A3 }, // Growlithe
            new EncounterStatic8N(Nest79,3,4,4) { Species = 059, Ability = A4 }, // Arcanine
            new EncounterStatic8N(Nest80,0,0,1) { Species = 679, Ability = A3 }, // Honedge
            new EncounterStatic8N(Nest80,0,0,1) { Species = 562, Ability = A3, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest80,0,1,1) { Species = 854, Ability = A3 }, // Sinistea
            new EncounterStatic8N(Nest80,0,1,1) { Species = 092, Ability = A3 }, // Gastly
            new EncounterStatic8N(Nest80,1,2,2) { Species = 680, Ability = A3 }, // Doublade
            new EncounterStatic8N(Nest80,1,3,3) { Species = 222, Ability = A3, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest80,2,3,3) { Species = 093, Ability = A3 }, // Haunter
            new EncounterStatic8N(Nest80,2,4,4) { Species = 302, Ability = A3 }, // Sableye
            new EncounterStatic8N(Nest80,3,4,4) { Species = 855, Ability = A4 }, // Polteageist
            new EncounterStatic8N(Nest80,3,4,4) { Species = 864, Ability = A4 }, // Cursola
            new EncounterStatic8N(Nest80,4,4,4) { Species = 867, Ability = A4 }, // Runerigus
            new EncounterStatic8N(Nest80,4,4,4) { Species = 094, Ability = A4, CanGigantamax = true }, // Gengar
            new EncounterStatic8N(Nest81,0,0,1) { Species = 077, Ability = A3, Form = 1 }, // Ponyta-1
            new EncounterStatic8N(Nest81,2,4,4) { Species = 078, Ability = A3, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest82,0,0,1) { Species = 582, Ability = A3 }, // Vanillite
            new EncounterStatic8N(Nest82,0,0,1) { Species = 613, Ability = A3 }, // Cubchoo
            new EncounterStatic8N(Nest82,0,1,1) { Species = 122, Ability = A3, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest82,0,1,1) { Species = 712, Ability = A3 }, // Bergmite
            new EncounterStatic8N(Nest82,1,2,2) { Species = 361, Ability = A3 }, // Snorunt
            new EncounterStatic8N(Nest82,1,2,2) { Species = 225, Ability = A3 }, // Delibird
            new EncounterStatic8N(Nest82,2,3,3) { Species = 713, Ability = A3 }, // Avalugg
            new EncounterStatic8N(Nest82,2,4,4) { Species = 362, Ability = A3 }, // Glalie
            new EncounterStatic8N(Nest82,3,4,4) { Species = 584, Ability = A4 }, // Vanilluxe
            new EncounterStatic8N(Nest82,3,4,4) { Species = 866, Ability = A4 }, // Mr. Rime
            new EncounterStatic8N(Nest82,4,4,4) { Species = 875, Ability = A4 }, // Eiscue
            new EncounterStatic8N(Nest82,4,4,4) { Species = 131, Ability = A4, CanGigantamax = true }, // Lapras
            new EncounterStatic8N(Nest83,1,2,2) { Species = 600, Ability = A3 }, // Klang
            new EncounterStatic8N(Nest83,2,3,3) { Species = 632, Ability = A3 }, // Durant
            new EncounterStatic8N(Nest85,1,2,2) { Species = 757, Ability = A3 }, // Salandit
            new EncounterStatic8N(Nest85,4,4,4) { Species = 758, Ability = A4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest86,0,0,1) { Species = 682, Ability = A3 }, // Spritzee
            new EncounterStatic8N(Nest86,2,3,3) { Species = 683, Ability = A3 }, // Aromatisse
            new EncounterStatic8N(Nest87,0,1,1) { Species = 629, Ability = A3 }, // Vullaby
            new EncounterStatic8N(Nest87,3,4,4) { Species = 630, Ability = A4 }, // Mandibuzz
            new EncounterStatic8N(Nest87,4,4,4) { Species = 248, Ability = A4 }, // Tyranitar
            new EncounterStatic8N(Nest89,0,1,1) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest89,1,2,2) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest89,4,4,4) { Species = 617, Ability = A4 }, // Accelgor
            new EncounterStatic8N(Nest90,1,2,2) { Species = 550, Ability = A3, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest91,0,1,1) { Species = 616, Ability = A3 }, // Shelmet
            new EncounterStatic8N(Nest91,1,2,2) { Species = 588, Ability = A3 }, // Karrablast
            new EncounterStatic8N(Nest91,4,4,4) { Species = 617, Ability = A4 }, // Accelgor
        };
        #endregion
    }
}
