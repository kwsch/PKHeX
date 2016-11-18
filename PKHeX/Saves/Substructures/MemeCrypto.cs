using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX
{
    public static class MemeCrypto
    {
        private static byte[] AESECBEncrypt(byte[] key, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var aes = Util.GetAesProvider())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.None;

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(key, new byte[0x10]), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();

                        return ms.ToArray();
                    }
                }
            }
        }

        private static byte[] AESECBDecrypt(byte[] key, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var aes = Util.GetAesProvider())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.None;

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(key, new byte[0x10]), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();

                        return ms.ToArray();
                    }
                }
            }
        }

        private static byte[] MemeCryptoAESEncrypt(byte[] key, byte[] data) // key = SHA1(rsapubkey+hash)
        {
            byte[] temp = new byte[0x10];
            byte[] subkey = new byte[0x10];
            byte[] output = new byte[data.Length];
            for (int i = 0; i < data.Length / 0x10; i++) // Phase 1: CBC Encryption.
            {
                byte[] curblock = new byte[0x10];
                Array.Copy(data, i * 0x10, curblock, 0, 0x10);
                temp = AESECBEncrypt(key, temp.Xor(curblock));
                temp.CopyTo(output, i * 0x10);
            }

            // In between - CMAC stuff
            temp = temp.Xor(output.Take(0x10).ToArray());
            for (int ofs = 0; ofs < 0x10; ofs += 2) // Imperfect ROL implementation
            {
                byte b1 = temp[ofs + 0], b2 = temp[ofs + 1];
                subkey[ofs + 0] = (byte)(2 * b1 + (b2 >> 7));
                subkey[ofs + 1] = (byte)(2 * b2);
                if (ofs + 2 < temp.Length)
                    subkey[ofs + 1] += (byte)(temp[ofs + 2] >> 7);
            }
            if ((temp[0] & 0x80) != 0)
                subkey[0xF] ^= 0x87;

            temp = new byte[0x10]; // Memcpy from an all-zero buffer
            for (int i = 0; i < data.Length / 0x10; i++)
            {
                byte[] curblock = new byte[0x10];
                Array.Copy(output, (data.Length / 0x10 - 1 - i) * 0x10, curblock, 0, 0x10);
                byte[] temp2 = curblock.Xor(subkey);
                Array.Copy(AESECBEncrypt(key, temp2).Xor(temp), 0, output, (data.Length / 0x10 - 1 - i) * 0x10, 0x10);
                temp2.CopyTo(temp, 0);
            }

            return output;
        }

        private static byte[] MemeCryptoAESDecrypt(byte[] key, byte[] data)
        {
            byte[] temp = new byte[0x10];
            byte[] subkey = new byte[0x10];
            byte[] output = new byte[data.Length];
            for (int i = 0; i < data.Length / 0x10; i++) // Reverse Phase 2
            {
                byte[] curblock = new byte[0x10];
                Array.Copy(data, (data.Length / 0x10 - 1 - i) * 0x10, curblock, 0, 0x10);
                temp = AESECBDecrypt(key, temp.Xor(curblock));
                temp.CopyTo(output, (data.Length / 0x10 - 1 - i) * 0x10);
            }

            // At this point we have Phase1(buf) ^ subkey.
            // Subkey is (block first ^ block last) << 1
            // We don't have block first or block last, though?
            // How can we derive subkey?
            // Well, (a ^ a) = 0. so (block first ^ subkey) ^ (block last ^ subkey)
            // = block first ^ block last ;)
            Array.Copy(output, (data.Length / 0x10 - 1) * 0x10, temp, 0, 0x10);
            temp = temp.Xor(output.Take(0x10).ToArray());
            for (int ofs = 0; ofs < 0x10; ofs += 2) // Imperfect ROL implementation
            {
                byte b1 = temp[ofs + 0], b2 = temp[ofs + 1];
                subkey[ofs + 0] = (byte)(2 * b1 + (b2 >> 7));
                subkey[ofs + 1] = (byte)(2 * b2);
                if (ofs + 2 < temp.Length)
                    subkey[ofs + 1] += (byte)(temp[ofs + 2] >> 7);
            }
            if ((temp[0] & 0x80) != 0)
                subkey[0xF] ^= 0x87;

            for (int i = 0; i < data.Length / 0x10; i++)
            {
                byte[] curblock = new byte[0x10];
                Array.Copy(output, 0x10 * i, curblock, 0, 0x10);
                Array.Copy(curblock.Xor(subkey), 0, output, 0x10 * i, 0x10);
            }

            // Now we have Phase1Encrypt(buf).
            temp = new byte[0x10]; // Clear to all zero
            for (int i = 0; i < data.Length / 0x10; i++) // Phase 1: CBC Encryption.
            {
                byte[] curblock = new byte[0x10];
                Array.Copy(output, i * 0x10, curblock, 0, 0x10);
                AESECBDecrypt(key, curblock).Xor(temp).CopyTo(output, i * 0x10);
                curblock.CopyTo(temp, 0);
            }

            return output;
        }

        private static byte[] RSADecrypt(byte[] data)
        {
            byte[] modulus =
            {
                0xb6, 0x1e, 0x19, 0x20, 0x91, 0xf9, 0x0a, 0x8f, 0x76, 0xa6, 0xea, 0xaa, 0x9a, 0x3c, 0xe5, 0x8c, 0x86, 0x3f,
                0x39, 0xae, 0x25, 0x3f, 0x03, 0x78, 0x16, 0xf5, 0x97, 0x58, 0x54, 0xe0, 0x7a, 0x9a, 0x45, 0x66, 0x01,
                0xe7, 0xc9, 0x4c, 0x29, 0x75, 0x9f, 0xe1, 0x55, 0xc0, 0x64, 0xed, 0xdf, 0xa1, 0x11, 0x44, 0x3f, 0x81,
                0xef, 0x1a, 0x42, 0x8c, 0xf6, 0xcd, 0x32, 0xf9, 0xda, 0xc9, 0xd4, 0x8e, 0x94, 0xcf, 0xb3, 0xf6, 0x90,
                0x12, 0x0e, 0x8e, 0x6b, 0x91, 0x11, 0xad, 0xda, 0xf1, 0x1e, 0x7c, 0x96, 0x20, 0x8c, 0x37, 0xc0, 0x14,
                0x3f, 0xf2, 0xbf, 0x3d, 0x7e, 0x83, 0x11, 0x41, 0xa9, 0x73
            };

            byte[] privKey =
            {
                0x77, 0x54, 0x55, 0x66, 0x8f, 0xff, 0x3c, 0xba, 0x30, 0x26, 0xc2, 0xd0, 0xb2, 0x6b, 0x80,
                0x85, 0x89, 0x59, 0x58, 0x34, 0x11, 0x57, 0xae, 0xb0, 0x3b, 0x6b, 0x04, 0x95, 0xee, 0x57, 0x80, 0x3e,
                0x21, 0x86, 0xeb, 0x6c, 0xb2, 0xeb, 0x62, 0xa7, 0x1d, 0xf1, 0x8a, 0x3c, 0x9c, 0x65, 0x79, 0x07, 0x76,
                0x70, 0x96, 0x1b, 0x3a, 0x61, 0x02, 0xda, 0xbe, 0x5a, 0x19, 0x4a, 0xb5, 0x8c, 0x32, 0x50, 0xae, 0xd5,
                0x97, 0xfc, 0x78, 0x97, 0x8a, 0x32, 0x6d, 0xb1, 0xd7, 0xb2, 0x8d, 0xcc, 0xcb, 0x2a, 0x3e, 0x01, 0x4e,
                0xdb, 0xd3, 0x97, 0xad, 0x33, 0xb8, 0xf2, 0x8c, 0xd5, 0x25, 0x05, 0x42, 0x51
            };

            var N = new BigInteger(modulus.Reverse().Concat(new byte[] { 0 }).ToArray());
            var D = new BigInteger(privKey.Reverse().Concat(new byte[] { 0 }).ToArray());
            var M = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());
            byte[] sig = BigInteger.ModPow(M, D, N).ToByteArray().Reverse().ToArray();
            if (sig.Length < 0x60)
                sig = new byte[0x60 - sig.Length].Concat(sig).ToArray();
            else if (sig.Length > 0x60)
                sig = sig.Skip(sig.Length - 0x60).ToArray();
            return sig;
        }

        private static byte[] RSAEncrypt(byte[] data)
        {
            byte[] modulus =
            {
                0xb6, 0x1e, 0x19, 0x20, 0x91, 0xf9, 0x0a, 0x8f, 0x76, 0xa6, 0xea, 0xaa, 0x9a, 0x3c, 0xe5, 0x8c, 0x86, 0x3f,
                0x39, 0xae, 0x25, 0x3f, 0x03, 0x78, 0x16, 0xf5, 0x97, 0x58, 0x54, 0xe0, 0x7a, 0x9a, 0x45, 0x66, 0x01,
                0xe7, 0xc9, 0x4c, 0x29, 0x75, 0x9f, 0xe1, 0x55, 0xc0, 0x64, 0xed, 0xdf, 0xa1, 0x11, 0x44, 0x3f, 0x81,
                0xef, 0x1a, 0x42, 0x8c, 0xf6, 0xcd, 0x32, 0xf9, 0xda, 0xc9, 0xd4, 0x8e, 0x94, 0xcf, 0xb3, 0xf6, 0x90,
                0x12, 0x0e, 0x8e, 0x6b, 0x91, 0x11, 0xad, 0xda, 0xf1, 0x1e, 0x7c, 0x96, 0x20, 0x8c, 0x37, 0xc0, 0x14,
                0x3f, 0xf2, 0xbf, 0x3d, 0x7e, 0x83, 0x11, 0x41, 0xa9, 0x73
            };

            byte[] pubKey = { 1, 0, 1 };

            var N = new BigInteger(modulus.Reverse().Concat(new byte[] { 0 }).ToArray());
            var E = new BigInteger(pubKey.Reverse().Concat(new byte[] { 0 }).ToArray());
            var M = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());
            byte[] sig = BigInteger.ModPow(M, E, N).ToByteArray().Reverse().ToArray();
            if (sig.Length < 0x60)
                sig = new byte[0x60 - sig.Length].Concat(sig).ToArray();
            else if (sig.Length > 0x60)
                sig = sig.Skip(sig.Length - 0x60).ToArray();
            return sig;
        }

        private static byte[] ReverseCrypt(byte[] input)
        {            
            if (input.Length != 0x80)
                throw new ArgumentException("Invalid Memecrypto decryption byte[]!");

            using (var sha1 = Util.GetSHA1Provider())
            {
                byte[] PubKeyDer = "307C300D06092A864886F70D0101010500036B003068026100B61E192091F90A8F76A6EAAA9A3CE58C863F39AE253F037816F5975854E07A9A456601E7C94C29759FE155C064EDDFA111443F81EF1A428CF6CD32F9DAC9D48E94CFB3F690120E8E6B9111ADDAF11E7C96208C37C0143FF2BF3D7E831141A9730203010001".ToByteArray();
                byte[] enc = new byte[0x60];
                Array.Copy(input, 0x20, enc, 0, 0x60);

                byte[] keybuf = new byte[PubKeyDer.Length + 0x20];
                Array.Copy(PubKeyDer, keybuf, PubKeyDer.Length);
                Array.Copy(input, 0, keybuf, PubKeyDer.Length, 0x20);
                byte[] key = sha1.ComputeHash(keybuf).Take(0x10).ToArray();

                byte[] RSA = RSAEncrypt(enc);
                var dec = MemeCryptoAESDecrypt(key, RSA);
                if (sha1.ComputeHash(dec).Take(0x8).SequenceEqual(dec.Skip(0x58)))
                    return dec;
                RSA[0] |= 0x80;
                dec = MemeCryptoAESDecrypt(key, RSA);
                if (sha1.ComputeHash(dec).Take(0x8).SequenceEqual(dec.Skip(0x58)))
                    return dec;
            }            
            return null;
        }

        public static byte[] SignMemeData(byte[] input)
        {
            if (input.Length < 0x60)
                throw new ArgumentException("Bad Meme input!");
            byte[] PubKeyDer = "307C300D06092A864886F70D0101010500036B003068026100B61E192091F90A8F76A6EAAA9A3CE58C863F39AE253F037816F5975854E07A9A456601E7C94C29759FE155C064EDDFA111443F81EF1A428CF6CD32F9DAC9D48E94CFB3F690120E8E6B9111ADDAF11E7C96208C37C0143FF2BF3D7E831141A9730203010001".ToByteArray();
            using (var sha1 = Util.GetSHA1Provider())
            {
                byte[] key = sha1.ComputeHash(PubKeyDer.Concat(input.Take(input.Length - 0x60)).ToArray()).Take(0x10).ToArray();

                byte[] output = (byte[])input.Clone();
                Array.Copy(sha1.ComputeHash(input, 0, input.Length - 8), 0, output, output.Length - 8, 8);
                byte[] MemeCrypted = MemeCryptoAESEncrypt(key, output.Skip(output.Length - 0x60).ToArray());
                MemeCrypted[0] &= 0x7F;
                var RSA = RSADecrypt(MemeCrypted);
                RSA.CopyTo(output, output.Length - 0x60);
                return output;
            }
        }

        /// <summary>
        /// Resigns save data.
        /// </summary>
        /// <param name="sav7">The save data to resign.</param>
        /// <param name="throwIfUnsupported">If true, throw an <see cref="InvalidOperationException"/> if MemeCrypto is unsupported.  If false, calling this function will have no effect.</param>
        /// <exception cref="InvalidOperationException">Thrown if the current platform has FIPS mode enabled on a platform that does not support the required crypto service providers.</exception>
        /// <returns>The resigned save data.</returns>
        public static byte[] Resign(byte[] sav7, bool throwIfUnsupported = true)
        {
            if (sav7 == null || sav7.Length != 0x6BE00)
                return null;            

            try
            {
                byte[] outSav = (byte[])sav7.Clone();

                using (var sha256 = Util.GetSHA256Provider())
                {
                    byte[] CurSig = new byte[0x80];
                    Array.Copy(sav7, 0x6BB00, CurSig, 0, 0x80);

                    byte[] ChecksumTable = new byte[0x140];
                    Array.Copy(sav7, 0x6BC00, ChecksumTable, 0, 0x140);

                    SignMemeData(sha256.ComputeHash(ChecksumTable).Concat((ReverseCrypt(CurSig) ?? new byte[0x60])).ToArray()).CopyTo(outSav, 0x6BB00);
                }
                return outSav;
            }           
            catch (InvalidOperationException)
            {
                if (throwIfUnsupported)
                {
                    throw;
                }
                else
                {
                    return (byte[])sav7.Clone();
                }                
            }            
        }

        public static bool CanUseMemeCrypto()
        {
            try
            {
                Util.GetSHA256Provider();
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }
    }

    public static class StringExtentions
    {
        public static byte[] ToByteArray(this string toTransform)
        {
            return Enumerable
                .Range(0, toTransform.Length / 2)
                .Select(i => Convert.ToByte(toTransform.Substring(i * 2, 2), 16))
                .ToArray();
        }
    }

    public static class ByteArrayExtensions
    {
        public static byte[] Xor(this byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                throw new ArgumentException("Cannot xor two arrays of uneven length!");
            byte[] x = new byte[b1.Length];
            for (int i = 0; i < b1.Length; i++)
                x[i] = (byte)(b1[i] ^ b2[i]);
            return x;
        }

        public static string ToHexString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
    }
}