using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

internal static class Encounters8Nest
{
    internal static readonly byte[][] NestLocations =
    [
        [144, 134, 122],      // 000 : Stony Wilderness, South Lake Miloch, Rolling Fields
        [144, 126],           // 001 : Stony Wilderness, Watchtower Ruins
        [144, 122],           // 002 : Stony Wilderness, Rolling Fields
        [142, 124, 122],      // 003 : Bridge Field, Dappled Grove, Rolling Fields
        [142, 134],           // 004 : Bridge Field, South Lake Miloch
        [144, 126],           // 005 : Stony Wilderness, Watchtower Ruins
        [128, 130],           // 006 : East Lake Axewell, West Lake Axewell
        [154, 142, 134],      // 007 : Lake of Outrage, Bridge Field, South Lake Miloch
        [146, 130],           // 008 : Dusty Bowl, West Lake Axewell
        [146, 138],           // 009 : Dusty Bowl, North Lake Miloch
        [146, 136],           // 010 : Dusty Bowl, Giants Seat
        [150, 144, 136],      // 011 : Hammerlocke Hills, Stony Wilderness, Giants Seat
        [142],                // 012 : Bridge Field
        [150, 144, 140],      // 013 : Hammerlocke Hills, Stony Wilderness, Motostoke Riverbank
        [146, 136],           // 014 : Dusty Bowl, Giants Seat
        [142, 122],           // 015 : Bridge Field, Rolling Fields
        [146],                // 016 : Dusty Bowl
        [154, 152, 144],      // 017 : Lake of Outrage, Giants Cap, Stony Wilderness
        [150, 144],           // 018 : Hammerlocke Hills, Stony Wilderness
        [146],                // 019 : Dusty Bowl
        [146],                // 020 : Dusty Bowl
        [144],                // 021 : Stony Wilderness
        [150, 152],           // 022 : Hammerlocke Hills, Giants Cap
        [152, 140],           // 023 : Giants Cap, Motostoke Riverbank
        [154, 148],           // 024 : Lake of Outrage, Giants Mirror
        [124],                // 025 : Dappled Grove
        [148, 144, 142],      // 026 : Giants Mirror, Stony Wilderness, Bridge Field
        [148, 124, 146],      // 027 : Giants Mirror, Dappled Grove AND Dusty Bowl (Giant's Mirror load-line overlap)
        [138, 128],           // 028 : North Lake Miloch, East Lake Axewell
        [150, 152, 140],      // 029 : Hammerlocke Hills, Giants Cap, Motostoke Riverbank
        [128, 122],           // 030 : East Lake Axewell, Rolling Fields
        [150, 152],           // 031 : Hammerlocke Hills, Giants Cap
        [150, 122],           // 032 : Hammerlocke Hills, Rolling Fields
        [154, 142],           // 033 : Lake of Outrage, Bridge Field
        [144, 130],           // 034 : Stony Wilderness, West Lake Axewell
        [142, 146, 148],      // 035 : Bridge Field, Dusty Bowl, Giants Mirror
        [122],                // 036 : Rolling Fields
        [132],                // 037 : Axew's Eye
        [128, 122],           // 038 : East Lake Axewell, Rolling Fields
        [144, 142, 140],      // 039 : Stony Wilderness, Bridge Field, Motostoke Riverbank
        [134, 138],           // 040 : South Lake Miloch, North Lake Miloch
        [148, 130],           // 041 : Giants Mirror, West Lake Axewell
        [148, 144, 134, 146], // 042 : Giants Mirror, Stony Wilderness, South Lake Miloch AND Dusty Bowl (Giant's Mirror load-line overlap)
        [154, 142, 128, 130], // 043 : Lake of Outrage, Bridge Field, East Lake Axewell, West Lake Axewell 
        [150, 136],           // 044 : Hammerlocke Hills, Giants Seat
        [142, 134, 122],      // 045 : Bridge Field, South Lake Miloch, Rolling Fields
        [126],                // 046 : Watchtower Ruins
        [146, 138, 122, 134], // 047 : Dusty Bowl, North Lake Miloch, Rolling Fields, South Lake Miloch
        [146, 136],           // 048 : Dusty Bowl, Giants Seat
        [144, 140, 126],      // 049 : Stony Wilderness, Motostoke Riverbank, Watchtower Ruins
        [144, 136, 122],      // 050 : Stony Wilderness, Giants Seat, Rolling Fields
        [146, 142, 122],      // 051 : Dusty Bowl, Bridge Field, Rolling Fields
        [150],                // 052 : Hammerlocke Hills
        [146, 144],           // 053 : Dusty Bowl, Stony Wilderness
        [152, 146, 144],      // 054 : Giants Cap, Dusty Bowl, Stony Wilderness
        [154, 140],           // 055 : Lake of Outrage, Motostoke Riverbank
        [150],                // 056 : Hammerlocke Hills
        [124],                // 057 : Dappled Grove
        [144, 142, 124],      // 058 : Stony Wilderness, Bridge Field, Dappled Grove
        [152, 140, 138],      // 059 : Giants Cap, Motostoke Riverbank, North Lake Miloch
        [150, 128],           // 060 : Hammerlocke Hills, East Lake Axewell
        [150, 122],           // 061 : Hammerlocke Hills, Rolling Fields
        [144, 142, 130],      // 062 : Stony Wilderness, Bridge Field, West Lake Axewell
        [132, 122],           // 063 : Axew's Eye, Rolling Fields
        [142, 140, 128, 122], // 064 : Bridge Field, Motostoke Riverbank, East Lake Axewell, Rolling Fields 
        [144],                // 065 : Stony Wilderness
        [148],                // 066 : Giants Mirror
        [150],                // 067 : Hammerlocke Hills
        [148],                // 068 : Giants Mirror
        [148],                // 069 : Giants Mirror
        [152],                // 070 : Giants Cap
        [148],                // 071 : Giants Mirror
        [150],                // 072 : Hammerlocke Hills
        [154],                // 073 : Lake of Outrage
        [146, 130],           // 074 : Dusty Bowl, West Lake Axewell
        [138, 134],           // 075 : North Lake Miloch, South Lake Miloch
        [154],                // 076 : Lake of Outrage
        [152],                // 077 : Giants Cap
        [124],                // 078 : Dappled Grove
        [144],                // 079 : Stony Wilderness
        [144],                // 080 : Stony Wilderness
        [142],                // 081 : Bridge Field
        [136],                // 082 : Giants Seat
        [136],                // 083 : Giants Seat
        [144],                // 084 : Stony Wilderness
        [128],                // 085 : East Lake Axewell
        [142],                // 086 : Bridge Field
        [146],                // 087 : Dusty Bowl
        [152],                // 088 : Giants Cap
        [122],                // 089 : Rolling Fields
        [130, 134],           // 090 : West Lake Axewell, South Lake Miloch
        [142, 124],           // 091 : Bridge Field, Dappled Grove
        [146],                // 092 : Dusty Bowl
        [],                   // 093 : None
        [],                   // 094 : None
        [],                   // 095 : None
        [],                   // 096 : None
        [],                   // 097 : None
        [164, 166, 188, 190], // 098 : Fields of Honor, Soothing Wetlands, Stepping-Stone Sea, Insular Sea
        [164, 166, 188, 190], // 099 : Fields of Honor, Soothing Wetlands, Stepping-Stone Sea, Insular Sea
        [166, 176, 180],      // 100 : Soothing Wetlands, Courageous Cavern, Training Lowlands
        [166, 176, 180],      // 101 : Soothing Wetlands, Courageous Cavern, Training Lowlands
        [170, 176, 184, 188], // 102 : Challenge Beach, Courageous Cavern, Potbottom Desert, Stepping-Stone Sea
        [170, 176, 188],      // 103 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
        [164, 168, 170, 192], // 104 : Fields of Honor, Forest of Focus, Challenge Beach, Honeycalm Sea
        [164, 168, 170, 192], // 105 : Fields of Honor, Forest of Focus, Challenge Beach, Honeycalm Sea
        [174, 178, 186, 188], // 106 : Challenge Road, Loop Lagoon, Workout Sea, Stepping-Stone Sea
        [174, 178, 186, 188], // 107 : Challenge Road, Loop Lagoon, Workout Sea, Stepping-Stone Sea
        [164, 168, 180, 186], // 108 : Fields of Honor, Forest of Focus, Training Lowlands, Workout Sea
        [164, 168, 186],      // 109 : Fields of Honor, Forest of Focus, Workout Sea
        [164, 166, 180, 190], // 110 : Fields of Honor, Soothing Wetlands, Training Lowlands, Insular Sea
        [164, 166, 180, 190], // 111 : Fields of Honor, Soothing Wetlands, Training Lowlands, Insular Sea
        [164, 170, 178, 184], // 112 : Fields of Honor, Challenge Beach, Loop Lagoon, Potbottom Desert
        [164, 170, 178, 184], // 113 : Fields of Honor, Challenge Beach, Loop Lagoon, Potbottom Desert
        [164, 180, 184],      // 114 : Fields of Honor, Training Lowlands, Potbottom Desert
        [164, 180, 184],      // 115 : Fields of Honor, Training Lowlands, Potbottom Desert
        [166, 170, 180, 188], // 116 : Soothing Wetlands, Challenge Beach, Training Lowlands, Stepping-Stone Sea
        [166, 170, 180, 188], // 117 : Soothing Wetlands, Challenge Beach, Training Lowlands, Stepping-Stone Sea
        [166, 168, 180, 188], // 118 : Soothing Wetlands, Forest of Focus, Training Lowlands, Stepping-Stone Sea
        [166, 168, 180, 188], // 119 : Soothing Wetlands, Forest of Focus, Training Lowlands, Stepping-Stone Sea
        [166, 174, 186, 192], // 120 : Soothing Wetlands, Challenge Road, Workout Sea, Honeycalm Sea
        [166, 174, 186, 192], // 121 : Soothing Wetlands, Challenge Road, Workout Sea, Honeycalm Sea
        [164, 170, 174, 192], // 122 : Fields of Honor, Challenge Beach, Challenge Road, Honeycalm Sea
        [164, 170, 174, 192], // 123 : Fields of Honor, Challenge Beach, Challenge Road, Honeycalm Sea
        [164, 166, 168, 190], // 124 : Fields of Honor, Soothing Wetlands, Forest of Focus, Insular Sea
        [164, 166, 168, 190], // 125 : Fields of Honor, Soothing Wetlands, Forest of Focus, Insular Sea
        [170, 176, 188],      // 126 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
        [170, 176, 188],      // 127 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
        [172, 176, 188],      // 128 : Brawlers' Cave, Courageous Cavern, Stepping-Stone Sea
        [172, 176, 188],      // 129 : Brawlers' Cave, Courageous Cavern, Stepping-Stone Sea
        [178, 186, 192],      // 130 : Loop Lagoon, Workout Sea, Honeycalm Sea
        [186, 192],           // 131 : Workout Sea, Honeycalm Sea
        [164, 166, 176],      // 132 : Fields of Honor, Soothing Wetlands, Courageous Cavern
        [164, 166, 176],      // 133 : Fields of Honor, Soothing Wetlands, Courageous Cavern
        [166, 168, 170, 176], // 134 : Soothing Wetlands, Forest of Focus, Challenge Beach, Courageous Cavern
        [166, 168, 170],      // 135 : Soothing Wetlands, Forest of Focus, Challenge Beach
        [164, 170, 178, 190], // 136 : Fields of Honor, Challenge Beach, Loop Lagoon, Insular Sea
        [164, 170, 178],      // 137 : Fields of Honor, Challenge Beach, Loop Lagoon
        [186, 188, 190, 192], // 138 : Workout Sea, Stepping-Stone Sea, Insular Sea, Honeycalm Sea
        [186, 188, 190, 192], // 139 : Workout Sea, Stepping-Stone Sea, Insular Sea, Honeycalm Sea
        [],                   // 140 : None
        [],                   // 141 : None
        [194],                // 142 : Honeycalm Island
        [194],                // 143 : Honeycalm Island
        [168, 180],           // 144 : Forest of Focus, Training Lowlands
        [186, 188],           // 145 : Workout Sea, Stepping-Stone Sea
        [190],                // 146 : Insular Sea
        [176],                // 147 : Courageous Cavern
        [180],                // 148 : Training Lowlands
        [184],                // 149 : Potbottom Desert
        [178],                // 150 : Loop Lagoon
        [186],                // 151 : Workout Sea
        [186],                // 152 : Workout Sea
        [168, 180],           // 153 : Forest of Focus, Training Lowlands
        [186, 188],           // 154 : Workout Sea, Stepping-Stone Sea
        [174],                // 155 : Challenge Road
        [174],                // 156 : Challenge Road

        [204,210,222,230],    // 157 : Slippery Slope, Giant's Bed, Giant's Foot, Ballimere Lake
        [204,210,222,230],    // 158 : Slippery Slope, Giant's Bed, Giant's Foot, Ballimere Lake
        [210,214,222,230],    // 159 : Giant's Bed, Snowslide Slope, Giant's Foot, Ballimere Lake
        [210,214,222,230],    // 160 : Giant's Bed, Snowslide Slope, Giant's Foot, Ballimere Lake
        [210,222,226,230],    // 161 : Giant's Bed, Giant's Foot, Frigid Sea, Ballimere Lake
        [210,222,226,230],    // 162 : Giant's Bed, Giant's Foot, Frigid Sea, Ballimere Lake
        [208,210,226,228,230],// 163 : Frostpoint Field, Giant's Bed, Frigid Sea, Three-Point Pass, Ballimere Lake
        [208,210,226,228,230],// 164 : Frostpoint Field, Giant's Bed, Frigid Sea, Three-Point Pass, Ballimere Lake
        [204,210,220,222,230],// 165 : Slippery Slope, Giant's Bed, Crown Shrine, Giant's Foot, Ballimere Lake
        [204,210,220,222,230],// 166 : Slippery Slope, Giant's Bed, Crown Shrine, Giant's Foot, Ballimere Lake
        [204,214,226],        // 167 : Slippery Slope, Snowslide Slope, Frigid Sea
        [204,214,226],        // 168 : Slippery Slope, Snowslide Slope, Frigid Sea
        [210,226],            // 169 : Giant's Bed, Frigid Sea
        [210,226],            // 170 : Giant's Bed, Frigid Sea
        [208,210,214,226,230],// 171 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        [208,210,214,226,230],// 172 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        [210,226,230],        // 173 : Giant's Bed, Frigid Sea, Ballimere Lake
        [210,226,230],        // 174 : Giant's Bed, Frigid Sea, Ballimere Lake
        [208,210,226,230,234],// 175 : Frostpoint Field, Giant's Bed, Frigid Sea, Ballimere Lake, Dyna Tree Hill
        [208,210,226,230,234],// 176 : Frostpoint Field, Giant's Bed, Frigid Sea, Ballimere Lake, Dyna Tree Hill
        [210,214,218,230],    // 177 : Giant's Bed, Snowslide Slope, Path to the Peak, Ballimere Lake
        [210,214,218,230],    // 178 : Giant's Bed, Snowslide Slope, Path to the Peak, Ballimere Lake
        [204,210,214,230],    // 179 : Slippery Slope, Giant's Bed, Snowslide Slope, Ballimere Lake
        [204,210,214,230],    // 180 : Slippery Slope, Giant's Bed, Snowslide Slope, Ballimere Lake
        [204,212,222,226,230],// 181 : Slippery Slope, Old Cemetery, Giant's Foot, Frigid Sea, Ballimere Lake
        [204,212,222,226,230],// 182 : Slippery Slope, Old Cemetery, Giant's Foot, Frigid Sea, Ballimere Lake
        [210,218,226,228,230],// 183 : Giant's Bed, Path to the Peak, Frigid Sea, Three-Point Pass, Ballimere Lake
        [210,218,226,228,230],// 184 : Giant's Bed, Path to the Peak, Frigid Sea, Three-Point Pass, Ballimere Lake
        [208,210,214,222,226],// 185 : Frostpoint Field, Giant's Bed, Snowslide Slope, Giant's Foot, Frigid Sea
        [208,210,214,222,226],// 186 : Frostpoint Field, Giant's Bed, Snowslide Slope, Giant's Foot, Frigid Sea
        [210,214,218,226],    // 187 : Giant's Bed, Snowslide Slope, Path to the Peak, Frigid Sea
        [210,214,218,226],    // 188 : Giant's Bed, Snowslide Slope, Path to the Peak, Frigid Sea
        [208,210,214,226,230],// 189 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        [208,210,214,226,230],// 190 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        [210,212,230],        // 191 : Giant's Bed, Old Cemetery, Ballimere Lake
        [210,212,230],        // 192 : Giant's Bed, Old Cemetery, Ballimere Lake
        [230],                // 193 : Ballimere Lake
        [230],                // 194 : Ballimere Lake
        [214],                // 195 : Snowslide Slope
        [214],                // 196 : Snowslide Slope 
    ];

    /// <summary>
    /// Location IDs containing Dens that cannot be accessed without Rotom Bike's Water Mode.
    /// </summary>
    internal static ReadOnlySpan<byte> InaccessibleRank12DistributionLocations => [154,178,186,188,190,192,194,226,228,230,234]; // Areas that are entirely restricted to water

    /// <summary>
    /// Location IDs containing Dens that cannot be accessed without Rotom Bike's Water Mode.
    /// </summary>
    internal static bool IsInaccessibleRank12Nest(byte nestID, byte location)
    {
        var noNest = GetInaccessibleRank12Nests(location);
        return noNest.Length > 0 && noNest.Contains(nestID);
    }

    private static ReadOnlySpan<byte> GetInaccessibleRank12Nests(byte location) => location switch
    {
        128 => [6,43], // East Lake Axewell
        130 => [6,41,43], // West Lake Axewell
        132 => [37,63], // Axew's Eye
        134 => [7,40,75,90], // South Lake Miloch
        138 => [40,75], // North Lake Miloch
        142 => [7,43], // Bridge Field
        146 => [8,74], // Dusty Bowl
        148 => [41,66], // Giant's Mirror
        154 => [7,17,24,33,43,55,73,76], // Lake of Outrage
        164 => [136,137], // Fields of Honor
        168 => [124,125,134,135,144,153], // Forest of Focus
        170 => [126,127], // Challenge Beach
        176 => [132,133], // Courageous Cavern
        178 => [106,107,112,113,130,136,137,150], // Loop Lagoon
        180 => [116,117], // Training Lowlands
        186 => [106,107,108,109,120,121,130,131,138,139,145,151,152,154], // Workout Sea
        188 => [98,99,102,103,106,107,116,117,118,119,126,127,128,129,138,139,145,154], // Stepping-Stone Sea
        190 => [98,99,110,111,124,125,136,138,139,146], // Insular Sea
        192 => [104,105,120,121,122,123,130,131,138,139], // Honeycalm Sea
        194 => [142,143], // Honeycalm Island
        210 => [169,170,183,184], // Giant's Bed
        222 => [181,182,185,186], // Giant's Foot
        226 => [161,162,163,164,167,168,169,170,171,172,173,174,175,176,181,182,183,184,185,186,187,188,189,190], // Frigid Sea
        228 => [163,164,183,184], // Three-Point Pass
        230 => [157,158,159,160,161,162,163,164,165,166,171,172,173,174,175,176,177,178,179,180,181,182,183,184,189,190,191,192,193,194], // Ballimere Lake
        234 => [175,176], // Dyna Tree Hill

        162 => [6,7,37,40,41,43,66,73,75,76,130,131,138,139,142,143,145,146,150,151,152,154,169,170,193,194], // Completely inaccessible

        _ => [],
    };

    // Abilities Allowed
    private const AbilityPermission A0 = AbilityPermission.OnlyFirst;
  //private const AbilityPermission A1 = AbilityPermission.OnlySecond;
    private const AbilityPermission A2 = AbilityPermission.OnlyHidden;
    private const AbilityPermission A3 = AbilityPermission.Any12;
  //private const AbilityPermission A4 = AbilityPermission.Any12H;

    internal const int SharedNest = 162;
    internal const int Watchtower = 126;
    internal const int MaxLair = 244;

    internal static readonly EncounterStatic8N[] Nest_SW = GetBase("sw", SW);
    internal static readonly EncounterStatic8N[] Nest_SH = GetBase("sh", SH);

    internal static readonly EncounterStatic8ND[] Dist_SW = GetDist("sw", SW);
    internal static readonly EncounterStatic8ND[] Dist_SH = GetDist("sh", SH);

    internal static readonly EncounterStatic8U[] DynAdv_SWSH = GetUnderground();

    internal static readonly EncounterStatic8NC[] Crystal_SWSH =
    [
        new(SWSH) { Species = 782, Level = 16, Ability = A3, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 2, Moves = new(033,029,525,043) }, // ★And458 Jangmo-o
        new(SWSH) { Species = 246, Level = 16, Ability = A3, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 2, Moves = new(033,157,371,044) }, // ★And15 Larvitar
        new(SWSH) { Species = 823, Level = 50, Ability = A2, IVs = new(31,31,31,-1,-1,31), DynamaxLevel = 5, Moves = new(065,442,034,796), CanGigantamax = true }, // ★And337 Gigantamax Corviknight
        new(SWSH) { Species = 875, Level = 15, Ability = A3, IVs = new(31,31,-1,31,-1,-1), DynamaxLevel = 2, Moves = new(181,311,054,556) }, // ★And603 Eiscue
        new(SWSH) { Species = 874, Level = 15, Ability = A3, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 2, Moves = new(397,317,335,157) }, // ★And390 Stonjourner
        new(SWSH) { Species = 879, Level = 35, Ability = A3, IVs = new(31,31,-1, 0,31,-1), DynamaxLevel = 4, Moves = new(484,174,776,583), CanGigantamax = true }, // ★Sgr6879 Gigantamax Copperajah
        new(SWSH) { Species = 851, Level = 35, Ability = A2, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 5, Moves = new(680,679,489,438), CanGigantamax = true }, // ★Sgr6859 Gigantamax Centiskorch
        new(SW  ) { Species = 842, Level = 40, Ability = A0, IVs = new(31,-1,31,-1,31,-1), DynamaxLevel = 5, Moves = new(787,412,406,076), CanGigantamax = true }, // ★Sgr6913 Gigantamax Appletun
        new(  SH) { Species = 841, Level = 40, Ability = A0, IVs = new(31,31,-1,31,-1,-1), DynamaxLevel = 5, Moves = new(788,491,412,406), CanGigantamax = true }, // ★Sgr6913 Gigantamax Flapple
        new(SWSH) { Species = 844, Level = 40, Ability = A0, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 5, Moves = new(523,776,489,157), CanGigantamax = true }, // ★Sgr7348 Gigantamax Sandaconda
        new(SWSH) { Species = 884, Level = 40, Ability = A2, IVs = new(31,-1,-1,31,31,-1), DynamaxLevel = 5, Moves = new(796,063,784,319), CanGigantamax = true }, // ★Sgr7121 Gigantamax Duraludon
        new(SWSH) { Species = 025, Level = 25, Ability = A2, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 5, Moves = new(606,273,104,085), CanGigantamax = true }, // ★Sgr6746 Gigantamax Pikachu
        new(SWSH) { Species = 133, Level = 25, Ability = A2, IVs = new(31,31,31,-1,-1,-1), DynamaxLevel = 5, Moves = new(606,273,038,129), CanGigantamax = true }, // ★Sgr7194 Gigantamax Eevee
    ];

    private static EncounterStatic8N[] GetBase([Length(2, 2)] string name, [ConstantExpected] GameVersion game)
    {
        var data = EncounterUtil.Get($"{name}_nest");
        const int size = 10;
        var result = new EncounterStatic8N[data.Length / size];

        for (int i = 0; i < result.Length; i++)
        {
            var slice = data.Slice(i * size, size);
            result[i] = EncounterStatic8N.Read(slice, game);
        }

        return result;
    }

    private static EncounterStatic8ND[] GetDist([Length(2, 2)] string name, [ConstantExpected] GameVersion game)
    {
        var data = EncounterUtil.Get($"{name}_dist");
        const int size = 0x10;
        var result = new EncounterStatic8ND[data.Length / size];

        for (int i = 0; i < result.Length; i++)
        {
            var slice = data.Slice(i * size, size);
            result[i] = EncounterStatic8ND.Read(slice, game);
        }

        return result;
    }

    // These are encountered as never-shiny, but forced shiny (Star) if the 1:300 (1:100 w/charm) post-adventure roll activates.
    // The game does try to gate specific entries to Sword / Shield, but this restriction is ignored for online battles.
    // All captures share the same met location, so there is no way to distinguish an online-play result from a local-play result.
    private static EncounterStatic8U[] GetUnderground()
    {
        var data = EncounterUtil.Get("swsh_underground");
        const int size = 14;
        var result = new EncounterStatic8U[data.Length / size];

        for (int i = 0; i < result.Length; i++)
        {
            var slice = data.Slice(i * size, size);
            result[i] = EncounterStatic8U.Read(slice);
        }

        return result;
    }
}
