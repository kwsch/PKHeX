using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static bool getIsPKM(long len)
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
        
        public static uint LCRNG(uint seed)
        {
            const uint a = 0x41C64E6D;
            const uint c = 0x00006073;

            return seed * a + c;
        }
        public static uint LCRNG(ref uint seed)
        {
            const uint a = 0x41C64E6D;
            const uint c = 0x00006073;

            return seed = seed * a + c;
        }
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

        public static readonly string[][] SpeciesLang = 
        {
            Util.getSpeciesList("ja"), // none
            Util.getSpeciesList("ja"), // 1
            Util.getSpeciesList("en"), // 2
            Util.getSpeciesList("fr"), // 3
            Util.getSpeciesList("it"), // 4
            Util.getSpeciesList("de"), // 5
            Util.getSpeciesList("es"), // none
            Util.getSpeciesList("es"), // 7
            Util.getSpeciesList("ko"), // 8
            Util.getSpeciesList("zh"), // 9 Simplified
            Util.getSpeciesList("zh2"), // 10 Traditional
        };

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="lang">Language ID of the Pokémon</param>
        /// <returns>The Species name if within expected range, else an empty string.</returns>
        public static string getSpeciesName(int species, int lang)
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
        public static string getSpeciesNameGeneration(int species, int lang, int generation)
        {
            string nick = getSpeciesName(species, lang);

            if (generation < 5) // All caps GenIV and previous
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
        public static bool getIsNicknamedAnyLanguage(int species, string nick, int generation)
        {
            int len = SpeciesLang.Length;
            if (generation < 3)
                len = 3;
            else if (generation < 7)
                len = 8;

            for (int i = 0; i < len; i++)
                if (getSpeciesNameGeneration(species, i, generation) == nick)
                    return false;
            return true;
        }

        /// <summary>
        /// Gets randomized EVs for a given generation format
        /// </summary>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Array containing randomized EVs (H/A/B/S/C/D)</returns>
        public static uint[] getRandomEVs(int generation = Generation)
        {
            if (generation > 2)
            {
                uint[] evs = new uint[6];
                do
                {
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
            else
            {
                uint[] evs = new uint[6];
                for (int i = 0; i < evs.Length; i++)
                    evs[i] = Util.rnd32() & ushort.MaxValue;
                return evs;
            }
        }

        /// <summary>
        /// Gets the current level of a species.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <param name="exp">Experience points</param>
        /// <returns>Current level of the species.</returns>
        public static int getLevel(int species, uint exp)
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
        public static uint getEXP(int level, int species)
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
        public static int getGender(string s)
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
        public static readonly byte[][] blockPosition =
        {
            new byte[] {0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3},
            new byte[] {1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2},
            new byte[] {2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1},
            new byte[] {3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0},
        };

        /// <summary>
        /// Positions for unshuffling.
        /// </summary>
        public static readonly byte[] blockPositionInvert =
        {
            0, 1, 2, 4, 3, 5, 6, 7, 12, 18, 13, 19, 8, 10, 14, 20, 16, 22, 9, 11, 15, 21, 17, 23
        };


        /// <summary>
        /// Shuffles a 232 byte array containing <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Data to shuffle</param>
        /// <param name="sv">Block Shuffle order</param>
        /// <returns>Shuffled byte array</returns>
        public static byte[] shuffleArray(byte[] data, uint sv)
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
        public static byte[] decryptArray(byte[] ekx)
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

        /// <summary>
        /// Encrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="pkx">Decrypted <see cref="PKM"/> data.</param>
        public static byte[] encryptArray(byte[] pkx)
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

        /// <summary>
        /// Gets the checksum of a 232 byte array.
        /// </summary>
        /// <param name="data">Decrypted <see cref="PKM"/> data.</param>
        /// <returns></returns>
        public static ushort getCHK(byte[] data)
        {
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(data, i);
            
            return chk;
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
        public static uint getRandomPID(int species, int cg, int origin, int nature, int form, uint OLDPID)
        {
            uint bits = OLDPID & 0x00010001;
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
                else if (bits != (pid & 0x00010001)) // keep ability bits
                    continue;

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

        // Data Requests
        public static string getBallString(int ball) => "_ball" + ball;
        public static string getSpriteString(int species, int form, int gender, int generation)
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
        /// <param name="t">List of type names</param>
        /// <param name="f">List of forme names</param>
        /// <param name="g">List of gender names</param>
        /// <param name="generation">Generation number for exclusive formes</param>
        /// <returns>A list of strings corresponding to the formes that a Pokémon can have.</returns>
        public static string[] getFormList(int species, string[] t, string[] f, string[] g, int generation = Generation)
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
                        f[804], // Mega
                    };}
            switch (species)
            {
                case 6:
                case 150:
                    return new[]
                    {
                        t[000], // Normal
                        f[805], // Mega X
                        f[806], // Mega Y
                    };
                case 025:
                    if (generation == 6)
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
                    if (generation == 7)
                    return new[]
                    {
                        t[000], // Normal
                        f[813], // Original
                        f[814], // Hoenn
                        f[815], // Sinnoh
                        f[816], // Unova
                        f[817], // Kalos
                        f[818], // Alola
                    };
                    break;
                case 172:
                    if (generation != 4)
                        break;
                    return new[]
                    {
                        t[000], // Normal
                        f[000], // Spiky
                    };
                case 201:
                    if (generation == 2)
                    return new[]
                    {
                        "A", "B", "C", "D", "E",
                        "F", "G", "H", "I", "J",
                        "K", "L", "M", "N", "O",
                        "P", "Q", "R", "S", "T",
                        "U", "V", "W", "X", "Y",
                        "Z",
                        // "!", "?", not in Gen II
                    };
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
                        f[889], // Sunny
                        f[890], // Rainy
                        f[891], // Snowy
                    };
                case 382:
                case 383:
                    return new[]
                    {
                        t[000], // Normal
                        f[899], // Primal
                    };
                case 386:
                    return new[]
                    {
                        t[000], // Normal
                        f[902], // Attack
                        f[903], // Defense
                        f[904], // Speed
                    };

                case 412:
                case 413:
                case 414:
                    return new[]
                    {
                        f[412], // Plant
                        f[905], // Sandy
                        f[906], // Trash
                    };

                case 421:
                    return new[]
                    {
                        f[421], // Overcast
                        f[909], // Sunshine
                    };

                case 422:
                case 423:
                    return new[]
                    {
                        f[422], // West
                        f[911], // East
                    };

                case 479:
                    return new[]
                    {
                        t[000], // Normal
                        f[917], // Heat
                        f[918], // Wash
                        f[919], // Frost
                        f[920], // Fan
                        f[921], // Mow
                    };

                case 487:
                    return new[]
                    {
                        f[487], // Altered
                        f[922], // Origin
                    };

                case 492:
                    return new[]
                    {
                        f[492], // Land
                        f[923], // Sky
                    };

                case 493: // Arceus
                case 773: // Silvally
                    if (generation == 4)
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
                            "???", // ???-type arceus
                            t[09],
                            t[10],
                            t[11],
                            t[12],
                            t[13],
                            t[14],
                            t[15],
                            t[16] // No Fairy Type
                        };
                    if (generation == 5)
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
                            t[16] // No Fairy type
                        };
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
                        f[942], // Blue
                    };

                case 555:
                    return new[]
                    {
                        f[555], // Standard
                        f[943], // Zen
                    };

                case 585:
                case 586:
                    return new[]
                    {
                        f[585], // Spring
                        f[947], // Summer
                        f[948], // Autumn
                        f[949], // Winter
                    };

                case 641:
                case 642:
                case 645:
                    return new[]
                    {
                        f[641], // Incarnate
                        f[952], // Therian
                    };

                case 646:
                    return new[]
                    {
                        t[000], // Normal
                        f[953], // White
                        f[954], // Black
                    };

                case 647:
                    return new[]
                    {
                        f[647], // Ordinary
                        f[955], // Resolute
                    };

                case 648:
                    return new[]
                    {
                        f[648], // Aria
                        f[956], // Pirouette
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

                case 658:
                    return new[]
                    {
                        t[000], // Normal
                        f[962], // "Ash",
                        f[1012], // "Bonded" - Active
                    };

                case 664:
                case 665:
                case 666:
                    return new[]
                    {
                        f[666], // Icy Snow
                        f[963], // Polar
                        f[964], // Tundra
                        f[965], // Continental 
                        f[966], // Garden
                        f[967], // Elegant
                        f[968], // Meadow
                        f[969], // Modern 
                        f[970], // Marine
                        f[971], // Archipelago
                        f[972], // High-Plains
                        f[973], // Sandstorm
                        f[974], // River
                        f[975], // Monsoon
                        f[976], // Savannah 
                        f[977], // Sun
                        f[978], // Ocean
                        f[979], // Jungle
                        f[980], // Fancy
                        f[981], // Poké Ball
                    };

                case 669:
                case 671:
                    return new[]
                    {
                        f[669], // Red
                        f[986], // Yellow
                        f[987], // Orange
                        f[988], // Blue
                        f[989], // White
                    };

                case 670:
                    return new[]
                    {
                        f[669], // Red
                        f[986], // Yellow
                        f[987], // Orange
                        f[988], // Blue
                        f[989], // White
                        f[990], // Eternal
                    };

                case 676:
                    return new[]
                    {
                        f[676], // Natural
                        f[995], // Heart
                        f[996], // Star
                        f[997], // Diamond
                        f[998], // Deputante
                        f[999], // Matron
                        f[1000], // Dandy
                        f[1001], // La Reine
                        f[1002], // Kabuki 
                        f[1003], // Pharaoh
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
                        f[1005], // Blade
                    };

                case 710:
                case 711:
                    return new[]
                    {
                        f[710], // Average
                        f[1006], // Small
                        f[1007], // Large
                        f[1008], // Super
                    };

                case 716:
                    return new[]
                    {
                        t[000], // Normal
                        f[1012], // Active
                    };

                case 720:
                    return new[]
                    {
                        t[000], // Normal
                        f[1018], // Unbound
                    };

                case 718: // Zygarde
                    return new[]
                    {
                        t[000], // Normal (Aura Break)
                        "10%", // (Aura Break)
                        "10%-C", // Cell (Power Construct)
                        "50%-C", // Cell (Power Construct)
                        "100%-C" // Cell (Power Construct)
                    };

                case 741: // Oricorio
                    return new[]
                    {
                        f[741], // "RED" - Baile
                        f[1021], // "YLW" - Pom-Pom
                        f[1022], // "PNK" - Pa'u
                        f[1023], // "BLU" - Sensu
                    };

                case 745: // Lycanroc
                    return new[]
                    {
                        f[745], // Midday
                        f[1024], // Midnight
                    };

                case 746: // Wishiwashi
                    return new[]
                    {
                        f[746],
                        f[1025], // School
                    };

                case 774: // Minior
                    return new[]
                    {
                        f[774], // "R-Meteor", // Meteor Red
                        f[1045], // "O-Meteor", // Meteor Orange
                        f[1046], // "Y-Meteor", // Meteor Yellow
                        f[1047], // "G-Meteor", // Meteor Green
                        f[1048], // "B-Meteor", // Meteor Blue
                        f[1049], // "I-Meteor", // Meteor Indigo
                        f[1050], // "V-Meteor", // Meteor Violet
                        f[1051], // "R-Core", // Core Red
                        f[1052], // "O-Core", // Core Orange
                        f[1053], // "Y-Core", // Core Yellow
                        f[1054], // "G-Core", // Core Green
                        f[1055], // "B-Core", // Core Blue
                        f[1056], // "I-Core", // Core Indigo
                        f[1057], // "V-Core", // Core Violet
                    };

                case 778: // Mimikyu
                    return new[]
                    {
                        t[000],
                        f[1058], // Busted
                    };

                case 19: // Rattata
                case 20: // Raticate
                case 26: // Raichu
                case 27: // Sandshrew
                case 28: // Sandslash
                case 37: // Vulpix
                case 38: // Ninetails
                case 50: // Diglett
                case 51: // Dugtrio
                case 52: // Meowth
                case 53: // Persian
                case 74: // Geodude
                case 75: // Graveler
                case 76: // Golem
                case 88: // Grimer
                case 89: // Muk
                case 105: // Marowak
                case 103: // Exeggutor
                    if (generation < 7)
                        break;
                    return new[]
                    {
                        t[000],
                        f[810] // Alolan
                    };

                case 801: // Magearna
                    return new[]
                    {
                        t[000],
                        f[1062], // Original
                    };
            }
            return new[] {""};
        }

        /// <summary>Calculate the Hidden Power Type of the entered IVs.</summary>
        /// <param name="type">Hidden Power Type</param>
        /// <param name="ivs">Individual Values (H/A/B/S/C/D)</param>
        /// <returns>Hidden Power Type</returns>
        public static int[] setHPIVs(int type, int[] ivs)
        {
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + hpivs[type, i];
            return ivs;
        }
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
        /// Converts full width to single width
        /// </summary>
        /// <param name="str">Input string to sanitize.</param>
        /// <returns></returns>
        public static string SanitizeString(string str)
        {
            if (str.Length == 0)
                return str;
            var s = str.Replace("\u2019", "\u0027"); // farfetch'd
            s = s.Replace("\uE08F", "\u2640"); // ♀
            s = s.Replace("\uE08E", "\u2642"); // ♂
            return s;
        }

        /// <summary>
        /// Converts full width to half width when appropriate
        /// </summary>
        /// <param name="str">Input string to set.</param>
        /// <param name="species"></param>
        /// <param name="nicknamed"></param>
        /// <returns></returns>
        public static string UnSanitizeString(string str, int species = -1, bool nicknamed = true)
        {
            var s = str.Replace("\u0027", "\u2019"); // farfetch'd

            bool foreign = true;
            if ((species == 029 || species == 032) && !nicknamed)
                foreign = str[0] != 'N';
            else if (nicknamed)
                foreign = str.Select(c => c >> 12).Any(c => c != 0 && c != 0xE);

            if (foreign)
                return s;
            // Convert back to half width
            s = s.Replace("\u2640", "\uE08F"); // ♀
            s = s.Replace("\u2642", "\uE08E"); // ♂
            return s;
        }

        /// <summary>
        /// Trims a string at the first instance of a 0xFFFF terminator.
        /// </summary>
        /// <param name="input">String to trim.</param>
        /// <returns>Trimmed string.</returns>
        public static string TrimFromFFFF(string input)
        {
            int index = input.IndexOf((char)0xFFFF);
            return index < 0 ? input : input.Substring(0, index);
        }

        /// <summary>
        /// Shuffles a 136 byte array containing <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Data to shuffle</param>
        /// <param name="sv">Block Shuffle order</param>
        /// <returns>Shuffled byte array</returns>
        public static byte[] shuffleArray45(byte[] data, uint sv)
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
        public static byte[] decryptArray45(byte[] ekm)
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
            pkm = shuffleArray45(pkm, sv);

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
        public static byte[] encryptArray45(byte[] pkm)
        {
            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint sv = ((pv & 0x3E000) >> 0xD) % 24;

            uint chk = BitConverter.ToUInt16(pkm, 6);
            byte[] ekm = (byte[])pkm.Clone();

            ekm = shuffleArray45(ekm, blockPositionInvert[sv]);

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
        public static int getUnownForm(uint PID)
        {
            byte[] data = BitConverter.GetBytes(PID);
            return (((data[3] & 3) << 6) + ((data[2] & 3) << 4) + ((data[1] & 3) << 2) + ((data[0] & 3) << 0)) % 28;
        }

        /// <summary>
        /// Converts a Unicode string to Generation 7 in-game chinese string.
        /// </summary>
        /// <param name="inputstr">Unicode string.</param>
        /// <param name="cht">Pkm language is Traditional Chinese.</param>
        /// <returns>In-game chinese string.</returns>
        public static string str2binG7_zh(string inputstr, bool cht = false)
        {
            string resultstr = "";
            bool IsCHT = inputstr.Any(chr => Gen7_CHT.Contains(chr) && !Gen7_CHS.Contains(chr));
            IsCHT |= cht && !inputstr.Any(chr => Gen7_CHT.Contains(chr) ^ Gen7_CHS.Contains(chr)); // CHS and CHT have the same display name
            var table = IsCHT ? Gen7_CHT : Gen7_CHS;
            ushort ofs = IsCHT ? Gen7_CHT_Ofs : Gen7_CHS_Ofs;
            foreach (char chr in inputstr)
            {
                var index = Array.IndexOf(table, chr);
                resultstr += index > -1 ? (char)(ofs + index) : chr;
            }
            return resultstr;
        }
        /// <summary>
        /// Converts a Generation 7 in-game chinese string to Unicode string.
        /// </summary>
        /// <param name="inputstr">In-game chinese string.</param>
        /// <returns>Unicode string.</returns>
        public static string bin2strG7_zh(string inputstr)
        {
            string resultstr = "";
            foreach (var val in inputstr)
            {
                if (Gen7_CHS_Ofs <= val && val < Gen7_CHS_Ofs + Gen7_CHS.Length)
                    resultstr += Gen7_CHS[val - Gen7_CHS_Ofs];
                else if (Gen7_CHT_Ofs <= val && val < Gen7_CHT_Ofs + Gen7_CHT.Length)
                    resultstr += Gen7_CHT[val - Gen7_CHT_Ofs];
                else resultstr += val;
            }
            return resultstr;
        }

        /// <summary>
        /// Converts a Generation 4 value to Unicode character.
        /// </summary>
        /// <param name="val">Encoded value.</param>
        /// <returns>Decoded value (unicode).</returns>
        public static ushort val2charG4(ushort val)
        {
            int index = Array.IndexOf(G4Values, val);
            return index > -1 ? G4Chars[index] : (ushort)0xFFFF;
        }

        /// <summary>
        /// Converts a Unicode character to Generation 4 value.
        /// </summary>
        /// <param name="chr">Decoded value (unicode).</param>
        /// <returns>Encoded value.</returns>
        public static ushort char2valG4(ushort chr)
        {
            int index = Array.IndexOf(G4Chars, chr);
            return index > -1 ? G4Values[index] : (ushort)0xFFFF;
        }

        /// <summary>
        /// Converts Generation 4 Big Endian encoded character data to string.
        /// </summary>
        /// <param name="strdata">Byte array containing encoded character data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Converted string.</returns>
        public static string getBEString4(byte[] strdata, int offset, int count)
        {
            string s = "";
            for (int i = 0; i < count; i += 2)
            {
                ushort val = BigEndian.ToUInt16(strdata, offset + i);
                if (val == 0xFFFF) break;
                ushort chr = val2charG4(val);
                if (chr == 0xFFFF) break;
                s += (char)chr;
            }
            return s;
        }

        /// <summary>
        /// Converts a string to Generation 4 Big Endian encoded character data.
        /// </summary>
        /// <param name="value">String to be converted.</param>
        /// <param name="maxLength">Maximum length of string</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Byte array containing encoded character data</returns>
        public static byte[] setBEString4(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            byte[] strdata = new byte[value.Length * 2 + 2]; // +2 for 0xFFFF
            for (int i = 0; i < value.Length; i++)
            {
                ushort chr = value[i];
                ushort val = char2valG4(chr);
                if (val == 0xFFFF || chr == 0xFFFF)
                { Array.Resize(ref strdata, i * 2 + 2); break; }
                BigEndian.GetBytes(val).CopyTo(strdata, i * 2);
            }
            BitConverter.GetBytes((ushort)0xFFFF).CopyTo(strdata, strdata.Length - 2);
            if (padTo > strdata.Length)
            {
                int start = strdata.Length;
                Array.Resize(ref strdata, padTo);
                for (int i = start; i < padTo; i += 2)
                    BitConverter.GetBytes(padWith).CopyTo(strdata, i);
            }
            return strdata;
        }

        /// <summary>
        /// Converts a Generation 3 encoded value to corresponding Generation 4 encoded value.
        /// </summary>
        /// <param name="val">Generation 3 encoded value.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Generation 4 encoded value.</returns>
        public static ushort getG4Val(byte val, bool jp) => jp ? G34_4J[val] : G34_4E[val];

        /// <summary>
        /// Converts a Generation 3 encoded value to corresponding Generation 4 decoded character.
        /// </summary>
        /// <param name="val">Generation 3 encoded value.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Decoded value.</returns>
        public static ushort getG3Char(byte val, bool jp) => val2charG4(getG4Val(val, jp));

        /// <summary>
        /// Converts a Generation 4 decoded character to Generation 3 encoded value.
        /// </summary>
        /// <param name="chr">Generation 4 decoded character.</param>
        /// <param name="jp">Character destination is Japanese font.</param>
        /// <returns>Generation 3 encoded value.</returns>
        public static byte setG3Char(ushort chr, bool jp)
        {
            int index = Array.IndexOf(jp ? G34_4J : G34_4E, char2valG4(chr));
            return (byte)(index > -1 ? index : 0xFF);
        }

        /// <summary>
        /// Converts a Generation 3 encoded value array to string.
        /// </summary>
        /// <param name="strdata">Byte array containing string data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Decoded string.</returns>
        public static string getString3(byte[] strdata, int offset, int count, bool jp)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                byte val = strdata[offset + i];
                if (val >= 247) // Take valid values
                    break;
                var c = getG3Char(val, jp); // Convert to Unicode
                if (c == 0xFF) // Stop if Terminator
                    break;
                s.Append((char)c);
            }
            return SanitizeString(s.ToString());
        }

        /// <summary>
        /// Converts a string to a Generation 3 encoded value array.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="jp">String destination is Japanese font.</param>
        /// <param name="maxLength">Maximum length of string</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns></returns>
        public static byte[] setString3(string value, int maxLength, bool jp, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            byte[] strdata = new byte[value.Length + 1]; // +1 for 0xFF
            for (int i = 0; i < value.Length; i++)
            {
                ushort chr = value[i];
                byte val = setG3Char(chr, jp);
                if (val == 0xFF || chr == 0xFF)
                { Array.Resize(ref strdata, i); break; }
                strdata[i] = val;
            }
            if (strdata.Length > 0)
                strdata[strdata.Length - 1] = 0xFF;
            if (strdata.Length > maxLength)
                Array.Resize(ref strdata, maxLength);
            if (strdata.Length < padTo)
            {
                int start = strdata.Length;
                Array.Resize(ref strdata, padTo);
                for (int i = start; i < strdata.Length; i++)
                    strdata[i] = (byte)padWith;
            }
            return strdata;
        }

        /// <summary>
        /// Converts Generation 3 species ID to National Dex ID.
        /// </summary>
        /// <param name="g3index">Generation 3 species ID.</param>
        /// <returns>National Dex ID.</returns>
        public static int getG4Species(int g3index)
        {
            int index = Array.IndexOf(oldindex, g3index);
            return newindex[index > -1 ? index : 0];
        }

        /// <summary>
        /// Converts a National Dex ID to Generation 3 species ID.
        /// </summary>
        /// <param name="g4index">National Dex ID</param>
        /// <returns>Generation 3 species ID.</returns>
        public static int getG3Species(int g4index)
        {
            int index = Array.IndexOf(newindex, g4index);
            return oldindex[index > -1 ? index : 0];
        }

        /// <summary>
        /// Gets the gender ID of the species based on the Personality ID.
        /// </summary>
        /// <param name="species">National Dex ID.</param>
        /// <param name="PID">Personality ID.</param>
        /// <returns>Gender ID (0/1/2)</returns>
        /// <remarks>This method should only be used for Generations 3-5 origin.</remarks>
        public static int getGender(int species, uint PID)
        {
            int genderratio = Personal[species].Gender;
            switch (genderratio)
            {
                case 255: return 2;
                case 254: return 1;
                case 0: return 0;
                default: return (PID & 0xFF) < genderratio ? 1 : 0;
            }
        }
        #region Gen 3 Species Table
        private static readonly int[] newindex =
        {
            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
            31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,
            59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,
            87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,
            111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,
            132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,
            153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,
            174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,
            195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
            216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,
            237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,
            258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,290,291,292,
            276,277,285,286,327,278,279,283,284,320,321,300,301,352,343,344,299,324,302,339,340,
            370,341,342,349,350,318,319,328,329,330,296,297,309,310,322,323,363,364,365,331,332,
            361,362,337,338,298,325,326,311,312,303,307,308,333,334,360,355,356,315,287,288,289,
            316,317,357,293,294,295,366,367,368,359,353,354,336,335,369,304,305,306,351,313,314,
            345,346,347,348,280,281,282,371,372,373,374,375,376,377,378,379,382,383,384,380,381,
            385,386,358,
        };

        private static readonly int[] oldindex =
        {
            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
            31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,
            59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,
            87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,
            111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,
            132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,
            153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,
            174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,
            195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
            216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,
            237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,277,278,279,280,281,282,
            283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,
            304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,
            325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,
            346,347,348,349,350,351,352,353,354,355,356,357,358,359,360,361,362,363,364,365,366,
            367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,
            388,389,390,391,392,393,394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,
            409,410,411,
        };
        #endregion
        #region Gen 3/4 Character Tables (Val->Unicode)

        public static readonly ushort[] G4Values =
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75,
            76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
            102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123,
            124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145,
            146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
            168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
            190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211,
            212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234,
            235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256,
            257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278,
            279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300,
            301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322,
            323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344,
            345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366,
            367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388,
            389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410,
            411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432,
            433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454,
            455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476,
            477, 478, 479, 480, 481, 488, 489, 490, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034,
            1035, 1036, 1037, 1038, 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053,
            1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071,
            1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089,
            1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 1104, 1105, 1106, 1107,
            1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125,
            1126, 1127, 1128, 1129, 1130, 1131, 1132, 1133, 1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143,
            1144, 1145, 1146, 1147, 1148, 1149, 1150, 1151, 1152, 1153, 1154, 1155, 1156, 1157, 1158, 1159, 1160, 1161,
            1162, 1163, 1164, 1165, 1166, 1167, 1168, 1169, 1170, 1171, 1172, 1173, 1174, 1175, 1176, 1177, 1178, 1179,
            1180, 1181, 1182, 1183, 1184, 1185, 1186, 1187, 1188, 1189, 1190, 1191, 1192, 1193, 1194, 1195, 1196, 1197,
            1198, 1199, 1200, 1201, 1202, 1203, 1204, 1205, 1206, 1207, 1208, 1209, 1210, 1211, 1212, 1213, 1214, 1215,
            1216, 1217, 1218, 1219, 1220, 1221, 1222, 1223, 1224, 1225, 1226, 1227, 1228, 1229, 1230, 1231, 1232, 1233,
            1234, 1235, 1236, 1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250, 1251,
            1252, 1253, 1254, 1255, 1256, 1257, 1258, 1259, 1260, 1261, 1262, 1263, 1264, 1265, 1266, 1267, 1268, 1269,
            1270, 1271, 1272, 1273, 1274, 1275, 1276, 1277, 1278, 1279, 1280, 1281, 1282, 1283, 1284, 1285, 1286, 1287,
            1288, 1289, 1290, 1291, 1292, 1293, 1294, 1295, 1296, 1297, 1298, 1299, 1300, 1301, 1302, 1303, 1304, 1305,
            1306, 1307, 1308, 1309, 1310, 1311, 1312, 1313, 1314, 1315, 1316, 1317, 1318, 1319, 1320, 1321, 1322, 1323,
            1324, 1325, 1326, 1327, 1328, 1329, 1330, 1331, 1332, 1333, 1334, 1335, 1336, 1337, 1338, 1339, 1340, 1341,
            1342, 1343, 1344, 1345, 1346, 1347, 1348, 1349, 1350, 1351, 1352, 1353, 1354, 1355, 1356, 1357, 1358, 1359,
            1360, 1361, 1362, 1363, 1364, 1365, 1366, 1367, 1368, 1369, 1370, 1371, 1372, 1373, 1374, 1375, 1376, 1377,
            1378, 1379, 1380, 1381, 1382, 1383, 1384, 1385, 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395,
            1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412, 1413,
            1414, 1415, 1416, 1417, 1418, 1419, 1420, 1421, 1422, 1423, 1424, 1425, 1426, 1427, 1428, 1429, 1430, 1431,
            1432, 1433, 1434, 1435, 1436, 1437, 1438, 1439, 1440, 1441, 1442, 1443, 1444, 1445, 1446, 1447, 1448, 1449,
            1450, 1451, 1452, 1453, 1454, 1455, 1456, 1457, 1458, 1459, 1460, 1461, 1462, 1463, 1464, 1465, 1466, 1467,
            1468, 1469, 1470, 1471, 1472, 1473, 1474, 1475, 1476, 1477, 1478, 1479, 1480, 1481, 1482, 1483, 1484, 1485,
            1486, 1487, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503,
            1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 1515, 1516, 1517, 1518, 1519, 1520, 1521,
            1522, 1523, 1524, 1525, 1526, 1527, 1528, 1529, 1530, 1531, 1532, 1533, 1534, 1535, 1536, 1537, 1538, 1539,
            1540, 1541, 1542, 1543, 1544, 1545, 1546, 1547, 1548, 1549, 1550, 1551, 1552, 1553, 1554, 1555, 1556, 1557,
            1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568, 1569, 1570, 1571, 1572, 1573, 1574, 1575,
            1576, 1577, 1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 1589, 1590, 1591, 1592, 1593,
            1594, 1595, 1596, 1597, 1598, 1599, 1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608, 1609, 1610, 1611,
            1612, 1613, 1614, 1615, 1616, 1617, 1618, 1619, 1620, 1621, 1622, 1623, 1624, 1625, 1626, 1627, 1628, 1629,
            1630, 1631, 1632, 1633, 1634, 1635, 1636, 1637, 1638, 1639, 1640, 1641, 1642, 1643, 1644, 1645, 1646, 1647,
            1648, 1649, 1650, 1651, 1652, 1653, 1654, 1655, 1656, 1657, 1658, 1659, 1660, 1661, 1662, 1663, 1664, 1665,
            1666, 1667, 1668, 1669, 1670, 1671, 1672, 1673, 1674, 1675, 1676, 1677, 1678, 1679, 1680, 1681, 1682, 1683,
            1684, 1685, 1686, 1687, 1688, 1689, 1690, 1691, 1692, 1693, 1694, 1695, 1696, 1697, 1698, 1699, 1700, 1701,
            1702, 1703, 1704, 1705, 1706, 1707, 1708, 1709, 1710, 1711, 1712, 1713, 1714, 1715, 1716, 1717, 1718, 1719,
            1720, 1721, 1722, 1723, 1724, 1725, 1726, 1727, 1728, 1729, 1730, 1731, 1732, 1733, 1734, 1735, 1736, 1737,
            1738, 1739, 1740, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748, 1749, 1750, 1751, 1752, 1753, 1754, 1755,
            1756, 1757, 1758, 1759, 1760, 1761, 1762, 1763, 1764, 1765, 1766, 1767, 1768, 1769, 1770, 1771, 1772, 1773,
            1774, 1775, 1776, 1777, 1778, 1779, 1780, 1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1791,
            1792, 1793, 1794, 1795, 1796, 1797, 1798, 1799, 1800, 1801, 1802, 1803, 1804, 1805, 1806, 1807, 1808, 1809,
            1810, 1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823, 1824, 1825, 1826, 1827,
            1828, 1829, 1830, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839, 1840, 1841, 1842, 1843, 1844, 1845,
            1846, 1847, 1848, 1849, 1850, 1851, 1852, 1853, 1854, 1855, 1856, 1857, 1858, 1859, 1860, 1861, 1862, 1863,
            1864, 1865, 1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875, 1876, 1877, 1878, 1879, 1880, 1881,
            1882, 1883, 1884, 1885, 1886, 1887, 1888, 1889, 1890, 1891, 1892, 1893, 1894, 1895, 1896, 1897, 1898, 1899,
            1900, 1901, 1902, 1903, 1904, 1905, 1906, 1907, 1908, 1909, 1910, 1911, 1912, 1913, 1914, 1915, 1916, 1917,
            1918, 1919, 1920, 1921, 1922, 1923, 1924, 1925, 1926, 1927, 1928, 1929, 1930, 1931, 1932, 1933, 1934, 1935,
            1936, 1937, 1938, 1939, 1940, 1941, 1942, 1943, 1944, 1945, 1946, 1947, 1948, 1949, 1950, 1951, 1952, 1953,
            1954, 1955, 1956, 1957, 1958, 1959, 1960, 1961, 1962, 1963, 1964, 1965, 1966, 1967, 1968, 1969, 1970, 1971,
            1972, 1973, 1974, 1975, 1976, 1977, 1978, 1979, 1980, 1981, 1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989,
            1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007,
            2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025,
            2026, 2027, 2028, 2029, 2030, 2031, 2032, 2033, 2034, 2035, 2036, 2037, 2038, 2039, 2040, 2041, 2042, 2043,
            2044, 2045, 2046, 2047, 2048, 2049, 2050, 2051, 2052, 2053, 2054, 2055, 2056, 2057, 2058, 2059, 2060, 2061,
            2062, 2063, 2064, 2065, 2066, 2067, 2068, 2069, 2070, 2071, 2072, 2073, 2074, 2075, 2076, 2077, 2078, 2079,
            2080, 2081, 2082, 2083, 2084, 2085, 2086, 2087, 2088, 2089, 2090, 2091, 2092, 2093, 2094, 2095, 2096, 2097,
            2098, 2099, 2100, 2101, 2102, 2103, 2104, 2105, 2106, 2107, 2108, 2109, 2110, 2111, 2112, 2113, 2114, 2115,
            2116, 2117, 2118, 2119, 2120, 2121, 2122, 2123, 2124, 2125, 2126, 2127, 2128, 2129, 2130, 2131, 2132, 2133,
            2134, 2135, 2136, 2137, 2138, 2139, 2140, 2141, 2142, 2143, 2144, 2145, 2146, 2147, 2148, 2149, 2150, 2151,
            2152, 2153, 2154, 2155, 2156, 2157, 2158, 2159, 2160, 2161, 2162, 2163, 2164, 2165, 2166, 2167, 2168, 2169,
            2170, 2171, 2172, 2173, 2174, 2175, 2176, 2177, 2178, 2179, 2180, 2181, 2182, 2183, 2184, 2185, 2186, 2187,
            2188, 2189, 2190, 2191, 2192, 2193, 2194, 2195, 2196, 2197, 2198, 2199, 2200, 2201, 2202, 2203, 2204, 2205,
            2206, 2207, 2208, 2209, 2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219, 2220, 2221, 2222, 2223,
            2224, 2225, 2226, 2227, 2228, 2229, 2230, 2231, 2232, 2233, 2234, 2235, 2236, 2237, 2238, 2239, 2240, 2241,
            2242, 2243, 2244, 2245, 2246, 2247, 2248, 2249, 2250, 2251, 2252, 2253, 2254, 2255, 2256, 2257, 2258, 2259,
            2260, 2261, 2262, 2263, 2264, 2265, 2266, 2267, 2268, 2269, 2270, 2271, 2272, 2273, 2274, 2275, 2276, 2277,
            2278, 2279, 2280, 2281, 2282, 2283, 2284, 2285, 2286, 2287, 2288, 2289, 2290, 2291, 2292, 2293, 2294, 2295,
            2296, 2297, 2298, 2299, 2300, 2301, 2302, 2303, 2304, 2305, 2306, 2307, 2308, 2309, 2310, 2311, 2312, 2313,
            2314, 2315, 2316, 2317, 2318, 2319, 2320, 2321, 2322, 2323, 2324, 2325, 2326, 2327, 2328, 2329, 2330, 2331,
            2332, 2333, 2334, 2335, 2336, 2337, 2338, 2339, 2340, 2341, 2342, 2343, 2344, 2345, 2346, 2347, 2348, 2349,
            2350, 2351, 2352, 2353, 2354, 2355, 2356, 2357, 2358, 2359, 2360, 2361, 2362, 2363, 2364, 2365, 2366, 2367,
            2368, 2369, 2370, 2371, 2372, 2373, 2374, 2375, 2376, 2377, 2378, 2379, 2380, 2381, 2382, 2383, 2384, 2385,
            2386, 2387, 2388, 2389, 2390, 2391, 2392, 2393, 2394, 2395, 2396, 2397, 2398, 2399, 2400, 2401, 2402, 2403,
            2404, 2405, 2406, 2407, 2408, 2409, 2410, 2411, 2412, 2413, 2414, 2415, 2416, 2417, 2418, 2419, 2420, 2421,
            2422, 2423, 2424, 2425, 2426, 2427, 2428, 2429, 2430, 2431, 2432, 2433, 2434, 2435, 2436, 2437, 2438, 2439,
            2440, 2441, 2442, 2443, 2444, 2445, 2446, 2447, 2448, 2449, 2450, 2451, 2452, 2453, 2454, 2455, 2456, 2457,
            2458, 2459, 2460, 2461, 2462, 2463, 2464, 2465, 2466, 2467, 2468, 2469, 2470, 2471, 2472, 2473, 2474, 2475,
            2476, 2477, 2478, 2479, 2480, 2481, 2482, 2483, 2484, 2485, 2486, 2487, 2488, 2489, 2490, 2491, 2492, 2493,
            2494, 2495, 2496, 2497, 2498, 2499, 2500, 2501, 2502, 2503, 2504, 2505, 2506, 2507, 2508, 2509, 2510, 2511,
            2512, 2513, 2514, 2515, 2516, 2517, 2518, 2519, 2520, 2521, 2522, 2523, 2524, 2525, 2526, 2527, 2528, 2529,
            2530, 2531, 2532, 2533, 2534, 2535, 2536, 2537, 2538, 2539, 2540, 2541, 2542, 2543, 2544, 2545, 2546, 2547,
            2548, 2549, 2550, 2551, 2552, 2553, 2554, 2555, 2556, 2557, 2558, 2559, 2560, 2561, 2562, 2563, 2564, 2565,
            2566, 2567, 2568, 2569, 2570, 2571, 2572, 2573, 2574, 2575, 2576, 2577, 2578, 2579, 2580, 2581, 2582, 2583,
            2584, 2585, 2586, 2587, 2588, 2589, 2590, 2591, 2592, 2593, 2594, 2595, 2596, 2597, 2598, 2599, 2600, 2601,
            2602, 2603, 2604, 2605, 2606, 2607, 2608, 2609, 2610, 2611, 2612, 2613, 2614, 2615, 2616, 2617, 2618, 2619,
            2620, 2621, 2622, 2623, 2624, 2625, 2626, 2627, 2628, 2629, 2630, 2631, 2632, 2633, 2634, 2635, 2636, 2637,
            2638, 2639, 2640, 2641, 2642, 2643, 2644, 2645, 2646, 2647, 2648, 2649, 2650, 2651, 2652, 2653, 2654, 2655,
            2656, 2657, 2658, 2659, 2660, 2661, 2662, 2663, 2664, 2665, 2666, 2667, 2668, 2669, 2670, 2671, 2672, 2673,
            2674, 2675, 2676, 2677, 2678, 2679, 2680, 2681, 2682, 2683, 2684, 2685, 2686, 2687, 2688, 2689, 2690, 2691,
            2692, 2693, 2694, 2695, 2696, 2697, 2698, 2699, 2700, 2701, 2702, 2703, 2704, 2705, 2706, 2707, 2708, 2709,
            2710, 2711, 2712, 2713, 2714, 2715, 2716, 2717, 2718, 2719, 2720, 2721, 2722, 2723, 2724, 2725, 2726, 2727,
            2728, 2729, 2730, 2731, 2732, 2733, 2734, 2735, 2736, 2737, 2738, 2739, 2740, 2741, 2742, 2743, 2744, 2745,
            2746, 2747, 2748, 2749, 2750, 2751, 2752, 2753, 2754, 2755, 2756, 2757, 2758, 2759, 2760, 2761, 2762, 2763,
            2764, 2765, 2766, 2767, 2768, 2769, 2770, 2771, 2772, 2773, 2774, 2775, 2776, 2777, 2778, 2779, 2780, 2781,
            2782, 2783, 2784, 2785, 2786, 2787, 2788, 2789, 2790, 2791, 2792, 2793, 2794, 2795, 2796, 2797, 2798, 2799,
            2800, 2801, 2802, 2803, 2804, 2805, 2806, 2807, 2808, 2809, 2810, 2811, 2812, 2813, 2814, 2815, 2816, 2817,
            2818, 2819, 2820, 2821, 2822, 2823, 2824, 2825, 2826, 2827, 2828, 2829, 2830, 2831, 2832, 2833, 2834, 2835,
            2836, 2837, 2838, 2839, 2840, 2841, 2842, 2843, 2844, 2845, 2846, 2847, 2848, 2849, 2850, 2851, 2852, 2853,
            2854, 2855, 2856, 2857, 2858, 2859, 2860, 2861, 2862, 2863, 2864, 2865, 2866, 2867, 2868, 2869, 2870, 2871,
            2872, 2873, 2874, 2875, 2876, 2877, 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885, 2886, 2887, 2888, 2889,
            2890, 2891, 2892, 2893, 2894, 2895, 2896, 2897, 2898, 2899, 2900, 2901, 2902, 2903, 2904, 2905, 2906, 2907,
            2908, 2909, 2910, 2911, 2912, 2913, 2914, 2915, 2916, 2917, 2918, 2919, 2920, 2921, 2922, 2923, 2924, 2925,
            2926, 2927, 2928, 2929, 2930, 2931, 2932, 2933, 2934, 2935, 2936, 2937, 2938, 2939, 2940, 2941, 2942, 2943,
            2944, 2945, 2946, 2947, 2948, 2949, 2950, 2951, 2952, 2953, 2954, 2955, 2956, 2957, 2958, 2959, 2960, 2961,
            2962, 2963, 2964, 2965, 2966, 2967, 2968, 2969, 2970, 2971, 2972, 2973, 2974, 2975, 2976, 2977, 2978, 2979,
            2980, 2981, 2982, 2983, 2984, 2985, 2986, 2987, 2988, 2989, 2990, 2991, 2992, 2993, 2994, 2995, 2996, 2997,
            2998, 2999, 3000, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010, 3011, 3012, 3013, 3014, 3015,
            3016, 3017, 3018, 3019, 3020, 3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028, 3029, 3030, 3031, 3032, 3033,
            3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3044, 3045, 3046, 3047, 3048, 3049, 3050, 3051,
            3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3061, 3062, 3063, 3064, 3065, 3066, 3067, 3068, 3069,
            3070, 3071, 3072, 3073, 3074, 3075, 3076, 3077, 3078, 3079, 3080, 3081, 3082, 3083, 3084, 3085, 3086, 3087,
            3088, 3089, 3090, 3091, 3092, 3093, 3094, 3095, 3096, 3097, 3098, 3099, 3100, 3101, 3102, 3103, 3104, 3105,
            3106, 3107, 3108, 3109, 3110, 3111, 3112, 3113, 3114, 3115, 3116, 3117, 3118, 3119, 3120, 3121, 3122, 3123,
            3124, 3125, 3126, 3127, 3128, 3129, 3130, 3131, 3132, 3133, 3134, 3135, 3136, 3137, 3138, 3139, 3140, 3141,
            3142, 3143, 3144, 3145, 3146, 3147, 3148, 3149, 3150, 3151, 3152, 3153, 3154, 3155, 3156, 3157, 3158, 3159,
            3160, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3169, 3170, 3171, 3172, 3173, 3174, 3175, 3176, 3177,
            3178, 3179, 3180, 3181, 3182, 3183, 3184, 3185, 3186, 3187, 3188, 3189, 3190, 3191, 3192, 3193, 3194, 3195,
            3196, 3197, 3198, 3199, 3200, 3201, 3202, 3203, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213,
            3214, 3215, 3216, 3217, 3218, 3219, 3220, 3221, 3222, 3223, 3224, 3225, 3226, 3227, 3228, 3229, 3230, 3231,
            3232, 3233, 3234, 3235, 3236, 3237, 3238, 3239, 3240, 3241, 3242, 3243, 3244, 3245, 3246, 3247, 3248, 3249,
            3250, 3251, 3252, 3253, 3254, 3255, 3256, 3257, 3258, 3259, 3260, 3261, 3262, 3263, 3264, 3265, 3266, 3267,
            3268, 3269, 3270, 3271, 3272, 3273, 3274, 3275, 3276, 3277, 3278, 3279, 3280, 3281, 3282, 3283, 3284, 3285,
            3286, 3287, 3288, 3289, 3290, 3291, 3292, 3293, 3294, 3295, 3296, 3297, 3298, 3299, 3300, 3301, 3302, 3303,
            3304, 3305, 3306, 3307, 3308, 3309, 3310, 3311, 3312, 3313, 3314, 3315, 3316, 3317, 3318, 3319, 3320, 3321,
            3322, 3323, 3324, 3325, 3326, 3327, 3328, 3329, 3330, 3331, 3332, 3333, 3334, 3335, 3336, 3337, 3338, 3339,
            3340, 3341, 3342, 3343, 3344, 3345, 3346, 3347, 3348, 3349, 3350, 3351, 3352, 3353, 3354, 3355, 3356, 3357,
            3358, 3359, 3360, 3361, 3362, 3363, 3364, 3365, 3366, 3367, 3368, 3369, 3370, 3371, 3372, 3373, 3374, 3377,
            3378, 3379, 3380, 3381, 3382, 3383, 3384, 3385, 3386, 3387, 3388, 3389, 3390, 3391, 3392, 3393, 3394, 3395,
            3396, 3397, 3398, 3399, 3400, 3401, 3402, 3403, 3404, 3405, 3406, 3407, 3408, 3409, 3425, 3426, 3427, 3428,
            3429, 65535
        };

        public static readonly ushort[] G4Chars =
        {
            12288, 12353, 12354, 12355, 12356, 12357, 12358, 12359, 12360, 12361, 12362, 12363,
            12364, 12365, 12366, 12367, 12368, 12369, 12370, 12371, 12372, 12373, 12374, 12375, 12376, 12377, 12378,
            12379, 12380, 12381, 12382, 12383, 12384, 12385, 12386, 12387, 12388, 12389, 12390, 12391, 12392, 12393,
            12394, 12395, 12396, 12397, 12398, 12399, 12400, 12401, 12402, 12403, 12404, 12405, 12406, 12407, 12408,
            12409, 12410, 12411, 12412, 12413, 12414, 12415, 12416, 12417, 12418, 12419, 12420, 12421, 12422, 12423,
            12424, 12425, 12426, 12427, 12428, 12429, 12431, 12434, 12435, 12449, 12450, 12451, 12452, 12453, 12454,
            12455, 12456, 12457, 12458, 12459, 12460, 12461, 12462, 12463, 12464, 12465, 12466, 12467, 12468, 12469,
            12470, 12471, 12472, 12473, 12474, 12475, 12476, 12477, 12478, 12479, 12480, 12481, 12482, 12483, 12484,
            12485, 12486, 12487, 12488, 12489, 12490, 12491, 12492, 12493, 12494, 12495, 12496, 12497, 12498, 12499,
            12500, 12501, 12502, 12503, 12504, 12505, 12506, 12507, 12508, 12509, 12510, 12511, 12512, 12513, 12514,
            12515, 12516, 12517, 12518, 12519, 12520, 12521, 12522, 12523, 12524, 12525, 12527, 12530, 12531, 65296,
            65297, 65298, 65299, 65300, 65301, 65302, 65303, 65304, 65305, 65313, 65314, 65315, 65316, 65317, 65318,
            65319, 65320, 65321, 65322, 65323, 65324, 65325, 65326, 65327, 65328, 65329, 65330, 65331, 65332, 65333,
            65334, 65335, 65336, 65337, 65338, 65345, 65346, 65347, 65348, 65349, 65350, 65351, 65352, 65353, 65354,
            65355, 65356, 65357, 65358, 65359, 65360, 65361, 65362, 65363, 65364, 65365, 65366, 65367, 65368, 65369,
            65370, 65281, 65311, 12289, 12290, 8943, 12539, 65295, 12300, 12301, 12302, 12303, 65288, 65289, 9325, 9326,
            65291, 12540, 9319, 9320, 65309, 65370, 65306, 65307, 65294, 65292, 9327, 9328, 9329, 9330, 9331, 9332, 9333,
            9334, 9335, 9336, 65312, 9337, 65285, 9338, 9339, 9341, 10052, 9739, 9812, 9813, 9738, 8663, 8664, 9790, 165,
            9800, 9801, 9802, 9803, 9804, 9805, 9806, 9807, 8592, 8593, 8595, 8594, 8227, 65286, 48, 49, 50, 51, 52, 53,
            54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87,
            88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
            117, 118, 119, 120, 121, 122, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
            230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
            252, 253, 254, 255, 338, 339, 350, 351, 170, 186, 185, 178, 179, 36, 161, 191, 33, 63, 44, 46, 9324, 65381,
            47, 8216, 8217, 8220, 8221, 8222, 12298, 12299, 40, 41, 9794, 9792, 43, 45, 42, 35, 61, 38, 126, 58, 59,
            9327, 9328, 9329, 9330, 9331, 9332, 9333, 9334, 9335, 9336, 64, 9337, 37, 9338, 9339, 9340, 9341, 9342, 9343,
            9344, 9345, 9346, 9347, 9348, 32, 9349, 9350, 9351, 176, 95, 65343, 44032, 44033, 44036, 44039, 44040, 44041,
            44042, 44048, 44049, 44050, 44051, 44052, 44053, 44054, 44055, 44057, 44058, 44059, 44060, 44061, 44064,
            44068, 44076, 44077, 44079, 44080, 44081, 44088, 44089, 44092, 44096, 44107, 44109, 44116, 44120, 44124,
            44144, 44145, 44148, 44151, 44152, 44154, 44160, 44161, 44163, 44164, 44165, 44166, 44169, 44170, 44171,
            44172, 44176, 44180, 44188, 44189, 44191, 44192, 44193, 44200, 44201, 44202, 44204, 44207, 44208, 44216,
            44217, 44219, 44220, 44221, 44225, 44228, 44232, 44236, 44245, 44247, 44256, 44257, 44260, 44263, 44264,
            44266, 44268, 44271, 44272, 44273, 44275, 44277, 44278, 44284, 44285, 44288, 44292, 44294, 44300, 44301,
            44303, 44305, 44312, 44316, 44320, 44329, 44332, 44333, 44340, 44341, 44344, 44348, 44356, 44357, 44359,
            44361, 44368, 44372, 44376, 44385, 44387, 44396, 44397, 44400, 44403, 44404, 44405, 44406, 44411, 44412,
            44413, 44415, 44417, 44418, 44424, 44425, 44428, 44432, 44444, 44445, 44452, 44471, 44480, 44481, 44484,
            44488, 44496, 44497, 44499, 44508, 44512, 44516, 44536, 44537, 44540, 44543, 44544, 44545, 44552, 44553,
            44555, 44557, 44564, 44592, 44593, 44596, 44599, 44600, 44602, 44608, 44609, 44611, 44613, 44614, 44618,
            44620, 44621, 44622, 44624, 44628, 44630, 44636, 44637, 44639, 44640, 44641, 44645, 44648, 44649, 44652,
            44656, 44664, 44665, 44667, 44668, 44669, 44676, 44677, 44684, 44732, 44733, 44734, 44736, 44740, 44748,
            44749, 44751, 44752, 44753, 44760, 44761, 44764, 44776, 44779, 44781, 44788, 44792, 44796, 44807, 44808,
            44813, 44816, 44844, 44845, 44848, 44850, 44852, 44860, 44861, 44863, 44865, 44866, 44867, 44872, 44873,
            44880, 44892, 44893, 44900, 44901, 44921, 44928, 44932, 44936, 44944, 44945, 44949, 44956, 44984, 44985,
            44988, 44992, 44999, 45000, 45001, 45003, 45005, 45006, 45012, 45020, 45032, 45033, 45040, 45041, 45044,
            45048, 45056, 45057, 45060, 45068, 45072, 45076, 45084, 45085, 45096, 45124, 45125, 45128, 45130, 45132,
            45134, 45139, 45140, 45141, 45143, 45145, 45149, 45180, 45181, 45184, 45188, 45196, 45197, 45199, 45201,
            45208, 45209, 45210, 45212, 45215, 45216, 45217, 45218, 45224, 45225, 45227, 45228, 45229, 45230, 45231,
            45233, 45235, 45236, 45237, 45240, 45244, 45252, 45253, 45255, 45256, 45257, 45264, 45265, 45268, 45272,
            45280, 45285, 45320, 45321, 45323, 45324, 45328, 45330, 45331, 45336, 45337, 45339, 45340, 45341, 45347,
            45348, 45349, 45352, 45356, 45364, 45365, 45367, 45368, 45369, 45376, 45377, 45380, 45384, 45392, 45393,
            45396, 45397, 45400, 45404, 45408, 45432, 45433, 45436, 45440, 45442, 45448, 45449, 45451, 45453, 45458,
            45459, 45460, 45464, 45468, 45480, 45516, 45520, 45524, 45532, 45533, 45535, 45544, 45545, 45548, 45552,
            45561, 45563, 45565, 45572, 45573, 45576, 45579, 45580, 45588, 45589, 45591, 45593, 45600, 45620, 45628,
            45656, 45660, 45664, 45672, 45673, 45684, 45685, 45692, 45700, 45701, 45705, 45712, 45713, 45716, 45720,
            45721, 45722, 45728, 45729, 45731, 45733, 45734, 45738, 45740, 45744, 45748, 45768, 45769, 45772, 45776,
            45778, 45784, 45785, 45787, 45789, 45794, 45796, 45797, 45798, 45800, 45803, 45804, 45805, 45806, 45807,
            45811, 45812, 45813, 45815, 45816, 45817, 45818, 45819, 45823, 45824, 45825, 45828, 45832, 45840, 45841,
            45843, 45844, 45845, 45852, 45908, 45909, 45910, 45912, 45915, 45916, 45918, 45919, 45924, 45925, 45927,
            45929, 45931, 45934, 45936, 45937, 45940, 45944, 45952, 45953, 45955, 45956, 45957, 45964, 45968, 45972,
            45984, 45985, 45992, 45996, 46020, 46021, 46024, 46027, 46028, 46030, 46032, 46036, 46037, 46039, 46041,
            46043, 46045, 46048, 46052, 46056, 46076, 46096, 46104, 46108, 46112, 46120, 46121, 46123, 46132, 46160,
            46161, 46164, 46168, 46176, 46177, 46179, 46181, 46188, 46208, 46216, 46237, 46244, 46248, 46252, 46261,
            46263, 46265, 46272, 46276, 46280, 46288, 46293, 46300, 46301, 46304, 46307, 46308, 46310, 46316, 46317,
            46319, 46321, 46328, 46356, 46357, 46360, 46363, 46364, 46372, 46373, 46375, 46376, 46377, 46378, 46384,
            46385, 46388, 46392, 46400, 46401, 46403, 46404, 46405, 46411, 46412, 46413, 46416, 46420, 46428, 46429,
            46431, 46432, 46433, 46496, 46497, 46500, 46504, 46506, 46507, 46512, 46513, 46515, 46516, 46517, 46523,
            46524, 46525, 46528, 46532, 46540, 46541, 46543, 46544, 46545, 46552, 46572, 46608, 46609, 46612, 46616,
            46629, 46636, 46644, 46664, 46692, 46696, 46748, 46749, 46752, 46756, 46763, 46764, 46769, 46804, 46832,
            46836, 46840, 46848, 46849, 46853, 46888, 46889, 46892, 46895, 46896, 46904, 46905, 46907, 46916, 46920,
            46924, 46932, 46933, 46944, 46948, 46952, 46960, 46961, 46963, 46965, 46972, 46973, 46976, 46980, 46988,
            46989, 46991, 46992, 46993, 46994, 46998, 46999, 47000, 47001, 47004, 47008, 47016, 47017, 47019, 47020,
            47021, 47028, 47029, 47032, 47047, 47049, 47084, 47085, 47088, 47092, 47100, 47101, 47103, 47104, 47105,
            47111, 47112, 47113, 47116, 47120, 47128, 47129, 47131, 47133, 47140, 47141, 47144, 47148, 47156, 47157,
            47159, 47160, 47161, 47168, 47172, 47185, 47187, 47196, 47197, 47200, 47204, 47212, 47213, 47215, 47217,
            47224, 47228, 47245, 47272, 47280, 47284, 47288, 47296, 47297, 47299, 47301, 47308, 47312, 47316, 47325,
            47327, 47329, 47336, 47337, 47340, 47344, 47352, 47353, 47355, 47357, 47364, 47384, 47392, 47420, 47421,
            47424, 47428, 47436, 47439, 47441, 47448, 47449, 47452, 47456, 47464, 47465, 47467, 47469, 47476, 47477,
            47480, 47484, 47492, 47493, 47495, 47497, 47498, 47501, 47502, 47532, 47533, 47536, 47540, 47548, 47549,
            47551, 47553, 47560, 47561, 47564, 47566, 47567, 47568, 47569, 47570, 47576, 47577, 47579, 47581, 47582,
            47585, 47587, 47588, 47589, 47592, 47596, 47604, 47605, 47607, 47608, 47609, 47610, 47616, 47617, 47624,
            47637, 47672, 47673, 47676, 47680, 47682, 47688, 47689, 47691, 47693, 47694, 47699, 47700, 47701, 47704,
            47708, 47716, 47717, 47719, 47720, 47721, 47728, 47729, 47732, 47736, 47747, 47748, 47749, 47751, 47756,
            47784, 47785, 47787, 47788, 47792, 47794, 47800, 47801, 47803, 47805, 47812, 47816, 47832, 47833, 47868,
            47872, 47876, 47885, 47887, 47889, 47896, 47900, 47904, 47913, 47915, 47924, 47925, 47926, 47928, 47931,
            47932, 47933, 47934, 47940, 47941, 47943, 47945, 47949, 47951, 47952, 47956, 47960, 47969, 47971, 47980,
            48008, 48012, 48016, 48036, 48040, 48044, 48052, 48055, 48064, 48068, 48072, 48080, 48083, 48120, 48121,
            48124, 48127, 48128, 48130, 48136, 48137, 48139, 48140, 48141, 48143, 48145, 48148, 48149, 48150, 48151,
            48152, 48155, 48156, 48157, 48158, 48159, 48164, 48165, 48167, 48169, 48173, 48176, 48177, 48180, 48184,
            48192, 48193, 48195, 48196, 48197, 48201, 48204, 48205, 48208, 48221, 48260, 48261, 48264, 48267, 48268,
            48270, 48276, 48277, 48279, 48281, 48282, 48288, 48289, 48292, 48295, 48296, 48304, 48305, 48307, 48308,
            48309, 48316, 48317, 48320, 48324, 48333, 48335, 48336, 48337, 48341, 48344, 48348, 48372, 48373, 48374,
            48376, 48380, 48388, 48389, 48391, 48393, 48400, 48404, 48420, 48428, 48448, 48456, 48457, 48460, 48464,
            48472, 48473, 48484, 48488, 48512, 48513, 48516, 48519, 48520, 48521, 48522, 48528, 48529, 48531, 48533,
            48537, 48538, 48540, 48548, 48560, 48568, 48596, 48597, 48600, 48604, 48617, 48624, 48628, 48632, 48640,
            48643, 48645, 48652, 48653, 48656, 48660, 48668, 48669, 48671, 48708, 48709, 48712, 48716, 48718, 48724,
            48725, 48727, 48729, 48730, 48731, 48736, 48737, 48740, 48744, 48746, 48752, 48753, 48755, 48756, 48757,
            48763, 48764, 48765, 48768, 48772, 48780, 48781, 48783, 48784, 48785, 48792, 48793, 48808, 48848, 48849,
            48852, 48855, 48856, 48864, 48867, 48868, 48869, 48876, 48897, 48904, 48905, 48920, 48921, 48923, 48924,
            48925, 48960, 48961, 48964, 48968, 48976, 48977, 48981, 49044, 49072, 49093, 49100, 49101, 49104, 49108,
            49116, 49119, 49121, 49212, 49233, 49240, 49244, 49248, 49256, 49257, 49296, 49297, 49300, 49304, 49312,
            49313, 49315, 49317, 49324, 49325, 49327, 49328, 49331, 49332, 49333, 49334, 49340, 49341, 49343, 49344,
            49345, 49349, 49352, 49353, 49356, 49360, 49368, 49369, 49371, 49372, 49373, 49380, 49381, 49384, 49388,
            49396, 49397, 49399, 49401, 49408, 49412, 49416, 49424, 49429, 49436, 49437, 49438, 49439, 49440, 49443,
            49444, 49446, 49447, 49452, 49453, 49455, 49456, 49457, 49462, 49464, 49465, 49468, 49472, 49480, 49481,
            49483, 49484, 49485, 49492, 49493, 49496, 49500, 49508, 49509, 49511, 49512, 49513, 49520, 49524, 49528,
            49541, 49548, 49549, 49550, 49552, 49556, 49558, 49564, 49565, 49567, 49569, 49573, 49576, 49577, 49580,
            49584, 49597, 49604, 49608, 49612, 49620, 49623, 49624, 49632, 49636, 49640, 49648, 49649, 49651, 49660,
            49661, 49664, 49668, 49676, 49677, 49679, 49681, 49688, 49689, 49692, 49695, 49696, 49704, 49705, 49707,
            49709, 49711, 49713, 49714, 49716, 49736, 49744, 49745, 49748, 49752, 49760, 49765, 49772, 49773, 49776,
            49780, 49788, 49789, 49791, 49793, 49800, 49801, 49808, 49816, 49819, 49821, 49828, 49829, 49832, 49836,
            49837, 49844, 49845, 49847, 49849, 49884, 49885, 49888, 49891, 49892, 49899, 49900, 49901, 49903, 49905,
            49910, 49912, 49913, 49915, 49916, 49920, 49928, 49929, 49932, 49933, 49939, 49940, 49941, 49944, 49948,
            49956, 49957, 49960, 49961, 49989, 50024, 50025, 50028, 50032, 50034, 50040, 50041, 50044, 50045, 50052,
            50056, 50060, 50112, 50136, 50137, 50140, 50143, 50144, 50146, 50152, 50153, 50157, 50164, 50165, 50168,
            50184, 50192, 50212, 50220, 50224, 50228, 50236, 50237, 50248, 50276, 50277, 50280, 50284, 50292, 50293,
            50297, 50304, 50324, 50332, 50360, 50364, 50409, 50416, 50417, 50420, 50424, 50426, 50431, 50432, 50433,
            50444, 50448, 50452, 50460, 50472, 50473, 50476, 50480, 50488, 50489, 50491, 50493, 50500, 50501, 50504,
            50505, 50506, 50508, 50509, 50510, 50515, 50516, 50517, 50519, 50520, 50521, 50525, 50526, 50528, 50529,
            50532, 50536, 50544, 50545, 50547, 50548, 50549, 50556, 50557, 50560, 50564, 50567, 50572, 50573, 50575,
            50577, 50581, 50583, 50584, 50588, 50592, 50601, 50612, 50613, 50616, 50617, 50619, 50620, 50621, 50622,
            50628, 50629, 50630, 50631, 50632, 50633, 50634, 50636, 50638, 50640, 50641, 50644, 50648, 50656, 50657,
            50659, 50661, 50668, 50669, 50670, 50672, 50676, 50678, 50679, 50684, 50685, 50686, 50687, 50688, 50689,
            50693, 50694, 50695, 50696, 50700, 50704, 50712, 50713, 50715, 50716, 50724, 50725, 50728, 50732, 50733,
            50734, 50736, 50739, 50740, 50741, 50743, 50745, 50747, 50752, 50753, 50756, 50760, 50768, 50769, 50771,
            50772, 50773, 50780, 50781, 50784, 50796, 50799, 50801, 50808, 50809, 50812, 50816, 50824, 50825, 50827,
            50829, 50836, 50837, 50840, 50844, 50852, 50853, 50855, 50857, 50864, 50865, 50868, 50872, 50873, 50874,
            50880, 50881, 50883, 50885, 50892, 50893, 50896, 50900, 50908, 50909, 50912, 50913, 50920, 50921, 50924,
            50928, 50936, 50937, 50941, 50948, 50949, 50952, 50956, 50964, 50965, 50967, 50969, 50976, 50977, 50980,
            50984, 50992, 50993, 50995, 50997, 50999, 51004, 51005, 51008, 51012, 51018, 51020, 51021, 51023, 51025,
            51026, 51027, 51028, 51029, 51030, 51031, 51032, 51036, 51040, 51048, 51051, 51060, 51061, 51064, 51068,
            51069, 51070, 51075, 51076, 51077, 51079, 51080, 51081, 51082, 51086, 51088, 51089, 51092, 51094, 51095,
            51096, 51098, 51104, 51105, 51107, 51108, 51109, 51110, 51116, 51117, 51120, 51124, 51132, 51133, 51135,
            51136, 51137, 51144, 51145, 51148, 51150, 51152, 51160, 51165, 51172, 51176, 51180, 51200, 51201, 51204,
            51208, 51210, 51216, 51217, 51219, 51221, 51222, 51228, 51229, 51232, 51236, 51244, 51245, 51247, 51249,
            51256, 51260, 51264, 51272, 51273, 51276, 51277, 51284, 51312, 51313, 51316, 51320, 51322, 51328, 51329,
            51331, 51333, 51334, 51335, 51339, 51340, 51341, 51348, 51357, 51359, 51361, 51368, 51388, 51389, 51396,
            51400, 51404, 51412, 51413, 51415, 51417, 51424, 51425, 51428, 51445, 51452, 51453, 51456, 51460, 51461,
            51462, 51468, 51469, 51471, 51473, 51480, 51500, 51508, 51536, 51537, 51540, 51544, 51552, 51553, 51555,
            51564, 51568, 51572, 51580, 51592, 51593, 51596, 51600, 51608, 51609, 51611, 51613, 51648, 51649, 51652,
            51655, 51656, 51658, 51664, 51665, 51667, 51669, 51670, 51673, 51674, 51676, 51677, 51680, 51682, 51684,
            51687, 51692, 51693, 51695, 51696, 51697, 51704, 51705, 51708, 51712, 51720, 51721, 51723, 51724, 51725,
            51732, 51736, 51753, 51788, 51789, 51792, 51796, 51804, 51805, 51807, 51808, 51809, 51816, 51837, 51844,
            51864, 51900, 51901, 51904, 51908, 51916, 51917, 51919, 51921, 51923, 51928, 51929, 51936, 51948, 51956,
            51976, 51984, 51988, 51992, 52000, 52001, 52033, 52040, 52041, 52044, 52048, 52056, 52057, 52061, 52068,
            52088, 52089, 52124, 52152, 52180, 52196, 52199, 52201, 52236, 52237, 52240, 52244, 52252, 52253, 52257,
            52258, 52263, 52264, 52265, 52268, 52270, 52272, 52280, 52281, 52283, 52284, 52285, 52286, 52292, 52293,
            52296, 52300, 52308, 52309, 52311, 52312, 52313, 52320, 52324, 52326, 52328, 52336, 52341, 52376, 52377,
            52380, 52384, 52392, 52393, 52395, 52396, 52397, 52404, 52405, 52408, 52412, 52420, 52421, 52423, 52425,
            52432, 52436, 52452, 52460, 52464, 52481, 52488, 52489, 52492, 52496, 52504, 52505, 52507, 52509, 52516,
            52520, 52524, 52537, 52572, 52576, 52580, 52588, 52589, 52591, 52593, 52600, 52616, 52628, 52629, 52632,
            52636, 52644, 52645, 52647, 52649, 52656, 52676, 52684, 52688, 52712, 52716, 52720, 52728, 52729, 52731,
            52733, 52740, 52744, 52748, 52756, 52761, 52768, 52769, 52772, 52776, 52784, 52785, 52787, 52789, 52824,
            52825, 52828, 52831, 52832, 52833, 52840, 52841, 52843, 52845, 52852, 52853, 52856, 52860, 52868, 52869,
            52871, 52873, 52880, 52881, 52884, 52888, 52896, 52897, 52899, 52900, 52901, 52908, 52909, 52929, 52964,
            52965, 52968, 52971, 52972, 52980, 52981, 52983, 52984, 52985, 52992, 52993, 52996, 53000, 53008, 53009,
            53011, 53013, 53020, 53024, 53028, 53036, 53037, 53039, 53040, 53041, 53048, 53076, 53077, 53080, 53084,
            53092, 53093, 53095, 53097, 53104, 53105, 53108, 53112, 53120, 53125, 53132, 53153, 53160, 53168, 53188,
            53216, 53217, 53220, 53224, 53232, 53233, 53235, 53237, 53244, 53248, 53252, 53265, 53272, 53293, 53300,
            53301, 53304, 53308, 53316, 53317, 53319, 53321, 53328, 53332, 53336, 53344, 53356, 53357, 53360, 53364,
            53372, 53373, 53377, 53412, 53413, 53416, 53420, 53428, 53429, 53431, 53433, 53440, 53441, 53444, 53448,
            53449, 53456, 53457, 53459, 53460, 53461, 53468, 53469, 53472, 53476, 53484, 53485, 53487, 53488, 53489,
            53496, 53517, 53552, 53553, 53556, 53560, 53562, 53568, 53569, 53571, 53572, 53573, 53580, 53581, 53584,
            53588, 53596, 53597, 53599, 53601, 53608, 53612, 53628, 53636, 53640, 53664, 53665, 53668, 53672, 53680,
            53681, 53683, 53685, 53690, 53692, 53696, 53720, 53748, 53752, 53767, 53769, 53776, 53804, 53805, 53808,
            53812, 53820, 53821, 53823, 53825, 53832, 53852, 53860, 53888, 53889, 53892, 53896, 53904, 53905, 53909,
            53916, 53920, 53924, 53932, 53937, 53944, 53945, 53948, 53951, 53952, 53954, 53960, 53961, 53963, 53972,
            53976, 53980, 53988, 53989, 54000, 54001, 54004, 54008, 54016, 54017, 54019, 54021, 54028, 54029, 54030,
            54032, 54036, 54038, 54044, 54045, 54047, 54048, 54049, 54053, 54056, 54057, 54060, 54064, 54072, 54073,
            54075, 54076, 54077, 54084, 54085, 54140, 54141, 54144, 54148, 54156, 54157, 54159, 54160, 54161, 54168,
            54169, 54172, 54176, 54184, 54185, 54187, 54189, 54196, 54200, 54204, 54212, 54213, 54216, 54217, 54224,
            54232, 54241, 54243, 54252, 54253, 54256, 54260, 54268, 54269, 54271, 54273, 54280, 54301, 54336, 54340,
            54364, 54368, 54372, 54381, 54383, 54392, 54393, 54396, 54399, 54400, 54402, 54408, 54409, 54411, 54413,
            54420, 54441, 54476, 54480, 54484, 54492, 54495, 54504, 54508, 54512, 54520, 54523, 54525, 54532, 54536,
            54540, 54548, 54549, 54551, 54588, 54589, 54592, 54596, 54604, 54605, 54607, 54609, 54616, 54617, 54620,
            54624, 54629, 54632, 54633, 54635, 54637, 54644, 54645, 54648, 54652, 54660, 54661, 54663, 54664, 54665,
            54672, 54693, 54728, 54729, 54732, 54736, 54738, 54744, 54745, 54747, 54749, 54756, 54757, 54760, 54764,
            54772, 54773, 54775, 54777, 54784, 54785, 54788, 54792, 54800, 54801, 54803, 54804, 54805, 54812, 54816,
            54820, 54829, 54840, 54841, 54844, 54848, 54853, 54856, 54857, 54859, 54861, 54865, 54868, 54869, 54872,
            54876, 54887, 54889, 54896, 54897, 54900, 54915, 54917, 54924, 54925, 54928, 54932, 54941, 54943, 54945,
            54952, 54956, 54960, 54969, 54971, 54980, 54981, 54984, 54988, 54993, 54996, 54999, 55001, 55008, 55012,
            55016, 55024, 55029, 55036, 55037, 55040, 55044, 55057, 55064, 55065, 55068, 55072, 55080, 55081, 55083,
            55085, 55092, 55093, 55096, 55100, 55108, 55111, 55113, 55120, 55121, 55124, 55126, 55127, 55128, 55129,
            55136, 55137, 55139, 55141, 55145, 55148, 55152, 55156, 55164, 55165, 55169, 55176, 55177, 55180, 55184,
            55192, 55193, 55195, 55197, 4352, 4353, 4354, 4355, 4356, 4357, 4358, 4359, 4360, 4361, 4362, 4363, 4364,
            4365, 4366, 4367, 4368, 4369, 4370, 4449, 4450, 4451, 4452, 4453, 4454, 4455, 4456, 4457, 4461, 4462, 4466,
            4467, 4469, 47252, 49968, 50108, 50388, 52012, 65535
        };

        public static readonly ushort[] G34_4E =
        {
            478, 351, 352, 353, 358, 359, 360, 361, 362, 363, 020, 365, 366, 369, 370, 371, // 0
            415, 376, 377, 378, 368, 382, 383, 384, 046, 358, 359, 392, 393, 394, 395, 396, // 1
            397, 398, 401, 402, 403, 416, 408, 409, 410, 400, 420, 419, 479, 450, 445, 003, // 2
            004, 006, 008, 010, 068, 449, 072, 013, 015, 017, 019, 021, 023, 025, 027, 029, // 3
            031, 033, 035, 038, 040, 042, 049, 052, 055, 058, 061, 050, 053, 056, 059, 062, // 4
            036, 426, 425, 480, 481, 091, 092, 094, 096, 098, 364, 100, 102, 106, 108, 110, // 5
            112, 114, 117, 119, 121, 123, 124, 125, 385, 127, 128, 131, 134, 137, 140, 396, // 6
            144, 145, 146, 147, 149, 151, 153, 154, 155, 156, 157, 158, 159, 160, 161, 082, // 7
            084, 086, 088, 090, 148, 150, 152, 093, 095, 097, 099, 101, 103, 105, 107, 109, // 8
            111, 113, 115, 118, 120, 122, 129, 132, 135, 138, 141, 130, 133, 136, 139, 142, // 9
            116, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 427, 428, 430, 446, 230, // A
            431, 436, 437, 434, 435, 443, 444, 424, 429, 242, 433, 299, 300, 301, 302, 303, // B
            304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, // C
            320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, // D
            336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 289, // E
            452, 355, 373, 379, 387, 405, 411                                               // F
        };

        public static readonly ushort[] G34_4J =
        {
            001, 003, 005, 007, 009, 011, 012, 014, 016, 018, 020, 022, 024, 026, 028, 030, // 0
            032, 034, 037, 039, 041, 043, 044, 045, 046, 047, 048, 051, 054, 057, 060, 063, // 1
            064, 065, 066, 067, 069, 071, 073, 074, 075, 076, 077, 078, 079, 080, 081, 002, // 2
            004, 006, 008, 010, 068, 070, 072, 013, 015, 017, 019, 021, 023, 025, 027, 029, // 3
            031, 033, 035, 038, 040, 042, 049, 052, 055, 058, 061, 050, 053, 056, 059, 062, // 4
            036, 083, 085, 087, 089, 091, 092, 094, 096, 098, 100, 102, 104, 106, 108, 110, // 5
            112, 114, 117, 119, 121, 123, 124, 125, 126, 127, 128, 131, 134, 137, 140, 143, // 6
            144, 145, 146, 147, 149, 151, 153, 154, 155, 156, 157, 158, 159, 160, 161, 082, // 7
            084, 086, 088, 090, 148, 150, 152, 093, 095, 097, 099, 101, 103, 105, 107, 109, // 8
            111, 113, 115, 118, 120, 122, 129, 132, 135, 138, 141, 130, 133, 136, 139, 142, // 9
            116, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 225, 226, 228, 241, 230, // A
            229, 234, 235, 232, 233, 443, 444, 424, 430, 242, 433, 172, 173, 174, 175, 176, // B
            177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, // C
            193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, // D
            209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 289, // E
            452, 355, 373, 379, 387, 405, 411                                               // F
        };
        #endregion
        #region Gen7 Chinese Character Tables
        public static readonly char[] Gen7_CHS = Util.getStringList("Char", "zh")[0].ToCharArray();
        public const ushort Gen7_CHS_Ofs = 0xE800;
        public static readonly char[] Gen7_CHT = Util.getStringList("Char", "zh2")[0].ToCharArray();
        public const ushort Gen7_CHT_Ofs = 0xEB0F;
        #endregion
        /// <summary>
        /// Trash Bytes for Generation 3->4
        /// </summary>
        /// <remarks>String buffers are reused, data is not cleared which yields the trash bytes.</remarks>
        public static readonly byte[][] G4TransferTrashBytes = {
            new byte[] { }, // Unused
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { }, // Unused
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
        };

        /// <summary>
        /// Decrypts an 80 byte format <see cref="PK3"/> byte array.
        /// </summary>
        /// <param name="ekm">Encrypted data.</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] decryptArray3(byte[] ekm)
        {
            if (ekm.Length != SIZE_3PARTY && ekm.Length != SIZE_3STORED)
                return null;

            uint PID = BitConverter.ToUInt32(ekm, 0);
            uint OID = BitConverter.ToUInt32(ekm, 4);
            uint seed = PID ^ OID;

            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i % 4];
            return shuffleArray3(ekm, PID%24);
        }

        /// <summary>
        /// Shuffles an 80 byte format <see cref="PK3"/> byte array.
        /// </summary>
        /// <param name="data">Unshuffled data.</param>
        /// <param name="sv">Block order shuffle value</param>
        /// <returns></returns>
        public static byte[] shuffleArray3(byte[] data, uint sv)
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
        public static byte[] encryptArray3(byte[] pkm)
        {
            if (pkm.Length != SIZE_3PARTY && pkm.Length != SIZE_3STORED)
                return null;

            uint PID = BitConverter.ToUInt32(pkm, 0);
            uint OID = BitConverter.ToUInt32(pkm, 4);
            uint seed = PID ^ OID;

            byte[] ekm = shuffleArray3(pkm, blockPositionInvert[PID%24]);
            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i % 4];
            return ekm;
        }

        /// <summary>Unused item, placeholder for item/sprite finding</summary>
        private const ushort ITEM_UNK = 128;

        /// <summary>
        /// Converts a Generation 2 Item ID to Generation 4+ Item ID.
        /// </summary>
        /// <param name="g2val">Generation 2 Item ID.</param>
        /// <returns>Generation 4+ Item ID.</returns>
        public static ushort getG4Item(byte g2val)
        {
            ushort[] arr =
            {
                00000, 00001, 00002, 00213, 00003, 00004, 65535, 00450, 00081, 00018,
                00019, 00020, 00021, 00022, 00023, 00024, 00025, 00026, 00017, 00078,
                00079, 00041, 00082, 00083, 00084, 65535, 00045, 00046, 00047, 00048,
                00256, 00049, 00050, 00060, 00085, 00257, 00092, 00063, 00027, 00028,
                00029, 00055, 00076, 00077, 00056, 65535, 00030, 00031, 00032, 00057,
                65535, 00058, 00059, 00061, 00444, 65535, 65535, 00216, 00445, 00446,
                65535, 00447, 00051, 00038, 00039, 00040, 00478, 00464, 00456, 00484,
                65535, 00482, 00033, 00217, 65535, 65535, 00237, 00244, 65535, 65535,
                65535, 00245, 00221, 65535, 65535, 00485, 00086, 00087, 00222, 00487,
                65535, 00223, 00486, 00488, 00224, 00243, 00248, 00490, 00241, 00491,
                65535, 00489, 00240, 00473, 65535, 00259, 00228, 00246, 00242, 65535,
                00088, 00089, 00229, 00247, 00504, 65535, 65535, 00239, 00258, 00230,
                65535, 00034, 00035, 00036, 00037, 00238, 00231, 00475, 00481, 65535,
                65535, 00090, 00091, 00476, 00480, 65535, 65535, 65535, 00249, 00043,
                00232, 65535, 65535, 00233, 00250, 65535, 00234, 65535, 65535, 65535,
                65535, 00235, 65535, 65535, 65535, 65535, 00044, 00495, 65535, 00493,
                65535, 00492, 65535, 00236, 00497, 00498, 00496, 65535, 65535, 00080,
                65535, 65535, 00252, 65535, 65535, 00477, 65535, 00500, 00483, 65535,
                65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
                65535, 00328, 00329, 00330, 00331, 00331, 00332, 00333, 00334, 00335,
                00336, 00337, 00338, 00339, 00340, 00341, 00342, 00343, 00344, 00345,
                00346, 00347, 00348, 00349, 00350, 00351, 00352, 00353, 00354, 00355,
                00355, 00356, 00357, 00358, 00359, 00360, 00361, 00362, 00363, 00364,
                00365, 00366, 00367, 00368, 00369, 00370, 00371, 00372, 00373, 00374,
                00375, 00376, 00377, 00420, 00421, 00422, 00423, 00424, 00425, 00426,
                00427, 65535, 65535, 65535, 65535, 65535,
            };

            ushort val = g2val > arr.Length ? ushort.MaxValue : arr[g2val];
            return val == ushort.MaxValue ? ITEM_UNK : val;
        }

        /// <summary>
        /// Converts a Generation 3 Item ID to Generation 4+ Item ID.
        /// </summary>
        /// <param name="g3val">Generation 3 Item ID.</param>
        /// <returns>Generation 4+ Item ID.</returns>
        public static ushort getG4Item(ushort g3val)
        {
            ushort[] arr =
            {
                00000,00001,00002,00003,00004,00005,00006,00007,00008,00009,
                00010,00011,00012,00017,00018,00019,00020,00021,00022,00023,
                00024,00025,00026,00027,00028,00029,00030,00031,00032,00033,
                00034,00035,00036,00037,00038,00039,00040,00041,00042,00065,
                00066,00067,00068,00069,00043,00044,00070,00071,00072,00073,
                00074,00075,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,00045,00046,00047,00048,00049,00050,00051,
                00052,00053,65535,00055,00056,00057,00058,00059,00060,00061,
                00063,00064,65535,00076,00077,00078,00079,65535,65535,65535,
                65535,65535,65535,00080,00081,00082,00083,00084,00085,65535,
                65535,65535,65535,00086,00087,65535,00088,00089,00090,00091,
                00092,00093,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,00149,00150,00151,00152,00153,00154,00155,
                00156,00157,00158,00159,00160,00161,00162,00163,00164,00165,
                00166,00167,00168,00169,00170,00171,00172,00173,00174,00175,
                00176,00177,00178,00179,00180,00181,00182,00183,00201,00202,
                00203,00204,00205,00206,00207,00208,65535,65535,65535,00213,
                00214,00215,00216,00217,00218,00219,00220,00221,00222,00223,
                00224,00225,00226,00227,00228,00229,00230,00231,00232,00233,
                00234,00235,00236,00237,00238,00239,00240,00241,00242,00243,
                00244,00245,00246,00247,00248,00249,00250,00251,00252,00253,
                00254,00255,00256,00257,00258,00259,65535,65535,65535,65535,
                65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,65535,00260,00261,00262,00263,00264,

                65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,
                65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,
                00328,00329,00330,00331,00332,00333,00334,00335,00336,00337,
                00338,00339,00340,00341,00342,00343,00344,00345,00346,00347,
                00348,00349,00350,00351,00352,00353,00354,00355,00356,00357,
                00358,00359,00360,00361,00362,00363,00364,00365,00366,00367,
                00368,00369,00370,00371,00372,00373,00374,00375,00376,00377,
            };

            ushort val = g3val > arr.Length ? ushort.MaxValue : arr[g3val];
            return val == ushort.MaxValue ? ITEM_UNK : val;
        }

        /// <summary>
        /// Checks if the item can be kept during 3->4 conversion.
        /// </summary>
        /// <param name="item">Generation 3 Item ID.</param>
        /// <returns>True if transferrable, False if not transferrable.</returns>
        public static bool isTransferrable34(ushort item)
        {
            return item != ITEM_UNK && item > 0;
        }

        #region Gen 1 Character Tables
        private static Dictionary<byte, string> RBY2U_U => new Dictionary<byte, string>{
            {0x50, "\0"},
            {0x5D, "TRAINER"},
            {0x7F, " "},
            {0x80, "A"},
            {0x81, "B"},
            {0x82, "C"},
            {0x83, "D"},
            {0x84, "E"},
            {0x85, "F"},
            {0x86, "G"},
            {0x87, "H"},
            {0x88, "I"},
            {0x89, "J"},
            {0x8A, "K"},
            {0x8B, "L"},
            {0x8C, "M"},
            {0x8D, "N"},
            {0x8E, "O"},
            {0x8F, "P"},
            {0x90, "Q"},
            {0x91, "R"},
            {0x92, "S"},
            {0x93, "T"},
            {0x94, "U"},
            {0x95, "V"},
            {0x96, "W"},
            {0x97, "X"},
            {0x98, "Y"},
            {0x99, "Z"},
            {0x9A, "["},
            {0x9B, "\\"},
            {0x9C, "]"},
            {0x9D, "^"},
            {0x9E, "_"},
            {0x9F, "`"},
            {0xA0, "a"},
            {0xA1, "b"},
            {0xA2, "c"},
            {0xA3, "d"},
            {0xA4, "e"},
            {0xA5, "f"},
            {0xA6, "g"},
            {0xA7, "h"},
            {0xA8, "i"},
            {0xA9, "j"},
            {0xAA, "k"},
            {0xAB, "l"},
            {0xAC, "m"},
            {0xAD, "n"},
            {0xAE, "o"},
            {0xAF, "p"},
            {0xB0, "q"},
            {0xB1, "r"},
            {0xB2, "s"},
            {0xB3, "t"},
            {0xB4, "u"},
            {0xB5, "v"},
            {0xB6, "w"},
            {0xB7, "x"},
            {0xB8, "y"},
            {0xB9, "z"},
            {0xE0, "’"},
            {0xE1, "{"}, /* Pk */
            {0xE2, "}"}, /* Mn */
            {0xE3, "-"},
            {0xE6, "?"},
            {0xE7, "!"},
            {0xE8, "."}, // Alias decimal point to .
            {0xEF, "♂"},
            {0xF2, "."},
            {0xF3, "/"},
            {0xF4, ","},
            {0xF5, "♀"},
            {0xF6, "0"},
            {0xF7, "1"},
            {0xF8, "2"},
            {0xF9, "3"},
            {0xFA, "4"},
            {0xFB, "5"},
            {0xFC, "6"},
            {0xFD, "7"},
            {0xFE, "8"},
            {0xFF, "9"}
        };

        private static Dictionary<string, byte> U2RBY_U => new Dictionary<string, byte> {
            {"\0", 0x50},
            {"TRAINER", 0x5D},
            {" ", 0x7F},
            {"A", 0x80},
            {"B", 0x81},
            {"C", 0x82},
            {"D", 0x83},
            {"E", 0x84},
            {"F", 0x85},
            {"G", 0x86},
            {"H", 0x87},
            {"I", 0x88},
            {"J", 0x89},
            {"K", 0x8A},
            {"L", 0x8B},
            {"M", 0x8C},
            {"N", 0x8D},
            {"O", 0x8E},
            {"P", 0x8F},
            {"Q", 0x90},
            {"R", 0x91},
            {"S", 0x92},
            {"T", 0x93},
            {"U", 0x94},
            {"V", 0x95},
            {"W", 0x96},
            {"X", 0x97},
            {"Y", 0x98},
            {"Z", 0x99},
            {"[", 0x9A},
            {"\\", 0x9B},
            {"]", 0x9C},
            {"^", 0x9D},
            {"_", 0x9E},
            {"`", 0x9F},
            {"a", 0xA0},
            {"b", 0xA1},
            {"c", 0xA2},
            {"d", 0xA3},
            {"e", 0xA4},
            {"f", 0xA5},
            {"g", 0xA6},
            {"h", 0xA7},
            {"i", 0xA8},
            {"j", 0xA9},
            {"k", 0xAA},
            {"l", 0xAB},
            {"m", 0xAC},
            {"n", 0xAD},
            {"o", 0xAE},
            {"p", 0xAF},
            {"q", 0xB0},
            {"r", 0xB1},
            {"s", 0xB2},
            {"t", 0xB3},
            {"u", 0xB4},
            {"v", 0xB5},
            {"w", 0xB6},
            {"x", 0xB7},
            {"y", 0xB8},
            {"z", 0xB9},
            {"'", 0xE0}, // Alias ' to ’ for Farfetch'd
            {"’", 0xE0}, 
            {"{", 0xE1}, /* Pk */
            {"}", 0xE2}, /* Mn */
            {"-", 0xE3},
            {"?", 0xE6},
            {"!", 0xE7},
            {"♂", 0xEF},
            {".", 0xF2},
            {"/", 0xF3},
            {",", 0xF4},
            {"♀", 0xF5},
            {"0", 0xF6},
            {"1", 0xF7},
            {"2", 0xF8},
            {"3", 0xF9},
            {"4", 0xFA},
            {"5", 0xFB},
            {"6", 0xFC},
            {"7", 0xFD},
            {"8", 0xFE},
            {"9", 0xFF}
        };
        private static Dictionary<string, byte> U2RBY_J => new Dictionary<string, byte> {
            {"ガ", 0x05},
            {"ギ", 0x06},
            {"グ", 0x07},
            {"ゲ", 0x08},
            {"ゴ", 0x09},
            {"ザ", 0x0A},
            {"ジ", 0x0B},
            {"ズ", 0x0C},
            {"ゼ", 0x0D},
            {"ゾ", 0x0E},
            {"ダ", 0x0F},
            {"ヂ", 0x10},
            {"ヅ", 0x11},
            {"デ", 0x12},
            {"ド", 0x13},
            {"バ", 0x19},
            {"ビ", 0x1A},
            {"ブ", 0x1B},
            {"ボ", 0x1C},
            {"が", 0x26},
            {"ぎ", 0x27},
            {"ぐ", 0x28},
            {"げ", 0x29},
            {"ご", 0x2A},
            {"ざ", 0x2B},
            {"じ", 0x2C},
            {"ず", 0x2D},
            {"ぜ", 0x2E},
            {"ぞ", 0x2F},
            {"だ", 0x30},
            {"ぢ", 0x31},
            {"づ", 0x32},
            {"で", 0x33},
            {"ど", 0x34},
            {"ば", 0x3A},
            {"び", 0x3B},
            {"ぶ", 0x3C},
            {"ベ", 0x3D},
            {"べ", 0x3D},
            {"ぼ", 0x3E},
            {"パ", 0x40},
            {"ピ", 0x41},
            {"プ", 0x42},
            {"ポ", 0x43},
            {"ぱ", 0x44},
            {"ぴ", 0x45},
            {"ぷ", 0x46},
            {"ぺ", 0x47},
            {"ペ", 0x47},
            {"ぽ", 0x48},
            {"\0", 0x50},
            {"トレーナー", 0x5D},
            {"ア", 0x80},
            {"イ", 0x81},
            {"ウ", 0x82},
            {"ェ", 0x83},
            {"エ", 0x83},
            {"オ", 0x84},
            {"ォ", 0x84},
            {"カ", 0x85},
            {"キ", 0x86},
            {"ク", 0x87},
            {"ケ", 0x88},
            {"コ", 0x89},
            {"サ", 0x8A},
            {"シ", 0x8B},
            {"ス", 0x8C},
            {"セ", 0x8D},
            {"ソ", 0x8E},
            {"タ", 0x8F},
            {"チ", 0x90},
            {"ツ", 0x91},
            {"テ", 0x92},
            {"ト", 0x93},
            {"ナ", 0x94},
            {"ニ", 0x95},
            {"ヌ", 0x96},
            {"ネ", 0x97},
            {"ノ", 0x98},
            {"ハ", 0x99},
            {"ヒ", 0x9A},
            {"フ", 0x9B},
            {"ホ", 0x9C},
            {"マ", 0x9D},
            {"ミ", 0x9E},
            {"ム", 0x9F},
            {"メ", 0xA0},
            {"モ", 0xA1},
            {"ヤ", 0xA2},
            {"ユ", 0xA3},
            {"ヨ", 0xA4},
            {"ラ", 0xA5},
            {"ル", 0xA6},
            {"レ", 0xA7},
            {"ロ", 0xA8},
            {"ワ", 0xA9},
            {"ヲ", 0xAA},
            {"ン", 0xAB},
            {"ッ", 0xAC},
            {"ャ", 0xAD},
            {"ュ", 0xAE},
            {"ョ", 0xAF},
            {"ィ", 0xB0},
            {"あ", 0xB1},
            {"い", 0xB2},
            {"う", 0xB3},
            {"え", 0xB4},
            {"お", 0xB5},
            {"か", 0xB6},
            {"き", 0xB7},
            {"く", 0xB8},
            {"け", 0xB9},
            {"こ", 0xBA},
            {"さ", 0xBB},
            {"し", 0xBC},
            {"す", 0xBD},
            {"せ", 0xBE},
            {"そ", 0xBF},
            {"た", 0xC0},
            {"ち", 0xC1},
            {"つ", 0xC2},
            {"て", 0xC3},
            {"と", 0xC4},
            {"な", 0xC5},
            {"に", 0xC6},
            {"ぬ", 0xC7},
            {"ね", 0xC8},
            {"の", 0xC9},
            {"は", 0xCA},
            {"ひ", 0xCB},
            {"ふ", 0xCC},
            {"へ", 0xCD},
            {"ほ", 0xCE},
            {"ま", 0xCF},
            {"み", 0xD0},
            {"む", 0xD1},
            {"め", 0xD2},
            {"も", 0xD3},
            {"や", 0xD4},
            {"ゆ", 0xD5},
            {"よ", 0xD6},
            {"ら", 0xD7},
            {"リ", 0xD8},
            {"り", 0xD8},
            {"る", 0xD9},
            {"れ", 0xDA},
            {"ろ", 0xDB},
            {"わ", 0xDC},
            {"を", 0xDD},
            {"ん", 0xDE},
            {"っ", 0xDF},
            {"ゃ", 0xE0},
            {"ゅ", 0xE1},
            {"ょ", 0xE2},
            {"ー", 0xE3},
            {"ァ", 0xE9},
            {"♂", 0xEF},
            {"♀", 0xF5},
            {"0", 0xF6},
            {"1", 0xF7},
            {"2", 0xF8},
            {"3", 0xF9},
            {"4", 0xFA},
            {"5", 0xFB},
            {"6", 0xFC},
            {"7", 0xFD},
            {"8", 0xFE},
            {"9", 0xFF}
        };

        private static Dictionary<byte, string> RBY2U_J => new Dictionary<byte, string> {
            {0x05, "ガ"},
            {0x06, "ギ"},
            {0x07, "グ"},
            {0x08, "ゲ"},
            {0x09, "ゴ"},
            {0x0A, "ザ"},
            {0x0B, "ジ"},
            {0x0C, "ズ"},
            {0x0D, "ゼ"},
            {0x0E, "ゾ"},
            {0x0F, "ダ"},
            {0x10, "ヂ"},
            {0x11, "ヅ"},
            {0x12, "デ"},
            {0x13, "ド"},
            {0x19, "バ"},
            {0x1A, "ビ"},
            {0x1B, "ブ"},
            {0x1C, "ボ"},
            {0x26, "が"},
            {0x27, "ぎ"},
            {0x28, "ぐ"},
            {0x29, "げ"},
            {0x2A, "ご"},
            {0x2B, "ざ"},
            {0x2C, "じ"},
            {0x2D, "ず"},
            {0x2E, "ぜ"},
            {0x2F, "ぞ"},
            {0x30, "だ"},
            {0x31, "ぢ"},
            {0x32, "づ"},
            {0x33, "で"},
            {0x34, "ど"},
            {0x3A, "ば"},
            {0x3B, "び"},
            {0x3C, "ぶ"},
            {0x3D, "ベ"},
            {0x3E, "ぼ"},
            {0x40, "パ"},
            {0x41, "ピ"},
            {0x42, "プ"},
            {0x43, "ポ"},
            {0x44, "ぱ"},
            {0x45, "ぴ"},
            {0x46, "ぷ"},
            {0x47, "ペ"},
            {0x48, "ぽ"},
            {0x50, "\0"},
            {0x5D, "トレーナー"},
            {0x80, "ア"},
            {0x81, "イ"},
            {0x82, "ウ"},
            {0x83, "ェ"},
            {0x84, "オ"},
            {0x85, "カ"},
            {0x86, "キ"},
            {0x87, "ク"},
            {0x88, "ケ"},
            {0x89, "コ"},
            {0x8A, "サ"},
            {0x8B, "シ"},
            {0x8C, "ス"},
            {0x8D, "セ"},
            {0x8E, "ソ"},
            {0x8F, "タ"},
            {0x90, "チ"},
            {0x91, "ツ"},
            {0x92, "テ"},
            {0x93, "ト"},
            {0x94, "ナ"},
            {0x95, "ニ"},
            {0x96, "ヌ"},
            {0x97, "ネ"},
            {0x98, "ノ"},
            {0x99, "ハ"},
            {0x9A, "ヒ"},
            {0x9B, "フ"},
            {0x9C, "ホ"},
            {0x9D, "マ"},
            {0x9E, "ミ"},
            {0x9F, "ム"},
            {0xA0, "メ"},
            {0xA1, "モ"},
            {0xA2, "ヤ"},
            {0xA3, "ユ"},
            {0xA4, "ヨ"},
            {0xA5, "ラ"},
            {0xA6, "ル"},
            {0xA7, "レ"},
            {0xA8, "ロ"},
            {0xA9, "ワ"},
            {0xAA, "ヲ"},
            {0xAB, "ン"},
            {0xAC, "ッ"},
            {0xAD, "ャ"},
            {0xAE, "ュ"},
            {0xAF, "ョ"},
            {0xB0, "ィ"},
            {0xB1, "あ"},
            {0xB2, "い"},
            {0xB3, "う"},
            {0xB4, "え"},
            {0xB5, "お"},
            {0xB6, "か"},
            {0xB7, "き"},
            {0xB8, "く"},
            {0xB9, "け"},
            {0xBA, "こ"},
            {0xBB, "さ"},
            {0xBC, "し"},
            {0xBD, "す"},
            {0xBE, "せ"},
            {0xBF, "そ"},
            {0xC0, "た"},
            {0xC1, "ち"},
            {0xC2, "つ"},
            {0xC3, "て"},
            {0xC4, "と"},
            {0xC5, "な"},
            {0xC6, "に"},
            {0xC7, "ぬ"},
            {0xC8, "ね"},
            {0xC9, "の"},
            {0xCA, "は"},
            {0xCB, "ひ"},
            {0xCC, "ふ"},
            {0xCD, "へ"},
            {0xCE, "ほ"},
            {0xCF, "ま"},
            {0xD0, "み"},
            {0xD1, "む"},
            {0xD2, "め"},
            {0xD3, "も"},
            {0xD4, "や"},
            {0xD5, "ゆ"},
            {0xD6, "よ"},
            {0xD7, "ら"},
            {0xD8, "リ"},
            {0xD9, "る"},
            {0xDA, "れ"},
            {0xDB, "ろ"},
            {0xDC, "わ"},
            {0xDD, "を"},
            {0xDE, "ん"},
            {0xDF, "っ"},
            {0xE0, "ゃ"},
            {0xE1, "ゅ"},
            {0xE2, "ょ"},
            {0xE3, "ー"},
            {0xE9, "ァ"},
            {0xEF, "♂"},
            {0xF5, "♀"},
            {0xF6, "0"},
            {0xF7, "1"},
            {0xF8, "2"},
            {0xF9, "3"},
            {0xFA, "4"},
            {0xFB, "5"},
            {0xFC, "6"},
            {0xFD, "7"},
            {0xFE, "8"},
            {0xFF, "9"}
        };
        #endregion
        
        /// <summary>
        /// Converts Generation 1 species ID to National Dex ID.
        /// </summary>
        /// <param name="raw_id">Generation 1 species ID.</param>
        /// <returns>National Dex ID.</returns>
        public static int getG1Species(int raw_id)
        {
            int[] table = { 0x00, 0x70, 0x73, 0x20, 0x23, 0x15, 0x64, 0x22, 0x50, 0x02, 0x67, 0x6C, 0x66, 0x58, 0x5E, 0x1D, 0x1F, 0x68, 0x6F, 0x83, 0x3B, 0x97, 0x82, 0x5A, 0x48, 0x5C, 0x7B, 0x78, 0x09, 0x7F, 0x72, 0x00, 0x00, 0x3A, 0x5F, 0x16, 0x10, 0x4F, 0x40, 0x4B, 0x71, 0x43, 0x7A, 0x6A, 0x6B, 0x18, 0x2F, 0x36, 0x60, 0x4C, 0x00, 0x7E, 0x00, 0x7D, 0x52, 0x6D, 0x00, 0x38, 0x56, 0x32, 0x80, 0x00, 0x00, 0x00, 0x53, 0x30, 0x95, 0x00, 0x00, 0x00, 0x54, 0x3C, 0x7C, 0x92, 0x90, 0x91, 0x84, 0x34, 0x62, 0x00, 0x00, 0x00, 0x25, 0x26, 0x19, 0x1A, 0x00, 0x00, 0x93, 0x94, 0x8C, 0x8D, 0x74, 0x75, 0x00, 0x00, 0x1B, 0x1C, 0x8A, 0x8B, 0x27, 0x28, 0x85, 0x88, 0x87, 0x86, 0x42, 0x29, 0x17, 0x2E, 0x3D, 0x3E, 0x0D, 0x0E, 0x0F, 0x00, 0x55, 0x39, 0x33, 0x31, 0x57, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x44, 0x00, 0x37, 0x61, 0x2A, 0x96, 0x8F, 0x81, 0x00, 0x00, 0x59, 0x00, 0x63, 0x5B, 0x00, 0x65, 0x24, 0x6E, 0x35, 0x69, 0x00, 0x5D, 0x3F, 0x41, 0x11, 0x12, 0x79, 0x01, 0x03, 0x49, 0x00, 0x76, 0x77, 0x00, 0x00, 0x00, 0x00, 0x4D, 0x4E, 0x13, 0x14, 0x21, 0x1E, 0x4A, 0x89, 0x8E, 0x00, 0x51, 0x00, 0x00, 0x04, 0x07, 0x05, 0x08, 0x06, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x2C, 0x2D, 0x45, 0x46, 0x47, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            return table[raw_id];
        }

        /// <summary>
        /// Converts a National Dex ID to Generation 1 species ID.
        /// </summary>
        /// <param name="dex_id">National Dex ID.</param>
        /// <returns>Generation 1 species ID.</returns>
        public static int setG1Species(int dex_id)
        {
            int[] table = { 0x00, 0x99, 0x09, 0x9A, 0xB0, 0xB2, 0xB4, 0xB1, 0xB3, 0x1C, 0x7B, 0x7C, 0x7D, 0x70, 0x71, 0x72, 0x24, 0x96, 0x97, 0xA5, 0xA6, 0x05, 0x23, 0x6C, 0x2D, 0x54, 0x55, 0x60, 0x61, 0x0F, 0xA8, 0x10, 0x03, 0xA7, 0x07, 0x04, 0x8E, 0x52, 0x53, 0x64, 0x65, 0x6B, 0x82, 0xB9, 0xBA, 0xBB, 0x6D, 0x2E, 0x41, 0x77, 0x3B, 0x76, 0x4D, 0x90, 0x2F, 0x80, 0x39, 0x75, 0x21, 0x14, 0x47, 0x6E, 0x6F, 0x94, 0x26, 0x95, 0x6A, 0x29, 0x7E, 0xBC, 0xBD, 0xBE, 0x18, 0x9B, 0xA9, 0x27, 0x31, 0xA3, 0xA4, 0x25, 0x08, 0xAD, 0x36, 0x40, 0x46, 0x74, 0x3A, 0x78, 0x0D, 0x88, 0x17, 0x8B, 0x19, 0x93, 0x0E, 0x22, 0x30, 0x81, 0x4E, 0x8A, 0x06, 0x8D, 0x0C, 0x0A, 0x11, 0x91, 0x2B, 0x2C, 0x0B, 0x37, 0x8F, 0x12, 0x01, 0x28, 0x1E, 0x02, 0x5C, 0x5D, 0x9D, 0x9E, 0x1B, 0x98, 0x2A, 0x1A, 0x48, 0x35, 0x33, 0x1D, 0x3C, 0x85, 0x16, 0x13, 0x4C, 0x66, 0x69, 0x68, 0x67, 0xAA, 0x62, 0x63, 0x5A, 0x5B, 0xAB, 0x84, 0x4A, 0x4B, 0x49, 0x58, 0x59, 0x42, 0x83, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            return table[dex_id];
        }

        /// <summary>
        /// Converts Generation 1 encoded data into a string.
        /// </summary>
        /// <param name="strdata">Encoded data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count"></param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string getString1(byte[] strdata, int offset, int count, bool jp)
        {
            Dictionary<byte, string> dict = jp ? RBY2U_J : RBY2U_U;

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                byte val = strdata[offset + i];
                if (!dict.ContainsKey(val)) // Take valid values
                    break;
                string c = dict[val]; // Convert to Unicode
                if (c == "\0") // Stop if Terminator
                    break;
                s.Append(c);
            }
            return SanitizeString(s.ToString());
        }

        /// <summary>
        /// Converts Generation 1 encoded character to string.
        /// </summary>
        /// <param name="key">Encoded character.</param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string getG1Char(byte key, bool jp)
        {
            Dictionary<byte, string> dict = jp ? RBY2U_J : RBY2U_U;
            return dict.ContainsKey(key) ? dict[key] : "";
        }

        /// <summary>
        /// Converts Generation 1 encoded data the same way Bank converts.
        /// </summary>
        /// <param name="strdata">Generation 1 encoded data.</param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string getG1ConvertedString(byte[] strdata, bool jp)
        {
            var us_table = new ushort[] { 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0000, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 0x0058, 0x0059, 0x005A, 0x0028, 0x0029, 0x003A, 0x003B, 0x0028, 0x0029, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x00C4, 0x00D6, 0x00DC, 0x00E4, 0x00F6, 0x00FC, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0050, 0x004D, 0x002D, 0x0020, 0x0020, 0x003F, 0x0021, 0x002D, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0xE08E, 0x0020, 0x0078, 0x002E, 0x002F, 0x002C, 0xE08F, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020 };
            var jp_table = new ushort[] { 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30AC, 0x30AE, 0x30B0, 0x30B2, 0x30B4, 0x30B6, 0x30B8, 0x30BA, 0x30BC, 0x30BE, 0x30C0, 0x30C2, 0x30C5, 0x30C7, 0x30C9, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30D0, 0x30D3, 0x30D6, 0x30DC, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x304C, 0x304E, 0x3050, 0x3052, 0x3054, 0x3056, 0x3058, 0x305A, 0x305C, 0x305E, 0x3060, 0x3062, 0x3065, 0x3067, 0x3069, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3070, 0x3073, 0x3076, 0x30D9, 0x307C, 0x3000, 0x30D1, 0x30D4, 0x30D7, 0x30DD, 0x3071, 0x3074, 0x3077, 0x30DA, 0x307D, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x0000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A2, 0x30A4, 0x30A6, 0x30A8, 0x30AA, 0x30AB, 0x30AD, 0x30AF, 0x30B1, 0x30B3, 0x30B5, 0x30B7, 0x30B9, 0x30BB, 0x30BD, 0x30BF, 0x30C1, 0x30C4, 0x30C6, 0x30C8, 0x30CA, 0x30CB, 0x30CC, 0x30CD, 0x30CE, 0x30CF, 0x30D2, 0x30D5, 0x30DB, 0x30DE, 0x30DF, 0x30E0, 0x30E1, 0x30E2, 0x30E4, 0x30E6, 0x30E8, 0x30E9, 0x30EB, 0x30EC, 0x30ED, 0x30EF, 0x30F2, 0x30F3, 0x30C3, 0x30E3, 0x30E5, 0x30E7, 0x30A3, 0x3042, 0x3044, 0x3046, 0x3048, 0x304A, 0x304B, 0x304D, 0x304F, 0x3051, 0x3053, 0x3055, 0x3057, 0x3059, 0x305B, 0x305D, 0x305F, 0x3061, 0x3064, 0x3066, 0x3068, 0x306A, 0x306B, 0x306C, 0x306D, 0x306E, 0x306F, 0x3072, 0x3075, 0x30D8, 0x307B, 0x307E, 0x307F, 0x3080, 0x3081, 0x3082, 0x3084, 0x3086, 0x3088, 0x3089, 0x30EA, 0x308B, 0x308C, 0x308D, 0x308F, 0x3092, 0x3093, 0x3063, 0x3083, 0x3085, 0x3087, 0x30FC, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A1, 0x30A5, 0x30A7, 0x3000, 0x3000, 0x3000, 0x2642, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A9, 0x2640, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000 };
            var table = jp ? jp_table : us_table;
            return Util.TrimFromZero(new string(strdata.TakeWhile(b => b != 0).Select(b => (char)table[b]).ToArray()));
        }

        /// <summary>
        /// Converts a string to Generation 1 encoded data.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="jp">Data destination is Japanese.</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] setString1(string value, int maxLength, bool jp, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap

            Dictionary<string, byte> dict = jp ? U2RBY_J : U2RBY_U;
            if (dict.ContainsKey(value)) // Handle "[TRAINER]"
                return new[] {dict[value], dict["\0"]};

            List<byte> arr = new List<byte>();
            foreach (char c in value)
            {
                byte val;
                if (!dict.TryGetValue(c.ToString(), out val))
                    break;
                arr.Add(val);
            }
            var term = dict["\0"]; // terminator
            arr.Add(term);
            while (arr.Count < padTo)
                arr.Add((byte)padWith);
            return arr.ToArray();
        }

        /// <summary>Converts Big Endian encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string getBEString3(byte[] data, int offset, int count)
        {
            return Util.TrimFromZero(Encoding.BigEndianUnicode.GetString(data, offset, count));
        }

        /// <summary>Gets the bytes for a BigEndian string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] setBEString3(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = SanitizeString(value)
                .PadRight(value.Length + 1, (char)0) // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.BigEndianUnicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 4 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string getString4(byte[] data, int offset, int count)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i += 2)
            {
                ushort val = BitConverter.ToUInt16(data, offset + i);
                if (val == 0xFFFF) break;
                ushort chr = val2charG4(val);
                if (chr == 0xFFFF) break;
                s.Append((char)chr);
            }
            return s.ToString();
        }

        /// <summary>Gets the bytes for a 4th Generation String</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] setString4(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value) // Replace Special Characters and add Terminator
                .PadRight(value.Length + 1, (char)0xFFFF) // Null Terminator
                .PadRight(padTo, (char)padWith); // Padding

            byte[] strdata = new byte[temp.Length * 2];
            for (int i = 0; i < temp.Length; i++)
            {
                ushort chr = temp[i];
                ushort val = char2valG4(chr);
                BitConverter.GetBytes(val).CopyTo(strdata, i * 2);
            }
            return strdata;
        }

        /// <summary>Converts Generation 5 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string getString5(byte[] data, int offset, int count)
        {
            return SanitizeString(TrimFromFFFF(Encoding.Unicode.GetString(data, offset, count)));
        }

        /// <summary>Gets the bytes for a Generation 5 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] setString5(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value)
                .PadRight(value.Length + 1, (char)0xFFFF) // Null Terminator
                .PadRight(padTo, (char)padWith); // Padding
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 6 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns> 
        public static string getString6(byte[] data, int offset, int count)
        {
            return SanitizeString(Util.TrimFromZero(Encoding.Unicode.GetString(data, offset, count)));
        }

        /// <summary>Gets the bytes for a Generation 6 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns> 
        public static byte[] setString6(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 7 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string getString7(byte[] data, int offset, int count)
        {
            return bin2strG7_zh(SanitizeString(Util.TrimFromZero(Encoding.Unicode.GetString(data, offset, count))));
        }

        /// <summary>Gets the bytes for a Generation 7 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] setString7(string value, int maxLength, int language, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, 12); // Hard cap
            string temp = str2binG7_zh(UnSanitizeString(value), language == 10)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>
        /// Converts bytes to a string according to the input parameters.
        /// </summary>
        /// <param name="data">Encoded data</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="bigendian">Encoding is BigEndian</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string getString(byte[] data, int generation, bool jp, bool bigendian, int count, int offset = 0)
        {
            if (bigendian)
                return generation == 3 ? getBEString3(data, offset, count) : getBEString4(data, offset, count);

            switch (generation)
            {
                case 1:
                case 2: return getString1(data, offset, count, jp);
                case 3: return getString3(data, offset, count, jp);
                case 4: return getString4(data, offset, count);
                case 5: return getString5(data, offset, count);
                case 6: return getString6(data, offset, count);
                default: return getString7(data, offset, count);
            }
        }

        /// <summary>
        /// Gets the bytes for a Generation specific string according to the input parameters.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="bigendian">Encoding is BigEndian</param>
        /// <param name="maxLength"></param>
        /// <param name="language"></param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] setString(string value, int generation, bool jp, bool bigendian, int maxLength, int language = 0, int padTo = 0, ushort padWith = 0)
        {
            if (bigendian)
                return generation == 3 ? setBEString3(value, maxLength, padTo, padWith) : setBEString4(value, maxLength, padTo, padWith);

            switch (generation)
            {
                case 1:
                case 2: return setString1(value, maxLength, jp, padTo, padWith);
                case 3: return setString3(value, maxLength, jp, padTo, padWith);
                case 4: return setString4(value, maxLength, padTo, padWith);
                case 5: return setString5(value, maxLength, padTo, padWith);
                case 6: return setString6(value, maxLength, padTo, padWith);
                default: return setString7(value, maxLength, language, padTo, padWith);
            }
        }

        /// <summary>
        /// Gets the Main Series language ID from a GameCube (C/XD) language ID. Re-maps Spanish 6->7.
        /// </summary>
        /// <param name="value">GameCube (C/XD) language ID.</param>
        /// <returns>Main Series language ID.</returns>
        public static byte getMainLangIDfromGC(byte value) => value == 6 ? (byte)7 : value;

        /// <summary>
        /// Gets the GameCube (C/XD) language ID from a Main Series language ID. Re-maps Spanish 7->6.
        /// </summary>
        /// <param name="value">Main Series language ID.</param>
        /// <returns>GameCube (C/XD) language ID.</returns>
        public static byte getGCLangIDfromMain(byte value) => value == 7 ? (byte)6 : value;

        /// <summary>
        /// Gets an array of valid <see cref="PKM"/> file extensions.
        /// </summary>
        /// <returns>Valid <see cref="PKM"/> file extensions.</returns>
        public static string[] getPKMExtensions(int MaxGeneration = Generation)
        {
            var result = new List<string>();
            result.AddRange(new [] {"ck3", "xk3", "bk4"}); // Special Cases
            for (int i = 1; i <= MaxGeneration; i++)
                result.Add("pk"+i);
            return result.ToArray();
        }

        // Extensions
        public static string getLocation(this PKM pk, bool eggmet)
        {
            if (pk.Format < 2)
                return "";

            int gen = -1;
            int bankID = 0;
            int locval = eggmet ? pk.Egg_Location : pk.Met_Location;

            if (pk.Format == 2)
                gen = 2;
            else if (pk.Format == 3)
                gen = 3;
            else if (pk.Gen4 && (eggmet || pk.Format == 4)) // 4
            {
                const int size = 1000;
                bankID = locval/size;
                gen = 4;
                locval %= size;
            }
            else // 5-7+
            {
                const int size = 10000;
                bankID = locval/size;

                int g = pk.GenNumber;
                if (g >= 5)
                    gen = g;
                else if (pk.Format >= 5)
                    gen = pk.Format;

                locval %= size;
                if (bankID >= 3)
                    locval -= 1;
            }

            var bank = GameInfo.getLocationNames(gen, bankID);
            if (bank == null || bank.Length <= locval)
                return "";
            return bank[locval];
        }
        public static string[] getQRText(this PKM pkm)
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
