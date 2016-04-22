using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace PKHeX
{
    public class PKX
    {
        // C# PKX Function Library
        // No WinForm object related code, only to calculate information.
        // May require re-referencing to main form for string array referencing.
        // Relies on Util for some common operations.

        // Data
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
        #region ExpTable
        internal static readonly uint[,] ExpTable =
        {
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {8, 15, 4, 9, 6, 10},
            {27, 52, 13, 57, 21, 33},
            {64, 122, 32, 96, 51, 80},
            {125, 237, 65, 135, 100, 156},
            {216, 406, 112, 179, 172, 270},
            {343, 637, 178, 236, 274, 428},
            {512, 942, 276, 314, 409, 640},
            {729, 1326, 393, 419, 583, 911},
            {1000, 1800, 540, 560, 800, 1250},
            {1331, 2369, 745, 742, 1064, 1663},
            {1728, 3041, 967, 973, 1382, 2160},
            {2197, 3822, 1230, 1261, 1757, 2746},
            {2744, 4719, 1591, 1612, 2195, 3430},
            {3375, 5737, 1957, 2035, 2700, 4218},
            {4096, 6881, 2457, 2535, 3276, 5120},
            {4913, 8155, 3046, 3120, 3930, 6141},
            {5832, 9564, 3732, 3798, 4665, 7290},
            {6859, 11111, 4526, 4575, 5487, 8573},
            {8000, 12800, 5440, 5460, 6400, 10000},
            {9261, 14632, 6482, 6458, 7408, 11576},
            {10648, 16610, 7666, 7577, 8518, 13310},
            {12167, 18737, 9003, 8825, 9733, 15208},
            {13824, 21012, 10506, 10208, 11059, 17280},
            {15625, 23437, 12187, 11735, 12500, 19531},
            {17576, 26012, 14060, 13411, 14060, 21970},
            {19683, 28737, 16140, 15244, 15746, 24603},
            {21952, 31610, 18439, 17242, 17561, 27440},
            {24389, 34632, 20974, 19411, 19511, 30486},
            {27000, 37800, 23760, 21760, 21600, 33750},
            {29791, 41111, 26811, 24294, 23832, 37238},
            {32768, 44564, 30146, 27021, 26214, 40960},
            {35937, 48155, 33780, 29949, 28749, 44921},
            {39304, 51881, 37731, 33084, 31443, 49130},
            {42875, 55737, 42017, 36435, 34300, 53593},
            {46656, 59719, 46656, 40007, 37324, 58320},
            {50653, 63822, 50653, 43808, 40522, 63316},
            {54872, 68041, 55969, 47846, 43897, 68590},
            {59319, 72369, 60505, 52127, 47455, 74148},
            {64000, 76800, 66560, 56660, 51200, 80000},
            {68921, 81326, 71677, 61450, 55136, 86151},
            {74088, 85942, 78533, 66505, 59270, 92610},
            {79507, 90637, 84277, 71833, 63605, 99383},
            {85184, 95406, 91998, 77440, 68147, 106480},
            {91125, 100237, 98415, 83335, 72900, 113906},
            {97336, 105122, 107069, 89523, 77868, 121670},
            {103823, 110052, 114205, 96012, 83058, 129778},
            {110592, 115015, 123863, 102810, 88473, 138240},
            {117649, 120001, 131766, 109923, 94119, 147061},
            {125000, 125000, 142500, 117360, 100000, 156250},
            {132651, 131324, 151222, 125126, 106120, 165813},
            {140608, 137795, 163105, 133229, 112486, 175760},
            {148877, 144410, 172697, 141677, 119101, 186096},
            {157464, 151165, 185807, 150476, 125971, 196830},
            {166375, 158056, 196322, 159635, 133100, 207968},
            {175616, 165079, 210739, 169159, 140492, 219520},
            {185193, 172229, 222231, 179056, 148154, 231491},
            {195112, 179503, 238036, 189334, 156089, 243890},
            {205379, 186894, 250562, 199999, 164303, 256723},
            {216000, 194400, 267840, 211060, 172800, 270000},
            {226981, 202013, 281456, 222522, 181584, 283726},
            {238328, 209728, 300293, 234393, 190662, 297910},
            {250047, 217540, 315059, 246681, 200037, 312558},
            {262144, 225443, 335544, 259392, 209715, 327680},
            {274625, 233431, 351520, 272535, 219700, 343281},
            {287496, 241496, 373744, 286115, 229996, 359370},
            {300763, 249633, 390991, 300140, 240610, 375953},
            {314432, 257834, 415050, 314618, 251545, 393040},
            {328509, 267406, 433631, 329555, 262807, 410636},
            {343000, 276458, 459620, 344960, 274400, 428750},
            {357911, 286328, 479600, 360838, 286328, 447388},
            {373248, 296358, 507617, 377197, 298598, 466560},
            {389017, 305767, 529063, 394045, 311213, 486271},
            {405224, 316074, 559209, 411388, 324179, 506530},
            {421875, 326531, 582187, 429235, 337500, 527343},
            {438976, 336255, 614566, 447591, 351180, 548720},
            {456533, 346965, 639146, 466464, 365226, 570666},
            {474552, 357812, 673863, 485862, 379641, 593190},
            {493039, 367807, 700115, 505791, 394431, 616298},
            {512000, 378880, 737280, 526260, 409600, 640000},
            {531441, 390077, 765275, 547274, 425152, 664301},
            {551368, 400293, 804997, 568841, 441094, 689210},
            {571787, 411686, 834809, 590969, 457429, 714733},
            {592704, 423190, 877201, 613664, 474163, 740880},
            {614125, 433572, 908905, 636935, 491300, 767656},
            {636056, 445239, 954084, 660787, 508844, 795070},
            {658503, 457001, 987754, 685228, 526802, 823128},
            {681472, 467489, 1035837, 710266, 545177, 851840},
            {704969, 479378, 1071552, 735907, 563975, 881211},
            {729000, 491346, 1122660, 762160, 583200, 911250},
            {753571, 501878, 1160499, 789030, 602856, 941963},
            {778688, 513934, 1214753, 816525, 622950, 973360},
            {804357, 526049, 1254796, 844653, 643485, 1005446},
            {830584, 536557, 1312322, 873420, 664467, 1038230},
            {857375, 548720, 1354652, 902835, 685900, 1071718},
            {884736, 560922, 1415577, 932903, 707788, 1105920},
            {912673, 571333, 1460276, 963632, 730138, 1140841},
            {941192, 583539, 1524731, 995030, 752953, 1176490},
            {970299, 591882, 1571884, 1027103, 776239, 1212873},
            {1000000, 600000, 1640000, 1059860, 800000, 1250000},
        };
        #endregion

        internal static readonly string[][] SpeciesLang = 
        {
            Util.getStringList("species", "ja"), // none
            Util.getStringList("species", "ja"), // 1
            Util.getStringList("species", "en"), // 2
            Util.getStringList("species", "fr"), // 3
            Util.getStringList("species", "it"), // 4
            Util.getStringList("species", "de"), // 5
            Util.getStringList("species", "es"), // none
            Util.getStringList("species", "es"), // 7
            Util.getStringList("species", "ko"), // 8
        };

        internal static string getSpeciesName(int species, int lang)
        {
            try { return SpeciesLang[lang][species]; }
            catch { return ""; }
        }
        internal static readonly PersonalInfo[] Personal = Legal.PersonalAO;

        // Stat Fetching
        internal static int getMovePP(int move, int ppup)
        {
            return getBasePP(move) * (5 + ppup) / 5;
        }
        internal static int getBasePP(int move)
        {
            byte[] movepptable = 
            {
	            00, 
	            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 20, 30, 35, 35, 20, 15, 20, 
	            20, 25, 20, 30, 05, 10, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 10, 15, 30, 35, 
	            20, 20, 30, 25, 40, 20, 15, 20, 20, 20, 30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 
	            20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 25, 15, 10, 20, 25, 10, 35, 30, 15, 10, 
	            40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20, 
	            15, 10, 40, 15, 10, 30, 10, 20, 10, 40, 40, 20, 30, 30, 20, 30, 10, 10, 20, 05, 
	            10, 30, 20, 20, 20, 05, 15, 10, 20, 10, 15, 35, 20, 15, 10, 10, 30, 15, 40, 20, 
	            15, 10, 05, 10, 30, 10, 15, 20, 15, 40, 20, 10, 05, 15, 10, 10, 10, 15, 30, 30, 
	            10, 10, 20, 10, 01, 01, 10, 25, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 
	            25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 10, 
	            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 
	            05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 
	            05, 15, 20, 10, 05, 05, 15, 10, 15, 15, 10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 
	            15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 
	            10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15, 
	            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 15, 05, 40, 15, 
	            20, 20, 05, 15, 20, 20, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 
	            15, 25, 25, 15, 20, 15, 20, 15, 20, 10, 20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 
	            10, 10, 15, 10, 20, 15, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 
	            30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15, 
	            10, 15, 15, 15, 10, 10, 10, 20, 10, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 
	            15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 
	            05, 15, 20, 05, 20, 20, 20, 20, 10, 20, 10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 
	            10, 05, 05, 10, 05, 05, 05, 15, 10, 10, 10, 10, 10, 10, 15, 20, 15, 10, 15, 10, 
	            15, 10, 20, 10, 15, 10, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 15, 20, 15, 10, 
	            15, 15, 15, 15, 10, 10, 10, 10, 10, 15, 15, 15, 15, 05, 05, 15, 05, 10, 10, 10, 
	            20, 20, 20, 10, 10, 30, 15, 15, 10, 15, 25, 10, 15, 10, 10, 10, 20, 10, 10, 10, 
	            10, 10, 15, 15, 05, 05, 10, 10, 10, 05, 05, 10, 05, 05, 15, 10, 05, 05, 05, 10, 
	            10, 10, 10, 20, 25, 10, 20, 30, 25, 20, 20, 15, 20, 15, 20, 20, 10, 10, 10, 10, 
	            10, 20, 10, 30, 15, 10, 10, 10, 20, 20, 05, 05, 05, 20, 10, 10, 20, 15, 20, 20, 
	            10, 20, 30, 10, 10, 40, 40, 30, 20, 40, 20, 20, 10, 10, 10, 10, 05, 10, 10, 05, 
	            05 
            };
            if (move < 0) move = 0;
            return movepptable[move];
        }
        internal static byte[] getRandomEVs()
        {
            byte[] evs = new byte[6];
            do {
                evs[0] = (byte)Math.Min(Util.rnd32() % 300, 252); // bias two to get maybe 252
                evs[1] = (byte)Math.Min(Util.rnd32() % 300, 252);
                evs[2] = (byte)Math.Min(Util.rnd32() % (510 - evs[0] - evs[1]), 252);
                evs[3] = (byte)Math.Min(Util.rnd32() % (510 - evs[0] - evs[1] - evs[2]), 252);
                evs[4] = (byte)Math.Min(Util.rnd32() % (510 - evs[0] - evs[1] - evs[2] - evs[3]), 252);
                evs[5] = (byte)Math.Min(510 - evs[0] - evs[1] - evs[2] - evs[3] - evs[4], 252);
            } while (evs.Sum(b => b) > 510); // recalculate random EVs...
            Util.Shuffle(evs);
            return evs;
        }
        internal static byte getBaseFriendship(int species)
        {
            return Personal[species].BaseFriendship;
        }
        internal static int getLevel(int species, uint exp)
        {
            int growth = Personal[species].EXPGrowth;
            int tl = 1; // Initial Level. Iterate upwards to find the level
            while (ExpTable[++tl, growth] <= exp)
                if (tl == 100) return 100;
            return --tl;
        }
        internal static bool getIsShiny(uint PID, uint TID, uint SID)
        {
            uint PSV = getPSV(PID);
            uint TSV = getTSV(TID, SID);
            return TSV == PSV;
        }
        internal static uint getEXP(int level, int species)
        {
            if (level <= 1) return 0;
            if (level > 100) level = 100;
            return ExpTable[level, Personal[species].EXPGrowth];
        }
        internal static byte[] getAbilities(int species, int formnum)
        {
            return Personal[Personal[species].FormeIndex(species, formnum)].Abilities;
        }
        internal static int getAbilityNumber(int species, int ability, int formnum)
        {
            byte[] spec_abilities = Personal[Personal[species].FormeIndex(species, formnum)].Abilities;
            int abilval = Array.IndexOf(spec_abilities, (byte)ability);
            if (abilval >= 0)
                return 1 << abilval;
            return -1;
        }
        internal static int getGender(string s)
        {
            if (s == null) 
                return -1;
            if (s == "♂" || s == "M")
                return 0;
            if (s == "♀" || s == "F")
                return 1;
            return 2;
        }

        internal static string[] getCountryRegionText(int country, int region, string lang)
        {
            // Get Language we're fetching for
            int index = Array.IndexOf(new[] { "ja", "en", "fr", "de", "it", "es", "zh", "ko"}, lang);
            // Return value storage
            string[] data = new string[2]; // country, region

            // Get Country Text
            try
            {
                string[] inputCSV = Util.getStringList("countries");
                // Set up our Temporary Storage
                string[] unsortedList = new string[inputCSV.Length - 1];
                int[] indexes = new int[inputCSV.Length - 1];

                // Gather our data from the input file
                for (int i = 1; i < inputCSV.Length; i++)
                {
                    string[] countryData = inputCSV[i].Split(',');
                    if (countryData.Length <= 1) continue;
                    indexes[i - 1] = Convert.ToInt32(countryData[0]);
                    unsortedList[i - 1] = countryData[index + 1];
                }

                int countrynum = Array.IndexOf(indexes, country);
                data[0] = unsortedList[countrynum];
            }
            catch { data[0] = "Illegal"; }

            // Get Region Text
            try
            {
                string[] inputCSV = Util.getStringList("sr_" + country.ToString("000"));
                // Set up our Temporary Storage
                string[] unsortedList = new string[inputCSV.Length - 1];
                int[] indexes = new int[inputCSV.Length - 1];

                // Gather our data from the input file
                for (int i = 1; i < inputCSV.Length; i++)
                {
                    string[] countryData = inputCSV[i].Split(',');
                    if (countryData.Length <= 1) continue;
                    indexes[i - 1] = Convert.ToInt32(countryData[0]);
                    unsortedList[i - 1] = countryData[index + 1];
                }

                int regionnum = Array.IndexOf(indexes, region);
                data[1] = unsortedList[regionnum];
            }
            catch { data[1] = "Illegal"; }
            return data;
        }
        internal static string getLocation(bool eggmet, int gameorigin, int locval)
        {
            if (gameorigin < 13 && gameorigin > 6 && eggmet)
            {
                if (locval < 2000) return Main.metHGSS_00000[locval];
                if (locval < 3000) return Main.metHGSS_02000[locval % 2000];
                                   return Main.metHGSS_03000[locval % 3000];
            }
            if (gameorigin < 24)
            {
                if (locval < 30000) return Main.metBW2_00000[locval];
                if (locval < 40000) return Main.metBW2_30000[locval % 10000 - 1];
                if (locval < 60000) return Main.metBW2_40000[locval % 10000 - 1];
                                    return Main.metBW2_60000[locval % 10000 - 1];
            }
            if (gameorigin > 23)
            {
                if (locval < 30000) return Main.metXY_00000[locval];
                if (locval < 40000) return Main.metXY_30000[locval % 10000 - 1];
                if (locval < 60000) return Main.metXY_40000[locval % 10000 - 1];
                                    return Main.metXY_60000[locval % 10000 - 1];
            }
            return null; // Shouldn't happen.
        }
        internal static string[] getQRText(PK6 pk6)
        {
            string[] response = new string[3];
            // Summarize
            string filename = pk6.Nickname;
            if (pk6.Nickname != Main.specieslist[pk6.Species] && Main.specieslist[pk6.Species] != null)
                filename += $" ({Main.specieslist[pk6.Species]})";
            response[0] = $"{filename} [{Main.abilitylist[pk6.Ability]}] lv{pk6.Stat_Level} @ {Main.itemlist[pk6.HeldItem]} -- {Main.natures[pk6.Nature]}";
            response[1] = $"{Main.movelist[pk6.Move1]} / {Main.movelist[pk6.Move2]} / {Main.movelist[pk6.Move3]} / {Main.movelist[pk6.Move4]}";
            response[2] = string.Format(
                "IVs:{0}{1}{2}{3}{4}{5}"
                + Environment.NewLine + Environment.NewLine +
                "EVs:{6}{7}{8}{9}{10}{11}",
                Environment.NewLine + pk6.IV_HP.ToString("00"),
                Environment.NewLine + pk6.IV_ATK.ToString("00"),
                Environment.NewLine + pk6.IV_DEF.ToString("00"),
                Environment.NewLine + pk6.IV_SPA.ToString("00"),
                Environment.NewLine + pk6.IV_SPD.ToString("00"),
                Environment.NewLine + pk6.IV_SPE.ToString("00"),
                Environment.NewLine + pk6.EV_HP,
                Environment.NewLine + pk6.EV_ATK,
                Environment.NewLine + pk6.EV_DEF,
                Environment.NewLine + pk6.EV_SPA,
                Environment.NewLine + pk6.EV_SPD,
                Environment.NewLine + pk6.EV_SPE);

            return response;
        }
        internal static string getFileName(PK6 pk6)
        {
            return
                $"{pk6.Species.ToString("000")}{(pk6.IsShiny ? " ★" : "")} - {pk6.Nickname} - {pk6.Checksum.ToString("X4")}{pk6.EncryptionConstant.ToString("X8")}.pk6";
        }
        internal static ushort[] getStats(PK6 pk6)
        {
            return getStats(pk6.Species, pk6.Stat_Level, pk6.Nature, pk6.AltForm,
                pk6.EV_HP, pk6.EV_ATK, pk6.EV_DEF, pk6.EV_SPA, pk6.EV_SPD, pk6.EV_SPE,
                pk6.IV_HP, pk6.IV_ATK, pk6.IV_DEF, pk6.IV_SPA, pk6.IV_SPD, pk6.IV_SPE);
        }
        internal static ushort[] getStats(int species, int level, int nature, int form,
                                        int HP_EV, int ATK_EV, int DEF_EV, int SPA_EV, int SPD_EV, int SPE_EV,
                                        int HP_IV, int ATK_IV, int DEF_IV, int SPA_IV, int SPD_IV, int SPE_IV)
        {
            PersonalInfo p = Personal[Personal[species].FormeIndex(species, form)];
            // Calculate Stats
            ushort[] stats = new ushort[6]; // Stats are stored as ushorts in the PKX structure. We'll cap them as such.
            stats[0] = (ushort)(p.HP == 1 ? 1 : (HP_IV + 2 * p.HP + HP_EV / 4 + 100) * level / 100 + 10);
            stats[1] = (ushort)((ATK_IV + 2 * p.ATK + ATK_EV / 4) * level / 100 + 5);
            stats[2] = (ushort)((DEF_IV + 2 * p.DEF + DEF_EV / 4) * level / 100 + 5);
            stats[4] = (ushort)((SPA_IV + 2 * p.SPA + SPA_EV / 4) * level / 100 + 5);
            stats[5] = (ushort)((SPD_IV + 2 * p.SPD + SPD_EV / 4) * level / 100 + 5);
            stats[3] = (ushort)((SPE_IV + 2 * p.SPE + SPE_EV / 4) * level / 100 + 5);

            // Account for nature
            int incr = nature / 5 + 1;
            int decr = nature % 5 + 1;
            if (incr == decr) return stats; // if neutral return stats without mod
            stats[incr] *= 11; stats[incr] /= 10;
            stats[decr] *= 9; stats[decr] /= 10;

            // Return Result
            return stats;
        }


        // PKX Manipulation
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
            byte[] sdata = new byte[260];
            Array.Copy(data, sdata, 8); // Copy unshuffled bytes

            // Shuffle Away!
            for (int block = 0; block < 4; block++)
                Array.Copy(data, 8 + 56*blockPosition[block][sv], sdata, 8 + 56*block, 56);

            // Fill the Battle Stats back
            if (data.Length > 232)
                Array.Copy(data, 232, sdata, 232, 28);

            return sdata;
        }
        internal static byte[] decryptArray(byte[] ekx)
        {
            byte[] pkx = (byte[])ekx.Clone();

            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (pv >> 0xD & 0x1F) % 24;

            uint seed = pv;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkx, i);

            // Deshuffle
            pkx = shuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            if (pkx.Length <= 232) return pkx;
            for (int i = 232; i < 260; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkx, i);

            return pkx;
        }
        internal static byte[] encryptArray(byte[] pkx)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (pv >> 0xD & 0x1F) % 24;

            byte[] ekx = (byte[])pkx.Clone();

            ekx = shuffleArray(ekx, blockPositionInvert[sv]);

            uint seed = pv;

            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekx, i) ^ LCRNG(ref seed) >> 16)).CopyTo(ekx, i);

            // If no party stats, return.
            if (ekx.Length <= 232) return ekx;

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekx, i) ^ LCRNG(ref seed) >> 16)).CopyTo(ekx, i);

            // Done
            return ekx;
        }
        internal static ushort getCHK(byte[] data)
        {
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(data, i);
            
            return chk;
        }
        internal static bool verifychk(byte[] input)
        {
            ushort checksum = 0;
            if (input.Length == 100 || input.Length == 80) // Gen 3 Files
            {
                for (int i = 32; i < 80; i += 2)
                    checksum += BitConverter.ToUInt16(input, i);

                return checksum == BitConverter.ToUInt16(input, 28);
            }

            if (input.Length == 236 || input.Length == 220 || input.Length == 136) // Gen 4/5
                Array.Resize(ref input, 136);
            else if (input.Length == 232 || input.Length == 260) // Gen 6
                Array.Resize(ref input, 232);
            else throw new ArgumentException("Wrong sized input array to verifychecksum");

            ushort chk = 0;
            for (int i = 8; i < input.Length; i += 2)
                chk += BitConverter.ToUInt16(input, i);

            return chk == BitConverter.ToUInt16(input, 0x6);
        }

        internal static uint getPSV(uint PID)
        {
            return (PID >> 16 ^ PID & 0xFFFF) >> 4;
        }
        internal static uint getTSV(uint TID, uint SID)
        {
            return (TID ^ SID) >> 4;
        }
        internal static uint getRandomPID(int species, int cg, int origin, int nature, int form)
        {
            int gt = Personal[species].Gender;
            if (origin >= 24)
                return Util.rnd32();

            bool g3unown = origin <= 5 && species == 201;
            while (true) // Loop until we find a suitable PID
            {
                uint pid = Util.rnd32();

                // Gen 3/4: Nature derived from PID
                if (origin <= 15 && pid%25 != nature)
                    continue;

                // Gen 3 Unown: Letter/form derived from PID
                if (g3unown)
                {
                    uint pidLetter = ((pid & 0x3000000) >> 18 | (pid & 0x30000) >> 12 | (pid & 0x300) >> 6 | pid & 0x3) % 28;
                    if (pidLetter != form)
                        continue;
                }

                // Gen 3/4/5: Gender derived from PID
                uint gv = pid & 0xFF;
                if (gt == 255 || gt == 254 || gt == 0) // Set Gender(less)
                    return pid; // PID can be anything
                if (cg == 1 && gv <= gt) // Female
                    return pid; // PID Passes
                if (cg == 0 && gv > gt) // Male
                    return pid; // PID Passes
            }
        }

        // SAV Manipulation
        /// <summary>Calculates the CRC16-CCITT checksum over an input byte array.</summary>
        /// <param name="data">Input byte array</param>
        /// <returns>Checksum</returns>
        internal static ushort ccitt16(byte[] data)
        {
            const ushort init = 0xFFFF;
            const ushort poly = 0x1021;

            ushort crc = init;
            foreach (byte b in data)
            {
                crc ^= (ushort)(b << 8);
                for (int j = 0; j < 8; j++)
                {
                    bool flag = (crc & 0x8000) > 0;
                    crc <<= 1;
                    if (flag)
                        crc ^= poly;
                }
            }
            return crc;
        }
        /// <summary>Simple check to see if the save is valid.</summary>
        /// <param name="savefile">Input binary file</param>
        /// <returns>True/False</returns>
        internal static bool verifyG6SAV(byte[] savefile)
        {
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != SAV6.BEEF)
                verificationOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?

            int count = (savefile.Length - verificationOffset - 0x8) / 8;
            verificationOffset += 4;
            int[] Lengths = new int[count];
            ushort[] BlockIDs = new ushort[count];
            ushort[] Checksums = new ushort[count];
            int[] Start = new int[count];
            int CurrentPosition = 0;
            for (int i = 0; i < count; i++)
            {
                Start[i] = CurrentPosition;
                Lengths[i] = BitConverter.ToInt32(savefile, verificationOffset + 0 + 8 * i);
                BlockIDs[i] = BitConverter.ToUInt16(savefile, verificationOffset + 4 + 8 * i);
                Checksums[i] = BitConverter.ToUInt16(savefile, verificationOffset + 6 + 8 * i);

                CurrentPosition += Lengths[i] % 0x200 == 0 ? Lengths[i] : 0x200 - Lengths[i] % 0x200 + Lengths[i];

                if ((BlockIDs[i] != 0) || i == 0) continue;
                count = i;
                break;
            }
            // Verify checksums
            for (int i = 0; i < count; i++)
            {
                ushort chk = ccitt16(savefile.Skip(Start[i]).Take(Lengths[i]).ToArray());
                ushort old = BitConverter.ToUInt16(savefile, verificationOffset + 6 + i * 8);

                if (chk != old)
                    return false;
            }
            return true;
        }
        /// <summary>Verbose check to see if the save is valid.</summary>
        /// <param name="savefile">Input binary file</param>
        /// <returns>String containing invalid blocks.</returns>
        internal static string verifyG6CHK(byte[] savefile)
        {
            string rv = "";
            int invalid = 0;
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != SAV6.BEEF)
                verificationOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?

            int count = (savefile.Length - verificationOffset - 0x8) / 8;
            verificationOffset += 4;
            int[] Lengths = new int[count];
            ushort[] BlockIDs = new ushort[count];
            ushort[] Checksums = new ushort[count];
            int[] Start = new int[count];
            int CurrentPosition = 0;
            for (int i = 0; i < count; i++)
            {
                Start[i] = CurrentPosition;
                Lengths[i] = BitConverter.ToInt32(savefile, verificationOffset + 0 + 8 * i);
                BlockIDs[i] = BitConverter.ToUInt16(savefile, verificationOffset + 4 + 8 * i);
                Checksums[i] = BitConverter.ToUInt16(savefile, verificationOffset + 6 + 8 * i);

                CurrentPosition += Lengths[i] % 0x200 == 0 ? Lengths[i] : 0x200 - Lengths[i] % 0x200 + Lengths[i];

                if (BlockIDs[i] != 0 || i == 0) continue;
                count = i;
                break;
            }
            // Apply checksums
            for (int i = 0; i < count; i++)
            {
                ushort chk = ccitt16(savefile.Skip(Start[i]).Take(Lengths[i]).ToArray());
                ushort old = BitConverter.ToUInt16(savefile, verificationOffset + 6 + i * 8);

                if (chk == old) continue;

                invalid++;
                rv += $"Invalid: {i.ToString("X2")} @ Region {Start[i].ToString("X5") + Environment.NewLine}";
            }
            // Return Outputs
            rv += $"SAV: {count - invalid}/{count + Environment.NewLine}";
            return rv;
        }
        /// <summary>Fix checksums in the input save file.</summary>
        /// <param name="savefile">Input binary file</param>
        /// <returns>Fixed save file.</returns>
        internal static void writeG6CHK(byte[] savefile)
        {
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != SAV6.BEEF)
                verificationOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?

            int count = (savefile.Length - verificationOffset - 0x8) / 8;
            verificationOffset += 4;
            int[] Lengths = new int[count];
            ushort[] BlockIDs = new ushort[count];
            ushort[] Checksums = new ushort[count];
            int[] Start = new int[count];
            int CurrentPosition = 0;
            for (int i = 0; i < count; i++)
            {
                Start[i] = CurrentPosition;
                Lengths[i] = BitConverter.ToInt32(savefile, verificationOffset + 0 + 8 * i);
                BlockIDs[i] = BitConverter.ToUInt16(savefile, verificationOffset + 4 + 8 * i);
                Checksums[i] = BitConverter.ToUInt16(savefile, verificationOffset + 6 + 8 * i);

                CurrentPosition += Lengths[i]%0x200 == 0 ? Lengths[i] : 0x200 - Lengths[i]%0x200 + Lengths[i];

                if (BlockIDs[i] != 0 || i == 0) continue;
                count = i;
                break;
            }
            // Apply checksums
            for (int i = 0; i < count; i++)
            {
                byte[] array = savefile.Skip(Start[i]).Take(Lengths[i]).ToArray();
                BitConverter.GetBytes(ccitt16(array)).CopyTo(savefile, verificationOffset + 6 + i * 8);
            }
        }

        // Data Requests
        internal static Image getSprite(int species, int form, int gender, int item, bool isegg, bool shiny)
        {
            if (species == 0)
                return (Image)Properties.Resources.ResourceManager.GetObject("_0");
            if (new[] { 664, 665, 414, 493 }.Contains(species)) // Species who show their default sprite regardless of Form
                form = 0;

            string file = "_" + species;
            if (form > 0) // Alt Form Handling
                file = file + "_" + form;
            else if (gender == 1 && new[] { 592, 593, 521, 668 }.Contains(species)) // Frillish & Jellicent, Unfezant & Pyroar
                file = file + "_" + gender;

            // Redrawing logic
            Image baseImage = (Image)Properties.Resources.ResourceManager.GetObject(file);
            if (baseImage == null)
            {
                if (species < 722)
                {
                    baseImage = Util.LayerImage(
                        (Image)Properties.Resources.ResourceManager.GetObject("_" + species),
                        Properties.Resources.unknown,
                        0, 0, .5);
                }
                else
                    baseImage = Properties.Resources.unknown;
            }
            if (isegg)
            {
                // Start with a partially transparent species by layering the species with partial opacity onto a blank image.
                baseImage = Util.LayerImage((Image)Properties.Resources.ResourceManager.GetObject("_0"), baseImage, 0, 0, 0.33);
                // Add the egg layer over-top with full opacity.
                baseImage = Util.LayerImage(baseImage, (Image)Properties.Resources.ResourceManager.GetObject("egg"), 0, 0, 1);
            }
            if (shiny)
            {   
                // Add shiny star to top left of image.
                baseImage = Util.LayerImage(baseImage, Properties.Resources.rare_icon, 0, 0, 0.7);
            }
            if (item > 0)
            {
                Image itemimg = (Image)Properties.Resources.ResourceManager.GetObject("item_" + item) ?? Properties.Resources.helditem;
                // Redraw
                baseImage = Util.LayerImage(baseImage, itemimg, 22 + (15 - itemimg.Width) / 2, 15 + (15 - itemimg.Height), 1);
            }
            return baseImage;
        }
        internal static Image getSprite(PK6 pk6)
        {
            return getSprite(pk6.Species, pk6.AltForm, pk6.Gender, pk6.HeldItem, pk6.IsEgg, pk6.IsShiny);
        }
        internal static Image getSprite(byte[] data)
        {
            return new PK6(data).Sprite;
        }

        // Font Related
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        internal static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        internal static readonly PrivateFontCollection s_FontCollection = new PrivateFontCollection();
        internal static FontFamily[] FontFamilies
        {
            get
            {
                if (s_FontCollection.Families.Length == 0) setPKXFont();
                return s_FontCollection.Families;
            }
        }
        internal static Font getPKXFont(float size)
        {
            return new Font(FontFamilies[0], size);
        }
        internal static void setPKXFont()
        {
            try
            {
                byte[] fontData = Properties.Resources.pgldings_normalregular;
                IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                s_FontCollection.AddMemoryFont(fontPtr, Properties.Resources.pgldings_normalregular.Length); uint dummy = 0;
                AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.pgldings_normalregular.Length, IntPtr.Zero, ref dummy);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            }
            catch { Util.Error("Unable to add ingame font."); }
        }

        // Personal.dat
        internal static string[] getFormList(int species, string[] t, string[] f, string[] g)
        {
            // Mega List            
            if (Array.IndexOf(new[] 
                { // XY
                  003, 009, 065, 094, 115, 127, 130, 142, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359, 380, 381, 445, 448, 460, 
                  // ORAS
                  015, 018, 080, 208, 254, 260, 302, 319, 323, 334, 362, 373, 376, 384, 428, 475, 531, 719,
                }, species) > -1) { // ...
                    return new[]
                    {
                        t[000], // Normal
                        f[723], // Mega
                    };}
            // MegaXY List
            switch (species)
            {
                case 6:
                case 150:
                    return new[]
                    {
                        t[000], // Normal
                        f[724], // Mega X
                        f[725], // Mega Y
                    };
                case 025:
                    return new[]
                    {
                        t[000], // Normal
                        f[729], // Rockstar
                        f[730], // Belle
                        f[731], // Pop
                        f[732], // PhD
                        f[733], // Libre
                        f[734], // Cosplay
                    };
                case 201:
                    return new[]
                    {
                        "A", "B", "C", "D", "E",
                        "F", "G", "H", "I", "J",
                        "K", "L", "M", "N", "O",
                        "P", "Q", "R", "S", "T",
                        "U", "V", "W", "X", "Y",
                        "Z",
                        "!", "?",
                    };
                case 351:
                    return new[]
                    {
                        t[000], // Normal
                        f[789], // Sunny
                        f[790], // Rainy
                        f[791], // Snowy
                    };
                case 382:
                case 383:
                    return new[]
                    {
                        t[000], // Normal
                        f[800], // Primal
                    };
                case 386:
                    return new[]
                    {
                        t[000], // Normal
                        f[802], // Attack
                        f[803], // Defense
                        f[804], // Speed
                    };

                case 412:
                case 413:
                case 414:
                    return new[]
                    {
                        f[412], // Plant
                        f[805], // Sandy
                        f[806], // Trash
                    };

                case 421:
                    return new[]
                    {
                        f[421], // Overcast
                        f[809], // Sunshine
                    };

                case 422:
                case 423:
                    return new[]
                    {
                        f[422], // West
                        f[811], // East
                    };

                case 479:
                    return new[]
                    {
                        t[000], // Normal
                        f[817], // Heat
                        f[818], // Wash
                        f[819], // Frost
                        f[820], // Fan
                        f[821], // Mow
                    };

                case 487:
                    return new[]
                    {
                        f[487], // Altered
                        f[822], // Origin
                    };

                case 492:
                    return new[]
                    {
                        f[492], // Land
                        f[823], // Sky
                    };

                case 493:
                    return new[]
                    {
                        t[00], // Normal
                        t[01], // Fighting
                        t[02], // Flying
                        t[03], // Poison
                        t[04], // etc
                        t[05],
                        t[06],
                        t[07],
                        t[08],
                        t[09],
                        t[10],
                        t[11],
                        t[12],
                        t[13],
                        t[14],
                        t[15],
                        t[16],
                        t[17],
                    };

                case 550:
                    return new[]
                    {
                        f[550], // Red
                        f[842], // Blue
                    };

                case 555:
                    return new[]
                    {
                        f[555], // Standard
                        f[843], // Zen
                    };

                case 585:
                case 586:
                    return new[]
                    {
                        f[585], // Spring
                        f[844], // Summer
                        f[845], // Autumn
                        f[846], // Winter
                    };

                case 641:
                case 642:
                case 645:
                    return new[]
                    {
                        f[641], // Incarnate
                        f[852], // Therian
                    };

                case 646:
                    return new[]
                    {
                        t[000], // Normal
                        f[853], // White
                        f[854], // Black
                    };

                case 647:
                    return new[]
                    {
                        f[647], // Ordinary
                        f[855], // Resolute
                    };

                case 648:
                    return new[]
                    {
                        f[648], // Aria
                        f[856], // Pirouette
                    };

                case 649:
                    return new[]
                    {
                        t[000], // Normal
                        t[010], // Douse
                        t[012], // Shock
                        t[009], // Burn
                        t[014], // Chill
                    };

                case 664:
                case 665:
                case 666:
                    return new[]
                    {
                        f[666], // Icy Snow
                        f[861], // Polar
                        f[862], // Tundra
                        f[863], // Continental 
                        f[864], // Garden
                        f[865], // Elegant
                        f[866], // Meadow
                        f[867], // Modern 
                        f[868], // Marine
                        f[869], // Archipelago
                        f[870], // High-Plains
                        f[871], // Sandstorm
                        f[872], // River
                        f[873], // Monsoon
                        f[874], // Savannah 
                        f[875], // Sun
                        f[876], // Ocean
                        f[877], // Jungle
                        f[878], // Fancy
                        f[879], // Poké Ball
                    };

                case 669:
                case 671:
                    return new[]
                    {
                        f[669], // Red
                        f[884], // Yellow
                        f[885], // Orange
                        f[886], // Blue
                        f[887], // White
                    };

                case 670:
                    return new[]
                    {
                        f[669], // Red
                        f[884], // Yellow
                        f[885], // Orange
                        f[886], // Blue
                        f[887], // White
                        f[888], // Eternal
                    };

                case 676:
                    return new[]
                    {
                        f[676], // Natural
                        f[893], // Heart
                        f[894], // Star
                        f[895], // Diamond
                        f[896], // Deputante
                        f[897], // Matron
                        f[898], // Dandy
                        f[899], // La Reine
                        f[900], // Kabuki 
                        f[901], // Pharaoh
                    };

                case 678:
                    return new[]
                    {
                        g[000], // Male
                        g[001], // Female
                    };

                case 681:
                    return new[]
                    {
                        f[681], // Shield
                        f[903], // Blade
                    };

                case 710:
                case 711:
                    return new[]
                    {
                        f[904], // Small
                        f[710], // Average
                        f[905], // Large
                        f[906], // Super
                    };

                case 716:
                    return new[]
                    {
                        t[000], // Normal
                        f[910], // Active
                    };

                case 720:
                    return new[]
                    {
                        t[000], // Normal
                        f[912], // Unbound
                    };
            }
            return new[] {""};
        }
        internal static int getDexFormIndexXY(int species, int formct)
        {
            if (formct < 1 || species < 0)
                return -1; // invalid
            switch (species)
            {
                case 201: return 000; // 28 Unown
                case 386: return 028; // 4 Deoxys
                case 492: return 032; // 2 Shaymin
                case 487: return 034; // 2 Giratina
                case 479: return 036; // 6 Rotom
                case 422: return 042; // 2 Shellos
                case 423: return 044; // 2 Gastrodon
                case 412: return 046; // 3 Burmy
                case 413: return 049; // 3 Wormadam
                case 351: return 052; // 4 Castform
                case 421: return 056; // 2 Cherrim
                case 585: return 058; // 4 Deerling
                case 586: return 062; // 4 Sawsbuck
                case 648: return 066; // 2 Meloetta
                case 555: return 068; // 2 Darmanitan
                case 550: return 070; // 2 Basculin
                case 646: return 072; // 3 Kyurem
                case 647: return 075; // 2 Keldeo
                case 642: return 077; // 2 Thundurus
                case 641: return 079; // 2 Tornadus
                case 645: return 081; // 2 Landorus
                case 666: return 083; // 20 Vivillion
                case 669: return 103; // 5 Flabébé
                case 670: return 108; // 6 Floette
                case 671: return 114; // 5 Florges
                case 710: return 119; // 4 Pumpkaboo
                case 711: return 123; // 4 Gourgeist
                case 681: return 127; // 2 Aegislash
                case 716: return 129; // 2 Xerneas
                case 003: return 131; // 2 Venusaur
                case 006: return 133; // 3 Charizard
                case 009: return 136; // 2 Blastoise
                case 065: return 138; // 2 Alakazam
                case 094: return 140; // 2 Gengar
                case 115: return 142; // 2 Kangaskhan
                case 127: return 144; // 2 Pinsir
                case 130: return 146; // 2 Gyarados
                case 142: return 148; // 2 Aerodactyl
                case 150: return 150; // 3 Mewtwo
                case 181: return 153; // 2 Ampharos
                case 212: return 155; // 2 Scizor
                case 214: return 157; // 2 Heracros
                case 229: return 159; // 2 Houndoom
                case 248: return 161; // 2 Tyranitar
                case 257: return 163; // 2 Blaziken
                case 282: return 165; // 2 Gardevoir
                case 303: return 167; // 2 Mawile
                case 306: return 169; // 2 Aggron
                case 308: return 171; // 2 Medicham
                case 310: return 173; // 2 Manetric
                case 354: return 175; // 2 Banette
                case 359: return 177; // 2 Absol
                case 380: return 179; // 2 Latias
                case 381: return 181; // 2 Latios
                case 445: return 183; // 2 Garchomp
                case 448: return 185; // 2 Lucario
                case 460: return 187; // 2 Abomasnow
                default: return -1;
            }
        }
        internal static int getDexFormIndexORAS(int species, int formct)
        {
            if (formct < 1 || species < 0)
                return -1; // invalid
            switch (species)
            {
                case 025: return 189; // 7 Pikachu
                case 720: return 196; // 2 Hoopa
                case 015: return 198; // 2 Beedrill
                case 018: return 200; // 2 Pidgeot
                case 080: return 202; // 2 Slowbro
                case 208: return 204; // 2 Steelix
                case 254: return 206; // 2 Sceptile
                case 360: return 208; // 2 Swampert
                case 302: return 210; // 2 Sableye
                case 319: return 212; // 2 Sharpedo
                case 323: return 214; // 2 Camerupt
                case 334: return 216; // 2 Altaria
                case 362: return 218; // 2 Glalie
                case 373: return 220; // 2 Salamence
                case 376: return 222; // 2 Metagross
                case 384: return 224; // 2 Rayquaza
                case 428: return 226; // 2 Lopunny
                case 475: return 228; // 2 Gallade
                case 531: return 230; // 2 Audino
                case 719: return 232; // 2 Diancie
                case 382: return 234; // 2 Kyogre
                case 383: return 236; // 2 Groudon
                case 493: return 238; // 18 Arceus
                case 649: return 256; // 5 Genesect
                case 676: return 261; // 10 Furfrou
                default: return getDexFormIndexXY(species, formct);
            }
        }

        /// <summary>Calculate the Hidden Power Type of the entered IVs.</summary>
        /// <param name="ivs">Order: HP,ATK,DEF,SPEED,SPA,SPD</param>
        /// <returns>Hidden Power Type</returns>
        internal static int getHPType(int[] ivs)
        {
            return 15 * ((ivs[0] & 1) + 2 * (ivs[1] & 1) + 4 * (ivs[2] & 1) + 8 * (ivs[3] & 1) + 16 * (ivs[4] & 1) + 32 * (ivs[5] & 1)) / 63;
        }
        internal static int[] setHPIVs(int type, int[] ivs)
        {
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + hpivs[type, i];
            return ivs;
        }
        internal static readonly int[,] hpivs = {
            { 1, 1, 0, 0, 0, 0 }, // Fighting
            { 0, 0, 0, 0, 0, 1 }, // Flying
            { 1, 1, 0, 0, 0, 1 }, // Poison
            { 1, 1, 1, 0, 0, 1 }, // Ground
            { 1, 1, 0, 1, 0, 0 }, // Rock
            { 1, 0, 0, 1, 0, 1 }, // Bug
            { 1, 0, 1, 1, 0, 1 }, // Ghost
            { 1, 1, 1, 1, 0, 1 }, // Steel
            { 1, 0, 1, 0, 1, 0 }, // Fire
            { 1, 0, 0, 0, 1, 1 }, // Water
            { 1, 0, 1, 0, 1, 1 }, // Grass
            { 1, 1, 1, 0, 1, 1 }, // Electric
            { 1, 0, 1, 1, 1, 0 }, // Psychic
            { 1, 0, 0, 1, 1, 1 }, // Ice
            { 1, 0, 1, 1, 1, 1 }, // Dragon
            { 1, 1, 1, 1, 1, 1 }, // Dark
        };

        internal static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        public class ShowdownSet
        {
            // String to Values
            public static readonly string[] types = Util.getStringList("types", "en");
            public static readonly string[] forms = Util.getStringList("forms", "en");
            private static readonly string[] species = Util.getStringList("species", "en");
            private static readonly string[] items = Util.getStringList("items", "en");
            private static readonly string[] natures = Util.getStringList("natures", "en");
            private static readonly string[] moves = Util.getStringList("moves", "en");
            private static readonly string[] abilities = Util.getStringList("abilities", "en");
            private static readonly string[] hptypes = types.Skip(1).ToArray();

            // Default Set Data
            public string Nickname;
            public int Species;
            public string Form;
            public string Gender;
            public int Item;
            public int Ability;
            public int Level;
            public bool Shiny;
            public int Friendship;
            public int Nature;
            public int[] EVs;
            public int[] IVs;
            public int[] Moves;

            // Parsing Utility
            public ShowdownSet(string input = null)
            {
                if (input == null)
                    return;

                Nickname = null;
                Species = -1;
                Form = null;
                Gender = null;
                Item = 0;
                Ability = 0;
                Level = 100;
                Shiny = false;
                Friendship = 255;
                Nature = 0;
                EVs = new int[6];
                IVs = new[] { 31, 31, 31, 31, 31, 31 };
                Moves = new int[4];

                string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].Replace("'", "’").Trim(); // Sanitize apostrophes

                if (lines.Length < 3) return;

                // Seek for start of set
                int start = -1;
                for (int i = 0; i < lines.Length; i++)
                    if (lines[i].Contains(" @ ")) { start = i; break; }
                lines = lines.Skip(start).Take(lines.Length - start).ToArray();

                // Abort if no text is found
                if (start == -1)
                {
                    // Try to parse the first line if it does not have any item
                    string ld = lines[0];
                    // Gender Detection
                    string last3 = ld.Substring(ld.Length - 3);
                    if (last3 == "(M)" || last3 == "(F)")
                    {
                        Gender = last3.Substring(1, 1);
                        ld = ld.Substring(0, ld.Length - 3);
                    }
                    // Nickname Detection
                    string spec = ld;
                    if (spec.Contains("("))
                    {
                        int index = spec.LastIndexOf("(", StringComparison.Ordinal);
                        string n1 = spec.Substring(0, index - 1);
                        string n2 = spec.Substring(index).Replace("(", "").Replace(")", "").Replace(" ", "");

                        bool inverted = Array.IndexOf(species, n2.Replace(" ", "")) > -1 || (Species = Array.IndexOf(species, n2.Split('-')[0])) > 0;
                        spec = inverted ? n2 : n1;
                        Nickname = inverted ? n1 : n2;
                    }
                    Species = Array.IndexOf(species, spec.Replace(" ", ""));
                    if (
                        (Species = Array.IndexOf(species, spec)) < 0 // Not an Edge Case
                        &&
                        (Species = Array.IndexOf(species, spec.Replace(" ", ""))) < 0 // Has Form
                        ) 
                    {
                        string[] tmp = spec.Split(new[] { "-" }, StringSplitOptions.None);
                        if (tmp.Length < 2) return;
                        Species = Array.IndexOf(species, tmp[0].Replace(" ", ""));
                        Form = tmp[1].Replace(" ", "");
                        if (tmp.Length > 2)
                            Form += " " + tmp[2];
                    }
                    if (Species < -1)
                        return;
                    lines = lines.Skip(1).Take(lines.Length - 1).ToArray();
                }
                int movectr = 0;
                // Detect relevant data
                foreach (string line in lines)
                {
                    if (line.Length < 2) continue;
                    if (line.Contains("- "))
                    {
                        string moveString = line.Substring(2);
                        if (moveString.Contains("Hidden Power"))
                        {
                            if (moveString.Length > 13) // Defined Hidden Power
                            {
                                string type = moveString.Remove(0, 13).Replace("[", "").Replace("]", ""); // Trim out excess data
                                int hpVal = Array.IndexOf(hptypes, type); // Get HP Type
                                if (hpVal >= 0) IVs = setHPIVs(hpVal, IVs); // Get IVs
                            }
                            moveString = "Hidden Power";
                        }
                        Moves[movectr++] = Array.IndexOf(moves, moveString);
                        if (movectr == 4)
                            break; // End of moves
                        continue;
                    }

                    string[] brokenline = line.Split(new[] { ": " }, StringSplitOptions.None);
                    switch (brokenline[0])
                    {
                        case "Trait":
                        case "Ability": { Ability = Array.IndexOf(abilities, brokenline[1]); break; }
                        case "Level": { Level = Util.ToInt32(brokenline[1]); break; }
                        case "Shiny": { Shiny = brokenline[1] == "Yes"; break; }
                        case "Happiness": { Friendship = Util.ToInt32(brokenline[1]); break; }
                        case "EVs":
                            {
                                // Get EV list String
                                string[] evlist = brokenline[1].Replace("SAtk", "SpA").Replace("SDef", "SpD").Replace("Spd", "Spe").Split(new[] { " / ", " " }, StringSplitOptions.None);
                                for (int i = 0; i < evlist.Length / 2; i++)
                                    EVs[Array.IndexOf(StatNames, evlist[1 + i * 2])] = (byte)Util.ToInt32(evlist[0 + 2 * i]);
                                break;
                            }
                        case "IVs":
                            {
                                // Get IV list String
                                string[] ivlist = brokenline[1].Split(new[] { " / ", " " }, StringSplitOptions.None);
                                for (int i = 0; i < ivlist.Length / 2; i++)
                                    IVs[Array.IndexOf(StatNames, ivlist[1 + i * 2])] = (byte)Util.ToInt32(ivlist[0 + 2 * i]);
                                break;
                            }
                        default:
                            {
                                // Either Nature or Gender ItemSpecies
                                if (brokenline[0].Contains(" @ "))
                                {
                                    string[] ld = line.Split(new[] { " @ " }, StringSplitOptions.None);
                                    Item = Array.IndexOf(items, ld.Last());
                                    // Gender Detection
                                    string last3 = ld[0].Substring(ld[0].Length - 3);
                                    if (last3 == "(M)" || last3 == "(F)")
                                    {
                                        Gender = last3.Substring(1, 1);
                                        ld[0] = ld[0].Substring(0, ld[ld.Length - 2].Length - 3);
                                    }
                                    // Nickname Detection
                                    string spec = ld[0];
                                    if (spec.Contains("("))
                                    {
                                        int index = spec.LastIndexOf("(", StringComparison.Ordinal);
                                        string n1 = spec.Substring(0, index - 1);
                                        string n2 = spec.Substring(index).Replace("(", "").Replace(")", "").Replace(" ", "");

                                        bool inverted = Array.IndexOf(species, n2.Replace(" ", "")) > -1 || (Species = Array.IndexOf(species, n2.Split('-')[0])) > 0;
                                        spec = inverted ? n2 : n1;
                                        Nickname = inverted ? n1 : n2;
                                    }
                                    if (
                                        (Species = Array.IndexOf(species, spec)) < 0 // Not an Edge Case
                                        &&
                                        (Species = Array.IndexOf(species, spec.Replace(" ", ""))) < 0 // Has Form
                                        )
                                    {
                                        string[] tmp = spec.Split(new[] { "-" }, StringSplitOptions.None);
                                        Species = Array.IndexOf(species, tmp[0].Replace(" ", ""));
                                        Form = tmp[1].Replace(" ", "");
                                        if (tmp.Length > 2)
                                            Form += " " + tmp[2];
                                    }
                                }
                                else if (brokenline[0].Contains("Nature"))
                                    Nature = Array.IndexOf(natures, line.Split(' ')[0]);
                                else // Fallback
                                    Species = Array.IndexOf(species, line.Split('(')[0]);
                            } break;
                    }
                }
            }
            public string getText()
            {
                if (Species == 0 || Species > 722)
                    return "";

                // First Line: Name, Nickname, Gender, Item
                string result = string.Format(species[Species] != Nickname ? "{0} ({1})" : "{1}", Nickname,
                    species[Species] + ((Form ?? "") != "" ? "-" + Form.Replace("Mega ", "Mega-") : "")) // Species (& Form if necessary)
                                + Gender + (Item != 0 ? " @ " + items[Item] : "") + Environment.NewLine;

                // IVs
                string[] ivstr = new string[6];
                int ivctr = 0;
                int[] sIVs = { IVs[0], IVs[1], IVs[2], IVs[4], IVs[5], IVs[3] }; // Reorganize speed
                for (int i = 0; i < 6; i++)
                {
                    if (sIVs[i] == 31) continue;
                    ivstr[ivctr++] += $"{sIVs[i]} {StatNames[i]}";
                }
                if (ivctr > 0)
                result += "IVs: " + string.Join(" / ", ivstr.Take(ivctr)) + Environment.NewLine;

                // EVs
                string[] evstr = new string[6];
                int[] sEVs = { EVs[0], EVs[1], EVs[2], EVs[4], EVs[5], EVs[3] }; // Reorganize speed
                int evctr = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (sEVs[i] == 0) continue;
                    evstr[evctr++] += $"{sEVs[i]} {StatNames[i]}";
                }
                if (evctr > 0)
                    result += "EVs: " + string.Join(" / ", evstr.Take(evctr)) + Environment.NewLine;

                // Secondary Stats
                result += "Ability: " + abilities[Ability] + Environment.NewLine;
                result += "Level: " + Level + Environment.NewLine;
                if (Shiny)
                    result += "Shiny: Yes" + Environment.NewLine;

                result += natures[Nature] + " Nature" + Environment.NewLine;
                // Add in Moves
                string[] MoveLines = new string[Moves.Length];
                int movectr = 0;
                foreach (int move in Moves.Where(move => move != 0 && move < moves.Length))
                {
                    MoveLines[movectr] += "- " + moves[move];
                    if (move == 237)
                        MoveLines[movectr] += $" [{hptypes[getHPType(IVs)]}]";
                    movectr++;
                }
                result += string.Join(Environment.NewLine, MoveLines.Take(movectr));

                return result;
            }
        }
        internal static string getShowdownText(PK6 pk6)
        {
            if (pk6.Species == 0) return "";
            ShowdownSet Set = new ShowdownSet
            {
                Nickname = pk6.Nickname,
                Species = pk6.Species,
                Item = pk6.HeldItem,
                Ability = pk6.Ability,
                EVs = pk6.EVs,
                IVs = pk6.IVs,
                Moves = pk6.Moves,
                Nature = pk6.Nature,
                Gender = new[] { " (M)", " (F)", "" }[pk6.Gender],
                Friendship = pk6.CurrentFriendship,
                Level = getLevel(pk6.Species, pk6.EXP),
                Shiny = pk6.IsShiny,
                Form = pk6.AltForm > 0 ? getFormList(pk6.Species, ShowdownSet.types, ShowdownSet.forms, new [] {"", "F", ""})[pk6.AltForm] : "",
            };
            if (Set.Form == "F") Set.Gender = "";
            return Set.getText();
        }
    }
}
