using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic related to Encrypting and Decrypting Pokémon entity data.
    /// </summary>
    public static class PokeCrypto
    {
        internal const int SIZE_1ULIST = 69;
        internal const int SIZE_1JLIST = 59;
        internal const int SIZE_1PARTY = 44;
        internal const int SIZE_1STORED = 33;

        internal const int SIZE_2ULIST = 73;
        internal const int SIZE_2JLIST = 63;
        internal const int SIZE_2PARTY = 48;
        internal const int SIZE_2STORED = 32;
        internal const int SIZE_2STADIUM = 60;

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

        // Gen7 Format is the same size as Gen6.

        internal const int SIZE_8STORED = 8 + (4 * SIZE_8BLOCK); // 0x148
        internal const int SIZE_8PARTY = SIZE_8STORED + 0x10; // 0x158
        internal const int SIZE_8BLOCK = 80; // 0x50

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
        /// Shuffles a 232 byte array containing Pokémon data.
        /// </summary>
        /// <param name="data">Data to shuffle</param>
        /// <param name="sv">Block Shuffle order</param>
        /// <param name="blockSize">Size of shuffling chunks</param>
        /// <returns>Shuffled byte array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ShuffleArray(byte[] data, uint sv, int blockSize)
        {
            byte[] sdata = (byte[])data.Clone();
            uint index = sv * 4;
            const int start = 8;
            for (int block = 0; block < 4; block++)
            {
                int ofs = BlockPosition[index + block];
                Array.Copy(data, start + (blockSize * ofs), sdata, start + (blockSize * block), blockSize);
            }
            return sdata;
        }

        /// <summary>
        /// Decrypts a Gen8 pkm byte array.
        /// </summary>
        /// <param name="ekm">Encrypted Pokémon data.</param>
        /// <returns>Decrypted Pokémon data.</returns>
        /// <returns>Encrypted Pokémon data.</returns>
        public static byte[] DecryptArray8(byte[] ekm)
        {
            uint pv = BitConverter.ToUInt32(ekm, 0);
            uint sv = pv >> 13 & 31;

            CryptPKM(ekm, pv, SIZE_8BLOCK);
            return ShuffleArray(ekm, sv, SIZE_8BLOCK);
        }

        /// <summary>
        /// Encrypts a Gen8 pkm byte array.
        /// </summary>
        /// <param name="pkm">Decrypted Pokémon data.</param>
        public static byte[] EncryptArray8(byte[] pkm)
        {
            uint pv = BitConverter.ToUInt32(pkm, 0);
            uint sv = pv >> 13 & 31;

            byte[] ekm = ShuffleArray(pkm, blockPositionInvert[sv], SIZE_8BLOCK);
            CryptPKM(ekm, pv, SIZE_8BLOCK);
            return ekm;
        }

        /// <summary>
        /// Decrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="ekm">Encrypted Pokémon data.</param>
        /// <returns>Decrypted Pokémon data.</returns>
        /// <returns>Encrypted Pokémon data.</returns>
        public static byte[] DecryptArray6(byte[] ekm)
        {
            uint pv = BitConverter.ToUInt32(ekm, 0);
            uint sv = pv >> 13 & 31;

            CryptPKM(ekm, pv, SIZE_6BLOCK);
            return ShuffleArray(ekm, sv, SIZE_6BLOCK);
        }

        /// <summary>
        /// Encrypts a 232 byte + party stat byte array.
        /// </summary>
        /// <param name="pkm">Decrypted Pokémon data.</param>
        public static byte[] EncryptArray6(byte[] pkm)
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
        /// <param name="ekm">Encrypted Pokémon data.</param>
        /// <returns>Decrypted Pokémon data.</returns>
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
        /// <param name="pkm">Decrypted Pokémon data.</param>
        /// <returns>Encrypted Pokémon data.</returns>
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
            CryptArray(data, pv, start, end); // Blocks
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
        private static void Crypt(byte[] data, ref uint seed, in int i)
        {
            seed = (0x41C64E6D * seed) + 0x00006073;
            data[i] ^= (byte)(seed >> 16);
            data[i + 1] ^= (byte)(seed >> 24);
        }

        /// <summary>
        /// Decrypts an 80 byte format Generation 3 Pokémon byte array.
        /// </summary>
        /// <param name="ekm">Encrypted data.</param>
        /// <returns>Decrypted data.</returns>
        public static byte[] DecryptArray3(byte[] ekm)
        {
            Debug.Assert(ekm.Length is SIZE_3PARTY or SIZE_3STORED);

            uint PID = BitConverter.ToUInt32(ekm, 0);
            uint OID = BitConverter.ToUInt32(ekm, 4);
            uint seed = PID ^ OID;

            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < 80; i++)
                ekm[i] ^= xorkey[i & 3];
            return ShuffleArray3(ekm, PID % 24);
        }

        /// <summary>
        /// Shuffles an 80 byte format Generation 3 Pokémon byte array.
        /// </summary>
        /// <param name="data">Un-shuffled data.</param>
        /// <param name="sv">Block order shuffle value</param>
        /// <returns>Un-shuffled  data.</returns>
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
        /// Encrypts an 80 byte format Generation 3 Pokémon byte array.
        /// </summary>
        /// <param name="pkm">Decrypted data.</param>
        /// <returns>Encrypted data.</returns>
        public static byte[] EncryptArray3(byte[] pkm)
        {
            Debug.Assert(pkm.Length is SIZE_3PARTY or SIZE_3STORED);

            uint PID = BitConverter.ToUInt32(pkm, 0);
            uint OID = BitConverter.ToUInt32(pkm, 4);
            uint seed = PID ^ OID;

            byte[] ekm = ShuffleArray3(pkm, blockPositionInvert[PID % 24]);
            byte[] xorkey = BitConverter.GetBytes(seed);
            for (int i = 32; i < SIZE_3STORED; i++)
                ekm[i] ^= xorkey[i & 3];
            return ekm;
        }

        /// <summary>
        /// Gets the checksum of a 232 byte array.
        /// </summary>
        /// <param name="data">Decrypted Pokémon data.</param>
        /// <param name="partyStart">Offset at which the Stored data ends and the Party data starts.</param>
        public static ushort GetCHK(byte[] data, int partyStart)
        {
            ushort chk = 0;
            for (int i = 8; i < partyStart; i += 2)
                chk += BitConverter.ToUInt16(data, i);
            return chk;
        }

        /// <summary>
        /// Gets the checksum of a Generation 3 byte array.
        /// </summary>
        /// <param name="data">Decrypted Pokémon data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetCHK3(byte[] data)
        {
            ushort chk = 0;
            for (int i = 0x20; i < SIZE_3STORED; i += 2)
                chk += BitConverter.ToUInt16(data, i);
            return chk;
        }

        /// <summary>
        /// Decrypts the input <see cref="pkm"/> data into a new array if it is encrypted, and updates the reference.
        /// </summary>
        /// <remarks>Generation 3 Format encryption check which verifies the checksum</remarks>
        public static void DecryptIfEncrypted3(ref byte[] pkm)
        {
            ushort chk = GetCHK3(pkm);
            if (chk != BitConverter.ToUInt16(pkm, 0x1C))
                pkm = DecryptArray3(pkm);
        }

        /// <summary>
        /// Decrypts the input <see cref="pkm"/> data into a new array if it is encrypted, and updates the reference.
        /// </summary>
        /// <remarks>Generation 4 &amp; 5 Format encryption check which checks for the unused bytes</remarks>
        public static void DecryptIfEncrypted45(ref byte[] pkm)
        {
            if (BitConverter.ToUInt32(pkm, 0x64) != 0)
                pkm = DecryptArray45(pkm);
        }

        /// <summary>
        /// Decrypts the input <see cref="pkm"/> data into a new array if it is encrypted, and updates the reference.
        /// </summary>
        /// <remarks>Generation 6 &amp; 7 Format encryption check</remarks>
        public static void DecryptIfEncrypted67(ref byte[] pkm)
        {
            if (BitConverter.ToUInt16(pkm, 0xC8) != 0 || BitConverter.ToUInt16(pkm, 0x58) != 0)
                pkm = DecryptArray6(pkm);
        }

        /// <summary>
        /// Decrypts the input <see cref="pkm"/> data into a new array if it is encrypted, and updates the reference.
        /// </summary>
        /// <remarks>Generation 8 Format encryption check</remarks>
        public static void DecryptIfEncrypted8(ref byte[] pkm)
        {
            if (BitConverter.ToUInt16(pkm, 0x70) != 0 || BitConverter.ToUInt16(pkm, 0xC0) != 0)
                pkm = DecryptArray8(pkm);
        }
    }
}