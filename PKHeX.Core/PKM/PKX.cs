using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Common logic for <see cref="PKM"/> data providing and manipulation.
    /// </summary>
    public static class PKX
    {
        private static readonly PersonalTable Personal = PersonalTable.SM;
        private const int Generation = 7;

        internal const int SIZE_1ULIST = 69;
        internal const int SIZE_1JLIST = 59;
        internal const int SIZE_1PARTY = 44;
        internal const int SIZE_1STORED = 33;

        internal const int SIZE_2ULIST = 73;
        internal const int SIZE_2JLIST = 63;
        internal const int SIZE_2PARTY = 48;
        internal const int SIZE_2STORED = 32;

        internal const int SIZE_3CSTORED = 312;
        internal const int SIZE_3XSTORED = 196;
        internal const int SIZE_3PARTY = 100;
        internal const int SIZE_3STORED = 80;
        internal const int SIZE_3BLOCK = 12;

        internal const int SIZE_4PARTY = 236;
        internal const int SIZE_4STORED = 136;
        internal const int SIZE_4BLOCK = 32;

        internal const int SIZE_5PARTY = 220;
        internal const int SIZE_5STORED = 136;
        internal const int SIZE_5BLOCK = 32;

        internal const int SIZE_6PARTY = 0x104;
        internal const int SIZE_6STORED = 0xE8;
        internal const int SIZE_6BLOCK = 56;

        /// <summary>
        /// Determines if the given length is valid for a <see cref="PKM"/>.
        /// </summary>
        /// <param name="len">Data length of the file/array.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not the length is valid for a <see cref="PKM"/>.</returns>
        public static bool IsPKM(long len)
        {
            return new[]
            {
                SIZE_1JLIST, SIZE_1ULIST,
                SIZE_2ULIST, SIZE_2JLIST,
                SIZE_3STORED, SIZE_3PARTY,
                SIZE_3CSTORED, SIZE_3XSTORED,
                SIZE_4STORED, SIZE_4PARTY,
                SIZE_5PARTY,
                SIZE_6STORED, SIZE_6PARTY
            }.Contains((int)len);
        }
        
        public static uint LCRNG(uint seed) => RNG.LCRNG.Next(seed);
        public static uint LCRNG(ref uint seed) => seed = RNG.LCRNG.Next(seed);
        #region ExpTable
        private static readonly uint[,] ExpTable =
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

        /// <summary>
        /// Species name lists indexed by the <see cref="PKM.Language"/> value.
        /// </summary>
        public static readonly string[][] SpeciesLang = 
        {
            Util.GetSpeciesList("ja"), // 0 (unused, invalid)
            Util.GetSpeciesList("ja"), // 1
            Util.GetSpeciesList("en"), // 2
            Util.GetSpeciesList("fr"), // 3
            Util.GetSpeciesList("it"), // 4
            Util.GetSpeciesList("de"), // 5
            Util.GetSpeciesList("es"), // 6 (reserved for Gen3 KO?, unused)
            Util.GetSpeciesList("es"), // 7
            Util.GetSpeciesList("ko"), // 8
            Util.GetSpeciesList("zh"), // 9 Simplified
            Util.GetSpeciesList("zh2"), // 10 Traditional
        };

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="lang">Language ID of the Pokémon</param>
        /// <returns>The Species name if within expected range, else an empty string.</returns>
        public static string GetSpeciesName(int species, int lang)
        {
            if (lang < 0 || SpeciesLang.Length <= lang)
                return "";
            if (species < 0 || SpeciesLang[0].Length <= species)
                return "";

            return SpeciesLang[lang][species];
        }

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="lang">Language ID of the Pokémon</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Generation specific default species name</returns>
        public static string GetSpeciesNameGeneration(int species, int lang, int generation)
        {
            if (generation == 3 && species == 0)
                return "タマゴ";

            string nick = GetSpeciesName(species, lang);

            if (generation < 5 && (generation != 4 || species != 0)) // All caps GenIV and previous, except GenIV eggs.
                nick = nick.ToUpper();
            if (generation < 3)
                nick = nick.Replace(" ", "");
            return nick;
        }

        /// <summary>
        /// Checks if a nickname matches the species name of any language.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>True if it does not match any language name, False if not nicknamed</returns>
        public static bool IsNicknamedAnyLanguage(int species, string nick, int generation)
        {
            int len = SpeciesLang.Length;
            if (generation < 3)
                len = 3;
            else if (generation < 7)
                len = 9; // chinese (CHS/CHT) introduced in Gen7

            for (int i = 0; i < len; i++)
                if (GetSpeciesNameGeneration(species, i, generation) == nick)
                    return false;
            return true;
        }

        /// <summary>
        /// Gets the Species name Language ID for the current name and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
        public static int GetSpeciesNameLanguage(int species, string nick, int generation)
        {
            int len = SpeciesLang.Length;
            if (generation < 3)
                len = 3;
            else if (generation < 7)
                len = 8;

            for (int i = 0; i < len; i++)
                if (GetSpeciesNameGeneration(species, i, generation) == nick)
                    return i;
            return -1;
        }

        /// <summary>
        /// Gets randomized EVs for a given generation format
        /// </summary>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Array containing randomized EVs (H/A/B/S/C/D)</returns>
        public static uint[] GetRandomEVs(int generation = Generation)
        {
            if (generation > 2)
            {
                uint[] evs = new uint[6];
                do
                {
                    evs[0] = (byte)Math.Min(Util.Rand32() % 300, 252); // bias two to get maybe 252
                    evs[1] = (byte)Math.Min(Util.Rand32() % 300, 252);
                    evs[2] = (byte)Math.Min(Util.Rand32() % (510 - evs[0] - evs[1]), 252);
                    evs[3] = (byte)Math.Min(Util.Rand32() % (510 - evs[0] - evs[1] - evs[2]), 252);
                    evs[4] = (byte)Math.Min(Util.Rand32() % (510 - evs[0] - evs[1] - evs[2] - evs[3]), 252);
                    evs[5] = (byte)Math.Min(510 - evs[0] - evs[1] - evs[2] - evs[3] - evs[4], 252);
                } while (evs.Sum(b => b) > 510); // recalculate random EVs...
                Util.Shuffle(evs);
                return evs;
            }
            else
            {
                uint[] evs = new uint[6];
                for (int i = 0; i < evs.Length; i++)
                    evs[i] = Util.Rand32() & ushort.MaxValue;
                return evs;
            }
        }

        /// <summary>
        /// Gets the current level of a species.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <param name="exp">Experience points</param>
        /// <returns>Current level of the species.</returns>
        public static int GetLevel(int species, uint exp)
        {
            int growth = Personal[species].EXPGrowth;
            int tl = 1; // Initial Level. Iterate upwards to find the level
            while (ExpTable[++tl, growth] <= exp)
                if (tl == 100) return 100;
            return --tl;
        }

        /// <summary>
        /// Gets the minimum Experience points for the specified level.
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <returns>Experience points needed to have specified level.</returns>
        public static uint GetEXP(int level, int species)
        {
            if (level <= 1) return 0;
            if (level > 100) level = 100;
            return ExpTable[level, Personal[species].EXPGrowth];
        }

        /// <summary>
        /// Translates a Gender string to Gender integer.
        /// </summary>
        /// <param name="s">Gender string</param>
        /// <returns>Gender integer</returns>
        public static int GetGender(string s)
        {
            if (s == null) 
                return -1;
            if (s == "♂" || s == "M")
                return 0;
            if (s == "♀" || s == "F")
                return 1;
            return 2;
        }
        
        /// <summary>
        /// Positions for shuffling.
        /// </summary>
        private static readonly byte[][] blockPosition =
        {
            new byte[] {0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3},
            new byte[] {1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2},
            new byte[] {2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1},
            new byte[] {3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0},
        };

        /// <summary>
        /// Positions for unshuffling.
        /// </summary>
        internal static readonly byte[] blockPositionInvert =
        {
            0, 1, 2, 4, 3, 5, 6, 7, 12, 18, 13, 19, 8, 10, 14, 20, 16, 22, 9, 11, 15, 21, 17, 23
        };


        /// <summary>
        /// Shuffles a 232 byte array containing <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Data to shuffle</param>
        /// <param name="sv">Block Shuffle order</param>
        /// <returns>Shuffled byte array</returns>
        public static byte[] ShuffleArray(byte[] data, uint sv)
        {
            byte[] sdata = new byte[data.Length];
            Array.Copy(data, sdata, 8); // Copy unshuffled bytes

            // Shuffle Away!
            for (int block = 0; block < 4; block++)
                Array.Copy(data, 8 + 56*blockPosition[block][sv], sdata, 8 + 56*block, 56);

            // Fill the Battle Stats back
            if (data.Length > 232)
                Array.Copy(data, 232, sdata, 232, 28);

            return sdata;
        }

        /// <summary>
        /// Decrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="ekx">Encrypted <see cref="PKM"/> data.</param>
        /// <returns>Decrypted <see cref="PKM"/> data.</returns>
        /// <returns>Encrypted <see cref="PKM"/> data.</returns>
        public static byte[] DecryptArray(byte[] ekx)
        {
            byte[] pkx = (byte[])ekx.Clone();

            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (pv >> 0xD & 0x1F) % 24;

            uint seed = pv;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkx, i);

            // Deshuffle
            pkx = ShuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            if (pkx.Length <= 232) return pkx;
            for (int i = 232; i < 260; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkx, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkx, i);

            return pkx;
        }

        /// <summary>
        /// Encrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="pkx">Decrypted <see cref="PKM"/> data.</param>
        public static byte[] EncryptArray(byte[] pkx)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (pv >> 0xD & 0x1F) % 24;

            byte[] ekx = (byte[])pkx.Clone();

            ekx = ShuffleArray(ekx, blockPositionInvert[sv]);

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

        /// <summary>
        /// Gets the checksum of a 232 byte array.
        /// </summary>
        /// <param name="data">Decrypted <see cref="PKM"/> data.</param>
        /// <returns></returns>
        public static ushort GetCHK(byte[] data)
        {
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(data, i);
            
            return chk;
        }

        /// <summary>
        /// Gets the Wurmple Evolution Value for a given <see cref="PKM.EncryptionConstant"/>
        /// </summary>
        /// <param name="EC">Encryption Constant</param>
        /// <returns>Wurmple Evolution Value</returns>
        public static uint GetWurmpleEvoVal(uint EC)
        {
            var evoVal = EC >> 16;
            return evoVal % 10 / 5;
        }

        /// <summary>
        /// Gets the Wurmple <see cref="PKM.EncryptionConstant"/> for a given Evolution Value
        /// </summary>
        /// <param name="evoVal">Wurmple Evolution Value</param>
        /// <remarks>0 = Silcoon, 1 = Cascoon</remarks>
        /// <returns>Encryption Constan</returns>
        public static uint GetWurmpleEC(int evoVal)
        {
            uint EC;
            while (true)
                if (evoVal == GetWurmpleEvoVal(EC = Util.Rand32()))
                    return EC;
        }

        /// <summary>
        /// Gets a random PID according to specifications.
        /// </summary>
        /// <param name="species">National Dex ID</param>
        /// <param name="cg">Current Gender</param>
        /// <param name="origin">Origin Generation</param>
        /// <param name="nature">Nature</param>
        /// <param name="form">AltForm</param>
        /// <param name="OLDPID">Current PID</param>
        /// <remarks>Used to retain ability bits.</remarks>
        /// <returns>Rerolled PID.</returns>
        public static uint GetRandomPID(int species, int cg, int origin, int nature, int form, uint OLDPID)
        {
            uint bits = OLDPID & 0x00010001;
            int gt = Personal[species].Gender;
            if (origin >= 24)
                return Util.Rand32();

            bool g3unown = origin <= 5 && species == 201;
            while (true) // Loop until we find a suitable PID
            {
                uint pid = Util.Rand32();

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
                else if (bits != (pid & 0x00010001)) // keep ability bits
                    continue;

                if (gt == 255 || gt == 254 || gt == 0) // Set Gender(less)
                    return pid; // PID can be anything

                // Gen 3/4/5: Gender derived from PID
                if (cg == GetGender(pid, gt))
                    return pid;
            }
        }

        // Data Requests
        public static string GetResourceStringBall(int ball) => "_ball" + ball;
        public static string GetResourceStringSprite(int species, int form, int gender, int generation)
        {
            if (new[] { 778, 664, 665, 414, 493, 773 }.Contains(species)) // Species who show their default sprite regardless of Form
                form = 0;

            string file = "_" + species;
            if (form > 0) // Alt Form Handling
                file += "_" + form;
            else if (gender == 1 && new[] { 592, 593, 521, 668 }.Contains(species)) // Frillish & Jellicent, Unfezant & Pyroar
                file += "_" + gender;

            if (species == 25 && form > 0 && generation >= 7) // Pikachu
                file += "c"; // Cap

            return file;
        }

        /// <summary>
        /// Gets a list of formes that the species can have.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <param name="types">List of type names</param>
        /// <param name="forms">List of form names</param>
        /// <param name="genders">List of genders names</param>
        /// <param name="generation">Generation number for exclusive formes</param>
        /// <returns>A list of strings corresponding to the formes that a Pokémon can have.</returns>
        public static string[] GetFormList(int species, string[] types, string[] forms, string[] genders, int generation = Generation)
        {
            return FormConverter.GetFormList(species, types, forms, genders, generation);
        }

        /// <summary>Calculate the Hidden Power Type of the entered IVs.</summary>
        /// <param name="type">Hidden Power Type</param>
        /// <param name="ivs">Individual Values (H/A/B/S/C/D)</param>
        /// <returns>Hidden Power Type</returns>
        public static int[] SetHPIVs(int type, int[] ivs)
        {
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + hpivs[type, i];
            return ivs;
        }

        /// <summary>
        /// Hidden Power IV values (even or odd) to achieve a specified Hidden Power Type
        /// </summary>
        /// <remarks>
        /// There are other IV combinations to achieve the same Hidden Power Type.
        /// These are just precomputed for fast modification.
        /// </remarks>
        public static readonly int[,] hpivs = {
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

        /// <summary>
        /// Shuffles a 136 byte array containing <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Data to shuffle</param>
        /// <param name="sv">Block Shuffle order</param>
        /// <returns>Shuffled byte array</returns>
        public static byte[] ShuffleArray45(byte[] data, uint sv)
        {
            byte[] sdata = new byte[data.Length];
            Array.Copy(data, sdata, 8); // Copy unshuffled bytes

            // Shuffle Away!
            for (int block = 0; block < 4; block++)
                Array.Copy(data, 8 + 32 * blockPosition[block][sv], sdata, 8 + 32 * block, 32);

            // Fill the Battle Stats back
            if (data.Length > 136)
                Array.Copy(data, 136, sdata, 136, data.Length - 136);

            return sdata;
        }

        /// <summary>
        /// Decrypts a 136 byte + party stat byte array.
        /// </summary>
        /// <param name="ekm">Encrypted <see cref="PKM"/> data.</param>
        /// <returns>Decrypted <see cref="PKM"/> data.</returns>
        public static byte[] DecryptArray45(byte[] ekm)
        {
            byte[] pkm = (byte[])ekm.Clone();

            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint chk = BitConverter.ToUInt16(pkm, 6);
            uint sv = ((pv & 0x3E000) >> 0xD) % 24;

            uint seed = chk;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 136; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkm, i);

            // Deshuffle
            pkm = ShuffleArray45(pkm, sv);

            // Decrypt the Party Stats
            seed = pv;
            if (pkm.Length <= 136) return pkm;
            for (int i = 136; i < pkm.Length; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(pkm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(pkm, i);

            return pkm;
        }

        /// <summary>
        /// Encrypts a 136 byte + party stat byte array.
        /// </summary>
        /// <param name="pkm">Decrypted <see cref="PKM"/> data.</param>
        /// <returns>Encrypted <see cref="PKM"/> data.</returns>
        public static byte[] EncryptArray45(byte[] pkm)
        {
            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint sv = ((pv & 0x3E000) >> 0xD) % 24;

            uint chk = BitConverter.ToUInt16(pkm, 6);
            byte[] ekm = (byte[])pkm.Clone();

            ekm = ShuffleArray45(ekm, blockPositionInvert[sv]);

            uint seed = chk;

            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 136; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(ekm, i);

            // If no party stats, return.
            if (ekm.Length <= 136) return ekm;

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 136; i < ekm.Length; i += 2)
                BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(ekm, i) ^ LCRNG(ref seed) >> 16)).CopyTo(ekm, i);

            // Done
            return ekm;
        }

        /// <summary>
        /// Gets the Unown Forme ID from PID.
        /// </summary>
        /// <param name="PID">Personality ID</param>
        /// <remarks>Should only be used for 3rd Generation origin specimens.</remarks>
        /// <returns></returns>
        public static int GetUnownForm(uint PID)
        {
            byte[] data = BitConverter.GetBytes(PID);
            return (((data[3] & 3) << 6) + ((data[2] & 3) << 4) + ((data[1] & 3) << 2) + ((data[0] & 3) << 0)) % 28;
        }
        
        /// <summary>
        /// Gets the gender ID of the species based on the Personality ID.
        /// </summary>
        /// <param name="species">National Dex ID.</param>
        /// <param name="PID">Personality ID.</param>
        /// <returns>Gender ID (0/1/2)</returns>
        /// <remarks>This method should only be used for Generations 3-5 origin.</remarks>
        public static int GetGender(int species, uint PID)
        {
            int genderratio = Personal[species].Gender;
            return GetGender(PID, genderratio);
        }
        public static int GetGender(uint PID, int gr)
        {
            switch (gr)
            {
                case 255: return 2;
                case 254: return 1;
                case 0: return 0;
                default: return (PID & 0xFF) < gr ? 1 : 0;
            }
        }

        /// <summary>
        /// Decrypts an 80 byte format <see cref="PK3"/> byte array.
        /// </summary>
        /// <param name="ekm">Encrypted data.</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] DecryptArray3(byte[] ekm)
        {
            if (ekm.Length != SIZE_3PARTY && ekm.Length != SIZE_3STORED)
                return null;

            uint PID = BitConverter.ToUInt32(ekm, 0);
            uint OID = BitConverter.ToUInt32(ekm, 4);
            uint seed = PID ^ OID;

            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i % 4];
            return ShuffleArray3(ekm, PID%24);
        }

        /// <summary>
        /// Shuffles an 80 byte format <see cref="PK3"/> byte array.
        /// </summary>
        /// <param name="data">Unshuffled data.</param>
        /// <param name="sv">Block order shuffle value</param>
        /// <returns></returns>
        private static byte[] ShuffleArray3(byte[] data, uint sv)
        {
            byte[] sdata = new byte[data.Length];
            Array.Copy(data, sdata, 32); // Copy unshuffled bytes

            // Shuffle Away!
            for (int block = 0; block < 4; block++)
                Array.Copy(data, 32 + 12 * blockPosition[block][sv], sdata, 32 + 12 * block, 12);

            // Fill the Battle Stats back
            if (data.Length > SIZE_3STORED)
                Array.Copy(data, SIZE_3STORED, sdata, SIZE_3STORED, data.Length - SIZE_3STORED);

            return sdata;
        }

        /// <summary>
        /// Encrypts an 80 byte format <see cref="PK3"/> byte array.
        /// </summary>
        /// <param name="pkm">Decrypted data.</param>
        /// <returns>Encrypted data.</returns>
        public static byte[] EncryptArray3(byte[] pkm)
        {
            if (pkm.Length != SIZE_3PARTY && pkm.Length != SIZE_3STORED)
                return null;

            uint PID = BitConverter.ToUInt32(pkm, 0);
            uint OID = BitConverter.ToUInt32(pkm, 4);
            uint seed = PID ^ OID;

            byte[] ekm = ShuffleArray3(pkm, blockPositionInvert[PID%24]);
            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i % 4];
            return ekm;
        }

        /// <summary>
        /// Gets the Main Series language ID from a GameCube (C/XD) language ID. Re-maps Spanish 6->7.
        /// </summary>
        /// <param name="value">GameCube (C/XD) language ID.</param>
        /// <returns>Main Series language ID.</returns>
        public static byte GetMainLangIDfromGC(byte value) => value == 6 ? (byte)7 : value;

        /// <summary>
        /// Gets the GameCube (C/XD) language ID from a Main Series language ID. Re-maps Spanish 7->6.
        /// </summary>
        /// <param name="value">Main Series language ID.</param>
        /// <returns>GameCube (C/XD) language ID.</returns>
        public static byte GetGCLangIDfromMain(byte value) => value == 7 ? (byte)6 : value;

        /// <summary>
        /// Gets an array of valid <see cref="PKM"/> file extensions.
        /// </summary>
        /// <returns>Valid <see cref="PKM"/> file extensions.</returns>
        public static string[] GetPKMExtensions(int MaxGeneration = Generation)
        {
            var result = new List<string>();
            result.AddRange(new [] {"ck3", "xk3", "bk4"}); // Special Cases
            for (int i = 1; i <= MaxGeneration; i++)
                result.Add("pk"+i);
            return result.ToArray();
        }

        // Extensions
        /// <summary>
        /// Gets the Location Name for the <see cref="PKM"/>
        /// </summary>
        /// <param name="pk">PKM to fetch data for</param>
        /// <param name="eggmet">Location requested is the egg obtained location, not met location.</param>
        /// <returns>Location string</returns>
        public static string GetLocationString(this PKM pk, bool eggmet)
        {
            if (pk.Format < 2)
                return "";

            int locval = eggmet ? pk.Egg_Location : pk.Met_Location;
            return GameInfo.GetLocationName(eggmet, locval, pk.Format, pk.GenNumber);
        }
        public static string[] GetQRLines(this PKM pkm)
        {
            var s = GameInfo.Strings;
            // Summarize
            string filename = pkm.Nickname;
            if (pkm.Nickname != s.specieslist[pkm.Species] && s.specieslist[pkm.Species] != null)
                filename += $" ({s.specieslist[pkm.Species]})";
            
            string header = $"{filename} [{s.abilitylist[pkm.Ability]}] lv{pkm.Stat_Level} @ {s.itemlist[pkm.HeldItem]} -- {s.natures[pkm.Nature]}";
            string moves = string.Join(" / ", pkm.Moves.Select(move => move < s.movelist.Length ? s.movelist[move] : "ERROR"));
            string IVs = $"IVs: {pkm.IV_HP:00}/{pkm.IV_ATK:00}/{pkm.IV_DEF:00}/{pkm.IV_SPA:00}/{pkm.IV_SPD:00}/{pkm.IV_SPE:00}";
            string EVs = $"EVs: {pkm.EV_HP:00}/{pkm.EV_ATK:00}/{pkm.EV_DEF:00}/{pkm.EV_SPA:00}/{pkm.EV_SPD:00}/{pkm.EV_SPE:00}";

            return new[]
            {
                header,
                moves,
                IVs + "   " + EVs,
            };
        }
    }
}
