using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Drawing.Text;

namespace PKHeX
{
    public partial class PKX
    {
        // C# PKX Function Library
        // No WinForm object related code, only to calculate information.
        // May require re-referencing to main form for string array referencing.
        // Relies on Util for some common operations.

        // Data
        internal static uint LCRNG(uint seed)
        {
            uint a = 0x41C64E6D;
            uint c = 0x00006073;

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }
        internal static uint LCRNG(ref uint seed)
        {
            uint a = 0x41C64E6D;
            uint c = 0x00006073;

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
	            05, 
            };
            if (move < 0) { move = 0; }
            return (byte)movepptable[move];
        }
        internal static byte[] getRandomEVs()
        {
            byte[] evs = new byte[6] { 0xDE, 0xAD, 0xBE, 0xEF, 0xBA, 0xBE, }; // ha ha, just to start off above 510!

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
        internal static int getLevel(int species, ref uint exp)
        {
            if (exp == 0) { return 1; }

            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int growth = MonData.EXPGrowth;
            DataTable table = PKX.ExpTable();

            // Iterate upwards to find the level above our current level
            int tl = 0; // Initial Level, immediately incremented before loop.
            while ((uint)table.Rows[++tl][growth + 1] <= exp)
            {
                if (tl == 100)
                {
                    exp = getEXP(100, species); // Fix EXP
                    return 100;
                }
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

            uint exp = (uint)PKX.ExpTable().Rows[level][growth + 1];
            return exp;
        }
        internal static byte[] getAbilities(int species, int formnum)
        {
            return PersonalGetter.GetPersonal(species, formnum).Abilities;
        }
        internal static int getGender(string s)
        {
            if (s == "♂" || s == "M")
                return 0;
            else if (s == "♀" || s == "F")
                return 1;
            else return 2;
        }
        internal static string[] getCountryRegionText(int country, int region, string lang)
        {
            // Get Language we're fetching for
            int index = Array.IndexOf(new string[] { "ja", "en", "fr", "de", "it", "es", "zh", "ko", }, lang);
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
                        if (countryData.Length > 1)
                        {
                            indexes[i - 1] = Convert.ToInt32(countryData[0]);
                            unsortedList[i - 1] = countryData[index + 1];
                        }
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
                        if (countryData.Length > 1)
                        {
                            indexes[i - 1] = Convert.ToInt32(countryData[0]);
                            unsortedList[i - 1] = countryData[index + 1];
                        }
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
            string loctext = "";
            if (gameorigin < 13 && gameorigin > 6 && eggmet)
            {
                if (locval < 2000)
                    loctext = Form1.metHGSS_00000[locval];
                else if (locval < 3000)
                    loctext = Form1.metHGSS_02000[locval % 2000];
                else
                    loctext = Form1.metHGSS_03000[locval % 3000];
            }
            else if (gameorigin < 24)
            {
                if (locval < 30000)
                    loctext = Form1.metBW2_00000[locval];
                else if (locval < 40000)
                    loctext = Form1.metBW2_30000[locval % 10000 - 1];
                else if (locval < 60000)
                    loctext = Form1.metBW2_40000[locval % 10000 - 1];
                else
                    loctext = Form1.metBW2_60000[locval % 10000 - 1];
            }
            else if (gameorigin > 23)
            {
                if (locval < 30000)
                    loctext = Form1.metXY_00000[locval];
                else if (locval < 40000)
                    loctext = Form1.metXY_30000[locval % 10000 - 1];
                else if (locval < 60000)
                    loctext = Form1.metXY_40000[locval % 10000 - 1];
                else
                    loctext = Form1.metXY_60000[locval % 10000 - 1];
            }
            return loctext;
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
            stats[0] = (ushort)((((HP_IV + (2 * HP_B) + (HP_EV / 4) + 100) * level) / 100) + 10);
            stats[1] = (ushort)((((ATK_IV + (2 * ATK_B) + (ATK_EV / 4)) * level) / 100) + 5);
            stats[2] = (ushort)((((DEF_IV + (2 * DEF_B) + (DEF_EV / 4)) * level) / 100) + 5);
            stats[4] = (ushort)((((SPA_IV + (2 * SPA_B) + (SPA_EV / 4)) * level) / 100) + 5);
            stats[5] = (ushort)((((SPD_IV + (2 * SPD_B) + (SPD_EV / 4)) * level) / 100) + 5);
            stats[3] = (ushort)((((SPE_IV + (2 * SPE_B) + (SPE_EV / 4)) * level) / 100) + 5);

            // Account for nature
            int incr = nature / 5 + 1;
            int decr = nature % 5 + 1;
            if (incr != decr)
            {
                stats[incr] *= 11; stats[incr] /= 10;
                stats[decr] *= 9; stats[decr] /= 10;
            }

            // Return Result
            return stats;
        }

        // Manipulation
        internal static byte[] shuffleArray(byte[] pkx, uint sv)
        {
            byte[] ekx = new byte[260];
            Array.Copy(pkx, ekx, 8);

            // Now to shuffle the blocks

            // Define Shuffle Order Structure
            byte[] aloc = new byte[] { 0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3 };
            byte[] bloc = new byte[] { 1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2 };
            byte[] cloc = new byte[] { 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1 };
            byte[] dloc = new byte[] { 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0 };

            // Get Shuffle Order
            byte[] shlog = new byte[] { aloc[sv], bloc[sv], cloc[sv], dloc[sv] };

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
                Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ (PKX.LCRNG(ref seed) >> 16))), 0, pkx, i, 2);

            // Deshuffle
            pkx = shuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            if (pkx.Length > 232)
                for (int i = 232; i < 260; i += 2)
                    Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ (PKX.LCRNG(ref seed) >> 16))), 0, pkx, i, 2);

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
                Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekx, i) ^ (PKX.LCRNG(ref seed) >> 16))), 0, ekx, i, 2);

            // Encrypt the Party Stats
            seed = pv;
            if (ekx.Length > 232)
                for (int i = 232; i < 260; i += 2)
                    Array.Copy(BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekx, i) ^ (PKX.LCRNG(ref seed) >> 16))), 0, ekx, i, 2);

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
            else
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
            uint pid = (uint)Util.rnd32();
            if (gt == 255) //Genderless
                return pid;
            if (gt == 254) //Female Only
                gt++;
            while (true) // Loop until we find a suitable PID
            {
                uint gv = (pid & 0xFF);
                if (cg == 2) // Genderless
                    break;  // PID Passes
                else if ((cg == 1) && (gv <= gt)) // Female
                    break;  // PID Passes
                else if ((cg == 0) && (gv > gt))
                    break;  // PID Passes
                pid = (uint)Util.rnd32();
            }
            return pid;
        }

        // Object
        #region PKX Object
        private Image pksprite;
        private uint mEC, mPID, mIV32,

            mexp,
            mHP_EV, mATK_EV, mDEF_EV, mSPA_EV, mSPD_EV, mSPE_EV,
            mHP_IV, mATK_IV, mDEF_IV, mSPE_IV, mSPA_IV, mSPD_IV,
            mcnt_cool, mcnt_beauty, mcnt_cute, mcnt_smart, mcnt_tough, mcnt_sheen,
            mmarkings, mhptype;

        private string
            slot,
            mnicknamestr, mgenderstring, mnotOT, mot, mSpeciesName, mNatureName, mHPName, mAbilityName,
            mMove1N, mMove2N, mMove3N, mMove4N, mhelditemN,
            mRMove1N, mRMove2N, mRMove3N, mRMove4N,
            mMetLocN, mEggLocN,
            mcountryID, mregionID, mGameN, mBallN, mdsregIDN, motlangN;

        private int
            mability, mabilitynum, mnature, mgenderflag, maltforms, mPKRS_Strain, mPKRS_Duration,
            mmetlevel, motgender;

        private bool
            misegg, misnick, misshiny, mfeflag;

        private ushort
            mhelditem, mspecies, mTID, mSID, mTSV, mESV,
            mmove1, mmove2, mmove3, mmove4,
            mmove1_pp, mmove2_pp, mmove3_pp, mmove4_pp,
            mmove1_ppu, mmove2_ppu, mmove3_ppu, mmove4_ppu,
            meggmove1, meggmove2, meggmove3, meggmove4,
            mchk,

            mOTfriendship, mOTaffection,
            megg_year, megg_month, megg_day,
            mmet_year, mmet_month, mmet_day,
            meggloc, mmetloc,
            mball, mencountertype,
            mgamevers, mdsregID, motlang;

        public string Position { get { return slot; } }
        public Image Sprite { get { return pksprite; } }
        public string Nickname { get { return mnicknamestr; } }
        public string Species { get { return mSpeciesName; } }
        public string Nature { get { return mNatureName; } }
        public string Gender { get { return mgenderstring; } }
        public string ESV { get { return mESV.ToString("0000"); } }
        public string HP_Type { get { return mHPName; } }
        public string Ability { get { return mAbilityName; } }
        public string Move1 { get { return mMove1N; } }
        public string Move2 { get { return mMove2N; } }
        public string Move3 { get { return mMove3N; } }
        public string Move4 { get { return mMove4N; } }
        public string HeldItem { get { return mhelditemN; } }
        public string MetLoc { get { return mMetLocN; } }
        public string EggLoc { get { return mEggLocN; } }
        public string Ball { get { return mBallN; } }
        public string OT { get { return mot; } }
        public string Version { get { return mGameN; } }
        public string OTLang { get { return motlangN; } }
        public string CountryID { get { return mcountryID; } }
        public string RegionID { get { return mregionID; } }
        public string DSRegionID { get { return mdsregIDN; } }

        #region Extraneous
        public string EC { get { return mEC.ToString("X8"); } }
        public string PID { get { return mPID.ToString("X8"); } }
        public uint HP_IV { get { return mHP_IV; } }
        public uint ATK_IV { get { return mATK_IV; } }
        public uint DEF_IV { get { return mDEF_IV; } }
        public uint SPA_IV { get { return mSPA_IV; } }
        public uint SPD_IV { get { return mSPD_IV; } }
        public uint SPE_IV { get { return mSPE_IV; } }
        public uint EXP { get { return mexp; } }
        public uint HP_EV { get { return mHP_EV; } }
        public uint ATK_EV { get { return mATK_EV; } }
        public uint DEF_EV { get { return mDEF_EV; } }
        public uint SPA_EV { get { return mSPA_EV; } }
        public uint SPD_EV { get { return mSPD_EV; } }
        public uint SPE_EV { get { return mSPE_EV; } }
        public uint Cool { get { return mcnt_cool; } }
        public uint Beauty { get { return mcnt_beauty; } }
        public uint Cute { get { return mcnt_cute; } }
        public uint Smart { get { return mcnt_smart; } }
        public uint Tough { get { return mcnt_tough; } }
        public uint Sheen { get { return mcnt_sheen; } }
        public uint Markings { get { return mmarkings; } }

        public string NotOT { get { return mnotOT; } }

        public int AbilityNum { get { return mabilitynum; } }
        public int GenderFlag { get { return mgenderflag; } }
        public int AltForms { get { return maltforms; } }
        public int PKRS_Strain { get { return mPKRS_Strain; } }
        public int PKRS_Days { get { return mPKRS_Duration; } }
        public int MetLevel { get { return mmetlevel; } }
        public int OT_Gender { get { return motgender; } }

        public bool FatefulFlag { get { return mfeflag; } }
        public bool IsEgg { get { return misegg; } }
        public bool IsNicknamed { get { return misnick; } }
        public bool IsShiny { get { return misshiny; } }

        public ushort TID { get { return mTID; } }
        public ushort SID { get { return mSID; } }
        public ushort TSV { get { return mTSV; } }
        public ushort Move1_PP { get { return mmove1_pp; } }
        public ushort Move2_PP { get { return mmove2_pp; } }
        public ushort Move3_PP { get { return mmove3_pp; } }
        public ushort Move4_PP { get { return mmove4_pp; } }
        public ushort Move1_PPUp { get { return mmove1_ppu; } }
        public ushort Move2_PPUp { get { return mmove2_ppu; } }
        public ushort Move3_PPUp { get { return mmove3_ppu; } }
        public ushort Move4_PPUp { get { return mmove4_ppu; } }
        public string Relearn1 { get { return mRMove1N; } }
        public string Relearn2 { get { return mRMove2N; } }
        public string Relearn3 { get { return mRMove3N; } }
        public string Relearn4 { get { return mRMove4N; } }
        public ushort Checksum { get { return mchk; } }
        public ushort mFriendship { get { return mOTfriendship; } }
        public ushort OT_Affection { get { return mOTaffection; } }
        public ushort Egg_Year { get { return megg_year; } }
        public ushort Egg_Day { get { return megg_month; } }
        public ushort Egg_Month { get { return megg_day; } }
        public ushort Met_Year { get { return mmet_year; } }
        public ushort Met_Day { get { return mmet_month; } }
        public ushort Met_Month { get { return mmet_day; } }
        public ushort Encounter { get { return mencountertype; } }

        #endregion
        public PKX(byte[] pkx, string ident)
        {
            slot = ident;
            mnicknamestr = "";
            mnotOT = "";
            mot = "";
            mEC = BitConverter.ToUInt32(pkx, 0);
            mchk = BitConverter.ToUInt16(pkx, 6);
            mspecies = BitConverter.ToUInt16(pkx, 0x08);
            mhelditem = BitConverter.ToUInt16(pkx, 0x0A);
            mTID = BitConverter.ToUInt16(pkx, 0x0C);
            mSID = BitConverter.ToUInt16(pkx, 0x0E);
            mexp = BitConverter.ToUInt32(pkx, 0x10);
            mability = pkx[0x14];
            mabilitynum = pkx[0x15];
            // 0x16, 0x17 - Training bag
            mPID = BitConverter.ToUInt32(pkx, 0x18);
            mnature = pkx[0x1C];
            mfeflag = (pkx[0x1D] % 2) == 1;
            mgenderflag = (pkx[0x1D] >> 1) & 0x3;
            maltforms = (pkx[0x1D] >> 3);
            mHP_EV = pkx[0x1E];
            mATK_EV = pkx[0x1F];
            mDEF_EV = pkx[0x20];
            mSPA_EV = pkx[0x22];
            mSPD_EV = pkx[0x23];
            mSPE_EV = pkx[0x21];
            mcnt_cool = pkx[0x24];
            mcnt_beauty = pkx[0x25];
            mcnt_cute = pkx[0x26];
            mcnt_smart = pkx[0x27];
            mcnt_tough = pkx[0x28];
            mcnt_sheen = pkx[0x29];
            mmarkings = pkx[0x2A];
            mPKRS_Strain = pkx[0x2B] >> 4;
            mPKRS_Duration = pkx[0x2B] % 0x10;

            // Block B
            mnicknamestr = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0x40, 24));
            // 0x58, 0x59 - unused
            mmove1 = BitConverter.ToUInt16(pkx, 0x5A);
            mmove2 = BitConverter.ToUInt16(pkx, 0x5C);
            mmove3 = BitConverter.ToUInt16(pkx, 0x5E);
            mmove4 = BitConverter.ToUInt16(pkx, 0x60);
            mmove1_pp = pkx[0x62];
            mmove2_pp = pkx[0x63];
            mmove3_pp = pkx[0x64];
            mmove4_pp = pkx[0x65];
            mmove1_ppu = pkx[0x66];
            mmove2_ppu = pkx[0x67];
            mmove3_ppu = pkx[0x68];
            mmove4_ppu = pkx[0x69];
            meggmove1 = BitConverter.ToUInt16(pkx, 0x6A);
            meggmove2 = BitConverter.ToUInt16(pkx, 0x6C);
            meggmove3 = BitConverter.ToUInt16(pkx, 0x6E);
            meggmove4 = BitConverter.ToUInt16(pkx, 0x70);

            // 0x72 - Super Training Flag - Passed with pkx to new form

            // 0x73 - unused/unknown
            mIV32 = BitConverter.ToUInt32(pkx, 0x74);
            mHP_IV = mIV32 & 0x1F;
            mATK_IV = (mIV32 >> 5) & 0x1F;
            mDEF_IV = (mIV32 >> 10) & 0x1F;
            mSPE_IV = (mIV32 >> 15) & 0x1F;
            mSPA_IV = (mIV32 >> 20) & 0x1F;
            mSPD_IV = (mIV32 >> 25) & 0x1F;
            misegg = Convert.ToBoolean((mIV32 >> 30) & 1);
            misnick = Convert.ToBoolean((mIV32 >> 31));

            // Block C
            mnotOT = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0x78, 24));
            bool notOTG = Convert.ToBoolean(pkx[0x92]);
            // Memory Editor edits everything else with pkx in a new form

            // Block D
            mot = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0xB0, 24));
            // 0xC8, 0xC9 - unused
            mOTfriendship = pkx[0xCA];
            mOTaffection = pkx[0xCB]; // Handled by Memory Editor
            // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0
            megg_year = pkx[0xD1];
            megg_month = pkx[0xD2];
            megg_day = pkx[0xD3];
            mmet_year = pkx[0xD4];
            mmet_month = pkx[0xD5];
            mmet_day = pkx[0xD6];
            // 0xD7 - unused
            meggloc = BitConverter.ToUInt16(pkx, 0xD8);
            mmetloc = BitConverter.ToUInt16(pkx, 0xDA);
            mball = pkx[0xDC];
            mmetlevel = pkx[0xDD] & 0x7F;
            motgender = (pkx[0xDD]) >> 7;
            mencountertype = pkx[0xDE];
            mgamevers = pkx[0xDF];
            string[] data = getCountryRegionText(pkx[0xE0], pkx[0xE1], Form1.curlanguage);
            mcountryID = data[0x0];
            mregionID = data[0x1];
            mdsregID = pkx[0xE2];
            motlang = pkx[0xE3];

            if (mgenderflag == 0)
                mgenderstring = "♂";
            else if (mgenderflag == 1)
                mgenderstring = "♀";
            else
                mgenderstring = "-";

            mhptype = (15 * ((mHP_IV & 1) + 2 * (mATK_IV & 1) + 4 * (mDEF_IV & 1) + 8 * (mSPE_IV & 1) + 16 * (mSPA_IV & 1) + 32 * (mSPD_IV & 1))) / 63 + 1;

            mTSV = (ushort)((mTID ^ mSID) >> 4);
            mESV = (ushort)(((mPID >> 16) ^ (mPID & 0xFFFF)) >> 4);

            misshiny = (mTSV == mESV);
            // Nidoran Gender Fixing Text
            {
                mnicknamestr = Regex.Replace(mnicknamestr, "\uE08F", "\u2640");
                mnicknamestr = Regex.Replace(mnicknamestr, "\uE08E", "\u2642");
            }
            {
                int species = BitConverter.ToInt16(pkx, 0x08); // Get Species
                uint isegg = (BitConverter.ToUInt32(pkx, 0x74) >> 30) & 1;

                int altforms = (pkx[0x1D] >> 3);
                int gender = (pkx[0x1D] >> 1) & 0x3;

                string file;
                {
                    file = "_" + species.ToString();
                    if (altforms > 0) // Alt Form Handling
                        file = file + "_" + altforms.ToString();
                    else if ((species == 521) && (gender == 1))   // Unfezant
                        file = "_" + species.ToString() + "f";
                }
                if (species == 0)
                    file = "_0";

                pksprite = (Image)Properties.Resources.ResourceManager.GetObject(file);

                if (misegg)
                {
                    // Start with a partially transparent species by layering the species with partial opacity onto a blank image.
                    pksprite = Util.LayerImage((Image)Properties.Resources.ResourceManager.GetObject("_0"), pksprite, 0, 0, 0.33);
                    // Add the egg layer over-top with full opacity.
                    pksprite = Util.LayerImage(pksprite, (Image)Properties.Resources.ResourceManager.GetObject("egg"), 0, 0, 1);
                }
                if (misshiny)
                {   // Is Shiny
                    // Redraw our image
                    pksprite = Util.LayerImage(pksprite, Properties.Resources.rare_icon, 0, 0, 0.7);
                }
                if (mhelditem > 0)
                {
                    // Has Item
                    int item = mhelditem;
                    Image itemimg = (Image)Properties.Resources.ResourceManager.GetObject("item_" + item.ToString());
                    if (itemimg == null) itemimg = Properties.Resources.helditem;
                    // Redraw
                    pksprite = Util.LayerImage(pksprite, itemimg, 22 + (15 - itemimg.Width) / 2, 15 + (15 - itemimg.Height), 1);
                }
            }
            try
            {
                mSpeciesName = Form1.specieslist[mspecies];
                mhelditemN = Form1.itemlist[mhelditem];
                mNatureName = Form1.natures[mnature];
                mHPName = Form1.types[mhptype];
                mAbilityName = Form1.abilitylist[mability];
                mMove1N = Form1.movelist[mmove1];
                mMove2N = Form1.movelist[mmove2];
                mMove3N = Form1.movelist[mmove3];
                mMove4N = Form1.movelist[mmove4];
                mRMove1N = Form1.movelist[meggmove1];
                mRMove2N = Form1.movelist[meggmove2];
                mRMove3N = Form1.movelist[meggmove3];
                mRMove4N = Form1.movelist[meggmove4];
                mMetLocN = PKX.getLocation(false, mgamevers, mmetloc);
                mEggLocN = PKX.getLocation(true, mgamevers, meggloc);
                mGameN = Form1.gamelist[mgamevers];
                mBallN = Form1.balllist[mball];
                motlangN = Form1.gamelanguages[motlang];
                mdsregIDN = Form1.consoleregions[mdsregID];
            }
            catch { return; }
        }
        #endregion
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
                public int Box, TrainerCard, Party, BattleBox, GTS, Daycare, Fused, SUBE, Puff, Item, Trainer1, Trainer2, PCLayout, Wondercard, BerryField, OPower, EventFlag, PokeDex, HoF, PSS, JPEG;
                public string Name;
                public SaveGame(string GameID)
                {
                    if (GameID == "XY")
                    {
                        Name = "XY";
                        Box = 0x27A00;
                        TrainerCard = 0x19400;
                        Party = 0x19600;
                        BattleBox = 0x09E00;
                        Daycare = 0x20600;
                        GTS = 0x1CC00;
                        Fused = 0x1B400;
                        SUBE = 0x22C90;

                        Puff = 0x5400;
                        Item = 0x5800;
                        Trainer1 = 0x6800;
                        Trainer2 = 0x9600;
                        PCLayout = 0x9800;
                        Wondercard = 0x21000;
                        BerryField = 0x20C00;
                        OPower = 0x1BE00;
                        EventFlag = 0x19E00;
                        PokeDex = 0x1A400;

                        HoF = 0x1E800;
                        JPEG = 0x5C600;
                        PSS = 0x0A400;
                    }
                    else if (GameID == "ORAS")
                    {
                        // Temp
                        Name = "ORAS";
                        Box = 0x38400;      // Confirmed
                        TrainerCard = 0x19400; // Confirmed
                        Party = 0x19600;    // Confirmed
                        BattleBox = 0x09E00;// Confirmed
                        Daycare = 0x21000; // Confirmed (thanks Rei)
                        GTS = 0x1D600; // Confirmed
                        Fused = 0x1BE00; // Confirmed
                        SUBE = 0x22C90; // ****not in use, not updating?****

                        Puff = 0x5400; // Confirmed
                        Item = 0x5800; // Confirmed
                        Trainer1 = 0x6800; // Confirmed
                        Trainer2 = 0x9600; // Confirmed
                        PCLayout = 0x9800; // Confirmed
                        Wondercard = 0x22000; // Confirmed
                        BerryField = 0x20C00; // ****changed****
                        OPower = 0x1BE00;
                        EventFlag = 0x19E00; // Confirmed
                        PokeDex = 0x1A400;

                        HoF = 0x1F200; // Confirmed
                        JPEG = 0x6D000; // Confirmed
                        PSS = 0x0A400; // Confirmed (thanks Rei)
                    }
                    else
                    {
                        // Copied...
                        Name = "Unknown";
                        Box = 0x27A00;
                        TrainerCard = 0x19400;
                        Party = 0x19600;
                        BattleBox = 0x09E00;
                        Daycare = 0x20600;
                        GTS = 0x1CC00;
                        Fused = 0x1B400;
                        SUBE = 0x22C90;

                        Puff = 0x5400;
                        Item = 0x5800;
                        Trainer1 = 0x6800;
                        Trainer2 = 0x9600;
                        PCLayout = 0x9800;
                        Wondercard = 0x21000;
                        BerryField = 0x20C00;
                        OPower = 0x1BE00;
                        EventFlag = 0x19E00;
                        PokeDex = 0x1A400;

                        HoF = 0x1E800;
                        JPEG = 0x5C600;
                        PSS = 0x0A400;
                    }
                }
            }
        }
        #endregion

        // Save File Related
        internal static int detectSAVIndex(byte[] data, ref int savindex)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
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
            if ((data[0x168] ^ 1) != savindex && savindex != 2)
            {
                Console.WriteLine("ERROR: ACTIVE BLOCK MISMATCH");
                savindex = 2;
            }
            return savindex;
        }
        internal static ushort ccitt16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
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

        // Font Related
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        internal static Font getPKXFont(float fontsize)
        {
            byte[] fontData = Properties.Resources.PGLDings_NormalRegular;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            PrivateFontCollection fonts = new PrivateFontCollection();
            try
            {
                fonts.AddMemoryFont(fontPtr, Properties.Resources.PGLDings_NormalRegular.Length); uint dummy = 0;
                AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.PGLDings_NormalRegular.Length, IntPtr.Zero, ref dummy);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            }
            catch { Util.Error("Unable to add ingame font."); }

            return new Font(fonts.Families[0], fontsize);
        }

        // Personal.dat
        internal static PersonalParser PersonalGetter = new PersonalParser();
        internal class PersonalParser
        {
            internal byte[] file = (byte[])Properties.Resources.personal;
            internal int EntryLength = 0xE;
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
                if (formID > 0 && formID <= data.AltFormCount && data.AltFormCount > 0 && data.FormPointer > 0) // Working with an Alt Forme with a base stat change
                {
                    formID--;
                    data = GetPersonal(721 + formID + data.FormPointer);
                }
                return data;
            }
        }
    }
}