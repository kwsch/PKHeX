using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// Common logic for <see cref="PKM"/> data providing and manipulation.
    /// </summary>
    public static class PKX
    {
        internal static readonly PersonalTable Personal = PersonalTable.GG;
        public const int Generation = 7;

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

        private static readonly HashSet<int> Sizes = new HashSet<int>
        {
            SIZE_1JLIST, SIZE_1ULIST,
            SIZE_2ULIST, SIZE_2JLIST,
            SIZE_3STORED, SIZE_3PARTY,
            SIZE_3CSTORED, SIZE_3XSTORED,
            SIZE_4STORED, SIZE_4PARTY,
            SIZE_5PARTY,
            SIZE_6STORED, SIZE_6PARTY
        };

        /// <summary>
        /// Determines if the given length is valid for a <see cref="PKM"/>.
        /// </summary>
        /// <param name="len">Data length of the file/array.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not the length is valid for a <see cref="PKM"/>.</returns>
        public static bool IsPKM(long len) => Sizes.Contains((int)len);

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

        public static readonly Dictionary<string, int>[] SpeciesDict = SpeciesLang.Select(z => z
            .Select((value, index) => new {value, index}).ToDictionary(pair => pair.value, pair => pair.index))
            .ToArray();

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="lang">Language ID of the Pokémon</param>
        /// <returns>The Species name if within expected range, else an empty string.</returns>
        /// <remarks>Should only be used externally for message displays; for accurate in-game names use <see cref="GetSpeciesNameGeneration"/>.</remarks>
        public static string GetSpeciesName(int species, int lang)
        {
            if (lang < 0 || SpeciesLang.Length <= lang)
                return string.Empty;
            if (species < 0 || SpeciesLang[0].Length <= species)
                return string.Empty;

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
            if (generation == 2 && lang == (int)LanguageID.Korean)
                return StringConverter2KOR.LocalizeKOR2(nick);

            if (generation < 5 && (generation != 4 || species != 0)) // All caps GenIV and previous, except GenIV eggs.
            {
                nick = nick.ToUpper();
                if (lang == (int)LanguageID.French)
                    nick = StringConverter4.StripDiacriticsFR4(nick); // strips accents on E and I
            }
            if (generation < 3)
                nick = nick.Replace(" ", string.Empty);
            return nick;
        }

        /// <summary>
        /// Checks if a nickname matches the species name of any language.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>True if it does not match any language name, False if not nicknamed</returns>
        public static bool IsNicknamedAnyLanguage(int species, string nick, int generation = Generation)
        {
            if (species == 083 && string.Equals(nick, "Farfetch'd", StringComparison.OrdinalIgnoreCase)) // stupid ’
                return false;
            var langs = GetAvailableGameLanguages(generation);
            return langs.All(lang => GetSpeciesNameGeneration(species, lang, generation) != nick);
        }

        private static ICollection<int> GetAvailableGameLanguages(int generation = Generation)
        {
            if (generation < 3)
                return Legal.Languages_GB;
            if (generation < 4)
                return Legal.Languages_3;
            if (generation < 7)
                return Legal.Languages_46;
            return Legal.Languages_7;
        }

        /// <summary>
        /// Gets the Species name Language ID for the current name and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <param name="priorlang">Language ID with a higher priority</param>
        /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
        public static int GetSpeciesNameLanguage(int species, string nick, int generation = Generation, int priorlang = -1)
        {
            var langs = GetAvailableGameLanguages(generation);

            if (langs.Contains(priorlang) && GetSpeciesNameGeneration(species, priorlang, generation) == nick)
                return priorlang;

            foreach (var lang in langs)
            {
                if (GetSpeciesNameGeneration(species, lang, generation) == nick)
                    return lang;
            }

            return -1;
        }

        /// <summary>
        /// Gets randomized EVs for a given generation format
        /// </summary>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Array containing randomized EVs (H/A/B/S/C/D)</returns>
        public static int[] GetRandomEVs(int generation = Generation)
        {
            if (generation > 2)
            {
                var evs = new int[6];
                do
                {
                    int max = 510;
                    int randomEV() => (byte)Math.Min(Util.Rand.Next(Math.Min(300, max)), 252);
                    for (int i = 0; i < evs.Length - 1; i++)
                        max -= evs[i] = randomEV();
                    evs[5] = max;
                } while (evs[5] > 252);
                Util.Shuffle(evs);
                return evs;
            }
            else
            {
                var evs = new int[6];
                for (int i = 0; i < evs.Length; i++)
                    evs[i] = Util.Rand.Next(ushort.MaxValue + 1);
                return evs;
            }
        }

        /// <summary>
        /// Gets the current level of a species.
        /// </summary>
        /// <param name="exp">Experience points</param>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <param name="forme">AltForm ID (starters in Let's Go)</param>
        /// <returns>Current level of the species.</returns>
        public static int GetLevel(uint exp, int species, int forme)
        {
            return Experience.GetLevel(exp, species, forme);
        }

        /// <summary>
        /// Gets the minimum Experience points for the specified level.
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <param name="forme">AltForm ID (starters in Let's Go)</param>
        /// <returns>Experience points needed to have specified level.</returns>
        public static uint GetEXP(int level, int species, int forme)
        {
            return Experience.GetEXP(level, species, forme);
        }

        /// <summary>
        /// Translates a Gender string to Gender integer.
        /// </summary>
        /// <param name="s">Gender string</param>
        /// <returns>Gender integer</returns>
        public static int GetGenderFromString(string s)
        {
            if (s.Length != 1)
                return 2;
            switch (s[0])
            {
                case '♂': case 'M': return 0;
                case '♀': case 'F': return 1;
                default: return 2;
            }
        }

        /// <summary>
        /// Gets the nature modification values and checks if they are equal.
        /// </summary>
        /// <param name="nature">Nature</param>
        /// <param name="incr">Increased stat</param>
        /// <param name="decr">Decreased stat</param>
        /// <returns>True if nature modification values are equal or the Nature is out of range.</returns>
        public static bool GetNatureModification(int nature, out int incr, out int decr)
        {
            incr = (nature / 5) + 1;
            decr = (nature % 5) + 1;
            return incr == decr || nature >= 25; // invalid
        }

        /// <summary>
        /// Updates stats according to the specified nature.
        /// </summary>
        /// <param name="stats">Current stats to amplify if appropriate</param>
        /// <param name="nature">Nature</param>
        public static void ModifyStatsForNature(ushort[] stats, int nature)
        {
            if (GetNatureModification(nature, out int incr, out int decr))
                return;
            stats[incr] *= 11; stats[incr] /= 10;
            stats[decr] *= 9; stats[decr] /= 10;
        }

        /// <summary>
        /// Positions for shuffling.
        /// </summary>
        private static readonly byte[] BlockPosition =
        {
            0, 1, 2, 3,
            0, 1, 3, 2,
            0, 2, 1, 3,
            0, 3, 1, 2,
            0, 2, 3, 1,
            0, 3, 2, 1,
            1, 0, 2, 3,
            1, 0, 3, 2,
            2, 0, 1, 3,
            3, 0, 1, 2,
            2, 0, 3, 1,
            3, 0, 2, 1,
            1, 2, 0, 3,
            1, 3, 0, 2,
            2, 1, 0, 3,
            3, 1, 0, 2,
            2, 3, 0, 1,
            3, 2, 0, 1,
            1, 2, 3, 0,
            1, 3, 2, 0,
            2, 1, 3, 0,
            3, 1, 2, 0,
            2, 3, 1, 0,
            3, 2, 1, 0,

            // duplicates of 0-7 to eliminate modulus
            0, 1, 2, 3,
            0, 1, 3, 2,
            0, 2, 1, 3,
            0, 3, 1, 2,
            0, 2, 3, 1,
            0, 3, 2, 1,
            1, 0, 2, 3,
            1, 0, 3, 2,
        };

        /// <summary>
        /// Positions for unshuffling.
        /// </summary>
        internal static readonly byte[] blockPositionInvert =
        {
            0, 1, 2, 4, 3, 5, 6, 7, 12, 18, 13, 19, 8, 10, 14, 20, 16, 22, 9, 11, 15, 21, 17, 23,
            0, 1, 2, 4, 3, 5, 6, 7, // duplicates of 0-7 to eliminate modulus
        };

        /// <summary>
        /// Shuffles a 232 byte array containing <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Data to shuffle</param>
        /// <param name="sv">Block Shuffle order</param>
        /// <param name="blockSize">Size of shuffling chunks</param>
        /// <returns>Shuffled byte array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ShuffleArray(byte[] data, uint sv, int blockSize)
        {
            byte[] sdata = (byte[])data.Clone();
            uint index = sv*4;
            for (int block = 0; block < 4; block++)
            {
                int ofs = BlockPosition[index + block];
                Array.Copy(data, 8 + (blockSize * ofs), sdata, 8 + (blockSize * block), blockSize);
            }
            return sdata;
        }

        /// <summary>
        /// Decrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="ekm">Encrypted <see cref="PKM"/> data.</param>
        /// <returns>Decrypted <see cref="PKM"/> data.</returns>
        /// <returns>Encrypted <see cref="PKM"/> data.</returns>
        public static byte[] DecryptArray(byte[] ekm)
        {
            uint pv = BitConverter.ToUInt32(ekm, 0);
            uint sv = pv >> 13 & 31;

            CryptPKM(ekm, pv, SIZE_6BLOCK);
            return ShuffleArray(ekm, sv, SIZE_6BLOCK);
        }

        /// <summary>
        /// Encrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="pkm">Decrypted <see cref="PKM"/> data.</param>
        public static byte[] EncryptArray(byte[] pkm)
        {
            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint sv = pv >> 13 & 31;

            byte[] ekm = ShuffleArray(pkm, blockPositionInvert[sv], SIZE_6BLOCK);
            CryptPKM(ekm, pv, SIZE_6BLOCK);
            return ekm;
        }

        /// <summary>
        /// Decrypts a 136 byte + party stat byte array.
        /// </summary>
        /// <param name="ekm">Encrypted <see cref="PKM"/> data.</param>
        /// <returns>Decrypted <see cref="PKM"/> data.</returns>
        public static byte[] DecryptArray45(byte[] ekm)
        {
            uint pv = BitConverter.ToUInt32(ekm, 0);
            uint chk = BitConverter.ToUInt16(ekm, 6);
            uint sv = pv >> 13 & 31;

            CryptPKM45(ekm, pv, chk, SIZE_4BLOCK);
            return ShuffleArray(ekm, sv, SIZE_4BLOCK);
        }

        /// <summary>
        /// Encrypts a 136 byte + party stat byte array.
        /// </summary>
        /// <param name="pkm">Decrypted <see cref="PKM"/> data.</param>
        /// <returns>Encrypted <see cref="PKM"/> data.</returns>
        public static byte[] EncryptArray45(byte[] pkm)
        {
            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint chk = BitConverter.ToUInt16(pkm, 6);
            uint sv = pv >> 13 & 31;

            byte[] ekm = ShuffleArray(pkm, blockPositionInvert[sv], SIZE_4BLOCK);
            CryptPKM45(ekm, pv, chk, SIZE_4BLOCK);
            return ekm;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CryptPKM(byte[] data, uint pv, int blockSize)
        {
            const int start = 8;
            int end = (4 * blockSize) + start;
            CryptArray(data, pv, 8, end); // Blocks
            if (data.Length > end)
                CryptArray(data, pv, end, data.Length); // Party Stats
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CryptPKM45(byte[] data, uint pv, uint chk, int blockSize)
        {
            const int start = 8;
            int end = (4 * blockSize) + start;
            CryptArray(data, chk, start, end); // Blocks
            if (data.Length > end)
                CryptArray(data, pv, end, data.Length); // Party Stats
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CryptArray(byte[] data, uint seed, int start, int end)
        {
            int i = start;
            do // all block sizes are multiples of 4
            {
                Crypt(data, ref seed, i); i += 2;
                Crypt(data, ref seed, i); i += 2;
            }
            while (i < end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CryptArray(byte[] data, uint seed) => CryptArray(data, seed, 0, data.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Crypt(byte[] data, ref uint seed, int i)
        {
            seed = (0x41C64E6D * seed) + 0x00006073;
            data[i] ^= (byte)(seed >> 16);
            data[i + 1] ^= (byte)(seed >> 24);
        }

        /// <summary>
        /// Gets the checksum of a 232 byte array.
        /// </summary>
        /// <param name="data">Decrypted <see cref="PKM"/> data.</param>
        /// <returns></returns>
        public static ushort GetCHK(byte[] data)
        {
            ushort chk = 0;
            for (int i = 8; i < SIZE_6STORED; i += 2)
                chk += BitConverter.ToUInt16(data, i);

            return chk;
        }

        /// <summary>
        /// Gets the checksum of a Generation 3 byte array.
        /// </summary>
        /// <param name="data">Decrypted <see cref="PKM"/> data.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort GetCHK3(byte[] data)
        {
            ushort chk = 0;
            for (int i = 0x20; i < SIZE_3STORED; i += 2)
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
                    var pidLetter = GetUnownForm(pid);
                    if (pidLetter != form)
                        continue;
                }
                else if (bits != (pid & 0x00010001)) // keep ability bits
                {
                    continue;
                }

                if (gt == 255 || gt == 254 || gt == 0) // Set Gender(less)
                    return pid; // PID can be anything

                // Gen 3/4/5: Gender derived from PID
                if (cg == GetGenderFromPIDAndRatio(pid, gt))
                    return pid;
            }
        }

        // Data Requests
        public static string GetResourceStringBall(int ball) => $"_ball{ball}";
        private const string ResourceSeparator = "_";
        private const string ResourcePikachuCosplay = "c"; // osplay
        private const string ResourceShiny = "s"; // hiny
        private const string ResourceGGStarter = "p"; //artner
        public static bool AllowShinySprite { get; set; }

        public static string GetResourceStringSprite(int species, int form, int gender, int generation = Generation, bool shiny = false)
        {
            if (Legal.SpeciesDefaultFormSprite.Contains(species)) // Species who show their default sprite regardless of Form
                form = 0;

            var sb = new System.Text.StringBuilder();
            { sb.Append(ResourceSeparator); sb.Append(species); }
            if (form > 0)
            { sb.Append(ResourceSeparator); sb.Append(form); }
            else if (gender == 1 && Legal.SpeciesGenderedSprite.Contains(species)) // Frillish & Jellicent, Unfezant & Pyroar
            { sb.Append(ResourceSeparator); sb.Append(gender); }

            if (species == 25 && form > 0 && generation == 6) // Cosplay Pikachu
                sb.Append(ResourcePikachuCosplay);
            else if (GameVersion.GG.Contains(PKMConverter.Trainer.Game) && (species == 25 || species == 133) && form != 0)
                sb.Append(ResourceGGStarter);

            if (shiny && AllowShinySprite)
                sb.Append(ResourceShiny);
            return sb.ToString();
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
        public static string[] GetFormList(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, int generation = Generation)
        {
            return FormConverter.GetFormList(species, types, forms, genders, generation);
        }

        /// <summary>
        /// Gets the Unown Forme ID from PID.
        /// </summary>
        /// <param name="pid">Personality ID</param>
        /// <remarks>Should only be used for 3rd Generation origin specimens.</remarks>
        /// <returns></returns>
        public static int GetUnownForm(uint pid)
        {
            var val = (pid & 0x3000000) >> 18 | (pid & 0x30000) >> 12 | (pid & 0x300) >> 6 | (pid & 0x3);
            return (int)(val % 28);
        }

        /// <summary>
        /// Gets the gender ID of the species based on the Personality ID.
        /// </summary>
        /// <param name="species">National Dex ID.</param>
        /// <param name="PID">Personality ID.</param>
        /// <returns>Gender ID (0/1/2)</returns>
        /// <remarks>This method should only be used for Generations 3-5 origin.</remarks>
        public static int GetGenderFromPID(int species, uint PID)
        {
            int gt = Personal[species].Gender;
            return GetGenderFromPIDAndRatio(PID, gt);
        }

        public static int GetGenderFromPIDAndRatio(uint PID, int gr)
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
            Debug.Assert(ekm.Length == SIZE_3PARTY || ekm.Length == SIZE_3STORED);

            uint PID = BitConverter.ToUInt32(ekm, 0);
            uint OID = BitConverter.ToUInt32(ekm, 4);
            uint seed = PID ^ OID;

            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i & 3];
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
            byte[] sdata = (byte[])data.Clone();
            uint index = sv * 4;
            for (int block = 0; block < 4; block++)
            {
                int ofs = BlockPosition[index + block];
                Array.Copy(data, 32 + (12 * ofs), sdata, 32 + (12 * block), 12);
            }

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
            Debug.Assert(pkm.Length == SIZE_3PARTY || pkm.Length == SIZE_3STORED);

            uint PID = BitConverter.ToUInt32(pkm, 0);
            uint OID = BitConverter.ToUInt32(pkm, 4);
            uint seed = PID ^ OID;

            byte[] ekm = ShuffleArray3(pkm, blockPositionInvert[PID%24]);
            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i & 3];
            return ekm;
        }

        /// <summary>
        /// Checks if a PKM is encrypted; if encrypted, decrypts the PKM.
        /// </summary>
        /// <remarks>The input PKM object is decrypted; no new object is returned.</remarks>
        /// <param name="pkm">PKM to check encryption for (and decrypt if appropriate).</param>
        /// <param name="format">Format specific check selection</param>
        public static void CheckEncrypted(ref byte[] pkm, int format)
        {
            switch (format)
            {
                case 1:
                case 2: // no encryption
                    return;
                case 3:
                    if (pkm.Length > SIZE_3PARTY) // C/XD
                        return; // no encryption
                    ushort chk = GetCHK3(pkm);
                    if (chk != BitConverter.ToUInt16(pkm, 0x1C))
                        pkm = DecryptArray3(pkm);
                    return;
                case 4:
                case 5:
                    if (BitConverter.ToUInt16(pkm, 4) != 0) // BK4
                        return;
                    if (BitConverter.ToUInt32(pkm, 0x64) != 0)
                        pkm = DecryptArray45(pkm);
                    return;
                case 6:
                case 7:
                    if (BitConverter.ToUInt16(pkm, 0xC8) != 0 && BitConverter.ToUInt16(pkm, 0x58) != 0)
                        pkm = DecryptArray(pkm);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }
        }

        /// <summary>
        /// Gets the Main Series language ID from a GameCube (C/XD) language ID.
        /// </summary>
        /// <param name="value">GameCube (C/XD) language ID.</param>
        /// <returns>Main Series language ID.</returns>
        public static byte GetMainLangIDfromGC(byte value)
        {
            if (value <= 2 || value > 7)
                return value;
            return (byte)GCtoMainSeries[(LanguageGC)value];
        }

        private static readonly Dictionary<LanguageGC, LanguageID> GCtoMainSeries = new Dictionary<LanguageGC, LanguageID>
        {
            {LanguageGC.German, LanguageID.German},
            {LanguageGC.French, LanguageID.French},
            {LanguageGC.Italian, LanguageID.Italian},
            {LanguageGC.Spanish, LanguageID.Spanish},
            {LanguageGC.UNUSED_6, LanguageID.UNUSED_6},
        };

        /// <summary>
        /// Gets the GameCube (C/XD) language ID from a Main Series language ID.
        /// </summary>
        /// <param name="value">Main Series language ID.</param>
        /// <returns>GameCube (C/XD) language ID.</returns>
        public static byte GetGCLangIDfromMain(byte value)
        {
            if (value <= 2 || value > 7)
                return value;
            return (byte)MainSeriesToGC[(LanguageID)value];
        }

        private static readonly Dictionary<LanguageID, LanguageGC> MainSeriesToGC = new Dictionary<LanguageID, LanguageGC>
        {
            {LanguageID.German, LanguageGC.German},
            {LanguageID.French, LanguageGC.French},
            {LanguageID.Italian, LanguageGC.Italian},
            {LanguageID.Spanish, LanguageGC.Spanish},
            {LanguageID.UNUSED_6, LanguageGC.UNUSED_6},
        };

        /// <summary>
        /// Gets an array of valid <see cref="PKM"/> file extensions.
        /// </summary>
        /// <param name="maxGeneration">Maximum Generation to permit</param>
        /// <returns>Valid <see cref="PKM"/> file extensions.</returns>
        public static string[] GetPKMExtensions(int maxGeneration = Generation)
        {
            var result = new List<string>();
            int min = maxGeneration <= 2 || maxGeneration >= 7 ? 1 : 3;
            for (int i = min; i <= maxGeneration; i++)
                result.Add($"pk{i}");

            if (maxGeneration >= 3)
            {
                result.Add("ck3"); // colosseum
                result.Add("xk3"); // xd
            }
            if (maxGeneration >= 4)
                result.Add("bk4"); // battle revolution
            if (maxGeneration >= 7)
                result.Add("pb7"); // let's go

            return result.ToArray();
        }

        /// <summary>
        /// Roughly detects the PKM format from the file's extension.
        /// </summary>
        /// <param name="ext">File extension.</param>
        /// <param name="prefer">Preference if not a valid extension, usually the highest acceptable format.</param>
        /// <returns>Format hint that the file is.</returns>
        public static int GetPKMFormatFromExtension(string ext, int prefer)
        {
            if (string.IsNullOrEmpty(ext))
                return prefer;
            return GetPKMFormatFromExtension(ext[ext.Length - 1], prefer);
        }

        /// <summary>
        /// Roughly detects the PKM format from the file's extension.
        /// </summary>
        /// <param name="last">Last character of the file's extension.</param>
        /// <param name="prefer">Preference if not a valid extension, usually the highest acceptable format.</param>
        /// <returns>Format hint that the file is.</returns>
        public static int GetPKMFormatFromExtension(char last, int prefer)
        {
            if ('1' <= last && last <= '9')
                return last - '0';
            return last == 'x' ? 6 : prefer;
        }

        /// <summary>
        /// Copies a <see cref="PKM"/> list to the destination list, with an option to copy to a starting point.
        /// </summary>
        /// <param name="list">Source list to copy from</param>
        /// <param name="dest">Destination list/array</param>
        /// <param name="skip">Criteria for skipping a slot</param>
        /// <param name="start">Starting point to copy to</param>
        /// <returns>Count of <see cref="PKM"/> copied.</returns>
        public static int CopyTo(this IEnumerable<PKM> list, IList<PKM> dest, Func<PKM, bool> skip, int start = 0)
        {
            int ctr = start;
            int skipped = 0;
            foreach (var z in list)
            {
                // seek forward to next open slot
                int next = FindNextSlot(dest, skip, ctr);
                if (next == -1)
                    break;
                skipped += next - ctr;
                ctr = next;
                dest[ctr++] = z;
            }
            return ctr - start - skipped;
        }

        private static int FindNextSlot(IList<PKM> dest, Func<PKM, bool> skip, int ctr)
        {
            while (true)
            {
                if (ctr >= dest.Count)
                    return -1;
                var exist = dest[ctr];
                if (exist == null || !skip(exist))
                    return ctr;
                ctr++;
            }
        }

        /// <summary>
        /// Copies an <see cref="Enumerable"/> list to the destination list, with an option to copy to a starting point.
        /// </summary>
        /// <typeparam name="T">Typed object to copy</typeparam>
        /// <param name="list">Source list to copy from</param>
        /// <param name="dest">Destination list/array</param>
        /// <param name="start">Starting point to copy to</param>
        /// <returns>Count of <see cref="T"/> copied.</returns>
        public static int CopyTo<T>(this IEnumerable<T> list, IList<T> dest, int start = 0)
        {
            int ctr = start;
            foreach (var z in list)
            {
                if (ctr >= dest.Count)
                    break;
                dest[ctr++] = z;
            }
            return ctr - start;
        }

        /// <summary>
        /// Gets an <see cref="Enumerable"/> list of PKM data from a concatenated byte array binary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len">Length of each PKM byte[]</param>
        /// <param name="start">Starting offset to rip from. If omitted, will iterate from the start of the <see cref="data"/>.</param>
        /// <param name="end">Ending offset to rip to. If omitted, will iterate to the end of the <see cref="data"/>.</param>
        /// <returns>Enumerable list of PKM byte arrays</returns>
        public static IEnumerable<byte[]> GetPKMDataFromConcatenatedBinary(byte[] data, int len, int start = 0, int end = -1)
        {
            if (end < 0)
                end = data.Length;
            // split up data to individual pkm
            for (int i = start; i < end; i += len)
            {
                var pk = new byte[len];
                Buffer.BlockCopy(data, i, pk, 0, len);
                yield return pk;
            }
        }

        /// <summary>
        /// Detects the language of a <see cref="PK1"/> or <see cref="PK2"/> by checking the current Species name against possible names.
        /// </summary>
        /// <param name="pk">PKM to fetch language for</param>
        /// <returns>Language ID best match (<see cref="LanguageID"/>)</returns>
        public static int GetVCLanguage(PKM pk)
        {
            if (pk.Japanese)
                return 1;
            if (pk.Korean)
                return 8;
            int lang = GetSpeciesNameLanguage(pk.Species, pk.Nickname, pk.Format);
            return lang > 0 ? lang : (int)LanguageID.English; // Default to ENG
        }

        internal static bool IsPKMPresentGB(byte[] data, int offset) => data[offset] != 0;
        internal static bool IsPKMPresentGC(byte[] data, int offset) => BitConverter.ToUInt16(data, offset) != 0;
        internal static bool IsPKMPresentGBA(byte[] data, int offset) => (data[offset + 0x13] & 0xFB) == 2; // ignore egg flag, must be FlagHasSpecies.

        internal static bool IsPKMPresent(byte[] data, int offset)
        {
            if (BitConverter.ToUInt32(data, offset) != 0) // PID
                return true;
            ushort species = BitConverter.ToUInt16(data, offset + 8);
            return species != 0;
        }

        /// <summary>
        /// Gets a function that can check a byte array (at an offset) to see if a <see cref="PKM"/> is possibly present.
        /// </summary>
        /// <param name="blank"></param>
        /// <returns></returns>
        public static Func<byte[], int, bool> GetFuncIsPKMPresent(PKM blank)
        {
            if (blank.Format >= 4)
                return IsPKMPresent;
            if (blank.Format <= 2)
                return IsPKMPresentGB;
            if (blank.Data.Length <= SIZE_3PARTY)
                return IsPKMPresentGBA;
            return IsPKMPresentGC;
        }

        /// <summary>
        /// Reorders (in place) the input array of stats to have the Speed value last rather than before the SpA/SpD stats.
        /// </summary>
        /// <param name="value">Input array to reorder</param>
        /// <returns>Same array, reordered.</returns>
        public static int[] ReorderSpeedLast(int[] value)
        {
            var spe = value[3];
            value[3] = value[4];
            value[4] = value[5];
            value[5] = spe;
            return value;
        }
    }
}
