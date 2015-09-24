using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }
        internal static uint LCRNG(ref uint seed)
        {
            const uint a = 0x41C64E6D;
            const uint c = 0x00006073;

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }
        internal static DataTable ExpTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Level", typeof(byte));
            table.Columns.Add("0 - MF", typeof(uint));
            table.Columns.Add("1 - Erratic", typeof(uint));
            table.Columns.Add("2 - Fluctuating", typeof(uint));
            table.Columns.Add("3 - MS", typeof(uint));
            table.Columns.Add("4 - Fast", typeof(uint));
            table.Columns.Add("5 - Slow", typeof(uint));
            table.Rows.Add(0, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(1, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(2, 8, 15, 4, 9, 6, 10);
            table.Rows.Add(3, 27, 52, 13, 57, 21, 33);
            table.Rows.Add(4, 64, 122, 32, 96, 51, 80);
            table.Rows.Add(5, 125, 237, 65, 135, 100, 156);
            table.Rows.Add(6, 216, 406, 112, 179, 172, 270);
            table.Rows.Add(7, 343, 637, 178, 236, 274, 428);
            table.Rows.Add(8, 512, 942, 276, 314, 409, 640);
            table.Rows.Add(9, 729, 1326, 393, 419, 583, 911);
            table.Rows.Add(10, 1000, 1800, 540, 560, 800, 1250);
            table.Rows.Add(11, 1331, 2369, 745, 742, 1064, 1663);
            table.Rows.Add(12, 1728, 3041, 967, 973, 1382, 2160);
            table.Rows.Add(13, 2197, 3822, 1230, 1261, 1757, 2746);
            table.Rows.Add(14, 2744, 4719, 1591, 1612, 2195, 3430);
            table.Rows.Add(15, 3375, 5737, 1957, 2035, 2700, 4218);
            table.Rows.Add(16, 4096, 6881, 2457, 2535, 3276, 5120);
            table.Rows.Add(17, 4913, 8155, 3046, 3120, 3930, 6141);
            table.Rows.Add(18, 5832, 9564, 3732, 3798, 4665, 7290);
            table.Rows.Add(19, 6859, 11111, 4526, 4575, 5487, 8573);
            table.Rows.Add(20, 8000, 12800, 5440, 5460, 6400, 10000);
            table.Rows.Add(21, 9261, 14632, 6482, 6458, 7408, 11576);
            table.Rows.Add(22, 10648, 16610, 7666, 7577, 8518, 13310);
            table.Rows.Add(23, 12167, 18737, 9003, 8825, 9733, 15208);
            table.Rows.Add(24, 13824, 21012, 10506, 10208, 11059, 17280);
            table.Rows.Add(25, 15625, 23437, 12187, 11735, 12500, 19531);
            table.Rows.Add(26, 17576, 26012, 14060, 13411, 14060, 21970);
            table.Rows.Add(27, 19683, 28737, 16140, 15244, 15746, 24603);
            table.Rows.Add(28, 21952, 31610, 18439, 17242, 17561, 27440);
            table.Rows.Add(29, 24389, 34632, 20974, 19411, 19511, 30486);
            table.Rows.Add(30, 27000, 37800, 23760, 21760, 21600, 33750);
            table.Rows.Add(31, 29791, 41111, 26811, 24294, 23832, 37238);
            table.Rows.Add(32, 32768, 44564, 30146, 27021, 26214, 40960);
            table.Rows.Add(33, 35937, 48155, 33780, 29949, 28749, 44921);
            table.Rows.Add(34, 39304, 51881, 37731, 33084, 31443, 49130);
            table.Rows.Add(35, 42875, 55737, 42017, 36435, 34300, 53593);
            table.Rows.Add(36, 46656, 59719, 46656, 40007, 37324, 58320);
            table.Rows.Add(37, 50653, 63822, 50653, 43808, 40522, 63316);
            table.Rows.Add(38, 54872, 68041, 55969, 47846, 43897, 68590);
            table.Rows.Add(39, 59319, 72369, 60505, 52127, 47455, 74148);
            table.Rows.Add(40, 64000, 76800, 66560, 56660, 51200, 80000);
            table.Rows.Add(41, 68921, 81326, 71677, 61450, 55136, 86151);
            table.Rows.Add(42, 74088, 85942, 78533, 66505, 59270, 92610);
            table.Rows.Add(43, 79507, 90637, 84277, 71833, 63605, 99383);
            table.Rows.Add(44, 85184, 95406, 91998, 77440, 68147, 106480);
            table.Rows.Add(45, 91125, 100237, 98415, 83335, 72900, 113906);
            table.Rows.Add(46, 97336, 105122, 107069, 89523, 77868, 121670);
            table.Rows.Add(47, 103823, 110052, 114205, 96012, 83058, 129778);
            table.Rows.Add(48, 110592, 115015, 123863, 102810, 88473, 138240);
            table.Rows.Add(49, 117649, 120001, 131766, 109923, 94119, 147061);
            table.Rows.Add(50, 125000, 125000, 142500, 117360, 100000, 156250);
            table.Rows.Add(51, 132651, 131324, 151222, 125126, 106120, 165813);
            table.Rows.Add(52, 140608, 137795, 163105, 133229, 112486, 175760);
            table.Rows.Add(53, 148877, 144410, 172697, 141677, 119101, 186096);
            table.Rows.Add(54, 157464, 151165, 185807, 150476, 125971, 196830);
            table.Rows.Add(55, 166375, 158056, 196322, 159635, 133100, 207968);
            table.Rows.Add(56, 175616, 165079, 210739, 169159, 140492, 219520);
            table.Rows.Add(57, 185193, 172229, 222231, 179056, 148154, 231491);
            table.Rows.Add(58, 195112, 179503, 238036, 189334, 156089, 243890);
            table.Rows.Add(59, 205379, 186894, 250562, 199999, 164303, 256723);
            table.Rows.Add(60, 216000, 194400, 267840, 211060, 172800, 270000);
            table.Rows.Add(61, 226981, 202013, 281456, 222522, 181584, 283726);
            table.Rows.Add(62, 238328, 209728, 300293, 234393, 190662, 297910);
            table.Rows.Add(63, 250047, 217540, 315059, 246681, 200037, 312558);
            table.Rows.Add(64, 262144, 225443, 335544, 259392, 209715, 327680);
            table.Rows.Add(65, 274625, 233431, 351520, 272535, 219700, 343281);
            table.Rows.Add(66, 287496, 241496, 373744, 286115, 229996, 359370);
            table.Rows.Add(67, 300763, 249633, 390991, 300140, 240610, 375953);
            table.Rows.Add(68, 314432, 257834, 415050, 314618, 251545, 393040);
            table.Rows.Add(69, 328509, 267406, 433631, 329555, 262807, 410636);
            table.Rows.Add(70, 343000, 276458, 459620, 344960, 274400, 428750);
            table.Rows.Add(71, 357911, 286328, 479600, 360838, 286328, 447388);
            table.Rows.Add(72, 373248, 296358, 507617, 377197, 298598, 466560);
            table.Rows.Add(73, 389017, 305767, 529063, 394045, 311213, 486271);
            table.Rows.Add(74, 405224, 316074, 559209, 411388, 324179, 506530);
            table.Rows.Add(75, 421875, 326531, 582187, 429235, 337500, 527343);
            table.Rows.Add(76, 438976, 336255, 614566, 447591, 351180, 548720);
            table.Rows.Add(77, 456533, 346965, 639146, 466464, 365226, 570666);
            table.Rows.Add(78, 474552, 357812, 673863, 485862, 379641, 593190);
            table.Rows.Add(79, 493039, 367807, 700115, 505791, 394431, 616298);
            table.Rows.Add(80, 512000, 378880, 737280, 526260, 409600, 640000);
            table.Rows.Add(81, 531441, 390077, 765275, 547274, 425152, 664301);
            table.Rows.Add(82, 551368, 400293, 804997, 568841, 441094, 689210);
            table.Rows.Add(83, 571787, 411686, 834809, 590969, 457429, 714733);
            table.Rows.Add(84, 592704, 423190, 877201, 613664, 474163, 740880);
            table.Rows.Add(85, 614125, 433572, 908905, 636935, 491300, 767656);
            table.Rows.Add(86, 636056, 445239, 954084, 660787, 508844, 795070);
            table.Rows.Add(87, 658503, 457001, 987754, 685228, 526802, 823128);
            table.Rows.Add(88, 681472, 467489, 1035837, 710266, 545177, 851840);
            table.Rows.Add(89, 704969, 479378, 1071552, 735907, 563975, 881211);
            table.Rows.Add(90, 729000, 491346, 1122660, 762160, 583200, 911250);
            table.Rows.Add(91, 753571, 501878, 1160499, 789030, 602856, 941963);
            table.Rows.Add(92, 778688, 513934, 1214753, 816525, 622950, 973360);
            table.Rows.Add(93, 804357, 526049, 1254796, 844653, 643485, 1005446);
            table.Rows.Add(94, 830584, 536557, 1312322, 873420, 664467, 1038230);
            table.Rows.Add(95, 857375, 548720, 1354652, 902835, 685900, 1071718);
            table.Rows.Add(96, 884736, 560922, 1415577, 932903, 707788, 1105920);
            table.Rows.Add(97, 912673, 571333, 1460276, 963632, 730138, 1140841);
            table.Rows.Add(98, 941192, 583539, 1524731, 995030, 752953, 1176490);
            table.Rows.Add(99, 970299, 591882, 1571884, 1027103, 776239, 1212873);
            table.Rows.Add(100, 1000000, 600000, 1640000, 1059860, 800000, 1250000);
            return table;
        }

        // Stat Fetching
        internal static int getMovePP(int move, int ppup)
        {
            return (getBasePP(move) * (5 + ppup) / 5);
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
            if (move < 0) { move = 0; }
            return movepptable[move];
        }
        internal static byte[] getRandomEVs()
        {
            byte[] evs = { 0xDE, 0xAD, 0xBE, 0xEF, 0xBA, 0xBE }; // ha ha, just to start off above 510!

            while (evs.Sum(b => (ushort)b) > 510) // recalculate random EVs...
            {
                evs[0] = (byte)Math.Min(Util.rnd32() % 300, 252); // bias two to get maybe 252
                evs[1] = (byte)Math.Min(Util.rnd32() % 300, 252);
                evs[2] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1])), 252);
                evs[3] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1] - evs[2])), 252);
                evs[4] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1] - evs[2] - evs[3])), 252);
                evs[5] = (byte)Math.Min((510 - evs[0] - evs[1] - evs[2] - evs[3] - evs[4]), 252);
            }
            Util.Shuffle(evs);
            return evs;
        }
        internal static byte getBaseFriendship(int species)
        {
            return PersonalGetter.GetPersonal(species).BaseFriendship;
        }
        internal static int getLevel(int species, uint exp)
        {
            if (exp == 0) { return 1; }

            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int growth = MonData.EXPGrowth;
            DataTable table = ExpTable();

            // Iterate upwards to find the level above our current level
            int tl = 0; // Initial Level, immediately incremented before loop.
            while ((uint)table.Rows[++tl][growth + 1] <= exp)
            {
                if (tl != 100) continue;
                return 100;
                // After we find the level above ours, we're done.
            }
            return --tl;
        }
        internal static bool getIsShiny(uint PID, uint TID, uint SID)
        {
            uint PSV = getPSV(PID);
            uint TSV = getTSV(TID, SID);
            return (TSV == PSV);
        }
        internal static uint getEXP(int level, int species)
        {
            // Fetch Growth
            if ((level == 0) || (level == 1))
                return 0;
            if (level > 100) level = 100;

            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int growth = MonData.EXPGrowth;

            uint exp = (uint)ExpTable().Rows[level][growth + 1];
            return exp;
        }
        internal static byte[] getAbilities(int species, int formnum)
        {
            return PersonalGetter.GetPersonal(species, formnum).Abilities;
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
                {
                    string[] inputCSV = Util.getSimpleStringList("countries");
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
            }
            catch { data[0] = "Illegal"; }

            // Get Region Text
            try
            {
                {
                    string[] inputCSV = Util.getSimpleStringList("sr_" + country.ToString("000"));
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
        internal static ushort[] getStats(int species, int level, int nature, int form,
                                        int HP_EV, int ATK_EV, int DEF_EV, int SPA_EV, int SPD_EV, int SPE_EV,
                                        int HP_IV, int ATK_IV, int DEF_IV, int SPA_IV, int SPD_IV, int SPE_IV)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species, form);
            int HP_B = MonData.BaseStats[0];
            int ATK_B = MonData.BaseStats[1];
            int DEF_B = MonData.BaseStats[2];
            int SPE_B = MonData.BaseStats[3];
            int SPA_B = MonData.BaseStats[4];
            int SPD_B = MonData.BaseStats[5];

            // Calculate Stats
            ushort[] stats = new ushort[6]; // Stats are stored as ushorts in the PKX structure. We'll cap them as such.
            stats[0] = (HP_B == 1) ? (ushort)1 : (ushort)((((HP_IV + (2 * HP_B) + (HP_EV / 4) + 100) * level) / 100) + 10);
            stats[1] = (ushort)((((ATK_IV + (2 * ATK_B) + (ATK_EV / 4)) * level) / 100) + 5);
            stats[2] = (ushort)((((DEF_IV + (2 * DEF_B) + (DEF_EV / 4)) * level) / 100) + 5);
            stats[4] = (ushort)((((SPA_IV + (2 * SPA_B) + (SPA_EV / 4)) * level) / 100) + 5);
            stats[5] = (ushort)((((SPD_IV + (2 * SPD_B) + (SPD_EV / 4)) * level) / 100) + 5);
            stats[3] = (ushort)((((SPE_IV + (2 * SPE_B) + (SPE_EV / 4)) * level) / 100) + 5);

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
        internal static byte[] shuffleArray(byte[] pkx, uint sv)
        {
            byte[] ekx = new byte[260];
            Array.Copy(pkx, ekx, 8);

            // Now to shuffle the blocks

            // Define Shuffle Order Structure
            byte[] aloc = { 0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3 };
            byte[] bloc = { 1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2 };
            byte[] cloc = { 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1 };
            byte[] dloc = { 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0 };

            // Get Shuffle Order
            byte[] shlog = { aloc[sv], bloc[sv], cloc[sv], dloc[sv] };

            // UnShuffle Away!
            for (int b = 0; b < 4; b++)
                Array.Copy(pkx, 8 + 56 * shlog[b], ekx, 8 + 56 * b, 56);

            // Fill the Battle Stats back
            if (pkx.Length > 232)
                Array.Copy(pkx, 232, ekx, 232, 28);

            return ekx;
        }
        internal static byte[] decryptArray(byte[] ekx)
        {
            byte[] pkx = (byte[])ekx.Clone();

            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            uint seed = pv;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
                Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ (LCRNG(ref seed) >> 16))), 0, pkx, i, 2);

            // Deshuffle
            pkx = shuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            if (pkx.Length <= 232) return pkx;
            for (int i = 232; i < 260; i += 2)
                Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ (LCRNG(ref seed) >> 16))), 0, pkx, i, 2);

            return pkx;
        }
        internal static byte[] encryptArray(byte[] pkx)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            byte[] ekx = (byte[])pkx.Clone();

            // If I unshuffle 11 times, the 12th (decryption) will always decrypt to ABCD.
            // 2 x 3 x 4 = 12 (possible unshuffle loops -> total iterations)
            for (int i = 0; i < 11; i++)
                ekx = shuffleArray(ekx, sv);

            uint seed = pv;
            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
                Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekx, i) ^ (LCRNG(ref seed) >> 16))), 0, ekx, i, 2);

            // If no party stats, return.
            if (ekx.Length <= 232) return ekx;

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
                Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekx, i) ^ (LCRNG(ref seed) >> 16))), 0, ekx, i, 2);

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
            if (input.Length == 100 || input.Length == 80)  // Gen 3 Files
            {
                for (int i = 32; i < 80; i += 2)
                    checksum += BitConverter.ToUInt16(input, i);

                return (checksum == BitConverter.ToUInt16(input, 28));
            }
            {
                if (input.Length == 236 || input.Length == 220 || input.Length == 136) // Gen 4/5
                    Array.Resize(ref input, 136);
                else if (input.Length == 232 || input.Length == 260) // Gen 6
                    Array.Resize(ref input, 232);
                else throw new Exception("Wrong sized input array to verifychecksum");

                ushort chk = 0;
                for (int i = 8; i < input.Length; i += 2)
                    chk += BitConverter.ToUInt16(input, i);

                return (chk == BitConverter.ToUInt16(input, 0x6));
            }
        }
        internal static uint getPSV(uint PID)
        {
            return Convert.ToUInt16(((PID >> 16) ^ (PID & 0xFFFF)) >> 4);
        }
        internal static uint getTSV(uint TID, uint SID)
        {
            return ((TID ^ SID) >> 4);
        }
        internal static uint getRandomPID(int species, int cg)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int gt = MonData.GenderRatio;
            uint pid = Util.rnd32();
            if (gt == 255) //Genderless
                return pid;
            if (gt == 254) //Female Only
                gt++;
            while (true) // Loop until we find a suitable PID
            {
                uint gv = (pid & 0xFF);
                if (cg == 2) // Genderless
                    break;  // PID Passes
                if ((cg == 1) && (gv <= gt)) // Female
                    break;  // PID Passes
                if ((cg == 0) && (gv > gt))
                    break;  // PID Passes
                pid = Util.rnd32();
            }
            return pid;
        }

        #region SaveGame

        public class Structures
        {
            public struct PK6
            {
                public uint EC;
                public ushort Checksum, Sanity;

                public uint PID, Experience;
                public ushort Species, HeldItem, TID, SID;
                public byte Ability, AbilityNumber, Nature, Gender, Form;
                public bool Fateful;

                public byte HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV;
                public byte HP_IV, ATK_IV, DEF_IV, SPA_IV, SPD_IV, SPE_IV;
                public byte CNT_Cool, CNT_Beauty, CNT_Cute, CNT_Clever, CNT_Tough, CNT_Sheen;
                public bool MARK_Circle, MARK_Triangle, MARK_Square, MARK_Heart, MARK_Star, MARK_Diamond;
                public byte PKRS_Strain, PKRS_Duration;

                public string Nickname, OT, CT;
                public ushort Move1, Move2, Move3, Move4, Relearn1, Relearn2, Relearn3, Relearn4;
                public byte Move1_PP, Move2_PP, Move3_PP, Move4_PP, Move1_PPUps, Move2_PPUps, Move3_PPUps, Move4_PPUps;

                public bool isNicknamed, isEgg, notWithOT;
                public byte Ball, MetLevel, OTGender, CTGender, OTFriendship, OTAffection, CTFriendship, CTAffection;
                public ushort MetLocation, EggLocation;
                public byte MetYear, MetMonth, MetDay, EggYear, EggMonth, EggDay;
                public byte Gen4EncounterType, GameLanguage, GameVersion, ConsoleRegionID, CountryID, SubRegionID;

                // Misc
                public byte HitsRemaining, TrainingBag;
                public PK6(byte[] pk6)
                {
                    // Encryption Constant
                    EC = BitConverter.ToUInt32(pk6, 0);
                    Sanity = BitConverter.ToUInt16(pk6, 4);
                    Checksum = BitConverter.ToUInt16(pk6, 6);

                    // Block A
                    Species = BitConverter.ToUInt16(pk6, 0x08);
                    HeldItem = BitConverter.ToUInt16(pk6, 0x0A);
                    TID = BitConverter.ToUInt16(pk6, 0x0C);
                    SID = BitConverter.ToUInt16(pk6, 0x0E);
                    Experience = BitConverter.ToUInt32(pk6, 0x10);
                    Ability = pk6[0x14];
                    AbilityNumber = pk6[0x15];
                    HitsRemaining = pk6[0x16];
                    TrainingBag = pk6[0x17];
                    PID = BitConverter.ToUInt32(pk6, 0x18);
                    Nature = pk6[0x1C];
                    Fateful = (pk6[0x1D] % 2 == 0);
                    Gender = (byte)((pk6[0x1D] >> 1) & 0x3);
                    Form = (byte)(pk6[0x1D] >> 3);
                    HP_EV = pk6[0x1E];
                    ATK_EV = pk6[0x1F];
                    DEF_EV = pk6[0x20];
                    SPA_EV = pk6[0x22];
                    SPD_EV = pk6[0x23];
                    SPE_EV = pk6[0x21];
                    CNT_Cool = pk6[0x24];
                    CNT_Beauty = pk6[0x25];
                    CNT_Cute = pk6[0x26];
                    CNT_Clever = pk6[0x27];
                    CNT_Tough = pk6[0x28];
                    CNT_Sheen = pk6[0x29];
                    MARK_Circle = ((pk6[0x2A] >> 0) & 1) == 1;
                    MARK_Triangle = ((pk6[0x2A] >> 1) & 1) == 1;
                    MARK_Square = ((pk6[0x2A] >> 2) & 1) == 1;
                    MARK_Heart = ((pk6[0x2A] >> 3) & 1) == 1;
                    MARK_Star = ((pk6[0x2A] >> 4) & 1) == 1;
                    MARK_Diamond = ((pk6[0x2A] >> 5) & 1) == 1;
                    PKRS_Strain = (byte)(pk6[0x2B] >> 4);
                    PKRS_Duration = (byte)(pk6[0x2B] % 0x10);

                    // Block B
                    Nickname = Util.TrimFromZero(Encoding.Unicode.GetString(pk6, 0x40, 24));
                    // 0x58, 0x59 - Null Terminator
                    Move1 = BitConverter.ToUInt16(pk6, 0x5A);
                    Move2 = BitConverter.ToUInt16(pk6, 0x5C);
                    Move3 = BitConverter.ToUInt16(pk6, 0x5E);
                    Move4 = BitConverter.ToUInt16(pk6, 0x60);
                    Move1_PP = pk6[0x62];
                    Move2_PP = pk6[0x63];
                    Move3_PP = pk6[0x64];
                    Move4_PP = pk6[0x65];
                    Move1_PPUps = pk6[0x66];
                    Move2_PPUps = pk6[0x67];
                    Move3_PPUps = pk6[0x68];
                    Move4_PPUps = pk6[0x69];
                    Relearn1 = BitConverter.ToUInt16(pk6, 0x6A);
                    Relearn2 = BitConverter.ToUInt16(pk6, 0x6C);
                    Relearn3 = BitConverter.ToUInt16(pk6, 0x6E);
                    Relearn4 = BitConverter.ToUInt16(pk6, 0x70);

                    // 0x72 - Super Training Flag - Passed with buff to new form

                    // 0x73 - unused/unknown
                    uint IV32 = BitConverter.ToUInt32(pk6, 0x74);
                    HP_IV = (byte)(IV32 & 0x1F);
                    ATK_IV = (byte)((IV32 >> 5) & 0x1F);
                    DEF_IV = (byte)((IV32 >> 10) & 0x1F);
                    SPE_IV = (byte)((IV32 >> 15) & 0x1F);
                    SPA_IV = (byte)((IV32 >> 20) & 0x1F);
                    SPD_IV = (byte)((IV32 >> 25) & 0x1F);
                    isEgg = ((IV32 >> 30) & 1) == 1;
                    isNicknamed = (IV32 >> 31) == 1;

                    // Block C
                    CT = Util.TrimFromZero(Encoding.Unicode.GetString(pk6, 0x78, 24));
                    CTGender = pk6[0x92];
                    notWithOT = pk6[0x93] == 1;
                    CTFriendship = pk6[0xA2];
                    CTAffection = pk6[0xA3];
                    // Memory Editor edits everything else with buff in a new form

                    // Block D
                    OT = Util.TrimFromZero(Encoding.Unicode.GetString(pk6, 0xB0, 24));
                    // 0xC8, 0xC9 - unused
                    OTFriendship = pk6[0xCA];
                    OTAffection = pk6[0xCB];
                    // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0 unused
                    EggYear = pk6[0xD1];
                    EggMonth = pk6[0xD2];
                    EggDay = pk6[0xD3];
                    MetYear = pk6[0xD4];
                    MetMonth = pk6[0xD5];
                    MetDay = pk6[0xD6];
                    // 0xD7 - unused
                    EggLocation = BitConverter.ToUInt16(pk6, 0xD8);
                    MetLocation = BitConverter.ToUInt16(pk6, 0xDA);
                    Ball = pk6[0xDC];
                    MetLevel = (byte)(pk6[0xDD] & 0x7F);
                    OTGender = (byte)(pk6[0xDD] >> 7);
                    Gen4EncounterType = pk6[0xDE];
                    GameVersion = pk6[0xDF];
                    CountryID = pk6[0xE0];
                    SubRegionID = pk6[0xE1];
                    ConsoleRegionID = pk6[0xE2];
                    GameLanguage = pk6[0xE3];
                }
            }
            public struct SaveGame
            {
                public class Inventory
                {
                    public int HeldItem, KeyItem, Medicine, TMHM, Berry;
                    public Inventory(string GameID, int Offset)
                    {
                        switch (GameID)
                        {
                            case "XY":
                                HeldItem = Offset + 0;
                                KeyItem = Offset + 0x640;
                                TMHM = Offset + 0x7C0;
                                Medicine = Offset + 0x968;
                                Berry = Offset + 0xA68;
                                break;
                            case "ORAS":
                                HeldItem = Offset + 0;
                                KeyItem = Offset + 0x640;
                                TMHM = Offset + 0x7C0;
                                Medicine = Offset + 0x970;
                                Berry = Offset + 0xA70;
                                break;
                            default:
                                HeldItem = KeyItem = TMHM = Medicine = Berry = 0;
                                break;
                        }
                    }
                }
                public int Box, TrainerCard, Party, BattleBox, GTS, Daycare, EonTicket,
                    Fused, SUBE, Puff, Item, Trainer1, Trainer2, SuperTrain, PSSStats, MaisonStats, Vivillon, SecretBase,
                    PCLayout, PCBackgrounds, PCFlags, WondercardFlags, WondercardData, BerryField, OPower, EventConst, EventFlag, EventAsh,
                    PokeDex, PokeDexLanguageFlags, Spinda, EncounterCount, HoF, PSS, JPEG;

                public Inventory Items;
                public string Name;
                public int[] DaycareSlot;
                public bool ORAS;

                public SaveGame(string GameID)
                {
                    switch (GameID)
                    {
                        case "XY":
                            Name = "XY";
                            ORAS = false;
                            Box = 0x22600;
                            TrainerCard = 0x14000;
                            Party = 0x14200;
                            BattleBox = 0x04A00;
                            Daycare = 0x1B200;
                            GTS = 0x17800;
                            Fused = 0x16000;
                            SUBE = 0x1D890;
                            Puff = 0x00000;
                            Item = 0x00400;
                            Items = new Inventory(Name, Item);
                            Trainer1 = 0x1400;
                            Trainer2 = 0x4200;
                            PCLayout = 0x4400;
                            PCBackgrounds = PCLayout + 0x41E;
                            PCFlags = PCLayout + 0x43D;
                            WondercardFlags = 0x1BC00;
                            WondercardData = WondercardFlags + 0x100;
                            BerryField = 0x1B800;
                            OPower = 0x16A00;
                            EventConst = 0x14A00;
                            EventAsh = -1;
                            EventFlag = EventConst + 0x2FC;
                            PokeDex = 0x15000;
                            PokeDexLanguageFlags = PokeDex + 0x3C8;
                            Spinda = PokeDex + 0x648;
                            EncounterCount = -1;
                            HoF = 0x19400;
                            SuperTrain = 0x1F200;
                            JPEG = 0x57200;
                            MaisonStats = 0x1B1C0;
                            PSS = 0x05000;
                            PSSStats = 0x1E400;
                            Vivillon = 0x4250;
                            SecretBase = -1;
                            EonTicket = -1;
                            break;
                        case "ORAS":
                            Name = "ORAS";
                            ORAS = true;
                            Box = 0x33000;      // Confirmed
                            TrainerCard = 0x14000; // Confirmed
                            Party = 0x14200;    // Confirmed
                            BattleBox = 0x04A00;// Confirmed
                            Daycare = 0x1BC00; // Confirmed (thanks Rei)
                            GTS = 0x18200; // Confirmed
                            Fused = 0x16A00; // Confirmed
                            SUBE = 0x1D890; // ****not in use, not updating?****
                            Puff = 0x00000; // Confirmed
                            Item = 0x00400; // Confirmed
                            Items = new Inventory(Name, Item);
                            Trainer1 = 0x01400; // Confirmed
                            Trainer2 = 0x04200; // Confirmed
                            PCLayout = 0x04400; // Confirmed
                            PCBackgrounds = PCLayout + 0x41E;
                            PCFlags = PCLayout + 0x43D;
                            WondercardFlags = 0x1CC00; // Confirmed
                            WondercardData = WondercardFlags + 0x100;
                            BerryField = 0x1C400; // ****changed****
                            OPower = 0x17400; // ****changed****
                            EventConst = 0x14A00;
                            EventAsh = EventConst + 0x78;
                            EventFlag = EventConst + 0x2FC;
                            PokeDex = 0x15000;
                            Spinda = PokeDex + 0x680;
                            EncounterCount = PokeDex + 0x686;
                            PokeDexLanguageFlags = PokeDex + 0x400;
                            HoF = 0x19E00; // Confirmed
                            SuperTrain = 0x20200;
                            JPEG = 0x67C00; // Confirmed
                            MaisonStats = 0x1BBC0;
                            PSS = 0x05000; // Confirmed (thanks Rei)
                            PSSStats = 0x1F400;
                            Vivillon = 0x4244;
                            SecretBase = 0x23A00;
                            EonTicket = 0x319B8;
                            break;
                        default:
                            Box = TrainerCard = Party = BattleBox = GTS = Daycare = 
                            Fused = SUBE = Puff = Item = Trainer1 = Trainer2 = SecretBase = EonTicket = 
                            PCLayout = PCBackgrounds = PCFlags = WondercardFlags = WondercardData = BerryField = OPower = SuperTrain = MaisonStats = PSSStats = Vivillon = 
                            EventConst = EventAsh = EventFlag = PokeDex = PokeDexLanguageFlags = Spinda = EncounterCount = HoF = PSS = JPEG = 0;
                            Name = "Unknown";
                            ORAS = false;
                            Items = new Inventory(Name, Item);
                            break;
                    }
                    DaycareSlot = new[] { Daycare, Daycare + 0x1F0 };
                }
            }
        }
        #endregion

        // SAV Manipulation
        internal static int detectSAVIndex(byte[] data, out int savindex)
        {
            SHA256 mySHA256 = SHA256.Create();
            {
                byte[] difihash1 = new byte[0x12C];
                byte[] difihash2 = new byte[0x12C];
                Array.Copy(data, 0x330, difihash1, 0, 0x12C);
                Array.Copy(data, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new byte[0x20];
                Array.Copy(data, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                {
                    Console.WriteLine("Active DIFI 2 - Save 1.");
                    savindex = 0;
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    Console.WriteLine("Active DIFI 1 - Save 2.");
                    savindex = 1;
                }
                else
                {
                    Console.WriteLine("ERROR: NO ACTIVE DIFI HASH MATCH");
                    savindex = 2;
                }
            }
            if ((data[0x168] ^ 1) == savindex || savindex == 2) // success
                return savindex;
            Console.WriteLine("ERROR: ACTIVE BLOCK MISMATCH");
            savindex = 2;
            return savindex;
        }
        internal static ushort ccitt16(byte[] data)
        {
            ushort crc = 0xFFFF;
            foreach (byte t in data)
            {
                crc ^= (ushort)(t << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }
        internal static string verifyG6CHK(byte[] savefile)
        {
            string rv = "";
            int invalid = 0;
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != 0x42454546)
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

                CurrentPosition += (Lengths[i] % 0x200 == 0) ? Lengths[i] : (0x200 - Lengths[i] % 0x200 + Lengths[i]);

                if ((BlockIDs[i] != 0) || i == 0) continue;
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
                rv += String.Format("Invalid: {0} @ Region {1}", i.ToString("X2"), Start[i].ToString("X5") + Environment.NewLine);
            }
            // Return Outputs
            rv += String.Format("SAV: {0}/{1}", (count - invalid), count + Environment.NewLine);
            return rv;
        }
        internal static string verifyG6SHA(byte[] savefile, bool oras)
        {
            string rv = "";
            int invalid1 = 0;
            // Verify Hashes
            #region hash table data
            uint[] hashtabledata = {
                                    0x2020,	    0x203F,	    0x2000,	0x200,
                                    0x2040,	    0x2FFF,	    0x2020,	0x1000,
                                    0x3000,	    0x3FFF,	    0x2040,	0x1000,
                                    0x4000,	    0x4FFF,	    0x2060,	0x1000,
                                    0x5000,	    0x5FFF,	    0x2080,	0x1000,
                                    0x6000,	    0x6FFF,	    0x20A0,	0x1000,
                                    0x7000,	    0x7FFF,	    0x20C0,	0x1000,
                                    0x8000,	    0x8FFF,	    0x20E0,	0x1000,
                                    0x9000,	    0x9FFF,	    0x2100,	0x1000,
                                    0xA000,	    0xAFFF,	    0x2120,	0x1000,
                                    0xB000,	    0xBFFF,	    0x2140,	0x1000,
                                    0xC000,	    0xCFFF,	    0x2160,	0x1000,
                                    0xD000,	    0xDFFF,	    0x2180,	0x1000,
                                    0xE000,	    0xEFFF,	    0x21A0,	0x1000,
                                    0xF000,	    0xFFFF,	    0x21C0,	0x1000,
                                    0x10000,	0x10FFF,	0x21E0,	0x1000,
                                    0x11000,	0x11FFF,	0x2200,	0x1000,
                                    0x12000,	0x12FFF,	0x2220,	0x1000,
                                    0x13000,	0x13FFF,	0x2240,	0x1000,
                                    0x14000,	0x14FFF,	0x2260,	0x1000,
                                    0x15000,	0x15FFF,	0x2280,	0x1000,
                                    0x16000,	0x16FFF,	0x22A0,	0x1000,
                                    0x17000,	0x17FFF,	0x22C0,	0x1000,
                                    0x18000,	0x18FFF,	0x22E0,	0x1000,
                                    0x19000,	0x19FFF,	0x2300,	0x1000,
                                    0x1A000,	0x1AFFF,	0x2320,	0x1000,
                                    0x1B000,	0x1BFFF,	0x2340,	0x1000,
                                    0x1C000,	0x1CFFF,	0x2360,	0x1000,
                                    0x1D000,	0x1DFFF,	0x2380,	0x1000,
                                    0x1E000,	0x1EFFF,	0x23A0,	0x1000,
                                    0x1F000,	0x1FFFF,	0x23C0,	0x1000,
                                    0x20000,	0x20FFF,	0x23E0,	0x1000,
                                    0x21000,	0x21FFF,	0x2400,	0x1000,
                                    0x22000,	0x22FFF,	0x2420,	0x1000,
                                    0x23000,	0x23FFF,	0x2440,	0x1000,
                                    0x24000,	0x24FFF,	0x2460,	0x1000,
                                    0x25000,	0x25FFF,	0x2480,	0x1000,
                                    0x26000,	0x26FFF,	0x24A0,	0x1000,
                                    0x27000,	0x27FFF,	0x24C0,	0x1000,
                                    0x28000,	0x28FFF,	0x24E0,	0x1000,
                                    0x29000,	0x29FFF,	0x2500,	0x1000,
                                    0x2A000,	0x2AFFF,	0x2520,	0x1000,
                                    0x2B000,	0x2BFFF,	0x2540,	0x1000,
                                    0x2C000,	0x2CFFF,	0x2560,	0x1000,
                                    0x2D000,	0x2DFFF,	0x2580,	0x1000,
                                    0x2E000,	0x2EFFF,	0x25A0,	0x1000,
                                    0x2F000,	0x2FFFF,	0x25C0,	0x1000,
                                    0x30000,	0x30FFF,	0x25E0,	0x1000,
                                    0x31000,	0x31FFF,	0x2600,	0x1000,
                                    0x32000,	0x32FFF,	0x2620,	0x1000,
                                    0x33000,	0x33FFF,	0x2640,	0x1000,
                                    0x34000,	0x34FFF,	0x2660,	0x1000,
                                    0x35000,	0x35FFF,	0x2680,	0x1000,
                                    0x36000,	0x36FFF,	0x26A0,	0x1000,
                                    0x37000,	0x37FFF,	0x26C0,	0x1000,
                                    0x38000,	0x38FFF,	0x26E0,	0x1000,
                                    0x39000,	0x39FFF,	0x2700,	0x1000,
                                    0x3A000,	0x3AFFF,	0x2720,	0x1000,
                                    0x3B000,	0x3BFFF,	0x2740,	0x1000,
                                    0x3C000,	0x3CFFF,	0x2760,	0x1000,
                                    0x3D000,	0x3DFFF,	0x2780,	0x1000,
                                    0x3E000,	0x3EFFF,	0x27A0,	0x1000,
                                    0x3F000,	0x3FFFF,	0x27C0,	0x1000,
                                    0x40000,	0x40FFF,	0x27E0,	0x1000,
                                    0x41000,	0x41FFF,	0x2800,	0x1000,
                                    0x42000,	0x42FFF,	0x2820,	0x1000,
                                    0x43000,	0x43FFF,	0x2840,	0x1000,
                                    0x44000,	0x44FFF,	0x2860,	0x1000,
                                    0x45000,	0x45FFF,	0x2880,	0x1000,
                                    0x46000,	0x46FFF,	0x28A0,	0x1000,
                                    0x47000,	0x47FFF,	0x28C0,	0x1000,
                                    0x48000,	0x48FFF,	0x28E0,	0x1000,
                                    0x49000,	0x49FFF,	0x2900,	0x1000,
                                    0x4A000,	0x4AFFF,	0x2920,	0x1000,
                                    0x4B000,	0x4BFFF,	0x2940,	0x1000,
                                    0x4C000,	0x4CFFF,	0x2960,	0x1000,
                                    0x4D000,	0x4DFFF,	0x2980,	0x1000,
                                    0x4E000,	0x4EFFF,	0x29A0,	0x1000,
                                    0x4F000,	0x4FFFF,	0x29C0,	0x1000,
                                    0x50000,	0x50FFF,	0x29E0,	0x1000,
                                    0x51000,	0x51FFF,	0x2A00,	0x1000,
                                    0x52000,	0x52FFF,	0x2A20,	0x1000,
                                    0x53000,	0x53FFF,	0x2A40,	0x1000,
                                    0x54000,	0x54FFF,	0x2A60,	0x1000,
                                    0x55000,	0x55FFF,	0x2A80,	0x1000,
                                    0x56000,	0x56FFF,	0x2AA0,	0x1000,
                                    0x57000,	0x57FFF,	0x2AC0,	0x1000,
                                    0x58000,	0x58FFF,	0x2AE0,	0x1000,
                                    0x59000,	0x59FFF,	0x2B00,	0x1000,
                                    0x5A000,	0x5AFFF,	0x2B20,	0x1000,
                                    0x5B000,	0x5BFFF,	0x2B40,	0x1000,
                                    0x5C000,	0x5CFFF,	0x2B60,	0x1000,
                                    0x5D000,	0x5DFFF,	0x2B80,	0x1000,
                                    0x5E000,	0x5EFFF,	0x2BA0,	0x1000,
                                    0x5F000,	0x5FFFF,	0x2BC0,	0x1000,
                                    0x60000,	0x60FFF,	0x2BE0,	0x1000,
                                    0x61000,	0x61FFF,	0x2C00,	0x1000,
                                    0x62000,	0x62FFF,	0x2C20,	0x1000,
                                    0x63000,	0x63FFF,	0x2C40,	0x1000,
                                    0x64000,	0x64FFF,	0x2C60,	0x1000,
                                    0x65000,	0x65FFF,	0x2C80,	0x1000,
                                    0x66000,	0x66FFF,	0x2CA0,	0x1000,
                                    0x67000,	0x67FFF,	0x2CC0,	0x1000,
                                    0x68000,	0x68FFF,	0x2CE0,	0x1000,
                                    0x69000,	0x69FFF,	0x2D00,	0x1000,
                                    0x6A000,	0x6AFFF,	0x2D20,	0x1000,
                                 };
            #endregion
            SHA256 mySHA256 = SHA256.Create();

            for (int i = 0; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new byte[0x20];
                Array.Copy(savefile, offset, actualhash, 0, 0x20);

                if (hashValue.SequenceEqual(actualhash)) continue;
                invalid1++;
                rv += "Invalid: " + hashtabledata[2 + 4 * i].ToString("X5") + " @ " + hashtabledata[0 + 4 * i].ToString("X5") + "-" + hashtabledata[1 + 4 * i].ToString("X5") + Environment.NewLine;
            }
            rv += "1st SAV: " + (106 - invalid1) + "/" + 106 + Environment.NewLine;

            // Check The Second Half of Hashes
            int invalid2 = 0;
            for (int i = 0; i < hashtabledata.Length; i += 4)
            {
                hashtabledata[i + 0] += 0x7F000;
                hashtabledata[i + 1] += 0x7F000;
                hashtabledata[i + 2] += 0x7F000;
            }
            // Problem with save2 saves is that 0x3000-0x4FFF doesn't use save2 data. Probably different when hashed, but different when stored.
            for (int i = 2; i < 4; i++)
            {
                hashtabledata[i * 4 + 0] -= 0x7F000;
                hashtabledata[i * 4 + 1] -= 0x7F000;
            }

            for (int i = 0; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new byte[0x20];
                Array.Copy(savefile, offset, actualhash, 0, 0x20);

                if (hashValue.SequenceEqual(actualhash)) continue;
                invalid2++;
                rv += "Invalid: " + hashtabledata[2 + 4 * i].ToString("X5") + " @ " + hashtabledata[0 + 4 * i].ToString("X5") + "-" + hashtabledata[1 + 4 * i].ToString("X5") + Environment.NewLine;
            }
            rv += "2nd SAV: " + (106 - invalid2) + "/" + 106 + Environment.NewLine;

            if (invalid1 + invalid2 == (2 * 106))
                rv = "None of the IVFC hashes are valid." + Environment.NewLine;

            // Check the Upper Level IVFC Hashes
            {
                byte[] zeroarray = new byte[0x200];
                Array.Copy(savefile, 0x2000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new byte[0x20];
                Array.Copy(savefile, 0x43C, actualhash, 0, 0x20);
                if (!hashValue.SequenceEqual(actualhash))
                    rv += "Invalid: " + 0x2000.ToString("X5") + " @ " + 0x43C.ToString("X3") + Environment.NewLine;
            }
            {
                byte[] zeroarray = new byte[0x200];
                Array.Copy(savefile, 0x81000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new byte[0x20];
                Array.Copy(savefile, 0x30C, actualhash, 0, 0x20);
                if (!hashValue.SequenceEqual(actualhash))
                    rv += "Invalid: " + 0x81000.ToString("X5") + " @ " + 0x30C.ToString("X3") + Environment.NewLine;
            }
            {
                byte[] difihash1 = new byte[0x12C];
                byte[] difihash2 = new byte[0x12C];
                Array.Copy(savefile, 0x330, difihash1, 0, 0x12C);
                Array.Copy(savefile, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new byte[0x20];
                Array.Copy(savefile, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                    rv += "Active DIFI partition is Save 1.";
                else if (hashValue2.SequenceEqual(actualhash))
                    rv += "Active DIFI partition is Save 2.";
                else
                    rv += "ERROR: NO ACTIVE DIFI HASH MATCH";
            }
            return rv;
        }
        internal static void writeG6CHK(byte[] savefile)
        {
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != 0x42454546)
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

                CurrentPosition += (Lengths[i]%0x200 == 0) ? Lengths[i] : (0x200 - Lengths[i]%0x200 + Lengths[i]);

                if ((BlockIDs[i] != 0) || i == 0) continue;
                count = i;
                break;
            }
            // Apply checksums
            for (int i = 0; i < count; i++)
            {
                byte[] array = savefile.Skip(Start[i]).Take(Lengths[i]).ToArray();
                Array.Copy(BitConverter.GetBytes(ccitt16(array)), 0, savefile, verificationOffset + 6 + i * 8, 2);
            }
        }
        internal static byte[] writeG6SHA(byte[] savefile, bool oras, int savegame)
        {
            #region hash table data
            uint[] hashtabledata = {
                                    0x2020,	    0x203F,	    0x2000,	0x200,
                                    0x2040,	    0x2FFF,	    0x2020,	0x1000,
                                    0x3000,	    0x3FFF,	    0x2040,	0x1000,
                                    0x4000,	    0x4FFF,	    0x2060,	0x1000,
                                    0x5000,	    0x5FFF,	    0x2080,	0x1000,
                                    0x6000,	    0x6FFF,	    0x20A0,	0x1000,
                                    0x7000,	    0x7FFF,	    0x20C0,	0x1000,
                                    0x8000,	    0x8FFF,	    0x20E0,	0x1000,
                                    0x9000,	    0x9FFF,	    0x2100,	0x1000,
                                    0xA000,	    0xAFFF,	    0x2120,	0x1000,
                                    0xB000,	    0xBFFF,	    0x2140,	0x1000,
                                    0xC000,	    0xCFFF,	    0x2160,	0x1000,
                                    0xD000,	    0xDFFF,	    0x2180,	0x1000,
                                    0xE000,	    0xEFFF,	    0x21A0,	0x1000,
                                    0xF000,	    0xFFFF,	    0x21C0,	0x1000,
                                    0x10000,	0x10FFF,	0x21E0,	0x1000,
                                    0x11000,	0x11FFF,	0x2200,	0x1000,
                                    0x12000,	0x12FFF,	0x2220,	0x1000,
                                    0x13000,	0x13FFF,	0x2240,	0x1000,
                                    0x14000,	0x14FFF,	0x2260,	0x1000,
                                    0x15000,	0x15FFF,	0x2280,	0x1000,
                                    0x16000,	0x16FFF,	0x22A0,	0x1000,
                                    0x17000,	0x17FFF,	0x22C0,	0x1000,
                                    0x18000,	0x18FFF,	0x22E0,	0x1000,
                                    0x19000,	0x19FFF,	0x2300,	0x1000,
                                    0x1A000,	0x1AFFF,	0x2320,	0x1000,
                                    0x1B000,	0x1BFFF,	0x2340,	0x1000,
                                    0x1C000,	0x1CFFF,	0x2360,	0x1000,
                                    0x1D000,	0x1DFFF,	0x2380,	0x1000,
                                    0x1E000,	0x1EFFF,	0x23A0,	0x1000,
                                    0x1F000,	0x1FFFF,	0x23C0,	0x1000,
                                    0x20000,	0x20FFF,	0x23E0,	0x1000,
                                    0x21000,	0x21FFF,	0x2400,	0x1000,
                                    0x22000,	0x22FFF,	0x2420,	0x1000,
                                    0x23000,	0x23FFF,	0x2440,	0x1000,
                                    0x24000,	0x24FFF,	0x2460,	0x1000,
                                    0x25000,	0x25FFF,	0x2480,	0x1000,
                                    0x26000,	0x26FFF,	0x24A0,	0x1000,
                                    0x27000,	0x27FFF,	0x24C0,	0x1000,
                                    0x28000,	0x28FFF,	0x24E0,	0x1000,
                                    0x29000,	0x29FFF,	0x2500,	0x1000,
                                    0x2A000,	0x2AFFF,	0x2520,	0x1000,
                                    0x2B000,	0x2BFFF,	0x2540,	0x1000,
                                    0x2C000,	0x2CFFF,	0x2560,	0x1000,
                                    0x2D000,	0x2DFFF,	0x2580,	0x1000,
                                    0x2E000,	0x2EFFF,	0x25A0,	0x1000,
                                    0x2F000,	0x2FFFF,	0x25C0,	0x1000,
                                    0x30000,	0x30FFF,	0x25E0,	0x1000,
                                    0x31000,	0x31FFF,	0x2600,	0x1000,
                                    0x32000,	0x32FFF,	0x2620,	0x1000,
                                    0x33000,	0x33FFF,	0x2640,	0x1000,
                                    0x34000,	0x34FFF,	0x2660,	0x1000,
                                    0x35000,	0x35FFF,	0x2680,	0x1000,
                                    0x36000,	0x36FFF,	0x26A0,	0x1000,
                                    0x37000,	0x37FFF,	0x26C0,	0x1000,
                                    0x38000,	0x38FFF,	0x26E0,	0x1000,
                                    0x39000,	0x39FFF,	0x2700,	0x1000,
                                    0x3A000,	0x3AFFF,	0x2720,	0x1000,
                                    0x3B000,	0x3BFFF,	0x2740,	0x1000,
                                    0x3C000,	0x3CFFF,	0x2760,	0x1000,
                                    0x3D000,	0x3DFFF,	0x2780,	0x1000,
                                    0x3E000,	0x3EFFF,	0x27A0,	0x1000,
                                    0x3F000,	0x3FFFF,	0x27C0,	0x1000,
                                    0x40000,	0x40FFF,	0x27E0,	0x1000,
                                    0x41000,	0x41FFF,	0x2800,	0x1000,
                                    0x42000,	0x42FFF,	0x2820,	0x1000,
                                    0x43000,	0x43FFF,	0x2840,	0x1000,
                                    0x44000,	0x44FFF,	0x2860,	0x1000,
                                    0x45000,	0x45FFF,	0x2880,	0x1000,
                                    0x46000,	0x46FFF,	0x28A0,	0x1000,
                                    0x47000,	0x47FFF,	0x28C0,	0x1000,
                                    0x48000,	0x48FFF,	0x28E0,	0x1000,
                                    0x49000,	0x49FFF,	0x2900,	0x1000,
                                    0x4A000,	0x4AFFF,	0x2920,	0x1000,
                                    0x4B000,	0x4BFFF,	0x2940,	0x1000,
                                    0x4C000,	0x4CFFF,	0x2960,	0x1000,
                                    0x4D000,	0x4DFFF,	0x2980,	0x1000,
                                    0x4E000,	0x4EFFF,	0x29A0,	0x1000,
                                    0x4F000,	0x4FFFF,	0x29C0,	0x1000,
                                    0x50000,	0x50FFF,	0x29E0,	0x1000,
                                    0x51000,	0x51FFF,	0x2A00,	0x1000,
                                    0x52000,	0x52FFF,	0x2A20,	0x1000,
                                    0x53000,	0x53FFF,	0x2A40,	0x1000,
                                    0x54000,	0x54FFF,	0x2A60,	0x1000,
                                    0x55000,	0x55FFF,	0x2A80,	0x1000,
                                    0x56000,	0x56FFF,	0x2AA0,	0x1000,
                                    0x57000,	0x57FFF,	0x2AC0,	0x1000,
                                    0x58000,	0x58FFF,	0x2AE0,	0x1000,
                                    0x59000,	0x59FFF,	0x2B00,	0x1000,
                                    0x5A000,	0x5AFFF,	0x2B20,	0x1000,
                                    0x5B000,	0x5BFFF,	0x2B40,	0x1000,
                                    0x5C000,	0x5CFFF,	0x2B60,	0x1000,
                                    0x5D000,	0x5DFFF,	0x2B80,	0x1000,
                                    0x5E000,	0x5EFFF,	0x2BA0,	0x1000,
                                    0x5F000,	0x5FFFF,	0x2BC0,	0x1000,
                                    0x60000,	0x60FFF,	0x2BE0,	0x1000,
                                    0x61000,	0x61FFF,	0x2C00,	0x1000,
                                    0x62000,	0x62FFF,	0x2C20,	0x1000,
                                    0x63000,	0x63FFF,	0x2C40,	0x1000,
                                    0x64000,	0x64FFF,	0x2C60,	0x1000,
                                    0x65000,	0x65FFF,	0x2C80,	0x1000,
                                    0x66000,	0x66FFF,	0x2CA0,	0x1000,
                                    0x67000,	0x67FFF,	0x2CC0,	0x1000,
                                    0x68000,	0x68FFF,	0x2CE0,	0x1000,
                                    0x69000,	0x69FFF,	0x2D00,	0x1000,
                                    0x6A000,	0x6AFFF,	0x2D20,	0x1000,
                                 };
            if (savegame == 1)
            {
                for (int i = 0; i < hashtabledata.Length / 4; i++)
                {
                    hashtabledata[i * 4 + 0] += 0x7F000;
                    hashtabledata[i * 4 + 1] += 0x7F000;
                    hashtabledata[i * 4 + 2] += 0x7F000;
                }

                // Problem with save2 saves is that 0x3000-0x4FFF doesn't use save2 data. Probably different when hashed, but different when stored.
                for (int i = 2; i < 4; i++)
                {
                    hashtabledata[i * 4 + 0] -= 0x7F000;
                    hashtabledata[i * 4 + 1] -= 0x7F000;
                }
            }
            #endregion
            SHA256 mySHA256 = SHA256.Create();

            // Hash for 0x3000 onwards
            for (int i = 2; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                Array.Copy(hashValue, 0, savefile, offset, 0x20);
            }
            // Fix 2nd Hash
            {
                uint start = hashtabledata[0 + 4 * 1];
                uint length = hashtabledata[1 + 4 * 1] - hashtabledata[0 + 4 * 1];
                uint offset = hashtabledata[2 + 4 * 1];
                uint blocksize = hashtabledata[3 + 4 * 1];

                byte[] zeroarray = new byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                Array.Copy(hashValue, 0, savefile, offset, 0x20);
            }
            // Fix 1st Hash
            {
                uint start = hashtabledata[0 + 4 * 0];
                uint length = hashtabledata[1 + 4 * 0] - hashtabledata[0 + 4 * 0];
                uint offset = hashtabledata[2 + 4 * 0];
                uint blocksize = hashtabledata[3 + 4 * 0];

                byte[] zeroarray = new byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                Array.Copy(hashValue, 0, savefile, offset, 0x20);
            }
            // Fix IVFC Hash
            {
                byte[] zeroarray = new byte[0x200];
                Array.Copy(savefile, 0x2000 + savegame * 0x7F000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                Array.Copy(hashValue, 0, savefile, 0x43C - savegame * 0x130, 0x20);
            }
            // Fix DISA Hash
            {
                byte[] difihash = new byte[0x12C];
                Array.Copy(savefile, 0x330 - savegame * 0x130, difihash, 0, 0x12C);
                byte[] hashValue = mySHA256.ComputeHash(difihash);

                Array.Copy(hashValue, 0, savefile, 0x16C, 0x20);
            }
            return savefile;
        }

        // Data Requests
        internal static Image getSprite(int species, int form, int gender, int item, bool isegg, bool shiny)
        {
            string file;
            if (species == 0)
            { return (Image)Properties.Resources.ResourceManager.GetObject("_0"); }
            {
                file = "_" + species;
                if (form > 0) // Alt Form Handling
                    file = file + "_" + form;
                else if (gender == 1 && (species == 592 || species == 593)) // Frillish & Jellicent
                    file = file + "_" + gender;
                else if (gender == 1 && (species == 521 || species == 668)) // Unfezant & Pyroar
                    file = "_" + species + "f";
            }

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
            return pk6.Sprite;
        }
        internal static Image getSprite(byte[] data)
        {
            return new PK6(data).Sprite;
        }

        internal static string[] getShowdownText(string Nickname, int Species, int HeldItem, int Ability, int[] EVs,
            int HPType, int Move1, int Move2, int Move3, int Move4)
        {
            // TODO
            return null;
        }

        // Font Related
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        internal static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        internal static PrivateFontCollection s_FontCollection = new PrivateFontCollection();
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
                byte[] fontData = Properties.Resources.PGLDings_NormalRegular;
                IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                s_FontCollection.AddMemoryFont(fontPtr, Properties.Resources.PGLDings_NormalRegular.Length); uint dummy = 0;
                AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.PGLDings_NormalRegular.Length, IntPtr.Zero, ref dummy);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            }
            catch { Util.Error("Unable to add ingame font."); }
        }

        // Personal.dat
        internal static PersonalParser PersonalGetter = new PersonalParser();
        internal class PersonalParser
        {
            internal byte[] file = Properties.Resources.personal;
            internal const int EntryLength = 0xE;
            internal struct Personal
            {
                public byte[] BaseStats;
                public byte[] Abilities;
                public byte BaseFriendship;
                public byte GenderRatio;
                public byte EXPGrowth;
                public byte AltFormCount;
                public byte FormPointer; // 721 + FormPointer + (FormID - 1) = SpeciesIndex           
            }

            internal Personal GetPersonal(int species)
            {
                Personal data = new Personal();
                byte[] MonData = new byte[EntryLength];
                data.BaseStats = new byte[6];
                data.Abilities = new byte[3];
                Array.Copy(file, species * EntryLength, MonData, 0, EntryLength);
                Array.Copy(MonData, data.BaseStats, 6);
                Array.Copy(MonData, 6, data.Abilities, 0, 3);
                data.BaseFriendship = MonData[0x9];
                data.GenderRatio = MonData[0xA];
                data.EXPGrowth = MonData[0xB];
                data.AltFormCount = MonData[0xC];
                data.FormPointer = MonData[0xD];
                return data;
            }
            internal Personal GetPersonal(int species, int formID)
            {
                Personal data = GetPersonal(species);
                if (formID <= 0 || formID > data.AltFormCount || data.AltFormCount <= 0 || data.FormPointer <= 0)
                    return data;

                // Working with an Alt Forme with a base stat change
                return GetPersonal(721 + --formID + data.FormPointer);
            }
        }
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

        /// <summary>
        /// Calculate the Hidden Power Type of the entered IVs.
        /// </summary>
        /// <param name="ivs">HP/ATK/DEF/SPEED/SPA/SPD</param>
        /// <returns></returns>
        internal static int getHPType(int[] ivs)
        {
            return (15 * ((ivs[0] & 1) + 2 * (ivs[1] & 1) + 4 * (ivs[2] & 1) + 8 * (ivs[3] & 1) + 16 * (ivs[4] & 1) + 32 * (ivs[5] & 1))) / 63;
        }
        internal static int[] setHPIVs(int type, int[] ivs)
        {
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + hpivs[type][i];
            return ivs;
        }
        internal static string[] hptypes = { 
            "Fighting", "Flying", "Poison", "Ground",
            "Rock", "Bug", "Ghost", "Steel", "Fire", "Water",
            "Grass", "Electric", "Psychic", "Ice", "Dragon", "Dark"
        };
        internal static readonly int[][] hpivs = {
            new[] { 1, 1, 0, 0, 0, 0 }, // Fighting
            new[] { 0, 0, 0, 0, 0, 1 }, // Flying
            new[] { 1, 1, 0, 0, 0, 1 }, // Poison
            new[] { 1, 1, 1, 0, 0, 1 }, // Ground
            new[] { 1, 1, 0, 1, 0, 0 }, // Rock
            new[] { 1, 0, 0, 1, 0, 1 }, // Bug
            new[] { 1, 0, 1, 1, 0, 1 }, // Ghost
            new[] { 1, 1, 1, 1, 0, 1 }, // Steel
            new[] { 1, 0, 1, 0, 1, 0 }, // Fire
            new[] { 1, 0, 0, 0, 1, 1 }, // Water
            new[] { 1, 0, 1, 0, 1, 1 }, // Grass
            new[] { 1, 1, 1, 0, 1, 1 }, // Electric
            new[] { 1, 0, 1, 1, 1, 0 }, // Psychic
            new[] { 1, 0, 0, 1, 1, 1 }, // Ice
            new[] { 1, 0, 1, 1, 1, 1 }, // Dragon
            new[] { 1, 1, 1, 1, 1, 1 }, // Dark
        };
        public class Simulator
        {
            public struct Set
            {
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
                public byte[] EVs;
                public int[] IVs;
                public int[] Moves;

                // Parsing Utility
                public Set(string input, string[] species, string[] items, string[] natures, string[] moves, string[] abilities)
                {
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
                    EVs = new byte[6];
                    IVs = new[] { 31, 31, 31, 31, 31, 31 };
                    Moves = new int[4];
                    string[] stats =  { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };

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
                            continue;
                        }

                        string[] brokenline = line.Split(new[] { ": " }, StringSplitOptions.None);
                        switch (brokenline[0])
                        {
                            case "Trait":
                            case "Ability": { Ability = Array.IndexOf(abilities, brokenline[1]); break; }
                            case "Level": { Level = Util.ToInt32(brokenline[1]); break; }
                            case "Shiny": { Shiny = (brokenline[1] == "Yes"); break; }
                            case "Happiness": { Friendship = Util.ToInt32(brokenline[1]); break; }
                            case "EVs":
                                {
                                    // Get EV list String
                                    string[] evlist = brokenline[1].Replace("SAtk", "SpA").Replace("SDef", "SpD").Replace("Spd", "Spe").Split(new[] { " / ", " " }, StringSplitOptions.None);
                                    for (int i = 0; i < evlist.Length / 2; i++)
                                        EVs[Array.IndexOf(stats, evlist[1 + i * 2])] = (byte)Util.ToInt32(evlist[0 + 2 * i]);
                                    break;
                                }
                            case "IVs":
                                {
                                    // Get IV list String
                                    string[] ivlist = brokenline[1].Split(new[] { " / ", " " }, StringSplitOptions.None);
                                    for (int i = 0; i < ivlist.Length / 2; i++)
                                        IVs[Array.IndexOf(stats, ivlist[1 + i * 2])] = (byte)Util.ToInt32(ivlist[0 + 2 * i]);
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
            }
        }
    }
}