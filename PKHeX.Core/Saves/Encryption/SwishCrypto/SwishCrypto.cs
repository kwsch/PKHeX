using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    /// <summary>
    /// MemeCrypto V2 - The Next Generation
    /// </summary>
    /// <remarks>
    /// A variant of <see cref="SaveFile"/> encryption and obfuscation used in <see cref="GameVersion.SWSH"/>.
    /// <br> Individual save blocks are stored in a hash map, with some object-type details prefixing the block's raw data. </br>
    /// <br> Once the raw save file data is dumped, the binary is hashed with SHA256 using a static Intro salt and static Outro salt. </br>
    /// <br> With the hash computed, the data is encrypted with a repeating irregular-sized static xor cipher. </br>
    /// </remarks>
    public static class SwishCrypto
    {
        private const int SIZE_HASH = 0x20;

        private static readonly byte[] IntroHashBytes =
        {
            0x9E, 0xC9, 0x9C, 0xD7, 0x0E, 0xD3, 0x3C, 0x44, 0xFB, 0x93, 0x03, 0xDC, 0xEB, 0x39, 0xB4, 0x2A,
            0x19, 0x47, 0xE9, 0x63, 0x4B, 0xA2, 0x33, 0x44, 0x16, 0xBF, 0x82, 0xA2, 0xBA, 0x63, 0x55, 0xB6,
            0x3D, 0x9D, 0xF2, 0x4B, 0x5F, 0x7B, 0x6A, 0xB2, 0x62, 0x1D, 0xC2, 0x1B, 0x68, 0xE5, 0xC8, 0xB5,
            0x3A, 0x05, 0x90, 0x00, 0xE8, 0xA8, 0x10, 0x3D, 0xE2, 0xEC, 0xF0, 0x0C, 0xB2, 0xED, 0x4F, 0x6D,
        };

        private static readonly byte[] OutroHashBytes =
        {
            0xD6, 0xC0, 0x1C, 0x59, 0x8B, 0xC8, 0xB8, 0xCB, 0x46, 0xE1, 0x53, 0xFC, 0x82, 0x8C, 0x75, 0x75,
            0x13, 0xE0, 0x45, 0xDF, 0x32, 0x69, 0x3C, 0x75, 0xF0, 0x59, 0xF8, 0xD9, 0xA2, 0x5F, 0xB2, 0x17,
            0xE0, 0x80, 0x52, 0xDB, 0xEA, 0x89, 0x73, 0x99, 0x75, 0x79, 0xAF, 0xCB, 0x2E, 0x80, 0x07, 0xE6,
            0xF1, 0x26, 0xE0, 0x03, 0x0A, 0xE6, 0x6F, 0xF6, 0x41, 0xBF, 0x7E, 0x59, 0xC2, 0xAE, 0x55, 0xFD,
        };

        private static readonly byte[] StaticXorpad =
        {
            0xA0, 0x92, 0xD1, 0x06, 0x07, 0xDB, 0x32, 0xA1, 0xAE, 0x01, 0xF5, 0xC5, 0x1E, 0x84, 0x4F, 0xE3,
            0x53, 0xCA, 0x37, 0xF4, 0xA7, 0xB0, 0x4D, 0xA0, 0x18, 0xB7, 0xC2, 0x97, 0xDA, 0x5F, 0x53, 0x2B,
            0x75, 0xFA, 0x48, 0x16, 0xF8, 0xD4, 0x8A, 0x6F, 0x61, 0x05, 0xF4, 0xE2, 0xFD, 0x04, 0xB5, 0xA3,
            0x0F, 0xFC, 0x44, 0x92, 0xCB, 0x32, 0xE6, 0x1B, 0xB9, 0xB1, 0x2E, 0x01, 0xB0, 0x56, 0x53, 0x36,
            0xD2, 0xD1, 0x50, 0x3D, 0xDE, 0x5B, 0x2E, 0x0E, 0x52, 0xFD, 0xDF, 0x2F, 0x7B, 0xCA, 0x63, 0x50,
            0xA4, 0x67, 0x5D, 0x23, 0x17, 0xC0, 0x52, 0xE1, 0xA6, 0x30, 0x7C, 0x2B, 0xB6, 0x70, 0x36, 0x5B,
            0x2A, 0x27, 0x69, 0x33, 0xF5, 0x63, 0x7B, 0x36, 0x3F, 0x26, 0x9B, 0xA3, 0xED, 0x7A, 0x53, 0x00,
            0xA4, 0x48, 0xB3, 0x50, 0x9E, 0x14, 0xA0, 0x52, 0xDE, 0x7E, 0x10, 0x2B, 0x1B, 0x77, 0x6E,
        };

        private static void CryptStaticXorpadBytes(byte[] data)
        {
            var xp = StaticXorpad;
            for (var i = 0; i < data.Length - SIZE_HASH; i++)
                data[i] ^= xp[i % xp.Length];
        }

        private static byte[] ComputeHash(byte[] data)
        {
#if !NET46
            using var h = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            h.AppendData(IntroHashBytes);
            h.AppendData(data, 0, data.Length - SIZE_HASH);
            h.AppendData(OutroHashBytes);
            return h.GetHashAndReset();
#else
            var intro = IntroHashBytes;
            var outro = OutroHashBytes;
            using var stream = new MemoryStream(intro.Length + data.Length - SIZE_HASH + outro.Length);
            stream.Write(intro, 0, intro.Length);
            stream.Write(data, 0, data.Length - SIZE_HASH); // hash is at the end
            stream.Write(outro, 0, outro.Length);
            stream.Seek(0, SeekOrigin.Begin);
            using var sha = SHA256.Create();
            return sha.ComputeHash(stream);
#endif
        }

        /// <summary>
        /// Checks if the file is a rough example of a save file.
        /// </summary>
        /// <param name="data">Encrypted save data</param>
        /// <returns>True if hash matches</returns>
        public static bool GetIsHashValid(byte[] data)
        {
            if (!SaveUtil.SizesSWSH.Contains(data.Length))
                return false;

            var hash = ComputeHash(data);
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != data[data.Length - SIZE_HASH + i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Decrypts the save data.
        /// </summary>
        /// <param name="data">Encrypted save data</param>
        /// <returns>Decrypted blocks.</returns>
        /// <remarks>
        /// Hash is assumed to be valid before calling this method.
        /// </remarks>
        public static IReadOnlyList<SCBlock> Decrypt(byte[] data)
        {
            var temp = GetDecryptedRawData(data);
            return ReadBlocks(temp);
        }

        /// <summary>
        /// Decrypts the save data, with raw block data concatenated together.
        /// </summary>
        public static byte[] GetDecryptedRawData(byte[] data)
        {
            // de-ref from input data, since we're going to modify the contents in-place
            var temp = (byte[])data.Clone();
            CryptStaticXorpadBytes(temp);
            return temp;
        }

        private const int BlockDataRatioEstimate1 = 777; // bytes per block, on average (generous)
        private const int BlockDataRatioEstimate2 = 555; // bytes per block, on average (stingy)

        private static IReadOnlyList<SCBlock> ReadBlocks(byte[] data)
        {
            var result = new List<SCBlock>(data.Length / BlockDataRatioEstimate2);
            int offset = 0;
            while (offset < data.Length - SIZE_HASH)
            {
                var block = SCBlock.ReadFromOffset(data, ref offset);
                result.Add(block);
            }

            return result;
        }

        /// <summary>
        /// Tries to encrypt the save data.
        /// </summary>
        /// <param name="blocks">Decrypted save data</param>
        /// <returns>Encrypted save data.</returns>
        public static byte[] Encrypt(IReadOnlyList<SCBlock> blocks)
        {
            var result = GetDecryptedRawData(blocks);
            CryptStaticXorpadBytes(result);

            var hash = ComputeHash(result);
            hash.CopyTo(result, result.Length - SIZE_HASH);

            return result;
        }

        /// <summary>
        /// Tries to encrypt the save data.
        /// </summary>
        /// <returns>Raw save data without the final xorpad layer.</returns>
        public static byte[] GetDecryptedRawData(IReadOnlyList<SCBlock> blocks)
        {
            using var ms = new MemoryStream(blocks.Count * BlockDataRatioEstimate1);
            using var bw = new BinaryWriter(ms);
            foreach (var block in blocks)
                block.WriteBlock(bw);

            // Allocate hash bytes at the end
            for (int i = 0; i < SIZE_HASH; i++)
                bw.Write((byte)0);

            return ms.ToArray();
        }
    }
}
