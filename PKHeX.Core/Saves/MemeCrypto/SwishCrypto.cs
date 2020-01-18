using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    /// <summary>
    /// MemeCrypto V2 - The Next Generation
    /// </summary>
    /// <remarks>
    /// A new variant of <see cref="SaveFile"/> encryption and obfuscation, used in <see cref="GameVersion.SWSH"/>.
    /// </remarks>
    public static class SwishCrypto
    {
        private static readonly object _lock = new object();
        private static readonly SHA256 sha256 = new SHA256CryptoServiceProvider();
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
            for (var i = 0; i < data.Length - SIZE_HASH; i++)
                data[i] ^= StaticXorpad[i % StaticXorpad.Length];
        }

        private static byte[] ComputeHash(byte[] data)
        {
            // can't use IncrementalHash.CreateHash(HashAlgorithmName.SHA256); cuz net46 doesn't support
            using var stream = new MemoryStream();
            stream.Write(IntroHashBytes, 0, IntroHashBytes.Length);
            stream.Write(data, 0, data.Length - SIZE_HASH); // hash is at the end
            stream.Write(OutroHashBytes, 0, OutroHashBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            lock (_lock)
            {
                return sha256.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Checks if the file is a rough example of a save file.
        /// </summary>
        /// <param name="data">Encrypted save data</param>
        /// <returns>True if hash matches</returns>
        public static bool GetIsHashValid(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G8SWSH && data.Length != SaveUtil.SIZE_G8SWSH_1)
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

        private static IReadOnlyList<SCBlock> ReadBlocks(byte[] data)
        {
            var result = new List<SCBlock>();
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
        public static byte[] GetDecryptedRawData(IEnumerable<SCBlock> blocks)
        {
            using var ms = new MemoryStream();
            foreach (var block in blocks)
            {
                var enc_data = block.GetEncryptedData();
                ms.Write(enc_data, 0, enc_data.Length);
            }

            // Allocate hash bytes at the end
            var result = new byte[ms.Length + SIZE_HASH];
            ms.ToArray().CopyTo(result, 0);
            return result;
        }
    }

    /// <summary>
    /// Block of <see cref="Data"/> obtained from a <see cref="SwishCrypto"/> encrypted block storage binary.
    /// </summary>
    public sealed class SCBlock : BlockInfo
    {
        /// <summary>
        /// Used to encrypt the rest of the block.
        /// </summary>
        public readonly uint Key;

        /// <summary>
        /// What kind of block is it?
        /// </summary>
        public SCTypeCode Type { get; set; }

        /// <summary>
        /// For <see cref="SCTypeCode.Array"/>: What kind of array is it?
        /// </summary>
        public SCTypeCode SubType { get; set; }

        /// <summary>
        /// Decrypted data for this block.
        /// </summary>
        public byte[] Data = Array.Empty<byte>();

        internal SCBlock(uint key) => Key = key;
        protected override bool ChecksumValid(byte[] data) => true;
        protected override void SetChecksum(byte[] data) { }

        public bool HasValue() => Type > SCTypeCode.Array;
        public object GetValue() => Type.GetValue(Data);
        public void SetValue(object value) => Type.SetValue(Data, value);

        public SCBlock Clone()
        {
            var block = (SCBlock)MemberwiseClone();
            block.Data = (byte[])Data.Clone();
            return block;
        }

        private static void XorshiftAdvance(ref uint key)
        {
            key ^= (key << 2);
            key ^= (key >> 15);
            key ^= (key << 13);
        }

        private static uint PopCount(ulong key)
        {
            // https://en.wikipedia.org/wiki/Hamming_weight#Efficient_implementation
            const ulong m1 = 0x5555555555555555;
            const ulong m2 = 0x3333333333333333;
            const ulong m4 = 0x0f0f0f0f0f0f0f0f;
            // const ulong m8 = 0x00ff00ff00ff00ff;
            // const ulong m16 = 0x0000ffff0000ffff;
            // const ulong m32 = 0x00000000ffffffff;
            const ulong h01 = 0x0101010101010101;
            key -= (key >> 1) & m1;
            key = (key & m2) + ((key >> 2) & m2);
            key = (key + (key >> 4)) & m4;
            return (uint)((key * h01) >> 56);
        }

        private byte[] GetKeyStream(int start, int size)
        {
            // Initialize the xorshift rng.
            var key = Key;
            var pop_count = PopCount(Key);
            for (var i = 0; i < pop_count; i++)
                XorshiftAdvance(ref key);

            int ofs = 0;
            int out_ofs = 0;
            while (ofs + 4 < start)
            {
                // Discard keystream until we're at offset.
                XorshiftAdvance(ref key);
                ofs += 4;
            }

            var result = new byte[size];
            // If we aren't aligned, handle that.
            if (ofs < start)
            {
                int cur_size = Math.Min(size, 4 - (start - ofs));
                Array.Copy(BitConverter.GetBytes(key), start - ofs, result, out_ofs, cur_size);
                out_ofs += cur_size;
                XorshiftAdvance(ref key);
            }

            // Generate keystream until we're done.
            while (out_ofs < size)
            {
                int cur_size = Math.Min(size - out_ofs, 4);
                Array.Copy(BitConverter.GetBytes(key), 0, result, out_ofs, cur_size);
                out_ofs += cur_size;
                XorshiftAdvance(ref key);
            }

            return result;
        }

        private byte[] CryptBytes(byte[] data, int input_offset, int start, int size)
        {
            var result = new byte[size];
            Array.Copy(data, input_offset + start, result, 0, result.Length);

            var key_stream = GetKeyStream(start, size);
            for (var i = 0; i < result.Length; i++)
                result[i] ^= key_stream[i];
            return result;
        }

        private int GetEncryptedDataSize()
        {
            const int size = 4 + 1; // key + type
            switch (Type)
            {
                case SCTypeCode.Bool1:
                case SCTypeCode.Bool2:
                case SCTypeCode.Bool3:
                    return size;
                case SCTypeCode.Object:
                    return size + 4 + Data.Length;
                case SCTypeCode.Array:
                    return size + 5 + Data.Length;
                case SCTypeCode.Byte:
                case SCTypeCode.UInt16:
                case SCTypeCode.UInt32:
                case SCTypeCode.UInt64:
                case SCTypeCode.SByte:
                case SCTypeCode.Int16:
                case SCTypeCode.Int32:
                case SCTypeCode.Int64:
                case SCTypeCode.Single:
                case SCTypeCode.Double:
                    return size + Data.Length;
                default:
                    throw new ArgumentException(nameof(Type));
            }
        }

        /// <summary>
        /// Encrypts the <see cref="Data"/> according to the <see cref="Type"/> and <see cref="SubType"/>.
        /// </summary>
        /// <returns>Encrypted data.</returns>
        public byte[] GetEncryptedData()
        {
            var result = new byte[GetEncryptedDataSize()];
            BitConverter.GetBytes(Key).CopyTo(result, 0);
            result[4] = (byte)Type;
            var out_ofs = 5;

            if (Type == SCTypeCode.Object)
            {
                BitConverter.GetBytes(Data.Length).CopyTo(result, out_ofs);
                out_ofs += 4;
            }
            else if (Type == SCTypeCode.Array)
            {
                var size = SubType.GetTypeSize();
                BitConverter.GetBytes(Data.Length / size).CopyTo(result, out_ofs);
                result[out_ofs + 4] = (byte)SubType;
                out_ofs += 5;
            }

            Data.CopyTo(result, out_ofs);
            CryptBytes(result, 4, 0, result.Length - 4).CopyTo(result, 4);

            return result;
        }

        /// <summary>
        /// Reads a new <see cref="SCBlock"/> object from the <see cref="data"/>, determining the <see cref="Type"/> and <see cref="SubType"/> during read.
        /// </summary>
        /// <param name="data">Decrypted data</param>
        /// <param name="offset">Offset the block is to be read from (modified to offset by the amount of bytes consumed).</param>
        /// <returns>New object containing all info for the block.</returns>
        public static SCBlock ReadFromOffset(byte[] data, ref int offset)
        {
            // Create block, parse its key.
            var key = BitConverter.ToUInt32(data, offset);
            offset += 4;
            var block = new SCBlock(key);

            // Parse the block's type
            block.Type = (SCTypeCode)block.CryptBytes(data, offset, 0, 1)[0];

            switch (block.Type)
            {
                case SCTypeCode.Bool1:
                case SCTypeCode.Bool2:
                case SCTypeCode.Bool3:
                    // Block types are empty, and have no extra data.
                    offset++;
                    break;

                case SCTypeCode.Object:
                    var num_bytes = BitConverter.ToInt32(block.CryptBytes(data, offset, 1, 4), 0);
                    block.Data = block.CryptBytes(data, offset, 5, num_bytes);
                    offset += 5 + num_bytes;
                    break;

                case SCTypeCode.Array:
                    var num_entries = BitConverter.ToInt32(block.CryptBytes(data, offset, 1, 4), 0);
                    block.SubType = (SCTypeCode)block.CryptBytes(data, offset, 5, 1)[0];
                    switch (block.SubType)
                    {
                        case SCTypeCode.Bool3:
                            // This is an array of booleans.
                            block.Data = block.CryptBytes(data, offset, 6, num_entries);
                            offset += 6 + num_entries;
                            Debug.Assert(block.Data.All(entry => entry <= 1));
                            break;

                        case SCTypeCode.Byte:
                        case SCTypeCode.UInt16:
                        case SCTypeCode.UInt32:
                        case SCTypeCode.UInt64:
                        case SCTypeCode.SByte:
                        case SCTypeCode.Int16:
                        case SCTypeCode.Int32:
                        case SCTypeCode.Int64:
                        case SCTypeCode.Single:
                        case SCTypeCode.Double:
                            var entry_size = block.SubType.GetTypeSize();
                            block.Data = block.CryptBytes(data, offset, 6, num_entries * entry_size);
                            offset += 6 + (num_entries * entry_size);
                            break;

                        default:
                            throw new ArgumentException(nameof(block.SubType));
                    }
                    break;

                case SCTypeCode.Byte:
                case SCTypeCode.UInt16:
                case SCTypeCode.UInt32:
                case SCTypeCode.UInt64:
                case SCTypeCode.SByte:
                case SCTypeCode.Int16:
                case SCTypeCode.Int32:
                case SCTypeCode.Int64:
                case SCTypeCode.Single:
                case SCTypeCode.Double:
                    {
                        var entry_size = block.Type.GetTypeSize();
                        block.Data = block.CryptBytes(data, offset, 1, entry_size);
                        offset += 1 + entry_size;
                        break;
                    }
                default:
                    throw new ArgumentException(nameof(block.Type));
            }

            return block;
        }
    }

    /// <summary>
    /// Block type for a <see cref="SCBlock"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "NOT FLAGS")]
    public enum SCTypeCode : byte
    {
        None = 0,

        Bool1 = 1, // False?
        Bool2 = 2, // True?
        Bool3 = 3, // Either? (Array boolean type)

        Object = 4,

        Array = 5,

        Byte = 8,
        UInt16 = 9,
        UInt32 = 10,
        UInt64 = 11,
        SByte = 12,
        Int16 = 13,
        Int32 = 14,
        Int64 = 15,
        Single = 16,
        Double = 17,
    }

    public static class SCTypeCodeExtensions
    {
        public static bool IsBoolean(this SCTypeCode type) => (byte)type - 1 < 3;

        public static int GetTypeSize(this SCTypeCode type)
        {
            return type switch
            {
                SCTypeCode.Bool3 => sizeof(bool),

                SCTypeCode.Byte => sizeof(byte),
                SCTypeCode.UInt16 => sizeof(ushort),
                SCTypeCode.UInt32 => sizeof(uint),
                SCTypeCode.UInt64 => sizeof(ulong),

                SCTypeCode.SByte => sizeof(sbyte),
                SCTypeCode.Int16 => sizeof(short),
                SCTypeCode.Int32 => sizeof(int),
                SCTypeCode.Int64 => sizeof(long),

                SCTypeCode.Single => sizeof(float),
                SCTypeCode.Double => sizeof(double),

                _ => throw new ArgumentException(nameof(type))
            };
        }

        public static Type GetType(this SCTypeCode type)
        {
            return type switch
            {
                SCTypeCode.Byte => typeof(byte),
                SCTypeCode.UInt16 => typeof(ushort),
                SCTypeCode.UInt32 => typeof(uint),
                SCTypeCode.UInt64 => typeof(ulong),

                SCTypeCode.SByte => typeof(sbyte),
                SCTypeCode.Int16 => typeof(short),
                SCTypeCode.Int32 => typeof(int),
                SCTypeCode.Int64 => typeof(long),

                SCTypeCode.Single => typeof(float),
                SCTypeCode.Double => typeof(double),

                _ => throw new ArgumentException(nameof(type)),
            };
        }

        public static object GetValue(this SCTypeCode type, byte[] data)
        {
            return type switch
            {
                SCTypeCode.Byte => data[0],
                SCTypeCode.UInt16 => BitConverter.ToUInt16(data, 0),
                SCTypeCode.UInt32 => BitConverter.ToUInt32(data, 0),
                SCTypeCode.UInt64 => BitConverter.ToUInt64(data, 0),

                SCTypeCode.SByte => (sbyte)data[0],
                SCTypeCode.Int16 => BitConverter.ToInt16(data, 0),
                SCTypeCode.Int32 => BitConverter.ToInt32(data, 0),
                SCTypeCode.Int64 => BitConverter.ToInt64(data, 0),

                SCTypeCode.Single => BitConverter.ToSingle(data, 0),
                SCTypeCode.Double => BitConverter.ToDouble(data, 0),

                _ => throw new ArgumentException(nameof(type)),
            };
        }

        public static void SetValue(this SCTypeCode type, byte[] data, object value)
        {
            switch (type)
            {
                case SCTypeCode.Byte: data[0] = (byte)value; break;
                case SCTypeCode.UInt16: BitConverter.GetBytes((ushort)value).CopyTo(data, 0); break;
                case SCTypeCode.UInt32: BitConverter.GetBytes((uint)value).CopyTo(data, 0); break;
                case SCTypeCode.UInt64: BitConverter.GetBytes((ulong)value).CopyTo(data, 0); break;

                case SCTypeCode.SByte: data[0] = (byte)value; break;
                case SCTypeCode.Int16: BitConverter.GetBytes((short)value).CopyTo(data, 0); break;
                case SCTypeCode.Int32: BitConverter.GetBytes((int)value).CopyTo(data, 0); break;
                case SCTypeCode.Int64: BitConverter.GetBytes((long)value).CopyTo(data, 0); break;

                case SCTypeCode.Single: BitConverter.GetBytes((float)value).CopyTo(data, 0); break;
                case SCTypeCode.Double: BitConverter.GetBytes((double)value).CopyTo(data, 0); break;

                default: throw new ArgumentException(nameof(type));
            }
        }
    }
}
