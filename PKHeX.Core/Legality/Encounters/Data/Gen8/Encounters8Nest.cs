using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

internal static class Encounters8Nest
{
    internal static ReadOnlySpan<byte> GetNestLocations(byte loc) => loc switch
    {
        000 => [144, 134, 122],      // 000 : Stony Wilderness, South Lake Miloch, Rolling Fields
        001 => [144, 126],           // 001 : Stony Wilderness, Watchtower Ruins
        002 => [144, 122],           // 002 : Stony Wilderness, Rolling Fields
        003 => [142, 124, 122],      // 003 : Bridge Field, Dappled Grove, Rolling Fields
        004 => [142, 134],           // 004 : Bridge Field, South Lake Miloch
        005 => [144, 126],           // 005 : Stony Wilderness, Watchtower Ruins
        006 => [128, 130],           // 006 : East Lake Axewell, West Lake Axewell
        007 => [154, 142, 134],      // 007 : Lake of Outrage, Bridge Field, South Lake Miloch
        008 => [146, 130],           // 008 : Dusty Bowl, West Lake Axewell
        009 => [146, 138],           // 009 : Dusty Bowl, North Lake Miloch
        010 => [146, 136],           // 010 : Dusty Bowl, Giants Seat
        011 => [150, 144, 136],      // 011 : Hammerlocke Hills, Stony Wilderness, Giants Seat
        012 => [142],                // 012 : Bridge Field
        013 => [150, 144, 140],      // 013 : Hammerlocke Hills, Stony Wilderness, Motostoke Riverbank
        014 => [146, 136],           // 014 : Dusty Bowl, Giants Seat
        015 => [142, 122],           // 015 : Bridge Field, Rolling Fields
        016 => [146],                // 016 : Dusty Bowl
        017 => [154, 152, 144],      // 017 : Lake of Outrage, Giants Cap, Stony Wilderness
        018 => [150, 144],           // 018 : Hammerlocke Hills, Stony Wilderness
        019 => [146],                // 019 : Dusty Bowl
        020 => [146],                // 020 : Dusty Bowl
        021 => [144],                // 021 : Stony Wilderness
        022 => [150, 152],           // 022 : Hammerlocke Hills, Giants Cap
        023 => [152, 140],           // 023 : Giants Cap, Motostoke Riverbank
        024 => [154, 148],           // 024 : Lake of Outrage, Giants Mirror
        025 => [124],                // 025 : Dappled Grove
        026 => [148, 144, 142],      // 026 : Giants Mirror, Stony Wilderness, Bridge Field
        027 => [148, 124, 146],      // 027 : Giants Mirror, Dappled Grove AND Dusty Bowl (Giant's Mirror load-line overlap)
        028 => [138, 128],           // 028 : North Lake Miloch, East Lake Axewell
        029 => [150, 152, 140],      // 029 : Hammerlocke Hills, Giants Cap, Motostoke Riverbank
        030 => [128, 122],           // 030 : East Lake Axewell, Rolling Fields
        031 => [150, 152],           // 031 : Hammerlocke Hills, Giants Cap
        032 => [150, 122],           // 032 : Hammerlocke Hills, Rolling Fields
        033 => [154, 142],           // 033 : Lake of Outrage, Bridge Field
        034 => [144, 130],           // 034 : Stony Wilderness, West Lake Axewell
        035 => [142, 146, 148],      // 035 : Bridge Field, Dusty Bowl, Giants Mirror
        036 => [122],                // 036 : Rolling Fields
        037 => [132],                // 037 : Axew's Eye
        038 => [128, 122],           // 038 : East Lake Axewell, Rolling Fields
        039 => [144, 142, 140],      // 039 : Stony Wilderness, Bridge Field, Motostoke Riverbank
        040 => [134, 138],           // 040 : South Lake Miloch, North Lake Miloch
        041 => [148, 130],           // 041 : Giants Mirror, West Lake Axewell
        042 => [148, 144, 134, 146], // 042 : Giants Mirror, Stony Wilderness, South Lake Miloch AND Dusty Bowl (Giant's Mirror load-line overlap)
        043 => [154, 142, 128, 130], // 043 : Lake of Outrage, Bridge Field, East Lake Axewell, West Lake Axewell 
        044 => [150, 136],           // 044 : Hammerlocke Hills, Giants Seat
        045 => [142, 134, 122],      // 045 : Bridge Field, South Lake Miloch, Rolling Fields
        046 => [126],                // 046 : Watchtower Ruins
        047 => [146, 138, 122, 134], // 047 : Dusty Bowl, North Lake Miloch, Rolling Fields, South Lake Miloch
        048 => [146, 136],           // 048 : Dusty Bowl, Giants Seat
        049 => [144, 140, 126],      // 049 : Stony Wilderness, Motostoke Riverbank, Watchtower Ruins
        050 => [144, 136, 122],      // 050 : Stony Wilderness, Giants Seat, Rolling Fields
        051 => [146, 142, 122],      // 051 : Dusty Bowl, Bridge Field, Rolling Fields
        052 => [150],                // 052 : Hammerlocke Hills
        053 => [146, 144],           // 053 : Dusty Bowl, Stony Wilderness
        054 => [152, 146, 144],      // 054 : Giants Cap, Dusty Bowl, Stony Wilderness
        055 => [154, 140],           // 055 : Lake of Outrage, Motostoke Riverbank
        056 => [150],                // 056 : Hammerlocke Hills
        057 => [124],                // 057 : Dappled Grove
        058 => [144, 142, 124],      // 058 : Stony Wilderness, Bridge Field, Dappled Grove
        059 => [152, 140, 138],      // 059 : Giants Cap, Motostoke Riverbank, North Lake Miloch
        060 => [150, 128],           // 060 : Hammerlocke Hills, East Lake Axewell
        061 => [150, 122],           // 061 : Hammerlocke Hills, Rolling Fields
        062 => [144, 142, 130],      // 062 : Stony Wilderness, Bridge Field, West Lake Axewell
        063 => [132, 122],           // 063 : Axew's Eye, Rolling Fields
        064 => [142, 140, 128, 122], // 064 : Bridge Field, Motostoke Riverbank, East Lake Axewell, Rolling Fields 
        065 => [144],                // 065 : Stony Wilderness
        066 => [148],                // 066 : Giants Mirror
        067 => [150],                // 067 : Hammerlocke Hills
        068 => [148],                // 068 : Giants Mirror
        069 => [148],                // 069 : Giants Mirror
        070 => [152],                // 070 : Giants Cap
        071 => [148],                // 071 : Giants Mirror
        072 => [150],                // 072 : Hammerlocke Hills
        073 => [154],                // 073 : Lake of Outrage
        074 => [146, 130],           // 074 : Dusty Bowl, West Lake Axewell
        075 => [138, 134],           // 075 : North Lake Miloch, South Lake Miloch
        076 => [154],                // 076 : Lake of Outrage
        077 => [152],                // 077 : Giants Cap
        078 => [124],                // 078 : Dappled Grove
        079 => [144],                // 079 : Stony Wilderness
        080 => [144],                // 080 : Stony Wilderness
        081 => [142],                // 081 : Bridge Field
        082 => [136],                // 082 : Giants Seat
        083 => [136],                // 083 : Giants Seat
        084 => [144],                // 084 : Stony Wilderness
        085 => [128],                // 085 : East Lake Axewell
        086 => [142],                // 086 : Bridge Field
        087 => [146],                // 087 : Dusty Bowl
        088 => [152],                // 088 : Giants Cap
        089 => [122],                // 089 : Rolling Fields
        090 => [130, 134],           // 090 : West Lake Axewell, South Lake Miloch
        091 => [142, 124],           // 091 : Bridge Field, Dappled Grove
        092 => [146],                // 092 : Dusty Bowl
        093 => [],                   // 093 : None
        094 => [],                   // 094 : None
        095 => [],                   // 095 : None
        096 => [],                   // 096 : None
        097 => [],                   // 097 : None
        098 => [164, 166, 188, 190], // 098 : Fields of Honor, Soothing Wetlands, Stepping-Stone Sea, Insular Sea
        099 => [164, 166, 188, 190], // 099 : Fields of Honor, Soothing Wetlands, Stepping-Stone Sea, Insular Sea
        100 => [166, 176, 180],      // 100 : Soothing Wetlands, Courageous Cavern, Training Lowlands
        101 => [166, 176, 180],      // 101 : Soothing Wetlands, Courageous Cavern, Training Lowlands
        102 => [170, 176, 184, 188], // 102 : Challenge Beach, Courageous Cavern, Potbottom Desert, Stepping-Stone Sea
        103 => [170, 176, 188],      // 103 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
        104 => [164, 168, 170, 192], // 104 : Fields of Honor, Forest of Focus, Challenge Beach, Honeycalm Sea
        105 => [164, 168, 170, 192], // 105 : Fields of Honor, Forest of Focus, Challenge Beach, Honeycalm Sea
        106 => [174, 178, 186, 188], // 106 : Challenge Road, Loop Lagoon, Workout Sea, Stepping-Stone Sea
        107 => [174, 178, 186, 188], // 107 : Challenge Road, Loop Lagoon, Workout Sea, Stepping-Stone Sea
        108 => [164, 168, 180, 186], // 108 : Fields of Honor, Forest of Focus, Training Lowlands, Workout Sea
        109 => [164, 168, 186],      // 109 : Fields of Honor, Forest of Focus, Workout Sea
        110 => [164, 166, 180, 190], // 110 : Fields of Honor, Soothing Wetlands, Training Lowlands, Insular Sea
        111 => [164, 166, 180, 190], // 111 : Fields of Honor, Soothing Wetlands, Training Lowlands, Insular Sea
        112 => [164, 170, 178, 184], // 112 : Fields of Honor, Challenge Beach, Loop Lagoon, Potbottom Desert
        113 => [164, 170, 178, 184], // 113 : Fields of Honor, Challenge Beach, Loop Lagoon, Potbottom Desert
        114 => [164, 180, 184],      // 114 : Fields of Honor, Training Lowlands, Potbottom Desert
        115 => [164, 180, 184],      // 115 : Fields of Honor, Training Lowlands, Potbottom Desert
        116 => [166, 170, 180, 188], // 116 : Soothing Wetlands, Challenge Beach, Training Lowlands, Stepping-Stone Sea
        117 => [166, 170, 180, 188], // 117 : Soothing Wetlands, Challenge Beach, Training Lowlands, Stepping-Stone Sea
        118 => [166, 168, 180, 188], // 118 : Soothing Wetlands, Forest of Focus, Training Lowlands, Stepping-Stone Sea
        119 => [166, 168, 180, 188], // 119 : Soothing Wetlands, Forest of Focus, Training Lowlands, Stepping-Stone Sea
        120 => [166, 174, 186, 192], // 120 : Soothing Wetlands, Challenge Road, Workout Sea, Honeycalm Sea
        121 => [166, 174, 186, 192], // 121 : Soothing Wetlands, Challenge Road, Workout Sea, Honeycalm Sea
        122 => [164, 170, 174, 192], // 122 : Fields of Honor, Challenge Beach, Challenge Road, Honeycalm Sea
        123 => [164, 170, 174, 192], // 123 : Fields of Honor, Challenge Beach, Challenge Road, Honeycalm Sea
        124 => [164, 166, 168, 190], // 124 : Fields of Honor, Soothing Wetlands, Forest of Focus, Insular Sea
        125 => [164, 166, 168, 190], // 125 : Fields of Honor, Soothing Wetlands, Forest of Focus, Insular Sea
        126 => [170, 176, 188],      // 126 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
        127 => [170, 176, 188],      // 127 : Challenge Beach, Courageous Cavern, Stepping-Stone Sea
        128 => [172, 176, 188],      // 128 : Brawlers' Cave, Courageous Cavern, Stepping-Stone Sea
        129 => [172, 176, 188],      // 129 : Brawlers' Cave, Courageous Cavern, Stepping-Stone Sea
        130 => [178, 186, 192],      // 130 : Loop Lagoon, Workout Sea, Honeycalm Sea
        131 => [186, 192],           // 131 : Workout Sea, Honeycalm Sea
        132 => [164, 166, 176],      // 132 : Fields of Honor, Soothing Wetlands, Courageous Cavern
        133 => [164, 166, 176],      // 133 : Fields of Honor, Soothing Wetlands, Courageous Cavern
        134 => [166, 168, 170, 176], // 134 : Soothing Wetlands, Forest of Focus, Challenge Beach, Courageous Cavern
        135 => [166, 168, 170],      // 135 : Soothing Wetlands, Forest of Focus, Challenge Beach
        136 => [164, 170, 178, 190], // 136 : Fields of Honor, Challenge Beach, Loop Lagoon, Insular Sea
        137 => [164, 170, 178],      // 137 : Fields of Honor, Challenge Beach, Loop Lagoon
        138 => [186, 188, 190, 192], // 138 : Workout Sea, Stepping-Stone Sea, Insular Sea, Honeycalm Sea
        139 => [186, 188, 190, 192], // 139 : Workout Sea, Stepping-Stone Sea, Insular Sea, Honeycalm Sea
        140 => [],                   // 140 : None
        141 => [],                   // 141 : None
        142 => [194],                // 142 : Honeycalm Island
        143 => [194],                // 143 : Honeycalm Island
        144 => [168, 180],           // 144 : Forest of Focus, Training Lowlands
        145 => [186, 188],           // 145 : Workout Sea, Stepping-Stone Sea
        146 => [190],                // 146 : Insular Sea
        147 => [176],                // 147 : Courageous Cavern
        148 => [180],                // 148 : Training Lowlands
        149 => [184],                // 149 : Potbottom Desert
        150 => [178],                // 150 : Loop Lagoon
        151 => [186],                // 151 : Workout Sea
        152 => [186],                // 152 : Workout Sea
        153 => [168, 180],           // 153 : Forest of Focus, Training Lowlands
        154 => [186, 188],           // 154 : Workout Sea, Stepping-Stone Sea
        155 => [174],                // 155 : Challenge Road
        156 => [174],                // 156 : Challenge Road

        157 => [204,210,222,230],    // 157 : Slippery Slope, Giant's Bed, Giant's Foot, Ballimere Lake
        158 => [204,210,222,230],    // 158 : Slippery Slope, Giant's Bed, Giant's Foot, Ballimere Lake
        159 => [210,214,222,230],    // 159 : Giant's Bed, Snowslide Slope, Giant's Foot, Ballimere Lake
        160 => [210,214,222,230],    // 160 : Giant's Bed, Snowslide Slope, Giant's Foot, Ballimere Lake
        161 => [210,222,226,230],    // 161 : Giant's Bed, Giant's Foot, Frigid Sea, Ballimere Lake
        162 => [210,222,226,230],    // 162 : Giant's Bed, Giant's Foot, Frigid Sea, Ballimere Lake
        163 => [208,210,226,228,230],// 163 : Frostpoint Field, Giant's Bed, Frigid Sea, Three-Point Pass, Ballimere Lake
        164 => [208,210,226,228,230],// 164 : Frostpoint Field, Giant's Bed, Frigid Sea, Three-Point Pass, Ballimere Lake
        165 => [204,210,220,222,230],// 165 : Slippery Slope, Giant's Bed, Crown Shrine, Giant's Foot, Ballimere Lake
        166 => [204,210,220,222,230],// 166 : Slippery Slope, Giant's Bed, Crown Shrine, Giant's Foot, Ballimere Lake
        167 => [204,214,226],        // 167 : Slippery Slope, Snowslide Slope, Frigid Sea
        168 => [204,214,226],        // 168 : Slippery Slope, Snowslide Slope, Frigid Sea
        169 => [210,226],            // 169 : Giant's Bed, Frigid Sea
        170 => [210,226],            // 170 : Giant's Bed, Frigid Sea
        171 => [208,210,214,226,230],// 171 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        172 => [208,210,214,226,230],// 172 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        173 => [210,226,230],        // 173 : Giant's Bed, Frigid Sea, Ballimere Lake
        174 => [210,226,230],        // 174 : Giant's Bed, Frigid Sea, Ballimere Lake
        175 => [208,210,226,230,234],// 175 : Frostpoint Field, Giant's Bed, Frigid Sea, Ballimere Lake, Dyna Tree Hill
        176 => [208,210,226,230,234],// 176 : Frostpoint Field, Giant's Bed, Frigid Sea, Ballimere Lake, Dyna Tree Hill
        177 => [210,214,218,230],    // 177 : Giant's Bed, Snowslide Slope, Path to the Peak, Ballimere Lake
        178 => [210,214,218,230],    // 178 : Giant's Bed, Snowslide Slope, Path to the Peak, Ballimere Lake
        179 => [204,210,214,230],    // 179 : Slippery Slope, Giant's Bed, Snowslide Slope, Ballimere Lake
        180 => [204,210,214,230],    // 180 : Slippery Slope, Giant's Bed, Snowslide Slope, Ballimere Lake
        181 => [204,212,222,226,230],// 181 : Slippery Slope, Old Cemetery, Giant's Foot, Frigid Sea, Ballimere Lake
        182 => [204,212,222,226,230],// 182 : Slippery Slope, Old Cemetery, Giant's Foot, Frigid Sea, Ballimere Lake
        183 => [210,218,226,228,230],// 183 : Giant's Bed, Path to the Peak, Frigid Sea, Three-Point Pass, Ballimere Lake
        184 => [210,218,226,228,230],// 184 : Giant's Bed, Path to the Peak, Frigid Sea, Three-Point Pass, Ballimere Lake
        185 => [208,210,214,222,226],// 185 : Frostpoint Field, Giant's Bed, Snowslide Slope, Giant's Foot, Frigid Sea
        186 => [208,210,214,222,226],// 186 : Frostpoint Field, Giant's Bed, Snowslide Slope, Giant's Foot, Frigid Sea
        187 => [210,214,218,226],    // 187 : Giant's Bed, Snowslide Slope, Path to the Peak, Frigid Sea
        188 => [210,214,218,226],    // 188 : Giant's Bed, Snowslide Slope, Path to the Peak, Frigid Sea
        189 => [208,210,214,226,230],// 189 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        190 => [208,210,214,226,230],// 190 : Frostpoint Field, Giant's Bed, Snowslide Slope, Frigid Sea, Ballimere Lake
        191 => [210,212,230],        // 191 : Giant's Bed, Old Cemetery, Ballimere Lake
        192 => [210,212,230],        // 192 : Giant's Bed, Old Cemetery, Ballimere Lake
        193 => [230],                // 193 : Ballimere Lake
        194 => [230],                // 194 : Ballimere Lake
        195 => [214],                // 195 : Snowslide Slope
        196 => [214],                // 196 : Snowslide Slope

        _ => [],
    };

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
        return noNest.Length != 0 && noNest.Contains(nestID);
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

    internal static readonly EncounterStatic8N[] Nest_SW = GetBase("sw_nest", SW);
    internal static readonly EncounterStatic8N[] Nest_SH = GetBase("sh_nest", SH);

    internal static readonly EncounterStatic8ND[] Dist_SW = GetDist("sw_dist", SW);
    internal static readonly EncounterStatic8ND[] Dist_SH = GetDist("sh_dist", SH);

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

    private static EncounterStatic8N[] GetBase([Length(2, 2), ConstantExpected] string name, [ConstantExpected] GameVersion game)
    {
        var data = EncounterUtil.Get(name);
        const int size = 10;
        var result = new EncounterStatic8N[data.Length / size];

        for (int i = 0; i < result.Length; i++)
        {
            var slice = data.Slice(i * size, size);
            result[i] = EncounterStatic8N.Read(slice, game);
        }

        return result;
    }

    private static EncounterStatic8ND[] GetDist([Length(2, 2), ConstantExpected] string name, [ConstantExpected] GameVersion game)
    {
        var data = EncounterUtil.Get(name);
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
