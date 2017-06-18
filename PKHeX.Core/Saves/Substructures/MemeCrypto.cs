using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX.Core
{
    public static class MemeCrypto
    {
        private static byte[] AESECBEncrypt(byte[] key, byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var aes = Util.GetAesProvider())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.None;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(key, new byte[0x10]), CryptoStreamMode.Write))
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
            using (var ms = new MemoryStream())
            {
                using (var aes = Util.GetAesProvider())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.None;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(key, new byte[0x10]), CryptoStreamMode.Write))
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
            var temp = new byte[0x10];
            var subkey = new byte[0x10];
            var output = new byte[data.Length];
            for (var i = 0; i < data.Length / 0x10; i++) // Phase 1: CBC Encryption.
            {
                var curblock = new byte[0x10];
                Array.Copy(data, i * 0x10, curblock, 0, 0x10);
                temp = AESECBEncrypt(key, temp.Xor(curblock));
                temp.CopyTo(output, i * 0x10);
            }

            // In between - CMAC stuff
            temp = temp.Xor(output.Take(0x10).ToArray());
            for (var ofs = 0; ofs < 0x10; ofs += 2) // Imperfect ROL implementation
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
            for (var i = 0; i < data.Length / 0x10; i++)
            {
                var curblock = new byte[0x10];
                Array.Copy(output, (data.Length / 0x10 - 1 - i) * 0x10, curblock, 0, 0x10);
                byte[] temp2 = curblock.Xor(subkey);
                Array.Copy(AESECBEncrypt(key, temp2).Xor(temp), 0, output, (data.Length / 0x10 - 1 - i) * 0x10, 0x10);
                temp2.CopyTo(temp, 0);
            }

            return output;
        }

        private static byte[] MemeCryptoAESDecrypt(byte[] key, byte[] data)
        {
            var temp = new byte[0x10];
            var subkey = new byte[0x10];
            var output = new byte[data.Length];
            for (var i = 0; i < data.Length / 0x10; i++) // Reverse Phase 2
            {
                var curblock = new byte[0x10];
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
            for (var ofs = 0; ofs < 0x10; ofs += 2) // Imperfect ROL implementation
            {
                byte b1 = temp[ofs + 0], b2 = temp[ofs + 1];
                subkey[ofs + 0] = (byte)(2 * b1 + (b2 >> 7));
                subkey[ofs + 1] = (byte)(2 * b2);
                if (ofs + 2 < temp.Length)
                    subkey[ofs + 1] += (byte)(temp[ofs + 2] >> 7);
            }
            if ((temp[0] & 0x80) != 0)
                subkey[0xF] ^= 0x87;

            for (var i = 0; i < data.Length / 0x10; i++)
            {
                var curblock = new byte[0x10];
                Array.Copy(output, 0x10 * i, curblock, 0, 0x10);
                Array.Copy(curblock.Xor(subkey), 0, output, 0x10 * i, 0x10);
            }

            // Now we have Phase1Encrypt(buf).
            temp = new byte[0x10]; // Clear to all zero
            for (var i = 0; i < data.Length / 0x10; i++) // Phase 1: CBC Encryption.
            {
                var curblock = new byte[0x10];
                Array.Copy(output, i * 0x10, curblock, 0, 0x10);
                AESECBDecrypt(key, curblock).Xor(temp).CopyTo(output, i * 0x10);
                curblock.CopyTo(temp, 0);
            }

            return output;
        }

        private static byte[] RSADecrypt(byte[] data)
        {
            var N = new BigInteger(MemeKeys[3].N);
            var D = new BigInteger(MemeKeys[3].D);
            var M = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());

            var sig = BigInteger.ModPow(M, D, N).ToByteArray().Reverse().ToArray();
            if (sig.Length < 0x60)
                sig = new byte[0x60 - sig.Length].Concat(sig).ToArray();
            else if (sig.Length > 0x60)
                sig = sig.Skip(sig.Length - 0x60).ToArray();
            return sig;
        }

        private static byte[] RSAEncrypt(byte[] data, int index)
        {
            var N = new BigInteger(MemeKeys[index].N);
            var E = new BigInteger(MemeKeys[index].E);
            var M = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());

            var sig = BigInteger.ModPow(M, E, N).ToByteArray().Reverse().ToArray();
            if (sig.Length < 0x60)
                sig = new byte[0x60 - sig.Length].Concat(sig).ToArray();
            else if (sig.Length > 0x60)
                sig = sig.Skip(sig.Length - 0x60).ToArray();
            return sig;
        }

        private static byte[] ReverseCrypt(byte[] input, int meme_ofs, int memeindex)
        {
            var output = (byte[])input.Clone();

            var memekey = MemeKeys[memeindex];

            using (var sha1 = Util.GetSHA1Provider())
            {
                var enc = new byte[0x60];
                Array.Copy(input, meme_ofs, enc, 0, 0x60);

                var keybuf = new byte[memekey.DER.Length + meme_ofs];
                Array.Copy(memekey.DER, keybuf, memekey.DER.Length);
                if (meme_ofs > 0)
                    Array.Copy(input, 0, keybuf, memekey.DER.Length, meme_ofs);
                var key = sha1.ComputeHash(keybuf).Take(0x10).ToArray();

                var RSA = RSAEncrypt(enc, memeindex);
                MemeCryptoAESDecrypt(key, RSA).CopyTo(output, meme_ofs);
                if (sha1.ComputeHash(output, 0, meme_ofs + 0x58).Take(0x8).SequenceEqual(output.Skip(meme_ofs + 0x58).Take(0x8)))
                {
                    return output;
                }

                RSA[0] |= 0x80;
                MemeCryptoAESDecrypt(key, RSA).CopyTo(output, meme_ofs);
                if (sha1.ComputeHash(output, 0, meme_ofs + 0x58).Take(0x8).SequenceEqual(output.Skip(meme_ofs + 0x58).Take(0x8)))
                {
                    return output;
                }
            }
            return null;
        }

        public static byte[] VerifyMemeData(byte[] input)
        {
            if (input.Length < 0x60)
                throw new ArgumentException("Invalid Memecrypto decryption byte[]!");
            var memeindex = 3;
            var meme_ofs = input.Length - 0x60;

            for (var i = input.Length - 8; i >= 0; i--)
            {
                if (BitConverter.ToUInt32(input, i) != 0x454B4F50 ||
                    BitConverter.ToUInt32(input, i + 4) >= MemeKeys.Length) continue;

                meme_ofs = i - 0x60;
                memeindex = BitConverter.ToInt32(input, i + 4);
                break;
            }

            if (meme_ofs < 0)
                return null;

            // Standard Format.
            var meme_sig = ReverseCrypt(input, meme_ofs, memeindex) ?? ReverseCrypt(input, meme_ofs, 3);

            if (meme_ofs >= 2 && meme_sig == null) // Pokedex QR Edge case
                meme_sig = ReverseCrypt(input, meme_ofs - 2, memeindex) ?? ReverseCrypt(input, meme_ofs - 2, 3);

            return meme_sig;
        }

        public static byte[] SignMemeData(byte[] input)
        {
            if (input.Length < 0x60)
                throw new ArgumentException("Bad Meme input!");
            const int memeindex = 3;
            using (var sha1 = Util.GetSHA1Provider())
            {
                var key = sha1.ComputeHash(MemeKeys[memeindex].DER.Concat(input.Take(input.Length - 0x60)).ToArray()).Take(0x10).ToArray();

                var output = (byte[])input.Clone();
                Array.Copy(sha1.ComputeHash(input, 0, input.Length - 8), 0, output, output.Length - 8, 8);
                var MemeCrypted = MemeCryptoAESEncrypt(key, output.Skip(output.Length - 0x60).ToArray());
                MemeCrypted[0] &= 0x7F;
                var RSA = RSADecrypt(MemeCrypted);
                RSA.CopyTo(output, output.Length - 0x60);
                return output;
            }
        }

        /// <summary>
        ///     Resigns save data.
        /// </summary>
        /// <param name="sav7">The save data to resign.</param>
        /// <param name="throwIfUnsupported">
        ///     If true, throw an <see cref="InvalidOperationException" /> if MemeCrypto is
        ///     unsupported.  If false, calling this function will have no effect.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the current platform has FIPS mode enabled on a platform that
        ///     does not support the required crypto service providers.
        /// </exception>
        /// <returns>The resigned save data.</returns>
        public static byte[] Resign(byte[] sav7, bool throwIfUnsupported = true)
        {
            if (sav7 == null || sav7.Length != 0x6BE00)
                return null;

            try
            {
                var outSav = (byte[])sav7.Clone();

                using (var sha256 = Util.GetSHA256Provider())
                {
                    var CurSig = new byte[0x80];
                    Array.Copy(sav7, 0x6BB00, CurSig, 0, 0x80);

                    var ChecksumTable = new byte[0x140];
                    Array.Copy(sav7, 0x6BC00, ChecksumTable, 0, 0x140);

                    var newSig = new byte[0x80];
                    sha256.ComputeHash(ChecksumTable).CopyTo(newSig, 0);
                    var memeSig = VerifyMemeData(CurSig);
                    if (memeSig != null)
                        Array.Copy(memeSig, 0x20, newSig, 0x20, 0x60);

                    SignMemeData(newSig).CopyTo(outSav, 0x6BB00);
                }
                return outSav;
            }
            catch (InvalidOperationException)
            {
                if (throwIfUnsupported)
                {
                    throw;
                }
                return (byte[])sav7.Clone();
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

        #region Meme Key Data

        private static readonly byte[] DER_0 = "307C300D06092A864886F70D0101010500036B003068026100B3D68C9B1090F6B1B88ECFA9E2F60E9C62C3033B5B64282F262CD393B433D97BD3DB7EBA470B1A77A3DB3C18A1E7616972229BDAD54FB02A19546C65FA4773AABE9B8C926707E7B7DDE4C867C01C0802985E438656168A4430F3F3B9662D7D010203010001".ToByteArray();
        private static readonly byte[] DER_1 = "307C300D06092A864886F70D0101010500036B003068026100C10F4097FD3C781A8FDE101EF3B2F091F82BEE4742324B9206C581766EAF2FBB42C7D60D749B999C529B0E22AD05E0C880231219AD473114EC454380A92898D7A8B54D9432584897D6AFE4860235126190A328DD6525D97B9058D98640B0FA050203010001".ToByteArray();
        private static readonly byte[] DER_2 = "307C300D06092A864886F70D0101010500036B003068026100C3C8D89F55D6A236A115C77594D4B318F0A0A0E3252CC0D6345EB9E33A43A5A56DC9D10B7B59C135396159EC4D01DEBC5FB3A4CAE47853E205FE08982DFCC0C39F0557449F97D41FED13B886AEBEEA918F4767E8FBE0494FFF6F6EE3508E3A3F0203010001".ToByteArray();
        private static readonly byte[] DER_3 = "307C300D06092A864886F70D0101010500036B003068026100B61E192091F90A8F76A6EAAA9A3CE58C863F39AE253F037816F5975854E07A9A456601E7C94C29759FE155C064EDDFA111443F81EF1A428CF6CD32F9DAC9D48E94CFB3F690120E8E6B9111ADDAF11E7C96208C37C0143FF2BF3D7E831141A9730203010001".ToByteArray();
        private static readonly byte[] DER_4 = "307C300D06092A864886F70D0101010500036B003068026100A0F2AC80B408E2E4D58916A1C706BEE7A24758A62CE9B50AF1B31409DFCB382E885AA8BB8C0E4AD1BCF6FF64FB3037757D2BEA10E4FE9007C850FFDCF70D2AFAA4C53FAFE38A9917D467862F50FE375927ECFEF433E61BF817A645FA5665D9CF0203010001".ToByteArray();
        private static readonly byte[] DER_5 = "307C300D06092A864886F70D0101010500036B003068026100D046F2872868A5089205B226DE13D86DA552646AC152C84615BE8E0A5897C3EA45871028F451860EA226D53B68DDD5A77D1AD82FAF857EA52CF7933112EEC367A06C0761E580D3D70B6B9C837BAA3F16D1FF7AA20D87A2A5E2BCC6E383BF12D50203010001".ToByteArray();
        private static readonly byte[] DER_6 = "307C300D06092A864886F70D0101010500036B003068026100D379919001D7FF40AC59DF475CF6C6368B1958DD4E870DFD1CE11218D5EA9D88DD7AD530E2806B0B092C02E25DB092518908EDA574A0968D49B0503954B24284FA75445A074CE6E1ABCEC8FD01DAA0D21A0DD97B417BC3E54BEB7253FC06D3F30203010001".ToByteArray();
        private static readonly byte[] DER_7 = "307C300D06092A864886F70D0101010500036B003068026100B751CB7D282625F2961A7138650ABE1A6AA80D69548BA3AE9DFF065B2805EB3675D960C62096C2835B1DF1C290FC19411944AFDF3458E3B1BC81A98C3F3E95D0EE0C20A0259E614399404354D90F0C69111A4E525F425FBB31A38B8C558F23730203010001".ToByteArray();
        private static readonly byte[] DER_8 = "307C300D06092A864886F70D0101010500036B003068026100B328FE4CC41627882B04FBA0A396A15285A8564B6112C1203048766D827E8E4E5655D44B266B2836575AE68C8301632A3E58B1F4362131E97B0AA0AFC38F2F7690CBD4F3F4652072BFD8E9421D2BEEF177873CD7D08B6C0D1022109CA3ED5B630203010001".ToByteArray();
        private static readonly byte[] DER_9 = "307C300D06092A864886F70D0101010500036B003068026100C4B32FD1161CC30D04BD569F409E878AA2815C91DD009A5AE8BFDAEA7D116BF24966BF10FCC0014B258DFEF6614E55FB6DAB2357CD6DF5B63A5F059F724469C0178D83F88F45048982EAE7A7CC249F84667FC393684DA5EFE1856EB10027D1D70203010001".ToByteArray();
        private static readonly byte[] DER_A = "307C300D06092A864886F70D0101010500036B003068026100C5B75401E83352A64EEC8916C4206F17EC338A24A6F7FD515260696D7228496ABC1423E1FF30514149FC199720E95E682539892E510B239A8C7A413DE4EEE74594F073815E9B434711F6807E8B9E7C10C281F89CF3B1C14E3F0ADF83A2805F090203010001".ToByteArray();
        private static readonly byte[] DER_B = "307C300D06092A864886F70D0101010500036B003068026100AC36B88D00C399C660B4846287FFC7F9DF5C07487EAAE3CD4EFD0029D3B86ED3658AD7DEE4C7F5DA25F9F6008885F343122274994CAB647776F0ADCFBA1E0ECEC8BF57CAAB8488BDD59A55195A0167C7D2C4A9CF679D0EFF4A62B5C8568E09770203010001".ToByteArray();
        private static readonly byte[] DER_C = "307C300D06092A864886F70D0101010500036B003068026100CAC0514D4B6A3F70771C461B01BDE3B6D47A0ADA078074DDA50703D8CC28089379DA64FB3A34AD3435D24F7331383BDADC4877662EFB555DA2077619B70AB0342EBE6EE888EBF3CF4B7E8BCCA95C61E993BDD6104C10D11115DC84178A5894350203010001".ToByteArray();
        private static readonly byte[] DER_D = "307C300D06092A864886F70D0101010500036B003068026100B906466740F5A9428DA84B418C7FA6146F7E24C783373D671F9214B40948A4A317C1A4460111B45D2DADD093815401573E52F0178890D35CBD95712EFAAE0D20AD47187648775CD9569431B1FC3C784113E3A48436D30B2CD162218D6781F5ED0203010001".ToByteArray();

        private static readonly byte[] D_3 = "00775455668FFF3CBA3026C2D0B26B8085895958341157AEB03B6B0495EE57803E2186EB6CB2EB62A71DF18A3C9C6579077670961B3A6102DABE5A194AB58C3250AED597FC78978A326DB1D7B28DCCCB2A3E014EDBD397AD33B8F28CD525054251".ToByteArray();

        private static readonly MemeKey[] MemeKeys =
        {
            new MemeKey(DER_0),      // Friendly Competition QRs
            new MemeKey(DER_1),      // Live Competition QRs
            new MemeKey(DER_2),      // Rental Team QRs
            new MemeKey(DER_3, D_3), // Save files/Pokedex QRs
            new MemeKey(DER_4),      // Ga-Ole QRs
            new MemeKey(DER_5),      // Magearna QRs
            new MemeKey(DER_6),      // Moncolle GET QRs
            new MemeKey(DER_7),      // Unknown (7)
            new MemeKey(DER_8),      // Unknown (8)
            new MemeKey(DER_9),      // Unknown (9)
            new MemeKey(DER_A),      // Unknown (10)
            new MemeKey(DER_B),      // Unknown (11)
            new MemeKey(DER_C),      // Unknown (12)
            new MemeKey(DER_D)       // Unknown (13)
        };
        #endregion
    }

    public class MemeKey
    {
        /// <summary> Private Exponent </summary>
        public readonly byte[] D;
        /// <summary> Distinguished Encoding Rules </summary>
        public readonly byte[] DER;
        /// <summary> Public Exponent </summary>
        public readonly byte[] E;
        /// <summary> Modulus </summary>
        public readonly byte[] N;

        public MemeKey(byte[] der, byte[] d = null)
        {
            DER = der;
            D = d;
            N = new byte[0x61];
            E = new byte[0x3];
            Array.Copy(der, 0x18, N, 0, 0x61);
            Array.Copy(der, 0x7B, E, 0, 3);
            Array.Reverse(N);
            Array.Reverse(E);
            if (D != null)
                Array.Reverse(D);
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
            var x = new byte[b1.Length];
            for (var i = 0; i < b1.Length; i++)
                x[i] = (byte)(b1[i] ^ b2[i]);
            return x;
        }

        public static string ToHexString(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
    }
}