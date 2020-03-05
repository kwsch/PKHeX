using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Common logic for <see cref="PKM"/> data providing and manipulation.
    /// </summary>
    public static class PKX
    {
        internal static readonly PersonalTable Personal = PersonalTable.SWSH;
        public const int Generation = 8;

        private static readonly HashSet<int> Sizes = new HashSet<int>
        {
            PokeCrypto.SIZE_1JLIST,   PokeCrypto.SIZE_1ULIST,
            PokeCrypto.SIZE_2ULIST,   PokeCrypto.SIZE_2JLIST,
            PokeCrypto.SIZE_3STORED,  PokeCrypto.SIZE_3PARTY,
            PokeCrypto.SIZE_3CSTORED, PokeCrypto.SIZE_3XSTORED,
            PokeCrypto.SIZE_4STORED,  PokeCrypto.SIZE_4PARTY,
            PokeCrypto.SIZE_5PARTY,
            PokeCrypto.SIZE_6STORED,  PokeCrypto.SIZE_6PARTY,
            PokeCrypto.SIZE_8STORED,  PokeCrypto.SIZE_8PARTY,
        };

        /// <summary>
        /// Determines if the given length is valid for a <see cref="PKM"/>.
        /// </summary>
        /// <param name="len">Data length of the file/array.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not the length is valid for a <see cref="PKM"/>.</returns>
        public static bool IsPKM(long len) => Sizes.Contains((int)len);

        /// <summary>
        /// Gets randomized EVs for a given generation format
        /// </summary>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Array containing randomized EVs (H/A/B/S/C/D)</returns>
        public static int[] GetRandomEVs(int generation = Generation)
        {
            var rnd = Util.Rand;
            if (generation > 2)
            {
                var evs = new int[6];
                do
                {
                    int max = 510;
                    for (int i = 0; i < evs.Length - 1; i++)
                        max -= evs[i] = (byte)Math.Min(rnd.Next(Math.Min(300, max)), 252);
                    evs[5] = max;
                } while (evs[5] > 252);
                Util.Shuffle(evs);
                return evs;
            }
            else
            {
                var evs = new int[6];
                for (int i = 0; i < evs.Length; i++)
                    evs[i] = rnd.Next(ushort.MaxValue + 1);
                return evs;
            }
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
        /// Gets a random PID according to specifications.
        /// </summary>
        /// <param name="rnd">RNG to use</param>
        /// <param name="species">National Dex ID</param>
        /// <param name="cg">Current Gender</param>
        /// <param name="origin">Origin Generation</param>
        /// <param name="nature">Nature</param>
        /// <param name="form">AltForm</param>
        /// <param name="OLDPID">Current PID</param>
        /// <remarks>Used to retain ability bits.</remarks>
        /// <returns>Rerolled PID.</returns>
        public static uint GetRandomPID(Random rnd, int species, int cg, int origin, int nature, int form, uint OLDPID)
        {
            uint bits = OLDPID & 0x00010001;
            int gt = Personal[species].Gender;
            if (origin >= 24)
                return Util.Rand32(rnd);

            bool g3unown = origin <= 5 && species == (int)Species.Unown;
            while (true) // Loop until we find a suitable PID
            {
                uint pid = Util.Rand32(rnd);

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
            return gr switch
            {
                255 => 2,
                254 => 1,
                0 => 0,
                _ => ((PID & 0xFF) < gr ? 1 : 0)
            };
        }

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
        /// <returns>Function that checks if a byte array (at an offset) has a <see cref="PKM"/> present</returns>
        public static Func<byte[], int, bool> GetFuncIsPKMPresent(PKM blank)
        {
            if (blank.Format >= 4)
                return IsPKMPresent;
            if (blank.Format <= 2)
                return IsPKMPresentGB;
            if (blank.Data.Length <= PokeCrypto.SIZE_3PARTY)
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
