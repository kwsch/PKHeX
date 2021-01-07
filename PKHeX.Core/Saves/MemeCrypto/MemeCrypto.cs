using System;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    /// <summary>
    /// MemeCrypto V1 - The Original Series
    /// </summary>
    /// <remarks>
    /// A variant of <see cref="SaveFile"/> encryption and obfuscation used in <see cref="GameVersion.Gen7"/>.
    /// <br> The save file stores a dedicated block to contain a hash of the savedata, computed when the block is zeroed. </br>
    /// <br> This signing logic is reused for other authentication; refer to <see cref="MemeKeyIndex"/>. </br>
    /// <br> The save file first computes a SHA256 Hash over the block checksum region.
    /// The logic then applies a SHA1 hash over the SHA256 hash result, encrypts it with a <see cref="MemeKey"/>, and signs it with an RSA private key in a non-straightforward manner. </br>
    /// </remarks>
    public static class MemeCrypto
    {
        private const uint POKE = 0x454B4F50;

        public static bool VerifyMemePOKE(byte[] input, out byte[] output)
        {
            if (input.Length < 0x60)
                throw new ArgumentException("Invalid POKE buffer!");
            var memeLen = input.Length - 8;
            var memeIndex = MemeKeyIndex.PokedexAndSaveFile;
            for (var i = input.Length - 8; i >= 0; i--)
            {
                if (BitConverter.ToUInt32(input, i) != POKE)
                    continue;

                var keyIndex = BitConverter.ToInt32(input, i + 4);
                if (!MemeKey.IsValidPokeKeyIndex(keyIndex))
                    continue;

                memeLen = i;
                memeIndex = (MemeKeyIndex)keyIndex;
                break;
            }

            foreach (var len in new[] { memeLen, memeLen - 2 }) // Account for Pokédex QR Edge case
            {
                if (VerifyMemeData(input, out output, 0, len, memeIndex))
                    return true;

                if (VerifyMemeData(input, out output, 0, len, MemeKeyIndex.PokedexAndSaveFile))
                    return true;
            }

            output = input;
            return false;
        }

        public static bool VerifyMemeData(byte[] input, out byte[] output)
        {
            foreach (MemeKeyIndex keyIndex in Enum.GetValues(typeof(MemeKeyIndex)))
            {
                if (VerifyMemeData(input, out output, keyIndex))
                    return true;
            }
            output = input;
            return false;
        }

        public static bool VerifyMemeData(byte[] input, out byte[] output, MemeKeyIndex keyIndex)
        {
            if (input.Length < 0x60)
            {
                output = input;
                return false;
            }
            var key = new MemeKey(keyIndex);
            output = (byte[])input.Clone();

            var sigBuffer = key.RsaPublic(input.SliceEnd(input.Length - 0x60));
            using var sha1 = SHA1.Create();
            if (DecryptCompare(output, sigBuffer, key, sha1))
                return true;
            sigBuffer[0x0] |= 0x80;
            if (DecryptCompare(output, sigBuffer, key, sha1))
                return true;

            output = input;
            return false;
        }

        private static bool DecryptCompare(byte[] output, byte[] sigBuffer, MemeKey key, SHA1 sha1)
        {
            sigBuffer.CopyTo(output, output.Length - 0x60);
            key.AesDecrypt(output).CopyTo(output, 0);
            // Check for 8-byte equality.
            var hash = sha1.ComputeHash(output, 0, output.Length - 0x8);
            var computed = BitConverter.ToUInt64(hash, 0);
            var existing = BitConverter.ToUInt64(output, output.Length - 0x8);
            return computed == existing;
        }

        public static bool VerifyMemeData(byte[] input, out byte[] output, int offset, int length)
        {
            var data = new byte[length];
            Array.Copy(input, offset, data, 0, length);
            if (VerifyMemeData(data, out output))
            {
                var newOutput = (byte[])input.Clone();
                output.CopyTo(newOutput, offset);
                output = newOutput;
                return true;
            }
            output = input;
            return false;
        }

        public static bool VerifyMemeData(byte[] input, out byte[] output, int offset, int length, MemeKeyIndex keyIndex)
        {
            var data = new byte[length];
            Array.Copy(input, offset, data, 0, length);
            if (VerifyMemeData(data, out output, keyIndex))
            {
                var newOutput = (byte[])input.Clone();
                output.CopyTo(newOutput, offset);
                output = newOutput;
                return true;
            }
            output = input;
            return false;
        }

        public static byte[] SignMemeData(byte[] input, MemeKeyIndex keyIndex = MemeKeyIndex.PokedexAndSaveFile)
        {
            // Validate Input
            if (input.Length < 0x60)
                throw new ArgumentException("Cannot memesign a buffer less than 0x60 bytes in size!");
            var key = new MemeKey(keyIndex);
            if (!key.CanResign)
                throw new ArgumentException("Cannot sign with the specified memekey!");

            var output = (byte[])input.Clone();

            // Copy in the SHA1 signature
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(input, 0, input.Length - 8);
                Array.Copy(hash, 0, output, output.Length - 8, 8);
            }

            // Perform AES operations
            output = key.AesEncrypt(output);
            var sigBuffer = new byte[0x60];
            Array.Copy(output, output.Length - 0x60, sigBuffer, 0, 0x60);
            sigBuffer[0] &= 0x7F;
            sigBuffer = key.RsaPrivate(sigBuffer);
            sigBuffer.CopyTo(output, output.Length - 0x60);
            return output;
        }

        /// <summary>
        /// Resigns save data.
        /// </summary>
        /// <param name="sav7">Save file data to resign</param>
        /// <returns>The resigned save data. Invalid input returns null.</returns>
        public static byte[] Resign7(byte[] sav7)
        {
            if (sav7.Length is not (SaveUtil.SIZE_G7SM or SaveUtil.SIZE_G7USUM))
                throw new ArgumentException("Should not be using this for unsupported saves.");

            // Save Chunks are 0x200 bytes each; Memecrypto signature is 0x100 bytes into the 2nd to last chunk.
            var isUSUM = sav7.Length == SaveUtil.SIZE_G7USUM;
            var ChecksumTableOffset = sav7.Length - 0x200;
            var MemeCryptoOffset = isUSUM ? 0x6C100 : 0x6BB00;
            var ChecksumSignatureLength = isUSUM ? 0x150 : 0x140;
            const int MemeCryptoSignatureLength = 0x80;

            var result = (byte[])sav7.Clone();

            // Store current signature
            var oldSig = sav7.Slice(MemeCryptoOffset, MemeCryptoSignatureLength);

            using var sha256 = SHA256.Create();
            var newSig = sha256.ComputeHash(sav7, ChecksumTableOffset, ChecksumSignatureLength);
            Array.Resize(ref newSig, MemeCryptoSignatureLength);

            if (VerifyMemeData(oldSig, out var memeSig, MemeKeyIndex.PokedexAndSaveFile))
                Buffer.BlockCopy(memeSig, 0x20, newSig, 0x20, 0x60);

            SignMemeData(newSig).CopyTo(result, MemeCryptoOffset);
            return result;
        }
    }
}
