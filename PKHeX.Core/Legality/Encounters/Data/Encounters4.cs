using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Encounters
    /// </summary>
    internal static class Encounters4
    {
        internal static readonly EncounterArea4[] SlotsD = EncounterArea4.GetAreas(Get("d", "da"), D);
        internal static readonly EncounterArea4[] SlotsP = EncounterArea4.GetAreas(Get("p", "pe"), P);
        internal static readonly EncounterArea4[] SlotsPt = EncounterArea4.GetAreas(Get("pt", "pt"), Pt);
        internal static readonly EncounterArea4[] SlotsHG = EncounterArea4.GetAreas(Get("hg", "hg"), HG);
        internal static readonly EncounterArea4[] SlotsSS = EncounterArea4.GetAreas(Get("ss", "ss"), SS);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        static Encounters4()
        {
            MarkG4PokeWalker(Encounter_PokeWalker);

            MarkEncountersGeneration(4, StaticD, StaticP, StaticPt, StaticHG, StaticSS, TradeGift_DPPt, TradeGift_HGSS);

            MarkEncounterTradeStrings(TradeGift_DPPt, TradeDPPt);
            MarkEncounterTradeStrings(TradeGift_HGSS, TradeHGSS);

            foreach (var t in RanchGifts)
                t.TrainerNames = RanchOTNames;

            Encounter_DPPt.SetVersion(DPPt);
            Encounter_HGSS.SetVersion(HGSS);
            TradeGift_DPPt.SetVersion(DPPt);
            TradeGift_HGSS.SetVersion(HGSS);
        }

        private static void MarkG4PokeWalker(IEnumerable<EncounterStatic> t)
        {
            foreach (EncounterStatic s in t)
            {
                s.Location = 233;  //Pokéwalker
                s.Gift = true;    //Pokeball only
                s.Version = HGSS;
            }
        }

        #region Pokéwalker Encounter
        // all pkm are in Poke Ball and have a met location of "PokeWalker"
        private static readonly EncounterStatic4[] Encounter_PokeWalker =
        {
            // Some pkm has a pre-level move, an egg move or even a special move, it might be also available via HM/TM/Tutor
            // Johto/Kanto Courses
            new EncounterStatic4 { Species = 084, Gender = 1, Level = 08, }, // Doduo
            new EncounterStatic4 { Species = 115, Gender = 1, Level = 08, }, // Kangaskhan
            new EncounterStatic4 { Species = 029, Gender = 1, Level = 05, }, // Nidoran♀
            new EncounterStatic4 { Species = 032, Gender = 0, Level = 05, }, // Nidoran♂
            new EncounterStatic4 { Species = 016, Gender = 0, Level = 05, }, // Pidgey
            new EncounterStatic4 { Species = 161, Gender = 1, Level = 05, }, // Sentret
            new EncounterStatic4 { Species = 202, Gender = 1, Level = 15, }, // Wobbuffet
            new EncounterStatic4 { Species = 069, Gender = 1, Level = 08, }, // Bellsprout
            new EncounterStatic4 { Species = 046, Gender = 1, Level = 06, }, // Paras
            new EncounterStatic4 { Species = 048, Gender = 0, Level = 06, }, // Venonat
            new EncounterStatic4 { Species = 021, Gender = 0, Level = 05, }, // Spearow
            new EncounterStatic4 { Species = 043, Gender = 1, Level = 05, }, // Oddish
            new EncounterStatic4 { Species = 095, Gender = 0, Level = 09, }, // Onix
            new EncounterStatic4 { Species = 240, Gender = 0, Level = 09, Moves = new[]{241},}, // Magby: Sunny Day
            new EncounterStatic4 { Species = 066, Gender = 1, Level = 07, }, // Machop
            new EncounterStatic4 { Species = 077, Gender = 1, Level = 07, }, // Ponyta
            new EncounterStatic4 { Species = 074, Gender = 1, Level = 08, Moves = new[]{189},}, // Geodude: Mud-Slap
            new EncounterStatic4 { Species = 163, Gender = 1, Level = 06, }, // Hoothoot
            new EncounterStatic4 { Species = 054, Gender = 1, Level = 10, }, // Psyduck
            new EncounterStatic4 { Species = 120, Gender = 2, Level = 10, }, // Staryu
            new EncounterStatic4 { Species = 060, Gender = 0, Level = 08, }, // Poliwag
            new EncounterStatic4 { Species = 079, Gender = 0, Level = 08, }, // Slowpoke
            new EncounterStatic4 { Species = 191, Gender = 1, Level = 06, }, // Sunkern
            new EncounterStatic4 { Species = 194, Gender = 0, Level = 06, }, // Wooper
            new EncounterStatic4 { Species = 081, Gender = 2, Level = 11, }, // Magnemite
            new EncounterStatic4 { Species = 239, Gender = 0, Level = 11, Moves = new[]{009},}, // Elekid: Thunder Punch
            new EncounterStatic4 { Species = 081, Gender = 2, Level = 08, }, // Magnemite
            new EncounterStatic4 { Species = 198, Gender = 1, Level = 11, }, // Murkrow
            new EncounterStatic4 { Species = 019, Gender = 1, Level = 07, }, // Rattata
            new EncounterStatic4 { Species = 163, Gender = 1, Level = 07, }, // Hoothoot
            new EncounterStatic4 { Species = 092, Gender = 1, Level = 15, Moves = new[]{194},}, // Gastly: Destiny Bond
            new EncounterStatic4 { Species = 238, Gender = 1, Level = 12, Moves = new[]{419},}, // Smoochum: Avalanche
            new EncounterStatic4 { Species = 092, Gender = 1, Level = 10, }, // Gastly
            new EncounterStatic4 { Species = 095, Gender = 0, Level = 10, }, // Onix
            new EncounterStatic4 { Species = 041, Gender = 0, Level = 08, }, // Zubat
            new EncounterStatic4 { Species = 066, Gender = 0, Level = 08, }, // Machop
            new EncounterStatic4 { Species = 060, Gender = 1, Level = 15, Moves = new[]{187}, }, // Poliwag: Belly Drum
            new EncounterStatic4 { Species = 147, Gender = 1, Level = 10, }, // Dratini
            new EncounterStatic4 { Species = 090, Gender = 1, Level = 12, }, // Shellder
            new EncounterStatic4 { Species = 098, Gender = 0, Level = 12, Moves = new[]{152}, }, // Krabby: Crabhammer
            new EncounterStatic4 { Species = 072, Gender = 1, Level = 09, }, // Tentacool
            new EncounterStatic4 { Species = 118, Gender = 1, Level = 09, }, // Goldeen
            new EncounterStatic4 { Species = 063, Gender = 1, Level = 15, }, // Abra
            new EncounterStatic4 { Species = 100, Gender = 2, Level = 15, }, // Voltorb
            new EncounterStatic4 { Species = 088, Gender = 0, Level = 13, }, // Grimer
            new EncounterStatic4 { Species = 109, Gender = 1, Level = 13, Moves = new[]{120}, }, // Koffing: Self-Destruct
            new EncounterStatic4 { Species = 019, Gender = 1, Level = 16, }, // Rattata
            new EncounterStatic4 { Species = 162, Gender = 0, Level = 15, }, // Furret
            // Hoenn Courses
            new EncounterStatic4 { Species = 264, Gender = 1, Level = 30, }, // Linoone
            new EncounterStatic4 { Species = 300, Gender = 1, Level = 30, }, // Skitty
            new EncounterStatic4 { Species = 313, Gender = 0, Level = 25, }, // Volbeat
            new EncounterStatic4 { Species = 314, Gender = 1, Level = 25, }, // Illumise
            new EncounterStatic4 { Species = 263, Gender = 1, Level = 17, }, // Zigzagoon
            new EncounterStatic4 { Species = 265, Gender = 1, Level = 15, }, // Wurmple
            new EncounterStatic4 { Species = 298, Gender = 1, Level = 20, }, // Azurill
            new EncounterStatic4 { Species = 320, Gender = 1, Level = 31, }, // Wailmer
            new EncounterStatic4 { Species = 116, Gender = 1, Level = 20, }, // Horsea
            new EncounterStatic4 { Species = 318, Gender = 1, Level = 26, }, // Carvanha
            new EncounterStatic4 { Species = 118, Gender = 1, Level = 22, Moves = new[]{401}, }, // Goldeen: Aqua Tail
            new EncounterStatic4 { Species = 129, Gender = 1, Level = 15, }, // Magikarp
            new EncounterStatic4 { Species = 218, Gender = 1, Level = 31, }, // Slugma
            new EncounterStatic4 { Species = 307, Gender = 0, Level = 32, }, // Meditite
            new EncounterStatic4 { Species = 111, Gender = 0, Level = 25, }, // Rhyhorn
            new EncounterStatic4 { Species = 228, Gender = 0, Level = 27, }, // Houndour
            new EncounterStatic4 { Species = 074, Gender = 0, Level = 29, }, // Geodude
            new EncounterStatic4 { Species = 077, Gender = 1, Level = 19, }, // Ponyta
            new EncounterStatic4 { Species = 351, Gender = 1, Level = 30, }, // Castform
            new EncounterStatic4 { Species = 352, Gender = 0, Level = 30, }, // Kecleon
            new EncounterStatic4 { Species = 203, Gender = 1, Level = 28, }, // Girafarig
            new EncounterStatic4 { Species = 234, Gender = 1, Level = 28, }, // Stantler
            new EncounterStatic4 { Species = 044, Gender = 1, Level = 14, }, // Gloom
            new EncounterStatic4 { Species = 070, Gender = 0, Level = 13, }, // Weepinbell
            new EncounterStatic4 { Species = 105, Gender = 1, Level = 30, Moves = new[]{037}, }, // Marowak: Thrash
            new EncounterStatic4 { Species = 128, Gender = 0, Level = 30, }, // Tauros
            new EncounterStatic4 { Species = 042, Gender = 0, Level = 33, }, // Golbat
            new EncounterStatic4 { Species = 177, Gender = 1, Level = 24, }, // Natu
            new EncounterStatic4 { Species = 066, Gender = 0, Level = 13, Moves = new[]{418}, }, // Machop: Bullet Punch
            new EncounterStatic4 { Species = 092, Gender = 1, Level = 15, }, // Gastly
            // Sinnoh Courses
            new EncounterStatic4 { Species = 415, Gender = 0, Level = 30, }, // Combee
            new EncounterStatic4 { Species = 439, Gender = 0, Level = 29, }, // Mime Jr.
            new EncounterStatic4 { Species = 403, Gender = 1, Level = 33, }, // Shinx
            new EncounterStatic4 { Species = 406, Gender = 0, Level = 30, }, // Budew
            new EncounterStatic4 { Species = 399, Gender = 1, Level = 13, }, // Bidoof
            new EncounterStatic4 { Species = 401, Gender = 0, Level = 15, }, // Kricketot
            new EncounterStatic4 { Species = 361, Gender = 1, Level = 28, }, // Snorunt
            new EncounterStatic4 { Species = 459, Gender = 0, Level = 31, Moves = new[]{452}, }, // Snover: Wood Hammer
            new EncounterStatic4 { Species = 215, Gender = 0, Level = 28, Moves = new[]{306}, }, // Sneasel: Crash Claw
            new EncounterStatic4 { Species = 436, Gender = 2, Level = 20, }, // Bronzor
            new EncounterStatic4 { Species = 179, Gender = 1, Level = 15, }, // Mareep
            new EncounterStatic4 { Species = 220, Gender = 1, Level = 16, }, // Swinub
            new EncounterStatic4 { Species = 357, Gender = 1, Level = 35, }, // Tropius
            new EncounterStatic4 { Species = 438, Gender = 0, Level = 30, }, // Bonsly
            new EncounterStatic4 { Species = 114, Gender = 1, Level = 30, }, // Tangela
            new EncounterStatic4 { Species = 400, Gender = 1, Level = 30, }, // Bibarel
            new EncounterStatic4 { Species = 102, Gender = 1, Level = 17, }, // Exeggcute
            new EncounterStatic4 { Species = 179, Gender = 0, Level = 19, }, // Mareep
            new EncounterStatic4 { Species = 200, Gender = 1, Level = 32, Moves = new[]{194},}, // Misdreavus: Destiny Bond
            new EncounterStatic4 { Species = 433, Gender = 0, Level = 22, Moves = new[]{105},}, // Chingling: Recover
            new EncounterStatic4 { Species = 093, Gender = 0, Level = 25, }, // Haunter
            new EncounterStatic4 { Species = 418, Gender = 0, Level = 28, Moves = new[]{226},}, // Buizel: Baton Pass
            new EncounterStatic4 { Species = 170, Gender = 1, Level = 17, }, // Chinchou
            new EncounterStatic4 { Species = 223, Gender = 1, Level = 19, }, // Remoraid
            new EncounterStatic4 { Species = 422, Gender = 1, Level = 30, Moves = new[]{243},}, // Shellos: Mirror Coat
            new EncounterStatic4 { Species = 456, Gender = 1, Level = 26, }, // Finneon
            new EncounterStatic4 { Species = 086, Gender = 1, Level = 27, }, // Seel
            new EncounterStatic4 { Species = 129, Gender = 1, Level = 30, }, // Magikarp
            new EncounterStatic4 { Species = 054, Gender = 1, Level = 22, Moves = new[]{281},}, // Psyduck: Yawn
            new EncounterStatic4 { Species = 090, Gender = 0, Level = 20, }, // Shellder
            new EncounterStatic4 { Species = 025, Gender = 1, Level = 30, }, // Pikachu
            new EncounterStatic4 { Species = 417, Gender = 1, Level = 33, Moves = new[]{175},}, // Pachirisu: Flail
            new EncounterStatic4 { Species = 035, Gender = 1, Level = 31, }, // Clefairy
            new EncounterStatic4 { Species = 039, Gender = 1, Level = 30, }, // Jigglypuff
            new EncounterStatic4 { Species = 183, Gender = 1, Level = 25, }, // Marill
            new EncounterStatic4 { Species = 187, Gender = 1, Level = 25, }, // Hoppip
            new EncounterStatic4 { Species = 442, Gender = 0, Level = 31, }, // Spiritomb
            new EncounterStatic4 { Species = 446, Gender = 0, Level = 33, }, // Munchlax
            new EncounterStatic4 { Species = 349, Gender = 0, Level = 30, }, // Feebas
            new EncounterStatic4 { Species = 433, Gender = 1, Level = 26, }, // Chingling
            new EncounterStatic4 { Species = 042, Gender = 0, Level = 33, }, // Golbat
            new EncounterStatic4 { Species = 164, Gender = 1, Level = 30, }, // Noctowl
            // Special Courses
            new EncounterStatic4 { Species = 120, Gender = 2, Level = 18, Moves = new[]{113}, }, // Staryu: Light Screen
            new EncounterStatic4 { Species = 224, Gender = 1, Level = 19, Moves = new[]{324}, }, // Octillery: Signal Beam
            new EncounterStatic4 { Species = 116, Gender = 0, Level = 15, }, // Horsea
            new EncounterStatic4 { Species = 222, Gender = 1, Level = 16, }, // Corsola
            new EncounterStatic4 { Species = 170, Gender = 1, Level = 12, }, // Chinchou
            new EncounterStatic4 { Species = 223, Gender = 0, Level = 14, }, // Remoraid
            new EncounterStatic4 { Species = 035, Gender = 0, Level = 08, Moves = new[]{236}, }, // Clefairy: Moonlight
            new EncounterStatic4 { Species = 039, Gender = 0, Level = 10, }, // Jigglypuff
            new EncounterStatic4 { Species = 041, Gender = 0, Level = 09, }, // Zubat
            new EncounterStatic4 { Species = 163, Gender = 1, Level = 06, }, // Hoothoot
            new EncounterStatic4 { Species = 074, Gender = 0, Level = 05, }, // Geodude
            new EncounterStatic4 { Species = 095, Gender = 1, Level = 05, Moves = new[]{088}, }, // Onix: Rock Throw
            new EncounterStatic4 { Species = 025, Gender = 0, Level = 15, Moves = new[]{019}, }, // Pikachu: Fly
            new EncounterStatic4 { Species = 025, Gender = 1, Level = 14, Moves = new[]{057}, }, // Pikachu: Surf
            new EncounterStatic4 { Species = 025, Gender = 1, Level = 12, Moves = new[]{344, 252}, }, // Pikachu: Volt Tackle, Fake Out
            new EncounterStatic4 { Species = 025, Gender = 0, Level = 13, Moves = new[]{175}, }, // Pikachu: Flail
            new EncounterStatic4 { Species = 025, Gender = 0, Level = 10, }, // Pikachu
            new EncounterStatic4 { Species = 025, Gender = 1, Level = 10, }, // Pikachu
            new EncounterStatic4 { Species = 302, Gender = 1, Level = 15, }, // Sableye
            new EncounterStatic4 { Species = 441, Gender = 0, Level = 15, }, // Chatot
            new EncounterStatic4 { Species = 025, Gender = 1, Level = 10, }, // Pikachu
            new EncounterStatic4 { Species = 453, Gender = 0, Level = 10, }, // Croagunk
            new EncounterStatic4 { Species = 417, Gender = 0, Level = 05, }, // Pachirisu
            new EncounterStatic4 { Species = 427, Gender = 1, Level = 05, }, // Buneary
            new EncounterStatic4 { Species = 133, Gender = 0, Level = 10, }, // Eevee
            new EncounterStatic4 { Species = 255, Gender = 0, Level = 10, }, // Torchic
            new EncounterStatic4 { Species = 061, Gender = 1, Level = 15, Moves = new[]{003}, }, // Poliwhirl: Double Slap
            new EncounterStatic4 { Species = 279, Gender = 0, Level = 15, }, // Pelipper
            new EncounterStatic4 { Species = 025, Gender = 1, Level = 08, }, // Pikachu
            new EncounterStatic4 { Species = 052, Gender = 0, Level = 10, }, // Meowth
            new EncounterStatic4 { Species = 374, Gender = 2, Level = 05, Moves = new[]{428,334,442}, }, // Beldum: Zen Headbutt, Iron Defense & Iron Head.
            new EncounterStatic4 { Species = 446, Gender = 0, Level = 05, Moves = new[]{120}, }, // Munchlax: Self-Destruct
            new EncounterStatic4 { Species = 116, Gender = 0, Level = 05, Moves = new[]{330}, }, // Horsea: Muddy Water
            new EncounterStatic4 { Species = 355, Gender = 0, Level = 05, Moves = new[]{286}, }, // Duskull: Imprison
            new EncounterStatic4 { Species = 129, Gender = 0, Level = 05, Moves = new[]{340}, }, // Magikarp: Bounce
            new EncounterStatic4 { Species = 436, Gender = 2, Level = 05, Moves = new[]{433}, }, // Bronzor: Trick Room
            new EncounterStatic4 { Species = 239, Gender = 0, Level = 05, Moves = new[]{9}}, // Elekid: Thunder Punch (can be tutored)
            new EncounterStatic4 { Species = 240, Gender = 0, Level = 05, Moves = new[]{7}}, // Magby: Fire Punch (can be tutored)
            new EncounterStatic4 { Species = 238, Gender = 1, Level = 05, Moves = new[]{8}}, // Smoochum: Ice Punch (can be tutored)
            new EncounterStatic4 { Species = 440, Gender = 1, Level = 05, Moves = new[]{215}}, // Happiny: Heal Bell
            new EncounterStatic4 { Species = 173, Gender = 1, Level = 05, Moves = new[]{118}}, // Cleffa: Metronome
            new EncounterStatic4 { Species = 174, Gender = 0, Level = 05, Moves = new[]{273}}, // Igglybuff: Wish
        };
        #endregion
        #region Static Encounter/Gift Tables
        private static readonly int[] Roaming_MetLocation_DPPt_Grass =
        {
            // Routes 201-218, 221-222 can be encountered in grass
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 36, 37,
            47,     // Valley Windworks
            49,     // Fuego Ironworks
        };

        private static readonly int[] Roaming_MetLocation_DPPt_Surf =
        {
            // Routes 203-205, 208-210, 212-214, 218-222 can be encountered in water
            18, 19, 20, 23, 24, 25, 27, 28, 29, 33,
            34, 35, 36, 37,
            47,     // Valley Windworks
            49,     // Fuego Ironworks
        };

        private static readonly EncounterStaticTyped[] Encounter_DPPt_Roam_Grass =
        {
            new EncounterStaticTyped { Species = 481, Level = 50, Roaming = true, TypeEncounter = EncounterType.TallGrass }, // Mesprit
            new EncounterStaticTyped { Species = 488, Level = 50, Roaming = true, TypeEncounter = EncounterType.TallGrass }, // Cresselia
            new EncounterStaticTyped { Species = 144, Level = 60, Roaming = true, TypeEncounter = EncounterType.TallGrass, Version = Pt }, // Articuno
            new EncounterStaticTyped { Species = 145, Level = 60, Roaming = true, TypeEncounter = EncounterType.TallGrass, Version = Pt }, // Zapdos
            new EncounterStaticTyped { Species = 146, Level = 60, Roaming = true, TypeEncounter = EncounterType.TallGrass, Version = Pt }, // Moltres
        };

        private static readonly EncounterStaticTyped[] Encounter_DPPt_Roam_Surf =
        {
            new EncounterStaticTyped { Species = 481, Level = 50, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing }, // Mesprit
            new EncounterStaticTyped { Species = 488, Level = 50, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing }, // Cresselia
            new EncounterStaticTyped { Species = 144, Level = 60, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, Version = Pt }, // Articuno
            new EncounterStaticTyped { Species = 145, Level = 60, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, Version = Pt }, // Zapdos
            new EncounterStaticTyped { Species = 146, Level = 60, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, Version = Pt }, // Moltres
        };

        private static readonly EncounterStatic4[] Encounter_DPPt_Regular =
        {
            // Starters
            new EncounterStaticTyped { Gift = true, Species = 387, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Turtwig @ Lake Verity
            new EncounterStaticTyped { Gift = true, Species = 390, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Chimchar
            new EncounterStaticTyped { Gift = true, Species = 393, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Piplup
            new EncounterStaticTyped { Gift = true, Species = 387, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Turtwig @ Route 201
            new EncounterStaticTyped { Gift = true, Species = 390, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Chimchar
            new EncounterStaticTyped { Gift = true, Species = 393, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Piplup

            // Fossil @ Mining Museum
            new EncounterStaticTyped { Gift = true, Species = 138, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Omanyte
            new EncounterStaticTyped { Gift = true, Species = 140, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Kabuto
            new EncounterStaticTyped { Gift = true, Species = 142, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Aerodactyl
            new EncounterStaticTyped { Gift = true, Species = 345, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Lileep
            new EncounterStaticTyped { Gift = true, Species = 347, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP}, // Anorith
            new EncounterStaticTyped { Gift = true, Species = 408, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Cranidos
            new EncounterStaticTyped { Gift = true, Species = 410, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = DP }, // Shieldon
            new EncounterStaticTyped { Gift = true, Species = 138, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Omanyte
            new EncounterStaticTyped { Gift = true, Species = 140, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Kabuto
            new EncounterStaticTyped { Gift = true, Species = 142, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Aerodactyl
            new EncounterStaticTyped { Gift = true, Species = 345, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Lileep
            new EncounterStaticTyped { Gift = true, Species = 347, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt}, // Anorith
            new EncounterStaticTyped { Gift = true, Species = 408, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Cranidos
            new EncounterStaticTyped { Gift = true, Species = 410, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = Pt }, // Shieldon

            // Gift
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 05, Location = 010, Version = DP, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, }, // Eevee @ Hearthome City
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 20, Location = 010, Version = Pt, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Eevee @ Hearthome City
            new EncounterStaticTyped { Gift = true, Species = 137, Level = 25, Location = 012, Version = Pt, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Porygon @ Veilstone City
            new EncounterStatic4 { Gift = true, Species = 175, Level = 01, EggLocation = 2011, Version = Pt,}, // Togepi Egg from Cynthia
            new EncounterStatic4 { Gift = true, Species = 440, Level = 01, EggLocation = 2009, Version = DP,}, // Happiny Egg from Traveling Man
            new EncounterStatic4 { Gift = true, Species = 447, Level = 01, EggLocation = 2010, }, // Riolu Egg from Riley

            // Stationary
            new EncounterStatic4 { Species = 425, Level = 22, Location = 47, Version = DP }, // Drifloon @ Valley Windworks
            new EncounterStatic4 { Species = 425, Level = 15, Location = 47, Version = Pt }, // Drifloon @ Valley Windworks
            new EncounterStaticTyped { Species = 479, Level = 15, Location = 70, Version = DP, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Rotom @ Old Chateau
            new EncounterStaticTyped { Species = 479, Level = 20, Location = 70, Version = Pt, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Rotom @ Old Chateau
            new EncounterStatic4 { Species = 442, Level = 25, Location = 24 }, // Spiritomb @ Route 209

            // Stationary Legendary
            new EncounterStaticTyped { Species = 377, Level = 30, Location = 125, Version = Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Regirock @ Rock Peak Ruins
            new EncounterStaticTyped { Species = 378, Level = 30, Location = 124, Version = Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Regice @ Iceberg Ruins
            new EncounterStaticTyped { Species = 379, Level = 30, Location = 123, Version = Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Registeel @ Iron Ruins
            new EncounterStaticTyped { Species = 480, Level = 50, Location = 089, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Uxie @ Acuity Cavern
            new EncounterStaticTyped { Species = 482, Level = 50, Location = 088, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Azelf @ Valor Cavern
            new EncounterStaticTyped { Species = 483, Level = 47, Location = 051, Version = D, TypeEncounter = EncounterType.DialgaPalkia }, // Dialga @ Spear Pillar
            new EncounterStaticTyped { Species = 484, Level = 47, Location = 051, Version = P, TypeEncounter = EncounterType.DialgaPalkia }, // Palkia @ Spear Pillar
            new EncounterStaticTyped { Species = 483, Level = 70, Location = 051, Version = Pt, TypeEncounter = EncounterType.DialgaPalkia }, // Dialga @ Spear Pillar
            new EncounterStaticTyped { Species = 484, Level = 70, Location = 051, Version = Pt, TypeEncounter = EncounterType.DialgaPalkia }, // Palkia @ Spear Pillar
            new EncounterStaticTyped { Species = 485, Level = 70, Location = 084, Version = DP, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Heatran @ Stark Mountain
            new EncounterStaticTyped { Species = 485, Level = 50, Location = 084, Version = Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Heatran @ Stark Mountain
            new EncounterStaticTyped { Species = 486, Level = 70, Location = 064, Version = DP, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Regigigas @ Snowpoint Temple
            new EncounterStaticTyped { Species = 486, Level = 01, Location = 064, Version = Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Regigigas @ Snowpoint Temple
            new EncounterStaticTyped { Species = 487, Level = 70, Location = 062, Version = DP, Form = 0, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Giratina @ Turnback Cave
            new EncounterStaticTyped { Species = 487, Level = 47, Location = 117, Version = Pt, Form = 1, TypeEncounter = EncounterType.DistortionWorld_Pt, HeldItem = 112 }, // Giratina @ Distortion World
            new EncounterStaticTyped { Species = 487, Level = 47, Location = 062, Version = Pt, Form = 0, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Giratina @ Turnback Cave

            // Event
            new EncounterStaticTyped { Species = 491, Level = 40, Location = 079, Version = DP, TypeEncounter = EncounterType.TallGrass }, // Darkrai @ Newmoon Island (Unreleased in Diamond and Pearl)
            new EncounterStaticTyped { Species = 491, Level = 50, Location = 079, Version = Pt, TypeEncounter = EncounterType.TallGrass }, // Darkrai @ Newmoon Island
            new EncounterStatic4 { Species = 492, Form = 0, Level = 30, Location = 063, Version = Pt, Fateful = true }, // Shaymin @ Flower Paradise
            new EncounterStatic4 { Species = 492, Form = 0, Level = 30, Location = 063, Version = DP, Fateful = false }, // Shaymin @ Flower Paradise (Unreleased in Diamond and Pearl)
            new EncounterStaticTyped { Species = 493, Form = 0, Level = 80, Location = 086, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Arceus @ Hall of Origin (Unreleased)
        };

        private static readonly EncounterStatic4[] Encounter_DPPt = Encounter_DPPt_Roam_Grass.SelectMany(e => e.Clone(Roaming_MetLocation_DPPt_Grass)).Concat(
            Encounter_DPPt_Roam_Surf.SelectMany(e => e.Clone(Roaming_MetLocation_DPPt_Surf))).Concat(
            Encounter_DPPt_Regular).ToArray();

        // Grass 29-39, 42-46, 47, 48
        // Surf 30-32 34-35, 40-45, 47
        // Route 45 innacesible surf
        private static readonly int[] Roaming_MetLocation_HGSS_Johto_Grass =
        {
            // Routes 29-48 can be encountered in grass
            // Won't go to routes 40,41,47,48
            177, 178, 179, 180, 181, 182, 183, 184, 185, 186,
            187,                     190, 191, 192, 193, 194,
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Johto_Surf =
        {
            // Routes 30-32,34-35,40-45 and 47 can be encountered in water
            // Won't go to routes 40,41,47,48
            178, 179, 180, 182, 183, 190, 191, 192, 193
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_JohtoRoam_Grass =
        {
            new EncounterStaticTyped { Species = 243, Level = 40, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Raikou
            new EncounterStaticTyped { Species = 244, Level = 40, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Entei
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_JohtoRoam_Surf =
        {
            new EncounterStaticTyped { Species = 243, Level = 40, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Raikou
            new EncounterStaticTyped { Species = 244, Level = 40, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Entei
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Kanto_Grass =
        {
            // Route 01-18,21,22,24,26 and 28 can be encountered in grass
            // Won't go to route 23 25 27
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158,
            159, 160, 161, 162, 163, 164, 165, 166,
            169, 170,      172,      174,      176,
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Kanto_Surf =
        {
            // Route 4,6,9,10,12,13,19-22,24,26 and 28 can be encountered in water
            // Won't go to route 23 25 27
            152, 154, 157, 158, 160, 161, 167, 168, 169, 170,
            172,      174,      176,
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_KantoRoam_Grass =
        {
            new EncounterStaticTyped { Species = 380, Level = 35, Version = HG, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Latias
            new EncounterStaticTyped { Species = 381, Level = 35, Version = SS, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Latios
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_KantoRoam_Surf =
        {
            new EncounterStaticTyped { Species = 380, Level = 35, Version = HG, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Latias
            new EncounterStaticTyped { Species = 381, Level = 35, Version = SS, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Latios
        };

        internal static readonly EncounterStaticTyped SpikyEaredPichu = new EncounterStaticTyped // Spiky-Eared Pichu @ Ilex Forest
        {
            Species = 172,
            Level = 30,
            Gender = 1,
            Form = 1,
            Nature = Nature.Naughty,
            Location = 214,
            Moves = new[] { 344, 270, 207, 220 },
            TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio,
            Shiny = Shiny.Never
        };

        private static readonly EncounterStatic4[] Encounter_HGSS_Regular =
        {
            // Starters
            new EncounterStaticTyped { Gift = true, Species = 001, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Bulbasaur @ Pallet Town
            new EncounterStaticTyped { Gift = true, Species = 004, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Charmander
            new EncounterStaticTyped { Gift = true, Species = 007, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Squirtle
            new EncounterStaticTyped { Gift = true, Species = 152, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Chikorita @ New Bark Town
            new EncounterStaticTyped { Gift = true, Species = 155, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Cyndaquil
            new EncounterStaticTyped { Gift = true, Species = 158, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Totodile
            new EncounterStaticTyped { Gift = true, Species = 252, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Treecko @ Saffron City
            new EncounterStaticTyped { Gift = true, Species = 255, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Torchic
            new EncounterStaticTyped { Gift = true, Species = 258, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Mudkip

            // Fossils @ Pewter City
            new EncounterStaticTyped { Gift = true, Species = 138, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Omanyte
            new EncounterStaticTyped { Gift = true, Species = 140, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Kabuto
            new EncounterStaticTyped { Gift = true, Species = 142, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Aerodactyl
            new EncounterStaticTyped { Gift = true, Species = 345, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Lileep
            new EncounterStaticTyped { Gift = true, Species = 347, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Anorith
            new EncounterStaticTyped { Gift = true, Species = 408, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Cranidos
            new EncounterStaticTyped { Gift = true, Species = 410, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Shieldon

            // Gift
            new EncounterStaticTyped { Gift = true, Species = 072, Level = 15, Location = 130, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Tentacool @ Cianwood City
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 05, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Eevee @ Goldenrod City
            new EncounterStaticTyped { Gift = true, Species = 147, Level = 15, Location = 222, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Moves = new[] {245} }, // Dratini @ Dragon's Den (ExtremeSpeed)
            new EncounterStaticTyped { Gift = true, Species = 236, Level = 10, Location = 216, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Tyrogue @ Mt. Mortar
            new EncounterStatic4 { Gift = true, Species = 175, Level = 01, EggLocation = 2013, Moves = new[] {326} }, // Togepi Egg from Mr. Pokemon (Extrasensory as Egg move)
            new EncounterStatic4 { Gift = true, Species = 179, Level = 01, EggLocation = 2014, }, // Mareep Egg from Primo
            new EncounterStatic4 { Gift = true, Species = 194, Level = 01, EggLocation = 2014, }, // Wooper Egg from Primo
            new EncounterStatic4 { Gift = true, Species = 218, Level = 01, EggLocation = 2014, }, // Slugma Egg from Primo

            // Celadon City Game Corner
            new EncounterStaticTyped { Gift = true, Species = 122, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Mr. Mime
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Eevee
            new EncounterStaticTyped { Gift = true, Species = 137, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Porygon

            // Goldenrod City Game Corner
            new EncounterStaticTyped { Gift = true, Species = 063, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Abra
            new EncounterStaticTyped { Gift = true, Species = 023, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = HG }, // Ekans
            new EncounterStaticTyped { Gift = true, Species = 027, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = SS }, // Sandshrew
            new EncounterStaticTyped { Gift = true, Species = 147, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Dratini

            // Team Rocket HQ Trap Floor
            new EncounterStaticTyped { Species = 100, Level = 23, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Voltorb
            new EncounterStaticTyped { Species = 074, Level = 21, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Geodude
            new EncounterStaticTyped { Species = 109, Level = 21, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Koffing

            // Stationary
            new EncounterStaticTyped { Species = 130, Level = 30, Location = 135, TypeEncounter = EncounterType.Surfing_Fishing, Shiny = Shiny.Always }, // Gyarados @ Lake of Rage
            new EncounterStaticTyped { Species = 131, Level = 20, Location = 210, TypeEncounter = EncounterType.Surfing_Fishing, }, // Lapras @ Union Cave Friday Only
            new EncounterStaticTyped { Species = 101, Level = 23, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Electrode @ Team Rocket HQ
            new EncounterStatic4 { Species = 143, Level = 50, Location = 159, }, // Snorlax @ Route 11
            new EncounterStatic4 { Species = 143, Level = 50, Location = 160, }, // Snorlax @ Route 12
            new EncounterStatic4 { Species = 185, Level = 20, Location = 184, }, // Sudowoodo @ Route 36, Encounter does not have type
            SpikyEaredPichu,

            // Stationary Legendary
            new EncounterStaticTyped { Species = 144, Level = 50, Location = 203, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Articuno @ Seafoam Islands
            new EncounterStatic4 { Species = 145, Level = 50, Location = 158, }, // Zapdos @ Route 10
            new EncounterStaticTyped { Species = 146, Level = 50, Location = 219, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Moltres @ Mt. Silver Cave
            new EncounterStaticTyped { Species = 150, Level = 70, Location = 199, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Mewtwo @ Cerulean Cave
            new EncounterStatic4 { Species = 245, Level = 40, Location = 173, }, // Suicune @ Route 25
            new EncounterStaticTyped { Species = 245, Level = 40, Location = 206, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Suicune @ Burned Tower
            new EncounterStaticTyped { Species = 249, Level = 45, Location = 218, Version = SS, TypeEncounter = EncounterType.Surfing_Fishing }, // Lugia @ Whirl Islands
            new EncounterStaticTyped { Species = 249, Level = 70, Location = 218, Version = HG, TypeEncounter = EncounterType.Surfing_Fishing }, // Lugia @ Whirl Islands
            new EncounterStaticTyped { Species = 250, Level = 45, Location = 205, Version = HG, TypeEncounter = EncounterType.Building_EnigmaStone }, // Ho-Oh @ Bell Tower
            new EncounterStaticTyped { Species = 250, Level = 70, Location = 205, Version = SS, TypeEncounter = EncounterType.Building_EnigmaStone }, // Ho-Oh @ Bell Tower
            new EncounterStaticTyped { Species = 380, Level = 40, Location = 140, Version = SS, TypeEncounter = EncounterType.Building_EnigmaStone }, // Latias @ Pewter City
            new EncounterStaticTyped { Species = 381, Level = 40, Location = 140, Version = HG, TypeEncounter = EncounterType.Building_EnigmaStone }, // Latios @ Pewter City
            new EncounterStaticTyped { Species = 382, Level = 50, Location = 232, Version = HG, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Kyogre @ Embedded Tower
            new EncounterStaticTyped { Species = 383, Level = 50, Location = 232, Version = SS, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Groudon @ Embedded Tower
            new EncounterStaticTyped { Species = 384, Level = 50, Location = 232, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Rayquaza @ Embedded Tower
            new EncounterStaticTyped { Species = 483, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Dialga @ Sinjoh Ruins
            new EncounterStaticTyped { Species = 484, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Palkia @ Sinjoh Ruins
            new EncounterStaticTyped { Species = 487, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Form = 1, HeldItem = 112 }, // Giratina @ Sinjoh Ruins
        };

        private static readonly EncounterStatic4[] Encounter_HGSS = ConcatAll(
            Encounter_HGSS_KantoRoam_Grass.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Kanto_Grass)),
            Encounter_HGSS_KantoRoam_Surf.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Kanto_Surf)),
            Encounter_HGSS_JohtoRoam_Grass.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Johto_Grass)),
            Encounter_HGSS_JohtoRoam_Surf.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Johto_Surf)),
            Encounter_HGSS_Regular);
        #endregion
        #region Trade Tables
        private static readonly string[] RanchOTNames = { string.Empty, "ユカリ", "Hayley", "EULALIE", "GIULIA", "EUKALIA", string.Empty, "Eulalia" };

        private static readonly EncounterTrade[] RanchGifts =
        {
            new EncounterTrade4PID(323975838) { Species = 025, Level = 18, Moves = new[] {447,085,148,104}, TID = 1000, SID = 19840, OTGender = 1, Version = D, Location = 0068, Gender = 0, Ability = 1, CurrentLevel = 20, }, // Pikachu
            new EncounterTrade4PID(323977664) { Species = 037, Level = 16, Moves = new[] {412,109,053,219}, TID = 1000, SID = 21150, OTGender = 1, Version = D, Location = 3000, Gender = 0, Ability = 1, CurrentLevel = 30, }, // Vulpix
            new EncounterTrade4PID(323975579) { Species = 077, Level = 13, Moves = new[] {036,033,039,052}, TID = 1000, SID = 01123, OTGender = 1, Version = D, Location = 3000, Gender = 0, Ability = 2, CurrentLevel = 16, }, // Ponyta
            new EncounterTrade4PID(323975564) { Species = 108, Level = 34, Moves = new[] {076,111,014,205}, TID = 1000, SID = 03050, OTGender = 1, Version = D, Location = 0077, Gender = 0, Ability = 1, CurrentLevel = 40, }, // Lickitung
            new EncounterTrade4PID(323977579) { Species = 114, Level = 01, Moves = new[] {437,438,079,246}, TID = 1000, SID = 49497, OTGender = 1, Version = D, Location = 3000, Gender = 1, Ability = 2, }, // Tangela
            new EncounterTrade4PID(323977675) { Species = 133, Level = 16, Moves = new[] {363,270,098,247}, TID = 1000, SID = 47710, OTGender = 1, Version = D, Location = 0068, Gender = 0, Ability = 2, CurrentLevel = 30, }, // Eevee
            new EncounterTrade4PID(323977588) { Species = 142, Level = 20, Moves = new[] {363,089,444,332}, TID = 1000, SID = 43066, OTGender = 1, Version = D, Location = 0094, Gender = 0, Ability = 1, CurrentLevel = 50, }, // Aerodactyl
            new EncounterTrade4     { Species = 151, Level = 50, Moves = new[] {235,216,095,100}, TID = 1000, SID = 59228, OTGender = 1, Version = D, Location = 3000, Gender = 2, Fateful = true, Ball = 0x10, }, // Mew
            new EncounterTrade4PID(232975554) { Species = 193, Level = 22, Moves = new[] {318,095,246,138}, TID = 1000, SID = 42301, OTGender = 1, Version = D, Location = 0052, Gender = 0, Ability = 1, CurrentLevel = 45, Ball = 0x05, }, // Yanma
            new EncounterTrade4PID(323975570) { Species = 241, Level = 16, Moves = new[] {208,215,360,359}, TID = 1000, SID = 02707, OTGender = 1, Version = D, Location = 3000, Gender = 1, Ability = 1, CurrentLevel = 48, }, // Miltank
            new EncounterTrade4PID(323975563) { Species = 285, Level = 22, Moves = new[] {402,147,206,078}, TID = 1000, SID = 02788, OTGender = 1, Version = D, Location = 3000, Gender = 0, Ability = 2, CurrentLevel = 45, Ball = 0x05, }, // Shroomish
            new EncounterTrade4PID(323975559) { Species = 320, Level = 30, Moves = new[] {156,323,133,058}, TID = 1000, SID = 27046, OTGender = 1, Version = D, Location = 0038, Gender = 0, Ability = 2, CurrentLevel = 45, }, // Wailmer
            new EncounterTrade4PID(323977657) { Species = 360, Level = 01, Moves = new[] {204,150,227,000}, TID = 1000, SID = 01788, OTGender = 1, Version = D, Location = 0004, Gender = 0, Ability = 2, EggLocation = 2000, }, // Wynaut
            new EncounterTrade4PID(323975563) { Species = 397, Level = 02, Moves = new[] {355,017,283,018}, TID = 1000, SID = 59298, OTGender = 1, Version = D, Location = 0016, Gender = 0, Ability = 2, CurrentLevel = 23, }, // Staravia
            new EncounterTrade4PID(323970584) { Species = 415, Level = 05, Moves = new[] {230,016,000,000}, TID = 1000, SID = 54140, OTGender = 1, Version = D, Location = 0020, Gender = 1, Ability = 1, CurrentLevel = 20, }, // Combee
            new EncounterTrade4PID(323977539) { Species = 417, Level = 09, Moves = new[] {447,045,351,098}, TID = 1000, SID = 18830, OTGender = 1, Version = D, Location = 0020, Gender = 1, Ability = 2, CurrentLevel = 10, }, // Pachirisu
            new EncounterTrade4PID(323974107) { Species = 422, Level = 20, Moves = new[] {363,352,426,104}, TID = 1000, SID = 39272, OTGender = 1, Version = D, Location = 0028, Gender = 0, Ability = 2, CurrentLevel = 25, Form = 1 }, // Shellos
            new EncounterTrade4PID(323977566) { Species = 427, Level = 10, Moves = new[] {204,193,409,098}, TID = 1000, SID = 31045, OTGender = 1, Version = D, Location = 3000, Gender = 1, Ability = 1, CurrentLevel = 16, }, // Buneary
            new EncounterTrade4PID(323975579) { Species = 453, Level = 22, Moves = new[] {310,207,426,389}, TID = 1000, SID = 41342, OTGender = 1, Version = D, Location = 0052, Gender = 0, Ability = 2, CurrentLevel = 31, Ball = 0x05, }, // Croagunk
            new EncounterTrade4PID(323977566) { Species = 456, Level = 15, Moves = new[] {213,352,219,392}, TID = 1000, SID = 48348, OTGender = 1, Version = D, Location = 0020, Gender = 1, Ability = 1, CurrentLevel = 35, }, // Finneon
            new EncounterTrade4PID(323975582) { Species = 459, Level = 32, Moves = new[] {452,420,275,059}, TID = 1000, SID = 23360, OTGender = 1, Version = D, Location = 0031, Gender = 0, Ability = 1, CurrentLevel = 41, }, // Snover
            new EncounterTrade4    { Species = 489, Level = 01, Moves = new[] {447,240,156,057}, TID = 1000, SID = 09248, OTGender = 1, Version = D, Location = 3000, Gender = 2, Fateful = true, CurrentLevel = 50, Ball = 0x10, EggLocation = 3000, }, // Phione
        };

        internal static readonly EncounterTrade[] TradeGift_DPPt = new []
        {
            new EncounterTrade4PID(0x0000008E) { Species = 063, Level = 01, Ability = 1, TID = 25643, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {15,15,15,20,25,25} }, // Machop -> Abra
            new EncounterTrade4PID(0x00000867) { Species = 441, Level = 01, Ability = 2, TID = 44142, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,25,25,15}, Contest = new[] {20,20,20,20,20,0} }, // Buizel -> Chatot
            new EncounterTrade4PID(0x00000088) { Species = 093, Level = 35, Ability = 1, TID = 19248, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,25,15,25,15,15} }, // Medicham (35 from Route 217) -> Haunter
            new EncounterTrade4PID(0x0000045C) { Species = 129, Level = 01, Ability = 1, TID = 53277, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,15,20,25,15} }, // Finneon -> Magikarp
        }.Concat(RanchGifts).ToArray();

        internal static readonly EncounterTrade4PID[] TradeGift_HGSS =
        {
            new EncounterTrade4PID(0x000025EF) { Species = 095, Level = 01, Ability = 2, TID = 48926, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {25,20,25,15,15,15} }, // Bellsprout -> Onix
            new EncounterTrade4PID(0x00002310) { Species = 066, Level = 01, Ability = 1, TID = 37460, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,20,20,15,15} }, // Drowzee -> Machop
            new EncounterTrade4PID(0x000001DB) { Species = 100, Level = 01, Ability = 2, TID = 29189, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,25,25,15} }, // Krabby -> Voltorb
            new EncounterTrade4PID(0x0001FC0A) { Species = 085, Level = 15, Ability = 1, TID = 00283, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,15,15,15} }, // Dragonair (15 from DPPt) -> Dodrio
            new EncounterTrade4PID(0x0000D136) { Species = 082, Level = 19, Ability = 1, TID = 50082, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,20,20,20} }, // Dugtrio (19 from Diglett's Cave) -> Magneton
            new EncounterTrade4PID(0x000034E4) { Species = 178, Level = 16, Ability = 1, TID = 15616, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20} }, // Haunter (16 from Old Chateau) -> Xatu
            new EncounterTrade4PID(0x00485876) { Species = 025, Level = 02, Ability = 1, TID = 33038, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {20,25,18,31,25,13} }, // Pikachu
            new EncounterTrade4PID(0x0012B6D4) { Species = 374, Level = 31, Ability = 1, TID = 23478, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {28,29,24,23,24,25} }, // Forretress -> Beldum
            new EncounterTrade4PID(0x0012971C) { Species = 111, Level = 01, Ability = 1, TID = 06845, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,31,13,00,22,09}, Moves = new[]{422} }, // Bonsly -> Rhyhorn
            new EncounterTrade4PID(0x00101596) { Species = 208, Level = 01, Ability = 1, TID = 26491, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {08,30,28,06,18,20}}, // Any -> Steelix

            //Gift
            new EncounterTrade4PID(0x00006B5E) { Species = 021, Ability = 1, TID = 01001, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,20,20,20}, Level = 20, Location = 183, Moves = new[]{043,031,228,332} },// Webster's Spearow
            new EncounterTrade4PID(0x000214D7) { Species = 213, Ability = 2, TID = 04336, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20}, Level = 20, Location = 130, Moves = new[]{132,117,227,219} },// Kirk's Shuckle
        };

        private const string tradeDPPt = "tradedppt";
        private const string tradeHGSS = "tradehgss";
        private static readonly string[][] TradeDPPt = Util.GetLanguageStrings8(tradeDPPt);
        private static readonly string[][] TradeHGSS = Util.GetLanguageStrings8(tradeHGSS);
        #endregion

        internal static readonly EncounterStatic4[] StaticD = GetEncounters(Encounter_DPPt, D);
        internal static readonly EncounterStatic4[] StaticP = GetEncounters(Encounter_DPPt, P);
        internal static readonly EncounterStatic4[] StaticPt = GetEncounters(Encounter_DPPt, Pt);
        internal static readonly EncounterStatic4[] StaticHG = GetEncounters(ArrayUtil.ConcatAll(Encounter_HGSS, Encounter_PokeWalker), HG);
        internal static readonly EncounterStatic4[] StaticSS = GetEncounters(ArrayUtil.ConcatAll(Encounter_HGSS, Encounter_PokeWalker), SS);
    }
}
