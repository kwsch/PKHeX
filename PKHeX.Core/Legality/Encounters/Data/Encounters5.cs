using System;
using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Encounters
    /// </summary>
    public static class Encounters5
    {
        internal static readonly EncounterArea5[] SlotsB = EncounterArea5.GetAreas(Get("b", "51"), GameVersion.B);
        internal static readonly EncounterArea5[] SlotsW = EncounterArea5.GetAreas(Get("w", "51"), GameVersion.W);
        internal static readonly EncounterArea5[] SlotsB2 = EncounterArea5.GetAreas(Get("b2", "52"), GameVersion.B2);
        internal static readonly EncounterArea5[] SlotsW2 = EncounterArea5.GetAreas(Get("w2", "52"), GameVersion.W2);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        static Encounters5()
        {
            MarkEncounterTradeStrings(TradeGift_BW, TradeBW);
            MarkEncounterTradeStrings(TradeGift_B2W2_Regular, TradeB2W2);
            foreach (var t in TradeGift_B2W2_YancyCurtis)
                t.TrainerNames = t.OTGender == 0 ? TradeOT_B2W2_M : TradeOT_B2W2_F;

            DreamWorld_Common.SetVersion(GameVersion.Gen5);
            DreamWorld_BW.SetVersion(GameVersion.BW);
            DreamWorld_B2W2.SetVersion(GameVersion.B2W2);
            Encounter_BW.SetVersion(GameVersion.BW);
            Encounter_B2W2.SetVersion(GameVersion.B2W2);
            TradeGift_BW.SetVersion(GameVersion.BW);
            TradeGift_B2W2.SetVersion(GameVersion.B2W2);
        }

        private static EncounterStatic5[] MarkG5DreamWorld(EncounterStatic5[] t)
        {
            // Split encounters with multiple permitted special moves -- a pkm can only be obtained with 1 of the special moves!
            var count = t.Sum(z => Math.Max(1, z.Moves.Count));
            var result = new EncounterStatic5[count];

            int ctr = 0;
            foreach (var s in t)
            {
                s.Location = 075; // Entree Forest
                var p = (PersonalInfoBW)PersonalTable.B2W2[s.Species];
                s.Ability = p.HasHiddenAbility ? 4 : 1;
                s.Shiny = Shiny.Never;

                var moves = s.Moves;
                if (moves.Count <= 1) // no special moves
                {
                    result[ctr++] = s;
                    continue;
                }

                foreach (var move in moves)
                {
                    var clone = (EncounterStatic5)s.Clone();
                    clone.Moves = new[] { move };
                    result[ctr++] = clone;
                }
            }
            return result;
        }

        #region Dream Radar Tables

        private static readonly EncounterStatic5DR[] Encounter_DreamRadar =
        {
            new EncounterStatic5DR(120, 0), // Staryu
            new EncounterStatic5DR(137, 0), // Porygon
            new EncounterStatic5DR(174, 0), // Igglybuff
            new EncounterStatic5DR(175, 0), // Togepi
            new EncounterStatic5DR(213, 0), // Shuckle
            new EncounterStatic5DR(238, 0), // Smoochum
            new EncounterStatic5DR(280, 0), // Ralts
            new EncounterStatic5DR(333, 0), // Swablu
            new EncounterStatic5DR(425, 0), // Drifloon
            new EncounterStatic5DR(436, 0), // Bronzor
            new EncounterStatic5DR(442, 0), // Spiritomb
            new EncounterStatic5DR(447, 0), // Riolu
            new EncounterStatic5DR(479, 0, 0), // Rotom (no HA)
            new EncounterStatic5DR(517, 0), // Munna
            new EncounterStatic5DR(561, 0), // Sigilyph
            new EncounterStatic5DR(641, 1), // Therian Tornadus
            new EncounterStatic5DR(642, 1), // Therian Thundurus
            new EncounterStatic5DR(645, 1), // Therian Landorus
            new EncounterStatic5DR(249, 0), // Lugia (SoulSilver cart)
            new EncounterStatic5DR(250, 0), // Ho-Oh (HeartGold cart)
            new EncounterStatic5DR(483, 0), // Dialga (Diamond cart)
            new EncounterStatic5DR(484, 0), // Palkia (Pearl cart)
            new EncounterStatic5DR(487, 0), // Giratina (Platinum cart)
            new EncounterStatic5DR(079, 0), // Slowpoke
            new EncounterStatic5DR(163, 0), // Hoothoot
            new EncounterStatic5DR(374, 0), // Beldum
        };

        #endregion
        #region DreamWorld Encounter

        public static readonly EncounterStatic5[] DreamWorld_Common = MarkG5DreamWorld(new[]
        {
            // Pleasant Forest
            new EncounterStatic5 { Species = 019, Level = 10, Moves = new[]{098, 382, 231}, }, // Rattata
            new EncounterStatic5 { Species = 043, Level = 10, Moves = new[]{230, 298, 202}, }, // Oddish
            new EncounterStatic5 { Species = 069, Level = 10, Moves = new[]{022, 235, 402}, }, // Bellsprout
            new EncounterStatic5 { Species = 077, Level = 10, Moves = new[]{033, 037, 257}, }, // Ponyta
            new EncounterStatic5 { Species = 083, Level = 10, Moves = new[]{210, 355, 348}, }, // Farfetch'd
            new EncounterStatic5 { Species = 084, Level = 10, Moves = new[]{045, 175, 355}, }, // Doduo
            new EncounterStatic5 { Species = 102, Level = 10, Moves = new[]{140, 235, 202}, }, // Exeggcute
            new EncounterStatic5 { Species = 108, Level = 10, Moves = new[]{122, 214, 431}, }, // Lickitung
            new EncounterStatic5 { Species = 114, Level = 10, Moves = new[]{079, 073, 402}, }, // Tangela
            new EncounterStatic5 { Species = 115, Level = 10, Moves = new[]{252, 068, 409}, }, // Kangaskhan
            new EncounterStatic5 { Species = 161, Level = 10, Moves = new[]{010, 203, 343}, }, // Sentret
            new EncounterStatic5 { Species = 179, Level = 10, Moves = new[]{084, 115, 351}, }, // Mareep
            new EncounterStatic5 { Species = 191, Level = 10, Moves = new[]{072, 230, 414}, }, // Sunkern
            new EncounterStatic5 { Species = 234, Level = 10, Moves = new[]{033, 050, 285}, }, // Stantler
            new EncounterStatic5 { Species = 261, Level = 10, Moves = new[]{336, 305, 399}, }, // Poochyena
            new EncounterStatic5 { Species = 283, Level = 10, Moves = new[]{145, 056, 202}, }, // Surskit
            new EncounterStatic5 { Species = 399, Level = 10, Moves = new[]{033, 401, 290}, }, // Bidoof
            new EncounterStatic5 { Species = 403, Level = 10, Moves = new[]{268, 393, 400}, }, // Shinx
            new EncounterStatic5 { Species = 431, Level = 10, Moves = new[]{252, 372, 290}, }, // Glameow
            new EncounterStatic5 { Species = 054, Level = 10, Moves = new[]{346, 227, 362}, }, // Psyduck
            new EncounterStatic5 { Species = 058, Level = 10, Moves = new[]{044, 034, 203}, }, // Growlithe
            new EncounterStatic5 { Species = 123, Level = 10, Moves = new[]{098, 226, 366}, }, // Scyther
            new EncounterStatic5 { Species = 128, Level = 10, Moves = new[]{099, 231, 431}, }, // Tauros
            new EncounterStatic5 { Species = 183, Level = 10, Moves = new[]{111, 453, 008}, }, // Marill
            new EncounterStatic5 { Species = 185, Level = 10, Moves = new[]{175, 205, 272}, }, // Sudowoodo
            new EncounterStatic5 { Species = 203, Level = 10, Moves = new[]{093, 243, 285}, }, // Girafarig
            new EncounterStatic5 { Species = 241, Level = 10, Moves = new[]{111, 174, 231}, }, // Miltank
            new EncounterStatic5 { Species = 263, Level = 10, Moves = new[]{033, 271, 387}, }, // Zigzagoon
            new EncounterStatic5 { Species = 427, Level = 10, Moves = new[]{193, 252, 409}, }, // Buneary
            new EncounterStatic5 { Species = 037, Level = 10, Moves = new[]{046, 257, 399}, }, // Vulpix
            new EncounterStatic5 { Species = 060, Level = 10, Moves = new[]{095, 054, 214}, }, // Poliwag
            new EncounterStatic5 { Species = 177, Level = 10, Moves = new[]{101, 297, 202}, }, // Natu
            new EncounterStatic5 { Species = 239, Level = 10, Moves = new[]{084, 238, 393}, }, // Elekid
            new EncounterStatic5 { Species = 300, Level = 10, Moves = new[]{193, 321, 445}, }, // Skitty

            // Windswept Sky
            new EncounterStatic5 { Species = 016, Level = 10, Moves = new[]{016, 211, 290}, }, // Pidgey
            new EncounterStatic5 { Species = 021, Level = 10, Moves = new[]{064, 185, 211}, }, // Spearow
            new EncounterStatic5 { Species = 041, Level = 10, Moves = new[]{048, 095, 162}, }, // Zubat
            new EncounterStatic5 { Species = 142, Level = 10, Moves = new[]{044, 372, 446}, }, // Aerodactyl
            new EncounterStatic5 { Species = 165, Level = 10, Moves = new[]{004, 450, 009}, }, // Ledyba
            new EncounterStatic5 { Species = 187, Level = 10, Moves = new[]{235, 227, 340}, }, // Hoppip
            new EncounterStatic5 { Species = 193, Level = 10, Moves = new[]{098, 364, 202}, }, // Yanma
            new EncounterStatic5 { Species = 198, Level = 10, Moves = new[]{064, 109, 355}, }, // Murkrow
            new EncounterStatic5 { Species = 207, Level = 10, Moves = new[]{028, 364, 366}, }, // Gligar
            new EncounterStatic5 { Species = 225, Level = 10, Moves = new[]{217, 420, 264}, }, // Delibird
            new EncounterStatic5 { Species = 276, Level = 10, Moves = new[]{064, 203, 413}, }, // Taillow
            new EncounterStatic5 { Species = 397, Level = 14, Moves = new[]{017, 297, 366}, }, // Staravia
            new EncounterStatic5 { Species = 227, Level = 10, Moves = new[]{064, 065, 355}, }, // Skarmory
            new EncounterStatic5 { Species = 357, Level = 10, Moves = new[]{016, 073, 318}, }, // Tropius

            // Sparkling Sea
            new EncounterStatic5 { Species = 086, Level = 10, Moves = new[]{029, 333, 214}, }, // Seel
            new EncounterStatic5 { Species = 090, Level = 10, Moves = new[]{110, 112, 196}, }, // Shellder
            new EncounterStatic5 { Species = 116, Level = 10, Moves = new[]{145, 190, 362}, }, // Horsea
            new EncounterStatic5 { Species = 118, Level = 10, Moves = new[]{064, 060, 352}, }, // Goldeen
            new EncounterStatic5 { Species = 129, Level = 10, Moves = new[]{150, 175, 340}, }, // Magikarp
            new EncounterStatic5 { Species = 138, Level = 10, Moves = new[]{044, 330, 196}, }, // Omanyte
            new EncounterStatic5 { Species = 140, Level = 10, Moves = new[]{071, 175, 446}, }, // Kabuto
            new EncounterStatic5 { Species = 170, Level = 10, Moves = new[]{086, 133, 351}, }, // Chinchou
            new EncounterStatic5 { Species = 194, Level = 10, Moves = new[]{055, 034, 401}, }, // Wooper
            new EncounterStatic5 { Species = 211, Level = 10, Moves = new[]{040, 453, 290}, }, // Qwilfish
            new EncounterStatic5 { Species = 223, Level = 10, Moves = new[]{199, 350, 362}, }, // Remoraid
            new EncounterStatic5 { Species = 226, Level = 10, Moves = new[]{048, 243, 314}, }, // Mantine
            new EncounterStatic5 { Species = 320, Level = 10, Moves = new[]{055, 214, 340}, }, // Wailmer
            new EncounterStatic5 { Species = 339, Level = 10, Moves = new[]{189, 214, 209}, }, // Barboach
            new EncounterStatic5 { Species = 366, Level = 10, Moves = new[]{250, 445, 392}, }, // Clamperl
            new EncounterStatic5 { Species = 369, Level = 10, Moves = new[]{055, 214, 414}, }, // Relicanth
            new EncounterStatic5 { Species = 370, Level = 10, Moves = new[]{204, 300, 196}, }, // Luvdisc
            new EncounterStatic5 { Species = 418, Level = 10, Moves = new[]{346, 163, 352}, }, // Buizel
            new EncounterStatic5 { Species = 456, Level = 10, Moves = new[]{213, 186, 352}, }, // Finneon
            new EncounterStatic5 { Species = 072, Level = 10, Moves = new[]{048, 367, 202}, }, // Tentacool
            new EncounterStatic5 { Species = 318, Level = 10, Moves = new[]{044, 037, 399}, }, // Carvanha
            new EncounterStatic5 { Species = 341, Level = 10, Moves = new[]{106, 232, 283}, }, // Corphish
            new EncounterStatic5 { Species = 345, Level = 10, Moves = new[]{051, 243, 202}, }, // Lileep
            new EncounterStatic5 { Species = 347, Level = 10, Moves = new[]{010, 446, 440}, }, // Anorith
            new EncounterStatic5 { Species = 349, Level = 10, Moves = new[]{150, 445, 243}, }, // Feebas
            new EncounterStatic5 { Species = 131, Level = 10, Moves = new[]{109, 032, 196}, }, // Lapras
            new EncounterStatic5 { Species = 147, Level = 10, Moves = new[]{086, 352, 225}, }, // Dratini

            // Spooky Manor
            new EncounterStatic5 { Species = 092, Level = 10, Moves = new[]{095, 050, 482}, }, // Gastly
            new EncounterStatic5 { Species = 096, Level = 10, Moves = new[]{095, 427, 409}, }, // Drowzee
            new EncounterStatic5 { Species = 122, Level = 10, Moves = new[]{112, 298, 285}, }, // Mr. Mime
            new EncounterStatic5 { Species = 167, Level = 10, Moves = new[]{040, 527, 450}, }, // Spinarak
            new EncounterStatic5 { Species = 200, Level = 10, Moves = new[]{149, 194, 517}, }, // Misdreavus
            new EncounterStatic5 { Species = 228, Level = 10, Moves = new[]{336, 364, 399}, }, // Houndour
            new EncounterStatic5 { Species = 325, Level = 10, Moves = new[]{149, 285, 278}, }, // Spoink
            new EncounterStatic5 { Species = 353, Level = 10, Moves = new[]{101, 194, 220}, }, // Shuppet
            new EncounterStatic5 { Species = 355, Level = 10, Moves = new[]{050, 220, 271}, }, // Duskull
            new EncounterStatic5 { Species = 358, Level = 10, Moves = new[]{035, 095, 304}, }, // Chimecho
            new EncounterStatic5 { Species = 434, Level = 10, Moves = new[]{103, 492, 389}, }, // Stunky
            new EncounterStatic5 { Species = 209, Level = 10, Moves = new[]{204, 370, 038}, }, // Snubbull
            new EncounterStatic5 { Species = 235, Level = 10, Moves = new[]{166, 445, 214}, }, // Smeargle
            new EncounterStatic5 { Species = 313, Level = 10, Moves = new[]{148, 271, 366}, }, // Volbeat
            new EncounterStatic5 { Species = 314, Level = 10, Moves = new[]{204, 313, 366}, }, // Illumise
            new EncounterStatic5 { Species = 063, Level = 10, Moves = new[]{100, 285, 356}, }, // Abra

            // Rugged Mountain
            new EncounterStatic5 { Species = 066, Level = 10, Moves = new[]{067, 418, 270}, }, // Machop
            new EncounterStatic5 { Species = 081, Level = 10, Moves = new[]{319, 278, 356}, }, // Magnemite
            new EncounterStatic5 { Species = 109, Level = 10, Moves = new[]{123, 399, 482}, }, // Koffing
            new EncounterStatic5 { Species = 218, Level = 10, Moves = new[]{052, 517, 257}, }, // Slugma
            new EncounterStatic5 { Species = 246, Level = 10, Moves = new[]{044, 399, 446}, }, // Larvitar
            new EncounterStatic5 { Species = 324, Level = 10, Moves = new[]{052, 090, 446}, }, // Torkoal
            new EncounterStatic5 { Species = 328, Level = 10, Moves = new[]{044, 324, 202}, }, // Trapinch
            new EncounterStatic5 { Species = 331, Level = 10, Moves = new[]{071, 298, 009}, }, // Cacnea
            new EncounterStatic5 { Species = 412, Level = 10, Moves = new[]{182, 450, 173}, }, // Burmy
            new EncounterStatic5 { Species = 449, Level = 10, Moves = new[]{044, 254, 276}, }, // Hippopotas
            new EncounterStatic5 { Species = 240, Level = 10, Moves = new[]{052, 009, 257}, }, // Magby
            new EncounterStatic5 { Species = 322, Level = 10, Moves = new[]{052, 034, 257}, }, // Numel
            new EncounterStatic5 { Species = 359, Level = 10, Moves = new[]{364, 224, 276}, }, // Absol
            new EncounterStatic5 { Species = 453, Level = 10, Moves = new[]{040, 409, 441}, }, // Croagunk
            new EncounterStatic5 { Species = 236, Level = 10, Moves = new[]{252, 364, 183}, }, // Tyrogue
            new EncounterStatic5 { Species = 371, Level = 10, Moves = new[]{044, 349, 200}, }, // Bagon

            // Icy Cave
            new EncounterStatic5 { Species = 027, Level = 10, Moves = new[]{028, 068, 162}, }, // Sandshrew
            new EncounterStatic5 { Species = 074, Level = 10, Moves = new[]{111, 446, 431}, }, // Geodude
            new EncounterStatic5 { Species = 095, Level = 10, Moves = new[]{020, 446, 431}, }, // Onix
            new EncounterStatic5 { Species = 100, Level = 10, Moves = new[]{268, 324, 363}, }, // Voltorb
            new EncounterStatic5 { Species = 104, Level = 10, Moves = new[]{125, 195, 067}, }, // Cubone
            new EncounterStatic5 { Species = 293, Level = 10, Moves = new[]{253, 283, 428}, }, // Whismur
            new EncounterStatic5 { Species = 304, Level = 10, Moves = new[]{106, 283, 457}, }, // Aron
            new EncounterStatic5 { Species = 337, Level = 10, Moves = new[]{093, 414, 236}, }, // Lunatone
            new EncounterStatic5 { Species = 338, Level = 10, Moves = new[]{093, 428, 234}, }, // Solrock
            new EncounterStatic5 { Species = 343, Level = 10, Moves = new[]{229, 356, 428}, }, // Baltoy
            new EncounterStatic5 { Species = 459, Level = 10, Moves = new[]{075, 419, 202}, }, // Snover
            new EncounterStatic5 { Species = 050, Level = 10, Moves = new[]{028, 251, 446}, }, // Diglett
            new EncounterStatic5 { Species = 215, Level = 10, Moves = new[]{269, 008, 067}, }, // Sneasel
            new EncounterStatic5 { Species = 361, Level = 10, Moves = new[]{181, 311, 352}, }, // Snorunt
            new EncounterStatic5 { Species = 220, Level = 10, Moves = new[]{316, 246, 333}, }, // Swinub
            new EncounterStatic5 { Species = 443, Level = 10, Moves = new[]{082, 200, 203}, }, // Gible

            // Dream Park
            new EncounterStatic5 { Species = 046, Level = 10, Moves = new[]{078, 440, 235}, }, // Paras
            new EncounterStatic5 { Species = 204, Level = 10, Moves = new[]{120, 390, 356}, }, // Pineco
            new EncounterStatic5 { Species = 265, Level = 10, Moves = new[]{040, 450, 173}, }, // Wurmple
            new EncounterStatic5 { Species = 273, Level = 10, Moves = new[]{074, 331, 492}, }, // Seedot
            new EncounterStatic5 { Species = 287, Level = 10, Moves = new[]{281, 400, 389}, }, // Slakoth
            new EncounterStatic5 { Species = 290, Level = 10, Moves = new[]{141, 203, 400}, }, // Nincada
            new EncounterStatic5 { Species = 311, Level = 10, Moves = new[]{086, 435, 324}, }, // Plusle
            new EncounterStatic5 { Species = 312, Level = 10, Moves = new[]{086, 435, 324}, }, // Minun
            new EncounterStatic5 { Species = 316, Level = 10, Moves = new[]{139, 151, 202}, }, // Gulpin
            new EncounterStatic5 { Species = 352, Level = 10, Moves = new[]{185, 285, 513}, }, // Kecleon
            new EncounterStatic5 { Species = 401, Level = 10, Moves = new[]{522, 283, 253}, }, // Kricketot
            new EncounterStatic5 { Species = 420, Level = 10, Moves = new[]{073, 505, 331}, }, // Cherubi
            new EncounterStatic5 { Species = 455, Level = 10, Moves = new[]{044, 476, 380}, }, // Carnivine
            new EncounterStatic5 { Species = 023, Level = 10, Moves = new[]{040, 251, 399}, }, // Ekans
            new EncounterStatic5 { Species = 175, Level = 10, Moves = new[]{118, 381, 253}, }, // Togepi
            new EncounterStatic5 { Species = 190, Level = 10, Moves = new[]{010, 252, 007}, }, // Aipom
            new EncounterStatic5 { Species = 285, Level = 10, Moves = new[]{078, 331, 264}, }, // Shroomish
            new EncounterStatic5 { Species = 315, Level = 10, Moves = new[]{074, 079, 129}, }, // Roselia
            new EncounterStatic5 { Species = 113, Level = 10, Moves = new[]{045, 068, 270}, }, // Chansey
            new EncounterStatic5 { Species = 127, Level = 10, Moves = new[]{011, 370, 382}, }, // Pinsir
            new EncounterStatic5 { Species = 133, Level = 10, Moves = new[]{028, 204, 129}, }, // Eevee
            new EncounterStatic5 { Species = 143, Level = 10, Moves = new[]{133, 007, 278}, }, // Snorlax
            new EncounterStatic5 { Species = 214, Level = 10, Moves = new[]{030, 175, 264}, }, // Heracross

            // Pokémon Café Forest
            new EncounterStatic5 { Species = 061, Level = 25, Moves = new[]{240, 114, 352}, }, // Poliwhirl
            new EncounterStatic5 { Species = 133, Level = 10, Moves = new[]{270, 204, 129}, }, // Eevee
            new EncounterStatic5 { Species = 235, Level = 10, Moves = new[]{166, 445, 214}, }, // Smeargle
            new EncounterStatic5 { Species = 412, Level = 10, Moves = new[]{182, 450, 173}, }, // Burmy

            // PGL
            new EncounterStatic5 { Species = 212, Level = 10, Moves = new[]{211}, Gender = 0, }, // Scizor
            new EncounterStatic5 { Species = 445, Level = 48, Gender = 0, }, // Garchomp
            new EncounterStatic5 { Species = 149, Level = 55, Moves = new[]{245}, Gender = 0, }, // Dragonite
            new EncounterStatic5 { Species = 248, Level = 55, Moves = new[]{069}, Gender = 0, }, // Tyranitar
            new EncounterStatic5 { Species = 376, Level = 45, Moves = new[]{038}, Gender = 2, }, // Metagross
        });

        public static readonly EncounterStatic5[] DreamWorld_BW = MarkG5DreamWorld(new[]
        {
            // Pleasant Forest
            new EncounterStatic5 { Species = 029, Level = 10, Moves = new[]{010, 389, 162}, }, // Nidoran♀
            new EncounterStatic5 { Species = 032, Level = 10, Moves = new[]{064, 068, 162}, }, // Nidoran♂
            new EncounterStatic5 { Species = 174, Level = 10, Moves = new[]{047, 313, 270}, }, // Igglybuff
            new EncounterStatic5 { Species = 187, Level = 10, Moves = new[]{235, 270, 331}, }, // Hoppip
            new EncounterStatic5 { Species = 270, Level = 10, Moves = new[]{071, 073, 352}, }, // Lotad
            new EncounterStatic5 { Species = 276, Level = 10, Moves = new[]{064, 119, 366}, }, // Taillow
            new EncounterStatic5 { Species = 309, Level = 10, Moves = new[]{086, 423, 324}, }, // Electrike
            new EncounterStatic5 { Species = 351, Level = 10, Moves = new[]{052, 466, 352}, }, // Castform
            new EncounterStatic5 { Species = 417, Level = 10, Moves = new[]{098, 343, 351}, }, // Pachirisu

            // Windswept Sky
            new EncounterStatic5 { Species = 012, Level = 10, Moves = new[]{093, 355, 314}, }, // Butterfree
            new EncounterStatic5 { Species = 163, Level = 10, Moves = new[]{193, 101, 278}, }, // Hoothoot
            new EncounterStatic5 { Species = 278, Level = 10, Moves = new[]{055, 239, 351}, }, // Wingull
            new EncounterStatic5 { Species = 333, Level = 10, Moves = new[]{064, 297, 355}, }, // Swablu
            new EncounterStatic5 { Species = 425, Level = 10, Moves = new[]{107, 095, 285}, }, // Drifloon
            new EncounterStatic5 { Species = 441, Level = 10, Moves = new[]{119, 417, 272}, }, // Chatot

            // Sparkling Sea
            new EncounterStatic5 { Species = 079, Level = 10, Moves = new[]{281, 335, 362}, }, // Slowpoke
            new EncounterStatic5 { Species = 098, Level = 10, Moves = new[]{011, 133, 290}, }, // Krabby
            new EncounterStatic5 { Species = 119, Level = 33, Moves = new[]{352, 214, 203}, }, // Seaking
            new EncounterStatic5 { Species = 120, Level = 10, Moves = new[]{055, 278, 196}, }, // Staryu
            new EncounterStatic5 { Species = 222, Level = 10, Moves = new[]{145, 109, 446}, }, // Corsola
            new EncounterStatic5 { Species = 422, Level = 10, Moves = new[]{189, 281, 290}, Form = 0 }, // Shellos-West
            new EncounterStatic5 { Species = 422, Level = 10, Moves = new[]{189, 281, 290}, Form = 1 }, // Shellos-East

            // Spooky Manor
            new EncounterStatic5 { Species = 202, Level = 15, Moves = new[]{243, 204, 227}, }, // Wobbuffet
            new EncounterStatic5 { Species = 238, Level = 10, Moves = new[]{186, 445, 285}, }, // Smoochum
            new EncounterStatic5 { Species = 303, Level = 10, Moves = new[]{313, 424, 008}, }, // Mawile
            new EncounterStatic5 { Species = 307, Level = 10, Moves = new[]{096, 409, 203}, }, // Meditite
            new EncounterStatic5 { Species = 436, Level = 10, Moves = new[]{095, 285, 356}, }, // Bronzor
            new EncounterStatic5 { Species = 052, Level = 10, Moves = new[]{010, 095, 290}, }, // Meowth
            new EncounterStatic5 { Species = 479, Level = 10, Moves = new[]{086, 351, 324}, }, // Rotom
            new EncounterStatic5 { Species = 280, Level = 10, Moves = new[]{093, 194, 270}, }, // Ralts
            new EncounterStatic5 { Species = 302, Level = 10, Moves = new[]{193, 389, 180}, }, // Sableye
            new EncounterStatic5 { Species = 442, Level = 10, Moves = new[]{180, 220, 196}, }, // Spiritomb

            // Rugged Mountain
            new EncounterStatic5 { Species = 056, Level = 10, Moves = new[]{067, 179, 009}, }, // Mankey
            new EncounterStatic5 { Species = 111, Level = 10, Moves = new[]{030, 068, 038}, }, // Rhyhorn
            new EncounterStatic5 { Species = 231, Level = 10, Moves = new[]{175, 484, 402}, }, // Phanpy
            new EncounterStatic5 { Species = 451, Level = 10, Moves = new[]{044, 097, 401}, }, // Skorupi
            new EncounterStatic5 { Species = 216, Level = 10, Moves = new[]{313, 242, 264}, }, // Teddiursa
            new EncounterStatic5 { Species = 296, Level = 10, Moves = new[]{292, 270, 008}, }, // Makuhita
            new EncounterStatic5 { Species = 327, Level = 10, Moves = new[]{383, 252, 276}, }, // Spinda
            new EncounterStatic5 { Species = 374, Level = 10, Moves = new[]{036, 428, 442}, }, // Beldum
            new EncounterStatic5 { Species = 447, Level = 10, Moves = new[]{203, 418, 264}, }, // Riolu

            // Icy Cave
            new EncounterStatic5 { Species = 173, Level = 10, Moves = new[]{227, 312, 214}, }, // Cleffa
            new EncounterStatic5 { Species = 213, Level = 10, Moves = new[]{227, 270, 504}, }, // Shuckle
            new EncounterStatic5 { Species = 299, Level = 10, Moves = new[]{033, 446, 246}, }, // Nosepass
            new EncounterStatic5 { Species = 363, Level = 10, Moves = new[]{181, 090, 401}, }, // Spheal
            new EncounterStatic5 { Species = 408, Level = 10, Moves = new[]{029, 442, 007}, }, // Cranidos
            new EncounterStatic5 { Species = 206, Level = 10, Moves = new[]{111, 277, 446}, }, // Dunsparce
            new EncounterStatic5 { Species = 410, Level = 10, Moves = new[]{182, 068, 090}, }, // Shieldon

            // Dream Park
            new EncounterStatic5 { Species = 048, Level = 10, Moves = new[]{050, 226, 285}, }, // Venonat
            new EncounterStatic5 { Species = 088, Level = 10, Moves = new[]{139, 114, 425}, }, // Grimer
            new EncounterStatic5 { Species = 415, Level = 10, Moves = new[]{016, 366, 314}, }, // Combee
            new EncounterStatic5 { Species = 015, Level = 10, Moves = new[]{031, 314, 210}, }, // Beedrill
            new EncounterStatic5 { Species = 335, Level = 10, Moves = new[]{098, 458, 067}, }, // Zangoose
            new EncounterStatic5 { Species = 336, Level = 10, Moves = new[]{044, 034, 401}, }, // Seviper

            // PGL
            new EncounterStatic5 { Species = 134, Level = 10, Gender = 0, }, // Vaporeon
            new EncounterStatic5 { Species = 135, Level = 10, Gender = 0, }, // Jolteon
            new EncounterStatic5 { Species = 136, Level = 10, Gender = 0, }, // Flareon
            new EncounterStatic5 { Species = 196, Level = 10, Gender = 0, }, // Espeon
            new EncounterStatic5 { Species = 197, Level = 10, Gender = 0, }, // Umbreon
            new EncounterStatic5 { Species = 470, Level = 10, Gender = 0, }, // Leafeon
            new EncounterStatic5 { Species = 471, Level = 10, Gender = 0, }, // Glaceon
            new EncounterStatic5 { Species = 001, Level = 10, Gender = 0, }, // Bulbasaur
            new EncounterStatic5 { Species = 004, Level = 10, Gender = 0, }, // Charmander
            new EncounterStatic5 { Species = 007, Level = 10, Gender = 0, }, // Squirtle
            new EncounterStatic5 { Species = 453, Level = 10, Gender = 0, }, // Croagunk
            new EncounterStatic5 { Species = 387, Level = 10, Gender = 0, }, // Turtwig
            new EncounterStatic5 { Species = 390, Level = 10, Gender = 0, }, // Chimchar
            new EncounterStatic5 { Species = 393, Level = 10, Gender = 0, }, // Piplup
            new EncounterStatic5 { Species = 493, Level = 100 }, // Arceus
            new EncounterStatic5 { Species = 252, Level = 10, Gender = 0, }, // Treecko
            new EncounterStatic5 { Species = 255, Level = 10, Gender = 0, }, // Torchic
            new EncounterStatic5 { Species = 258, Level = 10, Gender = 0, }, // Mudkip
            new EncounterStatic5 { Species = 468, Level = 10, Moves = new[]{217}, Gender = 0, }, // Togekiss
            new EncounterStatic5 { Species = 473, Level = 34, Gender = 0, }, // Mamoswine
            new EncounterStatic5 { Species = 137, Level = 10 }, // Porygon
            new EncounterStatic5 { Species = 384, Level = 50 }, // Rayquaza
            new EncounterStatic5 { Species = 354, Level = 37, Moves = new[]{538}, Gender = 1, }, // Banette
            new EncounterStatic5 { Species = 453, Level = 10, Moves = new[]{398}, Gender = 0, }, // Croagunk
            new EncounterStatic5 { Species = 334, Level = 35, Moves = new[]{206}, Gender = 0, },  // Altaria
            new EncounterStatic5 { Species = 242, Level = 10 }, // Blissey
            new EncounterStatic5 { Species = 448, Level = 10, Moves = new[]{418}, Gender = 0, }, // Lucario
            new EncounterStatic5 { Species = 189, Level = 27, Moves = new[]{206}, Gender = 0, }, // Jumpluff
        });

        public static readonly EncounterStatic5[] DreamWorld_B2W2 = MarkG5DreamWorld(new[]
        {
            // Pleasant Forest
            new EncounterStatic5 { Species = 535, Level = 10, Moves = new[]{496, 414, 352}, }, // Tympole
            new EncounterStatic5 { Species = 546, Level = 10, Moves = new[]{073, 227, 388}, }, // Cottonee
            new EncounterStatic5 { Species = 548, Level = 10, Moves = new[]{079, 204, 230}, }, // Petilil
            new EncounterStatic5 { Species = 588, Level = 10, Moves = new[]{203, 224, 450}, }, // Karrablast
            new EncounterStatic5 { Species = 616, Level = 10, Moves = new[]{051, 226, 227}, }, // Shelmet
            new EncounterStatic5 { Species = 545, Level = 30, Moves = new[]{342, 390, 276}, }, // Scolipede

            // Windswept Sky
            new EncounterStatic5 { Species = 519, Level = 10, Moves = new[]{016, 095, 234}, }, // Pidove
            new EncounterStatic5 { Species = 561, Level = 10, Moves = new[]{095, 500, 257}, }, // Sigilyph
            new EncounterStatic5 { Species = 580, Level = 10, Moves = new[]{432, 362, 382}, }, // Ducklett
            new EncounterStatic5 { Species = 587, Level = 10, Moves = new[]{098, 403, 204}, }, // Emolga

            // Sparkling Sea
            new EncounterStatic5 { Species = 550, Level = 10, Moves = new[]{029, 097, 428}, Form = 0 }, // Basculin-Red
            new EncounterStatic5 { Species = 550, Level = 10, Moves = new[]{029, 097, 428}, Form = 1 }, // Basculin-Blue
            new EncounterStatic5 { Species = 594, Level = 10, Moves = new[]{392, 243, 220}, }, // Alomomola
            new EncounterStatic5 { Species = 618, Level = 10, Moves = new[]{189, 174, 281}, }, // Stunfisk
            new EncounterStatic5 { Species = 564, Level = 10, Moves = new[]{205, 175, 334}, }, // Tirtouga

            // Spooky Manor
            new EncounterStatic5 { Species = 605, Level = 10, Moves = new[]{377, 112, 417}, }, // Elgyem
            new EncounterStatic5 { Species = 624, Level = 10, Moves = new[]{210, 427, 389}, }, // Pawniard
            new EncounterStatic5 { Species = 596, Level = 36, Moves = new[]{486, 050, 228}, }, // Galvantula
            new EncounterStatic5 { Species = 578, Level = 32, Moves = new[]{105, 286, 271}, }, // Duosion
            new EncounterStatic5 { Species = 622, Level = 10, Moves = new[]{205, 007, 009}, }, // Golett

            // Rugged Mountain
            new EncounterStatic5 { Species = 631, Level = 10, Moves = new[]{510, 257, 202}, }, // Heatmor
            new EncounterStatic5 { Species = 632, Level = 10, Moves = new[]{210, 203, 422}, }, // Durant
            new EncounterStatic5 { Species = 556, Level = 10, Moves = new[]{042, 073, 191}, }, // Maractus
            new EncounterStatic5 { Species = 558, Level = 34, Moves = new[]{157, 068, 400}, }, // Crustle
            new EncounterStatic5 { Species = 553, Level = 40, Moves = new[]{242, 068, 212}, }, // Krookodile

            // Icy Cave
            new EncounterStatic5 { Species = 529, Level = 10, Moves = new[]{229, 319, 431}, }, // Drilbur
            new EncounterStatic5 { Species = 621, Level = 10, Moves = new[]{044, 424, 389}, }, // Druddigon
            new EncounterStatic5 { Species = 525, Level = 25, Moves = new[]{479, 174, 484}, }, // Boldore
            new EncounterStatic5 { Species = 583, Level = 35, Moves = new[]{429, 420, 286}, }, // Vanillish
            new EncounterStatic5 { Species = 600, Level = 38, Moves = new[]{451, 356, 393}, }, // Klang
            new EncounterStatic5 { Species = 610, Level = 10, Moves = new[]{082, 068, 400}, }, // Axew

            // Dream Park
            new EncounterStatic5 { Species = 531, Level = 10, Moves = new[]{270, 227, 281}, }, // Audino
            new EncounterStatic5 { Species = 538, Level = 10, Moves = new[]{020, 008, 276}, }, // Throh
            new EncounterStatic5 { Species = 539, Level = 10, Moves = new[]{249, 009, 530}, }, // Sawk
            new EncounterStatic5 { Species = 559, Level = 10, Moves = new[]{067, 252, 409}, }, // Scraggy
            new EncounterStatic5 { Species = 533, Level = 25, Moves = new[]{067, 183, 409}, }, // Gurdurr

            // PGL
            new EncounterStatic5 { Species = 575, Level = 32, Moves = new[]{243}, Gender = 0, }, // Gothorita
            new EncounterStatic5 { Species = 025, Level = 10, Moves = new[]{029}, Gender = 0, }, // Pikachu
            new EncounterStatic5 { Species = 511, Level = 10, Moves = new[]{437}, Gender = 0, }, // Pansage
            new EncounterStatic5 { Species = 513, Level = 10, Moves = new[]{257}, Gender = 0, }, // Pansear
            new EncounterStatic5 { Species = 515, Level = 10, Moves = new[]{056}, Gender = 0, }, // Panpour
            new EncounterStatic5 { Species = 387, Level = 10, Moves = new[]{254}, Gender = 0, }, // Turtwig
            new EncounterStatic5 { Species = 390, Level = 10, Moves = new[]{252}, Gender = 0, }, // Chimchar
            new EncounterStatic5 { Species = 393, Level = 10, Moves = new[]{297}, Gender = 0, }, // Piplup
            new EncounterStatic5 { Species = 575, Level = 32, Moves = new[]{286}, Gender = 0, }, // Gothorita
        });

        #endregion
        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic5[] Encounter_BW =
        {
            // Starters @ Nuvema Town
            new EncounterStatic5 { Gift = true, Species = 495, Level = 5, Location = 004, }, // Snivy
            new EncounterStatic5 { Gift = true, Species = 498, Level = 5, Location = 004, }, // Tepig
            new EncounterStatic5 { Gift = true, Species = 501, Level = 5, Location = 004, }, // Oshawott

            // Fossils @ Nacrene City
            new EncounterStatic5 { Gift = true, Species = 138, Level = 25, Location = 007, }, // Omanyte
            new EncounterStatic5 { Gift = true, Species = 140, Level = 25, Location = 007, }, // Kabuto
            new EncounterStatic5 { Gift = true, Species = 142, Level = 25, Location = 007, }, // Aerodactyl
            new EncounterStatic5 { Gift = true, Species = 345, Level = 25, Location = 007, }, // Lileep
            new EncounterStatic5 { Gift = true, Species = 347, Level = 25, Location = 007, }, // Anorith
            new EncounterStatic5 { Gift = true, Species = 408, Level = 25, Location = 007, }, // Cranidos
            new EncounterStatic5 { Gift = true, Species = 410, Level = 25, Location = 007, }, // Shieldon
            new EncounterStatic5 { Gift = true, Species = 564, Level = 25, Location = 007, }, // Tirtouga
            new EncounterStatic5 { Gift = true, Species = 566, Level = 25, Location = 007, }, // Archen

            // Gift
            new EncounterStatic5 { Gift = true, Species = 511, Level = 10, Location = 032, }, // Pansage @ Dreamyard
            new EncounterStatic5 { Gift = true, Species = 513, Level = 10, Location = 032, }, // Pansear
            new EncounterStatic5 { Gift = true, Species = 515, Level = 10, Location = 032, }, // Panpour
            new EncounterStatic5 { Gift = true, Species = 129, Level = 05, Location = 068, }, // Magikarp @ Marvelous Bridge
            new EncounterStatic5 { Gift = true, Species = 636, Level = 01, EggLocation = 60003, }, // Larvesta Egg from Treasure Hunter

            // Stationary
            new EncounterStatic5 { Species = 518, Level = 50, Location = 032, Ability = 4, }, // Musharna @ Dreamyard Friday Only
            new EncounterStatic5 { Species = 590, Level = 20, Location = 019, }, // Foongus @ Route 6
            new EncounterStatic5 { Species = 590, Level = 30, Location = 023, }, // Foongus @ Route 10
            new EncounterStatic5 { Species = 591, Level = 40, Location = 023, }, // Amoonguss @ Route 10
            new EncounterStatic5 { Species = 555, Level = 35, Location = 034, Ability = 4, }, // HA Darmanitan @ Desert Resort
            new EncounterStatic5 { Species = 637, Level = 70, Location = 035, }, // Volcarona @ Relic Castle

            // Stationary Legendary
            new EncounterStatic5 { Species = 638, Level = 42, Location = 074, }, // Cobalion @ Guidance Chamber
            new EncounterStatic5 { Species = 639, Level = 42, Location = 073, }, // Terrakion @ Trial Chamber
            new EncounterStatic5 { Species = 640, Level = 42, Location = 055, }, // Virizion @ Rumination Field
            new EncounterStatic5 { Species = 643, Level = 50, Location = 045, Shiny = Shiny.Never, Version = GameVersion.B, }, // Reshiram @ N's Castle
            new EncounterStatic5 { Species = 643, Level = 50, Location = 039, Shiny = Shiny.Never, Version = GameVersion.B, }, // Reshiram @ Dragonspiral Tower
            new EncounterStatic5 { Species = 644, Level = 50, Location = 045, Shiny = Shiny.Never, Version = GameVersion.W, }, // Zekrom @ N's Castle
            new EncounterStatic5 { Species = 644, Level = 50, Location = 039, Shiny = Shiny.Never, Version = GameVersion.W, }, // Zekrom @ Dragonspiral Tower
            new EncounterStatic5 { Species = 645, Level = 70, Location = 070, }, // Landorus @ Abundant Shrine
            new EncounterStatic5 { Species = 646, Level = 75, Location = 061, }, // Kyurem @ Giant Chasm

            // Event
            new EncounterStatic5 { Species = 494, Level = 15, Location = 062, Shiny = Shiny.Never}, // Victini @ Liberty Garden
            new EncounterStatic5 { Species = 570, Level = 10, Location = 008, Shiny = Shiny.Never, Gender = 0, }, // Zorua @ Castelia City
            new EncounterStatic5 { Species = 571, Level = 25, Location = 072, Shiny = Shiny.Never, Gender = 1, }, // Zoroark @ Lostlorn Forest

            // Roamer
            new EncounterStatic5 { Roaming = true, Species = 641, Level = 40, Version = GameVersion.B, Location = 25, }, // Tornadus
            new EncounterStatic5 { Roaming = true, Species = 642, Level = 40, Version = GameVersion.W, Location = 25, }, // Thundurus
        };

        private static readonly EncounterStatic5[] Encounter_B2W2_Regular =
        {
            // Starters @ Aspertia City
            new EncounterStatic5 { Gift = true, Species = 495, Level = 5, Location = 117, }, // Snivy
            new EncounterStatic5 { Gift = true, Species = 498, Level = 5, Location = 117, }, // Tepig
            new EncounterStatic5 { Gift = true, Species = 501, Level = 5, Location = 117, }, // Oshawott

            // Fossils @ Nacrene City
            new EncounterStatic5 { Gift = true, Species = 138, Level = 25, Location = 007, }, // Omanyte
            new EncounterStatic5 { Gift = true, Species = 140, Level = 25, Location = 007, }, // Kabuto
            new EncounterStatic5 { Gift = true, Species = 142, Level = 25, Location = 007, }, // Aerodactyl
            new EncounterStatic5 { Gift = true, Species = 345, Level = 25, Location = 007, }, // Lileep
            new EncounterStatic5 { Gift = true, Species = 347, Level = 25, Location = 007, }, // Anorith
            new EncounterStatic5 { Gift = true, Species = 408, Level = 25, Location = 007, }, // Cranidos
            new EncounterStatic5 { Gift = true, Species = 410, Level = 25, Location = 007, }, // Shieldon
            new EncounterStatic5 { Gift = true, Species = 564, Level = 25, Location = 007, }, // Tirtouga
            new EncounterStatic5 { Gift = true, Species = 566, Level = 25, Location = 007, }, // Archen

            // Gift
            new EncounterStatic5 { Gift = true, Species = 133, Level = 10, Ability = 4, Location = 008, }, // HA Eevee @ Castelia City
            new EncounterStatic5 { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 019, Form = 0, }, // HA Deerling @ Route 6
            new EncounterStatic5 { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 019, Form = 1, }, // HA Deerling @ Route 6
            new EncounterStatic5 { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 019, Form = 2, }, // HA Deerling @ Route 6
            new EncounterStatic5 { Gift = true, Species = 585, Level = 30, Ability = 4, Location = 019, Form = 3, }, // HA Deerling @ Route 6
            new EncounterStatic5 { Gift = true, Species = 443, Level = 01, Shiny = Shiny.Always, Location = 122, Gender = 0, Version = GameVersion.B2, }, // Shiny Gible @ Floccesy Town
            new EncounterStatic5 { Gift = true, Species = 147, Level = 01, Shiny = Shiny.Always, Location = 122, Gender = 0, Version = GameVersion.W2, }, // Shiny Dratini @ Floccesy Town
            new EncounterStatic5 { Gift = true, Species = 129, Level = 05, Location = 068, }, // Magikarp @ Marvelous Bridge
            new EncounterStatic5 { Gift = true, Species = 440, Level = 01, EggLocation = 60003, }, // Happiny Egg from PKMN Breeder

            // Stationary
            new EncounterStatic5 { Species = 590, Level = 29, Location = 019, }, // Foongus @ Route 6
            new EncounterStatic5 { Species = 591, Level = 43, Location = 024, }, // Amoonguss @ Route 11
            new EncounterStatic5 { Species = 591, Level = 47, Location = 127, }, // Amoonguss @ Route 22
            new EncounterStatic5 { Species = 591, Level = 56, Location = 128, }, // Amoonguss @ Route 23
            new EncounterStatic5 { Species = 593, Level = 40, Location = 071, Ability = 4, Version = GameVersion.B2, Gender = 0, }, // HA Jellicent @ Undella Bay Mon Only
            new EncounterStatic5 { Species = 593, Level = 40, Location = 071, Ability = 4, Version = GameVersion.W2, Gender = 1, }, // HA Jellicent @ Undella Bay Thurs Only
            new EncounterStatic5 { Species = 593, Level = 40, Location = 071 }, // HA Jellicent @ Undella Bay EncounterSlot collision
            new EncounterStatic5 { Species = 628, Level = 25, Location = 017, Ability = 4, Version = GameVersion.W2, Gender = 0, }, // HA Braviary @ Route 4 Mon Only
            new EncounterStatic5 { Species = 630, Level = 25, Location = 017, Ability = 4, Version = GameVersion.B2, Gender = 1, }, // HA Mandibuzz @ Route 4 Thurs Only
            new EncounterStatic5 { Species = 637, Level = 35, Location = 035, }, // Volcarona @ Relic Castle
            new EncounterStatic5 { Species = 637, Level = 65, Location = 035, }, // Volcarona @ Relic Castle
            new EncounterStatic5 { Species = 558, Level = 42, Location = 141, }, // Crustle @ Seaside Cave
            new EncounterStatic5 { Species = 612, Level = 60, Location = 147, Shiny = Shiny.Always}, // Haxorus @ Nature Preserve

            // Stationary Legendary
            new EncounterStatic5 { Species = 377, Level = 65, Location = 150, }, // Regirock @ Rock Peak Chamber
            new EncounterStatic5 { Species = 378, Level = 65, Location = 151, }, // Regice @ Iceberg Chamber
            new EncounterStatic5 { Species = 379, Level = 65, Location = 152, }, // Registeel @ Iron Chamber
            new EncounterStatic5 { Species = 380, Level = 68, Location = 032, Version = GameVersion.W2, }, // Latias @ Dreamyard
            new EncounterStatic5 { Species = 381, Level = 68, Location = 032, Version = GameVersion.B2, }, // Latios @ Dreamyard
            new EncounterStatic5 { Species = 480, Level = 65, Location = 007, }, // Uxie @ Nacrene City
            new EncounterStatic5 { Species = 481, Level = 65, Location = 056, }, // Mesprit @ Celestial Tower
            new EncounterStatic5 { Species = 482, Level = 65, Location = 128, }, // Azelf @ Route 23
            new EncounterStatic5 { Species = 485, Level = 68, Location = 132, }, // Heatran @ Reversal Mountain
            new EncounterStatic5 { Species = 486, Level = 68, Location = 038, }, // Regigigas @ Twist Mountain
            new EncounterStatic5 { Species = 488, Level = 68, Location = 068, }, // Cresselia @ Marvelous Bridge
            new EncounterStatic5 { Species = 638, Level = 45, Location = 026, }, // Cobalion @ Route 13
            new EncounterStatic5 { Species = 638, Level = 65, Location = 026, }, // Cobalion @ Route 13
            new EncounterStatic5 { Species = 639, Level = 45, Location = 127, }, // Terrakion @ Route 22
            new EncounterStatic5 { Species = 639, Level = 65, Location = 127, }, // Terrakion @ Route 22
            new EncounterStatic5 { Species = 640, Level = 45, Location = 024, }, // Virizion @ Route 11
            new EncounterStatic5 { Species = 640, Level = 65, Location = 024, }, // Virizion @ Route 11
            new EncounterStatic5 { Species = 643, Level = 70, Location = 039, Shiny = Shiny.Never, Version = GameVersion.W2, }, // Reshiram @ Dragonspiral Tower
            new EncounterStatic5 { Species = 644, Level = 70, Location = 039, Shiny = Shiny.Never, Version = GameVersion.B2, }, // Zekrom @ Dragonspiral Tower
            new EncounterStatic5 { Species = 646, Level = 70, Location = 061, Form = 0 }, // Kyurem @ Giant Chasm

            // N's Pokemon
            new EncounterStatic5N(0xFF01007F) { Species = 509, Level = 07, Location = 015, Ability = 2, Nature = Nature.Timid }, // Purloin @ Route 2
            new EncounterStatic5N(0xFF01007F) { Species = 519, Level = 13, Location = 033, Ability = 2, Nature = Nature.Sassy }, // Pidove @ Pinwheel Forest
            new EncounterStatic5N(0xFF00003F) { Species = 532, Level = 13, Location = 033, Ability = 1, Nature = Nature.Rash }, // Timburr @ Pinwheel Forest
            new EncounterStatic5N(0xFF01007F) { Species = 535, Level = 13, Location = 033, Ability = 2, Nature = Nature.Modest }, // Tympole @ Pinwheel Forest
            new EncounterStatic5N(0xFF00007F) { Species = 527, Level = 55, Location = 053, Ability = 1, Nature = Nature.Timid }, // Woobat @ Wellspring Cave
            new EncounterStatic5N(0xFF01007F) { Species = 551, Level = 22, Location = 034, Ability = 2, Nature = Nature.Docile }, // Sandile @ Desert Resort
            new EncounterStatic5N(0xFF00007F) { Species = 554, Level = 22, Location = 034, Ability = 1, Nature = Nature.Naive }, // Darumaka @ Desert Resort
            new EncounterStatic5N(0xFF00007F) { Species = 555, Level = 35, Location = 034, Ability = 4, Nature = Nature.Calm }, // Darmanitan @ Desert Resort
            new EncounterStatic5N(0xFF00007F) { Species = 559, Level = 22, Location = 034, Ability = 1, Nature = Nature.Lax }, // Scraggy @ Desert Resort
            new EncounterStatic5N(0xFF01007F) { Species = 561, Level = 22, Location = 034, Ability = 2, Nature = Nature.Gentle }, // Sigilyph @ Desert Resort
            new EncounterStatic5N(0xFF00007F) { Species = 525, Level = 28, Location = 037, Ability = 1, Nature = Nature.Naive }, // Boldore @ Chargestone Cave
            new EncounterStatic5N(0xFF01007F) { Species = 595, Level = 28, Location = 037, Ability = 2, Nature = Nature.Docile }, // Joltik @ Chargestone Cave
            new EncounterStatic5N(0xFF00007F) { Species = 597, Level = 28, Location = 037, Ability = 1, Nature = Nature.Bashful }, // Ferroseed @ Chargestone Cave
            new EncounterStatic5N(0xFF000000) { Species = 599, Level = 28, Location = 037, Ability = 1, Nature = Nature.Rash }, // Klink @ Chargestone Cave
            new EncounterStatic5N(0xFF00001F) { Species = 570, Level = 25, Location = 010, Ability = 1, Nature = Nature.Hasty, Gift = true } // N's Zorua @ Driftveil City
        };

        private static readonly EncounterStatic5[] Encounter_B2W2 = ArrayUtil.ConcatAll(Encounter_B2W2_Regular, Encounter_DreamRadar);

        #endregion
        #region Trade Tables

        internal static readonly EncounterTrade5PID[] TradeGift_BW =
        {
            new EncounterTrade5PID(0x64000000) { Species = 548, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest, Version = GameVersion.B, }, // Petilil
            new EncounterTrade5PID(0x6400007E) { Species = 546, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest, Version = GameVersion.W, }, // Cottonee
            new EncounterTrade5PID(0x9400007F) { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Version = GameVersion.B, Form = 0, }, // Basculin-Red
            new EncounterTrade5PID(0x9400007F) { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Version = GameVersion.W, Form = 1, }, // Basculin-Blue
            new EncounterTrade5PID(0xD400007F) { Species = 587, Level = 30, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,20,31,20,20,20}, Nature = Nature.Lax, }, // Emolga
            new EncounterTrade5PID(0x2A000000) { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Gentle, }, // Rotom
            new EncounterTrade5PID(0x6200001F) { Species = 446, Level = 60, Ability = 2, TID = 40217, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Serious, }, // Munchlax
        };

        internal static readonly EncounterTrade5[] TradeGift_B2W2_Regular =
        {
            new EncounterTrade5 { Species = 548, Level = 20, Ability = 2, TID = 65217, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Timid, Version = GameVersion.B2, }, // Petilil
            new EncounterTrade5 { Species = 546, Level = 20, Ability = 1, TID = 05720, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest, Version = GameVersion.W2, }, // Cottonee
            new EncounterTrade5 { Species = 526, Level = 35, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, IsNicknamed = false }, // Gigalith
            new EncounterTrade5 { Species = 465, Level = 45, Ability = 1, TID = 27658, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Hardy, }, // Tangrowth
            new EncounterTrade5 { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Calm, }, // Rotom
            new EncounterTrade5 { Species = 424, Level = 40, Ability = 2, TID = 17074, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Jolly, }, // Ambipom
            new EncounterTrade5 { Species = 065, Level = 40, Ability = 1, TID = 17074, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Timid, }, // Alakazam
        };

        internal const int YancyTID = 10303;
        internal const int CurtisTID = 54118;

        internal static readonly EncounterTrade5[] TradeGift_B2W2_YancyCurtis =
        {
            // Player is Male
            new EncounterTrade5 { Species = 052, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Meowth
            new EncounterTrade5 { Species = 202, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Wobbuffet
            new EncounterTrade5 { Species = 280, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Ralts
            new EncounterTrade5 { Species = 410, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Shieldon
            new EncounterTrade5 { Species = 111, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Rhyhorn
            new EncounterTrade5 { Species = 422, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, Form = 0, }, // Shellos-West
            new EncounterTrade5 { Species = 303, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Mawile
            new EncounterTrade5 { Species = 442, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Spiritomb
            new EncounterTrade5 { Species = 143, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Snorlax
            new EncounterTrade5 { Species = 216, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Teddiursa
            new EncounterTrade5 { Species = 327, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Spinda
            new EncounterTrade5 { Species = 175, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, }, // Togepi

            // Player is Female
            new EncounterTrade5 { Species = 056, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Mankey
            new EncounterTrade5 { Species = 202, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Wobbuffet
            new EncounterTrade5 { Species = 280, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Ralts
            new EncounterTrade5 { Species = 408, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Cranidos
            new EncounterTrade5 { Species = 111, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Rhyhorn
            new EncounterTrade5 { Species = 422, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, Form = 1, }, // Shellos-East
            new EncounterTrade5 { Species = 302, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Sableye
            new EncounterTrade5 { Species = 442, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Spiritomb
            new EncounterTrade5 { Species = 143, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Snorlax
            new EncounterTrade5 { Species = 231, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Phanpy
            new EncounterTrade5 { Species = 327, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Spinda
            new EncounterTrade5 { Species = 175, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, }, // Togepi
        };

        private const string tradeBW = "tradebw";
        private const string tradeB2W2 = "tradeb2w2";
        private static readonly string[][] TradeBW = Util.GetLanguageStrings8(tradeBW);
        private static readonly string[][] TradeB2W2 = Util.GetLanguageStrings8(tradeB2W2);
        private static readonly string[] TradeOT_B2W2_F = {string.Empty, "ルリ", "Yancy", "Brenda", "Lilì", "Sabine", string.Empty, "Belinda", "루리"};
        private static readonly string[] TradeOT_B2W2_M = {string.Empty, "テツ", "Curtis", "Julien", "Dadi", "Markus", string.Empty, "Julián", "철권"};

        internal static readonly EncounterTrade[] TradeGift_B2W2 = ArrayUtil.ConcatAll(TradeGift_B2W2_Regular, TradeGift_B2W2_YancyCurtis);

        #endregion

        internal static readonly EncounterStatic5[] StaticB = ArrayUtil.ConcatAll(GetEncounters(Encounter_BW, GameVersion.B), DreamWorld_Common, DreamWorld_BW);
        internal static readonly EncounterStatic5[] StaticW = ArrayUtil.ConcatAll(GetEncounters(Encounter_BW, GameVersion.W), DreamWorld_Common, DreamWorld_BW);
        internal static readonly EncounterStatic5[] StaticB2 = ArrayUtil.ConcatAll(GetEncounters(Encounter_B2W2, GameVersion.B2), DreamWorld_Common, DreamWorld_B2W2);
        internal static readonly EncounterStatic5[] StaticW2 = ArrayUtil.ConcatAll(GetEncounters(Encounter_B2W2, GameVersion.W2), DreamWorld_Common, DreamWorld_B2W2);
    }
}
