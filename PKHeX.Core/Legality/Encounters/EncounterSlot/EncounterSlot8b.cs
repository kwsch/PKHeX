using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.BDSP"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot8b : EncounterSlot
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8b;
    public bool IsUnderground => Area.Location is (>= 508 and <= 617);
    public bool IsMarsh => Area.Location is (>= 219 and <= 224);
    public readonly bool IsBCAT;
    public override Ball FixedBall => IsMarsh ? Ball.Safari : Ball.None;

    public EncounterSlot8b(EncounterArea area, ushort species, byte form, byte min, byte max, bool isBCAT = false) : base(area, species, form, min, max)
    {
        IsBCAT = isBCAT;
    }

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        return base.GetMatchRating(pk);
    }

    protected override void SetFormatSpecificData(PKM pk)
    {
        if (IsUnderground)
        {
            if (GetBaseEggMove(out var move1))
                pk.RelearnMove1 = move1;
        }
        pk.SetRandomEC();
    }

    public bool CanBeUndergroundMove(ushort move)
    {
        var et = PersonalTable.BDSP;
        var sf = et.GetFormEntry(Species, Form);
        var species = sf.HatchSpecies;
        if (IsBCAT && IgnoreEggMoves.TryGetValue(species, out var exclude) && Array.IndexOf(exclude, move) != -1)
            return false;

        var baseEgg = Legal.EggMovesBDSP[species].Moves;
        if (baseEgg.Length == 0)
            return move == 0;
        return Array.IndexOf(baseEgg, move) >= 0;
    }

    public bool GetBaseEggMove(out ushort move)
    {
        var et = PersonalTable.BDSP;
        var sf = et.GetFormEntry(Species, Form);
        var species = sf.HatchSpecies;

        int[] Exclude = IgnoreEggMoves.TryGetValue(species, out var exclude) ? exclude : Array.Empty<int>();
        var baseEgg = Legal.EggMovesBDSP[species].Moves;
        if (baseEgg.Length == 0)
        {
            move = 0;
            return false;
        }

        // Official method creates a new List<ushort>() with all the egg moves, removes all ignored, then picks a random index.
        // We'll just loop instead to not allocate, and because it's a >50% chance the move won't be ignored, thus faster.
        var rnd = Util.Rand;
        while (true)
        {
            var index = rnd.Next(baseEgg.Length);
            move = baseEgg[index];
            if (!IsBCAT || Array.IndexOf(Exclude, move) == -1)
                return true;
        }
    }

    public bool CanUseRadar => Area.Type is SlotType.Grass && !IsUnderground && !IsMarsh && Location switch
    {
        195 or 196 => false, // Oreburgh Mine
        203 or 204 or 205 or 208 or 209 or 210 or 211 or 212 or 213 or 214 or 215 => false, // Mount Coronet, 206/207 exterior
        >= 225 and <= 243 => false, // Solaceon Ruins
        244 or 245 or 246 or 247 or 248 or 249 => false, // Victory Road
        252 => false, // Ravaged Path
        255 or 256 => false, // Oreburgh Gate
        260 or 261 or 262 => false, // Stark Mountain, 259 exterior
        >= 264 and <= 284 => false, // Turnback Cave
        286 or 287 or 288 or 289 or 290 or 291 => false, // Snowpoint Temple
        292 or 293 => false, // Wayward Cave
        294 or 295 => false, // Ruin Maniac Cave
        296 => false, // Maniac Tunnel
        299 or 300 or 301 or 302 or 303 or 304 or 305 => false, // Iron Island, 298 exterior
        306 or 307 or 308 or 309 or 310 or 311 or 312 or 313 or 314 => false, // Old Chateau
        368 or 369 or 370 or 371 or 372 => false, // Route 209 (Lost Tower)
        _ => true,
    };

    protected override HiddenAbilityPermission IsHiddenAbilitySlot() => CanUseRadar ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

    /// <summary>
    /// Unreferenced in v1.0, so all egg moves are possible for ROM encounters.
    /// Since the Underground supports BCAT distributions, we will keep this around on the off chance they do utilize that method of distribution.
    /// </summary>
    private static readonly Dictionary<int, int[]> IgnoreEggMoves = new()
    {
        {004, new[] {394}}, // Charmander
        {016, new[] {403}}, // Pidgey
        {019, new[] {044}}, // Rattata
        {027, new[] {229}}, // Sandshrew
        {037, new[] {180,050,326}}, // Vulpix
        {050, new[] {310}}, // Diglett
        {056, new[] {370}}, // Mankey
        {058, new[] {242,336,394}}, // Growlithe
        {060, new[] {061,341}}, // Poliwag
        {066, new[] {282}}, // Machop
        {077, new[] {172}}, // Ponyta
        {079, new[] {428}}, // Slowpoke
        {083, new[] {348}}, // Farfetch’d
        {084, new[] {098,283}}, // Doduo
        {086, new[] {227}}, // Seel
        {098, new[] {175,021}}, // Krabby
        {102, new[] {235}}, // Exeggcute
        {108, new[] {187}}, // Lickitung
        {109, new[] {194}}, // Koffing
        {113, new[] {270}}, // Chansey
        {114, new[] {072}}, // Tangela
        {115, new[] {023,116}}, // Kangaskhan
        {116, new[] {225}}, // Horsea
        {122, new[] {102,298}}, // Mr. Mime
        {127, new[] {450,276}}, // Pinsir
        {133, new[] {204,343}}, // Eevee
        {140, new[] {341}}, // Kabuto
        {143, new[] {122,562}}, // Snorlax
        {147, new[] {349,407}}, // Dratini
        {152, new[] {267,312,034}}, // Chikorita
        {155, new[] {098,038}}, // Cyndaquil
        {158, new[] {242,037,056}}, // Totodile
        {161, new[] {179}}, // Sentret
        {170, new[] {175}}, // Chinchou
        {173, new[] {150}}, // Cleffa
        {179, new[] {036,268}}, // Mareep
        {183, new[] {276}}, // Marill
        {187, new[] {388}}, // Hoppip
        {190, new[] {103,097}}, // Aipom
        {191, new[] {073,275}}, // Sunkern
        {198, new[] {017,372}}, // Murkrow
        {200, new[] {180}}, // Misdreavus
        {204, new[] {038}}, // Pineco
        {206, new[] {246}}, // Dunsparce
        {209, new[] {242,423,424,422}}, // Snubbull
        {214, new[] {224}}, // Heracross
        {216, new[] {313}}, // Teddiursa
        {218, new[] {414}}, // Slugma
        {220, new[] {036}}, // Swinub
        {222, new[] {392}}, // Corsola
        {223, new[] {062}}, // Remoraid
        {226, new[] {056,469}}, // Mantine
        {227, new[] {065,413}}, // Skarmory
        {228, new[] {251,424}}, // Houndour
        {234, new[] {428}}, // Stantler
        {236, new[] {270}}, // Tyrogue
        {238, new[] {008}}, // Smoochum
        {252, new[] {283,437}}, // Treecko
        {255, new[] {179,297}}, // Torchic
        {261, new[] {281,389,583}}, // Poochyena
        {270, new[] {175,055}}, // Lotad
        {276, new[] {413}}, // Taillow
        {278, new[] {054,097}}, // Wingull
        {283, new[] {453}}, // Surskit
        {285, new[] {388,402}}, // Shroomish
        {296, new[] {197}}, // Makuhita
        {298, new[] {021}}, // Azurill
        {299, new[] {335}}, // Nosepass
        {300, new[] {252}}, // Skitty
        {302, new[] {212}}, // Sableye
        {303, new[] {389}}, // Mawile
        {304, new[] {442}}, // Aron
        {309, new[] {435,422}}, // Electrike
        {311, new[] {435,204}}, // Plusle
        {312, new[] {435,313}}, // Minun
        {313, new[] {227}}, // Volbeat
        {314, new[] {227}}, // Illumise
        {315, new[] {235}}, // Roselia
        {316, new[] {220,441}}, // Gulpin
        {320, new[] {034}}, // Wailmer
        {322, new[] {281}}, // Numel
        {324, new[] {284,499}}, // Torkoal
        {325, new[] {428}}, // Spoink
        {328, new[] {414}}, // Trapinch
        {336, new[] {400}}, // Seviper
        {339, new[] {330}}, // Barboach
        {341, new[] {283,282}}, // Corphish
        {345, new[] {072}}, // Lileep
        {352, new[] {050,492}}, // Kecleon
        {353, new[] {425,566}}, // Shuppet
        {357, new[] {437,235,349,692}}, // Tropius
        {359, new[] {389,195}}, // Absol
        {363, new[] {205}}, // Spheal
        {369, new[] {401}}, // Relicanth
        {370, new[] {392}}, // Luvdisc
        {387, new[] {074}}, // Turtwig
        {390, new[] {612}}, // Chimchar
        {393, new[] {056}}, // Piplup
        {399, new[] {111,205}}, // Bidoof
        {408, new[] {043}}, // Cranidos
        {417, new[] {608}}, // Pachirisu
        {418, new[] {401}}, // Buizel
        {422, new[] {262}}, // Shellos
        {425, new[] {194,366}}, // Drifloon
        {439, new[] {102,298}}, // Mime Jr.
        {442, new[] {425}}, // Spiritomb
        {443, new[] {225,328}}, // Gible
        {446, new[] {122}}, // Munchlax
        {449, new[] {303,328}}, // Hippopotas
        {451, new[] {400}}, // Skorupi
        {456, new[] {175}}, // Finneon
        {458, new[] {056,469}}, // Mantyke
        {459, new[] {054}}, // Snover
    };
}
